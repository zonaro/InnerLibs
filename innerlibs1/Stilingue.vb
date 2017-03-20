Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Net
Imports InnerLibs

Namespace Stilingue

    ''' <summary>
    ''' API do Stilingue
    ''' </summary>
    Public NotInheritable Class StilingueAPI


        ''' <summary>
        ''' Inicializa a api do Stilingue e traz todas as keys da conta
        ''' </summary>
        '''<param name="Email">Email do stilingue</param>
        '''<param name="Password">Senha do stilingue</param> 
        Public Sub New(Email As String, Password As String, Optional URL As Endpoint = Endpoint.Stilinguemvp)
            Me.URL = URL
            Me.Credentials = New NetworkCredential(Email, Password)
            UpdateKeys()
        End Sub

        ''' <summary>
        '''Atualiza a lista de keys
        ''' </summary>
        Public Sub UpdateKeys()
            Dim cred As New NameValueCollection()
            cred("email") = Me.Credentials.UserName
            cred("password") = Me.Credentials.Password
            Keys = AJAX.POST(Of Object)(BaseURL & "userkeys/", cred)
        End Sub

        ''' <summary>
        ''' Retorna um WarRoom de uma Key
        ''' </summary>
        ''' <param name="IndexKey">Numro da Key na lista de Keys</param>
        ''' <param name="AutoRequest">Quando TRUE, faz uma requisição das informações de META e NEWS imediatamente ao declarar o objeto</param>
        ''' <param name="URLEndPoint">Qual Url deve ser utilizada</param>
        ''' <returns></returns>
        Public Function GetWarRoom(IndexKey As Integer, Optional AutoRequest As Boolean = True, Optional URLEndPoint As Endpoint = Endpoint.Stilinguemvp) As WarRoom
            Return New WarRoom(Keys(IndexKey), AutoRequest, URLEndPoint)
        End Function

        ''' <summary>
        ''' URLs do Stilingue
        ''' </summary>
        Public Enum Endpoint
            Final
            Stilinguemvp
            Alpha_Stilinguemvp
        End Enum

        ''' <summary>
        ''' Propriedade que indica se qual Endpoint (URL) a API irá trabalhar
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property URL As Endpoint = Endpoint.Stilinguemvp

        Public Property Credentials As New NetworkCredential()
        Public Property Keys As Object

        ''' <summary>
        ''' URL da API do Stilingue
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property BaseURL() As String
            Get
                Select Case URL
                    Case Endpoint.Stilinguemvp
                        Return "http://api.stilinguemvp.appspot.com/api/"
                    Case Endpoint.Alpha_Stilinguemvp
                        Return "http://alpha.stilinguemvp.appspot.com/api/"
                    Case Else
                        Return "http://warroom.stilingue.com.br/api/"
                End Select
            End Get
        End Property



        ''' <summary>
        ''' API de Cards e News do Stilingue
        ''' </summary>
        Public Class WarRoom
            ''' <summary>
            ''' Lista de Erros capturados ao fazer a requisiçao da API
            ''' </summary>
            ''' <returns></returns>
            Public Shared Property Errors As List(Of Exception)
            ''' <summary>
            ''' Declara um novo objeto WarRoom
            ''' </summary>
            ''' <param name="Key">Chave única de acesso da pesquisa disponível no dashboard do War Room</param>
            ''' <param name="AutoRequest">Quando TRUE, faz uma requisição das informações de META e NEWS imediatamente ao declarar o objeto</param>
            ''' <param name="URLEndPoint">Qual Url deve ser utilizada</param>
            Public Sub New(Key As String, Optional AutoRequest As Boolean = False, Optional URLEndPoint As Endpoint = 1)
                Me.Key = Key
                URL = URLEndPoint
                If AutoRequest Then
                    GetMeta()
                    GetNews(Channel.All)
                End If
            End Sub


            ''' <summary>
            ''' Gera um objeto URI com a URL de um Card especifico
            ''' </summary>
            ''' <param name="Card">Tipo do Card</param>
            ''' <returns>URI</returns>
            Public Function GetFrameCardURL(Card As Cards) As Uri
                Dim c As String = ""
                Select Case Card
                    Case 0
                        c = "sonar/"
                    Case 1
                        c = "galeria/"
                    Case 2
                        c = "visaogeral/"
                    Case 3
                        c = "estatisticasgerais/"
                    Case 4
                        c = "matriz/"
                    Case 5
                        c = "metro/"
                    Case 6
                        c = "stream/"
                    Case 7
                        c = "publicacoes/"
                    Case Else
                        Throw New Exception("Card Inválido")
                End Select
                Return New Uri(BaseURL & c & Key)
            End Function
            ''' <summary>
            ''' Lista de Cards do Stilingue
            ''' </summary>
            Public Enum Cards
                ''' <summary>
                ''' Sonar
                ''' </summary>
                Sonar
                ''' <summary>
                ''' Galeria de Mídia
                ''' </summary>
                Gallery
                ''' <summary>
                ''' Visão Geral
                ''' </summary>
                Overview
                ''' <summary>
                ''' Estatísticas Gerais
                ''' </summary>
                Statistics
                ''' <summary>
                ''' Matriz
                ''' </summary>
                Matrix
                ''' <summary>
                ''' Metro
                ''' </summary>
                Metro
                ''' <summary>
                ''' Evolução dos Clusters
                ''' </summary>
                Stream
                ''' <summary>
                ''' Noticias
                ''' </summary>
                Clipping


            End Enum

            Public Property Key As String
            Public Property Api_User_IP As String
            Public Property Api_User As Long
            Public Property Api_Status As String
            Public Property MetaResponse As Meta
            Public Property NewsResponse As News

            ''' <summary>
            ''' Traz as informações dos Metatados da Pesquisa
            ''' </summary>
            Public Sub GetMeta()
                Dim s = AJAX.GET(Of Object)(BaseURL() & "pesquisa/" & Key)
                Api_Status = s("api_status")

                If (Api_Status = "ERROR") Then
                    Return
                End If

                Api_User_IP = s("api_user_ip")
                Api_User = s("api_user")
                Dim m As New Meta
                m.Name = s("response")("name")
                m.Color = ColorTranslator.FromHtml(s("response")("color"))
                m.Total_Posts = s("response")("total_posts")
                m.Total_Classified_Posts = s("response")("total_classified_posts")

                m.Themes = New List(Of String)
                For Each t In s("response")("themes")
                    m.Themes.Add("" + t)
                Next

                m.Groups = New List(Of String)
                For Each g In s("response")("groups")
                    m.Groups.Add("" + g)
                Next


                MetaResponse = m
            End Sub

            ''' <summary>
            ''' Traz os posts de Mídias Sociais e/ou Notícias
            ''' </summary> 
            ''' <param name="Channel">Canal de publicações.</param>
            ''' <param name="Group_Posts">Agrupar Publicações</param>
            ''' <param name="Limit">Limite por página. Maximo de 36.</param>
            ''' <param name="Offset">Pagina</param>
            ''' <param name="Groups">Filtrar grupos especificos  (lista de títulos concatenados por dois pontos. Exemplo: 'Grupo1:Grupo2’)</param>
            ''' <param name="Themes">Filtrar temas especificos  (lista de títulos concatenados por dois pontos. Exemplo: 'Tema1:Tema2’)</param>
            Public Sub GetNews(Channel As Channel, Optional Group_Posts As Boolean = True, Optional Limit As Integer = 0, Optional Offset As Integer = 0, Optional Groups As String() = Nothing, Optional Themes As String() = Nothing)
                Dim Params = ""
                Errors = New List(Of Exception)
                Select Case Channel
                    Case 1
                        Params.Append("&channel=Notícias")
                    Case 2
                        Params.Append("&channel=Mídias Sociais")
                    Case Else
                End Select

                Params.Append("&group_posts=" & Group_Posts.ToString.ToLower)

                If Limit > 36 Then Limit = 36
                If Not Limit = Nothing Then Params.Append("&limit=" & Limit)

                If Not Offset = Nothing Then Params.Append("&offset=" & Offset)

                If Groups Is Nothing Then Groups = New String() {}
                If Themes Is Nothing Then Themes = New String() {}

                If Groups.Length > 0 Then Params.Append("&groups=" & Groups.Join(":"))
                If Themes.Length > 0 Then Params.Append("&themes=" & Themes.Join(":"))



                Dim s = AJAX.GET(Of Object)(BaseURL() & "publicacoes/" & Key + "?&output=json" + Params)
                Api_Status = s("api_status")
                Api_User_IP = s("api_user_ip")
                Api_User = s("api_user")

                Dim n As New News

                n.Limit = s("response")("limit")
                n.Offset = s("response")("offset")
                n.Next_Offset = s("response")("next_offset")

                Dim ps As New List(Of Post)
                For Each post In s("response")("posts")
                    Dim p As New Post
                    Try
                        p.Update_Time_Ago = post("update_time_ago")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.UID = post("uid")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Text = post("text")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.PID = post("pid")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Polarity = post("polarity")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Long_Posted_At = post("long_posted_at")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Order_By = post("order_by")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Spam = post("spam")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Posted_At = post("posted_at")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Comments = post("comments")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Shares = post("shares")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Hot = post("hot")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Followers = post("followers")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Location = post("location")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Channel = post("channel")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Same_Text = post("same_text")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Username = post("username")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Long_Updated_At = post("long_updated_at")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.AAA_Score = post("AAA_score")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Hot_Post = post("hot_post")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Name = post("name")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Gender = post("gender")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Likes = post("likes")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Favorite = post("favorite")
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try

                    Try
                        p.User_Url = New Uri("" & post("user_url"))
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Post_Url = New Uri("" & post("post_url"))
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.Image_Url = New Uri("" & post("image_url"))
                        p.Image = AJAX.GET(Of Image)(p.Image_Url.AbsoluteUri)
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try
                    Try
                        p.User_Image_Url = New Uri("" & post("user_image_url"))
                        p.User_Image = AJAX.GET(Of Image)(p.User_Image_Url.AbsoluteUri)
                    Catch ex As Exception
                        Errors.Add(ex)
                    End Try


                    p.Communities = New List(Of String)
                    For Each com In post("communities")
                        p.Communities.Add("" & com)
                    Next

                    p.Themes = New List(Of String)
                    For Each com In post("themes")
                        p.Themes.Add("" & com)
                    Next

                    p.Groups = New List(Of String)
                    For Each com In post("groups")
                        p.Groups.Add("" & com)
                    Next

                    p.Hashtags = New List(Of String)
                    For Each com In post("hashtags")
                        p.Hashtags.Add("" & com)
                    Next

                    p.Annotation_Comments = New List(Of String)
                    For Each com In post("annotation_comments")
                        p.Annotation_Comments.Add("" & com)
                    Next

                    p.Tags = New List(Of String)
                    For Each com In post("tags")
                        p.Tags.Add("" & com)
                    Next

                    p.Mentions = New List(Of String)
                    For Each com In post("mentions")
                        p.Mentions.Add("" & com)
                    Next

                    ps.Add(p)
                Next
                n.Posts = ps
                Me.NewsResponse = n
            End Sub
            ''' <summary>
            ''' Canal de Pesquisa
            ''' </summary>
            Public Enum Channel
                ''' <summary>
                ''' Todas as Publicações
                ''' </summary>
                All
                ''' <summary>
                ''' Apenas Notícias
                ''' </summary>
                Clipping
                ''' <summary>
                ''' Apenas Mídias Sociais
                ''' </summary>
                SocialMedia
            End Enum

            ''' <summary>
            ''' Metadados da Pesquisa
            ''' </summary>
            Class Meta
                Public Property Name As String
                Public Property Color As Color
                Public Property Total_Posts As Integer
                Public Property Themes As List(Of String)
                Public Property Groups As List(Of String)
                Public Property Total_Classified_Posts As Integer
            End Class

            ''' <summary>
            ''' Noticias e publicações em Redes Sociais
            ''' </summary>
            Class News
                Public Property Limit As Integer
                Public Property Offset As Integer
                Public Property Next_Offset As Integer
                Public Property Posts As List(Of Post)
            End Class

            ''' <summary>
            ''' Noticia ou Post
            ''' </summary>
            Class Post
                Public Property Update_Time_Ago As String
                Public Property UID As String
                Public Property Text As String
                Public Property PID As String
                Public Property Polarity As Integer
                Public Property Long_Posted_At As String
                Public Property Order_By As String
                Public Property Spam As Boolean
                Public Property Posted_At As String
                Public Property Comments As Integer
                Public Property Shares As Integer
                Public Property Hot As Double
                Public Property Followers As Integer
                Public Property Location As String
                Public Property Channel As String
                Public Property Same_Text As Integer
                Public Property Username As String
                Public Property Long_Updated_At As Integer
                Public Property AAA_Score As Double
                Public Property Hot_Post As Boolean
                Public Property Name As String
                Public Property Gender As String
                Public Property Likes As Integer
                Public Property Favorite As Boolean
                Public Property User_Url As Uri
                Public Property Post_Url As Uri
                Public Property Image_Url As Uri
                Public Property User_Image_Url As Uri
                Public Property Communities As List(Of String)
                Public Property Themes As List(Of String)
                Public Property Groups As List(Of String)
                Public Property Hashtags As List(Of String)
                Public Property Annotation_Comments As List(Of String)
                Public Property Tags As List(Of String)
                Public Property Mentions As List(Of String)
                Public Property User_Image As Image
                Public Property Image As Image

            End Class
        End Class


    End Class

End Namespace