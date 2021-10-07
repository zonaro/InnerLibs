using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using InnerLibs.LINQ;
using Microsoft.VisualBasic;

namespace InnerLibs
{
    public static class Converter
    {

        /// <summary>
        /// Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjectForDefinition">Objeto que definirá o tipo da lista</param>
        /// <returns></returns>
        public static List<T> DefineEmptyList<T>(this T ObjectForDefinition)
        {
            return DefineEmptyList<T>();
        }

        /// <summary>
        /// Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> DefineEmptyList<T>()
        {
            return new List<T>();
        }

        /// <summary>
        /// Cria uma e adciona um objeto a ela. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> StartList<T>(this T ObjectForDefinition)
        {
            var d = DefineEmptyList<T>();
            if (ObjectForDefinition is object)
            {
                d.Add(ObjectForDefinition);
            }

            return d;
        }

        /// <summary>
        /// Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <returns></returns>
        public static object[] ForceArray(object Obj, Type Type = null)
        {
            var a = new List<object>();
            Type = Type ?? typeof(object);
            if (Obj != null)
            {
                if (Verify.IsArray(Obj))
                {
                    var aobj = (object[])Obj;
                    return Converter.ChangeArrayType<object>(aobj, Type);
                }
                else if (Obj.IsList())
                {
                    var aobj = (List<object>)Obj;
                    return Converter.ChangeIEnumerableType(aobj, Type).ToArray();
                }
                else
                {
                    return (new[] { Obj }).ChangeArrayType(Type);
                }
            }

            return Array.Empty<object>().ChangeArrayType(Type);
        }

        /// <summary>
        /// Verifica se um objeto é um array, e se não, cria um array com oeste objeto
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <returns></returns>
        public static OutputType[] ForceArray<OutputType>(object Obj)
        {
            return ForceArray(Obj, typeof(OutputType)).Cast<OutputType>().ToArray();
        }

        /// <summary>
        /// Aplica as mesmas keys a todos os dicionarios de uma lista
        /// </summary>
        /// <typeparam name="TKey">Tipo da key</typeparam>
        /// <typeparam name="TValue">Tipo do Valor</typeparam>
        /// <param name="Dics">Dicionarios</param>
        /// <param name="AditionalKeys">Chaves para serem incluidas nos dicionários mesmo se não existirem em nenhum deles</param>
        public static IEnumerable<Dictionary<TKey, TValue>> MergeKeys<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> Dics, params TKey[] AditionalKeys)
        {
            AditionalKeys = AditionalKeys ?? Array.Empty<TKey>();
            var chave = Dics.SelectMany(x => x.Keys).Distinct().Union(AditionalKeys);
            foreach (var dic in Dics)
            {
                foreach (var key in chave)
                {
                    if (!dic.ContainsKey(key))
                    {
                        dic[key] = default;
                    }
                }
            }

            return Dics;
        }

        /// <summary>
        /// Converte um tipo para Boolean. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static bool ToBoolean<FromType>(this FromType Value)
        {
            return Value.ChangeType<bool, FromType>();
        }

        /// <summary>
        /// Converte um tipo para Integer. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static int ToInteger<FromType>(this FromType Value)
        {
            return Value.ChangeType<int, FromType>();
        }

        /// <summary>
        /// Converte um tipo para Decimal. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static decimal ToDecimal<FromType>(this FromType Value)
        {
            return Value.ChangeType<decimal, FromType>();
        }

        /// <summary>
        /// Converte um tipo para Single. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static float ToSingle<FromType>(this FromType Value)
        {
            return Value.ChangeType<float, FromType>();
        }

        /// <summary>
        /// Converte um tipo para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static DateTime ToDateTime<FromType>(this FromType Value)
        {
            return Value.ChangeType<DateTime, FromType>();
        }

        /// <summary>
        /// Converte um tipo para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static DateTime ToDateTime<FromType>(this FromType Value, string CultureInfoName)
        {
            return Value.ToDateTime(new CultureInfo(CultureInfoName));
        }

        /// <summary>
        /// Converte um tipo para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static DateTime ToDateTime<FromType>(this FromType Value, CultureInfo CultureInfo)
        {
            return Convert.ToDateTime(Value, CultureInfo);
        }

        /// <summary>
        /// Converte um tipo para Double. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static double ToDouble<FromType>(this FromType Value)
        {
            return Value.ChangeType<double, FromType>();
        }

        /// <summary>
        /// Converte um tipo para Integer. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo</returns>
        public static long ToLong<FromType>(this FromType Value)
        {
            return Value.ChangeType<long, FromType>();
        }

        /// <summary>
        /// Converte um tipo para outro. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="ToType">Tipo</typeparam>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo ou null se a conversão falhar</returns>
        public static ToType ChangeType<ToType, FromType>(this FromType Value)
        {
            return (ToType)Value.ChangeType(typeof(ToType).GetNullableTypeOf());
        }

        /// <summary>
        /// Converte um tipo para outro. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="ToType">Tipo</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo ou null se a conversão falhar</returns>
        public static ToType ChangeType<ToType>(this object Value)
        {
            return (ToType)Value.ChangeType(typeof(ToType).GetNullableTypeOf());
        }

        /// <summary>
        /// Converte um tipo para outro. Retorna Nothing (NULL) ou DEFAULT se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo tipo ou null (ou default) se a conversão falhar</returns>
        public static object ChangeType<FromType>(this FromType Value, Type ToType)
        {
            try
            {
                var tipo = ClassTools.GetNullableTypeOf(ToType);
                if (Value is null)
                {
                    return null;
                }

                if (ReferenceEquals(tipo, typeof(Guid)))
                {
                    return Guid.Parse(Value.ToString());
                }

                if (tipo.IsPrimitiveType())
                {
                    var Converter = TypeDescriptor.GetConverter(tipo);
                    if (Converter.CanConvertFrom(typeof(FromType)))
                    {
                        return Converter.ConvertTo(Value, tipo);
                    }
                    else
                    {
                        return Convert.ChangeType(Value, tipo);
                    }
                }
                else
                {
                    return Conversion.CTypeDynamic(Value, ToType);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Error on change type");
                return null;
            }
        }

        /// <summary>
        /// Converte um array de um tipo para outro
        /// </summary>
        /// <typeparam name="ToType">Tipo do array</typeparam>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo tipo</returns>
        public static ToType[] ChangeArrayType<ToType, FromType>(this FromType[] Value)
        {
            return Value.AsEnumerable().ChangeIEnumerableType<ToType, FromType>().ToArray();
        }

        /// <summary>
        /// Converte um array de um tipo para outro
        /// </summary>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo tipo</returns>
        public static object[] ChangeArrayType<FromType>(this FromType[] Value, Type Type)
        {
            return Value.ChangeIEnumerableType(Type).ToArray();
        }

        /// <summary>
        /// Converte um IEnumerable de um tipo para outro
        /// </summary>
        /// <typeparam name="ToType">Tipo do array</typeparam>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo tipo</returns>
        public static IEnumerable<ToType> ChangeIEnumerableType<ToType, FromType>(this IEnumerable<FromType> Value)
        {
            return (IEnumerable<ToType>)Value.ChangeIEnumerableType(typeof(ToType));
        }

        /// <summary>
        /// Converte um IEnumerable de um tipo para outro
        /// </summary>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo tipo</returns>
        public static IEnumerable<object> ChangeIEnumerableType<FromType>(this IEnumerable<FromType> Value, Type ToType)
        {
            return (Value ?? Array.Empty<FromType>()).Select(el => el.ChangeType(ToType));
        }

        /// <summary>
        /// Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um dicionario os valores sao agrupados em arrays
        /// </summary>
        /// <typeparam name="Tkey">Tipo da Key, Deve ser igual para todos os dicionarios</typeparam>
        /// <param name="FirstDictionary">Dicionario Principal</param>
        /// <param name="Dictionaries">Outros dicionarios</param>
        /// <returns></returns>

        public static Dictionary<Tkey, object> Merge<Tkey>(this Dictionary<Tkey, object> FirstDictionary, params Dictionary<Tkey, object>[] Dictionaries)
        {

            // dicionario que está sendo gerado a partir dos outros
            var result = new Dictionary<Tkey, object>();

            // adiciona o primeiro dicionario ao array principal e exclui dicionarios vazios
            Dictionaries = Dictionaries.Union(new[] { FirstDictionary }).Where(x => x.Count > 0).ToArray();

            // cria um array de keys unicas a partir de todos os dicionarios
            var keys = Dictionaries.SelectMany(x => x.Keys.ToArray()).Distinct();

            // para cada chave encontrada
            foreach (var key in keys)
            {
                // para cada dicionario a ser mesclado
                foreach (var dic in Dictionaries)
                {
                    // dicionario tem a chave?
                    if (dic.ContainsKey(key))
                    {
                        // resultado ja tem a chave atual adicionada?
                        if (result.ContainsKey(key))
                        {
                            // lista que vai mesclar tudo
                            var lista = new List<object>();

                            // chave do resultado é um array?
                            if (Verify.IsArray(result[key]))
                            {
                                lista.AddRange((IEnumerable<object>)result[key]);
                            }
                            else
                            {
                                lista.Add(result[key]);
                            }
                            // chave do dicionario é um array?
                            if (Verify.IsArray(dic[key]))
                            {
                                lista.AddRange((IEnumerable<object>)dic[key]);
                            }
                            else
                            {
                                lista.Add(dic[key]);
                            }

                            // transforma a lista em um resultado
                            if (lista.Count > 0)
                            {
                                if (lista.Count > 1)
                                {
                                    result[key] = lista.ToArray();
                                }
                                else
                                {
                                    result[key] = lista.First();
                                }
                            }
                        }
                        else if (dic[key].GetType() != typeof(string) && (Verify.IsArray(dic[key]) || dic[key].IsList()))
                        {
                            result.Add(key, dic[key].ChangeType<object[]>());
                        }
                        else
                        {
                            result.Add(key, dic[key]);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returna um <see cref=" Dictionary"/> a partir de um <see cref="IGrouping(Of TKey, TElement)"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="groupings"></param>
        /// <returns></returns>
        public static Dictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings)
        {
            return groupings.ToDictionary(group => group.Key, group => group.AsEnumerable());
        }

        /// <summary>
        /// Seta as propriedades de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>

        public static T SetValuesIn<T>(this Dictionary<string, object> Dic)
        {
            return (T)Dic.CreateOrSetObject(null, typeof(T));
        }

        /// <summary>
        /// Seta as propriedades e campos de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Obj"></param>
        public static T SetValuesIn<T>(this Dictionary<string, object> Dic, T obj, params object[] args)
        {
            return (T)Dic.CreateOrSetObject(obj, typeof(T), args);
        }

        /// <summary>
        /// Seta as propriedades e campos de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Obj"></param>
        public static T SetValuesIn<T>(this Dictionary<string, object> Dic, T obj)
        {
            return (T)Dic.CreateOrSetObject(obj, typeof(T), null);
        }

        public static object CreateOrSetObject(this Dictionary<string, object> Dic, object Obj, Type Type, params object[] args)
        {
            var tipo = Type.GetNullableTypeOf();
            if (tipo.IsPrimitiveType())
            {
                return (Dic?.Values.FirstOrDefault()).ChangeType(tipo);
            }

            if (Obj is null)
            {
                if ((args ?? Array.Empty<object>()).Any())
                {
                    Obj = Activator.CreateInstance(tipo, args);
                }
                else
                {
                    Obj = Activator.CreateInstance(tipo);
                }
            }

            if (ReferenceEquals(tipo, typeof(Dictionary<string, object>)))
            {
                if (Dic is object)
                {
                    return Dic.AsEnumerable().ToDictionary();
                }

                return null;
            }
            else if (ReferenceEquals(tipo, typeof(Dictionary<string, string>)))
            {
                if (Dic is object)
                {
                    return Dic.AsEnumerable().ToDictionary(x => x.Key, x => x.Value?.ToString());
                }

                return null;
            }

            if (Dic is object && Dic.Any())
            {
                foreach (var k in Dic)
                {
                    k.Key.PropertyNamesFor();
                    string propname1 = k.Key.Trim().Replace(" ", "_").Replace("-", "_").Replace("~", "_");
                    string propname3 = k.Key.Trim().Replace(" ", "").Replace("-", "").Replace("~", "");
                    string propname2 = propname1.RemoveAccents();
                    string propname4 = propname3.RemoveAccents();
                    var prop = ClassTools.NullCoalesce(tipo.GetProperty(propname1), tipo.GetProperty(propname2), tipo.GetProperty(propname3), tipo.GetProperty(propname4));
                    if (prop is object)
                    {
                        if (prop.CanWrite)
                        {
                            if (ReferenceEquals(k.Value.GetType(), typeof(DBNull)))
                            {
                                prop.SetValue(Obj, null);
                            }
                            else
                            {
                                prop.SetValue(Obj, k.Value.ChangeType(prop.PropertyType));
                            }
                        }
                    }
                    else
                    {
                        var fiif = ClassTools.NullCoalesce(tipo.GetField(propname1), tipo.GetField(propname2), tipo.GetField(propname3), tipo.GetField(propname4));
                        if (fiif is object)
                        {
                            if (ReferenceEquals(k.Value.GetType(), typeof(DBNull)))
                            {
                                prop.SetValue(Obj, null);
                            }
                            else
                            {
                                prop.SetValue(Obj, k.Value.ChangeType(fiif.FieldType));
                            }
                        }
                    }
                }
            }

            return Obj;
        }

        /// <summary>
        /// Transforma uma lista de pares em um Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.DistinctBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Converte um NameValueCollection para um <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        /// <param name="[NameValueCollection]">Formulario</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this NameValueCollection NameValueCollection, params string[] Keys)
        {
            var result = new Dictionary<string, object>();
            if (!(Keys ?? Array.Empty<string>()).Any())
                Keys = NameValueCollection.AllKeys;
            foreach (string key in NameValueCollection.Keys)
            {
                if (key.IsNotBlank() && key.IsLikeAny(Keys))
                {
                    var values = NameValueCollection.GetValues(key);
                    if (result.ContainsKey(key))
                    {
                        var l = new List<object>();
                        if (Verify.IsArray(result[key]))
                        {
                            foreach (var v in (IEnumerable)result[key])
                            {
                                switch (true)
                                {
                                    case object _ when v.IsNumber():
                                        {
                                            l.Add(Convert.ToDouble(v));
                                            break;
                                        }

                                    case object _ when Verify.IsDate(v):
                                        {
                                            l.Add(Convert.ToDateTime(v));
                                            break;
                                        }

                                    default:
                                        {
                                            l.Add(v);
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            switch (true)
                            {
                                case object _ when result[key].IsNumber():
                                    {
                                        l.Add(Convert.ToDouble(result[key]));
                                        break;
                                    }

                                case object _ when Verify.IsDate(result[key]):
                                    {
                                        break;
                                        l.Add(Convert.ToDateTime(result[key]));
                                        break;
                                    }

                                default:
                                    {
                                        l.Add(result[key]);
                                        break;
                                    }
                            }
                        }

                        if (l.Count == 1)
                        {
                            result[key] = l[0];
                        }
                        else
                        {
                            result[key] = l.ToArray();
                        }
                    }
                    else if (values.Length == 1)
                    {
                        switch (true)
                        {
                            case object _ when values[0].IsNumber():
                                {
                                    result.Add(key, Convert.ToDouble(values[0]));
                                    break;
                                }

                            case object _ when values[0].IsDate():
                                {
                                    result.Add(key, Convert.ToDateTime(values[0]));
                                    break;
                                }

                            default:
                                {
                                    result.Add(key, values[0]);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        var ar = new List<object>();
                        foreach (var v in values)
                        {
                            switch (true)
                            {
                                case object _ when v.IsNumber():
                                    {
                                        ar.Add(Convert.ToDouble(v));
                                        break;
                                    }

                                case object _ when v.IsDate():
                                    {
                                        ar.Add(Convert.ToDateTime(v));
                                        break;
                                    }

                                default:
                                    {
                                        ar.Add(v);
                                        break;
                                    }
                            }
                        }

                        result.Add(key, ar.ToArray());
                    }
                }
            }

            return result;
        }
    }
}