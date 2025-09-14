using System.ComponentModel.DataAnnotations;
using Extensions;
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
