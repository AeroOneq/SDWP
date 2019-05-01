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
        public ICloudTemplatesDB<Template> Database { get; }

        public CloudTemplateService()
        {
            Database = new TemplatesDB();
        }

        public async Task DeleteTemplate(Template template)
        {
            await Database.DeleteTemplate(template);
        }

        public async Task<IEnumerable<Template>> GetAllTemplates()
        {
            return await Database.GetAllTemplates();
        }

        public async Task<IEnumerable<Template>> GetUserTemplates(int userID)
        {
            return await Database.GetUserTemplates(userID);
        }

        public async Task InsertTemplate(Template template)
        {
            await Database.InsertTemplate(template);
        }

        public async Task UpdateTemplate(Template template)
        {
            await Database.UpdateTemplate(template);
        }
    }
}
