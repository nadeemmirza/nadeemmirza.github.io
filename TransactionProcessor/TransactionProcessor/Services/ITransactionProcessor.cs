using TransactionProcessor.Models;

namespace TransactionProcessor.Services;

/// <summary>
/// Interface for processing transactions with transaction type lookups
/// </summary>
public interface ITransactionProcessor
{
    /// <summary>
    /// Process a list of transactions and enrich them with transaction type descriptions
    /// </summary>
    Task<IEnumerable<ProcessedTransaction>> ProcessTransactionsAsync(IEnumerable<Transaction> transactions);
}
