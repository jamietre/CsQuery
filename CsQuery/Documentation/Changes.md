### Change Log

Added some tests from sizzle
Allow escaping characters in ID selectors
Allow escaping characters in class name selectors
Allow HTML parser to handle : and . in node names and attribute names
Make property indexer overloads match Select() overloads
Enhance compliance with HTML spec for allowable characters in attributes, ids

6/12/2012: Version 1.1

ADDED :nth-last-of-type(N), nth-last-child(N) -- final outstanding pseudoselectors
ADDED IDomObject.Name property
Some test coverage for ExtensionMethods.Forms

6/12/2012: Version 1.1 Beta 4

ADDED :parent pseudoclass
CHANGED ensure that HTML node is handled the same way it is in browsers, e.g. it can be 
   selected but is not considered a child of anything when targeted with selectors. Affects behavior
   for open-ended pseudoclass selectors e.g. dom[":last-child"].
CHANGED update logic for open-ended pseudoclass selectors to treat like "*:last-child" and handle as a filter
   instead of some other internal "all" logic.

6/11/2012: Version 1.1 Beta 3

API change: NodeName returns uppercase
ADDED :hidden, :header (jQuery pseudoclasses)
Broke out EquationParser and StringScanner into separate projects

6/7/2012

ADDED :only-of-type, :empty
Created NuGet package


6/6/2012

ADDED adjacent sibling (+) & general sibling (~) combinators
ADDED Promise API
UPDATED Async web get API


6/5/2012 Version 1.0.1

ADDED :last-of-type
ADDED :first-of-type
ADDED :nth-of-type
ADDED :only-child
FIXED pseudoselectors affecting all descendants only returning children


