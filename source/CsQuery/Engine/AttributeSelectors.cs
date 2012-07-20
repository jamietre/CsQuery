using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.HtmlParser;

namespace CsQuery.Engine
{
    /// <summary>
    /// Helper methods to perform matching against attribute-type selectors
    /// </summary>

    public static class AttributeSelectors
    {
        /// <summary>
        /// Test whether a single element matches a specific attribute selector
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown when an unknown AttributeSelectorType is passed
        /// </exception>
        ///
        /// <param name="element">
        /// The element to test
        /// </param>
        /// <param name="matchType">
        /// The attribute selector match type
        /// </param>
        /// <param name="attributeName">
        /// Name of the attribute.
        /// </param>
        /// <param name="matchValue">
        /// (optional) the value to match against, for selector types that test for certain values
        /// </param>
        ///
        /// <returns>
        /// true if the element matches, false if not.
        /// </returns>

        public static bool Matches(IDomElement element, 
            AttributeSelectorType matchType, 
            string attributeName, 
            string matchValue=null)
        {
            bool match = element.HasAttribute(attributeName);

            if (!match)
            {
                switch (matchType)
                {
                    case AttributeSelectorType.Exists:
                        return false;
                    case AttributeSelectorType.NotEquals:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                string value = element[attributeName];

                switch (matchType)
                {
                    case AttributeSelectorType.Exists:
                        return true;
                    case AttributeSelectorType.Equals:
                        return matchValue == value;
                    case AttributeSelectorType.StartsWith:
                        return value != null &&
                            value.Length >= matchValue.Length &&
                            value.Substring(0, matchValue.Length) == matchValue;
                    case AttributeSelectorType.Contains:
                        return value != null && value.IndexOf(matchValue) >= 0;

                    case AttributeSelectorType.ContainsWord:
                        return value != null && ContainsWord(value, matchValue);
                    case AttributeSelectorType.NotEquals:
                        return !matchValue.Equals(value);
                    case AttributeSelectorType.NotExists:
                        return false;
                    case AttributeSelectorType.EndsWith:
                        int len = matchValue.Length;
                        return value != null && value.Length >= len &&
                            value.Substring(value.Length - len) == matchValue;
                    case AttributeSelectorType.StartsWithOrHyphen:
                        if (value == null)
                        {
                            return false;
                        }
                        int dashPos = value.IndexOf("-");
                        string beforeDash = value;

                        if (dashPos >= 0)
                        {
                            // match a dash that's included in the match attribute according to common browser behavior
                            beforeDash = value.Substring(0, dashPos);
                        }

                        return matchValue == beforeDash || matchValue == value;
                    default:
                        throw new InvalidOperationException("No AttributeSelectorType set");

                }

            }
        }

        /// <summary>
        /// Test whether a single element matches a specific attribute selector.
        /// </summary>
        ///
        /// <param name="element">
        /// The element to test.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        ///
        /// <returns>
        /// true if the element matches, false if not.
        /// </returns>

        public static bool Matches(IDomElement element, SelectorClause selector)
        {
            return Matches(element, selector.AttributeSelectorType, selector.AttributeName, selector.AttributeValue);
        }

        /// <summary>
        /// Test whether a sentence contains a word
        /// </summary>
        ///
        /// <param name="sentence">
        /// The sentence.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        ///
        /// <returns>
        /// true if it contains the word, false if not.
        /// </returns>

        private static bool ContainsWord(string sentence, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(CharacterData.charsHtmlSpaceArray, 
                StringSplitOptions.RemoveEmptyEntries));

            return words.Contains(sentence);
        }
       
    }
}
