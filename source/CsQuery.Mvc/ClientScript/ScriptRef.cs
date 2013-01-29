using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Mvc.ClientScript
{
    /// <summary>
    /// A class representing a reference to a client script.
    /// </summary>

    public class ScriptRef
    {
        /// <summary>
        /// Default constructor.
        /// </summary>

        public ScriptRef()
        {
            Dependencies = new List<ScriptRef>();
        }

        private string _Path;


        /// <summary>
        /// Return the relative path root for this file, e.g. the path excluding the file name.
        /// </summary>

        public string RelativePathRoot
        {
            get
            {

                return Path.BeforeLast("/")+"/";
            }
        }


        /// <summary>
        /// Virtual path to the dependency
        /// </summary>

        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dependency path has been resolved. 
        /// </summary>

        public bool Resolved { get; protected set; }

        /// <summary>
        /// A has that uniquely identifies the contents to prevent browser caching when they have changed
        /// </summary>

        public string ScriptHash { get; set; }

        /// <summary>
        /// When true, the script should not be combined but always loaded individually.
        /// </summary>

        public bool NoCombine { get; set; }

        /// <summary>
        /// Dependencies for this ScriptRef
        /// </summary>
        ///
        /// <value>
        /// The dependencies.
        /// </value>

        public List<ScriptRef> Dependencies { get; protected set; }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        ///
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>

        public override int GetHashCode()
        {
            return Path != null ? Path.GetHashCode() : 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        ///
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        ///
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>

        public override bool Equals(object obj)
        {
            ScriptRef other = obj as ScriptRef;
            return other != null &&
                Path == other.Path;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        ///
        /// <returns>
        /// A string that represents the current object.
        /// </returns>

        public override string ToString()
        {
            return "[" + Path + "]: \"" + Path + "\"";
        }

        /// <summary>
        /// Updates this script reference's details from another equivalent reference. (Will fail if they do not have the same name).
        /// </summary>
        ///
        /// <param name="other">
        /// The other script reference.
        /// </param>

        public void UpdateFrom(ScriptRef other)
        {
            if (other == null || other.Path != Path)
            {
                throw new InvalidOperationException("The other script reference must have the same name as this one.");
            }
            ScriptHash = other.ScriptHash;
            Resolved = other.Resolved;
            Dependencies = new List<ScriptRef>(other.Dependencies);
            Path = other.Path;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        ///
        /// <param name="x">
        /// The first instance to compare.
        /// </param>
        /// <param name="y">
        /// The second instance to compare.
        /// </param>
        ///
        /// <returns>
        /// true if the parameters are considered equivalent.
        /// </returns>

        public static bool operator ==(ScriptRef x, ScriptRef y)
        {
            return AreEqual(x, y);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        ///
        /// <param name="x">
        /// The first instance to compare.
        /// </param>
        /// <param name="y">
        /// The second instance to compare.
        /// </param>
        ///
        /// <returns>
        /// true if the parameters are not considered equivalent.
        /// </returns>

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
