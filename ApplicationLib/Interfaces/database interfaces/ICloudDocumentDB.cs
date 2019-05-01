using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    interface ICloudDocumentDB<DocumentType>
    {
        Task<IEnumerable<DocumentType>> GetAllDocuments();
        Task<IEnumerable<DocumentType>> GetDocumentationDocuments(int documentationID);
        Task UpdateDocument(DocumentType document);
        Task DeleteDocument(DocumentType document);
        Task InsertDocument(DocumentType document);
    }
}
