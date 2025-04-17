namespace PatternDrafter.Library.Pant
{
    /// <summary>
    /// Drafter for creating pant patterns according to specified measurements and options
    /// </summary>
    public class PantDrafter : PieceDrafter
    {
        // Strongly-typed properties for access
        protected PantLandmarks PantLandmarks => (PantLandmarks)Landmarks;
        protected PantPaths PantPaths => (PantPaths)Paths;
        
        protected PantMeasurements PantMeasurements => (PantMeasurements)PieceMeasurements;
        protected PantOptions PantOptions;
        private bool EasyFit = true;
        
        /// <summary>
        /// Create a new PantPatternDrafter with the specified measurements and options
        /// </summary>
        public PantDrafter(PantMeasurements measurements, PantOptions options)
            : base(measurements, options)
        {
            EasyFit = options.EasyFit;
            PantOptions = options;
        }
        
        protected override void InitializeContainers()
        {
            Landmarks = new PantLandmarks();
            Paths = new PantPaths();
        }
        
        protected override string GetPieceName()
        {
            return "Pants";
        }
        
        protected override void AddLandmarkPoints()
        {
            // Get measurements with easy fit adjustments if needed
            double bodyRiseEase = EasyFit ? 4.0 : 2.0;
            double seatEase = EasyFit ? 8.0 : 4.0;
            double extraPointAdjustment = EasyFit ? 0.5 : -0.5;
            
            // Calculate quarter seat measurement
            double quarterSeat = PantMeasurements.Seat / 4.0;
            
            // **FRONT SECTION**
            // Point 0 - Origin
            PantLandmarks.Point0 = Pattern.Point()
                .WithName("Point0_Origin")
                .WithPosition(0, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            // 0-1 Body rise plus ease
            PantLandmarks.Point1 = Pattern.Point()
                .WithName("Point1_BodyRise")
                .WithPosition(0, PantMeasurements.Rise + bodyRiseEase)
                .WithType(PointType.Landmark)
                .Build();
                
            // 1-2 Inside leg measurement
            PantLandmarks.Point2 = Pattern.Point()
                .WithName("Point2_BottomLeg")
                .WithPosition(0, PantLandmarks.Point1.Y + PantMeasurements.InsideLeg)
                .WithType(PointType.Landmark)
                .Build();
                
            // 1-3 1/2 the measurement 1-2 (knee line)
            PantLandmarks.Point3 = Pattern.Point()
                .WithName("Point3_KneeLine")
                .WithPosition(0, PantLandmarks.Point1.Y + PantMeasurements.InsideLeg / 2.0)
                .WithType(PointType.Landmark)
                .Build();
                
            // 1-4 1/4 seat measurement plus ease
            PantLandmarks.Point4 = Pattern.Point()
                .WithName("Point4_HipLine")
                .WithPosition(quarterSeat + seatEase, PantLandmarks.Point1.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Square up from 4 to get point 5
            PantLandmarks.Point5 = Pattern.Point()
                .WithName("Point5_HipLineTop")
                .WithPosition(PantLandmarks.Point4.X, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            // 5-6 1cm
            PantLandmarks.Point6 = Pattern.Point()
                .WithName("Point6_TopFront")
                .WithPosition(PantLandmarks.Point5.X - 1.0, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            // 4-7 1/4 the measurement 4-5
            double distance4to5 = PantLandmarks.Point5.Distance(PantLandmarks.Point4);
            PantLandmarks.Point7 = Pattern.Point()
                .WithName("Point7_FrontCrotchCurve")
                .WithPosition(PantLandmarks.Point4.X, PantLandmarks.Point4.Y - (distance4to5 / 4.0))
                .WithType(PointType.Landmark)
                .Build();
                
            // 4-8 1/4 the measurement 1-4 minus 0.5cm (or plus 0.5cm for easy fit)
            double distance1to4 = PantLandmarks.Point4.X - PantLandmarks.Point1.X;
            PantLandmarks.Point8 = Pattern.Point()
                .WithName("Point8_FrontCrotch")
                .WithPosition(PantLandmarks.Point4.X + (distance1to4 / 4.0) + extraPointAdjustment, PantLandmarks.Point4.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Front crotch curve control point - 3cm from 4 (or 3.25cm for easy fit)
                
            // 1-9 1/2 measurement 1-4 plus 1cm
            PantLandmarks.Point9 = Pattern.Point()
                .WithName("Point9_CenterLeg")
                .WithPosition((PantLandmarks.Point4.X - PantLandmarks.Point1.X) / 2.0 + 1.0, PantLandmarks.Point1.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Square down from 9 to get points 10 on knee line and 11 on bottom line
            PantLandmarks.Point10 = Pattern.Point()
                .WithName("Point10_CenterKnee")
                .WithPosition(PantLandmarks.Point9.X, PantLandmarks.Point3.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            PantLandmarks.Point11 = Pattern.Point()
                .WithName("Point11_CenterBottom")
                .WithPosition(PantLandmarks.Point9.X, PantLandmarks.Point2.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // 11-12 1/3 measurement 1-4 plus 1cm
            PantLandmarks.Point12 = Pattern.Point()
                .WithName("Point12_BottomFront")
                .WithPosition(PantLandmarks.Point11.X + ((PantLandmarks.Point4.X - PantLandmarks.Point1.X) / 3.0) + 1.0, PantLandmarks.Point11.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // 11-13 = 11-12
            double distance11to12 = PantLandmarks.Point12.X - PantLandmarks.Point11.X;
            PantLandmarks.Point13 = Pattern.Point()
                .WithName("Point13_InnerAnkleFront")
                .WithPosition(PantLandmarks.Point11.X - distance11to12, PantLandmarks.Point11.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Join 1-12 and mark point 14 on knee line
            // Calculate the point where line 1-12 intersects with knee line (y = Point3.Y)
            var line_1_13 = Pattern.Line()
                .WithName("Line_1_13")
                .WithStart(PantLandmarks.Point1)
                .WithEnd(PantLandmarks.Point13)
                .Build();
            
            var line_3_10 = Pattern.Line()
                .WithName("Line_3_10")
                .WithStart(PantLandmarks.Point3)
                .WithEnd(PantLandmarks.Point10)
                .Build();
            
            PantLandmarks.Point14 = line_1_13.Intersect(line_3_10);
                
            // 10-15 = 10-14
            double distance10to14 = PantLandmarks.Point14.Distance(PantLandmarks.Point10);
            PantLandmarks.Point15 = Pattern.Point()
                .WithName("Point15_KneePointInside")
                .WithPosition(PantLandmarks.Point10.X + distance10to14, PantLandmarks.Point10.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // **BACK SECTION**
            // 6-16 5cm
            PantLandmarks.Point16 = Pattern.Point()
                .WithName("Point16_BackWaist")
                .WithPosition(PantLandmarks.Point6.X - 5.0, PantLandmarks.Point6.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // 16-17 4cm
            PantLandmarks.Point17 = Pattern.Point()
                .WithName("Point17_TopBack")
                .WithPosition(PantLandmarks.Point16.X, PantLandmarks.Point16.Y - 4.0)
                .WithType(PointType.Landmark)
                .Build();
                
            // 0-18 = 4cm (5cm for easy fit)
            double backWaistExtension = EasyFit ? 5.0 : 4.0;
            PantLandmarks.Point18 = Pattern.Point()
                .WithName("Point18_BackCenterWaist")
                .WithPosition(PantLandmarks.Point0.X - backWaistExtension, PantLandmarks.Point0.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // 4-19 1/2 the measurement 4-5
            PantLandmarks.Point19 = Pattern.Point()
                .WithName("Point19_BackCrotchCurve")
                .WithPosition(PantLandmarks.Point4.X, PantLandmarks.Point4.Y - (distance4to5 / 2.0))
                .WithType(PointType.Landmark)
                .Build();
                
            // 8-20 The measurement 4-8 plus 0.5cm (1cm for easy fit)
            double distance4to8 = PantLandmarks.Point4.Distance(PantLandmarks.Point8);
            double backCrotchExtension = EasyFit ? 1.0 : 0.5;
            PantLandmarks.Point20 = Pattern.Point()
                .WithName("Point20_BackCrotchExtension")
                .WithPosition(PantLandmarks.Point8.X + distance4to8 + backCrotchExtension, PantLandmarks.Point8.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // 20-21 1cm
            PantLandmarks.Point21 = Pattern.Point()
                .WithName("Point21_BackCrotch")
                .WithPosition(PantLandmarks.Point20.X, PantLandmarks.Point20.Y + 1.0)
                .WithType(PointType.Landmark)
                .Build();
                
            // Back crotch curve control point - 5.5cm from 4 (6cm for easy fit)
            // 12-22 1cm
            PantLandmarks.Point22 = Pattern.Point()
                .WithName("Point22_BackBottom")
                .WithPosition(PantLandmarks.Point13.X + 1.0, PantLandmarks.Point13.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Join 18-22 and mark point 23 on knee line
            // Calculate the point where line 18-22 intersects with knee line (y = Point3.Y)
            var line_18_22 = Pattern.Line()
                .WithStart(PantLandmarks.Point18)
                .WithEnd(PantLandmarks.Point22)
                .WithType(PathType.CutLine)
                .Build();

            PantLandmarks.Point23 = line_18_22.Intersect(line_3_10);
                
            // 13-24 1cm
            PantLandmarks.Point24 = Pattern.Point()
                .WithName("Point24_BackAnkleInside")
                .WithPosition(PantLandmarks.Point12.X - 1.0, PantLandmarks.Point12.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // 15-25 = 14-23
            double distance14to23 = PantLandmarks.Point23.Distance(PantLandmarks.Point14);
            PantLandmarks.Point25 = Pattern.Point()
                .WithName("Point25_BackKneeInside")
                .WithPosition(PantLandmarks.Point15.X + distance14to23, PantLandmarks.Point15.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Add grain line points
            // Front grain line
            PantLandmarks.FrontGrainLineTop = Pattern.Point()
                .WithName("FrontGrainLineTop")
                .WithPosition(PantLandmarks.Point9.X / 2, PantLandmarks.Point0.Y + 5.0)
                .WithType(PointType.Construction)
                .Build();
                
            PantLandmarks.FrontGrainLineBottom = Pattern.Point()
                .WithName("FrontGrainLineBottom")
                .WithPosition(PantLandmarks.Point9.X / 2, PantLandmarks.Point2.Y - 5.0)
                .WithType(PointType.Construction)
                .Build();
                
            // Back grain line
            PantLandmarks.BackGrainLineTop = Pattern.Point()
                .WithName("BackGrainLineTop")
                .WithPosition((PantLandmarks.Point18.X + PantLandmarks.Point17.X) / 2, PantLandmarks.Point0.Y + 5.0)
                .WithType(PointType.Construction)
                .Build();
                
            PantLandmarks.BackGrainLineBottom = Pattern.Point()
                .WithName("BackGrainLineBottom")
                .WithPosition((PantLandmarks.Point18.X + PantLandmarks.Point17.X) / 2, PantLandmarks.Point2.Y - 5.0)
                .WithType(PointType.Construction)
                .Build();
        }
        
        protected override void AddLandmarkPaths()
        {
            // Create construction lines
            PantPaths.BodyRiseLine = Pattern.Line()
                .WithStart(PantLandmarks.Point1)
                .WithEnd(PantLandmarks.Point4)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            PantPaths.BottomLine = Pattern.Line()
                .WithStart(PantLandmarks.Point2)
                .WithEnd(Pattern.Point()
                    .WithName("BottomRight")
                    .WithPosition(Math.Max(PantLandmarks.Point12.X, PantLandmarks.Point23.X) + 5.0, PantLandmarks.Point2.Y)
                    .WithType(PointType.Construction)
                    .Build())
                .WithType(PathType.ConstructionLine)
                .Build();
                
            PantPaths.CenterLine = Pattern.Line()
                .WithStart(PantLandmarks.Point0)
                .WithEnd(PantLandmarks.Point2)
                .WithType(PathType.ConstructionLine)
                .Build();
                
            // Front panel paths
            PantPaths.FronSideSeamTop = Pattern.Line()
                .WithStart(PantLandmarks.Point0)
                .WithEnd(PantLandmarks.Point1)
                .WithType(PathType.CutLine)
                .Build();
            
            PantPaths.FrontSideSeam = Pattern.Line()
                .WithStart(PantLandmarks.Point1)
                .WithEnd(PantLandmarks.Point13)
                .WithType(PathType.CutLine)
                .Build();
                
            PantPaths.FrontBottomLine = Pattern.Line()
                .WithStart(PantLandmarks.Point13)
                .WithEnd(PantLandmarks.Point12)
                .WithType(PathType.CutLine)
                .Build();
                
            // Inner leg curve for front - inwards 1cm
            // Calculate mid-point between 15 and 13 with 1cm curve inward
            double midX = (PantLandmarks.Point15.X + PantLandmarks.Point13.X) / 2.0;
            double midY = (PantLandmarks.Point15.Y + PantLandmarks.Point13.Y) / 2.0;
            
            PantPaths.FrontInnerLegBottomHalf = Pattern.Line()
                .WithStart(PantLandmarks.Point12)
                .WithEnd(PantLandmarks.Point15)
                .WithType(PathType.CutLine)
                .Build();
                
            PantPaths.FrontInnerLegTopHalf = Pattern.Curve()
                .WithStart(PantLandmarks.Point15)
                .WithEnd(PantLandmarks.Point8)
                .WithControlDistance(-1, 0.5)
                .WithType(PathType.CutLine)
                .Build();
                
            // Front crotch curve
            double crotchCurveControlDistance = EasyFit ? 3.25 : 3.0;
            PantPaths.FrontCrotchCurve = Pattern.Curve()
                .WithStart(PantLandmarks.Point8)
                .WithEnd(PantLandmarks.Point7)
                .WithControlPoint(PantLandmarks.Point4, crotchCurveControlDistance)
                .WithType(PathType.CutLine)
                .Build();
            
            PantPaths.FrontCrotchLine = Pattern.Line()
                .WithStart(PantLandmarks.Point7)
                .WithEnd(PantLandmarks.Point6)
                .WithType(PathType.CutLine)
                .Build();
            
            PantPaths.FrontWaistLine = Pattern.Line()
                .WithStart(PantLandmarks.Point6)
                .WithEnd(PantLandmarks.Point0)
                .WithType(PathType.CutLine)
                .Build();
                
            // Back panel paths
            PantPaths.BackCenterSeam = Pattern.Line()
                .WithStart(PantLandmarks.Point19)
                .WithEnd(PantLandmarks.Point17)
                .WithType(PathType.CutLine)
                .Build();
            
            PantPaths.BackWaistLine = Pattern.Line()
                .WithStart(PantLandmarks.Point17)
                .WithEnd(PantLandmarks.Point18)
                .WithType(PathType.CutLine)
                .Build();
                
            PantPaths.BackSideSeam = Pattern.Line()
                .WithStart(PantLandmarks.Point18)
                .WithEnd(PantLandmarks.Point22)
                .WithType(PathType.CutLine)
                .Build();
                
            PantPaths.BackBottomLine = Pattern.Line()
                .WithStart(PantLandmarks.Point22)
                .WithEnd(PantLandmarks.Point24)
                .WithType(PathType.CutLine)
                .Build();
            
            PantPaths.BackInnerLegStraight = Pattern.Line()
                .WithStart(PantLandmarks.Point24)
                .WithEnd(PantLandmarks.Point25)
                .WithType(PathType.CutLine)
                .Build();
            
            PantPaths.BackInnerLegCurve = Pattern.Curve()
                .WithStart(PantLandmarks.Point25)
                .WithEnd(PantLandmarks.Point21)
                .WithControlDistance(-2, 0.5)
                .WithType(PathType.CutLine)
                .Build();
                
            // Inner leg curve for back - inwards 2cm
            // Calculate mid-point between 25 and 24 with 2cm curve inward
            double backMidX = (PantLandmarks.Point25.X + PantLandmarks.Point22.X) / 2.0;
            double backMidY = (PantLandmarks.Point25.Y + PantLandmarks.Point22.Y) / 2.0;

            double backCrotchCurveControlDistance = EasyFit ? 6.0 : 5.5;
            PantPaths.BackCrotchCurve = Pattern.Curve()
                .WithStart(PantLandmarks.Point21)
                .WithEnd(PantLandmarks.Point19)
                .WithControlPoint(PantLandmarks.Point4, -backCrotchCurveControlDistance)
                .WithType(PathType.CutLine)
                .Build();
                
            // Grain lines
            PantPaths.FrontGrainLine = Pattern.Line()
                .WithStart(PantLandmarks.FrontGrainLineTop)
                .WithEnd(PantLandmarks.FrontGrainLineBottom)
                .WithType(PathType.GrainLine)
                .Build();
                
            PantPaths.BackGrainLine = Pattern.Line()
                .WithStart(PantLandmarks.BackGrainLineTop)
                .WithEnd(PantLandmarks.BackGrainLineBottom)
                .WithType(PathType.GrainLine)
                .Build();
        }
        
        protected override void AddRelativePointsAndPaths()
        {
            // No additional relative points needed for basic pant pattern
            // All relative points are already added in the landmark points and paths methods
        }
        
        protected override void AddAdditionalFeatures()
        {
            // Could add pocket markings, zipper placement, etc. here
            // For now, keeping it simple with just the basic pattern
        }
        
        protected override void ConfigurePieceProperties(PatternPiece.PatternPieceBuilder builder)
        {
            builder
                .WithQuantity(1)        // Usually one piece for pants (2 legs)
                .WithCutOnFold(false)   // Pants are not cut on fold
                .WithSeamAllowance(1.5); // Standard seam allowance
        }
        
        protected override void AddInstructions(PatternPiece.PatternPieceBuilder builder)
        {
            builder
                .WithInstruction("cutting", "Cut 2 of each piece (front and back)")
                .WithInstruction("sewing", "Sew front center seam, then back center seam, then inner leg seams, then side seams");
                
            if (PantOptions.IncludeWaistband)
            {
                builder.WithInstruction("waistband", $"Cut waistband {PantOptions.WaistbandWidth}cm wide and {PantMeasurements.Waist + 2}cm long");
            }
        }
        
        /// <summary>
        /// Override to build both front and back pieces
        /// </summary>
        protected override IEnumerable<PatternPiece> BuildPieces()
        {
            // First, build the front piece
            yield return BuildFrontPiece();
            
            // Then, build the back piece
            yield return BuildBackPiece();
            
            // Add waistband if requested
            if (PantOptions.IncludeWaistband)
            {
                yield return BuildWaistband();
            }
        }
        
        /// <summary>
        /// Builds the front piece of the pants
        /// </summary>
        private PatternPiece BuildFrontPiece()
        {
            var frontPieceBuilder = Pattern.Piece()
                .WithName(GetPieceName() + " - Front")
                .WithQuantity(2)
                .WithSeamAllowance(1.5);
            
            // Add front-specific points
            frontPieceBuilder
                .WithPoint(PantLandmarks.Point0)
                .WithPoint(PantLandmarks.Point1)
                .WithPoint(PantLandmarks.Point2)
                .WithPoint(PantLandmarks.Point3)
                .WithPoint(PantLandmarks.Point4)
                .WithPoint(PantLandmarks.Point5)
                .WithPoint(PantLandmarks.Point6)
                .WithPoint(PantLandmarks.Point7)
                .WithPoint(PantLandmarks.Point8)
                .WithPoint(PantLandmarks.Point9)
                .WithPoint(PantLandmarks.Point10)
                .WithPoint(PantLandmarks.Point11)
                .WithPoint(PantLandmarks.Point12)
                .WithPoint(PantLandmarks.Point13)
                .WithPoint(PantLandmarks.Point14)
                .WithPoint(PantLandmarks.Point15)
                .WithPoint(PantLandmarks.FrontGrainLineTop)
                .WithPoint(PantLandmarks.FrontGrainLineBottom);
                
            // Add front-specific paths
            frontPieceBuilder
                .WithPath(PantPaths.FronSideSeamTop)
                .WithPath(PantPaths.FrontSideSeam)
                .WithPath(PantPaths.FrontBottomLine)
                .WithPath(PantPaths.FrontInnerLegBottomHalf)
                .WithPath(PantPaths.FrontInnerLegTopHalf)
                .WithPath(PantPaths.FrontCrotchCurve)
                .WithPath(PantPaths.FrontCrotchLine)
                .WithPath(PantPaths.FrontWaistLine)
                .WithPath(PantPaths.FrontGrainLine);
                
            // Add grain line
            frontPieceBuilder.WithGrainLine("FrontGrainLineTop", "FrontGrainLineBottom");
            
            // Add instructions
            frontPieceBuilder
                .WithInstruction("cutting", "Cut 2")
                .WithInstruction("seam allowance", "1.5cm seam allowance included");
                
            if (PantOptions.FrontPockets)
            {
                frontPieceBuilder.WithInstruction("pockets", "Add front pocket as desired");
            }
            
            return frontPieceBuilder.Build();
        }
        
        /// <summary>
        /// Builds the back piece of the pants
        /// </summary>
        private PatternPiece BuildBackPiece()
        {
            var backPieceBuilder = Pattern.Piece()
                .WithName(GetPieceName() + " - Back")
                .WithQuantity(2)
                .WithSeamAllowance(1.5);
                
            // Add back-specific points
            backPieceBuilder
                .WithPoint(PantLandmarks.Point0)
                .WithPoint(PantLandmarks.Point1)
                .WithPoint(PantLandmarks.Point2)
                .WithPoint(PantLandmarks.Point3)
                .WithPoint(PantLandmarks.Point4)
                .WithPoint(PantLandmarks.Point16)
                .WithPoint(PantLandmarks.Point17)
                .WithPoint(PantLandmarks.Point18)
                .WithPoint(PantLandmarks.Point19)
                .WithPoint(PantLandmarks.Point20)
                .WithPoint(PantLandmarks.Point21)
                .WithPoint(PantLandmarks.Point22)
                .WithPoint(PantLandmarks.Point23)
                .WithPoint(PantLandmarks.Point24)
                .WithPoint(PantLandmarks.Point25)
                .WithPoint(PantLandmarks.BackGrainLineTop)
                .WithPoint(PantLandmarks.BackGrainLineBottom);
                
            // Add back-specific paths
            backPieceBuilder
                .WithPath(PantPaths.BackWaistLine)
                .WithPath(PantPaths.BackCenterSeam)
                .WithPath(PantPaths.BackSideSeam)
                .WithPath(PantPaths.BackBottomLine)
                .WithPath(PantPaths.BackInnerLegCurve)
                .WithPath(PantPaths.BackInnerLegStraight)
                .WithPath(PantPaths.BackCrotchCurve)
                .WithPath(PantPaths.BackGrainLine);
                
            // Add grain line
            backPieceBuilder.WithGrainLine("BackGrainLineTop", "BackGrainLineBottom");
            
            // Add instructions
            backPieceBuilder
                .WithInstruction("cutting", "Cut 2")
                .WithInstruction("seam allowance", "1.5cm seam allowance included");
                
            if (PantOptions.BackPockets)
            {
                backPieceBuilder.WithInstruction("pockets", "Add back pocket as desired");
            }
            
            return backPieceBuilder.Build();
        }
        
        /// <summary>
        /// Builds a waistband piece if requested
        /// </summary>
        private PatternPiece BuildWaistband()
        {
            // Calculate waistband measurements
            double waistbandLength = PantMeasurements.Waist + 4.0; // Extra for overlap and seam allowance
            double waistbandWidth = PantOptions.WaistbandWidth * 2 + 3.0; // Double width plus seam allowance and turn-under
            
            // Create a rectangular waistband
            var waistband = Pattern.Piece()
                .WithName(GetPieceName() + " - Waistband")
                .WithQuantity(1)
                .WithSeamAllowance(1.5);
                
            // Create the four corner points
            var topLeft = Pattern.Point()
                .WithName("WaistbandTopLeft")
                .WithPosition(0, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            var topRight = Pattern.Point()
                .WithName("WaistbandTopRight")
                .WithPosition(waistbandLength, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            var bottomRight = Pattern.Point()
                .WithName("WaistbandBottomRight")
                .WithPosition(waistbandLength, waistbandWidth)
                .WithType(PointType.Landmark)
                .Build();
                
            var bottomLeft = Pattern.Point()
                .WithName("WaistbandBottomLeft")
                .WithPosition(0, waistbandWidth)
                .WithType(PointType.Landmark)
                .Build();
                
            // Create fold line points
            var foldLeft = Pattern.Point()
                .WithName("WaistbandFoldLeft")
                .WithPosition(0, waistbandWidth / 2)
                .WithType(PointType.Construction)
                .Build();
                
            var foldRight = Pattern.Point()
                .WithName("WaistbandFoldRight")
                .WithPosition(waistbandLength, waistbandWidth / 2)
                .WithType(PointType.Construction)
                .Build();
                
            // Create the lines
            var topLine = Pattern.Line()
                .WithStart(topLeft)
                .WithEnd(topRight)
                .WithType(PathType.CutLine)
                .Build();
                
            var rightLine = Pattern.Line()
                .WithStart(topRight)
                .WithEnd(bottomRight)
                .WithType(PathType.CutLine)
                .Build();
                
            var bottomLine = Pattern.Line()
                .WithStart(bottomRight)
                .WithEnd(bottomLeft)
                .WithType(PathType.CutLine)
                .Build();
                
            var leftLine = Pattern.Line()
                .WithStart(bottomLeft)
                .WithEnd(topLeft)
                .WithType(PathType.CutLine)
                .Build();
                
            var foldLine = Pattern.Line()
                .WithStart(foldLeft)
                .WithEnd(foldRight)
                .WithType(PathType.FoldLine)
                .Build();
                
            // Add all points and paths
            waistband
                .WithPoint(topLeft)
                .WithPoint(topRight)
                .WithPoint(bottomRight)
                .WithPoint(bottomLeft)
                .WithPoint(foldLeft)
                .WithPoint(foldRight)
                .WithPath(topLine)
                .WithPath(rightLine)
                .WithPath(bottomLine)
                .WithPath(leftLine)
                .WithPath(foldLine)
                .WithInstruction("cutting", "Cut 1")
                .WithInstruction("sewing", "Fold in half along fold line, attach to pants waist");
                
            return waistband.Build();
        }

        public override IEnumerable<PatternLine> HemLines()
        {
            yield return PantPaths.FrontBottomLine;
            yield return PantPaths.BackBottomLine;
        }
    }
}