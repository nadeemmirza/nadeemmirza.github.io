namespace TransactionProcessor.Models;

/// <summary>
/// Represents transaction type information from the database
/// </summary>
public class TransactionTypeInfo
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
}
