## CsQuery.Test

6/25/2012

The CsQuery test suite can be run in **both Visual Studio and NUnit test runners**. If you add new tests please follow this convention so it will work in either place.


Every test should inherit from `CsQueryTest`. Test fixtures and tests should be marked with attributes for both Visual Studio and NUnit. Test setup and teardown methods should override `FixtureSetUp` and `FixtureTearDown` from the base class; this handles it for both frameworks.

Example:

    [TestFixture, TestClass]
    public class MyNewTestClass: CsQueryTest {
        
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            // set up this fixture
        }

        [Test,TestMethod]
        public void MyNewText()
        {
            ...
        }

        public override void FixtureTearDown()
        {
			// clean up this fixture
            base.FixtureTearDown();
            
        }

    }

You will need to set up the namespaces for the test class to understand which framework to use. You need to map `Assert` to use the NUnit assertions; the namespaces conflict so they must be aliased. This covers pretty much everything; or you can just copy one of the existing test classes.

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using Assert = NUnit.Framework.Assert;
    using CollectionAssert = NUnit.Framework.CollectionAssert;
    using Description = NUnit.Framework.DescriptionAttribute;
    
This is all in aid of letting you run the tests directly in Visual Studio, but still using NUnit as your framework.