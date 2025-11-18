using TransactionProcessor.Models;

namespace TransactionProcessor.Services;

/// <summary>
/// Interface for accessing transaction type data from the database
/// </summary>
public interface ITransactionTypeRepository
{
    /// <summary>
    /// Get all transaction types from the database
    /// </summary>
    Task<IEnumerable<TransactionTypeInfo>> GetAllTransactionTypesAsync();
    
    /// <summary>
    /// Get a specific transaction type by code (used for individual lookups - not recommended for batch processing)
    /// </summary>
    Task<TransactionTypeInfo?> GetTransactionTypeByCodeAsync(string code);
}
