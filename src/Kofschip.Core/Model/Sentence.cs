namespace Kofschip.Core.Model;

/// <summary>An example sentence with one blank where a form of the verb belongs.</summary>
public class Sentence
{
    /// <summary>Marks the place of the verb form in <see cref="Text"/>.</summary>
    public const string Blank = "___";

    public int Id { get; set; }

    public int VerbId { get; set; }

    public Verb Verb { get; set; } = null!;

    public Tense Tense { get; set; }

    /// <summary>Person of the subject, which decides the verb form.</summary>
    public Person Person { get; set; }

    /// <summary>True when the subject follows the verb ("Werk jij morgen?").</summary>
    public bool Inversion { get; set; }

    /// <summary>The sentence, containing <see cref="Blank"/> exactly once.</summary>
    public required string Text { get; set; }

    public bool HasSingleBlank
    {
        get
        {
            var first = Text.IndexOf(Blank, StringComparison.Ordinal);
            return first >= 0 && Text.IndexOf(Blank, first + Blank.Length, StringComparison.Ordinal) < 0;
        }
    }

    /// <summary>Splits the sentence into the text before and after the blank.</summary>
    public (string Before, string After) SplitAtBlank()
    {
        if (!HasSingleBlank)
        {
            throw new InvalidOperationException($"Sentence must contain '{Blank}' exactly once: \"{Text}\"");
        }

        var index = Text.IndexOf(Blank, StringComparison.Ordinal);
        return (Text[..index], Text[(index + Blank.Length)..]);
    }
}
