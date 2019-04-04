using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using FileLib.FileParsers;

namespace FileLib.Tests
{
    [TestFixture]
    public class CSFileParserTest
    {
        CSFileParser csFileParser;

        [SetUp]
        public void SetUpTest()
        {
            csFileParser = new CSFileParser();
        }

        [Test]
        public void TestGetAssenmblyTables()
        {
            string filePath = @"C:\Users\Aero\Desktop\Фракталы\Stepanov_159\Fractals\bin\Debug\Graphics.dll";
            var tables = csFileParser.GetAssemblyTables(filePath, new ApplicationLib.Models.Item());

            Assert.AreEqual(19, tables.Length);
        }

        [TearDown]
        public void TearDownTest()
        {
            csFileParser = null;
        }
    }
}
