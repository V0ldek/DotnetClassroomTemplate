using Xunit;

namespace ClassroomTemplate.Tests;

public class AdderTests
{
    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(5, 0, 5)]
    [InlineData(0, 5, 5)]
    [Trait("TestGroup", "Adder")]
    public void Adder_Sum_GivesSumOfParameters(int first, int second, int expected)
    {
        var adder = new Adder(first, second);

        Assert.Equal(expected, adder.Sum);
    }
}