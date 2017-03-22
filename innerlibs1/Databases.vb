Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Windows.Forms
Imports System.Xml

Public Class DataBase

    ''' <summary>
    ''' Conexão genérica (Oracle, MySQL, SQLServer etc.)
    ''' </summary>
    ''' <returns></returns>
    Public Property ConnectionString As String

    ''' <summary>
    ''' Tipo da conexão
    ''' </summary>
    ''' <returns></returns>
    Property ConnectionType As Type

    ''' <summary>
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="ConnectionString">String de conexão com o banco</param>
    ''' <param name="Type">Tipo de conexão com o banco</param>
    Public Sub New(Type As Type, ByVal ConnectionString As String)
        Me.ConnectionString = ConnectionString
        ConnectionType = Type
    End Sub

    Private Sub UnsupportedMethod(ParamArray AllowedTypes As Type())
        If Not AllowedTypes.Contains(ConnectionType) Then
            Throw New NotImplementedException("Este método/função ainda não é suportado em " & ConnectionType.Name)
        End If
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="ConnectionString">String de conexão com o banco</param>
    ''' <typeparam name="ConnectionType">Tipo de conexão com o banco</typeparam>
    Public Shared Function Create(Of Connectiontype As DbConnection)(ConnectionString As String) As DataBase
        Return New DataBase(GetType(Connectiontype), ConnectionString)
    End Function

    ''' <summary>
    ''' Executa uma Query no banco. Recomenda-se o uso de procedures.
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL a ser executado</param>
    ''' <returns>Um DataReader com as informações da consulta</returns>

    Public Function RunSQL(ByVal SQLQuery As String) As DbDataReader
        Debug.WriteLine(Environment.NewLine & SQLQuery & Environment.NewLine)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        Dim Reader As DbDataReader = command.ExecuteReader()
        command.Dispose()
        Return Reader
    End Function

    ''' <summary>
    ''' Executa uma Query no banco partir de um Arquivo.
    ''' </summary>
    ''' <param name="File">Arquivo com o comando SQL a ser executado</param>
    ''' <returns>Um DataReader com as informações da consulta</returns>
    Public Function RunSQL(ByVal File As FileInfo) As DbDataReader
        Using s = File.OpenText
            Return RunSQL(s.ReadToEnd)
        End Using
    End Function


    ''' <summary>
    ''' Executa uma Query no banco partir de um Arquivo.
    ''' </summary>
    ''' <param name="File">Arquivo com o comando SQL a ser executado</param>
    ''' <returns>Um DataReader com as informações da consulta</returns>
    Public Function RunSQL(ByVal File As HttpPostedFile) As DbDataReader
        Using s = New StreamReader(File.InputStream)
            Return RunSQL(s.ReadToEnd)
        End Using
    End Function

    ''' <summary>
    ''' Executa uma procedure para cada item dentro de uma coleção
    ''' </summary>
    ''' <param name="Procedure">Nome da procedure</param>
    ''' <param name="ForeignKey">Coluna que representa a chave estrangeira da tabela</param>
    ''' <param name="ForeignValue">Valor que será guardado como chave estrangeira</param>
    ''' <param name="Items">Coleçao de valores que serão inseridos em cada iteraçao</param>
    ''' <param name="Keys">as chaves de cada item</param>
    Public Sub RunProcedureForEach(ByVal Procedure As String, ForeignKey As String, ForeignValue As String, Items As NameValueCollection, ParamArray Keys() As String)
        UnsupportedMethod(GetType(OleDb.OleDbDataReader), GetType(SqlClient.SqlDataReader))
        Dim tamanho_loops_comando = Items.GetValues(Keys(0)).Count
        For index = 0 To tamanho_loops_comando - 1
            Dim comando = "EXEC " & Procedure & " "
            If ForeignKey.IsNotBlank Then
                comando.Append("@" & ForeignKey & "=" & ForeignValue.IsNull & ", ")
            End If
            For Each key In Keys
                Dim valor As String = Items.GetValues(key)(index)
                comando.Append("@" & key & "=" & valor.IsNull() & ", ")
            Next
            RunSQL(comando.Trim.RemoveLastIf(","))
        Next


    End Sub

    ''' <summary>
    ''' Executa uma série de procedures baseando-se eum uma unica chave estrangeira
    ''' </summary>
    ''' <param name="BatchProcedure">Configuraçoes das procedures</param>
    Public Sub RunBatchProcedure(BatchProcedure As BatchProcedure)
        BatchProcedure.Errors = New List(Of Exception)
        For Each p In BatchProcedure
            Try
                RunProcedureForEach(p.ProcedureName, BatchProcedure.ForeignKey, BatchProcedure.ForeignValue, BatchProcedure.Items, p.Keys)

            Catch ex As Exception
                BatchProcedure.Errors.Add(ex)
            End Try
        Next
    End Sub

    ''' <summary>
    ''' Executa uma série de procedures baseando-se eum uma unica chave estrangeira
    ''' </summary>
    ''' <param name="ForeignKey">Coluna que representa a chave estrangeira da tabela</param>
    ''' <param name="ForeignValue">Valor que será guardado como chave estrangeira</param>
    ''' <param name="Items">Coleçao de valores que serão inseridos em cada iteraçao</param>
    ''' <param name="ProcedureConfig">Informaçoes sobre qual procedure será executada e quais keys deverão ser usadas como parametros</param>
    Public Sub RunBatchProcedure(Items As NameValueCollection, ForeignKey As String, ForeignValue As String, ParamArray ProcedureConfig As ProcedureConfig())
        RunBatchProcedure(New BatchProcedure(Items, ForeignKey, ForeignValue, ProcedureConfig))
    End Sub

    ''' <summary>
    ''' Executa uma Query no banco com upload de arquivos.
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL a ser executado</param>
    ''' <param name="FileParameter">Nome do parâmetro que guarda o arquivo</param>
    ''' <param name="File">Arquivo</param>
    ''' <returns>Um DataReader com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, FileParameter As String, File As Byte()) As DbDataReader
        Debug.WriteLine(Environment.NewLine & SQLQuery & Environment.NewLine)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        command.AddFile(FileParameter, File)
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return Reader
    End Function

    ''' <summary>
    ''' Executa uma Query no banco com upload de arquivos.
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL a ser executado</param>
    ''' <param name="FileParameter">Nome do parâmetro que guarda o arquivo</param>
    ''' <param name="File">Arquivo postado</param>
    ''' <returns>Um DataReader com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, FileParameter As String, File As HttpPostedFile) As DbDataReader
        Debug.WriteLine(Environment.NewLine & SQLQuery & Environment.NewLine)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        command.AddFile(FileParameter, File)
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return Reader
    End Function

    ''' <summary>
    ''' Executa uma Query no banco com upload de arquivos.
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL a ser executado</param>
    ''' <param name="FileParameter">Nome do parâmetro que guarda o arquivo</param>
    ''' <param name="File">Arquivo</param>
    ''' <returns>Um DataReader com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, FileParameter As String, File As FileInfo) As DbDataReader
        Debug.WriteLine(Environment.NewLine & SQLQuery & Environment.NewLine)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        command.AddFile(FileParameter, File)
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return Reader
    End Function

    ''' <summary>
    ''' Executa uma Query no banco. Recomenda-se o uso de procedures.
    ''' </summary>
    ''' <param name="Command">Commando de banco de dados pre-pronto</param>
    ''' <returns></returns>
    Public Function RunSQL(Command As DbCommand) As DbDataReader
        Debug.WriteLine(Environment.NewLine & Command.CommandText & Environment.NewLine)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim Reader As DbDataReader = Command.ExecuteReader()
        Return Reader
    End Function

    ''' <summary>
    ''' Executa uma Query no banco. Recomenda-se o uso de procedures.
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL parametrizado a ser executado</param>
    ''' <param name="Parameters">Parametros que serão adicionados ao comando</param>
    ''' <returns>Um DataReader com as informações da consulta</returns> 
    Public Function RunSQL(SQLQuery As String, ParamArray Parameters() As DbParameter) As DbDataReader
        Debug.WriteLine(Environment.NewLine & SQLQuery & Environment.NewLine)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        For Each param In Parameters
            command.Parameters.Add(param)
        Next
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return Reader
    End Function

    ''' <summary>
    ''' Executa uma Query no banco usando como base um TableQuickConnector
    ''' </summary>
    ''' <param name="TableQuickConnector">TableQuickConnector configurado</param>
    ''' <param name="Action">Açao que será realizada no banco</param>
    ''' <param name="WhereConditions">Condições WHERE</param>
    ''' <returns></returns>
    ''' 
    Public Function RunSQL(TableQuickConnector As TableQuickConnector, Action As TableQuickConnector.Action, Optional WhereConditions As String = "") As DbDataReader
        Dim query As String = ""
        Select Case Action
            Case 0
                query = TableQuickConnector.SELECT(WhereConditions)
            Case 1
                query = TableQuickConnector.INSERT(WhereConditions)
            Case 2
                query = TableQuickConnector.UPDATE(WhereConditions)
            Case 3
                Dim ID = WhereConditions.Split("=")(1)
                Dim Column = WhereConditions.Split("=")(0)
                If Column.ContainsAny(" ") Then
                    Column = Column.GetBefore(" ")
                End If
                query = TableQuickConnector.INSERTorUPDATE(ID, Column)
            Case 4
                query = TableQuickConnector.DELETE(WhereConditions)
            Case Else
                Throw New ArgumentException("Ação especificada não existe!")
        End Select
        Dim reader As DbDataReader = RunSQL(query)
        While reader.Read
            For Each col In TableQuickConnector.ColumnControls
                col.SetQueryableValue(reader(col.Name))
            Next
        End While
        Return reader
    End Function
End Class


''' <summary>
''' Classe utilizada para intergligar os campos de um formulário a uma tabela no banco de dados
''' </summary>
Public Class TableQuickConnector
    ''' <summary>
    ''' Lista de ações que um TableQuickConnector pode realizar
    ''' </summary>
    Public Enum Action
        [SELECT] = 0
        INSERT = 1
        UPDATE = 2
        INSERTorUPDATE = 3
        DELETE = 4
    End Enum

    ''' <summary>
    ''' Nome da tabela de destino no banco de dados
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Table As String

    ''' <summary>
    ''' Campos do formulário que serão utilizados como colunas
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property ColumnControls As New List(Of Control)

    ''' <summary>
    ''' Adiciona controles que serão usados como colunas
    ''' </summary>
    ''' <param name="ColumnControls">Controles</param>
    Public Sub AddColumns(ParamArray ColumnControls() As Control)
        For Each col In ColumnControls
            If Not Me.ColumnControls.Contains(col) Then
                Me.ColumnControls.Add(col)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Remove controles que seriam usados como colunas
    ''' </summary>
    ''' <param name="ColumnControls">Controles</param>
    Public Sub RemoveColumns(ParamArray ColumnControls() As Control)
        For Each col In ColumnControls
            If Me.ColumnControls.Contains(col) Then
                Me.ColumnControls.Remove(col)
            End If
        Next
    End Sub


    ''' <summary>
    ''' Inicia uma instancia de TableQuickConnector
    ''' </summary>
    ''' <param name="Table"> Nome da tabela de destino no banco de dados</param>
    ''' <param name="ColumnControls">Campos do formulário que serão utilizados como colunas</param>
    Public Sub New(Table As String, ParamArray ColumnControls() As Control)
        Me.AddColumns(ColumnControls)
        Me.Table = Table
    End Sub


    ''' <summary>
    ''' Comando de INSERT ou UPDATE dependendo do ID. Se o ID for maior que 0, retorna UPDATE, caso contrario, retorna um INSERT.
    ''' </summary>
    ''' <param name="ID">Valor da coluna de ID da tabela</param>
    ''' <param name="Column">Coluna de id da tabela</param>
    ''' <returns></returns>
    ReadOnly Property INSERTorUPDATE(ID As Integer, Column As String) As String
        Get
            If ID > 0 Then
                Return UPDATE(Column & " = " & ID)
            Else
                Return INSERT()
            End If
        End Get
    End Property


    ''' <summary>
    ''' Retorna um comando de INSERT na tabela utilizando os campos como nome das colunas e seus valores como os valores do INSERT
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property INSERT As String
        Get
            Return "INSERT INTO  " & Table & "(" & GetColumnPartOfQuery() & ") values (" & GetValuePartOfQuery() & ")"
        End Get
    End Property

    ''' <summary>
    ''' Retorna um comando de UPDATE na tabela utilizando os campos como nome das colunas e seus valores como os valores do UPDATE
    ''' </summary>
    ''' <param name="WHereConditions">Condiçoes WHERE da Query</param>
    ''' <returns></returns>
    Public ReadOnly Property UPDATE(Optional WhereConditions As String = "") As String
        Get
            WhereConditions = If(WhereConditions.IsNotBlank, WhereConditions.Trim.RemoveFirstIf("where").Prepend(" WHERE "), "")
            Dim cmd = "UPDATE " & Table & " set "
            For Each c In ColumnControls
                cmd.Append(c.Name & " = " & c.GetQueryableValue() & ",")
            Next
            cmd = cmd.RemoveLastIf(",")
            cmd.Append(WhereConditions)
            Return cmd
        End Get
    End Property

    ''' <summary>
    ''' Retorna um comando de DELETE na tabela.
    ''' </summary>
    ''' <param name="WHereConditions">Condiçoes WHERE da Query</param>
    ''' <returns></returns>
    Public ReadOnly Property DELETE(Optional WhereConditions As String = "") As String
        Get
            WhereConditions = If(WhereConditions.IsNotBlank, WhereConditions.Trim.RemoveFirstIf("where").Prepend(" WHERE "), "")
            Dim cmd As String = "DELETE FROM " & Table & WhereConditions
            Return cmd
        End Get
    End Property

    ''' <summary>
    ''' Retorna um comando de SELECT na tabela utilizando os campos como nome das colunas
    ''' </summary>
    ''' <param name="WHereConditions">Condiçoes WHERE da Query</param>
    ''' <returns></returns>
    Public ReadOnly Property [SELECT](Optional WhereConditions As String = "") As String
        Get
            WhereConditions = If(WhereConditions.IsNotBlank, WhereConditions.Trim.RemoveFirstIf("where").Prepend(" WHERE "), "")
            Dim cmd As String = "SELECT " & GetColumnPartOfQuery() & " FROM " & Table & WhereConditions
            Return cmd
        End Get
    End Property


    Private Function GetColumnPartOfQuery() As String
        Dim fields As String = ""
        For Each c In ColumnControls
            fields.Append(c.Name & ",")
        Next
        fields = fields.RemoveLastIf(",")
        Return fields
    End Function

    Private Function GetValuePartOfQuery() As String
        Dim values As String = ""
        For Each c In ColumnControls
            values.Append(c.GetQueryableValue() & ",")
        Next
        values = values.RemoveLastIf(",")
        Return values
    End Function

End Class



''' <summary>
''' Módulo de manipulaçao de Datareaders
''' </summary>
Public Module DataManipulation
    ''' <summary>
    ''' Cria um array com os Itens de um DataReader
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="TextColumn">Coluna que será usada como Texto do elemento option</param>
    ''' <param name="ValueColumn">Coluna que será usada como Value do elemento option</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToListItems(Reader As DbDataReader, TextColumn As String, ValueColumn As String) As ListItem()
        Dim h As New List(Of ListItem)
        If Reader.HasRows Then
            While Reader.Read()
                h.Add(New ListItem(Reader(TextColumn).ToString, Reader(ValueColumn).ToString))
            End While
        End If
        Return h.ToArray
    End Function

    ''' <summary>
    ''' Retorna o valor de uma coluna especifica da primeira linha do primeiro resultado de um DataReader
    ''' </summary>
    ''' <typeparam name="Type">Tipo para qual o valor será convertido</typeparam>
    ''' <param name="Reader">Reader</param>
    ''' <param name="Column">Coluna</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetColumnValue(Of Type As IConvertible)(Reader As DbDataReader, Column As String) As Type
        Try
            Return DirectCast(Reader(Column), Type)
        Catch ex As Exception
            Reader.Read()
            Return DirectCast(Reader(Column), Type)
        End Try
    End Function

    ''' <summary>
    ''' Cria um Dictionary com os 2 Itens de um DataReader
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="KeyColumn">Coluna que será usada como Key do dicionario</param>
    ''' <param name="ValueColumn">Coluna que será usada como Value do dicionario</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToDictionary(Of TKey, TValue)(Reader As DbDataReader, KeyColumn As String, ValueColumn As String) As Dictionary(Of TKey, TValue)
        Dim h As New Dictionary(Of TKey, TValue)
        If Reader.HasRows Then
            While Reader.Read()
                h.Add(Reader(KeyColumn), Reader(ValueColumn))
            End While
        End If
        Return h
    End Function

    ''' <summary>
    ''' Cria uma lista de pares com os Itens de um DataReader
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="TextColumn">Coluna que será usada como Text do item</param>
    ''' <param name="ValueColumn">Coluna que será usada como Value do item</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToTextValueList(Of TValue)(Reader As DbDataReader, TextColumn As String, ValueColumn As String) As TextValueList(Of TValue)
        Dim h As New TextValueList(Of TValue)
        If Reader.HasRows Then
            While Reader.Read()
                h.Add("" & Reader(TextColumn), DirectCast(Reader(ValueColumn), TValue))
            End While
        End If
        Return h
    End Function


    ''' <summary>
    ''' Traz uma lista das possíveis conexões de Bancos de Dados
    ''' </summary>
    ''' <returns>Uma DataTable com todas as factories de Banco de dados</returns>

    Public Function GetDataBasesFactoryClasses() As DataTable
        Return DbProviderFactories.GetFactoryClasses
    End Function

    ''' <summary>
    ''' Concatena um parametro a uma string de comando SQL
    ''' </summary>
    ''' <param name="Command">Comando sql</param>
    ''' <param name="Key">nome do parametro</param>
    ''' <param name="Value">valor do parametro</param>
    <Extension>
    Public Sub AppendSQLParameter(ByRef Command As String, Key As String, Optional Value As String = "")
        Dim param = " @" & Key & "=" & Value.IsNull(Quotes:=Not Value.IsNumber)
        If (Command.Contains("@")) Then
            If Command.Trim.EndsWith(",") Then
                Command.Append(" " & param)
            Else
                Command.Append(", " & param)
            End If
        Else
            Command.Append(param)
        End If
    End Sub




    ''' <summary>
    ''' Conta o numero de linhas de um DbDatareader
    ''' </summary>
    ''' <param name="Reader">OleDbDataReader</param>
    ''' <returns></returns>
    <Extension>
    Public Function CountRows(Reader As DbDataReader) As Integer
        Dim cnt As Integer = 0
        Try
            While Reader.Read()
                cnt.Increment
            End While
            Return cnt
        Catch generatedExceptionName As Exception
            Return -1
        End Try
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
    ''' Converte um Array para um DataSet de 1 Coluna
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
    ''' Transforma o resultado de um DataReader em QueryString
    ''' </summary>
    ''' <param name="Reader">Data Reader</param>
    ''' <returns></returns>
    <Extension> Public Function ToQueryString(Reader As DbDataReader) As String
        Dim param As String = ""
        If Reader.HasRows Then
            Dim colunas As List(Of String) = Reader.GetColumns()
            While Reader.Read()
                For Each item In colunas
                    param.Append("&" & item & "=" & HttpUtility.UrlEncode("" & Reader(item)))
                Next
            End While
            Reader.Dispose()
            Reader.Close()
        End If
        Return param.RemoveFirstIf("?").Prepend("?")
    End Function



    ''' <summary>
    ''' Retorna uma Lista com todas as colunas de um DataReader
    ''' </summary>
    ''' <param name="Reader">Reader</param>
    ''' <returns>uma Lista de strings com os nomes das colunas</returns>
    <Extension>
    Public Function GetColumns(Reader As DbDataReader) As List(Of String)
        Dim columns As New List(Of String)
        For i As Integer = 0 To Reader.FieldCount - 1
            columns.Add(Reader.GetName(i))
        Next
        Return columns
    End Function

    ''' <summary>
    ''' Transforma um DataReader em uma string delimitada por caracteres
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="ColDelimiter">Delimitador de Coluna</param>
    ''' <param name="RowDelimiter">Delimitador de Linha</param>
    ''' <param name="TableDelimiter">Delimitador de Tabelas</param>
    ''' <returns>Uma string delimitada</returns>

    <Extension()>
    Public Function ToDelimitedString(Reader As DbDataReader, Optional ColDelimiter As String = "[ColDelimiter]", Optional RowDelimiter As String = "[RowDelimiter]", Optional TableDelimiter As String = "[TableDelimiter]") As String
        Try
            Dim DelimitedString As String = ""
            Do
                Dim columns As List(Of String) = Reader.GetColumns()

                While Reader.Read()
                    For Each coluna In columns
                        DelimitedString = DelimitedString & (HttpUtility.HtmlEncode(Reader(coluna)) & ColDelimiter)
                    Next
                    DelimitedString = DelimitedString & RowDelimiter
                End While
                DelimitedString = DelimitedString & TableDelimiter
            Loop While Reader.NextResult()

            Return DelimitedString
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function




    ''' <summary>
    ''' Converte um DataReader em XML
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="ItemName">Nome do nó que representa cada linha</param>
    ''' '<param name="TableName">Nome do nó principal do documento</param>

    <Extension()> Function ToXML(Reader As DbDataReader, Optional TableName As String = "Table", Optional ItemName As String = "Row") As XmlDocument
        Dim doc As New XmlDocument
        Dim stringas = ""
        Dim dt = Reader.ToDataTable
        dt.TableName = ItemName.IsNull("Row", False)
        Dim xmlWriter = New StringWriter()
        dt.WriteXml(xmlWriter)
        stringas.Append(xmlWriter.ToString)
        doc.LoadXml(stringas)
        Dim newDocElem As System.Xml.XmlElement = doc.CreateElement(TableName.IsNull("Table", False))

        ' Move the nodes from the old DocumentElement to the new one
        While doc.DocumentElement.HasChildNodes
            newDocElem.AppendChild(doc.DocumentElement.ChildNodes(0))
        End While

        ' Switch in the new DocumentElement
        doc.RemoveChild(doc.DocumentElement)
        doc.AppendChild(newDocElem)
        Return doc
    End Function

    ''' <summary>
    ''' Converte um DataReader em CSV
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="Separator">Separador de valores (vírgula)</param>
    ''' <returns>uma string Comma Separated Values (CSV)</returns>
    <Extension>
    Public Function ToCSV(Reader As DbDataReader, Optional Separator As String = ",") As String
        Dim Returned As String = "sep=" & Separator & Environment.NewLine
        If Reader.HasRows Then
            While Reader.Read()
                For Each item As String In Reader.GetColumns()
                    Returned.Append(Reader(item).ToString().Quote & Separator)
                Next
                Returned.Append(Environment.NewLine)
            End While
        End If
        Return Returned
    End Function

    ''' <summary>
    ''' Copia a primeira linha de um DataReader para uma sessão HttpSessionState usando os nomes das colunas como os nomes dos objetos da sessão
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="Session">Objeto da sessão</param>
    ''' <param name="Timeout">Tempo em minutos para a sessão expirar (se não especificado não altera o timeout da sessão)</param>
    <Extension()>
    Public Sub ToSession(Reader As DbDataReader, Session As HttpSessionState, Optional Timeout As Integer = 0)
        For Each coluna In Reader.GetColumns()
            Session(coluna) = Nothing
        Next

        While Reader.Read()
            For Each coluna In Reader.GetColumns()
                Session(coluna) = Reader(coluna)
            Next
        End While
        If Timeout > 0 Then
            Session.Timeout = Timeout
        End If
        Reader.Close()
    End Sub

    ''' <summary>
    ''' Converte um DataReader para Javascript Object Notation 
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <returns>String JSON</returns>
    <Extension>
    Public Function ToJSON(Reader As DbDataReader) As String
        Return Reader.ToDictionary.SerializeJSON
    End Function

    ''' <summary>
    ''' Converte um DataReader para Javascript Object Notation 
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="ResultIndex">Indice de resultado</param>
    ''' <returns>String JSON</returns>
    <Extension>
    Public Function ToJSON(Reader As DbDataReader, ResultIndex As Integer) As String
        Return Reader.ToDictionary(ResultIndex).SerializeJSON
    End Function

    ''' <summary>
    ''' Converte um  DataReader para uma Lista serializada de um objeto especifico
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="ResultIndex"></param>
    <Extension>
    Public Function ToList(Of Type)(Reader As DbDataReader, Optional ResultIndex As Integer = 0) As List(Of Type)
        Return Reader.ToJSON(ResultIndex).ParseJSON(Of List(Of Type))
    End Function

    ''' <summary>
    ''' Pega o primeiro item (linha) do resultado e serializa em um objeto especifico
    ''' </summary>
    ''' <typeparam name="Type">Tipo de objeto</typeparam>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="ResultIndex">Index do resultset que será serializado (normalmente 0)</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToItem(Of Type)(Reader As DbDataReader, Optional ResultIndex As Integer = 0) As Type
        Return Reader.ToList(Of Type)(ResultIndex)(0)
    End Function


    ''' <summary>
    ''' Converte um DataReader para Uma Lista de Listas de Dicionario
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <returns>String JSON</returns>
    <Extension()> Public Function ToDictionary(Reader As DbDataReader) As List(Of List(Of Dictionary(Of String, Object)))
        Dim todastabelas As New List(Of List(Of Dictionary(Of String, Object)))
        Do
            Dim listatabela As New List(Of Dictionary(Of String, Object))
            Dim cols = Reader.GetColumns
            While Reader.Read
                Dim lista As New Dictionary(Of String, Object)
                For Each col In cols
                    lista.Add(col, Reader(col))
                Next
                listatabela.Add(lista)
            End While
            todastabelas.Add(listatabela)
        Loop While Reader.NextResult
        Return todastabelas
    End Function

    ''' <summary>
    ''' Converte um DataReader para uma Lista de dicionario
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="ResultIndex">Indice de resultado</param>
    ''' <returns>String JSON</returns>
    <Extension()> Public Function ToDictionary(Reader As DbDataReader, ResultIndex As Integer) As List(Of Dictionary(Of String, Object))
        Dim resultsets As New List(Of List(Of Dictionary(Of String, Object)))
        For index = 0 To ResultIndex - 1
            Reader.NextResult()
        Next
        Dim listatabela As New List(Of Dictionary(Of String, Object))
        Dim cols = Reader.GetColumns
        While Reader.Read
            Dim lista As New Dictionary(Of String, Object)
            For Each col In cols
                lista.Add(col, Reader(col))
            Next
            listatabela.Add(lista)
        End While
        Return listatabela
    End Function

    ''' <summary>
    ''' Converte um DataReader para uma tabela em Markdown Pipe
    ''' </summary>
    ''' <param name="Reader">Reader</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToMarkdown(Reader As DbDataReader) As String
        Dim Returned As String = ""
        Do
            If Reader.HasRows Then
                Dim header As String = ""
                Dim base As String = ""
                For Each item As String In Reader.GetColumns()
                    header.Append("|" & item)
                    base.Append("|" & item.Censor("-", item))
                Next
                header.Append("|" & Environment.NewLine)
                base.Append("|" & Environment.NewLine)

                Returned.Append(header)
                Returned.Append(base)

                While Reader.Read()
                    For Each item As String In Reader.GetColumns()
                        Returned.Append("|" & Reader(item))
                    Next
                    Returned.Append("|" & Environment.NewLine)
                End While
            End If
            Returned.Append(Environment.NewLine)
            Returned.Append(Environment.NewLine)
        Loop While Reader.NextResult()
        Return Returned
    End Function

    ''' <summary>
    ''' Converte um DataReader para uma tabela em HTML
    ''' </summary>
    ''' <param name="Reader">Reader</param>
    ''' <param name="Attr">Atributos da tabela. Recomenda-se o uso de classes</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToHTMLTable(Reader As DbDataReader, ParamArray Attr As String()) As String
        Dim Returned As String = ""
        Do
            If Reader.HasRows Then
                Returned = "<table " & Attr.Join(" ") & ">"

                Returned.Append(" <thead>")
                Returned.Append("     <tr>")
                For Each item As String In Reader.GetColumns()
                    Returned.Append("         <th>" & item & "</th>")
                Next
                Returned.Append("     </tr>")
                Returned.Append(" </thead>")
                Returned.Append(" <tbody>")
                While Reader.Read()
                    Returned.Append("     <tr>")
                    For Each item As String In Reader.GetColumns()
                        Returned.Append(" <td>" & Reader(item) & "</td>")
                    Next
                    Returned.Append("     </tr>")
                End While
                Returned.Append(" </tbody>")
                Returned.Append(" </table>")
            End If
        Loop While Reader.NextResult()
        Return Returned
    End Function

    ''' <summary>
    ''' Converte um DataReader para uma lista de DataTables
    ''' </summary>
    ''' <param name="Reader">Reader</param>
    ''' <returns>Uma lista com todas as DataTables</returns>
    <Extension()>
    Public Function ToDataTable(Reader As DbDataReader) As DataTable
        Dim tbRetorno As DataTable = New DataTable
        Dim tbEsquema As DataTable = Reader.GetSchemaTable
        For Each r As DataRow In tbEsquema.Rows
            If Not tbRetorno.Columns.Contains(r("ColumnName")) Then
                Dim col As New DataColumn
                col.ColumnName = r("ColumnName").ToString
                col.Unique = Convert.ToBoolean(r("IsUnique"))
                col.AllowDBNull = Convert.ToBoolean(r("AllowDbNull"))
                col.ReadOnly = Convert.ToBoolean(r("IsReadOnly"))
                tbRetorno.Columns.Add(col)
            End If
        Next

        While Reader.Read
            Dim novaLinha As DataRow = tbRetorno.NewRow
            For i As Integer = 0 To tbRetorno.Columns.Count - 1
                novaLinha(i) = Reader.GetValue(i)
            Next
            tbRetorno.Rows.Add(novaLinha)
        End While
        Return tbRetorno
    End Function

    ''' <summary>
    ''' Aplica os valores encontrados nas colunas de um DataReader em inputs com mesmo ID das colunas. Se os inputs não existirem no resultado eles serão ignorados.
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="Inputs">Controles que serão Manipulados</param>
    ''' <returns>Um array contendo os inputs manipulados</returns>
    <Extension()>
    Public Function ApplyToInputs(Reader As DbDataReader, ParamArray Inputs() As HtmlInputControl) As HtmlInputControl()

        For Each c In Inputs
            Try
                c.Value = Reader(c.ID).ToString()
            Catch ex As Exception
            End Try
        Next


        Return Inputs
    End Function

    ''' <summary>
    ''' Aplica os valores encontrados nas colunas de um DataReader em selects com mesmo ID das colunas. Se os selects não existirem no resultado eles serão ignorados.
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="Selects">Controles que serão Manipulados</param>
    ''' <returns>Um array contendo os selects manipulados</returns>
    <Extension>
    Public Function ApplyToSelects(Reader As DbDataReader, ParamArray Selects() As HtmlSelect) As HtmlSelect()

        For Each c In Selects
            Try
                c.Value = Reader(c.ID).ToString()
            Catch ex As Exception
            End Try
        Next


        Return Selects
    End Function

    ''' <summary>
    ''' Aplica os valores encontrados nas colunas de um DataReader como texto de qualquer controle genérico com mesmo ID das colunas. Se os elementos não existirem no resultado eles serão ignorados.
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="Controls">Controles que serão Manipulados</param>
    ''' <returns>Um array contendo os controles HTML manipulados</returns>
    <Extension>
    Public Function ApplyToControls(Reader As DbDataReader, ParamArray Controls() As HtmlGenericControl) As HtmlGenericControl()

        For Each c In Controls
            Try
                c.InnerText = Reader(c.ID).ToString()
            Catch ex As Exception
            End Try
        Next

        Return Controls
    End Function

    ''' <summary>
    ''' Aplica os valores encontrados nas colunas de um DataReader como texto de textareas com mesmo ID das colunas. Se os elementos não existirem no resultado eles serão ignorados.
    ''' </summary>
    ''' <param name="Reader">DataReader</param>
    ''' <param name="TextAreas">Controles que serão Manipulados</param>
    ''' <returns>Um array contendo as Textareas manipuladas</returns>
    <Extension>
    Public Function ApplyToTextAreas(Reader As DbDataReader, ParamArray TextAreas() As HtmlTextArea) As HtmlTextArea()

        For Each c In TextAreas
            Try
                c.InnerText = Reader(c.ID).ToString()
            Catch ex As Exception
            End Try
        Next

        Return TextAreas
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




End Module

''' <summary>
''' Conjunto de configuraçoes de procedures para ser executado em sequencia
''' </summary>
Public Class BatchProcedure
    Inherits List(Of ProcedureConfig)

    ''' <summary>
    ''' Erros enxontrados ao executar procedures
    ''' </summary>
    ''' <returns></returns>
    Property Errors As New List(Of Exception)
    ''' <summary>
    ''' Coluna de chave estrangeira
    ''' </summary>
    ''' <returns></returns>
    Property ForeignKey As String
    ''' <summary>
    ''' Valor da chave estrangeira
    ''' </summary>
    ''' <returns></returns>
    Property ForeignValue As String

    ''' <summary>
    ''' Items usados na iteraçao
    ''' </summary>
    ''' <returns></returns>
    Property Items As NameValueCollection

    ''' <summary>
    ''' Adciona uma procedure a execuçao atual
    ''' </summary>
    ''' <param name="ProcedureName">Nome da procedure</param>
    ''' <param name="Keys">Chaves que serão utilizadas como parâmetro da procedure</param>
    Shadows Sub Add(ProcedureName As String, ParamArray Keys As String())
        MyBase.Add(New ProcedureConfig(ProcedureName, Keys))
    End Sub

    ''' <summary>
    ''' Cria uma nova lista de procedures
    ''' </summary>
    ''' <param name="Items">Items usados em cada iteração</param>
    ''' <param name="ForeignKey">Coluna de chave estrangeira</param>
    ''' <param name="ForeignValue">Valor da coluna d chave estrangeira</param>
    ''' <param name="Procs">Lista contendo as configurações de cada procedure</param>
    Sub New(Items As NameValueCollection, ForeignKey As String, ForeignValue As String, ParamArray Procs As ProcedureConfig())
        Me.Items = Me.Items
        Me.ForeignKey = ForeignKey
        Me.ForeignValue = ForeignValue
        MyBase.AddRange(Procs)
    End Sub

End Class

''' <summary>
''' Configuração de procedure para a classe <see cref="BatchProcedure"/>
''' </summary>
Public Class ProcedureConfig
    ''' <summary>
    ''' Nome da Procedure
    ''' </summary>
    ''' <returns></returns>
    Property ProcedureName As String

    ''' <summary>
    ''' Chaves usadas como parametros da procedure
    ''' </summary>
    ''' <returns></returns>
    Property Keys As String()

    ''' <summary>
    ''' Cria uma nova configuração de procedure
    ''' </summary>
    ''' <param name="ProcedureName">Nome da Procedure</param>
    ''' <param name="Keys">Chaves usadas como parametros da procedure</param>
    Sub New(ProcedureName As String, ParamArray Keys As String())
        Me.ProcedureName = ProcedureName
        Me.Keys = Keys
    End Sub
End Class





