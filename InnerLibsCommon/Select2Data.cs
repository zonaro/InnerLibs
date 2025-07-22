
using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Pagination;

namespace Extensions.Select2
{
    public static class Select2Extensions
    {
        #region Public Methods

        public static Select2Data<T> CreateSelect2Data<T>(this IEnumerable<T> List) where T : ISelect2Option => CreateSelect2Data<T, T>(List, x => x.Text, x => x.ID);

        public static Select2Data<Select2Option> CreateSelect2Data(this IEnumerable<Select2Option> List) => CreateSelect2Data<Select2Option, Select2Option>(List, x => x.Text, x => x.ID);

        public static Select2Data<OptionType> CreateSelect2Data<OptionType, T>(this IEnumerable<T> List, Func<T, string> TextSelector, Func<T, string> IdSelector, Action<T, OptionType> OtherSelectors = null, Func<T, string> GroupSelector = null) where OptionType : ISelect2Option
        {
            if (GroupSelector != null)
            {
                var itens = List.GroupBy(GroupSelector).Select(x => new Select2Group<OptionType>(x.Key, x.Select(c => c.CreateSelect2Option<OptionType, T>(TextSelector, IdSelector, OtherSelectors))));
                return new Select2Data<OptionType>(itens);
            }
            else
            {
                var itens = List.Select(c => c.CreateSelect2Option(TextSelector, IdSelector, OtherSelectors));
                return new Select2Data<OptionType>(itens.Cast<ISelect2Option>());
            }
        }

        public static Select2Data<OptionsType> CreateSelect2Data<OptionsType, T1, T2>(this PaginationFilter<T1, T2> Filter, Func<T2, string> TextSelector, Func<T2, string> IdSelector, Func<T2, string> GroupSelector = null, Action<T2, OptionsType> OtherSelectors = null)
            where OptionsType : ISelect2Option
            where T1 : class
        {
            var d = Filter.GetPage().CreateSelect2Data(TextSelector, IdSelector, OtherSelectors, GroupSelector);
            d.Pagination.More = Filter.PageCount > 1 && Filter.IsLastPage == false;
            return d;
        }

        public static Select2Data<OptionsType> CreateSelect2Data<OptionsType, T1, T2>(this PaginationFilter<T1, T2> Filter, Func<T2, string> TextSelector, Func<T2, string> IdSelector, Action<T2, OptionsType> OtherSelectors)
            where OptionsType : ISelect2Option
            where T1 : class => Filter.CreateSelect2Data(TextSelector, IdSelector, null, OtherSelectors);

        public static Select2Data<OptionsType> CreateSelect2Data<OptionsType, T1>(this PaginationFilter<T1, OptionsType> Filter)
            where OptionsType : ISelect2Option
            where T1 : class => Filter.CreateSelect2Data<OptionsType, T1, OptionsType>((x) => x.Text, (x) => x.ID);

        public static Select2Data<OptionsType> CreateSelect2Data<OptionsType, T1>(this PaginationFilter<T1, OptionsType> Filter, Func<OptionsType, string> GroupBySelector)
            where OptionsType : ISelect2Option
            where T1 : class => Filter.CreateSelect2Data<OptionsType, T1, OptionsType>((x) => x.Text, (x) => x.ID, GroupBySelector);

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
                OtherSelectors?.Invoke(item, Optionitem);
                return Optionitem;
            }
        }

        #endregion Public Methods
    }

    public class Pagination
    {
        #region Public Properties

        public bool More { get; set; } = false;

        #endregion Public Properties
    }

    /// <summary>
    /// Classe utilizada para auxiliar nas respostas de requisições feitas por aAJAX através do select2.js
    /// </summary>
    public class Select2Data<OptionType> where OptionType : ISelect2Option
    {
        #region Public Constructors

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

        public Select2Data(IEnumerable<Select2Group<OptionType>> Groups)
        {
            Results = Groups ?? Array.Empty<Select2Group<OptionType>>();
        }

        public Select2Data(IEnumerable<Select2Group<OptionType>> Groups, bool PaginationMore)
        {
            Results = Groups ?? Array.Empty<Select2Group<OptionType>>();
            Pagination.More = PaginationMore;
        }

        #endregion Public Constructors

        #region Public Properties

        public Pagination Pagination { get; set; } = new Pagination();
        public IEnumerable<ISelect2Result> Results { get; set; } = Array.Empty<ISelect2Result>();

        #endregion Public Properties
    }

    public class Select2Group<OptionType> : ISelect2Result where OptionType : ISelect2Option
    {
        #region Public Constructors

        public Select2Group(string Text) => this.Text = Text;

        public Select2Group(string Text, IEnumerable<OptionType> Children) : this(Text) => this.Children = (Children ?? Array.Empty<OptionType>());

        public Select2Group()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<OptionType> Children { get; set; } = Array.Empty<OptionType>();
        public string Text { get; set; }

        #endregion Public Properties
    }

    public sealed class Select2Option : ISelect2Option, IComparable<ISelect2Option>, IComparable
    {
        #region Public Constructors

        public Select2Option()
        {
        }

        public Select2Option(string Text, string Value)
        {
            ID = Value.IfBlank(Text);
            this.Text = Text.IfBlank(ID);
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Disabled { get; set; }
        public string ID { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }

        #endregion Public Properties

        #region Public Methods

        public int CompareTo(object obj) => ID.CompareTo(obj?.ToString());

        public int CompareTo(ISelect2Option other) => ID.CompareTo(other.ID);



        public override string ToString() => $"<option value='{ID}'{Disabled.AsIf(" disabled")}{Selected.AsIf(" selected")}>{Text}</option>";

        #endregion Public Methods
    }

    public interface ISelect2Option : ISelect2Result
    {
        #region Public Properties

        bool Disabled { get; set; }
        string ID { get; set; }
        bool Selected { get; set; }

        #endregion Public Properties
    }

    public interface ISelect2Result
    {
        #region Public Properties

        string Text { get; set; }

        #endregion Public Properties
    }
}