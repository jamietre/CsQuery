﻿/****** Installation Instructions ******

1. Update the Layout

Open the /Views/_ViewStart.cshtml
Change the Layout to _Foundation.cshtml like the example below:

@{
    //This is the default MVC template
    //Layout = "~/Views/Shared/_Layout.cshtml";
    
    //This is the Foundation MVC template
    Layout = "~/Views/Shared/_Foundation.cshtml";
}

2. Remove the default theme

Once the ViewStart has been updated. Replace the default Index.cshtml:

Rename ~/Views/Home/Index.cshtml to Index.cshtml.exclude (or delete the file)
Rename ~/Views/Home/Foundation_Index.cshtml to Index.cshtml

Replace the default style:

Rename ~/Content/Site.css to Site.css.exclude (or delete the file)

3. Automatic Bundling and Minification

Open the /App_Start/BundleConfig.cs
Add the following bundles:

      #region Foundation Bundles
      bundles.Add(new StyleBundle("~/Content/foundation/css").Include(
                 "~/Content/foundation/foundation.css",
                 "~/Content/foundation/foundation.mvc.css",
                 "~/Content/foundation/app.css"));

      bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
                "~/Scripts/foundation/jquery.*",
                "~/Scripts/foundation/app.js"));
      #endregion

4.

You are now ready to begin building your MVC project using Foundation.

/****** Related Nuget packages ******
Want to rapid prototype and wire frame directly from code using Html Helpers? 
Try the prototyping package on nuget. It works great with Foundation.
http://www.nuget.org/packages/Prototyping_MVC

Having trouble with media queries? Debug them with this simple CSS file.
http://nuget.org/packages/CSS_Media_Query_Debugger

/****** Documentation ******
Docs http://foundation.zurb.com/docs/
Demo http://edcharbeneau.github.com/FoundationSinglePageRWD/

Articles:
Responsive design using Foundation with asp.net MVC http://www.simple-talk.com/dotnet/asp.net/responsive-design-using-foundation-with-asp.net-mvc/
Prototyping Article http://www.simple-talk.com/dotnet/asp.net/rapid-prototyping,-the-mvc-working-model/

Presentations:
A Crashcourse in Responsive Design http://www.slideshare.net/edcharbeneauii/a-crash-course-in-responsive-design-12007229
Don't be a stereotype rapid prototype http://www.slideshare.net/edcharbeneauii/dont-be-a-stereotype-rapid-prototype

Follow us:
Ed Charbeneau http://twitter.com/#!/edcharbeneau
Foundation Zurb http://twitter.com/#!/foundationzurb

Change Log:
Version 3.0.325
Strengthens the compatibility with jQuery 1.9, including:
    Updates to the Foundation Topbar, including die() and live()
    Update to Magellan javascript that removes calls to obsolete event listeners in jQuery
    Also, Foundation 3.2.5 is fully compatible with jQuery 1.7 and above. However, before upgrading, we recommend that you read over the changes in the jQuery 1.9 Upgrade Guide. 
    http://zurb.com/article/1157/foundation-3-2-5-gets-up-close-and-person

Version 3.0.324
Added MVC CSS overrides. 
  The foundation.mvc.css file contains CSS overrides that are specific to making sure that Foundation works well with MVC

Updated Foundation core to 3.2.4
See Foundation changelog for details http://foundation.zurb.com/docs/changelog.php

Version 2.1.32

Fixed a typo on the dependency: Foundation3_Core

Version 2.0.32

Updated Foundation core to 3.2
Removed HTMLShiv from _Foundation.cshtml
-HTMLShiv is part of modernizr.js

Version 1.0.311
Initial NuGet Release

Note: version scheme <major>.<minor>.<foundation version>
foundation version represents the foundation version less the "." for example 2.1.4 would be #.#.214

Foundation Framework Support:
http://foundation.zurb.com/docs/support.php