using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IUserService<UserType>
    {
        Task<UserType> AuthorizeUserAsync(LoginData loginData);
        Task CreateNewAccountAsync(UserType user);
        Task RemindPassAsync(string login, string email);

        Task CheckLogin(string login);
        Task CheckEmail(string email);

        Task UpdateRecord(UserType user);
        Task<UserType> GetUserByID(int id);
    }
}
