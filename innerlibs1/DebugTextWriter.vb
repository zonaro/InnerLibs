Imports System.IO
Imports System.Text

Public Class DebugTextWriter
    Inherits StreamWriter

    Public Sub New()
        MyBase.New(New DebugOutStream(), Encoding.Unicode, 1024)
        Me.AutoFlush = True
    End Sub



    NotInheritable Class DebugOutStream
        Inherits Stream

        Public Overrides Sub Write(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            Debug.Write(Encoding.Unicode.GetString(buffer, offset, count))
        End Sub

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return True
            End Get
        End Property



        Public Overrides ReadOnly Property Length As Long
            Get
                Throw bad_op
            End Get
        End Property

        Public Overrides Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Throw bad_op
        End Function

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
            Throw bad_op
        End Function

        Public Overrides Sub SetLength(ByVal value As Long)
            Throw bad_op
        End Sub

        Public Overrides Sub Flush()
            Debug.Flush()
        End Sub

        Public Overrides Property Position As Long
            Get
                Throw bad_op
            End Get
            Set(ByVal value As Long)
                Throw bad_op
            End Set
        End Property

        Private Shared ReadOnly Property bad_op As InvalidOperationException
            Get
                Return New InvalidOperationException()
            End Get
        End Property


    End Class


End Class

