using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using CsQuery.ExtensionMethods.Forms;

namespace CsQuery.Tests.ExtensionMethods
{
    
    [TestFixture, TestClass]
    public class Forms_: CsQueryTest
    {


        [TestMethod,Test]
        public void InputText() {
            var dom = GetDom();
            var postData = new NameValueCollection();
            
            postData["H1"]=null;
            var h2 = postData["H2"]="hidden value";
            var pwd= postData["PWD"]="my secret password";
            var t1 = postData["T1"]="new text";
            var t2 = postData["T2"]="updated text";
            
            Assert.AreEqual("x",dom["input[name=H1]"].Val());
            Assert.AreEqual("",dom["input[name=H2]"].Val());
            Assert.AreEqual("",dom["input[name=PWD]"].Val());
            Assert.AreEqual("",dom["input[name=T1]"].Val());
            Assert.AreEqual("YES",dom["input[name=T2]"].Val());

            dom["input[name=H1]"].Val("change h1");

            dom.RestorePost(postData);

            Assert.AreEqual("", dom.FormValue("H1"));
            Assert.AreEqual(h2,dom.FormValue("H2"));
            Assert.AreEqual(pwd, dom.FormValue("PWD"));
            Assert.AreEqual(t1, dom.FormValue("T1"));
            Assert.AreEqual(t2, dom.FormValue("T2"));
        }

        //[TestMethod, Test]
        public void InputSelect()
        {
            throw new NotImplementedException();
        }

        [TestMethod, Test]
        public void InputCheckbox()
        {
            var dom = GetDom();
            var postData = new NameValueCollection();
            
            postData["check"]=null;
            postData["C1"] = "0";

            Assert.IsTrue(dom.FormElement("check").Is(":checked"));
            Assert.IsFalse(dom.FormElement("C1").Is(":checked"));
            
            dom.RestorePost(postData);
            Assert.IsFalse(dom.FormElement("check").Is(":checked"));
            Assert.IsFalse(dom.FormElement("C1").Is(":checked"));

            postData["check"] = "on";
            postData["C1"] = "1";
            
            dom.RestorePost(postData);
            Assert.IsTrue(dom.FormElement("check").Is(":checked"));
            Assert.IsTrue(dom.FormElement("C1").Is(":checked"));
        }

        //[TestMethod, Test]
        public void InputCheckboxMultiple()
        {
            var dom = GetDom();
            var postData = new NameValueCollection();
            postData["check"] = null;

            Assert.IsTrue(dom.FormValue<bool>("check"));

            dom.RestorePost(postData);
            Assert.IsFalse(dom.FormValue<bool>("check"));

            postData["check"] = "on";
            dom.RestorePost(postData);
            Assert.IsFalse(dom.FormValue<bool>("check"));


            

        }
        //[TestMethod, Test]
        public void InputRadio()
        {
            throw new NotImplementedException();
        }

        //[TestMethod, Test]
        public void InputSelectMultiple()
        {
            throw new NotImplementedException();
        }


        protected CQ GetDom()
        {

            return TestDom("jquery-unit-index");
        }
        
       
    }

}