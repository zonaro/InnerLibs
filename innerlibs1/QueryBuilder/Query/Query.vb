Imports System
Imports System.Data
Imports System.Data.Common
Imports System.Globalization
Imports System.Text

Namespace QueryLibrary

    ''' <summary>
    ''' A simplistic ADO.NET wrapper.
    ''' </summary>
    Public NotInheritable Partial Class Query
        Implements IDisposable

        Private ReadOnly _connectionString As String
        Private ReadOnly _manualClosing As Boolean
        Private ReadOnly _safe As Boolean
        Private ReadOnly _providerFactory As DbProviderFactory
        Private ReadOnly _parameterSetter As ParameterSetter
        Private _currentConnection As DbConnection = Nothing
        Private _currentTransaction As DbTransaction = Nothing
        Private _connectionLock As Object = New Object()
        Private _disposed As Boolean = False

        ''' <summary>
        ''' Ctor.
        ''' </summary>
        ''' <paramname="connectionString">Connection string to use</param>
        ''' <paramname="providerName">Data provider to use</param>
        ''' <paramname="enumAsString">Treat enum values as strings (ToString)</param>
        ''' <paramname="manualClosing">
        ''' Connection/transaction closing 'manually' instead of automatically on each call
        ''' </param>
        ''' <paramname="safe">Throws when a selected a property is not found in the given type</param>
        ''' <paramname="timeout">Optional DBCommand.CommandTimeout value</param>
        Public Sub New(ByVal connectionString As String, ByVal providerName As String, ByVal Optional enumAsString As Boolean = False, ByVal Optional manualClosing As Boolean = False, ByVal Optional safe As Boolean = False, ByVal Optional timeout As Integer? = Nothing)
            _providerFactory = DbProviderFactories.GetFactory(providerName)
            _connectionString = connectionString
            _manualClosing = manualClosing
            _safe = safe
            _parameterSetter = New ParameterSetter(enumAsString, timeout)
        End Sub

        Private ReadOnly Property OpenConnection As DbConnection
            Get

                SyncLock _connectionLock

                    If _currentConnection Is Nothing Then
                        _currentConnection = _providerFactory.CreateConnection()
                        _currentConnection.ConnectionString = _connectionString
                        _currentConnection.Open()
                    End If

                    Return _currentConnection
                End SyncLock
            End Get
        End Property

        Private ReadOnly Property OpenTransaction As DbTransaction
            Get

                SyncLock _connectionLock
                    If _currentTransaction Is Nothing Then _currentTransaction = OpenConnection.BeginTransaction()
                    Return _currentTransaction
                End SyncLock
            End Get
        End Property

        ''' <summary>
        ''' Closes and commit any underlying connection and transaction that were open automatically.
        ''' Does nothing if manualClosing is set to False.
        ''' </summary>
        Public Sub Close()
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            If _manualClosing Then CloseRegardless()
        End Sub

        ''' <summary>
        ''' Disposes any open underlying connection or transaction.
        ''' Only need when ManualClosing is set to True.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            CloseRegardless()
            _disposed = True
        End Sub

        Friend Shared Function FormatSql(ByVal command As DbCommand) As String
            Dim sql = New StringBuilder(command.CommandText)

            For Each p As DbParameter In command.Parameters
                sql.Replace("@"c & p.ParameterName, p.Value.ToString())
            Next

            Return sql.ToString()
        End Function

        Private Sub CloseIfNeeded()
            If Not _manualClosing Then CloseRegardless()
        End Sub

        Private Sub CloseRegardless(ByVal Optional rollback As Boolean = False)
            SyncLock _connectionLock
                If _currentConnection Is Nothing Then Return

                If _currentTransaction IsNot Nothing Then
                    If rollback Then
                        _currentTransaction.Rollback()
                    Else
                        _currentTransaction.Commit()
                    End If

                    _currentTransaction.Dispose()
                    _currentTransaction = Nothing
                End If

                _currentConnection.Dispose()
                _currentConnection = Nothing
            End SyncLock
        End Sub

        Private Function FillDataTable(ByVal command As DbCommand) As DataTable
            Dim dataTable = New DataTable()
            dataTable.Locale = CultureInfo.CurrentCulture
            Dim adapter = _providerFactory.CreateDataAdapter()
            adapter.SelectCommand = command
            adapter.Fill(dataTable)
            Return dataTable
        End Function
    End Class
End Namespace
