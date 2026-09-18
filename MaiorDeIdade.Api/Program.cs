using MaiorDeIdade;
using MaiorDeIdade.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.MapPost("/maioridade", (VerificarMaioridadeRequest request, TimeProvider timeProvider) =>
{
    var dataNascimento = request.DataNascimento.ToDateTime(TimeOnly.MinValue);
    var maiorDeIdade = dataNascimento.VerificaMaiorDeIdade(timeProvider);

    return Results.Ok(new VerificarMaioridadeResponse(maiorDeIdade));
});

app.Run();

public partial class Program
{
}
