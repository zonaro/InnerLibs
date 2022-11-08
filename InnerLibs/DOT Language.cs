using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InnerLibs.DOTLanguage
{
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

    public class Cluster : DotObject
    {
        public override string ID
        {
            get;
            set;
        }
    }

    public class DotAttributeCollection : Dictionary<string, object>
    {
        public override string ToString()
        {
            string dotstring = InnerLibs.Text.Empty;
            foreach (var prop in this)
            {
                string val = prop.Value.ToString().QuoteIf(prop.Value.ToString().Contains(" ") | prop.Value.ToString().IsBlank() | prop.Value.ToString().IsURL());
                if (Misc.IsIn(val, new[] { "True", "False" }))
                {
                    val = val.ToLower();
                }

                if (val.IsNumber())
                {
                    val = val.ChangeType<decimal>().ToString("00,00");
                }

                dotstring += prop.Key + "=" + val + " ";
            }

            return dotstring.Quote('[') + ";" + Environment.NewLine;
        }
    }

    /// <summary>
    /// Representa uma ligação entre nós de um grafico em DOT Language
    /// </summary>
    public class DotEdge : DotObject
    {
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

        public DotNode ChildNode { get; set; }

        public override string ID
        {
            get => ParentNode.ID.ToSlugCase(true) + (Oriented ? " -> " : " -- ") + ChildNode.ID.ToSlugCase(true);

            set => Debug.Write("Cannot change ID of a relation");
        }

        /// <summary>
        /// Indica se esta ligação é orientada ou não
        /// </summary>
        /// <returns></returns>
        public bool Oriented { get; set; } = true;

        public DotNode ParentNode { get; set; }

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
    }

    /// <summary>
    /// Representa um nó de um grafico em DOT Language
    /// </summary>
    public class DotNode : DotObject
    {
        private string _id;

        /// <summary>
        /// Cria um novo nó
        /// </summary>
        /// <param name="ID"></param>
        public DotNode(string ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// ID deste nó
        /// </summary>
        /// <returns></returns>
        public override string ID
        {
            get => _id;

            set => _id = value.ToSlugCase(true);
        }

        /// <summary>
        /// Escreve a DOT string deste nó e seus respectivos nós filhos
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ID + Attributes.ToString() + Environment.NewLine;
    }

    public abstract class DotObject
    {
        public DotAttributeCollection Attributes { get; private set; } = new DotAttributeCollection();
        public abstract string ID { get; set; }
    }

    /// <summary>
    /// Wrapper para criação de gráficos em DOT Language
    /// </summary>
    public class Graph : List<DotObject>
    {
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

        /// <summary>
        /// Escreve a DOT string correspondente a este gráfico
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var gg = $"{GraphType}".ToLower();
            string s = this.Select(n => n.ToString() + Environment.NewLine).ToArray().SelectJoinString();
            s = s.Split(Environment.NewLine).Distinct().SelectJoinString(Environment.NewLine) + Environment.NewLine;
            if (gg.Equals("graph"))
            {
                s = s.Replace("->", "--").Replace("<-", "--");
            }

            return gg + " " + ID.ToSlugCase(true) + " " + s.Quote('{');
        }
    }
}