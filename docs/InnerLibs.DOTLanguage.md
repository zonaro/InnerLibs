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

```csharp
public class InnerLibs.DOTLanguage.DotEdge
    : DotObject

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DotNode` | ChildNode |  | 
| `String` | ID |  | 
| `Boolean` | Oriented |  | 
| `DotNode` | ParentNode |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `DotNode`

```csharp
public class InnerLibs.DOTLanguage.DotNode
    : DotObject

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ID |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


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

```csharp
public class InnerLibs.DOTLanguage.Graph
    : List<DotObject>, IList<DotObject>, ICollection<DotObject>, IEnumerable<DotObject>, IEnumerable, IList, ICollection, IReadOnlyList<DotObject>, IReadOnlyCollection<DotObject>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<Cluster>` | Clusters |  | 
| `GraphType` | GraphType |  | 
| `String` | ID |  | 
| `Boolean` | Strict |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `GraphType`

```csharp
public enum InnerLibs.DOTLanguage.GraphType
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Graph |  | 
| `1` | Digraph |  | 


