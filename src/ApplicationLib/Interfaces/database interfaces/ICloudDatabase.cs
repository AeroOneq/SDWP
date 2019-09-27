using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ICloudDatabase<Type>
    {
        Task<IEnumerable<Type>> GetAllRecords();
        Task<IEnumerable<Type>> GetRecords(string columnName, object value);
        Task UpdateRecord(Type documentation);
        Task DeleteRecord(Type documentation);
        Task InsertRecord(Type documentation);
    }
}
