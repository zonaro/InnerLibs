## `Alignment`

```csharp
public enum InnerLibs.ConsoleTables.Alignment
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Left |  | 
| `1` | Right |  | 


## `ConsoleTable`

```csharp
public class InnerLibs.ConsoleTables.ConsoleTable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | Columns |  | 
| `Type[]` | ColumnTypes |  | 
| `ConsoleTableOptions` | Options |  | 
| `List<Object[]>` | Rows |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ConsoleTable` | AddColumn(`IEnumerable<String>` names) |  | 
| `ConsoleTable` | AddRow(`Object[]` Values) |  | 
| `ConsoleTable` | AddValue(`String` Key, `Object` obj) |  | 
| `ConsoleTable` | Configure(`Action<ConsoleTableOptions>` action) |  | 
| `String` | ToMarkDownString() |  | 
| `String` | ToMinimalString() |  | 
| `String` | ToString(`Format` format) |  | 
| `String` | ToString() |  | 
| `String` | ToStringAlternative() |  | 
| `void` | Write(`Format` format = Default) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ConsoleTable` | From(`IEnumerable<T>` values) |  | 


## `ConsoleTableOptions`

```csharp
public class InnerLibs.ConsoleTables.ConsoleTableOptions

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | Columns |  | 
| `Boolean` | EnableCount |  | 
| `Alignment` | NumberAlignment |  | 
| `TextWriter` | OutputTo |  | 


## `Format`

```csharp
public enum InnerLibs.ConsoleTables.Format
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Default |  | 
| `1` | MarkDown |  | 
| `2` | Alternative |  | 
| `3` | Minimal |  | 


