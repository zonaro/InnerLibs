Namespace HtmlParser
    Public MustInherit Class CssSelector
#Region "Constructor"
        Public Sub New()
            Me.SubSelectors = New List(Of CssSelector)()
        End Sub
#End Region

#Region "Properties"
        Private Shared ReadOnly s_Selectors As CssSelector() = FindSelectors()

        Public MustOverride ReadOnly Property Token() As String
        Protected Overridable ReadOnly Property IsSubSelector() As Boolean
            Get

                Return False

            End Get

        End Property

        Public Overridable ReadOnly Property AllowTraverse() As Boolean
            Get

                Return True

            End Get

        End Property


        Public Property SubSelectors() As IList(Of CssSelector)
            Get

                Return m_SubSelectors

            End Get

            Set
                m_SubSelectors = Value

            End Set

        End Property

        Private m_SubSelectors As IList(Of CssSelector)
        Public Property Selector() As String
            Get

                Return m_Selector

            End Get

            Set
                m_Selector = Value

            End Set

        End Property

        Private m_Selector As String
#End Region

#Region "Methods"
        Protected Friend MustOverride Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection


        Friend Function Filter(currentNodes As HtmlNodeCollection) As HtmlNodeCollection

            Dim nodes = currentNodes

            Dim rt As New HtmlNodeCollection

            rt.AddRange(Me.FilterCore(nodes).Distinct())


            If Me.SubSelectors.Count = 0 Then
                Return rt
            End If

            For Each ss In Me.SubSelectors
                rt = ss.FilterCore(rt)
            Next

            Return rt
        End Function

        Friend Overridable Function GetSelectorParameter(selector As String) As String

            Return selector.Substring(Me.Token.Length)

        End Function


        Friend Shared Function Parse(cssSelector As String) As IList(Of CssSelector)

            Dim rt = New List(Of CssSelector)()
            Dim tokens = Tokenizer.GetTokens(cssSelector)
            For Each tk In tokens

                rt.Add(ParseSelector(tk))
            Next

            Return rt
        End Function

        Private Shared Function ParseSelector(token As Token) As CssSelector

            Dim selectorType As Type

            Dim selector As CssSelector


            If Char.IsLetter(token.Filter(0)) Then
                selector = s_Selectors.First(Function(i) TypeOf i Is Selectors.TagNameSelector)

            Else
                selector = s_Selectors.Where(Function(s) s.Token.Length > 0).FirstOrDefault(Function(s) token.Filter.StartsWith(s.Token))

            End If


            If selector Is Nothing Then
                Throw New InvalidOperationException("Invalid token: " + token.Filter)

            End If


            selectorType = selector.[GetType]()
            Dim rt = DirectCast(Activator.CreateInstance(selectorType), CssSelector)


            Dim filter As String = token.Filter.Substring(selector.Token.Length)

            rt.SubSelectors = token.SubTokens.[Select](Function(i) CssSelector.ParseSelector(i)).ToList()


            rt.Selector = filter
            Return rt
        End Function

        Private Shared Function FindSelectors() As CssSelector()

            Dim defaultAsm = GetType(CssSelector).Assembly

            Dim typeQuery As Func(Of Type, Boolean) = Function(type) type.IsSubclassOf(GetType(CssSelector)) AndAlso Not type.IsAbstract

            Dim defaultTypes = defaultAsm.GetTypes().Where(typeQuery)

            Dim types = Reflection.Assembly.GetExecutingAssembly.GetTypes().Where(typeQuery)

            types = defaultTypes.Concat(types)

            Dim rt = types.[Select](Function(t) Activator.CreateInstance(t)).Cast(Of CssSelector)().ToArray()

            Return rt

        End Function

#End Region
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
