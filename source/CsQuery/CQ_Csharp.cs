using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CsQuery.Utility;
using CsQuery.Engine;
using CsQuery.ExtensionMethods;

namespace CsQuery
{
    /// <summary>
    /// This partial contains properties & methods that are specific to the C# implementation and not part of jQuery
    /// </summary>
    public partial class CQ
    {
        #region private properties

        private Selector _Selectors = null;
        private IDomDocument _Document = null;

        #endregion

        #region public properties

        /// <summary>
        /// Represents the full, parsed DOM for an object created with an HTML parameter.
        /// </summary>

        public IDomDocument Document
        {
            get
            {
                if (_Document == null)
                {
                    CreateNewDocument();
                }
                return _Document;
            }
            protected set
            {
                _Document = value;
            }
        }

        /// <summary>
        /// The selector (parsed) used to create this instance.
        /// </summary>

        public Selector Selectors
        {
            get
            {
                return _Selectors;
            }
            protected set
            {
                _Selectors = value;
            }
        }

        /// <summary>
        /// The entire selection set as an enumerable, same as enumerting on the object itself (though
        /// this may allow you to more easily use extension methods)
        /// </summary>

        public IEnumerable<IDomObject> Selection
        {
            get
            {
                return SelectionSet;
            }
        }

        /// <summary>
        /// Returns just IDomElements from the selection list.
        /// </summary>

        public IEnumerable<IDomElement> Elements
        {
            get
            {
                return onlyElements(SelectionSet);
            }
        }

        /// <summary>
        /// Gets or sets the order in which the selection set is returned. Usually, this is the order
        /// that elements appear in the DOM. Some operations could result in a selection set that's in an
        /// arbitrary order, though.
        /// </summary>

        public SelectionSetOrder Order
        {
            get
            {
                return SelectionSet.Order;
            }
            set
            {
                SelectionSet.Order = value;
            }
        }

      

        #endregion

        #region public methods

        /// <summary>
        /// Returns the HTML for all selected documents, separated by commas. No inner html or children
        /// are included.
        /// </summary>
        ///
        /// <remarks>
        /// This method does not return valid HTML, but rather a single string containing an abbreviated
        /// version of the markup for only documents in the selection set, separated by commas. This is
        /// intended for inspecting a selection set, for example while debugging.
        /// </remarks>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string SelectionHtml()
        {
            return SelectionHtml(false);
        }

        /// <summary>
        /// Returns the HTML for all selected documents, separated by commas.
        /// </summary>
        ///
        /// <remarks>
        /// This method does not return valid HTML, but rather a single string containing an abbreviated
        /// version of the markup for only documents in the selection set, separated by commas. This is
        /// intended for inspecting a selection set, for example while debugging.
        /// </remarks>
        ///
        /// <param name="includeInner">
        /// When true, the complete HTML (e.g. including children) is included for each element.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string SelectionHtml(bool includeInner)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDomObject elm in this)
            {

                sb.Append(sb.Length == 0 ? String.Empty : ", ");
                sb.Append(includeInner ? elm.Render() : elm.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Renders just the selection set completely.
        /// </summary>
        ///
        /// <remarks>
        /// This method will only render the HTML for elements in the current selection set. To render
        /// the entire document for output, use the Render method.
        /// </remarks>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string RenderSelection()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IDomObject elm in this)
            {
                sb.Append(elm.Render());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Renders the document to a string.
        /// </summary>
        ///
        /// <remarks>
        /// This method renders the entire document, regardless of the current selection. This is the
        /// primary method used for rendering the final HTML of a document after manipulation; it
        /// includes the &lt;doctype&gt; and &lt;html&gt; nodes.
        /// </remarks>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string Render()
        {
            return Document.Render();
        }

        /// <summary>
        /// Render the entire document, parsed through a formatter passed using the parameter.
        /// </summary>
        ///
        /// <remarks>
        /// CsQuery by default does not format the output at all, but rather returns exactly the same
        /// contents of each element from the source, including all extra whitespace. If you want to
        /// produce output that is formatted in a specific way, you can create an OutputFormatter for
        /// this purpose. The included <see cref="T:CsQuery.OutputFormatters.FormatPlainText"/> does some
        /// basic formatting by removing extra whitespace and adding newlines in a few useful places.
        /// (This formatter is pretty basic). A formatter to perform indenting to create human-readable
        /// output would be useful and will be included in some future release.
        /// </remarks>
        ///
        /// <param name="format">
        /// An object that parses a CQ object and returns a string of HTML.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string Render(IOutputFormatter format)
        {
            return format.Format(this);
        }

        /// <summary>
        /// Render the complete DOM with specific options.
        /// </summary>
        ///
        /// <param name="renderingOptions">
        /// The options flags in effect.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML
        /// </returns>

        public string Render(DomRenderingOptions renderingOptions)
        {
            Document.DomRenderingOptions = renderingOptions;
            return Render();
        }

        /// <summary>
        /// Render the entire document, parsed through a formatter passed using the parameter, with the
        /// specified options.
        /// </summary>
        ///
        /// <param name="formatter">
        /// The formatter.
        /// </param>
        /// <param name="renderingOptions">
        /// The options flags in effect.
        /// </param>
        ///
        /// <returns>
        /// A string of HTML.
        /// </returns>

        public string Render(IOutputFormatter formatter, DomRenderingOptions renderingOptions)
        {
            Document.DomRenderingOptions = renderingOptions;
            return Render(formatter);
        }

        /// <summary>
        /// Create a new, empty CsQuery object bound to this domain.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>

        public CQ New()
        {
            CQ csq = new CQ();
            csq.CsQueryParent = this;
            return csq;
        }

        /// <summary>
        /// Returns a new empty CsQuery object bound to this domain, whose results are returned in the
        /// specified order.
        /// </summary>
        ///
        /// <remarks>
        /// Usually, CQ objects return the elements in their selection set in the order that they appear
        /// in the DOM. Some operations could result in a selection set that's in an arbitrary order,
        /// though.
        /// </remarks>
        ///
        /// <param name="order">
        /// The order in which the selection set is returned.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object.
        /// </returns>

        public CQ New(SelectionSetOrder order)
        {
            CQ csq = new CQ();
            csq.CsQueryParent = this;
            csq.Order = order;
            return csq;
        }

        /// <summary>
        /// Return a CsQuery object wrapping the enumerable passed, or the object itself if it's already
        /// a CsQuery obect. Unlike CsQuery(context), this will not create a new CsQuery object from an
        /// existing one.
        /// </summary>
        ///
        /// <param name="elements">
        /// A sequence of IDomObject elements.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object when the source is disconnect elements, or the CQ object passed.
        /// </returns>

        public CQ EnsureCsQuery(IEnumerable<IDomObject> elements)
        {
            return elements is CQ ? (CQ)elements : new CQ(elements);
        }

        /// <summary>
        /// The first IDomElement (e.g. not text/special nodes) in the selection set, or null if none
        /// exists.
        /// </summary>
        ///
        /// <returns>
        /// An IDomElement object.
        /// </returns>

        public IDomElement FirstElement()
        {

            using (IEnumerator<IDomElement> enumer = Elements.GetEnumerator())
            {
                if (enumer.MoveNext())
                {
                    return enumer.Current;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Given two selectors, shows the content of one, and removes the content of the other, based on
        /// the boolean parameter.
        /// </summary>
        ///
        /// <param name="which">
        /// A boolean value to indicate whether the first or second selector should be used to determine
        /// the elements that are kept. When true, the first is kept and the 2nd removed. When false, the
        /// opposite happens.
        /// </param>
        /// <param name="trueSelector">
        /// The true selector.
        /// </param>
        /// <param name="falseSelector">
        /// The false selector.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ KeepOne(bool which, string trueSelector, string falseSelector)
        {
            return KeepOne(which ? 0 : 1, trueSelector, falseSelector);
        }

        /// <summary>
        /// Given two CQ objects, shows the one, and removes the the other from the document, based on
        /// the boolean parameter.
        /// </summary>
        ///
        /// <param name="which">
        /// A boolean value to indicate whether the first or second selector should be used to determine
        /// the elements that are kept. When true, the first is kept and the 2nd removed. When false, the
        /// opposite happens.
        /// </param>
        /// <param name="trueContent">
        /// The true content.
        /// </param>
        /// <param name="falseContent">
        /// The false content.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ KeepOne(bool which, CQ trueContent, CQ falseContent)
        {
            return KeepOne(which ? 0 : 1, trueContent, falseContent);
        }

        /// <summary>
        /// Removes all but one of a list selectors/objects based on the zero-based index of the first
        /// parameter. The remaining one is explicitly shown.
        /// </summary>
        ///
        /// <param name="which">
        /// An integer representing the zero-based index of the content from the list of items passed
        /// which should be kept and shown.
        /// </param>
        /// <param name="content">
        /// A variable-length parameters list containing content.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        public CQ KeepOne(int which, params string[] content)
        {
            CQ[] arr = new CQ[content.Length];
            for (int i = 0; i < content.Length; i++)
            {
                arr[i] = Select(content[i]);
            }
            return KeepOne(which, arr);
        }

        /// <summary>
        /// Removes all but one of a list selectors/objects based on the zero-based index of the first
        /// parameter. The remaining one is explicitly shown.
        /// </summary>
        ///
        /// <param name="which">
        /// An integer representing the zero-based index of the content from the list of items passed
        /// which should be kept and shown.
        /// </param>
        /// <param name="content">
        /// A variable-length parameters list containing content.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ KeepOne(int which, params CQ[] content)
        {
            for (int i = 0; i < content.Length; i++)
            {
                if (i == which)
                {
                    content[i].Show();
                }
                else
                {
                    content[i].Remove();
                }
            }
            return this;
        }

        /// <summary>
        /// Conditionally includes a selection. This is the equivalent of calling Remove() only when
        /// "include" is false.
        /// </summary>
        ///
        /// <param name="include">
        /// true to include, false to exclude.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        public CQ IncludeWhen(bool include)
        {
            if (!include)
            {
                Remove();
            }
            return this;
        }

        /// <summary>
        /// Set a specific item, identified by the 2nd parameter, of a named option group, identified by
        /// the first parameter, as selected.
        /// </summary>
        ///
        /// <param name="groupName">
        /// The value of the name attribute identifying this option group.
        /// </param>
        /// <param name="value">
        /// The option value to set as selected
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        public CQ SetSelected(string groupName, IConvertible value)
        {
            var group = this.Find("input[name='" + groupName + "']");
            var item = group.Filter("[value='" + value + "']");
            if (group.Length == 0)
            {
                item = this.Find("#" + groupName);
            }
            if (item.Length > 0)
            {
                string nodeName = group[0].NodeName;
                string type = group[0]["type"].ToUpper();
                if (nodeName == "OPTION")
                {
                    var ownerMultiple = group.Closest("select").Prop("multiple");
                    if (Objects.IsTruthy(ownerMultiple))
                    {
                        item.Prop("selected", true);
                    }
                    else
                    {
                        group.Prop("selected", false);
                        item.Prop("selected", true);
                    }
                }
                else if (nodeName == "INPUT" && (type == "RADIO" || type == "CHECKBOX"))
                {
                    if (type == "RADIO")
                    {
                        group.Prop("checked", false);
                    }
                    item.Prop("checked", true);
                }
            }
            return this;
        }

        /// <summary>
        /// Given a table header or cell, returns all members of the column in the table. This will most
        /// likely not work as you would expect if there are colspan cells.
        /// </summary>
        ///
        /// <returns>
        /// A new CQ object containing all the th and td cells in the specified column.
        /// </returns>

        public CQ GetTableColumn()
        {
            var els = this.Filter("th,td");
            CQ result = New();
            foreach (var el in els)
            {
                var elCq = el.Cq();
                int colIndex = elCq.Index();
                result.AddSelection(elCq.Closest("table").GetTableColumn(colIndex));
            }
            return result;
        }

        /// <summary>
        /// Selects then zero-based nth cells  (th and td) from all rows in any matched tables. This will
        /// most likely no do what you expect if the table has colspan cells.
        /// </summary>
        ///
        /// <param name="column">
        /// The zero-based index of the column to target.
        /// </param>
        ///
        /// <returns>
        /// A new CQ object containing all the th and td cells in the specified column.
        /// </returns>

        public CQ GetTableColumn(int column)
        {
            CQ result = New();
            foreach (var el in filterElements(this, "table"))
            {

                result.AddSelection(el.Cq().Find(String.Format("tr>th:eq({0}), tr>td:eq({0})", column)));

            }
            return result;
        }

        /// <summary>
        /// Perform a substring replace on the contents of the named attribute in each item in the
        /// selection set.
        /// </summary>
        ///
        /// <param name="name">
        /// The attribute name.
        /// </param>
        /// <param name="replaceWhat">
        /// The string to match.
        /// </param>
        /// <param name="replaceWith">
        /// The value to replace each occurrence with.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object.
        /// </returns>

        public CQ AttrReplace(string name, string replaceWhat, string replaceWith)
        {
            foreach (IDomElement item in SelectionSet)
            {
                string val = item[name];
                if (val != null)
                {
                    item[name] = val.Replace(replaceWhat, replaceWith);
                }
            }
            return this;
        }

        /// <summary>
        /// The current selection set will become the only members of the document in this object. This
        /// is a destructive method that will completely replace the document.
        /// </summary>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        public CQ MakeRoot()
        {
            Document.ChildNodes.Clear();
            Document.ChildNodes.AddRange(Elements);
            return this;
        }

        /// <summary>
        /// The elements identified by the selector will become the only members of the document in this
        /// object. This is a destructive method that will completely replace the document.
        /// </summary>
        ///
        /// <param name="selector">
        /// A selector that determines which elements will become the new document.
        /// </param>
        ///
        /// <returns>
        /// The current CQ object
        /// </returns>

        public CQ MakeRoot(string selector)
        {
            return Select(selector).MakeRoot();
        }

        public override string ToString()
        {
            return SelectionHtml();
        }

        #endregion

        #region interface members

        public IEnumerator<IDomObject> GetEnumerator()
        {
            return SelectionSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SelectionSet.GetEnumerator();
        }

        #endregion

    }
}
