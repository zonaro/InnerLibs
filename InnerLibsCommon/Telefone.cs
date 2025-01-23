using System.Text.RegularExpressions;

namespace Extensions.BR
{

    /// Classe que representa um n�mero de telefone.
    public class Telefone
    {
        public string DDD { get; set; }
        public string Prefixo { get; set; }
        public string Sufixo { get; set; }


        public Telefone(int numero) => new Telefone(numero.ToString());
        public Telefone(long numero) => new Telefone(numero.ToString());
        public Telefone(int ddd, int numero) => new Telefone(ddd.ToString(), numero.ToString());

        public Telefone(string ddd, string numero) => new Telefone(ddd + numero);


        /// Construtor da classe Telefone.
        ///
        /// [numero] � um par�metro (int ou string) que representa o n�mero de telefone.
        /// Se [numero] for fornecido e for v�lido, o n�mero ser� formatado e
        /// atribu�do �s propriedades [DDD], [Prefixo] e [Sufixo].
        public Telefone(string numero)
        {
            DDD = "";
            Prefixo = "";
            Sufixo = "";

            if (Brasil.ValidarTelefone(numero))
            {
                string t = Regex.Replace(numero.ToString(), @"\D", "");
                if (t.Length > 11)
                {
                    t = t.Substring(0, 11);
                }

                if (t.Length == 11)
                {
                    DDD = t.Substring(0, 2);
                    Prefixo = t.Substring(2, 7);
                    Sufixo = t.Substring(7, 4);
                }
                else if (t.Length == 10)
                {
                    DDD = t.Substring(0, 2);
                    Prefixo = t.Substring(2, 6);
                    Sufixo = t.Substring(6, 4);
                }
                else if (t.Length == 9)
                {
                    Prefixo = t.Substring(0, 5);
                    Sufixo = t.Substring(5, 4);
                }
                else if (t.Length == 8)
                {
                    Prefixo = t.Substring(0, 4);
                    Sufixo = t.Substring(4, 4);
                }
            }
            else
            {
                throw new System.ArgumentException("N�mero de telefone inv�lido.", nameof(numero));
            }
        }

        /// Retorna o n�mero de telefone completo, incluindo o DDD.
        public string Completo => $"{DDD}{Numero}";

        /// Retorna o n�mero de telefone completo, incluindo o DDD, formatado com m�scara.
        public string CompletoMascara => !string.IsNullOrEmpty(DDD) ? $"({DDD}) {NumeroMascara}" : NumeroMascara;


        /// Retorna o n�mero de telefone.
        public string Numero => $"{Prefixo}{Sufixo}";

        /// Retorna o n�mero de telefone formatado com m�scara.
        public string NumeroMascara => $"{Prefixo}-{Sufixo}";

        /// Compara se dois n�meros de telefone s�o iguais.
        public override bool Equals(object obj)
        {
            if (obj is Telefone telefone)
            {
                return Completo == telefone.Completo;
            }
            if (obj is string str)
            {
                return Completo == str;
            }
            if (obj is int num)
            {
                return Completo == num.ToString();
            }

            return false;
        }

        /// Retorna uma representa��o em string do n�mero de telefone.
        public override string ToString() => CompletoMascara;


        public static implicit operator string(Telefone telefone) => telefone.ToString();
        public static implicit operator long(Telefone telefone) => telefone.Completo.OnlyNumbersLong();
        public static implicit operator int(Telefone telefone) => telefone.Completo.OnlyNumbersInt();

        public static implicit operator Telefone(string telefone) => new Telefone(telefone);
        public static implicit operator Telefone(int telefone) => new Telefone(telefone);
        public static implicit operator Telefone(long telefone) => new Telefone(telefone);

    }

}
