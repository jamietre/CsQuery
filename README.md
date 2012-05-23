## CsQuery - C# jQuery Port

5/7/12

Release 1.0 Beta 1

CsQuery is a jQuery port for .NET 4. It implements all the CSS selectors and DOM manipulation methods of jQuery and some of the utility methods. The majority of the jQuery test suite (as of 1.6.2) is ported and passes. The project necessarily includes an object model that represents the browser DOM.

### Usage

    // create

    var dom = CQ.Create(htmlString);
    var dom = Server.CreateFromUrl("http://www.microsoft.com/en/us/default.aspx?redir=true");
    var dom = Server.StartAsyncWebRequest("http://www.jquery.com",response => { ...  },1);

    // manipulate

    dom.Select("div > span").Eq(1).Text("Change the text content of the 2nd span child of each div");

    // the default property indexer is equivalent to "select"
    
    var rowsWithClass = dom[".targetClass"].Closest("td");

    // output

    var html = dom.Render();
    var elementHtml = dom[2].Render();
    var selectionHtml = dom[".just-this-class"].RenderSelection();

   // DOM

    dom.Each((i,e) => {
        if (e.id == "remove-this-id") {
            e.Parent().RemoveChild(e);
        }
    });


### Performance

Including HTML parsing, about twice as as fast as HtmlAgilityPack + Fizzler in some simple tests. Tags, class, and attribute values are indexed using a subselect-capable index, meaning that complex selectors still benefit from the index. A lot more analysis is needed to say more, but I use it in real time and it's plenty fast. 



### Shortcomings

The DOM model is not perfect. Mimicing the browser DOM would sacrifice the benefits of strong typing; I opted for a compromise that exposes some nonapplicable members on all node types. This probably could use some refactoring at this point, but it's perfectly workable.

Most of the code for loading HTML from URI sources is minimally tested and in the "prototype" stage. I actually use this rarely my own purposes (most of what I do is real-time parsing of files being served during HTTP requests) but since scraping is an obvious use for this, I added them. They need a lot of attention, particularly the async request, but work well enough in simple cases.

There are some minor API issues that need resolving.


### Object Model

    CQ                    like $, a jQuery object
    Selectors             a Selectors object (contains one or more Selector objects, defines a selection set)
    Selector              a single selector

### Important nonstandard methods

    CQ.Elements           Only the element results of the selection.

`Elements` is important because of strong typing in C# vs. Javascript. The default enumerator exposes interface `IDomObject`, an interface common to all node types. As such it has very few standard DOM node methods. Most of the time, you only care about element nodes; this method provides the results in that cast.

    CQ.Create()           Constructor for new DOM, same as $(...) (when passed HTML or elements)

    CQ.Render()           Output the entire DOM as an html string

    CQ.RenderSelection()  Output only the selection set as an html string

