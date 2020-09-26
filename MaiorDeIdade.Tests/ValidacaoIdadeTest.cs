using MaiorDeIdade;
using System;
using Xunit;

namespace XUnit.Coverlet.Collector
{
    public class ValidacaoIdadeTest
    {
        [Fact(DisplayName = "Valida nascimento maior de idade")]
        public void VerificarMaiorDeIdadeSucesso()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-19).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Valida nascimento futuro")]
        public void VerificarDataFuturaFalha()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(1).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Valida aniversário de maioridade ontem")]
        public void VerificarMaioridadeOntemPassa()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-18).AddDays(-1).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Valida aniversário mês passado")]
        public void VerificarMaioridadeMesPassado()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-18).AddMonths(-1).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Valida aniversário de hoje")]
        public void VerificarMaioridadeHojePassa()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-18).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Valida aniversário de maioridade amanhã")]
        public void VerificarMaioridadeAmanhaFalha()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-18).AddDays(1).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Valida nascimento menor de idade")]
        public void VerificarMenorDeIdadeFalha()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-17).Date;

            // Act
            var resultado = dataNascimento.VerificaMaiorDeIdade();

            // Assert
            Assert.False(resultado);
        }
    }
}
