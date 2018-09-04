using System.Threading.Tasks;

namespace ColoArk.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
