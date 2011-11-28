## CsQuery - C# jQuery Port


(c) 2011 James Treworgy
MIT License

Requires: .NET 4.0 Framework


FEATURES:

- Fast, non-recursive, tokenizing, forgiving HTML parser
- Object model mostly replicates browser DOM
- Validate CSS if desired
- It's just like jQuery

SHORTCOMINGS:

- Subset of jQuery API, though at this point most DOM manipulation and selection methods have been implemented. 
- Doesn't validate HTML (on the list)

**Object Model**

CsQuery parses an HTML string into an object represetation of a DOM. There are objects that replicate most browser DOM objects:

    IDomObject            any element in the DOM (all interfaces inherit this)

    IDomRoot              a DOM, the equivalent of "document"
        IDomFragment      a disconnected element or collection of elements. Generally, this is something
 	                         you created that doesn't have a "body" tag.
    IDomElement           an Element node (something with a tag name)
    IDomText              a Text node
    IDomComment           a Comment node
    IDomCData             a CDATA node
    IDomDocumentType      a DOCTYPE node
    
There are other interfaces & objects:

    IDomContainer         any node type that can contain other nodes
    IDomInvalidElement    Data that could not be parsed (e.g. broken HTML that couldn't be handled). 
                              It is mostly treated as text.	                   
	IDomSpecialElement    any element that contains its information in the tag itself (comments, doctype, cdata)
    
    DomAttributes         An object to manage attributes for Element nodes
    NodeList              An object to manage collections of nodes (children)
    CSSStyleDeclaration   An object to manage styles

To create & manipulate a construct created of these elements, you use the static constructor methods of the
CsQuery object. You can build things directly from nodes, too, but usually you start with HTML. Most CsQuery
methods return a CsQuery object, which is also IEnumerable<IDomObject> exposing the current selection.

    // Create a DOM from a string of HTML
    var myDom = CsQuery.Create("<html><head></head><body><div>Hello world!</div></body></html>");
    var myDom = CsQuery.Create("<span>Hello world!</span>");
    
    // From a file
    var myDom = CsQuery.CreateFromFile(filePath);
    
    // From a web site
    var myDom = CsQuery.Server().CreateFromUrl(url);
    
    // from some elements
    List<IDomElement> elList = new List<IDomElement>();
    ...
    var myDom = CsQuery.Create(elList);

You can create new objects from an existing context like jQuery. Use "new" for this. 

    var newObj = new CsQuery("div",myDom);
    var newObj = new CsQuery("div",elList);

"New" methods create an object from an existing context (if possible). Static methods create a new DOM.


The default method is "Select." Use this as you would use the default $(..) method in jQuery:

    var allDivs = myDom.Select("div");

You can also use the indexer, which is synonymous with Select:

    var allDivs = myDom["div"];
    var webLinks = myDom["span > a[href*='www']"];
    
"Select" is different from "Find." Find is the same as the jQuery verison: it searches only the children.

    var html = "<div><div></div></div>";
    var myDom = CsQuery.Create(html);
    myDom.Select("div").Length == 2   // true
    myDom.Find("div").Length == 1     // true
    
    ReferenceEquals(myDom.Find("div")[0], myDom.Select("div").Eq(1)[0]);   // true
    
Everything is chainable like jQuery:

    var someStuff = myDom.Select("span").Clone().Append("<div />").Css("border-width","1px");

You can create new elements as you would in Javascript:

    var div = myDom.Document.CreateElement('div');

The jQuery "do everything" syntax works for creating elements too. 

	var div = myDom.Select("<div />");
    var div = myDom["<div />"];

There's a special object type called JsObject that is like and ExpandoObject. This lets you use CsQuery.Extend to 
do all kinds of crazy stuff. Extend can also accept regular objects and return JsObjects. The main difference
between JsObject and ExpandoObject is that it returns null when accessing missing properties.

    dynamic css = CsQuery.ParseJSON("{ 'width': '12px', 'height': '20px', 'checked': false }");
    CsQuery.Extend(css,"{'display':'block'});
    myDom["div"].Css(css);
    
    dynamic extended = CsQuery.Extend(null,SomeRegularObject,new { newProp: "This property was added"});
    string value = extended.newProp;

There are some unique properties and methods to CsQuery instances:

    Elements            only the Element node results of the current selection set (excludes text, comment, cdata, etc.)
    Selection           The selection set (same as enumerating the object directly).
    Selectors           current selectors applied to create the Elements
    Render()            Render the HTML text for the entire DOM
    RenderSelection()   Render the HTML for all selected elements in sequence

**Methods**

Most DOM manipulation and selection methods have been implemented. I will try to create a list of NOT implemented
methods soon as this is nearly complete.

    Add
	AddClass
	AndSelf
    Append
    AppendTo
    Attr
    AttrSet*
    AttrReplace
    Before
    Children
    Clone
    Closest
    Contents
    Css
    CssSet*
    Data
    Each (uses delegates - can pass a function delegate or anonymous function)
    Eq
    Extend
    Filter
    Find
    First
    FirstElement*
    Get
    Has
    Height
    Hide
    Html
    Index
    InsertAfter
    InsertBefore    
    Is
    Last
    Length
    Map
    Next
    Not
    Parent
    Parents
    Prev
    Prop
    Remove
    RemoveAttr
	RemoveClass
	RemoveData
    ReplaceWith
    Show
    Siblings
    Text
    Toggle
    ToggleClass
    Unwrap
    Val
    Width
    Wrap
    

**Each**

Each accepts a Func<int,IDomObject>.

    d.Find('div').Each((int index,IDomObject e) => {
        if (index==1) {
           d.Remove(e);
        }
    });

**Everything Else**

Matches jQuery syntax


**Selectors**

I've implemented most CSS3 selectors. There are some exceptions. This list is not complete right now.

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

**Change Log**

11/28/11 - 0.96

More methods. Some changes to the DOM model.

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

