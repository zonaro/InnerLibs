Imports System.Net
Imports System.Xml

Namespace Locations

    ''' <summary>
    ''' Representa um deteminado local com suas Informações
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Location

        ''' <summary>
        ''' Cria um novo objeto de localização vazio
        ''' </summary>
        Public Sub New()
        End Sub


        ''' <summary>
        ''' Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP
        ''' </summary>
        ''' <param name="PostalCode"></param>
        ''' <param name="Number">Numero da casa</param>
        Public Sub New(PostalCode As String, Optional Number As String = Nothing)
            Me.PostalCode = PostalCode
            If Not Number = Nothing Then Me.Number = Number
            Me.GetInfoByPostalCode()
        End Sub

        ''' <summary>
        ''' Tipo do Endereço
        ''' </summary>
        ''' <returns></returns>
        Property StreetType As String

        ''' <summary>
        ''' O nome do endereço
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        Property StreetName As String

        ''' <summary>
        ''' Logradouro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Street As String
            Get
                Return StreetType & " " & StreetName
            End Get
        End Property

        ''' <summary>
        ''' Numero da casa, predio etc.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Numero</returns>

        Property Number As String
        ''' <summary>
        ''' Complemento
        ''' </summary>
        ''' <value></value>
        ''' <returns>Complemento</returns>

        Property Complement As String
        ''' <summary>
        ''' Bairro
        ''' </summary>
        ''' <value></value>
        ''' <returns>Bairro</returns>

        Property Neighborhood As String
        ''' <summary>
        ''' CEP - Codigo de Endereçamento Postal
        ''' </summary>
        ''' <value></value>
        ''' <returns>CEP</returns>

        Property PostalCode As String
            Get
                Return _cep
            End Get
            Set(value As String)
                _cep = Location.FormatPostalCode(value)
            End Set
        End Property

        ''' <summary>
        ''' Formata uma string de CEP
        ''' </summary>
        ''' <param name="CEP"></param>
        ''' <returns></returns>
        Public Shared Function FormatPostalCode(CEP As String) As String
            CEP = CEP.Trim()
            If CEP.IsValidCEP Then
                If CEP.IsNumber() Then
                    CEP = CEP.Insert(5, "-")
                End If
                Return CEP
            End If
            Return Nothing
        End Function

        Private _cep As String

        ''' <summary>
        ''' CEP - Codigo de Endereçamento Postal. Alias de <see cref="PostalCode"/>
        ''' </summary>
        ''' <value></value>
        ''' <returns>CEP</returns>
        Property ZipCode As String
            Get
                Return PostalCode
            End Get
            Set(value As String)
                PostalCode = value
            End Set
        End Property


        ''' <summary>
        ''' Cidade
        ''' </summary>
        ''' <value></value>
        ''' <returns>Cidade</returns>

        Property City As String
        ''' <summary>
        ''' Estado
        ''' </summary>
        ''' <value></value>
        ''' <returns>Estado</returns>

        Property State As String
        ''' <summary>
        ''' Unidade federativa
        ''' </summary>
        ''' <value></value>
        ''' <returns>Sigla do estado</returns>

        Property StateCode As String

        ''' <summary>
        ''' País
        ''' </summary>
        ''' <value></value>
        ''' <returns>País</returns>

        Property Country As String
        ''' <summary>
        ''' Coordenada geográfica LATITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Latitude</returns>
        Property Latitude As Decimal

        ''' <summary>
        ''' Coordenada geográfica LONGITUDE
        ''' </summary>
        ''' <value></value>
        ''' <returns>Longitude</returns>
        Property Longitude As Decimal



        ''' <summary>
        ''' Retorna o endereço completo
        ''' </summary>
        ''' <returns>Uma String com o endereço completo devidamente formatado</returns>
        ReadOnly Property Address As String
            Get
                Return ToString(True, True, True, True, True, True, False)
            End Get
        End Property

        ''' <summary>
        ''' Retorna as string do endereço omitindo ou não informações
        ''' </summary>
        ''' <param name="IncludeNumber"></param>
        ''' <param name="IncludeComplement"></param>
        ''' <param name="IncludeNeighborhood"></param>
        ''' <param name="IncludeCity"></param>
        ''' <param name="IncludeState"></param>
        ''' <param name="IncludePostalCode"></param>
        ''' <param name="IncludeCountry"></param>
        ''' <returns></returns>
        Public Overloads Function ToString(Optional IncludeNumber As Boolean = True, Optional IncludeComplement As Boolean = True, Optional IncludeNeighborhood As Boolean = True, Optional IncludeCity As Boolean = True, Optional IncludeState As Boolean = True, Optional IncludePostalCode As Boolean = True, Optional IncludeCountry As Boolean = False) As String
            ParseType()
            Dim retorno As String = Street
            If Number.IsNotBlank AndAlso IncludeNumber Then retorno &= (", " & Number)
            If Complement.IsNotBlank AndAlso IncludeComplement Then retorno &= (", " & Complement)
            If Neighborhood.IsNotBlank AndAlso IncludeNeighborhood Then retorno &= (" - " & Neighborhood)
            If City.IsNotBlank AndAlso IncludeCity Then retorno &= (" - " & City)
            If IncludeState Then
                If StateCode.IsNotBlank Then
                    retorno &= (" - " & StateCode)
                Else
                    If State.IsNotBlank Then retorno &= (" - " & State)
                End If
            End If
            If PostalCode.IsNotBlank AndAlso IncludePostalCode Then retorno &= (" - " & PostalCode)
            If Country.IsNotBlank AndAlso IncludeCountry Then retorno &= (" - " & Country)
            Return retorno
        End Function


        Friend Sub ParseType()
            If Me.StreetType.IsBlank() Then
                If Me.StreetName.IsNotBlank Then
                    Me.StreetType = AddressTypes.GetAddressType(Me.StreetName)
                    If Me.StreetType.IsNotBlank Then
                        Me.StreetName = Me.StreetName.Trim().RemoveFirstIf(StreetType).Trim().AdjustBlankSpaces().ToTitle()
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Retorna uma String contendo as informações do Local
        ''' </summary>
        ''' <returns>string</returns>
        Public Overrides Function ToString() As String
            Return Address
        End Function

        ''' <summary>
        ''' Retorna as coordenadas geográficas do Local
        ''' </summary>
        ''' <returns>Uma String contendo LATITUDE e LONGITUDE separados por virgula</returns>

        Public Function LatitudeLongitude() As String
            Return Latitude & "," & Longitude
        End Function


        ''' <summary>
        ''' Retorna o endereço de acordo com o CEP contidos em uma variavel do tipo InnerLibs.Location usando a API https://viacep.com.br/
        ''' </summary>
        Public Sub GetInfoByPostalCode()
            Dim url = "https://viacep.com.br/ws/" & Me.PostalCode.RemoveAny("-") & "/xml/"
            Using c = New WebClient()
                Dim x = New XmlDocument()
                x.LoadXml(c.DownloadString(url))
                Dim cep = x("xmlcep")
                Me.Neighborhood = cep("bairro").InnerText
                Me.City = cep("localidade").InnerText
                Me.StateCode = cep("uf").InnerText
                Me.State = Brasil.GetNameOf(Me.StateCode)
                Me.StreetName = cep("logradouro").InnerText
                Me.Country = "Brasil"
                ParseType()
            End Using

        End Sub

    End Class

    Public Class AddressTypes

        Public Shared Function GetAddressType(Endereco As String) As String
            Dim tp = Endereco.Split(WordSplitters, StringSplitOptions.RemoveEmptyEntries).FirstOr("")
            If tp.IsNotBlank Then
                Dim df = New AddressTypes()
                Return df.GetProperties().FirstOrDefault(Function(x) tp.IsIn(CType(x.GetValue(df), String())) OrElse x.Name = tp).Name
            End If
            Return ""
        End Function

        Public ReadOnly Property Aeroporto As String() = {"Aeroporto", "Ar", "Aero"}
        Public ReadOnly Property Alameda As String() = {"Alameda", "Al", "Alm"}
        Public ReadOnly Property Área As String() = {"Área", "Area"}
        Public ReadOnly Property Avenida As String() = {"Avenida", "Av", "Avn"}
        Public ReadOnly Property Campo As String() = {"Cam", "Camp", "Campo"}
        Public ReadOnly Property Chácara As String() = {"Cha", "Chac", "Chacara"}
        Public ReadOnly Property Colônia As String() = {"Col", "Colonia"}
        Public ReadOnly Property Condomínio As String() = {"Condominio", "Cond"}
        Public ReadOnly Property Comunidade As String() = {"Com", "Comunidade"}
        Public ReadOnly Property Conjunto As String() = {"Conjunto", "Con"}
        Public ReadOnly Property Distrito As String() = {"Dis", "Dst", "Distrito"}
        Public ReadOnly Property Esplanada As String() = {"Esp", "Esplanada"}
        Public ReadOnly Property Estação As String() = {"Estacao", "Est", "st"}
        Public ReadOnly Property Estrada As String() = {"Et", "Es", "Estrada"}
        Public ReadOnly Property Favela As String() = {"Fav", "Favela"}
        Public ReadOnly Property Feira As String() = {"Fei", "Fr", "Feira"}
        Public ReadOnly Property Jardim As String() = {"Jd", "Jardim", "Jar"}
        Public ReadOnly Property Ladeira As String() = {"Lad", "Ld", "Ladeira"}
        Public ReadOnly Property Lago As String() = {"Lago", "Lg"}
        Public ReadOnly Property Lagoa As String() = {"Lagoa", "Lga"}
        Public ReadOnly Property Largo As String() = {"Lrg", "Largo", "Lgo"}
        Public ReadOnly Property Loteamento As String() = {"Lote", "Loteamento", "Lt"}
        Public ReadOnly Property Morro As String() = {"Mr", "Mrr", "Morro", "Mor"}
        Public ReadOnly Property Núcleo As String() = {"Nc", "Nucleo", "Nuc"}
        Public ReadOnly Property Parque As String() = {"Parque", "Pq", "Pk", "Par", "Parq", "Park"}
        Public ReadOnly Property Passarela As String() = {"Pass", "Pas", "Pa", "Passarela"}
        Public ReadOnly Property Pátio As String() = {"Pt", "Pat", "Pateo", "Patio"}
        Public ReadOnly Property Praça As String() = {"Praça", "Pc", "Praca", "Pç"}
        Public ReadOnly Property Quadra As String() = {"Qd", "Quadra", "Quad"}
        Public ReadOnly Property Recanto As String() = {"Rec", "Recanto", "Rc"}
        Public ReadOnly Property Residencial As String() = {"Residencial", "Residencia", "Res", "Resid"}
        Public ReadOnly Property Rodovia As String() = {"Rodovia", "Rod"}
        Public ReadOnly Property Rua As String() = {"Rua", "R"}
        Public ReadOnly Property Setor As String() = {"Setor", "Str"}
        Public ReadOnly Property Sítio As String() = {"Sitio", "Sit"}
        Public ReadOnly Property Travessa As String() = {"Trv", "Travessa", "Tvs"}
        Public ReadOnly Property Trecho As String() = {"Trc", "Trecho"}
        Public ReadOnly Property Trevo As String() = {"Trevo"}
        Public ReadOnly Property Vale As String() = {"Vale", "Val"}
        Public ReadOnly Property Vereda As String() = {"Vereda"}
        Public ReadOnly Property Via As String() = {"Via"}
        Public ReadOnly Property Viaduto As String() = {"Vd", "Viaduto"}
        Public ReadOnly Property Viela As String() = {"Viela"}
        Public ReadOnly Property Vila As String() = {"Vila", "Vl"}
    End Class

End Namespace