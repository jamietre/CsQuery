using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility.EquationParser.Implementation;

namespace Jtc.CsQuery.Utility.EquationParser
{
    //TODO - unimplemented
    //Add/subtract/multiply etc should just be functions with two arguments. Operator should just
    // return a function.
    public interface IFunction: IEquation
    {
        /// <summary>
        /// The name of this variable
        /// </summary>
        string Name { get; }

    }
    /// <summary>
    /// T is the output type of the function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFunction<T> : IEquation<T>, IFunction where T: IConvertible
    {
    }
   
}
