##HTML5 Compliance

CsQuery is being developed towards HTML5 compliance. CsQuery is not a web browser, so what this means could be debatable. The goals at this time are:

* Complete implementation of the HTML5 parsing rules for optional closing tags and invalid markup. This means that the DOM which CsQuery presents you with should mimic the DOM you would access from the same markup in a fully HTML5-compliant browser.
* Partial implementation of the DOM object model. I'm not trying to make CsQuery's DOM respond to "events" and so on. This would be a nice future, in that it could one day become part of a server-side .NET headless browser with this functionality. But the goals in the short term are the ability to parse and work with HTML and expect the same results as if you were accessing it from a client -- without any interaction of the sort that would be client initiated.

####Status

The DOM element types that are completely implemented:

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

At the present, the status is "just getting started." The parser more or less complies with optional tag parsing rules: it should render the same markup as an HTML5 browser given any valid HTML5 markup. It will determine when tags should be closed based on these rules and close them; and it will add optional elements such as `tbody` and `title` if missing.

It does not currently comply with invalid parsing rules, though it uses reasonable set of rules. Any similarity to an HTML5-compliant parser is coincidental. 

The DOM object model was initially designed to be as simple as possible. To that end, most commonly-used `HtmlElement` properties were included on the low-level interface `IDomObject`. I decided early on that I didn't want to require typecasting every different kind of node to access properties; this was inconvenient and didn't match the "non-typed" experience one has when working with javascript.

As the project evolved, I wanted to allow the model to more closely match the real DOM. This mean at some level moving away from the "everything under one roof" model. Some properties have overloaded return types; besides which, adding everything to the single interface would be horrendously cluttered. Adding logic to properties which function differently across different elements would be messy with a single object. 

To this end I started creating derived object interfaces and imlementations. 


