## `AsciiArt`

```csharp
public static class InnerLibs.AsciiArt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToAsciiArt(this `Bitmap` image, `Int32` ratio) |  | 


## `Base64`

```csharp
public static class InnerLibs.Base64

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Atob(this `String` Base, `Encoding` Encoding = null) |  | 
| `Byte[]` | Base64ToBytes(this `String` Base64StringOrDataURL) |  | 
| `Image` | Base64ToImage(this `String` DataUrlOrBase64String, `Int32` Width = 0, `Int32` Height = 0) |  | 
| `String` | Btoa(this `String` Text, `Encoding` Encoding = null) |  | 
| `FileInfo` | CreateFileFromDataURL(this `String` Base64StringOrDataURL, `String` FilePath) |  | 
| `String` | FixBase64(this `String` Base64StringOrDataUrl) |  | 
| `Boolean` | IsDataURL(this `String` Text) |  | 
| `String` | ToBase64(this `Byte[]` Bytes) |  | 
| `String` | ToBase64(this `Image` OriginalImage, `ImageFormat` OriginalImageFormat) |  | 
| `String` | ToBase64(this `Image` OriginalImage) |  | 
| `String` | ToBase64(this `Uri` ImageURL) |  | 
| `String` | ToBase64(this `String` ImageURL, `ImageFormat` OriginalImageFormat) |  | 
| `String` | ToDataURL(this `Byte[]` Bytes, `FileType` Type = null) |  | 
| `String` | ToDataURL(this `Byte[]` Bytes, `String` MimeType) |  | 
| `String` | ToDataURL(this `FileInfo` File) |  | 
| `String` | ToDataURL(this `Image` Image) |  | 
| `String` | ToDataURL(this `Image` OriginalImage, `ImageFormat` OriginalImageFormat) |  | 
| `Image` | ToImage(this `Byte[]` Bytes) |  | 


## `ColorExtensions`

```csharp
public static class InnerLibs.ColorExtensions

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<Color>` | KnowColors |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HSVColor` | FindColor(this `String` Text) |  | 
| `String` | GetClosestColorName(this `Color` Color) |  | 
| `Color` | GetClosestKnowColor(this `Color` Color) |  | 
| `String` | GetColorName(this `Color` Color) |  | 
| `Color` | GetContrastColor(this `Color` TheColor, `Single` Percent = 70) |  | 
| `Color` | GetNegativeColor(this `Color` TheColor) |  | 
| `IEnumerable<HSVColor>` | GrayscalePallette(`Int32` Amount) |  | 
| `Boolean` | IsDark(this `Color` TheColor) |  | 
| `Boolean` | IsHexaDecimalColor(this `String` Text) |  | 
| `Boolean` | IsLight(this `Color` TheColor) |  | 
| `Boolean` | IsReadable(this `Color` Color, `Color` BackgroundColor, `Int32` Size = 10) |  | 
| `Color` | Lerp(this `Color` FromColor, `Color` ToColor, `Single` Amount) |  | 
| `Color` | MakeDarker(this `Color` TheColor, `Single` Percent = 50) |  | 
| `Color` | MakeLighter(this `Color` TheColor, `Single` Percent = 50) |  | 
| `Color` | MergeWith(this `Color` TheColor, `Color` AnotherColor, `Single` Percent = 50) |  | 
| `IEnumerable<HSVColor>` | MonochromaticPallette(`Color` Color, `Int32` Amount) |  | 
| `Color` | RandomColor(`Int32` Red = -1, `Int32` Green = -1, `Int32` Blue = -1, `Int32` Alpha = 255) |  | 
| `HSVColor` | ToColor(this `ConsoleColor` Color) |  | 
| `Color` | ToColor(this `String` Text) |  | 
| `ConsoleColor` | ToConsoleColor(this `Color` Color) |  | 
| `String` | ToCssRGB(this `Color` Color) |  | 
| `String` | ToCssRGBA(this `Color` Color) |  | 
| `String` | ToHexadecimal(this `Color` Color, `Boolean` Hash = True) |  | 
| `IEnumerable<HSVColor>` | ToHSVColorList(this `IEnumerable<Color>` ColorList) |  | 


## `ColorMood`

```csharp
public enum InnerLibs.ColorMood
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `-65537` | Water |  | 
| `1` | Dark |  | 
| `2` | MediumDark |  | 
| `4` | Medium |  | 
| `8` | MediumLight |  | 
| `16` | Light |  | 
| `32` | Sad |  | 
| `64` | Happy |  | 
| `128` | Cooler |  | 
| `256` | Cool |  | 
| `512` | Warm |  | 
| `1024` | Warmer |  | 
| `2048` | Invisible |  | 
| `4096` | SemiVisible |  | 
| `8192` | Visible |  | 
| `16384` | LowLuminance |  | 
| `32768` | HighLuminance |  | 
| `65536` | Red |  | 
| `131072` | Green |  | 
| `262144` | Blue |  | 
| `524288` | MostRed |  | 
| `1048576` | MostGreen |  | 
| `1048640` | Nature |  | 
| `2097152` | MostBlue |  | 
| `4194304` | NoRed |  | 
| `4456576` | Ice |  | 
| `8388608` | NoGreen |  | 
| `8912960` | Love |  | 
| `12582912` | FullBlue |  | 
| `16777216` | NoBlue |  | 
| `16843776` | Fire |  | 
| `20971520` | FullGreen |  | 
| `25165824` | FullRed |  | 


## `ConnectionStringParser`

```csharp
public class InnerLibs.ConnectionStringParser
    : Dictionary<String, String>, IDictionary<String, String>, ICollection<KeyValuePair<String, String>>, IEnumerable<KeyValuePair<String, String>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<String, String>, IReadOnlyCollection<KeyValuePair<String, String>>, ISerializable, IDeserializationCallback

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ConnectionString |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `ConnectionStringParser` | Parse(`String` ConnectionString) |  | 
| `String` | ToString() |  | 


## `Converter`

```csharp
public static class InnerLibs.Converter

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AsBool(this `String` Value) |  | 
| `ToType[]` | ChangeArrayType(this `FromType[]` Value) |  | 
| `Object[]` | ChangeArrayType(this `FromType[]` Value, `Type` Type) |  | 
| `IEnumerable<ToType>` | ChangeIEnumerableType(this `IEnumerable<FromType>` Value) |  | 
| `IEnumerable<Object>` | ChangeIEnumerableType(this `IEnumerable<FromType>` Value, `Type` ToType) |  | 
| `ToType` | ChangeType(this `Object` Value) |  | 
| `Object` | ChangeType(this `FromType` Value, `Type` ToType) |  | 
| `Object` | CreateOrSetObject(this `Dictionary<String, Object>` Dic, `Object` Obj, `Type` Type, `Object[]` args) |  | 
| `List<T>` | DefineEmptyList(this `T` ObjectForDefinition) |  | 
| `Object[]` | ForceArray(this `Object` Obj, `Type` Type) |  | 
| `OutputType[]` | ForceArray(this `Object` Obj) |  | 
| `Dictionary<Tkey, Object>` | Merge(this `Dictionary<Tkey, Object>` FirstDictionary, `Dictionary`2[]` Dictionaries) |  | 
| `IEnumerable<Dictionary<TKey, TValue>>` | MergeKeys(this `IEnumerable<Dictionary<TKey, TValue>>` Dics, `TKey[]` AditionalKeys) |  | 
| `T` | SetValuesIn(this `Dictionary<String, Object>` Dic) |  | 
| `T` | SetValuesIn(this `Dictionary<String, Object>` Dic, `T` obj, `Object[]` args) |  | 
| `T` | SetValuesIn(this `Dictionary<String, Object>` Dic, `T` obj) |  | 
| `List<T>` | StartList(this `T` ObjectForDefinition) |  | 
| `Boolean` | ToBool(this `FromType` Value) |  | 
| `DateTime` | ToDateTime(this `FromType` Value) |  | 
| `DateTime` | ToDateTime(this `FromType` Value, `String` CultureInfoName) |  | 
| `DateTime` | ToDateTime(this `FromType` Value, `CultureInfo` CultureInfo) |  | 
| `Decimal` | ToDecimal(this `FromType` Value) |  | 
| `Dictionary<TKey, IEnumerable<TValue>>` | ToDictionary(this `IEnumerable<IGrouping<TKey, TValue>>` groupings) |  | 
| `Dictionary<TKey, TValue>` | ToDictionary(this `IEnumerable<KeyValuePair<TKey, TValue>>` items, `TKey[]` Keys) |  | 
| `Dictionary<String, Object>` | ToDictionary(this `NameValueCollection` NameValueCollection, `String[]` Keys) |  | 
| `Double` | ToDouble(this `FromType` Value) |  | 
| `Int32` | ToInt(this `FromType` Value) |  | 
| `Int64` | ToLong(this `FromType` Value) |  | 
| `Double` | ToShort(this `FromType` Value) |  | 
| `Single` | ToSingle(this `FromType` Value) |  | 


## `DataURI`

```csharp
public class InnerLibs.DataURI

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Data |  | 
| `String` | Encoding |  | 
| `String` | Extension |  | 
| `String` | FullMimeType |  | 
| `String` | Mime |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Byte[]` | ToBytes() |  | 
| `FileType` | ToFileType() |  | 
| `String` | ToString() |  | 
| `FileInfo` | WriteToFile(`String` Path, `Nullable<DateTime>` dateTime = null) |  | 


## `DebugTextWriter`

```csharp
public class InnerLibs.DebugTextWriter
    : StreamWriter, IDisposable

```

## `Directories`

```csharp
public static class InnerLibs.Directories

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `DirectoryInfo` | CleanDirectory(this `DirectoryInfo` TopDirectory, `Boolean` DeleteTopDirectoryIfEmpty = True) |  | 
| `IEnumerable<FileInfo>` | CopyTo(this `IEnumerable<FileInfo>` List, `DirectoryInfo` DestinationDirectory) |  | 
| `DirectoryInfo` | CreateDirectoryIfNotExists(this `String` DirectoryName) |  | 
| `DirectoryInfo` | CreateDirectoryIfNotExists(this `DirectoryInfo` DirectoryName) |  | 
| `DirectoryInfo` | CreateDirectoryIfNotExists(this `FileInfo` FileName) |  | 
| `FileInfo` | CreateFileIfNotExists(this `String` FileName, `FileType` Type = null) |  | 
| `Boolean` | DeleteIfExist(this `String` Path) |  | 
| `Boolean` | DeleteIfExist(this `FileSystemInfo` Path) |  | 
| `Boolean` | HasDirectories(this `DirectoryInfo` Directory) |  | 
| `Boolean` | HasFiles(this `DirectoryInfo` Directory) |  | 
| `T` | Hide(this `T` dir) |  | 
| `Boolean` | IsEmpty(this `DirectoryInfo` Directory) |  | 
| `Boolean` | IsNotEmpty(this `DirectoryInfo` Directory) |  | 
| `Boolean` | IsVisible(this `T` dir) |  | 
| `IEnumerable<String>` | ReadManyText(this `DirectoryInfo` directory, `SearchOption` Option, `String[]` Patterns) |  | 
| `IEnumerable<String>` | ReadManyText(this `DirectoryInfo` directory, `String[]` Patterns) |  | 
| `List<FileSystemInfo>` | Search(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) |  | 
| `List<FileSystemInfo>` | SearchBetween(this `DirectoryInfo` Directory, `DateTime` FirstDate, `DateTime` SecondDate, `SearchOption` SearchOption, `String[]` Searches) |  | 
| `List<DirectoryInfo>` | SearchDirectories(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) |  | 
| `List<DirectoryInfo>` | SearchDirectoriesBetween(this `DirectoryInfo` Directory, `DateTime` FirstDate, `DateTime` SecondDate, `SearchOption` SearchOption, `String[]` Searches) |  | 
| `IEnumerable<FileInfo>` | SearchFiles(this `DirectoryInfo` Directory, `SearchOption` SearchOption, `String[]` Searches) |  | 
| `List<FileInfo>` | SearchFilesBetween(this `DirectoryInfo` Directory, `DateTime` FirstDate, `DateTime` SecondDate, `SearchOption` SearchOption, `String[]` Searches) |  | 
| `T` | Show(this `T` dir) |  | 
| `T` | ToggleVisibility(this `T` dir) |  | 
| `IEnumerable<FindType>` | Where(this `DirectoryInfo` Directory, `Func<FindType, Boolean>` predicate, `SearchOption` SearchOption = AllDirectories) |  | 


## `Emoji`

```csharp
public class InnerLibs.Emoji

```

Static Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | _100 |  | 
| `String` | _1234 |  | 
| `String` | _8Ball |  | 
| `String` | A |  | 
| `String` | Ab |  | 
| `String` | Abc |  | 
| `String` | Abcd |  | 
| `String` | Accept |  | 
| `String` | Aerial_Tramway |  | 
| `String` | Airplane |  | 
| `String` | Alarm_Clock |  | 
| `String` | Alien |  | 
| `String` | Ambulance |  | 
| `String` | Anchor |  | 
| `String` | Angel |  | 
| `String` | Anger |  | 
| `String` | Angry |  | 
| `String` | Anguished |  | 
| `String` | Ant |  | 
| `String` | Apple |  | 
| `String` | Aquarius |  | 
| `String` | Aries |  | 
| `String` | Arrow_Backward |  | 
| `String` | Arrow_Double_Down |  | 
| `String` | Arrow_Double_Up |  | 
| `String` | Arrow_Down |  | 
| `String` | Arrow_Down_Small |  | 
| `String` | Arrow_Forward |  | 
| `String` | Arrow_Heading_Down |  | 
| `String` | Arrow_Heading_Up |  | 
| `String` | Arrow_Left |  | 
| `String` | Arrow_Lower_Left |  | 
| `String` | Arrow_Lower_Right |  | 
| `String` | Arrow_Right |  | 
| `String` | Arrow_Right_Hook |  | 
| `String` | Arrow_Up |  | 
| `String` | Arrow_Up_Down |  | 
| `String` | Arrow_Up_Small |  | 
| `String` | Arrow_Upper_Left |  | 
| `String` | Arrow_Upper_Right |  | 
| `String` | Arrows_Clockwise |  | 
| `String` | Arrows_Counterclockwise |  | 
| `String` | Art |  | 
| `String` | Articulated_Lorry |  | 
| `String` | Astonished |  | 
| `String` | Athletic_Shoe |  | 
| `String` | Atm |  | 
| `String` | B |  | 
| `String` | Baby |  | 
| `String` | Baby_Bottle |  | 
| `String` | Baby_Chick |  | 
| `String` | Baby_Symbol |  | 
| `String` | Back |  | 
| `String` | Baggage_Claim |  | 
| `String` | Balloon |  | 
| `String` | Ballot_Box_With_Check |  | 
| `String` | Bamboo |  | 
| `String` | Banana |  | 
| `String` | Bangbang |  | 
| `String` | Bank |  | 
| `String` | Bar_Chart |  | 
| `String` | Barber |  | 
| `String` | Baseball |  | 
| `String` | Basketball |  | 
| `String` | Bath |  | 
| `String` | Bathtub |  | 
| `String` | Battery |  | 
| `String` | Bear |  | 
| `String` | Bee |  | 
| `String` | Beer |  | 
| `String` | Beers |  | 
| `String` | Beetle |  | 
| `String` | Beginner |  | 
| `String` | Bell |  | 
| `String` | Bento |  | 
| `String` | Bicyclist |  | 
| `String` | Bike |  | 
| `String` | Bikini |  | 
| `String` | Bird |  | 
| `String` | Birthday |  | 
| `String` | Black_Circle |  | 
| `String` | Black_Joker |  | 
| `String` | Black_Large_Square |  | 
| `String` | Black_Medium_Small_Square |  | 
| `String` | Black_Medium_Square |  | 
| `String` | Black_Nib |  | 
| `String` | Black_Small_Square |  | 
| `String` | Black_Square_Button |  | 
| `String` | Blossom |  | 
| `String` | Blowfish |  | 
| `String` | Blue_Book |  | 
| `String` | Blue_Car |  | 
| `String` | Blue_Heart |  | 
| `String` | Blush |  | 
| `String` | Boar |  | 
| `String` | Bomb |  | 
| `String` | Book |  | 
| `String` | Bookmark |  | 
| `String` | Bookmark_Tabs |  | 
| `String` | Books |  | 
| `String` | Boom |  | 
| `String` | Boot |  | 
| `String` | Bouquet |  | 
| `String` | Bow |  | 
| `String` | Bowling |  | 
| `String` | Boy |  | 
| `String` | Bread |  | 
| `String` | Bride_With_Veil |  | 
| `String` | Bridge_At_Night |  | 
| `String` | Briefcase |  | 
| `String` | Broken_Heart |  | 
| `String` | Bug |  | 
| `String` | Bulb |  | 
| `String` | Bullettrain_Front |  | 
| `String` | Bullettrain_Side |  | 
| `String` | Bus |  | 
| `String` | Busstop |  | 
| `String` | Bust_In_Silhouette |  | 
| `String` | Busts_In_Silhouette |  | 
| `String` | Cactus |  | 
| `String` | Cake |  | 
| `String` | Calendar |  | 
| `String` | Calling |  | 
| `String` | Camel |  | 
| `String` | Camera |  | 
| `String` | Cancer |  | 
| `String` | Candy |  | 
| `String` | Capital_Abcd |  | 
| `String` | Capricorn |  | 
| `String` | Card_Index |  | 
| `String` | Carousel_Horse |  | 
| `String` | Cat |  | 
| `String` | Cat2 |  | 
| `String` | Cd |  | 
| `String` | Chart |  | 
| `String` | Chart_With_Downwards_Trend |  | 
| `String` | Chart_With_Upwards_Trend |  | 
| `String` | Checkered_Flag |  | 
| `String` | Cherries |  | 
| `String` | Cherry_Blossom |  | 
| `String` | Chestnut |  | 
| `String` | Chicken |  | 
| `String` | Children_Crossing |  | 
| `String` | Chocolate_Bar |  | 
| `String` | Christmas_Tree |  | 
| `String` | Church |  | 
| `String` | Cinema |  | 
| `String` | Circus_Tent |  | 
| `String` | City_Sunrise |  | 
| `String` | City_Sunset |  | 
| `String` | Cl |  | 
| `String` | Clap |  | 
| `String` | Clapper |  | 
| `String` | Clipboard |  | 
| `String` | Clock1 |  | 
| `String` | Clock10 |  | 
| `String` | Clock1030 |  | 
| `String` | Clock11 |  | 
| `String` | Clock1130 |  | 
| `String` | Clock12 |  | 
| `String` | Clock1230 |  | 
| `String` | Clock130 |  | 
| `String` | Clock2 |  | 
| `String` | Clock230 |  | 
| `String` | Clock3 |  | 
| `String` | Clock330 |  | 
| `String` | Clock4 |  | 
| `String` | Clock430 |  | 
| `String` | Clock5 |  | 
| `String` | Clock530 |  | 
| `String` | Clock6 |  | 
| `String` | Clock630 |  | 
| `String` | Clock7 |  | 
| `String` | Clock730 |  | 
| `String` | Clock8 |  | 
| `String` | Clock830 |  | 
| `String` | Clock9 |  | 
| `String` | Clock930 |  | 
| `String` | Closed_Book |  | 
| `String` | Closed_Lock_With_Key |  | 
| `String` | Closed_Umbrella |  | 
| `String` | Cloud |  | 
| `String` | Clubs |  | 
| `String` | Cn |  | 
| `String` | Cocktail |  | 
| `String` | Coffee |  | 
| `String` | Cold_Sweat |  | 
| `String` | Computer |  | 
| `String` | Confetti_Ball |  | 
| `String` | Confounded |  | 
| `String` | Confused |  | 
| `String` | Congratulations |  | 
| `String` | Construction |  | 
| `String` | Construction_Worker |  | 
| `String` | Convenience_Store |  | 
| `String` | Cookie |  | 
| `String` | Cool |  | 
| `String` | Cop |  | 
| `String` | Copyright |  | 
| `String` | Corn |  | 
| `String` | Couple |  | 
| `String` | Couple_With_Heart |  | 
| `String` | Couplekiss |  | 
| `String` | Cow |  | 
| `String` | Cow2 |  | 
| `String` | Credit_Card |  | 
| `String` | Crescent_Moon |  | 
| `String` | Crocodile |  | 
| `String` | Crossed_Flags |  | 
| `String` | Crown |  | 
| `String` | Cry |  | 
| `String` | Crying_Cat_Face |  | 
| `String` | Crystal_Ball |  | 
| `String` | Cupid |  | 
| `String` | Curly_Loop |  | 
| `String` | Currency_Exchange |  | 
| `String` | Curry |  | 
| `String` | Custard |  | 
| `String` | Customs |  | 
| `String` | Cyclone |  | 
| `String` | Dancer |  | 
| `String` | Dancers |  | 
| `String` | Dango |  | 
| `String` | Dart |  | 
| `String` | Dash |  | 
| `String` | Date |  | 
| `String` | De |  | 
| `String` | Deciduous_Tree |  | 
| `String` | Department_Store |  | 
| `String` | Diamond_Shape_With_A_Dot_Inside |  | 
| `String` | Diamonds |  | 
| `String` | Disappointed |  | 
| `String` | Disappointed_Relieved |  | 
| `String` | Dizzy |  | 
| `String` | Dizzy_Face |  | 
| `String` | Do_Not_Litter |  | 
| `String` | Dog |  | 
| `String` | Dog2 |  | 
| `String` | Dollar |  | 
| `String` | Dolls |  | 
| `String` | Dolphin |  | 
| `String` | Door |  | 
| `String` | Doughnut |  | 
| `String` | Dragon |  | 
| `String` | Dragon_Face |  | 
| `String` | Dress |  | 
| `String` | Dromedary_Camel |  | 
| `String` | Droplet |  | 
| `String` | Dvd |  | 
| `String` | E_Mail |  | 
| `String` | Ear |  | 
| `String` | Ear_Of_Rice |  | 
| `String` | Earth_Africa |  | 
| `String` | Earth_Americas |  | 
| `String` | Earth_Asia |  | 
| `String` | Egg |  | 
| `String` | Eggplant |  | 
| `String` | Eight |  | 
| `String` | Eight_Pointed_Black_Star |  | 
| `String` | Eight_Spoked_Asterisk |  | 
| `String` | Electric_Plug |  | 
| `String` | Elephant |  | 
| `String` | End |  | 
| `String` | Envelope |  | 
| `String` | Envelope_With_Arrow |  | 
| `String` | Es |  | 
| `String` | Euro |  | 
| `String` | European_Castle |  | 
| `String` | European_Post_Office |  | 
| `String` | Evergreen_Tree |  | 
| `String` | Exclamation |  | 
| `String` | Expressionless |  | 
| `String` | Eyeglasses |  | 
| `String` | Eyes |  | 
| `String` | Factory |  | 
| `String` | Fallen_Leaf |  | 
| `String` | Family |  | 
| `String` | Fast_Forward |  | 
| `String` | Fax |  | 
| `String` | Fearful |  | 
| `String` | Feet |  | 
| `String` | Ferris_Wheel |  | 
| `String` | File_Folder |  | 
| `String` | Fire |  | 
| `String` | Fire_Engine |  | 
| `String` | Fireworks |  | 
| `String` | First_Quarter_Moon |  | 
| `String` | First_Quarter_Moon_With_Face |  | 
| `String` | Fish |  | 
| `String` | Fish_Cake |  | 
| `String` | Fishing_Pole_And_Fish |  | 
| `String` | Fist |  | 
| `String` | Five |  | 
| `String` | Flags |  | 
| `String` | Flashlight |  | 
| `String` | Floppy_Disk |  | 
| `String` | Flower_Playing_Cards |  | 
| `String` | Flushed |  | 
| `String` | Foggy |  | 
| `String` | Football |  | 
| `String` | Footprints |  | 
| `String` | Fork_And_Knife |  | 
| `String` | Fountain |  | 
| `String` | Four |  | 
| `String` | Four_Leaf_Clover |  | 
| `String` | Fr |  | 
| `String` | Free |  | 
| `String` | Fried_Shrimp |  | 
| `String` | Fries |  | 
| `String` | Frog |  | 
| `String` | Frowning |  | 
| `String` | Fuelpump |  | 
| `String` | Full_Moon |  | 
| `String` | Full_Moon_With_Face |  | 
| `String` | Game_Die |  | 
| `String` | Gem |  | 
| `String` | Gemini |  | 
| `String` | Ghost |  | 
| `String` | Gift |  | 
| `String` | Gift_Heart |  | 
| `String` | Girl |  | 
| `String` | Globe_With_Meridians |  | 
| `String` | Goat |  | 
| `String` | Golf |  | 
| `String` | Grapes |  | 
| `String` | Green_Apple |  | 
| `String` | Green_Book |  | 
| `String` | Green_Heart |  | 
| `String` | Grey_Exclamation |  | 
| `String` | Grey_Question |  | 
| `String` | Grimacing |  | 
| `String` | Grin |  | 
| `String` | Grinning |  | 
| `String` | Guardsman |  | 
| `String` | Guitar |  | 
| `String` | Gun |  | 
| `String` | Haircut |  | 
| `String` | Hamburger |  | 
| `String` | Hammer |  | 
| `String` | Hamster |  | 
| `String` | Handbag |  | 
| `String` | Hash |  | 
| `String` | Hatched_Chick |  | 
| `String` | Hatching_Chick |  | 
| `String` | Headphones |  | 
| `String` | Hear_No_Evil |  | 
| `String` | Heart |  | 
| `String` | Heart_Decoration |  | 
| `String` | Heart_Eyes |  | 
| `String` | Heart_Eyes_Cat |  | 
| `String` | Heartbeat |  | 
| `String` | Heartpulse |  | 
| `String` | Hearts |  | 
| `String` | Heavy_Check_Mark |  | 
| `String` | Heavy_Division_Sign |  | 
| `String` | Heavy_Dollar_Sign |  | 
| `String` | Heavy_Minus_Sign |  | 
| `String` | Heavy_Multiplication_X |  | 
| `String` | Heavy_Plus_Sign |  | 
| `String` | Helicopter |  | 
| `String` | Herb |  | 
| `String` | Hibiscus |  | 
| `String` | High_Brightness |  | 
| `String` | High_Heel |  | 
| `String` | Honey_Pot |  | 
| `String` | Horse |  | 
| `String` | Horse_Racing |  | 
| `String` | Hospital |  | 
| `String` | Hotel |  | 
| `String` | Hotsprings |  | 
| `String` | Hourglass |  | 
| `String` | Hourglass_Flowing_Sand |  | 
| `String` | House |  | 
| `String` | House_With_Garden |  | 
| `String` | Hushed |  | 
| `String` | Ice_Cream |  | 
| `String` | Icecream |  | 
| `String` | Id |  | 
| `String` | Ideograph_Advantage |  | 
| `String` | Imp |  | 
| `String` | Inbox_Tray |  | 
| `String` | Incoming_Envelope |  | 
| `String` | Information_Desk_Person |  | 
| `String` | Information_Source |  | 
| `String` | Innocent |  | 
| `String` | Interrobang |  | 
| `String` | Iphone |  | 
| `String` | It |  | 
| `String` | Izakaya_Lantern |  | 
| `String` | Jack_O_Lantern |  | 
| `String` | Japan |  | 
| `String` | Japanese_Castle |  | 
| `String` | Japanese_Goblin |  | 
| `String` | Japanese_Ogre |  | 
| `String` | Jeans |  | 
| `String` | Joy |  | 
| `String` | Joy_Cat |  | 
| `String` | Jp |  | 
| `String` | Key |  | 
| `String` | Keycap_Ten |  | 
| `String` | Kimono |  | 
| `String` | Kiss |  | 
| `String` | Kissing |  | 
| `String` | Kissing_Cat |  | 
| `String` | Kissing_Closed_Eyes |  | 
| `String` | Kissing_Heart |  | 
| `String` | Kissing_Smiling_Eyes |  | 
| `String` | Knife |  | 
| `String` | Koala |  | 
| `String` | Koko |  | 
| `String` | Kr |  | 
| `String` | Large_Blue_Circle |  | 
| `String` | Large_Blue_Diamond |  | 
| `String` | Large_Orange_Diamond |  | 
| `String` | Last_Quarter_Moon |  | 
| `String` | Last_Quarter_Moon_With_Face |  | 
| `String` | Laughing |  | 
| `String` | Leaves |  | 
| `String` | Ledger |  | 
| `String` | Left_Luggage |  | 
| `String` | Left_Right_Arrow |  | 
| `String` | Leftwards_Arrow_With_Hook |  | 
| `String` | Lemon |  | 
| `String` | Leo |  | 
| `String` | Leopard |  | 
| `String` | Libra |  | 
| `String` | Light_Rail |  | 
| `String` | Link |  | 
| `String` | Lips |  | 
| `String` | Lipstick |  | 
| `String` | Lock |  | 
| `String` | Lock_With_Ink_Pen |  | 
| `String` | Lollipop |  | 
| `String` | Loop |  | 
| `String` | Loud_Sound |  | 
| `String` | Loudspeaker |  | 
| `String` | Love_Hotel |  | 
| `String` | Love_Letter |  | 
| `String` | Low_Brightness |  | 
| `String` | M |  | 
| `String` | Mag |  | 
| `String` | Mag_Right |  | 
| `String` | Mahjong |  | 
| `String` | Mailbox |  | 
| `String` | Mailbox_Closed |  | 
| `String` | Mailbox_With_Mail |  | 
| `String` | Mailbox_With_No_Mail |  | 
| `String` | Man |  | 
| `String` | Man_With_Gua_Pi_Mao |  | 
| `String` | Man_With_Turban |  | 
| `String` | Mans_Shoe |  | 
| `String` | Maple_Leaf |  | 
| `String` | Mask |  | 
| `String` | Massage |  | 
| `String` | Meat_On_Bone |  | 
| `String` | Mega |  | 
| `String` | Melon |  | 
| `String` | Mens |  | 
| `String` | Metro |  | 
| `String` | Microphone |  | 
| `String` | Microscope |  | 
| `String` | Milky_Way |  | 
| `String` | Minibus |  | 
| `String` | Minidisc |  | 
| `String` | Mobile_Phone_Off |  | 
| `String` | Money_With_Wings |  | 
| `String` | Moneybag |  | 
| `String` | Monkey |  | 
| `String` | Monkey_Face |  | 
| `String` | Monorail |  | 
| `String` | Mortar_Board |  | 
| `String` | Mount_Fuji |  | 
| `String` | Mountain_Bicyclist |  | 
| `String` | Mountain_Cableway |  | 
| `String` | Mountain_Railway |  | 
| `String` | Mouse |  | 
| `String` | Mouse2 |  | 
| `String` | Movie_Camera |  | 
| `String` | Moyai |  | 
| `String` | Muscle |  | 
| `String` | Mushroom |  | 
| `String` | Musical_Keyboard |  | 
| `String` | Musical_Note |  | 
| `String` | Musical_Score |  | 
| `String` | Mute |  | 
| `String` | Nail_Care |  | 
| `String` | Name_Badge |  | 
| `String` | Necktie |  | 
| `String` | Negative_Squared_Cross_Mark |  | 
| `String` | Neutral_Face |  | 
| `String` | New |  | 
| `String` | New_Moon |  | 
| `String` | New_Moon_With_Face |  | 
| `String` | Newspaper |  | 
| `String` | Ng |  | 
| `String` | Night_With_Stars |  | 
| `String` | Nine |  | 
| `String` | No_Bell |  | 
| `String` | No_Bicycles |  | 
| `String` | No_Entry |  | 
| `String` | No_Entry_Sign |  | 
| `String` | No_Good |  | 
| `String` | No_Mobile_Phones |  | 
| `String` | No_Mouth |  | 
| `String` | No_Pedestrians |  | 
| `String` | No_Smoking |  | 
| `String` | Non_Potable_Water |  | 
| `String` | Nose |  | 
| `String` | Notebook |  | 
| `String` | Notebook_With_Decorative_Cover |  | 
| `String` | Notes |  | 
| `String` | Nut_And_Bolt |  | 
| `String` | O |  | 
| `String` | O2 |  | 
| `String` | Ocean |  | 
| `String` | Octopus |  | 
| `String` | Oden |  | 
| `String` | Office |  | 
| `String` | Ok |  | 
| `String` | Ok_Hand |  | 
| `String` | Ok_Woman |  | 
| `String` | Older_Man |  | 
| `String` | Older_Woman |  | 
| `String` | On |  | 
| `String` | Oncoming_Automobile |  | 
| `String` | Oncoming_Bus |  | 
| `String` | Oncoming_Police_Car |  | 
| `String` | Oncoming_Taxi |  | 
| `String` | One |  | 
| `String` | Open_File_Folder |  | 
| `String` | Open_Hands |  | 
| `String` | Open_Mouth |  | 
| `String` | Ophiuchus |  | 
| `String` | Orange_Book |  | 
| `String` | Outbox_Tray |  | 
| `String` | Ox |  | 
| `String` | Package |  | 
| `String` | Page_Facing_Up |  | 
| `String` | Page_With_Curl |  | 
| `String` | Pager |  | 
| `String` | Palm_Tree |  | 
| `String` | Panda_Face |  | 
| `String` | Paperclip |  | 
| `String` | Parking |  | 
| `String` | Part_Alternation_Mark |  | 
| `String` | Partly_Sunny |  | 
| `String` | Passport_Control |  | 
| `String` | Peach |  | 
| `String` | Pear |  | 
| `String` | Pencil |  | 
| `String` | Pencil2 |  | 
| `String` | Penguin |  | 
| `String` | Pensive |  | 
| `String` | Performing_Arts |  | 
| `String` | Persevere |  | 
| `String` | Person_Frowning |  | 
| `String` | Person_With_Blond_Hair |  | 
| `String` | Person_With_Pouting_Face |  | 
| `String` | Pig |  | 
| `String` | Pig_Nose |  | 
| `String` | Pig2 |  | 
| `String` | Pill |  | 
| `String` | Pineapple |  | 
| `String` | Pisces |  | 
| `String` | Pizza |  | 
| `String` | Point_Down |  | 
| `String` | Point_Left |  | 
| `String` | Point_Right |  | 
| `String` | Point_Up |  | 
| `String` | Point_Up_2 |  | 
| `String` | Police_Car |  | 
| `String` | Poodle |  | 
| `String` | Poop |  | 
| `String` | Post_Office |  | 
| `String` | Postal_Horn |  | 
| `String` | Postbox |  | 
| `String` | Potable_Water |  | 
| `String` | Pouch |  | 
| `String` | Poultry_Leg |  | 
| `String` | Pound |  | 
| `String` | Pouting_Cat |  | 
| `String` | Pray |  | 
| `String` | Princess |  | 
| `String` | Punch |  | 
| `String` | Purple_Heart |  | 
| `String` | Purse |  | 
| `String` | Pushpin |  | 
| `String` | Put_Litter_In_Its_Place |  | 
| `String` | Question |  | 
| `String` | Rabbit |  | 
| `String` | Rabbit2 |  | 
| `String` | Racehorse |  | 
| `String` | Radio |  | 
| `String` | Radio_Button |  | 
| `String` | Rage |  | 
| `String` | Railway_Car |  | 
| `String` | Rainbow |  | 
| `String` | Raised_Hand |  | 
| `String` | Raised_Hands |  | 
| `String` | Raising_Hand |  | 
| `String` | Ram |  | 
| `String` | Ramen |  | 
| `String` | Rat |  | 
| `String` | Recycle |  | 
| `String` | Red_Car |  | 
| `String` | Red_Circle |  | 
| `String` | Registered |  | 
| `String` | Relaxed |  | 
| `String` | Relieved |  | 
| `String` | Repeat |  | 
| `String` | Repeat_One |  | 
| `String` | Restroom |  | 
| `String` | Revolving_Hearts |  | 
| `String` | Rewind |  | 
| `String` | Ribbon |  | 
| `String` | Rice |  | 
| `String` | Rice_Ball |  | 
| `String` | Rice_Cracker |  | 
| `String` | Rice_Scene |  | 
| `String` | Ring |  | 
| `String` | Robot_Face |  | 
| `String` | Rocket |  | 
| `String` | Roller_Coaster |  | 
| `String` | Rooster |  | 
| `String` | Rose |  | 
| `String` | Rotating_Light |  | 
| `String` | Round_Pushpin |  | 
| `String` | Rowboat |  | 
| `String` | Ru |  | 
| `String` | Rugby_Football |  | 
| `String` | Runner |  | 
| `String` | Running_Shirt_With_Sash |  | 
| `String` | Sa |  | 
| `String` | Sagittarius |  | 
| `String` | Sailboat |  | 
| `String` | Sake |  | 
| `String` | Sandal |  | 
| `String` | Santa |  | 
| `String` | Satellite |  | 
| `String` | Saxophone |  | 
| `String` | School |  | 
| `String` | School_Satchel |  | 
| `String` | Scissors |  | 
| `String` | Scorpius |  | 
| `String` | Scream |  | 
| `String` | Scream_Cat |  | 
| `String` | Scroll |  | 
| `String` | Seat |  | 
| `String` | Secret |  | 
| `String` | See_No_Evil |  | 
| `String` | Seedling |  | 
| `String` | Seven |  | 
| `String` | Shaved_Ice |  | 
| `String` | Sheep |  | 
| `String` | Shell |  | 
| `String` | Ship |  | 
| `String` | Shirt |  | 
| `String` | Shower |  | 
| `String` | Signal_Strength |  | 
| `String` | Six |  | 
| `String` | Six_Pointed_Star |  | 
| `String` | Ski |  | 
| `String` | Skull |  | 
| `String` | Sleeping |  | 
| `String` | Sleepy |  | 
| `String` | Slot_Machine |  | 
| `String` | Small_Blue_Diamond |  | 
| `String` | Small_Orange_Diamond |  | 
| `String` | Small_Red_Triangle |  | 
| `String` | Small_Red_Triangle_Down |  | 
| `String` | Smile |  | 
| `String` | Smile_Cat |  | 
| `String` | Smiley |  | 
| `String` | Smiley_Cat |  | 
| `String` | Smiling_Imp |  | 
| `String` | Smirk |  | 
| `String` | Smirk_Cat |  | 
| `String` | Smoking |  | 
| `String` | Snail |  | 
| `String` | Snake |  | 
| `String` | Snowboarder |  | 
| `String` | Snowflake |  | 
| `String` | Snowman |  | 
| `String` | Sob |  | 
| `String` | Soccer |  | 
| `String` | Soon |  | 
| `String` | Sos |  | 
| `String` | Sound |  | 
| `String` | Space_Invader |  | 
| `String` | Spades |  | 
| `String` | Spaghetti |  | 
| `String` | Sparkle |  | 
| `String` | Sparkler |  | 
| `String` | Sparkles |  | 
| `String` | Sparkling_Heart |  | 
| `String` | Speak_No_Evil |  | 
| `String` | Speaker |  | 
| `String` | Speech_Balloon |  | 
| `String` | Speedboat |  | 
| `String` | Star |  | 
| `String` | Star2 |  | 
| `String` | Stars |  | 
| `String` | Station |  | 
| `String` | Statue_Of_Liberty |  | 
| `String` | Steam_Locomotive |  | 
| `String` | Stew |  | 
| `String` | Straight_Ruler |  | 
| `String` | Strawberry |  | 
| `String` | Stuck_Out_Tongue |  | 
| `String` | Stuck_Out_Tongue_Closed_Eyes |  | 
| `String` | Stuck_Out_Tongue_Winking_Eye |  | 
| `String` | Sun_With_Face |  | 
| `String` | Sunflower |  | 
| `String` | Sunglasses |  | 
| `String` | Sunny |  | 
| `String` | Sunrise |  | 
| `String` | Sunrise_Over_Mountains |  | 
| `String` | Surfer |  | 
| `String` | Sushi |  | 
| `String` | Suspension_Railway |  | 
| `String` | Sweat |  | 
| `String` | Sweat_Drops |  | 
| `String` | Sweat_Smile |  | 
| `String` | Sweet_Potato |  | 
| `String` | Swimmer |  | 
| `String` | Symbols |  | 
| `String` | Syringe |  | 
| `String` | Tada |  | 
| `String` | Tanabata_Tree |  | 
| `String` | Tangerine |  | 
| `String` | Taurus |  | 
| `String` | Taxi |  | 
| `String` | Tea |  | 
| `String` | Telephone |  | 
| `String` | Telephone_Receiver |  | 
| `String` | Telescope |  | 
| `String` | Tennis |  | 
| `String` | Tent |  | 
| `String` | Thought_Balloon |  | 
| `String` | Three |  | 
| `String` | Thumbsdown |  | 
| `String` | Thumbsup |  | 
| `String` | Ticket |  | 
| `String` | Tiger |  | 
| `String` | Tiger2 |  | 
| `String` | Tired_Face |  | 
| `String` | Tm |  | 
| `String` | Toilet |  | 
| `String` | Tokyo_Tower |  | 
| `String` | Tomato |  | 
| `String` | Tongue |  | 
| `String` | Top |  | 
| `String` | Tophat |  | 
| `String` | Tractor |  | 
| `String` | Traffic_Light |  | 
| `String` | Train |  | 
| `String` | Train2 |  | 
| `String` | Tram |  | 
| `String` | Triangular_Flag_On_Post |  | 
| `String` | Triangular_Ruler |  | 
| `String` | Trident |  | 
| `String` | Triumph |  | 
| `String` | Trolleybus |  | 
| `String` | Trophy |  | 
| `String` | Tropical_Drink |  | 
| `String` | Tropical_Fish |  | 
| `String` | Truck |  | 
| `String` | Trumpet |  | 
| `String` | Tulip |  | 
| `String` | Turtle |  | 
| `String` | Tv |  | 
| `String` | Twisted_Rightwards_Arrows |  | 
| `String` | Two |  | 
| `String` | Two_Hearts |  | 
| `String` | Two_Men_Holding_Hands |  | 
| `String` | Two_Women_Holding_Hands |  | 
| `String` | U5272 |  | 
| `String` | U5408 |  | 
| `String` | U55B6 |  | 
| `String` | U6307 |  | 
| `String` | U6708 |  | 
| `String` | U6709 |  | 
| `String` | U6E80 |  | 
| `String` | U7121 |  | 
| `String` | U7533 |  | 
| `String` | U7981 |  | 
| `String` | U7A7A |  | 
| `String` | Uk |  | 
| `String` | Umbrella |  | 
| `String` | Unamused |  | 
| `String` | Underage |  | 
| `String` | Unlock |  | 
| `String` | Up |  | 
| `String` | Us |  | 
| `String` | V |  | 
| `String` | Vertical_Traffic_Light |  | 
| `String` | Vhs |  | 
| `String` | Vibration_Mode |  | 
| `String` | Video_Camera |  | 
| `String` | Video_Game |  | 
| `String` | Violin |  | 
| `String` | Virgo |  | 
| `String` | Volcano |  | 
| `String` | Vs |  | 
| `String` | Walking |  | 
| `String` | Waning_Crescent_Moon |  | 
| `String` | Waning_Gibbous_Moon |  | 
| `String` | Warning |  | 
| `String` | Watch |  | 
| `String` | Water_Buffalo |  | 
| `String` | Watermelon |  | 
| `String` | Wave |  | 
| `String` | Wavy_Dash |  | 
| `String` | Waxing_Crescent_Moon |  | 
| `String` | Waxing_Gibbous_Moon |  | 
| `String` | Wc |  | 
| `String` | Weary |  | 
| `String` | Wedding |  | 
| `String` | Whale |  | 
| `String` | Whale2 |  | 
| `String` | Wheelchair |  | 
| `String` | White_Check_Mark |  | 
| `String` | White_Circle |  | 
| `String` | White_Flower |  | 
| `String` | White_Large_Square |  | 
| `String` | White_Medium_Small_Square |  | 
| `String` | White_Medium_Square |  | 
| `String` | White_Small_Square |  | 
| `String` | White_Square_Button |  | 
| `String` | Wind_Chime |  | 
| `String` | Wine_Glass |  | 
| `String` | Wink |  | 
| `String` | Wolf |  | 
| `String` | Woman |  | 
| `String` | Womans_Clothes |  | 
| `String` | Womans_Hat |  | 
| `String` | Womens |  | 
| `String` | Worried |  | 
| `String` | Wrench |  | 
| `String` | X |  | 
| `String` | Yellow_Heart |  | 
| `String` | Yen |  | 
| `String` | Yum |  | 
| `String` | Zap |  | 
| `String` | Zero |  | 
| `String` | Zzz |  | 


## `Encryption`

```csharp
public static class InnerLibs.Encryption

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Decrypt(this `String` Text, `String` Key = null) |  | 
| `String` | Decrypt(this `String` text, `String` Key, `String` IV) |  | 
| `FileInfo` | DecryptRSA(this `FileInfo` File, `String` Key) |  | 
| `Byte[]` | DecryptRSA(this `Byte[]` bytes, `String` Key) |  | 
| `String` | Encrypt(this `String` Text, `String` Key = null) |  | 
| `String` | Encrypt(this `String` text, `String` Key, `String` IV) |  | 
| `String` | EncryptRSA(this `String` Text, `String` Key) |  | 
| `Byte[]` | EncryptRSA(this `Byte[]` bytes, `String` Key) |  | 
| `FileInfo` | EncryptRSA(this `FileInfo` File, `String` Key) |  | 
| `String` | ToMD5String(this `String` Text) |  | 


## `EquationPair<T>`

```csharp
public class InnerLibs.EquationPair<T>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | IsComplete |  | 
| `Boolean` | IsNotComplete |  | 
| `Boolean` | MissX |  | 
| `Boolean` | MissY |  | 
| `Nullable<T>` | X |  | 
| `Nullable<T>` | Y |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `PropertyInfo` | GetMissing() |  | 
| `void` | SetMissing(`T` value) |  | 
| `Nullable`1[]` | ToArray() |  | 


## `Files`

```csharp
public static class InnerLibs.Files

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | FormatPath(this `Nullable<DateTime>` DateAndTime, `String` FilePath, `Boolean` AlternativeChar = False) |  | 
| `String` | GetLatestDirectoryName(this `FileInfo` Path) |  | 
| `String` | ReadAllText(this `FileInfo` File, `Encoding` encoding = null) |  | 
| `FileInfo` | SaveMailAttachment(this `Attachment` attachment, `DirectoryInfo` Directory, `Nullable<DateTime>` DateAndTime = null) |  | 
| `FileInfo` | SaveMailAttachment(this `Attachment` attachment, `String` FilePath, `Nullable<DateTime>` DateAndTime = null) |  | 
| `Byte[]` | ToBytes(this `Attachment` attachment) |  | 
| `Byte[]` | ToBytes(this `Stream` stream) |  | 
| `Byte[]` | ToBytes(this `FileInfo` File) |  | 
| `FileInfo` | WriteToFile(this `Stream` Stream, `String` FilePath, `Nullable<DateTime>` DateAndTime = null) |  | 
| `FileInfo` | WriteToFile(this `Byte[]` Bytes, `String` FilePath, `Nullable<DateTime>` DateAndTime = null) |  | 
| `FileInfo` | WriteToFile(this `String` Text, `String` FilePath, `Boolean` Append = False, `Encoding` Enconding = null, `Nullable<DateTime>` DateAndTime = null) |  | 
| `FileInfo` | WriteToFile(this `String` Text, `FileInfo` File, `Boolean` Append = False, `Encoding` Enconding = null, `Nullable<DateTime>` DateAndTime = null) |  | 


## `FileTree`

```csharp
public class InnerLibs.FileTree

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<FileTree>` | Children |  | 
| `DateTime` | CreationTime |  | 
| `String` | Extension |  | 
| `Boolean` | IsDirectory |  | 
| `Boolean` | IsFile |  | 
| `DateTime` | LastAccessTime |  | 
| `DateTime` | LastWriteTime |  | 
| `String` | Name |  | 
| `FileTree` | Parent |  | 
| `String` | Path |  | 
| `String` | Title |  | 
| `String` | TypeDescription |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FileType` | GetFileType() |  | 
| `Bitmap` | GetIcon() |  | 
| `String` | ToString() |  | 


## `FileType`

```csharp
public class InnerLibs.FileType

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Description |  | 
| `List<String>` | Extensions |  | 
| `List<String>` | MimeTypes |  | 
| `IEnumerable<String>` | SubTypes |  | 
| `IEnumerable<String>` | Types |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | GetMimeTypesOrDefault() |  | 
| `Boolean` | IsApplication() |  | 
| `Boolean` | IsAudio() |  | 
| `Boolean` | IsFont() |  | 
| `Boolean` | IsImage() |  | 
| `Boolean` | IsMessage() |  | 
| `Boolean` | IsText() |  | 
| `Boolean` | IsType(`String` Type) |  | 
| `Boolean` | IsVideo() |  | 
| `IEnumerable<FileInfo>` | SearchFiles(`DirectoryInfo` Directory, `SearchOption` SearchOption = AllDirectories) |  | 
| `String` | ToFilterString() |  | 
| `String` | ToString() |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | GetExtensions(`String` MIME, `FileTypeList` FileTypeList = null) |  | 
| `FileTypeList` | GetFileType(`IEnumerable<String>` MimeTypeOrExtensionOrPathOrDataURI, `FileTypeList` FileTypeList = null) |  | 
| `FileType` | GetFileType(`String` MimeTypeOrExtensionOrPathOrDataURI, `FileTypeList` FileTypeList = null) |  | 
| `FileTypeList` | GetFileTypeList(`Boolean` Reset = False) |  | 
| `IEnumerable<String>` | GetFileTypeStringList(`FileTypeList` FileTypeList = null) |  | 


## `FileTypeExtensions`

```csharp
public static class InnerLibs.FileTypeExtensions

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | GetFileType(this `String` Extension) |  | 
| `IEnumerable<String>` | GetFileType(this `FileInfo` File) |  | 
| `IEnumerable<String>` | GetFileType(this `ImageFormat` RawFormat) |  | 
| `IEnumerable<String>` | GetFileType(this `Image` Image) |  | 
| `Icon` | GetIcon(this `FileSystemInfo` File) |  | 
| `FileType` | ToFileType(this `String` MimeTypeOrExtensionOrPathOrDataURI) |  | 


## `FileTypeList`

```csharp
public class InnerLibs.FileTypeList
    : List<FileType>, IList<FileType>, ICollection<FileType>, IEnumerable<FileType>, IEnumerable, IList, ICollection, IReadOnlyList<FileType>, IReadOnlyCollection<FileType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | Descriptions |  | 
| `IEnumerable<String>` | Extensions |  | 
| `IEnumerable<String>` | FirstTypes |  | 
| `IEnumerable<String>` | MimeTypes |  | 
| `IEnumerable<String>` | SubTypes |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<FileInfo>` | SearchFiles(`DirectoryInfo` Directory, `SearchOption` SearchOption = AllDirectories) |  | 
| `String` | ToFilterString() |  | 


## `FullMoneyWriter`

```csharp
public class InnerLibs.FullMoneyWriter
    : FullNumberWriter

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `QuantityTextPair` | CurrencyCentsName |  | 
| `QuantityTextPair` | CurrencyName |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString(`Decimal` Number, `Int32` DecimalPlaces = 2) |  | 
| `String` | ToString() |  | 


## `FullNumberWriter`

```csharp
public class InnerLibs.FullNumberWriter

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | And |  | 
| `QuantityTextPair` | Billion |  | 
| `String` | DecimalSeparator |  | 
| `String` | Eight |  | 
| `String` | Eighteen |  | 
| `String` | EightHundred |  | 
| `String` | Eighty |  | 
| `String` | Eleven |  | 
| `String` | ExactlyOneHundred |  | 
| `String` | Fifteen |  | 
| `String` | Fifty |  | 
| `String` | Five |  | 
| `String` | FiveHundred |  | 
| `String` | Four |  | 
| `String` | FourHundred |  | 
| `String` | Fourteen |  | 
| `String` | Fourty |  | 
| `String` | Item |  | 
| `QuantityTextPair` | Million |  | 
| `String` | Minus |  | 
| `String` | MoreThan |  | 
| `String` | Nine |  | 
| `String` | NineHundred |  | 
| `String` | Nineteen |  | 
| `String` | Ninety |  | 
| `String` | One |  | 
| `String` | OneHundred |  | 
| `QuantityTextPair` | Quadrillion |  | 
| `QuantityTextPair` | Quintillion |  | 
| `String` | Seven |  | 
| `String` | SevenHundred |  | 
| `String` | Seventeen |  | 
| `String` | Seventy |  | 
| `String` | Six |  | 
| `String` | SixHundred |  | 
| `String` | Sixteen |  | 
| `String` | Sixty |  | 
| `String` | Ten |  | 
| `String` | Thirteen |  | 
| `String` | Thirty |  | 
| `String` | Thousand |  | 
| `String` | Three |  | 
| `String` | ThreeHundred |  | 
| `QuantityTextPair` | Trillion |  | 
| `String` | Twelve |  | 
| `String` | Twenty |  | 
| `String` | Two |  | 
| `String` | TwoHundred |  | 
| `String` | Zero |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `String` | ToString(`Decimal` Number, `Int32` DecimalPlaces = 2) |  | 


## `Generate`

```csharp
public static class InnerLibs.Generate

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | BarcodeCheckSum(`Int64` Code) |  | 
| `String` | BarcodeCheckSum(`Int32` Code) |  | 
| `String` | BarcodeCheckSum(`String` Code) |  | 
| `String` | EAN(`Int32` ContryCode, `Int32` ManufacturerCode, `Int32` ProductCode) |  | 
| `String` | EANFromNumbers(`String[]` Numbers) |  | 
| `String` | EANFromNumbers(`Int32[]` Numbers) |  | 
| `String` | Password(`Int32` AlphaLenght, `Int32` NumberLenght, `Int32` SpecialLenght) |  | 
| `String` | Password(`Int32` AlphaUpperLenght, `Int32` AlphaLowerLenght, `Int32` NumberLenght, `Int32` SpecialLenght) |  | 
| `String` | Password(`Int32` Lenght = 8) |  | 
| `Boolean` | RandomBool(`Int32` Percent) |  | 
| `Boolean` | RandomBool(`Func<Int32, Boolean>` Condition, `Int32` Min = 0, `Int32` Max = 2147483647) |  | 
| `Boolean` | RandomBool() |  | 
| `List<Color>` | RandomColorList(`Int32` Quantity, `Int32` Red = -1, `Int32` Green = -1, `Int32` Blue = -1) |  | 
| `DateTime` | RandomDateTime(`Nullable<Int32>` Year = null, `Nullable<Int32>` Month = null, `Nullable<Int32>` Day = null, `Nullable<Int32>` Hour = null, `Nullable<Int32>` Minute = null, `Nullable<Int32>` Second = null) |  | 
| `DateTime` | RandomDateTime(`Nullable<DateTime>` MinDate, `Nullable<DateTime>` MaxDate = null) |  | 
| `String` | RandomEAN(`Int32` Len) |  | 
| `String` | RandomFixLenghtNumber(`Int32` Len = 8) |  | 
| `TextStructure` | RandomIpsum(`Int32` ParagraphCount = 5, `Int32` SentenceCount = 3, `Int32` MinWordCount = 10, `Int32` MaxWordCount = 50, `Int32` IdentSize = 0, `Int32` BreakLinesBetweenParagraph = 0) |  | 
| `Int32` | RandomNumber(`Int32` Min = 0, `Int32` Max = 2147483647) |  | 
| `Int64` | RandomNumber(`Int64` Min, `Int64` Max = 9223372036854775807) |  | 
| `IEnumerable<Int32>` | RandomNumberList(`Int32` Count, `Int32` Min = 0, `Int32` Max = 2147483647, `Boolean` UniqueNumbers = True) |  | 
| `String` | RandomWord(`Int32` MinLength, `Int32` MaxLenght) |  | 
| `String` | RandomWord(`Int32` Length = 0) |  | 
| `Uri` | ToGoogleMapsURL(this `AddressInfo` local, `Boolean` LatLong = False) |  | 


## `HSVColor`

```csharp
public class InnerLibs.HSVColor
    : IComparable<Int32>, IComparable<HSVColor>, IComparable<Color>, IComparable, ICloneable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Byte` | Alpha |  | 
| `Int32` | ARGB |  | 
| `Int32` | Blue |  | 
| `Double` | Brightness |  | 
| `String` | ClosestColorName |  | 
| `String` | CSS |  | 
| `String` | Description |  | 
| `Int32` | DominantValue |  | 
| `Int32` | Green |  | 
| `String` | Hexadecimal |  | 
| `Double` | Hue |  | 
| `Bitmap` | ImageSample |  | 
| `Double` | Luminance |  | 
| `ColorMood` | Mood |  | 
| `String` | Name |  | 
| `Decimal` | Opacity |  | 
| `Int32` | Red |  | 
| `Double` | Saturation |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HSVColor` | Addictive(`HSVColor` Color) |  | 
| `HSVColor[]` | Analogous(`Boolean` ExcludeMe = False) |  | 
| `HSVColor` | Average(`HSVColor` Color) |  | 
| `HSVColor` | BluePart() |  | 
| `Object` | Clone() |  | 
| `HSVColor` | Combine(`HSVColor` Color) |  | 
| `Int32` | CompareTo(`Int32` other) |  | 
| `Int32` | CompareTo(`HSVColor` other) |  | 
| `Int32` | CompareTo(`Color` other) |  | 
| `Int32` | CompareTo(`Object` obj) |  | 
| `HSVColor[]` | Complementary(`Boolean` ExcludeMe = False) |  | 
| `HSVColor[]` | ComplementaryPallete(`Int32` Amount = 3) |  | 
| `HSVColor` | ContrastColor() |  | 
| `HSVColor` | CreateCopy() |  | 
| `HSVColor[]` | CreatePallete(`String` PalleteType, `Int32` Amount = 4) |  | 
| `Bitmap` | CreateSolidImage(`Int32` Width, `Int32` Height) |  | 
| `Bitmap` | CreateSolidImage(`String` Size = ) |  | 
| `HSVColor` | Difference(`HSVColor` Color) |  | 
| `Double` | Distance(`HSVColor` Color) |  | 
| `Boolean` | Equals(`Object` obj) |  | 
| `HSVColor` | GetDominantColor() |  | 
| `Double` | GetEuclideanDistance(`HSVColor` Color) |  | 
| `Int32` | GetHashCode() |  | 
| `HSVColor` | GreenPart() |  | 
| `HSVColor` | Grey() |  | 
| `Boolean` | HasAnyMood(`ColorMood[]` Mood) |  | 
| `Boolean` | HasMood(`ColorMood[]` Mood) |  | 
| `Boolean` | IsCool() |  | 
| `Boolean` | IsCooler() |  | 
| `Boolean` | IsDark() |  | 
| `Boolean` | IsLight() |  | 
| `Boolean` | IsMedium() |  | 
| `Boolean` | IsMediumDark() |  | 
| `Boolean` | IsMediumLight() |  | 
| `Boolean` | IsReadable(`HSVColor` BackgroundColor, `Int32` Size = 10) |  | 
| `Boolean` | IsSad() |  | 
| `Boolean` | IssHappy() |  | 
| `Boolean` | IsWarm() |  | 
| `Boolean` | IsWarmer() |  | 
| `HSVColor` | MakeDarker(`Single` Percent = 50) |  | 
| `HSVColor` | MakeLighter(`Single` Percent = 50) |  | 
| `HSVColor[]` | ModColor(`Boolean` ExcludeMe, `Int32[]` Degrees) |  | 
| `HSVColor[]` | ModColor(`Int32[]` Degrees) |  | 
| `HSVColor[]` | Monochromatic(`Decimal` Amount = 4) |  | 
| `HSVColor` | Multiply(`HSVColor` Color) |  | 
| `HSVColor` | Negative() |  | 
| `Boolean` | NotHasMood(`ColorMood[]` Mood) |  | 
| `HSVColor` | RedPart() |  | 
| `HSVColor` | Sepia() |  | 
| `HSVColor[]` | SplitComplementary(`Boolean` IncludeMe = False) |  | 
| `HSVColor[]` | SplitComplementaryPallete(`Int32` Amount = 3) |  | 
| `HSVColor[]` | Square(`Boolean` ExcludeMe = False) |  | 
| `HSVColor` | Subtractive(`HSVColor` Color) |  | 
| `HSVColor[]` | Tetradic(`Boolean` ExcludeMe = False) |  | 
| `HSVColor[]` | TetradicPallete(`Int32` Amount = 3) |  | 
| `Color` | ToDrawingColor() |  | 
| `String` | ToString() |  | 
| `HSVColor[]` | Triadic(`Boolean` ExcludeMe = False) |  | 
| `HSVColor[]` | TriadicPallete(`Int32` Amount = 3) |  | 


Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<HSVColor>` | NamedColors |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<HSVColor>` | CreateColors(`String[]` Colors) |  | 
| `HSVColor` | FromImage(`Image` Img, `Int32` Reduce = 16) |  | 
| `HSVColor` | RandomColor(`Image` Img, `Int32` Reduce = 16) |  | 
| `HSVColor` | RandomColor(`IEnumerable<Color>` Colors) |  | 
| `HSVColor` | RandomColor(`String` Name = null) |  | 
| `HSVColor` | RandomColor(`ColorMood` Mood) |  | 
| `HSVColor` | RandomColor(`Expression<Func<HSVColor, Boolean>>` predicate) |  | 
| `IEnumerable<HSVColor>` | RandomColorList(`Int32` Quantity, `ColorMood` Mood) |  | 
| `IEnumerable<HSVColor>` | RandomColorList(`Int32` Quantity, `Expression<Func<HSVColor, Boolean>>` predicate) |  | 
| `HSVColor` | RandomTransparentColor(`String` Name = null) |  | 


## `HtmlTag`

```csharp
public class InnerLibs.HtmlTag

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dictionary<String, String>` | Attributes |  | 
| `String` | Class |  | 
| `String[]` | ClassList |  | 
| `String` | InnerHtml |  | 
| `String` | InnerText |  | 
| `String` | Item |  | 
| `Boolean` | SelfCloseTag |  | 
| `String` | TagName |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlTag` | AddClass(`String` ClassName) |  | 
| `Boolean` | HasAttribute(`String` AttrName) |  | 
| `HtmlTag` | RemoveAttr(`String` AttrName) |  | 
| `HtmlTag` | RemoveClass(`String` ClassName) |  | 
| `HtmlTag` | SetAttr(`String` AttrName, `String` Value, `Boolean` RemoveIfBlank = False) |  | 
| `HtmlTag` | SetInnerHtml(`String` Html) |  | 
| `HtmlTag` | SetInnerText(`String` Text) |  | 
| `HtmlTag` | SetProp(`String` AttrName, `Boolean` Value = True) |  | 
| `String` | ToString() |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlTag` | CreateAnchor(`String` URL, `String` Text, `String` Target = _self, `Object` htmlAttributes = null) |  | 
| `HtmlTag` | CreateImage(`String` URL, `Object` htmlAttributes = null) |  | 
| `HtmlTag` | CreateInput(`String` Name, `String` Value = null, `String` Type = text, `Object` htmlAttributes = null) |  | 
| `HtmlTag` | CreateOption(`String` Name, `String` Value = null, `Boolean` Selected = False) |  | 
| `HtmlTag` | CreateTable(`String[,]` Table, `Boolean` Header = False) |  | 
| `HtmlTag` | CreateTable(`IEnumerable<TPoco>` Rows, `Boolean` header, `String` IDProperty, `String[]` Properties) |  | 
| `HtmlTag` | CreateTable(`IEnumerable<TPoco>` Rows) |  | 
| `HtmlTag` | CreateTable(`IEnumerable<TPoco>` Rows, `TPoco` header, `String` IDProperty, `String[]` properties) |  | 


## `Images`

```csharp
public static class InnerLibs.Images

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `ImageFormat[]` | ImageTypes |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Image` | Blur(this `Image` Img, `Int32` BlurSize = 5) |  | 
| `Image` | BrightnessContrastAndGamma(this `Image` originalimage, `Single` Brightness, `Single` Contrast, `Single` Gamma) |  | 
| `Dictionary<HSVColor, Int32>` | ColorPallette(this `Image` Img, `Int32` PixelateSize = 0) |  | 
| `Bitmap` | CombineImages(`Boolean` VerticalFlow, `Image[]` Images) |  | 
| `Bitmap` | CombineImages(this `IEnumerable<Image>` Images, `Boolean` VerticalFlow = False) |  | 
| `Boolean` | CompareARGB(this `Color` Color1, `Color` Color2, `Boolean` IgnoreAlpha = True) |  | 
| `Boolean` | CompareARGB(this `Color` Color1, `Boolean` IgnoreAlpha, `Color[]` Colors) |  | 
| `Image` | CreateSolidImage(this `Color` Color, `Int32` Width, `Int32` Height) |  | 
| `Image` | Crop(this `Image` Image, `String` SizeExpression) |  | 
| `Image` | Crop(this `Image` Image, `Size` Size) |  | 
| `Image` | Crop(this `Image` Image, `Int32` MaxWidth, `Int32` MaxHeight) |  | 
| `Image` | CropToCircle(this `Image` Img, `Nullable<Color>` Background = null) |  | 
| `Image` | CropToEllipsis(this `Image` Img, `Nullable<Color>` Background = null) |  | 
| `Image` | CropToSquare(this `Image` Img, `Int32` WidthHeight = 0) |  | 
| `Image` | DrawString(this `Image` img, `String` Text, `Font` Font = null, `Nullable<Color>` Color = null, `Int32` X = -1, `Int32` Y = -1) |  | 
| `ImageCodecInfo` | GetEncoderInfo(this `ImageFormat` RawFormat) |  | 
| `ImageFormat` | GetImageFormat(this `Image` OriginalImage) |  | 
| `IEnumerable<HSVColor>` | GetMostUsedColors(this `Image` Image, `Int32` Count) |  | 
| `IEnumerable<HSVColor>` | GetMostUsedColors(this `Image` Image) |  | 
| `RotateFlipType` | GetRotateFlip(this `Image` Img) |  | 
| `Image` | Grayscale(this `Image` img) |  | 
| `Image` | Insert(this `Image` Image, `Image` InsertedImage, `Int32` X = -1, `Int32` Y = -1) |  | 
| `Image` | MakeDarker(this `Image` img, `Single` percent = 50) |  | 
| `Image` | MakeLighter(this `Image` img, `Single` percent = 50) |  | 
| `Image` | Monochrome(this `Image` Image, `Color` Color, `Single` Alpha = 0) |  | 
| `Image` | Negative(this `Image` img) |  | 
| `Size` | ParseSize(this `String` Text) |  | 
| `Image` | Pixelate(this `Image` Image, `Int32` PixelateSize = 1) |  | 
| `Image` | Resize(this `Image` Original, `String` ResizeExpression, `Boolean` OnlyResizeIfWider = True) |  | 
| `Image` | Resize(this `Image` Original, `Size` Size, `Boolean` OnlyResizeIfWider = True) |  | 
| `Image` | Resize(this `Image` Original, `Int32` NewWidth, `Int32` MaxHeight, `Boolean` OnlyResizeIfWider = True) |  | 
| `Image` | ResizeCrop(this `Image` Image, `Int32` Width, `Int32` Height) |  | 
| `Image` | ResizeCrop(this `Image` Image, `Int32` Width, `Int32` Height, `Boolean` OnlyResizeIfWider) |  | 
| `Image` | ResizePercent(this `Image` Original, `String` Percent, `Boolean` OnlyResizeIfWider = True) |  | 
| `Image` | ResizePercent(this `Image` Original, `Decimal` Percent, `Boolean` OnlyResizeIfWider = True) |  | 
| `Boolean` | TestAndRotate(this `Image` Img) |  | 
| `Bitmap` | ToBitmap(this `Image` Image) |  | 
| `Byte[]` | ToBytes(this `Image` Image, `ImageFormat` Format = null) |  | 
| `Stream` | ToStream(this `Image` Image, `ImageFormat` Format = null) |  | 
| `Image` | Translate(this `Image` img, `Color` Color, `Single` Alpha = 0) |  | 
| `Image` | Translate(this `Image` img, `Single` Red, `Single` Green, `Single` Blue, `Single` Alpha = 0) |  | 
| `Image` | Trim(this `Image` Img, `Nullable<Color>` Color = null) |  | 
| `Image` | Watermark(this `Image` Image, `Image` WaterMarkImage, `Int32` X = -1, `Int32` Y = -1) |  | 


## `MathExt`

```csharp
public static class InnerLibs.MathExt

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<Int32>` | ArithmeticProgression(this `Int32` FirstNumber, `Int32` Constant, `Int32` Length) |  | 
| `Decimal` | Average(`Decimal[]` Values) |  | 
| `Double` | Average(`Double[]` Values) |  | 
| `Double` | Average(`Int32[]` Values) |  | 
| `Double` | Average(`Int64[]` Values) |  | 
| `IEnumerable<Int32>` | ByteSequence(this `Int32` Length) |  | 
| `Double` | CalculateCompoundInterest(this `Double` Capital, `Double` Rate, `Double` Time) |  | 
| `Decimal` | CalculateCompoundInterest(this `Decimal` Capital, `Decimal` Rate, `Decimal` Time) |  | 
| `Double` | CalculateDistance(this `AddressInfo` FirstLocation, `AddressInfo` SecondLocation) |  | 
| `Tuple<AddressInfo, AddressInfo, Decimal>` | CalculateDistanceMatrix(`AddressInfo[]` Locations) |  | 
| `Dictionary<TKey, Decimal>` | CalculatePercent(this `Dictionary<TKey, TValue>` Dic) |  | 
| `Dictionary<TKey, Decimal>` | CalculatePercent(this `IEnumerable<TObject>` Obj, `Expression<Func<TObject, TKey>>` KeySelector, `Expression<Func<TObject, TValue>>` ValueSelector) |  | 
| `Dictionary<Tobject, Decimal>` | CalculatePercent(this `IEnumerable<Tobject>` Obj, `Expression<Func<Tobject, Tvalue>>` ValueSelector) |  | 
| `Dictionary<TValue, Decimal>` | CalculatePercent(this `IEnumerable<TValue>` Obj) |  | 
| `Decimal` | CalculatePercent(this `Decimal` Value, `Decimal` Total) |  | 
| `Decimal` | CalculatePercent(this `Decimal` Value, `Decimal` Total, `Int32` DecimalPlaces) |  | 
| `Decimal` | CalculatePercentCompletion(this `IEnumerable<TValue>` Obj, `Expression<Func<TValue, Boolean>>` selector) |  | 
| `Decimal` | CalculatePercentVariation(this `Decimal` StartValue, `Decimal` EndValue) |  | 
| `Decimal` | CalculatePercentVariation(this `Int32` StartValue, `Int32` EndValue) |  | 
| `Decimal` | CalculatePercentVariation(this `Int64` StartValue, `Int64` EndValue) |  | 
| `Decimal` | CalculateSimpleInterest(this `Decimal` Capital, `Decimal` Rate, `Decimal` Time) |  | 
| `Decimal` | CalculateValueFromPercent(this `String` Percent, `Decimal` Total) |  | 
| `Decimal` | CalculateValueFromPercent(this `Int32` Percent, `Decimal` Total) |  | 
| `Decimal` | CalculateValueFromPercent(this `Decimal` Percent, `Decimal` Total) |  | 
| `IEnumerable<IEnumerable<T>>` | CartesianProduct(`IEnumerable`1[]` Sets) |  | 
| `Decimal` | Ceil(this `Decimal` Number) |  | 
| `Double` | Ceil(this `Double` Number) |  | 
| `Int32` | CeilInt(this `Double` Number) |  | 
| `Int32` | CeilInt(this `Decimal` Number) |  | 
| `Int64` | CeilLong(this `Double` Number) |  | 
| `Int64` | CeilLong(this `Decimal` Number) |  | 
| `IEnumerable<Decimal>` | CollatzConjecture(this `Int32` n) |  | 
| `Int32` | DifferenceIfMax(this `Int32` Total, `Int32` MaxValue) |  | 
| `Int32` | DifferenceIfMin(this `Int32` Total, `Int32` MinValue) |  | 
| `Int32` | Factorial(this `Int32` Number) |  | 
| `IEnumerable<Int32>` | Fibonacci(this `Int32` Length) |  | 
| `Decimal` | Floor(this `Decimal` Number) |  | 
| `Double` | Floor(this `Double` Number) |  | 
| `Int32` | FloorInt(this `Double` Number) |  | 
| `Int32` | FloorInt(this `Decimal` Number) |  | 
| `Int64` | FloorLong(this `Double` Number) |  | 
| `Int64` | FloorLong(this `Decimal` Number) |  | 
| `Decimal` | ForceNegative(this `Decimal` Value) |  | 
| `Int32` | ForceNegative(this `Int32` Value) |  | 
| `Int64` | ForceNegative(this `Int64` Value) |  | 
| `Double` | ForceNegative(this `Double` Value) |  | 
| `Single` | ForceNegative(this `Single` Value) |  | 
| `Int16` | ForceNegative(this `Int16` Value) |  | 
| `Decimal` | ForcePositive(this `Decimal` Value) |  | 
| `Int32` | ForcePositive(this `Int32` Value) |  | 
| `Int64` | ForcePositive(this `Int64` Value) |  | 
| `Double` | ForcePositive(this `Double` Value) |  | 
| `Single` | ForcePositive(this `Single` Value) |  | 
| `Int16` | ForcePositive(this `Int16` Value) |  | 
| `IEnumerable<Int32>` | GeometricProgression(this `Int32` FirstNumber, `Int32` Constant, `Int32` Length) |  | 
| `Int64` | GetDecimalPart(this `Decimal` Value, `Int32` Length = 0) |  | 
| `String` | GetOrdinal(this `Int32` Number) |  | 
| `String` | GetOrdinal(this `Decimal` Number) |  | 
| `String` | GetOrdinal(this `Int16` Number) |  | 
| `String` | GetOrdinal(this `Double` Number) |  | 
| `String` | GetOrdinal(this `Int64` Number) |  | 
| `Boolean` | HasDecimalPart(this `Decimal` Value) |  | 
| `Boolean` | HasDecimalPart(this `Double` Value) |  | 
| `Boolean` | IsWholeNumber(this `Decimal` Number) |  | 
| `Boolean` | IsWholeNumber(this `Double` Number) |  | 
| `Single` | Lerp(this `Single` Start, `Single` End, `Single` Amount) |  | 
| `Int32` | LimitIndex(this `Int32` Int, `IEnumerable<AnyType>` Collection) |  | 
| `Int64` | LimitIndex(this `Int64` Lng, `IEnumerable<AnyType>` Collection) |  | 
| `T` | LimitRange(this `IComparable` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `String` | LimitRange(this `String` Number, `String` MinValue = null, `String` MaxValue = null) |  | 
| `Char` | LimitRange(this `Char` Number, `Nullable<Char>` MinValue = null, `Nullable<Char>` MaxValue = null) |  | 
| `Single` | LimitRange(this `Single` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `Int32` | LimitRange(this `Int32` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `Decimal` | LimitRange(this `Decimal` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `Int64` | LimitRange(this `Double` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `Int64` | LimitRange(this `Int64` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `DateTime` | LimitRange(this `DateTime` Number, `IComparable` MinValue = null, `IComparable` MaxValue = null) |  | 
| `Decimal` | RoundDecimal(this `Decimal` Number, `Nullable<Int32>` Decimals = null) |  | 
| `Double` | RoundDouble(this `Double` Number, `Nullable<Int32>` Decimals = null) |  | 
| `Int32` | RoundInt(this `Decimal` Number) |  | 
| `Int32` | RoundInt(this `Double` Number) |  | 
| `Int64` | RoundLong(this `Decimal` Number) |  | 
| `Int64` | RoundLong(this `Double` Number) |  | 
| `T` | SetMaxValue(this `T` Number, `T` MaxValue) |  | 
| `T` | SetMinValue(this `T` Number, `T` MinValue) |  | 
| `Double` | Sum(`Double[]` Values) |  | 
| `Int64` | Sum(`Int64[]` Values) |  | 
| `Int32` | Sum(`Int32[]` Values) |  | 
| `Decimal` | Sum(`Decimal[]` Values) |  | 
| `String` | ToOrdinalNumber(this `Int32` Number) |  | 
| `String` | ToOrdinalNumber(this `Int64` Number) |  | 
| `String` | ToOrdinalNumber(this `Int16` Number) |  | 
| `String` | ToOrdinalNumber(this `Double` Number) |  | 
| `String` | ToOrdinalNumber(this `Decimal` Number) |  | 
| `Double` | ToRadians(this `Double` Degrees) |  | 


## `Misc`

```csharp
public static class InnerLibs.Misc

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `R` | AsIf(this `T` obj, `Expression<Func<T, Boolean>>` BoolExp, `R` TrueValue, `R` FalseValue = null) |  | 
| `T` | AsIf(this `Boolean` Bool, `T` TrueValue, `T` FalseValue = null) |  | 
| `T` | AsIf(this `Nullable<Boolean>` Bool, `T` TrueValue, `T` FalseValue = null) |  | 
| `T` | AsIf(this `Nullable<Boolean>` Bool, `T` TrueValue, `T` FalseValue, `T` NullValue) |  | 
| `String` | BlankCoalesce(this `String` First, `String[]` N) |  | 
| `String` | BlankCoalesce(`String[]` N) |  | 
| `Boolean` | ContainsAll(this `IEnumerable<T>` List1, `IEnumerable<T>` List2, `IEqualityComparer<T>` Comparer = null) |  | 
| `Boolean` | ContainsAll(this `IEnumerable<T>` List1, `IEqualityComparer<T>` Comparer, `T[]` List2) |  | 
| `Boolean` | ContainsAny(this `IEnumerable<T>` List1, `IEnumerable<T>` List2, `IEqualityComparer<T>` Comparer = null) |  | 
| `Dictionary<String, Object>` | CreateDictionary(this `Type` Obj, `String[]` Keys) |  | 
| `IEnumerable<Dictionary<String, Object>>` | CreateDictionaryEnumerable(this `IEnumerable<Type>` Obj) |  | 
| `Guid` | CreateGuidOrDefault(this `String` Source) |  | 
| `T` | CreateObjectFromXML(this `String` XML) |  | 
| `T` | CreateObjectFromXMLFile(this `FileInfo` XML) |  | 
| `XmlDocument` | CreateXML(this `T` obj) |  | 
| `FileInfo` | CreateXmlFile(this `Object` obj, `String` FilePath) |  | 
| `T` | Detach(this `List<T>` List, `Int32` Index) |  | 
| `IEnumerable<T>` | DetachMany(this `List<T>` List, `Int32[]` Indexes) |  | 
| `Dictionary<Type, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr) |  | 
| `Dictionary<PropT, Int64>` | DistinctCount(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop) |  | 
| `Dictionary<Type, Int64>` | DistinctCountTop(this `IEnumerable<Type>` Arr, `Int32` Top, `Type` Others) |  | 
| `Dictionary<PropT, Int64>` | DistinctCountTop(this `IEnumerable<Type>` Arr, `Func<Type, PropT>` Prop, `Int32` Top, `PropT` Others) |  | 
| `T` | FirstAny(this `IEnumerable<T>` source, `Expression`1[]` predicate) |  | 
| `T` | FirstAnyOr(this `IEnumerable<T>` source, `T` Alternate, `Expression`1[]` predicate) |  | 
| `ValueTuple<T, T>` | FixOrder(`T&` FirstValue, `T&` SecondValue) |  | 
| `ValueTuple<T, T>` | FixOrderNotNull(`T&` FirstValue, `T&` SecondValue) |  | 
| `TValue` | GetAttributeValue(this `MemberInfo` prop, `Expression<Func<TAttribute, TValue>>` ValueSelector) |  | 
| `TValue` | GetAttributeValue(this `Type` type, `Expression<Func<TAttribute, TValue>>` ValueSelector) |  | 
| `TValue` | GetAttributeValue(this `TAttribute` att, `Expression<Func<TAttribute, TValue>>` ValueSelector) |  | 
| `T` | GetEnumValue(this `String` Name) |  | 
| `T` | GetEnumValue(this `Int32` Name) |  | 
| `String` | GetEnumValueAsString(this `T` Value) |  | 
| `IEnumerable<T>` | GetEnumValues() |  | 
| `FieldInfo` | GetField(this `T` MyObject, `String` Name) |  | 
| `IEnumerable<FieldInfo>` | GetFields(this `T` MyObject, `BindingFlags` BindAttr) |  | 
| `IEnumerable<FieldInfo>` | GetFields(this `T` MyObject) |  | 
| `IEnumerable<Type>` | GetInheritedClasses() |  | 
| `IEnumerable<Type>` | GetInheritedClasses(this `Type` MyType) |  | 
| `Type` | GetNullableTypeOf(this `T` Obj) |  | 
| `IEnumerable<PropertyInfo>` | GetProperties(this `T` MyObject, `BindingFlags` BindAttr) |  | 
| `IEnumerable<PropertyInfo>` | GetProperties(this `T` MyObject) |  | 
| `PropertyInfo` | GetProperty(this `T` MyObject, `String` Name) |  | 
| `Hashtable` | GetPropertyHash(`T` properties) |  | 
| `T` | GetPropertyValue(this `O` MyObject, `String` Name) |  | 
| `Byte[]` | GetResourceBytes(this `Assembly` Assembly, `String` FileName) |  | 
| `String` | GetResourceFileText(this `Assembly` Assembly, `String` FileName) |  | 
| `Type` | GetTypeOf(this `O` Obj) |  | 
| `TValue` | GetValueOr(this `IDictionary<TKey, TValue>` Dic, `TKey` Key, `TValue` ReplaceValue = null) |  | 
| `Dictionary<Group, Int64>` | GroupAndCountBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector) |  | 
| `Dictionary<Group, Dictionary<Count, Int64>>` | GroupAndCountSubGroupBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector, `Func<Type, Count>` CountObjectBy) |  | 
| `Dictionary<Group, Dictionary<SubGroup, IEnumerable<Type>>>` | GroupAndSubGroupBy(this `IEnumerable<Type>` obj, `Func<Type, Group>` GroupSelector, `Func<Type, SubGroup>` SubGroupSelector) |  | 
| `Boolean` | HasAttribute(this `PropertyInfo` target, `Type` attribType) |  | 
| `Boolean` | HasAttribute(this `PropertyInfo` target) |  | 
| `Boolean` | HasProperty(this `Type` Type, `String` PropertyName, `Boolean` GetPrivate = False) |  | 
| `Boolean` | HasProperty(this `Object` Obj, `String` Name) |  | 
| `Boolean` | IsArrayOf(this `Type` Type) |  | 
| `Boolean` | IsArrayOf(this `Object` Obj) |  | 
| `Boolean` | IsBetween(this `IComparable` Value, `IComparable` MinValue, `IComparable` MaxValue) |  | 
| `Boolean` | IsBetweenExclusive(this `IComparable` Value, `IComparable` MinValue, `IComparable` MaxValue) |  | 
| `Boolean` | IsBetweenOrEqual(this `IComparable` Value, `IComparable` MinValue, `IComparable` MaxValue) |  | 
| `Boolean` | IsDictionary(this `Object` obj) |  | 
| `Boolean` | IsEnumerable(this `Object` obj) |  | 
| `Boolean` | IsEqual(this `T` Value, `T` EqualsToValue) |  | 
| `Boolean` | IsGenericOf(this `Type` MainType, `Type` Type) |  | 
| `Boolean` | IsGreaterThan(this `T` Value, `T` MinValue) |  | 
| `Boolean` | IsGreaterThanOrEqual(this `T` Value, `T` MinValue) |  | 
| `Boolean` | IsIn(this `Type` Obj, `Type[]` List) |  | 
| `Boolean` | IsIn(this `Type` Obj, `IEqualityComparer<Type>` Comparer = null, `Type[]` List) |  | 
| `Boolean` | IsIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) |  | 
| `Boolean` | IsIn(this `Type` Obj, `String` Text, `Nullable<StringComparison>` Comparer = null) |  | 
| `Boolean` | IsInAny(this `Type` Obj, `IEnumerable`1[]` List, `IEqualityComparer<Type>` Comparer = null) |  | 
| `Boolean` | IsLessThan(this `T` Value, `T` MaxValue) |  | 
| `Boolean` | IsLessThanOrEqual(this `T` Value, `T` MaxValue) |  | 
| `Boolean` | IsList(this `Object` obj) |  | 
| `Boolean` | IsNotIn(this `Type` Obj, `IEnumerable<Type>` List, `IEqualityComparer<Type>` Comparer = null) |  | 
| `Boolean` | IsNotIn(this `Type` Obj, `String` Text, `Nullable<StringComparison>` Comparer = null) |  | 
| `Boolean` | IsNotNullOrEmpty(this `IEnumerable<T>` List) |  | 
| `Boolean` | IsNullableType(this `Type` t) |  | 
| `Boolean` | IsNullableType(this `O` Obj) |  | 
| `Boolean` | IsNullableTypeOf(this `Object` Obj) |  | 
| `Boolean` | IsNullableTypeOf(this `O` Obj, `Type` Type) |  | 
| `Boolean` | IsNullOrEmpty(this `IEnumerable<T>` List) |  | 
| `Boolean` | IsNumericType(this `T` Obj) |  | 
| `Boolean` | IsTypeOf(this `Object` Obj) |  | 
| `Boolean` | IsTypeOf(this `O` Obj, `Type` Type) |  | 
| `Boolean` | IsValueType(this `Type` T) |  | 
| `Boolean` | IsValueType(this `T` Obj) |  | 
| `NameValueCollection` | Merge(this `IEnumerable<NameValueCollection>` Collections) |  | 
| `NameValueCollection` | Merge(this `NameValueCollection` FirstCollection, `NameValueCollection[]` OtherCollections) |  | 
| `List<T>` | MoveItems(this `List<T>` FromList, `List<T>` ToList, `Int32[]` Indexes) |  | 
| `Nullable<T>` | NullCoalesce(this `Nullable<T>` First, `Nullable`1[]` N) |  | 
| `Nullable<T>` | NullCoalesce(this `IEnumerable<Nullable<T>>` List) |  | 
| `T` | NullCoalesce(this `T` First, `T[]` N) |  | 
| `T` | NullCoalesce(this `IEnumerable<T>` List) |  | 
| `T` | NullPropertiesAsDefault(this `T` Obj, `Boolean` IncludeVirtual = False) |  | 
| `Boolean` | OnlyOneOf(this `IEnumerable<T>` List, `Func<T, Boolean>` predicate) |  | 
| `IEnumerable<String>` | PropertyNamesFor(this `String` Name) |  | 
| `Dictionary<Group, Int64>` | ReduceToTop(this `Dictionary<Group, Int64>` obj, `Int32` First, `Group` OtherLabel) |  | 
| `Dictionary<Group, Dictionary<Count, Int64>>` | ReduceToTop(this `Dictionary<Group, Dictionary<Count, Int64>>` Grouped, `Int32` First, `Count` OtherLabel) |  | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `TKey[]` Keys) |  | 
| `void` | RemoveIfExist(this `IDictionary<TKey, TValue>` dic, `Func<KeyValuePair<TKey, TValue>, Boolean>` predicate) |  | 
| `List<T>` | RemoveLast(this `List<T>` List, `Int32` Count = 1) |  | 
| `IDictionary<KeyType, ValueType>` | Set(this `IDictionary<KeyType, ValueType>` Dic, `KT` Key, `VT` Value) |  | 
| `IDictionary<KeyType, String>` | SetOrRemove(this `IDictionary<KeyType, String>` Dic, `KT` Key, `String` Value, `Boolean` NullIfBlank) |  | 
| `IDictionary<KeyType, ValueType>` | SetOrRemove(this `IDictionary<KeyType, ValueType>` Dic, `KT` Key, `VT` Value) |  | 
| `Type` | SetPropertyValue(this `Type` MyObject, `String` PropertyName, `Object` Value) |  | 
| `Type` | SetPropertyValue(this `Type` obj, `Expression<Func<Type, Prop>>` Selector, `Prop` Value) |  | 
| `Dictionary<Group, Dictionary<Count, Int64>>` | SkipZero(this `Dictionary<Group, Dictionary<Count, Int64>>` Grouped) |  | 
| `Dictionary<Count, Int64>` | SkipZero(this `Dictionary<Count, Int64>` Grouped) |  | 
| `ValueTuple<T, T>` | Swap(`T&` FirstValue, `T&` SecondValue) |  | 
| `Dictionary<K, IEnumerable<T>>` | TakeTop(this `Dictionary<K, IEnumerable<T>>` Dic, `Int32` Top, `Expression<Func<T, Object>>` ValueSelector) |  | 
| `IEnumerable<T>` | TakeTop(this `IEnumerable<T>` List, `Int32` Top, `Expression`1[]` ValueSelector) |  | 
| `IEnumerable<T>` | TakeTop(this `IEnumerable<T>` List, `Int32` Top, `Expression<Func<T, L>>` LabelSelector, `L` GroupOthersLabel, `Expression`1[]` ValueSelector) |  | 
| `Dictionary<K, T>` | TakeTop(this `Dictionary<K, T>` Dic, `Int32` Top, `K` GroupOthersLabel) |  | 
| `String` | ToFullExceptionString(this `Exception` ex, `String` Separator =  => ) |  | 
| `T` | Toggle(this `T` Current, `T` TrueValue, `T` FalseValue = null) |  | 
| `String` | ToQueryString(this `Dictionary<String, String>` Dic) |  | 
| `String` | ToQueryString(this `NameValueCollection` NVC) |  | 
| `IEnumerable<Object>` | ToTableArray(this `Dictionary<GroupKey, Dictionary<SubGroupKey, SubGroupValue>>` Groups, `Func<SubGroupKey, HeaderProperty>` HeaderProp) |  | 
| `IEnumerable<Object[]>` | ToTableArray(this `Dictionary<GroupKeyType, GroupValueType>` Groups) |  | 
| `Exception` | TryExecute(`Action` action) |  | 
| `T` | With(this `T` Obj, `Action<T>` Callback) |  | 


## `Paragraph`

```csharp
public class InnerLibs.Paragraph
    : List<Sentence>, IList<Sentence>, ICollection<Sentence>, IEnumerable<Sentence>, IEnumerable, IList, ICollection, IReadOnlyList<Sentence>, IReadOnlyCollection<Sentence>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `TextStructure` | StructuredText |  | 
| `Int32` | WordCount |  | 
| `IEnumerable<String>` | Words |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 
| `String` | ToString(`Int32` Ident) |  | 


## `PasswordLevel`

```csharp
public enum InnerLibs.PasswordLevel
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `2` | VeryWeak |  | 
| `3` | Weak |  | 
| `4` | Medium |  | 
| `5` | Strong |  | 
| `6` | VeryStrong |  | 


## `Phonetic`

```csharp
public class InnerLibs.Phonetic

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Item |  | 
| `String` | SoundExCode |  | 
| `String` | Word |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `PredefinedArrays`

```csharp
public static class InnerLibs.PredefinedArrays

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | AlphaChars |  | 
| `IEnumerable<String>` | AlphaLowerChars |  | 
| `IEnumerable<String>` | AlphaNumericChars |  | 
| `IEnumerable<String>` | AlphaUpperChars |  | 
| `IEnumerable<String>` | BreakLineChars |  | 
| `IEnumerable<String>` | CloseWrappers |  | 
| `IEnumerable<String>` | Consonants |  | 
| `IEnumerable<String>` | EndOfSentencePunctuation |  | 
| `IEnumerable<String>` | IdentChars |  | 
| `IEnumerable<String>` | InvisibleChars |  | 
| `IEnumerable<String>` | LowerConsonants |  | 
| `IEnumerable<String>` | LowerVowels |  | 
| `IEnumerable<String>` | MidSentencePunctuation |  | 
| `IEnumerable<DayOfWeek>` | MondayToFriday |  | 
| `IEnumerable<DayOfWeek>` | MondayToSaturday |  | 
| `IEnumerable<String>` | NumberChars |  | 
| `IEnumerable<Type>` | NumericTypes |  | 
| `IEnumerable<String>` | OpenWrappers |  | 
| `IEnumerable<String>` | PasswordSpecialChars |  | 
| `IEnumerable<Char>` | RegexChars |  | 
| `IEnumerable<String>` | Slashes |  | 
| `IEnumerable<String>` | SpecialChars |  | 
| `IEnumerable<DayOfWeek>` | SundayToSaturday |  | 
| `IEnumerable<String>` | UpperConsonants |  | 
| `IEnumerable<String>` | UpperVowels |  | 
| `IEnumerable<Type>` | ValueTypes |  | 
| `IEnumerable<String>` | Vowels |  | 
| `IEnumerable<String>` | WhiteSpaceChars |  | 
| `IEnumerable<String>` | WordSplitters |  | 
| `IEnumerable<String>` | WordWrappers |  | 


## `QuantityTextPair`

```csharp
public class InnerLibs.QuantityTextPair

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Item |  | 
| `String` | Plural |  | 
| `String` | Singular |  | 


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

```csharp
public static class InnerLibs.Romanize

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | ToArabic(this `String` RomanNumber) |  | 
| `String` | ToRoman(this `Int32` ArabicNumber) |  | 


## `RuleOfThree`

```csharp
public class InnerLibs.RuleOfThree
    : RuleOfThree<Decimal>

```

## `RuleOfThree<T>`

```csharp
public class InnerLibs.RuleOfThree<T>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `EquationPair<T>` | FirstEquation |  | 
| `EquationPair<T>` | SecondEquation |  | 
| `String` | UnknownName |  | 
| `Nullable<T>` | UnknownValue |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `RuleOfThree<T>` | Resolve() |  | 
| `Nullable`1[][]` | ToArray() |  | 
| `Nullable`1[]` | ToFlatArray() |  | 
| `String` | ToString() |  | 


## `SelfKeyDictionary<KeyType, ClassType>`

```csharp
public class InnerLibs.SelfKeyDictionary<KeyType, ClassType>
    : IDictionary<KeyType, ClassType>, ICollection<KeyValuePair<KeyType, ClassType>>, IEnumerable<KeyValuePair<KeyType, ClassType>>, IEnumerable, IDictionary, ICollection

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Count |  | 
| `Boolean` | IsFixedSize |  | 
| `Boolean` | IsReadOnly |  | 
| `Boolean` | IsSynchronized |  | 
| `Object` | Item |  | 
| `ClassType` | Item |  | 
| `ICollection<KeyType>` | Keys |  | 
| `Object` | SyncRoot |  | 
| `ICollection<ClassType>` | Values |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`KeyType` key, `ClassType` value) |  | 
| `KeyType` | Add(`ClassType` Value) |  | 
| `void` | Add(`Object` key, `Object` value) |  | 
| `IEnumerable<KeyType>` | AddRange(`ClassType[]` Values) |  | 
| `IEnumerable<KeyType>` | AddRange(`IEnumerable<ClassType>` Values) |  | 
| `void` | Clear() |  | 
| `Boolean` | Contains(`KeyValuePair<KeyType, ClassType>` item) |  | 
| `Boolean` | Contains(`Object` key) |  | 
| `Boolean` | ContainsKey(`KeyType` key) |  | 
| `void` | CopyTo(`KeyValuePair`2[]` array, `Int32` arrayIndex) |  | 
| `void` | CopyTo(`Array` array, `Int32` index) |  | 
| `IEnumerator<KeyValuePair<KeyType, ClassType>>` | GetEnumerator() |  | 
| `Boolean` | Remove(`KeyType` key) |  | 
| `Boolean` | Remove(`ClassType` Value) |  | 
| `Boolean` | Remove(`KeyValuePair<KeyType, ClassType>` item) |  | 
| `void` | Remove(`Object` key) |  | 
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
| `Int32` | WordCount |  | 
| `IEnumerable<String>` | Words |  | 


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
| `Boolean` | IsCloseWrapChar |  | 
| `Boolean` | IsClosingQuote |  | 
| `Boolean` | IsComma |  | 
| `Boolean` | IsDoubleQuote |  | 
| `Boolean` | IsEndOfSentencePunctuation |  | 
| `Boolean` | IsMidSentencePunctuation |  | 
| `Boolean` | IsNotWord |  | 
| `Boolean` | IsOpeningQuote |  | 
| `Boolean` | IsOpenWrapChar |  | 
| `Boolean` | IsPunctuation |  | 
| `Boolean` | IsQuote |  | 
| `Boolean` | IsSingleQuote |  | 
| `Boolean` | IsWord |  | 
| `Boolean` | NeedSpaceOnNext |  | 
| `Sentence` | Sentence |  | 
| `String` | Text |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `SentencePart` | GetMatchQuote() |  | 
| `SentencePart` | GetNextPart() |  | 
| `SentencePart` | GetPreviousPart() |  | 
| `String` | ToString() |  | 


## `ShortLinkGenerator`

```csharp
public class InnerLibs.ShortLinkGenerator

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Seed |  | 
| `String` | Token |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | UrlPattern |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Uri` | CreateLink(`String` UrlPattern, `Int32` ID) |  | 
| `Uri` | CreateLink(`Int32` ID) |  | 
| `Int32` | Decode(`String` s) |  | 
| `String` | Encode(`Int32` i) |  | 
| `String` | RandomHash() |  | 
| `String` | RandomHash(`Int32` Min, `Int32` Max) |  | 
| `String` | ToString() |  | 


## `SoundExType`

```csharp
public static class InnerLibs.SoundExType

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | SoundEx(this `String` Text) |  | 
| `Boolean` | SoundsLike(this `String` FirstText, `String` SecondText) |  | 


## `SqlServerConnectionStringParser`

```csharp
public class InnerLibs.SqlServerConnectionStringParser
    : ConnectionStringParser, IDictionary<String, String>, ICollection<KeyValuePair<String, String>>, IEnumerable<KeyValuePair<String, String>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<String, String>, IReadOnlyCollection<KeyValuePair<String, String>>, ISerializable, IDeserializationCallback

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | InitialCatalog |  | 
| `Boolean` | IntegratedSecurity |  | 
| `String` | Password |  | 
| `String` | Server |  | 
| `String` | UserID |  | 


## `Text`

```csharp
public static class InnerLibs.Text

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Alphabetize(this `String` Text) |  | 
| `String` | Append(this `String` Text, `String` AppendText) |  | 
| `String` | AppendBarcodeCheckSum(this `String` Code) |  | 
| `String` | AppendIf(this `String` Text, `String` AppendText, `Boolean` Test) |  | 
| `String` | AppendIf(this `String` Text, `String` AppendText, `Func<String, Boolean>` Test) |  | 
| `String` | AppendLine(this `String` Text, `String` AppendText) |  | 
| `String` | AppendUrlParameter(this `String` Url, `String` Key, `String[]` Value) |  | 
| `String` | AppendWhile(this `String` Text, `String` AppendText, `Func<String, Boolean>` Test) |  | 
| `String` | ApplySpaceOnWrapChars(this `String` Text) |  | 
| `String` | BoxText(this `String` Text) |  | 
| `String` | BoxTextCSS(this `String` Text) |  | 
| `String` | Brackfy(this `String` Text, `Char` BracketChar = {) |  | 
| `ValueTuple<String, Boolean>` | Censor(this `String` Text, `IEnumerable<String>` BadWords, `Char` CensorshipCharacter) |  | 
| `ValueTuple<String, Boolean>` | Censor(this `String` Text, `Char` CensorshipCharacter, `String[]` BadWords) |  | 
| `Boolean` | Contains(this `String` Text, `String` OtherText, `StringComparison` StringComparison) |  | 
| `Boolean` | ContainsAll(this `String` Text, `String[]` Values) |  | 
| `Boolean` | ContainsAll(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) |  | 
| `Boolean` | ContainsAllWords(this `String` Text, `String[]` Words) |  | 
| `Boolean` | ContainsAllWords(this `String` Text, `IEqualityComparer<String>` Comparer, `String[]` Words) |  | 
| `Boolean` | ContainsAny(this `String` Text, `String[]` Values) |  | 
| `Boolean` | ContainsAny(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) |  | 
| `Boolean` | ContainsAnyWords(this `String` Text, `String[]` Words) |  | 
| `Boolean` | ContainsAnyWords(this `String` Text, `IEqualityComparer<String>` Comparer, `String[]` Words) |  | 
| `Boolean` | ContainsDigit(this `String` Text) |  | 
| `Boolean` | ContainsLower(this `String` Text) |  | 
| `Boolean` | ContainsMost(this `String` Text, `StringComparison` ComparisonType, `String[]` Values) |  | 
| `Boolean` | ContainsMost(this `String` Text, `String[]` Values) |  | 
| `Boolean` | ContainsUpper(this `String` Text) |  | 
| `Int32` | CountCharacter(this `String` Text, `Char` Character) |  | 
| `Dictionary<String, Int64>` | CountWords(this `String` Text, `Boolean` RemoveDiacritics = True, `String[]` Words = null) |  | 
| `Boolean` | CrossContains(this `String` Text, `String` OtherText, `StringComparison` StringComparison = InvariantCultureIgnoreCase) |  | 
| `String` | DeleteLine(this `String` Text, `Int32` LineIndex) |  | 
| `Dictionary<String, Int64>` | DistinctCount(`String[]` List) |  | 
| `Dictionary<String, Int64>` | DistinctCount(this `String` Text) |  | 
| `Boolean` | EndsWithAny(this `String` Text, `StringComparison` comparison, `String[]` Words) |  | 
| `Boolean` | EndsWithAny(this `String` Text, `String[]` Words) |  | 
| `String` | EscapeQuotesToQuery(this `String` Text, `Boolean` AlsoQuoteText = False) |  | 
| `IEnumerable<String>` | ExtractEmails(this `String` Text) |  | 
| `String[]` | FindByRegex(this `String` Text, `String` Regex, `RegexOptions` RegexOptions = None) |  | 
| `String[]` | FindCEP(this `String` Text) |  | 
| `IEnumerable<String>` | FindNumbers(this `String` Text) |  | 
| `String[]` | FindTelephoneNumbers(this `String` Text) |  | 
| `String` | FixCapitalization(this `String` Text) |  | 
| `String` | FixHTMLBreakLines(this `String` Text) |  | 
| `String` | FixPath(this `String` Text, `Boolean` AlternativeChar = False) |  | 
| `String` | FixPunctuation(this `String` Text, `String` Punctuation = ., `Boolean` ForceSpecificPunctuation = False) |  | 
| `String` | FixText(this `String` Text, `Int32` Ident = 0, `Int32` BreakLinesBetweenParagraph = 0) |  | 
| `String` | ForEachLine(this `String` Text, `Expression<Func<String, String>>` Action) |  | 
| `String` | FormatCEP(this `Int32` CEP) |  | 
| `String` | FormatCEP(this `String` CEP) |  | 
| `String` | FormatCNPJ(this `Int64` CNPJ) |  | 
| `String` | FormatCNPJ(this `String` CNPJ) |  | 
| `String` | FormatCPF(this `Int64` CPF) |  | 
| `String` | FormatCPF(this `String` CPF) |  | 
| `String` | FormatCPFOrCNPJ(this `Int64` Document) |  | 
| `String` | FormatCPFOrCNPJ(this `String` Document) |  | 
| `String` | FormatPIS(this `String` PIS) |  | 
| `String` | FormatPIS(this `Int64` PIS) |  | 
| `String` | FormatString(this `String` Text, `String[]` Args) |  | 
| `String` | GetAfter(this `String` Text, `String` Value, `Boolean` WhiteIfNotFound = False) |  | 
| `String[]` | GetAllBetween(this `String` Text, `String` Before, `String` After = ) |  | 
| `String` | GetBefore(this `String` Text, `String` Value, `Boolean` WhiteIfNotFound = False) |  | 
| `String` | GetBetween(this `String` Text, `String` Before, `String` After) |  | 
| `String` | GetDomain(this `Uri` URL, `Boolean` RemoveFirstSubdomain = False) |  | 
| `String` | GetDomain(this `String` URL, `Boolean` RemoveFirstSubdomain = False) |  | 
| `String` | GetDomainAndProtocol(this `String` URL) |  | 
| `String` | GetFirstChars(this `String` Text, `Int32` Number = 1) |  | 
| `String` | GetLastChars(this `String` Text, `Int32` Number = 1) |  | 
| `String` | GetMiddleChars(this `String` Text, `Int32` Length) |  | 
| `String` | GetOppositeWrapChar(this `String` Text) |  | 
| `Char` | GetOppositeWrapChar(this `Char` Char) |  | 
| `Type` | GetRandomItem(this `Type[]` Array) |  | 
| `String` | GetRelativeURL(this `Uri` URL, `Boolean` WithQueryString = True) |  | 
| `String` | GetRelativeURL(this `String` URL, `Boolean` WithQueryString = True) |  | 
| `IOrderedEnumerable<String>` | GetWords(this `String` Text) |  | 
| `String[]` | GetWrappedText(this `String` Text, `String` Character = ", `Boolean` ExcludeWrapChars = True) |  | 
| `String` | HtmlDecode(this `String` Text) |  | 
| `String` | HtmlEncode(this `String` Text) |  | 
| `String` | Inject(this `T` Obj, `String` TemplatedString, `Boolean` IsSQL = False) |  | 
| `String` | Inject(this `String` formatString, `T` injectionObject, `Boolean` IsSQL = False) |  | 
| `String` | Inject(this `String` formatString, `Hashtable` attributes, `Boolean` IsSQL = False) |  | 
| `String` | InjectSingleValue(this `String` formatString, `String` key, `Object` replacementValue, `Boolean` IsSQL = False, `CultureInfo` cultureInfo = null) |  | 
| `String` | Interpolate(this `String` Text, `String[]` Texts) |  | 
| `Boolean` | IsAnagramOf(this `String` Text, `String` AnotherText) |  | 
| `Boolean` | IsAny(this `String` Text, `String[]` Texts) |  | 
| `Boolean` | IsAny(this `String` Text, `StringComparison` Comparison, `String[]` Texts) |  | 
| `Boolean` | IsCloseWrapChar(this `String` Text) |  | 
| `Boolean` | IsCloseWrapChar(this `Char` Char) |  | 
| `Boolean` | IsCrossLikeAny(this `String` Text, `IEnumerable<String>` Patterns) |  | 
| `Boolean` | IsLikeAny(this `String` Text, `IEnumerable<String>` Patterns) |  | 
| `Boolean` | IsLikeAny(this `String` Text, `String[]` Patterns) |  | 
| `Boolean` | IsNotAny(this `String` Text, `String[]` Texts) |  | 
| `Boolean` | IsNotAny(this `String` Text, `StringComparison` Comparison, `String[]` Texts) |  | 
| `Boolean` | IsOpenWrapChar(this `String` Text) |  | 
| `Boolean` | IsOpenWrapChar(this `Char` Char) |  | 
| `Boolean` | IsPalindrome(this `String` Text, `Boolean` IgnoreWhiteSpaces = True) |  | 
| `Int32` | LevenshteinDistance(this `String` Text1, `String` Text2) |  | 
| `Boolean` | Like(this `String` source, `String` Pattern) |  | 
| `String` | MaskTelephoneNumber(this `String` Number) |  | 
| `String` | MaskTelephoneNumber(this `Int64` Number) |  | 
| `String` | MaskTelephoneNumber(this `Int32` Number) |  | 
| `String` | MaskTelephoneNumber(this `Decimal` Number) |  | 
| `String` | MaskTelephoneNumber(this `Double` Number) |  | 
| `String` | Pad(this `String` Text, `Int32` TotalLength, `Char` PaddingChar =  ) |  | 
| `String` | ParseAlphaNumeric(this `String` Text) |  | 
| `ConnectionStringParser` | ParseConnectionString(this `String` ConnectionString) |  | 
| `String` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) |  | 
| `Type` | ParseDigits(this `String` Text, `CultureInfo` Culture = null) |  | 
| `NameValueCollection` | ParseQueryString(this `String` QueryString, `String[]` Keys) |  | 
| `String` | PascalCaseAdjust(this `String` Text) |  | 
| `IEnumerable<String>` | PascalCaseSplit(this `String` Text) |  | 
| `String[]` | Poopfy(`String[]` Words) |  | 
| `String` | Poopfy(this `String` Text) |  | 
| `String` | PreetyPrint(this `XmlDocument` Document) |  | 
| `String` | Prepend(this `String` Text, `String` PrependText) |  | 
| `String` | PrependIf(this `String` Text, `String` PrependText, `Func<String, Boolean>` Test = null) |  | 
| `String` | PrependIf(this `String` Text, `String` PrependText, `Boolean` Test) |  | 
| `String` | PrependLine(this `String` Text, `String` PrependText) |  | 
| `String` | PrependWhile(this `String` Text, `String` PrependText, `Func<String, Boolean>` Test) |  | 
| `String` | PrintIf(this `String` Text, `Boolean` BooleanValue) |  | 
| `String` | QuantifyText(this `FormattableString` PluralText) |  | 
| `String` | QuantifyText(this `String` PluralText, `Object` Quantity) |  | 
| `String` | QuantifyText(this `String` PluralText, `Object` QuantityOrListOrBoolean, `Decimal&` OutQuantity) |  | 
| `String` | QuantifyText(this `IEnumerable<T>` List, `String` PluralText) |  | 
| `String` | QuantifyText(this `Int32` Quantity, `String` PluralText) |  | 
| `String` | QuantifyText(this `Decimal` Quantity, `String` PluralText) |  | 
| `String` | QuantifyText(this `Int16` Quantity, `String` PluralText) |  | 
| `String` | QuantifyText(this `Int64` Quantity, `String` PluralText) |  | 
| `String` | QuantifyText(this `Double` Quantity, `String` PluralText) |  | 
| `String` | Quote(this `String` Text, `Char` OpenQuoteChar = ") |  | 
| `String` | QuoteIf(this `String` Text, `Boolean` Condition, `Char` QuoteChar = ") |  | 
| `Type` | RandomItem(`Type[]` Array) |  | 
| `IEnumerable<String>` | ReduceToDifference(this `IEnumerable<String>` Texts, `Boolean` FromStart = False, `String` BreakAt = null) |  | 
| `IEnumerable<String>` | ReduceToDifference(this `IEnumerable<String>` Texts, `String&` RemovedPart, `Boolean` FromStart = False, `String` BreakAt = null) |  | 
| `String` | RegexEscape(this `String` Text) |  | 
| `String` | RemoveAccents(this `String` Text) |  | 
| `String` | RemoveAny(this `String` Text, `String[]` Values) |  | 
| `String` | RemoveAny(this `String` Text, `Char[]` Values) |  | 
| `String` | RemoveDiacritics(this `String` Text) |  | 
| `String` | RemoveFirstChars(this `String` Text, `Int32` Quantity = 1) |  | 
| `String` | RemoveHTML(this `String` Text) |  | 
| `String` | RemoveLastChars(this `String` Text, `Int32` Quantity = 1) |  | 
| `String` | RemoveNonPrintable(this `String` Text) |  | 
| `String` | Repeat(this `String` Text, `Int32` Times = 2) |  | 
| `String[]` | Replace(this `String[]` Strings, `String` OldValue, `String` NewValue, `Boolean` ReplaceIfEquals = True) |  | 
| `IEnumerable<String>` | Replace(this `IEnumerable<String>` Strings, `String` OldValue, `String` NewValue, `Boolean` ReplaceIfEquals = True) |  | 
| `String` | ReplaceFirst(this `String` Text, `String` OldText, `String` NewText = ) |  | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, String>` Dic) |  | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, T>` Dic) |  | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String, String[]>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) |  | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String[], String>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) |  | 
| `String` | ReplaceFrom(this `String` Text, `IDictionary<String[], String[]>` Dic, `StringComparison` Comparison = InvariantCultureIgnoreCase) |  | 
| `String` | ReplaceLast(this `String` Text, `String` OldText, `String` NewText = ) |  | 
| `String` | ReplaceMany(this `String` Text, `String` NewValue, `String[]` OldValues) |  | 
| `String` | ReplaceNone(this `String` Text, `String` OldValue) |  | 
| `String` | SelectJoinString(`String` Separator, `Type[]` Array) |  | 
| `String` | SelectJoinString(this `List<Type>` List, `String` Separator = ) |  | 
| `IEnumerable<String>` | SelectLike(this `IEnumerable<String>` source, `String` Pattern) |  | 
| `String` | SensitiveReplace(this `String` Text, `String` OldValue, `String` NewValue, `StringComparison` ComparisonType = InvariantCulture) |  | 
| `String` | SensitiveReplace(this `String` Text, `String` NewValue, `IEnumerable<String>` OldValues, `StringComparison` ComparisonType = InvariantCulture) |  | 
| `Type[]` | Shuffle(this `Type[]` Array) |  | 
| `List<Type>` | Shuffle(this `List<Type>` List) |  | 
| `String` | Shuffle(this `String` Text) |  | 
| `String` | Singularize(this `String` Text) |  | 
| `String` | Slice(this `String` Text, `Int32` TextLength = 0, `String` Ellipsis = ..., `Boolean` TrimCarriage = False) |  | 
| `String[]` | Split(this `String` Text, `String` Separator, `StringSplitOptions` Options = RemoveEmptyEntries) |  | 
| `String[]` | SplitAny(this `String` Text, `String[]` SplitText) |  | 
| `String[]` | SplitAny(this `String` Text, `StringSplitOptions` SplitOptions, `String[]` SplitText) |  | 
| `String[]` | SplitAny(this `String` Text, `IEnumerable<String>` SplitText) |  | 
| `String[]` | SplitAny(this `String` Text, `StringSplitOptions` SplitOptions, `IEnumerable<String>` SplitText) |  | 
| `Boolean` | StartsWithAny(this `String` Text, `StringComparison` comparison, `String[]` Words) |  | 
| `Boolean` | StartsWithAny(this `String` Text, `String[]` Words) |  | 
| `String` | ToAlternateCase(this `String` Text) |  | 
| `String` | ToAnagram(this `String` Text) |  | 
| `IEnumerable<Int32>` | ToAsc(this `String` c) |  | 
| `Int32` | ToAsc(this `Char` c) |  | 
| `Byte` | ToAscByte(this `Char` c) |  | 
| `String` | ToCSV(this `IEnumerable<Dictionary<String, Object>>` Items, `String` Separator = ,, `Boolean` IncludeHeader = False) |  | 
| `String` | ToCSV(this `IEnumerable<T>` Items, `String` Separator = ,, `Boolean` IncludeHeader = False) |  | 
| `String` | ToFileSizeString(this `Byte[]` Size, `Int32` DecimalPlaces = -1) |  | 
| `String` | ToFileSizeString(this `FileInfo` Size, `Int32` DecimalPlaces = -1) |  | 
| `String` | ToFileSizeString(this `Double` Size, `Int32` DecimalPlaces = -1) |  | 
| `String` | ToFileSizeString(this `Int32` Size, `Int32` DecimalPlaces = -1) |  | 
| `String` | ToFileSizeString(this `Int64` Size, `Int32` DecimalPlaces = -1) |  | 
| `String` | ToFileSizeString(this `Decimal` Size, `Int32` DecimalPlaces = -1) |  | 
| `FormattableString` | ToFormattableString(this `String` Text, `Object[]` args) |  | 
| `FormattableString` | ToFormattableString(`IEnumerable<T>` args, `String` Text) |  | 
| `FormattableString` | ToFormattableString(this `String` Text, `IEnumerable<Object[]>` args) |  | 
| `String` | ToFriendlyPathName(this `String` Text) |  | 
| `String` | ToFriendlyURL(this `String` Text, `Boolean` UseUnderscore = False) |  | 
| `String` | ToLeet(this `String` Text, `Int32` Degree = 7) |  | 
| `String` | ToNormalCase(this `String` Text) |  | 
| `String` | ToPercentString(this `Decimal` Number, `Int32` Decimals = -1) |  | 
| `String` | ToPercentString(this `Int32` Number) |  | 
| `String` | ToPercentString(this `Double` Number, `Int32` Decimals = -1) |  | 
| `String` | ToPercentString(this `Int16` Number) |  | 
| `String` | ToPercentString(this `Int64` Number) |  | 
| `String` | ToPhrase(this `IEnumerable<TSource>` Texts, `String` PhraseStart = , `String` And = and, `String` EmptyValue = null, `Char` Separator = ,) |  | 
| `String` | ToPhrase(`String` And, `String[]` Texts) |  | 
| `String` | ToProperCase(this `String` Text, `Boolean` ForceCase = False) |  | 
| `String` | ToRandomCase(this `String` Text, `Int32` Times = 0) |  | 
| `String` | ToSlugCase(this `String` Text, `Boolean` UseUnderscore = False) |  | 
| `String` | ToSnakeCase(this `String` Text) |  | 
| `Stream` | ToStream(this `String` Text) |  | 
| `String` | ToTitle(this `String` Text, `Boolean` ForceCase = False) |  | 
| `String` | ToXMLString(this `XmlDocument` XML) |  | 
| `String` | TrimAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StringTest) |  | 
| `String` | TrimAny(this `String` Text, `String[]` StringTest) |  | 
| `IEnumerable<String>` | TrimBetween(this `IEnumerable<String>` Texts) |  | 
| `String` | TrimBetween(this `String` Text) |  | 
| `String` | TrimCarriage(this `String` Text) |  | 
| `String` | TrimFirstAny(this `String` Text, `Boolean` ContinuouslyRemove, `StringComparison` comparison, `String[]` StartStringTest) |  | 
| `String` | TrimFirstAny(this `String` Text, `StringComparison` comparison, `String[]` StartStringTest) |  | 
| `String` | TrimFirstAny(this `String` Text, `String[]` StartStringTest) |  | 
| `String` | TrimFirstAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` StartStringTest) |  | 
| `String` | TrimFirstEqual(this `String` Text, `String` StartStringTest, `StringComparison` comparison = CurrentCulture) |  | 
| `String` | TrimLastAny(this `String` Text, `Boolean` ContinuouslyRemove, `StringComparison` comparison, `String[]` EndStringTest) |  | 
| `String` | TrimLastAny(this `String` Text, `String[]` EndStringTest) |  | 
| `String` | TrimLastAny(this `String` Text, `Boolean` ContinuouslyRemove, `String[]` EndStringTest) |  | 
| `String` | TrimLastAny(this `String` Text, `StringComparison` comparison, `String[]` EndStringTest) |  | 
| `String` | TrimLastEqual(this `String` Text, `String` EndStringTest, `StringComparison` comparison = CurrentCulture) |  | 
| `String` | UnBrackfy(this `String` Text) |  | 
| `String` | UnBrackfy(this `String` Text, `Char` BracketChar, `Boolean` ContinuouslyRemove = False) |  | 
| `String` | UnQuote(this `String` Text) |  | 
| `String` | UnQuote(this `String` Text, `Char` OpenQuoteChar, `Boolean` ContinuouslyRemove = False) |  | 
| `String` | UnWrap(this `String` Text, `String` WrapText = ", `Boolean` ContinuouslyRemove = False) |  | 
| `String` | UrlDecode(this `String` Text) |  | 
| `String` | UrlEncode(this `String` Text) |  | 
| `String` | Wrap(this `String` Text, `String` WrapText = ") |  | 
| `String` | Wrap(this `String` Text, `String` OpenWrapText, `String` CloseWrapText) |  | 
| `HtmlTag` | WrapInTag(this `String` Text, `String` TagName) |  | 


## `TextStructure`

```csharp
public class InnerLibs.TextStructure
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
| `Paragraph` | GetParagraph(`Int32` Index) |  | 
| `Sentence` | GetSentence(`Int32` Index) |  | 
| `IEnumerable<Sentence>` | GetSentences() |  | 
| `String` | ToString() |  | 


## `UnitConverter`

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
| `String` | Abreviate(`Decimal` Number, `Int32` DecimalPlaces = -1) |  | 
| `String` | Abreviate(`Int32` Number) |  | 
| `String` | Abreviate(`Int16` Number) |  | 
| `String` | Abreviate(`Int64` Number) |  | 
| `Decimal` | Convert(`Decimal` Number, `String` From, `String` To) |  | 
| `Decimal` | Convert(`String` AbreviatedNumber, `String` To) |  | 
| `String` | ConvertAbreviate(`String` AbreviatedNumber, `String` To) |  | 
| `Decimal` | Parse(`String` Number, `Int32` DecimalPlaces = -1) |  | 
| `String` | ParseUnit(`String` Number) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UnitConverter` | CreateBase1000Converter() |  | 
| `UnitConverter` | CreateComplexMassConverter() |  | 
| `UnitConverter` | CreateFileSizeConverter() |  | 
| `UnitConverter` | CreateSimpleMassConverter() |  | 


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


## `vCard`

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

```csharp
public static class InnerLibs.Verify

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | CanBeNumber(this `Object` Value) |  | 
| `PasswordLevel` | CheckPassword(this `String` Password) |  | 
| `Int32` | GetIndexOf(this `IEnumerable<T>` Arr, `T` item) |  | 
| `T` | IfBlank(this `Object` Value, `T` ValueIfBlank = null) |  | 
| `T` | IfBlankOrNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfBlankOrNoIndex) |  | 
| `T` | IfNoIndex(this `IEnumerable<T>` Arr, `Int32` Index, `T` ValueIfNoIndex = null) |  | 
| `T[]` | IfNullOrEmpty(this `Object[]` Value, `T[]` ValuesIfBlank) |  | 
| `IEnumerable<T>` | IfNullOrEmpty(this `IEnumerable<Object[]>` Value, `T[]` ValuesIfBlank) |  | 
| `IEnumerable<T>` | IfNullOrEmpty(this `IEnumerable<Object[]>` Value, `IEnumerable<T>` ValueIfBlank) |  | 
| `Boolean` | IsArray(`T` Obj) |  | 
| `Boolean` | IsBlank(this `String` Text) |  | 
| `Boolean` | IsBlank(this `FormattableString` Text) |  | 
| `Boolean` | IsBool(this `T` Obj) |  | 
| `Boolean` | IsDate(this `String` Obj) |  | 
| `Boolean` | IsDate(this `T` Obj) |  | 
| `Boolean` | IsDirectoryPath(this `String` Text) |  | 
| `Boolean` | IsDomain(this `String` Text) |  | 
| `Boolean` | IsEmail(this `String` Text) |  | 
| `Boolean` | IsEven(this `Decimal` Value) |  | 
| `Boolean` | IsEven(this `Int32` Value) |  | 
| `Boolean` | IsEven(this `Int16` Value) |  | 
| `Boolean` | IsEven(this `Single` Value) |  | 
| `Boolean` | IsEven(this `Int64` Value) |  | 
| `Boolean` | IsEven(this `Double` Value) |  | 
| `Boolean` | IsFilePath(this `String` Text) |  | 
| `Boolean` | IsInUse(this `FileInfo` File) |  | 
| `Boolean` | IsIP(this `String` IP) |  | 
| `Boolean` | IsNotBlank(this `String` Text) |  | 
| `Boolean` | IsNotBlank(this `FormattableString` Text) |  | 
| `Boolean` | IsNotNumber(this `Object` Value) |  | 
| `Boolean` | IsNumber(this `Object` Value) |  | 
| `Boolean` | IsOdd(this `Decimal` Value) |  | 
| `Boolean` | IsOdd(this `Int32` Value) |  | 
| `Boolean` | IsOdd(this `Int64` Value) |  | 
| `Boolean` | IsOdd(this `Int16` Value) |  | 
| `Boolean` | IsOdd(this `Single` Value) |  | 
| `Boolean` | IsPath(this `String` Text) |  | 
| `Boolean` | IsTelephone(this `String` Text) |  | 
| `Boolean` | IsURL(this `String` Text) |  | 
| `Boolean` | IsValidCEP(this `String` CEP) |  | 
| `Boolean` | IsValidCNH(this `String` CNH) |  | 
| `Boolean` | IsValidCNPJ(this `String` Text) |  | 
| `Boolean` | IsValidCPF(this `String` Text) |  | 
| `Boolean` | IsValidCPFOrCNPJ(this `String` Text) |  | 
| `Boolean` | IsValidDomain(this `String` DomainOrEmail) |  | 
| `Boolean` | IsValidEAN(this `String` Code) |  | 
| `Boolean` | IsValidEAN(this `Int32` Code) |  | 
| `Boolean` | IsValidPIS(this `String` PIS) |  | 
| `T` | NullIf(this `T` Value, `Func<T, Boolean>` TestExpression) |  | 
| `T` | NullIf(this `T` Value, `T` TestValue) |  | 
| `Nullable<T>` | NullIf(this `Nullable<T>` Value, `Nullable<T>` TestValue) |  | 
| `String` | NullIf(this `String` Value, `String` TestValue, `StringComparison` ComparisonType = InvariantCultureIgnoreCase) |  | 
| `Boolean` | Validate(this `T` Value, `Expression`1[]` Tests) |  | 
| `Boolean` | Validate(this `T` Value, `Int32` MinPoints, `Expression`1[]` Tests) |  | 
| `Int32` | ValidateCount(this `T` Value, `Expression`1[]` Tests) |  | 
| `T` | ValidateOr(this `T` Value, `Expression<Func<T, Boolean>>` Test, `E` Exception) |  | 
| `T` | ValidateOr(this `T` Value, `Expression<Func<T, Boolean>>` Test) |  | 
| `T` | ValidateOr(this `T` Value, `Expression<Func<T, Boolean>>` Test, `T` defaultValue) |  | 
| `Boolean` | ValidatePassword(this `String` Password, `PasswordLevel` PasswordLevel = Strong) |  | 
| `Boolean` | WaifForFile(this `FileInfo` File, `Int32` Seconds = 1, `Nullable<Int32>` MaxFailCount = null, `Action<Int32>` OnAttemptFail = null) |  | 
| `Boolean` | WithFile(this `FileInfo` File, `Action<FileInfo>` OnSuccess, `Action<FileInfo>` OnFail, `Action<Int32>` OnAttemptFail = null, `Int32` Seconds = 1, `Nullable<Int32>` MaxFailCount = null) |  | 


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

```csharp
public static class InnerLibs.Web

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Uri` | AddParameter(this `Uri` Url, `String` Key, `Boolean` Append, `String[]` Values) |  | 
| `Uri` | AddParameter(this `Uri` Url, `String` Key, `String[]` Values) |  | 
| `Byte[]` | DownloadFile(this `String` URL) |  | 
| `Image` | DownloadImage(this `String` URL) |  | 
| `String` | DownloadString(this `String` URL) |  | 
| `String` | FileNameAsTitle(this `FileSystemInfo` Info) |  | 
| `String` | FileNameAsTitle(this `String` FileName) |  | 
| `String` | GetFacebookUsername(this `String` URL) |  | 
| `String` | GetFacebookUsername(this `Uri` URL) |  | 
| `IEnumerable<String>` | GetIPs() |  | 
| `IEnumerable<String>` | GetLocalIP() |  | 
| `String` | GetPublicIP() |  | 
| `IEnumerable<String>` | GetURLSegments(this `String` URL) |  | 
| `String` | GetVideoId(this `Uri` URL) |  | 
| `String` | GetVideoID(this `String` URL) |  | 
| `Byte[]` | GetYoutubeThumbnail(`String` URL) |  | 
| `Byte[]` | GetYoutubeThumbnail(this `Uri` URL) |  | 
| `Boolean` | IsConnected(`String` Test = http://google.com) |  | 
| `String` | MinifyCSS(this `String` CSS, `Boolean` PreserveComments = False) |  | 
| `NameValueCollection` | ParseQueryString(this `Uri` URL) |  | 
| `Uri` | RemoveParameter(this `Uri` Url, `String[]` Keys) |  | 
| `String` | RemoveUrlParameters(this `String` UrlPattern) |  | 
| `String` | ReplaceUrlParameters(this `String` UrlPattern, `T` obj) |  | 


