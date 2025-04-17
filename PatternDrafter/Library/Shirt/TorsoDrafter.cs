using System;
using System.Collections.Generic;

namespace PatternDrafter.Library
{
    public class TorsoDrafter : PieceDrafter
    {
        private TorsoMeasurements _torsoMeasurements;
        private bool EasyFit = true; // Toggle between basic fit and easier fitting jersey overgarments
        
        // Strongly-typed properties for specific landmark types
        protected TorsoLandmarks TorsoLandmarks => (TorsoLandmarks)Landmarks;
        protected TorsoPaths TorsoPaths => (TorsoPaths)Paths;
        
        public double ArmholeDiameter => TorsoLandmarks.ShoulderTopPoint.Distance(TorsoLandmarks.ArmholeSidePoint);
        
        public TorsoDrafter(TorsoMeasurements measurements, PieceOptions options) 
            : base(measurements, options)
        {
            _torsoMeasurements = (TorsoMeasurements)measurements;
           EasyFit = options.EasyFit;
        }
        
        protected override void InitializeContainers()
        {
            Landmarks = new TorsoLandmarks();
            Paths = new TorsoPaths();
        }
        
        protected override string GetPieceName()
        {
            return EasyFit ? "Jersey Torso Block" : "Basic Torso Block";
        }
        
        protected override void ConfigurePieceProperties(PatternPiece.PatternPieceBuilder builder)
        {
            builder
                .WithCutOnFold(false) // This pattern has separate back and front pieces
                .WithQuantity(1)
                .WithSeamAllowance(1.0);
        }
        
        protected override void AddInstructions(PatternPiece.PatternPieceBuilder builder)
        {
            builder
                .WithInstruction("cutting", "Cut 1 for back, 1 for front")
                .WithInstruction("sewing", "Sew shoulder seams first, then side seams");
        }
        
        protected override void AddLandmarkPoints()
        {
            /*
             * Implementing the points as described in the text:
             * 
             * 0 - Origin point (top left corner)
             * 1 - Back neck to waist plus 1cm
             * 2 - Finished length
             * 3 - Scye depth plus 3cm (5cm for easier fit)
             * 4 - 1/2 of measurement 0-3
             * 5 - 1/4 of measurement 0-4
             * 6 - 1/5 neck size
             * 7 - 1.5 cm from point 6
             * 8 - Half back plus 2.5cm (3.5cm for easier fit) from point 3
             * 9 - Point above 8
             * 10 - Point above 8 at neck level
             * 11 - 0.75cm from point 10
             * 12 - 1/4 chest plus 4.5cm (6cm for easier fit) from point 3
             * 13 - Point below 12
             * 14 - 1/5 neck size minus 1.5cm
             */
            
            // Get basic measurements
            double neckSize = _torsoMeasurements.NeckWidth;
            double backNeckToWaist = _torsoMeasurements.Body.BackNeckToWaist;
            double finishedLength = _torsoMeasurements.FinishedLength;
            double halfBack = _torsoMeasurements.HalfBack;
            double quarterChest = _torsoMeasurements.QuarterChest;
            
            // Calculate scye depth with ease
            double scyeDepthEase = EasyFit ? 5.0 : 3.0;
            double scyeDepth = _torsoMeasurements.ScyeDepth + scyeDepthEase;
            
            // Calculate half back with ease
            double halfBackEase = EasyFit ? 3.5 : 2.5;
            double halfBackWithEase = halfBack + halfBackEase;
            
            // Calculate chest with ease
            double chestEase = EasyFit ? 4.0 : 2.5;
            double quarterChestWithEase = quarterChest + chestEase;
            
            // Point 0 - Origin
            TorsoLandmarks.Origin = Pattern.Point()
                .WithName("Origin")
                .WithPosition(0, 0)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 1 - Back neck to waist plus 1cm
            TorsoLandmarks.WaistLevel = Pattern.Point()
                .WithName("WaistLevel")
                .WithPosition(0, backNeckToWaist + 1.0)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 2 - Finished length
            TorsoLandmarks.HemLevel = Pattern.Point()
                .WithName("HemLevel")
                .WithPosition(0, finishedLength)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 3 - Scye depth plus ease
            TorsoLandmarks.ScyeDepthLevel = Pattern.Point()
                .WithName("ScyeDepthLevel")
                .WithPosition(0, scyeDepth)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 4 - 1/2 of scye depth
            TorsoLandmarks.HalfScyeDepthLevel = Pattern.Point()
                .WithName("HalfScyeDepthLevel")
                .WithPosition(0, scyeDepth / 2)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 5 - 1/4 of half scye depth
            TorsoLandmarks.QuarterHalfScyeDepthLevel = Pattern.Point()
                .WithName("QuarterHalfScyeDepthLevel")
                .WithPosition(0, scyeDepth / 8)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 6 - 1/5 neck size
            TorsoLandmarks.NeckShoulderPoint = Pattern.Point()
                .WithName("Neck Shoulder Point")
                .WithPosition(neckSize, 0)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 7 - 1.5 cm from point 6 (for back neck curve)
            TorsoLandmarks.BackNeckCurvePoint = Pattern.Point()
                .WithName("BackNeckCurvePoint")
                .WithPosition(neckSize, -0.75)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 8 - Half back plus ease from point 3
            TorsoLandmarks.BackSidePoint = Pattern.Point()
                .WithName("BackSidePoint")
                .WithPosition(halfBackWithEase, scyeDepth)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 12 - 1/4 chest plus ease from point 3 (armhole side point)
            TorsoLandmarks.ArmholeSidePoint = Pattern.Point()
                .WithName("Armhole Side Point")
                .WithPosition(quarterChestWithEase, scyeDepth)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 13 - Point below 12 at hem level
            TorsoLandmarks.HemSidePoint = Pattern.Point()
                .WithName("HemSidePoint")
                .WithPosition(quarterChestWithEase, finishedLength)
                .WithType(PointType.Landmark)
                .Build();
            
            // Point 14 - 1/5 neck size minus 1.5cm (front neck width)
            TorsoLandmarks.FrontNeckPoint = Pattern.Point()
                .WithName("FrontNeckWidth")
                .WithPosition(0, neckSize - 1.5)
                .WithType(PointType.Landmark)
                .Build();
        }
        
        protected override void AddLandmarkPaths()
        {
            // Add horizontal construction lines
            PatternPoint waistRight = Pattern.Point()
                .WithName("WaistRight")
                .WithPosition(TorsoLandmarks.ArmholeSidePoint.X, TorsoLandmarks.WaistLevel.Y)
                .WithType(PointType.Construction)
                .Build();
                
            TorsoPaths.WaistLine = Pattern.Line()
                .WithStart(TorsoLandmarks.WaistLevel)
                .WithEnd(waistRight)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            TorsoPaths.HemLine = Pattern.Line()
                .WithStart(TorsoLandmarks.HemLevel)
                .WithEnd(TorsoLandmarks.HemSidePoint)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            TorsoPaths.ScyeDepthLine = Pattern.Line()
                .WithStart(TorsoLandmarks.ScyeDepthLevel)
                .WithEnd(TorsoLandmarks.ArmholeSidePoint)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            PatternPoint halfScyeRight = Pattern.Point()
                .WithName("HalfScyeRight")
                .WithPosition(TorsoLandmarks.ArmholeSidePoint.X, TorsoLandmarks.HalfScyeDepthLevel.Y)
                .WithType(PointType.Construction)
                .Build();
                
            TorsoPaths.HalfScyeDepthLine = Pattern.Line()
                .WithStart(TorsoLandmarks.HalfScyeDepthLevel)
                .WithEnd(halfScyeRight)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            PatternPoint quarterHalfScyeRight = Pattern.Point()
                .WithName("QuarterHalfScyeRight")
                .WithPosition(TorsoLandmarks.ArmholeSidePoint.X, TorsoLandmarks.QuarterHalfScyeDepthLevel.Y)
                .WithType(PointType.Construction)
                .Build();
                
            TorsoPaths.QuarterHalfScyeDepthLine = Pattern.Line()
                .WithStart(TorsoLandmarks.QuarterHalfScyeDepthLevel)
                .WithEnd(quarterHalfScyeRight)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            // Create back neck curve
            TorsoPaths.BackNeckCurve = Pattern.Curve()
                .WithStart(TorsoLandmarks.Origin)
                .WithEnd(TorsoLandmarks.BackNeckCurvePoint)
                .WithControlDistance(0.25, 0.5) // Adjusted for a nice curve
                .WithType(PathType.CutLine)
                .Build();
        }
        
        protected override void AddRelativePointsAndPaths()
        {
            // According to the text description:
            // "Square up from point 8 to get points 9 and 10"
            // Point 9 would be at the intersection of the vertical from 8 and the half scye depth line
            // Point 10 would be at the intersection of the vertical from 8 and the neck level
            
            double backSideX = TorsoLandmarks.BackSidePoint.X;
            
            // Point 9 - Shoulder end point
            TorsoLandmarks.ArmholeMidPoint = Pattern.Point()
                .WithName("ShoulderEndPoint")
                .WithPosition(backSideX, TorsoLandmarks.HalfScyeDepthLevel.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 10 - Shoulder top point
            TorsoLandmarks.Tmp = Pattern.Point()
                .WithName("Tmp")
                .WithPosition(backSideX, TorsoLandmarks.QuarterHalfScyeDepthLevel.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point 11 - Shoulder extended point (0.75cm from point 10)
            TorsoLandmarks.ShoulderTopPoint = Pattern.Point()
                .WithName("Shoulder Top Point")
                .WithPosition(backSideX + 0.75, TorsoLandmarks.Tmp.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Create shoulder line from 7 to 11
            TorsoPaths.NeckToShoulderLine = Pattern.Line()
                .WithStart(TorsoLandmarks.BackNeckCurvePoint)
                .WithEnd(TorsoLandmarks.ShoulderTopPoint)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create armhole curve from 11 to 9
            TorsoPaths.TopArmholeCurve = Pattern.Curve()
                .WithStart(TorsoLandmarks.ShoulderTopPoint)
                .WithEnd(TorsoLandmarks.ArmholeMidPoint)
                .WithControlDistance(0.25, 0.4) // Adjusted for a smoother curve through point 9
                .WithType(PathType.CutLine)
                .Build();
            
            TorsoPaths.BottomArmholeCurve = Pattern.Curve()
                .WithStart(TorsoLandmarks.ArmholeMidPoint)
                .WithEnd(TorsoLandmarks.ArmholeSidePoint)
                .WithControlDistance(2.5, 0.7) // Adjusted for a smoother curve through point 9
                .WithType(PathType.CutLine)
                .Build();
                
            // Create side seam from 12 to 13
            TorsoPaths.SideSeam = Pattern.Line()
                .WithStart(TorsoLandmarks.ArmholeSidePoint)
                .WithEnd(TorsoLandmarks.HemSidePoint)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create hem line from 13 to 2
            TorsoPaths.BottomHem = Pattern.Line()
                .WithStart(TorsoLandmarks.HemSidePoint)
                .WithEnd(TorsoLandmarks.HemLevel)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create center back line from 2 to 0
            TorsoPaths.CenterBack = Pattern.Line()
                .WithStart(TorsoLandmarks.HemLevel)
                .WithEnd(TorsoLandmarks.Origin)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create front neck curve based on point 14
            TorsoPaths.FrontNeckCurve = Pattern.Curve()
                .WithStart(TorsoLandmarks.BackNeckCurvePoint)
                .WithEnd(TorsoLandmarks.FrontNeckPoint)
                .WithControlDistance(-2.5, 0.5) // Deeper curve for front neck
                .WithType(PathType.CutLine)
                .Build();
        }
        
        protected override void AddAdditionalFeatures()
        {
            // Add grain line
            TorsoLandmarks.GrainLineTop = Pattern.Point()
                .WithName("GrainLineTop")
                .WithPosition(TorsoLandmarks.ArmholeSidePoint.X / 2, TorsoLandmarks.Origin.Y + 2)
                .WithType(PointType.Construction)
                .Build();
                
            TorsoLandmarks.GrainLineBottom = Pattern.Point()
                .WithName("GrainLineBottom")
                .WithPosition(TorsoLandmarks.ArmholeSidePoint.X / 2, TorsoLandmarks.HemLevel.Y - 2)
                .WithType(PointType.Construction)
                .Build();
                
            TorsoPaths.GrainLine = Pattern.Line()
                .WithStart(TorsoLandmarks.GrainLineTop)
                .WithEnd(TorsoLandmarks.GrainLineBottom)
                .WithType(PathType.GrainLine)
                .Build();
        }
        
        /// <summary>
        /// Helper method to calculate a point on the shoulder seam at a relative position
        /// </summary>
        private PatternPoint CalculatePointOnShoulderSeam(double relativePosition)
        {
            double startX = TorsoLandmarks.BackNeckCurvePoint.X;
            double startY = TorsoLandmarks.BackNeckCurvePoint.Y;
            double endX = TorsoLandmarks.ShoulderTopPoint.X;
            double endY = TorsoLandmarks.ShoulderTopPoint.Y;
            
            double x = startX + relativePosition * (endX - startX);
            double y = startY + relativePosition * (endY - startY);
            
            return Pattern.Point()
                .WithName("ShoulderNotch")
                .WithPosition(x, y)
                .WithType(PointType.Notch)
                .Build();
        }
        
        protected override IEnumerable<PatternPiece> BuildPieces()
        {
            // Create separate pattern pieces for back and front
            yield return BuildFrontPiece(); // For this example, we're building just the back piece
            yield return BuildBackPiece();
        }
        
        private PatternPiece BuildBackPiece()
        {
            var backPieceBuilder = Pattern.Piece()
                .WithName(GetPieceName() + " - Back")
                .WithQuantity(1)
                .WithSeamAllowance(1.0);
            
            // Add all landmark points except front neck width
            foreach (var point in TorsoLandmarks.GetAllLandmarkPoints())
            {
                if (point != TorsoLandmarks.FrontNeckPoint)
                {
                    backPieceBuilder.WithPoint(point);
                }
            }
            
            // Add all relative points except front neck control
            foreach (var point in TorsoLandmarks.GetAllRelativePoints())
            {
                backPieceBuilder.WithPoint(point);
            }
            
            // Add all landmark paths except front neck curve
            foreach (var path in TorsoPaths.GetAllLandmarkPaths())
            {
                if (path != TorsoPaths.FrontNeckCurve)
                {
                    backPieceBuilder.WithPath(path);
                }
            }
            
            // Add all relative paths
            foreach (var path in TorsoPaths.GetAllRelativePaths())
            {
                backPieceBuilder.WithPath(path);
            }
            
            // Add grain line and instructions
            backPieceBuilder.WithGrainLine(TorsoLandmarks.GrainLineTop.Name, TorsoLandmarks.GrainLineBottom.Name);
            backPieceBuilder.WithInstruction("cutting", "Cut 1");
            backPieceBuilder.WithInstruction("sewing", "Sew shoulder seams first, then side seams");
            
            return backPieceBuilder.Build();
        }
        
        private PatternPiece BuildFrontPiece()
        {
            var backPieceBuilder = Pattern.Piece()
                .WithName(GetPieceName() + " - Front")
                .WithQuantity(1)
                .WithSeamAllowance(1.0);
            
            // Add all landmark points except front neck width
            foreach (var point in TorsoLandmarks.GetAllLandmarkPoints())
            {
                if (point != TorsoLandmarks.NeckShoulderPoint || point != TorsoLandmarks.QuarterHalfScyeDepthLevel)
                {
                    backPieceBuilder.WithPoint(point);
                }
            }
            
            // Add all relative points except front neck control
            foreach (var point in TorsoLandmarks.GetAllRelativePoints())
            {
                backPieceBuilder.WithPoint(point);
            }
            
            // Add all landmark paths except front neck curve
            foreach (var path in TorsoPaths.GetAllLandmarkPaths())
            {
                if (path != TorsoPaths.BackNeckCurve || path != TorsoPaths.QuarterHalfScyeDepthLine)
                {
                    backPieceBuilder.WithPath(path);
                }
            }
            
            // Add all relative paths
            foreach (var path in TorsoPaths.GetAllRelativePaths())
            {
                backPieceBuilder.WithPath(path);
            }
            
            // Add grain line and instructions
            backPieceBuilder.WithGrainLine(TorsoLandmarks.GrainLineTop.Name, TorsoLandmarks.GrainLineBottom.Name);
            backPieceBuilder.WithInstruction("cutting", "Cut 1");
            backPieceBuilder.WithInstruction("sewing", "Sew shoulder seams first, then side seams");
            
            return backPieceBuilder.Build();
        }

        public override IEnumerable<PatternLine> HemLines()
        {
            yield return TorsoPaths.BottomHem;
        }
    }
}