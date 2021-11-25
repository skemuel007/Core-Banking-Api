using System.Threading.Tasks;

namespace AliasWebApiCore.Services
{
    public interface IEmailSender
    {
       Task SendEmailAsync(string email, string subject, string message);
    }
}