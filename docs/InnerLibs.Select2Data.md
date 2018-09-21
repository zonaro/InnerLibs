## `AditionalData`

Informações adicionais a este resultado
```csharp
public class InnerLibs.Select2Data.AditionalData
    : Dictionary<String, Object>, IDictionary<String, Object>, ICollection<KeyValuePair<String, Object>>, IEnumerable<KeyValuePair<String, Object>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<String, Object>, IReadOnlyCollection<KeyValuePair<String, Object>>, ISerializable, IDeserializationCallback

```

## `Group`

Grupo de resultado, rendereiza como optgroup
```csharp
public class InnerLibs.Select2Data.Group
    : Select2ResultType

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<Result>` | children | Options deste optgroup | 
| `String` | id | id do optgroup | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | ToHtmlElement() |  | 


## `Pagination`

```csharp
public class InnerLibs.Select2Data.Pagination

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | more |  | 


## `Result`

Resultado, renderiza como option
```csharp
public class InnerLibs.Select2Data.Result
    : Select2ResultType

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | disabled | Atributo disabled do option | 
| `String` | id | Campo utilizado como value | 
| `Boolean` | selected | atributo selected do option | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | ToHtmlElement() |  | 


## `Select2Results<Type>`

Classe base para serializaçao de json para um Select2
```csharp
public class InnerLibs.Select2Data.Select2Results<Type>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AditionalData` | otherdata | Informações adicionais (formato QueryString) | 
| `Pagination` | pagination | Objeto paginação do Select2 | 
| `List<Type>` | results | Lista de options ou optgroups atrelados a este conjunto de resultados | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | ToHtmlElement(`String` Name = ) | Converte este objeto para um `InnerLibs.HtmlParser.HtmlElement` (Select com options) | 
| `String` | ToJSON() | Serializa um Json para o Select2 | 
| `String` | ToString() | retorna a string json deste select2 | 


## `Select2ResultType`

Classe Base para resultados do Select2. Deve ser herdada
```csharp
public abstract class InnerLibs.Select2Data.Select2ResultType

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AditionalData` | otherdata | informacao extra anexada a este optgroup ou option | 
| `String` | text | Texto deste optgroup ou option | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | ToHtmlElement() | Converte este objeto para `InnerLibs.HtmlParser.HtmlElement` | 


