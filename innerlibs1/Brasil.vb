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
        Public Property StateCode As String

        ''' <summary>
        ''' Nome do estado
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Região do Estado
        ''' </summary>
        ''' <returns></returns>
        Public Property Region As String

        ''' <summary>
        ''' Lista de cidades do estado
        ''' </summary>
        ''' <returns></returns>
        Public Property Cities As IEnumerable(Of String)

        ''' <summary>
        ''' Tipo de string representativa do estado (sigla ou nome)
        ''' </summary>
        Public Enum StateString
            Name
            StateCode
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
            Me.StateCode = StateCode
            Me.Name = Brasil.GetNameOf(StateCode)
            Me.Cities = Brasil.GetCitiesOf(StateCode).ToList()
        End Sub

        ''' <summary>
        ''' Retorna a String correspondente ao estado
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return StateCode
        End Function

        ''' <summary>
        ''' Retorna a String correspondente ao estado
        ''' </summary>
        ''' <param name="Type">Tipo de String (Sigla ou Nome)</param>
        ''' <returns></returns>
        Public Overloads Function ToString(Optional Type As StateString = StateString.StateCode) As String
            Return If(Type = StateString.Name, Name, StateCode)
        End Function

    End Class

    ''' <summary>
    ''' Objeto para manipular cidades e estados do Brasil
    ''' </summary>
    Public NotInheritable Class Brasil

        ''' <summary>
        ''' Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property States As IEnumerable(Of State)
            Get
                Return CreateList()
            End Get
        End Property

        Private Shared l As List(Of State) = New List(Of State)

        Private Shared Function CreateList() As List(Of State)
            If Not l.Any() Then
                Dim r = New StreamReader([Assembly].GetExecutingAssembly().GetManifestResourceStream("InnerLibs.brasil.xml"))
                Dim s = r.ReadToEnd().ToString
                Dim doc = New XmlDocument()
                doc.LoadXml(s)
                For Each node As XmlNode In doc("brasil").ChildNodes
                    Dim estado = New State
                    estado.StateCode = node("StateCode").InnerText
                    estado.Name = node("Name").InnerText
                    estado.Region = node("Region").InnerText
                    Dim lc = New List(Of String)
                    For Each subnode As XmlNode In node("Cities").ChildNodes
                        lc.Add(subnode.InnerText)
                    Next
                    estado.Cities = lc.AsEnumerable()
                    l.Add(estado)
                Next
            End If
            Return l
        End Function

        ''' <summary>
        ''' Retorna as Regiões dos estados brasileiros
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Regions As IEnumerable(Of String)
            Get
                Return States.Select(Function(x) x.Region).Distinct()
            End Get
        End Property

        ''' <summary>
        ''' Retorna todas as Cidades dos estados brasileiros
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Cities As IEnumerable(Of String)
            Get
                Return States.SelectMany(Function(x) x.Cities)
            End Get
        End Property

        Public Shared Function FindStateByCity(CityName As String) As IEnumerable(Of State)
            Return States.Where(Function(x) x.Cities.Any(Function(c) c.ToSlugCase() = CityName.ToSlugCase()))
        End Function

        ''' <summary>
        ''' Retorna os estados de uma região
        ''' </summary>
        ''' <param name="Region"></param>
        ''' <returns></returns>
        Public Shared Function GetStatesOf(Type As State.StateString, Optional Region As String = "") As IEnumerable(Of String)
            Return GetStatesOf(Region).Select(Function(x) x.ToString(Type))
        End Function

        ''' <summary>
        ''' Retorna os estados de uma região
        ''' </summary>
        ''' <param name="Region"></param>
        ''' <returns></returns>
        Public Shared Function GetStatesOf(Optional Region As String = "") As IEnumerable(Of State)
            Return States.Where(Function(x) x.Region.ToSlugCase = Region.ToSlugCase.AdjustBlankSpaces() OrElse Region.IsBlank())
        End Function

        ''' <summary>
        ''' Retorna as cidades de um estado a partir do nome ou sigla do estado
        ''' </summary>
        ''' <param name="NameOrStateCode">Nome ou sigla do estado</param>
        ''' <returns></returns>
        Public Shared Function GetCitiesOf(NameOrStateCode As String) As IEnumerable(Of String)
            Return If(GetState(NameOrStateCode)?.Cities, New List(Of String)).AsEnumerable()
        End Function

        ''' <summary>
        ''' Retorna uma lista contendo os nomes ou siglas dos estados do Brasil
        ''' </summary>
        ''' <param name="Type">Tipo de retorno (sigla ou nome)</param>
        ''' <returns></returns>
        Public Shared Function GetStateList(Optional Type As State.StateString = State.StateString.Name) As List(Of String)
            Return States.Select(Function(x) x.ToString(Type))
        End Function

        ''' <summary>
        ''' Retorna o nome do estado a partir da sigla
        ''' </summary>
        ''' <param name="NameOrStateCode"></param>
        ''' <returns></returns>
        Public Shared Function GetNameOf(NameOrStateCode As String) As String
            Return GetState(NameOrStateCode)?.Name
        End Function

        ''' <summary>
        ''' Retorna a Sigla a partir de um nome de estado
        ''' </summary>
        ''' <param name="NameOrStateCode"></param>
        ''' <returns></returns>
        Public Shared Function GetStateCodeOf(NameOrStateCode As String) As String
            Return GetState(NameOrStateCode)?.StateCode
        End Function

        ''' <summary>
        ''' Retorna a região a partir de um nome de estado
        ''' </summary>
        ''' <param name="NameOrStateCode"></param>
        ''' <returns></returns>
        Public Shared Function GetRegionOf(NameOrStateCode As String) As String
            Return GetState(NameOrStateCode)?.Region
        End Function

        ''' <summary>
        ''' Retorna a as informações do estado a partir de um nome de estado ou sua sigla
        ''' </summary>
        ''' <param name="NameOrStateCode">Nome ou UF</param>
        ''' <returns></returns>
        Public Shared Function GetState(NameOrStateCode As String) As State
            NameOrStateCode = NameOrStateCode.AdjustBlankSpaces().ToSlugCase
            Return Brasil.States.FirstOrDefault(Function(x) x.Name.ToSlugCase = NameOrStateCode OrElse x.StateCode = NameOrStateCode)
        End Function

    End Class

End Namespace