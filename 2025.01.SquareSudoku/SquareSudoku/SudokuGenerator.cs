
using System.Collections.Concurrent;

namespace SquareSudoku
{
    public class SudokuGenerator
    {
        public ConcurrentDictionary<int, MatchedRow> MatchedRows;

        private List<List<int>> InitialSudoku;

        public SudokuGenerator(ConcurrentDictionary<int, MatchedRow> matchedRows, List<List<int>> initialSudoku)
        {
            MatchedRows = matchedRows;
            InitialSudoku = initialSudoku;
        }

        public List<List<int>> Clone(List<List<int>> sudoku)
        {
            var newSudoku = new List<List<int>>(sudoku.Count);
            for (int i = 0; i < sudoku.Count; i++)
            {
                newSudoku.Add(new List<int>(sudoku[i].Count));
                for (int j = 0; j < sudoku[i].Count; j++)
                {
                    newSudoku[i].Add(sudoku[i][j]);
                }
            }
            return newSudoku;
        }

        public SudokuValidator? Generate()
        {
            var sudokuList = Clone(InitialSudoku);
            var sortedMatchedRows = MatchedRows.Select(x => (x.Value.Numbers.Count, x.Value)).ToList();
            sortedMatchedRows = sortedMatchedRows.OrderBy(x => x.Count).ToList();

            Console.WriteLine($"- Total sudoku to check {GetTotalSudoku()}.");
            return GenerateRecursive(0, sortedMatchedRows, sudokuList);
        }

        private long GetTotalSudoku()
        {
            long count = 1;
            foreach (var (_, matchedRow) in MatchedRows)
            {
                count *= matchedRow.Numbers.Count;
            }

            return count < 0 ? long.MaxValue : count;
        }

        private SudokuValidator? GenerateRecursive(int index, List<(int Count, MatchedRow MatchedRow)> sortedMatchedRows, List<List<int>> current)
        {
            if (index == sortedMatchedRows.Count())
            {
                var sudokuValidator = new SudokuValidator(current);
                return sudokuValidator.ValidateSudoku() ? sudokuValidator : null;
            }

            var rowIndex = sortedMatchedRows[index].MatchedRow.Value;
            foreach (var number in sortedMatchedRows[index].MatchedRow.Numbers)
            {
                current[rowIndex] = Utility.ToList(number.Value);
                var sudokuValidator = new SudokuValidator(current);
                if (!sudokuValidator.ValidateSudoku())
                {
                    current[rowIndex] = InitialSudoku[rowIndex];
                    continue;
                }

                var nextRecursion = GenerateRecursive(index + 1, sortedMatchedRows, current);
                if (nextRecursion != null)
                {
                    return nextRecursion;
                }

                // Back-track
                current[rowIndex] = InitialSudoku[rowIndex];
            }

            return null;
        }

        public IEnumerable<SudokuValidator> Generate2()
        {
            for (var row0 = 0; row0 < MatchedRows[0].Numbers.Count; row0++)
            {
                for (var row1 = 0; row1 < MatchedRows[1].Numbers.Count; row1++)
                {
                    for (var row2 = 0; row2 < MatchedRows[2].Numbers.Count; row2++)
                    {
                        for (var row3 = 0; row3 < MatchedRows[3].Numbers.Count; row3++)
                        {
                            for (var row4 = 0; row4 < MatchedRows[4].Numbers.Count; row4++)
                            {
                                for (var row5 = 0; row5 < MatchedRows[5].Numbers.Count; row5++)
                                {
                                    for (var row6 = 0; row6 < MatchedRows[6].Numbers.Count; row6++)
                                    {
                                        for (var row7 = 0; row7 < MatchedRows[7].Numbers.Count; row7++)
                                        {
                                            for (var row8 = 0; row8 < MatchedRows[8].Numbers.Count; row8++)
                                            {
                                                yield return new SudokuValidator(new List<List<int>>
                                                {
                                                    Utility.ToList(MatchedRows[0].Numbers.ElementAt(row0).Value),
                                                    Utility.ToList(MatchedRows[1].Numbers.ElementAt(row1).Value),
                                                    Utility.ToList(MatchedRows[2].Numbers.ElementAt(row2).Value),
                                                    Utility.ToList(MatchedRows[3].Numbers.ElementAt(row3).Value),
                                                    Utility.ToList(MatchedRows[4].Numbers.ElementAt(row4).Value),
                                                    Utility.ToList(MatchedRows[5].Numbers.ElementAt(row5).Value),
                                                    Utility.ToList(MatchedRows[6].Numbers.ElementAt(row6).Value),
                                                    Utility.ToList(MatchedRows[7].Numbers.ElementAt(row7).Value),
                                                    Utility.ToList(MatchedRows[8].Numbers.ElementAt(row8).Value),
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
