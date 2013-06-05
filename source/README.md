##Source Code

The CsQuery source is C# targeting .NET4. There are no dependencies. 

The CsQuery.NuSpec file is generated automatically from a batch file under /build.

Please see the main [readme](https://github.com/jamietre/CsQuery/blob/master/README.md) for usage information. This document is mostly a change log.

### Change Log

#### Version 1.4 (prerelease)

**Bug fixes**

- [Issue #104](https://github.com/jamietre/CsQuery/issues/104) `CQ.Data` method doesn't work for raw string values
- [Issue #102](https://github.com/jamietre/CsQuery/issues/102)  &amp; [Issue #100](https://github.com/jamietre/CsQuery/issues/100) Incorrect document type handling
- [Issue #82](https://github.com/jamietre/CsQuery/issues/82) `DomObject.Text` property not returning correct text when nested text nodes are present
- Index not being updated correctly for some node removals
- `this[int index]` indexer property not implemented on `DomDocument`. Moved implementation from `DomElement` to `DomContainer` to ensure it's available for all container nodes.
- Fix - Can't update POCO properties in `CsQuery.Extend`
- Fix - Can't remove styles with `CQ.Css()` method. Styles can now be removed by passing `null` as the value

**Performance improvements**

- HTML Parser: Optimize some constructs held over from C origins for C#/.NET 
- Decouple index implementation from the `Document` and `ElementFactory` classes to permit alternate implementations
- Implement `IDomIndexProvider` interface to add a service locator for the index implementation.
- Implement `DomIndexNone`, `DomIndexSimple` and `DomIndexRanged` index implementations to permit different strategies to optimize performance. `DomIndexSimple` can cut DOM construction time almost in half with some sacrifice in complex selector performance.
- In `DomIndexRanged`, defer index updates until next query, meaning index will not have to be rebuilt following DOM changes unless needed.

To configure the default index provider, change `Config.DomIndexProvider`:

*Use the default index - subselect capable, slowest build time*

    Config.DomIndexProvider = DomIndexProviders.Ranged;

*Use the new lookup-only index: fast build time, extremely fast for simple global selectors.*

    Config.DomIndexProvider = DomIndexProviders.Simple;

*Disable the index completely: fastests build, slow selectors. Probably a bad choice except for performance testing*

    Config.DomIndexProvider = DomIndexProviders.None;

*Use an index provider other than the default*

It's possible to do this, although there's no way to directly pass a `DomIndexProvider` when creating a new `CQ` object using the public API. However, it's fairly straightfoward to use `ElementFactory` directly to build a  new `CQ` object with an alternate strategy:

    var factory = new ElementFactory(DomIndexProviders.Simple);

    CQ dom;
    using (var stream = html.ToStream())
    {
        var document = factory.Parse(stream, Encoding.UTF8);
        dom = CQ.Create(document);
    }

`ToStream` is a `string` extension method defined in `CsQuery.ExtensionMethods.Internal` which converts a string to a UTF8 stream. If you intended to create documents often using different indexing strategies within the same application, this could be wrapped as an extension method easily.


**Other changes**

- Add `CsQueryConfig` class to encapsulate configurations. Static `Config` class now implements an instance of `CsQueryConfig`.
- Add `TextContent` method; standardize implementation of `InnerText` and `TextContent` to roughly match Chrome. Ensure `Text()` jQuery method is consistent with `TextContent`.

#### Version 1.3.4

- [Issue #76](https://github.com/jamietre/CsQuery/issues/76) Change Path to use `ushort` array instead of string to solve hash collission problem with index.
- [Issue #77](https://github.com/jamietre/CsQuery/issues/77) `Timeout` not respected for web methods
- [Issue #80](https://github.com/jamietre/CsQuery/issues/80) Some valid HTML5 ID selectors not allowed
- [Issue #78](https://github.com/jamietre/CsQuery/issues/78) and [Issue #75](https://github.com/jamietre/CsQuery/issues/75) Various character set handling issues: detect character set from META tag; allow changing character set outside of HEAD; handle invalid/unkown situations gracefully.
- Resolve reentrant threading issue with `When.All` causing it to potentially fail if promise is resolved concurrent with constructor.

*Issue #76 results in `string IDomObject.Path` and `char IDomObject.PathID` being deprecated and replaced 
with `ushort[] NodePath` and `ushort NodePathID` properties. The old properties will be removed in a future version*


####Version 1.3.3 (December 30, 2012)

- [Issue #71](https://github.com/jamietre/CsQuery/issues/71) Publish signed version of assembly (see below).
- [Issue #69](https://github.com/jamietre/CsQuery/issues/69) Parser aborts when http-equiv Content-Type header found after 4096 bytes
- `RenderElement` not rendering closing tag
- Add `OuterHTML` property to IDomObject
- Add `:regex(attribute,expression)` selector into main codebase (was previously only present as part of a unit test). This matches elements where the value of the `attribute` matches the regular expression `expression`
- Add Timeout object to Promises

Regarding Issue #71: An alternate, signed/strong named version of CsQuery will be pushed to nuget using the following convention:

- The package name will be `CsQuery.Signed`
- The version number will be of the format `1.3.3-signed` meaning that it is treated as a pre-release version by NuGet. 

The signed version is intended as a service to people who are working in environments that require strong-named DLLs. This is not a security mechanism of any kind, in fact, I am making the private key available as part of the public repository.

The pre-release convention is used explicity to prevent the signed repository from appearing in NuGet searches. You should only use it if you go looking for it.

Almost everyone should use the regular repository `CsQuery`. Only use `CsQuery.Signed` if you cannot compile with the unsigned version.


####Version 1.3.2 (December 17, 2012)

- [Issue #64](https://github.com/jamietre/CsQuery/issues/64) Expose the `HttpWebRequest` object in `CsqWebRequest` so client code can manipulate it
- [Issue #66](https://github.com/jamietre/CsQuery/issues/66) Exception while parsing valid nested CSS selectors
- [Issue #65](https://github.com/jamietre/CsQuery/issues/65) Stack overflow parsing documents with deep nesting
- [Issue #63](https://github.com/jamietre/CsQuery/issues/63) Exception in RangeSortedDictionary.
- [Issue #61](https://github.com/jamietre/CsQuery/issues/61) Closest method only returning results for first element in selection
- [Issue #59](https://github.com/jamietre/CsQuery/issues/59) Comments being omitted on rendering.
Notes:

- Add `TrueStringComparer` class for use in index; the native string comparison methods prove to be inadequate for comparing the compact keys used by the index (related to Issue #61)
- Stack overflow caused by use of recursion to obtain full path to a node; this was changed to use a different method in `DomObject.GetPath()` 

####Version 1.3.1

- [Issue #57](https://github.com/jamietre/CsQuery/issues/57) Unhandled NullReferenceException using Next()
- [Issue #56](https://github.com/jamietre/CsQuery/issues/56) Incorrect handling of escaped characters in attribute-equals type selectors
- [Issue #53](https://github.com/jamietre/CsQuery/issues/53) Slow Text() in some situations

####Version 1.3. 

Read [detailed release notes](http://blog.outsharked.com/2012/10/csquery-13-released.html) for 1.3.

- Map null values to empty string when assigning NodeValue to a Text, Comment node. (Text node may contain 
- Add `AllowSelfClosingTags` option to `ITokenHandler` - provides for cleaner handling of flexible input -- no more need to use a regex to preprocess, avoiding erroneous handling of complex html.
- [Issue #51](https://github.com/jamietre/CsQuery/issues/51): Fix issue with compound subselectors whose target included CSS matches above the level of the context.
- Add `HtmlEncoderFull` that will render text codes for all supported HTML5 entities with text aliases.
- `CQ.DefaultDocType` has been deprecated. Use `Config.DocType` instead
- `CQ.DefaultDomRenderingOptions` has been deprecated. Use `Config.DomRenderingOptions` instead.

####Version 1.3 Beta 2

- Build out `IOutputFormatter` interface/methods and move element rendering implementations to OutputFormatters.. Some `Render` methods have been marked as obsolete now.
- Add unicode astral plane handling for HTML decoder

####Version 1.3 Beta 1

- Switched to validator.nu HTML parser
- Add overrides to `Create` methods to provide parsing options
- Change of `CssStyleType` to `CSSStyleType` (and interface) for consistency with browser DOM "CSS" convention
- Deprecate Document.DomRenderingOptions
- Change IOutputFormatter interface to a more generic `TextWriter` processor
- Add `TextWriter` and `StringBuilder` overloads to `Render` methods
- Add `Save` method

####Version 1.2.2 (unreleased)

*Bug Fixes*

- Change to `CsQuery.Utility.JsonSerializer.Deserialize` to allow compiling under Mono
- Enumerate the root elements to bind the selection set for documents created from HTML. Otherwise changes in the document will affect the default selection set.
- [Issue #39](https://github.com/jamietre/CsQuery/issues/39): Incorrect parsing of orphaned open carets
- Wrap Create methods using streams with Using to ensure they are disposed properly
- [Issue #41](https://github.com/jamietre/CsQuery/issues/41) - Incorrect handling of "/" character in unquoted attribute values
- :empty could return false when non-text or non-element nodes are present

*Other Changes*

- Add new `DomRenderingOptions` to control encoding of text nodes per [Issue #38](https://github.com/jamietre/CsQuery/issues/38): `HtmlEncodingNone` does not HTMLencode textat all; `HtmlEncodingMinimum` encodes only carets and ampersands.
- Add `CQ.HasAttr` method (CsQuery extension)
- Add CSS descriptor for Paged Media Module per [Pull Request #40](https://github.com/jamietre/CsQuery/pull/40) from @kaleb

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