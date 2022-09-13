// Copyright © Sven Groot (Ookii.org)
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;

namespace Ookii.FormatC
{
    /// <summary>
    /// Represents an element of source code, such as keywords, comments or strings, and the regular expression
    /// that can be used to identify them.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public class CodeElement
    {
        private readonly string _name;
        private readonly string _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeElement" /> class with the specified name and regular
        /// expression.
        /// </summary>
        /// <param name="name">The name of this pattern. This name will be used as the CSS class name in the generated HTML.</param>
        /// <param name="regex">The regular expression for this code element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" /> or <paramref name="regex"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
        public CodeElement(string name, string regex)
        {
            if( name == null )
                throw new ArgumentNullException(nameof(name));
            if( regex == null )
                throw new ArgumentNullException(nameof(regex));
            if( name.Length == 0 )
                throw new ArgumentException(Properties.Resources.Error_NameEmptyString);
            ElementNameIsCssClass = true;
            _name = name;
            _regex = CreatePatternFromString(name, regex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeElement" /> class with the specified name and list of values.
        /// </summary>
        /// <param name="name">The name of this code element. This name will be used as the CSS class name for the generated HTML
        /// elements.</param>
        /// <param name="values">A list of identifiers that this code element should match.</param>
        /// <remarks>This constructor automatically creates a pattern to match the specified values.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" /> or <paramref name="values"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
        public CodeElement(string name, IEnumerable<string> values)
        {
            if( name == null )
                throw new ArgumentNullException(nameof(name));
            if( values == null )
                throw new ArgumentNullException(nameof(values));
            if( name.Length == 0 )
                throw new ArgumentException(Properties.Resources.Error_NameEmptyString);
            ElementNameIsCssClass = true;
            _name = name;
            _regex = CreatePatternFromValues(name, values);
        }

        /// <summary>
        /// Gets the name of this code element.
        /// </summary>
        /// <value>
        /// The name of this element.
        /// </value>
        /// <remarks>
        /// This name will be used as the CSS class name for the generated HTML elements.
        /// </remarks>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the regular expression for this code element.
        /// </summary>
        /// <value>
        /// The regular expression used to identify this code element. The regular expression will match the element in a group
        /// with the name in the <see cref="Name"/> property.
        /// </value>
        public string Regex
        {
            get
            {
                return _regex;
            }
        }

        /// <summary>
        /// Gets or sets a delegate that will be used to process the value of the regex match.
        /// </summary>
        /// <value>A delegate that will be used to process the value of the regex match.</value>
        /// <remarks>
        /// <para>
        ///   Use this property if you want to change the text value that is added to the formatted result.
        /// </para>
        /// </remarks>
        public Func<string, string>? MatchValueProcessor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Name"/> of this <see cref="CodeElement"/> is a CSS class to be used in the output.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if the <see cref="Name"/> of this <see cref="CodeElement"/> is a CSS class to be used in the output; otherwise, <see langword="false"/>.
        /// 	The default value is <see langword="true"/>.
        /// </value>
        public bool ElementNameIsCssClass { get; set; }

        private static string CreatePatternFromValues(string name, IEnumerable<string> values)
        {
            StringBuilder result = new StringBuilder();
            result.Append(@"((?<=(^|\W))(?<");
            result.Append(name);
            result.Append(@">");
            bool first = true;
            foreach( string item in values )
            {
                if( !first )
                    result.Append("|");
                else
                    first = false;
                result.Append(item);
            }
            result.Append(@")(?=(\W|$)))");
            return result.ToString();
        }

        private static string CreatePatternFromString(string name, string pattern)
        {
            StringBuilder result = new StringBuilder();
            result.Append(@"(?<");
            result.Append(name);
            result.Append(@">");
            result.Append(pattern);
            result.Append(@")");
            return result.ToString();
        }
    }
}
