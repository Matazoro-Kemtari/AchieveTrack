using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordReader;

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
            // idは除外して比較
            var expected = MakeTestDatas().Select(x => new
            {
                x.WorkingDate,
                x.EmployeeNumber,
                x.EmployeeName,
                x.WorkOrderId,
                x.JigCode,
                x.ManHour,
                x.Note,
            });
            CollectionAssert.AreEquivalent(
                expected.ToArray(),
                actual.Select(x => new
                {
                    x.WorkingDate,
                    x.EmployeeNumber,
                    x.EmployeeName,
                    x.WorkOrderId,
                    x.JigCode,
                    x.ManHour,
                    x.Note,
                }).ToArray());
        }

        private static IXLWorkbook MakeTestBook()
        {
            XLWorkbook workbook = new();
            var sht = workbook.AddWorksheet();
            // 1行目はヘッダ
            MakeTestWorkRecordHeader().Select((header, i) => (header, i))
                                      .ToList()
                                      .ForEach(x =>
                                      {
                                          sht.Cell(1, x.i + 1).SetValue(x.header);
                                      });

            // 2行目以降にデータ
            MakeTestDatas().Select((item, i) => (item, i))
                           .ToList()
                           .ForEach(x =>
            {
                sht.Cell(x.i + 2, "A").SetValue(x.item.WorkingDate);
                sht.Cell(x.i + 2, "B").SetValue(x.item.EmployeeNumber);
                sht.Cell(x.i + 2, "C").SetValue(x.item.EmployeeName);
                sht.Cell(x.i + 2, "E").SetValue(x.item.WorkOrderId.Value);
                sht.Cell(x.i + 2, "F").SetValue(x.item.JigCode);
                sht.Cell(x.i + 2, "G").SetValue(x.item.ProcessFlow);
                sht.Cell(x.i + 2, "H").SetValue(x.item.Note);
                sht.Cell(x.i + 2, "J").SetValue(x.item.ManHour.Value);
            });
            return workbook;
        }

        private static IEnumerable<WorkRecord> MakeTestDatas() => new List<WorkRecord>
        {
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: TestManHourFactory.Create(0.1m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: TestManHourFactory.Create(3.4m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: TestManHourFactory.Create(2.5m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,1), manHour: TestManHourFactory.Create(4.0m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,2), manHour: TestManHourFactory.Create(0.5m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,2), manHour: TestManHourFactory.Create(3.5m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,3), manHour: TestManHourFactory.Create(5.5m)),
            TestWorkRecordFactory.Create(workingDate: new DateTime(2023,4,3), manHour: TestManHourFactory.Create(2.0m)),
        };

        private static string[] MakeTestWorkRecordHeader() =>
        [
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
        ];

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("ABC")]
        [DataRow("漢字")]
        public async Task 異常系_作業日が取得できないとき例外を返すこと(dynamic testData)
        {
            // given
            using var workbook = MakeTestBook();
            var sht = workbook.Worksheets.First();
            const string CellAddressInRange = "A2";
            if (testData is null)
                sht.Cell(CellAddressInRange).Clear();
            else
                sht.Cell(CellAddressInRange).SetValue(testData);
            using var xlsStream = new MemoryStream();
            workbook.SaveAs(xlsStream);

            // when
            IWorkRecordReader workRecordReader = new WorkRecordReader();
            Task target() => workRecordReader.ReadWorkRecordsAsync(xlsStream!);

            // then
            var ex = await Assert.ThrowsExceptionAsync<DomainException>(target);
            var msg = "作業日が取得できませんでした 行: 2";
            Assert.AreEqual(msg, ex.Message);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow("ABC")]
        [DataRow("漢字")]
        public async Task 異常系_社員番号が取得できないとき例外を返すこと(dynamic testData)
        {
            // given
            using var workbook = MakeTestBook();
            var sht = workbook.Worksheets.First();
            const string CellAddressInRange = "B2";
            if (testData is null)
                sht.Cell(CellAddressInRange).Clear();
            else
                sht.Cell(CellAddressInRange).SetValue(testData);
            using var xlsStream = new MemoryStream();
            workbook.SaveAs(xlsStream);

            // when
            IWorkRecordReader workRecordReader = new WorkRecordReader();
            Task target() => workRecordReader.ReadWorkRecordsAsync(xlsStream!);

            // then
            var ex = await Assert.ThrowsExceptionAsync<DomainException>(target);
            var msg = "社員番号が取得できませんでした 行: 2";
            Assert.AreEqual(msg, ex.Message);
        }

        [DataTestMethod]
        [DataRow(null)]
        public async Task 異常系_社員名が取得できないとき例外を返すこと(dynamic testData)
        {
            // given
            using var workbook = MakeTestBook();
            var sht = workbook.Worksheets.First();
            const string CellAddressInRange = "C2";
            if (testData is null)
                sht.Cell(CellAddressInRange).Clear();
            else
                sht.Cell(CellAddressInRange).SetValue(testData);
            using var xlsStream = new MemoryStream();
            workbook.SaveAs(xlsStream);

            // when
            IWorkRecordReader workRecordReader = new WorkRecordReader();
            async Task target()
            {
                var hoge = await workRecordReader.ReadWorkRecordsAsync(xlsStream!);
                _ = hoge.ToList();
            }

            // then
            var ex = await Assert.ThrowsExceptionAsync<DomainException>(target);
            var msg = "社員名が取得できませんでした 行: 2";
            Assert.AreEqual(msg, ex.Message);
        }

        [DataTestMethod]
        [DataRow(null)]
        public async Task 異常系_作業番号が取得できないとき例外を返すこと(dynamic testData)
        {
            // given
            using var workbook = MakeTestBook();
            var sht = workbook.Worksheets.First();
            const string CellAddressInRange = "E2";
            if (testData is null)
                sht.Cell(CellAddressInRange).Clear();
            else
                sht.Cell(CellAddressInRange).SetValue(testData);
            using var xlsStream = new MemoryStream();
            workbook.SaveAs(xlsStream);

            // when
            IWorkRecordReader workRecordReader = new WorkRecordReader();
            async Task target()
            {
                var hoge = await workRecordReader.ReadWorkRecordsAsync(xlsStream!);
                _ = hoge.ToList();
            }

            // then
            var ex = await Assert.ThrowsExceptionAsync<DomainException>(target);
            var msg = "作業番号が取得できませんでした 行: 2";
            Assert.AreEqual(msg, ex.Message);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("ABC")]
        [DataRow("漢字")]
        public async Task 異常系_工数が取得できないとき例外を返すこと(dynamic testData)
        {
            // given
            using var workbook = MakeTestBook();
            var sht = workbook.Worksheets.First();
            const string CellAddressInRange = "J2";
            if (testData is null)
                sht.Cell(CellAddressInRange).Clear();
            else
                sht.Cell(CellAddressInRange).SetValue(testData);
            using var xlsStream = new MemoryStream();
            workbook.SaveAs(xlsStream);

            // when
            IWorkRecordReader workRecordReader = new WorkRecordReader();
            async Task target()
            {
                var hoge = await workRecordReader.ReadWorkRecordsAsync(xlsStream!);
                _ = hoge.ToList();
            }

            // then
            var ex = await Assert.ThrowsExceptionAsync<DomainException>(target);
            var msg = "工数が取得できませんでした 行: 2";
            Assert.AreEqual(msg, ex.Message);
        }
    }
}