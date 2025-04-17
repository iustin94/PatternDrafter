using NetTopologySuite.Geometries;

namespace PatternDrafter.Library;

/// <summary>
/// Base class for all path elements in a pattern (lines, curves)
/// </summary>
public abstract class PathElement
{
    public PathType Type { get; set; }
    public bool IsVisible { get; set; } = true;
    
    public abstract Geometry ToGeometry(GeometryFactory factory);
    public abstract List<Coordinate> Discretize(int pointCount = 20);
}

public enum PathType
{
    CutLine,
    SeamLine,
    FoldLine,
    ConstructionLine,
    GrainLine,
    ButtonLine,
    HemLine
}
