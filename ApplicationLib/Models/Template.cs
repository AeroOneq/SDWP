using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AeroORMFramework;

namespace ApplicationLib.Models
{
    public class Template
    {
        [PrimaryKey]
        [AutoincrementID]
        [CanBeNull(false)]
        public int ID { get; set; }
        [CanBeNull(false)]
        public int UserID { get; set; }

        [CanBeNull(false)]
        public string TemplateName { get; set; }
        [CanBeNull(false)]
        [Json]
        public DateTime CreationAt { get; set; }
        [CanBeNull(false)]
        [Json]
        public DateTime UpdatedAt { get; set; }

        [CanBeNull(false)]
        [Json]
        public List<Document> Documents { get; set; }
    }
}
