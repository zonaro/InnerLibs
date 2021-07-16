Imports System.Collections.Specialized
Imports System.Globalization
Imports System.Net
Imports System.Xml

Namespace Locations

    ''' <summary>
    ''' Representa um deteminado local com suas Informações
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AddressInfo

        ''' <summary>
        ''' Cria um novo objeto de localização
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Tipo do Endereço
        ''' </summary>
        ''' <returns></returns>
        Property StreetType As String
            Get
                Return Info.GetValueOr("street_type")
            End Get
            Set(value As String)
                Info("street_type") = value
            End Set
        End Property

        ''' <summary>
        ''' O nome do endereço
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        Property StreetName As String
            Get
                Return Info.GetValueOr("street_name")
            End Get
            Set(value As String)
                Info("street_name") = value
            End Set
        End Property

        ''' <summary>
        ''' Logradouro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Street As String
            Get
                Return ToString(AddressPart.StreetName, AddressPart.StreetType)
            End Get
        End Property

        ''' <summary>
        ''' Numero da casa, predio etc.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Numero</returns>

        Property Number As String
            Get
                Return Info.GetValueOr("street_number")
            End Get
            Set(value As String)
                Info("street_number") = value
            End Set
        End Property

        ''' <summary>
        ''' Complemento
        ''' </summary>
        ''' <value></value>
        ''' <returns>Complemento</returns>

        Property Complement As String
            Get
                Return Info.GetValueOr("subpremise")
            End Get
            Set(value As String)
                Info("subpremise") = value
            End Set
        End Property

        ''' <summary>
        ''' Bairro
        ''' </summary>
        ''' <value></value>
        ''' <returns>Bairro</returns>

        Property Neighborhood As String
            Get
                Return Info.GetValueOr("neighborhood")
            End Get
            Set(value As String)
                Info("neighborhood") = value
            End Set
        End Property

        ''' <summary>
        ''' CEP - Codigo de Endereçamento Postal
        ''' </summary>
        ''' <value></value>
        ''' <returns>CEP</returns>

        Property PostalCode As String
            Get
                Return Info.GetValueOr("postal_code")
            End Get
            Set(value As String)
                Info("postal_code") = AddressInfo.FormatPostalCode(value)
            End Set
        End Property

        ''' <summary>
        ''' Formata uma string de CEP
        ''' </summary>
        ''' <param name="CEP"></param>
        ''' <returns></returns>
        Public Shared Function FormatPostalCode(CEP As String) As String
            CEP = CEP.IfBlank("").Trim()
            If CEP.IsValidCEP Then
                If CEP.IsNumber() Then
                    CEP = CEP.Insert(5, "-")
                End If
                Return CEP
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' CEP - Codigo de Endereçamento Postal. Alias de <see cref="PostalCode"/>
        ''' </summary>
        ''' <value></value>
        ''' <returns>CEP</returns>
        Property ZipCode As String
            Get
                Return PostalCode
            End Get
            Set(value As String)
                PostalCode = value
            End Set
        End Property

        ''' <summary>
        ''' Cidade
        ''' </summary>
        ''' <value></value>
        ''' <returns>Cidade</returns>

        Property City As String
            Get
                Return Info.GetValueOr("city")
            End Get
            Set(value As String)
                Info("city") = value
            End Set
        End Property

        ''' <summary>
        ''' Estado
        ''' </summary>
        ''' <value></value>
        ''' <returns>Estado</returns>

        Property State As String
            Get
                Return Info.GetValueOr("state")
            End Get
            Set(value As String)
                Info("state") = value
            End Set
        End Property

        ''' <summary>
        ''' Unidade federativa
        ''' </summary>
        ''' <value></value>
        ''' <returns>Sigla do estado</returns>

        Property StateCode As String
            Get
                Return Info.GetValueOr("statecode")
            End Get
            Set(value As String)
                Info("statecode") = value
            End Set
        End Property

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property Region As String
            Get
                Return Info.GetValueOr("region")
            End Get
            Set(value As String)
                Info("region") = value
            End Set
        End Property

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property Country As String
            Get
                Return Info.GetValueOr("country")
            End Get
            Set(value As String)
                Info("country") = value
            End Set
        End Property

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property CountryCode As String
            Get
                Return Info.GetValueOr("country_code")
            End Get
            Set(value As String)
                Info("country_code") = value
            End Set
        End Property

        ''' <summary>
        ''' Coordenada geográfica LATITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Latitude</returns>
        Property Latitude As Decimal?
            Get
                Dim value = Info.GetValueOr("lat", Nothing)
                If value IsNot Nothing Then
                    Return Convert.ToDecimal(value, CultureInfo.InvariantCulture)
                End If
                Return Nothing
            End Get
            Set(value As Decimal?)
                If value.HasValue Then
                    Info("lat") = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Coordenada geográfica LONGITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Longitude</returns>
        Property Longitude As Decimal?
            Get
                Dim value = Info.GetValueOr("lng", Nothing)
                If value IsNot Nothing Then
                    Return Convert.ToDecimal(value, CultureInfo.InvariantCulture)
                End If
                Return Nothing
            End Get
            Set(value As Decimal?)
                If value.HasValue Then
                    Info("lng") = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Retorna o endereço completo
        ''' </summary>
        ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
        ReadOnly Property Address As String
            Get
                Return ToString(AddressPart.FullAddress)
            End Get
        End Property

        ''' <summary>
        ''' Retorna uma String contendo as informações do Local
        ''' </summary>
        ''' <returns>string</returns>
        Public Overrides Function ToString() As String
            Return ToString(Format)
        End Function

        ''' <summary>
        ''' Retorna uma string com as partes dos endereço especificas
        ''' </summary>
        ''' <param name="Parts"></param>
        ''' <returns></returns>
        Public Overloads Function ToString(Parts As IEnumerable(Of AddressPart)) As String
            Parts = If(Parts, {})
            If Parts.Any Then
                Dim d As AddressPart = Parts.First()
                For Each p In Parts.Skip(1)
                    d = d Or p
                Next
                Return ToString(d)
            End If
            Return ToString()
        End Function

        ''' <summary>
        ''' Retorna uma string com as partes dos endereço especificas
        ''' </summary>
        ''' <param name="Parts"></param>
        ''' <returns></returns>
        Public Overloads Function ToString(ParamArray Parts As AddressPart()) As String
            Return ToString(Parts.AsEnumerable())
        End Function

        ''' <summary>
        ''' Retorna uma string com as partes dos endereço especificas pelo codigo da formataçao
        ''' </summary>
        ''' <param name="FormatCode"></param>
        ''' <returns></returns>
        Public Overloads Function ToString(FormatCode As Integer) As String
            Return ToString(CType(FormatCode, AddressPart))
        End Function

        ''' <summary>
        ''' Retorna uma string com as partes dos endereço especificas
        ''' </summary>
        ''' <param name="Parts"></param>
        ''' <returns></returns>
        Public Overloads Function ToString(Parts As AddressPart) As String

            ParseType()
            Dim retorno As String = ""

            If Parts <= 0 Then
                Parts = Format
            End If

            If StreetType.IsNotBlank() AndAlso ContainsPart(Parts, AddressPart.StreetType) Then retorno &= StreetType
            If StreetName.IsNotBlank() AndAlso ContainsPart(Parts, AddressPart.StreetName) Then retorno &= " " & StreetName
            If Number.IsNotBlank AndAlso ContainsPart(Parts, AddressPart.Number) Then retorno &= (", " & Number)
            If Complement.IsNotBlank AndAlso ContainsPart(Parts, AddressPart.Complement) Then retorno &= (", " & Complement)
            If Neighborhood.IsNotBlank AndAlso ContainsPart(Parts, AddressPart.Neighborhood) Then retorno &= (" - " & Neighborhood)
            If City.IsNotBlank AndAlso ContainsPart(Parts, AddressPart.City) Then retorno &= (" - " & City)

            If ContainsPart(Parts, AddressPart.StateCode) AndAlso StateCode.IsNotBlank Then
                retorno &= (" - " & StateCode)
            Else
                If ContainsPart(Parts, AddressPart.State) AndAlso State.IsNotBlank Then retorno &= (" - " & State)
            End If

            If PostalCode.IsNotBlank AndAlso ContainsPart(Parts, AddressPart.PostalCode) Then retorno &= (" - " & PostalCode)
            If Country.IsNotBlank AndAlso ContainsPart(Parts, AddressPart.Country) Then retorno &= (" - " & Country)

            If Latitude.HasValue AndAlso ContainsPart(Parts, AddressPart.Latitude) Then retorno &= (" - lat:" & Latitude.ToString())
            If Longitude.HasValue AndAlso ContainsPart(Parts, AddressPart.Longitude) Then retorno &= (" - long:" & Longitude.ToString())

            Return New StructuredText(retorno).ToString().AdjustBlankSpaces().TrimAny(True, ".", " ", ",", " ", "-", " ")
        End Function

        Friend Shared Function ContainsPart(Parts As AddressPart, OtherPart As AddressPart) As Boolean
            Return ((Parts) And OtherPart) <> 0
        End Function

        Friend Sub ParseType()
            If Me.StreetType.IsBlank() Then
                If Me.StreetName.IsNotBlank Then
                    Me.StreetType = AddressTypes.GetAddressType(Me.StreetName)
                    If Me.StreetType.IsNotBlank Then
                        Me.StreetName = New StructuredText(Me.StreetName).ToString.TrimAny(False, AddressTypes.GetAddressTypeList(Me.StreetName)).AdjustBlankSpaces().ToTitle(True).TrimAny(True, " ", ".", " ", ",", " ", "-", " ")
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Shared Function FromViaCEP(PostalCode As String, Optional Number As String = Nothing, Optional Complement As String = Nothing) As AddressInfo
            Dim d = New AddressInfo()
            d.Info("original_string") = PostalCode
            d.PostalCode = PostalCode
            If Number.IsNotBlank() Then d.Number = Number
            If Complement.IsNotBlank() Then d.Complement = Complement
            Try
                Dim url = "https://viacep.com.br/ws/" & d.PostalCode.RemoveAny("-") & "/xml/"
                d.Info("search_url") = url
                Using c = New WebClient()
                    Dim x = New XmlDocument()
                    x.LoadXml(c.DownloadString(url))
                    Dim cep = x("xmlcep")
                    d.Neighborhood = cep("bairro")?.InnerText
                    d.City = cep("localidade")?.InnerText
                    d.StateCode = cep("uf")?.InnerText
                    d.State = Brasil.GetNameOf(d.StateCode)
                    d.Region = Brasil.GetRegionOf(d.StateCode)
                    d.StreetName = cep("logradouro")?.InnerText
                    Try
                        d.Info("DDD") = cep("ddd")?.InnerText
                    Catch ex As Exception

                    End Try
                    Try
                        d.Info("IBGE") = cep("ibge")?.InnerText
                    Catch ex As Exception

                    End Try
                    Try
                        d.Info("GIA") = cep("gia")?.InnerText
                    Catch ex As Exception

                    End Try
                    Try
                        d.Info("SIAFI") = cep("SIAFI")?.InnerText
                    Catch ex As Exception

                    End Try
                    d.Country = "Brasil"
                    d.CountryCode = "BR"
                    d.ParseType()
                End Using
            Catch ex As Exception
            End Try
            Return d
        End Function

        ''' <summary>
        ''' Tenta extrair as partes de um endereço de uma string
        ''' </summary>
        ''' <param name="Address"></param>
        ''' <returns></returns>
        Public Shared Function TryParse(Address As String) As AddressInfo

            Dim original = Address
            Dim PostalCode = "", State = "", City = "", Neighborhood = "", Complement = "", Number = "", Country = ""

            Address = Address.AdjustBlankSpaces() 'arruma os espacos do endereco

            Dim ceps = Address.FindCEP() ' procura ceps no endereco
            Address = Address.RemoveAny(ceps) 'remove ceps

            Address = Address.AdjustBlankSpaces() 'arruma os espacos do endereco

            If ceps.Any() Then
                PostalCode = FormatPostalCode(ceps.First())
            End If

            Address = Address.AdjustBlankSpaces().TrimAny("-", ",", "/") 'arruma os espacos do endereco

            If Address.Contains(" - ") Then

                Dim parts = Address.Split(" - ").ToList()
                If parts.Count() = 1 Then
                    Address = parts.First().IfBlank("").TrimAny(" ", ",", "-")
                End If

                If parts.Count() = 2 Then
                    Address = parts.First().IfBlank("")
                    Dim maybe_neigh = parts.Last().IfBlank("").TrimAny(" ", ",", "-")
                    If maybe_neigh.Length = 2 Then
                        State = maybe_neigh
                    Else
                        Neighborhood = maybe_neigh
                    End If
                End If

                If parts.Count() = 3 Then
                    Address = parts.First().IfBlank("")
                    Dim maybe_city = parts.Last().IfBlank("").TrimAny(" ", ",", "-")
                    If maybe_city.Length = 2 Then
                        State = maybe_city
                    Else
                        City = maybe_city
                    End If

                    parts.RemoveLast()
                    parts = parts.Skip(1).ToList()
                    Neighborhood = parts.FirstOrDefault().IfBlank("").TrimAny(" ", ",", "-")
                End If

                If parts.Count() = 6 Then
                    Dim ad = parts(0) & ", " & parts(1) & " " & parts(2)
                    parts.RemoveAt(1)
                    parts.RemoveAt(2)
                    parts(0) = ad
                End If

                If parts.Count() = 5 Then
                    Dim ad = parts(0) & ", " & parts(1)
                    parts.RemoveAt(1)
                    parts(0) = ad
                End If

                If parts.Count() = 4 Then
                    Address = parts.First().IfBlank("")
                    Dim maybe_state = parts.Last().IfBlank("").TrimAny(" ", ",", "-")
                    parts.RemoveLast()
                    Dim maybe_city = parts.Last().IfBlank("").TrimAny(" ", ",", "-")
                    parts.RemoveLast()
                    City = maybe_city
                    State = maybe_state
                    parts = parts.Skip(1).ToList()
                    Neighborhood = parts.FirstOrDefault().IfBlank("").TrimAny(" ", ",", "-")
                End If
            End If

            Address = Address.AdjustBlankSpaces()

            If (Address.Contains(",")) Then
                Dim parts = Address.GetAfter(",").GetWords().ToList()
                Number = parts.FirstOrDefault(Function(x) x = "s/n" OrElse x = "sn" OrElse x.IsNumber)
                parts.Remove(Number)
                Complement = parts.Join(" ")
                Address = Address.GetBefore(",")
            Else
                Dim adparts = Address.SplitAny(" ", "-").ToList()
                If adparts.Any() Then
                    Dim maybe_number = adparts.FirstOrDefault(Function(x) x = "s/n" OrElse x = "sn" OrElse x.IsNumber).IfBlank("").TrimAny(" ", ",")
                    Complement = adparts.Join(" ").GetAfter(maybe_number).TrimAny(" ", ",")
                    Number = maybe_number
                    Address = adparts.Join(" ").GetBefore(maybe_number).TrimAny(" ", ",")
                End If
            End If

            Number = Number.AdjustBlankSpaces().TrimAny(" ", ",", "-")
            Complement = Complement.AdjustBlankSpaces().TrimAny(" ", ",", "-")
            Dim d = CreateLocation(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode)
            d.Info("original_string") = original
            Return d
        End Function

        ''' <summary>
        ''' Retorna um <see cref="AddressInfo"/> usando a api de geocode do Google Maps para completar as informações
        ''' </summary>
        ''' <param name="Address">Endereço para Busca</param>
        ''' <param name="Key">Chave de acesso da API</param>
        ''' <returns></returns>
        Public Shared Function FromGoogleMaps(Address As String, Key As String, Optional NVC As NameValueCollection = Nothing) As AddressInfo
            Dim d = New AddressInfo()

            NVC = If(NVC, New NameValueCollection)

            NVC("key") = Key
            NVC("address") = Address

            d.Info("original_string") = Address
            Dim url = $"https://maps.googleapis.com/maps/api/geocode/xml?{NVC.ToQueryString()}"
            d.Info("search_url") = url
            Using c = New WebClient()
                Dim xml = New XmlDocument()
                xml.LoadXml(c.DownloadString(url))
                Dim x = xml("GeocodeResponse")
                Dim status = x("status").InnerText

                If status = "OK" Then

                    For Each item As XmlNode In x("result").ChildNodes

                        If item.Name = "geometry" Then
                            d.Latitude = Convert.ToDecimal(item("location")("lat").InnerText, CultureInfo.InvariantCulture)
                            d.Longitude = Convert.ToDecimal(item("location")("lng").InnerText, CultureInfo.InvariantCulture)
                        End If

                        If item.Name = "place_id" Then
                            d.Info("place_id") = item.InnerText
                        End If

                        If item.Name = "address_component" Then

                            For Each subitem As XmlNode In item.ChildNodes
                                If subitem.Name = "type" Then
                                    d.Info(subitem.InnerText) = item("long_name").InnerText
                                    Select Case subitem.InnerText
                                        Case "postal_code"
                                            d.PostalCode = item("long_name").InnerText
                                        Case "country"
                                            d.Country = item("long_name").InnerText
                                            d.CountryCode = item("short_name").InnerText
                                        Case "administrative_area_level_1"
                                            d.State = item("long_name").InnerText
                                            d.StateCode = item("short_name").InnerText
                                            d.Region = Brasil.GetRegionOf(d.StateCode)
                                        Case "administrative_area_level_2"
                                            d.City = item("long_name").InnerText
                                        Case "administrative_area_level_3", "locality", "sublocality"
                                            d.Neighborhood = d.Neighborhood.IfBlank(item("long_name").InnerText)
                                        Case "route"
                                            d.StreetName = item("long_name").InnerText
                                        Case "street_number"
                                            d.Number = item("long_name").InnerText
                                        Case Else
                                    End Select
                                End If

                            Next
                        End If
                    Next
                Else
                    Throw New Exception($"{status} - {x("error_message").InnerText}")
                End If
                d.ParseType()
            End Using

            Return d
        End Function

        ''' <summary>
        ''' Cria uma localização a partir de partes de endereço
        ''' </summary>
        ''' <param name="Address"></param>
        ''' <param name="Number"></param>
        ''' <param name="Complement"></param>
        ''' <param name="Neighborhood"></param>
        ''' <param name="City"></param>
        ''' <param name="State"></param>
        ''' <param name="Country"></param>
        ''' <param name="PostalCode"></param>
        ''' <returns></returns>
        Public Shared Function CreateLocation(Address As String, Optional Number As String = "", Optional Complement As String = "", Optional Neighborhood As String = "", Optional City As String = "", Optional State As String = "", Optional Country As String = "", Optional PostalCode As String = "") As AddressInfo
            Dim l = New AddressInfo()

            Address = Address.AdjustBlankSpaces()
            Number = Number.AdjustBlankSpaces()
            Complement = Complement.AdjustBlankSpaces()
            Neighborhood = Neighborhood.AdjustBlankSpaces()
            City = City.AdjustBlankSpaces()
            State = State.AdjustBlankSpaces()
            Country = Country.AdjustBlankSpaces()
            PostalCode = PostalCode.AdjustBlankSpaces()

            l.StreetName = Address.ToLower().ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            l.Neighborhood = Neighborhood.AdjustBlankSpaces().ToLower().ToTitle().NullIf(Function(x) x.IsBlank())
            l.Complement = Complement.AdjustBlankSpaces().ToLower().ToTitle().NullIf(Function(x) x.IsBlank())

            l.Number = Number.NullIf(Function(x) x.IsBlank())
            l.City = City.ToLower().ToTitle().NullIf(Function(x) x.IsBlank())

            Dim st = Brasil.GetState(State)
            If st IsNot Nothing Then
                l.State = st.Name
                l.StateCode = st.StateCode
                l.Region = st.Region
                Country = "Brasil"
                l.CountryCode = "BR"
            Else
                If State.Length = 2 Then
                    l.StateCode = State.AdjustBlankSpaces().ToUpper().NullIf(Function(x) x.IsBlank())
                Else
                    l.State = State.ToLower().ToTitle().NullIf(Function(x) x.IsBlank())
                End If

                If Country.Length = 2 Then
                    l.CountryCode = Country.ToUpper().NullIf(Function(x) x.IsBlank())
                Else
                    l.Country = Country.ToLower().ToTitle().NullIf(Function(x) x.IsBlank())
                End If
            End If

            l.PostalCode = PostalCode.NullIf(Function(x) x.IsBlank())
            l.ParseType()

            Return l
        End Function

        ''' <summary>
        ''' Retorna uma string de endereço a partir de varias partes de endereco
        ''' </summary>
        ''' <param name="Address"></param>
        ''' <param name="Number"></param>
        ''' <param name="Complement"></param>
        ''' <param name="Neighborhood"></param>
        ''' <param name="City"></param>
        ''' <param name="State"></param>
        ''' <param name="Country"></param>
        ''' <param name="PostalCode"></param>
        ''' <returns></returns>
        Public Shared Function FormatAddress(Address As String, Optional Number As String = "", Optional Complement As String = "", Optional Neighborhood As String = "", Optional City As String = "", Optional State As String = "", Optional Country As String = "", Optional PostalCode As String = "") As String
            Return CreateLocation(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode).Address
        End Function

        ''' <summary>
        ''' Retorna as coordenadas geográficas do Local
        ''' </summary>
        ''' <returns>Uma String contendo LATITUDE e LONGITUDE separados por virgula</returns>

        Public Function LatitudeLongitude() As String
            Return Latitude & "," & Longitude
        End Function

        ''' <summary>
        ''' Formato desta instancia de <see cref="AddressInfo"/> quando chamada pelo <see cref="AddressInfo.ToString()"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Format As AddressPart
            Get
                If _format = AddressPart.Default Then
                    Return GlobalFormat
                End If
                Return _format
            End Get
            Set(value As AddressPart)
                If value = AddressPart.Default Then
                    _format = GlobalFormat
                Else
                    _format = value
                End If
            End Set
        End Property

        Private _format As AddressPart = AddressPart.Default
        Private Shared _globalformat As AddressPart = AddressPart.FullAddress

        ''' <summary>
        ''' Formato global de todas as intancias de <see cref="AddressInfo"/> quando chamadas pelo <see cref="AddressInfo.ToString()"/>
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property GlobalFormat As AddressPart
            Get
                If _globalformat = AddressPart.Default Then
                    Return AddressPart.FullAddress
                End If
                Return _globalformat
            End Get
            Set(value As AddressPart)
                If value = AddressPart.Default Then
                    _globalformat = AddressPart.FullAddress
                Else
                    _globalformat = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Todas as Informações relacionadas a este endereço
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Info As New Dictionary(Of String, String)

        ''' <summary>
        ''' Retona uma informação deste endereço
        ''' </summary>
        ''' <param name="Key"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetInfo(Key As String) As String
            Get
                Return Info.GetValueOr(Key, Nothing)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Partes de um Endereço
    ''' </summary>
    <Flags>
    Public Enum AddressPart

        ''' <summary>
        ''' Formato default definido pela propriedade <see cref="AddressInfo.Format"/> ou <see cref="AddressInfo.GlobalFormat"/>
        ''' </summary>
        [Default] = 0

        ''' <summary>
        ''' Tipo do Lograoduro
        ''' </summary>
        StreetType = 1

        ''' <summary>
        ''' Nome do Logradouro
        ''' </summary>
        StreetName = 2

        ''' <summary>
        ''' Logradouro
        ''' </summary>
        Street = StreetType + StreetName

        ''' <summary>
        ''' Numero do local
        ''' </summary>
        Number = 4

        ''' <summary>
        ''' Complemento do local
        ''' </summary>
        Complement = 8

        ''' <summary>
        ''' Numero e complemento
        ''' </summary>
        LocationInfo = Number + Complement

        ''' <summary>
        ''' Logradouro, Numero e complemento
        ''' </summary>
        FullLocationInfo = Street + Number + Complement

        ''' <summary>
        ''' Bairro
        ''' </summary>
        Neighborhood = 16

        ''' <summary>
        ''' Cidade
        ''' </summary>
        City = 32

        ''' <summary>
        ''' Estado
        ''' </summary>
        State = 64

        ''' <summary>
        ''' Cidade e Estado
        ''' </summary>
        CityState = City + State

        ''' <summary>
        ''' UF
        ''' </summary>
        StateCode = 128

        ''' <summary>
        ''' Cidade e UF
        ''' </summary>
        CityStateCode = City + StateCode

        ''' <summary>
        ''' País
        ''' </summary>
        Country = 256

        ''' <summary>
        ''' CEP
        ''' </summary>
        PostalCode = 512

        ''' <summary>
        ''' Latitude
        ''' </summary>
        Latitude = 1024

        ''' <summary>
        ''' Longitude
        ''' </summary>
        Longitude = 2048

        ''' <summary>
        ''' Endereço completo
        ''' </summary>
        FullAddress = Street + LocationInfo + Neighborhood + CityStateCode + Country + PostalCode

        ''' <summary>
        ''' Latitude e Longitude
        ''' </summary>
        LatitudeLongitude = Latitude + Longitude

    End Enum

    Public Class AddressTypes

        Public Shared Function GetAddressType(Endereco As String) As String
            Dim tp = Endereco.Split(WordSplitters.ToArray, StringSplitOptions.RemoveEmptyEntries).FirstOr("")
            If tp.IsNotBlank Then
                Dim df = New AddressTypes()
                Return df.GetProperties().FirstOrDefault(Function(x) tp.IsIn(CType(x.GetValue(df), String())) OrElse x.Name = tp)?.Name.IfBlank("")
            End If
            Return ""
        End Function

        Public Shared Function GetAddressTypeList(Endereco As String) As String()
            Dim tp = Endereco.Split(WordSplitters.ToArray, StringSplitOptions.RemoveEmptyEntries).FirstOr("")
            If tp.IsNotBlank Then
                Dim df = New AddressTypes()
                Return df.GetProperties().FirstOrDefault(Function(x) tp.IsIn(CType(x.GetValue(df), String())) OrElse x.Name = tp)?.GetValue(df)
            End If
            Return {}
        End Function

        Public ReadOnly Property Aeroporto As String() = {"Aeroporto", "Ar", "Aero"}
        Public ReadOnly Property Alameda As String() = {"Alameda", "Al", "Alm"}
        Public ReadOnly Property Área As String() = {"Área", "Area"}
        Public ReadOnly Property Avenida As String() = {"Avenida", "Av", "Avn"}
        Public ReadOnly Property Campo As String() = {"Cam", "Camp", "Campo"}
        Public ReadOnly Property Chácara As String() = {"Cha", "Chac", "Chacara"}
        Public ReadOnly Property Colônia As String() = {"Col", "Colonia"}
        Public ReadOnly Property Condomínio As String() = {"Condominio", "Cond"}
        Public ReadOnly Property Comunidade As String() = {"Com", "Comunidade"}
        Public ReadOnly Property Conjunto As String() = {"Conjunto", "Con"}
        Public ReadOnly Property Distrito As String() = {"Dis", "Dst", "Distrito"}
        Public ReadOnly Property Esplanada As String() = {"Esp", "Esplanada"}
        Public ReadOnly Property Estação As String() = {"Estacao", "Est", "st"}
        Public ReadOnly Property Estrada As String() = {"Et", "Es", "Estrada"}
        Public ReadOnly Property Favela As String() = {"Fav", "Favela"}
        Public ReadOnly Property Feira As String() = {"Fei", "Fr", "Feira"}
        Public ReadOnly Property Jardim As String() = {"Jd", "Jardim", "Jar"}
        Public ReadOnly Property Ladeira As String() = {"Lad", "Ld", "Ladeira"}
        Public ReadOnly Property Lago As String() = {"Lago", "Lg"}
        Public ReadOnly Property Lagoa As String() = {"Lagoa", "Lga"}
        Public ReadOnly Property Largo As String() = {"Lrg", "Largo", "Lgo"}
        Public ReadOnly Property Loteamento As String() = {"Lote", "Loteamento", "Lt"}
        Public ReadOnly Property Morro As String() = {"Mr", "Mrr", "Morro", "Mor"}
        Public ReadOnly Property Núcleo As String() = {"Nc", "Nucleo", "Nuc"}
        Public ReadOnly Property Parque As String() = {"Parque", "Pq", "Pk", "Par", "Parq", "Park"}
        Public ReadOnly Property Passarela As String() = {"Pass", "Pas", "Pa", "Passarela"}
        Public ReadOnly Property Pátio As String() = {"Pt", "Pat", "Pateo", "Patio"}
        Public ReadOnly Property Praça As String() = {"Praça", "Pc", "Praca", "Pç"}
        Public ReadOnly Property Quadra As String() = {"Qd", "Quadra", "Quad"}
        Public ReadOnly Property Recanto As String() = {"Rec", "Recanto", "Rc"}
        Public ReadOnly Property Residencial As String() = {"Residencial", "Residencia", "Res", "Resid"}
        Public ReadOnly Property Rodovia As String() = {"Rodovia", "Rod"}
        Public ReadOnly Property Rua As String() = {"Rua", "R"}
        Public ReadOnly Property Setor As String() = {"Setor", "Str"}
        Public ReadOnly Property Sítio As String() = {"Sitio", "Sit"}
        Public ReadOnly Property Travessa As String() = {"Trv", "Travessa", "Tvs"}
        Public ReadOnly Property Trecho As String() = {"Trc", "Trecho"}
        Public ReadOnly Property Trevo As String() = {"Trevo"}
        Public ReadOnly Property Vale As String() = {"Vale", "Val"}
        Public ReadOnly Property Vereda As String() = {"Vereda"}
        Public ReadOnly Property Via As String() = {"Via"}
        Public ReadOnly Property Viaduto As String() = {"Vd", "Viaduto"}
        Public ReadOnly Property Viela As String() = {"Viela"}
        Public ReadOnly Property Vila As String() = {"Vila", "Vl"}
    End Class

End Namespace