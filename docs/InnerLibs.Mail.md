## `FluentMailMessage`

```csharp
public class InnerLibs.Mail.FluentMailMessage
    : MailMessage, IDisposable

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Action<MailAddress, FluentMailMessage, Exception>` | ErrorAction |  | 
| `IEnumerable<ValueTuple<MailAddress, Exception>>` | ErrorList |  | 
| `SentStatus` | SentStatus |  | 
| `IEnumerable<ValueTuple<MailAddress, SentStatus, Exception>>` | SentStatusList |  | 
| `SmtpClient` | Smtp |  | 
| `Action<MailAddress, FluentMailMessage>` | SuccessAction |  | 
| `IEnumerable<MailAddress>` | SuccessList |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `FluentMailMessage` | AddAttachment(`Attachment[]` Attachment) |  | 
| `FluentMailMessage` | AddAttachment(`IEnumerable<Attachment>` Attachment) |  | 
| `FluentMailMessage` | AddAttachment(`Attachment` Attachment) |  | 
| `FluentMailMessage` | AddAttachment(`Stream` Attachment, `String` Name) |  | 
| `FluentMailMessage` | AddAttachment(`Byte[]` Attachment, `String` Name) |  | 
| `FluentMailMessage` | AddAttachment(`FileInfo` Attachment) |  | 
| `FluentMailMessage` | AddBlindCarbonCopy(`String[]` Emails) |  | 
| `FluentMailMessage` | AddBlindCarbonCopy(`MailAddress[]` Emails) |  | 
| `FluentMailMessage` | AddCarbonCopy(`String[]` Emails) |  | 
| `FluentMailMessage` | AddCarbonCopy(`MailAddress[]` Emails) |  | 
| `FluentMailMessage` | AddRecipient(`String` Email, `T` TemplateData) |  | 
| `FluentMailMessage` | AddRecipient(`String` Email, `String` DisplayName, `T` TemplateData) |  | 
| `FluentMailMessage` | AddRecipient(`T` Data, `Expression<Func<T, String>>` EmailSelector, `Expression<Func<T, String>>` NameSelector = null) |  | 
| `FluentMailMessage` | AddRecipient(`IEnumerable<T>` Data, `Expression<Func<T, String>>` EmailSelector, `Expression<Func<T, String>>` NameSelector = null) |  | 
| `FluentMailMessage` | AddRecipient(`String[]` Emails) |  | 
| `FluentMailMessage` | AddRecipient(`TemplateMailAddress[]` Emails) |  | 
| `FluentMailMessage` | FromEmail(`String` Email) |  | 
| `FluentMailMessage` | FromEmail(`String` Email, `String` DisplayName) |  | 
| `FluentMailMessage` | FromEmail(`MailAddress` Email) |  | 
| `FluentMailMessage` | OnError(`Action<MailAddress, FluentMailMessage, Exception>` Action) |  | 
| `FluentMailMessage` | OnSuccess(`Action<MailAddress, FluentMailMessage>` Action) |  | 
| `FluentMailMessage` | ResetStatus() |  | 
| `FluentMailMessage` | Send() |  | 
| `SentStatus` | SendAndDispose() |  | 
| `String` | ToString() |  | 
| `FluentMailMessage` | UseGmailSmtp() |  | 
| `FluentMailMessage` | UseLocawebSmtp() |  | 
| `FluentMailMessage` | UseOffice365Smtp() |  | 
| `FluentMailMessage` | UseOutlookSmtp() |  | 
| `FluentMailMessage` | UseTemplate(`String` TemplateOrFilePathOrUrl) |  | 
| `FluentMailMessage` | UseTemplate(`String` TemplateOrFilePathOrUrl, `String` MessageTemplate) |  | 
| `FluentMailMessage` | UseTemplate(`String` TemplateOrFilePathOrUrl, `T` MessageTemplate) |  | 
| `FluentMailMessage` | WithCredentials(`NetworkCredential` Credentials) |  | 
| `FluentMailMessage` | WithCredentials(`String` Login, `String` Password) |  | 
| `FluentMailMessage` | WithMessage(`String` Text) |  | 
| `FluentMailMessage` | WithMessage(`HtmlTag` Text) |  | 
| `FluentMailMessage` | WithQuickConfig(`String` Email, `String` Password) |  | 
| `FluentMailMessage` | WithSmtp(`SmtpClient` Client) |  | 
| `FluentMailMessage` | WithSmtp(`String` Host) |  | 
| `FluentMailMessage` | WithSmtp(`String` Host, `Int32` Port) |  | 
| `FluentMailMessage` | WithSmtp(`String` Host, `Int32` Port, `Boolean` UseSSL) |  | 
| `FluentMailMessage` | WithSubject(`String` Text) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `SmtpClient` | GmailSmtp() |  | 
| `SmtpClient` | LocawebSmtp() |  | 
| `SmtpClient` | Office365Smtp() |  | 
| `SmtpClient` | OutlookSmtp() |  | 
| `SentStatus` | QuickSend(`String` Email, `String` Password, `String` Recipient, `String` Subject, `String` Message) |  | 


## `SentStatus`

```csharp
public enum InnerLibs.Mail.SentStatus
    : Enum, IComparable, IFormattable, IConvertible

```

Enum

| Value | Name | Summary | 
| --- | --- | --- | 
| `0` | NotSent |  | 
| `1` | Success |  | 
| `2` | Error |  | 
| `3` | PartialSuccess |  | 


## `TemplateMailAddress`

```csharp
public class InnerLibs.Mail.TemplateMailAddress
    : MailAddress

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | TemplateData |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `IEnumerable<TemplateMailAddress>` | FromList(`IEnumerable<T>` Data, `Expression<Func<T, String>>` EmailSelector, `Expression<Func<T, String>>` NameSelector = null) |  | 
| `TemplateMailAddress` | FromObject(`T` Data, `Expression<Func<T, String>>` EmailSelector, `Expression<Func<T, String>>` NameSelector = null) |  | 


