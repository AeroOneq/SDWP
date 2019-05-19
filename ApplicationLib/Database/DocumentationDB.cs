using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Newtonsoft.Json;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Exceptions;

using System.Runtime.InteropServices;

namespace ApplicationLib.Database
{
    public class DocumentationDB : ICloudDocumentationDB<Documentation>
    {
        #region Properties
        private string ApiURL { get; } = "https://aerothedeveloper.ru/sdwpapi/v1.0.0/documentations";
        #endregion
        public async Task<IEnumerable<Documentation>> GetAllDocumentations()
        {
            return await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());

                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServerException();

                string responseContent = HTTP.GetResponseContent(httpWebResponse);
                IEnumerable<Documentation> documentations =
                    JsonConvert.DeserializeObject<IEnumerable<Documentation>>(responseContent);

                return documentations;
            });
        }

        public async Task<IEnumerable<Documentation>> GetUserDocumentations(int userID)
        {
            return await Task.Run(async () =>
            {

                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?userID={userID}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());

                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServerException();

                string responseContent = HTTP.GetResponseContent(httpWebResponse);
                IEnumerable<Documentation> documentations = JsonConvert.
                    DeserializeObject<IEnumerable<Documentation>>(responseContent);

                return documentations;
            });
        }

        public async Task UpdateDocumentation(Documentation documentation)
        {
            await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "PUT");

                using (var rqStream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(rqStream))
                    {
                        sw.Write(JsonConvert.SerializeObject(documentation));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError ||
                    httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task DeleteDocumentation(int documentationID)
        {
            await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?documentationID={documentationID}", "DELETE");

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());

                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task InsertDocumentation(Documentation documentation)
        {
            await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "POST"); 

                using (var rqStream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(rqStream))
                    {
                        sw.Write(JsonConvert.SerializeObject(documentation));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError ||
                    httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ServerException();
                }
            });
        }
    }
}
