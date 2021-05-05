# FormatC 2.2

FormatC is a class library for Microsoft .Net that provides syntax
highlighting for several programming languages. FormatC includes support
for C#, Visual Basic, C++, XML, Transact-SQL and PowerShell. FormatC can
be extended to support other languages if necessary.

FormatC takes as input source code in any of the supported languages, and
produces HTML as output containing the syntax highlighted source. You need
to use the provided CSS file for the syntax highlighting to work. You can
customize the the CSS file to suit your own syntax highlighting preferences.

FormatC is not heavily developed, and adding support for additional languages
is unlikely at this point. However, it is still maintained because of a few
features that are rare in other syntax highlighters, such as highlighting
type names in C# code and parser-based PowerShell formatting.

To use FormatC in your application, use the [NuGet package](https://www.nuget.org/packages/Ookii.FormatC/).
FormatC is compatible with .Net Framework 4.5 and up, and .Net Standard 2.0.

Formatting source code is easy. Here is an example of how to format C# code:

```csharp
var formatter = new CodeFormatter();
formatter.FormattingInfo = new CSharpFormattingInfo();
string code = File.ReadAllText("code.cs");
string formattedCode = formatter.FormatCode(code);
```

For more usage information, please refer to the [class library documentation](http://www.ookii.org/Link/FormatCDoc).

The generated output will use CSS class names for different code element. A
[default style sheet](https://github.com/SvenGroot/Ookii.FormatC/blob/master/Ookii.FormatC/code.css)
is provided.

## Contextual keywords

The C# and Visual Basic languages contain several keywords that are contextual
keywords. For example, 'from' is a keyword in a Linq expression, but not
elsewhere, and you can still have variables named 'from' without prefixing
them with @ (in C#) or enclosing them in \[] (in Visual Basic).

Because of the limitations of regular expressions, FormatC cannot determine
when a contextual keyword should be treated as a keyword and when it should
be treated as a regular identifier. Because of this, it always treats them as
keywords.

You can prefix an identifier that is also a contextual keyword with \` (e.g. \`from)
to prevent it from being highlighted as a keyword. The \` character will not
appear in the formatted output, and the identifier will not be highlighted.

## Type names

You can specify identifiers that should be colored as type names using the
`Types` property of both the `CSharpFormattingInfo` and `VisualBasicFormattingInfo`
classes. These identifiers will then always be formatted as type names (even
in contexts where they are not). Like with contextual keywords, you can prefix
an identifier with \` to prevent it from being highlighted as a type name.

## XML Literals in Visual Basic

Visual Basic XML literals are supported, however the XML literals must be
marked explicitly with with \[xml]\[/xml]. For example, this would look like
this with a simple XML literal:

```vb
Dim xmlLiteral = [xml]<Foo />[/xml]
```

The \[xml]\[/xml] tags will not be included in the output, and the contents of
those tags will be formatted as XML literals. Embedded expressions in XML
literals (which are delimited by \<%= %> blocks) are also supported, and
the contents of embedded expressions will be formatted as Visual Basic code.

## PowerShell formatting

While most formatters in FormatC use regular expressions, `PowerShellFormattingInfo`
is capable of using the actual PowerShell parser to tokenize the input. To do
this, the project that uses FormatC must reference System.Management.Automation
(starting with version 2.2, Ookii.FormatC does not reference this anymore by
itself). Alternatively, you can manually load the assembly and pass it to the
`PowerShellFormattingInfo` constructor.

If System.Management.Automation is not available, FormatC will fall back to
using regular expressions.
