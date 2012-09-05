using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Mvc.ClientScript
{
    public class ScriptRef
    {
        public ScriptRef()
        {
            Dependencies = new List<ScriptRef>();
        }

        /// <summary>
        /// Normalized name of the dependency
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// Virtual path to the dependency
        /// </summary>

        public string Path { get; set; }
        public bool NoCombine { get; set; }

        public List<ScriptRef> Dependencies { get; protected set; }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            ScriptRef other = obj as ScriptRef;
            return other != null &&
                Path == other.Path;
        }

    }
}
