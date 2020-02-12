## `JsonReader`

```csharp
public class InnerLibs.JsonReader.JsonReader
    : DynamicObject, IDynamicMetaObjectProvider

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsArray |  | 
| `Boolean` | IsObject |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Delete(`String` name) |  | 
| `Boolean` | Delete(`Int32` index) |  | 
| `T` | Deserialize() |  | 
| `IEnumerable<String>` | GetDynamicMemberNames() |  | 
| `Boolean` | IsDefined(`String` name) |  | 
| `Boolean` | IsDefined(`Int32` index) |  | 
| `String` | ToString() |  | 
| `Boolean` | TryConvert(`ConvertBinder` binder, `Object&` result) |  | 
| `Boolean` | TryGetIndex(`GetIndexBinder` binder, `Object[]` indexes, `Object&` result) |  | 
| `Boolean` | TryGetMember(`GetMemberBinder` binder, `Object&` result) |  | 
| `Boolean` | TryInvoke(`InvokeBinder` binder, `Object[]` args, `Object&` result) |  | 
| `Boolean` | TryInvokeMember(`InvokeMemberBinder` binder, `Object[]` args, `Object&` result) |  | 
| `Boolean` | TrySetIndex(`SetIndexBinder` binder, `Object[]` indexes, `Object` value) |  | 
| `Boolean` | TrySetMember(`SetMemberBinder` binder, `Object` value) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | Parse(`String` json) |  | 
| `T` | Parse(`String` json) |  | 
| `T` | Parse(`String` json, `Encoding` Encoding) |  | 
| `Object` | Parse(`String` json, `Encoding` encoding) |  | 
| `Object` | Parse(`Stream` stream) |  | 
| `Object` | Parse(`Stream` stream, `Encoding` encoding) |  | 
| `String` | Serialize(`Object` obj) |  | 


