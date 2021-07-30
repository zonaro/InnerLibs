Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Common
Imports System.Linq

Namespace QueryLibrary
    Friend Class ParameterSetter
        Private ReadOnly _enumAsString As Boolean
        Private ReadOnly _timeout As Integer?

        Friend Sub New(ByVal enumAsString As Boolean, ByVal timeout As Integer?)
            _enumAsString = enumAsString
            _timeout = timeout
        End Sub

        Friend Function GetCommand(ByVal connection As DbConnection, ByVal sql As String, ByVal parameters As Object) As DbCommand
            Dim command = connection.CreateCommand()
            command.CommandText = sql
            If _timeout.HasValue Then command.CommandTimeout = _timeout.Value

            If parameters IsNot Nothing Then
                For Each kvp In ToDictionary(parameters)
                    Dim name = kvp.Key
                    Dim type = kvp.Value.Item1
                    Dim value = kvp.Value.Item2
                    Dim enumerable = TryCast(value, IEnumerable)

                    If enumerable Is Nothing OrElse TypeOf value Is String Then
                        SetSingleParameter(command, name, type, value)
                    Else
                        SetCollectionParameter(command, name, enumerable)
                    End If
                Next
            End If

            Return command
        End Function

        Private Sub SetSingleParameter(ByVal command As DbCommand, ByVal name As String, ByVal type As Type, ByVal value As Object)
            Dim parameter = command.CreateParameter()
            parameter.ParameterName = name
            parameter.DbType = ToDbType(type)
            parameter.Value = If(_enumAsString AndAlso TypeOf value Is [Enum], value.ToString(), If(value, DBNull.Value))
            command.Parameters.Add(parameter)
        End Sub

        Private Sub SetCollectionParameter(ByVal command As DbCommand, ByVal name As String, ByVal collection As IEnumerable)
            Dim parameters = New List(Of String)()
            Dim j = 0

            For Each el In collection
                Dim currentName = name & Math.Min(Threading.Interlocked.Increment(j), j - 1)
                SetSingleParameter(command, currentName, el.GetType(), el)
                parameters.Add("@" & currentName)
            Next

            Dim clause = String.Join(",", parameters)
            command.CommandText = command.CommandText.Replace("@" & name, clause)
        End Sub

        ' from https://github.com/tallesl/net-Dictionary
        Private Function ToDictionary(ByVal input As Object) As IDictionary(Of String, Tuple(Of Type, Object))
            If input Is Nothing Then Throw New ArgumentNullException(NameOf(input))
            If TypeOf input Is IDictionary(Of String, Object) Then Return CType(input, IDictionary(Of String, Object)).ToDictionary(Function(kvp) kvp.Key, Function(kvp) If(kvp.Value Is Nothing, New Tuple(Of Type, Object)(GetType(Object), kvp.Value), New Tuple(Of Type, Object)(kvp.Value.GetType(), kvp.Value)))
            Dim dict = New Dictionary(Of String, Tuple(Of Type, Object))()

            For Each [property] In input.GetType().GetProperties()
                dict.Add([property].Name, New Tuple(Of Type, Object)([property].PropertyType, [property].GetValue(input, Nothing)))
            Next

            For Each field In input.GetType().GetFields()
                dict.Add(field.Name, New Tuple(Of Type, Object)(field.FieldType, field.GetValue(input)))
            Next

            Return dict
        End Function

        Private Shared Function ToDbType(ByVal type As Type) As DbType
            type = If(Nullable.GetUnderlyingType(type), type)
            If type.GetElementType() Is GetType(Byte) Then Return DbType.Binary
            If type Is GetType(Guid) Then Return DbType.Guid
            If type Is GetType(TimeSpan) Then Return DbType.Time

            Select Case Type.GetTypeCode(type)
                Case TypeCode.Boolean
                    Return DbType.Boolean
                Case TypeCode.Byte
                    Return DbType.Byte
                Case TypeCode.Char
                    Return DbType.StringFixedLength
                Case TypeCode.DateTime
                    Return DbType.DateTime
                Case TypeCode.Decimal
                    Return DbType.Decimal
                Case TypeCode.Double
                    Return DbType.Double
                Case TypeCode.Int16
                    Return DbType.Int16
                Case TypeCode.Int32
                    Return DbType.Int32
                Case TypeCode.Int64
                    Return DbType.Int64
                Case TypeCode.SByte
                    Return DbType.SByte
                Case TypeCode.Single
                    Return DbType.Single
                Case TypeCode.String
                    Return DbType.String
                Case TypeCode.UInt16
                    Return DbType.UInt16
                Case TypeCode.UInt32
                    Return DbType.UInt32
                Case TypeCode.UInt64
                    Return DbType.UInt64
                Case Else
                    Return DbType.Object
            End Select
        End Function
    End Class
End Namespace
