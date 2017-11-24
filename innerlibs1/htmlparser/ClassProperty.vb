Imports System.ComponentModel

Namespace HtmlParser

    Public Class ClassList
        Inherits List(Of String)

        Private mElement As HtmlElement

        Friend Sub New(Element As HtmlElement)
            Me.mElement = Element
        End Sub

        Private Function CreateClassList() As List(Of String)
            Dim s = mElement.Attribute("class")
            Dim styledic As New List(Of String)
            If s.IsNotBlank Then
                For Each i In s.Split(" ")
                    styledic.Add(i.ToLower)
                Next
            End If
            Return styledic.Distinct.ToList
        End Function

        ''' <summary>
        ''' Gets or sets a value indicating if this element contains a specifc class
        ''' </summary>
        ''' <param name="ClassName">Name</param>
        ''' <returns></returns>
        Default Public Shadows Property Item(ClassName As String) As Boolean
            Get
                Return CreateClassList.ContainsAll(ClassName.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            End Get
            Set(value As Boolean)
                Dim styledic As List(Of String) = CreateClassList()
                If ClassName.IsNotBlank Then
                    For Each i As String In ClassName.Split(" ")
                        If value Then
                            styledic.Add(i)
                        Else
                            If styledic.Contains(i) Then
                                styledic.Remove(i)
                            End If
                        End If
                    Next
                End If
                Dim p = ""
                For Each k In styledic.Distinct
                    p.Append(k.ToLower & " ")
                Next
                mElement.Attribute("class") = p
            End Set
        End Property

        ''' <summary>
        ''' Count the class of element
        ''' </summary>
        ''' <returns></returns>
        Public Shadows ReadOnly Property Count As Integer
            Get
                Return CreateClassList.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Insert a class into specific index
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="ClassName"></param>
        Public Shadows Sub Insert(index As Integer, ClassName As String)
            Dim l = CreateClassList()
            l.Insert(index, ClassName)
            mElement.Attribute("class") = l.Join(" ")
        End Sub

        ''' <summary>
        ''' Remove the class with specific index
        ''' </summary>
        ''' <param name="index"></param>
        Public Shadows Sub RemoveAt(index As Integer)
            Dim l = CreateClassList()
            l.RemoveAt(index)
            mElement.Attribute("class") = l.Join(" ")
        End Sub

        ''' <summary>
        ''' Add a class to element
        ''' </summary>
        ''' <param name="ClassName"></param>
        Public Shadows Function Add(ClassName As String) As HtmlElement
            Me.Item(ClassName) = True
            Return Me.mElement
        End Function

        ''' <summary>
        ''' Remove the class attribute from element
        ''' </summary>
        Public Shadows Sub Clear()
            mElement.Attributes.Remove("class")
        End Sub

        Public Shadows Sub CopyTo(array() As String, arrayIndex As Integer)
            CreateClassList.CopyTo(array, arrayIndex)
        End Sub

        ''' <summary>
        ''' Gets the class position index in element
        ''' </summary>
        ''' <param name="ClassName"></param>
        ''' <returns></returns>
        Public Shadows Function IndexOf(ClassName As String) As Integer
            Return CreateClassList.IndexOf(ClassName)
        End Function

        ''' <summary>
        ''' Check if element coitains all the classes
        ''' </summary>
        ''' <param name="ClassName"></param>
        ''' <returns></returns>
        Public Shadows Function Contains(ParamArray ClassName As String()) As Boolean
            Return CreateClassList.ContainsAll(ClassName)
        End Function

        ''' <summary>
        ''' Remove a class from element
        ''' </summary>
        ''' <param name="ClassName"></param>
        ''' <returns></returns>
        Public Shadows Function Remove(ClassName As String) As Boolean
            Return Me.Item(ClassName) = False
            Return True
        End Function

        Public Shadows Function GetEnumerator() As IEnumerator(Of String)
            Return CreateClassList.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator
            Return CreateClassList.GetEnumerator
        End Function

        Public Overrides Function ToString() As String
            Return Me.mElement.Attribute("class")
        End Function

    End Class

End Namespace