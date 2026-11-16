namespace Kofschip.Core.Model;

/// <summary>Grammatical person of the subject of a sentence.</summary>
public enum Person
{
    Ik = 0,
    Jij = 1,
    U = 2,

    /// <summary>Third person singular: hij, zij, het, or a singular noun.</summary>
    Hij = 3,
    Wij = 4,
    Jullie = 5,

    /// <summary>Third person plural: zij, or a plural noun.</summary>
    Zij = 6,
}

public static class PersonExtensions
{
    public static bool IsPlural(this Person person) =>
        person is Person.Wij or Person.Jullie or Person.Zij;
}
