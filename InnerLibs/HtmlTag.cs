using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Classe para criação de strings contendo tags HTML
    /// </summary>
    public class HtmlTag
    {
        private Dictionary<string, string> attrs = new Dictionary<string, string>();
        private string _innerHtml;

        public HtmlTag()
        {
        }

        public HtmlTag(string TagName, string InnerHtml = "")
        {
            this.TagName = TagName.IfBlank("div");
            this.InnerHtml = InnerHtml;
        }

        public HtmlTag(string TagName, object Attributes, string InnerHtml = "")
        {
            this.TagName = TagName.IfBlank("div");
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

        public string Class
        {
            get => Attributes.GetValueOr("class", "");

            set => Attributes["class"] = value;
        }

        public string[] ClassList
        {
            get => Class.Split(" ");

            set => Class = (value ?? Array.Empty<string>()).SelectJoinString(" ");
        }

        public HtmlTag AddClass(string ClassName)
        {
            if (ClassName.IsNotBlank() && ClassName.IsNotIn(ClassList, StringComparer.InvariantCultureIgnoreCase))
            {
                Class = Class.Append(" " + ClassName);
            }

            return this;
        }
        public HtmlTag RemoveClass(string ClassName)
        {
            if (ClassName.IsNotBlank() && ClassName.IsIn(ClassList, StringComparer.InvariantCultureIgnoreCase))
            {
                ClassList = ClassList.Where(x => x.ToLower() != ClassName.ToLower()).ToArray();
            }

            return this;
        }


        public string InnerHtml
        {
            get => _innerHtml;
            set
            {
                if (value.IsNotBlank())
                {
                    SelfCloseTag = false;
                }
                _innerHtml = value;
            }
        }

        public bool SelfCloseTag { get; set; } = false;

        public string TagName { get; set; } = "div";

        public string this[string key]
        {
            get => Attributes.GetValueOr(key, "");
            set => Attributes.Set(key, value);
        }

        public static HtmlTag CreateAnchor(string URL, string Text, string Target = "_self", object htmlAttributes = null) => new HtmlTag("a", htmlAttributes, Text).SetAttr("href", URL, true).SetAttr("target", Target, true);

        public static HtmlTag CreateImage(string URL, object htmlAttributes = null) => new HtmlTag("img", htmlAttributes, null).SetAttr("src", URL, true).With(x =>
          {
              x.SelfCloseTag = true;
          });

        public static HtmlTag CreateInput(string Name, string Value = null, string Type = "text", object htmlAttributes = null) => new HtmlTag("input", htmlAttributes, null).With(x =>
          {
              x.SelfCloseTag = true;
              x.SetAttr("name", Name, true)
               .SetAttr("value", Value, true)
               .SetAttr("type", Type.IfBlank("text"), true);
          });

        public static HtmlTag CreateOption(string Name, string Value = null, bool Selected = false) => new HtmlTag("option", null, null).With(x =>
          {
              x.InnerHtml = Name.RemoveHTML();
              x.SetAttr("value", Value)
               .SetProp("selected", Selected);
          });

        public static implicit operator string(HtmlTag Tag) => Tag?.ToString();

        public bool HasAttribute(string AttrName) => Attributes.ContainsKey(AttrName);

        public HtmlTag RemoveAttr(string AttrName)
        {
            Attributes.SetOrRemove(AttrName, null, true);
            return this;
        }

        public HtmlTag SetAttr(string AttrName, string Value, bool RemoveIfBlank = false)
        {
            Attributes.SetOrRemove(AttrName, Value, RemoveIfBlank);
            return this;
        }

        public HtmlTag SetProp(string AttrName, bool Value = true) => Value ? SetAttr(AttrName, AttrName) : RemoveAttr(AttrName);

        public override string ToString()
        {
            TagName = TagName.RemoveAny("/", @"\").IfBlank("div");
            return $"<{TagName} {Attributes.SelectJoinString(x => x.Key == x.Value ? x.Key.ToLower() : $"{x.Key.ToLower()}={x.Value.Wrap()}")} " + (SelfCloseTag ? "/>" : $">{InnerHtml}</{TagName}>");
        }
    }
}