namespace Kofschip.Core.Training;

/// <summary>The outcome of one answered exercise.</summary>
public sealed record AnswerResult(Exercise Exercise, string Given, bool IsCorrect);
