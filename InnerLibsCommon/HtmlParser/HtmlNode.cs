using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Extensions.Web
{
    public class HtmlCommentNode : HtmlCDataNode
    {
        public HtmlCommentNode(string comment) : base("<!-- ", " -->", comment)
        {
        }
    }

    /// <summary>
    /// Represents a node that contains CDATA. This data is saved but not parsed. Examples include
    /// CDATA, comments and the content of SCRIPT and STYLE tags.
    /// </summary>
    public class HtmlCDataNode : HtmlTextNode
    {
        /// <summary>
        /// Constructs a new <see cref="HtmlCDataNode"/> instance.
        /// </summary>
        /// <param name="prefix">CDATA prefix markup.</param>
        /// <param name="suffix">CDATA suffix markup.</param>
        /// <param name="html">CDATA content.</param>
        public HtmlCDataNode(string prefix, string suffix, string html) : base(html)
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        /// <summary>
        /// Gets or sets this node's inner content.
        /// </summary>
        public override string InnerHtml
        {
            get => base.InnerHtml;
            set => base.InnerHtml = value;
        }

        /// <summary>
        /// Gets this node's markup, including the outer prefix and suffix.
        /// </summary>
        public override string OuterHtml => $"{Prefix}{InnerHtml}{Suffix}";

        /// <summary>
        /// Gets or sets this node's CDATA prefix markup.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets this node's CDATA suffix markup.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => OuterHtml;
    }

    /// <summary>
    /// Represents an HTML element (tag) node.
    /// </summary>
    public class HtmlElementNode : HtmlNode, IList<HtmlNode>
    {
        private List<HtmlNode> _children = new List<HtmlNode>();
        private CSSStyles _stl;
        private Dictionary<string, string> attrs = new Dictionary<string, string>();

        /// <summary>
        /// Constructs a new <see cref="HtmlElementNode"/> instance.
        /// </summary>
        /// <param name="tagName">Element tag name.</param>
        /// <param name="attributes">Optional element attributes.</param>
        public HtmlElementNode() : this(null) { }

        public HtmlElementNode(string TagName)
        {
            this.TagName = TagName.IfBlank("div");
            _stl = new CSSStyles(this);
        }

        public HtmlElementNode(string TagName, string InnerHtml) : this(TagName)
        {
            if (InnerHtml.IsNotBlank())
                this.InnerHtml = InnerHtml;
        }

        public HtmlElementNode(string TagName, object Attributes, string InnerHtml = Util.EmptyString) : this(TagName, InnerHtml)
        {
            foreach (var Attr in Attributes.CreateDictionary())
                this.SetAttribute(Attr.Key, $"{Attr.Value}");
        }

        /// <summary>
        /// Constructs a new <see cref="HtmlElementNode"/> instance.
        /// </summary>
        /// <param name="TagName">Element tag name.</param>
        /// <param name="attributes">Optional element attributes.</param>
        /// <param name="children">Child nodes for this node. Cannot be null.</param>
        public HtmlElementNode(string TagName, object attributes, IEnumerable<HtmlNode> children) : this(TagName, attributes) => Add(children);

        /// <summary>
        /// atributos desta tag
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get
            {
                attrs = attrs ?? new Dictionary<string, string>();
                return attrs;
            }
            set
            {
                attrs = value ?? new Dictionary<string, string>();
            }
        }

        [IgnoreDataMember]
        public string AttributeString
        {
            get => Attributes.SelectJoinString(x => $"{x.Key.Replace(" ", "-")}={x.Value.Quote()}", " ");
            set
            {
                var i = ParseNode($"<div {value}></div>") as HtmlElementNode;
                this.Attributes = i.Attributes.ToDictionary();
            }
        }

        /// <summary>
        /// Gets this element's child nodes.
        /// </summary>
        public IEnumerable<HtmlNode> ChildNodes => this.AsEnumerable();

        [IgnoreDataMember]
        public string Class
        {
            get => Attributes.GetValueOr("class") ?? Util.EmptyString;
            set => Attributes["class"] = value;
        }

        [IgnoreDataMember]
        public IEnumerable<string> ClassList
        {
            get => Class.Split(" ");

            set => Class = (value ?? Array.Empty<string>().AsEnumerable()).SelectJoinString(" ");
        }

        public int Count => _children.Count;

        public string Id { get => GetAttribute("id").BlankCoalesce(GetAttribute("Id"), GetAttribute("ID")); set => SetAttribute("id", value, true); }

        /// <summary>
        /// Gets or sets the inner markup. When setting the markup, it is parsed on the fly.
        /// </summary>
        public override string InnerHtml
        {
            get
            {
                if (!ChildNodes.Any())
                    return string.Empty;
                StringBuilder builder = new StringBuilder();
                foreach (var node in ChildNodes)
                    builder.Append(node.OuterHtml);
                return builder.ToString();
            }
            set
            {
                // Replaces all existing content
                Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var parser = new HtmlParser();
                    SetNodes(parser.ParseChildren(value));
                }
            }
        }

        public bool IsReadOnly => false;

        /// <summary>
        /// Returns true if this element is self-closing and has no children.
        /// </summary>
        public bool IsSelfClosing => !ChildNodes.Any() && !HtmlRules.GetTagFlags(TagName).HasFlag(HtmlTagFlag.NoSelfClosing);

        /// <summary>
        /// Gets or sets the outer markup.
        /// </summary>
        public override string OuterHtml
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                // Open tag
                builder.Append(HtmlRules.TagStart);
                builder.Append(TagName);
                builder.Append(AttributeString.IsNotBlank() ? " " : "");
                // Note: Attributes returned in non-deterministic order
                builder.Append(AttributeString);

                // Finish self-closing tag
                if (IsSelfClosing)
                {
                    Debug.Assert(!ChildNodes.Any());
                    builder.Append(' ');
                    builder.Append(HtmlRules.ForwardSlash);
                    builder.Append(HtmlRules.TagEnd);
                }
                else
                {
                    builder.Append(HtmlRules.TagEnd);
                    // Inner HTML
                    builder.Append(InnerHtml);
                    // Closing tag
                    builder.Append(HtmlRules.TagStart);
                    builder.Append(HtmlRules.ForwardSlash);
                    builder.Append(TagName);
                    builder.Append(HtmlRules.TagEnd);
                }
                return builder.ToString();
            }
        }

        [IgnoreDataMember]
        public CSSStyles Styles => _stl;

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public virtual string TagName { get; set; } = "div";

        /// <summary>
        /// Gets or sets this node's text.
        /// </summary>
        public override string Text
        {
            get
            {
                if (!ChildNodes.Any())
                    return string.Empty;
                StringBuilder builder = new StringBuilder();
                foreach (var node in ChildNodes)
                    builder.Append(node.Text);
                return builder.ToString();
            }
            set
            {
                // Replaces all existing content
                Clear();
                if (!string.IsNullOrEmpty(value))
                    Add(new HtmlTextNode() { Text = value });
            }
        }

        [IgnoreDataMember]
        public HtmlElementNode this[string ID]
        {
            get => ChildNodes.FirstOfType<HtmlElementNode>(x => x.Id == ID);
            set
            {
                if (value != null) Add(value.SetID(ID));
            }
        }

        [IgnoreDataMember]
        public HtmlNode this[int Index]
        {
            get => ChildNodes.FirstOrDefault(x => x.Index == Index);
            set
            {
                if (value != null) Insert(Index, value);
            }
        }

        public static HtmlElementNode CreateAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => new HtmlElementNode("a", htmlAttributes, Text).SetAttribute("src", URL, true).SetAttribute("target", Target, true);

        public static HtmlElementNode CreateAnchor(Uri URL, string Text, string Target = "_self", object htmlAttributes = null) => CreateAnchor(URL.AbsoluteUri, Text, Target, htmlAttributes);

        public static HtmlElementNode CreateBreakLine() => new HtmlElementNode("br");

        public static HtmlCommentNode CreateComment(string Comment) => new HtmlCommentNode(Comment);

        public static HtmlElementNode CreateFontAwesomeIcon(string Icon) => new HtmlElementNode("i").AddClass(Icon);

        public static HtmlElementNode CreateHorizontalRule() => new HtmlElementNode("hr");

        public static HtmlElementNode CreateImage(Image Img, object htmlAttributes = null) => CreateImage(Img?.ToDataURL(), htmlAttributes);

        public static HtmlElementNode CreateImage(string URL, object htmlAttributes = null) => new HtmlElementNode("img", htmlAttributes).SetAttribute("src", URL, true);

        public static HtmlElementNode CreateInput(string Name, string Value = null, string Type = "Text", object htmlAttributes = null) => new HtmlElementNode("input", htmlAttributes)
                                      .SetAttribute("name", Name, true)
                                      .SetAttribute("value", Value, true)
                                      .SetAttribute("type", Type.IfBlank("Text"), true);

        public static HtmlElementNode CreateList<T>(bool Ordened, IEnumerable<T> items, Expression<Func<T, string>> ItemHtml, Expression<Func<T, object>> ItemAttribute = null)
        {
            var node = new HtmlElementNode(Ordened ? "ol" : "ul");
            var arr = (items ?? Array.Empty<T>()).ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                if (item != null)
                {
                    var li = new HtmlElementNode("option");
                    if (ItemHtml != null)
                    {
                        li.InnerHtml = ItemHtml.Compile().Invoke(item);
                    }

                    if (ItemAttribute != null)
                    {
                        foreach (var att in ItemAttribute.Compile().Invoke(item)?.CreateDictionary())
                            li.SetAttribute(att.Key, $"{att.Value}");
                    }
                    node.Add(li);
                }
                else node.AddComment($"{typeof(T).Name} at {i} is null");
            }
            return node;
        }

        public static HtmlElementNode CreateOption(string Name, string Value = null, bool Selected = false) => new HtmlElementNode("option", null, Name.RemoveHTML()).SetAttribute("value", Value).SetProp("selected", Selected);

        public static HtmlElementNode CreateSelect<T>(IEnumerable<T> items, Expression<Func<T, string>> ValueSelector, Expression<Func<T, string>> NameSelector = null, Expression<Func<T, bool>> Selected = null, Expression<Func<T, object>> ItemAttribute = null)
        {
            var node = new HtmlElementNode("select");
            var arr = (items ?? Array.Empty<T>()).ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                if (item != null)
                {
                    if (NameSelector != null || ValueSelector != null)
                    {
                        var name = (NameSelector ?? ValueSelector).Compile().Invoke(item);
                        var value = (ValueSelector ?? NameSelector).Compile().Invoke(item);
                        var selected = false;
                        if (Selected != null)
                        {
                            selected = Selected.Compile().Invoke(item);
                        }

                        var opt = CreateOption(name, value, selected);

                        if (ItemAttribute != null)
                        {
                            opt.Attributes = ItemAttribute.Compile().Invoke(item)?.CreateDictionary().ToDictionary(x => x.Key, x => x.Value.ChangeType<string>());
                        }
                        node.Add(opt);
                    }
                    else
                    {
                        throw new ArgumentException("one of NameSelector or ValueSelector need to be provided");
                    }
                }
                else node.AddComment($"{typeof(T).Name} at {i} is null");
            }
            return node;
        }

        public static HtmlElementNode CreateTable(string[][] Table, bool Header = false) => CreateTable(Table?.To2D(), Header);

        public static HtmlElementNode CreateTable(string[,] Table, bool Header = false)
        {
            HtmlElementNode tag = new HtmlElementNode("table");

            if (Table != null)
                for (int i = 0; i < Table.GetLength(0); i++)
                {
                    var row = new HtmlElementNode("tr");
                    for (int j = 0; j < Table.GetLength(1); j++)
                    {
                        Header = Header && i == 0;
                        row.Add(Header ? "th" : "td", Table[i, j]);
                    }
                    tag.Add(row);
                }
            return tag;
        }

        public static HtmlElementNode CreateJavascriptObject<T>(T obj, string varName, bool useLet = false) => new HtmlElementNode("script", $"{useLet.AsIf("let", "var")} {varName.IfBlank(typeof(T).Name.ToFriendlyPathName())} = {obj?.ToJson() ?? "{}"}");

        public HtmlElementNode AddavascriptObject<T>(T obj, string varName, bool useLet = false) => this.Add(CreateJavascriptObject(obj, varName, useLet));

        public static HtmlElementNode CreateTable<TPoco>(IEnumerable<TPoco> Rows, bool header, string IDProperty, params string[] Properties) where TPoco : class
        {
            TPoco h = null;
            if (header)
            {
                try
                {
                    var nomes = typeof(TPoco).GetProperties().Select(x => x.Name);
                    h = Activator.CreateInstance<TPoco>();
                    foreach (var item in nomes)
                    {
                        h.SetPropertyValue(item, item);
                    }
                }
                catch
                {
                }
            }
            return CreateTable(Rows, h, IDProperty, Properties);
        }

        public static HtmlElementNode CreateTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => CreateTable(Rows, false);

        public static HtmlElementNode CreateTable<TPoco>(IEnumerable<TPoco> Rows, bool Header) where TPoco : class => CreateTable(Rows, Header, null, null);

        public static HtmlElementNode CreateTable<TPoco>(IEnumerable<TPoco> Rows, TPoco Header, string IDProperty, params string[] Properties) where TPoco : class
        {
            HtmlElementNode tag = new HtmlElementNode("table");

            if (Properties?.Any() == true)
            {
                Properties = Properties.Where(x => x.IsNotBlank()).ToArray();
            }
            else
            {
                Properties = typeof(TPoco).GetProperties().Select(x => x.Name).ToArray();
            }

            IEnumerable<PropertyInfo> props(TPoco t) => t.GetProperties().Where(x => x.Name.IsAny(StringComparison.InvariantCultureIgnoreCase, Properties));

            if (Header != null)
            {
                tag.InnerHtml += props(Header).SelectJoinString(x => (x.GetValue(Header)?.ToString() ?? x.Name).WrapInTag("th")).WrapInTag("tr").ToString().WrapInTag("thead");
            }

            if (Rows != null && Rows.Any())
            {
                tag.InnerHtml += Rows.SelectJoinString(row => props(row).SelectJoinString(column => column.GetValue(row)?.ToString().WrapInTag("td")).WrapInTag("tr").With(w =>
                {
                    if (IDProperty.IsNotBlank()) w.SetAttribute("Id", row.GetPropertyValue<object, TPoco>(IDProperty).ToString());
                }).ToString()).WrapInTag("tbody");
            }
            return tag;
        }

        public static HtmlTextNode CreateText(string Text) => new HtmlTextNode(Text);

        public static HtmlNode CreateWhiteSpace() => CreateText("&nbsp;");

        public static implicit operator string(HtmlElementNode Tag) => Tag?.ToString();

        /// <summary>
        /// Appends the specified node to the end of the collection. If both the last node in the
        /// collection and the node being added are of type <see cref="HtmlTextNode"></see>, the two
        /// text nodes are combined into one text node.
        /// </summary>
        /// <param name="node">Node to add.</param>
        public void Add(HtmlNode node) => Add<HtmlNode>(node);

        public HtmlElementNode Add<T>(T node) where T : HtmlNode
        {
            Insert(-1, node);
            return this;
        }

        public HtmlElementNode Add(string TagName, string InnerHtml = "", Action<HtmlElementNode> OtherActions = null) => Add(new HtmlElementNode(TagName, InnerHtml).With(OtherActions));

        public HtmlElementNode Add(params HtmlNode[] node) => Add((node ?? Array.Empty<HtmlNode>()).AsEnumerable());

        public HtmlElementNode Add(params IEnumerable<HtmlNode>[] nodes) => Add(nodes.SelectMany(x => x), false);

        public HtmlElementNode Add(IEnumerable<HtmlNode> nodes, bool copy = false)
        {
            if (nodes != null)
            {
                var nn = nodes.ToArray();
                for (int i = 0; i < nn.Length; i++)
                {
                    this.Insert(-1, nn[i], copy);
                }
            }

            return this;
        }

        public HtmlElementNode AddAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => Add(CreateAnchor(URL, Text, Target, htmlAttributes));

        public HtmlNode AddAttributes<T>(params T[] pairs)
        {
            pairs = pairs ?? Array.Empty<T>();
            foreach (var obj in pairs)
            {
                var dictionary = obj.CreateDictionary();
                foreach (var att in dictionary) SetAttribute(att.Key, $"{att.Value}");
            }
            return this;
        }

        public HtmlElementNode AddBreakLine() => Add(CreateBreakLine());

        public HtmlElementNode AddClass(params string[] ClassName)
        {
            if (ClassName != null && ClassName.Any())
            {
                ClassList = ClassList.Union(ClassName).Distinct(StringComparer.InvariantCultureIgnoreCase);
            }

            return this;
        }

        public HtmlElementNode AddComment(string Comment) => Add(CreateComment(Comment));

        public HtmlElementNode AddHorizontalRule() => Add(CreateHorizontalRule());

        public HtmlElementNode AddList<T>(bool Ordened, IEnumerable<T> items, Expression<Func<T, string>> ItemHtml, Expression<Func<T, object>> ItemAttribute = null) => Add(CreateList(Ordened, items, ItemHtml, ItemAttribute));

        /// <summary>
        /// Appends a range of nodes using the <see cref="Add"></see> method to add each one.
        /// </summary>
        /// <param name="nodes">List of nodes to add.</param>
        public void AddRange(IEnumerable<HtmlNode> nodes) => Add(nodes, false);

        public HtmlElementNode AddTable(string[][] Table, bool Header = false) => Add(CreateTable(Table, Header));

        public HtmlElementNode AddTable(string[,] Table, bool Header = false) => Add(CreateTable(Table, Header));

        ///<inheritdoc cref="AddTable{TPoco}(IEnumerable{TPoco}, TPoco, string, string[])"/>
        public HtmlElementNode AddTable<TPoco>(IEnumerable<TPoco> Rows, bool header, string IDProperty, params string[] Properties) where TPoco : class => Add(CreateTable(Rows, header, IDProperty, Properties));

        ///<inheritdoc cref="AddTable{TPoco}(IEnumerable{TPoco}, TPoco, string, string[])"/>
        public HtmlElementNode AddTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => Add(CreateTable(Rows));

        /// <summary>
        /// Generate a table from <typeparamref name="TPoco"/> classes as a children of this <see cref="HtmlNode"/>
        /// </summary>
        /// <typeparam name="TPoco"></typeparam>
        /// <param name="Rows"></param>
        /// <param name="header"></param>
        /// <param name="IDProperty"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public HtmlElementNode AddTable<TPoco>(IEnumerable<TPoco> Rows, TPoco header, string IDProperty, params string[] properties) where TPoco : class => Add(CreateTable(Rows, header, IDProperty, properties));

        public HtmlElementNode AddText(string Text)
        {
            Add(CreateText(Text));
            return this;
        }

        /// <summary>
        /// Add a new <see cref="HtmlNode"/> containing a whitespace
        /// </summary>
        /// <returns></returns>
        public HtmlElementNode AddWhiteSpace()
        {
            Add(CreateWhiteSpace());
            return this;
        }

        /// <summary>
        /// Clear all children
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            while (ChildNodes.Any())
            {
                RemoveAt(0);
            }
        }

        public HtmlElementNode Closest(Expression<Func<HtmlElementNode, bool>> predicate) => predicate != null ? this.Traverse((HtmlElementNode x) => x.ParentNode, predicate).LastOrDefault(x => x != this) : null;

        public HtmlNode Closest(string selector)
        {
            if (selector.IsNotBlank())
            {
                var current = this.ParentNode;
                do
                {
                    var r = current.QuerySelector(selector);
                    if (r != null)
                        return r;
                    current = current.ParentNode;
                }
                while (current != null);
            }
            return this;
        }

        public int CompareTo(HtmlNode other)
        {
            if (other == null || other == this) return 0;
            if (this.Index > other.Index)
                return 1;
            else if (this.Index < other.Index)
                return -1;
            else return 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is HtmlNode n)
            {
                return CompareTo(n);
            }
            else if (obj.IsNumber())
            {
                if (this.Index > obj.ToDecimal())
                    return 1;
                else if (this.Index < obj.ToDecimal())
                    return -1;
                else return 0;
            }

            return 0;
        }

        public bool Contains(HtmlNode item) => _children.Contains(item);

        public void CopyTo(HtmlNode[] array, int arrayIndex) => _children.CopyTo(array, arrayIndex);

        public IEnumerable<HtmlNode> Find(Expression<Func<HtmlNode, bool>> predicate) => Traverse().Where(predicate?.Compile() ?? (x => true)).Where(x => x != this);

        public HtmlNode FindFirst(string selector) => this.ChildNodes.QuerySelector(selector);

        public HtmlNode FindFirst(Expression<Func<HtmlNode, bool>> predicate) => Find(predicate).FirstOrDefault();

        public HtmlNode FindLast(string selector) => this.ChildNodes.QuerySelectorAll(selector).LastOrDefault();

        public HtmlNode FindLast(Expression<Func<HtmlNode, bool>> predicate) => Find(predicate).LastOrDefault();

        /// <summary>
        /// Return the first child
        /// </summary>
        /// <returns></returns>
        public HtmlNode FirstChild() => FirstChild(null);

        public HtmlNode FirstChild(Expression<Func<HtmlNode, bool>> predicate) => predicate != null ? ChildNodes.FirstOrDefault(predicate.Compile()) : ChildNodes.FirstOrDefault();

        public string GetAttribute(string key) => Attributes?.GetValueOr(key) ?? Util.EmptyString;

        public Dictionary<string, string> GetData() => Attributes.Where(x => x.Key.StartsWith("data-", StringComparison.OrdinalIgnoreCase)).ToDictionary();

        public string GetData(string Key) => GetAttribute("data-" + Key);

        public IEnumerator<HtmlNode> GetEnumerator() => _children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

        public bool HasAttribute(string AttrName) => AttrName.IsBlank() ? this.Attributes?.Any() ?? false : Attributes.ContainsKey(AttrName);

        /// <summary>
        /// Determine if attributes property has items.
        /// </summary>
        /// <returns>Returns true if attributes property has items, otherwise false.</returns>
        public bool HasAttributes() => HasAttribute(null);

        /// <summary>
        /// Determine if children property has items.
        /// </summary>
        /// <returns>Returns true if children property has items, otherwise false.</returns>
        public bool HasChildren() => this.ChildNodes?.Any() ?? false;

        public bool HasChildren(Expression<Func<HtmlNode, bool>> predicate) => this.ChildNodes?.Any(predicate?.Compile() ?? (x => false)) ?? false;

        public bool HasClass(params string[] Classes) => (Classes?.Any() ?? false ? Classes?.Any(x => ClassList.Contains(x, StringComparer.CurrentCultureIgnoreCase)) : ClassList.Any()) ?? false;

        public int IndexOf(HtmlNode item) => _children.IndexOf(item);

        /// <summary>
        /// Inject values from <typeparamref name="T"/> object into <see cref="Content"/> and <see
        /// cref="Attributes"/> of this <see cref="HtmlNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public HtmlElementNode Inject<T>(T obj)
        {
            foreach (var att in this.Attributes)
            {
                SetAttribute(att.Key, att.Value.Inject(obj));
            }

            foreach (var el in Traverse())
            {
                if (el is HtmlTextNode txt)
                {
                    txt.Text = txt.Text.Inject(obj);
                }
                else if (el is HtmlElementNode n)
                {
                    foreach (var att in n.Attributes)
                    {
                        n.SetAttribute(att.Key.Inject(obj), att.Value.Inject(obj));
                    }
                }
            }
            return this;
        }

        public HtmlElementNode Insert(int Index, string TagName, string InnerHtml, Action<HtmlElementNode> OtherActions = null) => Insert(Index, new HtmlElementNode(TagName, InnerHtml).With(OtherActions), false);

        public HtmlElementNode Insert(int Index, HtmlNode Tag, bool copy)
        {
            if (Tag != null)
            {
                Tag = copy ? Tag.CloneTag() : Tag.Detach();
                Tag.ParentNode = this;

                if (Index <= -1)
                {
                    Index = _children.Count - 1;
                }

                Index = Index.LimitIndex(_children);
                _children.Insert(Index, Tag);

            }
            return this;
        }

        public void Insert(int index, HtmlNode item) => Insert(index, item, false);

        public HtmlNode LastChild() => LastChild(null);

        public HtmlNode LastChild(Expression<Func<HtmlNode, bool>> predicate) => predicate != null ? ChildNodes.LastOrDefault(predicate.Compile()) : ChildNodes.LastOrDefault();

        public HtmlNode Move(int Increment)
        {
            if (Increment > 0) InsertInto(this.ParentNode, this.Index + Increment);
            return this;
        }

        public HtmlNode MoveDown(int Increment = 1) => Move(Increment.ForceNegative());

        public HtmlNode MoveUp(int Increment = 1) => Move(Increment.ForcePositive());

        public HtmlElementNode NextSiblingElement()
        {
            var rt = this.NextNode;

            while (rt != null && rt is HtmlElementNode == false)
                rt = rt.NextNode;

            return rt as HtmlElementNode;
        }

        public HtmlElementNode PreviousSiblingElement()
        {

            var rt = this.PrevNode;

            while (rt != null && rt is HtmlElementNode == false)
                rt = rt.PrevNode;

            return rt as HtmlElementNode;
        }

        public HtmlElementNode Remove(Expression<Func<HtmlNode, bool>> predicate) => Remove(this.ChildNodes.ToArray().Where(predicate?.Compile() ?? (x => false)) ?? Array.Empty<HtmlElementNode>());

        public HtmlElementNode Remove(params string[] IDs) => Remove(x => x.IsTypeOf<HtmlElementNode>() && IDs.Any(y => ((HtmlElementNode)x).Id.Equals(y, StringComparison.Ordinal)));
        public HtmlElementNode Remove(params int[] Indexes)
        {
            Indexes?.Each(x => RemoveAt(x));
            return this;
        }

        public HtmlElementNode Remove(IEnumerable<HtmlNode> Children)
        {
            foreach (var item in Children ?? Array.Empty<HtmlNode>())
            {
                Remove(item);
            }
            return this;
        }

        /// <summary>
        /// Removes the specified node from the collection.
        /// </summary>
        /// <param name="node"></param>
        public bool Remove(HtmlNode item)
        {
            if (item != null && item != this && _children.Contains(item))
            {
                if (_children.Remove(item))
                {
                    item.ParentNode = null;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the node at the specified position from the collection.
        /// </summary>
        /// <param name="index">The position of the item to be removed.</param>
        /// <remarks>
        /// Overrides <see cref="List{T}.RemoveAt(int)"/> in order to handle navigation property fixups.
        /// </remarks>
        public void RemoveAt(int Index) => Remove(x => x.Index == Index);

        public HtmlElementNode RemoveAttribute(string AttrName)
        {
            Attributes.SetOrRemove(AttrName, null, true);
            return this;
        }

        public HtmlElementNode RemoveClass(string ClassName)
        {
            if (ClassName.IsNotBlank() && ClassName.IsIn(ClassList, StringComparer.InvariantCultureIgnoreCase))
            {
                ClassList = ClassList.Where(x => x != null && !x.Equals(ClassName, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            return this;
        }

        public HtmlElementNode SetAttribute(string AttrName, string Value, bool RemoveIfBlank = false)
        {
            if (AttrName.IsNotBlank())
                Attributes.SetOrRemove(AttrName, Value, RemoveIfBlank);
            return this;
        }

        public HtmlElementNode SetData(string Key, string value, bool RemoveIfBlank = false) => SetAttribute(Key.PrependIf("data-", x => x.IsNotBlank()), value, RemoveIfBlank);

        public HtmlElementNode SetID(string Value)
        {
            Id = Value;
            return this;
        }

        public HtmlElementNode SetInnerHtml(string Html)
        {
            this.InnerHtml = Html;
            return this;
        }

        /// <summary>
        /// Sets the nodes in this collection. Clears any existing nodes.
        /// </summary>
        /// <param name="nodes">List of nodes to add.</param>
        public HtmlElementNode SetNodes(IEnumerable<HtmlNode> nodes)
        {
            Clear();
            AddRange(nodes);
            return this;
        }

        public HtmlElementNode SetProp(string AttrName, bool Value = true) => Value ? SetAttribute(AttrName, AttrName) : RemoveAttribute(AttrName);

        public HtmlElementNode SetStyle(string StyleName, string Value)
        {
            Styles.SetStyle(StyleName, Value);
            return this;
        }

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => OuterHtml;

        public IEnumerable<HtmlNode> Traverse() => this.ChildNodes.TraverseAll(x => x is HtmlElementNode cc && cc.Any() ? cc.Traverse() : Array.Empty<HtmlElementNode>());
    }


    public abstract class HeaderNode : HtmlElementNode
    {

    }

    /// <summary>
    /// Represents an HTML header (!DOCTYPE) node.
    /// </summary>
    public class HtmlHeaderNode : HeaderNode
    {
        /// <summary>
        /// Gets the outer markup.
        /// </summary>
        public override string OuterHtml => string.Concat(HtmlRules.TagStart,
            HtmlRules.HtmlHeaderTag,
            " ",
            AttributeString,
            " ",
            HtmlRules.TagEnd);

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => OuterHtml;
    }

    /// <summary>
    /// Abstract base class for all HTML nodes.
    /// </summary>
    public abstract class HtmlNode : ICloneable
    {
        public int DepthLevel => this.ParentNode?.DepthLevel + 1 ?? 0;

        [IgnoreDataMember]
        public int Index
        {
            get => ParentNode?.ChildNodes.GetIndexOf(this) ?? -1;
            set
            {
                if (ParentNode != null)
                {
                    value = value.LimitIndex(ParentNode.ChildNodes);
                    ParentNode.Insert(value, this);
                }
            }
        }

        /// <summary>
        /// Gets this node's markup, excluding the outer HTML tags.
        /// </summary>
        public virtual string InnerHtml
        {
            get => string.Empty;
            set { }
        }

        /// <summary>
        /// Returns <c>true</c> if this node is a top-level node and has no parent.
        /// </summary>
        public bool IsTopLevelNode => ParentNode == null;

        /// <summary>
        /// Gets this node's next sibling, or <c>null</c> if this node is the last of its siblings.
        /// </summary>
        public HtmlNode NextNode => this.ParentNode?.ChildNodes.FirstOrDefault(x => x.Index == this.Index + 1);

        public string NodeType
        {
            get
            {
                if (this is HtmlElementNode element)
                {
                    return element.TagName;
                }
                else if (this is HtmlCDataNode cdata)
                {
                    return cdata.Prefix + cdata.Suffix;
                }
                else if (this is HtmlTextNode text)
                {
                    return "[TEXT]";
                }
                else return "[NODE]";
            }
        }

        /// <summary>
        /// Gets this node's markup, including the outer HTML tags.
        /// </summary>
        public virtual string OuterHtml => string.Empty;

        /// <summary>
        /// Gets this node's parent node, or <c>null</c> if this node is a top-level node.
        /// </summary>
        public HtmlElementNode ParentNode { get; internal set; }

        /// <summary>
        /// Gets this node's previous sibling, or <c>null</c> if this node is the first of its siblings.
        /// </summary>
        public HtmlNode PrevNode => this.ParentNode?.ChildNodes.FirstOrDefault(x => x.Index == this.Index - 1);

        /// <summary>
        /// Gets this node's text. No markup is included.
        /// </summary>
        public virtual string Text
        {
            get => string.Empty;
            set { }
        }

        public static IEnumerable<HtmlNode> Parse(string HtmlString) => HtmlString.IsNotBlank() ? new HtmlParser().Parse(HtmlString).ChildNodes : Array.Empty<HtmlNode>();

        public static IEnumerable<HtmlNode> Parse(Uri URL) => Parse(URL?.DownloadString());

        public static IEnumerable<HtmlNode> Parse(FileInfo File) => File != null && File.Exists ? Parse(File.ReadAllText()) : Array.Empty<HtmlNode>();

        /// <summary>
        /// Parse a Html string into a <see cref="HtmlDocument"/> ensuring a HTML node as root of document
        /// </summary>
        /// <param name="HtmlString"></param>
        /// <returns></returns>
        public static HtmlDocument ParseDocument(string HtmlString) => new HtmlDocument(HtmlString);

        public static HtmlNode ParseNode(string HtmlString) => Parse(HtmlString).FirstOrDefault();

        public static HtmlNode ParseNode(FileInfo File) => Parse(File).FirstOrDefault();

        public static HtmlNode ParseNode(Uri Url) => Parse(Url).FirstOrDefault();

        /// <inheritdoc cref="CloneTag"/>
        public object Clone() => CloneTag();

        /// <summary>
        /// Clone this tag into a new <see cref="HtmlNode"/>
        /// </summary>
        /// <returns></returns>
        public HtmlNode CloneTag() => ParseNode(OuterHtml);

        /// <summary>
        /// Remove this tag from parent tag
        /// </summary>
        /// <returns></returns>
        public HtmlNode Detach()
        {
            this.ParentNode?.Remove(this);
            return this;
        }

        public HtmlNode InsertInto(HtmlElementNode toHtmlNode, bool copy = false) => InsertInto(toHtmlNode, -1, copy);

        public HtmlNode InsertInto(HtmlElementNode toHtmlNode, int Index, bool copy = false)
        {
            toHtmlNode?.Insert(Index, this, copy);
            return this;
        }

        /// <summary>
        /// Navigates to the next logical node. If this node has children, the first child is
        /// returned. Otherwise, if this node has a next sibling, that sibling is returned.
        /// Otherwise, this method traverses up to find the first parent with a next sibling.
        /// </summary>
        /// <returns>Returns the next logical node, or <c>null</c> if no more nodes were found.</returns>
        public HtmlNode NavigateNextNode()
        {
            HtmlNode node = this;

            if (node is HtmlElementNode elementNode && elementNode.ChildNodes.Any())
                return elementNode.ChildNodes.IfNoIndex(0);

            while (node != null)
            {
                if (node.NextNode != null)
                    return node.NextNode;
                node = node.ParentNode;
            }

            return null;
        }

        /// <summary>
        /// Navigates to the previous logical node. If this node has a previous node, the last child
        /// of the last child, etc. is returned. Otherwise, the parent is returned.
        /// </summary>
        /// <returns>
        /// Returns the previous logical node, or <c>null</c> if no more nodes were found.
        /// </returns>
        public HtmlNode NavigatePrevNode()
        {
            HtmlNode node = this;

            if (node.PrevNode != null)
            {
                node = node.PrevNode;
                while (node is HtmlElementNode elementNode && elementNode.ChildNodes.Any())

                    node = elementNode.ChildNodes.IfNoIndex(elementNode.ChildNodes.Count() - 1);

                return node;
            }

            return node.ParentNode;
        }
    }

    /// <summary>
    /// Represents a text node.
    /// </summary>
    public class HtmlTextNode : HtmlNode
    {
        protected string _content;

        /// <summary>
        /// Constructs a new <see cref="HtmlTextNode"/> instance.
        /// </summary>
        /// <param name="Text">Optional markup for this node.</param>
        public HtmlTextNode(string Text = null) => this.Text = Text ?? string.Empty;

        /// <summary>
        /// Gets or sets this node's raw text.
        /// </summary>
        public override string InnerHtml
        {
            get => Text;
            set => Text = value;
        }

        /// <summary>
        /// Gets this node's raw text.
        /// </summary>
        public override string OuterHtml => InnerHtml;

        /// <summary>
        /// Gets or sets the text for this node. Automatically HTML-encodes and decodes text values.
        /// </summary>
        public override string Text
        {
            get => _content.HtmlDecode();
            set => _content = value.HtmlEncode();
        }

        /// <summary>
        /// Converts this node to a string. (Same as <see cref="Text"/>.)
        /// </summary>
        public override string ToString() => Text;
    }

    /// <summary>
    /// Represents an XML header (?xml) node.
    /// </summary>
    public class XmlHeaderNode : HeaderNode
    {
        /// <summary>
        /// Gets the outer markup.
        /// </summary>
        public override string OuterHtml => string.Concat(HtmlRules.TagStart,
            HtmlRules.XmlHeaderTag,
            AttributeString,
            "?",
            HtmlRules.TagEnd);

        /// <summary>
        /// Converts this node to a string.
        /// </summary>
        public override string ToString() => OuterHtml;
    }
}