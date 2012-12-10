using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using Description = NUnit.Framework.DescriptionAttribute;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.Promises;

namespace CsQuery.Tests.Promises
{

    [TestFixture, TestClass]
    public class Promises : CsQueryTest
    {

        string message;
        [Test, TestMethod]
        public void ResolveExisting()
        {
            message = null;
            Deferred test = new Deferred();

            test.Then((Action<string>)Resolved);

            string cur = "test1";

            Assert.AreNotEqual(cur, message);
            test.Resolve(cur);
            Assert.AreEqual(cur, message);

        }

        [Test, TestMethod]
        public void ResolveDeferred()
        {

            Deferred test = new Deferred();

            string cur = "test2";
            
            message = null;
            Assert.AreNotEqual(cur, message);
            test.Resolve(cur);
            test.Then((Action<string>)Resolved);
            Assert.AreEqual(cur, message);

        }
        
        [Test, TestMethod]
        public void WhenAll()
        {

            Deferred test = new Deferred();
            Deferred test2 = new Deferred();

             message = null;
            IPromise all = When.All(test, test2)
                .Then((Action<string>)Resolved);

            test.Resolve();

            Assert.AreNotEqual("resolved", message);
            
            test2.Resolve();
            Assert.AreEqual("resolved", message);

            
        }


        [Test, TestMethod]
        public void WhenAllReject()
        {

            Deferred test = new Deferred();
            Deferred test2 = new Deferred();

            message = null;
            IPromise all = When.All(test, test2)
                .Then((Action<string>)Resolved, (Action<string>)Rejected);

            test.Reject();

            Assert.AreNotEqual("resolved", message);
            Assert.AreNotEqual("rejected", message);
            
            test2.Resolve();
            Assert.AreEqual("rejected", message);


        }

        [Test, TestMethod]
        public void WhenAllTimeout()
        {

            Deferred test = new Deferred();
            Deferred test2 = new Deferred();

            IPromise all = When.All(500,test, test2)
                .Then((Action<string>)Resolved, (Action<string>)Rejected);

            message = null;
            Assert.IsNull(message);
            System.Threading.Thread.Sleep(200);
            Assert.IsNull(message);
            System.Threading.Thread.Sleep(400);
            
            Assert.AreEqual("rejected", message);
        }
        protected void Resolved(string result=null)
        {
            message = result ?? "resolved";
        }
        protected void Rejected(string result = null)
        {
            message = result ?? "rejected";
        }
    }
}