namespace PatternDrafter.Library;

/// <summary>
/// Base class for features that can be added to pattern pieces (pockets, collars, etc.)
/// </summary>
public abstract class PatternFeature
{
    protected PieceOptions Options { get; }

    public PatternFeature(PieceOptions options)
    {
        Options = options;
    }

    public abstract IEnumerable<PatternPiece> Draft();
}
