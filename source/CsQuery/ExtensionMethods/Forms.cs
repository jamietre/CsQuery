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
using CsQuery.HtmlParser;

namespace CsQuery.ExtensionMethods.Forms
{
    /// <summary>
    /// Extension methods for use in form manipulation
    /// </summary>
    public static class ExtensionMethods
    {
        public static string Serialize(this CQ selection)
        {
            var doms = selection.SelectMany<IDomObject, IDomObject>(d =>
            {
                var cq = d.Cq();
                if (cq.Is("form"))
                {
                    return cq.Find(":input[name]:not(:image):not(:submit):not(:file):not(:checkbox):not(:radio)")
                        .Add(cq.Find(":checkbox[name]:checked, :radio[name]:checked"));
                }
                else if (cq.Is(":input[name]:not(:image):not(:submit):not(:file)"))
                {
                    if (!cq.Is(":checkbox, :radio") || cq.Is(":checkbox:checked, :radio:checked"))
                    {
                        return cq;
                    }
                }
                return Enumerable.Empty<IDomObject>();
            }).ToList();

            var pairs = doms.Select(d => String.Format("{0}={1}", urlEncode(d.Name), urlEncode(d.Value ?? "on"))).ToList();
            return String.Join("&", pairs);
        }

        /// <summary>
        /// Get the value for a particular form element identified by "#ID" or "name". This method will
        /// create a selector that identifies any input, select, button or textarea element by name
        /// attribute (if not passed an ID selector)
        /// </summary>
        ///
        /// <param name="obj">
        /// The CsQuery object to which this applies.
        /// </param>
        /// <param name="name">
        /// The name of the input element.
        /// </param>
        ///
        /// <returns>
        /// A string that represents the form field value.
        /// </returns>

        public static string FormValue(this CQ obj, string name)
        {
            return FormValue<string>(obj, name);
        }

        /// <summary>
        /// Get the value for a particular form element identified by "#ID" or "name".
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type to cast the value to
        /// </typeparam>
        /// <param name="context">
        /// The context in which to find the element
        /// </param>
        /// <param name="name">
        /// The name of the form element
        /// </param>
        ///
        /// <returns>
        /// A value of type T
        /// </returns>

        public static T FormValue<T>(this CQ context, string name)
        {
            var sel = FormElement(context, name);
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
        /// Return an element identified by "#id" or "name". (Special case selector to simplify accessing
        /// form elements).
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown if the name was invalid
        /// </exception>
        ///
        /// <param name="context">
        /// The context in which to find the element
        /// </param>
        /// <param name="name">
        /// The name of the form element
        /// </param>
        ///
        /// <returns>
        /// A CQ object with the form element.
        /// </returns>

        public static CQ FormElement(this CQ context, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or missing.");
            }
            return name[0] == '#' ?
                context[name] :
                context[String.Format("input[name='{0}'], select[name='{0}'], button[name='{0}'], textarea[name='{0}']", name)];

            
        }

        /// <summary>
        /// (BETA) Update form values from the HTTP post data in the current HttpContext.
        /// </summary>
        ///
        /// <param name="context">
        /// The context in which to find process form elements
        /// </param>
        ///
        /// <returns>
        /// The context
        /// </returns>

        public static CQ RestorePost(this CQ context)
        {

            return RestorePost(context, HttpContext.Current.Request.Form);
        }

        /// <summary>
        /// (Beta) Update form values from the data in collection provided.
        /// </summary>
        ///
        /// <param name="selection">
        /// The selections set to update form values within; only child elements of elements in the
        /// selection set will be processed.
        /// </param>
        /// <param name="postData">
        /// Information describing the post.
        /// </param>
        ///
        /// <returns>
        /// A CQ object with all form elements searched.
        /// </returns>

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
        /// (Alpha) Update form values from the data in httpContext.Request.Form.
        /// </summary>
        ///
        /// <param name="selection">
        /// The selections set to update form values within; only child elements of elements in the
        /// selection set will be processed.
        /// </param>
        /// <param name="httpContext">
        /// The HttpContext from which to obtain post data
        /// </param>
        ///
        /// <returns>
        /// A CQ object with all form elements searched.
        /// </returns>

        public static CQ RestorePost(this CQ selection, HttpContext httpContext)
        {
            return RestorePost(selection,httpContext.Request.Form);

        }

        /// <summary>
        /// Restore "value" to a single element.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown when the requested operation is invalid.
        /// </exception>
        ///
        /// <param name="element">
        /// The element to restore a value to
        /// </param>
        /// <param name="csQueryContext">
        /// The context to which this element belongs
        /// </param>
        /// <param name="value">
        /// The value to restore
        /// </param>

        private static void RestoreData(IDomElement element, CQ csQueryContext, string value)
        {
            switch (element.NodeNameID)
            {
                case HtmlData.tagTEXTAREA:
                    element.TextContent = value;
                    break;
                case HtmlData.tagINPUT:
                    switch (element["type"])
                    {
                        case "checkbox":
                        case "radio":
                            if (value != null && 
                                ((element.Value ?? "on") == value))
                            {
                                element.SetAttribute("checked");
                            }
                            else
                            {
                                element.RemoveAttribute("checked");
                            }
                            break;
                        case "hidden":
                        case "text":
                        case "password":
                        case "button":
                        case "submit":
                        case "image":
                            element.SetAttribute("value", value);
                            break;
                        case "file":
                            break;
                        default:
                            element.SetAttribute("value", value);
                            break;
                    }
                    break;
                case HtmlData.tagSELECT:
                case HtmlData.tagBUTTON:
                    csQueryContext[element].Val(value);
                    break;
                        
                default:
                    throw new InvalidOperationException(String.Format("An unknown element type was found while restoring post data: '{0}'", element.NodeName));
            }
            

        }

        /// <summary>
        /// Build a dropdown list for each element in the selection set using name/value pairs from data.
        /// Note tha the "key" becomes the "value" on the element, and the "value" becomes the text
        /// assocaited with it.
        /// </summary>
        ///
        /// <param name="selection">
        /// The target on which to create the dropdown list
        /// </param>
        /// <param name="data">
        /// The data source for the dropdown list
        /// </param>
        /// <param name="zeroText">
        /// If non-null, the text for any zero value will be this instead of the enum's description.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        public static CQ CreateDropDown(this CQ selection, 
            IEnumerable<KeyValuePair<string, object>> data, 
            string zeroText = null)
        {
            foreach (var el in selection.Elements.Where(item => item.NodeName == "SELECT"))
            {
                CreateDropDown(el, data, zeroText);
            }
            return selection;
        }

        /// <summary>
        /// Create a list from an enum's values &amp; descriptions.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Thrown when one or more arguments have unsupported or illegal values.
        /// </exception>
        ///
        /// <typeparam name="T">
        /// Generic type parameter.
        /// </typeparam>
        /// <param name="selection">
        /// The select element on which to create the list
        /// </param>
        /// <param name="zeroText">
        /// If non-null, the text for any zero value will be this instead of the enum's description.
        /// </param>
        /// <param name="format">
        /// When true, will attempt to format camelcased values.
        /// </param>
        ///
        /// <returns>
        /// The new drop down from enum&lt; t&gt;
        /// </returns>

        public static CQ CreateDropDownFromEnum<T>(this CQ selection, 
            string zeroText = null, 
            bool format = false) where T : IConvertible
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
        
        private static string urlEncode(string value)
        {
            return HttpUtility.UrlEncode(value).RegexReplace(@"(?i)%[0-9A-F]{2}", m => m.Value.ToUpper());
        }

        #endregion
    }

}
