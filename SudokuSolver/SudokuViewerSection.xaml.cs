using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for SudokuViewerSection.xaml
    /// </summary>
    public partial class SudokuViewerSection : UserControl
    {
        Label[,] LabelsArr;

        public SudokuViewerSection()
        {
            InitializeComponent();

            LabelsArr = new Label[3, 3];
            LabelsArr[0, 0] = NumLabel00;
            LabelsArr[0, 1] = NumLabel01;
            LabelsArr[0, 2] = NumLabel02;
            LabelsArr[1, 0] = NumLabel10;
            LabelsArr[1, 1] = NumLabel11;
            LabelsArr[1, 2] = NumLabel12;
            LabelsArr[2, 0] = NumLabel20;
            LabelsArr[2, 1] = NumLabel21;
            LabelsArr[2, 2] = NumLabel22;
        }

        public void DisplayIncomplete(int BoxNo, byte[,] IncompleteData)
        {
            SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);

            int StartRow, StartCol;
            SolverCore.SudokuGrid.SquareStartCoordinates(BoxNo, out StartRow, out StartCol);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    byte Val = IncompleteData[StartRow + i, StartCol + j];
                    if (Val > 0 && Val < 10)
                    {
                        LabelsArr[i, j].Content = IncompleteData[StartRow + i, StartCol + j];
                        LabelsArr[i, j].Foreground = RedBrush;
                    }
                    else
                    {
                        LabelsArr[i, j].Content = string.Empty;
                    }
                }
            }
        }

        public void DisplaySolution(int BoxNo, byte[,] IncompleteData, byte[,] SolutionData)
        {
            SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
            SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);

            int StartRow, StartCol;
            SolverCore.SudokuGrid.SquareStartCoordinates(BoxNo, out StartRow, out StartCol);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    byte Val = SolutionData[StartRow + i, StartCol + j];
                    byte RefVal = IncompleteData[StartRow + i, StartCol + j];

                    LabelsArr[i, j].Content = Val;
                    if (RefVal == Val)
                    {
                        LabelsArr[i, j].Foreground = RedBrush;
                    }
                    else
                    {
                        LabelsArr[i, j].Foreground = BlackBrush;
                    }
                }
            }
        }
    }
}
