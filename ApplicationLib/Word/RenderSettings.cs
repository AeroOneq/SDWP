using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word
{
    public class RenderSettings
    {
        public string FontFamily { get; set; }
        public bool AddFooter { get; set; }
        public bool AddHeader { get; set; }
        public string DefaultTextSize { get; set; }
        public string DefaultColor { get; set; }
        public bool AddLeftTable { get; set; }
        public bool AddTitlePage { get; set; }
        public bool AddSecondPage { get; set; }
        public string FolderPath { get; set; }

        public RenderSettings() { }
        public RenderSettings(RenderSettings settings)
        {
            FontFamily = settings.FontFamily;
            AddFooter = settings.AddFooter;
            AddHeader = settings.AddHeader;
            DefaultTextSize = settings.DefaultTextSize;
            DefaultColor = settings.DefaultColor;
            AddLeftTable = settings.AddLeftTable;
            AddTitlePage = settings.AddTitlePage;
            AddSecondPage = settings.AddSecondPage;
            FolderPath = settings.FolderPath;
        }
    }
}
