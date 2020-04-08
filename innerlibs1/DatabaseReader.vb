Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Windows.Forms
Imports System.Xml
Imports InnerLibs

Partial Public Class DataBase

    ''' <summary>
    ''' linha de um resultado do banco de dados
    ''' </summary>
    Public NotInheritable Class Row
        Inherits Dictionary(Of String, Object)

        Sub New()
            MyBase.New(StringComparer.OrdinalIgnoreCase)
        End Sub


        Public Function Simplify() As Object
            If Me.Count > 0 Then
                If Me.Count = 1 Then
                    Return Me.First.Value 'retorna o unico valor da unica coluna
                Else
                    Return Me
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna todas as colunas deta linha (é um alias para <see cref="Row.Keys"/>)
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Columns As String()
            Get
                Return Me.Keys.ToArray
            End Get
        End Property
    End Class


    ''' <summary>
    ''' Resultado de uma query no banco de dados
    ''' </summary>
    Public NotInheritable Class Result
        Inherits List(Of Row)

        ''' <summary>
        ''' Nome do <see cref="Result"/>. É utilizado na serialização do objeto. Quando não especificado, o objeto é serializado como um Array
        ''' </summary>
        ''' <returns></returns>
        Property ResultName As String = Nothing

        Public Function Simplify() As Object
            If Me.Count > 0 Then
                If Me.Count = 1 Then
                    Return Me.First.Simplify
                Else
                    If Me.All(Function(x) x.Count = 1) Then 'retorna array de valores de cada linha (tem 1 coluna apenas)
                        Return Me.Select(Function(x) x.Simplify).Where(Function(x) x IsNot Nothing)
                    Else
                        Return Me.ReturnMyself
                    End If
                End If
            End If
            Return Nothing
        End Function


        Protected Friend Function ReturnMyself()
            If Me.ResultName.IsNotBlank() Then
                Dim dic = New Dictionary(Of String, Object)
                dic(Me.ResultName) = Me
                Return dic
            End If
            Return Me
        End Function


        ''' <summary>
        ''' Retorna todas as colunas deste result
        ''' </summary>
        ''' <returns></returns>
        Public Function GetColumns() As List(Of String)
            Dim cols As New List(Of String)
            For Each c In Me.Select(Function(x) x.Columns)
                cols.AddRange(c)
            Next
            Return cols.Distinct.ToList
        End Function




    End Class

    ''' <summary>
    ''' Estrutura que imita um <see cref="DbDataReader"/> usando <see cref="List(Of List(Of Dictionary(Of String,Object)))"/>. Permite a leitura
    ''' releitura, atribuição e serialização mesmo após o fechamento da conexão.
    ''' </summary>
    Public NotInheritable Class Reader
        Inherits List(Of Result)
        Implements IDisposable

        ''' <summary>
        ''' Aplica nomes aos <see cref="Result"/> deste <see cref="Reader"/>
        ''' </summary>
        ''' <param name="Names"></param>
        Public Sub SetResultNames(ParamArray Names As String())
            Names = If(Names, {})
            If Me.HasResults Then
                For index = 0 To Names.Count.SetMaxValue(Me.CountResults) - 1
                    Me.GetResult(index).ResultName = Names(index)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Aplica nomes aos <see cref="Result"/> deste <see cref="Reader"/> utilizando um prefixo e um numero indexador
        ''' </summary>
        ''' <param name="Prefix"></param>
        Public Sub SetPrefixedResultNames(Prefix As String)
            If Me.HasResults Then
                For index = 0 To Me.CountResults - 1
                    Me.GetResult(index).ResultName = Prefix & index.ToString
                Next
            End If
        End Sub

        ''' <summary>
        ''' Retorna um Json do objeto
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return OldJsonSerializer.SerializeJSON(Me)
        End Function

        ''' <summary>
        ''' Esvazia e destroi o reader
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
        Public Function GetColumns() As IEnumerable(Of String)
            If HasResults AndAlso HasRows Then
                Dim cols As New List(Of String)
                For Each c In Me.Select(Function(x) x.GetColumns)
                    cols.AddRange(c)
                Next
                Return cols.Distinct
            Else
                Return New List(Of String)
            End If
        End Function

        ''' <summary>
        ''' Retorna apenas as colunas que o resultado atual possuir
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentRowColumns() As IEnumerable(Of String)
            If HasResults AndAlso HasRows Then
                Return Me(resultindex)(rowindex).Keys.ToArray
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
                    Debug.WriteLine(ex)
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
                Debug.WriteLine(ex)
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
                Debug.WriteLine(ex)
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
                    Debug.WriteLine(ex)
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
                    Debug.WriteLine(ex)
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
                    Debug.WriteLine(ex)
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

        Public ReadOnly Property CountResults As Integer
            Get
                Return MyBase.Count
            End Get
        End Property

        ''' <summary>
        ''' Retorna um resultado (tabela) a partir do seu Index.
        ''' </summary>
        ''' <param name="ResultIndex">Indice do resultado</param>
        ''' <returns></returns>
        Function GetResult(ResultIndex As Integer) As Result
            Return MyBase.Item(ResultIndex)
        End Function

        ''' <summary>
        ''' Retorna uma linha do resultado atual
        ''' </summary>
        ''' <param name="RowIndex"></param>
        ''' <returns></returns>
        Public Function GetRow(RowIndex As Integer) As Row
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
        Public Function GetCurrentRow() As Row
            Return GetRow(rowindex)
        End Function



        Public Function Simplify() As Object
            If Me.Count > 0 Then
                If Me.Count = 1 Then
                    Return Me.First.Simplify()
                Else
                    Return Me.Select(Function(x) x.Simplify()).Where(Function(x) x IsNot Nothing)
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Retorna um Json do Reader
        ''' </summary>
        ''' <returns></returns>
        Function ToJSON(Optional Simplify As Boolean = False) As String
            If Simplify Then
                Return OldJsonSerializer.SerializeJSON(Me.Simplify())
            Else
                Dim l = Me.Select(Function(x) x.ReturnMyself)
                If l.All(Function(x) ClassTools.IsDictionary(x)) Then
                    Dim nv = New Dictionary(Of String, Object)
                    For Each d In l.ChangeIEnumerableType(Of Dictionary(Of String, Object))
                        nv.Add(d.First.Key, d.First.Value)
                    Next
                    Return OldJsonSerializer.SerializeJSON(nv)
                Else
                    Return OldJsonSerializer.SerializeJSON(l)
                End If
            End If
            Return ""
        End Function

        ''' <summary>
        ''' Retorna o Json do resultado especifico
        ''' </summary>
        ''' <param name="ResultIndex">Índice do resultado</param>
        ''' <returns></returns>
        Function ToJSON(ResultIndex As Integer, Optional DateFormat As String = "yyyy-MM-dd HH:mm:ss") As String
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
        Public Function Search(Value As Object) As Result
            Try
                Return MyBase.Item(resultindex).Where(Function(x, y) x.Values(y).ToString().Contains(Value.ToString)).ToList
            Catch ex As Exception
                Return New Result
            End Try
        End Function

        ''' <summary>
        ''' Cria um novo Reader a partir de uma coleçao de listas de Dicionários
        ''' </summary>
        ''' <param name="Tables">Conunto de listas de dicionarios</param>
        Public Sub New(ParamArray Tables As Result())

            For Each l In Tables
                Me.Add(l)
            Next
        End Sub

        ''' <summary>
        ''' Cria um novo Reader a partir de uma coleção de dicionários
        ''' </summary>
        ''' <param name="Rows">Conunto de Dicionários</param>
        Public Sub New(ParamArray Rows As IDictionary(Of String, Object)())
            Dim r As New Result()
            r.AddRange(Rows)
            Me.Add(r)
        End Sub

        ''' <summary>
        ''' Cria um novo Reader a partir de um <see cref="DbDataReader"/>
        ''' </summary>
        ''' <param name="Reader">Reader</param>
        Public Sub New(Reader As DbDataReader, Optional ForceLowerCaseColumns As Boolean = False)
            Using Reader
                Try
                    Do
                        Dim listatabela As New Result
                        Dim columns = New List(Of String)
                        For i As Integer = 0 To Reader.FieldCount - 1
                            columns.Add(If(ForceLowerCaseColumns, Reader.GetName(i).ToLower(), Reader.GetName(i)))
                        Next
                        If Reader.HasRows Then
                            While Reader.Read
                                Dim lista As New DataBase.Row
                                For Each col In columns
                                    If Not lista.Columns.Contains(col) Then
                                        Try
                                            If Reader(col).GetType = GetType(String) Then
                                                lista.Add(col, If(IsDBNull(Reader(col)), "", JsonReader.JsonReader.Parse(Reader(col))))
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
                                    Else

                                    End If
                                Next
                                listatabela.Add(lista)
                            End While
                        End If
                        Me.Add(listatabela)
                    Loop While Reader.NextResult
                Catch ex As Exception
                End Try
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
        Public Function ToList(Of TValue As Class)() As List(Of TValue)
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
        ''' Retorna um Array de Valores da linha atual
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentRowAs(Of Type As Class)() As Type
            Dim i = Activator.CreateInstance(Of Type)
            For Each prop As PropertyInfo In GetType(Type).GetProperties
                If prop.CanWrite Then
                    prop.SetValue(i, Me(prop.Name))
                End If
            Next
            Return i
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
                    h.Add(CType(Me(KeyColumn), TKey), CType(Me(ValueColumn), TValue))
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
                        param &= ("&" & it & "=" & HttpUtility.UrlEncode("" & If(Me(it), "")))
                    Next
                End While
            End If
            Return param
        End Function

        ''' <summary>
        ''' Retorna uma url com os itens como parametros
        ''' </summary>
        ''' <param name="BaseUrl">Url Base</param>
        ''' <returns></returns>
        Public Function ToUrl(BaseUrl As Uri) As Uri
            Dim Url = New Uri(BaseUrl.AbsoluteUri)
            If Me.HasRows Then
                While Me.Read()
                    For Each it In Me.GetColumns()
                        Url = Url.AddParameter(it, If(Me(it), ""))
                    Next
                End While
            End If
            Return Url
        End Function

        ''' <summary>
        ''' Transforma o resultado de um <see cref="DataBase.Reader"/> em uma URL
        ''' </summary>
        ''' <param name="BaseUrl">Url Base</param>
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
                Dim columns = Me.GetColumns()
                While Me.Read()
                    For Each coluna In columns
                        DelimitedString = DelimitedString & If(Me(coluna), "") & ColDelimiter
                    Next
                    DelimitedString = DelimitedString & RowDelimiter
                End While
                DelimitedString = DelimitedString & TableDelimiter
            Loop While Me.NextResult()
            Return DelimitedString
        End Function

        ''' <summary>
        ''' Cria uma DataTable com os dados deste reader
        ''' </summary>
        ''' <returns></returns>
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
            stringas &= (xmlWriter.ToString)
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
                Dim col = Me.GetColumns()
                While Me.Read()
                    For Each item As String In col
                        Returned &= (If(Me(item), "").ToString().Quote & Separator)
                    Next
                    Returned &= Environment.NewLine
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
                        header &= ("|" & item)
                        base &= ("|" & item.Censor("-", {item}))
                    Next
                    header &= ("|" & Environment.NewLine)
                    base &= ("|" & Environment.NewLine)

                    Returned &= (header)
                    Returned &= (base)

                    While Me.Read()
                        For Each item As String In Me.GetColumns()
                            Returned &= ("|" & OldJsonSerializer.SerializeJSON(Me(item)))
                        Next
                        Returned &= ("|" & Environment.NewLine)
                    End While
                End If
                Returned &= (Environment.NewLine)
                Returned &= (Environment.NewLine)
            Loop While Me.NextResult()
            Return Returned

        End Function

        ''' <summary>
        ''' Converte um <see cref="DataBase.Reader"/> para uma tabela em HTML
        ''' </summary>
        ''' <param name="BeautfyColumnNames">Embeleza nomes de colunas</param>
        ''' <returns></returns>
        Public Function ToHTMLTable(Optional BeautfyColumnNames As Boolean = False) As HtmlParser.HtmlElement
            Dim tag As New HtmlParser.HtmlElement("div")
            Do
                Dim Returned As String = ""
                If Me.HasRows Then
                    Dim col = Me.GetColumns()
                    Returned &= (" <thead>")
                    Returned &= ("     <tr>")
                    For Each item As String In col
                        Returned &= ("         <th>" & If(BeautfyColumnNames, item.ToProperCase.Replace("_", " ").AdjustBlankSpaces, item) & "</th>")
                    Next
                    Returned &= ("     </tr>")
                    Returned &= (" </thead>")
                    Returned &= (" <tbody>")
                    While Me.Read()
                        Returned &= ("     <tr>")
                        For Each item As String In col
                            Returned &= (" <td>" & If(Me(item), "") & "</td>")

                        Next
                        Returned &= ("     </tr>")
                    End While
                    Returned &= (" </tbody>")

                End If
                Dim tabletag As New HtmlParser.HtmlElement("table")
                tabletag.InnerHTML = Returned
                tag.AddNode(tabletag)
            Loop While Me.NextResult()
            Return If(tag.ChildElements.Count <= 1, tag.FirstChild, tag)
        End Function

        ''' <summary>
        ''' Aplica os valores encontrados nas colunas de um <see cref="DataBase.Reader"/> em controles
        ''' com mesmo ID das colunas. Se os conroles não existirem no resultado eles serão ignorados.
        ''' </summary>
        ''' <param name="Controls">Controles que serão Manipulados</param>
        ''' <returns>Um array contendo os inputs manipulados</returns>
        Public Function ApplyToControls(ParamArray Controls As System.Windows.Forms.Control()) As System.Windows.Forms.Control()
            For Each c In Controls
                Try
                    c.CastValueForControl(Me(c.Name))
                Catch ex As Exception
                End Try
            Next
            Return Controls
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
                            CType(c, HtmlSelect).SelectValues(Me(c.ID).ToString)
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



    End Class

End Class