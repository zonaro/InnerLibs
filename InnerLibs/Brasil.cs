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
        private static List<City> l = new List<City>();



        /// <summary>
        /// Retorna as Regiões dos estados brasileiros
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> Regions => States.Select(x => x.Region).Distinct();


        public static IEnumerable<State> States => Cities.GroupBy(x => new { x.StateCode, x.State, x.Region, x.Country, x.CountryCode }).Select(x => new State() { Country = x.Key.Country, CountryCode = x.Key.CountryCode, StateCode = x.Key.StateCode, State = x.Key.State, Cities = x.ToArray(), Region = x.Key.Region }).ToArray();


        /// <summary>
        /// Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<City> Cities
        {
            get
            {
                l = l ?? new List<City>();
                if (!l.Any())
                {
                    string s = Assembly.GetExecutingAssembly().GetResourceFileText("InnerLibs.brasil.xml");
                    var doc = new XmlDocument();
                    doc.LoadXml(s);
                    foreach (XmlNode node in doc["brasil"].ChildNodes)
                    {

                        var c = new City()
                        {
                            IBGE = node["IBGE"].InnerText,
                            City = node["Name"].InnerText,
                            StateCode = node["StateCode"].InnerText,
                            State = node["State"].InnerText,
                            Region = node["Region"].InnerText,
                            Country = "Brasil",
                            CountryCode = "BR"
                        };

                        l.Add(c);


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
        /// <param name="NameOrStateCode"></param>
        /// <param name="City"></param>
        /// <returns></returns>
        public static T CreateAddressInfo<T>(string NameOrStateCode, string City) where T : AddressInfo
        {
            if (NameOrStateCode.IsBlank() && City.IsNotBlank())
            {
                NameOrStateCode = FindStateByCity(City).FirstOrDefault().IfBlank(NameOrStateCode);
            }

            var s = GetState(NameOrStateCode);
            if (s != null)
            {
                City = GetClosestCity(s.StateCode, City);
                var ends = Activator.CreateInstance<T>();
                ends.City = City;
                ends.State = s.Name;
                ends.StateCode = s.StateCode;
                ends.Region = s.Region;
                ends.Country = "Brasil";
                ends.CountryCode = "BR";
                return ends;
            }

            return null;
        }

        /// <summary>
        /// Retorna o estado de uma cidade especifa. Pode trazer mais de um estado caso o nome da
        /// cidade seja igual em 2 ou mais estados
        /// </summary>
        /// <param name="CityName"></param>
        /// <returns></returns>
        public static IEnumerable<State> FindStateByCity(string CityName) => States.Where(x => x.Cities.Any(c => (c.Name.ToSlugCase() ?? "") == (CityName.ToSlugCase() ?? "")));



        /// <summary>
        /// Retorna as cidades de um estado a partir do nome ou sigla do estado
        /// </summary>
        /// <param name="NameOrStateCode">Nome ou sigla do estado</param>
        /// <returns></returns>
        public static IEnumerable<City> GetCitiesOf(string NameOrStateCode) => (GetState(NameOrStateCode)?.Cities ?? new List<City>()).AsEnumerable();

        /// <summary>
        /// Retorna o nome da cidade mais parecido com o especificado em <paramref name="CityName"/>
        /// </summary>
        /// <param name="NameOrStateCode">Nome ou sigla do estado</param>
        /// <param name="CityName">Nome da cidade</param>
        /// <returns></returns>
        public static string GetClosestCity(string NameOrStateCode, string CityName) => (GetState(NameOrStateCode)?.Cities ?? new List<City>()).AsEnumerable().OrderBy(x => x.Name.LevenshteinDistance(CityName)).Where(x => CityName.IsNotBlank()).FirstOrDefault().IfBlank(CityName);

        /// <summary>
        /// Retorna o nome do estado a partir da sigla
        /// </summary>
        /// <param name="NameOrStateCode"></param>
        /// <returns></returns>
        public static string GetNameOf(string NameOrStateCode) => GetState(NameOrStateCode)?.Name;

        /// <summary>
        /// Retorna a região a partir de um nome de estado
        /// </summary>
        /// <param name="NameOrStateCode"></param>
        /// <returns></returns>
        public static string GetRegionOf(string NameOrStateCode) => GetState(NameOrStateCode)?.Region;

        /// <summary>
        /// Retorna a as informações do estado a partir de um nome de estado ou sua sigla
        /// </summary>
        /// <param name="NameOrStateCode">Nome ou UF</param>
        /// <returns></returns>
        public static State GetState(string NameOrStateCode)
        {
            NameOrStateCode = NameOrStateCode.TrimBetween().ToSlugCase();
            return States.FirstOrDefault(x => (x.Name.ToSlugCase() ?? "") == (NameOrStateCode ?? "") || (x.StateCode.ToSlugCase() ?? "") == (NameOrStateCode ?? ""));
        }

        /// <summary>
        /// Retorna a Sigla a partir de um nome de estado
        /// </summary>
        /// <param name="NameOrStateCode"></param>
        /// <returns></returns>
        public static string GetStateCodeOf(string NameOrStateCode) => GetState(NameOrStateCode)?.StateCode;

        /// <summary>
        /// Retorna os estados de uma região
        /// </summary>
        /// <param name="Region"></param>
        /// <returns></returns>
        public static IEnumerable<State> GetStatesOf(string Region) => States.Where(x => (x.Region.ToSlugCase() ?? "") == (Region.ToSlugCase().TrimBetween() ?? "") || Region.IsBlank());

        public void Reload() => l = new List<City>();
    }



    /// <summary>
    /// Objeto que representa um estado do Brasil e seus respectivos detalhes
    /// </summary>
    public class State : AddressInfo
    {
        /// <summary>
        /// Sigla do estado
        /// </summary>
        /// <returns></returns>
        public State(string StateCode, string Name, string Region)
        {
            base.StateCode = StateCode;
            State = Name;
            base.Region = Region;
        }

        /// <summary>
        /// Inicializa um objeto Estado a partir de uma sigla
        /// </summary>
        /// <param name="NameOrStateCode"></param>
        public State(string NameOrStateCode)
        {
            if (NameOrStateCode.IsNotBlank())
            {
                State = Brasil.GetNameOf(NameOrStateCode);
                StateCode = Brasil.GetStateCodeOf(NameOrStateCode);
                Cities = Brasil.GetCitiesOf(NameOrStateCode);
                Region = Brasil.GetRegionOf(NameOrStateCode);
            }
        }

        public State() : this(null)
        {
        }

        /// <summary>
        /// Lista de cidades do estado
        /// </summary>
        /// <returns></returns>
        public IEnumerable<City> Cities { get; set; }

        /// <summary>
        /// Nome do estado
        /// </summary>
        /// <returns></returns>
        public override string Name { get => State; }

        /// <summary>
        /// Retorna a String correspondente ao estado
        /// </summary>
        /// <returns></returns>
        public override string ToString() => StateCode;
    }

    public class City : AddressInfo
    {
        public City() : base() { }

        public City(string Name, string IBGE) : base()
        {
            City = Name;
            this.IBGE = IBGE;
        }

        public override string Name { get => City; }
        public string IBGE { get => this[nameof(IBGE)]; set => this[nameof(IBGE)] = value; }
        public override string ToString() => City;

    }

}