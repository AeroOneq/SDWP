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
    class TemplatesDB : ICloudDatabase<Template>
    {
        private Connector Connector { get; }

        public TemplatesDB(string connectionString)
        {
            Connector = new Connector(connectionString);
        }

        public async Task DeleteRecord(Template template)
        {
            await Task.Run(() =>
            {
                Connector.DeleteRecord(template);
            });
        }

        public async Task<IEnumerable<Template>> GetAllRecords()
        {
            return await Task.Run(() =>
            {
                return Connector.GetAllRecords<Template>();
            });
        }

        public async Task<IEnumerable<Template>> GetRecords(string columnName, object value)
        {
            return await Task.Run(() =>
            {
                return Connector.GetRecords<Template>(columnName, value);
            });
        }

        public async Task InsertRecord(Template template)
        {
            await Task.Run(() =>
            {
                Connector.Insert(template);
            });
        }

        public async Task UpdateRecord(Template template)
        {
            await Task.Run(() =>
            {
                Connector.UpdateRecord(template);
            });
        }
    }
}
