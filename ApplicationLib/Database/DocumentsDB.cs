using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;

using AeroORMFramework;

namespace ApplicationLib.Database
{
    public class DocumentsDB : ICloudDatabase<Document>
    {
        private Connector Connector { get; }

        public DocumentsDB(string connectionString)
        {
            Connector = new Connector(connectionString);
        }

        public async Task DeleteRecord(Document entity)
        {
            await Task.Run(() =>
            {
                Connector.DeleteRecord(entity);
            });
        }

        public async Task<IEnumerable<Document>> GetAllRecords()
        {
            return await Task.Run(() =>
            {
                return Connector.GetAllRecords<Document>();
            });
        }

        public async Task<IEnumerable<Document>> GetRecords(string columnName, object value)
        {
            return await Task.Run(() =>
            {
                return Connector.GetRecords<Document>(columnName, value);
            });
        }

        public async Task InsertRecord(Document entity)
        {
            await Task.Run(() =>
            {
                Connector.Insert(entity);
            });
        }

        public async Task UpdateRecord(Document entity)
        {
            await Task.Run(() =>
            {
                Connector.UpdateRecord(entity);
            });
        }
    }
}
