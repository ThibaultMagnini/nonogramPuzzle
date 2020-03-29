using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grid = DataStructures.Grid;
using Size = DataStructures.Size;
using PiCross;
using Vector2D = DataStructures.Vector2D;
using System.Globalization;
using Sequence = DataStructures.Sequence;
using Cells;
using DataStructures;
using System.Windows.Data;

namespace ViewModel
{
    public class GameViewModel
    {
        private IPlayablePuzzle PlayablePuzzle;

        public GameViewModel()
        {

            var puzzle = Puzzle.FromRowStrings(
                "xxxxx",
                "x...x",
                "x...x",
                "x...x",
                "xxxxx"
            );

            var facade = new PiCrossFacade();
            this.PlayablePuzzle = facade.CreatePlayablePuzzle(puzzle);
            Console.WriteLine(PlayablePuzzle.RowConstraints.Items);

            //var vmGrid = this.PlayablePuzzle.Grid.Map(square => new SquareViewModel(square)).Copy();
        }

        public string Test => "test";

        public IGrid<IPlayablePuzzleSquare> Grid => PlayablePuzzle.Grid;
        public ISequence<IPlayablePuzzleConstraints> RowConstraints => this.PlayablePuzzle.RowConstraints;
        public ISequence<IPlayablePuzzleConstraints> ColumnConstraints => this.PlayablePuzzle.ColumnConstraints;

       
    }




    /*public void On_click(object sender, MouseButtonEventArgs e)
    {
        var rect = sender as Rectangle;
        var dc = rect.DataContext as IPlayablePuzzleSquare;

        if (e.RightButton == MouseButtonState.Pressed)
        {
            dc.Contents.Value = Square.UNKNOWN;
        }
        else if (dc.Contents.Value == Square.EMPTY)
        {
            dc.Contents.Value = Square.FILLED;
        }
        else if (dc.Contents.Value == Square.FILLED)
        {
            dc.Contents.Value = Square.EMPTY;
        }
        else
        {
            dc.Contents.Value = Square.FILLED;
        }

    }

    private void Submit(object sender, RoutedEventArgs e)
    {
        if (playablePuzzle.IsSolved.Value)
        {
            solvedL.Visibility = Visibility.Visible;
            solvedM.Visibility = Visibility.Hidden;
        }
        else
        {
            solvedM.Visibility = Visibility.Visible;
            solvedL.Visibility = Visibility.Hidden;
        }
    }

    private void Draw(object sender, MouseEventArgs e)
    {
        var rect = sender as Rectangle;
        var dc = rect.DataContext as IPlayablePuzzleSquare;

        if (e.RightButton == MouseButtonState.Pressed)
        {
            dc.Contents.Value = Square.UNKNOWN;
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (dc.Contents.Value == Square.EMPTY)
            {
                dc.Contents.Value = Square.FILLED;
            }
            else if (dc.Contents.Value == Square.FILLED)
            {
                dc.Contents.Value = Square.EMPTY;
            }
            else
            {
                dc.Contents.Value = Square.FILLED;
            }
        }
    }*/

    public class SquareConverter : IValueConverter
    {

        public object Filled { get; set; }
        public object Empty { get; set; }
        public object Unknown { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var square = (Square)value;
            if (square == Square.EMPTY)
            {
                return Empty;
            }
            else if (square == Square.FILLED)
            {
                return Filled;
            }
            else
            {
                return Unknown;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}