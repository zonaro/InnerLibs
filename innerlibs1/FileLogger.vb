Imports System.IO

Public Class FileLogger
    Inherits List(Of LogEntry)

    Private _file As FileInfo

    ''' <summary>
    ''' Local do arquivo de log
    ''' </summary>
    ''' <returns></returns>
    Property File As FileInfo
        Get
            Return _file
        End Get
        Set(value As FileInfo)
            If File.Exists Then
                File.MoveTo(value.FullName)
            End If
            _file = value
        End Set
    End Property
    ''' <summary>
    ''' Adiciona uma Entrada ao Log
    ''' </summary>
    ''' <param name="Title"></param>
    ''' <param name="Message"></param>
    Shadows Sub Add(Title As String, Message As String)
        MyBase.Add(New LogEntry(Title, Message, Now.ToString))
        Me.SerializeJSON.WriteToFile(File, False)
    End Sub

    Private Shadows Sub Add(Title As String, Message As String, DateTime As String)
        MyBase.Add(New LogEntry(Title, Message, DateTime))
        Me.SerializeJSON.WriteToFile(File, False)
    End Sub

    Shadows Sub AddRange(Entries As LogEntry())
        Throw New Exception("Cant add Multiple logs at same time.")
    End Sub

    ''' <summary>
    ''' Inicia uma nova instancia de Log
    ''' </summary>
    ''' <param name="File">Arquivo</param>
    Sub New(File As FileInfo)
        Me.File = File
        If File.Exists Then
            Dim txt = File.ReadText.ParseJSON(Of Object)
            For Each i In txt
                Me.Add(i("Title"), i("Message"), i("DateTime"))
            Next
        End If
    End Sub
    ''' <summary>
    ''' Inicia uma nova instancia de Log
    ''' </summary>
    ''' <param name="Path">Caminho do arquivo</param>
    Sub New(Path As String)
        Me.New(New FileInfo(Path))
    End Sub

    Class LogEntry
        Friend Sub New(Title As String, Message As String, Datetime As String)
            Me.Title = Title
            Me.Message = Message
            Me.DateTime = Datetime
        End Sub

        ReadOnly Property Title As String
        ReadOnly Property Message As String
        ReadOnly Property DateTime As String = Now.ToString
    End Class
End Class


