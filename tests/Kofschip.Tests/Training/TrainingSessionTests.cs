using Kofschip.Core.Training;

namespace Kofschip.Tests.Training;

public class TrainingSessionTests
{
    [Fact]
    public void New_session_starts_at_the_first_exercise()
    {
        var session = new TrainingSession(TestData.ThreeExercises());

        Assert.Equal(3, session.Total);
        Assert.Equal(1, session.Position);
        Assert.False(session.IsAnswered);
        Assert.False(session.IsFinished);
        Assert.Null(session.CurrentResult);
        Assert.Equal("werkte", session.Current.Answer);
    }

    [Fact]
    public void Session_without_exercises_is_not_allowed() =>
        Assert.Throws<ArgumentException>(() => new TrainingSession([]));

    [Fact]
    public void Submit_checks_the_answer_and_keeps_the_result()
    {
        var session = new TrainingSession(TestData.ThreeExercises());

        var result = session.Submit(" Werkte ");

        Assert.True(result.IsCorrect);
        Assert.Equal("Werkte", result.Given);
        Assert.True(session.IsAnswered);
        Assert.Same(result, session.CurrentResult);
        Assert.Equal(1, session.Position);
    }

    [Fact]
    public void An_exercise_can_be_answered_only_once()
    {
        var session = new TrainingSession(TestData.ThreeExercises());
        session.Submit("werkde");

        Assert.Throws<InvalidOperationException>(() => session.Submit("werkte"));
    }

    [Fact]
    public void Next_requires_an_answer_first()
    {
        var session = new TrainingSession(TestData.ThreeExercises());

        Assert.Throws<InvalidOperationException>(session.Next);
    }

    [Fact]
    public void Full_run_counts_correct_answers_and_mistakes()
    {
        var session = new TrainingSession(TestData.ThreeExercises());

        session.Submit("werkte");
        session.Next();
        Assert.Equal(2, session.Position);

        session.Submit("woonte");
        session.Next();
        session.Submit(null);
        session.Next();

        Assert.True(session.IsFinished);
        Assert.Equal(3, session.Position);
        Assert.Equal(1, session.CorrectCount);
        Assert.Equal(33, session.Percentage);
        Assert.Equal(2, session.Mistakes.Count);
        Assert.Equal("woonte", session.Mistakes[0].Given);
        Assert.Equal(string.Empty, session.Mistakes[1].Given);
        Assert.Throws<InvalidOperationException>(() => session.Current);
        Assert.Throws<InvalidOperationException>(() => session.Submit("x"));
    }

    [Fact]
    public void Retry_contains_only_the_mistakes()
    {
        var session = new TrainingSession(TestData.ThreeExercises());
        session.Submit("werkte");
        session.Next();
        session.Submit("woonte");
        session.Next();
        session.Submit("gemaakt");
        session.Next();

        var retry = session.CreateRetryOfMistakes();

        Assert.Equal(1, retry.Total);
        Assert.Equal("woonde", retry.Current.Answer);
        Assert.Empty(retry.Results);
    }

    [Fact]
    public void Retry_without_mistakes_is_not_allowed()
    {
        var session = new TrainingSession(TestData.ThreeExercises().Take(1));
        session.Submit("werkte");
        session.Next();

        Assert.Throws<InvalidOperationException>(() => session.CreateRetryOfMistakes());
    }

    [Theory]
    [InlineData(3, 3, 100)]
    [InlineData(2, 3, 67)]
    [InlineData(0, 3, 0)]
    public void Percentage_is_rounded(int correct, int total, int expected)
    {
        var exercises = Enumerable.Range(0, total)
            .Select(_ => TestData.ThreeExercises()[0]);
        var session = new TrainingSession(exercises);

        for (var i = 0; i < total; i++)
        {
            session.Submit(i < correct ? "werkte" : "fout");
            session.Next();
        }

        Assert.Equal(expected, session.Percentage);
    }
}
