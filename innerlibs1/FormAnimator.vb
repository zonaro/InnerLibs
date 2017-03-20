Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

''' <summary>
''' Animates a form when it is shown, hidden or closed.
''' </summary>
''' <remarks>
''' MDI child forms do not support the Blend method and only support other methods while being displayed for the first time and when closing.
''' </remarks>
Public NotInheritable Class FormAnimator

#Region " Types "

    ''' <summary>
    ''' The methods of animation available.
    ''' </summary>
    Public Enum AnimationMethod
        ''' <summary>
        ''' Rolls out from edge when showing and into edge when hiding.
        ''' </summary>
        ''' <remarks>
        ''' This is the default animation method and requires a direction.
        ''' </remarks>
        Roll = &H0
        ''' <summary>
        ''' Expands out from centre when showing and collapses into centre when hiding.
        ''' </summary>
        Centre = &H10
        ''' <summary>
        ''' Slides out from edge when showing and slides into edge when hiding.
        ''' </summary>
        ''' <remarks>
        ''' Requires a direction.
        ''' </remarks>
        Slide = &H40000
        ''' <summary>
        ''' Fades from transaprent to opaque when showing and from opaque to transparent when hiding.
        ''' </summary>
        Fade = &H80000
    End Enum

    ''' <summary>
    ''' The directions in which the Roll and Slide animations can be shown.
    ''' </summary>
    ''' <remarks>
    ''' Horizontal and vertical directions can be combined to create diagonal animations.
    ''' </remarks>
    <Flags()> Public Enum AnimationDirection
        ''' <summary>
        ''' From left to right.
        ''' </summary>
        Right = &H1
        ''' <summary>
        ''' From right to left.
        ''' </summary>
        Left = &H2
        ''' <summary>
        ''' From top to bottom.
        ''' </summary>
        Down = &H4
        ''' <summary>
        ''' From bottom to top.
        ''' </summary>
        Up = &H8
    End Enum

#End Region 'Types

#Region " Constants "

    ''' <summary>
    ''' Hide the form.
    ''' </summary>
    Private Const AW_HIDE As Integer = &H10000
    ''' <summary>
    ''' Activate the form.
    ''' </summary>
    Private Const AW_ACTIVATE As Integer = &H20000

    ''' <summary>
    ''' The number of milliseconds over which the animation occurs if no value is specified.
    ''' </summary>
    Private Const DEFAULT_DURATION As Integer = 250

#End Region 'Constants

#Region " Variables "

    ''' <summary>
    ''' The form to be animated.
    ''' </summary>
    Private WithEvents _form As Form
    ''' <summary>
    ''' The animation method used to show and hide the form.
    ''' </summary>
    Private _method As AnimationMethod
    ''' <summary>
    ''' The direction in which to Roll or Slide the form.
    ''' </summary>
    Private _direction As AnimationDirection
    ''' <summary>
    ''' The number of milliseconds over which the animation is played.
    ''' </summary>
    Private _duration As Integer

#End Region 'Variables

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the animation method used to show and hide the form.
    ''' </summary>
    ''' <value>
    ''' The animation method used to show and hide the form.
    ''' </value>
    ''' <remarks>
    ''' <b>Roll</b> is used by default if no method is specified.
    ''' </remarks>
    Public Property Method() As AnimationMethod
        Get
            Return Me._method
        End Get
        Set(ByVal Value As AnimationMethod)
            Me._method = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the direction in which the animation is performed.
    ''' </summary>
    ''' <value>
    ''' The direction in which the animation is performed.
    ''' </value>
    ''' <remarks>
    ''' The direction is only applicable to the <b>Roll</b> and <b>Slide</b> methods.
    ''' </remarks>
    Public Property Direction() As AnimationDirection
        Get
            Return Me._direction
        End Get
        Set(ByVal Value As AnimationDirection)
            Me._direction = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the number of milliseconds over which the animation is played.
    ''' </summary>
    ''' <value>
    ''' The number of milliseconds over which the animation is played.
    ''' </value>
    Public Property Duration() As Integer
        Get
            Return Me._duration
        End Get
        Set(ByVal Value As Integer)
            Me._duration = Value
        End Set
    End Property

    ''' <summary>
    ''' Gets the form to be animated.
    ''' </summary>
    ''' <value>
    ''' The form to be animated.
    ''' </value>
    Public ReadOnly Property Form() As Form
        Get
            Return Me._form
        End Get
    End Property

#End Region 'Properties

#Region " APIs "

    ''' <summary>
    ''' Windows API function to animate a window.
    ''' </summary>
    <DllImport("user32")> _
    Private Shared Function AnimateWindow(ByVal hWnd As IntPtr, _
                                          ByVal dwTime As Integer, _
                                          ByVal dwFlags As Integer) As Boolean
    End Function

#End Region 'APIs

#Region " Constructors "

    ''' <summary>
    ''' Creates a new <b>FormAnimator</b> object for the specified form.
    ''' </summary>
    ''' <param name="form">
    ''' The form to be animated.
    ''' </param>
    ''' <remarks>
    ''' No animation will be used unless the <b>Method</b> and/or <b>Direction</b> properties are set independently. The <b>Duration</b> is set to quarter of a second by default.
    ''' </remarks>
    Public Sub New(ByVal form As Form)
        Me._form = form
        Me._duration = DEFAULT_DURATION
    End Sub

    ''' <summary>
    ''' Creates a new <b>FormAnimator</b> object for the specified form using the specified method over the specified duration.
    ''' </summary>
    ''' <param name="form">
    ''' The form to be animated.
    ''' </param>
    ''' <param name="method">
    ''' The animation method used to show and hide the form.
    ''' </param>
    ''' <param name="duration">
    ''' The number of milliseconds over which the animation is played.
    ''' </param>
    ''' <remarks>
    ''' No animation will be used for the <b>Roll</b> or <b>Slide</b> methods unless the <b>Direction</b> property is set independently.
    ''' </remarks>
    Public Sub New(ByVal form As Form, _
                   ByVal method As AnimationMethod, _
                   ByVal duration As Integer)
        Me.New(form)

        Me._method = method
        Me._duration = duration
    End Sub

    ''' <summary>
    ''' Creates a new <b>FormAnimator</b> object for the specified form using the specified method in the specified direction over the specified duration.
    ''' </summary>
    ''' <param name="form">
    ''' The form to be animated.
    ''' </param>
    ''' <param name="method">
    ''' The animation method used to show and hide the form.
    ''' </param>
    ''' <param name="direction">
    ''' The direction in which to animate the form.
    ''' </param>
    ''' <param name="duration">
    ''' The number of milliseconds over which the animation is played.
    ''' </param>
    ''' <remarks>
    ''' The <i>direction</i> argument will have no effect if the <b>Centre</b> or <b>Blend</b> method is
    ''' specified.
    ''' </remarks>
    Public Sub New(ByVal form As Form, _
                   ByVal method As AnimationMethod, _
                   ByVal direction As AnimationDirection, _
                   ByVal duration As Integer)
        Me.New(form, method, duration)

        Me._direction = direction
    End Sub

#End Region 'Constructors

#Region " Event Handlers "

    ''' <summary>
    ''' Animates the form automatically when it is loaded.
    ''' </summary>
    Private Sub Form_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles _form.Load
        'MDI child forms do not support transparency so do not try to use the Blend method.
        If Me._form.MdiParent Is Nothing OrElse Me._method <> AnimationMethod.Fade Then
            'Activate the form.
            Me.AnimateWindow(Me._form.Handle,
                             Me._duration,
                             AW_ACTIVATE Or Me._method Or Me._direction)
        End If
    End Sub

    ''' <summary>
    ''' Animates the form automatically when it is shown or hidden.
    ''' </summary>
    Private Sub Form_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _form.VisibleChanged
        'Do not attempt to animate MDI child forms while showing or hiding as they do not behave as expected.
        If Me._form.MdiParent Is Nothing Then
            Dim flags As Integer = Me._method Or Me._direction

            If Me._form.Visible Then
                'Activate the form.
                flags = flags Or AW_ACTIVATE
            Else
                'Hide the form.
                flags = flags Or AW_HIDE
            End If

            Me.AnimateWindow(Me._form.Handle, _
                             Me._duration, _
                             flags)
        End If
    End Sub

    ''' <summary>
    ''' Animates the form automatically when it closes.
    ''' </summary>
    Private Sub Form_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _form.Closing
        If Not e.Cancel Then
            'MDI child forms do not support transparency so do not try to use the Blend method.
            If Me._form.MdiParent Is Nothing OrElse Me._method <> AnimationMethod.Fade Then
                'Hide the form.
                Me.AnimateWindow(Me._form.Handle,
                                 Me._duration,
                                 AW_HIDE Or Me._method Or Me._direction)
            End If
        End If
    End Sub

#End Region 'Event Handlers

End Class
