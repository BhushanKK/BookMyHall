namespace BookMyHall.Persistence.Exceptions;

public sealed class DuplicateRecordException : Exception
{
    public DuplicateRecordException(string? constraintName = null)
        : base("Duplicate record found.")
    {
        ConstraintName = constraintName;
    }

    public string? ConstraintName { get; }
}