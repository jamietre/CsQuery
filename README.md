## CsQuery - C# jQuery Port

6/6/2012

Release 1.0 Beta 2

CsQuery is a jQuery port for .NET 4. It implements all the CSS selectors and DOM manipulation methods of jQuery and some of the utility methods. The majority of the jQuery test suite (as of 1.6.2) is ported and passes. The project necessarily includes an object model that represents the browser DOM. The document model uses a subselect-capable index that can perform multipart selectors on large documents in milliseconds.

### Contents

This is the only formal documentation for CsQuery. When I get time I'll organize this into a web site, but for now I'm trying to keep this up to date as best as possible with anything that you'll need to know. Most of CsQuery works like jQuery, as is it's intent, so please refer to the jQuery documentation for information on jQuery methods. This document covers usage differences and methods that are not part of jQuery

* Usage
	* Creating a new DOM
	* Manipulate the DOM with jQuery methods
	* Accessing DOM elements directly
	* Output as HTML
* CsQuery vs. jQuery
	* Creating a new DOM
	* C# objects vs. jQuery objects



### Usage

##### Creating a new DOM

*Create from a string of HTML*

    var dom = CQ.Create(htmlString);


*Load synchronously* 

    var dom = CQ.CreateFromUrl("http://www.jquery.com");
    

*Load asynchronously; the 2nd parameter is a callback*

CsQuery (as of 1.0 Beta 2) implements a basic Promise API for asynchronous callbacks. This is similar to jQuery promises and can be used to create convenient constructs for managing asynchronous requests.
   
    CQ.CreateFromUrlAsync("http://www.jquery.com")
        .Then(response => {
            Dom = ((ICsqWebRequest)response).Dom;        
        });

You can also use a regular callback with this signature:

    CQ.CreateFromUrlAsync("http://www.jquery.com", response => {
            Dom = response.Dom;        
        });

For more details and examples right now, please see the "_WebIO" tests.


##### Manipulate the DOM with jQuery methods

    dom.Select("div > span")
		.Eq(1)
		.Text("Change the text content of the 2nd span child of each div");


*The default property indexer is equivalent to "Select"*
    
    var rowsWithClass = dom[".targetClass"].Closest("td");


*Most methods are flexible with the kind of input they take to try to work as intutitively as they do in jQuery. Three ways to do the same thing:*

    rowsWithClass.AddClass("highlighted")
        .CssSet(new {
                width="100px",
                height=20
            });


    rowsWithClass.CssSet("{ width: 100px; height: 20px; }");

    rowsWithClass.Css("width",100).Css("height","20px");

See below "C# objects vs. jQuery objects" for an explanation of CssSet vs. Css.

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


### CsQuery vs. jQuery

The primary goal of this project was to make it as familiar and portable as possible. I even thought about using lowercased methods so it might be almost practical to copy and paste code between JS and C# but... I just couldn't. 

There are necessarily some differences in usage because of the language itself and strong typing. This section covers what you need to know to get going with CsQuery. Everything else should work more or less the same as jQuery.

##### Creating a new DOM

The static `Create` method is used to create a new DOM from an html string, a sequence of `IDomObject` elements, or another CQ object.

    CQ.Create(..)           // Constructor for new DOM, same as $(...) (when passed HTML or elements)

You don't need to do this in a browser. The "document" is already there. You can, however, create new fragments in jQuery:

    var frag = $('<div>This is a div</div'). 

There's not really a distinction between a true Document and a fragment in CsQuery; there's no actual browser involved, it's just a node tree.

This doesn't mean that every CQ instance referes to its own DOM. Quite the opposite, the CQ object returned from most methods will be bound to the same DOM as it's parent. For example:

    var dom = CQ.Create(someHtml);
    var divs = dom.Select("div");
    divs.Empty();

this is about the same as:

    var dom = $(document);
    var divs = $("div");
    divs.empty();

Just like jQuery, some methods return a new instance of CQ, typically, when that method results in a different selection set. Other methods return the same instance. But either way, they are bound to the same DOM. The rules for whether a new instance is returned, or the existing one is altered, are the same as for each method in jQuery.

##### C# objects vs. jQuery objects

The object in Javascript is a fundamental language construct; it's amorphous, nonstatic nature and simple syntax makes it useful for lots of purposes. Some jQuery methods accept objects as a convenient way to define data structures. 

CsQuery uses reflection to allow C# objects in most of the same situations. It usually also will allow you to pass a string of JSON when an object structure would be expected, providing more syntax portability with Javascript (though you'lll have to use quotes in C#, of course). For example:


    var anchor = dom["a"].Eq(0);

    div.AttrSet(new {
                href="http://www.jquery.com",
                target="_blank"
            })
       .Text("Go to jQuery.com!");

Alternatively:

    dynamic props = new ExpandoObject();
    props.href="http://www.jquery.com";
    props.target="_blank";
    
    div.AttrSet(props).Text("Go to jQuery.com!");

Using the Quick Setter syntax (which is sort of minimally documented by jQuery):


    div.AttrSet(new { 
                css = new { 
                    href="http://www.jquery.com",
                    target="_blank"
                },
                text = "Go to jQuery.com!"
            });

Using JSON:

     div.AttrSet("{ css: { 
                    href: 'http://www.jquery.com',
                    target: '_blank'
                },
                text: 'Go to jQuery.com!'
            }");


There are a couple things to note here.

1) The method `AttrSet`. This is a special case where overloading didn't work out very well in C#. The basic "get attribute" method:

    public string Attr(string)

conflicts with the signature for a general-purpose set method:
 
    public CQ Attr(object map)

I chose this convention to resolve the conflict for `Css` and `Attr` setting methods.

2) The JSON string permits apostrophes in addition to quotes as a legal bounding character. While this makes it not legal JSON, it is much more convenient because you must use double-quotes to bound the string in C#. 


##### Important nonstandard methods

CsQuery adds methods to return its contents (either the full DOM, or just the selection) as a string:

    Render()              Output the entire DOM as an html string

    RenderSelection()     Output only the selection set as an html string

CsQuery contains a number of methods that are specific to its language implementation.

    Elements              Only the element results of the selection.
    New()                 Create a new, empty CQ object bound to the parent's DOM
	EnsureCsQuery(obj)    Return either obj, or a new CsQuery object based on obj (if a sequence of elements)

`Elements` is important because of strong typing in C# vs. Javascript. The default enumerator exposes interface `IDomObject`, an interface common to all node types. As such it has very few standard DOM node methods. Most of the time, you only care about element nodes; this method provides the results in that cast.


##### Utility Methods

    Map(..)                You probably don't need this because you can use LINQ
    Extend(..)             Return an expando object composed of properties from the source objects
    ParseJSON(..)          Return an expando object from JSON string
    ParseJSON<T>(..)       Return a strongly-typed object from JSON string
    ToJSON(..)             Return a json string from an object

These methods' purposes are straightforward.

`Extend` tries to do a lot, allowing you to merge POCO and expando objects with properties of arbitrary types. It may not work in all situations, specifically, those involving deep-copying of list or enumerable types. This is a huge can of worms, and there are definitely some untested areas. 

Ideally, I will just replace the implementation with some other library that does a great job of complex type mapping. For the time being, though, it works well in most common situations and is useful for dealing with abitrary objects of the sort you get from a javascript application. 

The JSON handling uses the .NET framework JavaScriptSerializer along with some postprocessing to normalize object structures when returning expando objects. It also has some special treatment for dictionaries when serializing - that is, they are converted to objects (as if they were expando objects) rather than key/value arrays. This also works well enough but, again, would ideally be addressed using a more robust JSON parser. 

##### Promises

More recent versions jQuery introduced a "deferred" object for managing callbacks using a concept called Promises. Though this is less relevant for CsQuery because your work won't be interactive for the most part, there is one important situation where you will have to manage asynchronous events: loading data from a web server.

Making a request to a web server can take a substantial amount of time, and if you are using CsQuery for a real-time application, you probably won't want to make your users wait for the request to finish.

For example, I used CsQuery to provide current status information on the "What's New" section for the [ImageMapster](http://www.outsharked.com/imagemapster/) (my jQuery plugin) web site. But I certainly do not want to cause every single user to wait while the server makes a remote web request to GitHub (which could be slow or inaccessible). Rather, the code keeps track of when the last time it's updated it's information using a static variable. If it's become "stale", it initiates a new async request, and when that request is completed, it updates the cached data. 

So, the http request that actually triggered the update will be shown the old information, but there will be no lag. Any requests coming in after the request to GitHub has finished will of course use the new information. The code looks pretty much like this:

    private static DateTime LastUpdate;
    
    if (LastUpdate.AddHours(4) < DateTime.Now) {

        /// stale - start the update process. The actual code makes three independent requests
        /// to obtain commit & version info

        var url = "https://github.com/jamietre/ImageMapster/commits/master";
        CQ.CreateFromUrlAsync(url, response => {
            LastUpdate = DateTime.Now;
            var gitHubDOM = response.Dom;
            ... 
            // use CsQuery to extract needed info from the response
        });
    }

    ...

    // render the page using the current data - code flow is never blocked even if an update
    // was requested

Though C# 5 includes some language features that greatly improve asynchronous handling such as `await`, I dind't want to "wait", and the promise API used often in Javascript is actually extraordinarily elegant so I decided to make a basic C# implementation to assist in using this method. 

The `CreateFromUrlAsync` method can return an `IPromise<ICsqWebResponse>` object. The basic promise interface (from CommonJS Promises/A) has only one method:

    then(success,failure,progress)

The basic use in JS is this:
    
    someAsyncAcion().then(success,failure);

When the action is resolved, "success" is called with an optional parameter from the caller; if it failed, "failure" is called.

I decided to skip progress for now; handling the two callbacks in C# requires a bit of overloading because function delegates can have different signatures. The CsQuery implementation can accept any delegate that has zero or one parameters, and returns void or something. A promise can also be generically typed, with the generic type identifying the type of parameter that is passed to the callback functions. So the signature for `CreateFromUrlAsync` is this:





### Options

There are a few options that mostly affect rendering. These are set on the `Document` property of a `CQ` object. The static property

    CQ.DefaultDomRenderingOptions

defines default options set for each new `CQ` instance created. You can assign them to any object after creation. The options are all boolean flags and can be combined.

    var dom = CQ.Create(html);
    dom.Document.DomRenderingOptions = DomRenderingOptions.RemoveComments 
        | DomRenderingOptions.QuoteAllAttributes;

There options available are below. The default options are `QuoteAllAttributes` only.

    RemoveMismatchedCloseTags

When the HTML parser finds closing element tags that it cannot match to an element, this option causes them to be ignored. Otherwise, they will be rendered as-is, often resulting in the display of text that looks like an HTML tag, depending on the browser. This option is generally not safe, since it will basically make a decision about how to handle bad HTML that should probably be left up to the browser. It will, however, result in only valid HTML being produced by CsQuery regardless of input. 

    RemoveComments

HTML comments are stripped from the output.

     QuoteAllAttributes

HTML attributes (except those that are boolean properties, such as "checked") will quoted no matter what. When this is false, CsQuery will determine if an attribute value can be safely included without quotes. If so, no quotes will be used around the attribute value. 

When true, quotes are always used. Double-quotes are used by default, unless the content can be safely quoted without escaping using single-quotes but not using double-quotes. If escaping is required either way, double-quotes are also used.



### The basics of the CsQuery object model

This section is still very much a work in progress, but if you are familiar with jQuery, using CsQuery should feel very familiar. 

##### Overview


The `CQ` object is the jQuery object. In inherits a single interface:

    public partial class CQ : IEnumerable<IDomObject>

`IDomObject` represents a DOM node (element or text), the same as a single item in a jQuery selection set. 

The base method of CQ is `Select`. Given a CQ instance `dom`:
   

    dom.Select("div")  <===>  $('div')

The CQ object, in addition to the familiar jQuery methods, uses the default property indexer for several purposes.

*Selectors:* since there's no such thing as a default method in C#, we use the indexer to provide similar functionality.

    dom["div"]  <===>  $('div')

*Indexed access:* jQuery objects are also array-like. There's no way to "inherit" an array as a base class, nor did I want to implement a `IList` since the destructive methods don't really fit with the "selection set" mindset. (I may revisit this). Instead, though, you can use the same indexer as if it were an array. Any integral values will be treated as an index.

    dom["div"][0]  <===>  dom.Select("div")[0]  <===>  $('div')
    
*DOM Creation*: Same as jQuery. If you pass what appears to be HTML to the indexer (or the Select method), it will return a new CQ object built from that HTML string:

    dom["<div></div>"]  <===> $('<div></div>')
    
*Selection set creation*: Same as jQuery, you can build a selection set directly from another CsQuery object, or DOM elements.

    var copyOfDom2 = dom[dom2];

    var firstElementOfDom2 = dom[dom2[0]];


##### Creating a CQ obejct from HTML

The `Create` method works like the default `$` method.

    // get a new jQuery/CsQuery object representing the DOM
    var dom = $(document);
    --
    CQ dom = CQ.Create(htmlString);

    // create a new div element with some attributes using quickset
    
    var dom = $("<div />",{ 
        css: { 
            width: 500, height: 20
        },
        text: "My new div"
    });
    ---
    var dom = CQ.Create("<div />",new { 
        css: new { 
            width: 500, height: 20
        },
        text: "My new div"
    });

Static methods work like utility methods in jQuery:

    var obj = $.parseJSON('{ "key": "value" }');
    --
    dynamic obj = CQ.parseJSON("{ \"key\": \"value\" }");

There's also a `CQ.toJSON` method (unlike jQuery) that converst objects to JSON.

### The DOM (Document Object Model)

##### Caveats

 As I mentioned above, there are still some rough spots in the DOM model. This API may change slightly before the first formal release of CsQuery. However, any changes will almost certainly make it a more (not less) accurate representation of the true browser DOM model. 

Additionally, even though I am using interfaces to represent the DOM model, you are not free to substitute something else that implements this interface, or internal indexing operations will fail. The There are probably not many reasons why you would need to substitute implementations for the DOM entites without simply replacing the whole model, though.

Finally, though it is safe to create new instances of DOM elements using `new`, there aren't really good reasons to do so. Rather, just use `Document.CreateElement` as if this were the browser DOM, or `CQ.Create` to create new nodes. This ensures that they are configured correctly with required data.

I intend to clean up both of these situations. The breaking use of interfaces is only designed to hide indexing methods from clients but is not necessary. The elements just need constructors to ensure that they can't be created in a broken state.

##### Overview

CsQuery is built around an object model that mostly mimics the browser DOM. Every element in this model implements the most basic interface, `IDomNode`. The basic heirarchy looks like this:

    IDomNode
        IDomObject
            IDomContainer                          	A node which has children
                IDomElement: IDomIndexedNode       	An element node
            	IDomDocument                        The root node for a DOM (the Document)
            
            IDomText								A text node
				IDomInvalidElement				    A text node which is also invalid HTML
            
 			IDomInnerText*							A special text node for handling raw text
			
			IDomSpecialElement                      Node types that contain non-attribute data inside the tag itself
                IDomDocumentType                    the DOCTYPE node
			    IDomComment                         A comment node
				IDomCData                           a CDATA node

       	IDomIndexedNode                           	A node which must be indexed
	
    INodeList                            			A list of nodes, e.g. element.Children

    ICssStyleDeclaration                            Style property for an element node
	
* may be deprecated

You will notice that there's no attribute node. Attributes are managed using a string dictionary. I might try to make this more consistent with the DOM model, but there will likely be a substantial performance hit to the HTML parser in order to implementing attributes as an object. I don't have a compelling reason, other than purity, to do so at this point.

Methods from CsQuery return either `IDomElement` sequences or `IDomObject` sequences. In Javascript, the distinction between text nodes and element nodes is not important for the most part when dealing with jQuery output. Most methods return only element nodes, unless you've specifically asked for text nodes. However, if we used this approach with CsQuery, you would have to deal the more general interface of `IDomObject` all the time, when you really want to be dealing with `IDomElement` most of the time.

To deal with this in a way that is the lease obtrusive, I decided to expose many methods that really only apply to `IDomElement` on `IDomObject`. This is pretty much how you think when working in javascript, anyway, since there is no type checking. This can result in exceptions if you try to operate on a text node as if it were an element. To help with this, CsQuery includs an `Elements` property that returns only elements. If you are unsure if a particular selector could return text nodes, use `Elements` to filter the results first (and cast them correctly). There

##### Referencing the "document" equivalent or DOM


he `Document` property of a `CQ` object represents the DOM. This is an object of type `DomDocument`. When you use a method that returns a new instance of `CQ`, its `Document` refers to the same actual object as the original. Using destructive methods will affect any `CQ` objects that are based on the same DOM.

(More to come)


### Performance

Selecting "div span" from the HTML5 spec (a 6 megabyte HTML file) is about *a hundred times faster* than HtmlAgilityPack + Fizzler. CsQuery takes about 60-70% longer to parse the HTML in the first place (which is not unexpected, since it's building an index of everything at the same time). Without fizzler, the test case against the 6 megabyte HTML spec using a multipart XML selector with just HAP is unworkably slow so I haven't included that comparison.

On my desktop machine, the reference for these times is about 2.2 seconds for CsQuery to parse the HTML, and 17.8 *milliseconds* (0.0178 s) for it to perform the selector (returning about 2,800 elements). For HAP+Fizzler, it's about 1.2 seconds to parse, and 1.8 *seconds* (1800 miliseconds) to perform the single selection. This basic comparison is included in the test suite.

While the time to parse the HTML is significant, bear in mind that this is a huge file intended as a test case. It has over 100,000 nodes. In practice, the time to run selectors and do manipulation is far more significant, since you only need to parse the DOM once. If there were a need to use CsQuery in a real-time high-load web server situation, there is much that could be done to further optimize it. For example, you could easily cache the DOM for frequently-requested pages in a cache rather than parsing static HTML files on each request, and cut that time down significantly by cloning it instead of parsing from HTML. If you had to work with very large HTML files for some reason, you could fragment out only the parts of large files that you need to manipulate with CsQuery. And of course, if you used CsQuery *instead* of some other parsing engine line Razor you'd benefit from that tradeoff, too.

A comprehensive performance evaluation would require a lot of analysis, since there are obviously a lot of moving parts at play when trying to determine how this will fare compared to other ways of manipulating HTML. At the end of the day, though, CsQuery is fast enough for me to use in real-time to parse HTML before streaming to the client in public web sites. YMMV depending on demand and server capabilities, of course.

Internally, tags, class, and attribute names are indexed using a subselect-capable index, meaning that unlike jQuery, complex selectors still benefit from the index. (With jQuery, any selector that cannot be run directly against the browser DOM using `document.querySelector`, because it's a subquery, is performed manually using sizzler). I don't know anything about how the Fizzler index works, so this may or may not be an advantage compared to HAP + Fizzler.

### Shortcomings

The DOM model is not perfect. Mimicing the browser DOM would sacrifice the benefits of strong typing; I opted for a compromise that exposes some nonapplicable members on all node types. This probably could use some refactoring at this point, but it's perfectly workable.

Most of the code for pulling source HTML from URI sources is minimally tested and in the "prototype" stage. I actually use this rarely my own purposes; most of what I do is real-time parsing of html being served for HTTP requests. But since scraping is an obvious use for CsQuery, I added basic methods to grab data from a URI. They need much attention, particularly the async request code. But work well enough in simple cases. I use the asyc code on a project web site to grab & republish data in real time from GitHub about the latest commits, for example. 

The HTML parser itself is of a purely functional and nonrecursing design. I did this on purpose to make it fast. In retrospect this was probably a bad decision, because in practice the speed of parsing HTML text into an object model is limited by object creation time than by the parser itself. While it works fine, the code is hard to understand and therefore maintain. It does not comply to HTML5 rules for parsing invalid HTML. It does a decent job of recovering from errors by making some basic assumptions. If a good HTML5 parser for C# turns up one day I will probably try to replace the parser with it.

There are some minor API issues that need resolving. The "Server" object, which was originally envisioned as part of a namespaced extension design for methods that are not part of the core jQuery api, is not well conceived. The DOM model has a few problem areas. Specifically, document fragments are not represented correctly, and the value (inner text) of `script` and `textarea` nodes is not represented correctly. In the latter situation, this content is handled as a special-case text node; it should be handled as a property. This issue creates the need for some special handling in a few areas, and should be fixed.

In the early stages of this project I had not much time to get it working "well enough" to solve a particular problem.That resulted in a some regrettable decisions and code. The nice thing about porting something is that you don't need to start from scratch with unit tests. The jQuery tests have the benefit of covering a lot of edge cases discovered over the years, but have the disavantage of being a bit messy and disorganized. Not that my own are a lot better! But as time permits I have been cleaning up and adding to the tests. While I think this project has pretty good test coverage for the vast majority of its features (selectors and DOM manipulation methods) some of the more complex features like Extend -- which, in particular, is difficult to test well - are not well covered.










