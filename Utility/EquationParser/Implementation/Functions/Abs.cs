using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation.Functions
{

    public class Abs<T> : Equation<T>, IFunction<T> where T : IConvertible
    {
        public Abs()
        {

        }
        public override T Calculate()
        {
            if (Variables.Count != 1)
            {
                throw new Exception("The ABS function requires exactly one parameter");
            }

            if (Utils.IsIntegralType<T>())
            {
                return (T)Convert.ChangeType(Math.Abs(Convert.ToInt64(VariableValues["0"])), typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(Math.Abs(Convert.ToDecimal(VariableValues["0"])), typeof(T));
            }
        }
   
        public string Name
        {
            get { return "abs"; }
        }
    }
}
