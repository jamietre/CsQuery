using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CsQuery.Mvc
{
    /// <summary>
    /// A list of virtual paths
    /// </summary>
    
    [Serializable]
    public class PathList: IList<string>
    {

        #region private properties

        private List<string> InnerList = new List<string>();
        
        #endregion

        #region public properties

        public int IndexOf(string item)
        {
            return InnerList.IndexOf(Normalize(item));
        }

        public void Insert(int index, string item)
        {
            string norm = Normalize(item);
            if (!InnerList.Contains(norm))
            {
                InnerList.Insert(index, norm);
            }
        }

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }

        public void Add(string item)
        {
            string norm = Normalize(item);
            if (!InnerList.Contains(norm))
            {
                InnerList.Add(norm);
            }
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(string item)
        {
            return InnerList.Contains(Normalize(item));
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return InnerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return InnerList.Remove(Normalize(item));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        #endregion

        #region private methods

        private string Normalize(string virtualPath)
        {
            if (!virtualPath.StartsWith("/") && !virtualPath.StartsWith("~/")) {
                virtualPath = "~/"+virtualPath;
            }

            return virtualPath.ToLower();

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
