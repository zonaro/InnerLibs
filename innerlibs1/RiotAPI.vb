Public Class RiotAPI

    ReadOnly Property URL As String
        Get
            Return "https://br.api.pvp.net/"
        End Get
    End Property

    Property APIKey As String


    Public Sub New(APIKey As String)
        Me.APIKey = APIKey
    End Sub

    Public Function GetChampion(ID As Integer) As Champion
        Return AJAX.GET(Of ChampionList)(URL & "api/lol/br/v1.2/champion/" & ID & "?api_key=" & APIKey).champions(0)
    End Function

    Public Function GetChampionList(Optional FreeToPlay As Boolean = False) As List(Of Champion)
        Return AJAX.GET(Of ChampionList)(URL & "api/lol/br/v1.2/champion?api_key=" & APIKey & "&freetoplay=" & FreeToPlay).champions.ToList
    End Function

    Private Class ChampionList
        Public Property champions As Champion()
    End Class

    Public Class Champion
        Public Property id As Integer
        Public Property active As Boolean
        Public Property botEnabled As Boolean
        Public Property freeToPlay As Boolean
        Public Property botMmEnabled As Boolean
        Public Property rankedPlayEnabled As Boolean
    End Class

End Class





