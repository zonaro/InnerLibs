
Imports System.IO
Imports System.Text

Namespace HtmlParser
    ''' <summary>
    ''' HTML 4 Entity coding routines
    ''' </summary>
    Friend MustInherit Class HtmlEncoder
        Public Shared Function EncodeValue(value As String) As String
            Dim output As New StringBuilder()
            Dim reader As New StringReader(value)
            Dim c As Integer = reader.Read()
            While c <> -1
                Select Case (c)
                    Case Asc("<"c)
                        output.Append("&lt;")
                        Exit Select
                    Case Asc(">"c)
                        output.Append("&gt;")
                        Exit Select
                    Case Asc(""""c)
                        output.Append("&quot;")
                        Exit Select
                    Case Asc("&"c)
                        output.Append("&amp;")
                        Exit Select
                    Case 193
                        output.Append("&Aacute;")
                        Exit Select
                    Case 225
                        output.Append("&aacute;")
                        Exit Select
                    Case 194
                        output.Append("&Acirc;")
                        Exit Select
                    Case 226
                        output.Append("&acirc;")
                        Exit Select
                    Case 180
                        output.Append("&acute;")
                        Exit Select
                    Case 198
                        output.Append("&AElig;")
                        Exit Select
                    Case 230
                        output.Append("&aelig;")
                        Exit Select
                    Case 192
                        output.Append("&Agrave;")
                        Exit Select
                    Case 224
                        output.Append("&agrave;")
                        Exit Select
                    Case 8501
                        output.Append("&alefsym;")
                        Exit Select
                    Case 913
                        output.Append("&Alpha;")
                        Exit Select
                    Case 945
                        output.Append("&alpha;")
                        Exit Select

                    Case 8743
                        output.Append("&and;")
                        Exit Select
                    Case 8736
                        output.Append("&ang;")
                        Exit Select
                    Case 197
                        output.Append("&Aring;")
                        Exit Select
                    Case 229
                        output.Append("&aring;")
                        Exit Select
                    Case 8776
                        output.Append("&asymp;")
                        Exit Select
                    Case 195
                        output.Append("&Atilde;")
                        Exit Select
                    Case 227
                        output.Append("&atilde;")
                        Exit Select
                    Case 196
                        output.Append("&Auml;")
                        Exit Select
                    Case 228
                        output.Append("&auml;")
                        Exit Select
                    Case 8222
                        output.Append("&bdquo;")
                        Exit Select
                    Case 914
                        output.Append("&Beta;")
                        Exit Select
                    Case 946
                        output.Append("&beta;")
                        Exit Select
                    Case 166
                        output.Append("&brvbar;")
                        Exit Select
                    Case 8226
                        output.Append("&bull;")
                        Exit Select
                    Case 8745
                        output.Append("&cap;")
                        Exit Select
                    Case 199
                        output.Append("&Ccedil;")
                        Exit Select
                    Case 231
                        output.Append("&ccedil;")
                        Exit Select
                    Case 184
                        output.Append("&cedil;")
                        Exit Select
                    Case 162
                        output.Append("&cent;")
                        Exit Select
                    Case 935
                        output.Append("&Chi;")
                        Exit Select
                    Case 967
                        output.Append("&chi;")
                        Exit Select
                    Case 710
                        output.Append("&circ;")
                        Exit Select
                    Case 9827
                        output.Append("&clubs;")
                        Exit Select
                    Case 8773
                        output.Append("&cong;")
                        Exit Select
                    Case 169
                        output.Append("&copy;")
                        Exit Select
                    Case 8629
                        output.Append("&crarr;")
                        Exit Select
                    Case 8746
                        output.Append("&cup;")
                        Exit Select
                    Case 164
                        output.Append("&curren;")
                        Exit Select
                    Case 8224
                        output.Append("&dagger;")
                        Exit Select
                    Case 8225
                        output.Append("&Dagger;")
                        Exit Select
                    Case 8595
                        output.Append("&darr;")
                        Exit Select
                    Case 8659
                        output.Append("&dArr;")
                        Exit Select
                    Case 176
                        output.Append("&deg;")
                        Exit Select
                    Case 916
                        output.Append("&Delta;")
                        Exit Select
                    Case 948
                        output.Append("&delta;")
                        Exit Select
                    Case 9830
                        output.Append("&diams;")
                        Exit Select
                    Case 247
                        output.Append("&divide;")
                        Exit Select
                    Case 201
                        output.Append("&Eacute;")
                        Exit Select
                    Case 233
                        output.Append("&eacute;")
                        Exit Select
                    Case 202
                        output.Append("&Ecirc;")
                        Exit Select
                    Case 234
                        output.Append("&ecirc;")
                        Exit Select

                    Case 200
                        output.Append("&Egrave;")
                        Exit Select
                    Case 232
                        output.Append("&egrave;")
                        Exit Select
                    Case 8709
                        output.Append("&empty;")
                        Exit Select
                    Case 8195
                        output.Append("&emsp;")
                        Exit Select
                    Case 917
                        output.Append("&Epsilon;")
                        Exit Select
                    Case 949
                        output.Append("&epsilon;")
                        Exit Select
                    Case 8801
                        output.Append("&equiv;")
                        Exit Select
                    Case 919
                        output.Append("&Eta;")
                        Exit Select
                    Case 951
                        output.Append("&eta;")
                        Exit Select
                    Case 208
                        output.Append("&ETH;")
                        Exit Select
                    Case 240
                        output.Append("&eth;")
                        Exit Select
                    Case 203
                        output.Append("&Euml;")
                        Exit Select
                    Case 235
                        output.Append("&euml;")
                        Exit Select
                    Case 128
                        output.Append("&euro;")
                        Exit Select
                    Case 8707
                        output.Append("&exist;")
                        Exit Select
                    Case 402
                        output.Append("&fnof;")
                        Exit Select
                    Case 8704
                        output.Append("&forall;")
                        Exit Select
                    Case 189
                        output.Append("&frac12;")
                        Exit Select
                    Case 188
                        output.Append("&frac14;")
                        Exit Select
                    Case 190
                        output.Append("&frac34;")
                        Exit Select
                    Case 8260
                        output.Append("&fras1;")
                        Exit Select
                    Case 915
                        output.Append("&Gamma;")
                        Exit Select
                    Case 947
                        output.Append("&gamma;")
                        Exit Select
                    Case 8805
                        output.Append("&ge;")
                        Exit Select
                    Case 8596
                        output.Append("&harr;")
                        Exit Select
                    Case 8660
                        output.Append("&hArr;")
                        Exit Select
                    Case 9829
                        output.Append("&hearts;")
                        Exit Select
                    Case 8230
                        output.Append("&hellip;")
                        Exit Select
                    Case 205
                        output.Append("&Iacute;")
                        Exit Select
                    Case 237
                        output.Append("&iacute;")
                        Exit Select
                    Case 206
                        output.Append("&Icirc;")
                        Exit Select
                    Case 238
                        output.Append("&icirc;")
                        Exit Select
                    Case 161
                        output.Append("&iexcl;")
                        Exit Select
                    Case 204
                        output.Append("&Igrave;")
                        Exit Select
                    Case 236
                        output.Append("&igrave;")
                        Exit Select
                    Case 8465
                        output.Append("&image;")
                        Exit Select
                    Case 8734
                        output.Append("&infin;")
                        Exit Select
                    Case 8747
                        output.Append("&int;")
                        Exit Select
                    Case 921
                        output.Append("&Iota;")
                        Exit Select

                    Case 953
                        output.Append("&iota;")
                        Exit Select
                    Case 191
                        output.Append("&iquest;")
                        Exit Select
                    Case 8712
                        output.Append("&isin;")
                        Exit Select
                    Case 207
                        output.Append("&Iuml;")
                        Exit Select
                    Case 239
                        output.Append("&iuml;")
                        Exit Select
                    Case 922
                        output.Append("&Kappa;")
                        Exit Select
                    Case 954
                        output.Append("&kappa;")
                        Exit Select
                    Case 923
                        output.Append("&Lambda;")
                        Exit Select
                    Case 955
                        output.Append("&lambda;")
                        Exit Select
                    Case 9001
                        output.Append("&lang;")
                        Exit Select
                    Case 171
                        output.Append("&laquo;")
                        Exit Select
                    Case 8592
                        output.Append("&larr;")
                        Exit Select
                    Case 8656
                        output.Append("&lArr;")
                        Exit Select
                    Case 8968
                        output.Append("&lceil;")
                        Exit Select
                    Case 8220
                        output.Append("&ldquo;")
                        Exit Select
                    Case 8804
                        output.Append("&le;")
                        Exit Select
                    Case 8970
                        output.Append("&lfloor;")
                        Exit Select
                    Case 8727
                        output.Append("&lowast;")
                        Exit Select
                    Case 9674
                        output.Append("&loz;")
                        Exit Select
                    Case 8206
                        output.Append("&lrm;")
                        Exit Select
                    Case 8249
                        output.Append("&lsaquo;")
                        Exit Select
                    Case 8216
                        output.Append("&lsquo;")
                        Exit Select
                    Case 175
                        output.Append("&macr;")
                        Exit Select
                    Case 8212
                        output.Append("&mdash;")
                        Exit Select
                    Case 181
                        output.Append("&micro;")
                        Exit Select
                    Case 183
                        output.Append("&middot;")
                        Exit Select
                    Case 8722
                        output.Append("&minus;")
                        Exit Select
                    Case 924
                        output.Append("&Mu;")
                        Exit Select
                    Case 956
                        output.Append("&mu;")
                        Exit Select
                    Case 8711
                        output.Append("&nabla;")
                        Exit Select
                    Case 160
                        output.Append("&nbsp;")
                        Exit Select
                    Case 8211
                        output.Append("&ndash;")
                        Exit Select
                    Case 8800
                        output.Append("&ne;")
                        Exit Select
                    Case 8715
                        output.Append("&ni;")
                        Exit Select
                    Case 172
                        output.Append("&not;")
                        Exit Select
                    Case 8713
                        output.Append("&notin;")
                        Exit Select
                    Case 8836
                        output.Append("&nsub;")
                        Exit Select
                    Case 209
                        output.Append("&Ntilde;")
                        Exit Select
                    Case 241
                        output.Append("&ntilde;")
                        Exit Select
                    Case 925
                        output.Append("&Nu;")
                        Exit Select

                    Case 957
                        output.Append("&nu;")
                        Exit Select
                    Case 211
                        output.Append("&Oacute;")
                        Exit Select
                    Case 243
                        output.Append("&oacute;")
                        Exit Select
                    Case 212
                        output.Append("&Ocirc;")
                        Exit Select
                    Case 244
                        output.Append("&ocirc;")
                        Exit Select
                    Case 338
                        output.Append("&OElig;")
                        Exit Select
                    Case 339
                        output.Append("&oelig;")
                        Exit Select
                    Case 210
                        output.Append("&Ograve;")
                        Exit Select
                    Case 242
                        output.Append("&ograve;")
                        Exit Select
                    Case 8254
                        output.Append("&oline;")
                        Exit Select
                    Case 937
                        output.Append("&Omega;")
                        Exit Select
                    Case 969
                        output.Append("&omega;")
                        Exit Select
                    Case 927
                        output.Append("&Omicron;")
                        Exit Select
                    Case 959
                        output.Append("&omicron;")
                        Exit Select
                    Case 8853
                        output.Append("&oplus;")
                        Exit Select
                    Case 8744
                        output.Append("&or;")
                        Exit Select
                    Case 170
                        output.Append("&ordf;")
                        Exit Select
                    Case 186
                        output.Append("&ordm;")
                        Exit Select
                    Case 216
                        output.Append("&Oslash;")
                        Exit Select
                    Case 248
                        output.Append("&oslash;")
                        Exit Select
                    Case 213
                        output.Append("&Otilde;")
                        Exit Select
                    Case 245
                        output.Append("&otilde;")
                        Exit Select
                    Case 8855
                        output.Append("&otimes;")
                        Exit Select
                    Case 214
                        output.Append("&Ouml;")
                        Exit Select
                    Case 246
                        output.Append("&ouml;")
                        Exit Select
                    Case 182
                        output.Append("&para;")
                        Exit Select
                    Case 8706
                        output.Append("&part;")
                        Exit Select
                    Case 8240
                        output.Append("&permil;")
                        Exit Select
                    Case 8869
                        output.Append("&perp;")
                        Exit Select
                    Case 934
                        output.Append("&Phi;")
                        Exit Select
                    Case 966
                        output.Append("&phi;")
                        Exit Select
                    Case 928
                        output.Append("&Pi;")
                        Exit Select
                    Case 960
                        output.Append("&pi;")
                        Exit Select
                    Case 982
                        output.Append("&piv;")
                        Exit Select
                    Case 177
                        output.Append("&plusmn;")
                        Exit Select
                    Case 163
                        output.Append("&pound;")
                        Exit Select
                    Case 8242
                        output.Append("&prime;")
                        Exit Select
                    Case 8243
                        output.Append("&Prime;")
                        Exit Select
                    Case 8719
                        output.Append("&prod;")
                        Exit Select
                    Case 8733
                        output.Append("&prop;")
                        Exit Select
                    Case 936
                        output.Append("&Psi;")
                        Exit Select

                    Case 968
                        output.Append("&psi;")
                        Exit Select
                    Case 8730
                        output.Append("&radic;")
                        Exit Select
                    Case 9002
                        output.Append("&rang;")
                        Exit Select
                    Case 187
                        output.Append("&raquo;")
                        Exit Select
                    Case 8594
                        output.Append("&rarr;")
                        Exit Select
                    Case 8658
                        output.Append("&rArr;")
                        Exit Select
                    Case 8969
                        output.Append("&rceil;")
                        Exit Select
                    Case 8221
                        output.Append("&rdquo;")
                        Exit Select
                    Case 8476
                        output.Append("&real;")
                        Exit Select
                    Case 174
                        output.Append("&reg;")
                        Exit Select
                    Case 8971
                        output.Append("&rfloor;")
                        Exit Select
                    Case 929
                        output.Append("&Rho;")
                        Exit Select
                    Case 961
                        output.Append("&rho;")
                        Exit Select
                    Case 8207
                        output.Append("&rlm;")
                        Exit Select
                    Case 8250
                        output.Append("&rsaquo;")
                        Exit Select
                    Case 8217
                        output.Append("&rsquo;")
                        Exit Select
                    Case 8218
                        output.Append("&sbquo;")
                        Exit Select
                    Case 352
                        output.Append("&Scaron;")
                        Exit Select
                    Case 353
                        output.Append("&scaron;")
                        Exit Select
                    Case 8901
                        output.Append("&sdot;")
                        Exit Select
                    Case 167
                        output.Append("&sect;")
                        Exit Select
                    Case 173
                        output.Append("&shy;")
                        Exit Select
                    Case 931
                        output.Append("&Sigma;")
                        Exit Select
                    Case 963
                        output.Append("&sigma;")
                        Exit Select
                    Case 962
                        output.Append("&sigmaf;")
                        Exit Select
                    Case 8764
                        output.Append("&sim;")
                        Exit Select
                    Case 9824
                        output.Append("&spades;")
                        Exit Select
                    Case 8834
                        output.Append("&sub;")
                        Exit Select
                    Case 8838
                        output.Append("&sube;")
                        Exit Select
                    Case 8721
                        output.Append("&sum;")
                        Exit Select
                    Case 8835
                        output.Append("&sup;")
                        Exit Select
                    Case 185
                        output.Append("&sup1;")
                        Exit Select
                    Case 178
                        output.Append("&sup2;")
                        Exit Select
                    Case 179
                        output.Append("&sup3;")
                        Exit Select
                    Case 8839
                        output.Append("&supe;")
                        Exit Select
                    Case 223
                        output.Append("&szlig;")
                        Exit Select
                    Case 932
                        output.Append("&Tau;")
                        Exit Select
                    Case 964
                        output.Append("&tau;")
                        Exit Select
                    Case 8756
                        output.Append("&there4;")
                        Exit Select
                    Case 920
                        output.Append("&Theta;")
                        Exit Select

                    Case 952
                        output.Append("&theta;")
                        Exit Select
                    Case 977
                        output.Append("&thetasym;")
                        Exit Select
                    Case 8201
                        output.Append("&thinsp;")
                        Exit Select
                    Case 222
                        output.Append("&THORN;")
                        Exit Select
                    Case 254
                        output.Append("&thorn;")
                        Exit Select
                    Case 732
                        output.Append("&tilde;")
                        Exit Select
                    Case 215
                        output.Append("&times;")
                        Exit Select
                    Case 8482
                        output.Append("&trade;")
                        Exit Select
                    Case 218
                        output.Append("&Uacute;")
                        Exit Select
                    Case 250
                        output.Append("&uacute;")
                        Exit Select
                    Case 8593
                        output.Append("&uarr;")
                        Exit Select
                    Case 8657
                        output.Append("&uArr;")
                        Exit Select
                    Case 219
                        output.Append("&Ucirc;")
                        Exit Select
                    Case 251
                        output.Append("&ucirc;")
                        Exit Select
                    Case 217
                        output.Append("&Ugrave;")
                        Exit Select
                    Case 249
                        output.Append("&ugrave;")
                        Exit Select
                    Case 168
                        output.Append("&uml;")
                        Exit Select
                    Case 978
                        output.Append("&upsih;")
                        Exit Select
                    Case 933
                        output.Append("&Upsilon;")
                        Exit Select
                    Case 965
                        output.Append("&upsilon;")
                        Exit Select
                    Case 220
                        output.Append("&Uuml;")
                        Exit Select
                    Case 252
                        output.Append("&uuml;")
                        Exit Select
                    Case 8472
                        output.Append("&weierp;")
                        Exit Select
                    Case 926
                        output.Append("&Xi;")
                        Exit Select
                    Case 958
                        output.Append("&xi;")
                        Exit Select
                    Case 221
                        output.Append("&Yacute;")
                        Exit Select
                    Case 253
                        output.Append("&yacute;")
                        Exit Select
                    Case 165
                        output.Append("&yen;")
                        Exit Select
                    Case 376
                        output.Append("&Yuml;")
                        Exit Select
                    Case 255
                        output.Append("&yuml;")
                        Exit Select
                    Case 918
                        output.Append("&Zeta;")
                        Exit Select
                    Case 950
                        output.Append("&zeta;")
                        Exit Select
                    Case 8205
                        output.Append("&zwj;")
                        Exit Select
                    Case 8204
                        output.Append("&zwnj;")
                        Exit Select
                    Case Else
                        If c <= 127 Then
                            output.Append(Chr(c))
                        Else
                            output.Append("&#" + c + ";")
                        End If
                        Exit Select
                End Select
                c = reader.Read()
            End While
            Return output.ToString()
        End Function

        Public Shared Function DecodeValue(value As String) As String
            Dim output As New StringBuilder()
            Dim reader As New StringReader(value)
            Dim token As StringBuilder
            Dim c As Integer = reader.Read()
            While c <> -1
                token = New StringBuilder()
                While c <> Asc("&"c) AndAlso c <> -1
                    token.Append(Chr(c))
                    c = reader.Read()
                End While
                output.Append(token.ToString())
                If c = Asc("&"c) Then
                    token = New StringBuilder()
                    While c <> Asc(";"c) AndAlso c <> -1
                        token.Append(Chr(c))
                        c = reader.Read()
                    End While
                    If c = Asc(";"c) Then
                        c = reader.Read()
                        token.Append(";"c)
                        If token(1) = "#"c Then
                            Dim v As Integer = Integer.Parse(token.ToString().Substring(2, token.Length - 3))
                            output.Append(Chr(v))
                        Else
                            Select Case token.ToString()
                                Case "&lt;"
                                    output.Append("<")
                                    Exit Select
                                Case "&gt;"
                                    output.Append(">")
                                    Exit Select
                                Case "&quot;"
                                    output.Append("""")
                                    Exit Select
                                Case "&amp;"
                                    output.Append("&")
                                    Exit Select
                                Case "&Aacute;"
                                    output.Append(Chr(193))
                                    Exit Select
                                Case "&aacute;"
                                    output.Append(Chr(225))
                                    Exit Select
                                Case "&Acirc;"
                                    output.Append(Chr(194))
                                    Exit Select
                                Case "&acirc;"
                                    output.Append(Chr(226))
                                    Exit Select
                                Case "&acute;"
                                    output.Append(Chr(180))
                                    Exit Select
                                Case "&AElig;"
                                    output.Append(Chr(198))
                                    Exit Select
                                Case "&aelig;"
                                    output.Append(Chr(230))
                                    Exit Select
                                Case "&Agrave;"
                                    output.Append(Chr(192))
                                    Exit Select
                                Case "&agrave;"
                                    output.Append(Chr(224))
                                    Exit Select
                                Case "&alefsym;"
                                    output.Append(Chr(8501))
                                    Exit Select
                                Case "&Alpha;"
                                    output.Append(Chr(913))
                                    Exit Select
                                Case "&alpha;"
                                    output.Append(Chr(945))
                                    Exit Select

                                Case "&and;"
                                    output.Append(Chr(8743))
                                    Exit Select
                                Case "&ang;"
                                    output.Append(Chr(8736))
                                    Exit Select
                                Case "&Aring;"
                                    output.Append(Chr(197))
                                    Exit Select
                                Case "&aring;"
                                    output.Append(Chr(229))
                                    Exit Select
                                Case "&asymp;"
                                    output.Append(Chr(8776))
                                    Exit Select
                                Case "&Atilde;"
                                    output.Append(Chr(195))
                                    Exit Select
                                Case "&atilde;"
                                    output.Append(Chr(227))
                                    Exit Select
                                Case "&Auml;"
                                    output.Append(Chr(196))
                                    Exit Select
                                Case "&auml;"
                                    output.Append(Chr(228))
                                    Exit Select
                                Case "&bdquo;"
                                    output.Append(Chr(8222))
                                    Exit Select
                                Case "&Beta;"
                                    output.Append(Chr(914))
                                    Exit Select
                                Case "&beta;"
                                    output.Append(Chr(946))
                                    Exit Select
                                Case "&brvbar;"
                                    output.Append(Chr(166))
                                    Exit Select
                                Case "&bull;"
                                    output.Append(Chr(8226))
                                    Exit Select
                                Case "&cap;"
                                    output.Append(Chr(8745))
                                    Exit Select
                                Case "&Ccedil;"
                                    output.Append(Chr(199))
                                    Exit Select
                                Case "&ccedil;"
                                    output.Append(Chr(231))
                                    Exit Select
                                Case "&cedil;"
                                    output.Append(Chr(184))
                                    Exit Select
                                Case "&cent;"
                                    output.Append(Chr(162))
                                    Exit Select
                                Case "&Chi;"
                                    output.Append(Chr(935))
                                    Exit Select
                                Case "&chi;"
                                    output.Append(Chr(967))
                                    Exit Select
                                Case "&circ;"
                                    output.Append(Chr(710))
                                    Exit Select
                                Case "&clubs;"
                                    output.Append(Chr(9827))
                                    Exit Select
                                Case "&cong;"
                                    output.Append(Chr(8773))
                                    Exit Select
                                Case "&copy;"
                                    output.Append(Chr(169))
                                    Exit Select
                                Case "&crarr;"
                                    output.Append(Chr(8629))
                                    Exit Select
                                Case "&cup;"
                                    output.Append(Chr(8746))
                                    Exit Select
                                Case "&curren;"
                                    output.Append(Chr(164))
                                    Exit Select
                                Case "&dagger;"
                                    output.Append(Chr(8224))
                                    Exit Select
                                Case "&Dagger;"
                                    output.Append(Chr(8225))
                                    Exit Select
                                Case "&darr;"
                                    output.Append(Chr(8595))
                                    Exit Select
                                Case "&dArr;"
                                    output.Append(Chr(8659))
                                    Exit Select
                                Case "&deg;"
                                    output.Append(Chr(176))
                                    Exit Select
                                Case "&Delta;"
                                    output.Append(Chr(916))
                                    Exit Select
                                Case "&delta;"
                                    output.Append(Chr(948))
                                    Exit Select
                                Case "&diams;"
                                    output.Append(Chr(9830))
                                    Exit Select
                                Case "&divide;"
                                    output.Append(Chr(247))
                                    Exit Select
                                Case "&Eacute;"
                                    output.Append(Chr(201))
                                    Exit Select
                                Case "&eacute;"
                                    output.Append(Chr(233))
                                    Exit Select
                                Case "&Ecirc;"
                                    output.Append(Chr(202))
                                    Exit Select
                                Case "&ecirc;"
                                    output.Append(Chr(234))
                                    Exit Select

                                Case "&Egrave;"
                                    output.Append(Chr(200))
                                    Exit Select
                                Case "&egrave;"
                                    output.Append(Chr(232))
                                    Exit Select
                                Case "&empty;"
                                    output.Append(Chr(8709))
                                    Exit Select
                                Case "&emsp;"
                                    output.Append(Chr(8195))
                                    Exit Select
                                Case "&Epsilon;"
                                    output.Append(Chr(917))
                                    Exit Select
                                Case "&epsilon;"
                                    output.Append(Chr(949))
                                    Exit Select
                                Case "&equiv;"
                                    output.Append(Chr(8801))
                                    Exit Select
                                Case "&Eta;"
                                    output.Append(Chr(919))
                                    Exit Select
                                Case "&eta;"
                                    output.Append(Chr(951))
                                    Exit Select
                                Case "&ETH;"
                                    output.Append(Chr(208))
                                    Exit Select
                                Case "&eth;"
                                    output.Append(Chr(240))
                                    Exit Select
                                Case "&Euml;"
                                    output.Append(Chr(203))
                                    Exit Select
                                Case "&euml;"
                                    output.Append(Chr(235))
                                    Exit Select
                                Case "&euro;"
                                    output.Append(Chr(128))
                                    Exit Select
                                Case "&exist;"
                                    output.Append(Chr(8707))
                                    Exit Select
                                Case "&fnof;"
                                    output.Append(Chr(402))
                                    Exit Select
                                Case "&forall;"
                                    output.Append(Chr(8704))
                                    Exit Select
                                Case "&frac12;"
                                    output.Append(Chr(189))
                                    Exit Select
                                Case "&frac14;"
                                    output.Append(Chr(188))
                                    Exit Select
                                Case "&frac34;"
                                    output.Append(Chr(190))
                                    Exit Select
                                Case "&fras1;"
                                    output.Append(Chr(8260))
                                    Exit Select
                                Case "&Gamma;"
                                    output.Append(Chr(915))
                                    Exit Select
                                Case "&gamma;"
                                    output.Append(Chr(947))
                                    Exit Select
                                Case "&ge;"
                                    output.Append(Chr(8805))
                                    Exit Select
                                Case "&harr;"
                                    output.Append(Chr(8596))
                                    Exit Select
                                Case "&hArr;"
                                    output.Append(Chr(8660))
                                    Exit Select
                                Case "&hearts;"
                                    output.Append(Chr(9829))
                                    Exit Select
                                Case "&hellip;"
                                    output.Append(Chr(8230))
                                    Exit Select
                                Case "&Iacute;"
                                    output.Append(Chr(205))
                                    Exit Select
                                Case "&iacute;"
                                    output.Append(Chr(237))
                                    Exit Select
                                Case "&Icirc;"
                                    output.Append(Chr(206))
                                    Exit Select
                                Case "&icirc;"
                                    output.Append(Chr(238))
                                    Exit Select
                                Case "&iexcl;"
                                    output.Append(Chr(161))
                                    Exit Select
                                Case "&Igrave;"
                                    output.Append(Chr(204))
                                    Exit Select
                                Case "&igrave;"
                                    output.Append(Chr(236))
                                    Exit Select
                                Case "&image;"
                                    output.Append(Chr(8465))
                                    Exit Select
                                Case "&infin;"
                                    output.Append(Chr(8734))
                                    Exit Select
                                Case "&int;"
                                    output.Append(Chr(8747))
                                    Exit Select
                                Case "&Iota;"
                                    output.Append(Chr(921))
                                    Exit Select

                                Case "&iota;"
                                    output.Append(Chr(953))
                                    Exit Select
                                Case "&iquest;"
                                    output.Append(Chr(191))
                                    Exit Select
                                Case "&isin;"
                                    output.Append(Chr(8712))
                                    Exit Select
                                Case "&Iuml;"
                                    output.Append(Chr(207))
                                    Exit Select
                                Case "&iuml;"
                                    output.Append(Chr(239))
                                    Exit Select
                                Case "&Kappa;"
                                    output.Append(Chr(922))
                                    Exit Select
                                Case "&kappa;"
                                    output.Append(Chr(954))
                                    Exit Select
                                Case "&Lambda;"
                                    output.Append(Chr(923))
                                    Exit Select
                                Case "&lambda;"
                                    output.Append(Chr(955))
                                    Exit Select
                                Case "&lang;"
                                    output.Append(Chr(9001))
                                    Exit Select
                                Case "&laquo;"
                                    output.Append(Chr(171))
                                    Exit Select
                                Case "&larr;"
                                    output.Append(Chr(8592))
                                    Exit Select
                                Case "&lArr;"
                                    output.Append(Chr(8656))
                                    Exit Select
                                Case "&lceil;"
                                    output.Append(Chr(8968))
                                    Exit Select
                                Case "&ldquo;"
                                    output.Append(Chr(8220))
                                    Exit Select
                                Case "&le;"
                                    output.Append(Chr(8804))
                                    Exit Select
                                Case "&lfloor;"
                                    output.Append(Chr(8970))
                                    Exit Select
                                Case "&lowast;"
                                    output.Append(Chr(8727))
                                    Exit Select
                                Case "&loz;"
                                    output.Append(Chr(9674))
                                    Exit Select
                                Case "&lrm;"
                                    output.Append(Chr(8206))
                                    Exit Select
                                Case "&lsaquo;"
                                    output.Append(Chr(8249))
                                    Exit Select
                                Case "&lsquo;"
                                    output.Append(Chr(8216))
                                    Exit Select
                                Case "&macr;"
                                    output.Append(Chr(175))
                                    Exit Select
                                Case "&mdash;"
                                    output.Append(Chr(8212))
                                    Exit Select
                                Case "&micro;"
                                    output.Append(Chr(181))
                                    Exit Select
                                Case "&middot;"
                                    output.Append(Chr(183))
                                    Exit Select
                                Case "&minus;"
                                    output.Append(Chr(8722))
                                    Exit Select
                                Case "&Mu;"
                                    output.Append(Chr(924))
                                    Exit Select
                                Case "&mu;"
                                    output.Append(Chr(956))
                                    Exit Select
                                Case "&nabla;"
                                    output.Append(Chr(8711))
                                    Exit Select
                                Case "&nbsp;"
                                    output.Append(Chr(160))
                                    Exit Select
                                Case "&ndash;"
                                    output.Append(Chr(8211))
                                    Exit Select
                                Case "&ne;"
                                    output.Append(Chr(8800))
                                    Exit Select
                                Case "&ni;"
                                    output.Append(Chr(8715))
                                    Exit Select
                                Case "&not;"
                                    output.Append(Chr(172))
                                    Exit Select
                                Case "&notin;"
                                    output.Append(Chr(8713))
                                    Exit Select
                                Case "&nsub;"
                                    output.Append(Chr(8836))
                                    Exit Select
                                Case "&Ntilde;"
                                    output.Append(Chr(209))
                                    Exit Select
                                Case "&ntilde;"
                                    output.Append(Chr(241))
                                    Exit Select
                                Case "&Nu;"
                                    output.Append(Chr(925))
                                    Exit Select

                                Case "&nu;"
                                    output.Append(Chr(957))
                                    Exit Select
                                Case "&Oacute;"
                                    output.Append(Chr(211))
                                    Exit Select
                                Case "&oacute;"
                                    output.Append(Chr(243))
                                    Exit Select
                                Case "&Ocirc;"
                                    output.Append(Chr(212))
                                    Exit Select
                                Case "&ocirc;"
                                    output.Append(Chr(244))
                                    Exit Select
                                Case "&OElig;"
                                    output.Append(Chr(338))
                                    Exit Select
                                Case "&oelig;"
                                    output.Append(Chr(339))
                                    Exit Select
                                Case "&Ograve;"
                                    output.Append(Chr(210))
                                    Exit Select
                                Case "&ograve;"
                                    output.Append(Chr(242))
                                    Exit Select
                                Case "&oline;"
                                    output.Append(Chr(8254))
                                    Exit Select
                                Case "&Omega;"
                                    output.Append(Chr(937))
                                    Exit Select
                                Case "&omega;"
                                    output.Append(Chr(969))
                                    Exit Select
                                Case "&Omicron;"
                                    output.Append(Chr(927))
                                    Exit Select
                                Case "&omicron;"
                                    output.Append(Chr(959))
                                    Exit Select
                                Case "&oplus;"
                                    output.Append(Chr(8853))
                                    Exit Select
                                Case "&or;"
                                    output.Append(Chr(8744))
                                    Exit Select
                                Case "&ordf;"
                                    output.Append(Chr(170))
                                    Exit Select
                                Case "&ordm;"
                                    output.Append(Chr(186))
                                    Exit Select
                                Case "&Oslash;"
                                    output.Append(Chr(216))
                                    Exit Select
                                Case "&oslash;"
                                    output.Append(Chr(248))
                                    Exit Select
                                Case "&Otilde;"
                                    output.Append(Chr(213))
                                    Exit Select
                                Case "&otilde;"
                                    output.Append(Chr(245))
                                    Exit Select
                                Case "&otimes;"
                                    output.Append(Chr(8855))
                                    Exit Select
                                Case "&Ouml;"
                                    output.Append(Chr(214))
                                    Exit Select
                                Case "&ouml;"
                                    output.Append(Chr(246))
                                    Exit Select
                                Case "&para;"
                                    output.Append(Chr(182))
                                    Exit Select
                                Case "&part;"
                                    output.Append(Chr(8706))
                                    Exit Select
                                Case "&permil;"
                                    output.Append(Chr(8240))
                                    Exit Select
                                Case "&perp;"
                                    output.Append(Chr(8869))
                                    Exit Select
                                Case "&Phi;"
                                    output.Append(Chr(934))
                                    Exit Select
                                Case "&phi;"
                                    output.Append(Chr(966))
                                    Exit Select
                                Case "&Pi;"
                                    output.Append(Chr(928))
                                    Exit Select
                                Case "&pi;"
                                    output.Append(Chr(960))
                                    Exit Select
                                Case "&piv;"
                                    output.Append(Chr(982))
                                    Exit Select
                                Case "&plusmn;"
                                    output.Append(Chr(177))
                                    Exit Select
                                Case "&pound;"
                                    output.Append(Chr(163))
                                    Exit Select
                                Case "&prime;"
                                    output.Append(Chr(8242))
                                    Exit Select
                                Case "&Prime;"
                                    output.Append(Chr(8243))
                                    Exit Select
                                Case "&prod;"
                                    output.Append(Chr(8719))
                                    Exit Select
                                Case "&prop;"
                                    output.Append(Chr(8733))
                                    Exit Select
                                Case "&Psi;"
                                    output.Append(Chr(936))
                                    Exit Select

                                Case "&psi;"
                                    output.Append(Chr(968))
                                    Exit Select
                                Case "&radic;"
                                    output.Append(Chr(8730))
                                    Exit Select
                                Case "&rang;"
                                    output.Append(Chr(9002))
                                    Exit Select
                                Case "&raquo;"
                                    output.Append(Chr(187))
                                    Exit Select
                                Case "&rarr;"
                                    output.Append(Chr(8594))
                                    Exit Select
                                Case "&rArr;"
                                    output.Append(Chr(8658))
                                    Exit Select
                                Case "&rceil;"
                                    output.Append(Chr(8969))
                                    Exit Select
                                Case "&rdquo;"
                                    output.Append(Chr(8221))
                                    Exit Select
                                Case "&real;"
                                    output.Append(Chr(8476))
                                    Exit Select
                                Case "&reg;"
                                    output.Append(Chr(174))
                                    Exit Select
                                Case "&rfloor;"
                                    output.Append(Chr(8971))
                                    Exit Select
                                Case "&Rho;"
                                    output.Append(Chr(929))
                                    Exit Select
                                Case "&rho;"
                                    output.Append(Chr(961))
                                    Exit Select
                                Case "&rlm;"
                                    output.Append(Chr(8207))
                                    Exit Select
                                Case "&rsaquo;"
                                    output.Append(Chr(8250))
                                    Exit Select
                                Case "&rsquo;"
                                    output.Append(Chr(8217))
                                    Exit Select
                                Case "&sbquo;"
                                    output.Append(Chr(8218))
                                    Exit Select
                                Case "&Scaron;"
                                    output.Append(Chr(352))
                                    Exit Select
                                Case "&scaron;"
                                    output.Append(Chr(353))
                                    Exit Select
                                Case "&sdot;"
                                    output.Append(Chr(8901))
                                    Exit Select
                                Case "&sect;"
                                    output.Append(Chr(167))
                                    Exit Select
                                Case "&shy;"
                                    output.Append(Chr(173))
                                    Exit Select
                                Case "&Sigma;"
                                    output.Append(Chr(931))
                                    Exit Select
                                Case "&sigma;"
                                    output.Append(Chr(963))
                                    Exit Select
                                Case "&sigmaf;"
                                    output.Append(Chr(962))
                                    Exit Select
                                Case "&sim;"
                                    output.Append(Chr(8764))
                                    Exit Select
                                Case "&spades;"
                                    output.Append(Chr(9824))
                                    Exit Select
                                Case "&sub;"
                                    output.Append(Chr(8834))
                                    Exit Select
                                Case "&sube;"
                                    output.Append(Chr(8838))
                                    Exit Select
                                Case "&sum;"
                                    output.Append(Chr(8721))
                                    Exit Select
                                Case "&sup;"
                                    output.Append(Chr(8835))
                                    Exit Select
                                Case "&sup1;"
                                    output.Append(Chr(185))
                                    Exit Select
                                Case "&sup2;"
                                    output.Append(Chr(178))
                                    Exit Select
                                Case "&sup3;"
                                    output.Append(Chr(179))
                                    Exit Select
                                Case "&supe;"
                                    output.Append(Chr(8839))
                                    Exit Select
                                Case "&szlig;"
                                    output.Append(Chr(223))
                                    Exit Select
                                Case "&Tau;"
                                    output.Append(Chr(932))
                                    Exit Select
                                Case "&tau;"
                                    output.Append(Chr(964))
                                    Exit Select
                                Case "&there4;"
                                    output.Append(Chr(8756))
                                    Exit Select
                                Case "&Theta;"
                                    output.Append(Chr(920))
                                    Exit Select

                                Case "&theta;"
                                    output.Append(Chr(952))
                                    Exit Select
                                Case "&thetasym;"
                                    output.Append(Chr(977))
                                    Exit Select
                                Case "&thinsp;"
                                    output.Append(Chr(8201))
                                    Exit Select
                                Case "&THORN;"
                                    output.Append(Chr(222))
                                    Exit Select
                                Case "&thorn;"
                                    output.Append(Chr(254))
                                    Exit Select
                                Case "&tilde;"
                                    output.Append(Chr(732))
                                    Exit Select
                                Case "&times;"
                                    output.Append(Chr(215))
                                    Exit Select
                                Case "&trade;"
                                    output.Append(Chr(8482))
                                    Exit Select
                                Case "&Uacute;"
                                    output.Append(Chr(218))
                                    Exit Select
                                Case "&uacute;"
                                    output.Append(Chr(250))
                                    Exit Select
                                Case "&uarr;"
                                    output.Append(Chr(8593))
                                    Exit Select
                                Case "&uArr;"
                                    output.Append(Chr(8657))
                                    Exit Select
                                Case "&Ucirc;"
                                    output.Append(Chr(219))
                                    Exit Select
                                Case "&ucirc;"
                                    output.Append(Chr(251))
                                    Exit Select
                                Case "&Ugrave;"
                                    output.Append(Chr(217))
                                    Exit Select
                                Case "&ugrave;"
                                    output.Append(Chr(249))
                                    Exit Select
                                Case "&uml;"
                                    output.Append(Chr(168))
                                    Exit Select
                                Case "&upsih;"
                                    output.Append(Chr(978))
                                    Exit Select
                                Case "&Upsilon;"
                                    output.Append(Chr(933))
                                    Exit Select
                                Case "&upsilon;"
                                    output.Append(Chr(965))
                                    Exit Select
                                Case "&Uuml;"
                                    output.Append(Chr(220))
                                    Exit Select
                                Case "&uuml;"
                                    output.Append(Chr(252))
                                    Exit Select
                                Case "&weierp;"
                                    output.Append(Chr(8472))
                                    Exit Select
                                Case "&Xi;"
                                    output.Append(Chr(926))
                                    Exit Select
                                Case "&xi;"
                                    output.Append(Chr(958))
                                    Exit Select
                                Case "&Yacute;"
                                    output.Append(Chr(221))
                                    Exit Select
                                Case "&yacute;"
                                    output.Append(Chr(253))
                                    Exit Select
                                Case "&yen;"
                                    output.Append(Chr(165))
                                    Exit Select
                                Case "&Yuml;"
                                    output.Append(Chr(376))
                                    Exit Select
                                Case "&yuml;"
                                    output.Append(Chr(255))
                                    Exit Select
                                Case "&Zeta;"
                                    output.Append(Chr(918))
                                    Exit Select
                                Case "&zeta;"
                                    output.Append(Chr(950))
                                    Exit Select
                                Case "&zwj;"
                                    output.Append(Chr(8205))
                                    Exit Select
                                Case "&zwnj;"
                                    output.Append(Chr(8204))
                                    Exit Select
                                Case Else

                                    output.Append(token.ToString())
                                    Exit Select
                            End Select
                        End If
                    Else
                        output.Append(token.ToString())
                    End If
                End If
            End While
            Return output.ToString()
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
