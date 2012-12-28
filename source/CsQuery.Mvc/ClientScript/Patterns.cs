using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// A set of RegexPatterns used to parse JavaScript files
    /// </summary>
    public static class Patterns
    {
        private static Regex RegexStartComment = new Regex(@"^\s*/\*(?<comment>.*?)(\*/)*$");

        /// <summary>
        /// The regular expression end comment.
        /// </summary>

        private static Regex RegexEndComment = new Regex(@"^(?<comment>.*)\*/\s*$");
        private static Regex RegexWhiteSpace = new Regex(@"^\s*$");

        // matches "/* xxx */" or "// xxx", ignoring whitespace
        private static Regex RegexFullLineComment = new Regex(@"^\s*//(?<comment>.*)$");
        private static Regex RegexOneLineComment = new Regex(@"^\s*/\*(?<comment>.*)\*/$");

        // match "using arg1 arg2;" ignoring whitespace around ;
        private static Regex RegexDependency = new Regex(@"^\s*using\s+(?<dep>[%/\-A-Za-z0-9\{\}\.]+?)(\s+(?<opt>[A-Za-z0-9\./]+?))*\s*;*\s*$");
        
        // match "using-options aaaa bbbb xxxx;"
        private static Regex RegexOptions = new Regex(@"^\s*using-options\s+([A-Za-z]+\s*)+\s*;*\s*$");

        /// <summary>
        /// Regex to match file names with version info of the form x.y.z-beta (the - part optional)
        /// </summary>

        private static Regex RegexNonLiteralFilenames = new Regex(@"^.*{version}.*$");

        /// <summary>
        /// The file version regular expression. This is exposed just as a string - it's embedded in other regexes
        /// </summary>

        public const string FileVersionRegex = @"([0-9]+\.*)*(-[a-zA-Z][a-zA-Z0-9]*)?";

        /// <summary>
        /// Regex to match file names with version info of the form "x.y.z-beta" (the - part optional)
        /// </summary>

        public static Regex NonLiteralFilenames
        {
            get
            {
                return RegexNonLiteralFilenames;
            }
        }
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
        /// Gets a regex matching a line with file options, e.g. "using-options something somethineelse"
        /// </summary>
        ///
        /// <value>
        /// A regex
        /// </value>

        public static Regex Options
        {
            get
            {
                return RegexOptions;
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
        /// Gets a regex for a full-line comment, e.g. // this is a comment
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
        /// Gets the one-line comment, e.g. /* this is a comment */
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
