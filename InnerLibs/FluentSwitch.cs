using System;
using System.Collections.Generic;

namespace InnerLibs
{
    public static class FluentSwitchExt
    {
        public static FluentSwitch<T1, T2> Switch<T2, T1>(this T1 Input)
        {
            return new FluentSwitch<T1, T2>(Input);
        }

        public static T2 Switch<T2, T1>(this T1 Input, Action<FluentSwitch<T1, T2>> Test)
        {
            var a = Input.Switch<T2, T1>();
            Test(a);
            return a.GetValue();
        }
    }

    public class FluentSwitch<T1, T2>
    {
        private Dictionary<T1, T2> dic = new Dictionary<T1, T2>();
        private T1 input = default;
        private T2 defaultv = default;

        public FluentSwitch(T1 Input, T2 DefaultValue = default)
        {
            input = Input;
            defaultv = DefaultValue;
        }

        public FluentSwitch<T1, T2> Case(T1 Value, T2 ReturnValue)
        {
            return Case(new[] { Value }, ReturnValue);
        }

        public FluentSwitch<T1, T2> Case(IEnumerable<T1> Values, T2 ReturnValue)
        {
            foreach (T1 item in Values)
                dic.Set(item, ReturnValue);
            return this;
        }

        public FluentSwitch<T1, T2> Default(T2 ReturnValue)
        {
            defaultv = ReturnValue;
            return this;
        }

        public T2 GetValue()
        {
            return dic.GetValueOr(input, defaultv);
        }

        public static implicit operator T2(FluentSwitch<T1, T2> FS)
        {
            if (FS is null)
                return default;
            return FS.GetValue();
        }
    }
}