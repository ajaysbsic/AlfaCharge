namespace AlfaCharge.Admin.Services;

/// <summary>
/// In-memory admin UI preferences for the current user circuit.
/// </summary>
public sealed class AdminPreferencesService
{
    public int SessionsAutoRefreshSeconds { get; private set; } = 15;
    public bool CompactTables { get; private set; }

    public void Update(int sessionsAutoRefreshSeconds, bool compactTables)
    {
        SessionsAutoRefreshSeconds = Math.Clamp(sessionsAutoRefreshSeconds, 5, 120);
        CompactTables = compactTables;
    }
}
