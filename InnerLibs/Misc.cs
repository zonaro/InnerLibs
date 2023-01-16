using InnerLibs.LINQ;
using InnerLibs.Mail;
using InnerLibs.MicroORM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace InnerLibs
{
    public static class Misc
    {
        #region Public Methods

        public static IEnumerable<TemplateMailAddress<T>> AddAttachmentFromData<T>(this IEnumerable<TemplateMailAddress<T>> recipients, Expression<Func<T, IEnumerable<System.Net.Mail.Attachment>>> AttachmentSelector) where T : class
        {
            if (AttachmentSelector != null)
                foreach (var rec in recipients ?? Array.Empty<TemplateMailAddress<T>>())
                {
                    if (rec?.TemplateData != null)
                    {
                        var att = AttachmentSelector.Compile().Invoke(rec.TemplateData);
                        if (att != null)
                        {
                            if (rec.Attachments == null) rec.Attachments = new List<Attachment>();
                            rec.Attachments.AddRange(att.Where(x => x != null));
                        }
                    }
                }
            return recipients;
        }

        public static TemplateMailAddress<T> AddAttachmentFromData<T>(this TemplateMailAddress<T> recipient, Expression<Func<T, IEnumerable<System.Net.Mail.Attachment>>> AttachmentSelector) where T : class => AddAttachmentFromData(new[] { recipient }, AttachmentSelector).FirstOrDefault();

        public static TemplateMailAddress<T> AddAttachmentFromData<T>(this TemplateMailAddress<T> recipient, Expression<Func<T, System.Net.Mail.Attachment>> AttachmentSelector) where T : class => AddAttachmentFromData(new[] { recipient }, AttachmentSelector).FirstOrDefault();

        public static IEnumerable<TemplateMailAddress<T>> AddAttachmentFromData<T>(this IEnumerable<TemplateMailAddress<T>> recipients, Expression<Func<T, System.Net.Mail.Attachment>> AttachmentSelector) where T : class
        {
            if (AttachmentSelector != null)
                foreach (var rec in recipients ?? Array.Empty<TemplateMailAddress<T>>())
                {
                    if (rec?.TemplateData != null)
                    {
                        var att = AttachmentSelector.Compile().Invoke(rec.TemplateData);
                        if (att != null)
                        {
                            if (rec.Attachments == null) rec.Attachments = new List<Attachment>();
                            rec.Attachments.Add(att);
                        }
                    }
                }
            return recipients;
        }

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BoolExp">Expressão de teste de Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static TR AsIf<T, TR>(this T obj, Expression<Func<T, bool>> BoolExp, TR TrueValue, TR FalseValue = default) => obj == null || BoolExp == null ? FalseValue : BoolExp.Compile().Invoke(obj).AsIf(TrueValue, FalseValue);

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool Bool, T TrueValue, T FalseValue = default) => Bool ? TrueValue : FalseValue;

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue = default) => (Bool.HasValue && Bool.Value).AsIf(TrueValue, FalseValue);

        /// <inheritdoc cref="AsIf{T}(bool?, T, T)"/>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue, T NullValue)
        {
            if (Bool.HasValue)
                return Bool.Value.AsIf(TrueValue, FalseValue);
            else
                return NullValue;
        }

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento
        /// que possuir um valor
        /// </summary>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(this string First, params string[] N) => BlankCoalesce(new[] { First }.Union(N ?? Array.Empty<string>()).ToArray());

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento
        /// que possuir um valor
        /// </summary>
        /// <param name="N">Itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(params string[] N) => (N ?? Array.Empty<string>()).FirstOr(x => x.IsNotBlank(), InnerLibs.Text.Empty);

        /// <summary>
        /// Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="List1">Lista 1</param>
        /// <param name="List2">Lista2</param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEnumerable<T> List2, IEqualityComparer<T> Comparer = null)
        {
            foreach (T value in List2 ?? Array.Empty<T>())
            {
                if (Comparer != null)
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEqualityComparer<T> Comparer, params T[] List2) => List1.ContainsAll((List2 ?? Array.Empty<T>()).AsEnumerable(), Comparer);

        /// <summary>
        /// Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="List1">Lista 1</param>
        /// <param name="List2">Lista2</param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> List1, IEnumerable<T> List2, IEqualityComparer<T> Comparer = null)
        {
            foreach (T value in List2.AsEnumerable() ?? Array.Empty<T>())
            {
                if (Comparer == null)
                {
                    if ((List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return true;
                    }
                }
                else if ((List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converte um objeto para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="T">
        /// Tipo da classe, <see cref="NameValueCollection"/> ou <see cref="Dictionary{TKey, TValue}"/>
        /// </typeparam>
        /// <param name="Obj">valor do objeto</param>
        /// <param name="Keys">Chaves incluidas no dicionario final</param>
        /// <returns></returns>
        public static Dictionary<string, object> CreateDictionary<T>(this T Obj, params string[] Keys)
        {
            if (Obj != null)
            {
                Keys = Keys ?? Array.Empty<string>();
                if (Obj.IsDictionary())
                {
                    return ((Dictionary<string, object>)(object)Obj).ToDictionary(Keys);
                }
                else if (Obj.IsTypeOf<NameValueCollection>())
                {
                    return ((NameValueCollection)(object)Obj).ToDictionary(Keys);
                }
                else
                {
                    return Obj.GetTypeOf().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => (Keys.Any() == false || x.Name.IsLikeAny(Keys)) && x.CanRead).ToDictionary(prop => prop.Name, prop => prop.GetValue(Obj, null));
                }
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// Converte uma classe para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="Type">Tipo da classe</typeparam>
        /// <param name="Obj">Object</param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> CreateDictionaryEnumerable<Type>(this IEnumerable<Type> Obj) => (Obj ?? Array.Empty<Type>()).Select(x => x.CreateDictionary());

        /// <summary>
        /// Cria um <see cref="Guid"/> a partir de uma string ou um novo <see cref="Guid"/> se a
        /// conversão falhar
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Guid CreateGuidOrDefault(this string Source)
        {
            Guid g;
            if (Source.IsNotBlank() || !Guid.TryParse(Source, out g))
            {
                g = Guid.NewGuid();
            }

            return g;
        }

        public static T CreateObjectFromXML<T>(this string XML) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            T obj;
            using (var reader = new StringReader(XML))
            {
                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }

        public static T CreateObjectFromXMLFile<T>(this FileInfo XML) where T : class => File.ReadAllText(XML.FullName).CreateObjectFromXML<T>();

        /// <summary>
        /// Converte um objeto para XML
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="obj">Valor do objeto</param>
        /// <returns>um <see cref="XmlDocument"/></returns>
        public static XmlDocument CreateXML<T>(this T obj) where T : class
        {
            var xs = new XmlSerializer(obj.GetType());
            var doc = new XmlDocument();
            using (var sw = new StringWriter())
            {
                xs.Serialize(sw, obj);
                doc.LoadXml(sw.ToString());
            }

            return doc;
        }

        /// <summary>
        /// Cria um arquivo a partir de qualquer objeto usando o <see cref="Misc.CreateXML()"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FileInfo CreateXmlFile(this object obj, string FilePath) => obj.CreateXML().ToXMLString().WriteToFile(FilePath);

        /// <summary>
        /// Remove um item de uma lista e retorna este item
        /// </summary>
        /// <typeparam name="T">Tipo do item</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="Index">Posicao do item</param>
        /// <returns></returns>
        public static T Detach<T>(this List<T> List, int Index)
        {
            if (List != null && Index.IsBetween(0, List.Count))
            {
                var p = List.ElementAt(Index);
                List.RemoveAt(Index);
                return p;
            }
            return default;
        }

        /// <summary>
        /// Remove itens de uma lista e retorna uma outra lista com estes itens
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="Indexes"></param>
        /// <returns></returns>
        public static IEnumerable<T> DetachMany<T>(this List<T> List, params int[] Indexes) => List.MoveItems(null, Indexes);

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<T, long> DistinctCount<T>(this IEnumerable<T> Arr) => Arr.Distinct().Select(p => new KeyValuePair<T, long>(p, Arr.Where(x => x.Equals(p)).LongCount())).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao a partir de uma propriedade
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<TProp, long> DistinctCount<T, TProp>(this IEnumerable<T> Arr, Func<T, TProp> Prop) => Arr.GroupBy(Prop).ToDictionary(x => x.Key, x => x.LongCount()).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao e agrupa o resto
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<T, long> DistinctCountTop<T>(this IEnumerable<T> Arr, int Top, T Others)
        {
            var a = Arr.DistinctCount();
            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao a partir de uma propriedade e agrupa o
        /// resto em outra
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<TProp, long> DistinctCountTop<T, TProp>(this IEnumerable<T> Arr, Func<T, TProp> Prop, int Top, TProp Others)
        {
            var a = Arr.DistinctCount(Prop);
            if (Top < 1)
            {
                return a;
            }

            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        public static FieldInfo FindField(this Type type, string Name) => FindFields(type, Name).FirstOrDefault();

        public static IEnumerable<FieldInfo> FindFields(this Type type, params string[] Names)
        {
            if (type != null && Names != null)
            {
                var propnames = Names.SelectMany(x => x.PropertyNamesFor()).ToList();
                return type.GetFields().Where(x => x.GetCustomAttributes<ColumnName>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
            }
            return Array.Empty<FieldInfo>();
        }

        public static IEnumerable<PropertyInfo> FindProperties(this Type type, params string[] Names)
        {
            if (type != null && Names != null)
            {
                var propnames = Names.SelectMany(x => x.PropertyNamesFor()).ToList();
                return type.GetProperties().Where(x => x.GetCustomAttributes<ColumnName>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
            }
            return Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo FindProperty(this Type type, string Name) => FindProperties(type, Name).FirstOrDefault();

        /// <summary>
        /// T primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAny<T>(this IEnumerable<T> source, params Expression<Func<T, bool>>[] predicate)
        {
            predicate = predicate ?? Array.Empty<Expression<Func<T, bool>>>();
            for (int index = 0, loopTo = predicate.Length - 1; index <= loopTo; index++)
            {
                var v = source.FirstOrDefault(predicate[index].Compile());
                if (v != null)
                {
                    return v;
                }
            }

            return default;
        }

        /// <summary>
        /// T primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAnyOr<T>(this IEnumerable<T> source, T Alternate, params Expression<Func<T, bool>>[] predicate)
        {
            var item = (source ?? Array.Empty<T>()).FirstAny(predicate);
            return (item == null) ? Alternate : item;
        }

        /// <summary>
        /// Troca valor de <paramref name="FirstValue"/> pelo de <paramref name="SecondValue"/> se
        /// <paramref name="FirstValue"/> for maior que <paramref name="SecondValue"/> fazendo com
        /// que <paramref name="FirstValue"/> seja sempre menor que <paramref name="SecondValue"/>.
        /// Util para tratar ranges
        /// </summary>
        /// <remarks>
        /// Caso <paramref name="FirstValue"/> e/ou <paramref name="SecondValue"/> forem
        /// <b>null</b>, nada acontece
        /// </remarks>
        public static (T, T) FixOrder<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue != null && SecondValue != null)
            {
                if (FirstValue.IsGreaterThan(SecondValue))
                {
                    return Swap(ref FirstValue, ref SecondValue);
                }
            }

            return (FirstValue, SecondValue);
        }

        /// <summary>
        /// Troca valor de <paramref name="FirstValue"/> pelo de <paramref name="SecondValue"/> se
        /// <paramref name="FirstValue"/> for maior que <paramref name="SecondValue"/> fazendo com
        /// que <paramref name="FirstValue"/> seja sempre menor que <paramref name="SecondValue"/>.
        /// Util para tratar ranges. Se qualquer um dos 2 valores for null, copia o valor da outra
        /// variavel não <b>null</b>. Se ambas forem <b>null</b> nada acontece.
        /// </summary>
        public static (T, T) FixOrderNotNull<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue == null && SecondValue != null)
            {
                FirstValue = SecondValue;
            }

            if (SecondValue == null && FirstValue != null)
            {
                SecondValue = FirstValue;
            }

            if (SecondValue == null && FirstValue == null)
            {
                FirstValue = default;
                SecondValue = default;
            }

            return Misc.FixOrder(ref FirstValue, ref SecondValue);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this MemberInfo prop, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (prop.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att && att != null)
            {
                return att.GetAttributeValue(ValueSelector);
            }

            return default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (type != null && type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att && att != null)
            {
                return att.GetAttributeValue(ValueSelector);
            }

            return default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this TAttribute att, Expression<Func<TAttribute, TValue>> ValueSelector)
        {
            if (att != null)
            {
                if (ValueSelector == null)
                {
                    ValueSelector = x => x.ToString().ChangeType<TValue>();
                }
                try
                {
                    return ValueSelector.Compile()(att);
                }
                catch { }
            }
            return default;
        }

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de uma string </summary> <typeparam name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this string Name)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("type must be an Enumeration type.");
            return Name.IsNotBlank() ? ((T[])Enum.GetValues(typeof(T))).FirstOrDefault(x => x.ToString().RemoveAccents().Equals(Name.RemoveAccents(), StringComparison.InvariantCultureIgnoreCase) || (Name.IsNumber() && Name.ToInt() == x.ToInt())) : default;
        }

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de um <paramref name="Value"/> inteiro </summary> <typeparam
        /// name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this int? Value) => Value.HasValue ? GetEnumValue<T>($"{Value.Value}") : default(T);

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de um <paramref name="Value"/> inteiro </summary> <typeparam
        /// name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this int Value) => GetEnumValue<T>($"{Value}");

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this T Value)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("type must be an Enumeration type.", nameof(T));
            return Enum.GetName(typeof(T), Value);
        }

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this string Value) => Value.GetEnumValue<T>().GetEnumValueAsString<T>();

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this int Value) => Value.GetEnumValue<T>().GetEnumValueAsString<T>();

        /// <summary>
        /// Traz todos os Valores de uma enumeração
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("type must be an Enumeration type.", nameof(T));
            return Enum.GetValues(typeof(T)).Cast<T>().AsEnumerable();
        }

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static FieldInfo GetField<T>(this T MyObject, string Name) => MyObject.GetTypeOf().GetFields().SingleOrDefault(x => (x.Name ?? InnerLibs.Text.Empty) == (Name ?? InnerLibs.Text.Empty));

        public static IEnumerable<FieldInfo> GetFields<T>(this T MyObject, BindingFlags BindAttr) => MyObject.GetTypeOf().GetFields(BindAttr).ToList();

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFields<T>(this T MyObject) => MyObject.GetTypeOf().GetFields().ToList();

        public static IEnumerable<Type> GetInheritedClasses<T>() where T : class => GetInheritedClasses(typeof(T));

        public static IEnumerable<Type> GetInheritedClasses(this Type MyType)
        {
            //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType));
        }

        public static string GetMemberName(MemberInfo member)
        {
            if (member != null)
            {
                if (member.IsDefined(typeof(DataMemberAttribute), true))
                {
                    DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        return dataMemberAttribute.Name;
                }

                return member.Name;
            }
            return null;
        }

        /// <summary>
        /// Retorna o <see cref="Type"/> equivalente a <typeparamref name="T"/> ou o <see
        /// cref="Type"/> do objeto <see cref="Nullable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>
        /// o tipo do objeto ou o tipo do objeto anulavel ou o prorio objeto se ele for um <see cref="Type"/>
        /// </returns>
        public static Type GetNullableTypeOf<T>(this T Obj)
        {
            var tt = Obj.GetTypeOf();
            tt = Nullable.GetUnderlyingType(tt) ?? tt;
            return tt;
        }

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T MyObject, BindingFlags BindAttr) => MyObject.GetTypeOf().GetProperties(BindAttr).ToList();

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T MyObject) => MyObject.GetTypeOf().GetProperties().ToList();

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<T>(this T MyObject, string Name) => MyObject.GetTypeOf().GetProperties().SingleOrDefault(x => (x.Name ?? InnerLibs.Text.Empty) == (Name ?? InnerLibs.Text.Empty));

        /// <summary>
        /// Retorna uma <see cref="Hashtable"/> das propriedades de um objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Hashtable GetPropertyHash<T>(T properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                var props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }

            return values;
        }

        /// <summary>
        /// Traz o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static T GetPropertyValue<T, O>(this O MyObject, string Name) where O : class
        {
            if (MyObject != null)
            {
                var prop = MyObject.GetProperty(Name);
                if (prop != null && prop.CanRead)
                {
                    return (T)prop.GetValue(MyObject);
                }
            }

            return default;
        }

        /// <summary>
        /// Pega os bytes de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static byte[] GetResourceBytes(this Assembly Assembly, string FileName) => Assembly.GetManifestResourceStream(FileName)?.ToBytes() ?? Array.Empty<byte>();

        public static byte[] GetResourceBytes(string FileName) => GetResourceBytes(Assembly.GetExecutingAssembly(), FileName);

        /// <summary>
        /// Pega o texto de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
        /// <returns></returns>
        public static string GetResourceFileText(this Assembly Assembly, string FileName)
        {
            string txt = null;
            if (Assembly != null && FileName.IsNotBlank())
            {
                using (var x = Assembly.GetManifestResourceStream(FileName))
                {
                    if (x != null)
                    {
                        using (var r = new StreamReader(x))
                        {
                            txt = r.ReadToEnd();
                        }
                    };
                }
            }

            return txt;
        }

        public static string GetResourceFileText(string FileName) => GetResourceFileText(Assembly.GetExecutingAssembly(), FileName);

        /// <summary>
        /// Retorna o <see cref="Type"/> do objeto mesmo se ele for nulo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>o tipo do objeto ou o prorio objeto se ele for um <see cref="Type"/></returns>
        public static Type GetTypeOf<T>(this T Obj)
        {
            if (Obj is Type istype)
            {
                return istype;
            }
            else
            {
                try
                {
                    return Obj.GetType();
                }
                catch { }
            }
            return typeof(T);
        }

        /// <summary>
        /// Tries to get a value from <see cref="Dictionary{TKey, TValue}"/>. if fails, return
        /// <paramref name="ReplaceValue"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Key"></param>
        /// <param name="ReplaceValue"></param>
        /// <remarks>
        /// if <paramref name="ReplaceValue"/> is not provided. the default value for type
        /// <typeparamref name="TValue"/> is returned
        /// </remarks>
        /// <returns></returns>
        public static TValue GetValueOr<TKey, TValue>(this IDictionary<TKey, TValue> Dic, TKey Key, TValue ReplaceValue = default) => Dic != null && Dic.ContainsKey(Key) ? Dic[Key] : ReplaceValue;

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, long> GroupAndCountBy<Type, Group>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector) => obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, long>(x.Key, x.LongCount())).ToDictionary();

        /// <summary>
        /// Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada
        /// grupo a partir de outra propriedade do mesmo objeto
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <typeparam name="Count"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <param name="CountObjectBy"></param>
        /// <returns></returns>
        public static Dictionary<Group, Dictionary<Count, long>> GroupAndCountSubGroupBy<Type, Group, Count>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector, Func<Type, Count> CountObjectBy)
        {
            var dic_of_dic = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, Dictionary<Count, long>>(x.Key, x.GroupBy(CountObjectBy).ToDictionary(y => y.Key, y => y.LongCount()))).ToDictionary();
            dic_of_dic.Values.MergeKeys();

            return dic_of_dic;
        }

        /// <summary>
        /// Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um
        /// grupo com subgrupos daquele objeto
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <typeparam name="SubGroup"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <param name="SubGroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, Dictionary<SubGroup, IEnumerable<Type>>> GroupAndSubGroupBy<Type, Group, SubGroup>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector, Func<Type, SubGroup> SubGroupSelector)
        {
            var dic_of_dic = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, Dictionary<SubGroup, IEnumerable<Type>>>(x.Key, x.GroupBy(SubGroupSelector).ToDictionary(y => y.Key, y => y.AsEnumerable()))).ToDictionary();
            dic_of_dic.Values.MergeKeys();
            return dic_of_dic;
        }

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attribType"></param>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo target, Type attribType) => target?.GetCustomAttributes(attribType, false).Any() ?? false;

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo target) => target?.HasAttribute(typeof(T)) ?? false;

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type Type, string PropertyName, bool GetPrivate = false)
        {
            if (Type != null && PropertyName.IsNotBlank())
            {
                var parts = new List<string>();
                bool stop = false;
                string current = Text.Empty;
                for (int i = 0, loopTo = PropertyName.Length - 1; i <= loopTo; i++)
                {
                    if (PropertyName[i] != '.')
                    {
                        current += $"{PropertyName[i]}";
                    }

                    if (PropertyName[i] == '[')
                    {
                        stop = true;
                    }

                    if (PropertyName[i] == ']')
                    {
                        stop = false;
                    }

                    if (PropertyName[i] == '.' && !stop || i == PropertyName.Length - 1)
                    {
                        parts.Add(current.ToString());
                        current = Text.Empty;
                    }
                }

                PropertyInfo prop;
                string propname = parts.First().GetBefore("[");
                if (GetPrivate)
                {
                    prop = Type.GetProperty(propname, (BindingFlags)((int)BindingFlags.Public + (int)BindingFlags.NonPublic + (int)BindingFlags.Instance));
                }
                else
                {
                    prop = Type.GetProperty(propname);
                }

                bool exist = prop != null;
                parts.RemoveAt(0);
                if (exist && parts.Count > 0)
                {
                    exist = prop.PropertyType.HasProperty(parts.First(), GetPrivate);
                }

                return exist;
            }

            return false;
        }

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool HasProperty(this object Obj, string Name) => Obj?.GetType().HasProperty(Name, true) ?? false;

        public static bool IsAny<T>(this T obj, params T[] others) => others?.Any(x => x.Equals(obj)) ?? false;

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this Type Type) => Type == typeof(T[]);

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this object Obj) => Obj.GetTypeOf().IsArrayOf<T>();

        /// <summary>
        /// Verifica se <paramref name="Value"/> é igual a <paramref name="MinValue"/> ou está entre
        /// <paramref name="MinValue"/> e <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <b>true</b> se <paramref name="Value"/> for igual a <paramref name="MinValue"/>.
        /// Retorna <b>false</b> se <paramref name="Value"/> for igual a <paramref
        /// name="MaxValue"/>. <br/> Utilize <see cref="IsBetweenOrEqual(IComparable, IComparable,
        /// IComparable)"/> para incluir <paramref name="MaxValue"/> ou <see
        /// cref="IsBetweenExclusive(IComparable, IComparable, IComparable)"/> para excluir
        /// <paramref name="MinValue"/>
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetween(this IComparable Value, IComparable MinValue, IComparable MaxValue)
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue == MaxValue ? Value == MinValue : Value.IsGreaterThanOrEqual(MinValue) && Value.IsLessThan(MaxValue);
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> está entre <paramref name="MinValue"/> e <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <see cref="false"/> se <paramref name="Value"/> for igual a <paramref
        /// name="MinValue"/> ou <paramref name="MaxValue"/>. <br/> Utilize <see
        /// cref="IsBetween(IComparable, IComparable, IComparable)"/> para incluir <paramref
        /// name="MinValue"/> ou <see cref="IsBetweenOrEqual(IComparable, IComparable,
        /// IComparable)"/> para incluir ambos
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetweenExclusive(this IComparable Value, IComparable MinValue, IComparable MaxValue)
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue != MaxValue && Value.IsGreaterThan(MinValue) && Value.IsLessThan(MaxValue);
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> é igual ou está entre <paramref name="MinValue"/> e
        /// <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <b>true</b> se <paramref name="Value"/> for igual a <paramref name="MinValue"/>
        /// ou <paramref name="MaxValue"/>. <br/> Utilize <see cref="IsBetween(IComparable,
        /// IComparable, IComparable)"/> para excluir <paramref name="MaxValue"/> ou <see
        /// cref="IsBetweenExclusive(IComparable, IComparable, IComparable)"/> para excluir ambos
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetweenOrEqual(this IComparable Value, IComparable MinValue, IComparable MaxValue)
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue == MaxValue ? Value == MinValue : Value.IsGreaterThanOrEqual(MinValue) && Value.IsLessThanOrEqual(MaxValue);
        }

        /// <summary>
        /// Verifica se o objeto é um iDictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDictionary(this object obj) => IsGenericOf(obj, typeof(IDictionary<,>)) || IsGenericOf(obj, typeof(IDictionary));

        /// <summary>
        /// Verifica se o objeto é um enumeravel (lista)
        /// </summary>
        /// <param name="obj"></param>
        /// <remarks>NÃO considera strings (IEnumerable{char}) como true</remarks>
        /// <returns></returns>
        public static bool IsEnumerable(this object obj) => IsGenericOf(obj, typeof(IEnumerable<>)) || IsGenericOf(obj, typeof(IEnumerable));

        public static bool IsEqual<T>(this T Value, T EqualsToValue) where T : IComparable => Value.Equals(EqualsToValue);

        public static bool IsGenericOf(this object obj, Type GenericType)
        {
            var type = obj.GetTypeOf();

            if (type == null || GenericType == null) return false;
            if (type == GenericType) return true;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == GenericType) return true;
            if (GenericType.IsGenericType && GenericType.GetGenericTypeDefinition().IsAssignableFrom(type)) return true;
            if (GenericType.IsAssignableFrom(type)) return true;
            if (type.GetInterfaces().Append(type).Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericType)) return true;
            return false;
        }

        /// <summary>
        /// Verifica se um tipo e generico de outro
        /// </summary>
        /// <param name="MainType"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsGreaterThan<T>(this T Value, T MinValue) where T : IComparable => Value.CompareTo(MinValue) > 0;

        public static bool IsGreaterThanOrEqual<T>(this T Value, T MinValue) where T : IComparable => Value.IsGreaterThan(MinValue) || Value.IsEqual(MinValue);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<T>(this T Obj, params T[] List) => Obj.IsIn((List ?? Array.Empty<T>()).ToList());

        public static bool IsIn<T>(this T Obj, IEqualityComparer<T> Comparer = null, params T[] List) => Obj.IsIn((List ?? Array.Empty<T>()).ToList(), Comparer);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<Type>(this Type Obj, IEnumerable<Type> List, IEqualityComparer<Type> Comparer = null) => Comparer is null ? List.Contains(Obj) : List.Contains(Obj, Comparer);

        public static bool IsIn<Type>(this Type Obj, string Text, StringComparison? Comparer = null) => Comparer == null ? Text.Contains(Obj.ToString()) : Text.Contains(Obj.ToString(), Comparer.Value);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsInAny<Type>(this Type Obj, IEnumerable<Type>[] List, IEqualityComparer<Type> Comparer = null) => (List ?? Array.Empty<IEnumerable<Type>>()).Any(x => Obj.IsIn(x, Comparer));

        public static bool IsLessThan<T>(this T Value, T MaxValue) where T : IComparable => Value.CompareTo(MaxValue) < 0;

        public static bool IsLessThanOrEqual<T>(this T Value, T MaxValue) where T : IComparable => Value.IsLessThan(MaxValue) || Value.IsEqual(MaxValue);

        /// <summary>
        /// Verifica se o objeto é uma lista
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsList(this object obj) => IsGenericOf(obj, typeof(List<>));

        /// <summary>
        /// Verifica se o não objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsNotIn<Type>(this Type Obj, IEnumerable<Type> List, IEqualityComparer<Type> Comparer = null) => !Obj.IsIn(List, Comparer);

        /// <summary>
        /// Verifica se o objeto não existe dentro de um texto
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="TExt">Texto</param>
        /// <returns></returns>
        public static bool IsNotIn<Type>(this Type Obj, string Text, StringComparison? Comparer = null) => !Obj.IsIn(Text, Comparer);

        /// <summary>
        /// Checks if a <paramref name="List"/> is not <b>null</b> and contains at least one item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> List) => (List ?? Array.Empty<T>()).Any();

        public static bool IsNullableType(this Type t) => t.IsGenericType && Nullable.GetUnderlyingType(t) != null;

        public static bool IsNullableType<O>(this O Obj) => IsNullableType(Obj.GetTypeOf());

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<T>(this object Obj) => Obj.IsNullableTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<O>(this O Obj, Type Type) => Obj.GetNullableTypeOf() == Type.GetNullableTypeOf();

        /// <summary>
        /// Checks if a <paramref name="List"/> is <b>null</b> or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> List) => !List.IsNotNullOrEmpty();

        /// <summary>
        /// Verifica se o objeto é do tipo numérico.
        /// </summary>
        /// <remarks>Boolean is not considered numeric.</remarks>
        public static bool IsNumericType<T>(this T Obj) => Obj.GetNullableTypeOf().IsIn(PredefinedArrays.NumericTypes);

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsTypeOf<T>(this object Obj) => Obj.IsTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsTypeOf<O>(this O Obj, Type Type) => Obj.GetTypeOf() == Type.GetTypeOf();

        public static bool IsValueType(this Type T) => T.IsIn(PredefinedArrays.ValueTypes);

        public static bool IsValueType<T>(this T Obj) => Obj.GetNullableTypeOf().IsValueType();

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="Collections"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(this IEnumerable<NameValueCollection> Collections)
        {
            Collections = Collections ?? new List<NameValueCollection>();
            var all = new NameValueCollection();
            foreach (var i in Collections)
            {
                all.Add(i);
            }

            return all;
        }

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="OtherCollections"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(this NameValueCollection FirstCollection, params NameValueCollection[] OtherCollections)
        {
            OtherCollections = OtherCollections ?? Array.Empty<NameValueCollection>();
            OtherCollections = new[] { FirstCollection }.Union(OtherCollections).ToArray();
            return OtherCollections.Merge();
        }

        /// <summary>
        /// Move os itens de uma lista para outra
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FromList"></param>
        /// <param name="ToList"></param>
        /// <param name="Indexes"></param>
        /// <returns></returns>
        public static List<T> MoveItems<T>(this List<T> FromList, List<T> ToList, params int[] Indexes)
        {
            ToList = ToList ?? new List<T>();
            if (FromList != null)
            {
                Indexes = Indexes?.Where(x => x.IsBetween(0, FromList.Count)).ToArray() ?? Array.Empty<int>();
                foreach (var index in Indexes)
                {
                    var item = FromList.Detach(index);
                    try
                    {
                        ToList.Insert(index, item);
                    }
                    catch
                    {
                        ToList.Add(item);
                    }
                }
            }
            return ToList;
        }

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this T? First, params T?[] N) where T : struct => (T?)(T)First ?? N.NullCoalesce<T>();

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this IEnumerable<T?> List) where T : struct => List?.FirstOrDefault(x => x.HasValue) ?? default;

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this T First, params T[] N) where T : class => First ?? NullCoalesce((N ?? Array.Empty<T>()).AsEnumerable());

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this IEnumerable<T> List) => List == null ? default : List.FirstOrDefault(x => x != null);

        /// <summary>
        /// Substitui todas as propriedades nulas de uma classe pelos seus valores Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static T NullPropertiesAsDefault<T>(this T Obj, bool IncludeVirtual = false) where T : class
        {
            TryExecute(() => Obj = Obj ?? Activator.CreateInstance<T>());
            if (Obj != null)
                foreach (var item in Obj.GetProperties())
                {
                    if (item.CanRead && item.CanWrite && item.GetValue(Obj) is null)
                    {
                        switch (item.PropertyType)
                        {
                            case var @case when @case == typeof(string):
                                {
                                    item.SetValue(Obj, InnerLibs.Text.Empty);
                                    break;
                                }

                            default:
                                {
                                    bool IsVirtual = item.GetAccessors().All(x => x.IsVirtual) && IncludeVirtual;
                                    if (item.IsValueType() || IsVirtual)
                                    {
                                        var o = Activator.CreateInstance(item.PropertyType.GetNullableTypeOf());
                                        item.SetValue(Obj, o);
                                    }

                                    break;
                                }
                        }
                    }
                }

            return Obj;
        }

        /// <summary>
        /// Verifica se somente um unico elemento corresponde a condição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool OnlyOneOf<T>(this IEnumerable<T> List, Func<T, bool> predicate) => List?.Count(predicate) == 1;

        public static string Peek(this Queue<char> queue, int take) => new String(queue.Take(take).ToArray());

        public static IEnumerable<string> PropertyNamesFor(this string Name)
        {
            var propnames = new List<string>();

            if (Name.IsNotBlank())
            {
                if (Name.StartsWith("_", StringComparison.InvariantCultureIgnoreCase))
                {
                    propnames.Add(Name.TrimStart('_'));
                }
                string propname1 = Name.Trim().Replace(" ", "_").Replace("-", "_").Replace("~", "_");
                string propname3 = Name.Trim().Replace(" ", InnerLibs.Text.Empty).Replace("-", InnerLibs.Text.Empty).Replace("~", InnerLibs.Text.Empty);
                string propname2 = propname1.RemoveAccents();
                string propname4 = propname3.RemoveAccents();
                propnames.AddRange(new[] { Name, propname1, propname2, propname3, propname4 });
                propnames.AddRange(propnames.Select(x => $"_{x}").ToArray());
                return propnames.Distinct();
            }
            return Array.Empty<string>();
        }

        public static HtmlTag QueryLinq(this HtmlTag tags, Func<HtmlTag, bool> query) => QueryLinq(tags.Children, query);

        public static HtmlTag QueryLinq(this IEnumerable<HtmlTag> tags, Func<HtmlTag, bool> query) => QueryLinqAll(tags, query).FirstOrDefault();

        public static IEnumerable<HtmlTag> QueryLinqAll(this HtmlTag tags, Func<HtmlTag, bool> query) => QueryLinqAll(tags.Children, query);

        public static IEnumerable<HtmlTag> QueryLinqAll(this IEnumerable<HtmlTag> tags, Func<HtmlTag, bool> query) => tags.Traverse(ht => ht.Children).Where(query);

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<T, long> ReduceToTop<T>(this Dictionary<T, long> obj, int First, T OtherLabel)
        {
            var grouped = obj.OrderByDescending(x => x.Value);
            return grouped.Take(First).Union(new[] { new KeyValuePair<T, long>(OtherLabel, grouped.Skip(First).Sum(s => s.Value)) }).ToDictionary();
        }

        public static Dictionary<TGroup, Dictionary<TCount, long>> ReduceToTop<TGroup, TCount>(this Dictionary<TGroup, Dictionary<TCount, long>> Grouped, int First, TCount OtherLabel)
        {
            if (Grouped != null)
            {
                foreach (var item in Grouped.ToArray())
                {
                    var gp = item.Value.OrderByDescending(x => x.Value).ToDictionary();
                    Grouped[item.Key] = gp.Take(First).Union(new[] { new KeyValuePair<TCount, long>(OtherLabel, gp.Skip(First).Sum(s => s.Value)) }).ToDictionary();
                }

                Grouped.Values.MergeKeys();
            }
            return Grouped;
        }

        /// <summary>
        /// Remove de um dicionario as respectivas Keys se as mesmas existirem
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="Keys"></param>
        public static IDictionary<TKey, TValue> RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, params TKey[] Keys)
        {
            if (dic != null)
                foreach (var k in (Keys ?? Array.Empty<TKey>()).Where(x => dic.ContainsKey(x)))
                {
                    dic.Remove(k);
                }

            return dic;
        }

        /// <summary>
        /// Remove de um dicionario os valores encontrados pelo predicate
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="predicate"></param>
        public static IDictionary<TKey, TValue> RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<KeyValuePair<TKey, TValue>, bool> predicate) => dic.RemoveIfExist(dic.Where(predicate).Select(x => x.Key).ToArray());

        /// <summary>
        /// Remove <paramref name="Count"/> elementos de uma <paramref name="List"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<T> RemoveLast<T>(this List<T> List, int Count = 1)
        {
            if (List != null)
                for (int index = 1, loopTo = Count; index <= loopTo; index++)
                {
                    if (List.Any())
                    {
                        List.RemoveAt(List.Count - 1);
                    }
                }

            return List;
        }

        /// <summary>
        /// Adciona ou substitui um valor a este <see cref="Dictionary(Of TKey, TValue)"/> e retorna
        /// a mesma instancia deste <see cref="Dictionary(Of TKey, TValue)"/>
        /// </summary>
        /// <typeparam name="TKey">Tipo da Key</typeparam>
        /// <typeparam name="TValue">Tipo do valor</typeparam>
        /// <param name="Key">Valor da key</param>
        /// <param name="Value">Valor do Value</param>
        /// <returns>o mesmo objeto do tipo <see cref="Dictionary"/> que chamou este método</returns>
        public static IDictionary<TKey, TValue> Set<TKey, TValue, TK, TV>(this IDictionary<TKey, TValue> Dic, TK Key, TV Value)
        {
            if (Key != null && Dic != null)
            {
                Dic[Key.ChangeType<TKey>()] = Value.ChangeType<TValue>();
            }

            return Dic;
        }

        public static T SetOrRemove<T, TK, TV>(this T Dictionary, KeyValuePair<TK, TV> Pair) where T : IDictionary<TK, TV>
        {
            Dictionary?.SetOrRemove(Pair.Key, Pair.Value);
            return Dictionary;
        }

        public static IDictionary<TKey, string> SetOrRemove<TKey, TK>(this IDictionary<TKey, string> Dic, TK Key, string Value, bool NullIfBlank) => Dic.SetOrRemove(Key, NullIfBlank.AsIf(Value.NullIf(x => x.IsBlank()), Value));

        public static IDictionary<TKey, TValue> SetOrRemove<TKey, TValue, TK, TV>(this IDictionary<TKey, TValue> Dic, TK Key, TV Value)
        {
            if (Dic != null && Key != null)
            {
                if (Value != null)
                {
                    Dic[Key.ChangeType<TKey>()] = Value.ChangeType<TValue>();
                }
                else
                {
                    Dic.RemoveIfExist(Key.ChangeType<TKey>());
                }
            }

            return Dic;
        }

        /// <summary>
        /// Seta o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <param name="PropertyName">Nome da properiedade</param>
        /// <param name="Value">Valor da propriedade definida por <paramref name="PropertyName"/></param>
        /// <typeparam name="T">
        /// Tipo do <paramref name="Value"/> da propriedade definida por <paramref name="PropertyName"/>
        /// </typeparam>
        public static T SetPropertyValue<T>(this T MyObject, string PropertyName, object Value) where T : class
        {
            if (PropertyName.IsNotBlank() && MyObject != null)
            {
                var props = MyObject.GetProperties();

                var prop = props.FirstOrDefault(p => p != null && p.CanWrite && p.Name.IsAny(PropertyNamesFor(PropertyName).ToArray()));

                if (prop != null)
                    if (Value is DBNull)
                    {
                        prop.SetValue(MyObject, null);
                    }
                    else
                    {
                        prop.SetValue(MyObject, Converter.ChangeType(Value, prop.PropertyType));
                    }
            }

            return MyObject;
        }

        public static T SetPropertyValue<T, TProp>(this T obj, Expression<Func<T, TProp>> Selector, TProp Value) where T : class
        {
            obj?.SetPropertyValue(obj.GetPropertyInfo(Selector).Name, Value);
            return obj;
        }

        public static Dictionary<TGroup, Dictionary<TCount, long>> SkipZero<TGroup, TCount>(this Dictionary<TGroup, Dictionary<TCount, long>> Grouped)
        {
            if (Grouped != null)
            {
                foreach (var dic in Grouped.ToArray())
                {
                    Grouped[dic.Key] = dic.Value.Where(x => x.Value > 0).ToDictionary();
                }

                Grouped = Grouped.Where(x => x.Value.Any()).ToDictionary();
            }

            return Grouped;
        }

        public static Dictionary<TCount, long> SkipZero<TCount>(this Dictionary<TCount, long> Grouped)
        {
            Grouped = Grouped?.Where(x => x.Value > 0).ToDictionary();
            return Grouped;
        }

        /// <summary>
        /// Troca o valor de <paramref name="FirstValue"/> pelo valor de <paramref
        /// name="SecondValue"/> e o valor de <paramref name="SecondValue"/> pelo valor de <paramref name="FirstValue"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstValue"></param>
        /// <param name="SecondValue"></param>
        public static (T, T) Swap<T>(ref T FirstValue, ref T SecondValue)
        {
            (SecondValue, FirstValue) = (FirstValue, SecondValue);
            return (FirstValue, SecondValue);
        }

        /// <summary>
        /// Traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> TakeTop<TKey, TValue>(this Dictionary<TKey, TValue> Dic, int Top, TKey GroupOthersLabel)
        {
            if (Dic == null)
            {
                return null;
            }

            if (Top < 1)
            {
                return Dic.ToDictionary();
            }

            var novodic = Dic.Take(Top).ToDictionary();
            if (GroupOthersLabel != null)
            {
                novodic[GroupOthersLabel] = Dic.Values.Skip(Top).Select(x => x.ChangeType<decimal>()).Sum().ChangeType<TValue>();
            }

            return novodic;
        }

        /// <summary>
        /// Traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<TKey, IEnumerable<T>> TakeTop<TKey, T>(this Dictionary<TKey, IEnumerable<T>> Dic, int Top, Expression<Func<T, dynamic>> ValueSelector) where T : class
        {
            Dictionary<TKey, IEnumerable<T>> novodic = Dic.ToDictionary();

            if (ValueSelector != null)
            {
                novodic = Dic.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(ValueSelector.Compile()).AsEnumerable());
            }

            if (Top > 0)
            {
                novodic = Dic.ToDictionary(x => x.Key, x => x.Value.TakeTop(Top, ValueSelector));
            }

            return novodic;
        }

        public static IEnumerable<T> TakeTop<T>(this IEnumerable<T> List, int Top, params Expression<Func<T, dynamic>>[] ValueSelector) where T : class => TakeTop<T, object>(List, Top, null, null, ValueSelector?.ToArray());

        public static IEnumerable<T> TakeTop<T, TLabel>(this IEnumerable<T> List, int Top, Expression<Func<T, TLabel>> LabelSelector, TLabel GroupOthersLabel, params Expression<Func<T, dynamic>>[] ValueSelector) where T : class
        {
            ValueSelector = ValueSelector ?? Array.Empty<Expression<Func<T, dynamic>>>();

            if (ValueSelector.WhereNotNull().IsNullOrEmpty())
            {
                throw new ArgumentException("You need at least one value selector", nameof(ValueSelector));
            }

            var newlist = List.OrderByManyDescending(ValueSelector).Take(Top).ToList();

            if (LabelSelector != null && GroupOthersLabel != null)
            {
                var others = Activator.CreateInstance<T>();
                LabelSelector.GetPropertyInfo().SetValue(others, GroupOthersLabel);
                foreach (var v in ValueSelector)
                {
                    var values = List.Skip(Top).Select(x => (v.Compile().Invoke(x) as object).ChangeType<decimal>()).Sum();
                    v.GetPropertyInfo().SetValue(others, values);
                }
                newlist.Add(others);
            }
            return newlist.AsEnumerable();
        }

        public static T[,] To2D<T>(this T[][] source)
        {
            int FirstDim = source.Length;
            int SecondDim = source.GroupBy(row => row.Length).Max().Key;

            var result = new T[FirstDim, SecondDim];
            for (int i = 0; i < FirstDim; ++i)
                for (int j = 0; j < SecondDim; ++j)
                    result[i, j] = source[i].IfNoIndex(j);

            return result;
        }

        public static Attachment ToAttachment(this FileInfo file) => file != null && file.Exists ? new Attachment(file.FullName) : null;

        public static Attachment ToAttachment(this Stream stream, string name) => stream != null && stream.Length > 0 ? new Attachment(stream, name.IfBlank("untitledFile.bin")) : null;

        public static Attachment ToAttachment(this byte[] bytes, string name)
        {
            if (bytes != null && bytes.Any())
                using (var s = new MemoryStream(bytes))
                    return s.ToAttachment(name);
            return null;
        }

        /// <summary>
        /// Concatena todas as <see cref="Exception.InnerException"/> em uma única string
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToFullExceptionString(this Exception ex, string Separator = " => ") => ex.Traverse(x => x.InnerException).SelectJoinString(x => x.Message, Separator);

        /// <summary>
        /// Alterna uma variavel ente 2 valores diferentes
        /// </summary>
        /// <param name="Current">Objeto contendo o primeiro ou segundo valor</param>
        /// <param name="TrueValue">Primeiro valor</param>
        /// <param name="FalseValue">Segundo Valor</param>
        public static T Toggle<T>(this T Current, T TrueValue, T FalseValue = default) => Current.Equals(TrueValue) ? FalseValue : TrueValue;

        public static T[][] ToJaggedArray<T>(this T[,] inputArray)
        {
            // Get the number of rows and columns in the input array
            int rows = inputArray.GetLength(0);
            int cols = inputArray.GetLength(1);

            // Create the jagged array with the same number of rows as the input array
            T[][] jaggedArray = new T[rows][];

            // Copy the elements from the input array to the jagged array
            for (int i = 0; i < rows; i++)
            {
                // Create a new sub-array for each row
                jaggedArray[i] = new T[cols];

                // Copy the elements from the input array to the jagged array
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = inputArray[i, j];
                }
            }

            return jaggedArray;
        }

        /// <summary>
        /// Retorna um dicionário em QueryString
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> Dic) => Dic?.Where(x => x.Key.IsNotBlank()).SelectJoinString(x => new[] { x.Key, (x.Value ?? InnerLibs.Text.Empty).UrlEncode() }.SelectJoinString("="), "&") ?? InnerLibs.Text.Empty;

        /// <summary>
        /// Retorna um <see cref="NameValueCollection"/> em QueryString
        /// </summary>
        /// <param name="NVC"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection NVC) => NVC?.AllKeys.SelectManyJoinString(n => NVC.GetValues(n).Select(v => n + "=" + v).Where(x => x.IsNotBlank() && x != "="), "&");

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
        /// </summary>
        /// <typeparam name="TGroupKey"></typeparam>
        /// <typeparam name="TSubGroupKey"></typeparam>
        /// <typeparam name="TSubGroupValue"></typeparam>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static IEnumerable<object> ToTableArray<TGroupKey, TSubGroupKey, TSubGroupValue, THeaderProperty>(this Dictionary<TGroupKey, Dictionary<TSubGroupKey, TSubGroupValue>> Groups, Func<TSubGroupKey, THeaderProperty> HeaderProp)
        {
            var lista = new List<object>();
            var header = new List<object>
            {
                HeaderProp.Method.GetParameters().First().Name
            };
            Groups.Values.MergeKeys();
            foreach (var h in Groups.SelectMany(x => x.Value.Keys.ToArray()).Distinct().OrderBy(x => x))
            {
                header.Add(HeaderProp(h));
            }

            lista.Add(header);
            lista.AddRange(Groups.Select(x =>
            {
                var l = new List<object>
                {
                    x.Key // GroupKey
                };
                foreach (var item in x.Value.OrderBy(k => k.Key).Select(v => v.Value))
                {
                    l.Add(item); // SubGroupValue
                }

                return l;
            }));
            return lista;
        }

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de
        /// arrays formando uma tabela
        /// </summary>
        public static IEnumerable<object[]> ToTableArray<TGroupKey, TGroupValue>(this Dictionary<TGroupKey, TGroupValue> Groups) => Groups.Select(x => new List<object> { x.Key, x.Value }.ToArray());

        /// <summary>
        /// Run a <see cref="Action"/> inside a Try-catch block and return a <see cref="Exception"/>
        /// if fail
        /// </summary>
        /// <param name="action"></param>
        /// <returns>
        /// A null <see cref="Exception"/> if <paramref name="action"/> runs successfully, otherwise
        /// the captured <see cref="Exception"/>
        /// </returns>
        public static Exception TryExecute(Action action)
        {
            try
            {
                action?.Invoke();
                return null;
            }
            catch (Exception exx)
            {
                return exx;
            }
        }

        ///<inheritdoc cref="With{T}(T, Action{T}, out Exception)"/>
        public static T With<T>(this T Obj, Action<T> Callback) => With(Obj, Callback, out _);

        /// <summary>
        /// Run a <see cref="Action{T}"/> inside a Try-Catch block and return the same <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Obj">Object</param>
        /// <param name="Callback">The action to execute</param>
        /// <param name="ex">
        /// An out param to capture a <see cref="Exception"/> if <paramref name="Callback"/> fails
        /// </param>
        /// <returns>The same <paramref name="Obj"/></returns>
        public static T With<T>(this T Obj, Action<T> Callback, out Exception ex)
        {
            ex = TryExecute(() => Callback?.Invoke(Obj));
            return Obj;
        }

        #endregion Public Methods
    }
}