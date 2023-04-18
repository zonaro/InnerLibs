// CNPJInfo myDeserializedClass = JsonConvert.DeserializeObject<CNPJInfo>(myJsonResponse);
using System;
using System.Collections.Generic;

namespace Extensions.BR
{
    public class AtividadePrincipal
    {
        public string Id { get; set; }
        public string Secao { get; set; }
        public string Divisao { get; set; }
        public string Grupo { get; set; }
        public string Classe { get; set; }
        public string Subclasse { get; set; }
        public string Descricao { get; set; }
    }

    public class AtividadeSecundariaInfo
    {
        public string Id { get; set; }
        public string Secao { get; set; }
        public string Divisao { get; set; }
        public string Grupo { get; set; }
        public string Classe { get; set; }
        public string Subclasse { get; set; }
        public string Descricao { get; set; }
    }

    public class EstabelecimentoInfo : Locations.AddressInfo
    {
        public string Cnpj { get => this[nameof(Cnpj)]?.NullIf(x => !x.CNPJValido()); set => this[nameof(Cnpj)] = value; }
        public List<AtividadeSecundariaInfo> AtividadesSecundarias { get; set; }
        public string CnpjRaiz { get => this[nameof(CnpjRaiz)]; set => this[nameof(CnpjRaiz)] = value; }
        public string CnpjOrdem { get => this[nameof(CnpjOrdem)]; set => this[nameof(CnpjOrdem)] = value; }
        public string CnpjDigitoVerificador { get => this[nameof(CnpjDigitoVerificador)]; set => this[nameof(CnpjDigitoVerificador)] = value; }
        public string Tipo { get => this[nameof(Tipo)]; set => this[nameof(Tipo)] = value; }
        public string NomeFantasia { get => this[nameof(NomeFantasia)]; set => this[nameof(NomeFantasia)] = value; }
        public string SituacaoCadastral { get => this[nameof(SituacaoCadastral)]; set => this[nameof(SituacaoCadastral)] = value; }
        public string DataSituacaoCadastral { get => this[nameof(DataSituacaoCadastral)]; set => this[nameof(DataSituacaoCadastral)] = value; }
        public string DataInicioAtividade { get => this[nameof(DataInicioAtividade)]; set => this[nameof(DataInicioAtividade)] = value; }
        public string NomeCidadeExterior { get => this[nameof(NomeCidadeExterior)]; set => this[nameof(NomeCidadeExterior)] = value; }
        public string TipoLogradouro { get => this[nameof(TipoLogradouro)]; set => this[nameof(TipoLogradouro)] = value; }
        public string Logradouro { get => Street; set => Street = value; }
        public string Numero { get => Number; set => Number = value; }
        public string Complemento { get => Complement; set => Complement = value; }
        public string Bairro { get => Neighborhood; set => Neighborhood = value; }
        public string Cep { get => ZipCode; set => ZipCode = value; }
        public string Ddd1 { get => this[nameof(Ddd1)]; set => this[nameof(Ddd1)] = value; }
        public string Telefone1 { get => this[nameof(Telefone1)]; set => this[nameof(Telefone1)] = value; }
        public string Ddd2 { get => this[nameof(Ddd2)]; set => this[nameof(Ddd2)] = value; }
        public string Telefone2 { get => this[nameof(Telefone2)]; set => this[nameof(Telefone2)] = value; }
        public string DddFax { get => this[nameof(DddFax)]; set => this[nameof(DddFax)] = value; }
        public string Fax { get => this[nameof(Fax)]; set => this[nameof(Fax)] = value; }
        public string Email { get => this[nameof(Email)]; set => this[nameof(Email)] = value; }
        public string SituacaoEspecial { get => this[nameof(SituacaoEspecial)]; set => this[nameof(SituacaoEspecial)] = value; }
        public DateTime? DataSituacaoEspecial { get => this[nameof(DataSituacaoEspecial)]?.ToDateTime(); set => this[nameof(DataSituacaoEspecial)] = value?.ToSQLDateString(); }
        public DateTime? AtualizadoEm { get => this[nameof(AtualizadoEm)]?.ToDateTime(); set => this[nameof(AtualizadoEm)] = value?.ToSQLDateString(); }
        public AtividadePrincipal AtividadePrincipal { get; set; }
        public Pais Pais { get; set; }
        public Estado Estado => Cidade.Estado;
        public Cidade Cidade { get; set; }
        public string MotivoSituacaoCadastral { get => this[nameof(MotivoSituacaoCadastral)]; set => this[nameof(MotivoSituacaoCadastral)] = value; }
        public List<InscricaoEstadualInfo> InscricoesEstaduais { get; set; }
    }

    public class InscricaoEstadualInfo
    {
        public string InscricaoEstadual { get; set; }
        public bool? Ativo { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public Estado Estado { get; set; }
    }

    public class NaturezaJuridica
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
    }

    public class Pais
    {
        public string Id { get; set; }
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Nome { get; set; }
        public string ComexId { get; set; }
    }

    public class DescricaoInfo
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
    }

    public class CNPJInfo
    {
        public static CNPJInfo Consultar(string CNPJ)
        {
            if (CNPJ.CNPJValido())
            {
                dynamic x = new Uri("https://publica.cnpj.ws/cnpj/" + CNPJ.RemoveMask()).DownloadJson();

                var info = new CNPJInfo();
                info.CnpjRaiz = x[nameof(CnpjRaiz)] as string;
                info.RazaoSocial = x[nameof(RazaoSocial)] as string;
                info.CapitalSocial = x[nameof(CapitalSocial)] as string;
                info.ResponsavelFederativo = x[nameof(ResponsavelFederativo)] as string;
                info.AtualizadoEm = x[nameof(AtualizadoEm)] as DateTime?;
                info.Porte = x[nameof(Porte)] as DescricaoInfo;
                info.NaturezaJuridica = x[nameof(NaturezaJuridica)] as NaturezaJuridica;
                info.QualificacaoDoResponsavel = x[nameof(QualificacaoDoResponsavel)] as DescricaoInfo;
                info.Socios = x[nameof(Socios)] as List<SocioInfo>;
                info.Simples = x[nameof(Simples)] as SimplesInfo;
                info.Estabelecimento = x[nameof(Estabelecimento)] as EstabelecimentoInfo;
                return info;
            }
            else throw new ArgumentException("CNPJ Inválido", nameof(CNPJ));

        }

        public string CnpjRaiz { get; set; }
        public string RazaoSocial { get; set; }
        public string CapitalSocial { get; set; }
        public string ResponsavelFederativo { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public DescricaoInfo Porte { get; set; }
        public NaturezaJuridica NaturezaJuridica { get; set; }
        public DescricaoInfo QualificacaoDoResponsavel { get; set; }
        public List<SocioInfo> Socios { get; set; }
        public SimplesInfo Simples { get; set; }
        public EstabelecimentoInfo Estabelecimento { get; set; }
    }

    public class SimplesInfo
    {
        public string Simples { get; set; }
        public string DataOpcaoSimples { get; set; }
        public DateTime? DataExclusaoSimples { get; set; }
        public string Mei { get; set; }
        public DateTime? DataOpcaoMei { get; set; }
        public DateTime? DataExclusaoMei { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }

    public class SocioInfo
    {
        public string CpfCnpjSocio { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string DataEntrada { get; set; }
        public string CpfRepresentanteLegal { get; set; }
        public string NomeRepresentante { get; set; }
        public string FaixaEtaria { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public string PaisId { get; set; }
        public DescricaoInfo QualificacaoSocio { get; set; }
        public DescricaoInfo QualificacaoRepresentante { get; set; }
    }
}