using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CsQuery.Mvc
{
    internal class Helpers
    {
        /// <summary>
        /// Return true if the reflected method appears to be a CsQuery target method.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <param name="mi">
        /// The MethodInfo to test
        /// </param>
        ///
        /// <returns>
        /// true the method has a CQ method signature, false if not
        /// </returns>

        public static bool HasCqSignature(MethodInfo mi)
        {
            var pi = mi.GetParameters();

            if (mi.Name.StartsWith("Cq_"))
            {
                if (mi.IsStatic)
                {
                    throw new ArgumentException(IncorrectSignatureError());
                }

                switch (pi.Length)
                {
                    case 0:
                        return true;
                    case 1:
                        // verify parameters are correct
                        if (pi[0].ParameterType != typeof(CQ) ||
                            mi.ReturnType != typeof(void))
                        {
                            throw new ArgumentException(IncorrectSignatureError());
                        }
                        return true;
                    default:
                        throw new ArgumentException(IncorrectSignatureError());
                }
            }
            else
            {
                return false;
            }
        }

        private static string IncorrectSignatureError()
        {
            return "Incorrect signature: a method starting with Cq_ must return void, have zero parameters, or a single CQ parameter.";
        }
    }
}
