using SquareSudoku;
using System.Collections.Concurrent;
using System.Diagnostics;


var executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
//var min = 200000000;
//var max = 2000000000;
var answer = 12345679;

var allDigits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
var mustHaveDigits = new List<int> { 0, 2, 5 };
var all9Digits = Utility.GetNineDigits(allDigits, mustHaveDigits);

var sudoku = new SudokuValidator(new List<List<int>>
{
    new List<int> { -1, -1, -1, -1, -1, -1, -1, 2, -1 },
    new List<int> { -1, -1, -1, -1, 2, -1, -1, -1, 5 },
    new List<int> { -1, 2, -1, -1, -1, -1, -1, -1, -1 },
    new List<int> { -1, -1, 0, -1, -1, -1, -1, -1, -1 },
    new List<int> { -1, -1, -1, -1, -1, -1, -1, -1, -1 },
    new List<int> { -1, -1, -1, 2, -1, -1, -1, -1, -1 },
    new List<int> { -1, -1, -1, -1, 0, -1, -1, -1, -1 },
    new List<int> { -1, -1, -1, -1, -1, 2, -1, -1, -1 },
    new List<int> { -1, -1, -1, -1, -1, -1, 5, -1, -1 },
});
sudoku.Print();
//Console.WriteLine(sudoku.ValidateSudoku());

var stopwatch = Stopwatch.StartNew();

var max = 329107847;
max = 32921811;
max = 32676369;
max = 31628371;
max = 30987103;
max = 28659319;
max = 26001483;
max = 12843319;
var min = 104092;
for (int potentialDivisor = max; potentialDivisor >= min; potentialDivisor -= 4)
{
    if (potentialDivisor % 5 == 0)
    {
        continue;
    }
    var allDivisors = new ConcurrentDictionary<int, Divisor>();
    Parallel.ForEach(all9Digits, (item, _) =>
    {
        var (skipDigit, nineDigits, permutations) = item;
        // Console.WriteLine($"Using 9 digits out of 10: {Utility.ToNumber(nineDigits)} (skip digit {skipDigit})");
        var subDivisors = new ConcurrentDictionary<int, Divisor>();
        foreach (var permutation in permutations)
        {
            var number = Utility.ToNumber(permutation);
            if (number % potentialDivisor != 0)
            {
                continue;
            }
            if (!sudoku.ValidateNumberInAnyRows(permutation, out var matchedRow))
            {
                continue;
            }

            // Utility.CreateTextFile($"{executableDirectory}\\result\\{divisor}\\{skipDigits}\\{matchedRow}", $"{number}.txt");
            subDivisors.AddOrUpdate(potentialDivisor, new Divisor(potentialDivisor, skipDigit, matchedRow, number), (key, existingDivisor) =>
            {
                existingDivisor.AddMatchedRow(skipDigit, matchedRow, number);
                return existingDivisor;
            });
        }

        foreach (var (d, divisor) in subDivisors)
        {
            divisor.ClearInvalidNumbers();
            if (divisor.HasEnoughMatchedRows())
            {
                allDivisors.AddOrUpdate(d, divisor, (key, existingDivisor) =>
                {
                    existingDivisor.Merge(divisor);
                    return existingDivisor;
                });
            }
        }
    });

    if (allDivisors.Count == 0)
    {
        continue;
    }
    Console.WriteLine();
    Console.WriteLine($"Potential Divisor: {potentialDivisor}.");
    Console.WriteLine($"Total divisor count: {allDivisors.Count}. Time eslapsed: {stopwatch.Elapsed.TotalMinutes} minutes.");
    Console.WriteLine($"Converting dictionary of {allDivisors.Count} divisors to priority queue");
    var priorityQueue = new PriorityQueue<Divisor, int>(allDivisors.Count);
    foreach (var (d, divisor) in allDivisors)
    {
        priorityQueue.Enqueue(divisor, -d);
    }
    
    Console.WriteLine($"Priority queue count: {priorityQueue.Count}");
    while (priorityQueue.Count > 0)
    {
        var divisor = priorityQueue.Dequeue();
        Console.WriteLine($"Checking divisor {divisor.Value} which has {divisor.SkipDigits.Count} skip digits and {divisor.CountNumber()} numbers. Time eslapsed: {stopwatch.Elapsed.TotalMinutes} minutes.");
        foreach (var kv in divisor.SkipDigits)
        {
            var sudokuGenerator = new SudokuGenerator(kv.Value, sudoku.Sudoku);
            var validSudoku = sudokuGenerator.Generate();
            if (validSudoku != null)
            {
                Console.WriteLine($"Found valid sudoku. GCD is {divisor.Value}");
                validSudoku.Print();
                validSudoku.Save(divisor.Value);
            }
        }
    }
}

