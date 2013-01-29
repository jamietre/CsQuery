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

        /// <summary>
        /// A whitespace line
        /// </summary>

        public static readonly Regex WhiteSpace = new Regex(@"^\s*$");

        /// <summary>
        /// Gets a regex matching a line that is the start of a multiline comment. Will also match a full-
        /// line comment. e.g.
        ///
        ///    /* xxx
        /// </summary>

        public static readonly Regex StartComment = new Regex(@"^\s*/\*\s*(?<comment>.*?)(\*/)*$");

        /// <summary>
        /// Matches the end of a multi-line comment block e.g.
        ///    xxx */
        /// </summary>

        public static readonly Regex EndComment = new Regex(@"^(?<comment>.*)\*/\s*$");

        /// <summary>
        ///  matches a single line comment, e.g. 
        ///    // xxx
        /// ignoring whitespace. Will also eliminate any extra slashes opening the comment block.
        /// </summary>

        public static readonly Regex FullLineComment = new Regex(@"^\s*//+\s*(?<comment>.*)$");

        /// <summary>
        /// matches a multi-line comment that is opened and closed on a single line, e.g. 
        ///    /* xxx */
        /// ignoring whitespace
        /// </summary>

        public static readonly Regex OneLineComment = new Regex(@"^\s*/\*\s*(?<comment>.*)\*/$");


        /// <summary>
        /// The regular expression dependency.
        /// </summary>

        public static readonly Regex Dependency = new Regex(@"^\s*using\s+(?<dep>[%/\-A-Za-z0-9\{\}\.]+?)(\s+(?<opt>[A-Za-z0-9\./]+?))*\s*;*\s*$");

        /// <summary>
        /// Gets a regex matching a line with file options, e.g. "using-options something somethineelse"
        /// </summary>

        public static readonly Regex Options = new Regex(@"^\s*using-options\s+([A-Za-z]+\s*)+\s*;*\s*$");

        /// <summary>
        /// Regex to match file names with version info of the form x.y.z-beta (the - part optional)
        /// </summary>

        public static readonly Regex NonLiteralFilenames = new Regex(@"^.*{version}.*$");

        /// <summary>
        /// Matches a URL starting with anything://
        /// </summary>

        public static readonly Regex UriProtocol = new Regex(@"^[a-zA-Z]+:\/\/(.*)$");

        /// <summary>
        /// The file version regular expression. This is exposed just as a string - it's embedded in other regexes
        /// </summary>

        public const string FileVersionRegex = @"([0-9]+\.*)*(-[a-zA-Z][a-zA-Z0-9]*)?";






    }

}
