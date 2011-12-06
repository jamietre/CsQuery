using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser
{
    public interface IOperand: IConvertible
    {
        IConvertible Value { get; }
        //bool IsNumber { get; }
        //bool IsFloatingPoint { get; }
        bool IsInteger { get; }
        //bool IsBoolean { get; }
        //bool IsText { get; }
    }
    public interface IOperand<T>: IOperand where T: IConvertible
    {
        new T Value { get;}
    }
    

   
}
