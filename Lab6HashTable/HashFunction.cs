using System;
using System.Linq;

namespace Lab6HashTable
{
    public static class HashFunctions
    {
        // Размер таблицы задается извне
        public static int TableSize { get; set; } = 1009;

        // Хеш-функция методом умножения
        public static int MultiplicativeHashFunction(string key)
        {
            const double A = 0.6180339887; // Константа A (золотое сечение)
            int intKey = key.Aggregate(0, (hash, ch) => hash + ch); // Преобразование строки в число (сумма символов)
            double fractionalPart = (intKey * A) % 1; // Дробная часть от умножения
            return (int)(TableSize * fractionalPart); // Хеш по методу умножения
        }

        // Базовая хеш-функция, использующая стандартный GetHashCode
        public static int DefaultHashFunction(string key)
        {
            return Math.Abs(key.GetHashCode() % TableSize);
        }

        // Простейшая хеш-функция на основе длины строки
        public static int LengthBasedHashFunction(string key)
        {
            return key.Length % TableSize;
        }

        // Полиномиальная хеш-функция
        public static int PolynomialHashFunction(string key)
        {
            const int P = 31; // Константа
            int hash = 0;
            foreach (char ch in key)
            {
                hash = (hash * P + ch) % TableSize;
            }
            return Math.Abs(hash);
        }

        // Хеш-функция на основе первых и последних символов строки
        public static int FirstLastHashFunction(string key)
        {
            if (string.IsNullOrEmpty(key))
                return 0;

            int first = key[0];
            int last = key[key.Length - 1];
            return Math.Abs((first * 31 + last) % TableSize);
        }
    }
}
