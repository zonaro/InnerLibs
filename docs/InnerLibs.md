## `AJAX`

Métodos de requisição
```csharp
public class InnerLibs.AJAX

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FileInfo` | DownloadFile(`String` URL, `String` FilePath, `String` Method = GET, `NameValueCollection` Parameters = null, `String` ContentType = multipart/form-data, `Encoding` Encoding = null) | Faz o download de um arquivo diretamente em um diretório | 
| `Type` | GET(`String` URL, `Encoding` Encoding = null) | Realiza um GET em uma URL | 
| `Type` | POST(`String` URL, `NameValueCollection` Parameters, `Encoding` Encoding = null) | Realiza um POST em uma URL e retorna um Objeto convertido para o tipo especificado | 
| `Type` | Request(`String` URL, `String` Method = GET, `NameValueCollection` Parameters = null, `String` ContentType = application/x-www-form-urlencoded, `Encoding` Encoding = null, `String` FilePath = , `WebHeaderCollection` Header = null) | Retorna o conteúdo de uma página | 


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


## `AsxEntry`

```csharp
public class InnerLibs.AsxEntry

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Author |  | 
| `String` | Copyright |  | 
| `String` | Title |  | 
| `Uri` | URL |  | 


## `AsxFile`

```csharp
public class InnerLibs.AsxFile

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<AsxEntry>` | Entries |  | 
| `String` | Title |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AsxEntry` | AddEntry(`String` Title, `Uri` Url, `String` Author = , `String` Copyright = ) |  | 
| `Byte[]` | AsBytes(`Encoding` Encoding = null) |  | 
| `String` | ToString() |  | 
| `FileInfo` | WriteFile(`String` FilePath) |  | 


## `Base64`

Modulo para manipulação de imagens e Strings Base64
```csharp
public class InnerLibs.Base64

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Atob(`String` Base, `Encoding` Encoding = null) | Decoda uma string em Base64 | 
| `String` | Btoa(`String` Text, `Encoding` Encoding = null) | Encoda uma string em Base64 | 
| `FileInfo` | CreateFileFromDataURL(this `String` Base64StringOrDataURL, `String` FilePath) | Cria um arquivo fisico a partir de uma Base64 ou DataURL | 
| `String` | FixBase64(this `String` Base64StringOrDataUrl) | Arruma os caracteres de uma string Base64 | 
| `String` | ToBase64(this `Byte[]` Bytes) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `Image` OriginalImage, `ImageFormat` OriginalImageFormat) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `Image` OriginalImage) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `Uri` ImageURL) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `String` ImageURL, `ImageFormat` OriginalImageFormat) | Converte um Array de Bytes em uma string Base64 | 
| `String` | ToBase64(this `HttpPostedFile` PostedFile, `Boolean` DataUrl = False) | Converte um Array de Bytes em uma string Base64 | 
| `Byte[]` | ToBytes(this `String` Base64StringOrDataURL) | Converte uma DATAURL ou Base64 String em um array de Bytes | 
| `String` | ToDataURL(this `Byte[]` Bytes, `FileType` Type = null) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `FileInfo` File) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `Image` Image) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `Image` OriginalImage, `ImageFormat` OriginalImageFormat) | Converte um Array de Bytes em uma DATA URL Completa | 
| `String` | ToDataURL(this `HttpPostedFile` PostedFile) | Converte um Array de Bytes em uma DATA URL Completa | 
| `Image` | ToImage(this `String` DataUrlOrBase64String, `Int32` Width = 0, `Int32` Height = 0) | Converte uma String DataURL ou Base64 para Imagem | 
| `Image` | ToImage(this `HttpPostedFile` PostedFile) | Converte uma String DataURL ou Base64 para Imagem | 
| `Image` | ToImage(this `Byte[]` Bytes) | Converte uma String DataURL ou Base64 para Imagem | 


## `BrowserClipper`

```csharp
public class InnerLibs.BrowserClipper

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Image` | Capture(`String` URL, `Int32` DelaySeconds = 5) | Cria um snapshot de uma pagina da web a partir de uma URL | 
| `Image` | Capture(`WebBrowser` Browser, `Int32` DelaySeconds = 5) | Cria um snapshot de uma pagina da web a partir de uma URL | 
| `String` | GetHtmlContent(`String` URL, `Int32` DelaySeconds = 0) | Pega o conteudo HTML de uma página da web logo após seu carregamento | 
| `String` | GetTitle(`String` URL) | Pega o titulo de uma página da web | 


## `Calendars`

Modulo para manipulação de calendário
```csharp
public class InnerLibs.Calendars

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
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
| `List<DateTime>` | ClearTime(this `List<DateTime>` List) | Remove o tempo de todas as datas de uma lista e retorna uma nova lista | 
| `void` | FillWith(this `HtmlSelect` Box, `CalendarType` CalendarType, `TypeOfFill` TextType = LongName, `TypeOfFill` ValueType = Number) | Preenche um HtmlSelect com MESES ou DIAS DA SEMANA | 
| `void` | FixDateOrder(`DateTime&` StartDate, `DateTime&` EndDate) | Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate  sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um  `System.TimeSpan` negativo | 
| `List<DateTime>` | GetBetween(this `DateTime` StartDate, `DateTime` EndDate, `DayOfWeek[]` Days) | Retorna uma lista com as datas de dias especificos da semana entre 2 datas | 
| `TimeFlow` | GetDifference(this `DateTime` InitialDate, `DateTime` SecondDate) | Retorna uma `InnerLibs.TimeMachine.TimeFlow` com a diferença entre 2 Datas | 
| `DateTime` | GetFirstDayOfFortnight(this `DateTime` Date) | Retorna a primeira data da quinzena a partir de uma outra data | 
| `DateTime` | GetFirstDayOfMonth(this `DateTime` Date) | Retorna a primeira data do mes a partir de uma outra data | 
| `DateTime` | GetFirstDayOfWeek(this `DateTime` Date, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna o primeiro dia da semana da data especificada | 
| `DateTime` | GetLastDayOfFortnight(this `DateTime` Date) | Retorna a ultima data da quinzena a partir de uma outra data | 
| `DateTime` | GetLastDayOfMonth(this `DateTime` Date) | Retorna a ultima data do mes a partir de uma outra data | 
| `DateTime` | GetLastDayOfWeek(this `DateTime` Date, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna o primeiro dia da semana da data especificada | 
| `DateRange` | GetWeek(this `DateTime` Date, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna um DateRange equivalente a semana de uma data especifica | 
| `Int32` | GetWeekOfYear(this `DateTime` Date, `CultureInfo` Culture = null, `DayOfWeek` FirstDayOfWeek = Sunday) | Retorna o numero da semana relativa ao ano | 
| `Boolean` | IsAnniversary(this `DateTime` BirthDate, `Nullable<DateTime>` CompareWith = null) | Verifica se a Data de hoje é um aniversário | 
| `Boolean` | IsBetween(this `DateTime` MidDate, `DateTime` StartDate, `DateTime` EndDate, `Boolean` IgnoreTime = False) | Verifica se uma data se encontra entre 2 datas | 
| `Boolean` | IsSameMonth(this `DateTime` Date, `DateTime` AnotherDate) | Verifica se uma data é do mesmo mês e ano que outra data | 
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


## `ClassTools`

```csharp
public class InnerLibs.ClassTools

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | AddFontFromBytes(this `PrivateFontCollection&` FontCollection, `Byte[]` FontBytes) | Adiciona uma fonte a uma PrivateFontCollection a partir de um Resource | 
| `void` | AddFontFromResource(this `PrivateFontCollection&` FontCollection, `Assembly` Assembly, `String` FileName) | Adiciona uma fonte a uma PrivateFontCollection a partir de um Resource | 
| `T` | AsIf(this `Boolean` Bool, `T` TrueValue, `T` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `T` | AsIf(this `Nullable<Boolean>` Bool, `T` TrueValue, `T` FalseValue = null) | Retorna um valor de um tipo especifico de acordo com um valor boolean | 
| `String` | BlankCoalesce(this `String` First, `String[]` N) | Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que  possuir um valor | 
| `String` | BlankCoalesce(`String[]` N) | Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que  possuir um valor | 
| `T` | Choose(this `Boolean` BooleanValue, `T` ChooseIfTrue, `T` ChooseIfFalse) | Escolhe um valor de acordo com o resultado de uma variavel booliana | 
| `T` | Choose(this `String` Expression, `T` ChooseIfTrue, `T` ChooseIfFalse) | Escolhe um valor de acordo com o resultado de uma variavel booliana | 
| `Boolean` | ContainsAll(this `IEnumerable<Type>` List1, `IEnumerable<Type>` List2, `IEqualityComparer<Type>` Comparer = null) | Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array. | 
| `Boolean` | ContainsAny(this `IEnumerable<Type>` List1, `IEnumerable<Type>` List2, `IEqualityComparer<Type>` Comparer = null) | Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array. | 
| `Type` | CopyToObject(this `NameValueCollection` Collection, `Type&` Obj, `String[]` Keys) | Copia os valores de um `System.Collections.Specialized.NameValueCollection` para um objeto de um tipo especifico | 
| `Dictionary<String, Object>` | CreateDictionary(this `Type` Obj) | Converte uma classe para um `System.Collections.Generic.Dictionary`2` | 
| `Type` | CreateObject(this `NameValueCollection` Collection, `String[]` Keys) | Cria um objeto de um tipo especifico a partir de um `System.Collections.Specialized.NameValueCollection` | 
| `T` | Detach(this `List<T>` List, `Int32` Index) | Remove um item de uma lista e retorna este item | 
| `Dictionary<Type, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr) | Conta de maneira distinta items de uma coleçao | 
| `Dictionary<PropT, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop) | Conta de maneira distinta items de uma coleçao | 
| `T` | FirstOr(this `IEnumerable<T>` source, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `T` | FirstOr(this `IEnumerable<T>` source, `Func<T, Boolean>` predicate, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `NameValueCollection` | FlatRequest(this `HttpRequest` Request) | Cria um unico `System.Collections.Specialized.NameValueCollection` a partir de um  `System.Web.HttpRequest.QueryString` e um `System.Web.HttpRequest.Form` | 
| `TValue` | GetAttributeValue(this `Type` type, `Func<TAttribute, TValue>` ValueSelector) |  | 
| `T` | GetEnumValue(`String` Name) | Traz o valor de uma enumeração a partir de uma string | 
| `List<T>` | GetEnumValues() | Traz todos os Valores de uma enumeração | 
| `List<PropertyInfo>` | GetProperties(this `Object` MyObject, `BindingFlags` BindAttr) | Traz uma Lista com todas as propriedades de um objeto | 
| `List<PropertyInfo>` | GetProperties(this `Object` MyObject) | Traz uma Lista com todas as propriedades de um objeto | 
| `Object[]` | GetPropertyParameterFromString(this `String` Text) | Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe | 
| `Object[]` | GetPropertyParametersFromString(this `Type` Type, `String` Text) | Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe | 
| `Object` | GetPropertyValue(this `Object` MyObject, `String` PropertyName, `Type` Type, `Boolean` GetPrivate = False) | Traz o valor de uma propriedade de um objeto | 
| `Type` | GetPropertyValue(this `Object` MyObject, `String` PropertyName, `Boolean` GetPrivate = False) | Traz o valor de uma propriedade de um objeto | 
| `Object` | GetPropertyValue(this `Object` MyObject, `String` PropertyName, `Boolean` GetPrivate = True) | Traz o valor de uma propriedade de um objeto | 
| `Byte[]` | GetResourceBytes(this `Assembly` Assembly, `String` FileName) | Pega os bytes de um arquivo embutido no assembly | 
| `String` | GetResourceFileText(this `Assembly` Assembly, `String` FileName) | Pega o texto de um arquivo embutido no assembly | 
| `HtmlDocument` | GetResourceHtmlDocument(this `Assembly` Assembly, `String` FileName) | Pega o texto de um arquivo embutido no assembly | 
| `Tvalue` | GetValueOr(this `IDictionary<tkey, Tvalue>` Dic, `tkey` Key, `Tvalue` ReplaceValue = null) |  | 
| `Dictionary<Group, Dictionary<Count, Int64>>` | GroupAndCountBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector, `Func<Type, Count>` CountObjectBy) | Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada grupo a partir de outra propriedade deo mesmo objeto | 
| `Dictionary<Group, Dictionary<SubGroup, IEnumerable<Type>>>` | GroupAndSubGroupBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector, `Func<Type, SubGroup>` SubGroupSelector) | Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um grupo com subgrupos daquele objeto | 
| `Boolean` | HasProperty(this `Type` Type, `String` PropertyName, `Boolean` GetPrivate = False) | Verifica se um tipo possui uma propriedade | 
| `Boolean` | HasProperty(this `Object` Obj, `String` Name) | Verifica se um tipo possui uma propriedade | 
| `Boolean` | IsArrayOf(this `Type` Type) | Verifica se o tipo é um array de um objeto especifico | 
| `Boolean` | IsArrayOf(this `Object` Obj) | Verifica se o tipo é um array de um objeto especifico | 
| `Boolean` | IsDictionary(this `Object` obj) | Verifica se o objeto é um iDictionary | 
| `Boolean` | IsIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsIn(this `Type` Obj, `String` Text, `IEqualityComparer<Char>` Comparer = null) | Verifica se o objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsInAny(this `Type` Obj, `IEnumerable`1[]` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays. | 
| `Boolean` | IsList(this `Object` obj) | Verifica se o objeto é uma lista | 
| `Boolean` | IsNotIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) | Verifica se o não objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsNotIn(this `Type` Obj, `String` Text, `IEqualityComparer<Char>` Comparer = null) | Verifica se o não objeto existe dentro de uma Lista, coleção ou array. | 
| `Boolean` | IsNumericType(this `Type` Obj) | Verifica se o objeto é do tipo numérico. | 
| `Boolean` | IsType(this `Object` Obj) | Verifica se um objeto é de um determinado tipo | 
| `T` | LastOr(this `IEnumerable<T>` source, `T` Alternate) | Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia | 
| `T` | Map(this `Dictionary<String, v>` Dic, `T&` Obj = null) | Copia os valores de um dicionário para as propriedades de uma classe | 
| `List<T>` | Map(this `DbDataReader` Reader, `Object[]` Params) | Copia os valores de um dicionário para as propriedades de uma classe | 
| `NameValueCollection` | Merge(`NameValueCollection[]` NVC) | Mescla varios `System.Collections.Specialized.NameValueCollection` em um unico `System.Collections.Specialized.NameValueCollection` | 
| `Dictionary<String, Object>` | MergeProperties(`Object[]` Items) | Mescla varios tipos de objeto em um unico dicionario a partir de suas propriedades | 
| `T` | NullCoalesce(this `Nullable<T>` First, `Nullable`1[]` N) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `IEnumerable<Nullable<T>>` List) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `T` First, `T[]` N) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `T` | NullCoalesce(this `IEnumerable<T>` List) | Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor | 
| `Type` | NullifyProperties(this `Type` Obj) | Transforma todas as propriedades String em NULL quando suas estiverem em branco | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `Func<KeyValuePair<TKey, TValue>, Boolean>` predicate) | Remove de um dicionario as respectivas Keys se as mesmas existirem | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `TKey[]` Keys) | Remove de um dicionario as respectivas Keys se as mesmas existirem | 
| `void` | SetPropertyValue(this `Object` MyObject, `String` PropertyName, `Type` Value) | Seta o valor de uma propriedade de um objeto | 
| `void` | SetPropertyValueFromCollection(this `Object` MyObject, `String` PropertyName, `CollectionBase` Collection) |  | 
| `String` | ToFlatString(this `Object` Obj, `String` DateFormat = ) | Retorna o objeto em seu formato padrão de String, ou serializa o objeto em Json se o mesmo  não possuir formato em string | 


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


## `Converter`

```csharp
public class InnerLibs.Converter

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ToType[]` | ChangeArrayType(this `FromType[]` Value) | Converte um array de um tipo para outro | 
| `ToType` | ChangeType(this `FromType` Value) | Converte um tipo para outro. Retorna Nothing (NULL) se a covnersão falhar | 
| `List<T>` | DefineEmptyList(this `T` ObjectForDefinition) | Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos | 
| `Object[]` | ForceArray(`Object` Obj) | Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto | 
| `OutputType[]` | ForceArray(`Object` Obj) | Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto | 
| `Dictionary<Tkey, Object>` | Merge(this `Dictionary<Tkey, Object>` FirstDictionary, `Dictionary`2[]` Dictionaries) | Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um dicionario os valores sao agrupados em arrays | 
| `void` | SetPropertiesIn(this `IDictionary<String, Object>` Dic, `T` Obj) | Seta as propriedades de uma classe a partir de um dictionary | 
| `T` | SetPropertiesIn(this `HttpRequest` Request, `T&` Obj, `String[]` Keys) | Seta as propriedades de uma classe a partir de um dictionary | 
| `Dictionary<TKey, List<TValue>>` | ToDictionary(this `IEnumerable<IGrouping<TKey, TValue>>` groupings) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `Dictionary<String, Object>` | ToDictionary(this `HttpRequest` Request, `String[]` keys) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `Dictionary<TKey, TValue>` | ToDictionary(this `IEnumerable<KeyValuePair<TKey, TValue>>` items) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `Dictionary<String, Object>` | ToDictionary(this `NameValueCollection` NameValueCollection, `String[]` Keys) | Returna um `System.Collections.Generic.Dictionary`2` a partir de um `System.Linq.IGrouping`2` | 
| `HtmlElement` | ToHtmlTable(this `IEnumerable<Dictionary<String, Object>>` Table) | Converte uma lista de dicionários para uma tabela HTML | 
| `String` | ToJSON(this `NameValueCollection` NameValueCollection) | Converte um NameValueCollection para string JSON | 
| `String` | ToJSON(this `HttpRequest` Request) | Converte um NameValueCollection para string JSON | 
| `void` | Uniform(this `IEnumerable`1&` Dics, `TKey[]` AditionalKeys) | Aplica as mesmas keys a todos os dicionarios de uma lista | 


## `CssParser`

```csharp
public class InnerLibs.CssParser

```

## `DataBase`

```csharp
public class InnerLibs.DataBase

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Assembly` | ApplicationAssembly | Assembly da aplicação que contém os arquivos SQL | 
| `DirectoryInfo` | CommandDirectory | Diretório que contém os arquivos SQL | 
| `String` | ConnectionString | Conexão genérica (Oracle, MySQL, SQLServer etc.) | 
| `Type` | ConnectionType | Tipo da conexão | 
| `FileInfo` | LogFile | Arquivo onde serão salvos os logs | 
| `Reader` | SELECT | Insere um objeto em uma tabela a partir de suas propriedades e valores | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `DbCommand` | CreateCommandFromClass(`String` SQLQuery, `Type` Object) | Cria um comando usando como base as propriedades de uma classe | 
| `DbCommand` | CreateCommandFromDictionary(`String` SQLQuery, `IDictionary<String, Object>` Parameters) | Cria um comando SQL utilizando as key e os valores de um `System.Collections.IDictionary` | 
| `DbCommand` | CreateCommandFromRequest(`HttpRequest` Request, `String` SQLQuery, `DbParameter[]` CustomParameters) | Cria um comando SQL utilizando as key e os valores de um `System.Web.HttpRequest` | 
| `DbCommand` | CreateInsertCommand(`String` TableName, `IDictionary<String, Object>` Dic) | Cria um comando de INSERT baseado em um `System.Collections.Generic.IDictionary`2` | 
| `String` | CreateInsertCommandText(`String` TableName, `String[]` Columns) | Cria um comando de INSERT | 
| `DbParameter` | CreateParameter(`String` Name, `Object` Value) | Cria um parametro de Query SQL a partir de uma variavel convertida para um tipo especifico | 
| `DbParameter` | CreateParameter(`String` Name, `Object` Value) | Cria um parametro de Query SQL a partir de uma variavel convertida para um tipo especifico | 
| `DbCommand` | CreateUpdateCommand(`String` TableName, `String` WhereClausule, `IDictionary<String, Object>` Dic) | Cria um comando de UPDATE baseado em um `System.Collections.Generic.IDictionary`2` | 
| `String` | CreateUpdateCommandText(`String` TableName, `String` WhereClausule, `String[]` Columns) | Cria um comando de UPDATE | 
| `void` | DELETE(`String` TableName, `String` WhereConditions, `Boolean` SafeMode = True) | Deleta um registro de uma tabela | 
| `String` | GetCommand(`String` CommandFile) | Pega o comando SQL de um arquivo ou resource | 
| `List<String>` | GetSqlFilesList() | Retorna a lista de arquivos SQL disponiveis | 
| `Reader` | INSERT(`String` TableName, `IDictionary<String, Object>` Dic) | Faz um INSERT no banco de dados de acordo com um  `System.Collections.IDictionary` | 
| `String` | INSERTorUPDATE(`String` TableName, `String` PrimaryKeyColumn, `IDictionary<String, Object>` Dic) | Faz um INSERT out UPDATE no banco de dados de acordo com o valor da coluna de chave primária especificado em um `System.Collections.IDictionary` | 
| `String` | INSERTorUPDATE(`String` TableName, `String` PrimaryKeyColumn, `NameValueCollection` Columns) | Faz um INSERT out UPDATE no banco de dados de acordo com o valor da coluna de chave primária especificado em um `System.Collections.IDictionary` | 
| `String` | INSERTorUPDATE(`HttpRequest` Request, `String` TableName, `String` PrimaryKeyColumn, `String[]` Columns) | Faz um INSERT out UPDATE no banco de dados de acordo com o valor da coluna de chave primária especificado em um `System.Collections.IDictionary` | 
| `Reader` | OpenFile(`String` CommandFile, `DbParameter[]` Parameters) | Executa o comando de um arquivo SQL configurado | 
| `void` | RunProcedureForEach(`String` Procedure, `String` ForeignKey, `String` ForeignValue, `NameValueCollection` Items, `String[]` Keys) | Executa uma procedure para cada item dentro de uma coleção | 
| `Reader` | RunSQL(`String` SQLQuery) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`String` SQLQuery, `IDictionary<String, Object>` Values) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`FileInfo` File) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`HttpPostedFile` File) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`String` SQLQuery, `String` FileParameter, `Byte[]` File) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`String` SQLQuery, `String` FileParameter, `HttpPostedFile` File) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`String` SQLQuery, `String` FileParameter, `FileInfo` File) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`DbCommand` Command) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | RunSQL(`String` SQLQuery, `DbParameter[]` Parameters) | Executa uma Query no banco. Recomenda-se o uso de procedures. | 
| `Reader` | SelectAndFill(`String` TableName, `String` WhereConditions, `HtmlControl[]` Controls) | Seleciona a primeira linha de um resultset e aplica no `System.Web.UI.HtmlControls.HtmlControl` equivalente ao nome da coluna | 
| `Reader` | SelectAndFill(`String` TableName, `String` WhereConditions, `Control[]` Controls) | Seleciona a primeira linha de um resultset e aplica no `System.Web.UI.HtmlControls.HtmlControl` equivalente ao nome da coluna | 
| `Reader` | UPDATE(`String` TableName, `String` WhereClausule, `IDictionary<String, Object>` Dic) | Faz um UPDATE no banco de dados de acordo com um `System.Collections.IDictionary` | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `DataBase` | Create(`String` ConnectionString) | Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString e em um Tipo de Conexão | 
| `DataBase` | Create(`String` ConnectionString, `DirectoryInfo` CommandDirectory) | Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString e em um Tipo de Conexão | 
| `DataBase` | Create(`String` ConnectionString, `Assembly` ApplicationAssembly) | Cria uma nova instancia de Banco de Dados baseada em uma ConnectionString e em um Tipo de Conexão | 


## `DataGridViewManager`

```csharp
public class InnerLibs.DataGridViewManager

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | BuildFromAJAX(this `DataGridView&` DataGridView, `String` URL) |  | 
| `Object` | BuildFromClass(this `DataGridView&` DataGridView, `List<Type>` MyObject) | Cria as colunas e linhas de um DataGridView de acordo com uma lista de um objeto do mesmo tipo | 
| `Object` | BuildFromJSON(this `DataGridView&` DataGridView, `String` JSON) |  | 


## `DataGridViewProgressCell`

```csharp
public class InnerLibs.DataGridViewProgressCell
    : DataGridViewImageCell, ICloneable, IDisposable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Color` | ProgressColor |  | 


## `DataGridViewProgressColumn`

```csharp
public class InnerLibs.DataGridViewProgressColumn
    : DataGridViewImageColumn, ICloneable, IDisposable, IComponent

```

## `DataManipulation`

```csharp
public class InnerLibs.DataManipulation

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `DbCommand` | AddFile(this `DbCommand&` Command, `String` FileParameter, `HttpPostedFile` File) | Adiciona um parametro de Arquivo no commando | 
| `DbCommand` | AddFile(this `DbCommand&` Command, `String` FileParameter, `FileInfo` File) | Adiciona um parametro de Arquivo no commando | 
| `DbCommand` | AddFile(this `DbCommand&` Command, `String` FileParameter, `Byte[]` File) | Adiciona um parametro de Arquivo no commando | 
| `Object` | AppendSQLParameter(this `String&` Command, `String` Key, `String` Value = null) | Concatena um parametro a uma string de comando SQL | 
| `DbType` | GetDbType(this `Object` Obj) | Retorna o DbType de acordo com o tipo do objeto | 
| `Reader` | RunSQL(this `DataContext` Context, `String` SQLQuery, `DbParameter[]` Parameters) | Executa um Comando SQL e retorna uma estrutura estatica com os dados (`InnerLibs.DataBase.Reader`) | 
| `Reader` | SELECT(this `DataContext` Context, `String` TableName, `String` WhereConditions = , `String[]` Columns = null) | Cria um `InnerLibs.DataBase.Reader` a partir de um comando SELECT | 
| `void` | SetParameter(this `DbParameterCollection&` Params, `DbParameter` Parameter) | Adiciona ou troca o valor de um parametro em uma coleção | 
| `Reader` | ToDataBaseReader(this `DbDataReader` Reader) | Converte um `System.Data.Common.DbDataReader` para um `InnerLibs.DataBase.Reader` | 
| `DataSet` | ToDataSet(this `String[]` Input) | Converte um Array para um `System.Data.DataSet` de 1 Coluna | 
| `DataTableReader` | ToDataTableReader(this `String[]` Input) | Converte um Array para um DataTableReader de 1 Coluna | 
| `String` | ToINSERT(this `IDictionary<String, T>` Dic, `String` TableName) | Cria uma string com um comando de INSERT a partir dos dados do dicionario | 
| `String` | ToUPDATE(this `IDictionary<String, T>` Dic, `String` TableName, `String` WhereClausule) | Cria uma string com um comando de UPDATE a partir dos dados do dicionario | 


## `DateRange`

Classe que representa um periodo entre 2 datas
```csharp
public class InnerLibs.DateRange

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | EndDate |  | 
| `DateTime` | StartDate |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | CalculatePercent(`Nullable<DateTime>` Date = null) | Verifica quantos porcento uma data representa  em distancia dentro deste periodo | 
| `Boolean` | Contains(`DateRange` Period) | Verifica se este periodo contém um outro periodo | 
| `Boolean` | Contains(`DateTime` Day) | Verifica se este periodo contém um outro periodo | 
| `FortnightGroup` | CreateFortnightGroup() | Cria um grupo de quinzenas que contenham este periodo | 
| `TimeFlow` | Difference() |  | 
| `Boolean` | IsIn(`DateRange` Period) | Verifica se este periodo está dentro de outro periodo | 
| `Boolean` | IsNow() | Verifica se hoje está dentro deste periodo | 
| `Boolean` | MatchAny(`DateRange` Period) | Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou contém um periodo) | 
| `Boolean` | Overlaps(`DateRange` Period) | Verifica se 2 periodos possuem interseção de datas | 
| `List<DateTime>` | ToList(`DateInterval` Interval = Day) | Cria uma lista de datas contendo todas as datas entre os periodos | 
| `String` | ToString() |  | 


## `Dice`

Dado de RPG
```csharp
public class InnerLibs.Dice

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DiceFace` | Face | Retorna a face correspondente ao numero | 
| `ReadOnlyCollection<DiceFace>` | Faces | Faces do dado | 
| `Boolean` | IsCustom | Indica se o dado é um dado com faces customizadas | 
| `Boolean` | IsVicious | Verifica se o dado possui algum lado viciado | 
| `Boolean` | Locked | Se TRUE, Impede este dado de ser rolado | 
| `Int32` | RolledTimes | Numero de vezes que este dado já foi rolado | 
| `DiceType` | Type | Tipo do dado | 
| `Int32` | Value | Valor atual deste dado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | NormalizeWeight() | Normaliza o peso das faces do dado | 
| `Int32` | Roll() | Rola o dado e retorna seu valor | 


## `DiceRoller`

Combinação de varios dados de RPG que podem ser rolados ao mesmo tempo
```csharp
public class InnerLibs.DiceRoller
    : List<Dice>, IList<Dice>, ICollection<Dice>, IEnumerable<Dice>, IEnumerable, IList, ICollection, IReadOnlyList<Dice>, IReadOnlyCollection<Dice>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Value | Retorna a soma de todos os valores dos dados | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Roll() | Rola todos os dados (não travados) e retorna a soma de seus valores | 


## `DiceType`

Tipos de Dados
```csharp
public enum InnerLibs.DiceType
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Custom | Dado customizado | 
| `2` | Coin | Moeda | 
| `4` | D4 | Dado de 4 Lados (Tetraedro/Pirâmide) | 
| `6` | D6 | Dado de 6 Lados (Pentalátero/Cubo/Dado Tradicional) | 
| `8` | D8 | Dado de 8 Lados (Octaedro) | 
| `10` | D10 | Dado de 10 Lados (Decaedro) | 
| `12` | D12 | Dado de 12 Lados (Dodecaedro) | 
| `20` | D20 | Dado de 20 Lados (Icosaedro) | 
| `100` | D100 | Dado de 100 Lados (Esfera/Bola - Particulamente util para porcentagem) | 


## `DictionaryPropertyGridAdapter`

```csharp
public class InnerLibs.DictionaryPropertyGridAdapter
    : ICustomTypeDescriptor

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `AttributeCollection` | GetAttributes() |  | 
| `String` | GetClassName() |  | 
| `String` | GetComponentName() |  | 
| `TypeConverter` | GetConverter() |  | 
| `EventDescriptor` | GetDefaultEvent() |  | 
| `PropertyDescriptor` | GetDefaultProperty() |  | 
| `Object` | GetEditor(`Type` editorBaseType) |  | 
| `EventDescriptorCollection` | GetEvents(`Attribute[]` attributes) |  | 
| `PropertyDescriptorCollection` | GetProperties(`Attribute[]` attributes) |  | 
| `Object` | GetPropertyOwner(`PropertyDescriptor` pd) |  | 


## `Directories`

Funções para trabalhar com diretorios
```csharp
public class InnerLibs.Directories

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | CleanDirectory(this `DirectoryInfo` TopDirectory, `Boolean` DeleteTopDirectoryIfEmpty = True) | Remove todos os subdiretorios vazios | 
| `DirectoryInfo` | CopyTo(this `DirectoryInfo` Directory, `DirectoryInfo` DestinationDirectory) | Copia um diretório para dentro de outro diretório | 
| `List<FileInfo>` | CopyTo(this `List<FileInfo>` List, `DirectoryInfo` DestinationDirectory) | Copia um diretório para dentro de outro diretório | 
| `Boolean` | DeleteIfExist(this `String` Path) | Deleta um arquivo ou diretório se o mesmo existir e retorna TURE se o arquivo puder ser criado novamente | 
| `Boolean` | DeleteIfExist(this `FileSystemInfo` Path) | Deleta um arquivo ou diretório se o mesmo existir e retorna TURE se o arquivo puder ser criado novamente | 
| `DirectoryInfo` | ExtractZipFile(this `FileInfo` File, `DirectoryInfo` Directory) | Extrai um arquivo zip em um diretório | 
| `IEnumerable<FindType>` | Find(this `DirectoryInfo` Directory, `Func<FindType, Boolean>` predicate, `SearchOption` SearchOption = AllDirectories) | Retorna uma lista de arquivos ou diretórios baseado em uma busca usando predicate | 
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
| `DirectoryInfo` | ToDirectory(this `String` DirectoryName) | Cria um diretório se o mesmo nao existir e retorna um DirectoryInfo deste diretório | 
| `FileInfo` | ToFile(this `String` FileName, `FileType` Type) | Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo | 
| `FileInfo` | ToFile(this `String` FileName) | Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo | 
| `FileInfo` | ToZipFile(this `DirectoryInfo` FilesDirectory, `String` OutputFile, `CompressionLevel` CompressionLevel = Optimal) | Cria um arquivo .ZIP de um diretório | 
| `FileInfo` | ToZipFile(this `DirectoryInfo` FilesDirectory, `String` OutputFile, `CompressionLevel` CompressionLevel, `SearchOption` SearchOption, `String[]` Searches) | Cria um arquivo .ZIP de um diretório | 


## `DocumentationPage`

```csharp
public abstract class InnerLibs.DocumentationPage
    : Page, IComponent, IDisposable, IParserAccessor, IUrlResolutionService, IDataBindingsAccessor, IControlBuilderAccessor, IControlDesignerAccessor, IExpressionsAccessor, INamingContainer, IFilterResolutionService, IHttpHandler

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | LoadingMessage |  | 
| `String` | ProductName |  | 
| `String` | SearchPlaceholder |  | 
| `String` | SourceDirectory |  | 


## `EditorCreatedEventArgs`

```csharp
public class InnerLibs.EditorCreatedEventArgs
    : EventArgs

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Form` | EditorForm |  | 


## `Encryption`

Modulo de Criptografia
```csharp
public class InnerLibs.Encryption

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Decrypt(this `String` Text) | Descriptografa uma string | 
| `String` | Encrypt(this `String` Text) | Criptografa uma string | 
| `String` | ToMD5String(this `String` Text) | Criptografa um Texto em MD5 | 
| `String` | TryReverseMD5(this `String` Text) | Tenta reverter uma string MD5 para seu valor original | 


## `EnhancedCollectionEditor`

```csharp
public abstract class InnerLibs.EnhancedCollectionEditor
    : CollectionEditor

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | EditValue(`ITypeDescriptorContext` context, `IServiceProvider` provider, `Object` value) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | BaseNameFromType(`Type` ItemType) |  | 
| `String` | BaseNameFromTypeName(`String` ItemTypeName) |  | 


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


## `FileContextMenu`

```csharp
public class InnerLibs.FileContextMenu

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Register(`String` FileType, `String` ShellKeyName, `String` MenuText, `String` MenuCommand) | Adciona opções customizadas ao menu de contexto do windows | 
| `Boolean` | Unregister(`String` FileType, `String` ShellKeyName) | Remove uma opção customizada do menu de contexto do windows | 


## `FileLogger`

```csharp
public class InnerLibs.FileLogger
    : List<LogEntry>, IList<LogEntry>, ICollection<LogEntry>, IEnumerable<LogEntry>, IEnumerable, IList, ICollection, IReadOnlyList<LogEntry>, IReadOnlyCollection<LogEntry>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `FileInfo` | File | Local do arquivo de log | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` Title, `String` Message) | Adiciona uma Entrada ao Log | 
| `void` | AddRange(`LogEntry[]` Entries) |  | 


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
| `Byte[]` | ToBytes(this `FileInfo` File) | Transforma um arquivo em um Array de Bytes | 
| `Byte[]` | ToBytes(this `HttpPostedFile` File) | Transforma um arquivo em um Array de Bytes | 
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
| `FileType` | GetFileType(`String` MimeTypeOrExtension) | Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string | 
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
| `FileType` | ToFileType(this `String` MimeTypeOrExtension) | Retorna um Objeto FileType a partir de uma string MIME Type, Nome ou Extensão de Arquivo | 


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
| `void` | ApplyDialogFilter(`OpenFileDialog&` Dialog) | Aplica um filtro no OpenFileDialog | 
| `void` | ApplyDialogFilter(`SaveFileDialog&` Dialog) | Aplica um filtro no OpenFileDialog | 
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


## `FormAnimator`

Animates a form when it is shown, hidden or closed.
```csharp
public class InnerLibs.FormAnimator

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AnimationDirection` | Direction | Gets or sets the direction in which the animation is performed. | 
| `Int32` | Duration | Gets or sets the number of milliseconds over which the animation is played. | 
| `Form` | Form | Gets the form to be animated. | 
| `AnimationMethod` | Method | Gets or sets the animation method used to show and hide the form. | 


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
| `Int32` | RandomNumber(`Int64` Min = 0, `Int64` Max = 999999) | Gera um numero Aleatório entre 2 números | 
| `String` | RandomWord(`Int32` Length = 0) | Gera uma palavra aleatória com o numero de caracteres | 
| `String` | RandomWord(`String` BaseText) | Gera uma palavra aleatória com o numero de caracteres | 
| `Image` | ScreenshotFromWebsite(this `String` Url, `String` AccessKey, `Boolean` FullPage = True, `Int32` Delay = 1, `String` Viewport = 1440x900, `Int32` ImageWidth = 500) | Tira uma screenshot de um site usando o servico ATS | 
| `Uri` | ToGoogleMapsURL(this `Location` local, `Boolean` LatLong = False) | Gera uma URL do google MAPs baseado na localização | 
| `Image` | ToQRCode(this `String` Data, `Int32` Size = 100) | Converte uma String para um QR Code usando uma API (Nescessita de Internet) | 
| `Image` | ToStaticGoogleMap(this `Location` Location, `Int32` Width = 400, `Int32` Height = 400, `Int32` Zoom = 16, `Int32` Scale = 2, `MapType` Maptype = RoadMap) | Cria um Mapa estatico utilizando a API do google Maps | 


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


## `GoogleSuggestion`

```csharp
public class InnerLibs.GoogleSuggestion

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | GetGoogleSuggestions(this `String` Text, `String` Language = pt) | Retorna uma lista de sugestões de pesquisa do google baseado em um texto | 


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
| `Image` | DrawImage(this `String` Text, `Font` Font = null, `Nullable<Color>` TextColor = null, `Nullable<Color>` BackColor = null) | Escreve uma string em uma imagem | 
| `ImageCodecInfo` | GetEncoderInfo(this `ImageFormat` RawFormat) | Pega o encoder a partir de um formato de imagem | 
| `ImageFormat` | GetImageFormat(this `Image` OriginalImage) | Retorna o formato da imagem correspondente a aquela imagem | 
| `List<Color>` | GetMostUsedColors(this `Image` Image, `Int32` Count = 10) | Retorna uma lista com as 10 cores mais utilizadas na imagem | 
| `List<Color>` | GetMostUsedColors(this `Bitmap` Image, `Int32` Count = 10) | Retorna uma lista com as 10 cores mais utilizadas na imagem | 
| `Image` | InsertWatermark(this `Image` Image, `String` Watermark, `String` Font = Arial, `Nullable<Color>` FontColor = null, `Nullable<Color>` BackColor = null, `Int32` X = -1, `Int32` Y = -1) | Insere um texto de marca Dágua na imagem | 
| `Image` | InsertWatermark(this `Image` Image, `Image` WaterMark, `Int32` X = -1, `Int32` Y = -1) | Insere um texto de marca Dágua na imagem | 
| `Image` | Resize(this `Image` Original, `Int32` NewWidth, `Int32` MaxHeight, `Boolean` OnlyResizeIfWider = True) | Redimensiona e converte uma Imagem | 
| `Boolean` | TestAndRotate(this `Image&` Img) | Rotaciona uma imagem para sua pocisão original caso ela já tenha sido rotacionada (EXIF) | 
| `Byte[]` | ToBytes(this `Image` Image, `ImageFormat` Format = null) | Transforma uma imagem em array de bytes | 
| `Size` | ToSize(this `String` Text) | Interperta uma string de diversas formas e a transforma em um `System.Drawing.Size` | 
| `Image` | Trim(this `Image` Img, `Color` Color) | Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo | 


## `INameProvider`

```csharp
public interface InnerLibs.INameProvider

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | GetNewName(`String` typeName) |  | 


## `InnerCrypt`

```csharp
public class InnerLibs.InnerCrypt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | InnCrypt(this `String` Text) | Criptografa uma suma string usando a logica InnerCrypt | 
| `String` | UnnCrypt(this `String` EncryptedText) | Descriptografa uma string previamente criptografada com InnerCrypt | 


## `iVariableBag`

```csharp
public interface InnerLibs.iVariableBag

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `iEvalTypedValue` | GetVariable(`String` varname) |  | 


## `Json`

```csharp
public class InnerLibs.Json
    : JavaScriptSerializer

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | DeserializeJSON(`Object` Obj) | Converte um JSON para objeto | 
| `Type` | DeserializeJSON(`Object` Obj) | Converte um JSON para objeto | 
| `String` | SerializeJSON(`Object` Obj) | Converte um objeto para JSON | 


## `JsonViewer`

```csharp
public class InnerLibs.JsonViewer

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `TreeNode` | CreateNode(this `Object` Item) |  | 
| `void` | LoadObject(this `TreeView` TreeView, `Object` Obj) |  | 


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
| `Double` | CalculateDistance(`Location` FirstLocation, `Location` SecondLocation) | Calcula a distancia entre 2 locais | 
| `Double` | CalculateDistance(`Location[]` Locations) | Calcula a distancia entre 2 locais | 
| `Dictionary<TKey, Decimal>` | CalculatePercent(this `Dictionary<TKey, TValue>` Dic) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Dictionary<TKey, Decimal>` | CalculatePercent(this `IEnumerable<TObject>` Obj, `Func<TObject, TKey>` KeySelector, `Func<TObject, TValue>` ValueSelector) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Dictionary<Tobject, Decimal>` | CalculatePercent(this `IEnumerable<Tobject>` Obj, `Func<Tobject, Tvalue>` ValueSelector) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Dictionary<TValue, Decimal>` | CalculatePercent(this `IEnumerable<TValue>` Obj) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Decimal` | CalculatePercent(this `Decimal` Value, `Decimal` Total) | Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade | 
| `Decimal` | CalculateValueFromPercent(this `String` Percent, `Decimal` Total) | Retorna o valor de um determinado percentual de um valor total | 
| `List<T[]>` | CartesianProduct(`T[][]` Sets) | Retorna todas as possiveis combinações de Arrays do mesmo tipo (Produto Cartesiano) | 
| `Int64` | Ceil(this `Decimal` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Int64` | Ceil(this `Double` Number) | Arredonda um numero para cima. Ex.: 4,5 -&gt; 5 | 
| `Object` | EvaluateExpression(`String` Formula, `Boolean` Exception = False) | Executa uma Expressão matematica/lógica simples | 
| `T` | EvaluateExpression(`String` Formula, `Boolean` Exception = False) | Executa uma Expressão matematica/lógica simples | 
| `Int32` | Factorial(this `Int32` Number) | Calcula o fatorial de um numero | 
| `List<Int32>` | Fibonacci(`Int32` Length) | Retorna uma sequencia Fibonacci de N numeros | 
| `Int64` | Floor(this `Decimal` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `Int64` | Floor(this `Double` Number) | Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4 | 
| `List<Int32>` | GeometricProgression(`Int32` FirstNumber, `Int32` Constant, `Int32` Length) | Retorna uma Progressão Gemoétrica com N numeros | 
| `Int64` | GetDecimalPlaces(this `Decimal` Value, `Int32` DecimalPlaces = 0, `CultureInfo` Culture = null) | Retorna um numero inteiro representando a parte decimal de um numero decimal | 
| `Boolean` | IsBetween(this `Type` Number, `Object` FirstNumber, `Object` SecondNumber) | Verifica se um numero está entre outros 2 números | 
| `Single` | Lerp(this `Single` Start, `Single` End, `Single` Amount) | Realiza um calculo de interpolação Linear | 
| `Int32` | LimitIndex(this `Int32` Int, `IEnumerable<AnyType>` Collection) |  | 
| `Int64` | LimitIndex(this `Int64` Lng, `IEnumerable<AnyType>` Collection) |  | 
| `Type` | LimitRange(this `Type` Number, `Object` MinValue = null, `Object` MaxValue = null) | Limita um range para um numero | 
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
| `String` | CurrencySymbol | Simbolo de moeda | 
| `String` | ISOCurrencySymbol | Simbolo de moeda utilizada em cambio (ISO) | 
| `String` | MoneyString | String do valor formatado como moeda | 
| `Decimal` | Value |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Money` | ConvertCurrency(`String` Symbol) | Converte de uma moeda para a outra utilizando a api http://cryptonator.com | 
| `Money` | ConvertCurrency(`CultureInfo` Culture) | Converte de uma moeda para a outra utilizando a api http://cryptonator.com | 
| `String` | ToString() | String do valor formatado como moeda, é um alias para `InnerLibs.Money.MoneyString` | 
| `Object` | ToString(`Int32` Precision) | String do valor formatado como moeda, é um alias para `InnerLibs.Money.MoneyString` | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<CultureInfo>` | GetCultureInfosByCurrencySymbol(`String` Currency) | Traz uma lista de `System.Globalization.CultureInfo` que utilizam uma determinada moeda de acordo com o simbolo, simbolo ISO ou | 


## `NameServices`

```csharp
public enum InnerLibs.NameServices
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | None |  | 
| `1` | Automatic |  | 
| `2` | NameProvider |  | 


## `NewItemCreatedEventArgs`

```csharp
public class InnerLibs.NewItemCreatedEventArgs
    : EventArgs

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ItemBaseName |  | 


## `NotificationForm`

Formulário de notificações interativas
```csharp
public class InnerLibs.NotificationForm
    : Form, IComponent, IDisposable, IOleControl, IOleObject, IOleInPlaceObject, IOleInPlaceActiveObject, IOleWindow, IViewObject, IViewObject2, IPersist, IPersistStreamInit, IPersistPropertyBag, IPersistStorage, IQuickActivate, ISupportOleDropSource, IDropTarget, ISynchronizeInvoke, IWin32Window, IArrangedElement, IBindableComponent, IContainerControl

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AnimationDirection` | CloseDirection | Direção que a notificação desliza ao ser fechada | 
| `AutoCompleteStringCollection` | InputBoxAutoComplete | Coleção de strings para o AutoCompletar do InputBox | 
| `String` | InputBoxText | Texto da caixa de input | 
| `String` | OKButtonText | Texto do Botão OK | 
| `AnimationDirection` | OpenDirection | Direção que a notificação desliza ao aparecer | 
| `Int32` | RemainTime | Tempo restante antes que a notificação seja fchada automaticamente | 
| `RemainTimeBehavior` | RemainTimeBehavior | Comportamento do tempo restante da notificação caso ela seja re-utilizada | 
| `Boolean` | ShowInputBox | Exibe uma caixa de texto na notificação para entrada de informações | 
| `Boolean` | ShowRemainTime | Valor que representa se o contador de segundos deve ser exibido na notificação | 
| `Size` | Size | Tamanho da notificação | 
| `ContentAlignment` | TextAlign | Alinhamento do texto da notificação | 


Events

| Type | Name | Summary | 
| --- | --- | --- | 
| `EventHandler` | OnInputBoxTextChanged |  | 
| `EventHandler` | OnOKButtonClick |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Show(`Int32` Seconds = 0) | Exibe ou altera a notificação | 


Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<NotificationForm>` | VisibleNotifications | Lista com as notificações abertas | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | DestroyNotifications() | Destroi todas as notificações criadas na aplicação. | 


## `oEmbed`

Classe para integrar a api oEmbed em aplicações .NET
```csharp
public class InnerLibs.oEmbed

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Endpoint[]` | Endpoints |  | 
| `String` | Provider_name |  | 
| `String` | Provider_url |  | 
| `Object` | Response | Retorna o um `System.Object` contendo as informaçoes da URL | 
| `String` | Url |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlElement` | CreateElement(`String` Name = span) | Cria um `InnerLibs.HtmlParser.HtmlElement` com a resposta do oEmbed | 
| `Type` | GetResponse(`Int32` EndPointIndex = 0) | Retorna o um objeto convertido para um tipo contendo as informaçoes da URL | 
| `String` | ToString() | Retorna a string do elemento do oEmbed | 
| `String` | ToString(`String` Name) | Retorna a string do elemento do oEmbed | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `oEmbed[]` | GetProviders() | Retorna uma lista de provedores compativeis com oEmbed | 


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
| `Object` | Add(`UserType` Obj) |  | 
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
| `Page` | LastPage |  | 
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


## `PictureService`

Retorna imagens de diversos serviços para serem usadas como marcação ou sugestão.
```csharp
public class InnerLibs.PictureService

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Size` | Size | Tamanho da imagem que será gerada pelo serviço | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Picture` | AnyService(`Boolean` OnlyPhotos = True) | Retorna uma imagem de qualquer serviço aleatóriamente | 
| `Picture` | LoremPixel(`String` Category = , `Int32` Index = -1, `Boolean` Grayscale = False, `String` Text = ) | Retorna uma imagem usando LoremPixel.com | 
| `Picture` | Pipsum() | Retorna uma imagem usando Pipsum.com | 
| `Picture` | PlaceHold(`String` Text = , `Nullable<Color>` Color = null, `Nullable<Color>` TextColor = null) | Retorna uma imagem usando Placehold.It | 
| `Picture` | Unsplash(`Int32` Index = -1, `Boolean` Grayscale = False, `Boolean` Blur = False) | Retorna uma Imagem utilizando Unsplash.it | 


## `RemainTimeBehavior`

Comportamento do contador da notificação
```csharp
public enum InnerLibs.RemainTimeBehavior
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | StackTime | Adiciona segundos ao total de segundos restantes se a notificação já estiver sendo exibida | 
| `1` | ResetTime | Atribui o valor especificado aos do segundos restantes se a notificação já estiver sendo exibida | 
| `2` | None | Não altera o tempo restante da notificação | 


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

```csharp
public class InnerLibs.SentencePart

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Sentence` | Sentence |  | 
| `String` | Text |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsCloseWrapChar() |  | 
| `Boolean` | IsComma() |  | 
| `Boolean` | IsEndOfSentencePunctuation() |  | 
| `Boolean` | IsMidSentencePunctuation() |  | 
| `Boolean` | IsNotWord() |  | 
| `Boolean` | IsOpenWrapChar() |  | 
| `Boolean` | IsPunctuation() |  | 
| `Boolean` | IsWord() |  | 
| `Boolean` | NeedSpaceOnNext() |  | 
| `SentencePart` | Next() |  | 
| `SentencePart` | Previous() |  | 
| `String` | ToString() |  | 


## `Shortcuts`

```csharp
public class InnerLibs.Shortcuts

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | CreateShortcut(this `DirectoryInfo` Directory, `String` Name, `String` Arguments = null, `String` Target = null, `String` Description = null, `String` Icon = null) | Cria atalhos em um diretõrio especifico | 


## `SoundEx`

```csharp
public class InnerLibs.SoundEx

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | SoundEx(this `String` Text) | Gera um código SOUNDEX para comparação de fonemas | 
| `Boolean` | SoundsLike(this `String` FirstText, `String` SecondText) | Compara 2 palavras e verifica se elas possuem fonema parecido | 


## `Startup`

```csharp
public class InnerLibs.Startup

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | AddToWindowsStartup(this `Application` MyApplication, `String` AppName) |  | 
| `void` | RemoveFromWindowsStartup(this `Application` MyApplication, `String` AppName) |  | 


## `StructuredText`

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
| `String` | AdjustWhiteSpaces(this `String` Text) |  | 
| `String` | Alphabetize(this `String` Text) | Retorna uma string em ordem afabética baseada em uma outra string | 
| `String` | Append(this `String&` Text, `String` AppendText) | Adiciona texto ao final de uma string | 
| `String` | AppendIf(this `String&` Text, `String` AppendText, `Boolean` Test) | Adiciona texto ao final de uma string se um criterio for cumprido | 
| `String` | ApplySpaceOnWrapChars(this `String` Text) | Aplica espacos em todos os caracteres de encapsulamento | 
| `String` | CamelSplit(this `String` Text) | Transforma uma palavra em CameCase em varias palavras a partir de suas letras maíusculas | 
| `Boolean` | Censor(this `String&` Text, `List<String>` BadWords, `Char` CensorshipCharacter = *) | Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro  caractere desejado) e retorna um valor indicando se o texto precisou ser censurado | 
| `String` | Censor(this `String` Text, `Char` CensorshipCharacter, `String[]` BadWords) | Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro  caractere desejado) e retorna um valor indicando se o texto precisou ser censurado | 
| `Boolean` | ContainsAll(this `String` Text, `String[]` Values) | Verifica se uma String contém todos os valores especificados | 
| `Boolean` | ContainsAll(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma String contém todos os valores especificados | 
| `Boolean` | ContainsAny(this `String` Text, `String[]` Values) | Verifica se uma String contém qualquer um dos valores especificados | 
| `Boolean` | ContainsAny(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) | Verifica se uma String contém qualquer um dos valores especificados | 
| `Int32` | CountCharacter(this `String` Text, `Char` Character) | Conta os caracters especificos de uma string | 
| `Dictionary<String, Int64>` | CountWords(this `String` Text, `Boolean` RemoveDiacritics = True, `String[]` Words = null) | Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade | 
| `HtmlGenericControl` | CreateElement(this `String` HTMLString) | Cria um elemento HTML a partir de uma string HTML | 
| `Int32` | Decrement(this `Int32&` Number, `Int32` Amount = 1) | Decrementa em 1 ou mais um numero inteiro | 
| `Int32` | Decrement(this `Int64&` Number, `Int32` Amount = 1) | Decrementa em 1 ou mais um numero inteiro | 
| `String` | DeleteLine(this `String&` Text, `Int32` LineIndex) |  | 
| `Dictionary<String, Int64>` | DistinctCount(`String[]` List) | Cria um dicionário com as palavras de uma lista e a quantidade de cada uma. | 
| `Dictionary<String, Int64>` | DistinctCount(this `String` Phrase) | Cria um dicionário com as palavras de uma lista e a quantidade de cada uma. | 
| `Boolean` | EndsWithAny(this `String` Text, `String[]` Words) | Verifica se uma string termina com alguma outra string de um array | 
| `String` | EscapeQuotesToQuery(this `String` Text) | Prepara uma string com aspas simples para uma Query TransactSQL | 
| `Int64[]` | FindNumbers(this `String` Text) | Procura numeros em uma string e retorna um array deles | 
| `List<String>` | FindTelephoneNumbers(this `String` Text) | Procurea numeros de telefone em um texto | 
| `String` | FixBreakLines(this `String` Text) | Transforma quebras de linha HTML em quebras de linha comuns ao .net | 
| `String` | FixCaptalization(this `String` Text) |  | 
| `String` | FixPunctuation(this `String&` Text, `String` Punctuation = ., `Boolean` ForceSpecificPunctuation = False) | Remove os espaços excessivos (duplos) no meio da frase e remove os espaços no inicio e final  (é um alias para `InnerLibs.Text.AdjustWhiteSpaces(System.String)` da frase | 
| `String` | FixText(this `String` Text) | Arruma a ortografia do texto captalizando corretamente, adcionando pontução ao final de frase  caso nescessário e removendo espaços excessivos ou incorretos | 
| `String` | Format(this `String` Text, `Object[]` Args) | Extension Method para `System.String` | 
| `String` | GetAfter(this `String` Text, `String` Value) | Retorna um texto posterior a outro | 
| `String[]` | GetAllBetween(this `String` Text, `String` Before, `String` After = ) | Retorna todas as ocorrencias de um texto entre dois textos | 
| `String` | GetBefore(this `String` Text, `String` Value) | Retorna um texto anterior a outro | 
| `String` | GetBetween(this `String` Text, `String` Before, `String` After) | Retorna o texto entre dois textos | 
| `String` | GetDomain(this `Uri` URL, `Boolean` RemoveFirstSubdomain = False) | Pega o dominio principal de uma URL | 
| `String` | GetDomain(this `String` URL, `Boolean` RemoveFirstSubdomain = False) | Pega o dominio principal de uma URL | 
| `String` | GetFirstChars(this `String` Text, `Int32` Number = 1) |  | 
| `Dictionary<String, Int64>` | GetKeyWords(this `String` TextOrURL, `Int32` MinWordCount = 1, `Int32` MinWordLenght = 1, `Int32` LimitCollection = 0, `Boolean` RemoveDiacritics = True, `String[]` IgnoredWords = null, `String[]` ImportantWords = null) | Extrai palavras chave de um texto seguindo critérios especificos. | 
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
| `Boolean` | IsPalindrome(`String` Text, `Boolean` IgnoreWhiteSpaces = False) | Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da esqueda  para direita | 
| `String` | Join(this `IEnumerable<Type>` Array, `String` Separator = ;) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(this `Type[]` Array, `String` Separator = ;) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(`String` Separator, `Type[]` Array) | Une todos os valores de um objeto em uma unica string | 
| `String` | Join(this `List<Type>` List, `String` Separator = ) | Une todos os valores de um objeto em uma unica string | 
| `Int32` | LevenshteinDistance(this `String` Text1, `String` Text2) | Computa a distancia de Levenshtein entre 2 strings. | 
| `String` | MakeClean(this `String` Text) | limpa um texto deixando apenas os caracteres alfanumericos. | 
| `String` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) | Remove caracteres não numéricos de uma string | 
| `Type` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) | Remove caracteres não numéricos de uma string | 
| `TypeClass` | ParseJSON(this `String` JSON, `String` DateFormat = yyyy-MM-dd HH:mm:ss) | Transforma uma JSON String em um Objeto ou Classe | 
| `Object` | ParseJSON(this `String` JSON) | Transforma uma JSON String em um Objeto ou Classe | 
| `String` | Poopfy(`String[]` Words) | Retorna uma string em sua forma poop | 
| `String` | Poopfy(this `String` Text) | Retorna uma string em sua forma poop | 
| `String` | PreetyPrint(this `XmlDocument` Document) | Return a Idented XML string | 
| `String` | Prepend(this `String&` Text, `String` PrependText) | Adiciona texto ao começo de uma string | 
| `String` | PrependIf(this `String&` Text, `String` PrependText, `Boolean` Test) | Adiciona texto ao começo de uma string se um criterio for cumprido | 
| `String` | QuantifyText(this `String` PluralText, `CultureInfo` Culture = null, `String` Identifier = q) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado em ``. | 
| `String` | QuantifyText(this `String` PluralText, `Object` Quantity, `CultureInfo` Culture = null) | Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado em ``. | 
| `String` | QuantityFormula(this `Decimal` Number, `Decimal` BaseNumber, `Decimal` Round, `String[]` BaseUnity) | Reduz um numero utilizando uma unidade de medida e uma base | 
| `String` | QuantityFormula(this `Decimal` Number, `Decimal` BaseNumber, `String[]` BaseUnity) | Reduz um numero utilizando uma unidade de medida e uma base | 
| `String` | QuantityFormula(this `Int32` Number, `Decimal` BaseNumber, `String[]` BaseUnity) | Reduz um numero utilizando uma unidade de medida e uma base | 
| `String` | QuantityFormula(this `Int64` Number, `Decimal` BaseNumber, `String[]` BaseUnity) | Reduz um numero utilizando uma unidade de medida e uma base | 
| `String` | QuantityFormula(this `Double` Number, `Decimal` BaseNumber, `String[]` BaseUnity) | Reduz um numero utilizando uma unidade de medida e uma base | 
| `String` | QuantityFormula(this `Double` Number, `Decimal` BaseNumber, `Int32` Round, `String[]` BaseUnity) | Reduz um numero utilizando uma unidade de medida e uma base | 
| `String` | Quote(this `String` Text, `Char` QuoteChar = ") | Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) | 
| `String` | QuoteIf(this `String` Text, `Boolean` Condition, `String` QuoteChar = ") | Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes) se uma  condiçao for cumprida | 
| `Type` | RandomItem(`Type[]` Array) | Sorteia um item da Matriz | 
| `String` | RegexEscape(this `String` Text) | Escapa caracteres exclusivos de uma regex | 
| `String` | RemoveAccents(this `String` Text) | Remove os acentos de uma string | 
| `String` | RemoveAny(this `String` Text, `String[]` Values) | Remove várias strings de uma string | 
| `String` | RemoveDiacritics(this `String&` Text) | Remove os acentos de uma string | 
| `String` | RemoveFirstAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StartStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveFirstChars(this `String` Text, `Int32` Quantity = 1) | Remove uma determinada linha de um texto | 
| `String` | RemoveFirstIf(this `String` Text, `String` StartStringTest) | Remove um texto do inicio de uma string se ele for um outro texto especificado | 
| `String` | RemoveHTML(this `String` Text) |  | 
| `String` | RemoveLastAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` EndStringTest) | Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes | 
| `String` | RemoveLastChars(this `String` Text, `Int32` Quantity = 1) | Remove os X ultimos caracteres | 
| `String` | RemoveLastIf(this `String` Text, `String` EndStringTest) | Remove um texto do final de uma string se ele for um outro texto | 
| `String` | RemoveNonPrintable(this `String` Text) | Remove caracteres não printaveis de uma string | 
| `String[]` | Replace(this `String[]` Strings, `String` OldValue, `String` NewValue, `Boolean` ReplaceIfEquals = True) | Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum | 
| `List<String>` | Replace(this `List<String>` Strings, `String` OldValue, `String` NewValue, `Boolean` ReplaceIfEquals = True) | Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, String>` Dic) | aplica um replace a um texto baseando-se em um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, String[]>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) | aplica um replace a um texto baseando-se em um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String[], String>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) | aplica um replace a um texto baseando-se em um `System.Collections.IDictionary` | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String[], String[]>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) | aplica um replace a um texto baseando-se em um `System.Collections.IDictionary` | 
| `String` | ReplaceMany(this `String` Text, `String` NewValue, `String[]` OldValues) | Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são  substituídas por um novo valor. | 
| `String` | ReplaceNone(this `String` Text, `String` OldValue) | Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são  substituídas por vazio. | 
| `String` | SensitiveReplace(this `String` Text, `String` NewValue, `String` OldValue, `StringComparison` ComparisonType = InvariantCulture) | Realiza um replace em uma string usando um tipo especifico de comparacao | 
| `String` | SensitiveReplace(this `String` Text, `String` NewValue, `IEnumerable<String>` OldValues, `StringComparison` ComparisonType = InvariantCulture) | Realiza um replace em uma string usando um tipo especifico de comparacao | 
| `String` | SerializeJSON(this `Object` Obj, `Object[]` params) | Transforma um Objeto em JSON utilizando o método ToJson() do objeto. Caso o método não existir, utiliza-se `InnerLibs.Json.SerializeJSON(System.Object)` | 
| `Type[]` | Shuffle(this `Type[]&` Array) | Randomiza a ordem dos itens de um Array | 
| `List<Type>` | Shuffle(this `List`1&` List) | Randomiza a ordem dos itens de um Array | 
| `String` | Shuffle(this `String&` Text) | Randomiza a ordem dos itens de um Array | 
| `String` | Singularize(this `String` Text) | Retorna a frase especificada em sua forma singular | 
| `String` | Slice(this `String` Text, `Int32` TextLength = 0, `String` Ellipsis = ...) |  | 
| `String[]` | Split(this `String` Text, `String` Separator, `StringSplitOptions` Options = RemoveEmptyEntries) | Separa um texto em um array de strings a partir de uma outra string | 
| `Boolean` | StartsWithAny(this `String` Text, `String[]` Words) | Verifica se uma string começa com alguma outra string de um array | 
| `Int32` | SyllableCount(this `String` Word) | Conta as silabas de uma palavra | 
| `String` | ToAlternateCase(this `String` Text) | Alterna maiusculas e minusculas para cada letra de uma string | 
| `String` | ToAnagram(this `String` Text) | Retorna um anagrama de um texto | 
| `List<String>` | ToAnagramList(this `String` Text) | Retorna uma lista com todos os anagramas de uma palavra (Metodo Lento) | 
| `String` | ToCamel(this `String` Text) | Transforma uma frase em uma palavra CamelCase | 
| `String` | ToExtensiveForm(this `Decimal` Number, `Int32` DecimalPlaces = 3) | Transforma um numero em sua forma extensa (com até 3 casas apos a virgula) | 
| `String` | ToExtensiveForm(this `Int32` Number) | Transforma um numero em sua forma extensa (com até 3 casas apos a virgula) | 
| `String` | ToExtensiveForm(this `Money` MoneyValue) | Transforma um numero em sua forma extensa (com até 3 casas apos a virgula) | 
| `String` | ToExtensiveMoneyForm(this `Decimal` Value) | Transforma um valor monetário R$ em sua forma extensa | 
| `String` | ToFileSizeString(this `Byte[]` Size) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `FileInfo` Size) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Double` Size) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Int32` Size) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Int64` Size) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFileSizeString(this `Decimal` Size) | Retorna o uma string representando um valor em bytes, KB, MB ou TB | 
| `String` | ToFriendlyURL(this `String` Text, `Boolean` UseUnderscore = False) | Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca  espacos por hifen) | 
| `HtmlElement` | ToHtmlList(this `IEnumerable<T>` List, `Boolean` OrdenedList = False) | Transforma uma lista em uma lista HTML (OL ou UL) | 
| `String` | ToHtmlString(this `HtmlGenericControl` Control) | Transforma um HtmlGenericControl em uma stringHTML | 
| `String` | ToLeet(this `String` Text, `Int32` Degree = 30) | Converte um texo para Leet (1337) | 
| `Object` | ToPercentString(this `Decimal` Number) | Retorna um numero com o sinal de porcentagem | 
| `Object` | ToPercentString(this `Int32` Number) | Retorna um numero com o sinal de porcentagem | 
| `Object` | ToPercentString(this `Double` Number) | Retorna um numero com o sinal de porcentagem | 
| `Object` | ToPercentString(this `Int16` Number) | Retorna um numero com o sinal de porcentagem | 
| `Object` | ToPercentString(this `Int64` Number) | Retorna um numero com o sinal de porcentagem | 
| `String` | ToProper(this `String` Text) |  | 
| `String` | ToRandomCase(this `String` Text) | Coloca a string em Randomcase (aleatoriamente letras maiusculas ou minusculas) | 
| `String` | ToSlug(this `String` Text, `Boolean` UseUnderscore = False) | Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca  espacos por hifen). É um alias para `InnerLibs.Text.ToFriendlyURL(System.String,System.Boolean)` | 
| `String` | ToSnake(this `String` Text) | Retorna uma string em Snake Case | 
| `Stream` | ToStream(this `String` Text) | Cria um `System.IO.Stream` a partir de uma string | 
| `String` | ToTelephone(this `String` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ToTelephone(this `Int64` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ToTelephone(this `Int32` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ToTelephone(this `Decimal` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ToTelephone(this `Double` Number) | Aplica uma mascara a um numero de telefone | 
| `String` | ToTitle(this `String` Text) | Transforma um texto em titulo | 
| `String` | ToXMLString(this `XmlDocument` XML) | Transforma um XML Document em string | 
| `String` | TrimAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StringTest) | Remove do começo e do final de uma string qualquer valor que estiver no conjunto | 
| `String` | TrimAny(this `String` Text, `String[]` StringTest) | Remove do começo e do final de uma string qualquer valor que estiver no conjunto | 
| `String` | TrimCarriage(this `String` Text) | Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines | 
| `String` | UrlDecode(this `String` Text) | Decoda uma string de uma transmissão por URL | 
| `String` | UrlEncode(this `String` Text) | Encoda uma string para transmissão por URL | 
| `String` | Wrap(this `String` Text, `String` WrapText = ") | Encapsula um tento entre 2 textos | 
| `HtmlElement` | WrapInTag(this `String` Text, `String` TagName) | Encapsula um texto dentro de um elemento HTML | 


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
| `void` | Toggle(this `Form&` Form) | Inverte os valores TRUE/FALSE | 
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
| `StringBuilder` | value |  | 


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
| `Dictionary<Decimal, String>` | Units | Unidades de Medida | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Abreviate(`Decimal` Number, `Int32` DecimalPlaces = -1) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Int32` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Int16` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `String` | Abreviate(`Int64` Number) | Abrevia um numero com a unidade mais alta encontrada dentro do conversor | 
| `Decimal` | Convert(`Decimal` Number, `String` To, `String` From) | Converte um numero   decimal em outro numero decimal a partir de unidades de medida | 
| `Decimal` | Convert(`String` AbreviatedNumber, `String` To) | Converte um numero   decimal em outro numero decimal a partir de unidades de medida | 
| `String` | ConvertAbreviate(`String` AbreviatedNumber, `String` To) | Converte um numero abreviado em outro numero abreviado de outra unidade | 
| `KeyValuePair<Decimal, String>` | GetUnit(`String` U) | Retorna a unidade e a base a partir do nome da unidade | 
| `Decimal` | Parse(`String` Number, `Int32` DecimalPlaces = -1) | Retorna o numero decimal a partir de uma string abreviada | 
| `String` | ParseUnit(`String` Number) | Extrai a Unidade utilizada a partir de um numero abreviado | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UnitConverter` | CreateBase1000Converter() | Cria um `InnerLibs.UnitConverter` de Base 1000 | 
| `UnitConverter` | CreateFileSizeConverter() | Cria um `InnerLibs.UnitConverter` de Base 1024 (Bytes) | 


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
| `Byte[]` | Backup() |  | 
| `FileInfo` | Backup(`String` Path) |  | 
| `void` | DeleteConversation(`UserType` User, `UserType` WithUser = null) |  | 
| `IEnumerable<UserConversation<UserType, IdType>>` | GetConversation(`UserType` User, `UserType` WithUser = null) |  | 
| `void` | Restore(`Byte[]` Backup) |  | 
| `void` | Restore(`FileInfo` File) |  | 
| `UserConversation<UserType, IdType>` | Send(`UserType` FromUser, `UserType` ToUser, `String` Message) |  | 
| `void` | SetPeriodicBackup(`String` Path, `Double` Interval) |  | 
| `void` | StopPeriodicBackup() |  | 


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
| `T` | IfBlank(this `Object` Value, `T` ValueIfBlank) | Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE | 
| `T` | IfBlankOrNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfBlankOrNoIndex) | Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um valor default se o index nao existir ou seu valor for branco ou nothing | 
| `T` | IfNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfNoIndex = null) | Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um valor default se o index nao existir | 
| `Boolean` | IsAndroid(this `HttpRequest` HttpRequest) | Verifica se o User Agent da requisição é um Android | 
| `Boolean` | IsBlank(this `String` Text) | Verifica se uma String está em branco | 
| `Boolean` | IsDesktop(this `HttpRequest` HttpRequest) | Verifica se o User Agent da requisição é um PC/NOTEBOOK/MAC | 
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
| `Boolean` | IsIpad(this `HttpRequest` HttpRequest) | Verifica se o User Agent da requisição é um Ipad | 
| `Boolean` | IsJson(this `String` Text, `String` DateFormat = yyyy-MM-dd HH:mm:ss) | Verifica se o texto é um JSON valido | 
| `Boolean` | IsMobile(this `HttpRequest` HttpRequest) | Verifica se o User Agent da requisição é um dispositivel móvel (Celulares e Tablets) | 
| `Boolean` | IsNotBlank(this `String` Text) | Verifica se uma String não está em branco | 
| `Boolean` | IsNotNumber(this `Object` Value) | Verifica se o valor não é um numero | 
| `String` | IsNull(this `String` Text, `String` DefaultValue = null, `Boolean` Quotes = True) | Verifica se um valor é NULO e prepara a string para uma query TransactSQL | 
| `Boolean` | IsNumber(this `Object` Value) | Verifica se o valor é um numero | 
| `Boolean` | IsOdd(this `Decimal` Value) | Verifica se um numero é impar | 
| `Boolean` | IsOdd(this `Int32` Value) | Verifica se um numero é impar | 
| `Boolean` | IsOdd(this `Int64` Value) | Verifica se um numero é impar | 
| `Boolean` | IsPath(this `String` Text) | Verifica se uma string é um caminho de diretóio válido | 
| `Boolean` | IsRunningAsAdministrator() | Verifica se a aplicação está rodando como administrador | 
| `Boolean` | IsTelephone(this `String` Text) | Valida se a string é um telefone | 
| `Boolean` | IsURL(this `String` Text) | Verifica se um determinado texto é uma URL válida | 
| `Boolean` | IsValidCPF(`String` CPF) | Verifica se a string é um CPF válido | 
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
| `IEnumerable<HttpPostedFile>` | AsEnumerable(this `HttpFileCollection` Files) | Retorna todos os arquivos de uma `System.Web.HttpFileCollection` em um  `System.Collections.Generic.IEnumerable`1` | 
| `ListItem` | AsListItem(this `T` Obj, `Func<T, TextType>` Text, `Func<T, ValueType>` Value = null) | Retorna um `System.Web.UI.WebControls.ListItem` a partir de 2 propriedades de um objeto | 
| `void` | CreateFromAjax(this `Type&` TheObject, `String` URL, `String` Method, `NameValueCollection` Parameters = null, `String` ContentType = application/x-www-form-urlencoded, `Encoding` Encoding = null) | Cria um objeto a partir de uma requisiçao AJAX | 
| `String` | DestroySessionAndCookies(this `HttpApplication` Page) | Destroi a Sessão, cache e cookies de uma aplicação ASP.NET | 
| `HtmlSelect` | DisselectValues(this `HtmlSelect` Control, `Func<ListItem, Boolean>` Predicate) | Seleciona Valores de um `System.Web.UI.HtmlControls.HtmlSelect` | 
| `HtmlSelect` | DisselectValues(this `HtmlSelect` Control, `String[]` Values) | Seleciona Valores de um `System.Web.UI.HtmlControls.HtmlSelect` | 
| `String` | ExtractOptions(this `HtmlSelect` Control) | Retorna uma string HTML com os options de um `System.Web.UI.HtmlControls.HtmlSelect` | 
| `IEnumerable<HttpPostedFile>` | GetAllFiles(this `HttpRequest` Request) | Retorna todos os arquivos de uma `System.Web.HttpRequest` em um  `System.Collections.Generic.IEnumerable`1` | 
| `String` | GetFacebookUsername(this `String` URL) | Captura o Username ou UserID de uma URL do Facebook | 
| `String` | GetFacebookUsername(this `Uri` URL) | Captura o Username ou UserID de uma URL do Facebook | 
| `String` | GetVideoId(`String` URL) | Captura o ID de um video do YOUTUBE ou VIMEO em uma URL | 
| `String` | GetVideoId(this `Uri` URL) | Captura o ID de um video do YOUTUBE ou VIMEO em uma URL | 
| `Image` | GetYoutubeThumbnail(`String` URL) | Captura a Thumbnail de um video do youtube | 
| `Image` | GetYoutubeThumbnail(`Uri` URL) | Captura a Thumbnail de um video do youtube | 
| `Boolean` | IsConnected(`String` Test = http://google.com) | Verifica se o computador está conectado com a internet | 
| `Boolean` | IsDown(this `String` Url) | Verifica se um site está indisponível usando o serviço IsUp.Me | 
| `Boolean` | IsDown(`Uri` Url) | Verifica se um site está indisponível usando o serviço IsUp.Me | 
| `Boolean` | IsUp(this `String` Url) | Verifica se um site está disponível usando o serviço IsUp.Me | 
| `Boolean` | IsUp(`Uri` Url) | Verifica se um site está disponível usando o serviço IsUp.Me | 
| `String` | MinifyCSS(`String` CSS) | Minifica uma folha de estilo CSS | 
| `void` | RangeDownload(this `HttpContext&` Context, `Byte[]&` Bytes, `FileType` FileType) | Realiza um download parcial de um `System.Byte` | 
| `void` | RangeDownload(this `HttpContext&` Context, `Byte[]&` Bytes, `String` ContentType) | Realiza um download parcial de um `System.Byte` | 
| `Uri` | RemoveParameter(this `Uri` Url, `String[]` Keys) |  | 
| `Boolean` | RewriteUrl(this `HttpApplication` App, `String` URLPattern, `String` OriginalURL) | Reescreve a URL original a partir de uma REGEX aplicada em uma URL amigavel | 
| `HtmlSelect` | SelectValues(this `HtmlSelect` Control, `String[]` Values) | Seleciona Valores de um `System.Web.UI.HtmlControls.HtmlSelect` | 
| `HtmlSelect` | SelectValues(this `HtmlSelect` Control, `Func<ListItem, Boolean>` Predicate) | Seleciona Valores de um `System.Web.UI.HtmlControls.HtmlSelect` | 
| `ListItem` | SetItem(this `HtmlSelect` Control, `String` Text, `String` Value = ) | Adiciona um novo `System.Web.UI.WebControls.ListItem` ao `System.Web.UI.HtmlControls.HtmlSelect` se um item identico nao existir no mesmo | 
| `ListItem` | SetItem(this `HtmlSelect` Control, `ListItem` li) | Adiciona um novo `System.Web.UI.WebControls.ListItem` ao `System.Web.UI.HtmlControls.HtmlSelect` se um item identico nao existir no mesmo | 
| `HtmlSelect` | SetItems(this `HtmlSelect` Control, `IEnumerable<T>` List, `Func<T, TextType>` Text, `Func<T, ValueType>` Value = null, `Func<T, Boolean>` Selected = null) | Adiciona varios `System.Web.UI.WebControls.ListItem` ao `System.Web.UI.HtmlControls.HtmlSelect` se estes nao existirem no mesmo | 
| `HtmlSelect` | SetItems(this `HtmlSelect` Control, `IEnumerable<T>` List, `Func<T, TextType>` Text, `Func<T, ValueType>` Value, `ValueType[]` SelectedValues) | Adiciona varios `System.Web.UI.WebControls.ListItem` ao `System.Web.UI.HtmlControls.HtmlSelect` se estes nao existirem no mesmo | 
| `HtmlSelect` | SetItems(this `HtmlSelect` Control, `List<ListItem>` List) | Adiciona varios `System.Web.UI.WebControls.ListItem` ao `System.Web.UI.HtmlControls.HtmlSelect` se estes nao existirem no mesmo | 
| `HttpCookie` | ToCookie(this `HttpSessionState` Session, `String` CookieName, `String[]` SessionKeys) | Cria um cookie guardando valores especificos da sessão atual (1 dia de duração) | 
| `HttpCookie` | ToCookie(this `HttpSessionState` Session, `String` CookieName, `DateTime` Expires, `String[]` SessionKeys) | Cria um cookie guardando valores especificos da sessão atual (1 dia de duração) | 
| `HttpCookie` | ToCookie(this `HttpSessionState` Session, `String` CookieName = , `DateTime` Expires = 01/01/0001 12:00:00 AM) | Cria um cookie guardando valores especificos da sessão atual (1 dia de duração) | 
| `String` | ToHtmlString(this `HtmlSelect` Control) | Retorna uma string HTML de um `System.Web.UI.HtmlControls.HtmlSelect` | 
| `Object` | ToINSERT(this `HttpRequest` Request, `String` TableName, `String[]` QueryStringKeys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `Object` | ToINSERT(this `NameValueCollection` Request, `String` TableName, `String[]` QueryStringKeys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `String` | ToINSERTorUPDATE(this `HttpRequest` Request, `String` TableName, `String` QueryStringPrimaryKey, `String[]` QueryStringKeys) | Monta um Comando SQL para executar um INSERT ou UPDATE e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `String` | ToINSERTorUPDATE(this `NameValueCollection` Request, `String` TableName, `String` QueryStringPrimaryKey, `String[]` QueryStringKeys) | Monta um Comando SQL para executar um INSERT ou UPDATE e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `List<ListItem>` | ToListItems(this `IEnumerable<T>` List, `Func<T, TextType>` Text, `Func<T, ValueType>` Value = null, `Func<T, Boolean>` Selected = null) | Retorna uma lista de `System.Web.UI.WebControls.ListItem` a partir de uma coleçao de objetos | 
| `List<ListItem>` | ToListItems(this `IEnumerable<T>` List, `Func<T, TextType>` Text, `Func<T, ValueType>` Value, `ValueType[]` SelectedValues) | Retorna uma lista de `System.Web.UI.WebControls.ListItem` a partir de uma coleçao de objetos | 
| `String` | ToProcedure(this `HttpRequest` Request, `String` ProcedureName, `String[]` QueryStringKeys) | Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de  uma URL como parametros da procedure | 
| `String` | ToProcedure(this `HttpRequest` Request, `String` ProcedureName) | Monta um Comando SQL para executar uma procedure especifica e trata parametros espicificos de  uma URL como parametros da procedure | 
| `Object` | ToUPDATE(this `NameValueCollection` Request, `String` TableName, `String` WhereClausule, `String[]` QueryStringKeys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `Object` | ToUPDATE(this `HttpRequest` Request, `String` TableName, `String` WhereClausule, `String[]` QueryStringKeys) | Monta um Comando SQL para executar um INSERT e trata parametros espicificos de  uma URL como as colunas da tabela de destino | 
| `void` | WriteCSV(this `HttpResponse` Response, `String` CSVString, `String` FileName = CSV) | Escreve um arquivo CSV e finaliza um HttpResponse | 
| `void` | WriteEnd(this `HttpResponse` Response, `String` Text) | Escreve um texto e finaliza um HttpResponse | 
| `void` | WriteEnd(this `HttpResponse` Response, `HtmlDocument` Text) | Escreve um texto e finaliza um HttpResponse | 
| `void` | WriteImage(this `HttpResponse` Response, `Byte[]` Image, `String` MimeType) | Escreve uma imagem e finaliza um HttpResponse | 
| `void` | WriteImage(this `HttpResponse` Response, `Byte[]` Image, `FileType` MimeType) | Escreve uma imagem e finaliza um HttpResponse | 
| `void` | WriteImage(this `HttpResponse` Response, `Image` Image, `ImageFormat` ImageFormat = null) | Escreve uma imagem e finaliza um HttpResponse | 
| `void` | WriteJSON(this `HttpResponse` Response, `String` JSON) | Escreve um JSON e finaliza um HttpResponse | 
| `void` | WriteJSON(this `HttpResponse` Response, `Response` JSON) | Escreve um JSON e finaliza um HttpResponse | 
| `void` | WriteJSON(this `HttpResponse` Response, `Type` JSON) | Escreve um JSON e finaliza um HttpResponse | 
| `void` | WriteJSON(this `HttpResponse` HttpResponse, `String` Status, `String` Message, `Object` Response = null) | Escreve um JSON e finaliza um HttpResponse | 
| `void` | WriteJSON(this `HttpResponse` HttpResponse, `Func<Type>` ActResponse, `String` SuccessMessage = , `String` ErrorMessage = ) | Escreve um JSON e finaliza um HttpResponse | 
| `void` | WriteJSON(this `HttpResponse` HttpResponse, `HtmlDocument` Document) | Escreve um JSON e finaliza um HttpResponse | 
| `void` | WriteRedirect(this `HttpResponse` Response, `Uri` Url, `String` Message = , `Int32` RefreshTime = 1) | Esreve um script de Redirect na pagina e finaliza um HttpResponse | 
| `void` | WriteScript(this `HttpResponse` Response, `String` ScriptOrURL) | Escreve um script na página | 
| `void` | WriteXML(this `HttpResponse` Response, `String` XML) | Escreve um XML e finaliza um HttpResponse | 
| `void` | WriteXML(this `HttpResponse` Response, `XmlDocument` XML) | Escreve um XML e finaliza um HttpResponse | 


## `WinForms`

Módulo de controle de formulários
```csharp
public class InnerLibs.WinForms

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | AddClick(this `Control&` Control, `EventHandler` Action) | Adiciona funções ao clique de algum controle | 
| `void` | Alert(`String` Message) | Exibe uma caixa de alerta com uma mensagem | 
| `void` | CastValueForControl(this `Object&` Control, `Object` Value) | Aplica um valor a um controle ou variavel dependendo do seu tipo | 
| `Boolean` | Confirm(`String` Message) | Exibe uma caixa de comfirmação com uma mensagem | 
| `Dictionary<String, Object>` | CreateDictionary(this `IEnumerable<Control>` Controls) | Cria um `System.Collections.Generic.Dictionary`2` a partir de uma lista de controles onde a KEY será o nome do controle e Value será extraido do valor de acordo com seu tipo | 
| `Font` | CreateFontFromFile(`String` Path, `Int32` Size = 12, `FontStyle` Style = Regular) | Cria uma Font a partir de um arquivo | 
| `Font` | CreateFontFromResource(`Byte[]` Resource, `Int32` Size = 12, `FontStyle` Style = Regular) | Cria uma Font a partir de um arquivo embutido | 
| `IEnumerable<ControlType>` | GetAllControls(this `Control` Control) | Pega todos os controles filhos de um controle pai | 
| `IEnumerable<ControlType>` | GetAllControls(this `Control` Control) | Pega todos os controles filhos de um controle pai | 
| `Type` | GetAllControls(`Control` Control, `Func<Control, Boolean>` predicate) | Pega todos os controles filhos de um controle pai | 
| `List<TreeNode>` | GetAllNodes(this `TreeNode` Node) | Traz todos os nós descendentes de um nó pai | 
| `List<TreeNode>` | GetAllNodes(this `TreeView` Node) | Traz todos os nós descendentes de um nó pai | 
| `Object` | GetCastedValue(this `Object` Control) | Retorna o valor de um Controle de formulário ou variavel de acordo com seu tipo | 
| `NotificationForm` | Notify(`String` Text, `EventHandler` Action = null, `String` OKButtonText = OK, `Size` Size = null, `Int32` LifeTimeSeconds = 10, `Boolean` ShowRemainTime = False, `RemainTimeBehavior` RemainTimeBehavior = StackTime) | Exibe uma notificação com uma mensagem | 
| `String` | Prompt(`String` Message, `String` DefaultText = ) | Exibe uma caixa de mensagem ao usuário esperando uma resposta | 
| `void` | RemoveClick(this `Control&` Control, `EventHandler` Action) | Remove funções do clique de algum controle | 
| `void` | RestartAsAdmin(`Boolean` ForceRestart = False) | Reinicia a aplicação solicitando acesso administrativo se a mesma já não estiver em modo administrativo | 
| `void` | SetTelephoneMask(this `MaskedTextBox` TheTextBox) | Aplica máscara de telefone com ou sem o nono dígito automaticamente de acordo com o número  inputado. Utilize este metodo no Evento GotFocus e LostFocus simultaneamente | 
| `Form` | ToFullScreen(this `Form` Form, `Int32` TheScreen = 0) | Deixa o Form em tela cheia. | 


