using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Web.Services;

/// <summary>Keeps the learner's results per tense in the browser.</summary>
public sealed class ProgressStore(BrowserStorage storage)
{
    private const string Key = "kofschip.progress";

    private Progress? _progress;

    public async Task<Progress> LoadAsync() =>
        _progress ??= await storage.GetAsync<Progress>(Key) ?? new Progress();

    public async Task RecordAsync(Tense tense, bool correct)
    {
        var progress = await LoadAsync();
        progress.Record(tense, correct);
        await storage.SetAsync(Key, progress);
    }

    public async Task ResetAsync()
    {
        _progress = new Progress();
        await storage.RemoveAsync(Key);
    }
}
