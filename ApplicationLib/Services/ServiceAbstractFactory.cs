using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace ApplicationLib.Services
{
    public class ServiceAbstractFactory : IServiceAbstractFactory
    {
        public IEmailService<UserInfo> GetEmailService()
        {
            return new EmailService();
        }

        public IUserService<UserInfo> GetUserService()
        {
            return new UserService();
        }
    }
}
