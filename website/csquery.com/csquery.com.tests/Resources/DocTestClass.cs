using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsQuerySiteTests
{
    public class DocTestClass
    {
        /// <summary>
        /// Constructor for DocTestClass.
        /// </summary>
        ///
        /// <param name="parm">
        /// The parameter.
        /// </param>

        public DocTestClass(string parm)
        {

        }

        /// <summary>
        /// This method has no return
        /// </summary>

        public void ReturnlessMethod()
        {

        }

        /// <summary>
        /// A static method.
        /// </summary>
        ///
        /// <returns>
        /// A string
        /// </returns>

        public static string StaticMethod()
        {
            return "A static method";
        }

        /// <summary>
        /// Gets or sets the int property.
        /// </summary>
        /// <remarks>
        /// Different visibility for getter and setter
        /// </remarks>

        public int IntProp { get; protected set; }

        protected DocTestClass ClassProp { private get; set; }

        internal string StringProp { get; set; }

        public T GenericMethod<T>(int parameter, T genericParameter)
        {
            return default(T);
        }

        public Action<T> ComplexGenericMethod<T>(int param1, params object[] parmArray)
        {
            return null;
        }

        public void OptionalArgs(double required, double? optional = 2)
        {

        }
        public Func<T, U>[] ReallyOdd<T, U>(short?[] nullableShortArray, U[] genericArray = null, params Func<U>[] optionals) where U: class, new()
        {
            return null;
        }
        public ulong ReallyOdd2<T>(T input, out ITestInterface output) where T: ITestInterface, new()
        {
            output = null;
            return 2;
        }

    }

    public interface ITestInterface 
    {
    }

}
