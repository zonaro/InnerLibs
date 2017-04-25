Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Public Module DataGridViewManager

    ''' <summary>
    ''' Cria as colunas e linhas de um DataGridView de acordo com uma lista de um objeto do mesmo tipo
    ''' </summary>
    ''' <param name="DataGridView">DataGridView de Destino</param>
    ''' <param name="MyObject">Lista de Itens do mesmo Tipo</param>
    ''' <typeparam name="Type">Tipo de Item</typeparam>
    <Extension()> Public Function BuildFromClass(Of Type)(ByRef DataGridView As DataGridView, MyObject As List(Of Type))
        Dim o As List(Of PropertyInfo) = MyObject(0).GetProperties()

        DataGridView.Rows.Clear()
        DataGridView.Columns.Clear()

        For Each prop As PropertyInfo In o
            If prop.CanRead Then
                DataGridView.Columns.Add(DataGridView.Name & "_" & prop.Name, prop.Name)
            End If
        Next
        For Each item In MyObject
            Dim valores As New List(Of Object)
            For Each colum As DataGridViewColumn In DataGridView.Columns
                Dim prop = o(colum.Index)
                If prop.CanRead Then
                    valores.Add(prop.GetValue(item))
                End If
            Next
            DataGridView.Rows.Add(valores.ToArray())
        Next
        Return DataGridView
    End Function

    <Extension()> Public Function BuildFromJSON(Of Type)(ByRef DataGridView As DataGridView, JSON As String)
        Return BuildFromClass(Of Type)(DataGridView, JSON.ParseJSON(Of List(Of Type)))
    End Function

    <Extension()> Public Function BuildFromAJAX(Of Type)(ByRef DataGridView As DataGridView, URL As String)
        Return BuildFromClass(Of Type)(DataGridView, AJAX.GET(Of List(Of Type))(URL))
    End Function


End Module
