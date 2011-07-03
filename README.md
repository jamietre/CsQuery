## CsQuery - C# jQuery Port

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

- Small subset of jQuery API
- DOM model does not exactly match browser DOM API right now (this will be fixed so API for DomObject heirachy will change -- avoid using 
  DOM element methods directly, instead use Attr() to change attributes)
- Some nuances of element properties (e.g."checked") may not exactly mimic browser behavior. This isn't consistent across browsers though,
- Doesn't completely handle text nodes (some jQuery methods do deal with them). They are parsed and stored in the DOM and output properly.
  but cannot be manipulated directly. Probably not hard to fix.

HELP ME!

This code has a really, really simple infrastructure for the DOM, and selector engine. 
Implementing new selectors, methods, etc should be REALLY EASY. I wrote this 
to serve a specific purpose, and even though it's a fun project I don't have time to work 
on a lot of features I don't need right now. I will keep updating it over time but feel 
free to add any new methods you want.


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

**Create a new jQuery from existing one**

    var d2 = new CsQuery("div",d);  <= First parm is a selector, second is an existing CsQuery object. Internally, this method is
                                       used for many methods to create the return object. Like jQuery, CsQuery returns a new object
                                       for most methods, except for methods designed to affect the DOM like "remove" and "append."

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
    
    :button
    :checkbox
    :checked
    :contains
    :disabled
    :enabled
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