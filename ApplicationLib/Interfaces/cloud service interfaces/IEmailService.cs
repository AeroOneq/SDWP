using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IEmailService<UserType>
    {
        Task<int> SendCodeEmail(UserType user);
        Task<bool> CheckCode(int codeID, string code);
        Task DeleteCode(int codeID);
        Task SendChangePassLink(UserType user);
    }
}
