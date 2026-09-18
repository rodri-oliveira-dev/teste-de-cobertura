using System.Net;
using System.Net.Http.Json;
using System.Text;
using MaiorDeIdade.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace MaiorDeIdade.IntegrationTests;

public class MaioridadeEndpointTests
{
    private static readonly DateTimeOffset DataReferencia =
        new(2026, 9, 18, 12, 0, 0, TimeSpan.Zero);

    [Fact(DisplayName = "POST /maioridade retorna true para exatamente 18 anos")]
    public async Task PostMaioridade_ExatamenteDezoitoAnos_DeveRetornarTrue()
    {
        using var factory = new MaiorDeIdadeApiFactory(DataReferencia);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/maioridade",
            new VerificarMaioridadeRequest(new DateOnly(2008, 9, 18)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VerificarMaioridadeResponse>();

        Assert.NotNull(body);
        Assert.True(body!.MaiorDeIdade);
    }

    [Fact(DisplayName = "POST /maioridade retorna false antes dos 18 anos")]
    public async Task PostMaioridade_AntesDosDezoitoAnos_DeveRetornarFalse()
    {
        using var factory = new MaiorDeIdadeApiFactory(DataReferencia);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/maioridade",
            new VerificarMaioridadeRequest(new DateOnly(2008, 9, 19)));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VerificarMaioridadeResponse>();

        Assert.NotNull(body);
        Assert.False(body!.MaiorDeIdade);
    }

    [Fact(DisplayName = "POST /maioridade rejeita data inválida")]
    public async Task PostMaioridade_DataInvalida_DeveRetornarBadRequest()
    {
        using var factory = new MaiorDeIdadeApiFactory(DataReferencia);
        using var client = factory.CreateClient();
        using var content = new StringContent(
            """{"dataNascimento":"data-invalida"}""",
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/maioridade", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed class MaiorDeIdadeApiFactory(DateTimeOffset dataReferencia)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(new FixedTimeProvider(dataReferencia));
            });
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset dataReferencia) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => dataReferencia;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }
}
