using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PreparatoriaIIM.Application.Common.Interfaces;
using System.Threading;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace PreparatoriaIIM.Infrastructure.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("El destinatario no puede estar vacío", nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("El asunto no puede estar vacío", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("El cuerpo del mensaje no puede estar vacío", nameof(body));

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var builder = new BodyBuilder();
                if (isHtml)
                {
                    builder.HtmlBody = body;
                }
                else
                {
                    builder.TextBody = body;
                }

                email.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
                    
                    if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
                    {
                        await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
                    }
                    
                    await client.SendAsync(email, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);
                }

                _logger.LogInformation("Correo electrónico enviado exitosamente a {To} con asunto: {Subject}", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo electrónico a {To}", to);
                throw;
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                    message.To.Add(to);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = isHtml;

                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo electrónico a {to}", to);
                throw; // Relanzar para manejar el error en el controlador
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken, string callbackUrl)
        {
            var subject = "Restablecer contraseña";
            var body = $@"
                <h2>Restablecer contraseña</h2>
                <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
                <p>Por favor, haz clic en el siguiente enlace para restablecer tu contraseña:</p>
                <p><a href='{callbackUrl}'>{callbackUrl}</a></p>
                <p>Si no solicitaste restablecer tu contraseña, puedes ignorar este mensaje.</p>
                <p>Este enlace expirará en 24 horas.</p>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string userName)
        {
            var subject = "¡Bienvenido a Preparatoria IIM!";
            var body = $@"
                <h2>¡Bienvenido a Preparatoria IIM, {userName}!</h2>
                <p>Gracias por registrarte en nuestra plataforma. Estamos encantados de tenerte con nosotros.</p>
                <p>Ahora puedes iniciar sesión con tu correo electrónico y la contraseña que has establecido.</p>
                <p>Si tienes alguna pregunta, no dudes en contactar con nuestro equipo de soporte.</p>
                <p>¡Gracias!</p>
                <p>El equipo de Preparatoria IIM</p>";

            await SendEmailAsync(email, subject, body);
        }
    }
}
