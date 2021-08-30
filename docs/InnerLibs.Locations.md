## `AddressInfo`

Representa um deteminado local com suas Informações
```csharp
public class InnerLibs.Locations.AddressInfo

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | City | Cidade | 
| `String` | Complement | Complemento | 
| `String` | Country | País | 
| `String` | CountryCode | País | 
| `String` | Detail | Retona uma informação deste endereço | 
| `AddressPart` | Format | Formato desta instancia de `InnerLibs.Locations.AddressInfo` quando chamada pelo `InnerLibs.Locations.AddressInfo.ToString` | 
| `String` | FullAddress | Retorna o endereço completo | 
| `String` | FullLocationInfo | Logradouro, Numero e Complemento | 
| `Nullable<Decimal>` | Latitude | Coordenada geográfica LATITUDE | 
| `String` | LocationInfo | Retorna o Logradouro e numero | 
| `Nullable<Decimal>` | Longitude | Coordenada geográfica LONGITUDE | 
| `String` | Name | O nome do endereço | 
| `String` | Neighborhood | Bairro | 
| `String` | Number | Numero da casa, predio etc. | 
| `String` | PostalCode | CEP - Codigo de Endereçamento Postal | 
| `String` | Region | País | 
| `String` | State | Estado | 
| `String` | StateCode | Unidade federativa | 
| `String` | Street | Logradouro | 
| `String` | Type | Tipo do Endereço | 
| `String` | ZipCode | CEP - Codigo de Endereçamento Postal. Alias de `InnerLibs.Locations.AddressInfo.PostalCode` | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` key, `String` value) |  | 
| `void` | Clear() |  | 
| `Boolean` | ContainsKey(`String` key) |  | 
| `Dictionary<String, String>` | GetDetails() |  | 
| `Point` | GetPoint() |  | 
| `String` | LatitudeLongitude() | Retorna as coordenadas geográficas do Local | 
| `Boolean` | Remove(`String` key) |  | 
| `AddressInfo` | SetLatitudeLongitudeFromPoint(`Point` Point) |  | 
| `String` | ToString() | Retorna uma String contendo as informações do Local | 
| `String` | ToString(`IEnumerable<AddressPart>` Parts) | Retorna uma String contendo as informações do Local | 
| `String` | ToString(`AddressPart[]` Parts) | Retorna uma String contendo as informações do Local | 
| `String` | ToString(`Int32` FormatCode) | Retorna uma String contendo as informações do Local | 
| `String` | ToString(`AddressPart` Parts) | Retorna uma String contendo as informações do Local | 


Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressPart` | GlobalFormat | Formato global de todas as intancias de `InnerLibs.Locations.AddressInfo` quando chamadas pelo `InnerLibs.Locations.AddressInfo.ToString` | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressInfo` | CreateLocation(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) | Cria uma localização a partir de partes de endereço | 
| `T` | CreateLocation(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) | Cria uma localização a partir de partes de endereço | 
| `String` | FormatAddress(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) | Retorna uma string de endereço a partir de varias partes de endereco | 
| `String` | FormatPostalCode(`String` CEP) | Formata uma string de CEP | 
| `AddressInfo` | FromGoogleMaps(`String` Address, `String` Key, `NameValueCollection` NVC = null) | Retorna um `InnerLibs.Locations.AddressInfo` usando a api de geocode do Google Maps para completar as informações | 
| `T` | FromGoogleMaps(`String` Address, `String` Key, `NameValueCollection` NVC = null) | Retorna um `InnerLibs.Locations.AddressInfo` usando a api de geocode do Google Maps para completar as informações | 
| `AddressInfo` | FromViaCEP(`Int64` PostalCode, `String` Number = null, `String` Complement = null) | Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP | 
| `AddressInfo` | FromViaCEP(`String` PostalCode, `String` Number = null, `String` Complement = null) | Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP | 
| `T` | FromViaCEP(`Int64` PostalCode, `String` Number = null, `String` Complement = null) | Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP | 
| `T` | FromViaCEP(`String` PostalCode, `String` Number = null, `String` Complement = null) | Cria um objeto de localização e imadiatamente pesquisa as informações de um local através do CEP usando as APIs ViaCEP | 
| `AddressInfo` | TryParse(`String` Address) | Tenta extrair as partes de um endereço de uma string | 
| `T` | TryParse(`String` Address) | Tenta extrair as partes de um endereço de uma string | 


## `AddressPart`

Partes de um Endereço
```csharp
public enum InnerLibs.Locations.AddressPart
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Default | Formato default definido pela propriedade `InnerLibs.Locations.AddressInfo.Format` ou `InnerLibs.Locations.AddressInfo.GlobalFormat` | 
| `1` | StreetType | Tipo do Lograoduro | 
| `2` | StreetName | Nome do Logradouro | 
| `3` | Street | Logradouro | 
| `4` | Number | Numero do local | 
| `8` | Complement | Complemento do local | 
| `12` | LocationInfo | Numero e complemento | 
| `15` | FullLocationInfo | Logradouro, Numero e complemento | 
| `16` | Neighborhood | Bairro | 
| `32` | City | Cidade | 
| `64` | State | Estado | 
| `96` | CityState | Cidade e Estado | 
| `128` | StateCode | UF | 
| `160` | CityStateCode | Cidade e UF | 
| `256` | Country | País | 
| `512` | CountryCode | País | 
| `1024` | PostalCode | CEP | 
| `1471` | FullAddress | Endereço completo | 


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
| `String[]` | GetAddressTypeList(`String` Endereco) |  | 
| `PropertyInfo` | GetAddressTypeProperty(`String` Endereco) |  | 


## `Brasil`

Objeto para manipular cidades e estados do Brasil
```csharp
public class InnerLibs.Locations.Brasil

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | Cities | Retorna todas as Cidades dos estados brasileiros | 
| `IEnumerable<String>` | Regions | Retorna as Regiões dos estados brasileiros | 
| `IEnumerable<State>` | States | Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressInfo` | CreateAddressInfo(`String` NameOrStateCode, `String` City) | Retorna um `InnerLibs.Locations.AddressInfo` da cidade e estado correspondentes | 
| `T` | CreateAddressInfo(`String` NameOrStateCode, `String` City) | Retorna um `InnerLibs.Locations.AddressInfo` da cidade e estado correspondentes | 
| `IEnumerable<State>` | FindStateByCity(`String` CityName) | Retorna o estado de uma cidade especifa. Pode trazer mais de um estado caso o nome da cidade seja igual em 2 ou mais estados | 
| `IEnumerable<String>` | GetCitiesOf(`String` NameOrStateCode) | Retorna as cidades de um estado a partir do nome ou sigla do estado | 
| `String` | GetClosestCity(`String` NameOrStateCode, `String` CityName) | Retorna as cidades de um estado a partir do nome ou sigla do estado | 
| `String` | GetNameOf(`String` NameOrStateCode) | Retorna o nome do estado a partir da sigla | 
| `String` | GetRegionOf(`String` NameOrStateCode) | Retorna a região a partir de um nome de estado | 
| `State` | GetState(`String` NameOrStateCode) | Retorna a as informações do estado a partir de um nome de estado ou sua sigla | 
| `String` | GetStateCodeOf(`String` NameOrStateCode) | Retorna a Sigla a partir de um nome de estado | 
| `IEnumerable<State>` | GetStatesOf(`String` Region) | Retorna os estados de uma região | 


## `State`

Objeto que representa um estado do Brasil e seus respectivos detalhes
```csharp
public class InnerLibs.Locations.State

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | Cities | Lista de cidades do estado | 
| `String` | Name | Nome do estado | 
| `String` | Region | Região do Estado | 
| `String` | StateCode | Sigla do estado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Retorna a String correspondente ao estado | 


