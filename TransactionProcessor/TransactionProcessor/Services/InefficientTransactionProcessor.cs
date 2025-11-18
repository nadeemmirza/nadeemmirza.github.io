using TransactionProcessor.Models;

namespace TransactionProcessor.Services;

/// <summary>
/// INEFFICIENT implementation that demonstrates the N+1 query problem
/// This approach is NOT RECOMMENDED but shown for comparison
/// </summary>
public class InefficientTransactionProcessor : ITransactionProcessor
{
    private readonly ITransactionTypeRepository _repository;

    public InefficientTransactionProcessor(ITransactionTypeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>
    /// INEFFICIENT: Makes a database call for EACH transaction (N+1 problem)
    /// DO NOT USE this approach in production!
    /// </summary>
    public async Task<IEnumerable<ProcessedTransaction>> ProcessTransactionsAsync(IEnumerable<Transaction> transactions)
    {
        if (transactions == null)
            throw new ArgumentNullException(nameof(transactions));

        var transactionList = transactions.ToList();
        var processedTransactions = new List<ProcessedTransaction>();

        // PROBLEM: This loops through each transaction and makes a separate database call
        // If you have 1000 transactions, this makes 1000 database queries!
        foreach (var transaction in transactionList)
        {
            // Each call here hits the database - very inefficient!
            var typeInfo = await _repository.GetTransactionTypeByCodeAsync(transaction.TransactionType);

            processedTransactions.Add(new ProcessedTransaction
            {
                Id = transaction.Id,
                TransactionType = transaction.TransactionType,
                TransactionDescription = typeInfo?.Description ?? "Unknown Transaction Type",
                TransactionDate = transaction.TransactionDate,
                Amount = transaction.Amount,
                Reference = transaction.Reference,
                Category = typeInfo?.Category
            });
        }

        return processedTransactions;
    }
}
