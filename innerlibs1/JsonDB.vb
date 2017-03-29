
Imports System.Collections.Generic.Dictionary(Of String, Object)
Imports System.IO

Namespace JsonDB


    Public Class JsonFile
        Inherits Dictionary(Of String, JsonTable)

        Property File As FileInfo


        Property Encrypt As Boolean = False

        Sub New(Path As String)
            Me.New(New FileInfo(Path))
        End Sub
        Sub New(File As FileInfo)
            If Not File.Exists Then
                File.Create.Dispose()
            End If
            Me.File = File
        End Sub

        Sub New()

        End Sub

        Public Sub CreateTable(Of Type)(TableName As String, ParamArray Values As Type())
            Dim nt As New JsonTable(GetType(Type))
            If Me.Where(Function(t) t.Key = TableName).Count = 0 Then
                Me.Add(TableName, nt)
            End If
            If Not IsNothing(Values) AndAlso Values.Count > 0 Then
                Insert(TableName, Values)
            End If
        End Sub

        Public Function TableAs(Of Type)(TableName As String) As List(Of Type)
            Dim lista As New List(Of Type)
            For Each i In Me.Item(TableName).Values
                lista.Add(DirectCast(i.Value, Type))
            Next
            Return lista
        End Function

        Public Shadows Sub INSERT(Of Type)(TableName As String, ParamArray Values As Type())
            Dim table = Me.Where(Function(p) p.Key = TableName)
            If table.Count > 0 Then
                For Each v In Values
                    Dim id = 0
                    If table(0).Value.Values.Count > 0 Then
                        id = (table(0).Value.Values.Last.Key.To(Of Integer) + 1).ToString
                    End If
                    table(0).Value.Values.Add(id, v)
                Next
            End If
            Me.Item(table(0).Key) = table(0).Value
        End Sub

        Public Function DROP(TableName As String) As Boolean
            Try
                Me.Remove(TableName)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function [SELECT](Of Type)(TableName As String, Where As Func(Of KeyValuePair(Of String, Object), Boolean))
            Return DirectCast(Me.Item(TableName).Values.Where(Where), IEnumerable(Of Type))
        End Function

        Function Save() As FileInfo
            Return Me.SerializeJSON.WriteToFile(File.FullName)
        End Function

        Sub Load()
            Using jota = File.OpenText
                Dim tabelas = jota.ReadToEnd.ParseJSON(Of Object)
                For Each tabela In tabelas
                    Dim nt As New JsonTable(tabela.Value("ClassName").ToString)
                    Dim entradas = tabela.Value("Values")
                    For Each e In entradas
                        Dim par As KeyValuePair(Of String, Object) = e
                        nt.Values.Add(par.Key, par.Value)
                    Next
                    If Me.ContainsKey(tabela.Key) Then
                        Me.Remove(tabela.Key)
                    End If
                    Me.Add(tabela.Key, nt)
                Next
            End Using
        End Sub

    End Class

    Public Class JsonTable
        Property ClassName As String
        Property Values As New Dictionary(Of String, Object)
        Sub New(Type As Type)
            ClassName = Type.FullName
        End Sub
        Sub New(Type As String)
            ClassName = Type
        End Sub

        Sub Load()
            'terminar
        End Sub

    End Class
End Namespace