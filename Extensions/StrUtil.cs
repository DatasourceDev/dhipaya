using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dhipaya.Extensions
{

   public class MailImage
   {
      public string FILE_NAME { get; private set; }
      public string FILE_TYPE { get; private set; }
      public string PATH { get; private set; }
      public string ORIGINAL_TAG { get; private set; }
      public string CONTENT_ID { get; private set; }

      public MailImage(string fileName, string fileType, string path, string originalTag, string contentId)
      {
         this.FILE_NAME = fileName;
         this.FILE_TYPE = fileType;
         this.PATH = path;
         this.ORIGINAL_TAG = originalTag;
         this.CONTENT_ID = contentId;
      }
   }

   public class StrUtil
   {
      public static string Raw(string str)
      {
         if (!string.IsNullOrEmpty(str))
         {
            str = str.Replace("\r\n", "<br/>");
            str = str.Replace("\n\r", "<br/>");
            str = str.Replace("\n", "<br/>");
         }
         return str;
      }
    public static string RawText(string str)
    {
      if (!string.IsNullOrEmpty(str))
      {
        str = str.Replace("\r\n", "<br/>");
        str = str.Replace("\n\r", "<br/>");
        str = str.Replace("\n", "<br/>");
      }
      return str;
    }
    public static string randomString(int size)
      {
         Random _rng = new Random();
         string _chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmopqrstuvwxyz";
         char[] buffer = new char[size];

         for (int i = 0; i < size; i++)
         {
            buffer[i] = _chars[_rng.Next(_chars.Length)];
         }
         return new string(buffer);
      }

      public static string ToLower(string str)
      {
         if (string.IsNullOrEmpty(str))
            str = "";

         return str.ToLower();
      }

      async public Task<string> RandomizeString(int length)
      {

         var random = new Random();
         await Task.Delay(50);
         string chars = await Task.FromResult<string>(shuffle("ABCDE09FGHIJ18KLMNO27PQRST36UVWXY45Z" + (DateTime.Now.Ticks).ToString()));

         return new string(Enumerable.Repeat(chars, length)
           .Select(s => s[random.Next(s.Length)]).ToArray());
      }

      private string shuffle(string str)
      {
         char[] array = str.ToCharArray();
         Random rng = new Random();
         int n = array.Length;
         while (n > 1)
         {
            n--;
            int k = rng.Next(n + 1);
            var value = array[k];
            array[k] = array[n];
            array[n] = value;
         }
         return new string(array);
      }

      public static bool IsValidEmail(string email)
      {
         try
         {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
         }
         catch
         {
            return false;
         }
      }

      public List<MailImage> ExtractImages(string pMessage)
      {
         var matches = System.Text.RegularExpressions.Regex.Matches(pMessage, "<img .*?>(.*?)>");
         List<MailImage> imageAttachmentList = null;

         if (matches != null)
         {
            var fileNameWithExtension = "";
            var fileName = "";
            var fileType = "";
            var filePath = "";
            var strSrc = "src=\"..";

            imageAttachmentList = new List<MailImage>();
            foreach (var match in matches)
            {
               filePath = match.ToString().Substring(((match.ToString().IndexOf(strSrc) + (strSrc).Length)));
               filePath = filePath.Substring(0, filePath.IndexOf("\""));
               fileNameWithExtension = filePath.Substring(filePath.LastIndexOf("\\") + 1);
               fileName = fileNameWithExtension.Substring(0, fileNameWithExtension.LastIndexOf("."));
               fileType = fileNameWithExtension.Substring(fileNameWithExtension.LastIndexOf(".") + 1);

               imageAttachmentList.Add(new MailImage(fileNameWithExtension, fileType, filePath, match.ToString(), fileName));
            }
         }
         return imageAttachmentList;
      }

      public static string ToTitleCase(string pString)
      {
         if (!string.IsNullOrEmpty(pString))
         {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(pString);
         }
         else
         {
            return string.Empty;
         }
      }
   }
   public class LogUtil
   {
      public static void WriteLog(string name, string Message)
      {
         StreamWriter sw = null;
         try
         {

            if (!Directory.Exists(@"c:\pos3v2\log"))
               Directory.CreateDirectory(@"c:\pos3v2\log");

            var path = @"c:\pos3v2\log\" + "\\" + name + ".txt";
            sw = new StreamWriter(path, true);
            sw.WriteLine(DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ": " + Message);
            sw.Flush();
            sw.Close();

            Console.WriteLine();
            Console.Write(DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ": " + Message);
         }
         catch
         {
         }
      }
   }
}