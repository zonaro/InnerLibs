Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Runtime.CompilerServices
Imports InnerLibs.LINQ

Public Module DbExtensions

    Private typeMap As Dictionary(Of Type, DbType)

    Public ReadOnly Property DbTypes As Dictionary(Of Type, DbType)
        Get
            If typeMap Is Nothing Then
                typeMap = New Dictionary(Of Type, DbType)()
                typeMap(GetType(Byte)) = DbType.Byte
                typeMap(GetType(SByte)) = DbType.SByte
                typeMap(GetType(Short)) = DbType.Int16
                typeMap(GetType(UShort)) = DbType.UInt16
                typeMap(GetType(Integer)) = DbType.Int32
                typeMap(GetType(UInteger)) = DbType.UInt32
                typeMap(GetType(Long)) = DbType.Int64
                typeMap(GetType(ULong)) = DbType.UInt64
                typeMap(GetType(Single)) = DbType.Single
                typeMap(GetType(Double)) = DbType.Double
                typeMap(GetType(Decimal)) = DbType.Decimal
                typeMap(GetType(Boolean)) = DbType.Boolean
                typeMap(GetType(String)) = DbType.String
                typeMap(GetType(Char)) = DbType.StringFixedLength
                typeMap(GetType(Guid)) = DbType.Guid
                typeMap(GetType(DateTime)) = DbType.DateTime
                typeMap(GetType(DateTimeOffset)) = DbType.DateTimeOffset
                typeMap(GetType(Byte())) = DbType.Binary
                typeMap(GetType(Data.Linq.Binary)) = DbType.Binary
            End If
            Return typeMap
        End Get
    End Property

    <Extension> Public Function GetDbType(Of T)(obj As T) As DbType
        Return DbTypes.GetValueOr(GetNullableTypeOf(Of T)(obj), DbType.Object)
    End Function

    <Extension> Public Function GetTypeFromDb(Of T)(obj As DbType) As Type
        Dim tt = DbTypes.FirstOrDefault(Function(x) x.Value = obj)
        If Not IsNothing(tt) Then
            Return tt.Key
        End If
        Return GetType(Object)
    End Function

    <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As NameValueCollection) As DbCommand
        Return CreateCommand(Connection, SQL, Parameters.ToDictionary())
    End Function

    <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As Dictionary(Of String, Object)) As DbCommand
        If Connection IsNot Nothing Then
            Dim command = Connection.CreateCommand()
            command.CommandText = SQL
            If Parameters IsNot Nothing AndAlso Parameters.Any() Then
                For Each p In Parameters.Keys
                    Dim v = Parameters.GetValueOr(p)
                    Dim arr = ForceArray(v)
                    For index = 0 To arr.Length - 1
                        Dim param As DbParameter = command.CreateParameter()
                        If arr.Count() = 1 Then
                            param.ParameterName = $"__{p}"
                        Else
                            param.ParameterName = $"__{p}_{index}"
                        End If
                        param.Value = If(arr(index), DBNull.Value)
                        command.Parameters.Add(param)
                    Next
                Next
            End If
            Return command
        End If
        Return Nothing
    End Function

    <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As FormattableString) As DbCommand
        If SQL IsNot Nothing AndAlso Connection IsNot Nothing Then
            Dim cmd = Connection.CreateCommand()
            If SQL.ArgumentCount > 0 Then
                cmd.CommandText = SQL.Format
                For index = 0 To SQL.ArgumentCount - 1
                    Dim valores = SQL.GetArgument(index)
                    Dim v = ForceArray(valores)
                    Dim param_names As New List(Of String)
                    For v_index = 0 To v.Count - 1
                        Dim param = cmd.CreateParameter()
                        If v.Count() = 1 Then
                            param.ParameterName = $"__p{index}"
                        Else
                            param.ParameterName = $"__p{index}_{v_index}"
                        End If
                        param.Value = If(v(v_index), DBNull.Value)
                        cmd.Parameters.Add(param)
                        param_names.Add("@" & param.ParameterName)
                    Next
                    cmd.CommandText = cmd.CommandText.Replace("{" & index & "}", param_names.Join(",").IfBlank("NULL").QuoteIf("(", param_names.Any()))
                Next
            Else
                cmd.CommandText = SQL.ToString()
            End If
            Return cmd
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata valores espicificos de
    ''' um NameValueCollection como parametros da procedure
    ''' </summary>
    ''' <param name="NVC">Objeto</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">Valores do nameValueCollection o que devem ser utilizados</param>
    ''' <returns>Um DbCommand parametrizado</returns>

    <Extension()>
    Public Function ToProcedure(Connection As DbConnection, ByVal ProcedureName As String, NVC As NameValueCollection, ParamArray Keys() As String) As DbCommand
        Return Connection.ToProcedure(ProcedureName, NVC.ToDictionary(Keys), Keys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata propriedades espicificas de
    ''' um objeto como parametros da procedure
    ''' </summary>
    ''' <param name="Obj">Objeto</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">propriedades do objeto que devem ser utilizados</param>
    ''' <returns>Um DbCommand parametrizado</returns>
    <Extension()>
    Public Function ToProcedure(Of T)(Connection As DbConnection, ByVal ProcedureName As String, Obj As T, ParamArray Keys() As String) As DbCommand
        Return Connection.ToProcedure(ProcedureName, Obj.CreateDictionary(), Keys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e utiliza os pares de um dicionario como parametros da procedure
    ''' </summary>
    ''' <param name="Dic">Dicionario com os parametros</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
    ''' <returns>Um DbCommand parametrizado</returns>

    <Extension()>
    Public Function ToProcedure(Connection As DbConnection, ByVal ProcedureName As String, Dic As Dictionary(Of String, Object), ParamArray Keys() As String) As DbCommand
        Keys = If(Keys, {})
        If Not Keys.Any Then
            Keys = Dic.Keys.ToArray()
        Else
            Keys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If

        Dim sql = "EXEC " & ProcedureName & " " & Keys.SelectJoin(Function(key) " @" & key & " = " & "@__" & key, ", ")

        Return Connection.CreateCommand(sql, Dic.ToDictionary(Function(x) x.Key, Function(x) x.Value))

    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da tabela</param>
    ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToUPDATE(Request As NameValueCollection, ByVal TableName As String, WhereClausule As String, ParamArray Keys As String()) As String
        Keys = If(Keys, {})
        If Keys.Count = 0 Then
            Keys = Request.AllKeys
        Else
            Keys = Request.AllKeys.Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If
        Dim cmd As String = "UPDATE " & TableName & Environment.NewLine & " set "
        For Each col In Keys
            cmd &= (String.Format(" {0} = {1},", col, UrlDecode(Request(col)).Wrap("'")) & Environment.NewLine)
        Next
        cmd = cmd.TrimAny(Environment.NewLine, " ", ",") & If(WhereClausule.IsNotBlank, " WHERE " & WhereClausule.TrimAny(" ", "where", "WHERE"), "")
        Debug.WriteLine(cmd.Wrap(Environment.NewLine))
        Return cmd
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="Dictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="Dic">        Dicionario</param>
    ''' <param name="TableName">  Nome da Tabela</param>
    ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToSQLFilter(Dic As IDictionary(Of String, Object), ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As String
        Dim CMD = "SELECT " & CommaSeparatedColumns.IfBlank("*") & " FROM " & TableName
        FilterKeys = If(FilterKeys, {})

        If FilterKeys.Count = 0 Then
            FilterKeys = Dic.Keys.ToArray()
        Else
            FilterKeys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(FilterKeys)).ToArray
        End If

        FilterKeys = FilterKeys.Where(Function(x) Dic(x) IsNot Nothing AndAlso Dic(x).ToString().IsNotBlank()).ToArray()

        If FilterKeys.Count > 0 Then
            CMD = CMD & " WHERE " & FilterKeys.Select(Function(key) " " & key & "=" & UrlDecode("" & Dic(key)).Wrap("'")).ToArray.Join(" " & [Enum].GetName(GetType(LogicConcatenationOperator), LogicConcatenation) & " ")
        End If
        Return CMD
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="NameValueCollection"/>
    ''' </summary>
    ''' <param name="NVC">        Colecao</param>
    ''' <param name="TableName">  Nome da Tabela</param>
    ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()>
    Public Function ToSQLFilter(NVC As NameValueCollection, ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As String
        Return NVC.ToDictionary.ToSQLFilter(TableName, CommaSeparatedColumns, LogicConcatenation, FilterKeys)
    End Function

    Public Enum LogicConcatenationOperator
        [AND]
        [OR]
    End Enum

    ''' <summary>
    ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
    ''' uma URL como as colunas da tabela de destino
    ''' </summary>
    ''' <param name="Request">        Requisicao HTTP</param>
    ''' <param name="TableName">  Nome da tabela</param>
    ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>
    <Extension()> Public Function ToINSERT(Request As NameValueCollection, ByVal TableName As String, ParamArray Keys As String()) As String
        Keys = If(Keys, {})
        If Keys.Count = 0 Then
            Keys = Request.AllKeys
        Else
            Keys = Request.AllKeys.Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If
        Dim s = String.Format("INSERT INTO " & TableName & " ({0}) values ({1})", Keys.Join(","), Keys.Select(Function(p) Request(p).UrlDecode.Wrap("'")).ToArray.Join(","))
        Debug.WriteLine(s.Wrap(Environment.NewLine))
        Return s
    End Function

    <Extension()> Public Function CreateINSERTCommand(Of T As Class)(Connection As DbConnection, obj As T) As DbCommand
        Dim d = GetType(T)
        Dim dic As New Dictionary(Of String, Object)
        If obj IsNot Nothing AndAlso Connection IsNot Nothing Then
            If obj.IsDictionary Then
            ElseIf obj.GetType() Is GetType(NameValueCollection) Then
                dic = CType(CType(obj, Object), NameValueCollection).ToDictionary()
            Else
                dic = obj.CreateDictionary()
            End If
            Dim cmd = Connection.CreateCommand()
            cmd.CommandText = String.Format($"INSERT INTO " & d.Name & " ({0}) values ({1})", dic.Keys.Join(","), dic.Keys.SelectJoin(Function(x) $"@__{x}", ","))
            For Each k In dic.Keys
                Dim param = cmd.CreateParameter()
                param.ParameterName = $"__{k}"
                param.Value = dic.GetValueOr(k, DBNull.Value)
                cmd.Parameters.Add(param)
            Next
            Return cmd
        End If
        Return Nothing
    End Function


    <Extension()> Public Function RunSQLValue(Connection As DbConnection, SQL As DbCommand) As Object
        Dim v = RunSQLRow(Connection, SQL)
        If v IsNot Nothing AndAlso v.Any() Then
            Return v.First().Value
        End If
        Return Nothing
    End Function

    <Extension()> Public Function RunSQLValue(Of V)(Connection As DbConnection, SQL As DbCommand) As V
        Return ChangeType(Of V)(RunSQLValue(Connection, SQL))
    End Function

    <Extension()> Public Function RunSQLValue(Connection As DbConnection, SQL As FormattableString) As Object
        Dim v = RunSQLRow(Connection, SQL).FirstOrDefault()
        If Not IsNothing(v) Then
            Return v.Value
        End If
        Return Nothing
    End Function

    <Extension()> Public Function RunSQLValue(Of V)(Connection As DbConnection, SQL As FormattableString) As V
        Return ChangeType(Of V)(RunSQLValue(Connection, SQL))
    End Function

    <Extension()> Public Function RunSQLSet(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of Dictionary(Of String, Object))
        Return RunSQLSet(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    <Extension()> Public Function RunSQLRow(Connection As DbConnection, SQL As FormattableString) As Dictionary(Of String, Object)
        Return RunSQLRow(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    <Extension()> Public Function RunSQLSet(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of Dictionary(Of String, Object))
        Return RunSQLSet(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    <Extension()> Public Function RunSQLRow(Connection As DbConnection, SQL As DbCommand) As Dictionary(Of String, Object)
        Return RunSQLRow(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLRow(Of T)(Connection As DbConnection, SQL As DbCommand) As T
        Return Connection.RunSQLSet(Of T)(SQL).FirstOrDefault()
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLRow(Of T)(Connection As DbConnection, SQL As FormattableString) As T
        Return Connection.RunSQLSet(Of T)(SQL).FirstOrDefault()
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLSet(Of T)(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of T)
        Return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(Function(x) x.SetPropertiesIn(Of T))
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLSet(Of T)(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of T)
        Return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(Function(x) x.SetPropertiesIn(Of T))
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de <see cref="Dictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLMany(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
        Return Connection.RunSQLMany(Connection.CreateCommand(SQL))
    End Function

    <Extension()> Public Function RunSQLMany(Connection As DbConnection, Command As DbCommand) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
        Dim resposta As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
        If Connection IsNot Nothing AndAlso Command IsNot Nothing Then
            If Not Connection.State = ConnectionState.Open Then
                Connection.Open()
            End If
            For Each item As DbParameter In Command.Parameters
                Debug.WriteLine(item.Value, $"Parameter {item.ParameterName}".ToString())
            Next
            Debug.WriteLine(Command.CommandText, "SQL Command")
            Using reader = Command.ExecuteReader()
                resposta = reader.MapMany()
            End Using
            Return resposta
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Mapeia os objetos de um datareader para uma classe
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Reader"></param>
    ''' <returns></returns>
    <Extension()> Public Function Map(Of T As Class)(Reader As DbDataReader, ParamArray args As Object()) As List(Of T)
        Dim l = New List(Of T)
        args = If(args, {})
        While Reader IsNot Nothing AndAlso Reader.Read
            Dim d
            If args.Any Then
                d = Activator.CreateInstance(GetType(T), args)
            Else
                d = Activator.CreateInstance(Of T)
            End If
            For i As Integer = 0 To Reader.FieldCount - 1
                Dim name = Reader.GetName(i)
                Dim value = Reader.GetValue(i)
                If GetType(T) = GetType(Dictionary(Of String, Object)) Then
                    CType(CType(d, Object), Dictionary(Of String, Object)).Set(name, value)
                ElseIf GetType(T) = GetType(NameValueCollection) Then
                    CType(CType(d, Object), NameValueCollection).Add(name, value)
                Else
                    If d.HasProperty(name) AndAlso d.GetProperty(name).CanWrite Then
                        d.SetPropertyValue(name, value)
                    End If
                End If
            Next
            l.Add(d)
        End While
        Return l
    End Function

    <Extension()> Public Function MapMany(Reader As DbDataReader) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
        Dim l As New List(Of IEnumerable(Of Dictionary(Of String, Object)))
        Do
            l.Add(Reader.Map(Of Dictionary(Of String, Object))())
        Loop While Reader IsNot Nothing AndAlso Reader.NextResult()
        Return l
    End Function

    <Extension()> Public Function MapFirst(Of T As Class)(Reader As DbDataReader, ParamArray args As Object()) As T
        Return Reader.Map(Of T)(args).FirstOrDefault()
    End Function

End Module