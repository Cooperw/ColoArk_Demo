using ColoArk.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ColoArk.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking <a href='{HtmlEncoder.Default.Encode(link)}'>here</a>.");
        }
    }
}
