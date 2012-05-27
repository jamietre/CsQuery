using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation
{
    //public class Operator: IOperator
    //{
    //    #region constructors
    //    public Operator()
    //    {

    //    }
    //    public Operator(string op)
    //    {
    //        Set(op);
    //    }
    //    public Operator(Operation op)
    //    {
    //        _Operation = op;
    //    }
    //    public static implicit operator Operator(string op)
    //    {
    //        return new Operator(op);
    //    }
    //    #endregion

    //    #region private members
    //    // The order must match the enum

    //    protected static List<string> _Operators = new List<string>(new string[] 
    //        { "+", "-", "*", "/", "%", "^" }
    //    );
    //    public static IEnumerable<string> Operators
    //    {
    //        get
    //        {
    //            return _Operators;
    //        }
    //    }
    //    protected static HashSet<string> ValidOperators = new HashSet<string>(Operators);

    //    protected Operation _Operation;
    //    #endregion
        
    //    #region public properties
    //    public bool IsAssociative { get; protected set; }

    //    public Operation Operation
    //    {
    //        get { return _Operation; }
    //    }
    //    #endregion
        
    //    #region public methods
    //    public void Set(string op)
    //    {
    //        if (!TrySet(op))
    //        {
    //            throw new InvalidOperationException("'" + op + "' is not a valid operator.");
    //        }
    //    }

    //    public bool TrySet(string value)
    //    {
    //        if (!ValidOperators.Contains(value))
    //        {
    //            return false;
    //        }
    //        switch (value)
    //        {
    //            case "+": 
    //                _Operation = Operation.Addition;
    //                IsAssociative = false;
    //                break;
    //            case "-":
    //                _Operation = Operation.Subtraction;
    //                IsAssociative = false;
    //                break;
    //            case "*":
    //                _Operation = Operation.Multiplication;
    //                IsAssociative = true;
    //                break;
    //            case "/":
    //                _Operation = Operation.Division;
    //                IsAssociative = true;
    //                break;
    //            case "^":
    //                _Operation = Operation.Power;
    //                IsAssociative = true;
    //                break;
    //            case "%":
    //                _Operation = Operation.Modulus;
    //                IsAssociative = true;
    //                break;

    //        }
    //        return true;
    //    }

    //    public IOperator Clone()
    //    {
    //        return new Operator(Operation);
    //    }
    //    public override string ToString()
    //    {
    //        return _Operators[((int)Operation) - 1];
    //    }
    //    #endregion

    //    #region interface members

    //    object ICloneable.Clone()
    //    {
    //        return Clone();
    //    }
    //    #endregion
    //}
}
