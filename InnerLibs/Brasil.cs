using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace InnerLibs.Locations
{
    /// <summary>
    /// Objeto para manipular cidades e estados do Brasil
    /// </summary>
    public sealed class Brasil
    {
        private static List<State> l = new List<State>();

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
        public static IEnumerable<State> FindStateByCityName(string CityName) => States.Where(x => x.Cities.Any(c => (c.Name.ToSlugCase() ?? "") == (CityName.ToSlugCase() ?? "") || (c.IBGE.ToString() ?? "") == (CityName.ToSlugCase() ?? "")));

        public static State FindStateByIBGE(int IBGE) => States.FirstOrDefault(x => x.IBGE == IBGE) ?? FindCityByIBGE(IBGE)?.State;

        /// <summary>
        /// Retorna as cidades de um estado a partir do nome ou sigla do estado
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE">Nome ou sigla do estado</param>
        /// <returns></returns>
        public static IEnumerable<City> GetCitiesOf(string NameOrStateCodeOrIBGE) => (GetState(NameOrStateCodeOrIBGE)?.Cities ?? new List<City>()).AsEnumerable();

        /// <summary>
        /// Retorna o nome da cidade mais parecido com o especificado em <paramref name="CityName"/>
        /// </summary>
        /// <param name="NameOrStateCodeOrIBGE">Nome ou sigla do estado</param>
        /// <param name="CityName">Nome da cidade</param>
        /// <returns></returns>
        public static string GetClosestCityName(string NameOrStateCodeOrIBGE, string CityName) => (GetClosestCity(NameOrStateCodeOrIBGE, CityName)?.Name ?? "").IfBlank(CityName);
        public static City GetClosestCity(string NameOrStateCodeOrIBGE, string CityName) => (GetState(NameOrStateCodeOrIBGE)?.Cities ?? new List<City>()).AsEnumerable().OrderBy(x => x.Name.LevenshteinDistance(CityName)).Where(x => CityName.IsNotBlank()).FirstOrDefault();
        public static City GetCapital(string NameOrStateCodeOrIBGE) => (GetState(NameOrStateCodeOrIBGE)?.Cities ?? new List<City>()).FirstOrDefault(x => x.Capital);

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
            return States.FirstOrDefault(x => (x.Name.ToSlugCase() ?? "") == (NameOrStateCodeOrIBGE ?? "") || (x.StateCode.ToSlugCase() ?? "") == (NameOrStateCodeOrIBGE ?? "") || (x.IBGE.ToString()) == (NameOrStateCodeOrIBGE ?? ""));
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
        public static IEnumerable<State> GetStatesOf(string Region) => States.Where(x => (x.Region.ToSlugCase() ?? "") == (Region.ToSlugCase().TrimBetween() ?? "") || Region.IsBlank());

        public void Reload() => l = new List<State>();
    }

    public class City
    {


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

        public int DDD { get; }
        public int IBGE { get; }
        public string Name { get; }
        public State State { get; } = new State(null);

        public string SIAFI { get; }
        public string TimeZone { get; }

        public decimal Latitude { get; }
        public decimal Longitude { get; }
        public bool Capital { get; }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Objeto que representa um estado do Brasil e seus respectivos detalhes
    /// </summary>
    public class State
    {
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



        /// <summary>
        /// Lista de cidades do estado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<City> Cities { get; internal set; }

        public string Country { get; }

        public string CountryCode { get; }

        public int IBGE { get; }

        /// <summary>
        /// Nome do estado
        /// </summary>
        /// <returns></returns>
        public string Name { get; }

        public string Region { get; }
        public decimal Latitude { get; }
        public decimal Longitude { get; }
        public string StateCode { get; }

        /// <summary>
        /// Retorna a String correspondente ao estado
        /// </summary>
        /// <returns></returns>
        public override string ToString() => StateCode;
    }
}