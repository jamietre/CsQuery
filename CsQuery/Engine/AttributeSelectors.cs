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


        public static bool MatchesAttribute(SelectorClause selector, IDomElement elm)
        {
            bool match = elm.HasAttribute(selector.AttributeName);
            
            if (!match)
            {
                if (selector.SelectorType.HasFlag(SelectorType.AttributeExists))
                {
                    return false;
                }
                switch (selector.AttributeSelectorType)
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
                string value = elm[selector.AttributeName];

                switch (selector.AttributeSelectorType)
                {
                    case AttributeSelectorType.Exists:
                        return true;
                    case AttributeSelectorType.Equals:
                        return selector.AttributeValue == value;
                    case AttributeSelectorType.StartsWith:
                        return value != null &&
                            value.Length >= selector.AttributeValue.Length &&
                            value.Substring(0, selector.AttributeValue.Length) == selector.AttributeValue;
                    case AttributeSelectorType.Contains:
                        return value != null && value.IndexOf(selector.AttributeValue) >= 0;

                    case AttributeSelectorType.ContainsWord:
                        return value != null && ContainsWord(value, selector.AttributeValue);
                    case AttributeSelectorType.NotEquals:
                        return !selector.AttributeValue.Equals(value);
                    case AttributeSelectorType.NotExists:
                        return false;
                    case AttributeSelectorType.EndsWith:
                        int len = selector.AttributeValue.Length;
                        return value!=null && value.Length >= len &&
                            value.Substring(value.Length - len) == selector.AttributeValue;
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

                        return selector.AttributeValue == beforeDash || selector.AttributeValue == value;
                    default:
                        throw new InvalidOperationException("No AttributeSelectorType set");

                }

            }
        }

        private static bool ContainsWord(string text, string word)
        {
            HashSet<string> words = new HashSet<string>(word.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return words.Contains(text);
        }
       
    }
}
