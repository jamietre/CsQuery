using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Engine.FormulaParser
{
    public interface IFormula
    {
        string FormulaText { get; set; }
        void SetVariable(string name, object value);
        void SetVariable<T>(string name, T value);
        object Calculate();
        bool TryCalculate(out object result);
    }
    public interface IFormula<T>: IFormula
    {
        T Calulate();
        bool TryCalculate(out T result);
    }
}
