using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchieveTrackReader;

namespace Wada.AchieveTrackSpreadSheet.Tests
{
    [TestClass()]
    public class WorkRecordReaderTests
    {
        [TestMethod()]
        public async Task 正常系_日報が取得できること()
        {
            // given
            using var workbook = MakeTestBook();
            using var xlsStream = new MemoryStream();
            workbook.SaveAs(xlsStream);

            // when
            IWorkRecordReader workRecordReader = new WorkRecordReader();
            var actual = await workRecordReader.ReadWorkRecordsAsync(xlsStream);

            // then
            Assert.IsNotNull(actual);
            var expected = MakeTestDatas();
            CollectionAssert.AreEquivalent(expected.ToArray(), actual.ToArray());
        }

        private static IXLWorkbook MakeTestBook()
        {
            XLWorkbook workbook = new();
            var sht = workbook.AddWorksheet();
            MakeTestWorkRecordHeader().Select((header, i) => (header, i))
                                      .ToList()
                                      .ForEach(x =>
                                      {
                                          sht.Cell(1, x.i + 1).SetValue(x.header);
                                      });

            MakeTestDatas().Select((item, i) => (item, i))
                           .ToList()
                           .ForEach(x =>
            {
                sht.Cell(x.i + 2, "A").SetValue(x.item.WorkingDate);
                sht.Cell(x.i + 2, "B").SetValue(x.item.EmployeeNumber);
                sht.Cell(x.i + 2, "E").SetValue(x.item.WorkingNumber);
                sht.Cell(x.i + 2, "J").SetValue(x.item.ManHour);
            });
            return workbook;
        }

        private static IEnumerable<WorkRecord> MakeTestDatas() => new List<WorkRecord>
        {
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: 0.1m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: 3.4m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: 2.5m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: 4.0m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,2), manHour: 0.5m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,2), manHour: 3.5m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,3), manHour: 5.5m),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,3), manHour: 2.0m),
        };

        private static string[] MakeTestWorkRecordHeader() => new string[]
        {
            "日付",
            "社員番号",
            "氏名",
            "実働時間",
            "作業番号",
            "コード",
            "作業名",
            "特記事項",
            "目標工数",
            "工数",
            "ヘッダ",
            "番号",
            "部署",
            "機種名",
            "直間区分",
            "大分類",
            "中分類",
        };
    }
}