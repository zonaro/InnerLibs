
''' <summary>
''' Uma estrutura de <see cref="Idictionary"/> que utiliza como Key uma propriedade de Value
''' </summary>
''' <typeparam name="KeyType">Tipo da Key</typeparam>
''' <typeparam name="ClassType">Tipo da</typeparam>
Public Class SelfKeyDictionary(Of KeyType, ClassType As Class)
    Implements IDictionary(Of KeyType, ClassType)

    Private keyselector As Func(Of ClassType, KeyType)

    Sub New(KeySelector As Func(Of ClassType, KeyType))
        Me.keyselector = KeySelector
    End Sub

    Public Function ContainsKey(key As KeyType) As Boolean Implements IDictionary(Of KeyType, ClassType).ContainsKey
        Return collection.Select(keyselector).Contains(key)
    End Function

    Private Sub Add(key As KeyType, value As ClassType) Implements IDictionary(Of KeyType, ClassType).Add
        If Not Me.ContainsKey(keyselector(value)) Then
            collection.Add(value)
        End If
    End Sub


    Public Function Add(Value As ClassType) As KeyType
        Me.Add(Nothing, Value)
        Return keyselector(Value)
    End Function

    Public Function Remove(key As KeyType) As Boolean Implements IDictionary(Of KeyType, ClassType).Remove

        Dim toremove As New List(Of ClassType)
        For Each ii In collection.Where(Function(x) keyselector(x).Equals(key))
            toremove.Add(ii)
        Next
        For Each i In toremove
            collection.Remove(i)
        Next
        Return Me.ContainsKey(key)
    End Function

    Public Function Remove(Value As ClassType) As Boolean
        Return Me.Remove(keyselector(Value))
    End Function

    Public Function TryGetValue(key As KeyType, ByRef value As ClassType) As Boolean Implements IDictionary(Of KeyType, ClassType).TryGetValue
        Try
            value = collection.Where(Function(x) keyselector(x).Equals(key)).SingleOrDefault
            Return value IsNot Nothing
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub Add(item As KeyValuePair(Of KeyType, ClassType)) Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).Add
        collection.Add(item.Value)
    End Sub

    Public Sub Clear() Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).Clear
        collection.Clear()
    End Sub

    Public Function Contains(item As KeyValuePair(Of KeyType, ClassType)) As Boolean Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).Contains
        Return collection.AsEnumerable.Count(Function(x) keyselector(x).Equals(item.Key)) > 0
    End Function

    Public Sub CopyTo(array() As KeyValuePair(Of KeyType, ClassType), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).CopyTo
        collection.Select(Function(x) New KeyValuePair(Of KeyType, ClassType)(keyselector(x), x)).ToArray.CopyTo(array, arrayIndex)
    End Sub

    Public Function Remove(item As KeyValuePair(Of KeyType, ClassType)) As Boolean Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).Remove
        Return collection.Remove(item.Value)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of KeyType, ClassType)) Implements IEnumerable(Of KeyValuePair(Of KeyType, ClassType)).GetEnumerator
        Return collection.Select(Function(x) New KeyValuePair(Of KeyType, ClassType)(keyselector(x), x)).GetEnumerator
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    Private collection As List(Of ClassType)

    Default Public Property Item(key As KeyType) As ClassType Implements IDictionary(Of KeyType, ClassType).Item
        Get
            Return collection.Where(Function(x) keyselector(x).Equals(key)).SingleOrDefault
        End Get
        Set(value As ClassType)
            Dim indexo = collection.IndexOf(Item(key))
            If indexo > -1 Then
                collection.RemoveAt(indexo)
                collection.Insert(indexo, value)
            Else
                Me.Add(value)
            End If
        End Set
    End Property

    Public ReadOnly Property Keys As ICollection(Of KeyType) Implements IDictionary(Of KeyType, ClassType).Keys
        Get
            Return collection.Select(keyselector)
        End Get
    End Property

    Public ReadOnly Property Values As ICollection(Of ClassType) Implements IDictionary(Of KeyType, ClassType).Values
        Get
            Return collection
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).Count
        Get
            Return collection.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of KeyType, ClassType)).IsReadOnly
        Get
            Return False
        End Get
    End Property
End Class
