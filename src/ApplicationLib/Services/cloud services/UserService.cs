﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Database;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Services
{
    public class UserService : IUserService<UserInfo>
    {
        private IUserDatabase<UserInfo> Database { get; } = new UsersDB();

        public async Task<UserInfo> AuthorizeUserAsync(LoginData loginData) =>
            await Database.TryToLoginAsync(loginData);

        public async Task CreateNewAccountAsync(UserInfo newUser) =>
            await Database.CreateNewAccountAsync(newUser);

        public async Task RemindPassAsync(string login, string email) =>
            await Database.RemindPassAsync(login, email);

        public async Task UpdateRecord(UserInfo user) =>
            await Database.UpdateUserRecord(user);

        public async Task<UserInfo> GetUserByID(int id) =>
            await Database.GetUserByID(id);

        public async Task CheckLogin(string login) =>
            await Database.CheckLogin(login);

        public async Task CheckEmail(string email) =>
            await Database.CheckEmail(email);
    }
}
