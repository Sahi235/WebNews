using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public class MailJet : IEmail
    {
        public async Task Send(string emailAdress, string body, EmailOptionDTO option)
        {
            SmtpClient client = new SmtpClient
            {
                Host = option.Host,
                Credentials = new NetworkCredential(option.ApiKey, option.ApiKeySecret),
                Port = option.Port
            };


            MailMessage message = new MailMessage(option.SenderEmail, emailAdress)
            {
                Body = body,
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}
