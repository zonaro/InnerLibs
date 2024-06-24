using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Extensions
{
    public class Helper

    {
        #region Public Methods

        public static long AutoConv(object value, JSONParameters param)
        {
            if (value is string s)
            {
                if (param.AutoConvertStringToNumbers == true)
                {
                    return CreateLong(s, 0, s.Length);
                }
                else
                    throw new Exception($"AutoConvertStringToNumbers is disabled for converting vstring : {value}");
            }
            else return value is long vlong ? vlong : Convert.ToInt64(value);
        }


        public static DateTime CreateDateTime(string value, bool UseUTCDateTime)
        {
            if (value.Length < 19)
                return DateTime.MinValue;

            bool utc = false;
            // 0123456789012345678 9012 9/3 datetime format = yyyy-MM-ddTHH:mm:ss .nnn Z
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;
            int ms = 0;

            year = CreateInteger(value, 0, 4);
            month = CreateInteger(value, 5, 2);
            day = CreateInteger(value, 8, 2);
            hour = CreateInteger(value, 11, 2);
            min = CreateInteger(value, 14, 2);
            sec = CreateInteger(value, 17, 2);
            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);

            if (value[value.Length - 1] == 'Z')
                utc = true;

            if (UseUTCDateTime == false && utc == false)
                return new DateTime(year, month, day, hour, min, sec, ms);
            else
                return new DateTime(year, month, day, hour, min, sec, ms, DateTimeKind.Utc).ToLocalTime();
        }

        public static DateTimeOffset CreateDateTimeOffset(int year, int month, int day, int hour, int min, int sec, int milli, int extraTicks, TimeSpan offset)
        {
            var dt = new DateTimeOffset(year, month, day, hour, min, sec, milli, offset);

            if (extraTicks > 0)
                dt += TimeSpan.FromTicks(extraTicks);

            return dt;
        }

        public static object CreateDateTimeOffset(string value)
        {
            // 0123456789012345678 9012 9/3 0/4 1/5 datetime format = yyyy-MM-ddTHH:mm:ss .nnn _ + 00:00

            // ISO8601 roundtrip formats have 7 digits for ticks, and no space before the '+'
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnnnnnn + 00:00 datetime format =
            // yyyy-MM-ddTHH:mm:ss .nnnnnnn Z

            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;
            int ms = 0;
            int usTicks = 0; // ticks for xxx.x microseconds

            year = CreateInteger(value, 0, 4);
            month = CreateInteger(value, 5, 2);
            day = CreateInteger(value, 8, 2);
            hour = CreateInteger(value, 11, 2);
            min = CreateInteger(value, 14, 2);
            sec = CreateInteger(value, 17, 2);

            int p = 20;

            if (value.Length > 21 && value[19] == '.')
            {
                ms = CreateInteger(value, p, 3);
                p = 23;

                // handle 7 digit case
                if (value.Length > 25 && char.IsDigit(value[p]))
                {
                    usTicks = CreateInteger(value, p, 4);
                    p = 27;
                }
            }

            if (value[p] == 'Z')
                // UTC
                return CreateDateTimeOffset(year, month, day, hour, min, sec, ms, usTicks, TimeSpan.Zero);

            if (value[p] == ' ')
                ++p;

            // +00:00
            int th = CreateInteger(value, p + 1, 2);
            int tm = CreateInteger(value, p + 1 + 2 + 1, 2);
            if (value[p] == '-')
                th = -th;

            return CreateDateTimeOffset(year, month, day, hour, min, sec, ms, usTicks, new TimeSpan(th, tm, 0));
        }

        public static object CreateEnum(Type pt, object v) => Util.GetEnumValue($"{v}", pt);

        public static Guid CreateGuid(string s) => s.IsValid() && s?.Length > 30 ? new Guid(s) : new Guid(Convert.FromBase64String(s));

        public static unsafe int CreateInteger(string s, int index, int count)
        {
            int num = 0;
            int neg = 1;
            fixed (char* v = s)
            {
                char* str = v;
                str += index;
                if (*str == '-')
                {
                    neg = -1;
                    str++;
                    count--;
                }
                if (*str == '+')
                {
                    str++;
                    count--;
                }
                while (count > 0)
                {
                    num = num * 10
                        //(num << 4) - (num << 2) - (num << 1)
                        + (*str - '0');
                    str++;
                    count--;
                }
            }
            return num * neg;
        }

        public static unsafe long CreateLong(string s, int index, int count)
        {
            long num = 0;
            int neg = 1;
            fixed (char* v = s)
            {
                char* str = v;
                str += index;
                if (*str == '-')
                {
                    neg = -1;
                    str++;
                    count--;
                }
                if (*str == '+')
                {
                    str++;
                    count--;
                }
                while (count > 0)
                {
                    num = num * 10
                       //(num << 4) - (num << 2) - (num << 1)
                       + (*str - '0');
                    str++;
                    count--;
                }
            }
            return num * neg;
        }

        public static unsafe long CreateLong(char[] s, int index, int count)
        {
            long num = 0;
            int neg = 1;
            fixed (char* v = s)
            {
                char* str = v;
                str += index;
                if (*str == '-')
                {
                    neg = -1;
                    str++;
                    count--;
                }
                if (*str == '+')
                {
                    str++;
                    count--;
                }
                while (count > 0)
                {
                    num = num * 10
                        //(num << 4) - (num << 2) - (num << 1)
                        + (*str - '0');
                    str++;
                    count--;
                }
            }
            return num * neg;
        }

        public static NameValueCollection CreateNV(Dictionary<string, object> d)
        {

            NameValueCollection nv = new NameValueCollection();
            if (d != null)
                foreach (var o in d)
                    nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        public static StringDictionary CreateSD(Dictionary<string, object> d)
        {
            StringDictionary nv = new StringDictionary();

            if (d != null)
                foreach (var o in d)
                    nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        public static bool IsNullable(Type t)
        {
            if (!t.IsGenericType) return false;
            Type g = t.GetGenericTypeDefinition();
            return (g.Equals(typeof(Nullable<>)));
        }

        public static Type UnderlyingTypeOf(Type t) => Reflection.Instance.GetGenericArguments(t)[0];

        #endregion Public Methods
    }
}