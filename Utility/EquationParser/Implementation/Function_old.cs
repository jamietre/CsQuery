using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility.StringScanner;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    //public abstract class Function_old<T> : Equation<T>, IFunction<T> where T : IConvertible, IEquatable<T>
    //{
    //    #region constructors
    //    public Function(string name)
    //    {
    //        Name = name;

    //    }
    //    public Function(string name, IEnumerable<IOperand> operands)
    //    {
    //        Name = name;
    //        _Operands.AddRange(operands);

    //    }
    //    protected void Initialize()
    //    {
    //        RequiredParmCount = -1;
    //        MaxParmCount = -1;
    //    }
    //    #endregion

    //    #region public properties
    //    public int RequiredParmCount
    //    {
    //        get;
    //        protected set;
    //    }
    //    public int MaxParmCount
    //    {
    //        get;
    //        protected set;
    //    }


    //    public string Name { get; set; }
      
    //    public override T Value
    //    {
    //        get
    //        {
    //            return Calculate();
    //        }
    //        protected set
    //        {
    //            throw new Exception("You cannot set the value for a function, it is calculated. Try setting some inputs instead.");
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return Name + "(" + base.ToString() + ")";
    //    }
    //    #endregion

    //    #region private members
    //    protected abstract T Calculate();
        

    //    protected override IOperand<U> CloneAsImpl<U>()
    //    {
    //        return base.CloneAsImpl<U>();
    //    }

    //    protected string OperandExceptionMessage
    //    {
    //        get
    //        {
    //            string error = "";
                
    //            if (RequiredParmCount >= 0)
    //            {
    //                error += "The '" + Name + "' function requires at least " + RequiredParmCount;
    //            }
    //            if (MaxParmCount >= 0)
    //            {
    //                error += (error == "" ? "The '" + Name + "' function accepts no more than " : " and no more than") + MaxParmCount;
    //            }
    //            error += " parameters.";
    //            return error;
    //        }
    //    }
    //    #endregion
    //}

    
}
