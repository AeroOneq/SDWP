using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;

using Newtonsoft.Json;

namespace ApplicationLib.Services
{
    class LocalDocumentationService : ILocalDocumentationStorage
    {
        public LocalDocumentationService()
        {
        }

        public async Task CreateLocalDocumentationFile(LocalDocumentation localDocumentation, string folderPath)
        {
            string path = Path.Combine(folderPath, localDocumentation.Documentation.Name);
            byte[] localDocumentationBytes = GetByteArrayFromString(localDocumentation.GetJsonString());

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Seek(0, SeekOrigin.Begin);
                await fs.WriteAsync(localDocumentationBytes, 0, localDocumentationBytes.Length);
            }
        }

        private byte[] GetByteArrayFromString(string str)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Deletes the documentation file is this file exists
        /// </summary>
        /// <exception cref="FileNotFoundException"/>
        public void DeleteLocalDocumentationFile(LocalDocumentation localDocumentation)
        {
            string path = Path.Combine(localDocumentation.DocumentationPath, localDocumentation.Documentation.Name + ".sdwp");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                throw new FileNotFoundException("Файл документации не найден");
            }
        }

        public async Task<List<LocalDocumentation>> GetLocalDocumentations(string folderPath)
        {
            return await Task.Run(() =>
            {
                string[] filePaths = Directory.GetFiles(folderPath);
                List<LocalDocumentation> localDocumentations = new List<LocalDocumentation>();

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        byte[] fileBytes;
                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            fs.Seek(0, SeekOrigin.Begin);
                            fileBytes = new byte[fs.Length];
                            fs.Read(fileBytes, 0, (int)fs.Length);
                        }

                        string localDocJsonString = GetStringFromByteArray(fileBytes);
                        LocalDocumentation localDocumentation = JsonConvert.DeserializeObject
                            <LocalDocumentation>(localDocJsonString);

                        localDocumentations.Add(localDocumentation);
                    }
                    catch (Exception ex)
                    {
#warning Create count failed attempts count here
                    }
                }

                return localDocumentations;
            });
        }

        private string GetStringFromByteArray(byte[] arr)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetString(arr);
        }
    }
}
