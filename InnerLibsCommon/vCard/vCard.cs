using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Extensions.Files;
using Extensions.Locations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using static QRCoder.SvgQRCode.SvgLogo;

namespace Extensions.vCards
{
    /// <summary>
    /// A vCard object compatible with version 4.0 (RFC 6350)
    /// </summary>
    public class vCard
    {
        public vCard()
        {
            var ass = System.Reflection.Assembly.GetExecutingAssembly();
            // assembly name + version
            ProductId = ass.GetName().FullName;
        }

        /// <summary>
        /// List of addresses
        /// </summary>
        public List<(AddressInfo Address, vLocationType Location)> Addresses { get; set; } = new();

        /// <summary>
        /// Anniversary date (ANNIVERSARY) - new in vCard 4.0
        /// </summary>
        public DateTime? Anniversary { get; set; }

        /// <summary>
        /// Birthday date
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Calendar URIs (CALURI) - new in vCard 4.0
        /// </summary>
        public List<string> CalendarURIs { get; set; } = new List<string>();

        /// <summary>
        /// Calendar address URIs (CALADRURI) - new in vCard 4.0
        /// </summary>
        public List<string> CalendarAddressURIs { get; set; } = new List<string>();

        /// <summary>
        /// Categories/tags (CATEGORIES) - enhanced in vCard 4.0
        /// </summary>
        public List<string> Categories { get; set; } = new List<string>();

        /// <summary>
        /// Client unique identifiers (CLIENTPIDMAP) - new in vCard 4.0
        /// </summary>
        public Dictionary<string, string> ClientPidMaps { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Company name (alias for Organization)
        /// </summary>
        public string Company
        {
            get => Organization;
            set => Organization = value;
        }

        /// <summary>
        /// Department name (alias for OrganizationalUnit)
        /// </summary>
        public string Department
        {
            get => OrganizationalUnit;
            set => OrganizationalUnit = value;
        }

        /// <summary>
        /// List of email addresses
        /// </summary>
        public List<(string Email, string Type)?> Emails { get; set; } = new();

        /// <summary>
        /// Free/Busy calendar URLs (FBURL) - new in vCard 4.0
        /// </summary>
        public List<string> FreeBusyURLs { get; set; } = new List<string>();

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Formatted name (FN)
        /// </summary>
        public string FormattedName => $"{Title} {FirstName} {MiddleName} {LastName} {Suffix}".TrimBetween();

        /// <summary>
        /// Geographic coordinates extracted from addresses
        /// </summary>
        public IEnumerable<(decimal Latitude, decimal Longitude, vLocationType Type)> GeoLocations
        {
            get
            {
                if (Addresses != null && Addresses?.Any() == true)
                {
                    foreach (var addr in Addresses)
                    {
                        var geoLocation = addr.Address.GeoLocation();
                        if (geoLocation.HasValue)
                            yield return (geoLocation.Value.Latitude, geoLocation.Value.Longitude, addr.Location);
                    }
                }
            }
        }

        private Dictionary<string, string> _extraFields { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Adds or updates an extra field in the vCard
        /// </summary>
        public vCard Extra<T>(string Key, T Value)
        {
            if (Key.IsNotBlank())
            {
                var prop = this.GetProperty(Key);
                if (prop != null)
                {
                    if (Value != null)
                    {
                        if (Util.GetTypeOf(Value) == prop.PropertyType)
                        {
                            prop.SetValue(this, Value);
                        }
                        else
                        {
                            var v = Value.ChangeTypeStringFallback(prop.PropertyType) ?? throw new InvalidCastException($"Cannot convert value of type {Util.GetTypeOf(Value).FullName} to {prop.PropertyType.FullName}");
                            prop.SetValue(this, v);
                        }
                    }
                }
                else
                {
                    _extraFields.SetOrRemove($"{Key.ToUpper()}", Value);
                }
                return this;
            }
            return this;
        }

        /// <summary>
        /// Gets an extra field from the vCard
        /// </summary>
        public string Extra(string Key)
        {
            if (Key.IsNotBlank())
            {
                return _extraFields.GetValueOrDefault($"{Key.ToUpper()}");
            }

            return null;
        }

        public IEnumerable<string> ExtraFields => _extraFields.Keys;

        /// <summary>
        /// Indexer for accessing or setting extra fields in the vCard
        /// </summary>
        public string this[string key]
        {
            get => Extra(key);
            set => Extra(key, value);
        }

        /// <summary>
        /// Gender
        /// </summary>
        public (vGender Gender, string Description)? Gender { get; set; }

        /// <summary>
        /// Instant messaging (IMPP) - new in vCard 4.0
        /// </summary>
        public List<(string protocol, string handle)> InstantMessaging { get; set; } = new();

        /// <summary>
        /// Job title
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Entity type (KIND) - new in vCard 4.0
        /// </summary>
        public vKind Kind { get; set; } = vKind.Individual;

        /// <summary>
        /// Languages (LANG) - new in vCard 4.0
        /// </summary>
        public List<string> Languages { get; set; } = new();

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.Now;

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Group members (MEMBER) - new in vCard 4.0, used when Kind = Group
        /// </summary>
        public List<string> Members { get; set; } = new List<string>();

        public string MiddleName { get; set; }
        public string Nickname { get; set; }
        public string Note { get; set; }
        public string Organization { get; set; }
        public string OrganizationalUnit { get; set; }

        /// <summary>
        /// Photos of the person (PHOTO) - enhanced in vCard 4.0
        /// </summary>
        public List<(string Uri, string MediaType, vPhotoType Type)> Photos { get; set; } = new();

        /// <summary>
        /// Profession (ROLE) - enhanced in vCard 4.0
        /// </summary>
        public string Profession
        {
            get => Role;
            set => Role = value;
        }

        /// <summary>
        /// Product identifiers (PRODID) - enhanced in vCard 4.0
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Relationships (RELATED) - new in vCard 4.0
        /// </summary>
        public List<(string Type, string Value)> Related { get; set; } = new List<(string Type, string Value)>();

        public string Role { get; set; }

        /// <summary>
        /// Sound files (SOUND) - enhanced in vCard 4.0
        /// </summary>
        public List<string> SoundURIs { get; set; } = new List<string>();

        public string Suffix { get; set; }

        public List<(string Number, vPhoneTypes Type, vLocationType Location)?> Telephones { get; set; } = new();

        public string Title { get; set; }   // Mr., Mrs., Ms., Dr.

        public Guid? UID { get; set; } = Guid.NewGuid();

        public List<(Uri URL, vLocationType Location)?> URLs { get; set; } = new();

        /// <summary>
        /// XML data (XML) - new in vCard 4.0
        /// </summary>
        public List<string> XmlData { get; set; } = new List<string>();

        /// <summary>
        /// Adds an email address to the vCard
        /// </summary>
        public void AddEmail(string Email, string Type = null)
        {
            Emails = Emails ?? new();
            if (Email.IsEmail())
            {
                Emails.Add((Email, Type.IfBlank("INTERNET")));
            }
        }

        /// <summary>
        /// Adds an address to the vCard
        /// </summary>
        public void AddAddress(string Address, vLocationType Location = vLocationType.HOME)
        {
            var addr = AddressInfo.TryParse(Address);

            AddAddress(addr, Location);
        }

        /// <summary>
        /// Adds an address to the vCard
        /// </summary>
        public void AddAddress(AddressInfo Address, vLocationType Location = vLocationType.HOME)
        {
            Addresses = Addresses ?? new();
            if (Address != null)
            {
                Addresses.Add((Address, Location));
            }
        }

        /// <summary>
        /// Adds a telephone to the vCard
        /// </summary>
        public void AddTelephone(string Number, vLocationType PhoneLocation = vLocationType.HOME, vPhoneTypes PhoneType = vPhoneTypes.VOICE)
        {
            Telephones = Telephones ?? new();
            if (Number.IsValid())
            {
                var v = Telephones!.FirstOrDefault(x => x != null && x.Value.Number.FlatEqual(Number) && x.Value.Type == PhoneType && x.Value.Location == PhoneLocation);
                if (v != null)
                {
                    Telephones.Remove(v);
                }

                Telephones.Add((Number, PhoneType, PhoneLocation));
            }
        }

        /// <summary>
        /// Adds a URL to the vCard
        /// </summary>
        public void AddURL(string URL, vLocationType Location = vLocationType.HOME)
        {
            if (URL.IsURL())
                AddURL(URL, Location);
        }

        /// <summary>
        /// Adds a URL to the vCard
        /// </summary>
        public void AddURL(Uri URL, vLocationType Location = vLocationType.HOME)
        {
            URLs = URLs ?? new();
            URLs.Add((URL, Location));
        }

        /// <summary>
        /// Adds an instant messaging service
        /// </summary>
        public void AddInstantMessaging(string protocol, string handle)
        {
            if (protocol.IsValid() && handle.IsValid())
            {
                InstantMessaging.Add((protocol, handle));
            }
        }

        /// <summary>
        /// Adds a language
        /// </summary>
        public void AddLanguage(string languageCode)
        {
            if (languageCode.IsValid())
            {
                try
                {
                    var culture = new CultureInfo(languageCode);
                    AddLanguage(culture);
                }
                catch (CultureNotFoundException)
                {
                }
            }
        }

        public void AddLanguage(CultureInfo language)
        {
            if (language != null)
            {
                Languages.Add(language.Name);
            }
        }

        /// <summary>
        /// Adds a relationship
        /// </summary>
        public void AddRelated(string type, string value)
        {
            if (type.IsValid() && value.IsValid())
            {
                Related.Add((type, value));
            }
        }

        /// <summary>
        /// Adds a photo
        /// </summary>
        public void AddPhoto(string Value, string MediaType, vPhotoType type = vPhotoType.URI)
        {
            if (Value.IsNotBlank())
            {
                if (MediaType.IsBlank())
                    MediaType = FileType.GetMimeType(Value);

                Photos.Add((Value, MediaType, type));
            }
        }

        /// <summary>
        /// Saves the vCard to a directory
        /// </summary>
        public FileInfo SaveAs(DirectoryInfo Directory) => SaveAs(Directory?.FullName);

        /// <summary>
        /// Saves the vCard to a file
        /// </summary>
        public FileInfo SaveAs(FileInfo File) => SaveAs(File?.FullName);

        /// <summary>
        /// Saves the vCard to a specific path
        /// </summary>
        public FileInfo SaveAs(string FullPath)
        {
            if (FullPath.IsDirectoryPath())
            {
                FullPath = Path.Combine(FullPath, FormattedName.BlankCoalesce(UID?.ToString(), DateTime.Now.Ticks.ToString()) + ".vcf");
            }

            if (FullPath.IsFilePath())
            {
                var fi = ToString().WriteToFile(FullPath, false, new UTF8Encoding(false), LastModified);
                fi.LastWriteTime = LastModified;
                return fi;
            }
            return null;
        }

        private string TupleToString(string FieldName, string Type, string Value, int Count, int Pref)
        {
            var prefParam = Count > 1 ? $"PREF={Pref};" : "";
            var typeParam = Type.IsNotBlank() ? $"TYPE={Type.ToString().ToUpperInvariant()};" : "";
            return $"{FieldName.ToUpper()};{prefParam}{typeParam}:{Value}" + Environment.NewLine;
        }

        /// <summary>
        /// Converts the vCard to string in vCard 4.0 format
        /// </summary>
        public override string ToString()
        {
            string result = Util.EmptyString;
            result += "BEGIN:VCARD" + Environment.NewLine;
            result += "VERSION:4.0" + Environment.NewLine;

            if (ProductId.IsValid())
            {
                result += $"PRODID:{ProductId}" + Environment.NewLine;
            }

            result += $"FN:{FormattedName}" + Environment.NewLine;
            result += $"N:{LastName};{FirstName};{MiddleName};{Title};{Suffix}" + Environment.NewLine;

            if (Kind != vKind.Individual)
            {
                result += $"KIND:{Kind.ToString().ToLowerInvariant()}" + Environment.NewLine;
            }

            if (Nickname.IsValid())
            {
                result += $"NICKNAME:{Nickname}" + Environment.NewLine;
            }

            if (Gender != null)
            {
                var desc = Gender.Value.Description.IfBlank(Gender?.Gender.GetDisplayString()).Replace("\n", "\\n").Replace(";", "\\;");
                result += $"GENDER:{Gender.Value.Gender};{desc}" + Environment.NewLine;
            }

            if (UID.HasValue)
            {
                result += $"UID:{UID}" + Environment.NewLine;
            }

            if (Birthday.HasValue)
            {
                result += $"BDAY:{Birthday?.ToString("yyyy-MM-dd")}" + Environment.NewLine;
            }

            if (Anniversary.HasValue)
            {
                result += $"ANNIVERSARY:{Anniversary?.ToString("yyyy-MM-dd")}" + Environment.NewLine;
            }

            if (Photos.IsNotNullOrEmpty())
            {
                result += Photos.SelectJoinString((x, i) =>
                {
                    var mediaTypeParam = x.MediaType.IsNotBlank() ? $"MEDIATYPE={x.MediaType};" : "";
                    string enc = "";
                    string value = x.Uri;
                    var ft = FileType.GetFileType(x.MediaType);
                    var Type = ft.SubTypes.FirstOrDefault() ?? "JPEG";
                    if (ft.IsImage())
                    {
                        if (x.Type == vPhotoType.BINARY)
                        {
                            return $"PHOTO;TYPE={Type};{mediaTypeParam}ENCODING=b:{x.Uri}{Environment.NewLine}";
                        }
                        else
                        {
                            return $"PHOTO;TYPE={Type};{mediaTypeParam}{enc}:uri:{x.Uri}{Environment.NewLine}";
                        }
                    }
                    return "";
                }, Environment.NewLine) + Environment.NewLine;
            }

            if (Emails.IsNotNullOrEmpty())
            {
                result += Emails.SelectJoinString((x, i) =>
                {
                    return TupleToString("EMAIL", x.Value.Type.IfBlank("INTERNET"), x.Value.Email, Emails.Count, i);
                }, Environment.NewLine) + Environment.NewLine;
            }

            if (Telephones.IsNotNullOrEmpty())
            {
                result += Telephones.SelectJoinString((x, i) =>
                {
                    return TupleToString("TEL", x.Value.Type.ToString(), x.Value.Number, Telephones.Count, i);
                }, Environment.NewLine) + Environment.NewLine;
            }

            if (Addresses.IsNotNullOrEmpty())
            {
                result += Addresses.SelectJoinString(x => x.ToString(), Environment.NewLine) + Environment.NewLine;
            }

            // GEO separado dos endereços conforme vCard 4.0
            var gg = GeoLocations.ToList();
            if (gg.IsNotNullOrEmpty())
            {
                result += gg.SelectJoinString((x, i) =>
                {
                    return TupleToString("GEO", x.Type.ToString(), $"{x.Latitude}, {x.Longitude}", gg.Count, i);
                }, Environment.NewLine) + Environment.NewLine;
            }

            if (InstantMessaging.IsNotNullOrEmpty())
            {
                result += InstantMessaging.SelectJoinString((x, i) =>
                {
                    return TupleToString("IMPP", null, Util.JoinString([x.protocol, x.handle], ':'), InstantMessaging.Count, i);
                }, Environment.NewLine) + Environment.NewLine;
            }

            if (Languages.IsNotNullOrEmpty())
            {
                result += Languages.SelectJoinString((x, i) =>
                {
                    return TupleToString("LANG", null, x, Languages.Count, i);
                }, Environment.NewLine) + Environment.NewLine;
            }

            if (JobTitle.IsValid())
            {
                result += $"TITLE:{JobTitle}" + Environment.NewLine;
            }

            if (Role.IsValid())
            {
                result += $"ROLE:{Role}" + Environment.NewLine;
            }

            if (Organization.IsValid())
            {
                var orgValue = Organization;
                if (OrganizationalUnit.IsValid())
                {
                    orgValue += $";{OrganizationalUnit}";
                }
                result += $"ORG:{orgValue}" + Environment.NewLine;
            }

            if (Categories.IsNotNullOrEmpty())
            {
                result += $"CATEGORIES:{string.Join(",", Categories)}" + Environment.NewLine;
            }

            if (URLs.IsNotNullOrEmpty())
            {
                result += URLs.SelectJoinString(x => x.ToString(), Environment.NewLine) + Environment.NewLine;
            }

            if (CalendarURIs.IsNotNullOrEmpty())
            {
                foreach (var calUri in CalendarURIs)
                {
                    result += $"CALURI:{calUri}" + Environment.NewLine;
                }
            }

            if (CalendarAddressURIs.IsNotNullOrEmpty())
            {
                foreach (var calAddrUri in CalendarAddressURIs)
                {
                    result += $"CALADRURI:{calAddrUri}" + Environment.NewLine;
                }
            }

            if (FreeBusyURLs.IsNotNullOrEmpty())
            {
                foreach (var fbUrl in FreeBusyURLs)
                {
                    result += $"FBURL:{fbUrl}" + Environment.NewLine;
                }
            }

            if (SoundURIs.IsNotNullOrEmpty())
            {
                foreach (var soundUri in SoundURIs)
                {
                    result += $"SOUND:{soundUri}" + Environment.NewLine;
                }
            }

            if (Related.IsNotNullOrEmpty())
            {
                result += Related.SelectJoinString(x => $"RELATED;TYPE={x.Type.ToUpperInvariant()}:{x.Value}", Environment.NewLine) + Environment.NewLine;
            }

            if (Members.IsNotNullOrEmpty())
            {
                foreach (var member in Members)
                {
                    result += $"MEMBER:{member}" + Environment.NewLine;
                }
            }

            if (XmlData.IsNotNullOrEmpty())
            {
                foreach (var xml in XmlData)
                {
                    result += $"XML:{xml}" + Environment.NewLine;
                }
            }

            if (ClientPidMaps.IsNotNullOrEmpty())
            {
                foreach (var pidMap in ClientPidMaps)
                {
                    result += $"CLIENTPIDMAP:{pidMap.Key};{pidMap.Value}" + Environment.NewLine;
                }
            }

            if (Note.IsValid())
            {
                result += $"NOTE:{Note}" + Environment.NewLine;
            }

            foreach (var field in _extraFields)
            {
                if (field.Key.IsValid() && field.Value.IsValid())
                {
                    result += $"X-{field.Key}:{field.Value.Replace(Environment.NewLine, "\\n")}" + Environment.NewLine;
                }
            }

            result += $"REV:{LastModified.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss\Z")}" + Environment.NewLine;
            result += "END:VCARD" + Environment.NewLine;
            return result;
        }
    }
}