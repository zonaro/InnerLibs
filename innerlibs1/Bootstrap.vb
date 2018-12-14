Imports System.Runtime.CompilerServices
Imports System.Web.UI.HtmlControls
Imports System.Xml
Namespace HtmlParser.Bootstrap


    Public Class BootstrapBuilder
        Public Enum ElementType
            [Default]
            Primary
            Success
            Info
            Warning
            Danger
        End Enum


        Public ReadOnly Property Panel(Title As String, Optional Footer As String = Nothing, Optional Content As String = "", Optional Type As ElementType = ElementType.Default) As HtmlElement
            Get
                Dim el As New HtmlElement("div", "")
                el.Class.Add(CreateObjectClass("panel", Type))
                el.InnerHTML.Append("<div class='panel-heading'>")
                el.InnerHTML.Append("<h3 class='panel-title'>" & Title & "</h3>")
                el.InnerHTML.Append("</div>")
                el.InnerHTML.Append("<div class='panel-body'>")
                el.InnerHTML.Append(Content)
                el.InnerHTML.Append("</div>")
                If Footer.IsNotBlank() Then
                    el.InnerHTML.Append("<div class='panel-footer'>" & Footer & "</div>")
                End If
                Return el
            End Get
        End Property

        Public ReadOnly Property Alert(Content As String, Optional Dismissible As Boolean = False, Optional Type As ElementType = ElementType.Default) As HtmlElement
            Get
                Dim el As New HtmlElement("div", "")
                el.Class.Add(CreateObjectClass("alert", Type))
                el.Attribute("role") = "alert"
                If Dismissible Then
                    el.InnerHTML.Append("<button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>")
                End If
                el.InnerHTML.Append(Content)
                Return el
            End Get
        End Property

        Public ReadOnly Property ProgressBar(ProgressValue As Decimal, Min As Decimal, Max As Decimal, Content As String, Optional Label As Boolean = False, Optional Striped As Boolean = True, Optional Animated As Boolean = False, Optional Type As ElementType = ElementType.Default) As HtmlElement
            Get
                Dim el As New HtmlElement("div", "")
                el.Class.Add("progress")
                el.InnerHTML = "<div class='progress-bar " & CreateObjectClass("progress-bar", Type) & " " & If(Striped, "progress-bar-striped", "") & " " & If(Animated, "active", "") & " ' role='progressbar' aria-valuenow='" & ProgressValue & "' aria-valuemin='" & Min & "' aria-valuemax='" & Max & "'  style='width: " & Mathematic.CalculatePercent(ProgressValue, Max) & "%;'><span class='" & If(Label, "", "sr-only") & "'>" & Content & "</span></div>"
                Return el
            End Get
        End Property

        Public ReadOnly Property Row() As HtmlElement
            Get
                Dim el As New HtmlElement("div")
                el.Class.Add("row")
                Return el
            End Get
        End Property

        Public ReadOnly Property Row(ID As String) As HtmlElement
            Get
                Dim el As New HtmlElement("div")
                el.ID = ID
                el.Class.Add("row")
                Return el
            End Get
        End Property

        Public ReadOnly Property Row(ID As String, ParamArray Columns As HtmlElement()) As HtmlElement
            Get
                Dim el As New HtmlElement("div")
                el.ID = ID
                el.Class.Add("row")
                el.Nodes.Add(Columns)
                Return el
            End Get
        End Property

        Public ReadOnly Property Row(ParamArray Columns As HtmlElement()) As HtmlElement
            Get
                Dim el As New HtmlElement("div")
                el.Class.Add("row")
                el.Nodes.Add(Columns)
                Return el
            End Get
        End Property


        Public ReadOnly Property Container(ParamArray Rows As HtmlElement()) As HtmlElement
            Get
                Dim el As New HtmlElement("div")
                el.Class.Add("container")
                el.Nodes.Add(Rows)
                Return el
            End Get
        End Property


        ''' <summary>
        ''' Retorna uma classe baseada no tipo do element criado
        ''' </summary>
        ''' <param name="ElementType">Elemento do bootstrap</param>
        ''' <returns></returns>
        Public Function CreateObjectClass(ElementName As String, ElementType As ElementType) As String
            Dim classname = ElementName.ToLower() & " " & ElementName.Replace("_", "-")
            Select Case ElementType
                Case ElementType.Primary
                    Return classname & "-primary"
                Case ElementType.Success
                    Return classname & "-success"
                Case ElementType.Info
                    Return classname & "-info"
                Case ElementType.Warning
                    Return classname & "-warning"
                Case ElementType.Danger
                    Return classname & "-danger"
                Case Else
                    Return classname & "-default"
            End Select

        End Function

    End Class




End Namespace