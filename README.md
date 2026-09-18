# Pirâmide de testes e cobertura em .NET

[![CI](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/ci.yml)
[![Mutation Testing](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/mutation-testing.yml/badge.svg)](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/mutation-testing.yml)
[![CodeQL](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/codeql.yml)
[![Dependency Review](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/dependency-review.yml/badge.svg?event=pull_request)](https://github.com/rodri-oliveira-dev/dotnet-code-coverage/actions/workflows/dependency-review.yml)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Projeto didático para demonstrar **testes automatizados, cobertura de código, mutation testing e quality gates em .NET 10** usando xUnit v3, Microsoft Testing Platform, Coverlet, ReportGenerator e Stryker.NET.

## Contexto do projeto

Este repositório nasceu como um exemplo simples usado em uma apresentação sobre a **pirâmide de testes**. O domínio foi mantido propositalmente pequeno: uma regra que verifica se uma pessoa atingiu a maioridade.

A versão atual moderniza aquele exemplo em duas camadas: uma base maior de **testes unitários rápidos e determinísticos** e uma camada menor de **testes de integração HTTP** que valida a composição real da aplicação.

```text
              E2E
             /   \
            /     \
           /       \
          / Integração\      ← poucos cenários HTTP
         /-----------\
        /             \
       /   Unitários   \     ← maior parte da suíte
      /_________________\
```

A Minimal API adicionada ao exemplo cria uma fronteira HTTP real para demonstrar integração entre serialização JSON, model binding, DI, `TimeProvider` e a regra de negócio. Ainda não há testes E2E, porque não existe interface externa ou fluxo distribuído que justifique essa camada.

## Objetivo

Este projeto mostra, de forma reproduzível, como:

- escrever testes unitários determinísticos para regras dependentes de data;
- controlar o relógio da aplicação com `TimeProvider`;
- testar cenários de fronteira, incluindo exatamente 18 anos e anos bissextos;
- testar a integração HTTP da Minimal API com `WebApplicationFactory`;
- substituir dependências de infraestrutura, como `TimeProvider`, durante testes de integração;
- coletar line e branch coverage;
- gerar relatórios de cobertura em HTML;
- aplicar um quality gate de cobertura no CI;
- usar mutation testing para avaliar a efetividade dos testes;
- executar análise estática com CodeQL;
- revisar alterações de dependências automaticamente.

## Tecnologias

- .NET 10
- C#
- xUnit v3
- Microsoft Testing Platform
- Coverlet MTP
- ReportGenerator
- Stryker.NET 5
- ASP.NET Core Minimal API
- Microsoft.AspNetCore.Mvc.Testing
- GitHub Actions
- CodeQL
- Dependency Review

## Estrutura do projeto

```text
.
├── .config/
│   └── dotnet-tools.json
├── .github/
│   ├── ISSUE_TEMPLATE/
│   ├── dependabot.yml
│   ├── pull_request_template.md
│   └── workflows/
│       ├── ci.yml
│       ├── codeql.yml
│       ├── dependency-review.yml
│       └── mutation-testing.yml
├── MaiorDeIdade/
│   ├── MaiorDeIdade.csproj
│   └── ValidacaoIdade.cs
├── MaiorDeIdade.Api/
│   ├── MaiorDeIdade.Api.csproj
│   ├── Contracts.cs
│   └── Program.cs
├── MaiorDeIdade.Tests/
│   ├── MaiorDeIdade.Tests.csproj
│   ├── stryker-config.json
│   └── ValidacaoIdadeTest.cs
├── MaiorDeIdade.IntegrationTests/
│   ├── MaiorDeIdade.IntegrationTests.csproj
│   └── MaioridadeEndpointTests.cs
├── .editorconfig
├── .gitattributes
├── Directory.Build.props
├── DotNet.CodeCoverage.slnx
├── dotnet-code-coverage.code-workspace
├── global.json
└── LICENSE
```

## Ambiente de desenvolvimento

O repositório fixa o SDK em `10.0.400` via `global.json`, com `rollForward: latestFeature`. As convenções compartilhadas ficam em `.editorconfig` e `.gitattributes`, enquanto `Directory.Build.props` centraliza warnings como errors, analyzers do .NET 10, NuGet audit e build determinístico.

Para VS Code, abra `dotnet-code-coverage.code-workspace`. O workspace versionado oferece tasks para restore, build, testes unitários, testes de integração e cobertura, além de uma configuração de debug para `MaiorDeIdade.Api`.

A solution usa o formato `.slnx`, padrão para novas solutions no .NET 10.

## Executar os testes

Pré-requisito: SDK do .NET 10 instalado.

Restaure as dependências e execute os testes:

```bash
dotnet restore DotNet.CodeCoverage.slnx
dotnet test DotNet.CodeCoverage.slnx
```

Os testes unitários usam um `TimeProvider` controlado. Dessa forma, cenários como exatamente 18 anos, aniversário amanhã, virada de mês, virada de ano, data futura e nascimento em 29 de fevereiro não dependem da data real da máquina.

## Testes de integração

A Minimal API expõe `POST /maioridade` e usa a mesma regra de negócio do projeto `MaiorDeIdade`.

Os testes de integração sobem a aplicação em memória com `WebApplicationFactory<Program>` e exercitam o fluxo completo:

```text
HTTP → JSON/model binding → DI → TimeProvider → regra de negócio → resposta HTTP
```

A suíte cobre três cenários representativos:

- exatamente 18 anos → `200 OK` e `maiorDeIdade: true`;
- ainda menor de idade → `200 OK` e `maiorDeIdade: false`;
- data inválida no JSON → `400 Bad Request`.

O `TimeProvider.System` registrado pela aplicação é substituído por um relógio fixo durante os testes, mantendo a integração determinística sem mockar o endpoint ou a regra de negócio.

## Gerar cobertura localmente

Restaure primeiro as ferramentas locais versionadas no repositório:

```bash
dotnet tool restore
```

Execute os testes coletando cobertura no formato Cobertura:

```bash
dotnet test DotNet.CodeCoverage.slnx \
  --configuration Release \
  --results-directory artifacts/test-results \
  --coverlet \
  --coverlet-output-format cobertura
```

Depois gere o relatório:

```bash
dotnet reportgenerator \
  -reports:"artifacts/test-results/**/coverage.cobertura*.xml" \
  -targetdir:"artifacts/coverage-report" \
  -reporttypes:"Html;TextSummary"
```

Abra `artifacts/coverage-report/index.html` para visualizar o relatório HTML.

## Quality gate de cobertura

O CI exige no mínimo:

| Métrica | Mínimo |
| --- | ---: |
| Line coverage | 90% |
| Branch coverage | 90% |

O ReportGenerator combina os relatórios dos testes unitários e de integração antes de validar esses thresholds. Se uma das métricas ficar abaixo do mínimo, o job falha.

## Mutation testing

Code coverage responde **quais partes do código foram executadas**. Mutation testing verifica **se os testes percebem quando o comportamento do código é alterado**.

Para executar a análise:

```bash
dotnet tool restore
cd MaiorDeIdade.Tests
dotnet stryker --output ../artifacts/mutation-testing
```

A configuração está em `MaiorDeIdade.Tests/stryker-config.json` e usa:

- mutation level `Standard`;
- Microsoft Testing Platform como runner;
- relatórios HTML, JSON e Markdown;
- threshold visual `high = 90` e `low = 80`;
- `break = 0`, portanto mutation score não é usado como quality gate rígido.

Os relatórios são gerados em `artifacts/mutation-testing`.

> O runner MTP do Stryker.NET ainda é tratado como preview. Por isso, mutation testing é usado como sinal complementar de qualidade, enquanto o quality gate obrigatório permanece baseado em testes e cobertura.

## Entendendo as métricas

**Line coverage** mede quantas linhas instrumentadas foram executadas pela suíte de testes.

**Branch coverage** mede quantos caminhos condicionais foram exercitados, como os resultados verdadeiro e falso de uma decisão.

**Mutation score** mede a proporção de mutantes detectados pela suíte. Um mutante sobrevivente pode indicar uma lacuna de teste, mas também pode representar uma mutação equivalente ou sem impacto observável e deve ser analisado antes de qualquer alteração nos testes.

## Cobertura não é qualidade

**100% de cobertura não significa, por si só, que os testes são bons.**

Cobertura indica quais trechos foram executados, mas não comprova que:

- os asserts verificam o comportamento correto;
- cenários relevantes foram escolhidos;
- regras de negócio foram interpretadas corretamente;
- regressões semânticas serão detectadas.

Mutation testing complementa essa análise ao modificar o código de produção e verificar se a suíte falha. O objetivo não é maximizar uma métrica isolada, mas identificar testes incapazes de detectar regressões relevantes.

## Automação e segurança

O workflow `.github/workflows/ci.yml` executa em pull requests e pushes para `main` e realiza:

1. restore das ferramentas locais;
2. restore das dependências;
3. validação de formatação com `dotnet format --verify-no-changes`;
4. build em Release;
5. execução dos testes unitários e de integração;
6. coleta de cobertura de ambos os projetos com Coverlet MTP;
7. combinação dos relatórios com ReportGenerator;
8. publicação do resumo no GitHub Actions;
9. validação do quality gate;
10. upload do artifact `coverage-report`.

Além do CI principal:

- **Mutation Testing** executa em mudanças relacionadas ao código, testes ou configuração do Stryker;
- **CodeQL** realiza análise estática de segurança;
- **Dependency Review** verifica alterações de dependências em pull requests;
- **Dependabot** verifica atualizações de NuGet e GitHub Actions.

As GitHub Actions utilizadas nos workflows são fixadas por SHA para reduzir risco de alterações inesperadas em dependências de CI.

## Licença

Distribuído sob a licença MIT. Consulte [LICENSE](LICENSE).
