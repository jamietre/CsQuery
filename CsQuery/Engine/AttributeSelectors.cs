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
        public static bool MatchesAttribute(Selector selector, IDomElement elm)
        {
            string value;
            bool match = elm.TryGetAttribute(selector.AttributeName, out value);
            if (!match ||
                (match && selector.AttributeSelectorType.IsOneOf(AttributeSelectorType.NotExists, AttributeSelectorType.NotEquals)))
            {
                return false;
            }

            switch (selector.AttributeSelectorType)
            {
                case AttributeSelectorType.Exists:
                    break;
                case AttributeSelectorType.Equals:
                    match = selector.AttributeValue == value;
                    break;
                case AttributeSelectorType.StartsWith:
                    match = value.Length >= selector.AttributeValue.Length &&
                        value.Substring(0, selector.AttributeValue.Length) == selector.AttributeValue;
                    break;
                case AttributeSelectorType.Contains:
                    match = value.IndexOf(selector.AttributeValue) >= 0;
                    break;
                case AttributeSelectorType.ContainsWord:
                    match = ContainsWord(value, selector.AttributeValue);
                    break;
                case AttributeSelectorType.NotEquals:
                    match = value.IndexOf(selector.AttributeValue) == 0;
                    break;
                case AttributeSelectorType.EndsWith:
                    int len = selector.AttributeValue.Length;
                    match = value.Length >= len &&
                        value.Substring(value.Length - len) == selector.AttributeValue;
                    break;
                case AttributeSelectorType.StartsWithOrHyphen:
                    int dashPos = value.IndexOf("-");
                    string beforeDash = value;
  
                    if (dashPos >= 0)
                    {
                        // match a dash that's included in the match attribute according to common browser behavior
                        beforeDash = value.Substring(0, dashPos);
                    }

                    match = selector.AttributeValue == beforeDash || selector.AttributeValue == value;
                    break;
                default:
                    throw new InvalidOperationException("No AttributeSelectorType set");
            }

            return match;
        }

        private static bool ContainsWord(string text, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return words.Contains(text);
        }
       
    }
}
