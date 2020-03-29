# Addenda

This page will contain some extra information that might help you out.

## Creating a grid

The M contains an `IGrid<IPlayablePuzzleSquare>`.
In the VM, you might want to make a corresponding `IGrid<SquareViewModel>`,
which you can then pass along to the `PiCrossControl`.

The easiest way to create his new grid is

```csharp
var vmGrid = playablePuzzle.Grid.Map( square => new SquareViewModel(square) ).Copy();
```

The `Map` operation creates a new grid of the same dimensions,
but where every element is a "transformation" of the element
at the corresponding position in `vmGrid`. In the code above,
each square in `playablePuzzle.Grid` is wrapped inside a
`SquareViewModel`. All these wrapper objects are then
put in a new grid.

The `Copy()` is not strictly necessary, but depending
on how you write your code, it might prevent some nasty surprises from happening.