using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using CsQuery.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using System.Diagnostics;

namespace CsQuery.Tests.Examples
{
    /// <summary>
    /// This test is disabled by default because it accesses public web sites, activate it just to test this feature
    /// </summary>
    [TestFixture, TestClass]
    public class Tables : CsQueryTest
    {

        private string html = @"<table class='TABLEBORDER'>
 <tr>
                  <td valign='top' class='CELLTEXT'>SERVER1</td>
                  <td valign='top' class='CELLTEXT'>No</td>
                  <td valign='top' class='CELLTEXT'>USERNAME</td>
                  <td valign='top' class='CELLTEXT'>Yes</td>
                  <td valign='top' class='CELLTEXT'> </td><!-- S_STATUS_ROW -->
                </tr>
                <tr>
                  <td valign='top' class='CELLTEXT'>SERVER2</td>
                  <td valign='top' class='CELLTEXT'>No</td>
                  <td valign='top' class='CELLTEXT'>USERNAME</td>
                  <td valign='top' class='CELLTEXT'>Yes</td>
                  <td valign='top' class='CELLTEXT'> </td><!-- S_STATUS_ROW -->
                </tr>
                <tr>
                  <td valign='top' class='CELLTEXT'>SERVER3</td>
                  <td valign='top' class='CELLTEXT'>No</td>
                  <td valign='top' class='CELLTEXT'>USERNAME</td>
                  <td valign='top' class='CELLTEXT'>Yes</td>
                  <td valign='top' class='CELLTEXT'> </td><!-- S_STATUS_ROW -->
                </tr>
                <tr>
                  <td valign='top' class='CELLTEXT'>SERVER4</td>
                  <td valign='top' class='CELLTEXT'>No</td>
                  <td valign='top' class='CELLTEXT'>USERNAME</td>
                  <td valign='top' class='CELLTEXT'>Yes</td>
                  <td valign='top' class='CELLTEXT'> </td><!-- S_STATUS_CONTENT_BOTTOM -->
                </tr>
        </table>";

        [Test,TestMethod]
        public void StackOverflow_Tables()
        {

            CQ doc = CQ.Create(html);

            var myTable = doc[".TABLEBORDER"];
            var res = myTable.Find("tr")
                .Select(tr=>tr.Cq().Find("td")
                                 .Select(td => td.TextContent)
                                 .ToList())
                .ToList();

            string[] strings = res.SelectMany(l => l).ToArray();
            var tableData = new string[strings.Length];

            int j = 0;
            foreach (string tdData in tableData)
            {
                string temp = System.Text.RegularExpressions.Regex.Replace(tdData ?? "", @"\r\n+", "");
                if (temp.Equals("&nbsp;"))
                {
                    temp = " ";
                    temp = temp.Trim();
                }
                tableData[j] = temp;
                j++;
            }
            tableData = cleanStrings(tableData);
        }
        public string[] cleanStrings(string[] clean)
        {
            int j = 0;

            foreach (string data in clean)
            {
                //string temp = System.Text.RegularExpressions.Regex.Replace(data, @"\r\n+", "");
                string temp = System.Text.RegularExpressions.Regex.Replace(data, @"[\r\n]", "");
                if (temp.Equals("&nbsp;"))
                {
                    temp = " ";
                    temp = temp.Trim();
                }

                clean[j] = temp;
                j++;
            }

            return clean;
        }
    }
}
