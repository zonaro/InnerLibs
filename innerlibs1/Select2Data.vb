
Imports System.Collections.Specialized
Imports InnerLibs.HtmlParser

Namespace Select2Data

    ''' <summary>
    ''' Resultado, renderiza como option
    ''' </summary>
    Public Class Result
        Inherits Select2ResultType

        Sub New()

        End Sub

        Sub New(Text As String, Optional Id As String = Nothing)
            Me.text = Text
            Me.id = Id
        End Sub

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

        ''' <summary>
        ''' Retorna um ListItem deste objeto
        ''' </summary>
        ''' <returns></returns>
        Function ToListItem() As System.Web.UI.WebControls.ListItem
            Dim l As New System.Web.UI.WebControls.ListItem(Me.text, Me.id)
            If otherdata IsNot Nothing Then
                For Each entry In otherdata.Keys
                    If entry.IsNotBlank Then
                        l.Attributes.Add("data-" & entry, otherdata(entry))
                    End If
                Next
            End If
            l.Selected = Me.selected
            l.Enabled = Not disabled
            Return l
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
        Public Property children As New List(Of Result)

        ''' <summary>
        ''' id do optgroup
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String = ""

        Public Sub New()
        End Sub

        Public Sub New(Text As String)
            Me.text = Text
            Me.id = Text.ToSlugCase
        End Sub

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

    ''' <summary>
    ''' Classe Base para resultados do Select2. Deve ser herdada
    ''' </summary>
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

        ''' <summary>
        ''' Converte este objeto para <see cref="HtmlElement"/>
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function ToHtmlElement() As HtmlElement

    End Class

    ''' <summary>
    ''' Informações adicionais a este resultado
    ''' </summary>
    Public Class AditionalData
        Inherits Dictionary(Of String, Object)

        ''' <summary>
        ''' Converte uma querystring para AditionalData
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Shared Widening Operator CType(s As String) As AditionalData
            Dim e As New AditionalData
            Dim n = System.Web.HttpUtility.ParseQueryString(s).ToDictionary
            For Each el In n.Keys
                e(el) = n(el)
            Next
            Return e
        End Operator
    End Class

    ''' <summary>
    ''' Classe base para serializaçao de json para um Select2
    ''' </summary>
    ''' <typeparam name="Type"></typeparam> 
    Public Class Select2Results(Of Type As Select2ResultType)

        ''' <summary>
        ''' Lista de options ou optgroups atrelados a este conjunto de resultados
        ''' </summary>
        ''' <returns></returns>
        Public Property results As New List(Of Type)
        ''' <summary>
        ''' Objeto paginação do Select2
        ''' </summary>
        ''' <returns></returns>
        Public Property pagination As New Pagination
        ''' <summary>
        ''' Informações adicionais (formato QueryString)
        ''' </summary>
        ''' <returns></returns>
        Public Property otherdata As New AditionalData

        ''' <summary>
        ''' Converte este objeto para um <see cref="HtmlElement"/> (Select com options)
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
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
            Return OldJsonSerializer.SerializeJSON(Me)
        End Function


        ''' <summary>
        ''' retorna a string json deste select2
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.ToJSON
        End Function

    End Class


End Namespace

