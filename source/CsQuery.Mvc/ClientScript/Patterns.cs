using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CsQuery.Mvc.ClientScript
{
    public static class Patterns
    {
        private static Regex RegexStartComment = new Regex(@"^\s*/\*(?<comment>.*)$");
        private static Regex RegexEndComment = new Regex(@"^(?<comment>.*)\*/\s*$");
        private static Regex RegexWhiteSpace = new Regex(@"^\s*$");

        // matches "/* xxx */" or "// xxx", ignoring whitespace
        private static Regex RegexFullLineComment = new Regex(@"^\s*//(?<comment>.*)$");
        private static Regex RegexOneLineComment = new Regex(@"^\s*/\*(?<comment>.*)\*/$");

        private static Regex RegexDependency = new Regex(@"^\s*using (?<dep>[a-zA-Z0-9\-\.]+)\s*;*\s*$");

        /// <summary>
        /// Gets a regex matching a line that signfies a dependency, e.g. "using something"
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex Dependency
        {
            get
            {
                return RegexDependency;
            }
        }

        /// <summary>
        /// Gets a regex matching a line that is the start of a multiline comment. Will also match a full-
        /// line comment.
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex StartComment
        {
            get
            {
                return RegexStartComment;
            }
        }

        /// <summary>
        /// Gets a regex matching a line that is the end of a multiline comment. Will also match a full-
        /// line comment.
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex EndComment
        {
            get
            {
                return RegexEndComment;
            }
        }

        /// <summary>
        /// Gets a regex matching a line with only whitespace
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex WhiteSpace
        {
            get
            {
                return RegexWhiteSpace;
            }
        }

        /// <summary>
        /// Gets a regex for a one-line comment, e.g. // this is a comment
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex FullLineComment
        {
            get
            {
                return RegexFullLineComment;
            }
        }


        /// <summary>
        /// Gets the full line comment, e.g. /* this is a comment */
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex OneLineComment
        {
            get
            {
                return RegexOneLineComment;
            }
        }
    }

}
