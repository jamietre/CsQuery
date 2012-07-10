## CsQuery.Test

6/25/2012

The CsQuery test suite can be run in **both Visual Studio and NUnit test runners**. If you add new tests please follow this convention so it will work in either place. The purpose of this is to allow the use of NUnit as a testing framework, but still be able to use the Visual Studio IDE to run the tests interactively. 

A test called `TestTests.cs` in the root will perform basic validation that test classes & methods that have been marked for either framework are also marked for both, so if you make a mistake, it will likely be caught.

#####Attribute marking for NUnit and Visual Studio

Every test should inherit from `CsQueryTest`. Test fixtures and tests should be marked with attributes for both Visual Studio and NUnit. Test setup and teardown methods should override `FixtureSetUp` and `FixtureTearDown` from the base class; this handles it for both frameworks.

The class itself need only be marked with `TestClass`. This is because the `TestFixture` attribute for NUnite is inherited from CsQueryTest, but the Visual Studio framework does not honor inherited attributes, so you must still mark each class for Visual Studio.

Example:

    [TestClass]
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

#####Test Run Setup/Teardown

The class `TestAssemblyConfig.cs` performs overall test setup/teardown for both frameworks. Mostly this has to do with figuring out where it's running so the test methods can find the HTML files needed to perform most tests, and setting up some things for the performance tests (see the last section for details).

#####Dealing with conflicting namespaces

You will need to set up the namespaces for the test class to understand which framework to use. You need to map `Assert` to use the NUnit assertions; the namespaces conflict so they must be aliased. This covers pretty much everything; or you can just copy one of the existing test classes.

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using Assert = NUnit.Framework.Assert;
    using CollectionAssert = NUnit.Framework.CollectionAssert;
    using Description = NUnit.Framework.DescriptionAttribute;
    
This is all in aid of letting you run the tests directly in Visual Studio, but still using NUnit as your framework.

#####Extensions to NUnit

A class called `AssertEx.cs` in the root can be used to add test methods. There are only two now:

    AssertEx.ObjectPropertiesAreEqual(object expected, object actual, string message)
	AssertEx.ObjectPropertiesAreNotEqual(object expected, object actual, string message)

This compares the properties and values of each object, and treats expando objects -- really anything that implements `IDictionary<string,object>` -- as an object. So essentially this method converts any objects to dictionaries, and compares the dictionaries. This is needed for validating things that in javascript would return objects.


#####Performance Tests

There's a set of classes in the namespace `CsQuery.Tests.Performance` that are designed to compare the performance of CsQuery to other similar tools, right now, just HTML Agility Pack & Fizzler. These should be disabled by default in the Visual Studio IDE since they can be time consuming to run. These tests will produce detail output in txt & csv format in the subfolder `Output`.
