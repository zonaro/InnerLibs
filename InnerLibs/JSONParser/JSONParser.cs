using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace InnerLibs
{

    /// <summary>
    /// Class that when inherited by a POCO class implements methods to save the values in an encrypted Json file
    /// </summary>
    public abstract class JsonFile
    {
        private string file_path;
        private string encrypt_key;
        private bool exclude_null;
        private CultureInfo culture;

        /// <summary>
        /// Return a <see cref="FileInfo"/> of current JsonFile
        /// </summary>
        /// <returns></returns>
        public FileInfo GetFile() => new FileInfo(file_path.BlankCoalesce(DefaultFileName));

        /// <summary>
        /// The default path used to save JsonFiles
        /// </summary>
        public static string DefaultFileName => $"{Environment.CurrentDirectory}\\config.json";

        /// <summary>
        /// Load values of a JsonFile into a <typeparamref name="T"/> object
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="FilePath">File Path</param>
        /// <param name="EncryptKey">Encrypt Key. Leave Null or blank to not encrypt</param>
        /// <param name="ExcludeNull">When true, exclude properties with null values from serialization</param>
        /// <returns></returns>
        public static T Load<T>(FileInfo FilePath, string EncryptKey, bool ExcludeNull = false) where T : JsonFile => Load<T>(FilePath?.FullName, EncryptKey, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>(FileInfo FilePath) where T : JsonFile => Load<T>(FilePath, null);
        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>(string FilePath) where T : JsonFile => Load<T>(FilePath, null);
        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>() where T : JsonFile => Load<T>(DefaultFileName, null);
        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>(string FilePath, string EncryptKey, bool ExcludeNull = false, CultureInfo culture = null) where T : JsonFile
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            FilePath = FilePath.IfBlank(DefaultFileName).FixPath();
            T c = Activator.CreateInstance<T>();

            if (File.Exists(c.file_path))
            {
                string s = File.ReadAllText(c.file_path);
                if (EncryptKey.IsNotBlank())
                {
                    s = s.Decrypt(EncryptKey);
                }

                c = s.FromJson<T>(culture);
            }

            c.SetFilePath(FilePath);
            c.SetEncryptKey(EncryptKey);
            c.ExcludeNullValues(ExcludeNull);
            c.SetCulture(culture);
            c.Save();
            return c;
        }



        /// <summary>
        /// Save the current values into a JsonFile
        /// </summary>
        /// <returns></returns>     
        public FileInfo Save()
        {
            this.file_path = this.file_path.BlankCoalesce(DefaultFileName).FixPath();
            var s = this.ToJson(!exclude_null, culture);
            if (this.encrypt_key.IsNotBlank())
            {
                s = s.Encrypt(this.encrypt_key);
            }

            s.WriteToFile(this.file_path);
            return new FileInfo(this.file_path);
        }

        /// <summary>
        /// Set the <see cref="CultureInfo"/> used during serialization
        /// </summary>
        /// <param name="culture"></param>
        public void SetCulture(CultureInfo culture) => this.culture = culture;

        /// <summary>
        /// Set the Encrypt key using to encrypt the content of JsonFile after serialization
        /// </summary>
        /// <param name="EncryptKey"></param>
        public void SetEncryptKey(string EncryptKey) => this.encrypt_key = EncryptKey.NullIf(x => x.IsBlank());

        /// <summary>
        /// Change the filepath used to <see cref="Save"/>
        /// </summary>
        /// <param name="FilePath"></param>
        public void SetFilePath(string FilePath) => this.file_path = FilePath.BlankCoalesce(this.file_path, DefaultFileName);

        /// <summary>
        /// When true, exclude properties with null values from serialization
        /// </summary>
        /// <param name="Exclude"></param>
        public void ExcludeNullValues(bool Exclude) => this.exclude_null = Exclude;


    }

    // Really simple JSON parser/writer
    // - Attempts to parse JSON files with minimal GC allocation
    // - Nice and simple "[1,2,3]".FromJson<List<int>>() API
    // - Classes and structs can be parsed too!
    //      class Foo { public int Value; }
    //      "{\"Value\":10}".FromJson<Foo>()
    // - Can parse JSON without type information into Dictionary<string,object> and List<object> e.g.
    //      "[1,2,3]".FromJson<object>().GetType() == typeof(List<object>)
    //      "{\"Value\":10}".FromJson<object>().GetType() == typeof(Dictionary<string,object>)
    // - No JIT Emit support to support AOT compilation on iOS
    // - Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns null instead.
    // - Only public fields and property setters on classes/structs will be written to
    //
    // Limitations:
    // - No JIT Emit support to parse structures quickly
    // - Limited to parsing <2GB JSON files (due to int.MaxValue)
    // - Parsing of abstract classes or interfaces is NOT supported and will throw an exception.
    // 
    // - Outputs JSON structures from an object
    // - Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
    // - Will only output public fields and property getters on objects
    public static class JSONParser
    {
        [ThreadStatic] static Stack<List<string>> splitArrayPool;
        [ThreadStatic] static StringBuilder stringBuilder;
        [ThreadStatic] static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache;
        [ThreadStatic] static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache;


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
                        stringBuilder.Append(Misc.GetMemberName(fieldInfo));
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
                        stringBuilder.Append(Misc.GetMemberName(propertyInfo));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }


                }

                stringBuilder.Append('}');
            }
        }



        public static string ToJson(this object item, bool IncludeNull = true, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            StringBuilder stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item, IncludeNull, culture);
            return stringBuilder.ToString();
        }
        public static T FromJson<T>(this string json) => FromJson<T>(json, null);
        public static object FromJson(this string json) => FromJson(json, null);
        public static T FromJson<T>(this string json, CultureInfo culture) => (T)FromJson(json, typeof(T), culture);
        public static object FromJson(this string json, CultureInfo culture) => FromJson(json, typeof(object), culture);
        public static object FromJson(this string json, Type Type, CultureInfo culture)
        {
            if (json == null) return default;
            culture = culture ?? CultureInfo.InvariantCulture;
            Type = Type ?? typeof(object);
            // Initialize, if needed, the ThreadStatic variables
            if (propertyInfoCache == null) propertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            if (fieldInfoCache == null) fieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            if (stringBuilder == null) stringBuilder = new StringBuilder();
            if (splitArrayPool == null) splitArrayPool = new Stack<List<string>>();

            //Remove all whitespace not within strings to make parsing simpler
            stringBuilder.Length = 0;
            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '"')
                {
                    i = AppendUntilStringEnd(true, i, json);
                    continue;
                }
                if (char.IsWhiteSpace(c))
                    continue;

                stringBuilder.Append(c);
            }

            //Parse the thing!
            return ParseValue(Type, stringBuilder.ToString(), culture);
        }

        internal static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
        {
            stringBuilder.Append(json[startIdx]);
            for (int i = startIdx + 1; i < json.Length; i++)
            {
                if (json[i] == '\\')
                {
                    if (appendEscapeCharacter)
                        stringBuilder.Append(json[i]);
                    stringBuilder.Append(json[i + 1]);
                    i++;//Skip next character as it is escaped
                }
                else if (json[i] == '"')
                {
                    stringBuilder.Append(json[i]);
                    return i;
                }
                else
                    stringBuilder.Append(json[i]);
            }
            return json.Length - 1;
        }

        //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
        internal static List<string> Split(string json)
        {
            List<string> splitArray = splitArrayPool.Count > 0 ? splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if (json.Length == 2)
                return splitArray;
            int parseDepth = 0;
            stringBuilder.Length = 0;
            for (int i = 1; i < json.Length - 1; i++)
            {
                switch (json[i])
                {
                    case '[':
                    case '{':
                        parseDepth++;
                        break;
                    case ']':
                    case '}':
                        parseDepth--;
                        break;
                    case '"':
                        i = AppendUntilStringEnd(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0)
                        {
                            splitArray.Add(stringBuilder.ToString());
                            stringBuilder.Length = 0;
                            continue;
                        }
                        break;
                }

                stringBuilder.Append(json[i]);
            }

            splitArray.Add(stringBuilder.ToString());

            return splitArray;
        }

        internal static object ParseValue(Type type, string json, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.InvariantCulture;

            if (json == "null")
            {
                return null;
            }

            if (type == typeof(string))
            {
                if (json.Length <= 2)
                    return string.Empty;
                StringBuilder parseStringBuilder = new StringBuilder(json.Length);
                for (int i = 1; i < json.Length - 1; ++i)
                {
                    if (json[i] == '\\' && i + 1 < json.Length - 1)
                    {
                        int j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                        if (j >= 0)
                        {
                            parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                            ++i;
                            continue;
                        }
                        if (json[i + 1] == 'u' && i + 5 < json.Length - 1)
                        {
                            if (uint.TryParse(json.Substring(i + 2, 4), NumberStyles.AllowHexSpecifier, null, out uint c))
                            {
                                parseStringBuilder.Append((char)c);
                                i += 5;
                                continue;
                            }
                        }
                    }
                    parseStringBuilder.Append(json[i]);
                }
                return parseStringBuilder.ToString();
            }
            if (type.IsPrimitive)
            {
                var result = Convert.ChangeType(json, type, culture);
                return result;
            }
            if (type == typeof(decimal))
            {
                decimal.TryParse(json, NumberStyles.Float, culture, out decimal result);
                return result;
            }
            if (type == typeof(DateTime))
            {
                DateTime.TryParse(json.Replace("\"", ""), culture, DateTimeStyles.None, out DateTime result);
                return result;
            }

            if (type.IsEnum)
            {
                if (json[0] == '"')
                    json = json.Substring(1, json.Length - 2);
                try
                {
                    return Enum.Parse(type, json, false);
                }
                catch
                {
                    return 0;
                }
            }
            if (type.IsArray)
            {
                Type arrayType = type.GetElementType();
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                Array newArray = Array.CreateInstance(arrayType, elems.Count);
                for (int i = 0; i < elems.Count; i++)
                    newArray.SetValue(ParseValue(arrayType, elems[i], culture), i);
                splitArrayPool.Push(elems);
                return newArray;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type underlyingType = type.GetGenericArguments()[0];
                return ParseValue(underlyingType, json, culture);
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GetGenericArguments()[0];
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                var list = (IList)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count });
                for (int i = 0; i < elems.Count; i++)
                    list.Add(ParseValue(listType, elems[i], culture));
                splitArrayPool.Push(elems);
                return list;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType, valueType;
                {
                    Type[] args = type.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }

                //Refuse to parse dictionary keys that aren't of type string
                if (keyType != typeof(string))
                    return null;
                //Must be a valid dictionary element
                if (json[0] != '{' || json[json.Length - 1] != '}')
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;

                var dictionary = (IDictionary)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count / 2 });
                for (int i = 0; i < elems.Count; i += 2)
                {
                    if (elems[i].Length <= 2)
                        continue;
                    string keyValue = elems[i].Substring(1, elems[i].Length - 2);
                    object val = ParseValue(valueType, elems[i + 1], culture);
                    dictionary[keyValue] = val;
                }
                return dictionary;
            }
            if (type == typeof(object))
            {
                return ParseAnonymousValue(json, culture);
            }
            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                return ParseObject(type, json, culture);
            }

            return null;
        }

        internal static object ParseAnonymousValue(string json, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            if (json.Length == 0)
                return null;
            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (int i = 0; i < elems.Count; i += 2)
                    dict[elems[i].Substring(1, elems[i].Length - 2)] = ParseAnonymousValue(elems[i + 1], culture);
                return dict;
            }
            if (json[0] == '[' && json[json.Length - 1] == ']')
            {
                List<string> items = Split(json);
                var finalList = new List<object>(items.Count);
                for (int i = 0; i < items.Count; i++)
                    finalList.Add(ParseAnonymousValue(items[i], culture));
                return finalList;
            }
            if (json[0] == '"' && json[json.Length - 1] == '"')
            {
                string str = json.Substring(1, json.Length - 2);
                return str.Replace("\\", string.Empty);
            }
            if (char.IsDigit(json[0]) || json[0] == '-')
            {
                if (json.Contains("."))
                {
                    double.TryParse(json, NumberStyles.Float, culture, out double result);
                    return result;
                }
                else
                {
                    int.TryParse(json, out int result);
                    return result;
                }
            }
            if (json == "true")
                return true;
            if (json == "false")
                return false;
            // handles json == "null" as well as invalid JSON
            return null;
        }

        internal static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo
        {
            Dictionary<string, T> nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < members.Length; i++)
            {
                T member = members[i];
                if (member.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                string name = member.Name;
                if (member.IsDefined(typeof(DataMemberAttribute), true))
                {
                    DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        name = dataMemberAttribute.Name;
                }

                nameToMember.Add(name, member);
            }

            return nameToMember;
        }

        internal static object ParseObject(Type type, string json, CultureInfo culture)
        {
            object instance = FormatterServices.GetUninitializedObject(type);

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            List<string> elems = Split(json);
            if (elems.Count % 2 != 0)
                return instance;

            if (!fieldInfoCache.TryGetValue(type, out Dictionary<string, FieldInfo> nameToField))
            {
                nameToField = CreateMemberNameDictionary(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                fieldInfoCache.Add(type, nameToField);
            }
            if (!propertyInfoCache.TryGetValue(type, out Dictionary<string, PropertyInfo> nameToProperty))
            {
                nameToProperty = CreateMemberNameDictionary(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                propertyInfoCache.Add(type, nameToProperty);
            }

            for (int i = 0; i < elems.Count; i += 2)
            {
                if (elems[i].Length <= 2)
                    continue;
                string key = elems[i].Substring(1, elems[i].Length - 2);
                string value = elems[i + 1];

                if (nameToField.TryGetValue(key, out FieldInfo fieldInfo))
                    fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value, culture));
                else if (nameToProperty.TryGetValue(key, out PropertyInfo propertyInfo))
                    propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value, culture), null);
            }

            return instance;
        }
    }
}
