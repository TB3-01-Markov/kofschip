namespace Kofschip.Core.Conjugation;

/// <summary>One step of the explanation shown to the learner (in Dutch).</summary>
public sealed record ExplanationStep(Rule Rule, string Text);

/// <summary>A verb form together with the rules that produced it.</summary>
/// <param name="Form">The conjugated form in lower case.</param>
/// <param name="Steps">The explanation, in the order the rules were applied.</param>
/// <param name="KofschipLetter">
/// The letter that was checked against 't kofschip, or null when the form does not depend on it.
/// </param>
public sealed record ConjugatedForm(string Form, IReadOnlyList<ExplanationStep> Steps, char? KofschipLetter = null);
