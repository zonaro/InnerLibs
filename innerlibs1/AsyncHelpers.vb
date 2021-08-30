Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks

Namespace HESBrowser
    Public Module AsyncHelpers
        ''' <summary>
        ''' Execute's an async Task<T> method which has a void return value synchronously
        ''' </summary>
        ''' <paramname="task">Task<T> method to execute</param></param></summary>       
        Public Sub RunSync(ByVal task As Func(Of Task))
            Dim oldContext = SynchronizationContext.Current
            Dim synch = New ExclusiveSynchronizationContext()
            SynchronizationContext.SetSynchronizationContext(synch)
            synch.Post(Async Sub(__)
                           Try
                               Await task()
                           Catch e As Exception
                               synch.InnerException = e
                               Throw
                           Finally
                               synch.EndMessageLoop()
                           End Try
                       End Sub, Nothing)
            synch.BeginMessageLoop()
            SynchronizationContext.SetSynchronizationContext(oldContext)
        End Sub

        ''' <summary>
        ''' Execute's an async Task<T> method which has a T return type synchronously
        ''' </summary>
        ''' <typeparamname="T">Return Type</typeparam>
        ''' <paramname="task">Task<T> method to execute</param>
        ''' <returns></returns></param></summary>       

        Public Function RunSync(Of T)(ByVal task As Func(Of Task(Of T))) As T
            Dim oldContext = SynchronizationContext.Current
            Dim synch = New ExclusiveSynchronizationContext()
            SynchronizationContext.SetSynchronizationContext(synch)
            Dim ret As T = Nothing
            synch.Post(Async Sub(__)
                           Try
                               ret = Await task()
                           Catch e As Exception
                               synch.InnerException = e
                               Throw
                           Finally
                               synch.EndMessageLoop()
                           End Try
                       End Sub, Nothing)
            synch.BeginMessageLoop()
            SynchronizationContext.SetSynchronizationContext(oldContext)
            Return ret
        End Function

        Private Class ExclusiveSynchronizationContext
            Inherits SynchronizationContext

            Private done As Boolean
            Public Property InnerException As Exception
            Private ReadOnly workItemsWaiting As AutoResetEvent = New AutoResetEvent(False)
            Private ReadOnly items As Queue(Of Tuple(Of SendOrPostCallback, Object)) = New Queue(Of Tuple(Of SendOrPostCallback, Object))()

            Public Overrides Sub Send(ByVal d As SendOrPostCallback, ByVal state As Object)
                Throw New NotSupportedException("We cannot send to our same thread")
            End Sub

            Public Overrides Sub Post(ByVal d As SendOrPostCallback, ByVal state As Object)
                SyncLock items
                    items.Enqueue(Tuple.Create(d, state))
                End SyncLock

                workItemsWaiting.Set()
            End Sub

            Public Sub EndMessageLoop()
                Post(Sub(__) done = True, Nothing)
            End Sub

            Public Sub BeginMessageLoop()
                While Not done
                    Dim task As Tuple(Of SendOrPostCallback, Object) = Nothing

                    SyncLock items

                        If items.Count > 0 Then
                            task = items.Dequeue()
                        End If
                    End SyncLock

                    If task IsNot Nothing Then
                        task.Item1(task.Item2)

                        If InnerException IsNot Nothing Then ' the method threw an exeption
                            Throw New AggregateException("AsyncHelpers.Run method threw an exception.", InnerException)
                        End If
                    Else
                        workItemsWaiting.WaitOne()
                    End If
                End While
            End Sub

            Public Overrides Function CreateCopy() As SynchronizationContext
                Return Me
            End Function
        End Class
    End Module
End Namespace
