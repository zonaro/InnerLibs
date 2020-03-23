Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Modulo de Criptografia
''' </summary>
''' <remarks></remarks>
Public Module Encryption

    ''' <summary>
    ''' Criptografa um Texto em MD5
    ''' </summary>
    ''' <param name="Text">Texto a ser Criptografado</param>
    ''' <returns>Uma String MD5</returns>

    <Extension()>
    Public Function ToMD5String(Text As String) As String
        If (Text.IsNotBlank()) Then
            Dim md5 = System.Security.Cryptography.MD5.Create()
            Dim inputBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(Text)
            Dim hash As Byte() = md5.ComputeHash(inputBytes)
            Dim sb = New StringBuilder()
            For i As Integer = 0 To hash.Length - 1
                sb.Append(hash(i).ToString("X2"))
            Next
            Return sb.ToString()
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Tenta reverter uma string MD5 para seu valor original
    ''' </summary>
    ''' <param name="Text">String MD5</param>
    ''' <returns></returns>
    <Extension()>
    Public Function TryReverseMD5(Text As String) As String
        Return AJAX.Request(Of String)("http://md5.gromweb.com/query/" & Text)
    End Function

    ''' <summary>
    ''' Criptografa uma string
    ''' </summary>
    ''' <param name="Text">Texto descriptografado</param>
    ''' <returns></returns>
    <Extension> Public Function Encrypt(Text As String, Optional Key As String = Nothing) As String
        If Text.IsNotBlank Then

            Dim Results As Byte()
            Dim UTF8 As New System.Text.UTF8Encoding()
            Dim HashProvider As New MD5CryptoServiceProvider()
            Dim TDESKey As Byte() = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("12345")))
            Dim TDESAlgorithm As New TripleDESCryptoServiceProvider()
            TDESAlgorithm.Key = TDESKey
            TDESAlgorithm.Mode = CipherMode.ECB
            TDESAlgorithm.Padding = PaddingMode.PKCS7
            Dim DataToEncrypt As Byte() = UTF8.GetBytes(Text)
            Try
                Dim Encryptor As ICryptoTransform = TDESAlgorithm.CreateEncryptor()
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length)
            Finally
                TDESAlgorithm.Clear()
                HashProvider.Clear()
            End Try
            Return Convert.ToBase64String(Results)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Descriptografa uma string
    ''' </summary>
    ''' <param name="Text">Texto Criptografado</param>
    ''' <returns></returns>
    <Extension> Public Function Decrypt(Text As String, Optional Key As String = Nothing) As String
        If Text.IsNotBlank Then

            Dim Results As Byte()
            Dim UTF8 As New System.Text.UTF8Encoding()
            Dim HashProvider As New MD5CryptoServiceProvider()
            Dim TDESKey As Byte() = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("12345")))
            Dim TDESAlgorithm As New TripleDESCryptoServiceProvider()
            TDESAlgorithm.Key = TDESKey
            TDESAlgorithm.Mode = CipherMode.ECB
            TDESAlgorithm.Padding = PaddingMode.PKCS7
            Dim DataToDecrypt As Byte() = Convert.FromBase64String(Text)
            Try
                Dim Decryptor As ICryptoTransform = TDESAlgorithm.CreateDecryptor()
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length)
            Finally
                TDESAlgorithm.Clear()
                HashProvider.Clear()
            End Try
            Return UTF8.GetString(Results)
        End If
        Return Text
    End Function

    ''' <summary>
    ''' Criptografa uma string
    ''' </summary>
    ''' <param name="Text">Texto descriptografado</param>
    ''' <returns></returns>
    <Extension()>
    Public Function Encrypt(ByVal text As String, Key As String, IV As String) As String
        If text.IsNotBlank() Then
            Dim aes As AesCryptoServiceProvider = New AesCryptoServiceProvider()
            aes.BlockSize = 128
            aes.KeySize = 256
            aes.IV = Encoding.UTF8.GetBytes(IV)
            aes.Key = Encoding.UTF8.GetBytes(Key)
            aes.Mode = CipherMode.CBC
            aes.Padding = PaddingMode.PKCS7
            Dim src As Byte() = Encoding.UTF8.GetBytes(text)

            Using eencrypt As ICryptoTransform = aes.CreateEncryptor()
                Dim dest As Byte() = eencrypt.TransformFinalBlock(src, 0, src.Length)
                Return Convert.ToBase64String(dest)
            End Using
        End If
        Return text
    End Function

    ''' <summary>
    ''' Descriptografa uma string
    ''' </summary>
    ''' <param name="Text">Texto Criptografado</param>
    ''' <returns></returns>
    <Extension()>
    Public Function Decrypt(ByVal text As String, Key As String, IV As String) As String
        If text.IsNotBlank Then
            Dim aes As AesCryptoServiceProvider = New AesCryptoServiceProvider()
            aes.BlockSize = 128
            aes.KeySize = 256
            aes.IV = Encoding.UTF8.GetBytes(IV)
            aes.Key = Encoding.UTF8.GetBytes(Key)
            aes.Mode = CipherMode.CBC
            aes.Padding = PaddingMode.PKCS7
            Dim src As Byte()
            Try
                src = System.Convert.FromBase64String(text.FixBase64())
            Catch
                src = System.Convert.FromBase64String(text)

            End Try

            Using ddecrypt As ICryptoTransform = aes.CreateDecryptor()
                Try
                    Dim dest As Byte() = ddecrypt.TransformFinalBlock(src, 0, src.Length)
                    Return Encoding.UTF8.GetString(dest)
                Catch ex As Exception
                End Try
            End Using
        End If
        Return text
    End Function

End Module