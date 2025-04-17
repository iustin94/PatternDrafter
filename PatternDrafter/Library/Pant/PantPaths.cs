namespace PatternDrafter.Library;

/// <summary>
/// Contains all paths for pant pattern pieces
/// </summary>
public class PantPaths : PatternPaths
{
    // Construction Lines
    public PatternLine BodyRiseLine { get; set; }
    public PatternLine HipLine { get; set; }
    public PatternLine KneeLine { get; set; }
    public PatternLine BottomLine { get; set; }
    public PatternLine CenterLine { get; set; }
    
    // Front Piece Cutting Lines
    public PatternLine FrontWaistLine { get; set; }
    public PatternLine FronSideSeamTop { get; set; }
    public PatternLine FrontSideSeam { get; set; }
    public PatternLine FrontBottomLine { get; set; }
    public PatternLine FrontInnerLegBottomHalf { get; set; }
    public PatternCurve FrontInnerLegTopHalf { get; set; }
    public PatternCurve FrontCrotchCurve { get; set; }
    
    // Back Piece Cutting Lines
    public PatternLine BackWaistLine { get; set; }
    public PatternLine BackSideSeam { get; set; }
    public PatternLine BackBottomLine { get; set; }
    public PatternCurve BackInnerLegCurve { get; set; }
    public PatternLine BackInnerLegStraight { get; set; }
    public PatternCurve BackCrotchCurve { get; set; }
    public PatternLine BackCenterSeam { get; set; }
    
    // Grain Lines
    public PatternLine FrontGrainLine { get; set; }
    public PatternLine BackGrainLine { get; set; }
    public PatternLine FrontCrotchLine { get; set; }

    public override IEnumerable<PathElement> GetAllLandmarkPaths()
    {
        var paths = new List<PathElement>();
        
        // Add main cutting lines for front
        if (FrontWaistLine != null) paths.Add(FrontWaistLine);
        if (FrontCrotchLine != null) paths.Add(FrontCrotchLine);
        if (FronSideSeamTop != null) paths.Add(FronSideSeamTop);
        if (FrontSideSeam != null) paths.Add(FrontSideSeam);
        if (FrontBottomLine != null) paths.Add(FrontBottomLine);
        if (FrontInnerLegBottomHalf != null) paths.Add(FrontInnerLegBottomHalf);
        if (FrontInnerLegTopHalf != null) paths.Add(FrontInnerLegTopHalf);
        if (FrontCrotchCurve != null) paths.Add(FrontCrotchCurve);
        
        // Add main cutting lines for back
        if (BackWaistLine != null) paths.Add(BackWaistLine);
        if (BackSideSeam != null) paths.Add(BackSideSeam);
        if (BackBottomLine != null) paths.Add(BackBottomLine);
        if (BackInnerLegCurve != null) paths.Add(BackInnerLegCurve);
        if (BackInnerLegStraight != null) paths.Add(BackInnerLegStraight);
        if (BackCrotchCurve != null) paths.Add(BackCrotchCurve);
        if (BackCenterSeam != null) paths.Add(BackCenterSeam);
        
        return paths;
    }
    
    public override IEnumerable<PathElement> GetAllRelativePaths()
    {
        var paths = new List<PathElement>();
        
        // Add construction lines
        if (BodyRiseLine != null) paths.Add(BodyRiseLine);
        if (HipLine != null) paths.Add(HipLine);
        if (KneeLine != null) paths.Add(KneeLine);
        if (BottomLine != null) paths.Add(BottomLine);
        if (CenterLine != null) paths.Add(CenterLine);
        
        // Add grain lines
        if (FrontGrainLine != null) paths.Add(FrontGrainLine);
        if (BackGrainLine != null) paths.Add(BackGrainLine);
        
        return paths;
    }
}