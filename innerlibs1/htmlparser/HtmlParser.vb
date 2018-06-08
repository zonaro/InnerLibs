Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Collections.Specialized
Imports InnerLibs.HtmlParser

Namespace HtmlParser

    ''' <summary>
    ''' This is the main HTML parser class. I recommend you don't play around too much in here
    ''' as it's a little fiddly.
    '''
    ''' Bascially, this class will build a tree containing HtmlNode elements.
    ''' </summary>
    Friend Class HtmlParser


        ''' <summary>
        ''' Internal FSM to represent the state of the parser
        ''' </summary>
        Private Enum ParseStatus
            ReadText = 0
            ReadEndTag = 1
            ReadStartTag = 2
            ReadAttributeName = 3
            ReadAttributeValue = 4
        End Enum

        ''' <summary>
        ''' This constructs a new parser. Even though this object is currently stateless,
        ''' in the future, parameters coping for tollerance and SGML (etc.) will be passed.
        ''' </summary>
        Public Sub New()
        End Sub


#Region "The main parser"

        ''' <summary>
        ''' This will parse a string containing HTML and will produce a domain tree.
        ''' </summary>
        ''' <param name="html">The HTML to be parsed</param>
        ''' <returns>A tree representing the elements</returns>
        Public Function Parse(html As String) As HtmlNodeCollection
            Dim nodes As New HtmlNodeCollection(Nothing)

            html = PreprocessScript(html, "script")
            html = PreprocessScript(html, "style")

            html = RemoveComments(html)
            html = RemoveSGMLComments(html)
            Dim tokens As StringCollection = GetTokens(html)

            Dim index As Integer = 0
            Dim element As HtmlElement = Nothing
            While index < tokens.Count
                If "<".Equals(tokens(index)) Then
                    ' Read open tag

                    index += 1
                    If index >= tokens.Count Then
                        Exit While
                    End If
                    Dim tag_name As String = tokens(index)
                    index += 1

                    Select Case tag_name.ToLower
                        Case "img"
                            element = New HtmlImageElement()
                        Case "a"
                            element = New HtmlAnchorElement()
                        Case "select"
                            element = New HtmlSelectElement()
                        Case "time"
                            element = New HtmlTimeElement()
                        Case "option"
                            element = New HtmlOptionElement()
                        Case "table"
                            element = New HtmlTableElement()
                        Case "thead"
                            element = New HtmlTableHeadElement()
                        Case "tbody"
                            element = New HtmlTableBodyElement()
                        Case "tfoot"
                            element = New HtmlTableFootElement()
                        Case "caption"
                            element = New HtmlTableCaptionElement()
                        Case "tr"
                            element = New HtmlTableRowElement()
                        Case "td", "th"
                            element = New HtmlTableCellElement()
                        Case "ol", "ul"
                            element = New HtmlListElement(tag_name = "ol")
                        Case Else
                            element = New HtmlElement(tag_name)
                    End Select

                    ' read the attributes and values

                    While index < tokens.Count AndAlso Not ">".Equals(tokens(index)) AndAlso Not "/>".Equals(tokens(index))
                        Dim attribute_name As String = tokens(index)
                        index += 1
                        If index < tokens.Count AndAlso "=".Equals(tokens(index)) Then
                            index += 1
                            Dim attribute_value As String
                            If index < tokens.Count Then
                                attribute_value = tokens(index)
                            Else
                                attribute_value = Nothing
                            End If
                            index += 1
                            Dim attribute As New HtmlAttribute(attribute_name, System.Net.WebUtility.HtmlDecode(attribute_value))
                            element.Attributes.Add(attribute)
                        ElseIf index < tokens.Count Then
                            ' Null-value attribute
                            Dim attribute As New HtmlAttribute(attribute_name, Nothing)
                            element.Attributes.Add(attribute)
                        End If
                    End While
                    nodes.Add(element)
                    If index < tokens.Count AndAlso "/>".Equals(tokens(index)) Then
                        element.IsTerminated = True
                        index += 1
                        element = Nothing
                    ElseIf index < tokens.Count AndAlso ">".Equals(tokens(index)) Then
                        index += 1
                    End If
                ElseIf ">".Equals(tokens(index)) Then
                    index += 1
                ElseIf "</".Equals(tokens(index)) Then
                    ' Read close tag
                    index += 1
                    If index >= tokens.Count Then
                        Exit While
                    End If
                    Dim tag_name As String = tokens(index)
                    index += 1

                    Dim open_index As Integer = FindTagOpenNodeIndex(nodes, tag_name)
                    If open_index <> -1 Then
                        MoveNodesDown(nodes, open_index + 1, DirectCast(nodes(open_index), HtmlElement))
                        ' Er, there is a close tag without an opening tag!!
                    Else
                    End If

                    ' Skip to the end of this tag
                    While index < tokens.Count AndAlso Not ">".Equals(tokens(index))
                        index += 1
                    End While
                    If index < tokens.Count AndAlso ">".Equals(tokens(index)) Then
                        index += 1
                    End If

                    element = Nothing
                Else
                    ' Read text
                    Dim value As String = tokens(index)
                    value = DecodeScript(value)

                    ' We do nothing
                    If value.IsBlank Then
                    Else
                        If Not (element IsNot Nothing AndAlso element.NoEscaping) Then
                            value = System.Net.WebUtility.HtmlDecode(value)
                        End If
                        If value.IsNotBlank Then
                            Dim node As New HtmlText(value)
                            nodes.Add(node)
                        End If
                    End If
                    index += 1
                End If
            End While
            Return nodes
        End Function

        ''' <summary>
        ''' This will move all the nodes from the specified index to the new parent.
        ''' </summary>
        ''' <param name="nodes">The collection of nodes</param>
        ''' <param name="node_index">The index of the first node (in the above collection) to move</param>
        ''' <param name="new_parent">The node which will become the parent of the moved nodes</param>

        Private Sub MoveNodesDown(ByRef nodes As HtmlNodeCollection, node_index As Integer, new_parent As HtmlElement)
            For i As Integer = node_index To nodes.Count - 1
                DirectCast(new_parent, HtmlElement).Nodes.Add(nodes(i))
                nodes(i).SetParent(new_parent)
            Next
            Dim c As Integer = nodes.Count
            For i As Integer = node_index To c - 1
                nodes.RemoveAt(node_index)
            Next
            new_parent.IsExplicitlyTerminated = True
        End Sub

        ''' <summary>
        ''' This will find the corresponding opening tag for the named one. This is identified as
        ''' the most recently read node with the same name, but with no child nodes.
        ''' </summary>
        ''' <param name="nodes">The collection of nodes</param>
        ''' <param name="name">The name of the tag</param>
        ''' <returns>The index of the opening tag, or -1 if it was not found</returns>
        Private Function FindTagOpenNodeIndex(nodes As HtmlNodeCollection, name As String) As Integer
            For index As Integer = nodes.Count - 1 To 0 Step -1
                If TypeOf nodes(index) Is HtmlElement Then
                    If DirectCast(nodes(index), HtmlElement).Name.ToLower().Equals(name.ToLower()) AndAlso DirectCast(nodes(index), HtmlElement).Nodes.Count = 0 AndAlso DirectCast(nodes(index), HtmlElement).IsTerminated = False Then
                        Return index
                    End If
                End If
            Next
            Return -1
        End Function

#End Region

#Region "HTML clean-up functions"

        ''' <summary>
        ''' This will remove all HTML comments from the input string. This will
        ''' not remove comment markers from inside tag attribute values.
        ''' </summary>
        ''' <param name="input">Input HTML containing comments</param>
        ''' <returns>HTML containing no comments</returns>

        Private Function RemoveComments(input As String) As String
            Dim output As New StringBuilder()

            Dim i As Integer = 0
            Dim inTag As Boolean = False

            While i < input.Length
                If i + 4 < input.Length AndAlso input.Substring(i, 4).Equals("<!--") Then
                    i += 4
                    i = input.IndexOf("-->", i)
                    If i = -1 Then
                        Exit While
                    End If
                    i += 3
                ElseIf input.Substring(i, 1).Equals("<") Then
                    inTag = True
                    output.Append("<")
                    i += 1
                ElseIf input.Substring(i, 1).Equals(">") Then
                    inTag = False
                    output.Append(">")
                    i += 1
                ElseIf input.Substring(i, 1).Equals("""") AndAlso inTag Then
                    Dim string_start As Integer = i
                    i += 1
                    i = input.IndexOf("""", i)
                    If i = -1 Then
                        Exit While
                    End If
                    i += 1
                    output.Append(input.Substring(string_start, i - string_start))
                ElseIf input.Substring(i, 1).Equals("'") AndAlso inTag Then
                    Dim string_start As Integer = i
                    i += 1
                    i = input.IndexOf("'", i)
                    If i = -1 Then
                        Exit While
                    End If
                    i += 1
                    output.Append(input.Substring(string_start, i - string_start))
                Else
                    output.Append(input.Substring(i, 1))
                    i += 1
                End If
            End While

            Return output.ToString()
        End Function

        ''' <summary>
        ''' This will remove all HTML comments from the input string. This will
        ''' not remove comment markers from inside tag attribute values.
        ''' </summary>
        ''' <param name="input">Input HTML containing comments</param>
        ''' <returns>HTML containing no comments</returns>

        Private Function RemoveSGMLComments(input As String) As String
            Dim output As New StringBuilder()

            Dim i As Integer = 0
            Dim inTag As Boolean = False

            While i < input.Length
                If i + 2 < input.Length AndAlso input.Substring(i, 2).Equals("<!") Then
                    i += 2
                    i = input.IndexOf(">", i)
                    If i = -1 Then
                        Exit While
                    End If
                    i += 3
                ElseIf input.Substring(i, 1).Equals("<") Then
                    inTag = True
                    output.Append("<")
                    i += 1
                ElseIf input.Substring(i, 1).Equals(">") Then
                    inTag = False
                    output.Append(">")
                    i += 1
                ElseIf input.Substring(i, 1).Equals("""") AndAlso inTag Then
                    Dim string_start As Integer = i
                    i += 1
                    i = input.IndexOf("""", i)
                    If i = -1 Then
                        Exit While
                    End If
                    i += 1
                    output.Append(input.Substring(string_start, i - string_start))
                ElseIf input.Substring(i, 1).Equals("'") AndAlso inTag Then
                    Dim string_start As Integer = i
                    i += 1
                    i = input.IndexOf("'", i)
                    If i = -1 Then
                        Exit While
                    End If
                    i += 1
                    output.Append(input.Substring(string_start, i - string_start))
                Else
                    output.Append(input.Substring(i, 1))
                    i += 1
                End If
            End While

            Return output.ToString()
        End Function

        ''' <summary>
        ''' This will encode the scripts within the page so they get passed through the
        ''' parser properly. This is due to some people using comments protect the script
        ''' and others who don't. It also takes care of issues where the script itself has
        ''' HTML comments in (in strings, for example).
        ''' </summary>
        ''' <param name="input">The HTML to examine</param>
        ''' <returns>The HTML with the scripts marked up differently</returns>
        Private Function PreprocessScript(input As String, tag_name As String) As String
            Dim output As New StringBuilder()
            Dim index As Integer = 0
            Dim tag_name_len As Integer = tag_name.Length
            While index < input.Length
                Dim omit_body As Boolean = False
                If index + tag_name_len + 1 < input.Length AndAlso input.Substring(index, tag_name_len + 1).ToLower().Equals(Convert.ToString("<") & tag_name) Then
                    ' Look for the end of the tag (we pass the attributes through as normal)
                    Do
                        If index >= input.Length Then
                            Exit Do
                        ElseIf input.Substring(index, 1).Equals(">") Then
                            output.Append(">")
                            index += 1
                            Exit Do
                        ElseIf index + 1 < input.Length AndAlso input.Substring(index, 2).Equals("/>") Then
                            output.Append("/>")
                            index += 2
                            omit_body = True
                            Exit Do
                        ElseIf input.Substring(index, 1).Equals("""") Then
                            output.Append("""")
                            index += 1
                            While index < input.Length AndAlso Not input.Substring(index, 1).Equals("""")
                                output.Append(input.Substring(index, 1))
                                index += 1
                            End While
                            If index < input.Length Then
                                index += 1
                                output.Append("""")
                            End If
                        ElseIf input.Substring(index, 1).Equals("'") Then
                            output.Append("'")
                            index += 1
                            While index < input.Length AndAlso Not input.Substring(index, 1).Equals("'")
                                output.Append(input.Substring(index, 1))
                                index += 1
                            End While
                            If index < input.Length Then
                                index += 1
                                output.Append("'")
                            End If
                        Else
                            output.Append(input.Substring(index, 1))
                            index += 1
                        End If
                    Loop While True
                    If index >= input.Length Then
                        Exit While
                    End If
                    ' Phew! Ok now we are reading the script body

                    If Not omit_body Then
                        Dim script_body As New StringBuilder()
                        While index + tag_name_len + 3 < input.Length AndAlso Not input.Substring(index, tag_name_len + 3).ToLower().Equals((Convert.ToString("</") & tag_name) + ">")
                            script_body.Append(input.Substring(index, 1))
                            index += 1
                        End While
                        ' Done - now encode the script
                        output.Append(EncodeScript(script_body.ToString()))
                        output.Append((Convert.ToString("</") & tag_name) + ">")
                        If index + tag_name_len + 3 < input.Length Then
                            index += tag_name_len + 3
                        End If
                    End If
                Else
                    output.Append(input.Substring(index, 1))
                    index += 1
                End If
            End While
            Return output.ToString()
        End Function

        Private Shared Function EncodeScript(script As String) As String
            Dim output As String = script.Replace("<", "[MIL-SCRIPT-LT]")
            output = output.Replace(">", "[MIL-SCRIPT-GT]")
            output = output.Replace(vbCr, "[MIL-SCRIPT-CR]")
            output = output.Replace(vbLf, "[MIL-SCRIPT-LF]")
            Return output
        End Function

        Private Shared Function DecodeScript(script As String) As String
            Dim output As String = script.Replace("[MIL-SCRIPT-LT]", "<")
            output = output.Replace("[MIL-SCRIPT-GT]", ">")
            output = output.Replace("[MIL-SCRIPT-CR]", vbCr)
            output = output.Replace("[MIL-SCRIPT-LF]", vbLf)
            Return output
        End Function

#End Region

#Region "HTML tokeniser"

        ''' <summary>
        ''' This will tokenise the HTML input string.
        ''' </summary>
        ''' <param name="input"></param>
        ''' <returns></returns>
        Private Function GetTokens(input As String) As StringCollection
            Dim tokens As New StringCollection()

            Dim i As Integer = 0
            Dim status As ParseStatus = ParseStatus.ReadText

            While i < input.Length
                If status = ParseStatus.ReadText Then
                    If i + 2 < input.Length AndAlso input.Substring(i, 2).Equals("</") Then
                        i += 2
                        tokens.Add("</")
                        status = ParseStatus.ReadEndTag
                    ElseIf input.Substring(i, 1).Equals("<") Then
                        i += 1
                        tokens.Add("<")
                        status = ParseStatus.ReadStartTag
                    Else
                        Dim next_index As Integer = input.IndexOf("<", i)
                        If next_index = -1 Then
                            tokens.Add(input.Substring(i))
                            Exit While
                        Else
                            tokens.Add(input.Substring(i, next_index - i))
                            i = next_index
                        End If
                    End If
                ElseIf status = ParseStatus.ReadStartTag Then
                    ' Skip leading whitespace in tag
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    ' Read tag name
                    Dim tag_name_start As Integer = i
                    While i < input.Length AndAlso input.Substring(i, 1).IndexOfAny(" " & vbCr & vbLf & vbTab & "/>".ToCharArray()) = -1
                        i += 1
                    End While
                    tokens.Add(input.Substring(tag_name_start, i - tag_name_start))
                    ' Skip trailing whitespace in tag
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    If i + 1 < input.Length AndAlso input.Substring(i, 1).Equals("/>") Then
                        tokens.Add("/>")
                        status = ParseStatus.ReadText
                        i += 2
                    ElseIf i < input.Length AndAlso input.Substring(i, 1).Equals(">") Then
                        tokens.Add(">")
                        status = ParseStatus.ReadText
                        i += 1
                    Else
                        status = ParseStatus.ReadAttributeName
                    End If
                ElseIf status = ParseStatus.ReadEndTag Then
                    ' Skip leading whitespace in tag
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    ' Read tag name
                    Dim tag_name_start As Integer = i
                    While i < input.Length AndAlso input.Substring(i, 1).IndexOfAny(" " & vbCr & vbLf & vbTab & ">".ToCharArray()) = -1
                        i += 1
                    End While
                    tokens.Add(input.Substring(tag_name_start, i - tag_name_start))
                    ' Skip trailing whitespace in tag
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    If i < input.Length AndAlso input.Substring(i, 1).Equals(">") Then
                        tokens.Add(">")
                        status = ParseStatus.ReadText
                        i += 1
                    End If
                ElseIf status = ParseStatus.ReadAttributeName Then
                    ' Read attribute name
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    Dim attribute_name_start As Integer = i
                    While i < input.Length AndAlso input.Substring(i, 1).IndexOfAny(" " & vbCr & vbLf & vbTab & "/>=".ToCharArray()) = -1
                        i += 1
                    End While
                    tokens.Add(input.Substring(attribute_name_start, i - attribute_name_start))
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    If i + 1 < input.Length AndAlso input.Substring(i, 2).Equals("/>") Then
                        tokens.Add("/>")
                        status = ParseStatus.ReadText
                        i += 2
                    ElseIf i < input.Length AndAlso input.Substring(i, 1).Equals(">") Then
                        tokens.Add(">")
                        status = ParseStatus.ReadText
                        i += 1
                    ElseIf i < input.Length AndAlso input.Substring(i, 1).Equals("=") Then
                        tokens.Add("=")
                        i += 1
                        status = ParseStatus.ReadAttributeValue
                    ElseIf i < input.Length AndAlso input.Substring(i, 1).Equals("/") Then
                        i += 1
                    End If
                ElseIf status = ParseStatus.ReadAttributeValue Then
                    ' Read the attribute value
                    While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                        i += 1
                    End While
                    If i < input.Length AndAlso input.Substring(i, 1).Equals("""") Then
                        Dim value_start As Integer = i
                        i += 1
                        While i < input.Length AndAlso Not input.Substring(i, 1).Equals("""")
                            i += 1
                        End While
                        If i < input.Length AndAlso input.Substring(i, 1).Equals("""") Then
                            i += 1
                        End If
                        tokens.Add(input.Substring(value_start + 1, i - value_start - 2))
                        status = ParseStatus.ReadAttributeName
                    ElseIf i < input.Length AndAlso input.Substring(i, 1).Equals("'") Then
                        Dim value_start As Integer = i
                        i += 1
                        While i < input.Length AndAlso Not input.Substring(i, 1).Equals("'")
                            i += 1
                        End While
                        If i < input.Length AndAlso input.Substring(i, 1).Equals("'") Then
                            i += 1
                        End If
                        tokens.Add(input.Substring(value_start + 1, i - value_start - 2))
                        status = ParseStatus.ReadAttributeName
                    Else
                        Dim value_start As Integer = i
                        While i < input.Length AndAlso input.Substring(i, 1).IndexOfAny(" " & vbCr & vbLf & vbTab & "/>".ToCharArray()) = -1
                            i += 1
                        End While
                        tokens.Add(input.Substring(value_start, i - value_start))
                        While i < input.Length AndAlso input.Substring(i, 1).ContainsAny(WhiteSpaceChars)
                            i += 1
                        End While
                        status = ParseStatus.ReadAttributeName
                    End If
                    If i + 1 < input.Length AndAlso input.Substring(i, 2).Equals("/>") Then
                        tokens.Add("/>")
                        status = ParseStatus.ReadText
                        i += 2
                    ElseIf i < input.Length AndAlso input.Substring(i, 1).Equals(">") Then
                        tokens.Add(">")
                        i += 1
                        status = ParseStatus.ReadText
                        ' ANDY
                    End If
                End If
            End While

            Return tokens
        End Function

#End Region

    End Class

End Namespace

