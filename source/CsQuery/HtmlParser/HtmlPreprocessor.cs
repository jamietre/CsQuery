using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsQuery.HtmlParser;

namespace CsQuery.HtmlParser
{
    ///// <summary>
    ///// Functions for preprocessing HTML.
    ///// </summary>

    //public class HtmlPreprocessor
    //{

    //    private static Regex SelfClosingTag = new Regex(@"<([:A-Z_a-z\xC0-\xD6\xD8-\xF6\xF8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD][-.0-9\xB7\u0300-\u036F\u0203F-\u2040]*)([^>]*?)\s*?\/>");

    //    public static string ExpandSelfClosingTags(string html)
    //    {
    //        return SelfClosingTag.Replace(html, match=>{
    //            var keyword = match.Groups[1].Value;
    //            var data = match.Groups.Count > 1 ? match.Groups[2].Value : "";

    //            if (HtmlData.ChildrenAllowed(keyword))
    //            {
    //                return "<" + keyword + data + "></" + keyword + ">";
    //            }
    //            else
    //            {
    //                return match.Value;
    //            }
    //        });

    //    }
    //}
}
