using NetTopologySuite.Geometries;

namespace PatternDrafter.Library;

public class PatternLine : PathElement
{
    public string Name { get; private set; }
    public PatternPoint Start { get; private set; }
    public PatternPoint End { get; private set; }

    private PatternLine()
    {
    }

    public override Geometry ToGeometry(GeometryFactory factory)
    {
        return factory.CreateLineString(new[] { Start.ToCoordinate(), End.ToCoordinate() });
    }

    public override List<Coordinate> Discretize(int pointCount = 20)
    {
        var coords = new List<Coordinate>();

        for (int i = 0; i < pointCount; i++)
        {
            double t = i / (double)(pointCount - 1);
            double x = Start.X + t * (End.X - Start.X);
            double y = Start.Y + t * (End.Y - Start.Y);
            coords.Add(new Coordinate(x, y));
        }

        return coords;
    }

    // Factory method
    public static PatternLineBuilder Create() => new PatternLineBuilder();

    // Builder class
    public class PatternLineBuilder
    {
        private string _name;
        private PatternPoint _start;
        private PatternPoint _end;
        private PathType _type = PathType.CutLine;

        internal PatternLineBuilder()
        {
        }

        public PatternLineBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public PatternLineBuilder WithStart(PatternPoint start)
        {
            _start = start;
            return this;
        }

        public PatternLineBuilder WithEnd(PatternPoint end)
        {
            _end = end;
            return this;
        }

        public PatternLineBuilder WithType(PathType type)
        {
            _type = type;
            return this;
        }

        public PatternLine Build()
        {
            if (_start == null)
            {
                throw new InvalidOperationException("Start point is required");
            }

            if (_end == null)
            {
                throw new InvalidOperationException("End point is required");
            }

            if (string.IsNullOrEmpty(_name))
            {
                _name = $"line_{_start.Name}_{_end.Name}";
            }

            return new PatternLine
            {
                Name = _name,
                Start = _start,
                End = _end,
                Type = _type
            };
        }
    }

    /// <summary>
    /// Calculates the intersection point between two lines if it exists
    /// </summary>
    /// <param name="line1">The first line</param>
    /// <param name="line2">The second line</param>
    /// <param name="extendLines">If true, considers the lines as infinite; if false, only checks for intersection within line segments</param>
    /// <returns>The intersection point, or null if the lines don't intersect or are parallel</returns>
    public PatternPoint Intersect(PatternLine line2, bool extendLines = false)
    {
        // Get coordinates for first line
        double x1 = Start.X;
        double y1 = Start.Y;
        double x2 = End.X;
        double y2 = End.Y;

        // Get coordinates for second line
        double x3 = line2.Start.X;
        double y3 = line2.Start.Y;
        double x4 = line2.End.X;
        double y4 = line2.End.Y;

        // Calculate determinant
        double denominator = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

        // If the denominator is zero, the lines are parallel
        if (Math.Abs(denominator) < 0.0001)
            return null;

        // Calculate ua and ub
        double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
        double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;

        // If not extending lines, check if intersection is within both segments
        if (!extendLines && (ua < 0 || ua > 1 || ub < 0 || ub > 1))
            return null;

        // Calculate intersection point
        double intersectionX = x1 + ua * (x2 - x1);
        double intersectionY = y1 + ua * (y2 - y1);

        // Create the intersection point with a descriptive name
        return Pattern.Point()
            .WithName($"Intersection_{Name}_{line2.Name}")
            .WithPosition(intersectionX, intersectionY)
            .WithType(PointType.Construction)
            .Build();
    }
}