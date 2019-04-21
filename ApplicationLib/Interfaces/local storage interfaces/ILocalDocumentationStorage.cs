using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface ILocalDocumentationService : ILocalStorage
    {
        string Extension { get; }

        Task<IEnumerable<LocalDocumentation>> GetLocalDocumentations();
        Task CreateLocalDocumentationFile(LocalDocumentation localDocumentation);
        void DeleteLocalDocumentationFile(LocalDocumentation localDocumentation);
    }
}
