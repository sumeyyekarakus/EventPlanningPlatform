using EtkinlikProjem.Interfaces;
using System.Net.Mail;

namespace EtkinlikProjem.Services
{

    using MimeKit;
    using MailKit.Net.Smtp;
    using System.Threading.Tasks;

    public class EmailService
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string senderEmail = "sumeyyekarakusk@gmail.com";  // Gönderen e-posta adresi
        private readonly string senderPassword = "021406karakus"; // Uygulama şifresi

        // Şifre sıfırlama e-postası gönderme metodu
        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", senderEmail));
            message.To.Add(new MailboxAddress("Recipient", email));
            message.Subject = "Password Reset Request";

            // E-posta içeriği
            message.Body = new TextPart("plain")
            {
                Text = $"Click the link to reset your password: {resetLink}"
            };

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    // SMTP sunucusuna bağlan
                    await smtpClient.ConnectAsync(smtpServer, smtpPort, false);  // TLS bağlantısı
                    await smtpClient.AuthenticateAsync(senderEmail, senderPassword);  // Kullanıcı adı ve şifre ile kimlik doğrulama
                    await smtpClient.SendAsync(message);  // E-postayı gönder
                    await smtpClient.DisconnectAsync(true);  // Bağlantıyı kes
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

}
