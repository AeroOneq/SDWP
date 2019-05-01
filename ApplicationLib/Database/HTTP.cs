using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Database
{
    internal static class HTTP
    {
        const string Login = "sdwpapimainuser";
        const string Password = "dihfsodgoias;pdlvknkdslnvasoifjklfnsldafjsdlfa";
        public static string GetResponseContent(HttpWebResponse httpWebResponse)
        {
            string responseContent;
            using (var stream = httpWebResponse.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    responseContent = sr.ReadToEnd();
                };
            };

            return responseContent;
        }

        /// <summary>
        /// Creates a request with a given api url and sets the authorization headers
        /// </summary>
        public static HttpWebRequest GetRequest(string apiURL, string method)
        {
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(apiURL);

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Login + ":" + Password));
            httpWebRequest.Headers.Add("Authorization", "Basic " + credentials);

            httpWebRequest.Method = method;

            return httpWebRequest;
        }
    }
}
