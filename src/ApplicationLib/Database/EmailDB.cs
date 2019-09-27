using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Exceptions;

namespace ApplicationLib.Database
{
    public class EmailDB : IEmailDatabase<UserInfo>
    {
        private string ApiURL { get; } = "https://aerothedeveloper.ru/sdwpapi/v1.0.0/emailcodes";

        public async Task<bool> CheckCode(int codeID, string code)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?codeID={codeID}&code={code}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    return false;
                }
                else if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    throw new ServerException();
                }
            });
        }

        public async Task DeleteCode(int codeID)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?codeID={codeID}", "DELETE"); 

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task SendChangePassLink(UserInfo user)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?userID={user.ID}", "PUT");
                httpWebRequest.ContentLength = 0;

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task<int> SendCodeEmail(UserInfo user)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?userID={user.ID}&email={user.Email}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }

                return int.Parse(HTTP.GetResponseContent(httpWebResponse));
            });

        }
    }
}