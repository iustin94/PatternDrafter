namespace PatternDrafter.Library;

  /// <summary>
    /// Contains all paths for a torso pattern
    /// </summary>
    public class TorsoPaths : PatternPaths
    {
        // Construction lines
        public PatternLine WaistLine { get; set; }
        public PatternLine HemLine { get; set; }
        public PatternLine ScyeDepthLine { get; set; }
        public PatternLine HalfScyeDepthLine { get; set; }
        public PatternLine QuarterHalfScyeDepthLine { get; set; }
        
        // Pattern cutting lines
        public PatternCurve BackNeckCurve { get; set; }
        public PatternCurve FrontNeckCurve { get; set; }
        public PatternLine NeckToShoulderLine { get; set; }
        public PatternCurve TopArmholeCurve { get; set; }
        public PatternCurve BottomArmholeCurve { get; set; }
        public PatternLine SideSeam { get; set; }
        public PatternLine BottomHem { get; set; }
        public PatternLine CenterBack { get; set; }
        
        // Additional feature paths
        public PatternLine GrainLine { get; set; }
        
        public override IEnumerable<PathElement> GetAllLandmarkPaths()
        {
            yield return WaistLine;
            yield return HemLine;
            yield return ScyeDepthLine;
            yield return HalfScyeDepthLine;
            yield return QuarterHalfScyeDepthLine;
            yield return BackNeckCurve;
            yield return FrontNeckCurve;
            yield return NeckToShoulderLine;
            yield return TopArmholeCurve;
            yield return BottomArmholeCurve;
            yield return SideSeam;
            yield return BottomHem;
            yield return CenterBack;
            yield return GrainLine;
        }
        
        public override IEnumerable<PathElement> GetAllRelativePaths()
        {
            yield return WaistLine;
            yield return HemLine;
            yield return ScyeDepthLine;
            yield return HalfScyeDepthLine;
            yield return QuarterHalfScyeDepthLine;
            yield return GrainLine;
        }
    }