using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace InnerLibs.QuestionTest
{
    public static class QuestionTestExtensions
    {
        #region Public Methods

        public static List<QuestionTest> Rank(this List<QuestionTest> Tests)
        {
            Tests?.Sort();
            return Tests;
        }

        public static TQuestion SetWeight<TQuestion>(this TQuestion Question, decimal Weight) where TQuestion : Question
        {
            if (Question != null)
            {
                Question.Weight = Weight;
            }
            return Question;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Objeto que representa uma alternativa de uma pergunta de alternativas
    /// </summary>
    public class Alternative
    {
        #region Internal Fields

        internal AlternativeQuestion _question;

        #endregion Internal Fields



        #region Internal Constructors

        internal Alternative(AlternativeQuestion question)
        {
            _question = question;
        }

        #endregion Internal Constructors



        #region Public Properties

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
        public string ID => Question != null ? $"{Question.ID}A{Number}" : null;

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
            get => Question != null ? Question.Alternatives.IndexOf(this) + 1 : -1;

            set => Question?.Alternatives.Move(Question.Alternatives.IndexOf(this), (value - 1).LimitRange(0, Question.Alternatives.Count - 1));
        }

        public AlternativeQuestion Question => _question;

        /// <summary>
        /// Texto da alternativa
        /// </summary>
        /// <returns></returns>
        public string Text { get; set; }

        #endregion Public Properties



        #region Public Methods

        public override string ToString() => Number.ToRoman().Quote('(') + Text;

        #endregion Public Methods
    }

    /// <summary>
    /// Lista de Alternativas de uma questão de alternativas
    /// </summary>
    public class AlternativeList : ObservableCollection<Alternative>
    {
        #region Internal Constructors

        internal AlternativeList(AlternativeQuestion l)
        {
            Question = l;
        }

        #endregion Internal Constructors



        #region Public Properties

        public AlternativeQuestion Question { get; private set; }

        #endregion Public Properties



        #region Protected Methods

        protected override sealed void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e?.Action == NotifyCollectionChangedAction.Add)
            {
                Debug.WriteLine("Alternative Added");
                foreach (var ni in e.NewItems)
                {
                    Alternative i = (Alternative)ni;
                    i._question = Question;
                    if (!Question.AllowMultiple && i.Correct)
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

        #endregion Protected Methods

        /// <summary>
        /// Adiciona uma alternativa a questão. A alternativa é ignorada se já existir na lista
        /// </summary>
        /// <param name="Text">Texto da alternativa</param>
        /// <param name="Correct">Parametro que indica se esta alternativa é correta ou verdadeira</param>
        public void Add(string Text, bool Correct) => Add(new Alternative(this.Question) { Text = Text, Correct = Correct });

        public void AddRange(IEnumerable<Alternative> Alternatives)
        {
            if (Alternatives != null)
                foreach (var alt in Alternatives)
                    Add(alt);
        }

        public override string ToString() => ToString("Alternatives");

        public string ToString(string AlternativesWord) => $"{Count} {AlternativesWord.QuantifyText(Count)}";
    }

    /// <summary>
    /// Classe base para questões de 'alternativa' ou de 'verdadeiro ou falso'
    /// </summary>
    public abstract class AlternativeQuestion : Question
    {
        #region Internal Constructors

        internal AlternativeQuestion() : base()
        {
            Alternatives = new AlternativeList(this);
        }

        #endregion Internal Constructors



        #region Public Properties

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

        #endregion Public Properties



        #region Public Methods

        public void CreateAlternative(string Text, bool Correct = false)
        {
            Alternatives.Add(Text, Correct);
        }

        /// <summary>
        /// Embaralha as alternativas
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < Alternatives.Count; i++)
            {
                var num1 = 0;
                var num2 = 0;
                while (num1 == num2)
                {
                    num1 = Generate.RandomNumber(0, Alternatives.Count - 1);
                    num2 = Generate.RandomNumber(0, Alternatives.Count - 1);
                }

                this.Alternatives.Move(num1, num2);
            }
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Questão Dissertativa. Deve ser corrigida manualmente
    /// </summary>
    public class DissertativeQuestion : Question
    {
        #region Private Fields

        private decimal ass;

        #endregion Private Fields



        #region Internal Constructors

        internal DissertativeQuestion(QuestionTest Test) : base()
        {
            _test = Test;
        }

        #endregion Internal Constructors



        #region Public Properties

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
        public override decimal Hits
        {
            get => Assertiveness;
            set => Assertiveness = value;
        }

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

        #endregion Public Properties
    }

    /// <summary>
    /// Pergunta de Verdadeiro ou Falso. O Usuário deverá assinalar as questões verdadeiras ou
    /// falsas correspondente ao enunciado da pergunta.
    /// </summary>
    public class MultipleAlternativeQuestion : AlternativeQuestion
    {
        #region Internal Constructors

        internal MultipleAlternativeQuestion(QuestionTest Test) : base()
        {
            this._test = Test;
        }

        #endregion Internal Constructors



        #region Public Properties

        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public override decimal Hits
        {
            get => Alternatives.Count > 0 ? Alternatives.Count(x => x.IsCorrect) * Weight / Alternatives.Count : 0;
            set => Debug.WriteLine($"Can't set hits on {nameof(MultipleAlternativeQuestion)}");
        }

        /// <summary>
        /// Verifica se a pergunta está corretamente assinalada
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect => Hits > 0m;

        #endregion Public Properties
    }

    /// <summary>
    /// Questões em que a resposta é numerica e implica diretamente no peso da questão (normalmente
    /// utilizada em pesquisas)
    /// </summary>
    public class NumericQuestion : Question
    {
        #region Private Fields

        private decimal a = 0m;

        #endregion Private Fields



        #region Internal Constructors

        internal NumericQuestion(QuestionTest Test) : base()
        {
            _test = Test;
        }

        #endregion Internal Constructors



        #region Public Properties

        /// <summary>
        /// Pontos que o usuario atribuiu a esta questão
        /// </summary>
        /// <returns></returns>
        public decimal Answer
        {
            get => a;

            set => a = value.LimitRange(MinValue, MaxValue);
        }

        /// <summary>
        /// Pontos multiplicados pelo peso da questão
        /// </summary>
        /// <returns></returns>
        public override decimal Hits
        {
            get => Answer * Weight;
            set => Answer = value;
        }

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

        #endregion Public Properties
    }

    /// <summary>
    /// Classe Base para as questões de uma avaliação
    /// </summary>
    public abstract class Question
    {
        #region Internal Fields

        internal QuestionStatement _statement;

        internal QuestionTest _test;

        internal decimal _weight = 1m;

        #endregion Internal Fields



        #region Internal Constructors

        internal Question() => _statement = new QuestionStatement() { _question = this };

        #endregion Internal Constructors



        #region Public Properties

        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public abstract decimal Hits { get; set; }

        public decimal HitsPercent => Hits.CalculatePercent(Weight);

        /// <summary>
        /// O codigo de identificação desta questão
        /// </summary>
        /// <returns></returns>
        public string ID => Test != null ? $"Q{Number}" : null;

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
            get => Test != null ? Test.IndexOf(this) + 1 : -1;

            set => Test?.Move(Test.IndexOf(this), (value - 1).LimitRange(0, Test.Count - 1));
        }

        /// <summary>
        /// Tipo da Questão
        /// </summary>
        /// <returns></returns>
        public string QuestionType => GetType().Name;

        /// <summary>
        /// Indica se esta questão foi revisada pelo professor
        /// </summary>
        /// <returns></returns>
        public bool Reviewed { get; set; }

        /// <summary>
        /// Enunciado da questão (texto da pergunta)
        /// </summary>
        /// <returns></returns>
        public QuestionStatement Statement
        {
            get => _statement;

            set
            {
                if (value != null)
                {
                    _statement = value;
                    value._question = this;
                }
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

        #endregion Public Properties



        #region Public Methods

        /// <summary>
        /// Return the statment text for this question
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ($"{Number}) " + Statement?.Text).Trim();

        #endregion Public Methods
    }

    /// <summary>
    /// Enunciado de uma pergunta
    /// </summary>
    public class QuestionStatement
    {
        #region Internal Fields

        internal Question _question;

        internal string _text = InnerLibs.Text.Empty;

        #endregion Internal Fields



        #region Internal Constructors

        internal QuestionStatement() => this.Images = new StatementImages(this);

        #endregion Internal Constructors



        #region Public Properties

        /// <summary>
        /// Imagens adicionadas ao enunciado (com legenda)
        /// </summary>
        /// <returns></returns>
        public StatementImages Images { get; }

        public Question Question => _question;
        public QuestionTest Test => Question.Test;

        /// <summary>
        /// Texto do enunciado
        /// </summary>
        /// <returns></returns>
        public string Text
        {
            get => _text.FixText();

            set => _text = value.FixText();
        }

        #endregion Public Properties



        #region Public Methods

        public override string ToString() => Text;

        #endregion Public Methods
    }

    /// <summary>
    /// Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem
    /// Dissertativas, Multipla Escolha ou de Atribuição de Pontos
    /// </summary>
    [Serializable]
    public class QuestionTest : ObservableCollection<Question>, IComparable<QuestionTest>, IComparable
    {
        #region Private Fields

        private string _title = Text.Empty;

        private Dictionary<string, object> personalInfo = new Dictionary<string, object>();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Instancia uma nova avaliação com titulo
        /// </summary>
        /// <param name="Title">Titulo da avaliação</param>
        public QuestionTest(string Title) : this(Title, "")
        {
            GenerateID();
        }

        public QuestionTest() : this("New Test")
        {
        }

        public QuestionTest(string Title, string ID)
        {
            this.Title = Title;
            this.ID = ID;
        }

        public QuestionTest(string Title, Guid ID) : this(Title, ID.ToString())
        {
        }

        #endregion Public Constructors



        #region Public Properties

        /// <summary>
        /// Média da Avaliação
        /// </summary>
        /// <returns></returns>
        public decimal Average => this.Count > 0 ? this.Sum(x => (x.IsCorrect ? x.Hits : 0) * x.Weight) / this.Sum(x => x.Weight) : 0;

        public decimal Bonus { get; set; }

        /// <summary>
        /// Pontos de bonificação que serão somados a média final da avaliação
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Porcentagem de Erros do Usuário
        /// </summary>
        /// <returns></returns>
        public decimal FailPercent => Math.Round((Weight - Average).CalculatePercent(Weight));

        /// <summary>
        /// Numero de questões que o usuário errou
        /// </summary>
        /// <returns></returns>
        public int Fails => Count - Hits;

        /// <summary>
        /// Nota final da avaliação (Bonus + Média)
        /// </summary>
        /// <returns></returns>
        public decimal FinalNote => Average + Bonus;

        public string Footer { get; set; }

        public string Header { get; set; }

        public decimal HitPercent => Math.Round(Average.CalculatePercent(Weight));

        public int Hits => this.Count(x => x.IsCorrect);

        public string ID { get; set; }

        /// <summary>
        /// Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo
        /// permitido, caso contrário, FALSE
        /// </summary>
        /// <returns></returns>
        public bool IsApproved => FinalNote >= MinimumWeightAllowed;

        public bool IsValid => this.Sum(q => q.Weight) == Weight;

        public decimal MinimumWeightAllowed { get; set; } = 6m;

        public Dictionary<string, object> PersonalInfo { get => personalInfo; set => personalInfo = value ?? new Dictionary<string, object>(); }

        public QuestionTest Questions => this;

        public string Title
        {
            get => _title.ToProperCase();

            set => _title = value.ToProperCase();
        }

        /// <summary>
        /// Verifica se o peso da prova equivale a soma dos pesos das questões
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Valor Minimo da nota para aprovação (Normalmente 6)
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Informações adicionais, normalmente nome do usuario e outras informações unicas
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Retorna as questões desta avaliação
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Titulo da Avaliação
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Peso da Avaliação (Normalmente 10)
        /// </summary>
        /// <returns></returns>
        public decimal Weight { get; set; } = 10m;

        #endregion Public Properties



        #region Protected Methods

        protected override sealed void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e?.Action)
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

        #endregion Protected Methods

        public static bool operator !=(QuestionTest left, QuestionTest right)
        {
            return !(left == right);
        }

        public static bool operator <(QuestionTest left, QuestionTest right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(QuestionTest left, QuestionTest right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator ==(QuestionTest left, QuestionTest right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator >(QuestionTest left, QuestionTest right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(QuestionTest left, QuestionTest right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }

        public int CompareTo(object obj)
        {
            var pos = 0;
            if (obj != null)
            {
                if (obj is QuestionTest q)
                {
                    obj = q.FinalNote;
                }

                if (obj.IsNumber())
                {
                    obj = obj.ToDecimal();
                }

                if (obj is decimal i)
                {
                    if (i < this.FinalNote)
                        pos = -1;

                    if (i > this.FinalNote)
                        pos = 1;
                }
            }
            return pos;
        }

        public int CompareTo(QuestionTest other) => CompareTo(other?.FinalNote);

        /// <summary>
        /// Cria uma questão dissertativa para esta prova
        /// </summary>
        /// <param name="Statement">Enunciado</param>
        /// <param name="Lines">Linhas para esta pergunta</param>
        /// <returns></returns>
        public DissertativeQuestion CreateDissertativeQuestion(string Statement, int Lines = 3)
        {
            var q = CreateQuestion<DissertativeQuestion>(Statement);
            q.Lines = Lines;
            return q;
        }

        public MultipleAlternativeQuestion CreateMultipleAlternativeQuestion(string Statement, params string[] Alternatives)
        {
            var q = CreateQuestion<MultipleAlternativeQuestion>(Statement);
            foreach (var item in Alternatives ?? Array.Empty<string>())
            {
                q.CreateAlternative(item, Alternatives.GetIndexOf(item) == 0);
            }
            q.Shuffle();
            return q;
        }

        public MultipleAlternativeQuestion CreateMultipleAlternativeQuestion(string Statement, params Alternative[] Alternatives)
        {
            var q = CreateQuestion<MultipleAlternativeQuestion>(Statement);
            q.Alternatives.AddRange(Alternatives);
            return q;
        }

        /// <summary>
        /// Cria uma questão de
        /// </summary>
        /// <param name="Statement"></param>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public NumericQuestion CreateNumericQuestion(string Statement, decimal MinValue = 1, decimal MaxValue = 10)
        {
            var q = CreateQuestion<NumericQuestion>(Statement);
            Misc.FixOrder(ref MinValue, ref MaxValue);
            q.MinValue = MinValue;
            q.MaxValue = MaxValue;
            return q;
        }

        /// <summary>
        /// Adiciona uma nova questão a avaliação.
        /// </summary>
        public TQuestion CreateQuestion<TQuestion>(string Statement = null) where TQuestion : Question => (TQuestion)CreateQuestion(typeof(TQuestion), Statement);

        /// <summary>
        /// Adiciona uma nova questão a avaliação.
        /// </summary>
        public Question CreateQuestion(Type QuestionType, string Statement = null)
        {
            if (QuestionType != null && QuestionType.IsSubclassOf(typeof(Question)))
            {
                Question Question = (Question)Activator.CreateInstance(QuestionType, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new object[] { this }, null);
                if (Statement.IsNotBlank()) Question.Statement.Text = Statement;
                if (!Contains(Question))
                {
                    Add(Question);
                }

                return Question;
            }
            return null;
        }

        public SingleAlternativeQuestion CreateSingleAlternativeQuestion(string Statement, params string[] Alternatives)
        {
            var q = CreateQuestion<SingleAlternativeQuestion>(Statement);
            foreach (var item in Alternatives ?? Array.Empty<string>())
            {
                q.CreateAlternative(item, Alternatives.GetIndexOf(item) == 0);
            }
            q.Shuffle();
            return q;
        }

        public SingleAlternativeQuestion CreateSingleAlternativeQuestion(string Statement, params Alternative[] Alternatives)
        {
            var q = CreateQuestion<SingleAlternativeQuestion>(Statement);
            q.Alternatives.AddRange(Alternatives);
            return q;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Rodapé da prova. Texto adicional que ficará após as questões
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Cabeçalho da prova. Texto adicional que ficará antes das questões e apoós o título
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Porcentagem de Acertos do Usuário
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Numero de questões que o usuário acertou
        /// </summary>
        /// <returns></returns>
        /// <summary>
        ///ID da prova, para identifica-la como unica
        /// </summary>
        /// <returns></returns>
        public QuestionTest GenerateID()
        {
            ID = Guid.NewGuid().ToString();
            return this;
        }

        /// <summary>
        /// Pega uma Alternativa de uma Questão pelo ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Alternative GetAlternative(string ID) => GetQuestion<AlternativeQuestion>(ID.GetFirstChars(2)).Alternatives.FirstOrDefault(a => $"{a.ID}" == $"{ID}");

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pega uma questão por ID
        /// </summary>
        /// <typeparam name="T">Tipo da Questão</typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T GetQuestion<T>(string ID) where T : Question => (T)this.FirstOrDefault(q => $"{q.ID}" == $"{ID}");

        /// <summary>
        /// Configura o valor minimo permitido para aprovação como metade do peso da avaliação
        /// </summary>
        /// <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        public QuestionTest SetMinimumAllowedAsHalf(decimal Weight = 0m)
        {
            if (Weight < 1m || Weight == default)
            {
                Weight = this.Weight;
            }

            this.Weight = Weight;
            MinimumWeightAllowed = this.Weight / 2m;
            return this;
        }

        /// <summary>
        /// Configura o valor minimo permitido para aprovação como n% do peso da avaliação
        /// </summary>
        /// <param name="Percent">Porcentagem da prova</param>
        /// <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        public QuestionTest SetMinimumAllowedAsPercent(string Percent, decimal Weight = 0m)
        {
            if (Weight < 1m | Weight == default)
            {
                Weight = this.Weight;
            }

            this.Weight = Weight;
            MinimumWeightAllowed = Percent.CalculateValueFromPercent(this.Weight);
            return this;
        }

        public QuestionTest Shuffle()
        {
            for (int i = 0; i < Count; i++)
            {
                var num1 = 0;
                var num2 = 0;
                while (num1 == num2)
                {
                    num1 = Generate.RandomNumber(0, Count - 1);
                    num2 = Generate.RandomNumber(0, Count - 1);
                }

                this.Move(num1, num2);
            }
            return this;
        }

/// <summary>
/// Retorna uma <see cref="HtmlTag" /> contendo um formulário com esta <see cref="QuestionTest" />
/// </summary>
/// <returns></returns>
        public HtmlTag ToHTML()
        {
            var form = new HtmlTag("form");
            var header = new HtmlTag("header").InsertInto(form);

            if (Title.IsNotBlank())
            {
                header.AddChildren("h1", Title).AddClass("main-title");
            }
            if (PersonalInfo != null && PersonalInfo.Any())
            {
                var section = new HtmlTag("section")
                .AddClass("personal-info")
                .InsertInto(header);

                foreach (var inf in PersonalInfo)
                {
                    new HtmlTag("div")
                    .AddClass("personal-info-item")
                   .AddChildren("label", inf.Key)
                   .AddChildren(HtmlTag.CreateInput($"{inf.Key}_input", $"{inf.Value}"))
                   .AddBreakLine()
                   .InsertInto(section);
                }
            }

            header.AddHorizontalRule();

            var list = new HtmlTag("section").AddClass("question-list").InsertInto(form);

            foreach (var quest in this.Questions)
            {
                var sq = new HtmlTag("article").AddClass($"question-{quest.Number}").SetID(quest.ID).InsertInto(list)
                .AddChildren(quest.Statement.ToString().WrapInTag("h3").AddClass($"question-text-{quest.Number}"));

                if (quest.Statement.Images.Any())
                {
                    var imgpart = new HtmlTag("div").AddClass($"images-{quest.Number}").InsertInto(sq);
                    foreach (var i in quest.Statement.Images)
                    {
                        new HtmlTag("figure")
                        .AddChildren(HtmlTag.CreateImage(i.Image).SetAttribute("alt", i.Subtitle).SetID(i.ID).AddClass($"img-{i.Number}"))
                        .AddChildren("figcaption", $"{i.Number} - {i.Subtitle}").AddClass($"cap-{i.Number}")
                        .InsertInto(imgpart);
                    }

                }

                if (quest is DissertativeQuestion diss)
                {
                    new HtmlTag("textarea", diss.Answer).SetID(diss.ID).SetAttribute("name", diss.ID).SetAttribute("rows", $"{diss.Lines}").InsertInto(sq);
                }
                else if (quest is AlternativeQuestion altquest)
                {
                    var alts = new HtmlTag("div").AddClass("alt-" + altquest.ID).InsertInto(sq);
                    foreach (var alt in altquest.Alternatives)
                    {
                        var lab = new HtmlTag("label").SetAttribute("for", alt.ID).InsertInto(alts);

                        if (altquest is MultipleAlternativeQuestion)
                        {
                            HtmlTag.CreateInput(alt.Question.ID, alt.ID, "checkbox").SetProp("checked", alt.Checked).InsertInto(lab);
                        }
                        else
                        {
                            HtmlTag.CreateInput(alt.Question.ID, alt.ID, "radio").SetProp("checked", alt.Checked).InsertInto(lab);
                        }

                        lab.AddChildren("span", alt.Text).AddBreakLine();

                    }

                }
                else if (quest is NumericQuestion num)
                {
                    HtmlTag.CreateInput(num.ID, $"{num.Answer}", "number").SetID(num.ID).SetAttribute("min", $"{num.MinValue}").SetAttribute("max", $"{num.MaxValue}").InsertInto(sq);
                }
            }

            return form;
        }

        public override string ToString() => Title;
    }

    /// <summary>
    /// Pergunta de alternativa. o Usuário deverá assinalar a UNICA alternativa correta entre varias alternativas
    /// </summary>
    public class SingleAlternativeQuestion : AlternativeQuestion
    {
        #region Internal Constructors

        internal SingleAlternativeQuestion(QuestionTest Test) : base()
        {
            this._test = Test;
        }

        #endregion Internal Constructors



        #region Public Properties

        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public override decimal Hits
        {
            get => IsCorrect ? Weight : 0m;
            set
            {
                if (value > 0)
                {
                    this.Alternatives.Each(x => x.Checked = x.Correct);
                }
            }
        }

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

        #endregion Public Properties
    }

    /// <summary>
    /// Imagem com legenda de um enunciado
    /// </summary>
    public class StatementImage
    {
        #region Internal Constructors

        internal StatementImage(StatementImages l)
        {
            StatementImages = l;
        }

        #endregion Internal Constructors



        #region Public Properties

        /// <summary>
        /// ID desta imagem
        /// </summary>
        public string ID => $"{Statement.Question.ID}{this.StatementImages.IndexOf(this) + 1}";

        /// <summary>
        /// Imagem do enunciado
        /// </summary>
        /// <returns></returns>
        public Image Image { get; set; }

        /// <summary>
        /// Número desta imagem em relação ao <see cref="Test"/>
        /// </summary>
        public int Number => Statement.Question.Test.SelectMany(x => x.Statement.Images).GetIndexOf(this);

        public Question Question => Statement.Question;
        public QuestionStatement Statement => StatementImages.Statement;
        public StatementImages StatementImages { get; private set; }

        /// <summary>
        /// Legenda da Imagem
        /// </summary>
        /// <returns></returns>
        public string Subtitle { get; set; } = Text.Empty;

        public QuestionTest Test => Question.Test;

        #endregion Public Properties
    }

    /// <summary>
    /// Imagens adicionada a um enuncidado
    /// </summary>
    public class StatementImages : List<StatementImage>
    {
        #region Internal Constructors

        internal StatementImages(QuestionStatement Statement)
        {
            this.Statement = Statement;
        }

        #endregion Internal Constructors



        #region Public Properties

        public QuestionStatement Statement { get; private set; }

        #endregion Public Properties



        #region Public Methods

        public StatementImages Add(Image Image, string Subtitle = Text.Empty)
        {
            var i = new StatementImage(this)
            {
                Image = Image,
                Subtitle = Subtitle
            };
            Add(i);
            return this;
        }

        public StatementImages Add(string ImagePath, string Subtitle = Text.Empty)
        {
            var i = new StatementImage(this)
            {
                Image = Image.FromFile(ImagePath),
                Subtitle = Subtitle
            };
            Add(i);
            return this;
        }

        public override string ToString() => ToString("Images");

        public string ToString(string ImagesWord) => $"{Count} {ImagesWord.QuantifyText(Count)}";

        #endregion Public Methods
    }
}