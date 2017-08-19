Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Windows.Forms
Imports System.Xml
Imports InnerLibs

Public NotInheritable Class DataBase

    ''' <summary>
    ''' Estrutura que imita um <see cref="DbDataReader"/> usando estruturas de  <see cref="Dictionary"/>. Permite a leitura
    ''' releitura, atribuição e serialização mesmo após o fechamento da conexão.
    ''' </summary>
    Public NotInheritable Class Reader
        Inherits List(Of List(Of Dictionary(Of String, Object)))
        Implements IDisposable

        ''' <summary>
        ''' Retorna um Json do objeto
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Json.SerializeJSON(Me)
        End Function

        ''' <summary>
        ''' Esvazia o reader
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            MyBase.Clear()
            ' Suppress finalization.
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' Esvazia e destroi o reader
        ''' </summary>
        Public Sub Close()
            Me.Dispose()
        End Sub

        ''' <summary>
        ''' Verifica se o Reader está vazio ou fechado ( <see cref="Count"/> = 0)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsClosed
            Get
                Return MyBase.Count = 0
            End Get
        End Property

        ''' <summary>
        ''' Retorna as colunas do resultado atual
        ''' </summary>
        ''' <returns></returns>
        Public Function GetColumns() As List(Of String)
            If HasResults AndAlso HasRows Then
                Return MyBase.Item(resultindex)(rowindex.SetMinValue(0)).Keys.ToList
            Else
                Return New List(Of String)
            End If
        End Function

        ''' <summary>
        ''' Retorna o numero de linhas do resultado atual
        ''' </summary>
        ''' <returns></returns>
        Public Function CountRows() As Integer
            If HasRows Then
                Return MyBase.Item(resultindex).Count
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' Retorna o numero de linhas de um resultado
        ''' </summary>
        ''' <returns></returns>
        Public Function CountRows(ResultIndex As Integer) As Integer
            If HasRows And ResultIndex.IsBetween(0, MyBase.Count - 1) Then
                Return MyBase.Item(ResultIndex).Count
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' Retorna o valor da coluna do resultado e linha atual a partir do nome da coluna
        ''' </summary>
        ''' <param name="ColumnName">Nome da Coluna</param>
        ''' <returns></returns>
        Default Public Shadows ReadOnly Property Item(ColumnName As String) As Object
            Get
                Try
                    If rowindex < 0 Then rowindex = 0
                    Return MyBase.Item(resultindex)(rowindex)(ColumnName)
                Catch ex As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Retorna o valor da coluna do resultado e linha atual a partir do nome da coluna convertendo para um outro tipo
        ''' </summary>
        ''' <typeparam name="Type"></typeparam>
        ''' <param name="ColumnName"></param>
        ''' <returns></returns>
        Public Function GetItem(Of Type)(ColumnName As String) As Type
            Try
                Return CType(Item(ColumnName), Type)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Retorna o valor da coluna do resultado e linha atual a partir do índice da coluna convertendo para um outro tipo
        ''' </summary>
        ''' <typeparam name="Type"></typeparam>
        ''' <param name="ColumnIndex"></param>
        ''' <returns></returns>
        Public Function GetItem(Of Type)(ColumnIndex As Integer) As Type
            Try
                Return CType(Item(ColumnIndex), Type)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Retorna o valor da coluna do resultado de uma linha especifica a partir do nome da coluna
        ''' e o Index da linha
        ''' </summary>
        ''' <param name="ColumnName">Nome da Coluna</param>
        ''' <param name="RowIndex">  Índice da Linha</param>
        ''' <returns></returns>
        Default Public Shadows ReadOnly Property Item(ColumnName As String, RowIndex As Integer) As Object
            Get
                Try
                    If RowIndex < 0 Then RowIndex = 0
                    Return MyBase.Item(resultindex)(RowIndex)(ColumnName)
                Catch ex As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Retorna o valor da coluna do resultado e linha atual a partir do índice da coluna
        ''' </summary>
        ''' <param name="ColumnIndex">Índice da Coluna</param>
        ''' <returns></returns>
        Default Public Shadows ReadOnly Property Item(ColumnIndex As Integer) As Object
            Get
                Try
                    If rowindex < 0 Then rowindex = 0
                    Return MyBase.Item(resultindex)(rowindex)(MyBase.Item(resultindex)(rowindex).Keys(ColumnIndex))
                Catch ex As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Retorna o valor da coluna do resultado de uma linha especifica a partir do nome da coluna
        ''' e o Index da linha
        ''' </summary>
        ''' <param name="ColumnIndex">Índice da Coluna</param>
        ''' <param name="RowIndex">   Índice da Linha</param>
        ''' <returns></returns>
        Default Public Shadows ReadOnly Property Item(ColumnIndex As Integer, RowIndex As Integer) As Object
            Get
                Try
                    Return MyBase.Item(resultindex)(RowIndex)(MyBase.Item(resultindex)(RowIndex).Keys(ColumnIndex))
                Catch ex As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Verifica se o resultado atual possui linhas
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HasRows As Boolean
            Get
                Return HasResults AndAlso MyBase.Item(resultindex).Count > 0
            End Get
        End Property

        ''' <summary>
        ''' Verifica se exitem resultados
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HasResults As Boolean
            Get
                Return MyBase.Count > 0
            End Get
        End Property

        ''' <summary>
        ''' Retorna um resultado (tabela) a partir do seu Index.
        ''' </summary>
        ''' <param name="ResultIndex">Indice do resultado</param>
        ''' <returns></returns>
        Function GetResult(ResultIndex As Integer) As List(Of Dictionary(Of String, Object))
            Return MyBase.Item(ResultIndex)
        End Function

        ''' <summary>
        ''' Retorna uma linha do resultado atual
        ''' </summary>
        ''' <param name="RowIndex"></param>
        ''' <returns></returns>
        Public Function GetRow(RowIndex As Integer) As Dictionary(Of String, Object)
            If HasRows Then
                Return MyBase.Item(resultindex)(RowIndex)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Retorna a linha atual do resultado atual
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentRow() As Dictionary(Of String, Object)
            Return GetRow(rowindex)
        End Function

        ''' <summary>
        ''' Retorna um Json do Reader
        ''' </summary>
        ''' <returns></returns>
        Function ToJSON(Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
            Try
                Return Me.SerializeJSON(DateFormat)
            Catch ex As Exception
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' Retorna o Json do resultado especifico
        ''' </summary>
        ''' <param name="ResultIndex">Índice do resultado</param>
        ''' <returns></returns>
        Function ToJSON(ResultIndex As Integer, Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
            If HasRows Then
                Return MyBase.Item(ResultIndex).SerializeJSON(DateFormat)
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' Encontra linhas onde qualquer valor de suas colunas conter um determinado valor
        ''' </summary>
        ''' <param name="Value">Valor</param>
        ''' <returns></returns>
        Function Search(Value As Object) As List(Of Dictionary(Of String, Object))
            Try
                Return MyBase.Item(resultindex).Where(Function(x, y) x.Values(y).ToString().Contains(Value.ToString)).ToList
            Catch ex As Exception
                Return New List(Of Dictionary(Of String, Object))
            End Try
        End Function

        ''' <summary>
        ''' Cria um novo Reader a partir de uma coleçao de listas de Dicionários
        ''' </summary>
        ''' <param name="Tables">Conunto de listas de dicionarios</param>
        Public Sub New(ParamArray Tables As List(Of Dictionary(Of String, Object))())
            For Each l In Tables
                Me.Add(l)
            Next
        End Sub

        ''' <summary>
        ''' Cria um novo Reader a partir de uma coleção de dicionários
        ''' </summary>
        ''' <param name="Rows">Conunto de Dicionários</param>
        Public Sub New(ParamArray Rows As IDictionary(Of String, Object)())
            Me.New(New List(Of Dictionary(Of String, Object))(Rows))
        End Sub

        ''' <summary>
        ''' Cria um novo Reader a partir de um <see cref="DbDataReader"/>
        ''' </summary>
        ''' <param name="Reader">Reader</param>
        Public Sub New(Reader As DbDataReader)
            Using Reader
                Do
                    Dim listatabela As New List(Of Dictionary(Of String, Object))
                    Dim columns = New List(Of String)
                    For i As Integer = 0 To Reader.FieldCount - 1
                        columns.Add(Reader.GetName(i))
                    Next
                    While Reader.Read
                        Dim lista As New Dictionary(Of String, Object)
                        For Each col In columns
                            If Not lista.Keys.Contains(col) Then
                                Try
                                    If Reader(col).GetType = GetType(String) Then
                                        lista.Add(col, If(IsDBNull(Reader(col)), "", Json.DeserializeJSON(Reader(col))))
                                    Else
                                        Try
                                            lista.Add(col, If(IsDBNull(Reader(col)), Nothing, Reader(col)))
                                        Catch ex2 As Exception
                                            lista.Add(col, Nothing)
                                        End Try
                                    End If
                                Catch ex As Exception
                                    Try
                                        lista.Add(col, If(IsDBNull(Reader(col)), Nothing, Reader(col)))
                                    Catch ex2 As Exception
                                        lista.Add(col, Nothing)
                                    End Try
                                End Try
                            End If
                        Next
                        listatabela.Add(lista)
                    End While
                    Me.Add(listatabela)
                Loop While Reader.NextResult
            End Using
        End Sub

        Private resultindex As Integer = 0
        Private rowindex As Integer = -1

        ''' <summary>
        ''' Reinicia a leitura do Reader retornando os índices para seus valores padrão, é um alias
        ''' para <see cref="Reset()"/>)
        ''' </summary>
        Public Sub StartOver()
            resultindex = 0
            rowindex = -1
        End Sub

        ''' <summary>
        ''' Reinicia a leitura do Reader retornando os índices para seus valores padrão
        ''' </summary>
        Public Sub Reset()
            StartOver()
        End Sub

        ''' <summary>
        ''' Avança para o próximo resultado
        ''' </summary>
        ''' <returns></returns>
        Function NextResult() As Boolean
            If resultindex < Me.Count - 1 Then
                resultindex.Increment
                rowindex = -1
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Retorna para os resultado anterior
        ''' </summary>
        ''' <returns></returns>
        Function PreviousResult() As Boolean
            If resultindex > 0 Then
                resultindex.Decrement
                rowindex = -1
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Avança para o próximo registro, é um alias para <see cref="GoForward()"/>
        ''' </summary>
        ''' <returns></returns>
        Function Read() As Boolean
            Return GoForward()
        End Function

        ''' <summary>
        ''' Retornar para o registro anterior
        ''' </summary>
        ''' <returns></returns>
        Function GoBack() As Boolean
            If rowindex > 0 Then
                rowindex.Decrement
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Avança para o próximo registro
        ''' </summary>
        ''' <returns></returns>
        Function GoForward() As Boolean
            If rowindex < CountRows() - 1 Then
                rowindex.Increment
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Cria um array de <see cref="ListItem"/> com os Itens de um <see cref="DataBase.Reader"/>
        ''' </summary>
        ''' <param name="TextColumn">    Coluna que será usada como Texto do elemento option</param>
        ''' <param name="ValueColumn">   Coluna que será usada como Value do elemento option</param>
        ''' <param name="SelectedValues">Valores Selecionados</param>
        ''' <returns></returns>
        Public Function ToListItems(TextColumn As String, Optional ValueColumn As String = "", Optional SelectedValues As String() = Nothing) As ListItem()
            Dim h As New List(Of ListItem)
            If Me.HasRows Then
                While Me.Read()
                    ValueColumn = If(ValueColumn.IsBlank, TextColumn, ValueColumn)
                    h.Add(New ListItem(Me.GetItem(Of String)(TextColumn), Me.GetItem(Of String)(ValueColumn)) With {.Selected = (Not IsNothing(SelectedValues) AndAlso .Value.IsIn(SelectedValues))})
                End While
            End If
            Return h.ToArray
        End Function

        ''' <summary>
        ''' Preenche um <see cref="HtmlSelect"/> com itens do <see cref="DataBase.Reader"/>
        ''' </summary>
        ''' <param name="[SelectControl]">Controle HtmlSelect</param>
        ''' <param name="TextColumn">     Coluna que será usada como Texto do elemento option</param>
        ''' <param name="ValueColumn">    Coluna que será usada como Value do elemento option</param>
        ''' '
        ''' <param name="SelectedValues"> Valores Selecionados</param>
        Public Sub FillSelectControl(ByRef [SelectControl] As HtmlSelect, TextColumn As String, Optional ValueColumn As String = "", Optional SelectedValues As String() = Nothing)
            SelectControl.Items.AddRange(Me.ToListItems(TextColumn, ValueColumn, SelectedValues))
        End Sub

        ''' <summary>
        ''' Cria uma lista de com os Itens de um <see cref="DataBase.Reader"/> convertendo os valores
        ''' para uma classe ou tipo especifico
        ''' </summary>
        ''' <param name="Column">Coluna que será usada</param>
        ''' <returns></returns>
        Public Function ToList(Of TValue)(Column As String) As List(Of TValue)
            Dim h As New List(Of TValue)
            If Me.HasRows Then
                While Me.Read()
                    Try
                        h.Add(CType(Me(Column), TValue))
                    Catch ex As Exception
                    End Try
                End While
            End If
            Return h
        End Function

        ''' <summary>
        ''' Cria uma lista de uma classe específica com os Itens de um <see cref="DataBase.Reader"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function ToList(Of TValue)() As List(Of TValue)
            Dim h As New List(Of TValue)
            If Me.HasRows Then
                While Me.Read()
                    Dim i = Activator.CreateInstance(Of TValue)
                    For Each prop As PropertyInfo In GetType(TValue).GetProperties
                        If prop.CanWrite Then
                            prop.SetValue(i, Me(prop.Name))
                        End If
                    Next
                    h.Add(i)
                End While
            End If
            Return h
        End Function

        ''' <summary>
        ''' Retorna o valor de uma coluna especifica de um resultado de um <see cref="DataBase.Reader"/>
        ''' </summary>
        ''' <param name="Column">     Coluna</param>
        ''' <param name="ResultIndex">Indice do resultado</param>
        ''' <param name="RowIndex">   Indice da linha dor resultado</param>
        ''' <returns></returns>
        Public Function GetValue(ResultIndex As Integer, RowIndex As Integer, Column As String) As Object
            Return Me.GetResult(ResultIndex).Item(RowIndex).Item(Column)
        End Function

        ''' <summary>
        ''' Retorna o valor de uma coluna especifica de um resultado de um <see cref="DataBase.Reader"/>
        ''' </summary>
        ''' <typeparam name="Type">Tipo para qual o valor será convertido</typeparam>
        ''' <param name="Column">     Coluna</param>
        ''' <param name="ResultIndex">Indice do resultado</param>
        ''' <param name="RowIndex">   Indice da linha dor resultado</param>
        ''' <returns></returns>
        Public Function GetValue(Of Type)(ResultIndex As Integer, RowIndex As Integer, Column As String) As Type
            Return CType(Me.GetResult(ResultIndex).Item(RowIndex).Item(Column), Type)
        End Function

        ''' <summary>
        ''' Retorna um Array de Valores da linha atual
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentRowValues() As Object()
            Return MyBase.Item(resultindex).Item(rowindex).Values.ToArray
        End Function

        ''' <summary>
        ''' Cria um Dictionary a partir de <see cref="DataBase.Reader"/> usando uma coluna como Key e
        ''' outra como Value
        ''' </summary>
        ''' <param name="KeyColumn">  Coluna que será usada como Key do dicionario</param>
        ''' <param name="ValueColumn">Coluna que será usada como Value do dicionario</param>
        ''' <returns></returns>

        Public Function ToDictionary(Of TKey, TValue)(KeyColumn As String, ValueColumn As String) As Dictionary(Of TKey, TValue)
            Dim h As New Dictionary(Of TKey, TValue)
            If Me.HasRows Then
                While Me.Read()
                    h.Add(Me(KeyColumn), Me(ValueColumn))
                End While
            End If
            Return h
        End Function

        ''' <summary>
        ''' Cria uma lista de pares com os Itens de um <see cref="DataBase.Reader"/>
        ''' </summary>
        ''' <param name="TextColumn"> Coluna que será usada como Text do item</param>
        ''' <param name="ValueColumn">Coluna que será usada como Value do item</param>
        ''' <returns></returns>
        Public Function ToTextValueList(Of TValue)(TextColumn As String, Optional ValueColumn As String = "") As TextValueList(Of TValue)
            If ValueColumn.IsBlank Then ValueColumn = TextColumn
            Dim h As New TextValueList(Of TValue)
            If Me.HasRows Then
                While Me.Read()
                    h.Add("" & Me(TextColumn), CType(Me(ValueColumn), TValue))
                End While
            End If
            Return h
        End Function

        ''' <summary>
        ''' Transforma o resultado de um <see cref="DataBase.Reader"/> em QueryString
        ''' </summary>
        ''' <returns></returns>
        Public Function ToQueryString() As String
            Dim param As String = ""
            If Me.HasRows Then
                While Me.Read()
                    For Each it In Me.GetColumns()
                        param.Append("&" & it & "=" & HttpUtility.UrlEncode("" & Me(it)))
                    Next
                End While
            End If
            Return param.RemoveFirstIf("?").Prepend("?")

        End Function

        ''' <summary>
        ''' Transforma o resultado de um <see cref="DataBase.Reader"/> em uma URL
        ''' </summary>
        ''' <returns></returns>
        Public Function ToUrl(BaseUrl As String) As String
            If BaseUrl.IsURL Then
                Return BaseUrl.Split("?").First & ToQueryString()
            Else
                Throw New ArgumentException("BaseUrl is no a valid URL")
            End If
        End Function

        ''' <summary>
        ''' Transforma um <see cref="DataBase.Reader"/> em uma string delimitada por caracteres
        ''' </summary>
        ''' <param name="ColDelimiter">  Delimitador de Coluna</param>
        ''' <param name="RowDelimiter">  Delimitador de Linha</param>
        ''' <param name="TableDelimiter">Delimitador de Tabelas</param>
        ''' <returns>Uma string delimitada</returns>

        Public Function ToDelimitedString(Optional ColDelimiter As String = "[ColDelimiter]", Optional RowDelimiter As String = "[RowDelimiter]", Optional TableDelimiter As String = "[TableDelimiter]") As String
            Dim DelimitedString As String = ""
            Do
                Dim columns As List(Of String) = Me.GetColumns()
                While Me.Read()
                    For Each coluna In columns
                        DelimitedString = DelimitedString & (Me(coluna) & ColDelimiter)
                    Next
                    DelimitedString = DelimitedString & RowDelimiter
                End While
                DelimitedString = DelimitedString & TableDelimiter
            Loop While Me.NextResult()
            Return DelimitedString
        End Function

        Public Function ToDataTable() As DataTable
            Dim result As New DataTable()
            If MyBase.Item(resultindex).Count = 0 Then
                Return result
            End If
            result.Columns.AddRange(MyBase.Item(resultindex)(0).[Select](Function(r) New DataColumn(r.Key)).ToArray())
            For Each i As Dictionary(Of String, Object) In MyBase.Item(resultindex)
                result.Rows.Add(i.Values.ToArray)
            Next
            Return result
        End Function

        ''' <summary>
        ''' Converte um <see cref="DataBase.Reader"/> em XML
        ''' </summary>
        ''' <param name="ItemName"> Nome do nó que representa cada linha</param>
        ''' '
        ''' <param name="TableName">Nome do nó principal do documento</param>

        Public Function ToXML(Optional TableName As String = "Table", Optional ItemName As String = "Row") As XmlDocument

            Dim doc As New XmlDocument
            Dim stringas = ""
            Dim dt = Me.ToDataTable
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
        ''' Converte um <see cref="DataBase.Reader"/> em CSV
        ''' </summary>
        ''' <param name="Separator">Separador de valores (vírgula)</param>
        ''' <returns>uma string Comma Separated Values (CSV)</returns>

        Public Function ToCSV(Optional Separator As String = ";") As String

            Dim Returned As String = "sep=" & Separator & Environment.NewLine
            If Me.HasRows Then
                While Me.Read()
                    For Each item As String In Me.GetColumns()
                        Returned.Append(Me(item).ToString().Quote & Separator)
                    Next
                    Returned.Append(Environment.NewLine)
                End While
            End If
            Return Returned
        End Function

        ''' <summary>
        ''' Copia a primeira linha de um <see cref="DataBase.Reader"/> para uma sessão
        ''' HttpSessionState usando os nomes das colunas como os nomes dos objetos da sessão
        ''' </summary>
        ''' <param name="Session">Objeto da sessão</param>
        ''' <param name="Timeout">
        ''' Tempo em minutos para a sessão expirar (se não especificado não altera o timeout da sessão)
        ''' </param>

        Public Sub ToSession(Session As HttpSessionState, Optional Timeout As Integer = 0)
            For Each coluna In Me.GetColumns()
                Session(coluna) = Nothing
            Next
            While Me.Read()
                For Each coluna In Me.GetColumns()
                    Session(coluna) = Me(coluna)
                Next
            End While
            If Timeout > 0 Then
                Session.Timeout = Timeout
            End If
        End Sub

        ''' <summary>
        ''' Converte um <see cref="DataBase.Reader"/> para uma tabela em Markdown Pipe
        ''' </summary>
        ''' <returns></returns>
        Public Function ToMarkdownTable() As String
            Dim Returned As String = ""
            Do
                If Me.HasRows Then
                    Dim header As String = ""
                    Dim base As String = ""
                    For Each item As String In Me.GetColumns()
                        header.Append("|" & item)
                        base.Append("|" & item.Censor("-", item))
                    Next
                    header.Append("|" & Environment.NewLine)
                    base.Append("|" & Environment.NewLine)

                    Returned.Append(header)
                    Returned.Append(base)

                    While Me.Read()
                        For Each item As String In Me.GetColumns()
                            Returned.Append("|" & Me(item))
                        Next
                        Returned.Append("|" & Environment.NewLine)
                    End While
                End If
                Returned.Append(Environment.NewLine)
                Returned.Append(Environment.NewLine)
            Loop While Me.NextResult()
            Return Returned

        End Function

        ''' <summary>
        ''' Converte um <see cref="DataBase.Reader"/> para uma tabela em HTML
        ''' </summary>
        ''' <param name="Attr">Atributos da tabela.</param>
        ''' <returns></returns>
        Public Function ToHTMLTable(Optional Attr As IDictionary(Of String, String) = Nothing) As HtmlTag
            If IsNothing(Attr) Then Attr = New SortedDictionary(Of String, String)
            Dim Returned As String = ""
            Do
                If Me.HasRows Then
                    Returned.Append(" <thead>")
                    Returned.Append("     <tr>")
                    For Each item As String In Me.GetColumns()
                        Returned.Append("         <th>" & item & "</th>")
                    Next
                    Returned.Append("     </tr>")
                    Returned.Append(" </thead>")
                    Returned.Append(" <tbody>")
                    While Me.Read()
                        Returned.Append("     <tr>")
                        For Each item As String In Me.GetColumns()
                            Returned.Append(" <td>" & Me(item) & "</td>")
                        Next
                        Returned.Append("     </tr>")
                    End While
                    Returned.Append(" </tbody>")

                End If
            Loop While Me.NextResult()
            Dim tag As New HtmlTag("<table></table>")
            tag.Attributes = Attr
            tag.InnerHtml = Returned
            Return tag
        End Function

        ''' <summary>
        ''' Aplica os valores encontrados nas colunas de um <see cref="DataBase.Reader"/> em controles
        ''' com mesmo ID das colunas. Se os conroles não existirem no resultado eles serão ignorados.
        ''' </summary>
        ''' <param name="Controls">Controles que serão Manipulados</param>
        ''' <returns>Um array contendo os inputs manipulados</returns>
        Public Function ApplyToControls(ParamArray Controls As System.Web.UI.HtmlControls.HtmlControl()) As System.Web.UI.HtmlControls.HtmlControl()
            For Each c In Controls
                Try
                    Select Case c.TagName.ToLower
                        Case "input"
                            CType(c, HtmlInputControl).Value = Me(c.ID)
                        Case "select"
                            CType(c, HtmlSelect).SelectValues(Me(c.ID))
                        Case Else
                            CType(c, HtmlContainerControl).InnerHtml = Me(c.ID)
                    End Select
                Catch ex As Exception
                End Try
            Next
            Return Controls
        End Function

        ''' <summary>
        ''' Preenche um DataGrivView com os resultados
        ''' </summary>
        ''' <param name="DatagridView"></param>
        ''' <returns></returns>
        Public Function FillDataGridView(ByRef DataGridView As DataGridView, Optional ResultIndex As Integer = 0) As DataGridView
            DataGridView.Rows.Clear()
            DataGridView.Columns.Clear()
            For Each c In GetColumns()
                DataGridView.Columns.Add(c, c)
            Next
            StartOver()
            While Me.resultindex < ResultIndex
                ResultIndex.Increment
            End While
            While Read()
                DataGridView.Rows.Add(GetCurrentRowValues)
            End While
            Return DataGridView
        End Function

        ''' <summary>
        ''' Preenche um combobox com um TextValueList criado a partir deste DataBase.Reader
        ''' </summary>
        ''' <typeparam name="TValue">Tipo do Valor da coluna</typeparam>
        ''' <param name="ComboBox">   </param>
        ''' <param name="TextColumn"> Coluna usada como Texto do ComboBox</param>
        ''' <param name="ValueColumn">Coluna usada como Valor do ComboBox</param>
        ''' <returns></returns>
        Public Function FillComboBox(Of TValue)(ByRef ComboBox As ComboBox, TextColumn As String, Optional ValueColumn As String = Nothing) As ComboBox
            ComboBox.SetPairDataSource(Of TValue)(Me.ToTextValueList(Of TValue)(TextColumn, ValueColumn))
            Return ComboBox
        End Function

    End Class

    ''' <summary>
    ''' Cria um parametro de Query SQL a partir de uma variavel convertida para um tipo especifico
    ''' </summary>
    ''' <typeparam name="Type">Tipo</typeparam>
    ''' <param name="Name"> Nome do Parametro</param>
    ''' <param name="Value">Valor do Parametro</param>
    ''' <returns></returns>
    Public Function CreateParameter(Of Type)(Name As String, Value As Object) As DbParameter
        Return CreateParameter(Name, CType(Value, Type))
    End Function

    ''' <summary>
    ''' Cria um comando de INSERT baseado em um <see cref="IDictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Dic">Dicionario contendo os Valores</param>
    ''' <returns></returns>
    Public Function CreateInsertCommand(TableName As String, Dic As IDictionary(Of String, Object)) As DbCommand
        Return Me.CreateCommandFromDictionary(Me.CreateInsertCommandText(TableName, Dic.Where(Function(p) Not IsNothing(p)).Select(Function(p) p.Key).ToArray), Dic)
    End Function

    ''' <summary>
    ''' Cria um comando de INSERT
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Columns">Colunas do INSERT</param>
    ''' <returns></returns>
    Public Function CreateInsertCommandText(TableName As String, ParamArray Columns As String()) As String
        Return String.Format("INSERT INTO " & TableName & " ({0}) values (@{1})", Columns.Join(","), Columns.Join(", @").RemoveLastIf("@"))
    End Function

    ''' <summary>
    ''' Cria um comando de UPDATE baseado em um <see cref="IDictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Dic">Dicionario contendo os Valores</param>
    ''' <returns></returns>
    Public Function CreateUpdateCommand(TableName As String, WhereClausule As String, Dic As IDictionary(Of String, Object)) As DbCommand
        Return Me.CreateCommandFromDictionary(Me.CreateUpdateCommandText(TableName, WhereClausule, Dic.Where(Function(p) Not IsNothing(p)).Select(Function(p) p.Key).ToArray), Dic)
    End Function

    ''' <summary>
    ''' Cria um comando de UPDATE
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Columns">Colunas do INSERT</param>
    ''' <returns></returns>
    Public Function CreateUpdateCommandText(TableName As String, WhereClausule As String, ParamArray Columns As String()) As String
        Dim cmd As String = "UPDATE " & TableName & Environment.NewLine & " set "
        For Each col In Columns
            cmd.Append(String.Format(" {0} = @{0},", col) & Environment.NewLine)
        Next
        cmd = cmd.TrimAny(Environment.NewLine, " ", ",") & If(WhereClausule.IsNotBlank, " WHERE " & WhereClausule.TrimAny(" ", "where", "WHERE"), "")
        Return cmd
    End Function

    ''' <summary>
    ''' Faz um INSERT out UPDATE no banco de dados de acordo com o valor da coluna de chave primária especificado em um <see cref="IDictionary"/>
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Dic">Dicionário contendo os valores</param>
    ''' <param name="PrimaryKeyColumn">Nome da coluna de chave primária (Ela deve estar dentro do <see cref="IDictionary"/> especificado no parametro <paramref name="Dic"/>, caso contrário será processado como INSERT </param>
    ''' <returns></returns>
    Public Function INSERTorUPDATE(TableName As String, PrimaryKeyColumn As String, Dic As IDictionary(Of String, Object)) As String
        If Dic.ContainsKey(PrimaryKeyColumn) AndAlso CType(Dic(PrimaryKeyColumn).ToString.IfBlank(Of String)(0), Decimal) > 0 Then
            UPDATE(TableName, PrimaryKeyColumn & " = " & Dic(PrimaryKeyColumn).ToString.IsNull(Quotes:=False), Dic.Where(Function(p) p.Key <> PrimaryKeyColumn).ToDictionary(Function(p) p.Key, Function(p) p.Value))
            Return "UPDATE"
        Else
            INSERT(TableName, Dic.Where(Function(p) p.Key <> PrimaryKeyColumn).ToDictionary(Function(p) p.Key, Function(p) p.Value))
            Return "INSERT"
        End If
    End Function

    ''' <summary>
    ''' Faz um UPDATE no banco de dados de acordo com um <see cref="IDictionary"/>
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Dic">Dicionário contendo os valores</param>
    ''' <param name="WhereClausule">Clausula WHERE, condiçoes para realizar o UPDATE</param>
    ''' <returns></returns>
    Public Function UPDATE(TableName As String, WhereClausule As String, Dic As IDictionary(Of String, Object)) As DataBase.Reader
        Return RunSQL(CreateUpdateCommand(TableName, WhereClausule, Dic))
    End Function

    ''' <summary>
    ''' Faz um INSERT no banco de dados de acordo com um  <see cref="IDictionary"/>
    ''' </summary>
    ''' <param name="TableName">Nome da Tabela</param>
    ''' <param name="Dic">Dicionário contendo os valores</param>
    ''' <returns></returns>
    Public Function INSERT(TableName As String, Dic As IDictionary(Of String, Object)) As DataBase.Reader
        Return RunSQL(CreateInsertCommand(TableName, Dic))
    End Function

    ''' <summary>
    ''' Cria um parametro de Query SQL a partir de uma variavel
    ''' </summary>
    ''' <param name="Name"> Nome do Parametro</param>
    ''' <param name="Value">Valor do Parametro</param>
    ''' <returns></returns>
    Public Function CreateParameter(Name As String, Value As Object) As DbParameter
        Using con = Activator.CreateInstance(ConnectionType)
            con.ConnectionString = Me.ConnectionString
            con.Open()
            Using command As DbCommand = con.CreateCommand()
                Dim param = command.CreateParameter()
                param.DbType = DataManipulation.GetDbType(Value)
                param.ParameterName = "@" & Name.TrimAny("@", " ")
                Dim valor As Object = DBNull.Value
                If Not IsNothing(Value) AndAlso Not IsNothing(Value.GetType) Then
                    Select Case Value.GetType()
                        Case GetType(String), GetType(Char())
                            valor = Value.ToString
                        Case GetType(Char)
                            valor = Value.ToString.GetFirstChars
                        Case GetType(Byte())
                            If Value.LongLength > 0 Then
                                valor = Value
                            Else
                                valor = New Byte()
                            End If
                        Case GetType(HttpPostedFile)
                            If CType(Value, HttpPostedFile).ContentLength > 0 Then
                                Return Me.CreateParameter(Name, Value.ToBytes())
                            End If
                        Case GetType(FileInfo)
                            If CType(Value, FileInfo).Length > 0 Then
                                Return Me.CreateParameter(Name, Value.ToBytes())
                            End If
                        Case GetType(Drawing.Image)
                            If Not IsNothing(Value) Then
                                Return Me.CreateParameter(Name, Value.ToBytes())
                            End If
                        Case GetType(Date), GetType(DateTime)
                            If CType(Value, Date) = DateTime.MinValue Then
                                Return Me.CreateParameter(Of DateTime)(Name, Nothing)
                            Else
                                valor = Value
                            End If
                        Case GetType(Short), GetType(Integer), GetType(Long), GetType(Decimal), GetType(Double), GetType(Byte)
                            valor = Value
                        Case Else
                            Return Me.CreateParameter(Of String)(Name, Json.SerializeJSON(Value))
                    End Select
                Else

                    valor = DBNull.Value
                End If
                param.Value = valor
                Return param
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Retorna a lista de arquivos SQL disponiveis
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSqlFilesList() As List(Of String)
        Dim l As New List(Of String)
        Select Case True
            Case IsNothing(ApplicationAssembly) And Not IsNothing(CommandDirectory)
                For Each f In CommandDirectory.Search(SearchOption.AllDirectories, "*.sql")
                    l.Add(f.Name)
                Next
            Case Not IsNothing(ApplicationAssembly) And IsNothing(CommandDirectory)
                For Each n In ApplicationAssembly.GetManifestResourceNames()
                    If Path.GetExtension(n).IsAny(".sql", "sql", "SQL", ".SQL") Then
                        l.Add(n)
                    End If
                Next
            Case Else
                Throw New Exception("ApplicationAssembly or CommandDirectory is not configured!")
        End Select
        Return l
    End Function

    ''' <summary>
    ''' Pega o comando SQL de um arquivo ou resource
    ''' </summary>
    ''' <param name="CommandFile">Nome do arquivo ou resource</param>
    ''' <returns></returns>
    Function GetCommand(CommandFile As String) As String
        CommandFile = Path.GetFileNameWithoutExtension(CommandFile) & ".sql"
        Select Case True
            Case IsNothing(ApplicationAssembly) And Not IsNothing(CommandDirectory)
                Dim filefound = CommandDirectory.SearchFiles(SearchOption.TopDirectoryOnly, CommandFile).First
                If Not filefound.Exists Then Throw New FileNotFoundException(CommandFile.Quote & "  not found in " & CommandDirectory.Name.Quote)
                Using file As StreamReader = filefound.OpenText
                    Return file.ReadToEnd
                End Using
            Case Not IsNothing(ApplicationAssembly) And IsNothing(CommandDirectory)
                Try
                    Return GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & CommandFile)
                Catch ex As Exception
                    Throw New FileNotFoundException(CommandFile.Quote & "  not found in " & ApplicationAssembly.GetName.Name.Quote & " resources. Check if Build Action is marked as ""Embedded Resource"" in File Properties.")
                End Try
            Case Else
                Throw New Exception("ApplicationAssembly or CommandDirectory is not configured!")
        End Select
    End Function

    ''' <summary>
    ''' Executa o comando de um arquivo SQL configurado
    ''' </summary>
    ''' <param name="CommandFile">Nome do arquivo SQL</param>
    ''' <param name="Parameters"> Parametros do comando SQL</param>
    ''' <returns></returns>
    Function OpenFile(CommandFile As String, ParamArray Parameters As DbParameter()) As DataBase.Reader
        Return RunSQL(GetCommand(CommandFile), Parameters)
    End Function

    ''' <summary>
    ''' Assembly da aplicação que contém os arquivos SQL
    ''' </summary>
    ''' <returns></returns>
    Public Property ApplicationAssembly As Assembly = Nothing

    ''' <summary>
    ''' Diretório que contém os arquivos SQL
    ''' </summary>
    ''' <returns></returns>
    Public Property CommandDirectory As DirectoryInfo = Nothing

    ''' <summary>
    ''' Conexão genérica (Oracle, MySQL, SQLServer etc.)
    ''' </summary>
    ''' <returns></returns>
    Public Property ConnectionString As String

    ''' <summary>
    ''' Tipo da conexão
    ''' </summary>
    ''' <returns></returns>
    Public Property ConnectionType As Type

    ''' <summary>
    ''' Arquivo onde serão salvos os logs
    ''' </summary>
    ''' <returns></returns>
    Public Property LogFile As FileInfo

    Private Sub Log(ByVal SQLQuery As String)
        Try
            Debug.WriteLine(Environment.NewLine & SQLQuery & Environment.NewLine)
            If Not IsNothing(LogFile) Then
                Dim logger As New FileLogger(LogFile) From {
                    {"Query Executed", SQLQuery.Replace(Environment.NewLine, " ")}
                }
            End If
        Catch ex As Exception
            Debug.WriteLine(Environment.NewLine & "Can't write to log file!" & Environment.NewLine)
        End Try
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="ConnectionString">String de conexão com o banco</param>
    ''' <param name="Type">            Tipo de conexão com o banco</param>
    Public Sub New(Type As Type, ByVal ConnectionString As String)
        Me.ConnectionString = ConnectionString
        ConnectionType = Type
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString, um diretório de
    ''' arquivos SQL e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="Type">            Tipo de Conexão</param>
    ''' <param name="ConnectionString">String de conexão com o banco</param>
    ''' <param name="CommandDirectory">Diretorio de arquivos SQL</param>
    Public Sub New(Type As Type, ByVal ConnectionString As String, CommandDirectory As DirectoryInfo)
        Me.ConnectionString = ConnectionString
        ConnectionType = Type
        Me.CommandDirectory = CommandDirectory
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString, Resources de
    ''' arquivos SQL e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="Type">               Tipo de Conexão</param>
    ''' <param name="ConnectionString">   String de conexão com o banco</param>
    ''' <param name="ApplicationAssembly">Assembly contendo os arquivos SQL</param>
    Public Sub New(Type As Type, ByVal ConnectionString As String, ApplicationAssembly As Assembly)
        Me.ConnectionString = ConnectionString
        ConnectionType = Type
        Me.ApplicationAssembly = ApplicationAssembly
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
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString, um diretório de
    ''' arquivos SQL e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="ConnectionString">String de conexão com o banco</param>
    ''' <param name="CommandDirectory">Diretório onde estão guardados os arquivos SQL</param>
    ''' <typeparam name="ConnectionType">Tipo de conexão com o banco</typeparam>
    Public Shared Function Create(Of Connectiontype As DbConnection)(ConnectionString As String, CommandDirectory As DirectoryInfo) As DataBase
        Return New DataBase(GetType(Connectiontype), ConnectionString, CommandDirectory)
    End Function

    ''' <summary>
    ''' Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString, Resources de
    ''' arquivos SQL e em um Tipo de Conexão
    ''' </summary>
    ''' <param name="ConnectionString">   String de conexão com o banco</param>
    ''' <param name="ApplicationAssembly">Diretório onde estão guardados os arquivos SQL</param>
    ''' <typeparam name="ConnectionType">Tipo de conexão com o banco</typeparam>
    Public Shared Function Create(Of Connectiontype As DbConnection)(ConnectionString As String, ApplicationAssembly As Assembly) As DataBase
        Return New DataBase(GetType(Connectiontype), ConnectionString, ApplicationAssembly)
    End Function

    ''' <summary>
    ''' Executa uma Query no banco. Recomenda-se o uso de procedures.
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL a ser executado</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>

    Public Function RunSQL(ByVal SQLQuery As String) As Reader
        Log(SQLQuery)
        Using con = Activator.CreateInstance(ConnectionType)
            con.ConnectionString = Me.ConnectionString
            con.Open()
            Using command As DbCommand = con.CreateCommand()
                command.CommandText = SQLQuery
                Using Reader As DbDataReader = command.ExecuteReader()
                    Return New Reader(Reader)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Executa uma Query no banco criando um comando a partir de um <see cref="IDictionary(Of String,Object)"/>
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL</param>
    ''' <param name="Values">Dicionario contendo os valores</param>
    ''' <returns></returns>
    Public Function RunSQL(ByVal SQLQuery As String, Values As IDictionary(Of String, Object)) As Reader
        Return RunSQL(Me.CreateCommandFromDictionary(SQLQuery, Values))
    End Function

    ''' <summary>
    ''' Executa uma Query no banco partir de um Arquivo.
    ''' </summary>
    ''' <param name="File">Arquivo com o comando SQL a ser executado</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>
    Public Function RunSQL(ByVal File As FileInfo) As Reader
        Using s = File.OpenText
            Return RunSQL(s.ReadToEnd)
        End Using
    End Function

    ''' <summary>
    ''' Executa uma Query no banco partir de um Arquivo.
    ''' </summary>
    ''' <param name="File">Arquivo com o comando SQL a ser executado</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>
    Public Function RunSQL(ByVal File As HttpPostedFile) As Reader
        Using s = New StreamReader(File.InputStream)
            Return RunSQL(s.ReadToEnd)
        End Using
    End Function

    ''' <summary>
    ''' Executa uma Query no banco com upload de arquivos.
    ''' </summary>
    ''' <param name="SQLQuery">     Comando SQL a ser executado</param>
    ''' <param name="FileParameter">Nome do parâmetro que guarda o arquivo</param>
    ''' <param name="File">         Arquivo</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, FileParameter As String, File As Byte()) As Reader
        Log(SQLQuery)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        command.AddFile(FileParameter, File)
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return New Reader(Reader)
    End Function

    ''' <summary>
    ''' Executa uma Query no banco com upload de arquivos.
    ''' </summary>
    ''' <param name="SQLQuery">     Comando SQL a ser executado</param>
    ''' <param name="FileParameter">Nome do parâmetro que guarda o arquivo</param>
    ''' <param name="File">         Arquivo postado</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, FileParameter As String, File As HttpPostedFile) As Reader
        Log(SQLQuery)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        command.AddFile(FileParameter, File)
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return New Reader(Reader)
    End Function

    ''' <summary>
    ''' Executa uma Query no banco com upload de arquivos.
    ''' </summary>
    ''' <param name="SQLQuery">     Comando SQL a ser executado</param>
    ''' <param name="FileParameter">Nome do parâmetro que guarda o arquivo</param>
    ''' <param name="File">         Arquivo</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, FileParameter As String, File As FileInfo) As Reader
        Log(SQLQuery)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        command.AddFile(FileParameter, File)
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return New Reader(Reader)
    End Function

    ''' <summary>
    ''' Executa uma Query no banco. Recomenda-se o uso de procedures.
    ''' </summary>
    ''' <param name="Command">Commando de banco de dados pre-pronto</param>
    ''' <returns></returns>
    Public Function RunSQL(Command As DbCommand) As Reader
        Log(Command.CommandText)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        Command.Connection = con
        con.Open()
        Dim Reader As DbDataReader = Command.ExecuteReader()
        Return New Reader(Reader)
    End Function

    ''' <summary>
    ''' Executa uma Query no banco. Recomenda-se o uso de procedures.
    ''' </summary>
    ''' <param name="SQLQuery">  Comando SQL parametrizado a ser executado</param>
    ''' <param name="Parameters">Parametros que serão adicionados ao comando</param>
    ''' <returns>Um <see cref="DataBase.Reader"/> com as informações da consulta</returns>
    Public Function RunSQL(SQLQuery As String, ParamArray Parameters() As DbParameter) As Reader
        Log(SQLQuery)
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        For Each param In Parameters
            command.Parameters.Add(param)
        Next
        Dim Reader As DbDataReader = command.ExecuteReader()
        Return New Reader(Reader)
    End Function

    ''' <summary>
    ''' Cria um comando usando como base as propriedades de uma classe
    ''' </summary>
    ''' <typeparam name="Type">Tipo da Classe</typeparam>
    ''' <param name="SQLQuery">Comando SQL parametrizado a ser executado</param>
    ''' <param name="[Object]">Objeto de onde serão extraidos os parâmetros e valores</param>
    ''' <returns></returns>
    Public Function CreateCommandFromClass(Of Type)(SQLQuery As String, [Object] As Type) As DbCommand
        Dim con = Activator.CreateInstance(ConnectionType)
        con.ConnectionString = Me.ConnectionString
        con.Open()
        Dim command As DbCommand = con.CreateCommand()
        command.CommandText = SQLQuery
        For Each prop As PropertyInfo In GetType(Type).GetProperties
            Dim param As DbParameter = command.CreateParameter
            param.Value = prop.GetValue([Object])
            param.DbType = prop.GetValue([Object]).GetDbType()
            param.ParameterName = "@" & prop.Name
            command.Parameters.Add(param)
        Next
        Return command
    End Function

    ''' <summary>
    ''' Cria um comando SQL utilizando as key e os valores de um <see cref="HttpRequest"/>
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL</param>
    ''' <param name="Request"> Request de onde serão extraidos os valores</param>
    ''' <returns></returns>
    Public Function CreateCommandFromRequest(Request As HttpRequest, SQLQuery As String, ParamArray CustomParameters As DbParameter()) As DbCommand
        Dim reg = New Regex("\@(?<param>[^=<>\s\',]+)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Matches(SQLQuery)
        Using con = Activator.CreateInstance(ConnectionType)
            con.ConnectionString = Me.ConnectionString
            con.Open()
            Dim command As DbCommand = con.CreateCommand()
            command.CommandText = SQLQuery
            Dim nomes As New List(Of String)
            Try
                nomes.AddRange(CustomParameters.[Select](Function(x) x.ParameterName.TrimAny(True, "@", " ")).Distinct())
            Catch ex As Exception
            End Try
            For Each p As Match In reg
                Dim param = p.Groups("param").Value
                param = param.TrimAny(True, "@", " ", ",", "(", ")")
                Select Case True
                    Case nomes.Contains(param)
                        For Each c In CustomParameters
                            If c.ParameterName.TrimAny("@", " ", ",", "(", ")") = param Then
                                command.Parameters.SetParameter(c)
                            End If
                        Next
                        Exit Select
                    Case Request.Form.AllKeys.Contains(param)
                        command.Parameters.SetParameter(Me.CreateParameter(param, Request.Form(param)))
                        Exit Select

                    Case Request.QueryString.AllKeys.Contains(param)
                        command.Parameters.SetParameter(Me.CreateParameter(param, Request.QueryString(param)))
                        Exit Select

                    Case Request.Files.AllKeys.Contains(param)
                        command.Parameters.SetParameter(Me.CreateParameter(param, Request.Files(param).ToBytes))
                        Exit Select

                    Case Request.Cookies.AllKeys.Contains(param)
                        command.Parameters.SetParameter(Me.CreateParameter(param, Request.Cookies(param)))
                        Exit Select
                    Case Else
                        command.Parameters.SetParameter(Me.CreateParameter(param, String.Empty))
                End Select
            Next
            Return command
        End Using
    End Function

    ''' <summary>
    ''' Cria um comando SQL utilizando as key e os valores de um <see cref="IDictionary"/>
    ''' </summary>
    ''' <param name="SQLQuery">Comando SQL</param>
    '''<param name="Parameters">Dicionario com os parametros e seus valores</param>
    ''' <returns></returns>
    Public Function CreateCommandFromDictionary(SQLQuery As String, Parameters As IDictionary(Of String, Object)) As DbCommand
        Dim reg = New Regex("\@(?<param>[^=<>\s\',]+)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Matches(SQLQuery)
        Using con = Activator.CreateInstance(ConnectionType)
            con.ConnectionString = Me.ConnectionString
            con.Open()
            Dim command As DbCommand = con.CreateCommand()
            command.CommandText = SQLQuery
            For Each p As Match In reg
                Dim param = p.Groups("param").Value
                param = param.TrimAny(True, "@", " ", ",", "(", ")")
                Try
                    command.Parameters.SetParameter(Me.CreateParameter(param, Parameters(param)))
                Catch ex As Exception
                    command.Parameters.SetParameter(Me.CreateParameter(param, String.Empty))
                End Try
            Next
            Return command
        End Using
    End Function

    ''' <summary>
    ''' Executa uma procedure para cada item dentro de uma coleção
    ''' </summary>
    ''' <param name="Procedure">   Nome da procedure</param>
    ''' <param name="ForeignKey">  Coluna que representa a chave estrangeira da tabela</param>
    ''' <param name="ForeignValue">Valor que será guardado como chave estrangeira</param>
    ''' <param name="Items">       Coleçao de valores que serão inseridos em cada iteraçao</param>
    ''' <param name="Keys">        as chaves de cada item</param>
    Public Sub RunProcedureForEach(ByVal Procedure As String, ForeignKey As String, ForeignValue As String, Items As NameValueCollection, ParamArray Keys() As String)
        UnsupportedMethod(GetType(OleDb.OleDbConnection), GetType(SqlClient.SqlConnection))
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
    ''' Executa uma série de procedures baseando-se em uma unica chave estrangeira
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
    ''' <param name="ForeignKey">     Coluna que representa a chave estrangeira da tabela</param>
    ''' <param name="ForeignValue">   Valor que será guardado como chave estrangeira</param>
    ''' <param name="Items">          Coleçao de valores que serão inseridos em cada iteraçao</param>
    ''' <param name="ProcedureConfig">
    ''' Informaçoes sobre qual procedure será executada e quais keys deverão ser usadas como parametros
    ''' </param>
    Public Sub RunBatchProcedure(Items As NameValueCollection, ForeignKey As String, ForeignValue As String, ParamArray ProcedureConfig As ProcedureConfig())
        RunBatchProcedure(New BatchProcedure(Items, ForeignKey, ForeignValue, ProcedureConfig))
    End Sub

    ''' <summary>
    ''' Insere um objeto em uma tabela a partir de suas propriedades e valores
    ''' </summary>
    ''' <param name="WhereConditions">Condições após a clausula WHERE</param>
    ''' <param name="TableName">      Nome da tabela</param>
    Default ReadOnly Property [SELECT](TableName As String, Optional WhereConditions As String = "", Optional Columns As String() = Nothing) As Reader
        Get
            Dim cmd = "SELECT " & If(Not IsNothing(Columns) AndAlso Columns.Count > 0, Columns.Join(", ").TrimAny(" ", ","), "*") & " FROM " & TableName
            If WhereConditions.IsNotBlank Then
                cmd.Append(" where " & WhereConditions.TrimAny(" ", "where", "WHERE"))
            End If
            Return RunSQL(cmd)
        End Get
    End Property

    ''' <summary>
    ''' Deleta um registro de uma tabela
    ''' </summary>
    ''' <param name="TableName">      Nome da Tabela</param>
    ''' <param name="WhereConditions">Condições após a clausula WHERE</param>
    ''' <param name="SafeMode">se False, indica se a operação pode ser realizada sem uma clausula WHERE</param>
    Public Sub DELETE(TableName As String, WhereConditions As String, Optional SafeMode As Boolean = True)
        Dim cmd = "DELETE FROM " & TableName
        If WhereConditions.IsNotBlank Or SafeMode = False Then
            If WhereConditions.IsNotBlank Then
                cmd.Append(" where " & WhereConditions.RemoveFirstAny(True, "where", " "))
            End If
            RunSQL(cmd)
        Else
            Debug.Write("WARNING: WhereConditions is Blank, set 'SafeMode' parameter as 'False' to allow DELETE commands without WHERE clausule!!")
        End If
    End Sub

    ''' <summary>
    ''' Seleciona a primeira linha de um resultset e aplica no <see cref="HtmlControl"/> equivalente ao nome da coluna
    ''' </summary>
    ''' <param name="Controls"></param>
    Public Function SelectAndFill(TableName As String, WhereConditions As String, ParamArray Controls As HtmlControl()) As DataBase.Reader
        Dim reader = Me.SELECT(TableName, WhereConditions, Controls.Select(Function(x) x.ID).ToArray)
        reader.ApplyToControls(Controls)
        reader.StartOver()
        Return reader
    End Function

End Class

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
    ''' <param name="Keys">         Chaves que serão utilizadas como parâmetro da procedure</param>
    Shadows Sub Add(ProcedureName As String, ParamArray Keys As String())
        MyBase.Add(New ProcedureConfig(ProcedureName, Keys))
    End Sub

    ''' <summary>
    ''' Cria uma nova lista de procedures
    ''' </summary>
    ''' <param name="Items">       Items usados em cada iteração</param>
    ''' <param name="ForeignKey">  Coluna de chave estrangeira</param>
    ''' <param name="ForeignValue">Valor da coluna d chave estrangeira</param>
    ''' <param name="Procs">       Lista contendo as configurações de cada procedure</param>
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
    ''' <param name="Keys">         Chaves usadas como parametros da procedure</param>
    Sub New(ProcedureName As String, ParamArray Keys As String())
        Me.ProcedureName = ProcedureName
        Me.Keys = Keys
    End Sub

End Class