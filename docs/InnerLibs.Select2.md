## `ISelect2Option`

```csharp
public interface InnerLibs.Select2.ISelect2Option
    : ISelect2Result

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Disabled |  | 
| `String` | ID |  | 
| `Boolean` | Selected |  | 


## `ISelect2Result`

```csharp
public interface InnerLibs.Select2.ISelect2Result

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Text |  | 


## `Pagination`

```csharp
public class InnerLibs.Select2.Pagination

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | More |  | 


## `Select2Data`

```csharp
public class InnerLibs.Select2.Select2Data

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Pagination` | Pagination |  | 
| `IEnumerable<ISelect2Result>` | Results |  | 


## `Select2Extensions`

```csharp
public class InnerLibs.Select2.Select2Extensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Select2Data` | CreateSelect2Data(this `IEnumerable<T>` List, `Func<T, String>` TextSelector, `Func<T, String>` IdSelector, `Action<T, OptionType>` OtherSelectors = null, `Func<T, String>` GroupSelector = null) |  | 
| `Select2Data` | CreateSelect2Data(this `PaginationFilter<T1, T2>` Filter, `Func<T2, String>` TextSelector, `Func<T2, String>` IdSelector, `Func<T2, String>` GroupSelector = null, `Action<T2, OptionsType>` OtherSelectors = null) |  | 
| `Select2Data` | CreateSelect2Data(this `PaginationFilter<T1, T2>` Filter, `Func<T2, String>` TextSelector, `Func<T2, String>` IdSelector, `Action<T2, OptionsType>` OtherSelectors) |  | 
| `Select2Data` | CreateSelect2Data(this `PaginationFilter<T1, OptionsType>` Filter) |  | 
| `Select2Data` | CreateSelect2Data(this `PaginationFilter<T1, OptionsType>` Filter, `Func<OptionsType, String>` GroupBySelector) |  | 
| `OptionType` | CreateSelect2Option(this `T` item, `Func<T, String>` TextSelector, `Func<T, String>` IdSelector, `Action<T, OptionType>` OtherSelectors = null) |  | 


## `Select2Group`

```csharp
public class InnerLibs.Select2.Select2Group
    : ISelect2Result

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<ISelect2Option>` | Children |  | 
| `String` | Text |  | 


## `Select2Option`

```csharp
public class InnerLibs.Select2.Select2Option
    : ISelect2Option, ISelect2Result

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Disabled |  | 
| `String` | ID |  | 
| `Boolean` | Selected |  | 
| `String` | Text |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


