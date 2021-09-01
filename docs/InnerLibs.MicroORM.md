## `Condition`

A condition with optional AND and OR clauses that can be used in WHERE or JOIN ON statements.
```csharp
public class InnerLibs.MicroORM.Condition

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Condition` | And(`FormattableString` condition) | Appends the given condition with AND in this condition. | 
| `Condition` | And(`Condition` condition) | Appends the given condition with AND in this condition. | 
| `Condition` | AndAll(`FormattableString[]` Conditions) |  | 
| `Condition` | AndAny(`FormattableString[]` Conditions) |  | 
| `Condition` | Or(`FormattableString` condition) | Appends the given condition with OR in this condition. | 
| `Condition` | Or(`Condition` condition) | Appends the given condition with OR in this condition. | 
| `Condition` | OrAll(`FormattableString[]` Conditions) |  | 
| `Condition` | OrAny(`FormattableString[]` Conditions) |  | 
| `String` | ParenthesisToString() | Returns the condition statement as a SQL query in parenthesis. | 
| `String` | ToString() | Returns the condition statement as a SQL query. | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Condition` | AndMany(`FormattableString[]` conditions) |  | 
| `Condition` | OrMany(`FormattableString[]` conditions) |  | 


## `DbExtensions`

```csharp
public class InnerLibs.MicroORM.DbExtensions

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dictionary<Type, DbType>` | DbTypes |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `String` SQL, `NameValueCollection` Parameters) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `String` SQL, `Dictionary<String, Object>` Parameters) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `IEnumerable<DbCommand>` | CreateINSERTCommand(this `DbConnection` Connection, `IEnumerable<T>` obj, `String` TableName = null) |  | 
| `DbCommand` | CreateINSERTCommand(this `DbConnection` Connection, `T` obj, `String` TableName = null) |  | 
| `DbType` | GetDbType(this `T` obj, `DbType` Def = Object) |  | 
| `Type` | GetTypeFromDb(this `DbType` Type, `Type` Def = null) |  | 
| `List<T>` | Map(this `DbDataReader` Reader, `Object[]` args) | Mapeia os objetos de um datareader para uma classe | 
| `T` | MapFirst(this `DbDataReader` Reader, `Object[]` args) |  | 
| `IEnumerable<IEnumerable<Dictionary<String, Object>>>` | MapMany(this `DbDataReader` Reader) |  | 
| `IEnumerable<Object>` | RunSQLArray(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `IEnumerable<Object>` | RunSQLArray(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `IEnumerable<T>` | RunSQLArray(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `IEnumerable<IEnumerable<Dictionary<String, Object>>>` | RunSQLMany(this `DbConnection` Connection, `FormattableString` SQL) | Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de `System.Collections.Generic.Dictionary`2` | 
| `IEnumerable<IEnumerable<Dictionary<String, Object>>>` | RunSQLMany(this `DbConnection` Connection, `DbCommand` Command) | Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de `System.Collections.Generic.Dictionary`2` | 
| `Int32` | RunSQLNone(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `Int32` | RunSQLNone(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Dictionary<Object, Object>` | RunSQLPairs(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `Dictionary<Object, Object>` | RunSQLPairs(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `Dictionary<K, V>` | RunSQLPairs(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `Dictionary<K, V>` | RunSQLPairs(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `Dictionary<String, Object>` | RunSQLRow(this `DbConnection` Connection, `FormattableString` SQL) | Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="!:T" /> | 
| `Dictionary<String, Object>` | RunSQLRow(this `DbConnection` Connection, `DbCommand` SQL) | Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="!:T" /> | 
| `T` | RunSQLRow(this `DbConnection` Connection, `DbCommand` SQL) | Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="!:T" /> | 
| `T` | RunSQLRow(this `DbConnection` Connection, `FormattableString` SQL) | Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="!:T" /> | 
| `IEnumerable<Dictionary<String, Object>>` | RunSQLSet(this `DbConnection` Connection, `FormattableString` SQL) | Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="!:T" /> | 
| `IEnumerable<Dictionary<String, Object>>` | RunSQLSet(this `DbConnection` Connection, `DbCommand` SQL) | Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="!:T" /> | 
| `IEnumerable<T>` | RunSQLSet(this `DbConnection` Connection, `FormattableString` SQL) | Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="!:T" /> | 
| `IEnumerable<T>` | RunSQLSet(this `DbConnection` Connection, `DbCommand` SQL) | Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="!:T" /> | 
| `Object` | RunSQLValue(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Object` | RunSQLValue(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `Nullable<V>` | RunSQLValue(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Nullable<V>` | RunSQLValue(this `DbConnection` Connection, `FormattableString` SQL) |  | 
| `DbCommand` | ToProcedure(this `DbConnection` Connection, `String` ProcedureName, `NameValueCollection` NVC, `String[]` Keys) | Monta um Comando SQL para executar uma procedure especifica e trata valores espicificos de  um NameValueCollection como parametros da procedure | 
| `DbCommand` | ToProcedure(this `DbConnection` Connection, `String` ProcedureName, `T` Obj, `String[]` Keys) | Monta um Comando SQL para executar uma procedure especifica e trata valores espicificos de  um NameValueCollection como parametros da procedure | 
| `DbCommand` | ToProcedure(this `DbConnection` Connection, `String` ProcedureName, `Dictionary<String, Object>` Dic, `String[]` Keys) | Monta um Comando SQL para executar uma procedure especifica e trata valores espicificos de  um NameValueCollection como parametros da procedure | 
| `Select` | ToSQLFilter(this `Dictionary<String, Object>` Dic, `String` TableName, `String` CommaSeparatedColumns, `LogicConcatenationOperator` LogicConcatenation, `String[]` FilterKeys) | Monta um Comando SQL para executar um SELECT com filtros a partir de um `System.Collections.Generic.Dictionary`2` | 
| `String` | ToSQLFilter(this `NameValueCollection` NVC, `String` TableName, `String` CommaSeparatedColumns, `LogicConcatenationOperator` LogicConcatenation, `String[]` FilterKeys) | Monta um Comando SQL para executar um SELECT com filtros a partir de um `System.Collections.Generic.Dictionary`2` | 
| `String` | ToSQLString(`Object` Obj) |  | 
| `String` | ToSQLString(this `FormattableString` SQL) |  | 
| `String` | ToUPDATE(this `NameValueCollection` NVC, `String` TableName, `String` WhereClausule, `String[]` Keys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 


## `ISelect`

```csharp
public interface InnerLibs.MicroORM.ISelect

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `String` | ToString(`Boolean` SubQuery) |  | 


## `Select`

```csharp
public class InnerLibs.MicroORM.Select
    : Select<Dictionary<String, Object>>, ISelect

```

## `Select<T>`

```csharp
public class InnerLibs.MicroORM.Select<T>
    : ISelect

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Select<T>` | AddColumns(`O` Obj = null) |  | 
| `Select<T>` | AddColumns(`String[]` Columns) |  | 
| `Select<T>` | And(`FormattableString[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an AND clause. | 
| `Select<T>` | And(`Condition[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an AND clause. | 
| `Select<T>` | AndAll(`FormattableString[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an AND clause. | 
| `Select<T>` | AndAny(`FormattableString[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an AND clause. | 
| `Select<T>` | AndSearch(`String` Value, `String[]` Columns) |  | 
| `Select<T>` | AndSearch(`IEnumerable<String>` Value, `String[]` Columns) |  | 
| `Select<T>` | ColumnQuote(`Char` QuoteChar) |  | 
| `DbCommand` | CreateDbCommand(`DbConnection` Connection, `Dictionary<String, Object>` dic) |  | 
| `DbCommand` | CreateDbCommand(`DbConnection` Connection) |  | 
| `Select<T>` | CrossApply(`String` table) | Sets a CROSS JOIN clause in the SELECT being built. | 
| `Select<T>` | CrossJoin(`String` table) | Sets a CROSS JOIN clause in the SELECT being built. | 
| `Select<T>` | From(`String` TableOrSubQuery) | Sets the FROM clause in the SELECT being built. | 
| `Select<T>` | From(`Select<O>` SubQuery, `String` SubQueryAlias = null) | Sets the FROM clause in the SELECT being built. | 
| `Select<T>` | From(`Action<Select<O>>` SubQuery, `String` SubQueryAlias = null) | Sets the FROM clause in the SELECT being built. | 
| `Select<T>` | From(`Action<Select>` SubQuery) | Sets the FROM clause in the SELECT being built. | 
| `Select<T>` | From() | Sets the FROM clause in the SELECT being built. | 
| `Select<T>` | FullOuterJoin(`String` table, `FormattableString` on) | Sets a FULL OUTER JOIN clause in the SELECT being built. | 
| `Select<T>` | FullOuterJoin(`String` table, `Condition` on) | Sets a FULL OUTER JOIN clause in the SELECT being built. | 
| `Select<T>` | GroupBy(`String[]` columns) | Sets the GROUP BY clause in the SELECT being built. | 
| `Select<T>` | Having(`String` condition) | Sets or overwrite the HAVING clause in the SELECT being built. | 
| `Select<T>` | InnerJoin(`String` table, `FormattableString` on) | Sets a INNER JOIN clause in the SELECT being built. | 
| `Select<T>` | InnerJoin(`String` table, `Condition` on) | Sets a INNER JOIN clause in the SELECT being built. | 
| `Select<T>` | Join(`String` table, `FormattableString` on) | Sets a JOIN clause in the SELECT being built. | 
| `Select<T>` | Join(`String` table, `Condition` on) | Sets a JOIN clause in the SELECT being built. | 
| `Select<T>` | LeftOuterJoin(`String` table, `FormattableString` on) | Sets a LEFT OUTER JOIN clause in the SELECT being built. | 
| `Select<T>` | LeftOuterJoin(`String` table, `Condition` on) | Sets a LEFT OUTER JOIN clause in the SELECT being built. | 
| `Select<T>` | OffSet(`Int32` Page, `Int32` PageSize) |  | 
| `Select<T>` | Or(`FormattableString[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an OR clause. | 
| `Select<T>` | Or(`Condition[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an OR clause. | 
| `Select<T>` | OrAll(`FormattableString[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an AND clause. | 
| `Select<T>` | OrAny(`FormattableString[]` conditions) | Sets the WHERE clause in the SELECT being built.  If WHERE is already set, appends the condition with an AND clause. | 
| `Select<T>` | OrderBy(`String[]` columns) | Sets the ORDER BY clause in the SELECT being built. | 
| `Select<T>` | OrSearch(`String` Value, `String[]` Columns) |  | 
| `Select<T>` | OrSearch(`IEnumerable<String>` Value, `String[]` Columns) |  | 
| `Select<T>` | RemoveColumns(`String[]` Columns) |  | 
| `Select<T>` | RightOuterJoin(`String` table, `FormattableString` on) | Sets a RIGHT OUTER JOIN clause in the SELECT being built. | 
| `Select<T>` | RightOuterJoin(`String` table, `Condition` on) | Sets a RIGHT OUTER JOIN clause in the SELECT being built. | 
| `Select<T>` | SetColumns(`String[]` Columns) |  | 
| `Select<T>` | SetColumns(`O` Obj = null) |  | 
| `String` | ToString() | Returns the SELECT statement as a SQL query. | 
| `String` | ToString(`Boolean` AsSubquery) | Returns the SELECT statement as a SQL query. | 
| `Select<T>` | Where(`FormattableString` condition) | Sets the WHERE clause in the SELECT being built. | 
| `Select<T>` | Where(`String` LogicOperator, `IEnumerable<FormattableString>` conditions) | Sets the WHERE clause in the SELECT being built. | 
| `Select<T>` | Where(`String` LogicOperator, `IEnumerable<Condition>` conditions) | Sets the WHERE clause in the SELECT being built. | 
| `Select<T>` | Where(`Expression<Func<T, Boolean>>` predicate) | Sets the WHERE clause in the SELECT being built. | 
| `Select<T>` | Where(`Condition` condition) | Sets the WHERE clause in the SELECT being built. | 
| `Select<T>` | Where(`Condition[]` condition) | Sets the WHERE clause in the SELECT being built. | 
| `Select<T>` | WhereObject(`O` Obj) |  | 
| `Select<T>` | WhereObject(`O` Obj, `String` LogicOperator = AND) |  | 


