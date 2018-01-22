
Imports System.ComponentModel
Imports System.Data.Linq
Imports System.Drawing
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace LINQ

    <DefaultProperty("Template")>
    <ToolboxBitmap(GetType(ListView))>
    <ToolboxData("<{0}:TriforceWriter Template="""" PageParameter=""Page"" PageSize=""12"" runat=""server"" />")>
    Public Class TriforceWriter
        Inherits WebControl



        Private tgkey As HtmlTextWriterTag
        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return tgkey
            End Get
        End Property

        Protected Overrides ReadOnly Property TagName As String
            Get
                Return [Enum].GetName(GetType(HtmlTextWriterTag), tgkey)
            End Get
        End Property

        Public Sub RenderAs(Tag As HtmlTextWriterTag)
            tgkey = Tag
        End Sub


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
        ''' <param name="Triforce"></param>
        ''' <param name="Items"></param>
        ''' <returns></returns>
        Public Function BuildFrom(Of Entity As Class)(Triforce As Triforce, Items As IEnumerable(Of Entity)) As TemplateList(Of Entity)
            Me.PageNumber = Page.Request(PageParameter).IfBlank(1).SetMinValue(1)
            Dim l = Triforce.ApplyTemplate(Items, Template, PageNumber, PageSize)
            _s = l.ToString
            Return l
        End Function

        ''' <summary>
        ''' Processa uma <see cref="System.Data.Linq.Table(Of TEntity)"/> usando um <see name="LINQ.Triforce"/> e um <paramref name="predicate"/> como filtro
        ''' </summary>
        ''' <typeparam name="Entity">Tipo do objeto</typeparam>
        ''' <param name="Triforce">Objeto Triforce usado para o processamento</param>
        ''' <param name="predicate">filtro do processamento</param>
        ''' <returns></returns>
        Public Function BuildFrom(Of Entity As Class)(Triforce As Triforce, Optional predicate As System.Linq.Expressions.Expression(Of Func(Of Entity, Boolean)) = Nothing) As TemplateList(Of Entity)
            Me.PageNumber = Page.Request(PageParameter).IfBlank(1).SetMinValue(1)
            Dim l = Triforce.ApplyTemplate(Of Entity)(Triforce.GetTemplate(Of Entity), predicate, PageNumber, PageSize)
            _s = l.ToString
            Return l
        End Function

        Protected Overrides Sub RenderContents(ByVal output As HtmlTextWriter)
            output.Write(_s)
        End Sub
    End Class
End Namespace



