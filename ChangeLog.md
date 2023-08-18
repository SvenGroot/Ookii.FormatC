# What's new in Ookii.FormatC

## Version 2.3.2

- [Source Link](https://github.com/dotnet/sourcelink) integration.

## Version 2.3.1

- Corrected an outdated binary in the NuGet package.

## Version 2.3

- Updated CSS stylesheet, including a new dark mode stylesheet.
- You can now write directly to a provided `TextWriter`.
- Support for nullable reference types.
- Customizable CSS class for line numbers.
- Updated with C# 10.0 keywords.
- Some bug fixes.

## Version 2.2

- Removed System.Management.Automation dependency. PowerShell formatting with the PSParser class is
  still possible, but now requires the library consumer to reference `System.Management.Automation` or
  pass the assembly to the `PowerShellFormattingInfo` class constructor (note: this removes the need
  for a separate .Net Core 3.0 package).
- Updated C# keywords for C# 9.0.
- Added support for omitting CSS class or pre element.

## Version 2.1

- Added .Net Standard 2.0 package.

## Version 2.0

- Support keywords for C# 4.0 and Visual Basic 10.0.
- Allow escaping of contextual keywords.
- Type name highlighting support for VB and C#.
- Improved Visual Basic XML literal support.
- Use System.Management.Automation.PSParser to highlight PowerShell code if possible.

## Version 1.2.1

- Fixed: XML formatter didn't support element or attribute names containing periods.

## Version 1.2

- Added support for escaped keywords in C# and VB.
- Added support for PowerShell and T-SQL.
- Improved string handling in C# and C++.
- Improved XML formatter.
- Added support for line numbers.

## Version 1.1

- Support for VB 9.0.
- Support for C# 3.0.

## Version 1.0

- Initial release.
