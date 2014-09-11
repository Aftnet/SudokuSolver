using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Essentially a wrapper around the 2D array represenattion of a Sudoku grid.
//Has functionality to check Sudoku rules compliance.

namespace SolverCore
{
    public class SudokuGrid
    {
        protected byte[,] data;
        public byte[,] Data { get { return data; } }

        public SudokuGrid()
        {
            data = new byte[9, 9];
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Data[i, j] = 0;
                }
            }
        }

        public byte[] LinearArr
        {
            get
            {
                byte[] Output = new byte[81];

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Output[i * 9 + j] = Data[i, j];
                    }
                }

                return Output;
            }
            set
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Data[i, j] = value[i * 9 + j];
                    }
                }
            }
        }

        public bool Valid
        {
            get
            {
                if (ValuesAreInRange() == false) { return false; }
                for (int i = 0; i < Data.GetLength(0); i++)
                {
                    if (RowIsValid(i) == false) { return false; }
                    if (ColumnIsValid(i) == false) { return false; }
                    if (SquareIsValid(i) == false) { return false; }
                }
                return true;
            }
        }

        public bool Solved
        {
            get
            {
                for (int i = 0; i < Data.GetLength(0); i++)
                {
                    for (int j = 0; j < Data.GetLength(1); j++)
                    {
                        if (Data[i, j] == 0) { return false; }
                    }
                }

                return true;
            }
        }

        public static void SquareStartCoordinates(int SquareNo, out int StartRow, out int StartCol)
        {
            StartRow = (SquareNo / 3) * 3;
            StartCol = (SquareNo % 3) * 3;
        }

        public string Print()
        {
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    SB.Append(Data[i, j]);
                }
                SB.Append("\n");
            }
            return SB.ToString();
        }

        protected bool ValuesAreInRange()
        {
            for (int i = 0; i < Data.GetLength(0); i++)
            {
                for (int j = 0; j < Data.GetLength(1); j++)
                {
                    int CurrentValue = Data[i, j];
                    if (CurrentValue < 0 || CurrentValue > 9)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool RowIsValid(int RowNumber)
        {
            int[] Array = new int[Data.GetLength(0)];
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i] = Data[i, RowNumber];
            }
            return ArrayIsValid(Array);
        }

        protected bool ColumnIsValid(int ColumnNumber)
        {
            int[] Array = new int[Data.GetLength(1)];
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i] = Data[ColumnNumber, i];
            }
            return ArrayIsValid(Array);
        }

        protected bool SquareIsValid(int SquareNumber)
        {
            int SquareStartRow, SquareStartColumn;
            SquareStartCoordinates(SquareNumber, out SquareStartRow, out SquareStartColumn);

            int[] Array = new int[Data.GetLength(0)];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Array[i * 3 + j] = Data[SquareStartColumn + i, SquareStartRow + j];
                }
            }
            return ArrayIsValid(Array);
        }

        protected bool ArrayIsValid(int[] Input)
        {
            for (int Control = 1; Control < 10; Control++)
            {
                int NumOccurrencies = 0;
                foreach (int i in Input)
                {
                    if (i == Control)
                    {
                        NumOccurrencies++;
                    }
                }
                if (NumOccurrencies > 1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
