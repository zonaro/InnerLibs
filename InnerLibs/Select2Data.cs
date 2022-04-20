using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs.Select2
{
    public interface ISelect2Option : ISelect2Result
    {
        bool Disabled { get; set; }
        string ID { get; set; }
        bool Selected { get; set; }
    }

    public interface ISelect2Result
    {
        string Text { get; set; }
    }

    public static class Select2Extensions
    {
        public static Select2Data CreateSelect2Data<OptionType, T>(this IEnumerable<T> List, Func<T, string> TextSelector, Func<T, string> IdSelector, Action<T, OptionType> OtherSelectors = null, Func<T, string> GroupSelector = null) where OptionType : ISelect2Option
        {
            if (GroupSelector != null)
            {
                var itens = List.GroupBy(GroupSelector).Select(x => new Select2Group(x.Key, (IEnumerable<ISelect2Option>)x.Select(c => c.CreateSelect2Option(TextSelector, IdSelector, OtherSelectors))));
                return new Select2Data(itens);
            }
            else
            {
                var itens = List.Select(c => c.CreateSelect2Option(TextSelector, IdSelector, OtherSelectors));
                return new Select2Data((IEnumerable<ISelect2Option>)itens);
            }
        }

        public static Select2Data CreateSelect2Data<OptionsType, T1, T2>(this PaginationFilter<T1, T2> Filter, Func<T2, string> TextSelector, Func<T2, string> IdSelector, Func<T2, string> GroupSelector = null, Action<T2, OptionsType> OtherSelectors = null)
            where OptionsType : ISelect2Option
            where T1 : class
        {
            var d = Filter.GetPage().CreateSelect2Data(TextSelector, IdSelector, OtherSelectors, GroupSelector);
            d.Pagination.More = Filter.PageCount > 1 && Filter.IsLastPage == false;
            return d;
        }

        public static Select2Data CreateSelect2Data<OptionsType, T1, T2>(this PaginationFilter<T1, T2> Filter, Func<T2, string> TextSelector, Func<T2, string> IdSelector, Action<T2, OptionsType> OtherSelectors)
            where OptionsType : ISelect2Option
            where T1 : class
        {
            return Filter.CreateSelect2Data(TextSelector, IdSelector, null, OtherSelectors);
        }

        public static Select2Data CreateSelect2Data<OptionsType, T1>(this PaginationFilter<T1, OptionsType> Filter)
            where OptionsType : ISelect2Option
            where T1 : class
        {
            return Filter.CreateSelect2Data<OptionsType, T1, OptionsType>((x) => x.Text, (x) => x.ID);
        }

        public static Select2Data CreateSelect2Data<OptionsType, T1>(this PaginationFilter<T1, OptionsType> Filter, Func<OptionsType, string> GroupBySelector)
            where OptionsType : ISelect2Option
            where T1 : class
        {
            return Filter.CreateSelect2Data<OptionsType, T1, OptionsType>((x) => x.Text, (x) => x.ID, GroupBySelector);
        }

        public static OptionType CreateSelect2Option<OptionType, T>(this T item, Func<T, string> TextSelector, Func<T, string> IdSelector, Action<T, OptionType> OtherSelectors = null) where OptionType : ISelect2Option
        {
            if (ReferenceEquals(typeof(T), typeof(OptionType)))
            {
                return item.ChangeType<OptionType>();
            }
            else
            {
                IdSelector = IdSelector ?? TextSelector;
                TextSelector = TextSelector ?? IdSelector;
                var Optionitem = Activator.CreateInstance<OptionType>();
                Optionitem.ID = IdSelector(item);
                Optionitem.Text = TextSelector(item);
                if (OtherSelectors != null)
                {
                    OtherSelectors(item, Optionitem);
                }

                return Optionitem;
            }
        }
    }

    public class Pagination
    {
        public bool More { get; set; } = false;
    }

    /// <summary>
    /// Classe utilizada para auxiliar nas respostas de requisições feitas por aAJAX através do select2.js
    /// </summary>
    public class Select2Data
    {
        public Select2Data()
        {
        }

        public Select2Data(IEnumerable<ISelect2Option> Options)
        {
            Results = Options ?? Array.Empty<ISelect2Option>();
        }

        public Select2Data(IEnumerable<ISelect2Option> Options, bool PaginationMore)
        {
            Results = Options ?? Array.Empty<ISelect2Option>();
            Pagination.More = PaginationMore;
        }

        public Select2Data(IEnumerable<Select2Group> Groups)
        {
            Results = Groups ?? Array.Empty<Select2Group>();
        }

        public Select2Data(IEnumerable<Select2Group> Groups, bool PaginationMore)
        {
            Results = Groups ?? Array.Empty<Select2Group>();
            Pagination.More = PaginationMore;
        }

        public Pagination Pagination { get; set; } = new Pagination();
        public IEnumerable<ISelect2Result> Results { get; set; } = Array.Empty<ISelect2Result>();
    }

    public class Select2Group : ISelect2Result
    {
        public Select2Group(string Text)
        {
            this.Text = Text;
        }

        public Select2Group(string Text, IEnumerable<ISelect2Option> Children)
        {
            this.Text = Text;
            this.Children = Children ?? Array.Empty<ISelect2Option>();
        }

        public Select2Group()
        {
        }

        public IEnumerable<ISelect2Option> Children { get; set; } = Array.Empty<ISelect2Option>();
        public string Text { get; set; }
    }

    public sealed class Select2Option : ISelect2Option
    {
        public Select2Option()
        {
        }

        public Select2Option(string Text, string Value)
        {
            ID = Value.IfBlank(Text);
            this.Text = Text;
        }

        public bool Disabled { get; set; }
        public string ID { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"<option value='{ID}'{Disabled.AsIf(" disabled")}{Selected.AsIf(" selected")}>{Text}</option>";
        }
    }
}