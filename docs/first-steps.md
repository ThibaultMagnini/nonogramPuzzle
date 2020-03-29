# Getting started

## PiCross

PiCross puzzles are also known as *nonograms*.
First, you should familiarize yourself with the game: read the [rules](https://en.wikipedia.org/wiki/Nonogram)
and solve some puzzles online:

* https://www.nonograms.org/
* https://www.puzzle-nonograms.com/
* https://www.hanjie-star.com/

Let's introduce some terminology:

* The central part of the puzzle is called the *grid*.
* The grid is made out of *squares*.
* The numbers on the sides of the grid are called the *constraints*.
  The numbers on the left are the *row constraints*, the numbers on the top
  are the *column constraints*.

## `Puzzle` Class

The first thing we'll want to implement is a working playable puzzle, i.e. focus on creating a GUI that
allows people to play a puzzle and ignore all the other functionality the domain offers.

First, we need a puzzle to solve. A puzzle is modeled by the `Puzzle` class.
A `Puzzle` object contains all information about a puzzle. The most important members are

* the `Grid` property: this grid contains the actual solution of the puzzle. It has type
 `IGrid<bool>`, where `true` values represent filled squares and `false` empty squares.
* the properties `RowConstraints` and `ColumnConstraints` represent the constraints of the puzzle.
  The type of both of these properties is `ISequence<Constraints>`. `ISequence<T>` is quite similar to
  arrays/lists, but with some added functionality, which we won't get into right now.

A `Puzzle` object is *immutable*, which means that once created, it can never be changed.

## Creating a `Puzzle` Object

The `Puzzle` class sports multiple static factory methods which you can use to create a puzzle,
either from constraints using `Puzzle.FromConstraints` or from the solution using `Puzzle.FromRowStrings`.
For example,

```C#
var puzzle = Puzzle.FromRowStrings(
    "xxxxx",
    "x...x",
    "x...x",
    "x...x",
    "xxxxx"
)
```

creates a puzzle whose solution is a 5&times;5 square.

## Creating a Playable Puzzle

The `Puzzle` class does not provide any functionality to interact with a puzzle.
Instead, it just contains the data describing the puzzle.

* A `Puzzle` is immutable. It would be a rather absurd game if the player
  is not able to do anything.
* A `Puzzle`'s Grid contains `bool`s: either a square is filled, or it is empty.
  To solve a puzzle, we also want an "as of yet unknown" value. When
  starting to solve a puzzle, the entire grid should be filled with this `unknown` value,
  and the player then completes the puzzle by gradually marking
  certain squares as empty or filled.

The domain offers a separate class called `PlayablePuzzle`.
`PlayablePuzzle` objects offer a *modifiable* grid of type
`IGrid<IPlayablePuzzleSquare>`. Each square in this grid can contain one of *three* possible values:
`Square.UNKNOWN`, `Square.FILLED` or `Square.EMPTY`.

We encourage you to take a look through the domain code and find this `PlayablePuzzle` class. However,
you'll find that it is declared `internal`, i.e. it is private to the domain. What now?

We endeavored to minimize the number of domain classes
you can instantiate directly. The reason is that once we allow you to create
a `PlayablePuzzle` directly, this takes a lot of freedom away from us domain authors:
the `PlayablePuzzle` class cannot be renamed without breaking your code,
we cannot redesign it, etc. But, admittedly, making it `internal` makes it rather useless, as
you cannot access it at all.

A solution to this problem consists relying on interfaces. The
interface makes certain guarantees about what functionality we will provide,
so that at least you can actually intereact with it. On our side, we can do all kinds
of crazy stuff: we can have this interface implemented by any class we want, we
can spread it out over multiple classes, etc. So, using interfaces gives you promises of functionality
and gives us freedom of design. It's a win win situation!

Well, almost. Interfaces have a limitation: they cannot be instantiated.
To remedy this, we introduce factory methods, available in the `PiCrossFacade` class:

```c#
var puzzle = Puzzle.FromRowStrings(
    "xxxxx",
    "x...x",
    "x...x",
    "x...x",
    "xxxxx"
);
var facade = new PiCrossFacade();
var playablePuzzle = facade.CreatePlayablePuzzle(puzzle);
```

Here, `CreatePlayablePuzzle` wraps `puzzle` inside a `IPlayablePuzzle` object.

## Using a Playable Puzzle

Let's take a look at this `IPlayablePuzzle` interface. It contains only a few members:

* `Grid` represents the current state of the grid.
  This is what the player interacts with: (s)he's supposed to be able to
  change an arbitrary square of the grid to filled, empty (or back to unknown.)
* `ColumnConstraints` and `RowConstraints` represent the constraints.
* `IsSolved` is a `Cell<bool>`. If `true`, it means the grid, in its current state, contains
  the correct solution, i.e. the puzzle is solved.
  `false` means the opposite.

The `Grid` and both `Constraints` properties are actually upgraded version
of their `Puzzle` counterparts: they all rely heavily on `Cell`s, which
makes it easy for you to bind your GUI controls to them. They also
offer extra information such as `IsSatisfied` in `IPlayablePuzzleConstraints` and
`IPlayablePuzzleConstraintsValue`.
You should take a quick peek at `IPlayablePuzzle` and the related interfaces
to get an idea of what functionality they offer.

## Visualizing a Playable Puzzle

If there's anything you need to learn about software development, it's this: baby steps.
We're serious. Don't try to create the entire GUI in one go,
because if it doesn't work, you won't know where to look for mistakes.

Let's start with visualizing the puzzle. This is probably the most
complicated part of developing PiCross, and unfortunately, we have
little choice but to start with it. To alleviate your pain, we
have written a `PiCrossControl` for you.

### Step 1: Red Rectangles

To learn to work with it, careful experimentation is key. Let's start
with adding a `PiCrossControl` to our `MainWindow`:

```diff
  <Window x:Class="View.MainWindow"
          xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
          xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
          xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
+         xmlns:controls="clr-namespace:View.Controls"
          mc:Ignorable="d"
          Title="MainWindow" Height="350" Width="525">
      <Grid>
+         <controls:PiCrossControl>
+         </controls:PiCrossControl>
      </Grid>
  </Window>
```

`PiCrossControl` cannot magically know what to show. We need to give it
some data. In WPF, this is geneerally done using dependency properties, so
let's explore what properties `PiCrossControl` has to offer. For this, you
can either take a look at its source code or use the XAML Designer help you.

The `Grid` property allows you to tell `PiCrossControl` which grid
to draw. The property's type is `IGrid<object>`, which means
you can pass along any object you wish. This raises the question:
how can `PiCrossControl` know how to draw that object?

`SquareTemplate` seems like an interesting property: it tells `PiCrossControl` how
to draw each square in the `Grid`. It looks as if we're now ready to get
something to appear on our screens.

First, we need a `IGrid<object>`. We can make one using `Grid.Create`.

```diff
  // Using declarations
+ using Grid = DataStructures.Grid;
+ using Size = DataStructures.Size;

  namespace View
  {
      /// <summary>
      /// Interaction logic for MainWindow.xaml
      /// </summary>
      public partial class MainWindow : Window
      {
          public MainWindow()
          {
              InitializeComponent();

+             var grid = Grid.Create<string>( new Size( 5, 5 ), p => "x" );
          }
      }
  }
```

Both WPF and our code define `Grid` and `Size`. If we were to simply use
`Grid` and `Size` in our code, the compiler would not know which one
we meant. The `using` declarations at the top of the file
resolve this ambiguity: it effectly tells the compiler
that whenever you write `Grid`, you men `DataStructures.Grid`.
Idem for `Size`.

The line added to `MainWindow`'s constructor creates a 5&times;5 grid
filled with `"x"`. Understanding the second parameter
is not important, but for those interested: it's an anonymous function
that given a parameter `p` (of type `Vector2D`, which is inferred by the compiler)
returns `"x"`.

So, now we've got a 5&times;5 grid filled with `"x"`es. It's a good enough start.
We'll fill it with more interesting values later on.
Let's focus now on finding a way to pass this grid along to our `PiCrossControl`.
The easiest way to achieve this is to give the control a name:

```diff
  <Grid>
-     <controls:PiCrossControl>
+     <controls:PiCrossControl x:Name="picrossControl">
      </controls:PiCrossControl>
  </Grid>
```

and to programmatically set its `Grid` property:

```diff
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var grid = Grid.Create<string>( new Size( 5, 5 ), p => "x" );
+           picrossControl.Grid = grid;
        }
    }
```

Next, let's define a `SquareTemplate`.

```diff
  <Grid>
      <controls:PiCrossControl>
+         <controls:PiCrossControl.SquareTemplate>
+             <DataTemplate>
+                 <Rectangle Width="32" Height="32" Fill="Red" Stroke="Black" />
+             </DataTemplate>
+         </controls:PiCrossControl.SquareTemplate>
      </controls:PiCrossControl>
  </Grid>
```

Running your project should make a window appear with 5&times;5 red rectangles. Make sure
you understand why there are 25 such rectangles. Feel free to experiment a bit (e.g. change the rectangle's color
or grid's size) to verify your assumptions.

### Step 2: DataContexts

Every square is now drawn the same, i.e., as a red rectangle. For our game to be playable,
each square has to be able to adapt its looks depending on the state of the game. In the case of PiCross,
squares can have one of three states: empty, filled or unknown. The `SquareTemplate` needs
to be able to access that information and draw itself accordingly.

As with other WPF-controls relying on templates, we will rely on `DataContext`s to pass along information.
The `PiCrossControl` was given a `Grid` which right now contains nothing but `"x"`s. For each element
of the `Grid`, the `PiCrossControl` instantiates the `SquareTemplate` and sets its `DataContext` to
the corresponding element. Using bindings we can access the data stored in this `DataContext`.

Right now, we ignore the `"x"` value completely. Let's make it appear.
Instead of a `Rectangle`, we'll use a `TextBlock` whose `Text` property
is bound to the `Grid`'s corresponding value.

```diff
    <controls:PiCrossControl x:Name="picrossControl">
        <controls:PiCrossControl.SquareTemplate>
            <DataTemplate>
-               <Rectangle Width="32" Height="32" Fill="Red" Stroke="Black" />
+               <TextBlock Width="32" Height="32" Background="Red" Text="{Binding}" />
            </DataTemplate>
        </controls:PiCrossControl.SquareTemplate>
    </controls:PiCrossControl>
```

`{Binding}` means "take the value of the `DataContext` itself." Since
the `DataContext` always equals `"x"`, regardless of which square is being processed,
each `TextBlock`'s `Text` property should be set to `"x"`. You can verify this by launching
the application: a 5&times;5 grid of `x`'s should appear.

If this works, we know we have successfully accessed the `DataContext`. We can now
take the next step: make the `DataContext` different for each square.

### Step 3: Coordinates

We created our grid as follows:

```c#
var grid = Grid.Create<string>( new Size( 5, 5 ), p => "x" );
```

Instead of having each grid square be equal to `"x"`, let's have
it show the square's coordinates:

```diff
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

-           var grid = Grid.Create<string>( new Size( 5, 5 ), p => "x" );
+           var grid = Grid.Create<string>( new Size( 5, 5 ), p => p.ToString() );
            picrossControl.Grid = grid;
        }
    }
```

Run the application to verify that the `x`s have indeed been replaced by coordinates.

### Step 4: Text Squares

Let's now switch to showing an actual puzzle.

```diff
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

-           var grid = Grid.Create<string>( new Size( 5, 5 ), p => p.ToString() );
-           picrossControl.Grid = grid;

+           var puzzle = Puzzle.FromRowStrings(
+               "xxxxx",
+               "x...x",
+               "x...x",
+               "x...x",
+               "xxxxx"
+           );
+           var facade = new PiCrossFacade();
+           var playablePuzzle = facade.CreatePlayablePuzzle( puzzle );

+           picrossControl.Grid = playablePuzzle.Grid;
        }
    }
```

Let's run this to see what happens. You should see a 5&times;5 grid whose
squares contain some string starting with `PiCross`. The fact that there are 5&times;5
squares is a good sign. But where does that string come from?

`playablePuzzle.Grid` returns a grid, but what is its type?
Hovering over it makes a tooltip appear telling us
its type is `IGrid<IPlayablePuzzleSquare>`. `IPlayablePuzzleSquare`
is an interface; we'd prefer to know what the actual class is.
In order to find out, add a breakpoint on `MainWindow.MainWindow`'s last line.
Start the application in debug mode (F5). Hovering over `playablePuzzle.Grid`
should give you more detailed information: it's actually
a `PiCross.PlayablePuzzle.PlayablePuzzleSquare`!
This is probably what is being printed inside each square.
Let's check if we are correct about this.

Go dig into the domain and look for the `PlayablePuzzle` class. Within
it there should be a nested class `PlayablePuzzleSquare`. Extend it with a
`ToString()` method:

```diff
    private class PlayablePuzzleSquare : IPlayablePuzzleSquare
    {
        public PlayablePuzzleSquare( PlayablePuzzle parent, IVar<Square> contents, Vector2D position )
        {
            this.Contents = new PlayablePuzzleSquareContentsCell( parent, contents, position );
            this.Position = position;
        }

        Cell<Square> IPlayablePuzzleSquare.Contents => Contents;

        public PlayablePuzzleSquareContentsCell Contents { get; }

        public Vector2D Position { get; }

+       public override string ToString()
+       {
+           return "test!";
+       }
    }
```

Launch the application. Each square should now say `test!`.

During software development, it is important for you to fully comprehend what is happening.
Try to check your assumptions at each step, otherwise you might start building
things on shaky ground and sooner or later everything will collapse.
Don't let things "stay magical": the better students are those who are
willing to spend a couple of extra seconds getting a good grasp on what they are working with.

You can now remove the `ToString()` method, it serves little purpose.

Let's see what a `IPlayablePuzzleSquare` has to offer. Go to this interface's definition
in the domain code. You'll see it exposes two properties: `Contents` and `Position`.
The former sounds particularly interesting.

`Contents` is a `Cell<Square>`. `Cell` should sound familiar, so let's skip that
and go straight to `Square`. Read its documentation. Just like a `bool`
can only take on two different values (`true` and `false`), there are only three `Square` values:
`UNKNOWN`, `EMPTY` and `FILLED`. These are defined as static fields. This is particularly
interesting to us: depending on the square value, we can choose which color
to use to draw it with.

Let's go back to our XAML. Let's say we want filled squares to be black,
empty squares to be white and unknown squares to be gray. But as always, small steps,
so let's first try to access the square information.

Right now, our squares' look is determined by the following line of XAML:

```xml
<TextBlock Width="32" Height="32" Background="Red" Text="{Binding}" />
```

We know that its `DataContext` is an `IPlayablePuzzleSquare`, which
has a property `Contents` of type `Cell.Square`. Let's bind to that instead:


```diff
    <controls:PiCrossControl x:Name="picrossControl">
        <controls:PiCrossControl.SquareTemplate>
            <DataTemplate>
-               <TextBlock Width="32" Height="32" Background="Red" Text="{Binding}" />
+               <TextBlock Width="32" Height="32" Background="Red" Text="{Binding Contents.Value}" />
            </DataTemplate>
        </controls:PiCrossControl.SquareTemplate>
    </controls:PiCrossControl>
```

When you run the application, a grid of question marks appears. This is probably
due to the fact that a newly created `PlayablePuzzle` fills the grid with
`Square.UNKNOWN` values. Let's check this.

Go to `Square.cs` in the domain and look for question marks. You'll find that
there's a class `Unknown` whose `Symbol` property returns `'?'`.
This makes sense with our previous theory. Temporarily change it to something else, for example `@`.
If our assumption is correct, we should be greeted by a 25 `@`s.
After ensuring this is indeed the case, change it back to `?`.

Why does the `PiCrossControl` decide to show the object's `Symbol`? Does `Symbol` have a special meaning?
Look for all references to `Symbol` and you'll discover that there's simply a
`Square.ToString()` method that returns the `Symbol` as a string. As you should know,
using `ToString()` is the default way to render objects.

We'd like to make the grid more interesting by changing some squares.
But earlier, we told you that `Grid`s are immutable, which would mean
changing a square should be impossible.

Fortunately, there's a loophole: the `Grid` itself is indeed immutable,
but the *elements* of the `Grid` needn't be. As mentioned earlier,
the `Grid` contains `IPlayablePuzzleSquare` objects, which
have a `Contents` property which has type `Cell<Square>`, and
a `Cell`s contents can be modified. That's our loophole.

Be sure to understand the following nuances:

* The grid's immutability means you cannot add/remove columns or rows.
* The grid's immutability means that you cannot change which element a grid contains. In our case,
it contains `Cell`s and the grid cannot be made to refer to other `Cell` objects.
* However, the contents of the `Cell` itself can be changed. The `Grid` cannot prevent that from happening. (This ain't C++.)

This immutability is a good thing: the more everything stays the same,
the easier it is to built a GUI for it. Were you to have to deal with
resizable grids and changing cells, it would be much harder to keep
everything in working order. The `Cell`s are a necessity to implement
the game: without it, the player wouldn't be able to interact with the puzzle in any way.

In `MainWindow.MainWindow`, add the following code:

```diff
    public MainWindow()
    {
        InitializeComponent();

        var puzzle = Puzzle.FromRowStrings(
            "xxxxx",
            "x...x",
            "x...x",
            "x...x",
            "xxxxx"
        );
        var facade = new PiCrossFacade();
        var playablePuzzle = facade.CreatePlayablePuzzle( puzzle );

+       playablePuzzle.Grid[new Vector2D( 0, 0 )].Contents.Value = Square.FILLED;
+       playablePuzzle.Grid[new Vector2D( 1, 0 )].Contents.Value = Square.EMPTY;

        picrossControl.Grid = playablePuzzle.Grid;
    }
```

Make sure you understand what these two lines do. Run your application to
see if it behaves as expected.

### Step 5: Color Squares

Right now, each square's contents is shown as a string (`?`, `x` or `.`).
Let's turn this into gray, black and white, respectively.

```diff
<Window x:Class="View.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:View"
        xmlns:controls="clr-namespace:View.Controls"
        mc:Ignorable="d"
        Title="MainWindow" Height="350" Width="525">
+   <Window.Resources>
+       <local:SquareConverter x:Key="squareConverter" />
+   </Window.Resources>
    <Grid>
        <controls:PiCrossControl x:Name="picrossControl">
            <controls:PiCrossControl.SquareTemplate>
                <DataTemplate>
-                   <TextBlock Width="32" Height="32" Background="Red" Text="{Binding Contents.Value}" />
+                   <Rectangle Width="32" Height="32" Stroke="Black" Fill="{Binding Contents.Value, Converter={StaticResource squareConverter}}" />
                </DataTemplate>
            </controls:PiCrossControl.SquareTemplate>
        </controls:PiCrossControl>
    </Grid>
</Window>
```

Add the following `IValueConverter` to your code:

```diff
+   public class SquareConverter : IValueConverter
+   {
+       public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
+       {
+           var square = (Square) value;
++           if ( square == Square.EMPTY )
+           {
+               return Brushes.White;
+           }
+           else if ( square == Square.FILLED )
+           {
+               return Brushes.Black;
+           }
+           else
+           {
+               return Brushes.Gray;
+           }
+       }
+
+       public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
+       {
+           throw new NotImplementedException();
+       }
+   }
```

Run your application to make sure it works correctly.

We can improve upon this:

* Our `SquareConverter` is not reusable: it hardcodes the different colors.
* We'd prefer having all "artistic" choices to be made in the XAML.

Parameterize your `SquareConverter` as follows:

```diff
    public class SquareConverter : IValueConverter
    {
+       public object Filled { get; set; }

+       public object Empty { get; set; }

+       public object Unknown { get; set; }

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            var square = (Square) value;

            if ( square == Square.EMPTY )
            {
-               return Brushes.White;
+               return Empty;
            }
            else if ( square == Square.FILLED )
            {
-               return Brushes.Black;
+               return Filled;
            }
            else
            {
-               return Brushes.Gray;
+               return Unknown;
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
```

In the XAML:

```diff
    <Window x:Class="View.MainWindow"
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:local="clr-namespace:View"
            xmlns:controls="clr-namespace:View.Controls"
            mc:Ignorable="d"
            Title="MainWindow" Height="350" Width="525">
-       <Window.Resources>
-           <local:SquareConverter x:Key="squareConverter" />
-       </Window.Resources>
        <Grid>
            <controls:PiCrossControl x:Name="picrossControl">
                <controls:PiCrossControl.SquareTemplate>
                    <DataTemplate>
-                       <Rectangle Width="32" Height="32" Stroke="Black" Fill="{Binding Contents.Value, Converter={StaticResource squareConverter}}" />
+                       <Rectangle Width="32" Height="32" Stroke="Black">
+                           <Rectangle.Fill>
+                               <Binding Path="Contents.Value">
+                                   <Binding.Converter>
+                                       <local:SquareConverter Empty="White" Filled="Black" Unknown="Gray" />
+                                   </Binding.Converter>
+                               </Binding>
+                           </Rectangle.Fill>
+                       </Rectangle>
                    </DataTemplate>
                </controls:PiCrossControl.SquareTemplate>
            </controls:PiCrossControl>
        </Grid>
    </Window>
```

Take a good look at this new XAML code:

* We define the `Rectangle`'s `Fill` property using the property element syntax (`<Rectangle.Fill>...</Rectangle.Fill`) instead of the attribute syntax (`Fill="..."`). We trade in readability for flexibility: we are now free to assign any object we want to `Fill`.
* We create a `Binding` object that binds to `Contents.Value`, same as before.
* We specify the converter using property element syntax (no more using a static resource.)
* We parameterize the `SquareConverter` from within the XAML code: `Empty="White" Filled="Black" Unknown="Gray"`.

Run the application to check if everything still works.

### Step 6: Constraints

Without constraints, the player cannot be expected to solve the puzzle.
Fortunately for ~~us~~ you, `PiCrossControl` also provides
the necessary logic to show constraints.

Take a look at `PiCrossControl`'s code: you'll find the following
properties that will seem pertinent to the task at hand:

* `ColumnConstraints` of type `ISequence<object>`.
* `RowConstraints` of type `ISequence<object>`.
* `ColumnConstraintsTemplate` of type `DataTemplate`.
* `RowConstraintsTemplate` of type `DataTemplate`.

We'll experiment with `RowConstraints` and `RowConstraintsTemplate`, assuming that the corresponding column properties will exhibit the same behavior.

Let's start simple and just hardcode a `ISequence` object.

```diff
    public MainWindow()
    {
        InitializeComponent();

        var puzzle = Puzzle.FromRowStrings(
            "xxxxx",
            "x...x",
            "x...x",
            "x...x",
            "xxxxx"
        );
        var facade = new PiCrossFacade();
        var playablePuzzle = facade.CreatePlayablePuzzle( puzzle );

        playablePuzzle.Grid[new Vector2D( 0, 0 )].Contents.Value = Square.FILLED;
        playablePuzzle.Grid[new Vector2D( 1, 0 )].Contents.Value = Square.EMPTY;

        picrossControl.Grid = playablePuzzle.Grid;
+       picrossControl.RowConstraints = Sequence.FromItems<object>( 1, 2, 3, 4, 5 );
    }
```

In the XAML:

```diff
    <controls:PiCrossControl x:Name="picrossControl">
        <controls:PiCrossControl.SquareTemplate>
            <DataTemplate>
                <Rectangle Width="32" Height="32" Stroke="Black">
                    <Rectangle.Fill>
                        <Binding Path="Contents.Value">
                            <Binding.Converter>
                                <local:SquareConverter Empty="White" Filled="Black" Unknown="Gray" />
                            </Binding.Converter>
                        </Binding>
                    </Rectangle.Fill>
                </Rectangle>
            </DataTemplate>
        </controls:PiCrossControl.SquareTemplate>
+       <controls:PiCrossControl.RowConstraintsTemplate>
+           <DataTemplate>
+               <TextBlock Width="32" Height="32" Text="{Binding}" />
+           </DataTemplate>
+       </controls:PiCrossControl.RowConstraintsTemplate>
    </controls:PiCrossControl>
```

To the left of the grid, we see the numbers `1` to `5` appear, one per row.
The `PiCrossControl` instantiates the `RowConstraintsTemplate` once for each row.
However, for our PiCross puzzle, this isn't enough: a single row's constraints
is not simply one number, it is a series of numbers, e.g. `2 3 2`. It seems
the `PiCrossControl` does not take care of this and that we will need to do it ourselves.

Let's first upgrade our integers to lists of integers:

```diff
    public MainWindow()
    {
        InitializeComponent();

        var puzzle = Puzzle.FromRowStrings(
            "xxxxx",
            "x...x",
            "x...x",
            "x...x",
            "xxxxx"
        );
        var facade = new PiCrossFacade();
        var playablePuzzle = facade.CreatePlayablePuzzle( puzzle );

        playablePuzzle.Grid[new Vector2D( 0, 0 )].Contents.Value = Square.FILLED;
        playablePuzzle.Grid[new Vector2D( 1, 0 )].Contents.Value = Square.EMPTY;

        picrossControl.Grid = playablePuzzle.Grid;
-       picrossControl.RowConstraints = Sequence.FromItems<object>( 1, 2, 3, 4, 5 );
+       picrossControl.RowConstraints = Sequence.FromItems(
+           new int[] { 1, 2 },
+           new int[] { 4 },
+           new int[] { 1, 1 },
+           new int[] { 1, 1 },
+           new int[] { 1, 1, 1 }
+       );
    }
```

If we run the application, the numbers `1` to `5` are replaced by a rather
unhelpful `Int32[]`: this is what `int[]`'s `ToString()` method returns.
To render lists, WPF offers the `ItemsControl`, which you should already have
encountered earlier.

Make the following changes to the XAML code:

```diff
    <controls:PiCrossControl x:Name="picrossControl">
        <controls:PiCrossControl.SquareTemplate>
            <DataTemplate>
                <Rectangle Width="32" Height="32" Stroke="Black">
                    <Rectangle.Fill>
                        <Binding Path="Contents.Value">
                            <Binding.Converter>
                                <local:SquareConverter Empty="White" Filled="Black" Unknown="Gray" />
                            </Binding.Converter>
                        </Binding>
                    </Rectangle.Fill>
                </Rectangle>
            </DataTemplate>
        </controls:PiCrossControl.SquareTemplate>
        <controls:PiCrossControl.RowConstraintsTemplate>
            <DataTemplate>
-               <TextBlock Width="32" Height="32" Text="{Binding}" />
+               <ItemsControl ItemsSource="{Binding}">
+                   <ItemsControl.ItemsPanel>
+                       <ItemsPanelTemplate>
+                           <StackPanel Orientation="Horizontal" />
+                       </ItemsPanelTemplate>
+                   </ItemsControl.ItemsPanel>
+                   <ItemsControl.ItemTemplate>
+                       <DataTemplate>
+                           <TextBlock Width="32" Height="32" Text="{Binding}" />
+                       </DataTemplate>
+                   </ItemsControl.ItemTemplate>
+               </ItemsControl>
            </DataTemplate>
        </controls:PiCrossControl.RowConstraintsTemplate>
    </controls:PiCrossControl>
```

Based on this, you should be able to predict what will be shown when you run the application.

Let's now replace the hardcoded constraints by the puzzle's. `PlayablePuzzle`
has a property `RowConstraints` of type `ISequence<IPlayablePuzzleConstraints>`.

```diff
    public MainWindow()
    {
        InitializeComponent();

        var puzzle = Puzzle.FromRowStrings(
            "xxxxx",
            "x...x",
            "x...x",
            "x...x",
            "xxxxx"
        );
        var facade = new PiCrossFacade();
        var playablePuzzle = facade.CreatePlayablePuzzle( puzzle );

        playablePuzzle.Grid[new Vector2D( 0, 0 )].Contents.Value = Square.FILLED;
        playablePuzzle.Grid[new Vector2D( 1, 0 )].Contents.Value = Square.EMPTY;

        picrossControl.Grid = playablePuzzle.Grid;
-       picrossControl.RowConstraints = Sequence.FromItems(
-           new int[] { 1, 2 },
-           new int[] { 4 },
-           new int[] { 1, 1 },
-           new int[] { 1, 1 },
-           new int[] { 1, 1, 1 }
-       );
+       picrossControl.RowConstraints = playablePuzzle.RowConstraints;
    }
```


We guess there is one `IPlayablePuzzleConstraints` per row, so
we expect one `IPlayablePuzzleConstraints` object to model a list
of integers. Navigate through the code (using F12 to jump straight to a definition always
comes in handy in situations like this) and see how you can match
`IPlayablePuzzleConstraints` with your expectations.
Update the bindings in the XAML so as to make the right constraints appear:

```text
5
1 1
1 1
1 1
5
```

## Future Steps

* Start by making the column constraints work.
* Next, you'll want to make the puzzle interactive. Start simple: make a left mouse click mark the square as filled.
* Refactor your code so as to comply with MVVM.
* Add the required functionality as described [here](requirements.md).
