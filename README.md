## CsQuery - C# jQuery Port

5/7/12

Release 1.0 Beta 1

CsQuery is a jQuery port for .NET 4. It implements all the CSS selectors and DOM manipulation methods of jQuery and some of the utility methods. The majority of the jQuery test suite (as of 1.6.2) is ported and passes. The project necessarily includes an object model that represents the browser DOM.

### Usage

##### Creating a new DOM

*Create from a string of HTML*

    var dom = CQ.Create(htmlString);


*Load synchronously*

    var dom = CsQuery.Server.CreateFromUrl("http://www.jquery.com");
    

*Load asynchronously; the 2nd parameter is a callback*
   
    CsQuery.Server.StartAsyncWebRequest("http://www.jquery.com",response => {
        Dom = response.Dom;        
    });


##### Manipulate the DOM with jQuery methods

    dom.Select("div > span")
		.Eq(1)
		.Text("Change the text content of the 2nd span child of each div");


*The default property indexer is equivalent to "Select"*
    
    var rowsWithClass = dom[".targetClass"].Closest("td");


*Most methods are flexible with the kind of input they take to try to work as intutitively as they do in jQuery. Three ways to do the same thing:*

    rowsWithClass.AddClass("highlighted")
        .Css(new {
                width="100px",
                height=20
            });


    rowsWithClass.Css("{ width: 100px; height: 20px; }");

    rowsWithClass.Css("width",100).Css("height","20px");


*`Data` will create "data-xxx" elements that can be directly read by the jQuery data method*

    Contact contact = GetContactInfo();

    var newRow = rowsWithClass
		.Clone()
		.Data("address",contact);

    rowsWithClass.Before(newRow);


##### Accessing DOM elements directly

    var sel = dom.Select("a");


*The property indexer is overloaded as a simple list element indexer returning the DOM element at that position, just like $(...)[n].*

    var id = dom[0].id;


*The property indexer for IDomElement returns attributes*

    var href = dom[0]["href"];


*Most DOM node methods are implemented too...*

    var linkHtml = Dom.Document.GetElementById("my-link").InnerHTML;
    var sameHtml = Dom["#my-link"].Html();

*Some utility methods return nodes, same as jQuery*

    dom.Each((i,e) => {
        if (e.id == "remove-this-id") {
            e.Parent().RemoveChild(e);
        }
    });



##### Output as HTML

*Render the entire DOM*

    var html = dom.Render();


*You can render any DOM element individually*

    var elementHtml = dom[2].Render();


*You can render just the elements that are part of the selection*

    var selectionHtml = dom[".just-this-class"].RenderSelection();


### Performance

About twice as as fast as HtmlAgilityPack + Fizzler in some simple tests. Tags, class, and attribute values are indexed using a subselect-capable index, meaning that complex selectors still benefit from the index. A lot more analysis is needed to say more, but it should be fast enough to use for real-time HTML parsing in most situations.

### Shortcomings

The DOM model is not perfect. Mimicing the browser DOM would sacrifice the benefits of strong typing; I opted for a compromise that exposes some nonapplicable members on all node types. This probably could use some refactoring at this point, but it's perfectly workable.

Most of the code for pulling source HTML from URI sources is minimally tested and in the "prototype" stage. I actually use this rarely my own purposes; most of what I do is real-time parsing of html being served for HTTP requests. But since scraping is an obvious use for CsQuery, I added basic methods to grab data from a URI. They need much attention, particularly the async request code. But work well enough in simple cases. I use the asyc code on a project web site to grab & republish data in real time from GitHub about the latest commits, for example. 

The HTML parser itself is of a purely functional and nonrecursing design. I did this on purpose to make it fast. In retrospect this was probably a bad decision, because in practice the speed of parsing HTML text into an object model is limited by object creation time than by the parser itself. While it works fine, the code is hard to understand and therefore maintain. It does not comply to HTML5 rules for parsing invalid HTML. It does a decent job of recovering from errors by making some basic assumptions. If a good HTML5 parser for C# turns up one day I will probably try to replace the parser with it.

There are some minor API issues that need resolving. The "Server" object, which was originally envisioned as part of a namespaced extension design for methods that are not part of the core jQuery api, is not well conceived. The DOM model has a few problem areas. Specifically, document fragments are not represented correctly, and the value (inner text) of `script` and `textarea` nodes is not represented correctly. In the latter situation, this content is handled as a special-case text node; it should be handled as a property. This issue creates the need for some special handling in a few areas, and should be fixed.


### Basic use in C#

The static `Create` method is used to create a new DOM from an html string, a sequence of `IDomObject` elements, or another CQ object.

    CQ.Create(..)            Constructor for new DOM, same as $(...) (when passed HTML or elements)

Each instance of CQ is bound to a single DOM. Most methods return a new instance of CQ, however, they are bound to the same DOM as their originator. For example:

    var dom = CQ.Create(someHtml);
    var divs = dom.Select("div");
    divs.Empty();

the two variables `dom` and `divs` are just like two jQuery objects. `dom` is the same as `$(document)`. `divs` is the result of a selector, of course. The final `Empty` statement alters the underlying DOM associated with both. 

When working in a web browser with jQuery, you generally only have to worry about one `document`. Every method will select against it, and destructive methods will affect it. With CsQuery, however, you create a new DOM every time you use `Create`, so you could have many different ones. But every `CQ` object returned from the original target of a `Create` will be bound to its ancestor's DOM.

Some methods return a new instance of CQ, typically, when that method results in a different selection set. Other methods return the same instance. The rules for each method are the same as jQuery.

CsQuery also adds methods to return its contents (either the full DOM, or just the selection) as a string:

    Render()              Output the entire DOM as an html string

    RenderSelection()     Output only the selection set as an html string


### Important nonstandard methods

CsQuery contains a number of methods that are specific to its language implementation.

    Elements              Only the element results of the selection.
    New()                 Create a new, empty CQ object bound to the parent's DOM
	EnsureCsQuery(obj)    Return either obj, or a new CsQuery object based on obj (if a sequence of elements)

`Elements` is important because of strong typing in C# vs. Javascript. The default enumerator exposes interface `IDomObject`, an interface common to all node types. As such it has very few standard DOM node methods. Most of the time, you only care about element nodes; this method provides the results in that cast.

### Utility Methods

    Map(..)                You probably don't need this because you can use LINQ
    Extend(..)             Return an expando object composed of properties from the source objects
    ParseJSON(..)          Return an expando object from JSON string
    ParseJSON<T>(..)       Return a strongly-typed object from JSON string
    ToJSON(..)             Return a json string from an object

These methods' purposes are straightforward.

Extend tries to do a lot, allowing you to merge POCO and expando objects with properties of arbitrary types. It may not work in all situations, specifically, those involving deep-copying of list or enumerable types. This is a huge can of worms, and should not be considered part of the core framework. Ideally, I will just replace the implementation with some other library that does a great job of complex type mapping. For the time being, though, it works well in most common situations and is useful for dealing with abitrary objects of the sort you get from a javascript application. 

The JSON handling uses the .NET framework JavaScriptSerializer with some postprocessing to normalize object structures when using expando objects. This also works well enough but, again, would ideal be addressed using a more robust JSON parser.

