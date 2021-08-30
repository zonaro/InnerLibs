## `Printer`

```csharp
public class InnerLibs.Printer.Printer

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | ColsCondensed |  | 
| `Int32` | ColsExpanded |  | 
| `Int32` | ColsNomal |  | 
| `IPrintCommand` | Command |  | 
| `Boolean` | Diacritics |  | 
| `Byte[]` | DocumentBuffer |  | 
| `XDocument` | HTMLDocument |  | 
| `Boolean` | IsBold |  | 
| `Boolean` | IsCenterAligned |  | 
| `Boolean` | IsCondensed |  | 
| `Boolean` | IsDoubleWidth2 |  | 
| `Boolean` | IsDoubleWidth3 |  | 
| `Boolean` | IsExpanded |  | 
| `Boolean` | IsItalic |  | 
| `Boolean` | IsLeftAligned |  | 
| `Boolean` | IsNormal |  | 
| `Boolean` | IsRightAligned |  | 
| `Boolean` | IsUnderline |  | 
| `String` | PrinterName |  | 
| `Func<String, String>` | RewriteFunction |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Printer` | AlignCenter() |  | 
| `Printer` | AlignLeft() |  | 
| `Printer` | AlignRight() |  | 
| `Printer` | AutoTest() |  | 
| `Printer` | Bold(`Boolean` state = True) |  | 
| `Printer` | Clear() |  | 
| `Printer` | Code128(`String` code) |  | 
| `Printer` | Code39(`String` code) |  | 
| `Printer` | Condensed(`Boolean` state = True) |  | 
| `Printer` | DontUseDiacritics() | Remove todos os acentod das chamadas `InnerLibs.Printer.Printer.Write(System.String,System.Boolean)` posteriores | 
| `Printer` | DoubleWidth2() |  | 
| `Printer` | DoubleWidth3() |  | 
| `Printer` | Ean13(`String` code) |  | 
| `Printer` | Expanded(`Boolean` state = True) |  | 
| `Printer` | FullPaperCut() |  | 
| `Printer` | Image(`String` Path, `Boolean` highDensity = True) |  | 
| `Printer` | Image(`Stream` stream, `Boolean` HighDensity = True) |  | 
| `Printer` | Image(`Byte[]` bytes, `Boolean` HighDensity = True) |  | 
| `Printer` | Image(`Image` pImage, `Boolean` highDensity = True) |  | 
| `Printer` | InitializePrint() |  | 
| `Printer` | Italic(`Boolean` state = True) |  | 
| `Printer` | NewLine(`Int32` Lines = 1) | Adciona um numero `` de quebras de linha | 
| `Printer` | NormalWidth() |  | 
| `Printer` | NotBold() |  | 
| `Printer` | NotCondensed() |  | 
| `Printer` | NotExpanded() |  | 
| `Printer` | NotItalic() |  | 
| `Printer` | NotUnderline() |  | 
| `Printer` | OpenDrawer() |  | 
| `Printer` | PartialPaperCut() |  | 
| `Printer` | PrintDocument(`Int32` Copies = 1) | Imprime o conteudo do `InnerLibs.Printer.Printer.DocumentBuffer` atual e limpa o buffer | 
| `Printer` | PrintDocument(`String` FileOrDirectoryPath, `Int32` Copies = 1) | Imprime o conteudo do `InnerLibs.Printer.Printer.DocumentBuffer` atual e limpa o buffer | 
| `Printer` | PrintDocument(`Byte[]` Bytes, `Int32` Copies = 1) | Imprime o conteudo do `InnerLibs.Printer.Printer.DocumentBuffer` atual e limpa o buffer | 
| `Printer` | QrCode(`String` qrData) |  | 
| `Printer` | QrCode(`String` qrData, `QrCodeSize` qrCodeSize) |  | 
| `Printer` | RemoveRewriteFunction() | Remove a função de reescrita de valor definida pela `InnerLibs.Printer.Printer.UseRewriteFunction(System.Func{System.String,System.String})` | 
| `Printer` | SaveFile(`String` FileOrDirectoryPath, `Boolean` IncludeHtmlDoc = True) | Escreve um Arquivo com os dados binarios desta impressao | 
| `Printer` | Separator(`Char` Character = -) |  | 
| `Printer` | Space(`Int32` Spaces = 1) | Adciona um numero `` de espaços em branco | 
| `Printer` | TestDiacritics() |  | 
| `Printer` | UnderLine(`Boolean` state = True) |  | 
| `Printer` | UseDiacritics(`Boolean` OnOff = True) | Permite a ultilização de acentos nas chamadas `InnerLibs.Printer.Printer.Write(System.String,System.Boolean)` posteriores | 
| `Printer` | UseRewriteFunction(`Func<String, String>` StringAction) | Funcao que reescreveo valor antes de chamar o `InnerLibs.Printer.Printer.Write(System.String,System.Boolean)` | 
| `Printer` | Write(`String` value, `Boolean` Test = True) | Escreve os bytes contidos em `` no `InnerLibs.Printer.Printer.DocumentBuffer` | 
| `Printer` | Write(`Byte[]` value) | Escreve os bytes contidos em `` no `InnerLibs.Printer.Printer.DocumentBuffer` | 
| `Printer` | WriteClass(`T[]` objs) |  | 
| `Printer` | WriteClass(`Boolean` PartialCutOnEach, `T[]` objs) |  | 
| `Printer` | WriteClass(`IEnumerable<T>` obj, `Boolean` PartialCutOnEach = False) |  | 
| `Object` | WriteDate(`DateTime` DateAndTime, `String` Format = null) |  | 
| `Object` | WriteDate(`String` Format = null) |  | 
| `Printer` | WriteDictionary(`IDictionary`2[]` dics) |  | 
| `Printer` | WriteDictionary(`Boolean` PartialCutOnEach, `IDictionary`2[]` dics) |  | 
| `Printer` | WriteDictionary(`IEnumerable<IDictionary<T1, T2>>` dics, `Boolean` PartialCutOnEach = False) |  | 
| `Printer` | WriteLine(`String` value, `Boolean` Test = True) | Escreve o `` se `` for TRUE | 
| `Printer` | WriteList(`IEnumerable<Object>` Items, `String` ListOrdenator = null) |  | 
| `Printer` | WriteList(`Object[]` Items) |  | 
| `Printer` | WritePair(`Object` Key, `Object` Value) |  | 
| `Printer` | WritePriceLine(`String` Description, `Decimal` Price, `CultureInfo` Culture = null, `Nullable<Int32>` Columns = null) |  | 
| `Printer` | WritePriceList(`IEnumerable<Tuple<String, Decimal>>` List, `CultureInfo` Culture = null, `Nullable<Int32>` Columns = null) |  | 
| `Printer` | WritePriceList(`IEnumerable<T>` List, `Expression<Func<T, String>>` Description, `Expression<Func<T, Decimal>>` Price, `CultureInfo` Culture = null, `Nullable<Int32>` Columns = null) |  | 
| `Printer` | WriteTable(`IEnumerable<T>` Items) |  | 
| `Printer` | WriteTable(`T[]` Items) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `Boolean` PartialCutOnEach, `T[]` obj) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `T[]` obj) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `IEnumerable<T>` obj, `Boolean` PartiaCutOnEach = False) |  | 
| `Printer` | WriteTest() |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Printer` | CreatePrinter(`String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 
| `Printer` | CreatePrinter(`Type` CommandType, `String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 
| `Printer` | CreatePrinter(`IPrintCommand` CommandType, `String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 
| `String` | FixAccents(`String` Lin) |  | 


## `PrinterExtension`

```csharp
public class InnerLibs.Printer.PrinterExtension

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Printer` | CreatePrinter(this `IPrintCommand` CommandType, `String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 


## `QrCodeSize`

```csharp
public enum InnerLibs.Printer.QrCodeSize
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Size0 |  | 
| `1` | Size1 |  | 
| `2` | Size2 |  | 


