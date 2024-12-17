using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6HashTable
{
    public class OpenAddressingHashTable<K, V> : IHashTable<K, V>
    {
        private const double LoadFactor = 0.7;
        private readonly Func<K, int> hashFunction;
        private readonly Func<int, int, int, int> probingFunction; // Стратегия пробирования

        private Entry[] table;
        private int count;
        private int capacity;

        public OpenAddressingHashTable(int initialCapacity, Func<K, int> hashFunction, Func<int, int, int, int> probingFunction)
        {
            this.hashFunction = hashFunction;
            this.probingFunction = probingFunction;
            capacity = initialCapacity;
            table = new Entry[capacity];
            count = 0;
        }

        private int GetBucketIndex(K key, int probe)
        {
            int baseHash = hashFunction(key);
            return probingFunction(baseHash, probe, capacity); // Используем стратегию пробирования
        }

        public void Add(K key, V value)
        {
            if (count >= capacity * LoadFactor)
            {
                Resize();
            }

            int probe = 0;
            int index;

            while (true)
            {
                index = GetBucketIndex(key, probe);

                if (table[index] == null || table[index].IsDeleted)
                {
                    table[index] = new Entry(key, value);
                    count++;
                    return;
                }

                probe++;
            }
        }

        public V Get(K key)
        {
            int probe = 0;
            int index;

            while (probe < capacity)
            {
                index = GetBucketIndex(key, probe);

                if (table[index] == null)
                {
                    break;
                }

                if (!table[index].IsDeleted && table[index].Key.Equals(key))
                {
                    return table[index].Value;
                }

                probe++;
            }

            throw new KeyNotFoundException($"Key '{key}' not found.");
        }

        public bool Remove(K key)
        {
            int probe = 0;
            int index;

            while (probe < capacity)
            {
                index = GetBucketIndex(key, probe);

                if (table[index] == null)
                {
                    break;
                }

                if (!table[index].IsDeleted && table[index].Key.Equals(key))
                {
                    table[index].IsDeleted = true;
                    count--;
                    return true;
                }

                probe++;
            }

            return false;
        }

        public double LoadFactorValue => (double)count / capacity;

        public List<int> GetChainLengths()
        {
            return table.Select(bucket => bucket != null && !bucket.IsDeleted ? 1 : 0).ToList();
        }

        public List<int> GetClusterLengths()
        {
            var clusters = new List<int>();
            int currentCluster = 0;

            foreach (var bucket in table)
            {
                if (bucket != null && !bucket.IsDeleted)
                {
                    currentCluster++;
                }
                else
                {
                    if (currentCluster > 0)
                    {
                        clusters.Add(currentCluster);
                        currentCluster = 0;
                    }
                }
            }

            // Добавляем последний кластер, если он не был добавлен
            if (currentCluster > 0)
            {
                clusters.Add(currentCluster);
            }

            return clusters;
        }



        private void Resize()
        {
            var oldTable = table;
            capacity *= 2;
            table = new Entry[capacity];
            count = 0;

            foreach (var entry in oldTable)
            {
                if (entry != null && !entry.IsDeleted)
                {
                    Add(entry.Key, entry.Value);
                }
            }
        }

        private class Entry
        {
            public K Key { get; }
            public V Value { get; }
            public bool IsDeleted { get; set; }

            public Entry(K key, V value)
            {
                Key = key;
                Value = value;
                IsDeleted = false;
            }
        }
    }
}
