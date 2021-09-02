Namespace ExpressionParser

    Public Interface iEvalFunctions

        Function InheritedFunctions() As iEvalFunctions

    End Interface

    Public Interface iEvalHasDescription
        ReadOnly Property Name() As String
        ReadOnly Property Description() As String
    End Interface

    Public Interface iEvalTypedValue
        Inherits iEvalValue
        ReadOnly Property SystemType() As Type
        ReadOnly Property EvalType() As EvalType
    End Interface

    Public Interface iEvalValue
        ReadOnly Property Value() As Object
        Event ValueChanged(ByVal Sender As Object, ByVal e As EventArgs)
    End Interface
    Public Interface iVariableBag
        Function GetVariable(ByVal varname As String) As iEvalTypedValue
    End Interface


End Namespace