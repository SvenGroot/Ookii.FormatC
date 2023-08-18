# Ookii.FormatC

FormatC is a class library for Microsoft .Net that provides syntax highlighting for several
programming languages. FormatC includes support for C#, Visual Basic, C++, XML, Transact-SQL and
PowerShell. FormatC can be extended to support other languages if necessary.

FormatC takes as input source code in any of the supported languages, and produces HTML as output
containing the syntax highlighted source. You need to use the provided CSS file for the syntax
highlighting to work. CSS is provided for both light and dark styles, and you can customize the CSS
file to suit your own syntax highlighting preferences.

FormatC is not heavily developed, and adding support for additional languages is unlikely at this
point. However, it is still maintained because of a few features that are rare in other syntax
highlighters, such as highlighting type names in C# code and parser-based PowerShell formatting.

FormatC's syntax highlighting uses regular expressions, and as such isn't incredibly fast. It's
intended use is to add highlighting to small code snippets for inclusion on web pages, not to
highlight large files.

Formatting source code is easy. Here is an example of how to format C# code:

```csharp
var formatter = new CodeFormatter()
{
    formatter.FormattingInfo = new CSharpFormattingInfo()
};

var code = File.ReadAllText("code.cs");
var formattedCode = formatter.FormatCode(code);
```

You can also write directly to a `TextWriter`.

For more information, including CSS stylesheets required to view the highlighted output, see the
[GitHub project page](https://github.com/SvenGroot/Ookii.FormatC).
