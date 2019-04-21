using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface ICloudTemplateService : ICloudService<Template>
    {
        Task DeleteTemplate(Template entity);
        Task<IEnumerable<Template>> GetAllTemplates();
        Task<IEnumerable<Template>> GetTemplates(string columnName, object value);
        Task InsertTemplate(Template entity);
        Task UpdateTemplate(Template entity);
    }
}
