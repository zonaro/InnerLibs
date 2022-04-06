using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;

namespace InnerLibs.LINQ
{
    public enum FilterConditional
    {
        Or,
        And
    }

    /// <summary>
    /// Classe para criação de paginação e filtros dinâmicos para listas de classes
    /// </summary>
    /// <typeparam name="ClassType"></typeparam>
    public class PaginationFilter<ClassType, RemapType> where ClassType : class
    {
        private int? _total = default;

        private int pgnumber = 1;

        private string pnp, psp, pop;

        private Func<ClassType, RemapType> remapexp;

        private IEnumerable<ClassType> ApplyFilter()
        {
            var FilteredData = Data;
            _total = default;
            if (FilteredData != null)
            {
                if (LambdaExpression != null)
                {
                    if (FilteredData is IOrderedQueryable<ClassType>)
                    {
                        FilteredData = ((IOrderedQueryable<ClassType>)FilteredData).Where(LambdaExpression);
                        var dq = ((IOrderedQueryable<ClassType>)FilteredData).Select(x => 0);
                        _total = dq.Count();
                    }

                    if (FilteredData is IQueryable<ClassType>)
                    {
                        FilteredData = ((IQueryable<ClassType>)FilteredData).Where(LambdaExpression);
                        var dq = ((IQueryable<ClassType>)FilteredData).Select(x => 0);
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

        private IEnumerable<ClassType> ApplyPage(IEnumerable<ClassType> FilteredData)
        {
            if (Data != null)
            {
                if (PageNumber > 0 && PageSize > 0)
                {
                    if (FilteredData is IOrderedQueryable<ClassType>)
                    {
                        FilteredData = ((IOrderedQueryable<ClassType>)FilteredData).Skip((PageNumber - 1) * PageSize).Take(PageSize);
                    }
                    else if (Data is IQueryable<ClassType>)
                    {
                        FilteredData = ((IQueryable<ClassType>)FilteredData).Skip((PageNumber - 1) * PageSize).Take(PageSize);
                    }
                }

                return FilteredData;
            }

            return Data;
        }

        internal List<PropertyFilter<ClassType, RemapType>> _filters = new List<PropertyFilter<ClassType, RemapType>>();

        internal ParameterExpression param = LINQExtensions.GenerateParameterExpression<ClassType>();

        public PaginationFilter()
        {
        }

        /// <summary>
        /// Cria uma nova instancia e seta a exclusividade de filtro
        /// </summary>
        public PaginationFilter(Func<ClassType, RemapType> RemapExpression)
        {
            this.RemapExpression = RemapExpression;
        }

        public PaginationFilter(Func<ClassType, RemapType> RemapExpression, Action<PaginationFilter<ClassType, RemapType>> Options)
        {
            this.RemapExpression = RemapExpression;
            Config(Options);
        }

        public PaginationFilter(Action<PaginationFilter<ClassType, RemapType>> Options)
        {
            Config(Options);
        }

        /// <summary>
        /// Fonte de Dados deste filtro
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClassType> Data { get; set; } = null;

        /// <summary>
        /// Expressão binária contendo todos os filtros
        /// </summary>
        /// <returns></returns>
        public BinaryExpression Filter
        {
            get
            {
                Expression exp = null;
                foreach (var valor in Filters.Where(x => x.Enabled))
                {
                    if (valor != null && valor.Filter != null)
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
                }

                return (BinaryExpression)exp;
            }
        }

        /// <summary>
        /// Filtros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyFilter<ClassType, RemapType>> Filters => _filters;

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
        public Expression<Func<ClassType, bool>> LambdaExpression
        {
            get
            {
                Expression<Func<ClassType, bool>> exp = null;
                if (Filter != null)
                {
                    exp = Expression.Lambda<Func<ClassType, bool>>(Filter, param);
                }

                foreach (var valor in WhereFilters ?? new List<Expression<Func<ClassType, bool>>>())
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
                        exp = (Expression<Func<ClassType, bool>>)exp.Reduce();
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
        public int PageCount
        {
            get
            {
                if (PageSize > 0)
                {
                    return (int)Math.Round((Total / (double)PageSize).Ceil());
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Numero da pagina
        /// </summary>
        /// <returns></returns>
        public int PageNumber
        {
            get { return pgnumber; }
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
            get
            {
                return pnp.IfBlank(nameof(PageNumber));
            }

            set
            {
                pnp = value;
            }
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
                    arr.Add(index);
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
            get
            {
                return psp.IfBlank(nameof(PageSize));
            }

            set
            {
                psp = value;
            }
        }

        /// <summary>
        /// Quantidade de botões de paginacao exibidos após a pagina atual e anterior a página atual
        /// </summary>
        public int PaginationOffset { get; set; } = 2;

        public string PaginationOffsetQueryParameter
        {
            get
            {
                return pop.IfBlank(nameof(PaginationOffset));
            }

            set
            {
                pop = value;
            }
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
        public Func<ClassType, RemapType> RemapExpression
        {
            get
            {
                if (typeof(ClassType) == typeof(RemapType))
                {
                    return null;
                }
                else
                {
                    return remapexp;
                }
            }

            set
            {
                if (typeof(ClassType) == typeof(RemapType))
                {
                    remapexp = null;
                }
                else
                {
                    remapexp = value;
                }
            }
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
        public List<Expression<Func<ClassType, bool>>> WhereFilters { get; set; } = new List<Expression<Func<ClassType, bool>>>();

        /// <summary>
        /// Dados da Pagina Atual
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public RemapType[] this[int PageNumber]
        {
            get
            {
                return GetPage(PageNumber);
            }
        }

        public static implicit operator List<RemapType>(PaginationFilter<ClassType, RemapType> obj)
        {
            return obj.GetPage().ToList();
        }

        public static implicit operator PaginationFilter<ClassType, RemapType>(NameValueCollection NVC)
        {
            return new PaginationFilter<ClassType, RemapType>().UseNameValueCollection(NVC);
        }

        public static implicit operator PaginationFilter<ClassType, RemapType>(string QueryString)
        {
            return new PaginationFilter<ClassType, RemapType>().UseQueryStringExpression(QueryString);
        }

        public static implicit operator RemapType[](PaginationFilter<ClassType, RemapType> obj)
        {
            return obj.GetPage();
        }

        public PropertyFilter<ClassType, RemapType> And<T>(string PropertyName, bool Enabled = true)
        {
            return SetMember(PropertyName, FilterConditional.And, Enabled);
        }

        public PropertyFilter<ClassType, RemapType> And<T>(Expression<Func<ClassType, T>> PropertyName, bool Enabled = true)
        {
            return SetMember(PropertyName, FilterConditional.And, Enabled);
        }

        /// <summary>
        /// Quantidade de botões de paginação
        /// </summary>
        /// <returns></returns>
        public int ButtonCount(string Trailling = "...") => CreatePaginationButtons(Trailling).Count();

        /// <summary>
        /// Força o <see cref="IQueryable"/> a executar (sem paginação)
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> Compute()
        {
            Data = Data.ToList().AsEnumerable();
            return this;
        }

        /// <summary>
        /// Configura este Filtro
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> Config(Action<PaginationFilter<ClassType, RemapType>> options)
        {
            options(this);
            return this;
        }

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
            if (Trailling.IsNumber()) throw new ArgumentException($"Trailling cannot be a number! => {Trailling}");

            var l = new List<string>();
            if (IsPaginationNecessary)
            {
                if (IsFirstPageNecessary)
                {
                    l.Add(FirstPage.ToString());
                }

                if (Trailling.IsNotBlank() && IsFirstTraillingNecessary)
                {
                    l.Add(Trailling);
                }

                l.AddRange(PageRange.Select(x => x.ToString()));

                if (Trailling.IsNotBlank() && IsLastTraillingNecessary)
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
            var l = new List<string>();
            l.Add(GetFilterQueryString(ForceEnabled));
            l.Add(GetPaginationQueryString(PageNumber ?? this.PageNumber, IncludePageSize, IncludePaginationOffset));
            return l.Where(x => x.IsNotBlank()).JoinString("&");
        }

        /// <summary>
        /// Seta uma busca usando <see cref="Contains()"/> em <paramref name="PropertyValues"/> para
        /// cada propriedade em <paramref name="PropertyNames"/>
        /// </summary>
        /// <param name="PropertyValues"></param>
        /// <param name="PropertyNames"></param>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> CreateSearch<T>(IEnumerable<IComparable> PropertyValues, params Expression<Func<ClassType, T>>[] PropertyNames)
        {
            PropertyNames = (PropertyNames ?? Array.Empty<Expression<Func<ClassType, T>>>()).Where(x => x != null).ToArray();
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            foreach (var sel in PropertyNames)
                SetMember(sel, FilterConditional.Or).Contains(PropertyValues);
            return this;
        }

        /// <summary> Seta uma busca usando <see cref="Contains(<paramref name="PropertyValues"/>)"
        /// /> para cada propriedade em <paramref name="PropertyNames"/> </summary> <param
        /// name="PropertyValues"></param> <param name="PropertyNames"></param> <returns></returns>
        public PaginationFilter<ClassType, RemapType> CreateSearch(IEnumerable<IComparable> PropertyValues, params string[] PropertyNames)
        {
            PropertyNames = (PropertyNames ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).ToArray();
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            foreach (var sel in PropertyNames)
                SetMember(sel, FilterConditional.Or).Contains(PropertyValues);
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
                qs = new[] { u.Query, qs }.JoinString("&");
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
            string querystring = "";
            foreach (var q in dic)
            {
                var v = Converter.ForceArray<string>(q.Value).ToList();
                if (v.Any())
                {
                    if (parametros.Contains(q.Key, StringComparer.InvariantCultureIgnoreCase))
                    {
                        UrlPattern = UrlPattern.Replace($"{{{q.Key}}}", v.FirstOrDefault().IfBlank(""));
                        v.RemoveAt(0);
                    }

                    if (v.Any())
                    {
                        querystring = new[] { querystring, v.SelectJoinString(x => q.Key + "=" + x.IfBlank("").ToString().UrlDecode(), "&") }.Where(x => x.IsNotBlank()).JoinString("&");
                    }
                }
            }

            if (querystring.IsNotBlank())
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
        public IQueryable<ClassType> GetEnumerablePage()
        {
            return (IQueryable<ClassType>)GetEnumerablePage(PageNumber);
        }

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public IEnumerable<ClassType> GetEnumerablePage(int PageNumber)
        {
            return GetQueryablePage(PageNumber).AsEnumerable();
        }

        /// <summary>
        /// Cria uma querystring com os filtros ativos
        /// </summary>
        /// <returns></returns>
        public string GetFilterQueryString(bool ForceEnabled = false)
        {
            return Filters.Select(x => x.CreateQueryParameter(ForceEnabled)).Where(x => x.IsNotBlank()).JoinString("&");
        }

        /// <summary>
        /// Executa o Filtro e retorna os dados paginados
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public RemapType[] GetPage(int PageNumber)
        {
            if (Data != null)
            {
                var filtereddata = GetQueryablePage(PageNumber);
                if (RemapExpression is null || typeof(ClassType) == typeof(RemapType))
                {
                    return filtereddata.Cast<RemapType>().ToArray();
                }

                return filtereddata.Select(RemapExpression).ToArray();
            }

            return Array.Empty<RemapType>();
        }

        /// <summary>
        /// Retorna a pagina atual
        /// </summary>
        /// <returns></returns>
        public RemapType[] GetPage()
        {
            return GetPage(PageNumber);
        }

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
                    l.Add($"{PageNumberQueryParameter}={PageNumber}");
                if (IncludePageSize)
                    l.Add($"{PageSizeQueryParameter}={PageSize}");
                if (IncludePaginationOffset)
                    l.Add($"{PaginationOffsetQueryParameter}={PaginationOffset}");
                return l.JoinString("&");
            }

            return "";
        }

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <returns></returns>
        public IQueryable<ClassType> GetQueryablePage()
        {
            return GetQueryablePage(PageNumber);
        }

        /// <summary>
        /// Retorna <see cref="Data"/> com os filtros aplicados
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public IQueryable<ClassType> GetQueryablePage(int PageNumber)
        {
            this.PageNumber = PageNumber;
            if (Data != null)
            {
                var filtereddata = ApplyFilter();
                filtereddata = ApplyPage(filtereddata);
                return (IQueryable<ClassType>)filtereddata;
            }

            return (IQueryable<ClassType>)Data;
        }

        /// <summary>
        /// Verifica se á pagina atual é igual a uma pagina especifica
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public bool IsCurrentPage(int Index)
        {
            return Index == PageNumber;
        }

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
                    PageNumber = PageNumber - 1;
                    Quantity = Quantity + 1;
                }
                else

                if (Quantity > 0)
                {
                    PageNumber = PageNumber + 1;
                    Quantity = Quantity - 1;
                }
                else
                {
                    return PageNumber;
                }
            }
        }

        public PropertyFilter<ClassType, RemapType> Or<T>(string PropertyName, bool Enabled = true)
        {
            return SetMember(PropertyName, FilterConditional.Or, Enabled);
        }

        public PropertyFilter<ClassType, RemapType> Or<T>(Expression<Func<ClassType, T>> PropertyName, bool Enabled = true)
        {
            return SetMember(PropertyName, FilterConditional.Or, Enabled);
        }

        public PaginationFilter<ClassType, RemapType> OrderBy<T>(params Expression<Func<ClassType, T>>[] Selectors)
        {
            foreach (var Selector in Selectors ?? Array.Empty<Expression<Func<ClassType, T>>>())
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
        public PaginationFilter<ClassType, RemapType> OrderBy<T>(Expression<Func<ClassType, T>> Selector, bool Descending = false)
        {
            bool Ascending = !Descending;
            if (Selector != null)
            {
                if (Data is IOrderedQueryable<ClassType>)
                {
                    if (Ascending)
                    {
                        Data = ((IOrderedQueryable<ClassType>)Data).ThenBy(Selector);
                    }
                    else
                    {
                        Data = ((IOrderedQueryable<ClassType>)Data).ThenByDescending(Selector);
                    }

                    return this;
                }

                if (Data is IQueryable<ClassType>)
                {
                    if (Ascending)
                    {
                        Data = ((IQueryable<ClassType>)Data).OrderBy(Selector);
                    }
                    else
                    {
                        Data = ((IQueryable<ClassType>)Data).OrderByDescending(Selector);
                    }

                    return this;
                }

                if (Data is IOrderedEnumerable<ClassType>)
                {
                    if (Ascending)
                    {
                        Data = ((IOrderedEnumerable<ClassType>)Data).ThenBy(Selector.Compile());
                    }
                    else
                    {
                        Data = ((IOrderedEnumerable<ClassType>)Data).ThenByDescending(Selector.Compile());
                    }

                    return this;
                }

                if (Data is IEnumerable<ClassType>)
                {
                    if (Ascending)
                    {
                        Data = Data.OrderBy(Selector.Compile());
                    }
                    else
                    {
                        Data = Data.OrderByDescending(Selector.Compile());
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
        public PaginationFilter<ClassType, RemapType> OrderBy(string[] Selector, bool Descending = false)
        {
            bool Ascending = !Descending;
            if ((Selector ?? Array.Empty<string>()).Any())
            {
                if (Data is IQueryable<ClassType>)
                {
                    Data = ((IQueryable<ClassType>)Data).ThenByProperty(Selector, Ascending);
                    return this;
                }

                if (Data is IEnumerable<ClassType>)
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
        public PaginationFilter<ClassType, RemapType> OrderBy(string Selector, bool Descending = false)
        {
            return OrderBy(Selector.IfBlank("").SplitAny(" ", "/", ","), Descending);
        }

        public PaginationFilter<ClassType, RemapType> OrderByDescending<T>(Expression<Func<ClassType, T>> Selector)
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
        public string PageButtonsFromTemplate(string Template, string TraillingTemplate, string SeparatorTemplate = "", string Trailling = "...")
        {
            if (Template.IsNotBlank())
            {
                if (TraillingTemplate.IsBlank() || Trailling.IsBlank())
                {
                    return PageButtonsFromTemplate(Template, SeparatorTemplate);
                }
                else
                {
                    return CreatePaginationButtons(Trailling).Select(x =>
                    {
                        if (x.IsNumber())
                        {
                            return Template.Inject(new { Page = x });
                        }

                        if ((x ?? "") == (Trailling ?? ""))
                        {
                            return TraillingTemplate.Inject(new { Page = x, Trailling });
                        }

                        return "";
                    }).JoinString(SeparatorTemplate.IfBlank(""));
                }
            }

            return "";
        }

        /// <summary>
        /// Aplica a paginação a um template
        /// </summary>
        /// <param name="Template">Template de pagina</param>
        /// <returns></returns>
        public string PageButtonsFromTemplate(string Template, string SeparatorTemplate = "")
        {
            if (Template.IsNotBlank())
                return CreatePaginationButtons("").Select(x => Template.Inject(new { Page = x })).JoinString(SeparatorTemplate.IfBlank(""));
            return "";
        }

        /// <summary>
        /// Seta a lista com os dados a serem filtrados nesse filtro
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> SetData(IEnumerable<ClassType> List)
        {
            Data = List;
            return this;
        }

        /// <summary>
        /// Seta a lista com os dados a serem filtrados nesse filtro
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> SetData(IQueryable<ClassType> List)
        {
            Data = List.AsQueryable();
            return this;
        }

        /// <summary>
        /// Configura um novo membro para este filtro
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetMember<T>(Expression<Func<ClassType, T>> PropertyName, FilterConditional Conditional = FilterConditional.Or, bool Enabled = true)
        {
            var f = new PropertyFilter<ClassType, RemapType>(this);
            f.SetMember(PropertyName, Conditional).SetEnabled(Enabled);
            _filters.Add(f);
            return f;
        }

        /// <summary>
        /// Configura um novo membro para este filtro
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetMember(string PropertyName, FilterConditional Conditional = FilterConditional.Or, bool Enabled = true)
        {
            var f = new PropertyFilter<ClassType, RemapType>(this);
            f.SetMember(PropertyName, Conditional).SetEnabled(Enabled);
            _filters.Add(f);
            return f;
        }

        /// <summary>
        /// Seta a pagina atual
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> SetPage(int PageNumber)
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
        public PaginationFilter<ClassType, RemapType> SetPagination(int PageSize, int PaginationOffset)
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
        public PaginationFilter<ClassType, RemapType> SetPagination(int PageSize)
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
        public PaginationFilter<ClassType, RemapType> SetPaginationQueryParameters(string PageNumber, string PageSize, string PaginationOffset)
        {
            PageNumberQueryParameter = PageNumber.IfBlank(nameof(this.PageNumber));
            PageSizeQueryParameter = PageSize.IfBlank(nameof(this.PageSize));
            PaginationOffsetQueryParameter = PaginationOffset.IfBlank(nameof(this.PaginationOffset));
            return this;
        }

        public Dictionary<string, object> ToDictionary(int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false)
        {
            return ToNameValueCollection(PageNumber, ForceEnabled, IncludePageSize, IncludePaginationOffset).ToDictionary();
        }

        public NameValueCollection ToNameValueCollection(int? PageNumber = default, bool ForceEnabled = false, bool IncludePageSize = false, bool IncludePaginationOffset = false)
        {
            return CreateQueryString(PageNumber, ForceEnabled, IncludePageSize, IncludePaginationOffset).ParseQueryString();
        }

        /// <summary>
        /// Retorna uma QueryString que representa este filtro
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CreateQueryString().ToString();
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
        public PaginationFilter<ClassType, RemapType> UseArrayDictionary(IDictionary<string, IComparable[]> Collection, string DefaultOperator = "=")
        {
            Collection = Collection ?? new Dictionary<string, IComparable[]>();
            foreach (var K in Collection.Keys)
            {
                var t = typeof(ClassType);
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
        public PaginationFilter<ClassType, RemapType> UseDictionary(IDictionary<string, IComparable> Collection, string DefaultOperator = "=")
        {
            Collection = Collection ?? new Dictionary<string, IComparable>();
            foreach (var K in Collection.Keys)
            {
                var t = typeof(ClassType);
                if (t.HasProperty(K) || K == "this")
                {
                    var item = Collection[K];
                    SetMember(K).SetValue(item).SetOperator(DefaultOperator);
                }
            }

            return this;
        }

        /// <summary>
        /// Extrai os parametros de um <see cref="NameValueCollection"/> e seta os membros usando as
        /// Keys como membros
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="DefaultOperator"></param>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> UseNameValueCollection(NameValueCollection Collection, string DefaultOperator = "=")
        {
            Collection = Collection ?? new NameValueCollection();
            foreach (var K in Collection.AllKeys)
            {
                var t = typeof(ClassType);
                var l = t.GetProperties();
                if (l.Any(x => (x.Name ?? "") == (K ?? "")))
                {
                    if (Collection[K].IsNotBlank() && Collection.GetValues(K).Any())
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
        public PaginationFilter<ClassType, RemapType> UseQueryString(string Query, string DefaultOperator = "=")
        {
            if (Query.IsNotBlank())
            {
                UseNameValueCollection(Query.ParseQueryString(), DefaultOperator);
            }

            return this;
        }

        /// <summary> Configura este Filtro para utilizar uma querystring com operadores
        /// (&membro=operador:valor) </summary> <param name="QueryExpression"></param> <returns></returns>
        public PaginationFilter<ClassType, RemapType> UseQueryStringExpression(string QueryExpression, string Separator = ":", FilterConditional Conditional = FilterConditional.And)
        {
            var Collection = QueryExpression.ParseQueryString();
            foreach (var K in Collection.AllKeys)
            {
                string prop = K.UrlDecode();
                var t = typeof(ClassType);
                if (t.HasProperty(prop) || K == "this")
                {
                    if (Collection[K].IsNotBlank() && Collection.GetValues(K).Any())
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
        public PaginationFilter<ClassType, RemapType> Where(Expression<Func<ClassType, bool>> predicate)
        {
            WhereFilters = WhereFilters ?? new List<Expression<Func<ClassType, bool>>>();
            WhereFilters.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adciona Expressões a clausula where junto com os filtros se uma condiçao for cumprida
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> WhereIf(bool Test, Expression<Func<ClassType, bool>> predicate)
        {
            if (Test)
                Where(predicate);
            return this;
        }
    }

    public class PaginationFilter<ClassType> : PaginationFilter<ClassType, ClassType> where ClassType : class
    {
        public PaginationFilter()
        {
        }

        /// <summary>
        /// Cria uma nova instancia e seta a exclusividade de filtro
        /// </summary>
        public PaginationFilter(Action<PaginationFilter<ClassType>> Options)
        {
            Options(this);
        }
    }

    public class PropertyFilter<ClassType, RemapType> where ClassType : class
    {
        internal PropertyFilter(PaginationFilter<ClassType, RemapType> LB)
        {
            PaginationFilter = LB;
        }

        /// <summary>
        /// Configura este filtro para utilização de valores nulos na query
        /// </summary>
        /// <returns></returns>
        public bool AcceptNullValues { get; set; } = false;

        public bool CompareWith
        {
            get
            {
                return !Operator.StartsWithAny("!");
            }
        }

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
        public BinaryExpression Filter
        {
            get
            {
                if (Enabled)
                {
                    var v = ValidValues();
                    return LINQExtensions.GetOperatorExpression(Member, Operator.IfBlank(""), v, ValuesConditional);
                }

                return null;
            }
        }

        /// <summary>
        /// Comparara o valor do filtro com TRUE ou FALSE
        /// </summary>
        /// <returns></returns>
        public bool Is { get; set; } = true;

        /// <summary>
        /// Expressão do membro utilizado no filtro
        /// </summary>
        /// <returns></returns>
        public Expression Member { get; set; }

        /// <summary>
        /// Operador usado nesse filtro
        /// </summary>
        /// <returns></returns>
        public string Operator { get; set; } = "=";

        public PaginationFilter<ClassType, RemapType> PaginationFilter { get; private set; }

        /// <summary>
        /// Parametro da expressão lambda
        /// </summary>
        /// <returns></returns>
        public ParameterExpression Parameter
        {
            get
            {
                return PaginationFilter.Parameter;
            }
        }

        public string PropertyName
        {
            get
            {
                return Member.ToString().GetAfter(".");
            }
        }

        /// <summary>
        /// Valores a serem testados por esse filtro
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComparable> PropertyValues { get; set; }

        /// <summary>
        /// Separador utilizado pelo <see cref="CreateQueryParameter(Boolean)"/>
        /// </summary>
        /// <returns></returns>
        public string QueryStringSeparator { get; set; } = ":";

        public FilterConditional ValuesConditional { get; set; } = FilterConditional.Or;
        public Expression<Func<IComparable, bool>> ValueValidation { get; set; } = null;

        /// <summary>
        /// Adciona varios valores para esse filtro testar.
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> AddValues<T>(params T?[] Values) where T : struct
        {
            PropertyValues = (PropertyValues ?? Array.Empty<IComparable>());
            PropertyValues = PropertyValues.Union((IEnumerable<IComparable>)(Values?.AsEnumerable() ?? Array.Empty<T?>()));
            return this;
        }

        /// <summary>
        /// Permite que valores nulos sejam adcionados ao filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> AllowNull()
        {
            AcceptNullValues = true;
            return this;
        }

        /// <summary>
        /// Força uma comparação negativa para este filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> CompareFalse()
        {
            if (CompareWith == true)
            {
                Negate();
            }

            return this;
        }

        /// <summary>
        /// Força uma comparação positiva para este filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> CompareTrue()
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
        public PaginationFilter<ClassType, RemapType> Contains<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("contains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para Contains e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> Contains<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values);
            SetOperator("contains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para Contains e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> ContainsAll<T>(T? Value) where T : struct
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
        public PaginationFilter<ClassType, RemapType> ContainsAll<T>(IEnumerable<T> Values) where T : IComparable
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
                string xx = Operator.AppendIf(QueryStringSeparator, QueryStringSeparator.IsNotBlank() && Operator.ToLower().IsNotAny("", "=", "==", "===")).UrlEncode();
                return (OnlyValid ? ValidValues() : PropertyValues).Where(x => x != null && x.ToString().IsNotBlank()).SelectJoinString(x => $"{PropertyName}={xx}{x.ToString().UrlEncode()}");
            }

            return "";
        }

        /// <summary>
        /// Seta o operador para CrossContains e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> CrossContains<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("crosscontains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para CrossContains e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> CrossContains<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("crosscontains");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para EndsWith e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> EndsWith<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("EndsWith");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para EndsWith e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> EndsWith<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("EndsWith");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para = e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> Equal<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para = e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> Equal<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt; e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> GreaterThan<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator(">");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt; e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> GreaterThan<T>(IEnumerable<T> Values) where T : struct
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator(">");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt;= e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> GreaterThanOrEqual<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator(">=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &gt;= e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> GreaterThanOrEqual<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator(">=");
            return PaginationFilter;
        }

        /// <summary>
        /// Impede que valores nulos sejam adcionados ao filtro
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> IgnoreNull()
        {
            AcceptNullValues = false;
            return this;
        }

        /// <summary>
        /// Seta o operador para &lt; e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> LessThan<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("<");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &lt; e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> LessThan<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("<");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &lt;= e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> LessThanOrEqual<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("<=");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para &lt; e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> LessThanOrEqual<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values.Cast<IComparable>());
            SetOperator("<=");
            return PaginationFilter;
        }

        /// <summary>
        /// Nega o filtro atual
        /// </summary>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> Negate()
        {
            if (CompareWith == false)
            {
                Operator = Operator.RemoveFirstAny(false, "!");
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
        public PaginationFilter<ClassType, RemapType> NotEqual<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("<>");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para != e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> NotEqual<T>(IEnumerable<T> Values) where T : IComparable
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
        public PropertyFilter<ClassType, RemapType> SetEnabled(bool Enabled = true)
        {
            this.Enabled = Enabled;
            return this;
        }

        /// <summary>
        /// Sete um membro para ser utilizado neste filtro. É ignorado quando seus Values estão
        /// nulos ou vazios
        /// </summary>
        /// <param name="PropertySelector"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetMember<T>(Expression<Func<ClassType, T>> PropertySelector, FilterConditional Conditional = FilterConditional.Or)
        {
            return SetMember(PropertySelector.Body.ToString().Split(".").Skip(1).JoinString("."), Conditional);
        }

        /// <summary>
        /// Sete um membro para ser utilizado neste filtro. É ignorado quando seus Values estão
        /// nulos ou vazios
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetMember(string PropertyName, FilterConditional Conditional = FilterConditional.Or)
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
        public PropertyFilter<ClassType, RemapType> SetOperator(string Operator)
        {
            this.Operator = Operator.IfBlank("=").ToLower();
            return this;
        }

        /// <summary>
        /// Seta um unico valor para esse filtro testar. Substitui os antigos
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetValue<T>(T Value) where T : IComparable
        {
            PropertyValues = new[] { (IComparable)Value };
            return this;
        }

        /// <summary>
        /// Seta um unico valor para esse filtro testar. Substitui os antigos
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetValue<T>(T? Value) where T : struct
        {
            if (Value.HasValue)
                PropertyValues = (IEnumerable<IComparable>)new T[] { Value.Value }.AsEnumerable();
            else
                PropertyValues = (IEnumerable<IComparable>)new T[] { }.AsEnumerable();
            return this;
        }

        /// <summary>
        /// Seta varios valores para esse filtro testar. Substitui os valores antigos
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetValues<T>(params T[] Values) where T : IComparable
        {
            PropertyValues = (IEnumerable<IComparable>)(Values?.AsEnumerable() ?? Array.Empty<T>().AsEnumerable());
            return this;
        }

        /// <summary>
        /// Seta varios valores para esse filtro testar. Substitui os valores antigos
        /// </summary>
        /// <param name="Values"></param>
        /// <returns></returns>
        public PropertyFilter<ClassType, RemapType> SetValues<T>(IEnumerable<T> Values) where T : IComparable
        {
            PropertyValues = (IEnumerable<IComparable>)(Values ?? Array.Empty<T>());
            return this;
        }

        /// <summary>
        /// Seta o operador para StartsWith e o Valor para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> StartsWith<T>(T? Value) where T : struct
        {
            SetValue(Value);
            SetOperator("StartsWith");
            return PaginationFilter;
        }

        /// <summary>
        /// Seta o operador para StartsWith e os Valores para este filtro
        /// </summary>
        /// <returns></returns>
        public PaginationFilter<ClassType, RemapType> StartsWith<T>(IEnumerable<T> Values) where T : IComparable
        {
            SetValues(Values);
            SetOperator("StartsWith");
            return PaginationFilter;
        }

        public override string ToString()
        {
            return CreateQueryParameter();
        }

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
    }
}