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
        public ICloudDocumentationDB<Documentation> Database { get; }

        public CloudDocumentationService()
        {
            Database = new DocumentationDB();
        }

        public async Task DeleteDocumentation(Documentation entity) =>
            await Database.DeleteDocumentation(entity.ID);

        public async Task<IEnumerable<Documentation>> GetAllDocumentations() =>
            await Database.GetAllDocumentations();

        public async Task<IEnumerable<Documentation>> GetUserDocumentations(int userID) =>
            await Database.GetUserDocumentations(userID);

        public async Task InsertDocumentation(Documentation entity) =>
            await Database.InsertDocumentation(entity);

        public async Task UpdateDocumentation(Documentation entity) =>
            await Database.UpdateDocumentation(entity);
    }
}
