## `Dice`

```csharp
public class InnerLibs.RolePlayingGame.Dice

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `ReadOnlyCollection<DiceFace>` | Faces |  | 
| `IEnumerable<ValueTuple<Int32, DateTime>>` | History |  | 
| `Boolean` | IsCustom |  | 
| `Boolean` | IsVicious |  | 
| `DiceFace` | Item |  | 
| `Nullable<DateTime>` | LastRoll |  | 
| `Boolean` | Locked |  | 
| `Int32` | RolledTimes |  | 
| `String` | TextValue |  | 
| `DiceType` | Type |  | 
| `Nullable<Int32>` | Value |  | 
| `Decimal` | Weight |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Flip(`Int32` Times = 1) |  | 
| `Decimal` | GetChancePercent(`Int32` Face, `Int32` Precision = 2) |  | 
| `DiceFace` | GetFace(`Int32` FaceNumber = 0) |  | 
| `Decimal` | GetValueOfPercent(`Int32` Face, `Int32` Precision = 2) |  | 
| `void` | LoadHistory(`IEnumerable<ValueTuple<Int32, DateTime>>` history) |  | 
| `void` | NormalizeWeight(`Decimal` Weight = 1) |  | 
| `DiceFace` | Roll(`Int32` Times = 1) |  | 
| `Dice` | SetFaceName(`Int32` FaceNumber, `String` Name) |  | 
| `String` | ToString() |  | 


Static Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dice` | Coin |  | 
| `Dice` | D6 |  | 


## `DiceRoller`

```csharp
public class InnerLibs.RolePlayingGame.DiceRoller
    : List<Dice>, IList<Dice>, ICollection<Dice>, IEnumerable<Dice>, IEnumerable, IList, ICollection, IReadOnlyList<Dice>, IReadOnlyCollection<Dice>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int32` | Value |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<DiceFace>` | Roll(`Int32` Times = 1) |  | 


## `DiceType`

```csharp
public enum InnerLibs.RolePlayingGame.DiceType
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Custom |  | 
| `2` | Coin |  | 
| `4` | D4 |  | 
| `6` | D6 |  | 
| `8` | D8 |  | 
| `10` | D10 |  | 
| `12` | D12 |  | 
| `20` | D20 |  | 
| `100` | D100 |  | 


