Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Globalization
Imports System.Net
Imports System.Reflection
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
        ReadOnly Property Type As String
            Get
                Return AddressTypes.GetAddressType(Me.Street)
            End Get
        End Property

        ''' <summary>
        ''' O nome do endereço
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ReadOnly Property Name As String
            Get
                Return Street.TrimAny(AddressTypes.GetAddressTypeList(Me.Street)).TrimAny(" ")
            End Get
        End Property

        ''' <summary>
        ''' Logradouro
        ''' </summary>
        ''' <returns></returns>
        Property Street As String
            Get
                Return Me("street")
            End Get
            Set(value As String)
                If value.IsNotBlank Then
                    Me("street") = $"{AddressTypes.GetAddressType(value)} {value.TrimAny(True, AddressTypes.GetAddressTypeList(value))}".AdjustBlankSpaces().ToLower().ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
                Else
                    Me("street") = Nothing
                End If
            End Set
        End Property

        ''' <summary>
        ''' Numero da casa, predio etc.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Numero</returns>

        Property Number As String
            Get
                Return Me("number")
            End Get
            Set(value As String)
                Me("number") = value
            End Set
        End Property

        ''' <summary>
        ''' Complemento
        ''' </summary>
        ''' <value></value>
        ''' <returns>Complemento</returns>

        Property Complement As String
            Get
                Return Me("complement")
            End Get
            Set(value As String)
                Me("complement") = value.IfBlank("").TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' Bairro
        ''' </summary>
        ''' <value></value>
        ''' <returns>Bairro</returns>

        Property Neighborhood As String
            Get
                Return Me("Neighborhood")
            End Get
            Set(value As String)
                Me("Neighborhood") = value.IfBlank("").ToLower().ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' CEP - Codigo de Endereçamento Postal
        ''' </summary>
        ''' <value></value>
        ''' <returns>CEP</returns>

        Property PostalCode As String
            Get
                Return Me("PostalCode")
            End Get
            Set(value As String)
                Me("PostalCode") = AddressInfo.FormatPostalCode(value)
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
                PostalCode = value.IfBlank("").TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' Cidade
        ''' </summary>
        ''' <value></value>
        ''' <returns>Cidade</returns>

        Property City As String
            Get
                Return Me("city")
            End Get
            Set(value As String)
                Me("city") = value.IfBlank("").ToLower().ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' Estado
        ''' </summary>
        ''' <value></value>
        ''' <returns>Estado</returns>

        Property State As String
            Get
                Return Me("state")
            End Get
            Set(value As String)
                Me("state") = value.IfBlank("").ToLower.ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' Unidade federativa
        ''' </summary>
        ''' <value></value>
        ''' <returns>Sigla do estado</returns>

        Property StateCode As String
            Get
                Return Me("statecode")
            End Get
            Set(value As String)
                Me("statecode") = value.IfBlank("").ToUpper().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property Region As String
            Get
                Return Me("region")
            End Get
            Set(value As String)
                Me("region") = value.IfBlank("").ToLower.ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property Country As String
            Get
                Return Me("country")
            End Get
            Set(value As String)
                Me("country") = value.IfBlank("").ToLower.ToTitle().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property CountryCode As String
            Get
                Return Me("countrycode")
            End Get
            Set(value As String)
                Me("countrycode") = value.IfBlank("").ToUpper().TrimAny(True, " ", ".", " ", ",", " ", "-", " ").NullIf(Function(x) x.IsBlank())
            End Set
        End Property

        ''' <summary>
        ''' Coordenada geográfica LATITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Latitude</returns>
        Property Latitude As Decimal?
            Get
                Dim value = Me("Latitude")
                If value IsNot Nothing Then
                    Return Convert.ToDecimal(value, New CultureInfo("en-US"))

                End If
                Return Nothing
            End Get
            Set(value As Decimal?)
                If value.HasValue Then
                    Me("Latitude") = Convert.ToString(value.Value, New CultureInfo("en-US"))
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
                Dim value = Me("Longitude")
                If value IsNot Nothing Then
                    Return Convert.ToDecimal(value, New CultureInfo("en-US"))
                End If
                Return Nothing
            End Get
            Set(value As Decimal?)
                If value.HasValue Then
                    Me("Longitude") = Convert.ToString(value, New CultureInfo("en-US"))
                End If
            End Set
        End Property

        Public Function GetPoint() As Point
            If Latitude.HasValue AndAlso Longitude.HasValue Then
                Return New Point((Longitude * 1000000).ToInteger, (Latitude * 1000000).ToInteger)
            End If
            Return New Point
        End Function

        Public Function SetLatitudeLongitudeFromPoint(Point As Point) As AddressInfo
            Me.Longitude = Point.X.ToDecimal() * 0.000001
            Me.Latitude = Point.Y.ToDecimal() * 0.000001
            Return Me
        End Function

        Public Shared Widening Operator CType(AddressInfo As AddressInfo) As Point
            Return AddressInfo?.GetPoint()
        End Operator

        Public Shared Widening Operator CType(Point As Point) As AddressInfo
            Return New AddressInfo().SetLatitudeLongitudeFromPoint(Point)
        End Operator

        ''' <summary>
        ''' Retorna o endereço completo
        ''' </summary>
        ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
        ReadOnly Property FullAddress As String
            Get
                Return ToString(AddressPart.FullAddress)
            End Get
        End Property

        ''' <summary>
        ''' Retorna o Logradouro e numero
        ''' </summary>
        ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
        ReadOnly Property LocationInfo As String
            Get
                Return ToString(AddressPart.LocationInfo)
            End Get
        End Property

        ''' <summary>
        ''' Logradouro, Numero e Complemento
        ''' </summary>
        ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
        ReadOnly Property FullLocationInfo As String
            Get
                Return ToString(AddressPart.FullLocationInfo)
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

            Dim retorno As String = ""

            If Parts <= 0 Then
                Parts = Format
            End If

            If Type.IsNotBlank() AndAlso ContainsPart(Parts, AddressPart.StreetType) Then retorno &= Type
            If Name.IsNotBlank() AndAlso ContainsPart(Parts, AddressPart.StreetName) Then retorno &= " " & Name
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
            retorno = retorno.AdjustBlankSpaces().TrimAny(True, ".", " ", ",", " ", "-", " ")
            Return retorno
        End Function

        Friend Shared Function ContainsPart(Parts As AddressPart, OtherPart As AddressPart) As Boolean
            Return ((Parts) And OtherPart) <> 0
        End Function

        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Shared Function FromViaCEP(PostalCode As Long, Optional Number As String = Nothing, Optional Complement As String = Nothing) As AddressInfo
            Return FromViaCEP(Of AddressInfo)(PostalCode, Number, Complement)
        End Function

        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Shared Function FromViaCEP(PostalCode As String, Optional Number As String = Nothing, Optional Complement As String = Nothing) As AddressInfo
            Return FromViaCEP(Of AddressInfo)(PostalCode, Number, Complement)
        End Function

        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Shared Function FromViaCEP(Of T As AddressInfo)(PostalCode As Long, Optional Number As String = Nothing, Optional Complement As String = Nothing) As T
            Return FromViaCEP(Of T)(PostalCode.ToString.PadLeft(8, "0"c), Number, Complement)
        End Function

        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Shared Function FromViaCEP(Of T As AddressInfo)(PostalCode As String, Optional Number As String = Nothing, Optional Complement As String = Nothing) As T
            Dim d = Activator.CreateInstance(Of T)
            d("original_string") = PostalCode
            d.PostalCode = PostalCode
            If Number.IsNotBlank() Then d.Number = Number
            If Complement.IsNotBlank() Then d.Complement = Complement
            Try
                Dim url = "https://viacep.com.br/ws/" & d.PostalCode.RemoveAny("-") & "/xml/"
                d.Detail("search_url") = url
                Using c = New WebClient()
                    Dim x = New XmlDocument()
                    x.LoadXml(c.DownloadString(url))
                    Dim cep = x("xmlcep")
                    d.Neighborhood = cep("bairro")?.InnerText
                    d.City = cep("localidade")?.InnerText
                    d.StateCode = cep("uf")?.InnerText
                    d.State = Brasil.GetNameOf(d.StateCode)
                    d.Region = Brasil.GetRegionOf(d.StateCode)
                    d.Street = cep("logradouro")?.InnerText
                    Try
                        d("DDD") = cep("ddd")?.InnerText
                    Catch ex As Exception

                    End Try
                    Try
                        d("IBGE") = cep("ibge")?.InnerText
                    Catch ex As Exception

                    End Try
                    Try
                        d("GIA") = cep("gia")?.InnerText
                    Catch ex As Exception

                    End Try
                    Try
                        d("SIAFI") = cep("SIAFI")?.InnerText
                    Catch ex As Exception

                    End Try
                    d.Country = "Brasil"
                    d.CountryCode = "BR"

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
            Return TryParse(Of AddressInfo)(Address)
        End Function

        ''' <summary>
        ''' Tenta extrair as partes de um endereço de uma string
        ''' </summary>
        ''' <param name="Address"></param>
        ''' <returns></returns>
        Public Shared Function TryParse(Of T As AddressInfo)(Address As String) As T

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
                Dim parts = Address.GetAfter(",").SplitAny(" ", ".", ",").ToList()
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
            Dim d = CreateLocation(Of T)(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode)
            d("original_string") = original
            Return d
        End Function

        ''' <summary>
        ''' Retorna um <see cref="AddressInfo"/> usando a api de geocode do Google Maps para completar as informações
        ''' </summary>
        ''' <param name="Address">Endereço para Busca</param>
        ''' <param name="Key">Chave de acesso da API</param>
        ''' <returns></returns>
        Public Shared Function FromGoogleMaps(Address As String, Key As String, Optional NVC As NameValueCollection = Nothing) As AddressInfo
            Return FromGoogleMaps(Of AddressInfo)(Address, Key, NVC)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="AddressInfo"/> usando a api de geocode do Google Maps para completar as informações
        ''' </summary>
        ''' <param name="Address">Endereço para Busca</param>
        ''' <param name="Key">Chave de acesso da API</param>
        ''' <returns></returns>
        Public Shared Function FromGoogleMaps(Of T As AddressInfo)(Address As String, Key As String, Optional NVC As NameValueCollection = Nothing) As T
            Dim d = Activator.CreateInstance(Of T)

            NVC = If(NVC, New NameValueCollection)

            NVC("key") = Key
            NVC("address") = Address

            d("original_string") = Address
            Dim url = $"https://maps.googleapis.com/maps/api/geocode/xml?{NVC.ToQueryString()}"
            d("search_url") = url
            Using c = New WebClient()
                Dim xml = New XmlDocument()
                xml.LoadXml(c.DownloadString(url))
                Dim x = xml("GeocodeResponse")
                Dim status = x("status").InnerText

                d("status") = status

                If status = "OK" Then
                    For Each item As XmlNode In x("result").ChildNodes

                        If item.Name = "geometry" Then
                            Dim cultura = New CultureInfo("en-US")
                            Dim lat = Convert.ToDecimal(item("location")("lat").InnerText, cultura)
                            Dim lng = Convert.ToDecimal(item("location")("lng").InnerText, cultura)
                            d.Latitude = lat
                            d.Longitude = lng
                        End If

                        If item.Name = "place_id" Then
                            d("place_id") = item.InnerText
                        End If

                        If item.Name = "address_component" Then

                            For Each subitem As XmlNode In item.ChildNodes
                                If subitem.Name = "type" Then
                                    d(subitem.InnerText) = item("long_name").InnerText
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
                                            d.Street = item("long_name").InnerText
                                        Case "street_number"
                                            d.Number = item("long_name").InnerText
                                        Case Else
                                    End Select
                                End If

                            Next
                        End If
                    Next
                Else
                    If x("error_message") IsNot Nothing Then
                        d("error_message") = x("error_message").InnerText
                    End If
                End If

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
            Return CreateLocation(Of AddressInfo)(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode)
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
        Public Shared Function CreateLocation(Of T As AddressInfo)(Address As String, Optional Number As String = "", Optional Complement As String = "", Optional Neighborhood As String = "", Optional City As String = "", Optional State As String = "", Optional Country As String = "", Optional PostalCode As String = "") As T
            Dim l = Activator.CreateInstance(Of T)
            l.Street = Address
            l.Neighborhood = Neighborhood
            l.Complement = Complement
            l.Number = Number
            l.City = City
            l.PostalCode = PostalCode

            Dim st = Brasil.GetState(State)
            If st IsNot Nothing Then
                l.State = st.Name
                l.StateCode = st.StateCode
                l.Region = st.Region
                l.Country = "Brasil"
                l.CountryCode = "BR"
            Else
                If State?.Length = 2 Then
                    l.StateCode = State
                Else
                    l.State = State
                End If
                If Country?.Length = 2 Then
                    l.CountryCode = Country
                Else
                    l.Country = Country
                End If
            End If
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
            Return CreateLocation(Address, Number, Complement, Neighborhood, City, State, Country, PostalCode).FullAddress
        End Function

        ''' <summary>
        ''' Retorna as coordenadas geográficas do Local
        ''' </summary>
        ''' <returns>Uma String contendo LATITUDE e LONGITUDE separados por virgula</returns>

        Public Function LatitudeLongitude() As String
            If Latitude.HasValue AndAlso Longitude.HasValue Then
                Return $"{Latitude?.ToString(New CultureInfo("en-US"))}, {Longitude?.ToString(New CultureInfo("en-US"))}"
            End If
            Return Nothing
        End Function

        Public Function ContainsKey(key As String) As Boolean
            Return details.ContainsKey(key.ToLower())
        End Function

        Public Sub Add(key As String, value As String)
            Me(key) = value
        End Sub

        Public Function Remove(key As String) As Boolean
            If Me.ContainsKey(key) Then
                details.Remove(key)
                Return True
            End If
            Return False
        End Function

        Public Sub Clear()
            details.Clear()
        End Sub

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
                _format = value
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
                _globalformat = value
            End Set
        End Property

        Private details As New Dictionary(Of String, String)

        ''' <summary>
        ''' Retona uma informação deste endereço
        ''' </summary>
        ''' <param name="Key"></param>
        ''' <returns></returns>

        Default Public Property Detail(key As String) As String
            Get
                If details Is Nothing Then details = New Dictionary(Of String, String)
                key = key.ToLower()
                If Not details.ContainsKey(key) Then
                    Select Case key
                        Case "geolocation"
                            Return LatitudeLongitude()
                        Case "fulladdress", "tostring", "address"
                            Return FullAddress
                        Case "streetname", "name"
                            Return Name
                        Case "streettype", "type"
                            Return Type
                        Case Else
                    End Select
                End If
                Return details.GetValueOr(key, Nothing)
            End Get
            Set(value As String)
                If value.IsNotBlank Then
                    details(key.ToLower()) = value.TrimAny(" ")
                Else
                    If Me.ContainsKey(key) Then
                        Me.Remove(key)
                    End If
                End If
            End Set
        End Property

        Public Function GetDetails() As Dictionary(Of String, String)
            Return details
        End Function

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
        ''' Endereço completo
        ''' </summary>
        FullAddress = Street + LocationInfo + Neighborhood + CityStateCode + Country + PostalCode

    End Enum

    Public Class AddressTypes

        Public Shared Function GetAddressType(Endereco As String) As String
            Return GetAddressTypeProperty(Endereco)?.Name.IfBlank("")
        End Function

        Public Shared Function GetAddressTypeProperty(Endereco As String) As PropertyInfo
            Dim tp = Endereco.IfBlank("").Split(WordSplitters.ToArray, StringSplitOptions.RemoveEmptyEntries).FirstOr("")
            If tp.IsNotBlank Then
                Dim df = New AddressTypes()
                Return df.GetProperties().FirstOrDefault(Function(x) tp.IsIn(CType(x.GetValue(df), String())) OrElse x.Name = tp)
            End If
            Return Nothing
        End Function

        Public Shared Function GetAddressTypeList(Endereco As String) As String()
            Return If(AddressTypes.GetAddressTypeProperty(Endereco)?.GetValue(New AddressTypes()), New String() {})
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