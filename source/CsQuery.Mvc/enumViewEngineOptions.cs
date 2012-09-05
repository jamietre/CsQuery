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
        /// When true, scripts and dependencies that cannot be loaded will not cause an exception
        /// </summary>
        IgnoreMissingScripts=4,

        /// <summary>
        /// When false, scripts will not be minified
        /// </summary>
        NoMinifyScripts=8,

        /// <summary>
        /// When false, bundles will not be cached. (This is useful for debugging; otherwise, changes to javascript files
        /// would not be refelected until the application is restarted)
        /// </summary>
        NoCacheBundles=16
    }
}
