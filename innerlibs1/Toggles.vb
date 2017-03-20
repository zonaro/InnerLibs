
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
''' <summary>
''' Modulo que liga/desliga, (inverte) valores de variaveis
''' </summary>
''' <remarks></remarks>
Public Module Toggles
    ''' <summary>
    ''' Inverte os valores TRUE/FALSE
    ''' </summary>
    ''' <param name="Bool">Variavel BOOLEANA que será invertida</param>

    <Extension()>
    Public Sub Toggle(ByRef Bool As Boolean)
        Bool = Not Bool
    End Sub
    ''' <summary>
    ''' Inverte os valores 0/1
    ''' </summary>
    ''' <param name="Int">Variavel INTEIRA que será invertida</param> 
    <Extension()>
    Public Sub Toggle(ByRef Int As Integer)
        If Int > 0 Then
            Int = 0
        Else
            Int = 1
        End If
    End Sub
    ''' <summary>
    ''' Inverte a visibilidade do form
    ''' </summary>
    ''' <param name="Form">Variavel INTEIRA que será invertida</param>
    <Extension()>
    Public Sub Toggle(ByRef Form As Form)
        If Form.Visible Then Form.Hide() Else Form.Show()
    End Sub

    ''' <summary>
    ''' Alterna uma String ente 2 valores diferentes
    ''' </summary>
    ''' <param name="CurrentString">String contendo o primeiro ou segundo valor</param>
    ''' <param name="TrueValue">Primeiro valor</param>
    ''' <param name="FalseValue">Segundo Valor</param>
    <Extension()>
    Public Sub Toggle(ByRef CurrentString As String, Optional TrueValue As String = "True", Optional FalseValue As String = "False")
        CurrentString = If(CurrentString = TrueValue, FalseValue, TrueValue)
    End Sub

    ''' <summary>
    ''' Alterna um char ente 2 valores diferentes
    ''' </summary>
    ''' <param name="CurrentChar">String contendo o primeiro ou segundo valor</param>
    ''' <param name="TrueValue">Primeiro valor</param>
    ''' <param name="FalseValue">Segundo Valor</param>
    <Extension()>
    Public Sub Toggle(ByRef CurrentChar As Char, Optional TrueValue As Char = "1", Optional FalseValue As Char = "0")
        CurrentChar = If(CurrentChar = TrueValue, FalseValue, TrueValue)
    End Sub


End Module