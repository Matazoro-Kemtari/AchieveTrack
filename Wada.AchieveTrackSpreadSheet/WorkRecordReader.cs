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

            ColumnNumbers columnNumbers = SearchSubjects(usedRage.Rows().First());

            return await Task.WhenAll(
                usedRage.Rows()
                        // ヘッダ行を飛ばす
                        .Skip(1)
                        .Select(async row => await Task.Run(() =>
                        {
                            if (!row.Cell(columnNumbers.WorkingDateColumnNumber).TryGetValue(out DateTime workingDate))
                                throw new DomainException(
                                    $"作業日が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(columnNumbers.EmployeeNumberColumnNumber).TryGetValue(out int employeeNumber)
                                || employeeNumber <= 0)
                                throw new DomainException(
                                    $"社員番号が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(columnNumbers.EmployeeNameColumnNumber).TryGetValue(out string employeeName)
                                || employeeName == string.Empty)
                                throw new DomainException(
                                    $"社員名が取得できませんでした 行: {row.RowNumber()}");

                            if (!row.Cell(columnNumbers.WorkingNumberColumnNumber).TryGetValue(out string workingNumber)
                                || workingNumber == string.Empty)
                                throw new DomainException(
                                    $"作業番号が取得できませんでした 行: {row.RowNumber()}");

                            var jigCode = row.Cell(columnNumbers.JigCodeNumberColumnNumber).GetString();

                            var note = row.Cell(columnNumbers.NoteColumnNumber).GetString();

                            if (!row.Cell(columnNumbers.ManHourColumnNumber).TryGetValue(out decimal manHour))
                                throw new DomainException(
                                    $"工数が取得できませんでした 行: {row.RowNumber()}");

                            return WorkRecord.Create(workingDate,
                                                     (uint)employeeNumber,
                                                     employeeName,
                                                     WorkingNumber.Create(workingNumber),
                                                     jigCode,
                                                     note,
                                                     ManHour.Create(manHour));
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
        catch (DomainException)
        {
            throw;
        }
    }

    private static ColumnNumbers SearchSubjects(IXLRangeRow subjectRow)
    {
        const string workingDateSubject = "日付";
        const string employeeNumberSubject = "社員番号";
        const string employeeNameSubject = "氏名";
        const string workingNumberSubject = "作業番号";
        const string jigCodeNumberSubject = "コード";
        const string noteSubject = "特記事項";
        const string manHourSubject = "工数";

        try
        {
            return new ColumnNumbers(
                subjectRow.Cells().Where(cell => cell.GetString() == workingDateSubject).First().Address.ColumnNumber,
                subjectRow.Cells().Where(cell => cell.GetString() == employeeNumberSubject).First().Address.ColumnNumber,
                subjectRow.Cells().Where(cell => cell.GetString() == employeeNameSubject).First().Address.ColumnNumber,
                subjectRow.Cells().Where(cell => cell.GetString() == workingNumberSubject).First().Address.ColumnNumber,
                subjectRow.Cells().Where(cell => cell.GetString() == jigCodeNumberSubject).First().Address.ColumnNumber,
                subjectRow.Cells().Where(cell => cell.GetString() == noteSubject).First().Address.ColumnNumber,
                subjectRow.Cells().Where(cell => cell.GetString() == manHourSubject).First().Address.ColumnNumber);
        }
        catch (InvalidOperationException ex)
        {
            throw new DomainException("ヘッダが見つかりません", ex);
        }
    }

    private record struct ColumnNumbers(
        int WorkingDateColumnNumber,
        int EmployeeNumberColumnNumber,
        int EmployeeNameColumnNumber,
        int WorkingNumberColumnNumber,
        int JigCodeNumberColumnNumber,
        int NoteColumnNumber,
        int ManHourColumnNumber);
}