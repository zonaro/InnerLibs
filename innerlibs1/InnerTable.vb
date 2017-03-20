
Public Class InnerTable
    Inherits List(Of List(Of Object))

    Property Columns As New List(Of String)


    Public Sub New(ParamArray Columns() As String)
        Me.Columns.AddRange(Columns)
    End Sub

    Public Function Cell(Column As String, RowIndex As Integer) As Object
        Return Me(RowIndex)(Columns.IndexOf(Column))
    End Function

    Public Overloads Sub Add(ParamArray Values() As Object)
        If Values.Count > Columns.Count Then
            Throw New IndexOutOfRangeException("Valores ultrapassam o numero de colunas!")
        Else
            Dim lista As New List(Of Object)
            lista.AddRange(Values)
            MyBase.Add(lista)
        End If
    End Sub

    Public Function ToJSON()
        Return Me.SerializeJSON
    End Function
End Class
