Imports InnerLibs
Imports FastColoredTextBoxNS
Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Character
    Inherits DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Character))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SourceCode = New FastColoredTextBoxNS.FastColoredTextBox()
        Me.SpriteSelector = New System.Windows.Forms.PictureBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.HumorList = New System.Windows.Forms.ListView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SourceCode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SpriteSelector, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SpriteSelector)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TextBox1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.HumorList)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Button1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SourceCode)
        Me.SplitContainer1.Size = New System.Drawing.Size(576, 560)
        Me.SplitContainer1.SplitterDistance = 256
        Me.SplitContainer1.TabIndex = 0
        '
        'SourceCode
        '
        Me.SourceCode.AutoCompleteBracketsList = New Char() {Global.Microsoft.VisualBasic.ChrW(40), Global.Microsoft.VisualBasic.ChrW(41), Global.Microsoft.VisualBasic.ChrW(123), Global.Microsoft.VisualBasic.ChrW(125), Global.Microsoft.VisualBasic.ChrW(91), Global.Microsoft.VisualBasic.ChrW(93), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(39), Global.Microsoft.VisualBasic.ChrW(39), Global.Microsoft.VisualBasic.ChrW(60), Global.Microsoft.VisualBasic.ChrW(62)}
        Me.SourceCode.AutoIndentCharsPatterns = "" & Global.Microsoft.VisualBasic.ChrW(10) & "^\s*[\w\.]+(\s\w+)?\s*(?<range>=)\s*(?<range>[^;]+);" & Global.Microsoft.VisualBasic.ChrW(10)
        Me.SourceCode.AutoScrollMinSize = New System.Drawing.Size(571, 70)
        Me.SourceCode.BackBrush = Nothing
        Me.SourceCode.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2
        Me.SourceCode.CharHeight = 14
        Me.SourceCode.CharWidth = 8
        Me.SourceCode.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.SourceCode.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
        Me.SourceCode.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SourceCode.IsReplaceMode = False
        Me.SourceCode.Language = FastColoredTextBoxNS.Language.JS
        Me.SourceCode.LeftBracket = Global.Microsoft.VisualBasic.ChrW(40)
        Me.SourceCode.LeftBracket2 = Global.Microsoft.VisualBasic.ChrW(123)
        Me.SourceCode.Location = New System.Drawing.Point(0, 0)
        Me.SourceCode.Name = "SourceCode"
        Me.SourceCode.Paddings = New System.Windows.Forms.Padding(0)
        Me.SourceCode.RightBracket = Global.Microsoft.VisualBasic.ChrW(41)
        Me.SourceCode.RightBracket2 = Global.Microsoft.VisualBasic.ChrW(125)
        Me.SourceCode.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.SourceCode.ServiceColors = CType(resources.GetObject("SourceCode.ServiceColors"), FastColoredTextBoxNS.ServiceColors)
        Me.SourceCode.Size = New System.Drawing.Size(576, 300)
        Me.SourceCode.TabIndex = 16
        Me.SourceCode.Text = "//Código gerado para o personagem a partir do Designer de Personagem" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "//Cód" &
    "igo customizado pelo usuário (Não remova esta linha)"
        Me.SourceCode.Zoom = 100
        '
        'SpriteSelector
        '
        Me.SpriteSelector.BackColor = System.Drawing.Color.Silver
        Me.SpriteSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SpriteSelector.Location = New System.Drawing.Point(12, 9)
        Me.SpriteSelector.Name = "SpriteSelector"
        Me.SpriteSelector.Size = New System.Drawing.Size(184, 203)
        Me.SpriteSelector.TabIndex = 10
        Me.SpriteSelector.TabStop = False
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(243, 7)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(321, 20)
        Me.TextBox1.TabIndex = 11
        '
        'HumorList
        '
        Me.HumorList.Location = New System.Drawing.Point(243, 35)
        Me.HumorList.Name = "HumorList"
        Me.HumorList.Size = New System.Drawing.Size(195, 97)
        Me.HumorList.TabIndex = 15
        Me.HumorList.UseCompatibleStateImageBehavior = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(202, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Nome"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(444, 32)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(63, 23)
        Me.Button1.TabIndex = 14
        Me.Button1.Text = "Adicionar Humor"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(202, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Humor"
        '
        'Character
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(576, 560)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "Character"
        Me.Text = "Character"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.SourceCode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SpriteSelector, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SpriteSelector As PictureBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents HumorList As ListView
    Friend WithEvents Label1 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents SourceCode As FastColoredTextBox
End Class
