using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageImport
{
    static class BackupUtils
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BackupUtils));

        public static string SanitizePhoneNumber(string phoneNumber)
        {
            string sanitizedPhoneNumber = null;
            if (!String.IsNullOrEmpty(phoneNumber))
            {
                Regex reg = new Regex(@"(\+1)?\s?\(?([0-9]{3})\)?[- ]?([0-9]{3})-?([0-9]{4})");
                if (reg.IsMatch(phoneNumber))
                {
                    var result = reg.Match(phoneNumber);
                    sanitizedPhoneNumber = String.Format("{0}{1}{2}", result.Groups[2], result.Groups[3], result.Groups[4]);
                }
                else
                {
                    logger.Warn("Unexpected phone number format: " + phoneNumber);
                }
            }
            return sanitizedPhoneNumber;
        }
    }
}
