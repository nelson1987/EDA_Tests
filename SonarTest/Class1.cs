namespace SonarTest;
public interface IOptions
{
}
public class Class1
{
    public Class1(IOptions designerOptionService)
    {
        _optionSecure = designerOptionService;
    }

    private readonly IOptions _optionSecure;
    private void Main()
    {
        try
        {
            Console.WriteLine(_optionSecure.ToString());
            int variable = 10;
            if (variable == 10)
            {
                Console.WriteLine("variable equals 10");
            }
            else
            {
                switch (variable)
                {
                    case 0:
                        Console.WriteLine("variable equals 0");
                        break;
                }
            }
        }
        catch
        {
            throw;
        }
    }

}