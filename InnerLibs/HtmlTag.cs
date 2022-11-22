using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace InnerLibs
{
    public class HtmlTag
    {
        private List<HtmlTag> _children = new List<HtmlTag>();
        private string _tagname = "div";
        private Dictionary<string, string> attrs = new Dictionary<string, string>();
        internal string _content;

        public HtmlTag() : base()
        {
            this.Type = HtmlNodeType.Element;
        }

        public HtmlTag(HtmlNodeType type)
        {
            this.Type = type;
        }

        public HtmlTag(string TagName, string InnerHtml = Text.Empty) : this()
        {
            this.TagName = TagName;
            this.InnerHtml = InnerHtml;
        }

        public HtmlTag(string TagName, object Attributes, string InnerHtml = Text.Empty) : this()
        {
            this.TagName = TagName;
            this.InnerHtml = InnerHtml;

            foreach (var Attr in Attributes.CreateDictionary())
            {
                this.Attributes.SetOrRemove(Attr.Key, Attr.Value);
            }
        }

        public Dictionary<string, string> Attributes
        {
            get
            {
                attrs = attrs ?? new Dictionary<string, string>();
                return attrs;
            }
        }

        public IEnumerable<HtmlTag> Children
        {
            get
            {
                _children = _children ?? new List<HtmlTag>();
                return _children;
            }
        }

        public string Class
        {
            get => Attributes.GetValueOr("class", Text.Empty);

            set => Attributes["class"] = value;
        }

        public IEnumerable<string> ClassList
        {
            get => Class.Split(" ");

            set => Class = (value ?? Array.Empty<string>().AsEnumerable()).SelectJoinString(" ");
        }

        public string Content
        {
            get
            {
                switch (this.Type)
                {
                    case HtmlNodeType.Element:
                        return InnerText;

                    case HtmlNodeType.Text:
                        return _content;

                    case HtmlNodeType.Comment:
                        return $"<!-- {_content} -->";

                    default:
                        return "";
                }
            }

            set
            {
                switch (this.Type)
                {
                    case HtmlNodeType.Element:
                        InnerHtml = value;
                        break;

                    case HtmlNodeType.Comment:
                    case HtmlNodeType.Text:
                        _content = value;
                        break;

                    default:
                        break;
                }
            }
        }

        public string ID { get => GetAttribute("id").IfBlank(GetAttribute("ID")); set => SetAttribute("id", value, true); }

        public string InnerHtml
        {
            get
            {
                switch (this.Type)
                {
                    case HtmlNodeType.Element:
                        return Children.SelectJoinString(x => x.OuterHtml) ?? "";

                    case HtmlNodeType.Text:
                    case HtmlNodeType.Comment:
                        return Content;

                    default:
                        return "";
                }
            }

            set
            {
                if (value.IsNotBlank())
                {
                    SelfClosing = false;
                }

                this.ClearChildren();

                if (value.IsNotBlank())
                    this.AddChildren(Parse(value));
            }
        }

        public string InnerText
        {
            get => this.Type == HtmlNodeType.Element ? Children.Traverse(x => x.Children).Where(x => x.Type == HtmlNodeType.Text).SelectJoinString() : _content;
            set
            {
                ClearChildren();
                if (value.IsNotBlank())
                    this.AddChildren(new HtmlTag(HtmlNodeType.Text) { Content = value });
            }
        }

        public string OuterHtml
        {
            get
            {
                switch (this.Type)
                {
                    case HtmlNodeType.Element:
                        return $"<{TagName.IfBlank("div")}{Attributes.SelectJoinString(x => x.Key == x.Value ? x.Key : $"{x.Key}={x.Value.Wrap()}", " ").PrependIf(" ", b => b.IsNotBlank())}" + (SelfClosing ? " />" : $">{InnerHtml}</{TagName.IfBlank("div")}>");

                    case HtmlNodeType.Text:
                        return Content;

                    case HtmlNodeType.Comment:
                        return $"<!-- {Content} -->";

                    default:
                        return "";
                }
            }
            set
            {
                if (value.IsNotBlank())
                {
                    var list = Parse(value);

                    if (list.Count() == 1)
                    {
                        var l = list.FirstOrDefault();
                        if (l != null)
                        {
                            this.TagName = l.TagName;
                            this.Attributes.Clear();
                            this.AddAttributes(l.Attributes);
                            ClearChildren();
                            this.AddChildren(l._children);
                        }
                    }
                    else
                    {
                        ClearChildren();
                        this.AddChildren(list);
                    }
                }
            }
        }

        public bool SelfClosing { get; set; }

        public string TagName
        {
            get => this.Type == HtmlNodeType.Element ? _tagname : "";
            set => _tagname = value.IfBlank("div");
        }

        public HtmlNodeType Type { get; private set; }

        public HtmlTag this[string ID]
        {
            get => Children.FirstOrDefault(x => x.ID == ID);
            set
            {
                if (value != null) AddChildren(value.SetID(ID));
            }
        }

        public static HtmlTag CreateAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => new HtmlTag("a", htmlAttributes, Text).SetAttribute("href", URL, true).SetAttribute("target", Target, true);

        public static HtmlTag CreateImage(Image Img, object htmlAttributes = null) => CreateImage(Img?.ToDataURL(), htmlAttributes);

        public static HtmlTag CreateImage(string URL, object htmlAttributes = null) => new HtmlTag("img", htmlAttributes, null) { SelfClosing = true }
               .SetAttribute("src", URL, true);

        public static HtmlTag CreateInput(string Name, string Value = null, string Type = "text", object htmlAttributes = null) => new HtmlTag("input", htmlAttributes, null) { SelfClosing = true }
                      .SetAttribute("name", Name, true)
                      .SetAttribute("value", Value, true)
                      .SetAttribute("type", Type.IfBlank("text"), true);

        public static HtmlTag CreateOption(string Name, string Value = null, bool Selected = false) => new HtmlTag("option", null, Name.RemoveHTML()).SetAttribute("value", Value).SetProp("selected", Selected);

        public static HtmlTag CreateTable(string[,] Table, bool Header = false)
        {
            HtmlTag tag = new HtmlTag("table");

            if (Table != null)
                for (int i = 0; i < Table.GetLength(0); i++)
                {
                    var row = new HtmlTag("tr");
                    for (int j = 0; j < Table.GetLength(1); j++)
                    {
                        Header = Header && i == 0;
                        var col = new HtmlTag(Header ? "th" : "td", Table[i, j]);
                        row.InnerHtml += col;
                    }
                    tag.InnerHtml += row;
                }
            return tag;
        }

        public static HtmlTag CreateTable<TPoco>(IEnumerable<TPoco> Rows, bool header, string IDProperty, params string[] Properties) where TPoco : class
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

        public static HtmlTag CreateTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => CreateTable(Rows, false, null, null);

        public static HtmlTag CreateTable<TPoco>(IEnumerable<TPoco> Rows, TPoco header, string IDProperty, params string[] properties) where TPoco : class
        {
            HtmlTag tag = new HtmlTag("table");

            if (properties?.Any() == true)
            {
                properties = properties.Where(x => x.IsNotBlank()).ToArray();
            }
            else
            {
                properties = typeof(TPoco).GetProperties().Select(x => x.Name).ToArray();
            }

            IEnumerable<PropertyInfo> props(TPoco t) => t.GetProperties().Where(x => x.Name.IsAny(StringComparison.InvariantCultureIgnoreCase, properties));

            if (header != null)
            {
                tag.InnerHtml += props(header).SelectJoinString(x => (x.GetValue(header)?.ToString() ?? x.Name).WrapInTag("th")).WrapInTag("tr").ToString().WrapInTag("thead");
            }

            if (Rows != null && Rows.Any())
            {
                tag.InnerHtml += Rows.SelectJoinString(row => props(row).SelectJoinString(column => column.GetValue(row)?.ToString().WrapInTag("td")).WrapInTag("tr").With(w =>
                {
                    if (IDProperty.IsNotBlank()) w.SetAttribute("ID", row.GetPropertyValue<object, TPoco>(IDProperty).ToString());
                }).ToString()).WrapInTag("tbody");
            }

            return tag;
        }

        public static implicit operator string(HtmlTag Tag) => Tag?.ToString();

        public static IEnumerable<HtmlTag> Parse(string HtmlStringOrURL)
        {
            if (HtmlStringOrURL.IsURL())
            {
                HtmlStringOrURL = Web.DownloadString(HtmlStringOrURL);
            }
            return HtmlStringOrURL.IsNotBlank() ? HtmlParser.Instance.Parse(HtmlStringOrURL) : Array.Empty<HtmlTag>();
        }

        public static IEnumerable<HtmlTag> Parse(Uri Url) => Parse(Url?.ToString());

        public static HtmlTag ParseTag(string HtmlStringOrURL) => Parse(HtmlStringOrURL).FirstOrDefault();

        public static HtmlTag ParseTag(Uri Url) => Parse(Url).FirstOrDefault();

        public HtmlTag AddAttributes(params (string, string)[] pairs)
        {
            pairs = pairs ?? Array.Empty<(string, string)>();
            return AddAttributes(pairs.ToDictionary(x => x.Item1, x => x.Item2));
        }

        public HtmlTag AddAttributes(IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            if (dictionary != null)
            {
                foreach (var att in dictionary) { SetAttribute(att.Key, att.Value); }
            }
            return this;
        }

        public HtmlTag AddChildren(string TagName, string InnerHtml = "") => AddChildren(new HtmlTag(TagName, InnerHtml));

        public HtmlTag AddChildren(params HtmlTag[] node) => AddChildren((node ?? Array.Empty<HtmlTag>()).AsEnumerable());

        public HtmlTag AddChildren(IEnumerable<HtmlTag> nodes)
        {
            SelfClosing = false;
            this._children.AddRange(nodes);
            return this;
        }

        public HtmlTag AddClass(string ClassName)
        {
            if (ClassName.IsNotBlank() && ClassName.IsNotIn(ClassList, StringComparer.InvariantCultureIgnoreCase))
            {
                Class = Class.Append(" " + ClassName);
            }

            return this;
        }

        public HtmlTag ClearChildren()
        {
            _children.Clear();
            return this;
        }

        public string GetAttribute(string key) => Attributes.GetValueOr(key, Text.Empty);

        public bool HasAttribute(string AttrName) => AttrName.IsNotBlank() ? Attributes.ContainsKey(AttrName) : HasAttributes();

        /// <summary>
        /// Determine if attributes property has items.
        /// </summary>
        /// <returns>Returns true if attributes property has items, otherwise false.</returns>
        public bool HasAttributes() => this.Attributes?.Any() ?? false;

        /// <summary>
        /// Determine if children property has items.
        /// </summary>
        /// <returns>Returns true if children property has items, otherwise false.</returns>
        public bool HasChildren() => this.Children?.Any() ?? false;

        public HtmlTag RemoveAttr(string AttrName)
        {
            Attributes.SetOrRemove(AttrName, null, true);
            return this;
        }

        public HtmlTag RemoveChildren(int Index)
        {
            _children.RemoveAt(Index);
            return this;
        }

        public HtmlTag RemoveChildren(params HtmlTag[] Children)
        {
            foreach (var item in Children ?? Array.Empty<HtmlTag>())
            {
                if (_children.Contains(item))
                {
                    _children.Remove(item);
                }
            }
            return this;
        }

        public HtmlTag RemoveClass(string ClassName)
        {
            if (ClassName.IsNotBlank() && ClassName.IsIn(ClassList, StringComparer.InvariantCultureIgnoreCase))
            {
                ClassList = ClassList.Where(x => x != null && x.Equals(ClassName, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            return this;
        }

        public HtmlTag SetAttribute(string AttrName, string Value, bool RemoveIfBlank = false)
        {
            Attributes.SetOrRemove(AttrName, Value, RemoveIfBlank);
            return this;
        }

        public HtmlTag SetID(string Value)
        {
            ID = Value;
            return this;
        }

        public HtmlTag SetInnerHtml(string Html)
        {
            this.InnerHtml = Html;
            return this;
        }

        public HtmlTag SetInnerText(string Text)
        {
            this.InnerText = Text;
            return this;
        }

        public HtmlTag SetProp(string AttrName, bool Value = true) => Value ? SetAttribute(AttrName, AttrName) : RemoveAttr(AttrName);

        public override string ToString() => OuterHtml;
    }
}