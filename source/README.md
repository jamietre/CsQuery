##Source Code

The CsQuery source is C# targeting .NET4. There are no dependencies. 

Please see the main [readme](https://github.com/jamietre/CsQuery/blob/master/README.md) for usage information. This document is mostly a change log.

### Change Log

####Version 1.2.2 (development)

*Bug Fixes*

- Change to `CsQuery.Utility.JsonSerializer.Deserialize` to allow compiling under Mono
- Enumerate the root elements to bind the selection set for documents created from HTML. Otherwise changes in the document will affect the default selection set.
- [Issue #39](https://github.com/jamietre/CsQuery/issues/39): Incorrect parsing of orphaned open carets
- Wrap Create methods using streams with Using to ensure they are disposed properly

*Other Changes*

- Add new `DomRenderingOptions` to control encoding of text nodes per [Issue #38](https://github.com/jamietre/CsQuery/issues/38): `HtmlEncodingNone` does not HTMLencode textat all; `HtmlEncodingMinimum` encodes only carets and ampersands.
- Add `CQ.HasAttr` method (CsQuery extension)

####Version 1.2.1

*Bug Fixes*

- [Issue #36](https://github.com/jamietre/CsQuery/issues/36) - remove `IHtmlString` interface from CQ object
- [Issue #35](https://github.com/jamietre/CsQuery/issues/35) - nth-last-of-type and nth-of-type could return incorrect results
- [Issue #34](https://github.com/jamietre/CsQuery/issues/34) - After ReplaceWith, a CQ object which is source does not refer to its new location
- [Issue #33](https://github.com/jamietre/CsQuery/issues/33) - [type="text"] selecting `input` elements with no type attribute. 
- [Issue #31](https://github.com/jamietre/CsQuery/issues/31) - Tests don't work in NUnit. Remove performance tests from the main test suite to simplify test suite architecture

####Version 1.2

Version 1.2 is primarily a bug fix release, but also represents the minimum version needed for compatibility with the [CsQuery.Mvc](https://github.com/jamietre/CsQuery/tree/master/source/CsQuery.Mvc) framework. Additionally, there is a change to the public API that could be breaking (see "other changes" below). For this reason the version number has been changed to a new major release.

*Bug Fixes*

- [Issue #27](https://github.com/jamietre/CsQuery/issues/27) - Some `Value` properties not implemented.

*Other Changes*

**Breaking change:** The DOM element model has been revised to include interfaces and subclasses implementing node type specific behavior. That is, some element type (for example, `li` and `a`) that have properties with unique behavior may be implemented using a derived class rather than `DomElement`.

For this reason it is *no longer permitted* for client code to create instances of `DomElement` directly. Instead, the `DomElement.Create` factory method must be used to ensure that derived classes are returned for certain element types. Generally speaking, you shouldn't even be doing this - it has always been advisable to use `Document.CreateElement`. However there could be situations where you want to create an unbound element without any `CQ` object context. This is the way to do it now.

Apart from the change in the constructor for `DomElement`, this only other scenario that would break existing code is testing an object's type explicitly against `typeof(DomElement)`. As long as you are testing for interfaces or using `is` to test an object's type this should not break anything. 


####Version 1.1.3.1

*Bug Fixes*

- [Issue #25](https://github.com/jamietre/CsQuery/issues/25) - `CreateFromUrl` not accepting compressed content - [pull request #25 from @vitallium](https://github.com/jamietre/CsQuery/pull/26)
- [Issue #23](https://github.com/jamietre/CsQuery/issues/23) - Possible index out of range parsing invalid HTML
- [Issue #20](https://github.com/jamietre/CsQuery/issues/20) - selector engine doesn't recognize hex escape sequences - [pull request #22 from @vitallium](https://github.com/jamietre/CsQuery/pull/22)
- Store string values without JSON-syntax quotes to match the format expected by jQuery when using `Data(name,value)` method. 

*Other Changes*

- [Issue #24](https://github.com/jamietre/CsQuery/issues/24) - improve performance of substring extraction method in HTML parser
- Add `CsQuery.Config.DynamicObjectType` to specify default dynamic type when dynamic objects are created.
- Change internal type from List<T> to T[] for JSON deserialization methods. Testing unknown objects for enumerable types, and treating them as a sequence in those situations, is too risky. We should only treat actual arrays as JSON arrays.
- Clean up XML documentation in Selector, IterationData, HtmlData, others

####Version 1.1.3

Released 2012.07.17. See [release notes for version 1.1.3](http://blog.outsharked.com/2012/07/csquery-113-released.html)

*Features*

- [Pull Request #17:](https://github.com/jamietre/CsQuery/pull/17) API to add custom filter selectors
- Added `CsQuery.Config` static class for default configuration. Static configuration properties on the `CQ` object refer to this now and will be deprecated in the future.

*Bug Fixes*

- [Issue #18](https://github.com/jamietre/CsQuery/issues/18) - class & style not being indexed as an attribute
- "IDomObject.Classes" breaking when no classes returned (no bug report)

*Other changes*

- Refactored most pseudoclass selectors into modules using new API
- Some performance improvements to selection engine achieved by "lazy-sorting" selection set results and better caching of root Document node

####Version 1.1.2.2

- [Issue #16](https://github.com/jamietre/CsQuery/issues/16) causing HTML to parse incorrectly where the tag name and the first attribute were separated by new lines
- [Issue #15](https://github.com/jamietre/CsQuery/issues/15) crashing when using ":hidden" pseudoselector on an input element with no "type" attribute
- Fix for return values of `InsertAfter(sequence)` and `InsertBefore(sequence)`; was returning the original object, should have been returning the elements that were inserted. No issue for this bug, discovered during testing.
- Added `EachUntil` methods to implement the jQuery "each" method's functionality where a "false" return value causes the looping to end
- Added complete XML documentation for most common objects and interfaces: CQ, IDomDocument, IDomElement, IDomObject, some others, including links to related documentation from jQuery and Mozilla.
- Reorganization of CQ object properties and methods into more files to make it easier to work with


####Version 1.1.2.1

- Added XML docs to NuGet distribution.
- [Issue #13](https://github.com/jamietre/CsQuery/pull/13) Fix `Data` method to conform with jQuery tests (failed a few tests)
- [Issue #14](https://github.com/jamietre/CsQuery/issues/14) `Slice` method bug

####Version 1.1.2

See [release notes for version 1.1.2](http://blog.outsharked.com/2012/06/csquery-112-released.html)

*Features*

- Updates to the HTML parser to conform with HTML5 optional opening & closing tag rules. This greatly improves compatibility between the final HTML generated by CsQuery, and the DOM produced by web browsers. 

*Bug Fixes*

* [Issue #12](https://github.com/jamietre/CsQuery/issues/12) CSS class names being output in lowercase
* [Issue #11](https://github.com/jamietre/CsQuery/issues/11) :hidden selector not selecting input[type=hidden]
* [Issue #8](https://github.com/jamietre/CsQuery/issues/8) allow leading + and - signs in nth-child type equations
* Corrected a problem with some last-child selectors (found during Sizzle unit test migration, no bug report)

####Version 1.1.1

All tests from jQuery Sizzle project have been migrated; some bugs in complex selectors were revealed during this exercise. All known bugs have been fixed and all tests pass with one exception: leading operators (+ and -) are not allowed in formula CSS selectors. For example, `:nth-child(+2n+1)` will fail, but `:nth-child(2n+1)` will work. This is legal CSS but also a low priority as it is an edge case.

This release added the `:input` jQuery pseudoclass selector which I had overlooked before. The CSS engine was refactored a bit and should perform better too.

####Version 1.1.0

First NuGet release. See [release notes for version 1.1](http://blog.outsharked.com/2012/06/csquery-11-released-and-available-on.html)