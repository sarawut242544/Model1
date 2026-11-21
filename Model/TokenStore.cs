using System.Collections.Concurrent;

public static class TokenStore
{
    class Info
    {
        public int UserId;
        public DateTime ExpireAt;
    }

    static readonly ConcurrentDictionary<string, Info> Map = new();

    public static string Issue(int userId)
    {
        var t = Guid.NewGuid().ToString("N");
        Map[t] = new Info { UserId = userId, ExpireAt = DateTime.Now.AddSeconds(3600) };
        return t;
    }

    public static int? Validate(string? header)
    {
        if (string.IsNullOrEmpty(header)) return null;
        var t = header.StartsWith("Bearer ") ? header.Substring(7) : header;
        if (!Map.TryGetValue(t, out var i)) return null;
        if (i.ExpireAt < DateTime.Now) return null;
        return i.UserId;
    }
}
