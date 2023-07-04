using ClosedXML.Excel;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AOP.Logging;

namespace Wada.AchieveTrackSpreadSheet;

public class WorkRecordReader : IWorkRecordReader
{
    [Logging]
    public async Task<IEnumerable<WorkRecord>> ReadWorkRecordsAsync(Stream stream)
    {
        using var xlBook = new XLWorkbook(stream);
        try
        {
            IXLWorksheet targetSheet = xlBook.Worksheets.First();
            var usedRage = targetSheet.RangeUsed();

            return await Task.WhenAll(
                usedRage.Rows()
                        // ヘッダ行を飛ばす
                        .Skip(1)
                        .Select(async row => await Task.Run(() =>
                        {
                            const string workingDateLetter = "A";
                            const string employeeNumberLetter = "B";
                            const string employeeNameLetter = "C";
                            const string workingNumberLetter = "E";
                            const string monHourLetter = "J";

                            if (!row.Cell(workingDateLetter).TryGetValue(out DateTime workingDate))
                                throw new DomainException(
                                    $"作業日が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(employeeNumberLetter).TryGetValue(out int employeeNumber)
                                || employeeNumber <= 0)
                                throw new DomainException(
                                    $"社員番号が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(employeeNameLetter).TryGetValue(out string employeeName)
                                || employeeName == string.Empty)
                                throw new DomainException(
                                    $"社員名が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(workingNumberLetter).TryGetValue(out string workingNumber)
                                || workingNumber == string.Empty)
                                throw new DomainException(
                                    $"作業番号が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(monHourLetter).TryGetValue(out decimal manHour))
                                throw new DomainException(
                                    $"工数が取得できませんでした 行: {row.RowNumber()}");

                            return WorkRecord.Create(workingDate, (uint)employeeNumber, employeeName, WorkingNumber.Create(workingNumber), ManHour.Create(manHour));
                        })));
        }
        catch (InvalidOperationException ex)
        {
            throw new DomainException("ワークシートが見つかりません", ex);
        }
        catch (Data.OrderManagement.Models.ValueObjects.WorkingNumberException ex)
        {
            throw new DomainException(ex.Message, ex);
        }
    }
}