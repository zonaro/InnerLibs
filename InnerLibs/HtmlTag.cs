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
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public HtmlTag()
        {
        }

        public HtmlTag(string TagName, string InnerHtml = "")
        {
            this.TagName = TagName.IfBlank("div");
            this.InnerHtml = InnerHtml;
        }

        public string Class
        {
            get
            {
                return Attributes.GetValueOr("class", "");
            }

            set
            {
                Attributes = Attributes ?? new Dictionary<string, string>();
                Attributes["class"] = value;
            }
        }

        public string[] ClassArray
        {
            get
            {
                return Class.Split(" ");
            }

            set
            {
                Class = (value ?? Array.Empty<string>()).JoinString(" ");
            }
        }

        public override string ToString()
        {
            TagName = TagName.RemoveAny("/", @"\");
            Attributes = Attributes ?? new Dictionary<string, string>();
            return $"<{TagName.IfBlank("div")} {Attributes.SelectJoinString(x => x.Key.ToLower() + "=" + x.Value.Wrap())}>{InnerHtml}</{TagName.IfBlank("div")}>";
        }

        public static implicit operator string(HtmlTag Tag)
        {
            return Tag?.ToString();
        }
    }
}