using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Database
{
    public class EmailDB : IEmailDatabase<UserInfo>
    {
        private Random Random { get; set; } = new Random();
        public string Code { get; private set; }

        public async Task SendCodeEmail(UserInfo user)
        {
            await Task.Run(() =>
            {
                MailAddress senderAdress = new MailAddress("stepanov-ev@yandex.ru");
                MailAddress addresseeAdress = new MailAddress(user.Email);

                Code = CreateNewCode();

                MailMessage mailMessage = new MailMessage(senderAdress, addresseeAdress)
                {
                    Subject = "This is automatically-generated message, do not reply",
                    Body = $"Your code is {Code}"
                };

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("aeroone90@gmail.com", "AeroOne1"),
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
            });
        }

        private string CreateNewCode()
        {
            string code = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                code += (char)Random.Next('A', 'Z' + 1);
            }
            return code;
        }

        public async Task SendNewPasswordToUser(UserInfo user, string newPassword)
        {
            await Task.Run(() =>
            {
                MailAddress senderAdress = new MailAddress("stepanov-ev@yandex.ru");
                MailAddress addresseeAdress = new MailAddress(user.Email);

                MailMessage mailMessage = new MailMessage(senderAdress, addresseeAdress)
                {
                    Subject = "This is automatically-generated message, do not reply",
                    Body = $"Your new password is {newPassword}"
                };

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("aeroone90@gmail.com", "AeroOne1"),
                    EnableSsl = true
                };

                smtpClient.Send(mailMessage);
            });
        }

        public async Task ResetCode() => await Task.Run(() => Code = null);
    }
}
