using AuthWithIdentityFramework.Repository.Interface;
using AuthWithIdentityFramework.ViewModels.Email;
using System.Net.Mail;

namespace AuthWithIdentityFramework.Repository.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            bool status = false;
            try
            {
                GetEmailSetting getEmailSetting = new GetEmailSetting()
                {
                    SecretKey = _configuration.GetValue<string>("AppSetting:GoogleMailSecretCode"),
                    From = _configuration.GetValue<string>("AppSetting:EmailSetting:From"),
                    SmtpServer = _configuration.GetValue<string>("AppSetting:EmailSetting:SmtpServer"),
                    Port = _configuration.GetValue<int>("AppSetting:EmailSetting:Port"),
                    EnableSSL = _configuration.GetValue<bool>("AppSetting:EmailSetting:EnableSSL"),
                };
                MailMessage mailMessage= new MailMessage()
                {
                    From = new MailAddress(getEmailSetting.From),
                    Subject = subject,
                    Body = htmlMessage,
                    //IsBodyHtml = true,
                };
                mailMessage.To.Add(email); 

                SmtpClient smtpClient= new SmtpClient(getEmailSetting.SmtpServer)
                {
                    Port = getEmailSetting.Port,
                    Credentials = new System.Net.NetworkCredential(getEmailSetting.From, getEmailSetting.SecretKey),
                    EnableSsl = getEmailSetting.EnableSSL,
                    //UseDefaultCredentials = false,
                };
                await smtpClient.SendMailAsync(mailMessage);    
                status= true;   
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
    }
}
