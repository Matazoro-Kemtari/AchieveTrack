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

                            if (!row.Cell(columnNumbers.AchievementClassificationColumnNumber).TryGetValue(out string achievementClassification)
                                || achievementClassification == string.Empty)
                                throw new DomainException(
                                    $"作業名が取得できませんでした 行: {row.RowNumber()}");

                            var note = row.Cell(columnNumbers.NoteColumnNumber).GetString();

                            if (!row.Cell(columnNumbers.ManHourColumnNumber).TryGetValue(out decimal manHour))
                                throw new DomainException(
                                    $"工数が取得できませんでした 行: {row.RowNumber()}");

                            return WorkRecord.Create(workingDate,
                                                     (uint)employeeNumber,
                                                     employeeName,
                                                     WorkingNumber.Create(workingNumber),
                                                     jigCode,
                                                     achievementClassification,
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

    [Logging]
    private static ColumnNumbers SearchSubjects(IXLRangeRow subjectRow)
    {
        const string workingDateSubject = "日付";
        const string employeeNumberSubject = "社員番号";
        const string employeeNameSubject = "氏名";
        const string workingNumberSubject = "作業番号";
        const string jigCodeNumberSubject = "コード";
        const string AchievementClassificationSubject = "作業名";
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
                WorkingNumberColumnNumber: subjectRow.Cells()
                                                     .First(cell => cell.GetString() == workingNumberSubject).Address.ColumnNumber,
                JigCodeNumberColumnNumber: subjectRow.Cells()
                                                     .First(cell => cell.GetString() == jigCodeNumberSubject).Address.ColumnNumber,
                AchievementClassificationColumnNumber: subjectRow.Cells()
                                                                 .First(cell => cell.GetString() == AchievementClassificationSubject).Address.ColumnNumber,
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
        int WorkingNumberColumnNumber,
        int JigCodeNumberColumnNumber,
        int AchievementClassificationColumnNumber,
        int NoteColumnNumber,
        int ManHourColumnNumber);
}