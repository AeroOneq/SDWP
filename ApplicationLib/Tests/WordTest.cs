using ApplicationLib.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Tests
{
    [TestFixture]
    public class WordTest
    {
        [Test]
        public async void TestWordTitlePage()
        {
            WordDB wordDB = new WordDB(new Models.Documentation(), new List<Models.Document>(), "");
            await wordDB.CreateWordDocument();
        }
    }
}
