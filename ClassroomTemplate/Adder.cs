namespace ClassroomTemplate;

public record class Adder(int First, int Second)
{
    public int Sum => First + Second;
}
