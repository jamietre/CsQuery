## CsQuery - C# jQuery Port


7/3/2011 - Version 0.6

- Added a bunch of selectors
- Remodel the engine to support descendent/child selectors
- Changes to the DOM objets to better support cloning, css and style transparently

7/1/2011 - Version 0.5

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

    DomObject             abstract base class for any element in the DOM
    DomLiteral            a text node
    DomContainer          abstract class for any DOM element that can contain other elements
    DomRoot               special purpose node to contain the DOM
    DomElement            a regular DOM element

**Create DOM**

    var d = CsQuery.Create(html);
	var d = CsQuery.CreateFromElement(DomObject e);
	var d = CsQuery.CreateFromElement(IEnumerable<DomObject> e);

**Create a new jQuery from existing one**

    var d = new CsQuery("div",d);  <= First parm is a selector, second is an existing CsQuery object. Internally, this method is
                                       used for many methods to create the return object. Like jQuery, CsQuery returns a new object
                                       for most methods, except for methods designed to affect the DOM like "remove" and "append."

	var d = new CsQuery(DomObject e, CsQuery context);
	var d = new CsQuery(IEnumerable<DomObject> e, CsQuery context);
	
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
    
    :button           type="button" or <button>
    :checkbox         type="checkbox"
    :checked          checked
    :contains         
    :disabled
    :enabled
    :even
    :first
    :eq(n)            nth matching result
    :file
    :last
    :odd
    :selected

    selectorA, selectorB 	cumulative selector
    selectorA selectorB		descendant selector
    selecotrA > selectorB	child selector


**Implemented methods so far:**

    jQuery (create new jQuery object from existing object, HTML, or DOM element(s))

    Add
    Append
    Attr
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