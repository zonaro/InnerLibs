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
public static class InnerLibs.LINQ.LINQExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AllFalse(`Boolean[]` Tests) |  | 
| `Boolean` | AllTrue(`Boolean[]` Tests) |  | 
| `Expression<Func<T, Boolean>>` | And(this `Expression<Func<T, Boolean>>` FirstExpression, `Expression`1[]` OtherExpressions) |  | 
| `Expression<Func<T, Boolean>>` | AndSearch(this `Expression<Func<T, Boolean>>` FirstExpression, `IEnumerable<String>` Text, `Expression`1[]` Properties) |  | 
| `Expression<Func<T, Boolean>>` | AndSearch(this `Expression<Func<T, Boolean>>` FirstExpression, `String` Text, `Expression`1[]` Properties) |  | 
| `Expression<Func<TModel, TToProperty>>` | ConvertParameterType(this `Expression<Func<TModel, TFromProperty>>` expression) |  | 
| `ConstantExpression` | CreateConstant(`Expression` Member, `IComparable` Value) |  | 
| `ConstantExpression` | CreateConstant(`Type` Type, `IComparable` Value) |  | 
| `ConstantExpression` | CreateConstant(`IComparable` Value) |  | 
| `PaginationFilter<T>` | CreateFilter(this `IEnumerable<T>` List) |  | 
| `PaginationFilter<T>` | CreateFilter(this `IEnumerable<T>` List, `Action<PaginationFilter<T, T>>` Configuration) |  | 
| `PaginationFilter<T, R>` | CreateFilter(this `IEnumerable<T>` List, `Func<T, R>` RemapExpression, `Action<PaginationFilter<T, R>>` Configuration) |  | 
| `PaginationFilter<T, R>` | CreateFilter(this `IEnumerable<T>` List, `Func<T, R>` RemapExpression) |  | 
| `Type` | CreateNullableType(this `Type` type) |  | 
| `MemberExpression` | CreatePropertyExpression(this `Expression<Func<T, V>>` Property) |  | 
| `Expression<Func<T, Boolean>>` | CreateWhereExpression(this `Boolean` DefaultReturnValue) |  | 
| `Expression<Func<T, Boolean>>` | CreateWhereExpression(`Expression<Func<T, Boolean>>` predicate) |  | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property, `Func<T, TOrder>` OrderBy, `Boolean` Descending = False) |  | 
| `IEnumerable<T>` | DistinctBy(this `IEnumerable<T>` Items, `Func<T, TKey>` Property) |  | 
| `IQueryable<T>` | DistinctBy(this `IQueryable<T>` Items, `Expression<Func<T, TKey>>` Property) |  | 
| `IEnumerable<T>` | Each(this `IEnumerable<T>` items, `Action<T>` Action) |  | 
| `BinaryExpression` | Equal(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `IEnumerable<T>` | FilterDateRange(this `IEnumerable<T>` List, `Expression<Func<T, Nullable<DateTime>>>` Property, `DateRange` Range, `Nullable<DateRangeFilterBehavior>` FilterBehavior = null) |  | 
| `IQueryable<T>` | FilterDateRange(this `IQueryable<T>` List, `Expression<Func<T, DateTime>>` Property, `DateRange` Range, `Nullable<DateRangeFilterBehavior>` FilterBehavior = null) |  | 
| `IQueryable<T>` | FilterDateRange(this `IQueryable<T>` List, `Expression<Func<T, Nullable<DateTime>>>` Property, `DateRange` Range, `Nullable<DateRangeFilterBehavior>` FilterBehavior = null) |  | 
| `IEnumerable<T>` | FilterDateRange(this `IEnumerable<T>` List, `Expression<Func<T, DateTime>>` Property, `DateRange` Range, `Nullable<DateRangeFilterBehavior>` FilterBehavior = null) |  | 
| `IQueryable<T>` | FilterRange(this `IQueryable<T>` List, `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `IEnumerable<V>` Values) |  | 
| `IQueryable<T>` | FilterRange(this `IQueryable<T>` List, `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `V[]` Values) |  | 
| `T` | FirstOr(this `IEnumerable<T>` source, `T[]` Alternate) |  | 
| `T` | FirstOr(this `IEnumerable<T>` source, `Func<T, Boolean>` predicade, `T[]` Alternate) |  | 
| `T` | FirstOrDefaultExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) |  | 
| `void` | FixNullable(`Expression&` e1, `Expression&` e2) |  | 
| `ParameterExpression` | GenerateParameterExpression() |  | 
| `ParameterExpression` | GenerateParameterExpression(this `Type` Type) |  | 
| `String` | GenerateParameterName(this `Type` Type) |  | 
| `FieldInfo` | GetFieldInfo(this `Expression<Func<TSource, TProperty>>` propertyLambda) |  | 
| `MemberInfo` | GetMemberInfo(this `Expression<Func<TSource, TProperty>>` propertyLambda) |  | 
| `BinaryExpression` | GetOperatorExpression(`Expression` Member, `String` Operator, `IEnumerable<IComparable>` PropertyValues, `FilterConditional` Conditional = Or) |  | 
| `PropertyInfo` | GetPropertyInfo(this `Expression<Func<TSource, TProperty>>` propertyLambda) |  | 
| `PropertyInfo` | GetPropertyInfo(this `TSource` source, `Expression<Func<TSource, TProperty>>` propertyLambda) |  | 
| `BinaryExpression` | GreaterThan(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `BinaryExpression` | GreaterThanOrEqual(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `Dictionary<Int64, IEnumerable<Tsource>>` | GroupByPage(this `IQueryable<Tsource>` source, `Int32` PageSize) |  | 
| `Dictionary<Int64, IEnumerable<Tsource>>` | GroupByPage(this `IEnumerable<Tsource>` source, `Int32` PageSize) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `IEnumerable<V>` Values) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `V[]` Values) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, V>>` Property, `V` MinValue, `V` MaxValue) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, DateTime>>` Property, `DateRange` DateRange) |  | 
| `Expression<Func<T, Boolean>>` | IsBetween(this `Expression<Func<T, Nullable<DateTime>>>` Property, `DateRange` DateRange) |  | 
| `Expression<Func<T, Boolean>>` | IsBetweenOrEqual(this `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `IEnumerable<V>` Values) |  | 
| `Expression<Func<T, Boolean>>` | IsBetweenOrEqual(this `Expression<Func<T, V>>` MinProperty, `Expression<Func<T, V>>` MaxProperty, `V[]` Values) |  | 
| `Expression<Func<T, Boolean>>` | IsBetweenOrEqual(this `Expression<Func<T, V>>` Property, `V` MinValue, `V` MaxValue) |  | 
| `Expression<Func<T, Boolean>>` | IsEqual(this `Expression<Func<T, V>>` Property, `V` Value) |  | 
| `Expression<Func<T, Boolean>>` | IsInDateRange(this `Expression<Func<T, DateTime>>` Property, `DateRange` DateRange, `Nullable<DateRangeFilterBehavior>` FilterBehavior = null) |  | 
| `Expression<Func<T, Boolean>>` | IsInDateRange(this `Expression<Func<T, Nullable<DateTime>>>` Property, `DateRange` DateRange, `Nullable<DateRangeFilterBehavior>` FilterBehavior = null) |  | 
| `T` | LastOr(this `IEnumerable<T>` source, `T[]` Alternate) |  | 
| `T` | LastOr(this `IEnumerable<T>` source, `Func<T, Boolean>` predicade, `T[]` Alternate) |  | 
| `BinaryExpression` | LessThan(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `BinaryExpression` | LessThanOrEqual(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `Boolean` | Most(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate, `Boolean` Result = True) |  | 
| `Boolean` | Most(this `IEnumerable<Boolean>` List, `Boolean` Result = True) |  | 
| `Boolean` | MostFalse(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) |  | 
| `Boolean` | MostFalse(`Boolean[]` Tests) |  | 
| `Boolean` | MostTrue(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) |  | 
| `Boolean` | MostTrue(`Boolean[]` Tests) |  | 
| `BinaryExpression` | NotEqual(this `Expression` MemberExpression, `Expression` ValueExpression) |  | 
| `Expression<Func<T, Boolean>>` | Or(this `Expression<Func<T, Boolean>>` FirstExpression, `Expression`1[]` OtherExpressions) |  | 
| `IOrderedEnumerable<T>` | OrderByLike(this `IEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) |  | 
| `IOrderedQueryable<T>` | OrderByMany(this `IQueryable<T>` Data, `Boolean` Ascending, `Expression`1[]` Selectors) |  | 
| `IOrderedEnumerable<T>` | OrderByMany(this `IEnumerable<T>` Data, `Expression`1[]` Selectors) |  | 
| `IOrderedEnumerable<T>` | OrderByMany(this `IEnumerable<T>` Data, `Boolean` Ascending, `Expression`1[]` Selectors) |  | 
| `IOrderedQueryable<T>` | OrderByMany(this `IQueryable<T>` Data, `Expression`1[]` Selectors) |  | 
| `IOrderedEnumerable<T>` | OrderByManyDescending(this `IEnumerable<T>` Data, `Expression`1[]` Selectors) |  | 
| `IOrderedQueryable<T>` | OrderByManyDescending(this `IQueryable<T>` Data, `Expression`1[]` Selectors) |  | 
| `IEnumerable<T>` | OrderByPredefinedOrder(this `IEnumerable<T>` Source, `Expression<Func<T, TOrder>>` PropertySelector, `TOrder[]` order) |  | 
| `IOrderedEnumerable<T>` | OrderByRandom(this `IEnumerable<T>` items) |  | 
| `IOrderedQueryable<T>` | OrderByRandom(this `IQueryable<T>` items) |  | 
| `IOrderedEnumerable<T>` | OrderByWithPriority(this `IEnumerable<T>` items, `Func`2[]` Priority) |  | 
| `Expression<Func<T, Boolean>>` | OrSearch(this `Expression<Func<T, Boolean>>` FirstExpression, `IEnumerable<String>` Text, `Expression`1[]` Properties) |  | 
| `Expression<Func<T, Boolean>>` | OrSearch(this `Expression<Func<T, Boolean>>` FirstExpression, `String` Text, `Expression`1[]` Properties) |  | 
| `IQueryable<TSource>` | Page(this `IQueryable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) |  | 
| `IEnumerable<TSource>` | Page(this `IEnumerable<TSource>` Source, `Int32` PageNumber, `Int32` PageSize) |  | 
| `IEnumerable<ValueTuple<T, T>>` | PairUp(this `IEnumerable<T>` source) |  | 
| `Expression` | PropertyExpression(this `ParameterExpression` Parameter, `String` PropertyName) |  | 
| `T` | RandomItem(this `IEnumerable<T>` l) |  | 
| `T` | RandomItem(this `IEnumerable<T>` l, `Func<T, Boolean>` predicade) |  | 
| `T` | RandomItemOr(this `IEnumerable<T>` l, `T[]` Alternate) |  | 
| `T` | RandomItemOr(this `IEnumerable<T>` l, `Func<T, Boolean>` predicade, `T[]` Alternate) |  | 
| `List<T>` | RemoveWhere(this `List<T>` list, `Expression<Func<T, Boolean>>` predicate) |  | 
| `IQueryable<ClassType>` | Search(this `IQueryable<ClassType>` Table, `IEnumerable<String>` SearchTerms, `Expression`1[]` Properties) |  | 
| `IQueryable<ClassType>` | Search(this `IQueryable<ClassType>` Table, `String` SearchTerm, `Expression`1[]` Properties) |  | 
| `Expression<Func<T, Boolean>>` | SearchExpression(this `IEnumerable<String>` Text, `Expression`1[]` Properties) |  | 
| `Expression<Func<T, Boolean>>` | SearchExpression(this `String` Text, `Expression`1[]` Properties) |  | 
| `IOrderedEnumerable<ClassType>` | SearchInOrder(this `IEnumerable<ClassType>` Table, `IEnumerable<String>` SearchTerms, `Expression`1[]` Properties) |  | 
| `IOrderedQueryable<ClassType>` | SearchInOrder(this `IQueryable<ClassType>` Table, `IEnumerable<String>` SearchTerms, `Expression`1[]` Properties) |  | 
| `String` | SelectJoinString(this `IEnumerable<TSource>` Source, `String` Separator = ) |  | 
| `String` | SelectJoinString(this `IEnumerable<TSource>` Source, `Func<TSource, String>` Selector, `String` Separator = ) |  | 
| `String` | SelectManyJoinString(this `IEnumerable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ) |  | 
| `String` | SelectManyJoinString(this `IQueryable<TSource>` Source, `Func<TSource, IEnumerable<String>>` Selector = null, `String` Separator = ) |  | 
| `T` | SingleOrDefaultExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `Object` PropertyValue, `Boolean` Is = True) |  | 
| `IEnumerable<T>` | SkipLast(this `IEnumerable<T>` l, `Int32` Count = 1) |  | 
| `IEnumerable<T>` | TakeLast(this `IEnumerable<T>` l, `Int32` Count = 1) |  | 
| `IEnumerable<T>` | TakeRandom(this `IEnumerable<T>` l, `Int32` Count = 1) |  | 
| `IEnumerable<T>` | TakeRandom(this `IEnumerable<T>` l, `Func<T, Boolean>` predicade, `Int32` Count = 1) |  | 
| `IOrderedQueryable<T>` | ThenByLike(this `IQueryable<T>` items, `String[]` Searches, `String` SortProperty, `Boolean` Ascending = True) |  | 
| `IOrderedQueryable<T>` | ThenByLike(this `IQueryable<T>` items, `IEnumerable<String>` Searches, `Expression<Func<T, String>>` SortProperty, `Boolean` Ascending = True) |  | 
| `IOrderedEnumerable<T>` | ThenByLike(this `IEnumerable<T>` items, `Func<T, String>` PropertySelector, `Boolean` Ascending, `String[]` Searches) |  | 
| `IOrderedEnumerable<T>` | ThenByList(this `IOrderedEnumerable<T>` Source, `T[]` OrderSource) |  | 
| `IOrderedQueryable<T>` | ThenByProperty(this `IQueryable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) |  | 
| `IOrderedEnumerable<T>` | ThenByProperty(this `IEnumerable<T>` source, `String[]` SortProperty, `Boolean` Ascending = True) |  | 
| `IEnumerable<T>` | Traverse(this `IEnumerable<T>` items, `Func<T, IEnumerable<T>>` ChildSelector) |  | 
| `IEnumerable<T>` | Traverse(this `T` item, `Func<T, T>` ParentSelector) |  | 
| `IEnumerable<T>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Boolean` IncludeMe = False) |  | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, P>` PropertySelector, `Boolean` IncludeMe = False) |  | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, IEnumerable<P>>` PropertySelector, `Boolean` IncludeMe = False) |  | 
| `IEnumerable<P>` | Traverse(this `T` Item, `Func<T, IEnumerable<T>>` ChildSelector, `Func<T, IQueryable<P>>` PropertySelector, `Boolean` IncludeMe = False) |  | 
| `Expression<Func<Type, Boolean>>` | WhereExpression(`String` PropertyName, `String` Operator, `IEnumerable<IComparable>` PropertyValue, `Boolean` Is = True, `FilterConditional` Conditional = Or) |  | 
| `Expression<Func<Type, Boolean>>` | WhereExpression(`Expression<Func<Type, V>>` PropertySelector, `String` Operator, `IEnumerable<IComparable>` PropertyValue, `Boolean` Is = True, `FilterConditional` Conditional = Or) |  | 
| `IQueryable<T>` | WhereExpression(this `IQueryable<T>` List, `String` PropertyName, `String` Operator, `IEnumerable<IComparable>` PropertyValue, `Boolean` Is = True, `Boolean` Exclusive = True) |  | 
| `IEnumerable<T>` | WhereNotBlank(this `IEnumerable<T>` List) |  | 
| `IQueryable<T>` | WhereNotNull(this `IQueryable<T>` List) |  | 
| `IEnumerable<T>` | WhereNotNull(this `IEnumerable<T>` List) |  | 
| `IEnumerable<Type>` | WhereType(this `IEnumerable<BaseType>` List) |  | 


## `PaginationFilter<ClassType>`

```csharp
public class InnerLibs.LINQ.PaginationFilter<ClassType>
    : PaginationFilter<ClassType, ClassType>

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PaginationFilter<ClassType>` | Config(`Action<PaginationFilter<ClassType>>` options) |  | 


## `PaginationFilter<ClassType, RemapType>`

```csharp
public class InnerLibs.LINQ.PaginationFilter<ClassType, RemapType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<ClassType>` | Data |  | 
| `BinaryExpression` | Filter |  | 
| `IEnumerable<PropertyFilter<ClassType, RemapType>>` | Filters |  | 
| `Int32` | FirstPage |  | 
| `Boolean` | IsFirstPage |  | 
| `Boolean` | IsFirstPageNecessary |  | 
| `Boolean` | IsFirstTraillingNecessary |  | 
| `Boolean` | IsLastPage |  | 
| `Boolean` | IsLastPageNecessary |  | 
| `Boolean` | IsLastTraillingNecessary |  | 
| `Boolean` | IsNextPageNecessary |  | 
| `Boolean` | IsPaginationNecessary |  | 
| `Boolean` | IsPreviousPageNecessary |  | 
| `RemapType[]` | Item |  | 
| `Expression<Func<ClassType, Boolean>>` | LambdaExpression |  | 
| `Int32` | LastPage |  | 
| `Int32` | NextPage |  | 
| `IEnumerable<String>` | PageButtons |  | 
| `Int32` | PageCount |  | 
| `Int32` | PageNumber |  | 
| `String` | PageNumberQueryParameter |  | 
| `Int32[]` | PageRange |  | 
| `Int32` | PageSize |  | 
| `String` | PageSizeQueryParameter |  | 
| `Int32` | PaginationOffset |  | 
| `String` | PaginationOffsetQueryParameter |  | 
| `ParameterExpression` | Parameter |  | 
| `Int32` | PreviousPage |  | 
| `Func<ClassType, RemapType>` | RemapExpression |  | 
| `Int32` | Total |  | 
| `List<Expression<Func<ClassType, Boolean>>>` | WhereFilters |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PropertyFilter<ClassType, RemapType>` | And(`String` PropertyName, `Boolean` Enabled = True) |  | 
| `PropertyFilter<ClassType, RemapType>` | And(`Expression<Func<ClassType, T>>` PropertyName, `Boolean` Enabled = True) |  | 
| `Int32` | ButtonCount(`String` Trailling = ...) |  | 
| `PaginationFilter<ClassType, RemapType>` | Compute() |  | 
| `PaginationFilter<ClassType, RemapType>` | Config(`Action<PaginationFilter<ClassType, RemapType>>` options) |  | 
| `Boolean` | ContainsPage(`IEnumerable<Int32>` PageNumbers) |  | 
| `Boolean` | ContainsPage(`Int32[]` PageNumbers) |  | 
| `IEnumerable<String>` | CreatePaginationButtons(`String` Trailling = ...) |  | 
| `String` | CreateQueryString(`Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `PaginationFilter<ClassType, RemapType>` | CreateSearch(`IEnumerable<IComparable>` PropertyValues, `Expression`1[]` PropertyNames) |  | 
| `PaginationFilter<ClassType, RemapType>` | CreateSearch(`IEnumerable<IComparable>` PropertyValues, `String[]` PropertyNames) |  | 
| `String` | CreateUrl(`String` Url, `Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `String` | CreateUrlFromPattern(`String` UrlPattern, `Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `IQueryable<ClassType>` | GetEnumerablePage() |  | 
| `IEnumerable<ClassType>` | GetEnumerablePage(`Int32` PageNumber) |  | 
| `String` | GetFilterQueryString(`Boolean` ForceEnabled = False) |  | 
| `RemapType[]` | GetPage(`Int32` PageNumber) |  | 
| `RemapType[]` | GetPage() |  | 
| `String` | GetPaginationQueryString(`Nullable<Int32>` PageNumber = null, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `IQueryable<ClassType>` | GetQueryablePage() |  | 
| `IQueryable<ClassType>` | GetQueryablePage(`Int32` PageNumber) |  | 
| `Boolean` | IsCurrentPage(`Int32` Index) |  | 
| `Int32` | JumpPages(`Int32` Quantity) |  | 
| `PropertyFilter<ClassType, RemapType>` | Or(`String` PropertyName, `Boolean` Enabled = True) |  | 
| `PropertyFilter<ClassType, RemapType>` | Or(`Expression<Func<ClassType, T>>` PropertyName, `Boolean` Enabled = True) |  | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`Expression`1[]` Selectors) |  | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`Expression<Func<ClassType, T>>` Selector, `Boolean` Descending = False) |  | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`String[]` Selector, `Boolean` Descending = False) |  | 
| `PaginationFilter<ClassType, RemapType>` | OrderBy(`String` Selector, `Boolean` Descending = False) |  | 
| `PaginationFilter<ClassType, RemapType>` | OrderByDescending(`Expression<Func<ClassType, T>>` Selector) |  | 
| `String` | PageButtonsFromTemplate(`String` Template, `String` TraillingTemplate, `String` SeparatorTemplate = , `String` Trailling = ...) |  | 
| `String` | PageButtonsFromTemplate(`String` Template, `String` SeparatorTemplate = ) |  | 
| `PaginationFilter<ClassType, RemapType>` | SetData(`IEnumerable<ClassType>` List) |  | 
| `PaginationFilter<ClassType, RemapType>` | SetData(`IQueryable<ClassType>` List) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`Expression<Func<ClassType, T>>` PropertyName, `FilterConditional` Conditional = Or, `Boolean` Enabled = True) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`String` PropertyName, `FilterConditional` Conditional = Or, `Boolean` Enabled = True) |  | 
| `PaginationFilter<ClassType, RemapType>` | SetPage(`Int32` PageNumber) |  | 
| `PaginationFilter<ClassType, RemapType>` | SetPagination(`Int32` PageSize, `Int32` PaginationOffset) |  | 
| `PaginationFilter<ClassType, RemapType>` | SetPagination(`Int32` PageSize) |  | 
| `PaginationFilter<ClassType, RemapType>` | SetPaginationQueryParameters(`String` PageNumber, `String` PageSize, `String` PaginationOffset) |  | 
| `Dictionary<String, Object>` | ToDictionary(`Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `NameValueCollection` | ToNameValueCollection(`Nullable<Int32>` PageNumber = null, `Boolean` ForceEnabled = False, `Boolean` IncludePageSize = False, `Boolean` IncludePaginationOffset = False) |  | 
| `String` | ToString() |  | 
| `PaginationFilter<ClassType, RemapType>` | UseArrayDictionary(`IDictionary<String, IComparable[]>` Collection, `String` DefaultOperator = =) |  | 
| `PaginationFilter<ClassType, RemapType>` | UseDictionary(`IDictionary<String, IComparable>` Collection, `String` DefaultOperator = =) |  | 
| `PaginationFilter<ClassType, RemapType>` | UseNameValueCollection(`NameValueCollection` Collection, `String` DefaultOperator = =) |  | 
| `PaginationFilter<ClassType, RemapType>` | UseQueryString(`String` Query, `String` DefaultOperator = =) |  | 
| `PaginationFilter<ClassType, RemapType>` | UseQueryStringExpression(`String` QueryExpression, `String` Separator = :, `FilterConditional` Conditional = And) |  | 
| `PaginationFilter<ClassType, RemapType>` | Where(`Expression<Func<ClassType, Boolean>>` predicate) |  | 
| `PaginationFilter<ClassType, RemapType>` | WhereIf(`Boolean` Test, `Expression<Func<ClassType, Boolean>>` predicate) |  | 


## `PropertyFilter<ClassType, RemapType>`

```csharp
public class InnerLibs.LINQ.PropertyFilter<ClassType, RemapType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AcceptNullValues |  | 
| `Boolean` | CompareWith |  | 
| `FilterConditional` | Conditional |  | 
| `Boolean` | Enabled |  | 
| `BinaryExpression` | Filter |  | 
| `Boolean` | Is |  | 
| `Expression` | Member |  | 
| `String` | Operator |  | 
| `PaginationFilter<ClassType, RemapType>` | PaginationFilter |  | 
| `ParameterExpression` | Parameter |  | 
| `String` | PropertyName |  | 
| `IEnumerable<IComparable>` | PropertyValues |  | 
| `String` | QueryStringSeparator |  | 
| `FilterConditional` | ValuesConditional |  | 
| `Expression<Func<IComparable, Boolean>>` | ValueValidation |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PropertyFilter<ClassType, RemapType>` | AddValues(`Nullable`1[]` Values) |  | 
| `PropertyFilter<ClassType, RemapType>` | AllowNull() |  | 
| `PropertyFilter<ClassType, RemapType>` | CompareFalse() |  | 
| `PropertyFilter<ClassType, RemapType>` | CompareTrue() |  | 
| `PaginationFilter<ClassType, RemapType>` | Contains(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | Contains(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | ContainsAll(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | ContainsAll(`IEnumerable<T>` Values) |  | 
| `String` | CreateQueryParameter(`Boolean` ForceEnabled = False, `Boolean` OnlyValid = True) |  | 
| `PaginationFilter<ClassType, RemapType>` | CrossContains(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | CrossContains(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | EndsWith(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | EndsWith(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | Equal(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | Equal(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThan(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThan(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThanOrEqual(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | GreaterThanOrEqual(`IEnumerable<T>` Values) |  | 
| `PropertyFilter<ClassType, RemapType>` | IgnoreNull() |  | 
| `PaginationFilter<ClassType, RemapType>` | LessThan(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | LessThan(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | LessThanOrEqual(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | LessThanOrEqual(`IEnumerable<T>` Values) |  | 
| `PropertyFilter<ClassType, RemapType>` | Negate() |  | 
| `PaginationFilter<ClassType, RemapType>` | NotEqual(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | NotEqual(`IEnumerable<T>` Values) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetEnabled(`Boolean` Enabled = True) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`Expression<Func<ClassType, T>>` PropertySelector, `FilterConditional` Conditional = Or) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetMember(`String` PropertyName, `FilterConditional` Conditional = Or) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetOperator(`String` Operator) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetValue(`T` Value) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetValue(`Nullable<T>` Value) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetValues(`T[]` Values) |  | 
| `PropertyFilter<ClassType, RemapType>` | SetValues(`IEnumerable<T>` Values) |  | 
| `PaginationFilter<ClassType, RemapType>` | StartsWith(`Nullable<T>` Value) |  | 
| `PaginationFilter<ClassType, RemapType>` | StartsWith(`IEnumerable<T>` Values) |  | 
| `String` | ToString() |  | 
| `IEnumerable<IComparable>` | ValidValues() |  | 


