## Resumo

Descreva o que mudou e por quê.

## Issue relacionada

Closes #

## Validação

Descreva os checks executados e eventuais limitações.

## Checklist

- [ ] O escopo e a motivação do PR estão descritos acima.
- [ ] Testes foram adicionados ou atualizados para mudanças de comportamento, ou foi explicado por que não são necessários.
- [ ] `dotnet tool restore` foi executado com sucesso.
- [ ] `dotnet restore XUnit.Coverage.sln` foi executado com sucesso.
- [ ] `dotnet build XUnit.Coverage.sln --configuration Release --no-restore` foi executado com sucesso.
- [ ] `dotnet test XUnit.Coverage.sln --configuration Release --no-build` foi executado com sucesso.
- [ ] Alterações que afetam testes ou regra de negócio consideraram cobertura e mutation testing quando aplicável.
- [ ] Nenhum segredo, credencial, dado pessoal ou informação sensível foi incluído.
