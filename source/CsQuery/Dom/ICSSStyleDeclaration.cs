using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery
{
    public interface ICSSStyleDeclaration
    {
        bool HasStyle(string styleName);

        void SetStyles(string styles);
        void SetStyles(string styles, bool strict);

        void SetStyle(string name, string value);
        void SetStyle(string name, string value, bool strict);
        
        string GetStyle(string name);
        bool RemoveStyle(string name);

    }
}

