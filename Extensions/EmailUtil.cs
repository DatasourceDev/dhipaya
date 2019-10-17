using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dhipaya.Extensions
{
   public class EmailUtil
   {

      static Regex ValidEmailRegex = CreateValidEmailRegex();

      /// <summary>
      /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
      /// </summary>
      /// <returns></returns>
      private static Regex CreateValidEmailRegex()
      {
         string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
             + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
             + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

         return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
      }

      internal static bool EmailIsValid(string emailAddress)
      {
         if (string.IsNullOrEmpty(emailAddress))
            return false;
         bool isValid = ValidEmailRegex.IsMatch(emailAddress);

         return isValid;
      }

      public static string sendNotificationEmail(Smtp smtp, string to, string header, string message)
      {
         var msg = new System.Text.StringBuilder();
         try
         {
            //msg.Append("TO_EMAIL: " + to + "   ");
            //msg.Append(" SMTP_SERVER: " + smtp.SMTP_SERVER + "   ");
            //msg.Append(" SMTP_PORT: " + smtp.SMTP_PORT + "   ");
            //msg.Append(" SMTP_FROM: " + smtp.SMTP_FROM + "   ");
            //msg.Append(" SMTP_USERNAME: " + smtp.SMTP_USERNAME + "   ");
            //msg.Append(" SMTP_PASSWORD: " + smtp.SMTP_PASSWORD + "   ");
            //msg.Append(" STMP_SSL: " + smtp.STMP_SSL + "   ");

            var SMTP_SERVER = smtp.SMTP_SERVER;
            var SMTP_PORT = smtp.SMTP_PORT;
            var SMTP_USERNAME = smtp.SMTP_USERNAME;
            var SMTP_PASSWORD = smtp.SMTP_PASSWORD;
            var SMTP_FROM = smtp.SMTP_FROM;
            bool STMP_SSL = smtp.STMP_SSL;

            SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

            smtpClient.UseDefaultCredentials = true;
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = STMP_SSL;

            var mail = new MailMessage(SMTP_FROM, to, header, message);
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            smtpClient.Credentials = cred;
            smtpClient.Send(mail);

            return msg.ToString();
         }
         catch (Exception ex)
         {
            msg.AppendLine(" EXCEPTION: " + ex.Message);
         }
         return msg.ToString();
      }
   }
}
