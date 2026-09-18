namespace MaiorDeIdade;

public static class ValidacaoIdade
{
    private const int IdadeMinima = 18;

    public static bool VerificaMaiorDeIdade(this DateTime dataNascimento, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);

        var nascimento = DateOnly.FromDateTime(dataNascimento);
        var hoje = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

        if (nascimento > hoje)
        {
            return false;
        }

        var dataMaioridade = nascimento.AddYears(IdadeMinima);

        return dataMaioridade <= hoje;
    }
}
