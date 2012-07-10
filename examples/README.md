##Sample Applications

This folder contains examples of how to use CsQuery in an application. It's small right now but I will set up new examples as fast as I can.

The basic uses are:

#####Inline HTML processing in an MVC application

Use CsQuery to let you manipulate the HTML rendered after Razor is done with it, but before it's sent to the client. 

Example: `CsQuery.MvcApp`

#####Inline HTML processing in an ASP.NET application

Use CsQuery to let you manipulate the HTML rendered after Webforms is done with it (and after any server-side WebControl manipulation), but before it's sent to the client.

Example: `CsQuery.WebFormsApp`

#####Web Scraping

CsQuery can make requests and process the response from remote servers. Use it for web scraping the same as you would any other tool, except, of course, you get the power of CSS3 and jQuery selectors.

*No Example Yet, But Look At "Csharp/Miscellaneous/_WebIO" Tests.*

#####Real-time integration of remote content

Use CsQuery to query remote web servers from the *server* instead of doing that from Javascript. This avoid all problems with cross-domain requests -- but more importantly, your clients will never have to wait for a remote request to finish every time the page loads. 

That is, instead of making a request to a remote server with each page being served, you serve up the data from the *prior* request that you've cached already. If the cache is stale, then initiate a new asynchronous (non-blocking) request. Whenever that request finishes, the cache will be updated. But nobody will ever have to wait for it.

*No Example Yet*

#####Anything else you can think of

CsQuery is being used for testing web sites - it's a piece of cake to use it to verify the output of some page matches expected values. 

You can use it to parse XML -- XML is also valid HTML.