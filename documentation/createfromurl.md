###Creating CsQuery objects from a URL

In the Create documentation the basic methods are described:

#####Create synchronously from a URL

    CQ dom = CQ.CreateFromUrl("http://www.jquery.com");

    CQ.CreateFromUrlAsync("http://www.jquery.com", successDelegate, failureDelegate);

    IPromise promise = CQ.CreateFromUrlAsync("http://www.jquery.com");

The first method is straightforward. However, it provides limited error handling capabilities, and can be very time consuming since your application will stop until the request has finished. Any problems could cause lengthy pauses. So, it's usually preferable to use one of the asynchronous methods.

#####Calling delegates when finished

The second method above invokes a method when the request finishes successfully, or fails. For example:

    void WhenFinishedAction(ICsqWebResponse response) {
        var dom = response.Dom;
        // do stuff with dom
    }

    CQ.CreateFromUrlAsync("http://something.com",WhenFinishedAction);

The method `WhenFinishedAction` is called when the contents of the URL have been loaded. The `ICsqWebResponse` object includes a property `Dom` that has the CQ object of the URL's contents, as well as other information about the response. An optional third parameter would be called in the event of failure.

#####Promises

A more flexible way to use the `CreateFromUrlAsync` method is with [Promises](https://github.com/jamietre/CsQuery/blob/master/documentation/create.md). This is a concept borrowed from Javascript that simplifies handling of asynchronous events. The usage is covered in detail on the page referenced. Simply, it's used like this:

    // start two requests

    IPromise promise1 = CQ.CreateFromUrlAsync(someUrl);
    IPromise promise2 = CQ.CreateFromUrlAsync(someOtherUrl);

    // Call a function ProcessResponse(ICsqWebResponse response) when the 1st request is done

    promise1.Then(ProcessResponse);

    // Do something inline when the 2nd is done

    var jsnodes;
    promise2.Then((response) => {
        jsnodes = response.Dom["script[type='text/javascript']"];
    });
   
    // Do something when they're all done

    When.All(promise1,promise2)
        .Then(AllFinishedMethod);

   // Account for slow responses by adding a 5 second timeout

    When.All(5000,promise1,promise2)
        .Then(AllFinishedSuccessfullyMethod, AllFinishedWithErrorsMethod);

#####Accessing the HttpWebRequest

If you need more control over the way the request is made, the `CsQuery.Web.CsqWebRequest` object can be used to create your requests directly. 

    var request = new CsQuery.Web.CsqWebRequest("someurl.com");
    IHttpWebRequest httpRequest  = cqRequest.GetWebRequest();
    
    .. do stuff to httpRequest 

    var html = request.Get(httpRequest);
    var dom = CQ.Create(html);

The `IHttpWebRequest` interface is a wrapper for the .NET framework `HttpWebRequest` object; it contains all the same properties and methods. You can use this to change the configuration of the request before it is completed.

You can also start asynchronous requests directly using the `CsQuery.Web.AsyncWebRequestManager` object. This is used internally by the `CQ.CreateFromWebAsync` methods, but you can also access it directly:

    CsQuery.Web.AsyncWebRequestManager.StartAsyncWebRequest(request, successDelegate, failDelegate);

Using promises with this method requires a little plumbing, since the promise API is part of CsQuery, not the HTTP subsystem. But it's easy enough to create your own wrapper using the promises API in CsQuery:


    IPromise StartAsync(CsqWebRequest request) {
  
        // create a deffered object that has a parameter of type ICsqWebResponse

        var deferred = When.Deferred<ICsqWebResponse>();

        // start the async request, and use the deferred object's Resolve(parm) and Reject(parm) methods as the delegates

        CsQuery.Web.AsyncWebRequestManager.StartAsyncWebRequest(request, deferred.Resolve, deferred.Reject);
    }

Now you've got a function that invokes the `AsyncWebRequestManager` and returns a promise that resolves when the request is complete:

    var request = new CsQuery.Web.CsqWebRequest("someurl.com");
    IHttpWebRequest httpRequest = cqRequest.GetWebRequest();
    
    .. do stuff to httpRequest 
    
    StartAsync(request).Then((response)=> {
        var dom = response.Dom;
        ... do something with it
    });
