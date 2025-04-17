    // Updated SleeveLandmarks class with new properties

    using PatternDrafter.Library;

    public class SleeveLandmarks : PatternLandmarks
    {
        // Landmark points based on numbered points from instructions
        public PatternPoint Origin { get; set; }                    // Point 15
        public PatternPoint HalfScyeDepthPoint { get; set; }        // Point 16
        public PatternPoint SleeveLengthPoint { get; set; }         // Point 17
        public PatternPoint SleeveCapWidthPoint { get; set; }       // Point 18
        public PatternPoint UnderarmPoint { get; set; }             // Point 19
        public PatternPoint FrontSleeveCapPoint { get; set; }       // Point 20
        public PatternPoint WristPoint { get; set; }                // Point 21
        
        // Control points for curves
        public PatternPoint BackCapCurveControl { get; set; }
        public PatternPoint FrontCapCurveControl { get; set; }
        public PatternPoint UnderarmCurveControl { get; set; }
        
        // Additional feature points
        public PatternPoint ShoulderNotch { get; set; }
        public PatternPoint GrainLineTop { get; set; }
        public PatternPoint GrainLineBottom { get; set; }
        
        // Cuff or hem related points
        public PatternPoint CuffFoldLeft { get; set; }
        public PatternPoint CuffFoldRight { get; set; }
        public PatternPoint HemFoldLeft { get; set; }
        public PatternPoint HemFoldRight { get; set; }
        
        public override IEnumerable<PatternPoint> GetAllLandmarkPoints()
        {
            var points = new List<PatternPoint>();
            
            // Add all landmark points that are not null
            if (Origin != null) points.Add(Origin);
            if (HalfScyeDepthPoint != null) points.Add(HalfScyeDepthPoint);
            if (SleeveLengthPoint != null) points.Add(SleeveLengthPoint);
            if (SleeveCapWidthPoint != null) points.Add(SleeveCapWidthPoint);
            if (UnderarmPoint != null) points.Add(UnderarmPoint);
            if (FrontSleeveCapPoint != null) points.Add(FrontSleeveCapPoint);
            if (WristPoint != null) points.Add(WristPoint);
            
            return points;
        }
        
        public override IEnumerable<PatternPoint> GetAllRelativePoints()
        {
            var points = new List<PatternPoint>();
            
            // Add all relative points that are not null
            if (BackCapCurveControl != null) points.Add(BackCapCurveControl);
            if (FrontCapCurveControl != null) points.Add(FrontCapCurveControl);
            if (UnderarmCurveControl != null) points.Add(UnderarmCurveControl);
            if (ShoulderNotch != null) points.Add(ShoulderNotch);
            if (GrainLineTop != null) points.Add(GrainLineTop);
            if (GrainLineBottom != null) points.Add(GrainLineBottom);
            if (CuffFoldLeft != null) points.Add(CuffFoldLeft);
            if (CuffFoldRight != null) points.Add(CuffFoldRight);
            if (HemFoldLeft != null) points.Add(HemFoldLeft);
            if (HemFoldRight != null) points.Add(HemFoldRight);
            
            return points;
        }
    }
