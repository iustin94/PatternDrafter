namespace PatternDrafter.Library;

/// <summary>
/// Contains all landmark points for a torso pattern
/// </summary>
public class TorsoLandmarks : PatternLandmarks
{
    // Landmark points with descriptive names instead of numbered points
    public PatternPoint Origin { get; set; }
    public PatternPoint WaistLevel { get; set; }
    public PatternPoint HemLevel { get; set; }
    public PatternPoint ScyeDepthLevel { get; set; }
    public PatternPoint HalfScyeDepthLevel { get; set; }
    public PatternPoint QuarterHalfScyeDepthLevel { get; set; }
    public PatternPoint NeckShoulderPoint { get; set; }
    public PatternPoint BackNeckCurvePoint { get; set; }
    public PatternPoint BackSidePoint { get; set; }
    public PatternPoint ArmholeSidePoint { get; set; }
    public PatternPoint HemSidePoint { get; set; }
    public PatternPoint FrontNeckPoint { get; set; }
    
    // Relative points derived from landmark points
    public PatternPoint ArmholeMidPoint { get; set; }  // Point 9
    public PatternPoint Tmp { get; set; }  // Point 10
    public PatternPoint ShoulderTopPoint { get; set; }  // Point 11
    
    // Control points for curves
    public PatternPoint BackNeckControl { get; set; }
    public PatternPoint TopArmholeControl { get; set; }
    public PatternPoint BottomArmholeControl { get; set; }
    
    // Additional feature points
    public PatternPoint ShoulderNotch { get; set; }
    public PatternPoint ArmholeNotch { get; set; }
    public PatternPoint GrainLineTop { get; set; }
    public PatternPoint GrainLineBottom { get; set; }
    
    // Any other special points needed for the torso pattern
    public PatternPoint DartBase { get; set; }
    public PatternPoint DartLeft { get; set; }
    public PatternPoint DartRight { get; set; }
    public PatternPoint DartPoint { get; set; }
    
    public override IEnumerable<PatternPoint> GetAllLandmarkPoints()
    {
        var points = new List<PatternPoint>();
        
        // Add all landmark points that are not null
        if (Origin != null) points.Add(Origin);
        if (WaistLevel != null) points.Add(WaistLevel);
        if (HemLevel != null) points.Add(HemLevel);
        if (ScyeDepthLevel != null) points.Add(ScyeDepthLevel);
        if (HalfScyeDepthLevel != null) points.Add(HalfScyeDepthLevel);
        if (QuarterHalfScyeDepthLevel != null) points.Add(QuarterHalfScyeDepthLevel);
        if (NeckShoulderPoint != null) points.Add(NeckShoulderPoint);
        if (BackNeckCurvePoint != null) points.Add(BackNeckCurvePoint);
        if (BackSidePoint != null) points.Add(BackSidePoint);
        if (ArmholeSidePoint != null) points.Add(ArmholeSidePoint);
        if (HemSidePoint != null) points.Add(HemSidePoint);
        if (FrontNeckPoint != null) points.Add(FrontNeckPoint);
        
        return points;
    }
    
    public override IEnumerable<PatternPoint> GetAllRelativePoints()
    {
        var points = new List<PatternPoint>();
        
        // Add all relative points that are not null
        if (ArmholeMidPoint != null) points.Add(ArmholeMidPoint);
        if (Tmp != null) points.Add(Tmp);
        if (ShoulderTopPoint != null) points.Add(ShoulderTopPoint);
        if (BackNeckControl != null) points.Add(BackNeckControl);
        if (ShoulderNotch != null) points.Add(ShoulderNotch);
        if (ArmholeNotch != null) points.Add(ArmholeNotch);
        if (GrainLineTop != null) points.Add(GrainLineTop);
        if (GrainLineBottom != null) points.Add(GrainLineBottom);
        if (DartBase != null) points.Add(DartBase);
        if (DartLeft != null) points.Add(DartLeft);
        if (DartRight != null) points.Add(DartRight);
        if (DartPoint != null) points.Add(DartPoint);
        
        return points;
    }
}