Imports System.IO
Imports System.Text

Imports InnerLibs
Imports InnerLibs.LINQ
Imports InnerLibs.Locations

''' <summary>
''' Um objeto vCard
''' </summary>
Public Class vCard

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
    Public Property Organization As String = ""           'MS Outlook calls this Company
    Public Property OrganizationalUnit As String = ""     'MS Outlook calls this Department
    Public Property Role As String = ""   'MS Outlook calls this the profession
    Public Property JobTitle As String = ""
    Public Property Note As String = ""
    Property Gender As String = ""
    Property UID As String = ""

    Public Property Birthday As Date? = Nothing

    'Collections
    Public Property URLs As New List(Of vURL)

    Public Property Emails As New List(Of vEmail)
    Public Property Telephones As New List(Of vTelephone)
    Public Property Addresses As New List(Of vAddress)
    Public Property Social As New List(Of VSocial)
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

        If UID.IsNotBlank Then
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
    Public Property Preferred As Boolean
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
    Public Property Location As vLocations = vLocations.WORK       'MS Outlook shows the WORK location on the contact form front page

    Public Sub New(ByVal NewURL As String)
        URL = NewURL
    End Sub

    Public Sub New(ByVal NewURL As String, ByVal IsPreffered As Boolean)
        URL = NewURL
        Preferred = IsPreffered
    End Sub

    Public Overrides Function ToString() As String
        Dim result = ""
        result &= ("URL")
        If Preferred Then result &= (";PREF")
        If Not Nothing = (Location) Then result &= Format(";{0}", Location.ToString.ToUpper)
        result &= Format(":{0}{1}", URL, System.Environment.NewLine)
        Return result.ToString
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
    Public Property Location As vLocations
    Public Property Type As vPhoneTypes

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
    Public Property Preferred As Boolean
    Public Property AddressName As String = ""    'MS Outlook calls this Office
    Public Property StreetAddress As String = ""
    Public Property AddressLabel As String = ""   'If you don't want to waste time creating this, don't and let the vCard reader format it for you
    Public Property Location As vLocations  'HOME, WORK, CELL
    Public Property AddressType As vAddressTypes    'PARCEL, DOM, INT

    Sub New()

    End Sub


    Public Overrides Function ToString() As String
        Dim result = ""
        'TODO: FIX ADRESSES
        'Write the Address
        result &= ("ADR")
        If Preferred Then result &= (";PREF")
        If Location <> Nothing Then result &= String.Format(";{0}", Location.ToString.ToUpper)
        If Type <> Nothing Then result &= String.Format(";{0}", Type.ToString.ToUpper)
        result &= String.Format(";ENCODING=QUOTED-PRINTABLE:;{0}", AddressName)
        result &= String.Format(";{0}", StreetAddress.Replace(Environment.NewLine, "=0D=0A"))
        result &= String.Format(";{0}", City.Replace(Environment.NewLine, "=0D=0A"))
        result &= String.Format(";{0}", State.Replace(Environment.NewLine, "=0D=0A"))
        result &= String.Format(";{0}", ZipCode.Replace(Environment.NewLine, "=0D=0A"))
        result &= String.Format(";{0}", Country.Replace(Environment.NewLine, "=0D=0A"))
        result &= (System.Environment.NewLine)

        'Write the Address label
        If AddressLabel.IsNotBlank Then
            result &= ("LABEL")
            If Not Nothing = (Location) Then result &= String.Format(";{0}", Location.ToString.ToUpper)
            If Not Nothing = (Type) Then result &= String.Format(";{0}", Type.ToString.ToUpper)
            result &= String.Format(";ENCODING=QUOTED-PRINTABLE:{0}", AddressLabel.Replace(Environment.NewLine, "=0D=0A"))
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
End Enum