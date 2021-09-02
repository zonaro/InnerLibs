Imports System.Reflection
Imports System.Data


Namespace ExpressionParser

    Public Class Evaluator
        Friend mEnvironmentFunctionsList As ArrayList
        Public RaiseVariableNotFoundException As Boolean
        Public ReadOnly CaseSensitive As Boolean

        Sub New(Optional ByVal CaseSensitive As Boolean = False)
            Me.CaseSensitive = CaseSensitive
            mEnvironmentFunctionsList = New ArrayList
        End Sub

        Public Sub AddEnvironmentFunctions(ByVal obj As Object)
            If obj Is Nothing Then Exit Sub
            If Not mEnvironmentFunctionsList.Contains(obj) Then
                mEnvironmentFunctionsList.Add(obj)
            End If
        End Sub

        Public Sub RemoveEnvironmentFunctions(ByVal obj As Object)
            If mEnvironmentFunctionsList.Contains(obj) Then
                mEnvironmentFunctionsList.Remove(obj)
            End If
        End Sub

        Public Function Parse(ByVal str As String) As opCode
            str = str.TrimCarriage
            str = str.IfBlank(Of String)(False)
            If str.IsAny("True", "true", "False", "false") Then
                Return New opCodeImmediate(EvalType.Boolean, str.ToLower().ChangeType(Of Boolean))
            End If
            If str.IsNumber Then
                Return New opCodeImmediate(EvalType.Number, str.ChangeType(Of Long))
            End If
            Return New parser(Me).Parse(str)
        End Function

        Public Shared Function ConvertToString(ByVal value As Object) As String
            If TypeOf value Is String Then
                Return DirectCast(value, String)
            ElseIf value Is Nothing Then
                Return String.Empty
            ElseIf TypeOf value Is Date Then
                Dim d As Date = DirectCast(value, Date)
                If d.TimeOfDay.TotalMilliseconds > 0 Then
                    Return d.ToString
                Else
                    Return d.ToShortDateString()
                End If
            ElseIf TypeOf value Is Decimal Then
                Dim d As Decimal = DirectCast(value, Decimal)
                If (d Mod 1) <> 0 Then
                    Return d.ToString("#,##0.00")
                Else
                    Return d.ToString("#,##0")
                End If
            ElseIf TypeOf value Is Double Then
                Dim d As Double = DirectCast(value, Double)
                If (d Mod 1) <> 0 Then
                    Return d.ToString("#,##0.00")
                Else
                    Return d.ToString("#,##0")
                End If

            End If
            Return If(value?.ToString(), "")
        End Function

        Public Class parserException
            Inherits Exception
            Public ReadOnly formula As String
            Public ReadOnly pos As Integer

            Friend Sub New(ByVal str As String, ByVal formula As String, ByVal pos As Integer)
                MyBase.New(str)
                Me.formula = formula
                Me.pos = pos
            End Sub

        End Class

    End Class

End Namespace

