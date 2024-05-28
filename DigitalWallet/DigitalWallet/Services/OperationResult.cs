namespace DigitalWallet.Services;

public class OperationResult
{
    public required bool Succeeded { get; init; }

    public IEnumerable<string> Errors { get; init; } = Array.Empty<string>();

    public static OperationResult Success { get; } = new() { Succeeded = true };

    public static OperationResult Failed(params string[] errors) => new() { Succeeded = false, Errors = errors };
}
