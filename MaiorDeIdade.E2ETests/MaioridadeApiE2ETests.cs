using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace MaiorDeIdade.E2ETests;

public sealed class MaioridadeApiE2ETests
{
    [Fact(DisplayName = "POST /maioridade retorna true para adulto")]
    public async Task PostMaioridadeAdultoDeveRetornarTrue()
    {
        using var client = CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        using var response = await client.PostAsJsonAsync(
            "/maioridade",
            new VerificarMaioridadeRequest(new DateOnly(1900, 1, 1)),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VerificarMaioridadeResponse>(
            cancellationToken);

        Assert.NotNull(body);
        Assert.True(body.MaiorDeIdade);
    }

    [Fact(DisplayName = "POST /maioridade retorna false para data futura")]
    public async Task PostMaioridadeDataFuturaDeveRetornarFalse()
    {
        using var client = CreateClient();
        var cancellationToken = TestContext.Current.CancellationToken;

        using var response = await client.PostAsJsonAsync(
            "/maioridade",
            new VerificarMaioridadeRequest(new DateOnly(2999, 1, 1)),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VerificarMaioridadeResponse>(
            cancellationToken);

        Assert.NotNull(body);
        Assert.False(body.MaiorDeIdade);
    }

    [Fact(DisplayName = "POST /maioridade rejeita payload inválido")]
    public async Task PostMaioridadePayloadInvalidoDeveRetornarBadRequest()
    {
        using var client = CreateClient();
        using var content = new StringContent(
            """{"dataNascimento":"data-invalida"}""",
            Encoding.UTF8,
            "application/json");
        var cancellationToken = TestContext.Current.CancellationToken;

        using var response = await client.PostAsync(
            "/maioridade",
            content,
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static HttpClient CreateClient()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL");

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "A variável E2E_BASE_URL deve apontar para uma instância real da API.");
        }

        return new HttpClient
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    private sealed record VerificarMaioridadeRequest(DateOnly DataNascimento);

    private sealed record VerificarMaioridadeResponse(bool MaiorDeIdade);
}
