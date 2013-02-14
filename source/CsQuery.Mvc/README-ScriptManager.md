##CsQuery.MVC Script Manager

The ScriptManager is an optional component of the CsQuery.MVC framework. It is enabled with the 
`EnableScriptManager` option on the view engine.

When activated it prodives the following functionality:

* Client scripts can include a reference section in comments at the top of the 
  script that will automatically load dependent scripts
* Dependencies can be bundled using MVC 4 Bundles.
* Bundles are created as needed but cached - a bundle will be served with the same URL if the 
  scripts that make it up have not changed since the last load. Caching can be disabled for testing.

The convention for loading dependencies is not intended to tell you how to actually use them. 
It simply ensures the script is loaded and run on each page. For example, if you load a jQuery plugin, 
there should be nothing more to do: it will be available. If you load a node module, you'll need to use 
a `require` implementation designed for browsers to access it. The intent is that your web page works 
in exactly the same way it would if you just loaded each script individually. 

If a script has already been included on the page with a `script` element, it won't be added to a bundle, 
even if a later script requires it.  That is - this won't ever *remove* a script directive from your page. 
However, it will not duplicate the same script in a bundle either, if it's already loaded.

Dependencies are identified with XML format tags of the same structure visual studio uses:

    /// <reference path="require.js" />
	/// <reference path="jquery.js" />

You can also add options:

    /// <reference path="jquery.js" nocombine/>

Valid options are:

* nocombine - will add a `script` reference for this script, but will not combine it when creating bundles. 
  This is useful for large scripts that appear on many pages such as frameworks.
* ignore - will ignore this entry. For example, if you are using `reference` tags for intellisense, but this script
  is already part of a bundle, you can add this to cause it to be ignored.

These directives can actually appear in any kind of comment, however, by using the triple-slash 
format you'll also get intellisense. But they must appear before the first non-comment or non-empty/whitespace line.

The target of the `using` is a file name (without the `.js.` extension; it is optional.) The file will be located by search the `LibraryPath` assigned to the view engine. An example config:

    ViewEngine.Options = ViewEngineOptions.EnableScriptManager;
    ViewEngine.LibraryPath.Add("~/scripts/libs");
    ViewEngine.LibraryPath.Add("~/scripts/thirdparty");

Search is *not* recursive; rather, subfolders are expected to be used like namespaces. For example:

    using jquery.tooltips;

would locate the file "tooltips" in subfolder "jquery" of anything on the library path. You can use slashes too, if you prefer.

#### Options

You can pass options as a third keyword on a `using` directive, or as keyword after a `using-options` directive. 
The latter case makes this option apply to the file itself. For example:

/* using jquery nocombine */

Will include the script `jquery.js` but will not bundle it, instead, it will add a direct script reference.

#### Script tag options

`data-moveto="head|selector"`	Will relocate a script as the last child of the first element matching the selector (head, of course, is also just a selector)

(not implemented)

You can invoke bundling on script tags by adding a `data-bundle` tag, or optionally `data-bundle="bundle-name"`.
