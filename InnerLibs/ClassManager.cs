using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace InnerLibs
{
    public static class ClassTools
    {
        public static IEnumerable<Type> GetInheritedClasses<T>() where T : class => GetInheritedClasses(typeof(T));

        public static IEnumerable<Type> GetInheritedClasses(this Type MyType)
        {
            //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType));
        }

        public static IEnumerable<string> PropertyNamesFor(this string Name)
        {
            string propname1 = Name.Trim().Replace(" ", "_").Replace("-", "_").Replace("~", "_");
            string propname3 = Name.Trim().Replace(" ", "").Replace("-", "").Replace("~", "");
            string propname2 = propname1.RemoveAccents();
            string propname4 = propname3.RemoveAccents();
            var propnames = new[] { Name, propname1, propname2, propname3, propname4 }.ToList();
            propnames.AddRange(propnames.Select(x => $"_{x}").ToArray());
            return propnames.Distinct();
        }

        /// <summary>
        /// Metodo de extensão para utilizar qualquer objeto usando FluentAPI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        public static T With<T>(this T Obj, Action<T> Callback)
        {
            if (Obj != null && Callback != null)
            {
                Callback(Obj);
            }

            return Obj;
        }

        /// <summary>
        /// Substitui todas as propriedades nulas de uma classe pelos seus valores Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static T NullPropertiesAsDefault<T>(this T Obj, bool IncludeVirtual = false) where T : class
        {
            if (Obj != null)
            {
                foreach (var item in Obj.GetProperties())
                {
                    if (item.CanRead && item.CanWrite && item.GetValue(Obj) is null)
                    {
                        switch (item.PropertyType)
                        {
                            case var @case when @case == typeof(string):
                                {
                                    item.SetValue(Obj, "");
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
            }

            return Obj;
        }

        /// <summary>
        /// Adciona ou substitui um valor a este <see cref="Dictionary(Of TKey, TValue)"/> e retorna a mesma instancia deste <see cref="Dictionary(Of TKey, TValue)"/>
        /// </summary>
        /// <typeparam name="KeyType">Tipo da Key</typeparam>
        /// <typeparam name="ValueType">Tipo do valor</typeparam>
        /// <param name="Key">Valor da key</param>
        /// <param name="Value">Valor do Value</param>
        /// <returns>o mesmo objeto do tipo <see cref="Dictionary"/> que chamou este método</returns>
        public static IDictionary<KeyType, ValueType> Set<KeyType, ValueType, KT, VT>(this IDictionary<KeyType, ValueType> Dic, KT Key, VT Value)
        {
            if (Key != null)
            {
                Dic[Key.ChangeType<KeyType, KT>()] = Value.ChangeType<ValueType, VT>();
            }

            return Dic;
        }

        public static IDictionary<KeyType, string> SetOrRemove<KeyType, KT>(this IDictionary<KeyType, string> Dic, KT Key, string Value, bool NullIfBlank) => Dic.SetOrRemove(Key, NullIfBlank ? Value.NullIf(x => x.IsBlank()) : Value);

        public static IDictionary<KeyType, ValueType> SetOrRemove<KeyType, ValueType, KT, VT>(this IDictionary<KeyType, ValueType> Dic, KT Key, VT Value)
        {
            if (Key != null)
            {
                if (Value != null)
                {
                    Dic[Key.ChangeType<KeyType, KT>()] = Value.ChangeType<ValueType, VT>();
                }
                else
                {
                    Dic.RemoveIfExist(Key.ChangeType<KeyType, KT>());
                }
            }

            return Dic;
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> List) => (List ?? Array.Empty<T>()).Any();

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> List) => !List.IsNotNullOrEmpty();

        public static List<T> RemoveLast<T>(this List<T> List, int Count = 1)
        {
            for (int index = 1, loopTo = Count; index <= loopTo; index++)
            {
                if (List != null && List.Any())
                {
                    List.RemoveAt(List.Count - 1);
                }
            }

            return List;
        }

        public static bool IsEqual<T>(this T Value1, T Value2) where T : IComparable
        {
            return Value1.Equals(Value2);
        }

        public static bool IsGreaterThan<T>(this T Value1, T Value2) where T : IComparable
        {
            return Value1.CompareTo(Value2) > 0;
        }

        public static bool IsGreaterThanOrEqual<T>(this T Value1, T Value2) where T : IComparable
        {
            return Value1.IsGreaterThan(Value2) || Value1.IsEqual(Value2);
        }

        public static bool IsLessThan<T>(this T Value1, T Value2) where T : IComparable
        {
            return Value1.CompareTo(Value2) < 0;
        }

        public static bool IsLessThanOrEqual<T>(this T Value1, T Value2) where T : IComparable
        {
            return Value1.IsLessThan(Value2) || Value1.IsEqual(Value2);
        }

        /// <summary>
        /// Verifica se um valor numerico ou data está entre outros 2 valores
        /// </summary>
        /// <param name="Value">      Numero</param>
        /// <param name="Value1"> Primeiro numero comparador</param>
        /// <param name="Value2">Segundo numero comparador</param>
        /// <returns></returns>
        public static bool IsBetween(this IComparable Value, IComparable Value1, IComparable Value2)
        {
            FixOrder(ref Value1, ref Value2);
            if (Value1 == Value2) return Value == Value1;
            else return Value.IsLessThan(Value2) && Value.IsGreaterThan(Value1);
        }

        /// <summary>
        /// Verifica se um valor numerico ou data está entre outros 2 valores
        /// </summary>
        /// <param name="Value">      Numero</param>
        /// <param name="Value1"> Primeiro numero comparador</param>
        /// <param name="Value2">Segundo numero comparador</param>
        /// <returns></returns>
        public static bool IsBetweenOrEqual(this IComparable Value, IComparable Value1, IComparable Value2)
        {
            FixOrder(ref Value1, ref Value2);
            if (Value1 == Value2) return Value == Value1;
            else return Value.IsLessThanOrEqual(Value2) && Value.IsGreaterThanOrEqual(Value1);
        }

        /// <summary>
        /// Troca ou não a ordem das variaveis de inicio e fim  fazendo com que a Value1
        /// sempre seja menor que a Value2. Util para tratar ranges
        /// </summary>

        public static void FixOrder<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue != null && SecondValue != null)
            {
                if (FirstValue.IsGreaterThan(SecondValue))
                {
                    Swap(ref FirstValue, ref SecondValue);
                }
            }
        }

        /// <summary>
        /// Troca o valor de <paramref name="FirstValue"/> pelo valor de <paramref name="SecondValue"/> e o  valor de <paramref name="SecondValue"/> pelo valor de <paramref name="FirstValue"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstValue"></param>
        /// <param name="SecondValue"></param>
        public static void Swap<T>(ref T FirstValue, ref T SecondValue)
        {
            var temp = FirstValue;
            FirstValue = SecondValue;
            SecondValue = temp;
        }

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
        /// Cria um arquivo a partir de qualquer objeto usando o <see cref="ClassTools.CreateXML()"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FileInfo CreateXmlFile(this object obj, string FilePath) => obj.CreateXML().ToXMLString().WriteToFile(FilePath);

        /// <summary>
        /// Cria um <see cref="Guid"/> a partir de uma string ou um novo <see cref="Guid"/> se a conversão falhar
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Guid CreateGuidOrDefault(this string Source)
        {
            var g = Guid.NewGuid();
            if (Source.IsNotBlank())
            {
                if (!Guid.TryParse(Source, out g))
                {
                    g = Guid.NewGuid();
                }
            }

            return g;
        }

        /// <summary>
        /// Concatena todas as  <see cref="Exception.InnerException"/> em uma única string
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToFullExceptionString(this Exception ex, string Separator = " >> ") => ex.Traverse(x => x.InnerException).SelectJoinString(x => x.Message, Separator);

        /// <summary>
        /// Retorna um dicionário em QueryString
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> Dic)
        {
            if (Dic != null)
            {
                return Dic.Where(x => x.Key.IsNotBlank()).SelectJoinString(x => new[] { x.Key, (x.Value ?? "").UrlEncode() }.JoinString("="), "&");
            }

            return "";
        }

        /// <summary>
        /// Retorna um <see cref="NameValueCollection"/> em QueryString
        /// </summary>
        /// <param name="NVC"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection NVC) => NVC.AllKeys.SelectManyJoinString(n => NVC.GetValues(n).Select(v => n + "=" + v).Where(x => x.IsNotBlank() && x != "="), "&");

        /// <summary>
        /// Remove um item de uma lista e retorna este item
        /// </summary>
        /// <typeparam name="T">Tipo do item</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="Index">Posicao do item</param>
        /// <returns></returns>
        public static T Detach<T>(this List<T> List, int Index)
        {
            var p = List.IfNoIndex(Index, default);
            if (p != null)
            {
                List.RemoveAt(Index);
            }

            return p;
        }

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
        /// </summary>
        /// <typeparam name="GroupKey"></typeparam>
        /// <typeparam name="SubGroupKey"></typeparam>
        /// <typeparam name="SubGroupValue"></typeparam>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static IEnumerable<object> ToTableArray<GroupKey, SubGroupKey, SubGroupValue, HeaderProperty>(this Dictionary<GroupKey, Dictionary<SubGroupKey, SubGroupValue>> Groups, Func<SubGroupKey, HeaderProperty> HeaderProp)
        {
            var lista = new List<object>();
            var header = new List<object>
            {
                HeaderProp.Method.GetParameters().First().Name
            };
            Groups.Values.MergeKeys();
            foreach (var h in Groups.SelectMany(x => x.Value.Keys.ToArray()).Distinct().OrderBy(x => x))
                header.Add(HeaderProp(h));
            lista.Add(header);
            lista.AddRange(Groups.Select(x =>
            {
                var l = new List<object>
                {
                    x.Key // GroupKey
                };
                foreach (var item in x.Value.OrderBy(k => k.Key).Select(v => v.Value))
                    l.Add(item); // SubGroupValue
                return l;
            }));
            return lista;
        }

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
        /// </summary>
        public static object ToTableArray<GroupKeyType, GroupValueType>(this Dictionary<GroupKeyType, GroupValueType> Groups)
        {
            return Groups.Select(x =>
            {
                var l = new List<object>
                {
                    x.Key,
                    x.Value
                };
                return l.ToArray();
            });
        }

        /// <summary>
        /// Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada grupo a partir de outra propriedade do mesmo objeto
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
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, long> GroupAndCountBy<Type, Group>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector)
        {
            return obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, long>(x.Key, x.LongCount())).ToDictionary();
        }

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, long> GroupFirstAndCountBy<Type, Group>(this IEnumerable<Type> obj, int First, Func<Type, Group> GroupSelector, Group OtherLabel)
        {
            var grouped = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, long>(x.Key, x.LongCount())).OrderByDescending(x => x.Value);
            return grouped.Take(First).Union(new[] { new KeyValuePair<Group, long>(OtherLabel, grouped.Skip(First).Sum(s => s.Value)) }).ToDictionary();
        }

        /// <summary>
        /// Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um grupo com subgrupos daquele objeto
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
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BoolExp">Expressão de teste de Valor boolean</param>
        /// <param name="TrueValue"> Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static R AsIf<T, R>(this T obj, Expression<Func<T, bool>> BoolExp, R TrueValue, R FalseValue = default)
        {
            if (obj == null || BoolExp == null)
            {
                return FalseValue;
            }
            else
            {
                return BoolExp.Compile()(obj).AsIf(TrueValue, FalseValue);
            }
        }

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">      Valor boolean</param>
        /// <param name="TrueValue"> Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool Bool, T TrueValue, T FalseValue = default) => Bool ? TrueValue : FalseValue;

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">      Valor boolean</param>
        /// <param name="TrueValue"> Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue = default) => (Bool.HasValue && Bool.Value).AsIf(TrueValue, FalseValue);

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que
        /// possuir um valor
        /// </summary>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">    Outros itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(this string First, params string[] N) => BlankCoalesce(new[] { First }.Union(N ?? Array.Empty<string>()).ToArray());

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que
        /// possuir um valor
        /// </summary>
        /// <param name="N">Itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(params string[] N) => (N ?? Array.Empty<string>()).FirstOr(x => x.IsNotBlank(), "");

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
                if (Comparer is null)
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return false;
                    }
                }
                else if (!(List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEqualityComparer<T> Comparer, params T[] List2) => List1.ContainsAll((List2 ?? Array.Empty<T>()).AsEnumerable(), Comparer);

        /// <summary>
        /// Verifica se somente um unico elemento corresponde a condição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool OnlyOneOf<T>(this IEnumerable<T> List, Func<T, bool> predicate) => List?.Count(predicate) == 1;

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
        /// Converte uma classe para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="Type">Tipo da classe</typeparam>
        /// <param name="Obj">Object</param>
        /// <returns></returns>
        public static Dictionary<string, object> CreateDictionary<Type>(this Type Obj) => Obj != null ? typeof(Type).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(Obj, null)) : new Dictionary<string, object>();

        /// <summary>
        /// Converte uma classe para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="Type">Tipo da classe</typeparam>
        /// <param name="Obj">Object</param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> CreateDictionaryEnumerable<Type>(this IEnumerable<Type> Obj) => (Obj ?? Array.Empty<Type>()).Select(x => x.CreateDictionary());

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao
        /// </summary>
        /// <typeparam name="Type">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<Type, long> DistinctCount<Type>(this IEnumerable<Type> Arr) => Arr.Distinct().Select(p => new KeyValuePair<Type, long>(p, Arr.Where(x => x.Equals(p)).LongCount())).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao e agrupa o resto
        /// </summary>
        /// <typeparam name="Type">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<Type, long> DistinctCountTop<Type>(this IEnumerable<Type> Arr, int Top, Type Others)
        {
            var a = Arr.DistinctCount();
            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        /// <summary>
        /// traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<K, T> TakeTop<K, T>(this IDictionary<K, T> Dic, int Top, K GroupOthersLabel) where T : IConvertible
        {
            if (Top < 1)
                return (Dictionary<K, T>)Dic;
            var novodic = Dic.Take(Top).ToDictionary();
            if (GroupOthersLabel != null)
            {
                novodic[GroupOthersLabel] = Dic.Values.Skip(Top).Select(x => x.ChangeType<decimal, T>()).Sum().ChangeType<T, decimal>();
            }

            return novodic;
        }

        /// <summary>
        /// traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<K, IEnumerable<T>> TakeTop<K, T>(this IDictionary<K, IEnumerable<T>> Dic, int Top, K GroupOthersLabel)
        {
            if (Top < 1)
                return (Dictionary<K, IEnumerable<T>>)Dic;
            var novodic = Dic.Take(Top).ToDictionary();
            if (GroupOthersLabel != null)
            {
                novodic[GroupOthersLabel] = (IEnumerable<T>)Dic.Values.Skip(Top).SelectMany(x => x).Select(x => x.ChangeType<decimal, T>()).Sum().ChangeType<T, decimal>();
            }

            return novodic;
        }

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<PropT, long> DistinctCount<Type, PropT>(this IEnumerable<Type> Arr, Func<Type, PropT> Prop)
        {
            return Arr.GroupBy(Prop).ToDictionary(x => x.Key, x => x.LongCount()).OrderByDescending(p => p.Value).ToDictionary();
        }

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao a partir de uma propriedade e agrupa o resto em outra
        /// </summary>
        /// <typeparam name="Type">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<PropT, long> DistinctCountTop<Type, PropT>(this IEnumerable<Type> Arr, Func<Type, PropT> Prop, int Top, PropT Others)
        {
            var a = Arr.DistinctCount(Prop);
            if (Top < 1)
                return a;
            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        /// <summary>
        /// Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">   </param>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public static T FirstOr<T>(this IEnumerable<T> source, T Alternate)
        {
            var item = (source ?? Array.Empty<T>()).FirstOrDefault();
            if (item == null)
            {
                return Alternate;
            }
            return item;
        }

        /// <summary>
        /// Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">   </param>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public static T FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicate, T Alternate)
        {
            var item = (source ?? Array.Empty<T>()).FirstOrDefault(predicate);
            return item == null ? Alternate : item;
        }

        /// <summary>
        /// O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAny<T>(this IEnumerable<T> source, params Expression<Func<T, bool>>[] predicate)
        {
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
        /// O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
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

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> ValueSelector) where TAttribute : Attribute
        {
            TAttribute att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return ValueSelector(att);
            }

            return default;
        }

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attribType"></param>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo target, Type attribType)
        {
            return target?.GetCustomAttributes(attribType, false).Any() == true;
        }

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo target)
        {
            return target?.HasAttribute(typeof(T)) == true;
        }

        /// <summary>
        /// Traz o valor de uma enumeração a partir de uma string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetEnumValue<T>(string Name)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            var val = ((T[])Enum.GetValues(typeof(T)))[0];
            if (!string.IsNullOrEmpty(Name))
            {
                foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
                {
                    if (enumValue.ToString().ToUpper().Equals(Name.ToUpper()))
                    {
                        val = enumValue;
                        break;
                    }
                }
            }

            return val;
        }

        /// <summary>
        /// Traz o valor de uma enumeração a partir de uma string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this T Value)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return Enum.GetName(typeof(T), Value);
        }

        /// <summary>
        /// Traz todos os Valores de uma enumeração
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetEnumValues<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }

            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<O>(this O MyObject, BindingFlags BindAttr)
        {
            return MyObject.GetTypeOf().GetProperties(BindAttr).ToList();
        }

        public static IEnumerable<FieldInfo> GetFields<O>(this O MyObject, BindingFlags BindAttr)
        {
            return MyObject.GetTypeOf().GetFields(BindAttr).ToList();
        }

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<O>(this O MyObject)
        {
            return MyObject.GetTypeOf().GetProperties().ToList();
        }

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFields<O>(this O MyObject)
        {
            return MyObject.GetTypeOf().GetFields().ToList();
        }

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<O>(this O MyObject, string Name)
        {
            return MyObject.GetTypeOf().GetProperties().SingleOrDefault(x => (x.Name ?? "") == (Name ?? ""));
        }

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static FieldInfo GetField<O>(this O MyObject, string Name)
        {
            return MyObject.GetTypeOf().GetFields().SingleOrDefault(x => (x.Name ?? "") == (Name ?? ""));
        }

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static T GetPropertyValue<T, O>(this O MyObject, string Name)
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
        public static byte[] GetResourceBytes(this Assembly Assembly, string FileName)
        {
            var resourceStream = Assembly.GetManifestResourceStream(FileName);
            if (resourceStream is null)
            {
                return null;
            }

            var fontBytes = new byte[(int)(resourceStream.Length - 1L + 1)];
            resourceStream.Read(fontBytes, 0, (int)resourceStream.Length);
            resourceStream.Close();
            return fontBytes;
        }

        /// <summary>
        /// Pega o texto de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
        /// <returns></returns>
        public static string GetResourceFileText(this Assembly Assembly, string FileName)
        {
            string txt = null;
            if (Assembly != null && FileName.IsNotBlank())
                using (var x = Assembly.GetManifestResourceStream(FileName))
                {
                    if (x != null)
                        using (var r = new StreamReader(x))
                        {
                            txt = r.ReadToEnd();
                        };
                }
            return txt;
        }

        public static TValue GetValueOr<TKey, TValue>(this IDictionary<TKey, TValue> Dic, TKey Key, TValue ReplaceValue = default)
        {
            if (Dic != null && Dic.ContainsKey(Key))
            {
                return Dic[Key];
            }
            else
            {
                return ReplaceValue;
            }
        }

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Type">        </param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type Type, string PropertyName, bool GetPrivate = false)
        {
            if (PropertyName.IsNotBlank())
            {
                var parts = new List<string>();
                bool stop = false;
                string current = "";
                for (int i = 0, loopTo = PropertyName.Length - 1; i <= loopTo; i++)
                {
                    if (PropertyName[i] != '.')
                        current += Convert.ToString(PropertyName[i]);
                    if (PropertyName[i] == '(')
                        stop = true;
                    if (PropertyName[i] == ')')
                        stop = false;
                    if (PropertyName[i] == '.' && !stop || i == PropertyName.Length - 1)
                    {
                        parts.Add(current.ToString());
                        current = "";
                    }
                }

                PropertyInfo prop;
                string propname = parts.First().GetBefore("(");
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
        /// <param name="Obj"> </param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool HasProperty(this object Obj, string Name)
        {
            return Obj.GetType().HasProperty(Name, true);
        }

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this Type Type)
        {
            return Type == typeof(T[]);
        }

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this object Obj)
        {
            return Obj.GetType().IsArrayOf<T>();
        }

        /// <summary>
        /// Verifica se o objeto é um iDictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDictionary(this object obj)
        {
            return obj.GetTypeOf().IsGenericOf(typeof(Dictionary<,>));
        }

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj"> objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<Type>(this Type Obj, params Type[] List)
        {
            return Obj.IsIn((List ?? Array.Empty<Type>()).ToList());
        }

        public static bool IsAny<Type>(this Type Obj, params Type[] List) => IsIn(Obj, List);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj"> objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<Type>(this Type Obj, IEnumerable<Type> List, IEqualityComparer<Type> Comparer = null)
        {
            if (Comparer is null)
            {
                return List.Contains(Obj);
            }
            else
            {
                return List.Contains(Obj, Comparer);
            }
        }

        /// <summary>
        /// Verifica se o objeto existe dentro de um texto
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj"> objeto</param>
        /// <param name="TExt">Texto</param>
        /// <returns></returns>
        public static bool IsIn<Type>(this Type Obj, string Text, IEqualityComparer<char> Comparer = null)
        {
            if (Comparer is null)
            {
                return Text.Contains(Obj.ToString());
            }
            else
            {
                return Text.Contains(Convert.ToChar(Obj.ToString()), Comparer);
            }
        }

        /// <summary>
        /// Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj"> objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsInAny<Type>(this Type Obj, IEnumerable<Type>[] List, IEqualityComparer<Type> Comparer = null)
        {
            return (List ?? Array.Empty<IEnumerable<Type>>()).Any(x => Obj.IsIn(x, Comparer));
        }

        /// <summary>
        /// Verifica se um tipo e generico de outro
        /// </summary>
        /// <param name="MainType"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsGenericOf(this Type MainType, Type Type)
        {
            return MainType.IsGenericType && MainType.GetGenericTypeDefinition().IsAssignableFrom(Type);
        }

        /// <summary>
        /// Verifica se o objeto é uma lista
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsList(this object obj)
        {
            return obj.GetTypeOf().IsGenericOf(typeof(List<>));
        }

        /// <summary>
        /// Verifica se o objeto é uma lista
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this object obj)
        {
            return obj.GetTypeOf().IsGenericOf(typeof(IEnumerable<>));
        }

        /// <summary>
        /// Verifica se o não objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj"> objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsNotIn<Type>(this Type Obj, IEnumerable<Type> List, IEqualityComparer<Type> Comparer = null)
        {
            if (Comparer is null)
            {
                return !List.Contains(Obj);
            }
            else
            {
                return !List.Contains(Obj, Comparer);
            }
        }

        /// <summary>
        /// Verifica se o objeto não existe dentro de um texto
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto</typeparam>
        /// <param name="Obj"> objeto</param>
        /// <param name="TExt">Texto</param>
        /// <returns></returns>
        public static bool IsNotIn<Type>(this Type Obj, string Text, IEqualityComparer<char> Comparer = null)
        {
            if (Comparer == null)
            {
                return Text.Contains(Obj.ToString());
            }
            else
            {
                return Text.Contains(Convert.ToChar(Obj.ToString()), Comparer);
            }
        }

        public static bool IsValueType(this Type T) => T.IsIn(PredefinedArrays.ValueTypes);

        public static bool IsValueType<T>(this T Obj) => Obj.GetNullableTypeOf().IsValueType();

        /// <summary>
        /// Verifica se o objeto é do tipo numérico.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumericType<T>(this T Obj)
        {
            return Obj.GetNullableTypeOf().IsIn(PredefinedArrays.NumericTypes);
        }

        public static bool IsNullableType(this Type t)
        {
            return t.IsGenericType && Nullable.GetUnderlyingType(t) != null;
        }

        public static bool IsNullableType<O>(this O Obj)
        {
            var t = Obj.GetTypeOf();
            return IsNullableType(t);
        }

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

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<O, T>(this O Obj) => Obj.IsNullableTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<O>(this O Obj, Type Type) => Obj.GetNullableTypeOf() == Type.GetNullableTypeOf();

        /// <summary>
        /// Retorna o <see cref="Type"/> do objeto mesmo se ele for nulo
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>o tipo do objeto ou o prorio objeto se ele for um <see cref="Type"/></returns>
        public static Type GetTypeOf<O>(this O Obj)
        {
            if (typeof(O) == typeof(Type))
            {
                return (Type)(object)Obj;
            }
            else
            {
                return Obj?.GetType() ?? typeof(O);
            }
        }

        /// <summary>
        /// Retorna o <see cref="Type"/> equivalente a <typeparamref name="T"/>   ou o <see cref="Type"/> do objeto <see cref="Nullable(Of T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>o tipo do objeto ou o tipo do objeto anulavel ou o prorio objeto se ele for um <see cref="Type"/></returns>
        public static Type GetNullableTypeOf<T>(this T Obj)
        {
            var tt = Obj.GetTypeOf();
            tt = Nullable.GetUnderlyingType(tt) ?? tt;
            return tt;
        }

        /// <summary>
        /// Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">   </param>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public static T LastOr<T>(this IEnumerable<T> source, params T[] Alternate)
        {
            return source?.Any() ?? false ? source.Last() : Alternate.AsEnumerable().NullCoalesce();
        }

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="NVC"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(params NameValueCollection[] NVC)
        {
            var all = new NameValueCollection();
            foreach (var i in NVC)
                all.Add(i);
            return all;
        }

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">    Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this T? First, params T?[] N) where T : struct
        {
            var l = new List<T?>();
            l.Add(First);
            l.AddRange(N);
            if (First != null)
                return (T)First;
            return N.NullCoalesce<T>();
        }

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
        /// <param name="N">    Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this T First, params T[] N) where T : class
        {
            if (First != null) return First;
            return NullCoalesce((N ?? Array.Empty<T>()).AsEnumerable());
        }

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this IEnumerable<T> List)
        {
            if (List == null) return default;
            return List.FirstOrDefault(x => x != null);
        }

        /// <summary>
        /// Remove de um dicionario as respectivas Keys se as mesmas existirem
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"> </param>
        /// <param name="Keys"></param>
        public static void RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, params TKey[] Keys)
        {
            foreach (var k in Keys)
            {
                if (dic.ContainsKey(k))
                {
                    dic.Remove(k);
                }
            }
        }

        /// <summary>
        /// Remove de um dicionario os valores encontrados pelo predicate
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic">      </param>
        /// <param name="predicate"></param>
        public static void RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<KeyValuePair<TKey, TValue>, bool> predicate) => dic.RemoveIfExist(dic.Where(predicate).Select(x => x.Key).ToArray());

        /// <summary>
        /// Seta o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">    Objeto</param>
        /// <param name="PropertyName">Nome da properiedade</param>
        /// <param name="Value">       Valor da propriedade definida por <paramref name="PropertyName"/></param>
        /// <typeparam name="Type">
        /// Tipo do <paramref name="Value"/> da propriedade definida por <paramref name="PropertyName"/>
        /// </typeparam>
        public static Type SetPropertyValue<Type>(this Type MyObject, string PropertyName, object Value)
        {
            var prop = MyObject.GetProperties().Where(p => (p.Name ?? "") == (PropertyName ?? "")).FirstOrDefault();
            if (prop != null && prop.CanWrite)
            {
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

        public static Type SetPropertyValue<Type, Prop>(this Type obj, Expression<Func<Type, Prop>> Selector, Prop Value) where Type : class
        {
            obj.GetPropertyInfo(Selector).SetValue(obj, Value);
            return obj;
        }

        /// <summary>
        /// Alterna uma variavel ente 2 valores diferentes
        /// </summary>
        /// <param name="Current">Objeto contendo o primeiro ou segundo valor</param>
        /// <param name="TrueValue">Primeiro valor</param>
        /// <param name="FalseValue">Segundo Valor</param>
        public static T Toggle<T>(this T Current, T TrueValue, T FalseValue = default) => Current.Equals(TrueValue) ? FalseValue : TrueValue;
    }
}