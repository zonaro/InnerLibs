
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser
    <AttributeUsage(AttributeTargets.[Class])> _
    Public Class PseudoClassNameAttribute

        Inherits Attribute

        Public Property FunctionName() As String

            Get
                Return m_FunctionName
            End Get
            Private Set
                m_FunctionName = Value

            End Set

        End Property

        Private m_FunctionName As String


        Public Sub New(name As String)

            Me.FunctionName = name
        End Sub
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
