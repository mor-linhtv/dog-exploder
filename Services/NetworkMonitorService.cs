using System.Net.Http;

namespace Dog_Exploder.Services;

internal sealed class NetworkStatusChangedEventArgs : EventArgs
{
    public bool IsOnline { get; }
    public DateTime CheckedAt { get; }
    public string? Error { get; }

    public NetworkStatusChangedEventArgs(bool isOnline, DateTime checkedAt, string? error)
    {
        IsOnline = isOnline;
        CheckedAt = checkedAt;
        Error = error;
    }
}

internal sealed class NetworkMonitorService : IDisposable
{
    private static readonly TimeSpan PollInterval    = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan RequestTimeout  = TimeSpan.FromSeconds(5);
    private const string ProbeUrl = "https://dogapi.dog/api/v2/breeds?page[number]=1";

    private readonly HttpClient _http;
    private readonly System.Threading.Timer _timer;
    private readonly object _gate = new();
    private bool _started;
    private bool _disposed;
    private int _checkInFlight; // 0 = idle, 1 = running

    public bool IsOnline { get; private set; }
    public DateTime LastCheckedAt { get; private set; }
    public string? LastError { get; private set; }

    public event EventHandler<NetworkStatusChangedEventArgs>? StatusChanged;

    public NetworkMonitorService()
    {
        _http = new HttpClient { Timeout = RequestTimeout };
        _timer = new System.Threading.Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start()
    {
        lock (_gate)
        {
            if (_disposed || _started) return;
            _started = true;
            _timer.Change(TimeSpan.Zero, PollInterval);
        }
    }

    public void Stop()
    {
        lock (_gate)
        {
            if (!_started) return;
            _started = false;
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    private void OnTimerTick(object? _)
    {
        if (System.Threading.Interlocked.Exchange(ref _checkInFlight, 1) == 1) return;
        _ = RunCheckAsync();
    }

    private async Task RunCheckAsync()
    {
        bool online = false;
        string? error = null;
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Head, ProbeUrl);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            online = (int)resp.StatusCode < 500;
            if (!online) error = $"HTTP {(int)resp.StatusCode}";
        }
        catch (Exception ex)
        {
            online = false;
            error = ex.Message;
        }
        finally
        {
            System.Threading.Interlocked.Exchange(ref _checkInFlight, 0);
        }

        var now = DateTime.Now;
        IsOnline = online;
        LastCheckedAt = now;
        LastError = error;

        try
        {
            StatusChanged?.Invoke(this, new NetworkStatusChangedEventArgs(online, now, error));
        }
        catch
        {
            // Never let a handler exception crash the timer.
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _started = false;
        }
        _timer.Dispose();
        _http.Dispose();
    }
}
