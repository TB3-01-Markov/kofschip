namespace Kofschip.Core.Model;

/// <summary>
/// A weak (regular) Dutch verb. Only the infinitive is required: every form is
/// derived from it by <see cref="Conjugation.ConjugationEngine"/>.
/// </summary>
public class Verb
{
    public int Id { get; set; }

    /// <summary>Infinitive in lower case, e.g. "werken".</summary>
    public required string Infinitive { get; set; }

    /// <summary>
    /// Optional stem, for verbs whose stem depends on word stress and so cannot be
    /// derived from spelling alone: proberen → probeer, but luisteren → luister.
    /// </summary>
    public string? Stem { get; set; }

    /// <summary>
    /// Whether the verb starts with an unstressed, inseparable prefix (be-, ver-, ont-, ...),
    /// which means the past participle gets no ge-. Null lets the engine detect it;
    /// set it for the exceptions (bedelen starts with "be" but has no prefix).
    /// </summary>
    public bool? HasInseparablePrefix { get; set; }

    public List<Sentence> Sentences { get; set; } = [];
}
