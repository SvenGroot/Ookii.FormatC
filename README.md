# FormatC 2.0

Copyright © Sven Groot (Ookii.org)

FormatC is a class library for Microsoft .Net that provides syntax
highlighting for several programming languages. FormatC includes support
for C#, Visual Basic, C++, XML, Transact-SQL and PowerShell. FormatC can
be extended to support other languages if necessary.

FormatC takes as input source code in any of the supported languages, and
produces HTML as output containing the syntax highlighted source. You need
to use the provided CSS file for the syntax highlighting to work. You can
customize the the CSS file to suit your own syntax highlighting preferences.

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

You can specify identifiers that should be colored as type names using the
Types property of both the CSharpFormattingInfo and VisualBasicFormattingInfo
classes. These identifiers will then always be formatted as type names (even
in contexts where they are not). Like with contextual keywords, you can prefix
an identifier with ` to prevent it from being highlighted as a type name.

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

For information on how to use FormatC in your application, and how to extend
it, please refer to the included library documentation.
