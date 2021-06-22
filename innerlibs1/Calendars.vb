Imports System.Globalization
Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

Imports InnerLibs.LINQ
Imports InnerLibs.TimeMachine

Public Enum DateRangeInterval
    LessAccurate = -1
    Milliseconds = 0
    Seconds = 1
    Minutes = 2
    Hours = 3
    Days = 4
    Weeks = 5
    Months = 6
    Years = 7
End Enum

''' <summary>
''' Classe que representa um periodo entre 2 datas
''' </summary>
Public Class DateRange

    Public Property StartDate As Date
        Get
            FixDateOrder(_startDate, _enddate)
            If ForceFirstAndLastMoments Then
                _startDate = _startDate.Date
            End If
            Return _startDate
        End Get
        Set(value As Date)
            _isdefault = False
            If _startDate <> value Then
                _Difference = Nothing
                _startDate = value
            End If
        End Set
    End Property

    Public Property EndDate As Date
        Get
            FixDateOrder(_startDate, _enddate)
            If ForceFirstAndLastMoments Then
                _enddate = _enddate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
            End If
            Return _enddate
        End Get
        Set(value As Date)
            _isdefault = False
            If _enddate <> value Then
                _Difference = Nothing
                _enddate = value
            End If
        End Set
    End Property




    ''' <summary>
    ''' Se true, ajusta as horas de <see cref="StartDate"/> para o primeiro momento do dia e as horas de <see cref="EndDate"/> para o último momento do dia
    ''' </summary>
    Property ForceFirstAndLastMoments As Boolean = True

    Private _isdefault = False

    ''' <summary>
    ''' Indica se este <see cref="DateRange"/> foi construido sem nenhuma data definida
    ''' </summary>
    ''' <returns></returns>
    Public Function IsDefaultDateRange() As Boolean
        Return _isdefault
    End Function

    Private _startDate As Date
    Private _enddate As Date

    ''' <summary>
    ''' Instancia um novo periodo do dia de hoje
    ''' </summary>
    Public Sub New()
        Me.New(DateTime.Now, DateTime.Now, True)
        _isdefault = True
    End Sub

    Public Sub New(Dates As IEnumerable(Of Date))
        If Dates Is Nothing OrElse Not Dates.Any() Then
            Throw New ArgumentException("Argument 'Dates' is null or empty")
        End If
        Me.StartDate = Dates.Min()
        Me.EndDate = Dates.Max()
        Me.ForceFirstAndLastMoments = GetLessAccurateDateRangeInterval() > DateRangeInterval.Hours
        _isdefault = False
    End Sub

    Public Sub New(Dates As IEnumerable(Of Date?))
        Dates = If(Dates, {})
        Dates = Dates.Where(Function(x) x.HasValue)
        If Dates.Any() Then
            Me.StartDate = Dates.Min().Value
            Me.EndDate = Dates.Max().Value
            Me.ForceFirstAndLastMoments = GetLessAccurateDateRangeInterval() > DateRangeInterval.Hours
            _isdefault = False
        Else
            Throw New ArgumentException("Argument 'Dates' is null or empty")
        End If
    End Sub

    Public Sub New(Dates As IEnumerable(Of Date), ForceFirstAndLastMoments As Boolean)
        Me.New(Dates)
        Me.ForceFirstAndLastMoments = ForceFirstAndLastMoments
    End Sub

    Public Sub New(Dates As IEnumerable(Of Date?), ForceFirstAndLastMoments As Boolean)
        Me.New(Dates)
        Me.ForceFirstAndLastMoments = ForceFirstAndLastMoments
    End Sub

    ''' <summary>
    ''' Instancia um novo periodo entre 2 datas
    ''' </summary>
    ''' <param name="StartDate"></param>
    ''' <param name="EndDate"></param>
    Sub New(StartDate As Date, EndDate As Date)
        Me.StartDate = StartDate
        Me.EndDate = EndDate
        Me.ForceFirstAndLastMoments = GetLessAccurateDateRangeInterval() > DateRangeInterval.Hours
        _isdefault = False
    End Sub

    ''' <summary>
    ''' Instancia um novo periodo entre 2 datas
    ''' </summary>
    ''' <param name="StartDate"></param>
    ''' <param name="EndDate"></param>
    ''' <param name="ForceFirstAndLastMoments"> Ajusta as horas de <see cref="StartDate"/> para o primeiro momento do dia e as horas de <see cref="EndDate"/> para o último momento do dia</param>
    Sub New(StartDate As Date, EndDate As Date, ForceFirstAndLastMoments As Boolean)
        Me.StartDate = StartDate
        Me.EndDate = EndDate
        Me.ForceFirstAndLastMoments = ForceFirstAndLastMoments
        _isdefault = False
    End Sub

    ''' <summary>
    ''' Retorna uma lista de dias entre <see cref="StartDate"/> e <see cref="EndDate"/>
    ''' </summary>
    ''' <param name="DaysOfWeek"></param>
    ''' <returns></returns>
    Public Function GetDays(ParamArray DaysOfWeek As DayOfWeek()) As IEnumerable(Of Date)
        Return StartDate.GetDaysBetween(EndDate, DaysOfWeek)
    End Function

    ''' <summary>
    ''' Retorna o periodo em um total especificado por <see cref="DateRangeInterval"/>
    ''' </summary>
    ''' <param name="DateRangeInterval"></param>
    ''' <returns></returns>
    Public Function GetPeriodAs(Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As Decimal
        If DateRangeInterval = DateRangeInterval.LessAccurate Then
            DateRangeInterval = GetLessAccurateDateRangeInterval()
        End If
        Dim sd = StartDate
        Dim ed = EndDate
        If ForceFirstAndLastMoments Then
            ed = ed.AddHours(1).Date
        End If
        Dim range_diferenca = sd.GetDifference(ed)
        Select Case DateRangeInterval
            Case DateRangeInterval.Milliseconds
                Return range_diferenca.TotalMilliseconds
            Case DateRangeInterval.Seconds
                Return range_diferenca.TotalSeconds
            Case DateRangeInterval.Minutes
                Return range_diferenca.TotalMinutes
            Case DateRangeInterval.Hours
                Return range_diferenca.TotalHours
            Case DateRangeInterval.Days
                Return range_diferenca.TotalDays
            Case DateRangeInterval.Weeks
                Return range_diferenca.TotalWeeks
            Case DateRangeInterval.Months
                Return range_diferenca.TotalMonths
            Case DateRangeInterval.Years
                Return range_diferenca.TotalYears
        End Select
        Return -1
    End Function


    Public Shared Function AddInterval(Datetime As DateTime, DateRangeInterval As DateRangeInterval, Total As Decimal)
        If DateRangeInterval = DateRangeInterval.LessAccurate Then
            Throw New ArgumentException("You cant use LessAcurate on this scenario. LessAccurate only work inside DateRanges")
        End If
        Select Case DateRangeInterval
            Case DateRangeInterval.Seconds
                Return Datetime.AddSeconds(Total)
            Case DateRangeInterval.Minutes
                Return Datetime.AddMinutes(Total)
            Case DateRangeInterval.Hours
                Return Datetime.AddHours(Total)
            Case DateRangeInterval.Days
                Return Datetime.AddDays(Total)
            Case DateRangeInterval.Weeks
                Return Datetime.AddDays(Total * 7)
            Case DateRangeInterval.Months
                Return Datetime.AddMonths(Total)
            Case DateRangeInterval.Years
                Return Datetime.AddYears(Total)
            Case Else
                Return Datetime.AddMilliseconds(Total)
        End Select
    End Function

    ''' <summary>
    ''' Move um periodo a partir de um <paramref name="Total"/> especificado por <paramref name="DateRangeInterval"/>
    ''' </summary>
    ''' <param name="DateRangeInterval"></param>
    ''' <param name="Total"></param>
    ''' <returns></returns>
    Public Function MovePeriod(DateRangeInterval As DateRangeInterval, Total As Decimal) As DateRange
        If DateRangeInterval = DateRangeInterval.LessAccurate Then
            DateRangeInterval = GetLessAccurateDateRangeInterval()
        End If
        Return New DateRange(AddInterval(StartDate, DateRangeInterval, Total), AddInterval(EndDate, DateRangeInterval, Total), ForceFirstAndLastMoments)
    End Function

    ''' <summary>
    ''' Clona este DateRange
    ''' </summary>
    ''' <returns></returns>
    Public Function Clone() As DateRange
        Return New DateRange(StartDate, EndDate, ForceFirstAndLastMoments) With {._isdefault = Me._isdefault, ._Difference = _Difference}
    End Function

    ''' <summary>
    ''' Pula um determinado numero de periodos
    ''' </summary>
    ''' <returns></returns>
    Public Function JumpPeriod(Amount As Integer, Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As DateRange
        If Amount = 0 Then
            Return Clone()
        End If
        Return MovePeriod(DateRangeInterval, GetPeriodAs(DateRangeInterval) * Amount)
    End Function

    ''' <summary>
    ''' Move para o periodo equivalente anterior
    ''' </summary>
    ''' <returns></returns>
    Public Function PreviousPeriod(Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As DateRange
        Return MovePeriod(DateRangeInterval, -GetPeriodAs(DateRangeInterval))
    End Function

    ''' <summary>
    ''' Move para ao proximo periodo equivalente
    ''' </summary>
    ''' <returns></returns>
    Public Function NextPeriod(Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As DateRange
        Return MovePeriod(DateRangeInterval, GetPeriodAs(DateRangeInterval))
    End Function

    ''' <summary>
    ''' Retorna o <see cref="DateRangeInterval"/> menos preciso para calcular periodos
    ''' </summary>
    ''' <returns></returns>
    Public Function GetLessAccurateDateRangeInterval() As DateRangeInterval
        Dim sd = StartDate
        Dim ed = EndDate
        If ForceFirstAndLastMoments Then
            ed = ed.AddHours(1).Date
        End If

        Dim t = sd.GetDifference(ed)

        If t.TotalYears >= 1 OrElse t.TotalYears <= -1 Then
            Return DateRangeInterval.Years
        End If
        If t.TotalMonths >= 1 OrElse t.TotalMonths <= -1 Then
            Return DateRangeInterval.Months
        End If
        If t.TotalWeeks >= 1 OrElse t.TotalWeeks <= -1 Then
            Return DateRangeInterval.Weeks
        End If
        If t.TotalDays >= 1 OrElse t.TotalDays <= -1 Then
            Return DateRangeInterval.Days
        End If
        If t.TotalHours >= 1 OrElse t.TotalHours <= -1 Then
            Return DateRangeInterval.Hours
        End If
        If t.TotalMinutes >= 1 OrElse t.TotalMinutes <= -1 Then
            Return DateRangeInterval.Minutes
        End If
        If t.TotalSeconds >= 1 OrElse t.TotalSeconds <= -1 Then
            Return DateRangeInterval.Seconds
        End If

        Return DateRangeInterval.Milliseconds

    End Function

    ''' <summary>
    ''' Retorna TRUE se a data de inicio e fim for a mesma
    ''' </summary>
    ''' <returns></returns>
    Function IsSingleDate() As Boolean
        Return Me.StartDate.Date = Me.EndDate.Date
    End Function

    ''' <summary>
    ''' Retorna TRUE se a data e hora de inicio e fim for a mesma
    ''' </summary>
    ''' <returns></returns>
    Function IsSingleDateTime() As Boolean
        Return Me.StartDate = Me.EndDate
    End Function

    ''' <summary>
    ''' Retorna um <see cref="LongTimeSpan"/> contendo a diferença entre as datas
    ''' </summary>
    ''' <returns></returns>
    Function Difference() As LongTimeSpan
        If _Difference Is Nothing Then
            If ForceFirstAndLastMoments Then
                _Difference = StartDate.GetDifference(EndDate.AddSeconds(1))
            Else
                _Difference = StartDate.GetDifference(EndDate)
            End If
        End If
        Return _Difference
    End Function



    Private _Difference As LongTimeSpan = Nothing

    ''' <summary>
    ''' Cria um grupo de quinzenas que contenham este periodo
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateFortnightGroup() As FortnightGroup
        Return FortnightGroup.CreateFromDateRange(Me.StartDate, Me.EndDate)
    End Function

    ''' <summary>
    ''' Retorna uma strin representando a diferença das datas
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return Difference.ToString
    End Function

    ''' <summary>
    ''' Filtra uma lista considerando o periodo deste DateRange
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="PropertyExpression"></param>
    ''' <returns></returns>
    Public Function FilterList(Of T)(List As IEnumerable(Of T), PropertyExpression As Expression(Of Func(Of T, DateTime))) As IEnumerable(Of T)
        Return List.Where(LINQExtensions.IsBetween(PropertyExpression, Me).Compile())
    End Function

    ''' <summary>
    ''' Filtra uma lista considerando o periodo deste DateRange
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="PropertyExpression"></param>
    ''' <returns></returns>
    Public Function FilterList(Of T)(List As IQueryable(Of T), PropertyExpression As Expression(Of Func(Of T, DateTime))) As IQueryable(Of T)
        Return List.Where(LINQExtensions.IsBetween(PropertyExpression, Me))
    End Function

    ''' <summary>
    ''' Filtra uma lista considerando o periodo deste DateRange
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="PropertyExpression"></param>
    ''' <returns></returns>
    Public Function FilterList(Of T)(List As IEnumerable(Of T), PropertyExpression As Expression(Of Func(Of T, DateTime?))) As IEnumerable(Of T)
        Return List.Where(LINQExtensions.IsBetween(PropertyExpression, Me).Compile())
    End Function

    ''' <summary>
    ''' Filtra uma lista considerando o periodo deste DateRange
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="PropertyExpression"></param>
    ''' <returns></returns>
    Public Function FilterList(Of T)(List As IQueryable(Of T), PropertyExpression As Expression(Of Func(Of T, DateTime?))) As IQueryable(Of T)
        Return List.Where(LINQExtensions.IsBetween(PropertyExpression, Me))
    End Function

    ''' <summary>
    ''' Agrupa itens de uma lista de acordo com uma propriedade e uma expressão de agrugrupamento de datas
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="PropertyExpression"></param>
    ''' <param name="GroupByExpression"></param>
    ''' <param name="DateRangeInterval"></param>
    ''' <returns></returns>
    Public Function GroupList(Of T)(List As IEnumerable(Of T), PropertyExpression As Func(Of T, DateTime), GroupByExpression As Func(Of DateTime, String), Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As Dictionary(Of String, IEnumerable(Of T))
        Dim keys = GetBetween(DateRangeInterval).Select(GroupByExpression).Distinct()
        Dim gp = List.GroupBy(Function(x) GroupByExpression(PropertyExpression(x)))
        Dim dic As New Dictionary(Of String, IEnumerable(Of T))
        For Each k In keys
            If Not dic.ContainsKey(k) Then
                dic(k) = New List(Of T)
            End If
            CType(dic(k), List(Of T)).AddRange(gp(k).AsEnumerable)
        Next
        Return dic
    End Function

    Public Function GroupList(Of T)(List As IEnumerable(Of T), PropertyExpression As Func(Of T, DateTime?), GroupByExpression As Func(Of DateTime?, String), Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As Dictionary(Of String, IEnumerable(Of T))
        Dim keys = GetBetween(DateRangeInterval).Cast(Of Date?).Select(GroupByExpression).Distinct()
        Dim gp = List.GroupBy(Function(x) GroupByExpression(PropertyExpression(x))).ToDictionary()
        Dim dic As New Dictionary(Of String, IEnumerable(Of T))
        For Each k In keys
            If Not dic.ContainsKey(k) Then
                dic(k) = New List(Of T)
            End If
            Dim l = CType(dic(k), List(Of T))
            If (gp.ContainsKey(k)) Then
                For Each item In gp(k).AsEnumerable
                    l.Add(item)
                Next
            End If
            dic(k) = l
        Next
        Return dic
    End Function

    ''' <summary>
    ''' Verifica se 2 periodos possuem interseção de datas
    ''' </summary>
    ''' <param name="Period">Periodo</param>
    ''' <returns></returns>
    Public Function Overlaps(Period As DateRange) As Boolean
        FixDateOrder(StartDate, EndDate)
        Select Case True
            Case Period.StartDate <= Me.EndDate And Period.StartDate >= Me.StartDate
                Return True
            Case Me.StartDate <= Period.EndDate And Me.StartDate >= Period.StartDate
                Return True
            Case Else
                Return False
        End Select
    End Function

    ''' <summary>
    ''' Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou contém um periodo)
    ''' </summary>
    ''' <param name="Period"></param>
    ''' <returns></returns>
    '''
    Public Function MatchAny(Period As DateRange) As Boolean
        FixDateOrder(StartDate, EndDate)
        Return Me.Overlaps(Period) Or Me.Contains(Period) Or Me.IsIn(Period)
    End Function

    ''' <summary>
    ''' Verifica se este periodo contém um outro periodo
    ''' </summary>
    ''' <param name="Period"></param>
    ''' <returns></returns>
    Public Function Contains(Period As DateRange) As Boolean
        FixDateOrder(StartDate, EndDate)
        Return Me.StartDate <= Period.StartDate And Period.EndDate <= Me.EndDate
    End Function

    ''' <summary>
    ''' Verifica se este periodo contém uma data
    ''' </summary>
    ''' <param name="Day"></param>
    ''' <returns></returns>
    Public Function Contains(Day As Date) As Boolean
        FixDateOrder(StartDate, EndDate)
        Return Me.StartDate <= Day And Day <= Me.EndDate
    End Function

    ''' <summary>
    ''' Verifica se hoje está dentro deste periodo
    ''' </summary>
    ''' <returns></returns>
    Public Function IsNow() As Boolean
        Return Contains(DateTime.Now)
    End Function

    ''' <summary>
    ''' Verifica se este periodo está dentro de outro periodo
    ''' </summary>
    ''' <param name="Period"></param>
    ''' <returns></returns>
    Public Function IsIn(Period As DateRange) As Boolean
        FixDateOrder(StartDate, EndDate)
        Return Period.Contains(Me)
    End Function

    ''' <summary>
    ''' Verifica quantos porcento uma data representa  em distancia dentro deste periodo
    ''' </summary>
    ''' <param name="[Date]">Data correspondente</param>
    ''' <returns></returns>
    Public Function CalculatePercent(Optional [Date] As Date? = Nothing) As Decimal
        Return If([Date], DateTime.Now).CalculatePercent(StartDate, EndDate)
    End Function

    Public Function Pair() As IEnumerable(Of DateTime)
        Return {StartDate, EndDate}
    End Function

    ''' <summary>
    ''' Retorna uma lista com as datas entre <see cref="StartDate"/> e <see cref="EndDate"/> utilizando um Intervalo
    ''' </summary>
    ''' <param name="DateRangeInterval"></param>
    ''' <returns></returns>
    Public Function GetBetween(Optional DateRangeInterval As DateRangeInterval = DateRangeInterval.LessAccurate) As IEnumerable(Of DateTime)
        If DateRangeInterval = DateRangeInterval.LessAccurate Then
            DateRangeInterval = GetLessAccurateDateRangeInterval()
        End If
        Dim l = New List(Of Date) From {StartDate}
        Dim curdate = StartDate
        While curdate < EndDate
            curdate = AddInterval(curdate, DateRangeInterval, 1)
            l.Add(curdate)
        End While
        l.Add(EndDate)
        Return l.Where(Function(x) x <= EndDate).Where(Function(x) x >= StartDate).Distinct()
    End Function

End Class

''' <summary>
''' Modulo para manipulação de calendário
''' </summary>
''' <remarks></remarks>
Public Module Calendars

    <Extension()>
    Public Function CreateDateRange(Of T As Class)(ByVal List As IQueryable(Of T), ByVal PropertyExpression As Expression(Of Func(Of T, DateTime?)), ByVal Optional StartDate As DateTime? = Nothing, ByVal Optional EndDate As DateTime? = Nothing) As DateRange

        Dim Period = New DateRange()

        Period.ForceFirstAndLastMoments = True

        StartDate = If(StartDate, List.Min(PropertyExpression))
        EndDate = If(EndDate, List.Max(PropertyExpression))

        If StartDate.HasValue Then
            Period.StartDate = StartDate.Value
        End If

        If EndDate.HasValue Then
            Period.StartDate = EndDate.Value
        End If

        Return Period
    End Function


    ''' <summary>
    ''' Pega o numero da semana a partir de uma data
    ''' </summary>
    ''' <param name="DateAndTime"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetWeekNumberOfMonth(ByVal DateAndTime As DateTime) As Integer
        Return GetWeekInfoOfMonth(DateAndTime).FirstOrDefault()
    End Function

    ''' <summary>
    ''' Pega o numero da semana, do mês e ano pertencente
    ''' </summary>
    ''' <param name="DateAndTime"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetWeekInfoOfMonth(DateAndTime As DateTime) As Integer()
        DateAndTime = DateAndTime.Date
        Dim firstMonthDay As DateTime = DateAndTime.GetFirstDayOfMonth
        Dim firstMonthMonday As DateTime = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) Mod 7)

        If firstMonthMonday > DateAndTime Then
            firstMonthDay = firstMonthDay.AddMonths(-1)
            firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) Mod 7)
        End If

        Return New Integer() {(DateAndTime - firstMonthMonday).Days / 7 + 1, firstMonthDay.Month, firstMonthDay.Year}
    End Function

    ''' <summary>
    ''' Pega o numero do Bimestre a partir de uma data
    ''' </summary>
    ''' <param name="DateAndtime"></param>
    ''' <returns></returns>
    <Extension> Public Function GetDoubleMonthOfYear(DateAndtime As DateTime) As Integer
        If DateAndtime.Month <= 2 Then
            Return 1
        ElseIf DateAndtime.Month <= 4 Then
            Return 2
        ElseIf DateAndtime.Month <= 6 Then
            Return 3
        ElseIf DateAndtime.Month <= 8 Then
            Return 4
        ElseIf DateAndtime.Month <= 10 Then
            Return 5
        Else
            Return 6
        End If
    End Function

    ''' <summary>
    ''' Pega o numero do trimestre a partir de uma data
    ''' </summary>
    ''' <param name="DateAndTime"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetQuarterOfYear(ByVal DateAndTime As DateTime) As Integer
        If DateAndTime.Month <= 3 Then
            Return 1
        ElseIf DateAndTime.Month <= 6 Then
            Return 2
        ElseIf DateAndTime.Month <= 9 Then
            Return 3
        Else
            Return 4
        End If
    End Function

    ''' <summary>
    ''' Pega o numero do semestre a partir de uma data
    ''' </summary>
    ''' <param name="DateAndTime"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetHalfOfYear(ByVal DateAndTime As DateTime) As Integer
        If DateAndTime.Month <= 6 Then
            Return 1
        Else
            Return 2
        End If
    End Function

    ''' <summary>
    ''' Retorna a idade
    ''' </summary>
    ''' <param name="BirthDate"></param>
    ''' <param name="FromDate"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetAge(BirthDate As DateTime, Optional FromDate As DateTime? = Nothing) As Integer
        FromDate = If(FromDate, DateTime.Now)
        Dim age As Integer
        age = FromDate.Value.Year - BirthDate.Year
        If (BirthDate > DateTime.Today.AddYears(-age)) Then age -= 1
        Return age
    End Function

    ''' <summary>
    ''' Converte um <see cref="Date"/> para um timezone Especifico
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <param name="TimeZone"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToTimeZoneUtc([Date] As Date, TimeZone As TimeZoneInfo) As Date
        Return TimeZoneInfo.ConvertTimeFromUtc([Date], TimeZone)
    End Function

    ''' <summary>
    ''' Converte um <see cref="Date"/> para um timezone Especifico
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <param name="TimeZoneId"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToTimeZone([Date] As Date, TimeZoneId As String) As Date
        Return TimeZoneInfo.ConvertTime([Date], TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId))
    End Function

    ''' <summary>
    ''' Converte uma string em datetime a partir de um formato especifico
    ''' </summary>
    ''' <param name="DateString">String original</param>
    ''' <param name="Format"></param>
    ''' <param name="Culture"></param>
    ''' <returns></returns>
    <Extension()> Public Function ConvertDateString(DateString As String, Format As String, Optional Culture As CultureInfo = Nothing) As Date
        Culture = If(Culture, CultureInfo.CurrentCulture)
        Return DateTime.ParseExact(DateString, Format, Culture)
    End Function

    ''' <summary>
    ''' Converte uma string de data para outra string de data com formato diferente
    ''' </summary>
    ''' <param name="DateString">String original</param>
    ''' <param name="InputFormat"></param>
    ''' <param name="Culture"></param>
    ''' <returns></returns>
    <Extension()> Public Function ChangeFormat(DateString As String, InputFormat As String, OutputFormat As String, Optional Culture As CultureInfo = Nothing) As String
        Return DateString.ConvertDateString(InputFormat, Culture).ToString(OutputFormat)
    End Function

    ''' <summary>
    ''' Pula para a data inicial da proxima quinzena
    ''' </summary>
    ''' <param name="FromDate">Data de partida</param>
    ''' <param name="Num">Numero de quinzenas para adiantar</param>
    ''' <returns></returns>
    <Extension> Public Function NextFortnight(FromDate As Date, Optional Num As Integer = 1) As DateTime
        Return New FortnightGroup(FromDate, Num).Last.Period.StartDate
    End Function

    ''' <summary>
    ''' Retorna o primeiro dia da semana da data especificada
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetFirstDayOfWeek([Date] As DateTime, Optional FirstDayOfWeek As DayOfWeek = DayOfWeek.Sunday) As DateTime
        While [Date].DayOfWeek > FirstDayOfWeek
            [Date] = [Date].AddDays(-1)
        End While
        Return [Date]
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia da semana da data especificada
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetLastDayOfWeek([Date] As DateTime, Optional FirstDayOfWeek As DayOfWeek = DayOfWeek.Sunday) As DateTime
        Return [Date].GetFirstDayOfWeek(FirstDayOfWeek).AddDays(6)
    End Function

    ''' <summary>
    ''' Retorna um DateRange equivalente a semana de uma data especifica
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é domingo)</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetWeek([Date] As DateTime, Optional FirstDayOfWeek As DayOfWeek = DayOfWeek.Sunday) As DateRange
        Return New DateRange([Date].GetFirstDayOfWeek(FirstDayOfWeek), [Date].GetLastDayOfWeek(FirstDayOfWeek))
    End Function

    ''' <summary>
    ''' Retorna a ultima data do mes a partir de uma outra data
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetLastDayOfMonth([Date] As DateTime) As DateTime
        Return New DateTime([Date].Year, [Date].Month, DateTime.DaysInMonth([Date].Year, [Date].Month), [Date].Hour, [Date].Minute, [Date].Second, [Date].Millisecond, [Date].Kind)
    End Function

    ''' <summary>
    ''' Retorna a ultima data do mes a partir de uma outra data
    ''' </summary>
    ''' <param name="MonthNumber">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetLastDayOfMonth(MonthNumber As Integer, Optional Year As Integer? = Nothing) As DateTime
        Year = If(Year, DateTime.Now.Month).SetMinValue(DateTime.MinValue.Month)
        Return New Date(Year, MonthNumber, 1).GetLastDayOfMonth
    End Function

    ''' <summary>
    ''' Retorna a ultima data do mes a partir de uma outra data
    ''' </summary>
    ''' <param name="MonthNumber">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetFirstDayOfMonth(MonthNumber As Integer, Optional Year As Integer? = Nothing) As DateTime
        Year = If(Year, DateTime.Now.Month).SetMinValue(DateTime.MinValue.Month)
        Return New Date(Year, MonthNumber, 1)
    End Function

    ''' <summary>
    ''' Retorna a primeira data do mes a partir de uma outra data
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetFirstDayOfMonth([Date] As DateTime) As DateTime
        Return New DateTime([Date].Year, [Date].Month, 1, [Date].Hour, [Date].Minute, [Date].Second, [Date].Millisecond, [Date].Kind)
    End Function

    ''' <summary>
    ''' Retorna a primeira data da quinzena a partir de uma outra data
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetFirstDayOfFortnight([Date] As DateTime) As DateTime
        Return New DateTime([Date].Year, [Date].Month, If([Date].Day <= 15, 1, 16), [Date].Hour, [Date].Minute, [Date].Second, [Date].Millisecond, [Date].Kind)
    End Function

    ''' <summary>
    ''' Retorna a ultima data da quinzena a partir de uma outra data
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetLastDayOfFortnight([Date] As DateTime) As DateTime
        Return New DateTime([Date].Year, [Date].Month, If([Date].Day <= 15, 15, [Date].GetLastDayOfMonth().Day), [Date].Hour, [Date].Minute, [Date].Second, [Date].Millisecond, [Date].Kind)
    End Function

    ''' <summary>
    ''' Retorna o prmeiro dia de um ano especifico de outra data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetFirstDayOfYear([Date] As DateTime) As DateTime
        Return New DateTime([Date].Year, 1, 1).Date
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia de um ano especifico de outra data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetLastDayOfYear([Date] As DateTime) As DateTime
        Return New DateTime([Date].Year, 12, 31).Date
    End Function

    ''' <summary>
    ''' Retorna o prmeiro dia de um semestre a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetFirstDayOfHalf([Date] As DateTime) As DateTime
        If [Date].GetHalfOfYear() = 1 Then
            Return [Date].GetFirstDayOfYear()
        Else
            Return New Date([Date].Year, 7, 1).Date
        End If
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia de um semestre a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetLastDayOfHalf([Date] As DateTime) As DateTime
        If [Date].GetHalfOfYear() = 1 Then
            Return New Date([Date].Year, 6, 1).GetLastDayOfMonth
        Else
            Return [Date].GetLastDayOfYear
        End If
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia de um trimestre a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetLastDayOfQuarter([Date] As DateTime) As DateTime
        If [Date].GetQuarterOfYear() = 1 Then Return New Date([Date].Year, 3, 1).GetLastDayOfMonth()
        If [Date].GetQuarterOfYear() = 2 Then Return New Date([Date].Year, 6, 1).GetLastDayOfMonth()
        If [Date].GetQuarterOfYear() = 3 Then Return New Date([Date].Year, 9, 1).GetLastDayOfMonth()
        Return New Date([Date].Year, 12, 1).GetLastDayOfMonth()
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia de um trimestre a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetFirstDayOfQuarter([Date] As DateTime) As DateTime
        If [Date].GetQuarterOfYear() = 1 Then Return New Date([Date].Year, 1, 1).Date
        If [Date].GetQuarterOfYear() = 2 Then Return New Date([Date].Year, 4, 1).Date
        If [Date].GetQuarterOfYear() = 3 Then Return New Date([Date].Year, 7, 1).Date
        Return New Date([Date].Year, 10, 1).Date
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia de um bimestre a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetLastDayOfDoubleMonth([Date] As DateTime) As DateTime
        If [Date].GetDoubleMonthOfYear() = 1 Then Return New Date([Date].Year, 2, 1).GetLastDayOfMonth
        If [Date].GetDoubleMonthOfYear() = 2 Then Return New Date([Date].Year, 4, 1).GetLastDayOfMonth
        If [Date].GetDoubleMonthOfYear() = 3 Then Return New Date([Date].Year, 6, 1).GetLastDayOfMonth
        If [Date].GetDoubleMonthOfYear() = 4 Then Return New Date([Date].Year, 8, 1).GetLastDayOfMonth
        If [Date].GetDoubleMonthOfYear() = 5 Then Return New Date([Date].Year, 10, 1).GetLastDayOfMonth
        Return New Date([Date].Year, 12, 1).GetLastDayOfMonth
    End Function

    ''' <summary>
    ''' Retorna o ultimo dia de um bimestre a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetFirstDayOfDoubleMonth([Date] As DateTime) As DateTime
        If [Date].GetDoubleMonthOfYear() = 1 Then Return New Date([Date].Year, 1, 1).Date
        If [Date].GetDoubleMonthOfYear() = 2 Then Return New Date([Date].Year, 3, 1).Date
        If [Date].GetDoubleMonthOfYear() = 3 Then Return New Date([Date].Year, 5, 1).Date
        If [Date].GetDoubleMonthOfYear() = 4 Then Return New Date([Date].Year, 7, 1).Date
        If [Date].GetDoubleMonthOfYear() = 5 Then Return New Date([Date].Year, 9, 1).Date
        Return New Date([Date].Year, 11, 1).Date
    End Function

    ''' <summary>
    ''' Retorna o numero da semana relativa ao ano
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <param name="Culture"></param>
    ''' <param name="FirstDayOfWeek"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetWeekOfYear(ByVal [Date] As DateTime, Optional Culture As CultureInfo = Nothing, Optional FirstDayOfWeek As DayOfWeek = DayOfWeek.Sunday) As Integer
        Return If(Culture, CultureInfo.InvariantCulture).Calendar.GetWeekOfYear([Date], CalendarWeekRule.FirstFourDayWeek, FirstDayOfWeek)
    End Function

    ''' <summary>
    ''' Verifica se uma data é do mesmo mês e ano que outra data
    ''' </summary>
    ''' <param name="[Date]">Primeira data</param>
    ''' <param name="AnotherDate">Segunda data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsSameMonthAndYear([Date] As DateTime, AnotherDate As DateTime) As Boolean
        Return [Date].IsBetween(AnotherDate.GetFirstDayOfMonth, AnotherDate.GetLastDayOfMonth)
    End Function

    ''' <summary>
    ''' Verifica se a Data de hoje é um aniversário
    ''' </summary>
    ''' <param name="BirthDate">  Data de nascimento</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsAnniversary(BirthDate As Date, Optional CompareWith As Date? = Nothing) As Boolean
        If Not CompareWith.HasValue Then CompareWith = DateTime.Today
        Return (BirthDate.Day & "/" & BirthDate.Month) = (CompareWith.Value.Day & "/" & CompareWith.Value.Month)
    End Function

    ''' <summary>
    ''' Retorna o nome do mês a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <param name="Culture"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetLongMonthName([Date] As DateTime, Optional Culture As CultureInfo = Nothing) As String
        Return [Date].ToString("MMMM", If(Culture, CultureInfo.CurrentCulture))
    End Function

    ''' <summary>
    ''' Retorna o nome do mês a partir da data
    ''' </summary>
    ''' <param name="[Date]"></param>
    ''' <param name="Culture"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetShortMonthName([Date] As DateTime, Optional Culture As CultureInfo = Nothing) As String
        Return [Date].ToString("MM", If(Culture, CultureInfo.CurrentCulture))
    End Function

    ''' <summary>
    ''' COnverte um datetime para o formato de string do SQL server ou Mysql
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToSQLDateString([Date] As DateTime) As String
        Return [Date].Year & "-" & [Date].Month & "-" & [Date].Day & " " & [Date].Hour & ":" & [Date].Minute & ":" & [Date].Second & "." & [Date].Millisecond
    End Function

    ''' <summary>
    ''' Converte uma string dd/mm/aaaa hh:mm:ss.llll para o formato de string do SQL server ou Mysql
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension>
    Function ToSQLDateString([Date] As String, Optional FromCulture As String = "pt-BR") As String
        Return Convert.ToDateTime([Date], New CultureInfo(FromCulture, False).DateTimeFormat).ToSQLDateString
    End Function

    ''' <summary>
    ''' Retorna uma <see cref="LongTimeSpan"/> com a diferença entre 2 Datas
    ''' </summary>
    ''' <param name="InitialDate"></param>
    ''' <param name="SecondDate"> </param>
    ''' <returns></returns>
    <Extension>
    Public Function GetDifference(InitialDate As DateTime, SecondDate As DateTime) As LongTimeSpan
        FixDateOrder(InitialDate, SecondDate)
        Return New LongTimeSpan(InitialDate, SecondDate)
    End Function

    ''' <summary>
    ''' Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate
    ''' sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um
    ''' <see cref="TimeSpan"/> negativo
    ''' </summary>
    ''' <param name="StartDate">Data Inicial</param>
    ''' <param name="EndDate">  Data Final</param>
    Public Sub FixDateOrder(ByRef StartDate As DateTime, ByRef EndDate As DateTime)
        FixOrder(StartDate, EndDate)
    End Sub

    ''' <summary>
    ''' Verifica se uma data se encontra entre 2 datas
    ''' </summary>
    ''' <param name="MidDate">   Data</param>
    ''' <param name="StartDate"> Data Inicial</param>
    ''' <param name="EndDate">   Data final</param>
    ''' <param name="IgnoreTime">Indica se o tempo deve ser ignorado na comparação</param>
    ''' <returns></returns>
    <Extension()> Public Function IsBetween(MidDate As DateTime, StartDate As DateTime, EndDate As DateTime, Optional IgnoreTime As Boolean = False) As Boolean
        FixDateOrder(StartDate, EndDate)
        If IgnoreTime Then
            Return StartDate.Date <= MidDate.Date And MidDate.Date <= EndDate.Date
        Else
            Return StartDate <= MidDate And MidDate <= EndDate
        End If
    End Function

    ''' <summary>
    ''' Retorna as datas entre um periodo
    ''' </summary>
    ''' <param name="StartDate"></param>
    ''' <param name="EndDate"></param>
    ''' <param name="DaysOfWeek"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetDaysBetween(StartDate As Date, EndDate As Date, ParamArray DaysOfWeek() As DayOfWeek) As IEnumerable(Of Date)
        Dim l = New List(Of Date) From {StartDate.Date}
        DaysOfWeek = If(DaysOfWeek, {})
        If DaysOfWeek.Length = 0 Then
            DaysOfWeek = {DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday}
        End If
        Dim curdate = StartDate.Date
        While curdate.Date < EndDate.Date
            curdate = curdate.AddDays(1)
            l.Add(curdate)
        End While
        l.Add(EndDate.Date)
        Return l.Distinct().Where(Function(x) x.DayOfWeek.IsIn(DaysOfWeek))
    End Function

    ''' <summary>
    ''' Remove o tempo de todas as datas de uma lista e retorna uma nova lista
    ''' </summary>
    ''' <param name="List">Lista que será alterada</param>
    <Extension()> Function ClearTime(List As IEnumerable(Of Date)) As IEnumerable(Of Date)
        Return List.Select(Function(x) x.Date)
    End Function

    ''' <summary>
    ''' Retorna uma String no formato "W dias, X horas, Y minutos e Z segundos"
    ''' </summary>
    ''' <param name="TimeElapsed">TimeSpan com o intervalo</param>
    ''' <returns>string</returns>
    <Extension()>
    Public Function ToTimeElapsedString(TimeElapsed As TimeSpan, Optional DayWord As String = "dia", Optional HourWord As String = "hora", Optional MinuteWord As String = "minuto", Optional SecondWord As String = "segundo") As String
        Dim dia As String = (If(TimeElapsed.Days > 0, (If(TimeElapsed.Days = 1, TimeElapsed.Days & " " & DayWord & " ", TimeElapsed.Days & " " & DayWord & "s ")), ""))
        Dim horas As String = (If(TimeElapsed.Hours > 0, (If(TimeElapsed.Hours = 1, TimeElapsed.Hours & " " & HourWord & " ", TimeElapsed.Hours & " " & HourWord & "s ")), ""))
        Dim minutos As String = (If(TimeElapsed.Minutes > 0, (If(TimeElapsed.Minutes = 1, TimeElapsed.Minutes & " " & MinuteWord & " ", TimeElapsed.Minutes & " " & MinuteWord & "s ")), ""))
        Dim segundos As String = (If(TimeElapsed.Seconds > 0, (If(TimeElapsed.Seconds = 1, TimeElapsed.Seconds & " " & SecondWord & " ", TimeElapsed.Seconds & " " & SecondWord & "s ")), ""))

        dia &= If(",", dia.IsNotBlank And (horas.IsNotBlank Or minutos.IsNotBlank Or segundos.IsNotBlank))
        horas &= If(",", horas.IsNotBlank And (minutos.IsNotBlank Or segundos.IsNotBlank))
        minutos &= If(",", minutos.IsNotBlank And (segundos.IsNotBlank))
        Dim current As String = dia & " " & horas & " " & minutos & " " & segundos
        If current.Contains(",") Then
            current = current.Insert(current.LastIndexOf(","), " e ")
            current = current.Remove(current.LastIndexOf(","), 1)
        End If
        Return current.AdjustWhiteSpaces
    End Function

    ''' <summary>
    ''' Retorna uma String baseado no numero do Mês Ex.: 1 -&gt; Janeiro
    ''' </summary>
    ''' <param name="MonthNumber">Numero do Mês</param>
    ''' <returns>String com nome do Mês</returns>

    <Extension()>
    Function ToLongMonthName(MonthNumber As Integer) As String
        Return New Date(DateTime.Now.Year, MonthNumber, 1).TolongMonthName
    End Function

    ''' <summary>
    ''' Retorna uma String com o nome do mes baseado na data
    ''' </summary>
    ''' <param name="DateTime">Data</param>
    ''' <returns>String com nome do Mês</returns>
    <Extension()>
    Function TolongMonthName(DateTime As Date) As String
        Return DateTime.ToString("MMMM")
    End Function

    ''' <summary>
    ''' Retorna uma String curta baseado no numero do Mês Ex.: 1 -&gt; Jan
    ''' </summary>
    ''' <param name="MonthNumber">Numero do Mês</param>
    ''' <returns>String com nome curto do Mês</returns>

    <Extension()>
    Public Function ToShortMonthName(MonthNumber As Integer) As String
        Return ToLongMonthName(MonthNumber).GetFirstChars(3)
    End Function

    ''' <summary>
    ''' Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Segunda-Feira
    ''' </summary>
    ''' <param name="DayNumber">Numero do Dia</param>
    ''' <returns>String com nome do Dia</returns>

    <Extension()>
    Function ToLongDayOfWeekName(DayNumber As Integer) As String
        Return DateTimeFormatInfo.CurrentInfo.GetDayName(DayNumber)
    End Function

    ''' <summary>
    ''' Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Seg
    ''' </summary>
    ''' <param name="DayNumber">Numero do Dia</param>
    ''' <returns>String com nome curto do Dia</returns>

    <Extension()>
    Public Function ToShortDayOfWeekName(DayNumber As Integer) As String
        Return ToLongDayOfWeekName(DayNumber).GetFirstChars(3)
    End Function

    ''' <summary>
    ''' Retorna a data de amanhã
    ''' </summary>
    ''' <returns>Data de amanhã</returns>

    Public ReadOnly Property Tomorrow() As DateTime
        Get
            Return DateTime.Now.AddDays(1)
        End Get
    End Property

    ''' <summary>
    ''' Retorna a data de ontem
    ''' </summary>
    ''' <returns>Data de ontem</returns>

    Public ReadOnly Property Yesterday() As DateTime
        Get
            Return DateTime.Now.AddDays(-1)
        End Get
    End Property

    Public ReadOnly Property BrazilianNow() As DateTime
        Get
            Return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))
        End Get
    End Property

    Public ReadOnly Property BrazilianTomorrow() As DateTime
        Get
            Return BrazilianNow.AddDays(1)
        End Get
    End Property

    Public ReadOnly Property BrazilianYesterday() As DateTime
        Get
            Return BrazilianNow.AddDays(-1)
        End Get
    End Property

    ''' <summary>
    ''' Retorna o ultimo domingo
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LastSunday(Optional FromDate As Date? = Nothing)
        Get
            Return LastDay(DayOfWeek.Sunday, FromDate)
        End Get
    End Property

    ''' <summary>
    ''' Retorna o proximo domingo
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NextSunday(Optional FromDate As Date? = Nothing) As Date
        Get
            Return NextDay(DayOfWeek.Sunday, FromDate)
        End Get
    End Property

    Public ReadOnly Property LastDay(DayOfWeek As DayOfWeek, Optional FromDate As Date? = Nothing) As Date
        Get
            FromDate = If(FromDate, DateTime.Now)
            While FromDate.Value.DayOfWeek <> DayOfWeek
                FromDate = FromDate.Value.AddDays(-1)
            End While
            Return FromDate
        End Get
    End Property

    Public ReadOnly Property NextDay(DayOfWeek As DayOfWeek, Optional FromDate As Date? = Nothing) As Date
        Get
            FromDate = If(FromDate, DateTime.Now)
            While FromDate.Value.DayOfWeek <> DayOfWeek
                FromDate = FromDate.Value.AddDays(1)
            End While
            Return FromDate
        End Get
    End Property

    ''' <summary>
    ''' Verifica se o dia se encontra no fim de semana
    ''' </summary>
    ''' <param name="YourDate">Uma data qualquer</param>
    ''' <returns>TRUE se for sabado ou domingo, caso contrario FALSE</returns>

    <Extension()>
    Public Function IsWeekend(YourDate As DateTime) As Boolean
        Return (YourDate.DayOfWeek = DayOfWeek.Sunday Or YourDate.DayOfWeek = DayOfWeek.Saturday)
    End Function

    <Extension()>
    Private Function ToGreetingFarewell(Time As DateTime, Optional Language As String = "pt", Optional Farewell As Boolean = False) As String

        Dim bomdia As String = "Bom dia"
        Dim boatarde As String = "Boa tarde"
        Dim boanoite As String = "Boa noite"
        Dim boanoite_despedida As String = boanoite
        Dim seDespedidaManha As String = "tenha um ótimo dia"
        Dim seDespedidaTarde As String = "tenha uma ótima tarde"

        Select Case Language.ToLower()
            Case "en", "eng", "ingles", "english", "inglés"
                bomdia = "Good morning"
                boatarde = "Good afternoon"
                boanoite = "Good evening"
                boanoite_despedida = "Good night"
                seDespedidaManha = "have a nice day"
                seDespedidaTarde = "have a great afternoon"
                Exit Select
            Case "es", "esp", "espanhol", "espanol", "español", "spanish"
                bomdia = "Buenos días"
                boatarde = "Buenas tardes"
                boanoite = "Buenas noches"
                boanoite_despedida = boanoite
                seDespedidaManha = "que tengas un buen día"
                seDespedidaTarde = "que tengas una buena tarde"
                Exit Select
        End Select

        If Time.Hour < 12 Then
            Return If(Farewell, seDespedidaManha, bomdia)
        ElseIf Time.Hour >= 12 AndAlso Time.Hour < 18 Then
            Return If(Farewell, seDespedidaTarde, boatarde)
        Else
            Return If(Farewell, boanoite_despedida, boanoite)
        End If
    End Function

    ''' <summary>
    ''' Transforma um DateTime em uma despedida (Bom dia, Boa tarde, Boa noite)
    ''' </summary>
    ''' <param name="Time">    Horario</param>
    ''' <param name="Language">Idioma da saudação (pt, en, es)</param>
    ''' <returns>Uma string com a despedida</returns>
    <Extension>
    Public Function ToFarewell(Time As DateTime, Optional Language As String = "pt") As String
        Return Time.ToGreetingFarewell(Language, True)
    End Function

    ''' <summary>
    ''' Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite)
    ''' </summary>
    ''' <param name="Time">    Horario</param>
    ''' <param name="Language">Idioma da saudação (pt, en, es)</param>
    ''' <returns>Uma string com a despedida</returns>
    <Extension>
    Public Function ToGreeting(Time As DateTime, Optional Language As String = "pt") As String
        Return Time.ToGreetingFarewell(Language, False)
    End Function

    ''' <summary>
    ''' Retorna uma saudação
    ''' </summary>
    ''' <param name="Language">Idioma da saudação (pt, en, es)</param>
    ''' <returns>Uma string com a saudação</returns>
    Public ReadOnly Property Greeting(Optional Language As String = "pt") As String
        Get
            Return DateTime.Now.ToGreetingFarewell(Language)
        End Get
    End Property

    ''' <summary>
    ''' Retorna uma despedida
    ''' </summary>
    ''' <param name="Language">Idioma da despedida (pt, en, es)</param>
    ''' <returns>Uma string com a despedida</returns>
    Public ReadOnly Property Farewell(Optional Language As String = "pt") As String
        Get
            Return DateTime.Now.ToGreetingFarewell(Language, True)
        End Get
    End Property

    ''' <summary>
    ''' Returna uma lista dupla com os meses
    ''' </summary>
    ''' <param name="ValueType">Apresentação dos meses no valor</param>
    '''<param name="TextType">Apresentação dos meses no texto</param>

    Public ReadOnly Property Months(Optional TextType As TypeOfFill = TypeOfFill.LongName, Optional ValueType As TypeOfFill = TypeOfFill.Number) As List(Of KeyValuePair(Of String, String))
        Get
            Months = New List(Of KeyValuePair(Of String, String))
            For i = 1 To 12
                Dim key As String = ""
                Dim value As String = ""
                Select Case TextType
                    Case TypeOfFill.LongName
                        key = ToLongMonthName(i)
                    Case TypeOfFill.ShortName
                        key = ToShortMonthName(i)
                    Case Else
                        key = i
                End Select
                Select Case ValueType
                    Case TypeOfFill.LongName
                        value = ToLongMonthName(i)
                    Case TypeOfFill.ShortName
                        value = ToShortMonthName(i)
                    Case Else
                        value = i
                End Select
                Months.Add(New KeyValuePair(Of String, String)(key, value))
            Next
            Return Months
        End Get
    End Property

    ''' <summary>
    ''' Returna uma lista dupla com os meses
    ''' </summary>
    ''' <param name="ValueType">Apresentação dos meses no valor</param>
    '''<param name="TextType">Apresentação dos meses no texto</param>

    Public ReadOnly Property WeekDays(Optional TextType As TypeOfFill = TypeOfFill.LongName, Optional ValueType As TypeOfFill = TypeOfFill.Number) As List(Of KeyValuePair(Of String, String))
        Get
            WeekDays = New List(Of KeyValuePair(Of String, String))
            For i = 1 To 7
                Dim key As String = ""
                Dim value As String = ""
                Select Case TextType
                    Case TypeOfFill.LongName
                        key = ToLongDayOfWeekName(i)
                    Case TypeOfFill.ShortName
                        key = ToShortDayOfWeekName(i)
                    Case Else
                        key = i
                End Select
                Select Case ValueType
                    Case TypeOfFill.LongName
                        value = ToLongDayOfWeekName(i)
                    Case TypeOfFill.ShortName
                        value = ToShortDayOfWeekName(i)
                    Case Else
                        value = i
                End Select
                WeekDays.Add(New KeyValuePair(Of String, String)(key, value))
            Next
            Return WeekDays
        End Get
    End Property

    ''' <summary>
    ''' Tipo de Apresentação dos Meses/Dias da Semana/Estado
    ''' </summary>

    Enum TypeOfFill

        ''' <summary>
        ''' Numerico
        ''' </summary>
        Number = 2

        ''' <summary>
        ''' Abreviado
        ''' </summary>

        ShortName = 1
        ''' <summary>
        ''' Completo
        ''' </summary>

        LongName = 0
    End Enum

    ''' <summary>
    ''' Elemento do calendário
    ''' </summary>
    Enum CalendarType
        Weekdays
        Months
    End Enum

    ''' <summary>
    ''' Calcula a porcentagem de diferenca entre duas datas de acordo com a data inicial especificada
    ''' </summary>
    ''' <param name="MidDate">  Data do meio a ser verificada (normalmente Now)</param>
    ''' <param name="StartDate">Data Inicial</param>
    ''' <param name="EndDate">  Data Final</param>
    ''' <returns></returns>
    <Extension()>
    Function CalculatePercent(MidDate As DateTime, StartDate As DateTime, EndDate As DateTime) As Decimal
        FixDateOrder(StartDate, EndDate)
        If MidDate < StartDate Then Return 0
        If MidDate > EndDate Then Return 100
        Return (MidDate - StartDate).Ticks * 100 / (EndDate - StartDate).Ticks
    End Function

End Module