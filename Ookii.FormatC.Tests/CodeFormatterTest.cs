// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using Ookii.FormatC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
namespace Ookii.FormatC.Tests
{
    
    
    /// <summary>
    ///This is a test class for CodeFormatterTest and is intended
    ///to contain all CodeFormatterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CodeFormatterTest
    {
        [TestMethod()]
        public void CSharpFormattingInfoTest()
        {
            CodeFormatter target = new CodeFormatter();
            CSharpFormattingInfo info = new CSharpFormattingInfo();
            info.Types = new string[] { "Program", "Console", "String" };
            target.FormattingInfo = info;
            string code = File.ReadAllText("csinput.txt");
            string expected = File.ReadAllText("csexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void VisualBasicFormattingInfoTest()
        {
            CodeFormatter target = new CodeFormatter();
            VisualBasicFormattingInfo info = new VisualBasicFormattingInfo();
            target.FormattingInfo = new VisualBasicFormattingInfo() { Types = new string[] { "Program", "Console", "String" } };
            string code = File.ReadAllText("vbinput.txt");
            string expected = File.ReadAllText("vbexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CPlusPlusFormattingInfoTest()
        {
            CodeFormatter target = new CodeFormatter();
            target.FormattingInfo = new CPlusPlusFormattingInfo();
            string code = File.ReadAllText("cppinput.txt");
            string expected = File.ReadAllText("cppexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void XmlFormattingInfoTest()
        {
            CodeFormatter target = new CodeFormatter();
            target.FormattingInfo = new XmlFormattingInfo();
            string code = File.ReadAllText("xmlinput.txt");
            string expected = File.ReadAllText("xmlexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TSqlFormattingInfoTest()
        {
            CodeFormatter target = new CodeFormatter();
            target.FormattingInfo = new TSqlFormattingInfo();
            string code = File.ReadAllText("tsqlinput.txt");
            string expected = File.ReadAllText("tsqlexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void PowerShellFormattingInfoTest()
        {
            CodeFormatter target = new CodeFormatter();
            target.FormattingInfo = new PowerShellFormattingInfo();
            string code = File.ReadAllText("psinput.txt");
            string expected = File.ReadAllText("psexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.IsFalse(target.UsedFallbackFormatting);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LineNumberTest()
        {
            CodeFormatter target = new CodeFormatter();
            CSharpFormattingInfo info = new CSharpFormattingInfo();
            info.Types = new string[] { "Program", "Console", "String" };
            target.FormattingInfo = info;
            target.LineNumberMode = LineNumberMode.Inline;
            string code = File.ReadAllText("csinput.txt");
            string expected = File.ReadAllText("linenumberexpected.txt");
            string actual;
            actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }
    }
}
