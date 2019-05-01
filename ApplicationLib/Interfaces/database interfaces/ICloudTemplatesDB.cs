using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ICloudTemplatesDB<TemplateType>
    {
        Task<IEnumerable<TemplateType>> GetAllTemplates();
        Task<IEnumerable<TemplateType>> GetUserTemplates(int userID);
        Task UpdateTemplate(TemplateType template);
        Task DeleteTemplate(TemplateType template);
        Task InsertTemplate(TemplateType template);
    }
}
