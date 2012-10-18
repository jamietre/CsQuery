### Creating CsQuery objects from HTML

Whatever you do with CsQuery, you'll start by building a DOM. This could be from an existing HTML document, a snippet of HTML text, a URL, a file, or you can build it from scratch. 

Most of the time the simplest method will work just fine. Given an input `html`:

    CQ dom = CQ.Create(html);

will create a new DOM from that source (a `string`, `Stream`, or `TextReader`). For more fine-grained control over how the input is handled, there are a number of options.

####Creating from HTML

There are several ways to interpret HTML: 

* as a complete document, e.g. a web page; 
* as content that should stand on its own but is not a complete document, e.g. copy for a content block, or something in a CMS; 
* as a fragment, or a snippet or a template building-block which may be incomplete or out-of-context, such as a table cell like `<td></td>`

It's important for CsQuery to understand the context of a chunk of HTML. The HTML5 spec specifies particular way of handling missing or incorrect markup. But whether or not something's correct depends on where it appears. For example, a `<div>` can't be the child of an `<a>`. 

So, we need to know how something anything should be handled. If it's supposed to represent a complete document, then one set of rules applies. If it's just an arbitrary piece of HTML that will be added to a document later, then a different set of rules applies. Usually, you can just let CsQuery figure it out, but sometimes you may want to specify a certain context.

#####Basic Method

Create content, letting CsQuery automatically determine the context:

    var dom = CQ.Create(..); 

The HTML content can be: a complete document, general content that can be inserted in the `body` of a document, or context-specific content that can only exist in a certain tag context (e.g. `tr` can only be within a `tbody` or `thead` context). 

When called with no parameters other than the HTML, CsQuery will assess the context by looking at the first tag. There input can be a `string`, a `Stream`, or a `TextReader`. 

You can also specify the handling, as well as other options, using optional parameters:

    Create(string html, 
        HtmlParsingMode mode = HtmlParsingMode.Auto, 
        HtmlParsingOptions options = HtmlParsingOptions.Default,
        DocType docType = DocType.HTML5);

`HtmlParsingMode` determines context.

* `HtmlParsingMode.Auto` means let CsQuery figure it out. Anything starting an `html` or `body` is treated as a complete document; anything starting with a block element is content; anything else is considered a fragment and will be assumed to be in a valid context.
* `HtmlParsingMode.Document` means the content will be treated as a complete document. Missing `html`, `body`, and `head` tags will be generated to produce a complete DOM.
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

In addition to the handling of optional tags provided by the `Create` method, this method will ensure that any content is a complete HTML document. If the `html`, `body` or `head` tags are missing, they will be created. Optional tags such as `table > tbody` are always generated. Stranded text nodes (e.g. outside of `body`) will be moved inside the body. This is the same handling a browser would use when rendering any HTML source.

#####Create a fragment
    
This method forces the content to be treated as a fragment.

    CQ.CreateFragment(..)   // Create a fragment. 

This method interprets the content as a fragment that you may use for any purpose. The context will be assumed to be legal for the first tag found; howver, any invalid child constructs will be parsed according to HTML5 rules. 

When creating HTML from a selector using jQuery syntax, this is the method that is used:

    var fragment = someCsQueryDocument.Select["<div /">];

Note that when using `CreateFragment`, self-closing syntax is allowed for tags. HTML5 rules specify that this construct should be ignored, so normally, a self-closing tag would be handled the same as an opening tag. This method explicitly permits this syntax for convenience. You can also control this using the `DomRenderingOptions.AllowSelfClosingTags` flag.


<i>Create A Fragment In Tag Context</i>

There's an important overload for `CreateFragment` that offers functionality not available through the `Create` method:

    CQ CreateFragment(string html, string context)

This allows you to explicitly specify the *tag* context for the HTML. Normally, creating a document as a fragment will automatically detect the context based on the first tag it finds. For example, passing in `"<tr><td>a table row</td></tr>"` to `CreateFragment` -- or to `Create` when using `HtmlParsingMode.Fragment` -- would result in the fragment automatically being parsed in a `table` context since that is the only place it is allowed.

Trying to parse this same HTML snippet using any other parsing mode would result in the table tags being omitted since they can't be legally added to a DOM outside of a `table` context.

This method lets you specify the context explicitly for a fragment. While most of the time this isn't necessary, there could be situations where the context is important and CsQuery isn't able to determine it automatically.

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