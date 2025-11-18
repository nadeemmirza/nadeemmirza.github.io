namespace TransactionProcessor.Models;

/// <summary>
/// Represents a financial transaction
/// </summary>
public class Transaction
{
    public int Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}
