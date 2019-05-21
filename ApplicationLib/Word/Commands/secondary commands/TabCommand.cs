using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using ApplicationLib.Word.Interfaces;

namespace ApplicationLib.Word.Commands
{
    public class TabCommand : IWordSecondaryCommand
    {
        public OpenXmlCompositeElement GetElement()
        {
            Run run = new Run();
            RunProperties runProperties = new RunProperties(
                new RunFonts()
                {
                    HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                    Ascii = RenderData.Obj.RenderSettings.FontFamily
                });

            run.Append(runProperties);
            run.Append(new TabChar());

            return run;
        }
    }
}
