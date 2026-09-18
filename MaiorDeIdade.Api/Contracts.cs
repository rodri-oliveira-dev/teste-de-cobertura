namespace MaiorDeIdade.Api;

public sealed record VerificarMaioridadeRequest(DateOnly DataNascimento);

public sealed record VerificarMaioridadeResponse(bool MaiorDeIdade);
