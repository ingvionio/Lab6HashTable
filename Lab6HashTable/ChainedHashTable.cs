using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6HashTable
{
    public class ChainedHashTable<K, V> : IHashTable<K, V>
    {
        private readonly LinkedList<KeyValuePair<K, V>>[] buckets;
        private readonly Func<K, int> hashFunction;

        public ChainedHashTable(int capacity, Func<K, int> hashFunction)
        {
            buckets = new LinkedList<KeyValuePair<K, V>>[capacity];
            this.hashFunction = hashFunction;
        }

        private int GetBucketIndex(K key)
        {
            int hash = hashFunction(key);
            return Math.Abs(hash % buckets.Length);
        }

        public void Add(K key, V value)
        {
            int index = GetBucketIndex(key);

            if (buckets[index] == null)
            {
                buckets[index] = new LinkedList<KeyValuePair<K, V>>();
            }

            buckets[index].AddLast(new KeyValuePair<K, V>(key, value));
        }

        public V Get(K key)
        {
            int index = GetBucketIndex(key);

            if (buckets[index] != null)
            {
                foreach (var pair in buckets[index])
                {
                    if (EqualityComparer<K>.Default.Equals(pair.Key, key))
                    {
                        return pair.Value;
                    }
                }
            }

            throw new KeyNotFoundException($"Key '{key}' not found.");
        }

        public bool Remove(K key)
        {
            int index = GetBucketIndex(key);

            if (buckets[index] != null)
            {
                foreach (var pair in buckets[index])
                {
                    if (EqualityComparer<K>.Default.Equals(pair.Key, key))
                    {
                        buckets[index].Remove(pair);
                        return true;
                    }
                }
            }

            return false;
        }

        public double LoadFactorValue => buckets.Count(b => b != null && b.Any()) / (double)buckets.Length;

        public List<int> GetChainLengths()
        {
            return buckets.Select(bucket => bucket?.Count ?? 0).ToList();
        }
    }
}
