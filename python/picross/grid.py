class Grid:
    def __init__(self, width, height, initializer):
        self.__data = [ [ initializer(x, y) for x in range(width) ] for y in range(height) ]

    def __getitem__(self, position):
        x, y = position
        return self.__data[y][x]

    def __setitem__(self, position, value):
        x, y = position
        self.__data[y][x] = value

    def column(self, i):
        return Column(self, i)

    def row(self, i):
        return Row(self, i)

    @property
    def rows(self):
        return [ self.row(i) for i in range(self.height) ]

    @property
    def columns(self):
        return [ self.column(i) for i in range(self.width) ]

    @property
    def width(self):
        return len(self.__data[0])

    @property
    def height(self):
        return len(self.__data)

    def count(self, predicate):
        return len([1 for row in self.__data for elt in row if predicate(elt)])

    def contains(self, value):
        return self.count(lambda x: x == value) > 0


class Column:
    def __init__(self, parent, index):
        self.__parent = parent
        self.__index = index

    def __getitem__(self, i):
        return self.__parent[self.__index, i]

    def __setitem__(self, i, value):
        self.__parent[self.__index, i] = value

    def __len__(self):
        return self.__parent.height

    def __iter__(self):
        for i in range(len(self)):
            yield self[i]

class Row:
    def __init__(self, parent, index):
        self.__parent = parent
        self.__index = index

    def __getitem__(self, i):
        return self.__parent[i, self.__index]

    def __setitem__(self, i, value):
        self.__parent[i, self.__index] = value

    def __len__(self):
        return self.__parent.width

    def __iter__(self):
        for i in range(len(self)):
            yield self[i]