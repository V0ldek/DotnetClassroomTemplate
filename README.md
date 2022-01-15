# Classroom Template

Template for autograding enabled .NET projects in GitHub Classroom.

## Quickstart

Use this template as a baseline for defining your Classroom assignments. DO NOT use the Classroom GUI to add autograding tests &ndash; edit `.github/classroom/GenerateAutograding/Tests.cs` instead.

See PRs [#3 Correct implementation](https://github.com/V0ldek/ClassroomTemplate/pull/3) and [#4 Incorrect implementation](https://github.com/V0ldek/ClassroomTemplate/pull/4) to see the autograding in action.

## Implementation notes

There are three facts that you need to know to understand the workarounds here.

1. By default, the GitHub Classroom pipeline doesn't setup .NET with the `actions/setup-dotnet@v1` action and thus doesn't work for .NET 6 projects.
2. Adding or editing any tests in GitHub Classroom assignment edit page **Grading and feedback/Add autograding tests** sections overwrites the `.github/workflows` directory and the pipeline defined within.
3. When the `education/autograding@v1` action runs the defined command (e.g. `dotnet run` or `dotnet test`) it sandboxes it in an environment where only the `PATH` variable is set. .NET requires the `HOME` or `DOTNET_CLI_HOME` environment variables to point to the user's home directory to work correctly.

Because of point 1. a custom pipeline is required. Because of 2., setting up autograding tests has to be done by manually updating `.github/classroom/autograding.json`. Finally, because of 3., when updating the tests it is important to prefix every command ran with an explicit overwrite of the `DOTNET_CLI_HOME` for .NET to work properly in the sandbox.

## Example `autograding.json`

An example `autograding.json` looks like this:

```json
{
  "tests": [
    {
      "name": "Console Test (ClassroomTemplate)",
      "setup": "",
      "run": "DOTNET_CLI_HOME=/home/runner dotnet run --project ClassroomTemplate",
      "input": "",
      "output": "Hello, World!",
      "comparison": "exact",
      "timeout": 1,
      "points": 50
    },
    {
      "name": "Unit Tests (Adder)",
      "setup": "",
      "run": "DOTNET_CLI_HOME=/home/runner dotnet test --filter TestGroup=Adder",
      "input": "",
      "output": "",
      "comparison": "included",
      "timeout": 1,
      "points": 25
    },
    {
      "name": "Unit Tests (HelloWorld)",
      "setup": "",
      "run": "DOTNET_CLI_HOME=/home/runner dotnet test --filter TestGroup=HelloWorld",
      "input": "",
      "output": "",
      "comparison": "included",
      "timeout": 1,
      "points": 25
    }
  ]
}
```

For my purposes, I cannot think of any other reasonable tests than "run the program and see its output" or "run the unit tests". To enable dividing tests into groups and scoring them independently, I use a filter on the `TraitAttribute` of `xUnit`. So in this case we have a test method for `HelloWorld` marked with
```csharp
[Trait("TestGroup", "HelloWorld")]
```
and for `Adder` marked with
```csharp
[Trait("TestGroup", "Adder")]
```

You can produce such a JSON by hand and put it into the `.github/classroom` directory.

## Automating the automation

I really hate typing, especially typing JSONs. Therefore, included here is a small utility that can generate such JSON files with a simple object model in C#. Only thing one needs to do is edit the `.github/classroom/GenerateAutograding/Tests.cs` file and implement the `Tests` property. Here's the definition of the above example JSON:

```csharp
public IReadOnlyList<Test> Tests => new Test[]
{
    new InputOutputTest(_startupProject, "", "Hello, World!")
    {
        Points = 50
    },
    new DotnetTestGroup("Adder")
    {
        Points = 25
    },
    new DotnetTestGroup("HelloWorld")
    {
        Points = 25
    },
};
```

By default all tests are given `1` point and `1` minute timeout, determined by `Points` and `Timeout` properties. This utility is then invoked in the GitHub action, like so:
```
dotnet run --project ./.github/classroom/GenerateAutograding -- -e ClassroomTemplate -p 100 -o ./.github/classroom/autograding.json
```

You can change the `-e` and `-p` arguments to suit your needs. Here's the usage:

```
GenerateAutograding 1.0.0
Copyright (C) 2022 V0ldek

  -e, --entry-project    Required. Name of the project that provides input/output for use with `dotnet run`.

  -o, --output           Required. Full path to the output JSON file. The directory path must exist.

  -p, --points           Total number of points from all tests. If provided, the program will validate that the defined tests' points sum to this value.

  --help                 Display this help screen.

  --version              Display version information.
```
