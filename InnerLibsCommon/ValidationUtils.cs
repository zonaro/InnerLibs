using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Extensions;

namespace Extensions
{
    public partial class Util
    {


        /// <summary>
        /// Change a string expression that returns an error message into a boolean expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Test"></param>
        /// <param name="defaultReturnValue"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> ErrorExpressionToBoolean<T>(this Expression<Func<T, string>> Test, bool defaultReturnValue = false)
        {
            if (Test == null) return x => defaultReturnValue;
            return x => Test.Compile().Invoke(x).AsBool(defaultReturnValue);
        }

        /// <summary>
        /// Evaluates a set of validation tests on the specified object and returns any validation error messages.
        /// </summary>
        /// <remarks>This method allows you to define custom validation rules as expressions and apply them to an
        /// object. Only non-blank error messages are included in the result.</remarks>
        /// <typeparam name="T">The type of the object to validate.</typeparam>
        /// <param name="Value">The object to validate.</param>
        /// <param name="Tests">An array of validation expressions, where each expression represents a validation rule. Each expression should
        /// return a validation error message as a string if the rule is violated, or <see langword="null"/> or an empty string
        /// if the rule passes.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of strings containing the validation error messages for the object. If all
        /// validation rules pass, the returned collection will be empty.</returns>
        public static IEnumerable<string> ValidationErrors<T>(this T Value, bool AllowNull, params Expression<Func<T, string>>[] Tests)
        {
            if (Value == null && AllowNull == false)
            {
                yield return $"{Util.GetTypeOf(Value)} value is null";
            }
            else
            {
                Tests = Tests ?? Array.Empty<Expression<Func<T, string>>>();
                foreach (var item in Tests)
                {
                    if (item.Compile().Invoke(Value) is string s)
                    {
                        //if string is null or blank, no errors
                        bool? IsValidProp = s.AsNullableBool(false);

                        if (s.IsBlank() || IsValidProp == true) continue;

                        if (IsValidProp == false)
                        {
                            yield return $"{GetDisplayString(item, "Property")} value is not valid: {s}";
                        }
                        else
                        {
                            // if IsValidProp is null, the string is considered an error message
                            yield return s;
                        }
                    }
                }
            }
        }

        public static (bool IsValid, IEnumerable<string> Errors) Validate<T>(this T Value, params Expression<Func<T, string>>[] Tests) => Validate(Value, false, -1, Tests);

        public static (bool IsValid, IEnumerable<string> Errors) Validate<T>(this T Value, bool AllowNull, params Expression<Func<T, string>>[] Tests) => Validate(Value, AllowNull, -1, Tests);

        public static (bool IsValid, IEnumerable<string> Errors) Validate<T>(this T Value, int Minimal, params Expression<Func<T, string>>[] Tests) => Validate(Value, false, Minimal, Tests);

        public static (bool IsValid, IEnumerable<string> Errors) Validate<T>(this T Value, bool AllowNull, int Minimal, params Expression<Func<T, string>>[] Tests)
        {
            var errors = ValidationErrors(Value, AllowNull, Tests);
            if (Minimal < 0) Minimal = Tests.Length;
            Minimal = Minimal.LimitRange(0, Tests.Length);
            return (errors.Count() < Minimal, errors);
        }


        public static T ValidateOr<T>(this T Value, T defaultValue, int Minimal, params Expression<Func<T, string>>[] Tests) => ValidateOr(Value, defaultValue, false, Minimal, Tests);
        public static T ValidateOr<T>(this T Value, T defaultValue, bool AllowNull, params Expression<Func<T, string>>[] Tests) => ValidateOr(Value, defaultValue, AllowNull, -1, Tests);
        public static T ValidateOr<T>(this T Value, T defaultValue, params Expression<Func<T, string>>[] Tests) => ValidateOr(Value, defaultValue, typeof(T).IsNullableType(), -1, Tests);
        public static T ValidateOr<T>(this T Value, T defaultValue, bool AllowNull, int Minimal, params Expression<Func<T, string>>[] Tests)
        {
            var (IsValid, Errors) = Validate(Value, AllowNull, Minimal, Tests);
            if (IsValid) return Value;
            return defaultValue;
        }

        public static bool ValidatePassword(this string Password, PasswordLevel PasswordLevel = PasswordLevel.Strong) => PasswordLevel == PasswordLevel.None || Password.CheckPassword().ToInt() >= PasswordLevel.ToInt();
    }
}