## `Alphabet`

```csharp
public class InnerLibs.Alphabet

```

Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Alphabet |  | 
| `Int32` | Base |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Decode(`String` s) |  | 
| `String` | Encode(`Int32` i) |  | 


## `Base64`

Modulo para manipulação de imagens e Strings Base64
```csharp
public class InnerLibs.Base64

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Atob(this `String` Base, `Encoding` Encoding = null) | Decoda uma string em Base64 | 
| `String` | Btoa(this `String` Text, `Encoding` Encoding = null) | Encoda uma string em Base64 | 
| `FileInfo` | CreateFileFromDataURL(this `String` Base64StringOrDataURL, `String` FilePath) | Cria um arquivo fisico a partir de uma Base64 ou DataURL | 
| `String` | FixBase64(this `String` Base64StringOrDataUrl) | Arruma os caracteres de uma string Base64 | 
| `Boolean` | IsDataURL(this `String` Text) | Retorna TRUE se o texto for um dataurl valido | 
| `String` | ToBase64(this `Byte[]` Bytes) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `Image` OriginalImage, `ImageFormat` OriginalImageFormat) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `Image` OriginalImage) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `Uri` ImageURL) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `String` ImageURL, `ImageFormat` OriginalImageFormat) | Converte um Array de Bytes em uma string Base64 | 
| `Byte[]` | ToBytes(this `String` Base64StringOrDataURL) | Converte uma DATAURL ou Base64 String em um array de Bytes | 
| `String` | ToDataURL(this `Byte[]` Bytes, `FileType` Type = null) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `Byte[]` Bytes, `String` MimeType) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `FileInfo` File) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `Image` Image) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `Image` OriginalImage, `ImageFormat` OriginalImageFormat) | Converte um Array de Bytes em uma DATA URL Completa | 
| `Image` | ToImage(this `String` DataUrlOrBase64String, `Int32` Width = 0, `Int32` Height = 0) | Converte uma String DataURL ou Base64 para Imagem | 
| `Image` | ToImage(this `Byte[]` Bytes) | Converte uma String DataURL ou Base64 para Imagem | 


## `Calendars`

Modulo para manipulação de calendário
```csharp
public class InnerLibs.Calendars

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | BrazilianNow |  | 
| `DateTime` | BrazilianTomorrow |  | 
| `DateTime` | BrazilianYesterday |  | 
| `String` | Farewell | Retorna uma despedida | 
| `String` | Greeting | Retorna uma saudação | 
| `DateTime` | LastDay |  | 
| `Object` | LastSunday | Retorna o ultimo domingo | 
| `List<KeyValuePair<String, String>>` | Months | Returna uma lista dupla com os meses | 
| `DateTime` | NextDay |  | 
| `DateTime` | NextSunday | Retorna o proximo domingo | 
| `DateTime` | Tomorrow | Retorna a data de amanhã | 
| `List<KeyValuePair<String, String>>` | WeekDays | Returna uma lista dupla com os meses | 
| `DateTime` | Yesterday | Retorna a data de ontem | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | CalculatePercent(this `DateTime` MidDate, `DateTime` StartDate, `DateTime` EndDate) | Calcula a porcentagem de diferenca entre duas datas de acordo com a data inicial especificada | 
| `String` | ChangeFormat(this `String` DateString, `String` InputFormat, `String` OutputFormat, `CultureInfo` Culture = null) | Converte uma string de data para outra string de data com formato diferente | 
| `List<DateTime>` | ClearTime(this `List<DateTime>` List) | Remove o tempo de todas as datas de uma lista e retorna uma nova lista | 
| `DateTime` | ConvertDateString(this `String` DateString, `String` Format, `CultureInfo` Culture = null) | Converte uma string em datetime a partir de um formato especifico | 
| `void` | FixDateOrder(`DateTime&` StartDate, `DateTime&` EndDate) | Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate  sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um  `System.TimeSpan` negativo | 
| `Int32` | GetAge(this `DateTime` BirthDate, `Nullable<DateTime>` FromDate = null) | Retorna a idade | 
| `IEnumerable<DateTime>` | GetBetween(this `DateTime` StartDate, `DateTime` EndDate, `DayOfWeek[]` DaysOfWeek) | Retorna as datas entre um periodo | 
| `TimeFlow` | GetDifference(this `DateTime` InitialDate, `DateTime` SecondDate) | Retorna uma `InnerLibs.TimeMachine.TimeFlow` com a diferença entre 2 Datas | 
| `Int32` | GetDoubleMonthOfYear(this `DateTime` DateAndtime) | Pega o numero do Bimestre a partir de uma data | 
| `DateTime` | GetFirstDayOfDouleMonth(this `DateTime` Date) | Retorna o ultimo dia de um bimestre a partir da data | 
| `DateTime` | GetFirstDayOfFortnight(this `DateTime` Date) | Retorna a primeira data da quinzena a partir de uma outra data | 
| `DateTime` | GetFirstDayOfHalf(this `DateTime` Date) | Retorna o prmeiro dia de um semestre a partir da data | 
| `DateTime` | GetFirstDayOfMonth(this `Int32` MonthNumber, `Nullable<Int32>` Year = null) | Retorna a ultima data do mes a partir de uma outra data | 
| `DateTime` | GetFirstDayOfMonth(this `DateTime` Date) | Retorna a ultima data do mes a partir de uma outra data | 
| `DateTime` | GetFirstDayOfQuarter(this `DateTime` Date) | Retorna o ultimo dia de um trimestre a partir da data | 
| `DateTime` | GetFirstDayOfWeek(this `DateTime` Date, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna o primeiro dia da semana da data especificada | 
| `DateTime` | GetFirstDayOfYear(this `DateTime` Date) | Retorna o prmeiro dia de um ano especifico de outra data | 
| `Int32` | GetHalfOfYear(this `DateTime` DateAndTime) | Pega o numero do semestre a partir de uma data | 
| `DateTime` | GetLastDayOfDoubleMonth(this `DateTime` Date) | Retorna o ultimo dia de um bimestre a partir da data | 
| `DateTime` | GetLastDayOfFortnight(this `DateTime` Date) | Retorna a ultima data da quinzena a partir de uma outra data | 
| `DateTime` | GetLastDayOfHalf(this `DateTime` Date) | Retorna o ultimo dia de um semestre a partir da data | 
| `DateTime` | GetLastDayOfMonth(this `DateTime` Date) | Retorna a ultima data do mes a partir de uma outra data | 
| `DateTime` | GetLastDayOfMonth(this `Int32` MonthNumber, `Nullable<Int32>` Year = null) | Retorna a ultima data do mes a partir de uma outra data | 
| `DateTime` | GetLastDayOfQuarter(this `DateTime` Date) | Retorna o ultimo dia de um trimestre a partir da data | 
| `DateTime` | GetLastDayOfWeek(this `DateTime` Date, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna o ultimo dia da semana da data especificada | 
| `DateTime` | GetLastDayOfYear(this `DateTime` Date) | Retorna o ultimo dia de um ano especifico de outra data | 
| `String` | GetLongMonthName(this `DateTime` Date, `CultureInfo` Culture = null) | Retorna o nome do mês a partir da data | 
| `Int32` | GetQuarterOfYear(this `DateTime` DateAndTime) | Pega o numero do trimestre a partir de uma data | 
| `String` | GetShortMonthName(this `DateTime` Date, `CultureInfo` Culture = null) | Retorna o nome do mês a partir da data | 
| `DateRange` | GetWeek(this `DateTime` Date, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna um DateRange equivalente a semana de uma data especifica | 
| `Int32[]` | GetWeekInfoOfMonth(this `DateTime` DateAndTime) | Pega o numero da semana, do mês e ano pertencente | 
| `Int32` | GetWeekNumberOfMonth(this `DateTime` DateAndTime) | Pega o numero da semana a partir de uma data | 
| `Int32` | GetWeekOfYear(this `DateTime` Date, `CultureInfo` Culture = null, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna o numero da semana relativa ao ano | 
| `Boolean` | IsAnniversary(this `DateTime` BirthDate, `Nullable<DateTime>` CompareWith = null) | Verifica se a Data de hoje é um aniversário | 
| `Boolean` | IsBetween(this `DateTime` MidDate, `DateTime` StartDate, `DateTime` EndDate, `Boolean` IgnoreTime = False) | Verifica se uma data se encontra entre 2 datas | 
| `Boolean` | IsSameMonthAndYear(this `DateTime` Date, `DateTime` AnotherDate) | Verifica se uma data é do mesmo mês e ano que outra data | 
| `Boolean` | IsWeekend(this `DateTime` YourDate) | Verifica se o dia se encontra no fim de semana | 
| `DateTime` | NextFortnight(this `DateTime` FromDate, `Int32` Num = 1) | Pula para a data inicial da proxima quinzena | 
| `String` | ToFarewell(this `DateTime` Time, `String` Language = pt) | Transforma um DateTime em uma despedida (Bom dia, Boa tarde, Boa noite) | 
| `String` | ToGreeting(this `DateTime` Time, `String` Language = pt) | Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite) | 
| `String` | ToLongDayOfWeekName(this `Int32` DayNumber) | Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Segunda-Feira | 
| `String` | ToLongMonthName(this `Int32` MonthNumber) | Retorna uma String baseado no numero do Mês Ex.: 1 -&gt; Janeiro | 
| `String` | ToLongMonthName(this `DateTime` DateTime) | Retorna uma String baseado no numero do Mês Ex.: 1 -&gt; Janeiro | 
| `String` | ToShortDayOfWeekName(this `Int32` DayNumber) | Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Seg | 
| `String` | ToShortMonthName(this `Int32` MonthNumber) | Retorna uma String curta baseado no numero do Mês Ex.: 1 -&gt; Jan | 
| `String` | ToSQLDateString(this `DateTime` Date) | COnverte um datetime para o formato de string do SQL server ou Mysql | 
| `String` | ToSQLDateString(this `String` Date, `String` FromCulture = pt-BR) | COnverte um datetime para o formato de string do SQL server ou Mysql | 
| `String` | ToTimeElapsedString(this `TimeSpan` TimeElapsed, `String` DayWord = dia, `String` HourWord = hora, `String` MinuteWord = minuto, `String` SecondWord = segundo) | Retorna uma String no formato "W dias, X horas, Y minutos e Z segundos" | 
| `DateTime` | ToTimeZone(this `DateTime` Date, `String` TimeZoneId) | Converte um `System.DateTime` para um timezone Especifico | 
| `DateTime` | ToTimeZoneUtc(this `DateTime` Date, `TimeZoneInfo` TimeZone) | Converte um `System.DateTime` para um timezone Especifico | 


## `Class1`

```csharp
public class InnerLibs.Class1

```

## `ClassTools`

```csharp
public class InnerLibs.ClassTools

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `T` | AsIf(this `Boolean` Bool, `T` TrueValue, `T` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `T` | AsIf(this `Nullable<Boolean>` Bool, `T` TrueValue, `T` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `String` | BlankCoalesce(this `String` First, `String[]` N) | Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que  possuir um valor | 
| `String` | BlankCoalesce(`String[]` N) | Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que  possuir um valor | 
| `T` | Choose(this `Boolean` BooleanValue, `T` ChooseIfTrue, `T` ChooseIfFalse) | Escolhe um valor de acordo com o resultado de uma variavel booliana | 
| `T` | Choose(this `String` Expression, `T` ChooseIfTrue, `T` ChooseIfFalse) | Escolhe um valor de acordo com o resultado de uma variavel booliana | 
| `Boolean` | ContainsAll(this `IEnumerable<Type>` List1, `IEnumerable<Type>` List2, `IEqualityComparer<Type>` Comparer = null) | Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array. | 
| `Boolean` | ContainsAny(this `IEnumerable<Type>` List1, `IEnumerable<Type>` List2, `IEqualityComparer<Type>` Comparer = null) | Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array. | 
| `Dictionary<String, Object>` | CreateDictionary(this `Type` Obj) | Converte uma classe para um `System.Collections.Generic.Dictionary`2` | 
| `Guid` | CreateGuidOrDefault(this `String` Source) | Cria um `System.Guid` a partir de uma string ou um novo `System.Guid` se a conversão falhar | 
| `T` | Detach(this `List<T>` List, `Int32` Index) | Remove um item de uma lista e retorna este item | 
| `Object` | DictionaryToObject(this `IDictionary` dict) |  | 
| `Dictionary<Type, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr) | Conta de maneira distinta items de uma coleçao | 
| `Dictionary<PropT, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop) | Conta de maneira distinta items de uma coleçao | 
| `Dictionary<Type, Int64>` | DistinctCountTop(this `IEnumerable<Type>` Arr, `Int32` Top, `Type` Others) | Conta de maneira distinta N items de uma coleçao e agrupa o resto | 
| `Dictionary<PropT, Int64>` | DistinctCountTop(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop, `Int32` Top, `PropT` Others) | Conta de maneira distinta N items de uma coleçao e agrupa o resto | 
| `T` | FirstAny(this `IEnumerable<T>` source, `Func`2[]` predicate) | O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista | 
| `T` | FirstAnyOr(this `IEnumerable<T>` source, `T` Alternate, `Func`2[]` predicate) | O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista | 
| `T` | FirstOr(this `IEnumerable<T>` source, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `T` | FirstOr(this `IEnumerable<T>` source, `Func<T, Boolean>` predicate, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `TValue` | GetAttributeValue(this `Type` type, `Func<TAttribute, TValue>` ValueSelector) |  | 
| `T` | GetEnumValue(`String` Name) | Traz o valor de uma enumeração a partir de uma string | 
| `String` | GetEnumValueAsString(this `T` Value) | Traz o valor de uma enumeração a partir de uma string | 
| `List<T>` | GetEnumValues() | Traz todos os Valores de uma enumeração | 
| `List<PropertyInfo>` | GetProperties(this `Object` MyObject, `BindingFlags` BindAttr) | Traz uma Lista com todas as propriedades de um objeto | 
| `List<PropertyInfo>` | GetProperties(this `Object` MyObject) | Traz uma Lista com todas as propriedades de um objeto | 
| `PropertyInfo` | GetProperty(this `Object` MyObject, `String` Name) | Traz uma propriedade de um objeto | 
| `Object[]` | GetPropertyParameterFromString(this `String` Text) | Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe | 
| `Object[]` | GetPropertyParametersFromString(this `Type` Type, `String` Text) | Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe | 
| `T` | GetPropertyValue(this `Object` MyObject, `String` Name) | Traz uma propriedade de um objeto | 
| `Byte[]` | GetResourceBytes(this `Assembly` Assembly, `String` FileName) | Pega os bytes de um arquivo embutido no assembly | 
| `String` | GetResourceFileText(this `Assembly` Assembly, `String` FileName) | Pega o texto de um arquivo embutido no assembly | 
| `Tvalue` | GetValueOr(this `IDictionary<tkey, Tvalue>` Dic, `tkey` Key, `Tvalue` ReplaceValue = null) |  | 
| `Dictionary<Group, Int64>` | GroupAndCountBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector) | Agrupa e conta os itens de uma lista a partir de uma propriedade | 
| `Dictionary<Group, Dictionary<Count, Int64>>` | GroupAndCountSubGroupBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector, `Func<Type, Count>` CountObjectBy) | Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada grupo a partir de outra propriedade do mesmo objeto | 
| `Dictionary<Group, Dictionary<SubGroup, IEnumerable<Type>>>` | GroupAndSubGroupBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector, `Func<Type, SubGroup>` SubGroupSelector) | Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um grupo com subgrupos daquele objeto | 
| `Dictionary<Group, Int64>` | GroupFirstAndCountBy(this `IEnumerable<Type>` obj, `Int32` First, `Func<Type, Group>` GroupSelector, `Group` OtherLabel) | Agrupa e conta os itens de uma lista a partir de uma propriedade | 
| `Object` | HasAttribute(this `PropertyInfo` target, `Type` attribType) | Verifica se um atributo foi definido em uma propriedade de uma classe | 
| `Object` | HasAttribute(this `PropertyInfo` target) | Verifica se um atributo foi definido em uma propriedade de uma classe | 
| `Boolean` | HasProperty(this `Type` Type, `String` PropertyName, `Boolean` GetPrivate = False) | Verifica se um tipo possui uma propriedade | 
| `Boolean` | HasProperty(this `Object` Obj, `String` Name) | Verifica se um tipo possui uma propriedade | 
| `Boolean` | IsArrayOf(this `Type` Type) | Verifica se o tipo é um array de um objeto especifico | 
| `Boolean` | IsArrayOf(this `Object` Obj) | Verifica se o tipo é um array de um objeto especifico | 
| `Boolean` | IsDictionary(this `Object` obj) | Verifica se o objeto é um iDictionary | 
| `Boolean` | IsIn(this `Type` Obj, `Type[]` List) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsIn(this `Type` Obj, `String` Text, `IEqualityComparer<Char>` Comparer = null) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsInAny(this `Type` Obj, `IEnumerable`1[]` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays. | 
| `Boolean` | IsList(this `Object` obj) | Verifica se o objeto é uma lista | 
| `Boolean` | IsNotIn(this `Type` Obj, `String` Text, `IEqualityComparer<Char>` Comparer = null) | Verifica se o não objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsNotIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o não objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsNumericType(this `Type` Obj) | Verifica se o objeto é do tipo numérico. | 
| `Boolean` | IsType(this `Object` Obj) | Verifica se um objeto é de um determinado tipo | 
| `T` | LastOr(this `IEnumerable<T>` source, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `List<T>` | Map(this `DbDataReader` Reader, `Object[]` Params) | Mapeia os objetos de um datareader para uma classe | 
| `NameValueCollection` | Merge(`NameValueCollection[]` NVC) | Mescla varios `System.Collections.Specialized.NameValueCollection` em um unico `System.Collections.Specialized.NameValueCollection` | 
| `T` | NullCoalesce(this `Nullable<T>` First, `Nullable`1[]` N) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `IEnumerable<Nullable<T>>` List) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `T` First, `T[]` N) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `IEnumerable<T>` List) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `Boolean` | OnlyOneOf(this `IEnumerable<Type>` List, `Func<Type, Boolean>` predicate) | Verifica se somente um unico elemento corresponde a condição | 
| `String[]` | ParamSplit(this `String` Text) |  | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `TKey[]` Keys) | Remove de um dicionario as respectivas Keys se as mesmas existirem | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `Func<KeyValuePair<TKey, TValue>, Boolean>` predicate) | Remove de um dicionario as respectivas Keys se as mesmas existirem | 
| `void` | SetPropertyValue(this `Object` MyObject, `String` PropertyName, `Type` Value) | Seta o valor de uma propriedade de um objeto | 
| `void` | SetPropertyValueFromCollection(this `Object` MyObject, `String` PropertyName, `CollectionBase` Collection) |  | 
| `String` | ToFullExceptionString(this `Exception` ex) | Concatena todas as  `System.Exception.InnerException` em uma única string | 
| `String` | ToQueryString(this `Dictionary<String, String>` Dic) | Retorna um dicionário em QueryString | 
| `String` | ToQueryString(this `NameValueCollection` NVC) | Retorna um dicionário em QueryString | 
| `IEnumerable<Object>` | ToTableArray(this `Dictionary<GroupKey, Dictionary<SubGroupKey, SubGroupValue>>` Groups, `Func<SubGroupKey, HeaderProperty>` HeaderProp) | Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays | 
| `Object` | ToTableArray(this `Dictionary<GroupKeyType, GroupValueType>` Groups) | Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays | 


## `ColorConvert`

Modulo de Conversão de Cores
```csharp
public class InnerLibs.ColorConvert

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Color` | GenerateColor(this `String` Text) | Gera uma cor de acordo com um texto | 
| `Color` | GetContrastColor(this `Color` TheColor, `Single` Percent = 0,7) | Retorna uma cor de contraste baseado na iluminacao da primeira cor: Uma cor clara se a primeira for escura. Uma cor escura se a primeira for clara | 
| `Color` | GetNegativeColor(this `Color` TheColor) | Retorna  a cor negativa de uma cor | 
| `Boolean` | IsDark(this `Color` TheColor) | Verifica se uma cor é escura | 
| `Boolean` | IsLight(this `Color` TheColor) | Verifica se uma clor é clara | 
| `Color` | Lerp(this `Color` TheColor, `Color` to, `Single` amount) | Mescla duas cores usando Lerp | 
| `Color` | MakeDarker(this `Color` TheColor, `Single` percent = 0,6) | Escurece a cor mesclando ela com preto | 
| `Color` | MakeLighter(this `Color` TheColor, `Single` percent = 0,6) | Clareia a cor mistuando ela com branco | 
| `Color` | MergeWith(this `Color` TheColor, `Color` AnotherColor, `Single` Percent = 0,6) | Mescal duas cores a partir de uma porcentagem | 
| `Color` | ToColor(this `String` HexadecimalColorString) | Converte uma string hexadecimal (HTML) para objeto Color | 
| `String` | ToHexadecimal(this `Color` Color, `Boolean` Hash = True) | Converte uma cor de sistema para hexadecimal | 
| `String` | ToRGB(this `Color` Color) | Converte uma cor de sistema para CSS RGB | 
| `String` | ToRGBA(this `Color` Color) |  | 


## `ColorEx`

```csharp
public class InnerLibs.ColorEx

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int16` | MaxHue |  | 
| `Int16` | MaxSaturation |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Byte` | B |  | 
| `Color` | Color |  | 
| `Byte` | G |  | 
| `Int16` | H |  | 
| `Byte` | R |  | 
| `Byte` | S |  | 
| `Byte` | V |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ColorEx` | Clone() |  | 


## `Converter`

```csharp
public class InnerLibs.Converter

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ToType[]` | ChangeArrayType(this `FromType[]` Value) | Converte um array de um tipo para outro | 
| `IEnumerable<ToType>` | ChangeIEnumerableType(this `IEnumerable<FromType>` Value) | Converte um IEnumerable de um tipo para outro | 
| `ToType` | ChangeType(this `FromType` Value) | Converte um tipo para outro. Retorna Nothing (NULL) se a conversão falhar | 
| `List<T>` | DefineEmptyList(this `T` ObjectForDefinition) | Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos | 
| `Object[]` | ForceArray(`Object` Obj) | Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto | 
| `OutputType[]` | ForceArray(`Object` Obj) | Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto | 
| `Dictionary<Tkey, Object>` | Merge(this `Dictionary<Tkey, Object>` FirstDictionary, `Dictionary`2[]` Dictionaries) | Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um dicionario os valores sao agrupados em arrays | 
| `void` | SetPropertiesIn(this `IDictionary<String, Object>` Dic, `T` Obj) | Seta as propriedades de uma classe a partir de um dictionary | 
| `Boolean` | ToBoolean(this `FromType` Value) | Converte um tipo para Boolean. Retorna Nothing (NULL) se a conversão falhar | 
| `DateTime` | ToDateTime(this `FromType` Value) | Converte um tipo para DateTime. Retorna Nothing (NULL) se a conversão falhar | 
| `DateTime` | ToDateTime(this `FromType` Value, `String` CultureInfoName) | Converte um tipo para DateTime. Retorna Nothing (NULL) se a conversão falhar | 
| `DateTime` | ToDateTime(this `FromType` Value, `CultureInfo` CultureInfo) | Converte um tipo para DateTime. Retorna Nothing (NULL) se a conversão falhar | 
| `Decimal` | ToDecimal(this `FromType` Value) | Converte um tipo para Decimal. Retorna Nothing (NULL) se a conversão falhar | 
| `Dictionary<TKey, IEnumerable<TValue>>` | ToDictionary(this `IEnumerable<IGrouping<TKey, TValue>>` groupings) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `Dictionary<TKey, TValue>` | ToDictionary(this `IEnumerable<KeyValuePair<TKey, TValue>>` items) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `Dictionary<String, Object>` | ToDictionary(this `NameValueCollection` NameValueCollection, `String[]` Keys) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `Double` | ToDouble(this `FromType` Value) | Converte um tipo para Double. Retorna Nothing (NULL) se a conversão falhar | 
| `Int32` | ToInteger(this `FromType` Value) | Converte um tipo para Integer. Retorna Nothing (NULL) se a conversão falhar | 
| `Int64` | ToLong(this `FromType` Value) | Converte um tipo para Integer. Retorna Nothing (NULL) se a conversão falhar | 
| `IEnumerable<Dictionary<TKey, TValue>>` | Uniform(this `IEnumerable`1&` Dics, `TKey[]` AditionalKeys) | Aplica as mesmas keys a todos os dicionarios de uma lista | 


## `DataURI`

Classe para Extrair informaçoes de uma DATAURL
```csharp
public class InnerLibs.DataURI

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Data | String Base64 ou Base32 | 
| `String` | Encoding | Tipo de encoding (32 ou 64) | 
| `String` | Extension | Extensão do tipo do arquivo | 
| `String` | FullMimeType | MIME type completo | 
| `String` | Mime | Tipo do arquivo encontrado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Byte[]` | ToBytes() | Converte esta dataURI em Bytes() | 
| `FileType` | ToFileType() | Informaçoes referentes ao tipo do arquivo | 
| `String` | ToString() | Retorna uma string da dataURL | 
| `FileInfo` | WriteToFile(`String` Path) | Transforma este datauri em arquivo | 


## `DateRange`

Classe que representa um periodo entre 2 datas
```csharp
public class InnerLibs.DateRange

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | EndDate |  | 
| `Boolean` | ForceFirstAndLastMoments | Se true, ajusta as horas de `InnerLibs.DateRange.StartDate` para o primeiro momento do dia e as horas de `InnerLibs.DateRange.EndDate` para o último momento do dia | 
| `DateTime` | StartDate |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | CalculatePercent(`Nullable<DateTime>` Date = null) | Verifica quantos porcento uma data representa  em distancia dentro deste periodo | 
| `Boolean` | Contains(`DateRange` Period) | Verifica se este periodo contém um outro periodo | 
| `Boolean` | Contains(`DateTime` Day) | Verifica se este periodo contém um outro periodo | 
| `FortnightGroup` | CreateFortnightGroup() | Cria um grupo de quinzenas que contenham este periodo | 
| `TimeFlow` | Difference() | Retorna um `InnerLibs.TimeMachine.TimeFlow` contendo a diferença entre as datas | 
| `IEnumerable<T>` | Filter(`IEnumerable<T>` List, `Func<T, DateTime>` Selector) | Filtra uma lista a partir de um seletor de data trazendo apenas os valores dete periodo | 
| `IEnumerable<T>` | Filter(`Func<T, Nullable<DateTime>>` Selector, `IEnumerable<T>` List) | Filtra uma lista a partir de um seletor de data trazendo apenas os valores dete periodo | 
| `DateRangeInterval` | GetLessAccurateDateRangeInterval() | Retorna o `InnerLibs.DateRangeInterval` menos preciso para calcular periodos | 
| `Decimal` | GetPeriodAs(`DateRangeInterval` DateRangeInterval = LessAccurate) | Retorna o periodo em um total especificado por `InnerLibs.DateRangeInterval` | 
| `Boolean` | IsDefaultDateRange() | Indica se este `InnerLibs.DateRange` foi construido sem nenhuma data definida | 
| `Boolean` | IsIn(`DateRange` Period) | Verifica se este periodo está dentro de outro periodo | 
| `Boolean` | IsNow() | Verifica se hoje está dentro deste periodo | 
| `Boolean` | IsSingleDate() | Retorna TRUE se a data de inicio e fim for a mesma | 
| `Boolean` | IsSingleDateTime() | Retorna TRUE se a data e hora de inicio e fim for a mesma | 
| `Boolean` | MatchAny(`DateRange` Period) | Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou contém um periodo) | 
| `DateRange` | MovePeriod(`DateRangeInterval` DateRangeInterval, `Decimal` Total) | Move um poeriodo a partir de um `` especificado por `` | 
| `DateRange` | NextPeriod(`DateRangeInterval` DateRangeInterval = LessAccurate) | Move para ao proximo periodo equivalente | 
| `Boolean` | Overlaps(`DateRange` Period) | Verifica se 2 periodos possuem interseção de datas | 
| `DateRange` | PreviousPeriod(`DateRangeInterval` DateRangeInterval = LessAccurate) | Move para o periodo equivalente anterior | 
| `String` | ToString() | Retorna uma strin representando a diferença das datas | 


## `DateRange<DataType>`

```csharp
public class InnerLibs.DateRange<DataType>
    : DateRange

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<DataType>` | DataCollection |  | 
| `List<Func<DataType, DateTime>>` | DateSelector |  | 


## `DateRangeInterval`

```csharp
public enum InnerLibs.DateRangeInterval
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `-1` | LessAccurate |  | 
| `0` | Milliseconds |  | 
| `1` | Seconds |  | 
| `2` | Minutes |  | 
| `3` | Hours |  | 
| `4` | Days |  | 
| `5` | Weeks |  | 
| `6` | Months |  | 
| `7` | Years |  | 


## `Directories`

Funções para trabalhar com diretorios
```csharp
public class InnerLibs.Directories

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | CleanDirectory(this `DirectoryInfo` TopDirectory, `Boolean` DeleteTopDirectoryIfEmpty = True) | Remove todos os subdiretorios vazios | 
| `List<FileInfo>` | CopyTo(this `List<FileInfo>` List, `DirectoryInfo` DestinationDirectory) | Copia arquivos para dentro de outro diretório | 
| `Boolean` | DeleteIfExist(this `String` Path) | Deleta um arquivo ou diretório se o mesmo existir e retorna TURE se o arquivo puder ser criado novamente | 
| `Boolean` | DeleteIfExist(this `FileSystemInfo` Path) | Deleta um arquivo ou diretório se o mesmo existir e retorna TURE se o arquivo puder ser criado novamente | 
| `DirectoryInfo` | ExtractZipFile(this `FileInfo` File, `DirectoryInfo` Directory) | Extrai um arquivo zip em um diretório | 
| `IEnumerable<FindType>` | Find(this `DirectoryInfo` Directory, `Func<FindType, Boolean>` predicate, `SearchOption` SearchOption = AllDirectories) | Retorna uma lista de arquivos ou diretórios baseado em uma busca usando predicate | 
| `String` | FixPathSeparator(this `String` Path, `Boolean` Alternative = False) | Ajusta um caminho de arquivo ou diretório colocando o mesmo `System.IO.Path.DirectorySeparatorChar` evitando barras duplas ou alternativas | 
| `Boolean` | HasDirectories(this `DirectoryInfo` Directory) | Verifica se um diretório possui subdiretórios | 
| `Boolean` | HasFiles(this `DirectoryInfo` Directory) | Verifica se um diretório possui arquivos | 
| `Boolean` | IsEmpty(this `DirectoryInfo` Directory) | Verifica se um diretório está vazio | 
| `Boolean` | IsNotEmpty(this `DirectoryInfo` Directory) | Verifica se um diretório não está vazio | 
| `List<FileSystemInfo>` | Search(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas | 
| `List<FileSystemInfo>` | SearchBetween(this `DirectoryInfo` Directory, `DateTime` FirstDate, `DateTime` SecondDate, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas | 
| `List<DirectoryInfo>` | SearchDirectories(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de diretórios baseado em um ou mais padrões de pesquisas | 
| `List<DirectoryInfo>` | SearchDirectoriesBetween(this `DirectoryInfo` Directory, `DateTime` FirstDate, `DateTime` SecondDate, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas | 
| `List<FileInfo>` | SearchFiles(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas | 
| `List<FileInfo>` | SearchFilesBetween(this `DirectoryInfo` Directory, `DateTime` FirstDate, `DateTime` SecondDate, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas | 
| `DirectoryInfo` | ToDirectoryInfo(this `String` DirectoryName) | Cria um diretório se o mesmo nao existir e retorna um DirectoryInfo deste diretório | 
| `FileInfo` | ToFileInfo(this `String` FileName, `FileType` Type) | Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo | 
| `FileInfo` | ToFileInfo(this `String` FileName) | Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo | 
| `FileInfo` | ToZipFile(this `DirectoryInfo` FilesDirectory, `String` OutputFile, `CompressionLevel` CompressionLevel = Optimal) | Cria um arquivo .ZIP de um diretório | 
| `FileInfo` | ToZipFile(this `DirectoryInfo` FilesDirectory, `String` OutputFile, `CompressionLevel` CompressionLevel, `SearchOption` SearchOption, `String[]` Searches) | Cria um arquivo .ZIP de um diretório | 


## `Encryption`

Modulo de Criptografia
```csharp
public class InnerLibs.Encryption

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Decrypt(this `String` Text, `String` Key = null) | Descriptografa uma string | 
| `String` | Decrypt(this `String` text, `String` Key, `String` IV) | Descriptografa uma string | 
| `String` | DecryptRSA(this `String` Text, `String` Key) | Descriptografa uma string encriptada em RSA | 
| `String` | Encrypt(this `String` Text, `String` Key = null) | Criptografa uma string | 
| `String` | Encrypt(this `String` text, `String` Key, `String` IV) | Criptografa uma string | 
| `String` | EncryptRSA(this `String` Text, `String` Key) | Criptografa um string em RSA | 
| `String` | ToMD5String(this `String` Text) | Criptografa um Texto em MD5 | 


## `eParserSyntax`

```csharp
public enum InnerLibs.eParserSyntax
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | cSharp |  | 
| `1` | Vb |  | 


## `EquationPair`

Representa um Par X,Y para operaçoes matemáticas
```csharp
public class InnerLibs.EquationPair

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Nullable<Decimal>` | X |  | 
| `Nullable<Decimal>` | Y |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Nullable`1[]` | ToArray() |  | 


## `eTokenType`

```csharp
public enum InnerLibs.eTokenType
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | none |  | 
| `1` | end_of_formula |  | 
| `2` | operator_plus |  | 
| `3` | operator_minus |  | 
| `4` | operator_mul |  | 
| `5` | operator_div |  | 
| `6` | operator_percent |  | 
| `7` | open_parenthesis |  | 
| `8` | comma |  | 
| `9` | dot |  | 
| `10` | close_parenthesis |  | 
| `11` | operator_ne |  | 
| `12` | operator_gt |  | 
| `13` | operator_ge |  | 
| `14` | operator_eq |  | 
| `15` | operator_le |  | 
| `16` | operator_lt |  | 
| `17` | operator_and |  | 
| `18` | operator_or |  | 
| `19` | operator_not |  | 
| `20` | operator_concat |  | 
| `21` | operator_if |  | 
| `22` | operator_like |  | 
| `23` | operator_contains |  | 
| `24` | operator_in |  | 
| `25` | value_identifier |  | 
| `26` | value_true |  | 
| `27` | value_false |  | 
| `28` | value_number |  | 
| `29` | value_string |  | 
| `30` | value_date |  | 
| `31` | open_bracket |  | 
| `32` | close_bracket |  | 
| `33` | open_array |  | 
| `34` | close_array |  | 


## `EvalType`

```csharp
public enum InnerLibs.EvalType
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Unknown |  | 
| `1` | Number |  | 
| `2` | Boolean |  | 
| `3` | String |  | 
| `4` | Date |  | 
| `5` | Object |  | 
| `6` | Array |  | 


## `Evaluator`

```csharp
public class InnerLibs.Evaluator

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | CaseSensitive |  | 
| `Boolean` | RaiseVariableNotFoundException |  | 
| `eParserSyntax` | Syntax |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | AddEnvironmentFunctions(`Object` obj) |  | 
| `opCode` | Parse(`String` str) |  | 
| `void` | RemoveEnvironmentFunctions(`Object` obj) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ConvertToString(`Object` value) |  | 


## `EvalVariable`

```csharp
public class InnerLibs.EvalVariable
    : iEvalTypedValue, iEvalValue, iEvalHasDescription

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Description |  | 
| `EvalType` | EvalType |  | 
| `Object` | iEvalTypedValue_value |  | 
| `String` | Name |  | 
| `Type` | systemType |  | 
| `Object` | Value |  | 


Events

| Type | Name | Summary | 
| --- | --- | --- | 
| `ValueChangedEventHandler` | ValueChanged |  | 


## `Files`

Módulo para criação de arquivos baseados em Array de Bytes()
```csharp
public class InnerLibs.Files

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetLatestDirectoryName(this `FileInfo` Path) | Retorna o nome do diretorio onde o arquivo se encontra | 
| `String` | ReadText(this `FileInfo` File) | Retorna o conteudo de um arquivo de texto | 
| `FileInfo` | SaveMailAttachment(this `Attachment` attachment, `DirectoryInfo` Directory) | Salva um anexo para um diretório | 
| `FileInfo` | SaveMailAttachment(this `Attachment` attachment, `String` Path) | Salva um anexo para um diretório | 
| `Byte[]` | ToBytes(this `Attachment` attachment) | Salva um anexo para Byte() | 
| `Byte[]` | ToBytes(this `Stream` stream) | Salva um anexo para Byte() | 
| `Byte[]` | ToBytes(this `FileInfo` File) | Salva um anexo para Byte() | 
| `FileInfo` | WriteToFile(this `Byte[]` Bytes, `String` FilePath) | Transforma um  Array de Bytes em um arquivo | 
| `FileInfo` | WriteToFile(this `String` Text, `String` FilePath, `Boolean` Append = False) | Transforma um  Array de Bytes em um arquivo | 
| `FileInfo` | WriteToFile(this `String` Text, `FileInfo` File, `Boolean` Append = False) | Transforma um  Array de Bytes em um arquivo | 


## `FileTree`

```csharp
public class InnerLibs.FileTree

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<FileTree>` | Children |  | 
| `FileSystemInfo` | Info |  | 
| `String` | Name |  | 
| `FileTree` | Parent |  | 
| `String` | Path |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `FileType`

Classe que representa um MIME Type
```csharp
public class InnerLibs.FileType

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Description | Descrição do tipo de arquivo | 
| `List<String>` | Extensions | Extensão do arquivo | 
| `List<String>` | FirstType | Retorna o tipo do MIME Type (antes da barra) | 
| `List<String>` | MimeTypes | Tipo do arquivo (MIME Type String) | 
| `List<String>` | SubType | Retorna o subtipo do MIME Type (depois da barra) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsApplication() | Verifica se Tipo de arquivo é de audio | 
| `Boolean` | IsAudio() | Verifica se Tipo de arquivo é de audio | 
| `Boolean` | IsImage() | Verifica se Tipo de arquivo é de imagem | 
| `Boolean` | IsText() | Verifica se Tipo de arquivo é de audio | 
| `Boolean` | IsVideo() | Verifica se Tipo de arquivo é de audio | 
| `String` | ToFilterString() | Retorna uma string representando um filtro de caixa de dialogo WinForms | 
| `String` | ToString() | Retorna uma string com o primeiro MIME TYPE do arquivo | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | GetExtensions(`String` MIME) | Traz uma lista de extensões de acordo com o MIME type especificado | 
| `FileType` | GetFileType(`String` MimeTypeOrExtensionOrPathOrDataURI) | Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string | 
| `FileTypeList` | GetFileTypeList() | Retorna uma Lista com todos os MIME Types suportados | 
| `List<String>` | GetFileTypeStringList() | Retorna uma lista de strings contendo todos os MIME Types | 


## `FileTypeExtensions`

Módulo de manipulaçao de MIME Types
```csharp
public class InnerLibs.FileTypeExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | GetFileType(this `String` Extension) | Retorna o Mime Type a partir da extensão de um arquivo | 
| `List<String>` | GetFileType(this `FileInfo` File) | Retorna o Mime Type a partir da extensão de um arquivo | 
| `List<String>` | GetFileType(this `ImageFormat` RawFormat) | Retorna o Mime Type a partir da extensão de um arquivo | 
| `List<String>` | GetFileType(this `Image` Image) | Retorna o Mime Type a partir da extensão de um arquivo | 
| `Icon` | GetIcon(this `FileInfo` File) | Retorna um icone de acordo com o arquivo | 
| `FileType` | ToFileType(this `String` MimeTypeOrExtensionOrPathOrDataURI) | Retorna um Objeto FileType a partir de uma string MIME Type, Nome ou Extensão de Arquivo | 


## `FileTypeList`

Lista com Tipos de arquivo ultilizada para filtro e validação
```csharp
public class InnerLibs.FileTypeList
    : List<FileType>, IList<FileType>, ICollection<FileType>, IEnumerable<FileType>, IEnumerable, IList, ICollection, IReadOnlyList<FileType>, IReadOnlyCollection<FileType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | Extensions | Retorna todas as extensões da lista | 
| `List<String>` | MimeTypes | Retorna todas os MIME Types da lista | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<FileInfo>` | SearchFiles(`DirectoryInfo` Directory, `SearchOption` SearchOption = AllDirectories) | Busca arquivos que correspondam com as extensões desta lista | 
| `String` | ToFilterString() | Retorna uma string representando um filtro de caixa de dialogo WinForms | 


## `FontAwesome`

```csharp
public class InnerLibs.FontAwesome

```

Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | CDNFontAwesomeCSS |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetIconByFileExtension(this `String` Extension) | Retorna a classe do icone do FontAwesome que representa melhor o arquivo | 
| `String` | GetIconByFileType(this `FileSystemInfo` File, `Boolean` DirectoryOpen = False, `Boolean` InvertIcon = False) | Retorna a classe do icone do FontAwesome que representa melhor o arquivo ou diretório | 
| `String` | GetIconByFileType(this `FileType` MIME) | Retorna a classe do icone do FontAwesome que representa melhor o arquivo ou diretório | 


## `FullMoneyWriter`

Classe para escrever moedas por extenso com suporte até 999 quintilhoes de $$
```csharp
public class InnerLibs.FullMoneyWriter
    : FullNumberWriter

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `QuantityTextPair` | CurrencyCentsName | Par de strings que representam os centavos desta moeda em sua forma singular ou plural | 
| `QuantityTextPair` | CurrencyName | Par de strings que representam os nomes da moeda em sua forma singular ou plural | 
| `String` | Text | Escreve um numero por extenso | 
| `String` | Text | Escreve um numero por extenso | 


## `FullNumberWriter`

Classe para escrever numeros por extenso com suporte até 999 quintilhoes
```csharp
public class InnerLibs.FullNumberWriter

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | And | String que representa a palavra "e". Utilizada na concatenação de expressões | 
| `QuantityTextPair` | Billion | Par de strings que representam os numeros 1 bilhão a 999 bilhões | 
| `String` | Dot | String utilizada quando um numero possui casa decimais. Normalmente "virgula" | 
| `String` | Eight | String que representa o numero 8. | 
| `String` | Eighteen | String que representa o numero 18. | 
| `String` | EightHundred | String que representa os numeros 800 a 899. | 
| `String` | Eighty | String que representa os numeros 80 a 89. | 
| `String` | Eleven | String que representa o numero 11. | 
| `String` | ExactlyOneHundred | String que represena o exato numero 100. Em alguns idiomas esta string não é nescessária | 
| `String` | Fifteen | String que representa o numero 15. | 
| `String` | Fifty | String que representa os numeros 50 a 59. | 
| `String` | Five | String que representa o numero 5. | 
| `String` | FiveHundred | String que representa os numeros 500 a 599. | 
| `String` | Four | String que representa o numero 4. | 
| `String` | FourHundred | String que representa os numeros 400 a 499. | 
| `String` | Fourteen | String que representa o numero 14. | 
| `String` | Fourty | String que representa os numeros 40 a 49. | 
| `QuantityTextPair` | Million | Par de strings que representam os numeros 1 milhão a 999 milhões | 
| `String` | Minus | String que representa a palavra "Menos". Utilizada quando os números são negativos | 
| `String` | MoreThan | String utilizada quando o numero é maior que 999 quintilhões. Retorna uma string "Mais de 999 quintilhões" | 
| `String` | Nine | String que representa o numero 9. | 
| `String` | NineHundred | String que representa os numeros 900 a 999. | 
| `String` | Nineteen | String que representa o numero 19. | 
| `String` | Ninety | String que representa os numeros 90 a 99. | 
| `String` | One | String que representa o numero 1. | 
| `String` | OneHundred | String que representa os numeros 100 a 199. | 
| `QuantityTextPair` | Quadrillion | Par de strings que representam os numeros 1 quadrilhão a 999 quadrilhões | 
| `QuantityTextPair` | Quintillion | Par de strings que representam os numeros 1 quintilhão a 999 quintilhões | 
| `String` | Seven | String que representa o numero 7. | 
| `String` | SevenHundred | String que representa os numeros 700 a 799. | 
| `String` | Seventeen | String que representa o numero 17. | 
| `String` | Seventy | String que representa os numeros 70 a 79. | 
| `String` | Six | String que representa o numero 6. | 
| `String` | SixHundred | String que representa os numeros 600 a 699. | 
| `String` | Sixteen | String que representa o numero 16. | 
| `String` | Sixty | String que representa os numeros 60 a 69. | 
| `String` | Ten | String que representa o numero 10. | 
| `String` | Text | Escreve um numero por extenso | 
| `String` | Thirteen | String que representa o numero 13. | 
| `String` | Thirty | String que representa os numeros 30 a 39. | 
| `String` | Thousand | String que representa os numeros 1000 a 9999 | 
| `String` | Three | String que representa o numero 3. | 
| `String` | ThreeHundred | String que representa os numeros 300 a 399. | 
| `QuantityTextPair` | Trillion | Par de strings que representam os numeros 1 trilhão a 999 trilhões | 
| `String` | Twelve | String que representa o numero 12. | 
| `String` | Twenty | String que representa os numeros 20 a 29 . | 
| `String` | Two | String que representa o numero 2. | 
| `String` | TwoHundred | String que representa os numeros 200 a 299. | 
| `String` | Zero | String que representa o numero 0. | 


## `Generate`

Geradores de conteudo
```csharp
public class InnerLibs.Generate

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | InnerIpsum(`Int32` ParagraphNumber = 5) | Gera um InnerIpsum (InnerIpsum é uma modificação do classico Lorem Ipsum) | 
| `String` | LoremIpsum(`Int32` ParagraphNumber = 5) | Gera um InnerIpsum (InnerIpsum é uma modificação do classico Lorem Ipsum) | 
| `Color` | RandomColor(`Int32` Red = -1, `Int32` Green = -1, `Int32` Blue = -1) | Gera uma cor aleatória misturandoo ou não os canais RGB | 
| `List<Color>` | RandomColorList(`Int32` Quantity, `Int32` Red = -1, `Int32` Green = -1, `Int32` Blue = -1) | Gera uma lista com `` cores diferentes | 
| `Int32` | RandomNumber(`Int64` Min = 0, `Int64` Max = 999999) | Gera um numero Aleatório entre 2 números | 
| `String` | RandomWord(`Int32` Length = 0) | Gera uma palavra aleatória com o numero de caracteres | 
| `String` | RandomWord(`String` BaseText) | Gera uma palavra aleatória com o numero de caracteres | 
| `Image` | ScreenshotFromWebsite(this `String` Url, `String` AccessKey, `Boolean` FullPage = True, `Int32` Delay = 1, `String` Viewport = 1440x900, `Int32` ImageWidth = 500) | Tira uma screenshot de um site usando o servico ATS | 
| `Uri` | ToGoogleMapsURL(this `AddressInfo` local, `Boolean` LatLong = False) | Gera uma URL do google MAPs baseado na localização | 
| `Byte[]` | ToQRCode(this `String` Data, `Int32` Size = 100) | Converte uma String para um QR Code usando uma API (Nescessita de Internet) | 
| `Color` | WordToColor(this `String` Text) | Gera uma cor a partir de uma palavra | 


## `Globals`

```csharp
public class InnerLibs.Globals

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | TBool(`iEvalTypedValue` o) |  | 
| `DateTime` | TDate(`iEvalTypedValue` o) |  | 
| `Double` | TNum(`iEvalTypedValue` o) |  | 
| `String` | TStr(`iEvalTypedValue` o) |  | 


## `iEvalFunctions`

```csharp
public interface InnerLibs.iEvalFunctions

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `iEvalFunctions` | InheritedFunctions() |  | 


## `iEvalHasDescription`

```csharp
public interface InnerLibs.iEvalHasDescription

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Description |  | 
| `String` | Name |  | 


## `iEvalTypedValue`

```csharp
public interface InnerLibs.iEvalTypedValue
    : iEvalValue

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `EvalType` | EvalType |  | 
| `Type` | SystemType |  | 


## `iEvalValue`

```csharp
public interface InnerLibs.iEvalValue

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | Value |  | 


Events

| Type | Name | Summary | 
| --- | --- | --- | 
| `ValueChangedEventHandler` | ValueChanged |  | 


## `Images`

Modulo de Imagem
```csharp
public class InnerLibs.Images

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `ImageFormat[]` | ImageTypes | Lista com todos os formatos de imagem | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Bitmap` | CombineImages(`Boolean` VerticalFlow, `Image[]` Images) | Combina 2 ou mais imagens em uma única imagem | 
| `Bitmap` | CombineImages(this `List<Image>` Images, `Boolean` VerticalFlow = False) | Combina 2 ou mais imagens em uma única imagem | 
| `Bitmap` | CombineImages(this `Image[]` Images, `Boolean` VerticalFlow = False) | Combina 2 ou mais imagens em uma única imagem | 
| `Bitmap` | ConvertToGrayscale(this `Image` source) | Converte uma Imagem para Escala de cinza | 
| `Image` | Crop(this `Image` Image, `Size` Size) | Cropa uma imagem a patir do centro | 
| `Image` | Crop(this `Image` Image, `Int32` MaxWidth, `Int32` MaxHeight) | Cropa uma imagem a patir do centro | 
| `Image` | CropToCircle(this `Image` Img, `Nullable<Color>` Background = null) | Corta a imagem em um circulo | 
| `Image` | CropToSquare(this `Image` Img, `Int32` WidthHeight = 0) | Corta uma imagem para um quadrado perfeito a partir do centro | 
| `ImageCodecInfo` | GetEncoderInfo(this `ImageFormat` RawFormat) | Pega o encoder a partir de um formato de imagem | 
| `ImageFormat` | GetImageFormat(this `Image` OriginalImage) | Retorna o formato da imagem correspondente a aquela imagem | 
| `List<Color>` | GetMostUsedColors(this `Image` Image, `Int32` Count = 10) | Retorna uma lista com as 10 cores mais utilizadas na imagem | 
| `List<Color>` | GetMostUsedColors(this `Bitmap` Image, `Int32` Count = 10) | Retorna uma lista com as 10 cores mais utilizadas na imagem | 
| `Image` | InsertWatermark(this `Image` Image, `Image` WaterMark, `Int32` X = -1, `Int32` Y = -1) | Insere uma imagem de marca Dágua na imagem | 
| `Bitmap` | InvertImageColors(this `Image` Img) | Inverte as cores de uma imagem | 
| `Image` | Resize(this `Image` Original, `String` ResizeExpression, `Boolean` OnlyResizeIfWider = True) | Redimensiona e converte uma Imagem | 
| `Image` | Resize(this `Image` Original, `Size` Size, `Boolean` OnlyResizeIfWider = True) | Redimensiona e converte uma Imagem | 
| `Image` | Resize(this `Image` Original, `Int32` NewWidth, `Int32` MaxHeight, `Boolean` OnlyResizeIfWider = True) | Redimensiona e converte uma Imagem | 
| `Image` | ResizeCrop(this `Image` Image, `Int32` Width, `Int32` Height) | redimensiona e Cropa uma imagem, aproveitando a maior parte dela | 
| `Image` | ResizeCrop(this `Image` Image, `Int32` Width, `Int32` Height, `Boolean` OnlyResizeIfWider) | redimensiona e Cropa uma imagem, aproveitando a maior parte dela | 
| `Image` | ResizePercent(this `Image` Original, `String` Percent, `Boolean` OnlyResizeIfWider = True) | Redimensiona uma imagem para o tamanho definido por uma porcentagem | 
| `Image` | ResizePercent(this `Image` Original, `Decimal` Percent, `Boolean` OnlyResizeIfWider = True) | Redimensiona uma imagem para o tamanho definido por uma porcentagem | 
| `Boolean` | TestAndRotate(this `Image&` Img) | Rotaciona uma imagem para sua pocisão original caso ela já tenha sido rotacionada (EXIF) | 
| `Byte[]` | ToBytes(this `Image` Image, `ImageFormat` Format = null) | Transforma uma imagem em array de bytes | 
| `Size` | ToSize(this `String` Text) | Interperta uma string de diversas formas e a transforma em um `System.Drawing.Size` | 
| `Stream` | ToStream(this `Image` Image, `ImageFormat` Format = null) | Transforma uma imagem em um stream | 
| `Image` | Trim(this `Image` Img, `Color` Color) | Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo | 


## `InnerCrypt`

```csharp
public class InnerLibs.InnerCrypt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | InnCrypt(this `String` Text, `Int32` Seed = 1) | Criptografa uma suma string usando a logica InnerCrypt | 
| `String` | UnnCrypt(this `String` EncryptedText, `Int32` Seed = 1) | Descriptografa uma string previamente criptografada com InnerCrypt | 


## `iVariableBag`

```csharp
public interface InnerLibs.iVariableBag

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `iEvalTypedValue` | GetVariable(`String` varname) |  | 


## `JSMin`

```csharp
public class InnerLibs.JSMin

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsAlphanum(`Int32` c) |  | 
| `String` | Minify(`String` src) |  | 


## `Mathematic`

Módulo para calculos
```csharp
public class InnerLibs.Mathematic

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<Int32>` | ArithmeticProgression(`Int32` FirstNumber, `Int32` Constant, `Int32` Length) | Retorna uma progressão Aritmética com N numeros | 
| `Decimal` | Average(`Decimal[]` Values) | Tira a média de todos os números de um Array | 
| `Double` | Average(`Double[]` Values) | Tira a média de todos os números de um Array | 
| `Int32` | Average(`Int32[]` Values) | Tira a média de todos os números de um Array | 
| `Int64` | Average(`Int64[]` Values) | Tira a média de todos os números de um Array | 
| `Object` | CalculateCompoundInterest(this `Decimal` Capital, `Decimal` Rate, `Decimal` Time) | Calcula Juros compostos | 
| `Double` | CalculateDistance(`AddressInfo` FirstLocation, `AddressInfo` SecondLocation) | Calcula a distancia entre 2 locais | 
| `Double` | CalculateDistance(`AddressInfo[]` Locations) | Calcula a distancia entre 2 locais | 
| `Dictionary<TKey, Decimal>` | CalculatePercent(this `Dictionary<TKey, TValue>` Dic) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Dictionary<TKey, Decimal>` | CalculatePercent(this `IEnumerable<TObject>` Obj, `Func<TObject, TKey>` KeySelector, `Func<TObject, TValue>` ValueSelector) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Dictionary<Tobject, Decimal>` | CalculatePercent(this `IEnumerable<Tobject>` Obj, `Func<Tobject, Tvalue>` ValueSelector) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Dictionary<TValue, Decimal>` | CalculatePercent(this `IEnumerable<TValue>` Obj) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Decimal` | CalculatePercent(this `Decimal` Value, `Decimal` Total) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Decimal` | CalculatePercentVariation(this `Decimal` StartValue, `Decimal` EndValue) | Calcula a variação percentual entre 2 valores | 
| `Decimal` | CalculatePercentVariation(this `Int32` StartValue, `Int32` EndValue) | Calcula a variação percentual entre 2 valores | 
| `Decimal` | CalculatePercentVariation(this `Int64` StartValue, `Int64` EndValue) | Calcula a variação percentual entre 2 valores | 
| `Object` | CalculateSimpleInterest(this `Decimal` Capital, `Decimal` Rate, `Decimal` Time) | Calcula os Juros simples | 
| `Decimal` | CalculateValueFromPercent(this `String` Percent, `Decimal` Total) | Retorna o valor de um determinado percentual de um valor total | 
| `Decimal` | CalculateValueFromPercent(this `Int32` Percent, `Decimal` Total) | Retorna o valor de um determinado percentual de um valor total | 
| `Decimal` | CalculateValueFromPercent(this `Decimal` Percent, `Decimal` Total) | Retorna o valor de um determinado percentual de um valor total | 
| `List<T[]>` | CartesianProduct(`T[][]` Sets) | Retorna todas as possiveis combinações de Arrays do mesmo tipo (Produto Cartesiano) | 
| `Int64` | Ceil(this `Decimal` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int64` | Ceil(this `Double` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int32` | DifferenceIfMax(this `Int32` Total, `Int32` MaxValue) | Retorna a diferença entre 2 numeros se o valor maximo for menor que o total | 
| `Int32` | DifferenceIfMin(this `Int32` Total, `Int32` MinValue) | Retorna a diferença entre 2 numeros se o valor minimo for maior que o total | 
| `Object` | EvaluateExpression(`String` Formula, `Boolean` Exception = False) | Executa uma Expressão matematica/lógica simples | 
| `T` | EvaluateExpression(`String` Formula, `Boolean` Exception = False) | Executa uma Expressão matematica/lógica simples | 
| `Int32` | Factorial(this `Int32` Number) | Calcula o fatorial de um numero | 
| `List<Int32>` | Fibonacci(`Int32` Length) | Retorna uma sequencia Fibonacci de N numeros | 
| `Int64` | Floor(this `Decimal` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Int64` | Floor(this `Double` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `List<Int32>` | GeometricProgression(`Int32` FirstNumber, `Int32` Constant, `Int32` Length) | Retorna uma Progressão Gemoétrica com N numeros | 
| `Int64` | GetDecimalPlaces(this `Decimal` Value, `Int32` DecimalPlaces = 0, `CultureInfo` Culture = null) | Retorna um numero inteiro representando a parte decimal de um numero decimal | 
| `Boolean` | HasDecimalPart(this `Decimal` Value) | Verifica se um numero possui parte decimal | 
| `Boolean` | HasDecimalPart(this `Double` Value) | Verifica se um numero possui parte decimal | 
| `T` | HigherOf(this `IEnumerable<T>` Elements) | Retorna o elemento de maior valor de uma coleção | 
| `T` | HigherOf(`T[]` Elements) | Retorna o elemento de maior valor de uma coleção | 
| `Boolean` | IsBetween(this `Type` Value, `Object` FirstValue, `Object` SecondValue) | Verifica se um valor numerico ou data está entre outros 2 valores | 
| `Boolean` | IsEqualOrBetween(this `Type` Value, `Object` FirstValue, `Object` SecondValue) | Verifica se um valor é igual ou está entre outros 2 valores | 
| `Single` | Lerp(this `Single` Start, `Single` End, `Single` Amount) | Realiza um calculo de interpolação Linear | 
| `Int32` | LimitIndex(this `Int32` Int, `IEnumerable<AnyType>` Collection) |  | 
| `Int64` | LimitIndex(this `Int64` Lng, `IEnumerable<AnyType>` Collection) |  | 
| `Type` | LimitRange(this `Type` Number, `Object` MinValue = null, `Object` MaxValue = null) | Limita um range para um numero | 
| `T` | LowerOf(this `IEnumerable<T>` Elements) | Retorna o elemento de menor valor de uma coleção | 
| `T` | LowerOf(`T[]` Elements) | Retorna o elemento de menor valor de uma coleção | 
| `Int32` | Round(this `Decimal` Number, `Int32` MiddleNumber = 5, `CultureInfo` Culture = null) | Arredonda um numero para baixo ou para cima de acordo com outro numero | 
| `Int32` | Round(this `Decimal` Number) | Arredonda um numero para baixo ou para cima de acordo com outro numero | 
| `Type` | SetMaxValue(this `Type` Number, `Type` MaxValue) | Limita o valor Maximo de um numero | 
| `Type` | SetMinValue(this `Type` Number, `Type` MinValue) | Limita o valor minimo de um numero | 
| `Decimal` | Slice(this `Decimal` Value, `Int32` Places = 2) | Corta um numero decimal com a quantidade de casas especiicadas | 
| `Double` | Sum(`Double[]` Values) | Soma todos os números de um array | 
| `Int64` | Sum(`Int64[]` Values) | Soma todos os números de um array | 
| `Int32` | Sum(`Int32[]` Values) | Soma todos os números de um array | 
| `Decimal` | Sum(`Decimal[]` Values) | Soma todos os números de um array | 
| `String` | ToOrdinalNumber(this `Int32` Number, `Boolean` ExcludeNumber = False) | retorna o numeor em sua forma ordinal (inglês) | 
| `String` | ToOrdinalNumber(this `Int64` Number, `Boolean` ExcludeNumber = False) | retorna o numeor em sua forma ordinal (inglês) | 
| `String` | ToOrdinalNumber(this `Int16` Number) | retorna o numeor em sua forma ordinal (inglês) | 
| `String` | ToOrdinalNumber(this `Double` Number) | retorna o numeor em sua forma ordinal (inglês) | 
| `String` | ToOrdinalNumber(this `Decimal` Number) | retorna o numeor em sua forma ordinal (inglês) | 
| `Double` | ToRadians(this `Double` Degrees) | COnverte graus para radianos | 


## `Money`

Estrutura que representa valores em dinheiro de uma determinada `System.Globalization.CultureInfo`. Utiliza uma API (http://fixer.io) para conversão de moedas.
```csharp
public struct InnerLibs.Money

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `CultureInfo` | Culture | Cultura correspondente a esta moeda | 
| `String` | CurrencySymbol | Simbolo de moeda | 
| `String` | ISOCurrencySymbol | Simbolo de moeda utilizada em cambio (ISO) | 
| `String` | MoneyString | String do valor formatado como moeda | 
| `RegionInfo` | Region | Região correspondente a essa moeda | 
| `Decimal` | Value |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Equals(`Object` obj) | Compara se 2 valores são iguais (mesmo valor e moeda) | 
| `String` | ToString() | String do valor formatado como moeda, é um alias para `InnerLibs.Money.MoneyString` | 
| `String` | ToString(`Int32` Precision) | String do valor formatado como moeda, é um alias para `InnerLibs.Money.MoneyString` | 


Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | DefaultPattern |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<CultureInfo>` | GetCultureInfosByCurrencySymbol(`String` Currency) | Traz uma lista de `System.Globalization.CultureInfo` que utilizam uma determinada moeda de acordo com o simbolo, simbolo ISO ou | 


## `OnlineList<UserType, IdType>`

```csharp
public class InnerLibs.OnlineList<UserType, IdType>
    : Dictionary<IdType, OnlineUser<UserType, IdType>>, IDictionary<IdType, OnlineUser<UserType, IdType>>, ICollection<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, IEnumerable<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<IdType, OnlineUser<UserType, IdType>>, IReadOnlyCollection<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, ISerializable, IDeserializationCallback

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserChat<UserType, IdType>` | Chat |  | 
| `OnlineUser<UserType, IdType>` | Item |  | 
| `TimeSpan` | ToleranceTime |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | Add(`UserType` Obj) |  | 
| `OnlineUser<UserType, IdType>` | Add(`UserType` Obj, `Boolean` Online = False, `String` Activity = null) |  | 
| `Boolean` | ContainsUser(`UserType` User) |  | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | OnlineUsers() |  | 
| `void` | Remove(`UserType[]` Obj) |  | 
| `OnlineUser<UserType, IdType>` | SetOffline(`UserType` Obj) |  | 
| `OnlineUser<UserType, IdType>` | SetOnline(`UserType` Obj, `String` Activity = null) |  | 
| `UserType` | UserById(`IdType` Key) |  | 


## `OnlineUser<UserType, IdType>`

```csharp
public class InnerLibs.OnlineUser<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserConversation`2[]` | Conversations |  | 
| `UserType` | Data |  | 
| `IdType` | ID |  | 
| `Boolean` | IsOnline |  | 
| `String` | LastActivity |  | 
| `Nullable<DateTime>` | LastOnline |  | 
| `String` | LastUrl |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserConversation<UserType, IdType>` | SendMessage(`UserType` ToUser, `String` Message) |  | 


## `opCode`

```csharp
public abstract class InnerLibs.opCode
    : iEvalTypedValue, iEvalValue, iEvalHasDescription

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Description |  | 
| `EvalType` | EvalType |  | 
| `String` | Name |  | 
| `Type` | systemType |  | 
| `Object` | value |  | 


Events

| Type | Name | Summary | 
| --- | --- | --- | 
| `ValueChangedEventHandler` | ValueChanged |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | CanReturn(`EvalType` type) |  | 


## `opCodeCallMethod`

```csharp
public class InnerLibs.opCodeCallMethod
    : opCode, iEvalTypedValue, iEvalValue, iEvalHasDescription

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `EvalType` | EvalType |  | 
| `Type` | systemType |  | 
| `Object` | value |  | 


## `opCodeGetArrayEntry`

```csharp
public class InnerLibs.opCodeGetArrayEntry
    : opCode, iEvalTypedValue, iEvalValue, iEvalHasDescription

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `EvalType` | EvalType |  | 
| `Type` | systemType |  | 
| `Object` | value |  | 


## `opCodeGetVariable`

```csharp
public class InnerLibs.opCodeGetVariable
    : opCode, iEvalTypedValue, iEvalValue, iEvalHasDescription

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `EvalType` | EvalType |  | 
| `Type` | systemType |  | 
| `Object` | value |  | 


## `PaginationInfo<T, ListType>`

Classe para manipulação de coleções de forma paginada
```csharp
public class InnerLibs.PaginationInfo<T, ListType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `ListType` | Data | Registros desta pagina | 
| `Object` | Filter | Filtro aplicado previaménte em lista. Guarda qualquer objeto | 
| `Int32` | PageCount | Quantidade de páginas | 
| `Int32` | PageNumber | Numero desta pagina | 
| `IEnumerable<Int32>` | PageRange | Retorna um range de páginas a partir da pagina atual | 
| `Int32` | PageSize | Quantidade de itens por página | 
| `Int32` | PaginationOffset | Quantidade média de "botões de paginação" contidas no `InnerLibs.PaginationInfo`2.PageRange` | 
| `Int32` | Total | Total de itens da Lista | 


## `Paragraph`

```csharp
public class InnerLibs.Paragraph
    : List<Sentence>, IList<Sentence>, ICollection<Sentence>, IEnumerable<Sentence>, IEnumerable, IList, ICollection, IReadOnlyList<Sentence>, IReadOnlyCollection<Sentence>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Ident |  | 
| `StructuredText` | StructuredText |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `Phonetic`

Implementação da função SoundEX em Portugues
```csharp
public class InnerLibs.Phonetic

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | SoundExCode | Código SoundExBR que representa o fonema da palavra | 
| `Boolean` | SoundsLike | Compara o fonema de uma palavra em portugues com outra palavra | 
| `String` | Word | Palavra Original | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsListenedIn(`String` Text) | Verifica se o fonema atual está presente em alguma frase | 


## `QuantityTextPair`

```csharp
public class InnerLibs.QuantityTextPair

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Plural |  | 
| `String` | Singular |  | 
| `String` | Text |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `Object` | ToString(`Int64` Number) |  | 
| `Object` | ToString(`Decimal` Number) |  | 
| `Object` | ToString(`Int16` Number) |  | 
| `Object` | ToString(`Int32` Number) |  | 
| `Object` | ToString(`Double` Number) |  | 
| `Object` | ToString(`Single` Number) |  | 


## `Romanize`

Modulo para manipulação de numeros romanos
```csharp
public class InnerLibs.Romanize

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | ToArabic(this `String` RomanNumber) | Converte uma String contendo um numero romano para seu valor arabico | 
| `String` | ToRoman(this `Int32` ArabicNumber) | Converte um valor numérico arabico para numero romano | 


## `RuleOfThree`

```csharp
public class InnerLibs.RuleOfThree

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `EquationPair` | FirstEquation | Primeira Equaçao | 
| `EquationPair` | SecondEquation | Segunda Equaçao | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Nullable`1[][]` | ToArray() |  | 


## `SelfKeyDictionary<KeyType, ClassType>`

Uma estrutura de `System.Collections.IDictionary` que utiliza como Key uma propriedade de Value
```csharp
public class InnerLibs.SelfKeyDictionary<KeyType, ClassType>
    : IDictionary<KeyType, ClassType>, ICollection<KeyValuePair<KeyType, ClassType>>, IEnumerable<KeyValuePair<KeyType, ClassType>>, IEnumerable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Count |  | 
| `Boolean` | IsReadOnly |  | 
| `ClassType` | Item |  | 
| `ICollection<KeyType>` | Keys |  | 
| `ICollection<ClassType>` | Values |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `KeyType` | Add(`ClassType` Value) |  | 
| `void` | Clear() |  | 
| `Boolean` | Contains(`KeyValuePair<KeyType, ClassType>` item) |  | 
| `Boolean` | ContainsKey(`KeyType` key) |  | 
| `void` | CopyTo(`KeyValuePair`2[]` array, `Int32` arrayIndex) |  | 
| `IEnumerator<KeyValuePair<KeyType, ClassType>>` | GetEnumerator() |  | 
| `Boolean` | Remove(`KeyType` key) |  | 
| `Boolean` | Remove(`ClassType` Value) |  | 
| `Boolean` | Remove(`KeyValuePair<KeyType, ClassType>` item) |  | 
| `Boolean` | TryGetValue(`KeyType` key, `ClassType&` value) |  | 


## `Sentence`

Sentença de um texto (uma frase ou oração)
```csharp
public class InnerLibs.Sentence
    : List<SentencePart>, IList<SentencePart>, ICollection<SentencePart>, IEnumerable<SentencePart>, IEnumerable, IList, ICollection, IReadOnlyList<SentencePart>, IReadOnlyCollection<SentencePart>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Paragraph` | Paragraph |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `SentencePart`

Parte de uma sentença. Pode ser uma palavra, pontuaçao ou qualquer caractere de encapsulamento
```csharp
public class InnerLibs.SentencePart

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Sentence` | Sentence |  | 
| `String` | Text | Texto desta parte de sentença | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsCloseWrapChar() | Retorna TRUE se esta parte de senteça for um caractere de fechamento de encapsulamento | 
| `Boolean` | IsComma() | Retorna TRUE se esta parte de sentença é uma vírgula | 
| `Boolean` | IsEndOfSentencePunctuation() | Retorna TRUE se esta parte de senteça for um caractere de encerramento de frase (pontuaçao) | 
| `Boolean` | IsMidSentencePunctuation() | Retorna TRUE se esta parte de senteça for um caractere de de meio de sentença (dois pontos ou ponto e vírgula) | 
| `Boolean` | IsNotWord() | Retorna TRUE se esta parte de senteça não for uma palavra | 
| `Boolean` | IsOpenWrapChar() | Retorna TRUE se esta parte de senteça for um caractere de abertura de encapsulamento | 
| `Boolean` | IsPunctuation() | Retorna TRUE se esta parte de senteça for qualquer tipo de pontuaçao | 
| `Boolean` | IsWord() | Retorna TRUE se esta parte de senteça for uma palavra | 
| `Boolean` | NeedSpaceOnNext() | Retorna true se é nescessário espaço andes da proxima sentença | 
| `SentencePart` | Next() | Parte da próxima sentença | 
| `SentencePart` | Previous() | Parte de sentença anterior | 
| `String` | ToString() |  | 


## `SoundEx`

```csharp
public class InnerLibs.SoundEx

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | SoundEx(this `String` Text) | Gera um código SOUNDEX para comparação de fonemas | 
| `Boolean` | SoundsLike(this `String` FirstText, `String` SecondText) | Compara 2 palavras e verifica se elas possuem fonema parecido | 


## `StructuredText`

Texto estruturado (Dividido em parágrafos)
```csharp
public class InnerLibs.StructuredText
    : List<Paragraph>, IList<Paragraph>, ICollection<Paragraph>, IEnumerable<Paragraph>, IEnumerable, IList, ICollection, IReadOnlyList<Paragraph>, IReadOnlyCollection<Paragraph>

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `TableGenerator`

Assistente de criação de tabelas HTML
```csharp
public class InnerLibs.TableGenerator

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Table(`String` TableHeader, `String` Rows, `String` ID = , `String` Class = ) | Cria uma Table HTML a partir de strings geradas | 
| `String` | TableHeader(`String[]` Ths) | Cria um Table Header (thead) com as colunas especificadas | 
| `String` | TableRow(`String` ID, `String[]` Tds) | Cria uma linha de tabela html com diversas colunas (td) | 


## `Text`

Modulo de manipulação de Texto
```csharp
public class InnerLibs.Text

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String[]` | BreakLineChars |  | 
| `String[]` | CloseWrappers |  | 
| `String[]` | EndOfSentencePunctuation |  | 
| `String[]` | MidSentencePunctuation |  | 
| `String[]` | OpenWrappers |  | 
| `String[]` | WhiteSpaceChars | Caracteres em branco | 
| `String[]` | WordSplitters | Strings utilizadas para descobrir as palavras em uma string | 
| `String[]` | WordWrappers | Caracteres usado para encapsular palavras em textos | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | AdjustBlankSpaces(this `String` Text) |  | 
| `String` | AdjustPathChars(this `String` Text) | Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos | 
| `String` | AdjustWhiteSpaces(this `String` Text) |  | 
| `String` | Alphabetize(this `String` Text) | Retorna uma string em ordem afabética baseada em uma outra string | 
| `String` | AppendIf(this `String` Text, `String` AppendText, `Boolean` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | AppendIf(this `String` Text, `String` AppendText, `Func<String, Boolean>` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | AppendLine(this `String` Text, `String` AppendText) | Adiciona texto ao final de uma string com uma quebra de linha no final do `` | 
| `String` | AppendUrlParameter(this `String` Url, `String` Key, `String[]` Value) |  | 
| `String` | AppendWhile(this `String` Text, `String` AppendText, `Func<String, Boolean>` Test) | Adiciona texto ao final de uma string enquanto um criterio for cumprido | 
| `String` | ApplySpaceOnWrapChars(this `String` Text) | Aplica espacos em todos os caracteres de encapsulamento | 
| `String` | Brackfy(this `String` Text, `Char` QuoteChar = ") | Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) é um alias de `InnerLibs.Text.Quote(System.String,System.Char)` | 
| `String` | CamelSplit(this `String` Text) | Transforma uma palavra em CameCase em varias palavras a partir de suas letras maíusculas | 
| `String` | Censor(this `String` Text, `IEnumerable<String>` BadWords, `String` CensorshipCharacter = *, `Boolean` IsCensored = False) | Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro  caractere desejado) e retorna um valor indicando se o texto precisou ser censurado | 
| `String` | Censor(this `String` Text, `String` CensorshipCharacter, `String[]` BadWords) | Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro  caractere desejado) e retorna um valor indicando se o texto precisou ser censurado | 
| `Boolean` | ContainsAll(this `String` Text, `String[]` Values) | Verifica se uma String contém todos os valores especificados | 
| `Boolean` | ContainsAll(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma String contém todos os valores especificados | 
| `Boolean` | ContainsAny(this `String` Text, `String[]` Values) | Verifica se uma String contém qualquer um dos valores especificados | 
| `Boolean` | ContainsAny(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma String contém qualquer um dos valores especificados | 
| `Boolean` | ContainsMost(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma string contém a maioria dos valores especificados | 
| `Boolean` | ContainsMost(this `String` Text, `String[]` Values) | Verifica se uma string contém a maioria dos valores especificados | 
| `Int32` | CountCharacter(this `String` Text, `Char` Character) | Conta os caracters especificos de uma string | 
| `Dictionary<String, Int64>` | CountWords(this `String` Text, `Boolean` RemoveDiacritics = True, `String[]` Words = null) | Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade | 
| `Int32` | Decrement(this `Int32&` Number, `Int32` Amount = 1) | Decrementa em 1 ou mais um numero inteiro | 
| `Int32` | Decrement(this `Int64&` Number, `Int32` Amount = 1) | Decrementa em 1 ou mais um numero inteiro | 
| `String` | DeleteLine(this `String` Text, `Int32` LineIndex) | Remove uma linha especifica de um texto | 
| `Dictionary<String, Int64>` | DistinctCount(`String[]` List) | Cria um dicionário com as palavras de uma lista e a quantidade de cada uma. | 
| `Dictionary<String, Int64>` | DistinctCount(this `String` Phrase) | Cria um dicionário com as palavras de uma lista e a quantidade de cada uma. | 
| `Boolean` | EndsWithAny(this `String` Text, `String[]` Words) | Verifica se uma string termina com alguma outra string de um array | 
| `String` | EscapeQuotesToQuery(this `String` Text) | Prepara uma string com aspas simples para uma Query TransactSQL | 
| `Decimal[]` | FindNumbers(this `String` Text) | Procura numeros em uma string e retorna um array deles | 
| `List<String>` | FindTelephoneNumbers(this `String` Text) | Procurea numeros de telefone em um texto | 
| `String` | FixBreakLines(this `String` Text) | Transforma quebras de linha HTML em quebras de linha comuns ao .net | 
| `String` | FixCaptalization(this `String` Text) |  | 
| `String` | FixPunctuation(this `String&` Text, `String` Punctuation = ., `Boolean` ForceSpecificPunctuation = False) | Adciona pontuaçao ao final de uma string se a mesma não terminar com alguma pontuacao. | 
| `String` | FixText(this `String` Text) | Arruma a ortografia do texto captalizando corretamente, adcionando pontução ao final de frase  caso nescessário e removendo espaços excessivos ou incorretos | 
| `String` | Format(this `String` Text, `String[]` Args) | Extension Method para `System.String` | 
| `String` | FormatCNPJ(this `Int64` CNPJ) | Formata um numero para CNPJ | 
| `String` | FormatCNPJ(this `String` CNPJ) | Formata um numero para CNPJ | 
| `String` | FormatCPF(this `Int64` CPF) | Formata um numero para CPF | 
| `String` | FormatCPF(this `String` CPF) | Formata um numero para CPF | 
| `String` | FormatCPFOrCNPJ(this `Int64` Document) | Formata um numero para CNPJ ou CNPJ se forem validos | 
| `String` | GetAfter(this `String` Text, `String` Value) | Retorna um texto posterior a outro | 
| `String[]` | GetAllBetween(this `String` Text, `String` Before, `String` After = ) | Retorna todas as ocorrencias de um texto entre dois textos | 
| `String` | GetBefore(this `String` Text, `String` Value) | Retorna um texto anterior a outro | 
| `String` | GetBetween(this `String` Text, `String` Before, `String` After) | Retorna o texto entre dois textos | 
| `String` | GetDomain(this `Uri` URL, `Boolean` RemoveFirstSubdomain = False) | Pega o dominio principal de uma URL | 
| `String` | GetDomain(this `String` URL, `Boolean` RemoveFirstSubdomain = False) | Pega o dominio principal de uma URL | 
| `String` | GetFirstChars(this `String` Text, `Int32` Number = 1) |  | 
| `String` | GetLastChars(this `String` Text, `Int32` Number = 1) |  | 
| `String` | GetMiddleChars(this `String` Text, `Int32` Length) | Retorna N caracteres de uma string a partir do caractere encontrado no centro | 
| `String` | GetOppositeWrapChar(this `String` Text) | Retorna o caractere de encapsulamento oposto ao caractere indicado | 
| `Type` | GetRandomItem(this `List<Type>` List) | Sorteia um item da Lista | 
| `Type` | GetRandomItem(this `Type[]` Array) | Sorteia um item da Lista | 
| `String` | GetRelativeURL(this `Uri` URL) | Retorna o caminho relativo da url | 
| `String` | GetRelativeURL(this `String` URL) | Retorna o caminho relativo da url | 
| `IOrderedEnumerable<String>` | GetWords(this `String` Text) | Retorna uma lista de palavras encontradas no texto em ordem alfabetica | 
| `String[]` | GetWrappedText(this `String` Text, `String` Character = ", `Boolean` ExcludeWrapChars = True) | Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em um texto | 
| `String` | HtmlDecode(this `String` Text) | Retorna um texto com entidades HTML convertidas para caracteres e tags BR em breaklines | 
| `String` | HtmlEncode(this `String` Text) | Escapa o texto HTML | 
| `Int32` | Increment(this `Int32&` Number, `Int32` Amount = 1) | Incrementa em 1 ou mais um numero inteiro | 
| `Int64` | Increment(this `Int64&` Number, `Int32` Amount = 1) | Incrementa em 1 ou mais um numero inteiro | 
| `Boolean` | IsAnagramOf(this `String` Text, `String` AnotherText) | Verifica se uma palavra é um Anagrama de outra palavra | 
| `Boolean` | IsAny(this `String` Text, `String[]` Texts) | Compara se uma string é igual a outras strings | 
| `Boolean` | IsLikeAny(this `String` Text, `String` Pattern) | Verifica se um texto é parecido com outro outro usando comparação com caratere curinga | 
| `Boolean` | IsLikeAny(this `String` Text, `IEnumerable<String>` Patterns) | Verifica se um texto é parecido com outro outro usando comparação com caratere curinga | 
| `Boolean` | IsLikeAny(this `String` Text, `String[]` Patterns) | Verifica se um texto é parecido com outro outro usando comparação com caratere curinga | 
| `Boolean` | IsNotAny(this `String` Text, `String[]` Texts) | Compara se uma string é não igual a outras strings | 
| `Boolean` | IsPalindrome(this `String` Text, `Boolean` IgnoreWhiteSpaces = False) | Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da esqueda  para direita | 
| `String` | Join(this `IEnumerable<Type>` Array, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(this `Type[]` Array, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(`String` Separator, `Type[]` Array) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(this `List<Type>` List, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `Int32` | LevenshteinDistance(this `String` Text1, `String` Text2) | Computa a distancia de Levenshtein entre 2 strings. | 
| `String` | MaskTelephoneNumber(this `String` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Int64` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Int32` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Decimal` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Double` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ParseAlphaNumeric(this `String` Text) | limpa um texto deixando apenas os caracteres alfanumericos. | 
| `String` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) | Remove caracteres não numéricos de uma string | 
| `Type` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) | Remove caracteres não numéricos de uma string | 
| `NameValueCollection` | ParseQueryString(this `String` querystring) |  | 
| `String` | Poopfy(`String[]` Words) | Retorna uma string em sua forma poop | 
| `String` | Poopfy(this `String` Text) | Retorna uma string em sua forma poop | 
| `String` | PreetyPrint(this `XmlDocument` Document) | Return a Idented XML string | 
| `String` | Prepend(this `String` Text, `String` PrependText) | Adiciona texto ao começo de uma string | 
| `String` | PrependIf(this `String` Text, `String` PrependText, `Boolean` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | PrependIf(this `String` Text, `String` PrependText, `Func<String, Boolean>` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | PrependLine(this `String` Text, `String` AppendText) | Adiciona texto ao inicio de uma string com uma quebra de linha no final do `` | 
| `String` | PrependWhile(this `String` Text, `String` PrependText, `Func<String, Boolean>` Test) | Adiciona texto ao inicio de uma string enquanto um criterio for cumprido | 
| `String` | PrintIf(this `String` Text, `Boolean` BooleanValue) | Retorna a string especificada se o valor booleano for verdadeiro | 
| `String` | QuantifyText(this `String` PluralText, `String` Identifier = q) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado em ``. | 
| `String` | QuantifyText(this `String` PluralText, `Object` Quantity) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado em ``. | 
| `String` | Quote(this `String` Text, `Char` QuoteChar = ") | Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) | 
| `String` | QuoteIf(this `String` Text, `Boolean` Condition, `String` QuoteChar = ") | Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes) se uma  condiçao for cumprida | 
| `Type` | RandomItem(`Type[]` Array) | Sorteia um item da Matriz | 
| `String` | RegexEscape(this `String` Text) | Escapa caracteres exclusivos de uma regex | 
| `String` | RemoveAccents(this `String` Text) | Remove os acentos de uma string | 
| `String` | RemoveAny(this `String` Text, `String[]` Values) | Remove várias strings de uma string | 
| `String` | RemoveDiacritics(this `String&` Text) | Remove os acentos de uma string | 
| `String` | RemoveFirstAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StartStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveFirstChars(this `String` Text, `Int32` Quantity = 1) | Remove os X primeiros caracteres | 
| `String` | RemoveFirstIf(this `String` Text, `String` StartStringTest) | Remove um texto do inicio de uma string se ele for um outro texto especificado | 
| `String` | RemoveHTML(this `String` Text) |  | 
| `String` | RemoveLastAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` EndStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveLastChars(this `String` Text, `Int32` Quantity = 1) | Remove os X ultimos caracteres | 
| `String` | RemoveLastIf(this `String` Text, `String` EndStringTest) | Remove um texto do final de uma string se ele for um outro texto | 
| `String` | RemoveNonPrintable(this `String` Text) | Remove caracteres não printaveis de uma string | 
| `String[]` | Replace(this `String[]` Strings, `String` OldValue, `String` NewValue, `Boolean` ReplaceIfEquals = True) | Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum | 
| `List<String>` | Replace(this `List<String>` Strings, `String` OldValue, `String` NewValue, `Boolean` ReplaceIfEquals = True) | Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum | 
| `String` | ReplaceFirst(this `String` Text, `String` OldText, `String` NewText = ) | Substitui a primeira ocorrencia de um texto por outro | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, String>` Dic) | Aplica varios replaces a um texto a partir de um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, T>` Dic) | Aplica varios replaces a um texto a partir de um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, String[]>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) | Aplica varios replaces a um texto a partir de um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String[], String>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) | Aplica varios replaces a um texto a partir de um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String[], String[]>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) | Aplica varios replaces a um texto a partir de um `System.Collections.IDictionary` | 
| `String` | ReplaceLast(this `String` Text, `String` OldText, `String` NewText = ) | Substitui a ultima ocorrencia de um texto por outro | 
| `String` | ReplaceMany(this `String` Text, `String` NewValue, `String[]` OldValues) | Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são  substituídas por um novo valor. | 
| `String` | ReplaceNone(this `String` Text, `String` OldValue) | Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são  substituídas por vazio. | 
| `String` | SensitiveReplace(this `String` Text, `String` NewValue, `String` OldValue, `StringComparison` ComparisonType = InvariantCulture) | Realiza um replace em uma string usando um tipo especifico de comparacao | 
| `String` | SensitiveReplace(this `String` Text, `String` NewValue, `IEnumerable<String>` OldValues, `StringComparison` ComparisonType = InvariantCulture) | Realiza um replace em uma string usando um tipo especifico de comparacao | 
| `Type[]` | Shuffle(this `Type[]` Array) | Randomiza a ordem dos itens de um Array | 
| `List<Type>` | Shuffle(this `List`1&` List) | Randomiza a ordem dos itens de um Array | 
| `String` | Shuffle(this `String&` Text) | Randomiza a ordem dos itens de um Array | 
| `String` | Singularize(this `String` Text) | Retorna a frase ou termo especificado em sua forma singular | 
| `String` | Slice(this `String` Text, `Int32` TextLength = 0, `String` Ellipsis = ...) | Corta un texto para exibir um numero máximo de caracteres ou na primeira quebra de linha. | 
| `String[]` | Split(this `String` Text, `String` Separator, `StringSplitOptions` Options = RemoveEmptyEntries) | Separa um texto em um array de strings a partir de uma outra string | 
| `String[]` | SplitAny(this `String` Text, `String[]` SplitText) | Seprar uma string em varias partes a partir de varias strings removendo as entradas em branco | 
| `Boolean` | StartsWithAny(this `String` Text, `String[]` Words) | Verifica se uma string começa com alguma outra string de um array | 
| `Int32` | SyllableCount(this `String` Word) | Conta as silabas de uma palavra | 
| `String` | ToAlternateCase(this `String` Text) | Alterna maiusculas e minusculas para cada letra de uma string | 
| `String` | ToAnagram(this `String` Text) | Retorna um anagrama de um texto | 
| `String` | ToCamel(this `String` Text) | Transforma uma frase em uma palavra CamelCase | 
| `String` | ToFileSizeString(this `Byte[]` Size, `Int32` DecimalPlaces = -1) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `FileInfo` Size, `Int32` DecimalPlaces = -1) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Double` Size, `Int32` DecimalPlaces = -1) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Int32` Size, `Int32` DecimalPlaces = -1) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Int64` Size, `Int32` DecimalPlaces = -1) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Decimal` Size, `Int32` DecimalPlaces = -1) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFriendlyPathName(this `String` Text) | Prepara uma string para se tornar uma caminho amigavel (remove caracteres nao permitidos) | 
| `String` | ToFriendlyURL(this `String` Text, `Boolean` UseUnderscore = False) | Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca  espacos por hifen) | 
| `String` | ToLeet(this `String` Text, `Int32` Degree = 30) | Converte um texo para Leet (1337) | 
| `String` | ToPercentString(this `Decimal` Number, `Int32` Decimals = -1) | Retorna um numero com o sinal de porcentagem | 
| `String` | ToPercentString(this `Int32` Number) | Retorna um numero com o sinal de porcentagem | 
| `String` | ToPercentString(this `Double` Number, `Int32` Decimals = -1) | Retorna um numero com o sinal de porcentagem | 
| `String` | ToPercentString(this `Int16` Number) | Retorna um numero com o sinal de porcentagem | 
| `String` | ToPercentString(this `Int64` Number) | Retorna um numero com o sinal de porcentagem | 
| `String` | ToProperCase(this `String` Text, `Boolean` ForceCase = False) | Coloca o texto em TitleCase | 
| `String` | ToRandomCase(this `String` Text, `Int32` Times = 0) | Coloca a string em Randomcase (aleatoriamente letras maiusculas ou minusculas) | 
| `String` | ToSlugCase(this `String` Text, `Boolean` UseUnderscore = False) | Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca  espacos por hifen). É um alias para `InnerLibs.Text.ToFriendlyURL(System.String,System.Boolean)` | 
| `String` | ToSnakeCase(this `String` Text) | Retorna uma string em Snake_Case | 
| `Stream` | ToStream(this `String` Text) | Cria um `System.IO.Stream` a partir de uma string | 
| `String` | ToTitle(this `String` Text, `Boolean` ForceCase = False) | Transforma um texto em titulo | 
| `String` | ToXMLString(this `XmlDocument` XML) | Transforma um XML Document em string | 
| `String` | TrimAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StringTest) | Remove do começo e do final de uma string qualquer valor que estiver no conjunto | 
| `String` | TrimAny(this `String` Text, `String[]` StringTest) | Remove do começo e do final de uma string qualquer valor que estiver no conjunto | 
| `String` | TrimCarriage(this `String` Text) | Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines | 
| `String` | UrlDecode(this `String` Text) | Decoda uma string de uma transmissão por URL | 
| `String` | UrlEncode(this `String` Text) | Encoda uma string para transmissão por URL | 
| `String` | Wrap(this `String` Text, `String` WrapText = ") | Encapsula um tento entre 2 textos | 


## `Toggles`

Modulo que liga/desliga, (inverte) valores de variaveis
```csharp
public class InnerLibs.Toggles

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Toggle(this `Boolean&` Bool) | Inverte os valores TRUE/FALSE | 
| `void` | Toggle(this `Int32&` Int) | Inverte os valores TRUE/FALSE | 
| `void` | Toggle(this `String&` CurrentString, `String` TrueValue = True, `String` FalseValue = False) | Inverte os valores TRUE/FALSE | 
| `void` | Toggle(this `Char&` CurrentChar, `Char` TrueValue = 1, `Char` FalseValue = 0) | Inverte os valores TRUE/FALSE | 


## `tokenizer`

```csharp
public class InnerLibs.tokenizer

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | startpos |  | 
| `eTokenType` | type |  | 
| `Object` | value |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | NextToken() |  | 


## `UnitConverter`

Classe para manipulaçao de numeros e conversão unidades
```csharp
public class InnerLibs.UnitConverter

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `CultureInfo` | Culture |  | 
| `StringComparison` | UnitComparisonType |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Abreviate(`Decimal` Number, `Int32` DecimalPlaces) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Decimal` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Int32` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Int16` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Int64` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `Decimal` | Convert(`Decimal` Number, `String` To, `String` From) | Converte um numero   decimal em outro numero decimal a partir de unidades de medida | 
| `Decimal` | Convert(`String` AbreviatedNumber, `String` To) | Converte um numero   decimal em outro numero decimal a partir de unidades de medida | 
| `String` | ConvertAbreviate(`String` AbreviatedNumber, `String` To) | Converte um numero abreviado em outro numero abreviado de outra unidade | 
| `Decimal` | Parse(`String` Number, `Int32` DecimalPlaces = -1) | Retorna o numero decimal a partir de uma string abreviada | 
| `String` | ParseUnit(`String` Number) | Extrai a Unidade utilizada a partir de um numero abreviado | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UnitConverter` | CreateBase1000Converter() | Cria um `InnerLibs.UnitConverter` de Base 1000 (de y a E) | 
| `UnitConverter` | CreateComplexMassConverter() | Cria um `InnerLibs.UnitConverter` de de Massa (peso) complexos de base 10 (de mg a kg) | 
| `UnitConverter` | CreateFileSizeConverter() | Cria um `InnerLibs.UnitConverter` de Base 1024 (Bytes) de (B a EB) | 
| `UnitConverter` | CreateSimpleMassConverter() | Cria um `InnerLibs.UnitConverter` de de Massa (peso) simples de base 1000 (de mg a T) | 


## `UserChat<UserType, IdType>`

```csharp
public class InnerLibs.UserChat<UserType, IdType>
    : List<UserConversation<UserType, IdType>>, IList<UserConversation<UserType, IdType>>, ICollection<UserConversation<UserType, IdType>>, IEnumerable<UserConversation<UserType, IdType>>, IEnumerable, IList, ICollection, IReadOnlyList<UserConversation<UserType, IdType>>, IReadOnlyCollection<UserConversation<UserType, IdType>>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Encoding` | Encoding |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | DeleteConversation(`UserType` User, `UserType` WithUser = null) |  | 
| `IEnumerable<UserConversation<UserType, IdType>>` | GetConversation(`UserType` User, `UserType` WithUser = null) |  | 
| `UserConversation<UserType, IdType>` | Send(`UserType` FromUser, `UserType` ToUser, `String` Message) |  | 


## `UserConversation<UserType, IdType>`

```csharp
public class InnerLibs.UserConversation<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | FromUser |  | 
| `String` | ID |  | 
| `String` | Message |  | 
| `DateTime` | SentDate |  | 
| `OnlineUser<UserType, IdType>` | ToUser |  | 
| `Boolean` | Viewed |  | 
| `Nullable<DateTime>` | ViewedDate |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | GetMyUser(`UserType` Myself) |  | 
| `OnlineUser<UserType, IdType>` | GetOtherUser(`UserType` Myself) |  | 
| `Boolean` | IsFrom(`UserType` User) |  | 


## `VariableComplexity`

```csharp
public enum InnerLibs.VariableComplexity
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | normal |  | 


## `VariableNotFoundException`

```csharp
public class InnerLibs.VariableNotFoundException
    : Exception, ISerializable, _Exception

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | VariableName |  | 


## `vCard`

Um objeto vCard
```csharp
public class InnerLibs.vCard

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `vAddresss` | Addresses |  | 
| `Nullable<DateTime>` | Birthday |  | 
| `vEmails` | Emails |  | 
| `String` | FirstName |  | 
| `String` | FormattedName |  | 
| `String` | JobTitle |  | 
| `DateTime` | LastModified |  | 
| `String` | LastName |  | 
| `String` | MiddleName |  | 
| `String` | Nickname |  | 
| `String` | Note |  | 
| `String` | Organization |  | 
| `String` | OrganizationalUnit |  | 
| `String` | Role |  | 
| `String` | Suffix |  | 
| `vTelephones` | Telephones |  | 
| `String` | Title |  | 
| `vURLs` | URLs |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FileInfo` | ToFile(`String` FullPath) |  | 
| `Image` | ToQRCode(`Int32` Size = 100) |  | 
| `String` | ToString() |  | 


## `Verify`

Verifica determinados valores como Arquivos, Numeros e URLs
```csharp
public class InnerLibs.Verify

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `T` | IfBlank(this `Object` Value, `T` ValueIfBlank = null) | Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE | 
| `T` | IfBlankOrNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfBlankOrNoIndex) | Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um valor default se o index nao existir ou seu valor for branco ou nothing | 
| `T` | IfNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfNoIndex = null) | Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um valor default se o index nao existir | 
| `T[]` | IfNullOrEmpty(this `Object[]` Value, `T[]` ValuesIfBlank) | Verifica se um aray está vazio ou nula e retorna um outro valor caso TRUE | 
| `IEnumerable<T>` | IfNullOrEmpty(this `IEnumerable<Object[]>` Value, `T[]` ValuesIfBlank) | Verifica se um aray está vazio ou nula e retorna um outro valor caso TRUE | 
| `IEnumerable<T>` | IfNullOrEmpty(this `IEnumerable<Object[]>` Value, `IEnumerable<T>` ValueIfBlank) | Verifica se um aray está vazio ou nula e retorna um outro valor caso TRUE | 
| `Boolean` | IsArray(`Object` Obj) |  | 
| `Boolean` | IsBlank(this `String` Text) | Verifica se uma String está em branco | 
| `Boolean` | IsDate(`String` Obj) |  | 
| `Boolean` | IsDirectoryPath(this `String` Text) | Verifica se uma string é um caminho de diretório válido | 
| `Boolean` | IsEmail(this `String` Text) | Verifica se um determinado texto é um email | 
| `Boolean` | IsEmpty(this `IEnumerable<T>` Col) | Verifica se um `System.Collections.IEnumerable` é nulo ou está vazio | 
| `Boolean` | IsEven(this `Decimal` Value) | Verifica se um numero é par | 
| `Boolean` | IsEven(this `Int32` Value) | Verifica se um numero é par | 
| `Boolean` | IsEven(this `Int64` Value) | Verifica se um numero é par | 
| `Boolean` | IsEven(this `Double` Value) | Verifica se um numero é par | 
| `Boolean` | IsFilePath(this `String` Text) | Verifica se uma string é um caminho de arquivo válido | 
| `Boolean` | IsInUse(this `FileInfo` File) | Verifica se o arquivo está em uso por outro procedimento | 
| `Boolean` | IsIP(this `String` IP) | Verifica se a string é um endereço IP válido | 
| `Boolean` | IsNotBlank(this `String` Text) | Verifica se uma String não está em branco | 
| `Boolean` | IsNotNumber(this `Object` Value) | Verifica se o valor não é um numero | 
| `Boolean` | IsNumber(this `Object` Value) | Verifica se o valor é um numero | 
| `Boolean` | IsOdd(this `Decimal` Value) | Verifica se um numero é impar | 
| `Boolean` | IsOdd(this `Int32` Value) | Verifica se um numero é impar | 
| `Boolean` | IsOdd(this `Int64` Value) | Verifica se um numero é impar | 
| `Boolean` | IsPath(this `String` Text) | Verifica se uma string é um caminho de diretóio válido | 
| `Boolean` | IsTelephone(this `String` Text) | Valida se a string é um telefone | 
| `Boolean` | IsURL(this `String` Text) | Verifica se um determinado texto é uma URL válida | 
| `Boolean` | IsValidCEP(this `String` CEP) | Verifica se uma string é um cep válido | 
| `Boolean` | IsValidCNH(this `String` cnh) | Verifica se a string é um CNH válido | 
| `Boolean` | IsValidCNPJ(this `String` Text) | Verifica se a string é um CNPJ válido | 
| `Boolean` | IsValidCPF(this `String` Text) | Verifica se a string é um CPF válido | 
| `Boolean` | IsValidCPFOrCNPJ(this `String` Text) | Verifica se a string é um CPF ou CNPJ válido | 
| `Boolean` | IsValidDomain(this `String` DomainOrEmail) | Verifica se o dominio é válido (existe) em uma URL ou email | 
| `T` | NullIf(this `T` Value, `Func<T, Boolean>` TestExpression) | Anula o valor de um objeto se ele for igual a outro objeto | 
| `T` | NullIf(this `T` Value, `T` TestValue) | Anula o valor de um objeto se ele for igual a outro objeto | 
| `Nullable<T>` | NullIf(this `Nullable<T>` Value, `Nullable<T>` TestValue) | Anula o valor de um objeto se ele for igual a outro objeto | 
| `String` | NullIf(this `String` Value, `String` TestValue) | Anula o valor de um objeto se ele for igual a outro objeto | 


## `Web`

Modulo Web
```csharp
public class InnerLibs.Web

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Uri` | AddParameter(this `Uri` Url, `String` Key, `String[]` Values) | Adciona um parametro a Query String de uma URL | 
| `String` | FileNameAsTitle(this `FileSystemInfo` Info) | Retorna o Titulo do arquivo a partir do nome do arquivo | 
| `String` | FileNameAsTitle(this `String` FileName) | Retorna o Titulo do arquivo a partir do nome do arquivo | 
| `String` | GetFacebookUsername(this `String` URL) | Captura o Username ou UserID de uma URL do Facebook | 
| `String` | GetFacebookUsername(this `Uri` URL) | Captura o Username ou UserID de uma URL do Facebook | 
| `Byte[]` | GetFile(`String` URL) |  | 
| `Image` | GetImage(`String` URL) |  | 
| `String` | GetString(`Object` URL) |  | 
| `IEnumerable<String>` | GetUrlSegments(this `String` Url) | Retorna os segmentos de uma url | 
| `String` | GetVideoId(`String` URL) | Captura o ID de um video do YOUTUBE ou VIMEO em uma URL | 
| `String` | GetVideoId(this `Uri` URL) | Captura o ID de um video do YOUTUBE ou VIMEO em uma URL | 
| `Byte[]` | GetYoutubeThumbnail(`String` URL) | Captura a Thumbnail de um video do youtube | 
| `Byte[]` | GetYoutubeThumbnail(`Uri` URL) | Captura a Thumbnail de um video do youtube | 
| `Boolean` | IsConnected(`String` Test = http://google.com) | Verifica se o computador está conectado com a internet | 
| `Boolean` | IsDown(this `String` Url) | Verifica se um site está indisponível usando o serviço IsUp.Me | 
| `Boolean` | IsDown(`Uri` Url) | Verifica se um site está indisponível usando o serviço IsUp.Me | 
| `Boolean` | IsUp(this `String` Url) | Verifica se um site está disponível usando o serviço IsUp.Me | 
| `Boolean` | IsUp(`Uri` Url) | Verifica se um site está disponível usando o serviço IsUp.Me | 
| `String` | MinifyCSS(this `String` CSS) | Minifica uma folha de estilo CSS | 
| `String` | MinifyJS(this `String` Js) | Minifica um arquivo JavaScript | 
| `Uri` | RemoveParameter(this `Uri` Url, `String[]` Keys) |  | 
| `String` | ToINSERT(this `NameValueCollection` Request, `String` TableName, `String[]` Keys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `String` | ToProcedure(this `NameValueCollection` NVC, `String` ProcedureName, `String[]` Keys) | Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de  uma URL como parametros da procedure | 
| `String` | ToProcedure(this `IDictionary<String, Object>` Dic, `String` ProcedureName, `String[]` Keys) | Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de  uma URL como parametros da procedure | 
| `String` | ToSQLFilter(this `IDictionary<String, Object>` Dic, `String` TableName, `String` CommaSeparatedColumns, `LogicConcatenationOperator` LogicConcatenation, `String[]` FilterKeys) | Monta um Comando SQL para executar um SELECT com filtros a partir de um `System.Collections.Generic.Dictionary`2` | 
| `String` | ToSQLFilter(this `NameValueCollection` NVC, `String` TableName, `String` CommaSeparatedColumns, `LogicConcatenationOperator` LogicConcatenation, `String[]` FilterKeys) | Monta um Comando SQL para executar um SELECT com filtros a partir de um `System.Collections.Generic.Dictionary`2` | 
| `String` | ToUPDATE(this `NameValueCollection` Request, `String` TableName, `String` WhereClausule, `String[]` Keys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 


