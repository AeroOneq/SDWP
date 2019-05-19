using System;
using System.Collections.Generic;
using ApplicationLib.Exceptions;

namespace ApplicationLib.Models
{
    public class UserInfo
    {
        #region Constants
        private const string loginAllowedSymbols = "qwertyuiopasdfghklzxcvbnmQWERTYUIOPASDFG" +
            "HJKLZXCVBNM1234567890";
        private const string emailAllowedSymbols = "qwertyuiopasdfghklzxcvbnmQWERTYUIOPASDFG" +
            "HJKLZXCVBNM1234567890!#$%&'*+-/=?^_`{|}~@\"(),:;<>@[\\]";
        #endregion

        /// <summary>
        /// The user who is now working with a system
        /// </summary>
        public static UserInfo CurrentUser { get; set; }

        #region Public database properties
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public byte[] UserPhoto { get; set; }
        #endregion

        #region Constructors
        public UserInfo() { }
        public UserInfo(UserInfo user)
        {
            ID = user.ID;
            Login = user.Login;
            Password = user.Password;
            Name = user.Name;
            Surname = user.Surname;
            BirthDate = user.BirthDate;
            Email = user.Email;
            UserPhoto = user.UserPhoto;
        }
        #endregion

        /// <exception cref="NotAppropriateUserParam">
        /// When some of the param is wrong
        /// </exception>
        public static void CheckUserProperties(UserInfo user)
        {
            //Login check
            if (user.Login.Length < 6 || user.Login.Length > 200)
                throw new NotAppropriateUserParam("Длина логина должна быть больше 6 и " +
                    "меньше 200 символов, логин состоит только из букв латинского алфавита и цифр");
            for (int i = 0; i < user.Login.Length; i++)
            {
                if (loginAllowedSymbols.IndexOf(user.Login[i]) < 0)
                    throw new NotAppropriateUserParam("Длина логина должна быть больше 6 и " +
                    "меньше 200 символов, логин состоит только из букв латинского алфавита и цифр");
            }
            //Name check
            if (user.Name.Length < 1)
                throw new NotAppropriateUserParam("Имя должно состоять минимум из одной буквы");
            //Surname check
            if (user.Surname.Length < 1)
                throw new NotAppropriateUserParam("Фамилия должна состоять минимум из одного " +
                    "символа");
            //Email check
            string email = user.Email;
            if (email.Length < 3)
                throw new NotAppropriateUserParam("Длина email должна быть больше трех");
            if (email[0] == '.')
                throw new NotAppropriateUserParam("Email не может начинаться с точки");
            if (email.IndexOf("@") < 0)
                throw new NotAppropriateUserParam("Email должен содержать символ '@'");
            for (int i = 0; i < email.Length; i++)
            {
                if (emailAllowedSymbols.IndexOf(email[0]) < 0)
                    throw new NotAppropriateUserParam("Один мз символов введенного email недопустим");
            }
        }
    }
}
