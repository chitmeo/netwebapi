namespace Dev.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, Guid id) : base($"Entity {entityName} not found with id: {id}")
    {
    }
}
