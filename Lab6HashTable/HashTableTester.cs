using System;
using System.Collections.Generic;
using System.Linq;
using Lab6HashTable;

public class HashTableTester
{
    private readonly Dictionary<Type, List<Func<string, int>>> hashFunctionsByTable;

    public HashTableTester()
    {
        hashFunctionsByTable = new Dictionary<Type, List<Func<string, int>>>
        {
            // Хеш-функции для ChainedHashTable
            {
                typeof(ChainedHashTable<string, string>),
                new List<Func<string, int>>
                {
                    HashFunctions.DefaultHashFunction,
                    HashFunctions.MultiplicativeHashFunction,
                    HashFunctions.PolynomialHashFunction,
                    HashFunctions.FirstLastHashFunction,
                    HashFunctions.DivisionHashFunction,
                    HashFunctions.Adler32HashFunction
                }
            },
            // Хеш-функции для OpenAddressingHashTable
            {
                typeof(OpenAddressingHashTable<string, string>),
                new List<Func<string, int>>
                {
                    HashFunctions.DefaultHashFunction,
                    HashFunctions.MultiplicativeHashFunction,
                    HashFunctions.LengthBasedHashFunction,
                    HashFunctions.PolynomialHashFunction,
                    HashFunctions.FirstLastHashFunction
                }
            }
        };
    }

    public void TestChainedHashTable()
    {
        const int elementCount = 100000;
        const int tableSize = 1000;

        // Генерация пар "ключ-значение"
        var keyValuePairs = GenerateKeyValuePairs(elementCount);
        var results = new List<(string FunctionName, double FillFactor, int MaxChain, int MinChain)>();

        Console.WriteLine("=== Тестирование ChainedHashTable ===");

        foreach (var hashFunction in hashFunctionsByTable[typeof(ChainedHashTable<string, string>)])
        {
            Console.WriteLine($"\nТестирование хеш-функции: {hashFunction.Method.Name}");

            var hashTable = new ChainedHashTable<string, string>(tableSize, hashFunction);

            // Вставляем пары в таблицу
            foreach (var pair in keyValuePairs)
            {
                hashTable.Add(pair.Key, pair.Value);
            }

            var chainLengths = hashTable.GetChainLengths();

            // Количество заполненных ячеек
            int filledBuckets = chainLengths.Count(x => x > 0);

            // Коэффициент заполнения как процент
            double fillFactor = (double)filledBuckets / tableSize * 100;

            // Длина самой длинной и самой короткой цепочек
            int maxChainLength = chainLengths.Max();
            int minChainLength = chainLengths.Where(x => x > 0).DefaultIfEmpty(0).Min();

            results.Add((hashFunction.Method.Name, fillFactor, maxChainLength, minChainLength));

            Console.WriteLine($"Заполненные ячейки: {filledBuckets}/{tableSize}");
            Console.WriteLine($"Коэффициент заполнения: {fillFactor:F2}%");
            Console.WriteLine($"Самая длинная цепочка: {maxChainLength}");
            Console.WriteLine($"Самая короткая цепочка: {minChainLength}");
        }

        var bestFunction = results.OrderBy(r => r.MaxChain).ThenBy(r => r.FillFactor).First();
        Console.WriteLine($"\nЛучшая хеш-функция: {bestFunction.FunctionName}");
        Console.WriteLine($"Обоснование: минимальная длина самой длинной цепочки ({bestFunction.MaxChain}) и хороший коэффициент заполнения ({bestFunction.FillFactor:F2}%).");
    }


    public void TestOpenAddressingHashTable()
    {
        const int elementCount = 10000;
        const int tableSize = 10000;

        // Генерация пар "ключ-значение"
        var keyValuePairs = GenerateKeyValuePairs(elementCount);
        var results = new List<(string FunctionName, int MaxCluster)>();

        Console.WriteLine("=== Тестирование OpenAddressingHashTable ===");

        foreach (var hashFunction in hashFunctionsByTable[typeof(OpenAddressingHashTable<string, string>)])
        {
            Console.WriteLine($"\nТестирование хеш-функции: {hashFunction.Method.Name}");

            var hashTable = new OpenAddressingHashTable<string, string>(tableSize);

            // Вставляем пары в таблицу
            foreach (var pair in keyValuePairs)
            {
                try
                {
                    hashTable.Add(pair.Key, pair.Value);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine($"Ошибка: {e.Message} - Таблица переполнена.");
                    break;
                }
            }

            var chainLengths = hashTable.GetChainLengths();
            int maxClusterLength = CalculateMaxCluster(chainLengths);
            results.Add((hashFunction.Method.Name, maxClusterLength));

            Console.WriteLine($"Самый длинный кластер: {maxClusterLength}");
        }

        var bestFunction = results.OrderBy(r => r.MaxCluster).First();
        Console.WriteLine($"\nЛучшая хеш-функция: {bestFunction.FunctionName}");
        Console.WriteLine($"Обоснование: минимальная длина самого длинного кластера ({bestFunction.MaxCluster}).");
    }


    private int CalculateMaxCluster(List<int> chainLengths)
    {
        int maxCluster = 0;
        int currentCluster = 0;

        foreach (var length in chainLengths)
        {
            if (length > 0)
            {
                currentCluster++;
                maxCluster = Math.Max(maxCluster, currentCluster);
            }
            else
            {
                currentCluster = 0;
            }
        }

        return maxCluster;
    }

    private List<KeyValuePair<string, string>> GenerateKeyValuePairs(int count)
    {
        var random = new Random();
        var uniqueKeys = new HashSet<string>();
        var pairs = new List<KeyValuePair<string, string>>();

        while (uniqueKeys.Count < count)
        {
            // Генерация уникального ключа
            var key = Guid.NewGuid().ToString("N").Substring(0, 8);

            // Проверяем уникальность ключа
            if (uniqueKeys.Add(key)) // HashSet.Add вернет true, если ключ уникален
            {
                // Генерация значения для ключа
                var value = $"Value_{random.Next(1, 100000)}";
                pairs.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        return pairs;
    }


}
