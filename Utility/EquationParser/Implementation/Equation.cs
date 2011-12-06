using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.ExtensionMethods;
using Jtc.CsQuery;
using Jtc.CsQuery.Utility.EquationParser;
using Jtc.CsQuery.Utility.StringScanner;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    public class Equation<T> : Operand<T>,IEquation<T> where T : IConvertible
    {
        #region protected properties
        
        protected IDictionary<string, IVariable> Variables
        {
            get
            {
                if (_Variables == null)
                {
                    _Variables = new SmallDictionary<string, IVariable>();
                }
                return _Variables;
            }
        }
        private IDictionary<string, IVariable> _Variables;
        protected IDictionary<string, IConvertible> VariableValues
        {
            get
            {
                if (_VariableValues == null)
                {
                    _VariableValues = new SmallDictionary<string, IConvertible>();
                }
                return _VariableValues;
            }
        }
        private  IDictionary<string, IConvertible> _VariableValues;
        protected List<string> VariableOrder
        {
            get
            {
                if (_VariableOrder == null)
                {
                    _VariableOrder = new List<string>();
                }
                return _VariableOrder;
            }
        }
        private List<string> _VariableOrder;
        public bool TryParse(string text)
        {
            try {
                Parse(text);
                return true;
            }
            catch {
                return false;
            }
        }
        public void Parse(string text)
        {
            IEquationParser<T> parser = new EquationParser<T>();
            parser.Parse(text);
            _Clause = parser.Equation;
            foreach (var item in parser.Variables)
            {
                VariableOrder.Add(item.Name);
                Variables.Add(item.Name, item);
                item.OnGetValue += new EventHandler<VariableReadEventArgs>(Variable_OnGetValue);
            }

        }
        
        protected IOperand<T> _Clause;
        protected string _Text;
        #endregion
        #region public properties

        #endregion

        #region public methods
        public virtual T Calculate()
        {
            return Value;
        }
        public virtual T Calculate(params IConvertible[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                SetVariable(VariableOrder[i], values[i]);
            }
            return Calculate();
        }
        public virtual bool TryCalculate(out T result)
        {
            try {
                result = Value;
                return true;
            }
            catch {
                result = default(T);
                return false;
            }
        }


        public virtual void SetVariable(string name, IConvertible value)
        {
            VariableValues[name] = value;
            IVariable variable;
            if (Variables.TryGetValue(name, out variable))
            {
                variable.Clear();
            }

        }

        public virtual void SetVariable<U>(string name, U value) where U : IConvertible
        {
            SetVariable(name,(IConvertible)value);
        }

        #endregion

        #region protected methods


        
        protected void Variable_OnGetValue(object sender, VariableReadEventArgs e)
        {
            IConvertible value;
            if (VariableValues.TryGetValue(e.Name, out value))
            {
                e.Value = value;
            } else {
                throw new Exception("The value for variable '" + e.Name +"' was not set.");
            }

        }
        #endregion
        #region Interface members
        public override T Value
        {
            get
            {
                return _Clause.Value;
            }
            protected set
            {
                throw new Exception("You cannot set the value for this kind of operand.");
            }
        }
        IConvertible IEquation.Calculate()
        {
            return (T)Calculate();
        }
        IConvertible IEquation.Calculate(params IConvertible[] values)
        {
            return (T)Calculate(values);
        }
        bool IEquation.TryCalculate(out object result)
        {
            T tempResult;
            if (TryCalculate(out tempResult)) {
                result = tempResult;
                return true;
            }
            else 
            {
                result = null;
                return false;
            }
        }
        public override string ToString()
        {
            return Value.ToString();
            
        }
        #endregion


    }
    
}
