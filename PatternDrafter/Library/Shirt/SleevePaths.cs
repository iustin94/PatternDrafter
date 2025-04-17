namespace PatternDrafter.Library;

public class SleevePaths : PatternPaths
{
    // Construction lines
    public PatternLine HalfScyeDepthLine { get; set; }
    public PatternLine SleeveLengthLine { get; set; }
    public PatternLine CapWidthLine { get; set; }
    
    // Main cutting lines
    public PatternLine CenterLine { get; set; }
    public PatternCurve BackCapCurve { get; set; }
    public PatternCurve FrontCapCurve { get; set; }
    public PatternLine UnderarmToWristLine { get; set; }    // For short sleeve
    public PatternCurve UnderarmToWristCurve { get; set; }  // For long sleeve
    public PatternLine WristLine { get; set; }
    
    // Additional feature lines
    public PatternLine GrainLine { get; set; }
    public PatternLine CuffFoldLine { get; set; }
    public PatternLine HemFoldLine { get; set; }
    
    public override IEnumerable<PathElement> GetAllLandmarkPaths()
    {
        var paths = new List<PathElement>();
        
        // Add main cutting lines
        if (CenterLine != null) paths.Add(CenterLine);
        if (BackCapCurve != null) paths.Add(BackCapCurve);
        if (FrontCapCurve != null) paths.Add(FrontCapCurve);
        if (UnderarmToWristLine != null) paths.Add(UnderarmToWristLine);
        if (UnderarmToWristCurve != null) paths.Add(UnderarmToWristCurve);
        if (WristLine != null) paths.Add(WristLine);
        
        return paths;
    }
    
    public override IEnumerable<PathElement> GetAllRelativePaths()
    {
        var paths = new List<PathElement>();
        
        // Add construction and feature lines
        if (HalfScyeDepthLine != null) paths.Add(HalfScyeDepthLine);
        if (SleeveLengthLine != null) paths.Add(SleeveLengthLine);
        if (CapWidthLine != null) paths.Add(CapWidthLine);
        if (GrainLine != null) paths.Add(GrainLine);
        if (CuffFoldLine != null) paths.Add(CuffFoldLine);
        if (HemFoldLine != null) paths.Add(HemFoldLine);
        
        return paths;
    }
}
