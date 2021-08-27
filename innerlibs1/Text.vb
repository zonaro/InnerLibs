Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Imports System.Xml
Imports InnerLibs.LINQ

''' <summary>
''' Modulo de manipulação de Texto
''' </summary>
''' <remarks></remarks>
Public Module Text

    <Extension()> Public Function ToFormattableString(Text As String, ParamArray args As Object()) As FormattableString
        Return FormattableStringFactory.Create(Text, If(args, {}))
    End Function

    <Extension()> Public Function ToFormattableString(Text As String, args As IEnumerable(Of Object())) As FormattableString
        Return FormattableStringFactory.Create(Text, If(args, {}))
    End Function

    ''' <summary>
    ''' Parseia uma ConnectionString em um Dicionário
    ''' </summary>
    ''' <param name="ConnectionString"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ParseConnectionString(ByVal ConnectionString As String) As ConnectionStringParser
        Try
            Return New ConnectionStringParser(ConnectionString)
        Catch ex As Exception
            Return New ConnectionStringParser
        End Try
    End Function

    <Extension()> Public Function WrapInTag(Text As String, ByVal TagName As String) As HtmlTag
        Return New HtmlTag() With {.InnerHtml = Text, .TagName = TagName}
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="querystring"></param>
    ''' <returns></returns>
    <Extension()> Function ParseQueryString(Querystring As String) As NameValueCollection
        Dim queryParameters = New NameValueCollection()
        Dim querySegments As String() = Querystring.Split("&"c)
        For Each segment As String In querySegments
            Dim parts As String() = segment.Split("="c)
            If parts.Any Then
                Dim key As String = parts(0).Trim(New Char() {"?"c, " "c})
                Dim val = ""
                If parts.Skip(1).Any Then
                    val = parts(1).Trim()
                End If
                queryParameters.Add(key, val.UrlDecode())
            End If
        Next
        Return queryParameters
    End Function

    ''' <summary>
    ''' Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Patterns"></param>
    ''' <returns></returns>
    <Extension> Function IsLikeAny(Text As String, Patterns As IEnumerable(Of String)) As Boolean
        Text = Text.IfBlank("")
        For Each item In If(Patterns, {})
            If item Like Text Or Text Like item Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Patterns"></param>
    ''' <returns></returns>
    <Extension> Function IsLikeAny(Text As String, ParamArray Patterns As String()) As Boolean
        Return Text.IsLikeAny(If(Patterns, {}).AsEnumerable)
    End Function

    ''' <summary>
    ''' operador LIKE do VB para C# em forma de extension method
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="OtherText"></param>
    ''' <returns></returns>
    <Extension> Function [Like](Text As String, OtherText As String) As Boolean
        Return Text Like OtherText
    End Function

    ''' <summary>
    ''' Formata um numero para CNPJ ou CNPJ se forem validos
    ''' </summary>
    ''' <param name="Document"></param>
    ''' <returns></returns>
    <Extension()> Public Function FormatCPFOrCNPJ(Document As Long) As String
        If Document.ToString().IsValidCPF() Then
            Return Document.FormatCPF()
        End If
        If Document.ToString().IsValidCNPJ() Then
            Return Document.FormatCNPJ()
        End If
        Return Document.ToString()
    End Function

    ''' <summary>
    ''' Formata um numero para CNPJ
    ''' </summary>
    ''' <param name="CNPJ"></param>
    ''' <returns></returns>
    <Extension()> Public Function FormatCNPJ(CNPJ As Long) As String
        Return String.Format("{0:00\.000\.000\/0000\-00}", CNPJ)
    End Function

    ''' <summary>
    ''' Formata um numero para CNPJ
    ''' </summary>
    ''' <param name="CNPJ"></param>
    ''' <returns></returns>
    <Extension()> Public Function FormatCNPJ(CNPJ As String) As String
        If CNPJ.IsValidCNPJ Then
            If CNPJ.IsNumber Then CNPJ = CNPJ.ToLong.FormatCNPJ()
        Else
            Throw New FormatException("String is not a valid CNPJ")
        End If
        Return CNPJ
    End Function

    ''' <summary>
    ''' Formata um numero para CPF
    ''' </summary>
    ''' <param name="CPF"></param>
    ''' <returns></returns>
    <Extension()> Public Function FormatCPF(CPF As Long) As String
        Return String.Format("{0:000\.000\.000\-00}", CPF)
    End Function

    ''' <summary>
    ''' Formata um numero para CPF
    ''' </summary>
    ''' <param name="CPF"></param>
    ''' <returns></returns>
    <Extension()> Public Function FormatCPF(CPF As String) As String
        If CPF.IsValidCPF Then
            If CPF.IsNumber Then CPF = CPF.ToLong.FormatCPF()
        Else
            Throw New FormatException("String is not a valid CPF")
        End If
        Return CPF
    End Function

    ''' <summary>
    ''' Retorna a string especificada se o valor booleano for verdadeiro
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="BooleanValue"></param>
    ''' <returns></returns>
    <Extension()> Public Function PrintIf(Text As String, BooleanValue As Boolean) As String
        Return If(BooleanValue, Text, "")
    End Function

    ''' <summary>
    ''' Seprar uma string em varias partes a partir de varias strings removendo as entradas em branco
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="SplitText"></param>
    ''' <returns></returns>
    <Extension()> Public Function SplitAny(Text As String, ParamArray SplitText As String()) As String()
        SplitText = If(SplitText, {})
        Return Text.Split(SplitText, StringSplitOptions.RemoveEmptyEntries)
    End Function

    ''' <summary>
    ''' Substitui a ultima ocorrencia de um texto por outro
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="OldText"></param>
    ''' <param name="NewText"></param>
    ''' <returns></returns>
    <Extension()> Public Function ReplaceLast(Text As String, OldText As String, Optional NewText As String = "") As String
        If Text.Contains(OldText) Then
            Text = Text.Insert(Text.LastIndexOf(OldText), NewText)
            Text = Text.Remove(Text.LastIndexOf(OldText), 1)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Substitui a primeira ocorrencia de um texto por outro
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="OldText"></param>
    ''' <param name="NewText"></param>
    ''' <returns></returns>
    <Extension()> Public Function ReplaceFirst(Text As String, OldText As String, Optional NewText As String = "") As String
        If Text.Contains(OldText) Then
            Text = Text.Insert(Text.IndexOf(OldText), NewText)
            Text = Text.Remove(Text.IndexOf(OldText), 1)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Caracteres usado para encapsular palavras em textos
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property WordWrappers As IEnumerable(Of String)
        Get
            Return OpenWrappers.Union(CloseWrappers)
        End Get
    End Property

    ReadOnly Property AlphaLowerChars As IEnumerable(Of String)
        Get
            Return Consonants.Union(Vowels).OrderBy(Function(x) x).AsEnumerable
        End Get
    End Property

    ReadOnly Property AlphaUpperChars As IEnumerable(Of String)
        Get
            Return AlphaLowerChars.Select(Function(x) x.ToUpper())
        End Get
    End Property

    ReadOnly Property AlphaChars As IEnumerable(Of String)
        Get
            Return AlphaUpperChars.Union(AlphaLowerChars).OrderBy(Function(x) x).AsEnumerable
        End Get
    End Property

    ReadOnly Property NumberChars As IEnumerable(Of String)
        Get
            Return {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}.AsEnumerable
        End Get
    End Property

    ReadOnly Property BreakLineChars As IEnumerable(Of String)
        Get
            Return {Environment.NewLine, vbCr, vbLf, vbCrLf, vbNewLine}.AsEnumerable
        End Get
    End Property

    ReadOnly Property CloseWrappers As IEnumerable(Of String)
        Get
            Return {"""", "'", ")", "}", "]", ">"}.AsEnumerable
        End Get
    End Property

    ReadOnly Property EndOfSentencePunctuation As IEnumerable(Of String)
        Get
            Return {".", "?", "!"}.AsEnumerable
        End Get
    End Property

    ReadOnly Property MidSentencePunctuation As IEnumerable(Of String)
        Get
            Return {":", ";", ","}.AsEnumerable
        End Get
    End Property

    ReadOnly Property OpenWrappers As IEnumerable(Of String)
        Get
            Return {"""", "'", "(", "{", "[", "<"}.AsEnumerable
        End Get
    End Property

    ''' <summary>
    ''' Caracteres em branco
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property WhiteSpaceChars As IEnumerable(Of String)
        Get
            Return {Environment.NewLine, " ", vbTab, vbLf, vbCr, vbCrLf}.AsEnumerable
        End Get
    End Property

    ''' <summary>
    ''' Strings utilizadas para descobrir as palavras em uma string
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property WordSplitters As IEnumerable(Of String)
        Get
            Return {"&nbsp;", """", "'", "(", ")", ",", ".", "?", "!", ";", "{", "}", "[", "]", "|", " ", ":", vbNewLine, "<br>", "<br/>", "<br/>", Environment.NewLine, vbCr, vbCrLf}.AsEnumerable
        End Get
    End Property

    Public ReadOnly Property Consonants As IEnumerable(Of String)
        Get
            Return {"b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z"}.AsEnumerable
        End Get
    End Property

    Public ReadOnly Property Vowels As IEnumerable(Of String)
        Get
            Return {"a", "e", "i", "o", "u"}.AsEnumerable
        End Get
    End Property



    <Extension()>
    Public Function AdjustBlankSpaces(ByVal Text As String) As String
        Return AdjustWhiteSpaces(Text)
    End Function

    <Extension()>
    Public Function AdjustWhiteSpaces(ByVal Text As String) As String
        Text = Text.IfBlank("")
        If Text.IsNotBlank Then
            'adiciona espaco quando nescessario
            Text = Text.Replace(")", ") ")
            Text = Text.Replace("]", "] ")
            Text = Text.Replace("}", "} ")
            Text = Text.Replace(">", "> ")
            Text = Text.Replace("(", " (")
            Text = Text.Replace("<", " <")
            Text = Text.Replace("[", " [")
            Text = Text.Replace("{", " {")
            Text = Text.Replace(":", ": ")
            Text = Text.Replace(";", "; ")

            For Each item In AlphaChars
                Text = Text.SensitiveReplace($" -{item}", $" - {item}")
            Next

            Text = Text.Replace("- ", " - ")
            Text = Text.Replace("""", " """)

            'remove espaco quando nescessario
            Text = Text.Replace(" ,", ",")
            Text = Text.Replace(" .", ".")
            Text = Text.Replace(" !", "!")
            Text = Text.Replace(" ?", "?")
            Text = Text.Replace(" ;", ";")
            Text = Text.Replace(" :", ":")
            Text = Text.Replace(" )", ")")
            Text = Text.Replace(" ]", "]")
            Text = Text.Replace(" }", "}")
            Text = Text.Replace(" >", ">")
            Text = Text.Replace("( ", "(")
            Text = Text.Replace("[ ", "[")
            Text = Text.Replace("{ ", "{")
            Text = Text.Replace("< ", "<")
            Text = Text.Replace(""" ", """")

            Dim arr = Text.Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
            Text = arr.Join(Environment.NewLine)
            arr = Text.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)
            Text = arr.Join(" ")

        End If

        Return Text.TrimAny(" ", Environment.NewLine)

    End Function

    ''' <summary>
    ''' Retorna uma string em ordem afabética baseada em uma outra string
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function Alphabetize(Text As String) As String
        Dim a = Text.ToCharArray()
        Array.Sort(a)
        Return a.Join("")
    End Function

    <Extension()>
    Public Function AppendUrlParameter(Url As String, Key As String, ParamArray Value As String()) As String
        For Each v In If(Value, {})
            Url &= (String.Format("&{0}={1}", Key, v.IfBlank("")))
        Next
        Return Url
    End Function

    ''' <summary>
    ''' Adiciona texto ao final de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="AppendText">Texto adicional</param>
    <Extension()>
    Public Function AppendLine(Text As String, AppendText As String) As String
        Return Text.Append(AppendText).Append(Environment.NewLine)
    End Function

    ''' <summary>
    ''' Adiciona texto ao inicio de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="AppendText">Texto adicional</param>
    <Extension()>
    Public Function PrependLine(Text As String, AppendText As String) As String
        Return Text.Prepend(Environment.NewLine).Prepend(AppendText)
    End Function

    ''' <summary>
    ''' Adiciona texto ao final de uma string se um criterio for cumprido
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="AppendText">Texto adicional</param>
    ''' <param name="Test">      Teste</param>
    <Extension()> Public Function AppendIf(Text As String, AppendText As String, Test As Boolean) As String
        If Test Then
            Return Text.Append(AppendText)
        End If
        Return If(Text, "")
    End Function

    ''' <summary>
    ''' Adiciona texto ao final de uma string se um criterio for cumprido
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="AppendText">Texto adicional</param>
    ''' <param name="Test">      Teste</param>
    <Extension()> Public Function AppendIf(Text As String, AppendText As String, Test As Func(Of String, Boolean)) As String
        Test = If(Test, Function(x) False)
        Return Text.AppendIf(AppendText, Test(Text))
    End Function

    ''' <summary>
    ''' Adiciona texto ao final de uma string se um criterio for cumprido
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="PrependText">Texto adicional</param>
    ''' <param name="Test">      Teste</param>
    <Extension()> Public Function PrependIf(ByVal Text As String, PrependText As String, Test As Func(Of String, Boolean)) As String
        Test = If(Test, Function(x) False)
        Return Text.PrependIf(PrependText, Test(Text))
    End Function

    ''' <summary>
    ''' Adiciona texto ao inicio de uma string enquanto um criterio for cumprido
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="PrependText">Texto adicional</param>
    ''' <param name="Test">      Teste</param>
    <Extension()> Public Function PrependWhile(ByVal Text As String, PrependText As String, Test As Func(Of String, Boolean)) As String
        Test = If(Test, Function(x) False)
        While Test(Text)
            Text = Text.Prepend(PrependText)
        End While
        Return Text
    End Function

    ''' <summary>
    ''' Adiciona texto ao final de uma string enquanto um criterio for cumprido
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="AppendText">Texto adicional</param>
    ''' <param name="Test">      Teste</param>
    <Extension()> Public Function AppendWhile(ByVal Text As String, AppendText As String, Test As Func(Of String, Boolean)) As String
        Test = If(Test, Function(x) False)
        While Test(Text)
            Text = Text.Append(AppendText)
        End While
        Return Text
    End Function

    ''' <summary>
    ''' Aplica espacos em todos os caracteres de encapsulamento
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension> Function ApplySpaceOnWrapChars(Text As String) As String
        For Each c In WordWrappers
            Text = Text.Replace(c, " " & c & " ")
        Next
        Return Text
    End Function

    ''' <summary>
    ''' Transforma um texto em CamelCase em um array de palavras  a partir de suas letras maíusculas
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function CamelSplit(Text As String) As IEnumerable(Of String)
        Return Text.CamelAdjust().Split(" ")
    End Function

    ''' <summary>
    ''' Separa as palavras de um texto CamelCase a partir de suas letras maíusculas
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function CamelAdjust(Text As String) As String
        Text = Text.IfBlank("")
        Dim chars = Text.ToArray
        Text = ""
        For Each c In chars
            If Char.IsUpper(c) Then
                Text &= " "
            End If
            Text &= c
        Next
        Return Text.Trim
    End Function

    ''' <summary>
    ''' Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro
    ''' caractere desejado) e retorna um valor indicando se o texto precisou ser censurado
    ''' </summary>
    ''' <param name="Text">               Texto</param>
    ''' <param name="BadWords">           Lista de palavras indesejadas</param>
    ''' <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
    ''' <returns>TRUE se a frase precisou ser censurada, FALSE se a frase não precisou de censura</returns>
    <Extension()>
    Public Function Censor(ByVal Text As String, BadWords As IEnumerable(Of String), Optional CensorshipCharacter As String = "*", ByRef Optional IsCensored As Boolean = False) As String
        Dim words As String() = Text.Split(" ", StringSplitOptions.None)
        BadWords = If(BadWords, {})
        If words.ContainsAny(BadWords) Then
            For Each bad In BadWords
                Dim censored = ""
                For index = 1 To bad.Length
                    censored &= (CensorshipCharacter)
                Next
                For index = 0 To words.Length - 1
                    If words(index).RemoveDiacritics.RemoveAny(WordSplitters.ToArray()).ToLower = bad.RemoveDiacritics.RemoveAny(WordSplitters.ToArray).ToLower Then
                        words(index) = words(index).ToLower().Replace(bad, censored)
                        IsCensored = True
                    End If
                Next
            Next
            Text = words.Join(" ")
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Retorna um novo texto censurando as palavras de um texto substituindo as palavras indesejadas
    ''' por um caractere desejado)
    ''' </summary>
    ''' <param name="Text">               Texto</param>
    ''' <param name="BadWords">           Array de palavras indesejadas</param>
    ''' <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
    <Extension()>
    Public Function Censor(ByVal Text As String, CensorshipCharacter As String, ParamArray BadWords As String()) As String
        Return Text.Censor(If(BadWords, {}).ToList, CensorshipCharacter)
    End Function

    ''' <summary>
    ''' Verifica se uma string contém a maioria dos valores especificados
    ''' </summary>
    ''' <param name="Text">  Texto correspondente</param>
    ''' <param name="Values">Lista de valores</param>
    ''' <returns>True se conter a maioria dos valores, false se não</returns>
    <Extension>
    Public Function ContainsMost(Text As String, ComparisonType As StringComparison, ParamArray Values As String()) As Boolean
        Return If(Values, {}).Most(Function(value) Text IsNot Nothing AndAlso Text.Contains(value, ComparisonType))
    End Function

    ''' <summary>
    ''' Verifica se uma string contém a maioria dos valores especificados
    ''' </summary>
    ''' <param name="Text">  Texto correspondente</param>
    ''' <param name="Values">Lista de valores</param>
    ''' <returns>True se conter todos os valores, false se não</returns>
    <Extension>
    Public Function ContainsMost(Text As String, ParamArray Values As String()) As Boolean
        Return Text.ContainsMost(StringComparison.InvariantCultureIgnoreCase, Values)
    End Function

    ''' <summary>
    ''' Verifica se uma String contém todos os valores especificados
    ''' </summary>
    ''' <param name="Text">  Texto correspondente</param>
    ''' <param name="Values">Lista de valores</param>
    ''' <returns>True se conter todos os valores, false se não</returns>
    <Extension>
    Public Function ContainsAll(Text As String, ParamArray Values As String()) As Boolean
        Return Text.ContainsAll(StringComparison.InvariantCultureIgnoreCase, Values)
    End Function

    ''' <summary>
    ''' Verifica se uma String contém todos os valores especificados
    ''' </summary>
    ''' <param name="Text">          Texto correspondente</param>
    ''' <param name="Values">        Lista de valores</param>
    ''' <param name="ComparisonType">Tipo de comparacao</param>
    ''' <returns>True se conter algum valor, false se não</returns>
    <Extension>
    Public Function ContainsAll(Text As String, ComparisonType As StringComparison, ParamArray Values As String()) As Boolean
        Values = If(Values, {})
        If Values.Any Then
            For Each value As String In Values
                If Text Is Nothing OrElse Text.IndexOf(value, ComparisonType) = -1 Then
                    Return False
                End If
            Next
            Return True
        End If
        Return Text.IsBlank
    End Function

    ''' <summary>
    ''' Verifica se uma String contém qualquer um dos valores especificados
    ''' </summary>
    ''' <param name="Text">  Texto correspondente</param>
    ''' <param name="Values">Lista de valores</param>
    ''' <returns>True se conter algum valor, false se não</returns>
    <Extension>
    Public Function ContainsAny(Text As String, ParamArray Values As String()) As Boolean
        Return Text.ContainsAny(StringComparison.InvariantCultureIgnoreCase, Values)
    End Function

    ''' <summary>
    ''' Verifica se uma String contém qualquer um dos valores especificados
    ''' </summary>
    ''' <param name="Text">          Texto correspondente</param>
    ''' <param name="Values">        Lista de valores</param>
    ''' <param name="ComparisonType">Tipo de comparacao</param>
    ''' <returns>True se conter algum valor, false se não</returns>
    <Extension>
    Public Function ContainsAny(Text As String, ComparisonType As StringComparison, ParamArray Values As String()) As Boolean
        Values = If(Values, {})
        If Values.Any Then
            For Each value As String In If(Values, {})
                If Text IsNot Nothing AndAlso Text.IndexOf(value, ComparisonType) <> -1 Then
                    Return True
                End If
            Next
            Return False
        Else
            Return Text.IsNotBlank
        End If
    End Function

    ''' <summary>
    ''' Conta os caracters especificos de uma string
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="Character">Caractere</param>
    ''' <returns></returns>
    <Extension>
    Public Function CountCharacter(ByVal Text As String, ByVal Character As Char) As Integer
        Return Text.Count(Function(c As Char) c = Character)
    End Function

    ''' <summary>
    ''' Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade
    ''' </summary>
    ''' <param name="Text">            TExto</param>
    ''' <param name="RemoveDiacritics">indica se os acentos devem ser removidos das palavras</param>
    ''' <param name="Words">
    ''' Desconsidera outras palavras e busca a quantidadade de cada palavra especificada em um array
    ''' </param>
    ''' <returns></returns>
    <Extension()> Function CountWords(Text As String, Optional RemoveDiacritics As Boolean = True, Optional Words As String() = Nothing) As Dictionary(Of String, Long)
        If Words Is Nothing Then Words = {}
        Dim palavras = Text.Split(WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray

        If Words.Any Then
            palavras = palavras.Where(Function(x) Words.Select(Function(y) y.ToLower).Contains(x.ToLower)).ToArray
        End If

        If RemoveDiacritics Then
            palavras = palavras.Select(Function(p) p.RemoveDiacritics).ToArray
            Words = Words.Select(Function(p) p.RemoveDiacritics).ToArray
        End If

        Dim dic As Dictionary(Of String, Long) = palavras.DistinctCount()

        For Each w In Words.Where(Function(x) Not dic.Keys.Contains(x))
            dic.Add(w, 0)
        Next
        Return dic
    End Function

    ''' <summary>
    ''' Remove uma linha especifica de um texto
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <param name="LineIndex">Numero da linha</param>
    ''' <returns></returns>
    <Extension()>
    Public Function DeleteLine(ByVal Text As String, LineIndex As Integer) As String
        Dim parts As New List(Of String)
        Dim strReader = New StringReader(Text)
        Dim NewText As String = ""
        LineIndex = LineIndex.SetMinValue(0)
        While True
            NewText = strReader.ReadLine()
            If NewText Is Nothing Then
                Exit While
            Else
                parts.Add(NewText)
            End If
        End While
        NewText = ""
        If parts.Count > LineIndex Then
            parts.RemoveAt(LineIndex)
        End If
        For Each part In parts
            NewText = NewText & part & Environment.NewLine
        Next
        Return NewText
    End Function

    ''' <summary>
    ''' Cria um dicionário com as palavras de uma lista e a quantidade de cada uma.
    ''' </summary>
    ''' <param name="List">Lista de palavras</param>
    ''' <returns></returns>
    Public Function DistinctCount(ParamArray List As String()) As Dictionary(Of String, Long)
        Return List.ToList.DistinctCount
    End Function

    ''' <summary>
    ''' Cria um dicionário com as palavras de uma frase e sua respectiva quantidade.
    ''' </summary>
    ''' <param name="Phrase">Lista de palavras</param>
    ''' <returns></returns>
    <Extension()> Function DistinctCount(Phrase As String) As Dictionary(Of String, Long)
        Return Phrase.Split(" ").ToList.DistinctCount
    End Function

    ''' <summary>
    ''' Verifica se uma string termina com alguma outra string de um array
    ''' </summary>
    ''' <param name="Text"> </param>
    ''' <param name="Words"></param>
    ''' <returns></returns>
    <Extension()> Function EndsWithAny(Text As String, ParamArray Words As String()) As Boolean
        Return Words.Any(Function(p) Text.EndsWith(p))
    End Function

    ''' <summary>
    ''' Prepara uma string com aspas simples para uma Query TransactSQL
    ''' </summary>
    ''' <param name="Text">Texto a ser tratado</param>
    ''' <returns>String pornta para a query</returns>
    <Extension()>
    Public Function EscapeQuotesToQuery(Text As String) As String
        Return Text.Replace("'", "''")
    End Function

    ''' <summary>
    ''' Procura numeros em uma string e retorna um array deles
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function FindNumbers(Text As String) As IEnumerable(Of String)
        Dim l As New List(Of String)
        Dim numbers As String() = Regex.Split(Text, "\D+")
        For Each value In numbers
            If Not value.IsBlank Then
                l.Add(value)
            End If
        Next
        Return l
    End Function

    ''' <summary>
    ''' Procura CEPs em uma string
    ''' </summary>
    ''' <param name="TExt"></param>
    ''' <returns></returns>
    <Extension()> Public Function FindCEP(Text As String) As String()
        Return Text.FindByRegex("\d{5}-\d{3}").Union(Text.FindNumbers().Where(Function(x) x.Length = 8)).ToArray()
    End Function

    ''' <summary>
    ''' Procura CEPs em uma string
    ''' </summary>
    ''' <param name="TExt"></param>
    ''' <returns></returns>
    <Extension()> Public Function FindByRegex(Text As String, Regex As String, Optional RegexOptions As RegexOptions = RegexOptions.None) As String()
        Dim textos As New List(Of String)
        For Each m As Match In New Regex(Regex, RegexOptions).Matches(Text)
            textos.Add(m.Value)
        Next
        Return textos.ToArray()
    End Function

    ''' <summary>
    ''' Procurea numeros de telefone em um texto
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function FindTelephoneNumbers(Text As String) As String()
        Return Text.FindByRegex("\b[\s()\d-]{6,}\d\b", RegexOptions.Singleline + RegexOptions.IgnoreCase).Select(Function(x) x.MaskTelephoneNumber()).ToArray()
    End Function

    ''' <summary>
    ''' Transforma quebras de linha HTML em quebras de linha comuns ao .net
    ''' </summary>
    ''' <param name="Text">Texto correspondente</param>
    ''' <returns>String fixada</returns>
    <Extension>
    Public Function FixBreakLines(Text As String) As String
        Return Text.ReplaceMany(vbCr & vbLf, "<br/>", "<br />", "<br>")
        Return Text.Replace("&nbsp;", " ")
    End Function

    <Extension()>
    Public Function FixCaptalization(ByVal Text As String) As String
        Text = Text.Trim().GetFirstChars(1).ToUpper() & Text.RemoveFirstChars(1)
        Dim dots As String() = {"...", ". ", "? ", "! "}
        Dim sentences As List(Of String)
        For Each dot In dots
            sentences = Text.Split(dot, StringSplitOptions.None).ToList
            For index = 0 To sentences.Count - 1
                sentences(index) = "" & sentences(index).Trim().GetFirstChars(1).ToUpper() & sentences(index).RemoveFirstChars(1)
            Next
            Text = sentences.Join(dot)
        Next

        sentences = Text.Split(" ").ToList
        Text = ""
        For Each c In sentences
            Dim palavra = c
            If palavra.EndsWith(".") AndAlso palavra.Length = 2 Then
                palavra = palavra.ToUpper
                Text &= (palavra)

                Dim proximapalavra = sentences.IfNoIndex(sentences.IndexOf(c) + 1, "")
                If Not (proximapalavra.EndsWith(".") AndAlso palavra.Length = 2) Then
                    Text &= (" ")
                End If
            Else
                Text &= (c & " ")
            End If
        Next

        Return Text.RemoveLastChars(1)
    End Function

    ''' <summary>
    ''' Adciona pontuaçao ao final de uma string se a mesma não terminar com alguma pontuacao.
    ''' </summary>
    ''' <param name="Text">       Frase, Texto a ser pontuado</param>
    ''' <param name="Punctuation">Ponto a ser adicionado na frase se a mesma não estiver com pontuacao</param>
    ''' <returns>Frase corretamente pontuada</returns>
    <Extension>
    Public Function FixPunctuation(ByRef Text As String, Optional Punctuation As String = ".", Optional ForceSpecificPunctuation As Boolean = False) As String
        Text = Text.RemoveLastAny(True, ",", " ")
        Dim pts = {".", "!", "?", ":", ";"}
        If ForceSpecificPunctuation Then
            Text = Text.RemoveLastAny(True, pts).Trim & Punctuation
        Else
            If Not Text.EndsWithAny(pts) Then
                Text = Text & Punctuation
            End If
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Arruma a ortografia do texto captalizando corretamente, adcionando pontução ao final de frase
    ''' caso nescessário e removendo espaços excessivos ou incorretos
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()> Public Function FixText(ByVal Text As String, Optional Ident As Integer = 0, Optional BreakLinesBetweenParagraph As Integer = 0) As String
        Return New StructuredText(Text) With {.Ident = Ident, .BreakLinesBetweenParagraph = BreakLinesBetweenParagraph}.ToString
    End Function

    ''' <summary>
    ''' Extension Method para <see cref="String.Format(String,Object())"/>
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <param name="Args">Objetos de substituição</param>
    ''' <returns></returns>
    <Extension> Public Function Format(Text As String, ParamArray Args As String()) As String
        Return String.Format(Text, Args)
    End Function

    ''' <summary>
    ''' Retorna um texto posterior a outro
    ''' </summary>
    ''' <param name="Text"> Texto correspondente</param>
    ''' <param name="Value">Texto Posterior</param>
    ''' <returns>Uma string com o valor posterior ao valor especificado.</returns>
    <Extension>
    Public Function GetAfter(Text As String, Value As String, Optional WhiteIfNotFound As Boolean = False) As String
        If Nothing = (Value) Then Value = ""
        If Nothing = (Text) OrElse Text.IndexOf(Value) = -1 Then
            If WhiteIfNotFound Then
                Return ""
            Else
                Return "" & Text
            End If
        End If
        Return Text.Substring(Text.IndexOf(Value) + Value.Length)
    End Function

    ''' <summary>
    ''' Retorna todas as ocorrencias de um texto entre dois textos
    ''' </summary>
    ''' <param name="Text">  O texto correspondente</param>
    ''' <param name="Before">O texto Anterior</param>
    ''' <param name="After"> O texto Posterior</param>
    ''' <returns>Uma String com o texto entre o texto anterior e posterior</returns>
    <Extension()> Public Function GetAllBetween(Text As String, Before As String, Optional After As String = "") As String()
        Dim lista As New List(Of String)
        Dim regx = Before.RegexEscape & "(.*?)" & After.IfBlank(Before).RegexEscape
        Dim mm = New Regex(regx, RegexOptions.Singleline + RegexOptions.IgnoreCase).Matches(Text)
        For Each a As Match In mm
            lista.Add(a.Value.RemoveFirstEqual(Before).RemoveLastEqual(After))
        Next
        Return lista.ToArray
    End Function

    ''' <summary>
    ''' Retorna um texto anterior a outro
    ''' </summary>
    ''' <param name="Text"> Texto correspondente</param>
    ''' <param name="Value">Texto Anterior</param>
    ''' <returns>Uma string com o valor anterior ao valor especificado.</returns>
    <Extension>
    Public Function GetBefore(Text As String, Value As String, Optional WhiteIfNotFound As Boolean = False) As String
        If Value Is Nothing Then Value = ""
        If Text Is Nothing OrElse Text.IndexOf(Value) = -1 Then
            If WhiteIfNotFound Then
                Return ""
            Else
                Return "" & Text
            End If
        End If
        Return Text.Substring(0, Text.IndexOf(Value))
    End Function

    ''' <summary>
    ''' Retorna o texto entre dois textos
    ''' </summary>
    ''' <param name="Text">  O texto correspondente</param>
    ''' <param name="Before">O texto Anterior</param>
    ''' <param name="After"> O texto Posterior</param>
    ''' <returns>Uma String com o texto entre o texto anterior e posterior</returns>
    <Extension>
    Public Function GetBetween(Text As String, Before As String, After As String) As String
        If Text.IsBlank() Then
            Return ""
        End If
        Dim beforeStartIndex As Integer = Text.IndexOf(Before)
        Dim startIndex As Integer = beforeStartIndex + Before.Length
        Dim afterStartIndex As Integer = Text.IndexOf(After, startIndex)

        If beforeStartIndex < 0 OrElse afterStartIndex < 0 Then
            Return Text
        End If

        Return Text.Substring(startIndex, afterStartIndex - startIndex)
    End Function

    ''' <summary>
    ''' Pega o dominio principal de uma URL
    ''' </summary>
    ''' <param name="URL">URL</param>
    ''' <returns>nome do dominio</returns>
    <Extension>
    Public Function GetDomain(URL As Uri, Optional RemoveFirstSubdomain As Boolean = False) As String
        Dim d = URL.GetLeftPart(UriPartial.Authority).RemoveAny("http://", "https://", "www.")
        If RemoveFirstSubdomain Then
            Dim parts = d.Split(".").ToList
            parts.Remove(parts.Item(0))
            d = parts.Join(".")
        End If
        Return d
    End Function

    ''' <summary>
    ''' Pega o dominio principal de uma URL
    ''' </summary>
    ''' <param name="URL">URL</param>
    ''' <returns>nome do dominio</returns>
    <Extension>
    Public Function GetDomain(URL As String, Optional RemoveFirstSubdomain As Boolean = False) As String
        Return New Uri(URL).GetDomain(RemoveFirstSubdomain)
    End Function

    <Extension()>
    Public Function GetFirstChars(Text As String, Optional Number As Integer = 1) As String
        If Text.IsNotBlank Then
            If Text.Length < Number Or Number < 1 Then
                Return Text
            Else
                Return Text.Substring(0, Number)
            End If
        End If
        Return ""
    End Function

    <Extension()>
    Public Function GetLastChars(Text As String, Optional Number As Integer = 1) As String
        If Text.IsNotBlank Then
            If Text.Length < Number Or Number < 1 Then
                Return Text
            Else
                Return Text.Substring(Text.Length - Number)
            End If
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Retorna N caracteres de uma string a partir do caractere encontrado no centro
    ''' </summary>
    ''' <param name="Text">  </param>
    ''' <param name="Length"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetMiddleChars(ByVal Text As String, Length As Integer) As String
        Text = Text.IfBlank("")
        If Text.Length >= Length Then
            If Text.Length Mod 2 <> 0 Then
                Try
                    Return Text.Substring(Text.Length / 2 - 1, Length)
                Catch ex As Exception
                    Return Text.GetMiddleChars(Length - 1)
                End Try
            Else
                Return Text.RemoveLastChars(1).GetMiddleChars(Length)
            End If
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Retorna o caractere de encapsulamento oposto ao caractere indicado
    ''' </summary>
    ''' <param name="Text">Caractere</param>
    ''' <returns></returns>
    <Extension> Function GetOppositeWrapChar(Text As String) As String
        Select Case Text.GetFirstChars()
            Case """"
                Return """"
            Case "'"
                Return "'"
            Case "("
                Return ")"
            Case ")"
                Return "("
            Case "["
                Return "]"
            Case "]"
                Return "["
            Case "{"
                Return "}"
            Case "}"
                Return "{"
            Case "<"
                Return ">"
            Case ">"
                Return "<"
            Case "\"
                Return "/"
            Case "/"
                Return "\"
            Case "¿"
                Return "?"
            Case "?"
                Return "¿"
            Case "!"
                Return "¡"
            Case "¡"
                Return "!"
            Case "."
                Return "."
            Case ":"
                Return ":"
            Case ";"
                Return ";"
            Case "_"
                Return "_"
            Case "*"
                Return "*"
            Case Else
                Return Text
        End Select
    End Function

    ''' <summary>
    ''' Retorna o caractere de encapsulamento oposto ao caractere indicado
    ''' </summary>
    ''' <param name="Text">Caractere</param>
    ''' <returns></returns>
    <Extension> Function IsOpenWrapChar(Text As String) As String
        Return Text.GetFirstChars().IsIn(OpenWrappers)
    End Function

    <Extension> Function IsCloseWrapChar(Text As String) As String
        Return Text.GetFirstChars().IsIn(CloseWrappers)
    End Function

    ''' <summary>
    ''' Sorteia um item da Lista
    ''' </summary>
    ''' <typeparam name="Type">Tipo de lista</typeparam>
    ''' <param name="List">Lista</param>
    ''' <returns>Um valor do tipo especificado</returns>
    <Extension>
    Public Function GetRandomItem(Of Type)(ByVal List As IEnumerable(Of Type)) As Type
        Return List.ToArray()(RandomNumber(0, List.Count - 1))
    End Function

    ''' <summary>
    ''' Sorteia um item da Lista
    ''' </summary>
    ''' <typeparam name="Type">Tipo da Matriz</typeparam>
    ''' <param name="Array">Matriz</param>
    ''' <returns>Um valor do tipo especificado</returns>
    <Extension>
    Public Function GetRandomItem(Of Type)(ByVal Array As Type()) As Type
        Return Array(RandomNumber(0, Array.Count - 1))
    End Function

    ''' <summary>
    ''' Retorna o caminho relativo da url
    ''' </summary>
    ''' <param name="URL">Url</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetRelativeURL(URL As Uri) As String
        Return URL.PathAndQuery
    End Function

    ''' <summary>
    ''' Retorna o caminho relativo da url
    ''' </summary>
    ''' <param name="URL">Url</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetRelativeURL(URL As String) As String
        If URL.IsURL Then Return New Uri(URL).GetRelativeURL()
        Return Nothing
    End Function

    ''' <summary>
    ''' Retorna uma lista de palavras encontradas no texto em ordem alfabetica
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Function GetWords(Text As String) As IOrderedEnumerable(Of String)
        Dim txt As New List(Of String)
        Dim palavras As List(Of String) = Text.AdjustWhiteSpaces.FixBreakLines.ToLower.RemoveHTML.Split(WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList
        For Each w In palavras
            txt.Add(w)
        Next
        Return txt.Distinct.OrderBy(Function(x) x)
    End Function

    ''' <summary>
    ''' Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em um texto
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension> Public Function GetWrappedText(Text As String, Optional Character As String = """", Optional ExcludeWrapChars As Boolean = True) As String()
        Dim lista As New List(Of String)
        Dim regx = Character.RegexEscape & "(.*?)" & Character.ToString.GetOppositeWrapChar.RegexEscape
        Dim mm = New Regex(regx, RegexOptions.Singleline + RegexOptions.IgnoreCase).Matches(Text)
        For Each a As Match In mm
            If ExcludeWrapChars Then
                lista.Add(a.Value.RemoveFirstEqual(Character).RemoveLastEqual(GetOppositeWrapChar(Character)))
            Else
                lista.Add(a.Value)
            End If
        Next
        Return lista.ToArray
    End Function

    ''' <summary>
    ''' Retorna um texto com entidades HTML convertidas para caracteres e tags BR em breaklines
    ''' </summary>
    ''' <param name="Text">string HTML</param>
    ''' <returns>String HTML corrigido</returns>
    <Extension()>
    Public Function HtmlDecode(ByVal Text As String) As String
        Return System.Net.WebUtility.HtmlDecode("" & Text).ReplaceMany(vbCr & vbLf, "<br/>", "<br />", "<br>")
    End Function

    ''' <summary>
    ''' Escapa o texto HTML
    ''' </summary>
    ''' <param name="Text">string HTML</param>
    ''' <returns>String HTML corrigido</returns>
    <Extension()>
    Public Function HtmlEncode(ByVal Text As String) As String
        Return System.Net.WebUtility.HtmlEncode("" & Text.ReplaceMany("<br>", BreakLineChars))
    End Function

    ''' <summary>
    ''' Verifica se uma palavra é um Anagrama de outra palavra
    ''' </summary>
    ''' <param name="Text">       </param>
    ''' <param name="AnotherText"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsAnagramOf(Text As String, AnotherText As String) As Boolean
        Dim char1 As Char() = Text.ToLower().ToCharArray()
        Dim char2 As Char() = Text.ToLower().ToCharArray()
        Array.Sort(char1)
        Array.Sort(char2)
        Dim NewWord1 As New String(char1)
        Dim NewWord2 As New String(char2)
        Return NewWord1 = NewWord2
    End Function

    ''' <summary>
    ''' Compara se uma string é igual a outras strings
    ''' </summary>
    ''' <param name="Text"> string principal</param>
    ''' <param name="Texts">strings para comparar</param>
    ''' <returns>TRUE se alguma das strings for igual a principal</returns>
    <Extension()>
    Public Function IsAny(Text As String, ParamArray Texts As String()) As Boolean
        Return Text.IsAny(StringComparison.CurrentCultureIgnoreCase, Texts)
    End Function

    ''' <summary>
    ''' Compara se uma string é igual a outras strings
    ''' </summary>
    ''' <param name="Text"> string principal</param>
    ''' <param name="Texts">strings para comparar</param>
    ''' <returns>TRUE se alguma das strings for igual a principal</returns>
    <Extension()>
    Public Function IsAny(Text As String, Comparison As StringComparison, ParamArray Texts As String()) As Boolean
        Return If(Texts, {}).Any(Function(x) Text.Equals(x, Comparison))
    End Function

    ''' <summary>
    ''' Compara se uma string nao é igual a outras strings
    ''' </summary>
    ''' <param name="Text"> string principal</param>
    ''' <param name="Texts">strings para comparar</param>
    ''' <returns>TRUE se nenhuma das strings for igual a principal</returns>
    <Extension()>
    Public Function IsNotAny(Text As String, ParamArray Texts As String()) As Boolean
        Return Not Text.IsAny(Texts)
    End Function

    ''' <summary>
    ''' Compara se uma string nao é igual a outras strings
    ''' </summary>
    ''' <param name="Text"> string principal</param>
    ''' <param name="Texts">strings para comparar</param>
    ''' <returns>TRUE se alguma das strings for igual a principal</returns>
    <Extension()>
    Public Function IsNotAny(Text As String, Comparison As StringComparison, ParamArray Texts As String()) As Boolean
        Return Not Text.IsAny(Comparison, Texts)
    End Function

    ''' <summary>
    ''' Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da esqueda
    ''' para direita
    ''' </summary>
    ''' <param name="Text">             Texto</param>
    ''' <param name="IgnoreWhiteSpaces">Ignora os espaços na hora de comparar</param>
    ''' <returns></returns>
    <Extension()> Public Function IsPalindrome(ByVal Text As String, Optional IgnoreWhiteSpaces As Boolean = False) As Boolean
        If IgnoreWhiteSpaces Then Text = Text.RemoveAny(" ")
        Dim c = Text.ToArray()
        Dim p = c
        Array.Reverse(p)
        Return p = c
    End Function

    ''' <summary>
    ''' Une todos os valores de um objeto em uma unica string
    ''' </summary>
    ''' <param name="Array">    Objeto com os valores</param>
    ''' <param name="Separator">Separador entre as strings</param>
    ''' <returns>string</returns>
    <Extension()>
    Public Function Join(Of Type)(Array As IEnumerable(Of Type), Optional Separator As String = "") As String
        Return String.Join(Separator, Array)
    End Function

    ''' <summary>
    ''' Une todos os valores de um objeto em uma unica string
    ''' </summary>
    ''' <param name="Array">    Objeto com os valores</param>
    ''' <param name="Separator">Separador entre as strings</param>
    ''' <returns>string</returns>
    <Extension()>
    Public Function Join(Of Type)(Array As Type(), Optional Separator As String = "") As String
        Return String.Join(Separator, Array)
    End Function

    ''' <summary>
    ''' Une todos os valores de um objeto em uma unica string
    ''' </summary>
    ''' <param name="Array">    Objeto com os valores</param>
    ''' <param name="Separator">Separador entre as strings</param>
    ''' <returns>string</returns>
    Public Function Join(Of Type)(Separator As String, ParamArray Array() As Type) As String
        Return String.Join(Separator, Array)
    End Function

    ''' <summary>
    ''' Une todos os valores de um objeto em uma unica string
    ''' </summary>
    ''' <param name="List">     Objeto com os valores</param>
    ''' <param name="Separator">Separador entre as strings</param>
    ''' <returns>string</returns>
    <Extension()>
    Public Function Join(Of Type)(List As List(Of Type), Optional Separator As String = "") As String
        Return List.ToArray.Join(Separator)
    End Function

    ''' <summary>
    ''' Verifica se um texto contém outro ou vice versa
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="OtherText"></param>
    ''' <returns></returns>
    <Extension> Public Function CrossContains(Text As String, OtherText As String, Optional StringComparison As StringComparison = StringComparison.InvariantCultureIgnoreCase) As Boolean
        Return Text.Contains(OtherText, StringComparison) OrElse OtherText.Contains(Text, StringComparison)
    End Function

    ''' <summary>
    ''' Verifica se um texto contém outro
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="OtherText"></param>
    ''' <returns></returns>
    <Extension> Public Function Contains(Text As String, OtherText As String, StringComparison As StringComparison) As Boolean
        Return Text.IndexOf(OtherText, StringComparison) > -1
    End Function

    '''<summary>
    ''' Computa a distancia de Levenshtein entre 2 strings.
    ''' </summary>
    <Extension()> Public Function LevenshteinDistance(Text1 As String, Text2 As String) As Integer

        Dim n As Integer = Text1.Length
        Dim m As Integer = Text2.Length
        Dim d As Integer(,) = New Integer(n + 1, m + 1) {}

        ' Step 1
        If (n = 0) Then
            Return m
        End If

        If (m = 0) Then
            Return n
        End If

        ' Step 2
        For i As Integer = 0 To n
            d(i, 0) = i
        Next

        For j As Integer = 0 To m
            d(0, j) = j
        Next

        ' Step 3
        For i = 1 To n
            'Step 4
            For j = 1 To m
                ' Step 5
                Dim cost As Integer = If(Text2(j - 1) = Text1(i - 1), 0, 1)
                'Step 6
                d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1), d(i - 1, j - 1) + cost)
            Next
        Next
        ' Step 7
        Return d(n, m)
    End Function

    ''' <summary>
    ''' limpa um texto deixando apenas os caracteres alfanumericos.
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension> Function ParseAlphaNumeric(Text As String) As String
        Dim l As New List(Of String)
        For Each item In Text.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            l.Add(Regex.Replace(item, "[^A-Za-z0-9]", ""))
        Next
        Return l.Join(" ")
    End Function

    ''' <summary>
    ''' Remove caracteres não numéricos de uma string
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()> Function ParseDigits(ByVal Text As String, Optional Culture As CultureInfo = Nothing) As String
        Culture = If(Culture, CultureInfo.CurrentCulture)
        Dim strDigits As String = ""
        If Text = Nothing Then Return strDigits
        For Each c As Char In Text.ToCharArray()
            If Char.IsDigit(c) OrElse c = Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator) Then
                strDigits &= c
            End If
        Next c
        Return strDigits
    End Function

    <Extension()> Function ParseDigits(Of Type As IConvertible)(ByVal Text As String, Optional Culture As CultureInfo = Nothing) As Type
        Return Text.ParseDigits(Culture).ChangeType(Of Type)
    End Function

    ''' <summary>
    ''' Retorna uma string em sua forma poop
    ''' </summary>
    ''' <param name="Words"></param>
    ''' <returns></returns>
    Public Function Poopfy(ParamArray Words As String()) As String
        Dim p As New List(Of String)
        For Each Text In Words
            Dim l As Decimal = Text.Length / 2
            l = l.Floor
            If Not Text.GetFirstChars(l).Last.ToString.ToLower.IsIn({"a", "e", "i", "o", "u"}) Then
                l = l.ChangeType(Of Integer) - 1
            End If
            p.Add(Text.GetFirstChars(l).Trim & Text.GetFirstChars(l).Reverse.ToList.Join.ToLower.Trim)
        Next
        Return p.Join(" ").Trim
    End Function

    ''' <summary>
    ''' Retorna uma string em sua forma poop
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function Poopfy(Text As String) As String
        Return Poopfy(Text.Split(" "))
    End Function

    ''' <summary>
    ''' Return a Idented XML string
    ''' </summary>
    ''' <param name="Document"></param>
    ''' <returns></returns>
    <Extension()> Public Function PreetyPrint(Document As XmlDocument) As String
        Dim Result As [String] = ""

        Dim mStream As New MemoryStream()
        Dim writer As New XmlTextWriter(mStream, Encoding.Unicode)
        Try
            writer.Formatting = Formatting.Indented

            ' Write the XML into a formatting XmlTextWriter
            Document.WriteContentTo(writer)
            writer.Flush()
            mStream.Flush()

            ' Have to rewind the MemoryStream in order to read its contents.
            mStream.Position = 0

            ' Read MemoryStream contents into a StreamReader.
            Dim sReader As New StreamReader(mStream)

            ' Extract the text from the StreamReader.
            Result = sReader.ReadToEnd()
        Catch generatedExceptionName As XmlException
        Finally
            mStream?.Close()
            writer?.Close()
            mStream?.Dispose()
            writer?.Dispose()
        End Try

        Return Result
    End Function

    ''' <summary>
    ''' Adiciona texto ao começo de uma string
    ''' </summary>
    ''' <param name="Text">       Texto</param>
    ''' <param name="PrependText">Texto adicional</param>
    <Extension()>
    Public Function Prepend(Text As String, PrependText As String) As String
        Text = If(PrependText, "") & If(Text, "")
        Return Text
    End Function

    ''' <summary>
    ''' Adiciona texto ao fim de uma string
    ''' </summary>
    ''' <param name="Text">       Texto</param>
    ''' <param name="AppendText">Texto adicional</param>
    <Extension()>
    Public Function Append(Text As String, AppendText As String) As String
        Text = If(Text, "") & If(AppendText, "")
        Return Text
    End Function

    ''' <summary>
    ''' Adiciona texto ao começo de uma string se um criterio for cumprido
    ''' </summary>
    ''' <param name="Text">       Texto</param>
    ''' <param name="PrependText">Texto adicional</param>
    ''' <param name="Test">       Teste</param>
    <Extension()> Public Function PrependIf(Text As String, PrependText As String, Test As Boolean) As String
        If Test Then
            Text = Text.Prepend(PrependText)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <returns></returns>
    ''' <example>texto = $"{2} pães"</example>
    <Extension()> Public Function QuantifyText(PluralText As FormattableString) As String
        If PluralText.IsNotBlank Then
            If PluralText.ArgumentCount > 0 Then
                Dim n As Decimal = 0
                Dim ArgIndex = PluralText.GetArguments.GetIndexOf(PluralText.GetArguments().FirstOrDefault(Function(x) IsNumber(x)))
                n = PluralText.GetArguments().IfBlankOrNoIndex(ArgIndex, n)
                If n = 1 OrElse n = -1 Then
                    Return PluralText.ToString().Singularize()
                End If
            End If
        End If
        Return PluralText?.ToString()
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="Quantity">  Quantidade de Itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(PluralText As String, Quantity As Object) As String
        Dim numero As Decimal
        Select Case True
            Case IsNumber(Quantity)
                numero = CType(Quantity, Decimal)
                Exit Select
            Case GetType(IList).IsAssignableFrom(Quantity.GetType)
                numero = CType(Quantity, IList).Count
                Exit Select
            Case GetType(IDictionary).IsAssignableFrom(Quantity.GetType)
                numero = CType(Quantity, IDictionary).Count
                Exit Select

            Case GetType(Array).IsAssignableFrom(Quantity.GetType)
                numero = CType(Quantity, Array).Length
                Exit Select
            Case Else
                numero = CType(Quantity, Decimal)
        End Select

        If numero.Floor = 1 OrElse numero.Floor = -1 Then
            Return PluralText.Singularize()
        End If
        Return PluralText
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="List">Lista com itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(Of T)(List As IEnumerable(Of T), PluralText As String) As String
        Return QuantifyText(PluralText, If(List, {}))
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="Quantity">Quantidade de Itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(Quantity As Integer, PluralText As String) As String
        Return QuantifyText(PluralText, Quantity)
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="Quantity">Quantidade de Itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(Quantity As Decimal, PluralText As String) As String
        Return QuantifyText(PluralText, Quantity)
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="Quantity">Quantidade de Itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(Quantity As Short, PluralText As String) As String
        Return QuantifyText(PluralText, Quantity)
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="Quantity">Quantidade de Itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(Quantity As Long, PluralText As String) As String
        Return QuantifyText(PluralText, Quantity)
    End Function

    ''' <summary>
    ''' Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
    ''' </summary>
    ''' <param name="PluralText">Texto no plural</param>
    ''' <param name="Quantity">Quantidade de Itens</param>
    ''' <returns></returns>
    <Extension()> Public Function QuantifyText(Quantity As Double, PluralText As String) As String
        Return QuantifyText(PluralText, Quantity)
    End Function

    ''' <summary>
    ''' Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes)
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="OpenQuoteChar">Caractere de encapsulamento</param>
    ''' <returns></returns>
    <Extension()>
    Function Quote(Text As String, Optional OpenQuoteChar As Char = """"c) As String
        If OpenQuoteChar.ToString().IsCloseWrapChar Then
            OpenQuoteChar = OpenQuoteChar.ToString.GetOppositeWrapChar
        End If
        Return OpenQuoteChar & Text & OpenQuoteChar.ToString.GetOppositeWrapChar
    End Function

    <Extension()> Function UnQuote(Text As String) As String
        Return Text.UnQuote("", True)
    End Function

    <Extension()> Function UnQuote(Text As String, OpenQuoteChar As String, Optional ContinuouslyRemove As Boolean = False) As String
        If OpenQuoteChar.IsBlank() Then
            While Text.EndsWithAny(CloseWrappers.ToArray()) OrElse Text.StartsWithAny(OpenWrappers.ToArray)
                Text = Text.TrimAny(ContinuouslyRemove, WordWrappers.ToArray())
            End While
        Else
            If OpenQuoteChar.ToString().IsCloseWrapChar Then
                OpenQuoteChar = OpenQuoteChar.ToString.GetOppositeWrapChar
            End If
            Text = Text.TrimAny(ContinuouslyRemove, OpenQuoteChar, OpenQuoteChar.ToString.GetOppositeWrapChar)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) é um alias de <see cref="Quote(String, Char)"/>
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="BracketChar">Caractere de encapsulamento</param>
    ''' <returns></returns>
    <Extension()>
    Function Brackfy(Text As String, Optional BracketChar As Char = "{"c) As String
        Return Text.Quote(BracketChar)
    End Function

    <Extension()> Function UnBrackfy(Text As String) As String
        Return Text.UnBrackfy("", True)
    End Function

    <Extension()> Function UnBrackfy(Text As String, BracketChar As String, Optional ContinuouslyRemove As Boolean = False) As String
        Return Text.UnQuote(BracketChar, ContinuouslyRemove)
    End Function

    ''' <summary>
    ''' Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes) se uma
    ''' condiçao for cumprida
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="QuoteChar">Caractere de encapsulamento</param>
    ''' <returns></returns>
    <Extension()>
    Function QuoteIf(Text As String, Condition As Boolean, Optional QuoteChar As String = """") As String
        Return If(Condition, Text.Quote(QuoteChar), Text)
    End Function

    ''' <summary>
    ''' Sorteia um item da Matriz
    ''' </summary>
    ''' <typeparam name="Type">Tipo da Matriz</typeparam>
    ''' <param name="Array">Matriz</param>
    ''' <returns>Um valor do tipo especificado</returns>
    Public Function RandomItem(Of Type)(ParamArray Array As Type()) As Type
        Return Array.GetRandomItem
    End Function

    ''' <summary>
    ''' Escapa caracteres exclusivos de uma regex
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function RegexEscape(Text As String) As String
        Dim chars = {"."c, "$"c, "^"c, "{"c, "["c, "("c, "|"c, ")"c, "*"c, "+"c, "?"c, "|"c}
        Dim newstring As String = ""
        For Each c In Text.ToArray
            If c.IsIn(chars) Then
                newstring &= ("\" & c)
            Else
                newstring &= (c)
            End If
        Next
        Return newstring
    End Function

    ''' <summary>
    ''' Remove os acentos de uma string
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>String sem os acentos</returns>
    <Extension>
    Public Function RemoveAccents(ByVal Text As String) As String
        Dim s As String = Text.Normalize(NormalizationForm.FormD)
        Dim sb As New StringBuilder()
        Dim k As Integer = 0
        While k < s.Length
            Dim uc As UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(s(k))
            If uc <> UnicodeCategory.NonSpacingMark Then
                sb.Append(s(k))
            End If
            k = k + 1
        End While
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Remove várias strings de uma string
    ''' </summary>
    ''' <param name="Text">  Texto</param>
    ''' <param name="Values">Strings a serem removidas</param>
    ''' <returns>Uma string com os valores removidos</returns>
    <Extension>
    Public Function RemoveAny(ByVal Text As String, ParamArray Values() As String) As String
        Text = Text.ReplaceMany("", If(Values, {}))
        Return Text
    End Function

    ''' <summary>
    ''' Remove os acentos de uma string
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>String sem os acentos</returns>
    <Extension()>
    Public Function RemoveDiacritics(ByRef Text As String) As String
        Text = Text.RemoveAccents
        Return Text
    End Function

    ''' <summary>
    ''' Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
    ''' </summary>
    ''' <param name="Text">              Texto</param>
    ''' <param name="ContinuouslyRemove">
    ''' Parametro que indica se a string deve continuar sendo testada até que todas as ocorrencias
    ''' sejam removidas
    ''' </param>
    ''' <param name="StartStringTest">   Conjunto de textos que serão comparados</param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveFirstAny(ByVal Text As String, ContinuouslyRemove As Boolean, ParamArray StartStringTest As String()) As String
        Dim re = Text
        While re.StartsWithAny(StartStringTest)
            For Each item In StartStringTest
                If re.StartsWith(item) Then
                    re = re.RemoveFirstEqual(item)
                    If Not ContinuouslyRemove Then Return re
                End If
            Next
        End While
        Return re
    End Function

    ''' <summary>
    ''' Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
    ''' </summary>
    ''' <param name="Text">              Texto</param>
    ''' <param name="StartStringTest">     Conjunto de textos que serão comparados</param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveFirstAny(ByVal Text As String, ParamArray StartStringTest As String()) As String
        Return Text.RemoveFirstAny(True, StartStringTest)
    End Function

    ''' <summary>
    ''' Remove os X primeiros caracteres
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="Quantity"> Quantidade de Caracteres</param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveFirstChars(Text As String, Optional Quantity As Integer = 1) As String
        If Text.Length > Quantity Then
            Return Text.Remove(0, Quantity)
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' Remove um texto do inicio de uma string se ele for um outro texto especificado
    ''' </summary>
    ''' <param name="Text">           Texto</param>
    ''' <param name="StartStringTest">Texto inicial que será comparado</param>
    <Extension>
    Function RemoveFirstEqual(ByVal Text As String, StartStringTest As String) As String
        If Text.StartsWith(StartStringTest) Then
            Text = Text.RemoveFirstChars(StartStringTest.Length)
        End If
        Return Text
    End Function

    <Extension()>
    Public Function RemoveHTML(Text As String) As String
        If Text.IsNotBlank Then
            Return Regex.Replace(Text, "<.*?>", String.Empty).HtmlDecode
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
    ''' </summary>
    ''' <param name="Text">              Texto</param>
    ''' <param name="ContinuouslyRemove">
    ''' Parametro que indica se a string deve continuar sendo testada até que todas as ocorrencias
    ''' sejam removidas
    ''' </param>
    ''' <param name="EndStringTest">     Conjunto de textos que serão comparados</param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveLastAny(ByVal Text As String, ContinuouslyRemove As Boolean, ParamArray EndStringTest As String()) As String
        Dim re = Text
        While re.EndsWithAny(EndStringTest)
            For Each item In EndStringTest
                If re.EndsWith(item) Then
                    re = re.RemoveLastEqual(item)
                    If Not ContinuouslyRemove Then Return re
                End If
            Next
        End While
        Return re
    End Function

    ''' <summary>
    ''' Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
    ''' </summary>
    ''' <param name="Text">              Texto</param>
    ''' <param name="EndStringTest">     Conjunto de textos que serão comparados</param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveLastAny(ByVal Text As String, ParamArray EndStringTest As String()) As String
        Return Text.RemoveLastAny(True, EndStringTest)
    End Function

    ''' <summary>
    ''' Remove os X ultimos caracteres
    ''' </summary>
    ''' <param name="Text">    Texto</param>
    ''' <param name="Quantity">Quantidade de Caracteres</param>
    ''' <returns></returns>
    <Extension()>
    Public Function RemoveLastChars(Text As String, Optional Quantity As Integer = 1) As String
        Return Text.Substring(0, Text.Length - Quantity)
    End Function

    ''' <summary>
    ''' Remove um texto do final de uma string se ele for um outro texto
    ''' </summary>
    ''' <param name="Text">         Texto</param>
    ''' <param name="EndStringTest">Texto final que será comparado</param>
    <Extension>
    Function RemoveLastEqual(ByVal Text As String, EndStringTest As String) As String
        If Text.EndsWith(EndStringTest) Then
            Text = Text.RemoveLastChars(EndStringTest.Length)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Remove caracteres não printaveis de uma string
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>String corrigida</returns>
    <Extension()>
    Public Function RemoveNonPrintable(Text As String) As String
        For Each c As Char In Text.ToCharArray()
            If Char.IsControl(c) Then
                Text = Text.ReplaceNone(c)
            End If
        Next
        Return Text.Trim()
    End Function

    ''' <summary>
    ''' Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum
    ''' </summary>
    ''' <param name="Strings">        Array de strings</param>
    ''' <param name="OldValue">       Valor antigo que será substituido</param>
    ''' <param name="NewValue">       Valor utilizado para substituir o valor antigo</param>
    ''' <param name="ReplaceIfEquals">
    ''' Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE realiza
    ''' um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
    ''' </param>
    ''' <returns></returns>
    <Extension()>
    Public Function Replace(Strings As String(), OldValue As String, NewValue As String, Optional ReplaceIfEquals As Boolean = True) As String()
        Dim NewArray As String() = Strings
        For index = 0 To Strings.Length - 1
            If ReplaceIfEquals Then
                If NewArray(index) = OldValue Then
                    NewArray(index) = NewValue
                End If
            Else
                NewArray(index) = NewArray(index).Replace(OldValue, NewValue)
            End If
        Next
        Return NewArray
    End Function

    ''' <summary>
    ''' Faz uma busca em todos os elementos de uma lista e aplica um ReplaceFrom comum
    ''' </summary>
    ''' <param name="Strings">        Array de strings</param>
    ''' <param name="OldValue">       Valor antigo que será substituido</param>
    ''' <param name="NewValue">       Valor utilizado para substituir o valor antigo</param>
    ''' <param name="ReplaceIfEquals">
    ''' Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE realiza
    ''' um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
    ''' </param>
    ''' <returns></returns>
    <Extension()>
    Public Function Replace(Strings As List(Of String), OldValue As String, NewValue As String, Optional ReplaceIfEquals As Boolean = True) As List(Of String)
        Return Strings.ToArray.Replace(OldValue, NewValue, ReplaceIfEquals).ToList()
    End Function

    ''' <summary>
    ''' Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
    ''' </summary>
    <Extension> Public Function ReplaceFrom(ByVal Text As String, Dic As IDictionary(Of String, String)) As String
        If Dic IsNot Nothing AndAlso Text.IsNotBlank Then
            For Each p In Dic
                Text = Text.Replace(p.Key, p.Value)
            Next
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
    ''' </summary>
    <Extension> Public Function ReplaceFrom(Of T)(ByVal Text As String, Dic As IDictionary(Of String, T)) As String
        If Dic IsNot Nothing AndAlso Text.IsNotBlank Then
            For Each p In Dic
                Select Case True
                    Case IsDictionary(p.Value)
                        Text = Text.ReplaceFrom(CType(p.Value, IDictionary(Of String, Object)))
                    Case GetType(T).IsAssignableFrom(GetType(Array))
                        For Each item In ForceArray(p.Value)
                            Text = Text.ReplaceMany(p.Key, ForceArray(p.Value).Cast(Of String).ToArray())
                        Next
                    Case Else
                        Text = Text.Replace(p.Key, p.Value.ToString())
                End Select

            Next
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
    ''' </summary>
    <Extension> Public Function ReplaceFrom(ByVal Text As String, Dic As IDictionary(Of String, String()), Optional Comparison As StringComparison = StringComparison.InvariantCultureIgnoreCase) As String
        If Dic IsNot Nothing AndAlso Text.IsNotBlank Then
            For Each p In Dic
                Text = Text.SensitiveReplace(p.Key, p.Value)
            Next
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
    ''' </summary>
    <Extension> Public Function ReplaceFrom(ByVal Text As String, Dic As IDictionary(Of String(), String), Optional Comparison As StringComparison = StringComparison.InvariantCultureIgnoreCase) As String
        If Dic IsNot Nothing AndAlso Text.IsNotBlank Then
            For Each p In Dic
                Text = Text.SensitiveReplace(p.Value, p.Key.ToArray, Comparison)
            Next
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
    ''' </summary>
    <Extension> Public Function ReplaceFrom(ByVal Text As String, Dic As IDictionary(Of String(), String()), Optional Comparison As StringComparison = StringComparison.InvariantCultureIgnoreCase) As String
        If Dic IsNot Nothing AndAlso Text.IsNotBlank Then
            For Each p In Dic
                Dim froms = p.Key.ToList
                Dim tos = p.Value.ToList
                While froms.Count > tos.Count
                    tos.Add(String.Empty)
                End While
                For i = 0 To froms.Count - 1
                    Text = Text.SensitiveReplace(froms(i), tos(i), Comparison)
                Next
            Next
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
    ''' substituídas por um novo valor.
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="NewValue"> Novo Valor</param>
    ''' <param name="OldValues">Valores a serem substituido por um novo valor</param>
    ''' <returns></returns>
    <Extension>
    Public Function ReplaceMany(ByVal Text As String, NewValue As String, ParamArray OldValues As String()) As String
        Text = If(Text, "")
        For Each word In If(OldValues, {}).Where(Function(x) x.Length > 0)
            Text = Text.Replace(word, NewValue)
        Next
        Return Text
    End Function

    ''' <summary>
    ''' Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
    ''' substituídas por vazio.
    ''' </summary>
    ''' <param name="Text">    Texto</param>
    ''' <param name="OldValue">Valor a ser substituido por vazio</param>
    ''' <returns>String corrigida</returns>
    <Extension>
    Public Function ReplaceNone(Text As String, OldValue As String) As String
        Return Text.Replace(OldValue, "")
    End Function

    ''' <summary>
    ''' Realiza um replace em uma string usando um tipo especifico de comparacao
    ''' </summary>
    ''' <param name="Text">          </param>
    ''' <param name="NewValue">      </param>
    ''' <param name="OldValue">      </param>
    ''' <param name="ComparisonType"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function SensitiveReplace(ByVal Text As String, ByVal OldValue As String, ByVal NewValue As String, Optional ComparisonType As StringComparison = StringComparison.InvariantCulture) As String
        Return SensitiveReplace(Text, NewValue, {OldValue}, ComparisonType)
    End Function

    ''' <summary>
    ''' Realiza um replace em uma string usando um tipo especifico de comparacao
    ''' </summary>
    ''' <param name="Text">          </param>
    ''' <param name="NewValue">      </param>
    ''' <param name="OldValues">     </param>
    ''' <param name="ComparisonType"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function SensitiveReplace(ByVal Text As String, ByVal NewValue As String, ByVal OldValues As IEnumerable(Of String), Optional ComparisonType As StringComparison = StringComparison.InvariantCulture) As String
        If Text.IsNotBlank Then
            For Each oldvalue In If(OldValues, {""})
                NewValue = If(NewValue, String.Empty)
                If Not oldvalue.Equals(NewValue, ComparisonType) Then
                    Dim foundAt As Integer
                    Do
                        foundAt = Text.IndexOf(oldvalue, 0, ComparisonType)
                        If foundAt > -1 Then
                            Text = Text.Remove(foundAt, oldvalue.Length).Insert(foundAt, NewValue)
                        End If
                    Loop While foundAt <> -1
                End If
            Next
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Randomiza a ordem dos itens de um Array
    ''' </summary>
    ''' <typeparam name="Type">Tipo do Array</typeparam>
    ''' <param name="Array">Matriz</param>
    <Extension()> Public Function Shuffle(Of Type)(ByVal Array As Type()) As Type()
        Return Array.OrderByRandom().ToArray
    End Function

    ''' <summary>
    ''' Randomiza a ordem dos itens de uma Lista
    ''' </summary>
    ''' <typeparam name="Type">Tipo de Lista</typeparam>
    ''' <param name="List">Matriz</param>
    <Extension()> Public Function Shuffle(Of Type)(ByRef List As List(Of Type)) As List(Of Type)
        Return List.OrderByRandom.ToList()
    End Function

    ''' <summary>
    ''' Aleatoriza a ordem das letras de um texto
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()> Public Function Shuffle(ByRef Text As String) As String
        Return RandomWord(Text)
    End Function

    ''' <summary>
    ''' Retorna a frase ou termo especificado em sua forma singular
    ''' </summary>
    ''' <param name="Text">Texto no plural</param>
    ''' <returns></returns>
    <Extension()> Public Function Singularize(Text As String) As String
        Dim phrase As String() = Text.ApplySpaceOnWrapChars.Split(" ")
        For index = 0 To phrase.Count - 1

            Dim endchar As String = phrase(index).GetLastChars
            If endchar.IsAny(WordSplitters.ToArray()) Then
                phrase(index) = phrase(index).RemoveLastEqual(endchar)
            End If

            Select Case True
                Case phrase(index).IsNumber OrElse phrase(index).IsEmail OrElse phrase(index).IsURL OrElse phrase(index).IsIP OrElse phrase(index).IsIn(WordSplitters)
                    'nao alterar estes tipos
                    Exit Select
                Case phrase(index).EndsWith("ões")
                    phrase(index) = phrase(index).RemoveLastEqual("ões") & ("ão")
                    Exit Select
                Case phrase(index).EndsWith("ãos")
                    phrase(index) = phrase(index).RemoveLastEqual("ãos") & ("ão")
                    Exit Select
                Case phrase(index).EndsWith("ães")
                    phrase(index) = phrase(index).RemoveLastEqual("ães") & ("ão")
                    Exit Select
                Case phrase(index).EndsWith("ais")
                    phrase(index) = phrase(index).RemoveLastEqual("ais") & ("al")
                    Exit Select
                Case phrase(index).EndsWith("eis")
                    phrase(index) = phrase(index).RemoveLastEqual("eis") & ("el")
                    Exit Select
                Case phrase(index).EndsWith("ois")
                    phrase(index) = phrase(index).RemoveLastEqual("ois") & ("ol")
                    Exit Select
                Case phrase(index).EndsWith("uis")
                    phrase(index) = phrase(index).RemoveLastEqual("uis") & ("ul")
                    Exit Select
                Case phrase(index).EndsWith("es")
                    If phrase(index).RemoveLastEqual("es").EndsWithAny("z", "r") Then
                        phrase(index) = phrase(index).RemoveLastEqual("es")
                    Else
                        phrase(index) = phrase(index).RemoveLastEqual("s")
                    End If
                    Exit Select
                Case phrase(index).EndsWith("ns")
                    phrase(index) = phrase(index).RemoveLastEqual("ns") & ("m")
                    Exit Select
                Case phrase(index).EndsWith("s")
                    phrase(index) = phrase(index).RemoveLastEqual("s")
                    Exit Select
                Case Else
                    'ja esta no singular
            End Select
            If endchar.IsAny(WordSplitters.ToArray()) Then
                phrase(index) = phrase(index) & (endchar)
            End If
        Next
        Return phrase.Join(" ").AdjustWhiteSpaces
    End Function

    ''' <summary>
    ''' Corta un texto para exibir um numero máximo de caracteres ou na primeira quebra de linha.
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="TextLength"></param>
    ''' <param name="Ellipsis"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function Slice(Text As String, Optional TextLength As Integer = 0, Optional Ellipsis As String = "...") As String
        If Text.IsBlank() OrElse Text.Length <= TextLength OrElse TextLength < 1 Then
            Return Text
        Else
            Text = Text.GetBefore(Environment.NewLine)
            Return Text.GetFirstChars(TextLength) & Ellipsis
        End If
    End Function

    ''' <summary>
    ''' Separa um texto em um array de strings a partir de uma outra string
    ''' </summary>
    ''' <param name="Text">     Texto</param>
    ''' <param name="Separator">Texto utilizado como separador</param>
    ''' <returns></returns>
    <Extension>
    Public Function Split(Text As String, Separator As String, Optional Options As StringSplitOptions = StringSplitOptions.RemoveEmptyEntries) As String()
        Return If(Text, "").Split({Separator}, Options)
    End Function

    ''' <summary>
    ''' Verifica se uma string começa com alguma outra string de um array
    ''' </summary>
    ''' <param name="Text"> </param>
    ''' <param name="Words"></param>
    ''' <returns></returns>
    <Extension()> Function StartsWithAny(Text As String, ParamArray Words As String()) As Boolean
        Return Words.Any(Function(p) Text.StartsWith(p))
    End Function

    ''' <summary>
    ''' Conta as silabas de uma palavra
    ''' </summary>
    ''' <param name="Word"></param>
    ''' <returns></returns>
    <Extension()> Function SyllableCount(ByVal Word As String) As Integer
        Word = Word.ToLower().Trim()
        Dim lastWasVowel As Boolean
        Dim vowels = {"a"c, "e"c, "i"c, "o"c, "u"c, "y"c}.ToList()
        Dim count As Integer
        For Each c In Word
            If vowels.Contains(c) Then
                If Not lastWasVowel Then count += 1
                lastWasVowel = True
            Else
                lastWasVowel = False
            End If
        Next
        If (Word.EndsWith("e") OrElse (Word.EndsWith("es") OrElse Word.EndsWith("ed"))) AndAlso Not Word.EndsWith("le") Then count -= 1
        Return count
    End Function

    ''' <summary>
    ''' Alterna maiusculas e minusculas para cada letra de uma string
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension> Public Function ToAlternateCase(Text As String) As String
        Dim ch = Text.ToArray
        For index = 0 To ch.Length - 1
            Dim antec = ch.IfNoIndex(index - 1, "")

            If antec.ToString.IsBlank OrElse Char.IsLower(antec) OrElse antec = vbNullChar Then
                ch(index) = Char.ToUpper(ch(index))
            Else
                ch(index) = Char.ToLower(ch(index))
            End If
        Next
        Return New String(ch)
    End Function

    ''' <summary>
    ''' Retorna um anagrama de um texto
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()> Public Function ToAnagram(ByVal Text As String) As String
        Return Text.Shuffle().AdjustWhiteSpaces
    End Function

    ''' <summary>
    ''' Transforma uma frase em uma palavra CamelCase
    ''' </summary>
    ''' <param name="Text">Texto a ser manipulado</param>
    ''' <returns>Uma String com o texto am CameCase</returns>
    <Extension()>
    Public Function ToCamel(Text As String) As String
        Return ToProperCase(Text).Split(" ", StringSplitOptions.RemoveEmptyEntries).Join("")
    End Function

    ''' <summary>
    ''' Retorna o uma string representando um valor em bytes, KB, MB ou TB
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    ''' <returns>String com o tamanho + unidade de medida</returns>
    <Extension()>
    Public Function ToFileSizeString(ByVal Size As Byte(), Optional DecimalPlaces As Integer = -1) As String
        Return Size.LongLength.ToFileSizeString(DecimalPlaces)
    End Function

    ''' <summary>
    ''' Retorna o uma string representando um valor em bytes, KB, MB ou TB
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    ''' <returns>String com o tamanho + unidade de medida</returns>
    <Extension()>
    Public Function ToFileSizeString(ByVal Size As FileInfo, Optional DecimalPlaces As Integer = -1) As String
        Return Size.Length.ToFileSizeString(DecimalPlaces)
    End Function

    ''' <summary>
    ''' Retorna o uma string representando um valor em bytes, KB, MB ou TB
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    ''' <returns>String com o tamanho + unidade de medida</returns>
    <Extension()>
    Public Function ToFileSizeString(ByVal Size As Double, Optional DecimalPlaces As Integer = -1) As String
        Return Size.ChangeType(Of Decimal).ToFileSizeString(DecimalPlaces)
    End Function

    ''' <summary>
    ''' Retorna o uma string representando um valor em bytes, KB, MB ou TB
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    ''' <returns>String com o tamanho + unidade de medida</returns>
    <Extension()>
    Public Function ToFileSizeString(ByVal Size As Integer, Optional DecimalPlaces As Integer = -1) As String
        Return Size.ChangeType(Of Decimal).ToFileSizeString(DecimalPlaces)
    End Function

    ''' <summary>
    ''' Retorna o uma string representando um valor em bytes, KB, MB ou TB
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    ''' <returns>String com o tamanho + unidade de medida</returns>
    <Extension()>
    Public Function ToFileSizeString(ByVal Size As Long, Optional DecimalPlaces As Integer = -1) As String
        Return Size.ChangeType(Of Decimal).ToFileSizeString(DecimalPlaces)
    End Function

    ''' <summary>
    ''' Retorna o uma string representando um valor em bytes, KB, MB ou TB
    ''' </summary>
    ''' <param name="Size">Tamanho</param>
    ''' <returns>String com o tamanho + unidade de medida</returns>
    <Extension()>
    Public Function ToFileSizeString(ByVal Size As Decimal, Optional DecimalPlaces As Integer = -1) As String
        Return UnitConverter.CreateFileSizeConverter.Abreviate(Size, DecimalPlaces)
    End Function

    ''' <summary>
    ''' Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca
    ''' espacos por hifen)
    ''' </summary>
    ''' <param name="Text">         </param>
    ''' <param name="UseUnderscore">
    ''' Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
    ''' </param>
    ''' <returns>string amigavel para URL</returns>
    <Extension()>
    Public Function ToFriendlyURL(Text As String, Optional UseUnderscore As Boolean = False) As String
        Return Text.ReplaceMany(If(UseUnderscore, "_", "-"), "_", "-", " ").RemoveAny("(", ")", ".", ",", "#").ToFriendlyPathName().RemoveAccents().ToLower()
    End Function

    ''' <summary>
    ''' Prepara uma string para se tornar uma caminho amigavel (remove caracteres nao permitidos)
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns>string amigavel para URL</returns>
    <Extension()>
    Public Function ToFriendlyPathName(Text As String) As String
        Return Text.Replace("&", "e").Replace("@", "a").RemoveAny(":", "*", "?", "/", "\", "<", ">", "{", "}", "[", "]", "|", """", "'", vbTab, Environment.NewLine).AdjustBlankSpaces()
    End Function

    ''' <summary>
    ''' Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function AdjustPathChars(Text As String, Optional InvertedBar As Boolean = False) As String
        Return Text.Split({"/", "\"}, StringSplitOptions.RemoveEmptyEntries).Select(Function(x, i)
                                                                                        If i = 0 AndAlso x.Length = 2 AndAlso x.EndsWith(":") Then Return x
                                                                                        Return x.ToFriendlyPathName
                                                                                    End Function).Join(InvertedBar.AsIf("\", "/"))
    End Function

    ''' <summary>
    ''' Converte um texo para Leet (1337)
    ''' </summary>
    ''' <param name="text">  TExto original</param>
    ''' <param name="degree">Grau de itensidade (0 - 100%)</param>
    ''' <returns>Texto em 1337</returns>
    <Extension>
    Public Function ToLeet(Text As String, Optional Degree As Integer = 30) As String
        ' Adjust degree between 0 - 100
        Degree = Degree.LimitRange(0, 100)
        ' No Leet Translator
        If Degree = 0 Then
            Return Text
        End If
        ' StringBuilder to store result.
        Dim sb As New StringBuilder(Text.Length)
        For Each c As Char In Text
            '#Region "Degree > 0 and < 17"
            If Degree < 17 AndAlso Degree > 0 Then
                Select Case c
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
                '#Region "Degree > 16 and < 33"
            ElseIf Degree < 33 AndAlso Degree > 16 Then
                Select Case c
                    Case "a"c
                        sb.Append("4")
                        Exit Select
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "i"c
                        sb.Append("1")
                        Exit Select
                    Case "o"c
                        sb.Append("0")
                        Exit Select
                    Case "A"c
                        sb.Append("4")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case "I"c
                        sb.Append("1")
                        Exit Select
                    Case "O"c
                        sb.Append("0")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
                '#Region "Degree > 32 and < 49"
            ElseIf Degree < 49 AndAlso Degree > 32 Then
                Select Case c
                    Case "a"c
                        sb.Append("4")
                        Exit Select
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "i"c
                        sb.Append("1")
                        Exit Select
                    Case "o"c
                        sb.Append("0")
                        Exit Select
                    Case "A"c
                        sb.Append("4")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case "I"c
                        sb.Append("1")
                        Exit Select
                    Case "O"c
                        sb.Append("0")
                        Exit Select
                    Case "s"c
                        sb.Append("$")
                        Exit Select
                    Case "S"c
                        sb.Append("$")
                        Exit Select
                    Case "l"c
                        sb.Append("£")
                        Exit Select
                    Case "L"c
                        sb.Append("£")
                        Exit Select
                    Case "c"c
                        sb.Append("(")
                        Exit Select
                    Case "C"c
                        sb.Append("(")
                        Exit Select
                    Case "y"c
                        sb.Append("¥")
                        Exit Select
                    Case "Y"c
                        sb.Append("¥")
                        Exit Select
                    Case "U"c
                        sb.Append("µ")
                        Exit Select
                    Case "u"c
                        sb.Append("µ")
                        Exit Select
                    Case "d"c
                        sb.Append("Ð")
                        Exit Select
                    Case "D"c
                        sb.Append("Ð")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
                '#Region "Degree > 48 and < 65"
            ElseIf Degree < 65 AndAlso Degree > 48 Then
                Select Case c
                    Case "a"c
                        sb.Append("4")
                        Exit Select
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "i"c
                        sb.Append("1")
                        Exit Select
                    Case "o"c
                        sb.Append("0")
                        Exit Select
                    Case "A"c
                        sb.Append("4")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case "I"c
                        sb.Append("1")
                        Exit Select
                    Case "O"c
                        sb.Append("0")
                        Exit Select
                    Case "k"c
                        sb.Append("|{")
                        Exit Select
                    Case "K"c
                        sb.Append("|{")
                        Exit Select
                    Case "s"c
                        sb.Append("$")
                        Exit Select
                    Case "S"c
                        sb.Append("$")
                        Exit Select
                    Case "g"c
                        sb.Append("9")
                        Exit Select
                    Case "G"c
                        sb.Append("9")
                        Exit Select
                    Case "l"c
                        sb.Append("£")
                        Exit Select
                    Case "L"c
                        sb.Append("£")
                        Exit Select
                    Case "c"c
                        sb.Append("(")
                        Exit Select
                    Case "C"c
                        sb.Append("(")
                        Exit Select
                    Case "t"c
                        sb.Append("7")
                        Exit Select
                    Case "T"c
                        sb.Append("7")
                        Exit Select
                    Case "z"c
                        sb.Append("2")
                        Exit Select
                    Case "Z"c
                        sb.Append("2")
                        Exit Select
                    Case "y"c
                        sb.Append("¥")
                        Exit Select
                    Case "Y"c
                        sb.Append("¥")
                        Exit Select
                    Case "U"c
                        sb.Append("µ")
                        Exit Select
                    Case "u"c
                        sb.Append("µ")
                        Exit Select
                    Case "f"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "F"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "d"c
                        sb.Append("Ð")
                        Exit Select
                    Case "D"c
                        sb.Append("Ð")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
                '#Region "Degree > 64 and < 81"
            ElseIf Degree < 81 AndAlso Degree > 64 Then
                Select Case c
                    Case "a"c
                        sb.Append("4")
                        Exit Select
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "i"c
                        sb.Append("1")
                        Exit Select
                    Case "o"c
                        sb.Append("0")
                        Exit Select
                    Case "A"c
                        sb.Append("4")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case "I"c
                        sb.Append("1")
                        Exit Select
                    Case "O"c
                        sb.Append("0")
                        Exit Select
                    Case "k"c
                        sb.Append("|{")
                        Exit Select
                    Case "K"c
                        sb.Append("|{")
                        Exit Select
                    Case "s"c
                        sb.Append("$")
                        Exit Select
                    Case "S"c
                        sb.Append("$")
                        Exit Select
                    Case "g"c
                        sb.Append("9")
                        Exit Select
                    Case "G"c
                        sb.Append("6")
                        Exit Select
                    Case "l"c
                        sb.Append("£")
                        Exit Select
                    Case "L"c
                        sb.Append("£")
                        Exit Select
                    Case "c"c
                        sb.Append("(")
                        Exit Select
                    Case "C"c
                        sb.Append("(")
                        Exit Select
                    Case "t"c
                        sb.Append("7")
                        Exit Select
                    Case "T"c
                        sb.Append("7")
                        Exit Select
                    Case "z"c
                        sb.Append("2")
                        Exit Select
                    Case "Z"c
                        sb.Append("2")
                        Exit Select
                    Case "y"c
                        sb.Append("¥")
                        Exit Select
                    Case "Y"c
                        sb.Append("¥")
                        Exit Select
                    Case "U"c
                        sb.Append("µ")
                        Exit Select
                    Case "u"c
                        sb.Append("µ")
                        Exit Select
                    Case "f"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "F"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "d"c
                        sb.Append("Ð")
                        Exit Select
                    Case "D"c
                        sb.Append("Ð")
                        Exit Select
                    Case "n"c
                        sb.Append("|\|")
                        Exit Select
                    Case "N"c
                        sb.Append("|\|")
                        Exit Select
                    Case "w"c
                        sb.Append("\/\/")
                        Exit Select
                    Case "W"c
                        sb.Append("\/\/")
                        Exit Select
                    Case "h"c
                        sb.Append("|-|")
                        Exit Select
                    Case "H"c
                        sb.Append("|-|")
                        Exit Select
                    Case "v"c
                        sb.Append("\/")
                        Exit Select
                    Case "V"c
                        sb.Append("\/")
                        Exit Select
                    Case "m"c
                        sb.Append("|\/|")
                        Exit Select
                    Case "M"c
                        sb.Append("|\/|")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
                '#Region "Degree < 100 and > 80"
            ElseIf Degree > 80 AndAlso Degree < 100 Then
                Select Case c
                    Case "a"c
                        sb.Append("4")
                        Exit Select
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "i"c
                        sb.Append("1")
                        Exit Select
                    Case "o"c
                        sb.Append("0")
                        Exit Select
                    Case "A"c
                        sb.Append("4")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case "I"c
                        sb.Append("1")
                        Exit Select
                    Case "O"c
                        sb.Append("0")
                        Exit Select
                    Case "s"c
                        sb.Append("$")
                        Exit Select
                    Case "S"c
                        sb.Append("$")
                        Exit Select
                    Case "g"c
                        sb.Append("9")
                        Exit Select
                    Case "G"c
                        sb.Append("6")
                        Exit Select
                    Case "l"c
                        sb.Append("£")
                        Exit Select
                    Case "L"c
                        sb.Append("£")
                        Exit Select
                    Case "c"c
                        sb.Append("(")
                        Exit Select
                    Case "C"c
                        sb.Append("(")
                        Exit Select
                    Case "t"c
                        sb.Append("7")
                        Exit Select
                    Case "T"c
                        sb.Append("7")
                        Exit Select
                    Case "z"c
                        sb.Append("2")
                        Exit Select
                    Case "Z"c
                        sb.Append("2")
                        Exit Select
                    Case "y"c
                        sb.Append("¥")
                        Exit Select
                    Case "Y"c
                        sb.Append("¥")
                        Exit Select
                    Case "U"c
                        sb.Append("µ")
                        Exit Select
                    Case "u"c
                        sb.Append("µ")
                        Exit Select
                    Case "f"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "F"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "d"c
                        sb.Append("Ð")
                        Exit Select
                    Case "D"c
                        sb.Append("Ð")
                        Exit Select
                    Case "n"c
                        sb.Append("|\|")
                        Exit Select
                    Case "N"c
                        sb.Append("|\|")
                        Exit Select
                    Case "w"c
                        sb.Append("\/\/")
                        Exit Select
                    Case "W"c
                        sb.Append("\/\/")
                        Exit Select
                    Case "h"c
                        sb.Append("|-|")
                        Exit Select
                    Case "H"c
                        sb.Append("|-|")
                        Exit Select
                    Case "v"c
                        sb.Append("\/")
                        Exit Select
                    Case "V"c
                        sb.Append("\/")
                        Exit Select
                    Case "k"c
                        sb.Append("|{")
                        Exit Select
                    Case "K"c
                        sb.Append("|{")
                        Exit Select
                    Case "r"c
                        sb.Append("®")
                        Exit Select
                    Case "R"c
                        sb.Append("®")
                        Exit Select
                    Case "m"c
                        sb.Append("|\/|")
                        Exit Select
                    Case "M"c
                        sb.Append("|\/|")
                        Exit Select
                    Case "b"c
                        sb.Append("ß")
                        Exit Select
                    Case "B"c
                        sb.Append("ß")
                        Exit Select
                    Case "q"c
                        sb.Append("Q")
                        Exit Select
                    Case "Q"c
                        sb.Append("Q¸")
                        Exit Select
                    Case "x"c
                        sb.Append(")(")
                        Exit Select
                    Case "X"c
                        sb.Append(")(")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
                '#Region "Degree 100"
            ElseIf Degree > 99 Then
                Select Case c
                    Case "a"c
                        sb.Append("4")
                        Exit Select
                    Case "e"c
                        sb.Append("3")
                        Exit Select
                    Case "i"c
                        sb.Append("1")
                        Exit Select
                    Case "o"c
                        sb.Append("0")
                        Exit Select
                    Case "A"c
                        sb.Append("4")
                        Exit Select
                    Case "E"c
                        sb.Append("3")
                        Exit Select
                    Case "I"c
                        sb.Append("1")
                        Exit Select
                    Case "O"c
                        sb.Append("0")
                        Exit Select
                    Case "s"c
                        sb.Append("$")
                        Exit Select
                    Case "S"c
                        sb.Append("$")
                        Exit Select
                    Case "g"c
                        sb.Append("9")
                        Exit Select
                    Case "G"c
                        sb.Append("6")
                        Exit Select
                    Case "l"c
                        sb.Append("£")
                        Exit Select
                    Case "L"c
                        sb.Append("£")
                        Exit Select
                    Case "c"c
                        sb.Append("(")
                        Exit Select
                    Case "C"c
                        sb.Append("(")
                        Exit Select
                    Case "t"c
                        sb.Append("7")
                        Exit Select
                    Case "T"c
                        sb.Append("7")
                        Exit Select
                    Case "z"c
                        sb.Append("2")
                        Exit Select
                    Case "Z"c
                        sb.Append("2")
                        Exit Select
                    Case "y"c
                        sb.Append("¥")
                        Exit Select
                    Case "Y"c
                        sb.Append("¥")
                        Exit Select
                    Case "U"c
                        sb.Append("µ")
                        Exit Select
                    Case "u"c
                        sb.Append("µ")
                        Exit Select
                    Case "f"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "F"c
                        sb.Append("ƒ")
                        Exit Select
                    Case "d"c
                        sb.Append("Ð")
                        Exit Select
                    Case "D"c
                        sb.Append("Ð")
                        Exit Select
                    Case "n"c
                        sb.Append("|\|")
                        Exit Select
                    Case "N"c
                        sb.Append("|\|")
                        Exit Select
                    Case "w"c
                        sb.Append("\/\/")
                        Exit Select
                    Case "W"c
                        sb.Append("\/\/")
                        Exit Select
                    Case "h"c
                        sb.Append("|-|")
                        Exit Select
                    Case "H"c
                        sb.Append("|-|")
                        Exit Select
                    Case "v"c
                        sb.Append("\/")
                        Exit Select
                    Case "V"c
                        sb.Append("\/")
                        Exit Select
                    Case "k"c
                        sb.Append("|{")
                        Exit Select
                    Case "K"c
                        sb.Append("|{")
                        Exit Select
                    Case "r"c
                        sb.Append("®")
                        Exit Select
                    Case "R"c
                        sb.Append("®")
                        Exit Select
                    Case "m"c
                        sb.Append("|\/|")
                        Exit Select
                    Case "M"c
                        sb.Append("|\/|")
                        Exit Select
                    Case "b"c
                        sb.Append("ß")
                        Exit Select
                    Case "B"c
                        sb.Append("ß")
                        Exit Select
                    Case "j"c
                        sb.Append("_|")
                        Exit Select
                    Case "J"c
                        sb.Append("_|")
                        Exit Select
                    Case "P"c
                        sb.Append("|°")
                        Exit Select
                    Case "q"c
                        sb.Append("¶")
                        Exit Select
                    Case "Q"c
                        sb.Append("¶¸")
                        Exit Select
                    Case "x"c
                        sb.Append(")(")
                        Exit Select
                    Case "X"c
                        sb.Append(")(")
                        Exit Select
                    Case Else
                        sb.Append(c)
                        Exit Select
                End Select
                '#End Region
            End If
        Next
        Return sb.ToString()
        ' Return result.
    End Function

    ''' <summary>
    ''' Retorna um numero com o sinal de porcentagem
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToPercentString(Number As Decimal, Optional Decimals As Integer = -1) As String
        If Decimals > -1 Then
            Number = Decimal.Round(Number, Decimals)
        End If
        Return Number.ToString & "%"
    End Function

    ''' <summary>
    ''' Retorna um numero com o sinal de porcentagem
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToPercentString(Number As Integer) As String
        Return Number.ToString & ("%")
    End Function

    ''' <summary>
    ''' Retorna um numero com o sinal de porcentagem
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToPercentString(Number As Double, Optional Decimals As Integer = -1) As String
        If Decimals > -1 Then
            Number = Decimal.Round(Number.ChangeType(Of Decimal), Decimals)
        End If
        Return Number.ToString & ("%")
    End Function

    ''' <summary>
    ''' Retorna um numero com o sinal de porcentagem
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToPercentString(Number As Short) As String
        Return Number.ToString & ("%")
    End Function

    ''' <summary>
    ''' Retorna um numero com o sinal de porcentagem
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToPercentString(Number As Long) As String
        Return Number.ToString & ("%")
    End Function

    ''' <summary>
    ''' Coloca o texto em TitleCase
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToProperCase(Text As String, Optional ForceCase As Boolean = False) As String
        If Text.IsBlank Then Return Text
        If ForceCase Then Text = Text.ToLower()
        Dim l = Text.Split(" ", StringSplitOptions.None).ToList
        For index = 0 To l.Count - 1
            Dim pal = l(index)

            Dim artigo = index > 0 AndAlso pal.IsIn("o", "a", "os", "as", "um", "uma", "uns", "umas", "de", "do", "dos", "das", "e")

            If pal.IsNotBlank Then
                If ForceCase OrElse artigo = False Then
                    Dim c = pal.First

                    If Not Char.IsUpper(c) Then
                        pal = Char.ToUpper(c) & pal.RemoveFirstChars(1)
                    End If

                    l(index) = pal
                End If
            End If
        Next
        Return l.SelectJoin(" ")

    End Function

    ''' <summary>
    ''' Coloca a string em Randomcase (aleatoriamente letras maiusculas ou minusculas)
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <param name="Times">Numero de vezes que serão sorteados caracteres.</param>
    ''' <returns></returns>
    <Extension> Public Function ToRandomCase(Text As String, Optional Times As Integer = 0) As String
        Dim ch = Text.ToArray
        Times = Times.SetMinValue(ch.Length)
        For index = 1 To Times
            Dim newindex = RandomNumber(0, ch.Length - 1)
            If Char.IsUpper(ch(newindex)) Then
                ch(newindex) = Char.ToLower(ch(newindex))
            Else
                ch(newindex) = Char.ToUpper(ch(newindex))
            End If
        Next
        Return New String(ch)
    End Function

    ''' <summary>
    ''' Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca
    ''' espacos por hifen). É um alias para <see cref="ToFriendlyURL(String, Boolean)"/>
    ''' </summary>
    ''' <param name="Text">         </param>
    ''' <param name="UseUnderscore">
    ''' Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
    ''' </param>
    ''' <returns>string amigavel para URL</returns>
    <Extension()> Function ToSlugCase(Text As String, Optional UseUnderscore As Boolean = False) As String
        Return Text.ToFriendlyURL(UseUnderscore)
    End Function

    ''' <summary>
    ''' Retorna uma string em Snake_Case
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToSnakeCase(Text As String) As String
        Return Text.Replace(" ", "_")
    End Function

    ''' <summary>
    ''' Cria um <see cref="Stream"/> a partir de uma string
    ''' </summary>
    ''' <param name="TExt"></param>
    ''' <returns></returns>
    <Extension> Public Function ToStream(Text As String) As Stream
        Dim stream As New MemoryStream()
        Dim writer As New StreamWriter(stream)
        writer.Write(Text)
        writer.Flush()
        stream.Position = 0
        Return stream
    End Function

    ''' <summary>
    ''' Aplica uma mascara a um numero de telefone
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension> Public Function MaskTelephoneNumber(Number As String) As String
        Number = If(Number, "")
        Dim mask As String = ""
        Number = Number.ParseDigits.RemoveAny(",", ".")
        If Number.IsBlank Then
            Return ""
        End If
        Select Case Number.Length
            Case Is <= 4
                mask = "{0:####}"
            Case Is <= 8
                mask = "{0:####-####}"
            Case 9
                mask = "{0:#####-####}"
            Case 10
                mask = "{0:(##) ####-####}"
            Case 11
                mask = "{0:(##) #####-####}"
            Case 12
                mask = "{0:+## (##) ####-####}"
            Case 13
                mask = "{0:+## (##) #####-####}"
            Case Else
                Return Number.ToString
        End Select
        Return String.Format(mask, Long.Parse(Number))
    End Function

    ''' <summary>
    ''' Aplica uma mascara a um numero de telefone
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension> Public Function MaskTelephoneNumber(Number As Long) As String
        Return Number.ToString.MaskTelephoneNumber
    End Function

    ''' <summary>
    ''' Aplica uma mascara a um numero de telefone
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension> Public Function MaskTelephoneNumber(Number As Integer) As String
        Return Number.ToString.MaskTelephoneNumber
    End Function

    ''' <summary>
    ''' Aplica uma mascara a um numero de telefone
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension> Public Function MaskTelephoneNumber(Number As Decimal) As String
        Return Number.ToString.MaskTelephoneNumber
    End Function

    ''' <summary>
    ''' Aplica uma mascara a um numero de telefone
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension> Public Function MaskTelephoneNumber(Number As Double) As String
        Return Number.ToString.MaskTelephoneNumber
    End Function

    ''' <summary>
    ''' Transforma um texto em titulo
    ''' </summary>
    ''' <param name="Text">Texto a ser manipulado</param>
    ''' <param name="ForceCase">Se FALSE, apenas altera o primeiro caractere de cada palavra como UPPERCASE, dexando os demais intactos. Se TRUE, força o primeiro caractere de casa palavra como UPPERCASE e os demais como LOWERCASE</param>
    ''' <returns>Uma String com o texto em nome próprio</returns>
    <Extension()>
    Public Function ToTitle(Text As String, Optional ForceCase As Boolean = False) As String
        Return ToProperCase(Text, ForceCase)
    End Function

    ''' <summary>
    ''' Transforma um XML Document em string
    ''' </summary>
    ''' <param name="XML">Documento XML</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToXMLString(XML As XmlDocument) As String
        Using stringWriter = New StringWriter()
            Using xmlTextWriter = XmlWriter.Create(stringWriter)
                XML.WriteTo(xmlTextWriter)
                xmlTextWriter.Flush()
                Return stringWriter.GetStringBuilder().ToString()
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Remove do começo e do final de uma string qualquer valor que estiver no conjunto
    ''' </summary>
    ''' <param name="Text">              Texto</param>
    ''' <param name="ContinuouslyRemove">
    ''' Parametro que indica se a string deve continuar sendo testada até que todas as ocorrencias
    ''' sejam removidas
    ''' </param>
    ''' <param name="StringTest">        Conjunto de textos que serão comparados</param>
    ''' <returns></returns>
    <Extension()> Public Function TrimAny(ByVal Text As String, ContinuouslyRemove As Boolean, ParamArray StringTest As String()) As String
        If Text IsNot Nothing Then
            Text = Text.RemoveFirstAny(ContinuouslyRemove, StringTest)
            Text = Text.RemoveLastAny(ContinuouslyRemove, StringTest)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Remove do começo e do final de uma string qualquer valor que estiver no conjunto
    ''' </summary>
    ''' <param name="Text">      Texto</param>
    ''' <param name="StringTest">Conjunto de textos que serão comparados</param>
    ''' <returns></returns>
    <Extension()> Public Function TrimAny(ByVal Text As String, ParamArray StringTest As String()) As String
        Return Text.TrimAny(True, StringTest)
    End Function

    ''' <summary>
    ''' Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Function TrimCarriage(Text As String) As String
        Return Text.TrimAny(WhiteSpaceChars.ToArray())
    End Function

    ''' <summary>
    ''' Decoda uma string de uma transmissão por URL
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function UrlDecode(ByVal Text As String) As String
        If Text.IsNotBlank Then
            Return Net.WebUtility.UrlDecode(Text)
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Encoda uma string para transmissão por URL
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function UrlEncode(ByVal Text As String) As String
        If Text.IsNotBlank Then
            Return Net.WebUtility.UrlEncode(Text)
        End If
        Return ""
    End Function

    <Extension()> Public Function ForEachLine(Text As String, Action As Expression(Of Func(Of String, String))) As String
        If Text.IsNotBlank AndAlso Action IsNot Nothing Then
            Text = Text.SplitAny(BreakLineChars.ToArray).Select(Function(x) Action.Compile.Invoke(x)).Join(Environment.NewLine)
        End If
        Return Text
    End Function


    ''' <summary>
    ''' Encapsula um tento entre 2 textos
    ''' </summary>
    ''' <param name="Text">    Texto</param>
    ''' <param name="WrapText">Caractere de encapsulamento</param>
    ''' <returns></returns>
    <Extension()>
    Function Wrap(Text As String, Optional WrapText As String = """") As String
        If Text.IsNotBlank Then
            Return WrapText & Text & WrapText
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Encapsula um tento entre 2 textos
    ''' </summary>
    ''' <param name="Text">    Texto</param>
    ''' <returns></returns>
    <Extension()>
    Function Wrap(Text As String, OpenWrapText As String, CloseWrapText As String) As String
        If Text.IsNotBlank Then
            Return OpenWrapText & Text & CloseWrapText.IfBlank(OpenWrapText)
        End If
        Return Text
    End Function

    <Extension()> Function UnWrap(Text As String, Optional WrapText As String = """", Optional ContinuouslyRemove As Boolean = False) As String
        Return Text.TrimAny(ContinuouslyRemove, WrapText)
    End Function

    <Extension()>
    Function Inject(ByVal formatString As String, ByVal injectionObject As Object) As String
        Return formatString.Inject(GetPropertyHash(injectionObject))
    End Function

    <Extension()>
    Function Inject(ByVal formatString As String, ByVal dictionary As IDictionary) As String
        Return formatString.Inject(New Hashtable(dictionary))
    End Function

    <Extension()>
    Function Inject(ByVal formatString As String, ByVal attributes As Hashtable) As String
        Dim result As String = formatString
        If attributes IsNot Nothing AndAlso formatString IsNot Nothing Then
            For Each attributeKey As String In attributes.Keys
                result = result.InjectSingleValue(attributeKey, attributes(attributeKey))
            Next
        End If
        Return result
    End Function

    <Extension()>
    Function InjectSingleValue(ByVal formatString As String, ByVal key As String, ByVal replacementValue As Object) As String
        Dim result As String = formatString
        Dim attributeRegex As Regex = New Regex("{(" & key & ")(?:}|(?::(.[^}]*)}))")

        For Each m As Match In attributeRegex.Matches(formatString)
            Dim replacement As String = m.ToString()

            If m.Groups(2).Length > 0 Then
                Dim attributeFormatString As String = String.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups(2))
                replacement = String.Format(CultureInfo.CurrentCulture, attributeFormatString, replacementValue)
            Else
                replacement = (If(replacementValue, String.Empty)).ToString()
            End If

            result = result.Replace(m.ToString(), replacement)
        Next

        Return result
    End Function

    Private Function GetPropertyHash(ByVal properties As Object) As Hashtable
        Dim values As Hashtable = Nothing

        If properties IsNot Nothing Then
            values = New Hashtable()
            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(properties)

            For Each prop As PropertyDescriptor In props
                values.Add(prop.Name, prop.GetValue(properties))
            Next
        End If

        Return values
    End Function

End Module

Public Class QuantityTextPair

    Public Sub New(Plural As String, Optional Singular As String = "")
        Me.Plural = Plural
        Me.Singular = Singular.IfBlank(Plural.QuantifyText(1))
    End Sub

    Sub New()
    End Sub

    Default Property Text(Number As IComparable) As String
        Get
            Return Me.ToString(CType(Number, Decimal))
        End Get
        Set(value As String)
            If Verify.IsNumber(Number) AndAlso CType(Number, Decimal).Floor().IsIn(1, -1) Then
                Singular = value
            Else
                Plural = value
            End If
        End Set
    End Property

    Property Singular As String = "Item"

    Property Plural As String = "Items"


    Public Overrides Function ToString() As String
        Return Plural
    End Function

    Public Overloads Function ToString(Number As Long) As String
        Return If(Number.IsIn(1, -1), Singular, Plural)
    End Function

    Public Overloads Function ToString(Number As Decimal) As String
        Return If(Number.Floor.IsIn(1, -1), Singular, Plural)
    End Function

    Public Overloads Function ToString(Number As Short) As String
        Return If(Number.IsIn(1, -1), Singular, Plural)
    End Function

    Public Overloads Function ToString(Number As Integer) As String
        Return If(Number.IsIn(1, -1), Singular, Plural)
    End Function

    Public Overloads Function ToString(Number As Double) As String
        Return If(Number.Floor.IsIn(1, -1), Singular, Plural)
    End Function

    Public Overloads Function ToString(Number As Single) As String
        Return If(Number.IsIn(1, -1), Singular, Plural)
    End Function

End Class

''' <summary>
''' Classe para escrever numeros por extenso com suporte até 999 quintilhoes
''' </summary>
Public Class FullNumberWriter

    ''' <summary>
    ''' String que representa a palavra "Menos". Utilizada quando os números são negativos
    ''' </summary>
    ''' <returns></returns>
    Property Minus As String

    ''' <summary>
    ''' String que representa o numero 0.
    ''' </summary>
    ''' <returns></returns>
    Property Zero As String

    ''' <summary>
    ''' String que representa a palavra "e". Utilizada na concatenação de expressões
    ''' </summary>
    ''' <returns></returns>
    Property [And] As String

    ''' <summary>
    ''' String que representa o numero 1.
    ''' </summary>
    ''' <returns></returns>
    Property One As String

    ''' <summary>
    ''' String que representa o numero 2.
    ''' </summary>
    ''' <returns></returns>
    Property Two As String

    ''' <summary>
    ''' String que representa o numero 3.
    ''' </summary>
    ''' <returns></returns>
    Property Three As String

    ''' <summary>
    ''' String que representa o numero 4.
    ''' </summary>
    ''' <returns></returns>
    Property Four As String

    ''' <summary>
    ''' String que representa o numero 5.
    ''' </summary>
    ''' <returns></returns>
    Property Five As String

    ''' <summary>
    ''' String que representa o numero 6.
    ''' </summary>
    ''' <returns></returns>
    Property Six As String

    ''' <summary>
    ''' String que representa o numero 7.
    ''' </summary>
    ''' <returns></returns>
    Property Seven As String

    ''' <summary>
    ''' String que representa o numero 8.
    ''' </summary>
    ''' <returns></returns>
    Property Eight As String

    ''' <summary>
    ''' String que representa o numero 9.
    ''' </summary>
    ''' <returns></returns>
    Property Nine As String

    ''' <summary>
    ''' String que representa o numero 10.
    ''' </summary>
    ''' <returns></returns>
    Property Ten As String

    ''' <summary>
    ''' String que representa o numero 11.
    ''' </summary>
    ''' <returns></returns>
    Property Eleven As String

    ''' <summary>
    ''' String que representa o numero 12.
    ''' </summary>
    ''' <returns></returns>
    Property Twelve As String

    ''' <summary>
    ''' String que representa o numero 13.
    ''' </summary>
    ''' <returns></returns>
    Property Thirteen As String

    ''' <summary>
    ''' String que representa o numero 14.
    ''' </summary>
    ''' <returns></returns>
    Property Fourteen As String

    ''' <summary>
    ''' String que representa o numero 15.
    ''' </summary>
    ''' <returns></returns>
    Property Fifteen As String

    ''' <summary>
    ''' String que representa o numero 16.
    ''' </summary>
    ''' <returns></returns>
    Property Sixteen As String

    ''' <summary>
    ''' String que representa o numero 17.
    ''' </summary>
    ''' <returns></returns>
    Property Seventeen As String

    ''' <summary>
    ''' String que representa o numero 18.
    ''' </summary>
    ''' <returns></returns>
    Property Eighteen As String

    ''' <summary>
    ''' String que representa o numero 19.
    ''' </summary>
    ''' <returns></returns>
    Property Nineteen As String

    ''' <summary>
    ''' String que representa os numeros 20 a 29 .
    ''' </summary>
    ''' <returns></returns>
    Property Twenty As String

    ''' <summary>
    ''' String que representa os numeros 30 a 39.
    ''' </summary>
    ''' <returns></returns>
    Property Thirty As String

    ''' <summary>
    ''' String que representa os numeros 40 a 49.
    ''' </summary>
    ''' <returns></returns>
    Property Fourty As String

    ''' <summary>
    ''' String que representa os numeros 50 a 59.
    ''' </summary>
    ''' <returns></returns>
    Property Fifty As String

    ''' <summary>
    ''' String que representa os numeros 60 a 69.
    ''' </summary>
    ''' <returns></returns>
    Property Sixty As String

    ''' <summary>
    ''' String que representa os numeros 70 a 79.
    ''' </summary>
    ''' <returns></returns>
    Property Seventy As String

    ''' <summary>
    ''' String que representa os numeros 80 a 89.
    ''' </summary>
    ''' <returns></returns>
    Property Eighty As String

    ''' <summary>
    ''' String que representa os numeros 90 a 99.
    ''' </summary>
    ''' <returns></returns>
    Property Ninety As String

    ''' <summary>
    ''' String que represena o exato numero 100. Em alguns idiomas esta string não é nescessária
    ''' </summary>
    ''' <returns></returns>
    Property ExactlyOneHundred As String

    ''' <summary>
    ''' String que representa os numeros 100 a 199.
    ''' </summary>
    ''' <returns></returns>
    Property OneHundred As String

    ''' <summary>
    ''' String que representa os numeros 200 a 299.
    ''' </summary>
    ''' <returns></returns>
    Property TwoHundred As String

    ''' <summary>
    ''' String que representa os numeros 300 a 399.
    ''' </summary>
    ''' <returns></returns>
    Property ThreeHundred As String

    ''' <summary>
    ''' String que representa os numeros 400 a 499.
    ''' </summary>
    ''' <returns></returns>
    Property FourHundred As String

    ''' <summary>
    ''' String que representa os numeros 500 a 599.
    ''' </summary>
    ''' <returns></returns>
    Property FiveHundred As String

    ''' <summary>
    ''' String que representa os numeros 600 a 699.
    ''' </summary>
    ''' <returns></returns>
    Property SixHundred As String

    ''' <summary>
    ''' String que representa os numeros 700 a 799.
    ''' </summary>
    ''' <returns></returns>
    Property SevenHundred As String

    ''' <summary>
    ''' String que representa os numeros 800 a 899.
    ''' </summary>
    ''' <returns></returns>
    Property EightHundred As String

    ''' <summary>
    ''' String que representa os numeros 900 a 999.
    ''' </summary>
    ''' <returns></returns>
    Property NineHundred As String

    ''' <summary>
    ''' String que representa os numeros 1000 a 9999
    ''' </summary>
    ''' <returns></returns>
    Property Thousand As String

    ''' <summary>
    ''' Par de strings que representam os numeros 1 milhão a 999 milhões
    ''' </summary>
    ''' <returns></returns>
    Property Million As New QuantityTextPair

    ''' <summary>
    ''' Par de strings que representam os numeros 1 bilhão a 999 bilhões
    ''' </summary>
    ''' <returns></returns>
    Property Billion As New QuantityTextPair

    ''' <summary>
    ''' Par de strings que representam os numeros 1 trilhão a 999 trilhões
    ''' </summary>
    ''' <returns></returns>
    Property Trillion As New QuantityTextPair

    ''' <summary>
    ''' Par de strings que representam os numeros 1 quadrilhão a 999 quadrilhões
    ''' </summary>
    ''' <returns></returns>
    Property Quadrillion As New QuantityTextPair

    ''' <summary>
    ''' Par de strings que representam os numeros 1 quintilhão a 999 quintilhões
    ''' </summary>
    ''' <returns></returns>
    Property Quintillion As New QuantityTextPair

    ''' <summary>
    ''' String utilizada quando o numero é maior que 999 quintilhões. Retorna uma string "Mais de 999 quintilhões"
    ''' </summary>
    ''' <returns></returns>
    Property MoreThan As String

    ''' <summary>
    ''' String utilizada quando um numero possui casa decimais. Normalmente "virgula"
    ''' </summary>
    ''' <returns></returns>
    Property DecimalSeparator As String

    ''' <summary>
    ''' Instancia um novo <see cref="FullNumberWriter"/> com as configurações default (inglês)
    ''' </summary>
    Sub New()
        For Each prop In Me.GetProperties.Where(Function(x) x.CanWrite)
            Select Case prop.Name
                Case "ExactlyOneHundred", "DecimalSeparator"
                    Continue For
                Case Else
                    Select Case prop.PropertyType
                        Case GetType(String)
                            prop.SetValue(Me, prop.Name.CamelAdjust)
                        Case GetType(QuantityTextPair)
                            If CType(prop.GetValue(Me), QuantityTextPair).Plural.IsBlank Then
                                prop.SetValue(Me, New QuantityTextPair(prop.Name & "s", prop.Name))
                            End If
                        Case Else
                    End Select
            End Select
        Next
    End Sub

    ''' <summary>
    ''' Escreve um numero por extenso
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    Default Public Overridable ReadOnly Property Text(Number As Decimal, Optional DecimalPlaces As Integer = 2) As String
        Get
            Return ToString(Number, DecimalPlaces)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return ToString(0)
    End Function

    Public Overridable Overloads Function ToString(Number As Decimal, Optional DecimalPlaces As Integer = 2) As String
        Dim dec As Long = Number.GetDecimalPlaces(DecimalPlaces.LimitRange(0, 3))
        Dim num As Long = Number.Floor
        Return (InExtensive(num) & If(dec = 0 Or DecimalPlaces = 0, "", DecimalSeparator.Wrap(" ") & InExtensive(dec))).ToLower.AdjustWhiteSpaces
    End Function

    Friend Function InExtensive(ByVal Number As Decimal) As String

        Select Case Number
            Case Is < 0
                Return Minus & " " & InExtensive(Number * (-1))
            Case 0
                Return Zero
            Case 1 To 19
                Dim strArray() As String = {One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve,
                   Thirteen, Fourteen, Fifteen, Sixteen, Seventeen, Eighteen, Nineteen}
                Return strArray(Number - 1) & " "
            Case 20 To 99
                Dim strArray() As String = {Twenty, Thirty, Fourty, Fifty, Sixty, Seventy, Eighty, Ninety}
                If (Number Mod 10) = 0 Then
                    Return strArray(Number \ 10 - 2)
                Else
                    Return strArray(Number \ 10 - 2) & [And].Wrap(" ") & InExtensive(Number Mod 10)
                End If
            Case 100
                Return ExactlyOneHundred.IfBlank(OneHundred)
            Case 101 To 999
                Dim strArray() As String = {OneHundred, TwoHundred, ThreeHundred, FourHundred, FiveHundred, SixHundred, SevenHundred, EightHundred, NineHundred}
                If (Number Mod 100) = 0 Then
                    Return strArray(Number \ 100 - 1) & " "
                Else
                    Return strArray(Number \ 100 - 1) & [And].Wrap(" ") & InExtensive(Number Mod 100)
                End If
            Case 1000 To 1999
                Select Case (Number Mod 1000)
                    Case 0
                        Return Thousand
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return Thousand & [And].Wrap(" ") & InExtensive(Number Mod 1000)
                    Case Else
                        Return Thousand & " " & InExtensive(Number Mod 1000)
                End Select
            Case 2000 To 999999
                Select Case (Number Mod 1000)
                    Case 0
                        Return InExtensive(Number \ 1000) & " " & Thousand
                    Case Is <= 100
                        Return InExtensive(Number \ 1000) & " " & Thousand & [And].Wrap(" ") & InExtensive(Number Mod 1000)
                    Case Else
                        Return InExtensive(Number \ 1000) & " " & Thousand & " " & InExtensive(Number Mod 1000)
                End Select

#Region "Milhao"

            Case 1000000 To 1999999
                Select Case (Number Mod 1000000)
                    Case 0
                        Return One & " " & Million.Singular
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return One & " " & Million.Singular & [And].Wrap(" ") & InExtensive(Number Mod 1000000)
                    Case Else
                        Return One & " " & Million.Singular & " " & InExtensive(Number Mod 1000000)
                End Select
            Case 2000000 To 999999999
                Select Case (Number Mod 1000000)
                    Case 0
                        Return InExtensive(Number \ 1000000) & Million.Plural.Wrap(" ")
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return InExtensive(Number \ 1000000) & Million.Plural.Wrap(" ") & [And].Wrap(" ") & InExtensive(Number Mod 1000000)
                    Case Else
                        Return InExtensive(Number \ 1000000) & Million.Plural.Wrap(" ") & InExtensive(Number Mod 1000000)
                End Select

#End Region

#Region "Bilhao"

            Case 1000000000 To 1999999999
                Select Case (Number Mod 1000000000)
                    Case 0
                        Return One & " " & Billion.Singular
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return One & " " & Billion.Singular & [And].Wrap(" ") & InExtensive(Number Mod 1000000000)
                    Case Else
                        Return One & " " & Billion.Singular & " " & InExtensive(Number Mod 1000000000)
                End Select
            Case 2000000000 To 999999999999
                Select Case (Number Mod 1000000)
                    Case 0
                        Return InExtensive(Number \ 1000000000) & Billion.Plural.Wrap(" ")
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return InExtensive(Number \ 1000000000) & Billion.Plural.Wrap(" ") & [And].Wrap(" ") & InExtensive(Number Mod 1000000000)
                    Case Else
                        Return InExtensive(Number \ 1000000000) & Billion.Plural.Wrap(" ") & InExtensive(Number Mod 1000000000)
                End Select

#End Region

#Region "Trilhao"

            Case 1000000000000 To 1999999999999
                Select Case (Number Mod 1000000000000)
                    Case 0
                        Return One & " " & Trillion.Singular
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return One & " " & Trillion.Singular & [And].Wrap(" ") & InExtensive(Number Mod 1000000000000)
                    Case Else
                        Return One & " " & Trillion.Singular & " " & InExtensive(Number Mod 1000000000000)
                End Select
                                  '9.223.372.036.854.775.807
            Case 2000000000000 To 999999999999999
                Select Case (Number Mod 1000000000000)
                    Case 0
                        Return InExtensive(Number \ 1000000000000) & Trillion.Plural.Wrap(" ")
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return InExtensive(Number \ 1000000000000) & Trillion.Plural.Wrap(" ") & [And].Wrap(" ") & InExtensive(Number Mod 1000000000000)
                    Case Else
                        Return InExtensive(Number \ 1000000000000) & Trillion.Plural.Wrap(" ") & InExtensive(Number Mod 1000000000000)
                End Select

#End Region

#Region "Quadilhao"

            Case 1000000000000000 To 1999999999999999
                Select Case (Number Mod 1000000000000000)
                    Case 0
                        Return One & " " & Quadrillion.Singular
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return One & " " & Quadrillion.Singular & [And].Wrap(" ") & InExtensive(Number Mod 1000000000000)
                    Case Else
                        Return One & " " & Quadrillion.Singular & " " & InExtensive(Number Mod 1000000000000)
                End Select

            Case 2000000000000000 To 999999999999999999
                Select Case (Number Mod 1000000000000000)
                    Case 0
                        Return InExtensive(Number \ 1000000000000000) & Quadrillion.Plural.Wrap(" ")
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return InExtensive(Number \ 1000000000000000) & Quadrillion.Plural.Wrap(" ") & [And].Wrap(" ") & InExtensive(Number Mod 1000000000000000)
                    Case Else
                        Return InExtensive(Number \ 1000000000000000) & Quadrillion.Plural.Wrap(" ") & InExtensive(Number Mod 1000000000000000)
                End Select

#End Region

#Region "Quintilhao"

            Case 1000000000000000000 To 1999999999999999999
                Select Case (Number Mod 1000000000000000000)
                    Case 0
                        Return One & " " & Quintillion.Singular
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return One & " " & Quintillion.Singular & [And].Wrap(" ") & InExtensive(Number Mod 1000000000000000000)
                    Case Else
                        Return One & " " & Quintillion.Singular & " " & InExtensive(Number Mod 1000000000000000000)
                End Select

            Case 2000000000000000000 To 999999999999999999999D
                Select Case (Number Mod 1000000000000000000)
                    Case 0
                        Return InExtensive(Number \ 1000000000000000000) & Quintillion.Plural.Wrap(" ")
                    Case Is <= 100, 200, 300, 400, 500, 600, 700, 800, 900
                        Return InExtensive(Number \ 1000000000000000000) & Quintillion.Plural.Wrap(" ") & [And].Wrap(" ") & InExtensive(Number Mod 1000000000000000000)
                    Case Else
                        Return InExtensive(Number \ 1000000000000000000) & Quintillion.Plural.Wrap(" ") & InExtensive(Number Mod 1000000000000000000)
                End Select

#End Region

            Case Else
                Return MoreThan & " " & InExtensive(999999999999999999999D)
        End Select

    End Function

End Class

''' <summary>
''' Classe para escrever moedas por extenso com suporte até 999 quintilhoes de $$
''' </summary>
Public Class FullMoneyWriter
    Inherits FullNumberWriter

    ''' <summary>
    ''' Par de strings que representam os nomes da moeda em sua forma singular ou plural
    ''' </summary>
    ''' <returns></returns>
    Property CurrencyName As New QuantityTextPair("dollars", "dollar")

    ''' <summary>
    ''' Par de strings que representam os centavos desta moeda em sua forma singular ou plural
    ''' </summary>
    ''' <returns></returns>
    Property CurrencyCentsName As New QuantityTextPair("cents", "cent")

    ''' <summary>
    ''' Escreve um numero por extenso
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    Public Overrides Function ToString(Number As Decimal, Optional DecimalPlaces As Integer = 2) As String
        Dim dec As Long = Number.GetDecimalPlaces(DecimalPlaces.LimitRange(0, 3))
        Dim num As Long = Number.Floor
        Return (InExtensive(num) & CurrencyCentsName(num).Wrap(" ") & If(dec = 0 Or DecimalPlaces = 0, "", [And].Wrap(" ") & InExtensive(dec) & CurrencyCentsName(dec).Wrap(" "))).ToLower.AdjustWhiteSpaces
    End Function

    ''' <summary>
    ''' Escreve um numero por extenso
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    Public Overloads Function ToString(Number As Money, Optional DecimalPlaces As Integer = 2) As String
        Return ToString(Number.Value, DecimalPlaces)
    End Function

    Public Overrides Function ToString() As String
        Return Me.ToString(0D)
    End Function

End Class

Public Class ConnectionStringParser
    Inherits Dictionary(Of String, String)



    Sub New()
        MyBase.New
    End Sub

    Sub New(ConnectionString As String)
        MyBase.New
        Parse(ConnectionString)
    End Sub

    Public Function Parse(ConnectionString As String) As ConnectionStringParser
        Try
            For Each ii In ConnectionString.IfBlank("").SplitAny(";").[Select](Function(t) t.Split(New Char() {"="c}, 2)).ToDictionary(Function(t) t(0).Trim(), Function(t) t(1).Trim(), StringComparer.InvariantCultureIgnoreCase)
                Me.Set(ii.Key.ToTitle(True), ii.Value)
            Next
        Catch ex As Exception
        End Try
        Return Me
    End Function

    ''' <summary>
    ''' Retorna a connectionstring deste parser
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return Me.SelectJoin(Function(x) $"{x.Key.ToTitle()}={x.Value}", ";")
    End Function

    Public Shared Widening Operator CType(ByVal cs As ConnectionStringParser) As String
        Return cs.ToString()
    End Operator

    Public Shared Widening Operator CType(s As String) As ConnectionStringParser
        Return New ConnectionStringParser(s)
    End Operator

End Class

''' <summary>
''' Classe para criação de strings contendo tags HTML
''' </summary>
Public Class HtmlTag

    Public Property TagName As String = "div"
    Public Property InnerHtml As String
    Public Property Attributes As New Dictionary(Of String, String)

    Sub New()

    End Sub

    Sub New(TagName As String, Optional InnerHtml As String = "")
        Me.TagName = TagName.IfBlank("div")
        Me.InnerHtml = InnerHtml
    End Sub

    Public Property [Class] As String
        Get
            Return Attributes.GetValueOr("class", "")
        End Get
        Set(ByVal value As String)
            Attributes = If(Attributes, New Dictionary(Of String, String))
            Attributes("class") = value
        End Set
    End Property

    Public Property ClassArray As String()
        Get
            Return [Class].Split(" ")
        End Get
        Set(ByVal value As String())
            [Class] = If(value, {}).Join(" ")
        End Set
    End Property

    Public Overrides Function ToString() As String
        TagName = TagName.RemoveAny("/", "\")
        Attributes = If(Attributes, New Dictionary(Of String, String))
        Return $"<{TagName.IfBlank("div")} {Attributes.SelectJoin(Function(x) x.Key.ToLower & "=" & x.Value.Wrap())}>{InnerHtml}</{TagName.IfBlank("div")}>"
    End Function

    Public Shared Widening Operator CType(ByVal Tag As HtmlTag) As String
        Return Tag?.ToString()
    End Operator

End Class