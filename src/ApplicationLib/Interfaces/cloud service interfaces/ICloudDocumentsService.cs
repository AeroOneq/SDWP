using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
    public interface ICloudDocumentsService
    {
        Task DeleteDocument(Document entity);
        Task<IEnumerable<Document>> GetAllDocuments();
        Task<IEnumerable<Document>> GetDocumentationDocuments(int documentationID);
        Task InsertDocument(Document entity);
        Task UpdateDocument(Document entity);
    }
}
