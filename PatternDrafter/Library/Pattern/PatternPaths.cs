namespace PatternDrafter.Library;

/// <summary>
/// Base class for pattern paths - to be extended by specific pattern types
/// </summary>
public abstract class PatternPaths
{
    // Common properties that might be needed by all patterns

    /// <summary>
    /// Returns all landmark paths as a collection
    /// </summary>
    public abstract IEnumerable<PathElement> GetAllLandmarkPaths();

    /// <summary>
    /// Returns all relative paths as a collection
    /// </summary>
    public abstract IEnumerable<PathElement> GetAllRelativePaths();
}
