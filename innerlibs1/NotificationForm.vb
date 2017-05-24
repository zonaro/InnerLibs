Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports InnerLibs.FormAnimator

''' <summary>
''' Formulário de notificações interativas
''' </summary>
Public NotInheritable Class NotificationForm

    ''' <summary>
    ''' Lista com as notificações abertas
    ''' </summary>
    Public Shared Property VisibleNotifications As New List(Of NotificationForm)

    ''' <summary>
    ''' Indicates whether the form can receive focus or not.
    ''' </summary>
    Private allowFocus As Boolean = False

    ''' <summary>
    ''' The object that creates the sliding animation.
    ''' </summary>
    Private animator As FormAnimator

    ''' <summary>
    ''' The handle of the window that currently has focus.
    ''' </summary>
    Private currentForegroundWindow As IntPtr

    Private CorrectPos As Point = Nothing

    ''' <summary>
    ''' Gets the handle of the window that currently has focus.
    ''' </summary>
    ''' <returns>The handle of the window that currently has focus.</returns>
    <DllImport("user32")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    ''' <summary>
    ''' Activates the specified window.
    ''' </summary>
    ''' <param name="hWnd">The handle of the window to be focused.</param>
    ''' <returns>True if the window was focused; False otherwise.</returns>
    <DllImport("user32")>
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As Boolean
    End Function

    ''' <summary>
    ''' Cria uma Nova Notificação
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        CloseButton.Visible = OnOKButtonClickActions.Count > 0
        AddHandler InputBox.TextChanged, AddressOf TextChange

        AddHandler OKButton.Click, AddressOf OKButtonClick
        Me.animator = New FormAnimator(Me, AnimationMethod.Slide, OpenDirection, 500)
    End Sub

    Private Sub OKButtonClick(sender As Object, e As EventArgs)
        RaiseEvent OnOKButtonClick(sender, e)
    End Sub

    Private Sub TextChange(sender As Object, e As EventArgs)
        RaiseEvent OnInputBoxTextChanged(sender, e)
    End Sub

    ''' <summary>
    ''' Alinhamento do texto da notificação
    ''' </summary>
    ''' <returns></returns>
    Public Property TextAlign As ContentAlignment
        Get
            Return MessageLabel.TextAlign
        End Get
        Set(value As ContentAlignment)
            Me.MessageLabel.TextAlign = value
        End Set
    End Property

    ''' <summary>
    ''' Texto do Botão OK
    ''' </summary>
    ''' <returns></returns>
    Public Property OKButtonText As String
        Get
            Return OKButton.Text
        End Get
        Set(value As String)
            Me.OKButton.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Texto da caixa de input
    ''' </summary>
    ''' <returns></returns>
    Public Property InputBoxText As String
        Get
            Return InputBox.Text
        End Get
        Set(value As String)
            InputBox.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Exibe uma caixa de texto na notificação para entrada de informações
    ''' </summary>
    ''' <returns></returns>
    Public Property ShowInputBox As Boolean
        Get
            Return InputBox.Visible
        End Get
        Set(value As Boolean)
            InputBox.Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Coleção de strings para o AutoCompletar do InputBox
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property InputBoxAutoComplete As AutoCompleteStringCollection
        Get
            Return InputBox.AutoCompleteCustomSource
        End Get
    End Property

    ''' <summary>
    ''' Tempo restante antes que a notificação seja fchada automaticamente
    ''' </summary>
    ''' <returns></returns>
    Public Property RemainTime As Integer = 0

    ''' <summary>
    ''' Comportamento do tempo restante da notificação caso ela seja re-utilizada
    ''' </summary>
    ''' <returns></returns>
    Public Property RemainTimeBehavior As RemainTimeBehavior = RemainTimeBehavior.StackTime

    ''' <summary>
    ''' Valor que representa se o contador de segundos deve ser exibido na notificação
    ''' </summary>
    ''' <returns></returns>
    Public Property ShowRemainTime As Boolean = True

    ''' <summary>
    ''' Tamanho da notificação
    ''' </summary>
    ''' <returns></returns>
    Public Overloads Property Size As Size
        Get
            Return MyBase.Size
        End Get
        Set(value As Size)
            If value.Width > 0 And value.Height > 0 Then MyBase.Size = value
        End Set
    End Property

    ''' <summary>
    ''' Direção que a notificação desliza ao aparecer
    ''' </summary>
    ''' <returns></returns>
    Public Property OpenDirection As AnimationDirection = AnimationDirection.Up

    ''' <summary>
    ''' Direção que a notificação desliza ao ser fechada
    ''' </summary>
    ''' <returns></returns>
    Public Property CloseDirection As AnimationDirection = AnimationDirection.Down

    ''' <summary>
    ''' Destroi todas as notificações criadas na aplicação.
    ''' </summary>
    Public Shared Sub DestroyNotifications()
        Dim Notifications = New List(Of NotificationForm)
        For index = 0 To Application.OpenForms.Count - 1
            If GetType(NotificationForm) = Application.OpenForms.Item(index).GetType Then
                Notifications.Add(Application.OpenForms.Item(index))
            End If
        Next
        For index = 0 To Notifications.Count - 1
            Notifications(index).Dispose()
        Next
    End Sub

    ''' <summary>
    ''' Exibe ou altera a notificação
    ''' </summary>
    ''' <param name="Seconds">
    ''' Valor em segundos que define o tempo de exibição dessa notificação. Se a notificaáo já
    ''' estiver sendo exibida este valor é utilizado de acordo com a propriedade <see cref="RemainTimeBehavior"/>
    ''' </param>
    Public Shadows Sub Show(Optional Seconds As Integer = 0)
        If Seconds = 0 Then Seconds = -1
        Label1.Text = ""
        Me.animator.Direction = OpenDirection
        'Determine the current foreground window so it can be reactivated each time this form tries to get the focus.
        Me.currentForegroundWindow = GetForegroundWindow()
        Seconds = Seconds.SetMinValue(-1)
        Select Case RemainTimeBehavior
            Case RemainTimeBehavior.StackTime
                RemainTime = RemainTime + Seconds
            Case RemainTimeBehavior.ResetTime
                RemainTime = Seconds
            Case Else
                RemainTime = RemainTime.SetMinValue(1)
        End Select

        'Start counting down the form's liftime.
        If RemainTime > 0 Then
            Me.lifeTimer.Start()
            Me.Label1.Visible = ShowRemainTime
        End If

        'Display the form.
        If Not Me.Visible Then
            For Each openForm As NotificationForm In NotificationForm.VisibleNotifications  'Move each open form upwards to make room for this one.
                openForm.Top -= Me.Height + 5
            Next
            MyBase.Show()
            NotificationForm.VisibleNotifications.Add(Me)
        End If
        If ShowInputBox Then InputBox.Select()
        Me.animator.Direction = CloseDirection
    End Sub

    Private Sub NotificationForm_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        'Prevent the form taking focus when it is initially shown.
        If Not Me.allowFocus Then
            'Activate the window that previously had the focus.
            SetForegroundWindow(Me.currentForegroundWindow)
        End If
    End Sub

    Private Sub NotificationForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'Once the animation has completed the form can receive focus.
        Me.allowFocus = True
    End Sub

    Private Sub NotificationForm_FormClosed(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        Me.animator.Direction = CloseDirection
        'Move down any open forms above this one.
        For Each openForm As NotificationForm In NotificationForm.VisibleNotifications
            If openForm Is Me Then
                e.Cancel = True
                Me.Hide()
                NotificationForm.VisibleNotifications.Remove(Me)
                Exit For
            End If
            openForm.Top += Me.Height + 5
        Next
    End Sub

    Private Sub lifeTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles lifeTimer.Tick
        If Not InputBox.Focused Then
            Label1.Visible = RemainTime > 0 And ShowRemainTime
            Select Case RemainTime
                Case 0
                    Me.lifeTimer.Stop()
                    Me.animator.Direction = CloseDirection
                    Me.Close()
                Case Is < 0
                    Me.lifeTimer.Stop()
                Case Else
                    RemainTime.Decrement
                    Label1.Text = RemainTime + 1
            End Select
        End If
    End Sub

    Private Sub NotificationForm_TextChanged(sender As Object, e As EventArgs) Handles Me.TextChanged, lifeTimer.Tick
        MessageLabel.Text = Me.Text.Replace("##RemainTime##", Me.Label1.Text)
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
        Me.animator.Direction = CloseDirection
        If Me.RemainTime = 0 Then
            Me.Close()
        Else
            Me.RemainTime = 0
        End If

    End Sub

    Private Sub NotificationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Display the form just above the system tray.
        CorrectPos = New Point(Screen.PrimaryScreen.WorkingArea.Width - Me.Width - 5, Screen.PrimaryScreen.WorkingArea.Height - Me.Height - 5)
        Me.Location = CorrectPos
    End Sub

    Private OnInputBoxTextActions As New List(Of EventHandler)
    Private OnOKButtonClickActions As New List(Of EventHandler)

    Public Custom Event OnOKButtonClick As EventHandler
        AddHandler(ByVal value As EventHandler)
            If Not OnOKButtonClickActions.Contains(value) Then
                OnOKButtonClickActions.Add(value)
            End If
            Me.CloseButton.Visible = OnOKButtonClickActions.Count > 0
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            If OnOKButtonClickActions.Contains(value) Then
                OnOKButtonClickActions.Remove(value)
            End If
            Me.CloseButton.Visible = OnOKButtonClickActions.Count > 0
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
            If OnOKButtonClickActions.Count > 0 Then
                For Each handler As EventHandler In OnOKButtonClickActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            Else
                Me.Close()
            End If
        End RaiseEvent
    End Event

    Public Custom Event OnInputBoxTextChanged As EventHandler
        AddHandler(ByVal value As EventHandler)
            If Not OnInputBoxTextActions.Contains(value) Then
                OnInputBoxTextActions.Add(value)
            End If
        End AddHandler

        RemoveHandler(ByVal value As EventHandler)
            If OnInputBoxTextActions.Contains(value) Then
                OnInputBoxTextActions.Remove(value)
            End If
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
            For Each handler As EventHandler In OnInputBoxTextActions
                Try
                    handler.Invoke(sender, e)
                Catch ex As Exception
                    Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                End Try
            Next
        End RaiseEvent
    End Event

    Private Sub InputBox_KeyDown(sender As Object, e As KeyEventArgs) Handles InputBox.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                OKButton.PerformClick()
            Case Keys.Escape
                If InputBox.Text.IsBlank Then
                    Me.Close()
                Else
                    InputBox.Clear()
                End If
        End Select
    End Sub

End Class

''' <summary>
''' Comportamento do contador da notificação
''' </summary>
Public Enum RemainTimeBehavior

    ''' <summary>
    ''' Adiciona segundos ao total de segundos restantes se a notificação já estiver sendo exibida
    ''' </summary>
    StackTime

    ''' <summary>
    ''' Atribui o valor especificado aos do segundos restantes se a notificação já estiver sendo exibida
    ''' </summary>
    ResetTime

    ''' <summary>
    ''' Não altera o tempo restante da notificação
    ''' </summary>
    None

End Enum