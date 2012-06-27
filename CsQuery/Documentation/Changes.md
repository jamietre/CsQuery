﻿### Change Log

This is an informal log of changes to the project; major issues will be logged and described on github.
#####6/27/2012

- Fix .Data() method to comply with jquery test
- Add RemoveData methods

#####6/26/2012 Release 1.1.2

#####6/21/2012

- HTML5 compliance for optional start/end tags
- Fix <input type="hidden"> & :visible/:hidden bug (Issue#9)
- Fix class names not case sensitive (Issue#10)

#####6/20/2012

- Correct a problem with equation caching in nth-child; huge performance boost for calculated values
- FIXED allow +/- before the first operand in an equation
- FIXED handle descending values properly in nth-child; enable sizzle test related to this
- Add IAttributesCollection interface against DomElement to provide direct access to the getter/setters as though it was a real collection
- CHANGE - include a space between style name & value on accessing style attribute

#####6/19/2012

- Add tests validating some child selectors against values from Chrome+jQuery for the huge HTML5 spec doc
- Fix auto-closing tag handling. (Some tests added above failed at first b/c document wasn't being parsed right)


#####6/18/2012

- Revise DOM creation code to treat text like Chrome does. The Document should only have 2 children (DocType & HTML node). Any text found outside should be tossed or added to Body. This wouldn't come up much, but causes selectors to be wrong in some cases.
- FIXED bug in *nth-last-child* type selectors


#####6/15/2012

- ADDED :input selector (overlooked!)
- Added all the Sizzle tests:
	-   bug fixes for :not() and :has() with some selectors
	-   refactored a lot of the CSS engine to solve some problems and remove some exception based logic
- Allow escaping characters in ID selectors
- Allow escaping characters in class name selectors
- Allow HTML parser to handle : and . in node names and attribute names
- Make property indexer overloads match Select() overloads
- Enhance compliance with HTML spec for allowable characters in attributes, ids

#####6/12/2012 Release 1.1

- ADDED :nth-last-of-type(N), nth-last-child(N) -final outstanding pseudoselectors
- ADDED IDomObject.Name property
- Some test coverage for ExtensionMethods.Forms

#####6/12/2012: Version 1.1 Beta 4

- ADDED :parent pseudoclass
- CHANGED ensure that HTML node is handled the same way it is in browsers, e.g. it can be selected but is not considered a child of anything when targeted with selectors. Affects behavior for open-ended pseudoclass selectors e.g. dom[":last-child"].
- CHANGED update logic for open-ended pseudoclass selectors to treat like "*:last-child" and handle as a filter
-    instead of some other internal "all" logic.

#####6/11/2012: Version 1.1 Beta 3

- API change: NodeName returns uppercase
- ADDED :hidden, :header (jQuery pseudoclasses)
- Broke out EquationParser and StringScanner into separate projects

#####6/7/2012

- ADDED :only-of-type, :empty
- Created NuGet package


#####6/6/2012

- ADDED adjacent sibling (+) & general sibling (~) combinators
- ADDED Promise API
- UPDATED Async web get API


#####6/5/2012 Version 1.0.1

- ADDED :last-of-type
- ADDED :first-of-type
- ADDED :nth-of-type
- ADDED :only-child
- FIXED pseudoselectors affecting all descendants only returning children


