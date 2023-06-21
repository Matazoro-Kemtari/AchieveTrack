﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.AchieveTrackReader.Tests
{
    [TestClass()]
    public class WorkRecordTests
    {
        [TestMethod()]
        public void 異常系_作業番号にnullを渡したとき例外を返すこと()
        {
            // given
            // when
            void target() => _ = WorkRecord.Create(DateTime.Now,
                                                   1,
                                                   null!,
                                                   TestManHourFactory.Create());

            // then
            var ex = Assert.ThrowsException<ArgumentNullException>(() => target());
        }
    
        [TestMethod()]
        public void 異常系_工数にnullを渡したとき例外を返すこと()
        {
            // given
            // when
            void target() => _ = WorkRecord.Create(DateTime.Now,
                                                   1,
                                                   TestWorkingNumberFactory.Create(),
                                                   null!);

            // then
            var ex = Assert.ThrowsException<ArgumentNullException>(() => target());
        }
    }
}