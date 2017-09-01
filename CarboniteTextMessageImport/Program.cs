using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteTextMessageImport
{
   class Program
   {
      static void Main(string[] args)
      {
         if (args.Length < 1)
         {
            PrintUsage();
            return;
         }

         string filename = args[0];

         Console.WriteLine(filename);

         if (!File.Exists(filename))
         {
            Console.WriteLine("File not found.");
            return;
         }

         int numSms, numMms;
         Console.WriteLine("Performing initial message count...");
         var watch = System.Diagnostics.Stopwatch.StartNew();
         CountMessages(filename, out numSms, out numMms);
         watch.Stop();
         var elapsedMs = watch.ElapsedMilliseconds;
         double seconds = elapsedMs / 1000.0;
         Console.WriteLine(String.Format("Number of SMS: {0}, Number of MMS: {1}", numSms, numMms));
         Console.WriteLine(String.Format("Calculated in {0}s.", seconds));

         using (XmlReader reader = XmlReader.Create(filename))
         {
            reader.Read();
            watch = System.Diagnostics.Stopwatch.StartNew();
            reader.ReadToFollowing("mms");
            watch.Stop();
            seconds = watch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Took {0}s to seek to mms.", seconds);
            return;

            reader.ReadToFollowing("smses");
            UInt64 epochDate;
            if (!UInt64.TryParse(reader.GetAttribute("backup_date"), out epochDate))
            {
               Console.Error.WriteLine("Could not find backup date.");
            }
            DateTime backupDate = new DateTime(1970, 1, 1);
            backupDate = backupDate.AddMilliseconds(epochDate);
            Console.WriteLine(String.Format("Backup taken on {0}", backupDate.ToShortDateString()));
         }
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
