Imports System.Windows.Forms
Imports InnerLibs

Namespace TimeMachine

    Public Class Stopwatch
        Inherits Cronometer

        ''' <summary>
        ''' Valor inicial do contador
        ''' </summary>
        ''' <returns></returns>
        Property InitialTime As TimeSpan

        ''' <summary>
        ''' Declara um novo objeto Stopwatch
        ''' </summary>
        ''' <param name="InitialTime">Tempo inicial</param>
        ''' <param name="Format">     
        ''' Formato do Contador( <see cref="TimeSpan.ToString(String)"/> )
        ''' </param>
        Sub New(InitialTime As TimeSpan, Optional Format As String = "", Optional Interval As Long = 100)
            MyBase.New(Format, Interval)
            Me.InitialTime = InitialTime
        End Sub

        ''' <summary>
        ''' Texto atual do contador
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.ToString(Format.IfBlank(Me.Format).IfBlank("hh\:mm\:ss\.fff"))
        End Function

        ''' <summary>
        ''' Texto atual do contador
        ''' </summary>
        ''' Formato do contador(
        ''' <see cref="TimeSpan.ToString(String)"/>
        ''' )
        ''' <returns></returns>
        Public Overloads Function ToString(Format As String) As String
            Return Value.ToString(Format.IfBlank(Me.Format).IfBlank("hh\:mm\:ss\.fff"))
        End Function

        ''' <summary>
        ''' Retorna o valor atual do contador
        ''' </summary>
        ''' <returns></returns>
        Public Shadows ReadOnly Property Value As TimeSpan
            Get
                Dim t = InitialTime - MyBase.Value
                Dim en = Enabled
                If t = TimeSpan.Zero Then
                    Me.Stop()
                    MyBase._value = 0
                    If en Then RaiseEvent OnFinish(Me, New LapEventArgs With {.DateValue = Now, .Value = t})
                End If
                Return t
            End Get
        End Property

        Private FinishActions As New List(Of EventHandler)

        ''' <summary>
        ''' Ocorre toda vez que o contador chegar a zero
        ''' </summary>
        Public Custom Event OnFinish As EventHandler

            AddHandler(ByVal value As EventHandler)
                If Not FinishActions.Contains(value) Then
                    FinishActions.Add(value)
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If FinishActions.Contains(value) Then
                    FinishActions.Remove(value)
                End If
            End RemoveHandler

            RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
                For Each handler As EventHandler In FinishActions
                    Try
                        handler.Invoke(sender, e)
                    Catch ex As Exception
                        Debug.WriteLine("Exception while invoking event handler: " & ex.ToString())
                    End Try
                Next
            End RaiseEvent
        End Event

    End Class

End Namespace