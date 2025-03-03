using System.Linq;

namespace Extensions.BR
{
    /// <summary>
    /// Classe que representa um número de telefone.
    /// </summary>
    public class Telefone
    {
        /// <summary>
        /// Código de Discagem Direta à Distância (DDD).
        /// </summary>
        public string DDD { get; set; }

        /// <summary>
        /// Prefixo do número de telefone.
        /// </summary>
        public string Prefixo { get; set; }

        /// <summary>
        /// Sufixo do número de telefone.
        /// </summary>
        public string Sufixo { get; set; }



        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Telefone"/> com um número longo.
        /// </summary>
        /// <param name="numero">Número de telefone.</param>
        public Telefone(long numero) => new Telefone(numero.ToString());

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Telefone"/> com DDD e número.
        /// </summary>
        /// <param name="ddd">Código de Discagem Direta à Distância (DDD).</param>
        /// <param name="numero">Número de telefone.</param>
        public Telefone(int numero, int? ddd = null) => new Telefone(numero.ToString(), ddd?.ToString());



        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Telefone"/> com um número de telefone.
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

                DDD = c[0];
                Prefixo = c[1];
                Sufixo = c[2];
            }
            else
            {
                throw new System.ArgumentException("Número de telefone inválido.", nameof(numero));
            }
        }

        /// <summary>
        /// Verifica se este telefone possui nono digito
        /// </summary>
        public bool NonoDigito { get => Prefixo.Length == 5; set => Prefixo = (value ? "9" : "") + Prefixo.GetLastChars(4); }

        public bool IsValid => Brasil.TelefoneValido(Completo);

        public bool HasValidDDD => DDD.IsNotBlank() && DDD.ToInt().IsBetween(11, 99);

        /// <summary>
        /// Retorna o número de telefone completo, incluindo o DDD.
        /// </summary>
        public string Completo => $"{DDD}{Numero}";

        /// <summary>
        /// Retorna o número de telefone completo, incluindo o DDD, formatado com máscara.
        /// </summary>
        public string CompletoMascara => HasValidDDD ? $"({DDD}) {NumeroMascara}" : NumeroMascara;

        /// <summary>
        /// Retorna o número de telefone.
        /// </summary>
        public string Numero => $"{Prefixo}{Sufixo}";

        /// <summary>
        /// Retorna o número de telefone formatado com máscara.
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
        /// Conversão implícita de <see cref="Telefone"/> para <see cref="string"/>.
        /// </summary>
        /// <param name="telefone">Instância de <see cref="Telefone"/>.</param>
        public static implicit operator string(Telefone telefone) => telefone.ToString();

        /// <summary>
        /// Conversão implícita de <see cref="Telefone"/> para <see cref="long"/>.
        /// </summary>
        /// <param name="telefone">Instância de <see cref="Telefone"/>.</param>
        public static implicit operator long(Telefone telefone) => telefone.Completo.OnlyNumbersLong();

        /// <summary>
        /// Conversão implícita de <see cref="Telefone"/> para <see cref="int"/>.
        /// </summary>
        /// <param name="telefone">Instância de <see cref="Telefone"/>.</param>
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
