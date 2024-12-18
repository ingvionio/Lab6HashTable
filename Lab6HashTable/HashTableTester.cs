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
        {
            typeof(ChainedHashTable<string, string>),
            new List<Func<string, int>>
            {
                HashFunctions.DefaultHashFunction,
                HashFunctions.MultiplicativeHashFunction,
                HashFunctions.DivisionHashFunction,
                HashFunctions.Adler32HashFunction,
                HashFunctions.XorHashFunction,
                HashFunctions.ShiftXorMultiplyHashFunction
            }
        },
        {
            typeof(OpenAddressingHashTable<string, string>),
            new List<Func<string, int>>
            {
                HashFunctions.DefaultHashFunction,
                HashFunctions.MultiplicativeHashFunction,
                //HashFunctions.DivisionHashFunction,
                HashFunctions.Adler32HashFunction,
                HashFunctions.XorHashFunction,
                HashFunctions.ShiftXorMultiplyHashFunction
            }
        }
    };
    }


    public void TestChainedHashTable()
    {
        const int elementCount = 100000;
        const int tableSize = 1000;
        HashFunctions.TableSize = 1000;

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
        HashFunctions.TableSize = 10000;

        var probingStrategies = new Dictionary<string, Func<int, int, int, int>>
        {
            { "LinearProbing", HashFunctions.LinearProbing },
            { "QuadraticProbing", HashFunctions.QuadraticProbing },
            { "DoubleHashing", HashFunctions.DoubleHashing },
            { "ExponentialProbing", HashFunctions.ExponentialProbing },
            { "CubicProbing", HashFunctions.CubicProbing}
        };



        var results = new List<(string HashFunction, string ProbingStrategy, int MaxCluster)>();
        var keyValuePairs = GenerateKeyValuePairs(elementCount);

        Console.WriteLine("=== Тестирование OpenAddressingHashTable ===");

        foreach (var hashFunction in hashFunctionsByTable[typeof(OpenAddressingHashTable<string, string>)])
        {
            foreach (var probingStrategy in probingStrategies)
            {
                Console.WriteLine($"\nХеш-функция: {hashFunction.Method.Name}, Стратегия пробирования: {probingStrategy.Key}");

                var hashTable = new OpenAddressingHashTable<string, string>(
                    tableSize,
                    hashFunction,
                    probingStrategy.Value
                );

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

                var clusterLengths = hashTable.GetClusterLengths();
                int maxClusterLength = CalculateMaxCluster(clusterLengths);
                results.Add((hashFunction.Method.Name, probingStrategy.Key, maxClusterLength));

                Console.WriteLine($"Самый длинный кластер: {maxClusterLength}");
            }
        }

        var bestResult = results.OrderBy(r => r.MaxCluster).First();
        Console.WriteLine($"\nЛучшая комбинация:");
        Console.WriteLine($"Хеш-функция: {bestResult.HashFunction}");
        Console.WriteLine($"Стратегия пробирования: {bestResult.ProbingStrategy}");
        Console.WriteLine($"Самый длинный кластер: {bestResult.MaxCluster}");
    }






    private int CalculateMaxCluster(List<int> clusterLengths)
    {
        return clusterLengths.Max(); // Просто находим максимальный кластер
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
