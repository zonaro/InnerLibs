## `Triforce<DataContextType>`

Permite integrar `InnerLibs.Triforce.Triforce` a objetos LINQ to SQL
```csharp
public class InnerLibs.Triforce.LINQ.Triforce<DataContextType>
    : Triforce

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `DataContextType` | DataContext |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `TemplatePage<T>` | ApplyTemplate(`Expression<Func<T, Boolean>>` predicade, `Int32` PageNumber = 0, `Int32` PageSize = 0) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`String` Template, `Expression<Func<T, Boolean>>` predicade = null, `Int32` PageNumber = 0, `Int32` PageSize = 0) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `Template<T>` | ApplyTemplate(`T` Item, `String` Template = ) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`String` SQLQuery, `String` Template, `IEnumerable<Object>` Parameters, `Int32` PageNumber = 1, `Int32` PageSize = 0) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`IQueryable<T>` List, `Int32` PageNumber = 1, `Int32` PageSize = 0, `String` Template = ) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`EntitySet<T>` List, `Int32` PageNumber = 1, `Int32` PageSize = 0, `String` Template = ) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`ISingleResult<T>` List, `Int32` PageNumber = 1, `Int32` PageSize = 0, `String` Template = ) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`String` TemplateName, `Object[]` Parameters) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate() | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`Table<T>` List, `Int32` PageNumber = 1, `Int32` PageSize = 0, `String` Template = ) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `TemplatePage<T>` | ApplyTemplate(`IEnumerable<T>` List, `Int32` PageNumber = 1, `Int32` PageSize = 0, `String` Template = ) | Aplica um template a uma busca determinada pelo tipo de objeto | 
| `String` | GetCommand(`String` CommandFile) | Pega o comando SQL de um arquivo ou resource | 
| `void` | ProcessSubTemplate(`T` item, `HtmlDocument` doc) |  | 


