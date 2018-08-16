## `InnerForm`

```csharp
public class InnerLibs.InnerForm.InnerForm
    : Page, IComponent, IDisposable, IParserAccessor, IUrlResolutionService, IDataBindingsAccessor, IControlBuilderAccessor, IControlDesignerAccessor, IExpressionsAccessor, INamingContainer, IFilterResolutionService, IHttpHandler

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String[]` | AuthorizedDomains | Dominios autorizados a utilizar o formulário. | 
| `DataBase` | DataBase | Objeto de conexão com o banco de dados. Se estiver nulo, a informação não será salva no  banco de dados porém o email será enviado normalmente | 
| `String` | ErrorMessage | Mensagem de erro | 
| `String` | MailField | Nome do campo de email do formulario | 
| `String[]` | Recipients | Lista de destinatários que irão receber o conteudo do formulário | 
| `SmtpClient` | SmtpServer | Servidor SMTP que será utilizado para o disparo de email | 
| `String` | SuccessMessage | Mensagem de sucesso | 
| `String` | TableName | Nome da tabela no Banco de Dados. É utilizado pela propriedade `InnerLibs.InnerForm.InnerForm.DataBase` na  hora de salvar as informaçoes no banco. | 
| `String` | UserMailTemplate | Conteudo do email de AutoResposta que será enviado ao usuário. O email somente é enviado  se esta propriedade nao estiver nula ou em branco | 
| `String` | UserSubject | Assunto do email de AutoResposta que será enviado ao usuário. O email somente é enviado  se a propriedade `InnerLibs.InnerForm.InnerForm.UserMailTemplate` nao estiver nula ou em branco | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | SendForm(`Object` sender, `EventArgs` e) |  | 


Static Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `List<IDictionary<String, Object>>` | GetForm(`DataBase` DataBase, `String` TableName = InnerForm, `String` WhereConditions = ) |  | 


