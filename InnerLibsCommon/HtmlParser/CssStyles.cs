using System.Collections.Generic;
using System.Linq;
using Extensions;
using Extensions.Web;

public class CSSStyles
{
    #region Private Methods

    private void ParseStyle() => dic = _tag.GetAttribute("style").Split(";").ToDictionary(x => x.GetBefore(":"), x => x.GetAfter(":"));

    #endregion Private Methods

    #region Internal Fields

    internal HtmlElementNode _tag;

    internal Dictionary<string, string> dic = new Dictionary<string, string>();

    #endregion Internal Fields

    #region Public Constructors

    public CSSStyles(HtmlElementNode tag)
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