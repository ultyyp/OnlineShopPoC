using SendGrid.Helpers.Mail;
using SendGrid;

namespace OnlineShopPoC
{
    /// <summary>
    /// Class responsible for sending emails using SendGrid.
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClient _client = new(Environment.GetEnvironmentVariable("api_sendgrid_key"));

        /// <summary>
        /// Sends an email asynchronously to the specified recipient with the given subject and body.
        /// </summary>
        /// <param name="recipient">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(subject);
            ArgumentNullException.ThrowIfNull(body);

            var myEmail = Environment.GetEnvironmentVariable("myemail");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(myEmail, "OnlineShopPoC"),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = $"<strong>{body}</strong>"
            };

            msg.AddTo(new EmailAddress(recipient, "To user"));

            await _client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
