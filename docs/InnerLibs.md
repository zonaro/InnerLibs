## `Alphabet`

```csharp
public class InnerLibs.Alphabet

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Seed |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Alphabet |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Uri` | CreateLink(`String` UrlPattern, `Int32` ID) | Gera um link com a hash | 
| `Int32` | Decode(`String` s) |  | 
| `String` | Encode(`Int32` i) |  | 
| `String` | RandomHash() |  | 
| `String` | ToString() |  | 


## `AsciiArt`

```csharp
public class InnerLibs.AsciiArt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToAsciiArt(this `Bitmap` image, `Int32` ratio) |  | 
| `String` | ToAsciiArt(this `Bitmap` sourceBitmap, `Int32` pixelBlockSize, `Int32` colorCount = 0) |  | 


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


## `BeautyStrings`

```csharp
public class InnerLibs.BeautyStrings

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | BoxText(this `String` Text) |  | 
| `String` | BoxTextCSS(this `String` Text) |  | 


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
| `DateTime` | ClearMiliseconds(this `DateTime` Date) |  | 
| `IEnumerable<DateTime>` | ClearTime(this `IEnumerable<DateTime>` List) | Remove o tempo de todas as datas de uma lista e retorna uma nova lista | 
| `DateTime` | ConvertDateString(this `String` DateString, `String` Format, `CultureInfo` Culture = null) | Converte uma string em datetime a partir de um formato especifico | 
| `DateRange` | CreateDateRange(this `IQueryable<T>` List, `Expression<Func<T, Nullable<DateTime>>>` PropertyExpression, `Nullable<DateTime>` StartDate = null, `Nullable<DateTime>` EndDate = null) |  | 
| `void` | FixDateOrder(`DateTime&` StartDate, `DateTime&` EndDate) | Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate  sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um  `System.TimeSpan` negativo | 
| `Int32` | GetAge(this `DateTime` BirthDate, `Nullable<DateTime>` FromDate = null) | Retorna a idade | 
| `IEnumerable<DateTime>` | GetDaysBetween(this `DateTime` StartDate, `DateTime` EndDate, `DayOfWeek[]` DaysOfWeek) | Retorna as datas entre um periodo | 
| `LongTimeSpan` | GetDifference(this `DateTime` InitialDate, `DateTime` SecondDate) | Retorna uma `InnerLibs.TimeMachine.LongTimeSpan` com a diferença entre 2 Datas | 
| `Int32` | GetDoubleMonthOfYear(this `DateTime` DateAndtime) | Pega o numero do Bimestre a partir de uma data | 
| `DateTime` | GetFirstDayOfDoubleMonth(this `DateTime` Date) | Retorna o ultimo dia de um bimestre a partir da data | 
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


## `ClassTools`

```csharp
public class InnerLibs.ClassTools

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Type[]` | PrimitiveNumericTypes |  | 
| `Type[]` | PrimitiveTypes |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `R` | AsIf(this `T` obj, `Expression<Func<T, Boolean>>` BoolExp, `R` TrueValue, `R` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `T` | AsIf(this `Boolean` Bool, `T` TrueValue, `T` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `T` | AsIf(this `Nullable<Boolean>` Bool, `T` TrueValue, `T` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `T` | AsIf(this `String` Expression, `T` ChooseIfTrue, `T` ChooseIfFalse) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `String` | BlankCoalesce(this `String` First, `String[]` N) | Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que  possuir um valor | 
| `String` | BlankCoalesce(`String[]` N) | Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que  possuir um valor | 
| `Boolean` | ContainsAll(this `IEnumerable<Type>` List1, `IEnumerable<Type>` List2, `IEqualityComparer<Type>` Comparer = null) | Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array. | 
| `Boolean` | ContainsAny(this `IEnumerable<Type>` List1, `IEnumerable<Type>` List2, `IEqualityComparer<Type>` Comparer = null) | Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array. | 
| `Dictionary<String, Object>` | CreateDictionary(this `Type` Obj) | Converte uma classe para um `System.Collections.Generic.Dictionary`2` | 
| `IEnumerable<Dictionary<String, Object>>` | CreateDictionaryEnumerable(this `IEnumerable<Type>` Obj) | Converte uma classe para um `System.Collections.Generic.Dictionary`2` | 
| `Guid` | CreateGuidOrDefault(this `String` Source) | Cria um `System.Guid` a partir de uma string ou um novo `System.Guid` se a conversão falhar | 
| `Type` | CreateObjectFromXML(this `String` XML) |  | 
| `Type` | CreateObjectFromXMLFile(this `FileInfo` XML) |  | 
| `XmlDocument` | CreateXML(this `Type` obj) |  | 
| `FileInfo` | CreateXmlFile(this `Object` obj, `String` FilePath) | Cria um arquivo a partir de qualquer objeto usando o <see cref="!:ClassTools.CreateXML()" /> | 
| `T` | Detach(this `List<T>` List, `Int32` Index) | Remove um item de uma lista e retorna este item | 
| `Dictionary<Type, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr) | Conta de maneira distinta items de uma coleçao | 
| `Dictionary<PropT, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop) | Conta de maneira distinta items de uma coleçao | 
| `Dictionary<Type, Int64>` | DistinctCountTop(this `IEnumerable<Type>` Arr, `Int32` Top, `Type` Others) | Conta de maneira distinta N items de uma coleçao e agrupa o resto | 
| `Dictionary<PropT, Int64>` | DistinctCountTop(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop, `Int32` Top, `PropT` Others) | Conta de maneira distinta N items de uma coleçao e agrupa o resto | 
| `T` | FirstAny(this `IEnumerable<T>` source, `Expression`1[]` predicate) | O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista | 
| `T` | FirstAnyOr(this `IEnumerable<T>` source, `T` Alternate, `Expression`1[]` predicate) | O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista | 
| `T` | FirstOr(this `IEnumerable<T>` source, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `T` | FirstOr(this `IEnumerable<T>` source, `Func<T, Boolean>` predicate, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `void` | FixOrder(`T&` Value1, `T&` Value2) | Troca ou não a ordem das variaveis de inicio e fim  fazendo com que a Value1  sempre seja menor que a Value2. Util para tratar ranges | 
| `TValue` | GetAttributeValue(this `Type` type, `Func<TAttribute, TValue>` ValueSelector) |  | 
| `T` | GetEnumValue(`String` Name) | Traz o valor de uma enumeração a partir de uma string | 
| `String` | GetEnumValueAsString(this `T` Value) | Traz o valor de uma enumeração a partir de uma string | 
| `List<T>` | GetEnumValues() | Traz todos os Valores de uma enumeração | 
| `FieldInfo` | GetField(this `O` MyObject, `String` Name) | Traz uma propriedade de um objeto | 
| `IEnumerable<FieldInfo>` | GetFields(this `O` MyObject, `BindingFlags` BindAttr) | Traz uma Lista com todas as propriedades de um objeto | 
| `IEnumerable<FieldInfo>` | GetFields(this `O` MyObject) | Traz uma Lista com todas as propriedades de um objeto | 
| `Type` | GetNullableTypeOf(this `T` Obj) | Retorna o `System.Type` equivalente a `type`   ou o `System.Type` do objeto `System.Nullable`1` | 
| `IEnumerable<PropertyInfo>` | GetProperties(this `O` MyObject, `BindingFlags` BindAttr) | Traz uma Lista com todas as propriedades de um objeto | 
| `IEnumerable<PropertyInfo>` | GetProperties(this `O` MyObject) | Traz uma Lista com todas as propriedades de um objeto | 
| `PropertyInfo` | GetProperty(this `O` MyObject, `String` Name) | Traz uma propriedade de um objeto | 
| `T` | GetPropertyValue(this `O` MyObject, `String` Name) | Traz uma propriedade de um objeto | 
| `Byte[]` | GetResourceBytes(this `Assembly` Assembly, `String` FileName) | Pega os bytes de um arquivo embutido no assembly | 
| `String` | GetResourceFileText(this `Assembly` Assembly, `String` FileName) | Pega o texto de um arquivo embutido no assembly | 
| `Type` | GetTypeOf(this `O` Obj) | Retorna o `System.Type` do objeto mesmo se ele for nulo | 
| `Type[]` | GetTypesFromNamespace(this `Assembly` assembly, `String` desiredNamespace) | Retorna as classes de um Namespace | 
| `Type[]` | GetTypesFromNamespace(`String` desiredNamespace) | Retorna as classes de um Namespace | 
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
| `Boolean` | IsBetween(this `IComparable` Value, `IComparable` Value1, `IComparable` Value2) | Verifica se um valor numerico ou data está entre outros 2 valores | 
| `Boolean` | IsBetweenOrEqual(this `IComparable` Value, `IComparable` Value1, `IComparable` Value2) | Verifica se um valor numerico ou data está entre outros 2 valores | 
| `Boolean` | IsDictionary(this `Object` obj) | Verifica se o objeto é um iDictionary | 
| `Boolean` | IsEnumerable(this `Object` obj) | Verifica se o objeto é uma lista | 
| `Boolean` | IsEqual(this `T` Value1, `T` Value2) |  | 
| `Boolean` | IsGreaterThan(this `T` Value1, `T` Value2) |  | 
| `Boolean` | IsGreaterThanOrEqual(this `T` Value1, `T` Value2) |  | 
| `Boolean` | IsIn(this `Type` Obj, `Type[]` List) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsIn(this `Type` Obj, `String` Text, `IEqualityComparer<Char>` Comparer = null) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsInAny(this `Type` Obj, `IEnumerable`1[]` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays. | 
| `Boolean` | IsLessThan(this `T` Value1, `T` Value2) |  | 
| `Boolean` | IsLessThanOrEqual(this `T` Value1, `T` Value2) |  | 
| `Boolean` | IsList(this `Object` obj) | Verifica se o objeto é uma lista | 
| `Boolean` | IsNotIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o não objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsNotIn(this `Type` Obj, `String` Text, `IEqualityComparer<Char>` Comparer = null) | Verifica se o não objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsNotNullOrEmpty(this `IEnumerable<T>` List) |  | 
| `Boolean` | IsNullableType(this `Type` t) |  | 
| `Boolean` | IsNullableType(this `O` Obj) |  | 
| `Boolean` | IsNullableTypeOf(this `O` Obj) | Verifica se um objeto é de um determinado tipo | 
| `Boolean` | IsNullableTypeOf(this `O` Obj, `Type` Type) | Verifica se um objeto é de um determinado tipo | 
| `Boolean` | IsNullOrEmpty(this `IEnumerable<T>` List) |  | 
| `Boolean` | IsNumericType(this `T` Obj) | Verifica se o objeto é do tipo numérico. | 
| `Boolean` | IsPrimitiveType(this `Type` T) |  | 
| `Boolean` | IsPrimitiveType(this `T` Obj) |  | 
| `Boolean` | IsTypeOf(this `O` Obj) | Verifica se um objeto é de um determinado tipo | 
| `Boolean` | IsTypeOf(this `O` Obj, `Type` Type) | Verifica se um objeto é de um determinado tipo | 
| `T` | LastOr(this `IEnumerable<T>` source, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `NameValueCollection` | Merge(`NameValueCollection[]` NVC) | Mescla varios `System.Collections.Specialized.NameValueCollection` em um unico `System.Collections.Specialized.NameValueCollection` | 
| `IEnumerable<T>` | NullAsEmpty(this `IEnumerable<T>` List) |  | 
| `T` | NullCoalesce(this `Nullable<T>` First, `Nullable`1[]` N) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `IEnumerable<Nullable<T>>` List) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `T` First, `T[]` N) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `IEnumerable<T>` List) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullPropertiesAsDefault(this `T` Obj, `Boolean` IncludeVirtual = False) | Substitui todas as propriedades nulas de uma classe pelos seus valores Default | 
| `Boolean` | OnlyOneOf(this `IEnumerable<Type>` List, `Func<Type, Boolean>` predicate) | Verifica se somente um unico elemento corresponde a condição | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `TKey[]` Keys) | Remove de um dicionario as respectivas Keys se as mesmas existirem | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `Func<KeyValuePair<TKey, TValue>, Boolean>` predicate) | Remove de um dicionario as respectivas Keys se as mesmas existirem | 
| `List<T>` | RemoveLast(this `List<T>` List, `Int32` Count = 1) |  | 
| `IDictionary<KeyType, ValueType>` | Set(this `IDictionary<KeyType, ValueType>` Dic, `KT` Key, `VT` Value) | Adciona ou substitui um valor a este `System.Collections.Generic.Dictionary`2` e retorna a mesma instancia deste `System.Collections.Generic.Dictionary`2` | 
| `Type` | SetPropertyValue(this `Type` MyObject, `String` PropertyName, `Object` Value) | Seta o valor de uma propriedade de um objeto | 
| `Type` | SetPropertyValue(this `Type` obj, `Expression<Func<Type, Prop>>` Selector, `Prop` Value) | Seta o valor de uma propriedade de um objeto | 
| `Type` | SetPropertyValueFromCollection(this `Type` MyObject, `String` PropertyName, `CollectionBase` Collection) |  | 
| `Dictionary<K, T>` | TakeTop(this `IDictionary<K, T>` Dic, `Int32` Top, `K` GroupOthersLabel) | traz os top N valores de um dicionario e agrupa os outros | 
| `Dictionary<K, IEnumerable<T>>` | TakeTop(this `IDictionary<K, IEnumerable<T>>` Dic, `Int32` Top, `K` GroupOthersLabel) | traz os top N valores de um dicionario e agrupa os outros | 
| `String` | ToFullExceptionString(this `Exception` ex, `String` Separator =  >> ) | Concatena todas as  `System.Exception.InnerException` em uma única string | 
| `String` | ToQueryString(this `Dictionary<String, String>` Dic) | Retorna um dicionário em QueryString | 
| `String` | ToQueryString(this `NameValueCollection` NVC) | Retorna um dicionário em QueryString | 
| `IEnumerable<Object>` | ToTableArray(this `Dictionary<GroupKey, Dictionary<SubGroupKey, SubGroupValue>>` Groups, `Func<SubGroupKey, HeaderProperty>` HeaderProp) | Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays | 
| `Object` | ToTableArray(this `Dictionary<GroupKeyType, GroupValueType>` Groups) | Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays | 
| `T` | With(this `T` Obj, `Action<T>` a) | Metodo de extensão para utilizar qualquer objeto usando FluentAPI | 


## `ColorConvert`

Modulo de Conversão de Cores
```csharp
public class InnerLibs.ColorConvert

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<Color>` | KnowColors |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetClosestColorName(this `Color` Color) |  | 
| `Color` | GetClosestKnowColor(this `Color` Color) |  | 
| `String` | GetColorName(this `Color` Color) |  | 
| `Color` | GetContrastColor(this `Color` TheColor, `Single` Percent = 0,7) | Retorna uma cor de contraste baseado na iluminacao da primeira cor: Uma cor clara se a primeira for escura. Uma cor escura se a primeira for clara | 
| `Color` | GetNegativeColor(this `Color` TheColor) | Retorna  a cor negativa de uma cor | 
| `IEnumerable<Color>` | GrayscalePallete(`Int32` Amount) |  | 
| `Boolean` | IsDark(this `Color` TheColor) | Verifica se uma cor é escura | 
| `Boolean` | IsHexaDecimal(this `String` Text) |  | 
| `Boolean` | IsHexaDecimalColor(this `String` Text) |  | 
| `Boolean` | IsLight(this `Color` TheColor) | Verifica se uma clor é clara | 
| `Color` | Lerp(this `Color` TheColor, `Color` to, `Single` amount) | Mescla duas cores usando Lerp | 
| `Color` | MakeDarker(this `Color` TheColor, `Single` percent = 50) | Escurece a cor mesclando ela com preto | 
| `Color` | MakeLighter(this `Color` TheColor, `Single` percent = 50) | Clareia a cor mistuando ela com branco | 
| `Color` | MergeWith(this `Color` TheColor, `Color` AnotherColor, `Single` Percent = 50) | Mescal duas cores a partir de uma porcentagem | 
| `IEnumerable<Color>` | MonochromaticPallete(`Color` Color, `Int32` Amount) |  | 
| `Color` | RandomColor(`Int32` Red = -1, `Int32` Green = -1, `Int32` Blue = -1) | Gera uma cor aleatória misturandoo ou não os canais RGB | 
| `Color` | ToColor(this `String` Text) | Gera uma cor a partir de uma palavra | 
| `String` | ToHexadecimal(this `Color` Color, `Boolean` Hash = True) | Converte uma cor de sistema para hexadecimal | 
| `String` | ToRGB(this `Color` Color) | Converte uma cor de sistema para CSS RGB | 
| `String` | ToRGBA(this `Color` Color) |  | 


## `ConnectionStringParser`

```csharp
public class InnerLibs.ConnectionStringParser
    : Dictionary<String, String>, IDictionary<String, String>, ICollection<KeyValuePair<String, String>>, IEnumerable<KeyValuePair<String, String>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<String, String>, IReadOnlyCollection<KeyValuePair<String, String>>, ISerializable, IDeserializationCallback

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ConnectionStringParser` | Parse(`String` ConnectionString) |  | 
| `String` | ToString() | Retorna a connectionstring deste parser | 


## `Converter`

```csharp
public class InnerLibs.Converter

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ToType[]` | ChangeArrayType(this `FromType[]` Value) | Converte um array de um tipo para outro | 
| `Object[]` | ChangeArrayType(this `FromType[]` Value, `Type` Type) | Converte um array de um tipo para outro | 
| `IEnumerable<ToType>` | ChangeIEnumerableType(this `IEnumerable<FromType>` Value) | Converte um IEnumerable de um tipo para outro | 
| `IEnumerable<Object>` | ChangeIEnumerableType(this `IEnumerable<FromType>` Value, `Type` ToType) | Converte um IEnumerable de um tipo para outro | 
| `ToType` | ChangeType(this `FromType` Value) | Converte um tipo para outro. Retorna Nothing (NULL) se a conversão falhar | 
| `ToType` | ChangeType(this `Object` Value) | Converte um tipo para outro. Retorna Nothing (NULL) se a conversão falhar | 
| `Object` | ChangeType(this `FromType` Value, `Type` ToType) | Converte um tipo para outro. Retorna Nothing (NULL) se a conversão falhar | 
| `List<T>` | DefineEmptyList(this `T` ObjectForDefinition) | Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos | 
| `List<T>` | DefineEmptyList() | Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos | 
| `Object[]` | ForceArray(`Object` Obj, `Type` Type = null) | Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto | 
| `OutputType[]` | ForceArray(`Object` Obj) | Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto | 
| `Dictionary<Tkey, Object>` | Merge(this `Dictionary<Tkey, Object>` FirstDictionary, `Dictionary`2[]` Dictionaries) | Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um dicionario os valores sao agrupados em arrays | 
| `IEnumerable<Dictionary<TKey, TValue>>` | MergeKeys(this `IEnumerable<Dictionary<TKey, TValue>>` Dics, `TKey[]` AditionalKeys) | Aplica as mesmas keys a todos os dicionarios de uma lista | 
| `T` | SetValuesIn(this `Dictionary<String, Object>` Dic) | Seta as propriedades de uma classe a partir de um dictionary | 
| `T` | SetValuesIn(this `Dictionary<String, Object>` Dic, `T` Obj, `Object[]` args) | Seta as propriedades de uma classe a partir de um dictionary | 
| `List<T>` | StartList(this `T` ObjectForDefinition) | Cria uma e adciona um objeto a ela. Util para tipos anonimos | 
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
| `DateRange` | Clone() | Clona este DateRange | 
| `Boolean` | Contains(`DateRange` Period) | Verifica se este periodo contém um outro periodo | 
| `Boolean` | Contains(`DateTime` Day) | Verifica se este periodo contém um outro periodo | 
| `FortnightGroup` | CreateFortnightGroup() | Cria um grupo de quinzenas que contenham este periodo | 
| `LongTimeSpan` | Difference() | Retorna um `InnerLibs.TimeMachine.LongTimeSpan` contendo a diferença entre as datas | 
| `IEnumerable<T>` | FilterList(`IEnumerable<T>` List, `Expression<Func<T, DateTime>>` PropertyExpression) | Filtra uma lista considerando o periodo deste DateRange | 
| `IQueryable<T>` | FilterList(`IQueryable<T>` List, `Expression<Func<T, DateTime>>` PropertyExpression) | Filtra uma lista considerando o periodo deste DateRange | 
| `IEnumerable<T>` | FilterList(`IEnumerable<T>` List, `Expression<Func<T, Nullable<DateTime>>>` PropertyExpression) | Filtra uma lista considerando o periodo deste DateRange | 
| `IQueryable<T>` | FilterList(`IQueryable<T>` List, `Expression<Func<T, Nullable<DateTime>>>` PropertyExpression) | Filtra uma lista considerando o periodo deste DateRange | 
| `IEnumerable<DateTime>` | GetBetween(`DateRangeInterval` DateRangeInterval = LessAccurate) | Retorna uma lista com as datas entre `InnerLibs.DateRange.StartDate` e `InnerLibs.DateRange.EndDate` utilizando um Intervalo | 
| `IEnumerable<DateTime>` | GetDays(`DayOfWeek[]` DaysOfWeek) | Retorna uma lista de dias entre `InnerLibs.DateRange.StartDate` e `InnerLibs.DateRange.EndDate` | 
| `DateRangeInterval` | GetLessAccurateDateRangeInterval() | Retorna o `InnerLibs.DateRangeInterval` menos preciso para calcular periodos | 
| `Decimal` | GetPeriodAs(`DateRangeInterval` DateRangeInterval = LessAccurate) | Retorna o periodo em um total especificado por `InnerLibs.DateRangeInterval` | 
| `Dictionary<String, IEnumerable<T>>` | GroupList(`IEnumerable<T>` List, `Func<T, DateTime>` PropertyExpression, `Func<DateTime, String>` GroupByExpression, `DateRangeInterval` DateRangeInterval = LessAccurate) | Agrupa itens de uma lista de acordo com uma propriedade e uma expressão de agrugrupamento de datas | 
| `Dictionary<String, IEnumerable<T>>` | GroupList(`IEnumerable<T>` List, `Func<T, Nullable<DateTime>>` PropertyExpression, `Func<Nullable<DateTime>, String>` GroupByExpression, `DateRangeInterval` DateRangeInterval = LessAccurate) | Agrupa itens de uma lista de acordo com uma propriedade e uma expressão de agrugrupamento de datas | 
| `Boolean` | IsDefaultDateRange() | Indica se este `InnerLibs.DateRange` foi construido sem nenhuma data definida | 
| `Boolean` | IsIn(`DateRange` Period) | Verifica se este periodo está dentro de outro periodo | 
| `Boolean` | IsNow() | Verifica se hoje está dentro deste periodo | 
| `Boolean` | IsSingleDate() | Retorna TRUE se a data de inicio e fim for a mesma | 
| `Boolean` | IsSingleDateTime() | Retorna TRUE se a data e hora de inicio e fim for a mesma | 
| `DateRange` | JumpPeriod(`Int32` Amount, `DateRangeInterval` DateRangeInterval = LessAccurate) | Pula um determinado numero de periodos | 
| `Boolean` | MatchAny(`DateRange` Period) | Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou contém um periodo) | 
| `DateRange` | MovePeriod(`DateRangeInterval` DateRangeInterval, `Decimal` Total) | Move um periodo a partir de um `` especificado por `` | 
| `DateRange` | NextPeriod(`DateRangeInterval` DateRangeInterval = LessAccurate) | Move para ao proximo periodo equivalente | 
| `Boolean` | Overlaps(`DateRange` Period) | Verifica se 2 periodos possuem interseção de datas | 
| `IEnumerable<DateTime>` | Pair() |  | 
| `DateRange` | PreviousPeriod(`DateRangeInterval` DateRangeInterval = LessAccurate) | Move para o periodo equivalente anterior | 
| `String` | ToString() | Retorna uma strin representando a diferença das datas | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | AddInterval(`DateTime` Datetime, `DateRangeInterval` DateRangeInterval, `Decimal` Total) |  | 


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
| `IEnumerable<FileInfo>` | SearchFiles(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) | Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas | 
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
| `Object` | IsComplete |  | 
| `Object` | IsNotComplete |  | 
| `Object` | MissX |  | 
| `Object` | MissY |  | 
| `Nullable<Decimal>` | X |  | 
| `Nullable<Decimal>` | Y |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PropertyInfo` | GetMissing() |  | 
| `void` | SetMissing(`Decimal` value) |  | 
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
| `FileInfo` | WriteToFile(this `Byte[]` Bytes, `String` FilePath, `DateTime` DateTime) | Transforma um  Array de Bytes em um arquivo | 
| `FileInfo` | WriteToFile(this `String` Text, `String` FilePath, `Boolean` Append = False, `Encoding` Enconding = null) | Transforma um  Array de Bytes em um arquivo | 
| `FileInfo` | WriteToFile(this `String` Text, `FileInfo` File, `Boolean` Append = False, `Encoding` Enconding = null) | Transforma um  Array de Bytes em um arquivo | 


## `FileTree`

```csharp
public class InnerLibs.FileTree

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<FileTree>` | Children |  | 
| `FileType` | FileType |  | 
| `Bitmap` | Icon |  | 
| `FileSystemInfo` | Info |  | 
| `Boolean` | IsDirectory |  | 
| `Boolean` | IsFile |  | 
| `String` | Name |  | 
| `FileTree` | Parent |  | 
| `String` | Path |  | 
| `String` | TypeDescription |  | 


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
| `IEnumerable<String>` | FirstTypes | Retorna o tipo do MIME Type (antes da barra) | 
| `List<String>` | MimeTypes | Tipo do arquivo (MIME Type String) | 
| `IEnumerable<String>` | SubTypes | Retorna o subtipo do MIME Type (depois da barra) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | GetMimeTypesOrDefault() |  | 
| `Boolean` | IsApplication() | Verifica se Tipo de arquivo é de audio | 
| `Boolean` | IsAudio() | Verifica se Tipo de arquivo é de audio | 
| `Boolean` | IsImage() | Verifica se Tipo de arquivo é de imagem | 
| `Boolean` | IsText() | Verifica se Tipo de arquivo é de audio | 
| `Boolean` | IsVideo() | Verifica se Tipo de arquivo é de audio | 
| `IEnumerable<FileInfo>` | SearchFiles(`DirectoryInfo` Directory, `SearchOption` SearchOption = AllDirectories) |  | 
| `String` | ToFilterString() | Retorna uma string representando um filtro de caixa de dialogo WinForms | 
| `String` | ToString() | Retorna uma string com o primeiro MIME TYPE do arquivo | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | GetExtensions(`String` MIME, `FileTypeList` FileTypeList = null) | Traz uma lista de extensões de acordo com o MIME type especificado | 
| `FileTypeList` | GetFileType(`IEnumerable<String>` MimeTypeOrExtensionOrPathOrDataURI, `FileTypeList` FileTypeList = null) | Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string | 
| `FileType` | GetFileType(`String` MimeTypeOrExtensionOrPathOrDataURI, `FileTypeList` FileTypeList = null) | Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string | 
| `FileTypeList` | GetFileTypeList(`Boolean` Reset = False) | Retorna uma Lista com todos os MIME Types suportados | 
| `IEnumerable<String>` | GetFileTypeStringList(`FileTypeList` FileTypeList = null) | Retorna uma lista de strings contendo todos os MIME Types | 


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
| `Icon` | GetIcon(this `FileSystemInfo` File) | Retorna um icone de acordo com o arquivo | 
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
| `IEnumerable<String>` | Descriptions |  | 
| `IEnumerable<String>` | Extensions | Retorna todas as extensões da lista | 
| `IEnumerable<String>` | FirstTypes |  | 
| `IEnumerable<String>` | MimeTypes | Retorna todas os MIME Types da lista | 
| `IEnumerable<String>` | SubTypes |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<FileInfo>` | SearchFiles(`DirectoryInfo` Directory, `SearchOption` SearchOption = AllDirectories) | Busca arquivos que correspondam com as extensões desta lista | 
| `String` | ToFilterString() | Retorna uma string representando um filtro de caixa de dialogo WinForms | 


## `FluentSwitch<T1, T2>`

```csharp
public class InnerLibs.FluentSwitch<T1, T2>

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FluentSwitch<T1, T2>` | Case(`T1` Value, `T2` ReturnValue) |  | 
| `FluentSwitch<T1, T2>` | Case(`IEnumerable<T1>` Values, `T2` ReturnValue) |  | 
| `FluentSwitch<T1, T2>` | Default(`T2` ReturnValue) |  | 
| `T2` | GetValue() |  | 


## `FluentSwitchExt`

```csharp
public class InnerLibs.FluentSwitchExt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FluentSwitch<T1, T2>` | Switch(this `T1` Input) |  | 
| `T2` | Switch(this `T1` Input, `Action<FluentSwitch<T1, T2>>` Test) |  | 


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


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString(`Decimal` Number, `Int32` DecimalPlaces = 2) | Escreve um numero por extenso | 
| `String` | ToString(`Money` Number, `Int32` DecimalPlaces = 2) | Escreve um numero por extenso | 
| `String` | ToString() | Escreve um numero por extenso | 


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
| `String` | DecimalSeparator | String utilizada quando um numero possui casa decimais. Normalmente "virgula" | 
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


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `String` | ToString(`Decimal` Number, `Int32` DecimalPlaces = 2) |  | 


## `Generate`

Geradores de conteudo
```csharp
public class InnerLibs.Generate

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | RandomBoolean(`Int32` Percent) | Gera um valor boolean aleatorio considerando uma porcentagem de chance | 
| `Boolean` | RandomBoolean(`Func<Int64, Boolean>` Condition, `Int64` Min = 0, `Int64` Max = 999999) | Gera um valor boolean aleatorio considerando uma porcentagem de chance | 
| `Boolean` | RandomBoolean() | Gera um valor boolean aleatorio considerando uma porcentagem de chance | 
| `List<Color>` | RandomColorList(`Int32` Quantity, `Int32` Red = -1, `Int32` Green = -1, `Int32` Blue = -1) | Gera uma lista com `` cores diferentes | 
| `StructuredText` | RandomIpsum(`Int32` ParagraphCount = 5, `Int32` SentenceCount = 3, `Int32` MinWordCount = 10, `Int32` MaxWordCount = 50, `Int32` IdentSize = 0, `Int32` BreakLinesBetweenParagraph = 0) | Gera um texto aleatorio | 
| `Int32` | RandomNumber(`Int32` Min = 0, `Int32` Max = 999999) | Gera um numero Aleatório entre 2 números | 
| `Object` | RandomString(`Int32` Len) |  | 
| `String` | RandomWord(`Int32` Length = 0) | Gera uma palavra aleatória com o numero de caracteres | 
| `String` | RandomWord(`String` BaseText) | Gera uma palavra aleatória com o numero de caracteres | 
| `Uri` | ToGoogleMapsURL(this `AddressInfo` local, `Boolean` LatLong = False) | Gera uma URL do google MAPs baseado na localização | 
| `Byte[]` | ToQRCode(this `String` Data, `Int32` Size = 100) | Converte uma String para um QR Code usando uma API (Nescessita de Internet) | 


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


## `HSVColor`

```csharp
public class InnerLibs.HSVColor

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Byte` | A | Alpha (Transparencia) | 
| `Int32` | B | Blue (Azul) | 
| `String` | ColorName | Nome original mais proximo desta cor | 
| `String` | Description | Descricao desta cor | 
| `Int32` | G | Green (Verde) | 
| `Double` | H | Hue (Matiz) | 
| `String` | Hexadecimal | Valor hexadecimal desta cor | 
| `String` | Name | Nome atribuido a esta cor | 
| `Decimal` | Opacity | Opacidade (de 1 a 100%) | 
| `Int32` | R | Red (Vermelho) | 
| `Double` | S | Saturation (Saturação) | 
| `Double` | V | Value (Brilho) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HSVColor[]` | Analogous(`Boolean` ExcludeMe = False) | Retorna as cores análogas desta cor | 
| `HSVColor` | Average(`HSVColor` Color) | Retorna a cor media entre 2 cores | 
| `HSVColor` | Clone() | Retorna uma cópia desta cor | 
| `HSVColor` | Combine(`HSVColor` Color) | Retorna a combinação de 2 cores | 
| `HSVColor[]` | Complementary(`Boolean` ExcludeMe = False) | Retorna as cores complementares desta cor | 
| `HSVColor[]` | ComplementaryPallete(`Int32` Amount = 3) | Retorna uma paleta de cores complementares (complementares + monocromatica) | 
| `HSVColor[]` | CreatePallete(`String` PalleteType, `Int32` Amount = 4) | Cria uma paleta de cores usando esta cor como base e um metodo especifico | 
| `Boolean` | IsDark() | Verifica se uma cor e considerada escura | 
| `Boolean` | IsLight() | Verifica se uma cor e considerada clara | 
| `Boolean` | IsReadable(`HSVColor` BackgroundColor, `Int32` Size = 10) | Verifica se uma cor é legivel sobre outra cor | 
| `HSVColor[]` | ModColor(`Boolean` ExcludeMe, `Int32[]` Degrees) | Retorna  novas HSVColor a partir da cor atual, movendo ela N graus na roda de cores | 
| `HSVColor[]` | ModColor(`Int32[]` Degrees) | Retorna  novas HSVColor a partir da cor atual, movendo ela N graus na roda de cores | 
| `HSVColor[]` | Monochromatic(`Decimal` Amount = 4) | Retorna `` variacoes cores a partir da cor atual | 
| `HSVColor[]` | SplitComplementary(`Boolean` IncludeMe = False) | Retorna as cores split-complementares desta cor | 
| `HSVColor[]` | SplitComplementaryPallete(`Int32` Amount = 3) | Retorna uma paleta de cores split-complementares (split-complementares + monocromatica) | 
| `HSVColor[]` | Square(`Boolean` ExcludeMe = False) | Retorna as cores Quadraadas (tetradicas) desta cor | 
| `HSVColor[]` | Tetradic(`Boolean` ExcludeMe = False) | Retorna as cores Quadraadas (tetradicas) desta cor | 
| `HSVColor[]` | TetradicPallete(`Int32` Amount = 3) | Retorna uma paleta de cores tetradica (Monochromatica + Tetradica) | 
| `String` | ToString() |  | 
| `Color` | ToSystemColor() | Retorna uma `System.Drawing.Color` desta `InnerLibs.HSVColor` | 
| `HSVColor[]` | Triadic(`Boolean` ExcludeMe = False) | Retorna as cores triadicas desta cor | 
| `HSVColor[]` | TriadicPallete(`Int32` Amount = 3) | Retorna uma paleta de cores triadica (Monochromatica + Triadica) | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HSVColor` | RandomColor(`String` Name = null) | Gera uma `InnerLibs.HSVColor` aleatoria | 
| `HSVColor` | RandomTransparentColor(`String` Name = null) |  | 


## `HtmlTag`

Classe para criação de strings contendo tags HTML
```csharp
public class InnerLibs.HtmlTag

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dictionary<String, String>` | Attributes |  | 
| `String` | Class |  | 
| `String[]` | ClassArray |  | 
| `String` | InnerHtml |  | 
| `String` | TagName |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


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
| `Image` | CropToEllipsis(this `Image` Img, `Nullable<Color>` Background = null) | Corta a imagem em uma elipse | 
| `Image` | CropToSquare(this `Image` Img, `Int32` WidthHeight = 0) | Corta uma imagem para um quadrado perfeito a partir do centro | 
| `ImageCodecInfo` | GetEncoderInfo(this `ImageFormat` RawFormat) | Pega o encoder a partir de um formato de imagem | 
| `ImageFormat` | GetImageFormat(this `Image` OriginalImage) | Retorna o formato da imagem correspondente a aquela imagem | 
| `IEnumerable<Color>` | GetMostUsedColors(this `Image` Image, `Int32` Count = 10) | Retorna uma lista com as N cores mais utilizadas na imagem | 
| `IEnumerable<Color>` | GetMostUsedColors(this `Bitmap` Image) | Retorna uma lista com as N cores mais utilizadas na imagem | 
| `Dictionary<Color, Int32>` | GetMostUsedColorsIncidence(this `Bitmap` Image) | Retorna uma lista com as cores utilizadas na imagem | 
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


## `MathExt`

Módulo para calculos
```csharp
public class InnerLibs.MathExt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<Int32>` | ArithmeticProgression(`Int32` FirstNumber, `Int32` Constant, `Int32` Length) | Retorna uma progressão Aritmética com N numeros | 
| `Decimal` | Average(`Decimal[]` Values) | Tira a média de todos os números de um Array | 
| `Double` | Average(`Double[]` Values) | Tira a média de todos os números de um Array | 
| `Int32` | Average(`Int32[]` Values) | Tira a média de todos os números de um Array | 
| `Int64` | Average(`Int64[]` Values) | Tira a média de todos os números de um Array | 
| `Object` | CalculateCompoundInterest(this `Decimal` Capital, `Decimal` Rate, `Decimal` Time) | Calcula Juros compostos | 
| `Double` | CalculateDistance(this `AddressInfo` FirstLocation, `AddressInfo` SecondLocation) | Calcula a distancia entre 2 locais | 
| `Tuple<AddressInfo, AddressInfo, Decimal>` | CalculateDistanceMatrix(`AddressInfo[]` Locations) | Calcula a distancia passando por todos os pontos | 
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
| `IEnumerable<IEnumerable<T>>` | CartesianProduct(`IEnumerable`1[]` Sets) | Retorna todas as possiveis combinações de Arrays do mesmo tipo (Produto Cartesiano) | 
| `Decimal` | Ceil(this `Decimal` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Double` | Ceil(this `Double` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int32` | CeilInt(this `Double` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int32` | CeilInt(this `Decimal` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int64` | CeilLong(this `Double` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int64` | CeilLong(this `Decimal` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int32` | DifferenceIfMax(this `Int32` Total, `Int32` MaxValue) | Retorna a diferença entre 2 numeros se o valor maximo for menor que o total | 
| `Int32` | DifferenceIfMin(this `Int32` Total, `Int32` MinValue) | Retorna a diferença entre 2 numeros se o valor minimo for maior que o total | 
| `Object` | EvaluateExpression(`String` Formula, `Boolean` Exception = False) | Executa uma Expressão matematica/lógica simples | 
| `T` | EvaluateExpression(`String` Formula, `Boolean` Exception = False) | Executa uma Expressão matematica/lógica simples | 
| `Int32` | Factorial(this `Int32` Number) | Calcula o fatorial de um numero | 
| `IEnumerable<Int32>` | Fibonacci(`Int32` Length) | Retorna uma sequencia Fibonacci de N numeros | 
| `Decimal` | Floor(this `Decimal` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Double` | Floor(this `Double` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Int32` | FloorInt(this `Double` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Int32` | FloorInt(this `Decimal` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Int64` | FloorLong(this `Double` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Int64` | FloorLong(this `Decimal` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Decimal` | ForcePositive(`Decimal` Value) |  | 
| `Int32` | ForcePositive(`Int32` Value) |  | 
| `Double` | ForcePositive(`Double` Value) |  | 
| `Single` | ForcePositive(`Single` Value) |  | 
| `Int16` | ForcePositive(`Int16` Value) |  | 
| `IEnumerable<Int32>` | GeometricProgression(`Int32` FirstNumber, `Int32` Constant, `Int32` Length) | Retorna uma Progressão Gemoétrica com N numeros | 
| `Int64` | GetDecimalPlaces(this `Decimal` Value, `Int32` DecimalPlaces = 0) | Retorna um numero inteiro representando a parte decimal de um numero decimal | 
| `Boolean` | HasDecimalPart(this `Decimal` Value) | Verifica se um numero possui parte decimal | 
| `Boolean` | HasDecimalPart(this `Double` Value) | Verifica se um numero possui parte decimal | 
| `Boolean` | IsWholeNumber(this `Decimal` Number) |  | 
| `Boolean` | IsWholeNumber(this `Double` Number) |  | 
| `Single` | Lerp(this `Single` Start, `Single` End, `Single` Amount) | Realiza um calculo de interpolação Linear | 
| `Int32` | LimitIndex(this `Int32` Int, `IEnumerable<AnyType>` Collection) |  | 
| `Int64` | LimitIndex(this `Int64` Lng, `IEnumerable<AnyType>` Collection) |  | 
| `T` | LimitRange(this `IComparable` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) | Limita um range para um numero | 
| `Int32` | LimitRange(this `Int32` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) | Limita um range para um numero | 
| `Decimal` | LimitRange(this `Decimal` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) | Limita um range para um numero | 
| `Int64` | LimitRange(this `Double` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) | Limita um range para um numero | 
| `Int64` | LimitRange(this `Int64` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) | Limita um range para um numero | 
| `DateTime` | LimitRange(this `DateTime` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) | Limita um range para um numero | 
| `Decimal` | RoundDecimal(this `Decimal` Number, `Nullable<Int32>` Decimals = null) | Arredonda um numero para o valor inteiro mais próximo | 
| `Double` | RoundDouble(this `Double` Number, `Nullable<Int32>` Decimals = null) | Arredonda um numero para o valor inteiro mais próximo | 
| `Int32` | RoundInt(this `Decimal` Number) | Arredonda um numero para o valor inteiro mais próximo | 
| `Int32` | RoundInt(this `Double` Number) | Arredonda um numero para o valor inteiro mais próximo | 
| `Int64` | RoundLong(this `Decimal` Number) | Arredonda um numero para o valor inteiro mais próximo | 
| `Int64` | RoundLong(this `Double` Number) | Arredonda um numero para o valor inteiro mais próximo | 
| `T` | SetMaxValue(this `T` Number, `T` MaxValue) | Limita o valor Maximo de um numero | 
| `T` | SetMinValue(this `T` Number, `T` MinValue) | Limita o valor minimo de um numero | 
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

Estrutura que representa valores em dinheiro de uma determinada `System.Globalization.CultureInfo`.
```csharp
public struct InnerLibs.Money

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `CultureInfo` | Culture | Cultura correspondente a esta moeda | 
| `String` | MoneyString | String do valor formatado como moeda | 
| `RegionInfo` | Region | Região correspondente a essa moeda | 
| `Decimal` | Value |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Money` | ConvertFromCurrency(`Decimal` Base, `String` Culture) |  | 
| `Money` | ConvertFromCurrency(`Decimal` Base, `CultureInfo` Culture) |  | 
| `Money` | ConvertToCurrency(`Decimal` Base, `String` Culture) |  | 
| `Money` | ConvertToCurrency(`Decimal` Base, `CultureInfo` Culture) |  | 
| `Boolean` | Equals(`Object` obj) | Compara se 2 valores são iguais (mesmo valor e moeda) | 
| `String` | ToString() | String do valor formatado como moeda, é um alias para `InnerLibs.Money.MoneyString` | 
| `String` | ToString(`Int32` Precision) | String do valor formatado como moeda, é um alias para `InnerLibs.Money.MoneyString` | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<CultureInfo>` | GetCultureInfosByCurrencySymbol(`String` Currency) | Traz uma lista de `System.Globalization.CultureInfo` que utilizam uma determinada moeda de acordo com o simbolo, simbolo ISO ou | 


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


## `Paragraph`

```csharp
public class InnerLibs.Paragraph
    : List<Sentence>, IList<Sentence>, ICollection<Sentence>, IEnumerable<Sentence>, IEnumerable, IList, ICollection, IReadOnlyList<Sentence>, IReadOnlyCollection<Sentence>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `StructuredText` | StructuredText |  | 
| `Int32` | WordCount |  | 
| `IEnumerable<String>` | Words |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `String` | ToString(`Int32` Ident) |  | 


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
| `String` | ToString() |  | 


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
| `String` | ToString(`Int64` Number) |  | 
| `String` | ToString(`Decimal` Number) |  | 
| `String` | ToString(`Int16` Number) |  | 
| `String` | ToString(`Int32` Number) |  | 
| `String` | ToString(`Double` Number) |  | 
| `String` | ToString(`Single` Number) |  | 


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
| `String` | UnknowName |  | 
| `Nullable<Decimal>` | UnknowValue |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `RuleOfThree` | Resolve() | Atualiza o campo nulo da `InnerLibs.EquationPair` corrspondente pelo `InnerLibs.RuleOfThree.UnknowValue` | 
| `Nullable`1[][]` | ToArray() |  | 
| `Nullable`1[]` | ToFlatArray() |  | 
| `String` | ToString() |  | 


## `SelfKeyDictionary<KeyType, ClassType>`

Uma estrutura `System.Collections.IDictionary` que utiliza como Key uma propriedade de Value
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
| `IEnumerable<KeyType>` | AddRange(`ClassType[]` Values) |  | 
| `IEnumerable<KeyType>` | AddRange(`IEnumerable<ClassType>` Values) |  | 
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
| `Int32` | WordCount |  | 
| `IEnumerable<String>` | Words |  | 


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

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | BreakLinesBetweenParagraph |  | 
| `Int32` | Ident |  | 
| `String` | OriginalText |  | 
| `String` | Text |  | 
| `Int32` | WordCount |  | 
| `IEnumerable<String>` | Words |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Retorna o texto corretamente formatado | 


## `Text`

Modulo de manipulação de Texto
```csharp
public class InnerLibs.Text

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | AlphaChars |  | 
| `IEnumerable<String>` | AlphaLowerChars |  | 
| `IEnumerable<String>` | AlphaUpperChars |  | 
| `IEnumerable<String>` | BreakLineChars |  | 
| `IEnumerable<String>` | CloseWrappers |  | 
| `IEnumerable<String>` | Consonants |  | 
| `IEnumerable<String>` | EndOfSentencePunctuation |  | 
| `IEnumerable<String>` | MidSentencePunctuation |  | 
| `IEnumerable<String>` | NumberChars |  | 
| `IEnumerable<String>` | OpenWrappers |  | 
| `IEnumerable<String>` | Vowels |  | 
| `IEnumerable<String>` | WhiteSpaceChars | Caracteres em branco | 
| `IEnumerable<String>` | WordSplitters | Strings utilizadas para descobrir as palavras em uma string | 
| `IEnumerable<String>` | WordWrappers | Caracteres usado para encapsular palavras em textos | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | AdjustBlankSpaces(this `String` Text) |  | 
| `String` | AdjustPathChars(this `String` Text, `Boolean` InvertedBar = False) | Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos | 
| `String` | AdjustWhiteSpaces(this `String` Text) |  | 
| `String` | Alphabetize(this `String` Text) | Retorna uma string em ordem afabética baseada em uma outra string | 
| `String` | Append(this `String` Text, `String` AppendText) | Adiciona texto ao fim de uma string | 
| `String` | AppendIf(this `String` Text, `String` AppendText, `Boolean` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | AppendIf(this `String` Text, `String` AppendText, `Func<String, Boolean>` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | AppendLine(this `String` Text, `String` AppendText) | Adiciona texto ao final de uma string com uma quebra de linha no final do `` | 
| `String` | AppendUrlParameter(this `String` Url, `String` Key, `String[]` Value) |  | 
| `String` | AppendWhile(this `String` Text, `String` AppendText, `Func<String, Boolean>` Test) | Adiciona texto ao final de uma string enquanto um criterio for cumprido | 
| `String` | ApplySpaceOnWrapChars(this `String` Text) | Aplica espacos em todos os caracteres de encapsulamento | 
| `String` | Brackfy(this `String` Text, `Char` BracketChar = {) | Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) é um alias de `InnerLibs.Text.Quote(System.String,System.Char)` | 
| `String` | CamelAdjust(this `String` Text) | Separa as palavras de um texto CamelCase a partir de suas letras maíusculas | 
| `IEnumerable<String>` | CamelSplit(this `String` Text) | Transforma um texto em CamelCase em um array de palavras  a partir de suas letras maíusculas | 
| `String` | Censor(this `String` Text, `IEnumerable<String>` BadWords, `String` CensorshipCharacter = *, `Boolean&` IsCensored = False) | Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro  caractere desejado) e retorna um valor indicando se o texto precisou ser censurado | 
| `String` | Censor(this `String` Text, `String` CensorshipCharacter, `String[]` BadWords) | Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro  caractere desejado) e retorna um valor indicando se o texto precisou ser censurado | 
| `Boolean` | Contains(this `String` Text, `String` OtherText, `StringComparison` StringComparison) | Verifica se um texto contém outro | 
| `Boolean` | ContainsAll(this `String` Text, `String[]` Values) | Verifica se uma String contém todos os valores especificados | 
| `Boolean` | ContainsAll(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma String contém todos os valores especificados | 
| `Boolean` | ContainsAny(this `String` Text, `String[]` Values) | Verifica se uma String contém qualquer um dos valores especificados | 
| `Boolean` | ContainsAny(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma String contém qualquer um dos valores especificados | 
| `Boolean` | ContainsMost(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma string contém a maioria dos valores especificados | 
| `Boolean` | ContainsMost(this `String` Text, `String[]` Values) | Verifica se uma string contém a maioria dos valores especificados | 
| `Int32` | CountCharacter(this `String` Text, `Char` Character) | Conta os caracters especificos de uma string | 
| `Dictionary<String, Int64>` | CountWords(this `String` Text, `Boolean` RemoveDiacritics = True, `String[]` Words = null) | Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade | 
| `Boolean` | CrossContains(this `String` Text, `String` OtherText, `StringComparison` StringComparison = InvariantCultureIgnoreCase) | Verifica se um texto contém outro ou vice versa | 
| `String` | DeleteLine(this `String` Text, `Int32` LineIndex) | Remove uma linha especifica de um texto | 
| `Dictionary<String, Int64>` | DistinctCount(`String[]` List) | Cria um dicionário com as palavras de uma lista e a quantidade de cada uma. | 
| `Dictionary<String, Int64>` | DistinctCount(this `String` Phrase) | Cria um dicionário com as palavras de uma lista e a quantidade de cada uma. | 
| `Boolean` | EndsWithAny(this `String` Text, `String[]` Words) | Verifica se uma string termina com alguma outra string de um array | 
| `String` | EscapeQuotesToQuery(this `String` Text) | Prepara uma string com aspas simples para uma Query TransactSQL | 
| `String[]` | FindByRegex(this `String` Text, `String` Regex, `RegexOptions` RegexOptions = None) | Procura CEPs em uma string | 
| `String[]` | FindCEP(this `String` Text) | Procura CEPs em uma string | 
| `IEnumerable<String>` | FindNumbers(this `String` Text) | Procura numeros em uma string e retorna um array deles | 
| `String[]` | FindTelephoneNumbers(this `String` Text) | Procurea numeros de telefone em um texto | 
| `String` | FixBreakLines(this `String` Text) | Transforma quebras de linha HTML em quebras de linha comuns ao .net | 
| `String` | FixCaptalization(this `String` Text) |  | 
| `String` | FixPunctuation(this `String&` Text, `String` Punctuation = ., `Boolean` ForceSpecificPunctuation = False) | Adciona pontuaçao ao final de uma string se a mesma não terminar com alguma pontuacao. | 
| `String` | FixText(this `String` Text, `Int32` Ident = 0, `Int32` BreakLinesBetweenParagraph = 0) | Arruma a ortografia do texto captalizando corretamente, adcionando pontução ao final de frase  caso nescessário e removendo espaços excessivos ou incorretos | 
| `String` | ForEachLine(this `String` Text, `Expression<Func<String, String>>` Action) |  | 
| `String` | Format(this `String` Text, `String[]` Args) | Extension Method para `System.String` | 
| `String` | FormatCNPJ(this `Int64` CNPJ) | Formata um numero para CNPJ | 
| `String` | FormatCNPJ(this `String` CNPJ) | Formata um numero para CNPJ | 
| `String` | FormatCPF(this `Int64` CPF) | Formata um numero para CPF | 
| `String` | FormatCPF(this `String` CPF) | Formata um numero para CPF | 
| `String` | FormatCPFOrCNPJ(this `Int64` Document) | Formata um numero para CNPJ ou CNPJ se forem validos | 
| `String` | GetAfter(this `String` Text, `String` Value, `Boolean` WhiteIfNotFound = False) | Retorna um texto posterior a outro | 
| `String[]` | GetAllBetween(this `String` Text, `String` Before, `String` After = ) | Retorna todas as ocorrencias de um texto entre dois textos | 
| `String` | GetBefore(this `String` Text, `String` Value, `Boolean` WhiteIfNotFound = False) | Retorna um texto anterior a outro | 
| `String` | GetBetween(this `String` Text, `String` Before, `String` After) | Retorna o texto entre dois textos | 
| `String` | GetDomain(this `Uri` URL, `Boolean` RemoveFirstSubdomain = False) | Pega o dominio principal de uma URL | 
| `String` | GetDomain(this `String` URL, `Boolean` RemoveFirstSubdomain = False) | Pega o dominio principal de uma URL | 
| `String` | GetFirstChars(this `String` Text, `Int32` Number = 1) |  | 
| `String` | GetLastChars(this `String` Text, `Int32` Number = 1) |  | 
| `String` | GetMiddleChars(this `String` Text, `Int32` Length) | Retorna N caracteres de uma string a partir do caractere encontrado no centro | 
| `String` | GetOppositeWrapChar(this `String` Text) | Retorna o caractere de encapsulamento oposto ao caractere indicado | 
| `Type` | GetRandomItem(this `IEnumerable<Type>` List) | Sorteia um item da Lista | 
| `Type` | GetRandomItem(this `Type[]` Array) | Sorteia um item da Lista | 
| `String` | GetRelativeURL(this `String` URL) | Retorna o caminho relativo da url | 
| `String` | GetRelativeURL(this `Uri` URL) | Retorna o caminho relativo da url | 
| `IOrderedEnumerable<String>` | GetWords(this `String` Text) | Retorna uma lista de palavras encontradas no texto em ordem alfabetica | 
| `String[]` | GetWrappedText(this `String` Text, `String` Character = ", `Boolean` ExcludeWrapChars = True) | Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em um texto | 
| `String` | HtmlDecode(this `String` Text) | Retorna um texto com entidades HTML convertidas para caracteres e tags BR em breaklines | 
| `String` | HtmlEncode(this `String` Text) | Escapa o texto HTML | 
| `String` | Inject(this `String` formatString, `Object` injectionObject) |  | 
| `String` | Inject(this `String` formatString, `IDictionary` dictionary) |  | 
| `String` | Inject(this `String` formatString, `Hashtable` attributes) |  | 
| `String` | InjectSingleValue(this `String` formatString, `String` key, `Object` replacementValue) |  | 
| `Boolean` | IsAnagramOf(this `String` Text, `String` AnotherText) | Verifica se uma palavra é um Anagrama de outra palavra | 
| `Boolean` | IsAny(this `String` Text, `String[]` Texts) | Compara se uma string é igual a outras strings | 
| `Boolean` | IsAny(this `String` Text, `StringComparison` Comparison, `String[]` Texts) | Compara se uma string é igual a outras strings | 
| `String` | IsCloseWrapChar(this `String` Text) |  | 
| `Boolean` | IsLikeAny(this `String` Text, `IEnumerable<String>` Patterns) | Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga | 
| `Boolean` | IsLikeAny(this `String` Text, `String[]` Patterns) | Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga | 
| `Boolean` | IsNotAny(this `String` Text, `String[]` Texts) | Compara se uma string nao é igual a outras strings | 
| `Boolean` | IsNotAny(this `String` Text, `StringComparison` Comparison, `String[]` Texts) | Compara se uma string nao é igual a outras strings | 
| `String` | IsOpenWrapChar(this `String` Text) | Retorna o caractere de encapsulamento oposto ao caractere indicado | 
| `Boolean` | IsPalindrome(this `String` Text, `Boolean` IgnoreWhiteSpaces = False) | Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da esqueda  para direita | 
| `String` | Join(this `IEnumerable<Type>` Array, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(this `Type[]` Array, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(`String` Separator, `Type[]` Array) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(this `List<Type>` List, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `Int32` | LevenshteinDistance(this `String` Text1, `String` Text2) | Computa a distancia de Levenshtein entre 2 strings. | 
| `Boolean` | Like(this `String` Text, `String` OtherText) | operador LIKE do VB para C# em forma de extension method | 
| `String` | MaskTelephoneNumber(this `String` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Int64` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Int32` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Decimal` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | MaskTelephoneNumber(this `Double` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ParseAlphaNumeric(this `String` Text) | limpa um texto deixando apenas os caracteres alfanumericos. | 
| `ConnectionStringParser` | ParseConnectionString(this `String` ConnectionString) | Parseia uma ConnectionString em um Dicionário | 
| `String` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) | Remove caracteres não numéricos de uma string | 
| `Type` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) | Remove caracteres não numéricos de uma string | 
| `NameValueCollection` | ParseQueryString(this `String` Querystring) |  | 
| `String` | Poopfy(`String[]` Words) | Retorna uma string em sua forma poop | 
| `String` | Poopfy(this `String` Text) | Retorna uma string em sua forma poop | 
| `String` | PreetyPrint(this `XmlDocument` Document) | Return a Idented XML string | 
| `String` | Prepend(this `String` Text, `String` PrependText) | Adiciona texto ao começo de uma string | 
| `String` | PrependIf(this `String` Text, `String` PrependText, `Boolean` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | PrependIf(this `String` Text, `String` PrependText, `Func<String, Boolean>` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | PrependLine(this `String` Text, `String` AppendText) | Adiciona texto ao inicio de uma string com uma quebra de linha no final do `` | 
| `String` | PrependWhile(this `String` Text, `String` PrependText, `Func<String, Boolean>` Test) | Adiciona texto ao inicio de uma string enquanto um criterio for cumprido | 
| `String` | PrintIf(this `String` Text, `Boolean` BooleanValue) | Retorna a string especificada se o valor booleano for verdadeiro | 
| `String` | QuantifyText(this `FormattableString` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `String` PluralText, `Object` Quantity) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `IEnumerable<T>` List, `String` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `Int32` Quantity, `String` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `Decimal` Quantity, `String` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `Int16` Quantity, `String` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `Int64` Quantity, `String` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | QuantifyText(this `Double` Quantity, `String` PluralText) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado no parametro. | 
| `String` | Quote(this `String` Text, `Char` OpenQuoteChar = ") | Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) | 
| `String` | QuoteIf(this `String` Text, `Boolean` Condition, `String` QuoteChar = ") | Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes) se uma  condiçao for cumprida | 
| `Type` | RandomItem(`Type[]` Array) | Sorteia um item da Matriz | 
| `String` | RegexEscape(this `String` Text) | Escapa caracteres exclusivos de uma regex | 
| `String` | RemoveAccents(this `String` Text) | Remove os acentos de uma string | 
| `String` | RemoveAny(this `String` Text, `String[]` Values) | Remove várias strings de uma string | 
| `String` | RemoveDiacritics(this `String&` Text) | Remove os acentos de uma string | 
| `String` | RemoveFirstAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StartStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveFirstAny(this `String` Text, `String[]` StartStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveFirstChars(this `String` Text, `Int32` Quantity = 1) | Remove os X primeiros caracteres | 
| `String` | RemoveFirstEqual(this `String` Text, `String` StartStringTest) | Remove um texto do inicio de uma string se ele for um outro texto especificado | 
| `String` | RemoveHTML(this `String` Text) |  | 
| `String` | RemoveLastAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` EndStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveLastAny(this `String` Text, `String[]` EndStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveLastChars(this `String` Text, `Int32` Quantity = 1) | Remove os X ultimos caracteres | 
| `String` | RemoveLastEqual(this `String` Text, `String` EndStringTest) | Remove um texto do final de uma string se ele for um outro texto | 
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
| `String` | SensitiveReplace(this `String` Text, `String` OldValue, `String` NewValue, `StringComparison` ComparisonType = InvariantCulture) | Realiza um replace em uma string usando um tipo especifico de comparacao | 
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
| `FormattableString` | ToFormattableString(this `String` Text, `Object[]` args) |  | 
| `FormattableString` | ToFormattableString(this `String` Text, `IEnumerable<Object[]>` args) |  | 
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
| `String` | UnBrackfy(this `String` Text) |  | 
| `String` | UnBrackfy(this `String` Text, `String` BracketChar, `Boolean` ContinuouslyRemove = False) |  | 
| `String` | UnQuote(this `String` Text) |  | 
| `String` | UnQuote(this `String` Text, `String` OpenQuoteChar, `Boolean` ContinuouslyRemove = False) |  | 
| `String` | UnWrap(this `String` Text, `String` WrapText = ", `Boolean` ContinuouslyRemove = False) |  | 
| `String` | UrlDecode(this `String` Text) | Decoda uma string de uma transmissão por URL | 
| `String` | UrlEncode(this `String` Text) | Encoda uma string para transmissão por URL | 
| `String` | Wrap(this `String` Text, `String` WrapText = ") | Encapsula um tento entre 2 textos | 
| `String` | Wrap(this `String` Text, `String` OpenWrapText, `String` CloseWrapText) | Encapsula um tento entre 2 textos | 
| `HtmlTag` | WrapInTag(this `String` Text, `String` TagName) |  | 


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


## `vAddress`

```csharp
public class InnerLibs.vAddress
    : AddressInfo

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | AddressLabel |  | 
| `String` | AddressName |  | 
| `vAddressTypes` | AddressType |  | 
| `vLocations` | Location |  | 
| `Boolean` | Preferred |  | 
| `String` | StreetAddress |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `vAddressTypes`

```csharp
public enum InnerLibs.vAddressTypes
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | PARCEL |  | 
| `1` | DOM |  | 
| `2` | INT |  | 


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
| `List<vAddress>` | Addresses |  | 
| `Nullable<DateTime>` | Birthday |  | 
| `String` | Company |  | 
| `String` | Department |  | 
| `List<vEmail>` | Emails |  | 
| `String` | FirstName |  | 
| `String` | FormattedName |  | 
| `String` | Gender |  | 
| `String` | JobTitle |  | 
| `DateTime` | LastModified |  | 
| `String` | LastName |  | 
| `String` | MiddleName |  | 
| `String` | Nickname |  | 
| `String` | Note |  | 
| `String` | Organization |  | 
| `String` | OrganizationalUnit |  | 
| `String` | Profession |  | 
| `String` | Role |  | 
| `List<vSocial>` | Social |  | 
| `String` | Suffix |  | 
| `List<vTelephone>` | Telephones |  | 
| `String` | Title |  | 
| `Nullable<Guid>` | UID |  | 
| `List<vURL>` | URLs |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `vEmail` | AddEmail(`String` Email) |  | 
| `vSocial` | AddSocial(`String` Name, `String` URL) |  | 
| `vTelephone` | AddTelephone(`String` Tel) |  | 
| `vURL` | AddURL(`String` URL) |  | 
| `FileInfo` | ToFile(`String` FullPath) |  | 
| `String` | ToString() |  | 


## `vEmail`

```csharp
public class InnerLibs.vEmail

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | EmailAddress |  | 
| `Boolean` | Preferred |  | 
| `String` | Type |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `Verify`

Verifica determinados valores como Arquivos, Numeros e URLs
```csharp
public class InnerLibs.Verify

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | CanBeNumber(this `Object` Value) | Verifica se o valor é um numero ou pode ser convertido em numero | 
| `Int32` | GetIndexOf(this `IEnumerable<T>` Arr, `T` item) | Tenta retornar um index de um IEnumerable a partir de um valor especifico. retorna -1 se o index nao existir | 
| `T` | IfBlank(this `Object` Value, `T` ValueIfBlank = null) | Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE | 
| `T` | IfBlankOrNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfBlankOrNoIndex) | Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um valor default se o index nao existir ou seu valor for branco ou nothing | 
| `T` | IfNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfNoIndex = null) | Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um valor default se o index nao existir | 
| `T[]` | IfNullOrEmpty(this `Object[]` Value, `T[]` ValuesIfBlank) | Verifica se um aray está vazio ou nula e retorna um outro valor caso TRUE | 
| `IEnumerable<T>` | IfNullOrEmpty(this `IEnumerable<Object[]>` Value, `T[]` ValuesIfBlank) | Verifica se um aray está vazio ou nula e retorna um outro valor caso TRUE | 
| `IEnumerable<T>` | IfNullOrEmpty(this `IEnumerable<Object[]>` Value, `IEnumerable<T>` ValueIfBlank) | Verifica se um aray está vazio ou nula e retorna um outro valor caso TRUE | 
| `Boolean` | IsArray(`T` Obj) |  | 
| `Boolean` | IsBlank(this `String` Text) | Verifica se uma String está em branco | 
| `Boolean` | IsBlank(this `FormattableString` Text) | Verifica se uma String está em branco | 
| `Boolean` | IsBoolean(this `T` Obj) |  | 
| `Boolean` | IsDate(this `String` Obj) |  | 
| `Boolean` | IsDate(this `T` Obj) |  | 
| `Boolean` | IsDirectoryPath(this `String` Text) | Verifica se uma string é um caminho de diretório válido | 
| `Boolean` | IsEmail(this `String` Text) | Verifica se um determinado texto é um email | 
| `Boolean` | IsEven(this `Decimal` Value) | Verifica se um numero é par | 
| `Boolean` | IsEven(this `Int32` Value) | Verifica se um numero é par | 
| `Boolean` | IsEven(this `Int64` Value) | Verifica se um numero é par | 
| `Boolean` | IsEven(this `Double` Value) | Verifica se um numero é par | 
| `Boolean` | IsFilePath(this `String` Text) | Verifica se uma string é um caminho de arquivo válido | 
| `Boolean` | IsInUse(this `FileInfo` File) | Verifica se o arquivo está em uso por outro procedimento | 
| `Boolean` | IsIP(this `String` IP) | Verifica se a string é um endereço IP válido | 
| `Boolean` | IsNotBlank(this `String` Text) | Verifica se uma String não está em branco | 
| `Boolean` | IsNotBlank(this `FormattableString` Text) | Verifica se uma String não está em branco | 
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
| `String` | NullIf(this `String` Value, `String` TestValue, `StringComparison` ComparisonType = InvariantCultureIgnoreCase) | Anula o valor de um objeto se ele for igual a outro objeto | 


## `vLocations`

```csharp
public enum InnerLibs.vLocations
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | HOME |  | 
| `1` | WORK |  | 
| `2` | CELL |  | 


## `vPhoneTypes`

```csharp
public enum InnerLibs.vPhoneTypes
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | VOICE |  | 
| `1` | FAX |  | 
| `2` | MSG |  | 
| `3` | PAGER |  | 
| `4` | BBS |  | 
| `5` | MODEM |  | 
| `6` | CAR |  | 
| `7` | ISDN |  | 
| `8` | VIDEO |  | 


## `vSocial`

```csharp
public class InnerLibs.vSocial

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Name |  | 
| `String` | URL |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `vTelephone`

```csharp
public class InnerLibs.vTelephone

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `vLocations` | Location |  | 
| `Boolean` | Preferred |  | 
| `String` | TelephoneNumber |  | 
| `vPhoneTypes` | Type |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `vURL`

```csharp
public class InnerLibs.vURL

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `vLocations` | Location |  | 
| `Boolean` | Preferred |  | 
| `String` | URL |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `Web`

Modulo Web
```csharp
public class InnerLibs.Web

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Uri` | AddParameter(this `Uri` Url, `String` Key, `Boolean` Append, `String[]` Values) | Adciona um parametro a Query String de uma URL | 
| `Uri` | AddParameter(this `Uri` Url, `String` Key, `String[]` Values) | Adciona um parametro a Query String de uma URL | 
| `String` | FileNameAsTitle(this `FileSystemInfo` Info) | Retorna o Titulo do arquivo a partir do nome do arquivo | 
| `String` | FileNameAsTitle(this `String` FileName) | Retorna o Titulo do arquivo a partir do nome do arquivo | 
| `String` | GetFacebookUsername(this `String` URL) | Captura o Username ou UserID de uma URL do Facebook | 
| `String` | GetFacebookUsername(this `Uri` URL) | Captura o Username ou UserID de uma URL do Facebook | 
| `Byte[]` | GetFile(`String` URL) |  | 
| `Image` | GetImage(`String` URL) |  | 
| `IEnumerable<String>` | GetLocalIP() |  | 
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
| `NameValueCollection` | ParseQueryString(this `Uri` URL) |  | 
| `Uri` | RemoveParameter(this `Uri` Url, `String[]` Keys) | Adciona um parametro a Query String de uma URL | 
| `String` | RemoveUrlParameters(this `String` UrlPattern) |  | 
| `String` | ReplaceUrlParameters(this `String` UrlPattern, `T` obj) | Substitui os parametros de rota de uma URL por valores de um objeto | 


