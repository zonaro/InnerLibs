Imports System.ComponentModel
Imports System.Windows.Forms
Imports InnerLibs

Namespace TimeMachine

    Public Class Cronometer
        Inherits Timer

        Private marks As New List(Of TimeSpan)

        ''' <summary>
        ''' Formato do Cronometro
        ''' </summary>
        ''' <returns></returns>
        Property Format As String = "hh\:mm\:ss\.fff"

        ''' <summary>
        ''' Texto atual do cronometro
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.ToString(Format.IfBlank(Me.Format).IfBlank("hh\:mm\:ss\.fff"))
        End Function

        ''' <summary>
        ''' Texto atual do cronometro
        ''' </summary>
        ''' Formato do Cronometro(
        ''' <see cref="TimeSpan.ToString(String)"/>
        ''' )
        ''' <returns></returns>
        Public Overloads Function ToString(Format As String) As String
            Return Value.ToString(Format.IfBlank(Me.Format).IfBlank("hh\:mm\:ss\.fff"))
        End Function

        ''' <summary>
        ''' Lista de <see cref="DateTime"/> dos valores de cada Lap
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Laps As List(Of TimeSpan)
            Get
                Return New List(Of TimeSpan)(marks)
            End Get
        End Property

        ''' <summary>
        ''' Retorna uma lista de strings da extraidas da <see cref="Cronometer.Laps"/> em um formato
        ''' especifico de data
        ''' </summary>
        ''' Formato do Cronometro(
        ''' <see cref="TimeSpan.ToString(String)"/>
        ''' )
        ''' <returns></returns>
        Function GetLaps(Optional Format As String = "") As List(Of String)
            GetLaps = New List(Of String)
            For Each d In Laps
                GetLaps.Add(d.ToString(Format.IfBlank(Me.Format)))
            Next
            Return GetLaps
        End Function

        ''' <summary>
        ''' Retorna um <see cref="TimeFlow"/> calculado para este cronometro.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ToTimeFlow As TimeFlow
            Get
                Return New TimeFlow(Value)
            End Get
        End Property

        ''' <summary>
        ''' Declara um novo objeto Cronometer
        ''' </summary>
        ''' <param name="Format">
        ''' Formato do Cronometro( <see cref="TimeSpan.ToString(String)"/> )
        ''' </param>
        Public Sub New(Optional Format As String = "", Optional Interval As Long = 100)
            MyBase.New()
            MyBase.Interval = Interval
            AddHandler Me.Tick, AddressOf IncrementTick
            Me.Format = Format.IfBlank(Me.Format)
        End Sub

        Protected Friend Sub IncrementTick(sender As Object, e As EventArgs)
            _value.Increment(MyBase.Interval)
        End Sub

        ''' <summary>
        ''' Retorna o valor atual do cronometro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Value As TimeSpan
            Get
                Return TimeSpan.FromMilliseconds(_value)
            End Get
        End Property

        Protected Friend _value As Long = 0

        ''' <summary>
        ''' Inicia o cronometro
        ''' </summary>
        Public Shadows Sub Start()
            Dim e = New LapEventArgs With {.Value = TimeSpan.Zero, .DateValue = Now}
            If marks.Count = 0 Then marks.Add(e.Value)
            MyBase.Start()
            RaiseEvent OnStart(Me, e)
        End Sub

        ''' <summary>
        ''' Renicia o cronometro. é o equivalente em chamar <see cref="Reset"/> e <see cref="Start"/>
        ''' </summary>
        Public Shadows Sub StartOver()
            Me.Reset()
            Me.Start()
        End Sub

        ''' <summary>
        ''' Marca um valor no cronometro
        ''' </summary>
        Public Sub Lap()
            If Me.Enabled Then
                Dim e = New LapEventArgs With {.Value = Me.Value, .DateValue = Now}
                marks.Add(e.Value)
                RaiseEvent OnLap(Me, e)
            End If
        End Sub

        ''' <summary>
        ''' Limpa os valores do cronometro
        ''' </summary>
        Sub Reset()
            _value = 0
            marks = New List(Of TimeSpan)
            RaiseEvent OnReset(Me, EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Para o cronometro
        ''' </summary>
        Public Shadows Sub [Stop]()
            If Me.Enabled Then
                Dim e = New LapEventArgs With {.Value = Me.Value, .DateValue = Now}
                marks.Add(e.Value)
                MyBase.Stop()
                RaiseEvent OnStop(Me, e)
            End If
        End Sub

        Private StartActions As New List(Of EventHandler)
        Private StopActions As New List(Of EventHandler)
        Private ResetActions As New List(Of EventHandler)
        Private LapActions As New List(Of EventHandler)

        ''' <summary>
        ''' ocorre toda vez que a função <see cref="Lap"/> é chamada
        ''' </summary>
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
                RaiseEvent OnChange(sender, e)

            End RaiseEvent
        End Event

        ''' <summary>
        ''' Ocorre toda vez que o cronometro inicia ( <see cref="Start"/>)
        ''' </summary>
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
                RaiseEvent OnChange(sender, e)

            End RaiseEvent
        End Event

        ''' <summary>
        ''' Ocorre toda vez que o cronometro para ( <see cref="[Stop]"/>)
        ''' </summary>
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
                RaiseEvent OnChange(sender, e)

            End RaiseEvent
        End Event

        ''' <summary>
        ''' Ocorre toda vez que o cronometro Reinicia <see cref="Reset"/>)
        ''' </summary>
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
                RaiseEvent OnChange(sender, e)

            End RaiseEvent
        End Event

        ''' <summary>
        ''' Ocorre toda vez que o cronometro iniciar, mudar de valor, parar, reiniciar ou marcar uma volta
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e">     </param>
        Public Event OnChange(ByVal sender As Object, ByVal e As EventArgs)

    End Class

    Public Class LapEventArgs
        Inherits EventArgs
        Property Value As TimeSpan
        Property DateValue As Date
    End Class

End Namespace