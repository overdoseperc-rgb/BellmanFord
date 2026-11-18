using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    // Вершины: A = 0, B = 1, C = 2, D = 3, E = 4.
    // Строка массива: откуда, куда, вес.
    static int[,] edges = {
        {0, 1, 4}, {0, 2, 5}, {1, 2, -2},
        {1, 3, 6}, {2, 3, 3}, {3, 4, 2}, {2, 4, 7}
    };
    static bool[] active = {true, true, true, true, true, true, true};

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\nРёбра графа (движение только по стрелке):");
            for (int i = 0; i < active.Length; i++)
                Console.WriteLine($"{i + 1}: {(char)('A' + edges[i, 0])} -> " +
                    $"{(char)('A' + edges[i, 1])}, вес {edges[i, 2]} " +
                    (active[i] ? "[есть]" : "[разорвано]"));

            Console.WriteLine("1 — найти путь; 2 — разорвать/вернуть ребро; 0 — выход");
            string command = Console.ReadLine();
            if (command == null || command == "0") return;
            if (command == "1")
            {
                Console.Write("Начальная вершина (A–E, латиница): ");
                string from = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                Console.Write("Конечная вершина (A–E, латиница): ");
                string to = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                if (from.Length != 1 || to.Length != 1 ||
                    from[0] < 'A' || from[0] > 'E' || to[0] < 'A' || to[0] > 'E')
                    Console.WriteLine("Введите латинскую букву от A до E.");
                else
                    BellmanFord(from[0] - 'A', to[0] - 'A');
            }
            else if (command == "2")
            {
                Console.Write("Номер ребра (1–7): ");
                if (int.TryParse(Console.ReadLine(), out int number) &&
                    number >= 1 && number <= active.Length)
                    active[number - 1] = !active[number - 1];
                else
                    Console.WriteLine("Неверный номер.");
            }
            else Console.WriteLine("Неизвестная команда.");
        }
    }

    static void BellmanFord(int start, int finish)
    {
        const int n = 5;
        long[] distance = new long[n];
        int[] previous = new int[n];
        for (int i = 0; i < n; i++)
        {
            distance[i] = long.MaxValue;
            previous[i] = -1;
        }
        distance[start] = 0;

        // До V - 1 проходов по всем рёбрам.
        for (int pass = 0; pass < n - 1; pass++)
        {
            bool changed = false;
            for (int i = 0; i < active.Length; i++)
            {
                int a = edges[i, 0], b = edges[i, 1], weight = edges[i, 2];
                if (active[i] && distance[a] != long.MaxValue &&
                    distance[a] + weight < distance[b])
                {
                    distance[b] = distance[a] + weight;
                    previous[b] = a;
                    changed = true;
                }
            }
            if (!changed) break;
        }

        // Дополнительный проход: проверка отрицательного цикла.
        for (int i = 0; i < active.Length; i++)
        {
            int a = edges[i, 0], b = edges[i, 1], weight = edges[i, 2];
            if (active[i] && distance[a] != long.MaxValue &&
                distance[a] + weight < distance[b])
            {
                Console.WriteLine("Обнаружен отрицательный цикл, достижимый из старта.");
                return;
            }
        }

        if (distance[finish] == long.MaxValue)
        {
            Console.WriteLine("Пути нет.");
            return;
        }
        List<char> path = new List<char>();
        for (int v = finish; v != -1; v = previous[v])
            path.Add((char)('A' + v));
        path.Reverse();
        Console.WriteLine("Путь: " + string.Join(" -> ", path));
        Console.WriteLine("Суммарный вес: " + distance[finish]);
    }
}
