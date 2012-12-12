using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsQuery.Implementation
{
    public class TrueStringComparer : IComparer<string>, IEqualityComparer<string>
    {
        public int Compare(string x, string y)
        {
            int pos = 0;
            int len = Math.Min(x.Length, y.Length);
            while (pos < len)
            {
                var xi = (int)x[pos];
                var yi = (int)y[pos];
                if (xi < yi)
                {
                    return -1;
                }
                else if (yi < xi)
                {
                    return 1;
                }
                pos++;
            }
            if (x.Length < y.Length)
            {
                return -1;
            }
            else if (y.Length < x.Length)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public bool Equals(string x, string y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
