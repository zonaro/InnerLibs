## `MenuBuilder`

Estrutura para criação de menus com submenus
```csharp
public class InnerLibs.MenuBuilder.MenuBuilder
    : List<MenuBuilderItem>, IList<MenuBuilderItem>, ICollection<MenuBuilderItem>, IEnumerable<MenuBuilderItem>, IEnumerable, IList, ICollection, IReadOnlyList<MenuBuilderItem>, IReadOnlyCollection<MenuBuilderItem>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | HasItems | Verifica se este menu possui itens | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToJSON(`String` DateFormat = yyyy-MM-dd HH:mm:ss) | Transforma a classe em um json | 


## `MenuBuilderItem`

Item de um InnerMenu
```csharp
public class InnerLibs.MenuBuilder.MenuBuilderItem

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | HasItems | Verifica se este item possui subitens | 
| `String` | Icon | Icone correspondente a este menu | 
| `List<MenuBuilderItem>` | SubItems | Subitens do menu | 
| `String` | Target | Target do menu | 
| `String` | Title | Titulo do menu | 
| `String` | URL | URL do menu | 


