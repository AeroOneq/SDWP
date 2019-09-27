using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word
{
    public class RenderData
    {
        public RenderSettings RenderSettings { get; private set; }
        public Document Document { get; private set; }
        public Documentation Documentation { get; private set; }
        //id for numbered lists in the document
        public int CurrentNumID { get; set; } = 0;

        #region Singleton 
        private static RenderData renderData;
        public static RenderData Obj
        {
            get
            {
                if (renderData == null)
                    renderData = new RenderData();

                return renderData;
            }
        }
        #endregion

        #region Constructors
        private RenderData() { }
        #endregion

        public static void UpdateData(RenderSettings renderSettings, Document document,
            Documentation documentation)
        {
            Obj.RenderSettings = new RenderSettings(renderSettings);
            Obj.Document = document;
            Obj.Documentation = documentation;
            Obj.CurrentNumID = 0;

            Obj.RenderSettings.TitlePageTable[1][4] = Obj.Documentation.ProjectCode;
            Obj.RenderSettings.FooterTable[2][0] = Obj.Documentation.ProjectCode;
            Obj.RenderSettings.DefaultTextSize = (int.Parse(Obj.RenderSettings.DefaultTextSize) 
                    * 2).ToString();
        }
    }
}
