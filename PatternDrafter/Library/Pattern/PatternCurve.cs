using NetTopologySuite.Geometries;

namespace PatternDrafter.Library;


public class PatternCurve : PathElement
{
    public PatternPoint Start { get; private set; }
    public PatternPoint End { get; private set; }
    public PatternPoint Control { get; private set; }
    
    private PatternCurve() { }
    
    public override Geometry ToGeometry(GeometryFactory factory)
    {
        // Approximate Bezier with line segments
        var coords = Discretize();
        return factory.CreateLineString(coords.ToArray());
    }
    
    public override List<Coordinate> Discretize(int pointCount = 20)
    {
        var coords = new List<Coordinate>();
        
        for (int i = 0; i < pointCount; i++)
        {
            double t = i / (double)(pointCount - 1);
            
            // Quadratic Bezier formula
            double x = Math.Pow(1 - t, 2) * Start.X + 
                       2 * (1 - t) * t * Control.X + 
                       Math.Pow(t, 2) * End.X;
                       
            double y = Math.Pow(1 - t, 2) * Start.Y + 
                       2 * (1 - t) * t * Control.Y + 
                       Math.Pow(t, 2) * End.Y;
                       
            coords.Add(new Coordinate(x, y));
        }
        
        return coords;
    }

    // Factory method
    public static PatternCurveBuilder Create() => new PatternCurveBuilder();

    // Builder class
    public class PatternCurveBuilder
    {
        private PatternPoint _start;
        private PatternPoint _end;
        private PatternPoint _control;
        private PathType _type = PathType.CutLine;

        internal PatternCurveBuilder() { }

        public PatternCurveBuilder WithStart(PatternPoint start)
        {
            _start = start;
            return this;
        }

        public PatternCurveBuilder WithEnd(PatternPoint end)
        {
            _end = end;
            return this;
        }

        public PatternCurveBuilder WithControlPoint(PatternPoint control, double distanceFromControl = 0.5)
        {
            if (_start == null || _end == null)
            {
                throw new InvalidOperationException("Start and end points must be set before specifying a control point");
            }
            
            // If influence factor is 0, we just use the provided control point directly
            if (Math.Abs(distanceFromControl) < 0.001)
            {
                _control = control;
                return this;
            }
            
            // Calculate midpoint of the line
            double midX = (_start.X + _end.X) / 2;
            double midY = (_start.Y + _end.Y) / 2;
            
            // Vector from start to end
            double lineVectorX = _end.X - _start.X;
            double lineVectorY = _end.Y - _start.Y;
            double lineLength = Math.Sqrt(lineVectorX * lineVectorX + lineVectorY * lineVectorY);
            
            if (lineLength < 0.001)
            {
                throw new ArgumentException("Start and end points are too close");
            }
            
            // Find the point on the line that's closest to the control point
            // This is the projection of the control point onto the line
            
            // Normalize line vector
            double normalizedLineX = lineVectorX / lineLength;
            double normalizedLineY = lineVectorY / lineLength;
            
            // Vector from start point to control
            double startToControlX = control.X - _start.X;
            double startToControlY = control.Y - _start.Y;
            
            // Project control point onto the line to find closest point
            double projectionLength = startToControlX * normalizedLineX + startToControlY * normalizedLineY;
            double projectionRatio = projectionLength / lineLength; // Position along the line (0 to 1)
            
            // Calculate the closest point on the line
            double closestPointX = _start.X + projectionRatio * lineVectorX;
            double closestPointY = _start.Y + projectionRatio * lineVectorY;
            
            // Vector from closest point to control point (perpendicular component)
            double perpVectorX = control.X - closestPointX;
            double perpVectorY = control.Y - closestPointY;
            double perpDistance = Math.Sqrt(perpVectorX * perpVectorX + perpVectorY * perpVectorY);
            
            // Calculate the final control point position based on influence factor
            // influenceFactor of 1.0 means the curve passes directly through the control point
            // influenceFactor of 0.5 means the curve passes halfway between the line and the control point
            
            if (perpDistance < 0.001)
            {
                // Control point is directly on the line, create a tiny deviation to avoid a straight line
                perpVectorX = -normalizedLineY * 0.01; // Perpendicular to line
                perpVectorY = normalizedLineX * 0.01;
                perpDistance = 0.01;
            }
            
            // Quadratic Bezier curves pass at 1/2 the distance from the line to the control point
            // So to make it pass at the desired distance, we need to adjust the control point
            double adjustedPerpDistance = perpDistance - (0.5 / distanceFromControl);
            
            // Calculate the final control point position
            double finalControlX = closestPointX + (perpVectorX / perpDistance) * adjustedPerpDistance;
            double finalControlY = closestPointY + (perpVectorY / perpDistance) * adjustedPerpDistance;
            
            // Create the control point
            string controlPointName = $"control_{_start.Name}_{_end.Name}";
            _control = PatternPoint.Create()
                .WithName(controlPointName)
                .WithPosition(finalControlX, finalControlY)
                .WithType(PointType.Construction)
                .Build();
            
            return this;
        }

        public PatternCurveBuilder WithControlDistance(double distanceValue, double distancePosition = 0.5)
        {
            if (_start == null || _end == null)
            {
                throw new InvalidOperationException("Start and end points must be set before creating a peak control");
            }
            
            // Calculate control point for the desired peak
            var midX = (_start.X + _end.X) / 2;
            var midY = (_start.Y + _end.Y) / 2;
            
            // Calculate perpendicular vector
            var dx = _end.X - _start.X;
            var dy = _end.Y - _start.Y;
            var length = Math.Sqrt(dx * dx + dy * dy);
            
            if (length < 0.001)
            {
                throw new ArgumentException("Start and end points are too close");
            }
            
            // Normalize and rotate 90 degrees
            var perpX = -dy / length;
            var perpY = dx / length;
            
            // Create control point position
            var controlX = midX + perpX * distanceValue*2;
            var controlY = midY + perpY * distanceValue*2;
            
            // Adjust for peak position
            if (Math.Abs(distancePosition - 0.5) > 0.001)
            {
                var positionFactor = (distancePosition - 0.5) * 2; // Map from [0,1] to [-1,1]
                controlX += dx * positionFactor * 0.25;
                controlY += dy * positionFactor * 0.25;
            }
            
            var controlPointName = $"control_{_start.Name}_{_end.Name}";
            _control = PatternPoint.Create()
                .WithName(controlPointName)
                .WithPosition(controlX, controlY)
                .WithType(PointType.Construction)
                .Build();
            
            return this;
        }

        public PatternCurveBuilder WithType(PathType type)
        {
            _type = type;
            return this;
        }

        public PatternCurve Build()
        {
            if (_start == null)
            {
                throw new InvalidOperationException("Start point is required");
            }

            if (_end == null)
            {
                throw new InvalidOperationException("End point is required");
            }

            if (_control == null)
            {
                throw new InvalidOperationException("Control point is required");
            }

            return new PatternCurve
            {
                Start = _start,
                End = _end,
                Control = _control,
                Type = _type
            };
        }
    }
}


