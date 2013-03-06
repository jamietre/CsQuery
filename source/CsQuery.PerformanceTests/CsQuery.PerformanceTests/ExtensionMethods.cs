using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace CsQuery.PerformanceTests
{
    public static class ExtensionMethods
    {
        public static IEnumerable<HtmlNode> OrDefault(this IEnumerable<HtmlNode> collection)
        {
            if (collection== null || collection.FirstOrDefault() == null)
            {
                return Enumerable.Empty<HtmlNode>();
            }
            else
            {
                return collection;
            }
        }
    }
}
