using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.HtmlParser
{
    public class IterationData
    {
        public IterationData Parent;
        public IDomObject Object;
        public IDomElement Element
        {
            get
            {
                return (IDomElement)Object;
            }
        }
        // when true, the contents will be treated as text until the next close tag
        public bool ReadTextOnly;
        public int Pos;
        public int Step;
        public bool Finished;
        public bool AllowLiterals;
        public bool Invalid;
        public int HtmlStart;
        
        /// <summary>
        /// Use this to prepare the iterator object to continue finding siblings. It retains the parent. It just avoids having to recreate
        /// an instance of this object for the next tag.
        /// </summary>
        public void Reset()
        {
            Step = 0;
            HtmlStart = Pos;
            ReadTextOnly = false;
            Object = null;
        }
        public void Reset(int pos)
        {
            Pos = pos;
            Reset();
        }
    }
}
