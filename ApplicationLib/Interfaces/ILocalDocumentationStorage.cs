using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface ILocalDocumentationStorage
    {
        string StoragePath { get; }
        Task<List<LocalDocumentation>> GetLocalDocumentations();

        Task CreateLocalDocumentationFile(LocalDocumentation localDocumentation);
        void DeleteLocalDocumentationFile(LocalDocumentation localDocumentation);
    }
}
