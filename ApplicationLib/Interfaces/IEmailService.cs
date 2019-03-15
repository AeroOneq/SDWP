using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    interface IEmailService<UserType>
    {
        Task SendCodeEmail(UserType user);
        Task SendNewPasswordToUser(UserType user, string newPassword);
        Task ResetCode();
    }
}
