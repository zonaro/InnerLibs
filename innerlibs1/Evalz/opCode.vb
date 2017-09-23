Public MustInherit Class opCode
    Implements iEvalTypedValue, iEvalHasDescription

    Protected mValueDelegate As ValueDelegate
    Protected Delegate Function ValueDelegate() As Object

    Delegate Sub RunDelegate()

    Protected Sub New()

    End Sub

    Protected Sub RaiseEventValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent ValueChanged(sender, e)
    End Sub

    MustOverride ReadOnly Property EvalType() As EvalType Implements iEvalTypedValue.EvalType

    Public Function CanReturn(ByVal type As EvalType) As Boolean
        Return True
    End Function

    Public Overridable ReadOnly Property Description() As String Implements iEvalHasDescription.Description
        Get
            Return "opCode " & Me.GetType.Name
        End Get
    End Property

    Public Overridable ReadOnly Property Name() As String Implements iEvalHasDescription.Name
        Get
            Return "opCode " & Me.GetType.Name
        End Get
    End Property

    Public Overridable ReadOnly Property value() As Object Implements iEvalTypedValue.value
        Get
            Return mValueDelegate()
        End Get
    End Property

    Public Overridable ReadOnly Property systemType() As System.Type Implements iEvalTypedValue.systemType
        Get
            Return Globals.GetSystemType(Me.EvalType)
        End Get
    End Property


    Protected Friend Sub Convert(ByVal tokenizer As tokenizer, ByRef param1 As opCode, ByVal EvalType As EvalType)
        If param1.EvalType <> EvalType Then
            If param1.CanReturn(EvalType) Then
                param1 = New opCodeConvert(tokenizer, param1, EvalType)
            Else
                tokenizer.RaiseError("Cannot convert " & param1.Name & " into " & EvalType)
            End If
        End If
    End Sub

    Protected Shared Sub ConvertToSystemType(ByRef param1 As iEvalTypedValue, ByVal SystemType As Type)
        If Not param1.SystemType Is SystemType Then
            If SystemType Is GetType(Object) Then
                'ignore
            Else
                param1 = New opCodeSystemTypeConvert(param1, SystemType)
            End If
        End If
    End Sub

    Protected Sub SwapParams(ByRef Param1 As opCode, ByRef Param2 As opCode)
        Dim swp As opCode = Param1
        Param1 = Param2
        Param2 = swp
    End Sub

    Public Event ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Implements iEvalTypedValue.ValueChanged
End Class

Friend Class opCodeVariable
    Inherits opCode

    WithEvents mVariable As EvalVariable

    Sub New(ByVal variable As EvalVariable)
        mVariable = variable
    End Sub

    Public Overrides ReadOnly Property Value() As Object
        Get
            Return mVariable
        End Get
    End Property

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mVariable.EvalType
        End Get
    End Property

    Private Sub mVariable_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mVariable.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub
End Class

Friend Class opCodeImmediate
    Inherits opCode

    Private mValue As Object
    Private mEvalType As EvalType

    Sub New(ByVal EvalType As EvalType, ByVal value As Object)
        mEvalType = EvalType
        mValue = value
    End Sub

    Public Overrides ReadOnly Property Value() As Object
        Get
            Return mValue
        End Get
    End Property


    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mEvalType
        End Get
    End Property
End Class

Friend Class opCodeUnary
    Inherits opCode

    WithEvents mParam1 As opCode
    Private mEvalType As EvalType

    Sub New(ByVal tt As eTokenType, ByVal param1 As opCode)
        mParam1 = param1
        Dim v1Type As EvalType = mParam1.EvalType

        Select Case tt
            Case eTokenType.operator_not
                If v1Type = EvalType.Boolean Then
                    mValueDelegate = AddressOf BOOLEAN_NOT
                    mEvalType = EvalType.Boolean
                End If
            Case eTokenType.operator_minus
                If v1Type = EvalType.Number Then
                    mValueDelegate = AddressOf NUM_CHGSIGN
                    mEvalType = EvalType.Number
                End If
        End Select
    End Sub

    Private Function BOOLEAN_NOT() As Object
        Return Not DirectCast(mParam1.value, Boolean)
    End Function

    Private Function NUM_CHGSIGN() As Object
        Return -DirectCast(mParam1.value, Double)
    End Function

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mEvalType
        End Get
    End Property

    Private Sub mParam1_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mParam1.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub
End Class

Friend Class opCodeConvert
    Inherits opCode
    WithEvents mParam1 As iEvalTypedValue
    Private mEvalType As EvalType = EvalType.Unknown

    Sub New(ByVal tokenizer As tokenizer, ByVal param1 As iEvalTypedValue, ByVal EvalType As EvalType)
        mParam1 = param1
        Select Case EvalType
            Case EvalType.Boolean
                mValueDelegate = AddressOf TBool
                mEvalType = EvalType.Boolean
            Case EvalType.Date
                mValueDelegate = AddressOf TDate
                mEvalType = EvalType.Date
            Case EvalType.Number
                mValueDelegate = AddressOf TNum
                mEvalType = EvalType.Number
            Case EvalType.String
                mValueDelegate = AddressOf TStr
                mEvalType = EvalType.String
            Case Else
                tokenizer.RaiseError("Cannot convert " & param1.SystemType.Name & " to " & EvalType)
        End Select
    End Sub

    Private Function TBool() As Object
        Return Globals.TBool(mParam1)
    End Function

    Private Function TDate() As Object
        Return Globals.TDate(mParam1)
    End Function

    Private Function TNum() As Object
        Return Globals.TNum(mParam1)
    End Function

    Private Function TStr() As Object
        Return Globals.TStr(mParam1)
    End Function

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mEvalType
        End Get
    End Property

    Private Sub mParam1_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mParam1.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub
End Class

Friend Class opCodeSystemTypeConvert
    Inherits opCode
    WithEvents mParam1 As iEvalTypedValue
    Private mEvalType As EvalType = EvalType.Unknown
    Private mSystemType As System.Type

    Sub New(ByVal param1 As iEvalTypedValue, ByVal Type As System.Type)
        mParam1 = param1
        mValueDelegate = AddressOf [CType]
        mSystemType = Type
        mEvalType = Globals.GetEvalType(Type)
    End Sub

    Private Function [CType]() As Object
        Return System.Convert.ChangeType(mParam1.Value, mSystemType)
    End Function

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mEvalType
        End Get
    End Property

    Public Overrides ReadOnly Property systemType() As System.Type
        Get
            Return mSystemType
        End Get
    End Property

    Private Sub mParam1_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mParam1.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub
End Class

Friend Class opCodeBinary
    Inherits opCode

    WithEvents mParam1 As opCode
    WithEvents mParam2 As opCode
    Private mEvalType As EvalType

    Public Sub New(ByVal tokenizer As tokenizer, ByVal param1 As opCode, ByVal tt As eTokenType, ByVal param2 As opCode)
        mParam1 = param1
        mParam2 = param2

        Dim v1Type As EvalType = mParam1.EvalType
        Dim v2Type As EvalType = mParam2.EvalType

        Select Case tt
            Case eTokenType.operator_plus
                If v1Type = EvalType.Number And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf NUM_PLUS_NUM
                    mEvalType = EvalType.Number
                ElseIf v1Type = EvalType.Number And v2Type = EvalType.Date Then
                    SwapParams(mParam1, mParam2)
                    mValueDelegate = AddressOf DATE_PLUS_NUM
                    mEvalType = EvalType.Date
                ElseIf v1Type = EvalType.Date And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf DATE_PLUS_NUM
                    mEvalType = EvalType.Date
                ElseIf mParam1.CanReturn(EvalType.String) And mParam2.CanReturn(EvalType.String) Then
                    Convert(tokenizer, param1, EvalType.String)
                    mValueDelegate = AddressOf STR_CONCAT_STR
                    mEvalType = EvalType.String
                End If
            Case eTokenType.operator_minus
                If v1Type = EvalType.Number And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf NUM_MINUS_NUM
                    mEvalType = EvalType.Number
                ElseIf v1Type = EvalType.Date And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf DATE_MINUS_NUM
                    mEvalType = EvalType.Date
                ElseIf v1Type = EvalType.Date And v2Type = EvalType.Date Then
                    mValueDelegate = AddressOf DATE_MINUS_DATE
                    mEvalType = EvalType.Number
                End If
            Case eTokenType.operator_mul
                If v1Type = EvalType.Number And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf NUM_MUL_NUM
                    mEvalType = EvalType.Number
                End If
            Case eTokenType.operator_div
                If v1Type = EvalType.Number And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf NUM_DIV_NUM
                    mEvalType = EvalType.Number
                End If
            Case eTokenType.operator_percent
                If v1Type = EvalType.Number And v2Type = EvalType.Number Then
                    mValueDelegate = AddressOf NUM_PERCENT_NUM
                    mEvalType = EvalType.Number
                End If
            Case eTokenType.operator_and, eTokenType.operator_or
                Convert(tokenizer, mParam1, EvalType.Boolean)
                Convert(tokenizer, mParam2, EvalType.Boolean)
                Select Case tt
                    Case eTokenType.operator_or
                        mValueDelegate = AddressOf BOOL_OR_BOOL
                        mEvalType = EvalType.Boolean
                    Case eTokenType.operator_and
                        mValueDelegate = AddressOf BOOL_AND_BOOL
                        mEvalType = EvalType.Boolean
                End Select
            Case eTokenType.operator_lt, eTokenType.operator_le, eTokenType.operator_gt, eTokenType.operator_ge, eTokenType.operator_eq, eTokenType.operator_ne
                Select Case tt
                    Case eTokenType.operator_lt
                        mValueDelegate = AddressOf NUM_LT_NUM
                        mEvalType = EvalType.Boolean
                    Case eTokenType.operator_le
                        mValueDelegate = AddressOf NUM_LE_NUM
                        mEvalType = EvalType.Boolean
                    Case eTokenType.operator_gt
                        mValueDelegate = AddressOf NUM_GT_NUM
                        mEvalType = EvalType.Boolean
                    Case eTokenType.operator_ge
                        mValueDelegate = AddressOf NUM_GE_NUM
                        mEvalType = EvalType.Boolean
                    Case eTokenType.operator_eq
                        Select Case v1Type
                            Case EvalType.Number
                                mValueDelegate = AddressOf NUM_EQ_NUM
                            Case EvalType.String
                                mValueDelegate = AddressOf STR_EQ_STR
                            Case EvalType.Date
                                mValueDelegate = AddressOf DATE_EQ_DATE
                        End Select
                        mEvalType = EvalType.Boolean
                    Case eTokenType.operator_ne
                        Select Case v1Type
                            Case EvalType.Number
                                mValueDelegate = AddressOf NUM_NE_NUM
                            Case EvalType.String
                                mValueDelegate = AddressOf STR_NE_STR
                            Case EvalType.Date
                                mValueDelegate = AddressOf DATE_NE_DATE
                        End Select


                        mEvalType = EvalType.Boolean
                End Select
        End Select

        If mValueDelegate Is Nothing Then
            tokenizer.RaiseError( _
                "Cannot apply the operator " & tt.ToString.Replace("operator_", "") & _
                " on " & v1Type.ToString & _
                " and " & v2Type.ToString)
        End If
    End Sub

    Private Function BOOL_AND_BOOL() As Object
        Return DirectCast(mParam1.value, Boolean) And DirectCast(mParam2.value, Boolean)
    End Function

    Private Function BOOL_OR_BOOL() As Object
        Return DirectCast(mParam1.value, Boolean) Or DirectCast(mParam2.value, Boolean)
    End Function

    Private Function BOOL_XOR_BOOL() As Object
        Return DirectCast(mParam1.value, Boolean) Xor DirectCast(mParam2.value, Boolean)
    End Function

    Private Function NUM_EQ_NUM() As Object
        Return DirectCast(mParam1.value, Double) = DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_LT_NUM() As Object
        Return DirectCast(mParam1.value, Double) < DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_GT_NUM() As Object
        Return DirectCast(mParam1.value, Double) > DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_GE_NUM() As Object
        Return DirectCast(mParam1.value, Double) >= DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_LE_NUM() As Object
        Return DirectCast(mParam1.value, Double) <= DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_NE_NUM() As Object
        Return DirectCast(mParam1.value, Double) <> DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_PLUS_NUM() As Object
        Return DirectCast(mParam1.value, Double) + DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_MUL_NUM() As Object
        Return DirectCast(mParam1.value, Double) * DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_MINUS_NUM() As Object
        Return DirectCast(mParam1.value, Double) - DirectCast(mParam2.value, Double)
    End Function

    Private Function DATE_PLUS_NUM() As Object
        Return DirectCast(mParam1.value, Date).AddDays(DirectCast(mParam2.value, Double))
    End Function

    Private Function DATE_MINUS_DATE() As Object
        Return DirectCast(mParam1.value, Date).Subtract(DirectCast(mParam2.value, Date)).TotalDays
    End Function

    Private Function DATE_MINUS_NUM() As Object
        Return DirectCast(mParam1.value, Date).AddDays(-DirectCast(mParam2.value, Double))
    End Function

    Private Function DATE_EQ_DATE() As Object
        Return DirectCast(mParam1.value, Date).Date = (DirectCast(mParam2.value, Date)).Date
    End Function

    Private Function DATE_NE_DATE() As Object
        Return Not DirectCast(mParam1.value, Date).Date = (DirectCast(mParam2.value, Date)).Date
    End Function

    Private Function STR_CONCAT_STR() As Object
        Return mParam1.value.ToString & mParam2.value.ToString
    End Function

    Private Function STR_EQ_STR() As Object
        Return mParam1.value.ToString = mParam2.value.ToString
    End Function

    Private Function STR_NE_STR() As Object
        Return mParam1.value.ToString <> mParam2.value.ToString
    End Function

    Private Function NUM_DIV_NUM() As Object
        Return DirectCast(mParam1.value, Double) / DirectCast(mParam2.value, Double)
    End Function

    Private Function NUM_PERCENT_NUM() As Object
        Return DirectCast(mParam2.value, Double) * (DirectCast(mParam1.value, Double) / 100)
    End Function

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mEvalType
        End Get
    End Property

    Private Sub mParam1_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mParam1.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub

    Private Sub mParam2_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mParam2.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub
End Class

Public Class opCodeGetVariable
    Inherits opCode

    WithEvents mParam1 As iEvalTypedValue

    Sub New(ByVal value As iEvalTypedValue)
        mParam1 = value
    End Sub


    Public Overrides ReadOnly Property Value() As Object
        Get
            Return mParam1.Value
        End Get
    End Property

    Public Overrides ReadOnly Property systemType() As System.Type
        Get
            Return mParam1.SystemType
        End Get
    End Property

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mParam1.EvalType
        End Get
    End Property

    Private Sub mParam1_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mParam1.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub

End Class

Public Class opCodeCallMethod
    Inherits opCode

    Private mBaseObject As Object
    Private mBaseSystemType As System.Type
    Private mBaseEvalType As EvalType
    WithEvents mBaseValue As iEvalValue  ' for the events only
    Private mBaseValueObject As Object

    Private mMethod As System.Reflection.MemberInfo
    Private mParams As iEvalTypedValue()
    Private mParamValues As Object()

    Private mResultSystemType As System.Type
    Private mResultEvalType As EvalType
    WithEvents mResultValue As iEvalValue  ' just for some

    Friend Sub New(ByVal baseObject As Object, ByVal method As System.Reflection.MemberInfo, ByVal params As IList)
        If params Is Nothing Then params = New iEvalTypedValue() {}
        Dim newParams(params.Count - 1) As iEvalTypedValue
        Dim newParamValues(params.Count - 1) As Object

        params.CopyTo(newParams, 0)

        For Each p As iEvalTypedValue In newParams
            AddHandler p.ValueChanged, AddressOf mParamsValueChanged
        Next
        mParams = newParams
        mParamValues = newParamValues
        mBaseObject = baseObject
        mMethod = method

        If TypeOf mBaseObject Is iEvalValue Then
            If TypeOf mBaseObject Is iEvalTypedValue Then
                With DirectCast(mBaseObject, iEvalTypedValue)
                    mBaseSystemType = .SystemType
                    mBaseEvalType = .EvalType
                End With
            Else
                mBaseSystemType = mBaseObject.GetType()
                mBaseEvalType = Globals.GetEvalType(mBaseSystemType)
            End If
        Else
            mBaseSystemType = mBaseObject.GetType
            mBaseEvalType = Globals.GetEvalType(mBaseSystemType)
        End If

        Dim paramInfo() As Reflection.ParameterInfo
        If TypeOf method Is Reflection.PropertyInfo Then
            With DirectCast(method, Reflection.PropertyInfo)
                mResultSystemType = DirectCast(method, Reflection.PropertyInfo).PropertyType
                paramInfo = .GetIndexParameters()
            End With
            mValueDelegate = AddressOf GetProperty
        ElseIf TypeOf method Is Reflection.MethodInfo Then
            With DirectCast(method, Reflection.MethodInfo)
                mResultSystemType = .ReturnType
                paramInfo = .GetParameters()
            End With
            mValueDelegate = AddressOf GetMethod
        ElseIf TypeOf method Is Reflection.FieldInfo Then
            With DirectCast(method, Reflection.FieldInfo)
                mResultSystemType = .FieldType
                paramInfo = New Reflection.ParameterInfo() {}
            End With
            mValueDelegate = AddressOf GetField
        End If

        For i As Integer = 0 To mParams.Length - 1
            If i < paramInfo.Length Then
                ConvertToSystemType(mParams(i), paramInfo(i).ParameterType)
            End If
        Next

        If GetType(iEvalValue).IsAssignableFrom(mResultSystemType) Then
            mResultValue = DirectCast(InternalValue(), iEvalValue)
            If TypeOf mResultValue Is iEvalTypedValue Then
                With DirectCast(mResultValue, iEvalTypedValue)
                    mResultSystemType = .SystemType
                    mResultEvalType = .EvalType
                End With
            ElseIf mResultValue Is Nothing Then
                mResultSystemType = GetType(Object)
                mResultEvalType = EvalType.Object
            Else
                Dim v As Object = mResultValue.Value
                If v Is Nothing Then
                    mResultSystemType = GetType(Object)
                    mResultEvalType = EvalType.Object
                Else
                    mResultSystemType = v.GetType
                    mResultEvalType = Globals.GetEvalType(mResultSystemType)
                End If
            End If
        Else
            mResultSystemType = systemType
            mResultEvalType = Globals.GetEvalType(systemType)
        End If
    End Sub

    Protected Friend Shared Function GetNew(ByVal tokenizer As tokenizer, ByVal baseObject As Object, ByVal method As System.Reflection.MemberInfo, ByVal params As IList) As opCode
        Dim o As opCode
        o = New opCodeCallMethod(baseObject, method, params)

        If o.EvalType <> EvalType.Object _
                    AndAlso Not o.systemType Is Globals.GetSystemType(o.EvalType) Then
            Return New opCodeConvert(tokenizer, o, o.EvalType)
        Else
            Return o
        End If
    End Function

    Private Function GetProperty() As Object
        Dim res As Object = DirectCast(mMethod, Reflection.PropertyInfo).GetValue(mBaseValueObject, mParamValues)
        Return res
    End Function

    Private Function GetMethod() As Object
        Dim res As Object = DirectCast(mMethod, Reflection.MethodInfo).Invoke(mBaseValueObject, mParamValues)
        Return res
    End Function

    Private Function GetField() As Object
        Dim res As Object = DirectCast(mMethod, Reflection.FieldInfo).GetValue(mBaseValueObject)
        Return res
    End Function

    Private Function InternalValue() As Object
        For i As Integer = 0 To mParams.Length - 1
            mParamValues(i) = mParams(i).Value
        Next
        If TypeOf mBaseObject Is iEvalValue Then
            mBaseValue = DirectCast(mBaseObject, iEvalValue)
            mBaseValueObject = mBaseValue.Value
        Else
            mBaseValueObject = mBaseObject
        End If
        Return MyBase.mValueDelegate()
    End Function

    Public Overrides ReadOnly Property Value() As Object
        Get
            Dim res As Object = InternalValue()
            If TypeOf res Is iEvalValue Then
                mResultValue = DirectCast(res, iEvalValue)
                res = mResultValue.Value
            End If
            Return res
        End Get
    End Property

    Public Overrides ReadOnly Property SystemType() As System.Type
        Get
            Return mResultSystemType
        End Get
    End Property

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mResultEvalType
        End Get
    End Property

    Private Sub mParamsValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs)
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub

    Private Sub mBaseVariable_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mBaseValue.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub

    Private Sub mResultVariable_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mResultValue.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub
End Class

Public Class opCodeGetArrayEntry
    Inherits opCode

    WithEvents mArray As opCode

    Private mParams As iEvalTypedValue()
    Private mValues As Integer()
    Private mResultEvalType As EvalType
    Private mResultSystemType As System.Type

    Public Sub New(ByVal array As opCode, ByVal params As IList)
        Dim newParams(params.Count - 1) As iEvalTypedValue
        Dim newValues(params.Count - 1) As Integer
        params.CopyTo(newParams, 0)
        mArray = array
        mParams = newParams
        mValues = newValues
        mResultSystemType = array.systemType.GetElementType
        mResultEvalType = Globals.GetEvalType(mResultSystemType)
    End Sub

    Public Overrides ReadOnly Property Value() As Object
        Get
            Dim res As Object
            Dim arr As Array = DirectCast(mArray.value, Array)
            For i As Integer = 0 To mValues.Length - 1
                mValues(i) = CInt(mParams(i).Value)
            Next
            res = arr.GetValue(mValues)
            Return res
        End Get
    End Property

    Public Overrides ReadOnly Property SystemType() As System.Type
        Get
            Return mResultSystemType
        End Get
    End Property

    Public Overrides ReadOnly Property EvalType() As EvalType
        Get
            Return mResultEvalType
        End Get
    End Property

    Private Sub mBaseVariable_ValueChanged(ByVal Sender As Object, ByVal e As System.EventArgs) Handles mArray.ValueChanged
        MyBase.RaiseEventValueChanged(Sender, e)
    End Sub

End Class


