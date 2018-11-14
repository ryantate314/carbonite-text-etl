using MessageImport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteXmlParser.Test
{
    [TestClass]
    public class BackupUtilsTests
    {
        [TestMethod]
        public void PhoneNumberTest()
        {
            Assert.AreEqual("4235551234", BackupUtils.SanitizePhoneNumber("+1 423-555-1234"));
            Assert.AreEqual("4235551234", BackupUtils.SanitizePhoneNumber("+14235551234"));
            Assert.AreEqual("4235551234", BackupUtils.SanitizePhoneNumber("423-555-1234"));
            Assert.AreEqual("4235551234", BackupUtils.SanitizePhoneNumber("4235551234"));
            Assert.AreEqual("4235551234", BackupUtils.SanitizePhoneNumber("(423) 555-1234"));
        }
    }
}
