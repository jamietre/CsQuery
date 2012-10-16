### Creating CsQuery objects from HTML

Whatever you do with CsQuery, you'll start by building a DOM. This could be from an existing HTML document, a snippet of HTML text, a URL, a file, or you can build it from scratch. 

Most of the time the simplest method will work just fine. Given an input `html`:

    CQ dom = CQ.Create(html);

will create a new DOM from that source (a `string`, `Stream`, or `TextReader`). For more fine-grained control over how the input is handled, there are a number of options.

####Creating from HTML

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

`HtmlParsingOptions` are flags that enable specific behavior during parsing regardless of the mode:

* `HtmlParsingOptions.Default` means use the global default options as defined on  `CsQuery.Config.HtmlParsingOptions`. No options are enabled when CsQuery is started, you can assign specific options to the global default config if desired to affect all future operations.
* `HtmlParsingOptions.AllowSelfClosingTags` means that HTML tags which use the self-closing HTML syntax, e.g. `<div />` are permitted. Under HTML5 rules, tags like this are not valid and the closing construct is ignored. That is, this would be treated exactly like an opening tag only `<div>`. So HTML like `<div /><span />` would be rendered as `<div><span></span></div>`. Note that the span is nested in the div. HTML5's rules for missing close tags are coming into play here, too. This option lets you change this behavior so the self-closing construct is honored. (Note that for fragments, this is honored by default, in line with jQuery's HTML parsing rules).
* `HtmlParsingOptions.IgnoreComments` causes the parser to discard any HTML comments entirely; they won't appear in your DOM. You can also choose to omit contents when rendering even if you've allowed them to be parsed using `DomRenderingOptions.IgnoreComments`.

The final `DocType` parameter allows you to instruct the parser on the expected document type, if no `DOCTYPE` node is present. Generally, though, it's sufficient to let CsQuery figure out your intent from the markup that is passed.

There are also other methods described below to create content using one of the methods described here. Generally speaking, these just call `Create` with specific options, and are for convenience:

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

CsQuery implements a basic Promise API for asynchronous callbacks. This can be used to create convenient constructs for managing asynchronous requests; see the "Promises" section below for more details.
   
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