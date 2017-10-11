Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
Imports InnerLibs

Public Class CharacterEditor



    ReadOnly Property MyPath As DirectoryInfo
        Get
            Return MainForm.Instance.CharPath.FullName.Append(Path.DirectorySeparatorChar).Append(Me.Propriedades.ToString).ToDirectory
        End Get
    End Property

    ReadOnly Property MyHumorPath As DirectoryInfo
        Get
            Return MainForm.Instance.CharPath.FullName.Append(Path.DirectorySeparatorChar).Append(Me.Propriedades.ToString).Append(Path.DirectorySeparatorChar).Append("humor").ToDirectory
        End Get
    End Property

    Property Propriedades As New Character



    Private Sub SourceCode_KeyUp(sender As Object, e As KeyEventArgs) Handles SourceCode.KeyUp
        Me.Propriedades.CustomCode = SourceCode.Text.GetAfter("//Código customizado pelo usuário (Não remova esta linha)")
    End Sub


    Public Sub Salvar()
        Me.Propriedades.SerializeJSON.WriteToFile(MyPath.FullName.Append("\prop.json"))
        Me.Propriedades.Sprite.Save(MyPath.FullName.Append("\MainSprite").Append())
        SourceCode.SaveToFile(MyPath.FullName.Append("\code.js"), Encoding.UTF8)

        For Each h In Me.Propriedades.Humor
            h.Sprite.Save(MyHumorPath.FullName.Append(Path.DirectorySeparatorChar).Append(h.ToString & ".png"), Imaging.ImageFormat.Png)
        Next

    End Sub

    Public Sub New(Optional CharName As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        If CharName.IsNotBlank Then
            Using o As StreamReader = File.OpenText(MainForm.Instance.CharPath.FullName.Append(Path.DirectorySeparatorChar).Append(CharName).Append(Path.DirectorySeparatorChar).Append("prop.json"))
                Me.Propriedades = o.ReadToEnd.ParseJSON(Of Character)
                Me.Propriedades.Sprite = Image.FromFile(MyPath.FullName.Append(Path.DirectorySeparatorChar).Append("MainSprite.png"))
                For Each h In Me.Propriedades.Humor
                    h.Sprite = Image.FromFile(MyHumorPath.FullName.Append(Path.DirectorySeparatorChar).Append(h.ToString & ".png"))
                Next
            End Using

        End If

        ' Add any initialization after the InitializeComponent() call.

    End Sub



    Private Sub CharacterEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PropertyGrid1.SelectedObject = Me.Propriedades
        SourceCode.Text = Me.Propriedades.Code
        Me.Text = Me.Propriedades.Nome
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        SourceCode.Text = Me.Propriedades.Code
        Me.Text = Me.Propriedades.Nome
    End Sub



End Class



Public Class Character

    <Category("Bio"), Description("Nome do Personagem")>
    Property Nome As String = "Sem Nome"
    <Category("Visual"), Description("Imagem do Personagem"), ScriptIgnore>
    Public Property Sprite As Image
    <Category("Bio"), Description("Possiveis estados de humor do personagem. Cada humor  é representado por uma sprite")>
    Property Humor As New List(Of Humor)
    <Category("Bio"), Description("Informações Adicionais sobre este personagem")>
    Property Info As New List(Of CharInfo)

    <Category("Codigo"), Description("Informações Adicionais sobre este personagem na forma que será apresentada no código")>
    ReadOnly Property InfoCode As String
        Get
            Return Info.ToDictionary(Function(p) p.InfoName, Function(p)
                                                                 Select Case True
                                                                     Case p.InfoValue.IsNumber
                                                                         Return p.InfoValue.To(Of Decimal)
                                                                     Case Else
                                                                         Return p.InfoValue
                                                                 End Select
                                                             End Function).SerializeJSON
        End Get
    End Property

    <Category("Codigo"), Description("Código JS customizado do personagem")>
    Property CustomCode As String


    <Category("Codigo"), Description("Código JS do personagem")>
    ReadOnly Property Code
        Get
            Dim char_string = "//Código gerado a partir do Designer de Personagem" & Environment.NewLine
            char_string &= "var " & Me.ToString & " = new Character(""/char/" & Me.ToString & "/MainSprite.png"", " & Me.Nome.Quote & " ," & Me.Humor.Select(Function(i) i.ToString).SerializeJSON & ");"
            char_string &= Environment.NewLine & Environment.NewLine
            char_string &= "//Código customizado pelo usuário (Não remova esta linha)" & Environment.NewLine
            char_string &= CustomCode
            Return char_string
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Nome.ToSlug(True)
    End Function

End Class


Public Class CharInfo
    <Category("Bio"), Description("Nome Informacao especifica do Personagem")>
    Property InfoName As String
    <Category("Bio"), Description("Valor Informacao especifica do Personagem")>
    Property InfoValue As String
End Class


Public Class Humor
    <Category("Bio"), Description("Nome do estado de humor (ex.: Feliz, Triste, Mau Humor, Possuido, Morto etc")>
    Property Nome As String = "Novo Humor"
    <Category("Visual"), Description("Imagem que representa este humor"), ScriptIgnore>
    Property Sprite As Image


    Public Overrides Function ToString() As String
        Return Nome.ToSlug(True)
    End Function
End Class
