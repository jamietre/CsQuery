### Indexing

Starting with version 1.3.5, you can choose an indexing strategy that will be used when creating new CQ objects. This can have significant performance considerations. 

A simple approach to indexing an HTML document would be to build a cross-reference of everything that you're likely to be looking for: node names such as "div" and "span"; attribute names; the value of ID attributes, and CSS classes. This is quick to build, and would result in an index that performs with <i>o(1)</i> for any simple query that returns results from the entire document:

   CQ results = dom["#my-section"];
   CQ results = dom[".some-class"];

However, it won't perform as well for any query that's within a context. For example:

    CQ results = dom["div span"]

will return all "span" elements that are descendents of "div" elements. With a simple index, you'd be able to locate all the "div" elements easily enough. But then you'd have to resort to looking through every single descenant of every match to find the "span" elements within. This might be almost the entire document.

To deal with this, CsQuery builds an index that contains knowledge of each node's position in the DOM and can return a range that falls within an existing selection in <i>o(1)</i>. This means you get overall performance of <i>o(a)</i> where <i>a</i> is the number of levels in the query. Without a simple index, performance for complex queries could approach *o(n)* where *n* is the number of elements in the document!

However, this comes at a cost. It is much more expensive to build the subselect-capable index, generally *at least doubling* the time it takes to parse the document up front. For some purposes, this tradeoff isn't warranted. If you mostly use simple selectors, or the documents are typically small, a simpler index may make more sense.

#####Changing the default indexing strategy

Starting with version 1.3.5. you can change the indexing strategy. This is done using a service locator on the global configuration. The three built-in strategies are:

     Config.DomIndexProvider = DomIndexProviders.Ranged;
     Config.DomIndexProvider = DomIndexProviders.Simple;
     Config.DomIndexProvider = DomIndexProviders.None;

You can create a new CQ object with a different strategy from the default one, though there's no simple method right now to do so. You'd do it as follows, by instantiating `ElementFactory` (the parser front end) directly using your chosen index strategy:


    var factory = new ElementFactory(DomIndexProviders.Simple);

    // ToStream is an extension method to convert a string to UTF8 stream
    
    using (var stream = html.ToStream())
    {
        var document = factory.Parse(stream, Encoding.UTF8);
        CsqueryDocument_Simple = CQ.Create(document);
    }

Future versions may encapsulate this in some kind of create method, though I am trying to avoid changing the primary public API until the next major release.

*As a note, browsers generally parse CSS selectors from right to left, rather than left to right. This means that they would first find all "span" elements, and identify the ones that are children of "div" elements. This fact means that optimizing queries in browsers is different from optimizing them in CsQuery; however, the goal of building this kind of index is to make optimizing unnecessary*.

