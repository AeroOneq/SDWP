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
    internal class DocumentationDB : ICloudDatabase<Documentation>
    {
        #region Properties
        private Connector Connector { get; set; }
        #endregion

        public DocumentationDB(string connectionString)
        {
            Connector = new Connector(connectionString);
        }

        public async Task<IEnumerable<Documentation>> GetAllRecords()
        {
            return await Task.Run(() =>
            {
                List<Documentation> documentations = Connector.GetAllRecords<Documentation>();
                return documentations;
            });
        }

        public async Task<IEnumerable<Documentation>> GetRecords(string columnName, object value)
        {
            return await Task.Run(() =>
            {
                List<Documentation> documentations = Connector.GetRecords<Documentation>(columnName, value);
                return documentations;
            });
        }

        public async Task UpdateRecord(Documentation documentation)
        {
            await Task.Run(() =>
            {
                Connector.UpdateRecord(documentation);
            });
        }

        public async Task DeleteRecord(Documentation documentation)
        {
            await Task.Run(() =>
            {
                Connector.DeleteRecord(documentation);
            });
        }

        public async Task InsertRecord(Documentation documentation)
        {
            await Task.Run(() =>
            {
                Connector.Insert(documentation);
            });
        }
    }
}
