using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Extensions;
using Extensions.Locations;

namespace Extensions.BR
{
    /// <summary>
    /// Objeto para manipular cidades e estados do Brasil
    /// </summary>
    public static class Brasil
    {
        private static List<Estado> l = new List<Estado>();

        /// <summary>
        /// Array contendo os nomes mais comuns no Brasil
        /// </summary>
        public static IEnumerable<string> NomesComuns => new[] { "Ellie", "Ellie Rose", "Miguel", "Arthur", "Davi", "Gabriel", "Pedro", "Alice", "Sophia", "Sofia", "Manuela", "Isabella", "Laura", "Heitor", "Enzo", "Lorenzo", "Valentina", "Giovanna", "Giovana", "Maria Eduarda", "Beatriz", "Maria Clara", "Vinícius", "Rafael", "Lara", "Mariana", "Helena", "Mariana", "Isadora", "Lívia", "Luana", "Maria Luíza", "Luiza", "Ana Luiza", "Eduarda", "Letícia", "Lara", "Melissa", "Maria Fernanda", "Cecília", "Lorena", "Clara", "Gustavo", "Matheus", "João Pedro", "Breno", "Felipe", "Júlia", "Carolina", "Caroline", "Joaquim", "Enzo Gabriel", "Thiago", "Lucas", "Giovanni", "Bianca", "Sophie", "Antônio", "Benjamin", "Vitória", "Isabelly", "Amanda", "Emilly", "Maria Cecília", "Marina", "Analu", "Nina", "Júlia", "Gustavo Henrique", "Miguel", "Catarina", "Stella", "Miguel Henrique", "Guilherme", "Caio", "Maria Vitória", "Isis", "Heloísa", "Gabriela", "Eloá", "Agatha", "Arthur Miguel", "Luiza", "Pedro Henrique", "Ana Beatriz", "Ruan", "Sophia", "Lara", "Luana", "Bárbara", "Kaique", "Raissa", "Rafaela", "Maria Valentina", "Bernardo", "Mirella", "Leonardo", "Davi Lucas", "Luiz Felipe", "Emanuel", "Maria Alice", "Luana", "Luna", "Enrico" };


        /// <summary>
        /// Array contendo os sobrenomes mais comuns no Brasil
        /// </summary>
        public static IEnumerable<string> SobrenomesComuns => new[] { "Silva", "Santos", "Souza", "Oliveira", "Pereira", "Ferreira", "Alves", "Pinto", "Ribeiro", "Rodrigues", "Costa", "Carvalho", "Gomes", "Martins", "Araújo", "Melo", "Barbosa", "Cardoso", "Nascimento", "Lima", "Moura", "Cavalcanti", "Monteiro", "Moreira", "Nunes", "Sales", "Ramos", "Montenegro", "Siqueira", "Borges", "Teixeira", "Amaral", "Sampaio", "Correa", "Fernandes", "Batista", "Miranda", "Leal", "Xavier", "Marques", "Andrade", "Freitas", "Paiva", "Vieira", "Aguiar", "Macedo", "Garcia", "Lacerda", "Lopes", "Mautari", "Zonaro" };

        public static string GerarNomeAleatorio(bool SobrenomeUnico = false)
        {
            var s1 = SobrenomesComuns.RandomItem();
            var s2 = SobrenomesComuns.RandomItem().NullIf(x => Util.RandomBool() || x == s1 || SobrenomeUnico);
            return $"{NomesComuns.RandomItem()} {s1} {s2}".Trim();
        }

        public static AddressInfo GerarEnderecoFake()
        {
            var e = Estados.RandomItem();
            var ad = CriarAddressInfo<AddressInfo>(e.Nome, e.Cidades.RandomItem().Nome);
            ad.Street = $"{AddressTypes.GetAllAdressTypes().RandomItem().AppendIf(".", x => x.Length < 3)} {GerarNomeAleatorio()}";
            ad.Neighborhood = GerarNomeAleatorio(true).PrependIf(new[] { "Jardim ", "Campos " }.RandomItem(), Util.RandomBool(75));
            ad.Number = Util.RandomNumber(10, 2000).ToString();
            ad.ZipCode = Util.RandomNumber(11111111, 99999999).FormatarCEP();
            ad.Complement = new[]
            {
                "",
                $"Casa {PredefinedArrays.AlphaUpperChars.Take(4).RandomItem()}",
                $"Apto. {Util.RandomNumber(11,200)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Apto. {Util.RandomNumber(11,200)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Casa {Util.RandomNumber(1,12)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Casa {PredefinedArrays.AlphaUpperChars.Take(6).RandomItem()}",
            }.RandomItem();
            return ad;
        }

        public static IEnumerable<Cidade> Cidades => Estados.SelectMany(x => x.Cidades).ToArray();

        /// <summary>
        /// Retorna as Regiões dos estados brasileiros
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> Regioes => Estados.Select(x => x.Regiao).Distinct();

        /// <summary>
        /// Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Estado> Estados
        {
            get
            {
                l = l ?? new List<Estado>();
                if (!l.Any())
                {
                    string s = Assembly.GetExecutingAssembly().GetResourceFileText("brasil.xml");
                    var doc = new XmlDocument();
                    doc.LoadXml(s);

                    foreach (XmlNode estado in doc.SelectNodes("brasil/state"))
                    {
                        var e = new Estado(estado["StateCode"].InnerText, estado["Name"].InnerText, estado["Region"].InnerText, estado["IBGE"].InnerText.ToInt(), "Brasil", "BR", estado["Longitude"].InnerText.ToDecimal(), estado["Latitude"].InnerText.ToDecimal());
                        var cities = new List<Cidade>();

                        foreach (XmlNode node in doc.SelectNodes($"brasil/city[StateCode = '{e.UF}']"))
                        {
                            cities.Add(new Cidade(
                                node["Name"].InnerText,
                                node["IBGE"].InnerText.ToInt(),
                                node["DDD"].InnerText.ToInt(),
                                e,
                                node["SIAFI"].InnerText,
                                node["TimeZone"].InnerText,
                                node["Latitude"].InnerText.ToDecimal(),
                                node["Longitude"].InnerText.ToDecimal(),
                                node["Capital"].InnerText.AsBool()));
                        }

                        e.Cidades = cities;
                        l.Add(e);
                    }
                }
                return l;
            }
        }

        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> da cidade e estado correspondentes
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        public static AddressInfo CriarAddressInfo(string NomeOuUFouIBGE, string City) => CriarAddressInfo<AddressInfo>(NomeOuUFouIBGE, City);

        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> da cidade e estado correspondentes
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <param name="Cidade"></param>
        /// <returns></returns>
        public static T CriarAddressInfo<T>(string NomeOuUFouIBGE, string Cidade) where T : AddressInfo
        {
            if (NomeOuUFouIBGE.IsBlank() && Cidade.IsNotBlank())
            {
                NomeOuUFouIBGE = PegarEstadoPeloNomeDaCidade(Cidade).FirstOrDefault().IfBlank(NomeOuUFouIBGE);
            }

            var s = PegarEstado(NomeOuUFouIBGE);
            if (s != null)
            {
                var c = PegarCidadePorAproximacao(s.UF, Cidade);
                var ends = Activator.CreateInstance<T>();
                ends.City = c?.Nome ?? Cidade;
                ends.State = s.Nome;
                ends.StateCode = s.UF;
                ends.Region = s.Regiao;
                ends.Country = "Brasil";
                ends.CountryCode = "BR";
                ends["StateIBGE"] = s.IBGE.ToString();
                ends["IBGE"] = c?.IBGE.ToString();
                ends["DDD"] = c?.DDD.ToString();
                ends["SIAFI"] = c?.SIAFI.ToString();
                ends["Capital"] = c?.Capital.ToString();
                ends["TimeZone"] = c?.TimeZone.ToString();
                ends.Latitude = c?.Latitude;
                ends.Longitude = c?.Longitude;
                return ends;
            }

            return null;
        }

        public static Cidade PegarCidadePeloIBGE(int IBGE) => Cidades.FirstOrDefault(x => x.IBGE == IBGE) ?? Estados.FirstOrDefault(x => x.IBGE == IBGE)?.Cidades.FirstOrDefault(x => x.Capital);

        /// <summary>
        /// Retorna o estado de uma cidade especifa. Pode trazer mais de um estado caso o nome da
        /// cidade seja igual em 2 ou mais estados
        /// </summary>
        /// <param name="CityName"></param>
        /// <returns></returns>
        public static IEnumerable<Estado> PegarEstadoPeloNomeDaCidade(string CityName) => Estados.Where((Func<Estado, bool>)(x => x.Cidades.Any((Func<Cidade, bool>)(c => (Util.ToSlugCase(c.Nome) ?? Util.EmptyString) == (Util.ToSlugCase(CityName) ?? Util.EmptyString) || (c.IBGE.ToString() ?? Util.EmptyString) == (Util.ToSlugCase(CityName) ?? Util.EmptyString)))));

        public static Estado PegarEstadoPeloIBGE(int IBGE) => Estados.FirstOrDefault(x => x.IBGE == IBGE) ?? PegarCidadePeloIBGE(IBGE)?.Estado;

        /// <summary>
        /// Procura numeros de telefone em um Texto
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ProcurarNumerosDeTelefone(this string Text) => Text.FindByRegex(@"\b[\s()\d-]{6,}\d\b", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Select(x => x.FormatarTelefone());

        /// <inheritdoc cref="FormatarCEP(string)"/>
        public static string FormatarCEP(this int CEP) => FormatarCEP(CEP.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Formata um numero para CEP
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static string FormatarCEP(this string CEP)
        {
            CEP = CEP.RemoveAny(".", "-").GetBefore(",") ?? Util.EmptyString;
            CEP = CEP.PadLeft(8, '0');
            CEP = CEP.Insert(5, "-");
            if (CEP.CEPValido())
            {
                return CEP;
            }
            else
            {
                throw new FormatException("String is not a valid CEP");
            }
        }

        /// <summary>
        /// Formata um numero para CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static string FormatarCNPJ(this long CNPJ) => string.Format(CultureInfo.InvariantCulture, @"{0:00\.000\.000\/0000\-00}", CNPJ);

        /// <summary>
        /// Formata um numero para CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static string FormatarCNPJ(this string CNPJ)
        {
            if (CNPJ.CNPJValido())
            {
                if (CNPJ.IsNumber())
                {
                    CNPJ = CNPJ.ToLong().FormatarCNPJ();
                }
            }
            else
            {
                throw new FormatException("String is not a valid CNPJ");
            }

            return CNPJ;
        }

        /// <summary>
        /// Formata um numero para CPF
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public static string FormatarCPF(this long CPF) => string.Format(CultureInfo.InvariantCulture, @"{0:000\.000\.000\-00}", CPF);

        /// <summary>
        /// Formata um numero para CPF
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public static string FormatarCPF(this string CPF)
        {
            if (CPF.CPFValido())
            {
                if (CPF.IsNumber())
                {
                    CPF = CPF.ToLong().FormatarCPF();
                }
            }
            else
            {
                throw new FormatException("String is not a valid CPF");
            }

            return CPF;
        }

        /// <summary>
        /// Formata um numero para CNPJ ou CNPJ se forem validos
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string FormatarCPFOuCNPJ(this long Document)
        {
            if (Document.ToString(CultureInfo.InvariantCulture).CPFValido())
            {
                return Document.FormatarCPF();
            }
            else if (Document.ToString(CultureInfo.InvariantCulture).CNPJValido())
            {
                return Document.FormatarCNPJ();
            }
            else
            {
                return Document.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Formata um numero para CNPJ ou CNPJ se forem validos
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string FormatarCPFOuCNPJ(this string Document)
        {
            if (Document.CPFValido())
            {
                return Document.FormatarCPF();
            }
            else if (Document.CNPJValido())
            {
                return Document.FormatarCNPJ();
            }
            else
            {
                return Document;
            }
        }

        public static string FormatarDocumentoComRotulo(this string Text, string DefaultString = Util.EmptyString)
        {
            var x = Text.PegarRotuloDocumento(DefaultString);
            switch (x)
            {
                case "CPF":
                case "CNPJ":
                    Text = Text.FormatarCPFOuCNPJ();
                    break;

                case "CEP":
                    Text = Text.FormatarCEP();
                    break;

                case "PIS":
                    Text = Text.FormatarPIS();
                    break;

                case "Email":
                    Text = Text.ToLowerInvariant();
                    break;

                case "Tel":
                    Text = Text.FormatarTelefone();
                    break;

                default:
                    break;
            }
            return $"{x}: {Text}";
        }

        /// <summary>
        /// Formata o PIS no padrão ###.#####.##-#
        /// </summary>
        /// <param name="PIS">PIS a ser formatado</param>
        /// <returns>PIS formatado</returns>
        public static string FormatarPIS(this string PIS)
        {
            if (PIS.PISValido())
            {
                PIS = PIS.RemoveAny(".", "-");
                PIS = PIS.PadLeft(11, '0');
                PIS = PIS.ToLong().ToString(@"000\.00000\.00-0", CultureInfo.InvariantCulture);
                return PIS;
            }
            else
            {
                throw new FormatException("String is not a valid PIS");
            }
        }

        /// <summary>
        /// Formata o PIS no padrão ###.#####.##-#
        /// </summary>
        /// <param name="PIS">PIS a ser formatado</param>
        /// <returns>PIS formatado</returns>
        public static string FormatarPIS(this long PIS) => FormatarPIS(PIS.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string FormatarTelefone(this string Number)
        {
            Number = Number ?? Util.EmptyString;
            if (Number.IsBlank()) return Number;
            Number = Number.ParseDigits().RemoveAny(",", ".").TrimBetween().GetLastChars(13);
            string mask;
            if (Number.Length <= 4)
                mask = "{0:####}";
            else if (Number.Length <= 8)
                mask = "{0:####-####}";
            else if (Number.Length == 9)
                mask = "{0:#####-####}";
            else if (Number.Length == 10)
                mask = "{0:(##) ####-####}";
            else if (Number.Length == 11)
                mask = "{0:(##) #####-####}";
            else if (Number.Length == 12)
                mask = "{0:+## (##) ####-####}";
            else
                mask = "{0:+## (##) #####-####}";

            return string.Format(mask, long.Parse(Number.IfBlank("0")));
        }

        /// <inheritdoc cref="FormatarTelefone(int)"/>
        public static string FormatarTelefone(this long Number) => FormatarTelefone($"{Number}");

        /// <inheritdoc cref="FormatarTelefone(string)"/>
        public static string FormatarTelefone(this int Number) => FormatarTelefone($"{Number}");

        /// <inheritdoc cref="FormatarTelefone(int)"/>
        public static string FormatarTelefone(this decimal Number) => FormatarTelefone($"{Number}");

        /// <inheritdoc cref="FormatarTelefone(int)"/>
        public static string FormatarTelefone(this double Number) => FormatarTelefone($"{Number}");

        public static Cidade PegarCapital(string NomeOuUFouIBGE) => (PegarEstado(NomeOuUFouIBGE)?.Cidades ?? new List<Cidade>()).FirstOrDefault(x => x.Capital);

        /// <summary>
        /// Retorna as cidades de um estado a partir do nome ou sigla do estado
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE">Nome ou sigla do estado</param>
        /// <returns></returns>
        public static IEnumerable<Cidade> PegarCidades(string NameOrStateCodeOrIBGE) => (PegarEstado(NameOrStateCodeOrIBGE)?.Cidades ?? new List<Cidade>()).AsEnumerable();

        public static Cidade PegarCidadePorAproximacao(string NomeOuUFouIBGE, string NomeAproximadoDaCidade) => (PegarEstado(NomeOuUFouIBGE)?.Cidades ?? new List<Cidade>()).AsEnumerable().OrderBy(x => x.Nome.LevenshteinDistance(NomeAproximadoDaCidade)).Where(x => NomeAproximadoDaCidade.IsNotBlank()).FirstOrDefault();

        /// <summary>
        /// Retorna o nome da cidade mais parecido com o especificado em <paramref name="NomeAproximadoDaCidade"/>
        /// </summary>
        /// <param name="NomeOuUFouIBGE">Nome ou sigla do estado</param>
        /// <param name="NomeAproximadoDaCidade">Nome da cidade</param>
        /// <returns></returns>
        public static string PegarNomeDaCidadePorAproximacao(string NomeOuUFouIBGE, string NomeAproximadoDaCidade) => (PegarCidadePorAproximacao(NomeOuUFouIBGE, NomeAproximadoDaCidade)?.Nome ?? Util.EmptyString).IfBlank(NomeAproximadoDaCidade);

        public static string PegarRotuloDocumento(this string Input, string DefaultLabel = Util.EmptyString)
        {
            if (Input.CPFValido()) return "CPF";
            if (Input.CNPJValido()) return "CNPJ";
            if (Input.CEPValido()) return "CEP";
            if (Input.IsValidEAN()) return "EAN";
            if (Input.PISValido()) return "PIS";
            if (Input.CNHValido()) return "CNH";
            if (Input.IsEmail()) return "Email";
            if (Input.TelefoneValido()) return "Tel";
            if (Input.IsIP()) return "IP";
            return DefaultLabel;
        }

        public static int? PegarCodigoIBGE(string NomeOuUFouIBGE) => PegarEstado(NomeOuUFouIBGE)?.IBGE;

        /// <summary>
        /// Retorna o nome do estado a partir da sigla
        /// </summary>
        /// <param name="NomeOuUFOuIBGE"></param>
        /// <returns></returns>
        public static string PegarNomeEstado(string NomeOuUFOuIBGE) => PegarEstado(NomeOuUFOuIBGE)?.Nome;

        /// <summary>
        /// Retorna a região a partir de um nome de estado
        /// </summary>
        /// <param name="NomeOuUFOuIBGE"></param>
        /// <returns></returns>
        public static string PegarRegiao(string NomeOuUFOuIBGE) => PegarEstado(NomeOuUFOuIBGE)?.Regiao;

        /// <summary>
        /// Retorna a as informações do estado a partir de um nome de estado ou sua sigla
        /// </summary>
        /// <param name="NomeOuUFOuIBGE">Nome ou UF</param>
        /// <returns></returns>
        public static Estado PegarEstado(string NomeOuUFOuIBGE)
        {
            NomeOuUFOuIBGE = NomeOuUFOuIBGE.TrimBetween().ToSlugCase();
            return Estados.FirstOrDefault(x => (x.Nome.ToSlugCase() ?? Util.EmptyString) == (NomeOuUFOuIBGE ?? Util.EmptyString) || (x.UF.ToSlugCase() ?? Util.EmptyString) == (NomeOuUFOuIBGE ?? Util.EmptyString) || (x.IBGE.ToString()) == (NomeOuUFOuIBGE ?? Util.EmptyString));
        }

        /// <summary>
        /// Retorna a Sigla a partir de um nome de estado
        /// </summary>
        /// <param name="NomeOuUFOuIBGE"></param>
        /// <returns></returns>
        public static string PegarCodigoEstado(string NomeOuUFOuIBGE) => PegarEstado(NomeOuUFOuIBGE)?.UF;

        /// <summary>
        /// Retorna os estados de uma região
        /// </summary>
        /// <param name="Regiao"></param>
        /// <returns></returns>
        public static IEnumerable<Estado> PegarEstadosDaRegiao(string Regiao) => Estados.Where((Func<Estado, bool>)(x => (Util.ToSlugCase(x.Regiao) ?? Util.EmptyString) == (Util.TrimBetween(Util.ToSlugCase(Regiao)) ?? Util.EmptyString) || Regiao.IsBlank()));

        /// <summary>
        /// Valida se a string é um telefone
        /// </summary>
        /// <param name="Telefone"></param>
        /// <returns></returns>
        public static bool TelefoneValido(this string Telefone) => new Regex(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).IsMatch(Telefone.RemoveAny("(", ")"));

        /// <summary>
        /// Verifica se uma string é um cep válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool CEPValido(this string CEP) => new Regex(@"^\d{5}-\d{3}$").IsMatch(CEP) || (CEP.RemoveAny("-").IsNumber() && CEP.RemoveAny("-").Length == 8);

        /// <summary>
        /// Verifica se a string é um CNH válido
        /// </summary>
        /// <param name="Text">CNH</param>
        /// <returns></returns>
        public static bool CNHValido(this string CNH)
        {
            // char firstChar = cnh[0];
            if (CNH.IsNotBlank() && CNH.Length == 11 && CNH != new string('1', 11))
            {
                int dsc = 0;
                int v = 0;
                int i = 0;
                int j = 9;
                while (i < 9)
                {
                    v += Convert.ToInt32(CNH[i].ToString()) * j;
                    i += 1;
                    j -= 1;
                }

                int vl1 = v % 11;
                if (vl1 >= 10)
                {
                    vl1 = 0;
                    dsc = 2;
                }

                v = 0;
                i = 0;
                j = 1;
                while (i < 9)
                {
                    v += Convert.ToInt32(CNH[i]) * j;
                    i += 1;
                    j += 1;
                }

                int x = v % 11;
                int vl2 = x >= 10 ? 0 : x - dsc;
                return $"{vl1}{vl2}" == (CNH.Substring(CNH.Length - 2, 2));
            }

            return false;
        }

        /// <summary>
        /// Verifica se a string é um CNPJ válido
        /// </summary>
        /// <param name="CNPJ">CPF</param>
        /// <returns></returns>
        public static bool CNPJValido(this string CNPJ)
        {
            try
            {
                if (CNPJ.IsNotBlank())
                {
                    var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    int soma;
                    int resto;
                    string digito;
                    string tempCnpj;
                    CNPJ = CNPJ.Trim();
                    CNPJ = CNPJ.Replace(".", Util.EmptyString).Replace("-", Util.EmptyString).Replace("/", Util.EmptyString);
                    if (CNPJ.Length != 14)
                    {
                        return false;
                    }

                    tempCnpj = CNPJ.Substring(0, 12);
                    soma = 0;
                    for (int i = 0; i <= 12 - 1; i++)
                    {
                        soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
                    }

                    resto = soma % 11;
                    if (resto < 2)
                    {
                        resto = 0;
                    }
                    else
                    {
                        resto = 11 - resto;
                    }

                    digito = resto.ToString();
                    tempCnpj += digito;
                    soma = 0;
                    for (int i = 0; i <= 13 - 1; i++)
                    {
                        soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
                    }

                    resto = soma % 11;
                    if (resto < 2)
                    {
                        resto = 0;
                    }
                    else
                    {
                        resto = 11 - resto;
                    }

                    digito += resto.ToString();
                    return CNPJ.EndsWith(digito);
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Verifica se a string é um CPF válido
        /// </summary>
        /// <param name="CPF">CPF</param>
        /// <returns></returns>
        public static bool CPFValido(this string CPF)
        {
            try
            {
                if (CPF.IsNotBlank())
                {
                    CPF = CPF.RemoveAny(".", "-");
                    string digito = Util.EmptyString;
                    int k;
                    int j;
                    int soma;
                    for (k = 0; k <= 1; k++)
                    {
                        soma = 0;
                        var loopTo = 9 + (k - 1);
                        for (j = 0; j <= loopTo; j++)
                        {
                            soma += int.Parse($"{CPF[j]}", CultureInfo.InvariantCulture) * (10 + k - j);
                        }

                        digito += $"{(soma % 11 == 0 || soma % 11 == 1 ? 0 : 11 - (soma % 11))}";
                    }

                    return digito[0] == CPF[9] & digito[1] == CPF[10];
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Verifica se a string é um CPF ou CNPJ válido
        /// </summary>
        /// <param name="CPFouCNPJ">CPF ou CNPJ</param>
        /// <returns></returns>
        public static bool CPFouCNPJValido(this string CPFouCNPJ) => CPFouCNPJ.CPFValido() || CPFouCNPJ.CNPJValido();

        /// <summary>
        /// Verifica se uma string é um PIS válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool PISValido(this string PIS)
        {
            if (PIS.IsBlank())
            {
                return false;
            }

            PIS = Regex.Replace(PIS, "[^0-9]", Util.EmptyString).ToString();

            if (PIS.Length != 11)
            {
                return false;
            }

            var count = PIS[0];
            if (PIS.Count(w => w == count) == PIS.Length)
            {
                return false;
            }

            var multiplicador = new int[10] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;

            soma = 0;

            for (var i = 0; i < 10; i++)
            {
                soma += int.Parse($"{PIS[i]}", CultureInfo.InvariantCulture) * multiplicador[i];
            }

            resto = soma % 11;

            resto = resto < 2 ? 0 : 11 - resto;

            if (PIS.EndsWith(resto.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static void Reload() => l = new List<Estado>();
    }

    public class ChaveNFe
    {
        public const int TamanhoCNPJ = 14;

        public const int TamanhoCodigo = 9;

        public const int TamanhoDigito = 1;

        public const int TamanhoMesAno = 4;

        public const int TamanhoModelo = 2;

        public const int TamanhoNota = 9;

        public const int TamanhoSerie = 3;

        public const int TamanhoUF = 2;

        private string cnpj = "";

        public ChaveNFe()
        {
        }

        public ChaveNFe(string Chave)
        {
            this.Chave = Chave;
        }

        public int Ano { get; set; }

        public string Chave
        {
            get => ToString();
            set
            {
                var c = "";
                foreach (char item in value ?? Util.EmptyString)
                {
                    if (char.IsNumber(item))
                    {
                        c += item;
                    }
                }

                if (c.Length != 44)
                {
                    c = "".PadLeft(44, '0');
                }

                var parts = c.SplitChunk(TamanhoUF, TamanhoMesAno - 2, TamanhoMesAno - 2, TamanhoCNPJ, TamanhoModelo, TamanhoSerie, TamanhoNota, TamanhoCodigo, TamanhoDigito).ToArray();

                UF = parts[0].ToInt();
                Ano = parts[1].ToInt();
                Mes = parts[2].ToInt();
                CNPJ = parts[3];
                Modelo = parts[4].ToInt();
                Serie = parts[5].ToInt();
                Nota = parts[6].ToInt();
                Codigo = parts[7].ToInt();
                Digito = parts[8].ToInt();
            }
        }

        public string CNPJ { get => cnpj.FormatarCPFOuCNPJ(); set => cnpj = value.RemoveMask().PadZero(TamanhoCNPJ); }

        public int Codigo { get; set; }

        public int Digito { get; set; }

        public string ID => $"{UF.FixedLenght(TamanhoUF)}-{Ano.FixedLenght(TamanhoMesAno - 2)}{Mes.FixedLenght(TamanhoMesAno - 2)}-{CNPJ.RemoveMask().PadLeft(TamanhoCNPJ, '0')}-{Modelo.FixedLenght(TamanhoModelo)}-{Serie.FixedLenght(TamanhoSerie)}-{Nota.FixedLenght(TamanhoNota)}-{Codigo.FixedLenght(TamanhoCodigo)}-{Digito.FixedLenght(TamanhoDigito)}";

        public int Mes { get; set; }

        public int Modelo { get; set; }

        public DateTime MesEmissao => new DateTime(2000 + Ano, Mes, 1);

        public int Nota { get; set; }

        public int Serie { get; set; }

        public Estado Estado => Brasil.PegarEstado($"{UF}");

        public int UF { get; set; }

        public static int CalcularDigito(string Chave)
        {
            if (Chave != null && Chave.Length.IsAny(43, 44))
            {
                Chave = Chave.GetFirstChars(43);

                // Cálculo do dígito verificador
                int peso = 2;
                int soma = 0;
                for (int i = Chave.Length - 1; i >= 0; i--)
                {
                    int algarismo = Convert.ToInt32(Chave.Substring(i, 1));
                    soma += algarismo * peso;
                    peso++;
                    if (peso == 10)
                    {
                        peso = 2;
                    }
                }
                int resto = soma % 11;
                int dv = 11 - resto;
                if (dv == 10 || dv == 11)
                {
                    dv = 0;
                }
                return dv;
            }
            throw new FormatException("Chave NFe inválida");
        }

        public void CalcularDigito() => Digito = CalcularDigito(this.Chave);

        public override string ToString() => ID.RemoveMask();
    }

    public class Cidade
    {
        public Cidade(string Name, int IBGE, int DDD, Estado Estado, string SIAFI, string TimeZone, decimal Latitude, decimal Longitude, bool Capital) : base()
        {
            this.Nome = Name;
            this.IBGE = IBGE;
            this.DDD = DDD;
            this.Estado = Estado;
            this.SIAFI = SIAFI;
            this.TimeZone = TimeZone;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Capital = Capital;
        }

        public bool Capital { get; }
        public int DDD { get; }
        public int IBGE { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }
        public string Nome { get; }
        public string SIAFI { get; }
        public Estado Estado { get; } = new Estado(null);
        public string TimeZone { get; }

        public override string ToString() => Nome;
    }

    /// <summary>
    /// Objeto que representa um estado do Brasil e seus respectivos detalhes
    /// </summary>
    public class Estado
    {
        /// <summary>
        /// Sigla do estado
        /// </summary>
        /// <returns></returns>
        public Estado(string UF, string Nome, string Regiao, int IBGE, string Pais, string CodigoPais, decimal Longitude, decimal Latitude)
        {
            this.UF = UF;
            this.Nome = Nome;
            this.Regiao = Regiao;
            this.IBGE = IBGE;
            this.Pais = Pais;
            this.CodigoPais = CodigoPais;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
        }

        /// <summary>
        /// Inicializa um objeto Estado a partir de uma sigla
        /// </summary>
        /// <param name="NomeOuUF"></param>
        public Estado(string NomeOuUF)
        {
            if (NomeOuUF.IsNotBlank())
            {
                Nome = Brasil.PegarNomeEstado(NomeOuUF);
                UF = Brasil.PegarCodigoEstado(NomeOuUF);
                Cidades = Brasil.PegarCidades(NomeOuUF);
                Regiao = Brasil.PegarRegiao(NomeOuUF);
            }
        }

        /// <summary>
        /// Lista de cidades do estado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cidade> Cidades { get; internal set; }

        public string Pais { get; }

        public string CodigoPais { get; }

        public int IBGE { get; }

        public decimal Latitude { get; }

        public decimal Longitude { get; }

        /// <summary>
        /// Nome do estado
        /// </summary>
        /// <returns></returns>
        public string Nome { get; }

        public string Regiao { get; }
        public string UF { get; }

        /// <summary>
        /// Retorna a String correspondente ao estado
        /// </summary>
        /// <returns></returns>
        public override string ToString() => UF;
    }
}