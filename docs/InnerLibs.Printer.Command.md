## `IPrintCommand`

```csharp
public interface InnerLibs.Printer.Command.IPrintCommand

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | ColsCondensed |  | 
| `Int32` | ColsExpanded |  | 
| `Int32` | ColsNormal |  | 
| `Encoding` | Encoding |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Byte[]` | AutoTest() |  | 
| `Byte[]` | Bold(`Boolean` state) |  | 
| `Byte[]` | Center() |  | 
| `Byte[]` | Code128(`String` code) |  | 
| `Byte[]` | Code39(`String` code) |  | 
| `Byte[]` | Condensed(`Boolean` state) |  | 
| `Byte[]` | Ean13(`String` code) |  | 
| `Byte[]` | Expanded(`Boolean` state) |  | 
| `Byte[]` | FullCut() |  | 
| `Byte[]` | Initialize() |  | 
| `Byte[]` | Italic(`Boolean` state) |  | 
| `Byte[]` | LargeFont() |  | 
| `Byte[]` | LargerFont() |  | 
| `Byte[]` | Left() |  | 
| `Byte[]` | NormalFont() |  | 
| `Byte[]` | OpenDrawer() |  | 
| `Byte[]` | PartialCut() |  | 
| `Byte[]` | PrintImage(`Image` image, `Boolean` highDensity) |  | 
| `Byte[]` | PrintQrData(`String` qrData) |  | 
| `Byte[]` | PrintQrData(`String` qrData, `QrCodeSize` qrCodeSize) |  | 
| `Byte[]` | Right() |  | 
| `Byte[]` | Underline(`Boolean` state) |  | 


