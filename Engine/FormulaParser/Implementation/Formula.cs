using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Engine.FormulaParser.Implementation
{
    public class Formula<T>: IFormula<T>
    {
        protected string error;

        public string FormulaText
        {
            get;set;
        }
        public T Calculate()
        {
            T result;
            if (!TryCalculate(out result))
            {
                throw new Exception("Error during calculation: " + error);
            }
            return result;

        }
        //potected Stack<
        public bool TryCalculate(out T result)
        {
            
            Stack<int> values = new Stack<int>();

            int i = 0;
            int mode = 0;
            int curVal = 0;
            char oper = ' ';
            while (i < FormulaText.Length)
            {
                char c = FormulaText[i];
                switch (mode)
                {
                    case 0:
                        if (isDigit(c))
                        {
                            mode = 2;
                            curVal = 0;
                        }
                        else if ("+-/*".Contains(c))
                        {
                            if (values.Count == 0)
                            {
                                return false;
                            }
                            i++;
                            mode = 2;
                            oper = c;
                        }
                        else if (c == '(')
                        {
                            mode = 3;
                        }
                        else if (c == 'n')
                        {
                            values.Push(val);
                            if (oper == ' ')
                            {
                                oper = '*';
                            }
                            i++;
                            mode = 4;
                        }
                        else if (c == ' ')
                        {
                            i++;
                        }
                        else
                        {
                            return -1;
                        }
                        break;
                    case 2:
                        // get number
                        curVal = GetNumber(formula, i, out i);
                        values.Push(curVal);
                        mode = 4;
                        break;
                    case 3:
                        // inner parens
                        int endPos = Formula.IndexOf(')', i);
                        if (endPos >= 0)
                        {
                            curVal = Calculate(val, formula.SubstringBetween(i + 1, endPos));
                            values.Push(curVal);
                            mode = 4;
                            i = endPos + 1;
                        }
                        else
                        {
                            return -1;
                        }
                        break;
                    case 4:
                        if (values.Count == 2 && oper != ' ')
                        {
                            int v2 = values.Pop();
                            int v1 = values.Pop();
                            values.Push(DoMath(oper, v1, v2));
                            oper = ' ';
                        }
                        mode = 0;
                        break;
                }
            }
            switch (values.Count)
            {
                case 0:
                    return -1;
                case 1:
                    return values.Pop();
                case 2:
                    int v2 = values.Pop();
                    int v1 = values.Pop();
                    return DoMath(oper, v1, v2);
                default:
                    throw new Exception("Something went horribly wrong with nth child");
            }

        }

        /// <summary>
        /// Do basic math
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected int DoMath(char oper, int value1, int value2)
        {
            switch (oper)
            {
                case '+':
                    return value1 + value2;
                case '-':
                    return value1 - value2;
                case '*':
                    return value1 * value2;
                case '/':
                    return value1 / value2;
                default:
                    return 0;

            }
        }


        private static char[] plusMinus = new char[] { '+', '-' };

        /// <summary>
        /// Wrap inner associative stuff in parenthesis
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        protected string Parenthesize(string formula)
        {
            int i = 0;
            int lastPos = 0;
            string output = "";
            while (i < formula.Length)
            {
                if ("+-".Contains(formula[i]))
                {
                    i++;
                    int endPos = formula.IndexOfAny(plusMinus, i);
                    if (endPos < 0)
                    {
                        endPos = formula.Length;
                    }

                    output += "(" + formula.SubstringBetween(lastPos, endPos) + ")";
                    i = endPos;
                    lastPos = i + 1;
                }
                else
                {
                    output += formula[i];
                    lastPos++;
                }

                i++;
            }
            if (lastPos < formula.Length)
            {
                output += formula.Substring(lastPos);
            }
            return output;
        }
        /// <summary>
        /// Parse a number starting at index. Return the number, and send back the new position.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="index"></param>
        /// <param name="newIndex"></param>
        /// <returns></returns>
        protected int GetNumber(string formula, int index, out int newIndex)
        {
            string number = "";
            while (index < formula.Length && isDigit(formula[index]))
            {
                number += formula[index++];
            }
            int result;
            newIndex = index;
            if (int.TryParse(number, out result))
            {
                return result;
            }
            else
            {
                return -1;
            }
        }
        protected bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        public T Calulate()
        {
            throw new NotImplementedException();
        }


        public void SetVariable(string name, object value)
        {
            throw new NotImplementedException();
        }

        public void SetVariable<T>(string name, T value)
        {
            throw new NotImplementedException();
        }

        object IFormula.Calculate()
        {
            throw new NotImplementedException();
        }

        bool IFormula.TryCalculate(out object result)
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
    }
    
}
