### Output from CsQuery

There are a number of ways to produce output from CsQuery, and a set of associated object models for controlling how output is formatted. This document details the objects and methods used for rendering output.

#### Render and RenderSelection

The simplest is using the `Render` method, which produces a string of HTML using the default options:

    var dom = CQ.Create(...);

    string html = dom.Render();

The `Render` option will always render the *entire DOM* associated with a `CQ` object; it doesn't matter what the selection set is. You may want to render a particular element (and usually it's children). DOM element objects also have the same `Render` methods as CQ objects:

    IDomObject el = dom["div:first"][0];
    string html = el.Render();

Sometimes, you might want to render just the results of a selection. This "sometimes" is probably for debugging or testing, though, since the HTML produced from a sequence of arbitrarily selected elements is not likely to be especially meaningful.
 
    var spans = dom["span"];
    string spansHtml = spans.RenderSelection();


#### Options

There are several ways you can change the output. You can specify rendering options, which are flags to enable or disable specific features. You can specify an HTML encoder, which changes the way text is processed before being output. You can also create a completely custom implemenation of an OutputFormatter, and change anything and everything. 

#### Examples

Before delving into the details, here are some examples of creating output in different ways:

*Don't encode non-ASCII characters, pass them through as unicode.*

    var html = dom.Render(OutputFormatters.HtmlEncodingMinimum);
    var html = dom.Render(OutputFormatters.Create(HtmlEncoders.Minimum));
    var html = dom.Render(OutputFormatters.Create(new HtmlEncoderMinimum()));
    var html = dom.Render(new OutputFormatterDefault(DomRenderingOptions.Default, new HtmlEncoderMinimum());

*Discard comments*

    var html = dom.Render(DomRenderingOptions.RemoveComments);
    var html = dom.Render(OutputFormatters.Create(DomRenderingOptions.RemoveComments));
    
*Don't quote attributes, and use no HTML encoding*

    var html = dom.Render(OutputFormatters.Create(DomRenderingOptions.None,HtmlEncoders.None)

As you can see there are different ways to accomplish the same goal. Each of these examples (except for the first "discard comments" example) uses the same overload:

    public string Render(IOutputFormatter formatter)

The first "discard comments" example uses this overload:

   public string Render(DomRenderingOptions options)

This method is provided primarily for backwards compatibility and convenience; it simply creates an instance of the `OutputFormatterDefault` object using the provided options. Generally, the different examples show various ways to create an instance of an `IOutputFormatter` that does what you want. You could even create your own `OutputFormatter` entirely if you like. The DOM model has no knowledge of how to render itself; this is all contained withing the `OutputFormatterDefault` class. To create a custom output formatter, you should probably start by inheriting that class.

You can also render directly to a `TextWriter`: 

    public string Render(IOutputFormatter formatter, TextWriter writer)

*Why aren't there lots of overloads for RenderSelection?*

The `RenderSelection` method is provided for convenience, but it's not guaranteed to create any kind of valid HTML. The way some things are rendered depends on context such as its parent node; many HTML constructs are not valid outside of a particular context. So while it can be useful to look at the HTML output of a selection set, it's not useful enough that I felt it was worth cluttering the API with a lot of overloads. If you wanted to be able to use the full range of options against a selection set, it would be easy enough to just loop through the results and render each element in turn.

You can also create a new DOM from a selection, and then render that, if needed.

    var newDom = CQ.Create(dom);
    var html = newDom.Render(...);

**`DomRenderingOptions`**

You can specify certain behavior when rendering using `DomRenderingOptions`. These are flags; whenever you specify them you can specify one or more options at once. 

 * `DomRenderingOptions.None`: No option flags. This is not the same as "default", but rather explicitly uses "false" values for all flags.
 * `DomRenderingOptions.Default`: Use the global defaults as defined on `CsQuery.Config.DomRenderingOptions`
 * `DomRenderingOptions.RemoveComments`: Comments are stripped from output
 * `DomRenderingOptions.QuoteAllAttributes`: Attribute values are quoted using double-quotes (or single quotes if required by content), even if they could be rendered as valid HTML without quoting. 

The value of `CsQuery.Config.DomRenderingOptions` will be used whenever you use a method that doesn't specify options, or you specify `DomRenderingOptions.Default`. 

Some methods may allow you to specify `DomRenderingOptions` directly. In addition, these options are specified when creating an `IOutputFormatter` (see below).

**`HtmlEncoder`**

The encoding strategy used to map unicode characters to HTML is defined by a specific `IHtmlEncoder` implementation. Most of the time you won't need to worry about this. The default encoder converts ampersand to `&amp;`, left-caret to `&lt;`, right-caret to `&gt;`, and space to `&nbsp;`, and any character with a unicode value greater than 160 to `&#nnnn;`. If you want more control, or don't want to map unicode characters to HTML, you can use a different encoder (two others are provided) or roll your own. The three provided encoders are accessible as static properties:

 * `HtmlEncoders.Default`: The default encoder, works as described above.
 * `HtmlEncoders.Minimum`: Only encodes ampersant, left-caret and right-caret, the minimum needed to produce valid HTML.
 * `HtmlEncoders.None`: Does not encode anything; will most likely produce invalid HTML since carets and ampersands can be misinterpreted by the parser.


The rendering methods do not accept an `IHtmlEncoder` directly; rather, you create an `IOutputFormatter` (see below) that uses a particular encoder.

#### Global Defaults

There are several global configuration properties associated with this output model:

    IOutputFormatter CsQuery.Config.OuputFormatter {get;set;}

    IHtmlEncoder CsQuery.Config.HtmlEncoder {get;set;}
    [Flags] DomRenderingOptions CsQuery.Config.DomRenderingOptions {get;set;}

When you use a `Render` method that does not accept an output formatter, it uses a formatter obtained from the `CsQuery.Config.OutputFormatter` property. By default, this creates a new formatter object using the default `HtmlEncoder` and `DomRenderingOptions` values. You can assign a specific instance to be used as a a default instead by setting the `CsQuery.Config.OutputFormatter` property.

Another property allows you to set a delegate that returns a new object instance when a default formatter is requested instead. (This is what happens by default).

    Func<IOutputFormatter> CsQuery.Config.GetOuputFormatter {get;set;}

By assigning a delegate to this property, you can cause a new instance to be created whenever a method accesses the default output formatter.

The `OutputFormatter` property always returns either an instance, or invokes the delegate, to return the default formatter -- depending on which was assigned last. Assigning to either property supersedes any prior values of both and becomes the current implementation.

The default values for `HtmlEncoder` and `DomRenderingOptions` are only relevant when the deault `OutputFormatter` has not been changed. That is, if you override that with an instance, or a new delegate, it may not matter any more what the values of those options are, since they are used specifically by the default OutputFormatter delegate.

#### Using an IOutputFormatter



You can control the way output is created using an object that implements `IOutputFormatter`. This interface is defined in the `CsQuery.Output` namespace. In fact all output is processed through predefined `OutputFormatters`; the rendering methods that don't accept one just create an instance of the default formatter.

The API described below provides simple access to predefined `OutputFormatters`; you can also create your own implementation. These are static properties and methods that return an instance of an `IOutputFormatter` that can be used directly as a parameter value when a method requires an `IOutputFormatter`. 

* `OutputFormatters.Default` creates an an OutputFormatter configured from the global `DomRenderingOptions` and default `HtmlEncoder`
* `OutputFormatters.HtmlEncodingBasic` returns an OutputFormatter configured with basic HTML encoding. This does not use named entities except for ASCII characters less than or equal to 160 (&amp;nbsp); and encodes all other non-ASCII characters using numeric entities, e.g. &amp;#2323;
* `OutputFormatters.HtmlEncodingMinimum` returns an OutputFormatter that only encodes the minimum required for valid HTML.
* `OutputFormatters.HtmlEncodingMinimumNbsp` returns an OutputFormatter that only encodes the minimum required for valid HTML, plus ASCII 160.
* `OutputFormatters.HtmlEncodingFull` returns an OutputFormatter that encodes all known character entities using their text aliases.
* `OutputFormatters.PlainText` is a simple formatter that strips out the HTML, leaving behind the text. The formatter tries to coalesce whitespace blocks and insert new lines at sensible places based on the markup. It's not sophisticated, but should be fine for most functional purposes.

There are also methods to create OutputFormatters with specific configurations:

* `OutputFormatters.Create(DomRenderingOptions options)` creates an instance of the default formatter using the specifed options.
* `OutputFormatters.Create(IHtmlEncoder encoder)` creates an instance of the default formatter using the specifed encoder.
* `OutputFormatters.Create(DomRenderingOptions options, IHtmlEncoder encoder)` creates an instance of the default formatter using the specifed options and encoder.

*What about formatters other than the default one?*

Right now there's only one `OutputFormatter`, but in the future I'd expect to add ones to do things such as properly indent the HTML output, compress whitespace, and so on. But the idea is that you can create one for any specialized purpose. A general-purpose formatter should have a constructor accepting `DomRenderingOptions` and `IHtmlEncoder` objects, though that is not required and the purpose could be specialized enough that these settings aren't needed.

You can create your own implementation of `IHtmlEncoder` and pass it in as a contructor parameter to the default output formatter to enable custom handling for HTML encoding.