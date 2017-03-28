<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class NotificationForm
    Inherits System.Windows.Forms.Form

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
        Me.MessageLabel = New System.Windows.Forms.Label()
        Me.lifeTimer = New System.Windows.Forms.Timer(Me.components)
        Me.OKButton = New System.Windows.Forms.Button()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.InputBox = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'MessageLabel
        '
        Me.MessageLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.MessageLabel.Location = New System.Drawing.Point(4, 26)
        Me.MessageLabel.Name = "MessageLabel"
        Me.MessageLabel.Size = New System.Drawing.Size(265, 102)
        Me.MessageLabel.TabIndex = 0
        Me.MessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lifeTimer
        '
        Me.lifeTimer.Interval = 1000
        '
        'OKButton
        '
        Me.OKButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OKButton.BackColor = System.Drawing.SystemColors.ControlLight
        Me.OKButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.OKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.OKButton.ForeColor = System.Drawing.Color.Black
        Me.OKButton.Location = New System.Drawing.Point(188, 131)
        Me.OKButton.Margin = New System.Windows.Forms.Padding(2)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(79, 29)
        Me.OKButton.TabIndex = 1
        Me.OKButton.Text = "OK"
        Me.OKButton.UseVisualStyleBackColor = False
        '
        'CloseButton
        '
        Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CloseButton.BackColor = System.Drawing.Color.Red
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseButton.FlatAppearance.BorderSize = 0
        Me.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CloseButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CloseButton.ForeColor = System.Drawing.Color.Transparent
        Me.CloseButton.Location = New System.Drawing.Point(248, 3)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(21, 20)
        Me.CloseButton.TabIndex = 2
        Me.CloseButton.Text = "X"
        Me.CloseButton.UseVisualStyleBackColor = False
        Me.CloseButton.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(13, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "0"
        Me.Label1.Visible = False
        '
        'InputBox
        '
        Me.InputBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.InputBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.InputBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
        Me.InputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.InputBox.Location = New System.Drawing.Point(8, 137)
        Me.InputBox.Name = "InputBox"
        Me.InputBox.Size = New System.Drawing.Size(175, 20)
        Me.InputBox.TabIndex = 4
        '
        'NotificationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(272, 166)
        Me.Controls.Add(Me.InputBox)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.OKButton)
        Me.Controls.Add(Me.MessageLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "NotificationForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents MessageLabel As System.Windows.Forms.Label
    Private WithEvents lifeTimer As System.Windows.Forms.Timer
    Private WithEvents OKButton As Windows.Forms.Button
    Private WithEvents CloseButton As Windows.Forms.Button
    Private WithEvents Label1 As Windows.Forms.Label
    Private WithEvents InputBox As Windows.Forms.TextBox
End Class
