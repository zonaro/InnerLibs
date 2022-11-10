## `ColumnName`

```csharp
public class InnerLibs.MicroORM.ColumnName
    : Attribute, _Attribute

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String[]` | Names |  | 


## `Condition`

```csharp
public class InnerLibs.MicroORM.Condition

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Condition` | And(`FormattableString` condition) |  | 
| `Condition` | And(`Condition` condition) |  | 
| `Condition` | AndAll(`FormattableString[]` Conditions) |  | 
| `Condition` | AndAny(`FormattableString[]` Conditions) |  | 
| `Condition` | Or(`FormattableString` condition) |  | 
| `Condition` | Or(`Condition` condition) |  | 
| `Condition` | OrAll(`FormattableString[]` Conditions) |  | 
| `Condition` | OrAny(`FormattableString[]` Conditions) |  | 
| `String` | ParenthesisToString() |  | 
| `String` | ToString() |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Condition` | AndMany(`FormattableString[]` conditions) |  | 
| `Condition` | OrMany(`FormattableString[]` conditions) |  | 


## `DataSetType`

```csharp
public static class InnerLibs.MicroORM.DataSetType

```

Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Many |  | 
| `String` | Pair |  | 
| `String` | Row |  | 
| `String` | Value |  | 
| `String` | Values |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | ToList() |  | 


## `DbExtensions`

```csharp
public static class InnerLibs.MicroORM.DbExtensions

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dictionary<Type, DbType>` | DbTypes |  | 
| `TextWriter` | LogWriter |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | AsSQLColumns(this `IDictionary<String, Object>` obj, `Char` Quote = [) |  | 
| `String` | AsSQLColumns(this `T` obj, `Char` Quote = [) |  | 
| `String` | AsSQLColumns(this `NameValueCollection` obj, `Char` Quote = [, `String[]` Keys) |  | 
| `DbCommand` | BeforeRun(`DbConnection&` Connection, `DbCommand&` Command, `TextWriter` LogWriter = null) |  | 
| `IEnumerable<String>` | ColumnsFromClass() |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FileInfo` SQLFile, `T` obj, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateCommand(`DbConnection` connection, `String` SQL, `T` obj, `DbTransaction` transaction = null) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FileInfo` SQLFile, `Dictionary<String, Object>` Parameters, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FileInfo` SQLFile, `NameValueCollection` Parameters, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `String` SQL, `NameValueCollection` Parameters, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `String` SQL, `Dictionary<String, Object>` Parameters, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `String` SQL, `String[]` Args) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `String` SQL, `DbTransaction` Transaction, `String[]` Args) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FileInfo` SQLFile, `String[]` Args) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FileInfo` SQLFile, `DbTransaction` Transaction, `String[]` Args) |  | 
| `DbCommand` | CreateCommand(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<DbCommand>` | CreateINSERTCommand(this `DbConnection` Connection, `IEnumerable<T>` obj, `String` TableName = null, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateINSERTCommand(this `DbConnection` Connection, `T` obj, `String` TableName = null, `DbTransaction` Transaction = null) |  | 
| `SQLResponse<Object>` | CreateSQLQuickResponse(this `DbConnection` Connection, `FormattableString` Command, `String` DataSetType, `Boolean` IncludeCommandText = False) |  | 
| `SQLResponse<Object>` | CreateSQLQuickResponse(this `DbCommand` Command, `String` DataSetType, `Boolean` IncludeCommandText = False) |  | 
| `DbCommand` | CreateUPDATECommand(this `DbConnection` Connection, `T` obj, `FormattableString` WhereClausule, `String` TableName = null, `DbTransaction` Transaction = null) |  | 
| `String` | FormatSQLColumn(`Char` QuoteChar, `String[]` ColumnNameParts) |  | 
| `String` | FormatSQLColumn(`String[]` ColumnNameParts) |  | 
| `DbType` | GetDbType(this `T` obj, `DbType` DefaultType = Object) |  | 
| `DataRow` | GetFirstRow(this `DataSet` Data) |  | 
| `DataRow` | GetFirstRow(this `DataTable` Table) |  | 
| `DataTable` | GetFirstTable(this `DataSet` Data) |  | 
| `Type` | GetTypeFromDb(this `DbType` Type, `Type` DefaultType = null) |  | 
| `String` | GetValue(this `DataRow` row, `String` Name = null, `Expression<Func<Object, Object>>` valueParser = null) |  | 
| `T` | GetValue(this `DataRow` row, `String` Name = null, `Expression<Func<Object, Object>>` valueParser = null) |  | 
| `String` | GetValue(this `DataTable` Table, `String` Name = null, `Expression<Func<Object, Object>>` valueParser = null) |  | 
| `String` | GetValue(this `DataSet` Data, `String` Name = null, `Expression<Func<Object, Object>>` valueParser = null) |  | 
| `T` | GetValue(this `DataSet` Data, `String` Name = null, `Expression<Func<Object, Object>>` valueParser = null) |  | 
| `T` | GetValue(this `DataTable` Table, `String` Name = null, `Expression<Func<Object, Object>>` valueParser = null) |  | 
| `Boolean` | IsBroken(this `DbConnection` Connection) |  | 
| `Boolean` | IsClosed(this `DbConnection` Connection) |  | 
| `Boolean` | IsConnecting(this `DbConnection` Connection) |  | 
| `Boolean` | IsExecuting(this `DbConnection` Connection) |  | 
| `Boolean` | IsOpen(this `DbConnection` Connection) |  | 
| `DbCommand` | LogCommand(this `DbCommand` Command, `TextWriter` LogWriter = null) |  | 
| `T` | Map(this `DataRow` Row, `Object[]` args) |  | 
| `IEnumerable<T>` | Map(this `DataTable` Data, `Object[]` args) |  | 
| `IEnumerable<T>` | Map(this `DbDataReader` Reader, `Object[]` args) |  | 
| `T` | MapFirst(this `DataTable` Data, `Object[]` args) |  | 
| `T` | MapFirst(this `DbDataReader` Reader, `Object[]` args) |  | 
| `IEnumerable<IEnumerable<Dictionary<String, Object>>>` | MapMany(this `DbDataReader` Reader) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>` | MapMany(this `DbDataReader` Reader) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>` | MapMany(this `DbDataReader` Reader) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>` | MapMany(this `DbDataReader` Reader) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>>` | MapMany(this `DbDataReader` Reader) |  | 
| `ConnectionType` | OpenConnection(this `ConnectionStringParser` connection) |  | 
| `T` | ProccessSubQuery(this `DbConnection` Connection, `T` d, `String` PropertyName, `Boolean` Recursive = False) |  | 
| `T` | ProccessSubQuery(this `DbConnection` Connection, `T` d, `Boolean` Recursive = False) |  | 
| `String` | QueryForClass(`Object` InjectionObject = null) |  | 
| `IEnumerable<T>` | RunSQLArray(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `IEnumerable<T>` | RunSQLArray(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<Object>` | RunSQLArray(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `IEnumerable<Object>` | RunSQLArray(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>` | RunSQLMany(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>` | RunSQLMany(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>` | RunSQLMany(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>` | RunSQLMany(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>>` | RunSQLMany(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>>` | RunSQLMany(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `IEnumerable<IEnumerable<Dictionary<String, Object>>>` | RunSQLMany(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<IEnumerable<Dictionary<String, Object>>>` | RunSQLMany(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>` | RunSQLMany(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>` | RunSQLMany(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Int32` | RunSQLNone(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Int32` | RunSQLNone(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Dictionary<Object, Object>` | RunSQLPairs(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `Dictionary<Object, Object>` | RunSQLPairs(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Dictionary<K, V>` | RunSQLPairs(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `Dictionary<K, V>` | RunSQLPairs(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `DbDataReader` | RunSQLReader(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `DbDataReader` | RunSQLReader(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `T` | RunSQLRow(this `DbConnection` Connection, `Select<T>` Select, `Boolean` WithSubQueries = False, `DbTransaction` Transaction = null) |  | 
| `Dictionary<String, Object>` | RunSQLRow(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `Dictionary<String, Object>` | RunSQLRow(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `T` | RunSQLRow(this `DbConnection` Connection, `DbCommand` SQL, `Boolean` WithSubQueries = False) |  | 
| `T` | RunSQLRow(this `DbConnection` Connection, `FormattableString` SQL, `Boolean` WithSubQueries = False, `DbTransaction` Transaction = null) |  | 
| `T` | RunSQLRow(this `DbConnection` Connection, `Boolean` WithSubQueries = False, `DbTransaction` Transaction = null, `Object` InjectionObject = null) |  | 
| `IEnumerable<T>` | RunSQLSet(this `DbConnection` Connection, `Select<T>` Select, `Boolean` WithSubQueries = False, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<T>` | RunSQLSet(this `DbConnection` Connection, `Boolean` WithSubQueries = False, `DbTransaction` Transaction = null, `Object` InjectionObject = null) |  | 
| `IEnumerable<Dictionary<String, Object>>` | RunSQLSet(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<Dictionary<String, Object>>` | RunSQLSet(this `DbConnection` Connection, `DbCommand` SQL) |  | 
| `IEnumerable<T>` | RunSQLSet(this `DbConnection` Connection, `FormattableString` SQL, `Boolean` WithSubQueries = False, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<T>` | RunSQLSet(this `DbConnection` Connection, `DbCommand` SQL, `Boolean` WithSubQueries = False) |  | 
| `Object` | RunSQLValue(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `Object` | RunSQLValue(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `V` | RunSQLValue(this `DbConnection` Connection, `DbCommand` Command) |  | 
| `V` | RunSQLValue(this `DbConnection` Connection, `FormattableString` SQL, `DbTransaction` Transaction = null) |  | 
| `IEnumerable<DbCommand>` | ToBatchProcedure(this `DbConnection` Connection, `String` ProcedureName, `IEnumerable<T>` Items, `DbTransaction` Transaction = null, `String[]` Keys) |  | 
| `DataSet` | ToDataSet(this `DbDataReader` reader, `String` DataSetName) |  | 
| `DbCommand` | ToProcedure(this `DbConnection` Connection, `String` ProcedureName, `NameValueCollection` NVC, `DbTransaction` Transaction = null, `String[]` Keys) |  | 
| `DbCommand` | ToProcedure(this `DbConnection` Connection, `String` ProcedureName, `T` Obj, `DbTransaction` Transaction = null, `String[]` Keys) |  | 
| `DbCommand` | ToProcedure(this `DbConnection` Connection, `String` ProcedureName, `Dictionary<String, Object>` Dic, `DbTransaction` Transaction = null, `String[]` Keys) |  | 
| `Select` | ToSQLFilter(this `NameValueCollection` NVC, `String` TableName, `String` CommaSeparatedColumns, `String[]` FilterKeys) |  | 
| `Select` | ToSQLFilter(this `Dictionary<String, Object>` Dic, `String` TableName, `String` CommaSeparatedColumns, `LogicConcatenationOperator` LogicConcatenation, `String[]` FilterKeys) |  | 
| `String` | ToSQLString(this `T` Obj, `Boolean` Parenthesis = True) |  | 
| `String` | ToSQLString(this `FormattableString` SQL, `Boolean` Parenthesis = True) |  | 


## `FromSQL`

```csharp
public class InnerLibs.MicroORM.FromSQL
    : Attribute, _Attribute

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | File |  | 
| `String` | SQL |  | 
| `String` | TableName |  | 


## `ISelect`

```csharp
public interface InnerLibs.MicroORM.ISelect

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `String` | ToString(`Boolean` SubQuery) |  | 


## `JoinType`

```csharp
public enum InnerLibs.MicroORM.JoinType
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Join |  | 
| `1` | Inner |  | 
| `2` | LeftOuterJoin |  | 
| `3` | RightOuterJoin |  | 
| `4` | FullOuterJoin |  | 
| `5` | CrossJoin |  | 
| `6` | CrossApply |  | 


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

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Query |  | 
| `Char` | QuoteChar |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Select<T>` | AddColumns(`TO` Obj = null) |  | 
| `Select<T>` | AddColumns(`String[]` Columns) |  | 
| `Select<T>` | And(`IEnumerable<FormattableString>` conditions) |  | 
| `Select<T>` | And(`FormattableString` condition) |  | 
| `Select<T>` | And(`String` Column, `Object` Value, `String` Operator = =) |  | 
| `Select<T>` | And(`Condition[]` conditions) |  | 
| `Select<T>` | AndAll(`FormattableString[]` conditions) |  | 
| `Select<T>` | AndAny(`FormattableString[]` conditions) |  | 
| `Select<T>` | AndObject(`TO` Obj) |  | 
| `Select<T>` | AndSearch(`String` Value, `String[]` Columns) |  | 
| `Select<T>` | AndSearch(`IEnumerable<String>` Values, `String[]` Columns) |  | 
| `Select<T>` | ColumnQuote(`Nullable<Char>` QuoteChar = null, `Boolean` WithTableName = False) |  | 
| `DbCommand` | CreateDbCommand(`DbConnection` Connection, `Dictionary<String, Object>` dic, `DbTransaction` Transaction = null) |  | 
| `DbCommand` | CreateDbCommand(`DbConnection` Connection, `DbTransaction` Transaction = null) |  | 
| `Select<T>` | CrossApply(`String` table) |  | 
| `Select<T>` | CrossJoin(`String` table) |  | 
| `String` | FormatColumnName(`String` ColumnName) |  | 
| `Select<T>` | From(`String` TableOrSubQuery) |  | 
| `Select<T>` | From(`Select<TO>` SubQuery, `String` SubQueryAlias = null) |  | 
| `Select<T>` | From(`Action<Select<TO>>` SubQuery, `String` SubQueryAlias = null) |  | 
| `Select<T>` | From(`Action<Select>` SubQuery) |  | 
| `Select<T>` | From() |  | 
| `Select<T>` | FullOuterJoin(`String` table, `FormattableString` on) |  | 
| `Select<T>` | FullOuterJoin(`String` table, `Condition` on) |  | 
| `Select<T>` | FullOuterJoin(`String` table, `String` ThisColumn, `String` ForeignColumn) |  | 
| `String` | GetTableOrSubQuery() |  | 
| `Select<T>` | GroupBy(`String[]` columns) |  | 
| `Select<T>` | Having(`String` condition) |  | 
| `Select<T>` | InnerJoin(`String` table, `FormattableString` on) |  | 
| `Select<T>` | InnerJoin(`String` table, `String` ThisColumn, `String` ForeignColumn) |  | 
| `Select<T>` | InnerJoin(`String` table, `Condition` on) |  | 
| `Select<T>` | Join(`String` table, `FormattableString` on) |  | 
| `Select<T>` | Join(`String` table, `Condition` on) |  | 
| `Select<T>` | Join(`JoinType` JoinType, `String` Table, `Condition` on) |  | 
| `Select<T>` | LeftOuterJoin(`String` table, `FormattableString` on) |  | 
| `Select<T>` | LeftOuterJoin(`String` table, `String` ThisColumn, `String` ForeignColumn) |  | 
| `Select<T>` | LeftOuterJoin(`String` table, `Condition` on) |  | 
| `Select<T>` | OffSet(`Int32` Page, `Int32` PageSize) |  | 
| `Select<T>` | Or(`String` Column, `Object` Value, `String` Operator = =) |  | 
| `Select<T>` | Or(`IEnumerable<FormattableString>` conditions) |  | 
| `Select<T>` | Or(`FormattableString` condition) |  | 
| `Select<T>` | Or(`Condition[]` conditions) |  | 
| `Select<T>` | OrAll(`FormattableString[]` conditions) |  | 
| `Select<T>` | OrAny(`FormattableString[]` conditions) |  | 
| `Select<T>` | OrderBy(`String[]` columns) |  | 
| `Select<T>` | OrObject(`TO` Obj) |  | 
| `Select<T>` | OrSearch(`String` Value, `String[]` Columns) |  | 
| `Select<T>` | OrSearch(`IEnumerable<String>` Values, `String[]` Columns) |  | 
| `Select<T>` | RemoveColumns(`String[]` Columns) |  | 
| `Select<T>` | RightOuterJoin(`String` table, `String` ThisColumn, `String` ForeignColumn) |  | 
| `Select<T>` | RightOuterJoin(`String` table, `FormattableString` on) |  | 
| `Select<T>` | RightOuterJoin(`String` table, `Condition` on) |  | 
| `Select<T>` | SetColumns(`String[]` Columns) |  | 
| `Select<T>` | SetColumns(`TO` Obj = null) |  | 
| `Select<T>` | Top(`Int32` Top, `Boolean` Percent = False) |  | 
| `String` | ToString() |  | 
| `String` | ToString(`Boolean` AsSubquery) |  | 
| `Select<T>` | Where(`FormattableString` condition) |  | 
| `Select<T>` | Where(`String` LogicOperator, `IEnumerable<FormattableString>` conditions) |  | 
| `Select<T>` | Where(`String` LogicOperator, `IEnumerable<Condition>` conditions) |  | 
| `Select<T>` | Where(`Expression<Func<T, Boolean>>` predicate) |  | 
| `Select<T>` | Where(`String` Column, `Object` Value, `String` Operator = =) |  | 
| `Select<T>` | Where(`Condition` condition) |  | 
| `Select<T>` | Where(`Condition[]` conditions) |  | 
| `Select<T>` | Where(`ValueTuple`3[]` items) |  | 
| `Object` | Where(`Dictionary<String, Object>` Dic, `LogicConcatenationOperator` LogicConcatenation, `String[]` FilterKeys) |  | 
| `Select<T>` | Where(`NameValueCollection` NVC, `String[]` FilterKeys) |  | 
| `Select<T>` | WhereObject(`TO` Obj) |  | 
| `Select<T>` | WhereObject(`TO` Obj, `LogicConcatenationOperator` LogicOperator) |  | 
| `Select<T>` | WhereObject(`TO` Obj, `String` LogicOperator) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FormattableString` | CreateSearch(`IEnumerable<String>` Values, `String[]` Columns) |  | 


## `SQLResponse<T>`

```csharp
public class InnerLibs.MicroORM.SQLResponse<T>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `T` | Data |  | 
| `String` | Message |  | 
| `String` | SQL |  | 
| `String` | Status |  | 


