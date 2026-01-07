using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;


namespace Extensions.BR
{
    public static partial class Brasil
    {
        public static bool ChavePIXValida(this string chave)
        {
            return chave.IsEmail() ||
                chave.CPFouCNPJValido() ||
                chave.TelefoneValido() ||
                (chave.IsGuid());
        }

        public static string LimparChavePIX(this string Chave)
        {
            if (Chave.ChavePIXValida())
            {
                if (Chave.CPFValido()) return Chave.FormatarCPF().RemoveMask();
                else if (Chave.CNPJValido()) return Chave.FormatarCNPJ().RemoveMask();
                else if (Chave.TelefoneValido()) return Chave.FormatarTelefone().RemoveMask();
                else return Chave.ToLower();
            }
            return "";
        }

           public static string FormatarChavePIX(this string Chave)
        {
            if (Chave.ChavePIXValida())
            {
                if (Chave.CPFValido()) return Chave.FormatarCPF();
                else if (Chave.CNPJValido()) return Chave.FormatarCNPJ();
                else if (Chave.TelefoneValido()) return Chave.FormatarTelefone();
                else return Chave.ToLower();
            }
            return "";
        }

        public static string FormatarChavePIXComNome(this string Chave, string Nome, string Label = "Chave PIX")
        {
            if (Chave.ChavePIXValida())
            {
                if (Chave.IsEmail())

                    Label += " (Email)";
                else if (Chave.CPFValido()) Label += " (CPF)";
                else if (Chave.CNPJValido()) Label += " (CNPJ)";
                else if (Chave.TelefoneValido()) Label += " (Telefone)";
                else if (Chave.IsGuid()) Label += " (Chave Aleatória)";
                else
                {
                    Label += " (Inválida)";
                    return $"{Util.SelectJoinString("-", Nome, Label)} :{"-".Repeat(Chave.Length)}";
                }
            }
            if (Nome.IsNotBlank())
                return $"{Nome} - {Label} :{Chave.FormatarChavePIX()}";
            else
                return $"{Label} :{Chave.FormatarChavePIX()}";
        }


        /// <summary>
        /// Gera o payload do Pix (BR Code) seguindo a mesma lógica do script.js (EMVCo/BR Code).
        /// </summary>
        /// <param name="chave">Chave PIX (email, CPF/CNPJ, telefone ou chave aleatória)</param>
        /// <param name="nome">Nome do recebedor (até 25 caracteres). Será normalizado (remoção de acentos e maiúsculas).</param>
        /// <param name="cidade">Cidade do recebedor (até 15 caracteres). Será normalizada (remoção de acentos e maiúsculas).</param>
        /// <param name="txid">Identificador da transação (opcional). Se em branco será gerado automaticamente (22 chars) e incluído no payload (compatível com Itaú).</param>
        /// <param name="valor">Valor opcional (decimal) - ex: 12.34 (será formatado com 2 casas decimais)</param>
        /// <param name="descricao">Descrição opcional que vai no Merchant Account Information</param>
        /// <param name="mcc">Merchant Category Code (MCC). Use a enumeração <see cref="PixMcc"/> (padrão: PersonalUse / 0000).</param>
        /// <returns>Payload pronto para geração de QR code (inclui CRC)</returns>
        public static string GerarPayloadPIX(string chave, string nome, string cidade, string txid = null, decimal? valor = null, string descricao = null, PixMcc mcc = PixMcc.PersonalUse)
        {
            const string gui = "BR.GOV.BCB.PIX";  

            if (chave.IsBlank()) throw new ArgumentNullException(nameof(chave));

           
            var cleanedKey = LimparChavePIX(chave);
            if (cleanedKey.Length == 0) throw new ArgumentException("Chave PIX inválida", nameof(chave));

            nome = Util.NormalizeText(nome ?? string.Empty);

            cidade = Brasil.PegarCidade(cidade)?.Nome ?? string.Empty;

            cidade = Util.NormalizeText(cidade);

            if (nome.Length > 25) nome = nome.GetFirstChars(25);
            if (cidade.Length > 15) cidade = cidade.GetFirstChars(15);

            if (descricao.IsNotBlank()) descricao = Util.NormalizeText(descricao).GetFirstChars(72);

            if (txid.IsNotBlank()) txid = Util.NormalizeText(txid).GetFirstChars(25);
            else txid = GenerateTxId();

            var payload = new StringBuilder();

            // Payload Format Indicator (00)
            payload.Append(MakeTLV("00", "01"));

            // Merchant Account Information (ID 26) with subfields (00=GUI, 01=Chave, 02=Descrição)
            var mai = new StringBuilder();
            mai.Append(MakeTLV("00", gui));
            mai.Append(MakeTLV("01", cleanedKey));
            if (descricao.IsNotBlank()) mai.Append(MakeTLV("02", descricao));

            payload.Append(MakeTLV("26", mai.ToString()));

            // Merchant Category Code (MCC)
            payload.Append(MakeTLV("52", ((int)mcc).ToString("D4")));

            // Transaction Currency (986 = BRL)
            payload.Append(MakeTLV("53", "986"));

            // Transaction Amount - opcional (apenas se > 0, para casar com script.js)
            if (valor.HasValue && valor.Value > 0m)
                payload.Append(MakeTLV("54", valor.Value.ToString("0.00", CultureInfo.InvariantCulture)));

            // Country
            payload.Append(MakeTLV("58", "BR"));

            // Merchant name and city
            payload.Append(MakeTLV("59", nome));
            payload.Append(MakeTLV("60", cidade));

            // Additional Data Field Template (62) - sempre incluir subfield 05 = txid (script.js exige)
            var add = MakeTLV("05", txid);
            payload.Append(MakeTLV("62", add));

            // CRC - calcular sobre todo o payload até o campo 63 (inclusive Tag e Length)
            var crcInput = payload.ToString() + "6304";
            var crc = ComputeCRC16(crcInput);
            payload.Append(MakeTLV("63", crc));

            return payload.ToString();
        }

        /// <summary>
        /// Constrói TLV (Tag + Length 2 dígitos + Value)
        /// </summary>
        private static string MakeTLV(string tag, string value)
        {
            if (value == null) value = string.Empty;
            return $"{tag}{value.Length:D2}{value}";
        }


        /// <summary>
        /// Calcula CRC16-CCITT (0x1021) conforme especificação (valor em hex 4 caracteres maiúsculos)
        /// </summary>
        private static string ComputeCRC16(string input)
        {
            var bytes = Encoding.ASCII.GetBytes(input);
            const ushort polynomial = 0x1021;
            ushort crc = 0xFFFF;

            foreach (var b in bytes)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ polynomial);
                    else
                        crc <<= 1;
                }
            }

            return crc.ToString("X4");
        }

         

        /// <summary>
        /// Gera um txid no estilo do script.js: timestamp em base36 + random base36, em maiúsculas, 22 chars, preenchido com '0' se menor
        /// </summary>
        private static string GenerateTxId()
        {
            var timestamp = Util.ToBase36(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var random = Util.RandomBase36(13);
            var tx = (timestamp + random).ToUpperInvariant();
            if (tx.Length > 22) tx = tx.Substring(0, 22);
            else tx = tx.PadRight(22, '0');
            return tx;
        }


    }

    public enum PixMcc
    {
        /// <summary>
        /// Uso pessoal / transação sem categoria comercial.
        /// </summary>
        PersonalUse = 0000,

        /// <summary>
        /// Categoria genérica — alta compatibilidade entre bancos.
        /// </summary>
        Generic = 9999,

        /// <summary>
        /// Serviços financeiros, bancos, fintechs.
        /// </summary>
        FinancialServices = 6012,

        /// <summary>
        /// Transporte e serviços relacionados.
        /// </summary>
        Transportation = 4111,

        /// <summary>
        /// Serviços diversos — categoria ampla e aceita.
        /// </summary>
        MiscellaneousServices = 7399
    }

}