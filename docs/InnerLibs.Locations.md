## `AddressInfo`

```csharp
public class InnerLibs.Locations.AddressInfo

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | City |  | 
| `String` | Complement |  | 
| `String` | Country |  | 
| `String` | CountryCode |  | 
| `AddressPart` | Format |  | 
| `String` | FullAddress |  | 
| `String` | FullLocationInfo |  | 
| `String` | Item |  | 
| `Nullable<Decimal>` | Latitude |  | 
| `String` | LocationInfo |  | 
| `Nullable<Decimal>` | Longitude |  | 
| `String` | Name |  | 
| `String` | Neighborhood |  | 
| `String` | Number |  | 
| `String` | PostalCode |  | 
| `String` | Region |  | 
| `String` | State |  | 
| `String` | StateCode |  | 
| `String` | Street |  | 
| `String` | Type |  | 
| `String` | ZipCode |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` key, `String` value) |  | 
| `void` | Clear() |  | 
| `Boolean` | ContainsKey(`String` key) |  | 
| `Dictionary<String, String>` | GetDetails() |  | 
| `Point` | GetPoint() |  | 
| `String` | LatitudeLongitude() |  | 
| `Boolean` | Remove(`String` key) |  | 
| `AddressInfo` | SetLatitudeLongitudeFromPoint(`Point` Point) |  | 
| `String` | ToString() |  | 
| `String` | ToString(`IEnumerable<AddressPart>` Parts) |  | 
| `String` | ToString(`AddressPart[]` Parts) |  | 


Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressPart` | GlobalFormat |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressInfo` | CreateLocation(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) |  | 
| `T` | CreateLocation(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = ) |  | 
| `String` | FormatAddress(`String` Address, `String` Number = , `String` Complement = , `String` Neighborhood = , `String` City = , `String` State = , `String` Country = , `String` PostalCode = , `AddressPart[]` Parts) |  | 
| `String` | FormatPostalCode(`String` CEP) |  | 
| `AddressInfo` | FromGoogleMaps(`String` Address, `String` Key, `NameValueCollection` NVC = null) |  | 
| `T` | FromGoogleMaps(`String` Address, `String` Key, `NameValueCollection` NVC = null) |  | 
| `AddressInfo` | FromViaCEP(`Int32` PostalCode, `String` Number = null, `String` Complement = null) |  | 
| `AddressInfo` | FromViaCEP(`String` PostalCode, `String` Number = null, `String` Complement = null) |  | 
| `T` | FromViaCEP(`Int32` PostalCode, `String` Number = null, `String` Complement = null) |  | 
| `T` | FromViaCEP(`String` PostalCode, `String` Number = null, `String` Complement = null) |  | 
| `AddressInfo` | TryParse(`String` Address) |  | 
| `T` | TryParse(`String` Address) |  | 


## `AddressPart`

```csharp
public enum InnerLibs.Locations.AddressPart
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Default |  | 
| `1` | StreetType |  | 
| `2` | StreetName |  | 
| `3` | Street |  | 
| `4` | Number |  | 
| `8` | Complement |  | 
| `12` | LocationInfo |  | 
| `15` | FullLocationInfo |  | 
| `16` | Neighborhood |  | 
| `32` | City |  | 
| `64` | State |  | 
| `96` | CityState |  | 
| `128` | StateCode |  | 
| `160` | CityStateCode |  | 
| `256` | Country |  | 
| `512` | CountryCode |  | 
| `1024` | PostalCode |  | 
| `1471` | FullAddress |  | 


## `AddressTypes`

```csharp
public static class InnerLibs.Locations.AddressTypes

```

Static Properties

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

```csharp
public class InnerLibs.Locations.Brasil

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Reload() |  | 


Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<City>` | Cities |  | 
| `IEnumerable<String>` | Regions |  | 
| `IEnumerable<State>` | States |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AddressInfo` | CreateAddressInfo(`String` NameOrStateCode, `String` City) |  | 
| `T` | CreateAddressInfo(`String` NameOrStateCodeOrIBGE, `String` City) |  | 
| `City` | FindCityByIBGE(`Int32` IBGE) |  | 
| `IEnumerable<State>` | FindStateByCityName(`String` CityName) |  | 
| `State` | FindStateByIBGE(`Int32` IBGE) |  | 
| `City` | GetCapital(`String` NameOrStateCodeOrIBGE) |  | 
| `IEnumerable<City>` | GetCitiesOf(`String` NameOrStateCodeOrIBGE) |  | 
| `City` | GetClosestCity(`String` NameOrStateCodeOrIBGE, `String` CityName) |  | 
| `String` | GetClosestCityName(`String` NameOrStateCodeOrIBGE, `String` CityName) |  | 
| `Nullable<Int32>` | GetIBGEOf(`String` NameOrStateCodeOrIBGE) |  | 
| `String` | GetNameOf(`String` NameOrStateCodeOrIBGE) |  | 
| `String` | GetRegionOf(`String` NameOrStateCodeOrIBGE) |  | 
| `State` | GetState(`String` NameOrStateCodeOrIBGE) |  | 
| `String` | GetStateCodeOf(`String` NameOrStateCodeOrIBGE) |  | 
| `IEnumerable<State>` | GetStatesOf(`String` Region) |  | 


## `City`

```csharp
public class InnerLibs.Locations.City

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Capital |  | 
| `Int32` | DDD |  | 
| `Int32` | IBGE |  | 
| `Decimal` | Latitude |  | 
| `Decimal` | Longitude |  | 
| `String` | Name |  | 
| `String` | SIAFI |  | 
| `State` | State |  | 
| `String` | TimeZone |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `State`

```csharp
public class InnerLibs.Locations.State

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<City>` | Cities |  | 
| `String` | Country |  | 
| `String` | CountryCode |  | 
| `Int32` | IBGE |  | 
| `Decimal` | Latitude |  | 
| `Decimal` | Longitude |  | 
| `String` | Name |  | 
| `String` | Region |  | 
| `String` | StateCode |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


