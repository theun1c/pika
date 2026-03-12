namespace Pika.Sandbox.Contracts.Responses;

// отключаем наследование 
public sealed record PlaybackResponse(
    string SourceUrl, 
    IReadOnlyList<PlayerResponse> Players 
    // используем IReadOnlyList потому что его незачем изменять
);

public sealed record PlayerResponse(
    string Name,
    IReadOnlyList<PlayerDataResponse> Data
);

public sealed record PlayerDataResponse(
    string Voice, 
    string Quality, 
    string M3U8Url, 
    DateTime ResolvedAt
);