using System;
using System.Data;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    public class RuleOfThree
    {
        /// <summary>
        /// Calcula uma regra de tres
        /// </summary>
        /// <param name="FirstEquation"></param>
        /// <param name="SecondEquation"></param>
        public RuleOfThree(EquationPair FirstEquation, EquationPair SecondEquation)
        {
            this.FirstEquation = FirstEquation;
            this.SecondEquation = SecondEquation;
            GetExpression();
        }

        private void GetExpression()
        {
            if (Conversions.ToBoolean(FirstEquation.IsNotComplete && SecondEquation.IsComplete))
            {
                if (Conversions.ToBoolean(FirstEquation.MissX))
                {
                    equationexp = new Func<decimal?>(() => FirstEquation.Y * SecondEquation.X / SecondEquation.Y);
                    paramname = "FX";
                }
                else
                {
                    equationexp = new Func<decimal?>(() => FirstEquation.X * SecondEquation.Y / SecondEquation.X);
                    paramname = "FY";
                }
            }
            else if (Conversions.ToBoolean(FirstEquation.IsComplete && SecondEquation.IsNotComplete))
            {
                if (Conversions.ToBoolean(SecondEquation.MissX))
                {
                    equationexp = new Func<decimal?>(() => SecondEquation.Y * FirstEquation.X / FirstEquation.Y);
                    paramname = "SX";
                }
                else
                {
                    equationexp = new Func<decimal?>(() => SecondEquation.X * FirstEquation.Y / FirstEquation.X);
                    paramname = "SY";
                }
            }
            else
            {
                equationexp = new Func<decimal?>(() => default);
                paramname = null;
            }
        }

        /// <summary>
        /// Atualiza o campo nulo da <see cref="EquationPair"/> correspondente pelo <see cref="UnknownValue"/>
        /// </summary>
        /// <returns></returns>
        public RuleOfThree Resolve()
        {
            GetExpression();
            if (Conversions.ToBoolean(FirstEquation.IsComplete && SecondEquation.IsNotComplete))
            {
                SecondEquation.SetMissing((decimal)UnknownValue);
            }
            else if (Conversions.ToBoolean(SecondEquation.IsComplete && FirstEquation.IsNotComplete))
            {
                FirstEquation.SetMissing((decimal)UnknownValue);
            }

            return this;
        }

        /// <summary>
        /// Calcula uma regra de três
        /// </summary>
        public RuleOfThree(params decimal?[] Numbers) => RuleExpression(Numbers);

        private void RuleExpression(params decimal?[] numbers)
        {
            numbers ??= Array.Empty<decimal?>();

            if (numbers.Count() < 3)
            {
                throw new NoNullAllowedException("Three numbers need to be known to make a rule of three");
            }
            else if (numbers.Count() == 3)
            {
                FirstEquation.X = numbers[0];
                FirstEquation.Y = numbers[1];
                SecondEquation.X = numbers[2];
                SecondEquation.Y = default;

            }
            else if (numbers.All(x => x.HasValue))
            {
                throw new NoNullAllowedException("One of numbers must be NULL");
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

        public decimal? UnknownValue => equationexp();

        public string UnknownName => custom_param_name.IfBlank(paramname);

        private Func<decimal?> equationexp;
        private string paramname;
        private string custom_param_name;

        public RuleOfThree(string Equation)
        {
            var eqs = Equation.SplitAny(";", Environment.NewLine).Take(2);
            var e1 = eqs.First().Split("=");
            var e2 = eqs.Last().Split("=");
            string e1xs = e1.FirstOrDefault()?.AdjustBlankSpaces();
            string e1ys = e1.LastOrDefault()?.AdjustBlankSpaces();
            string e2xs = e2.FirstOrDefault()?.AdjustBlankSpaces();
            string e2ys = e2.LastOrDefault()?.AdjustBlankSpaces();
            decimal? e1x = default;
            if (e1xs.IsNumber())
                e1x = Convert.ToDecimal(e1xs);
            else
                custom_param_name = e1xs;
            decimal? e1y = default;
            if (e1ys.IsNumber())
                e1y = Convert.ToDecimal(e1ys);
            else
                custom_param_name = e1ys;
            decimal? e2x = default;
            if (e2xs.IsNumber())
                e2x = Convert.ToDecimal(e2xs);
            else
                custom_param_name = e2xs;
            decimal? e2y = default;
            if (e2ys.IsNumber())
                e2y = Convert.ToDecimal(e2ys);
            else
                custom_param_name = e2ys;
            RuleExpression(e1x, e1y, e2x, e2y);
        }

        /// <summary>
        /// Primeira Equação
        /// </summary>
        /// <returns></returns>
        public EquationPair FirstEquation { get; private set; } = new EquationPair();

        /// <summary>
        /// Segunda Equação
        /// </summary>
        /// <returns></returns>
        public EquationPair SecondEquation { get; private set; } = new EquationPair();

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
    }
}