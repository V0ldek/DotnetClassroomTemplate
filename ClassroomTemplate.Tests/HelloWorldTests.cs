using Xunit;

namespace ClassroomTemplate.Tests;

public class HelloWorldTests
{
    [Fact]
    public void Message_Is_HelloWorld()
    {
        Assert.Equal("Hello, World!", HelloWorld.Message);
    }
}