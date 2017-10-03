Imports System.ComponentModel

Namespace HtmlParser

    Public Class CssProperties
        Implements IDictionary(Of String, String)

        Private mElement As HtmlElement

        Friend Sub New(Element As HtmlElement)
            Me.mElement = Element
        End Sub


        Public ReadOnly Property Keys As ICollection(Of String) Implements IDictionary(Of String, String).Keys
            Get
                Dim s = mElement.Attribute("style")
                If s.IsNotBlank Then
                    Dim styledic As New Dictionary(Of String, String)
                    For Each i In s.Split(";")
                        Dim n = i.Split(":")
                        styledic.Add(n(0).ToLower, n(1).ToLower)
                        Return styledic.Keys
                    Next
                End If
                Return {}
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of String) Implements IDictionary(Of String, String).Values
            Get
                Dim s = mElement.Attribute("style")
                If s.IsNotBlank Then
                    Dim styledic As New Dictionary(Of String, String)
                    For Each i In s.Split(";")
                        Dim n = i.Split(":")
                        styledic.Add(n(0).ToLower, n(1).ToLower)
                        Return styledic.Values
                    Next
                End If
                Return {}
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, String)).Count
            Get
                Dim s = mElement.Attribute("style")
                If s.IsNotBlank Then
                    Dim styledic As New Dictionary(Of String, String)
                    For Each i In s.Split(";")
                        Dim n = i.Split(":")
                        styledic.Add(n(0).ToLower, n(1).ToLower)
                        Return styledic.Count
                    Next
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IDictionary(Of String, String).ContainsKey
            Return Me.Item(key).IsNotBlank
        End Function

        Public Sub Add(key As String, value As String) Implements IDictionary(Of String, String).Add
            Item(key) = value
        End Sub

        Private Function Remove(key As String) As Boolean Implements IDictionary(Of String, String).Remove
            If Me.Item(key).IsNotBlank Then
                Item(key) = ""
                Return True
            End If
            Return False
        End Function

        Public Function TryGetValue(key As String, ByRef value As String) As Boolean Implements IDictionary(Of String, String).TryGetValue
            value = Me.Item(key)
            Return value
        End Function

        Public Sub Add(item As KeyValuePair(Of String, String)) Implements ICollection(Of KeyValuePair(Of String, String)).Add
            Me.Add(item.Key, item.Value)
        End Sub

        Public Sub ClearStyle() Implements ICollection(Of KeyValuePair(Of String, String)).Clear
            Try
                mElement.Attributes.Remove(mElement.Attributes().Where(Function(p) p.Name = "style"))
            Catch ex As Exception
            End Try
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, String)) As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).Contains
            Return Me.Item(item.Key) = item.Value
        End Function

        Private Sub CopyTo(array() As KeyValuePair(Of String, String), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, String)).CopyTo

        End Sub

        Public Function Remove(item As KeyValuePair(Of String, String)) As Boolean Implements ICollection(Of KeyValuePair(Of String, String)).Remove
            Return Me.Remove(item.Key)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
            Throw New NotImplementedException()
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Throw New NotImplementedException()
        End Function

        ''' <summary>
        ''' Gets or sets the style of element
        ''' </summary>
        ''' <param name="Name">Name of CSS style</param>
        ''' <returns></returns>
        Default Shadows Property Item(Name As String) As String Implements IDictionary(Of String, String).Item
            Get
                If Name.IsNotBlank Then
                    Dim s = mElement.Attribute("style")
                    If s.IsNotBlank Then
                        Dim styledic As New Dictionary(Of String, String)
                        For Each i In s.Split(";")
                            Dim n = i.Split(":")
                            styledic.Add(n(0).ToLower, n(1).ToLower)
                        Next
                        If styledic.ContainsKey(Name.ToLower) Then
                            Return styledic(Name.ToLower)
                        End If
                    End If
                    Return ""
                Else
                    Return mElement.Attribute("style")
                End If
            End Get
            Set(value As String)
                Dim s = mElement.Attribute("style")
                Dim styledic As New Dictionary(Of String, String)
                If Name.IsNotBlank Then

                    If s.IsNotBlank Then
                        For Each i In s.Split(";")
                            Dim n = i.Split(":")
                            styledic.Add(n(0).ToLower, n(1).ToLower)
                        Next
                        styledic(Name.ToLower) = value
                    Else
                        If value.IsBlank Then
                            styledic.Remove(Name.ToLower)
                        Else
                            styledic(Name.ToLower) = value
                        End If
                    End If
                    Dim p = ""
                    For Each k In styledic.Where(Function(d) d.Value.IsNotBlank)
                        p.Append(k.Key.ToLower & ":" & k.Value.ToLower & ";")
                    Next
                    mElement.Attribute("style") = p
                End If

            End Set
        End Property


        Public Overrides Function ToString() As String
            Return mElement.Attribute("style")
        End Function

        <Category("Alignment"), Description("Set the align-content attribute")>
        Public Property align_content As String
            Get
                Return Me.Item("align-content")
            End Get
            Set(value As String)
                Me.Item("align-content") = value
            End Set
        End Property

        Public Property align_items As String
            Get
                Return Me.Item("align-items")
            End Get
            Set(value As String)
                Me.Item("align-items") = value
            End Set
        End Property

        Public Property align_self As String
            Get
                Return Me.Item("align-self")
            End Get
            Set(value As String)
                Me.Item("align-self") = value
            End Set
        End Property

        Public Property animation As String
            Get
                Return Me.Item("animation")
            End Get
            Set(value As String)
                Me.Item("animation") = value
            End Set
        End Property

        Public Property animation_delay As String
            Get
                Return Me.Item("animation-delay")
            End Get
            Set(value As String)
                Me.Item("animation-delay") = value
            End Set
        End Property

        Public Property animation_direction As String
            Get
                Return Me.Item("animation-direction")
            End Get
            Set(value As String)
                Me.Item("animation-direction") = value
            End Set
        End Property

        Public Property animation_duration As String
            Get
                Return Me.Item("animation-duration")
            End Get
            Set(value As String)
                Me.Item("animation-duration") = value
            End Set
        End Property

        Public Property animation_fill_mode As String
            Get
                Return Me.Item("animation-fill-mode")
            End Get
            Set(value As String)
                Me.Item("animation-fill-mode") = value
            End Set
        End Property

        Public Property animation_iteration_count As String
            Get
                Return Me.Item("animation-iteration-count")
            End Get
            Set(value As String)
                Me.Item("animation-iteration-count") = value
            End Set
        End Property

        Public Property animation_name As String
            Get
                Return Me.Item("animation-name")
            End Get
            Set(value As String)
                Me.Item("animation-name") = value
            End Set
        End Property

        Public Property animation_play_state As String
            Get
                Return Me.Item("animation-play-state")
            End Get
            Set(value As String)
                Me.Item("animation-play-state") = value
            End Set
        End Property

        Public Property animation_timing_function As String
            Get
                Return Me.Item("animation-timing-function")
            End Get
            Set(value As String)
                Me.Item("animation-timing-function") = value
            End Set
        End Property

        Public Property backface_visibility As String
            Get
                Return Me.Item("backface-visibility")
            End Get
            Set(value As String)
                Me.Item("backface-visibility") = value
            End Set
        End Property

        Public Property background As String
            Get
                Return Me.Item("background")
            End Get
            Set(value As String)
                Me.Item("background") = value
            End Set
        End Property

        Public Property background_attachment As String
            Get
                Return Me.Item("background-attachment")
            End Get
            Set(value As String)
                Me.Item("background-attachment") = value
            End Set
        End Property

        Public Property background_clip As String
            Get
                Return Me.Item("background-clip")
            End Get
            Set(value As String)
                Me.Item("background-clip") = value
            End Set
        End Property

        Public Property background_color As String
            Get
                Return Me.Item("background-color")
            End Get
            Set(value As String)
                Me.Item("background-color") = value
            End Set
        End Property

        Public Property background_image As String
            Get
                Return Me.Item("background-image")
            End Get
            Set(value As String)
                Me.Item("background-image") = value
            End Set
        End Property

        Public Property background_origin As String
            Get
                Return Me.Item("background-origin")
            End Get
            Set(value As String)
                Me.Item("background-origin") = value
            End Set
        End Property

        Public Property background_position As String
            Get
                Return Me.Item("background-position")
            End Get
            Set(value As String)
                Me.Item("background-position") = value
            End Set
        End Property

        Public Property background_repeat As String
            Get
                Return Me.Item("background-repeat")
            End Get
            Set(value As String)
                Me.Item("background-repeat") = value
            End Set
        End Property

        Public Property background_size As String
            Get
                Return Me.Item("background-size")
            End Get
            Set(value As String)
                Me.Item("background-size") = value
            End Set
        End Property

        Public Property border As String
            Get
                Return Me.Item("border")
            End Get
            Set(value As String)
                Me.Item("border") = value
            End Set
        End Property

        Public Property border_bottom As String
            Get
                Return Me.Item("border-bottom")
            End Get
            Set(value As String)
                Me.Item("border-bottom") = value
            End Set
        End Property

        Public Property border_bottom_color As String
            Get
                Return Me.Item("border-bottom-color")
            End Get
            Set(value As String)
                Me.Item("border-bottom-color") = value
            End Set
        End Property

        Public Property border_bottom_left_radius As String
            Get
                Return Me.Item("border-bottom-left-radius")
            End Get
            Set(value As String)
                Me.Item("border-bottom-left-radius") = value
            End Set
        End Property

        Public Property border_bottom_right_radius As String
            Get
                Return Me.Item("border-bottom-right-radius")
            End Get
            Set(value As String)
                Me.Item("border-bottom-right-radius") = value
            End Set
        End Property

        Public Property border_bottom_style As String
            Get
                Return Me.Item("border-bottom-style")
            End Get
            Set(value As String)
                Me.Item("border-bottom-style") = value
            End Set
        End Property

        Public Property border_bottom_width As String
            Get
                Return Me.Item("border-bottom-width")
            End Get
            Set(value As String)
                Me.Item("border-bottom-width") = value
            End Set
        End Property

        Public Property border_collapse As String
            Get
                Return Me.Item("border-collapse")
            End Get
            Set(value As String)
                Me.Item("border-collapse") = value
            End Set
        End Property

        Public Property border_color As String
            Get
                Return Me.Item("border-color")
            End Get
            Set(value As String)
                Me.Item("border-color") = value
            End Set
        End Property

        Public Property border_image As String
            Get
                Return Me.Item("border-image")
            End Get
            Set(value As String)
                Me.Item("border-image") = value
            End Set
        End Property

        Public Property border_image_outset As String
            Get
                Return Me.Item("border-image-outset")
            End Get
            Set(value As String)
                Me.Item("border-image-outset") = value
            End Set
        End Property

        Public Property border_image_repeat As String
            Get
                Return Me.Item("border-image-repeat")
            End Get
            Set(value As String)
                Me.Item("border-image-repeat") = value
            End Set
        End Property

        Public Property border_image_slice As String
            Get
                Return Me.Item("border-image-slice")
            End Get
            Set(value As String)
                Me.Item("border-image-slice") = value
            End Set
        End Property

        Public Property border_image_source As String
            Get
                Return Me.Item("border-image-source")
            End Get
            Set(value As String)
                Me.Item("border-image-source") = value
            End Set
        End Property

        Public Property border_image_width As String
            Get
                Return Me.Item("border-image-width")
            End Get
            Set(value As String)
                Me.Item("border-image-width") = value
            End Set
        End Property

        Public Property border_left As String
            Get
                Return Me.Item("border-left")
            End Get
            Set(value As String)
                Me.Item("border-left") = value
            End Set
        End Property

        Public Property border_left_color As String
            Get
                Return Me.Item("border-left-color")
            End Get
            Set(value As String)
                Me.Item("border-left-color") = value
            End Set
        End Property

        Public Property border_left_style As String
            Get
                Return Me.Item("border-left-style")
            End Get
            Set(value As String)
                Me.Item("border-left-style") = value
            End Set
        End Property

        Public Property border_left_width As String
            Get
                Return Me.Item("border-left-width")
            End Get
            Set(value As String)
                Me.Item("border-left-width") = value
            End Set
        End Property

        Public Property border_radius As String
            Get
                Return Me.Item("border-radius")
            End Get
            Set(value As String)
                Me.Item("border-radius") = value
            End Set
        End Property

        Public Property border_right As String
            Get
                Return Me.Item("border-right")
            End Get
            Set(value As String)
                Me.Item("border-right") = value
            End Set
        End Property

        Public Property border_right_color As String
            Get
                Return Me.Item("border-right-color")
            End Get
            Set(value As String)
                Me.Item("border-right-color") = value
            End Set
        End Property

        Public Property border_right_style As String
            Get
                Return Me.Item("border-right-style")
            End Get
            Set(value As String)
                Me.Item("border-right-style") = value
            End Set
        End Property

        Public Property border_right_width As String
            Get
                Return Me.Item("border-right-width")
            End Get
            Set(value As String)
                Me.Item("border-right-width") = value
            End Set
        End Property

        Public Property border_spacing As String
            Get
                Return Me.Item("border-spacing")
            End Get
            Set(value As String)
                Me.Item("border-spacing") = value
            End Set
        End Property

        Public Property border_style As String
            Get
                Return Me.Item("border-style")
            End Get
            Set(value As String)
                Me.Item("border-style") = value
            End Set
        End Property

        Public Property border_top As String
            Get
                Return Me.Item("border-top")
            End Get
            Set(value As String)
                Me.Item("border-top") = value
            End Set
        End Property

        Public Property border_top_color As String
            Get
                Return Me.Item("border-top-color")
            End Get
            Set(value As String)
                Me.Item("border-top-color") = value
            End Set
        End Property

        Public Property border_top_left_radius As String
            Get
                Return Me.Item("border-top-left-radius")
            End Get
            Set(value As String)
                Me.Item("border-top-left-radius") = value
            End Set
        End Property

        Public Property border_top_right_radius As String
            Get
                Return Me.Item("border-top-right-radius")
            End Get
            Set(value As String)
                Me.Item("border-top-right-radius") = value
            End Set
        End Property

        Public Property border_top_style As String
            Get
                Return Me.Item("border-top-style")
            End Get
            Set(value As String)
                Me.Item("border-top-style") = value
            End Set
        End Property

        Public Property border_top_width As String
            Get
                Return Me.Item("border-top-width")
            End Get
            Set(value As String)
                Me.Item("border-top-width") = value
            End Set
        End Property

        Public Property border_width As String
            Get
                Return Me.Item("border-width")
            End Get
            Set(value As String)
                Me.Item("border-width") = value
            End Set
        End Property

        Public Property bottom As String
            Get
                Return Me.Item("bottom")
            End Get
            Set(value As String)
                Me.Item("bottom") = value
            End Set
        End Property

        Public Property box_shadow As String
            Get
                Return Me.Item("box-shadow")
            End Get
            Set(value As String)
                Me.Item("box-shadow") = value
            End Set
        End Property

        Public Property box_sizing As String
            Get
                Return Me.Item("box-sizing")
            End Get
            Set(value As String)
                Me.Item("box-sizing") = value
            End Set
        End Property

        Public Property caption_side As String
            Get
                Return Me.Item("caption-side")
            End Get
            Set(value As String)
                Me.Item("caption-side") = value
            End Set
        End Property

        Public Property clear As String
            Get
                Return Me.Item("clear")
            End Get
            Set(value As String)
                Me.Item("clear") = value
            End Set
        End Property

        Public Property clip As String
            Get
                Return Me.Item("clip")
            End Get
            Set(value As String)
                Me.Item("clip") = value
            End Set
        End Property

        Public Property color As String
            Get
                Return Me.Item("color")
            End Get
            Set(value As String)
                Me.Item("color") = value
            End Set
        End Property

        Public Property column_count As String
            Get
                Return Me.Item("column-count")
            End Get
            Set(value As String)
                Me.Item("column-count") = value
            End Set
        End Property

        Public Property column_fill As String
            Get
                Return Me.Item("column-fill")
            End Get
            Set(value As String)
                Me.Item("column-fill") = value
            End Set
        End Property

        Public Property column_gap As String
            Get
                Return Me.Item("column-gap")
            End Get
            Set(value As String)
                Me.Item("column-gap") = value
            End Set
        End Property

        Public Property column_rule As String
            Get
                Return Me.Item("column-rule")
            End Get
            Set(value As String)
                Me.Item("column-rule") = value
            End Set
        End Property

        Public Property column_rule_color As String
            Get
                Return Me.Item("column-rule-color")
            End Get
            Set(value As String)
                Me.Item("column-rule-color") = value
            End Set
        End Property

        Public Property column_rule_style As String
            Get
                Return Me.Item("column-rule-style")
            End Get
            Set(value As String)
                Me.Item("column-rule-style") = value
            End Set
        End Property

        Public Property column_rule_width As String
            Get
                Return Me.Item("column-rule-width")
            End Get
            Set(value As String)
                Me.Item("column-rule-width") = value
            End Set
        End Property

        Public Property column_span As String
            Get
                Return Me.Item("column-span")
            End Get
            Set(value As String)
                Me.Item("column-span") = value
            End Set
        End Property

        Public Property column_width As String
            Get
                Return Me.Item("column-width")
            End Get
            Set(value As String)
                Me.Item("column-width") = value
            End Set
        End Property

        Public Property columns As String
            Get
                Return Me.Item("columns")
            End Get
            Set(value As String)
                Me.Item("columns") = value
            End Set
        End Property

        Public Property content As String
            Get
                Return Me.Item("content")
            End Get
            Set(value As String)
                Me.Item("content") = value
            End Set
        End Property

        Public Property counter_increment As String
            Get
                Return Me.Item("counter-increment")
            End Get
            Set(value As String)
                Me.Item("counter-increment") = value
            End Set
        End Property

        Public Property counter_reset As String
            Get
                Return Me.Item("counter-increment")
            End Get
            Set(value As String)
                Me.Item("counter-increment") = value
            End Set
        End Property

        Public Property cursor As String
            Get
                Return Me.Item("cursor")
            End Get
            Set(value As String)
                Me.Item("cursor") = value
            End Set
        End Property

        Public Property direction As String
            Get
                Return Me.Item("direction")
            End Get
            Set(value As String)
                Me.Item("direction") = value
            End Set
        End Property

        Public Property display As String
            Get
                Return Me.Item("display")
            End Get
            Set(value As String)
                Me.Item("display") = value
            End Set
        End Property

        Public Property empty_cells As String
            Get
                Return Me.Item("empty-cells")
            End Get
            Set(value As String)
                Me.Item("empty-cells") = value
            End Set
        End Property

        Public Property flex As String
            Get
                Return Me.Item("flex")
            End Get
            Set(value As String)
                Me.Item("flex") = value
            End Set
        End Property

        Public Property flex_basis As String
            Get
                Return Me.Item("flex-basis")
            End Get
            Set(value As String)
                Me.Item("flex-basis") = value
            End Set
        End Property

        Public Property flex_direction As String
            Get
                Return Me.Item("flex-direction")
            End Get
            Set(value As String)
                Me.Item("flex-direction") = value
            End Set
        End Property

        Public Property flex_flow As String
            Get
                Return Me.Item("flex-flow")
            End Get
            Set(value As String)
                Me.Item("flex-flow") = value
            End Set
        End Property

        Public Property flex_grow As String
            Get
                Return Me.Item("flex-grow")
            End Get
            Set(value As String)
                Me.Item("flex-grow") = value
            End Set
        End Property

        Public Property flex_shrink As String
            Get
                Return Me.Item("flex-shrink")
            End Get
            Set(value As String)
                Me.Item("flex-shrink") = value
            End Set
        End Property

        Public Property flex_wrap As String
            Get
                Return Me.Item("flex-wrap")
            End Get
            Set(value As String)
                Me.Item("flex-wrap") = value
            End Set
        End Property

        Public Property float As String
            Get
                Return Me.Item("float")
            End Get
            Set(value As String)
                Me.Item("float") = value
            End Set
        End Property

        Public Property font As String
            Get
                Return Me.Item("font")
            End Get
            Set(value As String)
                Me.Item("font") = value
            End Set
        End Property

        Public Property font_family As String
            Get
                Return Me.Item("font-family")
            End Get
            Set(value As String)
                Me.Item("font-family") = value
            End Set
        End Property

        Public Property font_size As String
            Get
                Return Me.Item("font-size")
            End Get
            Set(value As String)
                Me.Item("font-size") = value
            End Set
        End Property

        Public Property font_size_adjust As String
            Get
                Return Me.Item("font-size-adjust")
            End Get
            Set(value As String)
                Me.Item("font-size-adjust") = value
            End Set
        End Property

        Public Property font_stretch As String
            Get
                Return Me.Item("font-stretch")
            End Get
            Set(value As String)
                Me.Item("font-stretch") = value
            End Set
        End Property

        Public Property font_style As String
            Get
                Return Me.Item("font-style")
            End Get
            Set(value As String)
                Me.Item("font-style") = value
            End Set
        End Property

        Public Property font_variant As String
            Get
                Return Me.Item("font-variant")
            End Get
            Set(value As String)
                Me.Item("font-variant") = value
            End Set
        End Property

        Public Property font_weight As String
            Get
                Return Me.Item("font-weight")
            End Get
            Set(value As String)
                Me.Item("font-weight") = value
            End Set
        End Property

        Public Property height As String
            Get
                Return Me.Item("height")
            End Get
            Set(value As String)
                Me.Item("height") = value
            End Set
        End Property

        Public Property justify_content As String
            Get
                Return Me.Item("justify-content")
            End Get
            Set(value As String)
                Me.Item("justify-content") = value
            End Set
        End Property

        Public Property left As String
            Get
                Return Me.Item("left")
            End Get
            Set(value As String)
                Me.Item("left") = value
            End Set
        End Property

        Public Property letter_spacing As String
            Get
                Return Me.Item("letter-spacing")
            End Get
            Set(value As String)
                Me.Item("letter-spacing") = value
            End Set
        End Property

        Public Property line_height As String
            Get
                Return Me.Item("line-height")
            End Get
            Set(value As String)
                Me.Item("line-height") = value
            End Set
        End Property

        Public Property list_style As String
            Get
                Return Me.Item("list-style")
            End Get
            Set(value As String)
                Me.Item("list-style") = value
            End Set
        End Property

        Public Property list_style_image As String
            Get
                Return Me.Item("list-style-image")
            End Get
            Set(value As String)
                Me.Item("list-style-image") = value
            End Set
        End Property

        Public Property list_style_position As String
            Get
                Return Me.Item("list-style-position")
            End Get
            Set(value As String)
                Me.Item("list-style-position") = value
            End Set
        End Property

        Public Property list_style_type As String
            Get
                Return Me.Item("list-style-type")
            End Get
            Set(value As String)
                Me.Item("list-style-type") = value
            End Set
        End Property

        Public Property margin As String
            Get
                Return Me.Item("margin")
            End Get
            Set(value As String)
                Me.Item("margin") = value
            End Set
        End Property

        Public Property margin_bottom As String
            Get
                Return Me.Item("margin-bottom")
            End Get
            Set(value As String)
                Me.Item("margin-bottom") = value
            End Set
        End Property

        Public Property margin_left As String
            Get
                Return Me.Item("margin-left")
            End Get
            Set(value As String)
                Me.Item("margin-left") = value
            End Set
        End Property

        Public Property margin_right As String
            Get
                Return Me.Item("margin-right")
            End Get
            Set(value As String)
                Me.Item("margin-right") = value
            End Set
        End Property

        Public Property margin_top As String
            Get
                Return Me.Item("margin-top")
            End Get
            Set(value As String)
                Me.Item("margin-top") = value
            End Set
        End Property

        Public Property max_height As String
            Get
                Return Me.Item("max-height")
            End Get
            Set(value As String)
                Me.Item("max-height") = value
            End Set
        End Property

        Public Property max_width As String
            Get
                Return Me.Item("max-width")
            End Get
            Set(value As String)
                Me.Item("max-width") = value
            End Set
        End Property

        Public Property min_height As String
            Get
                Return Me.Item("min-height")
            End Get
            Set(value As String)
                Me.Item("min-height") = value
            End Set
        End Property

        Public Property min_width As String
            Get
                Return Me.Item("min-width")
            End Get
            Set(value As String)
                Me.Item("min-width") = value
            End Set
        End Property

        Public Property opacity As String
            Get
                Return Me.Item("opacity")
            End Get
            Set(value As String)
                Me.Item("opacity") = value
            End Set
        End Property

        Public Property order As String
            Get
                Return Me.Item("order")
            End Get
            Set(value As String)
                Me.Item("order") = value
            End Set
        End Property

        Public Property outline As String
            Get
                Return Me.Item("outline")
            End Get
            Set(value As String)
                Me.Item("outline") = value
            End Set
        End Property

        Public Property outline_color As String
            Get
                Return Me.Item("outline-color")
            End Get
            Set(value As String)
                Me.Item("outline-color") = value
            End Set
        End Property

        Public Property outline_offset As String
            Get
                Return Me.Item("outline-offset")
            End Get
            Set(value As String)
                Me.Item("outline-offset") = value
            End Set
        End Property

        Public Property outline_style As String
            Get
                Return Me.Item("outline-style")
            End Get
            Set(value As String)
                Me.Item("outline-style") = value
            End Set
        End Property

        Public Property outline_width As String
            Get
                Return Me.Item("outline-width")
            End Get
            Set(value As String)
                Me.Item("outline-width") = value
            End Set
        End Property

        Public Property overflow As String
            Get
                Return Me.Item("overflow")
            End Get
            Set(value As String)
                Me.Item("overflow") = value
            End Set
        End Property

        Public Property overflow_x As String
            Get
                Return Me.Item("overflow-x")
            End Get
            Set(value As String)
                Me.Item("overflow-x") = value
            End Set
        End Property

        Public Property overflow_y As String
            Get
                Return Me.Item("overflow-y")
            End Get
            Set(value As String)
                Me.Item("overflow-y") = value
            End Set
        End Property

        Public Property padding As String
            Get
                Return Me.Item("padding")
            End Get
            Set(value As String)
                Me.Item("padding") = value
            End Set
        End Property

        Public Property padding_bottom As String
            Get
                Return Me.Item("padding-bottom")
            End Get
            Set(value As String)
                Me.Item("padding-bottom") = value
            End Set
        End Property

        Public Property padding_left As String
            Get
                Return Me.Item("padding-left")
            End Get
            Set(value As String)
                Me.Item("padding-left") = value
            End Set
        End Property

        Public Property padding_right As String
            Get
                Return Me.Item("padding-right")
            End Get
            Set(value As String)
                Me.Item("padding-right") = value
            End Set
        End Property

        Public Property padding_top As String
            Get
                Return Me.Item("padding-top")
            End Get
            Set(value As String)
                Me.Item("padding-top") = value
            End Set
        End Property

        Public Property page_break_after As String
            Get
                Return Me.Item("page-break-after")
            End Get
            Set(value As String)
                Me.Item("page-break-after") = value
            End Set
        End Property

        Public Property page_break_before As String
            Get
                Return Me.Item("page-break-before")
            End Get
            Set(value As String)
                Me.Item("page-break-before") = value
            End Set
        End Property

        Public Property page_break_inside As String
            Get
                Return Me.Item("page-break-inside")
            End Get
            Set(value As String)
                Me.Item("page-break-inside") = value
            End Set
        End Property

        Public Property perspective As String
            Get
                Return Me.Item("perspective")
            End Get
            Set(value As String)
                Me.Item("perspective") = value
            End Set
        End Property

        Public Property perspective_origin As String
            Get
                Return Me.Item("perspective-origin")
            End Get
            Set(value As String)
                Me.Item("perspective-origin") = value
            End Set
        End Property

        Public Property position As String
            Get
                Return Me.Item("position")
            End Get
            Set(value As String)
                Me.Item("position") = value
            End Set
        End Property

        Public Property quotes As String
            Get
                Return Me.Item("quotes")
            End Get
            Set(value As String)
                Me.Item("quotes") = value
            End Set
        End Property

        Public Property resize As String
            Get
                Return Me.Item("resize")
            End Get
            Set(value As String)
                Me.Item("resize") = value
            End Set
        End Property

        Public Property right As String
            Get
                Return Me.Item("right")
            End Get
            Set(value As String)
                Me.Item("right") = value
            End Set
        End Property

        Public Property tab_size As String
            Get
                Return Me.Item("tab-size")
            End Get
            Set(value As String)
                Me.Item("tab-size") = value
            End Set
        End Property

        Public Property table_layout As String
            Get
                Return Me.Item("table-layout")
            End Get
            Set(value As String)
                Me.Item("table-layout") = value
            End Set
        End Property

        Public Property text_align As String
            Get
                Return Me.Item("text-align")
            End Get
            Set(value As String)
                Me.Item("text-align") = value
            End Set
        End Property

        Public Property text_align_last As String
            Get
                Return Me.Item("text-align-last")
            End Get
            Set(value As String)
                Me.Item("text-align-last") = value
            End Set
        End Property

        Public Property text_decoration As String
            Get
                Return Me.Item("text-decoration")
            End Get
            Set(value As String)
                Me.Item("text-decoration") = value
            End Set
        End Property

        Public Property text_decoration_color As String
            Get
                Return Me.Item("text-decoration-color")
            End Get
            Set(value As String)
                Me.Item("text-decoration-color") = value
            End Set
        End Property

        Public Property text_decoration_line As String
            Get
                Return Me.Item("text-decoration-line")
            End Get
            Set(value As String)
                Me.Item("text-decoration-line") = value
            End Set
        End Property

        Public Property text_decoration_style As String
            Get
                Return Me.Item("text-decoration-style")
            End Get
            Set(value As String)
                Me.Item("text-decoration-style") = value
            End Set
        End Property

        Public Property text_indent As String
            Get
                Return Me.Item("text-indent")
            End Get
            Set(value As String)
                Me.Item("text-indent") = value
            End Set
        End Property

        Public Property text_justify As String
            Get
                Return Me.Item("text-justify")
            End Get
            Set(value As String)
                Me.Item("text-justify") = value
            End Set
        End Property

        Public Property text_overflow As String
            Get
                Return Me.Item("text-overflow")
            End Get
            Set(value As String)
                Me.Item("text-overflow") = value
            End Set
        End Property

        Public Property text_shadow As String
            Get
                Return Me.Item("text-shadow")
            End Get
            Set(value As String)
                Me.Item("text-shadow") = value
            End Set
        End Property

        Public Property text_transform As String
            Get
                Return Me.Item("text-transform")
            End Get
            Set(value As String)
                Me.Item("text-transform") = value
            End Set
        End Property

        Public Property top As String
            Get
                Return Me.Item("top")
            End Get
            Set(value As String)
                Me.Item("top") = value
            End Set
        End Property

        Public Property transform As String
            Get
                Return Me.Item("transform")
            End Get
            Set(value As String)
                Me.Item("transform") = value
            End Set
        End Property

        Public Property transform_origin As String
            Get
                Return Me.Item("transform-origin")
            End Get
            Set(value As String)
                Me.Item("transform-origin") = value
            End Set
        End Property

        Public Property transform_style As String
            Get
                Return Me.Item("transform-style")
            End Get
            Set(value As String)
                Me.Item("transform-style") = value
            End Set
        End Property

        Public Property transition As String
            Get
                Return Me.Item("transition")
            End Get
            Set(value As String)
                Me.Item("transition") = value
            End Set
        End Property

        Public Property transition_delay As String
            Get
                Return Me.Item("transition-delay")
            End Get
            Set(value As String)
                Me.Item("transition-delay") = value
            End Set
        End Property

        Public Property transition_duration As String
            Get
                Return Me.Item("transition-duration")
            End Get
            Set(value As String)
                Me.Item("transition-duration") = value
            End Set
        End Property

        Public Property transition_property As String
            Get
                Return Me.Item("transition-property")
            End Get
            Set(value As String)
                Me.Item("transition-property") = value
            End Set
        End Property

        Public Property transition_timing_function As String
            Get
                Return Me.Item("transition-timing-function")
            End Get
            Set(value As String)
                Me.Item("transition-timing-function") = value
            End Set
        End Property

        Public Property vertical_align As String
            Get
                Return Me.Item("vertical-align")
            End Get
            Set(value As String)
                Me.Item("vertical-align") = value
            End Set
        End Property

        Public Property visibility As String
            Get
                Return Me.Item("visibility")
            End Get
            Set(value As String)
                Me.Item("visibility") = value
            End Set
        End Property

        Public Property white_space As String
            Get
                Return Me.Item("white-space")
            End Get
            Set(value As String)
                Me.Item("white-space") = value
            End Set
        End Property

        Public Property width As String
            Get
                Return Me.Item("width")
            End Get
            Set(value As String)
                Me.Item("width") = value
            End Set
        End Property

        Public Property word_break As String
            Get
                Return Me.Item("word-break")
            End Get
            Set(value As String)
                Me.Item("word-break") = value
            End Set
        End Property

        Public Property word_spacing As String
            Get
                Return Me.Item("word-spacing")
            End Get
            Set(value As String)
                Me.Item("word-spacing") = value
            End Set
        End Property

        Public Property word_wrap As String
            Get
                Return Me.Item("word-wrap")
            End Get
            Set(value As String)
                Me.Item("word-wrap") = value
            End Set
        End Property

        Public Property z_index As String
            Get
                Return Me.Item("z-index")
            End Get
            Set(value As String)
                Me.Item("z-index") = value
            End Set
        End Property


    End Class

End Namespace