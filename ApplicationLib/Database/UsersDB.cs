using AeroORMFramework;
using ApplicationLib.Exceptions;
using ApplicationLib.Models;
using ApplicationLib.Services;
using System;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;

namespace ApplicationLib.Database
{
    public class UsersDB
    {
        #region Services
        private IEmailService<UserInfo> EmailService { get; }
        #endregion

        #region Properties
        private Random Random { get; } = new Random();
        private string ConnectionString { get; } = DatabaseProperties.ConnectionString;
        #endregion

        public UsersDB()
        {
            IServiceAbstractFactory serviceFactory = new ServiceAbstractFactory();
            EmailService = serviceFactory.GetEmailService();
        }

        #region Authorization methods
        public async Task<UserInfo> TryToLoginAsync(LoginData loginData)
        {
            return await Task.Run(() =>
            {
                Connector connector = new Connector(ConnectionString);
                UserInfo user = connector.GetRecord<UserInfo>("Login", loginData.Login);

                if (user.Login == null)
                    throw new UserNotFoundException("Incorrect login or passwrod");
                if (user.Password == loginData.Password)
                    return user;

                throw new UserNotFoundException("Incorrect password");
            });
        }
        #endregion

        #region Create new account methods
        public async Task CreateNewAccountAsync(UserInfo newUser)
        {
            await Task.Run(() =>
            {
                Connector connector = new Connector(ConnectionString);
                connector.Insert(newUser);
            });
        }
        #endregion

        #region Check user object methods
        public async Task CheckLogin(string login)
        {
            await Task.Run(() =>
            {
                Connector connector = new Connector(ConnectionString);

                UserInfo tempUser = connector.GetRecord<UserInfo>("Login", login);

                if (tempUser.Login != null)
                    throw new NotAppropriateUserParam("This login is already taken");
            });
        }

        public async Task CheckEmail(string email)
        {
            await Task.Run(() =>
            {
                Connector connector = new Connector(ConnectionString);

                UserInfo tempUser = connector.GetRecord<UserInfo>("Email", email);

                if (tempUser.Login != null)
                    throw new NotAppropriateUserParam("This email is already taken");
            });
        }
        #endregion

        #region Remind pass methods
        public async Task RemindPassAsync(string login, string email)
        {
            await Task.Run(async () =>
            {
                UserInfo user = FindSuitableUserRecord(login, email);
                string newPassword = CreateNewPassword();
                ChangePassInTheDataBase(user, newPassword);
                await EmailService.SendNewPasswordToUser(user, newPassword);
            });
        }

        /// <summary>
        /// Finds the user recrod with the given login
        /// </summary>
        /// <returns>
        /// If the email of this record is the same as the input email returns
        /// the UserInfo object, throws an exception otherwise
        /// </returns>
        private static UserInfo FindSuitableUserRecord(string login, string email)
        {
            Connector connector = new Connector(DatabaseProperties.ConnectionString);
            UserInfo user = connector.GetRecord<UserInfo>("Login", login);
            if (user.Email == email)
                return user;
            throw new UserNotFoundException("User with such parameters doen't exists");
        }

        private void ChangePassInTheDataBase(UserInfo user, string newPassword)
        {
            Connector connector = new Connector(DatabaseProperties.ConnectionString);
#warning FIX THIS SHIT
            connector.DeleteRecord(user);
            user.Password = newPassword;
            connector.Insert(user);
        }

        private string CreateNewPassword()
        {
            string newPass = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                if (Random.Next() % 2 == 0)
                    newPass += (char)Random.Next('a', 'z' + 1);
                else
                    newPass += (char)Random.Next('A', 'Z' + 1);
            }
            return newPass;
        }
        #endregion

        #region Update methods
        public async Task UpdateUserRecord(UserInfo user)
        {
            await Task.Run(() =>
            {
                Connector connector = new Connector(DatabaseProperties.ConnectionString);
                connector.UpdateRecord<UserInfo>(user);
            });
        }
        #endregion

        public async Task<UserInfo> GetUser(string columnName, object value)
        {
            return await Task.Run(() =>
            {
                Connector connector = new Connector(DatabaseProperties.ConnectionString);
                return connector.GetRecord<UserInfo>(columnName, value);
            });
        }
    }
}
