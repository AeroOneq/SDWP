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
    public class LocalTemplatesService : ILocalTemplateService
    {
        public string StoragePath { get; set; }
        public int ErrorsCount { get; private set; }
        public static string TemplateExtension { get; } = ".tsdwp";

        public async Task RewriteTemplateFile(LocalTemplate localTemplate)
        {
            byte[] templateBytes = GetStringBytes(localTemplate.GetJsonString());
            string templatePath = StoragePath + "\\" + localTemplate.FileName;
           
            if (localTemplate.FileName != localTemplate.Template.TemplateName)
            {
                if (File.Exists(templatePath))
                    File.Delete(templatePath);

                templatePath = GetTemplatePath(localTemplate);
            }

            using (var fs = new FileStream(templatePath, FileMode.Create, FileAccess.Write))
            {
                fs.Seek(0, SeekOrigin.Begin);
                await fs.WriteAsync(templateBytes, 0, templateBytes.Length);
            }
        }

        public async Task CreateTemplateFile(LocalTemplate template)
        {
            byte[] templateBytes = GetStringBytes(template.GetJsonString());
            string templatePath = GetTemplatePath(template);

            using (var fs = new FileStream(templatePath, FileMode.Create, FileAccess.Write))
            {
                fs.Seek(0, SeekOrigin.Begin);
                await fs.WriteAsync(templateBytes, 0, templateBytes.Length);
            }
        }
        
        private string GetTemplatePath(LocalTemplate localTemplate)
        {
            string templatePath = Path.Combine(StoragePath, localTemplate.Template.TemplateName);

            if (File.Exists(templatePath + TemplateExtension))
            {
                int addNum = 1;

                while (File.Exists(templatePath + " " + addNum.ToString() + TemplateExtension))
                {
                    addNum++;
                }

                localTemplate.FileName = localTemplate.Template.TemplateName + " " + addNum.ToString() + TemplateExtension;
                templatePath += " " + addNum.ToString() + TemplateExtension;
            }
            else
            {
                localTemplate.FileName = localTemplate.Template.TemplateName + TemplateExtension;
                templatePath += TemplateExtension;
            }

            return templatePath;
        }

        private byte[] GetStringBytes(string str)
        {
            Encoding encoding = Encoding.UTF8;
            return encoding.GetBytes(str);
        }

        public void DeleteTemplateFile(LocalTemplate template)
        {
            string templatePath = Path.Combine(StoragePath, template.FileName);
            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
                return;
            }

            throw new FileNotFoundException("Файл шаблона не найден");
        }

        public async Task<IEnumerable<LocalTemplate>> GetLocalTemplates()
        {
            return await Task.Run(() =>
            {
                ErrorsCount = 0;
                string[] filePaths = Directory.GetFiles(StoragePath);
                List<LocalTemplate> templates = new List<LocalTemplate>();

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
                        LocalTemplate template = JsonConvert.DeserializeObject<LocalTemplate>(templateJsonString);
                        template.FileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

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
