using System;
using System.Collections.Generic;
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

        public static string GerarPayloadPIX(
             string chavePix,
             string nomeRecebedor,
             int cidadeRecebedor,
             decimal? valor = null,       // opcional
             string txId = null,           // opcional
             string descricao = null       // opcional
         ) => GerarPayloadPIX(
             chavePix,
             nomeRecebedor,
             cidadeRecebedor.ToString(),
             valor,
             txId,
             descricao
         );

        /// <summary>
        /// Gera um payload para um QR Code de pagamento via Pix, conforme o padrão EMV e regulamentação do Banco Central.
        /// </summary>
        /// <param name="chavePix">Chave PIX do recebedor (CPF, CNPJ, email, telefone ou chave aleatória)</param>
        /// <param name="nomeRecebedor">Nome do recebedor (máximo 25 caracteres)</param>
        /// <param name="cidadeRecebedor">Cidade do recebedor (máximo 15 caracteres)</param>
        /// <param name="valor">Valor da transação (opcional)</param>
        /// <param name="txId">Identificador da transação (máximo 25 caracteres, opcional)</param>
        /// <param name="descricao">Descrição adicional (máximo 72 caracteres, opcional)</param>
        /// <returns>String do payload PIX para QR Code</returns>
        /// <exception cref="ArgumentException">Quando os parâmetros são inválidos</exception>
        public static string GerarPayloadPIX(
               string chavePix,
               string nomeRecebedor,
               string cidadeRecebedor,
               decimal? valor = null,       // opcional
               string txId = null,           // opcional
               string descricao = null       // opcional
           )
        {
            // ======== Validações e formatações ========

            if (!Brasil.ChavePIXValida(chavePix))
                throw new ArgumentException("A chave PIX é inválida");

            // Somente CPF ou CNPJ devem ser sem máscara
            if (Brasil.CPFouCNPJValido(chavePix))
                chavePix = chavePix.RemoveMask();

            if (chavePix.Length > 77)
                throw new ArgumentException("A chave PIX não pode ter mais que 77 caracteres.");

            if (nomeRecebedor.IsBlank())
                throw new ArgumentException("O nome do recebedor é obrigatório");

            // Nome do recebedor: máximo 25 caracteres, sem acentos, maiúsculo
            nomeRecebedor = nomeRecebedor.TrimBetween().RemoveAccents().GetFirstChars(25).ToUpperInvariant();

            if (cidadeRecebedor.IsBlank())
                throw new ArgumentException("A cidade do recebedor é obrigatória");

            if (Brasil.CidadeIBGEValido(cidadeRecebedor))
            {
                cidadeRecebedor = Brasil.PegarCidade(cidadeRecebedor).Nome;
            }

            // Cidade: máximo 15 caracteres conforme padrão do BC
            cidadeRecebedor = cidadeRecebedor?.Trim().RemoveAccents().GetFirstChars(15).ToUpperInvariant();

            if (txId.IsNotBlank() && txId?.Length > 25)
                throw new ArgumentException("O TXID não pode ter mais que 25 caracteres.");

            // Descrição: máximo 72 caracteres conforme padrão
            if (descricao.IsNotBlank() && descricao?.Length > 72)
                throw new ArgumentException("A descrição não pode ter mais que 72 caracteres.");

            descricao = descricao?.TrimBetween().GetFirstChars(72);

            // Função auxiliar para montar campos EMV
            string MontarCampo(string id, string valorCampo)
            {
                if (valorCampo.IsBlank()) return string.Empty;
                return id + valorCampo.Length.ToString("00") + valorCampo;
            }

            // ======== Construção do Payload EMV ========

            string payload = string.Empty;

            // 00 - Payload Format Indicator (obrigatório)
            payload += MontarCampo("00", "01");

            // 01 - Point of Initiation Method (opcional - para QR reutilizável)
            if (!valor.HasValue || valor.Value <= 0)
            {
                payload += MontarCampo("01", "12"); // QR Code reutilizável
            }

            // 26 - Merchant Account Information (obrigatório)
            string gui = MontarCampo("00", "br.gov.bcb.pix");
            string chave = MontarCampo("01", chavePix);

            // Campo 02 para descrição dentro do MAI (opcional)
            string descricaoMAI = string.Empty;
            if (descricao.IsNotBlank())
                descricaoMAI = MontarCampo("02", descricao);

            string merchantAccountInfo = MontarCampo("26", gui + chave + descricaoMAI);
            payload += merchantAccountInfo;

            // 52 - Merchant Category Code (obrigatório)
            payload += MontarCampo("52", "0000");

            // 53 - Transaction Currency (obrigatório - 986 = BRL)
            payload += MontarCampo("53", "986");

            // 54 - Transaction Amount (opcional)
            if (valor.HasValue && valor.Value > 0)
            {
                string valorFormatado = valor.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                payload += MontarCampo("54", valorFormatado);
            }

            // 58 - Country Code (obrigatório)
            payload += MontarCampo("58", "BR");

            // 59 - Merchant Name (obrigatório)
            payload += MontarCampo("59", nomeRecebedor);

            // 60 - Merchant City (obrigatório)
            payload += MontarCampo("60", cidadeRecebedor);

            // 62 - Additional Data Field Template (opcional)
            if (txId.IsNotBlank())
            {
                string txidField = MontarCampo("05", txId);
                payload += MontarCampo("62", txidField);
            }

            // 63 - CRC16 (obrigatório)
            payload += "6304";
            string crc = CalcularCRC16(payload);
            payload += crc;

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

        /// <summary>
        /// Decodifica um payload PIX 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<string, object> LerPayloadPIX(string payload)
        {
            if (payload.IsBlank() || !payload.StartsWith("000201"))
                throw new ArgumentException("Payload inválido");
            var result = new Dictionary<string, object>();
            int index = 0;

            while (index < payload.Length)
            {
                if (index + 4 > payload.Length)
                    break;
                string id = payload.Substring(index, 2);
                int length = int.Parse(payload.Substring(index + 2, 2));
                if (index + 4 + length > payload.Length)
                    break;
                string value = payload.Substring(index + 4, length);
                index += 4 + length;
                // Verifica se o ID é um campo composto
                if (id == "26" || id == "62")
                {
                    var subFields = new Dictionary<string, string>();
                    int subIndex = 0;
                    while (subIndex < value.Length)
                    {
                        if (subIndex + 4 > value.Length)
                            break;
                        string subId = value.Substring(subIndex, 2);
                        int subLength = int.Parse(value.Substring(subIndex + 2, 2));
                        if (subIndex + 4 + subLength > value.Length)
                            break;
                        string subValue = value.Substring(subIndex + 4, subLength);
                        subFields[subId] = subValue;
                        subIndex += 4 + subLength;
                    }
                    result[id] = subFields;
                }
                else
                {
                    result[id] = value;
                }
            }
            return result;
        }
    }
}