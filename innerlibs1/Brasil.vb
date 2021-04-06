Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports InnerLibs

Namespace Locations

    ''' <summary>
    ''' Objeto que representa um estado do Brasil e seus respectivos detalhes
    ''' </summary>
    Public Class State

        ''' <summary>
        ''' Sigla do estado
        ''' </summary>
        ''' <returns></returns>
        Public Property Acronym As String

        ''' <summary>
        ''' Nome do estado
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Lista de cidades do estado
        ''' </summary>
        ''' <returns></returns>
        Public Property Cities As New List(Of String)

        ''' <summary>
        ''' Tipo de string representativa do estado (sigla ou nome)
        ''' </summary>
        Public Enum StateString
            Name
            Acronym
        End Enum

        ''' <summary>
        ''' inicializa um estado vazio
        ''' </summary>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' Inicializa um objeto Estado a partir de uma sigla
        ''' </summary>
        ''' <param name="StateCode"></param>
        Public Sub New(StateCode As String)
            Me.Acronym = StateCode
            Me.Name = Brasil.GetNameOf(StateCode)
            Me.Cities = Brasil.GetCitiesOf(StateCode)
        End Sub

        ''' <summary>
        ''' Retorna a String correspondente ao estado
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Acronym
        End Function

        ''' <summary>
        ''' Retorna a String correspondente ao estado
        ''' </summary>
        ''' <param name="Type">Tipo de String (Sigla ou Nome)</param>
        ''' <returns></returns>
        Public Overloads Function ToString(Optional Type As StateString = StateString.Acronym) As String
            Return If(Type = StateString.Name, Name, Acronym)
        End Function

    End Class

    Public NotInheritable Class Celebration

        ReadOnly Property [Date](Optional Year As Integer? = Nothing) As DateTime
            Get
                Return New Date(If(Year, Now.Year), Month, Day)
            End Get
        End Property

        ReadOnly Property Description As String
            Get
                Return _fact
            End Get
        End Property

        Private _fact As String

        ReadOnly Property Day As Integer
            Get
                Return _day
            End Get
        End Property

        Private _day As Integer

        ReadOnly Property Month As Integer
            Get
                Return _month
            End Get
        End Property

        Private _month As Integer

        Friend Sub New(Dia As Integer, Mes As Integer, Fato As String)
            _day = Dia
            _month = Mes
            _fact = Fato
        End Sub

    End Class

    ''' <summary>
    ''' Objeto para manipular cidades e estados do Brasil
    ''' </summary>
    Public NotInheritable Class Brasil

        ''' <summary>
        ''' Retorna uma lista com todas as datas comemorativas do Brasil
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Celebrations As List(Of Celebration)
            Get
                Dim l As New List(Of Celebration)
                For Each item In [Assembly].GetExecutingAssembly().GetResourceFileText("InnerLibs.facts.json").ParseJSON()
                    l.Add(New Celebration(item("Day"), item("Month"), item("Fact")))
                Next
                Return l
            End Get
        End Property

        ''' <summary>
        ''' Retorna todas as comemoracoes de uma data
        ''' </summary>
        ''' <param name="[Date]"></param>
        ''' <returns></returns>
        Public Shared Function GetCelebrationByDate([Date] As Date) As Celebration()
            Return Celebrations.Where(Function(x) x.Day = [Date].Day And x.Month = [Date].Month).OrderBy(Function(x) x.Date([Date].Year)).ToArray
        End Function

        ''' <summary>
        ''' Retorna todas as comemoracoes de um mês
        ''' </summary>
        ''' <param name="Month">Numero do Mês</param>
        ''' <returns></returns>
        Public Shared Function GetCelebrationByMonth(Month As Integer) As String()
            Return Celebrations.Where(Function(x) x.Month = Month).Select(Function(y) y.Description).ToArray
        End Function

        ''' <summary>
        ''' Retorna todas as comemoracoes entre 2 datas
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetCelebrationBetween(FirstDate As Date, SecondDate As Date) As Celebration()
            Dim l = Calendars.GetBetween(FirstDate, SecondDate)
            Dim ss As New List(Of Celebration)
            For Each i In l
                ss.AddRange(GetCelebrationByDate(i))
            Next
            Return ss.Distinct.ToArray
        End Function

        ''' <summary>
        ''' Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property States As List(Of State)
            Get
                States = New List(Of State)
                Dim r = New StreamReader([Assembly].GetExecutingAssembly().GetManifestResourceStream("InnerLibs.brasil.xml"))
                Dim s = r.ReadToEnd().ToString
                Dim doc = New XmlDocument()
                doc.LoadXml(s)
                For Each node As XmlNode In doc("brasil").ChildNodes
                    Dim estado = New State
                    estado.Acronym = node("Acronym").InnerText
                    estado.Name = node("Name").InnerText
                    For Each subnode As XmlNode In node("Cities").ChildNodes
                        estado.Cities.Add(subnode.InnerText)
                    Next
                    States.Add(estado)
                Next
                Return States
                'Return s.ParseJSON(Of List(Of State))
            End Get
        End Property

        ''' <summary>
        ''' Retorna as cidades de um estado a partir do nome ou sigla do estado
        ''' </summary>
        ''' <param name="NameOrStateCode">Nome ou sigla do estado</param>
        ''' <returns></returns>
        Public Shared Function GetCitiesOf(Optional NameOrStateCode As String = "") As List(Of String)
            Dim cities As New List(Of String)
            For Each estado As State In Brasil.States
                If estado.Acronym = NameOrStateCode Or estado.Name = NameOrStateCode Or NameOrStateCode.IsBlank() Then
                    cities.AddRange(estado.Cities)
                End If
            Next
            Return cities
        End Function

        ''' <summary>
        ''' Retorna uma lista contendo os nomes ou siglas dos estados do Brasil
        ''' </summary>
        ''' <param name="Type">Tipo de retorno (sigla ou nome)</param>
        ''' <returns></returns>
        Public Shared Function GetStateList(Optional Type As State.StateString = State.StateString.Name) As List(Of String)
            Dim StateCodes As New List(Of String)
            For Each es As State In States
                StateCodes.Add(es.ToString(Type))
            Next
            Return StateCodes
        End Function

        ''' <summary>
        ''' Retorna o nome do estado a partir da sigla
        ''' </summary>
        ''' <param name="StateCode"></param>
        ''' <returns></returns>
        Public Shared Function GetNameOf(StateCode As String) As String
            Dim name = ""
            For Each estado As State In Brasil.States
                If estado.Acronym = StateCode Then
                    name = estado.Name
                End If
            Next
            Return name
        End Function

        ''' <summary>
        ''' Retorna a Sigla a partir de um nome de estado
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Public Shared Function GetAcronymOf(Name As String) As String
            Dim StateCode = ""
            For Each estado As State In Brasil.States
                If estado.Name = Name Then
                    StateCode = estado.Acronym
                End If
            Next
            Return StateCode
        End Function

    End Class


End Namespace
