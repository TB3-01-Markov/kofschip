namespace Kofschip.Core.Conjugation;

/// <summary>A spelling or grammar rule the engine applied while building a form.</summary>
public enum Rule
{
    /// <summary>Infinitive minus -en.</summary>
    Stem,

    /// <summary>pakken → pak: a stem never ends in a doubled consonant.</summary>
    SingleConsonant,

    /// <summary>maken → maak: a long vowel is written double in a closed syllable.</summary>
    LongVowel,

    /// <summary>leven → leef.</summary>
    VBecomesF,

    /// <summary>reizen → reis.</summary>
    ZBecomesS,

    /// <summary>The stem comes from the data (stress-dependent stems such as probeer).</summary>
    StemFromData,

    /// <summary>The letter before -en is in 't kofschip: -te(n) / -t.</summary>
    Kofschip,

    /// <summary>The letter before -en is not in 't kofschip: -de(n) / -d.</summary>
    NotKofschip,

    PastSingular,
    PastPlural,

    /// <summary>Regular participle: ge + stem + t/d.</summary>
    Participle,

    /// <summary>betalen → betaald: no ge- after an inseparable prefix.</summary>
    ParticipleWithoutGe,

    /// <summary>praten → gepraat: no second t or d.</summary>
    ParticipleNoExtraEnding,

    /// <summary>eindigen → geëindigd.</summary>
    ParticipleTrema,

    PresensStem,
    PresensStemPlusT,

    /// <summary>praten → hij praat: no second t.</summary>
    PresensNoExtraT,

    /// <summary>"Werk jij?": no t when jij follows the verb.</summary>
    PresensInversion,

    /// <summary>Plural present tense equals the infinitive.</summary>
    PresensPlural,
}
