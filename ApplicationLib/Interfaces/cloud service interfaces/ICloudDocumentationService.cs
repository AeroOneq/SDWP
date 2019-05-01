using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ICloudDocumentationService
    {

        Task DeleteDocumentation(Documentation entity);
        Task<IEnumerable<Documentation>> GetAllDocumentations();
        Task<IEnumerable<Documentation>> GetUserDocumentations(int id);
        Task InsertDocumentation(Documentation entity);
        Task UpdateDocumentation(Documentation entity);
    }
}
