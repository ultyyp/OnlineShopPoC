using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace OnlineShopPoC
{
    /// <summary>
    /// Implementation of the email sender using SendGrid service.
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClient _client;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the SendGridEmailSender class.
        /// </summary>
        /// <param name="configuration">The config gotten from secrets.json.</param>
        public SendGridEmailSender(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _configuration = configuration;
            _client = new SendGridClient(_configuration.GetValue<string>("SendGridConfig:ApiKey"));
        }


        /// <summary>
        /// Sends an email asynchronously using SendGrid.
        /// </summary>
        /// <param name="recipient">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <returns>A Task representing the asynchronous operation with the response.</returns>
        public async Task<Response> SendEmailAsync(string recipient, string subject, string body)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(subject);
            ArgumentNullException.ThrowIfNull(body);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_configuration.GetValue<string>("SendGridConfig:FromEmail"), "OnlineShopPoC"),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = $"<strong>{body}</strong>"
            };

            msg.AddTo(new EmailAddress(recipient, "To user"));
            return await _client.SendEmailAsync(msg).ConfigureAwait(false);
        }

    }

          
}
