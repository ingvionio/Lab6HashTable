public class Program
{
    public static void Main(string[] args)
    {
        var tester = new HashTableTester();

        Console.WriteLine("=== Testing ChainedHashTable ===");
        tester.TestChainedHashTable();

        Console.WriteLine("=== Testing OpenAddressingHashTable ===");
        tester.TestOpenAddressingHashTable();
    }
}
