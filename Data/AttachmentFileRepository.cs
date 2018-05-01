using CarboniteXmlParser.XmlEntities;
using log4net;
using Data.Staging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Data
{
   public class AttachmentFileRepository
   {
      private static readonly ILog logger = LogManager.GetLogger(typeof(AttachmentFileRepository));
      private string _directory;

      public AttachmentFileRepository(string directory)
      {
         _directory = directory;
      }

      private string getShortPath(string filename, byte[] contents)
      {
         //Calculate MD5, append with extension
         string hash = String.Empty;
         using (var md5 = MD5.Create())
         {
            hash = Convert.ToBase64String(md5.ComputeHash(contents));
         }

         //Make short path filename safe
         //= are just padding.
         hash = hash.Replace("=", "").Replace("/", "_").Replace("+", "-");

         int dotPos = filename.LastIndexOf('.');
         if (dotPos == -1)
         {
            return hash;
         }
         return String.Format("{0}.{1}", hash, filename.Substring(dotPos + 1));
      }

      private string _getFullPath(string shortPath)
      {
         return _directory + shortPath;
      }

      //private bool sameFile(string path, byte[] newContents)
      //{
      //   if (!File.Exists(path)) { return false; }

      //   //Compute hash of new file
      //   string newHash = "";
      //   using (var md5 = MD5.Create())
      //   {
      //      newHash = Convert.ToBase64String(md5.ComputeHash(newContents));
      //   }
      //   string oldHash = "";
      //   //Compute hash of old file
      //   using (var md5 = MD5.Create())
      //   {
      //      using (var stream = File.OpenRead(path))
      //      {
      //         oldHash = Convert.ToBase64String(md5.ComputeHash(stream));
      //      }
      //   }
      //   return newHash.Equals(oldHash);
      //}

      private async Task<string> SaveFileAsync(string filename, byte[] contents, Staging.Message message)
      {
        
         string shortPath = getShortPath(filename, contents);
         string fullPath = _getFullPath(shortPath);

         if (File.Exists(fullPath))
         {
            return shortPath;
         }
         
         using (FileStream stream = new FileStream(fullPath, FileMode.CreateNew))
         {
            await stream.WriteAsync(contents, 0, contents.Length);
         }

         File.SetCreationTimeUtc(fullPath, message.SendDate);
         File.SetLastAccessTimeUtc(fullPath, DateTime.Now);

         return shortPath;
      }

      private string GetDefaultExtension(string mimeType)
      {
         string result;
         RegistryKey key;
         object value;

         key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
         value = key != null ? key.GetValue("Extension", null) : null;
         result = value != null ? value.ToString() : string.Empty;

         return result;
      }

      public async Task<Attachment> SaveAttachmentAsync(Data.Staging.Message message, MessagePart part)
      {
         if (part.FileName == "null")
         {
            logger.Warn("Unknown filename detected for file. Message ID: " + message.MessageId);
            //Query default extension for the mime type instead
            part.FileName = "unknown" + GetDefaultExtension(part.MimeType);
         }
         string filePath = await SaveFileAsync(part.FileName, part.Data, message);

         Attachment attachment = new Attachment()
         {
            FileName = part.FileName,
            MimeType = part.MimeType,
            Path = filePath,
            MessageId = message.MessageId
         };

         return attachment;
      }

      public String GetFullPath(string filename)
      {
         return _getFullPath(filename);
      }
   }
}
