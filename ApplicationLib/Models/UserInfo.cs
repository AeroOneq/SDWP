using System;
using System.Collections.Generic;
using AeroORMFramework;
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

        #region Public database properties
        [CanBeNull(false)]
        [PrimaryKey]
        public int ID { get; set; }
        [CanBeNull(false)]
        public string Login { get; set; }
        [CanBeNull(false)]
        public string Password { get; set; }
        [CanBeNull(false)]
        public string Name { get; set; }
        [CanBeNull(false)]
        public string Surname { get; set; }
        [CanBeNull(false)]
        public DateTime BirthDate { get; set; }
        [CanBeNull(false)]
        public string Email { get; set; }
        [CanBeNull(true)]
        [Json]
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
                    "меньше 200 символов");
            for (int i = 0; i < user.Login.Length; i++)
            {
                if (loginAllowedSymbols.IndexOf(user.Login[i]) < 0)
                    throw new NotAppropriateUserParam("Логин должен состоять из букв " +
                        "латиснкого алфавита и цифр");
            }
            //Password check
            if (user.Password.Length < 8)
                throw new NotAppropriateUserParam("Пароль должен состоять минимум из 8 символов");
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
