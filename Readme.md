# Cobertura de testes com Coverlet e Report Generator

Este projeto aborda o uso da cobertura de código para Teste de Unidades com [Coverlet](https://github.com/coverlet-coverage/coverlet) e geração de relatórios usando o [ReportGenerator](https://github.com/danielpalme/ReportGenerator). Embora este artigo se concentre em C# e xUnit como a estrutura de teste, o MSTest e o NUnit também funcionariam.  O coverlet é um projeto de software livre no GitHub  que fornece uma estrutura de cobertura de código para C#.  

Além disso, este projeto fornece detalhes sobre como usar as informações de cobertura de código coletadas de uma execução de teste do coverlet para gerar um relatório.  A geração de relatórios é possível usando o ReportGenerator.  O ReportGenerator converte relatórios de cobertura gerados por testes em relatórios legíveis por humanos.


## Execução dos Testes

No prompt de comando, altere os diretórios para o projeto _MaiorDeIdade.Tests_ e execute o comando:

```PowerShell
dotnet  test --collect:"XPlat Code Coverage"
```
Como resultado da execução do `dotnet test`, um arquivo _coverage.cobertura.xml_ é gerado no diretório _TestResults_ .

## Gerar relatórios

Agora que coletamos os dados de execuções de teste de unidade, podemos gerar os relatórios usando o ReportGenerator.  Para instalar o pacote use o comando:

```PowerShell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

O ReportGenerator tem três parâmetros como pode ser visto abaixo:
**-reports:** Caminho para o arquivo *coverage.cobertura.xml*
 **-targetdir:** Diretório para geração do relatório.
 **-reporttypes:** Formato de saída, mais opções de formatos podem ser vistas [aqui.](https://github.com/danielpalme/ReportGenerator)

Para gerar o relatório, de dentro da pasta **MaiorDeIdade.Tests** digite o comando abaixo:

```PowerShell
reportgenerator -reports:.\TestResults\*\coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```
*O asterisco no caminho do arquivo coverage.cobertura.xml, faz com que todos os arquivos coverage.cobertura.xml que estejam nas sub-pastas sejam usados no relatório, caso queira usar um coverage.cobertura.xml em especifico troque o asterisco pelo nome da pasta :)*


Depois de executar esse comando, um arquivo HTML que representa o relatório gerado vai estar na pasta **coveragereport**

Tela de resumo do projeto:

![](https://github.com/RodrigoDotNet/teste-de-cobertura/blob/master/images/cobertura.PNG)

Tela detalhada da classe e funções:

![](https://github.com/RodrigoDotNet/teste-de-cobertura/blob/master/images/cobertura2.PNG)

