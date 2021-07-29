Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Runtime.CompilerServices
Imports InnerLibs.LINQ

Public Module DbExtensions

    <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As NameValueCollection) As DbCommand
        Return CreateCommand(Connection, SQL, Parameters.ToDictionary())
    End Function

    <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As Dictionary(Of String, Object)) As DbCommand
        If Connection IsNot Nothing Then
            Dim command = Connection.CreateCommand()
            command.CommandText = SQL
            If Parameters IsNot Nothing AndAlso Parameters.Any() Then
                For Each p In Parameters.Keys
                    Dim param As DbParameter = command.CreateParameter()
                    param.ParameterName = p
                    Dim v = Parameters.GetValueOr(p)
                    If IsArray(v) OrElse IsList(v) Then
                        param.Value = ForceArray(v).SelectJoin(Function(x) x.ToString(), ",")
                    Else
                        param.Value = v
                    End If
                    command.Parameters.Add(param)
                Next
            End If
            Return command
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de
    ''' uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="NVC">        Requisicao HTTP</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToProcedure(Connection As DbConnection, ByVal ProcedureName As String, NVC As NameValueCollection, ParamArray Keys() As String) As DbCommand
        Return Connection.ToProcedure(ProcedureName, NVC.ToDictionary(Keys), Keys)
    End Function

    ''' <summary>
    ''' Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de
    ''' uma URL como parametros da procedure
    ''' </summary>
    ''' <param name="Dic">        Requisicao HTTP</param>
    ''' <param name="ProcedureName">  Nome da Procedure</param>
    ''' <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
    ''' <returns>Uma string com o comando montado</returns>

    <Extension()>
    Public Function ToProcedure(Connection As DbConnection, ByVal ProcedureName As String, Dic As Dictionary(Of String, Object), ParamArray Keys() As String) As DbCommand
        Keys = If(Keys, {})
        If Not Keys.Any Then
            Keys = Dic.Keys.ToArray()
        Else
            Keys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(Keys)).ToArray
        End If

        Dim sql = "EXEC " & ProcedureName & " " & Keys.SelectJoin(Function(key) " @" & key & " = " & "@__" & key, ", ")

        Return Connection.CreateCommand(sql, Dic.ToDictionary(Function(x) "@__" & x.Key, Function(x) x.Value))

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
                param.Value = dic.GetValueOr(k)
                cmd.Parameters.Add(param)
            Next
            Return cmd
        End If
        Return Nothing
    End Function

    <Extension()> Public Function RunSQL(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of Dictionary(Of String, Object))
        Return RunSQL(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    <Extension()> Public Function RunSQLFirst(Connection As DbConnection, SQL As FormattableString) As Dictionary(Of String, Object)
        Return RunSQLFirst(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    <Extension()> Public Function RunSQL(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of Dictionary(Of String, Object))
        Return RunSQL(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    <Extension()> Public Function RunSQLFirst(Connection As DbConnection, SQL As DbCommand) As Dictionary(Of String, Object)
        Return RunSQLFirst(Of Dictionary(Of String, Object))(Connection, SQL)
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLFirst(Of T As Class)(Connection As DbConnection, SQL As DbCommand) As T
        Return Connection.RunSQL(Of T)(SQL)?.FirstOrDefault()
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQLFirst(Of T As Class)(Connection As DbConnection, SQL As FormattableString) As T
        Return Connection.RunSQL(Of T)(SQL)?.FirstOrDefault()
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQL(Of T As Class)(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of T)
        Return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(Function(x) x.SetPropertiesIn(Of T))
    End Function

    ''' <summary>
    ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="T"/>
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Connection"></param>
    ''' <param name="SQL"></param>
    ''' <returns></returns>
    <Extension()> Public Function RunSQL(Of T As Class)(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of T)
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
            Debug.WriteLine(Command.CommandText, "Running SQL Command")
            Using reader = Command.ExecuteReader()
                resposta = reader.MapMany()
            End Using
            Return resposta
        End If
        Return Nothing
    End Function

    <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As FormattableString) As DbCommand
        If SQL IsNot Nothing AndAlso Connection IsNot Nothing Then
            Dim cmd = Connection.CreateCommand()
            If SQL.ArgumentCount > 0 Then
                cmd.CommandText = SQL.Format
                For index = 0 To SQL.ArgumentCount - 1
                    Dim param = cmd.CreateParameter()
                    param.ParameterName = $"__p{index}"
                    param.Value = SQL.GetArgument(index)
                    cmd.Parameters.Add(param)
                    cmd.CommandText = cmd.CommandText.Replace("{" & index & "}", $"@{param.ParameterName}")
                Next
            Else
                cmd.CommandText = SQL.ToString()
            End If
            Return cmd
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