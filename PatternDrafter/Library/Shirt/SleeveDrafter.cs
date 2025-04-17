using System;
using System.Collections.Generic;

namespace PatternDrafter.Library
{
    public class SleeveDrafter : PieceDrafter
    {
        private bool ShortSleeve = false;
        private SleeveMeasurements _sleeveMeasurements;
        
        // References to body measurements for sleeve calculations
        private double BodyDiagonal11to12 { get; set; }
        
        // Strongly-typed properties for specific landmark types
        protected SleeveLandmarks SleeveLandmarks => (SleeveLandmarks)Landmarks;
        protected SleevePaths SleevePaths => (SleevePaths)Paths;
        
        public SleeveDrafter(BodyMeasurements measurements, PieceOptions options, double bodyDiagonal11to12, double sleeveLength) 
            : base(new PieceMeasurements(measurements), options)
        {
            ShortSleeve = options.Short;
            BodyDiagonal11to12 = bodyDiagonal11to12;
            _sleeveMeasurements = new SleeveMeasurements(measurements, sleeveLength);
        }
        
        protected override void InitializeContainers()
        {
            Landmarks = new SleeveLandmarks();
            Paths = new SleevePaths();
        }
        
        protected override string GetPieceName()
        {
            return ShortSleeve ? "Short Sleeve" : "Long Sleeve";
        }
        
        protected override void ConfigurePieceProperties(PatternPiece.PatternPieceBuilder builder)
        {
            builder
                .WithQuantity(2) // Need two sleeves
                .WithCutOnFold(false)
                .WithSeamAllowance(1.0);
        }
        
        protected override void AddInstructions(PatternPiece.PatternPieceBuilder builder)
        {
            builder
                .WithInstruction("cutting", "Cut 2")
                .WithInstruction("sewing", "Ease sleeve cap into armhole");
                
            if (!ShortSleeve)
            {
                builder.WithInstruction("finishing", "Add cuff or hem as required");
            }
        }
        
        protected override void AddLandmarkPoints()
        {
            /*
             * Implementing the points as described in the instructions:
             * 
             * 15 - Origin point for sleeve (top left)
             * 16 - 1/2 measurement 0-3 from body section (half scye depth)
             * 17 - Sleeve length point
             * 18 - Measurement of diagonal line from 11-12 on body section plus 2.5cm
             * 19 - Point below 18
             * 20 - 1/3 measurement 18-15
             * 21 - 1/2 close wrist measurement plus 6cm (or 7cm for easy fit)
             */
            
            // Get necessary measurements
            double scyeDepth = _sleeveMeasurements.Body.ScyeDepth;
            double sleeveLength = _sleeveMeasurements.SleeveLength;
            double wristMeasurement = _sleeveMeasurements.Body.WristCircumference;
            
            // Calculate sleeve dimensions based on instructions
            double halfScyeDepth = scyeDepth / 2;
            
            double horizontalComponent = 0;
            if (BodyDiagonal11to12 > halfScyeDepth) // Ensure we don't try to take square root of negative number
            {
                horizontalComponent = Math.Sqrt(Math.Pow(BodyDiagonal11to12, 2) - Math.Pow(halfScyeDepth, 2));
            }

            // Add the standard 2.5cm adjustment from the instructions
            double sleeveCapWidth = horizontalComponent + 2.5;
            
            // Calculate wrist width with ease
            double wristEase = Options.EasyFit ? 7.0 : 6.0;
            double wristWidth = (wristMeasurement / 2) + wristEase;
            
            // Point 15 - Origin (top left of sleeve)
            SleeveLandmarks.Origin = Pattern.Point()
                .WithName("Point15_Origin")
                .WithPosition(0, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 16 - Half scye depth point
            SleeveLandmarks.HalfScyeDepthPoint = Pattern.Point()
                .WithName("Point16_HalfScyeDepth")
                .WithPosition(halfScyeDepth, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 17 - Sleeve length point
            SleeveLandmarks.SleeveLengthPoint = Pattern.Point()
                .WithName("Point17_SleeveLength")
                .WithPosition(0, sleeveLength)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 18 - Sleeve cap width point
            SleeveLandmarks.SleeveCapWidthPoint = Pattern.Point()
                .WithName("Point18_SleeveCapWidth")
                .WithPosition(sleeveCapWidth, halfScyeDepth)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 19 - Point below 18 (square down)
            SleeveLandmarks.UnderarmPoint = Pattern.Point()
                .WithName("Point19_Underarm")
                .WithPosition(sleeveCapWidth, halfScyeDepth)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 20 - 1/3 measurement 18-15
            double thirdOfCapWidth = sleeveCapWidth / 3;
            SleeveLandmarks.FrontSleeveCapPoint = Pattern.Point()
                .WithName("Point20_FrontSleeveCap")
                .WithPosition(sleeveCapWidth*2/3, halfScyeDepth*2/3)
                .WithType(PointType.Landmark)
                .Build();

            if (ShortSleeve)
            {
                // Point 21 - 1/2 close wrist measurement plus ease
                SleeveLandmarks.WristPoint = Pattern.Point()
                    .WithName("Point21_Wrist")
                    .WithPosition(sleeveCapWidth - 4, sleeveLength)
                    .WithType(PointType.Landmark)
                    .Build();
            }
            else
            {
                // Point 21 - 1/2 close wrist measurement plus ease
                SleeveLandmarks.WristPoint = Pattern.Point()
                    .WithName("Point21_Wrist")
                    .WithPosition(wristWidth, sleeveLength)
                    .WithType(PointType.Landmark)
                    .Build();
            }
        }
        
        protected override void AddLandmarkPaths()
        {
            // Create horizontal line for half scye depth
            SleevePaths.HalfScyeDepthLine = Pattern.Line()
                .WithStart(SleeveLandmarks.Origin)
                .WithEnd(SleeveLandmarks.HalfScyeDepthPoint)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            // Create horizontal line at sleeve length
            SleevePaths.SleeveLengthLine = Pattern.Line()
                .WithStart(SleeveLandmarks.SleeveLengthPoint)
                .WithEnd(SleeveLandmarks.WristPoint)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            // Create vertical line from origin to sleeve length
            SleevePaths.CenterLine = Pattern.Line()
                .WithStart(SleeveLandmarks.Origin)
                .WithEnd(SleeveLandmarks.SleeveLengthPoint)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create vertical line from cap width point to underarm point
            SleevePaths.CapWidthLine = Pattern.Line()
                .WithStart(SleeveLandmarks.SleeveCapWidthPoint)
                .WithEnd(SleeveLandmarks.UnderarmPoint)
                .WithType(PathType.ConstructionLine)
                .Build();
        }
        
        protected override void AddRelativePointsAndPaths()
        {
            // Create the sleeve head curve
            
            // First part: 18-20 curve in 0.75 cm
            SleeveLandmarks.BackCapCurveControl = Pattern.Point()
                .WithName("BackCapCurveControl")
                .WithPosition(
                    (SleeveLandmarks.SleeveCapWidthPoint.X + SleeveLandmarks.FrontSleeveCapPoint.X) / 2,
                    -0.75) // Curve in (upward) by 0.75cm
                .WithType(PointType.Construction)
                .Build();
                
            SleevePaths.BackCapCurve = Pattern.Curve()
                .WithStart(SleeveLandmarks.SleeveCapWidthPoint)
                .WithEnd(SleeveLandmarks.FrontSleeveCapPoint)
                .WithControlDistance(-0.75, 0.5)
                .WithType(PathType.CutLine)
                .Build();
                
            // Second part: 20-15 curve out 2cm
            SleeveLandmarks.FrontCapCurveControl = Pattern.Point()
                .WithName("FrontCapCurveControl")
                .WithPosition(
                    SleeveLandmarks.FrontSleeveCapPoint.X / 2,
                    -2.0) // Curve out (upward) by 2cm
                .WithType(PointType.Construction)
                .Build();
                
            SleevePaths.FrontCapCurve = Pattern.Curve()
                .WithStart(SleeveLandmarks.FrontSleeveCapPoint)
                .WithEnd(SleeveLandmarks.Origin)
                .WithControlDistance(2, 0.5)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create underarm to wrist line (with curve if required)
            // "join 18-21 with a curve if required"
            if (ShortSleeve)
            {
                SleevePaths.UnderarmToWristCurve = Pattern.Curve()
                    .WithStart(SleeveLandmarks.UnderarmPoint)
                    .WithEnd(SleeveLandmarks.WristPoint)
                    .WithControlDistance(0.75, 0.5)
                    .WithType(PathType.CutLine)
                    .Build();
            }
            else
            {
                // For long sleeve, use a slight curve for better fit
                SleeveLandmarks.UnderarmCurveControl = Pattern.Point()
                    .WithName("UnderarmCurveControl")
                    .WithPosition(
                        SleeveLandmarks.UnderarmPoint.X - 1.0, // Curve in slightly
                        (SleeveLandmarks.UnderarmPoint.Y + SleeveLandmarks.WristPoint.Y) / 2)
                    .WithType(PointType.Construction)
                    .Build();
                    
                SleevePaths.UnderarmToWristCurve = Pattern.Curve()
                    .WithStart(SleeveLandmarks.UnderarmPoint)
                    .WithEnd(SleeveLandmarks.WristPoint)
                    .WithControlDistance(2, 0.5)
                    .WithType(PathType.CutLine)
                    .Build();
            }
            
            // Create wrist line
            SleevePaths.WristLine = Pattern.Line()
                .WithStart(SleeveLandmarks.WristPoint)
                .WithEnd(SleeveLandmarks.SleeveLengthPoint)
                .WithType(PathType.CutLine)
                .Build();
        }
        
        protected override void AddAdditionalFeatures()
        {
            // Add notch at the top of the sleeve cap for matching with shoulder seam
            SleeveLandmarks.ShoulderNotch = Pattern.Point()
                .WithName("ShoulderNotch")
                .WithPosition(SleeveLandmarks.SleeveCapWidthPoint.X * 0.75, -0.5)
                .WithType(PointType.Notch)
                .Build();
                
            // Add grain line down center of sleeve
            SleeveLandmarks.GrainLineTop = Pattern.Point()
                .WithName("GrainLineTop")
                .WithPosition(SleeveLandmarks.SleeveCapWidthPoint.X / 2, SleeveLandmarks.Origin.Y + 2)
                .WithType(PointType.Construction)
                .Build();
                
            SleeveLandmarks.GrainLineBottom = Pattern.Point()
                .WithName("GrainLineBottom")
                .WithPosition(SleeveLandmarks.SleeveCapWidthPoint.X / 2, SleeveLandmarks.SleeveLengthPoint.Y - 2)
                .WithType(PointType.Construction)
                .Build();
                
            SleevePaths.GrainLine = Pattern.Line()
                .WithStart(SleeveLandmarks.GrainLineTop)
                .WithEnd(SleeveLandmarks.GrainLineBottom)
                .WithType(PathType.GrainLine)
                .Build();
                
            // Add fold line marking for cuff if long sleeve
            if (!ShortSleeve)
            {
                double cuffHeight = 5.0; // Typical cuff height
                double cuffY = SleeveLandmarks.SleeveLengthPoint.Y - cuffHeight;
                
                SleeveLandmarks.CuffFoldLeft = Pattern.Point()
                    .WithName("CuffFoldLeft")
                    .WithPosition(SleeveLandmarks.SleeveLengthPoint.X, cuffY)
                    .WithType(PointType.Construction)
                    .Build();
                    
                SleeveLandmarks.CuffFoldRight = Pattern.Point()
                    .WithName("CuffFoldRight")
                    .WithPosition(SleeveLandmarks.WristPoint.X, cuffY)
                    .WithType(PointType.Construction)
                    .Build();
                    
                SleevePaths.CuffFoldLine = Pattern.Line()
                    .WithStart(SleeveLandmarks.CuffFoldLeft)
                    .WithEnd(SleeveLandmarks.CuffFoldRight)
                    .WithType(PathType.ConstructionLine)
                    .Build();
            }
            
            // For short sleeve, add hem fold line
            if (ShortSleeve)
            {
                double hemHeight = 2.5; // Typical hem height
                double hemY = SleeveLandmarks.SleeveLengthPoint.Y - hemHeight;
                
                SleeveLandmarks.HemFoldLeft = Pattern.Point()
                    .WithName("HemFoldLeft")
                    .WithPosition(SleeveLandmarks.SleeveLengthPoint.X, hemY)
                    .WithType(PointType.Construction)
                    .Build();
                    
                SleeveLandmarks.HemFoldRight = Pattern.Point()
                    .WithName("HemFoldRight")
                    .WithPosition(SleeveLandmarks.WristPoint.X, hemY)
                    .WithType(PointType.Construction)
                    .Build();
                    
                SleevePaths.HemFoldLine = Pattern.Line()
                    .WithStart(SleeveLandmarks.HemFoldLeft)
                    .WithEnd(SleeveLandmarks.HemFoldRight)
                    .WithType(PathType.ConstructionLine)
                    .Build();
            }
        }

        public override IEnumerable<PatternLine> HemLines()
        {
            yield return SleevePaths.WristLine;
        }
    }
}
