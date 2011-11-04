using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Text;
using System.Reflection;
using Jtc.CsQuery;

namespace Jtc.CsQuery.ExtensionMethods
{
    public static class ExtensionMethods_Internal
    {
        public static bool HasValue(this DropDownList theList, string value)
        {
            for (int i = 0; i < theList.Items.Count; i++)
            {
                if (theList.Items[i].Value == value)
                {
                    return (true);
                }
            }
            return (false);
        }
        /// <summary>
        /// Returns true if the enum is any of the parameters in question
        /// </summary>
        /// <param name="theEnum"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        /// <returns></returns>
        public static bool IsOneOf(this Enum theEnum, Enum value1, Enum value2 = null, Enum value3 = null, Enum value4 = null, Enum value5 = null)
        {
            Enum[] values = { null, value5, value4, value3, value2, value1 };
            int i = values.Length;
            while (values[--i] != null)
            {
                if (theEnum.Equals(values[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsOneOf(this string match, string value1, string value2 = null, string value3 = null, string value4 = null, string value5 = null, bool matchCase=true)
        {
            string[] values = { null, value5, value4, value3, value2, value1 };
            int i = values.Length;
            StringComparison comp = matchCase ? StringComparison.CurrentCulture: StringComparison.CurrentCultureIgnoreCase;
            while (values[--i] != null)
            {
                if (match.Equals(values[i],comp))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool TryParse<T>(this Enum theEnum, string strType, out T result)
        {
            string strTypeFixed = strType.Replace(' ', '_');
            if (Enum.IsDefined(typeof(T), strTypeFixed))
            {
                result = (T)Enum.Parse(typeof(T), strTypeFixed, true);
                return true;
            }
            else
            {
                foreach (string value in Enum.GetNames(typeof(T)))
                {
                    if (value.Equals(strTypeFixed,
                                    StringComparison.OrdinalIgnoreCase))
                    {
                        result = (T)Enum.Parse(typeof(T), value);
                        return true;
                    }
                }
                result = default(T);
                return false;
            }
        }
        public static int GetValue(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static string GetValueAsString(this Enum value)
        {
            return GetValue(value).ToString();
        }
        public static void AddAfter(this Control baseControl, Control control)
        {
            Control parentControl = baseControl.Parent;
            if (parentControl == null)
            {
                throw new Exception("Control must be part of a collection to AddAfter");
            }

            int foundIndex = -1;
            for (int index = 0; index < parentControl.Controls.Count; index++)
            {
                if (parentControl.Controls[index] == baseControl)
                {
                    foundIndex = index;
                    break;
                }
            }
            parentControl.Controls.AddAt(foundIndex + 1, control);
        }
        public static void ReplaceWith(this Control baseControl, Control control)
        {
            baseControl.AddAfter(control);
            baseControl.Parent.Controls.Remove(baseControl);

        }
        public static void Remove(this Control baseControl)
        {
            baseControl.Parent.Controls.Remove(baseControl);
        }
        /// <summary>
        /// Similar to FindControl but accepts an interface for T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Control FindControlType<T>(this Control control, string id)
        {
            return control.FindControlsType<T>(id, true).FirstOrDefault();
        }
        public static Control FindControlType<T>(this Control control)
        {
            return control.FindControlsType<T>(String.Empty, true).FirstOrDefault();
        }
        public static IEnumerable<Control> FindControlsType<T>(this Control control, string id, bool recurse)
        {
            if (control != null)
            {

                foreach (Control ctl in control.Controls)
                {
                    if (ctl is T &&
                        (String.IsNullOrEmpty(id) || id == ctl.ID))
                    {
                        yield return (ctl);
                    }
                    if (recurse && ctl.Controls.Count > 0)
                    {
                        IEnumerable<Control> subCtls = ctl.FindControlsType<T>(id, true);
                        if (subCtls != null)
                        {
                            foreach (Control subCtl in subCtls)
                            {
                                yield return (subCtl);
                            }
                        }
                    }
                }
            }
            yield break;
        }

        public static Control FindControl(this Control control, string id, bool recurse)
        {
            return control.FindControl<Control>(id, recurse);
        }

        /// <summary>
        /// Recursively find first control of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseControl"></param>
        /// <returns></returns>
        public static T FindControl<T>(this Control control) where T : Control
        {
            return control.FindControl<T>(true);
        }

        public static T FindControl<T>(this Control control, string id) where T : Control
        {
            return control.FindControl<T>(id, true);
        }
        /// <summary>
        /// Find the first control of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseControl"></param>
        /// <returns></returns>
        public static T FindControl<T>(this Control control, bool recurse) where T : Control
        {
            return control.FindControl<T>(String.Empty, recurse);
        }
        /// <summary>
        /// Recursively find the first control of type T and specific id. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="?"></param>
        /// <returns></returns>
        public static T FindControl<T>(this Control control, string id, bool recurse) where T : Control
        {
            return control.FindControls<T>(id, recurse, false).FirstOrDefault();
        }
        /// <summary>
        /// Find all controls of type T in control.Controls and return in List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseControl"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindControls<T>(this Control control) where T : class
        {
            return (control.FindControls<T>(String.Empty, true, false));
        }
        /// <summary>
        /// Find all controls of type T recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindControls<T>(this Control control, string id) where T : Control
        {
            return control.FindControls<T>(id, true, false);
        }
        public static IEnumerable<T> FindControls<T>(this Control control, bool recurse) where T : Control
        {
            return control.FindControls<T>(String.Empty, true, false);
        }
        /// <summary>
        /// Find all controls of type T with ID starting with 'id' in control.Controls and return in List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">The control type to match</typeparam>
        /// <param name="baseControl">The control from which to begin the search.</param>
        /// <param name="id">The id substring to match</param>
        /// <returns></returns>
        public static IEnumerable<T> FindControls<T>(this Control control, string id, bool recurse, bool matchIDPrefix) where T : class
        {
            if (control == null)
            {
                yield break;
            }
            foreach (Control ctl in control.Controls)
            {
                if (ctl is T &&
                    (String.IsNullOrEmpty(id) ? true :
                        (String.IsNullOrEmpty(ctl.ID) || ctl.ID.Length < id.Length ? false :
                            matchIDPrefix ? ctl.ID.Substring(0, id.Length) == id : ctl.ID == id)))
                {
                    yield return ctl as T;

                }
                if (ctl.Controls.Count > 0)
                {
                    IEnumerable<T> subControls = ctl.FindControls<T>(id, recurse, matchIDPrefix);
                    if (subControls != null)
                    {
                        foreach (T subCtl in subControls)
                        {
                            yield return subCtl;
                        }
                    }
                }
            }
            //return (controls);
        }
        /// <summary>
        /// Returns all controls that match the types 
        /// </summary>
        /// <typeparam name="IEnumerable"></typeparam>
        /// <param name="baseControl"></param>
        /// <param name="recurse"></param>
        /// <returns></returns>
        public static IEnumerable<Control> FindControls(this Control baseControl, IEnumerable<Type> types, bool recurse)
        {
            foreach (Control ctl in baseControl.Controls)
            {
                if (types.Any(item => ctl.GetType().IsAssignableFrom(item.UnderlyingSystemType)))
                {
                    yield return (ctl);
                }
                if (recurse && ctl.Controls.Count > 0)
                {
                    IEnumerable<Control> subCtls = ctl.FindControls(types, true);
                    if (subCtls != null)
                    {
                        foreach (Control subCtl in subCtls)
                        {
                            yield return subCtl;
                        }
                    }
                }

            }
            yield break;
        }
        /// <summary>
        ///  Return the first ancestral relative of a control, recursively moving up the tree until it is found.
        ///  if checkSiblings=true, then the immediate siblings of startControl will be checked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <param name="checkSiblings"></param>
        /// <returns></returns>
        public static T FindCollateralRelative<T>(this Control control, bool checkSiblings = false) where T : Control
        {
            if (control.Parent == null)
            {
                return null;
            }
            if (control.Parent is T)
            {
                return ((T)control.Parent);
            }
            T result = null;
            if (checkSiblings)
            {
                result = control.Parent.FindControl<T>(false);
                if (result != null)
                {
                    return result;
                }
            }
            Control grandParent = control.Parent.Parent;

            if (grandParent == null)
            {
                return null;
            }

            // Scan all children of the grandparent (siblings of the parent)
            // if not found, keep moving up
            result = grandParent.FindControl<T>(false);
            if (result != null)
            {
                return (result);
            }
            else
            {
                return (control.Parent.FindCollateralRelative<T>(false));
            }
        }
        public static Control FindCollateralRelative(this Control control, string id, bool checkSiblings = false)
        {
            if (control.Parent == null)
            {
                return null;
            }
            if (control.Parent.ID == id)
            {
                return (control.Parent);
            }
            Control result = null;
            if (checkSiblings)
            {
                result = control.Parent.FindControl(id);
                if (result != null)
                {
                    return result;
                }
            }
            Control grandParent = control.Parent.Parent;

            if (grandParent == null)
            {
                return null;
            }

            // Scan all children of the grandparent (siblings of the parent)
            // if not found, keep moving up
            result = grandParent.FindControl(id);
            if (result != null)
            {
                return (result);
            }
            else
            {
                return (control.Parent.FindCollateralRelative(id, false));
            }
        }
        // Recursively locate a parent of base type T
        public static T FindParent<T>(this Control control) where T : Control
        {
            if (control.Parent == null)
            {
                return (null);
            }
            if (control.Parent is T)
            {
                return ((T)control.Parent);
            }
            else
            {
                return (control.Parent.FindParent<T>());
            }
        }
        public static List<T> ToListOf<T>(this IEnumerable baseList)
        {
            List<T> newList = new List<T>();
            foreach (T obj in baseList)
            {
                newList.Add(obj);
            }
            return (newList);
        }


        public static T ToList<T>(this IEnumerable baseList) where T : IList, new()
        {
            T newList = new T();

            foreach (object obj in baseList)
            {
                newList.Add(obj);
            }
            return (newList);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> baseList) where T : new()
        {
            HashSet<T> newList = new HashSet<T>();

            foreach (T obj in baseList)
            {
                newList.Add(obj);
            }
            return (newList);
        }
        public static void AddRange<T>(this ICollection<T> baseList, IEnumerable<T> list)
        {
            foreach (T obj in list)
            {
                baseList.Add(obj);
            }
        }
        public static bool IsNullOrEmpty<T>(this ICollection<T> baseList)
        {
            return (baseList == null ||
                baseList.Count == 0);

        }
        public static int OccurrencesOf(this string text, char find) {
            int pos=0;
            int count = 0;
            while ((pos = text.IndexOf(find,pos))>=0)
            {
                count++;
                pos++;
            }
            return count;
        }

        public static string ListAdd(this string list, string value, string separator)
        {
            if (String.IsNullOrEmpty(value))
            {
                return list.Trim();
            }
            if (list == null)
            {
                list = String.Empty;
            }
            else
            {
                list = list.Trim();
            }

            int pos = (list + separator).IndexOf(value + separator);
            if (pos < 0)
            {
                if (list.LastIndexOf(separator) == list.Length - separator.Length)
                {
                    // do not add separator - it already exists
                    return list + value;
                }
                else
                {
                    return (list + (list == "" ? "" : separator) + value);
                }
            }
            else
            {
                // already has value
                return (list);
            }
        }
        public static string ListRemove(this string list, string value, string separator)
        {
            string result = (separator + list).Replace(separator + value, "");
            if (result.Substring(0, 1) == separator)
            {
                result = result.Remove(0, 1);
            }
            return (result);
        }
        /// <summary>
        /// Returns the text between startIndex and endIndex (exclusive of endIndex)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string SubstringBetween(this string text, int startIndex, int endIndex)
        {
            return (text.Substring(startIndex, endIndex - startIndex));
        }


        public static string RemoveWhitespace(this string text)
        {
            return text == null ? null : Regex.Replace(text, @"\s+", " ");
        }
        /// <summary>
        /// Returns the part of the string before the last occurence of text
        /// </summary>
        /// <param name="?"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string BeforeLast(this string text, string find)
        {
            int index = text.LastIndexOf(find);
            if (index >= 0)
            {
                return (text.Substring(0, index));
            }
            else
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Returns the part of two strings that match
        /// </summary>
        /// <param name="text"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string CommonStart(this string text, string match)
        {
            int pos = 0;
            if (string.IsNullOrEmpty(text) || String.IsNullOrEmpty(match)) {
                return String.Empty;
            }
            while (pos<text.Length && pos<match.Length)
            {
                if (text.Substring(0, pos+1) != match.Substring(0, pos+1))
                {
                    break;
                }
                pos++;
            }
            return text.Substring(0, pos);

        }
        /// <summary>
        /// Returns the string after the end of the first occurrence of "find"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string After(this string text, string find)
        {
            int index = text.IndexOf(find);
            if (index < 0 || index + find.Length >= text.Length)
            {
                return (String.Empty);
            }
            else
            {
                return (text.Substring(index + find.Length));
            }
        }
        public static string AfterLast(this string text, string find)
        {
            int index = text.LastIndexOf(find);
            if (index < 0 || index + find.Length >= text.Length)
            {
                return (String.Empty);
            }
            else
            {
                return (text.Substring(index + find.Length));
            }
        }
        public static string Before(this string text, string find)
        {
            int index = text.IndexOf(find);
            if (index < 0 || index == text.Length)
            {
                return (String.Empty);
            }
            else
            {
                return (text.Substring(0, index));
            }
        }
        /// <summary>
        /// Returns the current key for a ListView - only relevant in ItemCommand
        /// </summary>
        /// <param name="e"></param>
        /// <param name="listView"></param>
        /// <returns></returns>
        public static int CurrentKey(this ListViewCommandEventArgs e, object listView)
        {
            return CurrentKey(e.Item, (ListView)listView, 0);
        }
        public static int CurrentKey(this ListViewCommandEventArgs e, object listView, int keyIndex)
        {
            return CurrentKey(e.Item, (ListView)listView, keyIndex);
        }
        public static int CurrentKey(this ListViewItemEventArgs e, object listView)
        {
            return CurrentKey(e.Item, (ListView)listView, 0);
        }
        public static int CurrentKey(this ListViewItemEventArgs e, object listView, int keyIndex)
        {
            return CurrentKey(e.Item, (ListView)listView, keyIndex);
        }
        private static int CurrentKey(ListViewItem item, ListView listView, int keyIndex)
        {
            int itemKey = 0;
            if (item.DataItemIndex >= 0)
            {
                itemKey = (int)((ListView)listView).DataKeys[item.DataItemIndex][keyIndex];
            }
            return itemKey;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> baseList)
        {
            if (baseList == null) return true;
            bool result = true;
            // I think this is the most efficient way to verify an empty IEnumerable
            foreach (T t in baseList)
            {
                result = false;
                break;
            }
            return (result);
        }
        public static bool TryGetFirst<T>(this IEnumerable<T> baseList, out T firstElement)
        {
            if (baseList == null)
            {
                firstElement = default(T);
                return false;
            }
            bool result = false;
            // I think this is the most efficient way to verify an empty IEnumerable
            firstElement=default(T);
            foreach (T obj in baseList)
            {
                result = true;
                firstElement = obj;
                break;
            }
            return result;
        }
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> func)
        {
            foreach (T obj in list)
            {
                func(obj);
            }
        }
        /// <summary>
        /// Converts null to String.Empty and trims
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanUp(this string value)
        {
            return (value ?? String.Empty).Trim();
        }
        public static bool IsKeyValuePair(this object obj)
        {
            Type valueType = obj.GetType();
            if (valueType.IsGenericType)
            {
                Type baseType = valueType.GetGenericTypeDefinition();
                if (baseType == typeof(KeyValuePair<,>))
                {
                    return true;
                }
            }
            return false;
        }


        public static string IfNullOrEmpty(this string value, string alternate)
        {
            return value.IsNullOrEmpty() ? alternate : value;
        }
        public static string IfNull(this string value, string alternate)
        {
            return value==null ? alternate : value;
        }
        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action) where T: class
        {
            foreach (T obj in list) {
                action(obj);
            }
            return list;
        }
        public static IEnumerable<T> DoWhen<T>(this IEnumerable<T> list, Action<T> action, Func<T,bool> condition) where T : class
        {
            foreach (T obj in list)
            {
                if (condition(obj)) action(obj);
            }
            return list;
        }
        public static string DoBuildString<T>(this IEnumerable<T> list, Func<T, string> action)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T obj in list)
            {
                sb.Append(action(obj));
            }
            return sb.ToString();
        }
    }
}
