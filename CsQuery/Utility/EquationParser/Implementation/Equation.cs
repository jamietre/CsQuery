using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using CsQuery.ExtensionMethods;
using CsQuery;
using CsQuery.Utility.EquationParser;
using CsQuery.Utility.StringScanner;

namespace CsQuery.Utility.EquationParser.Implementation
{
    public class Equation : Operand,IEquation
    {
        #region constructors

        public Equation()
        {
            Initialize();
        }
        public Equation(IOperand operand)
        {
            Initialize();
            Operand = operand;
        }
        protected virtual void Initialize()
        {
            _VariableValues = new OrderedDictionary<string, IConvertible>();
        }
        #endregion

        #region private properties
        private IOperand _Operand;
        private OrderedDictionary<string, IConvertible> _VariableValues;
        //xref of names in the order they appear
        
        /// <summary>
        /// The names of the variables in the order added. For functions (where the parameters are passed only by order)
        /// this is important. Probably could move this to the Function implementation
        /// but it requires overriding everything, almost easier to keep it here.
        /// </summary>

        #endregion
        
        #region public properties

        public IOrderedDictionary<string, IConvertible> VariableValues
        {
            get
            {
                return _VariableValues;
            }
        }
        /// <summary>
        /// The root operand for the equation. The equation must not be changed once set, or variables will not be bound.
        /// </summary>
        public IOperand Operand
        {
            get
            {
                return _Operand;
            }
            set
            {
                _Operand = value;
                VariableValues.Clear();
                if (value != null && value is IVariableContainer)
                {
                    foreach (IVariable variable in ((IVariableContainer)value).Variables)
                    {
                        AddVariable(variable);
                    }
                }
            }
        }
        public IEnumerable<IVariable> Variables
        {
            get
            {
                if (Operand is IVariableContainer)
                {
                    return ((IVariableContainer)Operand).Variables;
                }
                else
                {
                    return Objects.EmptyEnumerable<IVariable>();
                }
            }
        }
        /// <summary>
        /// The values set (on order that each variable appears first in the equation) for each varaiable
        /// </summary>
        
        #endregion

        #region public methods

        public new IEquation Clone()
        {
            return (IEquation)CopyTo(GetNewInstance());
        }

        public void Compile()
        {
            if (Operand is IFunction)
            {
                ((IFunction)Operand).Compile();
            }
        }


        public bool TryGetValue(out IConvertible result)
        {
            try
            {
                result = Value;
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public virtual bool TryGetValue(out IConvertible result, params IConvertible[] values)
        {
            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    SetVariable(i, values[i]);
                }
                result = Value;
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        /// <summary>
        /// Sets the value used for a variable when the function is next run
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public virtual void SetVariable(string name, IConvertible value)
        {
            // Setting a variable doesn't do anything directly, instead, it stores the value for use when it's accessed by the equation.
            // Each entity that makes up an equation has its own variable list - the objects used for "x" each time it appears are not the
            // same instance. This makes construction easier (otherwise, there would have to be an "owner" for each operand so they could
            // access an existing instance of same-named variable). So we get variable values from an event, rather than assigning them
            // to the objects. 
            //Variables.Where(item=> item.Name==name).Do(item=> {
            //    item.Clear();
            //});

            VariableValues[name] = value;
        }
        public virtual void SetVariable(int index, IConvertible value)
        {
            if (VariableValues.Count == index)
            {
                SetVariable(VariableValues.Count.ToString(), value);
            }
            else
            {
                SetVariable(_VariableValues.Keys[index], value);
            }

        }
        public virtual void SetVariable<U>(string name, U value) where U : IConvertible
        {
            SetVariable(name, (IConvertible)value);
        }

        protected override IConvertible GetValue()
        {
            return Operand.Value;
        }

        /// <summary>
        /// Set the paramenters in order to the values passed, and returns the result of the equation
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IConvertible GetValue(params IConvertible[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                SetVariable(i, values[i]);
            }
            return Value;
        }

        public override string ToString()
        {
            return Operand.ToString();
        }
        #endregion

        #region protected methods
        protected override IOperand GetNewInstance()
        {
            return new Equation();
        }
        protected override IOperand CopyTo(IOperand operand)
        {
            IEquation target = (IEquation)operand;
            target.Operand = Operand.Clone();

            return operand;
        }

        protected void Variable_OnGetValue(object sender, VariableReadEventArgs e)
        {
            IConvertible value;
            if (VariableValues.TryGetValue(e.Name, out value)) {
                e.Value = VariableValues[e.Name];
            } else {
                throw new InvalidOperationException("The value for variable '" + e.Name + "' was not set.");
            }

        }
        protected void AddVariable(IVariable variable) {
            if (!VariableValues.ContainsKey(variable.Name))
            {
                _VariableValues[variable.Name] = null;
            }
            variable.OnGetValue += new EventHandler<VariableReadEventArgs>(Variable_OnGetValue);
        }

        #endregion

        #region interface members
        IEnumerable<IVariable> IVariableContainer.Variables
        {
            get
            {
                return Variables;
            }
        }
        #endregion
    }

    public class Equation<T> : Equation, IEquation<T> where T : IConvertible
    {
        public Equation()
        {

        }
        public Equation(IConvertible operand)
        {
            Operand = Utils.EnsureOperand(operand);
        }
        #region public methods

        public new IEquation<T> Clone()
        {
            return (IEquation<T>)CopyTo(GetNewInstance());
        }
        public IEquation<U> CloneAs<U>() where U : IConvertible
        {
            return (IEquation<U>)CloneAsImpl<U>();
        }


        public new T GetValue(params IConvertible[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                SetVariable(i, values[i]);
            }
            return Value;
        }
        public bool TryGetValue(out T result)
        {

            IConvertible untypedResult;
            if (base.TryGetValue(out untypedResult))
            {
                result = (T)Convert.ChangeType(untypedResult, typeof(T));
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }

        }

        public virtual bool TryGetValue(out T result, params IConvertible[] values)
        {
            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    SetVariable(i, values[i]);
                }
                result = (T)Convert.ChangeType(Value, typeof(T));
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }


        public new T Value
        {
            get
            {
                return (T)Convert.ChangeType(Operand.Value, typeof(T));
            }
        }

        public override string ToString()
        {
            return Operand==null?"":Operand.ToString();
        }
        #endregion
        #region private methods
        protected IOperand<U> CloneAsImpl<U>() where U : IConvertible
        {
            Equation<U> clone = new Equation<U>();
            CopyTo(clone);
            return clone;
        }

        protected override IOperand GetNewInstance()
        {
            return new Equation<T>();
        }
        protected override IOperand CopyTo(IOperand operand)
        {
            CopyTo((IEquation)operand);
            return operand;
        }
        #endregion

        #region Interface members
        IConvertible IEquation.GetValue(params IConvertible[] values)
        {
            return GetValue(values);
        }
        bool IEquation.TryGetValue(out IConvertible value)
        {
            T typedValue;
            if (TryGetValue(out typedValue)) {
                value = typedValue;
                return true;
            } else {
                value = default(T);
                return false;
            }
        }
        bool IEquation.TryGetValue(out IConvertible value, params IConvertible[] variableValues)
        {
            T typedValue ;
            if (TryGetValue(out typedValue, variableValues))
            {
                value = typedValue;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        IOperand<T> IOperand<T>.Clone()
        {
            return Clone();
        }
        IEquation IEquation.Clone()
        {
            return Clone();
        }
        #endregion
    }
}
