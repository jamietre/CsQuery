using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Mvc
{
    /// <summary>
    /// Flags that control operation of the CsQueryViewEngine
    /// </summary>

    [Flags]
    public enum ViewEngineOptions
    {
        /// <summary>
        /// Enables script manager functionality to resolve dependencies.
        /// </summary>
        EnableScriptManager=1,

        /// <summary>
        /// When set, all scripts (not just those created with @Html.Script) are resolved.
        /// </summary>
        ProcessAllScripts=2,

        /// <summary>
        /// When set, scripts and dependencies that cannot be loaded will not cause an exception
        /// </summary>
        IgnoreMissingScripts=4,

        /// <summary>
        /// When false, scripts will not be minified
        /// </summary>
        NoMinifyScripts=8,

        /// <summary>
        /// When set, caching is disabled. This results in the bundles being recreated on each page load, and scripts 
        /// being re-analyzed for dependencies. (This is useful for debugging; otherwise, changes to javascript files
        /// would not be refelected until the application is restarted)
        /// </summary>
        NoCache=16,


        /// <summary>
        /// When set, scripts are resolved, but not bundled.
        /// </summary>
        NoBundle = 32,

        /// <summary>
        /// When true, the Visual Studio format XML &lt;reference ... &gt; tags will be resolved by the script manager as "using" 
        /// </summary>
        ResolveXmlReferences = 64
    }
}
