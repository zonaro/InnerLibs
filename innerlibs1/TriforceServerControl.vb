
Imports System.ComponentModel
Imports System.Data.Linq
Imports System.Drawing
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace LINQ

    <DefaultProperty("Template")>
    <ToolboxBitmap(GetType(ListBox))>
    <ToolboxData("<{0}:TriforceWriter RenderAs=""Div"" Template="""" PageParameter=""Page"" PageSize=""12"" runat=""server"" />")>
    Public Class TriforceWriter
        Inherits WebControl
        Private tgkey As HtmlTextWriterTag = HtmlTextWriterTag.Div

        ''' <summary>
        ''' TagKey, como o triforce será rendereizado
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return tgkey
            End Get
        End Property

        ''' <summary>
        ''' Nome da tag, como o triforce será renderizado
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides ReadOnly Property TagName As String
            Get
                Return [Enum].GetName(GetType(HtmlTextWriterTag), tgkey)
            End Get
        End Property


        ''' <summary>
        ''' Troca diretamente o valor do <ref cref="TriforceWriter.TriforceEngine"/>. Metodo util para setar o Engine e chamar logo em segida o <see cref="BuildFrom"/>
        ''' </summary>
        ''' <param name="Engine"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property LoadEngine(Engine As Triforce) As TriforceWriter
            Get
                Me.TriforceEngine = Engine
                Return Me
            End Get
        End Property

        ''' <summary>
        ''' Motor <see cref="LINQ.Triforce"/> utilizado para este TriforceWriter
        ''' </summary>
        ''' <returns></returns>
        Public Property TriforceEngine As Triforce

        ''' <summary>
        ''' Tag que dará forma como o TriforceWriter será renderizado. Default: Div
        ''' </summary>
        ''' <returns></returns>
        <Bindable(True)>
        <Category("Appearance")>
        <DefaultValue("Div")>
        <Localizable(True)>
        Public Property RenderAs As String
            Get
                Return TagName
            End Get
            Set(ByVal value As String)
                tgkey = GetEnumValue(Of HtmlTextWriterTag)(value)
            End Set
        End Property




        ''' <summary>
        ''' Nome do Template que será utilizado para este TriforceWriter
        ''' </summary>
        ''' <returns></returns>
        <Bindable(True)>
        <Category("Appearance")>
        <DefaultValue("")>
        <Localizable(True)>
        Public Property Template As String
            Get
                Dim s As String = CType(ViewState("Template"), String)
                Return (If((s Is Nothing), String.Empty, s))
            End Get

            Set(ByVal value As String)
                ViewState("Template") = value
            End Set
        End Property

        ''' <summary>
        ''' Numero de itens por página
        ''' </summary>
        ''' <returns></returns>
        <Bindable(True)>
        <Category("Pagination")>
        <DefaultValue("12")>
        <Localizable(True)>
        Public Property PageSize As Integer
            Get
                Dim s As Integer = CType(0 & ViewState("PageSize"), Integer)
                Return s
            End Get
            Set(ByVal value As Integer)
                ViewState("PageSize") = value
            End Set
        End Property

        ''' <summary>
        ''' Numero da Página
        ''' </summary>
        ''' <returns></returns>
        <Bindable(True)>
        <Category("Pagination")>
        <DefaultValue("")>
        <Localizable(True)>
        Public Property PageNumber As Integer
            Get
                Dim s As Integer = CType(0 & ViewState("PageNumber"), Integer)
                Return s
            End Get
            Set(ByVal value As Integer)
                ViewState("PageNumber") = value
            End Set
        End Property

        ''' <summary>
        ''' Atributo que define qual parametro GET está o numero da pagina atual
        ''' </summary>
        ''' <returns></returns>
        <Bindable(True)>
        <Category("Pagination")>
        <DefaultValue("Page")>
        <Localizable(True)>
        Public Property PageParameter As String
            Get
                Dim s As String = CType(ViewState("PageParameter"), String)
                Return (If((s Is Nothing), String.Empty, s)).IfBlank("Page")
            End Get

            Set(ByVal value As String)
                ViewState("PageParameter") = value.IfBlank("Page")
            End Set
        End Property

        Private _s As String

        ''' <summary>
        ''' Processa um <see cref="IEnumerable(Of T)"/> usando um objeto <see cref="LINQ.Triforce"/>
        ''' </summary>
        ''' <typeparam name="Entity"></typeparam>
        ''' <param name="Items"></param>

        Public Sub BuildFrom(Of Entity As Class)(Items As IEnumerable(Of Entity))
            Me.PageNumber = Page.Request(PageParameter).IfBlank(1).SetMinValue(1)
            Dim l = TriforceEngine.ApplyTemplate(Items, Template, PageNumber, PageSize)
            _s = l.ToString
        End Sub

        ''' <summary>
        ''' Processa uma <see cref="System.Data.Linq.Table(Of TEntity)"/> usando um <see name="LINQ.Triforce"/> e um <paramref name="predicate"/> como filtro
        ''' </summary>
        ''' <typeparam name="Entity">Tipo do objeto</typeparam>
        ''' <param name="predicate">filtro do processamento</param>

        Public Sub BuildFrom(Of Entity As Class)(Optional predicate As System.Linq.Expressions.Expression(Of Func(Of Entity, Boolean)) = Nothing)
            Me.PageNumber = Page.Request(PageParameter).IfBlank(1).SetMinValue(1)
            Dim l = TriforceEngine.ApplyTemplate(Of Entity)(TriforceEngine.GetTemplate(Of Entity), predicate, PageNumber, PageSize)
            _s = l.ToString
        End Sub



        Protected Overrides Sub RenderContents(ByVal output As HtmlTextWriter)
            output.Write(_s)
        End Sub
    End Class
End Namespace



