using System.ComponentModel.DataAnnotations;
using Extensions;
using Extensions.BR;


namespace Extensions
{

    public class CPFAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || (value.ToString().IsBlank() || Brasil.CPFValido(value.ToString()));

        public override string FormatErrorMessage(string name) => $"{name} não é um CPF válido";
    }

    public class CNPJAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value is string s && (s.IsBlank() || Brasil.CNPJValido(s));

        public override string FormatErrorMessage(string name) => $"{name} não é um CNPJ válido";
    }

    public class CPFouCNPJAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || (value.ToString().IsBlank() || Brasil.CPFouCNPJValido(value.ToString()));

        public override string FormatErrorMessage(string name) => $"{name} não é um CPF ou CNPJ válido";
    }

    public class CNHAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || (value.ToString().IsBlank() || Brasil.CNHValido(value.ToString()));

        public override string FormatErrorMessage(string name) => $"{name} não é uma CNH válida";
    }

    public class TelefoneAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || (value.ToString().IsBlank() || Brasil.TelefoneValido(value.ToString()));

        public override string FormatErrorMessage(string name) => $"{name} não é um telefone válido";
    }


    public class IbgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || Brasil.IBGEValido(value?.ToString() ?? string.Empty);
        public override string FormatErrorMessage(string name) => $"{name} não é um código de cidade válido";
    }

    public class CidadeIbgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || Brasil.CidadeIBGEValido(value?.ToString() ?? string.Empty);
        public override string FormatErrorMessage(string name) => $"{name} não é um código de cidade válido";
    }

    public class EstadoIbgeAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value == null || Brasil.EstadoIBGEValido(value?.ToString() ?? string.Empty);
        public override string FormatErrorMessage(string name) => $"{name} não é um código de estado válido";
    }


}
