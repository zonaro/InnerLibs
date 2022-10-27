## `Alternative`

```csharp
public class InnerLibs.QuestionTest.Alternative

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Checked |  | 
| `Boolean` | Correct |  | 
| `String` | ID |  | 
| `Boolean` | IsCorrect |  | 
| `Int32` | Number |  | 
| `AlternativeQuestion` | Question |  | 
| `String` | Text |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `AlternativeList`

```csharp
public class InnerLibs.QuestionTest.AlternativeList
    : ObservableCollection<Alternative>, IList<Alternative>, ICollection<Alternative>, IEnumerable<Alternative>, IEnumerable, IList, ICollection, IReadOnlyList<Alternative>, IReadOnlyCollection<Alternative>, INotifyCollectionChanged, INotifyPropertyChanged

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `AlternativeQuestion` | Question |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` Text, `Boolean` Correct) |  | 
| `void` | AddRange(`IEnumerable<Alternative>` Alternatives) |  | 
| `String` | ToString() |  | 


## `AlternativeQuestion`

```csharp
public abstract class InnerLibs.QuestionTest.AlternativeQuestion
    : Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AllowMultiple |  | 
| `AlternativeList` | Alternatives |  | 
| `IEnumerable<Alternative>` | Answer |  | 
| `Boolean` | RenderAsSelect |  | 


## `DissertativeQuestion`

```csharp
public class InnerLibs.QuestionTest.DissertativeQuestion
    : Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Answer |  | 
| `Decimal` | Assertiveness |  | 
| `Boolean` | Correct |  | 
| `Decimal` | Hits |  | 
| `Boolean` | IsCorrect |  | 
| `Int32` | Lines |  | 


## `MultipleAlternativeQuestion`

```csharp
public class InnerLibs.QuestionTest.MultipleAlternativeQuestion
    : AlternativeQuestion

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Hits |  | 
| `Boolean` | IsCorrect |  | 


## `NumericQuestion`

```csharp
public class InnerLibs.QuestionTest.NumericQuestion
    : Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Answer |  | 
| `Decimal` | Hits |  | 
| `Boolean` | IsCorrect |  | 
| `Decimal` | MaxValue |  | 
| `Decimal` | MinValue |  | 


## `Question`

```csharp
public abstract class InnerLibs.QuestionTest.Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Hits |  | 
| `String` | ID |  | 
| `Boolean` | IsCorrect |  | 
| `Int32` | Number |  | 
| `String` | QuestionType |  | 
| `Boolean` | Reviewed |  | 
| `QuestionStatement` | Statement |  | 
| `QuestionTest` | Test |  | 
| `Decimal` | Weight |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `QuestionStatement`

```csharp
public class InnerLibs.QuestionTest.QuestionStatement

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `StatementImages` | Images |  | 
| `Question` | Question |  | 
| `String` | Text |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `QuestionTest`

```csharp
public class InnerLibs.QuestionTest.QuestionTest
    : ObservableCollection<Question>, IList<Question>, ICollection<Question>, IEnumerable<Question>, IEnumerable, IList, ICollection, IReadOnlyList<Question>, IReadOnlyCollection<Question>, INotifyCollectionChanged, INotifyPropertyChanged

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Average |  | 
| `Decimal` | Bonus |  | 
| `Decimal` | FailPercent |  | 
| `Int32` | Fails |  | 
| `Decimal` | FinalNote |  | 
| `String` | Footer |  | 
| `String` | Header |  | 
| `Decimal` | HitPercent |  | 
| `Int32` | Hits |  | 
| `String` | ID |  | 
| `Boolean` | IsApproved |  | 
| `Boolean` | IsValid |  | 
| `Decimal` | MinimumWeightAllowed |  | 
| `Dictionary<String, Object>` | PersonalInfo |  | 
| `QuestionTest` | Questions |  | 
| `String` | Title |  | 
| `Decimal` | Weight |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `QuestionType` | CreateQuestion() |  | 
| `Alternative` | GetAlternative(`String` ID) |  | 
| `T` | GetQuestion(`String` ID) |  | 
| `void` | SetMinimumAllowedAsHalf(`Decimal` Weight = 0) |  | 
| `void` | SetMinimumAllowedAsPercent(`String` Percent, `Decimal` Weight = 0) |  | 
| `String` | ToString() |  | 


## `SingleAlternativeQuestion`

```csharp
public class InnerLibs.QuestionTest.SingleAlternativeQuestion
    : AlternativeQuestion

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Hits |  | 
| `Boolean` | IsCorrect |  | 
| `Boolean` | IsValidQuestion |  | 


## `StatementImage`

```csharp
public class InnerLibs.QuestionTest.StatementImage

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | HTML |  | 
| `Image` | Image |  | 
| `StatementImages` | StatementImages |  | 
| `String` | Subtitle |  | 


## `StatementImages`

```csharp
public class InnerLibs.QuestionTest.StatementImages
    : List<StatementImage>, IList<StatementImage>, ICollection<StatementImage>, IEnumerable<StatementImage>, IEnumerable, IList, ICollection, IReadOnlyList<StatementImage>, IReadOnlyCollection<StatementImage>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `QuestionStatement` | Statement |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`Image` Image, `String` Subtitle = ) |  | 
| `void` | Add(`String` ImagePath, `String` Subtitle = ) |  | 
| `String` | ToString() |  | 


