using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    //public abstract class Variable : Operand, IVariable
    //{
    //    public abstract string Name { get; set; }
    //    public abstract IConvertible Value { get; }
    //    public abstract void Clear();
    //    public event EventHandler<VariableReadEventArgs> OnGetValue;
    //}

    public class VariableReadEventArgs : EventArgs
    {
        public VariableReadEventArgs(string name)
        {
            Name = name;
        }
        public IConvertible Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = (IConvertible)Convert.ChangeType(value, Type);
            }
        }
        protected IConvertible _Value;
        
        public Type Type
        {
            get;
            set;
        }
        public string Name
        {
            get;
            protected set;
        }
    }
    public class Variable<T> : Operand<T>, IVariable<T> where T: IConvertible 
    {
        public Variable()
        {

        }
        public Variable(string name)
        {
            Name=name;
        }
        protected bool isGotValue=false;

        public event EventHandler<VariableReadEventArgs> OnGetValue;

        public string Name { get; set; }
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }
        public override T Value
        {
            get
            {
                if (!isGotValue)
                {
                    if (OnGetValue == null)
                    {
                        throw new Exception("This variable is not bound to a formula, so it's value cannot be read.");
                    }
                    VariableReadEventArgs args = new VariableReadEventArgs(Name);
                    args.Type = typeof(T);
                    OnGetValue(this, args);
                    base.Value = (T)args.Value;
                    isGotValue = true;
                }
                return base.Value;
            }
        }
        public void Clear()
        {
            isGotValue = false;
        }

        public override string ToString()
        {
            return Name;
        }

        IConvertible IOperand.Value
        {
            get { return Value; }
        }

    }
}
