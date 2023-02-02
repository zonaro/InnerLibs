using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InnerLibs.DOTLanguage
{
    public class Cluster : DotObject
    {
        #region Public Properties

        public override string ID
        {
            get;
            set;
        }

        #endregion Public Properties
    }

    public class DotAttributeCollection : Dictionary<string, object>
    {
        #region Public Methods

        public override string ToString()
        {
            string dotstring = InnerLibs.Text.Empty;
            foreach (var prop in this)
            {
                string val = prop.Value.ToString().QuoteIf(prop.Value.ToString().Contains(" ") | prop.Value.ToString().IsBlank() | prop.Value.ToString().IsURL());
                if (Misc.IsIn(val, new[] { "True", "False" }))
                {
                    val = val.ToLowerInvariant();
                }

                if (val.IsNumber())
                {
                    val = val.ChangeType<decimal>().ToString("00,00");
                }

                dotstring += prop.Key + "=" + val + " ";
            }

            return dotstring.Quote('[') + ";" + Environment.NewLine;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Representa uma ligação entre nós de um grafico em DOT Language
    /// </summary>
    public class DotEdge : DotObject
    {
        #region Public Constructors

        /// <summary>
        /// Cria uma nova ligação
        /// </summary>
        /// <param name="Oriented">Relação orientada</param>
        public DotEdge(DotNode ParentNode, DotNode ChildNode, bool Oriented = true)
        {
            this.ParentNode = ParentNode;
            this.ChildNode = ChildNode;
            this.Oriented = Oriented;
        }

        #endregion Public Constructors

        #region Public Properties

        public DotNode ChildNode { get; set; }

        public override string ID
        {
            get => ParentNode.ID.ToSlugCase(true) + (Oriented ? " -> " : " -- ") + ChildNode.ID.ToSlugCase(true);

            set => Misc.WriteDebug("Cannot change ID of a relation");
        }

        /// <summary>
        /// Indica se esta ligação é orientada ou não
        /// </summary>
        /// <returns></returns>
        public bool Oriented { get; set; } = true;

        public DotNode ParentNode { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Escreve a DOT String desta ligação
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string dotstring = InnerLibs.Text.Empty;
            if (Attributes.Any())
            {
                dotstring = ID + " " + Attributes.ToString() + Environment.NewLine;
            }

            return dotstring;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Representa um nó de um grafico em DOT Language
    /// </summary>
    public class DotNode : DotObject
    {
        #region Private Fields

        private string _id;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Cria um novo nó
        /// </summary>
        /// <param name="ID"></param>
        public DotNode(string ID)
        {
            this.ID = ID;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// ID deste nó
        /// </summary>
        /// <returns></returns>
        public override string ID
        {
            get => _id;

            set => _id = value.ToSlugCase(true);
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Escreve a DOT string deste nó e seus respectivos nós filhos
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ID + Attributes.ToString() + Environment.NewLine;

        #endregion Public Methods
    }

    public abstract class DotObject
    {
        #region Public Properties

        public DotAttributeCollection Attributes { get; private set; } = new DotAttributeCollection();
        public abstract string ID { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Wrapper para criação de gráficos em DOT Language
    /// </summary>
    public class Graph : List<DotObject>
    {
        #region Public Properties

        public List<Cluster> Clusters { get; set; } = new List<Cluster>();

        /// <summary>
        /// Tipo do Grafico (graph, digraph)
        /// </summary>
        /// <returns></returns>
        public GraphType GraphType { get; set; } = GraphType.Graph;

        /// <summary>
        /// Nome do Gráfico
        /// </summary>
        /// <returns></returns>
        public string ID { get; set; } = InnerLibs.Text.Empty;

        public bool Strict { get; set; } = false;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Escreve a DOT string correspondente a este gráfico
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var gg = $"{GraphType}".ToLowerInvariant();
            string s = this.Select(n => n.ToString() + Environment.NewLine).ToArray().SelectJoinString();
            s = s.Split(Environment.NewLine).Distinct().SelectJoinString(Environment.NewLine) + Environment.NewLine;
            if (gg.Equals("graph"))
            {
                s = s.Replace("->", "--").Replace("<-", "--");
            }

            return gg + " " + ID.ToSlugCase(true) + " " + s.Quote('{');
        }

        #endregion Public Methods
    }

    public enum GraphType
    {
        /// <summary>
        /// Gráficos não orientados
        /// </summary>
        Graph,

        /// <summary>
        /// Gráficos orientados
        /// </summary>
        Digraph
    }
}