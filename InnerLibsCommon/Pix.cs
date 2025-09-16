using System;
using System.Security.Cryptography;
using System.Text;
using Extensions;


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
        /// Gera um payload para um QR Code de pagamento via Pix, conforme o padrão EMV.
        /// </summary>
        /// <param name="chavePix"></param>
        /// <param name="nomeRecebedor"></param>
        /// <param name="cidadeRecebedor"></param>
        /// <param name="valor"></param>
        /// <param name="txId"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GerarPayloadPIX(
               string chavePix,
               string nomeRecebedor,
               string cidadeRecebedor,
               decimal? valor = null,       // opcional
               string txId = null,           // opcional
               string descricao = null       // opcional (não é usado no QR estático oficial, mas pode ser incluído no TXID)
           )
        {

            // ======== Validações e formatações ========

            if (!Brasil.ChavePIXValida(chavePix)) throw new ArgumentException("A chave Pix inválida");

            /// somente CPF ou CNPJ devem ser sem mascara
            if (Brasil.CPFouCNPJValido(chavePix)) chavePix = chavePix.RemoveMask();

            if (chavePix.Length > 77) throw new ArgumentException("A chave Pix não pode ter mais que 77 caracteres.");

            if (nomeRecebedor.IsBlank()) throw new ArgumentException("O nome do recebedor é obrigatório");

            nomeRecebedor = nomeRecebedor.TrimBetween().RemoveAccents().GetFirstChars(25).ToUpperInvariant();

            if (cidadeRecebedor.IsBlank()) throw new ArgumentException("A cidade do recebedor é obrigatória");

            cidadeRecebedor = cidadeRecebedor?.Trim().RemoveAccents().GetFirstChars(25).ToUpperInvariant();

            if (Brasil.CidadeIBGEValido(cidadeRecebedor))
            {
                cidadeRecebedor = Brasil.PegarCidade(cidadeRecebedor).Nome;
            }

            if (txId.IsNotBlank() && txId?.Length > 25)
                throw new ArgumentException("O TXID não pode ter mais que 25 caracteres.");

            descricao = descricao?.TrimBetween().GetFirstChars(99);

            string MontarCampo(string id, string valorCampo)
            {
                return id + valorCampo.Length.ToString("00") + valorCampo;
            }

            // 00 - Payload Format Indicator
            string payload = MontarCampo("00", "01");

            // 26 - Merchant Account Information
            string gui = MontarCampo("00", "br.gov.bcb.pix");
            string chave = MontarCampo("01", chavePix);

            // Se quiser incluir descrição no campo 26 (não obrigatório)
            string descricaoCampo = string.Empty;
            if (descricao.IsNotBlank())
                descricaoCampo = MontarCampo("02", descricao);

            string merchantAccountInfo = MontarCampo("26", gui + chave + descricaoCampo);
            payload += merchantAccountInfo;

            // 52 - Merchant Category Code
            payload += MontarCampo("52", "0000");

            // 53 - Transaction Currency (986 = BRL)
            payload += MontarCampo("53", "986");

            // 54 - Transaction Amount (opcional)
            if (valor.HasValue && valor.Value > 0)
                payload += MontarCampo("54", valor.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));

            // 58 - Country Code
            payload += MontarCampo("58", "BR");

            // 59 - Nome do recebedor
            payload += MontarCampo("59", nomeRecebedor);

            // 60 - Cidade do recebedor
            payload += MontarCampo("60", cidadeRecebedor);

            // 62 - Additional Data Field Template (TXID opcional)
            if (txId.IsNotBlank())
            {
                string txidField = MontarCampo("05", txId);
                payload += MontarCampo("62", txidField);
            }

            // 63 - CRC16
            payload += "6304";
            payload += CalcularCRC16(payload);

            return payload;
        }

        private static string CalcularCRC16(string input)
        {
            ushort polynomial = 0x1021;
            ushort crc = 0xFFFF;

            byte[] bytes = Encoding.ASCII.GetBytes(input);
            foreach (byte b in bytes)
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
    }
}
