using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Data;

using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace Extensions
{
    internal class deserializer
    {
        #region Private Fields

        private Dictionary<object, int> _circobj;

        // = new Dictionary<object, int>();
        private Dictionary<int, object> _cirrev = new Dictionary<int, object>();

        private JSONParameters _params;

        private bool _usingglobals;

        #endregion Private Fields

        #region Private Methods

        private static void ProcessMap(object obj, Dictionary<string, myPropInfo> props, Dictionary<string, object> dic)
        {
            foreach (KeyValuePair<string, object> kv in dic)
            {
                myPropInfo p = props[kv.Key];
                object o = p.getter(obj);
                // blacklist checking
                Type t = //NodeType.GetType((string)kv.Value);
                         Reflection.Instance.GetTypeFromCache((string)kv.Value, true);
                if (t == typeof(Guid))
                    p.setter(obj, Helper.CreateGuid((string)o));
            }
        }

        private object ChangeType(object value, Type conversionType)
        {
            if (conversionType == typeof(int))
            {
                string s = value as string;
                if (s == null)
                    return (int)((long)value);
                else if (_params.AutoConvertStringToNumbers)
                    return Helper.CreateInteger(s, 0, s.Length);
                else
                    throw new Exception("AutoConvertStringToNumbers is disabled for converting string : " + value);
            }
            else if (conversionType == typeof(long))
            {
                string s = value as string;
                if (s == null)
                    return (long)value;
                else if (_params.AutoConvertStringToNumbers)
                    return Helper.CreateLong(s, 0, s.Length);
                else
                    throw new Exception("AutoConvertStringToNumbers is disabled for converting string : " + value);
            }
            else if (conversionType == typeof(string))
                return (string)value;
            else if (conversionType.IsEnum)
                return Helper.CreateEnum(conversionType, value);
            else if (conversionType == typeof(DateTime))
                return Helper.CreateDateTime((string)value, _params.UseUTCDateTime);
            else if (conversionType == typeof(DateTimeOffset))
                return Helper.CreateDateTimeOffset((string)value);
            else if (Reflection.Instance.IsTypeRegistered(conversionType))
                return Reflection.Instance.CreateCustom((string)value, conversionType);

            // 8-30-2014 - James Brooks - Added code for nullable types.
            if (Helper.IsNullable(conversionType))
            {
                if (value == null)
                    return value;
                conversionType = Helper.UnderlyingTypeOf(conversionType);
            }

            // 8-30-2014 - James Brooks - Nullable Guid is a special case so it was moved after the
            // "IsNullable" check.
            if (conversionType == typeof(Guid))
                return Helper.CreateGuid((string)value);

            // 2016-04-02 - Enrico Padovani - proper conversion of byte[] back from string
            if (conversionType == typeof(byte[]))
                return Convert.FromBase64String((string)value);

            if (conversionType == typeof(TimeSpan))
                return new TimeSpan((long)value);

            return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        private object CreateArray(List<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
            if (bt == null)
                bt = typeof(object);

            Array col = Array.CreateInstance(bt, data.Count);
            var arraytype = bt.GetElementType();
            // create an array of objects
            for (int i = 0; i < data.Count; i++)
            {
                object ob = data[i];
                if (ob == null)
                {
                    continue;
                }
                if (ob is IDictionary)
                    col.SetValue(ParseDictionary((Dictionary<string, object>)ob, globalTypes, bt, null), i);
                else if (ob is ICollection)
                    col.SetValue(CreateArray((List<object>)ob, bt, arraytype, globalTypes), i);
                else
                    col.SetValue(ChangeType(ob, bt), i);
            }

            return col;
        }

        private DataSet CreateDataset(Dictionary<string, object> reader, Dictionary<string, object> globalTypes)
        {
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            ds.BeginInit();

            // read dataset schema here
            var schema = reader["$schema"];

            if (schema is string)
            {
                TextReader tr = new StringReader((string)schema);
                ds.ReadXmlSchema(tr);
            }
            else
            {
                DataSetSchema ms = (DataSetSchema)ParseDictionary((Dictionary<string, object>)schema, globalTypes, typeof(DataSetSchema), null);
                ds.DataSetName = ms.Name;
                for (int i = 0; i < ms.Info.Count; i += 3)
                {
                    if (ds.Tables.Contains(ms.Info[i]) == false)
                        ds.Tables.Add(ms.Info[i]);
                    // blacklist checking
                    var t = //NodeType.GetType(ms.Info[i + 2]);
                            Reflection.Instance.GetTypeFromCache(ms.Info[i + 2], true);
                    ds.Tables[ms.Info[i]].Columns.Add(ms.Info[i + 1], t);
                }
            }

            foreach (KeyValuePair<string, object> pair in reader)
            {
                if (pair.Key == "$type" || pair.Key == "$schema") continue;

                List<object> rows = (List<object>)pair.Value;
                if (rows == null) continue;

                DataTable dt = ds.Tables[pair.Key];
                ReadDataTable(rows, dt);
            }

            ds.EndInit();

            return ds;
        }

        private DataTable CreateDataTable(Dictionary<string, object> reader, Dictionary<string, object> globalTypes)
        {
            var dt = new DataTable();

            // read dataset schema here
            var schema = reader["$schema"];

            if (schema is string)
            {
                TextReader tr = new StringReader((string)schema);
                dt.ReadXmlSchema(tr);
            }
            else
            {
                var ms = (DataSetSchema)ParseDictionary((Dictionary<string, object>)schema, globalTypes, typeof(DataSetSchema), null);
                dt.TableName = ms.Info[0];
                for (int i = 0; i < ms.Info.Count; i += 3)
                {
                    // blacklist checking
                    var t = //NodeType.GetType(ms.Info[i + 2]);
                            Reflection.Instance.GetTypeFromCache(ms.Info[i + 2], true);
                    dt.Columns.Add(ms.Info[i + 1], t);
                }
            }

            foreach (var pair in reader)
            {
                if (pair.Key == "$type" || pair.Key == "$schema")
                    continue;

                var rows = (List<object>)pair.Value;
                if (rows == null)
                    continue;

                if (!dt.TableName.Equals(pair.Key, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                ReadDataTable(rows, dt);
            }

            return dt;
        }

        private object CreateDictionary(List<object> reader, Type pt, Type[] types, Dictionary<string, object> globalTypes)
        {
            IDictionary col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type t1 = null;
            Type t2 = null;
            Type generictype = null;
            if (types != null)
            {
                t1 = types[0];
                t2 = types[1];
            }
            Type arraytype = t2;
            if (t2 != null)
            {
                var ga = Reflection.Instance.GetGenericArguments(t2);// t2.GetGenericArguments();
                if (ga.Length > 0)
                    generictype = ga[0];
                arraytype = t2.GetElementType();
            }
            bool root = typeof(IDictionary).IsAssignableFrom(t2);

            foreach (Dictionary<string, object> values in reader)
            {
                object key = values["k"];
                object val = values["v"];

                if (key is Dictionary<string, object>)
                    key = ParseDictionary((Dictionary<string, object>)key, globalTypes, t1, null);
                else
                    key = ChangeType(key, t1);

                if (root)
                    val = RootDictionary(val, t2);
                else if (val is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)val, globalTypes, t2, null);
                else if (types != null && t2.IsArray)
                    val = CreateArray((List<object>)val, t2, arraytype, globalTypes);
                else if (val is IList)
                    val = CreateGenericList((List<object>)val, t2, generictype, globalTypes);
                else
                    val = ChangeType(val, t2);

                col.Add(key, val);
            }

            return col;
        }

        private object CreateGenericList(List<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
            if (pt != typeof(object))
            {
                IList col = (IList)Reflection.Instance.FastCreateList(pt, data.Count);
                var it = Reflection.Instance.GetGenericArguments(pt)[0];// pt.GetGenericArguments()[0];
                // create an array of objects
                foreach (object ob in data)
                {
                    if (ob is IDictionary)
                        col.Add(ParseDictionary((Dictionary<string, object>)ob, globalTypes, it, null));
                    else if (ob is List<object>)
                    {
                        if (bt.IsGenericType)
                            col.Add((List<object>)ob);//).ToArray());
                        else
                            col.Add(((List<object>)ob).ToArray());
                    }
                    else
                        col.Add(ChangeType(ob, it));
                }
                return col;
            }
            return data;
        }

        private object CreateStringKeyDictionary(Dictionary<string, object> reader, Type pt, Type[] types, Dictionary<string, object> globalTypes)
        {
            var col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type t2 = null;
            if (types != null)
                t2 = types[1];

            Type generictype = null;
            var ga = Reflection.Instance.GetGenericArguments(t2);// t2.GetGenericArguments();
            if (ga.Length > 0)
                generictype = ga[0];
            Type arraytype = t2.GetElementType();

            foreach (KeyValuePair<string, object> values in reader)
            {
                var key = values.Key;
                object val;
                if (values.Value is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)values.Value, globalTypes, t2, null);
                else if (types != null && t2.IsArray)
                {
                    if (values.Value is Array)
                        val = values.Value;
                    else
                        val = CreateArray((List<object>)values.Value, t2, arraytype, globalTypes);
                }
                else if (values.Value is IList)
                    val = CreateGenericList((List<object>)values.Value, t2, generictype, globalTypes);
                else
                    val = ChangeType(values.Value, t2);

                col.Add(key, val);
            }

            return col;
        }

        private void DoParseList(IList parse, Type it, IList o)
        {
            Dictionary<string, object> globals = new Dictionary<string, object>();

            foreach (var k in parse)
            {
                _usingglobals = false;
                var a = k as Dictionary<string, object>;
                object v;
                if (a != null)
                    v = ParseDictionary(a, globals, it, null);
                else
                    v = ChangeType(k, it);

                o.Add(v);
            }
        }

        private void ReadDataTable(List<object> rows, DataTable dt)
        {
            dt.BeginInit();
            dt.BeginLoadData();
            List<int> guidcols = new List<int>();
            List<int> datecol = new List<int>();
            List<int> bytearraycol = new List<int>();

            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(Guid) || c.DataType == typeof(Guid?))
                    guidcols.Add(c.Ordinal);
                if (_params.UseUTCDateTime && (c.DataType == typeof(DateTime) || c.DataType == typeof(DateTime?)))
                    datecol.Add(c.Ordinal);
                if (c.DataType == typeof(byte[]))
                    bytearraycol.Add(c.Ordinal);
            }

            foreach (List<object> row in rows)
            {
                //object[] v = row.ToArray(); //new object[row.Count];
                //row.CopyTo(v, 0);
                var v = row;
                foreach (int i in guidcols)
                {
                    string s = (string)v[i];
                    if (s != null && s.Length < 36)
                        v[i] = new Guid(Convert.FromBase64String(s));
                }
                foreach (int i in bytearraycol)
                {
                    string s = (string)v[i];
                    if (s != null)
                        v[i] = Convert.FromBase64String(s);
                }
                if (_params.UseUTCDateTime)
                {
                    foreach (int i in datecol)
                    {
                        string s = (string)v[i];
                        if (s != null)
                            v[i] = Helper.CreateDateTime(s, _params.UseUTCDateTime);
                    }
                }
                dt.Rows.Add(v.ToArray());
            }

            dt.EndLoadData();
            dt.EndInit();
        }

        private object RootArray(object parse, Type type)
        {
            Type it = type.GetElementType();
            var o = (IList)Reflection.Instance.FastCreateInstance(typeof(List<>).MakeGenericType(it));
            DoParseList((IList)parse, it, o);
            var array = Array.CreateInstance(it, o.Count);
            o.CopyTo(array, 0);
            return array;
        }

        private object RootDictionary(object parse, Type type)
        {
            Type[] gtypes = Reflection.Instance.GetGenericArguments(type);
            Type t1 = null;
            Type t2 = null;
            bool dictionary = false;
            if (gtypes != null)
            {
                t1 = gtypes[0];
                t2 = gtypes[1];
                if (t2 != null)
                    dictionary = t2.Name.StartsWith("Dictionary");
            }

            var arraytype = t2.GetElementType();
            if (parse is Dictionary<string, object>)
            {
                IDictionary o = (IDictionary)Reflection.Instance.FastCreateInstance(type);

                foreach (var kv in (Dictionary<string, object>)parse)
                {
                    object v;
                    object k = ChangeType(kv.Key, t1);

                    if (dictionary) // deserialize a dictionary
                        v = RootDictionary(kv.Value, t2);
                    else if (kv.Value is Dictionary<string, object>)
                        v = ParseDictionary(kv.Value as Dictionary<string, object>, null, t2, null);
                    else if (t2.IsArray && t2 != typeof(byte[]))
                        v = CreateArray((List<object>)kv.Value, t2, arraytype, null);
                    else if (kv.Value is IList)
                        v = CreateGenericList((List<object>)kv.Value, t2, t1, null);
                    else
                        v = ChangeType(kv.Value, t2);

                    o.Add(k, v);
                }

                return o;
            }
            if (parse is List<object>)
                return CreateDictionary(parse as List<object>, type, gtypes, null);

            return null;
        }

        private object RootHashTable(List<object> o)
        {
            Hashtable h = new Hashtable();

            foreach (Dictionary<string, object> values in o)
            {
                object key = values["k"];
                object val = values["v"];
                if (key is Dictionary<string, object>)
                    key = ParseDictionary((Dictionary<string, object>)key, null, typeof(object), null);

                if (val is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)val, null, typeof(object), null);

                h.Add(key, val);
            }

            return h;
        }

        private object RootList(object parse, Type type)
        {
            Type[] gtypes = Reflection.Instance.GetGenericArguments(type);
            var o = (IList)Reflection.Instance.FastCreateList(type, ((IList)parse).Count);
            DoParseList((IList)parse, gtypes[0], o);
            return o;
        }

        #endregion Private Methods

        #region Internal Methods

        internal object ParseDictionary(Dictionary<string, object> d, Dictionary<string, object> globaltypes, Type type, object input)
        {
            if (type == typeof(NameValueCollection))
                return Helper.CreateNV(d);
            if (type == typeof(StringDictionary))
                return Helper.CreateSD(d);

            object tn;
            if (d.TryGetValue("$i", out tn))
            {
                object v;
                _cirrev.TryGetValue((int)(long)tn, out v);
                return v;
            }

            if (d.TryGetValue("$types", out tn))
            {
                _usingglobals = true;
                if (globaltypes == null)
                    globaltypes = new Dictionary<string, object>();
                foreach (var kv in (Dictionary<string, object>)tn)
                {
                    globaltypes.Add((string)kv.Value, kv.Key);
                }
            }
            if (globaltypes != null)
                _usingglobals = true;

            bool found = d.TryGetValue("$type", out tn);

            if (found == false && type == typeof(System.Object))
            {
                return d;   // CreateDataset(d, globaltypes);
            }

            if (found)
            {
                if (_usingglobals)
                {
                    object tname;
                    if (globaltypes != null && globaltypes.TryGetValue((string)tn, out tname))
                        tn = tname;
                }
                type = Reflection.Instance.GetTypeFromCache((string)tn, _params.BadListTypeChecking);
            }

            if (type == null)
                throw new Exception("Cannot determine type : " + tn);

            string typename = type.FullName;
            object o = input;
            if (o == null)
            {
                if (_params.ParametricConstructorOverride)
                    o = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
                else
                    o = Reflection.Instance.FastCreateInstance(type);
            }
            int circount;
            if (_circobj.TryGetValue(o, out circount) == false)
            {
                circount = _circobj.Count + 1;
                _circobj.Add(o, circount);
                _cirrev.Add(circount, o);
            }

            var props = Reflection.Instance.Getproperties(type, typename, _params.ShowReadOnlyProperties);
            foreach (var kv in d)
            {
                var n = kv.Key;
                var v = kv.Value;

                string name = n;//.ToLower();
                if (name == "$map")
                {
                    ProcessMap(o, props, (Dictionary<string, object>)d[name]);
                    continue;
                }
                myPropInfo pi;
                if (props.TryGetValue(name, out pi) == false)
                    if (props.TryGetValue(name.ToLowerInvariant(), out pi) == false)
                        continue;

                if (pi.CanWrite)
                {
                    object oset = null;
                    if (v != null)
                    {
                        switch (pi.Type)
                        {
                            case myPropInfoType.Int: oset = Helper.AutoConv(v, _params).ToInt(); break;
                            case myPropInfoType.Long: oset = Helper.AutoConv(v, _params); break;
                            case myPropInfoType.String: oset = v.ToString(); break;
                            case myPropInfoType.Bool: oset = $"{v}".AsBool(); break;
                            case myPropInfoType.DateTime: oset = Helper.CreateDateTime((string)v, _params.UseUTCDateTime); break;
                            case myPropInfoType.Enum: oset = Helper.CreateEnum(pi.pt, v); break;
                            case myPropInfoType.Guid: oset = Helper.CreateGuid((string)v); break;

                            case myPropInfoType.Array:
                                if (!pi.IsValueType)
                                    oset = CreateArray((List<object>)v, pi.pt, pi.bt, globaltypes);
                                // what about 'else'?
                                break;

                            case myPropInfoType.ByteArray: oset = Convert.FromBase64String((string)v); break;

                            case myPropInfoType.DataSet: oset = CreateDataset((Dictionary<string, object>)v, globaltypes); break;
                            case myPropInfoType.DataTable: oset = CreateDataTable((Dictionary<string, object>)v, globaltypes); break;
                            case myPropInfoType.Hashtable: // same case as Dictionary

                            case myPropInfoType.Dictionary: oset = CreateDictionary((List<object>)v, pi.pt, pi.GenericTypes, globaltypes); break;
                            case myPropInfoType.StringKeyDictionary: oset = CreateStringKeyDictionary((Dictionary<string, object>)v, pi.pt, pi.GenericTypes, globaltypes); break;
                            case myPropInfoType.NameValue: oset = Helper.CreateNV((Dictionary<string, object>)v); break;
                            case myPropInfoType.StringDictionary: oset = Helper.CreateSD((Dictionary<string, object>)v); break;
                            case myPropInfoType.Custom: oset = Reflection.Instance.CreateCustom((string)v, pi.pt); break;
                            default:
                                {
                                    if (pi.IsGenericType && pi.IsValueType == false && v is List<object>)
                                        oset = CreateGenericList((List<object>)v, pi.pt, pi.bt, globaltypes);
                                    else if ((pi.IsClass || pi.IsStruct || pi.IsInterface) && v is Dictionary<string, object>)
                                        oset = ParseDictionary((Dictionary<string, object>)v, globaltypes, pi.pt, null);// pi.getter(o));
                                    else if (v is List<object>)
                                        oset = CreateArray((List<object>)v, pi.pt, typeof(object), globaltypes);
                                    else if (pi.IsValueType)
                                        oset = ChangeType(v, pi.changeType);
                                    else
                                        oset = v;
                                }
                                break;
                        }
                    }
                    o = pi.setter(o, oset);
                }
            }
            return o;
        }

        #endregion Internal Methods

        #region Public Constructors

        public deserializer(JSONParameters param)
        {
            if (param.OverrideObjectHashCodeChecking)
                _circobj = new Dictionary<object, int>(10, ReferenceEqualityComparer.Default);
            else
                _circobj = new Dictionary<object, int>();

            param.FixValues();
            _params = param.MakeCopy();
        }

        #endregion Public Constructors

        #region Public Methods

        public T ToObject<T>(string json)
        {
            Type t = typeof(T);
            var o = ToObject(json, t);

            if (t.IsArray)
            {
                if ((o as ICollection).Count == 0) // edge case for "[]" -> T[]
                {
                    Type tt = t.GetElementType();
                    object oo = Array.CreateInstance(tt, 0);
                    return (T)oo;
                }
                else
                    return (T)o;
            }
            else
                return (T)o;
        }

        public object ToObject(string json) => ToObject(json, null);

        public object ToObject(string json, Type type)
        {
            //_params.FixValues();
            Type t = null;
            if (type != null && type.IsGenericType)
                t = Reflection.Instance.GetGenericTypeDefinition(type);
            _usingglobals = _params.UsingGlobalTypes;
            if (typeof(IDictionary).IsAssignableFrom(t) || typeof(List<>).IsAssignableFrom(t))
                _usingglobals = false;

            object o = new JsonParser(json, _params.AllowNonQuotedKeys).Decode(type);
            if (o == null)
                return null;

            if (type != null)
            {
                if (type == typeof(DataSet))
                    return CreateDataset(o as Dictionary<string, object>, null);
                else if (type == typeof(DataTable))
                    return CreateDataTable(o as Dictionary<string, object>, null);
            }

            if (o is IDictionary)
            {
                if (type != null && typeof(Dictionary<,>).IsAssignableFrom(t)) // deserialize a dictionary
                    return RootDictionary(o, type);
                else // deserialize an object
                    return ParseDictionary(o as Dictionary<string, object>, null, type, null);
            }
            else if (o is List<object>)
            {
                if (type != null)
                {
                    if (typeof(Dictionary<,>).IsAssignableFrom(t)) // kv format
                        return RootDictionary(o, type);
                    else if (t == typeof(List<>)) // deserialize to generic list
                        return RootList(o, type);
                    else if (type.IsArray)
                        return RootArray(o, type);
                    else if (type == typeof(Hashtable))
                        return RootHashTable((List<object>)o);
                }
                else //if (type == null)
                {
                    List<object> l = (List<object>)o;
                    if (l.Count > 0 && l[0].GetType() == typeof(Dictionary<string, object>))
                    {
                        Dictionary<string, object> globals = new Dictionary<string, object>();
                        List<object> op = new List<object>();
                        // try to get $types
                        foreach (var i in l)
                            op.Add(ParseDictionary((Dictionary<string, object>)i, globals, null, null));
                        return op;
                    }
                    return l.ToArray();
                }
            }
            else if (type != null && o.GetType() != type)
                return ChangeType(o, type);

            return o;
        }

        #endregion Public Methods
    }

    public static class JSON
    {
        #region Private Fields

        private static JSONParameters parameters = new JSONParameters();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Globally set-able parameters for controlling the serializer
        /// </summary>
        public static JSONParameters Parameters
        {
            get
            {
                parameters = parameters ?? new JSONParameters();

                return parameters;
            }
            set => parameters = value ?? parameters;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create a human readable string from the json
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Beautify(string input)
        {
            var i = new string(' ', JSON.Parameters.FormatterIndentSpaces);
            return Formatter.PrettyPrint(input, i);
        }

        /// <summary>
        /// Create a human readable string from the json with specified indent spaces
        /// </summary>
        /// <param name="input"></param>
        /// <param name="spaces"></param>
        /// <returns></returns>
        public static string Beautify(string input, byte spaces)
        {
            var i = new string(' ', spaces);
            return Formatter.PrettyPrint(input, i);
        }

        /// <summary>
        /// Clear the internal reflection cache so you can start from new (you will loose performance)
        /// </summary>
        public static void ClearReflectionCache() => Reflection.Instance.ClearReflectionCache();

        /// <summary>
        /// Deep copy an object i.e. clone to a new object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepCopy(this object obj) => new deserializer(Parameters).ToObject(ToJson(obj));

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T obj) => new deserializer(Parameters).ToObject<T>(ToJson(obj));

        /// <summary>
        /// Fill a given object with the json represenation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object FillObject(this object input, string json, Type type)
        {
            type = type ?? input?.GetType();
            Dictionary<string, object> ht = new JsonParser(json, Parameters.AllowNonQuotedKeys).Decode(type) as Dictionary<string, object>;
            if (ht == null) return default;
            return new deserializer(Parameters).ParseDictionary(ht, null, type, input);
        }

        public static object FillObject(this object input, string json) => FillObject(input, json, null);
        public static T FillObject<T>(this T input, string json) => FillObject(input, json, typeof(T)).ChangeType<T>();



        /// <summary>
        /// FromJson a json string and generate a Dictionary&lt;string,object&gt; or
        /// List&lt;object&gt; structure
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object FromJson(this string json) => new JsonParser(json, Parameters.AllowNonQuotedKeys).Decode(null);

        /// <summary>
        /// Create a typed generic object from the json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json) => new deserializer(Parameters).ToObject<T>(json);

        /// <summary>
        /// Create a typed generic object from the json with parameter override on this call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json, JSONParameters param) => new deserializer(param).ToObject<T>(json);

        /// <summary>
        /// Create an object from the json with parameter override on this call
        /// </summary>
        /// <param name="json"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object FromJson(this string json, JSONParameters param) => new deserializer(param).ToObject(json, null);

        /// <summary>
        /// Create an object of type from the json
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromJson(this string json, Type type) => new deserializer(Parameters).ToObject(json, type);

        /// <summary>
        /// Create an object of type from the json with parameter override on this call
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public static object FromJson(this string json, Type type, JSONParameters par) => new deserializer(par).ToObject(json, type);

        /// <summary>
        /// Register custom type handlers for your own types not natively handled by Extensions
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        public static void RegisterCustomType(Type type, Reflection.Serialize serializer, Reflection.Deserialize deserializer) => Reflection.Instance.RegisterCustomType(type, serializer, deserializer);

        /// <summary>
        /// Create a .net4 dynamic object from the json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic ToDynamicJson(this string json) => new DynamicJson(json);

        /// <summary>
        /// Create a json representation for an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj) => ToJson(obj, Parameters);

        /// <summary>
        /// Create a json representation for an object with parameter override on this call
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, JSONParameters param)
        {
            param.FixValues();
            param = param.MakeCopy();
            Type t = null;

            if (obj == null)
                return "null";

            if (obj.GetType().IsGenericType)
                t = Reflection.Instance.GetGenericTypeDefinition(obj.GetType());
            if (typeof(IDictionary).IsAssignableFrom(t) || typeof(List<>).IsAssignableFrom(t))
                param.UsingGlobalTypes = false;

            // FEATURE : enable extensions when you can deserialize anon types
            if (param.EnableAnonymousTypes) { param.UseExtensions = false; param.UsingGlobalTypes = false; }
            return new JSONSerializer(param).ConvertToJSON(obj);
        }

        /// <summary>
        /// Create a formatted json string (beautified) from an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToNiceJson(this object obj) => Beautify(ToJson(obj, Parameters));

        /// <summary>
        /// Create a formatted json string (beautified) from an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToNiceJson(this object obj, JSONParameters param) => Beautify(ToJson(obj, param), param.FormatterIndentSpaces);

        #endregion Public Methods
    }

    public sealed class JSONParameters
    {
        #region Public Properties

        /// <summary>
        /// TESTING - allow non quoted keys in the json like javascript (default = false)
        /// </summary>
        public bool AllowNonQuotedKeys { get; set; }

        /// <summary>
        /// Auto convert string values to numbers when needed (default = true)
        ///
        /// When disabled you will get an exception if the types don't match
        /// </summary>
        public bool AutoConvertStringToNumbers { get; set; } = true;

        /// <summary>
        /// Checking list of bad types to prevent friday 13th json attacks (default = true)
        ///
        /// Will throw an exception if encountered and set
        /// </summary>
        public bool BadListTypeChecking { get; set; } = true;

        /// <summary>
        /// Serialize DateTime milliseconds i.e. yyyy-MM-dd HH:mm:ss.nnn (default = false)
        /// </summary>
        public bool DateTimeMilliseconds { get; set; }

        /// <summary>
        /// Anonymous types have read only properties
        /// </summary>
        public bool EnableAnonymousTypes { get; set; }

        /// <summary>
        /// Formatter indent spaces (default = 3)
        /// </summary>
        public byte FormatterIndentSpaces { get; set; } = 3;

        /// <summary>
        /// Fully Qualify the DataSet Schema (default = false)
        ///
        /// If you get deserialize errors with DataSets and DataTables
        /// </summary>
        public bool FullyQualifiedDataSetSchema { get; set; }

        /// <summary>
        /// Ignore attributes to check for (default : XmlIgnoreAttribute, NonSerialized, IgnoreDataMemberAttribute)
        /// </summary>
        public List<Type> IgnoreAttributes { get; set; } = new List<Type> { typeof(System.Xml.Serialization.XmlIgnoreAttribute), typeof(NonSerializedAttribute), typeof(IgnoreDataMemberAttribute) };

        /// <summary>
        /// Inline circular or already seen objects instead of replacement with $i (default = false)
        /// </summary>
        public bool InlineCircularReferences { get; set; }

        /// <summary>
        /// Output string key dictionaries as "k"/"v" format (default = False)
        /// </summary>
        public bool KVStyleStringDictionary { get; set; }

        /// <summary>
        /// Override object equality hash code checking (default = false)
        /// </summary>
        public bool OverrideObjectHashCodeChecking { get; set; }

        /// <summary>
        /// If you have parametric and no default constructor for you classes (default = False)
        ///
        /// IMPORTANT NOTE : If True then all initial values within the class will be ignored and
        /// will be not set
        /// </summary>
        public bool ParametricConstructorOverride { get; set; }

        /// <summary>
        /// Serialize null values to the output (default = True)
        /// </summary>
        public bool SerializeNullValues { get; set; } = true;

        /// <summary>
        /// Maximum depth for circular references in inline mode (default = 20)
        /// </summary>
        public byte SerializerMaxDepth { get; set; } = 20;

        /// <summary>
        /// Save property/field names as lowercase (default = false)
        /// </summary>
        public bool SerializeToLowerCaseNames { get; set; }

        /// <summary>
        /// Show the readonly properties of types in the output (default = False)
        /// </summary>
        public bool ShowReadOnlyProperties { get; set; }

        /// <summary>
        /// Use escaped unicode i.e. \uXXXX format for non ASCII characters (default = True)
        /// </summary>
        public bool UseEscapedUnicode { get; set; } = true;

        /// <summary>
        /// Enable JSON extensions $types, $type, $map (default = False)
        /// </summary>
        public bool UseExtensions { get; set; }

        /// <summary>
        /// Use the fast GUID format (default = True)
        /// </summary>
        public bool UseFastGuid { get; set; } = true;

        /// <summary>
        /// Use the optimized fast Dataset Schema format (default = True)
        /// </summary>
        public bool UseOptimizedDatasetSchema { get; set; } = true;

        /// <summary>
        /// Use the UTC date format (default = True)
        /// </summary>
        public bool UseUTCDateTime { get; set; } = true;

        /// <summary>
        /// Output Enum values instead of names (default = False)
        /// </summary>
        public bool UseValuesOfEnums { get; set; }

        /// <summary>
        /// Use the $types extension to optimise the output json (default = True)
        /// </summary>
        public bool UsingGlobalTypes { get; set; } = true;

        #endregion Public Properties

        #region Public Methods

        internal void FixValues()
        {
            if (UseExtensions == false) // disable conflicting params
            {
                UsingGlobalTypes = false;
                InlineCircularReferences = true;
            }
            if (EnableAnonymousTypes)
                ShowReadOnlyProperties = true;
        }

        /// <summary>
        /// Create a copy of this instance
        /// </summary>
        /// <returns></returns>
        public JSONParameters MakeCopy() => new JSONParameters
        {
            AllowNonQuotedKeys = AllowNonQuotedKeys,
            DateTimeMilliseconds = DateTimeMilliseconds,
            EnableAnonymousTypes = EnableAnonymousTypes,
            FormatterIndentSpaces = FormatterIndentSpaces,
            IgnoreAttributes = new List<Type>(IgnoreAttributes),
            InlineCircularReferences = InlineCircularReferences,
            KVStyleStringDictionary = KVStyleStringDictionary,
            ParametricConstructorOverride = ParametricConstructorOverride,
            SerializeNullValues = SerializeNullValues,
            SerializerMaxDepth = SerializerMaxDepth,
            SerializeToLowerCaseNames = SerializeToLowerCaseNames,
            ShowReadOnlyProperties = ShowReadOnlyProperties,
            UseEscapedUnicode = UseEscapedUnicode,
            UseExtensions = UseExtensions,
            UseFastGuid = UseFastGuid,
            UseOptimizedDatasetSchema = UseOptimizedDatasetSchema,
            UseUTCDateTime = UseUTCDateTime,
            UseValuesOfEnums = UseValuesOfEnums,
            UsingGlobalTypes = UsingGlobalTypes,
            AutoConvertStringToNumbers = AutoConvertStringToNumbers,
            OverrideObjectHashCodeChecking = OverrideObjectHashCodeChecking,
            FullyQualifiedDataSetSchema = FullyQualifiedDataSetSchema,
            BadListTypeChecking = BadListTypeChecking
        };

        #endregion Public Methods
    }
}