using Integrity.Core;

public class Program
{
    public static void Main(string[] args)
    {
        Engine engine = new Engine(new Game());
        engine.Run();
    }
}