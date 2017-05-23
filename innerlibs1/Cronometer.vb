Imports System.Windows.Forms
Imports InnerLibs

Namespace TimeMachine

    Public Class Cronometer
        Inherits Label
        Private timer As New Timer

        Private marks As New List(Of Date)
        Property Format As String = "hh\:mm\:ss\.ffff"

        ''' <summary>
        ''' Texto atual do cronometro
        ''' </summary>
        ''' <returns></returns>
        Public Shadows Property Text As String
            Get
                Return MyBase.Text
            End Get
            Set(value As String)
                Try
                    Try
                        MyBase.Text = TimeSpan.Parse(value).ToString(Format)
                    Catch ex As Exception
                        MyBase.Text = TimeSpan.Parse(value).ToString("hh\:mm\:ss\.ffff")
                    End Try
                Catch
                    Throw New ArgumentException("Value is not a TimeSpan string")
                End Try
            End Set
        End Property

        ReadOnly Property Laps As List(Of Date)
            Get
                Return New List(Of Date)(marks)
            End Get
        End Property

        Function GetLaps(Optional Format As String = "") As List(Of String)
            GetLaps = New List(Of String)
            For Each d In Laps
                GetLaps.Add(d.ToString(Format.IfBlank(Me.Format)))
            Next
            Return GetLaps
        End Function

        ReadOnly Property TotalTime As TimeFlow
            Get
                If marks.Count > 0 Then
                    Return GetDifference(marks.First, marks.Last)
                End If
                Return Nothing
            End Get
        End Property

        Sub New(Optional Format As String = "")
            MyBase.New()
            timer.Interval = 1
            AddHandler timer.Tick, AddressOf IncrementTick
            Me.Format = Format.IfBlank(Me.Format)
            Me.Text = TimeSpan.Zero.ToString(Format)
        End Sub

        ''' <summary>
        ''' Retorna o valor atual do cronometro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Value As TimeSpan
            Get
                Select Case True
                    Case timer.Enabled AndAlso marks.Count > 0
                        Return New TimeSpan(Now.Ticks - marks.First.Ticks)
                    Case timer.Enabled = False AndAlso marks.Count > 0
                        Return New TimeSpan(marks.Last.Ticks - marks.First.Ticks)
                    Case Else
                        Return TimeSpan.Zero
                End Select
            End Get
        End Property

        Private Sub IncrementTick(sender As Object, e As EventArgs)
            RaiseEvent Tick(sender, e)
        End Sub

        ''' <summary>
        ''' Inicia o cronometro
        ''' </summary>
        Public Sub Start()

            Reset()
            Dim e = New LapEventArgs With {.Value = Now}
            marks.Add(e.Value)
            timer.Start()
            RaiseEvent OnStart(Me, e)
        End Sub

        ''' <summary>
        ''' Marca um valor no cronometro
        ''' </summary>
        Public Sub Lap()
            Dim e = New LapEventArgs With {.Value = Now}
            If timer.Enabled Then
                marks.Add(e.Value)
            End If
            RaiseEvent OnLap(Me, e)
        End Sub

        ''' <summary>
        ''' Reinicia o cronometro
        ''' </summary>
        Sub Reset()

            Me.Text = TimeSpan.Zero.ToString(Format)
            marks = New List(Of Date)
            RaiseEvent OnReset(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Para o cronometro
        ''' </summary>
        Public Sub [Stop]()
            If timer.Enabled Then

                Dim e = New LapEventArgs With {.Value = Now}
                marks.Add(e.Value)
                timer.Stop()
                RaiseEvent OnStop(Me, e)
            End If
        End Sub

        Private TickActions As New List(Of EventHandler)
        Private StartActions As New List(Of EventHandler)
        Private StopActions As New List(Of EventHandler)
        Private ResetActions As New List(Of EventHandler)
        Private LapActions As New List(Of EventHandler)

        Public Custom Event Tick As EventHandler
            AddHandler(ByVal value As EventHandler)
                If Not TickActions.Contains(value) Then
                    TickActions.Add(value)
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If TickActions.Contains(value) Then
                    TickActions.Remove(value)
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)

                Me.Text = Value.ToString(Format)
                For Each handler As EventHandler In TickActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            End RaiseEvent
        End Event

        Public Custom Event OnLap As EventHandler
            AddHandler(ByVal value As EventHandler)
                If Not LapActions.Contains(value) Then
                    LapActions.Add(value)
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If LapActions.Contains(value) Then
                    LapActions.Remove(value)
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                For Each handler As EventHandler In LapActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            End RaiseEvent
        End Event

        Public Custom Event OnStart As EventHandler
            AddHandler(ByVal value As EventHandler)
                If Not StartActions.Contains(value) Then
                    StartActions.Add(value)
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If StartActions.Contains(value) Then
                    StartActions.Remove(value)
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                For Each handler As EventHandler In StartActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            End RaiseEvent
        End Event

        Public Custom Event OnStop As EventHandler
            AddHandler(ByVal value As EventHandler)
                If Not StopActions.Contains(value) Then
                    StopActions.Add(value)
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If StopActions.Contains(value) Then
                    StopActions.Remove(value)
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                For Each handler As EventHandler In StopActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            End RaiseEvent
        End Event

        Public Custom Event OnReset As EventHandler
            AddHandler(ByVal value As EventHandler)
                If Not ResetActions.Contains(value) Then
                    ResetActions.Add(value)
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If ResetActions.Contains(value) Then
                    ResetActions.Remove(value)
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                For Each handler As EventHandler In StopActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            End RaiseEvent
        End Event

    End Class

    Public Class LapEventArgs
        Inherits EventArgs
        Property Value As DateTime
    End Class

End Namespace