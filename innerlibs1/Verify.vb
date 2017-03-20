Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Security.Principal
Imports System.Text.RegularExpressions
Imports System.Web

''' <summary>
''' Verifica determinados valores como Arquivos, Numeros e URLs
''' </summary>
''' <remarks></remarks>
Public Module Verify

    ''' <summary>
    ''' Verifica se a string é um CPF válido
    ''' </summary>
    ''' <param name="CPF">CPF</param>
    ''' <returns></returns>
    Public Function IsValidCPF(CPF As String) As Boolean
        CPF = CPF.RemoveAny(".", "-")
        Dim digito As String = [String].Empty
        Dim k As Integer, j As Integer, soma As Integer
        For k = 0 To 1
            soma = 0
            For j = 0 To 9 + (k - 1)
                soma += Integer.Parse(CPF(j).ToString()) * (10 + k - j)
            Next
            digito += If((soma Mod 11 = 0 OrElse soma Mod 11 = 1), 0, (11 - (soma Mod 11)))
        Next
        Return (digito(0) = CPF(9) And digito(1) = CPF(10))
    End Function



    ''' <summary>
    ''' Verifica se uma string é um caminho de diretório válido
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>TRUE se o caminho for válido</returns>
    <Extension()>
    Public Function IsDirectory(ByVal Text As String) As Boolean
        Text = Text.Trim()
        If Directory.Exists(Text) Then
            Return True
        End If
        If File.Exists(Text) Then
            Return False
        End If

        ' if has trailing slash then it's a directory
        If New String() {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}.Any(Function(x) Text.EndsWith(x)) Then
            Return True
        End If
        ' ends with slash
        ' if has extension then its a file; directory otherwise
        Return String.IsNullOrWhiteSpace(Path.GetExtension(Text))
    End Function



    ''' <summary>
    ''' Verifica se uma string é um caminho de diretóio válido
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>TRUE se o caminho for válido</returns>
    <Extension()>
    Function IsPath(Text As String) As Boolean
        Return Text.IsDirectory
    End Function

    ''' <summary>
    ''' Verifica se a string é um endereço IP válido
    ''' </summary>
    ''' <param name="IP">Endereco IP</param>
    ''' <returns>TRUE ou FALSE</returns>
    <Extension>
    Public Function IsIP(IP As String) As Boolean
        Return Regex.IsMatch(IP, "\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$\b")
    End Function
    ''' <summary>
    ''' Verifica se a aplicação está rodando como administrador
    ''' </summary>
    ''' <returns>TRUE ou FALSE</returns>


    Public Function IsRunningAsAdministrator() As Boolean
        ' Get current Windows user
        Dim windowsIdentity1 As WindowsIdentity = WindowsIdentity.GetCurrent()

        ' Get current Windows user principal
        Dim windowsPrincipal As New WindowsPrincipal(windowsIdentity1)

        ' Return TRUE if user is in role "Administrator"
        Return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function



    ''' <summary>
    ''' Verifica se o arquivo está em uso por outro procedimento
    ''' </summary>
    ''' <param name="File">o Arquivo a ser verificado</param>
    ''' <returns>TRUE se o arquivo estiver em uso, FALSE se não estiver</returns>

    Public Function IsInUse(File As FileInfo) As Boolean
        Try
            File.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            Return False
        Catch ex As IOException
            Return True
        End Try
    End Function



    ''' <summary>
    ''' Verifica se o valor é um numero
    ''' </summary>
    ''' <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
    ''' <returns>TRUE se for um numero, FALSE se não for um numero</returns> 
    <Extension()>
    Public Function IsNumber(Value) As Boolean
        Try
            Convert.ToDecimal(Value)
            If Value.ToString.IsIP Then Return False Else Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Verifica se um determinado texto é um email
    ''' </summary>
    ''' <param name="Text">Texto a ser validado</param>
    ''' <returns>TRUE se for um email, FALSE se não for email</returns>

    <Extension()>
    Function IsEmail(ByVal Text As String) As Boolean
        Dim emailExpression As New Regex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])")
        Return emailExpression.IsMatch(Text)
    End Function
    ''' <summary>
    ''' Verifica se um determinado texto é uma URL válida
    ''' </summary>
    ''' <param name="Text">Texto a ser verificado</param>
    ''' <returns>TRUE se for uma URL, FALSE se não for uma URL válida</returns>

    <Extension()>
    Public Function IsURL(Text As String) As Boolean
        Dim urlRx = New Regex("^(http|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$")
        Return urlRx.IsMatch(Text)
    End Function

    ''' <summary>
    ''' Verifica se o dominio é válido (existe) em uma URL ou email
    ''' </summary>
    ''' <param name="DomainOrEmail">Uma String contendo a URL ou email</param>
    ''' <returns>TRUE se o dominio existir, FALSE se o dominio não existir</returns>

    <Extension()>
    Public Function IsValidDomain(DomainOrEmail As String) As Boolean
        Dim ObjHost As System.Net.IPHostEntry
        If DomainOrEmail.IsEmail = True Then
            DomainOrEmail = "Http://" & DomainOrEmail.GetAfter("@")
        End If
        Try
            Dim HostName As String = New Uri(DomainOrEmail).Host
            ObjHost = System.Net.Dns.GetHostEntry(HostName)
            If ObjHost.HostName = HostName Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

    End Function
    ''' <summary>
    ''' Verifica se o User Agent da requisição é um dispositivel móvel (Celulares e Tablets)
    ''' </summary>
    ''' <param name="HttpRequest">Requisição HTTP</param>
    ''' <returns>TRUE para mobile ou FALSE para desktop</returns>
    <Extension()>
    Public Function IsMobile(HttpRequest As HttpRequest) As Boolean
        Return HttpRequest.UserAgent.ToLower.ContainsAny("iphone", "ppc", "windows ce", "blackberry", "opera mini", "mobile", "palm", "portable", "opera mobi", "android", "phone")
    End Function

    ''' <summary>
    ''' Verifica se o User Agent da requisição é um Ipad
    ''' </summary>
    ''' <param name="HttpRequest">Requisição HTTP</param>
    ''' <returns>TRUE para ipad ou FALSE para outro dispositivo</returns>

    <Extension()>
    Public Function IsIpad(HttpRequest As HttpRequest) As Boolean
        Return HttpRequest.UserAgent.ToLower.Contains("ipad")
    End Function

    ''' <summary>
    ''' Verifica se o User Agent da requisição é um Android
    ''' </summary>
    ''' <param name="HttpRequest">Requisição HTTP</param>
    ''' <returns>TRUE para ipad ou FALSE para outro dispositivo</returns>

    <Extension()>
    Public Function IsAndroid(HttpRequest As HttpRequest) As Boolean
        Return HttpRequest.UserAgent.ToLower.Contains("android")
    End Function

    ''' <summary>
    ''' Verifica se o User Agent da requisição é um PC/NOTEBOOK/MAC
    ''' </summary>
    ''' <param name="HttpRequest">Requisição HTTP</param>
    ''' <returns>TRUE para desktops, FALSE para mobile</returns>

    Public Function IsDesktop(HttpRequest As HttpRequest) As Boolean
        Return Not HttpRequest.IsMobile()
    End Function
    ''' <summary>
    ''' Verifica se um valor é NULO e prepara a string para uma query TransactSQL
    ''' </summary>
    ''' <param name="Text">Valor a ser testado</param>
    ''' <param name="DefaultValue">Valor para retornar se o valor testado for Nulo, Vazio ou branco</param>
    ''' <param name="Quotes">Indica se o valor testado deve ser retornado entre aspas simples (prepara a string para SQL)</param>
    ''' <returns>uma String contento o valor ou o valor se Nulo</returns>
    <Extension()>
    Public Function IsNull(Text As String, Optional DefaultValue As String = Nothing, Optional Quotes As Boolean = True) As String
        DefaultValue = If(DefaultValue, "NULL")
        If Not String.IsNullOrWhiteSpace(Text) Then
            Return If(Quotes, Text.FixQuotesToQuery.Quote("'"), Text)
        Else
            Return DefaultValue
        End If
    End Function
    ''' <summary>
    ''' Verifica se uma String está em branco
    ''' </summary>
    ''' <param name="Text">Uma string</param>
    ''' <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
    <Extension>
    Public Function IsBlank(Text As String) As Boolean
        If Text Is Nothing Then
            Return True
        Else
            Return String.IsNullOrWhiteSpace(Text)
        End If
    End Function



    ''' <summary>
    ''' Verifica se uma String não está em branco
    ''' </summary>
    ''' <param name="Text">Uma string</param>
    ''' <returns>FALSE se estivar vazia ou em branco, caso contrario TRUE</returns>
    <Extension>
    Public Function IsNotBlank(Text As String) As Boolean
        Return Not Text.IsBlank()
    End Function

    ''' <summary>
    ''' Verifica se um numero é par
    ''' </summary>
    ''' <param name="Value">Velor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsEven(Value As Decimal) As Boolean
        Return Value Mod 2 = 0
    End Function

    ''' <summary>
    ''' Verifica se um numero é par
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsEven(Value As Integer) As Boolean
        Return Value.To(Of Decimal).IsEven()
    End Function

    ''' <summary>
    ''' Verifica se um numero é par
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsEven(Value As Long) As Boolean
        Return Value.To(Of Decimal).IsEven()
    End Function

    ''' <summary>
    ''' Verifica se um numero é par
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsEven(Value As Double) As Boolean
        Return Value.To(Of Decimal).IsEven()
    End Function

    ''' <summary>
    ''' Verifica se um numero é impar
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsOdd(Value As Decimal) As Boolean
        Return Not Value.IsEven
    End Function
    ''' <summary>
    ''' Verifica se um numero é impar
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsOdd(Value As Integer) As Boolean
        Return Not Value.IsEven
    End Function
    ''' <summary>
    ''' Verifica se um numero é impar
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsOdd(Value As Long) As Boolean
        Return Not Value.IsEven
    End Function
End Module