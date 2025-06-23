using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjetoRenar.CrossCutting.Configurations;
using ProjetoRenar.Domain.Contracts.Messages;
using ProjetoRenar.Domain.Models;
using Sidetech.Framework.Cryptography;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace ProjetoRenar.Infra.Message
{
    public class EmailMessage : IEmailMessage
    {
        private readonly EmailSettings emailSettings;

        public EmailMessage(IHttpContextAccessor httpContextAccessor)
        {
            var cookie = httpContextAccessor.HttpContext.Request.Cookies["peoplenet_settings"];

            if (cookie != null)
            {
                try
                {
                    var crConnection = new DefaultCrypto(DefaultCrypto.Algorithms.Rijndael);
                    //var urlBase = httpContextAccessor.HttpContext.Request.Host.ToString().Split('.')[0];  
                    var urlBase = AppSettings.UrlBase;

                    var data = crConnection.Decrypt(cookie);

                    var configurarionSettings = JsonConvert.DeserializeObject<ConfigurationSettings>(data);
                    emailSettings = new EmailSettings();

                    emailSettings.Mail = configurarionSettings.Mail;

                    var info = ObterConfiguracoes(urlBase);
                    if(info != null)
                        configurarionSettings.Name = info.Name;

                    emailSettings.Name = configurarionSettings.Name.Replace("SST", "RH");
                    emailSettings.Pass = configurarionSettings.Pass;
                    emailSettings.Port = configurarionSettings.Port;
                    emailSettings.Smtp = configurarionSettings.Smtp;
                    emailSettings.User = configurarionSettings.User;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public void Send(EmailMessageModel model)
        {
            var mailFrom = new MailAddress(emailSettings.Mail, emailSettings.Name);
            var mailTo = new MailAddress(model.To);

            var mail = new MailMessage(mailFrom, mailTo);
            mail.Subject = model.Subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = model.Body;
            mail.IsBodyHtml = model.IsBodyHtml;

            var smtp = new SmtpClient(emailSettings.Smtp, int.Parse(emailSettings.Port));
            smtp.EnableSsl = true;

            if(!string.IsNullOrEmpty(emailSettings.User))
                smtp.Credentials = new NetworkCredential(emailSettings.User, emailSettings.Pass);
            else
                smtp.Credentials = new NetworkCredential(emailSettings.Mail, emailSettings.Pass);

            smtp.Send(mail);
        }

        public void SendWithAttachment(EmailMessageModel model, byte[] pdfBytes, string attachmentFileName)
        {
            var mailFrom = new MailAddress(emailSettings.Mail, emailSettings.Name);
            var mailTo = new MailAddress(model.To);

            var mail = new MailMessage(mailFrom, mailTo);
            mail.Subject = model.Subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = model.Body;
            mail.IsBodyHtml = model.IsBodyHtml;

            // Anexar o arquivo em PDF
            if (pdfBytes != null && pdfBytes.Length > 0 && !string.IsNullOrEmpty(attachmentFileName))
            {
                using (var memoryStream = new MemoryStream(pdfBytes))
                {
                    var attachment = new Attachment(memoryStream, attachmentFileName, MediaTypeNames.Application.Pdf);
                    mail.Attachments.Add(attachment);

                    var smtp = new SmtpClient(emailSettings.Smtp, int.Parse(emailSettings.Port));
                    smtp.EnableSsl = true;

                    if (!string.IsNullOrEmpty(emailSettings.User))
                        smtp.Credentials = new NetworkCredential(emailSettings.User, emailSettings.Pass);
                    else
                        smtp.Credentials = new NetworkCredential(emailSettings.Mail, emailSettings.Pass);

                    smtp.Send(mail);
                }
            }
        }


        public static ConfigurationSettings ObterConfiguracoes(string chaveCliente)
        {            
            var configurationSettings = new ConfigurationSettings();

            try
            {
                using (var connection = new SqlConnection(AppSettings.ConnectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand("select vc_NomeEmail from View_ClienteSistemas where vc_ChaveCliente = @vc_ChaveCliente", connection);
                    cmd.Parameters.AddWithValue("@vc_ChaveCliente", chaveCliente);
                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        configurationSettings.Name = Convert.ToString(dr["vc_NomeEmail"]);
                    }

                    dr.Close();
                }
            }
            catch (Exception e) { Debug.WriteLine(e.Message); };

            return configurationSettings;
        }
    }
}
