using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Exceptions;

using Newtonsoft.Json;
using System.IO;

namespace ApplicationLib.Database
{
    public class DocumentsDB : ICloudDocumentDB<Document>
    {
        private string ApiURL { get; } = "https://aerothedeveloper.ru/sdwpapi/v1.0.0/documents";

        public async Task DeleteDocument(Document document)
        {
            await Task.Run(async () =>
            {

                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?documentID={document.ID}", "DELETE");

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServerException();
            });
        }

        public async Task<IEnumerable<Document>> GetDocumentationDocuments(int documentationID)
        {
            return await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?documentationID={documentationID}", "GET"); 

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServerException();

                string responseContent = HTTP.GetResponseContent(httpWebResponse);
                List<Document> documents = JsonConvert.DeserializeObject
                    <List<Document>>(responseContent);

                return documents;
            });
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "GET"); 

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServerException();

                string responseContent = HTTP.GetResponseContent(httpWebResponse);

                return JsonConvert.DeserializeObject<IEnumerable<Document>>(responseContent);
            });
        }

        public async Task InsertDocument(Document document)
        {
            await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "POST"); 

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(JsonConvert.SerializeObject(document));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
                if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest ||
                    httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task UpdateDocument(Document document)
        {
            await Task.Run(async () =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "PUT");

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(JsonConvert.SerializeObject(document));
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
