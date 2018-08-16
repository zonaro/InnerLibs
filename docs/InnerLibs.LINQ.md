## `LINQExtensions`

```csharp
public class InnerLibs.LINQ.LINQExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Expression<Func<T, Boolean>>` | And(this `Expression<Func<T, Boolean>>` expr1, `Expression<Func<T, Boolean>>` expr2) |  | 
| `HtmlControl[]` | ApplyToControls(this `T` Obj, `HtmlControl[]` Controls) | Aplica os valores encontrados nas propriedades de uma entidade em controles com mesmo ID  das colunas. Se os conroles não existirem no resultado eles serão ignorados. | 
| `Expression<Func<TTargetParm, TTargetReturn>>` | ConvertGeneric(`Expression<Func<TParm, TReturn>>` input) | Retorna uma express~zo genérica a partir de uma expressão tipada | 
| `Expression<Func<T, Boolean>>` | CreateExpression(`Boolean` DefaultReturnValue = True) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `Expression<Func<T, Boolean>>` | CreateExpression(`Expression<Func<T, Boolean>>` predicate) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `Expression<Func<T, T2>>` | CreateExpression(`Expression<Func<T, T2>>` predicate) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property, `Func<T, TOrder>` OrderBy, `Boolean` Descending = False) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IQueryable<T>` | DistinctBy(this `IQueryable<T>` Items, `Expression<Func<T, TKey>>` Property) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IEnumerable<T>` | ForEach(this `IEnumerable<T>` Items, `Action<T>` Action) | Realiza uma acão para cada item de uma lista. | 
| `T` | GetByPrimaryKey(this `DataContext` Context, `Object` ID, `Boolean` CreateIfNotExists = False, `Boolean&` IsNew = False) | Retorna um objeto de uma tabela especifica de acordo com uma chave primária. | 
| `IEnumerable<T>` | GetByPrimaryKeys(this `DataContext` Context, `Object[]` IDs) | Retorna um objeto de uma tabela especifica de acordo com sua chave primaria. Pode  opcionalmente criar o objeto se o mesmo não existir | 
| `Dictionary<Int64, List<Tsource>>` | GroupByPage(this `IQueryable<Tsource>` source, `Int32` PageSize) | Criar um `System.Collections.Generic.Dictionary`2` agrupando os itens em páginas de um tamanho especifico | 
| `Dictionary<Int64, List<Tsource>>` | GroupByPage(this `IEnumerable<Tsource>` source, `Int32` PageSize) | Criar um `System.Collections.Generic.Dictionary`2` agrupando os itens em páginas de um tamanho especifico | 
| `Object` | HasSamePropertyValues(this `T` Obj1, `T` Obj2, `Func`2[]` Properties) | Verifica se uma instancia de uma classe possui propriedades especificas com valores igual  as de outra instancia da mesma classe | 
| `Expression<Func<T, Boolean>>` | Or(this `Expression<Func<T, Boolean>>` expr1, `Expression<Func<T, Boolean>>` expr2) | Concatena uma expressão com outra usando o operador OR (||) | 
| `Object` | OrderBy(this `IQueryable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `Object` | OrderByLike(this `IQueryable<T>` items, `String[]` Searches, `String` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | OrderByLike(this `IEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | OrderByRandom(this `IEnumerable<T>` items) | Randomiza a ordem de um `System.Collections.IEnumerable` | 
| `IOrderedQueryable<T>` | OrderByRandom(this `IQueryable<T>` items) | Randomiza a ordem de um `System.Collections.IEnumerable` | 
| `IQueryable<TSource>` | Page(this `IQueryable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) | Reduz um `System.Linq.IQueryable` em uma página especifica | 
| `IEnumerable<TSource>` | Page(this `IEnumerable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) | Reduz um `System.Linq.IQueryable` em uma página especifica | 
| `IOrderedQueryable<ClassType>` | Search(this `DataContext` Context, `String` SearchTerm, `String[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `IOrderedQueryable<ClassType>` | Search(this `DataContext` Context, `String[]` SearchTerms, `String[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `IOrderedQueryable<ClassType>` | Search(this `DataContext` Context, `String[]` SearchTerms, `Expression`1[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `String` | SelectJoin(this `IEnumerable<TSource>` Source, `Func<TSource, String>` Selector = null, `String` Separator = ;) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectJoin(this `IQueryable<TSource>` Source, `Func<TSource, String>` Selector = null, `String` Separator = ;) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectManyJoin(this `IEnumerable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ;) | Seleciona e une em uma unica string varios elementos enumeraveis | 
| `String` | SelectManyJoin(this `IQueryable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ;) | Seleciona e une em uma unica string varios elementos enumeraveis | 
| `IEnumerable<T>` | TakeAndOrder(this `IEnumerable<T>` items, `Func<T, Boolean>` Priority, `Func<T, DefaultOrderType>` DefaultOrder = null) | Ordena um `System.Collections.IEnumerable` priorizando valores especificos a uma condição no  inicio da coleção e então segue uma ordem padrão para os outros. | 
| `IOrderedQueryable<T>` | ThenBy(this `IOrderedQueryable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `IOrderedQueryable<T>` | ThenByLike(this `IOrderedQueryable<T>` items, `String[]` Searches, `String` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedQueryable<T>` | ThenByLike(this `IOrderedQueryable<T>` items, `String[]` Searches, `Expression<Func<T, String>>` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | ThenByLike(this `IOrderedEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `T` | UpdateObjectFromDictionary(this `DataContext` Context, `IDictionary<String, Object>` Dic) | Atualiza um objeto de entidade a partir de valores em um Dictionary | 
| `T` | UpdateObjectFromRequest(this `DataContext` Context, `HttpRequest` Request, `String[]` Keys) | Atualiza um objeto de entidade a partir de valores em um HttpRequest | 


