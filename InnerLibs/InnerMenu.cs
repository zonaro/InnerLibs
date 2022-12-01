using System.Collections.Generic;

namespace InnerLibs.MenuBuilder
{
    public class MenuItem : MenuItem<object>
    {
    }

    /// <summary>
    /// Item de um InnerMenu
    /// </summary>
    public class MenuItem<T>
    {
        #region Public Constructors

        /// <summary>
        /// Inicializa um novo MenuBuilderItem
        /// </summary>
        /// <param name="Title">Titulo do menu</param>
        /// <param name="URL">URL do menu</param>
        /// <param name="Target">Alvo do menu, nomralmente _self</param>
        /// <param name="Icon">icone do menu</param>
        public MenuItem(string Title, string URL, string Target = "_self", string Icon = InnerLibs.Text.Empty)
        {
            this.Title = Title;
            this.URL = URL;
            this.Target = Target;
            this.Icon = Icon;
        }

        /// <summary>
        /// Inicializa um novo MenuBuilderItem
        /// </summary>
        /// <param name="Title">Titulo do Menu</param>
        /// <param name="SubItems">Subitens do menu</param>
        public MenuItem(string Title, IEnumerable<MenuItem<T>> SubItems, string Icon = InnerLibs.Text.Empty)
        {
            this.Title = Title;
            this.SubItems = (MenuList<T>)SubItems;
            this.Icon = Icon;
        }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public MenuItem()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Indica se o menu está ativo (selecionado)
        /// </summary>
        /// <returns></returns>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Informações relacionadas a este item
        /// </summary>
        /// <returns></returns>
        public T Data { get; set; }

        /// <summary>
        /// Indica se o menu está habilitado
        /// </summary>
        /// <returns></returns>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Verifica se este item possui subitens
        /// </summary>
        /// <returns></returns>
        public bool HasItems
        {
            get
            {
                return SubItems.Count > 0;
            }
        }

        /// <summary>
        /// Ícone correspondente a este menu
        /// </summary>
        /// <returns></returns>
        public string Icon { get; set; }

        public MenuList<T> SubItems { get; set; }

        /// <summary>
        /// Target do menu
        /// </summary>
        /// <returns></returns>
        public string Target { get; set; } = "_self";

        /// <summary>
        /// Titulo do menu
        /// </summary>
        /// <returns></returns>
        public string Title { get; set; }

        /// <summary>
        /// URL do menu
        /// </summary>
        /// <returns></returns>
        public string URL { get; set; } = "#";

        /// <summary>
        /// Subitens do menu
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Indica se o menu está visivel
        /// </summary>
        /// <returns></returns>
        public bool Visible { get; set; } = true;

        #endregion Public Properties
    }

    /// <summary>
    /// Estrutura para criação de menus com submenus
    /// </summary>
    public class MenuList<T> : List<MenuItem<T>>
    {
        #region Public Properties

        /// <summary>
        /// Verifica se este menu possui itens
        /// </summary>
        /// <returns></returns>
        public object HasItems
        {
            get
            {
                return Count > 0;
            }
        }

        #endregion Public Properties
    }

    public class MenuList : MenuList<object>
    {
    }
}