using System;
using System.Collections;
using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{

    /// <summary>
/// Modulo para manipulação de numeros romanos
/// </summary>
/// <remarks></remarks>
    public static class Romanize
    {
        /// <summary>
    /// Lista de algarismos romanos
    /// </summary>

        public enum RomanDigit
        {
            /// <summary>
        /// Valor correspondente
        /// </summary>

            I = 1,
            /// <summary>
        /// Valor correspondente
        /// </summary>

            V = 5,
            /// <summary>
        /// Valor correspondente
        /// </summary>

            X = 10,
            /// <summary>
        /// Valor correspondente
        /// </summary>

            L = 50,
            /// <summary>
        /// Valor correspondente
        /// </summary>

            C = 100,
            /// <summary>
        /// Valor correspondente
        /// </summary>

            D = 500,
            /// <summary>
        /// Valor correspondente
        /// </summary>

            M = 1000
        }

        /// <summary>
    /// Converte uma String contendo um numero romano para seu valor arabico
    /// </summary>
    /// <param name="RomanNumber">Stringo contendo o numero romano</param>
    /// <returns>Valor em arabico</returns>

        public static int ToArabic(this string RomanNumber)
        {
            RomanNumber = RomanNumber.ToUpper().Trim();
            if (RomanNumber == "N")
            {
                return 0;
            }

            // Os numerais que representam números que começam com um '5'(V, L e D) podem
            // aparecer apenas uma vez em cada numeral romano. Esta regra permite XVI, mas não VIV.
            if (RomanNumber.Split('V').Length > 2 || RomanNumber.Split('L').Length > 2 || RomanNumber.Split('D').Length > 2)
            {
                throw new ArgumentException("Número romano com algarismos inválidos. O número possui um algarismo V,L ou D repetido.");
            }

            // Uma única letra pode ser repetida até três vezes consecutivamente sendo
            // que cada ocorrência será somanda. Isto significa que I é um, II e III
            // significa dois é três. No entanto, IIII não é permitido.
            int contador = 1;
            char ultimo = 'Z';
            foreach (char numeral in RomanNumber)
            {
                // caractere inválido ?
                if ("IVXLCDM".IndexOf(numeral) == -1)
                {
                    throw new ArgumentException("O algarismo com posicionamento inválido.");
                }

                // Duplicado?
                if (numeral == ultimo)
                {
                    contador += 1;
                    if (contador == 4)
                    {
                        throw new ArgumentException("Um algarismo romano não pode ser repetido mais de 3 vezes no mesmo número.");
                    }
                }
                else
                {
                    contador = 1;
                    ultimo = numeral;
                }
            }

            // Cria um ArrayList contendo os valores
            int ptr = 0;
            var valores = new ArrayList();
            int digitoMaximo = 1000;
            while (ptr < RomanNumber.Length)
            {
                // valor base do digito
                char numeral = RomanNumber[ptr];
                int digito = Conversions.ToInteger(Enum.Parse(typeof(RomanDigit), numeral.ToString()));

                // Um numeral de pequena valor pode ser colocado à esquerda de um valor maior
                // Quando isto ocorre, por exemplo IX, o menor número é subtraído do maior
                // O dígito subtraído deve ser de pelo menos um décimo do valor do maior numeral e deve ser ou I, X ou C
                // Valores como MCMD ou CMC não são permitidos
                if (digito > digitoMaximo)
                {
                    throw new ArgumentException("Algarísmo com posicionamento inválido.");
                }

                // proximo digito
                int proximoDigito = 0;
                if (ptr < RomanNumber.Length - 1)
                {
                    char proximoNumeral = RomanNumber[ptr + 1];
                    proximoDigito = Conversions.ToInteger(Enum.Parse(typeof(RomanDigit), proximoNumeral.ToString()));
                    if (proximoDigito > digito)
                    {
                        if ("IXC".IndexOf(numeral) == -1 || proximoDigito > digito * 10 || RomanNumber.Split(numeral).Length > 3)
                        {
                            throw new ArgumentException("Rule 3");
                        }

                        digitoMaximo = digito - 1;
                        digito = proximoDigito - digito;
                        ptr += 1;
                    }
                }

                valores.Add(digito);

                // proximo digito
                ptr += 1;
            }

            // Outra regra é a que compara o tamanho do valor de cada numeral lido a partir da esquerda para a direita.
            // O valor nunca deve aumentar a partir de uma letra para a próxima.
            // Onde houver um numeral subtractivo, esta regra se aplica ao valor
            // combinado dos dois algarismos envolvidos na subtração quando comparado com a letra anterior.
            // Isto significa que XIX é aceitável, mas XIM e IIV não são.
            for (int i = 0, loopTo = valores.Count - 2; i <= loopTo; i++)
            {
                if (Conversions.ToInteger(valores[i]) < Conversions.ToInteger(valores[i + 1]))
                {
                    throw new ArgumentException("Algarismo romano inválido. Neste caso o algarismo não pode ser maior que o anterior.");
                }
            }

            // Numerais maiores devem ser colocados à esquerda dos números menores para
            // continuar a combinação aditiva. Assim VI é igual a seis e MDCLXI é 1.661.
            int total = 0;
            foreach (int digito in valores)
                total += digito;
            return total;
        }

        /// <summary>
    /// Converte um valor numérico arabico para numero romano
    /// </summary>
    /// <param name="ArabicNumber">Valor numerico arabico</param>
    /// <returns>uma string com o numero romano</returns>

        public static string ToRoman(this int ArabicNumber)
        {
            // valida : aceita somente valores entre 1 e 3999
            if (ArabicNumber < 1 || ArabicNumber > 3999)
            {
                ArabicNumber = ArabicNumber.LimitRange(1, 3999);
                Debug.Write("O valor numérico deve estar entre 1 e 3999.", "ArabicNumber");
            }

            var algarismosArabicos = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            var algarismosRomanos = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

            // inicializa o string builder
            string resultado = "";

            // percorre os valores nos arrays
            for (int i = 0; i <= 12; i++)
            {
                // se o numero a ser convertido é menor que o valor então anexa
                // o numero correspondente ou o par ao resultado
                while (ArabicNumber >= algarismosArabicos[i])
                {
                    ArabicNumber -= algarismosArabicos[i];
                    resultado += algarismosRomanos[i];
                }
            }

            // retorna o resultado
            return resultado.ToString();
        }
    }
}