using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Translates a given Sudoku into its equivalent exact cover representation,
//solves it using the CoverMatrixSolver and translates the solution back into a filled sudoku grid.

namespace SolverCore
{
    public class SudokuSolver : CoverMatrixSolver
    {
        protected SudokuGrid InputSudoku;

        public void SetInput(SudokuGrid Input)
        {
            ClearSolution();

            if (Input.Valid == false)
            {
                throw new ArgumentException();
            }

            int NumRows = 729;
            int NumCols = 324;

            InputSudoku = Input;
            BuildFrame(NumRows, NumCols);

            bool[] CoverMatrixRow = new bool[NumCols];
            int CellValue, RowNo, ColNo;
            for (int i = 0; i < NumRows; i++)
            {
                CoverMatrixIndexToVal(i, out CellValue, out RowNo, out ColNo);
                GenerateCoverMatrixRow(CellValue, RowNo, ColNo, CoverMatrixRow);
                AddInputRow(i, CoverMatrixRow);

                //Test
                int test = CoverMatrixValToIndex(CellValue, RowNo, ColNo);
                if (test != i)
                {
                    throw new Exception();
                }
            }

            //Select rows according to given knowns
            for (RowNo = 0; RowNo < InputSudoku.Data.GetLength(0); RowNo++)
            {
                for (ColNo = 0; ColNo < InputSudoku.Data.GetLength(1); ColNo++)
                {
                    CellValue = (int)InputSudoku.Data[RowNo, ColNo];
                    if (CellValue != 0)
                    {
                        ForceRowInSolution(CoverMatrixValToIndex(CellValue, RowNo, ColNo));
                    }
                }
            }
        }

        protected int CoverMatrixValToIndex(int CellValue, int RowNo, int ColNo)
        {
            return (CellValue - 1) + ColNo * 9 + RowNo * 81;
        }

        protected void CoverMatrixIndexToVal(int Index, out int CellValue, out int RowNo, out int ColNo)
        {
            RowNo = Index / 81;
            Index = Index - RowNo * 81;
            ColNo = Index / 9;
            CellValue = Index - ColNo * 9 + 1;
        }

        protected void GenerateCoverMatrixRow(int CellValue, int RowNo, int ColNo, bool[] MatrixRow)
        {
            CellValue--;
            int BlockNo = CalculateBlockNo(RowNo, ColNo);

            int Condition1Ix = RowNo * 9 + ColNo;
            int Condition2Ix = RowNo * 9 + CellValue + 81;
            int Condition3Ix = ColNo * 9 + CellValue + 81 * 2;
            int Condition4Ix = BlockNo * 9 + CellValue + 81 * 3;

            for (int i = 0; i < MatrixRow.Length; i++)
            {
                if (i == Condition1Ix || i == Condition2Ix || i == Condition3Ix || i == Condition4Ix)
                {
                    MatrixRow[i] = true;
                }
                else
                {
                    MatrixRow[i] = false;
                }
            }
        }

        protected int CalculateBlockNo(int RowNo, int ColNo)
        {
            int BlockNo = (RowNo / 3) * 3 + (ColNo / 3);
            return BlockNo;
        }

        public SudokuGrid ConvertSolution(int SolutionIx)
        {
            SudokuGrid Output = new SudokuGrid();
            var SelectedSolution = Solutions[SolutionIx];
            
            int CellValue, RowNo, ColNo;
            foreach (int i in SelectedSolution)
            {
                CoverMatrixIndexToVal(i, out CellValue, out RowNo, out ColNo);
                Output.Data[RowNo, ColNo] = (byte)CellValue;
            }

            return Output;
        }
    }
}
