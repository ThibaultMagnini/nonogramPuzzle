from .grid import Grid


EMPTY = 0
FILLED = 1
UNKNOWN = -1

def generate_combinations(width, ns):
    def aux(width, ns):
        if width >= 0:
            if ns:
                n, *ns = ns
                yield from ( [ EMPTY ] * k + [ FILLED ] * n + [ EMPTY ] + rest for k in range(0, width + 1) for rest in aux(width - n - k - 1, ns) )
            else:
                yield [ EMPTY ] * width

    return ( ns[:-1] for ns in aux(width + 1, ns) )


def compatible(bs, cs):
    def aux(b, c):
        return b == UNKNOWN or c == UNKNOWN or b == c

    return all( aux(b, c) for b, c in zip(bs, cs) )


def generate_compatible(bs, ns):
    return ( cs for cs in generate_combinations(len(bs), ns) if compatible(bs, cs) )


def merge(bs, cs):
    def aux(b, c):
        return UNKNOWN if b != c else b

    for i in range(len(bs)):
        bs[i] = aux(bs[i], cs[i])


def improve_sequence(bs, ns):
    first, *rest = list(generate_compatible(bs, ns))

    for i, x in enumerate(first):
        bs[i] = x

    for cs in rest:
        merge(bs, cs)

def improve(grid, column_constraints, row_constraints):
    def improve_column(i):
        improve_sequence(grid.column(i), column_constraints[i])

    def improve_row(i):
        improve_sequence(grid.row(i), row_constraints[i])

    def improve_columns():
        for i in range(grid.width):
            improve_column(i)

    def improve_rows():
        for i in range(grid.height):
            improve_row(i)

    improve_rows()
    improve_columns()


def solve_puzzle(column_constraints, row_constraints):
    width = len(column_constraints)
    height = len(row_constraints)
    grid = Grid(width, height, lambda x, y: UNKNOWN)

    def count_unknowns():
        return grid.count(lambda x: x == UNKNOWN)

    previous_count = -1
    current_count = count_unknowns()

    while previous_count != current_count:
        improve(grid, column_constraints, row_constraints)
        previous_count = current_count
        current_count = count_unknowns()

    return grid


def is_valid_puzzle(puzzle):
    column_constraints, row_constraints = derive_constraints(puzzle)
    solved = solve_puzzle(column_constraints=column_constraints, row_constraints=row_constraints)
    return not solved.contains(UNKNOWN)


def show(grid):
    def aux(b):
        if b == UNKNOWN:
            return '?'
        elif b == EMPTY:
            return '.'
        else:
            return 'x'

    return '\n'.join( ''.join( aux(c) for c in row ) for row in grid.rows )


def derive_constraints(grid):
    def derive(bs):
        def aux(ns, acc, bs):
            if bs:
                b, *bs = bs

                if b == EMPTY:
                    return aux( [*ns, acc], 0, bs )
                else:
                    return aux( ns, acc + 1, bs )
            else:
                return [ *ns, acc ]

        return [ x for x in aux([], 0, bs) if x != 0 ]

    column_constraints = [ derive(list(column)) for column in grid.columns ]
    row_constraints = [ derive(list(row)) for row in grid.rows ]

    return ( column_constraints, row_constraints )


def read_image_from_file(filename):
    with open(filename) as file:
        rows = [ line.strip() for line in file ]
        width = len(rows[0])
        height = len(rows)

        return Grid(width, height, lambda x, y: EMPTY if rows[y][x] == '.' else FILLED)
