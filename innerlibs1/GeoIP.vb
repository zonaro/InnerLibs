Imports InnerLibs

''' <summary>
''' Retorna a localizaçao de um IP
''' </summary>
Public Class GeoIP

    Public ReadOnly Property IP As String
    Public ReadOnly Property CountryCode As String
    Public ReadOnly Property CountryName As String
    Public ReadOnly Property RegionCode As String
    Public ReadOnly Property RegionName As String
    Public ReadOnly Property City As String
    Public ReadOnly Property ZipCode As String
    Public ReadOnly Property TimeZone As String
    Public ReadOnly Property Latitude As String
    Public ReadOnly Property Longitude As String
    Public ReadOnly Property MetroCode As Integer
    ''' <summary>
    ''' Declara uma instancia de GeoIP usando umma string contendo o IP ou URL
    ''' </summary>
    ''' <param name="URLorIP">Url ou IP</param>
    Public Sub New(Optional URLorIP As String = "")
        URLorIP.AdjustWhiteSpaces
        If URLorIP.IsURL Or URLorIP.IsIP Or URLorIP.IsBlank Then
            Try
                If URLorIP.IsURL Then URLorIP = New Uri(URLorIP).GetDomain
                Dim c = AJAX.GET(Of Object)("http://freegeoip.net/json/" & URLorIP)
                Me.IP = c("ip") & ""
                Me.CountryCode = c("country_code") & ""
                Me.CountryName = c("country_name") & ""
                Me.RegionCode = c("region_code") & ""
                Me.RegionName = c("region_name") & ""
                Me.City = c("city") & ""
                Me.ZipCode = c("zip_code") & ""
                Me.TimeZone = c("time_zone") & ""
                Me.Latitude = c("latitude") & ""
                Me.Longitude = c("longitude") & ""
                Me.MetroCode = c("metro_code")
            Catch ex As Exception
                Throw New Exception("Seu limite de requisições estourou ou você não possui conexão com a internet.")
            End Try
        Else Throw New ArgumentException("Sua string não contém uma URL ou IP")
        End If
    End Sub

    ''' <summary>
    ''' Declara uma instancia de GeoIP usando uma URI
    ''' </summary>
    ''' <param name="URL">Url</param>
    Public Sub New(URL As Uri)
        Me.New(URL.GetDomain)
    End Sub

    ''' <summary>
    ''' Cria um objeto Innerlibs.Location com as informaçoes do IP
    ''' </summary>
    ''' <returns></returns>
    Public Function ToLocation() As Location
        Return New Location(Me.Latitude, Me.Longitude)
    End Function

    ''' <summary>
    ''' Retorna uma string JSON do objeto
    ''' </summary>
    ''' <returns></returns>
    Public Function ToJSON(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
        Return Me.SerializeJSON(DateFormat)
    End Function
End Class