using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.CsQuery.Utility.StringScanner
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
        bool Parenthesis { get; }
        IConvertible Target { get; set; }
    }
    public interface IValueInfo<T> : IValueInfo where T : IConvertible
    {
        new T Target { get; set; }
    }
    public interface ICharacterInfo : IValueInfo<char>
    {

        bool Bound { get; }
        bool Quote { get; }
    }
    public interface IStringInfo : IValueInfo<string>
    {

        bool HtmlAttributeName { get; }
        bool HasAlpha { get; }
    }
}
