using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;


namespace ApplicationLib.Interfaces
{
    interface IUserDatabase<UserType>
    {
        Task<UserType> TryToLoginAsync(LoginData loginData);
        Task CreateNewAccountAsync(UserType newUser);
        Task CheckLogin(string login);
        Task CheckEmail(string email);
        Task RemindPassAsync(string login, string email);
        Task UpdateUserRecord(UserType user);
        Task<UserInfo> GetUserByID(int id);
    }
}
