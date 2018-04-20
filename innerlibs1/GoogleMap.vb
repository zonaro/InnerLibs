Imports System.Drawing
Imports System.IO
Imports System.Reflection

Namespace GoogleMaps


    Public Class Map

        ''' <summary>
        ''' Marcadores do Mapa
        ''' </summary>
        ''' <returns></returns>
        Public Property Markers As New List(Of Marker)
        ''' <summary>
        ''' Chave de API do Google Maps
        ''' </summary>
        ''' <returns></returns>
        Public Property APIKey As String
        ''' <summary>
        ''' ID do Mapa
        ''' </summary>
        ''' <returns></returns>
        Public Property MAP_ID As String

        ''' <summary>
        ''' Cria um novo objeto de Mapa
        ''' </summary>
        ''' <param name="APIKey">Chave da API do Google</param>
        Public Sub New(APIKey As String)
            Me.APIKey = APIKey

        End Sub

        ''' <summary>
        ''' Cria um mapa e seus respectivos marcadores a partir de varias localizações
        ''' </summary>
        ''' <param name="APIKey">Chave da API do Google</param>
        ''' <param name="Locations">Localizações</param>
        Public Sub New(APIKey As String, ParamArray Locations() As Location)
            Me.APIKey = APIKey
            For Each Location As Location In Locations
                Me.Markers.Add(New Marker(Location))
            Next
        End Sub

        ''' <summary>
        ''' Cria um mapa a partir de um conjunto de marcadores
        ''' </summary>
        ''' <param name="APIKey">Chave da API do Google</param>
        ''' <param name="Markers">Marcadores</param>
        Public Sub New(APIKey As String, ParamArray Markers() As Marker)
            Me.APIKey = APIKey
            For Each Marker As Marker In Markers
                Me.Markers.Add(Marker)
            Next
        End Sub

        ''' <summary>
        ''' Constroi uma div com o Mapa
        ''' </summary>
        ''' <param name="Center">Localização do Centro do Mapa</param>
        ''' <param name="Zoom">Distancia do Mapa</param>
        ''' <param name="Height">Altura da Div do Mapa</param>
        ''' <param name="MAP_ID">ID do MAPA</param>
        ''' <returns>Uma string html contendo as referencias e o Mapa</returns>
        Public Function MakeMap(MAP_ID As String, Optional Zoom As Integer = 11, Optional Height As String = "200px", Optional Center As Location = Nothing) As String
            Dim html = Assembly.GetExecutingAssembly().GetResourceFileText("InnerLibs.GoogleMap.html")
            Dim mk = ""
            Dim contador = 0
            If Center Is Nothing Then
                Center = Markers(0).Location
            End If
            html = html.Replace("[la]", Center.Latitude)
            html = html.Replace("[lo]", Center.Longitude)
            html = html.Replace("[height]", Height)
            For Each Marker As Marker In Me.Markers

                html = html.Replace("YOUR_API_KEY", APIKey)
                html = html.Replace("[zoom]", Zoom)

                mk.Append(MakeMarker(Marker, contador))
                contador.Increment
            Next
            html = html.Replace("[marker_area]", mk)
            html = html.Replace("innermap", "map_" & MAP_ID.ReplaceNone(" "))

            Return html
        End Function

        Private Function MakeMarker(Marker As Marker, Iterator As Integer) As String
            Dim html = "var latlang" & Iterator & " = new google.maps.LatLng(" & Marker.Location.Latitude & "," & Marker.Location.Longitude & ");  " & Environment.NewLine
            html.Append(" var infowindow" & Iterator & " = new google.maps.InfoWindow({" & Environment.NewLine)
            html.Append("content: """ & Marker.Description & " """ & Environment.NewLine)
            html.Append("});" & Environment.NewLine)
            If Marker.Pin.IsNotBlank Then html.Append("var pin" & Iterator & " = '" & Marker.Pin & "'" & Environment.NewLine)
            html.Append("var marker" & Iterator & " = new google.maps.Marker({" & Environment.NewLine)
            html.Append(" position:  latlang" & Iterator & ", " & Environment.NewLine)
            If Marker.Pin.IsNotBlank Then html.Append("  icon:   """ & Marker.Pin & """," & Environment.NewLine)
            html.Append("  title:  """ & Marker.Title & """" & Environment.NewLine)
            html.Append(" });" & Environment.NewLine & Environment.NewLine)
            html.Append(" marker" & Iterator & ".setMap(innermap);" & Environment.NewLine & Environment.NewLine)
            html.Append("marker" & Iterator & ".addListener('click', function() {" & Environment.NewLine)
            html.Append(" infowindow" & Iterator & ".open(innermap, marker" & Iterator & ");" & Environment.NewLine)
            html.Append(" });" & Environment.NewLine)
            Return html
        End Function

        ''' <summary>
        ''' URL da a API do Google Maps
        ''' </summary>
        ''' <returns>uma string contendo a URL da API.</returns>
        Public ReadOnly Property ApiUrl As Uri
            Get
                Return New Uri("https://maps.googleapis.com/maps/api/js?key=" & Me.APIKey)
            End Get
        End Property

        ''' <summary>
        ''' Tag Script contendo a URL da Api no SRC (Adicione no Header ou Body)
        ''' </summary>
        ''' <returns>uma string contendo a tag com a URL da API.</returns>
        Public ReadOnly Property ScriptTag As String
            Get
                Return "<script src=" & ApiUrl.AbsoluteUri.Quote & "></script>"
            End Get
        End Property

        ''' <summary>
        ''' Marcador da localização no mapa
        ''' </summary>
        Public Class Marker

            ''' <summary>
            ''' Cria um marcador
            ''' </summary>
            ''' <param name="Location">'Locaização do Marcador</param>
            ''' <param name="Pin">Imagem do marcador</param>
            Public Sub New(Location As Location, Optional Pin As String = "")
                Me.Location = Location
                Me.Title = Location.Address
                Me.Description = Location.ToString()
                Me.Pin = Pin
            End Sub

            ''' <summary>
            ''' Localizaçao
            ''' </summary>
            ''' <returns></returns>
            Property Location As Location
            ''' <summary>
            ''' Caminho da imagem do Marcador
            ''' </summary>
            ''' <returns></returns>
            Property Pin As String
            ''' <summary>
            ''' Titulo do Marcador
            ''' </summary>
            ''' <returns></returns>
            Property Title As String
            ''' <summary>
            ''' Descrição do marcador (balão)
            ''' </summary>
            ''' <returns></returns>
            Property Description As String
        End Class
    End Class
End Namespace

