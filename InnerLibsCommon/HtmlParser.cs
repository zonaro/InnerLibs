using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace InnerLibs
{
    internal class HtmlParser
    {
        #region Private Fields

        private Queue<char> q;

        #endregion Private Fields

        #region Private Methods

        private char Dequeue() => q.Dequeue();

        private string Dequeue(int length)
        {
            string text = string.Empty;
            while (length-- > 0)
            {
                if (q.Any())
                    text += q.Dequeue();
            }
            return text;
        }

        private Dictionary<string, string> GetAttributes()
        {
            var attrs = new Dictionary<string, string>();

            if (!q.Any()) return attrs;

            SkipSpace();

            if (q.Peek() == '/') return attrs;

            do
            {
                var att = GetAttribute();
                attrs[att.Key] = att.Value;
                SkipSpace();
            } while (q.Any() && q.Peek() != '>' && q.Peek(2) != "/>");

            return attrs;

        }


        internal KeyValuePair<string, string> GetAttribute()
        {
            var name = GetUpTo('=', ' ', '>');

            if (q.Peek() == ' ' || q.Peek() == '>') return new KeyValuePair<string, string>(name, null);

            Dequeue();

            if (q.Peek() == '>') return new KeyValuePair<string, string>(name, null);

            if (q.Peek(2) == "''" || q.Peek(2) == "\"\"" || q.Peek(2) == "``")
            {
                Dequeue(2);
                return new KeyValuePair<string, string>(name, string.Empty);
            }

            if (q.Any())
            {
                // attr=value is valid so check for the scenerio
                if (q.Peek() == '\'' || q.Peek() == '"' || q.Peek() == '`')
                {
                    var del = Dequeue();
                    var value = GetUpTo(del);
                    Dequeue();
                    return new KeyValuePair<string, string>(name, value);
                }
                else
                {
                    var value = GetUpTo(' ', '>', '<', '\'', '"', '=', '`');
                    return new KeyValuePair<string, string>(name, value);
                }
            }

            return new KeyValuePair<string, string>(name, null);
        }
        private string GetComment()
        {
            this.Dequeue(4);
            var text = GetUpTo("-->");
            this.Dequeue(3);
            return text;
        }

        private string GetTagName() => GetUpTo(' ', '>', '/', '\r', '\n');

        private string GetUpTo(params char[] chars) => GetUpTo(() => chars.Contains(q.Peek()));

        private string GetUpTo(string text) => GetUpTo(() => this.q.Peek(text.Length) == text);

        private string GetUpTo(Func<bool> fn)
        {
            var text = string.Empty;
            while (q.Any())
            {
                text += Dequeue();
                if (!q.Any()) return text;
                if (fn())
                    break;
            }
            return text;
        }

        private IEnumerable<HtmlTag> InternalParse(HtmlTag parent = null)
        {
            var list = new List<HtmlTag>();
            HtmlTag node = null;
            while (q.Any())
            {
                var c = q.Peek();

                if (q.Peek(2) == "</")
                {
                    Dequeue(2);
                    GetUpTo('>');
                    Dequeue();
                    return list;
                }
                else if (q.Peek(4) == "<!--")
                {
                    var comment = GetComment();
                    node = new HtmlTag(HtmlNodeType.Comment) { };
                    node.Content = comment;
                    list.Add(node);
                }
                else if (c == '<')
                {
                    node = new HtmlTag();

                    Dequeue();
                    node.TagName = GetTagName();

                    if (q.Peek() == ' ' || q.Peek() == '\r')

                        node.AddAttributes(GetAttributes());

                    SkipSpace();

                    if (!q.Any())
                    {
                        list.Add(node);
                        break;
                    }

                    // instantly closed
                    if (q.Peek() == '>' && (q.Peek(3) == "></"))
                    {
                        Dequeue(3 + node.TagName.Length);

                        Dequeue();

                        list.Add(node);
                    }
                    // self closing element
                    else if (q.Peek(2) == "/>")
                    {
                        Dequeue(2);
                        node._selfClosing = true;
                        list.Add(node);
                    }
                    // self closing tags that don't have '/>' ie: <br>
                    else if (q.Peek() == '>' && SelfClosingTags.Contains(node.TagName))
                    {
                        Dequeue();
                        node._selfClosing = true;
                        list.Add(node);
                    }
                    else if (node.TagName.Equals("script", StringComparison.OrdinalIgnoreCase))
                    {
                        Dequeue(); // >
                        node._content = GetUpTo("</script");

                        Dequeue(9);
                        list.Add(node);
                    }
                }
                else if (c == '>')
                {
                    Dequeue();
                    node.AddChildren(InternalParse(node));

                    list.Add(node);
                }
                else
                {
                    var text = string.Empty;

                    while (q.Any())
                    {
                        text += Dequeue();
                        if (q.Any() && q.Peek() == '<') break;
                    }
                    if (text.IsNotBlank()) list.Add(new HtmlTag(HtmlNodeType.Text) { Content = text });
                }
            }

            return list;
        }

        private void SkipSpace()
        {
            while (q.Any() && (q.Peek() == ' ' || q.Peek() == '\r' || q.Peek() == '\n'))
                Dequeue();
        }

        #endregion Private Methods

        #region Internal Fields

        internal static HtmlParser Instance = new HtmlParser();

        #endregion Internal Fields

        #region Public Properties

        public IEnumerable<string> SelfClosingTags { get; set; } = new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "menuitem", "meta", "param", "source", "track", "wbr" };

        #endregion Public Properties

        #region Public Methods

        public IEnumerable<HtmlTag> Parse(string source)
        {
            if (source.IsNotBlank())
            {
                this.q = new Queue<char>(source);

                return InternalParse();
            }
            return Array.Empty<HtmlTag>();
        }

        #endregion Public Methods
    }
    public class CSSStyles
    {
        #region Private Methods

        private void ParseStyle() => dic = _tag.GetAttribute("style").Split(";").ToDictionary(x => x.GetBefore(":"), x => x.GetAfter(":"));

        #endregion Private Methods

        #region Internal Fields

        internal HtmlTag _tag;

        internal Dictionary<string, string> dic = new Dictionary<string, string>();

        #endregion Internal Fields

        #region Public Constructors

        public CSSStyles(HtmlTag tag)
        {
            _tag = tag;
        }

        #endregion Public Constructors

        #region Public Properties

        public string AlignContent { get => GetStyle("align-content"); set => SetStyle("align-content", value); }

        public string AlignItems { get => GetStyle("align-items"); set => SetStyle("align-items", value); }

        public string AlignSelf { get => GetStyle("align-self"); set => SetStyle("align-self", value); }

        public string Animation { get => GetStyle("animation"); set => SetStyle("animation", value); }

        public string AnimationDelay { get => GetStyle("animation-delay"); set => SetStyle("animation-delay", value); }

        public string AnimationDirection { get => GetStyle("animation-direction"); set => SetStyle("animation-direction", value); }

        public string AnimationDuration { get => GetStyle("animation-duration"); set => SetStyle("animation-duration", value); }

        public string AnimationFillMode { get => GetStyle("animation-fill-mode"); set => SetStyle("animation-fill-mode", value); }

        public string AnimationIterationCount { get => GetStyle("animation-iteration-count"); set => SetStyle("animation-iteration-count", value); }

        public string AnimationName { get => GetStyle("animation-name"); set => SetStyle("animation-name", value); }

        public string AnimationPlayState { get => GetStyle("animation-play-state"); set => SetStyle("animation-play-state", value); }

        public string AnimationTimingFunction { get => GetStyle("animation-timing-function"); set => SetStyle("animation-timing-function", value); }

        public string BackfaceVisibility { get => GetStyle("backface-visibility"); set => SetStyle("backface-visibility", value); }

        public string Background { get => GetStyle("background"); set => SetStyle("background", value); }

        public string BackgroundAttachment { get => GetStyle("background-attachment"); set => SetStyle("background-attachment", value); }

        public string BackgroundClip { get => GetStyle("background-clip"); set => SetStyle("background-clip", value); }

        public string BackgroundColor { get => GetStyle("background-color"); set => SetStyle("background-color", value); }

        public string BackgroundImage { get => GetStyle("background-image"); set => SetStyle("background-image", value); }

        public string BackgroundOrigin { get => GetStyle("background-origin"); set => SetStyle("background-origin", value); }

        public string BackgroundPosition { get => GetStyle("background-position"); set => SetStyle("background-position", value); }

        public string BackgroundRepeat { get => GetStyle("background-repeat"); set => SetStyle("background-repeat", value); }

        public string BackgroundSize { get => GetStyle("background-size"); set => SetStyle("background-size", value); }

        public string Border { get => GetStyle("border"); set => SetStyle("border", value); }

        public string BorderBottom { get => GetStyle("border-bottom"); set => SetStyle("border-bottom", value); }

        public string BorderBottomColor { get => GetStyle("border-bottom-color"); set => SetStyle("border-bottom-color", value); }

        public string BorderBottomLeftRadius { get => GetStyle("border-bottom-left-radius"); set => SetStyle("border-bottom-left-radius", value); }

        public string BorderBottomRightRadius { get => GetStyle("border-bottom-right-radius"); set => SetStyle("border-bottom-right-radius", value); }

        public string BorderBottomStyle { get => GetStyle("border-bottom-style"); set => SetStyle("border-bottom-style", value); }

        public string BorderBottomWidth { get => GetStyle("border-bottom-width"); set => SetStyle("border-bottom-width", value); }

        public string BorderCollapse { get => GetStyle("border-collapse"); set => SetStyle("border-collapse", value); }

        public string BorderColor { get => GetStyle("border-color"); set => SetStyle("border-color", value); }

        public string BorderImage { get => GetStyle("border-image"); set => SetStyle("border-image", value); }

        public string BorderImageOutset { get => GetStyle("border-image-outset"); set => SetStyle("border-image-outset", value); }

        public string BorderImageRepeat { get => GetStyle("border-image-repeat"); set => SetStyle("border-image-repeat", value); }

        public string BorderImageSlice { get => GetStyle("border-image-slice"); set => SetStyle("border-image-slice", value); }

        public string BorderImageSource { get => GetStyle("border-image-source"); set => SetStyle("border-image-source", value); }

        public string BorderImageWidth { get => GetStyle("border-image-width"); set => SetStyle("border-image-width", value); }

        public string BorderLeft { get => GetStyle("border-left"); set => SetStyle("border-left", value); }

        public string BorderLeftColor { get => GetStyle("border-left-color"); set => SetStyle("border-left-color", value); }

        public string BorderLeftStyle { get => GetStyle("border-left-style"); set => SetStyle("border-left-style", value); }

        public string BorderLeftWidth { get => GetStyle("border-left-width"); set => SetStyle("border-left-width", value); }

        public string BorderRadius { get => GetStyle("border-radius"); set => SetStyle("border-radius", value); }

        public string BorderRight { get => GetStyle("border-right"); set => SetStyle("border-right", value); }

        public string BorderRightColor { get => GetStyle("border-right-color"); set => SetStyle("border-right-color", value); }

        public string BorderRightStyle { get => GetStyle("border-right-style"); set => SetStyle("border-right-style", value); }

        public string BorderRightWidth { get => GetStyle("border-right-width"); set => SetStyle("border-right-width", value); }

        public string BorderSpacing { get => GetStyle("border-spacing"); set => SetStyle("border-spacing", value); }

        public string BorderStyle { get => GetStyle("border-style"); set => SetStyle("border-style", value); }

        public string BorderTop { get => GetStyle("border-top"); set => SetStyle("border-top", value); }

        public string BorderTopColor { get => GetStyle("border-top-color"); set => SetStyle("border-top-color", value); }

        public string BorderTopLeftRadius { get => GetStyle("border-top-left-radius"); set => SetStyle("border-top-left-radius", value); }

        public string BorderTopRightRadius { get => GetStyle("border-top-right-radius"); set => SetStyle("border-top-right-radius", value); }

        public string BorderTopStyle { get => GetStyle("border-top-style"); set => SetStyle("border-top-style", value); }

        public string BorderTopWidth { get => GetStyle("border-top-width"); set => SetStyle("border-top-width", value); }

        public string BorderWidth { get => GetStyle("border-width"); set => SetStyle("border-width", value); }

        public string Bottom { get => GetStyle("bottom"); set => SetStyle("bottom", value); }

        public string BoxShadow { get => GetStyle("box-shadow"); set => SetStyle("box-shadow", value); }

        public string BoxSizing { get => GetStyle("box-sizing"); set => SetStyle("box-sizing", value); }

        public string CaptionSide { get => GetStyle("caption-side"); set => SetStyle("caption-side", value); }

        public string Clear { get => GetStyle("clear"); set => SetStyle("clear", value); }

        public string Clip { get => GetStyle("clip"); set => SetStyle("clip", value); }

        public string Color { get => GetStyle("color"); set => SetStyle("color", value); }

        public string ColumnCount { get => GetStyle("column-count"); set => SetStyle("column-count", value); }

        public string ColumnFill { get => GetStyle("column-fill"); set => SetStyle("column-fill", value); }

        public string ColumnGap { get => GetStyle("column-gap"); set => SetStyle("column-gap", value); }

        public string ColumnRule { get => GetStyle("column-rule"); set => SetStyle("column-rule", value); }

        public string ColumnRuleColor { get => GetStyle("column-rule-color"); set => SetStyle("column-rule-color", value); }

        public string ColumnRuleStyle { get => GetStyle("column-rule-style"); set => SetStyle("column-rule-style", value); }

        public string ColumnRuleWidth { get => GetStyle("column-rule-width"); set => SetStyle("column-rule-width", value); }

        public string Columns { get => GetStyle("columns"); set => SetStyle("columns", value); }

        public string ColumnSpan { get => GetStyle("column-span"); set => SetStyle("column-span", value); }

        public string ColumnWidth { get => GetStyle("column-width"); set => SetStyle("column-width", value); }

        public string Content { get => GetStyle("content"); set => SetStyle("content", value); }

        public string CounterIncrement { get => GetStyle("counter-increment"); set => SetStyle("counter-increment", value); }

        public string CounterReset { get => GetStyle("counter-reset"); set => SetStyle("counter-reset", value); }

        public string Cursor { get => GetStyle("cursor"); set => SetStyle("cursor", value); }

        public string Direction { get => GetStyle("direction"); set => SetStyle("direction", value); }

        public string Display { get => GetStyle("display"); set => SetStyle("display", value); }

        public string EmptyCells { get => GetStyle("empty-cells"); set => SetStyle("empty-cells", value); }

        public string Flex { get => GetStyle("flex"); set => SetStyle("flex", value); }

        public string FlexBasis { get => GetStyle("flex-basis"); set => SetStyle("flex-basis", value); }

        public string FlexDirection { get => GetStyle("flex-direction"); set => SetStyle("flex-direction", value); }

        public string FlexFlow { get => GetStyle("flex-flow"); set => SetStyle("flex-flow", value); }

        public string FlexGrow { get => GetStyle("flex-grow"); set => SetStyle("flex-grow", value); }

        public string FlexShrink { get => GetStyle("flex-shrink"); set => SetStyle("flex-shrink", value); }

        public string FlexWrap { get => GetStyle("flex-wrap"); set => SetStyle("flex-wrap", value); }

        public string Float { get => GetStyle("float"); set => SetStyle("float", value); }

        public string Font { get => GetStyle("font"); set => SetStyle("font", value); }

        public string FontFamily { get => GetStyle("font-family"); set => SetStyle("font-family", value); }

        public string FontSize { get => GetStyle("font-size"); set => SetStyle("font-size", value); }

        public string FontSizeAdjust { get => GetStyle("font-size-adjust"); set => SetStyle("font-size-adjust", value); }

        public string FontStretch { get => GetStyle("font-stretch"); set => SetStyle("font-stretch", value); }

        public string FontStyle { get => GetStyle("font-style"); set => SetStyle("font-style", value); }

        public string FontVariant { get => GetStyle("font-variant"); set => SetStyle("font-variant", value); }

        public string FontWeight { get => GetStyle("font-weight"); set => SetStyle("font-weight", value); }

        public string Height { get => GetStyle("height"); set => SetStyle("height", value); }

        public string Justify { get => GetStyle("justify"); set => SetStyle("justify", value); }

        public string JustifyContent { get => GetStyle("justify-content"); set => SetStyle("justify-content", value); }

        public string Left { get => GetStyle("left"); set => SetStyle("left", value); }

        public string LetterSpacing { get => GetStyle("letter-spacing"); set => SetStyle("letter-spacing", value); }

        public string LineHeight { get => GetStyle("line-height"); set => SetStyle("line-height", value); }

        public string ListStyle { get => GetStyle("list-style"); set => SetStyle("list-style", value); }

        public string ListStyleImage { get => GetStyle("list-style-image"); set => SetStyle("list-style-image", value); }

        public string ListStylePosition { get => GetStyle("list-style-position"); set => SetStyle("list-style-position", value); }

        public string ListStyleType { get => GetStyle("list-style-type"); set => SetStyle("list-style-type", value); }

        public string Margin { get => GetStyle("margin"); set => SetStyle("margin", value); }

        public string MarginBottom { get => GetStyle("margin-bottom"); set => SetStyle("margin-bottom", value); }

        public string MarginLeft { get => GetStyle("margin-left"); set => SetStyle("margin-left", value); }

        public string MarginRight { get => GetStyle("margin-right"); set => SetStyle("margin-right", value); }

        public string MarginTop { get => GetStyle("margin-top"); set => SetStyle("margin-top", value); }

        public string MaxHeight { get => GetStyle("max-height"); set => SetStyle("max-height", value); }

        public string MaxWidth { get => GetStyle("max-width"); set => SetStyle("max-width", value); }

        public string MinHeight { get => GetStyle("min-height"); set => SetStyle("min-height", value); }

        public string MinWidth { get => GetStyle("min-width"); set => SetStyle("min-width", value); }

        public string Opacity { get => GetStyle("opacity"); set => SetStyle("opacity", value); }

        public string Order { get => GetStyle("order"); set => SetStyle("order", value); }

        public string Outline { get => GetStyle("outline"); set => SetStyle("outline", value); }

        public string OutlineColor { get => GetStyle("outline-color"); set => SetStyle("outline-color", value); }

        public string OutlineOffset { get => GetStyle("outline-offset"); set => SetStyle("outline-offset", value); }

        public string OutlineStyle { get => GetStyle("outline-style"); set => SetStyle("outline-style", value); }

        public string OutlineWidth { get => GetStyle("outline-width"); set => SetStyle("outline-width", value); }

        public string Overflow { get => GetStyle("overflow"); set => SetStyle("overflow", value); }

        public string OverflowX { get => GetStyle("overflow-x"); set => SetStyle("overflow-x", value); }

        public string OverflowY { get => GetStyle("overflow-y"); set => SetStyle("overflow-y", value); }

        public string Padding { get => GetStyle("padding"); set => SetStyle("padding", value); }

        public string PaddingBottom { get => GetStyle("padding-bottom"); set => SetStyle("padding-bottom", value); }

        public string PaddingLeft { get => GetStyle("padding-left"); set => SetStyle("padding-left", value); }

        public string PaddingRight { get => GetStyle("padding-right"); set => SetStyle("padding-right", value); }

        public string PaddingTop { get => GetStyle("padding-top"); set => SetStyle("padding-top", value); }

        public string PageBreakAfter { get => GetStyle("page-break-after"); set => SetStyle("page-break-after", value); }

        public string PageBreakBefore { get => GetStyle("page-break-before"); set => SetStyle("page-break-before", value); }

        public string PageBreakInside { get => GetStyle("page-break-inside"); set => SetStyle("page-break-inside", value); }

        public string Perspective { get => GetStyle("perspective"); set => SetStyle("perspective", value); }

        public string PerspectiveOrigin { get => GetStyle("perspective-origin"); set => SetStyle("perspective-origin", value); }

        public string Position { get => GetStyle("position"); set => SetStyle("position", value); }

        public string Quotes { get => GetStyle("quotes"); set => SetStyle("quotes", value); }

        public string Resize { get => GetStyle("resize"); set => SetStyle("resize", value); }

        public string Right { get => GetStyle("right"); set => SetStyle("right", value); }

        public string TableLayout { get => GetStyle("table-layout"); set => SetStyle("table-layout", value); }

        public string TabSize { get => GetStyle("tab-size"); set => SetStyle("tab-size", value); }

        public string TextAlign { get => GetStyle("text-align"); set => SetStyle("text-align", value); }

        public string TextAlignLast { get => GetStyle("text-align-last"); set => SetStyle("text-align-last", value); }

        public string TextDecoration { get => GetStyle("text-decoration"); set => SetStyle("text-decoration", value); }

        public string TextDecorationColor { get => GetStyle("text-decoration-color"); set => SetStyle("text-decoration-color", value); }

        public string TextDecorationLine { get => GetStyle("text-decoration-line"); set => SetStyle("text-decoration-line", value); }

        public string TextDecorationStyle { get => GetStyle("text-decoration-style"); set => SetStyle("text-decoration-style", value); }

        public string TextIndent { get => GetStyle("text-indent"); set => SetStyle("text-indent", value); }

        public string TextJustify { get => GetStyle("text-justify"); set => SetStyle("text-justify", value); }

        public string TextOverflow { get => GetStyle("text-overflow"); set => SetStyle("text-overflow", value); }

        public string TextShadow { get => GetStyle("text-shadow"); set => SetStyle("text-shadow", value); }

        public string TextTransform { get => GetStyle("text-transform"); set => SetStyle("text-transform", value); }

        public string Top { get => GetStyle("top"); set => SetStyle("top", value); }

        public string Transform { get => GetStyle("transform"); set => SetStyle("transform", value); }

        public string TransformOrigin { get => GetStyle("transform-origin"); set => SetStyle("transform-origin", value); }

        public string TransformStyle { get => GetStyle("transform-style"); set => SetStyle("transform-style", value); }

        public string Transition { get => GetStyle("transition"); set => SetStyle("transition", value); }

        public string TransitionDelay { get => GetStyle("transition-delay"); set => SetStyle("transition-delay", value); }

        public string TransitionDuration { get => GetStyle("transition-duration"); set => SetStyle("transition-duration", value); }

        public string TransitionProperty { get => GetStyle("transition-property"); set => SetStyle("transition-property", value); }

        public string TransitionTimingFunction { get => GetStyle("transition-timing-function"); set => SetStyle("transition-timing-function", value); }

        public string VerticalAlign { get => GetStyle("vertical-align"); set => SetStyle("vertical-align", value); }

        public string Visibility { get => GetStyle("visibility"); set => SetStyle("visibility", value); }

        public string WhiteSpace { get => GetStyle("white-space"); set => SetStyle("white-space", value); }

        public string Width { get => GetStyle("width"); set => SetStyle("width", value); }

        public string WordBreak { get => GetStyle("word-break"); set => SetStyle("word-break", value); }

        public string WordSpacing { get => GetStyle("word-spacing"); set => SetStyle("word-spacing", value); }

        public string WordWrap { get => GetStyle("word-wrap"); set => SetStyle("word-wrap", value); }

        public string ZIndex { get => GetStyle("z-index"); set => SetStyle("z-index", value); }

        #endregion Public Properties

        #region Public Methods

        public string GetStyle(string name)
        {
            ParseStyle();
            return dic.GetValueOr(name);
        }

        public CSSStyles RemoveStyle(string name)
        {
            ParseStyle();
            dic.SetOrRemove(name, null, true);
            _tag.SetAttribute("style", ToString());
            return this;
        }

        public CSSStyles SetStyle(string name, string value)
        {
            ParseStyle();
            dic.SetOrRemove(name, value);
            _tag.SetAttribute("style", ToString());
            return this;
        }

        public override string ToString() => dic.SelectJoinString(x => $"{x.Key.ToLowerInvariant()}:{x.Value}", ";");

        #endregion Public Methods
    }

    /// <summary>
    /// A Helper for generate HTML or XML Tags/Documents
    /// </summary>
    public class HtmlTag : ICloneable
    {
        #region Private Fields

        private readonly CSSStyles _stl;
        private HtmlTag _parent;
        private string _tagname = "div";
        private Dictionary<string, string> attrs = new Dictionary<string, string>();

        #endregion Private Fields

        #region Internal Fields

        internal List<HtmlTag> _children = new List<HtmlTag>();
        internal string _content;
        internal bool _selfClosing;

        #endregion Internal Fields

        #region Public Constructors

        public HtmlTag() : this(HtmlNodeType.Element)
        {
        }

        public HtmlTag(HtmlNodeType type) : base()
        {
            this.Type = type;
            if (this.Type == HtmlNodeType.Element)
                this.TagName = "div";
            _stl = new CSSStyles(this);
        }

        public HtmlTag(string TagName, string InnerHtml = Ext.EmptyString) : this()
        {
            this.TagName = TagName;
            this.InnerHtml = InnerHtml;
        }

        public HtmlTag(string TagName, object Attributes, string InnerHtml = Ext.EmptyString) : this()
        {
            this.TagName = TagName;
            this.InnerHtml = InnerHtml;

            foreach (var Attr in Attributes.CreateDictionary())
                this.Attributes.SetOrRemove(Attr.Key, Attr.Value);
        }

        public HtmlTag(bool selfClosing, HtmlNodeType type)
        {
            this._selfClosing = selfClosing;
            this.Type = type;
        }

        #endregion Public Constructors

        #region Public Indexers

        [IgnoreDataMember]
        public HtmlTag this[string ID]
        {
            get => Children.FirstOrDefault(x => x.ID == ID);
            set
            {
                if (value != null) AddChildren(value.SetID(ID));
            }
        }

        #endregion Public Indexers

        #region Public Properties

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
                attrs = value ?? attrs;
            }
        }

        [IgnoreDataMember]
        public string AttributeString => Attributes.SelectJoinString(x => $"{x.Key.Replace(" ", "-")}={x.Value.Quote()}", " ");

        /// <summary>
        /// Filhos desta tag
        /// </summary>
        public IEnumerable<HtmlTag> Children
        {
            get
            {
                _children = _children ?? new List<HtmlTag>();
                return _children;
            }
            set
            {
                ClearChildren();
                _children = value?.ToList() ?? new List<HtmlTag>();
            }
        }

        [IgnoreDataMember]
        public string Class
        {
            get => Attributes.GetValueOr("class") ?? Ext.EmptyString;

            set => Attributes["class"] = value;
        }

        [IgnoreDataMember]
        public IEnumerable<string> ClassList
        {
            get => Class.Split(" ");

            set => Class = (value ?? Array.Empty<string>().AsEnumerable()).SelectJoinString(" ");
        }

        [IgnoreDataMember]
        public string Content
        {
            get
            {
                switch (this.Type)
                {
                    case HtmlNodeType.Element:
                        return InnerText;

                    case HtmlNodeType.Text:
                    case HtmlNodeType.Comment:
                        return _content?.HtmlDecode();

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
                        _content = $"{value}".HtmlEncode();
                        break;

                    default:
                        break;
                }
            }
        }

        [IgnoreDataMember]
        public string ID { get => GetAttribute("id").IfBlank(GetAttribute("ID")); set => SetAttribute("id", value, true); }

        [IgnoreDataMember]
        public int Index => Parent?.Children.GetIndexOf(this) ?? -1;

        //   [IgnoreDataMember]
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
                    _selfClosing = false;
                }

                this.ClearChildren();

                if (value.IsNotBlank())
                    this.AddChildren(Parse(value));
            }
        }

        [IgnoreDataMember]
        public string InnerText
        {
            get => this.Type == HtmlNodeType.Element ? Children.Traverse(x => x.Children).Where(x => x.Type == HtmlNodeType.Text).SelectJoinString() : _content;
            set
            {
                if (value.IsNotBlank())
                {
                    _selfClosing = false;
                }

                ClearChildren();
                if (value.IsNotBlank())
                    this.AddChildren(new HtmlTag(HtmlNodeType.Text) { Content = value, _parent = this });
            }
        }

        [IgnoreDataMember]
        public string OuterHtml
        {
            get
            {
                switch (this.Type)
                {
                    case HtmlNodeType.Element:
                        return $"<{TagName.IfBlank("div")}{AttributeString.PrependIf(" ", b => b.IsNotBlank())}" + (SelfClosing ? " />" : $">{InnerHtml}</{TagName.IfBlank("div")}>");

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

        [IgnoreDataMember]
        public HtmlTag Parent => _parent;
        public bool SelfClosing
        {
            get => _selfClosing;

            set
            {
                if (value)
                {
                    ClearChildren();
                }
                _selfClosing = value;
            }
        }

        [IgnoreDataMember]
        public CSSStyles Styles => _stl;

        public string TagName
        {
            get => this.Type == HtmlNodeType.Element ? _tagname : Enum.GetName(typeof(HtmlNodeType), this.Type).Quote('[');
            set => _tagname = value.IfBlank("div");
        }

        public HtmlNodeType Type { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public static HtmlTag CreateFontAwesomeIcon(string Icon) => new HtmlTag("i").AddClass(Icon);

        public static HtmlTag CreateAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => new HtmlTag("a", htmlAttributes, Text).SetAttribute("href", URL, true).SetAttribute("target", Target, true);

        public static HtmlTag CreateBreakLine() => new HtmlTag("br") { SelfClosing = true };

        public static HtmlTag CreateComment(string Comment) => new HtmlTag(HtmlNodeType.Comment).With(x => x.Content = Comment);

        public static HtmlTag CreateHorizontalRule() => new HtmlTag("hr") { SelfClosing = true };

        public static HtmlTag CreateImage(Image Img, object htmlAttributes = null) => CreateImage(Img?.ToDataURL(), htmlAttributes);

        public static HtmlTag CreateImage(string URL, object htmlAttributes = null) => new HtmlTag("img", htmlAttributes, null) { SelfClosing = true }
                      .SetAttribute("src", URL, true);

        public static HtmlTag CreateInput(string Name, string Value = null, string Type = "text", object htmlAttributes = null) => new HtmlTag("input", htmlAttributes, null) { SelfClosing = true }
                              .SetAttribute("name", Name, true)
                              .SetAttribute("value", Value, true)
                              .SetAttribute("type", Type.IfBlank("text"), true);

        public static HtmlTag CreateOption(string Name, string Value = null, bool Selected = false) => new HtmlTag("option", null, Name.RemoveHTML()).SetAttribute("value", Value).SetProp("selected", Selected);

        public static HtmlTag CreateTable(string[][] Table, bool Header = false) => CreateTable(Table?.To2D(), Header);

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
                        row.AddChildren(Header ? "th" : "td", Table[i, j]);
                    }
                    tag.AddChildren(row);
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

        public static HtmlTag CreateTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => CreateTable(Rows, false);

        public static HtmlTag CreateTable<TPoco>(IEnumerable<TPoco> Rows, bool Header) where TPoco : class => CreateTable(Rows, Header, null, null);

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

        public static HtmlTag CreateText(string Text) => new HtmlTag(HtmlNodeType.Text).With(x => x.Content = Text);

        public static HtmlTag CreateWhiteSpace() => new HtmlTag(HtmlNodeType.Text).With(x => x._content = "&nbsp;");



        public static implicit operator string(HtmlTag Tag) => Tag?.ToString();

        public static IEnumerable<HtmlTag> Parse(string HtmlString) => HtmlString.IsNotBlank() ? HtmlParser.Instance.Parse(HtmlString) : Array.Empty<HtmlTag>();

        public static IEnumerable<HtmlTag> Parse(Uri URL) => Parse(URL?.DownloadString());
        public static IEnumerable<HtmlTag> Parse(FileInfo File) => File != null && File.Exists ? Parse(File.ReadAllText()) : Array.Empty<HtmlTag>();

        public static HtmlTag ParseTag(string HtmlString) => Parse(HtmlString).FirstOrDefault();
        public static HtmlTag ParseTag(FileInfo File) => Parse(File).FirstOrDefault();

        public static HtmlTag ParseTag(Uri Url) => Parse(Url).FirstOrDefault();

        public HtmlTag AddAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => AddChildren(CreateAnchor(URL, Text, Target, htmlAttributes));

        public HtmlTag AddAttributes(params (string, string)[] pairs)
        {
            pairs = pairs ?? Array.Empty<(string, string)>();
            return AddAttributes(pairs.ToDictionary(x => x.Item1, x => x.Item2));
        }

        public HtmlTag AddAttributes(IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            if (dictionary != null)
            {
                foreach (var att in dictionary) SetAttribute(att.Key, att.Value);
            }
            return this;
        }

        public HtmlTag AddBreakLine() => AddChildren(CreateBreakLine());

        public HtmlTag AddChildren(string TagName, string InnerHtml = "", Action<HtmlTag> OtherActions = null) => AddChildren(new HtmlTag(TagName, InnerHtml).With(OtherActions));

        public HtmlTag AddChildren(params HtmlTag[] node) => AddChildren((node ?? Array.Empty<HtmlTag>()).AsEnumerable());

        public HtmlTag AddChildren(IEnumerable<HtmlTag> nodes)
        {
            if (nodes != null)
            {
                _selfClosing = false;
                this._children.AddRange(nodes.Where(x => x != null).Each(x => x._parent = this));
            }

            return this;
        }

        public HtmlTag AddClass(params string[] ClassName)
        {
            if (ClassName != null && ClassName.Any())
            {
                ClassList = ClassList.Union(ClassName).Distinct(StringComparer.InvariantCultureIgnoreCase);
            }

            return this;
        }

        public HtmlTag AddComment(string Comment) => AddChildren(CreateComment(Comment));

        public HtmlTag AddHorizontalRule() => AddChildren(CreateHorizontalRule());

        public HtmlTag AddTable(string[][] Table, bool Header = false) => AddChildren(CreateTable(Table, Header));

        public HtmlTag AddTable(string[,] Table, bool Header = false) => AddChildren(CreateTable(Table, Header));

        ///<inheritdoc cref="AddTable{TPoco}(IEnumerable{TPoco}, TPoco, string, string[])"/>
        public HtmlTag AddTable<TPoco>(IEnumerable<TPoco> Rows, bool header, string IDProperty, params string[] Properties) where TPoco : class => AddChildren(CreateTable(Rows, header, IDProperty, Properties));

        ///<inheritdoc cref="AddTable{TPoco}(IEnumerable{TPoco}, TPoco, string, string[])"/>
        public HtmlTag AddTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => AddChildren(CreateTable(Rows));

        /// <summary>
        /// Util a table from <typeparamref name="TPoco"/> classes as a children of this <see cref="HtmlTag"/>
        /// </summary>
        /// <typeparam name="TPoco"></typeparam>
        /// <param name="Rows"></param>
        /// <param name="header"></param>
        /// <param name="IDProperty"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public HtmlTag AddTable<TPoco>(IEnumerable<TPoco> Rows, TPoco header, string IDProperty, params string[] properties) where TPoco : class => AddChildren(CreateTable(Rows, header, IDProperty, properties));

        public HtmlTag AddText(string Text) => AddChildren(CreateText(Text));

        /// <summary>
        /// Add a new <see cref="HtmlTag"/> containing a whitespace
        /// </summary>
        /// <returns></returns>
        public HtmlTag AddWhiteSpace() => AddChildren(CreateWhiteSpace());

        /// <summary>
        /// Clear all children
        /// </summary>
        /// <returns></returns>
        public HtmlTag ClearChildren()
        {
            _children = _children ?? (new List<HtmlTag>());
            _children.Each(x => x._parent = null);
            _children.Clear();
            return this;
        }

        /// <inheritdoc cref="CloneTag"/>
        public object Clone() => CloneTag();

        /// <summary>
        /// Clone this tag into a new <see cref="HtmlTag"/>
        /// </summary>
        /// <returns></returns>
        public HtmlTag CloneTag() => ParseTag(OuterHtml);

        /// <summary>
        /// Remove this tag from parent tag
        /// </summary>
        /// <returns></returns>
        public HtmlTag Detach()
        {
            this.Parent?.RemoveChildren(this);
            return this;
        }

        /// <summary>
        /// Return the first child
        /// </summary>
        /// <returns></returns>
        public HtmlTag FirstChild() => Children.FirstOrDefault();

        public HtmlTag FirstChild(Expression<Func<HtmlTag, bool>> predicate) => predicate != null ? Children.FirstOrDefault(predicate.Compile()) : FirstChild();

        public string GetAttribute(string key) => Attributes.GetValueOr(key, Ext.EmptyString);

        public bool HasAttribute(string AttrName) => AttrName.IsBlank() ? HasAttributes() : Attributes.ContainsKey(AttrName);

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

        public bool HasChildren(Expression<Func<HtmlTag, bool>> predicate) => this.Children?.Any(predicate?.Compile() ?? (x => false)) ?? false;

        public bool HasClass(params string[] Classes) => (Classes?.Any() ?? false ? Classes?.Any(x => ClassList.Contains(x, StringComparer.CurrentCultureIgnoreCase)) : ClassList.Any()) ?? false;

        public HtmlTag Insert(int Index, string TagName, string InnerHtml = "") => Insert(Index, new HtmlTag(TagName, InnerHtml));

        public HtmlTag Insert(int Index, HtmlTag Tag)
        {
            if (Tag != null && Index >= 0)
            {
                Tag._parent = this;
                _selfClosing = false;
                _children.Insert(Index, Tag);
            }
            return this;
        }

        public HtmlTag InsertInto(HtmlTag ParentTag)
        {
            ParentTag?.AddChildren(this);
            return this;
        }

        public HtmlTag LastChild() => Children.LastOrDefault();

        public HtmlTag LastChild(Expression<Func<HtmlTag, bool>> predicate) => predicate != null ? Children.LastOrDefault(predicate.Compile()) : LastChild();

        public HtmlTag RemoveAttribute(string AttrName)
        {
            Attributes.SetOrRemove(AttrName, null, true);
            return this;
        }

        public HtmlTag RemoveChildren(int Index)
        {
            _children.RemoveAt(Index);
            return this;
        }

        public HtmlTag RemoveChildren(Expression<Func<HtmlTag, bool>> predicate) => RemoveChildren(this.Children?.Where(predicate?.Compile() ?? (x => false)) ?? Array.Empty<HtmlTag>());

        public HtmlTag RemoveChildren(params string[] IDs) => RemoveChildren(x => IDs.Any(y => x.ID.Equals(y, StringComparison.Ordinal)));

        public HtmlTag RemoveChildren(IEnumerable<HtmlTag> Children)
        {
            foreach (var item in Children ?? Array.Empty<HtmlTag>())
            {
                if (_children.Contains(item))
                {
                    _children.Remove(item);
                    item._parent = null;
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

        public HtmlTag SetProp(string AttrName, bool Value = true) => Value ? SetAttribute(AttrName, AttrName) : RemoveAttribute(AttrName);

        public override string ToString() => OuterHtml;

        #endregion Public Methods
    }
    public enum HtmlNodeType
    {
        Element,
        Comment,
        Text
    }
}