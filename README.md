## CsQuery - C# jQuery Port
11/30/11 - 0.99

THIS IS OUT OF DATE!

Everything has been implemented except for End, WrapInner & OffsetParent. Almost all the original jQuery tests cover it.

Will update docs shortly.


11/11/11 - 0.95

There have been many changes since the last update. Tests have been organized, substantial performance improvements
(nearly doubling speed to parse HTML and clone objects), new methods to numerous to list.

Release is imminent.

- Extend can support Enumerable parameters containing objects
- Added ability to select from other selector results (list of IDomObject)
- Started in on Traversing test suite

8/1/2011 - 0.91

- Many, many new methods
- Added half of jquery "attribute" test suite and many compatibility enhancements to attr
- Broad support for passing JSON strings to functions that take an object (e.g. css)
- Updated HTML parser to treat contents of <textarea> as text.
- Change to DOM model so most methods/properties of IDomElement are also in IDomObject with default behavior

7/29/2011 - 0.90

- Added many new utility functions: Extend, ToJSON, ParseJSON, and some more methods
- Extend works by returning ExpandoObjects, but can take anything as its source
- Added test suite. Started adding applicable jQuery unit tests (got through most of Core)
- Updated DOM model to more closely match actual browser DOM
  - Some IDomElement properties became part of IDomObject to simplify access to different element types.
    Because enumerators need to return IDomObject to handle all node types, but most of the time you are interested in
	IDomElement properties, common properties were moved to the base interaface and will return no data and thrown
	an exception if attempting to set for an invalid node type. 
  - Add/Remove/Insert operations have become part of the ChildNode object
  - This will probably change some more to continue to more closely match the browser DOM.


7/19/2011

- Added "select" and "textarea" functionality to val()
- Added Val(IEnumerable<object>) which allows setting multiple-select option lists from any array
- Indexer access works for IDomElement and IEnumerable<IDomElement> easing wrapping of elements.
- Improvements to selection engine - uses index for subqueries whenever possible now.

7/13/2011 - 0.6.2.

- Bug in Base62 function. Learned "don't reinvent" lesson.
- Added support for UpdatePanel processing in Server module. ASP.NET encodes updatepanel data with a length checksum
  which causes the client code to break if you alter anything. Server module now parses it an provides direct access
  to the components of each updatepanel.
- Added ability to select DocType 
- Fixed a problem with cloning. Clones now create their own DOM/index when created. This adds a little overhead when 
  cloning but otherwise it created special cases for clones. This makes much mores sense, it is consistent.
- Actually live tested this against a big ugly asp.net web page, fixed a few bugs, and it works!
- Handle "quoted" and non-quoted comment types
- Finish implementing DOM interfaces
- Handle CDATA
- Better handling of broken close tags (seek nearest match up tree, if none found, ignore, optionally remove)
- Add "DomRenderingOptions" to control handing of bad tags (remove) and allow removing comments
- SELECT not returning a new DOM object. (Should it select within the current CSQ? Or just use Find?)

7/13/2011 - 0.6.1

- Changed selection index to use range selectors, allowing indexed performance across any subset
- Bugfixes in HTML parser (was omitting top-level elements with no children, e.g. doctype)
- added Get()
- Refactored DOM object model to use interfaces properly
- Added DOM objects for text, comment, doctype nodes
- Added "NodeType" property to DOM elements
- Ensured selection results are output in the order they appear in the DOM regardless of where they were added during a single selection operation


7/3/2011 - Version 0.6

- Added a bunch of selectors
- Remodel the engine to support descendent/child selectors
- Changes to the DOM objets to better support cloning, css and style transparently

7/1/2011 - Version 0.5 

TODO

Rendering attributes without quotes (when setting doctype to HTML4) seems to break things - must not be handling some condition properly
   * This needs to be an option, not a function of doctype

(c) 2011 James Treworgy
MIT License

Requires: .NET 4.0 Framework


FEATURES:

- Fast, non-recursive, forgiving HTML parser
- Extensible with simple plugin model (see Server folder) 
- Included plugin to handle form postback values (e.g. update DOM with values from form posts)
- It's just like jQuery

SHORTCOMINGS

- Subset of jQuery API
- DOM model does not exactly match browser DOM API. Am not sure whether the convenience of having
  a better API outwieighs the lack of portability between client and server - may revisit this.
- Some nuances of element properties (e.g."checked") may not exactly mimic browser behavior. This isn't consistent across browsers though,


**Object Model**

    CsQuery               like $, a jQuery object
    Selectors             a Selectors object (contains one or more Selector objects, defines a selection set)
    Selector              a single selector

    CsQuery.Dom           The DOM. This is parsed from the html provided when a CsQuery is created. 
                          CsQuery objects that are created as a result of methods all reference the .Dom from the uppermost object.
    CsQuery.Elements      results of the selection
    CsQuery.Selectors     current selectors applied to create the Elements

	INTERFACES 

    IDomObject            any element in the DOM (all interfaces inherit this)
	IDomSpecialElement    any element that contains its information in the tag itself (comments, doctype, cdata)

	IDomContainer         Any DOM element that can contain other elements

    IDomRoot              The DOM itself
	    :IDomContainer
    IDomElement           A regular DOM element
	    :IDomContainer
	IDomText              A text node
	   :IDomSpecialElement
	IDomInvalidElement    A text node that looks like an HTML closing tag, but is mismatched (treated like text)
	   :IDomText
	IDomComment           A comment
	   :IDomSpecialElement
	IDomCData             A CDATA node
	   :IDomSpecialElement
	IDomDocumentType      A doctype node
	   :IDomSpecialElement

	ABSTRACT CLASSES

	DomContainer<T>:  DomObject<T>
	DomObject<T>: IDomObject

	OBJECTS

	DomRoot: DomContainer, IDomRoot
	DomDocumentType: DomContainer, IDomRoot
	DomElement: DomContainer, IDomElement
	DomText: IDomText
	DomInvalidElement: IDomInvalidElement
	DomComment: IDomComment
	DomCData: IDomCData
	DomDocumentType: IDomDocumentType


**Create DOM**

    var d = CsQuery.Create(html);
	var d = CsQuery.CreateFromElement(IDomObject e);
	var d = CsQuery.CreateFromElement(IEnumerable<IDomObject> e);

**Create a new jQuery from existing one**

    var d = new CsQuery("div",d);  <= First parm is a selector, second is an existing CsQuery object. Internally, this method is
                                       used for many methods to create the return object. Like jQuery, CsQuery returns a new object
                                       for most methods, except for methods designed to affect the DOM like "remove" and "append."

	var d = new CsQuery(IDomObject e, CsQuery context);
	var d = new CsQuery(IEnumerable<IDomObject> e, CsQuery context);
	
	var d = new CsQuery(CsQuery context)  <= Copies exactly

**Selecting**

A CsQuery object is representative of a specific DOM. Unlike a web browser, you can have any number of DOMs - each CsQuery is bound to the dom from which it
was created using one of the static methods.

The Select method creates a selection in that object. If subselection methods are used before a Select, then the top-level elements of the DOM are returned
as the selection. 

    var d = CsQuery.Select("selector"); <= this is the equivalent of $('selector');

This would return matches *within the children* of the top level matches. Assuming your DOM was created from a fully formed HTML document, this would be 
the children of the <html> element.
    
	var d = CsQuery.Find("body");  <= return just the body
	var d = CsQuery.Find("html");  <= returns nothing - html is a top-level element

	var d = CsQuery.Select("html") <= returns the DOM (except for any text nodes that may exist outside the <html> tag
	var d = CsQuery.Select("body") <= same result as .Find("body")

**Render DOM**

    string html = d.Render();

**Inspect HTML**

    d.Find('div')[0].html   <= [0] returns a DomElement object (just like a dom element in javascript). 
                               html renders its html. InnerHtml renders its inner html (just like javascript)

**Each**

    d.Find('div').Each((index,e) => {
        if (index==1) {
           d.Remove(e);
        }
    });

**Everything Else**

Matches jQuery syntax


**Implemented selectors so far**

    tagname
    .class
    #id
     
    [attr]            attribute exists
    [attr="value"]    attribute equals
    [attr^="value"]   attribute starts with
    [attr*="value"]   attribute contains
    [attr~="value"]   attribute contains word
    [attr!="value"]   attribute not equal (nor does not exist)
    [attr$="value"]   attribute ends with
    
    :checked          checked
    :contains         
    :disabled
    :enabled
    :selected
	    
	:eq(n)            nth matching result
    :gt(n)
	:lt(n)
	:even
    :odd
    :first
	:last
	
    :file
    :button           type="button" or <button>
    :checkbox         type="checkbox"    
	

    selectorA, selectorB 	cumulative selector
    selectorA selectorB		descendant selector
    selecotrA > selectorB	child selector


**Implemented methods so far:**

    jQuery (create new jQuery object from existing object, HTML, or DOM element(s))

    Add
	AddClass
    Append
    Attr
    Before
    Children
    Clone
    Css
    Each (uses delegates - can pass a function delegate or anonymous function)
    Eq
    Find
    First
    Hide
    InsertAfter
    Is
    Next
    Parent
    Prev
    Remove
	RemoveClass
    ReplaceWith
    Show
    Val

**Special/Nonstandard Methods:**
    
    Select(selector)
    
Because there's no notion of a global DOM in a C# app, the DOM is part of a CsQuery object. Each
object that gets created as a result of a selection refers to the root object which was created
from an HTML string or elements. Therefore the "Select" method is the equivalent of $('selector').

    CssGet
    
Same as Css( name ) to get a style. This signature is used to assign Css from a JSON object in
this implementation (as this is the more useful/more common usage).

**Other Methods**

    SelectionHtml()

Returns the full HTML for each element in the selection, separated by commas.

    SelectionElements()

Returns the markup for each element in the selection excluding inner HTML and children.

   Render()

Render the entire DOM as a string.