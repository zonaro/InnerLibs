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
    ''' Verifica se o texto é um JSON valido
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function IsJson(Of Type)(Text As String, Optional DateFormat As String = "yyyy-MM-dd HH:mm:ss") As Boolean
        Try
            ParseJSON(Of Type)(Text, DateFormat)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

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
    ''' Verifica se uma string é um caminho de arquivo válido
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>TRUE se o caminho for válido</returns>
    <Extension()>
    Public Function IsFilePath(ByVal Text As String) As Boolean
        Text = Text.Trim()
        If Directory.Exists(Text) Then
            Return False
        End If
        If File.Exists(Text) Then
            Return True
        End If
        ' if has extension then its a file; directory otherwise
        Return Not Text.EndsWith(Path.DirectorySeparatorChar) And Path.GetExtension(Text).IsNotBlank
    End Function

    ''' <summary>
    ''' Verifica se uma string é um caminho de diretório válido
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>TRUE se o caminho for válido</returns>
    <Extension()>
    Public Function IsDirectoryPath(ByVal Text As String) As Boolean
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
        ' ends with slash if has extension then its a file; directory otherwise
        Return String.IsNullOrWhiteSpace(Path.GetExtension(Text))
    End Function

    ''' <summary>
    ''' Verifica se uma string é um caminho de diretóio válido
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>TRUE se o caminho for válido</returns>
    <Extension()>
    Function IsPath(Text As String) As Boolean
        Return Text.IsDirectoryPath Or Text.IsFilePath
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
    ''' Valida se a string é um telefone
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function IsTelephone(Text As String) As Boolean
        Return New Regex("\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?", RegexOptions.Singleline + RegexOptions.IgnoreCase).IsMatch(Text.RemoveAny("(", ")"))
    End Function

    ''' <summary>
    ''' Verifica se a aplicação está rodando como administrador
    ''' </summary>
    ''' <returns>TRUE ou FALSE</returns>

    Public Function IsRunningAsAdministrator() As Boolean
        Return New WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)
    End Function

    ''' <summary>
    ''' Verifica se o arquivo está em uso por outro procedimento
    ''' </summary>
    ''' <param name="File">o Arquivo a ser verificado</param>
    ''' <returns>TRUE se o arquivo estiver em uso, FALSE se não estiver</returns>

    <Extension()> Public Function IsInUse(File As FileInfo) As Boolean
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
    Public Function IsNumber(Value As Object) As Boolean
        Try
            Convert.ToDecimal(Value)
            Return Not Value.ToString.IsIP And Not Value.GetType = GetType(DateTime)
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Verifica se o valor não é um numero
    ''' </summary>
    ''' <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
    ''' <returns>FALSE se for um numero, TRUE se não for um numero</returns>
    <Extension()> Public Function IsNotNumber(Value As Object) As Boolean
        Return Not IsNumber(Value)
    End Function

    ''' <summary>
    ''' Verifica se um determinado texto é um email
    ''' </summary>
    ''' <param name="Text">Texto a ser validado</param>
    ''' <returns>TRUE se for um email, FALSE se não for email</returns>

    <Extension()>
    Function IsEmail(ByVal Text As String) As Boolean
        Dim emailExpression As New Regex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])")
        Return Text.IsNotBlank AndAlso emailExpression.IsMatch(Text)
    End Function

    ''' <summary>
    ''' Verifica se um determinado texto é uma URL válida
    ''' </summary>
    ''' <param name="Text">Texto a ser verificado</param>
    ''' <returns>TRUE se for uma URL, FALSE se não for uma URL válida</returns>

    <Extension()>
    Public Function IsURL(Text As String) As Boolean
        Dim u As Uri
        Return Uri.TryCreate(Text, UriKind.Absolute, u)
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
            Return ObjHost.HostName = HostName
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

    <Extension()> Public Function IsDesktop(HttpRequest As HttpRequest) As Boolean
        Return Not HttpRequest.IsMobile()
    End Function

    ''' <summary>
    ''' Verifica se um valor é NULO e prepara a string para uma query TransactSQL
    ''' </summary>
    ''' <param name="Text">        Valor a ser testado</param>
    ''' <param name="DefaultValue">Valor para retornar se o valor testado for Nulo, Vazio ou branco</param>
    ''' <param name="Quotes">
    ''' Indica se o valor testado deve ser retornado entre aspas simples (prepara a string para SQL)
    ''' </param>
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
    ''' Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE
    ''' </summary>
    ''' <typeparam name="T">Tipo da Variavel</typeparam>
    ''' <param name="Value">       Valor</param>
    ''' <param name="ValueIfBlank">Valor se estiver em branco</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IfBlank(Of T)(ByVal Value As Object, ValueIfBlank As T) As T
        If IsNothing(Value) Then
            Return ValueIfBlank
        Else
            Dim blankas As Boolean
            Select Case Value.GetType
                Case GetType(String)
                    blankas = Value.ToString.IsBlank
                Case GetType(Long), GetType(Integer), GetType(Decimal), GetType(Short), GetType(Double)
                    blankas = Value.ToString.IsBlank OrElse Value.ToString.ChangeType(Of Double) = 0
                Case GetType(Date?)
                    blankas = Not CType(Value, Date?).HasValue
                Case GetType(Integer?)
                    blankas = Not CType(Value, Integer?).HasValue
                Case GetType(Double?)
                    blankas = Not CType(Value, Double?).HasValue
                Case GetType(Long?)
                    blankas = Not CType(Value, Long?).HasValue
                Case GetType(Decimal?)
                    blankas = Not CType(Value, Decimal?).HasValue
                Case GetType(Short?)
                    blankas = Not CType(Value, Short?).HasValue
                Case GetType(Money)
                    blankas = CType(Value, Money).Value = 0
                Case GetType(DateTime)
                    blankas = (Value = DateTime.MinValue)
                Case GetType(TimeSpan)
                    blankas = (Value = TimeSpan.MinValue)
                Case Else
                    blankas = (IsNothing(Value))
            End Select
            Return If(blankas, CType(ValueIfBlank, T), CType(Value, T))
        End If
    End Function

    ''' <summary>
    ''' Anula o valor de um objeto se ele for igual a outro objeto
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="TestValue">Outro Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function NullIf(Of T As Class)(ByVal Value As T, TestValue As T) As T
        If Value.Equals(TestValue) Then Return Nothing
        Return Value
    End Function

    ''' <summary>
    ''' Anula o valor de um objeto se ele for igual a outro objeto
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="TestValue">Outro Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function NullIf(Of T As Structure)(ByVal Value As T?, TestValue As T?) As T?
        If Value.HasValue Then
            If Value.Equals(TestValue) Then
                Value = Nothing
            End If
        End If
        Return Value
    End Function

    ''' <summary>
    ''' Anula o valor de um objeto se ele for igual a outro objeto
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="TestValue">Outro Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function NullIf(ByVal Value As String, TestValue As String) As String
        If Value.Equals(TestValue) Then Return Nothing
        Return Value
    End Function

    ''' <summary>
    ''' Verifica se um <see cref="IEnumerable"/> é nulo ou está vazio
    ''' </summary>
    ''' <typeparam name="T">Tipo dos objetos do <see cref="IEnumerable"/></typeparam>
    ''' <param name="Col">Colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function IsEmpty(Of T)(Col As IEnumerable(Of T)) As Boolean
        Return Not IsNothing(Col) AndAlso Col.Count > 0
    End Function

    ''' <summary>
    ''' Verifica se uma String está em branco
    ''' </summary>
    ''' <param name="Text">Uma string</param>
    ''' <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
    <Extension>
    Public Function IsBlank(Text As String) As Boolean
        Return IsNothing(Text) OrElse String.IsNullOrWhiteSpace(Text)
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
    ''' <param name="Value">Valor</param>
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
        Return Value.ChangeType(Of Decimal).IsEven()
    End Function

    ''' <summary>
    ''' Verifica se um numero é par
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsEven(Value As Long) As Boolean
        Return Value.ChangeType(Of Decimal).IsEven()
    End Function

    ''' <summary>
    ''' Verifica se um numero é par
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function IsEven(Value As Double) As Boolean
        Return Value.ChangeType(Of Decimal).IsEven()
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