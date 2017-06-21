Imports System.Drawing
Imports System.Drawing.Text
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

''' <summary>
''' Módulo de controle de formulários
''' </summary>
''' <remarks></remarks>
Public Module WinForms

    ''' <summary>
    ''' Reinicia a aplicação solicitando acesso administrativo se a mesma já não estiver em modo administrativo
    ''' </summary>
    '''<param name="ForceRestart">Força o reinicio da aplicação mesmo se ela estiver em modo administrativo</param>
    Public Sub RestartAsAdmin(Optional ForceRestart As Boolean = False)
        If Not IsRunningAsAdministrator() Or ForceRestart Then
            ' Setting up start info of the new process of the same application
            Dim processStartInfo As New ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase)

            ' Using operating shell and setting the ProcessStartInfo.Verb to “runas” will let it run
            ' as admin
            processStartInfo.UseShellExecute = True
            processStartInfo.Verb = "runas"

            ' Start the application as new process
            Process.Start(processStartInfo)

            ' Shut down the current (old) process
            Application.[Exit]()
        End If
    End Sub

    ''' <summary>
    ''' Exibe uma notificação com uma mensagem
    ''' </summary>
    ''' <param name="Text">           Texto da notiicação</param>
    ''' <param name="Action">         Ação do botão OK</param>
    ''' <param name="OKButtonText">   texto do botão OK</param>
    ''' <param name="Size">           Tamanho do Form</param>
    ''' <param name="LifeTimeSeconds">Tempo e que a notificação demora para fechar automaticamente</param>
    ''' <param name="ShowRemainTime"> Exibir/Esconder o contador da notificação</param>
    Public Function Notify(Text As String, Optional Action As EventHandler = Nothing, Optional OKButtonText As String = "OK", Optional Size As Size = Nothing, Optional LifeTimeSeconds As Integer = 10, Optional ShowRemainTime As Boolean = False, Optional RemainTimeBehavior As RemainTimeBehavior = RemainTimeBehavior.StackTime) As NotificationForm
        Dim n As New NotificationForm()
        n.Text = Text
        n.OKButtonText = OKButtonText
        n.Size = Size
        n.ShowRemainTime = ShowRemainTime
        n.RemainTimeBehavior = RemainTimeBehavior.StackTime
        n.ShowInputBox = False
        AddHandler n.OnOKButtonClick, Action
        n.Show(LifeTimeSeconds)
        AddHandler n.FormClosing, AddressOf n.Dispose
        Return n
    End Function

    ''' <summary>
    ''' Cria uma Font a partir de um arquivo
    ''' </summary>
    ''' <param name="Path">caminho do arquivo</param>
    ''' <returns></returns>
    Public Function CreateFontFromFile(Path As String, Optional Size As Integer = 12, Optional Style As FontStyle = FontStyle.Regular) As Font
        Dim pfc As New PrivateFontCollection
        pfc.AddFontFile(Path)
        Return New Font(pfc.Families(0), Size, Style)
    End Function

    ''' <summary>
    ''' Traz todos os nós descendentes de um nó pai
    ''' </summary>
    ''' <param name="Node">nó</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetAllNodes(Node As TreeNode) As List(Of TreeNode)
        Dim result As New List(Of TreeNode)()
        result.Add(Node)
        For Each child As TreeNode In Node.Nodes
            result.AddRange(child.GetAllNodes())
        Next
        Return result
    End Function

    ''' <summary>
    ''' Traz todos os nós descendentes de um TreeView
    ''' </summary>
    ''' <param name="Node">nó</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetAllNodes(Node As TreeView) As List(Of TreeNode)
        Dim result As New List(Of TreeNode)()
        For Each child As TreeNode In Node.Nodes
            result.AddRange(child.GetAllNodes())
        Next
        Return result
    End Function

    ''' <summary>
    ''' Cria uma Font a partir de um arquivo embutido
    ''' </summary>
    ''' <param name="Resource">Resource (Bytes()) do arquivo</param>
    ''' <returns></returns>
    Public Function CreateFontFromResource(Resource As Byte(), Optional Size As Integer = 12, Optional Style As FontStyle = FontStyle.Regular) As Font
        Dim pfc As New PrivateFontCollection
        pfc.AddFontFromBytes(Resource)
        Return New Font(pfc.Families(0), Size, Style)
    End Function

    ''' <summary>
    ''' Exibe uma caixa de alerta com uma mensagem
    ''' </summary>
    ''' <param name="Message">Texto da caixa de alerta</param>

    Public Sub Alert(Message As String)
        MsgBox(Message, MsgBoxStyle.Exclamation, My.Application.Info.AssemblyName)
    End Sub

    ''' <summary>
    ''' Exibe uma caixa de comfirmação com uma mensagem
    ''' </summary>
    ''' <param name="Message">Texto da caixa de confirmação</param>
    ''' <returns>TRUE ou FALSE</returns>

    Public Function Confirm(Message As String) As Boolean
        Return If(MsgBox(Message, MsgBoxStyle.OkCancel + MsgBoxStyle.Information, My.Application.Info.AssemblyName) = MsgBoxResult.Ok, True, False)
    End Function

    ''' <summary>
    ''' Exibe uma caixa de mensagem ao usuário esperando uma resposta
    ''' </summary>
    ''' <param name="Message">Mensagem String</param>
    ''' <returns></returns>
    Public Function Prompt(Message As String, Optional DefaultText As String = "") As String
        Return "" & InputBox(Message, My.Application.Info.AssemblyName, DefaultText)
    End Function

    ''' <summary>
    ''' Deixa o Form em tela cheia.
    ''' </summary>
    ''' <param name="Form">     O formulario</param>
    ''' <param name="TheScreen">Qual tela o form será aplicado</param>
    <System.Runtime.CompilerServices.Extension>
    Public Sub ToFullScreen(Form As Form, Optional TheScreen As Integer = 0)
        Form.WindowState = FormWindowState.Maximized
        Form.StartPosition = FormStartPosition.Manual
        Form.Bounds = Screen.AllScreens(TheScreen).Bounds
    End Sub

    ''' <summary>
    ''' Aplica máscara de telefone com ou sem o nono dígito automaticamente de acordo com o número
    ''' inputado. Utilize este metodo no Evento GotFocus e LostFocus simultaneamente
    ''' </summary>
    ''' <param name="theTextBox">A MaskedTextBox</param>
    <Extension()>
    Public Sub SetTelephoneMask(TheTextBox As MaskedTextBox)
        Dim txt As String = TheTextBox.Text.RemoveAny("(", "-", ")", " ")
        If txt.Count = 11 Or txt.Count = 0 Or TheTextBox.ContainsFocus Then
            TheTextBox.Mask = "(99) 99999-9999"
            If TheTextBox.ContainsFocus Then
                If txt.Count = 9 Or txt.Count = 8 Then
                    txt = "  " & txt
                End If
            End If
        ElseIf txt.Count = 8 Then
            TheTextBox.Mask = "9999-9999"
        ElseIf txt.Count = 9 Then
            TheTextBox.Mask = "99999-9999"
        Else
            TheTextBox.Mask = "(99) 9999-9999"
        End If
        TheTextBox.Text = txt
    End Sub

    ''' <summary>
    ''' Adiciona funções ao clique de algum controle
    ''' </summary>
    ''' <param name="Control">Controle</param>
    ''' <param name="Action"> Ação</param>
    <Extension()>
    Public Sub AddClick(ByRef Control As Control, Action As EventHandler)
        AddHandler Control.Click, Action
    End Sub

    ''' <summary>
    ''' Remove funções do clique de algum controle
    ''' </summary>
    ''' <param name="Control">Controle</param>
    ''' <param name="Action"> Ação</param>
    <Extension()>
    Public Sub RemoveClick(ByRef Control As Control, Action As EventHandler)
        RemoveHandler Control.Click, Action
    End Sub

    ''' <summary>
    ''' Pega todos os controles filhos de um controle pai
    ''' </summary>
    ''' <typeparam name="ControlType">Tipo de controle</typeparam>
    ''' <param name="Control">Controle Pai</param>
    ''' <returns>Uma lista com os controles</returns>
    <Extension()>
    Public Function GetAllControls(Of ControlType)(Control As System.Windows.Forms.Control) As List(Of ControlType)
        Dim lista As New List(Of ControlType)
        For Each c In Control.Controls
            If c.GetType = GetType(ControlType) Then
                lista.Add(c)
            End If
            If c.HasChildren Then
                lista.AddRange(GetAllControls(Of ControlType)(c))
            End If
        Next
        Return lista
    End Function

    ''' <summary>
    ''' Aplica um valor a um controle dependendo do seu tipo
    ''' </summary>
    ''' <param name="Control">Controle</param>
    ''' <param name="Value">  Valor</param>
    <Extension()> Public Sub CastControl(ByRef Control As Object, Value As Object)
        Select Case Control.GetType
            Case GetType(NumericUpDown), GetType(TrackBar)
                Control.Value = Convert.ToDecimal(Value)
            Case GetType(MonthCalendar)
                Control.SelectionStart = Convert.ToDateTime(Value)
            Case GetType(DateTimePicker)
                Control.Value = Convert.ToDateTime(Value)
            Case GetType(TextBox), GetType(MaskedTextBox), GetType(Form), GetType(RichTextBox), GetType(Label), GetType(ComboBox)
                Control.Text = Convert.ToString(Value)
            Case GetType(RadioButton), GetType(CheckBox)
                Control.Checked = Convert.ToBoolean(Value)
            Case GetType(String)
                Control = DirectCast(Value, String)
            Case GetType(Long)
                Control = DirectCast(Value, Long)
            Case GetType(Integer)
                Control = DirectCast(Value, Integer)
            Case GetType(Decimal)
                Control = DirectCast(Value, Decimal)
            Case GetType(Short)
                Control = DirectCast(Value, Short)
            Case GetType(DateTime)
                Control = DirectCast(Value, DateTime)
            Case GetType(Char)
                Control = DirectCast(Value, Char)
            Case GetType(Char)
                Control = DirectCast(Value, Char)
            Case Else
                Throw New ArgumentException("O controle ou tipo " & Control.GetType.Name & " não é suportado")
        End Select
    End Sub

    ''' <summary>
    ''' Retorna uma string contendo o valor do controle pronto para uma Query SQL dependendo do seu tipo
    ''' </summary>
    ''' <param name="Control">Controle</param>
    ''' <returns></returns>
    <Extension> Public Function GetQueryableValue(ByRef Control As Object) As String
        Select Case Control.GetType
            Case GetType(NumericUpDown), GetType(TrackBar)
                Return Control.Value.ToString
            Case GetType(MonthCalendar)
                Return Convert.ToDateTime(Control.SelectionStart).ToSQLDateString.IsNull
            Case GetType(DateTimePicker)
                Return Convert.ToDateTime(Control.Value).ToSQLDateString.IsNull
            Case GetType(TextBox), GetType(MaskedTextBox), GetType(Form), GetType(RichTextBox), GetType(Label), GetType(ComboBox)
                Return Control.Text.ToString.IsNull
            Case GetType(RadioButton), GetType(CheckBox)
                Return If(Control.Checked, 1, 0)
            Case Else
                Throw New ArgumentException("O controle " & Control.ToString() & " não é suportado")
        End Select
    End Function

End Module