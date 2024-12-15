using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6HashTable
{
    public class OpenAddressingHashTable<K, V> : IHashTable<K, V>
    {
        private const double LoadFactor = 0.75;

        private Entry[] table;
        private int count;
        private int capacity;

        public OpenAddressingHashTable(int initialCapacity = 16)
        {
            capacity = initialCapacity;
            table = new Entry[capacity];
            count = 0;
        }

        private int GetBucketIndex(K key, int probe)
        {
            int hash = key.GetHashCode();
            return Math.Abs((hash + probe) % capacity);
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
