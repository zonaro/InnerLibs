Imports System.Data.Common
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Web

Public Module DataManipulation

    ''' <summary>
    ''' Adiciona ou troca o valor de um parametro em uma coleção
    ''' </summary>
    ''' <param name="Params">Coleçao</param>
    ''' <param name="Parameter">Parâmetro</param>
    <Extension()> Sub SetParameter(ByRef Params As DbParameterCollection, Parameter As DbParameter)
        For Each p As DbParameter In Params
            If p.ParameterName.TrimAny("@", " ", "=") = Parameter.ParameterName.TrimAny("@", " ", "=") Then
                Params(Parameter.ParameterName).Value = If(Parameter.Value, DBNull.Value)
                Exit Sub
            End If
        Next
        Params.Add(Parameter)
    End Sub

    ''' <summary>
    ''' Executa um Comando SQL e retorna uma estrutura estatica com os dados (<see cref="DataBase.Reader"/>)
    ''' </summary>
    ''' <param name="Context"></param>
    ''' <param name="SQLQuery"></param>
    ''' <param name="Parameters"></param>
    ''' <returns></returns>
    <Extension()> Function RunSQL(Context As Data.Linq.DataContext, SQLQuery As String, ParamArray Parameters As DbParameter()) As DataBase.Reader
        Dim con = Activator.CreateInstance(Context.Connection.GetType)
        con.ConnectionString = Context.Connection.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        For Each param In Parameters
            command.Parameters.Add(param)
        Next
        Dim Reader As DbDataReader = command.ExecuteReader()
        Dim t = New DataBase.Reader(Reader)
        con.Close()
        Return t
    End Function


    ''' <summary>
    ''' Cria um <see cref="DataBase.Reader"/> a partir de um comando SELECT
    ''' </summary>
    ''' <param name="WhereConditions">Condições após a clausula WHERE</param>
    ''' <param name="TableName">      Nome da tabela</param>
    <Extension()> Public Function [SELECT](Context As Data.Linq.DataContext, TableName As String, Optional WhereConditions As String = "", Optional Columns As String() = Nothing) As DataBase.Reader
        Dim cmd = "SELECT " & If(Columns.IsEmpty, "*", Columns.Join(",")) & " FROM " & TableName
        If WhereConditions.IsNotBlank Then
            cmd &= (" where " & WhereConditions.TrimAny(" ", "where", "WHERE"))
        End If
        Return Context.RunSQL(cmd)
    End Function


    ''' <summary>
    ''' Retorna o DbType de acordo com o tipo do objeto
    ''' </summary>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDbType(Obj As Object) As DbType
        Try
            Select Case Obj.GetType
                Case GetType(String)
                    Return DbType.String
                Case GetType(Char)
                    Return DbType.String
                Case GetType(Short)
                    Return DbType.Int16
                Case GetType(Integer), GetType(Byte)
                    Return DbType.Int32
                Case GetType(Long)
                    Return DbType.Int64
                Case GetType(Decimal)
                    Return DbType.Decimal
                Case GetType(Double)
                    Return DbType.Double
                Case GetType(DateTime), GetType(Date)
                    Return DbType.DateTime
                Case GetType(Byte())
                    Return DbType.Binary
                Case GetType(Boolean)
                    Return DbType.Boolean
                Case Else
                    Return DbType.Object
            End Select
        Catch ex As Exception
            Return DbType.Object
        End Try
    End Function

    ''' <summary>
    ''' Concatena um parametro a uma string de comando SQL
    ''' </summary>
    ''' <param name="Command">Comando sql</param>
    ''' <param name="Key">nome do parametro</param>
    ''' <param name="Value">valor do parametro</param>
    <Extension>
    Public Function AppendSQLParameter(ByRef Command As String, Key As String, Optional Value As String = Nothing)
        Key = Key.TrimStart("@")
        Dim param = " @" & Key & "=" & Value.IsNull(Quotes:=Not Value.IsNumber)
        If (Command.Contains("@")) Then
            If Command.Trim.EndsWith(",") Then
                Command &= (" " & param)
            Else
                Command &= (", " & param)
            End If
        Else
            Command &= (param)
        End If
        Return Command
    End Function

    ''' <summary>
    ''' Converte um Array para um DataTableReader de 1 Coluna
    ''' </summary>
    ''' <param name="Input">Array com 1 coluna a ser convertida</param>
    ''' <returns>Um DataReader de 1 Coluna</returns>
    <Extension()>
    Public Function ToDataTableReader(Input As String()) As DataTableReader
        Return New DataTableReader(Input.ToDataSet().Tables(0))
    End Function

    ''' <summary>
    ''' Adiciona um parametro de Arquivo no commando
    ''' </summary>
    ''' <param name="Command">Comando</param>
    ''' <param name="FileParameter">Parametro de arquivo</param>
    ''' <param name="File">Arquivo postado</param>
    ''' <returns></returns>
    <Extension()> Public Function AddFile(ByRef Command As DbCommand, FileParameter As String, File As HttpPostedFile) As DbCommand
        Return AddFile(Command, FileParameter, File.ToBytes)
    End Function

    ''' <summary>
    ''' Adiciona um parametro de Arquivo no commando
    ''' </summary>
    ''' <param name="Command">Comando</param>
    ''' <param name="FileParameter">Parametro de arquivo</param>
    ''' <param name="File">Arquivo postado</param>
    ''' <returns></returns>
    <Extension()> Public Function AddFile(ByRef Command As DbCommand, FileParameter As String, File As FileInfo) As DbCommand
        Return AddFile(Command, FileParameter, File.ToBytes)
    End Function

    ''' <summary>
    ''' Adiciona um parametro de Arquivo no commando
    ''' </summary>
    ''' <param name="Command">Comando</param>
    ''' <param name="FileParameter">Parametro de arquivo</param>
    ''' <param name="File">Array de bytes</param>
    ''' <returns></returns>
    <Extension()> Public Function AddFile(ByRef Command As DbCommand, FileParameter As String, File As Byte()) As DbCommand
        Dim param = Command.CreateParameter()
        param.ParameterName = If(FileParameter.StartsWith("@"), FileParameter, "@" & FileParameter)
        param.DbType = DbType.Binary
        param.Value = File
        Command.Parameters.Add(param)
        Return Command
    End Function

    ''' <summary>
    ''' Converte um Array para um <see cref="DataSet"/> de 1 Coluna
    ''' </summary>
    ''' <param name="Input">Array com 1 coluna a ser convertida</param>
    ''' <returns>um DataSet de 1 Coluna</returns>

    <Extension()>
    Public Function ToDataSet(Input As String()) As DataSet
        Dim dataSet As New DataSet()
        Dim dataTable As DataTable = dataSet.Tables.Add()
        dataTable.Columns.Add()
        For Each value In Input.ToList()
            dataTable.Rows.Add(value)
        Next
        Return dataSet
    End Function

    ''' <summary>
    ''' Converte um <see cref="DbDataReader"/> para um <see cref="DataBase.Reader"/>
    ''' </summary>
    ''' <param name="Reader">Reader</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToDataBaseReader(Reader As DbDataReader) As DataBase.Reader
        Return New DataBase.Reader(Reader)
    End Function

    ''' <summary>
    ''' Cria uma string com um comando de INSERT a partir dos dados do dicionario
    ''' </summary>
    ''' <typeparam name="T">Tipo dos dados</typeparam>
    ''' <param name="Dic">Dicionario</param>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <returns></returns>
    <Extension> Public Function ToINSERT(Of T)(Dic As IDictionary(Of String, T), TableName As String) As String
        Return String.Format("INSERT INTO " & TableName & " ({0}) values ({1})", Dic.Keys.Join(","), Dic.Keys.Select(Function(p) Dic(p).ToString.IsNull(Quotes:=Not Dic(p).ToString.IsNumber)).ToArray.Join(","))
    End Function

    ''' <summary>
    ''' Cria uma string com um comando de UPDATE a partir dos dados do dicionario
    ''' </summary>
    ''' <typeparam name="T">Tipo dos dados</typeparam>
    ''' <param name="Dic">Dicionario</param>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <returns></returns>
    <Extension()> Public Function ToUPDATE(Of T)(Dic As IDictionary(Of String, T), TableName As String, WhereClausule As String) As String
        Dim cmd As String = "UPDATE " & TableName & Environment.NewLine & " set "
        For Each col In Dic.Keys
            cmd &= (String.Format(" {0} = {1},", col, Dic(col).ToString.IsNull(Quotes:=Not Dic(col).ToString.IsNumber)) & Environment.NewLine)
        Next
        cmd = cmd.TrimAny(Environment.NewLine, " ", ",") & If(WhereClausule.IsNotBlank, " WHERE " & WhereClausule.TrimAny(" ", "where", "WHERE"), "")
        Return cmd
    End Function

End Module