using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface IAttributeCollection: IEnumerable<KeyValuePair<string,string>>
    {
        string GetAttribute(string name);
        void SetAttribute(string name, string value);
        string this[string attributeName] { get; set; }
        int Length { get; }
    }
}
