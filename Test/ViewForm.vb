Public Class ViewForm
    Inherits System.Windows.Forms.Form

    Private Shared mInstance As ViewForm = Nothing

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents txtText As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtText = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'txtText
        '
        Me.txtText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtText.Multiline = True
        Me.txtText.Name = "txtText"
        Me.txtText.ReadOnly = True
        Me.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtText.Size = New System.Drawing.Size(292, 273)
        Me.txtText.TabIndex = 0
        Me.txtText.Text = ""
        '
        'ViewForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(292, 273)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.txtText})
        Me.Name = "ViewForm"
        Me.Text = "ViewForm"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub ViewForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Public Shared Sub ShowText(ByVal text As String)
        If mInstance Is Nothing Then
            mInstance = New ViewForm()
        End If
        mInstance.Show()
        mInstance.Focus()
        mInstance.txtText.Text = text
    End Sub

    Protected Overrides Sub OnClosed(ByVal e As System.EventArgs)
        mInstance = Nothing
    End Sub
End Class

