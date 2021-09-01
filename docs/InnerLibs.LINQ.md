## `FilterConditional`

```csharp
public enum InnerLibs.LINQ.FilterConditional
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Or |  | 
| `1` | And |  | 


## `LINQExtensions`

```csharp
public class InnerLibs.LINQ.LINQExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AllFalse(`Boolean[]` Tests) | Retorna TRUE se a todos os testes em uma lista retornarem FALSE | 
| `Boolean` | AllTrue(`Boolean[]` Tests) | Retorna TRUE se a todos os testes em uma lista retornarem TRUE | 
| `Expression<Func<T, Boolean>>` | And(this `Expression<Func<T, Boolean>>` FirstExpression, `Expression`1[]` OtherExpressions) |  | 
| `ConstantExpression` | CreateConstant(`Expression` Member, `IComparable` Value) |  | 
| `ConstantExpression` | CreateConstant(`Type` Type, `IComparable` Value) |  | 
| `ConstantExpression` | CreateConstant(`IComparable` Value) |  | 
| `PaginationFilter<T, T>` | CreateFilter(this `IEnumerable<T>` List) | Retorna um `InnerLibs.LINQ.PaginationFilter`2` para a lista especificada | 
| `PaginationFilter<T, T>` | CreateFilter(this `IEnumerable<T>` List, `Action<PaginationFilter<T, T>>` Configuration) | Retorna um `InnerLibs.LINQ.PaginationFilter`2` para a lista especificada | 
| `PaginationFilter<T, R>` | CreateFilter(this `IEnumerable<T>` List, `Func<T, R>` RemapExpression, `Action<PaginationFilter<T, R>>` Configuration) | Retorna um `InnerLibs.LINQ.PaginationFilter`2` para a lista especificada | 
| `PaginationFilter<T, R>` | CreateFilter(this `IEnumerable<T>` List, `Func<T, R>` RemapExpression) | Retorna um `InnerLibs.LINQ.PaginationFilter`2` para a lista especificada | 
| `Type` | CreateNullableType(this `Type` type) |  | 
| `MemberExpression` | CreatePropertyExpression(this `Expression<Func<T, V>>` Property) |  | 
| `Expression<Func<T, Boolean>>` | CreateWhereExpression(this `Boolean` DefaultReturnValue) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `Expression<Func<T, Boolean>>` | CreateWhereExpression(`Expression<Func<T, Boolean>>` predicate) | Cria uma `System.Linq.Expressions.Expression` condicional a partir de um valor `System.Boolean` | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property, `Func<T, TOrder>` OrderBy, `Boolean` Descending = False) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `IQueryable<T>` | DistinctBy(this `IQueryable<T>` Items, `Expression<Func<T, TKey>>` Property) | Distingui os items de uma lista a partir de uma propriedade da classe | 
| `BinaryExpression` | Equal(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `IQueryable<T>` | FilterDateRange(this `IQueryable<T>` List, `Expression<Func<T, DateTime>>` Property, `DateRange` Range) |  | 
| `IQueryable<T>` | FilterDateRange(this `IQueryable<T>` List, `Expression<Func<T, Nullable<DateTime>>>` Property, `DateRange` Range) |  | 
| `IQueryable<T>` | FilterDateRange(this `IQueryable<T>` List, `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `IEnumerable<V>` Values) |  | 
| `IQueryable<T>` | FilterDateRange(this `IQueryable<T>` List, `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `V[]` Values) |  | 
| `T` | FirstOrDefaultExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) | Busca em um `System.Linq.IQueryable`1` usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `T` | FirstRandom(this `IEnumerable<T>` l) |  | 
| `void` | FixNullable(this `Expression&` e1, `Expression&` e2) |  | 
| `ParameterExpression` | GenerateParameterExpression() | Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável | 
| `ParameterExpression` | GenerateParameterExpression(this `Type` Type) | Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável | 
| `String` | GenerateParameterName(this `Type` Type) |  | 
| `BinaryExpression` | GetOperatorExpression(`Expression` Member, `String` Operator, `IEnumerable<IComparable>` PropertyValues, `FilterConditional` Conditional = Or) | Retorna uma expressão de comparação para um ou mais valores | 
| `PropertyInfo` | GetPropertyInfo(this `Expression<Func<TSource, TProperty>>` propertyLambda) | Retorna as informacoes de uma propriedade a partir de um seletor | 
| `PropertyInfo` | GetPropertyInfo(this `TSource` source, `Expression<Func<TSource, TProperty>>` propertyLambda) | Retorna as informacoes de uma propriedade a partir de um seletor | 
| `BinaryExpression` | GreaterThan(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `BinaryExpression` | GreaterThanOrEqual(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `Dictionary<Int64, IEnumerable<Tsource>>` | GroupByPage(this `IQueryable<Tsource>` source, `Int32` PageSize) | Criar um `System.Collections.Generic.Dictionary`2` agrupando os itens em páginas de um tamanho especifico | 
| `Dictionary<Int64, IEnumerable<Tsource>>` | GroupByPage(this `IEnumerable<Tsource>` source, `Int32` PageSize) | Criar um `System.Collections.Generic.Dictionary`2` agrupando os itens em páginas de um tamanho especifico | 
| `Object` | HasSamePropertyValues(this `T` Obj1, `T` Obj2, `Func`2[]` Properties) | Verifica se uma instancia de uma classe possui propriedades especificas com valores igual  as de outra instancia da mesma classe | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `IEnumerable<V>` Values) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `V[]` Values) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, V>>` Property, `V` MinValue, `V` MaxValue) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, DateTime>>` Property, `DateRange` DateRange) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, Nullable<DateTime>>>` Property, `DateRange` DateRange) |  | 
| `BinaryExpression` | LessThan(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `BinaryExpression` | LessThanOrEqual(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `Boolean` | Most(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate, `Boolean` Result = True) | Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente | 
| `Boolean` | Most(this `IEnumerable<Boolean>` List, `Boolean` Result = True) | Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente | 
| `Boolean` | MostFalse(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) | Retorna TRUE se a maioria dos testes em uma lista retornarem false | 
| `Boolean` | MostFalse(`Boolean[]` Tests) | Retorna TRUE se a maioria dos testes em uma lista retornarem false | 
| `Boolean` | MostTrue(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) | Retorna TRUE se a maioria dos testes em uma lista retornarem true | 
| `Boolean` | MostTrue(`Boolean[]` Tests) | Retorna TRUE se a maioria dos testes em uma lista retornarem true | 
| `BinaryExpression` | NotEqual(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `Expression<Func<T, Boolean>>` | Or(this `Expression<Func<T, Boolean>>` FirstExpression, `Expression`1[]` OtherExpressions) | Concatena uma expressão com outra usando o operador OR (||) | 
| `IOrderedEnumerable<T>` | OrderByLike(this `IEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) | Orderna uma lista a partir da aproximaçao de um deerminado campo com uma string | 
| `IOrderedEnumerable<T>` | OrderByRandom(this `IEnumerable<T>` items) | Randomiza a ordem de um `System.Collections.IEnumerable` | 
| `IOrderedQueryable<T>` | OrderByRandom(this `IQueryable<T>` items) | Randomiza a ordem de um `System.Collections.IEnumerable` | 
| `IEnumerable<T>` | OrderByWithPriority(this `IEnumerable<T>` items, `Func<T, Boolean>` Priority, `Func<T, DefaultOrderType>` DefaultOrder = null) | Ordena um `System.Collections.IEnumerable` priorizando valores especificos a uma condição no  inicio da coleção e então segue uma ordem padrão para os outros. | 
| `IQueryable<TSource>` | Page(this `IQueryable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) | Reduz um `System.Linq.IQueryable` em uma página especifica | 
| `IEnumerable<TSource>` | Page(this `IEnumerable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) | Reduz um `System.Linq.IQueryable` em uma página especifica | 
| `Expression` | PropertyExpression(this `ParameterExpression` Parameter, `String` PropertyName) |  | 
| `IOrderedEnumerable<ClassType>` | Search(this `IEnumerable<ClassType>` Table, `String[]` SearchTerms, `Func`2[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `IOrderedQueryable<ClassType>` | Search(this `IQueryable<ClassType>` Table, `String[]` SearchTerms, `Expression`1[]` Properties) | Retorna um `System.Linq.IQueryable`1` procurando em varios campos diferentes de uma entidade | 
| `String` | SelectJoin(this `IEnumerable<TSource>` Source, `String` Separator = ) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectJoin(this `IEnumerable<TSource>` Source, `Func<TSource, String>` Selector = null, `String` Separator = ) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectJoin(this `IQueryable<TSource>` Source, `Func<TSource, String>` Selector = null, `String` Separator = ) | Seleciona e une em uma unica string varios elementos | 
| `String` | SelectManyJoin(this `IEnumerable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ) | Seleciona e une em uma unica string varios elementos enumeraveis | 
| `String` | SelectManyJoin(this `IQueryable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ;) | Seleciona e une em uma unica string varios elementos enumeraveis | 
| `T` | SingleOrDefaultExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) | Busca em um `System.Linq.IQueryable`1` usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `IEnumerable<T>` | TakeRandom(this `IEnumerable<T>` l, `Int32` Count) |  | 
| `IOrderedQueryable<T>` | ThenByLike(this `IQueryable<T>` items, `String[]` Searches, `String` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedQueryable<T>` | ThenByLike(this `IQueryable<T>` items, `String[]` Searches, `Expression<Func<T, String>>` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | ThenByLike(this `IEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir da aproximaçao de uma ou mais  `System.String` com o valor de um determinado campo | 
| `IOrderedEnumerable<T>` | ThenByList(this `IOrderedEnumerable<T>` Source, `T[]` OrderSource) | Ordena um `System.Collections.Generic.IEnumerable`1` a partir de outra lista do mesmo tipo | 
| `IOrderedQueryable<T>` | ThenByProperty(this `IQueryable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `IOrderedEnumerable<T>` | ThenByProperty(this `IEnumerable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) | Ordena um `System.Linq.IQueryable`1` a partir do nome de uma ou mais propriedades | 
| `IEnumerable<T>` | Traverse(this `IEnumerable<T>` items, `Func<T, IEnumerable<T>>` ChildSelector) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<T>` | Traverse(this `T` item, `Func<T, T>` ParentSelector) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<T>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, P>` PropertySelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, IEnumerable<P>>` PropertySelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, IQueryable<P>>` PropertySelector, `Boolean` IncludeMe = False) | Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente | 
| `Expression<Func<Type, Boolean>>` | WhereExpression(`String` PropertyName, `String` Operator, `IEnumerable<IComparable>` PropertyValue, `Boolean` Is = True, `FilterConditional` Conditional = Or) | Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `Expression<Func<Type, Boolean>>` | WhereExpression(`Expression<Func<Type, V>>` PropertySelector, `String` Operator, `IEnumerable<IComparable>` PropertyValue, `Boolean` Is = True, `FilterConditional` Conditional = Or) | Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 
| `IQueryable<T>` | WhereExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `IEnumerable<IComparable>` PropertyValue, `Boolean` Is = True, `Boolean` Exclusive = True) | Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor | 


## `PaginationFilter<ClassType>`

```csharp
public class InnerLibs.LINQ.PaginationFilter<ClassType>
    : PaginationFilter<ClassType, ClassType>

```

## `PaginationFilter<ClassType, RemapType>`

Classe para criação de paginação e filtros dinâmicos para listas de classes
```csharp
public class InnerLibs.LINQ.PaginationFilter<ClassType, RemapType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<ClassType>` | Data | Fonte de Dados deste filtro | 
| `BinaryExpression` | Filter | Expressão binária contendo todos os filtros | 
| `IEnumerable<PropertyFilter<ClassType, RemapType>>` | Filters | Filtros | 
| `Int32` | FirstPage | Numero da primeira pagina | 
| `Boolean` | IsFirstPage | Retorna true se esta pagina é a primeira | 
| `Boolean` | IsFirstPageNecessary | Retorna true se existir o botão de primeira página for necessário | 
| `Object` | IsFirstTraillingNecessary | Indica se o primeiro botão de reticencias é necessário | 
| `Boolean` | IsLastPage | Retorna true se esta pagina é a ultima | 
| `Boolean` | IsLastPageNecessary | Retorna true se existir o botão de primeira página for necessário | 
| `Object` | IsLastTraillingNecessary | Indica se o ultimo botão de reticencias é necessário | 
| `Boolean` | IsNextPageNecessary | Retorna true se existir o botão de proxima pagina for necessário | 
| `Boolean` | IsPaginationNecessary | Retorna true se existir mais de uma pagina | 
| `Boolean` | IsPreviousPageNecessary | Retorna true se existir o botão de pagina anterior for necessário | 
| `Expression<Func<ClassType, Boolean>>` | LambdaExpression | Expressão lambda deste filtro | 
| `Int32` | LastPage | Numero da ultima pagina | 
| `Int32` | NextPage | Numero da proxima pagina | 
| `RemapType[]` | Page | Dados da Pagina Atual | 
| `IEnumerable<String>` | PageButtons | Botões de paginação | 
| `Int32` | PageCount | Quantidade de páginas | 
| `Int32` | PageNumber | Numero da pagina | 
| `String` | PageNumberQueryParameter |  | 
| `Int32[]` | PageRange | Retorna um range de páginas a partir da pagina atual | 
| `Int32` | PageSize | Quantidade de itens por página | 
| `String` | PageSizeQueryParameter |  | 
| `Int32` | PaginationOffset | Quantidade média de "botões de paginação" contidas no `InnerLibs.LINQ.PaginationFilter`2.PageRange` | 
| `String` | PaginationOffsetQueryParameter |  | 
| `ParameterExpression` | Parameter | Parametro utilizado na contrução da expressão lambda | 
| `Int32` | PreviousPage | Numero da pagina anterior | 
| `Func<ClassType, RemapType>` | RemapExpression | Expressão de remapeamento da coleção | 
| `Int32` | Total | Total de itens da Lista | 
| `List<Expression<Func<ClassType, Boolean>>>` | WhereFilters | Expressões adicionadas a clausula where junto com os filtros | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PropertyFilter<ClassType, RemapType>` | And(`String` PropertyName, `Boolean` Enabled = True) |  | 
| `PropertyFilter<ClassType, RemapType>` | And(`Expression<Func<ClassType, T>>` PropertyName, `Boolean` Enabled = True) |  | 
| `Int32` | ButtonCount(`Object` Trailling = ...) | Quantidade de botões de paginação | 
| `PaginationFilter<ClassType, RemapType>` | Compute() | Força o `System.Linq.IQueryable` a executar (sem paginação) | 
| `PaginationFilter<ClassType, RemapType>` | Config(`Action<PaginationFilter<ClassType, RemapType>>` options) | Configura este Filtro | 
| `Boolean` | ContainsPage(`IEnumerable<Int32>` PageNumbers) | Verifica se o `InnerLibs.LINQ.PaginationFilter`2.PageRange` contém algumas páginas especificas | 
| `Boolean` | ContainsPage(`Int32[]` PageNumbers) | Verifica se o `InnerLibs.LINQ.PaginationFilter`2.PageRange` contém algumas páginas especificas | 
| `IEnumerable<String>` | CreatePaginationButtons(`String` Trailling = ...) | Cria uma lista de strings utilizadas nos botões de paginação | 
| `String` | CreateQueryString(`Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) | Cria uma querystring com  paginacao e os filtros ativos | 
| `PaginationFilter<ClassType, RemapType>` | CreateSearch(`IEnumerable<IComparable>` PropertyValues, `Expression`1[]` PropertyNames) | Seta uma busca usando <see cref="!:Contains()" /> em `` para cada propriedade em `` | 
| `PaginationFilter<ClassType, RemapType>` | CreateSearch(`IEnumerable<IComparable>` PropertyValues, `String[]` PropertyNames) | Seta uma busca usando <see cref="!:Contains()" /> em `` para cada propriedade em `` | 
| `String` | CreateUrl(`String` Url, `Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) | Cria uma Url com a query string deste filtro | 
| `String` | CreateUrlFromPattern(`String` UrlPattern, `Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) | Cria uma url a partir de um pattern de Url e concatena a query string | 
| `IQueryable<ClassType>` | GetEnumerablePage() | Retorna `InnerLibs.LINQ.PaginationFilter`2.Data` com os filtros aplicados | 
| `IEnumerable<ClassType>` | GetEnumerablePage(`Int32` PageNumber) | Retorna `InnerLibs.LINQ.PaginationFilter`2.Data` com os filtros aplicados | 
| `String` | GetFilterQueryString(`Boolean` ForceEnabled = False) | Cria uma querystring com os filtros ativos | 
| `RemapType[]` | GetPage(`Int32` PageNumber) | Executa o Filtro e retorna os dados paginados | 
| `RemapType[]` | GetPage() | Executa o Filtro e retorna os dados paginados | 
| `String` | GetPaginationQueryString(`Nullable<Int32>` PageNumber = null, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) | Retorna a parte da querystring usada para paginacao | 
| `IQueryable<ClassType>` | GetQueryablePage() | Retorna `InnerLibs.LINQ.PaginationFilter`2.Data` com os filtros aplicados | 
| `IQueryable<ClassType>` | GetQueryablePage(`Int32` PageNumber) | Retorna `InnerLibs.LINQ.PaginationFilter`2.Data` com os filtros aplicados | 
| `Boolean` | IsCurrentPage(`Int32` Index) | Verifica se á pagina atual é igual a uma pagina especifica | 
| `PropertyFilter<ClassType, RemapType>` | Or(`String` PropertyName, `Boolean` Enabled = True) |  | 
| `PropertyFilter<ClassType, RemapType>` | Or(`Expression<Func<ClassType, T>>` PropertyName, `Boolean` Enabled = True) |  | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`Expression`1[]` Selectors) | Ordena os resultados da lista | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`Expression<Func<ClassType, T>>` Selector, `Boolean` Descending = False) | Ordena os resultados da lista | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`String[]` Selector, `Boolean` Descending = False) | Ordena os resultados da lista | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`String` Selector, `Boolean` Descending = False) | Ordena os resultados da lista | 
| `PaginationFilter<ClassType, RemapType>` | OrderByDescending(`Expression<Func<ClassType, T>>` Selector) |  | 
| `String` | PageButtonsFromTemplate(`String` Template, `String` TraillingTemplate, `String` SeparatorTemplate = , `String` Trailling = ...) | Aplica a paginação a um template | 
| `String` | PageButtonsFromTemplate(`String` Template, `String` SeparatorTemplate = ) | Aplica a paginação a um template | 
| `PaginationFilter<ClassType, RemapType>` | SetData(`IEnumerable<ClassType>` List) | Seta a lista com os dados a serem filtrados nesse filtro | 
| `PaginationFilter<ClassType, RemapType>` | SetData(`IQueryable<ClassType>` List) | Seta a lista com os dados a serem filtrados nesse filtro | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`Expression<Func<ClassType, T>>` PropertyName, `FilterConditional` Conditional = Or, `Boolean` Enabled = True) | Configura um novo membro para este filtro | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`String` PropertyName, `FilterConditional` Conditional = Or, `Boolean` Enabled = True) | Configura um novo membro para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | SetPage(`Int32` PageNumber) | Seta a pagina atual | 
| `PaginationFilter<ClassType, RemapType>` | SetPagination(`Int32` PageSize, `Int32` PaginationOffset) | Configura a paginação do filtro | 
| `PaginationFilter<ClassType, RemapType>` | SetPagination(`Int32` PageSize) | Configura a paginação do filtro | 
| `PaginationFilter<ClassType, RemapType>` | SetPaginationQueryParameters(`String` PageNumber, `String` PageSize, `String` PaginationOffset) | Seta os parametros utilizados na querystring para a paginação | 
| `Dictionary<String, Object>` | ToDictionary(`Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `NameValueCollection` | ToNameValueCollection(`Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `String` | ToString() | Retorna uma QueryString que representa este filtro | 
| `PaginationFilter<ClassType, RemapType>` | UseArrayDictionary(`IDictionary<String, IComparable[]>` Collection, `String` DefaultOperator = =) | Configura este LambDafilter para utilizar um Dictionary como Filtro. | 
| `PaginationFilter<ClassType, RemapType>` | UseDictionary(`IDictionary<String, IComparable>` Collection, `String` DefaultOperator = =) | Configura este LambDafilter para utilizar um Dictionary como Filtro. | 
| `PaginationFilter<ClassType, RemapType>` | UseNameValueCollection(`NameValueCollection` Collection, `String` DefaultOperator = =) | Extrai os parametros de um `System.Collections.Specialized.NameValueCollection` e seta os membros usando as Keys como membros | 
| `PaginationFilter<ClassType, RemapType>` | UseQueryString(`String` Query, `String` DefaultOperator = =) | Configura este Filtro para utilizar uma querystring. | 
| `PaginationFilter<ClassType, RemapType>` | UseQueryStringExpression(`String` QueryExpression, `String` Separator = :, `FilterConditional` Conditional = And) |  | 
| `PaginationFilter<ClassType, RemapType>` | Where(`Expression<Func<ClassType, Boolean>>` predicate) | Adciona Expressões a clausula where junto com os filtros | 
| `PaginationFilter<ClassType, RemapType>` | WhereIf(`Boolean` Test, `Expression<Func<ClassType, Boolean>>` predicate) | Adciona Expressões a clausula where junto com os filtros se uma condiçao for cumprida | 


## `PropertyFilter<ClassType, RemapType>`

```csharp
public class InnerLibs.LINQ.PropertyFilter<ClassType, RemapType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AcceptNullValues | Configura este filtro para utilização de valores nulos na query | 
| `Boolean` | CompareWith |  | 
| `FilterConditional` | Conditional |  | 
| `Boolean` | Enabled | Indica se este filtro está ativo | 
| `BinaryExpression` | Filter | Expressão binaria deste filtro | 
| `Boolean` | Is | Comparara o valor do filtro com TRUE ou FALSE | 
| `Expression` | Member | Expressão do membro utilizado no filtro | 
| `String` | Operator | Operador usado nesse filtro | 
| `PaginationFilter<ClassType, RemapType>` | PaginationFilter |  | 
| `ParameterExpression` | Parameter | Parametro da expressão lambda | 
| `String` | PropertyName |  | 
| `IEnumerable<IComparable>` | PropertyValues | Valores a serem testados por esse filtro | 
| `String` | QueryStringSeparator | Separador utilizado pelo <see cref="!:CreateQueryParameter(Boolean)" /> | 
| `FilterConditional` | ValuesConditional |  | 
| `Expression<Func<IComparable, Boolean>>` | ValueValidation |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PropertyFilter<ClassType, RemapType>` | AddValues(`Nullable`1[]` Values) | Adciona varios valores para esse filtro testar. | 
| `PropertyFilter<ClassType, RemapType>` | AllowNull() | Permite que valores nulos sejam adcionados ao filtro | 
| `PropertyFilter<ClassType, RemapType>` | CompareFalse() | Força uma comparação negativa para este filtro | 
| `PropertyFilter<ClassType, RemapType>` | CompareTrue() | Força uma comparação positiva para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | Contains(`Nullable<T>` Value) | Seta o operador para Contains e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | Contains(`IEnumerable<T>` Values) | Seta o operador para Contains e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | ContainsAll(`Nullable<T>` Value) | Seta o operador para Contains e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | ContainsAll(`IEnumerable<T>` Values) | Seta o operador para Contains e o Valor para este filtro | 
| `String` | CreateQueryParameter(`Boolean` ForceEnabled = False, `Boolean` OnlyValid = True) | Retorna uma string em formato de parametro de QueryString deste filtro | 
| `PaginationFilter<ClassType, RemapType>` | CrossContains(`Nullable<T>` Value) | Seta o operador para CrossContains e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | CrossContains(`IEnumerable<T>` Values) | Seta o operador para CrossContains e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | EndsWith(`Nullable<T>` Value) | Seta o operador para EndsWith e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | EndsWith(`IEnumerable<T>` Values) | Seta o operador para EndsWith e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | Equal(`Nullable<T>` Value) | Seta o operador para = e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | Equal(`IEnumerable<T>` Values) | Seta o operador para = e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThan(`Nullable<T>` Value) | Seta o operador para &gt; e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThan(`IEnumerable<T>` Values) | Seta o operador para &gt; e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThanOrEqual(`Nullable<T>` Value) | Seta o operador para  &gt;= e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThanOrEqual(`IEnumerable<T>` Values) | Seta o operador para  &gt;= e o Valor para este filtro | 
| `PropertyFilter<ClassType, RemapType>` | IgnoreNull() | Impede que valores nulos sejam adcionados ao filtro | 
| `PaginationFilter<ClassType, RemapType>` | LessThan(`Nullable<T>` Value) | Seta o operador para &lt; e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | LessThan(`IEnumerable<T>` Values) | Seta o operador para &lt; e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | LessThanOrEqual(`Nullable<T>` Value) | Seta o operador para   &lt;= e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | LessThanOrEqual(`IEnumerable<T>` Values) | Seta o operador para   &lt;= e o Valor para este filtro | 
| `PropertyFilter<ClassType, RemapType>` | Negate() | Nega o filtro atual | 
| `PaginationFilter<ClassType, RemapType>` | NotEqual(`Nullable<T>` Value) | Seta o operador para  != e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | NotEqual(`IEnumerable<T>` Values) | Seta o operador para  != e o Valor para este filtro | 
| `PropertyFilter<ClassType, RemapType>` | SetEnabled(`Boolean` Enabled = True) | Ativa ou desativa esse filtro durante a construção da expressão | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`Expression<Func<ClassType, T>>` PropertySelector, `FilterConditional` Conditional = Or) | Sete um membro para ser utilizado neste filtro. É ignorado quando seus Values estão nulos ou vazios | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`String` PropertyName, `FilterConditional` Conditional = Or) | Sete um membro para ser utilizado neste filtro. É ignorado quando seus Values estão nulos ou vazios | 
| `PropertyFilter<ClassType, RemapType>` | SetOperator(`String` Operator) | Seta o operador utilizado nesse filtro | 
| `PropertyFilter<ClassType, RemapType>` | SetValue(`T` Value) | Seta um unico valor para esse filtro testar. Substitui os antigos | 
| `PropertyFilter<ClassType, RemapType>` | SetValue(`Nullable<T>` Value) | Seta um unico valor para esse filtro testar. Substitui os antigos | 
| `PropertyFilter<ClassType, RemapType>` | SetValues(`T[]` Values) | Seta varios valores para esse filtro testar. Substitui os valores antigos | 
| `PropertyFilter<ClassType, RemapType>` | SetValues(`IEnumerable<T>` Values) | Seta varios valores para esse filtro testar. Substitui os valores antigos | 
| `PaginationFilter<ClassType, RemapType>` | StartsWith(`Nullable<T>` Value) | Seta o operador para StartsWith e o Valor para este filtro | 
| `PaginationFilter<ClassType, RemapType>` | StartsWith(`IEnumerable<T>` Values) | Seta o operador para StartsWith e o Valor para este filtro | 
| `String` | ToString() |  | 
| `IEnumerable<IComparable>` | ValidValues() | Retorna apenas os valores validos para este filtro (`InnerLibs.LINQ.PropertyFilter`2.AcceptNullValues` e `InnerLibs.LINQ.PropertyFilter`2.ValueValidation`) | 


