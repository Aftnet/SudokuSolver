using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SolverCore.Test
{
    [TestClass]
    public class SolverCoreTest
    {
        const int TestIterations = 1000;
        Random RandomGenerator;

        [TestInitialize]
        public void Setup()
        {
            RandomGenerator = new Random(DateTime.UtcNow.Millisecond);
        }

        [TestMethod]
        public void InvalidSudokuRowConditionDetected()
        {
            for (int i = 0; i < TestIterations; i++)
            {
                var Sudoku = GenerateFailingSudokuRowCondition();
                Assert.IsFalse(Sudoku.Valid);
            }
        }

        [TestMethod]
        public void InvalidSudokuColumnConditionDetected()
        {
            for (int i = 0; i < TestIterations; i++)
            {
                var Sudoku = GenerateFailingSudokuColumnCondition();
                Assert.IsFalse(Sudoku.Valid);
            }
        }

        [TestMethod]
        public void InvalidSudokuSquareConditionDetected()
        {
            for (int i = 0; i < TestIterations; i++)
            {
                var Sudoku = GenerateFailingSudokuSquareCondition();
                Assert.IsFalse(Sudoku.Valid);
            }
        }

        [TestMethod]
        public void SingleSolutionSudokuProperlySolved()
        {
            var TestSudoku = new SudokuGrid();
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
            var TestSolver = new SudokuSolver();
            TestSolver.SetInput(TestSudoku);
            TestSolver.Solve();
            Assert.AreEqual(TestSolver.Solutions.Count, 1);

            var TestSolution = TestSolver.ConvertSolution(0);
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
                Assert.AreEqual(ExpectedSolution[i], TestSolution.LinearArr[i]);
            }
        }

        [TestMethod]
        public void MultiSolutionSudokuProperlySolved()
        {
            var TestSolver = new SudokuSolver();
            TestSolver.MaxSolutions = 1;
            for (int i = 0; i < TestIterations; i++)
            {
                var TestSudoku = GenerateRandomSudoku();
                TestSolver.SetInput(TestSudoku);
                TestSolver.Solve();
                var TestSolution = TestSolver.ConvertSolution(0);
                Assert.IsTrue(TestSolution.Valid);
            }
        }

        protected SudokuGrid GenerateFailingSudokuRowCondition()
        {
            int ColNo1 = RandomGenerator.Next() % 9;
            int ColNo2 = RandomGenerator.Next() % 9;
            int RowNo = RandomGenerator.Next() % 9;
            byte RandVal = (byte)((RandomGenerator.Next() % 9) + 1);
            while (ColNo1 == ColNo2)
            {
                ColNo2 = RandomGenerator.Next() % 9;
            }

            var Output = new SudokuGrid();
            Output.Data[RowNo, ColNo1] = RandVal;
            Output.Data[RowNo, ColNo2] = RandVal;
            return Output;
        }

        protected SudokuGrid GenerateFailingSudokuColumnCondition()
        {
            int RowNo1 = RandomGenerator.Next() % 9;
            int RowNo2 = RandomGenerator.Next() % 9;
            int ColNo = RandomGenerator.Next() % 9;
            byte RandVal = (byte)((RandomGenerator.Next() % 9) + 1);
            while (RowNo1 == RowNo2)
            {
                RowNo2 = RandomGenerator.Next() % 9;
            }

            var Output = new SudokuGrid();
            Output.Data[RowNo1, ColNo] = RandVal;
            Output.Data[RowNo2, ColNo] = RandVal;
            return Output;
        }

        protected SudokuGrid GenerateFailingSudokuSquareCondition()
        {
            int BoxNo = RandomGenerator.Next() % 9;
            int RowNo1 = RandomGenerator.Next() % 3;
            int RowNo2 = RandomGenerator.Next() % 3;
            int ColNo1 = RandomGenerator.Next() % 3;
            int ColNo2 = RandomGenerator.Next() % 3;
            byte RandVal = (byte)((RandomGenerator.Next() % 9) + 1);
            while (ColNo1 == ColNo2)
            {
                ColNo2 = RandomGenerator.Next() % 3;
            }
            while (RowNo1 == RowNo2)
            {
                RowNo2 = RandomGenerator.Next() % 3;
            }

            var Output = new SudokuGrid();
            int SquareStartRow, SquareStartCol;
            SolverCore.SudokuGrid.SquareStartCoordinates(BoxNo, out SquareStartRow, out SquareStartCol);
            Output.Data[SquareStartRow + RowNo1, SquareStartCol + ColNo1] = RandVal;
            Output.Data[SquareStartRow + RowNo2, SquareStartCol + ColNo2] = RandVal;
            return Output;
        }

        protected SudokuGrid GenerateRandomSudoku()
        {
            int RowNo = RandomGenerator.Next() % 9;
            int ColNo = RandomGenerator.Next() % 9;
            byte RandVal = (byte)((RandomGenerator.Next() % 9) + 1);

            var Output = new SudokuGrid();
            Output.Data[RowNo, ColNo] = RandVal;
            return Output;
        }
    }
}
