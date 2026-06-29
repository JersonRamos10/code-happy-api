namespace codeHappy.Business.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("No tienes permiso para realizar esta acción.") { }
}
