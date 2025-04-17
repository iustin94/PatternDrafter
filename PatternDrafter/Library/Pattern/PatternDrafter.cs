namespace PatternDrafter.Library;

/// <summary>
/// Base class for all pattern drafting algorithms
/// </summary>
public abstract class PatternDrafter
{
    protected BodyMeasurements Measurements { get; }
    protected PieceOptions Options { get; } = new();
    
    public PatternDrafter(BodyMeasurements measurements, PieceOptions options)
    {
        Measurements = measurements;
        Options = options;
    }
    
    public abstract Pattern DraftPattern();
}
