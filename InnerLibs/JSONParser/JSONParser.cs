
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace InnerLibs
{
    /// <summary>
    /// Really simple JSON parser/writer
    /// </summary>
    /// <remarks>
    /// <listheader>Features and Limitations:</listheader>
    /// <list type="bullet">
    /// <item>Attempts to parse JSON files with minimal GC allocation</item>
    /// <item>
    /// Nice and simple <see cref="FromJson(string)"/> and <see cref="ToJson(object, bool,
    /// CultureInfo)"/> API
    /// </item>
    /// <item>Classes and structs can be parsed too!</item>
    /// <item>Can parse JSON without type information into <see cref="Dictionary{string, object}"/></item>
    /// <item>No JIT Emit support to support AOT compilation on iOS</item>
    /// <item>No JIT Emit support to parse structures quickly</item>
    /// <item>
    /// Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns
    /// null instead.
    /// </item>
    /// <item>Only public fields and property setters on classes/structs will be written to</item>
    /// <item>Circular references will throw a <see cref="StackOverflowException"/></item>
    /// <item>Limited to parsing 2GB JSON files (due to <see cref="int.MaxValue"/>)</item>
    /// <item>
    /// Parsing of abstract classes or interfaces is NOT supported and will throw an <see cref="Exception"/>.
    /// </item>
    /// <item>Outputs JSON structures from an object</item>
    /// <item>Will only output public fields and property getters on objects</item>
    /// </list>
    /// </remarks>
    public static class JSONParser
    {
        #region Private Fields

        [ThreadStatic] private static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache;
        [ThreadStatic] private static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache;
        [ThreadStatic] private static Stack<List<string>> splitArrayPool;
        [ThreadStatic] private static StringBuilder stringBuilder;

        #endregion Private Fields

        #region Internal Methods

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

        internal static void AppendValue(StringBuilder stringBuilder, object item, bool IncludeNull, CultureInfo culture)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetTypeOf();

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
                stringBuilder.Append(item.ToInt().ToString(culture));
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
                    AppendValue(stringBuilder, dict[key], IncludeNull, culture);
                }
                stringBuilder.Append('}');
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
                    AppendValue(stringBuilder, list[i], IncludeNull, culture);
                }
                stringBuilder.Append(']');
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
                        stringBuilder.Append(Ext.GetMemberName(fieldInfo));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value, IncludeNull, culture);
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
                        stringBuilder.Append(Ext.GetMemberName(propertyInfo));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value, IncludeNull, culture);
                    }
                }

                stringBuilder.Append('}');
            }
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

        internal static object ParseAnonymousValue(string json, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            if (json.Length == 0)
                return null;
            if (json.IsWrapped('{'))
            {
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0) elems.Add(null); //or return null?

                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (int i = 0; i < elems.Count; i += 2)
                    dict.SetOrRemove(elems[i].Substring(1, elems[i].Length - 2), ParseAnonymousValue(elems[i + 1], culture));
                return dict;
            }
            if (json.IsWrapped('['))
            {
                return Split(json).Select(x => ParseAnonymousValue(x, culture)).ToList();
            }
            if (json.IsWrapped('"'))
            {
                return json.Substring(1, json.Length - 2).Replace("\\", string.Empty);
            }
            if (char.IsDigit(json.FirstOrDefault()) || json.FirstOrDefault() == '-')
            {
                if (json.Contains(culture.NumberFormat.NumberDecimalSeparator))
                {
                    double.TryParse(json, NumberStyles.Float, culture, out double result);
                    return result;
                }
                else
                {
                    _ = int.TryParse(json, out int result);
                    return result;
                }
            }
            if (json.IsAny("true", "false")) return json.AsBool();

            return null;
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
                    if (propertyInfo.CanWrite)
                        propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value, culture), null);
            }

            return instance;
        }

        internal static object ParseValue(Type type, string json, CultureInfo culture)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            json = json ?? "null";

            if (json.Equals("null", StringComparison.OrdinalIgnoreCase)) return default;
            else if (type == typeof(string))
            {
                if (json.IsBlank() || json.IsNumber() || json.IsAny(StringComparison.InvariantCultureIgnoreCase, "true", "false")) return json;

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
            else if (type.IsPrimitive)
            {
                var result = Convert.ChangeType(json, type, culture);
                return result;
            }
            else if (type == typeof(decimal))
            {
                decimal.TryParse(json, NumberStyles.Float, culture, out decimal result);
                return result;
            }
            else if (type == typeof(DateTime))
            {
                DateTime.TryParse(json.RemoveAny(Ext.DoubleQuoteChar), culture, DateTimeStyles.None, out DateTime result);
                return result;
            }
            else if (type.IsEnum)
            {
                json = json.IsWrapped(Ext.DoubleQuoteChar) ? json.UnWrap(Ext.DoubleQuoteChar) : json;

                return json.GetEnumValue(type);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type underlyingType = type.GetGenericArguments().FirstOrDefault();
                return ParseValue(underlyingType, json, culture);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
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
                if (!json.IsWrapped('{'))
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    elems.Add(null);

                var dictionary = (IDictionary)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count / 2 });
                for (int i = 0; i < elems.Count; i += 2)
                {
                    if (elems[i].Length <= 2) continue;
                    string keyValue = elems[i].Substring(1, elems[i].Length - 2);
                    object val = ParseValue(valueType, elems[i + 1], culture);
                    dictionary[keyValue] = val;
                }
                return dictionary;
            }
            else if (type.IsEnumerable())
            {
                object[] elems = null;
                if (!json.IsWrapped('[')) json = json.Quote('[');
                Type listType = null;
                if (type.IsArray)
                {
                    listType = type.GetElementType();
                }
                else
                {
                    listType = type.GetGenericArguments().FirstOrDefault();
                }

                elems = Split(json).Select(x => ParseValue(listType, x, culture)).ToArray();

                var arr = Array.CreateInstance(listType, elems.Length);

                elems.CopyTo(arr, 0);

                if (type.IsArray) return arr;

                Type concreteListType = typeof(List<>).MakeGenericType(listType);

                return Activator.CreateInstance(concreteListType, new object[] { arr });
            }
            else if (type == typeof(object))
            {
                return ParseAnonymousValue(json, culture);
            }
            else if (json.IsWrapped('{'))
            {
                return ParseObject(type, json, culture);
            }

            return null;
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

        #endregion Internal Methods

        #region Public Methods

        /// <summary>
        /// Format a json string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="identSize"></param>
        /// <returns></returns>
        public static string FormatJson(this string jsonString, int identSize = 4)
        {
            jsonString = jsonString ?? Ext.EmptyString;

            if (identSize < 1)
            {
                return jsonString;
            }

            string INDENT_STRING = " ".Repeat(identSize);
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < jsonString.Length; i++)
            {
                var ch = jsonString[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).Each(item => sb.Append(INDENT_STRING));
                        }
                        break;

                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).Each(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;

                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && jsonString[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;

                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).Each(item => sb.Append(INDENT_STRING));
                        }
                        break;

                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(Ext.WhitespaceChar);
                        break;

                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
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

        /// <summary>
        /// Return a new <see cref="JsonFile"/> with latest saved values of the <paramref
        /// name="current"/><see cref="JsonFile"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T Load<T>(this T current) where T : JsonFile => current != null ? JsonFile.Load<T>(current.FilePath, current.EncryptKey, current.ExcludeNull, current.Culture) : null;

        public static string ToJson(this object item, bool IncludeNull = true, CultureInfo culture = null, int formatJsonIdent = 0)
        {
            if (item == null)
            {
                return null;
            }

            culture = culture ?? CultureInfo.InvariantCulture;
            StringBuilder stringBuilder = new StringBuilder();

            AppendValue(stringBuilder, item, IncludeNull, culture);
            return stringBuilder.ToString().FormatJson(formatJsonIdent);
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Class that when inherited by a POCO class implements methods to save the values in an
    /// encrypted Json file
    /// </summary>
    /// <remarks>Your POCO class nedd a public, parameterless constructor</remarks>
    public abstract class JsonFile
    {
        #region Private Fields

        private int _ident = 4;
        private string filePath;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The <see cref="CultureInfo"/> used to serialize/deserialize json
        /// </summary>
        [IgnoreDataMember] public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// The default file name used to save JsonFiles
        /// </summary>
        /// <remarks>
        /// When <see cref="IsEncrypted"/>, assumes the <see cref="Assembly.Name"/> of current
        /// executing application, otherwise use the default ".json" extension
        /// </remarks>
        [IgnoreDataMember] public string DefaultFileName => $"{this.GetType().Name}.{(IsEncrypted ? this.GetType().Assembly.GetName().Name.ToLower(Culture) : "json")}";

        /// <summary>
        /// The default path used to save JsonFiles
        /// </summary>
        /// <remarks>Uses <see cref="Environment.CurrentDirectory"/> and <see cref="DefaultFileName"/></remarks>
        [IgnoreDataMember] public string DefaultFilePath => $"{Environment.CurrentDirectory}\\{DefaultFileName}";

        /// <summary>
        /// When not blank, encrypt the json file using this string as Key
        /// </summary>
        [IgnoreDataMember] public string EncryptKey { get; set; }

        /// <summary>
        /// When true, exclude properties and fields with null values from serialization
        /// </summary>
        [IgnoreDataMember] public bool ExcludeNull { get; set; }

        /// <summary>
        /// <see cref="FileInfo"/> of current JsonFile
        /// </summary>
        /// <returns></returns>
        [IgnoreDataMember] public FileInfo File => new FileInfo(FilePath);

        /// <summary>
        /// The current Path of this JsonFile
        /// </summary>
        [IgnoreDataMember]
        public string FilePath
        {
            get
            {
                filePath = filePath.NullIf(x => !x.IsFilePath() && !x.IsDirectoryPath()).BlankCoalesce(filePath, DefaultFilePath).FixPath();
                if (filePath.IsDirectoryPath())
                {
                    filePath = Path.Combine(filePath, DefaultFileName);
                }

                return filePath;
            }
            set => filePath = value;
        }

        /// <summary>
        /// Return the number of spaces used to ident the JsonFile
        /// </summary>
        /// <remarks>When <see cref="IsEncrypted"/> is true, always return 0.</remarks>
        [IgnoreDataMember]
        public int IdentSize
        {
            get => IsEncrypted ? 0 : _ident;
            set => _ident = value.SetMinValue(0);
        }

        /// <summary>
        /// Return if this JsonFile will be encrypted using <see cref="EncryptKey"/>
        /// </summary>
        [IgnoreDataMember] public bool IsEncrypted => EncryptKey.IsNotBlank();

        /// <summary>
        /// When true, the final json string will be minified
        /// </summary>
        [IgnoreDataMember]
        public bool IsMinified
        {
            get => IdentSize <= 0;
            set
            {
                if (value)
                    IdentSize = 0;
                else if (_ident > 0)
                    IdentSize = _ident;
                else
                    IdentSize = 4;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Load values of a JsonFile into a <typeparamref name="T"/> object
        /// </summary>
        /// <typeparam name="T">Object T</typeparam>
        /// <param name="File">File Path</param>
        /// <param name="EncryptKey">Encrypt Key. Leave Null or blank to not encrypt</param>
        /// <param name="ExcludeNull">When true, exclude properties with null values from serialization</param>
        /// <returns></returns>
        public static T Load<T>(FileInfo File, string EncryptKey, bool ExcludeNull = false) where T : JsonFile => Load<T>(File?.FullName, EncryptKey, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>
        public static T Load<T>(DirectoryInfo Directory, string EncryptKey, bool ExcludeNull = false) where T : JsonFile => Load<T>(Directory?.FullName, EncryptKey, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>
        public static T Load<T>(FileInfo File, bool ExcludeNull = false) where T : JsonFile => Load<T>(File, null, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>
        public static T Load<T>(DirectoryInfo Directory, bool ExcludeNull = false) where T : JsonFile => Load<T>(Directory, null, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>
        public static T Load<T>(string FileOrDirectoryPath, bool ExcludeNull = false) where T : JsonFile => Load<T>(FileOrDirectoryPath, null, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>
        public static T Load<T>() where T : JsonFile => Load<T>(Ext.EmptyString, null);

        /// <inheritdoc cref="Load{T}"/>
        public static T Load<T>(string FileOrDirectoryPath, string EncryptKey, bool ExcludeNull = false, CultureInfo culture = null) where T : JsonFile
        {
            try
            {
                T c = Activator.CreateInstance<T>();
                c.FilePath = FileOrDirectoryPath;
                if (c.File.Exists)
                {
                    string s = c.File.ReadAllText();
                    if (EncryptKey.IsNotBlank())
                    {
                        s = s.Decrypt(EncryptKey);
                    }

                    c = s.FromJson<T>(culture);
                }

                c.FilePath = FileOrDirectoryPath; //need to re-set after deserialize
                c.EncryptKey = EncryptKey;
                c.ExcludeNull = ExcludeNull;
                c.Culture = culture ?? CultureInfo.InvariantCulture;
                return c;
            }
            catch (Exception ex)
            {
                throw new NotSupportedException("Your POCO class need a public, parameterless contructor", ex);
            }
        }

        /// <summary>
        /// Delete the file and return TRUE if file can be re-created
        /// </summary>
        /// <returns></returns>
        public bool Delete() => this.File.DeleteIfExist();

        /// <summary>
        /// Get the Json String representation of this file.
        /// </summary>
        /// <returns></returns>
        public string GetJson() => this.ToJson(!ExcludeNull, Culture, IdentSize);

        /// <summary>
        /// Save the current values into a JsonFile
        /// </summary>
        /// <returns></returns>
        public FileInfo Save()
        {
            Culture = Culture ?? CultureInfo.InvariantCulture;
            var s = GetJson();
            if (IsEncrypted)
            {
                s = s.Encrypt(this.EncryptKey);
            }

            return s.WriteToFile(this.FilePath);
        }

        #endregion Public Methods
    }
}