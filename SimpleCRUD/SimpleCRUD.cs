using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Extensions;
using Microsoft.CSharp.RuntimeBinder;

/*
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* This file add overloads for each Dapper.SQLMapper method, allowing the use of FormattableString     *
* for building parametrized queries. Each parameter of string will be converted into a SQL parameter. *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*/

namespace Dapper
{
    /// <inheritdoc cref="SqlMapper"/>
    /// <summary>
    /// Main class for Dapper.SimpleCRUD extensions
    /// </summary>
    public static partial class SimpleCRUD

    {
        internal const string DefaultParameterPrefix = "@__p";

        public static int Execute(this IDbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.Execute(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static IEnumerable<T> Query<T>(this IDbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) => cnn.Query<T>(sql.Query, sql.Parameters, transaction, buffered, commandTimeout, commandType);
        public static T QueryFirst<T>(this IDbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QueryFirst<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static T QueryFirstOrDefault<T>(this IDbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QueryFirstOrDefault<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static T QuerySingle<T>(this IDbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QuerySingle<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static T QuerySingleOrDefault<T>(this IDbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QuerySingleOrDefault<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        /// <summary>
        /// Converts a FormattableString to a InterpolatedQuery. It contains the Query string and DynamicParameters.
        /// </summary>
        /// <param name="sql">The FormattableString to convert.</param>
        /// <param name="parameterPrefix">the prefix used for each parameter</param>
        /// <param name="aditionalParameters">Append aditional parameters using <see cref="DynamicParameters.AddDynamicParams(object)"/></param>
        /// <returns>A tuple containing the query string and DynamicParameters.</returns>
        public static InterpolatedQuery ToInterpolatedQuery(this FormattableString sql, string parameterPrefix = null, object aditionalParameters = null, CultureInfo culture = null) => new InterpolatedQuery(sql, parameterPrefix, aditionalParameters, culture);

        private static readonly ConcurrentDictionary<string, string> ColumnNames = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> StringBuilderCacheDict = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();

        private static IColumnNameResolver _columnNameResolver = new ColumnNameResolver();

        private static Dialect _dialect = Dialect.SQLServer;

        private static string _encapsulation;

        private static string _getIdentitySql;

        private static string _getPagedListSql;

        private static ITableNameResolver _tableNameResolver = new TableNameResolver();

        private static bool StringBuilderCacheEnabled = true;

        public static V GeneratePrimaryKey<T, V>(this IDbConnection connection, Expression<Func<T, V>> column, object whereConditions = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class => GeneratePrimaryKey<T, V>(connection, GetColumnName(column), whereConditions, transaction, commandTimeout);

        /// <summary>
        /// Generate a new ID for the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keyName"></param>
        /// <param name="whereConditions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static V GeneratePrimaryKey<T, V>(this IDbConnection connection, string keyName = null, object whereConditions = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var t = typeof(T);
            var sb = new StringBuilder();
            var keys = GetIdProperties(t);

            if (typeof(V) == typeof(Guid))
            {
                var vv = SequentialGuid();
                if (vv is V g)
                {
                    return g;
                }
            }
            else
            {
                if (keys.Any() == false)
                    throw new ArgumentException("Entity must have at least one [Key] or Id property");

                if (string.IsNullOrWhiteSpace(keyName))
                {
                    if (keys.Count(x => x.PropertyType == typeof(V)) > 1)
                    {
                        throw new ArgumentException("Entity has multiple keys, please specify the keyName");
                    }

                    keyName = GetColumnName(keys.FirstOrDefault(x => x.PropertyType == typeof(V)));
                }
                else
                {
                    keyName = t.GetColumnName(keyName);
                }

                var tableName = GetTableName(t);

                sb.Append($"SELECT MAX({keyName}) FROM {tableName} ");

                if (whereConditions is string s && string.IsNullOrWhiteSpace(s) == false)
                {
                    if (s.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        sb.Append(" WHERE ");
                    }

                    sb.Append(s);
                }
                else
                {
                    if (whereConditions != null)
                    {
                        sb.Append(" WHERE ");
                        var wheres = new List<string>();
                        if (whereConditions is DynamicParameters pp)
                        {
                            wheres = pp.ParameterNames.Select(x => $"{typeof(T).GetColumnName(x)} = @{x}").ToList();
                        }
                        else if (whereConditions is Dictionary<string, object> dic)
                        {
                            wheres = dic.Select(x => $"{typeof(T).GetColumnName(x.Key)} = @{x.Key}").ToList();
                            whereConditions = new DynamicParameters(dic);
                        }
                        else
                        {
                            wheres = GetAllProperties(whereConditions).Select(x => $"{typeof(T).GetColumnName(x.Name)} = @{x.Name}").ToList();
                        }
                        sb.Append(string.Join(" AND ", wheres));
                    }
                }

                var sql = sb.ToString();

                var value = connection.QueryFirstOrDefault<V>(sql, whereConditions, transaction, commandTimeout);

                if (value == null)
                {
                    if (connection.RecordCount<T>(whereConditions) == 0)
                    {
                        return (V)Convert.ChangeType(1, typeof(V));
                    }
                    else
                    {
                        throw new Exception("Cannot get the max value to increment");
                    }
                }

                if (value is long v1)
                {
                    return (V)Convert.ChangeType(v1 + 1, typeof(V));
                }
                else

                if (value is int v2)
                {
                    return (V)Convert.ChangeType(v2 + 1, typeof(V));
                }

                if (value is string ss)
                {
                    if (decimal.TryParse(value.ToString(), out decimal v3))
                    {
                        return (V)Convert.ChangeType(v3 + 1, typeof(V));
                    }
                    else
                    {
                        return (V)Convert.ChangeType(SequentialGuid().ToString(), typeof(V));
                    }
                }

                if (value is V v)
                {
                    throw new Exception($"Cannot generate a primary key for {typeof(T).Name}");
                }
            }
            return default;
        }

        //build insert parameters which include all properties in the class that are not:
        //marked with the Editable(false) attribute
        //marked with the [Key] attribute
        //marked with [IgnoreInsert]
        //named Id
        //marked with [NotMapped]
        private static void BuildInsertParameters<T>(StringBuilder masterSb)
        {
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_BuildInsertParameters", sb =>
            {
                var props = GetScaffoldableProperties<T>().ToArray();

                for (var i = 0; i < props.Count(); i++)
                {
                    var property = props.ElementAt(i);

                    if (property.PropertyType != typeof(Guid) && property.PropertyType != typeof(string)
                        && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                           && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                        continue;

                    if (property.GetCustomAttributes(true).Any(attr =>
                        attr.GetType().Name == typeof(IgnoreInsertAttribute).Name ||
                        attr.GetType().Name == typeof(NotMappedAttribute).Name ||
                        attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;

                    if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                    sb.Append(GetColumnName(property));
                    if (i < props.Count() - 1)
                        sb.Append(", ");
                }
                if (sb.ToString().EndsWith(", "))
                    sb.Remove(sb.Length - 2, 2);
            });
        }

        //build insert values which include all properties in the class that are:
        //Not named Id
        //Not marked with the Editable(false) attribute
        //Not marked with the [Key] attribute (without required attribute)
        //Not marked with [IgnoreInsert]
        //Not marked with [NotMapped]
        private static void BuildInsertValues<T>(StringBuilder masterSb)
        {
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_BuildInsertValues", sb =>
            {
                var props = GetScaffoldableProperties<T>().ToArray();
                for (var i = 0; i < props.Count(); i++)
                {
                    var property = props.ElementAt(i);
                    if (property.PropertyType != typeof(Guid) && property.PropertyType != typeof(string)
                        && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                           && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                        continue;
                    if (property.GetCustomAttributes(true).Any(attr =>
                        attr.GetType().Name == typeof(IgnoreInsertAttribute).Name ||
                        attr.GetType().Name == typeof(NotMappedAttribute).Name ||
                        attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))
                       ) continue;

                    if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                    sb.AppendFormat("@{0}", property.Name);
                    if (i < props.Count() - 1)
                        sb.Append(", ");
                }
                if (sb.ToString().EndsWith(", "))
                    sb.Remove(sb.Length - 2, 2);
            });
        }

        //build select clause based on list of properties skipping ones with the IgnoreSelect and NotMapped attribute
        private static void BuildSelect(StringBuilder masterSb, IEnumerable<PropertyInfo> props)
        {
            StringBuilderCache(masterSb, $"{props.CacheKey()}_BuildSelect", sb =>
            {
                var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
                var addedAny = false;
                for (var i = 0; i < propertyInfos.Count(); i++)
                {
                    var property = propertyInfos.ElementAt(i);

                    if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreSelectAttribute).Name || attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                    if (addedAny)
                        sb.Append(",");
                    sb.Append(GetColumnName(property));
                    //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                    if (property.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) != null)
                        sb.Append(" as " + Encapsulate(property.Name));
                    addedAny = true;
                }
            });
        }

        //build update statement based on list on an entity
        private static void BuildUpdateSet<T>(T entityToUpdate, StringBuilder masterSb)
        {
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_BuildUpdateSet", sb =>
            {
                var nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();

                for (var i = 0; i < nonIdProps.Length; i++)
                {
                    var property = nonIdProps[i];

                    sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
                    if (i < nonIdProps.Length - 1)
                        sb.AppendFormat(", ");
                }
            });
        }

        private static void BuildWhere<TEntity>(StringBuilder sb, IEnumerable<PropertyInfo> idProps, object whereConditions = null)
        {
            var propertyInfos = idProps.ToArray();
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var useIsNull = false;

                //match up generic properties to source entity properties to allow fetching of the column attribute
                //the anonymous object used for search doesn'classType have the custom attributes attached to them so this allows us to build the correct where clause
                //by converting the model type to the database column name via the column attribute
                var propertyToUse = propertyInfos.ElementAt(i);
                var sourceProperties = GetScaffoldableProperties<TEntity>().ToArray();
                for (var x = 0; x < sourceProperties.Count(); x++)
                {
                    if (sourceProperties.ElementAt(x).Name == propertyToUse.Name)
                    {
                        if (whereConditions != null && propertyToUse.CanRead && (propertyToUse.GetValue(whereConditions, null) == null || propertyToUse.GetValue(whereConditions, null) == DBNull.Value))
                        {
                            useIsNull = true;
                        }
                        propertyToUse = sourceProperties.ElementAt(x);
                        break;
                    }
                }
                sb.AppendFormat(
                    useIsNull ? "{0} is null" : "{0} = @{1}",
                    GetColumnName(propertyToUse),
                    propertyToUse.Name);

                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        //Get all properties in an entity
        private static IEnumerable<PropertyInfo> GetAllProperties<T>() where T : class => typeof(T).GetProperties();

        private static IEnumerable<PropertyInfo> GetAllProperties<T>(T entity) where T : class
        {
            if (entity == null) return Array.Empty<PropertyInfo>();
            if (entity is Type type)
            {
                return type.GetProperties();
            }

            return GetAllProperties(entity.GetType());
        }

        private static IEnumerable<PropertyInfo> GetIdProperties<T>() where T : class
        => typeof(T).GetIdProperties();

        //Get all properties that are named Id or have the Key attribute
        //For Inserts and updates we have a whole entity so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties<T>(this T entity) where T : class
        {
            if (entity is Type type)
                return GetIdProperties(type);
            return GetIdProperties(entity.GetType());
        }

        //Get all properties that are named Id or have the Key attribute
        //For Get(id) and Delete(id) we don'classType have an entity, just the type so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(this Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }

        //Get all properties that are not decorated with the Editable(false) attribute

        private static IEnumerable<PropertyInfo> GetScaffoldableProperties<T>() => typeof(T).GetScaffoldableProperties();

        private static IEnumerable<PropertyInfo> GetScaffoldableProperties(this Type type)
        {
            IEnumerable<PropertyInfo> props = type.GetProperties();

            props = props.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(EditableAttribute).Name && !IsEditable(p)) == false);

            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        //Get all properties that are:
        //Not named Id
        //Not marked with the Key attribute
        //Not marked ReadOnly
        //Not marked IgnoreInsert
        //Not marked NotMapped
        private static IEnumerable<PropertyInfo> GetUpdateableProperties<T>(T entity)
        {
            var updateableProperties = GetScaffoldableProperties<T>();
            //remove ones with ID
            updateableProperties = updateableProperties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            //remove ones with key attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name) == false);
            //remove ones that are readonly
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == typeof(ReadOnlyAttribute).Name) && IsReadOnly(p)) == false);
            //remove ones with IgnoreUpdate attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreUpdateAttribute).Name) == false);
            //remove ones that are not mapped
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name) == false);

            return updateableProperties;
        }

        //Determine if the Attribute has an AllowEdit key and return its boolean state
        //fake the funk and try to mimic EditableAttribute in System.ComponentModel.DataAnnotations
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsEditable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(EditableAttribute).Name);
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }

        //Determine if the Attribute has an IsReadOnly key and return its boolean state
        //fake the funk and try to mimic ReadOnlyAttribute in System.ComponentModel
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsReadOnly(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(ReadOnlyAttribute).Name);
                if (write != null)
                {
                    return write.IsReadOnly;
                }
            }
            return false;
        }

        /// <summary>
        /// Append a Cached version of a strinbBuilderAction result based on a cacheKey
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="cacheKey"></param>
        /// <param name="stringBuilderAction"></param>
        private static void StringBuilderCache(StringBuilder sb, string cacheKey, Action<StringBuilder> stringBuilderAction)
        {
            if (StringBuilderCacheEnabled && StringBuilderCacheDict.TryGetValue(cacheKey, out string value))
            {
                sb.Append(value);
                return;
            }

            StringBuilder newSb = new StringBuilder();
            stringBuilderAction(newSb);
            value = newSb.ToString();
            StringBuilderCacheDict.AddOrUpdate(cacheKey, value, (t, v) => value);
            sb.Append(value);
        }

        static SimpleCRUD()
        {
            SetDialect(_dialect);
        }

        /// <summary>
        /// <para>Deletes a record or records in the database that match the object passed in</para>
        /// <para>-By default deletes records in the table matching the class name</para>
        /// <para>Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns the number of records affected</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToDelete"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_Delete", sb =>
            {
                var idProps = entityToDelete.GetIdProperties().ToList();

                if (!idProps.Any())
                    throw new ArgumentException("Entity must have at least one [Key] or Id property");

                var name = GetTableName(entityToDelete);

                sb.AppendFormat("delete from {0}", name);

                sb.Append(" where ");
                BuildWhere<T>(sb, idProps, entityToDelete);

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Delete: {0}", sb));
            });
            return connection.Execute(masterSb.ToString(), entityToDelete, transaction, commandTimeout);
        }

        public static int Delete<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return entities.Sum(entity => Delete<T>(connection, entity, transaction, commandTimeout));
        }

        /// <summary>
        /// <para>Deletes a record or records in the database by ID</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// Deletes records where the Id property and properties with the [Key] attribute match
        /// those in the database
        /// </para>
        /// <para>The number of records affected</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int Delete<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Delete<TEntity> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0} where ", name);

            for (var i = 0; i < idProps.Count; i++)
            {
                if (i > 0)
                    sb.Append(" and ");
                sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
            }

            var dynParms = new DynamicParameters();
            if (idProps.Count == 1)
                dynParms.Add("@" + idProps.First().Name, id);
            else
            {
                foreach (var prop in idProps)
                    dynParms.Add("@" + prop.Name, id.GetType().GetProperty(prop.Name).GetValue(id, null));
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Delete<{0}> {1}", currenttype, sb));

            return connection.Execute(sb.ToString(), dynParms, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a list of records in the database</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where that match the where clause</para>
        /// <para>
        /// whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}
        /// </para>
        /// <para>The number of records affected</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int DeleteList<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_DeleteWhere{whereConditions?.GetType()?.FullName}", sb =>
            {
                var currenttype = typeof(T);
                var name = GetTableName(currenttype);

                var whereprops = GetAllProperties(whereConditions).ToArray();
                sb.AppendFormat("Delete from {0}", name);
                if (whereprops.Any())
                {
                    sb.Append(" where ");
                    BuildWhere<T>(sb, whereprops);
                }

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("DeleteList<{0}> {1}", currenttype, sb));
            });
            return connection.Execute(masterSb.ToString(), whereConditions, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a list of records in the database</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where that match the where clause</para>
        /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age"</para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int DeleteList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_DeleteWhere{conditions}", sb =>
            {
                if (string.IsNullOrEmpty(conditions))
                    throw new ArgumentException("DeleteList<TEntity> requires a where clause");
                if (!conditions.ToLower().Contains("where"))
                    throw new ArgumentException("DeleteList<TEntity> requires a where clause and must contain the WHERE keyword");

                var currenttype = typeof(T);
                var name = GetTableName(currenttype);

                sb.AppendFormat("Delete from {0}", name);
                sb.Append(" " + conditions);

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("DeleteList<{0}> {1}", currenttype, sb));
            });
            return connection.Execute(masterSb.ToString(), parameters, transaction, commandTimeout);
        }

        /// <summary>
        /// Encapsulates a string with the characters used for encapsulating table and column names
        /// </summary>
        /// <param name="databaseword"></param>
        /// <returns></returns>
        public static string Encapsulate(string databaseword)
        {
            return string.Format(_encapsulation, databaseword);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>By default filters on the Id column</para>
        /// <para>
        /// -Id column name can be overridden by adding an attribute on your primary key property [Key]
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a single entity by a single id from table TEntity</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a single entity by a single id from table TEntity.</returns>
        public static T Get<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);

            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<TEntity> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0} where ", name);

            for (var i = 0; i < idProps.Count; i++)
            {
                if (i > 0)
                    sb.Append(" and ");
                sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
            }

            var dynParms = new DynamicParameters();

            if (idProps.Count == 1)
            {
                if (id.GetType().IsSimpleType())
                {
                    dynParms.Add("@" + idProps.First().Name, id);
                }
                else if (id is IDictionary<string, object> dic)
                {
                    var prop = idProps.First();

                    if (dic.ContainsKey(prop.Name))
                    {
                        dynParms.Add("@" + prop.Name, dic[prop.Name]);
                    }
                }
                else
                {
                    var idProp = id.GetType().GetProperty(idProps.First().Name);
                    var v = idProp.GetValue(id, null);
                    dynParms.Add("@" + idProps.First().Name, v);
                }
            }
            else
            {
                if (id.GetType().IsSimpleType()) throw new ArgumentException($"{typeof(T).Name} needs a object with following properties: ${idProps.Select(x => x.Name).SelectJoinString(", ")}");

                if (id is IDictionary<string, object> dic)
                {
                    foreach (var prop in idProps)
                    {
                        if (dic.ContainsKey(prop.Name))
                        {
                            dynParms.Add("@" + prop.Name, dic[prop.Name]);
                        }
                    }
                }
                else
                {
                    foreach (var prop in idProps)
                    {
                        var idProp = id.GetType().GetProperty(prop.Name);
                        var v = idProp.GetValue(id, null);
                        dynParms.Add("@" + prop.Name, v);
                    }
                }
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Get<{0}>: {1} with Id: {2}", currenttype, sb, id));

            return connection.QueryFirstOrDefault<T>(sb.ToString(), dynParms, transaction, commandTimeout);
        }

        public static ColumnAttribute GetColumnAttribute(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(true).FirstOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) as ColumnAttribute;
        }
        public static ColumnAttribute GetColumnAttribute<T, V>(this Expression<Func<T, V>> expression) => GetProperty(expression).GetColumnAttribute();

        public static PropertyInfo GetProperty<T, V>(this Expression<Func<T, V>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member as PropertyInfo;
            }
            throw new ArgumentException("Expression must be a property expression");
        }

        public static string GetColumnName(this Type classType, string searchName) => GetScaffoldableProperties(classType).FirstOrDefault(x => x.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase) || x.GetColumnAttribute().Name == searchName)?.Name ?? searchName;

        public static string GetColumnName(PropertyInfo propertyInfo)
        {
            string columnName, key = string.Format("{0}.{1}", propertyInfo.DeclaringType, propertyInfo.Name);

            if (ColumnNames.TryGetValue(key, out columnName))
            {
                return columnName;
            }

            columnName = _columnNameResolver.ResolveColumnName(propertyInfo);

            ColumnNames.AddOrUpdate(key, columnName, (t, v) => columnName);

            return columnName;
        }

        public static string GetColumnName<T, V>(Expression<Func<T, V>> propertyExpression) => GetColumnName(GetProperty(propertyExpression));

        public static string GetColumnName<T, V>(this T obj, Expression<Func<T, V>> propertyExpression) => GetColumnName(propertyExpression);

        public static ColumnAttribute GetColumnAttribute<T, V>(this T obj, Expression<Func<T, V>> propertyExpression) => propertyExpression.GetColumnAttribute<T, V>();

        /// <summary>
        /// Returns the current dialect name
        /// </summary>
        /// <returns></returns>
        public static string GetDialect()
        {
            return _dialect.ToString();
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object whereConditions = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            if (whereConditions == null) whereConditions = new { };

            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0}", name);

            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere<T>(sb, whereprops, whereConditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), whereConditions, transaction, true, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// conditions is an SQL where clause and/or order by clause ex: "where name='bob'" or
        /// "where age&gt;=@Age"
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional SQL where conditions</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0} ", name);
            sb.Append(" ");

            if (string.IsNullOrWhiteSpace(conditions) == false)
            {
                conditions = conditions.Trim();

                if (!conditions.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" where ");
                }

                sb.Append(conditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), parameters, transaction, true, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age" - not required
        /// </para>
        /// <para>
        /// orderby is a column or list of columns to order by ex: "lastname, age desc" - not
        /// required - default is by primary key
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="pageNumber">pageNumber (starts with 0)</param>
        /// <param name="rowsPerPage"></param>
        /// <param name="conditions"></param>
        /// <param name="orderby"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a paged list of entities with optional exact match where conditions</returns>
        /// <returns></returns>
        public static PageResult<T> GetPage<T>(this IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, bool IncludeInsertPlaceholder = false) where T : class
        {
            var result = new PageResult<T>();

            //antes de setar o range, verifica se explicitamente foi a primeira pagina
            if (IncludeInsertPlaceholder && pageNumber == 0 && rowsPerPage >= 1)
            {
                try
                {
                    result.InsertPlaceholder = Activator.CreateInstance<T>();
                }
                catch
                {
                    result.InsertPlaceholder = default(T);
                }
            }

            var TotalItems = connection.RecordCount<T>(conditions, parameters, transaction, commandTimeout);

            var TotalPages = 0;

            if (rowsPerPage > 0)
            {
                TotalPages = (int)Math.Ceiling(TotalItems / (decimal)rowsPerPage);
            }

            if (TotalPages < 0)
            {
                TotalPages = 1;
            }

            if (pageNumber < 0) pageNumber = 0;
            if (pageNumber > TotalPages) pageNumber = TotalPages;

            if (rowsPerPage == 0)
            {
                rowsPerPage = TotalItems;
            }
            else if (rowsPerPage < 0)
            {
                //count only
            }

            if (rowsPerPage > 0)
            {
                result.TotalPages = TotalPages;
                result.PageSize = rowsPerPage;
                result.PageNumber = pageNumber;
                result.Items = connection.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
            }

            result.TotalItems = TotalItems;

            return result;
        }

        /// <inheritdoc cref="GetPage{T}(IDbConnection, int, int, string, string, object, IDbTransaction, int?, bool)"/>
        public static PageResult<T> GetPage<T, V>(this IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, Expression<Func<T, V>> orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, bool IncludeInsertPlaceholder = false) where T : class
            => GetPage<T>(connection, pageNumber, rowsPerPage, conditions, GetColumnName(orderby), parameters, transaction, commandTimeout, IncludeInsertPlaceholder);

        /// <inheritdoc cref="GetListPaged{T}(IDbConnection, int, int, string, string, object, IDbTransaction, int?)"/>
        public static IEnumerable<T> GetListPaged<T, V>(this IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, Expression<Func<T, V>> orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class => GetListPaged<T>(connection, pageNumber, rowsPerPage, conditions, GetColumnName(orderby), parameters, transaction, commandTimeout);

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age" - not required
        /// </para>
        /// <para>
        /// orderby is a column or list of columns to order by ex: "lastname, age desc" - not
        /// required - default is by primary key
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="pageNumber">pageNumber (starts with 0)</param>
        /// <param name="rowsPerPage"></param>
        /// <param name="conditions"></param>
        /// <param name="orderby"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a paged list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetListPaged<T>(this IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (string.IsNullOrEmpty(_getPagedListSql))
                throw new Exception("GetListPage is not supported with the current SQL Dialect");

            if (pageNumber < 0)
                pageNumber = 1;

            pageNumber++;

            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            var query = _getPagedListSql;
            if (string.IsNullOrEmpty(orderby))
            {
                var props = idProps.FirstOrDefault() ?? GetAllProperties<T>().FirstOrDefault();
                if (props == null) throw new ArgumentException("Entity must have at least one [Key] or [Column] property");
                orderby = GetColumnName(props);
            }

            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            query = query.Replace("{SelectColumns}", sb.ToString());
            query = query.Replace("{TableName}", name);
            query = query.Replace("{pageNumber}", pageNumber.ToString());
            query = query.Replace("{RowsPerPage}", rowsPerPage.ToString());
            query = query.Replace("{OrderBy}", orderby);
            query = query.Replace("{WhereClause}", conditions);
            query = query.Replace("{Offset}", ((pageNumber - 1) * rowsPerPage).ToString());

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetListPaged<{0}>: {1}", currenttype, query));

            return connection.Query<T>(query, parameters, transaction, true, commandTimeout);
        }

        //Gets the table name for this type
        //For Get(id) and Delete(id) we don'classType have an entity, just the type so this method is used
        //Use dynamic type to be able to handle both our Table-attribute and the DataAnnotation
        //Uses class name by default and overrides if the class has a Table attribute
        public static string GetTableName(Type type)
        {
            string tableName;

            if (TableNames.TryGetValue(type, out tableName))
                return tableName;

            tableName = _tableNameResolver.ResolveTableName(type);

            TableNames.AddOrUpdate(type, tableName, (t, v) => tableName);

            return tableName;
        }

        public static string GetTableName<T, V>(this Expression<Func<T, V>> exp) => GetTableName(typeof(T));

        public static string GetTableName<T>(this T entity)
        {
            if (entity is Type t) return GetTableName(t);
            return GetTableName(entity.GetType());
        }

        public static string GetTableName<T>() => GetTableName(typeof(T));

        /// <summary>
        /// <para>Inserts a row into the database</para>
        /// <para>By default inserts into the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// Returns the ID (primary key) of the newly inserted record if it is identity using the
        /// int? type, otherwise null
        /// </para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>
        /// The ID (primary key) of the newly inserted record if it is identity using the int? type,
        /// otherwise null
        /// </returns>
        public static TEntity Insert<TEntity>(this IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class => Insert<TEntity, TEntity>(connection, entityToInsert, transaction, commandTimeout);

        /// <summary>
        /// <para>Inserts a row into the database, using ONLY the properties defined by TEntity</para>
        /// <para>By default inserts into the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// Returns the ID (primary key) of the newly inserted record if it is identity using the
        /// defined type, otherwise null
        /// </para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>
        /// The ID (primary key) of the newly inserted record if it is identity using the defined
        /// type, otherwise null
        /// </returns>
        public static TKey Insert<TKey, TEntity>(this IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            if (typeof(TEntity).IsInterface) //FallBack to BaseType Generic Method : https://stackoverflow.com/questions/4101784/calling-a-generic-method-with-a-dynamic-type
            {
                return (TKey)typeof(SimpleCRUD)
                    .GetMethods().Where(methodInfo => methodInfo.Name == nameof(Insert) && methodInfo.GetGenericArguments().Count() == 2).Single()
                    .MakeGenericMethod(new Type[] { typeof(TKey), entityToInsert.GetType() })
                    .Invoke(null, new object[] { connection, entityToInsert, transaction, commandTimeout });
            }
            var idProps = GetIdProperties(entityToInsert).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Insert<TEntity> only supports an entity with a [Key] or Id property");

            var name = GetTableName(entityToInsert);
            var sb = new StringBuilder();
            sb.AppendFormat("insert into {0}", name);
            sb.Append(" (");
            BuildInsertParameters<TEntity>(sb);
            sb.Append(") ");
            sb.Append("values");
            sb.Append(" (");
            BuildInsertValues<TEntity>(sb);
            sb.Append(")");

            var keyHasPredefinedValue = false;
            var baseType = typeof(TKey);
            var underlyingType = Nullable.GetUnderlyingType(baseType);
            var keytype = underlyingType ?? baseType;

            if (keytype != typeof(TEntity) && keytype != typeof(int) && keytype != typeof(uint) && keytype != typeof(long) && keytype != typeof(ulong) && keytype != typeof(short) && keytype != typeof(ushort) && keytype != typeof(Guid) && keytype != typeof(string))
            {
                throw new Exception("Invalid return type");
            }

            if (keytype == typeof(Guid))
            {
                var guidvalue = (Guid)idProps.First().GetValue(entityToInsert, null);
                if (guidvalue == Guid.Empty)
                {
                    var newguid = SequentialGuid();
                    idProps.First().SetValue(entityToInsert, newguid, null);
                }
                else
                {
                    keyHasPredefinedValue = true;
                }
                sb.Append(";select '" + idProps.First().GetValue(entityToInsert, null) + "' as id");
            }

            try
            {
                if ((keytype == typeof(int) || keytype == typeof(long)) && Convert.ToInt64(idProps.First().GetValue(entityToInsert, null)) == 0)
                {
                    sb.Append(";" + _getIdentitySql);
                }
                else
                {
                    keyHasPredefinedValue = true;
                }
            }
            catch
            {
                keyHasPredefinedValue = true;
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Insert: {0}", sb));

            var r = connection.Query<TKey>(sb.ToString(), entityToInsert, transaction, true, commandTimeout).FirstOrDefault();

            if (r != null) return r;

            if (keytype == typeof(TEntity))
            {
                var ee = connection.Get<TEntity>(entityToInsert, transaction, commandTimeout);
                if (ee is TKey kk)
                {
                    return kk;
                }
            }

            if (keytype == typeof(Guid) || keyHasPredefinedValue)
            {
                var v = idProps.First().GetValue(entityToInsert, null);
                if (v is TKey key)
                {
                    return key;
                }
                else
                {
                    return (TKey)Convert.ChangeType(v, keytype);
                }
            }

            throw new DataException("The entity must have a [Key] or Id property");
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a number of records entity by a single id from table TEntity</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age" - not required
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this IDbConnection connection, string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select count(0)");
            sb.AppendFormat(" from {0} ", name);

            if (!string.IsNullOrWhiteSpace(conditions))
            {
                conditions = conditions.Trim();
                if (!conditions.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" where ");
                }
                sb.Append(conditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("RecordCount<{0}>: {1}", currenttype, sb));

            return connection.ExecuteScalar<int>(sb.ToString(), parameters, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a number of records entity by a single id from table TEntity</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select count(0)");
            sb.AppendFormat(" from {0}", name);
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere<T>(sb, whereprops);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("RecordCount<{0}>: {1}", currenttype, sb));

            return connection.ExecuteScalar<int>(sb.ToString(), whereConditions, transaction, commandTimeout);
        }

        /// <summary>
        /// Generates a GUID based on the current date/time http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp
        /// </summary>
        /// <returns></returns>
        public static Guid SequentialGuid()
        {
            var tempGuid = Guid.NewGuid();
            var bytes = tempGuid.ToByteArray();
            var time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }

        /// <summary>
        /// Sets the column name resolver
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a column name</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }

        /// <summary>
        /// Sets the database dialect
        /// </summary>
        /// <param name="dialect"></param>
        public static void SetDialect(Dialect dialect)
        {
            switch (dialect)
            {
                case Dialect.PostgreSQL:
                    _dialect = Dialect.PostgreSQL;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LASTVAL() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({pageNumber}-1) * {RowsPerPage})";
                    break;

                case Dialect.SQLite:
                    _dialect = Dialect.SQLite;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ROWID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({pageNumber}-1) * {RowsPerPage})";
                    break;

                case Dialect.MySQL:
                    _dialect = Dialect.MySQL;
                    _encapsulation = "`{0}`";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    break;

                case Dialect.Oracle:
                    _dialect = Dialect.Oracle;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = "";
                    _getPagedListSql = "SELECT * FROM (SELECT ROWNUM PagedNUMBER, u.* FROM(SELECT {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy}) u) WHERE PagedNUMBER BETWEEN (({pageNumber}-1) * {RowsPerPage} + 1) AND ({pageNumber} * {RowsPerPage})";
                    break;

                case Dialect.DB2:
                    _dialect = Dialect.DB2;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT CAST(IDENTITY_VAL_LOCAL() AS DEC(31,0)) AS \"id\" FROM SYSIBM.SYSDUMMY1");
                    _getPagedListSql = "Select * from (Select {SelectColumns}, row_number() over(order by {OrderBy}) as PagedNumber from {TableName} {WhereClause} Order By {OrderBy}) as classType where classType.PagedNumber between (({pageNumber}-1) * {RowsPerPage} + 1) AND ({pageNumber} * {RowsPerPage})";
                    break;

                default:
                    _dialect = Dialect.SQLServer;
                    _encapsulation = "[{0}]";
                    _getIdentitySql = string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
                    _getPagedListSql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNumber BETWEEN (({pageNumber}-1) * {RowsPerPage} + 1) AND ({pageNumber} * {RowsPerPage})";
                    break;
            }
        }

        /// <summary>
        /// Sets the table name resolver
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a table name</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        /// <summary>
        /// <para>Updates a record or records in the database with only the properties of TEntity</para>
        /// <para>By default updates records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// Updates records where the Id property and properties with the [Key] attribute match
        /// those in the database.
        /// </para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns number of rows affected</para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of affected records</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            if (typeof(TEntity).IsInterface) //FallBack to BaseType Generic Method: https://stackoverflow.com/questions/4101784/calling-a-generic-method-with-a-dynamic-type
            {
                return (int)typeof(SimpleCRUD)
                    .GetMethods().Where(methodInfo => methodInfo.Name == nameof(Update) && methodInfo.GetGenericArguments().Count() == 1).Single()
                    .MakeGenericMethod(new Type[] { entityToUpdate.GetType() })
                    .Invoke(null, new object[] { connection, entityToUpdate, transaction, commandTimeout });
            }
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(TEntity).FullName}_Update", sb =>
            {
                var idProps = GetIdProperties(entityToUpdate).ToList();

                if (!idProps.Any())
                    throw new ArgumentException("Entity must have at least one [Key] or Id property");

                var name = GetTableName(entityToUpdate);

                sb.AppendFormat("update {0}", name);

                sb.AppendFormat(" set ");
                BuildUpdateSet(entityToUpdate, sb);
                sb.Append(" where ");
                BuildWhere<TEntity>(sb, idProps, entityToUpdate);

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Update: {0}", sb));
            });
            return connection.Execute(masterSb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        public static int UpdateWhere<TEntity, Tup>(this IDbConnection connection, TEntity entityToUpdate, Tup whereConditions, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class where Tup : class

        {
            if (typeof(TEntity).IsInterface) //FallBack to BaseType Generic Method: https://stackoverflow.com/questions/4101784/calling-a-generic-method-with-a-dynamic-type
            {
                return (int)typeof(SimpleCRUD)
                    .GetMethods().Where(methodInfo => methodInfo.Name == nameof(UpdateWhere) && methodInfo.GetGenericArguments().Count() == 2).Single()
                    .MakeGenericMethod(new Type[] { entityToUpdate.GetType() })
                    .Invoke(null, new object[] { connection, entityToUpdate, whereConditions, transaction, commandTimeout });
            }
            var masterSb = new StringBuilder();
            var para = new DynamicParameters();
            StringBuilderCache(masterSb, $"{typeof(TEntity).FullName}_Update", sb =>
            {
                var whereProps = GetAllProperties(whereConditions).ToList();

                var name = GetTableName(entityToUpdate);

                sb.AppendFormat("update {0}", name);

                sb.AppendFormat(" set ");
                BuildUpdateSet(entityToUpdate, sb);

                GetUpdateableProperties(entityToUpdate).ToList().ForEach(property =>
                {
                    para.Add(property.Name, property.GetValue(entityToUpdate));
                });

                if (whereProps.Any())
                {
                    sb.Append(" where ");
                    BuildWhere<TEntity>(sb, whereProps, whereProps);
                    foreach (var prop in whereProps)
                    {
                        para.Add(prop.Name, prop.GetValue(whereConditions));
                    }
                }

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Update: {0}", sb));
            });
            return connection.Execute(masterSb.ToString(), para, transaction, commandTimeout);
        }

        public static int UpdateWhere<TEntity>(this IDbConnection connection, TEntity fieldsToUpdate, string whereConditions, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(TEntity).FullName}_UpdateWhere", sb =>
            {
                var name = GetTableName<TEntity>();
                sb.AppendFormat("update {0}", name);
                sb.Append(" set ");
                BuildUpdateSet(fieldsToUpdate, sb);
                sb.Append(" ");

                if (string.IsNullOrWhiteSpace(whereConditions) == false)
                {
                    whereConditions = whereConditions.Trim();
                    if (whereConditions.StartsWith("where", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        sb.Append("where ");
                    }

                    sb.Append(whereConditions);
                }
            });
            return connection.Execute(masterSb.ToString(), fieldsToUpdate, transaction, commandTimeout);
        }

        public static int UpdateWhere<TEntity>(this IDbConnection connection, TEntity entityToUpdate, Expression<Func<TEntity, bool>> whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return UpdateWhere(connection, entityToUpdate, new[] { whereConditions }, transaction, commandTimeout);
        }
        public static int UpdateWhere<TEntity>(this IDbConnection connection, TEntity entityToUpdate, Expression<Func<TEntity, bool>>[] whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var sb = new StringBuilder();
            sb.Append("update ");
            sb.Append(GetTableName(entityToUpdate));
            sb.Append(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            var parameters = new DynamicParameters();
            for (int i = 0; i < whereConditions.Length; i++)
            {
                var condition = whereConditions[i];
                if (condition.Body is BinaryExpression binaryExpression)
                {
                    MemberExpression left = binaryExpression.Left as MemberExpression;
                    if (left != null)
                    {
                        var columnName = GetColumnName(left.Member as PropertyInfo);
                        var parameterName = left.Member.Name;
                        Expression right = binaryExpression.Right as ConstantExpression;
                        object parameterValue;
                        if (right is ConstantExpression c)
                        {
                            parameterValue = c.Value;
                        }
                        else if (right is MemberExpression m)
                        {
                            parameterValue = Expression.Lambda(m).Compile().DynamicInvoke();
                        }
                        else
                        {
                            continue;
                        }
                        parameters.Add("@" + parameterName, parameterValue);
                        sb.AppendFormat("{0} = @{1}", columnName, parameterName);
                    }
                }
                if (i < whereConditions.Length - 1)
                {
                    sb.Append(" AND ");
                }
            }

            return connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        public static int UpdateColumn<TEntity, V>(this IDbConnection connection, Expression<Func<TEntity, object>> column, V value, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null) => UpdateColumn(connection, new[] { column }, value, whereConditions, transaction, commandTimeout);

        public static int UpdateColumn<TEntity, V>(this IDbConnection connection, IEnumerable<Expression<Func<TEntity, object>>> column, V value, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var sb = new StringBuilder();
            DynamicParameters para = new DynamicParameters();

            var name = GetTableName(typeof(TEntity));
            sb.AppendFormat("update {0}", name);
            sb.AppendFormat(" set ", name);
            foreach (var col in column)
            {
                var pname = $"value_{DateTime.Now.Ticks}";
                para.Add(pname, value);
                sb.AppendFormat(" {0} = @{1}, ", GetColumnName(col), pname);
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" ");
            if (whereConditions != null)
            {
                sb.Append(" WHERE ");

                var whereprops = GetAllProperties(whereConditions).ToArray();
                BuildWhere<TEntity>(sb, whereprops);
                foreach (var prop in whereprops)
                {
                    para.Add(prop.Name, prop.GetValue(whereConditions));
                }
            }

            return connection.Execute(sb.ToString(), para, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Inserts or updates a record in the database.</para>
        /// <para>If the record exists, it updates the record.</para>
        /// <para>If the record does not exist, it inserts a new record.</para>
        /// <para>By default, it uses the table matching the class name.</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")].</para>
        /// <para>Supports transaction and command timeout.</para>
        /// <para>Returns TRUE if new record was inserted, otherwise returns FALSE.</para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity to insert or update.</param>
        /// <param name="transaction">The transaction to use, or null.</param>
        /// <param name="commandTimeout">The command timeout, or null.</param>
        /// <returns>TRUE if the record was inserted, FALSE if the record was updated.</returns>
        public static bool Upsert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            var exists = Get<TEntity>(connection, entity, transaction, commandTimeout);
            if (exists == null)
            {
                Insert<TEntity, TEntity>(connection, entity, transaction, commandTimeout);
                return true;
            }
            else
            {
                Update(connection, entity, transaction, commandTimeout);
                return false;
            }
        }

        /// <summary>
        /// Database server dialects
        /// </summary>
        public enum Dialect
        {
            SQLServer,
            PostgreSQL,
            SQLite,
            MySQL,
            Oracle,
            DB2
        }

        public class ColumnNameResolver : IColumnNameResolver
        {
            public virtual string ResolveColumnName(PropertyInfo propertyInfo)
            {
                string columnName;

                if (GetDialect() == Dialect.DB2.ToString())
                {
                    columnName = propertyInfo.Name;
                }
                else
                {
                    columnName = Encapsulate(propertyInfo.Name);
                }

                var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) as dynamic;
                if (columnattr != null)
                {
                    columnName = Encapsulate(columnattr.Name);
                    if (Debugger.IsAttached)
                        Trace.WriteLine(String.Format("Column name for type overridden from {0} to {1}", propertyInfo.Name, columnName));
                }
                return columnName;
            }
        }

        public class TableNameResolver : ITableNameResolver
        {
            public virtual string ResolveTableName(Type type)
            {
                string tableName;

                if (GetDialect() == Dialect.DB2.ToString())
                {
                    tableName = type.Name;
                }
                else
                {
                    tableName = Encapsulate(type.Name);
                }

                var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
                if (tableattr != null)
                {
                    tableName = Encapsulate(tableattr.Name);
                    try
                    {
                        if (!String.IsNullOrEmpty(tableattr.Schema))
                        {
                            string schemaName = Encapsulate(tableattr.Schema);
                            tableName = String.Format("{0}.{1}", schemaName, tableName);
                        }
                    }
                    catch (RuntimeBinderException)
                    {
                        //Schema doesn'classType exist on this attribute.
                    }
                }

                return tableName;
            }
        }

        public interface IColumnNameResolver
        {
            string ResolveColumnName(PropertyInfo propertyInfo);
        }

        public interface ITableNameResolver
        {
            string ResolveTableName(Type type);
        }

        /// <summary>
        /// Validate an entity for required fields and string lengths
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="throwError">When true, throws a ValidationException if validation fails.</param>
        /// <returns>An IEnumerable of strings containing the validation errors.</returns>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<string> ValidateEntity<TEntity>(this TEntity entity, bool throwError = false) where TEntity : class
        {
            var errors = new List<string>();
            var properties = GetScaffoldableProperties<TEntity>().Union(GetIdProperties<TEntity>()).ToList();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);

                //required
                if (value == null && prop.GetCustomAttributes(true).Any(x => x.GetType().Name == typeof(RequiredAttribute).Name))
                {
                    var attr = prop.GetCustomAttributes(true).SingleOrDefault(x => x.GetType().Name == typeof(RequiredAttribute).Name);
                    var message = "";
                    if (attr.GetType().GetProperty(nameof(RequiredAttribute.ErrorMessage)) is PropertyInfo pi)
                    {
                        message = pi.GetValue(attr) as string;
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        message = $"{prop.Name} is required.";
                    }
                    errors.Add(message);
                }

                if (value is string s)
                {
                    //maxlength
                    if (prop.GetCustomAttributes(true).Any(x => x.GetType().Name == typeof(MaxLengthAttribute).Name))
                    {
                        var attr = prop.GetCustomAttributes(true).SingleOrDefault(x => x.GetType().Name == typeof(MaxLengthAttribute).Name);
                        if (attr.GetType().GetProperty(nameof(MaxLengthAttribute.Length)) is PropertyInfo pi)
                        {
                            if (pi.GetValue(attr) is int length)
                            {
                                if (s.Length > length)
                                {
                                    var message = "";
                                    if (attr.GetType().GetProperty(nameof(MaxLengthAttribute.ErrorMessage)) is PropertyInfo pi2)
                                    {
                                        message = pi2.GetValue(attr) as string;
                                    }
                                    if (string.IsNullOrWhiteSpace(message))
                                    {
                                        message = $"{prop.Name} must be less than {length} characters.";
                                    }
                                    message += $" ({s})";
                                    errors.Add(message);
                                }
                            }
                        }
                    }

                    //minlength
                    if (prop.GetCustomAttributes(true).Any(x => x.GetType().Name == typeof(MinLengthAttribute).Name))
                    {
                        var attr = prop.GetCustomAttributes(true).SingleOrDefault(x => x.GetType().Name == typeof(MinLengthAttribute).Name);
                        if (attr.GetType().GetProperty(nameof(MinLengthAttribute.Length)) is PropertyInfo pi)
                        {
                            if (pi.GetValue(attr) is int length)
                            {
                                if (s.Length < length)

                                {
                                    var message = "";
                                    if (attr.GetType().GetProperty(nameof(MinLengthAttribute.ErrorMessage)) is PropertyInfo pi2)
                                    {
                                        message = pi2.GetValue(attr) as string;
                                    }
                                    if (string.IsNullOrWhiteSpace(message))
                                    {
                                        message = $"{prop.Name} must be more than {length} characters.";
                                    }
                                    message += $" ({s})";
                                    errors.Add(message);
                                }
                            }
                        }
                    }
                }
            }
            errors = errors.Distinct().ToList();

            if (throwError && errors.Any())
            {
                ValidationException ex = new ValidationException(string.Join(Environment.NewLine, errors.Select(x => $" - {x}")));
                ex.Data.Add("ValidationErrors", errors);
                throw ex;
            }
            return errors;
        }
    }

    /// <summary>
    /// Optional Editable attribute. You can use the System.ComponentModel.DataAnnotations version
    /// in its place to specify the properties that are editable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute
    {
        /// <summary>
        /// Optional Editable attribute.
        /// </summary>
        /// <param name="iseditable"></param>
        public EditableAttribute(bool iseditable)
        {
            AllowEdit = iseditable;
        }

        /// <summary>
        /// Does this property persist to the database?
        /// </summary>
        public bool AllowEdit { get; private set; }
    }

    /// <summary>
    /// Optional IgnoreInsert attribute. Custom for Dapper.SimpleCRUD to exclude a property from
    /// Insert methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreInsertAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional IgnoreSelect attribute. Custom for Dapper.SimpleCRUD to exclude a property from
    /// Select methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreSelectAttribute : Attribute
    {
    }

    /// <summary>
    /// Optional IgnoreUpdate attribute. Custom for Dapper.SimpleCRUD to exclude a property from
    /// Update methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreUpdateAttribute : Attribute
    {
    }

    /// <summary>
    /// Represent a Paginated Result of a query with pagination information
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public class PageResult<TEntity>
    {
        internal PageResult() { }

        public int? PreviousPage
        {
            get
            {
                if (PageNumber.HasValue)
                {
                    var pg = PageNumber - 1;

                    if (pg < 1)
                    {
                        pg = LastPage;
                    }
                    return pg;
                }

                return null;
            }
        }
        public int? NextPage
        {
            get
            {
                if (PageNumber.HasValue)
                {
                    var pg = PageNumber + 1;
                    if (pg > LastPage)
                    {
                        pg = 1;
                    }
                    return pg;
                }
                return null;
            }
        }

        public bool IsFirstPage => PageNumber == 0;
        public bool IsLastPage => PageNumber == LastPage;

        public int? LastPage => TotalPages;

        /// <summary>
        /// Current Page Number
        /// </summary>
        public int? PageNumber { get; internal set; }
        /// <summary>
        /// Items in the current page
        /// </summary>
        public IEnumerable<TEntity> Items { get; internal set; }

        /// <summary>
        /// Page Size (items per page). Can be null if the query is not paged
        /// </summary>
        public int? PageSize { get; internal set; }

        /// <summary>
        /// Total Items in the query
        /// </summary>
        public int TotalItems { get; internal set; }

        /// <summary>
        /// Total Pages in the query. Can be null if the query is not paged
        /// </summary>
        public int? TotalPages { get; internal set; }

        /// <summary>
        /// Intance of TEntity to be inserted in the first position of the list, used to represent a
        /// empty item with default values. This is only inserted in the first page.
        /// </summary>
        public TEntity InsertPlaceholder { get; internal set; }

        /// <summary>
        /// Return a List of TEntity. Includes the InsertPlaceholder if it is the first page and it
        /// is not null.
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetList()
        {
            var l = Items?.ToList() ?? new List<TEntity>();
            if (InsertPlaceholder != null)
            {
                l.Insert(0, InsertPlaceholder);
            }
            return l;
        }

        public IEnumerable<int> GetPages() => Enumerable.Range(0, TotalPages ?? 0);

        /// <summary>
        /// Return a Dictionary with the page number and the page number button text
        /// </summary>
        /// <param name="PaginationOffset"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetPaginationButtons(int PaginationOffset)
        {
            if (TotalPages.HasValue)
            {
                var pages = GetPages(PaginationOffset).Select(x => (Button: x.ToString(), Number: x)).ToList();
                if (pages.Any(x => x.Number == 0) == false)
                {
                    pages.Insert(0, ("«", 0));
                    pages.Insert(1, ("‹", PreviousPage.Value));
                }

                if (pages.Any(x => x.Number == TotalPages.Value) == false)
                {
                    pages.Add(("›", NextPage.Value));
                    pages.Add(("»", TotalPages.Value));
                }

                return pages.ToDictionary(x => x.Button, x => x.Number);
            }

            return new Dictionary<string, int>();
        }

        public IEnumerable<int> GetPages(int PaginationOffset)
        {
            var arr = new List<int>();
            if (PageNumber.HasValue && TotalPages.HasValue)
            {
                int frange = 1;
                int lrange = 1;
                if (TotalPages > 1)
                {
                    frange = new[] { PageNumber.Value - PaginationOffset, 0 }.Max();
                    lrange = new[] { PageNumber.Value + PaginationOffset, TotalPages.Value }.Min();
                }

                for (int index = frange, loopTo = lrange; index <= loopTo; index++)
                {
                    arr.Add(index);
                }
            }

            return arr;
        }
    }

    public class InterpolatedQuery
    {
        public InterpolatedQuery(string parameterPrefix = null, CultureInfo culture = null)
        {
            this.ParameterPrefix = parameterPrefix;
            this.Culture = culture;
            defaults();
        }

        internal void processQuery(FormattableString sql, object aditionalParameters = null)
        {
            this.defaults();

            if (sql == null)
            {
                return;
            }

            var format = sql.Format;

            for (int i = 0; i < sql.ArgumentCount; i++)
            {
                var arg = sql.GetArgument(i);
                var index = "{" + i + ":";
                var f = format.IndexOf(index);

                if (f >= 0)
                {
                    if (arg is IFormattable farg && farg != null)
                    {
                        var argumentFormat = format.Substring(f + index.Length);
                        argumentFormat = argumentFormat.Substring(0, argumentFormat.IndexOf("}"));
                        arg = farg.ToString(argumentFormat, this.Culture);
                    }
                    else
                    {
                        arg = arg?.ToString();
                    }

                    this.Parameters.Add($"{this.ParameterPrefix}{i}", arg);
                }
                else
                {
                    if (arg is Type t)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.GetTableName(t));
                    }
                    else if (arg is TableAttribute table)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.Encapsulate(table.Name));
                    }
                    else if (arg is PropertyInfo member)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.GetColumnName(member));
                    }
                    else if (arg is ColumnAttribute c)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.Encapsulate(c.Name));
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(arg.GetType()) && arg.GetType() != typeof(string))
                    {
                        var list = arg as IEnumerable<object>;
                        var parameters = string.Join(", ", list.Select((x, j) =>
                        {
                            this.Parameters.Add($"{this.ParameterPrefix}{i}_{j}", x);
                            return $"{this.ParameterPrefix}{i}_{j}";
                        }));

                        format = format.Replace("{" + i + "}", parameters);
                    }
                    else
                    {
                        format = format.Replace("{" + i + "}", $"{this.ParameterPrefix}{i}");
                        this.Parameters.Add($"{this.ParameterPrefix}{i}", arg);
                    }
                }
            }

            if (aditionalParameters != null)
            {
                this.Parameters.AddDynamicParams(aditionalParameters);
            }

            this.Query += format;
        }

        internal void defaults()
        {
            this.Parameters = this.Parameters ?? new DynamicParameters();
            this.ParameterPrefix = this.ParameterPrefix ?? SimpleCRUD.DefaultParameterPrefix;
            this.Culture = this.Culture ?? CultureInfo.InvariantCulture;
            this.Query = this.Query ?? "";
        }

        public InterpolatedQuery(FormattableString sql, string parameterPrefix = null, object aditionalParameters = null, CultureInfo culture = null) : this(parameterPrefix, culture)
        {
            processQuery(sql, aditionalParameters);
        }

        public string Query { get; private set; }
        public DynamicParameters Parameters { get; private set; }

        public string ParameterPrefix { get; private set; }
        public CultureInfo Culture { get; private set; }

        public void Deconstruct(out string query, out DynamicParameters parameters)
        {
            query = Query;
            parameters = Parameters;
        }

        public InterpolatedQuery Append(InterpolatedQuery query)
        {
            defaults();
            this.Query += query.Query;
            Parameters.AddDynamicParams(query.Parameters);
            return this;
        }

        public InterpolatedQuery Append(FormattableString query, object aditionalParameters = null)
        {
            return Append(query.ToInterpolatedQuery(this.ParameterPrefix, aditionalParameters, this.Culture));
        }

        public InterpolatedQuery AppendIf(bool condition, FormattableString query)
        {
            if (condition) Append(query);
            return this;
        }

        public static implicit operator InterpolatedQuery(FormattableString query) => new InterpolatedQuery(query);

        public static implicit operator InterpolatedQuery((FormattableString query, DynamicParameters parameters) tuple) => new InterpolatedQuery(tuple.query, null, tuple.parameters, null);
    }

    internal static class TypeExtension
    {
        public static string CacheKey(this IEnumerable<PropertyInfo> props) => string.Join(",", props.Select(p => p.DeclaringType.FullName + "." + p.Name).ToArray());



    }
}