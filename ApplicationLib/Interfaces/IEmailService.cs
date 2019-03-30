using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IEmailService<UserType>
    {
        string Code { get; }

        Task SendCodeEmail(UserType user);
        Task SendNewPasswordToUser(UserType user, string newPassword);
        Task ResetCode();
    }
}
