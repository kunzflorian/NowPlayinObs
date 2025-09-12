namespace NowPlayinObs.Domain;

public class TrackInfo : IEquatable<TrackInfo>
{
    public string Status { get; set; } = "not live";
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;

    public bool Equals(TrackInfo? other)
    {
        var equal = (this.Status == other?.Status &&
                    this.Title == other?.Title &&
                    this.Artist == other?.Artist);
        return equal;
    }

    public override string ToString()
    {
        return $"[{Status}] {Title} - {Artist}";
    }
}
