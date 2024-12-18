using System;
using System.Linq;

namespace Lab6HashTable
{
    public static class HashFunctions
    {
        // Размер таблицы задается извне
        public static int TableSize { get; set; }

        // Хеш-функция методом деления
        public static int DivisionHashFunction(string key)
        {
            int intKey = 0;
            foreach (char ch in key)
            {
                intKey *= 128;
                intKey += ch; // Добавляем ASCII-код символа
            }
            return Math.Abs(intKey % TableSize);
        }

        // Хеш-функция методом умножения
        public static int MultiplicativeHashFunction(string key)
        {
            const double A = 0.6180339887; // Константа A (золотое сечение)
            //int intKey = key.Aggregate(0, (hash, ch) => hash * 31 + ch); // Усиливаем сумму символов
            int intKey = 0;
            foreach (char ch in key)
            {
                intKey *= 128;
                intKey += ch; // Добавляем ASCII-код символа
            }

            double fractionalPart = (intKey * A) % 1; // Дробная часть от умножения
            int result = (int)(TableSize * fractionalPart);

            return result == 0 ? 1 : result; // Гарантируем, что результат != 0
        }


        public static int XorHashFunction(string key)
        {
            int hash = 0;

            foreach (char ch in key)
            {
                hash ^= ch; // Побитовое XOR
                hash *= 31; // Дополнительное перемешивание
            }

            return Math.Abs(hash % TableSize);
        }

        public static int ShiftXorMultiplyHashFunction(string key)
        {
            int hash = 0;

            foreach (char ch in key)
            {
                hash ^= (hash << 5) + (hash >> 2) + ch; // Побитовый сдвиг и XOR с символом
                hash *= 16777619; // Множитель Фибоначчи для усиления
            }

            return Math.Abs(hash % TableSize); // Гарантируем, что хеш в пределах TableSize
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

       

        public static int QuadraticProbing(int baseHash, int attempt, int tableSize)
        {
            const int c1 = 1; // Константа для линейного компонента
            const int c2 = 3; // Константа для квадратичного компонента

            int index = (baseHash + c1 * attempt + c2 * attempt * attempt) % tableSize;
            return index >= 0 ? index : index + tableSize; // Гарантируем неотрицательный индекс
        }

        public static int LinearProbing(int baseHash, int attempt, int tableSize)
        {
            // Линейное пробирование: просто добавляем номер попытки (i) к базовому хешу
            int index = (baseHash + attempt) % tableSize;

            // Гарантируем неотрицательный индекс
            return index >= 0 ? index : index + tableSize;
        }

        public static int DoubleHashing(int baseHash, int attempt, int tableSize)
        {
            int step = 1 + (baseHash % (tableSize - 1));
            int index = (baseHash + attempt * step) % tableSize;
            return index >= 0 ? index : index + tableSize;
        }

        public static int ExponentialProbing(int baseHash, int attempt, int tableSize)
        {
            int step = (int)Math.Pow(2, attempt); // 2^i
            int index = (baseHash + step) % tableSize;

            return index >= 0 ? index : index + tableSize; // Гарантия положительного индекса
        }

        public static int CubicProbing(int baseHash, int attempt, int tableSize)
        {
            int c1 = 1;
            int c2 = 1;

            int step = c1 * attempt + c2 * (attempt * attempt * attempt); // c1 * i + c2 * i^3
            int index = (baseHash + step) % tableSize;

            return index >= 0 ? index : index + tableSize; // Гарантия положительного индекса
        }

    }
}
