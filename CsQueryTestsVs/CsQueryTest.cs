using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery;

namespace Jtc.CsQuery.Tests
{
    public class CsQueryTest
    {
        protected CsQuery jQuery()
        {
            return jQuery(null);
        }
        protected CsQuery jQuery(object parm)
        {

            if (parm == null)
            {
                return Dom.Select((string)null);
            }
            else if (parm.GetType() == typeof(string))
            {
                return Dom.Select((string)parm);
            }
            else if (typeof(IEnumerable<IDomObject>).IsAssignableFrom(parm.GetType()))
            {
                return Dom.Select((IEnumerable<IDomObject>)parm);
            }
            else if (typeof(IDomObject).IsAssignableFrom(parm.GetType())) {
                return Dom.Select((IDomObject)parm);
            }
            else
            {
                throw new Exception("Invalid parameter");
            }
        }
        protected IDomRoot document;
        protected CsQuery Dom
        {
            get
            {
                return _Dom;
            }
            set
            {
                _Dom = value;
                document = _Dom.Dom;
            }
        }
        private CsQuery _Dom = null;
        protected IEnumerable<IDomObject> q(string id1, string id2 = null, string id3 = null, string id4 = null, string id5 = null, string id6 = null, string id7 = null, string id8 = null, string id9 = null)
        {
            string[] ids = new string[] { id1, id2, id3, id4, id5, id6, id7, id8, id9 };
            foreach (string id in ids)
            {
                foreach (IDomElement el in Dom.Select("#" + id))
                {
                    yield return el;
                }
            }

        }
    }
}
