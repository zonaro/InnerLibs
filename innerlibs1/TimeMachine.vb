Imports System.Collections.ObjectModel
Imports InnerLibs.LINQ
Namespace TimeMachine

    Public Class FortnightGroup(Of DataType)
        Inherits FortnightGroup

        Property DataCollection As New List(Of DataType)

        Property StartDateSelector As Func(Of DataType, DateTime)

        Property EndDateSelector As Func(Of DataType, DateTime) = Nothing


        Public Function GetGroupedData(Key As String) As IEnumerable(Of DataType)
            Dim lista As New List(Of DataType)
            For Each ii In DataCollection
                Dim data As Date = StartDateSelector(ii)
                Dim fim_data As Date = Nothing

                Dim q1 As Date = Me(Key).First
                Dim q2 As Date = Me(Key).Last

                If data.IsBetween(q1, q2, False) Then
                    lista.Add(ii)
                End If

                If EndDateSelector IsNot Nothing Then
                    fim_data = EndDateSelector(ii)
                    If fim_data.IsBetween(q1, q2, False) Then
                        lista.Add(ii)
                    End If
                End If

            Next
            Return lista.Distinct
        End Function


        Public Shared Function CreateFromDataGroup(Data As IEnumerable(Of DataType), StartDateSelector As Func(Of DataType, DateTime), EndDateSelector As Func(Of DataType, DateTime)) As FortnightGroup(Of DataType)

            Dim stdate1 As Date? = Nothing
            Dim stdate2 As Date? = Nothing
            Dim edate1 As Date? = Nothing
            Dim edate2 As Date? = Nothing

            If StartDateSelector Is Nothing Then
                Throw New NoNullAllowedException("StartDateSelector is Nothing")
            Else
                stdate1 = Data.OrderBy(StartDateSelector).Select(StartDateSelector).First
                stdate2 = Data.OrderBy(StartDateSelector).Select(StartDateSelector).Last
            End If

            If EndDateSelector IsNot Nothing Then
                edate1 = Data.OrderByDescending(EndDateSelector).Select(EndDateSelector).First
                edate2 = Data.OrderByDescending(EndDateSelector).Select(EndDateSelector).Last
            Else
                edate1 = Data.OrderByDescending(StartDateSelector).Select(StartDateSelector).First
                edate2 = Data.OrderByDescending(StartDateSelector).Select(StartDateSelector).Last
            End If

            Dim arrdata = {stdate1, stdate2, edate1, edate2}.Where(Function(x) x.HasValue).OrderBy(Function(x) x)

            Dim fort = CreateFromDateRange(arrdata.First, arrdata.Last)

            fort = CType(fort, FortnightGroup(Of DataType))

            CType(fort, FortnightGroup(Of DataType)).DataCollection = Data.ToList
            CType(fort, FortnightGroup(Of DataType)).StartDateSelector = StartDateSelector
            CType(fort, FortnightGroup(Of DataType)).EndDateSelector = EndDateSelector

            Return fort
        End Function

        Public Shared Shadows Function CreateFromDateRange(StartDate As DateTime, EndDate As DateTime) As FortnightGroup(Of DataType)
            FixDateOrder(StartDate, EndDate)
            Dim fortcount As Integer = 1
            Dim fort As New FortnightGroup(Of DataType)(StartDate, fortcount)
            While fort.EndDate < EndDate
                fort = New FortnightGroup(Of DataType)(StartDate, fortcount.Increment)
            End While
            Return fort
        End Function

        Sub New(Optional StartDate As DateTime = Nothing, Optional FortnightCount As Integer = 1)
            MyBase.New(StartDate, FortnightCount)
        End Sub

    End Class


    ''' <summary>
    ''' Lista de dias agrupados em quinzenas
    ''' </summary>
    Public Class FortnightGroup
        Inherits Dictionary(Of String, ReadOnlyCollection(Of DateTime))


        Public Shared Function PrintFortnightName(Key As String, Format As String) As String
            Dim dia = Key.Split("@")(0)
            Dim mes = Key.Split("@")(1).Split("-")(0)
            Dim ano = Key.Split("@")(1).Split("-")(1)
            Return New Date(ano, mes, dia).ToString(Format)
        End Function


        ''' <summary>
        ''' Retorna a data inicial do periodo
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property StartDate As DateTime
            Get
                Return Me.First.Value.First
            End Get
        End Property

        ''' <summary>
        ''' Retorna a ultima data do periodo
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property EndDate As DateTime
            Get
                Return Me.Last.Value.Last
            End Get
        End Property

        ''' <summary>
        ''' Retorna uma lista com todos os dias entre as quinzenas
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property AllDays As IEnumerable(Of Date)
            Get
                Return Me.Values.SelectMany(Function(x) x.Select(Function(d) d))
            End Get
        End Property

        ''' <summary>
        ''' Instancia um novo <see cref="FortnightGroup"/> a partir de uma data e um numero de quinzenas
        ''' </summary>
        ''' <param name="StartDate"></param>
        ''' <param name="FortnightCount"></param>
        Sub New(Optional StartDate As DateTime = Nothing, Optional FortnightCount As Integer = 1)
            StartDate = If(IsNothing(StartDate), Now, StartDate)
            StartDate = New Date(StartDate.Year, StartDate.Month, If(StartDate.Day > 15, 16, 1), StartDate.Hour, StartDate.Minute, StartDate.Second, StartDate.Millisecond, StartDate.Kind)

            Dim EndDate = StartDate

            For index = 1 To FortnightCount.SetMinValue(1)
                Dim q As String
                EndDate = EndDate.AddDays(1)
                If EndDate.Day <= 15 Then
                    EndDate = New Date(EndDate.Year, EndDate.Month, 15, EndDate.Hour, EndDate.Minute, EndDate.Second, EndDate.Millisecond, EndDate.Kind)
                    q = New Date(EndDate.Year, EndDate.Month, 1).ToString("d@MM-yyyy")
                Else
                    EndDate = EndDate.GetLastDayOfMonth
                    q = New Date(EndDate.Year, EndDate.Month, 2).ToString("d@MM-yyyy")
                End If
                Me.Add(q, New ReadOnlyCollection(Of Date)(Calendars.GetBetween(StartDate, EndDate)))
                StartDate = EndDate.AddDays(1)
            Next
        End Sub

        Public Shared Function CreateFromDateRange(StartDate As DateTime, EndDate As DateTime) As FortnightGroup
            FixDateOrder(StartDate, EndDate)
            Dim fortcount As Integer = 1
            Dim fort As New FortnightGroup(StartDate, fortcount)
            While fort.EndDate < EndDate
                fort = New FortnightGroup(StartDate, fortcount.Increment)
            End While
            Return fort
        End Function




    End Class

    ''' <summary>
    ''' Classe para comapração entre 2 Datas com possibilidade de validação de dias Relevantes
    ''' </summary>
    Public Class TimeFlow

        ''' <summary>
        ''' Inicia uma instancia de TimeFlow
        ''' </summary>
        ''' <param name="StartDate">Data inicial</param>
        ''' <param name="EndDate">Data Final (data mais recente)</param>
        ''' <param name="RelevantDaysOfWeek">Lista de dias da semana que são relevantes (dias letivos)</param>
        Public Sub New(StartDate As DateTime, EndDate As DateTime, ParamArray RelevantDaysOfWeek() As DayOfWeek)
            FixDateOrder(StartDate, EndDate)
            Dim CurDate As DateTime = StartDate
            Dim years As Integer = 0
            Dim months As Integer = 0
            Dim days As Integer = 0
            Dim _phase As Phase = Phase.Years

            If RelevantDaysOfWeek.Count = 0 Then
                RelevantDaysOfWeek = {0, 1, 2, 3, 4, 5, 6}
            End If

            While CurDate <= EndDate
                If RelevantDaysOfWeek.Contains(CurDate.DayOfWeek) Then
                    Me.RelevantDays.Add(CurDate)
                Else
                    Me.NonRelevantDays.Add(CurDate)
                End If
                Me.AllDays.Add(CurDate)
                CurDate = CurDate.AddDays(1)
            End While

            CurDate = StartDate

            While _phase <> Phase.Done
                Select Case _phase
                    Case Phase.Years
                        If CurDate.AddYears(years + 1) > EndDate Then
                            _phase = Phase.Months
                            CurDate = CurDate.AddYears(years)
                        Else
                            years.Increment
                        End If
                        Exit Select
                    Case Phase.Months
                        If CurDate.AddMonths(months + 1) > EndDate Then
                            _phase = Phase.Days
                            CurDate = CurDate.AddMonths(months)
                        Else
                            months.Increment
                        End If
                        Exit Select
                    Case Phase.Days
                        If CurDate.AddDays(days + 1) > EndDate Then
                            CurDate = CurDate.AddDays(days)
                            Dim timespan = EndDate - CurDate
                            Me.Years = years
                            Me.Months = months
                            Me.Days = days
                            Me.Hours = timespan.Hours
                            Me.Minutes = timespan.Minutes
                            Me.Seconds = timespan.Seconds
                            Me.Milliseconds = timespan.Milliseconds
                            Me.StartDate = StartDate
                            Me.EndDate = EndDate
                            Me.RelevantDaysOfWeek = RelevantDaysOfWeek.ToList
                            _phase = Phase.Done
                        Else
                            days.Increment
                        End If
                        Exit Select
                End Select
            End While
        End Sub

        ''' <summary>
        ''' Inicia uma instancia de TimeFlow a partir de um TimeSpan
        ''' </summary>
        ''' <param name="Span">Intervalo de tempo</param>
        Public Sub New(Span As TimeSpan)
            Me.New(DateTime.MinValue, Span)
        End Sub

        ''' <summary>
        ''' Inicia uma instancia de TimeFlow a partir de uma data inicial e um TimeSpan
        ''' </summary>
        ''' <param name="StartDate">Data Inicial</param>
        ''' <param name="Span">Intervalo de tempo</param>
        Public Sub New(StartDate As DateTime, Span As TimeSpan)
            Me.New(StartDate, StartDate.Add(Span))
        End Sub

        ''' <summary>
        ''' Data Inicial
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property StartDate As Date

        ''' <summary>
        ''' Data Final
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EndDate As Date

        ''' <summary>
        ''' Dias Relevantes entre as datas Inicial e Final
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RelevantDays As New List(Of Date)

        ''' <summary>
        ''' Todos os dias entre as datas Inicial e Final
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AllDays As New List(Of Date)

        ''' <summary>
        ''' Dias não relevantes entre as datas Inicial e Final
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NonRelevantDays As New List(Of Date)

        ''' <summary>
        ''' Dias da semana relevantes
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RelevantDaysOfWeek As New List(Of DayOfWeek)

        ''' <summary>
        ''' Dias da semana não relevantes
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NonRelevantDaysOfWeek As List(Of DayOfWeek)
            Get
                Dim lista = {DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday}.ToList()
                Dim lista2 = New List(Of DayOfWeek)
                lista2.AddRange(lista)
                For Each item As DayOfWeek In lista
                    If RelevantDaysOfWeek.Contains(item) Then
                        lista2.Remove(item)
                    End If
                Next
                Return lista2
            End Get
        End Property

        ''' <summary>
        ''' Numero de Anos
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Years As Integer

        ''' <summary>
        ''' Numero de Meses
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Months As Integer

        ''' <summary>
        ''' Numero de Dias
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Days As Integer

        ''' <summary>
        ''' Numero de Horas
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Hours As Integer

        ''' <summary>
        ''' Numero de Minutos
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Minutes As Integer

        ''' <summary>
        ''' Numero de Segundos
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Seconds As Integer

        ''' <summary>
        ''' Numero de milisegundos
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Milliseconds As Integer

        ''' <summary>
        ''' Retorna uma String no formato "X anos, Y meses e Z dias"
        ''' </summary>
        ''' <param name="FullString">Parametro que indica se as horas, minutos e segundos devem ser apresentados caso o tempo seja maior que 1 dia</param>
        ''' <returns>string</returns>
        Public Function ToTimeElapsedString(Optional FullString As Boolean = True) As String
            Dim ano = "", mes = "", dia = "", horas = "", minutos = "", segundos = ""
            ano = (If(Me.Years > 0, (If(Me.Years = 1, Me.Years & " ano ", Me.Years & " anos")), ""))
            mes = (If(Me.Months > 0, (If(Me.Months = 1, Me.Months & " mês ", Me.Months & " meses ")), ""))
            dia = (If(Me.Days > 0, (If(Me.Days = 1, Me.Days & " dia ", Me.Days & " dias ")), ""))
            If FullString Or Me.Days < 1 Then
                horas = (If(Me.Hours > 0, (If(Me.Hours = 1, Me.Hours & " hora ", Me.Hours & " horas ")), ""))
                minutos = (If(Me.Minutes > 0, (If(Me.Minutes = 1, Me.Minutes & " minuto ", Me.Minutes & " minutos ")), ""))
                segundos = (If(Me.Seconds > 0, (If(Me.Seconds = 1, Me.Seconds & " segundo ", Me.Seconds & " segundos ")), ""))
            End If

            ano.AppendIf(",", ano.IsNotBlank And (mes.IsNotBlank Or dia.IsNotBlank Or horas.IsNotBlank Or minutos.IsNotBlank Or segundos.IsNotBlank))
            mes.AppendIf(",", mes.IsNotBlank And (dia.IsNotBlank Or horas.IsNotBlank Or minutos.IsNotBlank Or segundos.IsNotBlank))
            dia.AppendIf(",", dia.IsNotBlank And (horas.IsNotBlank Or minutos.IsNotBlank Or segundos.IsNotBlank))
            horas.AppendIf(",", horas.IsNotBlank And (minutos.IsNotBlank Or segundos.IsNotBlank))
            minutos.AppendIf(",", minutos.IsNotBlank And (segundos.IsNotBlank))

            Dim current As String = ano & " " & mes & " " & dia & " " & horas & " " & minutos & " " & segundos
            If current.Contains(",") Then
                current = current.Insert(current.LastIndexOf(","), " e ")
                current = current.Remove(current.LastIndexOf(","), 1)
            End If
            Return current.AdjustWhiteSpaces
        End Function

        ''' <summary>
        ''' Retorna uma string com a quantidade de itens e o tempo de produção
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.ToTimeElapsedString(True)
        End Function

        Private Enum Phase
            Years
            Months
            Days
            Done
        End Enum

    End Class

    ''' <summary>
    ''' Classe base para calculo de demandas
    ''' </summary>
    Public Class TimeDemand

        ''' <summary>
        ''' Domingo
        ''' </summary>
        ''' <returns></returns>
        Property Sunday As New Day

        ''' <summary>
        ''' Segunda-Feira
        ''' </summary>
        ''' <returns></returns>
        Property Monday As New Day

        ''' <summary>
        ''' Terça-Feira
        ''' </summary>
        ''' <returns></returns>
        Property Tuesday As New Day

        ''' <summary>
        ''' Quarta-Feira
        ''' </summary>
        ''' <returns></returns>
        Property Wednesday As New Day

        ''' <summary>
        ''' Quinta-Feira
        ''' </summary>
        ''' <returns></returns>
        Property Thursday As New Day

        ''' <summary>
        ''' Sexta-Feira
        ''' </summary>
        ''' <returns></returns>
        Property Friday As New Day

        ''' <summary>
        ''' Sábado
        ''' </summary>
        ''' <returns></returns>
        Property Saturday As New Day

        ''' <summary>
        ''' item da Produção
        ''' </summary>
        ''' <returns></returns>
        Property Item As New Item

        ''' <summary>
        ''' Data Inicial da produção
        ''' </summary>
        ''' <returns></returns>
        Property StartDate As DateTime

        ''' <summary>
        ''' Empurra a data para dentro da proxima hora disponivel dentro jornada de trabalho
        ''' </summary>
        ''' <param name="[Date]">Data a ser Verificada</param>
        Public Sub PushDateIntoJourney(ByRef [Date] As Date)
            While [Date].TimeOfDay > JourneyEndHour([Date]).TimeOfDay Or [Date].TimeOfDay < JourneyStartHour([Date]).TimeOfDay Or Not RelevantDaysOfWeek.Contains([Date].DayOfWeek) Or [Date].IsBetween(LunchStartHour([Date]), LunchEndHour([Date]))
                [Date] = [Date].AddSeconds(1)
            End While
        End Sub

        ''' <summary>
        ''' Data de encerramento da produção
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property EndDate As DateTime
            Get
                Me.PushDateIntoJourney(StartDate)

                Dim FinalDate = StartDate.Add(Item.ProductionTime)

                Dim t As New TimeFlow(StartDate, FinalDate, Me.RelevantDaysOfWeek.ToArray)

                For Each dia In t.RelevantDays
                    If Not dia.Date = FinalDate.Date Then
                        FinalDate = FinalDate.AddHours(24 - TotalTime(dia).TotalHours)
                    Else
                        If FinalDate.TimeOfDay > LunchStartHour(dia).TimeOfDay Then
                            FinalDate = FinalDate.Add(LunchTime(dia))
                        End If
                    End If

                    If FinalDate.IsBetween(LunchStartHour(dia), LunchEndHour(dia)) Then
                        FinalDate = FinalDate.Add(LunchEndHour(dia).TimeOfDay - dia.TimeOfDay)
                    End If
                Next

                For Each feriado In HoliDays.ClearTime
                    If Not t.NonRelevantDays.ClearTime.Contains(feriado) Then
                        t.NonRelevantDays.Add(feriado)
                    End If
                Next

                FinalDate = FinalDate.AddDays(t.NonRelevantDays.Count)

                Dim lasthours = New TimeSpan()
                If FinalDate.TimeOfDay > JourneyEndHour(FinalDate).TimeOfDay Then
                    lasthours = FinalDate.TimeOfDay - JourneyEndHour(FinalDate).TimeOfDay
                End If

                PushDateIntoJourney(FinalDate)

                FinalDate = FinalDate.Add(lasthours)

                Return FinalDate
            End Get
        End Property

        ''' <summary>
        ''' Inicia uma nova Demanda com as propriedades do item
        ''' </summary>
        ''' <param name="StartDate">Data Inicial da produção</param>
        ''' <param name="Time">Tempo do item</param>
        ''' <param name="Quantity">Quantidade de itens</param>
        Public Sub New(StartDate As DateTime, Time As TimeSpan, Optional Quantity As Integer = 1, Optional SingularItem As String = "Item", Optional MultipleItem As String = "Itens")
            Me.StartDate = StartDate
            Me.Item.Quantity = Quantity
            Me.Item.Time = Time
            Me.Item.SingularItem = SingularItem
            Me.Item.MultipleItem = MultipleItem
        End Sub

        ''' <summary>
        ''' Cria uma demanda após a demanda atual com as mesmas caracteristicas
        ''' </summary>
        ''' <param name="SafeTime">Tempo adicionado entre uma demanda e outra</param>
        ''' <returns></returns>
        Public Function CloneAndQueue(Optional SafeTime As TimeSpan = Nothing) As TimeDemand
            If SafeTime = Nothing Then
                SafeTime = New TimeSpan(0)
            End If
            Dim enda = Me.EndDate
            Return New TimeDemand(enda.Add(SafeTime), Me.Item.Time, Me.Item.Quantity, Me.Item.SingularItem, Me.Item.MultipleItem)
        End Function

        ''' <summary>
        ''' Dias especificos da semana entre as datas inicial e final da demanda
        ''' </summary>
        ''' <param name="DaysOfWeek">Dias da semana</param>
        ''' <returns></returns>
        Public ReadOnly Property WorkDays(ParamArray DaysOfWeek() As DayOfWeek) As List(Of Date)
            Get
                Return Calendars.GetBetween(StartDate, EndDate, DaysOfWeek.ToArray)
            End Get
        End Property

        ''' <summary>
        ''' Dias relevantes (letivos) entre as datas inicial e final
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RelevantDays As List(Of Date)
            Get
                Dim dias = WorkDays(RelevantDaysOfWeek.ToArray).ClearTime
                For Each feriado In HoliDays.ClearTime
                    If dias.Contains(feriado) Then
                        dias.Remove(feriado)
                    End If
                Next
                Return dias
            End Get
        End Property

        ''' <summary>
        ''' Dias não relevantes (nao letivos e feriados) entre as datas inicial e final
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NonRelevantDays As List(Of Date)
            Get
                Dim dias = WorkDays.ClearTime
                For Each d In RelevantDays()
                    dias.Remove(d)
                Next
                Return dias
            End Get
        End Property

        ''' <summary>
        ''' Retorna um TimeFlow desta demanda
        ''' </summary>
        ''' <returns></returns>
        Public Function BuildTimeFlow() As TimeFlow
            Return New TimeFlow(Me.StartDate, Me.EndDate, RelevantDaysOfWeek.ToArray)
        End Function

        ''' <summary>
        ''' Retorna uma string representado a quantidade de itens e o tempo gasto com a produção
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.Item.ToString & " - " & Me.ToTimeElapsedString
        End Function

        ''' <summary>
        ''' Retorna uma String no formato "X anos, Y meses e Z dias"
        ''' </summary>
        ''' <param name="FullString">Parametro que indica se as horas, minutos e segundos devem ser apresentados caso o tempo seja maior que 1 dia</param>
        ''' <returns></returns>
        Function ToTimeElapsedString(Optional FullString As Boolean = True) As String
            Dim data_final = Me.EndDate
            Dim data_inicial = Me.StartDate
            Return GetDifference(data_inicial, data_final).ToTimeElapsedString(FullString)
        End Function

        ''' <summary>
        ''' inicia uma nova demanda
        ''' </summary>
        Public Sub New()
            StartDate = Now
        End Sub

        ''' <summary>
        ''' Retorna a porcentagem em relacao a posição de uma data entre a data inicial (0%) e final (100%)
        ''' </summary>
        ''' <param name="MidDate"></param>
        ''' <returns></returns>
        Public Function GetPercentCompletion(MidDate As Date) As Decimal
            Return Calendars.CalculatePercent(MidDate, Me.StartDate, Me.EndDate)
        End Function

        ''' <summary>
        ''' Dias da semana relevantes
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property RelevantDaysOfWeek As IEnumerable(Of DayOfWeek)
            Get
                Dim dias As New List(Of DayOfWeek)
                If Me.Sunday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Sunday)
                If Me.Monday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Monday)
                If Me.Tuesday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Tuesday)
                If Me.Wednesday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Wednesday)
                If Me.Thursday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Thursday)
                If Me.Friday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Friday)
                If Me.Saturday.JourneyTime.TotalHours > 0 Then dias.Add(DayOfWeek.Saturday)
                If dias.Count = 0 Then
                    dias.AddRange({0, 1, 2, 3, 4, 5, 6})
                End If
                Return dias.ToArray
            End Get

        End Property

        ''' <summary>
        ''' Feriados, pontos facuultativos e/ou datas especificas consideradas não relevantes
        ''' </summary>
        ''' <returns></returns>
        Public Property HoliDays As New List(Of Date)

        ''' <summary>
        ''' Dias da semana não relevantes
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property NonRelevantDaysOfWeek As IEnumerable(Of DayOfWeek)
            Get
                Dim s = {DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday}.ToList()
                For Each d In RelevantDaysOfWeek
                    s.Remove(d)
                Next
                Return s
            End Get
        End Property

        ''' <summary>
        ''' Retorna a jornada de trabalho + hora de almoço de uma data de acordo com as configuracoes desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function TotalTime([Date] As DateTime) As TimeSpan
            Select Case [Date].DayOfWeek
                Case DayOfWeek.Sunday
                    Return Me.Sunday.TotalTime
                Case DayOfWeek.Monday
                    Return Me.Monday.TotalTime
                Case DayOfWeek.Tuesday
                    Return Me.Tuesday.TotalTime
                Case DayOfWeek.Wednesday
                    Return Me.Wednesday.TotalTime
                Case DayOfWeek.Thursday
                    Return Me.Thursday.TotalTime
                Case DayOfWeek.Friday
                    Return Me.Friday.TotalTime
                Case DayOfWeek.Saturday
                    Return Me.Saturday.TotalTime
            End Select
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna o tempo da jornada de trabalho de uma data de acordo com as configuracoes desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function JourneyTime([Date] As DateTime) As TimeSpan
            Select Case [Date].DayOfWeek
                Case DayOfWeek.Sunday
                    Return Me.Sunday.JourneyTime
                Case DayOfWeek.Monday
                    Return Me.Monday.JourneyTime
                Case DayOfWeek.Tuesday
                    Return Me.Tuesday.JourneyTime
                Case DayOfWeek.Wednesday
                    Return Me.Wednesday.JourneyTime
                Case DayOfWeek.Thursday
                    Return Me.Thursday.JourneyTime
                Case DayOfWeek.Friday
                    Return Me.Friday.JourneyTime
                Case DayOfWeek.Saturday
                    Return Me.Saturday.JourneyTime
            End Select
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna o tempo de almoço de uma data de acordo com as configuracoes desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function LunchTime([Date] As DateTime) As TimeSpan
            Select Case [Date].DayOfWeek
                Case DayOfWeek.Sunday
                    Return Me.Sunday.LunchTime
                Case DayOfWeek.Monday
                    Return Me.Monday.LunchTime
                Case DayOfWeek.Tuesday
                    Return Me.Tuesday.LunchTime
                Case DayOfWeek.Wednesday
                    Return Me.Wednesday.LunchTime
                Case DayOfWeek.Thursday
                    Return Me.Thursday.LunchTime
                Case DayOfWeek.Friday
                    Return Me.Friday.LunchTime
                Case DayOfWeek.Saturday
                    Return Me.Saturday.LunchTime
            End Select
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna a hora inicial da jornada de uma data de acordo com as configuracoes desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function JourneyStartHour([Date] As DateTime) As Date
            Select Case [Date].DayOfWeek
                Case DayOfWeek.Sunday
                    Return Me.Sunday.StartHour
                Case DayOfWeek.Monday
                    Return Me.Monday.StartHour
                Case DayOfWeek.Tuesday
                    Return Me.Tuesday.StartHour
                Case DayOfWeek.Wednesday
                    Return Me.Wednesday.StartHour
                Case DayOfWeek.Thursday
                    Return Me.Thursday.StartHour
                Case DayOfWeek.Friday
                    Return Me.Friday.StartHour
                Case DayOfWeek.Saturday
                    Return Me.Saturday.StartHour
            End Select
            Return DateTime.MinValue
        End Function

        ''' <summary>
        ''' Retorna a hora final da jornada de uma data acordo com as configuracoes desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function JourneyEndHour([Date] As DateTime) As Date
            Select Case [Date].DayOfWeek
                Case DayOfWeek.Sunday
                    Return Me.Sunday.EndHour
                Case DayOfWeek.Monday
                    Return Me.Monday.EndHour
                Case DayOfWeek.Tuesday
                    Return Me.Tuesday.EndHour
                Case DayOfWeek.Wednesday
                    Return Me.Wednesday.EndHour
                Case DayOfWeek.Thursday
                    Return Me.Thursday.EndHour
                Case DayOfWeek.Friday
                    Return Me.Friday.EndHour
                Case DayOfWeek.Saturday
                    Return Me.Saturday.EndHour
            End Select
            Return DateTime.MinValue
        End Function

        ''' <summary>
        ''' Retorno a hora de inicio do almoço de uma data de acordo com as configurações desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function LunchStartHour([Date] As DateTime) As Date
            Select Case [Date].DayOfWeek
                Case DayOfWeek.Sunday
                    Return Me.Sunday.LunchHour
                Case DayOfWeek.Monday
                    Return Me.Monday.LunchHour
                Case DayOfWeek.Tuesday
                    Return Me.Tuesday.LunchHour
                Case DayOfWeek.Wednesday
                    Return Me.Wednesday.LunchHour
                Case DayOfWeek.Thursday
                    Return Me.Thursday.LunchHour
                Case DayOfWeek.Friday
                    Return Me.Friday.LunchHour
                Case DayOfWeek.Saturday
                    Return Me.Saturday.LunchHour
            End Select
            Return DateTime.MinValue
        End Function

        ''' <summary>
        ''' Retorna a hora de termino do almoço de uma data de acordo com as configurações desta demanda
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Function LunchEndHour([Date] As DateTime) As Date
            Return LunchStartHour([Date]).Add(LunchTime([Date]))
        End Function

        ''' <summary>
        ''' Intervalo de horas trabalhadas entre as datas de inicio e fim desta demanda
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property WorkTime As TimeSpan
            Get
                Return GetWorkTimeBetween(Me.StartDate, Me.EndDate)
            End Get
        End Property

        ''' <summary>
        ''' Retorna o intervalo de horas trabalhadas entre duas datas baseado nas confuguracoes desta demanda
        ''' </summary>
        ''' <returns></returns>
        Public Function GetWorkTimeBetween(StartDate As DateTime, EndDate As DateTime) As TimeSpan
            FixDateOrder(StartDate, EndDate)
            Dim dias = Me.RelevantDaysOfWeek
            Dim totalhoras = 0
            Dim cal As New TimeFlow(StartDate, EndDate, dias.ToArray)
            For Each dia In cal.RelevantDays
                Select Case dia.DayOfWeek
                    Case DayOfWeek.Sunday
                        totalhoras = totalhoras + Me.Sunday.JourneyTime.TotalHours
                    Case DayOfWeek.Monday
                        totalhoras = totalhoras + Me.Monday.JourneyTime.TotalHours
                    Case DayOfWeek.Tuesday
                        totalhoras = totalhoras + Me.Tuesday.JourneyTime.TotalHours
                    Case DayOfWeek.Wednesday
                        totalhoras = totalhoras + Me.Wednesday.JourneyTime.TotalHours
                    Case DayOfWeek.Thursday
                        totalhoras = totalhoras + Me.Thursday.JourneyTime.TotalHours
                    Case DayOfWeek.Friday
                        totalhoras = totalhoras + Me.Friday.JourneyTime.TotalHours
                    Case DayOfWeek.Saturday
                        totalhoras = totalhoras + Me.Saturday.JourneyTime.TotalHours
                End Select
            Next
            Return New TimeSpan(totalhoras, 0, 0)
        End Function

    End Class

    ''' <summary>
    ''' Dia de Uma Demanda
    ''' </summary>
    Public Class Day

        ''' <summary>
        ''' Inicia uma instancia de dia letivo
        ''' </summary>
        Public Sub New()
            StartHour = DateTime.MinValue
            LunchHour = DateTime.MinValue.AddHours(12)
        End Sub

        ''' <summary>
        ''' Inicia uma instancia de dia letivo
        ''' </summary>
        ''' <param name="StartHour">Hora Incial</param>
        ''' <param name="Journey">Jornada de trabalho</param>
        Public Sub New(StartHour As DateTime, Journey As TimeSpan, Optional LunchHour As DateTime = Nothing, Optional LunchTime As TimeSpan = Nothing)
            Me.SetJourney(StartHour, Journey, LunchHour, LunchTime)
        End Sub

        ''' <summary>
        ''' Jornada de Trabalho/Produção
        ''' </summary>
        ''' <returns></returns>
        Public Property JourneyTime As TimeSpan = New TimeSpan(0, 0, 0)

        ''' <summary>
        ''' Hora de Almoço
        ''' </summary>
        ''' <returns></returns>
        Public Property LunchTime As TimeSpan = New TimeSpan(0, 0, 0)

        ''' <summary>
        ''' Jornada + hora de Almoço
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TotalTime As TimeSpan
            Get
                Return JourneyTime + LunchTime
            End Get
        End Property

        ''' <summary>
        ''' Hora inicial da jornada
        ''' </summary>
        ''' <returns></returns>
        Public Property StartHour As DateTime
            Get
                Return s
            End Get
            Set(value As DateTime)
                s = DateTime.MinValue.Add(New TimeSpan(value.TimeOfDay.Ticks))
            End Set
        End Property

        Private s As DateTime

        ''' <summary>
        ''' Hora de almoco
        ''' </summary>
        ''' <returns></returns>
        Public Property LunchHour As DateTime
            Get
                Return a
            End Get
            Set(value As DateTime)
                a = DateTime.MinValue.Add(New TimeSpan(value.TimeOfDay.Ticks))
            End Set
        End Property

        Private a As DateTime

        ''' <summary>
        ''' Hora que se encerra a jornada (inclui hora de almoço)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EndHour As DateTime
            Get
                Return StartHour.Add(TotalTime)
            End Get
        End Property

        ''' <summary>
        ''' Define a hora inicial e a jornada de trabalho deste dia
        ''' </summary>
        ''' <param name="StartHour"></param>
        ''' <param name="Journey"></param>
        ''' <param name="LunchTime">Horas de Almoço</param>
        Public Sub SetJourney(StartHour As DateTime, Journey As TimeSpan, Optional LunchHour As DateTime = Nothing, Optional LunchTime As TimeSpan = Nothing)
            Me.StartHour = StartHour
            Me.JourneyTime = Journey
            Me.LunchHour = If(LunchHour = Nothing, Date.MinValue.Date.AddHours(12), LunchHour)
            Me.LunchTime = If(LunchTime = Nothing, New TimeSpan(0, 0, 0), LunchTime)
        End Sub

    End Class

    ''' <summary>
    ''' Item de Uma demanda
    ''' </summary>
    Public Class Item

        ''' <summary>
        ''' Quantidade de itens
        ''' </summary>
        ''' <returns></returns>
        Property Quantity As Integer
            Get
                Return q
            End Get
            Set(value As Integer)
                q = value.SetMinValue(1)
            End Set
        End Property

        Dim q As Integer = 1

        ''' <summary>
        ''' Tempo de produção de 1 item
        ''' </summary>
        ''' <returns></returns>
        Property Time As TimeSpan = New TimeSpan(0, 1, 0)

        ''' <summary>
        ''' Tempo totald e produção de todos os itens
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ProductionTime As TimeSpan
            Get
                Return New TimeSpan(Quantity * Time.Ticks)
            End Get
        End Property

        ''' <summary>
        ''' String que representa o item quando sua quantidade é 1
        ''' </summary>
        ''' <returns></returns>
        Property SingularItem As String = "Item"

        ''' <summary>
        ''' string que representa o item quando sua quantidade é maior que 1
        ''' </summary>
        ''' <returns></returns>
        Property MultipleItem As String = "Itens"

        ''' <summary>
        ''' Retorna uma string que representa a quantidade do item
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Quantity & " " & If(Quantity = 1, SingularItem, MultipleItem)
        End Function

    End Class

End Namespace