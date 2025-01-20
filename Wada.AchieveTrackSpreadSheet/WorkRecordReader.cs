using ClosedXML.Excel;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordReader;
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

            var tasks = usedRage.Rows()
                                // ヘッダ行を飛ばす
                                .Skip(1)
                                .Select(async row => await Task.Run(
                                    () => MakeWorkRecord(row, columnNumbers)));

            return await Task.WhenAll(tasks);
        }
        catch (InvalidOperationException ex)
        {
            throw new DomainException("ワークシートが見つかりません", ex);
        }
        catch (WorkOrderIdException ex)
        {
            throw new DomainException(ex.Message, ex);
        }
        catch (DomainException)
        {
            throw;
        }
    }

    private static WorkRecord MakeWorkRecord(IXLRangeRow row, ColumnNumbers columnNumbers)
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

        if (!row.Cell(columnNumbers.WorkOrderIdColumnNumber).TryGetValue(out string workOrderId)
            || workOrderId == string.Empty)
            throw new DomainException(
                $"作業番号が取得できませんでした 行: {row.RowNumber()}");

        var jigCode = row.Cell(columnNumbers.JigCodeNumberColumnNumber).GetString();

        if (!row.Cell(columnNumbers.ProcessFlowColumnNumber).TryGetValue(out string processFlow)
            || processFlow == string.Empty)
            throw new DomainException(
                $"作業名が取得できませんでした 行: {row.RowNumber()}");

        var note = row.Cell(columnNumbers.NoteColumnNumber).GetString();

        if (!row.Cell(columnNumbers.ManHourColumnNumber).TryGetValue(out decimal manHour))
            throw new DomainException(
                $"工数が取得できませんでした 行: {row.RowNumber()}");

        return WorkRecord.Create(workingDate,
                                    (uint)employeeNumber,
                                    employeeName,
                                    WorkOrderId.Create(workOrderId),
                                    jigCode,
                                    processFlow,
                                    note,
                                    ManHour.Create(manHour));
    }

    [Logging]
    private static ColumnNumbers SearchSubjects(IXLRangeRow subjectRow)
    {
        const string workingDateSubject = "日付";
        const string employeeNumberSubject = "社員番号";
        const string employeeNameSubject = "氏名";
        const string workOrderIdSubject = "作業番号";
        const string jigCodeNumberSubject = "コード";
        const string ProcessFlowSubject = "作業名";
        const string noteSubject = "特記事項";
        const string manHourSubject = "工数";

        try
        {
            return new ColumnNumbers(
                WorkingDateColumnNumber: subjectRow.Cells()
                                                   .First(cell => cell.GetString() == workingDateSubject).Address.ColumnNumber,
                EmployeeNumberColumnNumber: subjectRow.Cells()
                                                      .First(cell => cell.GetString() == employeeNumberSubject).Address.ColumnNumber,
                EmployeeNameColumnNumber: subjectRow.Cells()
                                                    .First(cell => cell.GetString() == employeeNameSubject).Address.ColumnNumber,
                WorkOrderIdColumnNumber: subjectRow.Cells()
                                                     .First(cell => cell.GetString() == workOrderIdSubject).Address.ColumnNumber,
                JigCodeNumberColumnNumber: subjectRow.Cells()
                                                     .First(cell => cell.GetString() == jigCodeNumberSubject).Address.ColumnNumber,
                ProcessFlowColumnNumber: subjectRow.Cells()
                                                   .First(cell => cell.GetString() == ProcessFlowSubject).Address.ColumnNumber,
                NoteColumnNumber: subjectRow.Cells()
                                            .First(cell => cell.GetString() == noteSubject).Address.ColumnNumber,
                ManHourColumnNumber: subjectRow.Cells()
                                               .First(cell => cell.GetString() == manHourSubject).Address.ColumnNumber);
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
        int WorkOrderIdColumnNumber,
        int JigCodeNumberColumnNumber,
        int ProcessFlowColumnNumber,
        int NoteColumnNumber,
        int ManHourColumnNumber);
}