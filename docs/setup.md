# Setup

## Creating Your Very Own Fork

Do **not** clone this repository directly. Instead, rely on GitHub Classroom to
create a fork. A link should have been made available elsewhere
(presumably Toledo). If you can't find it, ask a lecturer.

Clone your forked repo using the following command, replacing `<<URL>>` by the url of your fork
(omit the `<` and `>`).

```bash
$ git clone <<URL>> picross
```

**Do not dare downloading the code as a zip.**
Also, do not clone your repository into a DropBox/OneDrive/Google Drive managed directory.

## Receiving Updates

In order to receive updates, you need to execute the shell code below in your repo directory once:

```bash
$ git remote add upstream https://github.com/UCLeuvenLimburg/picross-student
```

When updates are available, you can pull them as follows
(note that you should first commit/stash all your changes, otherwise git might complain):

```bash
$ git pull upstream master
```

In order to get e-mails whenever updates are made available, you can
register as a watcher: on the [main repository's website](https://github.com/UCLeuvenLimburg/picross-student),
click the Watch button.

## Opening in Visual Studio

Open the solution `PiCross.sln`. In the Solution Explorer,
right click View and choose Set as StartUp Project.
