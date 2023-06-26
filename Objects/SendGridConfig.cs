using System.ComponentModel.DataAnnotations;

namespace OnlineShopPoC.Objects
{
    public class SendGridConfig
    {
        [Required]
        public string ApiKey { get; set; }
        [Required, EmailAddress]
        public string FromEmail { get; set; }
    }
}