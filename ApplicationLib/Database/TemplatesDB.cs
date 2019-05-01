using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;

using AeroORMFramework;
using System.Net;
using ApplicationLib.Exceptions;
using Newtonsoft.Json;
using System.IO;

namespace ApplicationLib.Database
{
    class TemplatesDB : ICloudTemplatesDB<Template>
    {
        private string ApiURL { get; } = "https://aerothedeveloper.ru/sdwpapi/v1.0.0/templates";

        public async Task DeleteTemplate(Template template)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?templateID={template.ID}", "DELETE");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError ||
                    httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task<IEnumerable<Template>> GetAllTemplates()
        {
            return await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }

                string responseContent = HTTP.GetResponseContent(httpWebResponse);
                IEnumerable<Template> templates = JsonConvert.DeserializeObject<
                    IEnumerable<Template>>(responseContent);

                return templates;
            });
        }

        public async Task<IEnumerable<Template>> GetUserTemplates(int userID)
        {
            return await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL + $"?userID={userID}", "GET");

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServerException();
                }

                string responseContent = HTTP.GetResponseContent(httpWebResponse);
                IEnumerable<Template> templates = JsonConvert.DeserializeObject<
                    IEnumerable<Template>>(responseContent);

                return templates;
            });
        }

        public async Task InsertTemplate(Template template)
        {
            await Task.Run(() =>
            {
                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "POST");

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(JsonConvert.SerializeObject(template));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError ||
                    httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ServerException();
                }
            });
        }

        public async Task UpdateTemplate(Template template)
        {
            await Task.Run(() =>
            {

                HttpWebRequest httpWebRequest = HTTP.GetRequest(ApiURL, "PUT");

                using (var stream = httpWebRequest.GetRequestStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(JsonConvert.SerializeObject(template));
                    }
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError ||
                    httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new ServerException();
                }
            });
        }
    }
}
