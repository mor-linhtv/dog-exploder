using ClosedXML.Excel;
using Dog_Exploder.Models;

namespace Dog_Exploder.Services;

internal static class ExcelExportService
{
    private const string SheetName = "DeviceStatus";
    private static readonly string[] Headers = { "Ngày", "Giờ", "Thiết bị", "Loại", "Trạng thái", "Chi tiết" };

    public static void AppendDeviceStatus(string path, DeviceInfo device)
    {
        using var book = File.Exists(path) ? new XLWorkbook(path) : new XLWorkbook();

        if (!book.Worksheets.TryGetWorksheet(SheetName, out var sheet))
        {
            sheet = book.Worksheets.Add(SheetName);
            WriteHeader(sheet);
        }

        int row = (sheet.LastRowUsed()?.RowNumber() ?? 1) + 1;
        sheet.Cell(row, 1).Value = device.CheckedAt.ToString("yyyy-MM-dd");
        sheet.Cell(row, 2).Value = device.CheckedAt.ToString("HH:mm:ss");
        sheet.Cell(row, 3).Value = device.Name;
        sheet.Cell(row, 4).Value = device.Category;
        sheet.Cell(row, 5).Value = device.Status.ToString();
        sheet.Cell(row, 6).Value = device.Detail;

        sheet.Columns().AdjustToContents();
        book.SaveAs(path);
    }

    private static void WriteHeader(IXLWorksheet sheet)
    {
        for (int c = 0; c < Headers.Length; c++)
            sheet.Cell(1, c + 1).Value = Headers[c];
        var header = sheet.Range(1, 1, 1, Headers.Length);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromArgb(0xF3, 0xF3, 0xF3);
        header.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
    }
}
