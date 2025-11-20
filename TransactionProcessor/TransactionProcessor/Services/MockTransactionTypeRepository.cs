using TransactionProcessor.Models;

namespace TransactionProcessor.Services;

/// <summary>
/// Mock implementation of transaction type repository for demonstration purposes
/// In a real application, this would query a database using Entity Framework Core or similar
/// </summary>
public class MockTransactionTypeRepository : ITransactionTypeRepository
{
    private static readonly List<TransactionTypeInfo> _transactionTypes = new()
    {
        new TransactionTypeInfo { Code = "DEP", Description = "Deposit", Category = "Income" },
        new TransactionTypeInfo { Code = "WTH", Description = "Withdrawal", Category = "Expense" },
        new TransactionTypeInfo { Code = "TRF", Description = "Transfer", Category = "Transfer" },
        new TransactionTypeInfo { Code = "FEE", Description = "Service Fee", Category = "Expense" },
        new TransactionTypeInfo { Code = "INT", Description = "Interest Payment", Category = "Income" },
        new TransactionTypeInfo { Code = "CHG", Description = "Charge", Category = "Expense" },
        new TransactionTypeInfo { Code = "REF", Description = "Refund", Category = "Income" },
        new TransactionTypeInfo { Code = "PAY", Description = "Payment", Category = "Expense" }
    };

    public Task<IEnumerable<TransactionTypeInfo>> GetAllTransactionTypesAsync()
    {
        // Simulate database delay
        return Task.FromResult<IEnumerable<TransactionTypeInfo>>(_transactionTypes);
    }

    public Task<TransactionTypeInfo?> GetTransactionTypeByCodeAsync(string code)
    {
        // Simulate database delay
        var result = _transactionTypes.FirstOrDefault(tt => 
            tt.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result);
    }
}
