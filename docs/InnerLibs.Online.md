## `LogEntryBackup`

```csharp
public class InnerLibs.Online.LogEntryBackup

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | DateTime |  | 
| `String` | ID |  | 
| `Dictionary<String, String>` | LogData |  | 
| `String` | Message |  | 
| `String` | Url |  | 
| `String` | UserID |  | 


## `OnlineList<UserType, IdType>`

`System.Collections.Generic.Dictionary`2` utilizado para controle de usuários que estão online/offline em uma aplicação
```csharp
public class InnerLibs.Online.OnlineList<UserType, IdType>
    : Dictionary<IdType, OnlineUser<UserType, IdType>>, IDictionary<IdType, OnlineUser<UserType, IdType>>, ICollection<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, IEnumerable<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<IdType, OnlineUser<UserType, IdType>>, IReadOnlyCollection<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, ISerializable, IDeserializationCallback

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DirectoryInfo` | BackupDirectory | Diretorio onde serão guardados os XMLs deta lista | 
| `UserChat<UserType, IdType>` | Chat | Lista de conversas dos usuários | 
| `FileInfo` | ChatFile | Caminho do arquivo XML do Chat | 
| `OnlineUser<UserType, IdType>` | Item | Retorna um usuario de acordo com seu ID | 
| `UserLog<UserType, IdType>` | Log | Entradas de ações dos usuários | 
| `FileInfo` | LogFile | Caminho do arquivo XML do Log | 
| `Action<UserLogEntry<UserType, IdType>>` | OnCreateLog | Função que será executada quando ocorrer uma entrada no log | 
| `Action<OnlineUser<UserType, IdType>>` | OnUserOnlineChanged |  | 
| `TimeSpan` | ToleranceTime | Tolerancia que o servidor considera um usuário online ou na mesma atividade | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | Add(`UserType` User) | Adciona um usuario a esta lista | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | Add(`IEnumerable<UserType>` Users) | Adciona um usuario a esta lista | 
| `OnlineUser<UserType, IdType>` | Add(`UserType` Obj, `Nullable<Boolean>` Online = null, `String` Activity = null, `String` Url = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateTime = null) | Adciona um usuario a esta lista | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | AddMany(`UserType[]` Users) | Adciona varios usuarios a esta lista | 
| `Boolean` | ContainsUser(`UserType` User) | Verifica se um usuario está nesta lista | 
| `UserLogEntry<UserType, IdType>` | CreateLog(`UserType` User, `String` Message, `String` URL = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateTime = null) | Cria uma entrada no log deste usuário | 
| `IdType` | GetID(`UserType` User) | Retorna o ID do usuário | 
| `OnlineUser<UserType, IdType>` | GetUser(`UserType` User) | Retorna um OnlineUser a partir de uma instancia de `type` | 
| `IEnumerable<UserType>` | GetUsersData(`Nullable<Boolean>` IsOnline = null) | Retorna todo os usuarios | 
| `OnlineUser<UserType, IdType>` | KeepOnline(`UserType` Obj) | Mantém um usuario online mas não atribui nenhuma nova atividade nem cria entradas no LOG | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | OfflineUsers() | Retorna todos os `InnerLibs.Online.OnlineUser`2` que estão Offline | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | OnlineUsers() | Retorna todos os `InnerLibs.Online.OnlineUser`2` que estão Online no momento | 
| `void` | OpenChatXML() |  | 
| `void` | OpenLogXML() |  | 
| `void` | Remove(`UserType[]` Obj) | Remove um usuário desta lista | 
| `void` | Remove(`IdType` ID) | Remove um usuário desta lista | 
| `void` | SaveChatXML() |  | 
| `void` | SaveLogXML() |  | 
| `OnlineUser<UserType, IdType>` | SetOffline(`UserType` Obj) | Seta um usuario como offline e cria uma entrada no log | 
| `OnlineUser<UserType, IdType>` | SetOnline(`UserType` Obj) | Seta um usuario como online e cria uma entrada no Log | 
| `OnlineUser<UserType, IdType>` | SetOnlineActivity(`UserType` Obj, `String` Activity, `String` Url = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateTime = null) | Seta um usuario como online e atribui uma atividade a ele. Cria entrada no log automaticamente | 
| `OnlineUser<UserType, IdType>` | UserById(`IdType` Key) | Retorna um usuário desta lista a partir do ID | 
| `UserType` | UserDataById(`IdType` Key) | Retorna um usuário desta lista a partir do ID | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | Users() | Retorna todos os `InnerLibs.Online.OnlineUser`2` | 


## `OnlineUser<UserType, IdType>`

Usuario Online/Offline
```csharp
public class InnerLibs.Online.OnlineUser<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserConversation`2[]` | Conversations | Conversas deste usuário | 
| `IdType` | ID | ID deste usuário | 
| `Boolean` | IsOnline | Indica se o usuario está online ou não | 
| `String` | LastActivity | Ultima atividade do usuário | 
| `Nullable<DateTime>` | LastOnline | Ultima vez que o usuário esteve Online | 
| `String` | LastUrl | Ultima URL/caminho que o usuário acessou | 
| `UserType` | User | Informações relacionadas do usuário do tipo `type` | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserConversation`2[]` | GetConversations(`UserType` WithUser = null) | Retorna uma lista de conversas com um usuario específico ou todas | 
| `IEnumerable<UserLogEntry<UserType, IdType>>` | LogEntries() | Entradas no Log para este usuário | 
| `UserConversation<UserType, IdType>` | SendMessage(`UserType` ToUser, `String` Message) | Envia uma mensagem no chat para outro usuario | 
| `String` | ToString() |  | 


## `UserChat<UserType, IdType>`

Lista de conversas entre usuários
```csharp
public class InnerLibs.Online.UserChat<UserType, IdType>
    : List<UserConversation<UserType, IdType>>, IList<UserConversation<UserType, IdType>>, ICollection<UserConversation<UserType, IdType>>, IEnumerable<UserConversation<UserType, IdType>>, IEnumerable, IList, ICollection, IReadOnlyList<UserConversation<UserType, IdType>>, IReadOnlyCollection<UserConversation<UserType, IdType>>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Encoding` | Encoding |  | 
| `IEnumerable<String>` | IDs |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | DeleteConversation(`UserType` User, `UserType` WithUser = null) | Apaga uma conversa com um usuário | 
| `IEnumerable<UserConversation<UserType, IdType>>` | GetConversation(`UserType` User, `UserType` WithUser = null) | Retorna uma conversa entre 2 usuários | 
| `UserConversation<UserType, IdType>` | Send(`UserType` FromUser, `UserType` ToUser, `String` Message) | Envia uma nova mensagem | 


## `UserConversation<UserType, IdType>`

Mensagen do chat do usuário
```csharp
public class InnerLibs.Online.UserConversation<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IdType` | FromUserID | Id do usuário emissor | 
| `String` | ID | Id desta mensagem | 
| `String` | Message | Mensagem | 
| `DateTime` | SentDate | Data de envio da mensagem | 
| `IdType` | ToUserID | ID do usuário destinatario | 
| `Boolean` | Viewed | Inndica se esta conversa foi visualizada | 
| `Nullable<DateTime>` | ViewedDate | Data de visualização da mensagem | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | FromUser() | Usuario Emissor | 
| `OnlineUser<UserType, IdType>` | GetMyUser(`UserType` MySelf) | Retorna a instancia de `InnerLibs.Online.OnlineUser`2` (Emissor ou destinatário) de um <see cref="!:UserType" /> especifico | 
| `OnlineUser<UserType, IdType>` | GetOtherUser(`UserType` MySelf) | Retorna a instancia de `InnerLibs.Online.OnlineUser`2` (Emissor ou destinatário) de um <see cref="!:UserType" /> especifico | 
| `Boolean` | IsFrom(`UserType` User) | Retorna true se a mensagem for de `` | 
| `Boolean` | IsTo(`UserType` User) | Retorna true se a mensagem for para `` | 
| `OnlineUser<UserType, IdType>` | ToUser() | Usuario destinatário | 


## `UserConversationBackup`

```csharp
public class InnerLibs.Online.UserConversationBackup

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | FromUserID |  | 
| `String` | ID |  | 
| `String` | Message |  | 
| `DateTime` | SentDate |  | 
| `String` | ToUserID |  | 
| `Nullable<DateTime>` | ViewedDate |  | 


## `UserLog<UserType, IdType>`

```csharp
public class InnerLibs.Online.UserLog<UserType, IdType>
    : List<UserLogEntry<UserType, IdType>>, IList<UserLogEntry<UserType, IdType>>, ICollection<UserLogEntry<UserType, IdType>>, IEnumerable<UserLogEntry<UserType, IdType>>, IEnumerable, IList, ICollection, IReadOnlyList<UserLogEntry<UserType, IdType>>, IReadOnlyCollection<UserLogEntry<UserType, IdType>>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<String>` | IDs |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserLogEntry<UserType, IdType>` | CreateLog(`UserType` User, `String` Message, `String` URL = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateAndTime = null) | Cria uma entrada no log deste usuário com uma data especifica | 


## `UserLogEntry<UserType, IdType>`

Entrada de ação do usuário no sistema
```csharp
public class InnerLibs.Online.UserLogEntry<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | DateTime | Data e hora da ocorrência | 
| `String` | ID | ID desta entrada | 
| `Dictionary<String, String>` | LogData | Informações adicionais | 
| `String` | Message | Texto sobre a ocorrencia | 
| `String` | URL | Ultima URL da ocorrencia | 
| `IdType` | UserID | ID do Usuário | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | GetUser() | Usuário | 


