using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Engine;
using CsQuery.Implementation;

namespace CsQuery
{
    public partial class CQ
    {
        /// <summary>
        /// Perform a substring replace on the contents of the named attribute in each item in the
        /// selection set.
        /// </summary>
        ///
        /// <param name="name">
        /// The attribute name.
        /// </param>
        /// <param name="replaceWhat">
        /// The string to match.
        /// </param>
        /// <param name="replaceWith">
        /// The value to replace each occurrence with.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ AttrReplace(string name, string replaceWhat, string replaceWith)
        {
            foreach (IDomElement item in SelectionSet)
            {
                string val = item[name];
                if (val != null)
                {
                    item[name] = val.Replace(replaceWhat, replaceWith);
                }
            }
            return this;
        }

        /// <summary>
        /// Perform a regex replace on the contents of the named attribute in each item in the
        /// selection set.
        /// </summary>
        ///
        /// <param name="name">
        /// The attribute name.
        /// </param>
        /// <param name="pattern">
        /// The regex pattern.
        /// </param>
        /// <param name="replaceWith">
        /// The value to replace each occurrence with.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ AttrRegexReplace(string name, string pattern, string replaceWith) {
            foreach (IDomElement item in SelectionSet) {
                string val = item[name];
                if (val != null) {
                    item[name] = Regex.Replace(val, pattern, replaceWith);
                }
            }
            return this;
        }

        /// <summary>
        /// Perform a regex replace on the contents of the named attribute in each item in the
        /// selection set.
        /// </summary>
        ///
        /// <param name="name">
        /// The attribute name.
        /// </param>
        /// <param name="pattern">
        /// The regex pattern.
        /// </param>
        /// <param name="matchEvaluator">
        /// The evaluator used to replace the pattern
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ AttrRegexReplace(string name, string pattern, MatchEvaluator matchEvaluator) {
            foreach (IDomElement item in SelectionSet) {
                string val = item[name];
                if (val != null) {
                    item[name] = Regex.Replace(val, pattern, matchEvaluator);
                }
            }
            return this;
        }
    }
}
