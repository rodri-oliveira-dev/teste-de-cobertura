using MaiorDeIdade;
using Xunit;

namespace MaiorDeIdade.Tests;

public class ValidacaoIdadeTest
{
    [Theory(DisplayName = "Valida maioridade em cenários de fronteira")]
    [InlineData(2007, 9, 18, 2026, 9, 18, true)]
    [InlineData(2008, 9, 19, 2026, 9, 18, false)]
    [InlineData(2008, 9, 17, 2026, 9, 18, true)]
    [InlineData(2006, 9, 18, 2026, 9, 18, true)]
    [InlineData(2009, 9, 18, 2026, 9, 18, false)]
    [InlineData(2008, 8, 31, 2026, 9, 1, true)]
    [InlineData(2008, 9, 1, 2026, 8, 31, false)]
    [InlineData(2007, 12, 31, 2026, 1, 1, true)]
    [InlineData(2008, 1, 2, 2026, 1, 1, false)]
    public void VerificaMaiorDeIdade_DeveRetornarResultadoEsperado(
        int nascimentoAno,
        int nascimentoMes,
        int nascimentoDia,
        int referenciaAno,
        int referenciaMes,
        int referenciaDia,
        bool esperado)
    {
        // Arrange
        var dataNascimento = new DateTime(nascimentoAno, nascimentoMes, nascimentoDia);
        var timeProvider = CriarTimeProvider(referenciaAno, referenciaMes, referenciaDia);

        // Act
        var resultado = dataNascimento.VerificaMaiorDeIdade(timeProvider);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact(DisplayName = "Data de nascimento futura não é maior de idade")]
    public void VerificaMaiorDeIdade_DataNascimentoFutura_DeveRetornarFalso()
    {
        // Arrange
        var dataNascimento = new DateTime(2027, 1, 1);
        var timeProvider = CriarTimeProvider(2026, 9, 18);

        // Act
        var resultado = dataNascimento.VerificaMaiorDeIdade(timeProvider);

        // Assert
        Assert.False(resultado);
    }

    [Fact(DisplayName = "TimeProvider nulo lança ArgumentNullException")]
    public void VerificaMaiorDeIdade_TimeProviderNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        var dataNascimento = new DateTime(2000, 1, 1);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => dataNascimento.VerificaMaiorDeIdade(null!));

        // Assert
        Assert.Equal("timeProvider", exception.ParamName);
    }

    [Theory(DisplayName = "Valida maioridade para nascimento em ano bissexto")]
    [InlineData(2026, 2, 27, false)]
    [InlineData(2026, 2, 28, true)]
    [InlineData(2026, 3, 1, true)]
    public void VerificaMaiorDeIdade_NascimentoEm29DeFevereiro_DeveTratarAnoNaoBissexto(
        int referenciaAno,
        int referenciaMes,
        int referenciaDia,
        bool esperado)
    {
        // Arrange
        var dataNascimento = new DateTime(2008, 2, 29);
        var timeProvider = CriarTimeProvider(referenciaAno, referenciaMes, referenciaDia);

        // Act
        var resultado = dataNascimento.VerificaMaiorDeIdade(timeProvider);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    private static TimeProvider CriarTimeProvider(int ano, int mes, int dia)
    {
        var dataReferencia = new DateTimeOffset(ano, mes, dia, 12, 0, 0, TimeSpan.Zero);

        return new FixedTimeProvider(dataReferencia);
    }

    private sealed class FixedTimeProvider(DateTimeOffset dataReferencia) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => dataReferencia;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }
}
