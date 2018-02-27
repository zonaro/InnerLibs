 # Triforce For LINQ


##### Criação de HTML dinâmico a partir de coleções e arquivos HTML estáticos. Pode operar com ou sem um banco de dados. 


### Declarando um Objeto Triforce

Existem 2 formas de utilizar o Triforce. Utilizando um diretório para os arquivos
ou utilizando um Assembly com recursos inseridos (Embedded Resource)

#### Declarando um objeto Triforce sem Banco de Dados

Declarando um objeto Triforce utilizando um Diretório (`DirectoryInfo`)
```vb

Dim dir as new DirectoryInfo("e:\exemplo\templates")

Dim Engine as new Triforce(dir)

```


Declarando um objeto Triforce utilizando um Assembly  
```vb 

Dim Engine as new Triforce(Assembly.GetCurrentExecutionAssembly())

```


#### Declarando um objeto Triforce com Banco de Dados

O Triforce utiliza LINQ to SQL para fazer a conexão com o banco. Lembre-se de criar suas classes mapeadas utilizando o assistente do Visual Studio

**PROJECT > Add Item > LINQ to SQL Classes**

Agora que você possui um `DataContext` com suas classes de entidade mapeadas, declare seu objeto Triforce


Declarando um objeto Triforce utilizando um Diretório (`DirectoryInfo`)
```vb

Dim dir as new DirectoryInfo("e:\exemplo\templates")

Dim Engine as new Triforce(Of SeuDataContext)(dir)

```


Declarando um objeto Triforce utilizando um Assembly
```vb 

Dim Engine as new Triforce(Of SeuDataContext)(Assembly.GetCurrentExecutionAssembly())

```

OBS.: Lembre-se de substituir o `SeuDataContext` pelo nome gerado pelo Visual Studio usando o assistente do LINQ to SQL.

### Definindo a Propriedade de Template para uma Classe

```vb

Public Partial Class NomeClasse

    Public Property Prop1 as String
    Public Property Prop2 as String

    'Esta propriedade define qual arquivo de template é aplicado para esta classe.
    'Voce também pode criar a string do template diretamente aqui, desde que ela contenha no minimo 1 tag ou até mesmo criar um XmlNode ou HtmlDocument (da própria Innerlibs)
    Public Property TriforceTemplate as String = "template_usuario.html"

End Class


```

obs.: As classes normalmente são geradas pelo assistente do LINQ to SQL, e como todas elas são do tipo `Partial`, basta adicionar a propriedade `TriforceTemplate` nela.



 ### Definindo a estrutura do Template HTML

Veja abaixo a estrutura básica de um template do Triforce. Apenas a tag `body` é obrigatória.


```html

<head> 
<!--Conteudo do head é adicionado apenas uma vez antes do html
processado de cada item-->

   #_PAGINATION_#  <!--utilize esta Marcacao para incluir a paginacao no html do Head ou Footer-->

</head>

<body>
<!--Corpo do template, será replicado para cada item. Aqui as chaves de substituiçao são aplicadas.-->
</body>

<footer> 
<!--Conteudo do footer é adicionado apenas uma vez após o html
processado de cada item-->

   #_PAGINATION_#  <!--utilize esta Marcacao para incluir a paginacao no html do Head ou Footer-->

</footer>

<empty>
 <!--Conteudo aplicado no lugar do body quando não houver itens na coleção-->
</empty>

<pagination>
    <div class="paginas">
        <div>
            Exibindo pagina ##ACTIVEPAGE## de ##COUNT## / ##SIZE## resultados de ##TOTAL##
        </div>
        <div>
            <!--primeiro elemento da paginacao-->
            <first>
                <a href="/caminho?PAGINA=##PAGE##" class="btn"><i class="fa fa-arrow-left"></i><i class="fa fa-arrow-left"></i></a>
            </first>
            <!--elemento anterior da paginacao-->
            <back>
                <a href="/caminho?PAGINA=##PAGE##" class="btn"><i class="fa fa-arrow-left"></i></a>
            </back>
            <!--elemento replicado para cada pagina, atributo limit é o numero de paginas visiveis na paginacao-->
            <page limit="5">
                <a href="/caminho?PAGINA=##PAGE##" class="btn">##PAGE##</a>
            </page>
            <!--elemento ativo da paginacao (pagina atual)-->
            <active>
                <a href="/caminho?PAGINA=##PAGE##" class="btn active">##PAGE##</a>
            </active>
            <!--elemento proximo da paginacao-->
            <next>
                <a href="/caminho?PAGINA=##PAGE##" class="btn"><i class="fa fa-arrow-right"></i></a>
            </next>
            <!--ultimo elemento da paginacao-->
            <last>
                <a href="/caminho?PAGINA=##PAGE##" class="btn"><i class="fa fa-arrow-right"></i><i class="fa fa-arrow-right"></i></a>
            </last>
        </div>
    </div>

</pagination>

```


### Operando o Triforce no BackEnd

O Triforce possui um método chamado `ApplyTemplate` que possui deiversas sobrecargas.
Cada uma para um tipo de objeto diferente ou situaçao especifica, entre elas:



 - Aplicar um template a um objeto IEnumerable(of Tipo) - coleções estáticas
 - Aplicar um template a um objeto IQueryable(of Tipo) - coleções que normalmente vem do banco de dados

```vb

 'considere a variavel colecao como um IEnumerable(Of NomeClasse) ou IQueryable(Of NomeClasse)  

  Dim lista as TemplateList(Of NomeClasse) = Engine.ApplyTemplate(Of NomeClasse)(colecao)

```


 - Aplicar um template a um objeto Tipo - Aplicar a um unico item
 
```vb

 'considere a variavel obj como um objeto do tipo NomeClasse 

  Dim item as Template(Of NomeClasse) = Engine.ApplyTemplate(Of NomeClasse)(obj)

```

 - Aplicar um template a uma Query SQL - processa um comando SQL e gera uma coleçao com os resultados

```vb

 'considere a variavel obj como um objeto do tipo NomeClasse 

  Dim param as SqlParameter() = {} 'array de parametros SQL opcionais

  Dim lista as TemplateList(Of NomeClasse) = Engine.ApplyTemplate(Of NomeClasse)("SELECT * FROM TabelaNomeClasse",param)

```

 - Aplicar um template a uma Expression - filtra uma Table(of Entity) definida pelo LINQ to SQL e aplica um template aos resultados encontrados
```vb

 'considere a variavel colecao como um IEnumerable(Of NomeClasse) ou IQueryable(Of NomeClasse)  

  Dim lista as TemplateList(Of NomeClasse) = Engine.ApplyTemplate(Of NomeClasse)(Function(x) x.Prop1 IsNot Nothing)

```




O metodo `ApplyTemplate` tem suporte nativo a paginação de coleções do tipo IQueryable e IEnumerable. Basta passar os parametro opcionais de Pagina e tamanho da pagina:

```vb

 'considere a variavel colecao como um IEnumerable(Of NomeClasse) ou IQueryable(Of NomeClasse)  

  Dim lista as TemplateList(Of NomeClasse) = Engine.ApplyTemplate(Of NomeClasse)(colecao)


```


