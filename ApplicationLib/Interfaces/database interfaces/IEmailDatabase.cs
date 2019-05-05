using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    interface IEmailDatabase<UserType>
    { 
        Task<int> SendCodeEmail(UserType user);
        Task<bool> CheckCode(int codeID, string code);
        Task DeleteCode(int codeID);
        Task SendChangePassLink(UserType user);
    }
}
