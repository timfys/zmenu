using System;
using System.Net;
using System.Net.Mail;
using Menu4Tech.Configuration;
using Menu4Tech.Helper;

namespace SmartWinners.Helpers;

public class SmtpSender
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="message"></param>
    /// <returns>Indicates that method completed successfully and message have been sent</returns>
    public static bool Send(SmtpClientConfiguration configuration, string message, bool isHtml)
    {
        try
        {
            var smtpClient = new SmtpClient
            {
                Host = configuration.SmtpHost,
                Port = configuration.SmtpPort,
                EnableSsl = configuration.SmtpUseSsl,
                Credentials = new NetworkCredential(configuration.SmtpLogin, configuration.SmtpPassword),
            };

            var mailMessage = new MailMessage(from: new MailAddress(configuration.FromAddress),
                to: new MailAddress(configuration.ToAddress))
            {
                Body = message,
                Subject = configuration.Subject,
                IsBodyHtml = isHtml,
            };

            smtpClient.Send(mailMessage);

            return true;
        }
        catch(Exception error)
        {
            FileLogger.Log("500ErrorLog", error.ToString());
            
            return false;
        }
    }
}