Imports System.Xml

Namespace Locations

    ''' <summary>
    ''' Representa um deteminado local com suas Informações
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Location

        ''' <summary>
        ''' Cria um novo objeto de localização vazio
        ''' </summary>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP e opcionalmente Google Maps
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Sub New(PostalCode As String, Optional Number As Integer = Nothing, Optional SearchOnMaps As Boolean = False)
            Me.PostalCode = PostalCode
            If Not Number = Nothing Then Me.Number = Number
            Me.GetInfoByPostalCode()
            If SearchOnMaps Then
                Me.Update()
            End If
        End Sub

        ''' <summary>
        ''' Cria um objeto de localização e imediatamente pesquisa as informações de um local através da Latitude e Longitude usando a API do Google Maps
        ''' </summary>
        ''' <param name="Latitude"></param>
        ''' <param name="Longitude"></param>
        Public Sub New(Latitude As String, Longitude As String, Optional Search As Boolean = True)
            Me.Latitude = Latitude
            Me.Longitude = Longitude
            If Search Then
                Me.SearchOnGoogleMaps(Me.LatitudeLongitude, False)
            End If
        End Sub

        ''' <summary>
        ''' Tipo do Endereço
        ''' </summary>
        ''' <returns></returns>
        Property StreetType As String

        ''' <summary>
        ''' O nome do endereço
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>

        Property StreetName As String

        ''' <summary>
        ''' Logradouro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Street As String
            Get
                Return StreetType & " " & StreetName
            End Get
        End Property

        ''' <summary>
        ''' Numero da casa, predio etc.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Numero</returns>

        Property Number As String
        ''' <summary>
        ''' Complemento
        ''' </summary>
        ''' <value></value>
        ''' <returns>Complemento</returns>

        Property Complement As String
        ''' <summary>
        ''' Bairro
        ''' </summary>
        ''' <value></value>
        ''' <returns>Bairro</returns>

        Property Neighborhood As String
        ''' <summary>
        ''' CEP - Codigo de Endereçamento Postal
        ''' </summary>
        ''' <value></value>
        ''' <returns>CEP</returns>

        Property PostalCode As String

        ''' <summary>
        ''' Cidade
        ''' </summary>
        ''' <value></value>
        ''' <returns>Cidade</returns>

        Property City As String
        ''' <summary>
        ''' Estado
        ''' </summary>
        ''' <value></value>
        ''' <returns>Estado</returns>

        Property State As String
        ''' <summary>
        ''' Unidade federativa
        ''' </summary>
        ''' <value></value>
        ''' <returns>Sigla do estado</returns>

        Property StateCode As String

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property Country As String
        ''' <summary>
        ''' Coordenada geográfica LATITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Latitude</returns>

        Property Latitude As String
        ''' <summary>
        ''' Coordenada geográfica LONGITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Longitude</returns>

        Property Longitude As String

        ''' <summary>
        ''' URL do Google Maps
        ''' </summary>
        ''' <returns></returns>
        Property GoogleMapsURL As Uri

        ''' <summary>
        ''' Retorna o endereço completo
        ''' </summary>
        ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
        ReadOnly Property Address As String
            Get
                ParseType()
                Dim retorno As String = Street
                If Number.IsNotBlank Then retorno.Append(", " & Number)
                If Complement.IsNotBlank Then retorno.Append(", " & Complement)
                If Neighborhood.IsNotBlank Then retorno.Append(" - " & Neighborhood)
                If City.IsNotBlank Then retorno.Append(" - " & City)
                If StateCode.IsNotBlank Then
                    retorno.Append(" - " & StateCode)
                Else
                    If State.IsNotBlank Then retorno.Append(" - " & State)
                End If
                If PostalCode.IsNotBlank Then retorno.Append(" - " & PostalCode)
                Return retorno
            End Get
        End Property

        Private Sub ParseType()
            If Me.StreetType.IsBlank() Then
                If Me.StreetName.IsNotBlank Then
                    Me.StreetType = AddressTypes.GetAddressType(Me.StreetName)
                    If Me.StreetType.IsNotBlank Then Me.StreetName.RemoveFirstIf(StreetType)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Retorna uma String contendo as informações do Local
        ''' </summary>
        ''' <returns>string</returns>
        Public Overrides Function ToString() As String
            Return Address()
        End Function

        ''' <summary>
        ''' Retorna as coordenadas geográficas do Local
        ''' </summary>
        ''' <returns>Uma String contendo LATITUDE e LONGITUDE separados por virgula</returns>

        Public Function LatitudeLongitude() As String
            Return Latitude & "," & Longitude
        End Function

        Public Function ToJSON() As String

            Return Json.SerializeJSON(Me)
        End Function

        ''' <summary>
        ''' Retorna o endereço de acordo com o CEP contidos em uma variavel do tipo InnerLibs.Location usando a API https://viacep.com.br/
        ''' </summary>
        Public Sub GetInfoByPostalCode()
            Dim cep As Object = AJAX.GET(Of Object)("https://viacep.com.br/ws/" & Me.PostalCode & "/json/")
            Me.Neighborhood = cep("bairro")
            Me.City = cep("localidade")
            Me.StateCode = cep("uf")
            Me.State = Brasil.GetNameOf(Me.StateCode)
            Me.StreetName = cep("logradouro")
            Me.Country = "Brasil"
            ParseType()
        End Sub

        ''' <summary>
        ''' Realiza uma busca detalhada no google Maps
        ''' </summary>
        ''' <param name="Location">String contendo os detalhes da busca ex.: Av. Rio Pequeno, 240</param>
        ''' <param name="Sensor">Indica se a pesquisa deve ser baseada na sua localização atual. Padrao TRUE</param>

        Public Sub SearchOnGoogleMaps(Location As String, Optional Sensor As Boolean = True)
            Dim documento As New XmlDocument
            Dim searchlocation = Location.AdjustWhiteSpaces.Replace(" ", "+")
            Dim url = "http://maps.googleapis.com/maps/api/geocode/xml?address=" & searchlocation & "&sensor=" & Sensor.ToString
            documento.Load(url)
            If documento.SelectSingleNode("GeocodeResponse").SelectSingleNode("status").InnerText.ToString = "OVER_QUERY_LIMIT" Then
                Throw New Exception("Você estourou o limite de buscas diárias, continue novamente amanhã! (Em caso de IP dinâmico, reinicie sua conexão com a internet e tente novamente.)")
            ElseIf documento.SelectSingleNode("GeocodeResponse").SelectSingleNode("status").InnerText.ToString = "OK" Then
                Try
                    Me.StreetName = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'route']/long_name").InnerText
                Catch ex As Exception

                End Try
                Try
                    Me.Neighborhood = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'neighborhood']/long_name").InnerText
                Catch ex As Exception
                    Try
                        Me.Neighborhood = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'sublocality_level_1']/long_name").InnerText
                    Catch ex2 As Exception
                    End Try
                End Try

                Try
                    Me.City = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'administrative_area_level_2']/long_name").InnerText
                Catch ex As Exception

                End Try
                Try
                    Me.State = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'administrative_area_level_1']/long_name").InnerText
                Catch ex As Exception

                End Try
                Try
                    Me.StateCode = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'administrative_area_level_1']/short_name").InnerText
                Catch ex As Exception

                End Try
                Try
                    Me.Country = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'country']/long_name").InnerText
                Catch ex As Exception

                End Try
                Try
                    Me.Latitude = documento.SelectSingleNode("/GeocodeResponse/result/geometry/location/lat").InnerText
                Catch ex As Exception

                End Try
                Try
                    Me.Longitude = documento.SelectSingleNode("/GeocodeResponse/result/geometry/location/lng").InnerText
                Catch ex As Exception

                End Try

            End If
            Me.GoogleMapsURL = Me.ToGoogleMapsURL(True)
            ParseType()
        End Sub

        ''' <summary>
        ''' Realiza uma nova busca no google maps usando o endereço completo
        ''' </summary>
        Public Sub Update()
            Me.SearchOnGoogleMaps(Me.Address)
        End Sub

    End Class

    Friend Class AddressTypes

        Public Shared Function GetAddressType(Endereco As String) As String
            Dim tp = Endereco.Split(WordSplitters, StringSplitOptions.RemoveEmptyEntries).FirstOr("")
            If tp.IsNotBlank Then
                Return GetType(AddressTypes).GetProperties().FirstOrDefault(Function(x) tp.IsIn(x) OrElse x.Name = tp).Name
            End If
            Return ""
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