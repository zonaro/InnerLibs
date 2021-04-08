## `MenuItem`

```csharp
public class InnerLibs.MenuBuilder.MenuItem
    : MenuItem<Object>

```

## `MenuItem<T>`

Item de um InnerMenu
```csharp
public class InnerLibs.MenuBuilder.MenuItem<T>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Active | Indica se o menu está ativo (selecionado) | 
| `T` | Data | Informações relacionadas a este item | 
| `Boolean` | Enabled | Indica se o menu está habilitado | 
| `Boolean` | HasItems | Verifica se este item possui subitens | 
| `String` | Icon | Icone correspondente a este menu | 
| `MenuList<T>` | SubItems | Subitens do menu | 
| `String` | Target | Target do menu | 
| `String` | Title | Titulo do menu | 
| `String` | URL | URL do menu | 
| `Boolean` | Visible | Indica se o menu está visivel | 


## `MenuList`

```csharp
public class InnerLibs.MenuBuilder.MenuList
    : MenuList<Object>, IList<MenuItem<Object>>, ICollection<MenuItem<Object>>, IEnumerable<MenuItem<Object>>, IEnumerable, IList, ICollection, IReadOnlyList<MenuItem<Object>>, IReadOnlyCollection<MenuItem<Object>>

```

## `MenuList<T>`

Estrutura para criação de menus com submenus
```csharp
public class InnerLibs.MenuBuilder.MenuList<T>
    : List<MenuItem<T>>, IList<MenuItem<T>>, ICollection<MenuItem<T>>, IEnumerable<MenuItem<T>>, IEnumerable, IList, ICollection, IReadOnlyList<MenuItem<T>>, IReadOnlyCollection<MenuItem<T>>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | HasItems | Verifica se este menu possui itens | 


