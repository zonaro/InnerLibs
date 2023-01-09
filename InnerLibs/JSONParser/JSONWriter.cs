using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace InnerLibs.JSONParser
{
    //Really simple JSON writer
    //- Outputs JSON structures from an object
    //- Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
    //- Will only output public fields and property getters on objects
    public static class JSONWriter
    {

        public static string ToJson(this object item, bool IncludeNull = true, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            StringBuilder stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item, IncludeNull, culture);
            return stringBuilder.ToString();
        }

        internal static void AppendValue(StringBuilder stringBuilder, object item, bool IncludeNull = true, CultureInfo culture = null)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetType();
            if (type == typeof(string) || type == typeof(char) || type == typeof(FormattableString))
            {
                stringBuilder.Append('"');
                string str = item.ToString();
                for (int i = 0; i < str.Length; ++i)
                    if (str[i] < ' ' || str[i] == '"' || str[i] == '\\')
                    {
                        stringBuilder.Append('\\');
                        int j = "\"\\\n\r\t\b\f".IndexOf(str[i]);
                        if (j >= 0)
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat(culture, "u{0:X4}", (uint)str[i]);
                    }
                    else stringBuilder.Append(str[i]);
                stringBuilder.Append('"');
            }
            else if (type == typeof(byte) || type == typeof(sbyte))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(short) || type == typeof(ushort))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(long) || type == typeof(ulong))
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type == typeof(float))
            {
                stringBuilder.Append(((float)item).ToString(culture));
            }
            else if (type == typeof(double))
            {
                stringBuilder.Append(((double)item).ToString(culture));
            }
            else if (type == typeof(decimal))
            {
                stringBuilder.Append(((decimal)item).ToString(culture));
            }
            else if (type == typeof(bool))
            {
                stringBuilder.Append(((bool)item) ? "true" : "false");
            }
            else if (type == typeof(DateTime))
            {
                stringBuilder.Append('"');
                stringBuilder.Append(((DateTime)item).ToString(culture));
                stringBuilder.Append('"');
            }
            else if (type.IsEnum)
            {
                stringBuilder.Append('"');
                stringBuilder.Append(item.ToString());
                stringBuilder.Append('"');
            }
            else if (item is IList)
            {
                stringBuilder.Append('[');
                bool isFirst = true;
                IList list = item as IList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, list[i]);
                }
                stringBuilder.Append(']');
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = type.GetGenericArguments()[0];

                //Refuse to output dictionary keys that aren't compatible json types
                if (keyType.IsNotIn(new[] { typeof(string), typeof(int), typeof(long), typeof(uint), typeof(ulong), typeof(char) }))
                {
                    stringBuilder.Append("{}");
                    return;
                }

                stringBuilder.Append('{');
                IDictionary dict = item as IDictionary;
                bool isFirst = true;
                foreach (object key in dict.Keys)
                {
                    if (isFirst) isFirst = false; else stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append($"{key}");
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, dict[key]);
                }
                stringBuilder.Append('}');
            }
            else
            {
                stringBuilder.Append('{');

                bool isFirst = true;
                foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
                {
                    if (fieldInfo.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    object value = fieldInfo.GetValue(item);
                    if (value != null || IncludeNull)
                    {
                        if (isFirst) isFirst = false; else stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(fieldInfo));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }

                foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
                {
                    if (!propertyInfo.CanRead || propertyInfo.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    object value = propertyInfo.GetValue(item, null);
                    if (value != null || IncludeNull)
                    {
                        if (isFirst) isFirst = false; else stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(propertyInfo));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }


                }

                stringBuilder.Append('}');
            }
        }

        internal static string GetMemberName(MemberInfo member)
        {
            if (member.IsDefined(typeof(DataMemberAttribute), true))
            {
                DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    return dataMemberAttribute.Name;
            }

            return member.Name;
        }
    }
}
