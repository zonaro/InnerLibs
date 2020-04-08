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
                Return CreateClassList.ContainsAll(ClassName.Split(WordSplitters, StringSplitOptions.RemoveEmptyEntries))
            End Get
            Set(value As Boolean)
                Dim styledic As List(Of String) = CreateClassList()
                If ClassName.IsNotBlank Then
                    For Each i As String In ClassName.Split(WordSplitters, StringSplitOptions.RemoveEmptyEntries)
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
                    p &= (k.ToLower & " ")
                Next
                mElement.Attribute("class") = p.Trim
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
        Public Shadows Function Add(ParamArray ClassName As String()) As HtmlElement
            For Each c In ClassName.SelectMany(Function(x) x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                Me.Item(c) = True
            Next
            Return Me.mElement
        End Function

        ''' <summary>
        ''' Add a class to element
        ''' </summary>
        ''' <param name="ClassName"></param>
        ''' <returns></returns>
        Public Shadows Function AddRange(ParamArray ClassName As String()) As HtmlElement
            Return Me.Add(ClassName)
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
        End Function

        Public Shadows Function GetEnumerator() As IEnumerator(Of String)
            Return CreateClassList.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator
            Return CreateClassList.GetEnumerator
        End Function

        ''' <summary>
        ''' Returns the class attribute of element
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.mElement.Attribute("class")
        End Function

        ''' <summary>
        ''' Proccess a set of objects and apply class names according to the boolean properties, keyvaluepairs or strings
        ''' </summary>
        ''' <param name="ClassEx"></param>
        ''' <returns></returns>
        Public Function FromExpression(ParamArray ClassEx As Object()) As String
            For Each ooo In ClassEx.ToArray
                Select Case ooo.GetType()
                    Case GetType(String)
                        Me.Add(ooo)
                    Case GetType(KeyValuePair(Of String, Boolean))
                        Me.Item(ooo.Key) = ooo.Value
                    Case Else
                        For Each prop In ClassTools.GetProperties(ooo).Where(Function(x) x.GetType Is GetType(Boolean))
                            Me.Item(prop.Name) = If(prop.GetValue(ooo), False)
                        Next
                End Select
            Next
            Return Me.ToString
        End Function

    End Class

End Namespace