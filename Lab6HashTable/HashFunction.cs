using System;
using System.Linq;

namespace Lab6HashTable
{
    public static class HashFunctions
    {
        // Размер таблицы задается извне
        public static int TableSize { get; set; } = 1009;

        // Хеш-функция методом деления
        public static int DivisionHashFunction(string key)
        {
            int intKey = key.Aggregate(0, (hash, ch) => hash + ch);
            return Math.Abs(intKey % TableSize);
        }

        // Хеш-функция методом умножения
        public static int MultiplicativeHashFunction(string key)
        {
            const double A = 0.6180339887; // Константа A (золотое сечение)
            int intKey = key.Aggregate(0, (hash, ch) => hash + ch); // Преобразование строки в число (сумма символов)
            double fractionalPart = (intKey * A) % 1; // Дробная часть от умножения
            return (int)(TableSize * fractionalPart); // Хеш по методу умножения
        }

        // Adler-32 hash function
        public static int Adler32HashFunction(string key)
        {
            unchecked
            {
                uint a = 1, b = 0;
                foreach (char c in key)
                {
                    a = (a + c) % 65521;
                    b = (b + a) % 65521;
                }
                return (int)((b << 16) | a) % TableSize; // Ensure within table size
            }
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

        public static int QuadraticProbing(int baseHash, int attempt, int tableSize)
        {
            const int c1 = 1; // Константа для линейного компонента
            const int c2 = 3; // Константа для квадратичного компонента

            int index = (baseHash + c1 * attempt + c2 * attempt * attempt) % tableSize;
            return index >= 0 ? index : index + tableSize; // Гарантируем неотрицательный индекс
        }


    }
}
