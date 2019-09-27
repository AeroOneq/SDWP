using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Database;

namespace ApplicationLib.Services
{
    public class CloudDocumentsService : ICloudDocumentsService
    {
        public ICloudDocumentDB<Document> Database { get; }

        public CloudDocumentsService()
        {
            Database = new DocumentsDB();
        }

        public async Task DeleteDocument(Document document)
        {
            await Database.DeleteDocument(document);
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await Database.GetAllDocuments();
        }

        public async Task<IEnumerable<Document>> GetDocumentationDocuments(
            int documentationID)
        {
            return await Database.GetDocumentationDocuments(documentationID);
        }

        public async Task InsertDocument(Document document)
        {
            await Database.InsertDocument(document);
        }

        public async Task UpdateDocument(Document document)
        {
            await Database.UpdateDocument(document);
        }
    }
}
