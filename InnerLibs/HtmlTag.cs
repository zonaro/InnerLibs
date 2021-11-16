using System;
using System.Collections.Generic;
using InnerLibs.LINQ;

namespace InnerLibs
{
    /// <summary>
    /// Classe para criação de strings contendo tags HTML
    /// </summary>
    public class HtmlTag
    {
        public string TagName { get; set; } = "div";
        public string InnerHtml { get; set; }
        public Dictionary<string, string> Attributes
        {
            get
            {
                attrs = attrs ?? new Dictionary<string, string>();
                return attrs;
            }
        }
        private Dictionary<string, string> attrs = new Dictionary<string, string>();

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
                this.Attributes.SetOrRemove(Attr.Key, Attr.Value);
        }

        public string this[string key]
        {
            get => Attributes.GetValueOr(key, "");
            set => Attributes.Set(key, value);
        }

        public string Class
        {
            get => Attributes.GetValueOr("class", "");

            set => Attributes["class"] = value;
        }

        public string[] ClassArray
        {
            get => Class.Split(" ");

            set => Class = (value ?? Array.Empty<string>()).JoinString(" ");
        }

        public bool SelfCloseTag { get; set; } = false;

        public override string ToString()
        {
            TagName = TagName.RemoveAny("/", @"\");
            if (SelfCloseTag)
            {
                return $"<{TagName.IfBlank("div")} {Attributes.SelectJoinString(x => $"{x.Key.ToLower()}={x.Value.Wrap()}")} />";
            }
            else
            {
                return $"<{TagName.IfBlank("div")} {Attributes.SelectJoinString(x => $"{x.Key.ToLower()}={x.Value.Wrap()}")}>{InnerHtml}</{TagName.IfBlank("div")}>";
            }
        }

        public static implicit operator string(HtmlTag Tag) => Tag?.ToString();

        public static HtmlTag CreateAnchor(string URL, string Name, string Target = "_self", object htmlAttributes = null)
        {
            return new HtmlTag("a", htmlAttributes, Name).With(x =>
            {
                x.Attributes
                .SetOrRemove("href", URL, true)
                .SetOrRemove("target", Target, true);

            });
        }

        public static HtmlTag CreateInput(string Name, string Value = null, string Type = "text", object htmlAttributes = null)
        {
            return new HtmlTag("input", htmlAttributes, null).With(x =>
             {
                 x.SelfCloseTag = true;
                 x.Attributes
                  .SetOrRemove("name", Name, true)
                  .SetOrRemove("value", Value, true)
                  .SetOrRemove("type", Type.IfBlank("text"), true);

             });
        }

        public static HtmlTag CreateImage(string URL, object htmlAttributes = null)
        {
            return new HtmlTag("img", htmlAttributes, null).With(x =>
          {
              x.SelfCloseTag = true;
              x.Attributes
               .SetOrRemove("src", URL, true);

          });
        }


    }




}