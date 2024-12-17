using Lab6HashTable;
using System;
using System.Collections.Generic;

public class Program
{
    private static HashTableTester tester = new HashTableTester();
    private static IHashTable<string, string> currentHashTable;

    private static Dictionary<Type, List<Func<string, int>>> hashFunctionsByTable = new Dictionary<Type, List<Func<string, int>>>
    {
        {
            typeof(ChainedHashTable<string, string>),
            new List<Func<string, int>>
            {
                HashFunctions.DefaultHashFunction,
                HashFunctions.MultiplicativeHashFunction,
                HashFunctions.DivisionHashFunction,
                HashFunctions.Adler32HashFunction
            }
        },
        {
            typeof(OpenAddressingHashTable<string, string>),
            new List<Func<string, int>>
            {
                HashFunctions.DefaultHashFunction,
                HashFunctions.MultiplicativeHashFunction,
                HashFunctions.PolynomialHashFunction,
                HashFunctions.FirstLastHashFunction
            }
        }
    };

    private static Dictionary<string, Func<int, int, int, int>> probingStrategies = new Dictionary<string, Func<int, int, int, int>>
    {
        { "Quadratic Probing", HashFunctions.QuadraticProbing }
    };

    public static void Main(string[] args)
    {
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            int selectedOption = GetSelectedOption(new string[] { "Chained Hash Table", "Open Addressing Hash Table", "Run Tests", "Exit" });

            switch (selectedOption)
            {
                case 0:
                    currentHashTable = CreateChainedHashTable();
                    HashTableMenu();
                    break;
                case 1:
                    currentHashTable = CreateOpenAddressingHashTable();
                    HashTableMenu();
                    break;
                case 2:
                    RunTests();
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    static IHashTable<string, string> CreateChainedHashTable()
    {
        Console.Clear();
        int hashFunctionIndex = GetSelectedOption(hashFunctionsByTable[typeof(ChainedHashTable<string, string>)].Select(f => f.Method.Name).ToArray());
        var selectedHashFunction = hashFunctionsByTable[typeof(ChainedHashTable<string, string>)][hashFunctionIndex];

        Console.WriteLine($"Creating Chained Hash Table with {selectedHashFunction.Method.Name}...");
        return new ChainedHashTable<string, string>(1000, selectedHashFunction);
    }

    static IHashTable<string, string> CreateOpenAddressingHashTable()
    {
        Console.Clear();
        int hashFunctionIndex = GetSelectedOption(hashFunctionsByTable[typeof(OpenAddressingHashTable<string, string>)].Select(f => f.Method.Name).ToArray());
        var selectedHashFunction = hashFunctionsByTable[typeof(OpenAddressingHashTable<string, string>)][hashFunctionIndex];

        Console.WriteLine("\nSelect probing strategy:");
        var probingStrategyNames = probingStrategies.Keys.ToArray();
        int probingIndex = GetSelectedOption(probingStrategyNames);
        var selectedProbingStrategy = probingStrategies[probingStrategyNames[probingIndex]];

        Console.WriteLine($"\nCreating Open Addressing Hash Table with {selectedHashFunction.Method.Name} and {probingStrategyNames[probingIndex]}...");
        return new OpenAddressingHashTable<string, string>(10000, selectedHashFunction, selectedProbingStrategy);
    }

    static void HashTableMenu()
    {
        while (true)
        {
            Console.Clear();
            int selectedOption = GetSelectedOption(new string[] { "Add", "Get", "Remove", "Back to Main Menu" });

            switch (selectedOption)
            {
                case 0:
                    AddElement();
                    break;
                case 1:
                    GetElement();
                    break;
                case 2:
                    RemoveElement();
                    break;
                case 3:
                    return;
            }
        }
    }

    static void RunTests()
    {
        Console.Clear();
        Console.WriteLine("=== Testing Open Addressing Hash Table with Probing Strategies ===");

        foreach (var hashFunction in hashFunctionsByTable[typeof(OpenAddressingHashTable<string, string>)])
        {
            foreach (var probingStrategy in probingStrategies)
            {
                Console.WriteLine($"\nHash Function: {hashFunction.Method.Name}, Probing Strategy: {probingStrategy.Key}");

                var hashTable = new OpenAddressingHashTable<string, string>(10000, hashFunction, probingStrategy.Value);

                var keyValuePairs = GenerateKeyValuePairs(10000);
                foreach (var pair in keyValuePairs)
                {
                    try
                    {
                        hashTable.Add(pair.Key, pair.Value);
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Table overflow occurred.");
                        break;
                    }
                }

                var clusterLengths = hashTable.GetClusterLengths();
                int maxClusterLength = clusterLengths.Max();

                Console.WriteLine($"Maximum Cluster Length: {maxClusterLength}");
            }
        }

        Console.WriteLine("\nTesting complete. Press any key to return...");
        Console.ReadKey();
    }

    static void AddElement()
    {
        Console.Clear();
        Console.Write("Enter key: ");
        string key = Console.ReadLine();
        Console.Write("Enter value: ");
        string value = Console.ReadLine();

        currentHashTable.Add(key, value);

        Console.WriteLine("Element added. Press any key to continue...");
        Console.ReadKey();
    }

    static void GetElement()
    {
        Console.Clear();
        Console.Write("Enter key: ");
        string key = Console.ReadLine();

        try
        {
            string value = currentHashTable.Get(key);
            Console.WriteLine($"Value: {value}");
        }
        catch (KeyNotFoundException)
        {
            Console.WriteLine("Key not found.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static void RemoveElement()
    {
        Console.Clear();
        Console.Write("Enter key: ");
        string key = Console.ReadLine();

        if (currentHashTable.Remove(key))
        {
            Console.WriteLine("Element removed.");
        }
        else
        {
            Console.WriteLine("Key not found.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    static int GetSelectedOption(string[] options)
    {
        int currentSelection = 0;

        while (true)
        {
            Console.Clear();

            for (int i = 0; i < options.Length; i++)
            {
                if (i == currentSelection)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine(options[i]);
                Console.ResetColor();
            }

            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    currentSelection = (currentSelection - 1 + options.Length) % options.Length;
                    break;
                case ConsoleKey.DownArrow:
                    currentSelection = (currentSelection + 1) % options.Length;
                    break;
                case ConsoleKey.Enter:
                    Console.ResetColor();
                    return currentSelection;
            }
        }
    }

    static List<KeyValuePair<string, string>> GenerateKeyValuePairs(int count)
    {
        var random = new Random();
        var uniqueKeys = new HashSet<string>();
        var pairs = new List<KeyValuePair<string, string>>();

        while (uniqueKeys.Count < count)
        {
            var key = Guid.NewGuid().ToString("N").Substring(0, 8);

            if (uniqueKeys.Add(key))
            {
                var value = $"Value_{random.Next(1, 100000)}";
                pairs.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        return pairs;
    }
}
