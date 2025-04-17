namespace PatternDrafter.Library;

/// <summary>
/// Base class for pattern landmarks - to be extended by specific pattern types
/// </summary>
public abstract class PatternLandmarks
{
    // Common properties that might be needed by all patterns
    public PatternPoint Origin { get; set; }

    /// <summary>
    /// Returns all landmark points as a collection
    /// </summary>
    public abstract IEnumerable<PatternPoint> GetAllLandmarkPoints();

    /// <summary>
    /// Returns all relative points as a collection
    /// </summary>
    public abstract IEnumerable<PatternPoint> GetAllRelativePoints();
}