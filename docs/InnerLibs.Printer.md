## `Printer`

```csharp
public class InnerLibs.Printer.Printer

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AutoPrint |  | 
| `Int32` | ColumnsCondensed |  | 
| `Int32` | ColumnsExpanded |  | 
| `Int32` | ColumnsNormal |  | 
| `IPrintCommand` | Command |  | 
| `Boolean` | Diacritics |  | 
| `Byte[]` | DocumentBuffer |  | 
| `XDocument` | HTMLDocument |  | 
| `Boolean` | IsBold |  | 
| `Boolean` | IsCenterAligned |  | 
| `Boolean` | IsCondensed |  | 
| `Boolean` | IsExpanded |  | 
| `Boolean` | IsItalic |  | 
| `Boolean` | IsLarge |  | 
| `Boolean` | IsLeftAligned |  | 
| `Boolean` | IsMedium |  | 
| `Boolean` | IsNormal |  | 
| `Boolean` | IsRightAligned |  | 
| `Boolean` | IsUnderline |  | 
| `Boolean` | OnOff |  | 
| `String` | PrinterName |  | 
| `Func<String, String>` | RewriteFunction |  | 
| `TextWriter` | TextWriter |  | 


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
| `Printer` | DontUseDiacritics() |  | 
| `Printer` | Ean13(`String` code) |  | 
| `Printer` | Expanded(`Boolean` state = True) |  | 
| `Printer` | FullPaperCut() |  | 
| `Int32` | GetCurrentColumns() |  | 
| `Printer` | Image(`String` Path, `Boolean` HighDensity = True) |  | 
| `Printer` | Image(`Stream` Stream, `Boolean` HighDensity = True) |  | 
| `Printer` | Image(`Byte[]` Bytes, `Boolean` HighDensity = True) |  | 
| `Printer` | Image(`Image` Img, `Boolean` HighDensity = True) |  | 
| `Printer` | Initialize() |  | 
| `Printer` | Italic(`Boolean` state = True) |  | 
| `Printer` | LargeFontSize() |  | 
| `Printer` | MediumFontSize() |  | 
| `Printer` | NewLine(`Int32` Lines = 1) |  | 
| `Printer` | NormalFontSize() |  | 
| `Printer` | NormalFontStretch() |  | 
| `Printer` | NormalFontStyle() |  | 
| `Printer` | NotBold() |  | 
| `Printer` | NotCondensed() |  | 
| `Printer` | NotExpanded() |  | 
| `Printer` | NotItalic() |  | 
| `Printer` | NotUnderline() |  | 
| `Printer` | OpenDrawer() |  | 
| `Printer` | PartialPaperCut() |  | 
| `Printer` | PrintDocument(`Int32` Copies = 1, `Boolean` Clear = True) |  | 
| `Printer` | PrintDocument(`String` FileOrDirectoryPath, `Int32` Copies = 1) |  | 
| `Printer` | PrintDocument(`Byte[]` Bytes, `Int32` Copies = 1) |  | 
| `Printer` | QrCode(`String` qrData) |  | 
| `Printer` | QrCode(`String` qrData, `QrCodeSize` qrCodeSize) |  | 
| `Printer` | RemoveRewriteFunction() |  | 
| `Printer` | ResetFont() |  | 
| `Printer` | SaveFile(`String` FileOrDirectoryPath, `Boolean` IncludeHtmlDoc = False) |  | 
| `Printer` | Separator(`Char` Character = -, `Nullable<Int32>` Columns = null) |  | 
| `Printer` | Space(`Int32` Spaces = 1) |  | 
| `Printer` | TestDiacritics() |  | 
| `Printer` | UnderLine(`Boolean` state = True) |  | 
| `Printer` | UseDiacritics(`Boolean` OnOff = True) |  | 
| `Printer` | UseRewriteFunction(`Func<String, String>` StringAction) |  | 
| `Printer` | Write(`Byte[]` value) |  | 
| `Printer` | Write(`String` value, `Boolean` Test = True) |  | 
| `Printer` | Write(`String` value, `Expression<Func<String, Boolean>>` Test) |  | 
| `Printer` | WriteClass(`T[]` Objects) |  | 
| `Printer` | WriteClass(`Boolean` PartialCutOnEach, `T[]` Objects) |  | 
| `Printer` | WriteClass(`IEnumerable<T>` obj, `Boolean` PartialCutOnEach = False) |  | 
| `Printer` | WriteDate(`DateTime` DateAndTime, `String` Format = null) |  | 
| `Printer` | WriteDate(`DateTime` DateAndTime, `CultureInfo` Format = null) |  | 
| `Printer` | WriteDate(`String` Format = null) |  | 
| `Printer` | WriteDate(`CultureInfo` Format = null) |  | 
| `Printer` | WriteDictionary(`IEnumerable<IDictionary<T1, T2>>` Dictionaries, `Boolean` PartialCutOnEach = False) |  | 
| `Printer` | WriteDictionary(`IDictionary<T1, T2>` dic, `Boolean` PartialCutOnEach = False) |  | 
| `Printer` | WriteDictionary(`IDictionary`2[]` Dictionaries) |  | 
| `Printer` | WriteDictionary(`Boolean` PartialCutOnEach, `IDictionary`2[]` Dictionaries) |  | 
| `Printer` | WriteLine(`String` value, `Expression<Func<String, Boolean>>` Test) |  | 
| `Printer` | WriteLine(`String` value, `Boolean` Test) |  | 
| `Printer` | WriteLine(`String` value) |  | 
| `Printer` | WriteLine(`String[]` values) |  | 
| `Printer` | WriteList(`IEnumerable<Object>` Items, `Int32` ListOrdenator = 1) |  | 
| `Printer` | WriteList(`IEnumerable<Object>` Items, `String` ListOrdenator) |  | 
| `Printer` | WriteList(`Object[]` Items) |  | 
| `Printer` | WritePair(`Object` Key, `Object` Value, `Nullable<Int32>` Columns = null, `Char` CharLine =  ) |  | 
| `Printer` | WritePriceLine(`String` Description, `Decimal` Price, `CultureInfo` Culture = null, `Nullable<Int32>` Columns = null, `Char` CharLine = .) |  | 
| `Printer` | WritePriceList(`IEnumerable<Tuple<String, Decimal>>` List, `CultureInfo` Culture = null, `Nullable<Int32>` Columns = null, `Char` CharLine = .) |  | 
| `Printer` | WritePriceList(`IEnumerable<T>` List, `Expression<Func<T, String>>` Description, `Expression<Func<T, Decimal>>` Price, `CultureInfo` Culture = null, `Nullable<Int32>` Columns = null, `Char` CharLine = .) |  | 
| `Printer` | WriteScriptLine(`Nullable<Int32>` Columns = null, `String` Name = ) |  | 
| `Printer` | WriteTable(`IEnumerable<T>` Items) |  | 
| `Printer` | WriteTable(`T[]` Items) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `Boolean` PartialCutOnEach, `T[]` Objects) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `T[]` obj) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `IEnumerable<T>` obj, `Boolean` PartialCutOnEach = False) |  | 
| `Printer` | WriteTemplate(`String` TemplateString, `T` obj, `Boolean` PartialCutOnEach = False) |  | 
| `Printer` | WriteTest() |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Printer` | CreatePrinter(`String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 
| `Printer` | CreatePrinter(`Type` CommandType, `String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 
| `Printer` | CreatePrinter(`IPrintCommand` CommandType, `String` PrinterName, `Int32` ColsNormal = 0, `Int32` ColsCondensed = 0, `Int32` ColsExpanded = 0, `Encoding` Encoding = null) |  | 


## `PrinterExtension`

```csharp
public static class InnerLibs.Printer.PrinterExtension

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


