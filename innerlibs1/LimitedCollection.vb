Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

''' <summary>
''' Cria uma coleção Limitada
''' </summary>
''' <typeparam name="TypeCollection">Tipo dos itens da lista</typeparam>
Public Class LimitedCollection(Of TypeCollection)
    Inherits Collection(Of TypeCollection)

    ''' <summary>
    ''' Indica se sua capacidade pode ser alterada
    ''' </summary>
    ''' <returns></returns>
    Public Property AllowCapacityChange As Boolean = True
    ''' <summary>
    ''' Indica se itens podem ser removidos
    ''' </summary>
    ''' <returns></returns>
    Public Property AllowRemoveItem As Boolean = True
    ''' <summary>
    ''' Indica se itens podem ser adicionados
    ''' </summary>
    ''' <returns></returns>
    Public Property AllowAddItem As Boolean = True

    Private _capacity As Integer

    ''' <summary>
    ''' Capacidade maxima da lista
    ''' </summary>
    ''' <returns></returns>
    Public Property Capacity() As Integer
        Get
            Return _capacity
        End Get
        Set(ByVal value As Integer)
            If AllowCapacityChange Then
                _capacity = value
            Else
                Throw New InvalidOperationException("You cannot change the capacity of this collection")
            End If
        End Set
    End Property

    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As TypeCollection)
        If Me.Count = Capacity Then
            Dim message As String = String.Format("List cannot hold more than {0} items", Capacity)
            Throw New InvalidOperationException(message)
        End If
        If AllowAddItem Then
            MyBase.InsertItem(index, item)
        Else
            Throw New InvalidOperationException("You cannot insert new items to this collection")
        End If
    End Sub

    Protected Overrides Sub RemoveItem(ByVal index As Integer)
        If AllowRemoveItem Then
            MyBase.RemoveItem(index)
        Else
            Throw New InvalidOperationException("You cannot remove items of this collection")
        End If
    End Sub


    ''' <summary>
    ''' Cria uma nova lista limitada
    ''' </summary>
    ''' <param name="AllowCapacityChange">Indica se a capacidade pode ser alterada</param>
    ''' <param name="AllowAddItem">Indica se itens podem ser adicionados</param>
    ''' <param name="AllowRemoveItem">indica se intes podem ser removidos</param>
    Public Sub New(Optional AllowCapacityChange As Boolean = True, Optional AllowAddItem As Boolean = True, Optional AllowRemoveItem As Boolean = True)
        Me.AllowCapacityChange = AllowCapacityChange
        Me.AllowRemoveItem = AllowRemoveItem
        Me.AllowAddItem = AllowAddItem
    End Sub



End Class

''' <summary>
''' Coleção congelada (não permite adição nem remoção de itens após sua criação
''' </summary>
''' <typeparam name="TypeCollection">Tipo dos itens da lista</typeparam>
Public Class FrozenCollection(Of TypeCollection)
    Inherits Collection(Of TypeCollection)

    Private Created As Boolean = False

    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As TypeCollection)
        If Created Then
            Throw New InvalidOperationException("You cannot insert new items to a FrozenCollection. Use a Collection or LimitedCollection instead.")
        Else
            MyBase.InsertItem(index, item)
        End If

    End Sub

    Protected Overrides Sub RemoveItem(ByVal index As Integer)
        Throw New InvalidOperationException("You cannot remove items of a FrozenCollection. Use a Collection or LimitedCollection instead.")
    End Sub

    ''' <summary>
    ''' Cria uma coleção congelada baseada em uma outra coleção
    ''' </summary>
    ''' <param name="Collection">coleção</param>
    Public Sub New(Collection As Collection(Of TypeCollection))
        For Each f In Collection
            Add(f)
        Next
        Created = True
    End Sub

    ''' <summary>
    ''' Cria uma coleção congelada baseada em uma outra lista
    ''' </summary>
    ''' <param name="List">Lista</param>
    Public Sub New(List As List(Of TypeCollection))
        For Each f In List
            Add(f)
        Next
        Created = True
    End Sub

    ''' <summary>
    ''' Cria uma coleção congelada baseada em uma matriz de itens
    ''' </summary>
    ''' <param name="Items">Matriz de Intens</param>
    Public Sub New(ParamArray Items() As TypeCollection)
        Me.New(Items.ToList)
    End Sub


End Class

''' <summary>
''' Lista de Valores Duplos
''' </summary>
Public Class TextValueList(Of ValueType)
    Inherits List(Of TextValue(Of ValueType))

    ''' <summary>
    ''' Adiciona um novo TextValue a uma lista com key e valor
    ''' </summary>
    ''' <param name="Text">Valor da Key</param>
    ''' <param name="Value">Valor do Value</param>

    Public Overloads Sub Add(ByVal Text As String, ByVal Value As ValueType)
        MyBase.Add(New TextValue(Of ValueType)(Text, Value))
    End Sub

    Public Function GetText(index As Integer) As String
        Return Me.Item(index).Text
    End Function

    Public Function GetValue(index As Integer) As ValueType
        Return Me.Item(index).Value
    End Function


End Class

''' <summary>
''' Uma par de valores sem chave (texto e item)
''' </summary>
''' <typeparam name="ValueType">Tipo do valor do objeto</typeparam>
Public Class TextValue(Of ValueType)

    Property Text As String
    Property Value As ValueType

    Sub New(Text As String, Value As ValueType)
        Me.Text = Text
        Me.Value = Value
    End Sub

    Sub New(Pair As KeyValuePair(Of Object, Object))
        Me.Text = Pair.Key.ToString
        Me.Value = DirectCast(Pair.Value, ValueType)
    End Sub


    Sub New()

    End Sub
End Class


Public Module TextValueBinder


    Private Function SelectedItemPair(Of TValue)(ByRef Box As Object) As TextValue(Of TValue)
        Return DirectCast(Box.SelectedItem, TextValue(Of TValue))
    End Function

    ''' <summary>
    ''' Retorna o par do item selecionado na Combobox
    ''' </summary>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Box">Combobox</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetSelectedItemPair(Of TValue)(ByRef Box As ComboBox) As TextValue(Of TValue)
        Return SelectedItemPair(Of TValue)(Box)
    End Function

    ''' <summary>
    ''' Retorna o par do item selecionado na ListBox
    ''' </summary>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Box">ListBox</param>
    ''' <returns></returns>
    <Extension>
    Public Function GetSelectedItemPair(Of TValue)(ByRef Box As ListBox) As TextValue(Of TValue)
        Return SelectedItemPair(Of TValue)(Box)
    End Function

    Private Function ItemPair(Of TValue)(ByRef Box As Object, Index As Integer) As TextValue(Of TValue)
        Return DirectCast(Box.Items(Index), TextValue(Of TValue))
    End Function

    ''' <summary>
    ''' Retorna o par do item especificado pela index da Combobox
    ''' </summary>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Box">Combobox</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetItemPair(Of TValue)(ByRef Box As ComboBox, Index As Integer) As TextValue(Of TValue)
        Return ItemPair(Of TValue)(Box, Index)
    End Function
    ''' <summary>
    ''' Retorna o par do item especificado pela index da ListBox
    ''' </summary>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Box">Combobox</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetItemPair(Of TValue)(ByRef Box As ListBox, Index As Integer) As TextValue(Of TValue)
        Return ItemPair(Of TValue)(Box, Index)
    End Function

    Private Sub PairDataSource(Of TValue)(ByRef Box As Object, ByRef Source As TextValueList(Of TValue))
        Box.DataSource = Nothing
        If Source.Count > 0 Then
            Box.DataSource = New BindingSource(Source, Nothing)
            Box.DisplayMember = "Text"
            Box.ValueMember = "Value"
        End If
    End Sub

    ''' <summary>
    ''' Aplica uma lista de pares como source da Combobox
    ''' </summary>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Box">Combobox</param>
    ''' <param name="Source">Lista</param>
    <Extension>
    Public Sub SetPairDataSource(Of TValue)(ByRef Box As ComboBox, ByRef Source As TextValueList(Of TValue))
        PairDataSource(Box, Source)
    End Sub

    ''' <summary>
    ''' Aplica uma lista de pares como source da ListBox
    ''' </summary>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Box">ListBox</param>
    ''' <param name="Source">Lista</param>
    <Extension>
    Public Sub SetPairDataSource(Of TValue)(ByRef Box As ListBox, ByRef Source As TextValueList(Of TValue))
        PairDataSource(Box, Source)
    End Sub

End Module