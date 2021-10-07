using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InnerLibs.DOTLanguage
{
    enum GraphType
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

    /// <summary>
    /// Wrapper para criaçao de gráficos em DOT Language
    /// </summary>
    public class Graph : List<DotObject>
    {
        public List<Cluster> Clusters { get; set; } = new List<Cluster>();

        /// <summary>
        /// Tipo do Grafico (graph, digraph)
        /// </summary>
        /// <returns></returns>

        public string GraphType { get; set; } = "graph";
        public bool Strict { get; set; } = false;

        /// <summary>
        /// Nome do Gráfico
        /// </summary>
        /// <returns></returns>
        public string ID { get; set; } = "";

        /// <summary>
        /// Escreve a DOT string correspondente a este gráfico
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = this.Select(n => n.ToString() + Environment.NewLine).ToArray().Join("");
            s = s.Split(Environment.NewLine).Distinct().Join(Environment.NewLine) + Environment.NewLine;
            if (GraphType.ToLower().Equals("graph"))
            {
                s = s.Replace("->", "--").Replace("<-", "--");
            }

            return GraphType + " " + ID.ToSlugCase(true) + " " + s.Quote('{');
        }
    }

    public class Cluster : DotObject
    {
        public override string ID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public abstract class DotObject
    {
        public abstract string ID { get; set; }
        public DotAttributeCollection Attributes { get; private set; } = new DotAttributeCollection();
    }

    public class DotAttributeCollection : Dictionary<string, object>
    {
        public override string ToString()
        {
            string dotstring = "";
            foreach (var prop in this)
            {
                string val = prop.Value.ToString().QuoteIf(prop.Value.ToString().Contains(" ") | prop.Value.ToString().IsBlank() | prop.Value.ToString().IsURL());
                if (val.IsIn(new[] { "True", "False" }))
                    val = val.ToLower();
                if (val.IsNumber())
                    val = val.ChangeType<decimal, string>().ToString("00,00");
                dotstring += prop.Key + "=" + val + " ";
            }

            return dotstring.Quote('[') + ";" + Environment.NewLine;
        }
    }

    /// <summary>
    /// Representa um nó de um grafico em DOT Language
    /// </summary>
    public class DotNode : DotObject
    {

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
            get
            {
                return _id;
            }

            set
            {
                _id = value.ToSlugCase(true);
            }
        }

        private string _id;

        /// <summary>
        /// Escreve a DOT string deste nó e seus respectivos nós filhos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ID + Attributes.ToString() + Environment.NewLine;
        }
    }

    /// <summary>
    /// Representa uma ligação entre nós de um grafico em DOT Language
    /// </summary>
    public class DotEdge : DotObject
    {

        /// <summary>
        /// Cria uma nova ligaçao
        /// </summary>
        /// <param name="Oriented">Relação orientada</param>
        public DotEdge(DotNode ParentNode, DotNode ChildNode, bool Oriented = true)
        {
            this.ParentNode = ParentNode;
            this.ChildNode = ChildNode;
            this.Oriented = Oriented;
        }

        /// <summary>
        /// Indica se esta ligação é orientada ou não
        /// </summary>
        /// <returns></returns>
        public bool Oriented { get; set; } = true;
        public DotNode ParentNode { get; set; }
        public DotNode ChildNode { get; set; }

        public override string ID
        {
            get
            {
                return ParentNode.ID.ToSlugCase(true) + (Oriented ? " -> " : " -- ") + ChildNode.ID.ToSlugCase(true);
            }

            set
            {
                Debug.Write("Cannot change ID of a relation");
            }
        }

        /// <summary>
        /// Escreve a DOT String desta ligaçao
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string dotstring = "";
            if (Attributes.Any())
            {
                dotstring = ID + " " + Attributes.ToString() + Environment.NewLine;
            }

            return dotstring;
        }
    }
}