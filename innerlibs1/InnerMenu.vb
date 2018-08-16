
Namespace MenuBuilder
    ''' <summary>
    ''' Estrutura para criação de menus com submenus
    ''' </summary>
    Public Class MenuBuilder
        Inherits List(Of MenuBuilderItem)
        ''' <summary>
        ''' Verifica se este menu possui itens
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HasItems
            Get
                Return Me.Count > 0
            End Get
        End Property

        ''' <summary>
        ''' Transforma a classe em um json
        ''' </summary>
        ''' <returns></returns>
        Public Function ToJSON(Optional DateFormat As String = "yyyy-MM-dd HH:mm:ss") As String
            Return Me.SerializeJSON(DateFormat)
        End Function

    End Class

    ''' <summary>
    ''' Item de um InnerMenu
    ''' </summary>
    Public Class MenuBuilderItem
        ''' <summary>
        ''' Icone correspondente a este menu
        ''' </summary>
        ''' <returns></returns>
        Public Property Icon As String
        ''' <summary>
        ''' Titulo do menu
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String
        ''' <summary>
        ''' URL do menu
        ''' </summary>
        ''' <returns></returns>
        Public Property URL As String = "#"
        ''' <summary>
        ''' Target do menu
        ''' </summary>
        ''' <returns></returns>
        Public Property Target As String = "_self"
        ''' <summary>
        ''' Subitens do menu
        ''' </summary>
        ''' <returns></returns>
        Public Property SubItems As New List(Of MenuBuilderItem)
        ''' <summary>
        ''' Verifica se este item possui subitens
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property HasItems As Boolean
            Get
                Return SubItems.Count > 0
            End Get
        End Property

        ''' <summary>
        ''' Inicializa um novo MenuBuilderItem
        ''' </summary>
        ''' <param name="Title">Titulo do menu</param>
        ''' <param name="URL">URL do menu</param>
        ''' <param name="Target">Alvo do menu, nomralmente _self</param>
        ''' <param name="Icon">icone do menu</param>
        Public Sub New(Title As String, URL As String, Optional Target As String = "_self", Optional Icon As String = "")
            Me.Title = Title
            Me.URL = URL
            Me.Target = Target
            Me.Icon = Icon
        End Sub

        ''' <summary>
        ''' Inicializa um novo MenuBuilderItem
        ''' </summary>
        ''' <param name="Title">Titulo do Menu</param>
        ''' <param name="SubItems">Subitens do menu</param>
        Public Sub New(Title As String, SubItems As List(Of MenuBuilderItem), Optional Icon As String = "")
            Me.Title = Title
            Me.SubItems = SubItems
        End Sub
    End Class
End Namespace

