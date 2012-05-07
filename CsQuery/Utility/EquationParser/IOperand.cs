using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser
{
    public interface IOperand : IConvertible, ICloneable
    {
        IConvertible Value { get; }
        bool IsInteger { get; }
        new IOperand Clone();
    }
    public interface IOperand<T> : IOperand where T : IConvertible
    {
        new T Value { get;}
        new IOperand<T> Clone();
        
    }
    

   
}
