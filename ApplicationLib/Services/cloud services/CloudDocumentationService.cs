using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Database;

using AeroORMFramework;

namespace ApplicationLib.Services
{
    internal class CloudDocumentationService : ICloudDocumentationService
    {
        public ICloudDatabase<Documentation> Database { get; }

        public CloudDocumentationService(string connectionString)
        {
            Database = new DocumentationDB(connectionString);
        }

        public async Task DeleteDocumentation(Documentation entity) =>
            await Database.DeleteRecord(entity);

        public async Task<IEnumerable<Documentation>> GetAllDocumentations() =>
            await Database.GetAllRecords();

        public async Task<IEnumerable<Documentation>> GetDocumentations(string columnName, object value) =>
            await Database.GetRecords(columnName, value);

        public async Task InsertDocumentation(Documentation entity) =>
            await Database.InsertRecord(entity);

        public async Task UpdateDocumentation(Documentation entity) =>
            await Database.UpdateRecord(entity);
    }
}
