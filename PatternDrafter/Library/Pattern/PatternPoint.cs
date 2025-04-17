using NetTopologySuite.Geometries;

namespace PatternDrafter.Library;

public enum PointType
{
    Construction, // Used for calculations, not shown on final pattern
    Landmark, // Major reference points
    Notch, // Alignment notches
    CutPoint, // Points on cutting line
    SeamLine, // Points on seam line
    FoldLine // Points on fold line
}

public class PatternPoint
{
    public string Name { get; private set; }
    public double X { get; private set; }
    public double Y { get; private set; }
    public PointType Type { get; private set; }

    private PatternPoint()
    {
    }

    public double Distance(PatternPoint p2)
    {
        return Math.Sqrt(Math.Pow(X - p2.X, 2) + Math.Pow(Y - p2.Y, 2));
    }
        

    public string ToString()
    {
        return $"{Name} ({X}, {Y})";
    }

    public Coordinate ToCoordinate() => new Coordinate(X, Y);

    // Factory method
    public static PatternPointBuilder Create() => new PatternPointBuilder();

    // Builder class
    public class PatternPointBuilder
    {
        private string _name;
        private double _x;
        private double _y;
        private PointType _type = PointType.Construction;

        internal PatternPointBuilder()
        {
        }

        public PatternPointBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public PatternPointBuilder WithPosition(double x, double y)
        {
            _x = x;
            _y = y;
            return this;
        }

        public PatternPointBuilder WithType(PointType type)
        {
            _type = type;
            return this;
        }

        public PatternPoint Build()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("Point name is required");
            }

            return new PatternPoint
            {
                Name = _name,
                X = _x,
                Y = _y,
                Type = _type
            };
        }
    }

    public PatternPoint Mirror(bool vertically = false, bool horizontaly = false)
    {
        double newX = vertically ? -X : X;
        double newY = horizontaly ? -Y : Y; 
        
        return new PatternPointBuilder()
            .WithName($"{Name}_mirrored")
            .WithPosition(newX, newY)
            .WithType(Type)
            .Build();
    }
}