# Defense and Evaluation

Below are the rules which you have to follow
if you want to get a passing grade.

In the past, we (the lecturers) would
examine your code beforehand in search
for mistakes and confront you with them
during your defense. Once the defense
was over, the student had a good idea
about whether or not (s)he passed.

However, times have changed. Due to
the way the exams are organized,
there is very little time between the project submission deadline
and the actual defense. We will therefore
not able to take a thorough look at
your code beforehand.

A consequence of this is that a seemingly
good defense means very little: we'll still
have to take a look afterwards, and if we find
fatal mistakes, your grade might take an unforeseen
plunge.

It is our understanding that students
are not fond of this kind of uncertainty.
Neither are we. So, we took some countermeasures to alleviate
this current state of affairs.

First, we suggest you don't try to bluff
your way through the defense. If you think
you've made a mistake, or if you think
that something ought to be done differently
but you didn't implement it that way
for one reason or another (e.g. lack of time),
do mention this. The more you can show
you are aware of the shortcomings
of your project, the better.

Second, we will do our best to give you clear-cut
rules about what is okay and what isn't.
Design guidelines are often vague
and even contradictory at times,
so we don't want people to get
the impression that our demands
are subjective or whimsical.

## Rules

# MVVM

The M/VM/V layering must be respected at all cost.
The "knows-of" relation flows downwards in the diagram
below:

<center>

| Structure |
|:---:|
| V |
| VM |
| M |

</center>

* The M does not know about neither the VM or V and
must not mention anything from the layer above it.
* The VM knows the M, but does not know anything about the V.
* The V knows both the VM and M, but ideally, the V
speaks as little as possible directly with the M.

The reasoning behind this is that we want to be able to reuse code in other contexts. If the M
were to contain WPF types, it becomes impossible to reuse the M
on a platform for which WPF is not available, e.g., mobile devices.
Likewise, we want to be able to reuse the VM.

A necessary (but not sufficient) test
to check that the layering is respected,
is to verify the project references.

* The Model project should not contain a reference
to the ViewModel, View project or to WPF.
* The ViewModel can (and should) have a reference
to the model, but not to the View of WPF.
* The View can have any references you want.

References can be viewed in the Solution Explorer,
i.e., the pane which lists all your projects.
Each project is shown as a tree which contains
your files, but also has a References node under
which all references are listed.
WPF appears here as PresentationCore
and PresentationFramework. If after ensuring
the references are correct your project
stops compiling, you know you have done something
fundamentally wrong which you'll have to fix.

## View Model

The View Model must present *conceptual* information to the View.
Consider a `PersonViewModel` class which exposes
a person's gender as a `Cell<bool> IsMale`. Now say you wish to show
males as blue and females as pink.

The **wrong** way would be to place this logic in your View Model:

```csharp
// THIS IS WRONG
class PersonViewModel
{
    public PersonViewModel()
    {
        // Have GenderColor automatically synchronize with IsMale
        GenderColor = IsMale.Derive( m => m ? Brushes.Blue ? Brushes.Pink );
    }

    public Cell<bool> IsMale { get; }

    public Cell<Brush> GenderColor { get; }
}
```

The problem here is that you make use of `Brush`, which is a WPF-specific type.
Now, some people attempt to trick their way out of this rule:

```csharp
// THIS IS STILL WRONG
class PersonViewModel
{
    public PersonViewModel()
    {
        GenderColor = IsMale.Derive( m => m ? "blue" : "pink" );
    }

    public Cell<bool> IsMale { get; }

    public Cell<string> GenderColor { get; }
}
```

Even though no WPF types have been used, this code still makes
a View-related decision.

The correct way of handling this is to leave the color up to the View layer.
The View Model should only provide the information necessary to be able
to make the decision. In our case, `IsMale` is all that is necessary.

The most idiomatic way to perform the translation of `bool` to `Brush`
is relying on a converter (which resides in the View layer),
ideally a reusable one (e.g. a `BoolConverter`).

## Code Behind

The "Code-Behind" is the C# class hiding behind a XAML file,
such as `MainWindow.xaml.cs`. Since it is closely tied
to a WPF window or control, this Code-Behind can be
hard to test.

In order to improve testability,
you want to keep the Code-Behind class as empty as possible and
move all logic into separate, independent classes.
In the case of WPF, the ViewModel is generally the best place.

For example, consider the mouse click functionality. Many
controls offer a `Click` event which is signaled
whenever the user clicks the corresponding control.

```xml
<Button Click="DoSomething" />
```

However, a `Click` handling method must always reside
in the Code Behind class, which is exactly what we want to avoid.

The alternative is to rely on commands. Clickable controls
such as `MenuItem`s or `Button`s have, apart from a `Click` event,
also a `Command` property. These are set using `Binding`s:

```xml
<Button Command={Binding DoSomething} />
```

The `DoSomething` command is looked for in the `Button`'s `DataContext`,
which is normally set to a View Model class.
In other words, this allows the logic behind `DoSomething` to be placed
in the VM layer instead of the Code-Behind.

Whenever possible, you must rely on commands. If
you use a WPF control that lacks a `Command` property,
or you wish to handle different inputs (e.g. right clicks, key presses),
you can try to look for `InputBindings`.
If even that does not provide the functionality you look for,
you may make use of events and the Code-Behind. In this case,
you should absolutely keep the logic in the Code-Behind to a strict minimum
and give control to the underlying View Model as quickly as possible.

## Strings in the VM

Some students put string messages in their VM, for example `"You won!"`.
This violates MVVM principes: clearly, the message is meant
to be shown to the user, and hence it is part of the view.

Consider what would happen if you were to have to
localize your project: translators would
have to rummage through your code in search
of all strings that are exposed to the View layer.
This  is something you should avoid.

Normally, all messages should be grouped
into separate files, e.g. `french.json`, `english.json`, etc.
Every message then has its own id, and wherever in your project
you want to show some message, you would only mention its id.
The framework would then know to look up this id in the correct language file.

We do not ask of you to go this far: for this course,
it is sufficient that you put all such messages in the view, where
you are allowed to hard code them in XAML or converters.
You do not need to support multiple languages.

As mentioned earlier, the VM should only expose conceptual information.
If you want to be able to notify the user that (s)he has won,
provide a `Cell<bool> hasWon` which the view can then choose to observe.

## Music in the VM

Music, sounds, etc. are all part of the view. The VM should
not in any way make reference to such things. If you want
some sound to start playing at a certain event (e.g. puzzle is solved),
the VM should only provide a way to be notified when the puzzle is solved
(e.g. an event or a `Cell<bool>`). It is up to the view to keep
an eye on this event/cell and play the sound when needed.

## Program Initialization

Move your program initialization to the `App` class, more specifically
override its `OnStartup` method. Here you can create
your main window and set its `DataContext` to your main View Model class.

## Duplication

Try to keep duplication to a minimum. For example,
some students could have multiple commands, all of which
are always enabled. For each of these commands,
they generally repeat the same code all over again:

```csharp
public class SomeCommand : ICommand
{
    public event EventHandler CanExecuteChanged;

    public bool CanExecute( object parameter )
    {
        return true;
    }

    public void Execute( object parameter )
    {
        // Whatever
    }
}
```

Only the `Execute` method differs. Whenever you notice such a pattern
of shared code, *introduce a new class* that contains the command
code and have other classes inherit from it.

Converters often suffer the same fate. If converters look alike, again, find whatever they have in common,
generalize the class using parameters, etc. Unnecessary duplication of code
is one of the cardinal sins of software engineering. Do be critical of your own code.

## WinForms

WinForms is another Windows GUI library, i.e., an alternative to WPF.
In the past, some people have relied on WinForms classes to create their
project. This makes **no sense** whatsoever. Please **do not** combine
two GUI libraries in a single project.

Added to this, some used WinForms classes in their VM, claiming
it was ok because their VM was not dependent on WPF. While technically correct,
we do not want the VM to be dependent on *any* GUI library. In other words,
using WinForms in your VM is in clear violation of the rules.
