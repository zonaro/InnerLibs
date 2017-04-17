Imports System.Data.Common
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Web

Module DataManipulation

    ''' <summary>
    ''' Retorna o DbType de acordo com o tipo do objeto
    ''' </summary>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDbType(Obj As Object) As DbType
        Select Case Obj.GetType
            Case GetType(String)
                Return DbType.String
            Case GetType(Char)
                Return DbType.String
            Case GetType(Short)
                Return DbType.Int16
            Case GetType(Integer)
                Return DbType.Int32
            Case GetType(Long)
                Return DbType.Int64
            Case GetType(Double)
                Return DbType.Double
            Case GetType(DateTime)
                Return DbType.DateTime
            Case GetType(Byte), GetType(Byte())
                Return DbType.Binary
            Case Else
                Return DbType.Object
        End Select
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

End Module
