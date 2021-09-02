''' <summary>
''' Arrays de uso comum da biblioteca
''' </summary>
Public Module Arrays

    ''' <summary>
    ''' Caracteres usado para encapsular palavras em textos
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property WordWrappers As IEnumerable(Of String)
        Get
            Return OpenWrappers.Union(CloseWrappers)
        End Get
    End Property

    ReadOnly Property AlphaLowerChars As IEnumerable(Of String)
        Get
            Return Consonants.Union(Vowels).OrderBy(Function(x) x).AsEnumerable
        End Get
    End Property

    ReadOnly Property AlphaUpperChars As IEnumerable(Of String)
        Get
            Return AlphaLowerChars.Select(Function(x) x.ToUpper())
        End Get
    End Property

    ReadOnly Property AlphaChars As IEnumerable(Of String)
        Get
            Return AlphaUpperChars.Union(AlphaLowerChars).OrderBy(Function(x) x).AsEnumerable
        End Get
    End Property

    ReadOnly Property AlphaNumericChars As IEnumerable(Of String)
        Get
            Return AlphaChars.Union(NumberChars).AsEnumerable
        End Get
    End Property

    ReadOnly Property NumberChars As IEnumerable(Of String)
        Get
            Return {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}.AsEnumerable
        End Get
    End Property

    ReadOnly Property BreakLineChars As IEnumerable(Of String)
        Get
            Return {Environment.NewLine, vbCr, vbLf, vbCrLf, vbNewLine}.AsEnumerable
        End Get
    End Property

    ReadOnly Property CloseWrappers As IEnumerable(Of String)
        Get
            Return {"""", "'", ")", "}", "]", ">"}.AsEnumerable
        End Get
    End Property

    ReadOnly Property EndOfSentencePunctuation As IEnumerable(Of String)
        Get
            Return {".", "?", "!"}.AsEnumerable
        End Get
    End Property

    ReadOnly Property MidSentencePunctuation As IEnumerable(Of String)
        Get
            Return {":", ";", ","}.AsEnumerable
        End Get
    End Property

    ReadOnly Property OpenWrappers As IEnumerable(Of String)
        Get
            Return {"""", "'", "(", "{", "[", "<"}.AsEnumerable
        End Get
    End Property

    ''' <summary>
    ''' Caracteres em branco
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property WhiteSpaceChars As IEnumerable(Of String)
        Get
            Return {Environment.NewLine, " ", vbTab, vbLf, vbCr, vbCrLf}.AsEnumerable
        End Get
    End Property

    ''' <summary>
    ''' Strings utilizadas para descobrir as palavras em uma string
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property WordSplitters As IEnumerable(Of String)
        Get
            Return {"&nbsp;", """", "'", "(", ")", ",", ".", "?", "!", ";", "{", "}", "[", "]", "|", " ", ":", vbNewLine, "<br>", "<br/>", "<br/>", Environment.NewLine, vbCr, vbCrLf}.AsEnumerable
        End Get
    End Property

    Public ReadOnly Property Consonants As IEnumerable(Of String)
        Get
            Return {"b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z"}.AsEnumerable
        End Get
    End Property

    Public ReadOnly Property Vowels As IEnumerable(Of String)
        Get
            Return {"a", "e", "i", "o", "u", "y"}.AsEnumerable
        End Get
    End Property


    Public ReadOnly Property PrimitiveTypes As IEnumerable(Of Type)
        Get
            Return {GetType(String), GetType(Char), GetType(Byte), GetType(SByte), GetType(DateTime)}.Union(PrimitiveNumericTypes)
        End Get
    End Property

    Public ReadOnly Property PrimitiveNumericTypes As IEnumerable(Of Type)
        Get
            Return {GetType(Single), GetType(UShort), GetType(Short), GetType(Integer), GetType(UInteger), GetType(ULong), GetType(Long), GetType(Double), GetType(Decimal)}.AsEnumerable()
        End Get
    End Property
End Module
