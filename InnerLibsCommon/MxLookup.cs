using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Extensions
{
    public static partial class Util
    {
        public static List<string> GetMxRecords(this string domain)
        {
            domain = domain.GetDomain();
            var mxRecords = new List<string>();

            // Servidor DNS público (Google)
            var dnsServer = "8.8.8.8";
            var port = 53;

            using (var udp = new UdpClient())
            {
                udp.Connect(dnsServer, port);

                // Monta consulta DNS
                var query = BuildDnsQuery(domain);
                udp.Send(query, query.Length);

                var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var response = udp.Receive(ref remoteEP);

                // Interpreta resposta
                ParseMxResponse(response, mxRecords);
            }

            return mxRecords;
        }

        private static byte[] BuildDnsQuery(string domain)
        {
            var rand = new Random();
            ushort id = (ushort)rand.Next(ushort.MaxValue);

            var header = new byte[12];
            header[0] = (byte)(id >> 8);
            header[1] = (byte)(id & 0xFF);
            header[2] = 0x01; // Recursion desired
            header[5] = 0x01; // QDCOUNT = 1

            var qname = new List<byte>();
            foreach (var part in domain.Split('.'))
            {
                qname.Add((byte)part.Length);
                qname.AddRange(Encoding.ASCII.GetBytes(part));
            }
            qname.Add(0); // fim do nome

            var qtype = new byte[] { 0x00, 0x0F }; // MX = 15
            var qclass = new byte[] { 0x00, 0x01 }; // IN

            var query = new List<byte>();
            query.AddRange(header);
            query.AddRange(qname);
            query.AddRange(qtype);
            query.AddRange(qclass);

            return query.ToArray();
        }

        private static void ParseMxResponse(byte[] response, List<string> mxRecords)
        {
            int pos = 12; // pula cabeçalho
            SkipName(response, ref pos); // pula QNAME
            pos += 4; // QTYPE + QCLASS

            while (pos < response.Length)
            {
                SkipName(response, ref pos); // NAME
                ushort type = (ushort)((response[pos] << 8) | response[pos + 1]);
                pos += 2;
                pos += 2; // CLASS
                pos += 4; // TTL
                ushort rdlength = (ushort)((response[pos] << 8) | response[pos + 1]);
                pos += 2;

                if (type == 15) // MX
                {
                    ushort preference = (ushort)((response[pos] << 8) | response[pos + 1]);
                    pos += 2;
                    string exchange = ReadName(response, ref pos);
                    mxRecords.Add($"{preference} {exchange}");
                }
                else
                {
                    pos += rdlength;
                }
            }
        }

        private static void SkipName(byte[] message, ref int pos)
        {
            while (true)
            {
                byte len = message[pos++];
                if (len == 0) break;
                if ((len & 0xC0) == 0xC0)
                {
                    pos++;
                    break;
                }
                pos += len;
            }
        }

        private static string ReadName(byte[] message, ref int pos)
        {
            var sb = new StringBuilder();
            while (true)
            {
                byte len = message[pos++];
                if (len == 0) break;
                if ((len & 0xC0) == 0xC0)
                {
                    int offset = ((len & 0x3F) << 8) | message[pos++];
                    int savedPos = pos;
                    pos = offset;
                    sb.Append(ReadName(message, ref pos));
                    pos = savedPos;
                    break;
                }
                sb.Append(Encoding.ASCII.GetString(message, pos, len));
                pos += len;
                sb.Append('.');
            }
            if (sb.Length > 0 && sb[sb.Length - 1] == '.')
                sb.Length--;
            return sb.ToString();
        }
    }
}