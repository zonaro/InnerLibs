using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Extensions;
using Extensions.BR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class CssClassAttribute : Attribute
    {
        public string ClassNames { get; }
        public CssClassAttribute(string classNames)
        {
            ClassNames = classNames;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IdadeAttribute : ValidationAttribute
    {
        public int Minima { get; }

        public IdadeAttribute(int minima)
        {
            Minima = minima;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("Idade Minima: {Idade}").Inject(new
            {
                Minima,
                Idade = Minima,
                n = Minima,
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MultipleValidationsAttribute<T> : ValidationAttribute
    {
        public List<Expression<Func<T, string>>> Validations { get; } = new List<Expression<Func<T, string>>>();

        public bool AllowNull { get; set; } = false;
        public int MinimalValid { get; set; } = 1;

        public string ErrorSeparator { get; set; } = ", ";


        public bool AllMustBeValid
        {
            get => MinimalValid == Validations.Count;
            set
            {
                if (value) MinimalValid = Validations.Count;
                if (MinimalValid < 1) MinimalValid = 1;
            }
        }

        internal (bool IsValid, IEnumerable<string> Errors) GetValidationErrors(object value) => Util.Validate(value.ChangeType<T>(), AllowNull, Validations.ToArray());

        private IEnumerable<string> Errors = Enumerable.Empty<string>();

        public override bool IsValid(object value)
        {
            bool isValid = false;
            (isValid, Errors) = GetValidationErrors(value);
            return isValid;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} needs {MinimalValid} of {Validations} but got {ErrorCount}:{newLine}{Errors}")
            .Inject(new
            {
                name,
                MinimalValid,
                n = MinimalValid,
                Validations = Validations.Count,
                ErrorSeparator,
                ErrorCount = Errors.Count(),
                Errors = this.Errors.JoinString(ErrorSeparator.IfBlank(", ")),
                NewLine = Environment.NewLine,
            },
            CaseSensitive: false);
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : ValidationAttribute
    {
        public PasswordLevel Level { get; set; } = PasswordLevel.Strong;

        public override bool IsValid(object value) => Util.ValidatePassword(value?.ToString(), Level);

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é uma senha válida").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CPFAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || value.ToString().IsBlank() || Brasil.CPFValido(value.ToString());

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um CPF válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CNPJAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value is string s && (s.IsBlank() || Brasil.CNPJValido(s));

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um CNPJ válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CPFouCNPJAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || (value.ToString().IsBlank() || Brasil.CPFouCNPJValido(value.ToString()));

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um CPF ou CNPJ válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CNHAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || (value.ToString().IsBlank() || Brasil.CNHValido(value.ToString()));

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é uma CNH válida").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TelefoneAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value == null || value.ToString().IsBlank() || Brasil.TelefoneValido(value.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um telefone válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CEPAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || (value.ToString().IsBlank() || Brasil.CEPValido(value.ToString()));

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um CEP válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IbgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || Brasil.IBGEValido(value?.ToString() ?? string.Empty);
        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um código de cidade ou estado válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CidadeIbgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || Brasil.CidadeIBGEValido(value?.ToString() ?? string.Empty);
        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um código de cidade válido").Inject(new
            {
                name
            });
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EstadoIbgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) => value == null || Brasil.EstadoIBGEValido(value?.ToString() ?? string.Empty);

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage.IfBlank("{name} não é um código de estado válido").Inject(new
            {
                name
            });
        }
    }
 

}