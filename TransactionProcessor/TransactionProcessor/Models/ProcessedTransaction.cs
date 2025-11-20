namespace TransactionProcessor.Models;

/// <summary>
/// Represents a transaction with its type description populated
/// </summary>
public class ProcessedTransaction
{
    public int Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string TransactionDescription { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? Category { get; set; }
}
