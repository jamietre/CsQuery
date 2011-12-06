using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jtc.CsQuery.Utility.StringScanner;

namespace Jtc.CsQuery.Utility.EquationParser.Implementation
{
    
    public class EquationParser<T>: IEquationParser<T> where T: IConvertible
    {
        public EquationParser()
        {

        }

        public string Error { get; set; }

        public IEnumerable<IVariable> Variables
        {
            get
            {
                if (_Variables == null)
                {
                    return Objects.EmptyEnumerable<IVariable>();
                }
                else
                {
                    return _Variables;
                }
            }
        }
        protected List<IVariable> InnerVariables
        {
            get
            {
                if (_Variables == null)
                {
                    _Variables = new List<IVariable>();
                }
                return _Variables;
            }
        }
        protected List<IVariable> _Variables;
        protected string _Text;
        public IOperand<T> Equation
        {
            get;
            protected set;
        }
        IOperand IEquationParser.Equation
        {
            get
            {
                return Equation;
            }
        }

        // used by parser
        
        protected int CurPos;
        protected bool ParseEnd;
        protected IStringScanner scanner;

        public bool TryParse(string text)
        {
            try
            {
                Parse(text);
                return true;
            }
            catch (Exception e)
            {
                Error = e.Message;
                Equation = null;
                return false;
            }
        }
        public void Parse(string text)
        {

            scanner = Scanner.Create(text);

            IOperand opA;
            IClause<T> clause = new Clause<T>();
            Equation = clause;

            // it could have just one operand

            opA = GetOperand();
            clause.OperandA = opA;

            while (!ParseEnd)
            {
                IOperand opB;
                IOperator oper;
                oper = GetOperator();
                opB = GetOperand();

                clause = clause.Chain(opB, oper);
            }
            Error = "";
        }

        protected IOperand GetOperand()
        {
            string text;
            IOperand output;
            scanner.SkipWhitespace();
            if (scanner.Info.NumericExtended)
            {
                text = scanner.GetNumber();
                int num;
                if (Int32.TryParse(text, out num))
                {
                    output = Literal.Create(num);
                }
                else
                {
                    throw new Exception("Unable to parse number from '" + text + "'");
                }
            }
            else if (scanner.Info.Alpha)
            {
                text = scanner.GetAlpha();
                if (scanner.NextCharOrEmpty == "(")
                {
                    output = Utils.GetFunction<T>(text);
                    // TODO -- parse inner () into a parameter list
                    // Create operands for each (any could be a variable)

                    // it is a function = create the operand from inner parens
                }
                else
                {
                    IVariable var = GetVariable(text);
                    output = var;
                }
            }
            else if (scanner.NextChar == '(')
            {
                string inner = scanner.Get(MatchFunctions.BoundedBy("("));
                var parser = new EquationParser<T>();
                parser.Parse(inner);
                
                output = parser.Equation;
                foreach (var newVar in parser.Variables)
                {
                    if (!Variables.Any(item => item.Name == newVar.Name))
                    {
                        InnerVariables.Add(newVar);
                    }
                }
            }
            else
            {
                throw new Exception("Unexpected character '" + scanner.Match + "' found, expected an operand (a number or variable name)");
            }
            scanner.SkipWhitespace();
            ParseEnd = scanner.Finished;

            return output;

        }

        protected IOperator GetOperator()
        {
            IOperator output;
            if (scanner.Info.Alpha || scanner.NextChar=='(')
            {
                output = new Operator("*");
            }
            else
            {
                output = new Operator(scanner.Get(MatchFunctions.Operator));
            }
            return output;
        }
        protected IVariable GetVariable(string name)
        {
            IVariable output;
            output = Variables.FirstOrDefault(item=>item.Name==name);
            if (output==null)
            {
                var variable = new Variable<T>(name);

                if (!Variables.Any(item=>item.Name==name)) {
                    InnerVariables.Add(variable);
                }
                //variable.OnGetValue += new EventHandler<VariableReadEventArgs<T>>(Variable_OnGetValue);
                //variables.Add(name, variable);
                //variableOrder.Add(name);
                output = variable;
            }

            return output;
        }
    }
}
