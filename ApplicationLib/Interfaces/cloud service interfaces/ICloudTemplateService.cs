using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface ICloudTemplateService
    {
        Task DeleteTemplate(Template entity);
        Task<IEnumerable<Template>> GetAllTemplates();
        Task<IEnumerable<Template>> GetUserTemplates(int userID);
        Task InsertTemplate(Template entity);
        Task UpdateTemplate(Template entity);
    }
}
