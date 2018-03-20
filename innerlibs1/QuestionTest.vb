Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Web.Script.Serialization
Imports InnerLibs.HtmlParser

Namespace QuestionTest

    ''' <summary>
    ''' Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem Dissertativas, Multipla Escolha ou de Atribuição de Pontos
    ''' </summary>
    <Serializable>
    Public Class QuestionTest
        Inherits ObservableCollection(Of Question)

        Protected NotOverridable Overrides Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
            MyBase.OnCollectionChanged(e)
            If e.Action = NotifyCollectionChangedAction.Add Then
                Debug.WriteLine("Question Added")
                For Each ni In e.NewItems
                    Dim q = CType(ni, Question)
                    q._test = Me
                Next
            End If
        End Sub

        ''' <summary>
        ''' Verifica se o peso da prova equivale a soma dos pesos das questões
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsValid As Boolean
            Get
                Return Me.Select(Function(q) q.Weight).Sum = Me.Weight
            End Get
        End Property

        ''' <summary>
        ''' Retorna as questões desta avaliação
        ''' </summary>
        ''' <returns></returns>
        <Editor(GetType(QuestionEditor), GetType(System.Drawing.Design.UITypeEditor))>
        Public ReadOnly Property Questions As QuestionTest
            Get
                Return Me
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.Title
        End Function

        ''' <summary>
        ''' Titulo da Avaliação
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String
            Get
                Return _title.ToProper
            End Get
            Set(value As String)
                _title = value.ToProper
            End Set
        End Property

        Private _title As String = ""

        ''' <summary>
        ''' Adiciona uma nova questão a avaliação.
        ''' </summary>
        Public Function CreateQuestion(Of QuestionType As Question)() As QuestionType
            Dim Question = CType(Activator.CreateInstance(GetType(QuestionType), Me), QuestionType)
            If Not MyBase.Contains(Question) Then
                MyBase.Add(Question)
            End If
            Return Question
        End Function

        ''' <summary>
        ''' Pega uma questão por ID
        ''' </summary>
        ''' <typeparam name="T">Tipo da Questão</typeparam>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        Public Function GetQuestion(Of T As Question)(ID As String) As T
            Try
                Return CType(Me.Where(Function(q) q.ID.ToLower = ID.ToLower).First, T)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Pega uma Alternativa de uma Questão pelo ID
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <returns></returns>
        Public Function GetAlternative(ID As String) As Alternative
            Try
                Return GetQuestion(Of AlternativeQuestion)(ID.GetFirstChars(2)).Alternatives.Where(Function(a) a.ID.ToLower = ID.ToLower)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Valor Minimo da nota para aprovação (Normalmente 6)
        ''' </summary>
        ''' <returns></returns>
        Public Property MinimumWeightAllowed As Decimal = 6

        ''' <summary>
        ''' Peso da Avaliação (Normalmente 10)
        ''' </summary>
        ''' <returns></returns>
        Public Property Weight As Decimal = 10

        ''' <summary>
        ''' Cabeçalho da prova. Texto adicional que ficará antes das questões e apoós o título
        ''' </summary>
        ''' <returns></returns>
        Public Property Header As String = ""

        ''' <summary>
        ''' Rodapé da prova. Texto adicional que ficará após as questões
        ''' </summary>
        ''' <returns></returns>
        Public Property Footer As String = ""

        ''' <summary>
        ''' Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo permitido, caso contrário, FALSE
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsApproved
            Get
                Return Me.FinalNote >= MinimumWeightAllowed
            End Get
        End Property

        ''' <summary>
        ''' Numero de questões que o usuário acertou
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Hits As Integer
            Get
                Dim c = 0
                For Each q In Me
                    If q.IsCorrect Then c.Increment
                Next
                Return c
            End Get
        End Property

        ''' <summary>
        ''' Numero de questões que o usuário errou
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Fails As Integer
            Get
                Return Me.Count - Hits
            End Get
        End Property

        ''' <summary>
        ''' Média da Avaliação
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Average As Decimal
            Get
                Try
                    Dim correct = 0
                    Dim somapesos = 0
                    Dim somaquestoes = 0
                    For Each q In Me
                        somapesos = somapesos + q.Weight
                        somaquestoes = somaquestoes + If(q.IsCorrect, q.Hits, 0)
                    Next
                    Return (Weight * somaquestoes / Me.Count)
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Pontos de bonificação que serão somados a média final da avaliação
        ''' </summary>
        ''' <returns></returns>
        Public Property Bonus As Decimal = 0

        ''' <summary>
        ''' Nota final da avaliação (Bonus + Média)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FinalNote As Decimal
            Get
                Return Average + Bonus
            End Get
        End Property

        ''' <summary>
        ''' Porcentagem de Acertos do Usuário
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HitPercent As Integer
            Get
                Return Me.Average.CalculatePercent(Weight)
            End Get
        End Property

        ''' <summary>
        ''' Porcentagem de Erros do Usuário
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FailPercent As Integer
            Get
                Return (Me.Weight - Me.Average).CalculatePercent(Weight)
            End Get
        End Property

        ''' <summary>
        ''' Instancia uma nova avaliação com titulo
        ''' </summary>
        ''' <param name="Title">Titulo da avaliação</param>
        Public Sub New(Optional Title As String = "New Test")
            Me.Title = Title
        End Sub

        ''' <summary>
        ''' Configura o valor minimo permitido para aprovação como metade do peso da avaliação
        ''' </summary>
        ''' <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        Public Sub SetMinimumAllowedAsHalf(Optional Weight As Decimal = 0)
            If Weight < 1 Or Weight = Nothing Then
                Weight = Me.Weight
            End If
            Me.Weight = Weight
            Me.MinimumWeightAllowed = Me.Weight / 2
        End Sub

        ''' <summary>
        ''' Configura o valor minimo permitido para aprovação como n% do peso da avaliação
        ''' </summary>
        ''' <param name="Percent">Porcentagem da prova</param>
        ''' <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        Public Sub SetMinimumAllowedAsPercent(Percent As String, Optional Weight As Decimal = 0)
            If Weight < 1 Or Weight = Nothing Then
                Weight = Me.Weight
            End If
            Me.Weight = Weight
            Me.MinimumWeightAllowed = CalculateValueFromPercent(Percent, Me.Weight)
        End Sub

        ''' <summary>
        ''' Monta uma prova HTML
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        ReadOnly Property HTML As HtmlDocument
            Get
                Dim head = ""
                head &= Title.WrapInTag("title").ToString.WrapInTag("head").ToString

                Dim body = (Title.WrapInTag("h1").ToString.Append(Header.WrapInTag("header").ToString))

                body &= Me.Select(Function(q) q.HTML).ToArray.Join("").WrapInTag("ol").ToString.WrapInTag("article").Class.Add("Questions").ToString

                Dim foot = Footer.WrapInTag("footer").ToString
                Return New HtmlDocument(head & body & foot)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Classe Base para as questões de uma avaliação
    ''' </summary>
    Public MustInherit Class Question

        ''' <summary>
        ''' Tipo da QUestão
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property QuestionType As String
            Get
                Return Me.GetType.Name
            End Get
        End Property

        ''' <summary>
        ''' Teste a qual esta questão pertence
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public ReadOnly Property Test As QuestionTest
            Get
                Return _test
            End Get
        End Property

        Friend _test As New QuestionTest

        ''' <summary>
        ''' O codigo de identificação desta questão
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ID As String
            Get
                Return Test.IndexOf(Me).Increment.ToString.Prepend("Q")
            End Get
        End Property

        ''' <summary>
        ''' Enunciado da questão (texto da pergunta)
        ''' </summary>
        ''' <returns></returns>
        Public Property Statement As QuestionStatement
            Get
                Return _statement
            End Get
            Set(value As QuestionStatement)
                _statement = value
                value._question = Me
            End Set
        End Property

        Friend _statement As New QuestionStatement With {._question = Me}

        ''' <summary>
        ''' Peso da Pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Property Weight As Decimal
            Get
                Return _weight
            End Get
            Set(value As Decimal)
                _weight = value.SetMaxValue(Test.Weight - Test.Where(Function(q) q IsNot Me).Sum(Function(q) q.Weight))
            End Set
        End Property

        Friend _weight As Decimal = 1

        ''' <summary>
        ''' Retorna um numero que representa o quanto o usuario acertou essa pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property Hits As Decimal = 0

        ''' <summary>
        ''' Verifica se a pergunta está corretamente assinalada
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property IsCorrect As Boolean

        ''' <summary>
        ''' Return the statment text for this question
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Number.ToString.Append(") ").Append(Statement.Text)
        End Function

        ''' <summary>
        ''' Numero da questão
        ''' </summary>
        ''' <returns></returns>
        Public Property Number As Integer
            Get
                If Test IsNot Nothing Then
                    Return Me.Test.IndexOf(Me).Increment
                End If
                Return -1
            End Get
            Set(value As Integer)
                If Test IsNot Nothing Then
                    Me.Test.Move(Me.Test.IndexOf(Me), value.Decrement.LimitRange(0, Test.Count - 1))
                End If
            End Set
        End Property

        <ScriptIgnore>
        MustOverride ReadOnly Property HTML As String

        ''' <summary>
        ''' Indica se esta questão foi revisada pelo professor
        ''' </summary>
        ''' <returns></returns>
        Property Reviewed As Boolean

    End Class

    ''' <summary>
    ''' Imagens adicionada a um enuncidado
    ''' </summary>
    Public Class StatementImages
        Inherits List(Of StatementImage)

        <ScriptIgnore>
        ReadOnly Property Statement As QuestionStatement

        Friend Sub New(Statement As QuestionStatement)
            Me.Statement = Statement
        End Sub

        Shadows Sub Add(Image As Image, Optional Subtitle As String = "")
            Dim i As New StatementImage(Me)
            i.Image = Image
            i.Subtitle = Subtitle
        End Sub

        Shadows Sub Add(ImagePath As String, Optional Subtitle As String = "")
            Dim i As New StatementImage(Me)
            i.Image = File.ReadAllBytes(ImagePath).ToImage
            i.Subtitle = Subtitle
        End Sub

        <ScriptIgnore>
        ReadOnly Property HTML As String
            Get
                Return Me.Select(Function(i) i.HTML).ToArray().Join("").WrapInTag("ul").ToString
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.Count
        End Function

    End Class

    ''' <summary>
    ''' Enunciado de uma pergunta
    ''' </summary>
    <TypeConverter(GetType(ExpandableObjectConverter))>
    Public Class QuestionStatement

        <ScriptIgnore>
        ReadOnly Property Question As Question
            Get
                Return _question
            End Get
        End Property

        Friend _question As Question

        Friend _text As String = ""

        ''' <summary>
        ''' Texto do enunciado
        ''' </summary>
        ''' <returns></returns>
        Public Property Text As String
            Get
                Return _text.FixText
            End Get
            Set(value As String)
                _text = value.FixText
            End Set
        End Property

        ''' <summary>
        ''' Imagens adicionadas ao enunciado (com legenda)
        ''' </summary>
        ''' <returns></returns>
        Public Property Images As New StatementImages(Me)

        Public Overrides Function ToString() As String
            Return Me.Text
        End Function

        <ScriptIgnore>
        ReadOnly Property HTML As String
            Get
                HTML = ""
                If Question IsNot Nothing Then
                    HTML.Append(Text.WrapInTag("h3").ToString)
                    If Images.Count > 0 Then
                        HTML.Append(Images.HTML)
                    End If
                    HTML = HTML.WrapInTag("label").Class.Add("Statement").Attributes.Add("for", Me.Question.ID).ToString
                End If
                Return HTML
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Imagem com legenda de um enunciado
    ''' </summary>
    Public Class StatementImage

        ''' <summary>
        ''' Imagem do enunciado
        ''' </summary>
        ''' <returns></returns>
        Public Property Image As Image

        ''' <summary>
        ''' Legenda da Imagem
        ''' </summary>
        ''' <returns></returns>
        Public Property Subtitle As String = ""

        Friend Sub New(l As StatementImages)
            Me.StatementImages = l
        End Sub

        <ScriptIgnore>
        ReadOnly Property StatementImages As StatementImages

        <ScriptIgnore>
        Public ReadOnly Property HTML As String
            Get
                If StatementImages IsNot Nothing Then
                    Return "<li class='Image'><img src=" & Image.ToDataURL.Quote & " alt= " & Subtitle.Quote & "/><small>Imagem " & StatementImages.IndexOf(Me).Increment.ToString & ": " & Subtitle & "</small></li>"
                End If
                Return ""
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Questões em que a resposta é numerica e implica diretamente no peso da questão (normalmente utilizada em pesquisas)
    ''' </summary>
    Public Class NumericQuestion
        Inherits Question

        ''' <summary>
        ''' Pontos que o usuario atribuiu a esta questão
        ''' </summary>
        ''' <returns></returns>
        Public Property Answer As Decimal
            Get
                Return a
            End Get
            Set(Value As Decimal)
                a = Value.LimitRange(MinValue, MaxValue)
            End Set
        End Property

        Private a As Decimal = 0

        ''' <summary>
        ''' Menor valor permitido pela questão
        ''' </summary>
        ''' <returns></returns>
        Public Property MinValue As Decimal = 1

        ''' <summary>
        ''' Maior valor permitido pela questão
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxValue As Decimal = 10

        ''' <summary>
        ''' Pontos multiplicados pelo peso da questão
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property Hits As Decimal
            Get
                Return Answer * Weight
            End Get
        End Property

        ''' <summary>
        ''' Perguntas numericas sempre estão corretas. Neste caso, o que vale é a resposta multiplicada pelo peso que implica diretamente no peso da avaliação
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property IsCorrect As Boolean
            Get
                Return True
            End Get
        End Property

        <ScriptIgnore>
        Public Overrides ReadOnly Property HTML As String
            Get
                If Test IsNot Nothing Then
                    Dim mElement = New HtmlInput(HtmlInput.HtmlInputType.number)
                    mElement.Value = Me.Answer
                    mElement.ID = ID
                    mElement.Attribute("name") = ID
                    mElement.Attribute("min") = MinValue
                    mElement.Attribute("max") = MaxValue
                    mElement.Class.Add("Numeric")
                    Return (Statement.HTML & mElement.ToString.WrapInTag("div").ToString).WrapInTag("li").Class.Add("Question").ToString
                End If
                Return ""
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Questão Dissertativa. Deve ser corrigida manualmente
    ''' </summary>
    Public Class DissertativeQuestion
        Inherits Question

        <ScriptIgnore>
        Overrides ReadOnly Property HTML As String
            Get
                If Test IsNot Nothing Then
                    Dim mElement = New HtmlElement("textarea")
                    mElement.Attribute("rows") = Lines
                    mElement.ID = ID
                    mElement.Attribute("name") = ID
                    mElement.Class.Add("Dissertative")
                    mElement.IsExplicitlyTerminated = True
                    Return (Statement.HTML & mElement.ToString.WrapInTag("div").ToString).WrapInTag("li").Class.Add("Question").ToString
                End If
                Return ""
            End Get
        End Property

        ''' <summary>
        ''' Resposta dissertativa da pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Property Answer As String

        ''' <summary>
        ''' Valor que indica se a questão está de alguma forma correta
        ''' </summary>
        ''' <returns></returns>
        Public Property Correct As Boolean
            Get
                Return Me.Assertiveness > 0
            End Get
            Set(value As Boolean)
                Me.Assertiveness = If(value, Me.Weight, 0)
            End Set
        End Property

        ''' <summary>
        ''' Numero de linhas que devem ser impressas para esta questão
        ''' </summary>
        ''' <returns></returns>
        Public Property Lines As Integer = 3

        ''' <summary>
        ''' Verifica se a pergunta está preenchida
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property IsCorrect As Boolean
            Get
                Return Correct
            End Get
        End Property

        ''' <summary>
        ''' Representa quantos pontos essa questão vale de acordo com a assertividade
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property Hits As Decimal
            Get
                Return Me.Assertiveness
            End Get
        End Property

        ''' <summary>
        ''' Assertividade da questão, uma valor entre 0 e o peso da questão que representa o quanto esta questão está correta
        ''' </summary>
        ''' <returns></returns>
        Public Property Assertiveness As Decimal
            Set(value As Decimal)
                ass = value.LimitRange(0, Me.Weight)
            End Set
            Get
                Return ass.LimitRange(0, Me.Weight)
            End Get
        End Property

        Private ass As Decimal
    End Class

    ''' <summary>
    ''' Classe base para questões de 'alternativa' ou de 'verdadeiro ou falso'
    ''' </summary>
    Public MustInherit Class AlternativeQuestion
        Inherits Question

        ''' <summary>
        ''' Lista de alternativas da questão
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Alternatives As New AlternativeList(Me)

        ''' <summary>
        ''' Indica se esta alternativa deve ser renderizada no HTML como um <see cref="HtmlSelectElement"/>. Caso Contrario, serão renderizadas como listas de Check Box ou Radio Button
        ''' </summary>
        ''' <returns></returns>
        Public Property RenderAsSelect As Boolean = False

        ''' <summary>
        ''' Verifica se esta pergunta permite multiplas alternativas
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AllowMultiple As Boolean
            Get
                Return Me.GetType Is GetType(MultipleAlternativeQuestion)
            End Get
        End Property

        ''' <summary>
        ''' Retorna as alternativas marcadas pelo usuário
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Answer As IEnumerable(Of Alternative)
            Get
                Return Alternatives.Where(Function(p) p.Checked)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Pergunta de alternativa. o Usuário deverá assinalar a UNICA alternativa correta entre varias alternativas
    ''' </summary>
    Public Class SingleAlternativeQuestion
        Inherits AlternativeQuestion

        ''' <summary>
        ''' Retorna um numero que representa o quanto o usuario acertou essa pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property Hits As Decimal
            Get
                Return If(IsCorrect, Me.Weight, 0)
            End Get
        End Property

        ''' <summary>
        ''' Verifica se a pergunta está corretamente assinalada. Anula a questão automaticamente se estiver mal formada (com mais de uma alternativa correta ou nenhuma alternativa correta)
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property IsCorrect As Boolean
            Get
                If IsValidQuestion() Then
                    Dim total = Alternatives.Count
                    For Each q In Alternatives
                        If Not q.IsCorrect Then
                            Return False
                        End If
                    Next
                End If
                Return True
            End Get
        End Property

        <ScriptIgnore>
        Public Overrides ReadOnly Property HTML As String
            Get
                If Test IsNot Nothing Then
                    Return (Statement.HTML & Alternatives.HTML).WrapInTag("li").Class.Add("Question").ToString
                End If
                Return ""
            End Get
        End Property

        ''' <summary>
        ''' Verifica se as existe apenas uma unica alternativa correta na questão
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsValidQuestion As Boolean
            Get
                Dim c = 0
                For Each q In Me.Alternatives
                    If q.Correct Then
                        c.Increment
                    End If
                Next
                Return c = 1
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Pergunta de Verdadeiro ou Falso. O Usuário deverá assinalar as questões verdadeiras ou falsas correspondente ao enunciado da pergunta.
    ''' </summary>
    Public Class MultipleAlternativeQuestion
        Inherits AlternativeQuestion

        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Retorna um numero que representa o quanto o usuario acertou essa pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property Hits As Decimal
            Get
                Dim total = Alternatives.Count
                If total > 0 Then
                    Dim acertos = 0
                    For Each q In Alternatives
                        If q.IsCorrect Then
                            acertos.Increment
                        End If
                    Next
                    Return acertos * Weight / total
                End If
                Return 0
            End Get

        End Property

        ''' <summary>
        ''' Verifica se a pergunta está corretamente assinalada
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property IsCorrect As Boolean
            Get
                Return Hits > 0
            End Get
        End Property

        <ScriptIgnore>
        Public Overrides ReadOnly Property HTML As String
            Get
                If Test IsNot Nothing Then
                    Return (Statement.HTML & Alternatives.HTML).WrapInTag("li").Class.Add("Question").ToString
                End If
                Return ""
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Lista de Alternativas de uma questão de alternativas
    ''' </summary>
    Public Class AlternativeList
        Inherits ObservableCollection(Of Alternative)

        Protected NotOverridable Overrides Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
            MyBase.OnCollectionChanged(e)
            If e.Action = NotifyCollectionChangedAction.Add Then
                Debug.WriteLine("Alternative Added")
                For Each ni In e.NewItems
                    Dim i = CType(ni, Alternative)
                    i._question = Me.Question
                    If Not Me.Question.AllowMultiple And i.Correct Then
                        For Each ii In Me.Question.Alternatives
                            If Not ii.Question Is CType(i, Alternative).Question Then
                                ii.Correct = False
                            End If
                        Next
                    End If
                Next
            End If
        End Sub

        <ScriptIgnore>
        ReadOnly Property Question As AlternativeQuestion

        <ScriptIgnore>
        Public ReadOnly Property HTML As String
            Get
                If Question IsNot Nothing Then
                    Return Me.Select(Function(p) p.HTML).ToArray.Join("").WrapInTag(If(Question.RenderAsSelect, "select", "ol")).AddAttribute(If(Question.RenderAsSelect AndAlso Question.AllowMultiple, "multiple", "")).Class.Add("Alternatives").ToString
                End If
                Return ""
            End Get
        End Property

        Friend Sub New(l As AlternativeQuestion)
            Me.Question = l
        End Sub

        ''' <summary>
        ''' Adiciona uma alternativa a questão. A alternativa é ignorada se já existir na lista
        ''' </summary>
        ''' <param name="Text">Texto da alternativa</param>
        ''' <param name="Correct">Parametro que indica se esta alternativa é correta ou verdadeira</param>
        Public Overloads Sub Add(Text As String, Correct As Boolean)
            Dim c As New Alternative() With {.Text = Text, .Correct = Correct}
            Me.Add(c)
        End Sub

        Public Sub AddRange(Alternatives As IEnumerable(Of Alternative))
            For Each alt In Alternatives
                Me.Add(alt)
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Count
        End Function

    End Class

    ''' <summary>
    ''' Objeto que representa uma alternativa de uma pergunta de alternativas
    ''' </summary>
    Public Class Alternative

        Sub New()
            _question = New SingleAlternativeQuestion
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Number.ToRoman.Wrap("(") & Me.Text
        End Function

        ''' <summary>
        ''' ID da alternativa
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ID As String
            Get
                If Question IsNot Nothing Then
                    Return Me.Question.ID.Append("A").Append(Number)
                End If
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' O numero da alternativa
        ''' </summary>
        ''' <returns></returns>
        Public Property Number As Integer
            Get
                If Question IsNot Nothing Then
                    Return Question.Alternatives.IndexOf(Me).Increment
                End If
                Return -1
            End Get
            Set(value As Integer)
                If Question IsNot Nothing Then
                    Question.Alternatives.Move(Question.Alternatives.IndexOf(Me), value.Decrement.LimitRange(0, Question.Alternatives.Count - 1))
                End If
            End Set
        End Property

        ''' <summary>
        ''' Texto da alternativa
        ''' </summary>
        ''' <returns></returns>
        Public Property Text As String

        ''' <summary>
        ''' Valor que indica se a alternativa está correta ou verdadeira
        ''' </summary>
        ''' <returns></returns>
        Public Property Correct As Boolean

        ''' <summary>
        ''' Valor que indica se esta alternativa foi assinalada
        ''' </summary>
        ''' <returns></returns>
        Public Property Checked As Boolean

        ''' <summary>
        ''' Verifica se a resposta do usuário é correta para esta alternativa
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsCorrect As Boolean
            Get
                Return Checked = Correct
            End Get
        End Property

        <ScriptIgnore>
        ReadOnly Property HTML As String
            Get
                If Question IsNot Nothing Then
                    Return "<" & If(_question.RenderAsSelect, "option", "li") & " class='Alternative " & If(Question.GetType Is GetType(SingleAlternativeQuestion), "Single", "Multiple") & "'><input type=" & If(Question.GetType Is GetType(SingleAlternativeQuestion), "radio", "checkbox").ToString.Quote & " ID=" & ID.Quote & " name=" & Question.ID.Quote & "   value=" & ID.Quote & " /><label for=" & ID.Quote & ">" & Text & "</label>"
                End If
                Return ""
            End Get
        End Property

        <ScriptIgnore>
        ReadOnly Property Question As AlternativeQuestion
            Get
                Return _question
            End Get
        End Property

        Friend _question As AlternativeQuestion

    End Class

    Public Class QuestionEditor
        Inherits EnhancedCollectionEditor

        Public Sub New(t As Type)
            MyBase.New(t)

            MyBase.FormCaption = "Question Editor"
            MyBase.ShowPropGridHelp = False
            MyBase.AllowMultipleSelect = False

        End Sub

    End Class

End Namespace