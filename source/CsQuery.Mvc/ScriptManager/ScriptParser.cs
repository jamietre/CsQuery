using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CsQuery.Mvc
{
    class ScriptParser: IDisposable
    {
        public  ScriptParser(string fileName)
        {
            FileName = fileName;

        }

        private static Regex RegexStartComment = new Regex(@"^\s*/\*(?<comment>.*)$");
        private static Regex RegexEndComment = new Regex(@"^(?<comment>.*)\*/\s*$");
        private static Regex RegexWhiteSpace= new Regex(@"^\s*$");

        // matches "/* xxx */" or "// xxx", ignoring whitespace
        private static Regex RegexFullLineComment = new Regex(@"^\s*//(?<comment>.*)$");
        private static Regex RegexOneLineComment = new Regex(@"^\s*/\*(?<comment>.*)\*/$");

        public bool InMultilineComment { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last read line is a comment.
        /// </summary>
        ///
        /// <value>
        /// false if there are embedded comments inside code in a line, or no comment, and true only when
        /// the entire line is a commment.
        /// </value>

        public bool InComment { get; protected set; }

        private StreamReader StreamReader;
        private string _FileName;

        public string FileName { 
            get {
                return _FileName;
            }
            protected set {
                if (!File.Exists(value))
                {
                    throw new FileNotFoundException(String.Format("Dependency \"{0}\" could not be resolved in the file system.",
                        value));
                }

                _FileName = value;
                StreamReader = new StreamReader(value);
            }
        }

        /// <summary>
        /// When true, indicates that non-whitespace, non-code lines have been parsed already.
        /// </summary>
        ///
        /// <value>
        /// true if code has been found yet.
        /// </value>

        public bool AnyCodeYet { get; protected set; }

        /// <summary>
        /// Reads the next line from the file, stripping out comment markers (instead, setting the "InComment" property)
        /// and setting properties based on the contents of the line.
        /// </summary>
        ///
        /// <returns>
        /// The line.
        /// </returns>

        public string ReadLine()
        {
            string line= StreamReader.ReadLine();
            if (line != null)
            {
                if (InMultilineComment)
                {
                    
                    Match endComment = RegexEndComment.Match(line);

                    if (endComment.Success)
                    {
                        InMultilineComment = false;
                        InComment = false;
                        line = endComment.Groups["comment"].Value;
                    }
                    
                }
                else
                {
                    bool isFullLineComment = false;
                    Match singleLineComment = RegexFullLineComment.Match(line);

                    bool isSingleLineComment = singleLineComment.Success;
                    Match fullLineComment = null;

                    bool isStartComment = false;
                    Match startComment = null;

                    

                    if (!InMultilineComment) {
                        if (!isSingleLineComment)
                        {
                            fullLineComment = RegexFullLineComment.Match(line);
                            isFullLineComment = fullLineComment.Success;
                        }

                        if (!isSingleLineComment && !isFullLineComment)
                        {
                            startComment = RegexStartComment.Match(line);
                            isStartComment = startComment.Success;
                        }
                    } 

                    // set comment status flags
                    if (!isFullLineComment && isStartComment)
                    {
                        InMultilineComment = true;
                        InComment = true;
                        line = startComment.Groups["comment"].Value;
                    }
                    else if (isFullLineComment)
                    {
                        InComment = true;
                        line = fullLineComment.Groups["comment"].Value;
                    } else if (isSingleLineComment) {
                        InComment = true;
                        line = singleLineComment.Groups["comment"].Value;
                    } 
                    else if (!AnyCodeYet && !RegexWhiteSpace.IsMatch(line))
                    {
                        AnyCodeYet = true;
                    }
                }
            }
            return line;
        }

        void IDisposable.Dispose()
        {
            StreamReader.Dispose();
        }
    }
}
