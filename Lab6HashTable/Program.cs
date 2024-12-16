using Lab6HashTable;
using System;
using System.Collections.Generic;

public class Program
{
    private static HashTableTester tester = new HashTableTester();
    private static IHashTable<string, string> currentHashTable;

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
        // Здесь можно добавить выбор хеш-функции для ChainedHashTable
        Console.WriteLine("Creating Chained Hash Table... (Default hash function used)");
        return new ChainedHashTable<string, string>(1000, HashFunctions.DefaultHashFunction); // Используем DefaultHashFunction для примера
    }

    static IHashTable<string, string> CreateOpenAddressingHashTable()
    {
        Console.WriteLine("Creating Open Addressing Hash Table...");
        return new OpenAddressingHashTable<string, string>(10000);
    }

    static void HashTableMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Choose an operation:");
            //DisplayHashTableOperationsMenu();
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
                    return; // Возврат в главное меню
            }
        }
    }


    static void DisplayHashTableOperationsMenu()
    {
        Console.WriteLine("Choose an operation:");
        Console.WriteLine("1. Add element");
        Console.WriteLine("2. Get element");
        Console.WriteLine("3. Remove element");
        Console.WriteLine("4. Back to Main Menu");

    }

    static void RunTests()
    {
        while (true)
        {
            Console.Clear();
            int testOption = GetSelectedOption(new string[] { "Test Chained Hash Table", "Test Open Addressing Hash Table", "Back to Main Menu" });

            switch (testOption)
            {
                case 0:
                    Console.Clear();
                    Console.WriteLine("Running Chained Hash Table test...");
                    tester.TestChainedHashTable();
                    Console.WriteLine("Test complete. Press any key to continue...");
                    Console.ReadKey();
                    break;
                case 1:
                    Console.Clear();
                    Console.WriteLine("Running Open Addressing Hash Table test...");
                    tester.TestOpenAddressingHashTable();
                    Console.WriteLine("Test complete. Press any key to continue...");
                    Console.ReadKey();
                    break;
                case 2:
                    return; 
            }
        }
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
            Console.Clear(); // Clear *before* drawing the menu

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
                    Console.ResetColor(); // Ensure colors are reset before returning
                    return currentSelection;
            }
        }
    }
}