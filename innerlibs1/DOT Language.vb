
Imports System.Globalization
Imports System.Runtime.CompilerServices

''' <summary>
''' Wrapper para criaçao de gráficos em DOT Language
''' </summary>
Public Class Digraph
    Inherits List(Of DotNode)



    ''' <summary>
    ''' Tipo do Grafico
    ''' </summary>
    ''' <returns></returns>

    ReadOnly Property GraphType As String = "digraph"

    ''' <summary>
    ''' Nome do Gráfico
    ''' </summary>
    ''' <returns></returns>
    Property Name As String = ""

    ''' <summary>
    ''' Escreve a DOT string correspondente a este gráfico
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim s = Me.Select(Function(n) n.ToString & Environment.NewLine).ToArray.Join("")
        s = s.Split(Environment.NewLine).Distinct.Join(Environment.NewLine) & Environment.NewLine
        Return GraphType & " " & Name.ToSlug(True) & " " & s.Wrap("{")
    End Function


End Class

''' <summary>
''' Representa um nó de um grafico em DOT Language
''' </summary>
Public Class DotNode
    Inherits Dictionary(Of String, String)

    ''' <summary>
    ''' Cria um novo nó
    ''' </summary>
    ''' <param name="ID"></param>
    Sub New(ID As String)
        Me.ID = ID
    End Sub


    ''' <summary>
    ''' ID deste nó
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String

    ''' <summary>
    ''' Nós que serão relacionados a este nó
    ''' </summary>
    ''' <returns></returns>
    Public Property Relations As New Dictionary(Of DotEdge, DotNode)


    ''' <summary>
    ''' Escreve a DOT string deste nó e seus respectivos nós filhos
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim dotstring As String = ""
        For Each prop In Me
            Dim val = prop.Value.ToString.QuoteIf(prop.Value.ToString.Contains(" ") Or prop.Value.ToString.IsBlank Or prop.Value.IsURL)
            If val.IsIn({"True", "False"}) Then val = val.ToLower
            If val.IsNumber Then val = val.Replace(",", ".")
            dotstring &= ID.ToString.ToSlug(True) & " " & (prop.Key & "=" & val).Wrap("[").Append(";" & Environment.NewLine)
        Next
        For Each c In Relations
            If c.Key.Inverted Then
                dotstring.Append(c.Value.ID.ToSlug(True) & If(c.Key.Oriented, " -> ", " -- ") & ID.ToSlug(True) & " " & c.Key.ToString & ";")
            Else
                dotstring.Append(ID.ToSlug(True) & If(c.Key.Oriented, " -> ", " -- ") & c.Value.ID.ToSlug(True) & " " & c.Key.ToString & ";")
            End If
            dotstring.Append(Environment.NewLine)
            dotstring.Append(c.Value.ToString)
        Next
        Return dotstring
    End Function



End Class

''' <summary>
''' Representa uma ligação entre nós de um grafico em DOT Language
''' </summary>
Public Class DotEdge
    Inherits Dictionary(Of String, String)

    ''' <summary>
    ''' Cria uma nova ligaçao 
    ''' </summary>
    ''' <param name="Oriented">Relação orientada</param>
    ''' <param name="Inverted">Relação Invertida</param>
    Sub New(Optional Oriented As Boolean = True, Optional Inverted As Boolean = False)
        Me.Oriented = Oriented
        Me.Inverted = Inverted
    End Sub

    ''' <summary>
    ''' Indica se esta ligação é orientada ou não
    ''' </summary>
    ''' <returns></returns>
    Property Oriented As Boolean = True

    ''' <summary>
    ''' Indica se a orientação desta relaçao deverá ser invertida na hora de gerar a DOT String
    ''' </summary>
    ''' <returns></returns>
    Property Inverted As Boolean = False

    ''' <summary>
    ''' Escreve a DOT String desta ligaçao
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim propstring = ""
        If Me.Count > 0 Then
            For Each prop In Me
                Dim val = prop.Value.ToString.QuoteIf(prop.Value.ToString.Contains(" ") Or prop.Value.ToString.IsBlank Or prop.Value.IsURL)
                If val.IsIn({"True", "False"}) Then val = val.ToLower
                If val.IsNumber Then val = val.Replace(",", ".")
                propstring &= prop.Key & "=" & val & " "
            Next
            propstring = propstring.Trim.Wrap("[")
        End If
        Return propstring
    End Function

End Class
