using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//This is an implementation of Donald Knuth's Algorith X using the Dancing Links technique.
//It allows to solve the exact cover problem for arbitrary matrices by brute force and is widely documented on the internet.
//A limiting factor has been added so that the algorithm will stop recursing after a certain number of solutions has been found.
//http://en.wikipedia.org/wiki/Knuth%27s_Algorithm_X
//http://en.wikipedia.org/wiki/Dancing_Links

namespace SolverCore
{
    public class CoverMatrixSolver
    {
        public class Node
        {
            public int RowNo { get; set; }
            public int ColNo { get; set; }
            public HeaderNode Header { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public Node Up { get; set; }
            public Node Down { get; set; }

            public Node()
            {
                RowNo = -1;
                ColNo = -1;
                Left = this;
                Right = this;
                Up = this;
                Down = this;
            }
        }

        public class HeaderNode : Node
        {
            public string Name { get; set; }
            public int Size { get; set; }
        }

        public int MaxSolutions { get; set; }
        protected bool StopSearch = true;

        protected HeaderNode StartNode;
        protected HeaderNode[] ColumnHeaders;
        protected Node[] RowHeaders;

        protected List<Node> SolutionNodes;
        protected List<List<int>> solutions;
        public List<List<int>> Solutions { get { return solutions; } }

        public CoverMatrixSolver()
        {
            MaxSolutions = 10000;
            SolutionNodes = new List<Node>();
            solutions = new List<List<int>>();
        }

        public void SetInput(bool[,] Input)
        {
            ClearSolution();
            int NumRows = Input.GetLength(0);
            int NumCols = Input.GetLength(1);
            BuildFrame(NumRows, NumCols);
            bool[] InputRow = new bool[NumCols];
            for (int i = 0; i < NumRows; i++)
            {
                for (int j = 0; j < NumCols; j++)
                {
                    InputRow[j] = Input[i, j];
                }
                AddInputRow(i, InputRow);
            }
        }

        //Create the framework for the Dancing Links data structure
        protected void BuildFrame(int NumRows, int NumColumns)
        {
            StartNode = new HeaderNode();
            StartNode.Name = "Start Node";
            ColumnHeaders = new HeaderNode[NumColumns];
            for (int i = 0; i < ColumnHeaders.Length; i++)
            {
                HeaderNode NewNode = new HeaderNode();
                NewNode.Name = string.Format("Column {0}", i);
                ColumnHeaders[i] = NewNode;
                InsertLeft(StartNode, NewNode);
            }

            RowHeaders = new Node[NumRows];
        }

        protected void AddInputRow(int RowIx, bool[] Input)
        {
            for (int i = 0; i < Input.Length; i++)
            {
                if (Input[i] == true)
                {
                    Node NewNode = new Node();
                    NewNode.Header = ColumnHeaders[i];
                    NewNode.ColNo = i;
                    NewNode.RowNo = RowIx;
                    InsertUp(ColumnHeaders[i], NewNode);
                    if (RowHeaders[RowIx] == null)
                    {
                        RowHeaders[RowIx] = NewNode;
                    }
                    else
                    {
                        InsertLeft(RowHeaders[RowIx], NewNode);
                    }
                }
            }
        }

        public void ClearSolution()
        {
            Solutions.Clear();
            SolutionNodes.Clear();
        }

        //Force the inclusion of a row in the solution and hides the corresponding column/rows from the search algorithm. This may cause the problem to be unsolvable.
        //This is required to ensure a solution conforms to the propblem's given data.
        public void ForceRowInSolution(int RowIx)
        {
            Node CurrNode = RowHeaders[RowIx];
            SolutionNodes.Add(CurrNode);
            do
            {
                CoverColumn(CurrNode.Header);
                CurrNode = CurrNode.Right;
            } while (CurrNode != RowHeaders[RowIx]);
        }

        public void Solve()
        {
            StopSearch = false;
            Search();
        }

        //Recursive search function.
        protected void Search()
        {
            //If enough solutions have been found, stop recursing
            if (StopSearch == true) { return; }

            //Solution found
            if (StartNode.Right == StartNode)
            {
                AddSolution();
                if (Solutions.Count >= MaxSolutions) { StopSearch = true; }
                return;
            }

            HeaderNode ChosenColumn = ChooseColumn();
            CoverColumn(ChosenColumn);
            for (Node i = ChosenColumn.Down; i != ChosenColumn; i = i.Down)
            {
                SolutionNodes.Add(i);
                for (Node j = i.Right; j != i; j = j.Right)
                {
                    CoverColumn(j.Header);
                }
                Search();
                i = SolutionNodes.Last();
                SolutionNodes.Remove(i);
                ChosenColumn = i.Header;
                for (Node j = i.Left; j != i; j = j.Left)
                {
                    UncoverColumn(j.Header);
                }
            }
            UncoverColumn(ChosenColumn);
        }

        //Column manipulation, these two functions hide/restore matrix rows and columns from the search function.
        //It is important that the uncover function operates exactly in reverse order to the cover one.
        protected void CoverColumn(HeaderNode TargetNode)
        {
            RemoveLeftRight(TargetNode);
            for (Node i = TargetNode.Down; i != TargetNode; i = i.Down)
            {
                for (Node j = i.Right; j != i; j = j.Right)
                {
                    RemoveUpDown(j);
                    j.Header.Size--;
                }
            }
        }

        protected void UncoverColumn(HeaderNode TargetNode)
        {
            for (Node i = TargetNode.Up; i != TargetNode; i = i.Up)
            {
                for (Node j = i.Left; j != i; j = j.Left)
                {
                    RestoreUpDown(j);
                    j.Header.Size++;
                }
            }
            RestoreLeftRight(TargetNode);
        }

        //Choose column with least nodes to minimize time to discard branches without a solution.
        protected HeaderNode ChooseColumn()
        {
            int MinSize = int.MaxValue;
            HeaderNode Output = null;
            for (HeaderNode i = StartNode.Right as HeaderNode; i != StartNode; i = i.Right as HeaderNode)
            {
                if (i.Size < MinSize)
                {
                    MinSize = i.Size;
                    Output = i;
                }
            }

            return Output;
        }

        //Add a solution to the results vector.
        protected void AddSolution()
        {
            List<int> NewSolution = new List<int>();
            foreach (var i in SolutionNodes)
            {
                NewSolution.Add(i.RowNo);
            }
            solutions.Add(NewSolution);
        }

        //Auxiliary functions for linked list manipulation
        protected void InsertRight(Node TargetNode, Node NewNode)
        {
            NewNode.Right = TargetNode.Right;
            NewNode.Left = TargetNode;
            TargetNode.Right.Left = NewNode;
            TargetNode.Right = NewNode;
        }

        protected void InsertLeft(Node TargetNode, Node NewNode)
        {
            NewNode.Left = TargetNode.Left;
            NewNode.Right = TargetNode;
            TargetNode.Left.Right = NewNode;
            TargetNode.Left = NewNode;
        }

        protected void InsertUp(Node TargetNode, Node NewNode)
        {
            NewNode.Up = TargetNode.Up;
            NewNode.Down = TargetNode;
            TargetNode.Up.Down = NewNode;
            TargetNode.Up = NewNode;
        }

        protected void InsertDown(Node TargetNode, Node NewNode)
        {
            NewNode.Down = TargetNode.Down;
            NewNode.Up = TargetNode;
            TargetNode.Down.Up = NewNode;
            TargetNode.Down = NewNode;
        }

        protected void RemoveLeftRight(Node TargetNode)
        {
            TargetNode.Left.Right = TargetNode.Right;
            TargetNode.Right.Left = TargetNode.Left;
        }

        protected void RestoreLeftRight(Node TargetNode)
        {
            TargetNode.Left.Right = TargetNode;
            TargetNode.Right.Left = TargetNode;
        }

        protected void RemoveUpDown(Node TargetNode)
        {
            TargetNode.Up.Down = TargetNode.Down;
            TargetNode.Down.Up = TargetNode.Up;
        }

        public void RestoreUpDown(Node TargetNode)
        {
            TargetNode.Up.Down = TargetNode;
            TargetNode.Down.Up = TargetNode;
        }
    }
}
