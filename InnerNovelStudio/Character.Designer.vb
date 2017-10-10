Imports InnerLibs
Imports FastColoredTextBoxNS
Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CharacterEditor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CharacterEditor))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.SourceCode = New FastColoredTextBoxNS.FastColoredTextBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SourceCode, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.PropertyGrid1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SourceCode)
        Me.SplitContainer1.Size = New System.Drawing.Size(913, 560)
        Me.SplitContainer1.SplitterDistance = 320
        Me.SplitContainer1.TabIndex = 0
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark
        Me.PropertyGrid1.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(913, 320)
        Me.PropertyGrid1.TabIndex = 17
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
        Me.SourceCode.Font = New System.Drawing.Font("Courier New", 9.75!)
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
        Me.SourceCode.Size = New System.Drawing.Size(913, 236)
        Me.SourceCode.TabIndex = 16
        Me.SourceCode.Text = "//Código gerado para o personagem a partir do Designer de Personagem" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "//Cód" &
    "igo customizado pelo usuário (Não remova esta linha)"
        Me.SourceCode.Zoom = 100
        '
        'CharacterEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(913, 560)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "CharacterEditor"
        Me.Text = "Character"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.SourceCode, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents SourceCode As FastColoredTextBox
    Friend WithEvents PropertyGrid1 As PropertyGrid
End Class
