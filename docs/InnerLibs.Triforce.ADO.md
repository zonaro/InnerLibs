## `Triforce<ConnectionType>`

```csharp
public class InnerLibs.Triforce.ADO.Triforce<ConnectionType>
    : Triforce

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `ConnectionType` | Connection |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ConnectionString |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `TemplatePage<T>` | ApplyTemplate(`String` SQLQuery, `String` Template, `IEnumerable<Object>` Parameters, `Int32` PageNumber = 1, `Int32` PageSize = 0) | Executa uma query SQL e retorna um `System.Collections.IEnumerable` com os resultados (É um  wrapper para `System.Data.Common.DbCommand.ExecuteReader`  porém aplica os templates automaticamente | 
| `TemplatePage<Dictionary<String, Object>>` | ApplyTemplate(`String` SQLQuery, `String` Template, `IEnumerable<Object>` Parameters, `Int32` PageNumber = 1, `Int32` PageSize = 0) | Executa uma query SQL e retorna um `System.Collections.IEnumerable` com os resultados (É um  wrapper para `System.Data.Common.DbCommand.ExecuteReader`  porém aplica os templates automaticamente | 


