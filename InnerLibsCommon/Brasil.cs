﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using Extensions;
using Extensions.Locations;

namespace Extensions.BR
{
    /// <summary>
    /// Objeto para manipular operações relacionadas ao Brasil
    /// </summary>
    public static class Brasil
    {
        /// <inheritdoc cref = "ValidarTelefone(string)" />
        public static bool ValidarTelefone(int telefone)
        => ValidarTelefone(telefone.ToString());

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
        public static bool ValidarTelefone(string telefone)
        {
            try
            {
                string telefoneStr = telefone.ToString();
                string apenasNumeros = telefoneStr.OnlyNumbers();

                if (apenasNumeros.Length == 13 && apenasNumeros.StartsWith("55"))
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
                    if (ddd < 11 || ddd > 99)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static List<Estado> l = new List<Estado>();

        /// <summary>
        /// Array contendo os nomes mais comuns no Brasil
        /// </summary>
        public static IEnumerable<string> NomesComuns => new[] { "Miguel", "Arthur", "Davi", "Gabriel", "Pedro", "Alice", "Sophia", "Sofia", "Manuela", "Isabella", "Laura", "Heitor", "Enzo", "Lorenzo", "Valentina", "Giovanna", "Giovana", "Maria Eduarda", "Beatriz", "Maria Clara", "Vinícius", "Rafael", "Lara", "Mariana", "Helena", "Mariana", "Isadora", "Lívia", "Luana", "Maria Luíza", "Luiza", "Ana Luiza", "Eduarda", "Letícia", "Lara", "Melissa", "Maria Fernanda", "Cecília", "Lorena", "Clara", "Gustavo", "Matheus", "João Pedro", "Breno", "Felipe", "Júlia", "Carolina", "Caroline", "Joaquim", "Enzo Gabriel", "Thiago", "Lucas", "Giovanni", "Bianca", "Sophie", "Antônio", "Benjamin", "Vitória", "Isabelly", "Amanda", "Emilly", "Maria Cecília", "Marina", "Analu", "Nina", "Júlia", "Gustavo Henrique", "Miguel", "Catarina", "Stella", "Miguel Henrique", "Guilherme", "Caio", "Maria Vitória", "Isis", "Heloísa", "Gabriela", "Eloá", "Agatha", "Arthur Miguel", "Luiza", "Pedro Henrique", "Ana Beatriz", "Ruan", "Sophia", "Lara", "Luana", "Bárbara", "Kaique", "Raissa", "Rafaela", "Maria Valentina", "Bernardo", "Mirella", "Leonardo", "Davi Lucas", "Luiz Felipe", "Emanuel", "Maria Alice", "Luana", "Luna", "Enrico" };

        /// <summary>
        /// Array contendo os sobrenomes mais comuns no Brasil
        /// </summary>
        public static IEnumerable<string> SobrenomesComuns => new[] { "Silva", "Santos", "Souza", "Oliveira", "Pereira", "Ferreira", "Alves", "Pinto", "Ribeiro", "Rodrigues", "Costa", "Carvalho", "Gomes", "Martins", "Araújo", "Melo", "Barbosa", "Cardoso", "Nascimento", "Lima", "Moura", "Cavalcanti", "Monteiro", "Moreira", "Nunes", "Sales", "Ramos", "Montenegro", "Siqueira", "Borges", "Teixeira", "Amaral", "Sampaio", "Correa", "Fernandes", "Batista", "Miranda", "Leal", "Xavier", "Marques", "Andrade", "Freitas", "Paiva", "Vieira", "Aguiar", "Macedo", "Garcia", "Lacerda", "Lopes" };

        public static string GerarNomeAleatorio(bool SobrenomeUnico = false)
        {
            var s1 = SobrenomesComuns.RandomItem();
            var s2 = SobrenomesComuns.RandomItem().NullIf(x => Util.RandomBool() || x == s1 || SobrenomeUnico);
            return $"{NomesComuns.RandomItem()} {s1} {s2}".Trim();
        }

        public static Image GerarAvatarAleatorio(bool SobrenomeUnico = false) => GerarNomeAleatorio(SobrenomeUnico).GenerateAvatarByName();

        public static AddressInfo GerarEnderecoFake() => GerarEnderecoFake<AddressInfo>();
        public static T GerarEnderecoFake<T>(string Label = "Casa") where T : AddressInfo
        {
            var e = Estados.RandomItem();
            var ad = CriarAddressInfo<T>(e.Nome, e.Cidades.RandomItem().Nome);
            ad.Street = $"{AddressTypes.Avenida.Union(AddressTypes.Rua).Union(AddressTypes.Travessa).Union(AddressTypes.Alameda).RandomItem().AppendIf(".", x => x.Length < 3)} {GerarNomeAleatorio()}";
            ad.Neighborhood = GerarNomeAleatorio(true).PrependIf(new[] { "Jardim ", "Campos " }.RandomItem(), Util.RandomBool(75));
            ad.Number = Util.RandomInt(10, 2000).ToString();
            ad.ZipCode = Util.RandomInt(11111111, 99999999).FormatarCEP();
            ad.Label = Label;
            ad.Complement = new[]
            {
                "",
                $"Casa {PredefinedArrays.AlphaUpperChars.Take(4).RandomItem()}",
                $"Apto. {Util.RandomInt(11,209)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Apto. {Util.RandomInt(11,200)}",
                $"Bloco {PredefinedArrays.AlphaUpperChars.Take(20).RandomItem()} Casa {Util.RandomInt(1,12)}",
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
                                node["SIAFI"].InnerText,
                                node["TimeZone"].InnerText,
                                node["Latitude"].InnerText.ToDecimal(),
                                node["Longitude"].InnerText.ToDecimal(),
                                node["Capital"].InnerText.AsBool()
                                ));
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
        /// <param name="Cidade"></param>
        /// <returns></returns>
        public static AddressInfo CriarAddressInfo(string NomeOuUFouIBGE, string Cidade) => CriarAddressInfo<AddressInfo>(NomeOuUFouIBGE, Cidade);
        public static AddressInfo CriarAddressInfo(string CidadeOuIBGE) => CriarAddressInfo<AddressInfo>(CidadeOuIBGE);

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



        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGE. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE) => PegarCidade(NomeDaCidadeOuIBGE, 100);
        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGE. A busca por nome da cidade é feita a partir da similaridade entre o nome fornecido e o nome cadastrado no banco de dados. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE, int Similaridade) => PegarCidade(NomeDaCidadeOuIBGE, null, Similaridade);
        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGE. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE, string NomeOuUFOuIBGE) => PegarCidade(NomeDaCidadeOuIBGE, NomeOuUFOuIBGE, 100);

        /// <summary>
        /// Retorna uma cidade a partir de seu nome ou codigo IBGE.A busca por nome da cidade é feita a partir da similaridade entre o nome fornecido e o nome cadastrado no banco de dados. Caso a cidade não seja encontrada mas o estado seja identificado, a capital desse estado é retornada no lugar
        /// </summary>
        /// <param name="NomeOuUFOuIBGE">Nome do Estado ou Sigla do Estado ou código IBGE do Estado ou Cidade</param>
        /// <param name="NomeDaCidadeOuIBGE">Nome da cidade ou código IBGE da cidade</param>
        /// <param name="Similaridade">Porcentagem de similaridade entre o nome da cidade fornecido (<paramref name="NomeDaCidadeOuIBGE"/>) e o nome da cidade dentro do banco de dados (<see cref="Cidade.Nome"/>)</param>
        /// <returns>as informações da cidade encontrada</returns>
        public static Cidade PegarCidade(string NomeDaCidadeOuIBGE, string NomeOuUFOuIBGE, int Similaridade)
        {
            NomeOuUFOuIBGE = NomeOuUFOuIBGE.IfBlank(NomeDaCidadeOuIBGE);
            List<Cidade> cids = Cidades.ToList();
            var est = (PegarEstado(NomeOuUFOuIBGE) ?? PegarEstado(NomeDaCidadeOuIBGE));
            cids = est?.Cidades.ToList() ?? cids;

            cids = cids
                .Where(x => (NomeDaCidadeOuIBGE.SimilarityCaseInsensitive(x.Nome) >= (Similaridade / 100)) || x.IBGE == NomeDaCidadeOuIBGE.RemoveMaskInt() || x.Nome.ToUpperInvariant().Split(Util.WhitespaceChar).SelectJoinString(c => c.GetFirstChars()) == NomeDaCidadeOuIBGE.ToUpperInvariant() || x.Nome.ToUpperInvariant().Split(Util.WhitespaceChar).RemoveAny("DO", "DOS", "DE").SelectJoinString(c => c.GetFirstChars()) == NomeDaCidadeOuIBGE.ToUpperInvariant()).ToList();

            return cids.FirstOrDefault(x => x.IBGE == NomeDaCidadeOuIBGE.RemoveMaskInt()) ?? cids.OrderByDescending(x => x.Nome.SimilarityCaseInsensitive(NomeDaCidadeOuIBGE)).ThenByDescending(x => x.Capital).FirstOrDefault() ?? est?.Capital;
        }
        public static Cidade PegarCidade(int IBGE) => PegarEstado(IBGE.ToString())?.Cidades.FirstOrDefault(x => x.IBGE == IBGE);

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
        /// <param name="Text">CPF, CNPJ, CEP, PIS, Email, Celular ou Telefone</param>
        /// <param name="DefaultString"></param>
        /// <returns></returns>
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

                case "Telefone":
                case "Celular":
                    Text = Text.FormatarTelefone();
                    break;

                default:
                    break;
            }

            if (x.IsValid())
            {
                Text = $"{x}: {Text}";
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
            if (Input.IsEmail()) return "Email";
            if (Input.IsIP()) return "IP";
            if (Input.TelefoneValido()) return Input.RemoveMask().Length.IsAny(8, 10) ? "Telefone" : "Celular";

            return DefaultLabel;
        }

        /// <summary>
        /// Retorna o código IBGE do estado
        /// </summary>
        /// <param name="NomeOuUFouIBGE"></param>
        /// <returns></returns>
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
        /// Retorna a as informações do estado a partir de um nome de estado, sigla ou numero do IBGE do estado ou cidade
        /// </summary>
        /// <param name="NomeOuUFOuIBGE">Nome ou UF</param>
        /// <returns></returns>
        public static Estado PegarEstado(string NomeOuUFOuIBGE)
        {
            NomeOuUFOuIBGE = NomeOuUFOuIBGE.TrimBetween().ToSlugCase();
            return Estados.FirstOrDefault(x => (x.Nome.ToSlugCase() ?? Util.EmptyString) == (NomeOuUFOuIBGE ?? Util.EmptyString) || (x.UF.ToSlugCase() ?? Util.EmptyString) == (NomeOuUFOuIBGE ?? Util.EmptyString) || (x.IBGE.ToString()) == (NomeOuUFOuIBGE.GetFirstChars(2) ?? Util.EmptyString));
        }

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
        /// Valida se a string é um telefone
        /// </summary>
        /// <param name="Telefone"></param>
        /// <returns></returns>
        public static bool TelefoneValido(this string Telefone) => Telefone.IsValid() && new Regex(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).IsMatch(Telefone.RemoveAny("(", ")"));

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
        public static bool CNHValido(this string CNH)
        {
            // char firstChar = cnh[0];
            if (CNH.IsValid() && CNH.Length == 11 && CNH != new string('1', 11))
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

        public static void Reload() => l = new List<Estado>();
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
        public string FormaEmissaoFixo { get => FormaEmissao.FixedLenght(TamanhoFormaEmissao); set => FormaEmissao = value.RemoveMaskInt(); }

        public long CNPJ { get; set; }

        [IgnoreDataMember]
        public string CNPJFixo
        {
            get => CNPJ.FixedLenght(TamanhoCNPJ);
            set => CNPJ = value.RemoveMask().ToLong();
        }

        [IgnoreDataMember]
        public string CNPJFormatado { get => CNPJFixo.FormatarCNPJ(); set => CNPJ = value.RemoveMask().IfBlank(0L); }

        public int Codigo { get; set; }

        [IgnoreDataMember]
        public string CodigoFixo
        {
            get => Codigo.FixedLenght(TamanhoCodigo);
            set => Codigo = value.RemoveMaskInt();
        }

        public int? Digito { get; set; }
        [IgnoreDataMember]
        public string DigitoFixo
        {
            get => Digito?.FixedLenght(TamanhoDigito);
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
            get => $"{Ano.FixedLenght(TamanhoMesAno - 2)}{Mes.FixedLenght(TamanhoMesAno - 2)}";
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
            get => Modelo.IfBlank(55).FixedLenght(TamanhoModelo);
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
            get => Nota.FixedLenght(TamanhoNota);
            set => Nota = value.RemoveMaskInt();
        }

        public int Serie { get; set; }

        [IgnoreDataMember]
        public string SerieFixo
        {
            get => Serie.FixedLenght(TamanhoSerie);
            set => Serie = value.RemoveMaskInt();
        }

        [IgnoreDataMember]
        public Estado Estado { get => Brasil.PegarEstado($"{UF}"); set => UF = value?.IBGE ?? 0; }

        public int UF { get; set; }

        [IgnoreDataMember]
        public string UFFixo
        {
            get => UF.FixedLenght(TamanhoUF);
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

    public partial class Cidade
    {
        public Cidade(string Name, int IBGE, int DDD, string SIAFI, string TimeZone, decimal Latitude, decimal Longitude, bool Capital) : base()
        {
            this.Nome = Name;
            this.IBGE = IBGE;
            this.DDD = DDD;
            this.SIAFI = SIAFI;
            this.TimeZone = TimeZone;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Capital = Capital;
        }

        public bool Capital { get; }
        public int DDD { get; }

        [DataMember(Name = "ibge_id")]
        public int IBGE { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }
        public string Nome { get; }

        [DataMember(Name = "siafi_id")]
        public string SIAFI { get; internal set; }
        public Estado Estado => Brasil.PegarEstado($"{IBGE}");
        public string TimeZone { get; }

        public override string ToString() => Nome;
    }

    /// <summary>
    /// Objeto que representa um estado do Brasil e seus respectivos detalhes
    /// </summary>
    public partial class Estado
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
            if (NomeOuUF.IsValid())
            {
                Nome = Brasil.PegarNomeEstado(NomeOuUF);
                UF = Brasil.PegarUfEstado(NomeOuUF);
                Cidades = Brasil.PegarCidades(NomeOuUF);
                Regiao = Brasil.PegarRegiao(NomeOuUF);
            }
        }

        /// <summary>
        /// Lista de cidades do estado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cidade> Cidades { get; internal set; }

        public Cidade Capital => Cidades?.FirstOrDefault(x => x.Capital);

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