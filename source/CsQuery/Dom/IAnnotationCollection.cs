using System.Collections.Generic;

namespace CsQuery
{
    /// <summary>
    /// Interface for methods to access the custom annotations on a DOM node.
    /// </summary>
    public interface IAnnotationCollection : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Get the value of a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        /// <returns>The annotation value</returns>
        object GetAnnotation(string name);

        /// <summary>
        /// Set the value of a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        /// <param name="value">The new annotation value</param>
        void SetAnnotation(string name, object value);

        /// <summary>
        /// Removes a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        void RemoveAnnotation(string name);

        /// <summary>
        /// Gets or sets the value of a named annotation
        /// </summary>
        /// <param name="name">The annotation name</param>
        /// <returns>The annotation value</returns>
        /// <returntype>object</returntype>
        object this[string name] { get; set; }

        /// <summary>
        /// The number of annotations in this annotation collection.
        /// </summary>
        /// <returntype>int</returntype>
        int Length { get; }
    }
}