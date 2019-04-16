using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface ILocalTemplateService: ILocalStorage
    {
        Task<IEnumerable<Template>> GetLocalTemplates();

        Task CreateTemplateFile(Template template);
        void DeleteTemplateFile(Template template);
    }
}
