namespace codeHappy.Business.Exceptions;

public class ProfileNotFoundExeption : Exception
{
    public ProfileNotFoundExeption() : base()
    {

    }

    public ProfileNotFoundExeption(string message) : base(message)
    {

    }


    public ProfileNotFoundExeption(string message, Exception innerException) : base(message, innerException)
    {

    }
}