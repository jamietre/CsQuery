using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.Utility;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine
{
    public static class AttributeSelectors
    {
        public static bool Matches(IDomElement elm, 
            AttributeSelectorType matchType, 
            string attributeName, 
            string matchValue=null)
        {
            bool match = elm.HasAttribute(attributeName);

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
                string value = elm[attributeName];

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

        public static bool Matches(IDomElement element,SelectorClause selector )
        {
            return Matches(element, selector.AttributeSelectorType, selector.AttributeName, selector.AttributeValue);
        }

        private static bool ContainsWord(string text, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return words.Contains(text);
        }
       
    }
}
