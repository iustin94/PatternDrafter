using PatternDrafter.Library;
using System;
using System.Collections.Generic;

namespace PatternDrafter.Extensions
{
    /// <summary>
    /// Extension methods for the pattern drafting system to enable more complex operations
    /// </summary>
    public static class PatternExtensions
    {
        /// <summary>
        /// Creates a rectangular pattern piece with the specified width and height
        /// </summary>
        public static PatternPiece.PatternPieceBuilder CreateRectangle(
            string name, 
            double width, 
            double height, 
            bool cutOnFold = false,
            int quantity = 1,
            double seamAllowance = 1.0)
        {
            // Create the four corner points
            var topLeft = Pattern.Point()
                .WithName("top_left")
                .WithPosition(0, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            var topRight = Pattern.Point()
                .WithName("top_right")
                .WithPosition(width, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            var bottomRight = Pattern.Point()
                .WithName("bottom_right")
                .WithPosition(width, height)
                .WithType(PointType.Landmark)
                .Build();
                
            var bottomLeft = Pattern.Point()
                .WithName("bottom_left")
                .WithPosition(0, height)
                .WithType(PointType.Landmark)
                .Build();
                
            // Create the four lines
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
                
            // Return the pattern piece builder with the rectangle
            return Pattern.Piece()
                .WithName(name)
                .WithCutOnFold(cutOnFold)
                .WithQuantity(quantity)
                .WithSeamAllowance(seamAllowance)
                .WithPoint(topLeft)
                .WithPoint(topRight)
                .WithPoint(bottomRight)
                .WithPoint(bottomLeft)
                .WithPath(topLine)
                .WithPath(rightLine)
                .WithPath(bottomLine)
                .WithPath(leftLine);
        }
        
        /// <summary>
        /// Creates a circle pattern piece with the specified radius
        /// </summary>
        public static PatternPiece.PatternPieceBuilder CreateCircle(
            string name, 
            double radius, 
            int segments = 32,
            int quantity = 1,
            double seamAllowance = 1.0)
        {
            var center = Pattern.Point()
                .WithName("center")
                .WithPosition(0, 0)
                .WithType(PointType.Construction)
                .Build();
                
            var points = new List<PatternPoint>();
            var lines = new List<PatternLine>();
            
            // Create points around the circle
            for (int i = 0; i < segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);
                
                var point = Pattern.Point()
                    .WithName($"p{i}")
                    .WithPosition(x, y)
                    .WithType(PointType.CutPoint)
                    .Build();
                    
                points.Add(point);
            }
            
            // Create lines connecting the points
            for (int i = 0; i < segments; i++)
            {
                int nextIndex = (i + 1) % segments;
                
                var line = Pattern.Line()
                    .WithStart(points[i])
                    .WithEnd(points[nextIndex])
                    .WithType(PathType.CutLine)
                    .Build();
                    
                lines.Add(line);
            }
            
            // Create the pattern piece
            var builder = Pattern.Piece()
                .WithName(name)
                .WithQuantity(quantity)
                .WithSeamAllowance(seamAllowance)
                .WithPoint(center);
                
            // Add all the points and lines
            foreach (var point in points)
            {
                builder.WithPoint(point);
            }
            
            foreach (var line in lines)
            {
                builder.WithPath(line);
            }
            
            return builder;
        }
        
        /// <summary>
        /// Creates a basic bodice pattern (simplified example)
        /// </summary>
        public static PatternPiece CreateBasicBodice(
            string name,
            double bust,
            double waist,
            double shoulderToWaist,
            double acrossShoulder,
            bool cutOnFold = true,
            int quantity = 1,
            double seamAllowance = 1.5)
        {
            // Calculate derived measurements
            double width = bust / 4.0;
            double waistWidth = waist / 4.0;
            double height = shoulderToWaist;
            double shoulderWidth = acrossShoulder / 2.0;
            
            // Create key points for the bodice
            var neckPoint = Pattern.Point()
                .WithName("neck_center")
                .WithPosition(0, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            var shoulderPoint = Pattern.Point()
                .WithName("shoulder")
                .WithPosition(shoulderWidth, 0)
                .WithType(PointType.Landmark)
                .Build();
                
            var armholePoint = Pattern.Point()
                .WithName("armhole")
                .WithPosition(width, height * 0.4)
                .WithType(PointType.Landmark)
                .Build();
                
            var sideWaistPoint = Pattern.Point()
                .WithName("side_waist")
                .WithPosition(waistWidth, height)
                .WithType(PointType.Landmark)
                .Build();
                
            var centerWaistPoint = Pattern.Point()
                .WithName("center_waist")
                .WithPosition(0, height)
                .WithType(PointType.Landmark)
                .Build();
                
            // Create curved neck line
            var neckCurve = Pattern.Curve()
                .WithStart(neckPoint)
                .WithEnd(shoulderPoint)
                .WithControlDistance(-shoulderWidth * 0.2, 0.3)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create curved armhole
            var armholeCurve = Pattern.Curve()
                .WithStart(shoulderPoint)
                .WithEnd(armholePoint)
                .WithControlDistance(shoulderWidth * 0.15, 0.5)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create side seam
            var sideSeam = Pattern.Line()
                .WithStart(armholePoint)
                .WithEnd(sideWaistPoint)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create waist line
            var waistLine = Pattern.Line()
                .WithStart(sideWaistPoint)
                .WithEnd(centerWaistPoint)
                .WithType(PathType.CutLine)
                .Build();
                
            // Create center front line
            var centerLine = Pattern.Line()
                .WithStart(centerWaistPoint)
                .WithEnd(neckPoint)
                .WithType(cutOnFold ? PathType.FoldLine : PathType.CutLine)
                .Build();
                
            // Build the pattern piece
            return Pattern.Piece()
                .WithName(name)
                .WithCutOnFold(cutOnFold)
                .WithQuantity(quantity)
                .WithSeamAllowance(seamAllowance)
                // Add all points
                .WithPoint(neckPoint)
                .WithPoint(shoulderPoint)
                .WithPoint(armholePoint)
                .WithPoint(sideWaistPoint)
                .WithPoint(centerWaistPoint)
                .WithPoint(neckCurve.Control)
                .WithPoint(armholeCurve.Control)
                // Add all paths
                .WithPath(neckCurve)
                .WithPath(armholeCurve)
                .WithPath(sideSeam)
                .WithPath(waistLine)
                .WithPath(centerLine)
                // Add grain line
                .WithGrainLine("center_waist", "neck_center")
                // Add instructions
                .WithInstruction("cutting", cutOnFold ? "Cut 1 on fold" : "Cut 2")
                .WithInstruction("sewing", "Sew shoulder seams first, then side seams")
                .Build();
        }
        
        /// <summary>
        /// Creates a dart in a pattern piece between two specified points with a peak
        /// </summary>
        public static (PatternPoint, PatternPoint, PatternPoint) CreateDart(
            PatternPiece.PatternPieceBuilder builder,
            string baseName,
            PatternPoint basePoint,
            double width,
            double length,
            double angle = 0)
        {
            // Calculate dart points positions
            double halfWidth = width / 2.0;
            
            // Create rotation matrix components
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            
            // Calculate left and right dart points
            double leftX = basePoint.X - halfWidth * cos - 0 * sin;
            double leftY = basePoint.Y - halfWidth * sin + 0 * cos;
            
            double rightX = basePoint.X + halfWidth * cos - 0 * sin;
            double rightY = basePoint.Y + halfWidth * sin + 0 * cos;
            
            double pointX = basePoint.X + 0 * cos - length * sin;
            double pointY = basePoint.Y + 0 * sin + length * cos;
            
            // Create the dart points
            var leftPoint = Pattern.Point()
                .WithName($"{baseName}_dart_left")
                .WithPosition(leftX, leftY)
                .WithType(PointType.Landmark)
                .Build();
                
            var rightPoint = Pattern.Point()
                .WithName($"{baseName}_dart_right")
                .WithPosition(rightX, rightY)
                .WithType(PointType.Landmark)
                .Build();
                
            var dartPoint = Pattern.Point()
                .WithName($"{baseName}_dart_point")
                .WithPosition(pointX, pointY)
                .WithType(PointType.Landmark)
                .Build();
                
            // Create the dart lines
            var leftLine = Pattern.Line()
                .WithStart(leftPoint)
                .WithEnd(dartPoint)
                .WithType(PathType.CutLine)
                .Build();
                
            var rightLine = Pattern.Line()
                .WithStart(dartPoint)
                .WithEnd(rightPoint)
                .WithType(PathType.CutLine)
                .Build();
                
            // Add to the builder
            builder.WithPoint(leftPoint)
                  .WithPoint(rightPoint)
                  .WithPoint(dartPoint)
                  .WithPath(leftLine)
                  .WithPath(rightLine);
                  
            return (leftPoint, dartPoint, rightPoint);
        }
    }
}