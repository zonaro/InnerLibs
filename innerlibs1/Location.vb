Imports System.Runtime.CompilerServices
Imports System.Xml
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
    ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP e Google Maps
    ''' </summary>
    ''' <param name="PostalCode"></param>
    ''' <param name="Number">Numero da casa</param>
    Public Sub New(PostalCode As String, Optional Number As Integer = Nothing)
        Me.PostalCode = PostalCode
        If Not Number = Nothing Then Me.Number = Number
        Me.GetInfoByPostalCode()
        Me.SearchOnGoogleMaps(Me.FullAddress)
    End Sub

    ''' <summary>
    ''' Cria um objeto de localização e imediatamente pesquisa as informações de um local através da Latitude e Longitude usando a API do Google Maps
    ''' </summary>
    ''' <param name="Latitude"></param>
    ''' <param name="Longitude"></param>
    Public Sub New(Latitude As String, Longitude As String)
        Me.Latitude = Latitude
        Me.Longitude = Longitude
        Me.SearchOnGoogleMaps(Me.LatitudeLongitude, False)
    End Sub

    ''' <summary>
    ''' Endereco
    ''' </summary>
    ''' <value></value>
    ''' <returns>Endereco</returns>

    Property Address As String
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
    ''' Retorna o endereço completo (logradouro)
    ''' </summary>
    ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
    Function FullAddress() As String
        Dim retorno As String = Address
        If (String.IsNullOrWhiteSpace(Number) = False) Then retorno = retorno & ", " & Number
        If (String.IsNullOrWhiteSpace(Complement) = False) Then retorno = retorno & ", " & Complement
        If (String.IsNullOrWhiteSpace(Neighborhood) = False) Then retorno = retorno & " - " & Neighborhood
        If (String.IsNullOrWhiteSpace(City) = False) Then retorno = retorno & " - " & City
        If (String.IsNullOrWhiteSpace(StateCode) = False) Then
            retorno = retorno & " - " & StateCode
        Else
            If (String.IsNullOrWhiteSpace(State) = False) Then retorno = retorno & " - " & State
        End If
        If (String.IsNullOrWhiteSpace(PostalCode) = False) Then retorno = retorno & " - " & PostalCode
        Return retorno
    End Function

    ''' <summary>
    ''' Retorna uma String contendo as informações do Local
    ''' </summary>
    ''' <returns>string</returns>
    Public Overrides Function ToString() As String
        Return FullAddress()
    End Function

    ''' <summary>
    ''' Retorna as coordenadas geográficas do Local
    ''' </summary>
    ''' <returns>Uma String contendo LATITUDE e LONGITUDE separados por virgula</returns>

    Public Function LatitudeLongitude() As String
        Return Latitude & "," & Longitude
    End Function

    Public Function ToJSON(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
        Return Me.SerializeJSON(DateFormat)
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
        Me.Address = cep("logradouro")
        Me.Country = "Brasil"
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
                Me.Address = documento.SelectSingleNode("/GeocodeResponse/result/address_component[type = 'route']/long_name").InnerText
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
    End Sub

    ''' <summary>
    ''' Realiza uma nova busca no google maps usando o endereço completo
    ''' </summary>
    Public Sub Update()
        Me.SearchOnGoogleMaps(Me.FullAddress)
    End Sub

End Class