## `Cluster`

```csharp
public class InnerLibs.DOTLanguage.Cluster
    : DotObject

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ID |  | 


## `DotAttributeCollection`

```csharp
public class InnerLibs.DOTLanguage.DotAttributeCollection
    : Dictionary<String, Object>, IDictionary<String, Object>, ICollection<KeyValuePair<String, Object>>, IEnumerable<KeyValuePair<String, Object>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<String, Object>, IReadOnlyCollection<KeyValuePair<String, Object>>, ISerializable, IDeserializationCallback

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `DotEdge`

Representa uma ligação entre nós de um grafico em DOT Language
```csharp
public class InnerLibs.DOTLanguage.DotEdge
    : DotObject

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DotNode` | ChildNode |  | 
| `String` | ID |  | 
| `Boolean` | Oriented | Indica se esta ligação é orientada ou não | 
| `DotNode` | ParentNode |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Escreve a DOT String desta ligaçao | 


## `DotNode`

Representa um nó de um grafico em DOT Language
```csharp
public class InnerLibs.DOTLanguage.DotNode
    : DotObject

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ID | ID deste nó | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Escreve a DOT string deste nó e seus respectivos nós filhos | 


## `DotObject`

```csharp
public abstract class InnerLibs.DOTLanguage.DotObject

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DotAttributeCollection` | Attributes |  | 
| `String` | ID |  | 


## `Graph`

Wrapper para criaçao de gráficos em DOT Language
```csharp
public class InnerLibs.DOTLanguage.Graph
    : List<DotObject>, IList<DotObject>, ICollection<DotObject>, IEnumerable<DotObject>, IEnumerable, IList, ICollection, IReadOnlyList<DotObject>, IReadOnlyCollection<DotObject>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<Cluster>` | Clusters |  | 
| `String` | GraphType | Tipo do Grafico (graph, digraph) | 
| `String` | ID | Nome do Gráfico | 
| `Boolean` | Strict |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Escreve a DOT string correspondente a este gráfico | 


