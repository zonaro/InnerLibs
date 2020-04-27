# Triforce Templates

## O que é?

E um plugin para .NET framework focado (mas nao limitado) ao desenvolvimento de paginas da web atraves de templates HTML.
 Seu diferencial esta na forma como as paginas podem ser construidas atraves de templates, subtemplates e tags de ação




 

## Implementação
Existem 2 formas de utilizar o Triforce Template


 - `Triforce` - Aplicador de Templates. Funciona diretamente em arrays e coleções, idenpendente das conexões com banco de dados. 
 - `Triforce<DbConnection>` - Integra `Triforce` com uma `DbConnection`. Aplica templates diretamente em consultas SQL além das coleções e arrays.
 - `Triforce<DataContext>` - Integra `Triforce` com um `DataContext`. Aplica templates diretamente em consultas LINQ to SQL, além de consultas SQL padrão, coleções e arrays.



### Instanciando um TriforceTemplate

> Para exemplificar, usarei o método mais completo: `DataContext`


Existem 2 formas de implementar `Triforce` no projeto. Usando um diretório com os arquivos html ou embutindo os templates como recurso no seu  `Assembly`


#### Com diretório

```csharp

var dir = new DirectoryInfo("/caminho/dos/templates");

var tt = new Triforce<MeuDataContext>(dir);

```


#### Embutido

```csharp

var app = Assembly.GetExecutingAssembly();

var tt = new Triforce<MeuDataContext>(app);

```


### Criando Templates

Um template é um arquivo HTML em um diretório ou `Assembly`. Deve seguir a seguinte estrutura:

```html
<head>
    <!-- Contém a estrutura html de cabeçalho deste template. É renderizado antes do body -->
</head>

<body>
    <!-- Contém a estrutura html de um item do template. É repetido pra cada item em uma coleção -->
</body>
<empty>
    <!-- Contém a estrutura html de lista vazia. É renderizado no lugar do body quando a coleção não retornar nenhum item -->
</empty>
<footer>
    <!-- Contém a estrutura html de rodapé deste template. É renderizado após o body -->
</footer>
<pagination>
    <!-- Contém a estrutura html de paginação deste template. Não é renderizado por padrão. É necessário marcar o local da paginação no head ou footer com #_PAGINATION_# -->

    <first>
        <!-- Primeiro item da paginação -->
    </first>
    <back>
        <!-- Item "Anterior" da paginação -->
    </back>
    <page>
        <!-- Item da paginação. É renderizado para cada numero de página -->
    </page>
    <active>
        <!-- Página Atual -->
    </active>
    <next>
        <!-- Item "Próximo" da paginação -->
    </next>
    <last>
        <!-- Ultimo Item da paginação -->
    </last>
</pagination>
```


#### Variaveis de Template

Templates não seriam templates se seu conteúdo não fosse dinâmico certo? O `Trifoce` identificará todos os campos entre ## como variáveis de template:

```html

<body>

<li> ##ITEM## </li>

</body>

```

#### Aplicação:


Considere a tabela abaixo como nossa fonte de dados:


 



## FAQ

 -  **Porque o componente se chama `Triforce`?** É uma referência aos jogos da série The Legend of Zelda, onde o protagonista <s>Linq</s> **Link** pode obter a reliquia das deusas, a chamada **Triforce**
 -  

