namespace SquareSudoku
{
    public class Utility
    {
        private const int MaxNunber = 987654321;

        public static List<List<int>> GenerateNumberPermutations(List<int> availableNumbers)
        {
            return GenerateNumberPermutations(availableNumbers, 0).ToList();
        }

        private static IEnumerable<List<int>> GenerateNumberPermutations(List<int> availableNumbers, int start)
        {
            if (start == availableNumbers.Count)
            {
                yield return new List<int>(availableNumbers);
                yield break;
            }

            for (int i = start; i < availableNumbers.Count; i++)
            {
                Swap(availableNumbers, start, i);
                foreach (var perm in GenerateNumberPermutations(availableNumbers, start + 1))
                {
                    yield return perm;
                }
                Swap(availableNumbers, start, i); // Backtrack
            }
        }

        public static void Swap(List<int> nums, int i, int j)
        {
            int temp = nums[i];
            nums[i] = nums[j];
            nums[j] = temp;
        }

        public static List<(int, List<int>, List<List<int>>)> GetNineDigits(List<int> allDigits, List<int> mustHaveDigits)
        {
            var result = new List<(int, List<int>, List<List<int>>)>();

            for (int skipIndex = 0; skipIndex < allDigits.Count; skipIndex++)
            {
                var r = new List<int>();
                if (mustHaveDigits.Contains(skipIndex))
                {
                    // Do not skip
                    continue;
                }
                for (int j = 0; j < allDigits.Count; j++)
                {
                    // skip i
                    if (j == skipIndex)
                    {
                        continue;
                    }
                    r.Add(allDigits[j]);
                }
                var nineDigits = r.OrderByDescending(n => n).ToList();
                result.Add((skipIndex, nineDigits, GenerateNumberPermutations(nineDigits)));
            }

            return result;
        }

        public static IEnumerable<int> GetDivisorsDescending(int number, int min = 1, int max = int.MaxValue)
        {
            // var squareRoot = (int)Math.Ceiling(Math.Sqrt(number));
            max = Math.Min(max, number);
            for (int i = max; i >= min; i--)
            {
                if (i % 2 == 0 || i % 5 == 0)
                {
                    continue;
                }
                if (number % i == 0) // Check if i is a divisor
                {
                    yield return i; // Yield each divisor
                }
            }
        }

        public static int ToNumber(List<int> x)
        {
            var result = 0;
            foreach (var i in x)
            {
                result = result * 10 + i;
            }

            return result;
        }

        public static List<int> ToList(int number)
        {
            var result = new int[9];
            for (int i = 8; i >= 0; i--)
            {
                result[i] = number % 10;
                number /= 10;
            }

            return result.ToList();
        }

        public static void CreateTextFile(string directoryPath, string fileName)
        {
            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(directoryPath);

                // Create the file (empty file)
                File.Create(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}");
            }
        }
    }
}
