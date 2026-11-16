namespace Kofschip.Core.Model;

/// <summary>The tenses the trainer can ask for.</summary>
public enum Tense
{
    /// <summary>Present tense: ik werk.</summary>
    Presens = 0,

    /// <summary>Simple past: ik werkte.</summary>
    Imperfectum = 1,

    /// <summary>Present perfect; the trainer asks for the past participle: (ik heb) gewerkt.</summary>
    Perfectum = 2,
}
