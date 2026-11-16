using Kofschip.Core.Conjugation;
using Kofschip.Core.Model;

namespace Kofschip.Core.Training;

/// <summary>A sentence with a blank, plus the form that belongs in it.</summary>
public sealed class Exercise
{
    public Exercise(Sentence sentence, ConjugatedForm form)
    {
        ArgumentNullException.ThrowIfNull(sentence);
        ArgumentNullException.ThrowIfNull(form);

        Sentence = sentence;
        Form = form;
        (TextBefore, TextAfter) = sentence.SplitAtBlank();

        // A blank at the start of the sentence is shown with a capital letter.
        Answer = TextBefore.Length == 0
            ? char.ToUpperInvariant(form.Form[0]) + form.Form[1..]
            : form.Form;
    }

    public Sentence Sentence { get; }

    public ConjugatedForm Form { get; }

    public string TextBefore { get; }

    public string TextAfter { get; }

    /// <summary>The correct form as it is displayed in the sentence.</summary>
    public string Answer { get; }

    public Tense Tense => Sentence.Tense;

    public string Infinitive => Sentence.Verb.Infinitive;

    /// <summary>The complete sentence with the correct form filled in.</summary>
    public string Solution => TextBefore + Answer + TextAfter;
}
