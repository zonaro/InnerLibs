Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Windows.Forms
Imports InnerLibs.TimeMachine

''' <summary>
''' Modulo para manipulação de calendário
''' </summary>
''' <remarks></remarks>
Public Module Calendars
    ''' <summary>
    ''' Verifica se a Data de hoje é um aniversário
    ''' </summary>
    ''' <param name="BirthDate">Data de nascimento</param>
    ''' <param name="UseTomorrow">Verifica se o aniversario é Amanha</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsAnniversary(BirthDate As Date, Optional UseTomorrow As Boolean = False) As Boolean
        Return If(UseTomorrow, (BirthDate.Day & "/" & BirthDate.Month) = (Tomorrow.Day & "/" & Tomorrow.Month), (BirthDate.Day & "/" & BirthDate.Month) = (DateTime.Today.Day & "/" & DateTime.Today.Month))
    End Function

    ''' <summary>
    ''' COnverte um datetime para o formato de string do SQL server ou Mysql
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToSQLDateString([Date] As DateTime) As String
        Return [Date].Year & "-" & [Date].Month & "-" & [Date].Day & " " & [Date].Hour & ":" & [Date].Minute & ":" & [Date].Second
    End Function

    ''' <summary>
    ''' COnverte uma string dd/mm/aaaa para o formato de string do SQL server ou Mysql
    ''' </summary>
    ''' <param name="[Date]">Data</param>
    ''' <returns></returns>
    <Extension>
    Function ToSQLDateString([Date] As String) As String
        Return [Date].Split("/").Reverse.ToArray().Join("-")
    End Function

    ''' <summary>
    ''' Retorna uma TimeMachine com a diferença entre 2 Datas
    ''' </summary>
    ''' <param name="InitialDate"></param>
    ''' <param name="SecondDate"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetDifference(InitialDate As DateTime, SecondDate As DateTime) As TimeFlow
        FixDateOrder(InitialDate, SecondDate)
        Return New TimeFlow(InitialDate, SecondDate)
    End Function

    ''' <summary>
    ''' Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate sempre seja uma data menos que a EndDate, prevenindo que o calculo entre 2 datas de em um TimeSpan negativo
    ''' </summary>
    ''' <param name="StartDate">Data Inicial</param>
    ''' <param name="EndDate">Data Final</param>
    Public Sub FixDateOrder(ByRef StartDate As DateTime, ByRef EndDate As DateTime)
        If StartDate > EndDate Then
            Dim temp = StartDate
            StartDate = EndDate
            EndDate = temp
        End If
    End Sub

    ''' <summary>
    ''' Verifica se uma data se encontra entre 2 datas
    ''' </summary>
    ''' <param name="MidDate">Data</param>
    ''' <param name="StartDate">Data Inicial</param>
    ''' <param name="EndDate">Data final</param>
    ''' <param name="IgnoreTime">Indica se o tempo deve ser ignorado na comparação</param>
    ''' <returns></returns>
    <Extension()> Public Function IsBetween(MidDate As DateTime, StartDate As DateTime, EndDate As DateTime, Optional IgnoreTime As Boolean = False) As Boolean
        FixDateOrder(StartDate, EndDate)
        If IgnoreTime Then
            Return StartDate.Date < MidDate.Date And MidDate.Date < EndDate.Date
        Else
            Return StartDate < MidDate And MidDate < EndDate
        End If
    End Function

    ''' <summary>
    ''' Retorna uma lista com as datas de dias especificos da semana entre 2 datas
    ''' </summary>
    ''' <param name="StartDate">Data inicial</param>
    ''' <param name="EndDate">data Final</param>
    ''' <param name="Days">Dias da semana</param>
    ''' <returns></returns>
    Public Function GetBetween(StartDate As DateTime, EndDate As DateTime, ParamArray Days() As DayOfWeek) As List(Of Date)
        FixDateOrder(StartDate, EndDate)
        If Days.Length = 0 Then Days = {0, 1, 2, 3, 4, 5, 6}
        Dim curdate = StartDate.Date
        Dim l As New List(Of Date)
        Do
            If Days.Contains(curdate.DayOfWeek) Then
                l.Add(curdate.Date)
            End If
            curdate = curdate.AddDays(1)
        Loop While curdate <= EndDate.Date
        Return l.ClearTime
    End Function

    ''' <summary>
    ''' Remove o tempo de todas as datas de uma lista (retorna uma nova lista)
    ''' </summary>
    ''' <param name="List">Lista que será alterada</param>
    <Extension()> Function ClearTime(List As List(Of Date)) As List(Of Date)
        Dim h As New List(Of Date)
        For Each i In List
            h.Add(i.Date)
        Next
        Return h
    End Function

    ''' <summary>
    ''' Retorna uma String no formato "W dias, X horas, Y minutos e Z segundos"
    ''' </summary>
    ''' <param name="TimeElapsed">TimeSpan com o intervalo</param>
    ''' <returns>string</returns>
    <Extension()>
    Public Function ToTimeElapsedString(TimeElapsed As TimeSpan) As String
        Dim dia As String = (If(TimeElapsed.Days > 0, (If(TimeElapsed.Days = 1, TimeElapsed.Days & " dia ", TimeElapsed.Days & " dias ")), ""))
        Dim horas As String = (If(TimeElapsed.Hours > 0, (If(TimeElapsed.Hours = 1, TimeElapsed.Hours & " hora ", TimeElapsed.Hours & " horas ")), ""))
        Dim minutos As String = (If(TimeElapsed.Minutes > 0, (If(TimeElapsed.Minutes = 1, TimeElapsed.Minutes & " minuto ", TimeElapsed.Minutes & " minutos ")), ""))
        Dim segundos As String = (If(TimeElapsed.Seconds > 0, (If(TimeElapsed.Seconds = 1, TimeElapsed.Seconds & " segundo ", TimeElapsed.Seconds & " segundos ")), ""))

        dia.AppendIf(",", dia.IsNotBlank And (horas.IsNotBlank Or minutos.IsNotBlank Or segundos.IsNotBlank))
        horas.AppendIf(",", horas.IsNotBlank And (minutos.IsNotBlank Or segundos.IsNotBlank))
        minutos.AppendIf(",", minutos.IsNotBlank And (segundos.IsNotBlank))
        Dim current As String = dia & " " & horas & " " & minutos & " " & segundos
        If current.Contains(",") Then
            current = current.Insert(current.LastIndexOf(","), " e ")
            current = current.Remove(current.LastIndexOf(","), 1)
        End If
        Return current.AdjustWhiteSpaces
    End Function

    ''' <summary>
    ''' Retorna uma String baseado no numero do Mês Ex.: 1 -> Janeiro
    ''' </summary>
    ''' <param name="MonthNumber">Numero do Mês</param>
    ''' <returns>String com nome do Mês</returns>

    <Extension()>
    Function ToLongMonthName(MonthNumber As Integer) As String
        Select Case MonthNumber
            Case 1
                Return "Janeiro"
            Case 2
                Return "Fevereiro"
            Case 3
                Return "Março"
            Case 4
                Return "Abril"
            Case 5
                Return "Maio"
            Case 6
                Return "Junho"
            Case 7
                Return "Julho"
            Case 8
                Return "Agosto"
            Case 9
                Return "Setembro"
            Case 10
                Return "Outubro"
            Case 11
                Return "Novembro"
            Case 12
                Return "Dezembro"
            Case Else
                Return "Mês Inválido"
        End Select

    End Function

    ''' <summary>
    ''' Retorna uma String curta baseado no numero do Mês Ex.: 1 -> Jan
    ''' </summary>
    ''' <param name="MonthNumber">Numero do Mês</param>
    ''' <returns>String com nome curto do Mês</returns>

    <Extension()>
    Public Function ToShortMonthName(MonthNumber As Integer) As String
        Return ToLongMonthName(MonthNumber).GetFirstChars(3)
    End Function
    ''' <summary>
    ''' Retorna uma String  baseado no numero do Dia da Semana Ex.: 2 -> Segunda-Feira
    ''' </summary>
    ''' <param name="DayNumber">Numero do Dia</param>
    ''' <returns>String com nome do Dia</returns>

    <Extension()>
    Function ToLongDayOfWeekName(DayNumber As Integer) As String

        Select Case DayNumber

            Case 1
                Return "Domingo"
            Case 2
                Return "Segunda-Feira"
            Case 3
                Return "Terça-Feira"
            Case 4
                Return "Quarta-Feira"
            Case 5
                Return "Quinta-Feira"
            Case 6
                Return "Sexta-Feira"
            Case 7
                Return "Sábado"
            Case Else
                Return "Dia Inválido"
        End Select
    End Function
    ''' <summary>
    ''' Retorna uma String  baseado no numero do Dia da Semana Ex.: 2 -> Seg
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

    Public ReadOnly Property Tomorrow() As DateTime = DateTime.Now.AddDays(1)

    ''' <summary>
    ''' Retorna a data de ontem
    ''' </summary>
    ''' <returns>Data de ontem</returns>

    Public ReadOnly Property Yesterday() As DateTime = DateTime.Now.AddDays(-1)

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
    ''' <param name="Time">Horario</param>
    ''' <param name="Language">Idioma da saudação (pt, en, es)</param>
    ''' <returns>Uma string com a despedida</returns>
    <Extension>
    Public Function ToFarewell(Time As DateTime, Optional Language As String = "pt") As String
        Return Time.ToGreetingFarewell(Language, True)
    End Function

    ''' <summary>
    ''' Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite)
    ''' </summary>
    ''' <param name="Time">Horario</param>
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
            Return Now.ToGreetingFarewell(Language)
        End Get
    End Property
    ''' <summary>
    ''' Retorna uma despedida
    ''' </summary>
    ''' <param name="Language">Idioma da despedida (pt, en, es)</param>
    ''' <returns>Uma string com a despedida</returns>
    Public ReadOnly Property Farewell(Optional Language As String = "pt") As String
        Get
            Return Now.ToGreetingFarewell(Language, True)
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
    ''' Preenche um HtmlSelect com MESES ou DIAS DA SEMANA
    ''' </summary>
    ''' <param name="Box">Select HTML</param>
    ''' <param name="ValueType">Apresentação dos meses no valor</param>
    '''<param name="TextType">Apresentação dos meses no texto</param>
    <Extension> Public Sub FillWith(Box As UI.HtmlControls.HtmlSelect, CalendarType As CalendarType, Optional TextType As TypeOfFill = TypeOfFill.LongName, Optional ValueType As TypeOfFill = TypeOfFill.Number)
        For Each item In If(CalendarType = CalendarType.Months, Months(TextType, ValueType), WeekDays(TextType, ValueType))
            Box.Items.Add(New ListItem(item.Key, item.Value))
        Next
    End Sub

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
    ''' <param name="MidDate">Data do meio a ser verificada (normalmente Now)</param>
    ''' <param name="StartDate">Data Inicial</param>
    ''' <param name="EndDate">Data Final</param>
    ''' <returns></returns>
    <Extension()>
    Function CalculatePercent(MidDate As DateTime, StartDate As DateTime, EndDate As DateTime) As Decimal
        If StartDate > EndDate Then
            Dim temp = StartDate
            StartDate = EndDate
            EndDate = temp
        End If
        If MidDate < StartDate Then Return 0
        If MidDate > EndDate Then Return 100
        Return (MidDate - StartDate).Ticks * 100 / (EndDate - StartDate).Ticks
    End Function

End Module