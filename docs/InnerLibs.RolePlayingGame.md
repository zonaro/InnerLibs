## `Dice`

Dado de RPG
```csharp
public class InnerLibs.RolePlayingGame.Dice

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DiceFace` | Face | Retorna a face correspondente ao numero | 
| `ReadOnlyCollection<DiceFace>` | Faces | Faces do dado | 
| `IEnumerable<ValueTuple<Int32, DateTime>>` | History | Historico de valores rolados para este dado | 
| `Boolean` | IsCustom | Indica se o dado é um dado com faces customizadas | 
| `Boolean` | IsVicious | Verifica se o dado possui algum lado viciado | 
| `Nullable<DateTime>` | LastRoll |  | 
| `Boolean` | Locked | Se TRUE, Impede este dado de ser rolado | 
| `Int32` | RolledTimes | Numero de vezes que este dado já foi rolado | 
| `DiceType` | Type | Tipo do dado | 
| `Nullable<Int32>` | Value | Valor atual deste dado | 
| `Decimal` | Weight | Peso do dado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Flip() | Se este Dice for uma moeda (2 lados apenas) retorna true ou false baseado no lado da moeda qua saiu, caso seja um dado com mais de 2 lados retorna sempre true | 
| `DiceFace` | GetFace(`Int32` FaceNumber) | Retorna a face correspondente ao numero | 
| `void` | LoadHistory(`IEnumerable<ValueTuple<Int32, DateTime>>` history) |  | 
| `void` | NormalizeWeight(`Decimal` Weight = 1) | Normaliza o peso das faces do dado | 
| `DiceFace` | Roll() | Rola o dado e retorna seu valor | 


## `DiceRoller`

Combinação de varios dados de RPG que podem ser rolados ao mesmo tempo
```csharp
public class InnerLibs.RolePlayingGame.DiceRoller
    : List<Dice>, IList<Dice>, ICollection<Dice>, IEnumerable<Dice>, IEnumerable, IList, ICollection, IReadOnlyList<Dice>, IReadOnlyCollection<Dice>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Value | Retorna a soma de todos os valores dos dados | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<DiceFace>` | Roll() | Rola todos os dados (não travados) e retorna a soma de seus valores | 


## `DiceType`

Tipos de Dados
```csharp
public enum InnerLibs.RolePlayingGame.DiceType
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


