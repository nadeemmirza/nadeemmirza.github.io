using TransactionProcessor.Models;

namespace TransactionProcessor.Services;

/// <summary>
/// Efficient implementation of transaction processor using in-memory caching
/// This is the BEST PRACTICE approach to avoid N+1 query problems
/// </summary>
public class EfficientTransactionProcessor : ITransactionProcessor
{
    private readonly ITransactionTypeRepository _repository;

    public EfficientTransactionProcessor(ITransactionTypeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// Process transactions efficiently by loading all transaction types once
    /// and using a dictionary for O(1) lookups
    /// </summary>
    public async Task<IEnumerable<ProcessedTransaction>> ProcessTransactionsAsync(IEnumerable<Transaction> transactions)
    {
        if (transactions == null)
            throw new ArgumentNullException(nameof(transactions));

        var transactionList = transactions.ToList();
        
        if (!transactionList.Any())
            return Enumerable.Empty<ProcessedTransaction>();

        // BEST PRACTICE: Load all transaction types ONCE from the database
        // and store them in a dictionary for fast lookups
        var allTransactionTypes = await _repository.GetAllTransactionTypesAsync();
        var transactionTypeLookup = allTransactionTypes.ToDictionary(
            tt => tt.Code, 
            tt => tt,
            StringComparer.OrdinalIgnoreCase // Case-insensitive comparison
        );

        // Process each transaction using the in-memory dictionary
        // This is O(n) instead of O(n*m) where m is database query time
        // Skip transactions where the transaction type is not found in the lookup
        var processedTransactions = transactionList
            .Where(transaction => transactionTypeLookup.ContainsKey(transaction.TransactionType))
            .Select(transaction =>
            {
                var typeInfo = transactionTypeLookup[transaction.TransactionType];
                
                return new ProcessedTransaction
                {
                    Id = transaction.Id,
                    TransactionType = transaction.TransactionType,
                    TransactionDescription = typeInfo.Description,
                    TransactionDate = transaction.TransactionDate,
                    Amount = transaction.Amount,
                    Reference = transaction.Reference,
                    Category = typeInfo.Category
                };
            });

        return processedTransactions;
    }
}
