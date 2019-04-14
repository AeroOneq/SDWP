﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class ParagraphElement : IParagraphElement
    { 
        public string Hint { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public Paragraph ParentParagraph { get; set; }

        public virtual UserControl GetWatchView()
        {
            return null;
        }
        public virtual UserControl GetEditView()
        {
            return null;
        }
    }
}
