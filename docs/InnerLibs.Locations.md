## `Brasil`

Objeto para manipular cidades e estados do Brasil
```csharp
public class InnerLibs.Locations.Brasil

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<Celebration>` | Celebrations | Retorna uma lista com todas as datas comemorativas do Brasil | 
| `List<State>` | States | Retorna uma lista com todos os estados do Brasil e seus respectivos detalhes | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetAcronymOf(`String` Name) | Retorna a Sigla a partir de um nome de estado | 
| `Celebration[]` | GetCelebrationBetween(`DateTime` FirstDate, `DateTime` SecondDate) | Retorna todas as comemoracoes entre 2 datas | 
| `Celebration[]` | GetCelebrationByDate(`DateTime` Date) | Retorna todas as comemoracoes de uma data | 
| `String[]` | GetCelebrationByMonth(`Int32` Month) | Retorna todas as comemoracoes de um mês | 
| `List<String>` | GetCitiesOf(`String` NameOrStateCode = ) | Retorna as cidades de um estado a partir do nome ou sigla do estado | 
| `String` | GetNameOf(`String` StateCode) | Retorna o nome do estado a partir da sigla | 
| `List<String>` | GetStateList(`StateString` Type = Name) | Retorna uma lista contendo os nomes ou siglas dos estados do Brasil | 


## `Celebration`

```csharp
public class InnerLibs.Locations.Celebration

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | Date |  | 
| `Int32` | Day |  | 
| `String` | Description |  | 
| `Int32` | Month |  | 


## `GeoIP`

Retorna a localizaçao de um IP
```csharp
public class InnerLibs.Locations.GeoIP

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | City |  | 
| `String` | CountryCode |  | 
| `String` | CountryName |  | 
| `String` | Domain |  | 
| `String` | IP |  | 
| `String` | Latitude |  | 
| `String` | Longitude |  | 
| `Int32` | MetroCode |  | 
| `String` | RegionCode |  | 
| `String` | RegionName |  | 
| `String` | TimeZone |  | 
| `String` | ZipCode |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToJSON() | Retorna uma string JSON do objeto | 
| `Location` | ToLocation() | Cria um objeto Innerlibs.Location com as informaçoes do IP | 


## `Location`

Representa um deteminado local com suas Informações
```csharp
public class InnerLibs.Locations.Location

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Address | Retorna o endereço completo | 
| `String` | City | Cidade | 
| `String` | Complement | Complemento | 
| `String` | Country | País | 
| `Uri` | GoogleMapsURL | URL do Google Maps | 
| `String` | Latitude | Coordenada geográfica LATITUDE | 
| `String` | Longitude | Coordenada geográfica LONGITUDE | 
| `String` | Neighborhood | Bairro | 
| `String` | Number | Numero da casa, predio etc. | 
| `String` | PostalCode | CEP - Codigo de Endereçamento Postal | 
| `String` | State | Estado | 
| `String` | StateCode | Unidade federativa | 
| `String` | Street | Logradouro | 
| `String` | StreetName | O nome do endereço | 
| `String` | StreetType | Tipo do Endereço | 
| `String` | ZipCode | CEP - Codigo de Endereçamento Postal. Alias de `InnerLibs.Locations.Location.PostalCode` | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | GetInfoByPostalCode() | Retorna o endereço de acordo com o CEP contidos em uma variavel do tipo InnerLibs.Location usando a API https://viacep.com.br/ | 
| `String` | LatitudeLongitude() | Retorna as coordenadas geográficas do Local | 
| `void` | SearchOnGoogleMaps(`String` Location, `Boolean` Sensor = True) | Realiza uma busca detalhada no google Maps | 
| `String` | ToJSON() |  | 
| `String` | ToString() | Retorna uma String contendo as informações do Local | 
| `void` | Update() | Realiza uma nova busca no google maps usando o endereço completo | 


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


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Retorna a String correspondente ao estado | 
| `String` | ToString(`StateString` Type = Acronym) | Retorna a String correspondente ao estado | 


