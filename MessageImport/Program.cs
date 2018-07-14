using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using CarboniteXmlParser;
using CarboniteXmlParser.Android;
using log4net;
using log4net.Config;

namespace MessageImport
{
   class Program
   {

      private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

      static void Main(string[] args)
      {

         XmlConfigurator.Configure();

         if (args.Length < 1)
         {
            PrintUsage();
            return;
         }

         string filename = args[0];

         logger.Debug("Reading file " + filename);

         if (!File.Exists(filename))
         {
            //Console.WriteLine("File not found.");
            logger.Error("File not found.");
            return;
         }

         Backup backup = new Backup(filename, System.Configuration.ConfigurationManager.AppSettings["media-directory"]);
         backup.Run();


         Console.WriteLine("\nPress enter to exit...");
         Console.ReadLine();
      }

      static void PrintUsage()
      {
         Console.WriteLine("Usage:");
         Console.WriteLine("   msgImport {XML File Name}");
      }

      static void CountMessages(string filename, out int numSms, out int numMms)
      {
         numSms = 0;
         numMms = 0;
         using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
         using (StreamReader sr = new StreamReader(fs))
         {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
               line = line.TrimStart();
               //if (Regex.IsMatch(line, "^<sms.*"))
               if (line.Length > 3 && line.Substring(0, 4) == "<sms")
               {
                  numSms++;
               }
               //if (Regex.IsMatch(line, "^<mms.*"))
               if (line.Length > 3 && line.Substring(0, 4) == "<mms")
               {
                  numMms++;
               }
            }
         }
      }//End CountMessages
   }
}
