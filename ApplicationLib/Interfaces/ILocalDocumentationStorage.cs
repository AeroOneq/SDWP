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
        Task<List<LocalDocumentation>> GetLocalDocumentations(string folderPath);

        Task CreateLocalDocumentationFile(LocalDocumentation localDocumentation, string folderPath);
        void DeleteLocalDocumentationFile(LocalDocumentation localDocumentation);
    }
}
