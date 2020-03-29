# Cookbook

This page contains some short samples of how you can interact with the domain.

## `IGameData`

```C#
// Creating a dummy `IGameData` object
var facade = new PiCrossFacade();
var gameData = facade.CreateDummyGameData();

// Reading an `IGameData` object from file, fails if file does not exist
var facade = new PiCrossFacade();
var gameData = facade.LoadGameData(path);

// Reading an `IGameData` object from file, creates new file if no file is present at path
var facade = new PiCrossFacade();
var gameData = facade.LoadGameData( path, createIfNotExistent: true );
```

## Puzzle Library

```C#
// Creating a Puzzle object from constraints for
//   .....
//   .x...
//   .xx..
//   x.xx.
//   ..xxx
var puzzle = Puzzle.FromConstraints(columnConstraints: "1;2;3;2;1",
                                    rowConstraints: ";1;2;1 2;3");

// Creating a Puzzle object from solution
var puzzle = Puzzle.FromRowStrings(
        ".....",
        ".x...",
        ".xx..",
        "x.xx.",
        "..xxx"
    );

// Getting a list of puzzle entries in the puzzle library
var puzzleEntries = gameData.PuzzleLibrary.Entries;

// Adding a new puzzle to the Puzzle Library
var puzzleEntry = gameData.PuzzleLibrary.Create(puzzle, author);
```

## Playing

```C#
// Creating an IPlayablePuzzle
var facade = new PiCrossFacade();
var puzzle = GetHoldOfAPuzzleSomehow();
var playablePuzzle = facade.CreatePlayablePuzzle(puzzle);

// Marking the upper left square as filled
var position = new Vector(0, 0);
var square = playablePuzzle.Grid[position];
square.Contents.Value = Square.FILLED; // square.Contents is a Cell and therefore observable

// Checking if puzzle is solved (observable)
if ( playablePuzzle.IsSolved.Value ) { ... }

// Checking if there are cells marked unknown left (observable)
if ( playablePuzzle.ContainsUnknowns.Value ) { ... }

// Number of cells marked unknown left (observable)
var count = playablePuzzle.UnknownCount.Value;

// Checking if upper row constraints are satisfied (observable)
if ( playablePuzzle.RowConstraints[0].IsSatisfied.Value ) { ... }

// Getting the values in the upper row constraints
var values = playablePuzzle.RowConstraints[0].Values;
```

## Player Database

```C#
// Get player names
var facade = new PiCrossFacade();
var playerNames = facade.PlayerDatabase.PlayerNames;

// Get player profile
var playerProfile = facade.PlayerDatabase[playerName];

// Find out if player has played puzzle before
var puzzleEntry = facade.PuzzleLibrary.Entries.First();
if ( playerProfile[puzzleEntry].BestTime.HasValue ) { ... }

// Get player's best time
if ( playerProfile[puzzleEntry].BestTime.Value ) { ... }
```