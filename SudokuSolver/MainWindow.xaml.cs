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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SolverCore.SudokuGrid InputSudoku = null;
        private SolverCore.SudokuGrid CurrentSolution = null;
        private SolverCore.SudokuSolver SudokuSolver = null;

        int CurrentlyShownSolutionIx = 0;

        public MainWindow()
        {
            InitializeComponent();

            InputSudoku = new SolverCore.SudokuGrid();
            SudokuSolver = new SolverCore.SudokuSolver();
            SudokuSolver.MaxSolutions = 10000;

            SolveBtn.IsEnabled = false;
            PrevSolBtn.IsEnabled = false;
            NextSolBtn.IsEnabled = false;

            SolutionsStatusLabel.Content = "No solutions";
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SolverCore.SudokuGrid TempSudoku = new SolverCore.SudokuGrid();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Input"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Sudoku puzzles (.json)|*.json"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                try
                {
                    SolverCore.Loader.LoadFromFile(filename, TempSudoku);
                }
                catch (Exception Exc)
                {
                    string messageBoxText = "Unable to load selected file.";
                    string caption = "Error";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(messageBoxText, caption, button, icon);

                    return;
                }

                InputSudoku = TempSudoku;
                if (InputSudoku.Valid == true)
                {
                    SolveBtn.IsEnabled = true;
                }
                else
                {
                    //Entered input is invalid. Notify user
                    string messageBoxText = "Input sudoku is invalid";
                    string caption = "Error";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(messageBoxText, caption, button, icon);

                    SolveBtn.IsEnabled = false;
                }

                SudokuViewerPane.DisplayIncomplete(InputSudoku.Data);

                CurrentSolution = null;
                SolutionsStatusLabel.Content = "No solutions";
                PrevSolBtn.IsEnabled = false;
                NextSolBtn.IsEnabled = false;
            }
        }

        private void SaveMenuVoice_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSolution == null)
            {
                //No solution to save
                string messageBoxText = "No solution to save";
                string caption = "Warning";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);

                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Output"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Sudoku puzzles (.json)|*.json"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                try
                {
                    SolverCore.Loader.SaveToFile(filename, CurrentSolution);
                }
                catch (Exception Exc)
                {
                    string messageBoxText = "Unable to save to selected file.";
                    string caption = "Error";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }
            }
        }

        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            SudokuSolver.SetInput(InputSudoku);
            SudokuSolver.Solve();

            if (SudokuSolver.Solutions.Count > 0)
            {
                CurrentlyShownSolutionIx = 0;
                ShowSolution(CurrentlyShownSolutionIx);

                if (SudokuSolver.Solutions.Count > 1)
                {
                    PrevSolBtn.IsEnabled = true;
                    NextSolBtn.IsEnabled = true;
                }
            }
            else
            {
                SolutionsStatusLabel.Content = "No solutions";
                PrevSolBtn.IsEnabled = false;
                NextSolBtn.IsEnabled = false;
            }
        }

        private void PrevSolBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlyShownSolutionIx > 0)
            {
                CurrentlyShownSolutionIx--;
                ShowSolution(CurrentlyShownSolutionIx);
            }
        }

        private void NextSolBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlyShownSolutionIx < SudokuSolver.Solutions.Count - 1)
            {
                CurrentlyShownSolutionIx++;
                ShowSolution(CurrentlyShownSolutionIx);
            }
        }

        private void ShowSolution(int SolutionIx)
        {
            SolutionsStatusLabel.Content = string.Format("Solution {0}/{1}", CurrentlyShownSolutionIx + 1, SudokuSolver.Solutions.Count);
            CurrentSolution = SudokuSolver.ConvertSolution(SolutionIx);

            if (CurrentSolution.Valid == false)
            {
                //The solution is invalid. Notify user. This should never happen.
                string messageBoxText = "Solution found is invalid. The program has a bug.";
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
            SudokuViewerPane.DisplaySolution(InputSudoku.Data, CurrentSolution.Data);
        }
    }
}
