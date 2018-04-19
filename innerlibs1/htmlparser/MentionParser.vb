Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace HtmlParser

    Public Class HtmlAnchor
        Inherits HtmlElement

        Sub New()
            MyBase.New("a")
        End Sub

        Sub New(Url As String, Text As String)
            MyBase.New("a")
            Me.Href = Url
            Me.InnerHTML = Text
        End Sub

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
                Me.Attribute("src") = value
            End Set
        End Property

    End Class

    Public Module MentionParser

        <Extension()>
        Function ParseURL(ByVal Text As HtmlNode, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text.HTML, "(http(s)?://)?([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?", New MatchEvaluator(Function(x) Method(x.ToString)))
        End Function

        ''' <summary>
        ''' Localiza emojis no texto e automaticamente executa uma função de replace para cada emoji encontrado
        ''' </summary>
        ''' <param name="Text">  Texto</param>
        ''' <param name="Method"></param>
        ''' <returns></returns>
        <Extension()>
        Function ParseEmoji(ByVal Text As HtmlNode, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text.ToString, "(:)((?:[A-Za-z0-9-_]*))(:)", New MatchEvaluator(Function(x) Method(x.ToString)))
        End Function

        ''' <summary>
        ''' Localiza menções a usuários no texto e automaticamente executa uma função de replace para
        ''' cada hashtag encontrada
        ''' </summary>
        ''' <param name="Text">  Texto</param>
        ''' <param name="Method"></param>
        ''' <returns></returns>
        <Extension()>
        Function ParseUsername(ByVal Text As HtmlNode, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text.ToString, "(@)((?:[A-Za-z0-9-_]*))", New MatchEvaluator(Function(x) Method(x.ToString)))
        End Function

        ''' <summary>
        ''' Localiza hashtags no texto e automaticamente executa uma função de replace para cada
        ''' hashtag encontrada
        ''' </summary>
        ''' <param name="Text">  Texto</param>
        ''' <param name="Method"></param>
        ''' <returns></returns>
        <Extension()>
        Function ParseHashtag(ByVal Text As HtmlElement, Method As Func(Of String, String)) As String
            Return Regex.Replace(Text.ToString, "(#)((?:[A-Za-z0-9-_]*))", New MatchEvaluator(Function(x) Method(x.ToString)))
        End Function

        ''' <summary>
        ''' Cria um elemento de Ancora (a) a partir de uma string com URL. O titulo é obtido
        ''' automaticamente da url quando possivel. Se a string não for uma URL válida uma ancora com
        ''' o proprio texto é criada.
        ''' </summary>
        ''' <param name="URL">URL</param>
        ''' <returns></returns>
        <Extension()>
        Function CreateAnchor(URL As String) As HtmlAnchor
            If URL.IsURL Then
                Try
                    Return New HtmlAnchor(URL, BrowserClipper.GetTitle(URL))
                Catch ex As Exception
                    Return New HtmlAnchor(URL, URL)
                End Try
            End If
            Return New HtmlAnchor("javascript:void(0);", URL)
        End Function

    End Module

    Public NotInheritable Class Emoji

        Public Shared Function GetList() As Dictionary(Of String, String)
            Return GetType(Emoji).GetProperties.ToDictionary(Function(x) x.Name.RemoveAny("_").ToLower, Function(x) x.GetRawConstantValue.ToString)
        End Function

        Public Shared Function GetByName(Name As String) As String
            GetList.TryGetValue(Name.ToLower, GetByName)
            Return GetByName & ""
        End Function

        Public ReadOnly Property Hash As String = "#"

        Public ReadOnly Property Zero As String = "0"

        Public ReadOnly Property One As String = "1"

        Public ReadOnly Property Two As String = "2"

        Public ReadOnly Property Three As String = "3"

        Public ReadOnly Property Four As String = "4"

        Public ReadOnly Property Five As String = "5"

        Public ReadOnly Property Six As String = "6"

        Public ReadOnly Property Seven As String = "7"

        Public ReadOnly Property Eight As String = "8"

        Public ReadOnly Property Nine As String = "9"

        Public ReadOnly Property Copyright As String = "©"

        Public ReadOnly Property Registered As String = "®"

        Public ReadOnly Property Bangbang As String = "‼"

        Public ReadOnly Property Interrobang As String = "⁉"

        Public ReadOnly Property Tm As String = "™"

        Public ReadOnly Property Information_Source As String = "ℹ"

        Public ReadOnly Property Left_Right_Arrow As String = "↔"

        Public ReadOnly Property Arrow_Up_Down As String = "↕"

        Public ReadOnly Property Arrow_Upper_Left As String = "↖"

        Public ReadOnly Property Arrow_Upper_Right As String = "↗"

        Public ReadOnly Property Arrow_Lower_Right As String = "↘"

        Public ReadOnly Property Arrow_Lower_Left As String = "↙"

        Public ReadOnly Property Leftwards_Arrow_With_Hook As String = "↩"

        Public ReadOnly Property Arrow_Right_Hook As String = "↪"

        Public ReadOnly Property Watch As String = "⌚"

        Public ReadOnly Property Hourglass As String = "⌛"

        Public ReadOnly Property Fast_Forward As String = ChrW(9193)

        Public ReadOnly Property Rewind As String = ChrW(9194)

        Public ReadOnly Property Arrow_Double_Up As String = ChrW(9195)

        Public ReadOnly Property Arrow_Double_Down As String = ChrW(9196)

        Public ReadOnly Property Alarm_Clock As String = ChrW(9200)

        Public ReadOnly Property Hourglass_Flowing_Sand As String = ChrW(9203)

        Public ReadOnly Property M As String = "Ⓜ"

        Public ReadOnly Property Black_Small_Square As String = "▪"

        Public ReadOnly Property White_Small_Square As String = "▫"

        Public ReadOnly Property Arrow_Forward As String = "▶"

        Public ReadOnly Property Arrow_Backward As String = "◀"

        Public ReadOnly Property White_Medium_Square As String = "◻"

        Public ReadOnly Property Black_Medium_Square As String = "◼"

        Public ReadOnly Property White_Medium_Small_Square As String = "◽"

        Public ReadOnly Property Black_Medium_Small_Square As String = "◾"

        Public ReadOnly Property Sunny As String = "☀"

        Public ReadOnly Property Cloud As String = "☁"

        Public ReadOnly Property Telephone As String = "☎"

        Public ReadOnly Property Ballot_Box_With_Check As String = "☑"

        Public ReadOnly Property Umbrella As String = "☔"

        Public ReadOnly Property Coffee As String = "☕"

        Public ReadOnly Property Point_Up As String = "☝"

        Public ReadOnly Property Relaxed As String = "☺"

        Public ReadOnly Property Aries As String = "♈"

        Public ReadOnly Property Taurus As String = "♉"

        Public ReadOnly Property Gemini As String = "♊"

        Public ReadOnly Property Cancer As String = "♋"

        Public ReadOnly Property Leo As String = "♌"

        Public ReadOnly Property Virgo As String = "♍"

        Public ReadOnly Property Libra As String = "♎"

        Public ReadOnly Property Scorpius As String = "♏"

        Public ReadOnly Property Sagittarius As String = "♐"

        Public ReadOnly Property Capricorn As String = "♑"

        Public ReadOnly Property Aquarius As String = "♒"

        Public ReadOnly Property Pisces As String = "♓"

        Public ReadOnly Property Spades As String = "♠"

        Public ReadOnly Property Clubs As String = "♣"

        Public ReadOnly Property Hearts As String = "♥"

        Public ReadOnly Property Diamonds As String = "♦"

        Public ReadOnly Property Hotsprings As String = "♨"

        Public ReadOnly Property Recycle As String = "♻"

        Public ReadOnly Property Wheelchair As String = "♿"

        Public ReadOnly Property Anchor As String = "⚓"

        Public ReadOnly Property Warning As String = "⚠"

        Public ReadOnly Property Zap As String = "⚡"

        Public ReadOnly Property White_Circle As String = "⚪"

        Public ReadOnly Property Black_Circle As String = "⚫"

        Public ReadOnly Property Soccer As String = ChrW(9917)

        Public ReadOnly Property Baseball As String = ChrW(9918)

        Public ReadOnly Property Snowman As String = ChrW(9924)

        Public ReadOnly Property Partly_Sunny As String = ChrW(9925)

        Public ReadOnly Property Ophiuchus As String = ChrW(9934)

        Public ReadOnly Property No_Entry As String = ChrW(9940)

        Public ReadOnly Property Church As String = ChrW(9962)

        Public ReadOnly Property Fountain As String = ChrW(9970)

        Public ReadOnly Property Golf As String = ChrW(9971)

        Public ReadOnly Property Sailboat As String = ChrW(9973)

        Public ReadOnly Property Tent As String = ChrW(9978)

        Public ReadOnly Property Fuelpump As String = ChrW(9981)

        Public ReadOnly Property Scissors As String = "✂"

        Public ReadOnly Property White_Check_Mark As String = ChrW(9989)

        Public ReadOnly Property Airplane As String = "✈"

        Public ReadOnly Property Envelope As String = "✉"

        Public ReadOnly Property Fist As String = ChrW(9994)

        Public ReadOnly Property Raised_Hand As String = ChrW(9995)

        Public ReadOnly Property V As String = "✌"

        Public ReadOnly Property Pencil2 As String = "✏"

        Public ReadOnly Property Black_Nib As String = "✒"

        Public ReadOnly Property Heavy_Check_Mark As String = "✔"

        Public ReadOnly Property Heavy_Multiplication_X As String = "✖"

        Public ReadOnly Property Sparkles As String = ChrW(10024)

        Public ReadOnly Property Eight_Spoked_Asterisk As String = "✳"

        Public ReadOnly Property Eight_Pointed_Black_Star As String = "✴"

        Public ReadOnly Property Snowflake As String = "❄"

        Public ReadOnly Property Sparkle As String = "❇"

        Public ReadOnly Property X As String = ChrW(10060)

        Public ReadOnly Property Negative_Squared_Cross_Mark As String = ChrW(10062)

        Public ReadOnly Property Question As String = ChrW(10067)

        Public ReadOnly Property Grey_Question As String = ChrW(10068)

        Public ReadOnly Property Grey_Exclamation As String = ChrW(10069)

        Public ReadOnly Property Exclamation As String = ChrW(10071)

        Public ReadOnly Property Heart As String = "❤"

        Public ReadOnly Property Heavy_Plus_Sign As String = ChrW(10133)

        Public ReadOnly Property Heavy_Minus_Sign As String = ChrW(10134)

        Public ReadOnly Property Heavy_Division_Sign As String = ChrW(10135)

        Public ReadOnly Property Arrow_Right As String = "➡"

        Public ReadOnly Property Curly_Loop As String = ChrW(10160)

        Public ReadOnly Property Arrow_Heading_Up As String = "⤴"

        Public ReadOnly Property Arrow_Heading_Down As String = "⤵"

        Public ReadOnly Property Arrow_Left As String = "⬅"

        Public ReadOnly Property Arrow_Up As String = "⬆"

        Public ReadOnly Property Arrow_Down As String = "⬇"

        Public ReadOnly Property Black_Large_Square As String = "⬛"

        Public ReadOnly Property White_Large_Square As String = "⬜"

        Public ReadOnly Property Star As String = "⭐"

        Public ReadOnly Property O As String = ChrW(11093)

        Public ReadOnly Property Wavy_Dash As String = "〰"

        Public ReadOnly Property Part_Alternation_Mark As String = "〽"

        Public ReadOnly Property Congratulations As String = "㊗"

        Public ReadOnly Property Secret As String = "㊙"

        Public ReadOnly Property Mahjong As String = "🀄"

        Public ReadOnly Property Black_Joker As String = "🃏"

        Public ReadOnly Property A As String = "🅰"

        Public ReadOnly Property B As String = "🅱"

        Public ReadOnly Property O2 As String = "🅾"

        Public ReadOnly Property Parking As String = "🅿"

        Public ReadOnly Property Ab As String = "🆎"

        Public ReadOnly Property Cl As String = "🆑"

        Public ReadOnly Property Cool As String = "🆒"

        Public ReadOnly Property Free As String = "🆓"

        Public ReadOnly Property Id As String = "🆔"

        Public ReadOnly Property [New] As String = "🆕"

        Public ReadOnly Property Ng As String = "🆖"

        Public ReadOnly Property Ok As String = "🆗"

        Public ReadOnly Property Sos As String = "🆘"

        Public ReadOnly Property Up As String = "🆙"

        Public ReadOnly Property Vs As String = "🆚"

        Public ReadOnly Property Cn As String = "🇨 🇳"

        Public ReadOnly Property De As String = "🇩 🇪"

        Public ReadOnly Property Es As String = "🇪 🇸"

        Public ReadOnly Property Fr As String = "🇫 🇷"

        Public ReadOnly Property Uk As String = "🇬 🇧"

        Public ReadOnly Property It As String = "🇮 🇹"

        Public ReadOnly Property Jp As String = "🇯 🇵"

        Public ReadOnly Property Kr As String = "🇰 🇷"

        Public ReadOnly Property Ru As String = "🇷 🇺"

        Public ReadOnly Property Us As String = "🇺 🇸"

        Public ReadOnly Property Koko As String = "🈁"

        Public ReadOnly Property Sa As String = "🈂"

        Public ReadOnly Property U7121 As String = "🈚"

        Public ReadOnly Property U6307 As String = "🈯"

        Public ReadOnly Property U7981 As String = "🈲"

        Public ReadOnly Property U7A7A As String = "🈳"

        Public ReadOnly Property U5408 As String = "🈴"

        Public ReadOnly Property U6E80 As String = "🈵"

        Public ReadOnly Property U6709 As String = "🈶"

        Public ReadOnly Property U6708 As String = "🈷"

        Public ReadOnly Property U7533 As String = "🈸"

        Public ReadOnly Property U5272 As String = "🈹"

        Public ReadOnly Property U55B6 As String = "🈺"

        Public ReadOnly Property Ideograph_Advantage As String = "🉐"

        Public ReadOnly Property Accept As String = "🉑"

        Public ReadOnly Property Cyclone As String = "🌀"

        Public ReadOnly Property Foggy As String = "🌁"

        Public ReadOnly Property Closed_Umbrella As String = "🌂"

        Public ReadOnly Property Night_With_Stars As String = "🌃"

        Public ReadOnly Property Sunrise_Over_Mountains As String = "🌄"

        Public ReadOnly Property Sunrise As String = "🌅"

        Public ReadOnly Property City_Sunset As String = "🌆"

        Public ReadOnly Property City_Sunrise As String = "🌇"

        Public ReadOnly Property Rainbow As String = "🌈"

        Public ReadOnly Property Bridge_At_Night As String = "🌉"

        Public ReadOnly Property Ocean As String = "🌊"

        Public ReadOnly Property Volcano As String = "🌋"

        Public ReadOnly Property Milky_Way As String = "🌌"

        Public ReadOnly Property Earth_Asia As String = "🌏"

        Public ReadOnly Property New_Moon As String = "🌑"

        Public ReadOnly Property First_Quarter_Moon As String = "🌓"

        Public ReadOnly Property Waxing_Gibbous_Moon As String = "🌔"

        Public ReadOnly Property Full_Moon As String = "🌕"

        Public ReadOnly Property Crescent_Moon As String = "🌙"

        Public ReadOnly Property First_Quarter_Moon_With_Face As String = "🌛"

        Public ReadOnly Property Star2 As String = "🌟"

        Public ReadOnly Property Stars As String = "🌠"

        Public ReadOnly Property Chestnut As String = "🌰"

        Public ReadOnly Property Seedling As String = "🌱"

        Public ReadOnly Property Palm_Tree As String = "🌴"

        Public ReadOnly Property Cactus As String = "🌵"

        Public ReadOnly Property Tulip As String = "🌷"

        Public ReadOnly Property Cherry_Blossom As String = "🌸"

        Public ReadOnly Property Rose As String = "🌹"

        Public ReadOnly Property Hibiscus As String = "🌺"

        Public ReadOnly Property Sunflower As String = "🌻"

        Public ReadOnly Property Blossom As String = "🌼"

        Public ReadOnly Property Corn As String = "🌽"

        Public ReadOnly Property Ear_Of_Rice As String = "🌾"

        Public ReadOnly Property Herb As String = "🌿"

        Public ReadOnly Property Four_Leaf_Clover As String = "🍀"

        Public ReadOnly Property Maple_Leaf As String = "🍁"

        Public ReadOnly Property Fallen_Leaf As String = "🍂"

        Public ReadOnly Property Leaves As String = "🍃"

        Public ReadOnly Property Mushroom As String = "🍄"

        Public ReadOnly Property Tomato As String = "🍅"

        Public ReadOnly Property Eggplant As String = "🍆"

        Public ReadOnly Property Grapes As String = "🍇"

        Public ReadOnly Property Melon As String = "🍈"

        Public ReadOnly Property Watermelon As String = "🍉"

        Public ReadOnly Property Tangerine As String = "🍊"

        Public ReadOnly Property Banana As String = "🍌"

        Public ReadOnly Property Pineapple As String = "🍍"

        Public ReadOnly Property Apple As String = "🍎"

        Public ReadOnly Property Green_Apple As String = "🍏"

        Public ReadOnly Property Peach As String = "🍑"

        Public ReadOnly Property Cherries As String = "🍒"

        Public ReadOnly Property Strawberry As String = "🍓"

        Public ReadOnly Property Hamburger As String = "🍔"

        Public ReadOnly Property Pizza As String = "🍕"

        Public ReadOnly Property Meat_On_Bone As String = "🍖"

        Public ReadOnly Property Poultry_Leg As String = "🍗"

        Public ReadOnly Property Rice_Cracker As String = "🍘"

        Public ReadOnly Property Rice_Ball As String = "🍙"

        Public ReadOnly Property Rice As String = "🍚"

        Public ReadOnly Property Curry As String = "🍛"

        Public ReadOnly Property Ramen As String = "🍜"

        Public ReadOnly Property Spaghetti As String = "🍝"

        Public ReadOnly Property Bread As String = "🍞"

        Public ReadOnly Property Fries As String = "🍟"

        Public ReadOnly Property Sweet_Potato As String = "🍠"

        Public ReadOnly Property Dango As String = "🍡"

        Public ReadOnly Property Oden As String = "🍢"

        Public ReadOnly Property Sushi As String = "🍣"

        Public ReadOnly Property Fried_Shrimp As String = "🍤"

        Public ReadOnly Property Fish_Cake As String = "🍥"

        Public ReadOnly Property Icecream As String = "🍦"

        Public ReadOnly Property Shaved_Ice As String = "🍧"

        Public ReadOnly Property Ice_Cream As String = "🍨"

        Public ReadOnly Property Doughnut As String = "🍩"

        Public ReadOnly Property Cookie As String = "🍪"

        Public ReadOnly Property Chocolate_Bar As String = "🍫"

        Public ReadOnly Property Candy As String = "🍬"

        Public ReadOnly Property Lollipop As String = "🍭"

        Public ReadOnly Property Custard As String = "🍮"

        Public ReadOnly Property Honey_Pot As String = "🍯"

        Public ReadOnly Property Cake As String = "🍰"

        Public ReadOnly Property Bento As String = "🍱"

        Public ReadOnly Property Stew As String = "🍲"

        Public ReadOnly Property Egg As String = "🍳"

        Public ReadOnly Property Fork_And_Knife As String = "🍴"

        Public ReadOnly Property Tea As String = "🍵"

        Public ReadOnly Property Sake As String = "🍶"

        Public ReadOnly Property Wine_Glass As String = "🍷"

        Public ReadOnly Property Cocktail As String = "🍸"

        Public ReadOnly Property Tropical_Drink As String = "🍹"

        Public ReadOnly Property Beer As String = "🍺"

        Public ReadOnly Property Beers As String = "🍻"

        Public ReadOnly Property Ribbon As String = "🎀"

        Public ReadOnly Property Gift As String = "🎁"

        Public ReadOnly Property Birthday As String = "🎂"

        Public ReadOnly Property Jack_O_Lantern As String = "🎃"

        Public ReadOnly Property Christmas_Tree As String = "🎄"

        Public ReadOnly Property Santa As String = "🎅"

        Public ReadOnly Property Fireworks As String = "🎆"

        Public ReadOnly Property Sparkler As String = "🎇"

        Public ReadOnly Property Balloon As String = "🎈"

        Public ReadOnly Property Tada As String = "🎉"

        Public ReadOnly Property Confetti_Ball As String = "🎊"

        Public ReadOnly Property Tanabata_Tree As String = "🎋"

        Public ReadOnly Property Crossed_Flags As String = "🎌"

        Public ReadOnly Property Bamboo As String = "🎍"

        Public ReadOnly Property Dolls As String = "🎎"

        Public ReadOnly Property Flags As String = "🎏"

        Public ReadOnly Property Wind_Chime As String = "🎐"

        Public ReadOnly Property Rice_Scene As String = "🎑"

        Public ReadOnly Property School_Satchel As String = "🎒"

        Public ReadOnly Property Mortar_Board As String = "🎓"

        Public ReadOnly Property Carousel_Horse As String = "🎠"

        Public ReadOnly Property Ferris_Wheel As String = "🎡"

        Public ReadOnly Property Roller_Coaster As String = "🎢"

        Public ReadOnly Property Fishing_Pole_And_Fish As String = "🎣"

        Public ReadOnly Property Microphone As String = "🎤"

        Public ReadOnly Property Movie_Camera As String = "🎥"

        Public ReadOnly Property Cinema As String = "🎦"

        Public ReadOnly Property Headphones As String = "🎧"

        Public ReadOnly Property Art As String = "🎨"

        Public ReadOnly Property Tophat As String = "🎩"

        Public ReadOnly Property Circus_Tent As String = "🎪"

        Public ReadOnly Property Ticket As String = "🎫"

        Public ReadOnly Property Clapper As String = "🎬"

        Public ReadOnly Property Performing_Arts As String = "🎭"

        Public ReadOnly Property Video_Game As String = "🎮"

        Public ReadOnly Property Dart As String = "🎯"

        Public ReadOnly Property Slot_Machine As String = "🎰"

        Public ReadOnly Property _8Ball As String = "🎱"

        Public ReadOnly Property Game_Die As String = "🎲"

        Public ReadOnly Property Bowling As String = "🎳"

        Public ReadOnly Property Flower_Playing_Cards As String = "🎴"

        Public ReadOnly Property Musical_Note As String = "🎵"

        Public ReadOnly Property Notes As String = "🎶"

        Public ReadOnly Property Saxophone As String = "🎷"

        Public ReadOnly Property Guitar As String = "🎸"

        Public ReadOnly Property Musical_Keyboard As String = "🎹"

        Public ReadOnly Property Trumpet As String = "🎺"

        Public ReadOnly Property Violin As String = "🎻"

        Public ReadOnly Property Musical_Score As String = "🎼"

        Public ReadOnly Property Running_Shirt_With_Sash As String = "🎽"

        Public ReadOnly Property Tennis As String = "🎾"

        Public ReadOnly Property Ski As String = "🎿"

        Public ReadOnly Property Basketball As String = "🏀"

        Public ReadOnly Property Checkered_Flag As String = "🏁"

        Public ReadOnly Property Snowboarder As String = "🏂"

        Public ReadOnly Property Runner As String = "🏃"

        Public ReadOnly Property Surfer As String = "🏄"

        Public ReadOnly Property Trophy As String = "🏆"

        Public ReadOnly Property Football As String = "🏈"

        Public ReadOnly Property Swimmer As String = "🏊"

        Public ReadOnly Property House As String = "🏠"

        Public ReadOnly Property House_With_Garden As String = "🏡"

        Public ReadOnly Property Office As String = "🏢"

        Public ReadOnly Property Post_Office As String = "🏣"

        Public ReadOnly Property Hospital As String = "🏥"

        Public ReadOnly Property Bank As String = "🏦"

        Public ReadOnly Property Atm As String = "🏧"

        Public ReadOnly Property Hotel As String = "🏨"

        Public ReadOnly Property Love_Hotel As String = "🏩"

        Public ReadOnly Property Convenience_Store As String = "🏪"

        Public ReadOnly Property School As String = "🏫"

        Public ReadOnly Property Department_Store As String = "🏬"

        Public ReadOnly Property Factory As String = "🏭"

        Public ReadOnly Property Izakaya_Lantern As String = "🏮"

        Public ReadOnly Property Japanese_Castle As String = "🏯"

        Public ReadOnly Property European_Castle As String = "🏰"

        Public ReadOnly Property Snail As String = "🐌"

        Public ReadOnly Property Snake As String = "🐍"

        Public ReadOnly Property Racehorse As String = "🐎"

        Public ReadOnly Property Sheep As String = "🐑"

        Public ReadOnly Property Monkey As String = "🐒"

        Public ReadOnly Property Chicken As String = "🐔"

        Public ReadOnly Property Boar As String = "🐗"

        Public ReadOnly Property Elephant As String = "🐘"

        Public ReadOnly Property Octopus As String = "🐙"

        Public ReadOnly Property Shell As String = "🐚"

        Public ReadOnly Property Bug As String = "🐛"

        Public ReadOnly Property Ant As String = "🐜"

        Public ReadOnly Property Bee As String = "🐝"

        Public ReadOnly Property Beetle As String = "🐞"

        Public ReadOnly Property Fish As String = "🐟"

        Public ReadOnly Property Tropical_Fish As String = "🐠"

        Public ReadOnly Property Blowfish As String = "🐡"

        Public ReadOnly Property Turtle As String = "🐢"

        Public ReadOnly Property Hatching_Chick As String = "🐣"

        Public ReadOnly Property Baby_Chick As String = "🐤"

        Public ReadOnly Property Hatched_Chick As String = "🐥"

        Public ReadOnly Property Bird As String = "🐦"

        Public ReadOnly Property Penguin As String = "🐧"

        Public ReadOnly Property Koala As String = "🐨"

        Public ReadOnly Property Poodle As String = "🐩"

        Public ReadOnly Property Camel As String = "🐫"

        Public ReadOnly Property Dolphin As String = "🐬"

        Public ReadOnly Property Mouse As String = "🐭"

        Public ReadOnly Property Cow As String = "🐮"

        Public ReadOnly Property Tiger As String = "🐯"

        Public ReadOnly Property Rabbit As String = "🐰"

        Public ReadOnly Property Cat As String = "🐱"

        Public ReadOnly Property Dragon_Face As String = "🐲"

        Public ReadOnly Property Whale As String = "🐳"

        Public ReadOnly Property Horse As String = "🐴"

        Public ReadOnly Property Monkey_Face As String = "🐵"

        Public ReadOnly Property Dog As String = "🐶"

        Public ReadOnly Property Pig As String = "🐷"

        Public ReadOnly Property Frog As String = "🐸"

        Public ReadOnly Property Hamster As String = "🐹"

        Public ReadOnly Property Wolf As String = "🐺"

        Public ReadOnly Property Bear As String = "🐻"

        Public ReadOnly Property Panda_Face As String = "🐼"

        Public ReadOnly Property Pig_Nose As String = "🐽"

        Public ReadOnly Property Feet As String = "🐾"

        Public ReadOnly Property Eyes As String = "👀"

        Public ReadOnly Property Ear As String = "👂"

        Public ReadOnly Property Nose As String = "👃"

        Public ReadOnly Property Lips As String = "👄"

        Public ReadOnly Property Tongue As String = "👅"

        Public ReadOnly Property Point_Up_2 As String = "👆"

        Public ReadOnly Property Point_Down As String = "👇"

        Public ReadOnly Property Point_Left As String = "👈"

        Public ReadOnly Property Point_Right As String = "👉"

        Public ReadOnly Property Punch As String = "👊"

        Public ReadOnly Property Wave As String = "👋"

        Public ReadOnly Property Ok_Hand As String = "👌"

        Public ReadOnly Property Thumbsup As String = "👍"

        Public ReadOnly Property Thumbsdown As String = "👎"

        Public ReadOnly Property Clap As String = "👏"

        Public ReadOnly Property Open_Hands As String = "👐"

        Public ReadOnly Property Crown As String = "👑"

        Public ReadOnly Property Womans_Hat As String = "👒"

        Public ReadOnly Property Eyeglasses As String = "👓"

        Public ReadOnly Property Necktie As String = "👔"

        Public ReadOnly Property Shirt As String = "👕"

        Public ReadOnly Property Jeans As String = "👖"

        Public ReadOnly Property Dress As String = "👗"

        Public ReadOnly Property Kimono As String = "👘"

        Public ReadOnly Property Bikini As String = "👙"

        Public ReadOnly Property Womans_Clothes As String = "👚"

        Public ReadOnly Property Purse As String = "👛"

        Public ReadOnly Property Handbag As String = "👜"

        Public ReadOnly Property Pouch As String = "👝"

        Public ReadOnly Property Mans_Shoe As String = "👞"

        Public ReadOnly Property Athletic_Shoe As String = "👟"

        Public ReadOnly Property High_Heel As String = "👠"

        Public ReadOnly Property Sandal As String = "👡"

        Public ReadOnly Property Boot As String = "👢"

        Public ReadOnly Property Footprints As String = "👣"

        Public ReadOnly Property Bust_In_Silhouette As String = "👤"

        Public ReadOnly Property Boy As String = "👦"

        Public ReadOnly Property Girl As String = "👧"

        Public ReadOnly Property Man As String = "👨"

        Public ReadOnly Property Woman As String = "👩"

        Public ReadOnly Property Family As String = "👪"

        Public ReadOnly Property Couple As String = "👫"

        Public ReadOnly Property Cop As String = "👮"

        Public ReadOnly Property Dancers As String = "👯"

        Public ReadOnly Property Bride_With_Veil As String = "👰"

        Public ReadOnly Property Person_With_Blond_Hair As String = "👱"

        Public ReadOnly Property Man_With_Gua_Pi_Mao As String = "👲"

        Public ReadOnly Property Man_With_Turban As String = "👳"

        Public ReadOnly Property Older_Man As String = "👴"

        Public ReadOnly Property Older_Woman As String = "👵"

        Public ReadOnly Property Baby As String = "👶"

        Public ReadOnly Property Construction_Worker As String = "👷"

        Public ReadOnly Property Princess As String = "👸"

        Public ReadOnly Property Japanese_Ogre As String = "👹"

        Public ReadOnly Property Japanese_Goblin As String = "👺"

        Public ReadOnly Property Ghost As String = "👻"

        Public ReadOnly Property Angel As String = "👼"

        Public ReadOnly Property Alien As String = "👽"

        Public ReadOnly Property Space_Invader As String = "👾"

        Public ReadOnly Property Robot_Face As String = "🤖"

        Public ReadOnly Property Imp As String = "👿"

        Public ReadOnly Property Skull As String = "💀"

        Public ReadOnly Property Information_Desk_Person As String = "💁"

        Public ReadOnly Property Guardsman As String = "💂"

        Public ReadOnly Property Dancer As String = "💃"

        Public ReadOnly Property Lipstick As String = "💄"

        Public ReadOnly Property Nail_Care As String = "💅"

        Public ReadOnly Property Massage As String = "💆"

        Public ReadOnly Property Haircut As String = "💇"

        Public ReadOnly Property Barber As String = "💈"

        Public ReadOnly Property Syringe As String = "💉"

        Public ReadOnly Property Pill As String = "💊"

        Public ReadOnly Property Kiss As String = "💋"

        Public ReadOnly Property Love_Letter As String = "💌"

        Public ReadOnly Property Ring As String = "💍"

        Public ReadOnly Property Gem As String = "💎"

        Public ReadOnly Property Couplekiss As String = "💏"

        Public ReadOnly Property Bouquet As String = "💐"

        Public ReadOnly Property Couple_With_Heart As String = "💑"

        Public ReadOnly Property Wedding As String = "💒"

        Public ReadOnly Property Heartbeat As String = "💓"

        Public ReadOnly Property Broken_Heart As String = "💔"

        Public ReadOnly Property Two_Hearts As String = "💕"

        Public ReadOnly Property Sparkling_Heart As String = "💖"

        Public ReadOnly Property Heartpulse As String = "💗"

        Public ReadOnly Property Cupid As String = "💘"

        Public ReadOnly Property Blue_Heart As String = "💙"

        Public ReadOnly Property Green_Heart As String = "💚"

        Public ReadOnly Property Yellow_Heart As String = "💛"

        Public ReadOnly Property Purple_Heart As String = "💜"

        Public ReadOnly Property Gift_Heart As String = "💝"

        Public ReadOnly Property Revolving_Hearts As String = "💞"

        Public ReadOnly Property Heart_Decoration As String = "💟"

        Public ReadOnly Property Diamond_Shape_With_A_Dot_Inside As String = "💠"

        Public ReadOnly Property Bulb As String = "💡"

        Public ReadOnly Property Anger As String = "💢"

        Public ReadOnly Property Bomb As String = "💣"

        Public ReadOnly Property Zzz As String = "💤"

        Public ReadOnly Property Boom As String = "💥"

        Public ReadOnly Property Sweat_Drops As String = "💦"

        Public ReadOnly Property Droplet As String = "💧"

        Public ReadOnly Property Dash As String = "💨"

        Public ReadOnly Property Poop As String = "💩"

        Public ReadOnly Property Muscle As String = "💪"

        Public ReadOnly Property Dizzy As String = "💫"

        Public ReadOnly Property Speech_Balloon As String = "💬"

        Public ReadOnly Property White_Flower As String = "💮"

        Public ReadOnly Property _100 As String = "💯"

        Public ReadOnly Property Moneybag As String = "💰"

        Public ReadOnly Property Currency_Exchange As String = "💱"

        Public ReadOnly Property Heavy_Dollar_Sign As String = "💲"

        Public ReadOnly Property Credit_Card As String = "💳"

        Public ReadOnly Property Yen As String = "💴"

        Public ReadOnly Property Dollar As String = "💵"

        Public ReadOnly Property Money_With_Wings As String = "💸"

        Public ReadOnly Property Chart As String = "💹"

        Public ReadOnly Property Seat As String = "💺"

        Public ReadOnly Property Computer As String = "💻"

        Public ReadOnly Property Briefcase As String = "💼"

        Public ReadOnly Property Minidisc As String = "💽"

        Public ReadOnly Property Floppy_Disk As String = "💾"

        Public ReadOnly Property Cd As String = "💿"

        Public ReadOnly Property Dvd As String = "📀"

        Public ReadOnly Property File_Folder As String = "📁"

        Public ReadOnly Property Open_File_Folder As String = "📂"

        Public ReadOnly Property Page_With_Curl As String = "📃"

        Public ReadOnly Property Page_Facing_Up As String = "📄"

        Public ReadOnly Property [Date] As String = "📅"

        Public ReadOnly Property Calendar As String = "📆"

        Public ReadOnly Property Card_Index As String = "📇"

        Public ReadOnly Property Chart_With_Upwards_Trend As String = "📈"

        Public ReadOnly Property Chart_With_Downwards_Trend As String = "📉"

        Public ReadOnly Property Bar_Chart As String = "📊"

        Public ReadOnly Property Clipboard As String = "📋"

        Public ReadOnly Property Pushpin As String = "📌"

        Public ReadOnly Property Round_Pushpin As String = "📍"

        Public ReadOnly Property Paperclip As String = "📎"

        Public ReadOnly Property Straight_Ruler As String = "📏"

        Public ReadOnly Property Triangular_Ruler As String = "📐"

        Public ReadOnly Property Bookmark_Tabs As String = "📑"

        Public ReadOnly Property Ledger As String = "📒"

        Public ReadOnly Property Notebook As String = "📓"

        Public ReadOnly Property Notebook_With_Decorative_Cover As String = "📔"

        Public ReadOnly Property Closed_Book As String = "📕"

        Public ReadOnly Property Book As String = "📖"

        Public ReadOnly Property Green_Book As String = "📗"

        Public ReadOnly Property Blue_Book As String = "📘"

        Public ReadOnly Property Orange_Book As String = "📙"

        Public ReadOnly Property Books As String = "📚"

        Public ReadOnly Property Name_Badge As String = "📛"

        Public ReadOnly Property Scroll As String = "📜"

        Public ReadOnly Property Pencil As String = "📝"

        Public ReadOnly Property Telephone_Receiver As String = "📞"

        Public ReadOnly Property Pager As String = "📟"

        Public ReadOnly Property Fax As String = "📠"

        Public ReadOnly Property Satellite As String = "📡"

        Public ReadOnly Property Loudspeaker As String = "📢"

        Public ReadOnly Property Mega As String = "📣"

        Public ReadOnly Property Outbox_Tray As String = "📤"

        Public ReadOnly Property Inbox_Tray As String = "📥"

        Public ReadOnly Property Package As String = "📦"

        Public ReadOnly Property E_Mail As String = "📧"

        Public ReadOnly Property Incoming_Envelope As String = "📨"

        Public ReadOnly Property Envelope_With_Arrow As String = "📩"

        Public ReadOnly Property Mailbox_Closed As String = "📪"

        Public ReadOnly Property Mailbox As String = "📫"

        Public ReadOnly Property Postbox As String = "📮"

        Public ReadOnly Property Newspaper As String = "📰"

        Public ReadOnly Property Iphone As String = "📱"

        Public ReadOnly Property Calling As String = "📲"

        Public ReadOnly Property Vibration_Mode As String = "📳"

        Public ReadOnly Property Mobile_Phone_Off As String = "📴"

        Public ReadOnly Property Signal_Strength As String = "📶"

        Public ReadOnly Property Camera As String = "📷"

        Public ReadOnly Property Video_Camera As String = "📹"

        Public ReadOnly Property Tv As String = "📺"

        Public ReadOnly Property Radio As String = "📻"

        Public ReadOnly Property Vhs As String = "📼"

        Public ReadOnly Property Arrows_Clockwise As String = "🔃"

        Public ReadOnly Property Loud_Sound As String = "🔊"

        Public ReadOnly Property Battery As String = "🔋"

        Public ReadOnly Property Electric_Plug As String = "🔌"

        Public ReadOnly Property Mag As String = "🔍"

        Public ReadOnly Property Mag_Right As String = "🔎"

        Public ReadOnly Property Lock_With_Ink_Pen As String = "🔏"

        Public ReadOnly Property Closed_Lock_With_Key As String = "🔐"

        Public ReadOnly Property Key As String = "🔑"

        Public ReadOnly Property Lock As String = "🔒"

        Public ReadOnly Property Unlock As String = "🔓"

        Public ReadOnly Property Bell As String = "🔔"

        Public ReadOnly Property Bookmark As String = "🔖"

        Public ReadOnly Property Link As String = "🔗"

        Public ReadOnly Property Radio_Button As String = "🔘"

        Public ReadOnly Property Back As String = "🔙"

        Public ReadOnly Property [End] As String = "🔚"

        Public ReadOnly Property [On] As String = "🔛"

        Public ReadOnly Property Soon As String = "🔜"

        Public ReadOnly Property Top As String = "🔝"

        Public ReadOnly Property Underage As String = "🔞"

        Public ReadOnly Property Keycap_Ten As String = "🔟"

        Public ReadOnly Property Capital_Abcd As String = "🔠"

        Public ReadOnly Property Abcd As String = "🔡"

        Public ReadOnly Property _1234 As String = "🔢"

        Public ReadOnly Property Symbols As String = "🔣"

        Public ReadOnly Property Abc As String = "🔤"

        Public ReadOnly Property Fire As String = "🔥"

        Public ReadOnly Property Flashlight As String = "🔦"

        Public ReadOnly Property Wrench As String = "🔧"

        Public ReadOnly Property Hammer As String = "🔨"

        Public ReadOnly Property Nut_And_Bolt As String = "🔩"

        Public ReadOnly Property Knife As String = "🔪"

        Public ReadOnly Property Gun As String = "🔫"

        Public ReadOnly Property Crystal_Ball As String = "🔮"

        Public ReadOnly Property Six_Pointed_Star As String = "🔯"

        Public ReadOnly Property Beginner As String = "🔰"

        Public ReadOnly Property Trident As String = "🔱"

        Public ReadOnly Property Black_Square_Button As String = "🔲"

        Public ReadOnly Property White_Square_Button As String = "🔳"

        Public ReadOnly Property Red_Circle As String = "🔴"

        Public ReadOnly Property Large_Blue_Circle As String = "🔵"

        Public ReadOnly Property Large_Orange_Diamond As String = "🔶"

        Public ReadOnly Property Large_Blue_Diamond As String = "🔷"

        Public ReadOnly Property Small_Orange_Diamond As String = "🔸"

        Public ReadOnly Property Small_Blue_Diamond As String = "🔹"

        Public ReadOnly Property Small_Red_Triangle As String = "🔺"

        Public ReadOnly Property Small_Red_Triangle_Down As String = "🔻"

        Public ReadOnly Property Arrow_Up_Small As String = "🔼"

        Public ReadOnly Property Arrow_Down_Small As String = "🔽"

        Public ReadOnly Property Clock1 As String = "🕐"

        Public ReadOnly Property Clock2 As String = "🕑"

        Public ReadOnly Property Clock3 As String = "🕒"

        Public ReadOnly Property Clock4 As String = "🕓"

        Public ReadOnly Property Clock5 As String = "🕔"

        Public ReadOnly Property Clock6 As String = "🕕"

        Public ReadOnly Property Clock7 As String = "🕖"

        Public ReadOnly Property Clock8 As String = "🕗"

        Public ReadOnly Property Clock9 As String = "🕘"

        Public ReadOnly Property Clock10 As String = "🕙"

        Public ReadOnly Property Clock11 As String = "🕚"

        Public ReadOnly Property Clock12 As String = "🕛"

        Public ReadOnly Property Mount_Fuji As String = "🗻"

        Public ReadOnly Property Tokyo_Tower As String = "🗼"

        Public ReadOnly Property Statue_Of_Liberty As String = "🗽"

        Public ReadOnly Property Japan As String = "🗾"

        Public ReadOnly Property Moyai As String = "🗿"

        Public ReadOnly Property Grin As String = "😁"

        Public ReadOnly Property Joy As String = "😂"

        Public ReadOnly Property Smiley As String = "😃"

        Public ReadOnly Property Smile As String = "😄"

        Public ReadOnly Property Sweat_Smile As String = "😅"

        Public ReadOnly Property Laughing As String = "😆"

        Public ReadOnly Property Wink As String = "😉"

        Public ReadOnly Property Blush As String = "😊"

        Public ReadOnly Property Yum As String = "😋"

        Public ReadOnly Property Relieved As String = "😌"

        Public ReadOnly Property Heart_Eyes As String = "😍"

        Public ReadOnly Property Smirk As String = "😏"

        Public ReadOnly Property Unamused As String = "😒"

        Public ReadOnly Property Sweat As String = "😓"

        Public ReadOnly Property Pensive As String = "😔"

        Public ReadOnly Property Confounded As String = "😖"

        Public ReadOnly Property Kissing_Heart As String = "😘"

        Public ReadOnly Property Kissing_Closed_Eyes As String = "😚"

        Public ReadOnly Property Stuck_Out_Tongue_Winking_Eye As String = "😜"

        Public ReadOnly Property Stuck_Out_Tongue_Closed_Eyes As String = "😝"

        Public ReadOnly Property Disappointed As String = "😞"

        Public ReadOnly Property Angry As String = "😠"

        Public ReadOnly Property Rage As String = "😡"

        Public ReadOnly Property Cry As String = "😢"

        Public ReadOnly Property Persevere As String = "😣"

        Public ReadOnly Property Triumph As String = "😤"

        Public ReadOnly Property Disappointed_Relieved As String = "😥"

        Public ReadOnly Property Fearful As String = "😨"

        Public ReadOnly Property Weary As String = "😩"

        Public ReadOnly Property Sleepy As String = "😪"

        Public ReadOnly Property Tired_Face As String = "😫"

        Public ReadOnly Property Sob As String = "😭"

        Public ReadOnly Property Cold_Sweat As String = "😰"

        Public ReadOnly Property Scream As String = "😱"

        Public ReadOnly Property Astonished As String = "😲"

        Public ReadOnly Property Flushed As String = "😳"

        Public ReadOnly Property Dizzy_Face As String = "😵"

        Public ReadOnly Property Mask As String = "😷"

        Public ReadOnly Property Smile_Cat As String = "😸"

        Public ReadOnly Property Joy_Cat As String = "😹"

        Public ReadOnly Property Smiley_Cat As String = "😺"

        Public ReadOnly Property Heart_Eyes_Cat As String = "😻"

        Public ReadOnly Property Smirk_Cat As String = "😼"

        Public ReadOnly Property Kissing_Cat As String = "😽"

        Public ReadOnly Property Pouting_Cat As String = "😾"

        Public ReadOnly Property Crying_Cat_Face As String = "😿"

        Public ReadOnly Property Scream_Cat As String = "🙀"

        Public ReadOnly Property No_Good As String = "🙅"

        Public ReadOnly Property Ok_Woman As String = "🙆"

        Public ReadOnly Property Bow As String = "🙇"

        Public ReadOnly Property See_No_Evil As String = "🙈"

        Public ReadOnly Property Hear_No_Evil As String = "🙉"

        Public ReadOnly Property Speak_No_Evil As String = "🙊"

        Public ReadOnly Property Raising_Hand As String = "🙋"

        Public ReadOnly Property Raised_Hands As String = "🙌"

        Public ReadOnly Property Person_Frowning As String = "🙍"

        Public ReadOnly Property Person_With_Pouting_Face As String = "🙎"

        Public ReadOnly Property Pray As String = "🙏"

        Public ReadOnly Property Rocket As String = "🚀"

        Public ReadOnly Property Railway_Car As String = "🚃"

        Public ReadOnly Property Bullettrain_Side As String = "🚄"

        Public ReadOnly Property Bullettrain_Front As String = "🚅"

        Public ReadOnly Property Metro As String = "🚇"

        Public ReadOnly Property Station As String = "🚉"

        Public ReadOnly Property Bus As String = "🚌"

        Public ReadOnly Property Busstop As String = "🚏"

        Public ReadOnly Property Ambulance As String = "🚑"

        Public ReadOnly Property Fire_Engine As String = "🚒"

        Public ReadOnly Property Police_Car As String = "🚓"

        Public ReadOnly Property Taxi As String = "🚕"

        Public ReadOnly Property Red_Car As String = "🚗"

        Public ReadOnly Property Blue_Car As String = "🚙"

        Public ReadOnly Property Truck As String = "🚚"

        Public ReadOnly Property Ship As String = "🚢"

        Public ReadOnly Property Speedboat As String = "🚤"

        Public ReadOnly Property Traffic_Light As String = "🚥"

        Public ReadOnly Property Construction As String = "🚧"

        Public ReadOnly Property Rotating_Light As String = "🚨"

        Public ReadOnly Property Triangular_Flag_On_Post As String = "🚩"

        Public ReadOnly Property Door As String = "🚪"

        Public ReadOnly Property No_Entry_Sign As String = "🚫"

        Public ReadOnly Property Smoking As String = "🚬"

        Public ReadOnly Property No_Smoking As String = "🚭"

        Public ReadOnly Property Bike As String = "🚲"

        Public ReadOnly Property Walking As String = "🚶"

        Public ReadOnly Property Mens As String = "🚹"

        Public ReadOnly Property Womens As String = "🚺"

        Public ReadOnly Property Restroom As String = "🚻"

        Public ReadOnly Property Baby_Symbol As String = "🚼"

        Public ReadOnly Property Toilet As String = "🚽"

        Public ReadOnly Property Wc As String = "🚾"

        Public ReadOnly Property Bath As String = "🛀"

        Public ReadOnly Property Articulated_Lorry As String = "🚛"

        Public ReadOnly Property Kissing_Smiling_Eyes As String = "😙"

        Public ReadOnly Property Pear As String = "🍐"

        Public ReadOnly Property Bicyclist As String = "🚴"

        Public ReadOnly Property Rabbit2 As String = "🐇"

        Public ReadOnly Property Clock830 As String = "🕣"

        Public ReadOnly Property Train As String = "🚋"

        Public ReadOnly Property Oncoming_Automobile As String = "🚘"

        Public ReadOnly Property Expressionless As String = "😑"

        Public ReadOnly Property Smiling_Imp As String = "😈"

        Public ReadOnly Property Frowning As String = "😦"

        Public ReadOnly Property No_Mouth As String = "😶"

        Public ReadOnly Property Baby_Bottle As String = "🍼"

        Public ReadOnly Property Non_Potable_Water As String = "🚱"

        Public ReadOnly Property Open_Mouth As String = "😮"

        Public ReadOnly Property Last_Quarter_Moon_With_Face As String = "🌜"

        Public ReadOnly Property Do_Not_Litter As String = "🚯"

        Public ReadOnly Property Sunglasses As String = "😎"

        Public ReadOnly Property [Loop] As String = ChrW(10175)

        Public ReadOnly Property Last_Quarter_Moon As String = "🌗"

        Public ReadOnly Property Grinning As String = "😀"

        Public ReadOnly Property Euro As String = "💶"

        Public ReadOnly Property Clock330 As String = "🕞"

        Public ReadOnly Property Telescope As String = "🔭"

        Public ReadOnly Property Globe_With_Meridians As String = "🌐"

        Public ReadOnly Property Postal_Horn As String = "📯"

        Public ReadOnly Property Stuck_Out_Tongue As String = "😛"

        Public ReadOnly Property Clock1030 As String = "🕥"

        Public ReadOnly Property Pound As String = "💷"

        Public ReadOnly Property Two_Men_Holding_Hands As String = "👬"

        Public ReadOnly Property Tiger2 As String = "🐅"

        Public ReadOnly Property Anguished As String = "😧"

        Public ReadOnly Property Vertical_Traffic_Light As String = "🚦"

        Public ReadOnly Property Confused As String = "😕"

        Public ReadOnly Property Repeat As String = "🔁"

        Public ReadOnly Property Oncoming_Police_Car As String = "🚔"

        Public ReadOnly Property Tram As String = "🚊"

        Public ReadOnly Property Dragon As String = "🐉"

        Public ReadOnly Property Earth_Americas As String = "🌎"

        Public ReadOnly Property Rugby_Football As String = "🏉"

        Public ReadOnly Property Left_Luggage As String = "🛅"

        Public ReadOnly Property Sound As String = "🔉"

        Public ReadOnly Property Clock630 As String = "🕡"

        Public ReadOnly Property Dromedary_Camel As String = "🐪"

        Public ReadOnly Property Oncoming_Bus As String = "🚍"

        Public ReadOnly Property Horse_Racing As String = "🏇"

        Public ReadOnly Property Rooster As String = "🐓"

        Public ReadOnly Property Rowboat As String = "🚣"

        Public ReadOnly Property Customs As String = "🛃"

        Public ReadOnly Property Repeat_One As String = "🔂"

        Public ReadOnly Property Waxing_Crescent_Moon As String = "🌒"

        Public ReadOnly Property Mountain_Railway As String = "🚞"

        Public ReadOnly Property Clock930 As String = "🕤"

        Public ReadOnly Property Put_Litter_In_Its_Place As String = "🚮"

        Public ReadOnly Property Arrows_Counterclockwise As String = "🔄"

        Public ReadOnly Property Clock130 As String = "🕜"

        Public ReadOnly Property Goat As String = "🐐"

        Public ReadOnly Property Pig2 As String = "🐖"

        Public ReadOnly Property Innocent As String = "😇"

        Public ReadOnly Property No_Bicycles As String = "🚳"

        Public ReadOnly Property Light_Rail As String = "🚈"

        Public ReadOnly Property Whale2 As String = "🐋"

        Public ReadOnly Property Train2 As String = "🚆"

        Public ReadOnly Property Earth_Africa As String = "🌍"

        Public ReadOnly Property Shower As String = "🚿"

        Public ReadOnly Property Waning_Gibbous_Moon As String = "🌖"

        Public ReadOnly Property Steam_Locomotive As String = "🚂"

        Public ReadOnly Property Cat2 As String = "🐈"

        Public ReadOnly Property Tractor As String = "🚜"

        Public ReadOnly Property Thought_Balloon As String = "💭"

        Public ReadOnly Property Two_Women_Holding_Hands As String = "👭"

        Public ReadOnly Property Full_Moon_With_Face As String = "🌝"

        Public ReadOnly Property Mouse2 As String = "🐁"

        Public ReadOnly Property Clock430 As String = "🕟"

        Public ReadOnly Property Worried As String = "😟"

        Public ReadOnly Property Rat As String = "🐀"

        Public ReadOnly Property Ram As String = "🐏"

        Public ReadOnly Property Dog2 As String = "🐕"

        Public ReadOnly Property Kissing As String = "😗"

        Public ReadOnly Property Helicopter As String = "🚁"

        Public ReadOnly Property Clock1130 As String = "🕦"

        Public ReadOnly Property No_Mobile_Phones As String = "📵"

        Public ReadOnly Property European_Post_Office As String = "🏤"

        Public ReadOnly Property Ox As String = "🐂"

        Public ReadOnly Property Mountain_Cableway As String = "🚠"

        Public ReadOnly Property Sleeping As String = "😴"

        Public ReadOnly Property Cow2 As String = "🐄"

        Public ReadOnly Property Minibus As String = "🚐"

        Public ReadOnly Property Clock730 As String = "🕢"

        Public ReadOnly Property Aerial_Tramway As String = "🚡"

        Public ReadOnly Property Speaker As String = "🔈"

        Public ReadOnly Property No_Bell As String = "🔕"

        Public ReadOnly Property Mailbox_With_Mail As String = "📬"

        Public ReadOnly Property No_Pedestrians As String = "🚷"

        Public ReadOnly Property Microscope As String = "🔬"

        Public ReadOnly Property Bathtub As String = "🛁"

        Public ReadOnly Property Suspension_Railway As String = "🚟"

        Public ReadOnly Property Crocodile As String = "🐊"

        Public ReadOnly Property Mountain_Bicyclist As String = "🚵"

        Public ReadOnly Property Waning_Crescent_Moon As String = "🌘"

        Public ReadOnly Property Monorail As String = "🚝"

        Public ReadOnly Property Children_Crossing As String = "🚸"

        Public ReadOnly Property Clock230 As String = "🕝"

        Public ReadOnly Property Busts_In_Silhouette As String = "👥"

        Public ReadOnly Property Mailbox_With_No_Mail As String = "📭"

        Public ReadOnly Property Leopard As String = "🐆"

        Public ReadOnly Property Deciduous_Tree As String = "🌳"

        Public ReadOnly Property Oncoming_Taxi As String = "🚖"

        Public ReadOnly Property Lemon As String = "🍋"

        Public ReadOnly Property Mute As String = "🔇"

        Public ReadOnly Property Baggage_Claim As String = "🛄"

        Public ReadOnly Property Twisted_Rightwards_Arrows As String = "🔀"

        Public ReadOnly Property Sun_With_Face As String = "🌞"

        Public ReadOnly Property Trolleybus As String = "🚎"

        Public ReadOnly Property Evergreen_Tree As String = "🌲"

        Public ReadOnly Property Passport_Control As String = "🛂"

        Public ReadOnly Property New_Moon_With_Face As String = "🌚"

        Public ReadOnly Property Potable_Water As String = "🚰"

        Public ReadOnly Property High_Brightness As String = "🔆"

        Public ReadOnly Property Low_Brightness As String = "🔅"

        Public ReadOnly Property Clock530 As String = "🕠"

        Public ReadOnly Property Hushed As String = "😯"

        Public ReadOnly Property Grimacing As String = "😬"

        Public ReadOnly Property Water_Buffalo As String = "🐃"

        Public ReadOnly Property Neutral_Face As String = "😐"

        Public ReadOnly Property Clock1230 As String = "🕧"
    End Class

End Namespace