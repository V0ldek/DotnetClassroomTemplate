namespace GenerateAutograding;

internal sealed class TestSet
{
    private readonly string _startupProject;

    public TestSet(string startupProject) => _startupProject = startupProject;

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
}
