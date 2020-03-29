using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grid = DataStructures.Grid;
using Size = DataStructures.Size;
using PiCross;
using Vector2D = DataStructures.Vector2D;
using System.Globalization;
using Sequence = DataStructures.Sequence;
using ViewModel;


namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IPlayablePuzzle playablePuzzle;

        public MainWindow()
        {
            InitializeComponent();


            this.DataContext = new GameViewModel();


            //var puzzle = Puzzle.FromRowStrings(
            //    "xxxxx",
            //    "x...x",
            //    "x...x",
            //    "x...x",
            //    "xxxxx"
            //);

            //var facade = new PiCrossFacade();
            //playablePuzzle = facade.CreatePlayablePuzzle(puzzle);
            //picrossControl.Grid = playablePuzzle.Grid;
            //picrossControl.RowConstraints = playablePuzzle.RowConstraints;
            //picrossControl.ColumnConstraints = playablePuzzle.ColumnConstraints;

        }


       

        //public void On_click(object sender, MouseButtonEventArgs e)
        //{
        //    var rect = sender as Rectangle;
        //    var dc = rect.DataContext as IPlayablePuzzleSquare;

        //    if (e.RightButton == MouseButtonState.Pressed)
        //    {
        //        dc.Contents.Value = Square.UNKNOWN;
        //    }
        //    else if (dc.Contents.Value == Square.EMPTY)
        //    {
        //        dc.Contents.Value = Square.FILLED;
        //    }
        //    else if (dc.Contents.Value == Square.FILLED)
        //    {
        //        dc.Contents.Value = Square.EMPTY;
        //    }
        //    else
        //    {
        //        dc.Contents.Value = Square.FILLED;
        //    }

        //}

        //private void Submit(object sender, RoutedEventArgs e)
        //{
        //    if (playablePuzzle.IsSolved.Value)
        //    {
        //        solvedL.Visibility = Visibility.Visible;
        //        solvedM.Visibility = Visibility.Hidden;
        //    }
        //    else
        //    {
        //        solvedM.Visibility = Visibility.Visible;
        //        solvedL.Visibility = Visibility.Hidden;
        //    }
        //}

        //private void Draw(object sender, MouseEventArgs e)
        //{
        //    var rect = sender as Rectangle;
        //    var dc = rect.DataContext as IPlayablePuzzleSquare;

        //    if (e.RightButton == MouseButtonState.Pressed)
        //    {
        //        dc.Contents.Value = Square.UNKNOWN;
        //    }
        //    else if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        if (dc.Contents.Value == Square.EMPTY)
        //        {
        //            dc.Contents.Value = Square.FILLED;
        //        }
        //        else if (dc.Contents.Value == Square.FILLED)
        //        {
        //            dc.Contents.Value = Square.EMPTY;
        //        }
        //        else
        //        {
        //            dc.Contents.Value = Square.FILLED;
        //        }
        //    }
        //}
    }


    //public class BoolToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        bool bValue = (bool)value;

    //        if (bValue)
    //            return Visibility.Visible;
    //        else
    //            return Visibility.Hidden;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class SquarConverter : IValueConverter
    //{

    //    public object Filled { get; set; }
    //    public object Empty { get; set; }
    //    public object Unknown { get; set; }

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        var square = (Square)value;
    //        if (square == Square.EMPTY)
    //        {
    //            return Empty;
    //        }
    //        else if (square == Square.FILLED)
    //        {
    //            return Filled;
    //        } 
    //        else
    //        {
    //            return Unknown;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
