## `Alternative`

Objeto que representa uma alternativa de uma pergunta de alternativas
```csharp
public class InnerLibs.QuestionTest.Alternative

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `AlternativeQuestion` | _question |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | Checked | Valor que indica se esta alternativa foi assinalada | 
| `Boolean` | Correct | Valor que indica se a alternativa está correta ou verdadeira | 
| `String` | HTML |  | 
| `String` | ID | ID da alternativa | 
| `Boolean` | IsCorrect | Verifica se a resposta do usuário é correta para esta alternativa | 
| `Int32` | Number | O numero da alternativa | 
| `AlternativeQuestion` | Question |  | 
| `String` | Text | Texto da alternativa | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `AlternativeList`

Lista de Alternativas de uma questão de alternativas
```csharp
public class InnerLibs.QuestionTest.AlternativeList
    : ObservableCollection<Alternative>, IList<Alternative>, ICollection<Alternative>, IEnumerable<Alternative>, IEnumerable, IList, ICollection, IReadOnlyList<Alternative>, IReadOnlyCollection<Alternative>, INotifyCollectionChanged, INotifyPropertyChanged

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | HTML |  | 
| `AlternativeQuestion` | Question |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`String` Text, `Boolean` Correct) | Adiciona uma alternativa a questão. A alternativa é ignorada se já existir na lista | 
| `void` | AddRange(`IEnumerable<Alternative>` Alternatives) |  | 
| `void` | OnCollectionChanged(`NotifyCollectionChangedEventArgs` e) |  | 
| `String` | ToString() |  | 


## `AlternativeQuestion`

Classe base para questões de 'alternativa' ou de 'verdadeiro ou falso'
```csharp
public abstract class InnerLibs.QuestionTest.AlternativeQuestion
    : Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Boolean` | AllowMultiple | Verifica se esta pergunta permite multiplas alternativas | 
| `AlternativeList` | Alternatives | Lista de alternativas da questão | 
| `IEnumerable<Alternative>` | Answer | Retorna as alternativas marcadas pelo usuário | 
| `Boolean` | RenderAsSelect | Indica se esta alternativa deve ser renderizada no HTML como um `InnerLibs.HtmlParser.HtmlSelectElement`. Caso Contrario, serão renderizadas como listas de Check Box ou Radio Button | 


## `DissertativeQuestion`

Questão Dissertativa. Deve ser corrigida manualmente
```csharp
public class InnerLibs.QuestionTest.DissertativeQuestion
    : Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Answer | Resposta dissertativa da pergunta | 
| `Decimal` | Assertiveness | Assertividade da questão, uma valor entre 0 e o peso da questão que representa o quanto esta questão está correta | 
| `Boolean` | Correct | Valor que indica se a questão está de alguma forma correta | 
| `Decimal` | Hits | Representa quantos pontos essa questão vale de acordo com a assertividade | 
| `String` | HTML |  | 
| `Boolean` | IsCorrect | Verifica se a pergunta está preenchida | 
| `Int32` | Lines | Numero de linhas que devem ser impressas para esta questão | 


## `MultipleAlternativeQuestion`

Pergunta de Verdadeiro ou Falso. O Usuário deverá assinalar as questões verdadeiras ou falsas correspondente ao enunciado da pergunta.
```csharp
public class InnerLibs.QuestionTest.MultipleAlternativeQuestion
    : AlternativeQuestion

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Hits | Retorna um numero que representa o quanto o usuario acertou essa pergunta | 
| `String` | HTML |  | 
| `Boolean` | IsCorrect | Verifica se a pergunta está corretamente assinalada | 


## `NumericQuestion`

Questões em que a resposta é numerica e implica diretamente no peso da questão (normalmente utilizada em pesquisas)
```csharp
public class InnerLibs.QuestionTest.NumericQuestion
    : Question

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Answer | Pontos que o usuario atribuiu a esta questão | 
| `Decimal` | Hits | Pontos multiplicados pelo peso da questão | 
| `String` | HTML |  | 
| `Boolean` | IsCorrect | Perguntas numericas sempre estão corretas. Neste caso, o que vale é a resposta multiplicada pelo peso que implica diretamente no peso da avaliação | 
| `Decimal` | MaxValue | Maior valor permitido pela questão | 
| `Decimal` | MinValue | Menor valor permitido pela questão | 


## `Question`

Classe Base para as questões de uma avaliação
```csharp
public abstract class InnerLibs.QuestionTest.Question

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `QuestionStatement` | _statement |  | 
| `QuestionTest` | _test |  | 
| `Decimal` | _weight |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Hits | Retorna um numero que representa o quanto o usuario acertou essa pergunta | 
| `String` | HTML |  | 
| `String` | ID | O codigo de identificação desta questão | 
| `Boolean` | IsCorrect | Verifica se a pergunta está corretamente assinalada | 
| `Int32` | Number | Numero da questão | 
| `String` | QuestionType | Tipo da QUestão | 
| `Boolean` | Reviewed | Indica se esta questão foi revisada pelo professor | 
| `QuestionStatement` | Statement | Enunciado da questão (texto da pergunta) | 
| `QuestionTest` | Test | Teste a qual esta questão pertence | 
| `Decimal` | Weight | Peso da Pergunta | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Return the statment text for this question | 


## `QuestionEditor`

```csharp
public class InnerLibs.QuestionTest.QuestionEditor
    : EnhancedCollectionEditor

```

## `QuestionStatement`

Enunciado de uma pergunta
```csharp
public class InnerLibs.QuestionTest.QuestionStatement

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `Question` | _question |  | 
| `String` | _text |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | HTML |  | 
| `StatementImages` | Images | Imagens adicionadas ao enunciado (com legenda) | 
| `Question` | Question |  | 
| `String` | Text | Texto do enunciado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `QuestionTest`

Classe que representa uma Avaliação de Perguntas e respostas, podendo elas serem Dissertativas, Multipla Escolha ou de Atribuição de Pontos
```csharp
public class InnerLibs.QuestionTest.QuestionTest
    : ObservableCollection<Question>, IList<Question>, ICollection<Question>, IEnumerable<Question>, IEnumerable, IList, ICollection, IReadOnlyList<Question>, IReadOnlyCollection<Question>, INotifyCollectionChanged, INotifyPropertyChanged

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Average | Média da Avaliação | 
| `Decimal` | Bonus | Pontos de bonificação que serão somados a média final da avaliação | 
| `Int32` | FailPercent | Porcentagem de Erros do Usuário | 
| `Int32` | Fails | Numero de questões que o usuário errou | 
| `Decimal` | FinalNote | Nota final da avaliação (Bonus + Média) | 
| `String` | Footer | Rodapé da prova. Texto adicional que ficará após as questões | 
| `String` | Header | Cabeçalho da prova. Texto adicional que ficará antes das questões e apoós o título | 
| `Int32` | HitPercent | Porcentagem de Acertos do Usuário | 
| `Int32` | Hits | Numero de questões que o usuário acertou | 
| `HtmlDocument` | HTML | Monta uma prova HTML | 
| `Object` | IsApproved | Retorna TRUE se a nota final (média da avaliação + os bonus) é maior ou igual ao minimo permitido, caso contrário, FALSE | 
| `Boolean` | IsValid | Verifica se o peso da prova equivale a soma dos pesos das questões | 
| `Decimal` | MinimumWeightAllowed | Valor Minimo da nota para aprovação (Normalmente 6) | 
| `QuestionTest` | Questions | Retorna as questões desta avaliação | 
| `String` | Title | Titulo da Avaliação | 
| `Decimal` | Weight | Peso da Avaliação (Normalmente 10) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `QuestionType` | CreateQuestion() | Adiciona uma nova questão a avaliação. | 
| `Alternative` | GetAlternative(`String` ID) | Pega uma Alternativa de uma Questão pelo ID | 
| `T` | GetQuestion(`String` ID) | Pega uma questão por ID | 
| `void` | OnCollectionChanged(`NotifyCollectionChangedEventArgs` e) |  | 
| `void` | SetMinimumAllowedAsHalf(`Decimal` Weight = 0) | Configura o valor minimo permitido para aprovação como metade do peso da avaliação | 
| `void` | SetMinimumAllowedAsPercent(`String` Percent, `Decimal` Weight = 0) | Configura o valor minimo permitido para aprovação como n% do peso da avaliação | 
| `String` | ToString() |  | 


## `SingleAlternativeQuestion`

Pergunta de alternativa. o Usuário deverá assinalar a UNICA alternativa correta entre varias alternativas
```csharp
public class InnerLibs.QuestionTest.SingleAlternativeQuestion
    : AlternativeQuestion

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Decimal` | Hits | Retorna um numero que representa o quanto o usuario acertou essa pergunta | 
| `String` | HTML |  | 
| `Boolean` | IsCorrect | Verifica se a pergunta está corretamente assinalada. Anula a questão automaticamente se estiver mal formada (com mais de uma alternativa correta ou nenhuma alternativa correta) | 
| `Boolean` | IsValidQuestion | Verifica se as existe apenas uma unica alternativa correta na questão | 


## `StatementImage`

Imagem com legenda de um enunciado
```csharp
public class InnerLibs.QuestionTest.StatementImage

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | HTML |  | 
| `Image` | Image | Imagem do enunciado | 
| `StatementImages` | StatementImages |  | 
| `String` | Subtitle | Legenda da Imagem | 


## `StatementImages`

Imagens adicionada a um enuncidado
```csharp
public class InnerLibs.QuestionTest.StatementImages
    : List<StatementImage>, IList<StatementImage>, ICollection<StatementImage>, IEnumerable<StatementImage>, IEnumerable, IList, ICollection, IReadOnlyList<StatementImage>, IReadOnlyCollection<StatementImage>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | HTML |  | 
| `QuestionStatement` | Statement |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Add(`Image` Image, `String` Subtitle = ) |  | 
| `void` | Add(`String` ImagePath, `String` Subtitle = ) |  | 
| `String` | ToString() |  | 


