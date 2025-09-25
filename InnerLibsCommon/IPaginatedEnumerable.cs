using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using Extensions;
using Extensions.DataBases;


namespace Extensions.Pagination
{


    /// <summary>
    /// Classe para criação de paginação e filtros dinâmicos para listas de classes
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public class PaginationFilter<TClass, TRemap> where TClass : class
    {
        #region Private Fields

        private int? _total;



        private int pgnumber = 1;

        private string pnp, psp, pop;

        private Func<TClass, TRemap> remapexp;

        #endregion Private Fields

        #region Private Methods

        private IEnumerable<TClass> ApplyFilter(IEnumerable<TClass> FilteredData)
        {
            FilteredData = FilteredData ?? Data;
            _total = default;

            if (FilteredData != null)
            {
                if (LambdaExpression != null)
                {
                    if (FilteredData is IOrderedQueryable<TClass> orderedQuery)
                    {
                        FilteredData = orderedQuery.Where(LambdaExpression);
                        var dq = FilteredData.Select(x => 0);
                        _total = dq.Count();
                    }

                    if (FilteredData is IQueryable<TClass> query)
                    {
                        FilteredData = query.Where(LambdaExpression);
                        var dq = FilteredData.Select(x => 0);
                        _total = dq.Count();

                    }
                    else
                    {
                        FilteredData = FilteredData.Where(LambdaExpression.Compile());
                        _total = FilteredData.Count();

                    }
                }
                else
                {
                    _total = FilteredData.Count();

                }

                return FilteredData;
            }

            return Data;
        }

        private IEnumerable<TClass> ApplyPage(IEnumerable<TClass> FilteredData)
        {
            if (FilteredData != null)
            {
                if (PageNumber > 0 && PageSize > 0)
                {
                    if (FilteredData is IOrderedQueryable<TClass> orderedQuery)
                    {
                        FilteredData = orderedQuery.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                    }
                    else if (FilteredData is IQueryable<TClass> query)
                    {
                        FilteredData = query.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                    }
                    else if (FilteredData is IOrderedEnumerable<TClass> orderedEnum)
                    {
                        FilteredData = orderedEnum.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                    }
                    else if (FilteredData is IEnumerable<TClass>)
                    {
                        FilteredData = FilteredData.Skip((PageNumber - 1) * PageSize).Take(PageSize);
                    }
                }
            }

            return FilteredData ?? Data;
        }

        #endregion Private Methods

        #region Internal Fields

        private List<PropertyFilter<TClass, TRemap>> _filters = new List<PropertyFilter<TClass, TRemap>>();

        internal ParameterExpression param = Util.GenerateParameterExpression<TClass>();

        #endregion Internal Fields

        #region Public Constructors

        public PaginationFilter()
        {
        }

        /// <summary>
        /// Cria uma nova instancia e seta a exclusividade de filtro
        /// </summary>
        public PaginationFilter(Func<TClass, TRemap> RemapExpression) => this.RemapExpression = RemapExpression;

        public PaginationFilter(Func<TClass, TRemap> RemapExpression, Action<PaginationFilter<TClass, TRemap>> Options)
        {
            this.RemapExpression = RemapExpression;
            Config(Options);
        }

        public PaginationFilter(Action<PaginationFilter<TClass, TRemap>> Options) => Config(Options);

        #endregion Public Constructors

        #region Public Indexers

        /// <summary>
        /// Dados da Pagina Atual
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public TRemap[] this[int PageNumber] => GetPage(PageNumber);

        #endregion Public Indexers

        #region Public Properties

        /// <summary>
        /// Fonte de Dados deste filtro
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TClass> Data { get; set; }

        /// <summary>
        /// Expressão binária contendo todos os filtros
        /// </summary>
        /// <returns></returns>
        public BinaryExpression Filter
        {
            get
            {
                Expression exp = null;
                foreach (var valor in Filters.Where(x => x != null && x.Filter != null && x.Enabled))
                {
                    if (exp is null)
                    {
                        exp = valor.Filter;
                    }
                    else if (valor.Conditional == FilterConditional.And)
                    {
                        exp = Expression.AndAlso(valor.Filter, exp);
                    }
                    else
                    {
                        exp = Expression.OrElse(valor.Filter, exp);
                    }

                }

                return (BinaryExpression)exp;
            }
        }

        /// <summary>
        /// Filtros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyFilter<TClass, TRemap>> Filters => _filters;

        /// <summary>
        /// Numero da primeira pagina
        /// </summary>
        /// <returns></returns>
        public int FirstPage => 1;

        /// <summary>
        /// Retorna true se esta pagina é a primeira
        /// </summary>
        /// <returns></returns>
        public bool IsFirstPage => PageNumber == FirstPage;

        /// <summary>
        /// Retorna true se o botão de primeira página for necessário
        /// </summary>
        /// <returns></returns>
        public bool IsFirstPageNecessary => !ContainsPage(FirstPage, FirstPage + 1) || (PageNumber - PaginationOffset) == 2;

        /// <summary>
        /// Indica se o primeiro botão de reticencias é necessário
        /// </summary>
        /// <returns></returns>
        public bool IsFirstTraillingNecessary => IsFirstPageNecessary && (PageNumber - PaginationOffset) > 2;

        /// <summary>
        /// Retorna true se esta pagina é a ultima
        /// </summary>
        /// <returns></returns>
        public bool IsLastPage => PageNumber == LastPage;

        /// <summary>
        /// Retorna true se o botão de ultima página for necessário
        /// </summary>
        /// <returns></returns>
        public bool IsLastPageNecessary => !ContainsPage(LastPage, LastPage - 1) || (PageNumber + PaginationOffset) == (PageCount - 1);

        /// <summary>
        /// Indica se o ultimo botão de reticencias é necessário
        /// </summary>
        /// <returns></returns>
        public bool IsLastTraillingNecessary => IsLastPageNecessary && (PageNumber + PaginationOffset) < LastPage - 1;

        /// <summary>
        /// Retorna true se o botão de proxima pagina for necessário
        /// </summary>
        /// <returns></returns>
        public bool IsNextPageNecessary => IsLastPageNecessary;

        /// <summary>
        /// Retorna true se existir mais de uma pagina
        /// </summary>
        /// <returns></returns>
        public bool IsPaginationNecessary => PageCount > 1;

        /// <summary>
        /// Retorna true se o botão de pagina anterior for necessário
        /// </summary>
        /// <returns></returns>
        public bool IsPreviousPageNecessary => IsFirstPageNecessary;

        /// <summary>
        /// Expressão lambda deste filtro
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TClass, bool>> LambdaExpression
        {
            get
            {
                Expression<Func<TClass, bool>> exp = null;
                if (Filter != null)
                {
                    exp = Expression.Lambda<Func<TClass, bool>>(Filter, param);
                }

                foreach (var valor in WhereFilters ?? new List<Expression<Func<TClass, bool>>>())
                {
                    if (valor != null)
                    {
                        if (exp == null)
                        {
                            exp = valor;
                        }
                        else
                        {
                            exp = exp.And(valor);
                        }
                    }
                }

                if (exp != null)
                {
                    while (exp.CanReduce)
                    {
                        exp = (Expression<Func<TClass, bool>>)exp.Reduce();
                    }
                }

                return exp;
            }
        }

        /// <summary>
        /// Numero da ultima pagina
        /// </summary>
        /// <returns></returns>
        public int LastPage => PageCount;

        /// <summary>
        /// Numero da proxima pagina
        /// </summary>
        /// <returns></returns>
        public int NextPage
        {
            get
            {
                var pg = PageNumber + 1;

                if (pg > LastPage)
                {
                    pg = FirstPage;
                }
                return pg;
            }
        }

        /// <summary>
        /// Botões de paginação
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> PageButtons => CreatePaginationButtons();

        /// <summary>
        /// Quantidade de páginas
        /// </summary>
        /// <returns></returns>
        public int PageCount => PageSize > 0 ? (Total / (double)PageSize).CeilInt() : 1;

        /// <summary>
        /// Numero da pagina
        /// </summary>
        /// <returns></returns>
        public int PageNumber
        {
            get => pgnumber;
            set
            {
                if (value < 1)
                {
                    pgnumber = LastPage + value;
                }
                else if (value > LastPage)
                {
                    pgnumber = (value - LastPage);
                }
                else
                {
                    pgnumber = value;
                }
            }
        }

        public string PageNumberQueryParameter
        {
            get => pnp.IfBlank(nameof(PageNumber));

            set => pnp = value;
        }

        /// <summary>
        /// Retorna um range de páginas a partir da pagina atual
        /// </summary>
        /// <returns></returns>
        public int[] PageRange
        {
            get
            {
                int frange = 1;
                int lrange = 1;
                if (PageCount > 1)
                {
                    frange = new[] { PageNumber - PaginationOffset, FirstPage }.Max();
                    lrange = new[] { PageNumber + PaginationOffset, LastPage }.Min();
                }

                var arr = new List<int>();
                for (int index = frange, loopTo = lrange; index <= loopTo; index++)
                {
                    arr.Add(index);
                }

                return arr.ToArray();
            }
        }

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        /// <returns></returns>
        public int PageSize { get; set; } = 0;

        public string PageSizeQueryParameter
        {
            get => psp.IfBlank(nameof(PageSize));

            set => psp = value;
        }

        /// <summary>
        /// Quantidade de botões de paginacao exibidos após a pagina atual e anterior a página atual
        /// </summary>
        public int PaginationOffset { get; set; } = 2;

        public string PaginationOffsetQueryParameter
        {
            get => pop.IfBlank(nameof(PaginationOffset));

            set => pop = value;
        }

        /// <summary>
        /// Parametro utilizado na contrução da expressão lambda
        /// </summary>
        /// <returns></returns>
        public ParameterExpression Parameter => param;

        /// <summary>
        /// Numero da pagina anterior
        /// </summary>
        /// <returns></returns>
        public int PreviousPage
        {
            get
            {
                var pg = PageNumber - 1;

                if (pg < 1)
                {
                    pg = LastPage;
                }
                return pg;
            }
        }

        /// <summary>
        /// Expressão de remapeamento da coleção
        /// </summary>
        /// <returns></returns>
        public Func<TClass, TRemap> RemapExpression
        {
            get => remapexp;

            set => remapexp = value;
        }

        /// <summary>
        /// Total de itens da Lista
        /// </summary>
        /// <returns></returns>
        public int Total => _total ?? -1;



        /// <summary>
        /// Expressões adicionadas a clausula where junto com os filtros
        /// </summary>
        /// <returns></returns>
        public List<Expression<Func<TClass, bool>>> WhereFilters { get; set; } = new List<Expression<Func<TClass, bool>>>();

        #endregion Public Properties

        #region Public Methods

        public static implicit operator List<TRemap>(PaginationFilter<TClass, TRemap> obj) => obj.ToList();

        public static implicit operator PaginationFilter<TClass, TRemap>(NameValueCollection NVC) => new PaginationFilter<TClass, TRemap>().UseNameValueCollection(NVC);

        public static implicit operator PaginationFilter<TClass, TRemap>(string QueryString) => new PaginationFilter<TClass, TRemap>().UseQueryStringExpression(QueryString);

        public static implicit operator TRemap[](PaginationFilter<TClass, TRemap> obj) => obj.GetPage();

        public PropertyFilter<TClass, TRemap> And<T>(string PropertyName, bool Enabled = true) => SetMember(PropertyName, FilterConditional.And, Enabled);

        public PropertyFilter<TClass, TRemap> And<T>(Expression<Func<TClass, T>> PropertyName, bool Enabled = true) => SetMember(PropertyName, FilterConditional.And, Enabled);

        /// <summary>
        /// Quantidade de botões de paginação
        /// </summary>
        /// <returns></returns>
        public int ButtonCount(string Trailling = "...") => CreatePaginationButtons(Trailling).Count();

        /// <summary>
        /// Força o <see cref="IQueryable"/> a executar (sem paginação)
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> Compute()
        {
            if (Data is IQueryable<TClass>)
            {
                Data = Data.ToList();
            }
            return this;
        }

        /// <summary>
        /// Configura este Filtro
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> Config(Action<PaginationFilter<TClass, TRemap>> options) => this.With(options);

        /// <summary>
        /// Verifica se o <see cref="PageRange"/> contém algumas páginas especificas
        /// </summary>
        /// <param name="PageNumbers"></param>
        /// <returns></returns>
        public bool ContainsPage(IEnumerable<int> PageNumbers) => PageRange.ContainsAny(PageNumbers.Where(x => x.IsBetweenOrEqual(FirstPage, LastPage)).ToArray());

        /// <summary>
        /// Verifica se o <see cref="PageRange"/> contém algumas páginas especificas
        /// </summary>
        /// <param name="PageNumbers"></param>
        /// <returns></returns>
        public bool ContainsPage(params int[] PageNumbers) => ContainsPage((PageNumbers ?? Array.Empty<int>()).AsEnumerable());

        /// <summary>
        /// Cria uma lista de strings utilizadas nos botões de paginação
        /// </summary>
        /// <param name="Trailling"></param>
        /// <returns></returns>
        public IEnumerable<string> CreatePaginationButtons(string Trailling = "...")
        {
            if (Trailling.IsNumber())
            {
                throw new ArgumentException($"Trailling cannot be a number! => {Trailling}");
            }

            var l = new List<string>();
            if (IsPaginationNecessary)
            {
                if (IsFirstPageNecessary)
                {
                    l.Add(FirstPage.ToString());
                }

                if (Trailling.IsValid() && IsFirstTraillingNecessary)
                {
                    l.Add(Trailling);
                }

                l.AddRange(PageRange.Select(x => x.ToString()));

                if (Trailling.IsValid() && IsLastTraillingNecessary)
                {
                    l.Add(Trailling);
                }

                if (IsLastPageNecessary)
                {
                    l.Add(LastPage.ToString());
                }
            }

            return l;
        }

        /// <summary>
        /// Cria uma querystring com paginacao e os filtros ativos
        /// </summary>
        /// <returns></returns>
        public string CreateQueryString(int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false)
        {
            var l = new List<string>
            {
                GetFilterQueryString(ForceEnabled),
                GetPaginationQueryString(PageNumber ?? this.PageNumber, IncludePageSize, IncludePaginationOffset)
            };
            return Util.JoinString(l.Where(x => x.IsValid()), "&");
        }

        /// <summary>
        /// Seta uma busca usando <see cref="Contains()"/> em <paramref name="PropertyValues"/> para
        /// cada propriedade em <paramref name="PropertyNames"/>
        /// </summary>
        /// <param name="PropertyValues"></param>
        /// <param name="PropertyNames"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> CreateSearch<T>(IEnumerable<IComparable> PropertyValues, params Expression<Func<TClass, T>>[] PropertyNames)
        {
            PropertyNames = (PropertyNames ?? Array.Empty<Expression<Func<TClass, T>>>()).Where(x => x != null).ToArray();
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            foreach (var sel in PropertyNames)
            {
                SetMember(sel, FilterConditional.Or).Contains(PropertyValues);
            }

            return this;
        }



        /// <summary> Seta uma busca usando <see cref="Contains(<paramref name="PropertyValues"/>)"
        /// /> para cada propriedade em <paramref name="PropertyNames"/> </summary> <param
        /// name="PropertyValues"></param> <param name="PropertyNames"></param> <returns></returns>
        public PaginationFilter<TClass, TRemap> CreateSearch(IEnumerable<IComparable> PropertyValues, params string[] PropertyNames)
        {
            PropertyNames = (PropertyNames ?? Array.Empty<string>()).Where(x => x.IsValid()).ToArray();
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            foreach (var sel in PropertyNames)
            {
                SetMember(sel, FilterConditional.Or).Contains(PropertyValues);
            }

            return this;
        }

        /// <summary>
        /// Cria uma Url com a query string deste filtro
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="PageNumber"></param>
        /// <param name="ForceEnabled"></param>
        /// <param name="IncludePageSize"></param>
        /// <param name="IncludePaginationOffset"></param>
        /// <returns></returns>
        public string CreateUrl(string Url, int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false)
        {
            string qs = CreateQueryString(PageNumber ?? this.PageNumber, ForceEnabled, IncludePageSize, IncludePaginationOffset);
            if (Url.IsURL())
            {
                var u = new Uri(Url);
                Url = u.GetLeftPart(UriPartial.Path);
                qs = Util.JoinString(new[] { u.Query, qs }, "&");
            }

            return Url + "?" + qs;
        }

        /// <summary>
        /// Cria uma url a partir de um pattern de Url e concatena a query string
        /// </summary>
        /// <param name="UrlPattern"></param>
        /// <param name="ForceEnabled"></param>
        /// <returns></returns>
        public string CreateUrlFromPattern(string UrlPattern, int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false)
        {
            var parametros = UrlPattern.GetAllBetween("{", "}").Select(x => x.GetBefore(":"));
            var dic = ToDictionary(PageNumber, ForceEnabled, IncludePageSize, IncludePaginationOffset);
            UrlPattern = UrlPattern.ReplaceUrlParameters(dic);
            string querystring = Util.EmptyString;
            foreach (var q in dic)
            {
                var v = Util.ForceArray<string>(q.Value).ToList();
                if (v.Any())
                {
                    if (parametros.Contains(q.Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        UrlPattern = UrlPattern.Replace($"{{{q.Key}}}", v.FirstOrDefault().IfBlank(Util.EmptyString));
                        v.RemoveAt(0);
                    }

                    if (v.Any())
                    {
                        querystring = new[] { querystring, v.SelectJoinString(x => q.Key + "=" + x.IfBlank(Util.EmptyString).ToString().UrlDecode(), "&") }.Where(x => x.IsValid()).JoinString("&");
                    }
                }
            }

            if (querystring.IsValid())
            {
                UrlPattern = UrlPattern + "?" + querystring;
            }

            UrlPattern = UrlPattern.RemoveUrlParameters();
            return UrlPattern;
        }

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <returns></returns>
        public IQueryable<TClass> GetEnumerablePage() => GetQueryablePage(PageNumber);

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public IEnumerable<TClass> GetEnumerablePage(int PageNumber) => GetQueryablePage(PageNumber).AsEnumerable();

        /// <summary>
        /// Cria uma querystring com os filtros ativos
        /// </summary>
        /// <returns></returns>
        public string GetFilterQueryString(bool ForceEnabled = false) => Util.JoinString(Filters.Select(x => x.CreateQueryParameter(ForceEnabled)).Where(x => x.IsValid()), "&");

        /// <summary>
        /// Executa o Filtro e retorna os dados paginados
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public TRemap[] GetPage(int PageNumber)
        {
            if (Data != null)
            {
                var filtereddata = GetQueryablePage(PageNumber);

                if (RemapExpression != null)
                {
                    return filtereddata.Select(RemapExpression).ToArray();
                }
                else
                {
                    return filtereddata.Cast<TRemap>().ToArray();
                }
            }
            else
            {
                return Array.Empty<TRemap>();
            }
        }

        /// <summary>
        /// Retorna a pagina atual
        /// </summary>
        /// <returns></returns>
        public TRemap[] GetPage() => GetPage(PageNumber);

        /// <summary>
        /// Retorna a parte da querystring usada para paginacao
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public string GetPaginationQueryString(int? PageNumber = default, bool IncludePageSize = false, bool IncludePaginationOffset = false)
        {
            PageNumber = (PageNumber ?? this.PageNumber).LimitRange(FirstPage, LastPage);
            if (PageNumber > 0)
            {
                var l = new List<string>();
                if (PageNumber > 1)
                {
                    l.Add($"{PageNumberQueryParameter}={PageNumber}");
                }

                if (IncludePageSize)
                {
                    l.Add($"{PageSizeQueryParameter}={PageSize}");
                }

                if (IncludePaginationOffset)
                {
                    l.Add($"{PaginationOffsetQueryParameter}={PaginationOffset}");
                }

                return Util.JoinString(l, "&");
            }

            return Util.EmptyString;
        }

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <returns></returns>
        public IQueryable<TClass> GetQueryablePage() => GetQueryablePage(PageNumber);

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public IQueryable<TClass> GetQueryablePage(int PageNumber)
        {
            this.PageNumber = PageNumber;
            if (Data != null)
            {
                var filtereddata = ApplyFilter(Data);
                filtereddata = ApplyPage(filtereddata);
                return filtereddata.AsQueryable();
            }

            return Array.Empty<TClass>().AsQueryable();
        }

        /// <summary>
        /// Verifica se á pagina atual é igual a uma pagina especifica
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public bool IsCurrentPage(int Index) => Index == PageNumber;

        /// <summary>
        /// Pula um determinado numero de páginas a partir da pagina atual
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public int JumpPages(int Quantity)
        {
            while (true)
            {
                if (Quantity < 0)
                {
                    PageNumber--;
                    Quantity++;
                }
                else if (Quantity > 0)
                {
                    PageNumber++;
                    Quantity--;
                }
                else
                {
                    return PageNumber;
                }
            }
        }

        public bool Any() => this.GetPage().Any();

        public PropertyFilter<TClass, TRemap> Or<T>(string PropertyName, bool Enabled = true) => SetMember(PropertyName, FilterConditional.Or, Enabled);

        public PropertyFilter<TClass, TRemap> Or<T>(Expression<Func<TClass, T>> PropertyName, bool Enabled = true) => SetMember(PropertyName, FilterConditional.Or, Enabled);

        public PaginationFilter<TClass, TRemap> OrderBy<T>(params Expression<Func<TClass, T>>[] Selectors)
        {
            foreach (var Selector in Selectors ?? Array.Empty<Expression<Func<TClass, T>>>())
            {
                if (Selector != null)
                {
                    OrderBy(Selector);
                }
            }

            return this;
        }

        /// <summary>
        /// Ordena os resultados da lista
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="Selector"></param>
        /// <param name="Descending"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> OrderBy<T>(Expression<Func<TClass, T>> Selector, bool Descending = false)
        {
            bool Ascending = !Descending;
            if (Selector != null)
            {
                if (Data is IOrderedQueryable<TClass> ordered_queryable)
                {
                    if (Ascending)
                    {
                        Data = ordered_queryable.ThenBy(Selector);
                    }
                    else
                    {
                        Data = ordered_queryable.ThenByDescending(Selector);
                    }

                    return this;
                }

                if (Data is IQueryable<TClass> queryable)
                {
                    if (Ascending)
                    {
                        Data = queryable.OrderBy(Selector);
                    }
                    else
                    {
                        Data = queryable.OrderByDescending(Selector);
                    }

                    return this;
                }

                if (Data is IOrderedEnumerable<TClass> ordered_enumerable)
                {
                    if (Ascending)
                    {
                        Data = ordered_enumerable.ThenBy(Selector.Compile());
                    }
                    else
                    {
                        Data = ordered_enumerable.ThenByDescending(Selector.Compile());
                    }

                    return this;
                }

                if (Data is IEnumerable<TClass> ee)
                {
                    if (Ascending)
                    {
                        Data = ee.OrderBy(Selector.Compile());
                    }
                    else
                    {
                        Data = ee.OrderByDescending(Selector.Compile());
                    }

                    return this;
                }
            }

            return this;
        }

        /// <summary>
        /// Ordena os resultados da lista
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> OrderBy(string[] Selector, bool Descending = false)
        {
            bool Ascending = !Descending;
            if ((Selector ?? Array.Empty<string>()).Any())
            {
                if (Data is IQueryable<TClass> q)
                {
                    Data = q.ThenByProperty(Selector, Ascending);
                    return this;
                }

                if (Data is IEnumerable<TClass>)
                {
                    Data = Data.ThenByProperty(Selector, Ascending);
                    return this;
                }
            }

            return this;
        }

        /// <summary>
        /// Ordena os resultados da lista
        /// </summary>
        /// <param name="Selector"></param>
        /// <param name="Descending"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> OrderBy(string Selector, bool Descending = false) => OrderBy(Selector.IfBlank(Util.EmptyString).SplitAny(" ", "/", ","), Descending);

        public PaginationFilter<TClass, TRemap> OrderByDescending<T>(Expression<Func<TClass, T>> Selector)
        {
            if (Selector != null)
            {
                OrderBy(Selector, true);
            }

            return this;
        }

        /// <summary>
        /// Aplica a paginação a um template
        /// </summary>
        /// <param name="Template">Template de pagina</param>
        /// <param name="TraillingTemplate">emplate de botoes de reticencias</param>
        /// <param name="Trailling">botao de reticencias</param>
        /// <returns></returns>
        public string PageButtonsFromTemplate(string Template, string TraillingTemplate, string SeparatorTemplate = Util.EmptyString, string Trailling = "...") => Template.IsValid() ? TraillingTemplate.IsNotValid() || Trailling.IsNotValid() ? PageButtonsFromTemplate(Template, SeparatorTemplate) : Util.JoinString(CreatePaginationButtons(Trailling).Select(x =>
        {
            if (x.IsNumber())
            {
                return Template.Inject(new { Page = x });
            }
            else if ((x ?? Util.EmptyString) == (Trailling ?? Util.EmptyString))
            {
                return TraillingTemplate.Inject(new { Page = x, Trailling });
            }
            else
            {
                return Util.EmptyString;
            }
        }), SeparatorTemplate.IfBlank(Util.EmptyString)) : Util.EmptyString;

        /// <summary>
        /// Aplica a paginação a um template
        /// </summary>
        /// <param name="Template">Template de pagina</param>
        /// <returns></returns>
        public string PageButtonsFromTemplate(string Template, string SeparatorTemplate = Util.EmptyString) => Template.IsValid() ? Util.JoinString(CreatePaginationButtons(Util.EmptyString).Select(x => Template.Inject(new { Page = x })), SeparatorTemplate.IfBlank(Util.EmptyString)) : Util.EmptyString;

        /// <summary>
        /// Seta a lista com os dados a serem filtrados nesse filtro
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> SetData(IEnumerable<TClass> List)
        {
            Data = List.AsEnumerable();
            return this;
        }

        /// <summary>
        /// Seta a lista com os dados a serem filtrados nesse filtro
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> SetData(IQueryable<TClass> List)
        {
            Data = List.AsQueryable();
            return this;
        }

        /// <summary>
        /// Configura um novo integrante para este filtro
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public PropertyFilter<TClass, TRemap> SetMember<T>(Expression<Func<TClass, T>> PropertyName, FilterConditional Conditional = FilterConditional.Or, bool Enabled = true)
        {
            var f = new PropertyFilter<TClass, TRemap>(this);
            f.SetMember(PropertyName, Conditional).SetEnabled(Enabled);
            _filters.Add(f);
            return f;
        }

        /// <summary>
        /// Configura um novo integrante para este filtro
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public PropertyFilter<TClass, TRemap> SetMember(string PropertyName, FilterConditional Conditional = FilterConditional.Or, bool Enabled = true)
        {
            var f = new PropertyFilter<TClass, TRemap>(this);
            f.SetMember(PropertyName, Conditional).SetEnabled(Enabled);
            _filters.Add(f);
            return f;
        }

        /// <summary>
        /// Seta a pagina atual
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> SetPage(int PageNumber)
        {
            this.PageNumber = PageNumber;
            return this;
        }

        /// <summary>
        /// Configura a paginação do filtro
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PaginationOffset"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> SetPagination(int PageSize, int PaginationOffset)
        {
            this.PageSize = PageSize;
            this.PaginationOffset = PaginationOffset;
            return this;
        }

        /// <summary>
        /// Configura a paginação do filtro
        /// </summary>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> SetPagination(int PageSize)
        {
            this.PageSize = PageSize;
            return this;
        }

        /// <summary>
        /// Seta os parametros utilizados na querystring para a paginação
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <param name="PaginationOffset"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> SetPaginationQueryParameters(string PageNumber, string PageSize, string PaginationOffset)
        {
            PageNumberQueryParameter = PageNumber.IfBlank(nameof(this.PageNumber));
            PageSizeQueryParameter = PageSize.IfBlank(nameof(this.PageSize));
            PaginationOffsetQueryParameter = PaginationOffset.IfBlank(nameof(this.PaginationOffset));
            return this;
        }

        public Dictionary<string, object> ToDictionary(int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false) => ToNameValueCollection(PageNumber, ForceEnabled, IncludePageSize, IncludePaginationOffset).ToDictionary();

        public NameValueCollection ToNameValueCollection(int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false) => CreateQueryString(PageNumber, ForceEnabled, IncludePageSize, IncludePaginationOffset).ParseQueryString();

        /// <summary>
        /// Retorna uma QueryString que representa este filtro
        /// </summary>
        /// <returns></returns>
        public override string ToString() => CreateQueryString().ToString();

        /// <summary>
        /// Configura este LambDafilter para utilizar um Dictionary como Filtro.
        /// </summary>
        /// <param name="Collection">Collection</param>
        /// <remarks>
        /// Utiliza os names como propriedade e os values como valores do filtro. Propriedade que
        /// não existirem na classe serão ignoradas. Valores nulos serão ignorados por padrão
        /// </remarks>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> UseArrayDictionary(IDictionary<string, IComparable[]> Collection, string DefaultOperator = "=")
        {
            Collection = Collection ?? new Dictionary<string, IComparable[]>();
            foreach (var K in Collection.Keys)
            {
                var t = typeof(TClass);
                if (t.HasProperty(K) || K == "this")
                {
                    SetMember(K).SetValues(Collection[K].ToArray()).SetOperator(DefaultOperator);
                }
            }

            return this;
        }

        /// <summary>
        /// Configura este LambDafilter para utilizar um Dictionary como Filtro.
        /// </summary>
        /// <param name="Collection">Collection</param>
        /// <remarks>
        /// Utiliza os names como propriedade e os values como valores do filtro. Propriedade que
        /// não existirem na classe serão ignoradas. Valores nulos serão ignorados por padrão
        /// </remarks>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> UseDictionary(IDictionary<string, IComparable> Collection, string DefaultOperator = "=")
        {
            Collection = Collection ?? new Dictionary<string, IComparable>();
            foreach (var K in Collection.Keys)
            {
                var t = typeof(TClass);
                if (t.HasProperty(K) || K == "this")
                {
                    var item = Collection[K];
                    SetMember(K).SetValue(item).SetOperator(DefaultOperator);
                }
            }

            return this;
        }

        /// <summary>
        /// Extrai os parametros de um <see cref="NameValueCollection"/> e seta os integrantes usando as
        /// Keys como integrantes
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="DefaultOperator"></param>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> UseNameValueCollection(NameValueCollection Collection, string DefaultOperator = "=")
        {
            Collection = Collection ?? new NameValueCollection();
            foreach (var K in Collection.AllKeys)
            {
                var t = typeof(TClass);
                var l = t.GetProperties();
                if (l.Any(x => (x.Name ?? Util.EmptyString) == (K ?? Util.EmptyString)))
                {
                    if (Collection[K].IsValid() && Collection.GetValues(K).Any())
                    {
                        SetMember(K).SetValues(Collection.GetValues(K)).SetOperator(DefaultOperator);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Configura este Filtro para utilizar uma querystring.
        /// </summary>
        /// <param name="Query">QueryString</param>
        /// <remarks>
        /// Utiliza os names como propriedade e os values como valores do filtro. Propriedade que
        /// não existirem na classe serão ignoradas. Valores nulos serão ignorados por padrão
        /// </remarks>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> UseQueryString(string Query, string DefaultOperator = "=")
        {
            if (Query.IsValid())
            {
                UseNameValueCollection(Query.ParseQueryString(), DefaultOperator);
            }

            return this;
        }

        /// <summary> Configura este Filtro para utilizar uma querystring com operadores
        /// (&integrante=operador:valor) </summary> <param name="QueryExpression"></param> <param
        /// name="Separator"></param> <param name="Conditional"></param> <returns></returns>
        public PaginationFilter<TClass, TRemap> UseQueryStringExpression(string QueryExpression, string Separator = ":", FilterConditional Conditional = FilterConditional.And)
        {
            var Collection = QueryExpression.ParseQueryString();
            foreach (var K in Collection.AllKeys)
            {
                string prop = K.UrlDecode();
                var t = typeof(TClass);
                if (t.HasProperty(prop) || K == "this")
                {
                    if (Collection[K].IsValid() && Collection.GetValues(K).Any())
                    {
                        var buscas = Collection.GetValues(K).GroupBy(x => x.GetBefore(Separator, true).IfBlank("=")).ToDictionary();
                        foreach (var item in buscas)
                        {
                            var vals = item.Value.Select(x => x.GetAfter(Separator));
                            SetMember(prop, Conditional).SetValues(vals).SetOperator(item.Key).QueryStringSeparator = Separator;
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Adciona Expressões a clausula where junto com os filtros
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> Where(Expression<Func<TClass, bool>> predicate)
        {
            WhereFilters = WhereFilters ?? new List<Expression<Func<TClass, bool>>>();
            WhereFilters.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adciona Expressões a clausula where junto com os filtros se uma condiçao for cumprida
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClass, TRemap> WhereIf(bool Test, Expression<Func<TClass, bool>> predicate)
        {
            if (Test)
            {
                Where(predicate);
            }

            return this;
        }

        public List<TRemap> ToList() => GetPage().ToList();
        public List<TRemap> ToList(int PageNumber) => GetPage(PageNumber).ToList();

        #endregion Public Methods
    }

    public class PaginationFilter<TClass> : PaginationFilter<TClass, TClass> where TClass : class
    {
        #region Public Constructors

        public PaginationFilter() : base()
        {
        }

        /// <summary>
        /// Cria uma nova instancia e seta a exclusividade de filtro
        /// </summary>
        public PaginationFilter(Action<PaginationFilter<TClass>> Options) : base() => Config(Options);

        #endregion Public Constructors

        #region Public Methods

        public PaginationFilter<TClass> Config(Action<PaginationFilter<TClass>> options) => this.With(options);

        #endregion Public Methods
    }

    public class PropertyFilter<TClassFrom, TClassTo> where TClassFrom : class
    {
        #region Public Constructors

        public PropertyFilter(PaginationFilter<TClassFrom, TClassTo> PaginationFilter) => this.PaginationFilter = PaginationFilter;

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Configura este filtro para utilização de valores nulos na query
        /// </summary>
        /// <returns></returns>
        public bool AcceptNullValues { get; set; } = false;

        public bool CompareWith => !Operator.StartsWithAny("!");

        public FilterConditional Conditional { get; set; } = FilterConditional.Or;

        /// <summary>
        /// Indica se este filtro está ativo
        /// </summary>
        /// <returns></returns>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Expressão binaria deste filtro, se ativo
        /// </summary>
        /// <returns></returns>
        public BinaryExpression Filter => Enabled ? Util.GetOperatorExpression(Member, Operator.IfBlank(Util.EmptyString), ValidValues(), ValuesConditional) : null;

        /// <summary>
        /// Comparara o valor do filtro com TRUE ou FALSE
        /// </summary>
        /// <returns></returns>
        public bool Is { get; set; } = true;

        /// <summary>
        /// Expressão do integrante utilizado no filtro
        /// </summary>
        /// <returns></returns>
        public Expression Member { get; set; }

        /// <summary>
        /// Operador usado nesse filtro
        /// </summary>
        /// <returns></returns>
        public string Operator { get; set; } = "=";

        public PaginationFilter<TClassFrom, TClassTo> PaginationFilter { get; private set; }

        /// <summary>
        /// Parametro da expressão lambda
        /// </summary>
        /// <returns></returns>
        public ParameterExpression Parameter => PaginationFilter.Parameter;

        public string PropertyName => Member.ToString().GetAfter(".");

        /// <summary>
        /// Valores a serem testados por esse filtro
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComparable> PropertyValues { get; set; }

        /// <summary>
        /// Separador utilizado pelo <see cref="CreateQueryParameter(bool)"/>
        /// </summary>
        /// <returns></returns>
        public string QueryStringSeparator { get; set; } = ":";

        public FilterConditional ValuesConditional { get; set; } = FilterConditional.Or;
        public Expression<Func<IComparable, bool>> ValueValidation { get; set; } = null;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adciona varios valores para esse filtro testar.
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> AddValues<T>(params T?[] Values) where T : struct
        {
            PropertyValues = (PropertyValues ?? Array.Empty<IComparable>());
            PropertyValues = PropertyValues.Union((IEnumerable<IComparable>)(Values?.AsEnumerable() ?? Array.Empty<T?>()));
            return this;
        }

        /// <summary>
        /// Permite que valores nulos sejam adcionados ao filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> AllowNull()
        {
            AcceptNullValues = true;
            return this;
        }

        /// <summary>
        /// Força uma comparação negativa para este filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> CompareFalse()
        {
            if (CompareWith)
            {
                Negate();
            }

            return this;
        }

        /// <summary>
        /// Força uma comparação positiva para este filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> CompareTrue()
        {
            if (CompareWith == false)
            {
                Negate();
            }

            return this;
        }

        /// <summary>
        /// Seta o operador para Contains e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> Contains<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("contains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para Contains e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> Contains<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values);
            SetOperator("contains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para Contains e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> ContainsAll<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("contains");
            ValuesConditional = FilterConditional.And;
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para Contains e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> ContainsAll<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values);
            SetOperator("contains");
            ValuesConditional = FilterConditional.And;
            return PaginationFilter;
        }

        /// <summary>
        /// Retorna uma string em formato de parametro de QueryString deste filtro
        /// </summary>
        /// <param name="ForceEnabled"></param>
        /// <returns></returns>
        public string CreateQueryParameter(bool ForceEnabled = false, bool OnlyValid = true)
        {
            if (Enabled || ForceEnabled)
            {
                string xx = Operator.AppendIf(QueryStringSeparator, QueryStringSeparator.IsValid() && Operator.ToLowerInvariant().IsNotAny(Util.EmptyString, "=", "==", "===")).UrlEncode();
                return (OnlyValid ? ValidValues() : PropertyValues).Where(x => x != null && x.ToString().IsValid()).SelectJoinString(x => $"{PropertyName}={xx}{x.ToString().UrlEncode()}");
            }

            return Util.EmptyString;
        }

        /// <summary>
        /// Seta o operador para CrossContains e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> CrossContains<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("crosscontains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para CrossContains e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> CrossContains<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("crosscontains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para EndsWith e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> EndsWith<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("EndsWith");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para EndsWith e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> EndsWith<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("EndsWith");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para = e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> Equal<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para = e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> Equal<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt; e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> GreaterThan<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator(">");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt; e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> GreaterThan<T>(IEnumerable<T> Values) where T : struct
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator(">");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt;= e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> GreaterThanOrEqual<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator(">=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt;= e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> GreaterThanOrEqual<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator(">=");
            return PaginationFilter;
        }

        /// <summary>
        /// Impede que valores nulos sejam adcionados ao filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> IgnoreNull()
        {
            AcceptNullValues = false;
            return this;
        }

        /// <summary>
        /// Seta o operador para &lt; e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> LessThan<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("<");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &lt; e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> LessThan<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("<");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &lt;= e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> LessThanOrEqual<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("<=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &lt; e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> LessThanOrEqual<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("<=");
            return PaginationFilter;
        }

        /// <summary>
        /// Nega o filtro atual
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> Negate()
        {
            if (CompareWith == false)
            {
                Operator = Operator.TrimStartAny(false, "!");
            }
            else
            {
                Operator = "!" + Operator;
            }

            return this;
        }

        /// <summary>
        /// Seta o operador para != e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> NotEqual<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("<>");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para != e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> NotEqual<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("<>");
            return PaginationFilter;
        }

        /// <summary>
        /// Ativa ou desativa esse filtro durante a construção da expressão
        /// </summary>
        /// <param name="Enabled"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetEnabled(bool Enabled = true)
        {
            this.Enabled = Enabled;
            return this;
        }

        /// <summary>
        /// Sete um integrante para ser utilizado neste filtro. É ignorado quando seus Values estão
        /// nulos ou vazios
        /// </summary>
        /// <param name="PropertySelector"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetMember<T>(Expression<Func<TClassFrom, T>> PropertySelector, FilterConditional Conditional = FilterConditional.Or) => SetMember(Util.JoinString(PropertySelector.Body.ToString().Split(".").Skip(1), "."), Conditional);

        /// <summary>
        /// Sete um integrante para ser utilizado neste filtro. É ignorado quando seus Values estão
        /// nulos ou vazios
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetMember(string PropertyName, FilterConditional Conditional = FilterConditional.Or)
        {
            this.Conditional = Conditional;
            Member = Parameter.PropertyExpression(PropertyName);
            return this;
        }

        /// <summary>
        /// Seta o operador utilizado nesse filtro
        /// </summary>
        /// <param name="[Operator]"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetOperator(string Operator)
        {
            this.Operator = Operator.IfBlank("=").ToLowerInvariant();
            return this;
        }

        /// <summary>
        /// Seta um unico valor para esse filtro testar. Substitui os antigos
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetValue<T>(T Value) where T : IComparable
        {
            PropertyValues = new[] { (IComparable)Value };
            return this;
        }

        /// <summary>
        /// Seta um unico valor para esse filtro testar. Substitui os antigos
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetValue<T>(T? Value) where T : struct
        {
            PropertyValues = Value.HasValue
                ? (IEnumerable<IComparable>)new T[] { Value.Value }.AsEnumerable()
                : (IEnumerable<IComparable>)new T[] { }.AsEnumerable();
            return this;
        }

        /// <summary>
        /// Seta varios valores para esse filtro testar. Substitui os valores antigos
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetValues<T>(params T[] Values) where T : IComparable
        {
            PropertyValues = (IEnumerable<IComparable>)(Values?.AsEnumerable() ?? Array.Empty<T>().AsEnumerable());
            return this;
        }

        /// <summary>
        /// Seta varios valores para esse filtro testar. Substitui os valores antigos
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public PropertyFilter<TClassFrom, TClassTo> SetValues<T>(IEnumerable<T> Values) where T : IComparable
        {
            PropertyValues = (IEnumerable<IComparable>)(Values ?? Array.Empty<T>());
            return this;
        }

        /// <summary>
        /// Seta o operador para StartsWith e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> StartsWith<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("StartsWith");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para StartsWith e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<TClassFrom, TClassTo> StartsWith<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values);
            SetOperator("StartsWith");
            return PaginationFilter;
        }

        public override string ToString() => CreateQueryParameter();

        /// <summary>
        /// Retorna apenas os valores validos para este filtro ( <see cref="AcceptNullValues"/> e
        /// <see cref="ValueValidation"/>)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComparable> ValidValues()
        {
            var v = (PropertyValues ?? Array.Empty<IComparable>()).AsEnumerable();
            if (!AcceptNullValues)
            {
                v = v.Where(x => x != null);
            }

            if (ValueValidation != null)
            {
                v = v.Where(ValueValidation.Compile());
            }

            return v;
        }

        #endregion Public Methods
    }

    public enum FilterConditional
    {
        Or,
        And
    }

}