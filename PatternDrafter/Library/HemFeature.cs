using System;

namespace PatternDrafter.Library
{
    /// <summary>
    /// Generates a hem piece based on a provided edge line
    /// </summary>
    public class HemFeature : PatternFeature
    {
        private double Width = 3.0;
        private PatternLine EdgeLine { get; set; }
        
        /// <summary>
        /// Creates a new HemFeature that will generate a hem for the specified edge line
        /// </summary>
        /// <param name="edgeLine">The pattern line to create a hem for</param>
        /// <param name="width">The width of the hem, defaults to 3.0</param>
        public HemFeature(PatternLine edgeLine, double width = 3.0) : base(new PieceOptions())
        {
            EdgeLine = edgeLine ?? throw new ArgumentNullException(nameof(edgeLine), "Edge line cannot be null");
            Width = width > 0 ? width : 3.0;
        }
        
        public override IEnumerable<PatternPiece> Draft()
        {
            // Calculate the direction vector of the edge line
            double dx = EdgeLine.End.X - EdgeLine.Start.X;
            double dy = EdgeLine.End.Y - EdgeLine.Start.Y;
            
            // Calculate the length of the edge line
            double length = Math.Sqrt(dx * dx + dy * dy);
            
            // Normalize the direction vector
            double nx = dx / length;
            double ny = dy / length;
            
            // Calculate the perpendicular vector (90 degrees counterclockwise)
            double perpX = -ny;
            double perpY = nx;
            
            // Create the four corner points of the hem rectangle
            
            // Point A - Top left (start of the edge line)
            var topLeft = Pattern.Point()
                .WithName("HemTopLeft")
                .WithPosition(EdgeLine.Start.X, EdgeLine.Start.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point B - Top right (end of the edge line)
            var topRight = Pattern.Point()
                .WithName("HemTopRight")
                .WithPosition(EdgeLine.End.X, EdgeLine.End.Y)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point C - Bottom right (offset by the hem width in the perpendicular direction)
            var bottomRight = Pattern.Point()
                .WithName("HemBottomRight")
                .WithPosition(EdgeLine.End.X + perpX * Width, EdgeLine.End.Y + perpY * Width)
                .WithType(PointType.Landmark)
                .Build();
                
            // Point D - Bottom left (offset by the hem width in the perpendicular direction)
            var bottomLeft = Pattern.Point()
                .WithName("HemBottomLeft")
                .WithPosition(EdgeLine.Start.X + perpX * Width, EdgeLine.Start.Y + perpY * Width)
                .WithType(PointType.Landmark)
                .Build();
            
            // Create the rectangular outline using the factory pattern
            var topEdge = Pattern.Line()
                .WithStart(topLeft)
                .WithEnd(topRight)
                .WithType(PathType.CutLine)
                .Build();
                
            var rightEdge = Pattern.Line()
                .WithStart(topRight)
                .WithEnd(bottomRight)
                .WithType(PathType.CutLine)
                .Build();
                
            var bottomEdge = Pattern.Line()
                .WithStart(bottomRight)
                .WithEnd(bottomLeft)
                .WithType(PathType.CutLine)
                .Build();
                
            var leftEdge = Pattern.Line()
                .WithStart(bottomLeft)
                .WithEnd(topLeft)
                .WithType(PathType.CutLine)
                .Build();
            
            // Calculate mid-points for fold line
            var topMid = Pattern.Point()
                .WithName("HemTopMid")
                .WithPosition((topLeft.X + topRight.X) / 2, (topLeft.Y + topRight.Y) / 2)
                .WithType(PointType.Construction)
                .Build();
                
            var bottomMid = Pattern.Point()
                .WithName("HemBottomMid")
                .WithPosition((bottomLeft.X + bottomRight.X) / 2, (bottomLeft.Y + bottomRight.Y) / 2)
                .WithType(PointType.Construction)
                .Build();
                
            // Create the fold line
            var foldLine = Pattern.Line()
                .WithStart(topMid)
                .WithEnd(bottomMid)
                .WithType(PathType.FoldLine)
                .Build();
            
            // Create the pattern piece using the factory pattern
            yield return Pattern.Piece()
                .WithName($"Hem for {EdgeLine.Name}")
                // Add all points
                .WithPoint(topLeft)
                .WithPoint(topRight)
                .WithPoint(bottomRight)
                .WithPoint(bottomLeft)
                .WithPoint(topMid)
                .WithPoint(bottomMid)
                // Add all paths
                .WithPath(topEdge)
                .WithPath(rightEdge)
                .WithPath(bottomEdge)
                .WithPath(leftEdge)
                .WithPath(foldLine)
                // Add instructions
                .WithInstruction("cutting", "Cut 1")
                .WithInstruction("sewing", "Fold along center line and press before attaching to garment")
                .WithInstruction("alignment", $"Match top edge with {EdgeLine.Name}")
                .Build();
        }
        
        /// <summary>
        /// Creates a standalone hem piece
        /// </summary>
        /// <param name="length">The length of the hem</param>
        /// <param name="width">The width of the hem, defaults to 3.0</param>
        /// <returns>A new HemFeature instance</returns>
        public static HemFeature CreateStandalone(double length, double width = 3.0)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be positive", nameof(length));
                
            // Create a horizontal edge line
            var edgeLine = Pattern.Line()
                .WithStart(Pattern.Point().WithName("Start").WithPosition(0, 0).WithType(PointType.Landmark).Build())
                .WithEnd(Pattern.Point().WithName("End").WithPosition(length, 0).WithType(PointType.Landmark).Build())
                .WithType(PathType.ConstructionLine)
                .WithName("StandaloneEdge")
                .Build();
                
            return new HemFeature(edgeLine, width);
        }
    }
}