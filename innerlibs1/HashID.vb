
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions


''' <summary>
''' Generate YouTube-like hashes from one or many numbers. Use hashids when you do not want to expose your database ids to the user.
''' </summary>
Public Class Hashids
        Public Const DEFAULT_ALPHABET As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
        Public Const DEFAULT_SEPS As String = "cfhistuCFHISTU"

        Private Const MIN_ALPHABET_LENGTH As Integer = 16
        Private Const SEP_DIV As Double = 3.5
        Private Const GUARD_DIV As Double = 12.0

        Private alphabet As String
        Private salt As String
        Private seps As String
        Private guards As String
        Private minHashLength As Integer

        Private guardsRegex As Regex
        Private sepsRegex As Regex

        '  Creates the Regex in the first usage, speed up first use of non hex methods
#If CORE Then
		Private Shared hexValidator As New Lazy(Of Regex)(Function() New Regex("^[0-9a-fA-F]+$"))
		Private Shared hexSplitter As New Lazy(Of Regex)(Function() New Regex("[\w\W]{1,12}"))
#Else
        Private Shared hexValidator As New Lazy(Of Regex)(Function() New Regex("^[0-9a-fA-F]+$", RegexOptions.Compiled))
        Private Shared hexSplitter As New Lazy(Of Regex)(Function() New Regex("[\w\W]{1,12}", RegexOptions.Compiled))
#End If

        ''' <summary>
        ''' Instantiates a new Hashids with the default setup.
        ''' </summary>
        Public Sub New()
            Me.New(String.Empty, 0, DEFAULT_ALPHABET, DEFAULT_SEPS)
        End Sub

        ''' <summary>
        ''' Instantiates a new Hashids en/de-coder.
        ''' </summary>
        ''' <param name="salt"></param>
        ''' <param name="minHashLength"></param>
        ''' <param name="alphabet"></param>
        Public Sub New(Optional salt As String = "", Optional minHashLength As Integer = 0, Optional alphabet As String = DEFAULT_ALPHABET, Optional seps As String = DEFAULT_SEPS)
            If String.IsNullOrWhiteSpace(alphabet) Then
                Throw New ArgumentNullException("alphabet")
            End If

            Me.salt = salt
            Me.alphabet = New String(alphabet.ToCharArray().Distinct().ToArray())
            Me.seps = seps
            Me.minHashLength = minHashLength

            If Me.alphabet.Length < 16 Then
                Throw New ArgumentException("alphabet must contain at least 4 unique characters.", "alphabet")
            End If

            Me.SetupSeps()
            Me.SetupGuards()
        End Sub

        ''' <summary>
        ''' Encodes the provided numbers into a hashed string
        ''' </summary>
        ''' <param name="numbers">the numbers to encode</param>
        ''' <returns>the hashed string</returns>
        Public Overridable Function Encode(ParamArray numbers As Integer()) As String
            If numbers.Any(Function(n) n < 0) Then
                Return String.Empty
            End If
            Return Me.GenerateHashFrom(numbers.[Select](Function(n) CLng(n)).ToArray())
        End Function

        ''' <summary>
        ''' Encodes the provided numbers into a hashed string
        ''' </summary>
        ''' <param name="numbers">the numbers to encode</param>
        ''' <returns>the hashed string</returns>
        Public Overridable Function Encode(numbers As IEnumerable(Of Integer)) As String
            Return Me.Encode(numbers.ToArray())
        End Function

        ''' <summary>
        ''' Decodes the provided hash into
        ''' </summary>
        ''' <param name="hash">the hash</param>
        ''' <exception cref="T:System.OverflowException">if the decoded number overflows integer</exception>
        ''' <returns>the numbers</returns>
        Public Overridable Function Decode(hash As String) As Integer()
            Return Me.GetNumbersFrom(hash).[Select](Function(n) CInt(n)).ToArray()
        End Function

        ''' <summary>
        ''' Encodes the provided hex string to a hashids hash.
        ''' </summary>
        ''' <param name="hex"></param>
        ''' <returns></returns>
        Public Overridable Function EncodeHex(hex As String) As String
            If Not hexValidator.Value.IsMatch(hex) Then
                Return String.Empty
            End If

            Dim numbers = New List(Of Long)()
            Dim matches = hexSplitter.Value.Matches(hex)

            For Each match As Match In matches
                Dim number = Convert.ToInt64(String.Concat("1", match.Value), 16)
                numbers.Add(number)
            Next

            Return Me.EncodeLong(numbers.ToArray())
        End Function

        ''' <summary>
        ''' Decodes the provided hash into a hex-string
        ''' </summary>
        ''' <param name="hash"></param>
        ''' <returns></returns>
        Public Overridable Function DecodeHex(hash As String) As String
            Dim ret = New StringBuilder()
            Dim numbers = Me.DecodeLong(hash)

            For Each number In numbers
                ret.Append(String.Format("{0:X}", number).Substring(1))
            Next

            Return ret.ToString()
        End Function

        ''' <summary>
        ''' Decodes the provided hashed string into an array of longs 
        ''' </summary>
        ''' <param name="hash">the hashed string</param>
        ''' <returns>the numbers</returns>
        Public Function DecodeLong(hash As String) As Long()
            Return Me.GetNumbersFrom(hash)
        End Function

        ''' <summary>
        ''' Encodes the provided longs to a hashed string
        ''' </summary>
        ''' <param name="numbers">the numbers</param>
        ''' <returns>the hashed string</returns>
        Public Function EncodeLong(ParamArray numbers As Long()) As String
            If numbers.Any(Function(n) n < 0) Then
                Return String.Empty
            End If
            Return Me.GenerateHashFrom(numbers)
        End Function

        ''' <summary>
        ''' Encodes the provided longs to a hashed string
        ''' </summary>
        ''' <param name="numbers">the numbers</param>
        ''' <returns>the hashed string</returns>
        Public Function EncodeLong(numbers As IEnumerable(Of Long)) As String
            Return Me.EncodeLong(numbers.ToArray())
        End Function




    ''' <summary>
    ''' 
    ''' </summary>
    Private Sub SetupSeps()
            ' seps should contain only characters present in alphabet; 
            seps = New String(seps.ToCharArray().Intersect(alphabet.ToCharArray()).ToArray())

            ' alphabet should not contain seps.
            alphabet = New String(alphabet.ToCharArray().Except(seps.ToCharArray()).ToArray())

            seps = ConsistentShuffle(seps, salt)

            If seps.Length = 0 OrElse (alphabet.Length / seps.Length) > SEP_DIV Then
                Dim sepsLength = CInt(Math.Ceiling(alphabet.Length / SEP_DIV))
                If sepsLength = 1 Then
                    sepsLength = 2
                End If

                If sepsLength > seps.Length Then
                    Dim diff = sepsLength - seps.Length
                    seps += alphabet.Substring(0, diff)
                    alphabet = alphabet.Substring(diff)
                Else

                    seps = seps.Substring(0, sepsLength)
                End If
            End If

#If CORE Then
			sepsRegex = New Regex(String.Concat("[", seps, "]"))
#Else
            sepsRegex = New Regex(String.Concat("[", seps, "]"), RegexOptions.Compiled)
#End If
            alphabet = ConsistentShuffle(alphabet, salt)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        Private Sub SetupGuards()
            Dim guardCount = CInt(Math.Ceiling(alphabet.Length / GUARD_DIV))

            If alphabet.Length < 3 Then
                guards = seps.Substring(0, guardCount)
                seps = seps.Substring(guardCount)
            Else

                guards = alphabet.Substring(0, guardCount)
                alphabet = alphabet.Substring(guardCount)
            End If

#If CORE Then
			guardsRegex = New Regex(String.Concat("[", guards, "]"))
#Else
            guardsRegex = New Regex(String.Concat("[", guards, "]"), RegexOptions.Compiled)
#End If
        End Sub

        ''' <summary>
        ''' Internal function that does the work of creating the hash
        ''' </summary>
        ''' <param name="numbers"></param>
        ''' <returns></returns>
        Private Function GenerateHashFrom(numbers As Long()) As String
            If numbers Is Nothing OrElse numbers.Length = 0 Then
                Return String.Empty
            End If

            Dim ret = New StringBuilder()
            Dim alphabet = Me.alphabet

            Dim numbersHashInt As Long = 0
            For i = 0 To numbers.Length - 1
                numbersHashInt += CInt(numbers(i) Mod (i + 100))
            Next

            Dim lottery = alphabet(CInt(numbersHashInt Mod alphabet.Length))
            ret.Append(lottery.ToString())

            For i = 0 To numbers.Length - 1
                Dim number = numbers(i)
                Dim buffer = Convert.ToString(Convert.ToString(lottery) & Me.salt) & alphabet

                alphabet = ConsistentShuffle(alphabet, buffer.Substring(0, alphabet.Length))
                Dim last = Me.Hash(number, alphabet)

                ret.Append(last)

                If i + 1 < numbers.Length Then
                    number = number Mod (Asc(last(0)) + i)
                    Dim sepsIndex = (CInt(number) Mod Me.seps.Length)

                    ret.Append(Me.seps(sepsIndex))
                End If
            Next

            If ret.Length < Me.minHashLength Then
                Dim guardIndex = (CInt(numbersHashInt + Asc(ret(0))) Mod Me.guards.Length)
                Dim guard = Me.guards(guardIndex)

                ret.Insert(0, guard)

                If ret.Length < Me.minHashLength Then
                    guardIndex = (CInt(numbersHashInt + Asc(ret(2))) Mod Me.guards.Length)
                    guard = Me.guards(guardIndex)

                    ret.Append(guard)
                End If
            End If

            Dim halfLength = CInt(alphabet.Length / 2)
            While ret.Length < Me.minHashLength
                alphabet = ConsistentShuffle(alphabet, alphabet)
                ret.Insert(0, alphabet.Substring(halfLength))
                ret.Append(alphabet.Substring(0, halfLength))

                Dim excess = ret.Length - Me.minHashLength
                If excess > 0 Then
                    ret.Remove(0, excess / 2)
                    ret.Remove(Me.minHashLength, ret.Length - Me.minHashLength)
                End If
            End While

            Return ret.ToString()
        End Function

        Private Function Hash(input As Long, alphabet As String) As String
            Dim hash__1 = New StringBuilder()

            Do
                hash__1.Insert(0, alphabet(CInt(input Mod alphabet.Length)))
                input = (input / alphabet.Length)
            Loop While input > 0

            Return hash__1.ToString()
        End Function

        Private Function Unhash(input As String, alphabet As String) As Long
            Dim number As Long = 0

            For i = 0 To input.Length - 1
                Dim pos = alphabet.IndexOf(input(i))
                number += CLng(pos * Math.Pow(alphabet.Length, input.Length - i - 1))
            Next

            Return number
        End Function

        Private Function GetNumbersFrom(hash As String) As Long()
            If String.IsNullOrWhiteSpace(hash) Then
                Return New Long(-1) {}
            End If

            Dim alphabet = New String(Me.alphabet.ToCharArray())
            Dim ret = New List(Of Long)()
            Dim i As Integer = 0

            Dim hashBreakdown = guardsRegex.Replace(hash, " ")
            Dim hashArray = hashBreakdown.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

            If hashArray.Length = 3 OrElse hashArray.Length = 2 Then
                i = 1
            End If

            hashBreakdown = hashArray(i)
            If hashBreakdown(0) <> ControlChars.NullChar Then
                Dim lottery = hashBreakdown(0)
                hashBreakdown = hashBreakdown.Substring(1)

                hashBreakdown = sepsRegex.Replace(hashBreakdown, " ")
                hashArray = hashBreakdown.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

                For j = 0 To hashArray.Length - 1
                    Dim subHash = hashArray(j)
                    Dim buffer = Convert.ToString(Convert.ToString(lottery) & Me.salt) & alphabet

                    alphabet = ConsistentShuffle(alphabet, buffer.Substring(0, alphabet.Length))
                    ret.Add(Unhash(subHash, alphabet))
                Next

                If EncodeLong(ret.ToArray()) <> hash Then
                    ret.Clear()
                End If
            End If

            Return ret.ToArray()
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alphabet"></param>
        ''' <param name="salt"></param>
        ''' <returns></returns>
        Private Function ConsistentShuffle(alphabet As String, salt As String) As String
            If String.IsNullOrWhiteSpace(salt) Then
                Return alphabet
            End If

            Dim n As Integer
            Dim letters = alphabet.ToCharArray()
            Dim i As Integer = letters.Length - 1, v As Integer = 0, p As Integer = 0
            While i > 0
                v = v Mod salt.Length
                p += InlineAssignHelper(n, Asc(salt(v)))
                Dim j = (n + v + p) Mod i
                ' swap characters at positions i and j
                Dim temp = letters(j)
                letters(j) = letters(i)
                letters(i) = temp
                i -= 1
                v += 1
            End While

            Return New String(letters)
        End Function


        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

    End Class








