# Overall Structure

Let's define some Visual Studio.
A *solution* is Visual Studio's name for the top-level 'workspace' in
which you will develop your application. A solution consists
of one or more *projects*. Projects are the parts out of which the whole (the solution)
is built.

While it is certainly possible to dump everything in a single project,
we prefer to split things up:

<center>

| Project | Description |
|-|-|
| `Cells` | Contains `Cell`-related classes. This code is completely independent from all other projects and can be reused to develop other applications. |
| `DataStructures` | Contains data structures relied upon by PiCross domain code. This code is completely reusable outside of the PiCross context. |
| `Domain` | Contains the PiCross logic. This is also called the *model*. |
| `Tests` | Contains tests. You can ignore this project, there's nothing useful for you here. |
| `ViewModel` | Project to host your view model classes. |
| `View` | Project to put your view classes in. |

</center>


Each project compiles to an *assembly*. You can compare this to Java `.jar` files:
it groups all code in one file. In our case, an assembly can take two forms:

* A `.dll` file for libraries.
* An `.exe` file if the assembly contains an entry point, i.e., a `Main` method.

In the case of PiCross, each project will be compiled to a
`.dll` except for `View`. In other words, in order to distribute your PiCross game,
you need to provide `Cells.dll`, `DataStructures.dll`, `Domain.dll`, `Controls.dll`,
`ViewModel.dll` and `View.exe`.

As is typical in software development, an abstraction can be divided into a visible
and hidden part. In the case of a class, `public` members are visible
and `private` members are hidden. The same applies on the level of assemblies:
you can specify what parts should be kept hidden from the world
outside of the assembly, and what parts can be made visible.

A `public` class is, as you might guess, a class that is visible
beyond the boundaries of the assembly. An `internal` class, however,
appears `public` to other classes *within* the assembly, but is invisible
to everything outside the assembly.

```c#
// Usable from other assemblies
public class Foo { ... }

// Only visible to other classes within the same assembly
internal class Bar { ... }
```

The code we give you heavily relies on `internal`: this is done to shield
you from the internal complexity. But there are always students
who are under the impression that in order to make use
of classes, one needs complete comprehension of the internals
of those classes and consequently proceed to try to understand
all given code, and are intimidated and demotivated as a result.
(Not that the given code is not that complicated &mdash;
there's just a lot of it.) These students are make their needlessly difficult:
it is perfectly possible to build a fully functional GUI using
only the exposed parts of the given code.

But if you truly want, you are of course free to make
all the changes you think you need. Remember though:
you need to be able to motivate these changes, i.e.,
they should not violate software engineering principles.

## Bugs in domain code

It is of course always possible there are bugs in the domain code.
If you think this is the case, please tell us so
that we can update the domain in our own repository, thereby
allowing everyone to pull the fixes in.
