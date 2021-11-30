using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;

namespace InnerLibs.Locations
{
    /// <summary>
    /// Representa um deteminado local com suas Informações
    /// </summary>
    /// <remarks></remarks>
    public class AddressInfo
    {
        /// <summary>
        /// Cria um novo objeto de localização
        /// </summary>
        public AddressInfo()
        {
        }

        /// <summary>
        /// Tipo do Endereço
        /// </summary>
        /// <returns></returns>
        public string Type
        {
            get
            {
                return AddressTypes.GetAddressType(Street);
            }
        }

        /// <summary>
        /// O nome do endereço
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return Street.TrimAny(AddressTypes.GetAddressTypeList(Street)).TrimAny(" ");
            }
        }

        /// <summary>
        /// Logradouro
        /// </summary>
        /// <returns></returns>
        public string Street
        {
            get
            {
                return this["street"];
            }

            set
            {
                if (value.IsNotBlank())
                {
                    this["street"] = $"{AddressTypes.GetAddressType(value)} {value.TrimAny(true, AddressTypes.GetAddressTypeList(value))}".AdjustBlankSpaces().ToLower().ToTitle().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
                }
                else
                {
                    this["street"] = null;
                }
            }
        }

        /// <summary>
        /// Numero da casa, predio etc.
        /// </summary>
        /// <value></value>
        /// <returns>Numero</returns>

        public string Number
        {
            get
            {
                return this["number"];
            }

            set
            {
                this["number"] = value;
            }
        }

        /// <summary>
        /// Complemento
        /// </summary>
        /// <value></value>
        /// <returns>Complemento</returns>

        public string Complement
        {
            get
            {
                return this["complement"];
            }

            set
            {
                this["complement"] = value.IfBlank("").TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// Bairro
        /// </summary>
        /// <value></value>
        /// <returns>Bairro</returns>

        public string Neighborhood
        {
            get
            {
                return this["Neighborhood"];
            }

            set
            {
                this["Neighborhood"] = value.IfBlank("").ToLower().ToTitle().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// CEP - Codigo de Endereçamento Postal
        /// </summary>
        /// <value></value>
        /// <returns>CEP</returns>

        public string PostalCode
        {
            get
            {
                return this["PostalCode"];
            }

            set
            {
                this["PostalCode"] = FormatPostalCode(value);
            }
        }

        /// <summary>
        /// Formata uma string de CEP
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static string FormatPostalCode(string CEP)
        {
            CEP = CEP.IfBlank("").Trim();
            if (CEP.IsValidCEP())
            {
                if (CEP.IsNumber())
                {
                    CEP = CEP.Insert(5, "-");
                }

                return CEP;
            }

            return null;
        }

        /// <summary>
        /// CEP - Codigo de Endereçamento Postal. Alias de <see cref="PostalCode"/>
        /// </summary>
        /// <value></value>
        /// <returns>CEP</returns>
        public string ZipCode
        {
            get
            {
                return PostalCode;
            }

            set
            {
                PostalCode = value.IfBlank("").TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// Cidade
        /// </summary>
        /// <value></value>
        /// <returns>Cidade</returns>

        public string City
        {
            get
            {
                return this["city"];
            }

            set
            {
                this["city"] = value.IfBlank("").ToLower().ToTitle().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// Estado
        /// </summary>
        /// <value></value>
        /// <returns>Estado</returns>

        public string State
        {
            get
            {
                return this["state"];
            }

            set
            {
                this["state"] = value.IfBlank("").ToLower().ToTitle().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// Unidade federativa
        /// </summary>
        /// <value></value>
        /// <returns>Sigla do estado</returns>

        public string StateCode
        {
            get
            {
                return this["statecode"];
            }

            set
            {
                this["statecode"] = value.IfBlank("").ToUpper().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// País
        /// </summary>
        /// <value></value>
        /// <returns>País</returns>

        public string Region
        {
            get
            {
                return this["region"];
            }

            set
            {
                this["region"] = value.IfBlank("").ToLower().ToTitle().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// País
        /// </summary>
        /// <value></value>
        /// <returns>País</returns>

        public string Country
        {
            get
            {
                return this["country"];
            }

            set
            {
                this["country"] = value.IfBlank("").ToLower().ToTitle().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// País
        /// </summary>
        /// <value></value>
        /// <returns>País</returns>

        public string CountryCode
        {
            get
            {
                return this["countrycode"];
            }

            set
            {
                this["countrycode"] = value.IfBlank("").ToUpper().TrimAny(true, " ", ".", " ", ",", " ", "-", " ").NullIf(x => x.IsBlank());
            }
        }

        /// <summary>
        /// Coordenada geográfica LATITUDE
        /// </summary>
        /// <value></value>
        /// <returns>Latitude</returns>
        public decimal? Latitude
        {
            get
            {
                string value = this["Latitude"];
                if (value != null)
                {
                    return Convert.ToDecimal(value, new CultureInfo("en-US"));
                }

                return default;
            }

            set
            {
                if (value.HasValue)
                {
                    this["Latitude"] = Convert.ToString(value.Value, new CultureInfo("en-US"));
                }
            }
        }

        /// <summary>
        /// Coordenada geográfica LONGITUDE
        /// </summary>
        /// <value></value>
        /// <returns>Longitude</returns>
        public decimal? Longitude
        {
            get
            {
                string value = this["Longitude"];
                if (value != null)
                {
                    return Convert.ToDecimal(value, new CultureInfo("en-US"));
                }

                return default;
            }

            set
            {
                if (value.HasValue)
                {
                    this["Longitude"] = Convert.ToString(value, new CultureInfo("en-US"));
                }
            }
        }

        public Point GetPoint()
        {
            if (Latitude.HasValue && Longitude.HasValue)
            {
                return new Point((Longitude * 1000000).ToInteger(), (Latitude * 1000000).ToInteger());
            }

            return new Point();
        }

        public AddressInfo SetLatitudeLongitudeFromPoint(Point Point)
        {
            Longitude = (decimal?)((double)Point.X.ToDecimal() * 0.000001d);
            Latitude = (decimal?)((double)Point.Y.ToDecimal() * 0.000001d);
            return this;
        }

        public static implicit operator Point(AddressInfo AddressInfo)
        {
            return (Point)AddressInfo?.GetPoint();
        }

        public static implicit operator AddressInfo(Point Point)
        {
            return new AddressInfo().SetLatitudeLongitudeFromPoint(Point);
        }

        /// <summary>
        /// Retorna o endereço completo
        /// </summary>
        /// <returns>Uma String com o endereço completo devidamente formatado</returns>
        public string FullAddress
        {
            get
            {
                return ToString(AddressPart.FullAddress);
            }
        }

        /// <summary>
        /// Retorna o Logradouro e numero
        /// </summary>
        /// <returns>Uma String com o endereço completo devidamente formatado</returns>
        public string LocationInfo
        {
            get
            {
                return ToString(AddressPart.LocationInfo);
            }
        }

        /// <summary>
        /// Logradouro, Numero e Complemento
        /// </summary>
        /// <returns>Uma String com o endereço completo devidamente formatado</returns>
        public string FullLocationInfo
        {
            get
            {
                return ToString(AddressPart.FullLocationInfo);
            }
        }

        /// <summary>
        /// Retorna uma String contendo as informações do Local
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return ToString(Format);
        }

        /// <summary>
        /// Retorna uma string com as partes dos endereço especificas
        /// </summary>
        /// <param name="Parts"></param>
        /// <returns></returns>
        public string ToString(IEnumerable<AddressPart> Parts)
        {
            Parts = Parts ?? Array.Empty<AddressPart>();
            if (Parts.Any())
            {
                var d = Parts.First();
                foreach (var p in Parts.Skip(1))
                    d = d | p;
                return ToString(d);
            }

            return ToString();
        }
         

        /// <summary>
        /// Retorna uma string com as partes dos endereço especificas
        /// </summary>
        /// <param name="Parts"></param>
        /// <returns></returns>
        public string ToString(AddressPart Parts)
        {
            string retorno = "";
            if ((int)Parts <= 0)
            {
                Parts = Format;
            }

            retorno = retorno.AppendIf(Type, Type.IsNotBlank() && ContainsPart(Parts, AddressPart.StreetType));
            retorno = retorno.AppendIf($" {Name}", Name.IsNotBlank() && ContainsPart(Parts, AddressPart.StreetName));
            retorno = retorno.AppendIf($", {Number}", Number.IsNotBlank() && ContainsPart(Parts, AddressPart.Number));
            retorno = retorno.AppendIf($", {Complement}", Complement.IsNotBlank() && ContainsPart(Parts, AddressPart.Complement));
            retorno = retorno.AppendIf($" - {Neighborhood}", Neighborhood.IsNotBlank() && ContainsPart(Parts, AddressPart.Neighborhood));
            retorno = retorno.AppendIf($" - {City}", City.IsNotBlank() && ContainsPart(Parts, AddressPart.City));
            if (ContainsPart(Parts, AddressPart.State) && State.IsNotBlank())
            {
                retorno = retorno.AppendIf($" - {State}", State.IsNotBlank() && ContainsPart(Parts, AddressPart.State));
            }
            else
            {
                retorno = retorno.AppendIf($" - {StateCode}", StateCode.IsNotBlank() && ContainsPart(Parts, AddressPart.StateCode));
            }

            retorno = retorno.AppendIf($" - {PostalCode}", PostalCode.IsNotBlank() && ContainsPart(Parts, AddressPart.PostalCode));
            if (ContainsPart(Parts, AddressPart.Country) && Country.IsNotBlank())
            {
                retorno = retorno.AppendIf($" - {Country}", State.IsNotBlank() && ContainsPart(Parts, AddressPart.Country));
            }
            else
            {
                retorno = retorno.AppendIf($" - {CountryCode}", CountryCode.IsNotBlank() && ContainsPart(Parts, AddressPart.CountryCode));
            }

            return retorno.AdjustBlankSpaces().TrimAny(true, ".", " ", ",", " ", "-");
        }

        internal static bool ContainsPart(AddressPart Parts, AddressPart OtherPart)
        {
            return Parts.HasFlag(OtherPart);
            // Return ((Parts) And OtherPart) <> 0
        }

        /// <summary>
        /// Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        /// </summary>
        /// <param name="PostalCode"></param>
        /// <param name="Number">Numero da casa</param>
        public static AddressInfo FromViaCEP(long PostalCode, string Number = null, string Complement = null)
        {
            return FromViaCEP<AddressInfo>(PostalCode, Number, Complement);
        }

        /// <summary>
        /// Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        /// </summary>
        /// <param name="PostalCode"></param>
        /// <param name="Number">Numero da casa</param>
        public static AddressInfo FromViaCEP(string PostalCode, string Number = null, string Complement = null)
        {
            return FromViaCEP<AddressInfo>(PostalCode, Number, Complement);
        }

        /// <summary>
        /// Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        /// </summary>
        /// <param name="PostalCode"></param>
        /// <param name="Number">Numero da casa</param>
        public static T FromViaCEP<T>(long PostalCode, string Number = null, string Complement = null) where T : AddressInfo
        {
            return FromViaCEP<T>(PostalCode.ToString().PadLeft(8, '0'), Number, Complement);
        }

        /// <summary>
        /// Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        /// </summary>
        /// <param name="PostalCode"></param>
        /// <param name="Number">Numero da casa</param>
        public static T FromViaCEP<T>(string PostalCode, string Number = null, string Complement = null) where T : AddressInfo
        {
            var d = Activator.CreateInstance<T>();
            d["original_string"] = PostalCode;
            d.PostalCode = PostalCode;
            if (Number.IsNotBlank())
                d.Number = Number;
            if (Complement.IsNotBlank())
                d.Complement = Complement;
            try
            {
                string url = "https://viacep.com.br/ws/" + d.PostalCode.RemoveAny("-") + "/xml/";
                d["search_url"] = url;
                using (var c = new WebClient())
                {
                    var x = new XmlDocument();
                    x.LoadXml(c.DownloadString(url));
                    var cep = x["xmlcep"];
                    d.Neighborhood = cep["bairro"]?.InnerText;
                    d.City = cep["localidade"]?.InnerText;
                    d.StateCode = cep["uf"]?.InnerText;
                    d.State = Brasil.GetNameOf(d.StateCode);
                    d.Region = Brasil.GetRegionOf(d.StateCode);
                    d.Street = cep["logradouro"]?.InnerText;
                    try
                    {
                        d["DDD"] = cep["ddd"]?.InnerText;
                    }
                    catch  
                    {
                    }

                    try
                    {
                        d["IBGE"] = cep["ibge"]?.InnerText;
                    }
                    catch  
                    {
                    }

                    try
                    {
                        d["GIA"] = cep["gia"]?.InnerText;
                    }
                    catch  
                    {
                    }

                    try
                    {
                        d["SIAFI"] = cep["SIAFI"]?.InnerText;
                    }
                    catch  
                    {
                    }

                    d.Country = "Brasil";
                    d.CountryCode = "BR";
                }
            }
            catch  
            {
            }

            return d;
        }

        /// <summary>
        /// Tenta extrair as partes de um endereço de uma string
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public static AddressInfo TryParse(string Address)
        {
            return TryParse<AddressInfo>(Address);
        }

        /// <summary>
        /// Tenta extrair as partes de um endereço de uma string
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public static T TryParse<T>(string Address) where T : AddressInfo
        {
            string original = Address;
            string PostalCode = "";
            string State = "";
            string City = "";
            string Neighborhood = "";
            string Complement = "";
            string Number = "";
            string Country = "";
            Address = Address.AdjustBlankSpaces(); // arruma os espacos do endereco
            var ceps = Address.FindCEP(); // procura ceps no endereco
            Address = Address.RemoveAny(ceps); // remove ceps
            Address = Address.AdjustBlankSpaces(); // arruma os espacos do endereco
            if (ceps.Any())
            {
                PostalCode = FormatPostalCode(ceps.First());
            }

            Address = Address.AdjustBlankSpaces().TrimAny("-", ",", "/"); // arruma os espacos do endereco
            if (Address.Contains(" - "))
            {
                var parts = Address.Split(" - ").ToList();
                if (parts.Count == 1)
                {
                    Address = parts.First().IfBlank("").TrimAny(" ", ",", "-");
                }

                if (parts.Count == 2)
                {
                    Address = parts.First().IfBlank("");
                    string maybe_neigh = parts.Last().IfBlank("").TrimAny(" ", ",", "-");
                    if (maybe_neigh.Length == 2)
                    {
                        State = maybe_neigh;
                    }
                    else
                    {
                        Neighborhood = maybe_neigh;
                    }
                }

                if (parts.Count == 3)
                {
                    Address = parts.First().IfBlank("");
                    string maybe_city = parts.Last().IfBlank("").TrimAny(" ", ",", "-");
                    if (maybe_city.Length == 2)
                    {
                        State = maybe_city;
                    }
                    else
                    {
                        City = maybe_city;
                    }

                    parts.RemoveLast();
                    parts = parts.Skip(1).ToList();
                    Neighborhood = parts.FirstOrDefault().IfBlank("").TrimAny(" ", ",", "-");
                }

                if (parts.Count == 6)
                {
                    string ad = parts[0] + ", " + parts[1] + " " + parts[2];
                    parts.RemoveAt(1);
                    parts.RemoveAt(2);
                    parts[0] = ad;
                }

                if (parts.Count == 5)
                {
                    string ad = parts[0] + ", " + parts[1];
                    parts.RemoveAt(1);
                    parts[0] = ad;
                }

                if (parts.Count == 4)
                {
                    Address = parts.First().IfBlank("");
                    string maybe_state = parts.Last().IfBlank("").TrimAny(" ", ",", "-");
                    parts.RemoveLast();
                    string maybe_city = parts.Last().IfBlank("").TrimAny(" ", ",", "-");
                    parts.RemoveLast();
                    City = maybe_city;
                    State = maybe_state;
                    parts = parts.Skip(1).ToList();
                    Neighborhood = parts.FirstOrDefault().IfBlank("").TrimAny(" ", ",", "-");
                }
            }

            Address = Address.AdjustBlankSpaces();
            if (Address.Contains(","))
            {
                var parts = Address.GetAfter(",").SplitAny(" ", ".", ",").ToList();
                Number = parts.FirstOrDefault(x => x == "s/n" || x == "sn" || x.IsNumber());
                parts.Remove(Number);
                Complement = parts.JoinString(" ");
                Address = Address.GetBefore(",");
            }
            else
            {
                var adparts = Address.SplitAny(" ", "-").ToList();
                if (adparts.Any())
                {
                    string maybe_number = adparts.FirstOrDefault(x => x == "s/n" || x == "sn" || x.IsNumber()).IfBlank("").TrimAny(" ", ",");
                    Complement = adparts.JoinString(" ").GetAfter(maybe_number).TrimAny(" ", ",");
                    Number = maybe_number;
                    Address = adparts.JoinString(" ").GetBefore(maybe_number).TrimAny(" ", ",");
                }
            }

            Number = Number.AdjustBlankSpaces().TrimAny(" ", ",", "-");
            Complement = Complement.AdjustBlankSpaces().TrimAny(" ", ",", "-");
            var d = CreateLocation<T>(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode);
            d["original_string"] = original;
            return d;
        }

        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> usando a api de geocode do Google Maps para completar as informações
        /// </summary>
        /// <param name="Address">Endereço para Busca</param>
        /// <param name="Key">Chave de acesso da API</param>
        /// <returns></returns>
        public static AddressInfo FromGoogleMaps(string Address, string Key, NameValueCollection NVC = null)
        {
            return FromGoogleMaps<AddressInfo>(Address, Key, NVC);
        }

        /// <summary>
        /// Retorna um <see cref="AddressInfo"/> usando a api de geocode do Google Maps para completar as informações
        /// </summary>
        /// <param name="Address">Endereço para Busca</param>
        /// <param name="Key">Chave de acesso da API</param>
        /// <returns></returns>
        public static T FromGoogleMaps<T>(string Address, string Key, NameValueCollection NVC = null) where T : AddressInfo
        {
            var d = Activator.CreateInstance<T>();
            NVC = NVC ?? new NameValueCollection();
            NVC["key"] = Key;
            NVC["address"] = Address;
            d["original_string"] = Address;
            string url = $"https://maps.googleapis.com/maps/api/geocode/xml?{NVC.ToQueryString()}";
            d["search_url"] = url;
            using (var c = new WebClient())
            {
                var xml = new XmlDocument();
                xml.LoadXml(c.DownloadString(url));
                var x = xml["GeocodeResponse"];
                string status = x["status"].InnerText;
                d["status"] = status;
                if (status == "OK")
                {
                    foreach (XmlNode item in x["result"].ChildNodes)
                    {
                        if (item.Name == "geometry")
                        {
                            var cultura = new CultureInfo("en-US");
                            decimal lat = Convert.ToDecimal(item["location"]["lat"].InnerText, cultura);
                            decimal lng = Convert.ToDecimal(item["location"]["lng"].InnerText, cultura);
                            d.Latitude = lat;
                            d.Longitude = lng;
                        }

                        if (item.Name == "place_id")
                        {
                            d["place_id"] = item.InnerText;
                        }

                        if (item.Name == "address_component")
                        {
                            foreach (XmlNode subitem in item.ChildNodes)
                            {
                                if (subitem.Name == "type")
                                {
                                    d[subitem.InnerText] = item["long_name"].InnerText;
                                    switch (subitem.InnerText ?? "")
                                    {
                                        case "postal_code":
                                            {
                                                d.PostalCode = item["long_name"].InnerText;
                                                break;
                                            }

                                        case "country":
                                            {
                                                d.Country = item["long_name"].InnerText;
                                                d.CountryCode = item["short_name"].InnerText;
                                                break;
                                            }

                                        case "administrative_area_level_1":
                                            {
                                                d.State = item["long_name"].InnerText;
                                                d.StateCode = item["short_name"].InnerText;
                                                d.Region = Brasil.GetRegionOf(d.StateCode);
                                                break;
                                            }

                                        case "administrative_area_level_2":
                                            {
                                                d.City = item["long_name"].InnerText;
                                                break;
                                            }

                                        case "administrative_area_level_3":
                                        case "locality":
                                        case "sublocality":
                                            {
                                                d.Neighborhood = d.Neighborhood.IfBlank(item["long_name"].InnerText);
                                                break;
                                            }

                                        case "route":
                                            {
                                                d.Street = item["long_name"].InnerText;
                                                break;
                                            }

                                        case "street_number":
                                            {
                                                d.Number = item["long_name"].InnerText;
                                                break;
                                            }

                                        default:
                                            {
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (x["error_message"] != null)
                {
                    d["error_message"] = x["error_message"].InnerText;
                }
            }

            return d;
        }

        /// <summary>
        /// Cria uma localização a partir de partes de endereço
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Number"></param>
        /// <param name="Complement"></param>
        /// <param name="Neighborhood"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Country"></param>
        /// <param name="PostalCode"></param>
        /// <returns></returns>
        public static AddressInfo CreateLocation(string Address, string Number = "", string Complement = "", string Neighborhood = "", string City = "", string State = "", string Country = "", string PostalCode = "")
        {
            return CreateLocation<AddressInfo>(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode);
        }

        /// <summary>
        /// Cria uma localização a partir de partes de endereço
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Number"></param>
        /// <param name="Complement"></param>
        /// <param name="Neighborhood"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Country"></param>
        /// <param name="PostalCode"></param>
        /// <returns></returns>
        public static T CreateLocation<T>(string Address, string Number = "", string Complement = "", string Neighborhood = "", string City = "", string State = "", string Country = "", string PostalCode = "") where T : AddressInfo
        {
            var l = Activator.CreateInstance<T>();
            l.Street = Address;
            l.Neighborhood = Neighborhood;
            l.Complement = Complement;
            l.Number = Number;
            l.City = City;
            l.PostalCode = PostalCode;
            var st = Brasil.GetState(State);
            if (st != null)
            {
                l.State = st.Name;
                l.StateCode = st.StateCode;
                l.Region = st.Region;
                l.Country = "Brasil";
                l.CountryCode = "BR";
            }
            else
            {
                if ((State?.Length) == 2 == true)
                {
                    l.StateCode = State;
                }
                else
                {
                    l.State = State;
                }

                if ((Country?.Length) == 2 == true)
                {
                    l.CountryCode = Country;
                }
                else
                {
                    l.Country = Country;
                }
            }

            return l;
        }

        /// <summary>
        /// Retorna uma string de endereço a partir de varias partes de endereco
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Number"></param>
        /// <param name="Complement"></param>
        /// <param name="Neighborhood"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Country"></param>
        /// <param name="PostalCode"></param>
        /// <returns></returns>
        public static string FormatAddress(string Address, string Number = "", string Complement = "", string Neighborhood = "", string City = "", string State = "", string Country = "", string PostalCode = "")
        {
            return CreateLocation(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode).FullAddress;
        }

        /// <summary>
        /// Retorna as coordenadas geográficas do Local
        /// </summary>
        /// <returns>Uma String contendo LATITUDE e LONGITUDE separados por virgula</returns>

        public string LatitudeLongitude()
        {
            if (Latitude.HasValue && Longitude.HasValue)
            {
                return $"{Latitude?.ToString(new CultureInfo("en-US"))}, {Longitude?.ToString(new CultureInfo("en-US"))}";
            }

            return null;
        }

        public bool ContainsKey(string key)
        {
            return details.ContainsKey(key.ToLower());
        }

        public void Add(string key, string value)
        {
            this[key] = value;
        }

        public bool Remove(string key)
        {
            if (ContainsKey(key))
            {
                details.Remove(key);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            details.Clear();
        }

        /// <summary>
        /// Formato desta instancia de <see cref="AddressInfo"/> quando chamada pelo <see cref="AddressInfo.ToString()"/>
        /// </summary>
        /// <returns></returns>
        public AddressPart Format
        {
            get
            {
                if (_format == AddressPart.Default)
                {
                    return GlobalFormat;
                }

                return _format;
            }

            set
            {
                _format = value;
            }
        }

        private AddressPart _format = AddressPart.Default;
        private static AddressPart _globalformat = AddressPart.FullAddress;

        /// <summary>
        /// Formato global de todas as intancias de <see cref="AddressInfo"/> quando chamadas pelo <see cref="AddressInfo.ToString()"/>
        /// </summary>
        /// <returns></returns>
        public static AddressPart GlobalFormat
        {
            get
            {
                if (_globalformat == AddressPart.Default)
                {
                    return AddressPart.FullAddress;
                }

                return _globalformat;
            }

            set
            {
                _globalformat = value;
            }
        }

        private Dictionary<string, string> details = new Dictionary<string, string>();

        /// <summary>
        /// Retona uma informação deste endereço
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>

        public string this[string key]
        {
            get
            {
                if (details is null)
                    details = new Dictionary<string, string>();
                key = key.ToLower();
                if (!details.ContainsKey(key))
                {
                    switch (key ?? "")
                    {
                        case "geolocation":
                            {
                                return LatitudeLongitude();
                            }

                        case "fulladdress":
                        case "tostring":
                        case "address":
                            {
                                return FullAddress;
                            }

                        case "streetname":
                        case "name":
                            {
                                return Name;
                            }

                        case "streettype":
                        case "type":
                            {
                                return Type;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }

                return details.GetValueOr(key, null);
            }

            set
            {
                if (value.IsNotBlank())
                {
                    details[key.ToLower()] = value.TrimAny(" ");
                }
                else if (ContainsKey(key))
                {
                    Remove(key);
                }
            }
        }

        public Dictionary<string, string> GetDetails()
        {
            return details;
        }
    }

    /// <summary>
    /// Partes de um Endereço
    /// </summary>
    [Flags]
    public enum AddressPart
    {
        /// <summary>
        /// Formato default definido pela propriedade <see cref="AddressInfo.Format"/> ou <see cref="AddressInfo.GlobalFormat"/>
        /// </summary>
        Default = 0,

        /// <summary>
        /// Tipo do Lograoduro
        /// </summary>
        StreetType = 1,

        /// <summary>
        /// Nome do Logradouro
        /// </summary>
        StreetName = 2,

        /// <summary>
        /// Logradouro
        /// </summary>
        Street = StreetType + StreetName,

        /// <summary>
        /// Numero do local
        /// </summary>
        Number = 4,

        /// <summary>
        /// Complemento do local
        /// </summary>
        Complement = 8,

        /// <summary>
        /// Numero e complemento
        /// </summary>
        LocationInfo = Number + Complement,

        /// <summary>
        /// Logradouro, Numero e complemento
        /// </summary>
        FullLocationInfo = Street + Number + Complement,

        /// <summary>
        /// Bairro
        /// </summary>
        Neighborhood = 16,

        /// <summary>
        /// Cidade
        /// </summary>
        City = 32,

        /// <summary>
        /// Estado
        /// </summary>
        State = 64,

        /// <summary>
        /// Cidade e Estado
        /// </summary>
        CityState = City + State,

        /// <summary>
        /// UF
        /// </summary>
        StateCode = 128,

        /// <summary>
        /// Cidade e UF
        /// </summary>
        CityStateCode = City + StateCode,

        /// <summary>
        /// País
        /// </summary>
        Country = 256,

        /// <summary>
        /// País
        /// </summary>
        CountryCode = 512,

        /// <summary>
        /// CEP
        /// </summary>
        PostalCode = 1024,

        /// <summary>
        /// Endereço completo
        /// </summary>
        FullAddress = Street + LocationInfo + Neighborhood + CityStateCode + Country + PostalCode
    }

    public class AddressTypes
    {
        public static string GetAddressType(string Endereco)
        {
            return GetAddressTypeProperty(Endereco)?.Name.IfBlank("");
        }

        public static PropertyInfo GetAddressTypeProperty(string Endereco)
        {
            string tp = Endereco.IfBlank("").Split(Arrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).FirstOr("");
            if (tp.IsNotBlank())
            {
                var df = new AddressTypes();
                return df.GetProperties().FirstOrDefault(x => tp.IsIn((string[])x.GetValue(df)) || (x.Name ?? "") == (tp ?? ""));
            }

            return null;
        }

        public static string[] GetAddressTypeList(string Endereco)
        {
            return (string[])(GetAddressTypeProperty(Endereco)?.GetValue(new AddressTypes()) ?? new string[] { });
        }

        public string[] Aeroporto { get; private set; } = new[] { "Aeroporto", "Ar", "Aero" };
        public string[] Alameda { get; private set; } = new[] { "Alameda", "Al", "Alm" };
        public string[] Área { get; private set; } = new[] { "Área", "Area" };
        public string[] Avenida { get; private set; } = new[] { "Avenida", "Av", "Avn" };
        public string[] Campo { get; private set; } = new[] { "Cam", "Camp", "Campo" };
        public string[] Chácara { get; private set; } = new[] { "Cha", "Chac", "Chacara" };
        public string[] Colônia { get; private set; } = new[] { "Col", "Colonia" };
        public string[] Condomínio { get; private set; } = new[] { "Condominio", "Cond" };
        public string[] Comunidade { get; private set; } = new[] { "Com", "Comunidade" };
        public string[] Conjunto { get; private set; } = new[] { "Conjunto", "Con" };
        public string[] Distrito { get; private set; } = new[] { "Dis", "Dst", "Distrito" };
        public string[] Esplanada { get; private set; } = new[] { "Esp", "Esplanada" };
        public string[] Estação { get; private set; } = new[] { "Estacao", "Est", "st" };
        public string[] Estrada { get; private set; } = new[] { "Et", "Es", "Estrada" };
        public string[] Favela { get; private set; } = new[] { "Fav", "Favela" };
        public string[] Feira { get; private set; } = new[] { "Fei", "Fr", "Feira" };
        public string[] Jardim { get; private set; } = new[] { "Jd", "Jardim", "Jar" };
        public string[] Ladeira { get; private set; } = new[] { "Lad", "Ld", "Ladeira" };
        public string[] Lago { get; private set; } = new[] { "Lago", "Lg" };
        public string[] Lagoa { get; private set; } = new[] { "Lagoa", "Lga" };
        public string[] Largo { get; private set; } = new[] { "Lrg", "Largo", "Lgo" };
        public string[] Loteamento { get; private set; } = new[] { "Lote", "Loteamento", "Lt" };
        public string[] Morro { get; private set; } = new[] { "Mr", "Mrr", "Morro", "Mor" };
        public string[] Núcleo { get; private set; } = new[] { "Nc", "Nucleo", "Nuc" };
        public string[] Parque { get; private set; } = new[] { "Parque", "Pq", "Pk", "Par", "Parq", "Park" };
        public string[] Passarela { get; private set; } = new[] { "Pass", "Pas", "Pa", "Passarela" };
        public string[] Pátio { get; private set; } = new[] { "Pt", "Pat", "Pateo", "Patio" };
        public string[] Praça { get; private set; } = new[] { "Praça", "Pc", "Praca", "Pç" };
        public string[] Quadra { get; private set; } = new[] { "Qd", "Quadra", "Quad" };
        public string[] Recanto { get; private set; } = new[] { "Rec", "Recanto", "Rc" };
        public string[] Residencial { get; private set; } = new[] { "Residencial", "Residencia", "Res", "Resid" };
        public string[] Rodovia { get; private set; } = new[] { "Rodovia", "Rod" };
        public string[] Rua { get; private set; } = new[] { "Rua", "R" };
        public string[] Setor { get; private set; } = new[] { "Setor", "Str" };
        public string[] Sítio { get; private set; } = new[] { "Sitio", "Sit" };
        public string[] Travessa { get; private set; } = new[] { "Trv", "Travessa", "Tvs" };
        public string[] Trecho { get; private set; } = new[] { "Trc", "Trecho" };
        public string[] Trevo { get; private set; } = new[] { "Trevo" };
        public string[] Vale { get; private set; } = new[] { "Vale", "Val" };
        public string[] Vereda { get; private set; } = new[] { "Vereda" };
        public string[] Via { get; private set; } = new[] { "Via" };
        public string[] Viaduto { get; private set; } = new[] { "Vd", "Viaduto" };
        public string[] Viela { get; private set; } = new[] { "Viela" };
        public string[] Vila { get; private set; } = new[] { "Vila", "Vl" };
    }
}