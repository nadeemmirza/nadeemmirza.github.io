using TransactionProcessor.Models;
using TransactionProcessor.Services;
using Xunit;

namespace TransactionProcessor.Tests;

public class EfficientTransactionProcessorTests
{
    [Fact]
    public async Task ProcessTransactionsAsync_WithValidTransactions_ReturnsProcessedTransactions()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);
        var transactions = new List<Transaction>
        {
            new() { Id = 1, TransactionType = "DEP", TransactionDate = DateTime.Now, Amount = 100.00m },
            new() { Id = 2, TransactionType = "WTH", TransactionDate = DateTime.Now, Amount = 50.00m },
            new() { Id = 3, TransactionType = "TRF", TransactionDate = DateTime.Now, Amount = 75.00m }
        };

        // Act
        var result = await processor.ProcessTransactionsAsync(transactions);
        var resultList = result.ToList();

        // Assert
        Assert.Equal(3, resultList.Count);
        Assert.Equal("Deposit", resultList[0].TransactionDescription);
        Assert.Equal("Withdrawal", resultList[1].TransactionDescription);
        Assert.Equal("Transfer", resultList[2].TransactionDescription);
    }

    [Fact]
    public async Task ProcessTransactionsAsync_WithUnknownTransactionType_ReturnsUnknownDescription()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);
        var transactions = new List<Transaction>
        {
            new() { Id = 1, TransactionType = "UNKNOWN", TransactionDate = DateTime.Now, Amount = 100.00m }
        };

        // Act
        var result = await processor.ProcessTransactionsAsync(transactions);
        var resultList = result.ToList();

        // Assert
        Assert.Single(resultList);
        Assert.Equal("Unknown Transaction Type", resultList[0].TransactionDescription);
        Assert.Null(resultList[0].Category);
    }

    [Fact]
    public async Task ProcessTransactionsAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);
        var transactions = new List<Transaction>();

        // Act
        var result = await processor.ProcessTransactionsAsync(transactions);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ProcessTransactionsAsync_WithNullTransactions_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => processor.ProcessTransactionsAsync(null!));
    }

    [Fact]
    public async Task ProcessTransactionsAsync_WithMixedCaseTransactionTypes_HandlesCorrectly()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);
        var transactions = new List<Transaction>
        {
            new() { Id = 1, TransactionType = "dep", TransactionDate = DateTime.Now, Amount = 100.00m },
            new() { Id = 2, TransactionType = "DEP", TransactionDate = DateTime.Now, Amount = 150.00m },
            new() { Id = 3, TransactionType = "Dep", TransactionDate = DateTime.Now, Amount = 200.00m }
        };

        // Act
        var result = await processor.ProcessTransactionsAsync(transactions);
        var resultList = result.ToList();

        // Assert
        Assert.Equal(3, resultList.Count);
        Assert.All(resultList, r => Assert.Equal("Deposit", r.TransactionDescription));
    }

    [Fact]
    public async Task ProcessTransactionsAsync_PreservesAllTransactionProperties()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);
        var testDate = new DateTime(2023, 5, 15);
        var transactions = new List<Transaction>
        {
            new() 
            { 
                Id = 42, 
                TransactionType = "PAY", 
                TransactionDate = testDate, 
                Amount = 123.45m,
                Reference = "TEST-REF-001"
            }
        };

        // Act
        var result = await processor.ProcessTransactionsAsync(transactions);
        var processed = result.First();

        // Assert
        Assert.Equal(42, processed.Id);
        Assert.Equal("PAY", processed.TransactionType);
        Assert.Equal(testDate, processed.TransactionDate);
        Assert.Equal(123.45m, processed.Amount);
        Assert.Equal("TEST-REF-001", processed.Reference);
        Assert.Equal("Payment", processed.TransactionDescription);
        Assert.Equal("Expense", processed.Category);
    }

    [Fact]
    public async Task ProcessTransactionsAsync_WithLargeNumberOfTransactions_ProcessesEfficiently()
    {
        // Arrange
        var repository = new MockTransactionTypeRepository();
        var processor = new EfficientTransactionProcessor(repository);
        var transactions = Enumerable.Range(1, 1000).Select(i => new Transaction
        {
            Id = i,
            TransactionType = i % 2 == 0 ? "DEP" : "WTH",
            TransactionDate = DateTime.Now,
            Amount = i * 10.0m
        }).ToList();

        // Act
        var result = await processor.ProcessTransactionsAsync(transactions);
        var resultList = result.ToList();

        // Assert
        Assert.Equal(1000, resultList.Count);
        // Even IDs (2, 4, 6...) should be Deposits, odd IDs (1, 3, 5...) should be Withdrawals
        Assert.All(resultList.Where(r => r.Id % 2 == 0), 
            r => Assert.Equal("Deposit", r.TransactionDescription));
        Assert.All(resultList.Where(r => r.Id % 2 == 1), 
            r => Assert.Equal("Withdrawal", r.TransactionDescription));
    }
}
