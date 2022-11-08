using System;

namespace InnerLibs
{
    /// <summary>
    /// Classe para escrever moedas por extenso com suporte até 999 quintilhoes de $$
    /// </summary>
    public class FullMoneyWriter : FullNumberWriter
    {
        /// <summary>
        /// Par de strings que representam os centavos desta moeda em sua forma singular ou plural
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair CurrencyCentsName { get; set; } = new QuantityTextPair("cents", "cent");

        /// <summary>
        /// Par de strings que representam os nomes da moeda em sua forma singular ou plural
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair CurrencyName { get; set; } = new QuantityTextPair("dollars", "dollar");

        /// <summary>
        /// Escreve um numero por extenso
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public override string ToString(decimal Number, int DecimalPlaces = 2)
        {
            long dec = Number.GetDecimalPart(DecimalPlaces.LimitRange(0, 3));
            long num = (long)Math.Round(Number.Floor());
            return (InExtensive(num) + CurrencyCentsName[num].Wrap(" ") + (dec == 0L | DecimalPlaces == 0 ? InnerLibs.Text.Empty : And.Wrap(" ") + InExtensive(dec) + CurrencyCentsName[dec].Wrap(" "))).ToLower().TrimBetween();
        }

        public override string ToString() => ToString(0m);
    }
}