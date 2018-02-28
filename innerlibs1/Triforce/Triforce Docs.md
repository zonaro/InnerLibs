 

 
<center><img src="http://innercode.com.br/wp-content/uploads/2018/02/g4595.png" style="width:40%"></center>
<br>

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

Dim Engine as new Triforce(Of   SeuDataContext)(dir)

```

 
Declarando um objeto Triforce utilizando um Assembly
```vb 

Dim Engine as new Triforce(Of SeuDataContext)(Assembly.GetCurrentExecutionAssembly())

```

**ATENÇÃO Não esqueça de marcar alterar o Build Action arquivo .html e .sql para Embedded Resource caso esteja usando o método de Assembly**      

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

#### Tags de Estrutura

 - Head - Cabeçalho do Template, é concatenado antes dos templates processados.
 - Footer - Rodapé do Template, é concatenado após os templates processados.
 - Body - Conteúdo do Template. É replicado para cada item encontrado na coleção.
 - Empty - Quando uma coleção não possuir itens, o conteudo de empty é exibido no lugar do body.
 - Pagination - Guarda o template de paginação. Este só é exibido se a tag Footer ou Head possuir uma marcação #\_PAGINATION\_#.

#### Marcação

Em qualquer lugar do Body, escreva o nome da propriedade que sua classe possui entre 2 hashsign.

```html

<body>
    <span id="##Prop1##">##Prop2##</span>
</body>

```

##Prop1## será substituido pelo conteúdo de Prop1 e ##Prop2## pelo conteúdo de Prop2 da instancia dessa NomeClasse. Este process se repetirá até que todas as instancias encontradas na coleçao sejam processadas.


#### Aplicação

Veja abaixo a estrutura básica de um template do Triforce. Lembrando que apenas a tag `body` é obrigatória.

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
            Exibindo pagina ##ActivePage## de ##PageCount## / ##PageSize## resultados de ##Total##
        </div>
        <div>
            <!--primeiro elemento da paginacao-->
            <first>
                <a href="/caminho?PAGINA=##PageNumber##" class="btn"><i class="fa fa-arrow-left"></i><i class="fa fa-arrow-left"></i></a>
            </first>
            <!--elemento anterior da paginacao-->
            <back>
                <a href="/caminho?PAGINA=##PageNumber##" class="btn"><i class="fa fa-arrow-left"></i></a>
            </back>
            <!--elemento replicado para cada pagina, atributo limit é o numero de paginas visiveis na paginacao-->
            <page limit="5">
                <a href="/caminho?PAGINA=##PageNumber##" class="btn">##PageNumber##</a>
            </page>
            <!--elemento ativo da paginacao (pagina atual)-->
            <active>
                <a href="/caminho?PAGINA=##PageNumber##" class="btn active">##PageNumber##</a>
            </active>
            <!--elemento proximo da paginacao-->
            <next>
                <a href="/caminho?PAGINA=##PageNumber##" class="btn"><i class="fa fa-arrow-right"></i></a>
            </next>
            <!--ultimo elemento da paginacao-->
            <last>
                <a href="/caminho?PAGINA=##PageNumber##" class="btn"><i class="fa fa-arrow-right"></i><i class="fa fa-arrow-right"></i></a>
            </last>
        </div>
    </div>

</pagination>

```

### Entendendo o Template de Paginação (Tag)

#### SubTags
É a base para todos os botões de paginacão. É gerado a partir de tags simples:

- First - Representa o botão de "pular para o primeiro"
- Back - Representa o botão de "voltar uma página"
- Page - Representa o botão de uma página especifica
    - Atributo **Limit** - Indica quantas paginas devem ser exibidas. incuindo a pagina ativa (que ficará na maioria das vezes, no meio)
- Active - Representa o botão da página ativa.
- Next - Representa o botão de "avançar uma página"
- Last - Representa o botão de "pular para o ultimo"

#### Marcação

 - ##PageNumber## - Será trocado pelo numero da página correspondente (Usando o index da iteração)
 - ##ActivePage## - Será trocado pelo numero da página atual. (ignorando completamente a Iteração)
 - ##PageCount## - Será trocado pelo número total de páginas.
 - ##PageSize## Será trocado pelo tamanho da página (numero de itens por página).
 - ##Total## Será trocado pelo número total de resultados.



Utilize a marcação #\_PAGINATION\_# nas tags Head ou Footer de seu template para exibir a paginação.


### Operando o Triforce no BackEnd

O Triforce possui um método chamado `ApplyTemplate` que possui deiversas sobrecargas.
Cada uma para um tipo de objeto diferente ou situaçao especifica, entre elas:



 - Aplicar um template a um objeto IEnumerable(Of NomeClasse) - coleções estáticas
 - Aplicar um template a um objeto IQueryable(Of NomeClasse) - coleções que normalmente vem do banco de dados

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


OBS.: O metodo `ApplyTemplate` tem suporte nativo a paginação de coleções do tipo IQueryable e IEnumerable. Basta passar os parametro opcionais de Pagina e tamanho da pagina:

```vb

 'considere a variavel colecao como um IEnumerable(Of NomeClasse) ou IQueryable(Of NomeClasse)  

  Dim lista as TemplateList(Of NomeClasse) = Engine.ApplyTemplate(Of NomeClasse)(colecao,"nome_template.html",1,10)
  'Aqui teremos uma TemplateList representando a pagina 1, com 10 elementos.
  'O Segundo parametro pode ser passado como Nulo, vazio ou em branco se o template estiver definido na propriedade TriforceTemplate da classe NomeClasse.

```


#### Template e TemplateList.

São classes que guardam os templates após o processamento. Elas recebem também o tipo da coleção.


##### Template(Of NomeClasse)

Possui 2 Propriedades:

 - Data - Elemento da coleção que originou este template. 
 - ProcessedTemplate - Documento Html processado que guarda a estrutura do template aplicado para este elemento.


OBS.: Utilize a função ToString para retornar uma string HTML deste template


##### TemplateList(Of NomeClasse)

É uma coleçao somente leitura que guarda varios Template(Of NomeClasse) e dá acesso a algumas propriedades e funções.

Possui 8 Propriedades:

 - Pagination - Propriedade que retorna o HTML da paginação definida no template do Tipo especificado. 
 - Empty - Propriedade que retorna o HTML que representa uma coleçao vazia. (Placeholder).
 - Head - Propriedade que retorna o HTML do Cabeçalho.
 - Footer - Propriedade que retorna o HTML do Rodapé.
 - PageSize - QUantidade de itens por página.
 - PageNumber - Número da página atual.
 - Total - Total de itens encontrados na coleção.
 - PageCount - Quantidade total de páginas.




OBS.: Utilize a função BuildHtml para retornar um documento Html processado ou ToString para retornar uma string HTML com todos os elementos, paginacao, cabeçalho e rodapé.

**Dica: TemplateList é implicitamente convertida para uma string HTML quando atribuida a uma variavel do tipo string.**


### Utilizando CustomValues

O triforce pode substituir marcações que não são propriedades de uma classe. Basta defini-las no objeto CustomValues do triforce:

```vb

Engine.CustomValues("ValorCustom") = "teste"

```

Automaticamente qualquer marcação ##ValorCustom## encontrada no template será substituida pela string "teste". Valores custom, diferente das propriedades de classe, não se limitam apenas ao body e podem ser aplicados aso Head, footer, empty e pagination.


### Utilizando as TriforceTags para criar templates dinâmicos

O Triforce possui a capacidade de interpretar algumas expressões simples e processar tags específicas de um template. São Elas:

- Tag Repeat - Repete seu conteudo X vezes, igual a um `For` porém sem uma coleção para iterar.
- Tag Switch - Realiza operações de Switch Case diretamente no template,  permitindo conteúdos diferentes no template dependendo das condições.
- Tag If - Realiza operações de If Else diretamente no template, permitindo conteúdos diferentes no template dependendo das condições.
- Tag Condition - Realiza operação If Else. Diferente da tag If, esta se auto-destrói quando a condiçao não for atendida.
- Tag Template - Permite que um template tenha subtemplates (ideal para mostrar relações de entidade). As regras desta tag são as mesmas de um template Pai (permite Head, Body, Footer, Empty e Pagination). Pode ser aplicado a partir de uma propriedade da classe do template pai ou diretamente de uma Query SQL.


OBS.: Todas as tags podem ser renderizadas como uma Tag HML comum ou podem simplesmente serem substituidas pelo seu conteudo processado.

#### Atributos das TriforceTags

- RenderAs - Quando especificado, renderiza o conteudo processado como uma tag com o nome especificada neste atributo. Se especificado como branco ou vazio, a tag renderizada será Span.
- Disabled - Desativa o processamento desta tag e destroi a mesma.

```html

<!--renderizando como Div-->
<Nome_Da_TriforceTag renderas="div">

</Nome_Da_TriforceTag>

<!--renderizando o conteudo diretamente na pagina (na tag Parent)-->
<Nome_Da_TriforceTag>

</Nome_Da_TriforceTag>


<!--desativando o processamento desta tag (ela será removida do documento durante o processamento)-->
<Nome_Da_TriforceTag disabled>

</Nome_Da_TriforceTag>

```


#### Tag Repeat

```html

<repeat value="2" renderas="ul">
   <!--conteudo, use a marcação _index para exibir o numero da replicacao atual-->
   <li>_index</li>
</repeat>

```

#### Tag Switch

##### SubTags

 - *Tag Case* - Guarda parte do conteudo que será exibido se a condiçao desta for cumprida.
    - Atributo **value** - Guarda o valor que será comparado com o atributo value da tag switch pai. Obrigatório
    - Atributo **operator** - Guarda o operador de comparação aplicado entre o atributo value desta e o atributo value da tag switch pai. Quando não especificado, utiliza o operador =
    - Atributo **break** - Quando especificado, sai do processamento do switch assim que esta condição é atendida, evitando que outras condições também sejam.
 - *Tag Else*  - Guarda parte do conteudo que será exibido nunhuma condiçao for cumprida. Não obrigatória.

##### Aplicação no Body do Template

```html
 
 <switch value="5">
        <case value="0">
            <!--conteudo html-->
        </case>
        <case value="1" break>
            <!--conteudo html, break pula as verificacoes seguintes-->
        </case>
        <case value="2" operator=">">
            <!--conteudo html, operator define qual operador usado na comparacao. default é "=" -->
        </case>
        <else>
            <!--conteudo se nenhuma condiçao for atendida-->
        </else>
    </switch>

```
#### Tag Switch

##### SubTags

 - *Tag Expression* - Seu conteudo representa uma expressão onde o resultado deve ser do tipo numérico ou booleano.
 - *Tag True*  - Guarda o conteudo que será exibido se a expressão retornar True ou um numero maior que 0.
 - *Tag False*  - Guarda o conteudo que será exibido se a expressão retornar False ou um numero menor ou igual a 0. (Não obrigatória)

##### Aplicação no Body do Template

```html

  <if>
     <expression>
         2 > 3
     </expression>
     <true>
         <!--Conteudo se expressão for = true-->
     </true>
     <false>
         <!--Conteudo se expressão for = false-->
         <!--esta tag nao é obrigatoria, se nela nao existir, o a tag é apenas removida do documento-->
     </false>
  </if>
```

#### Tag Condition

##### SubTags

 - *Tag Expression* - Seu conteudo representa uma expressão onde o resultado deve ser do tipo numérico ou booleano.
 - *Tag Content*  - Guarda o conteudo que será exibido se a expressão retornar True ou um numero maior que 0.
 
##### Aplicação no Body do Template

```html

  <condition>
     <expression>
         2 > 3
     </expression>
     <content>
         <!--Conteudo se expressão for = true-->
     </content>   
  </if>
```


### Tag Template

##### SubTags

  - *Tag SQL* - Quando especificada, guarda uma Query SQL de onde serão extraidos os itens da coleção.
      - Atributo **File** - Quando especificado, utiliza um arquivo externo para o comando SQL ao invés do proprio conteúdo da tag.   
  - *Tag Content* - Quando especificado, guarda o conteúdo html deste subtemplate. Pode ter a mesma estrutura de tags um template pai.
      - Atributo **File** - Quando especificado, utiliza um arquivo externo para o conteudo do template ao invés do proprio conteúdo da tag.   
  - *Tag Property* - Quando especificada, indica que este template será construido a partir de uma propriedade enumeravel do template pai
      - Atributo **Name** - Nome da propriedade da classe pai onde se encontra a coleção utilizada para este subtemplate.
      - Atributo **Page** - Quando especificado e quando existir uma estrutura complexa, indica qual página será exibida neste subtemplate.
      - Atributo **Size** - Quando especificado e quando existir uma estrutura complexa, indica a quantidade de itens que serão exibidos por página neste subtemplate.


**OBS.: A Tag Property nunca deve ser usada junto com a tag SQL**

#### Aplicação no Body do Template

```html


  <!--Estrutura template com propriedade enumeravel e com template definido pela classe (propriedade TriforceTemplate)-->
    <template>
        <property name="PropriedadeDeLista" /> <!--Sim, é só isso! Considerando que PropriedadeDeLista seja um IEnumerable(Of NomeClasse) ou IQueryable(Of NomeClasse)-->
    </template>


    <!--Estrutura template inline com SQL-->
    <template>
        <sql>
            SELECT * FROM TABLE
            <!--query sql apenas para LINQ to SQL-->
        </sql>
        <content>
            <!--conteudo html cru ou template estruturado. Para tempaltes estruturados, aplicam-se as mesmas regras de estutura do template normalmente (tag head, footer, body, empty e pagination)-->
        </content>
    </template>



    <!--Estrutura template inline com propriedade enumeravel-->
    <template>
        <property name="Lista" page="1" size="20" /> <!--voce pode especificar a quantidade de itens por pagina e o numero da pagina (nao obrigatório) -->
        <content>
            <!--conteudo html cru ou template estruturado. Para tempaltes estruturados, aplicam-se as mesmas regras de estutura do template normalmente (tag head, footer, body, empty e pagination)-->
        </content>
    </template>


 

    <!--Estrutura Template a partir de Arquivos (SQL) -->
    <template>
        <sql file="query.sql" />
        <content file="temp.html" />
    </template>


  <!--Estrutura Template a partir de Arquivos (IEnumerable)-->
    <template>
        <property name="lista" />
        <content file="temp.html" />
    </template>

```


**Lembre-se que a tag Content pode conter a estrutura completa de um template**




