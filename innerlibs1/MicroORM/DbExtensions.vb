Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports InnerLibs.LINQ

Namespace MicroORM

    Public Module DbExtensions

        Private typeMap As Dictionary(Of Type, DbType) = Nothing

        ''' <summary>
        ''' Dicionario com os <see cref="Type"/> e seu <see cref="DbType"/> correspondente
        ''' </summary>
        ''' <returns></returns>
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
                    Try
                        typeMap(GetType(Data.Linq.Binary)) = DbType.Binary
                    Catch ex As Exception
                    End Try
                End If
                Return typeMap
            End Get
        End Property

        ''' <summary>
        ''' Retorna um <see cref="DbType"/> de um <see cref="Type"/>
        ''' </summary>
        <Extension> Public Function GetDbType(Of T)(obj As T, Optional Def As DbType = DbType.Object) As DbType
            Return DbTypes.GetValueOr(GetNullableTypeOf(obj), Def)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="Type"/> de um <see cref="DbType"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Type"></param>
        ''' <param name="Def"></param>
        ''' <returns></returns>
        <Extension> Public Function GetTypeFromDb(Of T)(Type As DbType, Optional Def As Type = Nothing) As Type
            Dim tt = DbTypes.FirstOrDefault(Function(x) x.Value = Type)
            If Not IsNothing(tt) Then
                Return tt.Key
            End If
            Return If(Def, GetType(Object))
        End Function

        ''' <summary>
        ''' Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="NameValueCollection"/>, tratando os parametros desta string como parametros SQL
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As NameValueCollection) As DbCommand
            Return CreateCommand(Connection, SQL, Parameters.ToDictionary())
        End Function

        ''' <summary>
        ''' Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="Dictionary(Of String, Object)"/>, tratando os parametros desta string como parametros SQL
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
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

        ''' <summary>
        ''' Cria um <see cref="DbCommand"/> a partir de uma string ou arquivo SQL, tratando os parametros {p} desta string como parametros SQL
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="FilePathOrSQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function CreateCommand(Connection As DbConnection, FilePathOrSQL As String, ParamArray Args As String()) As DbCommand
            If FilePathOrSQL IsNot Nothing Then
                If FilePathOrSQL.IsFilePath() Then
                    If IO.File.Exists(FilePathOrSQL.ToString()) Then
                        Return CreateCommand(Connection, IO.File.ReadAllText(FilePathOrSQL).ToFormattableString(Args))
                    Else
                        Return Nothing
                    End If
                End If
                Return CreateCommand(Connection, FilePathOrSQL.ToFormattableString(Args))
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Cria um <see cref="DbCommand"/> a partir de uma string interpolada, tratando os parametros desta string como parametros SQL
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
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
                        cmd.CommandText = cmd.CommandText.Replace("{" & index & "}", param_names.Join(",").IfBlank("NULL").QuoteIf(param_names.Count > 1, "("))
                    Next
                Else
                    cmd.CommandText = SQL.ToString()
                End If
                Return cmd
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Converte um objeto para uma string SQL, utilizando o objeto como parametro
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Function ToSQLString(Obj As Object) As String
            Return ToSQLString($"{Obj}")
        End Function

        ''' <summary>
        ''' Converte uma <see cref="FormattableString"/> para uma string SQL, tratando seus parametros como parametros da query
        ''' </summary>
        <Extension()> Public Function ToSQLString(SQL As FormattableString) As String
            If SQL IsNot Nothing Then
                If SQL.ArgumentCount > 0 Then
                    Dim CommandText = SQL.Format
                    For index = 0 To SQL.ArgumentCount - 1
                        Dim valores = SQL.GetArgument(index)
                        Dim v = ForceArray(valores)
                        Dim paramvalues As New List(Of Object)
                        For v_index = 0 To v.Count - 1
                            paramvalues.Add(v(v_index))
                        Next
                        Dim pv = paramvalues.Select(Function(x)
                                                        If x Is Nothing Then
                                                            Return "NULL"
                                                        End If
                                                        If GetNullableTypeOf(x).IsNumericType OrElse x.ToString().IsNumber() Then
                                                            Return x.ToString()
                                                        End If
                                                        If IsDate(x) Then
                                                            Return CType(x, Date).ToSQLDateString().Quote("'")
                                                        End If
                                                        If IsBoolean(x) Then
                                                            Return CType(x, Boolean).AsIf(1, 0).ToString()
                                                        End If
                                                        Return x.ToString().Quote("'")
                                                    End Function).ToList()
                        CommandText = CommandText.Replace("{" & index & "}", pv.Join(",").IfBlank("NULL").UnQuote("(", True).QuoteIf(pv.Count > 1, "("))
                    Next
                    Return CommandText
                Else
                    Return SQL.ToString()
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar uma procedure especifica e trata valores especificos de
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
        ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="NameValueCollection" />
        ''' </summary>
        ''' <remarks>
        ''' NameValueCollection pode usar a seguinte estrutura: &name=value1&or:surname=like:%value2% => WHERE [name] = 'value1' OR [surname] like '%value2%'
        ''' </remarks>
        ''' <param name="NVC">        Dicionario</param>
        ''' <param name="TableName">  Nome da Tabela</param>
        ''' <returns>Uma string com o comando montado</returns>
        <Extension()>
        Public Function ToSQLFilter(NVC As NameValueCollection, ByVal TableName As String, CommaSeparatedColumns As String, ParamArray FilterKeys As String()) As [Select]
            Return New [Select](CommaSeparatedColumns.Split(",")).From(TableName).Where(NVC, FilterKeys)
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        ''' <param name="Dic">        Dicionario</param>
        ''' <param name="TableName">  Nome da Tabela</param>
        ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        ''' <returns>Uma string com o comando montado</returns>

        <Extension()>
        Public Function ToSQLFilter(Dic As Dictionary(Of String, Object), ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As [Select]
            Return New [Select](CommaSeparatedColumns.Split(",")).From(TableName).Where(Dic, LogicConcatenation, FilterKeys)
        End Function

        Public Enum LogicConcatenationOperator
            [AND]
            [OR]
        End Enum

        ''' <summary>
        ''' Cria comandos de INSERT para cada objeto do tipo <typeparamref name="T"/> em uma lista
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="obj"></param>
        ''' <param name="TableName"></param>
        ''' <returns></returns>
        <Extension()> Public Function CreateINSERTCommand(Of T As Class)(Connection As DbConnection, obj As IEnumerable(Of T), Optional TableName As String = Nothing) As IEnumerable(Of DbCommand)
            Return If(obj, {}).Select(Function(x) Connection.CreateINSERTCommand(x, TableName))
        End Function

        ''' <summary>
        ''' Cria um comando de INSERT para o objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        <Extension()> Public Function CreateINSERTCommand(Of T As Class)(Connection As DbConnection, obj As T, Optional TableName As String = Nothing) As DbCommand
            Dim d = GetType(T)
            Dim dic As New Dictionary(Of String, Object)
            If obj IsNot Nothing AndAlso Connection IsNot Nothing Then
                If obj.IsDictionary Then
                    dic = CType(CType(obj, Object), Dictionary(Of String, Object))
                ElseIf obj.GetType() Is GetType(NameValueCollection) Then
                    dic = CType(CType(obj, Object), NameValueCollection).ToDictionary()
                Else
                    dic = obj.CreateDictionary()
                End If
                Dim cmd = Connection.CreateCommand()
                cmd.CommandText = String.Format($"INSERT INTO " & BlankCoalesce(TableName, d.Name, "#TableName") & " ({0}) values ({1})", dic.Keys.Join(","), dic.Keys.SelectJoin(Function(x) $"@__{x}", ","))
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

        ''' <summary>
        ''' Cria um comando de INSERT para o objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        <Extension()> Public Function CreateUPDATECommand(Of T As Class)(Connection As DbConnection, obj As T, WhereClausule As String, Optional TableName As String = Nothing) As DbCommand
            Dim d = GetType(T)
            Dim dic As New Dictionary(Of String, Object)
            If obj IsNot Nothing AndAlso Connection IsNot Nothing Then
                If obj.IsDictionary Then
                    dic = CType(CType(obj, Object), Dictionary(Of String, Object))
                ElseIf obj.GetType() Is GetType(NameValueCollection) Then
                    dic = CType(CType(obj, Object), NameValueCollection).ToDictionary()
                Else
                    dic = obj.CreateDictionary()
                End If
                Dim cmd = Connection.CreateCommand()
                cmd.CommandText = String.Format($"UPDATE " & BlankCoalesce(TableName, d.Name, "#TableName") & " set" & Environment.NewLine)
                For Each k In dic.Keys
                    cmd.CommandText &= $"set {k} = @__{k}, {Environment.NewLine}"
                    Dim param = cmd.CreateParameter()
                    param.ParameterName = $"__{k}"
                    param.Value = dic.GetValueOr(k, DBNull.Value)
                    cmd.Parameters.Add(param)
                Next
                cmd.CommandText = cmd.CommandText.TrimAny(Environment.NewLine, ",", " ")
                If WhereClausule.IfBlank("").Trim().StartsWith("where") Then WhereClausule = WhereClausule.IfBlank("").RemoveFirstEqual("WHERE").Trim()

                If WhereClausule.IsNotBlank() Then
                    cmd.CommandText &= $"{Environment.NewLine} WHERE {WhereClausule}"
                End If

                Return cmd
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Executa um comando SQL e retorna o numero de linhas afetadas
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLNone(Connection As DbConnection, SQL As FormattableString) As Integer
            Return RunSQLNone(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Executa um comando SQL e retorna o numero de linhas afetadas
        ''' </summary>
        <Extension()> Public Function RunSQLNone(Connection As DbConnection, Command As DbCommand) As Integer
            If Connection IsNot Nothing AndAlso Command IsNot Nothing Then
                If Not Connection.State = ConnectionState.Open Then
                    Connection.Open()
                End If
                Return Command.LogCommand.ExecuteNonQuery()
            End If
            Return -1
        End Function

        ''' <summary>
        ''' Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLValue(Connection As DbConnection, Command As DbCommand) As Object
            If Connection IsNot Nothing AndAlso Command IsNot Nothing Then
                If Not Connection.State = ConnectionState.Open Then
                    Connection.Open()
                End If
                Return Command.LogCommand().ExecuteScalar()
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        ''' </summary>
        <Extension()> Public Function RunSQLValue(Connection As DbConnection, SQL As FormattableString) As Object
            Return RunSQLValue(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Retorna o primeiro resultado da primeira coluna de uma consulta SQL como um tipo <typeparamref name="V"/>
        ''' </summary>
        <Extension()> Public Function RunSQLValue(Of V As Structure)(Connection As DbConnection, Command As DbCommand) As V?
            Dim vv = RunSQLValue(Connection, Command)
            If vv IsNot Nothing AndAlso vv <> DBNull.Value Then
                Return CType(vv, V)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna o primeiro resultado da primeira coluna de uma consulta SQL como um tipo <typeparamref name="V"/>
        ''' </summary>
        <Extension()> Public Function RunSQLValue(Of V As Structure)(Connection As DbConnection, SQL As FormattableString) As V?
            Return RunSQLValue(Of V)(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Quando Configurado, escreve os parametros e queries executadas no TextWriterEspecifico
        ''' </summary>
        ''' <returns></returns>
        Public Property LogWriter As TextWriter = New DebugTextWriter()

        <Extension()> Public Function LogCommand(Command As DbCommand) As DbCommand
            If LogWriter IsNot Nothing Then
                Dim oldout = System.Console.Out
                System.Console.SetOut(LogWriter)
                LogWriter.WriteLine(New String("="c, 10))
                If Command IsNot Nothing Then
                    For Each item As DbParameter In Command.Parameters
                        Dim bx = $"Parameter: @{item.ParameterName}{Environment.NewLine}Value: {item.Value}{Environment.NewLine}Type: {item.DbType}{Environment.NewLine}Precision/Scale: {item.Precision}/{item.Scale}"
                        System.Console.WriteLine(bx)
                        System.Console.WriteLine(New String("-"c, 10))
                    Next
                    System.Console.WriteLine(Command.CommandText, "SQL Command")
                Else
                    System.Console.WriteLine("Command is NULL")
                End If
                System.Console.WriteLine(New String("="c, 10))
                System.Console.SetOut(oldout)
            End If
            Return Command
        End Function

        ''' <summary>
        ''' Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo <typeparamref name="T"/>
        ''' </summary>
        <Extension()> Public Function RunSQLArray(Of T As Structure)(Connection As DbConnection, Command As DbCommand) As IEnumerable(Of T)
            Return RunSQLSet(Connection, Command).Select(Function(x) ChangeType(Of T)(x.Values.FirstOrDefault()))
        End Function

        ''' <summary>
        ''' Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo <typeparamref name="T"/>
        ''' </summary>
        <Extension()> Public Function RunSQLArray(Of T As Structure)(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of T)
            Return RunSQLArray(Of T)(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Retorna os resultado da primeira coluna de uma consulta SQL como um array
        ''' </summary>
        <Extension()> Public Function RunSQLArray(Connection As DbConnection, Command As DbCommand) As IEnumerable(Of Object)
            Return RunSQLSet(Connection, Command).Select(Function(x) x.Values.FirstOrDefault())
        End Function

        ''' <summary>
        ''' Retorna os resultado da primeira coluna de uma consulta SQL como um array
        ''' </summary>
        <Extension()> Public Function RunSQLArray(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of Object)
            Return RunSQLArray(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of Object, Object)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLPairs(Connection As DbConnection, SQL As DbCommand) As Dictionary(Of Object, Object)
            Return RunSQLSet(Connection, SQL).ToDictionary(Function(x) x.Values.FirstOrDefault(), Function(x) x.Values.LastOrDefault())
        End Function

        ''' <summary>
        ''' Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of Object, Object)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLPairs(Connection As DbConnection, SQL As FormattableString) As Dictionary(Of Object, Object)
            Return RunSQLPairs(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of K, V)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLPairs(Of K, V)(Connection As DbConnection, SQL As DbCommand) As Dictionary(Of K, V)
            Return RunSQLPairs(Connection, SQL).ToDictionary(Function(x) ChangeType(Of K)(x.Key), Function(x) ChangeType(Of V)(x.Value))
        End Function

        ''' <summary>
        ''' Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of K, V)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLPairs(Of K, V)(Connection As DbConnection, SQL As FormattableString) As Dictionary(Of K, V)
            Return RunSQLPairs(Of K, V)(Connection, CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de  <typeparamref name="T"/>
        ''' </summary>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLSet(Of T As Class)(Connection As DbConnection, [Select] As [Select](Of T), Optional WithSubQueries As Boolean = False) As IEnumerable(Of T)
            Return RunSQLSet(Of T)(Connection, [Select].CreateDbCommand(Connection), WithSubQueries)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados da primeira linha como um <typeparamref name="T"/>
        ''' </summary>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLRow(Of T As Class)(Connection As DbConnection, [Select] As [Select](Of T), Optional WithSubQueries As Boolean = False) As T
            Return RunSQLRow(Of T)(Connection, [Select].CreateDbCommand(Connection), WithSubQueries)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de  <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLSet(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of Dictionary(Of String, Object))
            Return RunSQLSet(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para um  <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLRow(Connection As DbConnection, SQL As FormattableString) As Dictionary(Of String, Object)
            Return RunSQLRow(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de  <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLSet(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of Dictionary(Of String, Object))
            Return RunSQLSet(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para um  <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        <Extension()> Public Function RunSQLRow(Connection As DbConnection, SQL As DbCommand) As Dictionary(Of String, Object)
            Return RunSQLRow(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLRow(Of T)(Connection As DbConnection, SQL As DbCommand, Optional WithSubQueries As Boolean = False) As T
            Dim x = Connection.RunSQLSet(Of T)(SQL, False).FirstOrDefault()
            If x IsNot Nothing AndAlso WithSubQueries Then
                ProccessSubQuery(Connection, x, WithSubQueries)
            End If
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLRow(Of T)(Connection As DbConnection, SQL As FormattableString, Optional WithSubQueries As Boolean = False) As T
            Return Connection.RunSQLRow(Of T)(CreateCommand(Connection, SQL), WithSubQueries)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLSet(Of T)(Connection As DbConnection, SQL As FormattableString, Optional WithSubQueries As Boolean = False) As IEnumerable(Of T)
            Return Connection.RunSQLSet(Of T)(Connection.CreateCommand(SQL), WithSubQueries)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLSet(Of T)(Connection As DbConnection, SQL As DbCommand, Optional WithSubQueries As Boolean = False) As IEnumerable(Of T)
            Return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(Function(x)
                                                                            Dim v = CType(x.CreateOrSetObject(Nothing, GetType(T)), T)
                                                                            If WithSubQueries Then
                                                                                Connection.ProccessSubQuery(v, WithSubQueries)
                                                                            End If
                                                                            Return v
                                                                        End Function).AsEnumerable()
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

        ''' <summary>
        ''' Executa uma query SQL e retorna todos os seus resultsets mapeados em uma <see cref="IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))"/>
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Connection As DbConnection, Command As DbCommand) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            Dim resposta As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            Using reader = Connection.RunSQLReader(Command)
                resposta = reader.MapMany()
            End Using
            Return resposta
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class)(Connection As DbConnection, SQL As FormattableString) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4), IEnumerable(Of T5))
            Return Connection.RunSQLMany(Of T1, T2, T3, T4, T5)(CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class)(Connection As DbConnection, Command As DbCommand) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4), IEnumerable(Of T5))
            Dim resposta As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4), IEnumerable(Of T5))
            Using reader = Connection.RunSQLReader(Command)
                resposta = reader.MapMany(Of T1, T2, T3, T4, T5)
            End Using
            Return resposta
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class)(Connection As DbConnection, SQL As FormattableString) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4))
            Return Connection.RunSQLMany(Of T1, T2, T3, T4)(CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class)(Connection As DbConnection, Command As DbCommand) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4))
            Dim resposta As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4))
            Using reader = Connection.RunSQLReader(Command)
                resposta = reader.MapMany(Of T1, T2, T3, T4)
            End Using
            Return resposta
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class, T3 As Class)(Connection As DbConnection, SQL As FormattableString) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3))
            Return Connection.RunSQLMany(Of T1, T2, T3)(CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class, T3 As Class)(Connection As DbConnection, Command As DbCommand) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3))
            Dim resposta As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3))
            Using reader = Connection.RunSQLReader(Command)
                resposta = reader.MapMany(Of T1, T2, T3)
            End Using
            Return resposta
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class)(Connection As DbConnection, SQL As FormattableString) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2))
            Return Connection.RunSQLMany(Of T1, T2)(CreateCommand(Connection, SQL))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="Command"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Of T1 As Class, T2 As Class)(Connection As DbConnection, Command As DbCommand) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2))
            Dim resposta As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2))
            Using reader = Connection.RunSQLReader(Command)
                resposta = reader.MapMany(Of T1, T2)
            End Using
            Return resposta
        End Function

        <Extension()> Public Function RunSQLReader(Connection As DbConnection, SQL As FormattableString) As DbDataReader
            If Connection IsNot Nothing Then
                Return Connection.RunSQLReader(Connection.CreateCommand(SQL))
            End If
            Return Nothing
        End Function

        <Extension()> Public Function RunSQLReader(Connection As DbConnection, Command As DbCommand) As DbDataReader
            If Connection IsNot Nothing AndAlso Command IsNot Nothing Then
                If Not Connection.State = ConnectionState.Open Then
                    Connection.Open()
                End If
                Try
                    Return Command.LogCommand().ExecuteReader()
                Catch ex As Exception
                    Debug.WriteLine(ex)
                End Try
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Mapeia os objetos de um datareader para uma classe
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function Map(Of T As Class)(Reader As DbDataReader, ParamArray args As Object()) As IEnumerable(Of T)
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
                    ElseIf HasProperty(d, name) AndAlso GetProperty(d, name).CanWrite Then
                        SetPropertyValue(d, name, value)
                    End If
                Next
                l.Add(d)
            End While
            Return l.AsEnumerable()
        End Function

        ''' <summary>
        ''' Processa uma propriedade de uma classe marcada com <see cref="FromSQL"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="d"></param>
        ''' <param name="PropertyName"></param>
        ''' <param name="Recursive"></param>
        ''' <returns></returns>
        <Extension()> Public Function ProccessSubQuery(Of T)(Connection As DbConnection, ByRef d As T, PropertyName As String, Optional Recursive As Boolean = False) As T
            If d IsNot Nothing Then
                Dim prop = d.GetProperty(PropertyName)
                If prop IsNot Nothing Then

                    Dim attr = prop.GetCustomAttributes(Of FromSQL)(True).FirstOrDefault()

                    Dim Sql = attr.SQL.Inject(d)

                    Dim gen = prop.PropertyType.IsGenericType
                    Dim lista = gen AndAlso prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(GetType(List(Of)))
                    Dim enume = gen AndAlso prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(GetType(IEnumerable(Of)))

                    If lista OrElse enume Then

                        Dim baselist As IList = Activator.CreateInstance(prop.PropertyType)

                        Dim eltipo = prop.PropertyType.GetGenericArguments().FirstOrDefault()

                        Connection.RunSQLSet(Sql.ToFormattableString).Select(Function(x)
                                                                                 baselist.Add(x.CreateOrSetObject(Nothing, eltipo))
                                                                                 Return Nothing
                                                                             End Function)

                        prop.SetValue(d, baselist)

                        If Recursive Then
                            For Each uu In baselist
                                ProccessSubQuery(Connection, uu, Recursive)
                            Next
                        End If

                        Return d
                    ElseIf prop.PropertyType.IsClass Then
                        If prop.GetValue(d) Is Nothing Then
                            Dim oo = Connection.RunSQLRow(Sql.ToFormattableString).CreateOrSetObject(Nothing, prop.PropertyType)
                            prop.SetValue(d, oo)
                            If Recursive Then
                                ProccessSubQuery(Connection, oo, Recursive)
                            End If
                        End If
                        Return d
                    ElseIf prop.PropertyType.IsValueType Then
                        If prop.GetValue(d) Is Nothing Then
                            Dim oo = Connection.RunSQLValue(Sql.ToFormattableString)
                            prop.SetValue(d, CTypeDynamic(oo, prop.PropertyType))
                            If Recursive Then
                                ProccessSubQuery(Connection, oo, Recursive)
                            End If
                        End If
                        Return d
                    End If
                End If
            End If

            Return d
        End Function

        ''' <summary>
        ''' Processa todas as propriedades de uma classe marcadas com <see cref="FromSQL"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="d"></param>
        ''' <param name="Recursive"></param>
        ''' <returns></returns>
        <Extension()> Public Function ProccessSubQuery(Of T)(Connection As DbConnection, ByRef d As T, Optional Recursive As Boolean = False) As T
            For Each prop In GetProperties(d).Where(Function(x) x.HasAttribute(Of FromSQL))
                ProccessSubQuery(Connection, d, prop.Name, Recursive)
            Next
            Return d
        End Function

        ''' <summary>
        ''' Mapeia a primeira linha de um datareader para uma classe POCO do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Reader"></param>
        ''' <param name="args">argumentos para o construtor da classe</param>
        ''' <returns></returns>
        <Extension()> Public Function MapFirst(Of T As Class)(Reader As DbDataReader, ParamArray args As Object()) As T
            Return Reader.Map(Of T)(args).FirstOrDefault()
        End Function

        ''' <summary>
        ''' Mapeia os resultsets de um datareader para um <see cref="IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))"/>
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function MapMany(Reader As DbDataReader) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            Dim l As New List(Of IEnumerable(Of Dictionary(Of String, Object)))
            Do
                l.Add(Reader.Map(Of Dictionary(Of String, Object))())
            Loop While Reader IsNot Nothing AndAlso Reader.NextResult()
            Return l.AsEnumerable
        End Function

        ''' <summary>
        ''' Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function MapMany(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class)(Reader As DbDataReader) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4), IEnumerable(Of T5))
            Dim o1 As IEnumerable(Of T1) = Nothing
            Dim o2 As IEnumerable(Of T2) = Nothing
            Dim o3 As IEnumerable(Of T3) = Nothing
            Dim o4 As IEnumerable(Of T4) = Nothing
            Dim o5 As IEnumerable(Of T5) = Nothing

            If Reader IsNot Nothing Then

                o1 = Reader.Map(Of T1)

                If Reader.NextResult() Then
                    o2 = Reader.Map(Of T2)
                End If

                If Reader.NextResult() Then
                    o3 = Reader.Map(Of T3)
                End If

                If Reader.NextResult() Then
                    o4 = Reader.Map(Of T4)
                End If

                If Reader.NextResult() Then
                    o5 = Reader.Map(Of T5)
                End If

            End If
            Return Tuple.Create(o1, o2, o3, o4, o5)

        End Function

        ''' <summary>
        ''' Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function MapMany(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class)(Reader As DbDataReader) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3), IEnumerable(Of T4))
            Dim o1 As IEnumerable(Of T1) = Nothing
            Dim o2 As IEnumerable(Of T2) = Nothing
            Dim o3 As IEnumerable(Of T3) = Nothing
            Dim o4 As IEnumerable(Of T4) = Nothing

            If Reader IsNot Nothing Then

                o1 = Reader.Map(Of T1)

                If Reader.NextResult() Then
                    o2 = Reader.Map(Of T2)
                End If

                If Reader.NextResult() Then
                    o3 = Reader.Map(Of T3)
                End If

                If Reader.NextResult() Then
                    o4 = Reader.Map(Of T4)
                End If

            End If
            Return Tuple.Create(o1, o2, o3, o4)

        End Function

        ''' <summary>
        ''' Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function MapMany(Of T1 As Class, T2 As Class, T3 As Class)(Reader As DbDataReader) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2), IEnumerable(Of T3))
            Dim o1 As IEnumerable(Of T1) = Nothing
            Dim o2 As IEnumerable(Of T2) = Nothing
            Dim o3 As IEnumerable(Of T3) = Nothing

            If Reader IsNot Nothing Then

                o1 = Reader.Map(Of T1)

                If Reader.NextResult() Then
                    o2 = Reader.Map(Of T2)
                End If

                If Reader.NextResult() Then
                    o3 = Reader.Map(Of T3)
                End If

            End If
            Return Tuple.Create(o1, o2, o3)

        End Function

        ''' <summary>
        ''' Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        ''' </summary>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function MapMany(Of T1 As Class, T2 As Class)(Reader As DbDataReader) As Tuple(Of IEnumerable(Of T1), IEnumerable(Of T2))
            Dim o1 As IEnumerable(Of T1) = Nothing
            Dim o2 As IEnumerable(Of T2) = Nothing

            If Reader IsNot Nothing Then

                o1 = Reader.Map(Of T1)

                If Reader.NextResult() Then
                    o2 = Reader.Map(Of T2)
                End If

            End If
            Return Tuple.Create(o1, o2)

        End Function

    End Module

End Namespace