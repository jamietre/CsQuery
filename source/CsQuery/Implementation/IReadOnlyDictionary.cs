using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{
    /// <summary>
    /// Interface for read only collection.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// Generic type parameter.
    /// </typeparam>


    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        // Summary:
        //     Gets an System.Collections.Generic.ICollection<T> containing the keys of
        //     the System.Collections.Generic.IDictionary<TKey,TValue>.
        //
        // Returns:
        //     An System.Collections.Generic.ICollection<T> containing the keys of the object
        //     that implements System.Collections.Generic.IDictionary<TKey,TValue>.
        
        ICollection<TKey> Keys { get; }
        
        //
        // Summary:
        //     Gets an System.Collections.Generic.ICollection<T> containing the values in
        //     the System.Collections.Generic.IDictionary<TKey,TValue>.
        //
        // Returns:
        //     An System.Collections.Generic.ICollection<T> containing the values in the
        //     object that implements System.Collections.Generic.IDictionary<TKey,TValue>.
        
        ICollection<TValue> Values { get; }

        // Summary:
        //     Gets or sets the element with the specified key.
        //
        // Parameters:
        //   key:
        //     The key of the element to get or set.
        //
        // Returns:
        //     The element with the specified key.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     key is null.
        //
        //   System.Collections.Generic.KeyNotFoundException:
        //     The property is retrieved and key is not found.
        //
        //   System.NotSupportedException:
        //     The property is set and the System.Collections.Generic.IDictionary<TKey,TValue>
        //     is read-only.
        
        TValue this[TKey key] { get; }

        
        //
        // Summary:
        //     Determines whether the System.Collections.Generic.IDictionary<TKey,TValue>
        //     contains an element with the specified key.
        //
        // Parameters:
        //   key:
        //     The key to locate in the System.Collections.Generic.IDictionary<TKey,TValue>.
        //
        // Returns:
        //     true if the System.Collections.Generic.IDictionary<TKey,TValue> contains
        //     an element with the key; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     key is null.
        
        bool ContainsKey(TKey key);
        
        //
        // Summary:
        //     Gets the value associated with the specified key.
        //
        // Parameters:
        //   key:
        //     The key whose value to get.
        //
        //   value:
        //     When this method returns, the value associated with the specified key, if
        //     the key is found; otherwise, the default value for the type of the value
        //     parameter. This parameter is passed uninitialized.
        //
        // Returns:
        //     true if the object that implements System.Collections.Generic.IDictionary<TKey,TValue>
        //     contains an element with the specified key; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     key is null.
        
        bool TryGetValue(TKey key, out TValue value);
    }
}
