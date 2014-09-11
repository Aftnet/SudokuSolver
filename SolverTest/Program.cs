using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolverTest
{
    class Program
    {
        protected delegate void GenerateSudokuFailCondition(Random Rand, SolverCore.SudokuGrid Sudoku);

        static void Main(string[] args)
        {
            bool[] TestPassed = new bool[16];
            Random Rand = new Random(DateTime.UtcNow.Millisecond);
            SolverCore.SudokuGrid TestSudoku = new SolverCore.SudokuGrid();
            SolverCore.SudokuGrid TestSolution = new SolverCore.SudokuGrid();
            SolverCore.SudokuSolver TestSolver = new SolverCore.SudokuSolver();
            TestSolver.MaxSolutions = 10000;

            Console.WriteLine("Beginning test\n");

            Console.WriteLine("Testing Sudoku grid validation:");
            int GridValidationTestsIterations = 10000;
            TestSudokuValidationFailCondition("row failure", TestPassed, 0, GenerateFailingSudokuRowCondition, TestSudoku, GridValidationTestsIterations, Rand);
            TestSudokuValidationFailCondition("column failure", TestPassed, 1, GenerateFailingSudokuColumnCondition, TestSudoku, GridValidationTestsIterations, Rand);
            TestSudokuValidationFailCondition("square failure", TestPassed, 2, GenerateFailingSudokuSquareCondition, TestSudoku, GridValidationTestsIterations, Rand);

            Console.WriteLine("\nTesting Sudoku solver:");
            Console.Write("Testing a sudoku with a single known solution...");
            TestPassed[3] = true;
            TestSudoku.LinearArr = new byte[81]
            {
                5, 3, 0, 0, 7, 0, 0, 0, 0,
                6, 0, 0, 1, 9, 5, 0, 0, 0,
                0, 9, 8, 0, 0, 0, 0, 6, 0,
                8, 0, 0, 0, 6, 0, 0, 0, 3,
                4, 0, 0, 8, 0, 3, 0, 0, 1,
                7, 0, 0, 0, 2, 0, 0, 0, 6,
                0, 6, 0, 0, 0, 0, 2, 8, 0,
                0, 0, 0, 4, 1, 9, 0, 0, 5,
                0, 0, 0, 0, 8, 0, 0, 7, 9
            };
            TestSolver.SetInput(TestSudoku);
            TestSolver.Solve();
            if (TestSolver.Solutions.Count != 1)
            {
                Console.WriteLine("\nSlover didn't find a single solution");
                TestPassed[3] = false;
            }
            else
            {
                TestSolution = TestSolver.ConvertSolution(0);
                byte[] ExpectedSolution = new byte[81]
                {
                    5, 3, 4, 6, 7, 8, 9, 1, 2,
                    6, 7, 2, 1, 9, 5, 3, 4, 8,
                    1, 9, 8, 3, 4, 2, 5, 6, 7,
                    8, 5, 9, 7, 6, 1, 4, 2, 3,
                    4, 2, 6, 8, 5, 3, 7, 9, 1,
                    7, 1, 3, 9, 2, 4, 8, 5, 6,
                    9, 6, 1, 5, 3, 7, 2, 8, 4,
                    2, 8, 7, 4, 1, 9, 6, 3, 5,
                    3, 4, 5, 2, 8, 6, 1, 7, 9
                };
                for (int i = 0; i < TestSolution.LinearArr.Length; i++)
                {
                    if (ExpectedSolution[i] != TestSolution.LinearArr[i])
                    {
                        Console.WriteLine("\nSolution doesn't match the expected one");
                        TestPassed[3] = false;
                        break;
                    }
                }
            }
            if (TestPassed[3] == true)
            {
                Console.Write(" Passed\n");
            }

            int SolutionValidityTestIterations = 1000;
            Console.Write(string.Format("Check first solution validity for random {0} one element sudokus... ", SolutionValidityTestIterations));
            TestSolver.MaxSolutions = 1;
            TestPassed[4] = true;
            for (int i = 0; i < SolutionValidityTestIterations; i++)
            {
                GenerateRandomSudoku(Rand, TestSudoku);
                TestSolver.SetInput(TestSudoku);
                TestSolver.Solve();
                TestSolution = TestSolver.ConvertSolution(0);
                if(TestSolution.Valid == false)
                {
                    TestPassed[4] = false;
                    Console.Write("\nInvalid solution found\n");
                    Console.Write(TestSolution.Print());
                }
            }

            if(TestPassed[4] == true)
            {
                Console.Write(" Passed\n");
            }
            Console.ReadKey();
        }

        protected static void TestSudokuValidationFailCondition(string FailconditionName, bool[] TestResultsArr, int ResultIx, GenerateSudokuFailCondition FailureGenFunc, SolverCore.SudokuGrid TestSudoku, int TestIterations, Random Rand)
        {
            Console.Write("Testing " + FailconditionName + " detection... ");
            TestResultsArr[ResultIx] = true;
            for (int i = 0; i < TestIterations; i++)
            {
                FailureGenFunc(Rand, TestSudoku);
                if (TestSudoku.Valid == true)
                {
                    TestResultsArr[ResultIx] = false;
                    Console.Write("\n" + FailconditionName + " not detected for grid:");
                    Console.Write(TestSudoku.Print());
                }
            }
            if (TestResultsArr[ResultIx] == true)
            {
                Console.Write("Passed\n");
            }
        }

        protected static void GenerateFailingSudokuRowCondition(Random Rand, SolverCore.SudokuGrid Sudoku)
        {
            int ColNo1 = Rand.Next() % 9;
            int ColNo2 = Rand.Next() % 9;
            int RowNo = Rand.Next() % 9;
            byte RandVal = (byte)((Rand.Next() % 9) + 1);
            while (ColNo1 == ColNo2)
            {
                ColNo2 = Rand.Next() % 9;
            }

            Sudoku.Clear();
            Sudoku.Data[RowNo, ColNo1] = RandVal;
            Sudoku.Data[RowNo, ColNo2] = RandVal;
        }

        protected static void GenerateFailingSudokuColumnCondition(Random Rand, SolverCore.SudokuGrid Sudoku)
        {
            int RowNo1 = Rand.Next() % 9;
            int RowNo2 = Rand.Next() % 9;
            int ColNo = Rand.Next() % 9;
            byte RandVal = (byte)((Rand.Next() % 9) + 1);
            while (RowNo1 == RowNo2)
            {
                RowNo2 = Rand.Next() % 9;
            }

            Sudoku.Clear();
            Sudoku.Data[RowNo1, ColNo] = RandVal;
            Sudoku.Data[RowNo2, ColNo] = RandVal;
        }

        protected static void GenerateFailingSudokuSquareCondition(Random Rand, SolverCore.SudokuGrid Sudoku)
        {
            int BoxNo = Rand.Next() % 9;
            int RowNo1 = Rand.Next() % 3;
            int RowNo2 = Rand.Next() % 3;
            int ColNo1 = Rand.Next() % 3;
            int ColNo2 = Rand.Next() % 3;
            byte RandVal = (byte)((Rand.Next() % 9) + 1);
            while (ColNo1 == ColNo2)
            {
                ColNo2 = Rand.Next() % 3;
            }
            while (RowNo1 == RowNo2)
            {
                RowNo2 = Rand.Next() % 3;
            }

            Sudoku.Clear();
            int SquareStartRow, SquareStartCol;
            SolverCore.SudokuGrid.SquareStartCoordinates(BoxNo, out SquareStartRow, out SquareStartCol);
            Sudoku.Data[SquareStartRow + RowNo1, SquareStartCol + ColNo1] = RandVal;
            Sudoku.Data[SquareStartRow + RowNo2, SquareStartCol + ColNo2] = RandVal;
        }

        protected static void GenerateRandomSudoku(Random Rand, SolverCore.SudokuGrid Sudoku)
        {
            int RowNo = Rand.Next() % 9;
            int ColNo = Rand.Next() % 9;
            byte RandVal = (byte)((Rand.Next() % 9) + 1);

            Sudoku.Clear();
            Sudoku.Data[RowNo, ColNo] = RandVal;
        }
    }
}
