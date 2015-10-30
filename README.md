![DocumentUp](http://www.outsharked.com/csquery/images/csquery-logo-small.gif)

## Not Actively Maintained

**Note from the author:** CsQuery is not being actively maintained. I no longer use it in my day-to-day work, and indeed don't even work in .NET much these day! Therefore it is difficult for me to spend any time addressing problems or questions. If you post issues, I may not be able to respond to them, and it's very unlikely I will be able to make bug fixes. 

While the current release on NuGet (1.3.4) is stable, there are a couple known bugs (see issues) and there are many changes since the last release in the repository. However, I am not going to publish any more official releases, since I don't have time to validate the current code base and address the known issues, or support any unforseen problems that may arise from a new release.

I would welcome any community involvement in making this project active again. *If you use CsQuery and are interested in being a collaborator on the project* please contact me directly.

You should also consider using [AngleSharp](https://github.com/AngleSharp/AngleSharp), which is a newer project that is being actively maintained. It's not a drop in replacement, but provides similar capabilities.

## CsQuery - .C# jQuery Port for .NET 4

CsQuery is a jQuery port for .NET 4. It implements all CSS2 & CSS3 selectors, all the DOM manipulation methods of jQuery, and some of the utility methods. The majority of the jQuery test suite (as of 1.6.2) has been ported to C#. 

### Why CsQuery?

CSS selectors and jQuery make it really easy to access and manipulate HTML on the client. There's no reason it should be any more difficult to do the same thing with some arbitrary HTML on the server. It's a simple as that. Use it in web projects to do post-processing on HTML pages before they're served, for web scraping, parsing templates, and more. 

##### Standards Compliant HTML parsing

CsQuery uses a C# port of the [validator.nu HTML parser](http://about.validator.nu/htmlparser/). This is the same code used in the Gecko browser engine. CsQuery will create an identical DOM from the same source as any Gecko-based browser. You should expect excellent results for handling both valid and invalid markup.

##### CSS3 Selectors and jQuery methods

CsQuery implements all CSS2 and CSS3 selectors and filters, and a comprehensive DOM model. You can use all the same jQuery (and DOM element) methods you're familiar with to traverse and manipulate the DOM.

##### Fast, indexed CSS selectors

The CSS selector engine fully indexes each document on tag name, id, class, and attribute. The index is subselect-capable, meaning that complex selectors will still be able to take advantage of the index (for any part of the selector that's indexed). [Performance](#performance) of selectors compared to other existing C# HTML parsing libraries is orders of magnitude faster.

What's more, the entire test suite from Sizzle (the jQuery CSS selector engine) and jQuery (1.6.2) has been ported from Javascript to C# to cover this project.

##### It's incredibly easy

Pretty much everything you need is in the `CQ` object, which is designed to work like a jQuery object. Assigning a string to a CQ object parses it. The property indexer `['...']` runs a CSS selector,
and returns new CQ object, like `$('...')` using jQuery. Finally, the `Render` method writes the DOM back to a string. From a `CQ` object, you
have access to the complete jQuery API to traverse and manipulate your document, as well as an extensive browser DOM model.

Here's a basic example of parsing HTML, selecting something, altering it, and rendering it back to a string.

    CQ dom = "<div>Hello world! <b>I am feeling bold!</b> What about <b>you?</b></div>";
    
    
    CQ bold = dom["b"];               /// find all "b" nodes (there are two in this example)

      > bold.ToList()
      > Count = 2
      > [0]: {<b>...</b>}
      > [1]: {<b>...</b>}
    
      > bold.First().RenderSelection()
      > "<b>I am feeling bold!</b>"
       
    string boldText = bold.Text();        /// jQuery text method;
    
      > boldText
      > "I am feeling bold! you?"
    
    bold.Remove();                        /// jQuery Remove method
   
    string html = dom.Render();           

      > html
      > "<div>Hello world!  What about </div>"

There are other ways to create `CQ` objects, run selectors, and change the DOM. You can also use the property indexer 
like an array indexer, e.g. `dom[0]` returns the first element in your selection. If there is one, that is! 
Using the LINQ method `dom.FirstOrDefault()` might be a better choice for many situations. In javascript, you'd often test the
`Length` property of a selection to see if there were any results. The `CQ` object exposes an `IEnumerable<IDomObject>` interface,
so you can use LINQ to simplify many operations. But you still have all the tools that you're used to from jQuery, too.

Like in jQuery, each CQ object is made up of DOM elements. In CsQuery, the basic node is an `IDomObject` and is analagous to
an HTML element or other node (like a text node or comment node). Most of the typical HTML element methods are available.
So, using these alternatives, to obtain only the first bolded item from the example above:

*use CSS to choose first node only*

    string bold = dom["div > b:first-child"].Text();
    
*use jQuery CSS filter extensions to return the first item in the selection*
    
    string bold = dom["b:first"].Text();

*use LINQ First to get the first item, and the DOM node "InnerText" method*

    string bold = dom["b"].First().InnerText;
    
*use indexer to get the first item, and "Select" instead of the indexer to make it more readable*
    
    string bold = dom.Select("b")[0].InnerText;
    
*Use jQuery "contents" method to return the text node children, the indexer to get the first, and the
 DOM node "nodeValue" method to get the contents of a text node*
    
    string bold = dom["b"].Contents()[0].NodeValue

Each of these returns the same thing: "I am feeling bold!"


### Installation


**Latest release:** Version 1.3.4 (February 5, 2013)

To install the latest release from NuGet package manager:

    PM> Install-Package CsQuery

To install manually, add a reference to `CsQuery.DLL`. There are no external dependencies.

### Compiling from Source

This repository contains a submodule for [HtmlParserSharp](https://github.com/jamietre/HtmlParserSharp). This configuration has been chosen to allow the HTML parser project to be completely independent of CsQuery, while still allowing CsQuery to include it directly and compile to a single DLL.

To clone the repostory with the submodule, you need to take an extra step. First create a clone as usual:
    
    git clone https://github.com/jamietre/CsQuery.git csquery

Next change to the repo folder, and initialize and clone the submodule.

	cd csquery
    git submodule update --init -f

You should be able to compile everything now.  If you have any trouble initializing the submodule, just delete the submodule folder `../source/CsQuery/HtmlParserSharp` and run the submodule init command again.

### Release Notes

The current release is 1.3.4. This is a bug fix release:

  * Handle out-of-bounds character set changes
  * Allow changing character set via meta tag outside of HEAD
  * Allow non-alpha ID selectors

See the [change log](https://github.com/jamietre/CsQuery/blob/master/source/README.md) for details.

The last major release is 1.3.0. This release implements a new HTML5-compliant parser.

* **[Release notes for 1.3](http://blog.outsharked.com/2012/10/csquery-13-released.html):** Read details on the new features and other changes. You can also see [all CsQuery release notes](http://blog.outsharked.com/search/label/csquery-release) at once.
* **[Detailed change log](https://github.com/jamietre/CsQuery/blob/master/source/README.md#source-code):** see more more detail about specific changes, or notes on changes made to the source since the last release.

### Documentation

Documentation is being moved from here to the `documentation` folder in the repository. There is detailed documentation for these topics:

- [Create](https://github.com/jamietre/CsQuery/blob/master/documentation/create.md): Creating a new DOM from HTML in memory, a file, a stream, or a URL
- [Render](https://github.com/jamietre/CsQuery/blob/master/documentation/render.md): Rendering your DOM back to HTML
- [CreateFromUrl](https://github.com/jamietre/CsQuery/blob/master/documentation/createfromurl.md): Creating CsQuery objects from a remote source
- [Promises](https://github.com/jamietre/CsQuery/blob/master/documentation/promises.md): An overview of the CsQuery Promise API, which is useful for managing asynchronous events. This is useful when loading content from remote URLs without blocking execution while you're waiting for the response.
- [How CsQuery handles character set encoding](https://github.com/jamietre/CsQuery/blob/master/documentation/charset-encoding.md): Explanation of the different ways a character set encoding can be specified in an HTML document, and how CsQuery detects and prioritizes them.


Everything else will be found here in the readme. It covers most common uses for reading HTML documents from files and URLS, and using it like jQuery. 

I also post about [CsQuery on my blog](http://blog.outsharked.com/search/label/csquery) from time to time. Here are a few tutorials from there:

* [Using the CsQuery MVC framework](http://blog.outsharked.com/2012/08/csquery-12-released.html) from the 1.2 release notes
* [Implementing a custom filter selector](http://blog.outsharked.com/2012/07/csquery-113-released.html) from the 1.1.3 release notes
* [Creating documents](http://blog.outsharked.com/2012/06/csquery-112-released.html) from the 1.1.2 release notes
* [Loading content from the web](http://blog.outsharked.com/2012/06/async-web-gets-and-promises-in-csquery.html) asynchronously using promises

For methods ported from the jQuery API, in almost all cases it will function exactly as it does in jQuery. There are exceptions related to differences in the languages, but this should generally be obvious. You can also look through the unit tests, which cover pretty much everything at some level, for straightforward examples of use.

Also be sure to look at the example applications under CsQuery.Examples. 

### Contents

* [Roadmap](https://github.com/jamietre/CsQuery#roadmap)
* [Usage](https://github.com/jamietre/CsQuery#usage)
	* [Creating a new DOM](https://github.com/jamietre/CsQuery#creating-a-new-dom)
	* [Manipulate the DOM with jQuery methods](https://github.com/jamietre/CsQuery#manipulate-the-dom-with-jquery-methods)
	* [Accessing DOM elements directly](https://github.com/jamietre/CsQuery#accessing-dom-elements-directly)
	* [Output as HTML](https://github.com/jamietre/CsQuery#output-as-html)
* [CsQuery vs. jQuery](https://github.com/jamietre/CsQuery#csquery-vs-jquery)
	* [Creating a new DOM](https://github.com/jamietre/CsQuery#creating-a-new-dom-1)
	* [C# objects vs. Javascript objects](https://github.com/jamietre/CsQuery#c-objects-vs-javascript-objects)
    * [Important nonstandard methods](https://github.com/jamietre/CsQuery#important-nonstandard-methods)
    * [Utility methods](https://github.com/jamietre/CsQuery#utility-methods)
* [Options](https://github.com/jamietre/CsQuery#options)
    * [Rendering Options](https://github.com/jamietre/CsQuery#rendering-options)
    * [HTTP request options](https://github.com/jamietre/CsQuery#http-request-options)
* [The CsQuery Object Model](https://github.com/jamietre/CsQuery#the-basics-of-the-csquery-object-model)
    * [Overview](https://github.com/jamietre/CsQuery#overview)
    * [Creating a CQ object from HTML](https://github.com/jamietre/CsQuery#creating-a-cq-object-from-html)
* [The DOM (Document Object Model)](https://github.com/jamietre/CsQuery#the-dom-document-object-model)
    * [Overview](https://github.com/jamietre/CsQuery#overview-1)
    * [Referencing the "document" equivalent or DOM](https://github.com/jamietre/CsQuery#referencing-the-document-equivalent-or-dom)
* [Performance](https://github.com/jamietre/CsQuery#performance)
* [Features](https://github.com/jamietre/CsQuery#features)
    * [Shortcomings](https://github.com/jamietre/CsQuery#shortcomings)
    * [Missing CSS Selectors](https://github.com/jamietre/CsQuery#missing-css-selectors)
* [Acknowledgements](https://github.com/jamietre/CsQuery#acknowledgements)

### Roadmap

As of 6/12/2012, all CSS3 selectors that don't depend on browser state have been implemented, and all jQuery DOM selection/manipulation methods have been implemented. See  [shortcomings](https://github.com/jamietre/CsQuery#shortcomings) for the specific exceptions.

The priorities for the future are, in this order:

* Writing documentation; and establishing a web site for the project. 
* Implement style sheet parser and API, which will allow complete programmatic access to styles (beyond those on the `style` attribute) and access to computed styles
* Flesh out the DOM model (properties/methods of specific element types) according to HTML5 specs. (You can always access any attribute you want just as an attribute with a string value. This has to do with the actual implementation of specific DOM element interfaces, as you would access element properties in a browser DOM).
* Implement CSS4 selectors

If you are interested in this project and want to contribute anything, let me know or just make a pull request! 




### Usage


##### Creating a new DOM

Complete documentation: [Create method](https://github.com/jamietre/CsQuery/blob/master/documentation/create.md)

*Create from a string of HTML, a TextReader, a Stream, or an existing CQ object or DOM elements*
    
    var dom = CQ.Create(html);   

*Create from a URL (synchronously)*

    var dom = CQ.CreateFromUrl("http://www.jquery.com");
   
There are many other methods and options for creating a DOM from local sources, and from the web asynchronously. 

*Create from a URL (asynchronously)*

    IPromise promise = CQ.CreateFromUrl("http://www.jquery.com");

    CQ.CreateFromUrl("http://www.jquery.com", successDelegate, failureDelegate);

 The first method is preferred and returns an `IPromise` object, which can be used to manage resolution of deferred events without blocking the code flow. See [Promises](https://github.com/jamietre/CsQuery/blob/master/documentation/promises.md) documentation for details.


##### Output as HTML

Complete documentation: [Render method](https://github.com/jamietre/CsQuery/blob/master/documentation/render.md)

*Render the entire DOM*

    string html = dom.Render();


*You can render any DOM element individually*

    string elementHtml = dom[2].Render();


*You can render just the elements that are part of the selection*

    string selectionHtml = dom[".just-this-class"].RenderSelection();



##### Manipulate the DOM with jQuery methods

    dom.Select("div > span")
		.Eq(1)
		.Text("Change the text content of the 2nd span child of each div");


*The default property indexer is equivalent to "Select"*
    
    var rowsWithClass = dom[".targetClass"].Closest("td");


*Use Find (like in jQuery) to access only child elements of a selection:*

    // get all elements that are first children within 'body' (e.g. excluding 'head')
    
	var childSpans = dom["body"].Find(":first-child");


*Most methods are flexible with the kind of input they take to try to work as intutitively as they do in jQuery. Three ways to do the same thing:*

    rowsWithClass.AddClass("highlighted")
        .CssSet(new {
                width="100px",
                height=20
            });


    rowsWithClass.CssSet("{ width: 100px; height: 20px; }");

    rowsWithClass.Css("width",100).Css("height","20px");

See below "C# objects vs. jQuery objects" for an explanation of CssSet vs. Css.

*`Data` will create "data-xxx" attributes that can be directly read by the jQuery data method*

    Contact contact = GetContactInfo();

    var newRow = rowsWithClass
		.Clone()
		.Data("address",contact);

    rowsWithClass.Before(newRow);


##### Accessing DOM elements directly

    var sel = dom.Select("a");


*The property indexer is overloaded as a simple list element indexer returning the DOM element at that position, just like $(...)[n].*

    IDomObject element = dom[0];
    string id = element.Id;
    string classes = element.ClassName;

*The property indexer for IDomObject returns attributes*

    string href = dom[0]["href"];

*Most DOM node methods are implemented too. These are equivalent.*

    string html = Dom["#my-link"].Html();
    
    string html = Dom.Document.GetElementById("my-link").InnerHTML;
    

*Some utility methods return nodes, same as jQuery*

    dom.Each((i,e) => {
        if (e.Id == "remove-this-id") {
            e.Parent().RemoveChild(e);
        }
    });


### CsQuery vs. jQuery

The primary goal of this project was to make it as familiar and portable as possible. There are some differences in usage that were necessary because of differences in strong typing and overloading in C#. This section covers what you need to know to get going with CsQuery. Everything else should work more or less the same as jQuery.

##### Creating a new DOM

Static methods are used to create a new DOM from an html string, a sequence of `IDomObject` elements, or another CQ object.

    CQ.Create(..)           // Create content. Missing tags will be generated, except for BODY and HTML
    CQ.CreateDocument(..)   // Create a document. Missing tags will be generated according to HTML5 specs; e.g, if there is no HTML or BODY tag, they will be created.
    CQ.CreateFragment(..)   // Create a fragment. No missing tag parsing will be done.
    


You don't need to do this in a browser. The "document" is already there. You can, however, create new fragments in jQuery:

    var frag = $('<div>This is a div</div'). 

There's not really a distinction between a true Document and a fragment in CsQuery; there's no actual browser involved, it's just a node tree.

This doesn't mean that every CQ instance referes to its own DOM. Quite the opposite, the CQ object returned from most methods will be bound to the same DOM as it's parent. For example:

    CQ dom = CQ.Create(someHtml);
    CQ divs = dom.Select("div");
    divs.Empty();

this is about the same as:

    var dom = $(document);
    var divs = $("div");
    divs.empty();

Just like jQuery, some methods return a new instance of CQ, typically, when that method results in a different selection set. Other methods return the same instance. But either way, they are bound to the same DOM. The rules for whether a new instance is returned, or the existing one is altered, are the same as for each method in jQuery.

##### C# objects vs. Javascript objects

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

Another thing that you do a lot in jQuery is this:

    var jqObject = $(domElement);

That is, you wrap a single DOM element in a jQuery object so you can use jQuery methods to manipulate it. The CsQuery way to do this is create a new `CQ` object, and pass in the element in the constructor:

    var csqObject = new CQ(domElement);

There is also a shortcut:

    var csqObject = domElement.Cq();

For example:

    bool visible = domElement.Cq().Is(":visible");

These both produce the same result: A `CQ` object bound to the with `domElement` as the selection.

Note that this is *not the same* as the very similar-looking Create method:

    var csqObject = CQ.Create(domElement);   // probably not what you want!!

The `Create` methods *always* create a new DOM. Calling the method above would result in a brand-new DOM with a single element. It's not bound to your original DOM any more; in fact, the element it contains is a clone of your original `domElement`.
    


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


### Options

##### Rendering options 

There are a few options that affect HTML rendering. These are set on the `Document` property of a `CQ` object. The static property

    public static DomRenderingOptions CQ.DefaultDomRenderingOptions

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


##### HTTP request options

You can also pass options when making requests from remote servers. The global defaults are found in

    public static ServerConfig DefaultServerConfig

At this point there are only two options, but this will surely expand in the future as this functionality is more fully developed.

    int Timeout

A time (in milliseconds) after which the request should fail if it has not resolved. Default is 10000 (10 seconds).

    string UserAgent

The user agent string that should be reported to the remote server.


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

*Indexed access:* jQuery objects are also array-like. So we expose the property indexer on CQ objects to provide indexed access to the results of a selection:

    dom["div"][0]  <===>  dom.Select("div")[0]  <===>  $('div')[0]

Remember, though, that the only interface that `CQ` implements is `IEnumerable<IDomObject>`. The indexed access is just a property on the object; it doesn't use a standard interface such as `IList`. The .NET 4 framework doesn't include a `IReadOnlyList<T>` interface, and I didn't want to use the regular `IList` because it permits destructive actions.
   
*DOM Creation*: Same as jQuery. If you pass what appears to be HTML to the indexer (or the Select method), it will return a new CQ object built from that HTML string:

    dom["<div></div>"]  <===> $('<div></div>')
    
*Selection set creation*: Same as jQuery, you can build a selection set directly from another CsQuery object, or DOM elements.

    var copyOfDom2 = dom[dom2] <===> var copyOfDom2 = $(dom2);

    var firstElementOfDom2 = dom[dom2[0]] <===> var firstElementOfDom2 = $(dom2[0]);


##### Creating a CQ object from HTML

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

Note: As of 7/2012 this is outdated. There are derived types for specific HTML elements that inherit DomElement now. 

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

Selecting "div span" from the HTML5 spec (a 6 megabyte HTML file) is about *500 hundred times faster* than HtmlAgilityPack + Fizzler. Simple selectors on medium to large documents can be hundreds or thousands of times faster -- the larger the document, the bigger the difference, since HAP must scan the entire tree for each selector.

CsQuery takes a bit longer to parse the HTML in the first place (which is not unexpected, since it's building an index of everything at the same time). Without fizzler, the test case against the 6 megabyte HTML spec using a multipart XML selector with just HAP is unworkably slow so I haven't included that comparison. This [blog post](http://blog.outsharked.com/2012/06/csquery-performance-vs-fizzler.html) shows the results of some performance comparsions. You can run the performance tests yourself from the [CsQuery.PerformanceTests](https://github.com/jamietre/CsQuery/tree/master/source/CsQuery.PerformanceTests) project.

Internally, tags, class, and attribute names are indexed using a subselect-capable index, meaning that unlike jQuery, even complex selectors still benefit from the index.

### Features

CsQuery is a .NET 4 library that provides an implementation of the jQuery API and a document object model that simulates the browser DOM. It includes a complete HTML5 parser.

All jQuery DOM manipulation methods have been implemented, and some utility methods like `Extend` and `ToJSON` have been implemented as well. It also includes other methods that are specific to CsQuery's server-based implementation for loading content from remote URLs and parsing HTTP POST data.

All CSS2 & CSS3 selectors have been implemented:

    *       			Universal selector
    TAG      			element type selector
    .class   			class name selector
    #id      			id selector
	[attr]				Attribute selector, with all matchers:
						CSS2: = | |= | ~=
                    	CSS3: ^= | $= ^ *=
                    	jQuery: !=

	E, F#id				Selector grouping
	E F           		Descendant selector
	E>F					Child selector
	E+F					Adjacent sibling selector
	E~F					General sibling selector

All pseudoclasses that do not depend on browser state except "lang" are implemented:

	:first-child				:last-child					
	:first-of-type				:last-of-type				
	:only-child					:only-of-type					
	:nth-child(N)				:nth-of-type(N)
	:nth-last-child(N)			:nth-last-of-type(N)
	:enabled					:disabled
	:empty						:checked
	:root						:not(S)

jQuery extensions:

	:first						:last
	:odd						:even
	:eq(N)						:gt(N)
	:lt(N)						:parent
	:visible					:hidden
	:radio						:button
	:file						:text
	:image						:reset
	:submit						:password
	:selected					:contains(T)
	:has(S)						:input

##### Shortcomings

The DOM model is not perfect. Mimicing the browser DOM would sacrifice the benefits of strong typing; I opted for a compromise that exposes some nonapplicable members on all node types. This probably could use some refactoring at this point, but it's perfectly workable.

There are some minor API issues that need resolving. The DOM model has a few problem areas. Specifically, document fragments are not represented correctly, and it uses internal methods which means you can't substitute things that implement the same interface. 

In the early stages of this project I had not much time to get it working "well enough" to solve a particular problem.That resulted in a bit of regrettable code that needs cleaning up and some weak areas of test coverage. On the other hand, the nice thing about porting something is that you don't need to start from scratch with unit tests. The jQuery tests have the benefit of covering a lot of edge cases discovered over the years, but have the disavantage of being a bit messy and disorganized. Not that my own are a lot better! But as time permits I have been cleaning up and adding to the tests. While I think this project has pretty good test coverage for the vast majority of its features (selectors and DOM manipulation methods) some of the more complex features like Extend -- which, in particular, is difficult to test well - are not well covered.

##### Missing CSS selectors

Some parts of the CSS3 specification have not been implemented; in each case it's because the selector doesn't make sense without a browser UI context. The only exception is

    :lang(C)

"lang" may eventually be added, but it's unique in that it depends on environmental information in the browser. I am not planning to implement it at this time. You can still use the attribute selector to target nodes specifically identified with the "lang" attribute, e.g. `[lang|='en']` which would match "en-uk" and "en-us", for example. It will only return nodes that actually have the attribute, though, and not nodes that inherit it. In the correct browser implementation of `lang(C)`, every otherwise unmarked node would be returned for the default langauge of the document.

Complete list of other unimplemented pseudoselectors:

<i>UI related</i>

	:link    
	:hover
	:active
	:focus
	:visited
	:target

<i>Pseudo-elements</i>

    :first-letter (pseudoelement)
	:first-line (pseudoelement)
    :before (pseudoelement)
	:after (pseudoelement)
                            
Everything else (both browser & jQuery extensions) has been implemented.

CSS4 will be added at some point.


### Acknowledgements

CsQuery is mostly original code, but I have either been influenced by, or included directly, code written by others.

First and foremost, of course, is John Resig's jQuery, which changed the web forever.

Patrick Reisert's [HtmlParserSharp](https://github.com/Boddlnagg/HtmlParserSharp), a C# port of the [validator.nu](http://validator.nu) HTML5 parser

Miron Abramson's [fast version of Activator.CreateInstance](http://mironabramson.com/blog/post/2008/08/Fast-version-of-the-ActivatorCreateInstance-method-using-IL.aspx) for speeding up dynamic object instantiation

Mauricio Scheffer's [HttpWebRequestAdapters](https://github.com/mausch/SolrNet/tree/master/HttpWebAdapters) to make mocking HttpWebRequest objects possible.

Roger Knapps' `CombinedStream` from his [csharptest.net](http://csharptest.net/) code library 

The API and operation for the `when` object was  inspired by Brian Cavalier's excellent [when.js](https://github.com/cujojs/when) project.

