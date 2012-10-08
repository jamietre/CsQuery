using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CsQuery.Implementation;

namespace CsQuery.Output
{
    public interface IOutputFormatter
    {
        void Render(IDomObject node,TextWriter writer);
        string Render(IDomObject node);

    }
}
