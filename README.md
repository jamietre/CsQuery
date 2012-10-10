![DocumentUp](http://www.outsharked.com/csquery/images/csquery-logo-small.gif)



## CsQuery - .C# jQuery Port for .NET 4

CsQuery is a jQuery port for .NET 4. It implements all CSS2 & CSS3 selectors, all the DOM manipulation methods of jQuery, and some of the utility methods. The majority of the jQuery test suite (as of 1.6.2) has been ported to C#. 

As of Version 1.3 (now in 2nd beta), CsQuery uses a C# port of the [validator.nu HTML parser](http://about.validator.nu/htmlparser/). This is the same code used in the Gecko browser engine. This should result in CsQuery creating a highly HTML5 standards-compliant DOM from markup, and an identical DOM to any Gecko-based browser.

The CSS selector engine fully indexes each document on tag name, id, class, and attribute. The index is subselect-capable, meaning that complex selectors will still be able to take advantage of the index (for any part of the selector that's indexed). [Performance](#performance) of selectors compared to other existing C# HTML parsing libraries is orders of magnitude faster.

### Installation

**Latest release:** Version 1.2.1 (August 21, 2012)

**Latest prerelase:** Version 1.3.0-beta2 (October 9, 2012)

To install the latest release from NuGet package manager:

    PM> Install-Package CsQuery

To install the latest prerelease from NuGet:

    PM> Install-Package CsQuery -Pre

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

The latest release is 1.2.1 This is a bugfix release.

If you are updating from version 1.1.x, please note that versions 1.2.x includes a change to the public API which will break code which *directly* instantiates `DomElement` objects. (This has never been recommended and will not affect any code which uses `CQ.Create` or `Document.CreateElement` methods; it only affects creation using `new` to create `DomElement` objects)

There are detailed nodes for major releases:

* [Release notes for 1.2](http://blog.outsharked.com/2012/08/csquery-12-released.html) 
* [All CsQuery release notes](http://blog.outsharked.com/search/label/csquery-release)

You can also review the change log to see more detail about specific changes, or notes on unreleased changes:

* [Complete change log](https://github.com/jamietre/CsQuery/blob/master/source/README.md#source-code)

For more information on the status of CsQuery's compliance with HTML5 spec, see the [HTML5 compliance notes](https://github.com/jamietre/CsQuery/blob/master/source/HTML5.md#html5-compliance)

### Documentation

This document explains how to use CsQuery to parse and manipulate HTML. It's not comprehensive, but covers most common uses for reading HTML documents from files and URLS, and using it like jQuery.

I've just started organizing various tutorials and details to a [documentation](documentation/readme.md) folder in the repository. So far this includes a writeup on the options for creating output from CsQuery. 

I also post about [CsQuery on my blog](http://blog.outsharked.com/search/label/csquery) from time to time. Here are a few tutorials from there:

* [Using the CsQuery MVC framework](http://blog.outsharked.com/2012/08/csquery-12-released.html) from the 1.2 release notes
* [Implementing a custom filter selector](http://blog.outsharked.com/2012/07/csquery-113-released.html) from the 1.1.3 release notes
* [Creating documents](http://blog.outsharked.com/2012/06/csquery-112-released.html) from the 1.1.2 release notes
* [Loading content from the web](http://blog.outsharked.com/2012/06/async-web-gets-and-promises-in-csquery.html) asynchronously using promises

If you can't seem to figure out how to use a particular method, in almost all cases the API mimics the jQuery API, so you should start with the jQuery documentation. You could also look through the unit tests, which cover pretty much everything at some level, for straightforward examples of use.

Also be sure to look at the example applications in the source repository. 

### Contents

* [Roadmap](https://github.com/jamietre/CsQuery#roadmap)
* [Features](https://github.com/jamietre/CsQuery#features)
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
* [Promises](https://github.com/jamietre/CsQuery#promises)
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
* [Shortcomings](https://github.com/jamietre/CsQuery#shortcomings)
    * [General](https://github.com/jamietre/CsQuery#general)
    * [Missing CSS Selectors](https://github.com/jamietre/CsQuery#missing-css-selectors)


### Roadmap

As of 6/12/2012, all CSS3 selectors that don't depend on browser state have been implemented, and all jQuery DOM selection/manipulation methods have been implemented. See  [shortcomings](https://github.com/jamietre/CsQuery#shortcomings) for the specific exceptions.

The priorities for the future are, in this order:

* Writing documentation; and establishing a web site for the project. 
* Implement style sheet parser and API, which will allow complete programmatic access to styles (beyond those on the `style` attribute) and access to computed styles
* Flesh out the DOM model (properties/methods of specific element types) according to HTML5 specs. (You can always access any attribute you want just as an attribute with a string value. This has to do with the actual implementation of specific DOM element interfaces, as you would access element properties in a browser DOM).
* Implement CSS4 selectors

If you are interested in this project and want to contribute anything, let me know or just make a pull request! 

### Features

CsQuery is a .NET library that provides an implementation of the jQuery API and a document object model that simulates the browser DOM. 

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


### Usage

#### Creating a new DOM

There are several ways to interpret HTML: 

* as a complete document, e.g. a web page; 
* as content that should stand on its own but is not a complete doucment, e.g. copy for a content block, or something in a CMS; 
* as a fragment, or a snippet or a template building-block which may be incomplete or out-of-context, such as a table cell like `<td></td>`

It's important for CsQuery to understand the context so it can determine how to handle HTML that appears to be invalid correctly. HTML5 parsing rules specify a particular way of handling missing or incorrect markup. But if the context isn't a complete document, then it might not actually be invalid in that context. Usually, you can just let CsQuery figure it out, but sometimes you may want to specify a certain context.

#####Basic Method

    var dom = CQ.Create(..);   // Create content

Creates content, automatically determining context. That is, HTML content can be: a complete document, general content that can be inserted in the `body` of a document, or context-specific content that can only exist in a certain tag context (e.g. `tr` can only be within a `tbody` or `thead` context).

There are overloads for accepting `string` and  `Stream` inputs. You can also specify the handling using optional parameters:

    Create(string html, 
        HtmlParsingMode mode = HtmlParsingMode.Auto, 
        HtmlParsingOptions options = HtmlParsingOptions.Default,
        DocType docType = DocType.HTML5);

`HtmlParsingMode` determines the way CsQuery interprets your HTML.

* `HtmlParsingMode.Auto` means let CsQuery figure it out. Anything starting an `html` or `body` is treated as a complete document; anything starting with a block element is content; anything else is considered a fragment and will be assumed to be in a valid context.
* `HtmlParsingMode.Document` means the content will be treated as a complete document. Missing `html`, `body`, `head` and `title` tags will be generated to produce a complete DOM.
* `HtmlParsingMode.Content` means its not a full document, but is expected to be valid in a `body` context. For example, content starting with `<tr> ... </tr>` would be invalid and handled according to HTML5 rules (typically, discarded).
* `HtmlParsingMode.Fragment` means its an arbitrary fragment and will be treated as if its in a valid context.

The `DocType` parameter allows you to instruct the parser to use HTML4 parsing rules, if desired. Generally, though, it's sufficient to let CsQuery figure out your intent from the markup that is passed.

There are also other methods for specific handling. Generally speaking, these just call `Create` with specific options.

#####Create a document

This method forces the content to be treated as a complete document.

    CQ.CreateDocument(..)   // Create a document. 

This method assumes that the content is a complete HTML document and not a fragment. In addition to the handling of optional tags provided by the `Create` method, this method will ensure that any content is a complete HTML document. If the `html`, `body` or `head` tags are missing, they will be created. Stranded text nodes (e.g. outside of `body`) will be moved inside the body. 

#####Create a fragment
    
This method forces the content to be treated as a fragment.

    CQ.CreateFragment(..)   // Create a fragment. 

This method interprets the content as a true fragment that you may use for any purpose. No attempt will be made to identify and create missing opening tags such as `tbody` -- the actual markup will be interpreted exactly as presented. Missing close tag rules still apply, though, since it's impossible to create a CQ document that has incomplete nodes. 

When creating HTML from a selector using jQuery syntax, this is the method that will be used, e.g.

    var fragment = someCsQueryDocument.Select["<div /">];

#####Create from a file

There are methods to create a document or just content from a file directly:

    CQ.CreateFromFile(..)
    CQ.CreateDocumentFromFile(..) 
    

#####Create synchronously from a URL

    var dom = CQ.CreateFromUrl("http://www.jquery.com");
    

#####Create asynchronously (non-blocking) from a URL

CsQuery (as of 1.0 Beta 2) implements a basic Promise API for asynchronous callbacks. This can be used to create convenient constructs for managing asynchronous requests; see the "Promises" section below for more details.
   
    private static CQ Dom;
    ..
 
    // code execution continues immediately; the delegates in "Then" get executed when the
    // request finishes

    var promise = CQ.CreateFromUrlAsync("http://www.jquery.com");
    
    promise.Then(responseSuccess => {
            Dom = responseSuccess.Dom;        
        }, responseFail => { 
            ..  
        });

Of course you could just chain "Then" directly to the request without assigning it to a variable, too, but I wanted to demonstrate that this signature actually returns something you can use. You can also use a regular "callback" type construct by calling CreateFromUrlAsync with this signature:

    CQ.CreateFromUrlAsync("http://www.jquery.com", responseSuccess => {
            Dom = response.Dom;        
        }, responseFail => {
            ..
        });

Returning a promise gives you a lot of flexibility, since you can than attach other events to the resolution or rejection of the request, or create a new promise that resolves when a set of promises have all resolved. For more details and examples right now, please see the "_WebIO" tests.


##### Manipulate the DOM with jQuery methods

    dom.Select("div > span")
		.Eq(1)
		.Text("Change the text content of the 2nd span child of each div");


*The default property indexer is equivalent to "Select"*
    
    var rowsWithClass = dom[".targetClass"].Closest("td");


*Use Find (like in jQuery) to access only child elements of a selection:

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



##### Output as HTML

*Render the entire DOM*

    string html = dom.Render();


*You can render any DOM element individually*

    string elementHtml = dom[2].Render();


*You can render just the elements that are part of the selection*

    string selectionHtml = dom[".just-this-class"].RenderSelection();


### CsQuery vs. jQuery

The primary goal of this project was to make it as familiar and portable as possible. I even thought about using lowercased methods so it might be almost practical to copy and paste code between JS and C# but... I just couldn't. 

There are necessarily some differences in usage because of the language itself and strong typing. This section covers what you need to know to get going with CsQuery. Everything else should work more or less the same as jQuery.

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

That is, you wrap a single DOM element in a jQuery object so you can use jQuery methods to manipulate it. Without the "default method" concept in C#, you'd be looking at:

    var csqObject = CQ.Create(domElement);

That felt a bit to wordy for something that you do a lot, so there is a `Csq()` method on DOM elements that does the same thing:

    var csqObject = domElement.Csq();
    
    bool visible = domElement.Csq().Is(":visible");

This, by the way, is the only external dependency that the DOM model has.

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

### Promises

More recent versions jQuery introduced a "deferred" object for managing callbacks using a concept called Promises. Though this is less relevant for CsQuery because your work won't be interactive for the most part, there is one important situation where you will have to manage asynchronous events: loading data from a web server.

Making a request to a web server can take a substantial amount of time, and if you are using CsQuery for a real-time application, you probably won't want to make your users wait for the request to finish.

For example, I used CsQuery to provide current status information on the "What's New" section for the [ImageMapster](http://www.outsharked.com/imagemapster/) (my jQuery plugin) web site. But I certainly do not want to cause every single user to wait while the server makes a remote web request to GitHub (which could be slow or inaccessible). Rather, the code keeps track of when the last time it's updated it's information using a static variable. If it's become "stale", it initiates a new async request, and when that request is completed, it updates the cached data. 

So, the http request that actually triggered the update will be shown the old information, but there will be no lag. Any requests coming in after the request to GitHub has finished will of course use the new information. The code looks pretty much like this:

    private static DateTime LastUpdate;
    
    if (LastUpdate.AddHours(4) < DateTime.Now) {

        /// stale - start the update process. The actual code makes three independent requests
        /// to obtain commit & version info

        var url = "https://github.com/jamietre/ImageMapster/commits/master";
        CQ.CreateFromUrlAsync(url)
           .Then(response => {
            	LastUpdate = DateTime.Now;
	            var gitHubDOM = response.Dom;
	            ... 
	            // use CsQuery to extract needed info from the response
	        });
    }

    ...

    // render the page using the current data - code flow is never blocked even if an update
    // was requested

Though C# 5 includes some language features & methods that greatly improve asynchronous handling (see `await` and the `Task` class), I'm not using C# 5 and I dind't want to "wait". Besides, the promise API used often in Javascript is actually extraordinarily elegant so I decided to make a basic C# implementation to assist in using this method. When VS 2012 comes out I may revisit this.

The `CreateFromUrlAsync` method can return an `IPromise<ICsqWebResponse>` object. The basic promise interface (from CommonJS Promises/A) has only one method:

    then(success,failure,progress)

The basic use in JS is this:
    
    someAsyncAction().then(successDelegate,failureDelegate);

When the action is resolved, "success" is called with an optional parameter from the caller; if it failed, "failure" is called.

I decided to skip progress for now; handling the two callbacks in C# requires a bit of overloading because function delegates can have different signatures. The CsQuery implementation can accept any delegate that has zero or one parameters, and returns void or something. A promise can also be generically typed, with the generic type identifying the type of parameter that is passed to the callback functions. The interface has ended up like this:

    public interface IPromise
    {
        IPromise Then(Delegate success, Delegate failure=null);
        IPromise Then(Action success, Action failure = null);
        IPromise Then(Func<IPromise> success, Func<IPromise> failure = null);
        IPromise Then(Action<object> success, Action<object> failure = null);
        IPromise Then(Func<object, IPromise> success, Func<object, IPromise> failure = null);

    }

    public interface IPromise<T> : IPromise
    {
        IPromise Then(Action<T> success, Action<T> failure = null);
        IPromise Then(Func<T, IPromise> success, Func<T, IPromise> failure = null);
    }


So the signature for `CreateFromUrlAsync` is this:

    IPromise<ICsqWebResponse> CreateFromUrlAsync(string url, ServerConfig options = null)

This makes it incredibly simple to write code with success & failure handlers inline. By strongly typing the returned promise, you don't have to cast the delegates, as in the original example: the `response` parameter is implicitly typed as `ICsqWebResponse`. If I wanted to add a fail handler, I could do this:

    CQ.CreateFromUrlAsync(url)
        .Then(responseSuccess => {
            LastUpdate = DateTime.Now;
             ...
        }, responseFail => {
             // do something
        });

CsQuery provides one other useful promise-related function called `When.All`. This is roughly equivalent to `Task.WhenAll` in C# 5 and  lets you create a new promise that resolves when every one of a set of promises has resolved. This is especially useful for this situation, since it means you can intiate several independent web requests, and have a promise that resolves only when all of them are complete. It works like this:


    var promise1 = CQ.CreateFromUrlAsync(url);
    var promise2 = CQ.CreateFromUrlAsync(url2);

    CsQuery.When.All(promise1,promise2).Then(successDelegate, failDelegate);

You can also give it a timeout which will cause the promise to reject if it has not resolved by that time. This is valuable for ensuring that you get a resolution no matter what happens in the client promises:

    // Automatically reject after 5 seconds

    CsQuery.When.All(5000,promise1,promise2)
        .Then(successDelegate, failDelegate);

`When` is a static object that is used to create instances of promise-related objects. You can also use it to create your own deferred entities:

    var deferred = CsQuery.When.Deferred();
    
    // a "deferred" object implements IPromise, and also has methods to resolve or reject

    deferred.Then(successDelegate, failDelegate);
    deferred.Resolve();   // causes successDelegate to run

Another interesting thing about promises is that they can be resolved *before* the appropriate delegates have been bound and everything still works:

    var deferred = CsQuery.When.Deferred();

    deferred.Resolve();
    deferred.Then(successDelegate, failDelegate);   // successDelegate runs immediately

By the way - the basic API and operation for "when" was 100% inspired by Brian Cavalier's excellent [when.js](https://github.com/cujojs/when) project which I use extensively in Javascript. As time permits I will probably expand the C# implementation to include many of the other promise-related utility functions from his project.

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

*Indexed access:* jQuery objects are also array-like. There's no way to "inherit" an array as a base class, nor did I want to implement a `IList` since the destructive methods don't really fit with the "selection set" mindset. (I may revisit this). Instead, though, you can use the same indexer as if it were an array. Any integral values will be treated as an index.

    dom["div"][0]  <===>  dom.Select("div")[0]  <===>  $('div')
    
*DOM Creation*: Same as jQuery. If you pass what appears to be HTML to the indexer (or the Select method), it will return a new CQ object built from that HTML string:

    dom["<div></div>"]  <===> $('<div></div>')
    
*Selection set creation*: Same as jQuery, you can build a selection set directly from another CsQuery object, or DOM elements.

    var copyOfDom2 = dom[dom2];

    var firstElementOfDom2 = dom[dom2[0]];


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

Selecting "div span" from the HTML5 spec (a 6 megabyte HTML file) is about *a hundred times faster* than HtmlAgilityPack + Fizzler. CsQuery takes a bit longer to parse the HTML in the first place (which is not unexpected, since it's building an index of everything at the same time). Without fizzler, the test case against the 6 megabyte HTML spec using a multipart XML selector with just HAP is unworkably slow so I haven't included that comparison. This [blog post](https://github.com/jamietre/CsQuery#performance) shows the results of some performance comparsions. CsQuery's HTML parsing performance has been improved considerably since then; for the large test document CsQuery is now faster than HAP; it is still slower for the other two test documents. You can run the performance tests yourself from the [CsQuery.PerformanceTests](https://github.com/jamietre/CsQuery/tree/master/source/CsQuery.PerformanceTests) project.

On my desktop machine, the reference for these times is about 2.2 seconds for CsQuery to parse the HTML, and 17.8 *milliseconds* (0.0178 s) for it to perform the selector (returning about 2,800 elements). For HAP+Fizzler, it's about 1.2 seconds to parse, and 1.8 *seconds* (1800 miliseconds) to perform the single selection. This basic comparison is included in the test suite.

A comprehensive performance evaluation would require a lot of analysis, since there are obviously a lot of moving parts at play when trying to determine how this will fare compared to other ways of manipulating HTML. At the end of the day, though, CsQuery is fast enough for me to use in real-time to parse typical (e.g. NOT several megabytes is size) HTML files before streaming to the client in public web sites. YMMV depending on demand and server capabilities, of course.

Internally, tags, class, and attribute names are indexed using a subselect-capable index, meaning that unlike jQuery, complex selectors still benefit from the index. (With jQuery, any selector that cannot be run directly against the browser DOM using `document.querySelectorAll`, because it's a subquery, is performed manually using sizzler). I don't know anything about how the Fizzler index works, so this may or may not be an advantage compared to HAP + Fizzler.

### Shortcomings

##### General

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


