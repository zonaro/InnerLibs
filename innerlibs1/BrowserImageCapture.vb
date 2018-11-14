Imports System.IO
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing.Imaging

Public Module BrowserClipper

    ''' <summary>
    ''' Cria um snapshot de uma pagina da web a partir de uma URL
    ''' </summary>
    ''' <param name="URL">URL</param>
    Public Function Capture(URL As String, Optional DelaySeconds As Integer = 5) As Image
        Dim Img As Image
        Dim thread As New Thread(Sub()
                                     If URL.IsURL = False Then Throw New Exception("Invalid URL")
                                     Dim web = New WebBrowser()
                                     web.ScrollBarsEnabled = False
                                     web.ScriptErrorsSuppressed = True
                                     web.Navigate(URL)
                                     Img = Capture(web, DelaySeconds)
                                 End Sub)
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        thread.Join()
        Return Img
    End Function

    ''' <summary>
    ''' Cria um snapshot de um componente WebBrowser
    ''' </summary>
    ''' <param name="Browser">WebBrowser</param>
    Public Function Capture(Browser As WebBrowser, Optional DelaySeconds As Integer = 5) As Image
        Dim scrollbars = Browser.ScrollBarsEnabled
        Dim erros = Browser.ScriptErrorsSuppressed
        Browser.ScriptErrorsSuppressed = True
        Dim ss = Browser.Size
        Dim min = Browser.MinimumSize
        While (Browser.ReadyState <> WebBrowserReadyState.Complete)
            Application.DoEvents()
        End While
        System.Threading.Thread.Sleep(1000 * DelaySeconds.SetMinValue(1))
        Dim width = Browser.Document.Body.ScrollRectangle.Width
        Dim height = Browser.Document.Body.ScrollRectangle.Height
        Browser.Width = width
        Browser.Height = height + 20
        Browser.MinimumSize = New Size(width, height)
        Dim bmp = New System.Drawing.Bitmap(width, height)
        Browser.DrawToBitmap(bmp, New System.Drawing.Rectangle(0, 0, width, height))
        Browser.ScrollBarsEnabled = scrollbars
        Browser.ScriptErrorsSuppressed = erros
        Browser.MinimumSize = min
        Browser.Size = ss
        Return bmp
    End Function

    ''' <summary>
    ''' Pega o titulo de uma página da web
    ''' </summary>
    ''' <param name="URL">URL</param>
    ''' <returns></returns>
    Function GetTitle(URL As String) As String
        Dim title As String = ""
        Dim thread As New Thread(Sub()
                                         If URL.IsURL = False Then Throw New Exception("Invalid URL")
                                         Dim web = New WebBrowser()
                                         web.ScrollBarsEnabled = False
                                         web.ScriptErrorsSuppressed = True
                                         web.Navigate(URL)
                                         While (web.ReadyState <> WebBrowserReadyState.Complete)
                                             Application.DoEvents()
                                             Try
                                                 title = web.DocumentTitle
                                                 If title.IsNotBlank Then
                                                     Exit While
                                                 End If
                                             Catch ex2 As Exception
                                             End Try
                                         End While
                                         title = web.DocumentTitle
                                     End Sub)
            thread.SetApartmentState(ApartmentState.STA)
            thread.Start()
        thread.Join()
        Return title
    End Function

    ''' <summary>
    ''' Pega o conteudo HTML de uma página da web logo após seu carregamento
    ''' </summary>
    ''' <param name="URL">URL</param>
    ''' <returns></returns>
    Function GetHtmlContent(URL As String, Optional DelaySeconds As Integer = 0) As String
        Dim content As String = ""
        Dim thread As New Thread(Sub()
                                     If URL.IsURL = False Then Throw New Exception("Invalid URL")
                                     Dim web = New WebBrowser()
                                     web.ScrollBarsEnabled = False
                                     web.ScriptErrorsSuppressed = True
                                     web.Navigate(URL)
                                     While (web.ReadyState <> WebBrowserReadyState.Complete)
                                         Application.DoEvents()
                                         Try
                                             content = web.DocumentText
                                         Catch ex As Exception
                                         End Try
                                     End While
                                     If DelaySeconds > 0 Then
                                         System.Threading.Thread.Sleep(1000 * DelaySeconds.SetMinValue(1))
                                     End If
                                     content = web.DocumentText
                                 End Sub)
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        thread.Join()
        Return content
    End Function
End Module