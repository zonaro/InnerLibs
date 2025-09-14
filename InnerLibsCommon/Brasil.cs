using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Extensions;
using Extensions.Locations;
using Extensions.NumberWriters;
using Extensions.vCards;

using System;
using System.Text;
using System.Security.Cryptography;

namespace Extensions.BR
{
    /// <summary>
    /// Objeto para manipular operações relacionadas ao Brasil
    /// </summary>
    public static partial class Brasil
    {
        public static string FonemaCodigo(string palavra)
        {
            if (palavra.IsBlank()) return palavra ?? string.Empty;

            if (palavra.Contains(" "))
            {
                return palavra.Split(" ").Select(x => FonemaCodigo(x)).JoinString(" ");
            }
            else
            {
                palavra = Fonema(palavra);

                var mapa = new Dictionary<char, string>
                {
                    ['a'] = "1",
                    ['e'] = "1",
                    ['i'] = "1",
                    ['o'] = "1",
                    ['u'] = "1",
                    ['b'] = "2",
                    ['p'] = "2",
                    ['c'] = "3",
                    ['k'] = "3",
                    ['q'] = "3",
                    ['g'] = "3",
                    ['d'] = "4",
                    ['t'] = "4",
                    ['f'] = "5",
                    ['v'] = "5",
                    ['s'] = "6",
                    ['z'] = "6",
                    ['x'] = "6",
                    ['l'] = "7",
                    ['L'] = "7",
                    ['m'] = "8",
                    ['n'] = "8",
                    ['r'] = "9",
                };

                var sb = new StringBuilder();
                palavra.OnlyAlpha();
                foreach (char c in palavra)
                {
                    if (mapa.ContainsKey(c))
                        sb.Append(mapa[c]);
                }

                return sb.ToString();
            }
        }

        public static string Fonema(string palavra)
        {
            if (palavra.IsBlank()) return palavra ?? string.Empty;

            /// simbolos para nomes
            palavra = palavra.Replace("@", " arroba ");


            // replace "#words" to "raxitagui words" but alone # with " cerquilha "      

            palavra = Regex.Replace(palavra, @"#(\w+)?", m =>
            {
                if (m.Groups[1].Success) // Tem palavra depois do #
                {
                    return "rashtag " + m.Groups[1].Value;
                }
                else // # sozinho
                {
                    return " cerquilha ";
                }
            });


            if (palavra.Contains(" "))
            {
                return palavra.Split(" ").Select(x => Fonema(x)).JoinString(" ");
            }
            else if (palavra.IsNumber())
            {
                palavra = palavra.OnlyNumbers();
                var w = FullNumberWriter.pt_BR();
                return Fonema(w.InExtensive(palavra.ToLong()));
            }
            else
            {
                // 1. Normalizar (minúsculas e sem acentos)
                palavra = palavra.ToLower().RemoveAccents();

                // 2. Regras fonéticas básicas
                var substituicoes = new (string, string)[]
                {
                   ("eat","iti"),
                   ("ph", "f"),
                   ("ca", "ka"), ("co", "ko"), ("cu", "ku"),
                   ("qu", "ku"), ("que", "ke"), ("qui", "ki"),
                   ("c(?=[ei])", "ss"), // c + e/i vira som de ss
                   ("ç", "ss"),
                   ("sh", "x"),
                   ("ch", "x"),
                   ("lh", "li"),
                   ("nh", "ni"),
                   ("rr", "r"),
                   ("zz","ts"),
                   ("v","w"),
                   ("h",""), // por ultimo remove os H sem som               
                };

                foreach (var (padrao, repl) in substituicoes)
                {
                    palavra = Regex.Replace(palavra, padrao, repl);
                }

                // 3. Casos especiais solicitados
                // S no início ou seguido de outro S → mantém S
                // S no meio/fim → vira Z
                // W no início → V
                // consoantes mudas (no meio ou no fim) vira consoante + "i"
                if (palavra.Length > 0)
                {
                    palavra = palavra.RemoveFirstEqual("h");
                    char[] chars = palavra.ToCharArray();
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (char.IsLetter(chars[i]))
                        {
                            if (chars[i] == 's')
                            {
                                if (i == 0) // primeiro caractere
                                {
                                    chars[i] = 's'; // mantém
                                }
                                else if (i == chars.Length - 1) //ultimo caractere
                                {
                                    chars[i] = 'z'; //  fim → Z
                                }
                                else
                                {
                                    //se antecede s opu sucede s, mantem s
                                    if (chars[i - 1] == 's' || chars[i + 1] == 's')
                                    {
                                        chars[i] = 's';
                                    }
                                    else
                                    {
                                        chars[i] = 'z';

                                    }
                                }
                            }
                            else if ("bcdgpt".Contains(chars[i]))
                            {
                                if (i == chars.Length - 1 || "bcdgpt".Contains(chars[i + 1]))
                                {
                                    // consoante muda no fim ou seguida de outra consoante
                                    chars[i] = chars[i]; // mantém a consoante
                                                         // insere 'i' após a consoante
                                    palavra = new string(chars);
                                    palavra = palavra.Insert(i + 1, "i");
                                    chars = palavra.ToCharArray();
                                    i++; // pula o 'i' que acabou de inserir
                                }
                            }
                        }

                    }
                    palavra = new string(chars);
                }

                return palavra;
            }
        }

        public static IEnumerable<string> Fonema(IEnumerable<string> palavras)
        {
            return palavras.Select(x => Fonema(x));
        }

        public static IEnumerable<string> FonemaCodigo(IEnumerable<string> palavras)
        {
            return palavras.Select(x => FonemaCodigo(x));
        }

        /// <summary>
        /// Formata uma data para o padrão brasileiro, encurtando a data para o padrão dd/MM/yyyy quando não houver horas, minutos ou segundos
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string FormatarDataBrasileira(this DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue) return string.Empty;

            if (dateTime.Millisecond > 0)
                return dateTime.ToString("dd/MM/yyyy HH:mm:ss:FFFFFFF");
            else if (dateTime.Second > 0)
                return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
            else if (dateTime.Minute > 0 || dateTime.Hour > 0)
                return dateTime.ToString("dd/MM/yyyy HH:mm");
            else

                return dateTime.ToString("dd/MM/yyyy");
        }

        public static string FormatarDataBrasileira(this DateTime? dateTime) => (dateTime ?? DateTime.MinValue).FormatarDataBrasileira();

        public static string FormatarDataBrasileira(this string dateTime) => dateTime.ToDateTime().FormatarDataBrasileira();

        /// <inheritdoc cref = "TelefoneValido(string)" />
        public static bool TelefoneValido(int telefone)
        => TelefoneValido(telefone.ToString());

        /// <summary>
        /// Valida um número de telefone.
        ///
        /// Esta função verifica se um número de telefone é válido de acordo com as seguintes regras:
        /// - Remove todos os caracteres não numéricos do número de telefone.
        /// - Verifica se o número tem o tamanho correto (8 ou 9 dígitos locais + 0 ou 2 dígitos DDD).
        /// - Se o número tem 10 ou 11 dígitos, verifica se os dois primeiros são um DDD válido.
        ///
        /// Retorna `true` se o número de telefone for válido, caso contrário retorna `false`.
        /// </summary>
        public static bool TelefoneValido(this string telefone)
        {
            try
            {
                if (telefone.RemoveAny(PredefinedArrays.TelephoneChars.ToArray()).IsNotBlank()) return false;

                string apenasNumeros = telefone.OnlyNumbers();

                if (apenasNumeros.IsBlank()) return false;

                if ((apenasNumeros.Length == 13 || apenasNumeros.Length == 12) && apenasNumeros.StartsWith("55") && telefone.StartsWith("+"))
                {
                    apenasNumeros = apenasNumeros.Substring(2);
                }

                if (apenasNumeros.Length < 8 || apenasNumeros.Length > 11)
                {
                    return false;
                }

                if (apenasNumeros.Length > 9)
                {
                    int ddd = int.Parse(apenasNumeros.Substring(0, 2));
                    if (Brasil.DDDValido(ddd) == false)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static List<Estado> states = new List<Estado>();
        internal static List<Cidade> cities = new List<Cidade>();

        /// <summary>
        /// Array contendo os nomes mais comuns no Brasil
        /// </summary>
        public static IEnumerable<string> NomesComuns => NomesMasculinosComuns.Union(NomesFemininosComuns).Distinct();

        public static IEnumerable<string> NomesMasculinosComuns => new[] { "Miguel", "Arthur", "Davi", "Gabriel", "Pedro", "Heitor", "Enzo", "Lorenzo", "Vinícius", "Rafael", "Gustavo", "Matheus", "João Pedro", "Breno", "Felipe", "Joaquim", "Enzo Gabriel", "Thiago", "Lucas", "Giovanni", "Antônio", "Benjamin", "Gustavo Henrique", "Miguel Henrique", "Guilherme", "Caio", "Arthur Miguel", "Pedro Henrique", "Bernardo", "Leonardo", "Davi Lucas", "Luiz Felipe", "Emanuel", "Enrico", "Ruan", "Kaique" };
        public static IEnumerable<string> NomesFemininosComuns => new[] { "Alice", "Sophia", "Sofia", "Manuela", "Isabella", "Laura", "Valentina", "Giovanna", "Giovana", "Maria Eduarda", "Beatriz", "Maria Clara", "Lara", "Mariana", "Helena", "Isadora", "Lívia", "Luana", "Maria Luíza", "Luiza", "Ana Luiza", "Eduarda", "Letícia", "Melissa", "Maria Fernanda", "Cecília", "Lorena", "Clara", "Júlia", "Carolina", "Caroline", "Bianca", "Sophie", "Vitória", "Isabelly", "Amanda", "Emilly", "Maria Cecília", "Marina", "Analu", "Nina", "Catarina", "Stella", "Maria Vitória", "Isis", "Heloísa", "Gabriela", "Eloá", "Agatha", "Ana Beatriz", "Bárbara", "Raissa", "Rafaela", "Maria Valentina", "Mirella", "Maria Alice", "Luna" };

        /// <summary>
        /// Array contendo os sobrenomes mais comuns no Brasil
        /// </summary>
        public static IEnumerable<string> SobrenomesComuns => new[] { "Silva", "Santos", "Souza", "Oliveira", "Pereira", "Ferreira", "Alves", "Pinto", "Ribeiro", "Rodrigues", "Costa", "Carvalho", "Gomes", "Martins", "Araújo", "Melo", "Barbosa", "Cardoso", "Nascimento", "Lima", "Moura", "Cavalcanti", "Monteiro", "Moreira", "Nunes", "Sales", "Ramos", "Montenegro", "Siqueira", "Borges", "Teixeira", "Amaral", "Sampaio", "Correa", "Fernandes", "Batista", "Miranda", "Leal", "Xavier", "Marques", "Andrade", "Freitas", "Paiva", "Vieira", "Aguiar", "Macedo", "Garcia", "Lacerda", "Lopes" };

        public static vCard GerarPessoa(bool? NomeMasculino = null, bool SobrenomeUnico = false, int IdadeMinima = 18, int IdadeMaxima = 80)

        {
            NomeMasculino = NomeMasculino ?? Util.RandomBool();

            var nome = GerarPrimeiroNomeAleatorio(NomeMasculino);
            var sobrenome = GerarSobrenomeAleatorio(true);
            var sobrenomeMeio = GerarSobrenomeAleatorio(true);
            if (SobrenomeUnico)
            {
                sobrenomeMeio = null;
            }
            else
            {
                while (sobrenomeMeio.FlatEqual(sobrenome))
                {
                    sobrenomeMeio = GerarSobrenomeAleatorio(true);
                }
            }

            var dataNascimento = Util.RandomDateTime(DateTime.Now.AddYears(IdadeMaxima.ForceNegative()), DateTime.Now.AddYears(IdadeMinima.ForceNegative()));
            var cpf = GerarCPFFake();
            var cnpj = dataNascimento.GetAge() >= 18 ? GerarCPFFake() : null;
            var cnh = dataNascimento.GetAge() >= 18 ? GerarCNHFake() : null;
            var endereco = GerarEnderecoFake();
            var telefone = GerarTelefoneFake(endereco.City);

            // gerar username pegando o nome até a primeira vogal + sobrenome até primeria vogal + ano de nascimento
            var usernames = Util.UsernamesFor(nome, sobrenome, dataNascimento);
            var email = $"{usernames.RandomItem()}@{PredefinedArrays.EmailDomains.RandomItem()}";

            var pessoa = new vCard
            {
                FirstName = nome,
                MiddleName = sobrenomeMeio,
                LastName = sobrenome,
                Birthday = dataNascimento
            };

            pessoa.Gender = (NomeMasculino.Value ? vGender.M : vGender.F, NomeMasculino.Value ? "Masculino" : "Feminino");
            pessoa.AddTelephone(telefone);

            pessoa.AddEmail(email);

            pessoa.Extra("CNH", cnh);
            pessoa.Extra("CPF", cpf);
            pessoa.Extra("CNPJ", cnpj);
            pessoa.Extra("UserName", usernames.RandomItem());
            pessoa.Extra("FAKE", true);
            pessoa.AddAddress(endereco);

            return pessoa;
        }

        public static string GerarPrimeiroNomeAleatorio(bool? NomeMasculino = null)
        {
            NomeMasculino = NomeMasculino ?? Util.RandomBool();
            var primeiroNome = NomeMasculino.Value ? NomesMasculinosComuns.RandomItem() : NomesFemininosComuns.RandomItem();
            return primeiroNome;
        }

        public static string GerarSobrenomeAleatorio(bool? SobrenomeUnico = null)
        {
            SobrenomeUnico = SobrenomeUnico ?? Util.RandomBool(30);
            var sobrenome1 = SobrenomesComuns.RandomItem();
            var sobrenome2 = SobrenomesComuns.RandomItem().NullIf(x => SobrenomeUnico.Value || x == sobrenome1);
            return $"{sobrenome1} {sobrenome2}".Trim();
        }

        public static string GerarNomeCompletoAleatorio(bool? NomeMasculino = null, bool SobrenomeUnico = false)
        {
            NomeMasculino ??= Util.RandomBool();

            return $"{GerarPrimeiroNomeAleatorio(NomeMasculino)} {GerarSobrenomeAleatorio(SobrenomeUnico)}";
        }

        public static Image GerarAvatarAleatorio(bool SobrenomeUnico = false) => GerarNomeCompletoAleatorio(SobrenomeUnico).GenerateAvatarByName();

        /// <inheritdoc cref= "GerarEnderecoFake{T}" />
        public static BrasilAddressInfo GerarEnderecoFake(string Label = "Residêncial", bool BuscarEnderecoReal = true) => GerarEnderecoFake<BrasilAddressInfo>(BuscarEnderecoReal: BuscarEnderecoReal);

        /// <summary>
        /// Retorna um endereço falso, mas com dados reais de cidade e estado. Tenta buscar um endereço real a partir do CEP gerado, mas caso não consiga, gera um endereço fictício
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Label"></param>
        /// <param name="BuscarEnderecoReal"></param>
        /// <returns></returns>
        public static T GerarEnderecoFake<T>(string Label = "Residêncial", bool BuscarEnderecoReal = true) where T : AddressInfo
        {
            var cid = Cidades.RandomItem();
            T ad = CriarAddressInfo<T>(cid.UF, cid.Nome);
            ad["endereco_fake"] = "true";
            try
            {
                if (!BuscarEnderecoReal) throw new Exception("Endereço FAKE");
                ad = PegarEndereco<T>(Util.RandomLong(cid.CepInicial, cid.CepFinal));
                ad["Origem"] = "ViaCep";
            }
            catch
            {
                ad["Origem"] = "Endereço Gerado";
                ad.ZipCode = Util.RandomLong(cid.CepInicial, cid.CepFinal).FormatarCEP();
                ad.Street = $"{AddressTypes.Avenida.Union(AddressTypes.Rua).Union(AddressTypes.Travessa).Union(AddressTypes.Alameda).RandomItem().AppendIf(".", x => x.Length < 3)} {GerarNomeCompletoAleatorio()}";
                ad.Neighborhood = GerarNomeCompletoAleatorio(true).PrependIf(new[] { "Jardim ", "Campos ", "Vila", "COHAB", "CDHU" }.RandomItem(), Util.RandomBool(75));
            }
            ad.Label = Label;
            ad.Number = Util.RandomInt(10, 2000).ToString();
            ad.Complement = Util.RandomBool() ? "" : new string[]
            {
                $"Casa {Util.RandomInt(1,250)}",
                $"Casa {PredefinedArrays.AlphaUpperChars.Take(4).RandomItem()}",
                $"Apto. {Util.RandomInt(11,209)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Apto. {Util.RandomInt(11,200)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Casa {Util.RandomInt(1,12)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Casa {PredefinedArrays.AlphaUpperChars.Take(6).RandomItem()}",
            }.RandomItem();

            return ad;
        }

        public static IEnumerable<Cidade> Cidades
        {
            get
            {
                LoadXML();
                return cities.OrderBy(x => x.Nome).ThenByDescending(x => x.TotalMunicipio).DistinctBy(x => x.IBGE);
            }
        }

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
                LoadXML();
                return states;
            }
        }

        private static bool cached = false;

        private static void LoadXML()
        {
            states = states ?? new List<Estado>();
            cities = cities ?? new List<Cidade>();
            if (!cached)
            {
                cached = true;
                states.Clear();
                cities.Clear();
                string s = Assembly.GetExecutingAssembly().GetResourceFileText("brasil.xml");
                var doc = new XmlDocument();
                doc.LoadXml(s);

                foreach (XmlNode estado in doc.SelectNodes("brasil/Estados"))
                {
                    var e = new Estado();
                    e.UF = estado["UF"].InnerText;
                    e.Nome = estado["Nome"].InnerText;
                    e.Regiao = estado["Regiao"].InnerText;
                    e.IBGE = estado["IBGE"].InnerText.ToInt();
                    e.Pais = "Brasil";
                    e.CodigoPais = "BR";
                    e.Longitude = estado["Longitude"].InnerText;
                    e.Latitude = estado["Latitude"].InnerText;

                    states.Add(e);
                }

                foreach (XmlNode node in doc.SelectNodes($"brasil/Cidades"))
                {
                    var cid = new Cidade();

                    cid.Id = node["id"].InnerText.ToInt();
                    cid.IBGE = node["IBGE"].InnerText.ToInt();
                    cid.Nome = node["Nome"].InnerText.ToTitle();
                    cid.DDD = node["DDD"].InnerText.ToInt();
                    cid.SIAFI = node["SIAFI"].InnerText;
                    cid.TimeZone = node["TimeZone"].InnerText;
                    cid.Latitude = node["Latitude"].InnerText;
                    cid.Longitude = node["Longitude"].InnerText;
                    cid.Capital = node["Capital"].InnerText.AsBool();
                    cid.Altitude = node["Altitude"].InnerText;
                    cid.CepFinal = node["CepFinal"].InnerText.ToLong();
                    cid.CepInicial = node["CepInicial"].InnerText.ToLong();
                    cid.CodigoGeograficoDistrito = node["CodigoGeograficoDistrito"].InnerText;
                    cid.CodigoGeograficoSubdivisao = node["CodigoGeograficoSubdivisao"].InnerText;
                    cid.MacroRegiao = node["MacroRegiao"].InnerText.ToTitle();
                    cid.MicroRegiao = node["MicroRegiao"].InnerText.ToTitle();
                    cid.ExclusivaSedeUrbana = node["TipoDeFaixa"].InnerText.FlatEqual("Exclusiva da sede urbana");
                    cid.TotalMunicipio = node["TipoDeFaixa"].InnerText.FlatEqual("Total do Município");

                    cid.Estado = states.FirstOrDefault(x => cid.IBGE.ToString().GetFirstChars(2).FlatEqual(x.IBGE));

                    var tags = new List<string>
                    {
                        cid.Nome,
                        cid.Estado?.UF,
                        cid.Estado?.Nome,
                        cid.Estado?.Regiao,
                    }.WhereNotBlank().SelectMany(x => new List<string> {
                       x.ToUpperInvariant().Split(Util.WhitespaceChar).JoinString(),
                       x.ToUpperInvariant().Split(Util.WhitespaceChar).RemoveAny("DO", "DOS", "DE", "DA","D","D'","DAS").JoinString(),
                       x.ToUpperInvariant().Split(Util.WhitespaceChar).SelectJoinString(c => c.GetFirstChars()),
                       x.ToUpperInvariant().Split(Util.WhitespaceChar).RemoveAny("DO", "DOS", "DE", "DA","D","D'","DAS").SelectJoinString(c => c.GetFirstChars()),
                    });

                    cid.Keywords = tags.Where(x => x.Length >= 2).Distinct();

                    cities.Add(cid);
                }
            }
        }

        /// <summary>
        /// Retorna um <see cref="BrasilAddressInfo"/> da cidade e estado correspondentes
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <param name="Cidade"></param>
        /// <returns></returns>
        public static BrasilAddressInfo CriarAddressInfo(string NomeOuUFouIBGE, string Cidade) => CriarAddressInfo<BrasilAddressInfo>(NomeOuUFouIBGE, Cidade);

        public static BrasilAddressInfo CriarAddressInfo(string CidadeOuIBGE) => CriarAddressInfo<BrasilAddressInfo>(CidadeOuIBGE);

        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> da cidade e estado correspondentes
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <param name="Cidade"></param>
        /// <returns></returns>
        public static T CriarAddressInfo<T>(string EstadoOuIBGE) where T : AddressInfo
        {
            var cid = PegarCidade(EstadoOuIBGE, EstadoOuIBGE);
            return CriarAddressInfo<T>(cid.Estado.UF, cid.Nome);
        }

        public static T CriarAddressInfo<T>(string NomeOuUFouIBGE, string Cidade) where T : AddressInfo
        {
            if (NomeOuUFouIBGE.IsNotValid() && Cidade.IsValid())
            {
                NomeOuUFouIBGE = PegarEstadoPeloNomeDaCidade(Cidade).FirstOrDefault().IfBlank(NomeOuUFouIBGE);
            }

            var s = PegarEstado(NomeOuUFouIBGE);
            if (s != null)
            {
                var c = PegarCidade(Cidade, s.UF);
                var ends = Activator.CreateInstance<T>();
                ends.City = c?.Nome ?? Cidade;
                ends.State = s.Nome;
                ends.StateCode = s.UF;
                ends.Region = s.Regiao;
                ends.Country = "Brasil";
                ends.CountryCode = "BR";
                ends[nameof(BrasilAddressInfo.StateIBGE)] = s.IBGE.ToString();
                ends[nameof(BrasilAddressInfo.IBGE)] = c?.IBGE.ToString();
                ends[nameof(BrasilAddressInfo.DDD)] = c?.DDD.ToString();
                ends[nameof(BrasilAddressInfo.SIAFI)] = c?.SIAFI.ToString();
                ends[nameof(BrasilAddressInfo.TimeZone)] = c?.TimeZone.ToString();
                ends.Capital = c?.Capital ?? false;
                ends.Latitude = c?.Latitude;
                ends.Longitude = c?.Longitude;
                return ends;
            }

            return null;
        }

        public static T PegarEndereco<T>(long CEP, string Numero = null, string Complemento = null) where T : AddressInfo => AddressInfo.FromViaCEP<T>(CEP.ToString(), Numero, Complemento);

        public static BrasilAddressInfo PegarEndereco(long CEP, string Numero = null, string Complemento = null) => PegarEndereco<BrasilAddressInfo>(CEP, Numero, Complemento);

        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGEouCEP. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE) => PegarCidade(NomeDaCidadeOuIBGE, 100);

        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGEouCEP. A busca por nome da cidade é feita a partir da similaridade entre o nome fornecido e o nome cadastrado no banco de dados. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE, int Similaridade) => PegarCidade(NomeDaCidadeOuIBGE, null, Similaridade);

        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGEouCEP. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE, string NomeOuUFOuIBGE) => PegarCidade(NomeDaCidadeOuIBGE, NomeOuUFOuIBGE, 100);

        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGEouCEP.A busca por nome da cidade é feita a partir da similaridade entre o nome fornecido e o nome cadastrado no banco de dados. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        /// <param name="NomeOuUFOuIBGE">Nome do Estado ou Sigla do Estado ou código IBGEouCEP do Estado ou Cidade</param>
        /// <param name="NomeDaCidadeOuIBGE">Nome da cidade ou código IBGEouCEP da cidade</param>
        /// <param name="Similaridade">Porcentagem de similaridade entre o nome da cidade fornecido (<paramref name="NomeDaCidadeOuIBGE"/>) e o nome da cidade dentro do banco de dados (<see cref="Cidade.Nome"/>)</param>
        /// <returns>as informações da cidade encontrada</returns>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE, string NomeOuUFOuIBGE, int Similaridade)
        {
            NomeDaCidadeOuIBGE = NomeDaCidadeOuIBGE ?? "";

            if (NomeOuUFOuIBGE.IsBlank() && NomeDaCidadeOuIBGE.Contains("- "))
            {
                NomeOuUFOuIBGE = NomeDaCidadeOuIBGE.GetAfter(" -").TrimBetween();
            }

            if (NomeDaCidadeOuIBGE.Contains(" -"))
            {
                NomeDaCidadeOuIBGE = NomeDaCidadeOuIBGE.GetBefore(" -").TrimBetween();
            }

            var est = PegarEstado(NomeOuUFOuIBGE) ?? PegarEstado(NomeDaCidadeOuIBGE);
            var cids = PesquisarCidade(NomeDaCidadeOuIBGE, NomeOuUFOuIBGE, Similaridade);
            return cids.FirstOrDefault(x => x.IBGE == NomeDaCidadeOuIBGE.RemoveMaskInt()) ?? cids.OrderByDescending(x => x.Nome.SimilarityCaseInsensitive(NomeDaCidadeOuIBGE)).ThenByDescending(x => x.Capital).FirstOrDefault() ?? est?.Capital;
        }

        public static IEnumerable<Cidade> PesquisarCidade(string NomeDaCidadeOuIBGE, string NomeOuUFOuIBGE = "", int Similaridade = 90)
        {
            NomeDaCidadeOuIBGE = NomeDaCidadeOuIBGE ?? "";
            NomeOuUFOuIBGE = NomeOuUFOuIBGE.IfBlank(NomeDaCidadeOuIBGE);
            if (NomeOuUFOuIBGE.IsBlank() && NomeDaCidadeOuIBGE.Contains("- "))
            {
                NomeOuUFOuIBGE = NomeDaCidadeOuIBGE.GetAfter(" -").TrimBetween();
            }

            if (NomeDaCidadeOuIBGE.Contains(" -"))
            {
                NomeDaCidadeOuIBGE = NomeDaCidadeOuIBGE.GetBefore(" -").TrimBetween();
            }

            List<Cidade> cids = Cidades.ToList();

            if (NomeDaCidadeOuIBGE.FormatoCodigoIBGEValido())
            {
                if (NomeDaCidadeOuIBGE.Length == 7)
                {
                    return cids.Where(x => x.IBGE == NomeDaCidadeOuIBGE.RemoveMaskInt());
                }
                else
                {
                    return Estados.FirstOrDefault(x => x.IBGE == NomeDaCidadeOuIBGE.RemoveMaskInt())?.Cidades;
                }
            }

            if (NomeDaCidadeOuIBGE.CEPValido())
            {
                return cids.Where(x => x.ContemCep(NomeDaCidadeOuIBGE));
            }

            var est = PegarEstado(NomeOuUFOuIBGE) ?? PegarEstado(NomeDaCidadeOuIBGE);
            cids = est?.Cidades.ToList() ?? cids;

            if (NomeDaCidadeOuIBGE.IsBlank())
            {
                return cids;
            }

            var similar = cids.Where(x =>
            {
                if (x.Nome.FlatContains(NomeDaCidadeOuIBGE))
                {
                    return true;
                }

                double sim = NomeDaCidadeOuIBGE.SimilarityFlat(x.Nome);
                var limit = (double)Similaridade / 100;
                return sim >= limit;
            });

            if (similar.Any())
            {
                cids = similar.ToList();
            }
            else
            {
                cids = cids.Where(x => x.Keywords.Any(c => c.FlatEqual(NomeOuUFOuIBGE))).ToList();
            }

            return cids.OrderByDescending(x => x.Nome.SimilarityFlat(NomeDaCidadeOuIBGE)).ThenByDescending(x => x.Capital);
        }

        public static Cidade PegarCidade(int IBGEouCEP) => PegarCidade(IBGEouCEP.ToString());

        public static Cidade PegarCidade(long IBGEouCEP) => PegarCidade(IBGEouCEP.ToString());

        /// <summary>
        /// Retorna o estado de uma cidade especifa. Pode trazer mais de um estado caso o nome da
        /// cidade seja igual em 2 ou mais estados
        /// </summary>
        /// <param name="NomeDaCidade"></param>
        /// <returns></returns>
        public static IEnumerable<Estado> PegarEstadoPeloNomeDaCidade(string NomeDaCidade) => Estados.Where(x => x.Cidades.Any(c => (Util.ToSlugCase(c.Nome) ?? Util.EmptyString) == (Util.ToSlugCase(NomeDaCidade) ?? Util.EmptyString) || (x.IBGE.ToString() ?? Util.EmptyString) == (Util.ToSlugCase(NomeDaCidade).GetFirstChars(2) ?? Util.EmptyString)));

        /// <summary>
        /// Procura numeros de telefone em um Texto
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ProcurarNumerosDeTelefone(this string Text) => Text.FindByRegex(@"\b[\s()\d-]{6,}\d\b", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Select(x => x.FormatarTelefone());

        /// <inheritdoc cref="FormatarCEP(string)"/>
        public static string FormatarCEP(this int CEP) => FormatarCEP(CEP.ToString(CultureInfo.InvariantCulture));

        public static string FormatarCEP(this long CEP) => FormatarCEP(CEP.ToString(CultureInfo.InvariantCulture));

        /// <inheritdoc cref="FormatarCEP(string)"/>
        public static string FormatarCEP(this int? CEP) => FormatarCEP(CEP ?? 0);

        public static string FormatarCEP(this long? CEP) => FormatarCEP(CEP ?? 0);

        /// <summary>
        /// Formata um numero para CEP
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static string FormatarCEP(this string CEP)
        {
            CEP = CEP.RemoveMask() ?? Util.EmptyString;
            CEP = CEP.PadLeft(8, '0');
            CEP = CEP.Insert(5, "-");
            if (CEP.CEPValido())
            {
                return CEP;
            }
            else
            {
                return Util.EmptyString;
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

        /// <summary>
        /// Retorna um documento formatado com seu rótulo
        /// </summary>
        /// <param name="Text">CPF, CNPJ, CNH, CEP, PIS, Email, Celular ou Telefone</param>
        /// <param name="DefaultString"></param>
        /// <returns></returns>
        public static string FormatarDocumentoComRotulo(this string Text, string DefaultString = Util.EmptyString)
        {
            var x = Text.PegarRotuloDocumento(DefaultString);

            if (x.IsValid())
            {
                Text = $"{x}: {Text}";
            }
            return Text;
        }

        /// <summary>
        /// Retorna um documento formatado
        /// </summary>
        /// <param name="Text">CPF, CNPJ, CNH, CEP, PIS, Email, Celular ou Telefone</param>

        /// <returns></returns>
        public static string FormatarDocumento(this string Text)
        {
            switch (Text)
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

                case "CNH":
                    Text = Text.FormatarCNH();
                    break;

                case "RENAVAM":
                    Text = Text.FormatarRENAVAM();
                    break;

                case "Email":
                    Text = Text.ToLowerInvariant();
                    break;

                case "Telefone":
                case "Celular":
                    Text = Text.FormatarTelefone();
                    break;

                default:

                    break;
            }
            return Text;
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
        /// Formata o CNH no padrão ########
        /// </summary>
        /// <param name="CNH"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static string FormatarCNH(this string CNH)
        {
            if (CNH.CNHValido())
            {
                CNH = CNH.OnlyNumbersLong().FixedLength(8);
                return CNH;
            }
            else
            {
                throw new FormatException("String is not a valid CNH");
            }
        }

        /// <summary>
        /// Formata o PIS no padrão ###.#####.##-#
        /// </summary>
        /// <param name="PIS">PIS a ser formatado</param>
        /// <returns>PIS formatado</returns>
        public static string FormatarPIS(this long PIS) => FormatarPIS(PIS.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Formata a CNH no padrão ########
        /// </summary>
        /// <param name="CNH"></param>
        /// <returns></returns>
        public static string FormatarCNH(this int CNH) => FormatarCNH(CNH.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Formata o RENAVAM no padrão ###########
        /// </summary>
        /// <param name="RENAVAM"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static string FormatarRENAVAM(this string RENAVAM)
        {
            if (RENAVAM.RENAVAMValido())
            {
                RENAVAM = RENAVAM.OnlyNumbersLong().FixedLength(11);
                return RENAVAM;
            }
            else
            {
                throw new FormatException("String is not a valid RENAVAM");
            }
        }

        public static string FormatarRENAVAM(this long RENAVAM) => FormatarRENAVAM(RENAVAM.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Verifica se um número de RENAVAM é válido
        /// </summary>
        /// <param name="renavam"></param>
        /// <returns></returns>
        public static bool RENAVAMValido(this string renavam)
        {
            // Remover quaisquer caracteres não numéricos
            renavam = renavam.OnlyNumbers();

            if (renavam.Length != 11)
            {
                return false;
            }

            // Inverter os caracteres do número de RENAVAM
            renavam = new string(renavam.Reverse().ToArray());

            int soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(renavam[i].ToString()) * (i % 10 + 2);
            }

            int resto = soma % 11;
            int digitoVerificador = resto < 2 ? 0 : 11 - resto;

            return digitoVerificador == int.Parse(renavam[10].ToString());
        }

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string FormatarTelefone(this string Number)
        {
            Number = Number ?? Util.EmptyString;
            if (Number.IsNotValid()) return Number;
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
        /// <param name="NomeOuUFOuIBGE">Nome ou sigla do estado</param>
        /// <returns></returns>
        public static IEnumerable<Cidade> PegarCidades(string NomeOuUFOuIBGE) => (PegarEstado(NomeOuUFOuIBGE)?.Cidades ?? new List<Cidade>()).AsEnumerable();

        /// <summary>
        /// Retorna o rótulo do documento (CPF, CNPJ, CEP,EAN,PIS, CNH, Email, IP, Telefone ou Celular)
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="DefaultLabel"></param>
        /// <returns></returns>
        public static string PegarRotuloDocumento(this string Input, string DefaultLabel = Util.EmptyString)
        {
            if (Input.CPFValido()) return "CPF";
            if (Input.CNPJValido()) return "CNPJ";
            if (Input.CEPValido()) return "CEP";
            if (Input.IsValidEAN()) return "EAN";
            if (Input.PISValido()) return "PIS";
            if (Input.CNHValido()) return "CNH";
            if (Input.RENAVAMValido()) return "RENAVAM";
            if (Input.IsEmail()) return "Email";
            if (Input.IsIP()) return "IP";
            if (Input.TelefoneValido()) return Input.RemoveMask().Length.IsAny(8, 10) ? "Telefone" : "Celular";
            if (Estado.ValidarInscricaoEstadual(Input, "")) return "Inscrição Estadual";

            return DefaultLabel;
        }

        /// <summary>
        /// Retorna o código IBGEouCEP do estado
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <returns></returns>
        public static int? PegarCodigoIbgeEstado(string NomeOuUFouIBGE) => PegarEstado(NomeOuUFouIBGE)?.IBGE;

        /// <summary>
        /// Retorna o código IBGEouCEP da cidade
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <returns></returns>
        public static int? PegarCodigoIbgeCidade(string NomeOuUFouIBGE) => PegarCidade(NomeOuUFouIBGE)?.IBGE;

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
        /// Retorna a as informações do estado a partir de um nome de estado, sigla ou numero do IBGEouCEP do estado ou cidade
        /// </summary>
        /// <param name="NomeOuUFOuIBGE">Nome ou UF</param>
        /// <returns></returns>
        public static Estado PegarEstado(string NomeOuUFOuIBGE)
        {
            NomeOuUFOuIBGE = NomeOuUFOuIBGE.TrimBetween().ToSlugCase();
            if (NomeOuUFOuIBGE.CEPValido())
            {
                return Cidades.FirstOrDefault(x => x.ContemCep(NomeOuUFOuIBGE))?.Estado;
            }

            if (NomeOuUFOuIBGE.FormatoCodigoIBGEValido())
            {
                if (NomeOuUFOuIBGE.Length == 7)
                {
                    return Cidades.FirstOrDefault(x => x.IBGE == NomeOuUFOuIBGE.ToInt())?.Estado;
                }
                else
                {
                    return Estados.FirstOrDefault(x => x.IBGE == NomeOuUFOuIBGE.ToInt());
                }
            }

            return Estados.FirstOrDefault(x => (x.Nome.ToSlugCase() ?? Util.EmptyString) == (NomeOuUFOuIBGE ?? Util.EmptyString) || (x.UF.ToSlugCase() ?? Util.EmptyString) == (NomeOuUFOuIBGE ?? Util.EmptyString));
        }

        public static bool FormatoCodigoIBGEValido(this string IBGE) => IBGE.IsNumber() && IBGE.ToInt() > 0 && (IBGE.Length == 7 || IBGE.Length == 2);

        public static bool CidadeIBGEValido(this int IBGE) => CidadeIBGEValido(IBGE.ToString(CultureInfo.InvariantCulture));

        public static bool CidadeIBGEValido(this string IBGE)
        {
            if (IBGE.IsNumber() && IBGE.Length == 7 && IBGE.ToInt() > 0)
            {
                return Cidades.Any(x => x.IBGE == IBGE.ToInt());
            }
            return false;
        }

        public static bool EstadoIBGEValido(this int IBGE) => EstadoIBGEValido(IBGE.ToString(CultureInfo.InvariantCulture));

        public static bool EstadoIBGEValido(this string IBGE)
        {
            if (IBGE.IsNumber() && IBGE.Length == 2 && IBGE.ToInt() > 0)
            {
                return Estados.Any(x => x.IBGE == IBGE.ToInt());
            }
            return false;
        }

        public static bool IBGEValido(this string IBGE) => EstadoIBGEValido(IBGE) || CidadeIBGEValido(IBGE);

        public static bool IBGEValido(this int IBGE) => EstadoIBGEValido(IBGE) || CidadeIBGEValido(IBGE);

        /// <summary>
        /// Retorna a Sigla (UF) a partir de um nome de estado
        /// </summary>
        /// <param name="NomeOuUFOuIBGE"></param>
        /// <returns></returns>
        public static string PegarUfEstado(string NomeOuUFOuIBGE) => PegarEstado(NomeOuUFOuIBGE)?.UF;

        /// <summary>
        /// Retorna os estados de uma região
        /// </summary>
        /// <param name="Regiao"></param>
        /// <returns></returns>
        public static IEnumerable<Estado> PegarEstadosDaRegiao(string Regiao) => Estados.Where(x => (Util.ToSlugCase(x.Regiao) ?? Util.EmptyString) == (Util.TrimBetween(Util.ToSlugCase(Regiao)) ?? Util.EmptyString) || Regiao.IsNotValid());

        /// <summary>
        /// Verifica se uma string é um cep válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool CEPValido(this string CEP) => CEP.IsValid() && new Regex(@"^\d{5}-\d{3}$").IsMatch(CEP) || (CEP.RemoveAny("-").IsNumber() && CEP.RemoveAny("-").Length == 8);

        /// <summary>
        /// Verifica se a string é um CNH válido
        /// </summary>
        /// <param name="Text">CNH</param>
        /// <returns></returns>
        public static bool CNHValido(this string cnh)
        {
            cnh = cnh.OnlyNumbers();
            if (String.IsNullOrEmpty(cnh))
                return false;
            if (cnh.Length != 11)
                return false;
            if (cnh == new string(cnh[0], 11))
                return false;

            int digitoVerificador1 = GerarDigitoVerificadorCNH(cnh.Substring(0, 9), false);
            int diferencial = 0;
            if (digitoVerificador1 == 10)
            {
                digitoVerificador1 = 0;
                diferencial = 2;
            }
            int digitoVerificador2 = GerarDigitoVerificadorCNH(cnh.Substring(0, 9), true);
            digitoVerificador2 = digitoVerificador2 == 10 ? 0 : digitoVerificador2 - diferencial;
            string CNHReal = cnh.Substring(0, 9) + digitoVerificador1.ToString() + digitoVerificador2.ToString();

            return cnh == CNHReal;
        }

        public static int GerarDigitoVerificadorCNH(string digitos, bool crescente)
        {
            int soma = 0;
            int multiplicador = crescente ? 1 : 9;
            for (int indice = 0; indice < digitos.Length; indice++)
            {
                soma += int.Parse(digitos.Substring(indice, 1)) * multiplicador;
                multiplicador += crescente ? 1 : -1;
            }
            return soma % 11;
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
                if (CNPJ.IsValid())
                {
                    var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    int soma;
                    int resto;
                    string digito;
                    string tempCnpj;
                    CNPJ = CNPJ.Trim();
                    CNPJ = CNPJ.Replace(".", Util.EmptyString).Replace("-", Util.EmptyString).Replace("/", Util.EmptyString);
                    if (CNPJ.Length != 14) return false;

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
                        soma += (int.Parse(tempCnpj[i].ToString()) * multiplicador2[i]);
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
                if (CPF.IsValid())
                {
                    CPF = CPF.RemoveAny(".", "-");
                    if (CPF.Length != 11) return false;
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
            if (PIS.IsNotValid())
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

        public static void Reload()
        {
            states = new List<Estado>();
            cities = new List<Cidade>();
            cached = false;
        }

        public static bool ValidarInscricaoEstadual(string estado, string inscricao)
        {
            if (inscricao.FlatEqual("ISENTO")) return true;
            return Brasil.PegarCidade(estado)?.Estado?.ValidarInscricaoEstadual(inscricao) ?? false;
        }

        public static string GerarCPFouCNPJFake() => Util.RandomBool() ? GerarCPFFake() : GerarCNPJFake();

        /// <summary>
        /// Gera um CNPJ falso, válido para testes, mas não existe na Receita Federal
        /// </summary>
        /// <returns>Um CNPJ falso formatado</returns>
        public static string GerarCNPJFake()
        {
            // Gerar os primeiros 8 dígitos aleatórios para a base do CNPJ
            Random random = new Random();
            string baseCNPJ = random.Next(10000000, 99999999).ToString();

            // Adicionar os 4 dígitos fixos para a filial e tipo de registro
            string filialETipo = "0001";

            // Calcular os dígitos verificadores
            string cnpjSemDigitos = baseCNPJ + filialETipo;

            int CalcularDigitoVerificadorCNPJ(string cnpjBase, int[] multiplicador)
            {
                int soma = 0;
                for (int i = 0; i < multiplicador.Length; i++)
                {
                    soma += int.Parse(cnpjBase[i].ToString()) * multiplicador[i];
                }

                int resto = soma % 11;
                return resto < 2 ? 0 : 11 - resto;
            }

            int primeiroDigito = CalcularDigitoVerificadorCNPJ(cnpjSemDigitos, multiplicador: new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
            int segundoDigito = CalcularDigitoVerificadorCNPJ(cnpjSemDigitos + primeiroDigito, multiplicador: new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

            // Retornar o CNPJ formatado
            return $"{baseCNPJ}{filialETipo}{primeiroDigito}{segundoDigito}".FormatarCNPJ();
        }

        /// <summary>
        /// Gera um CPF falso, válido para testes, mas não existe na Receita Federal
        /// </summary>
        /// <returns>Um CPF falso formatado</returns>
        public static string GerarCPFFake()
        {
            // Gera os 9 primeiros dígitos aleatórios
            Random random = new Random();
            int[] cpf = new int[11];
            for (int i = 0; i < 9; i++)
            {
                cpf[i] = random.Next(0, 10);
            }

            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += cpf[i] * (10 - i);
            }
            int resto = soma % 11;
            cpf[9] = (resto < 2) ? 0 : 11 - resto;

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += cpf[i] * (11 - i);
            }
            resto = soma % 11;
            cpf[10] = (resto < 2) ? 0 : 11 - resto;

            // Monta o número do CPF
            string cpfStr = string.Concat(cpf);
            return cpfStr.FormatarCPF();
        }

        public static string GerarTelefoneFake() => Util.RandomInt(90000, 99999).ToString() + Util.RandomInt(1000, 9999).ToString();

        public static string GerarTelefoneFake(string CidadeOuDDDouIBGE)
        {
            var cid = PegarCidade(CidadeOuDDDouIBGE);
            return GerarTelefoneFake(cid);
        }

        public static IEnumerable<int> DDDs => Telefone.DDDs;

        public static bool DDDValido(this int DDD) => Brasil.Cidades.Any(x => x.DDD == DDD);

        public static bool DDDValido(this string DDD) => DDD.IsNumber() && Telefone.DDDs.Contains(DDD.ToInt());

        public static string GerarTelefoneFake(Cidade cidade)
        {
            cidade = cidade ?? Brasil.Cidades.RandomItem();
            return GerarTelefoneFake(cidade.DDD);
        }

        public static string GerarTelefoneFake(int? ddd)
        {
            ddd ??= Brasil.Cidades.RandomItem().DDD;
            return $"({ddd}) {GerarTelefoneFake()}";
        }

        /// <summary>
        /// Gera uma CNH falsa, válida para testes, mas não existe no DETRAN
        /// </summary>
        /// <returns>Uma CNH falsa formatada</returns>
        public static string GerarCNHFake()
        {
            // Gera os 9 primeiros dígitos aleatórios
            Random random = new Random();
            int[] cnh = new int[11];
            for (int i = 0; i < 9; i++)
            {
                cnh[i] = random.Next(0, 10);
            }

            // Calcula o primeiro dígito verificador
            int digitoVerificador1 = GerarDigitoVerificadorCNH(string.Concat(cnh.Take(9)), false);
            int diferencial = 0;
            if (digitoVerificador1 == 10)
            {
                digitoVerificador1 = 0;
                diferencial = 2;
            }
            // Calcula o segundo dígito verificador
            int digitoVerificador2 = GerarDigitoVerificadorCNH(string.Concat(cnh.Take(9)), true);
            digitoVerificador2 = digitoVerificador2 == 10 ? 0 : digitoVerificador2 - diferencial;

            cnh[9] = digitoVerificador1;
            cnh[10] = digitoVerificador2;

            // Monta o número da CNH
            string cnhStr = string.Concat(cnh);
            return cnhStr;
        }
    }

    public class ChaveNFe
    {
        public static implicit operator string(ChaveNFe c) => c?.ToString();

        public static implicit operator ChaveNFe(string c) => new ChaveNFe(c);

        public const int TamanhoCNPJ = 14;

        public const int TamanhoCodigo = 8;

        public const int TamanhoDigito = 1;

        public const int TamanhoMesAno = 4;

        public const int TamanhoModelo = 2;

        public const int TamanhoNota = 9;

        public const int TamanhoSerie = 3;

        public const int TamanhoUF = 2;

        public const int TamanhoFormaEmissao = 1;

        [IgnoreDataMember]
        public string Tipo
        {
            get
            {
                switch (ModeloFixo)
                {
                    case "55":
                        return "NF-e";

                    case "57":
                        return "CT-e";

                    case "65":
                        return "NFC-e";

                    default:
                        return "DF-e Desconhecido";
                }
            }
            set => ModeloFixo = value.NullIf(x => x.IsNotIn(new[] { "55", "57", "65" })) ?? "0";
        }

        public ChaveNFe()
        {
        }

        public ChaveNFe(string UF, int Ano, int Mes, string CNPJ, int Modelo, int Serie, int Nota, int FormaEmissao, int Codigo)
        {
            UFFixo = UF;
            this.Ano = Ano;
            this.Mes = Mes;
            this.CNPJFormatado = CNPJ;
            this.Modelo = Modelo;
            this.Serie = Serie;
            this.Nota = Nota;
            this.FormaEmissao = FormaEmissao;
            this.Codigo = Codigo;
            CalcularDigito();
        }

        public ChaveNFe(string UF, DateTime Emissao, string CNPJ, int Modelo, int Serie, int Nota, int FormaEmissao, int Codigo)
        {
            UFFixo = UF;
            this.MesEmissao = Emissao;
            this.CNPJFormatado = CNPJ;
            this.Modelo = Modelo;
            this.Serie = Serie;
            this.Nota = Nota;
            this.FormaEmissao = FormaEmissao;
            this.Codigo = Codigo;
            CalcularDigito();
        }

        public ChaveNFe(string Chave) => this.Chave = Chave.RemoveMask();

        public int Ano { get; set; }

        public string Chave
        {
            get => ToString();
            set
            {
                var c = value.IfBlank("0").RemoveMask();
                if (c.Length == 43)
                {
                    c += CalcularDigito(c);
                }

                if (c.Length != 44)
                {
                    c = "".PadLeft(44, '0');
                }

                var parts = c.SplitChunk(TamanhoUF, TamanhoMesAno - 2, TamanhoMesAno - 2, TamanhoCNPJ, TamanhoModelo, TamanhoSerie, TamanhoNota, TamanhoFormaEmissao, TamanhoCodigo, TamanhoDigito).ToArray();

                UF = parts[0].ToInt();
                Ano = parts[1].ToInt();
                Mes = parts[2].ToInt();
                CNPJ = parts[3].ToLong();
                Modelo = parts[4].ToInt();
                Serie = parts[5].ToInt();
                Nota = parts[6].ToInt();
                FormaEmissao = parts[7].ToInt();
                Codigo = parts[8].ToInt();
                Digito = parts[9].ToInt();
            }
        }

        public int FormaEmissao { get; set; }

        [IgnoreDataMember]
        public string FormaEmissaoFixo { get => FormaEmissao.FixedLength(TamanhoFormaEmissao); set => FormaEmissao = value.RemoveMaskInt(); }

        public long CNPJ { get; set; }

        [IgnoreDataMember]
        public string CNPJFixo
        {
            get => CNPJ.FixedLength(TamanhoCNPJ);
            set => CNPJ = value.RemoveMask().ToLong();
        }

        [IgnoreDataMember]
        public string CNPJFormatado { get => CNPJFixo.FormatarCNPJ(); set => CNPJ = value.RemoveMask().IfBlank(0L); }

        public int Codigo { get; set; }

        [IgnoreDataMember]
        public string CodigoFixo
        {
            get => Codigo.FixedLength(TamanhoCodigo);
            set => Codigo = value.RemoveMaskInt();
        }

        public int? Digito { get; set; }

        [IgnoreDataMember]
        public string DigitoFixo
        {
            get => Digito?.FixedLength(TamanhoDigito);
            set => Digito = value?.RemoveMaskInt();
        }

        [IgnoreDataMember]
        public string ChaveFormatadaTraco => $"{UFFixo}-{MesAno}-{CNPJFixo}-{ModeloFixo}-{SerieFixo}-{NotaFixo}-{FormaEmissaoFixo}-{CodigoFixo}-{DigitoFixo}";

        [IgnoreDataMember]
        public string ChaveFormatadaComEspacos => Regex.Replace(Chave, ".{4}", "$0 ").TrimEnd();

        public int Mes { get; set; }

        [IgnoreDataMember]
        public string MesAno
        {
            get => $"{Ano.FixedLength(TamanhoMesAno - 2)}{Mes.FixedLength(TamanhoMesAno - 2)}";
            set
            {
                if (value.IsValid() && value.RemoveMask().Length == 4)
                {
                    Mes = value.RemoveMask().GetFirstChars(2).ToInt();
                    Ano = value.RemoveMask().GetLastChars(2).ToInt();
                }
            }
        }

        public int Modelo { get; set; }

        [IgnoreDataMember]
        public string ModeloFixo
        {
            get => Modelo.IfBlank(55).FixedLength(TamanhoModelo);
            set => Modelo = value.RemoveMaskInt();
        }

        [IgnoreDataMember]
        public DateTime MesEmissao
        {
            get => new DateTime(2000 + Ano, Mes, 1);
            set
            {
                this.Mes = value.Month;
                this.Ano = value.Year;
            }
        }

        public int Nota { get; set; }

        [IgnoreDataMember]
        public string NotaFixo
        {
            get => Nota.FixedLength(TamanhoNota);
            set => Nota = value.RemoveMaskInt();
        }

        public int Serie { get; set; }

        [IgnoreDataMember]
        public string SerieFixo
        {
            get => Serie.FixedLength(TamanhoSerie);
            set => Serie = value.RemoveMaskInt();
        }

        [IgnoreDataMember]
        public Estado Estado { get => Brasil.PegarEstado($"{UF}"); set => UF = value?.IBGE ?? 0; }

        public int UF { get; set; }

        [IgnoreDataMember]
        public string UFFixo
        {
            get => UF.FixedLength(TamanhoUF);
            set => UF = Brasil.PegarEstado(value)?.IBGE ?? value.RemoveMaskInt();
        }

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

        public ChaveNFe CalcularDigito()
        {
            Digito = CalcularDigito(Chave);
            return this;
        }

        public override string ToString() => ChaveFormatadaTraco.RemoveMask();
    }

    public class Cidade : IEqualityComparer<Cidade>
    {
        public static bool operator ==(Cidade x, Cidade y) => x?.Equals(y) ?? false;

        public static bool operator !=(Cidade x, Cidade y) => !x?.Equals(y) ?? false;

        public bool Equals(Cidade x, Cidade y) => x != null && y != null && x.IBGE == y.IBGE && x.IBGE > 0;

        public int GetHashCode(Cidade obj) => obj.IBGE.GetHashCode();

        public int Id { get; internal set; }

        public string Nome { get; internal set; }

        public IEnumerable<string> Keywords { get; set; }

        public long CepInicial { get; internal set; }

        public long CepFinal { get; internal set; }

        public bool ExclusivaSedeUrbana { get; internal set; }
        public bool TotalMunicipio { get; internal set; }

        public string Latitude { get; internal set; }
        public string Longitude { get; internal set; }
        public string CodigoGeograficoSubdivisao { get; internal set; }
        public string CodigoGeograficoDistrito { get; internal set; }
        public string MicroRegiao { get; internal set; }
        public string MacroRegiao { get; internal set; }
        public string Altitude { get; internal set; }
        public int DDD { get; internal set; }
        public bool Capital { get; internal set; }
        public string TimeZone { get; internal set; }
        public string UF => Estado?.UF;
        public string Pais => Estado?.Pais;

        public string CodigoPais => Estado?.CodigoPais;

        public string Regiao => Estado?.Regiao;

        public string CidadeEstado => $"{Nome} - {Estado?.UF}".TrimAny("-", " ");

        public int IBGEEstado => Estado?.IBGE ?? 0;

        [IgnoreDataMember]
        public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.FindSystemTimeZoneById(TimeZone);

        [DataMember(Name = "ibge_id")]
        public int IBGE { get; internal set; }

        [DataMember(Name = "siafi_id")]
        public string SIAFI { get; internal set; }

        [IgnoreDataMember]
        public Estado Estado { get; internal set; }

        public bool ContemCep(long Cep) => Cep.IsBetweenOrEqual(CepInicial, CepFinal);

        public bool ContemCep(string Cep) => Brasil.CEPValido(Cep) && ContemCep(Cep.OnlyNumbersLong());

        public override string ToString() => CidadeEstado;
    }

    /// <summary>
    /// Objeto que representa um estado do Brasil e seus respectivos detalhes
    /// </summary>
    public class Estado : IEqualityComparer<Estado>
    {
        /// <summary>
        /// Lista de cidades do estado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cidade> Cidades => Brasil.Cidades.Where(x => x.IBGE.ToString(CultureInfo.InvariantCulture).GetFirstChars(2).FlatEqual(IBGE));

        public Cidade Capital => Cidades?.FirstOrDefault(x => x.Capital);

        public string Pais { get; internal set; }

        public string CodigoPais { get; internal set; }

        public int IBGE { get; internal set; }

        public string Latitude { get; internal set; }

        public string Longitude { get; internal set; }

        /// <summary>
        /// Nome do estado
        /// </summary>
        /// <returns></returns>
        public string Nome { get; internal set; }

        public string Regiao { get; internal set; }
        public string UF { get; internal set; }

        /// <summary>
        /// Retorna a String correspondente ao estado
        /// </summary>
        /// <returns></returns>
        public override string ToString() => UF;

        public static Estado AC => Brasil.PegarEstado("AC");
        public static Estado AL => Brasil.PegarEstado("AL");
        public static Estado AP => Brasil.PegarEstado("AP");
        public static Estado AM => Brasil.PegarEstado("AM");
        public static Estado BA => Brasil.PegarEstado("BA");
        public static Estado CE => Brasil.PegarEstado("CE");
        public static Estado DF => Brasil.PegarEstado("DF");
        public static Estado ES => Brasil.PegarEstado("ES");
        public static Estado GO => Brasil.PegarEstado("GO");
        public static Estado MA => Brasil.PegarEstado("MA");
        public static Estado MT => Brasil.PegarEstado("MT");
        public static Estado MS => Brasil.PegarEstado("MS");
        public static Estado MG => Brasil.PegarEstado("MG");
        public static Estado PA => Brasil.PegarEstado("PA");
        public static Estado PB => Brasil.PegarEstado("PB");
        public static Estado PR => Brasil.PegarEstado("PR");
        public static Estado PE => Brasil.PegarEstado("PE");
        public static Estado PI => Brasil.PegarEstado("PI");
        public static Estado RJ => Brasil.PegarEstado("RJ");
        public static Estado RN => Brasil.PegarEstado("RN");
        public static Estado RS => Brasil.PegarEstado("RS");
        public static Estado RO => Brasil.PegarEstado("RO");
        public static Estado RR => Brasil.PegarEstado("RR");
        public static Estado SC => Brasil.PegarEstado("SC");
        public static Estado SP => Brasil.PegarEstado("SP");
        public static Estado SE => Brasil.PegarEstado("SE");
        public static Estado TO => Brasil.PegarEstado("TO");

        /// <summary>
        /// Verifica se uma string é uma Inscricao Estadual Válida
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public bool ValidarInscricaoEstadual(string inscricao) => ValidarInscricaoEstadual(inscricao, UF);

        /// <summary>
        /// Verifica se uma string é uma Inscricao Estadual Válida
        /// </summary>
        /// <param name="inscricao"></param>
        /// <returns></returns>
        public static bool ValidarInscricaoEstadual(string inscricao, string estado)
        {
            bool retorno = false;
            string strBase;
            string strBase2;
            string strOrigem;
            string strDigito1;
            string strDigito2;
            int intPos;
            int intValor;
            int intSoma = 0;
            int intResto;
            int intNumero;
            int intPeso = 0;
            strBase = "";
            strBase2 = "";
            strOrigem = "";

            if ((inscricao.Trim().ToUpper() == "ISENTO"))
                return true;

            for (intPos = 1; intPos <= inscricao.Trim().Length; intPos++)
            {
                if ((("0123456789P".IndexOf(inscricao.Substring((intPos - 1), 1), 0, StringComparison.OrdinalIgnoreCase) + 1) > 0))
                    strOrigem = (strOrigem + inscricao.Substring((intPos - 1), 1));
            }

            switch (Brasil.PegarUfEstado(estado))
            {
                case "AC":
                    #region
                    strBase = (strOrigem.Trim() + "00000000000").Substring(0, 11);
                    if (strBase.Substring(0, 2) == "01")
                    {
                        intSoma = 0;
                        intPeso = 4;
                        for (intPos = 1; (intPos <= 11); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            if (intPeso == 1) intPeso = 9;
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intResto = (intSoma % 11);
                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        intSoma = 0;
                        strBase = (strOrigem.Trim() + "000000000000").Substring(0, 12);
                        intPeso = 5;

                        for (intPos = 1; (intPos <= 12); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            if (intPeso == 1) intPeso = 9;
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intResto = (intSoma % 11);
                        strDigito2 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 12) + strDigito2);
                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "AL":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    if ((strBase.Substring(0, 2) == "24"))
                    {
                        //24000004-8
                        //98765432
                        intSoma = 0;
                        intPeso = 9;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intSoma = (intSoma * 10);
                        intResto = (intSoma % 11);
                        strDigito1 = ((intResto == 10) ? "0" : Convert.ToString(intResto)).Substring((((intResto == 10) ? "0" : Convert.ToString(intResto)).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "AM":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intSoma = 0;
                    intPeso = 9;
                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intSoma += intValor * intPeso;
                        intPeso--;
                    }

                    intResto = (intSoma % 11);
                    if (intSoma < 11)
                        strDigito1 = (11 - intSoma).ToString();
                    else
                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));

                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "AP":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intPeso = 9;
                    if ((strBase.Substring(0, 2) == "03"))
                    {
                        strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intResto = (intSoma % 11);
                        intValor = (11 - intResto);

                        strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);
                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "BA":
                    #region
                    if (strOrigem.Length == 8)
                        strBase = (strOrigem.Trim() + "00000000").Substring(0, 8);
                    else if (strOrigem.Length == 9)
                        strBase = (strOrigem.Trim() + "00000000").Substring(0, 9);
                    if ((("0123458".IndexOf(strBase.Substring(0, 1), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0) && strBase.Length == 8)
                    {
                        #region
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 6); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            if (intPos == 1) intPeso = 7;
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intResto = (intSoma % 10);
                        strDigito2 = ((intResto == 0) ? "0" : Convert.ToString((10 - intResto))).Substring((((intResto == 0) ? "0" : Convert.ToString((10 - intResto))).Length - 1));
                        strBase2 = strBase.Substring(0, 7) + strDigito2;
                        if (strBase2 == strOrigem)
                            retorno = true;
                        if (retorno)
                        {
                            intSoma = 0;
                            intPeso = 0;
                            for (intPos = 1; (intPos <= 7); intPos++)
                            {
                                intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                                if (intPos == 7)
                                    intValor = int.Parse(strBase.Substring((intPos), 1));
                                if (intPos == 1) intPeso = 8;

                                intSoma += intValor * intPeso;
                                intPeso--;
                            }

                            intResto = (intSoma % 10);
                            strDigito1 = ((intResto == 0) ? "0" : Convert.ToString((10 - intResto))).Substring((((intResto == 0) ? "0" : Convert.ToString((10 - intResto))).Length - 1));
                            strBase2 = (strBase.Substring(0, 6) + strDigito1 + strDigito2);
                            if ((strBase2 == strOrigem))
                                retorno = true;
                        }
                        #endregion
                    }
                    else if ((("679".IndexOf(strBase.Substring(0, 1), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0) && strBase.Length == 8)
                    {
                        #region
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 6); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            if (intPos == 1) intPeso = 7;
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intResto = (intSoma % 11);
                        strDigito2 = ((intResto == 0) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto == 0) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        strBase2 = strBase.Substring(0, 7) + strDigito2;
                        if (strBase2 == strOrigem)
                            retorno = true;
                        if (retorno)
                        {
                            intSoma = 0;
                            intPeso = 0;
                            for (intPos = 1; (intPos <= 7); intPos++)
                            {
                                intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                                if (intPos == 7)
                                    intValor = int.Parse(strBase.Substring((intPos), 1));

                                if (intPos == 1) intPeso = 8;
                                intSoma += intValor * intPeso;
                                intPeso--;
                            }

                            intResto = (intSoma % 11);
                            strDigito1 = ((intResto == 0) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto == 0) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                            strBase2 = (strBase.Substring(0, 6) + strDigito1 + strDigito2);
                            if ((strBase2 == strOrigem))
                                retorno = true;
                        }

                        #endregion
                    }
                    else if ((("0123458".IndexOf(strBase.Substring(1, 1), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0) && strBase.Length == 9)
                    {
                        #region
                        /* Segundo digito */
                        //1000003
                        //8765432
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 7); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            if (intPos == 1) intPeso = 8;
                            intSoma += intValor * intPeso;
                            intPeso--;
                        }

                        intResto = (intSoma % 10);
                        strDigito2 = ((intResto == 0) ? "0" : Convert.ToString((10 - intResto))).Substring((((intResto == 0) ? "0" : Convert.ToString((10 - intResto))).Length - 1));
                        strBase2 = strBase.Substring(0, 8) + strDigito2;
                        if (strBase2 == strOrigem)
                            retorno = true;

                        if (retorno)
                        {
                            //1000003 6
                            //9876543 2
                            intSoma = 0;
                            intPeso = 0;
                            for (intPos = 1; (intPos <= 8); intPos++)
                            {
                                intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                                if (intPos == 8)
                                    intValor = int.Parse(strBase.Substring((intPos), 1));

                                if (intPos == 1) intPeso = 9;
                                intSoma += intValor * intPeso;
                                intPeso--;
                            }

                            intResto = (intSoma % 10);
                            strDigito1 = ((intResto == 0) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto == 0) ? "0" : Convert.ToString((11 - intResto))).Length - 1));

                            strBase2 = (strBase.Substring(0, 7) + strDigito1 + strDigito2);
                            if ((strBase2 == strOrigem))
                                retorno = true;
                        }
                        #endregion
                    }
                    #endregion
                    break;

                case "CE":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    intSoma = 0;
                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * (10 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    intValor = (11 - intResto);
                    if ((intValor > 9))
                        intValor = 0;

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));

                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "DF":

                    #region
                    strBase = (strOrigem.Trim() + "0000000000000").Substring(0, 13);
                    if ((strBase.Substring(0, 3) == "073"))
                    {
                        intSoma = 0;
                        intPeso = 2;
                        for (intPos = 11; (intPos >= 1); intPos = (intPos + -1))
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);

                            if ((intPeso > 9))
                                intPeso = 2;
                        }

                        intResto = (intSoma % 11);

                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));

                        strBase2 = (strBase.Substring(0, 11) + strDigito1);

                        intSoma = 0;

                        intPeso = 2;

                        for (intPos = 12; (intPos >= 1); intPos = (intPos + -1))
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);
                            if ((intPeso > 9))
                                intPeso = 2;
                        }

                        intResto = (intSoma % 11);
                        strDigito2 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 12) + strDigito2);
                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "ES":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intSoma = 0;

                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * (10 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "GO":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((("10,11,15".IndexOf(strBase.Substring(0, 2), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0))
                    {
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * (10 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intResto = (intSoma % 11);
                        if ((intResto == 0))
                            strDigito1 = "0";
                        else if ((intResto == 1))
                        {
                            intNumero = int.Parse(strBase.Substring(0, 8));
                            strDigito1 = (((intNumero >= 10103105) && (intNumero <= 10119997)) ? "1" : "0").Substring(((((intNumero >= 10103105) && (intNumero <= 10119997)) ? "1" : "0").Length - 1));
                        }
                        else
                            strDigito1 = Convert.ToString((11 - intResto)).Substring((Convert.ToString((11 - intResto)).Length - 1));

                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "MA":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "12"))
                    {
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * (10 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intResto = (intSoma % 11);

                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));

                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "MT":
                    #region
                    strBase = (strOrigem.Trim() + "0000000000").Substring(0, 10);
                    intSoma = 0;
                    intPeso = 2;

                    for (intPos = 10; intPos >= 1; intPos = (intPos + -1))
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * intPeso);
                        intSoma = (intSoma + intValor);
                        intPeso = (intPeso + 1);
                        if ((intPeso > 9))
                            intPeso = 2;
                    }

                    intResto = (intSoma % 11);
                    strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 10) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "MS":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "28"))
                    {
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * (10 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intResto = (intSoma % 11);
                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "MG":
                    if (strBase.Trim().Length > 12)
                    {
                        strBase = (strOrigem.Trim() + "0000000000000").Substring(0, 13);
                        strBase2 = strBase.Substring(0, 3) + "0" + strBase.Substring(3, 9);
                        intNumero = 1;

                        intSoma = 0;

                        for (intPos = 0; intPos < 12; intPos++)
                        {
                            if (int.Parse(strBase2.Substring(intPos, 1)) * intNumero >= 10)
                                intSoma += (int.Parse(strBase2.Substring(intPos, 1)) * intNumero) - 9;
                            else
                                intSoma += int.Parse(strBase2.Substring(intPos, 1)) * intNumero;

                            intNumero = intNumero + 1;

                            if (intNumero == 3)
                                intNumero = 1;
                        }

                        intNumero = (int)((Math.Floor((Convert.ToDecimal(intSoma) + 10) / 10) * 10) - intSoma);
                        if (intNumero % 10 == 0)
                            intNumero = 0;

                        if (intNumero != Convert.ToInt32(strOrigem.Substring(11, 1)))
                            return false;

                        intNumero = 3;
                        intSoma = 0;

                        for (intPos = 0; intPos < 12; intPos++)
                        {
                            intSoma += int.Parse(strOrigem.Substring(intPos, 1)) * intNumero;

                            intNumero = intNumero - 1;
                            if (intNumero == 1)
                                intNumero = 11;
                        }

                        intNumero = 11 - (intSoma % 11);
                        if (intNumero >= 10)
                            intNumero = 0;

                        retorno = intNumero == Convert.ToInt32(strOrigem.Substring(12, 1));
                    }
                    break;

                case "PA":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "15"))
                    {
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * (10 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intResto = (intSoma % 11);
                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "PB":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intSoma = 0;

                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * (10 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    intValor = (11 - intResto);

                    if ((intValor > 9))
                        intValor = 0;

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;

                    #endregion
                    break;

                case "PE":
                    #region
                    strBase = (strOrigem.Trim() + "00000000000000").Substring(0, 14);
                    intSoma = 0;
                    intPeso = 2;

                    for (intPos = 7; (intPos >= 1); intPos = (intPos + -1))
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * intPeso);
                        intSoma = (intSoma + intValor);
                        intPeso = (intPeso + 1);

                        if ((intPeso > 9))
                            intPeso = 2;
                    }

                    intResto = (intSoma % 11);
                    intValor = (11 - intResto);

                    if ((intValor > 9))
                        intValor = (intValor - 10);

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 7) + strDigito1);

                    if ((strBase2 == strOrigem.Substring(0, 8)))
                        retorno = true;

                    if (retorno)
                    {
                        intSoma = 0;
                        intPeso = 2;

                        for (intPos = 8; (intPos >= 1); intPos = (intPos + -1))
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);

                            if ((intPeso > 9))
                                intPeso = 2;
                        }

                        intResto = (intSoma % 11);
                        intValor = (11 - intResto);

                        if ((intValor > 9))
                            intValor = (intValor - 10);

                        strDigito2 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito2);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "PI":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intSoma = 0;

                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * (10 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "PR":
                    #region
                    strBase = (strOrigem.Trim() + "0000000000").Substring(0, 10);
                    intSoma = 0;
                    intPeso = 2;

                    for (intPos = 8; (intPos >= 1); intPos = (intPos + -1))
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * intPeso);
                        intSoma = (intSoma + intValor);

                        intPeso = (intPeso + 1);
                        if ((intPeso > 7))
                            intPeso = 2;
                    }

                    intResto = (intSoma % 11);
                    strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);
                    intSoma = 0;
                    intPeso = 2;

                    for (intPos = 9; (intPos >= 1); intPos = (intPos + -1))
                    {
                        intValor = int.Parse(strBase2.Substring((intPos - 1), 1));
                        intValor = (intValor * intPeso);
                        intSoma = (intSoma + intValor);
                        intPeso = (intPeso + 1);

                        if ((intPeso > 7))
                            intPeso = 2;
                    }

                    intResto = (intSoma % 11);
                    strDigito2 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase2 + strDigito2);

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "RJ":
                    #region
                    strBase = (strOrigem.Trim() + "00000000").Substring(0, 8);
                    intSoma = 0;
                    intPeso = 2;

                    for (intPos = 7; (intPos >= 1); intPos = (intPos + -1))
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * intPeso);
                        intSoma = (intSoma + intValor);
                        intPeso = (intPeso + 1);

                        if ((intPeso > 7))
                            intPeso = 2;
                    }

                    intResto = (intSoma % 11);
                    strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 7) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;

                    #endregion
                    break;

                case "RN": //Verficar com 10 digitos
                    #region

                    if (strOrigem.Length == 9)
                        strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    else if (strOrigem.Length == 10)
                        strBase = (strOrigem.Trim() + "000000000").Substring(0, 10);

                    if ((strBase.Substring(0, 2) == "20") && strBase.Length == 9)
                    {
                        intSoma = 0;

                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * (10 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intSoma = (intSoma * 10);
                        intResto = (intSoma % 11);
                        strDigito1 = ((intResto > 9) ? "0" : Convert.ToString(intResto)).Substring((((intResto > 9) ? "0" : Convert.ToString(intResto)).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    else if (strBase.Length == 10)
                    {
                        intSoma = 0;

                        for (intPos = 1; (intPos <= 9); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * (11 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intSoma = (intSoma * 10);
                        intResto = (intSoma % 11);

                        strDigito1 = ((intResto > 10) ? "0" : Convert.ToString(intResto)).Substring((((intResto > 10) ? "0" : Convert.ToString(intResto)).Length - 1));
                        strBase2 = (strBase.Substring(0, 9) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "RO":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    strBase2 = strBase.Substring(3, 5);
                    intSoma = 0;

                    for (intPos = 1; (intPos <= 5); intPos++)
                    {
                        intValor = int.Parse(strBase2.Substring((intPos - 1), 1));
                        intValor = (intValor * (7 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    intValor = (11 - intResto);

                    if ((intValor > 9))
                        intValor = (intValor - 10);

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "RR":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);

                    if ((strBase.Substring(0, 2) == "24"))
                    {
                        intSoma = 0;
                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = intValor * intPos;
                            intSoma += intValor;
                        }

                        intResto = (intSoma % 9);
                        strDigito1 = Convert.ToString(intResto).Substring((Convert.ToString(intResto).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "RS":
                    #region
                    strBase = (strOrigem.Trim() + "0000000000").Substring(0, 10);
                    intNumero = int.Parse(strBase.Substring(0, 3));

                    if (((intNumero > 0) && (intNumero < 468)))
                    {
                        intSoma = 0;
                        intPeso = 2;

                        for (intPos = 9; (intPos >= 1); intPos = (intPos + -1))
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);

                            if ((intPeso > 9))
                                intPeso = 2;
                        }

                        intResto = (intSoma % 11);
                        intValor = (11 - intResto);
                        if ((intValor > 9))
                            intValor = 0;

                        strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                        strBase2 = (strBase.Substring(0, 9) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion
                    break;

                case "SC":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intSoma = 0;

                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * (10 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "SE":
                    #region
                    strBase = (strOrigem.Trim() + "000000000").Substring(0, 9);
                    intSoma = 0;

                    for (intPos = 1; (intPos <= 8); intPos++)
                    {
                        intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                        intValor = (intValor * (10 - intPos));
                        intSoma = (intSoma + intValor);
                    }

                    intResto = (intSoma % 11);
                    intValor = (11 - intResto);

                    if ((intValor > 9))
                        intValor = 0;

                    strDigito1 = Convert.ToString(intValor).Substring((Convert.ToString(intValor).Length - 1));
                    strBase2 = (strBase.Substring(0, 8) + strDigito1);

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "SP":
                    #region
                    if ((strOrigem.Substring(0, 1) == "P"))
                    {
                        strBase = (strOrigem.Trim() + "0000000000000").Substring(0, 13);
                        strBase2 = strBase.Substring(1, 8);
                        intSoma = 0;
                        intPeso = 1;

                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);

                            if ((intPeso == 2))
                                intPeso = 3;

                            if ((intPeso == 9))
                                intPeso = 10;
                        }

                        intResto = (intSoma % 11);
                        strDigito1 = Convert.ToString(intResto).Substring((Convert.ToString(intResto).Length - 1));
                        strBase2 = (strBase.Substring(0, 9) + (strDigito1 + strBase.Substring(10, 3)));
                    }
                    else
                    {
                        strBase = (strOrigem.Trim() + "000000000000").Substring(0, 12);
                        intSoma = 0;
                        intPeso = 1;

                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);

                            if ((intPeso == 2))
                                intPeso = 3;

                            if ((intPeso == 9))
                                intPeso = 10;
                        }

                        intResto = (intSoma % 11);
                        strDigito1 = Convert.ToString(intResto).Substring((Convert.ToString(intResto).Length - 1));
                        strBase2 = (strBase.Substring(0, 8) + (strDigito1 + strBase.Substring(9, 2)));
                        intSoma = 0;
                        intPeso = 2;

                        for (intPos = 11; (intPos >= 1); intPos = (intPos + -1))
                        {
                            intValor = int.Parse(strBase.Substring((intPos - 1), 1));
                            intValor = (intValor * intPeso);
                            intSoma = (intSoma + intValor);
                            intPeso = (intPeso + 1);
                            if ((intPeso > 10))
                                intPeso = 2;
                        }

                        intResto = (intSoma % 11);
                        strDigito2 = Convert.ToString(intResto).Substring((Convert.ToString(intResto).Length - 1));
                        strBase2 = (strBase2 + strDigito2);
                    }

                    if ((strBase2 == strOrigem))
                        retorno = true;
                    #endregion
                    break;

                case "TO":
                    #region
                    strBase = (strOrigem.Trim() + "00000000000").Substring(0, 11);
                    if ((("01,02,03,99".IndexOf(strBase.Substring(2, 2), 0, System.StringComparison.OrdinalIgnoreCase) + 1) > 0))
                    {
                        strBase2 = (strBase.Substring(0, 2) + strBase.Substring(4, 6));
                        intSoma = 0;

                        for (intPos = 1; (intPos <= 8); intPos++)
                        {
                            intValor = int.Parse(strBase2.Substring((intPos - 1), 1));
                            intValor = (intValor * (10 - intPos));
                            intSoma = (intSoma + intValor);
                        }

                        intResto = (intSoma % 11);
                        strDigito1 = ((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Substring((((intResto < 2) ? "0" : Convert.ToString((11 - intResto))).Length - 1));
                        strBase2 = (strBase.Substring(0, 10) + strDigito1);

                        if ((strBase2 == strOrigem))
                            retorno = true;
                    }
                    #endregion

                    break;

                default:
                    return Brasil.Estados.Any(x => x.ValidarInscricaoEstadual(inscricao));
            }

            return retorno;
        }

        public static bool operator ==(Estado x, Estado y) => x?.Equals(y) ?? false;

        public static bool operator !=(Estado x, Estado y) => !x?.Equals(y) ?? false;

        public bool Equals(Estado x, Estado y) => x != null && y != null && x.IBGE == y.IBGE && x.IBGE > 0;

        public int GetHashCode(Estado obj) => obj.IBGE.GetHashCode();
    }



 


}