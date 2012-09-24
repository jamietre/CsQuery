##HTML5 Compliance

CsQuery is being developed towards HTML5 compliance. CsQuery is not a web browser, but an HTML parser, so these goals are primarily oriented towards parsing and some parts of the Document Object Model:

* As of Version 1.3, the HTML parser is HTML5 compliant. The old parser has been replaced with Patrick Reisert's C# port of the validator.nu parser. 
* Partial implementation of the DOM. I'm not trying to make CsQuery's DOM respond to "events" and so on. This would be a nice future, in that it could one day become part of a server-side .NET headless browser with this functionality. But the goals in the short term are the ability to parse and work with HTML and expect the same results as if you were accessing it from a client -- without any interaction of the sort that would be client initiated.

####Status

For **valid** HTML the parser currently respects all optional tags opening and closing tags under the HTML5 spec. It will insert optional (but required) tags (such as `tbody`) into the generated DOM, meaning you should be able to expect DOM consistency between CsQuery and HTML5 compliant browsers.

For **invalid** HTML, e.g. malformed or incorrectly nested tags, the parser is not HTML5 compliant.

These DOM element types are completely implemented:

    IHTMlAnchorElement
    IHTMLFormElement
    IHTMLInputElement
    IHTMLLabelElement
    IHTMLLIElement
    IHTMLMeterElement
    IHTMLOptionElement
    IHTMLProgressElement
    IHTMLSelectElement

This means the interfaces expose strongly-typed properties and return values according to the HTML5 spec for that element.

All other elements/attributes are of course available - but they don't have implementation-specific interfaces/classes. Rather, you can just access any attribute as a generic attribute. 



