using System.Text.Json;
using CommandLine;
using GenerateAutograding;

return Parser.Default.ParseArguments<Options>(args).MapResult(
    options =>
    {
        var tests = new TestSet(options.EntryProjectName).Tests;

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

class Options
{
    [Option('e', "entry-project", Required = true, HelpText = "Name of the project that provides input/output for use with `dotnet run`.")]
    public string EntryProjectName { get; init; } = null!;

    [Option('o', "output", Required = true, HelpText = "Full path to the output JSON file. The directory path must exist.")]
    public string Output { get; init; } = null!;

    [Option('p', "points", Required = false, Default = null, HelpText = "Total number of points from all tests. If provided, the program will validate that the defined tests' points sum to this value.")]
    public int? PointsTotal { get; init; }
}