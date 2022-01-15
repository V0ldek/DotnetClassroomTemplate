namespace GenerateAutograding;
internal abstract record class Test(int Timeout, int Points)
{
    protected const string HomeVariableFix = "DOTNET_CLI_HOME=/home/runner";

    public abstract TestJson ToJsonModel();
}

internal record class DotnetTestGroup(string TestGroup) : Test(1, 1)
{
    public string Name { get; init; } = $"Unit Tests ({TestGroup})";

    private string Command => $"{HomeVariableFix} dotnet test --filter TestGroup={TestGroup}";

    public override TestJson ToJsonModel() => new TestJson(Name, "", Command, "", "", "included", Timeout, Points);
}

internal record class InputOutputTest(string Project, string Input, string Output) : Test(1, 1)
{
    public string Name { get; init; } = $"Console Test ({Project})";

    private string Command => $"{HomeVariableFix} dotnet run --project {Project}";

    public override TestJson ToJsonModel() => new TestJson(Name, "", Command, Input, Output, "exact", Timeout, Points);
}

internal record class TestJson(string Name, string Setup, string Run, string Input, string Output, string Comparison, int Timeout, int Points)
{
}