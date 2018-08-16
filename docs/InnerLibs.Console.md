## `Console`

Métodos para manipulação de aplicações baseadas em Console (System.Console)
```csharp
public class InnerLibs.Console.Console

```

Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Title | Titulo da janela do console | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Beep(`Int32` Times = 1) | Toca um Beep | 
| `void` | Beep(`Int32` Frequency, `Int32` Duration) | Toca um Beep | 
| `void` | BreakLine(`Int32` Lines = 1) | Pula uma ou mais linhas no console | 
| `void` | Clear() | Limpa a tela do console | 
| `Char` | Read() | Le o proximo caractere inserido no console pelo usuário | 
| `ConsoleKey` | ReadKey() | Le a proxima tecla pressionada pelo usuário | 
| `String` | ReadLine() | Le a proxima linha inserida no console pelo usuário | 
| `void` | Write(`String` Text, `TextValueList<ConsoleColor>` CustomColoredWords) | Escreve no console colorindo palavras especificas | 
| `void` | Write(`String` Text, `ConsoleColor` Color = White) | Escreve no console colorindo palavras especificas | 
| `void` | WriteLine(`String` Text, `TextValueList<ConsoleColor>` CustomColoredWords) | Escreve uma linha no console colorindo palavras especificas | 
| `void` | WriteLine(`String` Text, `ConsoleColor` Color = White) | Escreve uma linha no console colorindo palavras especificas | 


