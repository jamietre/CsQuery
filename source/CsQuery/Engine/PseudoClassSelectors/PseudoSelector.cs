using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsQuery.StringScanner;
using CsQuery.StringScanner.Patterns;

namespace CsQuery.Engine.PseudoClassSelectors
{
    /// <summary>
    /// Base class for any pseudoselector.
    /// </summary>

    public abstract class PseudoSelector: IPseudoSelector
    {

        private string _Arguments;
        /// <summary>
        /// Gets or sets criteria (or parameter) data passed with the pseudoselector
        /// </summary>

        protected virtual string[] Parameters {get;set;}

        public virtual string Arguments
        {
            get
            {
                return _Arguments;
            }
            set
            {

                string[] parms=null;
                if (!String.IsNullOrEmpty(value))
                {
                    if (MaximumParameterCount > 1 || MaximumParameterCount < 0)
                    {
                        parms = ParseArgs(value);
                    }
                    else
                    {
                        parms = new string[] { ParseSingleArg(value) };
                    }

                    
                }
                ValidateParameters(parms);
                _Arguments = value;
                Parameters = parms;
                
            }
        }

        /// <summary>
        /// Parse the arguments using the rules returned by the ParameterQuoted method.
        /// </summary>
        ///
        /// <param name="value">
        /// The arguments
        /// </param>
        ///
        /// <returns>
        /// An array of strings
        /// </returns>

        protected string[] ParseArgs(string value)
        {
            List<string> parms = new List<string>();
            int index = 0;


            IStringScanner scanner = Scanner.Create(value);
           
            while (!scanner.Finished)
            {
                if (ParameterQuoted(index) == null)
                {
                    scanner.Expect(MatchFunctions.OptionallyQuoted(","));
                }
                else if (ParameterQuoted(index) == true)
                {
                    scanner.Expect(MatchFunctions.Quoted());
                }
                else
                {
                    scanner.Seek(',', true);
                    scanner.Next();
                }
                parms.Add(scanner.Match);
                index++;
            }
            return parms.ToArray();
        }

        protected string ParseSingleArg(string value)
        {
            IStringScanner scanner = Scanner.Create(value);
                   
            if (ParameterQuoted(1) == null)
            {
                scanner.Expect(MatchFunctions.OptionallyQuoted());
                if (!scanner.Finished)
                {
                    throw new ArgumentException(InvalidArgumentsError());
                }
                return scanner.Match;
            }
            else if (ParameterQuoted(1) == true)
            {
                scanner.Expect(MatchFunctions.Quoted());
                if (!scanner.Finished)
                {
                    throw new ArgumentException(InvalidArgumentsError());
                }
                return scanner.Match;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Validates a parameter array against the expected number of parameters.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when the wrong number of parameters is passed.
        /// </exception>
        ///
        /// <param name="parameters">
        /// Criteria (or parameter) data passed with the pseudoselector.
        /// </param>

        protected virtual void ValidateParameters(string[] parameters) {

            if (parameters == null)
            {
                 if (MinimumParameterCount != 0) {
                     throw new ArgumentException(ParameterCountMismatchError());
                 } else {
                     return;
                 }
            }

            if ((parameters.Length < MinimumParameterCount ||
                    (MaximumParameterCount >= 0 &&
                        (parameters.Length > MaximumParameterCount))))
            {
                throw new ArgumentException(ParameterCountMismatchError());
            }


        }
        /// <summary>
        /// The minimum number of parameters that this selector requires. If there are no parameters, return 0
        /// </summary>
        ///
        /// <value>
        /// An integer
        /// </value>

        public virtual int MinimumParameterCount { get { return 0; } }

        /// <summary>
        /// The maximum number of parameters that this selector can accept. If there is no limit, return -1.
        /// </summary>
        ///
        /// <value>
        /// An integer
        /// </value>

        public virtual int MaximumParameterCount { get { return 0; } }

        /// <summary>
        /// Return the properly cased name of this selector (the class name in non-camelcase)
        /// </summary>
        
        public virtual string Name
        {
            get
            {
                return Objects.FromCamelCase(this.GetType().Name);

            }
        }

        /// <summary>
        /// A value to determine how to parse the string for a parameter at a specific index.
        /// </summary>
        ///
        /// <param name="index">
        /// Zero-based index of the parameter.
        /// </param>
        ///
        /// <returns>
        /// null to accept a string that can (but does not have to be) quoted, true to require a quoted
        /// parameter, false to only accept an unqouted parameter.
        /// </returns>

        protected virtual bool? ParameterQuoted(int index)
        {
            return false;
        }

        /// <summary>
        /// Gets a string describing a parameter count mismatch
        /// </summary>
        ///
        /// <returns>
        /// A string to throw as an error
        /// </returns>

        protected string ParameterCountMismatchError()
        {
            if (MinimumParameterCount == MaximumParameterCount )
            {
                if (MinimumParameterCount == 0)
                {
                    return String.Format("The :{0} pseudoselector cannot have arguments.",
                        Name);
                }
                else
                {
                    return String.Format("The :{0} pseudoselector must have exactly {1} arguments.",
                     Name,
                     MinimumParameterCount);
                }
            } else if (MaximumParameterCount >= 0)
            {
                return String.Format("The :{0} pseudoselector must have between {1} and {2} arguments.",
                    Name,
                    MinimumParameterCount,
                    MaximumParameterCount);
            }
            else
            {
                return String.Format("The :{0} pseudoselector must have between {1} and {2} arguments.",
                     Name,
                     MinimumParameterCount,
                     MaximumParameterCount);
            }
        }

        protected string InvalidArgumentsError()
        {
            return String.Format("The :{0} pseudoselector has some invalid arguments.",
                        Name);
        }



        public virtual bool IsReusable
        {
            get { return false; }
        }
    }

}
