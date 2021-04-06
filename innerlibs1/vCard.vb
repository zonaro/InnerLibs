Imports System.IO
Imports System.Text
Imports System.Web
Imports InnerLibs

''' <summary>
''' Um objeto vCard
''' </summary>
Public Class vCard

    Public Property Title As String = ""  'Mr., Mrs., Ms., Dr.
    Public Property FirstName As String = ""
    Public Property MiddleName As String = ""
    Public Property LastName As String = ""
    Public Property Suffix As String = "" 'I, II, Jr., Sr.
    Public Property FormattedName As String = ""
    Public Property Nickname As String = ""
    Public Property Organization As String = ""           'MS Outlook calls this Company
    Public Property OrganizationalUnit As String = ""     'MS Outlook calls this Department
    Public Property Role As String = ""   'MS Outlook calls this the profession
    Public Property JobTitle As String = ""
    Public Property Note As String = ""
    Public Property Birthday As Date? = Nothing

    'Collections
    Public Property URLs As New vURLs()

    Public Property Emails As New vEmails()
    Public Property Telephones As New vTelephones()
    Public Property Addresses As New vAddresss()
    Public Property LastModified As Date

    Public Sub New(Optional FirstName As String = "", Optional LastName As String = "")
        Me.FirstName = FirstName
        Me.LastName = LastName
    End Sub

    Public Overrides Function ToString() As String
        Dim result = ""
        result &= Format("BEGIN:VCARD{0}", System.Environment.NewLine)
        result &= Format("VERSION:2.1{0}", System.Environment.NewLine)
        result &= Format("N:{0};{1};{2};{3};{4}{5}", LastName, FirstName, MiddleName, Title, Suffix, System.Environment.NewLine)
        If IsNotBlank(FormattedName) Then result &= Format("FN:{0}{1}", FormattedName, System.Environment.NewLine)
        If IsNotBlank(Nickname) Then result &= Format("NICKNAME:{0}{1}", Nickname, System.Environment.NewLine)
        If Birthday.HasValue AndAlso Birthday.Value > Date.MinValue Then result &= Format("BDAY:{0}{1}", Birthday.Value.ToUniversalTime.ToString("yyyyMMdd"), System.Environment.NewLine)
        If IsNotBlank(Note) Then result &= Format("NOTE;ENCODING=QUOTED-PRINTABLE:{0}{1}", Note.Replace(System.Environment.NewLine, "=0D=0A"), System.Environment.NewLine)
        result &= Format("ORG:{0};{1}{2}", Organization, OrganizationalUnit, System.Environment.NewLine)
        If IsNotBlank(JobTitle) Then result &= Format("TITLE:{0}{1}", JobTitle, System.Environment.NewLine)
        If IsNotBlank(Role) Then result &= Format("ROLE:{0}{1}", Role, System.Environment.NewLine)
        result &= (Emails.ToString())
        result &= (Telephones.ToString())
        result &= (URLs.ToString())
        result &= (Addresses.ToString())
        result &= Format("REV:{0}{1}", LastModified.ToUniversalTime.ToString("yyyyMMdd\THHmmss\Z"), System.Environment.NewLine)
        result &= Format("END:VCARD{0}", System.Environment.NewLine)
        Return result.ToString
    End Function

    Public Function ToQRCode(Optional Size As Integer = 100) As Drawing.Image
        Return HttpUtility.UrlEncode(Me.ToString).ToQRCode(Size).ToImage
    End Function

    Public Function ToFile(FullPath As String) As FileInfo
        Dim writer As New StreamWriter(FullPath & ".vcf", False, Encoding.UTF8)
        writer.WriteLine(ToString)
        writer.Close()
        Return New FileInfo(FullPath)
    End Function

    Public Class vEmails
        ' The first thing to do when building a CollectionBase class is to inherit from System.Collections.CollectionBase
        Inherits System.Collections.CollectionBase

        Public Overloads Function Add(ByVal Value As vEmail) As vEmail
            ' After you inherit the CollectionBase class, you can access an intrinsic object
            ' called InnerList that represents your collection. InnerList is of type ArrayList.
            If Value.Preferred Then
                Dim item As vEmail
                For Each item In Me.InnerList
                    item.Preferred = False
                Next
            End If
            Me.InnerList.Add(Value)
            Return Value
        End Function

        Public Overloads Function Item(ByVal Index As Integer) As vEmail
            ' To retrieve an item from the InnerList, pass the index of that item to the .Item property.
            Return CType(Me.InnerList.Item(Index), vEmail)
        End Function

        Public Overloads Sub Remove(ByVal Index As Integer)
            ' This Remove expects an index.
            Dim cust As vEmail

            cust = CType(Me.InnerList.Item(Index), vEmail)
            If Not cust Is Nothing Then
                Me.InnerList.Remove(cust)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim result = ""
            Dim item As vEmail
            For Each item In Me.InnerList
                result &= Format("{0}", item.ToString)
            Next
            Return result.ToString
        End Function

    End Class

    Public Class vEmail
        Public Preferred As Boolean
        Public EmailAddress As String = ""
        Public Type As String = "INTERNET"

        Public Sub New(ByVal Email As String)
            EmailAddress = Email
        End Sub

        Public Sub New(ByVal Email As String, ByVal IsPreferred As Boolean)
            EmailAddress = Email
            Preferred = IsPreferred
        End Sub

        Public Overrides Function ToString() As String
            Dim result = ""
            result &= ("EMAIL")
            If Preferred Then result &= (";PREF")
            result &= Format(";{0}", Type.ToUpper)
            result &= Format(":{0}{1}", EmailAddress, System.Environment.NewLine)
            Return result.ToString
        End Function

    End Class

    Public Class vURLs
        ' The first thing to do when building a CollectionBase class is to inherit from System.Collections.CollectionBase
        Inherits System.Collections.CollectionBase

        Public Overloads Function Add(ByVal Value As vURL) As vURL
            ' After you inherit the CollectionBase class, you can access an intrinsic object
            ' called InnerList that represents your collection. InnerList is of type ArrayList.
            If Value.Preferred Then
                Dim item As vURL
                For Each item In Me.InnerList
                    Value.Preferred = False
                Next
            End If
            Me.InnerList.Add(Value)
            Return Value
        End Function

        Public Overloads Function Item(ByVal Index As Integer) As vURL
            ' To retrieve an item from the InnerList, pass the index of that item to the .Item property.
            Return CType(Me.InnerList.Item(Index), vURL)
        End Function

        Public Overloads Sub Remove(ByVal Index As Integer)
            ' This Remove expects an index.
            Dim cust As vURL

            cust = CType(Me.InnerList.Item(Index), vURL)
            If Not cust Is Nothing Then
                Me.InnerList.Remove(cust)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim result = ""
            Dim item As vURL
            For Each item In Me.InnerList
                result &= Format("{0}", item.ToString)
            Next
            Return result.ToString
        End Function

    End Class

    Public Class vURL
        Public Preferred As Boolean
        Public URL As String = ""
        Public Location As vLocations = vLocations.WORK       'MS Outlook shows the WORK location on the contact form front page

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

    Public Class vTelephones
        ' The first thing to do when building a CollectionBase class is to inherit from System.Collections.CollectionBase
        Inherits System.Collections.CollectionBase

        Public Overloads Function Add(ByVal Value As vTelephone) As vTelephone
            ' After you inherit the CollectionBase class, you can access an intrinsic object
            ' called InnerList that represents your collection. InnerList is of type ArrayList.
            If Value.Preferred Then
                Dim item As vTelephone
                For Each item In Me.InnerList
                    item.Preferred = False
                Next
            End If
            Me.InnerList.Add(Value)
            Return Value
        End Function

        Public Overloads Function Item(ByVal Index As Integer) As vTelephone
            ' To retrieve an item from the InnerList, pass the index of that item to the .Item property.
            Return CType(Me.InnerList.Item(Index), vTelephone)
        End Function

        Public Overloads Sub Remove(ByVal Index As Integer)
            ' This Remove expects an index.
            Dim cust As vTelephone

            cust = CType(Me.InnerList.Item(Index), vTelephone)
            If Not cust Is Nothing Then
                Me.InnerList.Remove(cust)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim result = ""
            Dim item As vTelephone
            For Each item In Me.InnerList
                result &= Format("{0}", item.ToString)
            Next
            Return result.ToString
        End Function

    End Class

    Public Class vTelephone
        Public Preferred As Boolean
        Public TelephoneNumber As String = ""
        Public Location As vLocations
        Public Type As vPhoneTypes

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
            Dim result = ""
            result &= ("TEL")
            If Preferred Then result &= (";PREF")
            If Not Nothing = (Location) Then result &= Format(";{0}", Location.ToString.ToUpper)
            If Not Nothing = (Type) Then result &= Format(";{0}", Type.ToString.ToUpper)
            result &= Format(":{0}{1}", TelephoneNumber, System.Environment.NewLine)
            Return result.ToString
        End Function

    End Class

    Public Class vAddresss
        ' The first thing to do when building a CollectionBase class is to inherit from System.Collections.CollectionBase
        Inherits System.Collections.CollectionBase

        Public Overloads Function Add(ByVal Value As vAddress) As vAddress
            ' After you inherit the CollectionBase class, you can access an intrinsic object
            ' called InnerList that represents your collection. InnerList is of type ArrayList.
            If Value.Preferred Then
                Dim item As vAddress
                For Each item In Me.InnerList
                    item.Preferred = False
                Next
            End If
            Me.InnerList.Add(Value)
            Return Value
        End Function

        Public Overloads Function Item(ByVal Index As Integer) As vAddress
            ' To retrieve an item from the InnerList, pass the index of that item to the .Item property.
            Return CType(Me.InnerList.Item(Index), vAddress)
        End Function

        Public Overloads Sub Remove(ByVal Index As Integer)
            ' This Remove expects an index.
            Dim cust As vAddress

            cust = CType(Me.InnerList.Item(Index), vAddress)
            If Not cust Is Nothing Then
                Me.InnerList.Remove(cust)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim result = ""
            Dim item As vAddress
            For Each item In Me.InnerList
                result &= Format("{0}", item.ToString)
            Next
            Return result.ToString
        End Function

    End Class

    Public Class vAddress
        Public Preferred As Boolean
        Public AddressName As String = ""    'MS Outlook calls this Office
        Public StreetAddress As String = ""
        Public City As String = ""
        Public State As String = ""
        Public Zip As String = ""
        Public Country As String = ""
        Public AddressLabel As String = ""   'If you don't want to waste time creating this, don't and let the vCard reader format it for you
        Public Location As vLocations  'HOME, WORK, CELL
        Public Type As vAddressTypes    'PARCEL, DOM, INT

        Public Overrides Function ToString() As String
            Dim result = ""

            'Write the Address
            result &= ("ADR")
            If Preferred Then result &= (";PREF")
            If Not Nothing = (Location) Then result &= Format(";{0}", Location.ToString.ToUpper)
            If Not Nothing = (Type) Then result &= Format(";{0}", Type.ToString.ToUpper)
            result &= Format(";ENCODING=QUOTED-PRINTABLE:;{0}", AddressName)
            result &= Format(";{0}", StreetAddress.Replace(System.Environment.NewLine, "=0D=0A"))
            result &= Format(";{0}", City.Replace(System.Environment.NewLine, "=0D=0A"))
            result &= Format(";{0}", State.Replace(System.Environment.NewLine, "=0D=0A"))
            result &= Format(";{0}", Zip.Replace(System.Environment.NewLine, "=0D=0A"))
            result &= Format(";{0}", Country.Replace(System.Environment.NewLine, "=0D=0A"))
            result &= (System.Environment.NewLine)

            'Write the Address label
            If AddressLabel.Length > 0 Then
                result &= ("LABEL")
                If Not Nothing = (Location) Then result &= Format(";{0}", Location.ToString.ToUpper)
                If Not Nothing = (Type) Then result &= Format(";{0}", Type.ToString.ToUpper)
                result &= Format(";ENCODING=QUOTED-PRINTABLE:{0}", AddressLabel.Replace(System.Environment.NewLine, "=0D=0A"))
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

End Class