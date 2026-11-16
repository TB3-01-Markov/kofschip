using Kofschip.Core.Model;

namespace Kofschip.Core.Conjugation;

/// <summary>
/// Conjugates weak (regular) Dutch verbs by rule and records which rules it used,
/// so the trainer can explain every answer. Strong verbs are not derived here:
/// their forms have to come from data.
/// </summary>
public sealed class ConjugationEngine
{
    /// <summary>
    /// The consonants of the mnemonic 't kofschip (x added, as in 't ex-kofschip).
    /// If the infinitive has one of them before -en, the past tense takes -te and the
    /// participle -t; otherwise -de and -d.
    /// </summary>
    public const string KofschipConsonants = "tkfschpx";

    public static bool IsInKofschip(char letter) =>
        KofschipConsonants.Contains(char.ToLowerInvariant(letter));

    public ConjugatedForm Conjugate(Verb verb, Tense tense, Person person, bool inversion = false)
    {
        ArgumentNullException.ThrowIfNull(verb);

        var stem = StemBuilder.Build(verb);
        return tense switch
        {
            Tense.Presens => Presens(stem, person, inversion),
            Tense.Imperfectum => Imperfectum(stem, person),
            Tense.Perfectum => Participle(verb, stem),
            _ => throw new ArgumentOutOfRangeException(nameof(tense), tense, "Unknown tense."),
        };
    }

    private static ConjugatedForm Presens(StemResult stem, Person person, bool inversion)
    {
        if (person.IsPlural())
        {
            return new ConjugatedForm(stem.Infinitive,
            [
                new(Rule.PresensPlural,
                    $"Bij wij, jullie en zij (meervoud) gebruik je de infinitief: {stem.Infinitive}."),
            ]);
        }

        var steps = stem.Steps.ToList();

        if (person == Person.Ik)
        {
            steps.Add(new(Rule.PresensStem, $"Bij ik gebruik je alleen de stam: {stem.Stem}."));
            return new ConjugatedForm(stem.Stem, steps);
        }

        if (person == Person.Jij && inversion)
        {
            steps.Add(new(Rule.PresensInversion,
                $"Jij staat achter het werkwoord: dan komt er geen t bij → {stem.Stem}."));
            return new ConjugatedForm(stem.Stem, steps);
        }

        var subject = person switch
        {
            Person.Jij => "jij",
            Person.U => "u",
            _ => "hij, zij of het",
        };

        if (stem.Stem.EndsWith('t'))
        {
            steps.Add(new(Rule.PresensNoExtraT,
                $"Bij {subject}: stam + t. De stam eindigt al op t, dus je schrijft geen extra t → {stem.Stem}."));
            return new ConjugatedForm(stem.Stem, steps);
        }

        var form = stem.Stem + "t";
        var note = stem.Stem.EndsWith('d') ? " Ook na een d schrijf je de t." : string.Empty;
        steps.Add(new(Rule.PresensStemPlusT, $"Bij {subject}: stam + t → {form}.{note}"));
        return new ConjugatedForm(form, steps);
    }

    private static ConjugatedForm Imperfectum(StemResult stem, Person person)
    {
        var steps = stem.Steps.ToList();
        var inKofschip = IsInKofschip(stem.LetterBeforeEn);
        var plural = person.IsPlural();
        var ending = (inKofschip ? "te" : "de") + (plural ? "n" : string.Empty);

        steps.Add(KofschipStep(stem, inKofschip, $"-{ending}"));

        var form = stem.Stem + ending;
        steps.Add(plural
            ? new(Rule.PastPlural, $"Meervoud: {stem.Stem} + {ending} → {form}.")
            : new(Rule.PastSingular, $"Enkelvoud: {stem.Stem} + {ending} → {form}."));

        return new ConjugatedForm(form, steps, stem.LetterBeforeEn);
    }

    private static ConjugatedForm Participle(Verb verb, StemResult stem)
    {
        var steps = stem.Steps.ToList();
        var inKofschip = IsInKofschip(stem.LetterBeforeEn);
        var ending = inKofschip ? "t" : "d";

        steps.Add(KofschipStep(stem, inKofschip, $"-{ending}"));

        var withoutGe = verb.HasInseparablePrefix ?? stem.Prefix is not null;
        var body = stem.Stem;
        var parts = new List<string>();

        if (withoutGe)
        {
            steps.Add(new(Rule.ParticipleWithoutGe, stem.Prefix is null
                ? $"{Capitalize(stem.Infinitive)} heeft een vast voorvoegsel: er komt geen ge- voor."
                : $"{Capitalize(stem.Infinitive)} begint met {stem.Prefix}-: er komt geen ge- voor."));
        }
        else
        {
            parts.Add("ge");
            if (NeedsTrema(body))
            {
                body = (body[0] == 'e' ? 'ë' : 'ï') + body[1..];
                steps.Add(new(Rule.ParticipleTrema,
                    $"Na ge- krijgt de {stem.Stem[0]} een trema, anders lees je het verkeerd: ge{body[..2]}…"));
            }
        }

        parts.Add(stem.Stem);
        var form = (withoutGe ? string.Empty : "ge") + body;

        if (stem.Stem.EndsWith(ending, StringComparison.Ordinal))
        {
            steps.Add(new(Rule.ParticipleNoExtraEnding,
                $"De stam eindigt al op {ending}: je schrijft geen extra {ending}."));
        }
        else
        {
            parts.Add(ending);
            form += ending;
        }

        steps.Add(new(Rule.Participle, $"Voltooid deelwoord: {string.Join(" + ", parts)} → {form}."));
        return new ConjugatedForm(form, steps, stem.LetterBeforeEn);
    }

    private static ExplanationStep KofschipStep(StemResult stem, bool inKofschip, string ending)
    {
        var letter = stem.LetterBeforeEn;
        var look = $"Kijk naar de letter vóór -en in {stem.Infinitive}: {letter}.";
        return inKofschip
            ? new(Rule.Kofschip, $"{look} De {letter} zit in 't kofschip → {ending}.")
            : new(Rule.NotKofschip, $"{look} De {letter} zit niet in 't kofschip → {ending}.");
    }

    /// <summary>ge + e/i would be read as one sound (gee-, gei-), so the vowel gets a trema.</summary>
    private static bool NeedsTrema(string stem) =>
        stem[0] == 'e' || (stem[0] == 'i' && stem.Length > 1 && stem[1] != 'j');

    private static string Capitalize(string word) => char.ToUpperInvariant(word[0]) + word[1..];
}
