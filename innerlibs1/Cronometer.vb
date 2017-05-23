Imports System.Windows.Forms
Imports InnerLibs

Public Class Cronometer

    Private timer As New Timer
    Private tick As Long = 0

    Private Marks As New List(Of Date)

    Sub New()
        AddHandler timer.Tick, AddressOf IncrementTick
    End Sub

    ReadOnly Property Ticks As TimeSpan
        Get
            Return New TimeSpan(tick)
        End Get
    End Property

    Private Sub IncrementTick(sender As Object, e As EventArgs)

        tick.Increment
    End Sub

    Public Function Start() As Date
        Reset()
        Marks.Add(Now)
        timer.Start()
        Return Marks.First
    End Function

    Public Function Lap() As Date
        Dim d = Now
        Marks.Add(d)
        Return d
    End Function

    Sub Reset()
        Marks = New List(Of Date)
        tick = 0
    End Sub

    ''' <summary>
    ''' Para o cronometro
    ''' </summary>
    ''' <returns></returns>
    Public Function [Stop]() As Date
        Marks.Add(Now)
        Return Marks.Last
    End Function

    Private Actions As New Dictionary(Of Long, EventHandler)

    Sub FireFunctionOn(Miliseconds As Long, Action As EventHandler)
        If Actions.ContainsKey(Miliseconds) Then
            FireFunctionOn(Miliseconds.Increment, Action)
        Else
            Actions.Add(Miliseconds.Increment, Action)
        End If
    End Sub

End Class