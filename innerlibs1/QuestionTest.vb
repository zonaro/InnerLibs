
Imports System.Drawing
Namespace QuestionTest


    ''' <summary>
    ''' Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem Dissertativas, Multipla Escolha ou de Atribuição de Pontos
    ''' </summary>
    Public Class QuestionTest
        ''' <summary>
        ''' Titulo da Avaliação
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String = ""
        ''' <summary>
        ''' Lista de Questões da avaliação
        ''' </summary>
        ''' <returns></returns>
        Public Property Questions As New QuestionList
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
                For Each q In Questions
                    If q.IsCorrect Then c.Increment
                Next
                Return c
            End Get
        End Property

        ''' <summary>
        ''' Numeor de questões que o usuário errou
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Fails As Integer
            Get
                Return Questions.Count - Hits
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
                    For Each q In Questions
                        somapesos = somapesos + q.Weight
                        somaquestoes = somaquestoes + If(q.IsCorrect, q.Hits, 0)
                    Next
                    Return (Weight * somaquestoes / Questions.Count)
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
        Public Sub New(Optional Title As String = Nothing)
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
        ''' Monta Uma prova HTML
        ''' </summary>
        ''' <returns></returns>
        Public Function BuildHtml() As String
            Dim a As New List(Of String)
            Return BuildHtml(a.ToArray)
        End Function

        ''' <summary>
        ''' Monta uma prova HTML
        ''' </summary>
        ''' <param name="CssClass">Lista de classes CSS que devem ser adiciondas ao elemento que contém toda a prova</param>
        ''' <returns></returns>
        Public Function BuildHtml(ParamArray CssClass() As String) As String
            Dim html = ""
            html.Append("<h2>" & Title & "</h2>")
            html.Append(Header.WrapInTag("header"))
            html.Append("<ol>")
            For Each question In Questions
                html.Append("    <li> ")
                html.Append("        <h3>" & question.Statement.Text & "</h3> ")
                If question.Statement.Images.Count > 0 Then
                    html.Append("        <ul class='StatementImageList'>")
                    Dim imagecount As Integer = 0
                    For Each img In question.Statement.Images
                        html.Append("<li><img src=" & img.Image.ToDataURL.Quote & " alt= " & img.Subtitle.Quote & "/><small>Imagem " & imagecount.Increment.ToString & ": " & img.Subtitle & "</small></li>")
                    Next
                    html.Append("        </ul>")
                End If

                Dim derived As Object = question
                Select Case question.Type
                    Case "SingleAlternativeQuestion", "MultipleAlternativeQuestion"
                        html.Append("<ul> ")
                        For Each a As Alternative In derived.Alternatives
                            Dim asid As String = (question.ID & a.ID).ToString
                            html.Append(WrapInTag("<input type=" & If(question.Type = "SingleAlternativeQuestion", "radio", "checkbox").ToString.Quote & " ID=" & asid.Quote & " name=" & question.ID.Quote & "   value=" & a.ID.Quote & " /><label for=" & asid.Quote & ">" & a.Text & "</label>", "li", "class=" & question.CssClass.Join(" ").Quote))
                        Next
                        html.Append("</ul> ")
                    Case "DissertativeQuestion"
                        html.Append(WrapInTag("<textarea rows=" & derived.Lines & " ID=" & derived.ID.ToString().Quote & " name=" & derived.ID.ToString().Quote & " ></textarea>", "div", "class=" & question.CssClass.Join(" ").Quote))
                    Case "NumericQuestion"
                        html.Append(WrapInTag("<input type='number' min=" & derived.MinValue.ToString.Quote & " max=" & derived.MaxValue.ToString.Quote & " ID=" & question.ID.Quote & " name=" & question.ID.Quote & "   value=" & (derived.MaxValue / 2).ToString.Quote & " />", "div", "class=" & question.CssClass.Join(" ").Quote))
                End Select
            Next
            html.Append("</ol> ")
            html.Append(Footer.WrapInTag("footer"))
            Return html.WrapInTag("article", "class=" & CssClass.Join(" ").Quote)
        End Function




    End Class


    ''' <summary>
    ''' Lista de questões da avaliação
    ''' </summary>
    Public Class QuestionList
        Inherits List(Of Question)

        ''' <summary>
        ''' Adiciona uma questão a avaliação. A questão é ignorada se já existir na lista
        ''' </summary>
        ''' <param name="Question">Questão</param>
        Public Overloads Sub Add(Question As Question)
            If Not MyBase.Contains(Question) Then
                Question.ID = If(Question.ID = Nothing, (Me.Count + 1).ToString.Prepend("Q"), Question.ID)
                MyBase.Add(Question)
            End If
        End Sub


        ''' <summary>
        ''' Adiciona um conjunto de questões a avaliação
        ''' </summary>
        ''' <param name="Questions">Questões</param>
        Public Overloads Sub AddRange(Questions As IEnumerable(Of Question))
            For Each q In Questions
                Me.Add(q)
            Next
        End Sub
    End Class




    ''' <summary>
    ''' Classe Base para as questões de uma avaliação
    ''' </summary>
    Public MustInherit Class Question

        Public Property ID As String = Nothing

        ''' <summary>
        ''' Enunciado da questão (texto da pergunta)
        ''' </summary>
        ''' <returns></returns>
        Public Property Statement As New QuestionStatement

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
        Public Overridable ReadOnly Property IsCorrect As Boolean = False
        ''' <summary>
        ''' Tipo da Pergunta
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Type As String = "Question"

        ''' <summary>
        ''' Classes CSS para a pergunta. Utilizado na construção de provas HTML
        ''' </summary>
        ''' <returns></returns>
        Public Property CssClass As New List(Of String)

        ''' <summary>
        ''' Instancia uma questão
        ''' </summary>
        Public Sub New()
            Type = Me.GetType.Name
            CssClass.Add(Type)
        End Sub
    End Class

    ''' <summary>
    ''' Enunciado de uma pergunta
    ''' </summary>
    Public Class QuestionStatement

        ''' <summary>
        ''' Texto do enunciado
        ''' </summary>
        ''' <returns></returns>
        Public Property Text As String = ""

        ''' <summary>
        ''' Imagens adicionadas ao enunciado (com legenda)
        ''' </summary>
        ''' <returns></returns>
        Public Property Images As New List(Of StatementImages)

        ''' <summary>
        ''' Instancia umm novo enunciado com um texto
        ''' </summary>
        ''' <param name="Text">Texto da pergunta</param>
        Public Sub New(Optional Text As String = "")
            Me.Text = Text
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Text
        End Function

    End Class

    ''' <summary>
    ''' Imagem com legenda de um enunciado
    ''' </summary>
    Public Class StatementImages

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

        Public Sub New(Optional Image As Image = Nothing, Optional Subtitle As String = "")
            Me.Image = Image
            Me.Subtitle = Subtitle
        End Sub
    End Class

    ''' <summary>
    ''' Questões em que a resposta é numerica e implica diretamente no peso da questão (normalmente utilizada em pesquisas)
    ''' </summary>
    Public Class NumericQuestion
        Inherits Question
        ''' <summary>
        ''' Pontos que o usuario fez nessa questão
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

    End Class

    ''' <summary>
    ''' Questão Dissertativa
    ''' </summary>
    Public Class DissertativeQuestion
        Inherits Question

        ''' <summary>
        ''' Resposta dissertativa da pergunta
        ''' </summary>
        ''' <returns></returns>
        Public Property Answer As String
        ''' <summary>
        ''' Valor que indica se a questão está de alguma forma correta
        ''' </summary>
        ''' <returns></returns>
        Public Property Correct As Boolean = False

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
                Correct = (ass > 0)
            End Set
            Get
                Correct = (ass > 0)
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
        Public Property Alternatives As New AlternativeList

    End Class

    ''' <summary>
    ''' Lista de Alternativas de uma questão de alternativas
    ''' </summary>
    Public Class AlternativeList
        Inherits List(Of Alternative)
        ''' <summary>
        ''' Adiciona uma alternativa a questão. A alternativa é ignorada se já existir na lista
        ''' </summary>
        ''' <param name="Alternative">Alternativa</param>
        Public Overloads Sub Add(Alternative As Alternative)
            If Not MyBase.Contains(Alternative) Then
                Alternative.ID = If(Alternative.ID = Nothing, (Me.Count + 1).ToString.Prepend("A"), Alternative.ID)
                MyBase.Add(Alternative)
            End If
        End Sub

        ''' <summary>
        ''' Adiciona uma alternativa a questão. A alternativa é ignorada se já existir na lista
        ''' </summary>
        ''' <param name="Text">Texto da alternativa</param>
        ''' <param name="Correct">Parametro que indica se esta alternativa é correta ou verdadeira</param>
        Public Overloads Sub Add(Text As String, Correct As Boolean)
            Dim c As New Alternative(Text, Correct)
            c.ID = If(c.ID = Nothing, (Me.Count + 1).ToString.Prepend("A"), c.ID)
            If Not MyBase.Contains(c) Then
                MyBase.Add(c)
            End If
        End Sub

        ''' <summary>
        ''' Adiciona um conjunto de alternativas a questão
        ''' </summary>
        ''' <param name="Alternatives">Alternativas</param>
        Public Overloads Sub AddRange(Alternatives As IEnumerable(Of Alternative))
            For Each q In Alternatives
                Me.Add(q)
            Next
        End Sub
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
                Return True
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Objeto que representa uma alternativa de uma pergunta de alternativas
    ''' </summary>
    Public Class Alternative

        ''' <summary>
        ''' ID da alternativa
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String = Nothing

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



        ''' <summary>
        ''' Cria uma instancia de Alternativa
        ''' </summary>
        ''' <param name="Text">Texto da Pergunta</param>
        ''' <param name="Correct">Valor que indica se a alternativa é correta ou verdadeira</param>
        Public Sub New(Text As String, Correct As Boolean)
            Me.Text = Text
            Me.Correct = Correct
        End Sub
    End Class
End Namespace

