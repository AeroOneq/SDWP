using ApplicationLib.Database;
using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Services
{
    public class EmailService : IEmailService<UserInfo>
    {
        private IEmailDatabase<UserInfo> Email { get; } = new EmailDB();

        public async Task<bool> CheckCode(int codeID, string code)
        {
            return await Email.CheckCode(codeID, code);
        }

        public async Task DeleteCode(int codeID)
        {
            await Email.DeleteCode(codeID);
        }

        public async Task SendChangePassLink(UserInfo user)
        {
            await Email.SendChangePassLink(user);
        }

        public async Task<int> SendCodeEmail(UserInfo user) =>
            await Email.SendCodeEmail(user);
    }
}
