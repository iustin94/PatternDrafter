namespace PatternDrafter.Library;

public class Pattern
{
    public string Name { get; }
    public List<PatternPiece> Pieces { get; } = new List<PatternPiece>();

    public Pattern(string name)
    {
        Name = name;
    }

    public void AddPiece(PatternPiece piece)
    {
        Pieces.Add(piece);
    }

        public static PatternPoint.PatternPointBuilder Point() => PatternPoint.Create();

        public static PatternLine.PatternLineBuilder Line() => PatternLine.Create();

        public static PatternCurve.PatternCurveBuilder Curve() => PatternCurve.Create();

        public static PatternPiece.PatternPieceBuilder Piece() => PatternPiece.Create();
} 