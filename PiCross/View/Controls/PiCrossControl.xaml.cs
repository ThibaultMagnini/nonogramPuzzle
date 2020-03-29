using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DataStructures;
using UIGrid = System.Windows.Controls.Grid;

namespace View.Controls
{
    public partial class PiCrossControl : UserControl
    {
        public PiCrossControl()
        {
            InitializeComponent();
        }
        
        #region Thumbnail

        public UIElement Thumbnail
        {
            get { return (UIElement) GetValue( ThumbnailProperty ); }
            set { SetValue( ThumbnailProperty, value ); }
        }

        public static readonly DependencyProperty ThumbnailProperty =
            DependencyProperty.Register( "Thumbnail", typeof( UIElement ), typeof( PiCrossControl ), new PropertyMetadata( null, (obj, args) => ((PiCrossControl) obj).OnThumbnailChanged(args) ) );

        private void OnThumbnailChanged(DependencyPropertyChangedEventArgs args)
        {
            if ( args.OldValue != null )
            {
                var oldThumbnail = (UIElement) args.OldValue;

                this.grid.Children.Remove( oldThumbnail );
            }

            if ( args.NewValue != null )
            {
                var newThumbnail = (UIElement) args.NewValue;

                System.Windows.Controls.Grid.SetColumn( newThumbnail, 0 );
                System.Windows.Controls.Grid.SetRow( newThumbnail, 0 );

                this.grid.Children.Add( newThumbnail );
            }            
        }

        #endregion

        #region SquareTemplate

        public DataTemplate SquareTemplate
        {
            get { return (DataTemplate) GetValue( SquareTemplateProperty ); }
            set { SetValue( SquareTemplateProperty, value ); }
        }

        public static readonly DependencyProperty SquareTemplateProperty =
            DependencyProperty.Register( "SquareTemplate", typeof( DataTemplate ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnSquareTemplateChanged( args ) ) );

        private void OnSquareTemplateChanged( DependencyPropertyChangedEventArgs args )
        {
            ClearChildren();
            CreateChildren();
        }

        #endregion

        #region ColumnConstraintsTemplate

        public DataTemplate ColumnConstraintsTemplate
        {
            get { return (DataTemplate) GetValue( ColumnConstraintsTemplateProperty ); }
            set { SetValue( ColumnConstraintsTemplateProperty, value ); }
        }

        public static readonly DependencyProperty ColumnConstraintsTemplateProperty =
            DependencyProperty.Register( "ColumnConstraintsTemplate", typeof( DataTemplate ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnColumnConstraintsTemplateChanged( args ) ) );

        private void OnColumnConstraintsTemplateChanged( DependencyPropertyChangedEventArgs args )
        {
            RecreateChildren();
        }

        #endregion

        #region RowConstraintsTemplate

        public DataTemplate RowConstraintsTemplate
        {
            get { return (DataTemplate) GetValue( RowConstraintsTemplateProperty ); }
            set { SetValue( RowConstraintsTemplateProperty, value ); }
        }

        public static readonly DependencyProperty RowConstraintsTemplateProperty =
            DependencyProperty.Register( "RowConstraintsTemplate", typeof( DataTemplate ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ( (PiCrossControl) obj ).OnRowConstraintsTemplateChanged( args ) ) );

        private void OnRowConstraintsTemplateChanged( DependencyPropertyChangedEventArgs args )
        {
            RecreateChildren();
        }

        #endregion

        #region PuzzleData

        public IGrid<object> Grid
        {
            get { return (IGrid<object>) GetValue( GridProperty ); }
            set { SetValue( GridProperty, value ); }
        }

        public static readonly DependencyProperty GridProperty =
            DependencyProperty.Register( "Grid", typeof( IGrid<object> ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ((PiCrossControl) obj).OnDataChanged( args ) ) );

        public ISequence<object> ColumnConstraints
        {
            get { return (ISequence<object>) GetValue( ColumnConstraintsProperty ); }
            set { SetValue( ColumnConstraintsProperty, value ); }
        }

        public static readonly DependencyProperty ColumnConstraintsProperty =
            DependencyProperty.Register( "ColumnConstraints", typeof( ISequence<object> ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ((PiCrossControl) obj).OnDataChanged( args ) ) );

        public ISequence<object> RowConstraints
        {
            get { return (ISequence<object>) GetValue( RowConstraintsProperty ); }
            set { SetValue( RowConstraintsProperty, value ); }
        }

        public static readonly DependencyProperty RowConstraintsProperty =
            DependencyProperty.Register( "RowConstraints", typeof( ISequence<object> ), typeof( PiCrossControl ), new PropertyMetadata( null, ( obj, args ) => ((PiCrossControl) obj).OnDataChanged( args ) ) );

        private void OnDataChanged( DependencyPropertyChangedEventArgs args )
        {
            RecreateAll();
        }

        #endregion

        #region Children

        private void RecreateAll()
        {
            ClearAll();
            CreateAll();
        }

        private void RecreateChildren()
        {
            ClearChildren();
            CreateChildren();
        }

        private void ClearAll()
        {
            ClearChildren();
            ClearGridLayout();
        }

        private void ClearChildren()
        {
            this.grid.Children.Clear();
        }

        private void ClearGridLayout()
        {
            ClearColumnDefinitions();
            ClearRowDefinitions();
        }

        private void ClearColumnDefinitions()
        {
            this.grid.ColumnDefinitions.Clear();
        }

        private void ClearRowDefinitions()
        {
            this.grid.RowDefinitions.Clear();
        }

        private void CreateAll()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            CreateColumnDefinitions();
            CreateRowDefinitions();
            CreateChildren();
        }

        private void RecreateColumnDefinitions()
        {
            ClearColumnDefinitions();
            CreateColumnConstraintControls();
        }

        private void RecreateRowDefinitions()
        {
            ClearRowDefinitions();
            CreateRowConstraintControls();
        }

        private void CreateColumnDefinitions()
        {
            Debug.Assert( this.grid.ColumnDefinitions.Count == 0 );

            if ( this.Grid != null )
            {
                // Add column for row constraints
                this.grid.ColumnDefinitions.Add( new ColumnDefinition() { Width = GridLength.Auto } );
                this.grid.ColumnDefinitions.Add( new ColumnDefinition() { Width = GridLength.Auto } );

                // Add column for each grid column
                for ( var i = 0; i != this.Grid.Size.Width; ++i )
                {
                    this.grid.ColumnDefinitions.Add( new ColumnDefinition() { Width = GridLength.Auto } );
                }
            }
        }

        private void CreateRowDefinitions()
        {
            Debug.Assert( this.grid.RowDefinitions.Count == 0 );

            if ( this.Grid != null )
            {
                // Add row for column constraints
                this.grid.RowDefinitions.Add( new RowDefinition() { Height = GridLength.Auto } );

                // Add row for each grid row
                for ( var i = 0; i != this.Grid.Size.Height; ++i )
                {
                    this.grid.RowDefinitions.Add( new RowDefinition() { Height = GridLength.Auto } );
                }
            }
        }

        private void CreateChildren()
        {
            Debug.Assert( this.grid.Children.Count == 0 );

            AddThumbnailChild();
            CreateSquareControls();
            CreateConstraintControls();
        }

        private void AddThumbnailChild()
        {
            if ( Thumbnail != null )
            {
                Debug.Assert( !this.grid.Children.Contains( Thumbnail ) );

                this.grid.Children.Add( this.Thumbnail );
            }
        }

        private void CreateSquareControls()
        {
            if ( this.Grid != null && SquareTemplate != null )
            {
                foreach ( var position in Grid.AllPositions )
                {
                    var gridCol = position.X + 1;
                    var gridRow = position.Y + 1;
                    var squareData = Grid[position];
                    var squareControl = (FrameworkElement) SquareTemplate.LoadContent();

                    squareControl.DataContext = squareData;
                    UIGrid.SetColumn( squareControl, gridCol );
                    UIGrid.SetRow( squareControl, gridRow );

                    this.grid.Children.Add( squareControl );
                }
            }
        }

        private void CreateConstraintControls()
        {
            CreateColumnConstraintControls();
            CreateRowConstraintControls();
        }

        private void CreateColumnConstraintControls()
        {
            if ( this.ColumnConstraints != null && ColumnConstraintsTemplate != null )
            {
                foreach ( var index in ColumnConstraints.Indices )
                {
                    var columnIndex = index + 1;
                    var columnConstraintData = ColumnConstraints[index];
                    var constraintsControl = (FrameworkElement) ColumnConstraintsTemplate.LoadContent();

                    constraintsControl.DataContext = columnConstraintData;
                    UIGrid.SetRow( constraintsControl, 0 );
                    UIGrid.SetColumn( constraintsControl, columnIndex );

                    this.grid.Children.Add( constraintsControl );
                }
            }
        }

        private void CreateRowConstraintControls()
        {
            if ( this.RowConstraints != null && RowConstraintsTemplate != null )
            {
                foreach ( var index in RowConstraints.Indices )
                {
                    var rowIndex = index + 1;
                    var rowConstraintData = RowConstraints[index];
                    var constraintsControl = (FrameworkElement) RowConstraintsTemplate.LoadContent();

                    constraintsControl.DataContext = rowConstraintData;
                    UIGrid.SetRow( constraintsControl, rowIndex );
                    UIGrid.SetColumn( constraintsControl, 0 );

                    this.grid.Children.Add( constraintsControl );
                }
            }
        }

        #endregion
    }
}
