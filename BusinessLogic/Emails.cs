using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace BusinessLogic
{
    public static class Emails
    {
        public static bool SendMail(string to, string subject, string body, bool IsBodyHtml)
        {
            string from = "noreply@netfive.tv";

            try
            {
                MailAddress fromAddress = new MailAddress(from);
                MailAddress toAddress = new MailAddress(to);

                return SendMail(fromAddress, toAddress, subject, body, IsBodyHtml);
            }
            catch (Exception ae)
            {
                Logs.WriteError("WebTech.Common.Email", "SendMail(" + from + "," + to + "," + subject + "," + body + ")", "General", ae.Message);
                return false;
            }
        }

        public static bool SendMail(string to, string cc, string subject, string body, bool IsBodyHtml)
        {
            string from = "noreply@netfive.tv";

            try
            {
                MailAddress fromAddress = new MailAddress(from);
                MailAddress toAddress = new MailAddress(to);
                MailAddress ccAddress = new MailAddress(cc);

                return SendMail(fromAddress, toAddress, ccAddress, subject, body, IsBodyHtml);
            }
            catch (Exception ae)
            {
                Logs.WriteError("WebTech.Common.Email", "SendMail(" + from + "," + to + "," + subject + "," + body + ")", "General", ae.Message);
                return false;
            }
        }

        public static bool SendMail(MailAddress from, MailAddress to, string subject, string body, bool IsBodyHtml)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("webmail.netfive.tv");
                MailMessage message = new MailMessage();

                smtpClient.Port = 25;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new System.Net.NetworkCredential("noreply@netfive.tv", "Tc2k?b62");
                smtpClient.EnableSsl = false;

                message.From = from; //here you can set address
                message.To.Add(to); //here you can add multiple to
                message.Subject = subject; //subject of email
                message.IsBodyHtml = IsBodyHtml; //To determine email body is html or not
                // ->> Define Email Message With Format<--	
                message.Body = body;
                // -->> End Define Email Message With Format<--
                smtpClient.Send(message);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("WebTech.Common.Email", "SendMail(" + from.Address + "," + to.Address + "," + subject + "," + body + ")", "General", ae.Message);
                return false;
            }
        }

        public static bool SendMail(MailAddress from, MailAddress to, MailAddress cc, string subject, string body, bool IsBodyHtml)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("webmail.netfive.tv");
                MailMessage message = new MailMessage();

                smtpClient.Port = 25;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new System.Net.NetworkCredential("noreply@netfive.tv", "Tc2k?b62");
                smtpClient.EnableSsl = false;

                message.From = from; //here you can set address
                message.To.Add(to); //here you can add multiple to
                message.CC.Add(cc); //here you can add multiple cc
                message.Subject = subject; //subject of email
                message.IsBodyHtml = IsBodyHtml; //To determine email body is html or not
                // ->> Define Email Message With Format<--	
                message.Body = body;
                // -->> End Define Email Message With Format<--
                smtpClient.Send(message);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("WebTech.Common.Email", "SendMail(" + from.Address + "," + to.Address + "," + subject + "," + body + ")", "General", ae.Message);
                return false;
            }
        }
        
    }
}
