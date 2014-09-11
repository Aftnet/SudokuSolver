# SudokuSolver

by [Alberto Fustinoni](http://aftnet.net)

A sudoku solver based on Donald Knuth’s DLX algorithm. The puzzle is converted into its equivalent exact cover problem, which is what DLX actually solves, and back. The UI is created in WPF.

## Usage

The program accepts input as a json formatted text file: use File->Open to load a suitably formatted input (sample json files are provided in the archive; zeroes signify empty cells).

Use the buttons to solve the sudoku and cycle between the solutions found; File->Save saves the solution found as a json formatted text file.

## System requirements

Windows XP or later (Vista or 7 recommended), .net framework 4 or later

