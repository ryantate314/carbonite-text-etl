﻿using CarboniteXmlParser.XmlEntities;
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

         //int numSms, numMms;
         //Console.WriteLine("Performing initial message count...");
         //var watch = System.Diagnostics.Stopwatch.StartNew();
         //CountMessages(filename, out numSms, out numMms);
         //watch.Stop();
         //var elapsedMs = watch.ElapsedMilliseconds;
         //double seconds = elapsedMs / 1000.0;
         //Console.WriteLine(String.Format("Number of SMS: {0}, Number of MMS: {1}", numSms, numMms));
         //Console.WriteLine(String.Format("Calculated in {0}s.", seconds));

         //var testContext = new StagingContext();
         //testContext.Database.Connection.Open();
         //testContext.Dispose();

         //using (var context = new StagingContext())
         //using (var uow = new UnitOfWork<StagingContext>(context))
         //{
           

         //   var fileRepo = new AttachmentRepository(System.Configuration.ConfigurationManager.AppSettings["media-directory"]);
         //   using (MessageReader reader = new MessageReader(filename))
         //   {
         //      using (var iterator = reader.getSmsIterator())
         //      {
         //         while (iterator.MoveNext())
         //         {
         //            Sms message = iterator.Current;
         //            Console.WriteLine(String.Format("On: {0}; From: {1}; Body: {2}", message.Date.ToShortDateString(), message.ContactName, message.Body));
         //         }
         //      }

         //      Console.WriteLine("MMS Messages:");

         //      using (var iterator = reader.getMmsIterator())
         //      {
         //         while (iterator.MoveNext())
         //         {
         //            Mms message = iterator.Current;
         //            Console.WriteLine(String.Format("On: {0}; {1}: {2}; Num Attachments: {3}", message.Date.ToShortDateString(), message.Box == MessageBox.Inbox ? "From" : "To", message.ContactName, message.Parts.Count));
         //            foreach (var part in message.Parts)
         //            {
         //               var objMessage = new MessageImport.Data.Message()
         //               {
         //                  Body = message.Body,
         //                  SendDate = message.Date
         //               };
         //               fileRepo.SaveAttachmentAsync(uow, objMessage, part);
         //            }
         //         }
         //      }
         //   }//end using message reader

         //   uow.Rollback();
         //}

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
