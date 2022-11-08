using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace InnerLibs.QuestionTest
{
    /// <summary>
    /// Objeto que representa uma alternativa de uma pergunta de alternativas
    /// </summary>
    public class Alternative
    {
        internal AlternativeQuestion _question;

        public Alternative()
        {
            _question = new SingleAlternativeQuestion();
        }

        /// <summary>
        /// Valor que indica se esta alternativa foi assinalada
        /// </summary>
        /// <returns></returns>
        public bool Checked { get; set; }

        /// <summary>
        /// Valor que indica se a alternativa está correta ou verdadeira
        /// </summary>
        /// <returns></returns>
        public bool Correct { get; set; }

        /// <summary>
        /// ID da alternativa
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                if (Question != null)
                {
                    return Question.ID + "A" + Number;
                }

                return null;
            }
        }

        /// <summary>
        /// Verifica se a resposta do usuário é correta para esta alternativa
        /// </summary>
        /// <returns></returns>
        public bool IsCorrect => Checked == Correct;

        /// <summary>
        /// O numero da alternativa
        /// </summary>
        /// <returns></returns>
        public int Number
        {
            get
            {
                if (Question != null)
                {
                    return Question.Alternatives.IndexOf(this) + 1;
                }

                return -1;
            }

            set
            {
                if (Question != null)
                {
                    Question.Alternatives.Move(Question.Alternatives.IndexOf(this), (value - 1).LimitRange(0, Question.Alternatives.Count - 1));
                }
            }
        }

        public AlternativeQuestion Question
        {
            get
            {
                return _question;
            }
        }

        /// <summary>
        /// Texto da alternativa
        /// </summary>
        /// <returns></returns>
        public string Text { get; set; }

        public override string ToString() => Number.ToRoman().Quote('(') + Text;
    }

    /// <summary>
    /// Lista de Alternativas de uma questão de alternativas
    /// </summary>
    public class AlternativeList : ObservableCollection<Alternative>
    {
        protected override sealed void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Debug.WriteLine("Alternative Added");
                foreach (var ni in e.NewItems)
                {
                    Alternative i = (Alternative)ni;
                    i._question = Question;
                    if (!Question.AllowMultiple & i.Correct)
                    {
                        foreach (var ii in Question.Alternatives)
                        {
                            if (!ReferenceEquals(ii.Question, i.Question))
                            {
                                ii.Correct = false;
                            }
                        }
                    }
                }
            }
        }

        internal AlternativeList(AlternativeQuestion l)
        {
            Question = l;
        }

        public AlternativeQuestion Question { get; private set; }

        /// <summary>
        /// Adiciona uma alternativa a questão. A alternativa é ignorada se já existir na lista
        /// </summary>
        /// <param name="Text">Texto da alternativa</param>
        /// <param name="Correct">Parametro que indica se esta alternativa é correta ou verdadeira</param>
        public void Add(string Text, bool Correct)
        {
            var c = new Alternative() { Text = Text, Correct = Correct };
            Add(c);
        }

        public void AddRange(IEnumerable<Alternative> Alternatives)
        {
            foreach (var alt in Alternatives)
                Add(alt);
        }

        public override string ToString() => Count.ToString();
    }

    /// <summary>
    /// Classe base para questões de 'alternativa' ou de 'verdadeiro ou falso'
    /// </summary>
    public abstract class AlternativeQuestion : Question
    {
        internal AlternativeQuestion() : base()
        {
            Alternatives = new AlternativeList(this);
        }

        /// <summary>
        /// Verifica se esta pergunta permite multiplas alternativas
        /// </summary>
        /// <returns></returns>
        public bool AllowMultiple => this.GetType() == typeof(MultipleAlternativeQuestion);

        /// <summary>
        /// Lista de alternativas da questão
        /// </summary>
        /// <returns></returns>
        public AlternativeList Alternatives { get; private set; }

        /// <summary>
        /// Retorna as alternativas marcadas pelo usuário
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Alternative> Answer => Alternatives.Where(p => p.Checked);

        /// <summary>
        /// Indica se esta alternativa deve ser renderizada no HTML como um <see
        /// cref="HtmlSelectElement"/>. Caso Contrario, serão renderizadas como listas de Check Box
        /// ou Radio Button
        /// </summary>
        /// <returns></returns>
        public bool RenderAsSelect { get; set; } = false;
    }

    /// <summary>
    /// Questão Dissertativa. Deve ser corrigida manualmente
    /// </summary>
    public class DissertativeQuestion : Question
    {
        private decimal ass;

        /// <summary>
        /// Resposta dissertativa da pergunta
        /// </summary>
        /// <returns></returns>
        public string Answer { get; set; }

        /// <summary>
        /// Assertividade da questão, uma valor entre 0 e o peso da questão que representa o quanto
        /// esta questão está correta
        /// </summary>
        /// <returns></returns>
        public decimal Assertiveness
        {
            set => ass = value.LimitRange(0, Weight);

            get => ass.LimitRange(0, Weight);
        }

        /// <summary>
        /// Valor que indica se a questão está de alguma forma correta
        /// </summary>
        /// <returns></returns>
        public bool Correct
        {
            get => Assertiveness > 0m;

            set => Assertiveness = value ? Weight : 0m;
        }

        /// <summary>
        /// Representa quantos pontos essa questão vale de acordo com a assertividade
        /// </summary>
        /// <returns></returns>
        public override decimal Hits => Assertiveness;

        /// <summary>
        /// Verifica se a pergunta está preenchida
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect => Correct;

        /// <summary>
        /// Numero de linhas que devem ser impressas para esta questão
        /// </summary>
        /// <returns></returns>
        public int Lines { get; set; } = 3;
    }

    /// <summary>
    /// Pergunta de Verdadeiro ou Falso. O Usuário deverá assinalar as questões verdadeiras ou
    /// falsas correspondente ao enunciado da pergunta.
    /// </summary>
    public class MultipleAlternativeQuestion : AlternativeQuestion
    {
        public MultipleAlternativeQuestion() : base()
        {
        }

        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public override decimal Hits
        {
            get
            {
                int total = Alternatives.Count;
                if (total > 0)
                {
                    int acertos = 0;
                    foreach (var q in Alternatives)
                    {
                        if (q.IsCorrect)
                        {
                            acertos = acertos + 1;
                        }
                    }

                    return acertos * Weight / total;
                }

                return 0m;
            }
        }

        /// <summary>
        /// Verifica se a pergunta está corretamente assinalada
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect => Hits > 0m;
    }

    /// <summary>
    /// Questões em que a resposta é numerica e implica diretamente no peso da questão (normalmente
    /// utilizada em pesquisas)
    /// </summary>
    public class NumericQuestion : Question
    {
        private decimal a = 0m;

        /// <summary>
        /// Pontos que o usuario atribuiu a esta questão
        /// </summary>
        /// <returns></returns>
        public decimal Answer
        {
            get
            {
                return a;
            }

            set
            {
                a = value.LimitRange(MinValue, MaxValue);
            }
        }

        /// <summary>
        /// Pontos multiplicados pelo peso da questão
        /// </summary>
        /// <returns></returns>
        public override decimal Hits => Answer * Weight;

        /// <summary>
        /// Perguntas numericas sempre estão corretas. Neste caso, o que vale é a resposta
        /// multiplicada pelo peso que implica diretamente no peso da avaliação
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect => true;

        /// <summary>
        /// Maior valor permitido pela questão
        /// </summary>
        /// <returns></returns>
        public decimal MaxValue { get; set; } = 10m;

        /// <summary>
        /// Menor valor permitido pela questão
        /// </summary>
        /// <returns></returns>
        public decimal MinValue { get; set; } = 1m;
    }

    /// <summary>
    /// Classe Base para as questões de uma avaliação
    /// </summary>
    public abstract class Question
    {
        internal QuestionStatement _statement;

        internal QuestionTest _test = new QuestionTest();

        internal decimal _weight = 1m;

        public Question() => _statement = new QuestionStatement() { _question = this };

        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public virtual decimal Hits { get; private set; } = 0m;

        /// <summary>
        /// O codigo de identificação desta questão
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                if (Test != null)
                {
                    return (Test.IndexOf(this) + 1).ToString().Prepend("Q");
                }

                return null;
            }
        }

        /// <summary>
        /// Verifica se a pergunta está corretamente assinalada
        /// </summary>
        /// <returns></returns>
        public abstract bool IsCorrect { get; }

        /// <summary>
        /// Numero da questão
        /// </summary>
        /// <returns></returns>
        public int Number
        {
            get
            {
                if (Test != null)
                {
                    return Test.IndexOf(this) + 1;
                }

                return -1;
            }

            set
            {
                if (Test != null)
                {
                    Test.Move(Test.IndexOf(this), (value - 1).LimitRange(0, Test.Count - 1));
                }
            }
        }

        /// <summary>
        /// Tipo da Questão
        /// </summary>
        /// <returns></returns>
        public string QuestionType => GetType().Name;

        /// <summary>
        /// Teste a qual esta questão pertence
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Indica se esta questão foi revisada pelo professor
        /// </summary>
        /// <returns></returns>
        public bool Reviewed { get; set; } = false;

        /// <summary>
        /// Enunciado da questão (texto da pergunta)
        /// </summary>
        /// <returns></returns>
        public QuestionStatement Statement
        {
            get
            {
                return _statement;
            }

            set
            {
                _statement = value;
                value._question = this;
            }
        }

        public QuestionTest Test => _test;

        /// <summary>
        /// Peso da Pergunta
        /// </summary>
        /// <returns></returns>
        public decimal Weight
        {
            get => _weight;

            set => _weight = value.SetMaxValue(Test.Weight - Test.Where(q => !ReferenceEquals(q, this)).Sum(q => q.Weight));
        }

        /// <summary>
        /// Return the statment text for this question
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Number.ToString() + ") " + Statement.Text;
    }

    /// <summary>
    /// Enunciado de uma pergunta
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class QuestionStatement
    {
        internal Question _question;

        internal string _text = InnerLibs.Text.Empty;

        internal QuestionStatement() => this.Images = new StatementImages(this);

        /// <summary>
        /// Imagens adicionadas ao enunciado (com legenda)
        /// </summary>
        /// <returns></returns>
        public StatementImages Images { get; set; }

        public Question Question => _question;

        /// <summary>
        /// Texto do enunciado
        /// </summary>
        /// <returns></returns>
        public string Text
        {
            get => _text.FixText();

            set => _text = value.FixText();
        }

        public override string ToString() => Text;
    }

    /// <summary>
    /// Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem
    /// Dissertativas, Multipla Escolha ou de Atribuição de Pontos
    /// </summary>
    [Serializable]
    [Category("Avaliação")]
    [Description("Representa uma avaliação de perguntas e respostas")]
    public class QuestionTest : ObservableCollection<Question>
    {
        private string _title = InnerLibs.Text.Empty;

        protected override sealed void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        Debug.WriteLine("Question Added");
                        foreach (var ni in e.NewItems)
                        {
                            Question q = (Question)ni;
                            q._test = this;
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        Debug.WriteLine("Question Removed");
                        foreach (var ni in e.OldItems)
                        {
                            Question q = (Question)ni;
                            q._test = null;
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        Debug.WriteLine("Question Replaced");
                        foreach (var ni in e.NewItems)
                        {
                            Question q = (Question)ni;
                            q._test = this;
                        }

                        foreach (var ni in e.OldItems)
                        {
                            Question q = (Question)ni;
                            q._test = null;
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Instancia uma nova avaliação com titulo
        /// </summary>
        /// <param name="Title">Titulo da avaliação</param>
        public QuestionTest(string Title = "New Test") => this.Title = Title;

        /// <summary>
        /// Média da Avaliação
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Média da Avaliação")]
        public decimal Average
        {
            get
            {
                try
                {
                    int somapesos = 0;
                    int somaquestoes = 0;
                    foreach (var q in this)
                    {
                        somapesos = (int)Math.Round(somapesos + q.Weight);
                        somaquestoes = (int)Math.Round(somaquestoes + (q.IsCorrect ? q.Hits : 0m));
                    }

                    return Weight * somaquestoes / Count;
                }
                catch
                {
                    return 0m;
                }
            }
        }

        /// <summary>
        /// Pontos de bonificação que serão somados a média final da avaliação
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Pontos de bonificação que serão somados a média final da avaliação")]
        public decimal Bonus { get; set; } = 0m;

        /// <summary>
        /// Porcentagem de Erros do Usuário
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Porcentagem de erros da avaliação")]
        public decimal FailPercent => Math.Round((Weight - Average).CalculatePercent(Weight));

        /// <summary>
        /// Numero de questões que o usuário errou
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Numero de questões que o usuário errou")]
        public int Fails => Count - Hits;

        /// <summary>
        /// Nota final da avaliação (Bonus + Média)
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Nota final da avaliação (Bonus + Média)")]
        public decimal FinalNote => Average + Bonus;

        /// <summary>
        /// Rodapé da prova. Texto adicional que ficará após as questões
        /// </summary>
        /// <returns></returns>
        [Category("Texto")]
        [Description("Rodapé da prova. Texto adicional que ficará após as questões")]
        public string Footer { get; set; } = InnerLibs.Text.Empty;

        /// <summary>
        /// Cabeçalho da prova. Texto adicional que ficará antes das questões e apoós o título
        /// </summary>
        /// <returns></returns>
        [Category("Texto")]
        [Description("Cabeçalho da prova. Texto adicional que ficará antes das questões e após o título")]
        public string Header { get; set; } = InnerLibs.Text.Empty;

        /// <summary>
        /// Porcentagem de Acertos do Usuário
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Porcentagem de acertos da avaliação")]
        public decimal HitPercent => Math.Round(Average.CalculatePercent(Weight));

        /// <summary>
        /// Numero de questões que o usuário acertou
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Numero de questões que o usuário acertou")]
        public int Hits => this.Count(x => x.IsCorrect);

        /// <summary>
        /// Informações adicionais, normalmente nome do usuario e outras informações unicas
        /// </summary>
        /// <returns></returns>
        [Category("ID")]
        [Description("ID único desta prova")]
        public string ID { get; set; }

        /// <summary>
        /// Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo
        /// permitido, caso contrário, FALSE
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo permitido, caso contrário, FALSE")]
        public bool IsApproved => FinalNote >= MinimumWeightAllowed;

        /// <summary>
        /// Verifica se o peso da prova equivale a soma dos pesos das questões
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Verifica se o peso da prova equivale a soma dos pesos das questões")]
        public bool IsValid => this.Sum(q => q.Weight) == Weight;

        /// <summary>
        /// Valor Minimo da nota para aprovação (Normalmente 6)
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Valor Minimo da nota para aprovação (Normalmente 6)")]
        public decimal MinimumWeightAllowed { get; set; } = 6m;

        /// <summary>
        /// Informações adicionais, normalmente nome do usuario e outras informações unicas
        /// </summary>
        /// <returns></returns>
        [Category("Usuário")]
        [Description("Informações relacionadas ao usuário/aluno, como Nome, documentos e outras informações pessoais")]
        public Dictionary<string, object> PersonalInfo { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Retorna as questões desta avaliação
        /// </summary>
        /// <returns></returns>

        public QuestionTest Questions => this;

        /// <summary>
        /// Titulo da Avaliação
        /// </summary>
        /// <returns></returns>
        [Category("Texto")]
        [Description("Título da Avaliação")]
        public string Title
        {
            get => _title.ToProperCase();

            set => _title = value.ToProperCase();
        }

        /// <summary>
        /// Peso da Avaliação (Normalmente 10)
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Peso da Avaliação (Normalmente 10)")]
        public decimal Weight { get; set; } = 10m;

        /// <summary>
        /// Adiciona uma nova questão a avaliação.
        /// </summary>
        public QuestionType CreateQuestion<QuestionType>() where QuestionType : Question
        {
            QuestionType Question = (QuestionType)Activator.CreateInstance(typeof(QuestionType), this);
            if (!Contains(Question))
            {
                Add(Question);
            }

            return Question;
        }

        /// <summary>
        /// Pega uma Alternativa de uma Questão pelo ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Alternative GetAlternative(string ID)
        {
            try
            {
                return (Alternative)GetQuestion<AlternativeQuestion>(ID.GetFirstChars(2)).Alternatives.Where(a => (a.ID.ToLower() ?? InnerLibs.Text.Empty) == (ID.ToLower() ?? InnerLibs.Text.Empty));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Pega uma questão por ID
        /// </summary>
        /// <typeparam name="T">Tipo da Questão</typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T GetQuestion<T>(string ID) where T : Question
        {
            try
            {
                return (T)this.Where(q => (q.ID.ToLower() ?? InnerLibs.Text.Empty) == (ID.ToLower() ?? InnerLibs.Text.Empty)).First();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Configura o valor minimo permitido para aprovação como metade do peso da avaliação
        /// </summary>
        /// <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        public void SetMinimumAllowedAsHalf(decimal Weight = 0m)
        {
            if (Weight < 1m || Weight == default)
            {
                Weight = this.Weight;
            }

            this.Weight = Weight;
            MinimumWeightAllowed = this.Weight / 2m;
        }

        /// <summary>
        /// Configura o valor minimo permitido para aprovação como n% do peso da avaliação
        /// </summary>
        /// <param name="Percent">Porcentagem da prova</param>
        /// <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        public void SetMinimumAllowedAsPercent(string Percent, decimal Weight = 0m)
        {
            if (Weight < 1m | Weight == default)
            {
                Weight = this.Weight;
            }

            this.Weight = Weight;
            MinimumWeightAllowed = Percent.CalculateValueFromPercent(this.Weight);
        }

        public override string ToString() => Title;
    }

    /// <summary>
    /// Pergunta de alternativa. o Usuário deverá assinalar a UNICA alternativa correta entre varias alternativas
    /// </summary>
    public class SingleAlternativeQuestion : AlternativeQuestion
    {
        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public override decimal Hits => IsCorrect ? Weight : 0m;

        /// <summary>
        /// Verifica se a pergunta está corretamente assinalada. Anula a questão automaticamente se
        /// estiver mal formada (com mais de uma alternativa correta ou nenhuma alternativa correta)
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect
        {
            get
            {
                if (IsValidQuestion)
                {
                    foreach (var q in Alternatives)
                    {
                        if (!q.IsCorrect)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Verifica se as existe apenas uma unica alternativa correta na questão
        /// </summary>
        /// <returns></returns>
        public bool IsValidQuestion => Alternatives.Count(q => q.Correct) == 1;
    }

    /// <summary>
    /// Imagem com legenda de um enunciado
    /// </summary>
    public class StatementImage
    {
        internal StatementImage(StatementImages l)
        {
            StatementImages = l;
        }

        public string HTML
        {
            get
            {
                if (StatementImages != null)
                {
                    return "<li class='Image'><img src=" + Image.ToDataURL().Quote() + " alt= " + Subtitle.Quote() + "/><small>Imagem " + (StatementImages.IndexOf(this) + 1).ToString() + ": " + Subtitle + "</small></li>";
                }

                return InnerLibs.Text.Empty;
            }
        }

        /// <summary>
        /// Imagem do enunciado
        /// </summary>
        /// <returns></returns>
        public Image Image { get; set; }

        public StatementImages StatementImages { get; private set; }

        /// <summary>
        /// Legenda da Imagem
        /// </summary>
        /// <returns></returns>
        public string Subtitle { get; set; } = InnerLibs.Text.Empty;
    }

    /// <summary>
    /// Imagens adicionada a um enuncidado
    /// </summary>
    public class StatementImages : List<StatementImage>
    {
        internal StatementImages(QuestionStatement Statement)
        {
            this.Statement = Statement;
        }

        public QuestionStatement Statement { get; private set; }

        public void Add(Image Image, string Subtitle = InnerLibs.Text.Empty)
        {
            var i = new StatementImage(this)
            {
                Image = Image,
                Subtitle = Subtitle
            };
            base.Add(i);
        }

        public void Add(string ImagePath, string Subtitle = InnerLibs.Text.Empty)
        {
            var i = new StatementImage(this)
            {
                Image = Image.FromFile(ImagePath),
                Subtitle = Subtitle
            };
            base.Add(i);
        }

        public override string ToString() => Count.ToString();
    }
}