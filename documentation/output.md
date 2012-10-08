###Output from CsQuery

There are a number of ways to produce output from CsQuery, and a set of associated object models for controlling how output is formatted.

#### Render and RenderSelection

The simplest is using the `Render` method, which produces a string of HTML using the default options:

    var dom = CQ.Create(...);

    string html = dom.Render();

The `Render` option will always render the *entire DOM* associated with a `CQ` object; it doesn't matter what the selection set is. You may want to render a particular element (and usually it's children). DOM element objects also have the same `Render` methods as CQ objects:

    IDomObject el = dom["div:first"][0];
    string html = el.Render();

Sometimes, you might want to render just the results of a selection (though this "sometimes" is probably for debugging or testing - a selection could be arbitrarily connected elements; it may not even result in valid HTML when put together):
 
    var spans = dom["span"];
    string spansHtml = spans.RenderSelection();

#### Options

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

#### Using an IOutputFormatter

You can control the way output is created using an object that implements `IOutputFormatter`. This interface is defined in the `CsQuery.Output` namespace. In fact all output is processed through predefined `OutputFormatters`; the rendering methods that don't accept one just create an instance.

The API described below provides simple access to predefined `OutputFormatters`; you can also create your own implementation. These are static properties and methods that return an instance of an `IOutputFormatter` that can be used directly as a parameter value when a method requires an `IOutputFormatter`. 

 * `OutputFormatters.Default` uses the global `DomRenderingOptions` and default `HtmlEncoder`
 * `OutputFormatters.Create(DomRenderingOptions options)` creates an instance of the default formatter using the specifed options.
* `OutputFormatters.Create(IHtmlEncoder encoder)` creates an instance of the default formatter using the specifed encoder.
* `OutputFormatters.Create(DomRenderingOptions options, IHtmlEncoder encoder)` creates an instance of the default formatter using the specifed options and encoder.

*What about formatters other than the default one?*

Right now there's only one `OutputFormatter`, but in the future I'd expect to add ones to do things such as properly indent the HTML output, compress whitespace, and so on. But the idea is that you can create one for any specialized purpose. A general-purpose formatter should have a constructor accepting `DomRenderingOptions` and `IHtmlEncoder` objects, though that is not required and the purpose could be specialized enough that these settings aren't needed.

You can create your own implementation of `IHtmlEncoder` and pass it in as a contructor parameter to the default output formatter to enable custom handling for HTML encoding.