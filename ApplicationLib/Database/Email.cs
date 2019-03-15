using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Database
{
    class Email
    {
        private static Random Random { get; set; } = new Random();
        public static string Code { get; private set; }

        public static void SendCodeEmail(UserInfo user)
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
        }

        private static string CreateNewCode()
        {
            string code = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                code += (char)Random.Next('A', 'Z' + 1);
            }
            return code;
        }

        public static void SendNewPasswordToUser(UserInfo user, string newPassword)
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
        }


        public static void ResetCode() => Code = null;
    }
}
