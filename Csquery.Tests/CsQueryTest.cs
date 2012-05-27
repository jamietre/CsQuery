using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using StringAssert = NUnit.Framework.StringAssert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using MSClassInitialize = Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute;

namespace CsqueryTests
{
    [TestClass]
    public class CsQueryTest
    {
        public CsQueryTest()
        {
            FixtureSetUp();
        }

        ~CsQueryTest() {
            FixtureTearDown();
        }
        public virtual void FixtureTearDown()
        {

        }
        public virtual void FixtureSetUp()
        {
            CQ.DefaultDocType = DocType.XHTML;
        }
        protected CQ jQuery()
        {
            return jQuery((string)null);
        }
        protected CQ jQuery(string parm)
        {
            return Dom[parm];
        }
        protected CQ jQuery(string parm, CQ context)
        {
            return Dom.Select(parm, context);
        }
        protected CQ jQuery(IEnumerable<IDomObject> parm)
        {
            return Dom[parm];
        }
        protected CQ jQuery(IDomObject parm)
        {
            return Dom[parm];
        }
        protected CQ jQueryAny(object parm)
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
            else if (typeof(IDomObject).IsAssignableFrom(parm.GetType()))
            {
                return Dom.Select((IDomObject)parm);
            }
            else
            {
                throw new ArgumentException("Invalid parameter");
            }
        }
        public IDomDocument document;
        public CQ Dom
        {
            get
            {
                return _Dom;
            }
            set
            {
                _Dom = value;
                document = _Dom.Document;
            }
        }
        public CQ _Dom = null;
        public IEnumerable<IDomObject> q(string id1, string id2 = null, string id3 = null, string id4 = null, string id5 = null, string id6 = null, string id7 = null, string id8 = null, string id9 = null)
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
        public void t(string testName, string selector, IEnumerable<string> ids)
        {
            var csq = jQuery(selector);
            List<string> idList = new  List<string>(ids);
            int index=0;
            bool success=true;
            foreach (var item in csq) {
                if (item.Id!=idList[index++]) {
                    success=false;
                    break;
                }
            }
            Assert.IsTrue(success,testName);
        }

        protected TestDelegate Del(Action action)
        {
            return action.Invoke;
        }
    }
}
