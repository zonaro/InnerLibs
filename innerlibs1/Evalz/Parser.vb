Imports System.Reflection
Imports System.Data

Friend Class parser
    Private mTokenizer As tokenizer
    Private mEvaluator As Evaluator

    Public Sub New(ByVal evaluator As Evaluator)
        mEvaluator = evaluator
    End Sub

    Public Function Parse(ByVal str As String) As opCode
        If str Is Nothing Then str = String.Empty
        mTokenizer = New tokenizer(Me, str)
        mTokenizer.NextToken()
        Dim res As opCode = ParseExpr(Nothing, ePriority.none)
        If mTokenizer.type = eTokenType.end_of_formula Then
            If res Is Nothing Then res = New opCodeImmediate(EvalType.String, String.Empty)
            Return res
        Else
            mTokenizer.RaiseUnexpectedToken()
        End If
    End Function

    Private Function ParseExpr(ByVal Acc As opCode, ByVal priority As ePriority) As opCode
        Dim ValueLeft, valueRight As opCode
        Do
            Select Case mTokenizer.type
                Case eTokenType.operator_minus
                    ' unary minus operator
                    mTokenizer.NextToken()
                    ValueLeft = ParseExpr(Nothing, ePriority.unaryminus)
                    ValueLeft = New opCodeUnary(eTokenType.operator_minus, ValueLeft)
                    Exit Do
                Case eTokenType.operator_plus
                    ' unary minus operator
                    mTokenizer.NextToken()
                Case eTokenType.operator_not
                    mTokenizer.NextToken()
                    ValueLeft = ParseExpr(Nothing, ePriority.not)
                    ValueLeft = New opCodeUnary(eTokenType.operator_not, ValueLeft)
                    Exit Do
                Case eTokenType.value_identifier
                    ParseIdentifier(ValueLeft)
                    Exit Do
                Case eTokenType.value_true
                        ValueLeft = New opCodeImmediate(EvalType.Boolean, True)
                        mTokenizer.NextToken()
                        Exit Do
                Case eTokenType.value_false
                        ValueLeft = New opCodeImmediate(EvalType.Boolean, False)
                        mTokenizer.NextToken()
                        Exit Do
                Case eTokenType.value_string
                        ValueLeft = New opCodeImmediate(EvalType.String, mTokenizer.value.ToString)
                        mTokenizer.NextToken()
                        Exit Do
                Case eTokenType.value_number
                        Try
                            ValueLeft = New opCodeImmediate(EvalType.Number, Double.Parse( _
                                mTokenizer.value.ToString, _
                                Globalization.NumberStyles.Float, _
                                System.Globalization.CultureInfo.InvariantCulture))
                        Catch ex As Exception
                            mTokenizer.RaiseError(String.Format("Invalid number {0}", mTokenizer.value.ToString))
                        End Try
                        mTokenizer.NextToken()
                        Exit Do
                Case eTokenType.value_date
                        Try
                            ValueLeft = New opCodeImmediate(EvalType.Date, mTokenizer.value.ToString)
                        Catch ex As Exception
                            mTokenizer.RaiseError(String.Format("Invalid date {0}, it should be #DD/MM/YYYY hh:mm:ss#", mTokenizer.value.ToString))
                        End Try
                        mTokenizer.NextToken()
                        Exit Do
                Case eTokenType.open_parenthesis
                        mTokenizer.NextToken()
                        ValueLeft = ParseExpr(Nothing, ePriority.none)
                        If mTokenizer.type = eTokenType.close_parenthesis Then
                            ' good we eat the end parenthesis and continue ...
                            mTokenizer.NextToken()
                            Exit Do
                        Else
                            mTokenizer.RaiseUnexpectedToken("End parenthesis not found")
                        End If
                Case eTokenType.operator_if
                        ' first check functions
                        Dim parameters As New ArrayList  ' parameters... 
                        mTokenizer.NextToken()

                        parameters = ParseParameters(False)
                        Exit Do
                Case Else
                        Exit Do
            End Select
        Loop
        If ValueLeft Is Nothing Then
            mTokenizer.RaiseUnexpectedToken("No Expression found")
        End If
        ParseDot(ValueLeft)
        Do
            Dim tt As eTokenType
            tt = mTokenizer.type
            Select Case tt
                Case eTokenType.end_of_formula
                    ' end of line
                    Return ValueLeft
                Case eTokenType.value_number
                    mTokenizer.RaiseUnexpectedToken("Unexpected number without previous opterator")
                Case eTokenType.operator_plus
                    If priority < ePriority.plusminus Then
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.plusminus)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_minus
                    If priority < ePriority.plusminus Then
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.plusminus)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_concat
                    If priority < ePriority.concat Then
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.concat)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_mul, eTokenType.operator_div
                    If priority < ePriority.muldiv Then
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.muldiv)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_percent
                    If priority < ePriority.percent Then
                        mTokenizer.NextToken()
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, Acc)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_or
                    If priority < ePriority.or Then
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.or)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_and
                    If priority < ePriority.and Then
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.and)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case eTokenType.operator_ne, eTokenType.operator_gt, eTokenType.operator_ge, eTokenType.operator_eq, eTokenType.operator_le, eTokenType.operator_lt
                    If priority < ePriority.equality Then
                        tt = mTokenizer.type
                        mTokenizer.NextToken()
                        valueRight = ParseExpr(ValueLeft, ePriority.equality)
                        ValueLeft = New opCodeBinary(mTokenizer, ValueLeft, tt, valueRight)
                    Else
                        Exit Do
                    End If
                Case Else

                    Exit Do
            End Select
        Loop

        Return ValueLeft
    End Function

    <Flags()> Private Enum eCallType
        [field] = 1
        [method] = 2
        [property] = 4
        [all] = 7
    End Enum

    Private Function EmitCallFunction(ByRef valueLeft As opCode, ByVal funcName As String, ByVal parameters As ArrayList, ByVal CallType As eCallType, ByVal ErrorIfNotFound As Boolean) As Boolean
        Dim newOpcode As opCode
        If valueLeft Is Nothing Then
            Dim functions As Object
            For Each functions In mEvaluator.mEnvironmentFunctionsList
                Do While Not functions Is Nothing
                    newOpcode = GetLocalFunction(functions, functions.GetType, funcName, parameters, CallType)
                    If Not newOpcode Is Nothing Then Exit For
                    If TypeOf functions Is iEvalFunctions Then
                        functions = DirectCast(functions, iEvalFunctions).InheritedFunctions
                    Else
                        Exit Do
                    End If
                Loop
            Next
        Else
            newOpcode = GetLocalFunction(valueLeft, valueLeft.systemType, funcName, parameters, CallType)
        End If
        If Not newOpcode Is Nothing Then
            valueLeft = newOpcode
            Return True
        Else
            If ErrorIfNotFound Then mTokenizer.RaiseError("Variable or method " & funcName & " was not found")
            Return False
        End If
    End Function

    Private Function GetLocalFunction(ByVal base As Object, ByVal baseType As Type, ByVal funcName As String, ByVal parameters As ArrayList, ByVal CallType As eCallType) As opCode
        Dim mi As MemberInfo
        Dim var As iEvalTypedValue
        mi = GetMemberInfo(baseType, funcName, parameters)
        If Not mi Is Nothing Then
            Select Case mi.MemberType
                Case MemberTypes.Field
                    If (CallType And eCallType.field) = 0 Then mTokenizer.RaiseError("Unexpected Field")
                Case MemberTypes.Method
                    If (CallType And eCallType.method) = 0 Then mTokenizer.RaiseError("Unexpected Method")
                Case MemberTypes.Property
                    If (CallType And eCallType.property) = 0 Then mTokenizer.RaiseError("Unexpected Property")
                Case Else
                    mTokenizer.RaiseUnexpectedToken(mi.MemberType.ToString & " members are not supported")
            End Select

            Return opCodeCallMethod.GetNew(mTokenizer, base, mi, parameters)
        End If
        If TypeOf base Is iVariableBag Then
            Dim val As iEvalTypedValue = DirectCast(base, iVariableBag).GetVariable(funcName)
            If Not val Is Nothing Then
                Return New opCodeGetVariable(val)
            End If
        End If
        Return Nothing
    End Function

    Private Function GetMemberInfo(ByVal objType As Type, ByVal func As String, ByVal parameters As ArrayList) As MemberInfo
        Dim bindingAttr As System.Reflection.BindingFlags
        bindingAttr = BindingFlags.GetProperty _
                Or BindingFlags.GetField _
                Or BindingFlags.Public _
                Or BindingFlags.InvokeMethod _
                Or BindingFlags.Instance _
                Or BindingFlags.Static
        If Me.mEvaluator.CaseSensitive = False Then
            bindingAttr = bindingAttr Or BindingFlags.IgnoreCase
        End If
        Dim mis As MemberInfo()

        If func = Nothing Then
            mis = objType.GetDefaultMembers()
        Else
            mis = objType.GetMember(func, bindingAttr)
        End If


        ' There is a bit of cooking here...
        ' lets find the most acceptable Member
        Dim Score, BestScore As Integer
        Dim BestMember As MemberInfo
        Dim plist As ParameterInfo()
        Dim idx As Integer

        Dim mi As MemberInfo
        For i As Integer = 0 To mis.Length - 1
            mi = mis(i)

            If TypeOf mi Is MethodInfo Then
                plist = CType(mi, MethodInfo).GetParameters()
            ElseIf TypeOf mi Is PropertyInfo Then
                plist = CType(mi, PropertyInfo).GetIndexParameters()
            ElseIf TypeOf mi Is FieldInfo Then
                plist = Nothing
            End If
            Score = 10 ' by default
            idx = 0
            If plist Is Nothing Then plist = New ParameterInfo() {}
            If parameters Is Nothing Then parameters = New ArrayList

            Dim pi As Reflection.ParameterInfo
            If parameters.Count > plist.Length Then
                Score = 0
            Else
                For index As Integer = 0 To plist.Length - 1
                    pi = plist(index)
                    'For Each pi As Reflection.ParameterInfo In plist
                    If idx < parameters.Count Then
                        Score += ParamCompatibility(parameters(idx), pi.ParameterType)
                    ElseIf pi.IsOptional() Then
                        Score += 10
                    Else
                        ' unknown parameter
                        Score = 0
                    End If
                    idx += 1
                Next
            End If
            If Score > BestScore Then
                BestScore = Score
                BestMember = mi
            End If
        Next
        Return BestMember
    End Function

    Private Shared Function ParamCompatibility(ByVal value As Object, ByVal type As Type) As Integer
        ' This function returns a score 1 to 10 to this question
        ' Can this value fit into this type ?
        If value Is Nothing Then
            If type Is GetType(Object) Then Return 10
            If type Is GetType(String) Then Return 8
            Return 5
        ElseIf type Is value.GetType Then
            Return 10
        Else
            Return 5
        End If
    End Function

    Private Sub ParseDot(ByRef ValueLeft As opCode)
        Do
            Select Case mTokenizer.type
                Case eTokenType.dot
                    mTokenizer.NextToken()
                Case eTokenType.open_parenthesis
                    ' fine this is either an array or a default property
                Case Else
                    Exit Sub
            End Select
            ParseIdentifier(ValueLeft)
        Loop
    End Sub

    Private Sub ParseIdentifier(ByRef ValueLeft As opCode)
        ' first check functions
        Dim parameters As ArrayList   ' parameters... 
        'Dim types As New ArrayList
        Dim func As String = mTokenizer.value.ToString
        mTokenizer.NextToken()
        Dim isBrackets As Boolean
        parameters = ParseParameters(isBrackets)
        If Not parameters Is Nothing Then
            Dim EmptyParameters As New ArrayList
            Dim ParamsNotUsed As Boolean
            If mEvaluator.Syntax = eParserSyntax.Vb Then
                ' in vb we don't know if it is array or not as we have only parenthesis
                ' so we try with parameters first
                If Not EmitCallFunction(ValueLeft, func, parameters, eCallType.all, ErrorIfNotFound:=False) Then
                    ' and if not found we try as array or default member
                    EmitCallFunction(ValueLeft, func, EmptyParameters, eCallType.all, ErrorIfNotFound:=True)
                    ParamsNotUsed = True
                End If
            Else
                If isBrackets Then
                    If Not EmitCallFunction(ValueLeft, func, parameters, eCallType.property, ErrorIfNotFound:=False) Then
                        EmitCallFunction(ValueLeft, func, EmptyParameters, eCallType.all, ErrorIfNotFound:=True)
                        ParamsNotUsed = True
                    End If
                Else
                    If Not EmitCallFunction(ValueLeft, func, parameters, eCallType.field Or eCallType.method, ErrorIfNotFound:=False) Then
                        EmitCallFunction(ValueLeft, func, EmptyParameters, eCallType.all, ErrorIfNotFound:=True)
                        ParamsNotUsed = True
                    End If
                End If
            End If
            ' we found a function without parameters 
            ' so our parameters must be default property or an array
            Dim t As Type = ValueLeft.systemType
            If ParamsNotUsed Then
                If t.IsArray Then
                    If parameters.Count = t.GetArrayRank() Then
                        ValueLeft = New opCodeGetArrayEntry(ValueLeft, parameters)
                    Else
                        mTokenizer.RaiseError("This array has " & t.GetArrayRank & " dimensions")
                    End If
                Else
                    Dim mi As MemberInfo
                    mi = GetMemberInfo(t, Nothing, parameters)
                    If Not mi Is Nothing Then
                        ValueLeft = opCodeCallMethod.GetNew(mTokenizer, ValueLeft, mi, parameters)
                    Else
                        mTokenizer.RaiseError("Parameters not supported here")
                    End If
                End If
            End If
        Else
            EmitCallFunction(ValueLeft, func, parameters, eCallType.all, ErrorIfNotFound:=True)
        End If
    End Sub

    Private Function ParseParameters(ByRef brackets As Boolean) As ArrayList
        Dim parameters As ArrayList
        Dim valueleft As opCode
        Dim lClosing As eTokenType

        If mTokenizer.type = eTokenType.open_parenthesis _
            OrElse (mTokenizer.type = eTokenType.open_bracket And mEvaluator.Syntax = eParserSyntax.cSharp) Then
            Select Case mTokenizer.type
                Case eTokenType.open_bracket
                    lClosing = eTokenType.close_bracket
                    brackets = True
                Case eTokenType.open_parenthesis
                    lClosing = eTokenType.close_parenthesis
            End Select
            parameters = New ArrayList
            mTokenizer.NextToken() 'eat the parenthesis
            Do
                If mTokenizer.type = lClosing Then
                    ' good we eat the end parenthesis and continue ...
                    mTokenizer.NextToken()
                    Exit Do
                End If
                valueleft = ParseExpr(Nothing, ePriority.none)
                parameters.Add(valueleft)

                If mTokenizer.type = lClosing Then
                    ' good we eat the end parenthesis and continue ...
                    mTokenizer.NextToken()
                    Exit Do
                ElseIf mTokenizer.type = eTokenType.comma Then
                    mTokenizer.NextToken()
                Else
                    mTokenizer.RaiseUnexpectedToken(lClosing.ToString & " not found")
                End If
            Loop
        End If
        Return parameters
    End Function

End Class
