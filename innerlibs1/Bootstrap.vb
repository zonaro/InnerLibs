Imports System.Runtime.CompilerServices
Imports System.Web.UI.HtmlControls
Imports System.Xml
Namespace Bootstrap

    Public Class Bootstrap

        ''' <summary>
        ''' Classe que representa a aparência de um elemento de forma geral
        ''' </summary>
        Public Enum ElementType
            [Default]
            Primary
            Success
            Info
            Warning
            Danger
        End Enum

        ''' <summary>
        ''' Classe mãe dos elementos de Bootstrap
        ''' </summary>
        Public MustInherit Class BootstrapElement
            Property OuterHtml As String
            Property InnerHtml As String

            ''' <summary>
            ''' Retorna uma classe baseada no tipo do element criado
            ''' </summary>
            ''' <param name="ElementType">Elemento do bootstrap</param>
            ''' <returns></returns>
            Public Function CreateObjectClass(ElementType As ElementType) As String
                Dim classname = Me.GetType().Name.ToLower() & " " & Me.GetType().Name.ToLower().Replace("_", "-")
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

#Region "Elements"
        ''' <summary>
        ''' Elemento Panel do Bootstrap
        ''' </summary>
        Public Class Panel
            Inherits BootstrapElement

            Public Property Type As ElementType = ElementType.Default
            Public Property Title As String
            Public Property Content As String
            Public Property Footer As String

            Public Sub New(Title As String, Optional Footer As String = Nothing)
                Me.Footer = Footer
                Me.Title = Title
                Me.ToString()
            End Sub

            ''' <summary>
            ''' Transforma um elemento em um Panel do Bootstrap
            ''' </summary>
            ''' <param name="Control">Elemento que vai sofrer a transformação</param>
            Public Sub TransformElement(ByRef Control As HtmlGenericControl)
                Control.Attributes("class") = Me.CreateObjectClass(Me.Type) & " " & Control.Attributes("class")
                Dim OldContent = Me.Content
                Me.Content = Control.InnerHtml
                Me.ToString()
                Control.InnerHtml = Me.InnerHtml
                Me.Content = OldContent
                Me.ToString()
            End Sub

            Public Overloads Function ToString(Optional ID As String = "", Optional [Class] As String = "", Optional Attributes As String = "") As String
                OuterHtml = "<div id='" & ID & "' class='" & Me.CreateObjectClass(Type) & " " & [Class] & "' " & Attributes & ">"
                InnerHtml = ""
                InnerHtml.Append("<div class='panel-heading'>")
                InnerHtml.Append("<h3 class='panel-title'>" & Title & "</h3>")
                InnerHtml.Append("</div>")
                InnerHtml.Append("<div class='panel-body'>")
                InnerHtml.Append(Content)
                InnerHtml.Append("</div>")
                If Footer.IsNotBlank() Then
                    InnerHtml.Append("<div class='panel-footer'>" & Footer & "</div>")
                End If
                OuterHtml.Append(InnerHtml)
                OuterHtml.Append("</div>")
                Return OuterHtml
            End Function
        End Class

        ''' <summary>
        ''' Elemento Alert do Bootstrap
        ''' </summary>
        Public Class Alert
            Inherits BootstrapElement

            Public Property Type As ElementType = ElementType.Danger
            Public Property Content As String
            Public Property Dismissible As Boolean

            Public Sub New(Optional Dismissible As Boolean = True)
                Me.Dismissible = Dismissible
                Me.ToString()
            End Sub

            ''' <summary>
            ''' Transforma um elemento em um Alert do Bootstrap
            ''' </summary>
            ''' <param name="Control">Elemento que vai sofrer a transformação</param>
            Public Sub TransformElement(ByRef Control As HtmlGenericControl)
                Control.Attributes("class") = Me.CreateObjectClass(Me.Type) & " " & Control.Attributes("class")
                Dim OldContent = Me.Content
                Me.Content = Control.InnerHtml
                Me.ToString()
                Control.InnerHtml = Me.InnerHtml
                Me.Content = OldContent
                Me.ToString()
            End Sub

            Public Overloads Function ToString(Optional ID As String = "", Optional [Class] As String = "", Optional Attributes As String = "") As String
                OuterHtml = "<div id='" & ID & "' class='" & Me.CreateObjectClass(Type) & " " & [Class] & "' role='alert' " & Attributes & ">"
                InnerHtml = ""
                If Dismissible Then
                    InnerHtml.Append("<button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>")
                End If
                InnerHtml.Append(Content)
                OuterHtml.Append(InnerHtml)
                OuterHtml.Append("</div>")
                Return OuterHtml
            End Function

        End Class

        Public Class ProgressBar
            Inherits BootstrapElement
            Property Value As Decimal
            Property Content As String
            Property Type As ElementType
            Property Striped As Boolean = False
            Property Animated As Boolean = False
            Property Label As Boolean = True
            Property Min As Decimal = 0
            Property Max As Decimal = 100

            Public Sub New(Value As Integer, Optional Min As Decimal = 0, Optional Max As Decimal = 100)
                Me.Value = Value
                Me.Min = Min
                Me.Max = Max
                Me.Content = Content.IsNull(Value, False)
            End Sub

            ''' <summary>
            ''' Transforma um elemento em um Panel do Bootstrap
            ''' </summary>
            ''' <param name="Control">Elemento que vai sofrer a transformação</param>
            Public Sub TransformElement(ByRef Control As HtmlGenericControl)
                Control.Attributes("class") = "progress " & Control.Attributes("class")
                Me.ToString()
                Control.InnerHtml = Me.InnerHtml
            End Sub

            Public Sub Stack(Bar As ProgressBar)
                Me.ToString()
                Bar.ToString()
                Debug.WriteLine(Bar.InnerHtml)
                Me.InnerHtml.Append(Bar.InnerHtml)
                Me.ToString()
            End Sub

            Public Overloads Function ToString(Optional ID As String = "", Optional [Class] As String = "", Optional Attributes As String = "") As String
                OuterHtml = "<div class='progress'>"
                InnerHtml = "<div id='" & ID & "' class='progress-bar " & Me.CreateObjectClass(Type) & " " & If(Striped, "progress-bar-striped", "") & " " & If(Animated, "active", "") & " ' role='progressbar' aria-valuenow='" & Me.Value & "' aria-valuemin='" & Min & "' aria-valuemax='" & Max & "'  style='width: " & Mathematic.CalculatePercent(Me.Value, Max) & "%;'><span class='" & If(Label, "", "sr-only") & "'>" & Content & "</span></div>"
                OuterHtml.Append(InnerHtml)

                OuterHtml.Append("</div>")
                Return OuterHtml
            End Function

        End Class

#End Region

    End Class

End Namespace