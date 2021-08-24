
Imports System.Runtime.CompilerServices

Public Module FluentSwitchExt

    <Extension()>
    Public Function Switch(Of T2, T1)(Input As T1) As FluentSwitch(Of T1, T2)
        Return New FluentSwitch(Of T1, T2)(Input)
    End Function

    <Extension()>
    Public Function Switch(Of T2, T1)(Input As T1, Test As Action(Of FluentSwitch(Of T1, T2))) As T2
        Dim a = Input.Switch(Of T2)()
        Test(a)
        Return a.GetValue()
    End Function
End Module

Public Class FluentSwitch(Of T1, T2)

    Private dic As New Dictionary(Of T1, T2)

    Private input As T1 = Nothing
    Private defaultv As T2 = Nothing

    Public Sub New(Input As T1, Optional DefaultValue As T2 = Nothing)
        Me.input = Input
        Me.defaultv = DefaultValue
    End Sub

    Public Function [Case](Value As T1, ReturnValue As T2) As FluentSwitch(Of T1, T2)
        Return [Case]({Value}, ReturnValue)
    End Function
    Public Function [Case](Values As IEnumerable(Of T1), ReturnValue As T2) As FluentSwitch(Of T1, T2)
        For Each item As T1 In Values
            dic.Set(item, ReturnValue)
        Next
        Return Me
    End Function


    Public Function [Default](ReturnValue As T2) As FluentSwitch(Of T1, T2)
        defaultv = ReturnValue
        Return Me
    End Function

    Public Function GetValue() As T2
        Return dic.GetValueOr(input, defaultv)
    End Function

    Public Shared Widening Operator CType(FS As FluentSwitch(Of T1, T2)) As T2
        If FS Is Nothing Then Return Nothing
        Return FS.GetValue()
    End Operator

End Class