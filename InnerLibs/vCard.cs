using InnerLibs.LINQ;
using InnerLibs.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InnerLibs
{
    public enum vAddressTypes
    {
        PARCEL,  // Parcel post
        DOM,     // Domestic
        INT     // International
    }

    public enum vLocations
    {
        HOME,
        WORK,
        CELL
    }

    public enum vPhoneTypes
    {
        VOICE,
        FAX,
        MSG,
        PAGER,
        BBS,
        MODEM,
        CAR,
        ISDN,
        VIDEO
    }

    public class vAddress : AddressInfo
    {
        public vAddress()
        {
        }

        public string AddressLabel { get; set; } = "";
        public string AddressName { get; set; } = "";
        public vAddressTypes AddressType { get; set; } = vAddressTypes.INT;
        public vLocations Location { get; set; } = vLocations.HOME;
        public bool Preferred { get; set; } = false;
        public string StreetAddress { get; set; } = "";

        public override string ToString()
        {
            string result = $"ADR{Preferred.AsIf(";PREF")};CHARSET=UTF-8;TYPE={AddressType.ToString().ToUpper()}:;;{ToString(AddressPart.FullLocationInfo | AddressPart.Neighborhood)};{City};{StateCode.IfBlank(State)};{ZipCode};{Country}".Replace(Environment.NewLine, "=0D=0A");
            if (AddressLabel.IsNotBlank())
            {
                result = result.Append($"{Environment.NewLine}LABEL;CHARSET=UTF-8;{Location.ToString().ToUpper()};{AddressType.ToString().ToUpper()}:{AddressLabel.Replace(Environment.NewLine, "=0D=0A")}");
            }

            if (LatitudeLongitude().IsNotBlank())
            {
                result = result.Append($"GEO:{Latitude};{Longitude}");
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Um objeto vCard
    /// </summary>
    public class vCard
    {
        private List<vAddress> _Addresses;
        private List<vEmail> _Emails;
        private List<vSocial> _Social;
        private List<vTelephone> _Telephones;
        private List<vURL> _URLs;

        public vCard()
        {
        }

        public List<vAddress> Addresses
        {
            get
            {
                _Addresses = _Addresses ?? new List<vAddress>();
                return _Addresses;
            }

            set
            {
                _Addresses = value;
            }
        }

        public DateTime? Birthday { get; set; } = default;

        public string Company
        {
            get
            {
                return Organization;
            }

            set
            {
                Organization = value;
            }
        }

        public string Department
        {
            get
            {
                return OrganizationalUnit;
            }

            set
            {
                OrganizationalUnit = value;
            }
        }

        public List<vEmail> Emails
        {
            get
            {
                _Emails = _Emails ?? new List<vEmail>();
                return _Emails;
            }

            set
            {
                _Emails = value;
            }
        }

        public string FirstName { get; set; } = "";

        public string FormattedName
        {
            get
            {
                return $"{Title} {FirstName} {MiddleName} {LastName} {Suffix}".AdjustBlankSpaces();
            }
        }

        public string Gender { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public DateTime LastModified { get; set; } = DateTime.Now;
        public string LastName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string Nickname { get; set; } = "";
        public string Note { get; set; } = "";
        public string Organization { get; set; } = "";
        public string OrganizationalUnit { get; set; } = "";

        public string Profession
        {
            get
            {
                return Role;
            }

            set
            {
                Role = value;
            }
        }

        public string Role { get; set; } = "";

        public List<vSocial> Social
        {
            get
            {
                _Social = _Social ?? new List<vSocial>();
                return _Social;
            }

            set
            {
                _Social = value;
            }
        }

        public string Suffix { get; set; } = "";

        public List<vTelephone> Telephones
        {
            get
            {
                _Telephones = _Telephones ?? new List<vTelephone>();
                return _Telephones;
            }

            set
            {
                _Telephones = value;
            }
        }

        public string Title { get; set; } = "";  // Mr., Mrs., Ms., Dr.

        // I, II, Jr., Sr.
        public Guid? UID { get; set; } = default;

        // Collections
        public List<vURL> URLs
        {
            get
            {
                _URLs = _URLs ?? new List<vURL>();
                return _URLs;
            }

            set
            {
                _URLs = value;
            }
        }

        public vEmail AddEmail(string Email)
        {
            if (Email.IsEmail())
            {
                var v = new vEmail(Email);
                Emails = Emails ?? new List<vEmail>();
                Emails.Add(v);
                return v;
            }

            return null;
        }

        public vSocial AddSocial(string Name, string URL)
        {
            if (URL.IsNotBlank() && Name.IsNotBlank())
            {
                var v = new vSocial(Name, URL);
                Social = Social ?? new List<vSocial>();
                Social.Add(v);
                return v;
            }

            return null;
        }

        public vTelephone AddTelephone(string Tel)
        {
            if (Tel.IsNotBlank())
            {
                var v = new vTelephone(Tel);
                Telephones = Telephones ?? new List<vTelephone>();
                Telephones.Add(v);
                return v;
            }

            return null;
        }

        public vURL AddURL(string URL)
        {
            if (URL.IsURL())
            {
                var v = new vURL(URL);
                URLs = URLs ?? new List<vURL>();
                URLs.Add(v);
                return v;
            }

            return null;
        }

        public FileInfo ToFile(string FullPath)
        {
            var fi = ToString().WriteToFile(FullPath, false, new UTF8Encoding(false));
            fi.LastWriteTime = LastModified;
            return fi;
        }

        public override string ToString()
        {
            string result = "";
            result = result.AppendLine("BEGIN:VCARD");
            result = result.AppendLine("VERSION:3.0");
            result = result.AppendLine($"FN;CHARSET=UTF-8:{FormattedName}");
            result = result.AppendLine($"N;CHARSET=UTF-8:{LastName};{FirstName};{MiddleName};{Title};{Suffix}");
            if (Nickname.IsNotBlank())
            {
                result = result.AppendLine($"NICKNAME;CHARSET=UTF-8:{Nickname}");
            }

            if (Gender.IsNotBlank())
            {
                result = result.AppendLine($"GENDER:{Gender.GetFirstChars().ToUpper()}");
            }

            if (UID.HasValue)
            {
                result = result.AppendLine($"UID;CHARSET=UTF-8:{UID}");
            }

            if (Birthday.HasValue)
            {
                result = result.AppendLine($"BDAY:{Birthday?.ToString("yyyyMMdd")}");
                result = result.AppendLine($"ANNIVERSARY:{Birthday?.ToString("MMdd")}");
            }

            if (Emails.IsNotNullOrEmpty())
            {
                result = result.AppendLine(Emails.SelectJoinString(x => x.ToString(), Environment.NewLine));
            }

            if (Telephones.IsNotNullOrEmpty())
            {
                result = result.AppendLine(Telephones.SelectJoinString(x => x.ToString(), Environment.NewLine));
            }

            if (Addresses.IsNotNullOrEmpty())
            {
                result = result.AppendLine(Addresses.SelectJoinString(x => x.ToString(), Environment.NewLine));
            }

            if (JobTitle.IsNotBlank())
            {
                result = result.AppendLine($"TITLE;CHARSET=UTF-8:{JobTitle}");
            }

            if (Role.IsNotBlank())
            {
                result = result.AppendLine($"ROLE;CHARSET=UTF-8:{Role}");
            }

            if (Organization.IsNotBlank())
            {
                result = result.AppendLine($"ORG;CHARSET=UTF-8:{Organization}");
            }

            if (URLs.IsNotNullOrEmpty())
            {
                result = result.AppendLine(URLs.SelectJoinString(x => x.ToString(), Environment.NewLine));
            }

            if (Note.IsNotBlank())
            {
                result = result.AppendLine($"NOTE;CHARSET=UTF-8:{Note}");
            }

            if (Social.IsNotNullOrEmpty())
            {
                result = result.AppendLine(Social.SelectJoinString(x => x.ToString(), ","));
            }

            result = result.AppendLine($"REV:{LastModified.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z")}");
            result = result.AppendLine("END:VCARD");
            return result;
        }
    }

    public class vEmail
    {
        public vEmail(string Email)
        {
            EmailAddress = Email;
        }

        public vEmail(string Email, bool IsPreferred)
        {
            EmailAddress = Email;
            Preferred = IsPreferred;
        }

        public string EmailAddress { get; set; } = "";
        public bool Preferred { get; set; } = false;
        public string Type { get; set; } = "INTERNET";

        public override string ToString()
        {
            return $"EMAIL{Preferred.AsIf(";PREF")};CHARSET=UTF-8;type={Type.ToUpper()},INTERNET:{EmailAddress}";
        }
    }

    public class vSocial
    {
        public vSocial(string Name, string URL)
        {
            this.URL = URL;
            this.Name = Name;
        }

        public string Name { get; set; } = "";
        public string URL { get; set; } = "";

        public override string ToString()
        {
            return $"X-SOCIALPROFILE;CHARSET=UTF-8;TYPE={Name.ToUpper()}:{URL}";
        }
    }

    public class vTelephone
    {
        public vTelephone(string Number)
        {
            TelephoneNumber = Number;
        }

        public vTelephone(string Number, bool IsPreferred)
        {
            TelephoneNumber = Number;
            Preferred = IsPreferred;
        }

        public vTelephone(string Number, vLocations PhoneLocation, vPhoneTypes PhoneType, bool IsPreferred)
        {
            TelephoneNumber = Number;
            Location = PhoneLocation;
            Type = PhoneType;
            Preferred = IsPreferred;
        }

        public vLocations Location { get; set; } = vLocations.HOME;
        public bool Preferred { get; set; }
        public string TelephoneNumber { get; set; } = "";
        public vPhoneTypes Type { get; set; } = vPhoneTypes.VOICE;

        public override string ToString()
        {
            return $"TEL{Preferred.AsIf(";PREF")};TYPE={Location},{Type}:{TelephoneNumber}";
        }
    }

    public class vURL
    {
        public vURL(string NewURL)
        {
            URL = NewURL;
        }

        public vURL(string NewURL, bool IsPreffered)
        {
            URL = NewURL;
            Preferred = IsPreffered;
        }

        public vLocations Location { get; set; } = vLocations.WORK;
        public bool Preferred { get; set; }
        public string URL { get; set; } = "";

        public override string ToString()
        {
            return $"URL{Preferred.AsIf(";PREF")};CHARSET=UTF-8;{Location}:{URL}";
        }
    }
}