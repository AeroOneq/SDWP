using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface ICloudService<Type>
    {
        ICloudDatabase<Type> Database { get; }
    }
}
