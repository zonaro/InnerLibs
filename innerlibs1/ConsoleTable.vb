Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Namespace ConsoleTables

    Public Class ConsoleTable

        Public Property Columns As List(Of String)
            Get
                Return Options?.Columns
            End Get
            Set(value As List(Of String))
                If value IsNot Nothing Then
                    Options.Columns = value
                End If
            End Set
        End Property

        Public Property Rows As New List(Of Object())
        Public Property Options As New ConsoleTableOptions
        Public Property ColumnTypes As Type()

        Public Sub New(ParamArray columns As String())
            Me.Options.Columns.AddRange(If(columns, {}))
        End Sub

        Public Sub New(Optional options As ConsoleTableOptions = Nothing)
            Me.Options = If(options, Me.Options)
        End Sub

        Public Function AddColumn(ByVal names As IEnumerable(Of String)) As ConsoleTable
            For Each name In names
                Options.Columns.Add(name)
            Next
            Return Me
        End Function

        Public Function AddValue(Key As String, obj As Object) As ConsoleTable
            If Columns.Contains(Key) Then
                Dim l = New List(Of Object)
                For Each item In Columns
                    If item = Key Then
                        l.Add(obj)
                    Else
                        l.Add(String.Empty)
                    End If
                Next
                AddRow(l.ToArray())
            End If
            Return Me
        End Function

        Public Function AddRow(ParamArray Values As Object()) As ConsoleTable
            Dim v = If(Values, {}).ToList
            If Not Columns.Any() Then
                Columns = Enumerable.Range(0, v.Count).Select(Function(x) "Col" & x.ToString()).ToList()
            End If

            While v.Count < Columns.Count
                v.Add(String.Empty)
            End While
            Rows.Add(v.Take(Columns.Count).ToArray())

            Return Me
        End Function

        Public Function Configure(ByVal action As Action(Of ConsoleTableOptions)) As ConsoleTable
            action(Options)
            Return Me
        End Function

        Public Shared Function From(Of T)(ByVal values As IEnumerable(Of T)) As ConsoleTable
            Dim table = New ConsoleTable With {
                .ColumnTypes = GetColumnsType(Of T)().ToArray()
            }
            Dim columns = GetColumns(Of T)(False)
            table.AddColumn(GetColumns(Of T))
            For Each propertyValues In values.[Select](Function(value) columns.[Select](Function(column) GetColumnValue(Of T)(value, column)))
                table.AddRow(propertyValues.ToArray())
            Next

            Return table
        End Function


        Public Overloads Function ToString(ByVal format As Format) As String
            Select Case format
                Case Format.MarkDown
                    Return ToMarkDownString()
                Case Format.Alternative
                    Return ToStringAlternative()
                Case Format.Minimal
                    Return ToMinimalString()
                Case Else
                    Return ToString()
            End Select
        End Function


        Public Overrides Function ToString() As String
            Dim builder = New StringBuilder()
            Dim columnLengths = Me.ColumnLengths()
            Dim columnAlignment = Enumerable.Range(0, Columns.Count).[Select](Function(x) GetNumberAlignment(x)).ToList()
            Dim format = Enumerable.Range(0, Columns.Count).[Select](Function(i) " | {" & i & "," & columnAlignment(i) & columnLengths(i) & "}").Aggregate(Function(s, a) s & a) & " |"
            Dim maxRowLength = Math.Max(0, If(Rows.Any(), Rows.Max(Function(row) String.Format(format, row).Length), 0))
            Dim columnHeaders = String.Format(format, Columns.ToArray())
            Dim longestLine = Math.Max(maxRowLength, columnHeaders.Length)
            Dim results = Rows.[Select](Function(row) String.Format(format, row)).ToList()
            Dim divider = " " & String.Join("", Enumerable.Repeat("-", longestLine - 1)) & " "
            builder.AppendLine(divider)
            builder.AppendLine(columnHeaders)

            For Each row In results
                builder.AppendLine(divider)
                builder.AppendLine(row)
            Next

            builder.AppendLine(divider)

            If Options.EnableCount Then
                builder.AppendLine("")
                builder.AppendFormat(" Count: {0}", Rows.Count)
            End If

            Return builder.ToString()
        End Function

        Public Function ToMarkDownString() As String
            Return ToMarkDownString("|"c)
        End Function

        Private Function ToMarkDownString(ByVal delimiter As Char) As String
            Dim builder = New StringBuilder()
            Dim columnLengths = Me.ColumnLengths()
            Dim format = Me.Format(columnLengths, delimiter)
            Dim columnHeaders = String.Format(format, Columns.ToArray())
            Dim results = Rows.Select(Function(row) String.Format(format, row)).ToList()
            Dim divider = Regex.Replace(columnHeaders, "[^|]", "-")
            builder.AppendLine(columnHeaders)
            builder.AppendLine(divider)
            results.ForEach(Sub(row) builder.AppendLine(row))
            Return builder.ToString()
        End Function

        Public Function ToMinimalString() As String
            Return ToMarkDownString(Char.MinValue)
        End Function

        Public Function ToStringAlternative() As String
            Dim builder = New StringBuilder()
            Dim columnLengths = Me.ColumnLengths
            Dim format = Me.Format(columnLengths)
            Dim columnHeaders = String.Format(format, Columns.ToArray())
            Dim results = Rows.[Select](Function(row) String.Format(format, row)).ToList()
            Dim divider = Regex.Replace(columnHeaders, "[^|]", "-")
            Dim dividerPlus = divider.Replace("|", "+")
            builder.AppendLine(dividerPlus)
            builder.AppendLine(columnHeaders)

            For Each row In results
                builder.AppendLine(dividerPlus)
                builder.AppendLine(row)
            Next

            builder.AppendLine(dividerPlus)
            Return builder.ToString()
        End Function

        Private Function Format(ByVal columnLengths As List(Of Integer), ByVal Optional delimiter As Char = "|"c) As String
            Dim columnAlignment = Enumerable.Range(0, Columns.Count).[Select](Function(x) GetNumberAlignment(x)).ToList()
            Dim delimiterStr = If(delimiter = Char.MinValue, String.Empty, delimiter.ToString())
            Return (Enumerable.Range(0, Columns.Count).[Select](Function(i) " " & delimiterStr & " {" & i & "," & columnAlignment(i) & columnLengths(i) & "}").Aggregate(Function(s, a) s & a) & " " & delimiterStr).Trim()

        End Function

        Private Function GetNumberAlignment(ByVal i As Integer) As String
            Return If(Options.NumberAlignment = Alignment.Right AndAlso ColumnTypes IsNot Nothing AndAlso PrimitiveNumericTypes.Contains(ColumnTypes(i)), "", "-")
        End Function

        Private Function ColumnLengths() As List(Of Integer)
            Return Columns.[Select](Function(t, i) Rows.[Select](Function(x) x(i)).Union({Columns(i)}).Where(Function(x) x IsNot Nothing).[Select](Function(x) x.ToString().Length).Max()).ToList()
        End Function

        Public Sub Write(ByVal Optional format As Format = ConsoleTables.Format.[Default])
            Options.OutputTo.WriteLine(ToString(format))
        End Sub

        Private Shared Function GetColumns(Of T)(Optional FixCase As Boolean = True) As IEnumerable(Of String)
            Return GetType(T).GetProperties().[Select](Function(x) If(Not FixCase, x.Name, x.Name.ToNormalCase().ToTitle())).ToArray()
        End Function

        Private Shared Function GetColumnValue(Of T)(ByVal target As Object, ByVal column As String) As Object
            Return GetType(T).GetProperty(column).GetValue(target, Nothing)
        End Function

        Private Shared Function GetColumnsType(Of T)() As IEnumerable(Of Type)
            Return GetType(T).GetTypeOf().GetProperties().[Select](Function(x) x.PropertyType).ToArray()
        End Function

    End Class

    Public Class ConsoleTableOptions
        Public Property Columns As List(Of String) = New List(Of String)()
        Public Property EnableCount As Boolean = True
        Public Property NumberAlignment As Alignment = Alignment.Left
        Public Property OutputTo As TextWriter = System.Console.Out
    End Class

    Public Enum Format
        [Default] = 0
        MarkDown = 1
        Alternative = 2
        Minimal = 3
    End Enum

    Public Enum Alignment
        Left
        Right
    End Enum

End Namespace