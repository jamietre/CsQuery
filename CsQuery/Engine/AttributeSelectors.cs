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
            //string value;
            //bool match = elm.TryGetAttribute(selector.AttributeName, out value);
            //if (!match ||
            //    (match && selector.AttributeSelectorType == AttributeSelectorType.NotExists))
            //{
            //    return false;
            //}

            bool match=true;
            string name = selector.AttributeName;
            string value;

            switch (selector.AttributeSelectorType)
            {
                case AttributeSelectorType.Exists:
                    match= elm.HasAttribute(name);
                    break;
                case AttributeSelectorType.Equals:
                    match = selector.AttributeValue == elm[name];
                    break;
                case AttributeSelectorType.StartsWith:
                    value = elm[name];
                    match = value.Length >= selector.AttributeValue.Length &&
                        value.Substring(0, selector.AttributeValue.Length) == selector.AttributeValue;
                    break;
                case AttributeSelectorType.Contains:
                    match = elm[name].IndexOf(selector.AttributeValue) >= 0;
                    break;
                case AttributeSelectorType.ContainsWord:
                    match = ContainsWord(elm[name], selector.AttributeValue);
                    break;
                case AttributeSelectorType.NotEquals:
                    match = !elm.HasAttribute(name) ||
                        !selector.AttributeValue.Equals(elm[name]);
                    break;
                case AttributeSelectorType.NotExists:
                    match = !elm.HasAttribute(name);
                    break;
                case AttributeSelectorType.EndsWith:
                    int len = selector.AttributeValue.Length;
                    value = elm[name];
                    match = value.Length >= len &&
                        value.Substring(value.Length - len) == selector.AttributeValue;
                    break;
                case AttributeSelectorType.StartsWithOrHyphen:
                    value = elm[name];
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
