namespace Diary.Domain.ValueObjects.Report;

public sealed class Name : IEquatable<Name>
{
    public Name(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        var separatedName = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        FirstName = Capitalize(separatedName[0]);
        LastName = Capitalize(string.Join(" ", separatedName.Skip(1))); //skips first name
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string FullName => string.Join(" ", FirstName, LastName).TrimEnd(); //TrimEnd() is there if LastName is null

    public bool Equals(Name other)
    {
        if (other is null) return false;

        return string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Name);
    }

    private static string Capitalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return char.ToUpper(value[0]) + value[1..];
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(FirstName),
            StringComparer.OrdinalIgnoreCase.GetHashCode(LastName)
        );
    }

    public override string ToString()
    {
        return FullName;
    }

    public static implicit operator Name(string name)
    {
        return new Name(name);
    }
}