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

A versão atual moderniza aquele exemplo em três camadas: uma base maior de **testes unitários rápidos e determinísticos**, uma camada menor de **testes de integração HTTP** e poucos **testes E2E black-box** contra o processo real da API.

```text
              E2E             ← 3 cenários black-box
             /---\
            /     \
           /Integração\       ← poucos cenários in-memory
          /---------\
         /           \
        /  Unitários  \       ← maior parte da suíte
       /_______________\
```

A Minimal API cria uma fronteira HTTP real para demonstrar integração entre serialização JSON, model binding, DI, `TimeProvider` e a regra de negócio. A camada E2E executa o DLL compilado em um processo Kestrel real e acessa a aplicação exclusivamente por HTTP, sem referência aos projetos internos.

## Objetivo

Este projeto mostra, de forma reproduzível, como:

- escrever testes unitários determinísticos para regras dependentes de data;
- controlar o relógio da aplicação com `TimeProvider`;
- testar cenários de fronteira, incluindo exatamente 18 anos e anos bissextos;
- testar a integração HTTP da Minimal API com `WebApplicationFactory`;
- substituir dependências de infraestrutura, como `TimeProvider`, durante testes de integração;
- testar o artefato compilado de ponta a ponta em Kestrel real;
- manter testes E2E desacoplados dos assemblies internos;
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
├── MaiorDeIdade.E2ETests/
│   ├── MaiorDeIdade.E2ETests.csproj
│   └── MaioridadeApiE2ETests.cs
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

Para VS Code, abra `dotnet-code-coverage.code-workspace`. O workspace versionado oferece tasks para restore, build, testes unitários, testes de integração, E2E e cobertura, além de uma configuração de debug para `MaiorDeIdade.Api`. Para o task E2E, inicie antes a API pela configuração de debug.

A solution usa o formato `.slnx`, padrão para novas solutions no .NET 10.

## Executar os testes

Pré-requisito: SDK do .NET 10 instalado.

Restaure as dependências, compile a solution e execute as camadas rápidas:

```bash
dotnet restore DotNet.CodeCoverage.slnx
dotnet build DotNet.CodeCoverage.slnx
dotnet test MaiorDeIdade.Tests/MaiorDeIdade.Tests.csproj
dotnet test MaiorDeIdade.IntegrationTests/MaiorDeIdade.IntegrationTests.csproj
```

Os E2E são executados separadamente porque dependem de uma instância real da API em execução.

Os testes unitários usam um `TimeProvider` controlado. Dessa forma, cenários como exatamente 18 anos, aniversário amanhã, virada de mês, virada de ano, data futura e nascimento em 29 de fevereiro não dependem da data real da máquina.

## Testes de integração

A Minimal API expõe `POST /maioridade` e usa a mesma regra de negócio do projeto `MaiorDeIdade`.

Os testes de integração sobem a aplicação em memória com `WebApplicationFactory<Program>` e exercitam o fluxo completo:

```text
HTTP → JSON/model binding → DI → TimeProvider → regra de negócio → resposta HTTP
```

A suíte cobre quatro cenários representativos:

- health check → `200 OK`;
- exatamente 18 anos → `200 OK` e `maiorDeIdade: true`;
- ainda menor de idade → `200 OK` e `maiorDeIdade: false`;
- data inválida no JSON → `400 Bad Request`.

O `TimeProvider.System` registrado pela aplicação é substituído por um relógio fixo durante os testes, mantendo a integração determinística sem mockar o endpoint ou a regra de negócio.

## Testes E2E

O projeto `MaiorDeIdade.E2ETests` não possui `ProjectReference` para a API ou para a regra de negócio. Ele atua como um consumidor externo e conhece apenas `E2E_BASE_URL`.

O CI executa o fluxo real:

```text
DLL compilado → processo Kestrel → porta HTTP → JSON → DI → regra → resposta HTTP
```

Os três cenários E2E são propositalmente amplos:

- adulto inequívoco (`1900-01-01`) → `200 OK` e `true`;
- data futura (`2999-01-01`) → `200 OK` e `false`;
- payload inválido → `400 Bad Request`.

Para executar localmente, inicie a API em um terminal:

```bash
dotnet run --project MaiorDeIdade.Api/MaiorDeIdade.Api.csproj \
  --configuration Release \
  --urls http://127.0.0.1:5055
```

Em outro terminal, no Bash:

```bash
E2E_BASE_URL=http://127.0.0.1:5055 \
  dotnet test MaiorDeIdade.E2ETests/MaiorDeIdade.E2ETests.csproj --configuration Release
```

No PowerShell:

```powershell
$env:E2E_BASE_URL = "http://127.0.0.1:5055"
dotnet test MaiorDeIdade.E2ETests/MaiorDeIdade.E2ETests.csproj --configuration Release
```

Os E2E ficam **fora da métrica de code coverage**. Essa camada valida comportamento observável do sistema em execução, enquanto line/branch coverage continuam medindo os testes unitários e de integração.

## Gerar cobertura localmente

Restaure primeiro as ferramentas locais versionadas no repositório:

```bash
dotnet tool restore
```

Execute unitários e integração em diretórios separados:

```bash
dotnet test MaiorDeIdade.Tests/MaiorDeIdade.Tests.csproj \
  --configuration Release \
  --results-directory artifacts/test-results/unit \
  --coverlet \
  --coverlet-output-format cobertura

dotnet test MaiorDeIdade.IntegrationTests/MaiorDeIdade.IntegrationTests.csproj \
  --configuration Release \
  --results-directory artifacts/test-results/integration \
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
10. inicialização do DLL real da API em Kestrel;
11. espera ativa pelo endpoint `/health`;
12. execução dos testes E2E black-box;
13. upload do artifact `coverage-report` e, em falhas E2E, dos logs de diagnóstico.

Além do CI principal:

- **Mutation Testing** executa em mudanças relacionadas ao código, testes ou configuração do Stryker;
- **CodeQL** realiza análise estática de segurança;
- **Dependency Review** verifica alterações de dependências em pull requests;
- **Dependabot** verifica atualizações de NuGet e GitHub Actions.

As GitHub Actions utilizadas nos workflows são fixadas por SHA para reduzir risco de alterações inesperadas em dependências de CI.

## Licença

Distribuído sob a licença MIT. Consulte [LICENSE](LICENSE).
