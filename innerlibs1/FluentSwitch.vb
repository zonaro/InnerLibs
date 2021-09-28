
Imports System.Runtime.CompilerServices

Public Module FluentSwitchExt

    <Extension()>
    Public Function Switch(Of TestType, ReturnType)(Input As TestType) As FluentSwitch(Of TestType, ReturnType)
        Return New FluentSwitch(Of TestType, ReturnType)(Input)
    End Function

    <Extension()>
    Public Function Switch(Of TestType, ReturnType)(Input As TestType, Test As Action(Of FluentSwitch(Of TestType, ReturnType))) As ReturnType
        Dim a = Input.Switch(Of ReturnType)()
        Test(a)
        Return a.GetValue()
    End Function
End Module

Public Class FluentSwitch(Of TestType, ReturnType)

    Private dic As New Dictionary(Of TestType, ReturnType)

    Private input As TestType = Nothing
    Private defaultv As ReturnType = Nothing

    Public Sub New(Input As TestType, Optional DefaultValue As ReturnType = Nothing)
        Me.input = Input
        Me.defaultv = DefaultValue
    End Sub

    Public Function [Case](Value As TestType, ReturnValue As ReturnType) As FluentSwitch(Of TestType, ReturnType)
        Return [Case]({Value}, ReturnValue)
    End Function
    Public Function [Case](Values As IEnumerable(Of TestType), ReturnValue As ReturnType) As FluentSwitch(Of TestType, ReturnType)
        For Each item As TestType In Values
            dic.Set(item, ReturnValue)
        Next
        Return Me
    End Function


    Public Function [Default](ReturnValue As ReturnType) As FluentSwitch(Of TestType, ReturnType)
        defaultv = ReturnValue
        Return Me
    End Function

    Public Function GetValue() As ReturnType
        Return dic.GetValueOr(input, defaultv)
    End Function

    Public Shared Widening Operator CType(FS As FluentSwitch(Of TestType, ReturnType)) As ReturnType
        If FS Is Nothing Then Return Nothing
        Return FS.GetValue()
    End Operator

End Class