Imports System.Collections.Specialized
Imports System.Net
Imports System.Text
Imports System.Web.Script.Serialization
Imports InnerLibs

Namespace AllIn

    Public Class ClienteAllIn

        ''' <summary>
        ''' Login da Allin
        ''' </summary>
        ''' <returns></returns>
        Property Login As String
        ''' <summary>
        ''' Senha da Allin
        ''' </summary>
        ''' <returns></returns>
        Property Senha As String


        ''' <summary>
        ''' Cria um objeto para requisiçoes na API da Allin
        ''' </summary>
        ''' <param name="Login">Login</param>
        ''' <param name="Senha">Senha</param>
        Public Sub New(Login As String, Senha As String)
            Me.Login = Login
            Me.Senha = Senha
        End Sub

        ''' <summary>
        ''' Token da API da Allin a partir da url
        ''' </summary>
        ''' <returns></returns>
        Public Function GerarToken(Url As Uri) As String
            Return AJAX.GET(Of Object)(String.Format("{0}?method=get_token&output=json&username={1}&password={2}", Url.AbsoluteUri, Login, Senha))("token")
        End Function


        Public ReadOnly Property UrlTransacional As Uri
            Get
                Return New Uri("https://transacional.allin.com.br/api/")
            End Get
        End Property

        Public ReadOnly Property UrlEmailMarketing As Uri
            Get
                Return New Uri("https://painel02.allinmail.com.br/allinapi/")
            End Get
        End Property


        Public Function Criar(Of Type)() As Type
            Return Activator.CreateInstance(GetType(Type), Me)
        End Function


        Friend Function POST(Of Type)(Url As Uri, Method As String, Obj As NameValueCollection, Optional QueryString As NameValueCollection = Nothing) As Type
            Url = New Uri(Url.AbsoluteUri)
            Url = Url.AddParameter("token", GerarToken(Url))
            Url = Url.AddParameter("method", Method)
            Url = Url.AddParameter("output", "json")
            Url = Url.AddParameter("encode", "UTF8")
            If QueryString IsNot Nothing AndAlso QueryString.HasKeys Then
                For Each p As String In QueryString.Keys
                    Url = Url.AddParameter(p.ToLower, GetAttr(p))
                Next
            End If
            Return AJAX.POST(Of Type)(Url.AbsoluteUri, Obj, Encoding.UTF8)
        End Function



        Public Function EnviarEmailTransacional(nm_envio As String, nm_subject As String, nm_remetente As String, nm_reply As String, nm_email As String, html As String, dt_envio As DateTime) As String

            Dim d As New With {.nm_envio = nm_envio,
                               .nm_email = nm_email,
                               .nm_subject = nm_subject,
                               .nm_remetente = nm_remetente,
                               .nm_reply = nm_reply,
                               .email_remetente = nm_reply,
                               .dt_envio = dt_envio.ToString("yyyy-MM-dd"),
                               .hr_envio = dt_envio.ToString("hh:mm"),
                               .html = Convert.ToBase64String(Encoding.UTF8.GetBytes(html))
                              }

            Dim nvc As New NameValueCollection
            nvc("dados") = d.SerializeJSON

            Dim url As Uri = Me.UrlTransacional


            Return Me.POST(Of String)(url, "enviar_email", nvc)

        End Function



        Public Class Envio

            Friend Sub New(Cliente As ClienteAllIn)
                Me.Cliente = Cliente
            End Sub

            <ScriptIgnore>
            ReadOnly Property Cliente As ClienteAllIn

            <ScriptIgnore>
            Public Property campanha_id As String = ""

            Public Property nm_campanha As String
            Public Property nm_subject As String
            Public Property nm_remetente As String
            Public Property nm_remetente_nome As String
            Public Property nm_reply As String
            Public Property nm_html As String
            Public Property dt_inicio As String
            Public Property nm_lista As String
            Public Property nm_filtro As String
            Public Property nm_categoria As String
            Public Property fl_analytics As Integer = 0
            Public Property nm_txt As Integer = 0



            Sub Enviar()
                Dim dados As New NameValueCollection
                Dim dic = ClassTools.CreateDictionary(Of Envio)(Me)
                If nm_html.IsNotBlank Then
                    dic("nm_html") = Encoding.UTF8.GetString((Convert.FromBase64String(nm_html)))
                End If
                dados("dados_envio") = dic.Select(Function(p) p.Value.IsNotBlank).SerializeJSON

                If Me.campanha_id.IsBlank Then
                    Me.campanha_id = Cliente.POST(Of String)(Cliente.UrlEmailMarketing, "criar_envio", dados)
                Else
                    Dim d As New NameValueCollection
                    d("campanha_id") = campanha_id
                    Cliente.POST(Of String)(Cliente.UrlEmailMarketing, "alterar_envio", dados, d)
                End If

            End Sub


        End Class

    End Class
End Namespace