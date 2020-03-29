# Archives

We included a small Python script that allows you to solve puzzles
and create archives (containing puzzles and players.)

To set up a new archive containing a couple of puzzle
and a player named `player`, you can run the script `create-archive.sh`.

## Solving puzzles

```bash
# Solves a puzzle given constraints
$ ./pc.py puzzle solve '5;1,1;1,1;1,1;5' '5;1,1;1,1;1,1;5'
xxxxx
x...x
x...x
x...x
xxxxx
```

You can also store the constraints in a file. The first line contains width and height.
Next come the column constraints, then the row constraints.

```text
5 5
4
2
1 2
1
3
3
1 1
1 1
3
3
```

```bash
$ ./pc.py puzzle solve-from-file constraints.txt
..xxx
x...x
x...x
xxx..
xxx..
```

## Deriving constraints

Given a solution, you can derive the constraints as follows:

```text
.x...
xx.xx
.xxx.
.xxx.
..x..
```

```bash
$ ./pc.py puzzle constraints solution.txt
1;4;3;3;1
1;2,2;3;3;1
```

## Adding Puzzle to Archive

Adding from solution in `solution.txt`:

```bash
$ ./pc.py archive add-from-solution archive.zip author solution.txt
```

Adding from constraints:

```bash
$ ./pc.py archive add-from-constraints archive.zip author '3;1,1;3' '3;1,1;3'
```
