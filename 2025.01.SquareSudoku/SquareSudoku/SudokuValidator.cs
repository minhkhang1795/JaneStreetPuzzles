using System;

namespace SquareSudoku
{
    public class SudokuValidator
    {
        public List<List<int>> Sudoku;

        public SudokuValidator(List<List<int>> sudoku)
        {
            Sudoku = sudoku;
        }

        public bool ValidateNumberInAnyRows(List<int> number, out int matchedRow)
        {
            for (int i = 0; i < Sudoku.Count; i++)
            {
                if (ValidateNumberInRow(number, i))
                {
                    matchedRow = i;
                    return true;
                }
            }

            matchedRow = -1;
            return false;
        }

        public bool ValidateNumberInRow(List<int> number, int rowNumber)
        {
            if (rowNumber < 0 || rowNumber > Sudoku.Count)
            {
                throw new ArgumentException($"rowNumber {rowNumber} is out of bound");
            }

            var row = Sudoku[rowNumber];
            for (int i = 0; i < Sudoku.Count; i++)
            {
                // Must match this row.
                if (row[i] != -1 && number[i] != row[i])
                {
                    return false;
                }

                // Must not conflict with other rows.
                var otherRows = Sudoku.Where((row, index) => index != rowNumber);
                foreach (var otherRow in otherRows)
                {
                    if (otherRow[i] != -1 && number[i] == otherRow[i])
                    {
                        return false;
                    }
                }

                // Must not conflict with 3x3 grid
                var gridIndex = GetGridIndex(i, rowNumber);
                var minRow = 3 * (gridIndex / 3);
                var minColumn = 3 * (gridIndex % 3);
                for (int r = minRow; r < minRow + 2; r++)
                {
                    for (int c = minColumn; c < minColumn + 2; c++)
                    {
                        if (r == rowNumber && c == i)
                        {
                            continue;
                        }

                        if (Sudoku[r][c] != -1 && number[i] == Sudoku[r][c])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool ValidateSudoku()
        {
            for (int i = 0; i < Sudoku.Count; i++)
            {
                if (!ValidateNumberInRow(Sudoku[i], i))
                {
                    return false;
                }
            }

            return true;
        }

        public void Print()
        {
            foreach (var sudoku in Sudoku)
            {
                foreach (var row in sudoku)
                {
                    Console.Write($"{row}\t");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public void Save(int gdc)
        {
            Utility.CreateTextFile($"{AppDomain.CurrentDomain.BaseDirectory}\\result\\", $"{gdc}.txt");
        }

        private bool ValidateRow(List<int> number)
        {
            var filtered = number.Where(x => x != -1);
            return filtered.Count() == filtered.Distinct().Count();
        }

        private int GetGridIndex(int col, int row)
        {
            if (row <= 2)
            {
                if (col <= 2)
                {
                    return 0;
                }
                else if (col > 2 && col <= 5)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else if (row > 2 && row <= 5)
            {
                if (col <= 2)
                {
                    return 3;
                }
                else if (col > 2 && col <= 5)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                if (col <= 2)
                {
                    return 6;
                }
                else if (col > 2 && col <= 5)
                {
                    return 7;
                }
                else
                {
                    return 8;
                }
            }
        }
    }
}
