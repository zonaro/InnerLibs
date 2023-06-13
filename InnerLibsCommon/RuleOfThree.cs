using System;
using System.Data;
using System.Linq;
using System.Reflection;


namespace Extensions.Equations
{

    /// <summary>
    /// Representa um Par X,Y para operaçoes matemáticas
    /// </summary>
    public class EquationPair<T> where T : struct
    {
        #region Public Constructors

        public EquationPair(T? X, T? Y)
        {
            this.X = X;
            this.Y = Y;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool IsComplete => X.HasValue && Y.HasValue;
        public bool IsNotComplete => !IsComplete;
        public bool MissX => !X.HasValue;
        public bool MissY => !Y.HasValue;
        public T? X { get; set; }
        public T? Y { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static explicit operator EquationPair<T>(Tuple<T?, T?> Equation) => new EquationPair<T>(Equation.Item1, Equation.Item2);

        public static explicit operator Tuple<T?, T?>(EquationPair<T> Equation) => new Tuple<T?, T?>(Equation.X, Equation.Y);

        public PropertyInfo GetMissing()
        {
            if (MissX)
            {
                return this.GetProperty("X");
            }

            if (MissY)
            {
                return this.GetProperty("Y");
            }

            return null;
        }

        public void SetMissing(decimal value) => GetMissing()?.SetValue(this, value);

        public T?[] ToArray() => new[] { X, Y };

        #endregion Public Methods
    }


    public class RuleOfThree
    {
        #region Private Fields

        private string custom_param_name;

        private Func<decimal?> equationexp;

        private string paramname;

        #endregion Private Fields

        #region Private Methods

        private void GetExpression()
        {
            if (FirstEquation.IsNotComplete && SecondEquation.IsComplete)
            {
                if (FirstEquation.MissX)
                {
                    equationexp = new Func<decimal?>(() => (FirstEquation.Y.ToDecimal() * SecondEquation.X.ToDecimal() / SecondEquation.Y.ToDecimal()).ChangeType<decimal>());
                    paramname = "FX";
                }
                else
                {
                    equationexp = new Func<decimal?>(() => (FirstEquation.X.ToDecimal() * SecondEquation.Y.ToDecimal() / SecondEquation.X.ToDecimal()).ChangeType<decimal>());
                    paramname = "FY";
                }
            }
            else if (FirstEquation.IsComplete && SecondEquation.IsNotComplete)
            {
                if (SecondEquation.MissX)
                {
                    equationexp = new Func<decimal?>(() => (SecondEquation.Y.ToDecimal() * FirstEquation.X.ToDecimal() / FirstEquation.Y.ToDecimal()).ChangeType<decimal>());
                    paramname = "SX";
                }
                else
                {
                    equationexp = new Func<decimal?>(() => (SecondEquation.X.ToDecimal() * FirstEquation.Y.ToDecimal() / FirstEquation.X.ToDecimal()).ChangeType<decimal>());
                    paramname = "SY";
                }
            }
            else
            {
                equationexp = new Func<decimal?>(() => UnknownValue);
                paramname = null;
            }
        }

        internal void RuleExpression(params decimal?[] numbers)
        {
            FirstEquation = FirstEquation ?? new EquationPair<decimal>(default, default);
            SecondEquation = SecondEquation ?? new EquationPair<decimal>(default, default);

            numbers = numbers ?? Array.Empty<decimal?>();

            if (numbers.Length < 3)
            {
                throw new NoNullAllowedException("Three numbers need to be known to make a rule of three");
            }
            else if (numbers.Length == 3)
            {
                FirstEquation.X = numbers[0];
                FirstEquation.Y = numbers[1];
                SecondEquation.X = numbers[2];
                SecondEquation.Y = default;
            }
            else if (numbers.All(x => x.HasValue))
            {
                throw new ArgumentException("One of numbers must be NULL");
            }
            else if (numbers.Count(x => x.HasValue) < 3)
            {
                throw new NoNullAllowedException("Three numbers need to be known to make a rule of three");
            }
            else
            {
                FirstEquation.X = numbers.IfNoIndex(0);
                FirstEquation.Y = numbers.IfNoIndex(1);
                SecondEquation.X = numbers.IfNoIndex(2);
                SecondEquation.Y = numbers.IfNoIndex(3);
                GetExpression();
                Resolve();
            }
        }

        #endregion Private Methods

        #region Public Constructors

        /// <summary>
        /// Calcula uma regra de tres
        /// </summary>
        /// <param name="FirstEquation"></param>
        /// <param name="SecondEquation"></param>
        public RuleOfThree(EquationPair<decimal> FirstEquation, EquationPair<decimal> SecondEquation)
        {
            this.FirstEquation = FirstEquation;
            this.SecondEquation = SecondEquation;
            GetExpression();
        }

        /// <summary>
        /// Calcula uma regra de três
        /// </summary>
        public RuleOfThree(params decimal?[] Numbers) => RuleExpression(Numbers);

        public RuleOfThree(string Equation)
        {
            var eqs = Equation.SplitAny(";", Environment.NewLine).Take(2);
            var e1 = eqs.First().Split("=");
            var e2 = eqs.Last().Split("=");
            string e1xs = e1.FirstOrDefault()?.TrimBetween();
            string e1ys = e1.LastOrDefault()?.TrimBetween();
            string e2xs = e2.FirstOrDefault()?.TrimBetween();
            string e2ys = e2.LastOrDefault()?.TrimBetween();
            decimal? e1x = default;
            if (e1xs.IsNumber())
                e1x = Util.ChangeType<decimal>(e1xs);
            else
                custom_param_name = e1xs;
            decimal? e1y = default;
            if (e1ys.IsNumber())
                e1y = Util.ChangeType<decimal>(e1ys);
            else
                custom_param_name = e1ys;
            decimal? e2x = default;
            if (e2xs.IsNumber())
                e2x = Util.ChangeType<decimal>(e2xs);
            else
                custom_param_name = e2xs;
            decimal? e2y = default;
            if (e2ys.IsNumber())
                e2y = Util.ChangeType<decimal>(e2ys);
            else
                custom_param_name = e2ys;
            RuleExpression(e1x, e1y, e2x, e2y);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Primeira Equação
        /// </summary>
        /// <returns></returns>
        public EquationPair<decimal> FirstEquation { get; private set; }

        /// <summary>
        /// Segunda Equação
        /// </summary>
        /// <returns></returns>
        public EquationPair<decimal> SecondEquation { get; private set; }

        public string UnknownName => custom_param_name.IfBlank(paramname);

        public decimal? UnknownValue => equationexp();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Atualiza o campo nulo da <see cref="EquationPair"/> correspondente pelo <see cref="UnknownValue"/>
        /// </summary>
        /// <returns></returns>
        public RuleOfThree Resolve()
        {
            GetExpression();
            if (FirstEquation.IsComplete && SecondEquation.IsNotComplete)
            {
                SecondEquation.SetMissing(UnknownValue ?? default);
            }
            else if (SecondEquation.IsComplete && FirstEquation.IsNotComplete)
            {
                FirstEquation.SetMissing(UnknownValue ?? default);
            }

            return this;
        }

        public decimal?[][] ToArray() => new[] { FirstEquation.ToArray(), SecondEquation.ToArray() };

        public decimal?[] ToFlatArray() => FirstEquation.ToArray().Union(SecondEquation.ToArray()).ToArray();

        public override string ToString()
        {
            if (UnknownValue != null)
            {
                return $"{UnknownName} = {UnknownValue}";
            }

            return null;
        }

        #endregion Public Methods
    }
}
