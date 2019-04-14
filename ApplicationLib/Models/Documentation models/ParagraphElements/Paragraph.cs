using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;

namespace ApplicationLib.Models 
{
    [JsonConverter(typeof(ParagraphJsonConverter))]
    public class Paragraph : IParentableParagraph
    {
        #region Properties
        public string Type { get; set; }
        public ParagraphElement ParagraphElement { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; private set; }
        [JsonIgnore]
        public List<Paragraph> ParentList { get; private set; }
        #endregion

        #region Constructors
        public Paragraph() { }
        public Paragraph(string type, ParagraphElement paragraphElement)
        {
            Type = type;
            ParagraphElement = paragraphElement;

            ParagraphElement.ParentParagraph = this;
        }
        #endregion

        public void SetParents(Item parentItem, List<Paragraph> parentList)
        {
            ParentItem = parentItem;
            ParentList = parentList;
        }

        public void RemoveParagraphFromParentList()
        {
            ParentList.Remove(this);
        }
    }
}
