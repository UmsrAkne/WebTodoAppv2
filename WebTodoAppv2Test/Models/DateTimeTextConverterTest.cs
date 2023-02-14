using System;
using NUnit.Framework;
using WebTodoAppv2.Models;

namespace WebTodoAppv2Test.Models
{
    public class DateTimeTextConverterTest
    {
        [Test]
        public void ConvertDateTimeTextTest()
        {
            var today = new DateTime(2023, 2, 14, 11, 0, 0);

            Assert.AreEqual(new DateTime(2023, 2, 14, 0, 0, 0), DateTimeTextConverter.ConvertDateTimeText("0d", today));
            Assert.AreEqual(new DateTime(2023, 2, 15, 0, 0, 0), DateTimeTextConverter.ConvertDateTimeText("1d", today));

            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 0), DateTimeTextConverter.ConvertDateTimeText("0h", today));
            Assert.AreEqual(new DateTime(2023, 2, 14, 12, 0, 0), DateTimeTextConverter.ConvertDateTimeText("1h", today));

            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 0), DateTimeTextConverter.ConvertDateTimeText("0m", today));
            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 30, 0), DateTimeTextConverter.ConvertDateTimeText("30m", today));

            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 0), DateTimeTextConverter.ConvertDateTimeText("0s", today));
            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 40), DateTimeTextConverter.ConvertDateTimeText("40s", today));
        }

        [Test]
        public void 入力値にスペースが紛れていた場合の対応()
        {
            var today = new DateTime(2023, 2, 14, 11, 0, 0);

            Assert.AreEqual(new DateTime(2023, 2, 14, 0, 0, 0), DateTimeTextConverter.ConvertDateTimeText(" 0d ", today));
            Assert.AreEqual(new DateTime(2023, 2, 15, 0, 0, 0), DateTimeTextConverter.ConvertDateTimeText("1 d", today));

            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 0), DateTimeTextConverter.ConvertDateTimeText(" 0h ", today));
            Assert.AreEqual(new DateTime(2023, 2, 14, 12, 0, 0), DateTimeTextConverter.ConvertDateTimeText("1 h", today));

            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 0), DateTimeTextConverter.ConvertDateTimeText(" 0 m ", today));
            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 30, 0), DateTimeTextConverter.ConvertDateTimeText("3  0m", today));

            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 0), DateTimeTextConverter.ConvertDateTimeText("0 s", today));
            Assert.AreEqual(new DateTime(2023, 2, 14, 11, 0, 40), DateTimeTextConverter.ConvertDateTimeText(" 40s ", today));
        }

        [Test]
        public void 不正な値を入れた場合のテスト()
        {
            var today = new DateTime(2023, 2, 14, 11, 0, 0);

            Assert.AreEqual(new DateTime(), DateTimeTextConverter.ConvertDateTimeText("xd", today));
        }
    }
}