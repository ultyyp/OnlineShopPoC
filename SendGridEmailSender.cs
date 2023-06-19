using SendGrid.Helpers.Mail;
using SendGrid;

namespace OnlineShopPoC
{
    /// <summary>
    /// Implementation of the <see cref="IEmailSender"/> interface using SendGrid for sending emails.
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously using SendGrid.
        /// </summary>
        /// <param name="recipient">The email recipient.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            ArgumentNullException.ThrowIfNull(recipient);
            ArgumentNullException.ThrowIfNull(subject);
            ArgumentNullException.ThrowIfNull(body);

            var apiKey = Environment.GetEnvironmentVariable("api_sendgrid_key");
            var client = new SendGridClient(apiKey);
            var myEmail = Environment.GetEnvironmentVariable("myemail");

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(myEmail, "OnlineShopPoC"),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = $"<strong>{body}</strong>"
            };

            msg.AddTo(new EmailAddress(recipient, "To user"));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
