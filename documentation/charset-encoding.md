### Character Set Encoding Handling

There are no fewer than five different ways to tell a client what the character set of a given page should be, and these directives can conflict with each other. So it can be a little tricky to use the correct (or best) one.it's not quite as simple as it might seem. 

You can always specify the encoding yourself by passing in a stream; however, most of the time, when loading documents from the web, you'd probably rather not have to deal with that.

Fortunately the w3c provides some basic [precedence rules](http://www.w3.org/International/questions/qa-html-encoding-declarations#precedence) which CsQuery largely follows.

1. HTTP Content-Type header
2. byte-order mark (BOM)
3. XML declaration
4. meta element
5. link charset attribute

CsQuery doesn't deal with `link charset` attribute because it doesn't actually parse documents recursively; if you used CsQuery to traverse a document and load something from a `link` then you would be responsible for handling the presence of that attribute.

They go on to say that some browsers [don't follow this](http://www.w3.org/International/tests/tests-html-css/tests-character-encoding/results-html-encoding-basic) though the information is fairly dated.

We have chosen to follow Chrome as of that writing, which means that a BOM takes precedence over a Content-Type header. That is, #1 and #2 are switched in priority. In practice, it's not especially likely that this will happen.

So CsQuery works like this.

1. If a BOM is present, always use it.
2. If an HTTP `Content-Type` header is present, use it unless it conflicts with a BOM.
3. Use an XML declaration only if no type was obtained from a `Content-Type` header.
4. Use a `meta http-equiv charset` element only if no type was obtained from the prior three means.
5. Default to UTF-8.

Note that the priority of the BOM has an important consequence. If you pass in a stream that contains a BOM, and specify an alternate encoding, the BOM-identified encoding will still always be used. If you really want to override a BOM marker with a different encoding, you'll need to strip the BOM. There's a helper class  `CsQuery.HtmlParser.BOMReader` that can simplify this, e.g.

    BOMReader reader = new BOMReader(myStream);
    Stream cleanStream = reader.StreamWithoutBOM;


