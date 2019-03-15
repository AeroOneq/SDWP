using AeroORMFramework;
using ApplicationLib.Exceptions;
using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Database
{
    internal class UsersDB
    {
        #region Properties
        private Random Random { get; } = new Random();

        #endregion


        public async Task<UserInfo> TryToLogin(LoginData loginData)
        {
            return await Task.Run(() =>
            {
                Connector connector = new Connector(DatabaseProperties.ConnectionString);
                UserInfo user = connector.GetRecord<UserInfo>("Login", loginData.Login);
                if (user.Login == null)
                    throw new UserNotFoundException("Incorrect login or passwrod");
                if (user.Password == loginData.Password)
                    return user;
                else
                    throw new UserNotFoundException("Incorrect password");
            });
        }
        public async Task CreateNewAccount(UserInfo newUser)
        {
            await Task.Run(() =>
            {
                Connector connector = new Connector(DatabaseProperties.ConnectionString);
                connector.Insert(newUser);
            });
        }
        public void CheckUserObject(UserInfo user)
        {
            Connector connector = new Connector(DatabaseProperties.ConnectionString);
            CheckInputData(user, connector);
        }
        private void CheckInputData(UserInfo user, Connector connector)
        {
            UserInfo tempUser = connector.GetRecord<UserInfo>("Login", user.Login);
            if (tempUser.Login != null)
                throw new NotAppropriateUserParam("This login is already taken");
            tempUser = connector.GetRecord<UserInfo>("Email", user.Email);
            if (tempUser.Email != null)
                throw new NotAppropriateUserParam("This emil is already registered");
        }
        public void RemindPass(string login, string email)
        {
            UserInfo user = FindSuitableUserRecord(login, email);
            string newPassword = CreateNewPassword();
            ChangePassInTheDataBase(user, newPassword);
            Email.SendNewPasswordToUser(user, newPassword);
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
        private static void ChangePassInTheDataBase(UserInfo user, string newPassword)
        {
            Connector connector = new Connector(DatabaseProperties.ConnectionString);
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
    }
}
