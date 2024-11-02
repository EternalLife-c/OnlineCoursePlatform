using System;
using System.Net.Mail;

namespace TopLearn.Core.Senders
{
    public class SendEmail
    {
        public static async Task SendAsync(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("hoseinsland@gmail.com", "تاپ لرن");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("hoseinsland@gmail.com", "zykfguhykxvkazlj"); // Use App Password
            SmtpServer.EnableSsl = true;

            // Use the async method here
            await SmtpServer.SendMailAsync(mail);
        }
    }
}