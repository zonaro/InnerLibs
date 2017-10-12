Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports InnerLibs.HtmlParser

Namespace QuestionTest

    ''' <summary>
    ''' Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem Dissertativas, Multipla Escolha ou de Atribuição de Pontos
    ''' </summary>
    Public Class QuestionTest
        Inherits List(Of Question)

        ''' <summary>
        ''' Titulo da Avaliação
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String = ""

        ''' <summary>
        ''' Adiciona uma nova questão a avaliação.
        ''' </summary>
        Public Shadows Function Add(Of QuestionType As Question)() As QuestionType
            Dim Question = CType(Activator.CreateInstance(GetType(QuestionType), Me), QuestionType)
            If Not MyBase.Contains(Question) Then
                MyBase.Add(Question)
            End If
            Return Question
        End Function

        ReadOnly Property Questions As QuestionTest
            Get
                Return Me
            End Get
        End Property

        Public Shadows Sub AddRange(Questions As IEnumerable(Of Question))
            Throw New NotSupportedException("Cannot add multiple Questions")
        End Sub

        Public Shadows Function Insert()
            Throw New NotSupportedException("Cannot add multiple Questions")
        End Function

        Public Shadows Function InsertAt()
            Throw New NotSupportedException("Cannot add multiple Questions")
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
        ReadOnly Property HTML As HtmlDocument
            Get
                Dim head = ""
                head &= Title.WrapInTag("title").WrapInTag("head")

                Dim body = (Title.WrapInTag("h1").Append(Header.WrapInTag("header")))

                body &= Me.Select(Function(q) q.HTML).ToArray.Join("").WrapInTag("ol").WrapInTag("article", "class='Questions'")

                Dim foot = Footer.WrapInTag("footer")
                Return New HtmlDocument(head & body & foot)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Classe Base para as questões de uma avaliação
    ''' </summary>
    Public MustInherit Class Question

        Friend Sub New(Test As QuestionTest)
            Me.Test = Test
        End Sub

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
        Public ReadOnly Property Test As QuestionTest

        Public ReadOnly Property ID As String
            Get
                Return Test.IndexOf(Me).Increment.ToString.Prepend("Q")
            End Get
        End Property

        ''' <summary>
        ''' Enunciado da questão (texto da pergunta)
        ''' </summary>
        ''' <returns></returns>
        Public Property Statement As New QuestionStatement(Me)

        ''' <summary>
        ''' Peso da Pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Property Weight As Decimal = 1

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
            Return Me.Statement.Text
        End Function

        MustOverride ReadOnly Property HTML As String

    End Class

    Public Class StatementImages
        Inherits List(Of StatementImage)

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

        ReadOnly Property HTML As String
            Get
                Return Me.Select(Function(i) i.HTML).ToArray().Join("").WrapInTag("ul")
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Enunciado de uma pergunta
    ''' </summary>
    Public Class QuestionStatement

        ReadOnly Property Question As Question

        Friend Sub New(Question As Question)
            Me.Question = Question
        End Sub

        ''' <summary>
        ''' Texto do enunciado
        ''' </summary>
        ''' <returns></returns>
        Public Property Text As String = ""

        ''' <summary>
        ''' Imagens adicionadas ao enunciado (com legenda)
        ''' </summary>
        ''' <returns></returns>
        Public Property Images As New StatementImages(Me)

        Public Overrides Function ToString() As String
            Return Me.Text
        End Function

        ReadOnly Property HTML As String
            Get
                HTML = ""
                HTML.Append(Text.WrapInTag("h3"))
                If Images.Count > 0 Then
                    HTML.Append(Images.HTML)
                End If
                Return HTML.WrapInTag("label", "for='" & Me.Question.ID & "' class='Statement'")
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

        ReadOnly Property StatementImages As StatementImages

        Public ReadOnly Property HTML As String
            Get
                Return "<li class='Image'><img src=" & Image.ToDataURL.Quote & " alt= " & Subtitle.Quote & "/><small>Imagem " & StatementImages.IndexOf(Me).Increment.ToString & ": " & Subtitle & "</small></li>"
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

        Public Sub New(Test As QuestionTest)
            MyBase.New(Test)
        End Sub

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

        Public Overrides ReadOnly Property HTML As String
            Get
                Dim mElement = New HtmlInput(HtmlInput.HtmlInputType.number)
                mElement.Value = Me.Answer
                mElement.ID = ID
                mElement.Attribute("name") = ID
                mElement.Attribute("min") = MinValue
                mElement.Attribute("max") = MaxValue
                mElement.Class.Add("Numeric")
                Return (Statement.HTML & mElement.ToString.WrapInTag("div")).WrapInTag("li", "class='Question'")
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Questão Dissertativa. Deve ser corrigida manualmente
    ''' </summary>
    Public Class DissertativeQuestion
        Inherits Question

        Public Sub New(Test As QuestionTest)
            MyBase.New(Test)
        End Sub

        Overrides ReadOnly Property HTML As String
            Get
                Dim mElement = New HtmlElement("textarea")
                mElement.Attribute("rows") = Lines
                mElement.ID = ID
                mElement.Attribute("name") = ID
                mElement.Class.Add("Dissertative")
                mElement.IsExplicitlyTerminated = True
                Return (Statement.HTML & mElement.ToString.WrapInTag("div")).WrapInTag("li", "class='Question'")
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

        Friend Sub New(Test As QuestionTest)
            MyBase.New(Test)
        End Sub

        ''' <summary>
        ''' Lista de alternativas da questão
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Alternatives As New AlternativeList(Me)

        ''' <summary>
        ''' Indica se esta alternativa deve ser renderizada no HTML como um <see cref="HtmlSelectElement"/>. Caso Contrario, serão renderizadas como Check Box ou Radio Button
        ''' </summary>
        ''' <returns></returns>
        Public Property RenderAsSelect As Boolean = False

    End Class

    ''' <summary>
    ''' Pergunta de alternativa. o Usuário deverá assinalar a UNICA alternativa correta entre varias alternativas
    ''' </summary>
    Public Class SingleAlternativeQuestion
        Inherits AlternativeQuestion

        Public Sub New(Test As QuestionTest)
            MyBase.New(Test)
        End Sub

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
                If ValidateAlternatives() Then
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

        Public Overrides ReadOnly Property HTML As String
            Get
                Return (Statement.HTML & Alternatives.HTML).WrapInTag("li", "class='Question'")
            End Get
        End Property

        ''' <summary>
        ''' Verifica se as existe apenas uma unica alternativa correta na questão
        ''' </summary>
        ''' <returns></returns>
        Public Function ValidateAlternatives() As Boolean
            Dim c = 0
            For Each q In Me.Alternatives
                If q.Correct Then
                    c.Increment
                End If
            Next
            Return c = 1
        End Function

    End Class

    ''' <summary>
    ''' Pergunta de Verdadeiro ou Falso. O Usuário deverá assinalar as questões verdadeiras ou falsas correspondente ao enunciado da pergunta.
    ''' </summary>
    Public Class MultipleAlternativeQuestion
        Inherits AlternativeQuestion

        Public Sub New(Test As QuestionTest)
            MyBase.New(Test)
        End Sub

        ''' <summary>
        ''' Retorna um numero que representa o quanto o usuario acertou essa pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property Hits As Decimal
            Get
                Dim total = Alternatives.Count
                Dim acertos = 0
                For Each q In Alternatives
                    If q.IsCorrect Then
                        acertos.Increment
                    End If
                Next
                Return acertos * Weight / total
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

        Public Overrides ReadOnly Property HTML As String
            Get
                Return (Statement.HTML & Alternatives.HTML).WrapInTag("li", "class='Question'")
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Lista de Alternativas de uma questão de alternativas
    ''' </summary>
    Public Class AlternativeList
        Inherits List(Of Alternative)

        ReadOnly Property Question As AlternativeQuestion

        Public ReadOnly Property HTML As String
            Get
                Return Me.Select(Function(p) p.HTML).ToArray.Join("").WrapInTag("ol", "class='Alternatives'")
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
        Public Shadows Sub Add(Text As String, Correct As Boolean)
            Dim c As New Alternative(Me.Question) With {.Text = Text}
            If Not MyBase.Contains(c) Then
                If TypeOf Me.Question Is SingleAlternativeQuestion And Correct Then
                    Me.Question.Alternatives.ForEach(Sub(i) i.Correct = False)
                End If
                c.Correct = Correct
                MyBase.Add(c)
            End If
        End Sub

        Public Shadows Sub AddRange(Alternatives As IEnumerable(Of Alternative))
            Throw New NotSupportedException("Cannot add multiple alternatives at same time")
        End Sub

        Public Shadows Sub Insert()
            Throw New NotSupportedException("Cannot insert alternatives")
        End Sub

        Public Shadows Sub InsertAt()
            Throw New NotSupportedException("Cannot insert alternatives")
        End Sub

    End Class

    ''' <summary>
    ''' Objeto que representa uma alternativa de uma pergunta de alternativas
    ''' </summary>
    Public Class Alternative

        ''' <summary>
        ''' ID da alternativa
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ID As String
            Get
                Return Me.Question.ID.Append("A").Append(Question.Alternatives.IndexOf(Me).Increment)
            End Get
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

        ReadOnly Property HTML As String
            Get
                Return "<li class='Alternative " & If(Question.GetType Is GetType(SingleAlternativeQuestion), "Single", "Multiple") & "'><input type=" & If(Question.GetType Is GetType(SingleAlternativeQuestion), "radio", "checkbox").ToString.Quote & " ID=" & ID.Quote & " name=" & Question.ID.Quote & "   value=" & ID.Quote & " /><label for=" & ID.Quote & ">" & Text & "</label></li>"
            End Get
        End Property

        Property Question As AlternativeQuestion

        Friend Sub New(Question As AlternativeQuestion)
            Me.Question = Question
        End Sub

    End Class

End Namespace