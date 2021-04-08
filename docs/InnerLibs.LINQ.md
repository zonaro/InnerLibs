## `LINQExtensions`

```csharp
public class InnerLibs.LINQ.LINQExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AllFalse(`Boolean[]` Tests) | Retorna TRUE se a todos os testes em uma lista retornarem FALSE | 
| `Boolean` | AllTrue(`Boolean[]` Tests) | Retorna TRUE se a todos os testes em uma lista retornarem TRUE | 
| `Expression<Func<T, Boolean>>` | And(this `Expression<Func<T, Boolean>>` expr1, `Expression<Func<T, Boolean>>` expr2) |  | 
| `Expression<Func<TTargetParm, TTargetReturn>>` | ConvertGeneric(this `Expression<Func<TParm, TReturn>>` input) | Retorna uma expressão genérica a partir de uma expressão tipada | 
| `Expression<Func<T, Boolean>>` | CreateExpression(`Boolean` DefaultReturnValue = True) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `Expression<Func<T, Boolean>>` | CreateExpression(`Expression<Func<T, Boolean>>` predicate) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `Expression<Func<T, T2>>` | CreateExpression(this `Expression<Func<T, T2>>` predicate) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `PaginationInfo<T, IQueryable<T>>` | CreatePaginationInfo(this `IQueryable<T>` List, `Int32` PageNumber, `Int32` PageSize, `Int32` PaginationOffset = 3, `Object` Filter = null) |  | 
| `PaginationInfo<T, IEnumerable<T>>` | CreatePaginationInfo(this `IEnumerable<T>` List, `Int32` PageNumber, `Int32` PageSize, `Int32` PaginationOffset = 3, `Object` Filter = null) |  | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property, `Func<T, TOrder>` OrderBy, `Boolean` Descending = False) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IQueryable<T>` | DistinctBy(this `IQueryable<T>` Items, `Expression<Func<T, TKey>>` Property) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `T` | FirstOrDefaultExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) | Busca em um `System.Linq.IQueryable`1` usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `IEnumerable<T>` | ForEach(this `IEnumerable<T>` Items, `Action<T>` Action) | Realiza uma acão para cada item de uma lista. | 
| `PropertyInfo` | GetPropertyInfo(this `TSource` source, `Expression<Func<TSource, TProperty>>` propertyLambda) | Retorna as informacoes de uma propriedade a partir de um seletor | 
| `Dictionary<Int64, IEnumerable<Tsource>>` | GroupByPage(this `IQueryable<Tsource>` source, `Int32` PageSize) | Criar um `System.Collections.Generic.Dictionary`2` agrupando os itens em páginas de um tamanho especifico | 
| `Dictionary<Int64, IEnumerable<Tsource>>` | GroupByPage(this `IEnumerable<Tsource>` source, `Int32` PageSize) | Criar um `System.Collections.Generic.Dictionary`2` agrupando os itens em páginas de um tamanho especifico | 
| `Object` | HasSamePropertyValues(this `T` Obj1, `T` Obj2, `Func`2[]` Properties) | Verifica se uma instancia de uma classe possui propriedades especificas com valores igual  as de outra instancia da mesma classe | 
| `Boolean` | Most(this `IEnumerable<Boolean>` List, `Boolean` Result = True) | Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente | 
| `Boolean` | Most(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate, `Boolean` Result = True) | Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente | 
| `Boolean` | MostFalse(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) | Retorna TRUE se a maioria dos testes em uma lista retornarem false | 
| `Boolean` | MostFalse(`Boolean[]` Tests) | Retorna TRUE se a maioria dos testes em uma lista retornarem false | 
| `Boolean` | MostTrue(`Boolean[]` Tests) | Retorna TRUE se a maioria dos testes em uma lista retornarem true | 
| `Boolean` | MostTrue(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) | Retorna TRUE se a maioria dos testes em uma lista retornarem true | 
| `Expression<Func<T, Boolean>>` | Or(this `Expression<Func<T, Boolean>>` expr1, `Expression<Func<T, Boolean>>` expr2) | Concatena uma expressão com outra usando o operador OR (||) | 
| `IOrderedQueryable<T>` | OrderBy(this `IQueryable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `IOrderedQueryable<T>` | OrderBy(this `IEnumerable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `Object` | OrderByLike(this `IQueryable<T>` items, `String[]` Searches, `String` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | OrderByLike(this `IEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | OrderByList(this `IOrderedEnumerable<T>` Source, `T[]` OrderSource) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir de outra lista do mesmo tipo | 
| `IOrderedEnumerable<T>` | OrderByRandom(this `IEnumerable<T>` items) | Randomiza a ordem de um `System.Collections.IEnumerable` | 
| `IOrderedQueryable<T>` | OrderByRandom(this `IQueryable<T>` items) | Randomiza a ordem de um `System.Collections.IEnumerable` | 
| `IQueryable<TSource>` | Page(this `IQueryable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) | Reduz um `System.Linq.IQueryable` em uma página especifica | 
| `IEnumerable<TSource>` | Page(this `IEnumerable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) | Reduz um `System.Linq.IQueryable` em uma página especifica | 
| `IOrderedEnumerable<ClassType>` | Search(this `IEnumerable<ClassType>` Table, `String[]` SearchTerms, `Func`2[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `IOrderedQueryable<ClassType>` | Search(this `IQueryable<ClassType>` Table, `String[]` SearchTerms, `Expression`1[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `String` | SelectJoin(this `IEnumerable<TSource>` Source, `String` Separator = ) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectJoin(this `IEnumerable<TSource>` Source, `Func<TSource, String>` Selector = null, `String` Separator = ) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectJoin(this `IQueryable<TSource>` Source, `Func<TSource, String>` Selector = null, `String` Separator = ) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectManyJoin(this `IEnumerable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ) | Seleciona e une em uma unica string varios elementos enumeraveis | 
| `String` | SelectManyJoin(this `IQueryable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ;) | Seleciona e une em uma unica string varios elementos enumeraveis | 
| `T` | SingleOrDefaultExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) | Busca em um `System.Linq.IQueryable`1` usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `IEnumerable<T>` | TakeAndOrder(this `IEnumerable<T>` items, `Func<T, Boolean>` Priority, `Func<T, DefaultOrderType>` DefaultOrder = null) | Ordena um `System.Collections.IEnumerable` priorizando valores especificos a uma condição no  inicio da coleção e então segue uma ordem padrão para os outros. | 
| `IOrderedEnumerable<T>` | ThenBProperty(this `IOrderedEnumerable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir do nome de uma ou mais propriedades | 
| `IOrderedQueryable<T>` | ThenBy(this `IOrderedQueryable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `IOrderedQueryable<T>` | ThenByLike(this `IOrderedQueryable<T>` items, `String[]` Searches, `String` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedQueryable<T>` | ThenByLike(this `IOrderedQueryable<T>` items, `String[]` Searches, `Expression<Func<T, String>>` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | ThenByLike(this `IOrderedEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | ThenByList(this `IOrderedEnumerable<T>` Source, `T[]` OrderSource) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir de outra lista do mesmo tipo | 
| `IEnumerable<T>` | Traverse(this `IEnumerable<T>` items, `Func<T, IEnumerable<T>>` ChildSelector) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<T>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, P>` PropertySelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, IEnumerable<P>>` PropertySelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, IQueryable<P>>` PropertySelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `Expression<Func<Type, Boolean>>` | WhereExpression(`String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) | Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `IQueryable<T>` | WhereExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) | Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 


