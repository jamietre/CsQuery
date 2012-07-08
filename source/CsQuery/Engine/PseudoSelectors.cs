using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Reflection;
using CsQuery.HtmlParser;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.Engine
{
    public class PseudoSelectors
    {

        #region contructors

        static PseudoSelectors()
        {
            Items = new PseudoSelectors();
            
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
            InnerSelectors = new ConcurrentDictionary<string, Type>();
            PopulateInnerSelectors();
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
            IPseudoSelector ps;
            if (TryGetInstance(name, out ps))
            {
                return ps;
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
            Type type;
            if (InnerSelectors.TryGetValue(name, out type))
            {
                instance = (IPseudoSelector)FastActivator.CreateInstance(type);
                return true;
            }
            instance = null;
            return false;
        }

        #endregion


        


        private void ValidateType(Type value)
        {
            if (value.GetInterface("IPseudoSelector")==null)
            {
                throw new ArgumentException("The type must implement IPseudoSelector.");
            }
        }

       

        #region private methods

        private void PopulateInnerSelectors()
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
                        InnerSelectors.Add(Objects.FromCamelCase(t.Name),t);
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
