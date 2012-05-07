using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Utility.StringScanner
{

    public interface IValueInfo
    {
        bool Alpha { get; }
        bool Numeric { get; }
        bool NumericExtended { get; }
        bool Lower { get; }
        bool Upper { get; }
        bool Whitespace { get; }
        bool Alphanumeric { get; }
        bool Operator { get; }
        /// <summary>
        /// Indicates that a character is alphabetic-like character defined as a-z, A-Z, hyphen, underscore, and ISO 10646 code U+00A1 and higher.
        /// (per characters allowed in CSS identifiers)
        /// </summary>
        bool AlphaISO10646 { get; }


        IConvertible Target { get; set; }
    }
    public interface IValueInfo<T> : IValueInfo where T : IConvertible
    {
        new T Target { get; set; }
    }
    public interface ICharacterInfo : IValueInfo<char>
    {
        bool Parenthesis { get; }
        bool Enclosing { get; }
        bool Bound { get; }
        bool Quote { get; }
        bool Separator { get; }
    }
    public interface IStringInfo : IValueInfo<string>
    {

        bool HtmlAttributeName { get; }
        bool HasAlpha { get; }
    }
}
