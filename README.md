# Classroom Template

Template for autograding enabled .NET projects in GitHub Classroom.

## Notes

There are three facts that you need to know to understand the workarounds here.

1. By default, the GitHub Classroom pipeline doesn't setup .NET with the `actions/setup-dotnet@v1` action and thus doesn't work for .NET 6 projects.
2. Adding or editing any tests in GitHub Classroom assignment edit page **Grading and feedback/Add autograding tests** sections overwrites the `.github/workflows` directory and the pipeline defined within.
3. When the `education/autograding@v1` action runs the defined command (e.g. `dotnet run` or `dotnet test`) it sandboxes it in an environment where only the `PATH` variable is set. .NET requires the `HOME` or `DOTNET_CLI_HOME` environment variables to point to the user's home directory to work correctly.

Because of point 1. a custom pipeline is required. Because of 2., setting up autograding tests has to be done by manually updating `.github/classroom/autograding.json`. Finally, because of 3., when updating the tests it is important to prefix every command ran with an explicit overwrite of the `DOTNET_CLI_HOME` for .NET to work properly in the sandbox.
