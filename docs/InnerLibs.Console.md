## `ConsoleExtensions`

Métodos para manipulação de aplicações baseadas em Console (System.Console)
```csharp
public class InnerLibs.Console.ConsoleExtensions

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Title | Titulo da janela do console | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Beep(`Int32` Times = 1) | Toca um Beep | 
| `void` | Beep(`Int32` Frequency, `Int32` Duration, `Int32` Times = 1) | Toca um Beep | 
| `void` | ConsoleBreakLine(`Int32` Lines = 1) | Pula uma ou mais linhas no console | 
| `String` | ConsoleBreakLine(this `String` Text, `Int32` Lines = 1) | Pula uma ou mais linhas no console | 
| `String` | ConsoleWrite(this `String` Text, `Dictionary<String, ConsoleColor>` CustomColoredWords, `Int32` Lines = 0) | Escreve no console colorindo palavras especificas | 
| `String` | ConsoleWrite(this `String` Text, `Dictionary<String, ConsoleColor>` CustomColoredWords, `StringComparison` Comparison, `Int32` BreakLines = 0) | Escreve no console colorindo palavras especificas | 
| `String` | ConsoleWrite(this `String` Text, `Int32` BreakLines = 0) | Escreve no console colorindo palavras especificas | 
| `String` | ConsoleWrite(this `String` Text, `ConsoleColor` Color, `Int32` BreakLines = 0) | Escreve no console colorindo palavras especificas | 
| `T` | ConsoleWriteError(this `T` Exception, `String` Message, `ConsoleColor` Color = Red, `Int32` Lines = 1) | Escreve o texto de uma exception no console | 
| `T` | ConsoleWriteError(this `T` Exception, `ConsoleColor` Color = Red, `Int32` Lines = 1) | Escreve o texto de uma exception no console | 
| `String` | ConsoleWriteLine(this `String` Text, `Dictionary<String, ConsoleColor>` CustomColoredWords, `Int32` Lines = 1) | Escreve uma linha no console colorindo palavras especificas | 
| `String` | ConsoleWriteLine(this `String` Text, `ConsoleColor` Color, `Int32` Lines = 1) | Escreve uma linha no console colorindo palavras especificas | 
| `String` | ConsoleWriteLine(this `String` Text, `Int32` Lines = 1) | Escreve uma linha no console colorindo palavras especificas | 
| `Char` | ReadChar() | Le o proximo caractere inserido no console pelo usuário | 
| `ConsoleKey` | ReadKey() | Le a proxima tecla pressionada pelo usuário | 
| `String` | ReadLine() | Le a proxima linha inserida no console pelo usuário | 


