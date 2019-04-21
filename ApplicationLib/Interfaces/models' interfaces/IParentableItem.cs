using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace ApplicationLib.Interfaces
{
     public interface IParentableItem
    {
        Item ParentItem { get; }
        List<Item> ParentList { get; }

        void SetParents(Item parentItem, List<Item> parentList);
    }
}
