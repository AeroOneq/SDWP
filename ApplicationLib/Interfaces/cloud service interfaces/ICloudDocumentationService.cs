using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ICloudDocumentationService : ICloudService<Documentation>
    {
        Task DeleteDocumentation(Documentation entity);
        Task<IEnumerable<Documentation>> GetAllDocumentations();
        Task<IEnumerable<Documentation>> GetDocumentations(string columnName, object value);
        Task InsertDocumentation(Documentation entity);
        Task UpdateDocumentation(Documentation entity);
    }
}
