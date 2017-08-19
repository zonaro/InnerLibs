Public Class EvalVariable
    Implements iEvalTypedValue, iEvalHasDescription

    Private mValue As Object
    Private mDescription As String
    Private mName As String
    Private mSystemType As System.Type
    Private mEvalType As EvalType

#Region "iEvalTypedValue implementation"
    Public ReadOnly Property Description() As String Implements iEvalHasDescription.Description
        Get
            Return mDescription
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements iEvalHasDescription.Name
        Get
            Return mName
        End Get
    End Property

    Public ReadOnly Property iEvalTypedValue_value() As Object Implements iEvalTypedValue.value
        Get
            Return mValue
        End Get
    End Property

    Public ReadOnly Property EvalType() As EvalType Implements iEvalTypedValue.EvalType
        Get
            Return mEvalType
        End Get
    End Property

    Public ReadOnly Property systemType() As System.Type Implements iEvalTypedValue.systemType
        Get
            Return mSystemType
        End Get
    End Property

#End Region

    Sub New(ByVal name As String, ByVal originalValue As Object, ByVal description As String, ByVal systemType As System.Type)
        mName = name
        mValue = originalValue
        mDescription = description
        mSystemType = systemType
        mEvalType = GetEvalType(systemType)
    End Sub

    Public Property Value() As Object
        Get
            Return mValue
        End Get
        Set(ByVal Value As Object)
            If Not Value Is mValue Then
                mValue = Value
                RaiseEvent ValueChanged(Me, New System.EventArgs)
            End If
        End Set
    End Property

    Public Event ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Implements iEvalTypedValue.ValueChanged
End Class

