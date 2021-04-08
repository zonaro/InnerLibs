## `AddressInfo`

Representa um deteminado local com suas Informações
```csharp
public class InnerLibs.Locations.AddressInfo

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Address | Retorna o endereço completo | 
| `String` | City | Cidade | 
| `String` | Complement | Complemento | 
| `String` | Country | País | 
| `String` | DDD | DDD do local | 
| `String` | GIA |  | 
| `String` | IBGE |  | 
| `Decimal` | Latitude | Coordenada geográfica LATITUDE | 
| `Decimal` | Longitude | Coordenada geográfica LONGITUDE | 
| `String` | Neighborhood | Bairro | 
| `String` | Number | Numero da casa, predio etc. | 
| `String` | PostalCode | CEP - Codigo de Endereçamento Postal | 
| `String` | SIAFI |  | 
| `String` | State | Estado | 
| `String` | StateCode | Unidade federativa | 
| `String` | Street | Logradouro | 
| `String` | StreetName | O nome do endereço | 
| `String` | StreetType | Tipo do Endereço | 
| `String` | ZipCode | CEP - Codigo de Endereçamento Postal. Alias de `InnerLibs.Locations.AddressInfo.PostalCode` | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | GetInfoByPostalCode() | Retorna o endereço de acordo com o CEP contidos em uma variavel do tipo InnerLibs.Location usando a API https://viacep.com.br/ | 
| `String` | LatitudeLongitude() | Retorna as coordenadas geográficas do Local | 
| `String` | ToString(`Boolean` IncludeNumber = True, `Boolean` IncludeComplement = True, `Boolean` IncludeNeighborhood = True, `Boolean` IncludeCity = True, `Boolean` IncludeState = True, `Boolean` IncludePostalCode = True, `Boolean` IncludeCountry = False) | Retorna as string do endereço omitindo ou não informações | 
| `String` | ToString() | Retorna as string do endereço omitindo ou não informações | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressInfo` | CreateLocation(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) | Cria uma localização a partir de informações | 
| `String` | FormatAddress(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) |  | 
| `String` | FormatPostalCode(`String` CEP) | Formata uma string de CEP | 


## `AddressTypes`

```csharp
public class InnerLibs.Locations.AddressTypes

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String[]` | Aeroporto |  | 
| `String[]` | Alameda |  | 
| `String[]` | Área |  | 
| `String[]` | Avenida |  | 
| `String[]` | Campo |  | 
| `String[]` | Chácara |  | 
| `String[]` | Colônia |  | 
| `String[]` | Comunidade |  | 
| `String[]` | Condomínio |  | 
| `String[]` | Conjunto |  | 
| `String[]` | Distrito |  | 
| `String[]` | Esplanada |  | 
| `String[]` | Estação |  | 
| `String[]` | Estrada |  | 
| `String[]` | Favela |  | 
| `String[]` | Feira |  | 
| `String[]` | Jardim |  | 
| `String[]` | Ladeira |  | 
| `String[]` | Lago |  | 
| `String[]` | Lagoa |  | 
| `String[]` | Largo |  | 
| `String[]` | Loteamento |  | 
| `String[]` | Morro |  | 
| `String[]` | Núcleo |  | 
| `String[]` | Parque |  | 
| `String[]` | Passarela |  | 
| `String[]` | Pátio |  | 
| `String[]` | Praça |  | 
| `String[]` | Quadra |  | 
| `String[]` | Recanto |  | 
| `String[]` | Residencial |  | 
| `String[]` | Rodovia |  | 
| `String[]` | Rua |  | 
| `String[]` | Setor |  | 
| `String[]` | Sítio |  | 
| `String[]` | Travessa |  | 
| `String[]` | Trecho |  | 
| `String[]` | Trevo |  | 
| `String[]` | Vale |  | 
| `String[]` | Vereda |  | 
| `String[]` | Via |  | 
| `String[]` | Viaduto |  | 
| `String[]` | Viela |  | 
| `String[]` | Vila |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetAddressType(`String` Endereco) |  | 


## `Brasil`

Objeto para manipular cidades e estados do Brasil
```csharp
public class InnerLibs.Locations.Brasil

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | Regions | Retorna as Regiões dos estados brasileiros | 
| `IEnumerable<State>` | States | Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetAcronymOf(`String` Name) | Retorna a Sigla a partir de um nome de estado | 
| `IEnumerable<String>` | GetCitiesOf(`String` NameOrStateCode = ) | Retorna as cidades de um estado a partir do nome ou sigla do estado | 
| `String` | GetNameOf(`String` StateCode) | Retorna o nome do estado a partir da sigla | 
| `List<String>` | GetStateList(`StateString` Type = Name) | Retorna uma lista contendo os nomes ou siglas dos estados do Brasil | 
| `IEnumerable<String>` | GetStatesOf(`String` Region = , `StateString` Type = Name) | Retorna os estados de uma região | 


## `State`

Objeto que representa um estado do Brasil e seus respectivos detalhes
```csharp
public class InnerLibs.Locations.State

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Acronym | Sigla do estado | 
| `List<String>` | Cities | Lista de cidades do estado | 
| `String` | Name | Nome do estado | 
| `String` | Region | Região do Estado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Retorna a String correspondente ao estado | 
| `String` | ToString(`StateString` Type = Acronym) | Retorna a String correspondente ao estado | 


