using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Extensions.BR;


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
    public class MultiEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || value.ToString().IsBlank())
            {
                return true;
            }
            
            return value.ToString().IsMultiEmail();
        }


    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidPathAttribute : ValidationAttribute
    {


        public bool AllowFilePath { get; set; }
        public bool AllowDirectoryPath { get; set; }

        public bool ValidateIfExists { get; set; }



        public override string FormatErrorMessage(string name)
        {

            if (AllowFilePath && AllowDirectoryPath)
            {
                if (ValidateIfExists)
                    return ErrorMessage.IfBlank("{name} deve ser um caminho de arquivo ou diretório existente").Inject(new { name });
                else
                    return ErrorMessage.IfBlank("{name} deve ser um caminho de arquivo ou diretório válido").Inject(new { name });
            }
            else if (AllowFilePath)
            {
                if (ValidateIfExists)
                    return ErrorMessage.IfBlank("{name} deve ser um caminho de arquivo existente").Inject(new { name });
                else
                    return ErrorMessage.IfBlank("{name} deve ser um caminho de arquivo válido").Inject(new { name });
            }
            else if (AllowDirectoryPath)
            {
                if (ValidateIfExists)
                    return ErrorMessage.IfBlank("{name} deve ser um caminho de diretório existente").Inject(new { name });
                else
                    return ErrorMessage.IfBlank("{name} deve ser um caminho de diretório válido").Inject(new { name });
            }
            else
            {
                return base.FormatErrorMessage(name);
            }
        }


        public override bool IsValid(object value)
        {
            if (value == null || value.ToString().IsBlank())
                return true;

            var s = value.ChangeType<string>();


            if (AllowFilePath && s.IsFilePath())
            {
                if (ValidateIfExists)
                {
                    return System.IO.File.Exists(s);
                }
                return true;
            }

            if (AllowDirectoryPath && s.IsDirectoryPath())
            {
                if (ValidateIfExists)
                {
                    return System.IO.Directory.Exists(s);
                }
                return true;
            }

            return false;
        }


    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IdadeAttribute : ValidationAttribute
    {
        public int Minima { get; }
        public int Maxima { get; }

        public IdadeAttribute(int? minima, int? maxima = null)
        {

            (minima, maxima) = Util.FixOrder(minima ?? 0, maxima ?? int.MaxValue);

            if (minima == null && maxima == null)
                throw new ArgumentException("Deve ser informado o valor mínimo ou máximo");

            if (minima < 0)
                throw new ArgumentOutOfRangeException(nameof(minima), "A idade mínima não pode ser negativa");

            if (maxima < 0)
                throw new ArgumentOutOfRangeException(nameof(maxima), "A idade máxima não pode ser negativa");



            Minima = minima.Value;
            Maxima = maxima.Value;
        }

        public override bool IsValid(object value)
        {
            if (value == null || value.ToString().IsBlank())
                return true;

            var age = 0;

            if (value is string str && DateTime.TryParse(str, out var dt))
            {
                age = Util.GetAge(dt);
            }
            else if (value.IsNumber())
            {
                age = value.ToDecimal().RoundInt();
            }
            else if (value is DateTime dt2)
            {
                age = Util.GetAge(dt2);
            }
            else
            {
                age = value.ChangeType<decimal>().RoundInt();
            }

            return age.IsBetweenOrEqual(Minima, Maxima);
        }

        public override string FormatErrorMessage(string name)
        {

            if (Minima != 0 && Maxima != int.MaxValue)
                return ErrorMessage.IfBlank("Idade entre {Minima} e {Maxima}").Inject(new
                {
                    Minima,
                    IdadeMin = Minima,
                    nMin = Minima,
                    Maxima,
                    IdadeMax = Maxima,
                    nMax = Maxima,
                    name
                });

            if (Maxima != int.MaxValue)
                return ErrorMessage.IfBlank("Idade máxima: {Idade}").Inject(new
                {
                    Maxima,
                    Idade = Maxima,
                    n = Maxima,
                    name
                });

            if (Minima != 0)
                return ErrorMessage.IfBlank("Idade mínima: {Idade}").Inject(new
                {
                    Minima,
                    Idade = Minima,
                    n = Minima,
                    name
                });

            return base.FormatErrorMessage(name);
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class MultipleValidationsAttribute : ValidationAttribute
    {
        public List<Expression<Func<object, string>>> Validations { get; } = new List<Expression<Func<object, string>>>();

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

        internal (bool IsValid, IEnumerable<string> Errors) GetValidationErrors(object value) => Util.Validate(value, AllowNull, Validations.ToArray());

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