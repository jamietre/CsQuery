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

        public string Arguments
        {
            get
            {
                return _Arguments;
            }
            set
            {
                _Arguments = value;
                if (MaximumParameterCount > 1 || MaximumParameterCount < 0)
                {
                    Parameters = ParseParms(value);
                }
                else
                {
                    Parameters = new string[] { value };
                }
            }
        }

        protected string[] ParseParms(string value)
        {

            List<string> parms = new List<string>();
            int index = 0;


            IStringScanner scanner = Scanner.Create(value);
            OptionallyQuoted pattern = (OptionallyQuoted)MatchFunctions.OptionallyQuoted;
            pattern.Terminators = ",";

            while (!scanner.Finished) {
                if (ParameterQuoted(index)==null) {
                    parms.Add(scanner.Get(MatchFunctions.OptionallyQuoted));
                } else if (ParameterQuoted(index)==true) {
                    parms.Add(scanner.Get(MatchFunctions.Quoted));
                } else {
                    scanner.
                }



        }

        public virtual void Initialize() {

            if (Parameters == null)
            {
                 if (MinimumParameterCount != 0) {
                     throw new ArgumentException(ParameterCountMismatchError());
                 } else {
                     return;
                 }
            }

            if ((Parameters.Length < MinimumParameterCount ||
                    (MaximumParameterCount >= 0 &&
                        (Parameters.Length > MaximumParameterCount))))
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

        protected virtual bool? ParameterQuoted(int index)
        {
            return false;
        }

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
    }

}
