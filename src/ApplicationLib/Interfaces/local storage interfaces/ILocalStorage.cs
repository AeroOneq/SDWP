using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ILocalStorage
    {
        string StoragePath { get; set; }
        int ErrorsCount { get; }
    }
}
