using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarboniteXmlParser.XmlEntities;

namespace CarboniteXmlParser.Test
{
   [TestClass]
   public class XmlUtilitiesTest
   {
      [TestMethod]
      public void TestFormatDate()
      {
         Assert.AreEqual("Jun 19, 2013 6:11:29 PM", XmlUtilities.FormatDate(new DateTime(2013, 6, 19, 18, 11, 29)));
      }

      [TestMethod]
      public void TestDateParse()
      {
         ulong epoch = 1371662647376L;
         DateTime date = XmlUtilities.ParseTimestamp(epoch);
         Console.WriteLine(date.ToString());
         Assert.AreEqual(new DateTime(2013, 6, 19, 17, 24, 7, 376), date);
      }
   }
}
