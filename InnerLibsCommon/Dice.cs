using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Extensions;
using Extensions.NumberWriters;

namespace Extensions.RolePlayingGame
{
    /// <summary>
    /// Dado de RPG
    /// </summary>
    public class Dice
    {
        #region Private Fields

        private static readonly Dice _coin = new Dice(DiceType.Coin);

        private static readonly Dice _d6 = new Dice(DiceType.D6);

        private int _rolledtimes = 0;

        #endregion Private Fields

        #region Private Methods

        private void ApplyPercent()
        {
            foreach (var f in Faces ?? Array.Empty<DiceFace>().AsEnumerable())
                f._weightpercent = GetChancePercent(f.Number);
        }

        #endregion Private Methods

        #region Public Constructors

        public Dice() : this(DiceType.D6)
        {
        }

        /// <summary>
        /// Cria um novo dado de um tipo especifico
        /// </summary>
        /// <param name="Type">Tipo de dado</param>
        public Dice(DiceType Type) : this(Type.ToInt())
        {
        }

        /// <summary>
        /// Cria um novo dado com um numero customizado de faces
        /// </summary>
        /// <param name="CustomFaces">Numero de faces do dado (Minimo de 2 faces)</param>
        public Dice(int CustomFaces)
        {
            Faces = new ReadOnlyCollection<DiceFace>(Enumerable.Range(1, CustomFaces.SetMinValue(2)).Select(x => new DiceFace(this)).ToList());
            ApplyPercent();
        }

        #endregion Public Constructors

        #region Public Indexers

        /// <summary>
        /// Retorna a face correspondente ao numero
        /// </summary>
        /// <param name="FaceNumber">Numero da face</param>
        /// <returns></returns>
        public DiceFace this[int FaceNumber]
        {
            get
            {
                if (FaceNumber.IsBetweenOrEqual(1, Faces.Count))
                    return Faces[FaceNumber - 1];
                return null;
            }
        }

        #endregion Public Indexers

        #region Public Properties

        public static Dice Coin => _coin;

        public static Dice D6 => _d6;

        /// <summary>
        /// Faces do dado
        /// </summary>
        /// <returns>Um array com a cópia das faces do dado</returns>
        public ReadOnlyCollection<DiceFace> Faces { get; private set; }

        /// <summary>
        /// Historico de valores rolados para este dado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(int Value, DateTime TimeStamp)> History => Faces.SelectMany(x => x.History.Select(y => (x.Number, y))).OrderByDescending(x => x.y).AsEnumerable();

        /// <summary>
        /// Indica se o dado é um dado com faces customizadas
        /// </summary>
        /// <returns></returns>
        public bool IsCustom => Type == DiceType.Custom;

        /// <summary>
        /// Verifica se o dado possui algum lado viciado
        /// </summary>
        /// <returns></returns>
        public bool IsVicious => Faces.AsEnumerable().Any(x => x.IsVicious);

        /// <summary>
        /// Ultima vez que este dado foi rolado
        /// </summary>
        public DateTime? LastRoll => History.Any() ? History.FirstOrDefault().TimeStamp : (DateTime?)default;

        /// <summary>
        /// Se TRUE, Impede este dado de ser rolado
        /// </summary>
        /// <returns></returns>
        public bool Locked { get; set; } = false;

        /// <summary>
        /// Numero de vezes que este dado já foi rolado
        /// </summary>
        /// <returns>Integer</returns>
        public int RolledTimes => _rolledtimes;

        /// <summary>
        /// Retona o nome da face do ultimo valor
        /// </summary>
        public string TextValue => Value.HasValue ? GetFace(Value.Value)?.FaceName : null;

        /// <summary>
        /// Tipo do dado
        /// </summary>
        /// <returns></returns>
        public DiceType Type
        {
            get
            {
                foreach (var i in Util.GetEnumValues<DiceType>())
                    if ((int)i == Faces.Count) return i;
                return DiceType.Custom;
            }
        }

        /// <summary>
        /// Valor atual deste dado
        /// </summary>
        /// <returns></returns>
        public int? Value => History.Any() ? History.FirstOrDefault().Value : (int?)default;

        /// <summary>
        /// Peso do dado
        /// </summary>
        /// <returns></returns>
        public decimal Weight
        {
            get => Faces.Sum(x => x.Weight);

            set => NormalizeWeight(value / Faces.Count);
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Combina 2 dados em um DiceRoller
        /// </summary>
        /// <param name="Dice1">Dado 1</param>
        /// <param name="Dice2">Dado 2</param>
        /// <returns></returns>
        public static DiceRoller operator +(Dice Dice1, Dice Dice2) => new DiceRoller(Dice1, Dice2);

        /// <summary>
        /// Se este Dice for uma moeda (2 lados apenas) retorna true ou false baseado no lado da
        /// moeda qua saiu, caso seja um dado com mais de 2 lados retorna sempre true
        /// </summary>
        /// <returns></returns>
        public bool Flip(int Times = 1) => Faces.Count != 2 || (Roll(Times).Number - 1).ToBool();

        /// <summary>
        /// Retorna a porcentagem de chance de uma Face ser sorteada
        /// </summary>
        /// <param name="Face"></param>
        /// <param name="Precision"></param>
        /// <returns></returns>
        public decimal GetChancePercent(int Face, int Precision = 2) => Math.Round(GetFace(Face).Weight.CalculatePercent(Weight), Precision);

        /// <summary>
        /// Retorna a face correspondente ao numero
        /// </summary>
        /// <param name="FaceNumber">Numero da face</param>
        /// <returns></returns>
        public DiceFace GetFace(int FaceNumber = 0) => this[FaceNumber.LimitRange(1, Faces.Count)];

        /// <summary>
        /// Retorna o valor de chance de uma Face ser sorteada
        /// </summary>
        /// <param name="Face"></param>
        /// <param name="Precision"></param>
        /// <returns></returns>
        public decimal GetValueOfPercent(int Face, int Precision = 2) => Math.Round(GetFace(Face).WeightPercent.CalculateValueFromPercent(Weight), Precision);

        /// <summary>
        /// Carrega o historio que roladas deste dado
        /// </summary>
        /// <param name="history"></param>
        public void LoadHistory(IEnumerable<(int, DateTime)> history)
        {
            foreach (var item in history ?? Array.Empty<(int, DateTime)>())
            {
                if (!this[item.Item1]._h.Contains(item.Item2))
                {
                    this[item.Item1]._h.Add(item.Item2);
                }
            }
        }

        /// <summary>
        /// Normaliza o peso das faces do dado
        /// </summary>
        public void NormalizeWeight(decimal Weight = 1m)
        {
            Weight = Weight.SetMinValue(1m);
            foreach (DiceFace f in Faces)
                f.Weight = Weight;
        }

        /// <summary>
        /// Rola o dado e retorna seu valor
        /// </summary>
        /// <returns>Integer</returns>
        public DiceFace Roll(int Times = 1)
        {
            Times = Times.SetMinValue(1);
            while (Times > 0)
            {
                Times--;
                if (!Locked)
                {
                    _rolledtimes++;
                    var numfaces = new List<DiceFace>();
                    foreach (var f in Faces)
                    {
                        for (decimal index = 1m, loopTo = f.Weight; index <= loopTo; index++)
                            numfaces.Add(f);
                    }

                    numfaces[Util.RandomInt(1, numfaces.Count) - 1]._h.Add(DateTime.Now);
                }
            }

            return this[Value.Value];
        }

        /// <summary>
        /// Seta o nome da face deste dado
        /// </summary>
        /// <param name="FaceNumber"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Dice SetFaceName(int FaceNumber, string Name)
        {
            GetFace(FaceNumber).FaceName = Name;
            return this;
        }

        public override string ToString() => $"{Type}{$"{Value}".PrependIf(" - ", x => x.IsValid())}";

        #endregion Public Methods

        #region Public Classes

        /// <summary>
        /// Face de um dado. Pode ser viciada ou não
        /// </summary>
        public class DiceFace
        {
            #region Private Fields

            private string _name = null;

            #endregion Private Fields

            #region Protected Internal Fields

            protected internal decimal _weightpercent = 1m;

            #endregion Protected Internal Fields

            #region Internal Fields

            internal List<DateTime> _h = new List<DateTime>();

            internal decimal _weight = 1m;

            internal Dice dice = null;

            #endregion Internal Fields

            #region Internal Constructors

            internal DiceFace(Dice d)
            {
                dice = d;
            }

            #endregion Internal Constructors

            #region Public Properties

            /// <summary>
            /// Objeto do dado desta face
            /// </summary>
            public Dice Dice => dice;

            public string FaceName
            {
                get
                {
                    if (_name.IsNotValid())
                    {
                        if (dice.Type == DiceType.Coin)
                        {
                            _name = (Number == 1 ? "heads" : "tails").ToTitle();
                        }
                        else
                        {
                            _name = new FullNumberWriter().ToString(Number, 0).ToTitle();
                        }
                    }

                    return _name;
                }

                set
                {
                    _name = value;
                }
            }

            public IEnumerable<DateTime> History => _h.OrderByDescending(x => x).AsEnumerable();

            /// <summary>
            /// Valor que indica se a face está viciada
            /// </summary>
            /// <returns></returns>
            public bool IsVicious => OtherFaces().Select(x => x.WeightPercent).Distinct().All(x => x != WeightPercent);

            /// <summary>
            /// Valor Da Face (numero)
            /// </summary>
            /// <returns></returns>
            public int Number => dice.Faces.IndexOf(this) + 1;

            /// <summary>
            /// Peso da face (vicia o dado)
            /// </summary>
            /// <returns></returns>
            public decimal Weight
            {
                get => _weight;

                set
                {
                    _weight = value; // .LimitRange(1, dice.Faces.Count - 1)
                    dice.ApplyPercent();
                }
            }

            /// <summary>
            /// Porcentagem do peso da face (vicia o dado)
            /// </summary>
            /// <returns></returns>
            public decimal WeightPercent
            {
                get => _weightpercent;

                set
                {
                    value = value.LimitRange(0, 100);
                    decimal total_peso = dice.Weight;
                    decimal total_antigo = dice.Weight;
                    decimal peso_outros = OtherFaces().Sum(x => x.Weight);
                    _weight = value.CalculateValueFromPercent(total_peso);
                    total_peso = _weight + peso_outros;
                    _weightpercent = value;
                    foreach (var item in OtherFaces())
                        item._weightpercent = item._weight.CalculatePercent(total_peso);
                    foreach (var item in dice.Faces)
                        item._weight = item.WeightPercent.CalculateValueFromPercent(total_antigo);
                    dice.ApplyPercent();
                }
            }

            #endregion Public Properties

            #region Public Methods

            public static implicit operator int(DiceFace v) => v.Number;

            public IEnumerable<DiceFace> OtherFaces() => dice.Faces.Where(x => x.Number != Number);

            public override string ToString() => FaceName;

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    /// <summary>
    /// Combinação de varios dados de RPG que podem ser rolados ao mesmo tempo
    /// </summary>
    public class DiceRoller : List<Dice>
    {
        #region Public Constructors

        /// <summary>
        /// Cria uma nova combinação de Dados
        /// </summary>
        /// <param name="Dices">Dados de RPG</param>
        public DiceRoller(params Dice[] Dices)
        {
            AddRange(Dices);
        }

        /// <summary>
        /// Cria uma nova combinação de Dados
        /// </summary>
        /// <param name="DiceRollers">Dados de RPG</param>
        public DiceRoller(params DiceRoller[] DiceRollers)
        {
            foreach (var d in DiceRollers) AddRange(d.ToArray());
        }

        /// <summary>
        /// Cria uma nova combinação de Dados
        /// </summary>
        /// <param name="ListOfDices">Lista de dados de RPG</param>
        public DiceRoller(params List<Dice>[] ListOfDices)
        {
            foreach (var item in ListOfDices)
            {
                AddRange(item);
            }
        }

        /// <summary>
        /// Cria uma nova combinação de novos dados a criados a partir de varios tipos
        /// </summary>
        /// <param name="DiceType">Tipos Dados de RPG</param>
        public DiceRoller(params DiceType[] DiceType)
        {
            foreach (var d in DiceType) Add(new Dice(d));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Retorna a soma de todos os valores dos dados
        /// </summary>
        /// <returns>Integer</returns>
        public int Value => this.Sum(x => x.Value).ToInt();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Combina um dado com DiceRoller
        /// </summary>
        /// <param name="Combo">Dado 1</param>
        /// <param name="Dice">Dado 2</param>
        /// <returns></returns>
        public static DiceRoller operator +(DiceRoller Combo, Dice Dice)
        {
            var s = new DiceRoller(Combo.ToArray())
            {
                Dice
            };
            return s;
        }

        /// <summary>
        /// Combina um dado com DiceRoller
        /// </summary>
        /// <param name="Combo">Dado 1</param>
        /// <param name="Dice">Dado 2</param>
        /// <returns></returns>
        public static DiceRoller operator +(Dice Dice, DiceRoller Combo) => Combo + Dice;

        /// <summary>
        /// Combina um dado com DiceRoller
        /// </summary>
        /// <param name="Combo1">Combo de Dados 1</param>
        /// <param name="Combo2">Combo de Dados 2</param>
        /// <returns></returns>
        public static DiceRoller operator +(DiceRoller Combo1, DiceRoller Combo2) => new DiceRoller(Combo1, Combo2);

        /// <summary>
        /// Rola todos os dados (não travados) e retorna a soma de seus valores
        /// </summary>
        /// <returns>Retorna a soma de todos os valores dos dados após a rolagem</returns>
        public IEnumerable<Dice.DiceFace> Roll(int Times = 1) => this.Select(x => x.Roll(Times.SetMinValue(1)));

        #endregion Public Methods
    }

    /// <summary>
    /// Tipos de Dados
    /// </summary>
    public enum DiceType
    {
        /// <summary>
        /// Dado customizado
        /// </summary>
        Custom = 0,

        /// <summary>
        /// Moeda
        /// </summary>
        Coin = 2,

        /// <summary>
        /// Dado de 4 Lados (Tetraedro/Pirâmide)
        /// </summary>
        D4 = 4,

        /// <summary>
        /// Dado de 6 Lados (Pentalátero/Cubo/Dado Tradicional)
        /// </summary>
        D6 = 6,

        /// <summary>
        /// Dado de 8 Lados (Octaedro)
        /// </summary>
        D8 = 8,

        /// <summary>
        /// Dado de 10 Lados (Decaedro)
        /// </summary>
        D10 = 10,

        /// <summary>
        /// Dado de 12 Lados (Dodecaedro)
        /// </summary>
        D12 = 12,

        /// <summary>
        /// Dado de 20 Lados (Icosaedro)
        /// </summary>
        D20 = 20,

        /// <summary>
        /// Dado de 100 Lados (Esfera/Bola - Particulamente util para porcentagem)
        /// </summary>
        D100 = 100
    }
}