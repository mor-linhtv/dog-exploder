using Dog_Exploder.Forms;

namespace Dog_Exploder.Services;

internal sealed class NetworkAlertCoordinator : IDisposable
{
    private readonly Form _ownerForm;
    private readonly NetworkMonitorService _monitor;

    // State (UI-thread-only, no locking needed).
    private bool? _lastOnline;                    // null until first check
    private bool _offlineDialogShownThisRun;      // true after modal shown, reset on recovery toast
    private NetworkOfflineDialog? _offlineDialog; // tracked so recovery can auto-close it

    private bool _disposed;

    public NetworkAlertCoordinator(Form ownerForm, NetworkMonitorService monitor)
    {
        _ownerForm = ownerForm;
        _monitor = monitor;
        _monitor.StatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(object? sender, NetworkStatusChangedEventArgs e)
    {
        if (_disposed) return;
        if (!_ownerForm.IsHandleCreated || _ownerForm.IsDisposed) return;
        try
        {
            _ownerForm.BeginInvoke(new Action(() => ApplyState(e)));
        }
        catch (InvalidOperationException)
        {
            // Handle could be torn down between the check and BeginInvoke; ignore.
        }
    }

    private void ApplyState(NetworkStatusChangedEventArgs e)
    {
        if (_disposed || _ownerForm.IsDisposed) return;

        bool current = e.IsOnline;

        if (_lastOnline == null)
        {
            // Initial check — record state but don't popup. Starting offline is not
            // "lost during use".
            _lastOnline = current;
            return;
        }

        if (_lastOnline == true && !current)
        {
            // Online -> Offline: show the modal once per offline run.
            // Mutate state BEFORE ShowDialog so any re-entrant ApplyState calls during
            // ShowDialog's nested message loop see consistent state.
            _offlineDialogShownThisRun = true;
            _lastOnline = false;

            using var dialog = new NetworkOfflineDialog();
            dialog.SetDetectedAt(e.CheckedAt);
            dialog.FormClosed += (s, args) => _offlineDialog = null;
            _offlineDialog = dialog;
            dialog.ShowDialog(_ownerForm);
            // After ShowDialog returns: dialog is closed and `using` disposes it.
            // No code after this — a re-entrant recovery ApplyState may have already
            // updated _offlineDialogShownThisRun / _lastOnline.
        }
        else if (_lastOnline == false && current)
        {
            // Offline -> Online: close modal (if still open), show recovery toast
            // (if the user was notified in this offline run).
            _offlineDialog?.Close();

            if (_offlineDialogShownThisRun)
            {
                var toast = new NetworkRecoveryToast();
                toast.Show(_ownerForm);
                // Toast shown via Show() auto-disposes when it closes.
            }

            _offlineDialogShownThisRun = false;
            _lastOnline = true;
        }
        // false->false or true->true: no-op.
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _monitor.StatusChanged -= OnStatusChanged;
        if (_offlineDialog != null && !_offlineDialog.IsDisposed)
        {
            try { _offlineDialog.Close(); } catch { /* form may already be tearing down */ }
        }
    }
}
