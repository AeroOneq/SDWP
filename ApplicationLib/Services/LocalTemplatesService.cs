using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;

using Newtonsoft.Json;

namespace ApplicationLib.Services
{
    internal class LocalTemplatesSevice : ILocalTemplateService
    {
        public string StoragePath { get; set; }
        public int ErrorsCount { get; private set; }
        public static string TemplateExtension { get; } = ".tsdwp";

        public async Task CreateTemplateFile(Template template)
        {
            byte[] templateBytes = GetStringBytes(template.GetJsonString());
            string templatePath = GetTemplatePath(template);

            using (var fs = new FileStream(templatePath, FileMode.Create, FileAccess.Write))
            {
                fs.Seek(0, SeekOrigin.Begin);
                await fs.WriteAsync(templateBytes, 0, templateBytes.Length);
            }
        }

        private string GetTemplatePath(Template template)
        {
            string templatePath = Path.Combine(StoragePath, template.TemplateName);

            if (File.Exists(templatePath + TemplateExtension))
            {
                int addNum = 1;

                while (File.Exists(templatePath + " " + addNum.ToString() + TemplateExtension))
                {
                    addNum++;
                }
                templatePath += " " + addNum.ToString() + TemplateExtension;
            }
            else
            {
                templatePath += TemplateExtension;
            }

            return templatePath;
        }

        private byte[] GetStringBytes(string str)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetBytes(str);
        }

        public void DeleteTemplateFile(Template template)
        {
            string templatePath = Path.Combine(StoragePath, template.TemplateName + TemplateExtension);
            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
                return;
            }

            throw new FileNotFoundException("Файл шаблона не найден");
        }

        public async Task<IEnumerable<Template>> GetLocalTemplates()
        {
            return await Task.Run(() =>
            {
                ErrorsCount = 0;
                string[] filePaths = Directory.GetFiles(StoragePath);
                List<Template> templates = new List<Template>();

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

                        string templateJsonString = GetStringFromBytes(fileBytes);
                        Template template = JsonConvert.DeserializeObject<Template>(templateJsonString);
                        templates.Add(template);
                    }
                    catch (Exception)
                    {
                        ErrorsCount++;
                    }
                }

                return templates;
            });
        }

        private string GetStringFromBytes(byte[] bytes)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetString(bytes);
        }
    }
}
