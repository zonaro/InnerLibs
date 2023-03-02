using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Extensions;

public class FortnightGroup<T> : FortnightGroup
{
    #region Public Constructors

    /// <summary>
    /// Instancia um novo <see cref="FortnightGroup{DataType}"/> a partir de uma data inicial e
    /// um numero fixo de quinzenas
    /// </summary>
    /// <param name="StartDate"></param>
    /// <param name="FortnightCount"></param>
    public FortnightGroup(DateTime StartDate = default, int FortnightCount = 1) : base(StartDate, FortnightCount)
    {
    }

    #endregion Public Constructors

    #region Public Indexers

    /// <summary>
    /// Retorna da <see cref="DataCollection"/> os valores correspondentes a quinzena especificada
    /// </summary>
    /// <param name="Fort">Quinzena</param>
    /// <returns></returns>
    public IEnumerable<T> this[Fortnight Fort] => this[Fort.Key];

    /// <summary>
    /// Retorna da <see cref="DataCollection"/> os valores correspondentes a quinzena
    /// especificada em <paramref name="Key"/>
    /// </summary>
    /// <param name="Key">Key da quinzena q@MM-YYYY</param>
    /// <returns></returns>
    public new IEnumerable<T> this[string Key]
    {
        get
        {
            var lista = new List<T>();
            foreach (var ii in DataCollection)
            {
                var datas = new List<DateTime>();
                foreach (var sel in DateSelector)
                {
                    datas.Add(sel(ii));
                }

                var periodo1 = new DateRange(datas.Min(), datas.Max());
                var periodo2 = new DateRange(base[Key].StartDate, base[Key].EndDate);
                if (periodo1.MatchAny(periodo2))
                {
                    lista.Add(ii);
                }
            }

            if (lista.Count > 0)
            {
                lista = lista.OrderBy(x => DateSelector.NullCoalesce()).Distinct().ToList();
            }

            return lista;
        }
    }

    #endregion Public Indexers

    #region Public Properties

    public List<T> DataCollection { get; set; } = new List<T>();
    public List<Func<T, DateTime>> DateSelector { get; set; } = new List<Func<T, DateTime>>();

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Cria um <see cref="FortnightGroup(Of T)"/> a partir de uma coleção de objetos
    /// </summary>
    /// <param name="Range">
    /// Periodo especifico que este grupo irá abranger idependentemente das datas em <paramref name="DateSelector"/>
    /// </param>
    /// <param name="Data">Coleção de objetos</param>
    /// <param name="DateSelector">
    /// Expressão Lambda que indica quais campos do objeto contém uma data que deve ser utilizada
    /// </param>
    /// <returns></returns>
    public static FortnightGroup<T> CreateFromDataGroup(IEnumerable<T> Data, DateRange Range, params Func<T, DateTime>[] DateSelector)
    {
        FortnightGroup<T> fort;
        fort = CreateFromDateRange(Range.StartDate, Range.EndDate);
        if (Data != null && Data.Any())
        {
            fort.DataCollection = Data.ToList();
        }

        if (DateSelector != null && DateSelector.Any())
        {
            fort.DateSelector = DateSelector.ToList();
        }

        return fort;
    }

    /// <summary>
    /// Cria um <see cref="FortnightGroup(Of T)"/> a partir de uma coleção de objetos
    /// </summary>
    /// <param name="Data">Coleção de objetos</param>
    /// <param name="DateSelector">
    /// Expressão Lambda que indica quais campos do objeto contém uma data que deve ser utilizada
    /// </param>
    /// <returns></returns>
    public static FortnightGroup<T> CreateFromDataGroup(IEnumerable<T> Data, params Func<T, DateTime>[] DateSelector)
    {
        var datas = new List<DateTime?>();
        if (DateSelector == null || !DateSelector.Any())
        {
            throw new ArgumentNullException(nameof(DateSelector), "DateSelector is null or empty");
        }
        else
        {
            foreach (var dd in DateSelector)
            {
                datas.Add(Data.OrderBy(dd).Select(dd).First());
                datas.Add(Data.OrderBy(dd).Select(dd).Last());
            }

            datas = datas.Distinct().Where(x => x.HasValue).OrderBy(x => x).ToList();
            return CreateFromDataGroup(Data, new DateRange((DateTime)datas.First(), (DateTime)datas.Last()), DateSelector);
        }
    }

    /// <summary>
    /// Cria um <see cref="FortnightGroup(Of T)"/> a partir de uma data inicial e uma
    /// data final
    /// </summary>
    /// <param name="StartDate">Data inicial</param>
    /// <param name="EndDate">Data Final</param>
    /// <returns></returns>
    public new static FortnightGroup<T> CreateFromDateRange(DateTime StartDate, DateTime EndDate)
    {
        Util.FixOrder(ref StartDate, ref EndDate);
        int fortcount = 0;
        FortnightGroup<T> fort;
        do
        {
            fort = new FortnightGroup<T>(StartDate, fortcount++);
        }
        while (fort.EndDate.Date < EndDate.Date);
        return fort;
    }

    /// <summary>
    /// Cria um <see cref="FortnightGroup(Of T)"/> a partir de uma data inicial e uma
    /// data final
    /// </summary>
    /// <param name="StartDate">Data inicial</param>
    /// <param name="EndDate">Data Final</param>
    /// <returns></returns>
    public new static FortnightGroup<T> CreateFromDateRange(DateRange DateRange) => CreateFromDateRange(DateRange.StartDate, DateRange.EndDate);

    /// <summary>
    /// Retorna um <see cref="Dictionary(Of String, T)"/> com as informações agrupadas
    /// por quinzena
    /// </summary>
    /// <returns></returns>
    public Dictionary<Fortnight, IEnumerable<T>> ToDataDictionary(bool IncludeFortnightsWithoutData = true)
    {
        var d = new Dictionary<Fortnight, IEnumerable<T>>();
        foreach (var k in this)
        {
            var dt = this[k.Key];
            if (dt.Any() || IncludeFortnightsWithoutData)
            {
                d.Add(k, dt ?? Array.Empty<T>().AsEnumerable());
            }
        }

        return d;
    }

    #endregion Public Methods
}

/// <summary>
/// Lista de dias agrupados em quinzenas
/// </summary>
public class FortnightGroup : ReadOnlyCollection<Fortnight>
{
    #region Private Methods

    /// <summary>
    /// Instancia um novo <see cref="FortnightGroup"/> a partir de uma data e um numero de quinzenas
    /// </summary>
    /// <param name="StartDate"></param>
    /// <param name="FortnightCount"></param>
    private static List<Fortnight> GerarLista(DateTime StartDate = default, int FortnightCount = 1)
    {
        var l = new List<Fortnight>
            {
                new Fortnight(StartDate)
            };
        for (int index = 2, loopTo = FortnightCount.SetMinValue(1); index <= loopTo; index++)
        {
            StartDate = l.Last().EndDate.AddDays(1d);
            l.Add(new Fortnight(StartDate));
        }

        return l;
    }

    #endregion Private Methods

    #region Public Constructors

    /// <summary>
    /// Instancia um novo <see cref="FortnightGroup"/> a partir de uma data e um numero de quinzenas
    /// </summary>
    /// <param name="StartDate"></param>
    /// <param name="FortnightCount"></param>
    public FortnightGroup(DateTime StartDate = default, int FortnightCount = 1) : base(GerarLista(StartDate, FortnightCount))
    {
    }

    #endregion Public Constructors

    #region Public Indexers

    /// <summary>
    /// Retorna uma quinzena a partir da sua Key
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    public Fortnight this[string Key] => base[IndexOf(this.Where(x => (x.Key ?? Util.EmptyString) == (Key ?? Util.EmptyString)).SingleOrDefault())];

    /// <summary>
    /// Retorna uma quinzena a partir da sua Index
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public new Fortnight this[int Index] => base[Index];

    #endregion Public Indexers

    #region Public Properties

    /// <summary>
    /// Retorna uma lista com todos os dias entre as quinzenas
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DateTime> AllDays => this.SelectMany(x => x.StartDate.GetDaysBetween(x.EndDate));

    /// <summary>
    /// Retorna a ultima data do periodo
    /// </summary>
    /// <returns></returns>
    public DateTime EndDate => this.Last().EndDate;

    /// <summary>
    /// Retorna um periodo equivalente a este grupo de quinzena
    /// </summary>
    /// <returns></returns>
    public DateRange Period => new DateRange(StartDate, EndDate);

    /// <summary>
    /// Retorna a data inicial do periodo
    /// </summary>
    /// <returns></returns>
    public DateTime StartDate => this.First().StartDate;

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Cria um grupo de quinzenas entre 2 datas
    /// </summary>
    /// <param name="StartDate"></param>
    /// <param name="EndDate"></param>
    /// <returns></returns>
    public static FortnightGroup CreateFromDateRange(DateTime StartDate, DateTime EndDate)
    {
        Util.FixOrder(ref StartDate, ref EndDate);
        int fortcount = 0;
        FortnightGroup fort;
        do
        {
            fort = new FortnightGroup(StartDate, fortcount++);
        } while (fort.EndDate.Date < EndDate.Date);
        return fort;
    }

    /// <summary>
    /// Cria um grupo de quinzenas entre 2 datas
    /// </summary>
    /// <param name="Range">Periodo</param>
    /// <returns></returns>
    public static FortnightGroup CreateFromDateRange(DateRange Range) => CreateFromDateRange(Range.StartDate, Range.EndDate);

    #endregion Public Methods
}
