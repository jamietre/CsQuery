using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CsQuery.AspNet;

namespace CsQuery.WebFormsApp
{
    public partial class WebUserControl1 : CsQueryUserControl
    {
        /// <summary>
        /// Gets or sets the table columns.
        /// </summary>

        public int TableColumns { get; set; }

        /// <summary>
        /// Gets or sets the table rows.
        /// </summary>

        public int TableRows { get; set; }
     
        protected override void Cq_Render()
        {
            var table = Doc["table"];
            for (int row = 0; row < TableRows; row++)
            {
                CQ curRow = Doc["<tr></tr>"];
                for (int col = 0; col < TableColumns; col++)
                {
                    CQ curCol = Doc["<td />"].Text(
                        String.Format("Row {0} Column {1}", row, col)
                    );
                    curRow.Append(curCol);
                }
                table.Append(curRow);
            }

            // fix caption
            
            var el = Doc["#table-caption"];
            el.Text(String.Format(el.Text(), TableRows, TableColumns));

        }
    }
}