using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IParentableParagraph
    {
        List<IParagraphElement> ParentList { get; }
        Item ParentItem { get; }

        void SetParents(Item parentItem, List<IParagraphElement> parentList);
    }
}
