using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ICloudDocumentationDB<Type>
    {
        Task<IEnumerable<Type>> GetAllDocumentations();
        Task<IEnumerable<Type>> GetUserDocumentations(int userID);
        Task UpdateDocumentation(Type documentation);
        Task DeleteDocumentation(int documentationID);
        Task InsertDocumentation(Type documentation);
    }
}
