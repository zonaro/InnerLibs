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

```csharp
public class InnerLibs.Online.OnlineList<UserType, IdType>
    : Dictionary<IdType, OnlineUser<UserType, IdType>>, IDictionary<IdType, OnlineUser<UserType, IdType>>, ICollection<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, IEnumerable<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<IdType, OnlineUser<UserType, IdType>>, IReadOnlyCollection<KeyValuePair<IdType, OnlineUser<UserType, IdType>>>, ISerializable, IDeserializationCallback

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DirectoryInfo` | BackupDirectory |  | 
| `UserChat<UserType, IdType>` | Chat |  | 
| `FileInfo` | ChatFile |  | 
| `OnlineUser<UserType, IdType>` | Item |  | 
| `UserLog<UserType, IdType>` | Log |  | 
| `FileInfo` | LogFile |  | 
| `Action<UserLogEntry<UserType, IdType>>` | OnCreateLog |  | 
| `Action<OnlineUser<UserType, IdType>>` | OnUserOnlineChanged |  | 
| `TimeSpan` | ToleranceTime |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | Add(`UserType` User) |  | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | Add(`IEnumerable<UserType>` Users) |  | 
| `OnlineUser<UserType, IdType>` | Add(`UserType` Obj, `Nullable<Boolean>` Online = null, `String` Activity = null, `String` Url = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateTime = null) |  | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | AddMany(`UserType[]` Users) |  | 
| `Boolean` | ContainsUser(`UserType` User) |  | 
| `UserLogEntry<UserType, IdType>` | CreateLog(`UserType` User, `String` Message, `String` URL = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateAndTime = null) |  | 
| `IdType` | GetID(`UserType` User) |  | 
| `OnlineUser<UserType, IdType>` | GetUser(`UserType` User) |  | 
| `IEnumerable<UserType>` | GetUsersData(`Nullable<Boolean>` IsOnline = null) |  | 
| `OnlineUser<UserType, IdType>` | KeepOnline(`UserType` Obj) |  | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | OfflineUsers() |  | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | OnlineUsers() |  | 
| `void` | OpenChatXML() |  | 
| `void` | OpenLogXML() |  | 
| `void` | Remove(`UserType[]` Obj) |  | 
| `void` | Remove(`IdType` ID) |  | 
| `void` | SaveChatXML() |  | 
| `void` | SaveLogXML() |  | 
| `OnlineUser<UserType, IdType>` | SetOffline(`UserType` Obj) |  | 
| `OnlineUser<UserType, IdType>` | SetOnline(`UserType` Obj) |  | 
| `OnlineUser<UserType, IdType>` | SetOnlineActivity(`UserType` Obj, `String` Activity, `String` Url = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateTime = null) |  | 
| `OnlineUser<UserType, IdType>` | UserById(`IdType` Key) |  | 
| `UserType` | UserDataById(`IdType` Key) |  | 
| `IEnumerable<OnlineUser<UserType, IdType>>` | Users() |  | 


## `OnlineUser<UserType, IdType>`

```csharp
public class InnerLibs.Online.OnlineUser<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserConversation`2[]` | Conversations |  | 
| `IdType` | ID |  | 
| `Boolean` | IsOnline |  | 
| `String` | LastActivity |  | 
| `Nullable<DateTime>` | LastOnline |  | 
| `String` | LastUrl |  | 
| `UserType` | User |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `UserConversation`2[]` | GetConversations(`UserType` WithUser = null) |  | 
| `IEnumerable<UserLogEntry<UserType, IdType>>` | LogEntries() |  | 
| `UserConversation<UserType, IdType>` | SendMessage(`UserType` ToUser, `String` Message) |  | 
| `String` | ToString() |  | 


## `UserChat<UserType, IdType>`

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
| `void` | DeleteConversation(`UserType` User, `UserType` WithUser = null) |  | 
| `IEnumerable<UserConversation<UserType, IdType>>` | GetConversation(`UserType` User, `UserType` WithUser = null) |  | 
| `UserConversation<UserType, IdType>` | Send(`UserType` FromUser, `UserType` ToUser, `String` Message) |  | 


## `UserConversation<UserType, IdType>`

```csharp
public class InnerLibs.Online.UserConversation<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `IdType` | FromUserID |  | 
| `String` | ID |  | 
| `String` | Message |  | 
| `DateTime` | SentDate |  | 
| `IdType` | ToUserID |  | 
| `Boolean` | Viewed |  | 
| `Nullable<DateTime>` | ViewedDate |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | FromUser() |  | 
| `OnlineUser<UserType, IdType>` | GetMyUser(`UserType` MySelf) |  | 
| `OnlineUser<UserType, IdType>` | GetOtherUser(`UserType` MySelf) |  | 
| `Boolean` | IsFrom(`UserType` User) |  | 
| `Boolean` | IsTo(`UserType` User) |  | 
| `OnlineUser<UserType, IdType>` | ToUser() |  | 


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
| `UserLogEntry<UserType, IdType>` | CreateLog(`UserType` User, `String` Message, `String` URL = null, `Dictionary<String, String>` LogData = null, `Nullable<DateTime>` DateAndTime = null) |  | 


## `UserLogEntry<UserType, IdType>`

```csharp
public class InnerLibs.Online.UserLogEntry<UserType, IdType>

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `DateTime` | DateTime |  | 
| `String` | ID |  | 
| `Dictionary<String, String>` | LogData |  | 
| `String` | Message |  | 
| `String` | URL |  | 
| `IdType` | UserID |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `OnlineUser<UserType, IdType>` | GetUser() |  | 


