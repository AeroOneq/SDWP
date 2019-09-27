using ApplicationLib.Exceptions;
using ApplicationLib.Models;
using ApplicationLib.Services;
using System;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace ApplicationLib.Database
{
    public class UsersDB : IUserDatabase<UserInfo>
    {
        #region Properties
        private string ApiURL { get; } = "https://aerothedeveloper.ru/sdwpapi/v1.0.0/users";
        #endregion

        #region Authorization methods
        public async Task<UserInfo> TryToLoginAsync(LoginData loginData)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL +
                    $"?login={loginData.Login}&pass={loginData.Password.GetHashCode()}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new UserNotFoundException("Неправильный пароль или логин");
                }

                string responseContent = HTTP.GetResponseContent(httpWebResponse);

                return JsonConvert.DeserializeObject<UserInfo>(responseContent);
            });
        }
        #endregion

        #region Create new account methods
        public async Task CreateNewAccountAsync(UserInfo newUser)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "POST");

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(JsonConvert.SerializeObject(newUser));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            });
        }
        #endregion

        #region Check user object methods
        public async Task CheckLogin(string login)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?login={login}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
                else if (httpWebResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new NotAppropriateUserParam("Логин уже занят");
                }
            });
        }

        public async Task CheckEmail(string email)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?email={email}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
                else if (httpWebResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new NotAppropriateUserParam("E-mail уже занят");
                }
            });
        }
        #endregion

        #region Remind pass methods
        public async Task RemindPassAsync(string login, string email)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL +
                    $"/remindpass?login={login}&email={email}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new UserNotFoundException("Пользователь с такими параметрами не существует");
                }
                else if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
            });
        }
        #endregion

        #region Update methods
        public async Task UpdateUserRecord(UserInfo user)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "PUT");

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(JsonConvert.SerializeObject(user));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
            });
        }
        #endregion

        public async Task<UserInfo> GetUserByID(int id)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?id={id}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }

                string responseContent = HTTP.GetResponseContent(httpWebResponse);
                return JsonConvert.DeserializeObject<UserInfo>(responseContent);
            });
        }
    }
}
