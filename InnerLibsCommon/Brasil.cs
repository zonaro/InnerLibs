using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
namespace InnerLibs.BR
{

    /// <summary>
    /// Objeto para manipular cidades e estados do Brasil
    /// </summary>
    public static class Brasil
    {
        #region Private Fields

        private static List<State> l = new List<State>();

        #endregion Private Fields

        #region Public Properties

        public static IEnumerable<City> Cities => States.SelectMany(x => x.Cities).ToArray();

        /// <summary>
        /// Retorna as Regiões dos estados brasileiros
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> Regions => States.Select(x => x.Region).Distinct();

        /// <summary>
        /// Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<State> States
        {
            get
            {
                l = l ?? new List<State>();
                if (!l.Any())
                {
                    string s = Assembly.GetExecutingAssembly().GetResourceFileText("InnerLibs.brasil.xml");
                    var doc = new XmlDocument();
                    doc.LoadXml(s);

                    foreach (XmlNode estado in doc.SelectNodes("brasil/state"))
                    {
                        var e = new State(estado["StateCode"].InnerText, estado["Name"].InnerText, estado["Region"].InnerText, estado["IBGE"].InnerText.ToInt(), "Brasil", "BR", estado["Longitude"].InnerText.ToDecimal(), estado["Latitude"].InnerText.ToDecimal());
                        var cities = new List<City>();

                        foreach (XmlNode node in doc.SelectNodes($"brasil/city[StateCode = '{e.StateCode}']"))
                        {
                            cities.Add(new City(
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

                        e.Cities = cities;
                        l.Add(e);
                    }
                }
                return l;
            }
        }

        #endregion Public Properties

        #region Public Methods


        /// <summary>
        /// Procura numeros de telefone em um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindTelephoneNumbers(this string Text) => Text.FindByRegex(@"\b[\s()\d-]{6,}\d\b", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Select(x => x.FormatTelephoneNumber());


        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> da cidade e estado correspondentes
        /// </summary>
        /// <param name="NameOrStateCode"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        public static AddressInfo CreateAddressInfo(string NameOrStateCode, string City) => CreateAddressInfo<AddressInfo>(NameOrStateCode, City);

        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> da cidade e estado correspondentes
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        public static T CreateAddressInfo<T>(string NameOrStateCodeOrIBGE, string City) where T : AddressInfo
        {
            if (NameOrStateCodeOrIBGE.IsBlank() && City.IsNotBlank())
            {
                NameOrStateCodeOrIBGE = FindStateByCityName(City).FirstOrDefault().IfBlank(NameOrStateCodeOrIBGE);
            }

            var s = GetState(NameOrStateCodeOrIBGE);
            if (s != null)
            {
                var c = GetClosestCity(s.StateCode, City);
                var ends = Activator.CreateInstance<T>();
                ends.City = c?.Name ?? City;
                ends.State = s.Name;
                ends.StateCode = s.StateCode;
                ends.Region = s.Region;
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

        public static City FindCityByIBGE(int IBGE) => Cities.FirstOrDefault(x => x.IBGE == IBGE);

        /// <summary>
        /// Retorna o estado de uma cidade especifa. Pode trazer mais de um estado caso o nome da
        /// cidade seja igual em 2 ou mais estados
        /// </summary>
        /// <param name="CityName"></param>
        /// <returns></returns>
        public static IEnumerable<State> FindStateByCityName(string CityName) => States.Where((Func<State, bool>)(x => x.Cities.Any((Func<City, bool>)(c => (Ext.ToSlugCase(c.Name) ?? Ext.EmptyString) == (Ext.ToSlugCase(CityName) ?? Ext.EmptyString) || (c.IBGE.ToString() ?? Ext.EmptyString) == (Ext.ToSlugCase(CityName) ?? Ext.EmptyString)))));

        public static State FindStateByIBGE(int IBGE) => States.FirstOrDefault(x => x.IBGE == IBGE) ?? FindCityByIBGE(IBGE)?.State;

        /// <inheritdoc cref="FormatCEP(string)"/>
        public static string FormatCEP(this int CEP) => FormatCEP(CEP.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Formata um numero para CEP
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static string FormatCEP(this string CEP)
        {
            CEP = CEP.RemoveAny(".", "-").GetBefore(",") ?? Ext.EmptyString;
            CEP = CEP.PadLeft(8, '0');
            CEP = CEP.Insert(5, "-");
            if (CEP.IsValidCEP())
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
        public static string FormatCNPJ(this long CNPJ) => string.Format(CultureInfo.InvariantCulture, @"{0:00\.000\.000\/0000\-00}", CNPJ);

        /// <summary>
        /// Formata um numero para CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static string FormatCNPJ(this string CNPJ)
        {
            if (CNPJ.IsValidCNPJ())
            {
                if (CNPJ.IsNumber())
                {
                    CNPJ = CNPJ.ToLong().FormatCNPJ();
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
        public static string FormatCPF(this long CPF) => string.Format(CultureInfo.InvariantCulture, @"{0:000\.000\.000\-00}", CPF);

        /// <summary>
        /// Formata um numero para CPF
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public static string FormatCPF(this string CPF)
        {
            if (CPF.IsValidCPF())
            {
                if (CPF.IsNumber())
                {
                    CPF = CPF.ToLong().FormatCPF();
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
        public static string FormatCPFOrCNPJ(this long Document)
        {
            if (Document.ToString(CultureInfo.InvariantCulture).IsValidCPF())
            {
                return Document.FormatCPF();
            }
            else if (Document.ToString(CultureInfo.InvariantCulture).IsValidCNPJ())
            {
                return Document.FormatCNPJ();
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
        public static string FormatCPFOrCNPJ(this string Document)
        {
            if (Document.IsValidCPF())
            {
                return Document.FormatCPF();
            }
            else if (Document.IsValidCNPJ())
            {
                return Document.FormatCNPJ();
            }
            else
            {
                return Document;
            }
        }

        public static string FormatDocumentWithLabel(this string Input, string DefaultString = Ext.EmptyString)
        {
            var x = Input.GetDocumentLabel(DefaultString);
            switch (x)
            {
                case "CPF":
                case "CNPJ":
                    Input = Input.FormatCPFOrCNPJ();
                    break;

                case "CEP":
                    Input = Input.FormatCEP();
                    break;

                case "PIS":
                    Input = Input.FormatPIS();
                    break;

                case "Email":
                    Input = Input.ToLowerInvariant();
                    break;

                case "Tel":
                    Input = Input.FormatTelephoneNumber();
                    break;

                default:
                    break;
            }
            return $"{x}: {Input}";
        }

        /// <summary>
        /// Formata o PIS no padrão ###.#####.##-#
        /// </summary>
        /// <param name="PIS">PIS a ser formatado</param>
        /// <param name="returnOnlyNumbers">Se verdadeiro, retorna apenas os números sem formatação</param>
        /// <returns>PIS formatado</returns>
        public static string FormatPIS(this string PIS)
        {
            if (PIS.IsValidPIS())
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
        /// <param name="returnOnlyNumbers">Se verdadeiro, retorna apenas os números sem formatação</param>
        /// <returns>PIS formatado</returns>
        public static string FormatPIS(this long PIS) => FormatPIS(PIS.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string FormatTelephoneNumber(this string Number)
        {
            Number = Number ?? Ext.EmptyString;
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

        /// <inheritdoc cref="FormatTelephoneNumber(int)"/>
        public static string FormatTelephoneNumber(this long Number) => FormatTelephoneNumber($"{Number}");

        /// <inheritdoc cref="FormatTelephoneNumber(string)"/>
        public static string FormatTelephoneNumber(this int Number) => FormatTelephoneNumber($"{Number}");

        /// <inheritdoc cref="FormatTelephoneNumber(int)"/>
        public static string FormatTelephoneNumber(this decimal Number) => FormatTelephoneNumber($"{Number}");

        /// <inheritdoc cref="FormatTelephoneNumber(int)"/>
        public static string FormatTelephoneNumber(this double Number) => FormatTelephoneNumber($"{Number}");

        public static City GetCapital(string NameOrStateCodeOrIBGE) => (GetState(NameOrStateCodeOrIBGE)?.Cities ?? new List<City>()).FirstOrDefault(x => x.Capital);

        /// <summary>
        /// Retorna as cidades de um estado a partir do nome ou sigla do estado
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE">Nome ou sigla do estado</param>
        /// <returns></returns>
        public static IEnumerable<City> GetCitiesOf(string NameOrStateCodeOrIBGE) => (GetState(NameOrStateCodeOrIBGE)?.Cities ?? new List<City>()).AsEnumerable();

        public static City GetClosestCity(string NameOrStateCodeOrIBGE, string CityName) => (GetState(NameOrStateCodeOrIBGE)?.Cities ?? new List<City>()).AsEnumerable().OrderBy(x => x.Name.LevenshteinDistance(CityName)).Where(x => CityName.IsNotBlank()).FirstOrDefault();

        /// <summary>
        /// Retorna o nome da cidade mais parecido com o especificado em <paramref name="CityName"/>
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE">Nome ou sigla do estado</param>
        /// <param name="CityName">Nome da cidade</param>
        /// <returns></returns>
        public static string GetClosestCityName(string NameOrStateCodeOrIBGE, string CityName) => (GetClosestCity(NameOrStateCodeOrIBGE, CityName)?.Name ?? InnerLibs.Ext.EmptyString).IfBlank(CityName);

        public static string GetDocumentLabel(this string Input, string DefaultLabel = Ext.EmptyString)
        {
            if (Input.IsValidCPF()) return "CPF";
            if (Input.IsValidCNPJ()) return "CNPJ";
            if (Input.IsValidCEP()) return "CEP";
            if (Input.IsValidEAN()) return "EAN";
            if (Input.IsValidPIS()) return "PIS";
            if (Input.IsValidCNH()) return "CNH";
            if (Input.IsEmail()) return "Email";
            if (Input.IsTelephone()) return "Tel";
            if (Input.IsIP()) return "IP";
            return DefaultLabel;
        }

        public static int? GetIBGEOf(string NameOrStateCodeOrIBGE) => GetState(NameOrStateCodeOrIBGE)?.IBGE;

        /// <summary>
        /// Retorna o nome do estado a partir da sigla
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE"></param>
        /// <returns></returns>
        public static string GetNameOf(string NameOrStateCodeOrIBGE) => GetState(NameOrStateCodeOrIBGE)?.Name;

        /// <summary>
        /// Retorna a região a partir de um nome de estado
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE"></param>
        /// <returns></returns>
        public static string GetRegionOf(string NameOrStateCodeOrIBGE) => GetState(NameOrStateCodeOrIBGE)?.Region;

        /// <summary>
        /// Retorna a as informações do estado a partir de um nome de estado ou sua sigla
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE">Nome ou UF</param>
        /// <returns></returns>
        public static State GetState(string NameOrStateCodeOrIBGE)
        {
            NameOrStateCodeOrIBGE = NameOrStateCodeOrIBGE.TrimBetween().ToSlugCase();
            return States.FirstOrDefault(x => (x.Name.ToSlugCase() ?? InnerLibs.Ext.EmptyString) == (NameOrStateCodeOrIBGE ?? InnerLibs.Ext.EmptyString) || (x.StateCode.ToSlugCase() ?? InnerLibs.Ext.EmptyString) == (NameOrStateCodeOrIBGE ?? InnerLibs.Ext.EmptyString) || (x.IBGE.ToString()) == (NameOrStateCodeOrIBGE ?? InnerLibs.Ext.EmptyString));
        }

        /// <summary>
        /// Retorna a Sigla a partir de um nome de estado
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE"></param>
        /// <returns></returns>
        public static string GetStateCodeOf(string NameOrStateCodeOrIBGE) => GetState(NameOrStateCodeOrIBGE)?.StateCode;

        /// <summary>
        /// Retorna os estados de uma região
        /// </summary>
        /// <param name="Region"></param>
        /// <returns></returns>
        public static IEnumerable<State> GetStatesOf(string Region) => States.Where((Func<State, bool>)(x => (Ext.ToSlugCase(x.Region) ?? Ext.EmptyString) == (Ext.TrimBetween(Ext.ToSlugCase(Region)) ?? Ext.EmptyString) || Region.IsBlank()));

        /// <summary>
        /// Valida se a string é um telefone
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool IsTelephone(this string Text) => new Regex(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).IsMatch(Text.RemoveAny("(", ")"));

        /// <summary>
        /// Verifica se uma string é um cep válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool IsValidCEP(this string CEP) => new Regex(@"^\d{5}-\d{3}$").IsMatch(CEP) || (CEP.RemoveAny("-").IsNumber() && CEP.RemoveAny("-").Length == 8);

        /// <summary>
        /// Verifica se a string é um CNH válido
        /// </summary>
        /// <param name="Text">CNH</param>
        /// <returns></returns>
        public static bool IsValidCNH(this string CNH)
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
        /// <param name="Text">CPF</param>
        /// <returns></returns>
        public static bool IsValidCNPJ(this string Text)
        {
            try
            {
                if (Text.IsNotBlank())
                {
                    var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    int soma;
                    int resto;
                    string digito;
                    string tempCnpj;
                    Text = Text.Trim();
                    Text = Text.Replace(".", InnerLibs.Ext.EmptyString).Replace("-", InnerLibs.Ext.EmptyString).Replace("/", InnerLibs.Ext.EmptyString);
                    if (Text.Length != 14)
                    {
                        return false;
                    }

                    tempCnpj = Text.Substring(0, 12);
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
                    return Text.EndsWith(digito);
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
        /// <param name="Text">CPF</param>
        /// <returns></returns>
        public static bool IsValidCPF(this string Text)
        {
            try
            {
                if (Text.IsNotBlank())
                {
                    Text = Text.RemoveAny(".", "-");
                    string digito = InnerLibs.Ext.EmptyString;
                    int k;
                    int j;
                    int soma;
                    for (k = 0; k <= 1; k++)
                    {
                        soma = 0;
                        var loopTo = 9 + (k - 1);
                        for (j = 0; j <= loopTo; j++)
                        {
                            soma += int.Parse($"{Text[j]}", CultureInfo.InvariantCulture) * (10 + k - j);
                        }

                        digito += $"{(soma % 11 == 0 || soma % 11 == 1 ? 0 : 11 - (soma % 11))}";
                    }

                    return digito[0] == Text[9] & digito[1] == Text[10];
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
        /// <param name="Text">CPF ou CNPJ</param>
        /// <returns></returns>
        public static bool IsValidCPFOrCNPJ(this string Text) => Text.IsValidCPF() || Text.IsValidCNPJ();

        /// <summary>
        /// Verifica se uma string é um PIS válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool IsValidPIS(this string PIS)
        {
            if (PIS.IsBlank())
            {
                return false;
            }

            PIS = Regex.Replace(PIS, "[^0-9]", InnerLibs.Ext.EmptyString).ToString();

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

        public static void Reload() => l = new List<State>();

        #endregion Public Methods
    }

    public class City
    {
        #region Public Constructors

        public City(string Name, int IBGE, int DDD, State State, string SIAFI, string TimeZone, decimal Latitude, decimal Longitude, bool Capital) : base()
        {
            this.Name = Name;
            this.IBGE = IBGE;
            this.DDD = DDD;
            this.State = State;
            this.SIAFI = SIAFI;
            this.TimeZone = TimeZone;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Capital = Capital;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Capital { get; }
        public int DDD { get; }
        public int IBGE { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }
        public string Name { get; }
        public string SIAFI { get; }
        public State State { get; } = new State(null);
        public string TimeZone { get; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => Name;

        #endregion Public Methods
    }

    /// <summary>
    /// Objeto que representa um estado do Brasil e seus respectivos detalhes
    /// </summary>
    public class State
    {
        #region Public Constructors

        /// <summary>
        /// Sigla do estado
        /// </summary>
        /// <returns></returns>
        public State(string StateCode, string Name, string Region, int IBGE, string Country, string CountryCode, decimal Longitude, decimal Latitude)
        {
            this.StateCode = StateCode;
            this.Name = Name;
            this.Region = Region;
            this.IBGE = IBGE;
            this.Country = Country;
            this.CountryCode = CountryCode;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
        }

        /// <summary>
        /// Inicializa um objeto Estado a partir de uma sigla
        /// </summary>
        /// <param name="NameOrStateCode"></param>
        public State(string NameOrStateCode)
        {
            if (NameOrStateCode.IsNotBlank())
            {
                Name = Brasil.GetNameOf(NameOrStateCode);
                StateCode = Brasil.GetStateCodeOf(NameOrStateCode);
                Cities = Brasil.GetCitiesOf(NameOrStateCode);
                Region = Brasil.GetRegionOf(NameOrStateCode);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Lista de cidades do estado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<City> Cities { get; internal set; }

        public string Country { get; }

        public string CountryCode { get; }

        public int IBGE { get; }

        public decimal Latitude { get; }

        public decimal Longitude { get; }

        /// <summary>
        /// Nome do estado
        /// </summary>
        /// <returns></returns>
        public string Name { get; }

        public string Region { get; }
        public string StateCode { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Retorna a String correspondente ao estado
        /// </summary>
        /// <returns></returns>
        public override string ToString() => StateCode;

        #endregion Public Methods
    }
}