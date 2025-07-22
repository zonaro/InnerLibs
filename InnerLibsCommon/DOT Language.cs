
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Extensions.DOTLanguage
{
    /// <summary>
    /// Represents compass points for node port connections according to DOT specification
    /// </summary>
    public enum CompassPoint
    {
        /// <summary>
        /// North compass point
        /// </summary>
        N,

        /// <summary>
        /// Northeast compass point
        /// </summary>
        NE,

        /// <summary>
        /// East compass point
        /// </summary>
        E,

        /// <summary>
        /// Southeast compass point
        /// </summary>
        SE,

        /// <summary>
        /// South compass point
        /// </summary>
        S,

        /// <summary>
        /// Southwest compass point
        /// </summary>
        SW,

        /// <summary>
        /// West compass point
        /// </summary>
        W,

        /// <summary>
        /// Northwest compass point
        /// </summary>
        NW,

        /// <summary>
        /// Center compass point
        /// </summary>
        C,

        /// <summary>
        /// Underscore compass point (same as center)
        /// </summary>
        _
    }

    /// <summary>
    /// Represents graph types supported by DOT language
    /// </summary>
    public enum GraphType
    {
        /// <summary>
        /// Undirected graphs (edges use --)
        /// </summary>
        Graph,

        /// <summary>
        /// Directed graphs (edges use ->)
        /// </summary>
        Digraph
    }

    /// <summary>
    /// Represents an attribute statement type in DOT language
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// Graph-level attributes
        /// </summary>
        Graph,

        /// <summary>
        /// Node-level attributes
        /// </summary>
        Node,

        /// <summary>
        /// Edge-level attributes
        /// </summary>
        Edge
    }

    /// <summary>
    /// Abstract base class for all DOT language objects
    /// </summary>
    public abstract class DotObject
    {
        #region Public Properties

        /// <summary>
        /// Gets the collection of attributes for this DOT object
        /// </summary>
        public DotAttributeCollection Attributes { get; private set; } = new DotAttributeCollection();

        /// <summary>
        /// Gets or sets the unique identifier for this DOT object
        /// </summary>
        public abstract string ID { get; set; }

        #endregion Public Properties
    }

    /// <summary>
    /// Represents a cluster subgraph in DOT language. Clusters are subgraphs with special layout properties.
    /// </summary>
    public class Cluster : Subgraph
    {
        #region Public Constructors

        /// <summary>
        /// Creates a new cluster with the specified ID. The ID will be automatically prefixed with "cluster" if not already present.
        /// </summary>
        /// <param name="id">The cluster identifier</param>
        public Cluster(string id) : base(EnsureClusterPrefix(id))
        {
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Ensures the cluster ID starts with "cluster" prefix as required by Graphviz specification
        /// </summary>
        /// <param name="id">The original ID</param>
        /// <returns>ID prefixed with "cluster" if needed</returns>
        private static string EnsureClusterPrefix(string id)
        {
            if (id.IsBlank()) return "cluster";
            return id.StartsWith("cluster") ? id : $"cluster_{id}";
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Represents a collection of attributes for DOT objects with proper value formatting according to DOT specification
    /// </summary>
    public class DotAttributeCollection : Dictionary<string, object?>
    {
        #region Public Methods

        /// <summary>
        /// Converts the attribute collection to DOT language syntax
        /// </summary>
        /// <returns>Formatted DOT attribute string</returns>
        public override string ToString()
        {
            if (!this.Any()) return Util.EmptyString;

            StringBuilder sb = new StringBuilder();
            foreach (var prop in this)
            {
                if (prop.Value == null) continue;

                string val = FormatAttributeValue(prop.Value);
                sb.Append($"{prop.Key}={val} ");
            }

            return $"[{sb.ToString().Trim()}]";
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Formats attribute values according to DOT specification
        /// </summary>
        /// <param name="value">The value to format</param>
        /// <returns>Properly formatted DOT value</returns>
        private static string FormatAttributeValue(object value)
        {
            string val = value.ToString() ?? Util.EmptyString;

            // Boolean values should be lowercase
            if (value is bool boolVal)
            {
                return boolVal.ToString().ToLowerInvariant();
            }

            // Numeric values
            if (val.IsNumber())
            {
                return val;
            }

            // Quote values that contain spaces, special characters, or are URLs
            bool needsQuoting = val.Contains(" ") ||
                               val.Contains("\n") ||
                               val.Contains("\t") ||
                               val.IsURL() ||
                               !IsValidUnquotedId(val);

            return needsQuoting ? val.Quote() : val;
        }

        /// <summary>
        /// Checks if a string is a valid unquoted ID according to DOT specification
        /// </summary>
        /// <param name="id">The ID to validate</param>
        /// <returns>True if the ID can be used unquoted</returns>
        private static bool IsValidUnquotedId(string id)
        {
            if (id.IsBlank()) return false;

            // DOT ID: alphabetic chars, underscores, digits (not starting with digit)
            return Regex.IsMatch(id, @"^[a-zA-Z\u0080-\u00ff_][a-zA-Z\u0080-\u00ff_0-9]*$") ||
                   Regex.IsMatch(id, @"^-?(\.?[0-9]+|[0-9]+(\.[0-9]*)?)$"); // numeric literal
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Represents an edge (connection) between nodes in a DOT graph
    /// </summary>
    public class DotEdge : DotObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates a new edge between two nodes
        /// </summary>
        /// <param name="parentNode">Source node</param>
        /// <param name="childNode">Target node</param>
        /// <param name="oriented">Whether the edge is directed (true) or undirected (false)</param>
        public DotEdge(DotNode parentNode, DotNode childNode, bool oriented = true)
        {
            this.ParentNode = parentNode ?? throw new ArgumentNullException(nameof(parentNode));
            this.ChildNode = childNode ?? throw new ArgumentNullException(nameof(childNode));
            this.Oriented = oriented;
        }

        /// <summary>
        /// Creates a new edge from a subgraph to a node
        /// </summary>
        /// <param name="parentSubgraph">Source subgraph</param>
        /// <param name="childNode">Target node</param>
        /// <param name="oriented">Whether the edge is directed (true) or undirected (false)</param>
        public DotEdge(Subgraph parentSubgraph, DotNode childNode, bool oriented = true)
        {
            this.ParentSubgraph = parentSubgraph ?? throw new ArgumentNullException(nameof(parentSubgraph));
            this.ChildNode = childNode ?? throw new ArgumentNullException(nameof(childNode));
            this.Oriented = oriented;
        }

        /// <summary>
        /// Creates a new edge from a node to a subgraph
        /// </summary>
        /// <param name="parentNode">Source node</param>
        /// <param name="childSubgraph">Target subgraph</param>
        /// <param name="oriented">Whether the edge is directed (true) or undirected (false)</param>
        public DotEdge(DotNode parentNode, Subgraph childSubgraph, bool oriented = true)
        {
            this.ParentNode = parentNode ?? throw new ArgumentNullException(nameof(parentNode));
            this.ChildSubgraph = childSubgraph ?? throw new ArgumentNullException(nameof(childSubgraph));
            this.Oriented = oriented;
        }

        /// <summary>
        /// Creates a new edge between two subgraphs
        /// </summary>
        /// <param name="parentSubgraph">Source subgraph</param>
        /// <param name="childSubgraph">Target subgraph</param>
        /// <param name="oriented">Whether the edge is directed (true) or undirected (false)</param>
        public DotEdge(Subgraph parentSubgraph, Subgraph childSubgraph, bool oriented = true)
        {
            this.ParentSubgraph = parentSubgraph ?? throw new ArgumentNullException(nameof(parentSubgraph));
            this.ChildSubgraph = childSubgraph ?? throw new ArgumentNullException(nameof(childSubgraph));
            this.Oriented = oriented;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the target node of this edge
        /// </summary>
        public DotNode? ChildNode { get; set; }

        /// <summary>
        /// Gets the target subgraph of this edge
        /// </summary>
        public Subgraph? ChildSubgraph { get; set; }

        /// <summary>
        /// Gets the unique identifier for this edge based on its endpoints
        /// </summary>
        public override string ID
        {
            get
            {
                string leftSide = GetNodeIdentifier(ParentNode, ParentSubgraph);
                string rightSide = GetNodeIdentifier(ChildNode, ChildSubgraph);
                string edgeOp = Oriented ? " -> " : " -- ";
                return leftSide + edgeOp + rightSide;
            }
            set => Util.WriteDebug("Cannot change ID of an edge - it's determined by its endpoints");
        }

        /// <summary>
        /// Indicates whether this edge is directed or undirected
        /// </summary>
        public bool Oriented { get; set; } = true;

        /// <summary>
        /// Gets the source node of this edge
        /// </summary>
        public DotNode? ParentNode { get; set; }

        /// <summary>
        /// Gets the source subgraph of this edge
        /// </summary>
        public Subgraph? ParentSubgraph { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts this edge to DOT language syntax
        /// </summary>
        /// <returns>DOT representation of this edge</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ID);

            if (Attributes.Any())
            {
                sb.Append(" ").Append(Attributes.ToString());
            }

            sb.AppendLine();
            return sb.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Gets the identifier for a node or subgraph
        /// </summary>
        /// <param name="node">Node to get identifier for</param>
        /// <param name="subgraph">Subgraph to get identifier for</param>
        /// <returns>Properly formatted identifier</returns>
        private static string GetNodeIdentifier(DotNode? node, Subgraph? subgraph)
        {
            if (node != null)
            {
                return FormatNodeId(node.ID);
            }

            if (subgraph != null)
            {
                return subgraph.ID.IsNotBlank() ? FormatNodeId(subgraph.ID) : "{ /* anonymous subgraph */ }";
            }

            return "/* unknown */";
        }

        /// <summary>
        /// Formats a node ID according to DOT specification
        /// </summary>
        /// <param name="id">Raw ID</param>
        /// <returns>Formatted ID</returns>
        private static string FormatNodeId(string id)
        {
            // Convert to friendly URL format and quote if necessary
            string friendlyId = id.ToFriendlyURL(true);
            return Regex.IsMatch(friendlyId, @"^[a-zA-Z\u0080-\u00ff_][a-zA-Z\u0080-\u00ff_0-9]*$")
                ? friendlyId
                : friendlyId.Quote();
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Represents a node in a DOT graph with support for ports and compass points
    /// </summary>
    public class DotNode : DotObject
    {
        #region Private Fields

        private string _id = Util.EmptyString;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new node with the specified ID
        /// </summary>
        /// <param name="id">Node identifier</param>
        public DotNode(string id)
        {
            this.ID = id;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the compass point for port connections
        /// </summary>
        public CompassPoint? CompassPoint { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for this node
        /// </summary>
        public override string ID
        {
            get => _id;
            set => _id = FormatNodeId(value ?? Util.EmptyString);
        }

        /// <summary>
        /// Gets or sets the port identifier for this node
        /// </summary>
        public string? Port { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts this node to DOT language syntax
        /// </summary>
        /// <returns>DOT representation of this node</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(GetFullNodeId());

            if (Attributes.Any())
            {
                sb.Append(" ").Append(Attributes.ToString());
            }

            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Gets the full node identifier including port and compass point if specified
        /// </summary>
        /// <returns>Complete node identifier</returns>
        public string GetFullNodeId()
        {
            var sb = new StringBuilder(ID);

            if (Port?.IsNotBlank() == true)
            {
                sb.Append(":").Append(Port);

                if (CompassPoint.HasValue)
                {
                    sb.Append(":").Append(CompassPoint.Value.ToString().ToLowerInvariant());
                }
            }
            else if (CompassPoint.HasValue)
            {
                sb.Append(":").Append(CompassPoint.Value.ToString().ToLowerInvariant());
            }

            return sb.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Formats a node ID according to DOT specification
        /// </summary>
        /// <param name="id">Raw ID</param>
        /// <returns>Formatted ID suitable for DOT output</returns>
        private static string FormatNodeId(string id)
        {
            if (id.IsBlank()) return "node_" + Guid.NewGuid().ToString("N")[..8];

            // Convert to friendly URL format
            string friendlyId = id.ToFriendlyURL(true);

            // Check if it needs quoting according to DOT ID rules
            bool needsQuoting = !Regex.IsMatch(friendlyId, @"^[a-zA-Z\u0080-\u00ff_][a-zA-Z\u0080-\u00ff_0-9]*$") &&
                               !Regex.IsMatch(friendlyId, @"^-?(\.?[0-9]+|[0-9]+(\.[0-9]*)?)$");

            return needsQuoting ? friendlyId.Quote() : friendlyId;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Represents a subgraph in DOT language, which can be named or anonymous
    /// </summary>
    public class Subgraph : DotObject
    {
        #region Public Constructors

        /// <summary>
        /// Creates a new anonymous subgraph
        /// </summary>
        public Subgraph() : this(Util.EmptyString)
        {
        }

        /// <summary>
        /// Creates a new subgraph with the specified ID
        /// </summary>
        /// <param name="id">Subgraph identifier (can be empty for anonymous subgraphs)</param>
        public Subgraph(string id)
        {
            this.ID = id ?? Util.EmptyString;
            this.Nodes = new List<DotNode>();
            this.Edges = new List<DotEdge>();
            this.Subgraphs = new List<Subgraph>();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the edges contained in this subgraph
        /// </summary>
        public List<DotEdge> Edges { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for this subgraph (empty for anonymous subgraphs)
        /// </summary>
        public override string ID { get; set; }

        /// <summary>
        /// Gets whether this subgraph is anonymous (has no ID)
        /// </summary>
        public bool IsAnonymous => ID.IsBlank();

        /// <summary>
        /// Gets or sets the nodes contained in this subgraph
        /// </summary>
        public List<DotNode> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the nested subgraphs contained in this subgraph
        /// </summary>
        public List<Subgraph> Subgraphs { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds a node to this subgraph
        /// </summary>
        /// <param name="node">Node to add</param>
        public void AddNode(DotNode node)
        {
            if (node != null && !Nodes.Contains(node))
            {
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// Adds an edge to this subgraph
        /// </summary>
        /// <param name="edge">Edge to add</param>
        public void AddEdge(DotEdge edge)
        {
            if (edge != null && !Edges.Contains(edge))
            {
                Edges.Add(edge);
            }
        }

        /// <summary>
        /// Adds a nested subgraph to this subgraph
        /// </summary>
        /// <param name="subgraph">Subgraph to add</param>
        public void AddSubgraph(Subgraph subgraph)
        {
            if (subgraph != null && !Subgraphs.Contains(subgraph))
            {
                Subgraphs.Add(subgraph);
            }
        }

        /// <summary>
        /// Converts this subgraph to DOT language syntax
        /// </summary>
        /// <returns>DOT representation of this subgraph</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            // Subgraph declaration
            if (IsAnonymous)
            {
                sb.Append("{");
            }
            else
            {
                sb.Append($"subgraph {FormatSubgraphId(ID)} {{");
            }

            sb.AppendLine();

            // Graph attributes
            if (Attributes.Any())
            {
                foreach (var attr in Attributes)
                {
                    sb.AppendLine($"    {attr.Key}={FormatAttributeValue(attr.Value)};");
                }
            }

            // Nodes
            foreach (var node in Nodes)
            {
                sb.Append("    ").Append(node.ToString().TrimEnd()).AppendLine();
            }

            // Edges
            foreach (var edge in Edges)
            {
                sb.Append("    ").Append(edge.ToString().TrimEnd()).AppendLine();
            }

            // Nested subgraphs
            foreach (var subgraph in Subgraphs)
            {
                string subgraphContent = subgraph.ToString();
                foreach (string line in subgraphContent.Split('\n'))
                {
                    if (line.Trim().IsNotBlank())
                    {
                        sb.Append("    ").AppendLine(line.TrimEnd());
                    }
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Formats a subgraph ID according to DOT specification
        /// </summary>
        /// <param name="id">Raw ID</param>
        /// <returns>Formatted ID suitable for DOT output</returns>
        private static string FormatSubgraphId(string id)
        {
            if (id.IsBlank()) return Util.EmptyString;

            string friendlyId = id.ToFriendlyURL(true);
            bool needsQuoting = !Regex.IsMatch(friendlyId, @"^[a-zA-Z\u0080-\u00ff_][a-zA-Z\u0080-\u00ff_0-9]*$");

            return needsQuoting ? friendlyId.Quote() : friendlyId;
        }

        /// <summary>
        /// Formats attribute values for DOT output
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Formatted value</returns>
        private static string FormatAttributeValue(object? value)
        {
            if (value == null) return "\"\"";

            string val = value.ToString() ?? Util.EmptyString;

            // Boolean values should be lowercase
            if (value is bool boolVal)
            {
                return boolVal.ToString().ToLowerInvariant();
            }

            // Numeric values don't need quoting
            if (val.IsNumber())
            {
                return val;
            }

            // Quote values that need it
            bool needsQuoting = val.Contains(" ") ||
                               val.Contains("\n") ||
                               val.Contains("\t") ||
                               val.Contains("\"") ||
                               val.IsURL() ||
                               !Regex.IsMatch(val, @"^[a-zA-Z\u0080-\u00ff_][a-zA-Z\u0080-\u00ff_0-9]*$");

            return needsQuoting ? val.Quote() : val;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Wrapper for creating graphs in DOT Language with complete specification support
    /// </summary>
    public class Graph : List<DotObject>
    {
        #region Public Constructors

        /// <summary>
        /// Creates a new graph with the specified type and ID
        /// </summary>
        /// <param name="graphType">Type of graph (graph or digraph)</param>
        /// <param name="id">Graph identifier (optional)</param>
        /// <param name="strict">Whether to enforce strict mode</param>
        public Graph(GraphType graphType = GraphType.Graph, string id = "", bool strict = false)
        {
            GraphType = graphType;
            ID = id ?? Util.EmptyString;
            Strict = strict;
            Clusters = new List<Cluster>();
            Subgraphs = new List<Subgraph>();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the clusters contained in this graph
        /// </summary>
        public List<Cluster> Clusters { get; set; }

        /// <summary>
        /// Gets or sets default graph attributes
        /// </summary>
        public DotAttributeCollection GraphAttributes { get; set; } = new DotAttributeCollection();

        /// <summary>
        /// Gets or sets default node attributes
        /// </summary>
        public DotAttributeCollection NodeAttributes { get; set; } = new DotAttributeCollection();

        /// <summary>
        /// Gets or sets default edge attributes
        /// </summary>
        public DotAttributeCollection EdgeAttributes { get; set; } = new DotAttributeCollection();

        /// <summary>
        /// Gets or sets the graph type (graph or digraph)
        /// </summary>
        public GraphType GraphType { get; set; } = GraphType.Graph;

        /// <summary>
        /// Gets or sets the graph identifier
        /// </summary>
        public string ID { get; set; } = Util.EmptyString;

        /// <summary>
        /// Gets or sets whether strict mode is enabled
        /// </summary>
        public bool Strict { get; set; } = false;

        /// <summary>
        /// Gets or sets the subgraphs contained in this graph
        /// </summary>
        public List<Subgraph> Subgraphs { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds a cluster to this graph
        /// </summary>
        /// <param name="cluster">Cluster to add</param>
        public void AddCluster(Cluster cluster)
        {
            if (cluster != null && !Clusters.Contains(cluster))
            {
                Clusters.Add(cluster);
            }
        }

        /// <summary>
        /// Adds a subgraph to this graph
        /// </summary>
        /// <param name="subgraph">Subgraph to add</param>
        public void AddSubgraph(Subgraph subgraph)
        {
            if (subgraph != null && !Subgraphs.Contains(subgraph))
            {
                Subgraphs.Add(subgraph);
            }
        }

        /// <summary>
        /// Converts this graph to DOT language syntax according to official specification
        /// </summary>
        /// <returns>Complete DOT representation of this graph</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            // Graph header
            if (Strict) sb.Append("strict ");
            sb.Append(GraphType.ToString().ToLowerInvariant());

            if (ID.IsNotBlank())
            {
                sb.Append($" {FormatGraphId(ID)}");
            }

            sb.AppendLine(" {");

            // Default attributes
            if (GraphAttributes.Any())
            {
                sb.AppendLine($"    graph {GraphAttributes.ToString()};");
            }

            if (NodeAttributes.Any())
            {
                sb.AppendLine($"    node {NodeAttributes.ToString()};");
            }

            if (EdgeAttributes.Any())
            {
                sb.AppendLine($"    edge {EdgeAttributes.ToString()};");
            }

            // Content
            var content = new List<string>();

            // Add all objects (nodes, edges, etc.)
            foreach (var obj in this)
            {
                string objString = obj?.ToString()?.Trim() ?? Util.EmptyString;
                if (objString.IsNotBlank())
                {
                    content.Add($"    {objString}");
                }
            }

            // Add subgraphs
            foreach (var subgraph in Subgraphs)
            {
                string subgraphContent = subgraph.ToString();
                foreach (string line in subgraphContent.Split('\n'))
                {
                    if (line.Trim().IsNotBlank())
                    {
                        content.Add($"    {line.TrimEnd()}");
                    }
                }
            }

            // Add clusters
            foreach (var cluster in Clusters)
            {
                string clusterContent = cluster.ToString();
                foreach (string line in clusterContent.Split('\n'))
                {
                    if (line.Trim().IsNotBlank())
                    {
                        content.Add($"    {line.TrimEnd()}");
                    }
                }
            }

            // Remove duplicates and fix edge operators for undirected graphs
            var uniqueContent = content.Distinct().ToList();
            if (GraphType == GraphType.Graph)
            {
                // Replace directed edge operators with undirected for graph type
                for (int i = 0; i < uniqueContent.Count; i++)
                {
                    uniqueContent[i] = uniqueContent[i].Replace(" -> ", " -- ").Replace(" <- ", " -- ");
                }
            }

            foreach (string line in uniqueContent)
            {
                sb.AppendLine(line);
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Formats a graph ID according to DOT specification
        /// </summary>
        /// <param name="id">Raw ID</param>
        /// <returns>Formatted ID suitable for DOT output</returns>
        private static string FormatGraphId(string id)
        {
            if (id.IsBlank()) return Util.EmptyString;

            string friendlyId = id.ToFriendlyURL(true);
            bool needsQuoting = !Regex.IsMatch(friendlyId, @"^[a-zA-Z\u0080-\u00ff_][a-zA-Z\u0080-\u00ff_0-9]*$");

            return needsQuoting ? friendlyId.Quote() : friendlyId;
        }

        #endregion Private Methods
    }
}