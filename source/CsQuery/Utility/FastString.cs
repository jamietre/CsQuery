using System.Text;

namespace CsQuery.Utility
{
    /// <summary>
    /// A string-class optimized for appending lots of text. Can support null as the
    /// initial string, which is used by some nodes (<seealso cref="IDomComment"/>).
    /// </summary>
    public class FastString
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private string _value;
        private bool _isDirty;

        public FastString(string value)
        {
            // Keep an initial null string as-is and only add/append non-null strings
            if (value != null)
            {
                AppendValue(value);
            }
        }

        public string Value
        {
            get
            {
                if (_isDirty)
                {
                    _value = _stringBuilder.ToString();
                    _isDirty = false;
                }
                return _value;
            }
        }

        public void AppendValue(string text)
        {
            _stringBuilder.Append(text);
            _isDirty = true;
        }
    }
}
