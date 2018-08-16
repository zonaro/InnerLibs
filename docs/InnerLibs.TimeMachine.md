## `Cronometer`

```csharp
public class InnerLibs.TimeMachine.Cronometer
    : Timer, IComponent, IDisposable

```

Fields

| Type | Name | Summary | 
| --- | --- | --- | 
| `Int64` | _value |  | 


Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Format | Formato do Cronometro | 
| `List<TimeSpan>` | Laps | Lista de `System.DateTime` dos valores de cada Lap | 
| `TimeFlow` | ToTimeFlow | Retorna um `InnerLibs.TimeMachine.TimeFlow` calculado para este cronometro. | 
| `TimeSpan` | Value | Retorna o valor atual do cronometro | 


Events

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnChangeEventHandler` | OnChange | Ocorre toda vez que o cronometro iniciar, mudar de valor, parar, reiniciar ou marcar uma volta | 
| `EventHandler` | OnLap | ocorre toda vez que a função `InnerLibs.TimeMachine.Cronometer.Lap` é chamada | 
| `EventHandler` | OnReset | Ocorre toda vez que o cronometro Reinicia `InnerLibs.TimeMachine.Cronometer.Reset`) | 
| `EventHandler` | OnStart | Ocorre toda vez que o cronometro inicia ( `InnerLibs.TimeMachine.Cronometer.Start`) | 
| `EventHandler` | OnStop | Ocorre toda vez que o cronometro para ( `InnerLibs.TimeMachine.Cronometer.Stop`) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<String>` | GetLaps(`String` Format = ) | Retorna uma lista de strings da extraidas da `InnerLibs.TimeMachine.Cronometer.Laps` em um formato  especifico de data | 
| `void` | IncrementTick(`Object` sender, `EventArgs` e) |  | 
| `void` | Lap() | Marca um valor no cronometro | 
| `void` | Reset() | Limpa os valores do cronometro | 
| `void` | Start() | Inicia o cronometro | 
| `void` | StartOver() | Renicia o cronometro. é o equivalente em chamar `InnerLibs.TimeMachine.Cronometer.Reset` e `InnerLibs.TimeMachine.Cronometer.Start` | 
| `void` | Stop() | Para o cronometro | 
| `String` | ToString() | Texto atual do cronometro | 
| `String` | ToString(`String` Format) | Texto atual do cronometro | 


## `Day`

Dia de Uma Demanda
```csharp
public class InnerLibs.TimeMachine.Day

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | EndHour | Hora que se encerra a jornada (inclui hora de almoço) | 
| `TimeSpan` | JourneyTime | Jornada de Trabalho/Produção | 
| `DateTime` | LunchHour | Hora de almoco | 
| `TimeSpan` | LunchTime | Hora de Almoço | 
| `DateTime` | StartHour | Hora inicial da jornada | 
| `TimeSpan` | TotalTime | Jornada + hora de Almoço | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | SetJourney(`DateTime` StartHour, `TimeSpan` Journey, `DateTime` LunchHour = 01/01/0001 12:00:00 AM, `TimeSpan` LunchTime = null) | Define a hora inicial e a jornada de trabalho deste dia | 


## `Fortnight`

```csharp
public class InnerLibs.TimeMachine.Fortnight

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Key | String que identifica a quinzena em uma coleção | 
| `Int32` | Number | Numero da quinzena (1 ou 2) | 
| `DateRange` | Period | Periodo que esta quinzena possui | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | FormatName(`String` Format = {q}{o} - {mmmm}/{yyyy}) | Retorna a Key de um `InnerLibs.TimeMachine.Fortnight` em um formato especifico. | 
| `String` | ToString() |  | 


## `FortnightGroup`

Lista de dias agrupados em quinzenas
```csharp
public class InnerLibs.TimeMachine.FortnightGroup
    : ReadOnlyCollection<Fortnight>, IList<Fortnight>, ICollection<Fortnight>, IEnumerable<Fortnight>, IEnumerable, IList, ICollection, IReadOnlyList<Fortnight>, IReadOnlyCollection<Fortnight>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<DateTime>` | AllDays | Retorna uma lista com todos os dias entre as quinzenas | 
| `DateTime` | EndDate | Retorna a ultima data do periodo | 
| `Fortnight` | Item | Retorna uma quinzena a partir da sua Key | 
| `Fortnight` | Item | Retorna uma quinzena a partir da sua Key | 
| `DateRange` | Period | Retorna um periodo equivalente a este grupo de quinzena | 
| `DateTime` | StartDate | Retorna a data inicial do periodo | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FortnightGroup` | CreateFromDateRange(`DateTime` StartDate, `DateTime` EndDate) | Cria um grupo de quinzenas entre 2 datas | 
| `FortnightGroup` | CreateFromDateRange(`DateRange` Range) | Cria um grupo de quinzenas entre 2 datas | 


## `FortnightGroup<DataType>`

```csharp
public class InnerLibs.TimeMachine.FortnightGroup<DataType>
    : FortnightGroup, IList<Fortnight>, ICollection<Fortnight>, IEnumerable<Fortnight>, IEnumerable, IList, ICollection, IReadOnlyList<Fortnight>, IReadOnlyCollection<Fortnight>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<DataType>` | DataCollection |  | 
| `List<Func<DataType, DateTime>>` | DateSelector |  | 
| `IEnumerable<DataType>` | GetData | Retorna da `InnerLibs.TimeMachine.FortnightGroup`1.DataCollection` os valores correspondentes a quinzena especificada | 
| `IEnumerable<DataType>` | GetData | Retorna da `InnerLibs.TimeMachine.FortnightGroup`1.DataCollection` os valores correspondentes a quinzena especificada | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Dictionary<Fortnight, IEnumerable<DataType>>` | ToDataDictionary(`Boolean` IncludeFortnightsWithoutData = True) | Retorna um `System.Collections.Generic.Dictionary`2` com as informaçoes agrupadas por quinzena | 
| `Object` | ToJSON(`Boolean` IncludeFortnightsWithoutData = True) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FortnightGroup<DataType>` | CreateFromDataGroup(`IEnumerable<DataType>` Data, `DateRange` Range, `Func`2[]` DateSelector) | Cria um `InnerLibs.TimeMachine.FortnightGroup`1` a partir de uma coleção de objetos | 
| `FortnightGroup<DataType>` | CreateFromDataGroup(`IEnumerable<DataType>` Data, `Func`2[]` DateSelector) | Cria um `InnerLibs.TimeMachine.FortnightGroup`1` a partir de uma coleção de objetos | 
| `FortnightGroup<DataType>` | CreateFromDateRange(`DateTime` StartDate, `DateTime` EndDate) | Cria um `InnerLibs.TimeMachine.FortnightGroup`1` a partir de uma data inicial e uma data final | 


## `Item`

Item de Uma demanda
```csharp
public class InnerLibs.TimeMachine.Item

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | MultipleItem | string que representa o item quando sua quantidade é maior que 1 | 
| `TimeSpan` | ProductionTime | Tempo totald e produção de todos os itens | 
| `Int32` | Quantity | Quantidade de itens | 
| `String` | SingularItem | String que representa o item quando sua quantidade é 1 | 
| `TimeSpan` | Time | Tempo de produção de 1 item | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Retorna uma string que representa a quantidade do item | 


## `LapEventArgs`

```csharp
public class InnerLibs.TimeMachine.LapEventArgs
    : EventArgs

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | DateValue |  | 
| `TimeSpan` | Value |  | 


## `Stopwatch`

```csharp
public class InnerLibs.TimeMachine.Stopwatch
    : Cronometer, IComponent, IDisposable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `TimeSpan` | InitialTime | Valor inicial do contador | 
| `TimeSpan` | Value | Retorna o valor atual do contador | 


Events

| Type | Name | Summary | 
| --- | --- | --- | 
| `EventHandler` | OnFinish | Ocorre toda vez que o contador chegar a zero | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Texto atual do contador | 
| `String` | ToString(`String` Format) | Texto atual do contador | 


## `TimeDemand`

Classe base para calculo de demandas
```csharp
public class InnerLibs.TimeMachine.TimeDemand

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | EndDate | Data de encerramento da produção | 
| `Day` | Friday | Sexta-Feira | 
| `List<DateTime>` | HoliDays | Feriados, pontos facuultativos e/ou datas especificas consideradas não relevantes | 
| `Item` | Item | item da Produção | 
| `Day` | Monday | Segunda-Feira | 
| `List<DateTime>` | NonRelevantDays | Dias não relevantes (nao letivos e feriados) entre as datas inicial e final | 
| `IEnumerable<DayOfWeek>` | NonRelevantDaysOfWeek | Dias da semana não relevantes | 
| `List<DateTime>` | RelevantDays | Dias relevantes (letivos) entre as datas inicial e final | 
| `IEnumerable<DayOfWeek>` | RelevantDaysOfWeek | Dias da semana relevantes | 
| `Day` | Saturday | Sábado | 
| `DateTime` | StartDate | Data Inicial da produção | 
| `Day` | Sunday | Domingo | 
| `Day` | Thursday | Quinta-Feira | 
| `Day` | Tuesday | Terça-Feira | 
| `Day` | Wednesday | Quarta-Feira | 
| `List<DateTime>` | WorkDays | Dias especificos da semana entre as datas inicial e final da demanda | 
| `TimeSpan` | WorkTime | Intervalo de horas trabalhadas entre as datas de inicio e fim desta demanda | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `TimeFlow` | BuildTimeFlow() | Retorna um TimeFlow desta demanda | 
| `TimeDemand` | CloneAndQueue(`TimeSpan` SafeTime = null) | Cria uma demanda após a demanda atual com as mesmas caracteristicas | 
| `Decimal` | GetPercentCompletion(`DateTime` MidDate) | Retorna a porcentagem em relacao a posição de uma data entre a data inicial (0%) e final (100%) | 
| `TimeSpan` | GetWorkTimeBetween(`DateTime` StartDate, `DateTime` EndDate) | Retorna o intervalo de horas trabalhadas entre duas datas baseado nas confuguracoes desta demanda | 
| `DateTime` | JourneyEndHour(`DateTime` Date) | Retorna a hora final da jornada de uma data acordo com as configuracoes desta demanda | 
| `DateTime` | JourneyStartHour(`DateTime` Date) | Retorna a hora inicial da jornada de uma data de acordo com as configuracoes desta demanda | 
| `TimeSpan` | JourneyTime(`DateTime` Date) | Retorna o tempo da jornada de trabalho de uma data de acordo com as configuracoes desta demanda | 
| `DateTime` | LunchEndHour(`DateTime` Date) | Retorna a hora de termino do almoço de uma data de acordo com as configurações desta demanda | 
| `DateTime` | LunchStartHour(`DateTime` Date) | Retorno a hora de inicio do almoço de uma data de acordo com as configurações desta demanda | 
| `TimeSpan` | LunchTime(`DateTime` Date) | Retorna o tempo de almoço de uma data de acordo com as configuracoes desta demanda | 
| `void` | PushDateIntoJourney(`DateTime&` Date) | Empurra a data para dentro da proxima hora disponivel dentro jornada de trabalho | 
| `String` | ToString() | Retorna uma string representado a quantidade de itens e o tempo gasto com a produção | 
| `TimeSpan` | TotalTime(`DateTime` Date) | Retorna a jornada de trabalho + hora de almoço de uma data de acordo com as configuracoes desta demanda | 
| `String` | ToTimeElapsedString(`Boolean` FullString = True) | Retorna uma String no formato "X anos, Y meses e Z dias" | 


## `TimeFlow`

Classe para comapração entre 2 Datas com possibilidade de validação de dias Relevantes
```csharp
public class InnerLibs.TimeMachine.TimeFlow

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<DateTime>` | AllDays | Todos os dias entre as datas Inicial e Final | 
| `Int32` | Days | Numero de Dias | 
| `DateTime` | EndDate | Data Final | 
| `Int32` | Hours | Numero de Horas | 
| `Int32` | Milliseconds | Numero de milisegundos | 
| `Int32` | Minutes | Numero de Minutos | 
| `Int32` | Months | Numero de Meses | 
| `List<DateTime>` | NonRelevantDays | Dias não relevantes entre as datas Inicial e Final | 
| `List<DayOfWeek>` | NonRelevantDaysOfWeek | Dias da semana não relevantes | 
| `List<DateTime>` | RelevantDays | Dias Relevantes entre as datas Inicial e Final | 
| `List<DayOfWeek>` | RelevantDaysOfWeek | Dias da semana relevantes | 
| `Int32` | Seconds | Numero de Segundos | 
| `DateTime` | StartDate | Data Inicial | 
| `Int32` | Years | Numero de Anos | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | ToString() | Retorna uma string com a quantidade de itens e o tempo de produção | 
| `String` | ToTimeElapsedString(`Boolean` FullString = True) | Retorna uma String no formato "X anos, Y meses e Z dias" | 


