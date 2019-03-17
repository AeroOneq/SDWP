using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Models
{
    public class TableCell
    {
        public string CellText { get; set; }

        public TableCell(string cellText)
        {
            CellText = cellText;
        }
    }
}
