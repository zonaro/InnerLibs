## `MixedTemplatePage<T>`

Classe que mescla paginas de um mesmo tipo de template. Particulamente util para união de  diferentes resultados filtrados de um mesmo tipo de objeto
```csharp
public class InnerLibs.Triforce.MixedTemplatePage<T>
    : List<Template<T>>, IList<Template<T>>, ICollection<Template<T>>, IEnumerable<Template<T>>, IEnumerable, IList, ICollection, IReadOnlyList<Template<T>>, IReadOnlyCollection<Template<T>>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Empty | HTML retornado quando não houver itens na lista ou na página atual | 
| `String` | Footer | html adicionado após os template | 
| `String` | Head | Html adcionado antes do template | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlDocument` | BuildHtml() | Retorna o HTML da pagina atual da lista de templates | 
| `String` | ToString() | Retorna o HTML da pagina atual da lista de templates | 


## `Template<T>`

Estrutura de template do Triforce
```csharp
public class InnerLibs.Triforce.Template<T>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `CultureInfo` | Culture |  | 
| `T` | Data | Objeto de onde são extraidos as informações do template | 
| `HtmlDocument` | ProcessedTemplate | Template processado | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() |  | 


## `TemplatePage<T>`

Lista paginada contendo `InnerLibs.Triforce.Template`1` previamente processados
```csharp
public class InnerLibs.Triforce.TemplatePage<T>
    : ReadOnlyCollection<Template<T>>, IList<Template<T>>, ICollection<Template<T>>, IEnumerable<Template<T>>, IEnumerable, IList, ICollection, IReadOnlyList<Template<T>>, IReadOnlyCollection<Template<T>>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Empty | HTML retornado quando não houver itens na lista ou na página atual | 
| `String` | Footer | html adicionado após os template | 
| `String` | Head | Html adcionado antes do template | 
| `Int32` | PageCount | Numero de Paginas deste template | 
| `Int32` | PageNumber | Pagina atual. Corresponde ao grupo de itens que foram processados | 
| `Int32` | PageSize | Numero de Itens por pagina | 
| `String` | Pagination | HTML da paginaçao dos itens | 
| `String` | PaginationUrlTemplate | Template aplicado a URL de paginação | 
| `Int32` | Total | Total de Itens encontrados na `System.Linq.IQueryable` ou `System.Collections.IEnumerable` | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `HtmlDocument` | BuildHtml() | Retorna o HTML da pagina atual da lista de templates | 
| `String` | ToString() | Retorna o HTML da pagina atual da lista de templates | 


## `TemplatePropertySelector`

Classe que permite pegar propriedades de outras classes durante o processamento de um template
```csharp
public class InnerLibs.Triforce.TemplatePropertySelector

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | Obj |  | 
| `String` | PropertyString |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | Proccess() | Retorna o valor daquela propriedade do objeto definido | 


## `TemplateTag`

Lista com as TemplateTags
```csharp
public enum InnerLibs.Triforce.TemplateTag
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | Head | Cabeçalho do template, aplicado uma vez antes do body | 
| `1` | Body | Corpo do template, replicado para cada objeto | 
| `2` | Footer | Rodapé do template, aplicado uma vez apos o body | 
| `3` | Pagination | template de paginacao | 
| `4` | Empty | Placeholder aplicado no lugar do body quando a lista não conter resultados | 


## `Triforce`

Gerador de HTML dinâmico a partir de objetos LINQ e arquivos HTML.
```csharp
public class InnerLibs.Triforce.Triforce

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Assembly` | ApplicationAssembly | Aplicaçao contendo os Resources (arquivos compilados internamente) dos arquivos HTML e  SQL utilizados como template | 
| `CultureInfo` | Culture | Especifica a cultura utilizda para este Triforce | 
| `Dictionary<String, TemplatePropertySelector>` | CustomProperties | Propriedades retiradas diretamente de um objeto com indexadores durante o processamento  do template. Particulamente util para propriedades de objetos que nao vem do banco, mas  que necessitam de um parametro que vem do banco | 
| `Dictionary<String, Object>` | CustomValues | Valores adicionados ao processamento do template que não vem do banco de dados ou do  objeto. Particulamente Util para dados de sessão. | 
| `String[]` | Selectors | Seletores de Template. | 
| `DirectoryInfo` | TemplateDirectory | Pasta contendo os arquivos HTML e SQL utilizados como template | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Template<Dictionary<Int32, Object>>` | ApplyArrayTemplate(`String` Template, `Object[]` Items) | Aplica um array de objetos em um template e retorna um `InnerLibs.Triforce.Template`1`  do resultado | 
| `Template<T>` | ApplyTemplate(`T` Item, `String` Template = ) | Aplica um template HTML a um unico objeto | 
| `TemplatePage<T>` | ApplyTemplate(`IEnumerable<T>` List, `Int32` PageNumber = 1, `Int32` PageSize = 0, `String` Template = ) | Aplica um template HTML a um unico objeto | 
| `String` | ApplyValues(`String` StringToApply, `IDictionary<String, Object>` Values) | Aplica valores de um dicionário a uma string com marcações de template | 
| `String` | ClearNotFoundValues(`String` StringToClear) | Limpa dos resultados dos templates as chaves que não foram encontradas | 
| `String` | CreatePaginationUrlTemplate(`String` Url, `String[]` FilterParams) | Cria um template de URL a partir de uma url base e parâmetros especificos | 
| `Uri` | CreateUrl(`T` Obj, `String` UrlTemplate) | Processa a uma string URL com marcaçoes de template e retorna uma URI | 
| `String` | GetTemplate(`Boolean` ProcessFile = False) | Retorna o nome do arquivo de template, ou o template processado se ProccessFile estiver True | 
| `String` | GetTemplate(`Type` Type, `Boolean` ProcessFile = False) | Retorna o nome do arquivo de template, ou o template processado se ProccessFile estiver True | 
| `String` | GetTemplateContent(`String` TemplateFile, `TemplateTag` Tag) | Retorna o conteudo estático de um arquivo de template | 


## `TriforceDateTimeFormat`

Atributo de Configuraçao do formato de data e hora. Se aplica a propriedade especificada ou a  classe toda
```csharp
public class InnerLibs.Triforce.TriforceDateTimeFormat
    : Attribute, _Attribute

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Format |  | 


## `TriforceDefaultTemplate`

Atributo de Configuraçao do Template. Aplica-se a classes de entidade
```csharp
public class InnerLibs.Triforce.TriforceDefaultTemplate
    : Attribute, _Attribute

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Template |  | 


