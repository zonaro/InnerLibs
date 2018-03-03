
Imports System.Collections.Specialized
Imports InnerLibs.HtmlParser

Namespace Select2Data

    ''' <summary>
    ''' Resultado, renderiza como option
    ''' </summary>
    Public Class Result
        Inherits Select2ResultType

        ''' <summary>
        ''' Campo utilizado como value
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String
            Get
                Return If(_id, text)
            End Get
            Set(value As String)
                _id = value
            End Set
        End Property

        Private _id As String = Nothing

        ''' <summary>
        ''' atributo selected do option
        ''' </summary>
        ''' <returns></returns>
        Public Property selected As Boolean = False
        ''' <summary>
        ''' Atributo disabled do option
        ''' </summary>
        ''' <returns></returns>
        Public Property disabled As Boolean = False

        Public Overrides Function ToHtmlElement() As HtmlElement
            Dim d As New HtmlElement("option")
            If selected Then
                d.Attributes.Add("selected")
            End If
            If disabled Then
                d.Attributes.Add("disabled")
            End If
            d.InnerHTML = text
            d.Attributes.Add("value", id)
            If otherdata IsNot Nothing Then
                For Each entry In otherdata.Keys
                    If entry.IsNotBlank Then
                        d.Attributes.Add("data-" & entry, otherdata(entry))
                    End If
                Next
            End If

            Return d
        End Function
    End Class

    Public Class Pagination
        Public Property more As Boolean = False
    End Class

    ''' <summary>
    ''' Grupo de resultado, rendereiza como optgroup
    ''' </summary>
    Public Class Group
        Inherits Select2ResultType
        ''' <summary>
        ''' Options deste optgroup
        ''' </summary>
        ''' <returns></returns>
        Public Property children As IEnumerable(Of Result)


        Public Overrides Function ToHtmlElement() As HtmlElement
            Dim d As New HtmlElement("optgroup")
            d.Attribute("label") = text
            If children IsNot Nothing Then
                d.Nodes.AddRange(children.Select(Function(x) x.ToHtmlElement))
            End If
            If otherdata IsNot Nothing Then
                For Each entry In otherdata.Keys
                    If entry.IsNotBlank Then
                        d.Attributes.Add("data-" & entry, otherdata(entry))
                    End If
                Next
            End If
            Return d
        End Function


    End Class

    Public MustInherit Class Select2ResultType
        ''' <summary>
        ''' Texto deste optgroup ou option
        ''' </summary>
        ''' <returns></returns>
        Public Property text As String
        ''' <summary>
        ''' informacao extra anexada a este optgroup ou option
        ''' </summary>
        ''' <returns></returns>
        Public Property otherdata As AditionalData

        Public MustOverride Function ToHtmlElement() As HtmlElement

    End Class

    Public Class AditionalData
        Inherits Dictionary(Of String, Object)

        Public Shared Widening Operator CType(s As String) As AditionalData
            Dim e As New AditionalData
            Dim n = System.Web.HttpUtility.ParseQueryString(s).ToDictionary
            For Each el In n.Keys
                e.Add(el, n(el))
            Next
            Return e
        End Operator
    End Class

    ''' <summary>
    ''' Classe base para serializaçao de json para um Select2
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    Public Class Select2Results(Of Type As Select2ResultType)
        Inherits Select2Results
        Public Shadows Property results As IEnumerable(Of Type)
    End Class

    Public MustInherit Class Select2Results
        Public Property results As IEnumerable(Of Select2ResultType)
        Public Property pagination As New Pagination
        Public Property otherdata As New AditionalData


        Public Function ToHtmlElement(Optional Name As String = "") As HtmlElement
            Dim d As New HtmlElement("select")
            If Name.IsNotBlank Then
                d.Attribute("name") = Name
            End If
            If otherdata IsNot Nothing Then
                For Each entry In otherdata.Keys
                    If entry.IsNotBlank Then
                        d.Attributes.Add("data-" & entry, otherdata(entry))
                    End If
                Next
            End If
            If results IsNot Nothing Then
                d.Nodes.AddRange(results.Select(Function(x) x.ToHtmlElement))
            End If
            Return d
        End Function

        ''' <summary>
        ''' Serializa um Json para o Select2
        ''' </summary>
        ''' <returns></returns>
        Public Function ToJSON() As String
            Return Json.SerializeJSON(Me)
        End Function


        ''' <summary>
        ''' retorna a string json deste select2
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.ToJson
        End Function

    End Class


End Namespace

