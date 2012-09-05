##CsQuery.MVC Script Manager

The ScriptManager is an optional component of the CsQuery.MVC framework. It is enabled with the `EnableScriptManager` option on the view engine.

When activated it prodives the following functionality:

* Client scripts can include a "using" section that will automatically load dependent scripts
* Scripts can be bundled using MVC 4 Bundles.
* Bundles are created as needed but cached - a bundle will be served with the same URL if the scripts that make it up have not changed since the last load.
* Caching can be disabled

Dependencies are identified with "using":

    /* using require;
       using jquery;
    */


The `using` directives can appear in any kind of comment, but must appear before the first non-comment or non-empty/whitespace line.

The target of the `using` is a file name (without the `.js.` extension; it is optional.) The file will be located by search the `LibraryPath` assigned to the view engine. An example config:

    ViewEngine.Options = ViewEngineOptions.EnableScriptManager;
    ViewEngine.LibraryPath.Add("~/scripts/libs");
    ViewEngine.LibraryPath.Add("~/scripts/thirdparty");

Search is *not* recursive; rather, subfolders are expected to be used like namespaces. For example:

    using jquery.tooltips;

would locate the file "tooltips" in subfolder "jquery" of anything on the library path. You can use slashes too, if you prefer.

#### Options

You can pass options as a third keyword on a `using` directive, or as keyword after a `using-options` directive. The latter case makes this option apply to the file itself. For example:

/* using jquery nocombine */

Will include the script `jquery.js` but will not bundle it, instead, it will add a direct script reference.

This directive at the top of `jquery.js` will cause it to never be bundled:

/* using-options nocombine */
