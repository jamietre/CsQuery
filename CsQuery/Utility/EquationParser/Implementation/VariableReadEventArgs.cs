using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.EquationParser.Implementation
{
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
                _Value = value;
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
}
