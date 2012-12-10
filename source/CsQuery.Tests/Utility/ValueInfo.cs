using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using StringAssert = NUnit.Framework.StringAssert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery;
using CsQuery.Utility;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.StringScanner;

namespace CsQuery.Tests.Utility
{
    [TestClass,TestFixture]
    public class ValueInfo_
    {

        [Test,TestMethod]
        public void CharacterInfo_()
        {
            ICharacterInfo charInfo;

            charInfo = CharacterData.CreateCharacterInfo('z');
            Assert.IsTrue(IsOnly(charInfo, "alpha,alphanumeric,lower"), "alpha lower");

            charInfo.Target = 'A';
            Assert.IsTrue(IsOnly(charInfo, "alpha,alphanumeric,upper"), "alpha upper");


            charInfo.Target = ' ';
            Assert.IsTrue(IsOnly(charInfo, "whitespace,separator"), "only whitespace");

            charInfo.Target = '{';
            Assert.IsTrue(IsOnly(charInfo, "bound"), "only bound");

            charInfo.Target = '(';
            Assert.IsTrue(IsOnly(charInfo, "parenthesis,bound"), "only paren & bound");

            charInfo.Target = '[';
            Assert.IsTrue(IsOnly(charInfo, "bound"), "only bound");

            charInfo.Target = '<';
            Assert.IsTrue(IsOnly(charInfo, "bound,operator"), "only operator & bound");

            charInfo.Target = '0';
            Assert.IsTrue(IsOnly(charInfo, "numeric,numericext,alphanumeric"), "numeric");

            charInfo.Target = '9';
            Assert.IsTrue(IsOnly(charInfo, "numeric,numericext,alphanumeric"), "only numeric");

            charInfo.Target = '.';
            Assert.IsTrue(IsOnly(charInfo, "numericext"), "only numeric ext");

            charInfo.Target = '-';
            Assert.IsTrue(IsOnly(charInfo, "numericext,operator"), "only numeric ext & oper");
            
            charInfo.Target = '+';
            Assert.IsTrue(IsOnly(charInfo, "numericext,operator"), "only numeric ext & operator");
            
            charInfo.Target = ':';
            Assert.IsTrue(IsOnly(charInfo, ""), "Nothing");

            charInfo.Target = '\\';
            Assert.IsTrue(IsOnly(charInfo, ""), "Nothing");
        }

        [TestMethod,Test]
        public void StringInfo_()
        {
            IStringInfo info;

            info = CharacterData.CreateStringInfo("alllower");
            Assert.IsTrue(IsOnly(info, "alpha,alphanumeric,lower,attribute"), "alpha");

            info.Target = "Mixed";
            Assert.IsTrue(IsOnly(info, "alpha,alphanumeric,attribute"), "only whitespace");
            
            info.Target = "ALLUPPER";
            Assert.IsTrue(IsOnly(info, "alpha,alphanumeric,upper,attribute"), "All upper");

            info.Target = "nn123";
            Assert.IsTrue(IsOnly(info, "alphanumeric,lower,attribute"), "Alphanumeric,attribute");

            info.Target = "    \n";
            Assert.IsTrue(IsOnly(info, "whitespace,separator"), "only separator & whitespace");

            info.Target = "([])";
            Assert.IsTrue(IsOnly(info, "bound"), "only bound");

            info.Target = "()";
            Assert.IsTrue(IsOnly(info, "parenthesis,bound"), "only paren & bound");

            info.Target = "<>";
            Assert.IsTrue(IsOnly(info, "bound,operator"), "only operator & bound");

            info.Target = "0123456789";
            Assert.IsTrue(IsOnly(info, "numeric,numericext,alphanumeric"), "only numeric");


            info.Target = "000";
            Assert.IsTrue(IsOnly(info, "numeric,numericext,alphanumeric"), "only numeric");

            info.Target = "-12.2";
            Assert.IsTrue(IsOnly(info, "numericext"), "only numeric ext");

            info.Target = "+12.2";
            Assert.IsTrue(IsOnly(info, "numericext"), "only numeric ext");

            info.Target = "-+";
            Assert.IsTrue(IsOnly(info, "operator,numericext"), "only numeric ext & oper");

            info.Target = "data-test";
            Assert.IsTrue(IsOnly(info, "attribute,lower"), "only attribute");

            info.Target = ":data-test";
            Assert.IsTrue(IsOnly(info, "attribute,lower"), "only attribute");

            info.Target = "-data-test";
            Assert.IsTrue(IsOnly(info, "lower"), "invalid attribute");

            info.Target = "-data-test,";
            Assert.IsTrue(IsOnly(info, "lower,separator"), "invalid attribute + separator");

        }


        protected bool IsOnly(IValueInfo info, string what)
        {
            HashSet<string> testFor = new HashSet<string>(what.Split(','));

            foreach (string val in testFor)
            {
                if (!val.IsOneOf("","separator","attribute","alpha", "alphanumeric", "bound", "lower", "upper", "whitespace", "operator", "parenthesis","numericext","numeric"))
                {
                    throw new ArgumentException("Invalid parameter passed to IsOnly");
                }
            }

            bool valid=true;

            valid &= (testFor.Contains("alpha")) ? info.Alpha : !info.Alpha;
            valid &= testFor.Contains("alphanumeric") ? info.Alphanumeric : !info.Alphanumeric;
            valid &= testFor.Contains("numeric") ? info.Numeric : !info.Numeric;
            valid &= testFor.Contains("numericext") ? info.NumericExtended : !info.NumericExtended;
            valid &= testFor.Contains("lower") ? info.Lower : !info.Lower;
            valid &= testFor.Contains("upper") ? info.Upper : !info.Upper;
            valid &= testFor.Contains("whitespace") ? info.Whitespace : !info.Whitespace;
            valid &= testFor.Contains("operator") ? info.Operator : !info.Operator;


            if (info is ICharacterInfo)
            {
                var cInfo = (ICharacterInfo)info;
                valid &= testFor.Contains("parenthesis") ? cInfo.Parenthesis : !cInfo.Parenthesis;
                valid &= testFor.Contains("quote") ? cInfo.Quote : !cInfo.Quote;
                valid &= testFor.Contains("bound") ? cInfo.Bound : !cInfo.Bound;
                valid &= testFor.Contains("separator") ? cInfo.Separator : !cInfo.Separator;
            }
            if (info is IStringInfo)
            {
                var sInfo = (IStringInfo)info;
                valid &= testFor.Contains("attribute") ? sInfo.HtmlAttributeName : !sInfo.HtmlAttributeName;
            }
            
            return valid;
        }
        
      
    }
}



