Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace HtmlParser

    Public Class HtmlAnchor
        Inherits HtmlElement

        Overrides Property Name As String
            Get
                Return "a"
            End Get
            Set(value As String)
                MyBase.Name = "a"
            End Set
        End Property

        Sub New()
            MyBase.New("a")
        End Sub

        Sub New(Url As String, Text As String)
            MyBase.New("a")
            Me.Href = Url
            Me.InnerHTML = Text
        End Sub

        Property Target As String
            Get
                Return Me.Attribute("target")
            End Get
            Set(value As String)
                If value Is Nothing Then
                    Me.RemoveAttribute("target")
                Else
                    Me.Attribute("target") = value
                End If
            End Set
        End Property

        Property Href As String
            Get
                Return Me.Attribute("href")
            End Get
            Set(value As String)
                Me.Attribute("href") = value
            End Set
        End Property

    End Class

    Public Class HtmlImage
        Inherits HtmlElement

        Overrides Property Name As String
            Get
                Return "img"
            End Get
            Set(value As String)
                MyBase.Name = "img"
            End Set
        End Property

        Sub New()
            MyBase.New("img")
        End Sub

        Sub New(Url As String)
            Me.New
            Me.Src = Url
        End Sub

        Sub New(Image As Image)
            Me.New
            Me.Src = Image.ToDataURL
        End Sub

        Property Src As String
            Get
                Return Me.Attribute("src")
            End Get
            Set(value As String)
                If value Is Nothing Then
                    Me.RemoveAttribute("src")
                End If
                Me.Attribute("src") = value
            End Set
        End Property

    End Class


    Public Class HtmlInput
        Inherits HtmlElement

        Overrides Property Name As String
            Get
                Return "input"
            End Get
            Set(value As String)
                MyBase.Name = "input"
            End Set
        End Property
        Enum HtmlInputType
            text
            button
            checkbox
            color
            [date]
            datetime_local
            email
            file
            hidden
            image
            month
            number
            password
            radio
            range
            reset
            search
            submit
            tel
            time
            url
            week
        End Enum

        Sub New(Type As HtmlInputType, Optional Value As String = Nothing)
            MyBase.New("input")
            mIsExplicitlyTerminated = True
            Me.Value = Value
            Me.Type = Type
        End Sub

        ''' <summary>
        ''' Type of Input
        ''' </summary>
        ''' <returns></returns>
        Property Type As HtmlInputType
            Get
                Return GetEnumValue(Of HtmlInputType)(Me.Attribute("type"))
            End Get
            Set(value As HtmlInputType)
                Me.Attribute("type") = [Enum].GetName(GetType(HtmlInputType), value)
            End Set
        End Property

        ''' <summary>
        ''' Value of Input
        ''' </summary>
        ''' <returns></returns>
        Property Value As String
            Get
                Return Me.Attribute("value")
            End Get
            Set(value As String)
                Me.Attribute("value") = ("" & value)
            End Set
        End Property

    End Class

    Public Class HtmlSelectElement
        Inherits HtmlElement

        ''' <summary>
        ''' Returns the name of element (OL or UL)
        ''' </summary>
        ''' <returns></returns>
        Overrides Property Name As String
            Get
                Return "select"
            End Get
            Set(value As String)
                MyBase.Name = "select"
            End Set
        End Property

        ''' <summary>
        ''' Create a select element
        ''' </summary>
        Sub New()
            MyBase.New("select")
        End Sub

        ''' <summary>
        ''' Add a option to this list
        ''' </summary>
        ''' <param name="Option"></param>
        Public Sub AddOption([Option] As HtmlOptionElement)
            Me.Nodes.Add([Option])
        End Sub

        Public ReadOnly Property Groups As IEnumerable(Of String)
            Get
                Return Me("option").Select(Function(a As HtmlOptionElement) a.Group).Distinct
            End Get
        End Property

        ''' <summary>
        ''' Redefines the node elements
        ''' </summary>
        Public Sub Organize()
            If Groups.Count > 0 Then
                Dim opts = Me("option")
                For Each group In Groups
                    Dim d As New HtmlElement("optgroup")
                    d.IsExplicitlyTerminated = True
                    d.Attribute("label") = group
                    Me.Nodes.Add(d)
                Next
                For Each opt In Me("option")
                    Dim o = CType(opt, HtmlOptionElement)
                    If o.Group.IsNotBlank Then
                        Dim destination = Me("optgroup[label=" & CType(opt, HtmlOptionElement).Group.Quote & "]").First
                        o.Move(destination)
                    Else
                        o.Move(Me)
                    End If
                Next
            End If
        End Sub

    End Class

    Public Class HtmlOptionElement
        Inherits HtmlElement

        Sub New()
            MyBase.New("option")
        End Sub

        Sub New(Text As String)
            MyBase.New("option", Text.RemoveHTML)
        End Sub

        Sub New(Text As String, Value As String)
            MyBase.New("option", Text.RemoveHTML)
            Me.Attribute("value") = Value
        End Sub

        Property Group As String = ""

    End Class

    Public Class HtmlListElement
        Inherits HtmlElement

        Property IsOrdenedList As Boolean

        ''' <summary>
        ''' Returns the name of element (OL or UL)
        ''' </summary>
        ''' <returns></returns>
        Shadows Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = If(IsOrdenedList, "ol", "ul")
            End Set
        End Property

        ''' <summary>
        ''' Create a List element (OL or UL)
        ''' </summary>
        ''' <param name="OrdenedList"></param>
        Sub New(Optional OrdenedList As Boolean = False)
            MyBase.New(If(OrdenedList, "ol", "ul"))
            IsOrdenedList = OrdenedList
        End Sub

        ''' <summary>
        ''' Add a LI to this list
        ''' </summary>
        ''' <param name="Text"></param>
        Public Sub Add(Text As String)
            Me.Nodes.Add(New HtmlElement("li", Text) With {.IsExplicitlyTerminated = True})
        End Sub

        ''' <summary>
        ''' Add a LI to this list
        ''' </summary>
        ''' <param name="Content"></param>
        Public Sub Add(ParamArray Content As HtmlNode())
            Dim d = New HtmlElement("li")
            d.IsExplicitlyTerminated = True
            For Each i In Content
                i.Move(d)
            Next
            Me.Nodes.Add(d)
        End Sub

    End Class


    Public Module MentionParser

        <Extension()>
        Function ParseURL(ByVal Text As String, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text, "(http(s)?://)?([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?", New MatchEvaluator(Function(x) Method(x.ToString)))
        End Function

        ''' <summary>
        ''' Localiza emojis no texto e automaticamente executa uma função de replace para cada emoji encontrado
        ''' </summary>
        ''' <param name="Text">  Texto</param>
        ''' <param name="Method"></param>

        <Extension()>
        Function ParseEmoji(ByVal Text As String, Optional Method As Func(Of String, String) = Nothing) As String
            Method = If(Method, Function(x) Emoji.GetByName(x))
            Return Regex.Replace(Text, "(:)((?:[A-Za-z0-9-_]*))(:)", New MatchEvaluator(Function(x) Method(x.ToString.GetBetween(":", ":"))))
        End Function

        ''' <summary>
        ''' Localiza menções a usuários no texto e automaticamente executa uma função de replace para
        ''' cada hashtag encontrada
        ''' </summary>
        ''' <param name="Text">  Texto</param>
        ''' <param name="Method"></param>

        <Extension()>
        Function ParseUsername(ByVal Text As String, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text, "(@)((?:[A-Za-z0-9-_]*))", New MatchEvaluator(Function(x) Method(x.ToString.RemoveFirstIf("@"))))
        End Function

        ''' <summary>
        ''' Localiza hashtags no texto e automaticamente executa uma função de replace para cada
        ''' hashtag encontrada
        ''' </summary>
        ''' <param name="Text">  Texto</param>
        ''' <param name="Method"></param>

        <Extension()>
        Function ParseHashtag(ByVal Text As String, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text.ToString, "(#)((?:[A-Za-z0-9-_]*))", New MatchEvaluator(Function(x) Method(x.ToString.RemoveFirstIf("#"))))
        End Function

        ''' <summary>
        ''' Cria um elemento de Ancora (a) a partir de uma string com URL. O titulo é obtido
        ''' automaticamente da url quando possivel. Se a string não for uma URL válida uma ancora com
        ''' o proprio texto é criada.
        ''' </summary>
        ''' <param name="URL">URL</param>
        ''' <returns></returns>
        <Extension()>
        Function CreateAnchor(URL As String, Optional Target As String = "_blank") As HtmlAnchor
            If URL.IsURL Then
                Try
                    Return New HtmlAnchor(URL, BrowserClipper.GetTitle(URL)) With {.Target = Target}
                Catch ex As Exception
                    Return New HtmlAnchor(URL, URL) With {.Target = Target}
                End Try
            End If
            Return New HtmlAnchor("javascript:void(0);", URL) With {.Target = Target}
        End Function

    End Module

    Public NotInheritable Class Emoji

        Public Shared Function GetList() As Dictionary(Of String, String)
            Dim l = GetType(Emoji).GetFields
            Return l.Select(Function(x) New KeyValuePair(Of String, String)(x.Name.RemoveAny("_").ToLower, x.GetValue(Nothing).ToString)).ToDictionary
        End Function

        Public Shared Function GetByName(Name As String) As String
            GetByName = ""
            GetList.TryGetValue(Name.ToLower.Replacenone("_"), GetByName)
            Return GetByName & ""
        End Function

        Public Shared Function ReplaceFaces(ByRef Tag As HtmlElement, Optional Method As Func(Of String, String) = Nothing)
            Tag.GetTextElements.ForEach(Sub(b As HtmlText) b.Text = Emoji.ReplaceFaces(b.Text))
            Return Tag.InnerHTML
        End Function

        Public Shared Function ReplaceFaces(ByVal Text As String, Optional Method As Func(Of String, String) = Nothing) As String
            Dim dic As New Dictionary(Of String(), String)

            Text = Text.ParseEmoji(Method)

            dic.Add({"0=)", "0=)", "0:)", "0:)"}, ":innocent:")
            dic.Add({"=)", ":)"}, ":smile:")
            dic.Add({";)"}, ":wink:")
            dic.Add({"¬¬"}, ":unamused:")
            dic.Add({";P"}, ":stuckouttonguewinkingeye:")
            dic.Add({"=D", ":D"}, ":smiley:")
            dic.Add({"=B", ":B"}, ":grimacing:")
            dic.Add({"XD", "xD"}, ":laughing:")
            dic.Add({"=O", ":O"}, ":openmouth:")
            dic.Add({"=(", ":("}, ":frowning:")
            dic.Add({"'-'", "`-`", ":|", "=|"}, ":neutralface:")
            dic.Add({"{<3}"}, ":heartbeat:")
            dic.Add({"[<3]"}, ":giftheart:")
            dic.Add({"<3<3"}, ":twohearts:")
            dic.Add({"<3b"}, ":blueheart:")
            dic.Add({"<3g"}, ":greenheart:")
            dic.Add({"<3y"}, ":yellowheart:")
            dic.Add({"<3p"}, ":purpleheart:")
            dic.Add({"<3p"}, ":purpleheart:")
            dic.Add({"</3"}, ":brokenheart:")
            dic.Add({"<i3"}, ":cupid:")
            dic.Add({"<3"}, ":heart:")
            dic.Add({"=p", ":p"}, ":yum:")
            dic.Add({"=P", ":P"}, ":stuckouttongue:")
            dic.Add({"=*"}, ":kissingheart:")
            dic.Add({":*"}, ":kissing:")
            dic.Add({":@", "=@"}, ":angry:")
            dic.Add({":'("}, ":cry:")
            dic.Add({"='O"}, ":sob:")
            dic.Add({"$$"}, ":heavydollarsign:")
            dic.Add({"zzzz"}, ":sleeping:")
            dic.Add({"zzz"}, ":zzz:")
            dic.Add({"zz"}, ":sleepy:")
            dic.Add({"*-*"}, ":dizzyface:")
            dic.Add({"-_-"}, ":expressionless:")
            dic.Add({"=/", ":/"}, ":confused:")
            dic.Add({"B)"}, ":sunglasses:")
            dic.Add({"kkk"}, ":laughing:")
            dic.Add({"8O"}, ":astonished:")
            dic.Add({":E"}, ":grin:")
            dic.Add({"(y)"}, ":thumbsup:")
            dic.Add({"(8)"}, ":musicalnote:")

            Return Text.ReplaceFrom(dic, StringComparison.InvariantCulture).ParseEmoji(Method)
        End Function


        Public Const Hash As String = "#"

        Public Const Zero As String = "0"

        Public Const One As String = "1"

        Public Const Two As String = "2"

        Public Const Three As String = "3"

        Public Const Four As String = "4"

        Public Const Five As String = "5"

        Public Const Six As String = "6"

        Public Const Seven As String = "7"

        Public Const Eight As String = "8"

        Public Const Nine As String = "9"

        Public Const Copyright As String = "©"

        Public Const Registered As String = "®"

        Public Const Bangbang As String = "‼"

        Public Const Interrobang As String = "⁉"

        Public Const Tm As String = "™"

        Public Const Information_Source As String = "ℹ"

        Public Const Left_Right_Arrow As String = "↔"

        Public Const Arrow_Up_Down As String = "↕"

        Public Const Arrow_Upper_Left As String = "↖"

        Public Const Arrow_Upper_Right As String = "↗"

        Public Const Arrow_Lower_Right As String = "↘"

        Public Const Arrow_Lower_Left As String = "↙"

        Public Const Leftwards_Arrow_With_Hook As String = "↩"

        Public Const Arrow_Right_Hook As String = "↪"

        Public Const Watch As String = "⌚"

        Public Const Hourglass As String = "⌛"

        Public Const Fast_Forward As String = ChrW(9193)

        Public Const Rewind As String = ChrW(9194)

        Public Const Arrow_Double_Up As String = ChrW(9195)

        Public Const Arrow_Double_Down As String = ChrW(9196)

        Public Const Alarm_Clock As String = ChrW(9200)

        Public Const Hourglass_Flowing_Sand As String = ChrW(9203)

        Public Const M As String = "Ⓜ"

        Public Const Black_Small_Square As String = "▪"

        Public Const White_Small_Square As String = "▫"

        Public Const Arrow_Forward As String = "▶"

        Public Const Arrow_Backward As String = "◀"

        Public Const White_Medium_Square As String = "◻"

        Public Const Black_Medium_Square As String = "◼"

        Public Const White_Medium_Small_Square As String = "◽"

        Public Const Black_Medium_Small_Square As String = "◾"

        Public Const Sunny As String = "☀"

        Public Const Cloud As String = "☁"

        Public Const Telephone As String = "☎"

        Public Const Ballot_Box_With_Check As String = "☑"

        Public Const Umbrella As String = "☔"

        Public Const Coffee As String = "☕"

        Public Const Point_Up As String = "☝"

        Public Const Relaxed As String = "☺"

        Public Const Aries As String = "♈"

        Public Const Taurus As String = "♉"

        Public Const Gemini As String = "♊"

        Public Const Cancer As String = "♋"

        Public Const Leo As String = "♌"

        Public Const Virgo As String = "♍"

        Public Const Libra As String = "♎"

        Public Const Scorpius As String = "♏"

        Public Const Sagittarius As String = "♐"

        Public Const Capricorn As String = "♑"

        Public Const Aquarius As String = "♒"

        Public Const Pisces As String = "♓"

        Public Const Spades As String = "♠"

        Public Const Clubs As String = "♣"

        Public Const Hearts As String = "♥"

        Public Const Diamonds As String = "♦"

        Public Const Hotsprings As String = "♨"

        Public Const Recycle As String = "♻"

        Public Const Wheelchair As String = "♿"

        Public Const Anchor As String = "⚓"

        Public Const Warning As String = "⚠"

        Public Const Zap As String = "⚡"

        Public Const White_Circle As String = "⚪"

        Public Const Black_Circle As String = "⚫"

        Public Const Soccer As String = ChrW(9917)

        Public Const Baseball As String = ChrW(9918)

        Public Const Snowman As String = ChrW(9924)

        Public Const Partly_Sunny As String = ChrW(9925)

        Public Const Ophiuchus As String = ChrW(9934)

        Public Const No_Entry As String = ChrW(9940)

        Public Const Church As String = ChrW(9962)

        Public Const Fountain As String = ChrW(9970)

        Public Const Golf As String = ChrW(9971)

        Public Const Sailboat As String = ChrW(9973)

        Public Const Tent As String = ChrW(9978)

        Public Const Fuelpump As String = ChrW(9981)

        Public Const Scissors As String = "✂"

        Public Const White_Check_Mark As String = ChrW(9989)

        Public Const Airplane As String = "✈"

        Public Const Envelope As String = "✉"

        Public Const Fist As String = ChrW(9994)

        Public Const Raised_Hand As String = ChrW(9995)

        Public Const V As String = "✌"

        Public Const Pencil2 As String = "✏"

        Public Const Black_Nib As String = "✒"

        Public Const Heavy_Check_Mark As String = "✔"

        Public Const Heavy_Multiplication_X As String = "✖"

        Public Const Sparkles As String = ChrW(10024)

        Public Const Eight_Spoked_Asterisk As String = "✳"

        Public Const Eight_Pointed_Black_Star As String = "✴"

        Public Const Snowflake As String = "❄"

        Public Const Sparkle As String = "❇"

        Public Const X As String = ChrW(10060)

        Public Const Negative_Squared_Cross_Mark As String = ChrW(10062)

        Public Const Question As String = ChrW(10067)

        Public Const Grey_Question As String = ChrW(10068)

        Public Const Grey_Exclamation As String = ChrW(10069)

        Public Const Exclamation As String = ChrW(10071)

        Public Const Heart As String = "❤"

        Public Const Heavy_Plus_Sign As String = ChrW(10133)

        Public Const Heavy_Minus_Sign As String = ChrW(10134)

        Public Const Heavy_Division_Sign As String = ChrW(10135)

        Public Const Arrow_Right As String = "➡"

        Public Const Curly_Loop As String = ChrW(10160)

        Public Const Arrow_Heading_Up As String = "⤴"

        Public Const Arrow_Heading_Down As String = "⤵"

        Public Const Arrow_Left As String = "⬅"

        Public Const Arrow_Up As String = "⬆"

        Public Const Arrow_Down As String = "⬇"

        Public Const Black_Large_Square As String = "⬛"

        Public Const White_Large_Square As String = "⬜"

        Public Const Star As String = "⭐"

        Public Const O As String = ChrW(11093)

        Public Const Wavy_Dash As String = "〰"

        Public Const Part_Alternation_Mark As String = "〽"

        Public Const Congratulations As String = "㊗"

        Public Const Secret As String = "㊙"

        Public Const Mahjong As String = "🀄"

        Public Const Black_Joker As String = "🃏"

        Public Const A As String = "🅰"

        Public Const B As String = "🅱"

        Public Const O2 As String = "🅾"

        Public Const Parking As String = "🅿"

        Public Const Ab As String = "🆎"

        Public Const Cl As String = "🆑"

        Public Const Cool As String = "🆒"

        Public Const Free As String = "🆓"

        Public Const Id As String = "🆔"

        Public Const [New] As String = "🆕"

        Public Const Ng As String = "🆖"

        Public Const Ok As String = "🆗"

        Public Const Sos As String = "🆘"

        Public Const Up As String = "🆙"

        Public Const Vs As String = "🆚"

        Public Const Cn As String = "🇨 🇳"

        Public Const De As String = "🇩 🇪"

        Public Const Es As String = "🇪 🇸"

        Public Const Fr As String = "🇫 🇷"

        Public Const Uk As String = "🇬 🇧"

        Public Const It As String = "🇮 🇹"

        Public Const Jp As String = "🇯 🇵"

        Public Const Kr As String = "🇰 🇷"

        Public Const Ru As String = "🇷 🇺"

        Public Const Us As String = "🇺 🇸"

        Public Const Koko As String = "🈁"

        Public Const Sa As String = "🈂"

        Public Const U7121 As String = "🈚"

        Public Const U6307 As String = "🈯"

        Public Const U7981 As String = "🈲"

        Public Const U7A7A As String = "🈳"

        Public Const U5408 As String = "🈴"

        Public Const U6E80 As String = "🈵"

        Public Const U6709 As String = "🈶"

        Public Const U6708 As String = "🈷"

        Public Const U7533 As String = "🈸"

        Public Const U5272 As String = "🈹"

        Public Const U55B6 As String = "🈺"

        Public Const Ideograph_Advantage As String = "🉐"

        Public Const Accept As String = "🉑"

        Public Const Cyclone As String = "🌀"

        Public Const Foggy As String = "🌁"

        Public Const Closed_Umbrella As String = "🌂"

        Public Const Night_With_Stars As String = "🌃"

        Public Const Sunrise_Over_Mountains As String = "🌄"

        Public Const Sunrise As String = "🌅"

        Public Const City_Sunset As String = "🌆"

        Public Const City_Sunrise As String = "🌇"

        Public Const Rainbow As String = "🌈"

        Public Const Bridge_At_Night As String = "🌉"

        Public Const Ocean As String = "🌊"

        Public Const Volcano As String = "🌋"

        Public Const Milky_Way As String = "🌌"

        Public Const Earth_Asia As String = "🌏"

        Public Const New_Moon As String = "🌑"

        Public Const First_Quarter_Moon As String = "🌓"

        Public Const Waxing_Gibbous_Moon As String = "🌔"

        Public Const Full_Moon As String = "🌕"

        Public Const Crescent_Moon As String = "🌙"

        Public Const First_Quarter_Moon_With_Face As String = "🌛"

        Public Const Star2 As String = "🌟"

        Public Const Stars As String = "🌠"

        Public Const Chestnut As String = "🌰"

        Public Const Seedling As String = "🌱"

        Public Const Palm_Tree As String = "🌴"

        Public Const Cactus As String = "🌵"

        Public Const Tulip As String = "🌷"

        Public Const Cherry_Blossom As String = "🌸"

        Public Const Rose As String = "🌹"

        Public Const Hibiscus As String = "🌺"

        Public Const Sunflower As String = "🌻"

        Public Const Blossom As String = "🌼"

        Public Const Corn As String = "🌽"

        Public Const Ear_Of_Rice As String = "🌾"

        Public Const Herb As String = "🌿"

        Public Const Four_Leaf_Clover As String = "🍀"

        Public Const Maple_Leaf As String = "🍁"

        Public Const Fallen_Leaf As String = "🍂"

        Public Const Leaves As String = "🍃"

        Public Const Mushroom As String = "🍄"

        Public Const Tomato As String = "🍅"

        Public Const Eggplant As String = "🍆"

        Public Const Grapes As String = "🍇"

        Public Const Melon As String = "🍈"

        Public Const Watermelon As String = "🍉"

        Public Const Tangerine As String = "🍊"

        Public Const Banana As String = "🍌"

        Public Const Pineapple As String = "🍍"

        Public Const Apple As String = "🍎"

        Public Const Green_Apple As String = "🍏"

        Public Const Peach As String = "🍑"

        Public Const Cherries As String = "🍒"

        Public Const Strawberry As String = "🍓"

        Public Const Hamburger As String = "🍔"

        Public Const Pizza As String = "🍕"

        Public Const Meat_On_Bone As String = "🍖"

        Public Const Poultry_Leg As String = "🍗"

        Public Const Rice_Cracker As String = "🍘"

        Public Const Rice_Ball As String = "🍙"

        Public Const Rice As String = "🍚"

        Public Const Curry As String = "🍛"

        Public Const Ramen As String = "🍜"

        Public Const Spaghetti As String = "🍝"

        Public Const Bread As String = "🍞"

        Public Const Fries As String = "🍟"

        Public Const Sweet_Potato As String = "🍠"

        Public Const Dango As String = "🍡"

        Public Const Oden As String = "🍢"

        Public Const Sushi As String = "🍣"

        Public Const Fried_Shrimp As String = "🍤"

        Public Const Fish_Cake As String = "🍥"

        Public Const Icecream As String = "🍦"

        Public Const Shaved_Ice As String = "🍧"

        Public Const Ice_Cream As String = "🍨"

        Public Const Doughnut As String = "🍩"

        Public Const Cookie As String = "🍪"

        Public Const Chocolate_Bar As String = "🍫"

        Public Const Candy As String = "🍬"

        Public Const Lollipop As String = "🍭"

        Public Const Custard As String = "🍮"

        Public Const Honey_Pot As String = "🍯"

        Public Const Cake As String = "🍰"

        Public Const Bento As String = "🍱"

        Public Const Stew As String = "🍲"

        Public Const Egg As String = "🍳"

        Public Const Fork_And_Knife As String = "🍴"

        Public Const Tea As String = "🍵"

        Public Const Sake As String = "🍶"

        Public Const Wine_Glass As String = "🍷"

        Public Const Cocktail As String = "🍸"

        Public Const Tropical_Drink As String = "🍹"

        Public Const Beer As String = "🍺"

        Public Const Beers As String = "🍻"

        Public Const Ribbon As String = "🎀"

        Public Const Gift As String = "🎁"

        Public Const Birthday As String = "🎂"

        Public Const Jack_O_Lantern As String = "🎃"

        Public Const Christmas_Tree As String = "🎄"

        Public Const Santa As String = "🎅"

        Public Const Fireworks As String = "🎆"

        Public Const Sparkler As String = "🎇"

        Public Const Balloon As String = "🎈"

        Public Const Tada As String = "🎉"

        Public Const Confetti_Ball As String = "🎊"

        Public Const Tanabata_Tree As String = "🎋"

        Public Const Crossed_Flags As String = "🎌"

        Public Const Bamboo As String = "🎍"

        Public Const Dolls As String = "🎎"

        Public Const Flags As String = "🎏"

        Public Const Wind_Chime As String = "🎐"

        Public Const Rice_Scene As String = "🎑"

        Public Const School_Satchel As String = "🎒"

        Public Const Mortar_Board As String = "🎓"

        Public Const Carousel_Horse As String = "🎠"

        Public Const Ferris_Wheel As String = "🎡"

        Public Const Roller_Coaster As String = "🎢"

        Public Const Fishing_Pole_And_Fish As String = "🎣"

        Public Const Microphone As String = "🎤"

        Public Const Movie_Camera As String = "🎥"

        Public Const Cinema As String = "🎦"

        Public Const Headphones As String = "🎧"

        Public Const Art As String = "🎨"

        Public Const Tophat As String = "🎩"

        Public Const Circus_Tent As String = "🎪"

        Public Const Ticket As String = "🎫"

        Public Const Clapper As String = "🎬"

        Public Const Performing_Arts As String = "🎭"

        Public Const Video_Game As String = "🎮"

        Public Const Dart As String = "🎯"

        Public Const Slot_Machine As String = "🎰"

        Public Const _8Ball As String = "🎱"

        Public Const Game_Die As String = "🎲"

        Public Const Bowling As String = "🎳"

        Public Const Flower_Playing_Cards As String = "🎴"

        Public Const Musical_Note As String = "🎵"

        Public Const Notes As String = "🎶"

        Public Const Saxophone As String = "🎷"

        Public Const Guitar As String = "🎸"

        Public Const Musical_Keyboard As String = "🎹"

        Public Const Trumpet As String = "🎺"

        Public Const Violin As String = "🎻"

        Public Const Musical_Score As String = "🎼"

        Public Const Running_Shirt_With_Sash As String = "🎽"

        Public Const Tennis As String = "🎾"

        Public Const Ski As String = "🎿"

        Public Const Basketball As String = "🏀"

        Public Const Checkered_Flag As String = "🏁"

        Public Const Snowboarder As String = "🏂"

        Public Const Runner As String = "🏃"

        Public Const Surfer As String = "🏄"

        Public Const Trophy As String = "🏆"

        Public Const Football As String = "🏈"

        Public Const Swimmer As String = "🏊"

        Public Const House As String = "🏠"

        Public Const House_With_Garden As String = "🏡"

        Public Const Office As String = "🏢"

        Public Const Post_Office As String = "🏣"

        Public Const Hospital As String = "🏥"

        Public Const Bank As String = "🏦"

        Public Const Atm As String = "🏧"

        Public Const Hotel As String = "🏨"

        Public Const Love_Hotel As String = "🏩"

        Public Const Convenience_Store As String = "🏪"

        Public Const School As String = "🏫"

        Public Const Department_Store As String = "🏬"

        Public Const Factory As String = "🏭"

        Public Const Izakaya_Lantern As String = "🏮"

        Public Const Japanese_Castle As String = "🏯"

        Public Const European_Castle As String = "🏰"

        Public Const Snail As String = "🐌"

        Public Const Snake As String = "🐍"

        Public Const Racehorse As String = "🐎"

        Public Const Sheep As String = "🐑"

        Public Const Monkey As String = "🐒"

        Public Const Chicken As String = "🐔"

        Public Const Boar As String = "🐗"

        Public Const Elephant As String = "🐘"

        Public Const Octopus As String = "🐙"

        Public Const Shell As String = "🐚"

        Public Const Bug As String = "🐛"

        Public Const Ant As String = "🐜"

        Public Const Bee As String = "🐝"

        Public Const Beetle As String = "🐞"

        Public Const Fish As String = "🐟"

        Public Const Tropical_Fish As String = "🐠"

        Public Const Blowfish As String = "🐡"

        Public Const Turtle As String = "🐢"

        Public Const Hatching_Chick As String = "🐣"

        Public Const Baby_Chick As String = "🐤"

        Public Const Hatched_Chick As String = "🐥"

        Public Const Bird As String = "🐦"

        Public Const Penguin As String = "🐧"

        Public Const Koala As String = "🐨"

        Public Const Poodle As String = "🐩"

        Public Const Camel As String = "🐫"

        Public Const Dolphin As String = "🐬"

        Public Const Mouse As String = "🐭"

        Public Const Cow As String = "🐮"

        Public Const Tiger As String = "🐯"

        Public Const Rabbit As String = "🐰"

        Public Const Cat As String = "🐱"

        Public Const Dragon_Face As String = "🐲"

        Public Const Whale As String = "🐳"

        Public Const Horse As String = "🐴"

        Public Const Monkey_Face As String = "🐵"

        Public Const Dog As String = "🐶"

        Public Const Pig As String = "🐷"

        Public Const Frog As String = "🐸"

        Public Const Hamster As String = "🐹"

        Public Const Wolf As String = "🐺"

        Public Const Bear As String = "🐻"

        Public Const Panda_Face As String = "🐼"

        Public Const Pig_Nose As String = "🐽"

        Public Const Feet As String = "🐾"

        Public Const Eyes As String = "👀"

        Public Const Ear As String = "👂"

        Public Const Nose As String = "👃"

        Public Const Lips As String = "👄"

        Public Const Tongue As String = "👅"

        Public Const Point_Up_2 As String = "👆"

        Public Const Point_Down As String = "👇"

        Public Const Point_Left As String = "👈"

        Public Const Point_Right As String = "👉"

        Public Const Punch As String = "👊"

        Public Const Wave As String = "👋"

        Public Const Ok_Hand As String = "👌"

        Public Const Thumbsup As String = "👍"

        Public Const Thumbsdown As String = "👎"

        Public Const Clap As String = "👏"

        Public Const Open_Hands As String = "👐"

        Public Const Crown As String = "👑"

        Public Const Womans_Hat As String = "👒"

        Public Const Eyeglasses As String = "👓"

        Public Const Necktie As String = "👔"

        Public Const Shirt As String = "👕"

        Public Const Jeans As String = "👖"

        Public Const Dress As String = "👗"

        Public Const Kimono As String = "👘"

        Public Const Bikini As String = "👙"

        Public Const Womans_Clothes As String = "👚"

        Public Const Purse As String = "👛"

        Public Const Handbag As String = "👜"

        Public Const Pouch As String = "👝"

        Public Const Mans_Shoe As String = "👞"

        Public Const Athletic_Shoe As String = "👟"

        Public Const High_Heel As String = "👠"

        Public Const Sandal As String = "👡"

        Public Const Boot As String = "👢"

        Public Const Footprints As String = "👣"

        Public Const Bust_In_Silhouette As String = "👤"

        Public Const Boy As String = "👦"

        Public Const Girl As String = "👧"

        Public Const Man As String = "👨"

        Public Const Woman As String = "👩"

        Public Const Family As String = "👪"

        Public Const Couple As String = "👫"

        Public Const Cop As String = "👮"

        Public Const Dancers As String = "👯"

        Public Const Bride_With_Veil As String = "👰"

        Public Const Person_With_Blond_Hair As String = "👱"

        Public Const Man_With_Gua_Pi_Mao As String = "👲"

        Public Const Man_With_Turban As String = "👳"

        Public Const Older_Man As String = "👴"

        Public Const Older_Woman As String = "👵"

        Public Const Baby As String = "👶"

        Public Const Construction_Worker As String = "👷"

        Public Const Princess As String = "👸"

        Public Const Japanese_Ogre As String = "👹"

        Public Const Japanese_Goblin As String = "👺"

        Public Const Ghost As String = "👻"

        Public Const Angel As String = "👼"

        Public Const Alien As String = "👽"

        Public Const Space_Invader As String = "👾"

        Public Const Robot_Face As String = "🤖"

        Public Const Imp As String = "👿"

        Public Const Skull As String = "💀"

        Public Const Information_Desk_Person As String = "💁"

        Public Const Guardsman As String = "💂"

        Public Const Dancer As String = "💃"

        Public Const Lipstick As String = "💄"

        Public Const Nail_Care As String = "💅"

        Public Const Massage As String = "💆"

        Public Const Haircut As String = "💇"

        Public Const Barber As String = "💈"

        Public Const Syringe As String = "💉"

        Public Const Pill As String = "💊"

        Public Const Kiss As String = "💋"

        Public Const Love_Letter As String = "💌"

        Public Const Ring As String = "💍"

        Public Const Gem As String = "💎"

        Public Const Couplekiss As String = "💏"

        Public Const Bouquet As String = "💐"

        Public Const Couple_With_Heart As String = "💑"

        Public Const Wedding As String = "💒"

        Public Const Heartbeat As String = "💓"

        Public Const Broken_Heart As String = "💔"

        Public Const Two_Hearts As String = "💕"

        Public Const Sparkling_Heart As String = "💖"

        Public Const Heartpulse As String = "💗"

        Public Const Cupid As String = "💘"

        Public Const Blue_Heart As String = "💙"

        Public Const Green_Heart As String = "💚"

        Public Const Yellow_Heart As String = "💛"

        Public Const Purple_Heart As String = "💜"

        Public Const Gift_Heart As String = "💝"

        Public Const Revolving_Hearts As String = "💞"

        Public Const Heart_Decoration As String = "💟"

        Public Const Diamond_Shape_With_A_Dot_Inside As String = "💠"

        Public Const Bulb As String = "💡"

        Public Const Anger As String = "💢"

        Public Const Bomb As String = "💣"

        Public Const Zzz As String = "💤"

        Public Const Boom As String = "💥"

        Public Const Sweat_Drops As String = "💦"

        Public Const Droplet As String = "💧"

        Public Const Dash As String = "💨"

        Public Const Poop As String = "💩"

        Public Const Muscle As String = "💪"

        Public Const Dizzy As String = "💫"

        Public Const Speech_Balloon As String = "💬"

        Public Const White_Flower As String = "💮"

        Public Const _100 As String = "💯"

        Public Const Moneybag As String = "💰"

        Public Const Currency_Exchange As String = "💱"

        Public Const Heavy_Dollar_Sign As String = "💲"

        Public Const Credit_Card As String = "💳"

        Public Const Yen As String = "💴"

        Public Const Dollar As String = "💵"

        Public Const Money_With_Wings As String = "💸"

        Public Const Chart As String = "💹"

        Public Const Seat As String = "💺"

        Public Const Computer As String = "💻"

        Public Const Briefcase As String = "💼"

        Public Const Minidisc As String = "💽"

        Public Const Floppy_Disk As String = "💾"

        Public Const Cd As String = "💿"

        Public Const Dvd As String = "📀"

        Public Const File_Folder As String = "📁"

        Public Const Open_File_Folder As String = "📂"

        Public Const Page_With_Curl As String = "📃"

        Public Const Page_Facing_Up As String = "📄"

        Public Const [Date] As String = "📅"

        Public Const Calendar As String = "📆"

        Public Const Card_Index As String = "📇"

        Public Const Chart_With_Upwards_Trend As String = "📈"

        Public Const Chart_With_Downwards_Trend As String = "📉"

        Public Const Bar_Chart As String = "📊"

        Public Const Clipboard As String = "📋"

        Public Const Pushpin As String = "📌"

        Public Const Round_Pushpin As String = "📍"

        Public Const Paperclip As String = "📎"

        Public Const Straight_Ruler As String = "📏"

        Public Const Triangular_Ruler As String = "📐"

        Public Const Bookmark_Tabs As String = "📑"

        Public Const Ledger As String = "📒"

        Public Const Notebook As String = "📓"

        Public Const Notebook_With_Decorative_Cover As String = "📔"

        Public Const Closed_Book As String = "📕"

        Public Const Book As String = "📖"

        Public Const Green_Book As String = "📗"

        Public Const Blue_Book As String = "📘"

        Public Const Orange_Book As String = "📙"

        Public Const Books As String = "📚"

        Public Const Name_Badge As String = "📛"

        Public Const Scroll As String = "📜"

        Public Const Pencil As String = "📝"

        Public Const Telephone_Receiver As String = "📞"

        Public Const Pager As String = "📟"

        Public Const Fax As String = "📠"

        Public Const Satellite As String = "📡"

        Public Const Loudspeaker As String = "📢"

        Public Const Mega As String = "📣"

        Public Const Outbox_Tray As String = "📤"

        Public Const Inbox_Tray As String = "📥"

        Public Const Package As String = "📦"

        Public Const E_Mail As String = "📧"

        Public Const Incoming_Envelope As String = "📨"

        Public Const Envelope_With_Arrow As String = "📩"

        Public Const Mailbox_Closed As String = "📪"

        Public Const Mailbox As String = "📫"

        Public Const Postbox As String = "📮"

        Public Const Newspaper As String = "📰"

        Public Const Iphone As String = "📱"

        Public Const Calling As String = "📲"

        Public Const Vibration_Mode As String = "📳"

        Public Const Mobile_Phone_Off As String = "📴"

        Public Const Signal_Strength As String = "📶"

        Public Const Camera As String = "📷"

        Public Const Video_Camera As String = "📹"

        Public Const Tv As String = "📺"

        Public Const Radio As String = "📻"

        Public Const Vhs As String = "📼"

        Public Const Arrows_Clockwise As String = "🔃"

        Public Const Loud_Sound As String = "🔊"

        Public Const Battery As String = "🔋"

        Public Const Electric_Plug As String = "🔌"

        Public Const Mag As String = "🔍"

        Public Const Mag_Right As String = "🔎"

        Public Const Lock_With_Ink_Pen As String = "🔏"

        Public Const Closed_Lock_With_Key As String = "🔐"

        Public Const Key As String = "🔑"

        Public Const Lock As String = "🔒"

        Public Const Unlock As String = "🔓"

        Public Const Bell As String = "🔔"

        Public Const Bookmark As String = "🔖"

        Public Const Link As String = "🔗"

        Public Const Radio_Button As String = "🔘"

        Public Const Back As String = "🔙"

        Public Const [End] As String = "🔚"

        Public Const [On] As String = "🔛"

        Public Const Soon As String = "🔜"

        Public Const Top As String = "🔝"

        Public Const Underage As String = "🔞"

        Public Const Keycap_Ten As String = "🔟"

        Public Const Capital_Abcd As String = "🔠"

        Public Const Abcd As String = "🔡"

        Public Const _1234 As String = "🔢"

        Public Const Symbols As String = "🔣"

        Public Const Abc As String = "🔤"

        Public Const Fire As String = "🔥"

        Public Const Flashlight As String = "🔦"

        Public Const Wrench As String = "🔧"

        Public Const Hammer As String = "🔨"

        Public Const Nut_And_Bolt As String = "🔩"

        Public Const Knife As String = "🔪"

        Public Const Gun As String = "🔫"

        Public Const Crystal_Ball As String = "🔮"

        Public Const Six_Pointed_Star As String = "🔯"

        Public Const Beginner As String = "🔰"

        Public Const Trident As String = "🔱"

        Public Const Black_Square_Button As String = "🔲"

        Public Const White_Square_Button As String = "🔳"

        Public Const Red_Circle As String = "🔴"

        Public Const Large_Blue_Circle As String = "🔵"

        Public Const Large_Orange_Diamond As String = "🔶"

        Public Const Large_Blue_Diamond As String = "🔷"

        Public Const Small_Orange_Diamond As String = "🔸"

        Public Const Small_Blue_Diamond As String = "🔹"

        Public Const Small_Red_Triangle As String = "🔺"

        Public Const Small_Red_Triangle_Down As String = "🔻"

        Public Const Arrow_Up_Small As String = "🔼"

        Public Const Arrow_Down_Small As String = "🔽"

        Public Const Clock1 As String = "🕐"

        Public Const Clock2 As String = "🕑"

        Public Const Clock3 As String = "🕒"

        Public Const Clock4 As String = "🕓"

        Public Const Clock5 As String = "🕔"

        Public Const Clock6 As String = "🕕"

        Public Const Clock7 As String = "🕖"

        Public Const Clock8 As String = "🕗"

        Public Const Clock9 As String = "🕘"

        Public Const Clock10 As String = "🕙"

        Public Const Clock11 As String = "🕚"

        Public Const Clock12 As String = "🕛"

        Public Const Mount_Fuji As String = "🗻"

        Public Const Tokyo_Tower As String = "🗼"

        Public Const Statue_Of_Liberty As String = "🗽"

        Public Const Japan As String = "🗾"

        Public Const Moyai As String = "🗿"

        Public Const Grin As String = "😁"

        Public Const Joy As String = "😂"

        Public Const Smiley As String = "😃"

        Public Const Smile As String = "😄"

        Public Const Sweat_Smile As String = "😅"

        Public Const Laughing As String = "😆"

        Public Const Wink As String = "😉"

        Public Const Blush As String = "😊"

        Public Const Yum As String = "😋"

        Public Const Relieved As String = "😌"

        Public Const Heart_Eyes As String = "😍"

        Public Const Smirk As String = "😏"

        Public Const Unamused As String = "😒"

        Public Const Sweat As String = "😓"

        Public Const Pensive As String = "😔"

        Public Const Confounded As String = "😖"

        Public Const Kissing_Heart As String = "😘"

        Public Const Kissing_Closed_Eyes As String = "😚"

        Public Const Stuck_Out_Tongue_Winking_Eye As String = "😜"

        Public Const Stuck_Out_Tongue_Closed_Eyes As String = "😝"

        Public Const Disappointed As String = "😞"

        Public Const Angry As String = "😠"

        Public Const Rage As String = "😡"

        Public Const Cry As String = "😢"

        Public Const Persevere As String = "😣"

        Public Const Triumph As String = "😤"

        Public Const Disappointed_Relieved As String = "😥"

        Public Const Fearful As String = "😨"

        Public Const Weary As String = "😩"

        Public Const Sleepy As String = "😪"

        Public Const Tired_Face As String = "😫"

        Public Const Sob As String = "😭"

        Public Const Cold_Sweat As String = "😰"

        Public Const Scream As String = "😱"

        Public Const Astonished As String = "😲"

        Public Const Flushed As String = "😳"

        Public Const Dizzy_Face As String = "😵"

        Public Const Mask As String = "😷"

        Public Const Smile_Cat As String = "😸"

        Public Const Joy_Cat As String = "😹"

        Public Const Smiley_Cat As String = "😺"

        Public Const Heart_Eyes_Cat As String = "😻"

        Public Const Smirk_Cat As String = "😼"

        Public Const Kissing_Cat As String = "😽"

        Public Const Pouting_Cat As String = "😾"

        Public Const Crying_Cat_Face As String = "😿"

        Public Const Scream_Cat As String = "🙀"

        Public Const No_Good As String = "🙅"

        Public Const Ok_Woman As String = "🙆"

        Public Const Bow As String = "🙇"

        Public Const See_No_Evil As String = "🙈"

        Public Const Hear_No_Evil As String = "🙉"

        Public Const Speak_No_Evil As String = "🙊"

        Public Const Raising_Hand As String = "🙋"

        Public Const Raised_Hands As String = "🙌"

        Public Const Person_Frowning As String = "🙍"

        Public Const Person_With_Pouting_Face As String = "🙎"

        Public Const Pray As String = "🙏"

        Public Const Rocket As String = "🚀"

        Public Const Railway_Car As String = "🚃"

        Public Const Bullettrain_Side As String = "🚄"

        Public Const Bullettrain_Front As String = "🚅"

        Public Const Metro As String = "🚇"

        Public Const Station As String = "🚉"

        Public Const Bus As String = "🚌"

        Public Const Busstop As String = "🚏"

        Public Const Ambulance As String = "🚑"

        Public Const Fire_Engine As String = "🚒"

        Public Const Police_Car As String = "🚓"

        Public Const Taxi As String = "🚕"

        Public Const Red_Car As String = "🚗"

        Public Const Blue_Car As String = "🚙"

        Public Const Truck As String = "🚚"

        Public Const Ship As String = "🚢"

        Public Const Speedboat As String = "🚤"

        Public Const Traffic_Light As String = "🚥"

        Public Const Construction As String = "🚧"

        Public Const Rotating_Light As String = "🚨"

        Public Const Triangular_Flag_On_Post As String = "🚩"

        Public Const Door As String = "🚪"

        Public Const No_Entry_Sign As String = "🚫"

        Public Const Smoking As String = "🚬"

        Public Const No_Smoking As String = "🚭"

        Public Const Bike As String = "🚲"

        Public Const Walking As String = "🚶"

        Public Const Mens As String = "🚹"

        Public Const Womens As String = "🚺"

        Public Const Restroom As String = "🚻"

        Public Const Baby_Symbol As String = "🚼"

        Public Const Toilet As String = "🚽"

        Public Const Wc As String = "🚾"

        Public Const Bath As String = "🛀"

        Public Const Articulated_Lorry As String = "🚛"

        Public Const Kissing_Smiling_Eyes As String = "😙"

        Public Const Pear As String = "🍐"

        Public Const Bicyclist As String = "🚴"

        Public Const Rabbit2 As String = "🐇"

        Public Const Clock830 As String = "🕣"

        Public Const Train As String = "🚋"

        Public Const Oncoming_Automobile As String = "🚘"

        Public Const Expressionless As String = "😑"

        Public Const Smiling_Imp As String = "😈"

        Public Const Frowning As String = "😦"

        Public Const No_Mouth As String = "😶"

        Public Const Baby_Bottle As String = "🍼"

        Public Const Non_Potable_Water As String = "🚱"

        Public Const Open_Mouth As String = "😮"

        Public Const Last_Quarter_Moon_With_Face As String = "🌜"

        Public Const Do_Not_Litter As String = "🚯"

        Public Const Sunglasses As String = "😎"

        Public Const [Loop] As String = ChrW(10175)

        Public Const Last_Quarter_Moon As String = "🌗"

        Public Const Grinning As String = "😀"

        Public Const Euro As String = "💶"

        Public Const Clock330 As String = "🕞"

        Public Const Telescope As String = "🔭"

        Public Const Globe_With_Meridians As String = "🌐"

        Public Const Postal_Horn As String = "📯"

        Public Const Stuck_Out_Tongue As String = "😛"

        Public Const Clock1030 As String = "🕥"

        Public Const Pound As String = "💷"

        Public Const Two_Men_Holding_Hands As String = "👬"

        Public Const Tiger2 As String = "🐅"

        Public Const Anguished As String = "😧"

        Public Const Vertical_Traffic_Light As String = "🚦"

        Public Const Confused As String = "😕"

        Public Const Repeat As String = "🔁"

        Public Const Oncoming_Police_Car As String = "🚔"

        Public Const Tram As String = "🚊"

        Public Const Dragon As String = "🐉"

        Public Const Earth_Americas As String = "🌎"

        Public Const Rugby_Football As String = "🏉"

        Public Const Left_Luggage As String = "🛅"

        Public Const Sound As String = "🔉"

        Public Const Clock630 As String = "🕡"

        Public Const Dromedary_Camel As String = "🐪"

        Public Const Oncoming_Bus As String = "🚍"

        Public Const Horse_Racing As String = "🏇"

        Public Const Rooster As String = "🐓"

        Public Const Rowboat As String = "🚣"

        Public Const Customs As String = "🛃"

        Public Const Repeat_One As String = "🔂"

        Public Const Waxing_Crescent_Moon As String = "🌒"

        Public Const Mountain_Railway As String = "🚞"

        Public Const Clock930 As String = "🕤"

        Public Const Put_Litter_In_Its_Place As String = "🚮"

        Public Const Arrows_Counterclockwise As String = "🔄"

        Public Const Clock130 As String = "🕜"

        Public Const Goat As String = "🐐"

        Public Const Pig2 As String = "🐖"

        Public Const Innocent As String = "😇"

        Public Const No_Bicycles As String = "🚳"

        Public Const Light_Rail As String = "🚈"

        Public Const Whale2 As String = "🐋"

        Public Const Train2 As String = "🚆"

        Public Const Earth_Africa As String = "🌍"

        Public Const Shower As String = "🚿"

        Public Const Waning_Gibbous_Moon As String = "🌖"

        Public Const Steam_Locomotive As String = "🚂"

        Public Const Cat2 As String = "🐈"

        Public Const Tractor As String = "🚜"

        Public Const Thought_Balloon As String = "💭"

        Public Const Two_Women_Holding_Hands As String = "👭"

        Public Const Full_Moon_With_Face As String = "🌝"

        Public Const Mouse2 As String = "🐁"

        Public Const Clock430 As String = "🕟"

        Public Const Worried As String = "😟"

        Public Const Rat As String = "🐀"

        Public Const Ram As String = "🐏"

        Public Const Dog2 As String = "🐕"

        Public Const Kissing As String = "😗"

        Public Const Helicopter As String = "🚁"

        Public Const Clock1130 As String = "🕦"

        Public Const No_Mobile_Phones As String = "📵"

        Public Const European_Post_Office As String = "🏤"

        Public Const Ox As String = "🐂"

        Public Const Mountain_Cableway As String = "🚠"

        Public Const Sleeping As String = "😴"

        Public Const Cow2 As String = "🐄"

        Public Const Minibus As String = "🚐"

        Public Const Clock730 As String = "🕢"

        Public Const Aerial_Tramway As String = "🚡"

        Public Const Speaker As String = "🔈"

        Public Const No_Bell As String = "🔕"

        Public Const Mailbox_With_Mail As String = "📬"

        Public Const No_Pedestrians As String = "🚷"

        Public Const Microscope As String = "🔬"

        Public Const Bathtub As String = "🛁"

        Public Const Suspension_Railway As String = "🚟"

        Public Const Crocodile As String = "🐊"

        Public Const Mountain_Bicyclist As String = "🚵"

        Public Const Waning_Crescent_Moon As String = "🌘"

        Public Const Monorail As String = "🚝"

        Public Const Children_Crossing As String = "🚸"

        Public Const Clock230 As String = "🕝"

        Public Const Busts_In_Silhouette As String = "👥"

        Public Const Mailbox_With_No_Mail As String = "📭"

        Public Const Leopard As String = "🐆"

        Public Const Deciduous_Tree As String = "🌳"

        Public Const Oncoming_Taxi As String = "🚖"

        Public Const Lemon As String = "🍋"

        Public Const Mute As String = "🔇"

        Public Const Baggage_Claim As String = "🛄"

        Public Const Twisted_Rightwards_Arrows As String = "🔀"

        Public Const Sun_With_Face As String = "🌞"

        Public Const Trolleybus As String = "🚎"

        Public Const Evergreen_Tree As String = "🌲"

        Public Const Passport_Control As String = "🛂"

        Public Const New_Moon_With_Face As String = "🌚"

        Public Const Potable_Water As String = "🚰"

        Public Const High_Brightness As String = "🔆"

        Public Const Low_Brightness As String = "🔅"

        Public Const Clock530 As String = "🕠"

        Public Const Hushed As String = "😯"

        Public Const Grimacing As String = "😬"

        Public Const Water_Buffalo As String = "🐃"

        Public Const Neutral_Face As String = "😐"

        Public Const Clock1230 As String = "🕧"
    End Class

End Namespace