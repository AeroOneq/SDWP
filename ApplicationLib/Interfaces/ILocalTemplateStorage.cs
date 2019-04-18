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
        Task<IEnumerable<LocalTemplate>> GetLocalTemplates();

        Task CreateTemplateFile(LocalTemplate template);
        void DeleteTemplateFile(LocalTemplate template);
        Task RewriteTemplateFile(LocalTemplate template);
    }
}
