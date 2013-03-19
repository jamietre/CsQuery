### Promises in CsQuery

**Note: this will be deprecated in version 2.0 and replaced with async methods**

More recent versions jQuery introduced a "deferred" object for managing callbacks using a concept called *Promises*. Though this is less relevant for CsQuery because your work won't be interactive for the most part, there is one important situation where you will have to manage asynchronous events: loading data from a web server.

The `CQ.CreateFromUrlAsync` method can return an `IPromise<ICsqWebResponse>` object. The basic promise API (from CommonJS Promises/A) has only one method:

    then(success,failure,progress)

In Javascript, you write code like this:
    
    someAsyncAction().then(function(result) { 
        // do something upon success
    },function(error) {
        // do something upon failure  
    });

When the action is resolved, the first function is called with an optional parameter from the caller. If it failed, the second function is called. 

In our C# version, the interface is a little more complicated because we care about the `type` of the response parameter. I've also chosen to leave out the progress parameter. So it looks like this:

    public interface IPromise
    {
        IPromise Then(Delegate success, Delegate failure=null);
        IPromise Then(Action success, Action failure = null);
        IPromise Then(Func<IPromise> success, Func<IPromise> failure = null);
        IPromise Then(PromiseAction<object> success, PromiseAction<object> failure = null);
        IPromise Then(PromiseFunction<object> success, PromiseFunction<object> failure = null);
    }

`PromiseAction` and `PromiseFunction` are just delegates for a methods that accept a parameter.

    public interface IPromise<T> : IPromise
    {
        IPromise Then(PromiseAction<T> success, PromiseAction<T> failure = null);
        IPromise Then(PromiseFunction<T> success, PromiseFunction<T> failure = null);
    }

These overloads for `Then` allow you to bind to methods or inine functions that are of type `void` or `IPromise`, and accept no parameters or a single parameter.

#####Using Promises to load content from a remote URL

The signature for `CreateFromUrlAsync` is this:

    IPromise<ICsqWebResponse> CreateFromUrlAsync(string url, ServerConfig options = null)

This makes it very simple to write code with success & failure handlers inline. By strongly typing the returned promise, you don't have to cast the delegates, as in the original example: the `response` parameter is implicitly typed as `ICsqWebResponse`. If I wanted to add a fail handler, I could do this:

    CQ.CreateFromUrlAsync(url)
        .Then(responseSuccess => {
            var dom = responseSuccess.Dom;
            // do something
        }, responseFail => {
            // do something
        });

#####The Deferred Object

CsQuery provides a `Deferred` object. This is simply a promise with methods to resolve or reject the promise. 

    bool? success = null;

    var deferred = new Deferred();
    deferred.Then(()=> {
        success=true;
    }, ()=> {
        success=false;
    }

    deferred.Resolve();
    Assert.IsTrue(true,success);    // passes
    
#####The Timeout Object

There's also a `Timeout` object, which creates a promise that resolves or rejects after the specified time:

    bool done=false;
    
    var timeout = new Timeout(2000); // 2 seconds

    timeout.Then(null,()=> {
        done=true;
    });
    Assert.IsFalse(done);
    System.Threading.Thread.Sleep(3000);  // wait 3 seconds
    Assert.IsTrue(done);   // passes

You'll notice that I used `null` as the first parameter to `Then`. This is because by default, `Timeout` rejects rather than resolves when the time has elapsed, since usually you'll use this to limit asynchronous processes to a certain amount of time.

The Timeout object has constructors to determine whether the result should be success or failure, and also to include a value to pass as the parameter when it resolves.

You can also stop a timer prematurely with the `Stop` method. This will cause it's outcome (resolve or reject) to occur immediately. If you want to change the outcome when you stop it, call `Stop` with a single boolean value: `true` to resolve or `false` to reject.

These `Deferred` and `Timeout` objects are used internally to provide the functionality for the asynchronous methods, but you can also use them yourself for interacting with other asynchronous events.

#####The When Object

Most of the functionality of the promise API can be used from the `When` object. This also provides another very useful promise-related function called `When.All`. This is roughly equivalent to `Task.WhenAll` in C# 5. It lets you create a new promise that resolves when every one of a set of promises has resolved:

    var promise1 = CQ.CreateFromUrlAsync(url);
    var promise2 = CQ.CreateFromUrlAsync(url2);

    When.All(promise1,promise2).Then(successDelegate, failDelegate);

You can also give it a timeout which will cause the promise to reject if it has not resolved by that time. This is valuable for ensuring that you get a resolution no matter what happens in the client promises:

    // Automatically reject after 5 seconds

    CsQuery.When.All(5000,promise1,promise2)
        .Then(successDelegate, failDelegate);

`When` is a static object that is used to create instances of promise-related objects. You can also use it to create your own deferred entities:

    var deferred = When.Deferred();
    
    deferred.Then(successDelegate, failDelegate);
    deferred.Resolve();   // causes successDelegate to run

Another interesting thing about promises is that they can be resolved *before* the appropriate delegates have been bound and everything still works:

    var deferred = When.Deferred();

    deferred.Resolve();
    deferred.Then(successDelegate, failDelegate);   // successDelegate runs immediately


####Example

Making a request to a web server can take a substantial amount of time, and if you are using CsQuery for a real-time application, you probably won't want to make your users wait for the request to finish.

I use CsQuery to provide current status information on the "What's New" section for the [ImageMapster](http://www.outsharked.com/imagemapster/) (my jQuery plugin) web site. But I certainly do not want to cause every single user to wait while the server makes a remote web request to GitHub (which could be slow or inaccessible). Rather, the code keeps track of when the last time it's updated it's information using a static variable. If it's become "stale", it initiates a new async request, and when that request is completed, it updates the cached data. 

So, the http request that actually triggered the update will be shown the old information, but it will start the process of updating the cached data. Any requests coming in after the request to GitHub has finished will of course use the new information. The code looks like this:

    private static DateTime LastUpdate;
    private static string CurrentVersion;
    private static string CommitHtml;
    
    // see if it's been more than 4 hours since our last update

    if (LastUpdate.AddHours(4) < DateTime.Now) {

        // stale - start the update process.
    
        string commitsUrl="https://github.com/jamietre/ImageMapster/commits/master";

        // CreateFromUrlAsync returns an `IPromise<ICsqWebResponse>` object. This means that
        // the delegate for Then will have a parameter of type `ICsqWebResponse`, which
        // has a `Dom` property containing a CQ object of the results
		
		IPromise promise1 = CQ.CreateFromUrlAsync(commitsUrl)
           .Then(response => {
	            var gitHubDom = response.Dom;          
                // parses the commit info & builds the table for the web site
                commitHtml = ExtractCommitInfo(gitHubDom);
	        });
 
        // Start another simultaneous request against the build, to extract the
        // current version number

		string scriptUrl="https://github.com/jamietre/ImageMapster/blob/master/dist/jquery.imagemapster.js";

        IPromise promise2 = CQ.CreateFromUrlAsync(scriptUrl)
            .Then(response=> {
	            buildDom = response.Dom;
                currentVerison = ExtractVersionFromScript(buildDom);
            });

        // When.All returns a new promise that resolves when each of the promises
        // it was passed are resolved, meaining we've gotten all our data.

        When.All(promise1,promise2).Then(()=> {
			LastUpdate = DateTime.Now;
        });
    }


Though C# 5 includes some language features & methods that greatly improve asynchronous handling (see `await` and the `Task` class), I'm not using C# 5 and I dind't want to "wait". Besides, the promise API used often in Javascript is actually extraordinarily elegant so I decided to make a basic C# implementation to assist in using this method. When VS 2012 comes out I may revisit this.
