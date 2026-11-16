using Kofschip.Core.Model;
using Kofschip.Core.Training;

namespace Kofschip.Web.Services;

/// <summary>Remembers the last chosen tenses and session length in the browser.</summary>
public sealed class SettingsStore(BrowserStorage storage)
{
    private const string Key = "kofschip.settings";

    public async Task<TrainingSettings> LoadAsync()
    {
        var stored = await storage.GetAsync<StoredSettings>(Key);
        return stored?.Tenses is null
            ? TrainingSettings.Default
            : new TrainingSettings(stored.Tenses, stored.Length);
    }

    public Task SaveAsync(TrainingSettings settings) =>
        storage.SetAsync(Key, new StoredSettings(settings.Tenses.ToList(), settings.Length));

    private sealed record StoredSettings(List<Tense>? Tenses, int Length);
}
