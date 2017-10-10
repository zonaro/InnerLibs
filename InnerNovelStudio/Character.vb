Imports InnerLibs

Public Class Character

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Character_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Property Sprite As String
    Property Nome As String
    Property Humor As New List(Of String)

    Property CustomCode As String

    Public Overrides Function ToString() As String
        Return SourceCode.Text
    End Function

    Private Sub TextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyUp
        Me.Nome = TextBox1.Text
        Me.Text = Me.Nome & ".json"
        GerarCodigo()
    End Sub

    Sub GerarCodigo()
        Dim char_string = "//Código gerado a partir do Designer de Personagem" & Environment.NewLine
        char_string &= "var " & Me.Nome.ToSlug(True) & " = new Character(" & Me.Sprite.Quote & ", " & Me.Nome.Quote & " ," & Me.Humor.SerializeJSON & ");"
        char_string &= Environment.NewLine & Environment.NewLine
        char_string &= "//Código customizado pelo usuário (Não remova esta linha)" & Environment.NewLine
        char_string &= CustomCode
        SourceCode.Text = char_string
    End Sub

    Private Sub SourceCode_KeyUp(sender As Object, e As KeyEventArgs) Handles SourceCode.KeyUp
        CustomCode = SourceCode.Text.GetAfter("//Código customizado pelo usuário (Não remova esta linha)")
    End Sub

    Private Sub HumorList_ControlAdded(sender As Object, e As ControlEventArgs) Handles HumorList.ControlAdded
        GerarCodigo()
    End Sub

    Public Sub Save()
        Me.SerializeJSON.WriteToFile(MainForm.CharPath.FullName.Append(Me.Text))
    End Sub

End Class