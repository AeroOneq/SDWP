using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word
{
    public class RenderSettings
    {
        #region Properties
        public string FontFamily { get; set; }
        public bool AddFooter { get; set; }
        public bool AddHeader { get; set; }
        public string DefaultTextSize { get; set; }
        public string DefaultColor { get; set; }
        public bool AddLeftTable { get; set; }
        public bool AddTitlePage { get; set; }
        public bool AddSecondPage { get; set; }
        public string FolderPath { get; set; }
        #endregion

        #region Word render constants
        public double TabValue { get; } = 500;
        public string[][] FooterTable { get; } = new string[4][]
        {
            new string[5]{ "", "", "", "", "" },
            new string[5]{ "Изм", "Лист.", "№ Документа", "Подпись", "Дата"},
            new string[5]{ "RU.17701729.04.03-01 ТЗ", "", "", "", "" },
            new string[5]{ "Инв. № подл.", "Подп. и дата", "Взам. Инв №", "Инв. № дубл", "Подп и дата"}
        };
        public string[][] TitlePageTable { get; } = new string[2][]
        {
            new string[5]{"Инв. № подл", "Подп и дата", "Взаим инв №", "Инв № дубл", "Подп и дата"}.Reverse().ToArray(),
            new string[5]{ "RU.17701729.04.03-01 ТЗ", "", "", "", "" }.Reverse().ToArray()
        };
        #endregion

        #region Constructors
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
        #endregion
    }
}
