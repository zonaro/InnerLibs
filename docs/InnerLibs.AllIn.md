## `ClienteAllIn`

```csharp
public class InnerLibs.AllIn.ClienteAllIn

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Login | Login da Allin | 
| `String` | Senha | Senha da Allin | 
| `Uri` | UrlEmailMarketing |  | 
| `Uri` | UrlTransacional |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Type` | Criar() |  | 
| `String` | EnviarEmailTransacional(`String` nm_envio, `String` nm_subject, `String` nm_remetente, `String` nm_reply, `String` nm_email, `String` html, `DateTime` dt_envio) |  | 
| `String` | GerarToken(`Uri` Url) | Token da API da Allin a partir da url | 
| `Type` | POST(`Uri` Url, `String` Method, `NameValueCollection` Obj, `NameValueCollection` QueryString = null) |  | 


