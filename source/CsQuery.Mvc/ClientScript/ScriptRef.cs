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

        private string _Name;

        /// <summary>
        /// The name in normalized form e.g. "folder/script.js". If this is from a libary, it should be relative to the
        /// base of its library folder. It should never include tildes and always include extensions.
        /// </summary>
        ///
        /// <value>
        /// The name.
        /// </value>

        public string Name { 
            get {
                return _Name;
            }
            set{
                _Name = value.ToLower();
            } 
        }

        /// <summary>
        /// Virtual path to the dependency
        /// </summary>

        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dependency path has been resolved. 
        /// </summary>

        public bool Resolved { get; set; }

        /// <summary>
        /// When true, the script should not be combined but always loaded individually.
        /// </summary>

        public bool NoCombine { get; set; }

        public List<ScriptRef> Dependencies { get; protected set; }

        public override int GetHashCode()
        {
            return Path != null ? Path.GetHashCode() : 0;
        }
        public override bool Equals(object obj)
        {
            ScriptRef other = obj as ScriptRef;
            return other != null &&
                Path == other.Path;
        }

        public override string ToString()
        {
            return "[" + Name + "]: \"" + Path + "\"";
        }

        public static bool operator ==(ScriptRef x, ScriptRef y)
        {
            return AreEqual(x, y);
        }
        public static bool operator !=(ScriptRef x, ScriptRef y)
        {
            return !AreEqual(x,y);
        }

        private static bool AreEqual(ScriptRef x, ScriptRef y) {
            bool xIsNull = ReferenceEquals(x, null);
            bool yIsNull = ReferenceEquals(y, null);

            if (xIsNull != yIsNull)
            {
                return false;
            }
            else
            {
                return xIsNull || x.Equals(y);
            }
        }
    }
}
