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
    /// Interaction logic for SudokuViewer.xaml
    /// </summary>
    public partial class SudokuViewer : UserControl
    {
        SudokuViewerSection[] Sections;

        public SudokuViewer()
        {
            InitializeComponent();

            Sections = new SudokuViewerSection[9]
            {
                Square0, Square1, Square2, Square3, Square4, Square5, Square6, Square7, Square8
            };
        }

        public void DisplayIncomplete(byte[,] IncompleteData)
        {
            for (int i = 0; i < 9; i++)
            {
                Sections[i].DisplayIncomplete(i, IncompleteData);
            }
        }

        public void DisplaySolution(byte[,] IncompleteData, byte[,] Solutiondata)
        {
            for (int i = 0; i < 9; i++)
            {
                Sections[i].DisplaySolution(i, IncompleteData, Solutiondata);
            }
        }
    }
}
