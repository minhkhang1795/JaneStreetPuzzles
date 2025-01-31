using System.Collections.Concurrent;

namespace SquareSudoku
{
    public class Divisor
    {
        public const int SudokuSize = 9;
        public readonly int Value;

        // digit => matchedRow => number list
        public ConcurrentDictionary<int, ConcurrentDictionary<int, MatchedRow>> SkipDigits;

        public Divisor(int divisor, int skipDigit, int matchedRow, int number)
        {
            Value = divisor;
            SkipDigits = new ConcurrentDictionary<int, ConcurrentDictionary<int, MatchedRow>>();
            AddMatchedRow(skipDigit, matchedRow, number);
        }

        public void AddMatchedRow(int skipDigit, int matchedRow, int number)
        {
            var newNumber = new Number(skipDigit, matchedRow, number);
            SkipDigits.AddOrUpdate(skipDigit, 
                new ConcurrentDictionary<int, MatchedRow>(new List<KeyValuePair<int, MatchedRow>> { KeyValuePair.Create(matchedRow, new MatchedRow(matchedRow, newNumber)) }),
                (key, existingMatchedRowDict) =>
                {
                    existingMatchedRowDict.AddOrUpdate(matchedRow, new MatchedRow(matchedRow, newNumber), (_, oldMatchedRow) =>
                    {
                        oldMatchedRow.AddNumber(newNumber);
                        return oldMatchedRow;
                    });
                    return existingMatchedRowDict;
                });
        }

        public void ClearInvalidNumbers()
        {
            var keysToRemove = new List<int>();
            foreach (var (skipDigit, matchedRows) in SkipDigits)
            {
                if (matchedRows.Count < SudokuSize)
                {
                    keysToRemove.Add(skipDigit);
                }
            }

            keysToRemove.ForEach(key => SkipDigits.Remove(key, out var _));
        }


        public bool HasEnoughMatchedRows()
        {
            return SkipDigits.Count > 0;
        }

        public long CountNumber()
        {
            long count = 0;
            foreach (var (skipDigit, matchedRows) in SkipDigits)
            {
                foreach (var (_, matchedRow) in matchedRows)
                {
                    count += matchedRow.Numbers.Count;
                }
            }

            return count;
        }

        public long CountPermutations()
        {
            long count = 1;
            foreach (var (skipDigit, matchedRows) in SkipDigits)
            {
                foreach (var (_, matchedRow) in matchedRows)
                {
                    count *= matchedRow.Numbers.Count;
                }
            }

            return count == 1 ? 0 : count;
        }

        public void Merge(Divisor divisor)
        {
            foreach (var (skipDigit, matchedRows) in divisor.SkipDigits)
            {
                SkipDigits.AddOrUpdate(skipDigit,
                    matchedRows,
                    (key, existingMatchedRowDict) =>
                    {
                        foreach (var newMatchedRows in matchedRows)
                        {
                            existingMatchedRowDict.AddOrUpdate(newMatchedRows.Key, newMatchedRows.Value, (_, oldMatchedRow) =>
                            {
                                oldMatchedRow.AddNumbers(newMatchedRows.Value.Numbers);
                                return oldMatchedRow;
                            });
                        }
                        
                        return existingMatchedRowDict;
                    });
            }
        }
    }

    public class MatchedRow
    {
        public int Value;

        public ConcurrentBag<Number> Numbers;

        public MatchedRow(int value, Number firstNumber)
        {
            this.Value = value;
            Numbers = new ConcurrentBag<Number> { firstNumber };
        }

        public void AddNumber(Number number)
        {
            Numbers.Add(number);
        }

        public void AddNumbers(IEnumerable<Number> numbers)
        {
            foreach (var number in numbers)
            {
                Numbers.Add(number);
            }
        }
    }

    public class Number
    {
        public int SkipDigit { get; set; }
        public int MatchedRow { get; set; }
        public int Value { get; set; }

        public Number(int skipDigit, int matchedRow, int number)
        {
            this.SkipDigit = skipDigit;
            this.MatchedRow = matchedRow;
            this.Value = number;
        }
    }
}
