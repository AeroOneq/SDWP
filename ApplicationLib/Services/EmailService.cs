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
        private Email Email { get; } = new Email();
        public string Code => Email.Code;

        #region Singleton
        private static EmailService emailService;
        public static EmailService GetService
        {
            get
            {
                if (emailService == null)
                    emailService = new EmailService();
                return emailService;
            }
        }
        #endregion

        public async Task SendCodeEmail(UserInfo user) =>
            await Email.SendCodeEmail(user);

        public async Task SendNewPasswordToUser(UserInfo user, string newPassword) =>
            await Email.SendNewPasswordToUser(user, newPassword);

        public async Task ResetCode() => await Email.ResetCode();
    }
}
