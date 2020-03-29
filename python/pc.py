#!/usr/bin/env python

from functools import reduce
import argparse
from zipfile import ZipFile
import sys
import os
import re
from picross.grid import Grid
from picross.solver import show, solve_puzzle, derive_constraints, read_image_from_file, is_valid_puzzle


def _parse_constraints(string):
    return [ [ int(n) for n in part.split(',') ] if part else [] for part in string.split(';') ]


def _solve(args):
    row_constraints = _parse_constraints(args.row)
    column_constraints = _parse_constraints(args.column)
    print(show(solve_puzzle(column_constraints, row_constraints)))

def _solve_file(args):
    filename = args.filename

    with open(filename, 'r') as file:
        width, height = map(int, file.readline().split(' '))
        column_constraints = [ [ int(n) for n in file.readline().split(' ') ] for _ in range(width) ]
        row_constraints = [ [ int(n) for n in file.readline().split(' ') ] for _ in range(height) ]

    print(show(solve_puzzle(column_constraints, row_constraints)))


def _derive_constraints(args):
    filename = args.filename
    grid = read_image_from_file(filename)
    column_constraints, row_constraints = derive_constraints(grid)
    print(';'.join( ','.join( str(n) for n in ns ) for ns in column_constraints ))
    print(';'.join( ','.join( str(n) for n in ns ) for ns in row_constraints ))


def _show(args):
    with ZipFile(args.filename, 'r') as zip:
        names = zip.namelist()
        players = [ name for name in names if name.startswith('players/')]
        puzzles = [ name for name in names if name.startswith('library/')]

        for player in players:
            player_name = player.split('/')[1].split('.')[0]
            print(f'Player {player_name}')

        for puzzle in puzzles:
            lines = zip.read(puzzle).decode('utf-8').strip().split("\n")
            author = lines[0]
            width, height = map(int, lines[1].split(' '))
            solution = "\n".join(lines[2:])
            print(f'Puzzle ({width}x{height}) by {author}')
            if args.show_solution:
                print(solution)
                print()


def _create(args):
    path = args.filename
    overwrite = args.force

    if os.path.exists(path):
        if overwrite:
            os.remove(path)
        else:
            print(f'Error: {path} already exists; use -f to force')
            sys.exit(-1)

    with ZipFile(path, 'w') as zip:
        pass

    print(f'Created {path}')


def _add_player(args):
    name = args.name
    archive = args.archive
    filename = f'players/{name}.txt'
    data = '0'

    with ZipFile(archive, 'a') as zip:
        zip.writestr(filename, data)

    print(f'Added player {args.name}')


def _extract_puzzle_index(filename):
    match = re.search(r'entry(\d+)\.txt', filename)
    return int(match.group(1))


def _add_puzzle(archive, author, solution, verify=True):
    if verify:
        print('Verifying puzzle... (can take a couple of seconds)')
        if not is_valid_puzzle(solution):
            print(f'{show(solution)}\n\nis not a valid puzzle')
            sys.exit(-1)
        else:
            print('Puzzle deemed valid!')

    width = solution.width
    height = solution.height

    with ZipFile(archive, 'r') as zip:
        names = zip.namelist()

    next_index = max([-1] + [ _extract_puzzle_index(name) for name in names if name.startswith('library/') ]) + 1
    filename = f'library/entry{str(next_index).rjust(5, "0")}.txt'
    data = f'{author}\n{width} {height}\n{show(solution)}'

    with ZipFile(archive, 'a') as zip:
        zip.writestr(filename, data)

    print(f'Added {filename} to archive')


def _add_puzzle_from_constraints(args):
    archive = args.archive
    author = args.author
    row_constraints = _parse_constraints(args.row_constraints)
    column_constraints = _parse_constraints(args.column_constraints)
    solution = solve_puzzle(row_constraints=row_constraints, column_constraints=column_constraints)
    _add_puzzle(archive, author, solution)


def _add_puzzle_from_solution(args):
    archive = args.archive
    author = args.author
    solution_file = args.solution_file
    verify = args.verify
    solution = read_image_from_file(solution_file)
    _add_puzzle(archive, author, solution, verify=verify)


def _process_command_line_arguments():
    def create_archive_parsers(subparsers):
        subparser = subparsers.add_parser('create', help='create empty archive')
        subparser.add_argument('filename', help='archive', action='store')
        subparser.add_argument('-f', '--force', help='overwrite existing archive', action='store_true')
        subparser.set_defaults(func=_create)

        subparser = subparsers.add_parser('show', help='show contents of archive')
        subparser.add_argument('filename', help='archive', action='store')
        subparser.add_argument('--show-solution', help='show solution', action='store_true')
        subparser.set_defaults(func=_show)

        subparser = subparsers.add_parser('add-from-solution', help='adds puzzle to archive; puzzle specified by solution')
        subparser.add_argument('archive', help='archive', action='store')
        subparser.add_argument('author', help='author', action='store')
        subparser.add_argument('solution_file', help='file containing solution', action='store')
        subparser.add_argument('--no-verify', help='do not check puzzle validity', action='store_false', dest='verify', default=True)
        subparser.set_defaults(func=_add_puzzle_from_solution)

        subparser = subparsers.add_parser('add-from-constraints', help='adds puzzle to archive; puzzle specified by constraints')
        subparser.add_argument('archive', help='archive', action='store')
        subparser.add_argument('author', help='author', action='store')
        subparser.add_argument('row_constraints', help='row constraints (use ; to separate rows and , to separate values)', action='store')
        subparser.add_argument('column_constraints', help='column constraints (use ; to separate columns and , to separate values', action='store')
        subparser.set_defaults(func=_add_puzzle_from_constraints)

        subparser = subparsers.add_parser('add-player', help='adds player to archive')
        subparser.add_argument('archive', help='archive', action='store')
        subparser.add_argument('name', help='name', action='store')
        subparser.set_defaults(func=_add_player)

    def create_puzzle_parsers(subparsers):
        subparser = subparsers.add_parser('solve', help='solves PiCross puzzle given its constraints')
        subparser.add_argument('row', help='row constraints (use ; to separate rows and , to separate values)', action='store')
        subparser.add_argument('column', help='column constraints (use ; to separate columns and , to separate values', action='store')
        subparser.set_defaults(func=_solve)

        subparser = subparsers.add_parser('solve-from-file', help='solves PiCross puzzle given its constraints')
        subparser.add_argument('filename', help='file containing constraints', action='store')
        subparser.set_defaults(func=_solve_file)

        subparser = subparsers.add_parser('constraints', help='derives constraints from image')
        subparser.add_argument('filename', help='file containing image', action='store')
        subparser.set_defaults(func=_derive_constraints)


    parser = argparse.ArgumentParser(prog='picross')
    parser.set_defaults(func=lambda args: parser.print_help())
    subparsers = parser.add_subparsers(help='sub-command help')

    subparser = subparsers.add_parser('archive', help='archive-related functionality')
    create_archive_parsers(subparser.add_subparsers())

    subparser = subparsers.add_parser('puzzle', help='puzzle-related functionality')
    create_puzzle_parsers(subparser.add_subparsers())

    args = parser.parse_args()
    args.func(args)


_process_command_line_arguments()