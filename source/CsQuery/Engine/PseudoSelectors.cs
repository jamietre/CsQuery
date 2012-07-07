using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CsQuery.HtmlParser;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;
using CsQuery.ExtensionMethods.Internal;

namespace CsQuery.Engine
{
    public class PseudoSelectors: IDictionary<string,Type>
    {

        #region contructors

        static PseudoSelectors()
        {
            Items = new PseudoSelectors();
            PopulateInnerSelectors();
        }

        /// <summary>
        /// Default constructor/.
        /// </summary>
        ///
        /// <exception cref="Exception">
        /// Throws an exception if an instance has already been assigned to the static Items property.
        /// This class should instantiate itself as a singleton.
        /// </exception>

        public PseudoSelectors()
        {
            if (Items!=null) {
                throw new Exception("You can only create one instance of the PseudoSelectors class.");
            }
            InnerSelectors = new Dictionary<string, Type>();
        }

        

        #endregion

        #region private properties

        private IDictionary<string, Type> InnerSelectors;

        #endregion


        #region public properties

        /// <summary>
        /// Static instance of the PseudoSelectors singleton.
        /// </summary>

        public static PseudoSelectors Items { get; protected set; }

        #endregion

        #region public methods

        /// <summary>
        /// Gets an instance of a named pseudoselector
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the pseudoselector does not exist
        /// </exception>
        ///
        /// <param name="name">
        /// The name of the pseudoselector
        /// </param>
        ///
        /// <returns>
        /// A new instance
        /// </returns>

        public IPseudoSelector GetInstance(string name) 
        {
            Type ps;
            if (InnerSelectors.TryGetValue(name, out ps))
            {
                return (IPseudoSelector)Activator.CreateInstance(ps);
            }
            else
            {
                throw new ArgumentException(String.Format("Attempt to use nonexistent pseudoselector :{0}", name));
            }
        }

        /// <summary>
        /// Try to gets an instance of a named pseudoselector.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the pseudoselector.
        /// </param>
        /// <param name="instance">
        /// [out] The new instance.
        /// </param>
        ///
        /// <returns>
        /// true if succesful, false if a pseudoselector of that name doesn't exist.
        /// </returns>

        public bool TryGetInstance(string name, out IPseudoSelector instance) {
            Type ps;
            if (InnerSelectors.TryGetValue(name, out ps))
            {
                instance = (IPseudoSelector)Activator.CreateInstance(ps);
                return true;
            }
            else
            {
                instance = null;
                return false;
            }
        }

        #endregion


        


        private void ValidateType(Type value)
        {
            if (value.GetInterface("IPseudoSelector")==null)
            {
                throw new ArgumentException("The type must implement IPseudoSelector.");
            }
        }

        #region IDictionary interface

        public void Add(string key, Type value)
        {
            ValidateType(value);
            InnerSelectors.Add(key,value);
        }

        public bool ContainsKey(string key)
        {
            return InnerSelectors.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return InnerSelectors.Keys; }
        }

        public bool Remove(string key)
        {
            return InnerSelectors.Remove(key);
        }

        public bool TryGetValue(string key, out Type value)
        {
            return InnerSelectors.TryGetValue(key, out value);
        }

        public ICollection<Type> Values
        {
            get {return InnerSelectors.Values; }
        }

        public Type this[string key]
        {
            get
            {
                return InnerSelectors[key];
            }
            set
            {
                ValidateType(value);
                InnerSelectors[key] = value;
                
            }
        }

        public void Add(KeyValuePair<string, Type> item)
        {
            ValidateType(item.Value);
            InnerSelectors.Add(item);

        }

        public void Clear()
        {
            InnerSelectors.Clear();
        }

        public bool Contains(KeyValuePair<string, Type> item)
        {
            return InnerSelectors.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Type>[] array, int arrayIndex)
        {
            InnerSelectors.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get {return InnerSelectors.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, Type> item)
        {
            return InnerSelectors.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
        {
            return InnerSelectors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region private methods

        private static void PopulateInnerSelectors()
        {
            string nameSpace = "CsQuery.Engine.PseudoClassSelectors";
            var assy = Assembly.GetExecutingAssembly();
            bool foundTypes = false;
            foreach (var t in assy.GetTypes())
            {
                if (t.IsClass && t.Namespace != null &&
                    !t.IsAbstract &&
                    t.Namespace.StartsWith(nameSpace))
                {
                    if (t.GetInterface("IPseudoSelector") != null)
                    {
                        foundTypes = true;
                        Items.Add(Objects.FromCamelCase(t.Name), t);
                    }

                }
            }
            if (!foundTypes)
            {
                throw new InvalidOperationException("Could not find any default PseudoClassSelectors. Did you change a namespace?");
            }
        }

        #endregion
    }
}
