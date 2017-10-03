Namespace HtmlParser

    Public Class CssProperties

        Private mElement As HtmlElement

        Friend Sub New(Element As HtmlElement)
            Me.mElement = Element
        End Sub

        Function Add(Key As String, Value As String) As CssProperties
            Item(Key) = Value
            Return Me
        End Function

        ''' <summary>
        ''' Gets or sets the style of element
        ''' </summary>
        ''' <param name="Name">Name of CSS style</param>
        ''' <returns></returns>
        Default Property Item(Name As String) As String
            Get
                If Name.IsNotBlank Then
                    Dim s = mElement.Attribute("style")
                    If s.IsNotBlank Then
                        Dim styledic As New Dictionary(Of String, String)
                        For Each Item In s.Split(";")
                            Dim n = Item.Split(":")
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

                If s.IsNotBlank Then
                    For Each i In s.Split(";")
                        Dim n = i.Split(":")
                        styledic.Add(n(0).ToLower, n(1).ToLower)
                    Next
                    If styledic.ContainsKey(Name.ToLower) Then
                        styledic(Name.ToLower) = value
                    End If
                End If
                Dim p = ""
                For Each k In styledic
                    p.Append(k.Key.ToLower & ":" & k.Value.ToLower & ";")
                Next
                mElement.Attribute("style") = p
            End Set
        End Property


        Public Property align_content As String
            Get
                Return mElement.Style("align-content")
            End Get
            Set(value As String)
                mElement.Style("align-content") = value
            End Set
        End Property

        Public Property align_items As String
            Get
                Return mElement.Style("align-items")
            End Get
            Set(value As String)
                mElement.Style("align-items") = value
            End Set
        End Property

        Public Property align_self As String
            Get
                Return mElement.Style("align-self")
            End Get
            Set(value As String)
                mElement.Style("align-self") = value
            End Set
        End Property

        Public Property animation As String
            Get
                Return mElement.Style("animation")
            End Get
            Set(value As String)
                mElement.Style("animation") = value
            End Set
        End Property

        Public Property animation_delay As String
            Get
                Return mElement.Style("animation-delay")
            End Get
            Set(value As String)
                mElement.Style("animation-delay") = value
            End Set
        End Property

        Public Property animation_direction As String
            Get
                Return mElement.Style("animation-direction")
            End Get
            Set(value As String)
                mElement.Style("animation-direction") = value
            End Set
        End Property

        Public Property animation_duration As String
            Get
                Return mElement.Style("animation-duration")
            End Get
            Set(value As String)
                mElement.Style("animation-duration") = value
            End Set
        End Property

        Public Property animation_fill_mode As String
            Get
                Return mElement.Style("animation-fill-mode")
            End Get
            Set(value As String)
                mElement.Style("animation-fill-mode") = value
            End Set
        End Property

        Public Property animation_iteration_count As String
            Get
                Return mElement.Style("animation-iteration-count")
            End Get
            Set(value As String)
                mElement.Style("animation-iteration-count") = value
            End Set
        End Property

        Public Property animation_name As String
            Get
                Return mElement.Style("animation-name")
            End Get
            Set(value As String)
                mElement.Style("animation-name") = value
            End Set
        End Property

        Public Property animation_play_state As String
            Get
                Return mElement.Style("animation-play-state")
            End Get
            Set(value As String)
                mElement.Style("animation-play-state") = value
            End Set
        End Property

        Public Property animation_timing_function As String
            Get
                Return mElement.Style("animation-timing-function")
            End Get
            Set(value As String)
                mElement.Style("animation-timing-function") = value
            End Set
        End Property

        Public Property backface_visibility As String
            Get
                Return mElement.Style("backface-visibility")
            End Get
            Set(value As String)
                mElement.Style("backface-visibility") = value
            End Set
        End Property

        Public Property background As String
            Get
                Return mElement.Style("background")
            End Get
            Set(value As String)
                mElement.Style("background") = value
            End Set
        End Property

        Public Property background_attachment As String
            Get
                Return mElement.Style("background-attachment")
            End Get
            Set(value As String)
                mElement.Style("background-attachment") = value
            End Set
        End Property

        Public Property background_clip As String
            Get
                Return mElement.Style("background-clip")
            End Get
            Set(value As String)
                mElement.Style("background-clip") = value
            End Set
        End Property

        Public Property background_color As String
            Get
                Return mElement.Style("background-color")
            End Get
            Set(value As String)
                mElement.Style("background-color") = value
            End Set
        End Property

        Public Property background_image As String
            Get
                Return mElement.Style("background-image")
            End Get
            Set(value As String)
                mElement.Style("background-image") = value
            End Set
        End Property

        Public Property background_origin As String
            Get
                Return mElement.Style("background-origin")
            End Get
            Set(value As String)
                mElement.Style("background-origin") = value
            End Set
        End Property

        Public Property background_position As String
            Get
                Return mElement.Style("background-position")
            End Get
            Set(value As String)
                mElement.Style("background-position") = value
            End Set
        End Property

        Public Property background_repeat As String
            Get
                Return mElement.Style("background-repeat")
            End Get
            Set(value As String)
                mElement.Style("background-repeat") = value
            End Set
        End Property

        Public Property background_size As String
            Get
                Return mElement.Style("background-size")
            End Get
            Set(value As String)
                mElement.Style("background-size") = value
            End Set
        End Property

        Public Property border As String
            Get
                Return mElement.Style("border")
            End Get
            Set(value As String)
                mElement.Style("border") = value
            End Set
        End Property

        Public Property border_bottom As String
            Get
                Return mElement.Style("border-bottom")
            End Get
            Set(value As String)
                mElement.Style("border-bottom") = value
            End Set
        End Property

        Public Property border_bottom_color As String
            Get
                Return mElement.Style("border-bottom-color")
            End Get
            Set(value As String)
                mElement.Style("border-bottom-color") = value
            End Set
        End Property

        Public Property border_bottom_left_radius As String
            Get
                Return mElement.Style("border-bottom-left-radius")
            End Get
            Set(value As String)
                mElement.Style("border-bottom-left-radius") = value
            End Set
        End Property

        Public Property border_bottom_right_radius As String
            Get
                Return mElement.Style("border-bottom-right-radius")
            End Get
            Set(value As String)
                mElement.Style("border-bottom-right-radius") = value
            End Set
        End Property

        Public Property border_bottom_style As String
            Get
                Return mElement.Style("border-bottom-style")
            End Get
            Set(value As String)
                mElement.Style("border-bottom-style") = value
            End Set
        End Property

        Public Property border_bottom_width As String
            Get
                Return mElement.Style("border-bottom-width")
            End Get
            Set(value As String)
                mElement.Style("border-bottom-width") = value
            End Set
        End Property

        Public Property border_collapse As String
            Get
                Return mElement.Style("border-collapse")
            End Get
            Set(value As String)
                mElement.Style("border-collapse") = value
            End Set
        End Property

        Public Property border_color As String
            Get
                Return mElement.Style("border-color")
            End Get
            Set(value As String)
                mElement.Style("border-color") = value
            End Set
        End Property

        Public Property border_image As String
            Get
                Return mElement.Style("border-image")
            End Get
            Set(value As String)
                mElement.Style("border-image") = value
            End Set
        End Property

        Public Property border_image_outset As String
            Get
                Return mElement.Style("border-image-outset")
            End Get
            Set(value As String)
                mElement.Style("border-image-outset") = value
            End Set
        End Property

        Public Property border_image_repeat As String
            Get
                Return mElement.Style("border-image-repeat")
            End Get
            Set(value As String)
                mElement.Style("border-image-repeat") = value
            End Set
        End Property

        Public Property border_image_slice As String
            Get
                Return mElement.Style("border-image-slice")
            End Get
            Set(value As String)
                mElement.Style("border-image-slice") = value
            End Set
        End Property

        Public Property border_image_source As String
            Get
                Return mElement.Style("border-image-source")
            End Get
            Set(value As String)
                mElement.Style("border-image-source") = value
            End Set
        End Property

        Public Property border_image_width As String
            Get
                Return mElement.Style("border-image-width")
            End Get
            Set(value As String)
                mElement.Style("border-image-width") = value
            End Set
        End Property

        Public Property border_left As String
            Get
                Return mElement.Style("border-left")
            End Get
            Set(value As String)
                mElement.Style("border-left") = value
            End Set
        End Property

        Public Property border_left_color As String
            Get
                Return mElement.Style("border-left-color")
            End Get
            Set(value As String)
                mElement.Style("border-left-color") = value
            End Set
        End Property

        Public Property border_left_style As String
            Get
                Return mElement.Style("border-left-style")
            End Get
            Set(value As String)
                mElement.Style("border-left-style") = value
            End Set
        End Property

        Public Property border_left_width As String
            Get
                Return mElement.Style("border-left-width")
            End Get
            Set(value As String)
                mElement.Style("border-left-width") = value
            End Set
        End Property

        Public Property border_radius As String
            Get
                Return mElement.Style("border-radius")
            End Get
            Set(value As String)
                mElement.Style("border-radius") = value
            End Set
        End Property

        Public Property border_right As String
            Get
                Return mElement.Style("border-right")
            End Get
            Set(value As String)
                mElement.Style("border-right") = value
            End Set
        End Property

        Public Property border_right_color As String
            Get
                Return mElement.Style("border-right-color")
            End Get
            Set(value As String)
                mElement.Style("border-right-color") = value
            End Set
        End Property

        Public Property border_right_style As String
            Get
                Return mElement.Style("border-right-style")
            End Get
            Set(value As String)
                mElement.Style("border-right-style") = value
            End Set
        End Property

        Public Property border_right_width As String
            Get
                Return mElement.Style("border-right-width")
            End Get
            Set(value As String)
                mElement.Style("border-right-width") = value
            End Set
        End Property

        Public Property border_spacing As String
            Get
                Return mElement.Style("border-spacing")
            End Get
            Set(value As String)
                mElement.Style("border-spacing") = value
            End Set
        End Property

        Public Property border_style As String
            Get
                Return mElement.Style("border-style")
            End Get
            Set(value As String)
                mElement.Style("border-style") = value
            End Set
        End Property

        Public Property border_top As String
            Get
                Return mElement.Style("border-top")
            End Get
            Set(value As String)
                mElement.Style("border-top") = value
            End Set
        End Property

        Public Property border_top_color As String
            Get
                Return mElement.Style("border-top-color")
            End Get
            Set(value As String)
                mElement.Style("border-top-color") = value
            End Set
        End Property

        Public Property border_top_left_radius As String
            Get
                Return mElement.Style("border-top-left-radius")
            End Get
            Set(value As String)
                mElement.Style("border-top-left-radius") = value
            End Set
        End Property

        Public Property border_top_right_radius As String
            Get
                Return mElement.Style("border-top-right-radius")
            End Get
            Set(value As String)
                mElement.Style("border-top-right-radius") = value
            End Set
        End Property

        Public Property border_top_style As String
            Get
                Return mElement.Style("border-top-style")
            End Get
            Set(value As String)
                mElement.Style("border-top-style") = value
            End Set
        End Property

        Public Property border_top_width As String
            Get
                Return mElement.Style("border-top-width")
            End Get
            Set(value As String)
                mElement.Style("border-top-width") = value
            End Set
        End Property

        Public Property border_width As String
            Get
                Return mElement.Style("border-width")
            End Get
            Set(value As String)
                mElement.Style("border-width") = value
            End Set
        End Property

        Public Property bottom As String
            Get
                Return mElement.Style("bottom")
            End Get
            Set(value As String)
                mElement.Style("bottom") = value
            End Set
        End Property

        Public Property box_shadow As String
            Get
                Return mElement.Style("box-shadow")
            End Get
            Set(value As String)
                mElement.Style("box-shadow") = value
            End Set
        End Property

        Public Property box_sizing As String
            Get
                Return mElement.Style("box-sizing")
            End Get
            Set(value As String)
                mElement.Style("box-sizing") = value
            End Set
        End Property

        Public Property caption_side As String
            Get
                Return mElement.Style("caption-side")
            End Get
            Set(value As String)
                mElement.Style("caption-side") = value
            End Set
        End Property

        Public Property clear As String
            Get
                Return mElement.Style("clear")
            End Get
            Set(value As String)
                mElement.Style("clear") = value
            End Set
        End Property

        Public Property clip As String
            Get
                Return mElement.Style("clip")
            End Get
            Set(value As String)
                mElement.Style("clip") = value
            End Set
        End Property

        Public Property color As String
            Get
                Return mElement.Style("color")
            End Get
            Set(value As String)
                mElement.Style("color") = value
            End Set
        End Property

        Public Property column_count As String
            Get
                Return mElement.Style("column-count")
            End Get
            Set(value As String)
                mElement.Style("column-count") = value
            End Set
        End Property

        Public Property column_fill As String
            Get
                Return mElement.Style("column-fill")
            End Get
            Set(value As String)
                mElement.Style("column-fill") = value
            End Set
        End Property

        Public Property column_gap As String
            Get
                Return mElement.Style("column-gap")
            End Get
            Set(value As String)
                mElement.Style("column-gap") = value
            End Set
        End Property

        Public Property column_rule As String
            Get
                Return mElement.Style("column-rule")
            End Get
            Set(value As String)
                mElement.Style("column-rule") = value
            End Set
        End Property

        Public Property column_rule_color As String
            Get
                Return mElement.Style("column-rule-color")
            End Get
            Set(value As String)
                mElement.Style("column-rule-color") = value
            End Set
        End Property

        Public Property column_rule_style As String
            Get
                Return mElement.Style("column-rule-style")
            End Get
            Set(value As String)
                mElement.Style("column-rule-style") = value
            End Set
        End Property

        Public Property column_rule_width As String
            Get
                Return mElement.Style("column-rule-width")
            End Get
            Set(value As String)
                mElement.Style("column-rule-width") = value
            End Set
        End Property

        Public Property column_span As String
            Get
                Return mElement.Style("column-span")
            End Get
            Set(value As String)
                mElement.Style("column-span") = value
            End Set
        End Property

        Public Property column_width As String
            Get
                Return mElement.Style("column-width")
            End Get
            Set(value As String)
                mElement.Style("column-width") = value
            End Set
        End Property

        Public Property columns As String
            Get
                Return mElement.Style("columns")
            End Get
            Set(value As String)
                mElement.Style("columns") = value
            End Set
        End Property

        Public Property content As String
            Get
                Return mElement.Style("content")
            End Get
            Set(value As String)
                mElement.Style("content") = value
            End Set
        End Property

        Public Property counter_increment As String
            Get
                Return mElement.Style("counter-increment")
            End Get
            Set(value As String)
                mElement.Style("counter-increment") = value
            End Set
        End Property

        Public Property counter_reset As String
            Get
                Return mElement.Style("counter-increment")
            End Get
            Set(value As String)
                mElement.Style("counter-increment") = value
            End Set
        End Property

        Public Property cursor As String
            Get
                Return mElement.Style("cursor")
            End Get
            Set(value As String)
                mElement.Style("cursor") = value
            End Set
        End Property

        Public Property direction As String
            Get
                Return mElement.Style("direction")
            End Get
            Set(value As String)
                mElement.Style("direction") = value
            End Set
        End Property

        Public Property display As String
            Get
                Return mElement.Style("display")
            End Get
            Set(value As String)
                mElement.Style("display") = value
            End Set
        End Property

        Public Property empty_cells As String
            Get
                Return mElement.Style("empty-cells")
            End Get
            Set(value As String)
                mElement.Style("empty-cells") = value
            End Set
        End Property

        Public Property flex As String
            Get
                Return mElement.Style("flex")
            End Get
            Set(value As String)
                mElement.Style("flex") = value
            End Set
        End Property

        Public Property flex_basis As String
            Get
                Return mElement.Style("flex-basis")
            End Get
            Set(value As String)
                mElement.Style("flex-basis") = value
            End Set
        End Property

        Public Property flex_direction As String
            Get
                Return mElement.Style("flex-direction")
            End Get
            Set(value As String)
                mElement.Style("flex-direction") = value
            End Set
        End Property

        Public Property flex_flow As String
            Get
                Return mElement.Style("flex-flow")
            End Get
            Set(value As String)
                mElement.Style("flex-flow") = value
            End Set
        End Property

        Public Property flex_grow As String
            Get
                Return mElement.Style("flex-grow")
            End Get
            Set(value As String)
                mElement.Style("flex-grow") = value
            End Set
        End Property

        Public Property flex_shrink As String
            Get
                Return mElement.Style("flex-shrink")
            End Get
            Set(value As String)
                mElement.Style("flex-shrink") = value
            End Set
        End Property

        Public Property flex_wrap As String
            Get
                Return mElement.Style("flex-wrap")
            End Get
            Set(value As String)
                mElement.Style("flex-wrap") = value
            End Set
        End Property

        Public Property float As String
            Get
                Return mElement.Style("float")
            End Get
            Set(value As String)
                mElement.Style("float") = value
            End Set
        End Property

        Public Property font As String
            Get
                Return mElement.Style("font")
            End Get
            Set(value As String)
                mElement.Style("font") = value
            End Set
        End Property

        Public Property font_family As String
            Get
                Return mElement.Style("font-family")
            End Get
            Set(value As String)
                mElement.Style("font-family") = value
            End Set
        End Property

        Public Property font_size As String
            Get
                Return mElement.Style("font-size")
            End Get
            Set(value As String)
                mElement.Style("font-size") = value
            End Set
        End Property

        Public Property font_size_adjust As String
            Get
                Return mElement.Style("font-size-adjust")
            End Get
            Set(value As String)
                mElement.Style("font-size-adjust") = value
            End Set
        End Property

        Public Property font_stretch As String
            Get
                Return mElement.Style("font-stretch")
            End Get
            Set(value As String)
                mElement.Style("font-stretch") = value
            End Set
        End Property

        Public Property font_style As String
            Get
                Return mElement.Style("font-style")
            End Get
            Set(value As String)
                mElement.Style("font-style") = value
            End Set
        End Property

        Public Property font_variant As String
            Get
                Return mElement.Style("font-variant")
            End Get
            Set(value As String)
                mElement.Style("font-variant") = value
            End Set
        End Property

        Public Property font_weight As String
            Get
                Return mElement.Style("font-weight")
            End Get
            Set(value As String)
                mElement.Style("font-weight") = value
            End Set
        End Property

        Public Property height As String
            Get
                Return mElement.Style("height")
            End Get
            Set(value As String)
                mElement.Style("height") = value
            End Set
        End Property

        Public Property justify_content As String
            Get
                Return mElement.Style("justify-content")
            End Get
            Set(value As String)
                mElement.Style("justify-content") = value
            End Set
        End Property

        Public Property left As String
            Get
                Return mElement.Style("left")
            End Get
            Set(value As String)
                mElement.Style("left") = value
            End Set
        End Property

        Public Property letter_spacing As String
            Get
                Return mElement.Style("letter-spacing")
            End Get
            Set(value As String)
                mElement.Style("letter-spacing") = value
            End Set
        End Property

        Public Property line_height As String
            Get
                Return mElement.Style("line-height")
            End Get
            Set(value As String)
                mElement.Style("line-height") = value
            End Set
        End Property

        Public Property list_style As String
            Get
                Return mElement.Style("list-style")
            End Get
            Set(value As String)
                mElement.Style("list-style") = value
            End Set
        End Property

        Public Property list_style_image As String
            Get
                Return mElement.Style("list-style-image")
            End Get
            Set(value As String)
                mElement.Style("list-style-image") = value
            End Set
        End Property

        Public Property list_style_position As String
            Get
                Return mElement.Style("list-style-position")
            End Get
            Set(value As String)
                mElement.Style("list-style-position") = value
            End Set
        End Property

        Public Property list_style_type As String
            Get
                Return mElement.Style("list-style-type")
            End Get
            Set(value As String)
                mElement.Style("list-style-type") = value
            End Set
        End Property

        Public Property margin As String
            Get
                Return mElement.Style("margin")
            End Get
            Set(value As String)
                mElement.Style("margin") = value
            End Set
        End Property

        Public Property margin_bottom As String
            Get
                Return mElement.Style("margin-bottom")
            End Get
            Set(value As String)
                mElement.Style("margin-bottom") = value
            End Set
        End Property

        Public Property margin_left As String
            Get
                Return mElement.Style("margin-left")
            End Get
            Set(value As String)
                mElement.Style("margin-left") = value
            End Set
        End Property

        Public Property margin_right As String
            Get
                Return mElement.Style("margin-right")
            End Get
            Set(value As String)
                mElement.Style("margin-right") = value
            End Set
        End Property

        Public Property margin_top As String
            Get
                Return mElement.Style("margin-top")
            End Get
            Set(value As String)
                mElement.Style("margin-top") = value
            End Set
        End Property

        Public Property max_height As String
            Get
                Return mElement.Style("max-height")
            End Get
            Set(value As String)
                mElement.Style("max-height") = value
            End Set
        End Property

        Public Property max_width As String
            Get
                Return mElement.Style("max-width")
            End Get
            Set(value As String)
                mElement.Style("max-width") = value
            End Set
        End Property

        Public Property min_height As String
            Get
                Return mElement.Style("min-height")
            End Get
            Set(value As String)
                mElement.Style("min-height") = value
            End Set
        End Property

        Public Property min_width As String
            Get
                Return mElement.Style("min-width")
            End Get
            Set(value As String)
                mElement.Style("min-width") = value
            End Set
        End Property

        Public Property opacity As String
            Get
                Return mElement.Style("opacity")
            End Get
            Set(value As String)
                mElement.Style("opacity") = value
            End Set
        End Property

        Public Property order As String
            Get
                Return mElement.Style("order")
            End Get
            Set(value As String)
                mElement.Style("order") = value
            End Set
        End Property

        Public Property outline As String
            Get
                Return mElement.Style("outline")
            End Get
            Set(value As String)
                mElement.Style("outline") = value
            End Set
        End Property

        Public Property outline_color As String
            Get
                Return mElement.Style("outline-color")
            End Get
            Set(value As String)
                mElement.Style("outline-color") = value
            End Set
        End Property

        Public Property outline_offset As String
            Get
                Return mElement.Style("outline-offset")
            End Get
            Set(value As String)
                mElement.Style("outline-offset") = value
            End Set
        End Property

        Public Property outline_style As String
            Get
                Return mElement.Style("outline-style")
            End Get
            Set(value As String)
                mElement.Style("outline-style") = value
            End Set
        End Property

        Public Property outline_width As String
            Get
                Return mElement.Style("outline-width")
            End Get
            Set(value As String)
                mElement.Style("outline-width") = value
            End Set
        End Property

        Public Property overflow As String
            Get
                Return mElement.Style("overflow")
            End Get
            Set(value As String)
                mElement.Style("overflow") = value
            End Set
        End Property

        Public Property overflow_x As String
            Get
                Return mElement.Style("overflow-x")
            End Get
            Set(value As String)
                mElement.Style("overflow-x") = value
            End Set
        End Property

        Public Property overflow_y As String
            Get
                Return mElement.Style("overflow-y")
            End Get
            Set(value As String)
                mElement.Style("overflow-y") = value
            End Set
        End Property

        Public Property padding As String
            Get
                Return mElement.Style("padding")
            End Get
            Set(value As String)
                mElement.Style("padding") = value
            End Set
        End Property

        Public Property padding_bottom As String
            Get
                Return mElement.Style("padding-bottom")
            End Get
            Set(value As String)
                mElement.Style("padding-bottom") = value
            End Set
        End Property

        Public Property padding_left As String
            Get
                Return mElement.Style("padding-left")
            End Get
            Set(value As String)
                mElement.Style("padding-left") = value
            End Set
        End Property

        Public Property padding_right As String
            Get
                Return mElement.Style("padding-right")
            End Get
            Set(value As String)
                mElement.Style("padding-right") = value
            End Set
        End Property

        Public Property padding_top As String
            Get
                Return mElement.Style("padding-top")
            End Get
            Set(value As String)
                mElement.Style("padding-top") = value
            End Set
        End Property

        Public Property page_break_after As String
            Get
                Return mElement.Style("page-break-after")
            End Get
            Set(value As String)
                mElement.Style("page-break-after") = value
            End Set
        End Property

        Public Property page_break_before As String
            Get
                Return mElement.Style("page-break-before")
            End Get
            Set(value As String)
                mElement.Style("page-break-before") = value
            End Set
        End Property

        Public Property page_break_inside As String
            Get
                Return mElement.Style("page-break-inside")
            End Get
            Set(value As String)
                mElement.Style("page-break-inside") = value
            End Set
        End Property

        Public Property perspective As String
            Get
                Return mElement.Style("perspective")
            End Get
            Set(value As String)
                mElement.Style("perspective") = value
            End Set
        End Property

        Public Property perspective_origin As String
            Get
                Return mElement.Style("perspective-origin")
            End Get
            Set(value As String)
                mElement.Style("perspective-origin") = value
            End Set
        End Property

        Public Property position As String
            Get
                Return mElement.Style("position")
            End Get
            Set(value As String)
                mElement.Style("position") = value
            End Set
        End Property

        Public Property quotes As String
            Get
                Return mElement.Style("quotes")
            End Get
            Set(value As String)
                mElement.Style("quotes") = value
            End Set
        End Property

        Public Property resize As String
            Get
                Return mElement.Style("resize")
            End Get
            Set(value As String)
                mElement.Style("resize") = value
            End Set
        End Property

        Public Property right As String
            Get
                Return mElement.Style("right")
            End Get
            Set(value As String)
                mElement.Style("right") = value
            End Set
        End Property

        Public Property tab_size As String
            Get
                Return mElement.Style("tab-size")
            End Get
            Set(value As String)
                mElement.Style("tab-size") = value
            End Set
        End Property

        Public Property table_layout As String
            Get
                Return mElement.Style("table-layout")
            End Get
            Set(value As String)
                mElement.Style("table-layout") = value
            End Set
        End Property

        Public Property text_align As String
            Get
                Return mElement.Style("text-align")
            End Get
            Set(value As String)
                mElement.Style("text-align") = value
            End Set
        End Property

        Public Property text_align_last As String
            Get
                Return mElement.Style("text-align-last")
            End Get
            Set(value As String)
                mElement.Style("text-align-last") = value
            End Set
        End Property

        Public Property text_decoration As String
            Get
                Return mElement.Style("text-decoration")
            End Get
            Set(value As String)
                mElement.Style("text-decoration") = value
            End Set
        End Property

        Public Property text_decoration_color As String
            Get
                Return mElement.Style("text-decoration-color")
            End Get
            Set(value As String)
                mElement.Style("text-decoration-color") = value
            End Set
        End Property

        Public Property text_decoration_line As String
            Get
                Return mElement.Style("text-decoration-line")
            End Get
            Set(value As String)
                mElement.Style("text-decoration-line") = value
            End Set
        End Property

        Public Property text_decoration_style As String
            Get
                Return mElement.Style("text-decoration-style")
            End Get
            Set(value As String)
                mElement.Style("text-decoration-style") = value
            End Set
        End Property

        Public Property text_indent As String
            Get
                Return mElement.Style("text-indent")
            End Get
            Set(value As String)
                mElement.Style("text-indent") = value
            End Set
        End Property

        Public Property text_justify As String
            Get
                Return mElement.Style("text-justify")
            End Get
            Set(value As String)
                mElement.Style("text-justify") = value
            End Set
        End Property

        Public Property text_overflow As String
            Get
                Return mElement.Style("text-overflow")
            End Get
            Set(value As String)
                mElement.Style("text-overflow") = value
            End Set
        End Property

        Public Property text_shadow As String
            Get
                Return mElement.Style("text-shadow")
            End Get
            Set(value As String)
                mElement.Style("text-shadow") = value
            End Set
        End Property

        Public Property text_transform As String
            Get
                Return mElement.Style("text-transform")
            End Get
            Set(value As String)
                mElement.Style("text-transform") = value
            End Set
        End Property

        Public Property top As String
            Get
                Return mElement.Style("top")
            End Get
            Set(value As String)
                mElement.Style("top") = value
            End Set
        End Property

        Public Property transform As String
            Get
                Return mElement.Style("transform")
            End Get
            Set(value As String)
                mElement.Style("transform") = value
            End Set
        End Property

        Public Property transform_origin As String
            Get
                Return mElement.Style("transform-origin")
            End Get
            Set(value As String)
                mElement.Style("transform-origin") = value
            End Set
        End Property

        Public Property transform_style As String
            Get
                Return mElement.Style("transform-style")
            End Get
            Set(value As String)
                mElement.Style("transform-style") = value
            End Set
        End Property

        Public Property transition As String
            Get
                Return mElement.Style("transition")
            End Get
            Set(value As String)
                mElement.Style("transition") = value
            End Set
        End Property

        Public Property transition_delay As String
            Get
                Return mElement.Style("transition-delay")
            End Get
            Set(value As String)
                mElement.Style("transition-delay") = value
            End Set
        End Property

        Public Property transition_duration As String
            Get
                Return mElement.Style("transition-duration")
            End Get
            Set(value As String)
                mElement.Style("transition-duration") = value
            End Set
        End Property

        Public Property transition_property As String
            Get
                Return mElement.Style("transition-property")
            End Get
            Set(value As String)
                mElement.Style("transition-property") = value
            End Set
        End Property

        Public Property transition_timing_function As String
            Get
                Return mElement.Style("transition-timing-function")
            End Get
            Set(value As String)
                mElement.Style("transition-timing-function") = value
            End Set
        End Property

        Public Property vertical_align As String
            Get
                Return mElement.Style("vertical-align")
            End Get
            Set(value As String)
                mElement.Style("vertical-align") = value
            End Set
        End Property

        Public Property visibility As String
            Get
                Return mElement.Style("visibility")
            End Get
            Set(value As String)
                mElement.Style("visibility") = value
            End Set
        End Property

        Public Property white_space As String
            Get
                Return mElement.Style("white-space")
            End Get
            Set(value As String)
                mElement.Style("white-space") = value
            End Set
        End Property

        Public Property width As String
            Get
                Return mElement.Style("width")
            End Get
            Set(value As String)
                mElement.Style("width") = value
            End Set
        End Property

        Public Property word_break As String
            Get
                Return mElement.Style("word-break")
            End Get
            Set(value As String)
                mElement.Style("word-break") = value
            End Set
        End Property

        Public Property word_spacing As String
            Get
                Return mElement.Style("word-spacing")
            End Get
            Set(value As String)
                mElement.Style("word-spacing") = value
            End Set
        End Property

        Public Property word_wrap As String
            Get
                Return mElement.Style("word-wrap")
            End Get
            Set(value As String)
                mElement.Style("word-wrap") = value
            End Set
        End Property

        Public Property z_index As String
            Get
                Return mElement.Style("z-index")
            End Get
            Set(value As String)
                mElement.Style("z-index") = value
            End Set
        End Property

    End Class

End Namespace