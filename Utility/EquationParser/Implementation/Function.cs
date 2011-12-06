//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Jtc.CsQuery.Engine.EquationParser.Implementation
//{
//    public abstract class Function: Operand, IFunction
//    {
//        public Function()
//        {

//        }
//        public Function(string name)
//        {
//            Name = name;
//        }
        
//        public abstract string Name { get; protected set; }
//        public abstract IConvertible Value { get; }
//    }

//    public abstract class Function<T> : Operand<T>, IFunction<T> where T: IConvertible 
//    {
//        public Function()
//            : base()
//        { }
//        public Function(string name): base()
//        {
//            Name = name;
//        }
//        public string Name { get; protected set; }
//        protected List<IOperand> parameters
//        {
//            get
//            {
//                if (_parameters == null)
//                {
//                    _parameters = new List<IConvertible>();
//                }
//                return _parameters;
//            }
//        }
//        private List<IOperand> _parameters;
//        public void AddParameter(IOperand value)
//        {
//            parameters.Add(value);
//        }
//        public override T Value
//        {
//            get
//            {

//                if (parameters.Count < RequiredParameters)
//                {
//                    throw new Exception(ParameterCountError);
//                }
//               return GetValue(parameters.ToArray<T>());
//            }
//            protected set
//            {
//                base.Value = value;
//            }
//        }
        
//        protected string ParameterCountError
//        {
//            get
//            {
//                string result = "The function '" + Name + "' requires ";
//                if (MaximumParameters == RequiredParameters)
//                {
//                    result += "exactly " + RequiredParameters;
//                }
//                else
//                {
//                    if (RequiredParameters > 0)
//                    {
//                        result += "at least " + RequiredParameters;
//                        if (MaximumParameters > 0)
//                        {
//                            result += " and";
//                        }
//                    }
//                    if (MaximumParameters > 0)
//                    {
//                        result += " no more than " + MaximumParameters;
//                    }
//                }
//                return result;
//            }
//        }
//        /// <summary>
//        /// The number of required parameters
//        /// </summary>
//        protected virtual int RequiredParameters
//        {
//            get
//            {
//                return 1;
//            }
//        }
//        /// <summary>
//        /// The maximum number of parameters (==RequiredParameters by default).
//        /// If there is no limit, this should be -1.
//        /// </summary>
//        protected virtual int MaximumParameters
//        {
//            get
//            {
//                return RequiredParameters;
//            }
//        }

//        /// <summary>
//        /// Implementations must override this method to return the function result. Invalid 
//        /// parameters should throw an error
//        /// </summary>
//        /// <param name="parameters"></param>
//        /// <returns></returns>
//        protected abstract T GetValue(IOperand[] parameters);
//    }
//}
