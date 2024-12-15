using System.Collections.Generic;

namespace Lab6HashTable
{
    public interface IHashTable<K, V>
    {
        void Add(K key, V value); // Добавление элемента
        V Get(K key); // Поиск значения по ключу, выбрасывает исключение, если ключ не найден
        bool Remove(K key); // Удаление элемента
        double LoadFactorValue { get; } // Коэффициент загрузки таблицы
        List<int> GetChainLengths(); // Анализ длины цепочек или распределения ячеек
    }
}
