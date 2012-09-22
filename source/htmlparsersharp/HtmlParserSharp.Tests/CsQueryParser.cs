using System;
using System.IO;
using HtmlParserSharp.Core;
using CsQuery;

namespace HtmlParserSharp
{
	/// <summary>
	/// This is a simple API for the parsing process.
	/// Part of this is a port of the nu.validator.htmlparser.io.Driver class.
	/// The parser currently ignores the encoding in the html source and parses everything as UTF-8.
	/// </summary>
	public class CsQueryParser
	{
		private Tokenizer tokenizer;
		private DomTreeBuilder treeBuilder;

		public IDomFragment ParseStringFragment(string str, string fragmentContext)
		{
			using (var reader = new StringReader(str))
				return ParseFragment(reader, fragmentContext);
		}

		public IDomDocument ParseString(string str)
		{
			using (var reader = new StringReader(str))
				return Parse(reader);
		}

        public IDomDocument Parse(string path)
		{
			using (var reader = new StreamReader(path))
				return Parse(reader);
		}

        public IDomDocument Parse(TextReader reader)
		{
			Reset();
			Tokenize(reader);
			return treeBuilder.Document;
		}

		public IDomFragment ParseFragment(TextReader reader, string fragmentContext)
		{
			Reset();
			treeBuilder.SetFragmentContext(fragmentContext);
			Tokenize(reader);
			return treeBuilder.getDocumentFragment();
		}

		private void Reset()
		{
			treeBuilder = new DomTreeBuilder();
			tokenizer = new Tokenizer(treeBuilder, false);
			treeBuilder.IsIgnoringComments = false;

			// optionally: report errors and more

			//treeBuilder.ErrorEvent +=
			//    (sender, a) =>
			//    {
			//        ILocator loc = tokenizer as ILocator;
			//        Console.WriteLine("{0}: {1} (Line: {2})", a.IsWarning ? "Warning" : "Error", a.Message, loc.LineNumber);
			//    };
			//treeBuilder.DocumentModeDetected += (sender, a) => Console.WriteLine("Document mode: " + a.Mode.ToString());
			//tokenizer.EncodingDeclared += (sender, a) => Console.WriteLine("Encoding: " + a.Encoding + " (currently ignored)");
		}

		private void Tokenize(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader was null.");
			}

			tokenizer.Start();
			bool swallowBom = true;

			try
			{
				char[] buffer = new char[2048];
				UTF16Buffer bufr = new UTF16Buffer(buffer, 0, 0);
				bool lastWasCR = false;
				int len = -1;
				if ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
				{
					int streamOffset = 0;
					int offset = 0;
					int length = len;
					if (swallowBom)
					{
						if (buffer[0] == '\uFEFF')
						{
							streamOffset = -1;
							offset = 1;
							length--;
						}
					}
					if (length > 0)
					{
						tokenizer.SetTransitionBaseOffset(streamOffset);
						bufr.Start = offset;
						bufr.End = offset + length;
						while (bufr.HasMore)
						{
							bufr.Adjust(lastWasCR);
							lastWasCR = false;
							if (bufr.HasMore)
							{
								lastWasCR = tokenizer.TokenizeBuffer(bufr);
							}
						}
					}
					streamOffset = length;
					while ((len = reader.Read(buffer, 0, buffer.Length)) != 0)
					{
						tokenizer.SetTransitionBaseOffset(streamOffset);
						bufr.Start = 0;
						bufr.End = len;
						while (bufr.HasMore)
						{
							bufr.Adjust(lastWasCR);
							lastWasCR = false;
							if (bufr.HasMore)
							{
								lastWasCR = tokenizer.TokenizeBuffer(bufr);
							}
						}
						streamOffset += len;
					}
				}
				tokenizer.Eof();
			}
			finally
			{
				tokenizer.End();
			}
		}
	}
}
