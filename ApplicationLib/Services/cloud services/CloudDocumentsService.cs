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
    class CloudDocumentsService : ICloudDocumentsService
    {
        public ICloudDatabase<Document> Database { get; }

        public CloudDocumentsService(string connectionString)
        {
            Database = new DocumentsDB(connectionString);
        }

        public async Task DeleteDocument(Document document)
        {
            await Database.DeleteRecord(document);
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            return await Database.GetAllRecords();
        }

        public async Task<IEnumerable<Document>> GetDocuments(string columnName, object value)
        {
            return await Database.GetRecords(columnName, value);
        }

        public async Task InsertDocument(Document document)
        {
            await Database.InsertRecord(document);
        }

        public async Task UpdateDocument(Document document)
        {
            await Database.UpdateRecord(document);
        }
    }
}
