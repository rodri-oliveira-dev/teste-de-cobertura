# Cobertura de código em .NET

[![CI](https://github.com/rodri-oliveira-dev/teste-de-cobertura/actions/workflows/ci.yml/badge.svg)](https://github.com/rodri-oliveira-dev/teste-de-cobertura/actions/workflows/ci.yml)

Projeto de referência para demonstrar **testes automatizados, cobertura de código e quality gates em .NET 10** usando xUnit v3, Microsoft Testing Platform, Coverlet e ReportGenerator.

O domínio é propositalmente pequeno: uma regra que verifica se uma pessoa atingiu a maioridade. O foco do repositório está nas práticas de teste e qualidade, não na complexidade da regra de negócio.

## Objetivo

Este projeto mostra, de forma reproduzível, como:

- escrever testes determinísticos para regras dependentes de data;
- controlar o relógio da aplicação com `TimeProvider`;
- coletar line e branch coverage;
- gerar relatórios de cobertura em HTML;
- publicar relatórios como artifacts no GitHub Actions;
- aplicar um quality gate de cobertura no CI.

## Tecnologias

- .NET 10
- C#
- xUnit v3
- Microsoft Testing Platform
- Coverlet MTP
- ReportGenerator
- GitHub Actions

## Estrutura do projeto

```text
.
├── .config/
│   └── dotnet-tools.json
├── .github/
│   └── workflows/
│       └── ci.yml
├── MaiorDeIdade/
│   ├── MaiorDeIdade.csproj
│   └── ValidacaoIdade.cs
├── MaiorDeIdade.Tests/
│   ├── MaiorDeIdade.Tests.csproj
│   └── ValidacaoIdadeTest.cs
├── XUnit.Coverage.sln
├── global.json
└── LICENSE
```

## Executar os testes

Pré-requisito: SDK do .NET 10 instalado.

Restaure as dependências e execute os testes:

```bash
dotnet restore XUnit.Coverage.sln
dotnet test XUnit.Coverage.sln
```

A suíte usa um `TimeProvider` controlado pelos testes. Dessa forma, cenários como aniversário hoje, amanhã, virada de mês, virada de ano e nascimento em 29 de fevereiro não dependem da data real da máquina.

## Gerar cobertura localmente

Restaure primeiro as ferramentas locais versionadas no repositório:

```bash
dotnet tool restore
```

Execute os testes coletando cobertura no formato Cobertura:

```bash
dotnet test XUnit.Coverage.sln \
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

## Quality gate

O CI exige no mínimo:

| Métrica | Mínimo |
| --- | ---: |
| Line coverage | 90% |
| Branch coverage | 90% |

O ReportGenerator valida esses thresholds durante o workflow. Se uma das métricas ficar abaixo do mínimo, o job falha.

## Entendendo as métricas

**Line coverage** mede quantas linhas instrumentadas foram executadas pela suíte de testes.

**Branch coverage** mede quantos caminhos condicionais foram exercitados, como os resultados verdadeiro e falso de uma decisão.

Branch coverage costuma revelar lacunas que line coverage isoladamente não evidencia. Uma linha pode ter sido executada sem que todos os caminhos lógicos associados a ela tenham sido validados.

## Cobertura não é qualidade

**100% de cobertura não significa, por si só, que os testes são bons.**

Cobertura indica quais trechos foram executados, mas não comprova que:

- os asserts verificam o comportamento correto;
- cenários relevantes foram escolhidos;
- regras de negócio foram interpretadas corretamente;
- regressões semânticas serão detectadas.

Por isso, este projeto trata cobertura como **sinal de qualidade**, não como objetivo isolado. A efetividade dos testes será complementada por mutation testing na evolução prevista do repositório.

## CI

O workflow `.github/workflows/ci.yml` executa em pull requests e pushes para a branch principal e realiza:

1. restore das ferramentas locais;
2. restore das dependências;
3. build em Release;
4. execução dos testes;
5. coleta de cobertura com Coverlet MTP;
6. geração dos relatórios com ReportGenerator;
7. publicação do resumo no GitHub Actions;
8. validação do quality gate;
9. upload do artifact `coverage-report`.

As GitHub Actions utilizadas no workflow são fixadas por SHA para reduzir risco de alterações inesperadas em dependências de CI.

## Licença

Distribuído sob a licença MIT. Consulte [LICENSE](LICENSE).
