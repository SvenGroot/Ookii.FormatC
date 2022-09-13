// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Management.Automation;
using System.Reflection;

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
            var target = new CodeFormatter()
            {
                FormattingInfo = new CSharpFormattingInfo()
                {
                    Types = new[] { "Program", "Console", "String" },
                }
            };

            TestFormatting(target, "csinput.txt", "csexpected.txt");
        }

        [TestMethod()]
        public void VisualBasicFormattingInfoTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new VisualBasicFormattingInfo()
                {
                    Types = new[] { "Program", "Console", "String" },
                }
            };

            TestFormatting(target, "vbinput.txt", "vbexpected.txt");
        }

        [TestMethod()]
        public void CPlusPlusFormattingInfoTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new CPlusPlusFormattingInfo(),
            };

            TestFormatting(target, "cppinput.txt", "cppexpected.txt");
        }

        [TestMethod()]
        public void XmlFormattingInfoTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new XmlFormattingInfo(),
            };

            TestFormatting(target, "xmlinput.txt", "xmlexpected.txt");
        }

        [TestMethod()]
        public void TSqlFormattingInfoTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new TSqlFormattingInfo(),
            };

            TestFormatting(target, "tsqlinput.txt", "tsqlexpected.txt");

        }

        [TestMethod()]
        public void PowerShellFormattingInfoTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new PowerShellFormattingInfo(),
            };

            TestFormatting(target, "psinput.txt", "psexpected.txt");
        }

        [TestMethod()]
        public void PowerShellFormattingInfoParserParamTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new PowerShellFormattingInfo(typeof(PSParser).Assembly),
            };

            TestFormatting(target, "psinput.txt", "psexpected.txt");

            // Try passing an assembly that doesn't have the required type.
            target.FormattingInfo = new PowerShellFormattingInfo(Assembly.GetExecutingAssembly());
            TestFormatting(target, "psinput.txt", "psfallbackexpected.txt", true);
        }

        [TestMethod()]
        public void PowerShellFormattingInfoFallbackTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new PowerShellFormattingInfo()
                {
                    ForceFallbackFormatting = true
                }
            };

            TestFormatting(target, "psinput.txt", "psfallbackexpected.txt", true);
        }

        [TestMethod]
        public void PowerShellAutomaticFallbackTest()
        {
            var target = new CodeFormatter()
            {
                FormattingInfo = new PowerShellFormattingInfo(),
            };

            Assert.IsFalse(target.UsedFallbackFormatting);

            // Unterminated array prevents PSParser from tokenizing this.
            var actual = target.FormatCode("Set-Content foo.txt @(\"foo\",\"bar\"");
            Assert.IsTrue(target.UsedFallbackFormatting);
            const string expected = "<pre class=\"code\">Set-Content foo.txt @(<span class=\"psString\">&quot;foo&quot;</span>,<span class=\"psString\">&quot;bar&quot;</span></pre>";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LineNumberTest()
        {
            var target = new CodeFormatter()
            {
                LineNumberMode = LineNumberMode.Inline,
                FormattingInfo = new CSharpFormattingInfo()
                {
                    Types = new[] { "Program", "Console", "String" },
                }
            };

            TestFormatting(target, "csinput.txt", "linenumberexpected.txt");
        }

        [TestMethod()]
        public void LineNumberTableTest()
        {
            var target = new CodeFormatter()
            {
                LineNumberMode = LineNumberMode.Table,
                FormattingInfo = new CSharpFormattingInfo()
                {
                    Types = new[] { "Program", "Console", "String" },
                }
            };

            TestFormatting(target, "csinput.txt", "linenumbertableexpected.txt");
        }

        [TestMethod()]
        public void LineNumberCustomizationTest()
        {
            var target = new CodeFormatter() 
            {
                LineNumberMode = LineNumberMode.Inline,
                LineNumberCssClass = "different",
                LineNumberFormat = "{0:000}: ",
            };
            target.FormattingInfo = new CSharpFormattingInfo();
            const string code = "int i = 5;";
            const string expected = "<pre class=\"code\"><span class=\"different\">001: </span><span class=\"keyword\">int</span> i = 5;</pre>";
            var actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CssClassTest()
        {
            var target = new CodeFormatter() { CssClass = "different" };
            target.FormattingInfo = new CSharpFormattingInfo();
            const string code = "int i = 5;";
            const string expected = "<pre class=\"different\"><span class=\"keyword\">int</span> i = 5;</pre>";
            var actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CssClassNullTest()
        {
            var target = new CodeFormatter() { CssClass = null };
            target.FormattingInfo = new CSharpFormattingInfo();
            const string code = "int i = 5;";
            const string expected = "<pre><span class=\"keyword\">int</span> i = 5;</pre>";
            var actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IncludePreElementTest()
        {
            var target = new CodeFormatter() { IncludePreElement = false };
            target.FormattingInfo = new CSharpFormattingInfo();
            const string code = "int i = 5;";
            const string expected = "<span class=\"keyword\">int</span> i = 5;";
            var actual = target.FormatCode(code);
            Assert.AreEqual(expected, actual);
        }

        private static void TestFormatting(CodeFormatter target, string inputFile, string expectedFile, bool fallback = false)
        {
            var code = File.ReadAllText(inputFile);
            var expected = File.ReadAllText(expectedFile);
            var actual = target.FormatCode(code);
            Assert.AreEqual(fallback, target.UsedFallbackFormatting);

            // Write actual output to a file so a diff tool can be used to compare it to expected.
            File.WriteAllText("actual.txt", actual);

            Assert.AreEqual(expected, actual);
        }
    }
}
