using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Web.Script.Serialization;
using Jtc.CsQuery.ExtensionMethods;

namespace Jtc.CsQuery.Utility
{
    public static class Objects
    {
        public static IEnumerable<T> Enumerate<T>(T obj) 
        {
            List<T> list = new List<T>();
            list.Add(obj);
            return list;
        }
        public static IEnumerable<T> EmptyEnumerable<T>()
        {
            yield break;
        }
        public static object Extend(HashSet<object> parents, bool deep, object target, object source1, object source2 = null, object source3 = null, object source4 = null, object source5 = null, object source6 = null, object source7 = null)
        {
            if (deep && parents == null)
            {
                parents = new HashSet<object>();
                parents.Add(target);
            }
            // Add all non-null parameters to a processing queue
            Queue<object> inputs = new Queue<object>(new object[] { source1, source2, source3, source4, source5, source6, source7 });
            Queue<object> sources= new Queue<object>();
            HashSet<object> unique = new HashSet<object>();

            while (inputs.Count>0)
            {
                object src = inputs.Dequeue();
                if (src is string && ((string)src).IsJson())
                {
                    src= CsQuery.ParseJSON((string)src);
                }
                if (!src.IsExpando() && src is IEnumerable)
                {
                    foreach (var innerSrc in (IEnumerable)src)
                    {
                        inputs.Enqueue(innerSrc);
                    }
                }
                if (!src.IsImmutable() && unique.Add(src))
                {
                    sources.Enqueue(src);
                }
            }
            // Create a new empty object if there's no existing target -- same as using {} as the jQuery parameter
            if (target == null)
            {
                target = new ExpandoObject();
            }

            else if (!target.IsExtendableType())
            {
                throw new Exception("Target type '" + target.GetType().ToString() + "' is not valid for CsQuery.Extend.");
            }

            //sources = sources.Dequeue();
            object source;
            while (sources.Count > 0)
            {
                source = sources.Dequeue();

                if (source.IsExpando())
                {
                    // Expando object -- copy/clone it
                    foreach (var kvp in (IDictionary<string, object>)source)
                    {

                        AddExtendKVP(deep, parents, target, kvp.Key, kvp.Value);
                    }
                }
                else if (source is IEnumerable)
                {
                    // For enumerables, treat each value as another object. Append to the operation list 
                    // This check is after the Expand check since Expandos are elso enumerable
                    foreach (object obj in ((IEnumerable)source))
                    {
                        sources.Enqueue(obj);
                        continue;
                    }
                }
                else
                {
                    // treat it as a regular object - try to copy fields/properties
                    IEnumerable<MemberInfo> members = source.GetType().GetMembers();

                    object value;
                    foreach (var member in members)
                    {
                        if (member is PropertyInfo)
                        {
                            value = ((PropertyInfo)member).GetGetMethod().Invoke(source, null);

                        }
                        else if (member is FieldInfo)
                        {
                            value = ((FieldInfo)member).GetValue(source);
                        }
                        else
                        {
                            //It's a method or something we don't know how to handle. Skip it.
                            continue;
                        }
                        AddExtendKVP(deep, parents, target, member.Name, value);

                    }
                }

            }
            return target;
        }
        private static void AddExtendKVP(bool deep, HashSet<object> parents, object target, string name, object value)
        {
            IDictionary<string, object> targetDict = null;
            if (target.IsExpando())
            {
                targetDict = (IDictionary<string, object>)target;
            }
            if (deep)
            {
                // Prevent recursion by seeing if this value has been added to the object already.
                // Though jQuery skips such elements, we could get away with this because we clone everything
                // during deep copies. The recursing property wouldn't exist yet when we cloned it.

                // for non-expando objects, we still want to add it & skip - but we can't remove a property
                if (value.IsExtendableType()
                    && !parents.Add(value))
                {
                    if (targetDict != null)
                    {
                        targetDict.Remove(name);
                    }
                    return;
                }

                object curValue;
                if (value.IsExtendableType()
                    && targetDict != null
                    && targetDict.TryGetValue(name, out curValue))
                {
                    //targetDic[name]=Extend(parents,true, null, curValue.IsExtendableType() ? curValue : null, value);
                    value = Extend(parents, true, null, curValue.IsExtendableType() ? curValue : null, value);

                }
                else
                {
                    // targetDic[name] = deep ? value.Clone(true) : value;
                    value = value.Clone(true);
                }
            }

            if (targetDict != null)
            {
                targetDict[name] = value;
            }
            else
            {
                // It's a regular object
                IEnumerable<MemberInfo> members = target.GetType().GetMembers();

                foreach (var member in members)
                {
                    if (member.Name.Equals(name, StringComparison.CurrentCulture))
                    {
                        if (member is PropertyInfo)
                        {
                            ((PropertyInfo)member).GetSetMethod().Invoke(value, null);

                        }
                        else if (member is FieldInfo)
                        {
                            ((FieldInfo)member).SetValue(target, value);
                        }
                        else
                        {
                            //It's a method or something we don't know how to handle. Skip it.
                            continue;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Remove a property from an object, returning a new object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static object DeleteProperty(object obj, string property)
        {
            if (!obj.IsExpando())
            {
                throw new Exception("This object does not have properties.");
            }
            ExpandoObject target = (ExpandoObject)obj.Clone();
            DeleteProperty(target,property);
            return target;

        }
        public static void DeleteProperty(ExpandoObject obj, string property)
        {
            IDictionary<string, object> objDict = (IDictionary<string, object>)obj;
            objDict.Remove(property);
        }
    }
}
