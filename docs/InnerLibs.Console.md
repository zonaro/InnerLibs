## `Cnsl`

```csharp
public static class InnerLibs.Console.Cnsl

```

Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Beep(this `String` Text, `Int32` Times = 1) |  | 
| `String` | Beep(this `String` Text, `Int32` Frequency, `Int32` Duration, `Int32` Times = 1) |  | 
| `String` | ConsoleBreakLine(this `String` Text, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleBreakLine(this `Int32` BreakLines) |  | 
| `String` | ConsoleBreakLine() |  | 
| `String` | ConsoleLog(this `String` Text, `Nullable<DateTime>` LogDateTime = null, `Nullable<ConsoleColor>` DateColor = null, `Nullable<ConsoleColor>` MessageColor = null, `String` DateFormat = null, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleLog(this `DateTime` LogDateTime, `String` Text, `Nullable<ConsoleColor>` DateColor = null, `Nullable<ConsoleColor>` MessageColor = null, `String` DateFormat = null, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWrite(this `String` Text, `Dictionary<String, ConsoleColor>` CustomColoredWords, `Int32` BreakLines = 0) |  | 
| `String` | ConsoleWrite(this `String` Text, `Dictionary<String, ConsoleColor>` CustomColoredWords, `StringComparison` Comparison, `Int32` BreakLines = 0) |  | 
| `String` | ConsoleWrite(this `String` Text, `Int32` BreakLines = 0) |  | 
| `String` | ConsoleWrite(this `String` Text, `ConsoleColor` Color, `Int32` BreakLines = 0) |  | 
| `T` | ConsoleWriteError(this `T` Exception, `String` Message, `String` Separator, `ConsoleColor` Color = Red, `Int32` BreakLines = 1) |  | 
| `T` | ConsoleWriteError(this `T` Exception, `String` Separator, `ConsoleColor` Color = Red, `Int32` BreakLines = 1) |  | 
| `T` | ConsoleWriteError(this `T` Exception, `String` Message) |  | 
| `T` | ConsoleWriteError(this `T` Exception) |  | 
| `T` | ConsoleWriteError(this `T` Exception, `ConsoleColor` Color, `Int32` BreakLines = 1) |  | 
| `T` | ConsoleWriteError(this `T` Exception, `Int32` BreakLines) |  | 
| `String` | ConsoleWriteLine(this `String` Text, `Dictionary<String, ConsoleColor>` CustomColoredWords, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWriteLine(this `String` Text, `ConsoleColor` Color, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWriteLine(this `String` Text, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWriteSeparator(`Char` Separator = -, `Nullable<ConsoleColor>` Color = null, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWriteSeparator(this `String` Text, `Char` Separator = -, `Nullable<ConsoleColor>` Color = null, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWriteTitle(this `String` Text, `Nullable<ConsoleColor>` Color = null, `Int32` BreakLines = 1) |  | 
| `String` | ConsoleWriteTitleBar(this `String` Text, `Nullable<ConsoleColor>` Color = null, `Int32` BreakLines = 1, `Char` BarChar = -) |  | 
| `String` | GetArgumentValue(this `String[]` args, `String` ArgName, `String` ValueIfNull = null) |  | 
| `T` | GetArgumentValue(this `String[]` args, `String` ArgName, `T` ValueIfNull = null) |  | 
| `Char` | ReadChar(this `Char&` Char) |  | 
| `ConsoleKey` | ReadConsoleKey(this `ConsoleKey&` Key) |  | 


