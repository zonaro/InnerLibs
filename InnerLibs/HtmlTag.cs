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
                return $"<{TagName.IfBlank("div")} {Attributes.SelectJoinString(x => x.Key.ToLower() + "=" + x.Value.Wrap())} />";
            }
            else
            {
                return $"<{TagName.IfBlank("div")} {Attributes.SelectJoinString(x => x.Key.ToLower() + "=" + x.Value.Wrap())}>{InnerHtml}</{TagName.IfBlank("div")}>";
            }
        }

        public static implicit operator string(HtmlTag Tag)
        {
            return Tag?.ToString();
        }

        public static HtmlTag CreateAnchor(string URL, string Name, string Target = "_self")
        {
            return new HtmlTag("a", Name).With(x =>
            {
                x["href"] = URL;
                x["target"] = Target;
            });
        }

        public static HtmlTag CreateImage(string URL)
        {
            return new HtmlTag("img").With(x =>
            {
                x.SelfCloseTag = true;
                x["src"] = URL;
            });
        }


    }




}