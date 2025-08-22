﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Extensions.Locations;

namespace Extensions.vCards
{

    public class vAddress : AddressInfo
    {
        #region Public Constructors

        public vAddress()
        {
            this.AddressType = vAddressTypes.INT;
            this.Location = vLocations.HOME;
        }

        #endregion Public Constructors

        #region Public Properties

        public vAddressTypes AddressType { get => this[nameof(AddressType)].GetEnumValue<vAddressTypes>(); set => this[nameof(AddressType)] = value.ToString(); }
        public vLocations Location { get => this[nameof(Location)].GetEnumValue<vLocations>(); set => this[nameof(Location)] = value.ToString(); }
        public bool Preferred { get => this[nameof(Preferred)].AsBool(); set => this[nameof(Preferred)] = $"{value}"; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            string result = $"ADR{Preferred.AsIf(";PREF")};CHARSET=UTF-8;TYPE={AddressType.ToString().ToUpperInvariant()}:;;{ToString(AddressPart.FullBuildingInfo | AddressPart.Neighborhood)};{City};{StateCode.IfBlank(State)};{ZipCode};{Country}".Replace(Environment.NewLine, "=0D=0A");
            if (Label.IsValid())
            {
                result = result.Append($"{Environment.NewLine}LABEL;CHARSET=UTF-8;{Location.ToString().ToUpperInvariant()};{AddressType.ToString().ToUpperInvariant()}:{Label.Replace(Environment.NewLine, "=0D=0A")}");
            }

            if (GeoLocation().IsValid())
            {
                result = result.Append($"GEO:{Latitude?.ToString(CultureInfo.InvariantCulture)};{Longitude?.ToString(CultureInfo.InvariantCulture)}");
            }

            return result.ToString();
        }



        #endregion Public Methods
    }

    /// <summary>
    /// Um objeto vCard
    /// </summary>
    public class vCard
    {
        #region Public Constructors

        public vCard()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public List<vAddress> Addresses { get; set; } = new List<vAddress>();

        public DateTime? Birthday { get; set; }

        public string Company
        {
            get => Organization;

            set => Organization = value;
        }

        public string Department
        {
            get => OrganizationalUnit;

            set => OrganizationalUnit = value;
        }

        public List<vEmail> Emails { get; set; } = new List<vEmail>();

        public string FirstName { get; set; }

        public string FormattedName => $"{Title} {FirstName} {MiddleName} {LastName} {Suffix}".TrimBetween();

        public string Gender { get; set; }
        public string JobTitle { get; set; }
        public DateTime LastModified { get; set; } = DateTime.Now;
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Nickname { get; set; }
        public string Note { get; set; }
        public string Organization { get; set; }
        public string OrganizationalUnit { get; set; }

        public string Profession
        {
            get => Role;

            set => Role = value;
        }

        public string Role { get; set; }

        public List<vSocial> Social { get; set; } = new List<vSocial>();

        public string Suffix { get; set; }

        public List<vTelephone> Telephones { get; set; } = new List<vTelephone>();

        public string Title { get; set; }   // Mr., Mrs., Ms., Dr.

        public Guid? UID { get; set; } = Guid.NewGuid();

        // Collections
        public List<vURL> URLs { get; set; }

        #endregion Public Properties

        #region Public Methods

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

        public vSocial AddSocial(string Name, Uri URL) => AddSocial(Name, URL?.AbsoluteUri);
        public vSocial AddSocial(string Name, string URL)
        {
            if (URL.IsValid() && Name.IsValid())
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
            if (Tel.IsValid())
            {
                var v = new vTelephone(Tel);
                Telephones = Telephones ?? new List<vTelephone>();
                Telephones.Add(v);
                return v;
            }

            return null;
        }

        public vURL AddURL(Uri URL) => AddURL(URL?.AbsoluteUri);
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

        public FileInfo SaveAs(DirectoryInfo Directory) => SaveAs(Directory?.FullName);
        public FileInfo SaveAs(FileInfo File) => SaveAs(File?.FullName);
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

        public override string ToString()
        {
            string result = Util.EmptyString;
            result = result.AppendLine("BEGIN:VCARD");
            result = result.AppendLine("VERSION:3.0");
            result = result.AppendLine($"FN;CHARSET=UTF-8:{FormattedName}");
            result = result.AppendLine($"N;CHARSET=UTF-8:{LastName};{FirstName};{MiddleName};{Title};{Suffix}");
            if (Nickname.IsValid())
            {
                result = result.AppendLine($"NICKNAME;CHARSET=UTF-8:{Nickname}");
            }

            if (Gender.IsValid())
            {
                result = result.AppendLine($"GENDER:{Gender.GetFirstChars().ToUpperInvariant()}");
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

            if (JobTitle.IsValid())
            {
                result = result.AppendLine($"TITLE;CHARSET=UTF-8:{JobTitle}");
            }

            if (Role.IsValid())
            {
                result = result.AppendLine($"ROLE;CHARSET=UTF-8:{Role}");
            }

            if (Organization.IsValid())
            {
                result = result.AppendLine($"ORG;CHARSET=UTF-8:{Organization}");
            }

            if (URLs.IsNotNullOrEmpty())
            {
                result = result.AppendLine(URLs.SelectJoinString(x => x.ToString(), Environment.NewLine));
            }

            if (Note.IsValid())
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

        #endregion Public Methods
    }

    public class vEmail
    {
        #region Public Constructors

        public vEmail(string Email)
        {
            EmailAddress = Email;
        }

        public vEmail(string Email, bool IsPreferred)
        {
            EmailAddress = Email;
            Preferred = IsPreferred;
        }

        #endregion Public Constructors

        #region Public Properties

        public string EmailAddress { get; set; }
        public bool Preferred { get; set; }
        public string Type { get; set; } = "INTERNET";

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => $"EMAIL{Preferred.AsIf(";PREF")};CHARSET=UTF-8;type={Type.ToUpperInvariant()},INTERNET:{EmailAddress}";

        #endregion Public Methods
    }

    public class vSocial
    {
        #region Public Constructors

        public vSocial(string Name, string URL)
        {
            this.URL = URL;
            this.Name = Name;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Name { get; set; }
        public string URL { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => $"X-SOCIALPROFILE;CHARSET=UTF-8;TYPE={Name.ToUpperInvariant()}:{URL}";

        #endregion Public Methods
    }

    public class vTelephone
    {
        #region Public Constructors

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

        #endregion Public Constructors

        #region Public Properties

        public vLocations Location { get; set; } = vLocations.HOME;
        public bool Preferred { get; set; }
        public string TelephoneNumber { get; set; }
        public vPhoneTypes Type { get; set; } = vPhoneTypes.VOICE;

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => $"TEL{Preferred.AsIf(";PREF")};TYPE={Location},{Type}:{TelephoneNumber}";

        #endregion Public Methods
    }

    public class vURL
    {
        #region Public Constructors

        public vURL(string URL)
        {
            this.URL = URL;
        }

        public vURL(string URL, bool IsPreffered)
        {
            this.URL = URL;
            Preferred = IsPreffered;
        }

        #endregion Public Constructors

        #region Public Properties

        public vLocations Location { get; set; } = vLocations.WORK;
        public bool Preferred { get; set; }
        public string URL { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => $"URL{Preferred.AsIf(";PREF")};CHARSET=UTF-8;{Location}:{URL}";

        #endregion Public Methods
    }

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

}