using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Database;

namespace ApplicationLib.Services
{
    class CloudTemplateService : ICloudTemplateService
    {
        public ICloudDatabase<Template> Database { get; }

        public CloudTemplateService(string connectionString)
        {
            Database = new TemplatesDB(connectionString);
        }

        public async Task DeleteTemplate(Template template)
        {
            await Database.DeleteRecord(template);
        }

        public async Task<IEnumerable<Template>> GetAllTemplates()
        {
            return await Database.GetAllRecords();
        }

        public async Task<IEnumerable<Template>> GetTemplates(string columnName, object value)
        {
            return await Database.GetRecords(columnName, value);
        }

        public async Task InsertTemplate(Template template)
        {
            await Database.InsertRecord(template);
        }

        public async Task UpdateTemplate(Template template)
        {
            await Database.UpdateRecord(template);
        }
    }
}
