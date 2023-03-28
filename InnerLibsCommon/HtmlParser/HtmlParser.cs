using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Extensions.Web
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
            string Text = string.Empty;
            while (length-- > 0)
            {
                if (q.Any())
                    Text += q.Dequeue();
            }
            return Text;
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

        private string GetComment()
        {
            this.Dequeue(4);
            var Text = GetUpTo("-->");
            this.Dequeue(3);
            return Text;
        }

        private string GetTagName() => GetUpTo(' ', '>', '/', '\r', '\n');

        private string GetUpTo(params char[] chars) => GetUpTo(() => chars.Contains(q.Peek()));

        private string GetUpTo(string Text) => GetUpTo(() => this.q.Peek(Text.Length) == Text);

        private string GetUpTo(Func<bool> fn)
        {
            var Text = string.Empty;
            while (q.Any())
            {
                Text += Dequeue();
                if (!q.Any()) return Text;
                if (fn())
                    break;
            }
            return Text;
        }

        private IEnumerable<HtmlNode> InternalParse(HtmlNode _p)
        {
            var list = new List<HtmlNode>();
            HtmlNode node = null;
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
                    node = new HtmlNode(HtmlNodeType.Comment)
                    {
                        _parent = _p,
                        Content = comment
                    };
                    list.Add(node);
                }
                else if (c == '<')
                {
                    node = new HtmlNode() { _parent = _p };

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
                    else if (node.TagName.IfBlank("div").Equals("script", StringComparison.OrdinalIgnoreCase))
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
                    var Text = string.Empty;

                    while (q.Any())
                    {
                        Text += Dequeue();
                        if (q.Any() && q.Peek() == '<') break;
                    }
                    if (Text.IsNotBlank()) list.Add(new HtmlNode(HtmlNodeType.Text) { _parent = _p, Content = Text });
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

        #region Internal Methods

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

        #endregion Internal Methods

        #region Public Properties

        public static IEnumerable<string> SelfClosingTags => new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "menuitem", "meta", "param", "source", "track", "wbr", "!doctype" };

        #endregion Public Properties

        #region Public Methods

        public IEnumerable<HtmlNode> Parse(string source)
        {
            if (source.IsNotBlank())
            {
                this.q = new Queue<char>(source);

                return InternalParse(null);
            }
            return Array.Empty<HtmlNode>();
        }

        #endregion Public Methods
    }

    public enum HtmlNodeType
    {
        Element,
        Comment,
        Text
    }

    public class CSSStyles
    {
        #region Private Methods

        private void ParseStyle() => dic = _tag.GetAttribute("style").Split(";").ToDictionary(x => x.GetBefore(":"), x => x.GetAfter(":"));

        #endregion Private Methods

        #region Internal Fields

        internal HtmlNode _tag;

        internal Dictionary<string, string> dic = new Dictionary<string, string>();

        #endregion Internal Fields

        #region Public Constructors

        public CSSStyles(HtmlNode tag)
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

        public string TextAlign { get => GetStyle("Text-align"); set => SetStyle("Text-align", value); }

        public string TextAlignLast { get => GetStyle("Text-align-last"); set => SetStyle("Text-align-last", value); }

        public string TextDecoration { get => GetStyle("Text-decoration"); set => SetStyle("Text-decoration", value); }

        public string TextDecorationColor { get => GetStyle("Text-decoration-color"); set => SetStyle("Text-decoration-color", value); }

        public string TextDecorationLine { get => GetStyle("Text-decoration-line"); set => SetStyle("Text-decoration-line", value); }

        public string TextDecorationStyle { get => GetStyle("Text-decoration-style"); set => SetStyle("Text-decoration-style", value); }

        public string TextIndent { get => GetStyle("Text-indent"); set => SetStyle("Text-indent", value); }

        public string TextJustify { get => GetStyle("Text-justify"); set => SetStyle("Text-justify", value); }

        public string TextOverflow { get => GetStyle("Text-overflow"); set => SetStyle("Text-overflow", value); }

        public string TextShadow { get => GetStyle("Text-shadow"); set => SetStyle("Text-shadow", value); }

        public string TextTransform { get => GetStyle("Text-transform"); set => SetStyle("Text-transform", value); }

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
    public class HtmlDocument : HtmlNode
    {
        #region Public Constructors

        public HtmlDocument() : base()
        {
            this.TagName = "html";
        }

        public HtmlDocument(bool CreateBasicStructure) : this()
        {
            if (CreateBasicStructure)
            {
                AddChildren("head");
                AddChildren("body");
            }
        }

        public HtmlDocument(string title, string description, string author, string language = null, string charset = null) : this(true)
        {
            Title = title;
            Charset = charset.IfBlank("utf-8");
            Language = language.IfBlank(CultureInfo.CurrentCulture.Name);
            SetMeta("author", author);
            SetMeta("content", description);
            SetMeta("viewport", "width=device-width, initial-scale=1");
        }

        public HtmlDocument(FileInfo file) : this(Parse(file.ReadAllText()).ToArray())
        {
        }

        public HtmlDocument(string HtmlString) : this(Parse(HtmlString).ToArray())
        {
        }

        public HtmlDocument(params HtmlNode[] nodes) : this()
        {
            nodes = nodes ?? Array.Empty<HtmlNode>();
            var headtag = nodes.QuerySelector("head");
            if (headtag != null)
            {
                AddChildren(headtag);
            }
            else
            {
                AddChildren("head");
            }

            var bodytag = nodes.QuerySelector("body");
            if (bodytag != null)
            {
                AddChildren(bodytag);
            }
            else
            {
                AddChildren("body");
            }

            var doctag = nodes.QuerySelector("html");
            if (doctag != null)
            {
                this.AddChildren(doctag.ChildNodes).AddAttributes(doctag.Attributes);
            }

            (nodes.QuerySelector("title") ?? this.QuerySelector("title"))?.InsertInto(this.Head, 0);

            (nodes.QuerySelector("title") ?? this.QuerySelector("title")).InsertInto(this.Head, 0);
        }

        #endregion Public Constructors

        #region Public Properties

        public HtmlNode Body => this.FindFirst(x => x.TagName.EqualsIgnoreCaseAndAccents("body")) ?? this;

        public string Charset
        {
            get => this.FindFirst(x => x.TagName == "meta" && x.HasAttribute("charset"))?.GetAttribute("charset");
            set
            {
                if (value.IsNotBlank())
                {
                    var m = this.FindFirst(x => x.TagName == "meta" && x.HasAttribute("charset")) ?? new HtmlNode("meta") { SelfClosing = true };
                    m.SetAttribute("charset", value);
                    Head?.AddChildren(m);
                }
            }
        }

        public HtmlNode Head => this.FindFirst(x => x.TagName.EqualsIgnoreCaseAndAccents("head")) ?? Body;

        public string Language
        {
            get => this.GetAttribute("lang");
            set
            {
                if (value.IsNotBlank())
                {
                    this.SetAttribute("lang", value);
                }
            }
        }

        public string Title
        {
            get => this.FindFirst(x => x.TagName == "title")?.InnerText;
            set
            {
                if (value.IsNotBlank())
                {
                    var m = this.FindFirst(x => x.TagName == "title") ?? new HtmlNode("title");
                    m.InnerText = value;
                    Head.AddChildren(m);
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public HtmlNode AddInlineCss(string InnerCss)
        {
            if (InnerCss.IsNotBlank())
            {
                var stl = new HtmlNode("style");
                stl.AddText(InnerCss);
                Head.AddChildren(stl);
                return stl;
            }
            return null;
        }

        public HtmlNode AddInlineScript(string jsString)
        {
            if (jsString.IsNotBlank())
            {
                var stl = new HtmlNode("script");
                stl.AddText(jsString);
                Body.AddChildren(stl);
                return stl;
            }
            return null;
        }

        public HtmlNode AddScript(string src)
        {
            if (src.IsNotBlank())
            {
                var scripto = new HtmlNode("script", new { src }) { SelfClosing = true };
                Body.AddChildren(scripto);
                return scripto;
            }
            return null;
        }

        public HtmlNode AddStyle(string href)
        {
            if (href.IsNotBlank())
            {
                var sheet = new HtmlNode("link", new { rel = "stylesheet", href }) { SelfClosing = true };
                Head.AddChildren(sheet);
                return sheet;
            }
            return null;
        }

        public FileInfo Save(string filename, bool Ident = true) => this.ToString(Ident).WriteToFile(filename);

        public HtmlNode SetMeta(string name, string content)
        {
            if (name.IsNotBlank())
            {
                var m = this.FindFirst(x => x.TagName == "meta" && x.GetAttribute("name") == name) ?? new HtmlNode("meta") { SelfClosing = true };
                m.SetAttribute("name", name);
                m.SetAttribute("content", content);
                Head.AddChildren(m);
                return m;
            }
            return null;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// A Helper for generate HTML or XML Tags/Documents
    /// </summary>
    public class HtmlNode : ICloneable
    {
        #region Private Fields

        private readonly CSSStyles _stl;
        private string _tagname = "div";
        private Dictionary<string, string> attrs = new Dictionary<string, string>();

        #endregion Private Fields

        #region Internal Fields

        internal List<HtmlNode> _children = new List<HtmlNode>();
        internal string _content;
        internal HtmlNode _parent;
        internal bool _selfClosing;

        #endregion Internal Fields

        #region Public Constructors

        public HtmlNode() : this(HtmlNodeType.Element)
        {
        }

        public HtmlNode(HtmlNodeType type) : base()
        {
            this.NodeType = type;
            if (this.NodeType == HtmlNodeType.Element)
                this.TagName = "div";
            _stl = new CSSStyles(this);
        }

        public HtmlNode(string TagName, string InnerHtml = Util.EmptyString) : this()
        {
            this.TagName = TagName;
            this.InnerHtml = InnerHtml;
        }

        public HtmlNode(string TagName, object Attributes, string InnerHtml = Util.EmptyString) : this()
        {
            this.TagName = TagName;
            this.InnerHtml = InnerHtml;

            foreach (var Attr in Attributes.CreateDictionary())
                this.Attributes.SetOrRemove(Attr.Key, Attr.Value);
        }

        public HtmlNode(bool selfClosing) : this()
        {
            this._selfClosing = selfClosing;
        }

        #endregion Public Constructors

        #region Public Indexers

        [IgnoreDataMember]
        public HtmlNode this[string ID]
        {
            get => ChildNodes.FirstOrDefault(x => x.Id == ID);
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
                attrs = value ?? new Dictionary<string, string>();
            }
        }

        [IgnoreDataMember]
        public string AttributeString
        {
            get => Attributes.SelectJoinString(x => $"{x.Key.Replace(" ", "-")}={x.Value.Quote()}", " ");
            set => this.Attributes = ParseTag($"<attr {value} />").Attributes.ToDictionary();
        }

        /// <summary>
        /// Filhos desta tag
        /// </summary>
        public IEnumerable<HtmlNode> ChildNodes
        {
            get
            {
                _children = _children ?? new List<HtmlNode>();
                return _children;
            }
            set
            {
                ClearChildren();
                AddChildren(value);
            }
        }

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

        [IgnoreDataMember]
        public string Content
        {
            get
            {
                switch (this.NodeType)
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
                switch (this.NodeType)
                {
                    case HtmlNodeType.Element:
                        InnerHtml = value;
                        break;

                    case HtmlNodeType.Comment:
                    case HtmlNodeType.Text:
                        _content = value?.HtmlEncode() ?? string.Empty;
                        break;

                    default:
                        break;
                }
            }
        }

        public int DepthLevel => this.ParentNode?.DepthLevel + 1 ?? 0;

        [IgnoreDataMember]
        public string Id { get => GetAttribute("id").BlankCoalesce(GetAttribute("Id"), GetAttribute("ID")); set => SetAttribute("id", value, true); }

        [IgnoreDataMember]
        public int Index => ParentNode?.ChildNodes.GetIndexOf(this) ?? -1;

        public string InnerHtml
        {
            get
            {
                switch (this.NodeType)
                {
                    case HtmlNodeType.Text:
                    case HtmlNodeType.Comment:
                        return Content;

                    default:
                    case HtmlNodeType.Element:
                        return ToString();
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
            get => this.NodeType == HtmlNodeType.Element ? ChildNodes.Traverse(x => x.ChildNodes).Where(x => x.NodeType == HtmlNodeType.Text).SelectJoinString() : _content;
            set
            {
                if (value.IsNotBlank())
                {
                    _selfClosing = false;
                }

                ClearChildren();
                if (value.IsNotBlank())
                    this.AddChildren(new HtmlNode(HtmlNodeType.Text) { Content = value, _parent = this });
            }
        }

        [IgnoreDataMember]
        public bool IsComment => this.NodeType == HtmlNodeType.Comment;

        [IgnoreDataMember]
        public bool IsElement => this.NodeType == HtmlNodeType.Element;

        [IgnoreDataMember]
        public bool IsText => this.NodeType == HtmlNodeType.Text;

        public HtmlNode NextSibling => this.ParentNode?.ChildNodes.FirstOrDefault(x => x.Index == this.Index + 1);

        public HtmlNodeType NodeType { get; private set; }

        [IgnoreDataMember]
        public string OuterHtml
        {
            get => ToString();

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
        public HtmlNode ParentNode => _parent;

        public HtmlNode PreviousSibling => this.ParentNode?.ChildNodes.FirstOrDefault(x => x.Index == this.Index - 1);

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
            get => this.NodeType == HtmlNodeType.Element ? _tagname : Enum.GetName(typeof(HtmlNodeType), this.NodeType).Quote('[');
            set => _tagname = value.IfBlank("div");
        }

        #endregion Public Properties

        #region Public Methods

        public static HtmlNode CreateAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => new HtmlNode("a", htmlAttributes, Text).SetAttribute("src", URL, true).SetAttribute("target", Target, true);

        public static HtmlNode CreateAnchor(Uri URL, string Text, string Target = "_self", object htmlAttributes = null) => CreateAnchor(URL.AbsoluteUri, Text, Target, htmlAttributes);

        public static HtmlNode CreateBreakLine() => new HtmlNode("br") { SelfClosing = true };

        public static HtmlNode CreateComment(string Comment) => new HtmlNode(HtmlNodeType.Comment).With(x => x.Content = Comment);

        public static HtmlNode CreateFontAwesomeIcon(string Icon) => new HtmlNode("i").AddClass(Icon);

        public static HtmlNode CreateHorizontalRule() => new HtmlNode("hr") { SelfClosing = true };

        public static HtmlNode CreateImage(Image Img, object htmlAttributes = null) => CreateImage(Img?.ToDataURL(), htmlAttributes);

        public static HtmlNode CreateImage(string URL, object htmlAttributes = null) => new HtmlNode("img", htmlAttributes, null) { SelfClosing = true }.SetAttribute("src", URL, true);

        public static HtmlNode CreateInput(string Name, string Value = null, string Type = "Text", object htmlAttributes = null) => new HtmlNode("input", htmlAttributes, null) { SelfClosing = true }
                               .SetAttribute("name", Name, true)
                               .SetAttribute("value", Value, true)
                               .SetAttribute("type", Type.IfBlank("Text"), true);

        public static HtmlNode CreateList<T>(bool Ordened, IEnumerable<T> items, Expression<Func<T, string>> ItemHtml, Expression<Func<T, object>> ItemAttribute = null)
        {
            var node = new HtmlNode(Ordened ? "ol" : "ul");
            var arr = (items ?? Array.Empty<T>()).ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                if (item != null)
                {
                    var li = new HtmlNode("li");
                    if (ItemHtml != null)
                    {
                        li.InnerHtml = ItemHtml.Compile().Invoke(item);
                    }

                    if (ItemAttribute != null)
                    {
                        li.Attributes = ItemAttribute.Compile().Invoke(item)?.CreateDictionary().ToDictionary(x => x.Key, x => x.Value.ChangeType<string>());
                    }
                    node.AddChildren(li);
                }
                else node.AddComment($"{typeof(T).Name} at {i} is null");
            }
            return node;
        }

        public static HtmlNode CreateOption(string Name, string Value = null, bool Selected = false) => new HtmlNode("option", null, Name.RemoveHTML()).SetAttribute("value", Value).SetProp("selected", Selected);

        public static HtmlNode CreateTable(string[][] Table, bool Header = false) => CreateTable(Table?.To2D(), Header);

        public static HtmlNode CreateTable(string[,] Table, bool Header = false)
        {
            HtmlNode tag = new HtmlNode("table");

            if (Table != null)
                for (int i = 0; i < Table.GetLength(0); i++)
                {
                    var row = new HtmlNode("tr");
                    for (int j = 0; j < Table.GetLength(1); j++)
                    {
                        Header = Header && i == 0;
                        row.AddChildren(Header ? "th" : "td", Table[i, j]);
                    }
                    tag.AddChildren(row);
                }
            return tag;
        }

        public static HtmlNode CreateTable<TPoco>(IEnumerable<TPoco> Rows, bool header, string IDProperty, params string[] Properties) where TPoco : class
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

        public static HtmlNode CreateTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => CreateTable(Rows, false);

        public static HtmlNode CreateTable<TPoco>(IEnumerable<TPoco> Rows, bool Header) where TPoco : class => CreateTable(Rows, Header, null, null);

        public static HtmlNode CreateTable<TPoco>(IEnumerable<TPoco> Rows, TPoco Header, string IDProperty, params string[] Properties) where TPoco : class
        {
            HtmlNode tag = new HtmlNode("table");

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

        public static HtmlNode CreateText(string Text) => new HtmlNode(HtmlNodeType.Text).With(x => x.Content = Text);

        public static HtmlNode CreateWhiteSpace() => new HtmlNode(HtmlNodeType.Text).With(x => x._content = "&nbsp;");

        public static explicit operator HtmlNode[](HtmlNode d) => d.ChildNodes.ToArray();

        public static implicit operator string(HtmlNode Tag) => Tag?.ToString();

        public static IEnumerable<HtmlNode> Parse(string HtmlString) => HtmlString.IsNotBlank() ? HtmlParser.Instance.Parse(HtmlString) : Array.Empty<HtmlNode>();

        public static IEnumerable<HtmlNode> Parse(Uri URL) => Parse(URL?.DownloadString());

        public static IEnumerable<HtmlNode> Parse(FileInfo File) => File != null && File.Exists ? Parse(File.ReadAllText()) : Array.Empty<HtmlNode>();

        public static HtmlDocument ParseDocument(Uri URL) => ParseDocument(URL?.DownloadString());

        public static HtmlDocument ParseDocument(FileInfo File) => File != null && File.Exists ? ParseDocument(File.ReadAllText()) : new HtmlDocument();

        /// <summary>
        /// Parse a Html string into a <see cref="HtmlDocument"/> ensuring a HTML node as root of document
        /// </summary>
        /// <param name="HtmlString"></param>
        /// <returns></returns>
        public static HtmlDocument ParseDocument(string HtmlString) => new HtmlDocument(HtmlString);

        public static HtmlNode ParseTag(string HtmlString) => Parse(HtmlString).FirstOrDefault();

        public static HtmlNode ParseTag(FileInfo File) => Parse(File).FirstOrDefault();

        public static HtmlNode ParseTag(Uri Url) => Parse(Url).FirstOrDefault();

        public HtmlNode AddAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => AddChildren(CreateAnchor(URL, Text, Target, htmlAttributes));

        public HtmlNode AddAttributes<T>(params T[] pairs)
        {
            pairs = pairs ?? Array.Empty<T>();
            foreach (var obj in pairs)
            {
                var dictionary = obj.CreateDictionary();
                if (dictionary != null)
                {
                    foreach (var att in dictionary) SetAttribute(att.Key, $"{att.Value}");
                }
            }
            return this;
        }

        public HtmlNode AddBreakLine() => AddChildren(CreateBreakLine());

        public HtmlNode AddChildren(string TagName, string InnerHtml = "", Action<HtmlNode> OtherActions = null) => AddChildren(new HtmlNode(TagName, InnerHtml).With(OtherActions));

        public HtmlNode AddChildren(params HtmlNode[] node) => AddChildren((node ?? Array.Empty<HtmlNode>()).AsEnumerable());

        public HtmlNode AddChildren(IEnumerable<HtmlNode> nodes, bool copy = false)
        {
            if (nodes != null)
            {
                var nn = nodes.ToArray();
                for (int i = 0; i < nn.Length; i++)
                {
                    nn[i].InsertInto(this, copy);
                }
            }

            return this;
        }

        public HtmlNode AddClass(params string[] ClassName)
        {
            if (ClassName != null && ClassName.Any())
            {
                ClassList = ClassList.Union(ClassName).Distinct(StringComparer.InvariantCultureIgnoreCase);
            }

            return this;
        }

        public HtmlNode AddComment(string Comment) => AddChildren(CreateComment(Comment));

        public HtmlNode AddHorizontalRule() => AddChildren(CreateHorizontalRule());

        public HtmlNode AddList<T>(bool Ordened, IEnumerable<T> items, Expression<Func<T, string>> ItemHtml, Expression<Func<T, object>> ItemAttribute = null) => AddChildren(CreateList(Ordened, items, ItemHtml, ItemAttribute));

        public HtmlNode AddTable(string[][] Table, bool Header = false) => AddChildren(CreateTable(Table, Header));

        public HtmlNode AddTable(string[,] Table, bool Header = false) => AddChildren(CreateTable(Table, Header));

        ///<inheritdoc cref="AddTable{TPoco}(IEnumerable{TPoco}, TPoco, string, string[])"/>
        public HtmlNode AddTable<TPoco>(IEnumerable<TPoco> Rows, bool header, string IDProperty, params string[] Properties) where TPoco : class => AddChildren(CreateTable(Rows, header, IDProperty, Properties));

        ///<inheritdoc cref="AddTable{TPoco}(IEnumerable{TPoco}, TPoco, string, string[])"/>
        public HtmlNode AddTable<TPoco>(IEnumerable<TPoco> Rows) where TPoco : class => AddChildren(CreateTable(Rows));

        /// <summary>
        /// Generate a table from <typeparamref name="TPoco"/> classes as a children of this <see cref="HtmlNode"/>
        /// </summary>
        /// <typeparam name="TPoco"></typeparam>
        /// <param name="Rows"></param>
        /// <param name="header"></param>
        /// <param name="IDProperty"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public HtmlNode AddTable<TPoco>(IEnumerable<TPoco> Rows, TPoco header, string IDProperty, params string[] properties) where TPoco : class => AddChildren(CreateTable(Rows, header, IDProperty, properties));

        public HtmlNode AddText(string Text) => AddChildren(CreateText(Text));

        /// <summary>
        /// Add a new <see cref="HtmlNode"/> containing a whitespace
        /// </summary>
        /// <returns></returns>
        public HtmlNode AddWhiteSpace() => AddChildren(CreateWhiteSpace());

        /// <summary>
        /// Clear all children
        /// </summary>
        /// <returns></returns>
        public HtmlNode ClearChildren()
        {
            while (ChildNodes.Any())
            {
                RemoveChildren(0);
            }
            return this;
        }

        /// <inheritdoc cref="CloneTag"/>
        public object Clone() => CloneTag();

        /// <summary>
        /// Clone this tag into a new <see cref="HtmlNode"/>
        /// </summary>
        /// <returns></returns>
        public HtmlNode CloneTag() => ParseTag(OuterHtml);

        public HtmlNode Closest(Expression<Func<HtmlNode, bool>> predicate) => predicate != null ? this.Traverse(x => x.ParentNode, predicate).LastOrDefault(x => x != this) : null;

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

        /// <summary>
        /// Remove this tag from parent tag
        /// </summary>
        /// <returns></returns>
        public HtmlNode Detach()
        {
            this.ParentNode?.RemoveChildren(this);
            return this;
        }

        public IEnumerable<HtmlNode> Find(string selector) => this.QuerySelectorAll(selector).Where(x => x != this);

        public IEnumerable<HtmlNode> Find(Expression<Func<HtmlNode, bool>> predicate) => predicate != null ? this.Traverse(x => x.ChildNodes).Where(predicate.Compile()).Where(x => x != this) : default;

        public HtmlNode FindFirst(string selector) => Find(selector).FirstOrDefault();

        public HtmlNode FindFirst(Expression<Func<HtmlNode, bool>> predicate) => Find(predicate).FirstOrDefault();

        public HtmlNode FindLast(string selector) => Find(selector).LastOrDefault();

        public HtmlNode FindLast(Expression<Func<HtmlNode, bool>> predicate) => Find(predicate).LastOrDefault();

        /// <summary>
        /// Return the first child
        /// </summary>
        /// <returns></returns>
        public HtmlNode FirstChild() => ChildNodes.FirstOrDefault();

        public HtmlNode FirstChild(Expression<Func<HtmlNode, bool>> predicate) => predicate != null ? ChildNodes.FirstOrDefault(predicate.Compile()) : FirstChild();

        public string GetAttribute(string key) => Attributes?.GetValueOr(key) ?? Util.EmptyString;

        public IEnumerable<HtmlNode> GetChildElements() => this.ChildNodes.Where(i => i.NodeType == HtmlNodeType.Element);

        public Dictionary<string, string> GetData() => Attributes.Where(x => x.Key.StartsWith("data-", StringComparison.OrdinalIgnoreCase)).ToDictionary();

        public string GetData(string Key) => GetAttribute("data-" + Key);

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

        /// <summary>
        /// Inject values from <typeparamref name="T"/> object into <see cref="Content"/> and <see
        /// cref="Attributes"/> of this <see cref="HtmlNode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public HtmlNode Inject<T>(T obj)
        {
            foreach (var att in this.Attributes)
            {
                SetAttribute(att.Key, att.Value.Inject(obj));
            }

            foreach (var el in this.ChildNodes.Traverse(x => x.ChildNodes))
            {
                if (el.NodeType == HtmlNodeType.Text || el.NodeType == HtmlNodeType.Comment)
                {
                    el.Content = el.Content.Inject(obj);

                    foreach (var att in el.Attributes)
                    {
                    }
                }
            }
            return this;
        }

        public HtmlNode Insert(int Index, string TagName, string InnerHtml = "") => Insert(Index, new HtmlNode(TagName, InnerHtml));

        public HtmlNode Insert(int Index, HtmlNode Tag, bool copy = false)
        {
            if (Tag != null)
            {
                Tag = copy ? Tag.CloneTag() : Tag.Detach();
                Tag._parent = this;
                _selfClosing = false;
                NodeType = HtmlNodeType.Element;
                if (Index >= 0)
                {
                    _children.Insert(Index.LimitIndex(_children), Tag);
                }
                else
                {
                    _children.Add(Tag);
                };
            }
            return this;
        }

        public HtmlNode InsertInto(HtmlNode toHtmlNode, bool copy = false) => InsertInto(toHtmlNode, -1, copy);

        public HtmlNode InsertInto(HtmlNode toHtmlNode, int Index, bool copy = false)
        {
            if (toHtmlNode != null)
            {
                toHtmlNode.Insert(Index, this, copy);
            }

            return this;
        }

        public HtmlNode LastChild() => ChildNodes.LastOrDefault();

        public HtmlNode LastChild(Expression<Func<HtmlNode, bool>> predicate) => predicate != null ? ChildNodes.LastOrDefault(predicate.Compile()) : LastChild();

        public HtmlNode NextSiblingElement()
        {
            var rt = this.NextSibling;

            while (rt != null && rt.NodeType != HtmlNodeType.Element)
                rt = rt.NextSibling;

            return rt;
        }

        public HtmlNode PreviousSiblingElement()
        {
            var rt = this.PreviousSibling;

            while (rt != null && rt.NodeType != HtmlNodeType.Element)
                rt = rt.PreviousSibling;

            return rt;
        }

        public HtmlNode RemoveAttribute(string AttrName)
        {
            Attributes.SetOrRemove(AttrName, null, true);
            return this;
        }

        public HtmlNode RemoveChildren(int Index)
        {

            RemoveChildren(x => x.Index == Index)


            return this;
        }

        public HtmlNode RemoveChildren(Expression<Func<HtmlNode, bool>> predicate) => RemoveChildren(this.ChildNodes?.Where(predicate?.Compile() ?? (x => false)) ?? Array.Empty<HtmlNode>());

        public HtmlNode RemoveChildren(params string[] IDs) => RemoveChildren(x => IDs.Any(y => x.Id.Equals(y, StringComparison.Ordinal)));

        public HtmlNode RemoveChildren(params HtmlNode[] Children) => RemoveChildren(Children?.AsEnumerable());

        public HtmlNode RemoveChildren(IEnumerable<HtmlNode> Children)
        {
            foreach (var item in Children ?? Array.Empty<HtmlNode>())
            {
                if (_children.Contains(item))
                {
                    _children.Remove(item);
                    item._parent = null;
                }
            }
            return this;
        }

        public HtmlNode RemoveClass(string ClassName)
        {
            if (ClassName.IsNotBlank() && ClassName.IsIn(ClassList, StringComparer.InvariantCultureIgnoreCase))
            {
                ClassList = ClassList.Where(x => x != null && !x.Equals(ClassName, StringComparison.OrdinalIgnoreCase)).ToArray();
            }

            return this;
        }

        public HtmlNode SetAttribute(string AttrName, string Value, bool RemoveIfBlank = false)
        {
            if (AttrName.IsNotBlank())
                Attributes.SetOrRemove(AttrName, Value, RemoveIfBlank);
            return this;
        }

        public HtmlNode SetData(string Key, string value, bool RemoveIfBlank = false) => SetAttribute(Key.PrependIf("data-", x => x.IsNotBlank()), value, RemoveIfBlank);

        public HtmlNode SetID(string Value)
        {
            Id = Value;
            return this;
        }

        public HtmlNode SetInnerHtml(string Html)
        {
            this.InnerHtml = Html;
            return this;
        }

        public HtmlNode SetInnerText(string Text)
        {
            this.InnerText = Text;
            return this;
        }

        public HtmlNode SetProp(string AttrName, bool Value = true) => Value ? SetAttribute(AttrName, AttrName) : RemoveAttribute(AttrName);

        public HtmlNode SetStyle(string StyleName, string Value)
        {
            Styles.SetStyle(StyleName, Value);
            return this;
        }

        public override string ToString() => ToString(false);

        public virtual string ToString(bool Ident)
        {
            var html = "";
            switch (this.NodeType)
            {
                case HtmlNodeType.Text:
                    html = $"{Content}";
                    break;

                case HtmlNodeType.Comment:
                    html = $"<!-- {Content} -->";
                    break;

                case HtmlNodeType.Element:
                default:
                    html = $"<{TagName.IfBlank("div")}{AttributeString.PrependIf(" ", b => b.IsNotBlank())}{(SelfClosing ? (TagName.EqualsIgnoreCaseAndAccents("!doctype") ? " >" : " />") : $">{ChildNodes.SelectJoinString(x => x.ToString(Ident))}</{TagName.IfBlank("div")}>")}";
                    break;
            }
            if (Ident)
            {
                var tabs = Util.Repeat(Util.TabChar, DepthLevel - 1);
                html = $"{Environment.NewLine.NullIf(DepthLevel == 0)}{Util.TabChar.NullIf(DepthLevel == 0)}{tabs}{html}{Environment.NewLine}{tabs}";
            }
            return html;
        }

        #endregion Public Methods
    }
}