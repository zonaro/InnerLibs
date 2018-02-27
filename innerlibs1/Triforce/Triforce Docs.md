 # Triforce For LINQ


Triforce é um componente que permite 
a criação de HTML dinâmico a partir de coleções e arquivos HTML estáticos.
Pode operar com ou sem banco de dados. 

Para operar com banco de dados, o Triforce utiliza um `DataContext` como ponto de entrada para as conexões SQL


### Declarando um Objeto Triforce

Existem 2 formas de utilizar o Triforce. Utilizando um diretório para os arquivos
ou utilizando um Assembly com recursos inseridos (Embedded Resource)

#### Declarando um objeto Triforce sem Banco de Dados

Declarando um objeto Triforce utilizando um Diretório (`DirectoryInfo`)
```vb

Dim dir as new DirectoryInfo("e:\exemplo\templates")

Dim Engine as new Triforce(dir)

```


Declarando um objeto Triforce utilizando um Assembly (`DirectoryInfo`)
```vb 

Dim Engine as new Triforce(Assembly.GetCurrentExecutionAssembly())

```


#### Delcarando um objeto Triforce com Banco de Dados


Declarando um objeto Triforce utilizando um Diretório (`DirectoryInfo`)
```vb

Dim dir as new DirectoryInfo("e:\exemplo\templates")

Dim Engine as new Triforce(Of SeuDataContext)(dir)

```


Declarando um objeto Triforce utilizando um Assembly (`DirectoryInfo`)
```vb 

Dim Engine as new Triforce(Of SeuDataContext)(Assembly.GetCurrentExecutionAssembly())

```

OBS.: Lembre-se que o `SeuDataContext` é gerado automaticamente pelo Visual Studio usando o assistente do LINQ to SQL.

### Definindo uma classe para o template

```vb

Public Partial Class NomeClasse

    Public Property Prop1 as String
    Public Property Prop2 as String

    'Esta propriedade define qual arquivo de template é aplicado para esta classe
    Public Property TriforceTemplate as String = "template_usuario.html"

End Class


```

obs.: As classes normalmente são geradas pelo assistente do LINQ to SQL, e como todas elas são do tipo `Partial`, basta adicionar a propriedade `TriforceTemplate` nela.

 ### Definindo a estrutura do Template HTML

```html

<head> 
Conteudo do head é adicionado apenas uma vez antes do html
processado de cada item
</head>

<body>
Corpo do template, será replicado para cada item
</body>

<footer> 
Conteudo do footer é adicionado apenas uma vez após o html
processado de cada item
</footer>

<empty>
 Conteudo aplicado no lugar do body quando não houver itens na coleção
</empty>


```


