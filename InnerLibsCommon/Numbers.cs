using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Extensions
{
    public static partial class Utils
    {
        /// <summary>
        /// Determines whether the length of the specified object equals the given value.
        /// </summary>
        /// <typeparam name="T">The type of the object to evaluate.</typeparam>
        /// <typeparam name="R">The type of the value to compare, which must implement <see cref="IConvertible"/>, <see
        /// cref="IComparable"/>, and <see cref="IFormattable"/>.</typeparam>
        /// <param name="number">The object whose length is to be evaluated.</param>
        /// <param name="equalValue">The value to compare against the length of the object.</param>
        /// <returns><see langword="true"/> if the length of <paramref name="number"/> equals <paramref name="equalValue"/>;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool Length<T, R>(this T number,R equalValue) where R : struct, IConvertible, IComparable, IFormattable
            => number.Length<T, R>().Equals(equalValue);

        /// <summary>
        /// Returns the length of the number, string, array or collection.
        /// if is a numeric type, returns the number of digits.
        /// if is a string, returns the length of the string.
        /// if is an array or collection, returns the number of elements.
        /// any other type, tries to find a property or method named Length or Count that returns an numeric type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R">A numeric type</typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static R Length<T, R>(this T number) where R : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(R).IsNumericType())
            {
                throw new InvalidOperationException($"R must be a numeric type: {PredefinedArrays.NumericTypes.SelectJoinString(x => x.Name, ", ")}");
            }

            if (number == null)
            {
                return 0.ChangeType<R>();
            }

            if (number is string str)
            {
                return str.Length.ChangeType<R>();
            }

            if (number.IsNumericType())
            {
                return number.ChangeType<string>().Length.ChangeType<R>();
            }


            if (number is System.Collections.IList list)
            {
                return list.Count.ChangeType<R>();
            }

            if (number is System.Collections.ICollection collection)
            {
                return collection.Count.ChangeType<R>();
            }

            if (number is Array array)
            {
                return array.Length.ChangeType<R>();
            }

            if (number is System.Collections.IEnumerable enumerable)
            {
                return enumerable.Cast<object>().Count().ChangeType<R>();
            }

            if (number is System.Data.DataTable dt)
            {
                return dt.Rows.Count.ChangeType<R>();
            }

            if (number is System.Data.DataSet ds)
            {
                return ds.Tables.Count.ChangeType<R>();
            }

            if (number is System.Data.DataRow dr)
            {
                return dr.ItemArray.Length.ChangeType<R>();
            }

            if (number is System.Text.StringBuilder sb)
            {
                return sb.Length.ChangeType<R>();
            }



            var names = new string[] { "Length", "Count", "LongLength", "LongCount" };
            names = names.Union(names.Select(x => $"Get{x}")).ToArray();

            var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.Name.FlatContains(names) && p.GetIndexParameters().Length == 0 && p.PropertyType.IsNumericType())
                .OrderByWithPriority(
                  p => p.PropertyType == typeof(R),
                  p => p.PropertyType.IsNumericType(),
                  p => p.Name.Equals("Length"),
                  p => p.Name.FlatEqual("Length")
                );

            var methods = typeof(T).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => names.Contains(m.Name) && m.GetParameters().Length == 0 && m.ReturnType.IsNumericType())
                .OrderByWithPriority(
                  p => p.ReturnType == typeof(R),
                  p => p.ReturnType.IsNumericType(),
                  p => p.Name.Equals("Length"),
                  p => p.Name.FlatEqual("Length")
                )
                .ThenByLike(x => x.Name, true,names)
                ;


            // check if type has a property int Length
            foreach (var p in properties)
            {
                if (p != null && (p.PropertyType == typeof(R) || p.PropertyType.IsNumericType()))
                {
                    var v = p.GetValue(number);
                    Util.WriteDebug(v, $"Using Property {p.Name}");
                    return p.GetValue(number).ChangeType<R>();
                }
            }

            // check if type has a method to get length
            foreach (var p in methods)
            {

                if (p != null && (p.ReturnType == typeof(R) || p.ReturnType.IsNumericType()))
                {
                    var v = p.Invoke(number, null);
                    Util.WriteDebug(v, $"Using Method {p.Name}");
                    return v.ChangeType<R>();  
                }
            }
                     

            throw new InvalidOperationException($"{typeof(T).Name} is not a string, numeric, array or collection type or does not have any property or function to return the length (${names.JoinString(", ")}).");

        }

        /// <summary>
        /// Return the string representation of the number using invariant culture.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToStringInvariant(this int number) => number.ToString(CultureInfo.InvariantCulture);

        /// <inheritdoc cref="ToStringInvariant(int)"/>
        public static string ToStringInvariant(this long number) => number.ToString(CultureInfo.InvariantCulture);

        /// <inheritdoc cref="ToStringInvariant(int)"/>
        public static string ToStringInvariant(this double number) => number.ToString(CultureInfo.InvariantCulture);

        /// <inheritdoc cref="ToStringInvariant(int)"/>
        public static string ToStringInvariant(this decimal number) => number.ToString(CultureInfo.InvariantCulture);

        /// <inheritdoc cref="ToStringInvariant(int)"/>
        public static string ToStringInvariant(this short number) => number.ToString(CultureInfo.InvariantCulture);


    }
}