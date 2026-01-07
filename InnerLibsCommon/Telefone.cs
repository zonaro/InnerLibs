using System.Collections.Generic;
using System.Linq;

namespace Extensions.BR
{
    /// <summary>
    /// Classe que representa um n�mero de telefone.
    /// </summary>
    public struct Telefone
    {

        public static IEnumerable<int> DDDs => new[]{
           11, 12, 13, 14, 15, 16, 17, 18, 19,
           21, 22, 24, 27, 28,
           31, 32, 33, 34, 35, 37, 38,
           41, 42, 43, 44, 45, 46, 47, 48, 49,
           51, 53, 54, 55,
           61, 62, 63, 64, 65, 66, 67, 68, 69,
           71, 73, 74, 75, 77, 79,
           81, 82, 83, 84, 85, 86, 87, 88, 89,
           91, 92, 93, 94, 95, 96, 97, 98, 99
        };


        /// <summary>
        /// Código de Discagem Direta à Distância (DDD).
        /// </summary>
        public int? DDD { get; set; }

        /// <summary>
        /// Prefixo do número de telefone.
        /// </summary>
        public int Prefixo { get; private set; }

        /// <summary>
        /// Sufixo do número de telefone.
        /// </summary>
        public int Sufixo { get; private set; }


        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Telefone"/> com um numero longo.
        /// </summary>
        /// <param name="numero">Numero de telefone.</param>
        public Telefone(long numero) : this(numero.ToString())
        { }

        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Telefone"/> com DDD e numero.
        /// </summary>
        /// <param name="ddd">Código de Discagem Direta à Distância (DDD).</param>
        /// <param name="numero">Número de telefone.</param>
        public Telefone(int numero, int? ddd = null) : this(numero.ToString(), ddd?.ToString())
        { }



        /// <summary>
        /// Inicializa uma nova instancia da classe <see cref="Telefone"/> com um numero de telefone.
        /// </summary>
        /// <param name="ddd">Código de Discagem Direta à Distância (DDD).</param>
        /// <param name="numero">Número de telefone.</param>
        /// <remarks>O numero de telefone pode possuir um DDD. neste caso, o parâmetro <paramref name="ddd"/> será ignorado.</remarks>
        /// </remarks>
        /// <exception cref="System.ArgumentException">Lançada quando o número de telefone é inválido.</exception>
        public Telefone(string numero, string ddd = null)
        {
            if (Brasil.TelefoneValido(numero))
            {
                numero = numero.OnlyNumbers();
                ddd = ddd.OnlyNumbers();
                if (numero.Length > 11)
                {
                    numero = numero.GetLastChars(11);
                }

                var c = new string[] { };

                if (numero.Length == 11)
                {
                    c = numero.SplitChunk(2, 5, 4).ToArray();
                }
                else if (numero.Length == 10)
                {

                    c = numero.SplitChunk(2, 4, 4).ToArray();
                }
                else if (numero.Length == 9)
                {
                    c = numero.SplitChunk(0, 5, 4).ToArray();
                    c[0] = ddd;

                }
                else if (numero.Length == 8)
                {
                    c = numero.SplitChunk(0, 4, 4).ToArray();
                    c[0] = ddd;

                }

                DDD = c[0].ToInt();
                Prefixo = c[1].ToInt();
                Sufixo = c[2].ToInt();
            }
            else
            {
                throw new System.ArgumentException("Número de telefone inválido.", nameof(numero));
            }
        }

        /// <summary>
        /// Verifica se este telefone possui nono digito
        /// </summary>
        public bool NonoDigito { get => Prefixo.Length(5); set => Prefixo = ((value ? "9" : "") + Prefixo.ToStringInvariant().GetLastChars(4)).ToInt(); }

        /// <summary>
        /// Verifica se o número de telefone é válido.
        /// </summary>
        public bool Valido => Brasil.TelefoneValido(Completo);

        public bool DDDValido => DDD.HasValue && DDDs.Contains(DDD.Value);

        /// <summary>
        /// Retorna o n�mero de telefone completo, incluindo o DDD.
        /// </summary>
        public string Completo => DDDValido ? $"{DDD}{Numero}" : Numero;

        /// <summary>
        /// Retorna o n�mero de telefone completo, incluindo o DDD, formatado com m�scara.
        /// </summary>
        public string CompletoMascara => DDDValido ? $"({DDD}) {NumeroMascara}" : NumeroMascara;

        /// <summary>
        /// Retorna o n�mero de telefone.
        /// </summary>
        public string Numero => $"{Prefixo}{Sufixo.FixedLength(4)}";

        /// <summary>
        /// Retorna o n�mero de telefone formatado com m�scara.
        /// </summary>
        public string NumeroMascara => $"{Prefixo}-{Sufixo}";

        /// <summary>
        /// Compara se dois números de telefone são iguais.
        /// </summary>
        /// <param name="obj">Objeto a ser comparado.</param>
        /// <returns>Retorna <c>true</c> se os números de telefone forem iguais; caso contrário, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Telefone telefone)
            {
                return Completo == telefone.Completo;
            }
            if (obj is string str)
            {
                return Completo == new Telefone(str).Completo;
            }
            if (obj is int num)
            {
                return Completo == new Telefone(num).Completo;
            }

            return false;
        }

        /// <summary>
        /// Retorna uma representação em string do número de telefone.
        /// </summary>
        /// <returns>Representação em string do número de telefone.</returns>
        public override string ToString() => CompletoMascara;

        /// <summary>
        /// Convers�o impl�cita de <see cref="Telefone"/> para <see cref="string"/>.
        /// </summary>
        /// <param name="telefone">Inst�ncia de <see cref="Telefone"/>.</param>
        public static implicit operator string(Telefone telefone) => telefone.ToString();

        /// <summary>
        /// Convers�o impl�cita de <see cref="Telefone"/> para <see cref="long"/>.
        /// </summary>
        /// <param name="telefone">Inst�ncia de <see cref="Telefone"/>.</param>
        public static implicit operator long(Telefone telefone) => telefone.Completo.OnlyNumbersLong();

        /// <summary>
        /// Convers�o impl�cita de <see cref="Telefone"/> para <see cref="int"/>.
        /// </summary>
        /// <param name="telefone">Inst�ncia de <see cref="Telefone"/>.</param>
        public static implicit operator int(Telefone telefone) => telefone.Completo.OnlyNumbersInt();

        /// <summary>
        /// Conversão implícita de <see cref="string"/> para <see cref="Telefone"/>.
        /// </summary>
        /// <param name="telefone">Número de telefone em formato de string.</param>
        public static implicit operator Telefone(string telefone) => new Telefone(telefone);

        /// <summary>
        /// Conversão implícita de <see cref="int"/> para <see cref="Telefone"/>.
        /// </summary>
        /// <param name="telefone">Número de telefone em formato de inteiro.</param>
        public static implicit operator Telefone(int telefone) => new Telefone(telefone);

        /// <summary>
        /// Conversão implícita de <see cref="long"/> para <see cref="Telefone"/>.
        /// </summary>
        /// <param name="telefone">Número de telefone em formato de longo.</param>
        public static implicit operator Telefone(long telefone) => new Telefone(telefone);
    }
}
