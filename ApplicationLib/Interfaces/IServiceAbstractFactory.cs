using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface IServiceAbstractFactory
    {
        IUserService<UserInfo> GetUserService();
        IEmailService<UserInfo> GetEmailService(); 
    }
}
