namespace codeHappy.Business.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base()
    {

    }

    public NotFoundException(string entity, Guid id) : base($"{entity} {id} not found")
    {

    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {

    }
}
