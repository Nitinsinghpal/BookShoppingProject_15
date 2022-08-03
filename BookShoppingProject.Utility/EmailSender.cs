﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.Utility
{
    public class EmailSender : IEmailSender
    {
        public EmailSettings _emailSettings { get; }
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email)
                                 ? _emailSettings.ToEmail
                                 : email;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Muhammad Hassan Tariq")
                };
                mail.To.Add(new MailAddress(toEmail));
                mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                mail.Subject = "Personal Management System - " + subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_emailSettings.SecondayDomain, _emailSettings.SecondaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //do something here
            }
        }
    }
}


//    public IEmailSettings _emailSettings { get; }
//    public EmailSender(Options<EmailSettings> emailSettings)
//    {
//        _emailSettings = emailSettings.value;
//    }

//    public Task SendEmailAsync(string email, string subject, string htmlMessage)
//    {
//        Execute(email, subject, htmlMessage).Wait();
//        return Task.FromResult(0);
//    }
//    public async Task Execute(string email, string subject, string message)
//    {
//        try
//        {
//            string toEmail = string.IsNullOrEmpty(email)
//                             ? _emailSettings.ToEmail
//                             : email;
//            MailMessage mail = new MailMessage()
//            {
//                From = new MailAddress(_emailSettings.UsernameEmail, "Muhammad Hassan Tariq")
//            };
//            mail.To.Add(new MailAddress(toEmail));
//            mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

//            mail.Subject = "Personal Management System - " + subject;
//            mail.Body = message;
//            mail.IsBodyHtml = true;
//            mail.Priority = MailPriority.High;

//            using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
//            {
//                smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
//                smtp.EnableSsl = true;
//                await smtp.SendMailAsync(mail);
//            }

//        }
//        catch (Exception ex)
//        {
//            //do something here
//        }
//    }

//}