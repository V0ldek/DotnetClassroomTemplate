using CommandLine;
using System.Text.Json;

return Parser.Default.ParseArguments<Options>(args).MapResult(
    options =>
    {
        var project = options.EntryProjectName;
        var tests = new Test[]
        {
            new InputOutputTest(project, "", "Hello, World!")
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

        if (options.PointsTotal is not null)
        {
            var actual = tests.Sum(x => x.Points);

            if (actual != options.PointsTotal)
            {
                Console.Error.WriteLine($"Validation failed - tests' points sum up to '{actual}', expected '{options.PointsTotal}'.");
                return 1;
            }
        }

        var jsonObject = new
        {
            tests = tests.Select(t => t.ToJsonModel()).ToList()
        };

        var json = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        File.WriteAllText(options.Output, json);

        return 0;
    },
    _ => 1);

abstract record class Test(int Timeout, int Points)
{
    protected const string HomeVariableFix = "DOTNET_CLI_HOME=/home/runner";

    public abstract TestJson ToJsonModel();
}

record class DotnetTestGroup(string TestGroup) : Test(1, 1)
{
    public string Name { get; init; } = $"Unit Tests ({TestGroup})";

    private string Command => $"{HomeVariableFix} dotnet test --filter TestGroup={TestGroup}";

    public override TestJson ToJsonModel() => new TestJson(Name, "", Command, "", "", "included", Timeout, Points);
}

record class InputOutputTest(string Project, string Input, string Output) : Test(1, 1)
{
    public string Name { get; init; } = $"Console Test ({Project})";

    private string Command => $"{HomeVariableFix} dotnet run --project {Project}";

    public override TestJson ToJsonModel() => new TestJson(Name, "", Command, Input, Output, "exact", Timeout, Points);
}

record class TestJson(string Name, string Setup, string Run, string Input, string Output, string Comparison, int Timeout, int Points)
{
}

class Options
{
    [Option('e', "entry-project", Required = true, HelpText = "Name of the project that provides input/output for use with `dotnet run`.")]
    public string EntryProjectName { get; init; } = null!;

    [Option('o', "output", Required = true, HelpText = "Full path to the output JSON file. The directory path must exist.")]
    public string Output { get; init; } = null!;

    [Option('p', "points", Required = false, Default = null, HelpText = "Total number of points from all tests. If provided, the program will validate that the defined tests' points sum to this value.")]
    public int? PointsTotal { get; init; }
}