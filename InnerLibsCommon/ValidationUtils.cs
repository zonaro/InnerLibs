using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extensions
{
    public partial class Util
    {

        /// <summary>
        /// Check if object is not a valid value by performing the following checks:
        /// <list type="bullet">
        ///     <item>
        ///         <term>null</term> - for reference or nullable types;
        ///     </item>
        ///     <item>
        ///         <term>zero</term> - for any numeric type;
        ///     </item>
        ///     <item>
        ///         <term>false</term> - for boolean type;
        ///     </item>
        ///     <item>
        ///         <term>empty or whitespace</term> - for string or char types;
        ///     </item>
        ///     <item>
        ///         <term>MinValue</term> - for DateTime, TimeSpan, or DateTimeOffset types;
        ///     </item>
        ///     <item>
        ///         <term>Empty</term> - for Guid type;
        ///     </item>
        ///     <item>
        ///         <term>all items invalid</term> - for collections and dictionaries;
        ///     </item>
        ///     <item>
        ///         <term>validation failure</term> - for complex objects using data annotations.
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="Value">The object to validate</param>
        /// <returns>
        /// <c>true</c> if the object is considered invalid according to the validation rules;
        /// otherwise, <c>false</c> if the object has a valid value
        /// </returns>
        /// <remarks>
        /// This method is the inverse of <see cref="IsValid(object)"/>. For collections and dictionaries,
        /// the method returns <c>true</c> only if all contained items are invalid. For complex objects,
        /// it uses data annotation validation attributes to determine validity.
        /// </remarks>
        /// <example>
        /// <code>
        /// string emptyString = "";
        /// int zero = 0;
        /// bool falseBool = false;
        /// DateTime minDate = DateTime.MinValue;
        /// List&lt;int&gt; emptyList = new List&lt;int&gt;();
        ///
        /// bool result1 = emptyString.IsNotValid(); // returns true
        /// bool result2 = zero.IsNotValid(); // returns true
        /// bool result3 = falseBool.IsNotValid(); // returns true
        /// bool result4 = minDate.IsNotValid(); // returns true
        /// bool result5 = emptyList.IsNotValid(); // returns true
        /// </code>
        /// </example>
        public static bool IsNotValid(this object Value)
        {
            try
            {
                if (Value != null)
                {
                    var tipo = Value.GetNullableTypeOf();
                    if (tipo.IsNumericType())
                    {
                        return Value.ChangeType<decimal>() == 0;
                    }
                    else if (Value is FormattableString fs)
                    {
                        return IsNotValid($"{fs}".ToUpperInvariant());
                    }
                    else if (Value is bool b)
                    {
                        return !b;
                    }
                    else if (Value is string s)
                    {
                        return string.IsNullOrWhiteSpace($"{s}".RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));
                    }
                    else if (Value is char c)
                    {
                        return string.IsNullOrWhiteSpace($"{c}".RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));
                    }
                    else if (Value is DateTime time)
                    {
                        return time.Equals(DateTime.MinValue);
                    }
                    else if (Value is TimeSpan span)
                    {
                        return span.Equals(TimeSpan.MinValue);
                    }
                    else if (Value is DateTimeOffset off)
                    {
                        return off.Equals(DateTimeOffset.MinValue);
                    }
                    else if (Value is Guid guid)
                    {
                        return guid == Guid.Empty;
                    }
                    else if (Value is IDictionary dic)
                    {
                        foreach (DictionaryEntry item in dic)
                        {
                            if (item.Value.IsValid())
                            {
                                return false;
                            }
                        }
                    }
                    else if (Value.IsEnumerableNotString() && Value is IEnumerable enumerable)
                    {
                        foreach (object item in enumerable)
                        {
                            if (item.IsValid())
                            {
                                return false;
                            }
                        }
                    }
                    else if (tipo.IsClass && !tipo.IsSimpleType())
                    {
                        /// check if object has any validation attributes and validate them                    
                        var props = tipo.GetMembers();
                        if (props.Any())
                        {
                            /// check if properties or fields have validation attributes, if true apply validation
                            if (props.Any(x => x.GetCustomAttributes(typeof(ValidationAttribute), true).Any()))
                            {
                                var context = new ValidationContext(Value);
                                var results = new List<ValidationResult>();
                                bool isValid = Validator.TryValidateObject(Value, context, results, validateAllProperties: true);
                                return !isValid;
                            }
                            /// check if object has properties
                            if (props.Any(x => x is PropertyInfo))
                            {
                                // check all properties recursively
                                foreach (var prop in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                                {
                                    var val = prop.GetValue(Value);
                                    if (val.IsValid())
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                            else if (props.Any(x => x is FieldInfo))
                            {
                                // check all fields recursively
                                foreach (var field in tipo.GetFields(BindingFlags.Public | BindingFlags.Instance))
                                {
                                    var val = field.GetValue(Value);
                                    if (val.IsValid())
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebug(ex);
            }
            return true;
        }

        /// <summary>
        /// Checks if the <paramref name="Value"/> is valid.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsValid(this object Value) => !IsNotValid(Value);

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