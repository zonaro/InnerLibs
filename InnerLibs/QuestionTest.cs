using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace InnerLibs.QuestionTest
{
    /// <summary>
    /// Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem Dissertativas, Multipla Escolha ou de Atribuição de Pontos
    /// </summary>
    [Serializable]
    [Category("Avaliação")]
    [Description("Representa uma avaliação de perguntas e respostas")]
    public class QuestionTest : ObservableCollection<Question>
    {
        /// <summary>
        /// Informações adicionais, normalmente nome do usuario e outras informações unicas
        /// </summary>
        /// <returns></returns>
        [Category("Usuário")]
        [Description("Informações relacionadas ao usuário/aluno, como Nome, documentos e outras informações pessoais")]
        public Dictionary<string, object> PersonalInfo { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Informações adicionais, normalmente nome do usuario e outras informações unicas
        /// </summary>
        /// <returns></returns>
        [Category("ID")]
        [Description("ID único desta prova")]
        public string ID { get; set; }

        protected sealed override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
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
        /// Verifica se o peso da prova equivale a soma dos pesos das questões
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Verifica se o peso da prova equivale a soma dos pesos das questões")]
        public bool IsValid
        {
            get
            {
                return this.Sum(q => q.Weight) == Weight;
            }
        }

        /// <summary>
        /// Retorna as questões desta avaliação
        /// </summary>
        /// <returns></returns>

        public QuestionTest Questions
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// Titulo da Avaliação
        /// </summary>
        /// <returns></returns>
        [Category("Texto")]
        [Description("Título da Avaliação")]
        public string Title
        {
            get
            {
                return _title.ToProperCase();
            }

            set
            {
                _title = value.ToProperCase();
            }
        }

        private string _title = "";

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
        /// Pega uma questão por ID
        /// </summary>
        /// <typeparam name="T">Tipo da Questão</typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T GetQuestion<T>(string ID) where T : Question
        {
            try
            {
                return (T)this.Where(q => (q.ID.ToLower() ?? "") == (ID.ToLower() ?? "")).First();
            }
            catch (Exception ex)
            {
                return null;
            }
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
                return (Alternative)GetQuestion<AlternativeQuestion>(ID.GetFirstChars(2)).Alternatives.Where(a => (a.ID.ToLower() ?? "") == (ID.ToLower() ?? ""));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Valor Minimo da nota para aprovação (Normalmente 6)
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Valor Minimo da nota para aprovação (Normalmente 6)")]
        public decimal MinimumWeightAllowed { get; set; } = 6m;

        /// <summary>
        /// Peso da Avaliação (Normalmente 10)
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Peso da Avaliação (Normalmente 10)")]
        public decimal Weight { get; set; } = 10m;

        /// <summary>
        /// Cabeçalho da prova. Texto adicional que ficará antes das questões e apoós o título
        /// </summary>
        /// <returns></returns>
        [Category("Texto")]
        [Description("Cabeçalho da prova. Texto adicional que ficará antes das questões e após o título")]
        public string Header { get; set; } = "";

        /// <summary>
        /// Rodapé da prova. Texto adicional que ficará após as questões
        /// </summary>
        /// <returns></returns>
        [Category("Texto")]
        [Description("Rodapé da prova. Texto adicional que ficará após as questões")]
        public string Footer { get; set; } = "";

        /// <summary>
        /// Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo permitido, caso contrário, FALSE
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo permitido, caso contrário, FALSE")]
        public bool IsApproved
        {
            get
            {
                return FinalNote >= MinimumWeightAllowed;
            }
        }

        /// <summary>
        /// Numero de questões que o usuário acertou
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Numero de questões que o usuário acertou")]
        public int Hits
        {
            get
            {
                int c = 0;
                foreach (var q in this)
                {
                    if (q.IsCorrect)
                        c = c + 1;
                }

                return c;
            }
        }

        /// <summary>
        /// Numero de questões que o usuário errou
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Numero de questões que o usuário errou")]
        public int Fails
        {
            get
            {
                return Count - Hits;
            }
        }

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
                    int correct = 0;
                    int somapesos = 0;
                    int somaquestoes = 0;
                    foreach (var q in this)
                    {
                        somapesos = (int)Math.Round(somapesos + q.Weight);
                        somaquestoes = (int)Math.Round(somaquestoes + (q.IsCorrect ? q.Hits : 0m));
                    }

                    return Weight * somaquestoes / Count;
                }
                catch (Exception ex)
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
        /// Nota final da avaliação (Bonus + Média)
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Nota final da avaliação (Bonus + Média)")]
        public decimal FinalNote
        {
            get
            {
                return Average + Bonus;
            }
        }

        /// <summary>
        /// Porcentagem de Acertos do Usuário
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Porcentagem de acertos da avaliação")]
        public int HitPercent
        {
            get
            {
                return (int)Math.Round(Average.CalculatePercent(Weight));
            }
        }

        /// <summary>
        /// Porcentagem de Erros do Usuário
        /// </summary>
        /// <returns></returns>
        [Category("Validação")]
        [Description("Porcentagem de erros da avaliação")]
        public int FailPercent
        {
            get
            {
                return (int)Math.Round((Weight - Average).CalculatePercent(Weight));
            }
        }

        /// <summary>
        /// Instancia uma nova avaliação com titulo
        /// </summary>
        /// <param name="Title">Titulo da avaliação</param>
        public QuestionTest(string Title = "New Test")
        {
            this.Title = Title;
        }

        /// <summary>
        /// Configura o valor minimo permitido para aprovação como metade do peso da avaliação
        /// </summary>
        /// <param name="Weight">Parametro opcional que altera o valor do peso da avaliação</param>
        public void SetMinimumAllowedAsHalf(decimal Weight = 0m)
        {
            if (Weight < 1m | Weight == default)
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
    }

    /// <summary>
    /// Classe Base para as questões de uma avaliação
    /// </summary>
    public abstract class Question
    {
        public Question()
        {
            _statement = new QuestionStatement() { _question = this };
        }

        /// <summary>
        /// Tipo da QUestão
        /// </summary>
        /// <returns></returns>
        public string QuestionType
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// Teste a qual esta questão pertence
        /// </summary>
        /// <returns></returns>

        public QuestionTest Test
        {
            get
            {
                return _test;
            }
        }

        internal QuestionTest _test = new QuestionTest();

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

        internal QuestionStatement _statement;

        /// <summary>
        /// Peso da Pergunta
        /// </summary>
        /// <returns></returns>
        public decimal Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                _weight = value.SetMaxValue(Test.Weight - Test.Where(q => !ReferenceEquals(q, this)).Sum(q => q.Weight));
            }
        }

        internal decimal _weight = 1m;

        /// <summary>
        /// Retorna um numero que representa o quanto o usuario acertou essa pergunta
        /// </summary>
        /// <returns></returns>
        public virtual decimal Hits { get; private set; } = 0m;

        /// <summary>
        /// Verifica se a pergunta está corretamente assinalada
        /// </summary>
        /// <returns></returns>
        public abstract bool IsCorrect { get; }

        /// <summary>
        /// Return the statment text for this question
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Number.ToString() + ") " + Statement.Text;
        }

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
        /// Indica se esta questão foi revisada pelo professor
        /// </summary>
        /// <returns></returns>
        public bool Reviewed { get; set; } = false;
    }

    /// <summary>
    /// Imagens adicionada a um enuncidado
    /// </summary>
    public class StatementImages : List<StatementImage>
    {
        public QuestionStatement Statement { get; private set; }

        internal StatementImages(QuestionStatement Statement)
        {
            this.Statement = Statement;
        }

        public new void Add(Image Image, string Subtitle = "")
        {
            var i = new StatementImage(this);
            i.Image = Image;
            i.Subtitle = Subtitle;
        }

        public new void Add(string ImagePath, string Subtitle = "")
        {
            var i = new StatementImage(this);
            i.Image = File.ReadAllBytes(ImagePath).ToImage();
            i.Subtitle = Subtitle;
        }

        public override string ToString()
        {
            return Count.ToString();
        }
    }

    /// <summary>
    /// Enunciado de uma pergunta
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class QuestionStatement
    {
        internal QuestionStatement()
        {
            this.Images = new StatementImages(this);
        }

        public Question Question
        {
            get
            {
                return _question;
            }
        }

        internal Question _question;
        internal string _text = "";

        /// <summary>
        /// Texto do enunciado
        /// </summary>
        /// <returns></returns>
        public string Text
        {
            get
            {
                return _text.FixText();
            }

            set
            {
                _text = value.FixText();
            }
        }

        /// <summary>
        /// Imagens adicionadas ao enunciado (com legenda)
        /// </summary>
        /// <returns></returns>
        public StatementImages Images { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// Imagem com legenda de um enunciado
    /// </summary>
    public class StatementImage
    {
        /// <summary>
        /// Imagem do enunciado
        /// </summary>
        /// <returns></returns>
        public Image Image { get; set; }

        /// <summary>
        /// Legenda da Imagem
        /// </summary>
        /// <returns></returns>
        public string Subtitle { get; set; } = "";

        internal StatementImage(StatementImages l)
        {
            StatementImages = l;
        }

        public StatementImages StatementImages { get; private set; }

        public string HTML
        {
            get
            {
                if (StatementImages != null)
                {
                    return "<li class='Image'><img src=" + Image.ToDataURL().Quote() + " alt= " + Subtitle.Quote() + "/><small>Imagem " + (StatementImages.IndexOf(this) + 1).ToString() + ": " + Subtitle + "</small></li>";
                }

                return "";
            }
        }
    }

    /// <summary>
    /// Questões em que a resposta é numerica e implica diretamente no peso da questão (normalmente utilizada em pesquisas)
    /// </summary>
    public class NumericQuestion : Question
    {
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

        private decimal a = 0m;

        /// <summary>
        /// Menor valor permitido pela questão
        /// </summary>
        /// <returns></returns>
        public decimal MinValue { get; set; } = 1m;

        /// <summary>
        /// Maior valor permitido pela questão
        /// </summary>
        /// <returns></returns>
        public decimal MaxValue { get; set; } = 10m;

        /// <summary>
        /// Pontos multiplicados pelo peso da questão
        /// </summary>
        /// <returns></returns>
        public override decimal Hits
        {
            get
            {
                return Answer * Weight;
            }
        }

        /// <summary>
        /// Perguntas numericas sempre estão corretas. Neste caso, o que vale é a resposta multiplicada pelo peso que implica diretamente no peso da avaliação
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect
        {
            get
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Questão Dissertativa. Deve ser corrigida manualmente
    /// </summary>
    public class DissertativeQuestion : Question
    {
        /// <summary>
        /// Resposta dissertativa da pergunta
        /// </summary>
        /// <returns></returns>
        public string Answer { get; set; }

        /// <summary>
        /// Valor que indica se a questão está de alguma forma correta
        /// </summary>
        /// <returns></returns>
        public bool Correct
        {
            get
            {
                return Assertiveness > 0m;
            }

            set
            {
                Assertiveness = value ? Weight : 0m;
            }
        }

        /// <summary>
        /// Numero de linhas que devem ser impressas para esta questão
        /// </summary>
        /// <returns></returns>
        public int Lines { get; set; } = 3;

        /// <summary>
        /// Verifica se a pergunta está preenchida
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect
        {
            get
            {
                return Correct;
            }
        }

        /// <summary>
        /// Representa quantos pontos essa questão vale de acordo com a assertividade
        /// </summary>
        /// <returns></returns>
        public override decimal Hits
        {
            get
            {
                return Assertiveness;
            }
        }

        /// <summary>
        /// Assertividade da questão, uma valor entre 0 e o peso da questão que representa o quanto esta questão está correta
        /// </summary>
        /// <returns></returns>
        public decimal Assertiveness
        {
            set
            {
                ass = value.LimitRange(0, Weight);
            }

            get
            {
                return ass.LimitRange(0, Weight);
            }
        }

        private decimal ass;
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
        /// Lista de alternativas da questão
        /// </summary>
        /// <returns></returns>
        public AlternativeList Alternatives { get; private set; }

        /// <summary>
        /// Indica se esta alternativa deve ser renderizada no HTML como um <see cref="HtmlSelectElement"/>. Caso Contrario, serão renderizadas como listas de Check Box ou Radio Button
        /// </summary>
        /// <returns></returns>
        public bool RenderAsSelect { get; set; } = false;

        /// <summary>
        /// Verifica se esta pergunta permite multiplas alternativas
        /// </summary>
        /// <returns></returns>
        public bool AllowMultiple
        {
            get
            {
                return ReferenceEquals(GetType(), typeof(MultipleAlternativeQuestion));
            }
        }

        /// <summary>
        /// Retorna as alternativas marcadas pelo usuário
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Alternative> Answer
        {
            get
            {
                return Alternatives.Where(p => p.Checked);
            }
        }
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
        public override decimal Hits
        {
            get
            {
                return IsCorrect ? Weight : 0m;
            }
        }

        /// <summary>
        /// Verifica se a pergunta está corretamente assinalada. Anula a questão automaticamente se estiver mal formada (com mais de uma alternativa correta ou nenhuma alternativa correta)
        /// </summary>
        /// <returns></returns>
        public override bool IsCorrect
        {
            get
            {
                if (IsValidQuestion)
                {
                    int total = Alternatives.Count;
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
        public bool IsValidQuestion
        {
            get
            {
                int c = 0;
                foreach (var q in Alternatives)
                {
                    if (q.Correct)
                    {
                        c = c + 1;
                    }
                }

                return c == 1;
            }
        }
    }

    /// <summary>
    /// Pergunta de Verdadeiro ou Falso. O Usuário deverá assinalar as questões verdadeiras ou falsas correspondente ao enunciado da pergunta.
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
        public override bool IsCorrect
        {
            get
            {
                return Hits > 0m;
            }
        }
    }

    /// <summary>
    /// Lista de Alternativas de uma questão de alternativas
    /// </summary>
    public class AlternativeList : ObservableCollection<Alternative>
    {
        protected sealed override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
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

        public AlternativeQuestion Question { get; private set; }

        internal AlternativeList(AlternativeQuestion l)
        {
            Question = l;
        }

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

        public override string ToString()
        {
            return Count.ToString();
        }
    }

    /// <summary>
    /// Objeto que representa uma alternativa de uma pergunta de alternativas
    /// </summary>
    public class Alternative
    {
        public Alternative()
        {
            _question = new SingleAlternativeQuestion();
        }

        public override string ToString()
        {
            return Number.ToRoman().Quote('(') + Text;
        }

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

        /// <summary>
        /// Texto da alternativa
        /// </summary>
        /// <returns></returns>
        public string Text { get; set; }

        /// <summary>
        /// Valor que indica se a alternativa está correta ou verdadeira
        /// </summary>
        /// <returns></returns>
        public bool Correct { get; set; }

        /// <summary>
        /// Valor que indica se esta alternativa foi assinalada
        /// </summary>
        /// <returns></returns>
        public bool Checked { get; set; }

        /// <summary>
        /// Verifica se a resposta do usuário é correta para esta alternativa
        /// </summary>
        /// <returns></returns>
        public bool IsCorrect
        {
            get
            {
                return Checked == Correct;
            }
        }

        public AlternativeQuestion Question
        {
            get
            {
                return _question;
            }
        }

        internal AlternativeQuestion _question;
    }
}