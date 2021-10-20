using System;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{

    public class RuleOfThree : RuleOfThree<decimal>
    {

    }


    public class RuleOfThree<T> where T : struct
    {
        /// <summary>
        /// Calcula uma regra de tres
        /// </summary>
        /// <param name="FirstEquation"></param>
        /// <param name="SecondEquation"></param>
        public RuleOfThree(EquationPair<T> FirstEquation, EquationPair<T> SecondEquation)
        {
            this.FirstEquation = FirstEquation;
            this.SecondEquation = SecondEquation;
            GetExpression();
        }

        private void GetExpression()
        {
            if (FirstEquation.IsNotComplete && SecondEquation.IsComplete)
            {
                if (FirstEquation.MissX)
                {
                    equationexp = new Func<T?>(() => (FirstEquation.Y.ToDecimal() * SecondEquation.X.ToDecimal() / SecondEquation.Y.ToDecimal()).ChangeType<T>());
                    paramname = "FX";
                }
                else
                {
                    equationexp = new Func<T?>(() => (FirstEquation.X.ToDecimal() * SecondEquation.Y.ToDecimal() / SecondEquation.X.ToDecimal()).ChangeType<T>());
                    paramname = "FY";
                }
            }
            else if (FirstEquation.IsComplete && SecondEquation.IsNotComplete)
            {
                if (SecondEquation.MissX)
                {
                    equationexp = new Func<T?>(() => (SecondEquation.Y.ToDecimal() * FirstEquation.X.ToDecimal() / FirstEquation.Y.ToDecimal()).ChangeType<T>());
                    paramname = "SX";
                }
                else
                {
                    equationexp = new Func<T?>(() => (SecondEquation.X.ToDecimal() * FirstEquation.Y.ToDecimal() / FirstEquation.X.ToDecimal()).ChangeType<T>());
                    paramname = "SY";
                }
            }
            else
            {
                equationexp = new Func<T?>(() => UnknownValue);
                paramname = null;
            }
        }

        /// <summary>
        /// Atualiza o campo nulo da <see cref="EquationPair"/> correspondente pelo <see cref="UnknownValue"/>
        /// </summary>
        /// <returns></returns>
        public RuleOfThree<T> Resolve()
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

        /// <summary>
        /// Calcula uma regra de três
        /// </summary>
        public RuleOfThree(params T?[] Numbers) => RuleExpression(Numbers);

        private void RuleExpression(params T?[] numbers)
        {
            numbers ??= Array.Empty<T?>();

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

        public T? UnknownValue => equationexp();

        public string UnknownName => custom_param_name.IfBlank(paramname);

        private Func<T?> equationexp;
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
            T? e1x = default;
            if (e1xs.IsNumber())
                e1x = Converter.ChangeType<T>(e1xs);
            else
                custom_param_name = e1xs;
            T? e1y = default;
            if (e1ys.IsNumber())
                e1y = Converter.ChangeType<T>(e1ys);
            else
                custom_param_name = e1ys;
            T? e2x = default;
            if (e2xs.IsNumber())
                e2x = Converter.ChangeType<T>(e2xs);
            else
                custom_param_name = e2xs;
            T? e2y = default;
            if (e2ys.IsNumber())
                e2y = Converter.ChangeType<T>(e2ys);
            else
                custom_param_name = e2ys;
            RuleExpression(e1x, e1y, e2x, e2y);
        }

        /// <summary>
        /// Primeira Equação
        /// </summary>
        /// <returns></returns>
        public EquationPair<T> FirstEquation { get; private set; }

        /// <summary>
        /// Segunda Equação
        /// </summary>
        /// <returns></returns>
        public EquationPair<T> SecondEquation { get; private set; }

        public T?[][] ToArray() => new[] { FirstEquation.ToArray(), SecondEquation.ToArray() };

        public T?[] ToFlatArray() => FirstEquation.ToArray().Union(SecondEquation.ToArray()).ToArray();

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