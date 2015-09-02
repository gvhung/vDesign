using Framework.Attributes;
using Framework.Morphology;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.FullTextSearch
{
    public static class FullTextSearchExtensions
    {
        private const string KEY_FULL_TEXT_SEARCH = "{932DB32F-30C2-425E-B0C6-13697FE3C1E9}";

        private static readonly object _CacheLock = new object();

        public static string RemoveSpecialCharacters(this string str)
        {
            return str.Replace("'", @"\'").Replace("\"", @"\'");

            // падает если "'''..,l;'\\./.;.;.'/.; 
            //return Regex.Replace(str, "[^a-zA-Z0-9a-яА-Я]+", " ", RegexOptions.Compiled);
        }

        public static Dictionary<string, PropertyInfo> WrapQuery(Dictionary<string, PropertyInfo> props)
        {
            Dictionary<string, PropertyInfo> strings = new Dictionary<string, PropertyInfo>();

            Regex regex = new Regex(Regex.Escape("[]"));

            foreach (KeyValuePair<string, PropertyInfo> prop in props.Where(x => x.Value.PropertyType == typeof(string)))
            {
                string query = null;

                if (prop.Key.Contains("[]"))
                {
                    string propPath = prop.Key.Replace("[].", "[]");

                    string propName = propPath;

                    string[] arr = propName.Split("[]".ToCharArray());

                    if (arr.Length > 0)
                    {
                        propName = arr[arr.Length - 1];
                    }

                    query = propPath + " != null && " + propName + ".ToUpper().Contains(\"{0}\".ToUpper())";

                    while (query.Contains("[]"))
                    {
                        query = regex.Replace(query, ".Where(", 1);
                        query += ").Any()";
                    }
                }
                else
                {
                    query = prop.Key + " != null && " + prop.Key + ".ToUpper().Contains(\"{0}\".ToUpper())";
                }

                strings.Add(query, prop.Value);
            }

            return strings;
        }

        public static Dictionary<string, PropertyInfo> GetAttributedProperties(Type type, Dictionary<string, PropertyInfo> props = null, PropertyInfo currentProp = null, bool isCollection = false, int depth = 1, string path = "")
        {
            if (props == null)
            {
                props = new Dictionary<string, PropertyInfo>();
            }

            if (depth >= 0)
            {
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    FullTextSearchPropertyAttribute attr = prop.GetCustomAttribute<FullTextSearchPropertyAttribute>();

                    string newPath = path;

                    if (attr != null)
                    {
                        Type collectionType = prop.PropertyType.GetInterface("IEnumerable`1");

                        if (currentProp != null)
                        {
                            if (collectionType != null && prop.PropertyType != typeof(String))
                            {
                                newPath = newPath + "." + prop.Name + "[]";
                            }
                            else
                            {
                                newPath = newPath + "." + prop.Name;
                            }
                        }
                        else
                        {
                            if (collectionType != null && prop.PropertyType != typeof(String))
                            {
                                newPath = prop.Name + "[]";
                            }
                            else
                            {
                                newPath = prop.Name;
                            }
                        }

                        if (!props.ContainsKey(newPath))
                        {
                            props.Add(newPath, prop);

                            if (prop.PropertyType != typeof(String) && collectionType != null)
                            {
                                Type collectionEntryType = collectionType.GetGenericArguments()[0];

                                GetAttributedProperties(collectionEntryType, props, prop, true, depth: attr.Depth - 1, path: newPath);
                            }
                            else
                            {
                                GetAttributedProperties(prop.PropertyType, props, prop, depth: attr.Depth - 1, path: newPath);
                            }
                        }
                    }
                }
            }

            return props;
        }

        public static IQueryable<T> FullTextSearch<T>(this IQueryable<T> query, string searchStr, ICacheWrapper cache) where T : class
        {
            if (String.IsNullOrEmpty(searchStr) || String.IsNullOrWhiteSpace(searchStr))
            {
                return query;
            }

            string[] arrSearch = MorphologyHelper.SearchString(searchStr.RemoveSpecialCharacters()).Take(3).ToArray();

            if (arrSearch.Length > 0)
            {
                var type = query.GetType().GetGenericArguments()[0];
                
                var props = new Dictionary<string, PropertyInfo>();

                lock (_CacheLock)
                {
                    if (cache[KEY_FULL_TEXT_SEARCH] == null)
                    {
                        cache[KEY_FULL_TEXT_SEARCH] = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
                    }

                    var dic = cache[KEY_FULL_TEXT_SEARCH] as Dictionary<Type, Dictionary<string, PropertyInfo>>;

                    if (!dic.ContainsKey(type))
                    {
                        dic.Add(type, WrapQuery(GetAttributedProperties(type)));
                    }

                    props = dic[type];
                }

                if (props.Count != 0)
                {
                    var criteria = new StringBuilder();

                    for (int i = 0; i < arrSearch.Length; i++)
                    {
                        criteria.Append("(");

                        foreach (KeyValuePair<string, PropertyInfo> prop in props)
                        {
                            criteria.Append(String.Format(prop.Key + " or ", arrSearch[i]));
                        }

                        criteria.Remove(criteria.Length - 4, 4);

                        criteria.Append(") and ");
                    }

                    criteria.Remove(criteria.Length - 5, 5);

                    return query.Where(criteria.ToString());
                }

            }

            return query.Where(x => false);
        }

        //private static Expression<Func<T, bool>> BuildLambda<T>(Type type, string str) where T : class
        //{
        //    Dictionary<PropertyInfo, string> props = WrapQuery(GetAttributedProperties(type));

        //    var innerItem = Expression.Parameter(type, "f");

        //    var innerProperty = Expression.Property(innerItem, "Content");

        //    MethodInfo innerMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        //    var innerSearchExpression = Expression.Constant(str, typeof(string));

        //    var innerMethodExpression = Expression.Call(innerProperty, innerMethod, new[] { innerSearchExpression });

        //    Expression<Func<T, bool>> innerLambda = Expression.Lambda<Func<T, bool>>(innerMethodExpression, innerItem);

        //    return null;
        //}
    }
}
