##HTML5 Compliance

CsQuery is being developed towards HTML5 compliance. CsQuery is not a web browser, but an HTML parser, so these goals are primarily oriented towards parsing and some parts of the Document Object Model:


As of Version 1.3, the HTML parser is **HTML5 compliant.** The old parser has been replaced with Patrick Reisert's C# port of the validator.nu parser. This is the same code base used for the Gecko engine.

The DOM interface is partially implemented. I'm not trying to make CsQuery's DOM respond to "events" and so on. This would be a nice feature, in that it could one day become part of a server-side .NET headless browser with this functionality. But the goals in the short term are the ability to parse and work with HTML and expect the same results as if you were accessing it from a client -- without any interaction of the sort that would be client initiated.

These element types are completely implemented:

    IHTMlAnchorElement
    IHTMLFormElement
    IHTMLInputElement
    IHTMLLabelElement
    IHTMLLIElement
    IHTMLMeterElement
    IHTMLOptionElement
    IHTMLProgressElement
    IHTMLSelectElement

This means the interfaces expose strongly-typed properties and return values according to the HTML5 spec for that element. Sometimes, the data returned by a DOM element's property may be tightly bound with browsers state. CsQuery's model will return valid values, but they may not make much sense (e.g. "Progress").

All elements and attributes are still available - but they don't have node-specific interfaces designed yet. For everything else, you can just access any attribute as a generic attribute. 



