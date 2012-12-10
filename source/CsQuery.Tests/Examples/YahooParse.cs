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
    ///<summary>
    ///     This test is disabled by default because it accesses public web sites, activate it just to test this feature
    ///    </summary>
    [TestFixture, TestClass]
    public class YahooFinance : CsQueryTest
    {
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            ServerConfig.Default.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
        }

        //[Test, TestMethod]
        public void YahooFinanceExample()
        {
            
            string URL = "http://finance.yahoo.com/q/op?s=MSFT&m=2012-09";

            CQ doc = CQ.CreateFromUrl(URL);

            // the two tables have a class "yfnc_datamodoutline1", but wrap an inner table

            var rows = doc.Select(".yfnc_datamodoutline1 table tr");

           //  in CsQuery the indexer [] is sysnoymous with Select method
          //   Each th header row has the class ".yfnc_tablehead1" - figure out which columsn to use
          //   for the four parts you are interested in
  
            var headers= rows.First().Find(".yfnc_tablehead1");
            int strikeIndex = headers.Filter(":contains('Strike')").Index();
            int symbolIndex = headers.Filter(":contains('Symbol')").Index();
            int bidIndex = headers.Filter(":contains('Bid')").Index();
            int askIndex = headers.Filter(":contains('Ask')").Index();

           //  iterate over all rows, except the header one (the "has" excludes the header row)
            
            foreach (var row in rows.Has("td")) {
                CQ cells = row.Cq().Find("td");

                string output = String.Format("Strike: {0} Symbol: {1} Bid: {2} ask: {3}",
                    cells[strikeIndex].Cq().Text(),
                    cells[symbolIndex].Cq().Text(),
                    cells[bidIndex].Cq().Text(),
                    cells[askIndex].Cq().Text());

                Console.WriteLine(output);
            }


        }

    }
}
