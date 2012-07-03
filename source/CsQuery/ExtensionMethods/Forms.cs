using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Text;
using System.Reflection;

using CsQuery.ExtensionMethods.Internal;
using CsQuery.Utility;

namespace CsQuery.ExtensionMethods.Forms
{
    /// <summary>
    /// Extension methods for use in form manipulation
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the value for a particular form element identified by "#ID" or "name". This method will create a selector
        /// that identifies any input, select, button or textarea element by name attribute (if not passed an ID selector)
        /// </summary>
        /// <typeparam name="T">The datatype that should be returned</typeparam>
        /// <param name="obj">The CsQuery object to which this applies</param>
        /// <param name="name">The name of the input element</param>
        /// <returns></returns>
        public static string FormValue(this CQ obj, string name)
        {
            return FormValue<string>(obj, name);
        }

        /// <summary>
        /// Get the value for a particular form element identified by "#ID" or "name"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T FormValue<T>(this CQ obj, string name)
        {
            var sel = FormElement(obj, name);
            if (sel.Length > 0)
            {
                return sel.Val<T>();
            }
            else
            {
                return default(T);
            }
        }
        /// <summary>
        /// Return an element identified by "#id" or "name". (Special case selector to simplify accessing form elements).
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CQ FormElement(this CQ obj, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or missing.");
            }
            return name[0] == '#' ?
                obj[name] :
                obj[String.Format("input[name='{0}'], select[name='{0}'], button[name='{0}'], textarea[name='{0}']", name)];

            
        }

        /// <summary>
        /// (BETA) Update form values from the HTTP post data in the current HttpContext
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CQ RestorePost(this CQ obj)
        {

            return RestorePost(obj, HttpContext.Current.Request.Form);
        }

        /// <summary>
        /// (BETA) Update form values from the data in collection provided
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A CQ object with all form elements searched</returns>
        public static CQ RestorePost(this CQ selection, NameValueCollection postData)
        {
            string selector = "input[name], select[name], button[name], textarea";
            CQ src = selection.Selector == null ?
                selection.Select(selector) :
                selection.Find(selector);

            HashSet<string> keys = new HashSet<string>(postData.AllKeys);
            foreach (IDomElement e in src)
            {
                if (keys.Contains(e.Name))
                {
                    RestoreData(e, selection, postData[e.Name]);
                }
            }
            return selection;

        }

        /// <summary>
        /// (BETA) Update form values from the data in httpContext.Request.Form
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>A CQ object with all form elements searched</returns>
        public static CQ RestorePost(this CQ selection, HttpContext httpContext)
        {
            return RestorePost(selection,httpContext.Request.Form);

        }
        /// <summary>
        /// Restore "value" to a single element
        /// </summary>
        /// <param name="e"></param>
        /// <param name="csQueryContext"></param>
        /// <param name="value"></param>
        private static void RestoreData(IDomElement e, CQ csQueryContext, string value)
        {
            switch (e.NodeName)
            {
                case "TEXTAREA":
                    e.InnerText = value;
                    break;
                case "INPUT":
                    switch (e["type"])
                    {
                        case "checkbox":
                        case "radio":
                            if (value != null && 
                                ((e.Value ?? "on") == value))
                            {
                                e.SetAttribute("checked");
                            }
                            else
                            {
                                e.RemoveAttribute("checked");
                            }
                            break;
                        case "hidden":
                        case "text":
                        case "password":
                        case "button":
                        case "submit":
                        case "image":
                            e.SetAttribute("value", value);
                            break;
                        case "file":
                            break;
                        default:
                            e.SetAttribute("value", value);
                            break;
                    }
                    break;
                case "SELECT":
                case "BUTTON":
                    csQueryContext[e].Val(value);
                    break;
                        
                default:
                    throw new InvalidOperationException(String.Format("An unknown element type was found while restoring post data: '{0}'", e.NodeName));
            }
            

        }
        /// <summary>
        /// Build a dropdown list for each element in the selection set using name/value pairs from data.
        /// Note tha the "key" becomes the "value" on the element, and the "value" becomes the text assocaited
        /// with it.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CQ CreateDropDown(this CQ selection, IEnumerable<KeyValuePair<string, object>> data, string zeroText = null)
        {
            foreach (var el in selection.Elements.Where(item => item.NodeName == "SELECT"))
            {
                CreateDropDown(el, data, zeroText);
            }
            return selection;
        }
        /// <summary>
        /// Create a list from an enum's values & descriptions
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="zeroText">If non-null, the text for any zero value will be this instead of the enum's description</param>
        /// <param name="format">When true, will attempt to format camelcased values</param>
        /// <returns></returns>
        public static CQ CreateDropDownFromEnum<T>(this CQ selection, string zeroText = null, bool format = false) where T : IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return CreateDropDown(selection, EnumKeyValuePairs(typeof(T), zeroText, format));
        }

        /// <summary>
        /// Adds or removes the "enabled" property based on the parameter value
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="addRemoveSwitch"></param>
        /// <returns></returns>
        public static CQ ToggleDisabled(this CQ selection, bool addRemoveSwitch)
        {
            return selection.Each((el) =>
            {
                selection.Prop("disabled", addRemoveSwitch);
            });

        }

        #region private methods

        private static IEnumerable<KeyValuePair<string, object>> EnumKeyValuePairs(Type enumType, string zeroText = null, bool format = false)
        {
            Array enumValArray = Enum.GetValues(enumType);

            foreach (int val in enumValArray)
            {
                string text = "";
                text = val == 0 && zeroText != null ?
                    zeroText :
                    text = FormatEnumText(Enum.Parse(enumType, val.ToString()).ToString());
                yield return new KeyValuePair<string, object>(val.ToString(), text);
            }
        }

        private static void CreateDropDown(IDomElement el, IEnumerable<KeyValuePair<string, object>> data, string zeroText = null)
        {
            foreach (var kvp in data)
            {
                var opt = el.Document.CreateElement("option");
                opt["value"] = kvp.Key;
                el.AppendChild(opt);

                var text =
                    el.Document.CreateTextNode(
                    zeroText != null && kvp.Key == "0" ?
                        zeroText : kvp.Value.ToString()
                    );

                opt.AppendChild(text);
            }
        }
        private static string FormatEnumText(string enumText)
        {
            char[] text = enumText.ToCharArray();
            char lastChar = '_';
            string result = "";
            for (int i = 0; i < text.Length; i++)
            {
                result += (text[i] >= 'A' && text[i] <= 'Z' && lastChar != '_' ? " " : "") + text[i].ToString();
                lastChar = text[i];
            }
            return (result.Replace("_", "-"));
        }

        #endregion
    }

}
