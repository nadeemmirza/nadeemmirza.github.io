using System.Diagnostics;
using TransactionProcessor.Models;
using TransactionProcessor.Services;

Console.WriteLine("=== Transaction Processing with Database Lookups ===");
Console.WriteLine("Demonstrating the BEST PRACTICE for C# .NET 8\n");

// Create sample transactions
var transactions = GenerateSampleTransactions(100);
Console.WriteLine($"Processing {transactions.Count} transactions...\n");

// Create repository
var repository = new MockTransactionTypeRepository();

// ===========================
// EFFICIENT APPROACH (BEST PRACTICE)
// ===========================
Console.WriteLine("--- EFFICIENT APPROACH (Recommended) ---");
var efficientProcessor = new EfficientTransactionProcessor(repository);
var sw1 = Stopwatch.StartNew();
var efficientResults = await efficientProcessor.ProcessTransactionsAsync(transactions);
sw1.Stop();

Console.WriteLine($"✓ Processed {efficientResults.Count()} transactions");
Console.WriteLine($"✓ Time taken: {sw1.ElapsedMilliseconds}ms");
Console.WriteLine($"✓ Database calls: 1 (loaded all types once)");
Console.WriteLine($"✓ Memory usage: O(n) where n is number of transaction types");

// Show some results
Console.WriteLine("\nSample Results:");
foreach (var result in efficientResults.Take(5))
{
    Console.WriteLine($"  {result.TransactionDate:yyyy-MM-dd} | {result.TransactionType} - {result.TransactionDescription} | ${result.Amount:N2}");
}

// ===========================
// INEFFICIENT APPROACH (N+1 Problem)
// ===========================
Console.WriteLine("\n--- INEFFICIENT APPROACH (Not Recommended) ---");
var inefficientProcessor = new InefficientTransactionProcessor(repository);
var sw2 = Stopwatch.StartNew();
var inefficientResults = await inefficientProcessor.ProcessTransactionsAsync(transactions);
sw2.Stop();

Console.WriteLine($"✗ Processed {inefficientResults.Count()} transactions");
Console.WriteLine($"✗ Time taken: {sw2.ElapsedMilliseconds}ms");
Console.WriteLine($"✗ Database calls: {transactions.Count} (one per transaction - N+1 problem!)");
Console.WriteLine($"✗ This approach scales poorly with more transactions");

// ===========================
// CONCLUSION
// ===========================
Console.WriteLine("\n=== CONCLUSION ===");
Console.WriteLine("The efficient approach using dictionary caching is:");
Console.WriteLine($"- {sw2.ElapsedMilliseconds / (double)sw1.ElapsedMilliseconds:F2}x faster");
Console.WriteLine($"- Makes {transactions.Count - 1} fewer database calls");
Console.WriteLine("- Scales linearly with transaction count");
Console.WriteLine("- Recommended for production use");

Console.WriteLine("\n=== KEY TAKEAWAYS ===");
Console.WriteLine("1. Load all transaction types ONCE at the beginning");
Console.WriteLine("2. Store them in a Dictionary<string, TransactionTypeInfo>");
Console.WriteLine("3. Use dictionary lookups (O(1)) instead of database queries");
Console.WriteLine("4. This avoids the N+1 query problem");
Console.WriteLine("5. Performance scales linearly, not exponentially");

static List<Transaction> GenerateSampleTransactions(int count)
{
    var random = new Random(42); // Fixed seed for reproducibility
    var types = new[] { "DEP", "WTH", "TRF", "FEE", "INT", "CHG", "REF", "PAY" };
    var transactions = new List<Transaction>();

    for (int i = 1; i <= count; i++)
    {
        transactions.Add(new Transaction
        {
            Id = i,
            TransactionType = types[random.Next(types.Length)],
            TransactionDate = DateTime.Now.AddDays(-random.Next(365)),
            Amount = (decimal)(random.NextDouble() * 1000),
            Reference = $"REF{i:D6}"
        });
    }

    return transactions;
}
