using System;
using System.Collections;
using System.Collections.Generic;

namespace CsQuery.Implementation
{
    /// <summary>
    /// A collection of annotations
    /// </summary>
    public class AnnotationCollection : IAnnotationCollection
    {
        private readonly Dictionary<string, object> _annotations 
            = new Dictionary<string, object>();

        /// <summary>
        /// Gets the enumerator of the annotation collection
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _annotations.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of the annotation collection
        /// </summary>
        /// <returns>An enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Get the value of a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        /// <returns>The annotation value</returns>
        public object GetAnnotation(string name)
        {
            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("You must provide an annotation name.", "name");

            object result;
            _annotations.TryGetValue(name, out result);

            return result;
        }

        /// <summary>
        /// Set the value of a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        /// <param name="value">The new annotation value</param>
        public void SetAnnotation(string name, object value)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("You must provide an annotation name.", "name");

            _annotations[name] = value;
        }

        /// <summary>
        /// Removes a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        public void RemoveAnnotation(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("You must provide an annotation name.", "name");

            _annotations.Remove(name);
        }

        /// <summary>
        /// Gets or sets the value of a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        /// <returns>The annotation value</returns>
        /// <returntype>object</returntype>
        public object this[string name]
        {
            get { return GetAnnotation(name); }
            set { SetAnnotation(name, value); }
        }

        /// <summary>
        /// The number of annotations in this annotation collection.
        /// </summary>
        /// <returntype>int</returntype>
        public int Length { get { return _annotations.Count; } }
    }
}
