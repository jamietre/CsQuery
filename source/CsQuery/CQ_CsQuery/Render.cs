using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;
using CsQuery.Output;

namespace CsQuery
{
    public partial class CQ
    {
        /// <summary>
        /// Renders just the selection set completely.
        /// </summary>
        ///
        /// <remarks>
        /// This method will only render the HTML for elements in the current selection set. To render
        /// the entire document for output, use the Render method.
        /// </remarks>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string RenderSelection()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDomObject elm in this)
            {
                sb.Append(elm.Render());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Renders the document to a string.
        /// </summary>
        ///
        /// <remarks>
        /// This method renders the entire document, regardless of the current selection. This is the
        /// primary method used for rendering the final HTML of a document after manipulation; it
        /// includes the &lt;doctype&gt; and &lt;html&gt; nodes.
        /// </remarks>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string Render()
        {
            return Document.Render(Document.DomRenderingOptions);
        }

        /// <summary>
        /// Render the complete DOM with specific options.
        /// </summary>
        ///
        /// <param name="renderingOptions">
        /// The options flags in effect.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML
        /// </returns>

        public string Render(DomRenderingOptions options = DomRenderingOptions.Default)
        {
            return Document.Render(options);
        }


        /// <summary>
        /// Render the entire document, parsed through a formatter passed using the parameter.
        /// </summary>
        ///
        /// <remarks>
        /// CsQuery by default does not format the output at all, but rather returns exactly the same
        /// contents of each element from the source, including all extra whitespace. If you want to
        /// produce output that is formatted in a specific way, you can create an OutputFormatter for
        /// this purpose. The included <see cref="T:CsQuery.OutputFormatters.FormatPlainText"/> does some
        /// basic formatting by removing extra whitespace and adding newlines in a few useful places.
        /// (This formatter is pretty basic). A formatter to perform indenting to create human-readable
        /// output would be useful and will be included in some future release.
        /// </remarks>
        ///
        /// <param name="format">
        /// An object that parses a CQ object and returns a string of HTML.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string Render(IOutputFormatter format)
        {
            StringBuilder sb= new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            format.Format(this,writer);
            return sb.ToString();
        }


        /// <summary>
        /// Render the entire document, parsed through a formatter passed using the parameter, with the
        /// specified options.
        /// </summary>
        ///
        /// <param name="formatter">
        /// The formatter.
        /// </param>
        /// <param name="renderingOptions">
        /// The options flags in effect.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string Render(IOutputFormatter formatter, DomRenderingOptions options)
        {
            return Render(formatter,options);
        }

        /// <summary>
        /// Render the entire document, parsed through a formatter passed using the parameter, with the
        /// specified options.
        /// </summary>
        ///
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="options">
        /// (optional) options for controlling the operation.
        /// </param>

        public void Render(StringBuilder sb, DomRenderingOptions options = DomRenderingOptions.Default)
        {
            Document.Render(sb, options);
        }


        public void Render(TextWriter writer, DomRenderingOptions options = DomRenderingOptions.Default)
        {
            Document.Render(writer, options);
        }
    }
}
