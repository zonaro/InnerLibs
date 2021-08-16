Imports System.IO
Imports System.Text

Imports InnerLibs
Imports InnerLibs.LINQ
Imports InnerLibs.Locations

''' <summary>
''' Um objeto vCard
''' </summary>
Public Class vCard
    Private _Social As List(Of VSocial)
    Private _Addresses As List(Of vAddress)
    Private _Telephones As List(Of vTelephone)
    Private _Emails As List(Of vEmail)
    Private _URLs As List(Of vURL)
    Public Property Title As String = ""  'Mr., Mrs., Ms., Dr.
    Public Property FirstName As String = ""
    Public Property MiddleName As String = ""
    Public Property LastName As String = ""
    Public Property Suffix As String = "" 'I, II, Jr., Sr.

    Public ReadOnly Property FormattedName As String
        Get
            Return $"{Title} {FirstName} {MiddleName} {LastName} {Suffix}".AdjustBlankSpaces()
        End Get
    End Property

    Public Property Nickname As String = ""
    Public Property Organization As String = ""

    Public Property Company As String
        Get
            Return Organization
        End Get
        Set(value As String)
            Organization = value
        End Set
    End Property

    Public Property OrganizationalUnit As String = ""

    Public Property Department As String
        Get
            Return OrganizationalUnit
        End Get
        Set(value As String)
            OrganizationalUnit = value
        End Set
    End Property

    Public Property Profession As String
        Get
            Return Role
        End Get
        Set(value As String)
            Role = value
        End Set
    End Property

    Public Property Role As String = ""
    Public Property JobTitle As String = ""
    Public Property Note As String = ""
    Public Property Gender As String = ""
    Public Property UID As Guid? = Nothing

    Public Property Birthday As Date? = Nothing

    'Collections
    Public Property URLs As List(Of vURL)
        Get
            _URLs = If(_URLs, New List(Of vURL))
            Return _URLs
        End Get
        Set
            _URLs = Value
        End Set
    End Property

    Public Property Emails As List(Of vEmail)
        Get
            _Emails = If(_Emails, New List(Of vEmail))
            Return _Emails
        End Get
        Set
            _Emails = Value
        End Set
    End Property

    Public Property Telephones As List(Of vTelephone)
        Get
            _Telephones = If(_Telephones, New List(Of vTelephone))
            Return _Telephones
        End Get
        Set
            _Telephones = Value
        End Set
    End Property

    Public Property Addresses As List(Of vAddress)
        Get
            _Addresses = If(_Addresses, New List(Of vAddress))
            Return _Addresses
        End Get
        Set
            _Addresses = Value
        End Set
    End Property

    Public Property Social As List(Of VSocial)
        Get
            _Social = If(_Social, New List(Of VSocial))
            Return _Social
        End Get
        Set
            _Social = Value
        End Set
    End Property

    Public Property LastModified As Date = DateTime.Now

    Public Function AddEmail(Email As String) As vEmail
        If Email.IsEmail Then
            Dim v As New vEmail(Email)
            Emails = If(Emails, New List(Of vEmail))
            Emails.Add(v)
            Return v
        End If
        Return Nothing
    End Function

    Public Function AddTelephone(Tel As String) As vTelephone
        If Tel.IsNotBlank Then
            Dim v As New vTelephone(Tel)
            Telephones = If(Telephones, New List(Of vTelephone))
            Telephones.Add(v)
            Return v
        End If
        Return Nothing
    End Function

    Public Function AddSocial(Name As String, URL As String) As VSocial
        If URL.IsNotBlank AndAlso Name.IsNotBlank Then
            Dim v As New VSocial(Name, URL)
            Social = If(Social, New List(Of VSocial))
            Social.Add(v)
            Return v
        End If
        Return Nothing
    End Function

    Public Sub New()

    End Sub

    Public Overrides Function ToString() As String
        Dim result = ""
        result = result.AppendLine("BEGIN:VCARD")
        result = result.AppendLine("VERSION:3.0")
        result = result.AppendLine($"FN;CHARSET=UTF-8:{FormattedName}")
        result = result.AppendLine($"N;CHARSET=UTF-8:{LastName};{FirstName};{MiddleName};{Title};{Suffix}")

        If Nickname.IsNotBlank Then
            result = result.AppendLine($"NICKNAME;CHARSET=UTF-8:{Nickname}")
        End If

        If Gender.IsNotBlank Then
            result = result.AppendLine($"GENDER:{Gender.GetFirstChars().ToUpper()}")
        End If

        If UID.HasValue Then
            result = result.AppendLine($"UID;CHARSET=UTF-8:{UID}")
        End If

        If Birthday.HasValue Then
            result = result.AppendLine($"BDAY:{Birthday?.ToString("yyyyMMdd")}")
            result = result.AppendLine($"ANNIVERSARY:{Birthday?.ToString("yyyyMMdd")}")
        End If

        If Emails.IsNotNullOrEmpty Then
            result = result.AppendLine(Emails.SelectJoin(Function(x) x.ToString(), Environment.NewLine))
        End If

        If Telephones.IsNotNullOrEmpty Then
            result = result.AppendLine(Telephones.SelectJoin(Function(x) x.ToString(), Environment.NewLine))
        End If

        If Addresses.IsNotNullOrEmpty Then
            result = result.AppendLine(Addresses.SelectJoin(Function(x) x.ToString(), Environment.NewLine))
        End If

        If JobTitle.IsNotBlank Then
            result = result.AppendLine($"TITLE;CHARSET=UTF-8:{JobTitle}")
        End If

        If Role.IsNotBlank Then
            result = result.AppendLine($"ROLE;CHARSET=UTF-8:{Role}")
        End If

        If Organization.IsNotBlank Then
            result = result.AppendLine($"ORG;CHARSET=UTF-8:{Organization}")
        End If

        If URLs.IsNotNullOrEmpty Then
            result = result.AppendLine(URLs.SelectJoin(Function(x) x.ToString(), Environment.NewLine))
        End If

        If Note.IsNotBlank Then
            result = result.AppendLine($"NOTE;CHARSET=UTF-8:{Note}")
        End If

        If Social.IsNotNullOrEmpty Then
            result = result.AppendLine(Social.SelectJoin(Function(x) x.ToString(), ","))
        End If

        result = result.AppendLine($"REV:{LastModified.ToUniversalTime.ToString("yyyyMMdd\THHmmss\Z")}")
        result = result.AppendLine("END:VCARD")
        Return result
    End Function

    Public Function ToFile(FullPath As String) As FileInfo
        Dim fi = ToString.WriteToFile(FullPath, False, New UTF8Encoding(False))
        fi.LastWriteTime = LastModified
        Return fi
    End Function

End Class

Public Class vEmail
    Public Property Preferred As Boolean = False
    Public Property EmailAddress As String = ""
    Public Property Type As String = "INTERNET"

    Public Sub New(ByVal Email As String)
        EmailAddress = Email
    End Sub

    Public Sub New(ByVal Email As String, ByVal IsPreferred As Boolean)
        EmailAddress = Email
        Preferred = IsPreferred
    End Sub

    Public Overrides Function ToString() As String
        Return $"EMAIL{Preferred.AsIf(";PREF")};CHARSET=UTF-8;type={Type.ToUpper()},INTERNET:{EmailAddress}"
    End Function

End Class

Public Class vURL
    Public Property Preferred As Boolean
    Public Property URL As String = ""
    Public Property Location As vLocations = vLocations.WORK

    Public Sub New(ByVal NewURL As String)
        URL = NewURL
    End Sub

    Public Sub New(ByVal NewURL As String, ByVal IsPreffered As Boolean)
        URL = NewURL
        Preferred = IsPreffered
    End Sub

    Public Overrides Function ToString() As String
        Return $"URL{Preferred.AsIf(";PREF")};CHARSET=UTF-8;{Location}:{URL}"
    End Function

End Class

Public Class VSocial

    Public Property URL As String = ""
    Public Property Name As String = ""

    Public Sub New(Name As String, ByVal URL As String)
        Me.URL = URL
        Me.Name = Name
    End Sub

    Public Overrides Function ToString() As String
        Return $"X-SOCIALPROFILE;CHARSET=UTF-8;TYPE={Name.ToUpper()}:{URL}"
    End Function

End Class

Public Class vTelephone
    Public Property Preferred As Boolean
    Public Property TelephoneNumber As String = ""
    Public Property Location As vLocations = vLocations.HOME
    Public Property Type As vPhoneTypes = vPhoneTypes.VOICE

    Public Sub New(ByVal Number As String)
        TelephoneNumber = Number
    End Sub

    Public Sub New(ByVal Number As String, ByVal IsPreferred As Boolean)
        TelephoneNumber = Number
        Preferred = IsPreferred
    End Sub

    Public Sub New(ByVal Number As String, ByVal PhoneLocation As vLocations, ByVal PhoneType As vPhoneTypes, ByVal IsPreferred As Boolean)
        TelephoneNumber = Number
        Location = PhoneLocation
        Type = PhoneType
        Preferred = IsPreferred
    End Sub

    Public Overrides Function ToString() As String
        Return $"TEL{Preferred.AsIf(";PREF")};TYPE={Location},{Type}:{TelephoneNumber}"
    End Function

End Class

Public Class vAddress
    Inherits AddressInfo
    Public Property Preferred As Boolean = False
    Public Property AddressName As String = ""
    Public Property StreetAddress As String = ""
    Public Property AddressLabel As String = ""
    Public Property Location As vLocations = vLocations.HOME
    Public Property AddressType As vAddressTypes = vAddressTypes.INT

    Sub New()

    End Sub

    Public Overrides Function ToString() As String
        Dim result = $"ADR{Preferred.AsIf(";PREF")};TYPE={Type.ToString.ToUpper}:;;{MyBase.ToString(LocationInfo, Neighborhood)};{City};{StateCode.IfBlank(State)};{ZipCode};{Country}".Replace(Environment.NewLine, "=0D=0A")
        ' Post Office Address; Extended Address; Street; Locality; Region; Postal Code; Country)
        'Write the Address label
        If AddressLabel.IsNotBlank Then
            result = result.Append($"{Environment.NewLine}LABEL;{Location.ToString.ToUpper};{AddressType.ToString.ToUpper}:{AddressLabel.Replace(Environment.NewLine, "=0D=0A")}")
        End If
        If LatitudeLongitude().IsNotBlank Then
            result = result.Append($"GEO:{Latitude};{Longitude}")
        End If
        Return result.ToString
    End Function

End Class

Public Enum vLocations
    HOME
    WORK
    CELL
End Enum

Public Enum vAddressTypes
    PARCEL  'Parcel post
    DOM     'Domestic
    INT     'International
End Enum

Public Enum vPhoneTypes
    VOICE
    FAX
    MSG
    PAGER
    BBS
    MODEM
    CAR
    ISDN
    VIDEO
End Enum