### Character Set Encoding Handling

There are no fewer than five different ways to tell a client what the character set of a given page should be, and these directives can conflict with each other. So it can be a little tricky to use the correct (or best) one.

You can always specify the encoding yourself by passing in a `Stream` and specifying the encoding; however, most of the time, when loading documents from the web, you'd probably rather not have to deal with that.

Fortunately the w3c provides some basic [precedence rules](http://www.w3.org/International/questions/qa-html-encoding-declarations#precedence) which CsQuery mostly follows.

1. HTTP `Content-Type` header
2. byte-order mark (BOM)
3. XML declaration
4. meta element
5. link charset attribute

CsQuery doesn't deal with #5, the `link charset` attribute, because it doesn't actually parse documents recursively.  If you used CsQuery to traverse a document and load something from a `link` then you would be responsible for handling the presence of that attribute.

The w3c discussion also notes that some browsers [don't follow this](http://www.w3.org/International/tests/tests-html-css/tests-character-encoding/results-html-encoding-basic) priority map, though the information is fairly dated.

We have chosen to follow Chrome as of that writing, which means that a BOM takes precedence over a Content-Type header. That is, #1 and #2 are switched in priority. In practice, it's not especially likely that this will happen. That would mean a file was transmitted with a BOM marker that was valid, but did not reflect the bytes that followed. This would involve a file being deliberately encoded with an incorrect marker.

On the other hand it seems more likely that the opposite situation could come up: a content-type header didn't reflect the actual encoding of a file, because the web server simply ignored the file's encoding. So we, like Chrome, have chosen to honor the BOM first.

So CsQuery works like this.

1. If a BOM is present, always use it.
2. If an HTTP `Content-Type` header is present, use it unless it conflicts with a BOM.
3. Use an XML declaration only if no type was obtained from a `Content-Type` header or a BOM.
4. Use a `meta http-equiv charset` element only if no type was obtained from the prior three means.
5. Default to UTF-8 if no other guidance was provided from #1 through #4.

Note that the priority of the BOM applies to any stream passed to CsQuery `Create` methods, not just to content loaded from the web. So if you pass in a stream that contains a BOM, and specify an alternate encoding, the BOM-identified encoding will still always be used. If you really want to override a BOM marker with a different encoding, you'll need to strip the BOM. There's a helper class  `CsQuery.HtmlParser.BOMReader` that can simplify this, e.g.

    BOMReader reader = new BOMReader(myStream);
    Stream cleanStream = reader.StreamWithoutBOM;

The situation where a BOM that does not correctly identify its stream, but you are otherwise privy to the correct encoding, seems contrived. So, purposely circumventing the BOM is not intended to be easy.

