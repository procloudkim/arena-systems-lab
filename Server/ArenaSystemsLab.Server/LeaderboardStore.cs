namespace ArenaSystemsLab.Server;

public sealed record ScoreEntry(string PlayerId, int Score);

public sealed class LeaderboardStore
{
    public const int MaxEntries = 10_000;

    // ponytail: One small in-memory table uses one lock; split storage only after measured contention.
    private readonly object sync = new();
    private readonly Dictionary<string, int> scores = new(StringComparer.Ordinal);
    private readonly int capacity;

    public LeaderboardStore(int capacity = MaxEntries)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        this.capacity = capacity;
    }

    public int SubmitScore(string playerId, int score)
    {
        lock (sync)
        {
            if (!scores.TryGetValue(playerId, out int current))
            {
                if (scores.Count >= capacity)
                {
                    throw new ProtocolException("leaderboard_capacity_reached");
                }

                scores[playerId] = score;
                return score;
            }

            if (score > current)
            {
                scores[playerId] = score;
                return score;
            }

            return current;
        }
    }

    public ScoreEntry[] GetTop(int limit)
    {
        lock (sync)
        {
            return scores
                .Select(pair => new ScoreEntry(pair.Key, pair.Value))
                .OrderByDescending(entry => entry.Score)
                .ThenBy(entry => entry.PlayerId, StringComparer.Ordinal)
                .Take(limit)
                .ToArray();
        }
    }
}
